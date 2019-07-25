using IRAP.BL.S7Gateway.Enums;
using IRAP.BL.S7Gateway.Utils;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                        DateTime now = DateTime.Now;
                        foreach (CustomDevice device in _devices.Values)
                        {
                            if (device is SiemensDevice)
                            {
                                SiemensDevice siemensDevice = device as SiemensDevice;

                                #region 根据轮询间隔时间读取PLC数据块内容
                                TimeSpan timeSpan = now - siemensDevice.LastReadTime;
                                int dbType = (int)siemensDevice.DBType;
                                if (timeSpan.TotalMilliseconds >= siemensDevice.SplitterTime)
                                {
                                    switch (siemensDevice.CycleReadMode)
                                    {
                                        case SiemensCycleReadMode.FullBlock:
                                            FullBlockModeRead(dbType, siemensDevice);
                                            break;
                                        case SiemensCycleReadMode.ControlBlock:
                                            ControlBlockModeRead(dbType, siemensDevice);
                                            break;
                                    }

                                    siemensDevice.LastReadTime = now;
                                }
                                #endregion

                                #region 每隔2秒钟，向PLC发送当前设备的MESHeartBeat信号
                                KeepMESHeartBeat(now, dbType, siemensDevice);
                                #endregion
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
        /// 每个Device对象单独一个S7连接
        /// </summary>
        private void CycleReadBufferPerDevice()
        {

        }

        /// <summary>
        /// 数据块全部内容模式读取
        /// </summary>
        /// <param name="dbType">数据块类别标识</param>
        /// <param name="device">SiemensDevice对象</param>
        private void FullBlockModeRead(
            int dbType,
            SiemensDevice device)
        {
            byte[] buffer = new byte[device.FullBlock.BufferLength];
            int resNo =
                CS7TcpClient.ReadBlockAsByte(
                    plcHandle,
                    dbType,
                    device.DBNumber,
                    device.FullBlock.Start_Offset,
                    device.FullBlock.BufferLength,
                    buffer);

            if (resNo != 0)
            {
                _log.Error(
                    $"设备[{device.Name}]" +
                    $"读取[{device.DBType}]" +
                    $"[{device.DBNumber}]失败，失败信息：" +
                    $"[Code:{resNo},Message:{GetErrorMessage(resNo)}");
                isConnected = false;
                return;
            }
            else
            {
                device.DoSomething(buffer);
            }
        }

        /// <summary>
        /// 数据块全部数据模式读取(异步方法)
        /// </summary>
        /// <param name="dbType">数据块列别标识</param>
        /// <param name="device">SiemensDevice对象</param>
        /// <returns></returns>
        private async Task FullBlockModeReadAsync(int dbType, SiemensDevice device)
        {
            var task = Task.Run(() =>
            {
                byte[] buffer = new byte[device.FullBlock.BufferLength];
                int resNo =
                    CS7TcpClient.ReadBlockAsByte(
                        plcHandle,
                        dbType,
                        device.DBNumber,
                        device.FullBlock.Start_Offset,
                        device.FullBlock.BufferLength,
                        buffer);

                if (resNo != 0)
                {
                    _log.Error(
                        $"设备[{device.Name}]" +
                        $"读取[{device.DBType}]" +
                        $"[{device.DBNumber}]失败，失败信息：" +
                        $"[Code:{resNo},Message:{GetErrorMessage(resNo)}");
                    isConnected = false;
                    return;
                }
                else
                {
                    device.DoSomething(buffer);
                }
            });

            await task;
        }

        /// <summary>
        /// 控制信号数据块模式读取
        /// </summary>
        /// <param name="dbType">数据块类别标识</param>
        /// <param name="device">SiemensDevice对象</param>
        private void ControlBlockModeRead(
            int dbType,
            SiemensDevice device)
        {
            byte[] buffer;
            for (int i = 0; i < device.ControlBlock.Count; i++)
            {
                buffer = new byte[device.ControlBlock[i].BufferLength];
                int resNo =
                    CS7TcpClient.ReadBlockAsByte(
                        plcHandle,
                        dbType,
                        device.DBNumber,
                        device.ControlBlock[i].Start_Offset,
                        device.ControlBlock[i].BufferLength,
                        buffer);

                if (resNo != 0)
                {
                    _log.Error(
                        $"设备[{device.Name}]" +
                        $"读取[{device.DBType}]" +
                        $"[{device.DBNumber}]失败，失败信息：" +
                        $"[Code:{resNo},Message:{GetErrorMessage(resNo)}");
                    isConnected = false;
                    return;
                }
                else
                {
                    device.DoSomething(device.ControlBlock.GetKey(i), buffer);
                }
            }
        }

        /// <summary>
        /// 保持每两秒中发送一次MESHeartBeat信号
        /// </summary>
        /// <param name="now">线程本次执行的瞬时时间</param>
        /// <param name="dbType">数据块类别标识</param>
        /// <param name="device">SiemensDevice对象</param>
        private void KeepMESHeartBeat(
            DateTime now,
            int dbType,
            SiemensDevice device)
        {
            TimeSpan timeSpan = now - device.LastMESHearBeatTime;
            if (timeSpan.TotalMilliseconds >= 2000)
            {
                CustomTag tag = device.FindTag("COMM", "MES_Heart_Beat");
                if (tag != null && tag is SiemensBoolOfTag)
                {
                    SiemensBoolOfTag heartBeatTag = tag as SiemensBoolOfTag;
                    heartBeatTag.Value = !heartBeatTag.Value;
                    CS7TcpClient.WriteBool(
                        plcHandle,
                        dbType,
                        device.DBNumber,
                        heartBeatTag.DB_Offset,
                        heartBeatTag.Position,
                        heartBeatTag.Value);
                }

                device.LastMESHearBeatTime = now;
            }
        }

        /// <summary>
        /// 将值回写到PLC中
        /// </summary>
        /// <param name="device">西门子Device对象</param>
        /// <param name="tag">西门子Tag对象</param>
        private void WriteToPLC(SiemensDevice device, SiemensTag tag)
        {
            int rlt = 0;
            try
            {
                if (tag is SiemensBoolOfTag)
                {
                    SiemensBoolOfTag ltag = tag as SiemensBoolOfTag;
                    rlt = CS7TcpClient.WriteBool(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Position,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensByteOfTag)
                {
                    SiemensByteOfTag ltag = tag as SiemensByteOfTag;
                    rlt = CS7TcpClient.WriteByte(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensWordOfTag)
                {
                    SiemensWordOfTag ltag = tag as SiemensWordOfTag;
                    rlt = CS7TcpClient.WriteWord(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensIntOfTag)
                {
                    SiemensIntOfTag ltag = tag as SiemensIntOfTag;
                    rlt = CS7TcpClient.WriteInt(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensDWordOfTag)
                {
                    SiemensDWordOfTag ltag = tag as SiemensDWordOfTag;
                    rlt = CS7TcpClient.WriteDWord(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensRealOfTag)
                {
                    SiemensRealOfTag ltag = tag as SiemensRealOfTag;
                    rlt = CS7TcpClient.WriteFloat(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Value);
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
                else if (tag is SiemensArrayCharOfTag)
                {
                    SiemensArrayCharOfTag ltag = tag as SiemensArrayCharOfTag;
                    rlt = CS7TcpClient.WriteString(
                        plcHandle,
                        (int)device.DBType,
                        device.DBNumber,
                        ltag.DB_Offset,
                        ltag.Length,
                        Encoding.ASCII.GetBytes(ltag.Value));
                    _log.Debug(
                        $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                        $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                }
            }
            catch (Exception error)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入时发生错误，{error.Message}");
            }

            if (rlt != 0)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:设备[{device.Name}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入失败，错误提示:[{rlt}]" +
                    $"[{GetErrorMessage(rlt)}]");
            }
        }

        /// <summary>
        /// 开启循环读线程
        /// </summary>
        public void Start()
        {
            // 由于当前的读取模式是对于本PLC中的所有设备进行轮询读取，因此处理速度会滞后，该方法暂时不用
            //Thread thread = new Thread(CycleReadBuffer);
            //thread.Start();

            foreach (CustomDevice device in _devices.Values)
            {
                if (device is SiemensDevice)
                {
                    ((SiemensDevice)device).Start(IPAddress, Rack, Slot);
                }
            }
        }

        /// <summary>
        /// 终止循环读取线程
        /// </summary>
        public void Stop()
        {
            foreach (CustomDevice device in _devices.Values)
            {
                if (device is SiemensDevice)
                {
                    ((SiemensDevice)device).Stop();
                }
            }
        }
    }

    /// <summary>
    /// 受西门子PLC控制的设备类
    /// </summary>
    public class SiemensDevice : CustomDevice
    {
        /// <summary>
        /// 西门子PLC循环读取连接对象
        /// </summary>
        private SiemensPLCConnection cycleReadConnection;
        /// <summary>
        /// 单独读写PLC连接对象
        /// </summary>
        private SiemensPLCConnection specialRWConnection;
        /// <summary>
        /// 是否终止循环读取数据线程
        /// </summary>
        private bool threadTerminated = false;

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

            if (node.Attributes["SplitterTime"] != null)
            {
                try
                {
                    SplitterTime = short.Parse(node.Attributes["SplitterTime"].Value);
                }
                catch (Exception error)
                {
                    _log.Error(
                        $"{node.Name}.{Name}节点中[SplitterTime]属性值错误，使用缺省10毫秒",
                        error);
                }
            }
            #endregion

            _log.Trace(
                $"创建设备[{Name}][DBType={DBType}|DBNumber={DBNumber}|" +
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
        /// 标记组
        /// </summary>
        public SiemensTagGroupCollection Groups
        {
            get { return _groups as SiemensTagGroupCollection; }
        }

        /// <summary>
        /// 整块数据块
        /// </summary>
        public PLCDBBlock FullBlock { get; private set; } = new PLCDBBlock();

        /// <summary>
        /// 控制量数据块列表
        /// </summary>
        public PLCDBBlockCollection ControlBlock { get; private set; } =
            new PLCDBBlockCollection();

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
        /// 轮询读取数据块内容间隔时间（毫秒）
        /// </summary>
        public short SplitterTime { get; private set; } = 10;

        /// <summary>
        /// Tag对象注册事件
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        /// <param name="tag">Tag对象</param>
        private void OnTagRegister(CustomGroup group, CustomTag tag)
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
                    if (siemensTag.Type == TagType.C)
                    {
                        string key = "";
                        if (group is SiemensTagGroup)
                        {
                            key = (group as SiemensTagGroup).Name;
                        }
                        else if (group is SiemensSubTagGroup)
                        {
                            SiemensSubTagGroup subGroup = group as SiemensSubTagGroup;
                            key = $"{subGroup.Parent.Name}.{subGroup.Prefix}";
                        }

                        PLCDBBlock block = ControlBlock[key];
                        if (block == null)
                        {
                            block = new PLCDBBlock();
                            ControlBlock.Add(key, block);
                        }
                        block.Add(siemensTag.DB_Offset, siemensTag.Length);
                    }
                    break;
            }
        }

        /// <summary>
        /// FullBlock模式读取数据块
        /// </summary>
        private void FullBlockModeRead()
        {
            byte[] buffer = new byte[FullBlock.BufferLength];
            int resNo =
                cycleReadConnection.ReadBlock(
                    DBType,
                    DBNumber,
                    FullBlock.Start_Offset,
                    FullBlock.BufferLength,
                    ref buffer,
                    out string errText);

            if (resNo != 0)
            {
                _log.Error(
                    $"设备[{Name}]读取[{DBType}][{DBNumber}]失败，失败信息" +
                    $"[Code:{resNo},Message:{errText}]");
            }
            else
            {
                DoSomething(buffer);
            }
        }

        /// <summary>
        /// ControlBlock模式读取数据块
        /// </summary>
        private void ControlBlockModeRead()
        {
            byte[] buffer;
            for (int i = 0; i < ControlBlock.Count; i++)
            {
                PLCDBBlock block = ControlBlock[i];
                buffer = new byte[block.BufferLength];
                int resNo =
                    cycleReadConnection.ReadBlock(
                        DBType,
                        DBNumber,
                        block.Start_Offset,
                        block.BufferLength,
                        ref buffer,
                        out string errText);

                if (resNo != 0)
                {
                    _log.Error(
                        $"设备[{Name}]读取[{DBType}][{DBNumber}]" +
                        $"Offset[{block.Start_Offset}]Length[{block.BufferLength}]" +
                        $"失败，失败信息：[Code:{resNo},Message:{errText}]");
                }
                else
                {
                    DoSomething(ControlBlock.GetKey(i), buffer);
                }
            }
        }

        /// <summary>
        /// 根据关键字处理指定TagGroup所对应的数据块数据
        /// </summary>
        private void DealPartBlockBuffer(string key, byte[] buffer)
        {
            PLCDBBlock block = ControlBlock[key];
            if (block != null)
            {
                _log.Debug(
                    $"[{key}.Offset={block.Start_Offset}, " +
                    $"{key}.Length={block.BufferLength}]");
            }
            _log.Debug($"[{key}]|[Data:{Tools.BytesToBCD(buffer)}]");

            // 根据key解析出TagGroup和SubTagGroup
            string[] keys = key.Split('.');
            string keyTagGroup = keys[0];
            string keySubTagGroup = "";
            if (keys.Length >= 2)
            {
                keySubTagGroup = keys[1];
            }

            SiemensTagGroup group = _groups[keyTagGroup] as SiemensTagGroup;
            if (group == null)
            {
                _log.Error($"设备[{Name}]未找到[{keyTagGroup}]Tag组");
                return;
            }
            if (keySubTagGroup == "")
            {
                SetControlTagValueInGroup(group, buffer, block.Start_Offset);
            }
            else
            {
                SiemensSubTagGroup subGroup =
                    group.SubGroups[keySubTagGroup] as SiemensSubTagGroup;
                if (subGroup == null)
                {
                    _log.Error($"设备[{Name}]未找到[{keyTagGroup}.{keySubTagGroup}]Tag组");
                    return;
                }
                else
                {
                    SetControlTagValueInGroup(subGroup, buffer, block.Start_Offset);
                }
            }
        }

        /// <summary>
        /// 设置控制Tag值
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        /// <param name="buffer">数据块内容</param>
        /// <param name="offset">当前数据块内容在整个数据块的起始偏移量</param>
        private void SetControlTagValueInGroup(
            SiemensTagGroup group,
            byte[] buffer,
            int offset)
        {
            foreach (CustomTag tag in group.Tags)
            {
                if (tag.Type == TagType.C)
                {
                    if (SetTagValue(buffer, tag, offset))
                    {
                        ControlTagValueChanged(tag);
                    }
                }
            }
        }
        /// <summary>
        /// 设置控制Tag值
        /// </summary>
        /// <param name="group">SubTagGroup对象</param>
        /// <param name="buffer">数据块内容</param>
        /// <param name="offset">当前数据块内容在整个数据块的起始偏移量</param>
        private void SetControlTagValueInGroup(
            SiemensSubTagGroup group,
            byte[] buffer,
            int offset)
        {
            foreach (CustomTag tag in group.Tags)
            {
                if (tag.Type == TagType.C)
                {
                    if (SetTagValue(buffer, tag, offset))
                    {
                        ControlTagValueChanged(tag);
                    }
                }
            }
        }

        /// <summary>
        /// 控制Tag值变化后的操作
        /// </summary>
        /// <param name="tag">控制Tag对象</param>
        private void ControlTagValueChanged(CustomTag tag)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IIRAPDCSTrade trade = IRAPDCSTraderCreator.CreateInstance(tag.Name.Replace("_", ""));
            if (trade != null)
            {
                List<SiemensTag> writeTags = trade.Do(this, tag as SiemensTag);
                foreach (SiemensTag writeTag in writeTags)
                {
                    specialRWConnection.WriteToPLC(DBType, DBNumber, writeTag);
                }
            }
            sw.Stop();
            _log.Debug($"触发信号[{tag.Name}]的执行时长：[{sw.ElapsedMilliseconds}]毫秒");
        }

        /// <summary>
        /// 设置Tag对象的值
        /// </summary>
        /// <param name="buffer">从PLC读取数据块中的内容</param>
        /// <param name="tag">Tag对象</param>
        /// <param name="beginOffset">数据块开始偏移量</param>
        private bool SetTagValue(byte[] buffer, CustomTag tag, int beginOffset)
        {
            int offset = tag.DB_Offset - beginOffset;

            if (tag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag ltag = tag as SiemensBoolOfTag;

                bool newValue = Tools.GetBitValue(buffer[offset], ltag.Position);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensByteOfTag)
            {
                SiemensByteOfTag ltag = tag as SiemensByteOfTag;

                byte newValue = buffer[offset];
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensWordOfTag)
            {
                SiemensWordOfTag ltag = tag as SiemensWordOfTag;

                ushort newValue = Tools.GetWordValue(buffer, offset);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensIntOfTag)
            {
                SiemensIntOfTag ltag = tag as SiemensIntOfTag;

                short newValue = Tools.GetIntValue(buffer, offset);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensDWordOfTag)
            {
                SiemensDWordOfTag ltag = tag as SiemensDWordOfTag;

                uint newValue = Tools.GetDWordValue(buffer, offset);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensRealOfTag)
            {
                SiemensRealOfTag ltag = tag as SiemensRealOfTag;

                float newValue=Tools.GetRealValue(buffer, offset);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else if (tag is SiemensArrayCharOfTag)
            {
                SiemensArrayCharOfTag ltag = tag as SiemensArrayCharOfTag;

                string newValue=Tools.GetStringValue(buffer, offset, ltag.Length);
                if (ltag.Value == newValue)
                {
                    return false;
                }

                ltag.Value = newValue;
                _log.Trace($"[{ltag.Name}={ltag.Value}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 处理 FullBlock 模式下获取到的数据块数据
        /// </summary>
        /// <param name="buffer">数据块数据</param>
        private void DealFullBlockModeBuffer(byte[] buffer)
        {
            foreach (CustomTagGroup group in _groups)
            {
                SiemensTagGroup sGroup = group as SiemensTagGroup;
                foreach (CustomTag tag in sGroup.Tags)
                {
                    SetTagValue(buffer, tag, 0);
                }
                foreach (SiemensSubTagGroup subGroup in sGroup.SubGroups)
                {
                    foreach (CustomTag tag in subGroup.Tags)
                    {
                        SetTagValue(buffer, tag, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 循环读取西门子PLC数据块内容的线程执行体
        /// </summary>
        private void CycleReadDBThread()
        {
            if (cycleReadConnection == null)
            {
                throw new Exception("未创建西门子PLC连接对象");
            }

            while (!threadTerminated)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpan = now - LastReadTime;
                if (timeSpan.TotalMilliseconds >= SplitterTime)
                {
                    LastReadTime = now;
                    switch (CycleReadMode)
                    {
                        case SiemensCycleReadMode.FullBlock:
                            FullBlockModeRead();
                            break;
                        case SiemensCycleReadMode.ControlBlock:
                            ControlBlockModeRead();
                            break;
                    }
                        _log.Trace($"设备[{Name}]间隔上次读取[{timeSpan.TotalMilliseconds}]毫秒后再次读取 PLC 数据块");
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 定时发送MES心跳信号线程，目前固定每隔2秒钟发送一次，间隔时间不能修改
        /// </summary>
        private void MESHeartBeat()
        {
            while (!threadTerminated)
            {
                DateTime now = DateTime.Now;
                if (cycleReadConnection != null)
                {
                    TimeSpan timeSpan = now - LastMESHearBeatTime;
                    if (timeSpan.TotalMilliseconds >= 2000)
                    {
                        CustomTag tag = FindTag("COMM", "MES_Heart_Beat");
                        if (tag != null && tag is SiemensBoolOfTag)
                        {
                            SiemensBoolOfTag heartBeatTag = tag as SiemensBoolOfTag;
                            heartBeatTag.Value = !heartBeatTag.Value;
                            try
                            {
                                cycleReadConnection.WriteToPLC(
                                    DBType,
                                    DBNumber,
                                    heartBeatTag);
                            }
                            catch (Exception error)
                            {
                                _log.Error(error.Message, error);
                            }
                        }

                        LastMESHearBeatTime = now;
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 初始化父类中的字段和属性
        /// </summary>
        protected override void InitComponents()
        {
            _groups = new SiemensTagGroupCollection(this);
        }

        /// <summary>
        /// 数据块内容发生变化的后续处理
        /// </summary>
        /// <param name="key">数据块关键字</param>
        /// <param name="buffer">数据块内容</param>
        protected override void DBDataChanged(string key, byte[] buffer)
        {
            //string tmp = "";
            //for (int i = 0; i < buffer.Length; i++)
            //{
            //    tmp += $"{string.Format("{0:x2}", buffer[i])} ";
            //}
            //_log.Debug($"[{Name}.{key}]Buffer Size={buffer.Length}|[{tmp}]");

            switch (CycleReadMode)
            {
                case SiemensCycleReadMode.FullBlock:
                    DealFullBlockModeBuffer(buffer);
                    break;
                case SiemensCycleReadMode.ControlBlock:
                    DealPartBlockBuffer(key, buffer);
                    break;
            }
        }

        /// <summary>
        /// 查找指定的Tag
        /// </summary>
        /// <param name="name">标记组名称</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>查找到的CustomTag对象，若没有找到则返回null</returns>
        public override CustomTag FindTag(string name, string tagName)
        {
            if (name == "" || tagName == "")
            {
                return null;
            }

            string[] names = name.Split('.');
            string groupName = names[0];
            string subGroupName = names.Length >= 2 ? names[1] : "";

            CustomTagGroup group = _groups[groupName];
            if (group == null)
            {
                return null;
            }
            else
            {
                if (subGroupName == "")
                {
                    return ((SiemensTagGroup)group).Tags[tagName];
                }
                else
                {
                    CustomSubTagGroup subGroup = ((SiemensTagGroup)group).SubGroups[subGroupName];
                    if (subGroup == null)
                    {
                        return null;
                    }
                    else
                    {
                        return ((SiemensSubTagGroup)subGroup).Tags[tagName];
                    }
                }
            }

        }

        /// <summary>
        /// 读取西门子PLC中该Tag的值
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public SiemensTag ReadTagValue(SiemensTag tag)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (tag is SiemensBoolOfTag)
            {
                byte[] buffer = new byte[1];
                int resNo =
                    specialRWConnection.ReadBlock(
                        DBType,
                        DBNumber,
                        tag.DB_Offset,
                        1,
                        ref buffer,
                        out string errText);
                SetTagValue(buffer, tag, tag.DB_Offset);
            }
            else if (tag is SiemensLengthTag)
            {
                SiemensLengthTag ltag = tag as SiemensLengthTag;
                byte[] buffer = new byte[ltag.Length];
                int resNo =
                    specialRWConnection.ReadBlock(
                        DBType,
                        DBNumber,
                        ltag.DB_Offset,
                        ltag.Length,
                        ref buffer,
                        out string errText);
                SetTagValue(buffer, tag, tag.DB_Offset);
            }

            sw.Stop();
            _log.Debug($"触发信号[{tag.Name}]的执行时长：[{sw.ElapsedMilliseconds}]毫秒");

            return tag;
        }

        /// <summary>
        /// 开始当前设备对象循环读取PLC数据块，并处理PLC的请求
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="rack"></param>
        /// <param name="slot"></param>
        public void Start(string ipAddress, int rack, int slot)
        {
            cycleReadConnection = new SiemensPLCConnection(ipAddress, rack, slot);
            specialRWConnection = new SiemensPLCConnection(ipAddress, rack, slot);

            threadTerminated = false;
            Thread threadHeartBeat = new Thread(MESHeartBeat);
            Thread threadCycleReader = new Thread(CycleReadDBThread);

            threadHeartBeat.Start();
            threadCycleReader.Start();
        }

        /// <summary>
        /// 终止当前设备对象读取PLC数据块线程和心跳信号线程
        /// </summary>
        public void Stop()
        {
            threadTerminated = true;
        }
    }

    /// <summary>
    /// 适用于西门子 PLC 的标记组
    /// </summary>
    public class SiemensTagGroup : CustomTagGroup
    {
        /// <summary>
        /// 父容器中的Tag对象注册事件
        /// </summary>
        private event TagRegisterHandler _tagRegisterInParentHandler = null;

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

            _tagRegisterInParentHandler = eventTagRegister;

            _log = Logger.Get<SiemensTagGroup>();
            _tags = new SiemensTagCollection(this, OnTagInGroupRegister);
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
                                    OnTagInGroupRegister);
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
            get { return _parent as SiemensDevice; }
        }

        /// <summary>
        /// 标记列表
        /// </summary>
        public SiemensTagCollection Tags
        {
            get { return _tags as SiemensTagCollection; }
        }

        /// <summary>
        /// 子标记组列表
        /// </summary>
        public SiemensSubTagGroupCollection SubGroups
        {
            get { return _groups as SiemensSubTagGroupCollection; }
        }

        /// <summary>
        /// TagGroup对象对应PLC数据块的DBBlock定义
        /// </summary>
        public PLCDBBlock Block { get; } = new PLCDBBlock();

        /// <summary>
        /// Tag对象注册事件
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        /// <param name="tag">Tag对象</param>
        private void OnTagInGroupRegister(CustomGroup group, CustomTag tag)
        {
            _tagRegisterInParentHandler?.Invoke(group, tag);

            SiemensTag siemensTag = tag as SiemensTag;
            Block.Add(siemensTag.DB_Offset, siemensTag.Length);
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

            OnTagRegister?.Invoke(_parent, tag as SiemensTag);
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
        public abstract short Length { get; }

        /// <summary>
        /// Tag值
        /// </summary>
        public object Value
        {
            get => value;
            set
            {
                base.value = value;
            }
        }
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

            value = false;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public int Position { get; private set; } = 0;

        /// <summary>
        /// 值
        /// </summary>
        public new bool Value
        {
            get { return (bool)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 1;

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

            value = 0;

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");

        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 1;

        /// <summary>
        /// Tag值
        /// </summary>
        public new byte Value
        {
            get { return (byte)value; }
            set { base.value = value; }
        }
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

            value = 0;

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public new ushort Value
        {
            get { return (ushort)value; }
            set { base.value = value; }
        }
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

            value = 0;

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public new short Value
        {
            get { return (short)value; }
            set { base.value = value; }
        }
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

            value = 0;

            _log.Trace($"创建Tag{Name},Offset={DB_Offset}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 4;

        /// <summary>
        /// Tag值
        /// </summary>
        public new uint Value
        {
            get { return (uint)value; }
            set { base.value = value; }
        }
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

            value = 0;
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => 2;

        /// <summary>
        /// Tag值
        /// </summary>
        public new float Value
        {
            get { return (float)value; }
            set { base.value = value; }
        }
    }

    /// <summary>
    /// Tag值类型为字符串数组的西门子Tag类
    /// </summary>
    public class SiemensArrayCharOfTag : SiemensLengthTag
    {
        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        private short length;

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
            short.TryParse(node.Attributes["Length"].Value, out length);
            value = "";

            _log.Trace($"创建Tag{Name},Offset={DB_Offset},Length={length}");
        }

        /// <summary>
        /// Tag值占用的字节长度
        /// </summary>
        public override short Length => length;

        /// <summary>
        /// 值
        /// </summary>
        public new string Value
        {
            get { return (string)value; }
            set { base.value = value; }
        }

        /// <summary>
        /// 设置Tag值占用的字节长度
        /// </summary>
        /// <param name="length">长度</param>
        public void SetLength(short length)
        {
            this.length = length;
        }
    }

    /// <summary>
    /// 西门子SubTagGroup类
    /// </summary>
    public class SiemensSubTagGroup : CustomSubTagGroup
    {
        /// <summary>
        /// 父容器中的Tag对象注册事件
        /// </summary>
        private TagRegisterHandler _tagRegisterInParentHandler = null;

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
            _tagRegisterInParentHandler = eventTagRegister;
            _log = Logger.Get<SiemensSubTagGroup>();
            _tags = new SiemensTagCollection(this, OnTagInGroupRegister);

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

        /// <summary>
        /// 标记列表
        /// </summary>
        public SiemensTagCollection Tags
        {
            get { return _tags as SiemensTagCollection; }
        }

        /// <summary>
        /// SubTagGroup对象对应PLC数据块的DBBlock定义
        /// </summary>
        public PLCDBBlock Block { get; } = new PLCDBBlock();

        /// <summary>
        /// Tag对象注册事件
        /// </summary>
        /// <param name="group">TagGroup对象</param>
        /// <param name="tag">Tag对象</param>
        private void OnTagInGroupRegister(CustomGroup group, CustomTag tag)
        {
            _tagRegisterInParentHandler?.Invoke(group, tag);

            SiemensTag siemensTag = tag as SiemensTag;
            Block.Add(siemensTag.DB_Offset, siemensTag.Length);
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