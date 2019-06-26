using IRAP.BL.S7Gateway.Enums;
using IRAP.BL.S7Gateway.Utils;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// 所有Product类的父类
    /// </summary>
    public class CustomProduct : IRAPBaseObject
    {
    }

    /// <summary>
    /// PLC 父类
    /// </summary>
    public abstract class CustomPLC : CustomProduct
    {
        /// <summary>
        /// 设备清单
        /// </summary>
        protected Dictionary<string, CustomDevice> _devices =
            new Dictionary<string, CustomDevice>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="node">PLC 对象属性的 Xml 节点</param>
        public CustomPLC(XmlNode node)
        {
            _log = Logger.Get<CustomPLC>();
        }
    }

    /// <summary>
    /// 设备父类
    /// </summary>
    public abstract class CustomDevice : CustomProduct
    {
        /// <summary>
        /// 当前设备的TagGroup集合，该集合对象延迟到派生类中生成
        /// </summary>
        protected CustomTagGroupCollection _groups = null;
        /// <summary>
        /// 最近一次收到的数据块内容的哈希值
        /// </summary>
        private byte[] hashBytes;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">设备所属PLC对象</param>
        /// <param name="node">设备属性Xml节点</param>
        public CustomDevice(CustomPLC parent, XmlNode node)
        {
            if (parent == null)
            {
                throw new Exception("没有设备所属的PLC对象，不能创建设备");
            }

            InitComponents();
        }

        /// <summary>
        /// 设备所属PLC对象
        /// </summary>
        public CustomPLC Parent { get; protected set; } = null;

        /// <summary>
        /// 初始化字段
        /// </summary>
        /// <remarks>延迟到派生类中进行初始化</remarks>
        public abstract void InitComponents();

        /// <summary>
        /// 收到数据块数据的后续处理
        /// </summary>
        /// <param name="buffer">数据</param>
        public void DoSomething(byte[] buffer)
        {
            byte[] newHashBytes = Tools.CalculateHash(buffer);
            if (Tools.ByteEquals(hashBytes, newHashBytes))
            {
                ;
            }
            else
            {
                hashBytes = newHashBytes;

                _log.Trace($"Buffer Size={buffer.Length}");
                string tmp = "";
                for (int i = 0; i < buffer.Length; i++)
                {
                    tmp += $"{string.Format("{0:x2}", buffer[i])} ";
                }
                _log.Trace(tmp);

                DBDataChanged(buffer);
            }
        }

        /// <summary>
        /// 数据块内容发生变化的后续处理
        /// </summary>
        /// <param name="buffer">数据块内容</param>
        public abstract void DBDataChanged(byte[] buffer);
    }

    /// <summary>
    /// TagGroup父类，其子类有CustomTagGroup，CustomSubTagGroup
    /// </summary>
    public abstract class CustomGroup : CustomProduct
    {
        /// <summary>
        /// Tag集合，该集合对象延迟到派生类中初始化
        /// </summary>
        protected CustomTagCollection _tags;
    }

    /// <summary>
    /// 设备 PLC 的 TagGroup 父类
    /// </summary>
    public abstract class CustomTagGroup : CustomGroup
    {
        /// <summary>
        /// 子TagGroup集合
        /// </summary>
        protected CustomSubTagGroupCollection _groups;
        /// <summary>
        /// 所属Device对象
        /// </summary>
        protected CustomDevice _parent;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">TagGroup对象所依赖的Device对象</param>
        /// <param name="node">TagGroup对象属性的Xml节点</param>
        public CustomTagGroup(CustomDevice parent, XmlNode node)
        {
            if (node.Name.ToUpper() != "TAGGROUP")
            {
                throw new Exception($"传入的Xml节点不是[TagGroup]，当前收到的Xml节点是:[{node.Name}]");
            }

            _parent = parent ?? throw new Exception("传入的Device对象是null，TagGroup必须依赖于Device对象");

            if (node.Attributes["Name"] == null)
            {
                throw new Exception("传入的Xml节点没有[Name]属性，请注意大小写");
            }
            Name = node.Attributes["Name"].Value;
        }

        /// <summary>
        /// TagGroup名称
        /// </summary>
        public string Name { get; protected set; }
    }

    /// <summary>
    /// TagGroup集合父类
    /// </summary>
    public abstract class CustomTagGroupCollection : CustomProduct
    {
        /// <summary>
        /// CustomTagGroup集合
        /// </summary>
        protected Dictionary<string, CustomTagGroup> _groups =
            new Dictionary<string, CustomTagGroup>();
        /// <summary>
        /// CustomTagGroupCollection对象所属的CustomDevic对象
        /// </summary>
        protected CustomDevice _parent;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">TagGroup集合隶属的CustomDevice对象</param>
        public CustomTagGroupCollection(CustomDevice parent)
        {
            _parent = parent ?? throw new Exception("传入的parent对象是null，TagGroupCollection必须依赖于Device对象");
        }

        /// <summary>
        /// 根据索引序号获取集合中的CustomTagGroup对象
        /// </summary>
        /// <param name="index">索引序号</param>
        public CustomTagGroup this[int index]
        {
            get
            {
                if (index >= 0 && index < _groups.Count)
                {
                    return _groups.ElementAt(index).Value;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 根据TagGroup名称获取集合中的CustomTagGroup对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CustomTagGroup this[string name]
        {
            get
            {
                _groups.TryGetValue(name, out CustomTagGroup rlt);
                return rlt;
            }
        }

        /// <summary>
        /// 增加TagGroup对象
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        public abstract void Add(CustomTagGroup group);
    }

    /// <summary>
    /// SubTagGroup类父类
    /// </summary>
    public abstract class CustomSubTagGroup : CustomGroup
    {
        /// <summary>
        /// 所属TagGroup对象
        /// </summary>
        protected CustomTagGroup _parent;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">TagGroup对象</param>
        /// <param name="node">SubTagGroup属性的Xml节点</param>
        public CustomSubTagGroup(CustomTagGroup parent, XmlNode node)
        {
            _parent = parent ?? throw new Exception("传入的parent对象是null，SubTagGroup必须依赖于TagGroup对象");

            if (node == null)
            {
                throw new Exception("node对象不能是空值");
            }
            if (node.Name.ToUpper() != "SUBTAGGROUP")
            {
                throw new Exception($"传入的Xml节点不是[SubTagGroup]，当前收到的Xml节点是:[{node.Name}]");
            }
            if (node.Attributes["Prefix"] == null)
            {
                throw new Exception("传入的Xml节点中没有[Prefix]属性定义，请注意大小写");
            }
            Prefix = node.Attributes["Prefix"].Value;
        }

        /// <summary>
        /// TagName的前缀
        /// </summary>
        public string Prefix { get; protected set; } = "";
    }

    /// <summary>
    /// SubTagGroup对象集合类
    /// </summary>
    public abstract class CustomSubTagGroupCollection : CustomProduct
    {
        /// <summary>
        /// 所属的TagGroup对象
        /// </summary>
        protected CustomTagGroup _parent;
        /// <summary>
        /// SubTagGroup对象集合
        /// </summary>
        protected Dictionary<string, CustomSubTagGroup> _groups;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象，不允许空值</param>
        public CustomSubTagGroupCollection(CustomTagGroup parent)
        {
            _parent = parent ?? throw new Exception("parent参数不允许空值！");
            _groups = new Dictionary<string, CustomSubTagGroup>();
        }

        /// <summary>
        /// 根据索引序号获取集合中的CustomTagGroup对象
        /// </summary>
        /// <param name="index">索引序号</param>
        public CustomSubTagGroup this[int index]
        {
            get
            {
                if (index >= 0 && index < _groups.Count)
                {
                    return _groups.ElementAt(index).Value;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 根据TagGroup名称获取集合中的CustomTagGroup对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CustomSubTagGroup this[string name]
        {
            get
            {
                _groups.TryGetValue(name, out CustomSubTagGroup rlt);
                return rlt;
            }
        }

        /// <summary>
        /// 增加SubTagGroup对象
        /// </summary>
        /// <param name="group">SubTagGroup对象</param>
        public abstract void Add(CustomSubTagGroup group);
    }

    /// <summary>
    /// 设备PLC的Tag父类
    /// </summary>
    public abstract class CustomTag : CustomProduct
    {
        /// <summary>
        /// 所属的TagGroup对象
        /// </summary>
        protected CustomGroup _parent;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public CustomTag(CustomGroup parent, XmlNode node)
        {
            _log = Logger.Get<CustomTag>();
            _parent = parent ?? throw new Exception("parent对象不能是空值");

            if (node == null)
            {
                throw new Exception("node对象不能是空值");
            }
            if (node.Name.ToUpper() != "TAG")
            {
                throw new Exception($"Xml节点[{node.Name}]不是\"Tag\"");
            }

            if (node.Attributes["Name"] == null)
            {
                throw new Exception($"{node.Name}节点中未找到[Name]属性，请注意属性名的大小写");
            }
            Name = node.Attributes["Name"].Value;
        }

        /// <summary>
        /// Tag名称
        /// </summary>
        public string Name { get; protected set; } = "";

        /// <summary>
        /// Tag全称
        /// </summary>
        public string FullName { get; protected set; } = "";

        /// <summary>
        /// 数据块中的地址偏移量
        /// </summary>
        public uint DB_Offset { get; protected set; } = 0;
    }

    /// <summary>
    /// Tag对象集合类
    /// </summary>
    public abstract class CustomTagCollection : CustomProduct
    {
        /// <summary>
        /// CustomTag集合
        /// </summary>
        protected Dictionary<string, CustomTag> _tags = new Dictionary<string, CustomTag>();
        /// <summary>
        /// Tag集合隶属的TagGroup对象，可能是SubTagGroup，可能是TagGroup
        /// </summary>
        protected CustomGroup _parent;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag集合隶属的CustomGroup对象</param>
        public CustomTagCollection(CustomGroup parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// 增加Tag对象
        /// </summary>
        /// <param name="tag">Tag对象</param>
        public abstract void Add(CustomTag tag);
    }
}