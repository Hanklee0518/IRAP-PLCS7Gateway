using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ServiceProcess;
using IRAP.MESGateway.Tools.Utils;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraTreeList;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCServicesTree : BaseControl
    {
        private bool allowDataSourceChanged = false;

        public UCServicesTree()
        {
            InitializeComponent();

            GetServices();

            tlServices.RowHeight =
                Math.Max(
                    Convert.ToInt32(tlServices.Font.GetHeight()),
                    imageCollection.ImageSize.Height) + 4;
        }

        [Browsable(false)]
        public TreeListNode FocusedNode
        {
            get
            {
                return tlServices.FocusedNode;
            }
            set
            {
                SetFocusedNode(value);
            }
        }

        internal event ServiceTreeDataSourceChangedEventHandler OnDataSourceChanged;

        private void SetFocusedColor(Skin skin)
        {
            if ("|Office 2016 Colorful|".IndexOf(skin.Name) > 0)
            {
                tlServices.Appearance.FocusedRow.BackColor =
                    skin.Colors.GetColor("HideSelection");
            }
            else
            {
                tlServices.Appearance.FocusedRow.BackColor =
                    skin.Colors.GetColor("Highlight");
            }
        }

        private void RefreshMenuItem(
            TreeListNode node,
            ServiceTreeDataSourceChangedEventArgs e)
        {
            if (node == null)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.RefreshServiceList].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.StartService].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.StopService].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.ServiceLogReload].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.ViewHistoryLog].Enabled = false;
            }
            else
            {
                if (node.Tag is ServiceEntity serivce)
                {
                    MenuItemHelper.Instance.Buttons[MenuItem.RefreshServiceList].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.StartService].Enabled = serivce.Status == ServiceControllerStatus.Stopped;
                    MenuItemHelper.Instance.Buttons[MenuItem.StopService].Enabled = serivce.Status == ServiceControllerStatus.Running;
                }
                else
                {
                    MenuItemHelper.Instance.Buttons[MenuItem.RefreshServiceList].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.StartService].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.StopService].Enabled = false;
                }
            }
        }

        private void GetServices()
        {
            if (!DesignTimeTools.IsDesignMode)
            {
                allowDataSourceChanged = false;

                List<ServiceEntity> services =
                  ServiceHelper.Instance.GetDCSGatewayServices();
                services.Sort(ServiceEntity.CompareByServiceName);

                tlServices.Nodes.Clear();
                foreach (ServiceEntity service in services)
                {
                    TreeListNode node =
                        tlServices.AppendNode(
                            new object[] {
                            service
                                .ServiceName
                                .Replace(
                                    ServiceHelper.Instance.ServiceCommName,
                                    "") + " DCS 服务"
                            },
                            null);
                    node.ImageIndex = (int)ServicesTreeImageIndex.ServiceImage;
                    node.SelectImageIndex = (int)ServicesTreeImageIndex.ServiceImage;
                    switch (service.Status)
                    {
                        case ServiceControllerStatus.Running:
                            node.StateImageIndex = (int)ServicesTreeImageIndex.Running;
                            break;
                        case ServiceControllerStatus.Stopped:
                            node.StateImageIndex = (int)ServicesTreeImageIndex.Stopped;
                            break;
                    }
                    node.Tag = service;
                }

                allowDataSourceChanged = true;
            }
        }

        private void RaiseDataSourceChanged(TreeListNode node)
        {
            if (OnDataSourceChanged != null && allowDataSourceChanged)
            {
                ServiceTreeDataSourceChangedEventArgs args;
                if (node == null || node.Tag == null)
                {
                    args = new ServiceTreeDataSourceChangedEventArgs(null);
                }
                else
                {
                    args =
                        new ServiceTreeDataSourceChangedEventArgs(
                            node.Tag as ServiceEntity);
                }

                OnDataSourceChanged(tlServices, args);
                RefreshMenuItem(node, args);
            }
        }

        private void SetFocusedNode(string text)
        {
            if (tlServices.Nodes.Count <= 0)
            {
                return;
            }

            if (text != "")
            {
                foreach (TreeListNode node in tlServices.Nodes)
                {
                    if (node.GetDisplayText(tlclmServiceName) == text)
                    {
                        SetFocusedNode(node);
                        return;
                    }
                }
            }
            SetFocusedNode(tlServices.Nodes[0]);
        }

        private void SetFocusedNode(TreeListNode node)
        {
            if (tlServices.FocusedNode == null || node == null)
            {
                return;
            }

            bool needRefresh = tlServices.FocusedNode == node;
            tlServices.FocusedNode = node;
            if (needRefresh)
            {
                if (node.Tag is ServiceEntity service)
                {
                    switch (service.Status)
                    {
                        case ServiceControllerStatus.Stopped:
                            node.StateImageIndex = (int)ServicesTreeImageIndex.Stopped;
                            break;
                        case ServiceControllerStatus.Running:
                            node.StateImageIndex = (int)ServicesTreeImageIndex.Running;
                            break;
                    }
                }

                tlServices_FocusedNodeChanged(
                    tlServices,
                    new FocusedNodeChangedEventArgs(null, node));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignTimeTools.IsDesignMode)
            {
                if (tlServices.Nodes.Count > 0)
                {
                    SetFocusedNode(tlServices.Nodes[0]);
                }
            }
        }

        protected override void LookAndFeelStyleChanged()
        {
            base.LookAndFeelStyleChanged();

            Color controlColor =
                CommonSkins.GetSkin(UserLookAndFeel.Default)
                    .Colors.GetColor("Control");
            tlServices.Appearance.Empty.BackColor = controlColor;
            tlServices.Appearance.Row.BackColor = controlColor;
            SetFocusedColor(CommonSkins.GetSkin(UserLookAndFeel.Default));
        }

        private void tlServices_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            RaiseDataSourceChanged(e.Node);
        }

        private void UCServicesTree_Enter(object sender, EventArgs e)
        {
            string nodeName = "";
            if (tlServices.FocusedNode != null)
            {
                nodeName = tlServices.FocusedNode.GetDisplayText(tlclmServiceName);
            }

            GetServices();
            SetFocusedNode(nodeName);
        }

        public void RefreshServices()
        {
            UCServicesTree_Enter(null, null);
        }
    }

    internal enum ServicesTreeImageIndex
    {
        ServiceImage = 0,
        Running,
        Stopped
    }
}
