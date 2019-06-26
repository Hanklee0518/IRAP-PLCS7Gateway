using IRAP.BL.S7Gateway.Entities;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// PLC 对象容器，该容器对象只允许存在一个
    /// </summary>
    public class PLCContainer : IRAPBaseObject
    {
        /// <summary>
        /// 唯一对象实例
        /// </summary>
        private static PLCContainer _instance = null;
        /// <summary>
        /// PLC对象列表
        /// </summary>
        private List<CustomPLC> _plcs = new List<CustomPLC>();

        private PLCContainer()
        {
            _log = Logger.Get<PLCContainer>();
            _log.Info("创建 PLC 对象容器");
        }

        /// <summary>
        /// 唯一对象实例的引用
        /// </summary>
        public static PLCContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PLCContainer();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 从Xml配置文件中加载PLC设置
        /// </summary>
        /// <param name="fileName">Xml配置文件名</param>
        /// <returns>True: 加载成功；False: 在加载过程中出现错误</returns>
        public bool LoadFromXml(string fileName)
        {
            _log.Info($"加载配置文件[{fileName}]中的PLC配置信息");

            try
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(fileName);
                }
                catch (Exception error)
                {
                    throw new Exception(
                        $"读取并解析配置文件[{fileName}]时出错：[{error.Message}]");
                }

                XmlNode root = xml.SelectSingleNode("root");
                if (root != null && root.HasChildNodes)
                {
                    XmlNode plcNode = root.FirstChild;
                    while (plcNode != null)
                    {
                        try
                        {
                            if (plcNode.Name.ToUpper() == "SIEMENSPLC")
                            {
                                _log.Trace(plcNode.Name);
                                _plcs.Add(new SiemensPLCCreator().CreateProduct(plcNode));
                            }
                        }
                        catch (Exception error)
                        {
                            _log.Error(error.Message, error);
                        }

                        plcNode = plcNode.NextSibling;
                    }
                }
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
                return false;
            }

            _log.Info("加载完成");
            return true;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            foreach (CustomPLC plc in _plcs)
            {
                if (plc is SiemensPLC)
                {
                    ((SiemensPLC)plc).Run();
                }
            }
        }
    }
}