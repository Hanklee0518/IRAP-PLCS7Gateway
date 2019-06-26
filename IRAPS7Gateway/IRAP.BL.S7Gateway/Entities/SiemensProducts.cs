using IRAP.BL.S7Gateway.Enums;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// 西门子PLC类
    /// </summary>
    public class SiemensPLC : CustomPLC
    {
        /// <summary>
        /// PLC对象识别码，读写时使用
        /// </summary>
        private long plcHandle;
        /// <summary>
        /// 是否已建立连接
        /// </summary>
        private bool isConnected = false;
        /// <summary>
        /// 是否终止循环读取PLC数据块
        /// </summary>
        private bool cycleReadTerminated = false;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SiemensPLC(XmlNode node) : base(node)
        {
            _log = Logger.Get<SiemensPLC>();
            plcHandle = CS7TcpClient.CreatePlc();

            #region 从Xml节点属性中获取属性值
            if (node.Attributes["IPAddress"] != null)
            {
                IPAddress = node.Attributes["IPAddress"].Value;
            }
            if (IPAddress == "")
            {
                IPAddress = "192.168.0.3";
            }

            if (node.Attributes["Rack"] != null)
            {
                int.TryParse(node.Attributes["Rack"].Value, out int i);
                Rack = i;
            }

            if (node.Attributes["Slot"] != null)
            {
                int.TryParse(node.Attributes["Slot"].Value, out int i);
                Slot = i;
            }
            #endregion

            _log.Trace($"创建西门子PLC对象[{IPAddress}][{Rack}][{Slot}]");

            #region 根据Xml节点属性生成受控的设备对象
            XmlNode deviceNode = node.FirstChild;
            while (deviceNode != null)
            {
                if (deviceNode.Name.ToUpper() == "DEVICE")
                {
                    try
                    {
                        SiemensDevice device = new SiemensDevice(this, deviceNode);
                        if (device != null)
                        {
                            _devices.Add(device.Name, device);
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }
                }

                deviceNode = deviceNode.NextSibling;
            }
            #endregion
        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~SiemensPLC()
        {
            CS7TcpClient.DestoryPlc(ref plcHandle);
        }

        /// <summary>
        /// PLC的IP地址
        /// </summary>
        public string IPAddress { get; private set; } = "";

        /// <summary>
        /// PLC机架号
        /// </summary>
        public int Rack { get; private set; } = 0;

        /// <summary>
        /// PLC插槽号
        /// </summary>
        public int Slot { get; private set; } = 0;

        /// <summary>
        /// 连接到PLC
        /// </summary>
        private bool Connect()
        {
            int iConnected = 0;
            try
            {
                iConnected =
                    CS7TcpClient.ConnectPlc(
                        plcHandle,
                        Encoding.Default.GetBytes(IPAddress),
                        Rack,
                        Slot,
                        false,
                        0,
                        0);

                if (iConnected == 0)
                {
                    isConnected = true;
                    return true;
                }
                else
                {
                    _log.Error($"ErrCode={iConnected}|ErrText={GetErrorMessage(iConnected)}");
                    isConnected = false;
                    return false;
                }
            }
            catch (Exception error)
            {
                isConnected = false;
                _log.Error(error.Message, error);
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private void Disconnect()
        {
            try
            {
                int resNo = CS7TcpClient.DisconnectPlc(plcHandle);
                if (resNo != 0)
                {
                    _log.Error($"ErrCode={resNo}|ErrText={GetErrorMessage(resNo)}");
                }
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
            }
        }

        /// <summary>
        /// 根据错误代码获取错误提示文本
        /// </summary>
        /// <param name="errCode">错误代码</param>
        private string GetErrorMessage(int errCode)
        {
            byte[] errText = new byte[1024];
            int resNo = CS7TcpClient.GetErrorMsg(errCode, errText, errText.Length);
            if (resNo == 0)
            {
                return Encoding.Default.GetString(errText);
            }
            else
            {
                return $"ResNo={resNo}";
            }
        }

        /// <summary>
        /// 循环读取PLC数据块中的数据
        /// </summary>
        private void CycleReadBuffer()
        {
            try
            {
                while (!cycleReadTerminated)
                {
                    if (!isConnected)
                    {
                        isConnected = Connect();
                    }

                    if (isConnected)
                    {
                        byte[] buffer;
                        DateTime now = DateTime.Now;
                        foreach (CustomDevice device in _devices.Values)
                        {
                            if (device is SiemensDevice)
                            {
                                SiemensDevice siemensDevice = device as SiemensDevice;

                                int dbType = (int)siemensDevice.DBType;
                               
                                switch (siemensDevice.CycleReadMode)
                                {
                                    case SiemensCycleReadMode.FullBlock:
                                        buffer = new byte[siemensDevice.FullBlock.BufferLength];
                                        int resNo =
                                            CS7TcpClient.ReadBlockAsByte(
                                                plcHandle,
                                                dbType,
                                                siemensDevice.DBNumber,
                                                siemensDevice.FullBlock.Start_Offset,
                                                siemensDevice.FullBlock.BufferLength,
                                                buffer);

                                        if (resNo != 0)
                                        {
                                            _log.Error(
                                                $"设备[{siemensDevice.Name}]" +
                                                $"读取[{siemensDevice.DBType}]" +
                                                $"[{siemensDevice.DBNumber}]失败，失败信息：" +
                                                $"[Code:{resNo},Message:{GetErrorMessage(resNo)}");
                                            isConnected = false;
                                            continue;
                                        }
                                        else
                                        {
                                            siemensDevice.DoSomething(buffer);
                                        }

                                        break;
                                    case SiemensCycleReadMode.ControlBlock:
                                        break;
                                }
                            }
                        }
                    }

                    Thread.Sleep(10);
                }
            }
            finally
            {
                if (isConnected)
                    Disconnect();
            }
        }

        /// <summary>
        /// 开启循环读取模式
        /// </summary>
        public void Run()
        {
            Thread thread = new Thread(CycleReadBuffer);
            thread.Start();
        }
    }

    /// <summary>
    /// 受西门子PLC控制的设备类
    /// </summary>
    public class SiemensDevice : CustomDevice
    {
        /// <summary>
        /// 整块数据块
        /// </summary>
        public PLCDBReadBlock FullBlock { get; private set; } = new PLCDBReadBlock();
        /// <summary>
        /// 控制量数据块列表
        /// </summary>
        public List<PLCDBReadBlock> ControlBlock { get; private set; } = new List<PLCDBReadBlock>();

        /// <summary>
        /// 构造方法
        /// </summary>
        public SiemensDevice(CustomPLC parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensDevice>();

            if (!(parent is SiemensPLC))
            {
                throw new Exception($"传入的PLC对象不是[SiemensPLC]对象");
            }
            else
            {
                Parent = parent;
            }

            #region 创建 Device
            if (node.Name.ToUpper() != "DEVICE")
            {
                throw new Exception($"Xml节点[{node.Name}]不是\"Device\"");
            }

            if (node.Attributes["Name"] != null)
            {
                Name = node.Attributes["Name"].Value;
            }
            else
            {
                throw new Exception($"{node.Name}.{Name}节点中未找到[Name]属性，请注意属性名的大小写");
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
            else
            {
                throw new Exception(
                    $"{node.Name}.{Name}节点中未找到[DBType]属性，请注意属性名的大小写");
            }

            if (node.Attributes["DBNumber"] != null)
            {
                try
                {
                    DBNumber = int.Parse(node.Attributes["DBNumber"].Value);
                }
                catch
                {
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[DBNumber]属性值错误，必须是整型值");
                }
            }
            else
            {
                throw new Exception(
                    $"{node.Name}.{Name}节点中未找到[DBNumber]属性，请注意属性名的大小写");
            }

            if (node.Attributes["CycleReadMode"] != null)
            {
                try
                {
                    CycleReadMode =
                        (SiemensCycleReadMode)Enum.Parse(
                            typeof(SiemensCycleReadMode),
                            node.Attributes["CycleReadMode"].Value);
                }
                catch
                {
                    string enumValues = "";
                    foreach (var value in Enum.GetValues(typeof(SiemensCycleReadMode)))
                    {
                        enumValues += $"[{value}]";
                    }
                    throw new Exception(
                        $"{node.Name}.{Name}节点中[CycleReadMode]属性值错误，只支持[{enumValues}]");
                }
            }
            #endregion

            _log.Trace(
                $"创建设备[{Name}][DBType={DBType}|DBNumber={DBNumber}|"+
                $"CycleReadMode={CycleReadMode}]");

            #region 创建 Tag 组
            if (node.HasChildNodes)
            {
                CustomTagGroupCreator creator = new SiemensTagGroupCreator();
                XmlNode tagGroupNode = node.FirstChild;
                while (tagGroupNode != null)
                {
                    if (tagGroupNode.Name.ToUpper() == "TAGGROUP")
                    {
                        try
                        {
                            CustomTagGroup group = 
                                creator.CreateProuct(
                                    this, 
                                    tagGroupNode,
                                    OnTagRegister);
                            _groups.Add(group);
                        }
                        catch (Exception error)
                        {
                            _log.Error(error.Message, error);
                        }
                    }

                    tagGroupNode = tagGroupNode.NextSibling;
                }
            }
            #endregion
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// PLC寄存器类别
        /// </summary>
        public SiemensRegisterType DBType { get; private set; }

        /// <summary>
        /// 数据块号
        /// </summary>
        public int DBNumber { get; private set; } = 0;

        /// <summary>
        /// 数据轮询读取模式
        /// </summary>
        public SiemensCycleReadMode CycleReadMode { get; private set; } 
            = SiemensCycleReadMode.FullBlock;

        /// <summary>
        /// 初始化父类中的字段和属性
        /// </summary>
        public override void InitComponents()
        {
            _groups = new SiemensTagGroupCollection(this);
        }

        /// <summary>
        /// Tag对象注册事件
        /// </summary>
        /// <param name="tag">Tag对象</param>
        public void OnTagRegister(CustomTag tag)
        {
            _log.Trace($"Device[{Name}]:Tag[{tag.Name}],Offset[{tag.DB_Offset}]");

            if (!(tag is SiemensTag))
            {
                throw new Exception($"tag对象：{tag.Name}不是SiemensTag对象");
            }

            SiemensTag siemensTag = tag as SiemensTag;
            switch (CycleReadMode)
            {
                case SiemensCycleReadMode.FullBlock:
                    FullBlock.Add(siemensTag.DB_Offset, siemensTag.Length);
                    break;
                case SiemensCycleReadMode.ControlBlock:
                    break;
            }
        }

        /// <summary>
        /// 根据设置的间隔时间以及读取模式读取西门子PLC数据块中的数据内容
        /// </summary>
        private void CycleReadBuffer()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 数据块内容发生变化的后续处理
        /// </summary>
        /// <param name="buffer">数据块内容</param>
        public override void DBDataChanged(byte[] buffer)
        {
            _log.Debug("什么事情都不做");
        }
    }

    /// <summary>
    /// 适用于西门子 PLC 的标记组
    /// </summary>
    public class SiemensTagGroup : CustomTagGroup
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">TagGroup对象所依赖的Device对象</param>
        /// <param name="node">TagGroup对象属性的Xml节点</param>
        /// <param name="eventTagRegister">西门子Tag注册委托</param>
        public SiemensTagGroup(
            CustomDevice parent, 
            XmlNode node, 
            TagRegisterHandler eventTagRegister) : base(parent, node)
        {
            if (!(parent is SiemensDevice))
            {
                throw new Exception("parent对象必须是SiemensDevice对象");
            }

            if (node.Name.ToUpper() != "TAGGROUP")
            {
                throw new Exception($"Xml节点[{node.Name}]不是\"TagGroup\"");
            }

            _log = Logger.Get<SiemensTagGroup>();
            _tags = new SiemensTagCollection(this, eventTagRegister);
            _groups = new SiemensSubTagGroupCollection(this);

            _log.Trace($"创建TagGroup[{Name}]");

            #region 创建Tag
            if (node.HasChildNodes)
            {
                SiemensTagCreator creator = new SiemensTagCreator();
                XmlNode childNode = node.FirstChild;
                while (childNode != null)
                {
                    string nodeName = childNode.Name.ToUpper();
                    switch (nodeName)
                    {
                        case "TAG":
                            CustomTag tag = creator.CreateProduct(this, childNode);
                            if (tag != null)
                            {
                                _tags.Add(tag);
                            }
                            break;
                        case "SUBTAGGROUP":
                            SiemensSubTagGroup subTagGroup = 
                                new SiemensSubTagGroup(
                                    this, 
                                    childNode,
                                    eventTagRegister);
                            if (subTagGroup != null)
                            {
                                _groups.Add(subTagGroup);
                            }
                            break;
                    }

                    childNode = childNode.NextSibling;
                }
            }
            #endregion
        }

        /// <summary>
        /// 所属西门子Device对象
        /// </summary>
        public SiemensDevice Parent
        {
            get
            {
                return _parent as SiemensDevice;
            }
        }
    }

    /// <summary>
    /// 西门子设备的TagGroup集合类
    /// </summary>
    public class SiemensTagGroupCollection : CustomTagGroupCollection
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">隶属的CustomDevice对象</param>
        public SiemensTagGroupCollection(CustomDevice parent) : base(parent)
        {
            if (!(parent is SiemensDevice))
            {
                throw new Exception("parent对象必须是SiemensDevice对象");
            }
        }

        /// <summary>
        /// 增加TagGroup对象
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        public override void Add(CustomTagGroup group)
        {
            if (!(group is SiemensTagGroup))
            {
                throw new Exception("group对象必须是SiemensTagGroup对象");
            }

            SiemensTagGroup sGroup = group as SiemensTagGroup;

            if (sGroup.Parent != _parent)
            {
                throw new Exception("group对象隶属的设备和当前TagGroup集合不是同一个设备");
            }

            if (_groups.Keys.Contains(sGroup.Name))
            {
                throw new Exception($"当前TagGroup集合中已经存在[{sGroup.Name}]!");
            }

            _groups.Add(sGroup.Name, group);
        }
    }

    /// <summary>
    /// 西门子设备的Tag集合
    /// </summary>
    public class SiemensTagCollection : CustomTagCollection
    {
        /// <summary>
        /// Tag对象在SiemensDevice对象中的注册事件
        /// </summary>
        private event TagRegisterHandler OnTagRegister;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="registerEvent">Tag对象在SiemensDevice对象中进行注册的委托</param>
        public SiemensTagCollection(CustomGroup parent, TagRegisterHandler registerEvent) : 
            base(parent)
        {
            OnTagRegister = registerEvent;
        }

        /// <summary>
        /// 增加Tag对象
        /// </summary>
        /// <param name="tag">Tag对象</param>
        public override void Add(CustomTag tag)
        {
            string typeName = tag.GetType().Name;
            if (!typeName.Contains("Siemens"))
            {
                throw new Exception("tag对象不是西门子Tag对象");
            }

            _tags.Add(tag.Name, tag);
            OnTagRegister?.Invoke(tag as SiemensTag);
        }
    }

    /// <summary>
    /// 西门子Tag父类
    /// </summary>
    public abstract class SiemensTag : CustomTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public abstract int Length { get; }
    }

    /// <summary>
    /// 西门子的Bool型Tag对象
    /// </summary>
    public class SiemensBoolOfTag : SiemensTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensBoolOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensBoolOfTag>();

            if (node.Attributes["Offset"] == null)
            {
                throw new Exception($"{Name}节点中未找到[Offset]属性，请注意属性名的大小写");
            }
            string[] offsets = node.Attributes["Offset"].Value.Split('.');
            int.TryParse(offsets[0], out int offset);
            DB_Offset = offset;
            if (offsets.Length >= 2)
            {
                int.TryParse(offsets[1], out int pos);
                Position = pos;
            }
            _log.Trace($"创建Tag[{Name}],Offset={DB_Offset},Position={Position}");
        }

        /// <summary>
        /// 位置
        /// </summary>
        public int Position { get; private set; } = 0;

        /// <summary>
        /// 值
        /// </summary>
        public bool Value { get; private set; } = false;

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 1;

        /// <summary>
        /// 设置PLC中对应的存储区域
        /// </summary>
        /// <param name="offset">数据块偏移量</param>
        /// <param name="position">取值的字节位</param>
        public void SetPosition(int offset, int position)
        {
            DB_Offset = offset;
            Position = position;
        }
    }

    /// <summary>
    /// 具有指定字节长度的西门子Tag类父类
    /// </summary>
    public abstract class SiemensLengthTag : SiemensTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensLengthTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensLengthTag>();

            if (node.Attributes["Offset"] == null)
            {
                throw new Exception($"{Name}节点中未找到[Offset]属性，请注意属性名的大小写");
            }
            int.TryParse(node.Attributes["Offset"].Value, out int offset);
            DB_Offset = offset;
        }
    }

    /// <summary>
    /// Tag值类型为Byte的西门子Tag类
    /// </summary>
    public class SiemensByteOfTag : SiemensLengthTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensByteOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensByteOfTag>();

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");

        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 1;

        /// <summary>
        /// Tag值
        /// </summary>
        public byte Value { get; set; } = 0;
    }

    /// <summary>
    /// Tag值类型为Word的西门子Tag类
    /// </summary>
    public class SiemensWordOfTag : SiemensLengthTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensWordOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensWordOfTag>();

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public ushort Value { get; set; } = 0;
    }

    /// <summary>
    /// Tag值类型为Int的西门子Tag类
    /// </summary>
    public class SiemensIntOfTag : SiemensLengthTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensIntOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensIntOfTag>();

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public short Value { get; set; } = 0;
    }

    /// <summary>
    /// Tag值类型为DWord的西门子Tag类
    /// </summary>
    public class SiemensDWordOfTag : SiemensLengthTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensDWordOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensDWordOfTag>();

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 4;

        /// <summary>
        /// Tag值
        /// </summary>
        public uint Value { get; set; } = 0;
    }

    /// <summary>
    /// Tag值类型为Float的西门子Tag类
    /// </summary>
    public class SiemensRealOfTag : SiemensLengthTag
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensRealOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensRealOfTag>();

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public float Value { get; set; } = 0;
    }

    /// <summary>
    /// Tag值类型为字符串数组的西门子Tag类
    /// </summary>
    public class SiemensArrayCharOfTag : SiemensLengthTag
    {
        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        private int length;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属的TagGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public SiemensArrayCharOfTag(CustomGroup parent, XmlNode node) : base(parent, node)
        {
            _log = Logger.Get<SiemensArrayCharOfTag>();

            if (node.Attributes["Length"] == null)
            {
                throw new Exception("传入的Xml节点没有[Length]属性，请注意大小写");
            }
            int.TryParse(node.Attributes["Length"].Value, out length);

            _log.Trace($"创建Tag{Name},Offset={DB_Offset},Length={length}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override int Length => length;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 西门子SubTagGroup类
    /// </summary>
    public class SiemensSubTagGroup : CustomSubTagGroup
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">TagGroup对象</param>
        /// <param name="node">SubTagGroup属性的Xml节点</param>
        /// <param name="eventTagRegister">西门子Tag注册委托</param>
        public SiemensSubTagGroup(
            CustomTagGroup parent, 
            XmlNode node,
            TagRegisterHandler eventTagRegister) : base(parent, node)
        {
            _log = Logger.Get<SiemensSubTagGroup>();
            _tags = new SiemensTagCollection(this, eventTagRegister);

            if (!(parent is SiemensTagGroup))
            {
                throw new Exception("parent对象必须是SiemensTagGroup对象");
            }

            _log.Trace($"创建[SubTagGroup={Prefix}]");

            XmlNode childNode = node.FirstChild;
            while (childNode != null)
            {
                SiemensTagCreator creator = new SiemensTagCreator();
                CustomTag tag = creator.CreateProduct(this, childNode);
                if (tag != null)
                {
                    _tags.Add(tag);
                }

                childNode = childNode.NextSibling;
            }
        }

        /// <summary>
        /// 所属的TagGroup对象
        /// </summary>
        public CustomTagGroup Parent
        {
            get { return _parent; }
        }
    }

    /// <summary>
    /// 西门子设备的SubTagGroup集合类
    /// </summary>
    public class SiemensSubTagGroupCollection : CustomSubTagGroupCollection
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">隶属的CustomTagGroup对象</param>
        public SiemensSubTagGroupCollection(CustomTagGroup parent) : base(parent)
        {
            if (!(parent is SiemensTagGroup))
            {
                throw new Exception("parent对象必须是SiemensTagGroup对象");
            }
        }

        /// <summary>
        /// 增加SubTagGroup对象
        /// </summary>
        /// <param name="group">SubTagGroup对象</param>
        public override void Add(CustomSubTagGroup group)
        {
            if (!(group is SiemensSubTagGroup))
            {
                throw new Exception("group对象必须是SiemensSubTagGroup对象");
            }

            SiemensSubTagGroup sGroup = group as SiemensSubTagGroup;

            if (sGroup.Parent != _parent)
            {
                throw new Exception("group对象隶属的TagGroup和当前SubTagGroup集合不是同一个TagGroup");
            }

            if (_groups.Keys.Contains(sGroup.Prefix))
            {
                throw new Exception($"当前SubTagGroup集合中已经存在前缀[{sGroup.Prefix}]的SubTagGroup!");
            }

            _groups.Add(sGroup.Prefix, group);
        }
    }
}