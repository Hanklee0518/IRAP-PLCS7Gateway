using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using IRAP.MESGateway.Tools.Controls;
using IRAP.MESGateway.Tools.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IRAP.MESGateway.Tools
{
    public class ParamHelper
    {
        private static ParamHelper _instance = null;
        public static ParamHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ParamHelper();
                }
                return _instance;
            }
        }

        private ParamHelper()
        {
            RefreshBaseServiceVersion();
        }

        #region 属性
        internal string[] ApplicationArguments;
        internal readonly string AppTitle =
            "IRAP DCSGateway for PLC 维护管理工具";

        internal string ProjectBasePath
        {
            get => GetString("ProjectPath");
            set => SaveParam("ProjectPath", value);
        }
        internal int CommunityID
        {
            get => GetInt("CommunityID", 57280);
            set => SaveParam("CommunityID", value.ToString());
        }
        internal string ServiceExecuteName
        {
            get => "IRAP.DCSGateway.Service.exe";
        }
        internal string WebAPIUrl
        {
            get => GetString("WebAPIUrl");
            set => SaveParam("WebAPIUrl", value);
        }
        internal Version BaseServiceVersion { get; private set; }
        internal string ServPackagePath
        {
            get => Path.Combine(ServPackageDirectory, ServiceExecuteName);
        }
        internal string ServPackageDirectory
        {
            get => Path.Combine(Application.StartupPath, "BaseService");
        }
        #endregion

        #region 私有方法
        private void SaveParam(string key, string value)
        {
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private string GetString(string key)
        {
            string rlt = "";
            if (ConfigurationManager.AppSettings[key] != null)
            {
                rlt = ConfigurationManager.AppSettings[key];
            }
            return rlt;
        }

        private int GetInt(string key, int defaultValue)
        {
            int rlt = defaultValue;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                if (int.TryParse(ConfigurationManager.AppSettings[key], out int value))
                {
                    rlt = value;
                }
            }
            return rlt;
        }

        private bool GetBoolean(string key)
        {
            bool rlt = false;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                try { rlt = Convert.ToBoolean(ConfigurationManager.AppSettings[key]); }
                catch { rlt = false; }
            }
            return rlt;
        }
        #endregion

        #region 公开方法
        public string GenerateSerivcePath(string lineName, string deviceName)
        {
            return $@"{ProjectBasePath}\{lineName}\{deviceName}\{ServiceExecuteName}";
        }

        public void RefreshBaseServiceVersion()
        {
            if (File.Exists(ServPackagePath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(ServPackagePath);
                BaseServiceVersion =
                    new Version(
                        versionInfo.FileMajorPart,
                        versionInfo.FileMinorPart,
                        versionInfo.FileBuildPart,
                        versionInfo.FilePrivatePart);
            }
            else
            {
                BaseServiceVersion = new Version(0, 0, 0, 0);
            }
        }
        #endregion
    }

    internal class DataHelper
    {
        private static DataHelper _instance = null;
        public static DataHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataHelper();
                }
                return _instance;
            }
        }

        public BaseEntityCollection AllEntities { get; } =
            new BaseEntityCollection();

        public ProductionLineEntityCollection Lines { get; } =
            new ProductionLineEntityCollection();

        private DataHelper() { }

        private void AddBaseEntity(BaseEntity entity)
        {
            AllEntities.Add(entity);
        }

        public int Load(string path)
        {
            Lines.Clear();

            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(path);
                }
                catch (Exception error)
                {
                    XtraMessageBox.Show(
                        error.Message,
                        "出错啦",
                        MessageBoxButtons.OK);
                    return 0;
                }

                XmlNode root = xml.SelectSingleNode("root");
                if (root != null && root.HasChildNodes)
                {
                    XmlNode lineNode = root.FirstChild;
                    while (lineNode != null)
                    {
                        try
                        {
                            ProductionLineEntity pline =
                                new ProductionLineEntity(
                                    lineNode,
                                    AddBaseEntity);
                            if (pline != null)
                            {
                                Lines.Add(pline, AddBaseEntity);
                            }
                        }
                        catch (Exception error)
                        {
                            XtraMessageBox.Show(
                                error.Message,
                                "出错啦",
                                MessageBoxButtons.OK);
                        }

                        lineNode = lineNode.NextSibling;
                    }
                }
            }

            return Lines.Count;
        }

        public void Save(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", "no"));

            XmlNode root = xml.CreateElement("root");
            xml.AppendChild(root);

            foreach (ProductionLineEntity line in Lines)
            {
                root.AppendChild(xml.ImportNode(line.GenerateXmlNode(), true));
            }

            string bakpath = path.Replace(".xml", ".bak");
            if (File.Exists(bakpath))
            {
                File.Delete(bakpath);
            }
            File.Move(path, bakpath);

            xml.Save(path);
        }
    }

    internal class MenuItemHelper
    {
        private static MenuItemHelper _instance = null;
        public static MenuItemHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MenuItemHelper();
                }
                return _instance;
            }
        }
        private MenuItemHelper() { }

        public BarItemCollection Buttons { get; } =
            new BarItemCollection();
    }

    internal class BarItemCollection
    {
        private Dictionary<MenuItem, BarItem> buttons =
            new Dictionary<MenuItem, BarItem>();

        public BarItem this[int index]
        {
            get
            {
                if (index >= 0 && index < buttons.Count)
                {
                    return buttons.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public BarItem this[MenuItem key]
        {
            get
            {
                buttons.TryGetValue(key, out BarItem rlt);
                return rlt;
            }
        }
        public int Count { get { return buttons.Count; } }

        public void Add(BarItem item)
        {
            MenuItem key = (MenuItem)item.Tag;
            if (!buttons.ContainsKey(key))
            {
                buttons.Add(key, item);
            }
        }
    }
}
