using IRAP.BL.S7Gateway.Enums;
using IRAP.BL.S7Gateway.Utils;
using Logrila.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        /// 最近一次收到的数据块内容的哈希值集合
        /// </summary>
        private Dictionary<string, byte[]> hashBytes =
            new Dictionary<string, byte[]>();

        /// <summary>
        /// 当前设备的TagGroup集合，该集合对象延迟到派生类中生成
        /// </summary>
        protected CustomTagGroupCollection _groups = null;

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

            if (node.Attributes["Name"] == null)
            {
                throw new Exception("传入的Xml节点没有[Name]属性，请注意大小写");
            }
            Name = node.Attributes["Name"].Value;

            if (node.Attributes["T133LeafID"] == null)
            {
                throw new Exception("传入的Xml节点没有[T133LeafID]属性");
            }
            else
            {
                if (int.TryParse(
                    node.Attributes["T133LeafID"].Value,
                    out int rlt))
                {
                    T133LeafID = rlt;
                }
            }

            if (node.Attributes["T216LeafID"] == null)
            {
                throw new Exception("传入的Xml节点没有[T216LeafID]属性");
            }
            else
            {
                if (int.TryParse(
                    node.Attributes["T216LeafID"].Value,
                    out int rlt))
                {
                    T216LeafID = rlt;
                }
            }

            if (node.Attributes["T107LeafID"] != null)
            {
                if (int.TryParse(
                    node.Attributes["T107LeafID"].Value,
                    out int rlt))
                {
                    T107LeafID = rlt;
                }
            }

            InitComponents();
        }

        /// <summary>
        /// 设备所属PLC对象
        /// </summary>
        public CustomPLC Parent { get; protected set; } = null;

        /// <summary>
        /// 最后一次读取PLC数据块时间
        /// </summary>
        public DateTime LastReadTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后一次发送MES心跳信号时间
        /// </summary>
        public DateTime LastMESHearBeatTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// IRAP中定义的设备叶标识
        /// </summary>
        public int T133LeafID { get; set; } = 0;
        /// <summary>
        /// IRAP中定义的工序叶标识
        /// </summary>
        public int T216LeafID { get; set; } = 0;
        /// <summary>
        /// IRAP中定义的工位叶标识
        /// </summary>
        public int T107LeafID { get; set; } = 0;

        /// <summary>
        /// 指定数据块内容签字是否被改变
        /// </summary>
        /// <param name="key">数据块关键字</param>
        /// <param name="newBuffer">数据块内容</param>
        /// <returns>True: 数据块内容发生改变；False: 数据块内容未改变</returns>
        protected bool IsBufferDataChanged(string key, byte[] newBuffer)
        {
            byte[] newHash = Tools.CalculateHash(newBuffer);
            byte[] oldHash = hashBytes.ContainsKey(key) ? hashBytes[key] : null;
            if (oldHash == null)
            {
                hashBytes.Add(key, newHash);
                return true;
            }
            else if (Tools.ByteEquals(oldHash, newHash))
            {
                return false;
            }
            else
            {
                hashBytes[key] = newHash;
                return true;
            }
        }

        /// <summary>
        /// 初始化字段
        /// </summary>
        /// <remarks>延迟到派生类中进行初始化</remarks>
        protected abstract void InitComponents();

        /// <summary>
        /// 数据块内容发生变化的后续处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buffer">数据块内容</param>
        protected abstract void DBDataChanged(string key, byte[] buffer);

        /// <summary>
        /// 收到数据块数据的后续处理
        /// </summary>
        /// <param name="buffer">数据</param>
        public void DoSomething(byte[] buffer)
        {
            DoSomething("Device.FullBlock", buffer);
        }

        /// <summary>
        /// 收到数据块数据的后续处理
        /// </summary>
        /// <param name="key">数据块关键字</param>
        /// <param name="buffer">数据</param>
        public void DoSomething(string key, byte[] buffer)
        {
            if (IsBufferDataChanged(key, buffer))
            {
                DBDataChanged(key, buffer);
            }
        }

        /// <summary>
        /// 查找指定的Tag
        /// </summary>
        /// <param name="groupName">标记组名称</param>
        /// <param name="tagName">标记名称</param>
        public abstract CustomTag FindTag(string groupName, string tagName);
    }

    /// <summary>
    /// TagGroup父类
    /// </summary>
    public abstract class CustomGroup : CustomProduct
    {
        /// <summary>
        /// Tag集合，该集合对象延迟到派生类中初始化
        /// </summary>
        protected CustomTagCollection _tags;
    }

    /// <summary>
    /// 设备PLC的TagGroup父类
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
                throw new Exception(
                    $"传入的Xml节点不是[TagGroup]，当前收到的Xml节点是:[{node.Name}]");
            }

            _parent =
                parent ??
                throw new Exception(
                    "传入的Device对象是null，TagGroup必须依赖于Device对象");

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
    public abstract class CustomTagGroupCollection :
        CustomProduct, IEnumerable
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
            _parent =
                parent ??
                throw new Exception(
                    "传入的parent对象是null，TagGroupCollection必须依赖于Device对象");
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

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (CustomTagGroup group in _groups.Values)
            {
                yield return group;
            }
        }
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
            _parent =
                parent ??
                throw new Exception(
                    "传入的parent对象是null，SubTagGroup必须依赖于TagGroup对象");

            if (node == null)
            {
                throw new Exception("node对象不能是空值");
            }
            if (node.Name.ToUpper() != "SUBTAGGROUP")
            {
                throw new Exception(
                    $"传入的Xml节点不是[SubTagGroup]，当前收到的Xml节点是:[{node.Name}]");
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
    public abstract class CustomSubTagGroupCollection : CustomProduct, IEnumerable
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
                return
                    _groups.ContainsKey(name) ? _groups[name] : null;
            }
        }

        /// <summary>
        /// 集合中对象数数量
        /// </summary>
        public int Count
        {
            get => _groups.Count;
        }

        /// <summary>
        /// 增加SubTagGroup对象
        /// </summary>
        /// <param name="group">SubTagGroup对象</param>
        public abstract void Add(CustomSubTagGroup group);

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (CustomSubTagGroup group in _groups.Values)
            {
                yield return group;
            }
        }
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
        /// 标签的值
        /// </summary>
        protected object value;

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

            if (node.Attributes["Type"] == null)
            {
                Type = TagType.A;
            }
            else
            {
                try
                {
                    Type = (TagType)Enum.Parse(typeof(TagType), node.Attributes["Type"].Value);
                }
                catch
                {
                    throw new Exception($"不支持Type={node.Attributes["Type"].Value}的类型");
                }
            }
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
        public int DB_Offset { get; protected set; } = 0;

        /// <summary>
        /// Tag类型（C-控制类；A-信息类）
        /// </summary>
        public TagType Type { get; protected set; } = TagType.A;

        /// <summary>
        /// Tag对象
        /// </summary>
        public CustomGroup Parent { get { return _parent; } }

        /// <summary>
        /// 标签的值
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }

    /// <summary>
    /// Tag对象集合类
    /// </summary>
    public abstract class CustomTagCollection : CustomProduct, IEnumerable
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
        /// 根据索引序号获取集合中的CustomTag对象
        /// </summary>
        /// <param name="index">索引序号</param>
        public CustomTag this[int index]
        {
            get
            {
                if (_tags == null)
                {
                    return null;
                }
                else if (index >= 0 && index < _tags.Count)
                {
                    return _tags.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 根据Tag名称获取集合中的CustomTag对象
        /// </summary>
        /// <param name="name">Tag名称</param>
        /// <returns></returns>
        public CustomTag this[string name]
        {
            get
            {
                if (_tags == null)
                {
                    return null;
                }
                else
                {
                    return _tags.ContainsKey(name) ? _tags[name] : null;
                }
            }
        }

        /// <summary>
        /// 增加Tag对象
        /// </summary>
        /// <param name="tag">Tag对象</param>
        public abstract void Add(CustomTag tag);

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (CustomTag tag in _tags.Values)
            {
                yield return tag;
            }
        }
    }
}