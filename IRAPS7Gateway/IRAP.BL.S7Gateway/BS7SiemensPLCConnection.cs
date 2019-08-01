using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// 西门子PLC连接类，用于读写西门子PLC的数据块
    /// </summary>
    public class BS7SiemensPLCConnection : IRAPBaseObject
    {
        /// <summary>
        /// PLC对象识别码，读写时使用
        /// </summary>
        private long plcHandle;
        /// <summary>
        /// 是否连接到PLC
        /// </summary>
        private bool isConnected = false;
        /// <summary>
        /// 锁定物
        /// </summary>
        private object _lockObjet = new object();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ipAddress">PLC的IP地址</param>
        /// <param name="rack">PLC的机架号</param>
        /// <param name="slot">PLC的插槽号</param>
        public BS7SiemensPLCConnection(string ipAddress, int rack, int slot)
        {
            _log = Logger.Get<BS7SiemensPLCConnection>();

            IPAddress = ipAddress;
            Rack = rack;
            Slot = slot;

            plcHandle = CS7TcpClient.CreatePlc();
            isConnected = Connect();
        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~BS7SiemensPLCConnection()
        {
            CS7TcpClient.DestoryPlc(ref plcHandle);
        }

        /// <summary>
        /// PLC的IP地址
        /// </summary>
        public string IPAddress { get; private set; }
        /// <summary>
        /// PLC的机架号
        /// </summary>
        public int Rack { get; private set; }
        /// <summary>
        /// PLC的插槽号
        /// </summary>
        public int Slot { get; private set; }

        /// <summary>
        /// 连接到PLC
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            int iConnected = 0;
            try
            {
                lock (_lockObjet)
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
                }

                if (iConnected == 0)
                {
                    return true;
                }
                else
                {
                    _log.Error($"ErrCode={iConnected}|ErrText={GetErrorMessage(iConnected)}");
                    return false;
                }
            }
            catch (Exception error)
            {
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
        /// 读取数据块内容
        /// </summary>
        /// <param name="dbType">数据块类别标识</param>
        /// <param name="dbNumber">DB数据块编号</param>
        /// <param name="dbOffset">读取的起始偏移量</param>
        /// <param name="bufferLength">缓冲区长度</param>
        /// <param name="buffer">byte[]类型的缓冲区</param>
        /// <param name="errText">执行结果信息</param>
        public int ReadBlock(
            SiemensRegisterType dbType,
            int dbNumber,
            int dbOffset,
            int bufferLength,
            ref byte[] buffer,
            out string errText)
        {
            if (!isConnected)
            {
                isConnected = Connect();
            }

            buffer = new byte[bufferLength];
            int resNo = 0;
            lock (_lockObjet)
            {
                resNo =
                    CS7TcpClient.ReadBlockAsByte(
                        plcHandle,
                        (int)dbType,
                        dbNumber,
                        dbOffset,
                        bufferLength,
                        buffer);
            }

            if (resNo != 0)
            {
                errText = GetErrorMessage(resNo);
                _log.Error(
                    $"PLC:[{IPAddress}]读取[{dbType}]" +
                    $"[{dbNumber}]失败，失败信息：" +
                    $"[Code:{resNo},Message:{errText}");
            }
            else
            {
                errText = "读取正常";
            }

            return resNo;
        }

        /// <summary>
        /// 将值回写到PLC中
        /// </summary>
        /// <param name="dbType">数据块类别</param>
        /// <param name="dbNumber">DB数据块编号</param>
        /// <param name="tag">西门子Tag对象</param>
        public void WriteToPLC(
            SiemensRegisterType dbType,
            int dbNumber,
            SiemensTag tag)
        {
            if (!isConnected)
            {
                isConnected = Connect();
            }

            int rlt = 0;
            try
            {
                lock (_lockObjet)
                {
                    if (tag is SiemensBoolOfTag)
                    {
                        SiemensBoolOfTag ltag = tag as SiemensBoolOfTag;
                        rlt = CS7TcpClient.WriteBool(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Position,
                            ltag.Value);
                        if (!ltag.Name.Contains("MES_Heart_Beat"))
                        {
                            _log.Debug(
                              $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                              $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                        }
                    }
                    else if (tag is SiemensByteOfTag)
                    {
                        SiemensByteOfTag ltag = tag as SiemensByteOfTag;
                        rlt = CS7TcpClient.WriteByte(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensWordOfTag)
                    {
                        SiemensWordOfTag ltag = tag as SiemensWordOfTag;
                        rlt = CS7TcpClient.WriteWord(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensIntOfTag)
                    {
                        SiemensIntOfTag ltag = tag as SiemensIntOfTag;
                        rlt = CS7TcpClient.WriteInt(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensDWordOfTag)
                    {
                        SiemensDWordOfTag ltag = tag as SiemensDWordOfTag;
                        rlt = CS7TcpClient.WriteDWord(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensRealOfTag)
                    {
                        SiemensRealOfTag ltag = tag as SiemensRealOfTag;
                        rlt = CS7TcpClient.WriteFloat(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensArrayCharOfTag)
                    {
                        SiemensArrayCharOfTag ltag = tag as SiemensArrayCharOfTag;
                        rlt = CS7TcpClient.WriteString(
                            plcHandle,
                            (int)dbType,
                            dbNumber,
                            ltag.DB_Offset,
                            ltag.Length,
                            Encoding.ASCII.GetBytes(ltag.Value));
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，待写入:[{ltag.Value}]");
                    }
                }
            }
            catch (Exception error)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入时发生错误，{error.Message}");
            }

            if (rlt != 0)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入失败，错误提示:[{rlt}]" +
                    $"[{GetErrorMessage(rlt)}]");
            }
        }
    }
}
