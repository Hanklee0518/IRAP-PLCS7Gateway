using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using IRAP.MESGateway.Tools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IRAP.MESGateway.Tools.Entities
{
    /// <summary>
    /// 产线
    /// </summary>
    internal class ProductionLineEntity : BaseEntity
    {
        private event AddToWholeEntityQueueHandler _addToWholeEntityQueueHandler;

        public ProductionLineEntity() { }
        public ProductionLineEntity(
            XmlNode node,
            AddToWholeEntityQueueHandler addToWholeEntityQueueHandler)
        {
            _addToWholeEntityQueueHandler = addToWholeEntityQueueHandler;

            #region 从Xml节点属性中获取属性值
            if (node.Attributes["Name"] != null)
            {
                Name = node.Attributes["Name"].Value;
            }
            #endregion

            XmlNode deviceNode = node.FirstChild;
            while (deviceNode != null)
            {
                if (deviceNode.Name.ToLower() == "device")
                {
                    try
                    {
                        DeviceEntity device =
                            new DeviceEntity(
                                deviceNode,
                                this,
                                addToWholeEntityQueueHandler);
                        if (device != null)
                        {
                            Devices.Add(device, addToWholeEntityQueueHandler);
                        }
                    }
                    catch (Exception error)
                    {
                        XtraMessageBox.Show(
                            error.Message,
                            "出错啦",
                            MessageBoxButtons.OK);
                    }
                }

                deviceNode = deviceNode.NextSibling;
            }
        }

        private string name = "";

        /// <summary>
        /// 产线名称
        /// </summary>
        [Category("设计"), Description("产线名称"), DisplayName("产线名称")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (Node != null)
                {
                    Node.SetValue(0, value);
                    Node.TreeList.BestFitColumns();
                }
            }
        }
        /// <summary>
        /// 产线所属设备集合
        /// </summary>
        [Browsable(false)]
        public DeviceEntityCollection Devices { get; } =
            new DeviceEntityCollection();
        [Browsable(false)]
        public TreeListNode Node { get; set; }

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("ProductionLine");
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", Name));

            foreach (DeviceEntity device in Devices)
            {
                node.AppendChild(xml.ImportNode(device.GenerateXmlNode(), true));
            }

            return node;
        }

        public void RemoveChildren()
        {
            for (int i = Devices.Count - 1; i >= Devices.Count; i--)
            {
                Devices.Remove(Devices[i]);
            }
        }

        public List<DeviceEntity> ImportDevice(string path)
        {
            List<DeviceEntity> rlt = new List<DeviceEntity>();

            if (path == "")
            {
                return rlt;
            }

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
            }
            catch (Exception error)
            {
                throw new Exception(
                    $"解析[{Path.GetFileName(path)}]时出错：[{error.Message}]");
            }

            XmlNode root = xml.SelectSingleNode("root");
            if (root == null)
            {
                throw new Exception("配置文件中没有 root 根节点");
            }

            PLCType plcType = PLCType.SIEMENS;
            XmlNode plcNode = root.FirstChild;
            while (plcNode != null)
            {
                if (plcNode.Name.ToUpper() == "SIEMENSPLC")
                {
                    plcType = PLCType.SIEMENS;
                    PLCEntity plcEntity = new PLCEntity()
                    {
                        IPAddress = XMLHelper.GetAttributeStringValue(plcNode, "IPAddress", "127.0.0.1"),
                        Rack = XMLHelper.GetAttributeIntValue(plcNode, "Rack", 0),
                        Slot = XMLHelper.GetAttributeIntValue(plcNode, "Slot", 0),
                    };

                    XmlNode deviceNode = plcNode.FirstChild;
                    while (deviceNode != null)
                    {
                        DeviceEntity device =
                            DeviceEntity.ImportFromXmlNode(
                                this,
                                deviceNode,
                                plcType,
                                plcEntity);
                        if (device != null)
                        {
                            Devices.Add(device, _addToWholeEntityQueueHandler);
                            rlt.Add(device);
                        }

                        deviceNode = deviceNode.NextSibling;
                    }
                }

                plcNode = plcNode.NextSibling;
            }

            return rlt;
        }
    }

    internal class ProductionLineEntityCollection : IEnumerable
    {
        private Dictionary<Guid, ProductionLineEntity> lines =
            new Dictionary<Guid, ProductionLineEntity>();

        public ProductionLineEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < lines.Count)
                {
                    return lines.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public ProductionLineEntity this[Guid key]
        {
            get
            {
                lines.TryGetValue(key, out ProductionLineEntity rlt);
                return rlt;
            }
        }
        public int Count
        {
            get { return lines.Count; }
        }

        public void Clear()
        {
            lines.Clear();
        }

        public void Add(
            ProductionLineEntity line,
            AddToWholeEntityQueueHandler addToWholeEntityQueueHandler)
        {
            if (this[line.ID] != null)
            {
                throw new Exception($"已经存在[{line.Name}]的产线");
            }
            else
            {
                lines.Add(line.ID, line);
                addToWholeEntityQueueHandler?.Invoke(line);
            }
        }

        public void Remove(ProductionLineEntity line)
        {
            line.RemoveChildren();
            lines.Remove(line.ID);
            DataHelper.Instance.AllEntities.Remove(line.ID);
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                yield return lines.ElementAt(i).Value;
            }
        }
    }
}
