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
    internal class GroupEntity : BaseEntity
    {
        private string name = "";

        public GroupEntity(DeviceEntity parent)
        {
            Parent =
                parent ??
                throw new Exception(
                    "标记组对象不能单独存在，必须要依赖于DeviceEntity对象");
            name = "NewGroup";
        }
        public GroupEntity(
            XmlNode node,
            DeviceEntity parent) :
            this(parent)
        {
            #region 从Xml节点属性中获取属性值
            if (node.Attributes["Name"] != null)
            {
                name = node.Attributes["Name"].Value;
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
                else if (childNode.Name == "SubTagGroup")
                {
                    SubGroupEntity subGroup = new SubGroupEntity(childNode, this);
                    SubGroups.Add(subGroup);
                }

                childNode = childNode.NextSibling;
            }
        }

        [Category("设计"), Description("标记组名称"), DisplayName("标记组名称")]
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

        [Browsable(false)]
        public TagEntityCollection Tags { get; } = new TagEntityCollection();
        [Browsable(false)]
        public SubGroupEntityCollection SubGroups { get; } = new SubGroupEntityCollection();
        [Browsable(false)]
        public DeviceEntity Parent { get; private set; } = null;
        [Browsable(false)]
        public TreeListNode Node { get; set; } = null;

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode node = xml.CreateElement("TagGroup");
            node.Attributes.Append(XMLHelper.CreateAttribute(xml, "Name", Name));

            foreach (TagEntity tag in Tags)
            {
                node.AppendChild(xml.ImportNode(tag.GenerateXmlNode(), true));
            }
            foreach (SubGroupEntity sgroup in SubGroups)
            {
                node.AppendChild(xml.ImportNode(sgroup.GenerateXmlNode(), true));
            }

            return node;
        }

        public void RemoveChildren()
        {
            for (int i = Tags.Count - 1; i >= 0; i--)
            {
                Tags.Remove(Tags[i]);
            }

            for (int i = SubGroups.Count - 1; i >= 0; i--)
            {
                SubGroups.Remove(SubGroups[i]);
            }
        }
    }

    internal class GroupEntityCollection : IEnumerable
    {
        private Dictionary<Guid, GroupEntity> groups =
            new Dictionary<Guid, GroupEntity>();

        public GroupEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < groups.Count)
                {
                    return groups.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public GroupEntity this[Guid key]
        {
            get
            {
                groups.TryGetValue(key, out GroupEntity rlt);
                return rlt;
            }
        }

        public int Count { get { return groups.Count; } }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (GroupEntity group in groups.Values)
            {
                yield return group;
            }
        }

        public void Add(GroupEntity group)
        {
            if (groups.ContainsKey(group.ID))
            {
                throw new Exception("已经存在相同的标记组");
            }

            groups.Add(group.ID, group);
            DataHelper.Instance.AllEntities.Add(group);
        }

        public void Remove(GroupEntity group)
        {
            group.RemoveChildren();
            groups.Remove(group.ID);
            DataHelper.Instance.AllEntities.Remove(group.ID);
        }
    }
}
