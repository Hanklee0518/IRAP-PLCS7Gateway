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
    /// 子组
    /// </summary>
    internal class SubGroupEntity : BaseEntity
    {
        private string prefix = "";

        public SubGroupEntity(GroupEntity parent)
        {
            Parent =
                parent ??
                throw new Exception(
                    "标记组对象不能单独存在，必须要依赖于DeviceEntity对象");
        }
        public SubGroupEntity(XmlNode node, GroupEntity parent) : this(parent)
        {
            #region 从Xml节点属性中获取属性值
            if (node.Attributes["Prefix"] != null)
            {
                prefix = node.Attributes["Prefix"].Value;
            }
            #endregion

            XmlNode childNode = node.FirstChild;
            while (childNode != null)
            {
                if (childNode.Name == "Tag")
                {
                    TagEntity tag = new TagEntity(childNode, this);
                    Tags.Add(tag);
                }

                childNode = childNode.NextSibling;
            }
        }

        [Category("设计"), Description("前缀"), DisplayName("前缀")]
        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                if (Node != null)
                {
                    Node.SetValue(0, value);
                    Node.TreeList.BestFitColumns();
                }
            }
        }

        [Browsable(false)]
        public GroupEntity Parent { get; private set; } = null;
        [Browsable(false)]
        public TagEntityCollection Tags { get; } =
            new TagEntityCollection();
        [Browsable(false)]
        public TreeListNode Node { get; set; } = null;

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("SubTagGroup");
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Prefix", prefix));

            foreach (TagEntity tag in Tags)
            {
                node.AppendChild(xml.ImportNode(tag.GenerateXmlNode(), true));
            }

            return node;
        }

        public void RemoveChildren()
        {
            for (int i = Tags.Count - 1; i >= 0; i--)
            {
                Tags.Remove(Tags[i]);
            }
        }
    }

    internal class SubGroupEntityCollection : IEnumerable
    {
        private Dictionary<Guid, SubGroupEntity> sGroups =
            new Dictionary<Guid, SubGroupEntity>();

        public SubGroupEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < sGroups.Count)
                {
                    return sGroups.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public SubGroupEntity this[Guid key]
        {
            get
            {
                sGroups.TryGetValue(key, out SubGroupEntity rlt);
                return rlt;
            }
        }
        public int Count
        {
            get
            {
                return sGroups.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (SubGroupEntity sGroup in sGroups.Values)
            {
                yield return sGroup;
            }
        }

        public void Add(SubGroupEntity sGroup)
        {
            if (sGroups.ContainsKey(sGroup.ID))
            {
                throw new Exception("已经存在相同的标记子组");
            }

            sGroups.Add(sGroup.ID, sGroup);
            DataHelper.Instance.AllEntities.Add(sGroup);
        }

        public void Remove(SubGroupEntity sGroup)
        {
            sGroup.RemoveChildren();
            sGroups.Remove(sGroup.ID);
            DataHelper.Instance.AllEntities.Remove(sGroup.ID);
        }
    }
}
