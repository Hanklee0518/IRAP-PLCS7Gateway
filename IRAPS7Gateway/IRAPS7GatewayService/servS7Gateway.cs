using IRAP.BL.S7Gateway;
using Logrila.Logging.NLogIntegration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IRAPS7GatewayService
{
    public partial class servS7Gateway : ServiceBase
    {
        public servS7Gateway()
        {
            InitializeComponent();

            NLogLogger.Use();
        }

        protected override void OnStart(string[] args)
        {
            string deviceCFGFileName = "Devices.xml";
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Substring(0, 5).ToLower() == "-file:")
                    {
                        deviceCFGFileName = args[i].Substring(6, args[i].Length - 6);
                    }
                }
            }
            else
            {
                if (ConfigurationManager.AppSettings["DeviceName"] != null)
                {
                    string strTemp = ConfigurationManager.AppSettings["DeviceName"];
                    if (strTemp != "")
                    {
                        deviceCFGFileName = $"{strTemp}.xml";
                    }
                }
            }

            if (!File.Exists(deviceCFGFileName))
            {
                deviceCFGFileName =
                    $"{Path.GetDirectoryName(GetType().Assembly.Location)}" +
                    $"\\{Path.GetFileName(deviceCFGFileName)}";
                if (!File.Exists(deviceCFGFileName))
                {
                    throw new FileNotFoundException($"找不到指定的配置文件", deviceCFGFileName);
                }
            }

            try
            {
                if (PLCContainer.Instance.LoadFromXml(deviceCFGFileName))
                {
                    PLCContainer.Instance.Run();
                }
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        protected override void OnStop()
        {
            PLCContainer.Instance.Stop();
        }
    }
}
