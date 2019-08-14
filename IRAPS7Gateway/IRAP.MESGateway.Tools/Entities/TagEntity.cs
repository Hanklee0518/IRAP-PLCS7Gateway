using DevExpress.XtraTreeList.Nodes;
using IRAP.MESGateway.Tools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAP.MESGateway.Tools.Entities
{
    /// <summary>
    /// 标签
    /// </summary>
    internal class TagEntity : BaseEntity
    {
        private string name = "Tag";
        private TagDataType dataType = TagDataType.Bool;
        private TagType type = TagType.A;
        private string offset = "";
        private string length = "0.1";

        public TagEntity(GroupEntity parent)
        {
            GroupParent =
                parent ??
                throw new Exception(
                    "标记对象不能单独存在，必须依赖GroupEntity/SubGroupEntity对象");
        }
        public TagEntity(SubGroupEntity parent)
        {
            SubGroupParent =
                parent ??
                throw new Exception(
                    "标记对象不能单独存在，必须依赖GroupEntity/SubGroupEntity对象");
        }
        public TagEntity(
            XmlNode node,
            GroupEntity parent) :
            this(parent)
        {
            InitTagValue(node);
        }
        public TagEntity(XmlNode node, SubGroupEntity parent) : this(parent)
        {
            InitTagValue(node);
        }

        [Category("设计"), Description("标记名称"), DisplayName("标记名称")]
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
        [Category("设计"), Description("数据类型"), DisplayName("数据类型")]
        public TagDataType DataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
                switch (value)
                {
                    case TagDataType.Bool:
                        length = "0.1";
                        break;
                    case TagDataType.Byte:
                        length = "1";
                        break;
                    case TagDataType.Word:
                        length = "2";
                        break;
                    case TagDataType.Int:
                        length = "2";
                        break;
                    case TagDataType.DWord:
                        length = "4";
                        break;
                    case TagDataType.Real:
                        length = "4";
                        break;
                    case TagDataType.ArrayChar:
                        length = "0";
                        break;
                }

                if (Node != null)
                {
                    Node.SetValue(2, EnumHelper.GetEnumDescription(value));
                    Node.SetValue(5, length);
                }
            }
        }
        [Category("设计"), Description("类型"), DisplayName("类型")]
        public TagType Type
        {
            get { return type; }
            set
            {
                type = value;
                if (Node != null)
                {
                    Node.SetValue(3, EnumHelper.GetEnumDescription(value));
                }
            }
        }
        [Category("设计"), Description("偏移量"), DisplayName("偏移量")]
        public string Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                if (Node != null)
                {
                    Node.SetValue(4, value);
                }
            }
        }
        [Category("设计"), Description("长度"), DisplayName("长度")]
        public string Length
        {
            get { return length; }
            set
            {
                if (DataType == TagDataType.ArrayChar)
                {
                    length = value;
                }
                if (Node != null)
                {
                    Node.SetValue(5, length);
                }
            }
        }

        [Browsable(false)]
        public GroupEntity GroupParent
        {
            get; private set;
        } = null;
        [Browsable(false)]
        public SubGroupEntity SubGroupParent { get; private set; } = null;
        [Browsable(false)]
        public TreeListNode Node { get; set; } = null;

        private void InitTagValue(XmlNode node)
        {
            #region 从Xml节点属性中获取属性值
            if (node.Attributes["Name"] != null)
            {
                name = node.Attributes["Name"].Value;
            }
            if (node.Attributes["DataType"] != null)
            {
                try
                {
                    DataType =
                        (TagDataType)Enum.Parse(
                            typeof(TagDataType),
                            node.Attributes["DataType"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(TagDataType)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[DataType]属性值错误，只支持[{enumValues}]");
                }
            }
            if (node.Attributes["Type"] != null)
            {
                try
                {
                    Type =
                        (TagType)Enum.Parse(
                            typeof(TagType),
                            node.Attributes["Type"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(TagType)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[Type]属性值错误，只支持[{enumValues}]");
                }
            }
            if (node.Attributes["Offset"] != null)
            {
                Offset = node.Attributes["Offset"].Value;
            }
            if (node.Attributes["Length"] != null)
            {
                length = node.Attributes["Length"].Value;
            }
            #endregion
        }

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("Tag");
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", Name));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "DataType", DataType.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Type", Type.ToString()));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Offset", Offset));
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Length", length));

            return node;
        }
    }

    internal class TagEntityCollection : IEnumerable
    {
        private Dictionary<Guid, TagEntity> tags =
            new Dictionary<Guid, TagEntity>();

        public TagEntityCollection()
        {

        }

        public TagEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < tags.Count)
                {
                    return tags.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public TagEntity this[Guid key]
        {
            get
            {
                tags.TryGetValue(key, out TagEntity rlt);
                return rlt;
            }
        }
        public int Count
        {
            get
            {
                return tags.Count;
            }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (TagEntity tag in tags.Values)
            {
                yield return tag;
            }
        }

        public void Add(TagEntity tag)
        {
            if (tags.ContainsKey(tag.ID))
            {
                throw new Exception("已经存在相同的标记");
            }

            tags.Add(tag.ID, tag);
            DataHelper.Instance.AllEntities.Add(tag);
        }

        public void Remove(TagEntity tag)
        {
            tags.Remove(tag.ID);
            DataHelper.Instance.AllEntities.Remove(tag.ID);
        }
    }
}
