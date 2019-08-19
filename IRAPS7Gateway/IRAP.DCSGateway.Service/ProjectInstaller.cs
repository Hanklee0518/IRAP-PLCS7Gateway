using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace IRAP.DCSGateway.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            serviceInstaller.ServiceName += GetDeviceName();
            serviceInstaller.DisplayName = serviceInstaller.ServiceName;
        }

        private string GetDeviceName()
        {
            string rlt = "未定义设备 ";

            string pathServiceInstalled =
                Path.GetDirectoryName(GetType().Assembly.Location);
            string serviceCFGFileName =
                GetType().Assembly.Location + ".config";

            rlt = Get_ConfigValue(serviceCFGFileName, "DeviceName");
            if (rlt != "")
            {
                return rlt;
            }

            rlt = "未定义设备";
            string fileName = $"{pathServiceInstalled}\\Devices.xml";
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.Load(fileName);
                XmlNode root = xdoc.DocumentElement;
                XmlNode deviceNode = root.SelectSingleNode("SiemensPLC/Device");
                if (deviceNode != null)
                {
                    if (deviceNode.Attributes["Name"] != null)
                    {
                        return deviceNode.Attributes["Name"].Value;
                    }
                    else
                    {
                        return rlt + "";
                    }
                }
                else
                {
                    return rlt + "";
                }
            }
            catch (Exception error)
            {
                return rlt + error.Message;
            }
            finally
            {

            }
        }

        /// <summary>
        /// 从程序配置文件中读取 appSettings 中的配置参数
        /// </summary>
        /// <param name="configPath">配置文件名（包括全路径）</param>
        /// <param name="strKeyName">关键字名称</param>
        /// <returns>关键字值</returns>
        private string Get_ConfigValue(string configPath, string strKeyName)
        {
            if (File.Exists(configPath))
            {
                using (XmlTextReader tr = new XmlTextReader(configPath))
                {
                    while (tr.Read())
                    {
                        if (tr.NodeType == XmlNodeType.Element)
                        {
                            if (tr.Name == "add" && tr.GetAttribute("key") == strKeyName)
                            {
                                return tr.GetAttribute("value");
                            }
                        }
                    }
                }
            }

            return "";
        }
    }
}
