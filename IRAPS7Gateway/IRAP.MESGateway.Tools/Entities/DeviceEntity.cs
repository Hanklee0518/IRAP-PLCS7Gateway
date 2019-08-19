﻿using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using IRAP.MESGateway.Tools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IRAP.MESGateway.Tools.Entities
{
    /// <summary>
    /// 生产设备
    /// </summary>
    internal class DeviceEntity : BaseEntity
    {
        private event AddToWholeEntityQueueHandler addToWholeEntityQueueHandler;

        public DeviceEntity(ProductionLineEntity parent)
        {
            Parent =
                parent ??
                throw new Exception(
                    "设备对象不能单独存在，必须要依赖于ProductLineEntity对象");

            name = $"KnownDevice[{Guid.NewGuid().ToString("N")}]";

            Service = new DCSGatewayServiceController(this);
        }
        public DeviceEntity(
            XmlNode node,
            ProductionLineEntity parent,
            AddToWholeEntityQueueHandler addToWholeEntityQueueHandler) :
            this(parent)
        {
            this.addToWholeEntityQueueHandler = addToWholeEntityQueueHandler;

            #region 从Xml节点属性中获取属性值
            if (node.Attributes["Name"] != null)
            {
                Name = node.Attributes["Name"].Value;
            }
            if (node.Attributes["T133LeafID"] != null)
            {
                if (int.TryParse(node.Attributes["T133LeafID"].Value, out int rlt))
                {
                    T133LeafID = rlt;
                }
            }
            if (node.Attributes["T216LeafID"] != null)
            {
                if (int.TryParse(node.Attributes["T216LeafID"].Value, out int rlt))
                {
                    T216LeafID = rlt;
                }
            }
            if (node.Attributes["T107LeafID"] != null)
            {
                if (int.TryParse(node.Attributes["T107LeafID"].Value, out int rlt))
                {
                    T107LeafID = rlt;
                }
            }
            if (node.Attributes["DBType"] != null)
            {
                try
                {
                    DBType =
                        (SiemensRegisterType)Enum.Parse(
                            typeof(SiemensRegisterType),
                            node.Attributes["DBType"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(SiemensRegisterType)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[DBType]属性值错误，只支持[{enumValues}]");
                }
            }
            if (node.Attributes["DBNumber"] != null)
            {
                if (int.TryParse(node.Attributes["DBNumber"].Value, out int rlt))
                {
                    DBNumber = rlt;
                }
            }
            if (node.Attributes["CycleReadMode"] != null)
            {
                try
                {
                    CycleReadMode =
                        (CycleReadMode)Enum.Parse(
                            typeof(CycleReadMode),
                            node.Attributes["CycleReadMode"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(CycleReadMode)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[CycleReadMode]属性值错误，只支持[{enumValues}]");
                }
            }
            if (node.Attributes["PLCType"] != null)
            {
                try
                {
                    PLCType =
                        (PLCType)Enum.Parse(
                            typeof(PLCType),
                            node.Attributes["PLCType"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(PLCType)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[PLCType]属性值错误，只支持[{enumValues}]");
                }
            }
            if (node.Attributes["SplitterTime"] != null)
            {
                if (int.TryParse(node.Attributes["SplitterTime"].Value, out int rlt))
                {
                    SplitterTime = rlt;
                }
            }
            #endregion

            XmlNode childNode = node.FirstChild;
            while (childNode != null)
            {
                if (childNode.Name == "PLC")
                {
                    BelongPLC.LoadFromXmlNode(childNode);
                }
                else if (childNode.Name == "TagGroup")
                {
                    try
                    {
                        GroupEntity group =
                            new GroupEntity(childNode, this);
                        if (group != null)
                        {
                            Groups.Add(group);
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

                childNode = childNode.NextSibling;
            }
        }

        private string name = "";
        /// <summary>
        /// 设备名称
        /// </summary>
        [Category("设计"), Description("设备名称"), DisplayName("设备名称")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Service.ResetServName();
                if (Node != null)
                {
                    Node.SetValue(0, value);
                    Node.TreeList.BestFitColumns();
                }
            }
        }
        [Category("MES配置数据"), Description("设备叶标识"), DisplayName("设备叶标识")]
        public int T133LeafID { get; set; } = 0;
        [Category("MES配置数据"), Description("工序叶标识"), DisplayName("工序叶标识")]
        public int T216LeafID { get; set; } = 0;
        [Category("MES配置数据"), Description("工位叶标识"), DisplayName("工位叶标识")]
        public int T107LeafID { get; set; } = 0;
        [Category("PLC配置数据"), Description("PLC类型"), DisplayName("PLC类型")]
        public PLCType PLCType { get; set; } = PLCType.SIEMENS;
        [Category("PLC配置数据"), Description("数据块类型"), DisplayName("数据块类型")]
        public SiemensRegisterType DBType { get; set; } = SiemensRegisterType.DB;
        [Category("PLC配置数据"), Description("数据块标识号"), DisplayName("数据块标识号")]
        public int DBNumber { get; set; } = 1;
        [Category("Gateway配置数据"), Description("监控模式"), DisplayName("监控模式")]
        public CycleReadMode CycleReadMode { get; set; } = CycleReadMode.ControlBlock;
        [Category("Gateway配置数据"), Description("循环读取间隔时间(毫秒)"), DisplayName("循环读取间隔时间")]
        public int SplitterTime { get; set; } = 100;
        [Category("PLC配置数据"), Description("PLC连接属性"), DisplayName("PLC连接属性")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public PLCEntity BelongPLC { get; } = new PLCEntity();

        [Browsable(false)]
        public ProductionLineEntity Parent { get; set; } = null;
        [Browsable(false)]
        public GroupEntityCollection Groups { get; } = new GroupEntityCollection();
        [Browsable(false)]
        public TreeListNode Node { get; set; } = null;
        [Browsable(false)]
        public DCSGatewayServiceController Service { get; private set; } = null;

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("Device");
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", Name));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "T133LeafID", T133LeafID.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "T216LeafID", T216LeafID.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "T107LeafID", T107LeafID.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "PLCType", PLCType.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "DBType", DBType.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "DBNumber", DBNumber.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "CycleReadMode", CycleReadMode.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "SplitterTime", SplitterTime.ToString()));

            node.AppendChild(xml.ImportNode(BelongPLC.GenerateXmlNode(), true));
            foreach (GroupEntity group in Groups)
            {
                node.AppendChild(xml.ImportNode(group.GenerateXmlNode(), true));
            }

            return node;
        }

        public void RemoveChildren()
        {
            for (int i = Groups.Count - 1; i >= 0; i--)
            {
                Groups.Remove(Groups[i]);
            }
        }

        public static DeviceEntity ImportFromXmlNode(
            ProductionLineEntity line,
            XmlNode node,
            PLCType plcType,
            PLCEntity plcEntity)
        {
            DeviceEntity rlt = null;
            if (node.Name.ToUpper() != "DEVICE")
            {
                return rlt;
            }

            rlt = new DeviceEntity(line)
            {
                Name = XMLHelper.GetAttributeStringValue(node, "Name", ""),
                PLCType = plcType,
                DBType =
                    (SiemensRegisterType)Enum.Parse(
                        typeof(SiemensRegisterType),
                        XMLHelper.GetAttributeStringValue(node, "DBType", "DB")),
                DBNumber = XMLHelper.GetAttributeIntValue(node, "DBNumber", 0),
                CycleReadMode =
                    (CycleReadMode)Enum.Parse(
                        typeof(CycleReadMode),
                        XMLHelper.GetAttributeStringValue(node, "CycleReadBlock", "ControlBlock")),
                T133LeafID = XMLHelper.GetAttributeIntValue(node, "T133LeafID", 0),
                T216LeafID = XMLHelper.GetAttributeIntValue(node, "T216LeafID", 0),
                T107LeafID = XMLHelper.GetAttributeIntValue(node, "T107LeafID", 0),
                SplitterTime = XMLHelper.GetAttributeIntValue(node, "SplitterTime", 100),
            };
            rlt.BelongPLC.IPAddress = plcEntity.IPAddress;
            rlt.BelongPLC.Rack = plcEntity.Rack;
            rlt.BelongPLC.Slot = plcEntity.Slot;

            XmlNode xmlGroup = node.FirstChild;
            while (xmlGroup != null)
            {
                GroupEntity group =
                    GroupEntity.ImportFromXmlNode(
                        rlt,
                        xmlGroup);
                if (group != null)
                {
                    rlt.Groups.Add(group);
                }

                xmlGroup = xmlGroup.NextSibling;
            }

            return rlt;
        }

        public void ExportToXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", "no"));

            XmlNode root = xml.CreateElement("root");
            xml.AppendChild(root);

            XmlNode plcNode = null;
            if (PLCType == PLCType.SIEMENS)
            {
                plcNode = xml.CreateElement("SiemensPLC");
                plcNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "IPAddress", BelongPLC.IPAddress));
                plcNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Rack", BelongPLC.Rack));
                plcNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Slot", BelongPLC.Slot));
            }
            root.AppendChild(plcNode);

            XmlNode deviceNode = xml.CreateElement("Device");
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", Name));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "T133LeafID", T133LeafID));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "T216LeafID", T216LeafID));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "T107LeafID", T107LeafID));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "DBType", DBType.ToString()));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "DBNumber", DBNumber));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "CycleReadMode", CycleReadMode.ToString()));
            deviceNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "SplitterTime", SplitterTime));

            foreach (GroupEntity group in Groups)
            {
                XmlNode groupNode = xml.CreateElement("TagGroup");
                groupNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", group.Name));

                foreach (TagEntity tag in group.Tags)
                {
                    XmlNode tagNode = xml.CreateElement("Tag");
                    tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", tag.Name));
                    tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Datatype", tag.DataType.ToString()));
                    if (tag.DataType == TagDataType.ArrayChar)
                    {
                        tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Length", tag.Length));
                    }
                    tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Offset", tag.Offset));
                    tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Type", tag.Type.ToString()));
                    groupNode.AppendChild(tagNode);
                }

                foreach (SubGroupEntity sgroup in group.SubGroups)
                {
                    XmlNode sgroupNode = xml.CreateElement("SubTagGroup");
                    sgroupNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Prefix", sgroup.Prefix));
                    
                    foreach (TagEntity tag in sgroup.Tags)
                    {
                        XmlNode tagNode = xml.CreateElement("Tag");
                        tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", tag.Name));
                        tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Datatype", tag.DataType.ToString()));
                        if (tag.DataType == TagDataType.ArrayChar)
                        {
                            tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Length", tag.Length));
                        }
                        tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Offset", tag.Offset));
                        tagNode.Attributes.Append(XMLHelper.CreateAttribute(xml, "Type", tag.Type.ToString()));
                        sgroupNode.AppendChild(tagNode);
                    }

                    groupNode.AppendChild(sgroupNode);
                }

                deviceNode.AppendChild(groupNode);
            }

            plcNode.AppendChild(deviceNode);

            xml.Save(filePath);
        }
    }

    internal class DeviceEntityCollection : IEnumerable
    {
        private Dictionary<Guid, DeviceEntity> devices =
            new Dictionary<Guid, DeviceEntity>();

        public DeviceEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < devices.Count)
                {
                    return devices.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public DeviceEntity this[Guid key]
        {
            get
            {
                devices.TryGetValue(key, out DeviceEntity rlt);
                return rlt;
            }
        }
        public int Count
        {
            get { return devices.Count; }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (DeviceEntity device in devices.Values)
            {
                yield return device;
            }
        }

        public void Add(
            DeviceEntity device,
            AddToWholeEntityQueueHandler addToWholeEntityQueueHandler)
        {
            if (devices.ContainsKey(device.ID))
            {
                throw new Exception("已经相同名称的设备");
            }

            devices.Add(device.ID, device);
            addToWholeEntityQueueHandler?.Invoke(device);
        }

        public void Modify(DeviceEntity device)
        {
            if (!devices.ContainsKey(device.ID))
            {
                throw new Exception($"未找到[{device.Name}]设备");
            }

            devices[device.ID] = device;
        }

        public void Remove(DeviceEntity device)
        {
            device.RemoveChildren();
            devices.Remove(device.ID);
        }

        public List<DeviceEntity> ToList()
        {
            return devices.Values.ToList();
        }
    }
}
