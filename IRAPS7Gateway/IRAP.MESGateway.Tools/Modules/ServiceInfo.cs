using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using IRAP.MESGateway.Tools.Controls;
using IRAP.MESGateway.Tools.Utils;

namespace IRAP.MESGateway.Tools
{
    public partial class ServiceInfo : BaseModule
    {
        private UCServicesTree tree = null;

        public ServiceInfo()
        {
            InitializeComponent();
        }

        internal override void InitModule(IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            if (tree == null)
            {
                tree = data as UCServicesTree;
            }
        }

        protected internal override void ButtonClick(MenuItem menuItemName)
        {
            base.ButtonClick(menuItemName);

            switch (menuItemName)
            {
                case MenuItem.RefreshServiceList:
                    RefreshServiceList();
                    break;
                case MenuItem.StartService:
                    StartService();
                    break;
                case MenuItem.StopService:
                    StopService();
                    break;
                case MenuItem.ServiceLogReload:
                    ReloadServiceLogFile();
                    break;
            }
        }

        private void RefreshServiceList()
        {
            tree.RefreshServices();
        }

        protected internal override void BarListItemClick(MenuItem menuItemName, int index)
        {
            base.BarListItemClick(menuItemName, index);

            switch (menuItemName)
            {
                case MenuItem.ViewHistoryLog:
                    ViewHistoryLog(menuItemName, index);
                    break;
            }
        }

        private void ViewHistoryLog(MenuItem menuItemName, int index)
        {
            if (tree == null || tree.FocusedNode == null)
            {
                return;
            }

            if (MenuItemHelper.Instance.Buttons[MenuItem.ViewHistoryLog] is BarListItem bliFiles)
            {
                if (index >= 0 && index < bliFiles.Strings.Count)
                {
                    if (tree.FocusedNode.Tag is ServiceEntity service)
                    {
                        string servHistoryLogPath =
                            Path.Combine(
                                Path.GetDirectoryName(service.ServiceHomePath),
                                "Logs",
                                bliFiles.Strings[index]);
                        ViewLogFile(servHistoryLogPath);
                    }
                }
            }
        }

        private void StopService()
        {
            TreeListNode node = tree.FocusedNode;
            if (node != null)
            {
                if (node.Tag is ServiceEntity service)
                {
                    if (SplashScreenManager.Default == null)
                    {
                        SplashScreenManager.ShowForm(
                            FindForm(),
                            typeof(Forms.wfMain),
                            false,
                            true);
                    }

                    try
                    {
                        ServiceHelper.Instance.StopService(service.ServiceName);
                        service.Service.Refresh();
                        tree.FocusedNode = node;
                    }
                    finally
                    {
                        if (SplashScreenManager.Default != null)
                        {
                            SplashScreenManager.CloseForm();
                        }
                    }
                }
            }
        }

        private void StartService()
        {
            TreeListNode node = tree.FocusedNode;
            if (node != null)
            {
                if (node.Tag is ServiceEntity service)
                {
                    if (SplashScreenManager.Default == null)
                    {
                        SplashScreenManager.ShowForm(
                            FindForm(),
                            typeof(Forms.wfMain),
                            false,
                            true);
                    }

                    try
                    {
                        ServiceHelper.Instance.StartService(service.ServiceName);
                        service.Service.Refresh();
                        tree.FocusedNode = node;
                    }
                    finally
                    {
                        if (SplashScreenManager.Default != null)
                        {
                            SplashScreenManager.CloseForm();
                        }
                    }
                }
            }
        }

        private void ReloadServiceLogFile()
        {
            if (tree != null && tree.FocusedNode != null)
            {
                if (tree.FocusedNode.Tag is ServiceEntity service)
                {
                    ServiceLogLoadFromFile(service);
                }
            }
        }

        private void ViewLogFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream fs;
                while (true)
                {
                    try
                    {
                        fs = new FileStream(fileName, FileMode.Open);
                        break;
                    }
                    catch { }
                }
                long fileSize = fs.Length;
                byte[] buffer = new byte[fileSize];
                fs.Read(buffer, 0, (int)fileSize);
                fs.Close();
                edtLog.Text =
                    Encoding.Default.GetString(buffer);
                tpLog.Text = $"{Path.GetFileName(fileName)}";
            }
            else
            {
                edtLog.Text = "";
                tpLog.Text = $"{Path.GetFileName(fileName)}[未记录]";
            }
        }

        private void RefreshServiceHistoryLogFiles(ServiceEntity service)
        {
            BarListItem bliLogs =
                MenuItemHelper.Instance.Buttons[MenuItem.ViewHistoryLog] as BarListItem;
            bliLogs.Strings.Clear();

            if (service != null)
            {
                string logPath =
                  Path.Combine(
                      Path.GetDirectoryName(service.ServiceHomePath),
                      "Logs");
                string currengLogPath =
                    Path.Combine(
                        logPath,
                        $"IRAPS7GatewayConsole-{DateTime.Now.ToString("yyyy-MM-dd")}.log");
                if (Directory.Exists(logPath))
                {
                    string[] files = Directory.GetFiles(logPath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].ToUpper() == currengLogPath.ToUpper())
                        {
                            continue;
                        }

                        bliLogs.Strings.Add(Path.GetFileName(files[i]));
                    }
                }
            }

            bliLogs.Enabled = bliLogs.Strings.Count > 0;
        }

        private void ServiceLogLoadFromFile(ServiceEntity service)
        {
            RefreshServiceHistoryLogFiles(service);

            if (service == null)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.ServiceLogReload].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.ViewHistoryLog].Enabled = false;
                edtLog.Text = "";
                tpLog.Text = "";
                return;
            }

            MenuItemHelper.Instance.Buttons[MenuItem.ServiceLogReload].Enabled = true;

            string servDirectory =
                Path.Combine(
                    Path.GetDirectoryName(service.ServiceHomePath),
                    "Logs");

            string servLogPath =
                Path.Combine(
                    servDirectory,
                    $"IRAPS7GatewayConsole-{DateTime.Now.ToString("yyyy-MM-dd")}.log");

            ViewLogFile(servLogPath);
        }

        protected internal override void ShowDataChanged(ServiceTreeDataSourceChangedEventArgs args)
        {
            base.ShowDataChanged(args);

            if (args != null && args.Service != null)
            {
                ServiceLogLoadFromFile(args.Service);
            }
            else
            {
                ServiceLogLoadFromFile(null);
            }
        }
    }
}
