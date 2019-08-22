﻿using HslCommunication;
using HslCommunication.Profinet.Siemens;
using IRAP.BL.S7Gateway.Entities;
using IRAP.BL.S7Gateway.Enums;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// 使用HslCommunication控件来读写西门子PLC 
    /// </summary>
    public class SiemensPLCConnection : IRAPBaseObject
    {
        private SiemensS7Net siemensTcpNet = null;
        private SiemensPLCS siemensPLCS = SiemensPLCS.S1200;
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
        public SiemensPLCConnection(string ipAddress, int rack, int slot)
        {
            _log = Logger.Get<SiemensPLCConnection>();
            siemensTcpNet = new SiemensS7Net(siemensPLCS);

            IPAddress = ipAddress;
            Rack = rack;
            Slot = slot;

            isConnected = Connect();
        }

        /// <summary>
        /// 析构方法，释放西门子S7的连接
        /// </summary>
        ~SiemensPLCConnection()
        {
            siemensTcpNet.ConnectClose();
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
        /// <returns>true-连接成功；false-连接失败</returns>
        private bool Connect()
        {
            try
            {
                siemensTcpNet.IpAddress = IPAddress;
                siemensTcpNet.Port = 102;
                siemensTcpNet.Rack = (byte)Rack;
                siemensTcpNet.Slot = (byte)Slot;

                OperateResult connect = siemensTcpNet.ConnectServer();
                if (connect.IsSuccess)
                {
                    return true;
                }
                else
                {
                    _log.Error(
                        $"连接到[{IPAddress}:{siemensTcpNet.Port}][" +
                        $"Rack={Rack}|Slot={Slot}失败，原因：[({connect.ErrorCode})" +
                        $"{connect.Message}");
                    return false;
                }
            }
            catch (Exception error)
            {
                _log.Error(
                    $"连接到[{IPAddress}:{siemensTcpNet.Port}][" +
                    $"Rack={Rack}|Slot={Slot}失败，原因：{error.Message}",
                    error);
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
                OperateResult rlt = siemensTcpNet.ConnectClose();
                if (!rlt.IsSuccess)
                {
                    _log.Error($"ErrCode={rlt.ErrorCode}|ErrText={rlt.Message}");
                }
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
            }
            finally
            {
                isConnected = false;

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
            OperateResult<byte[]> resNo;
            lock (_lockObjet)
            {
                string addr = $"{dbType.ToString()}{dbNumber}.{dbOffset}";
                resNo =
                    siemensTcpNet.Read(
                        addr,
                        (ushort)bufferLength);
            }

            if (!resNo.IsSuccess)
            {
                errText = resNo.ToMessageShowString();
                _log.Error(
                    $"PLC:[{IPAddress}]读取[{dbType}]" +
                    $"[{dbNumber}]失败，失败信息：" +
                    $"[Message:{errText}");
            }
            else
            {
                buffer = resNo.Content;
                errText = "读取正常";
            }
            
            return resNo.ErrorCode;
        }

        /// <summary>
        /// 读取Float值
        /// </summary>
        /// <param name="dbType">数据块类别标识</param>
        /// <param name="dbNumber">DB数据块编号</param>
        /// <param name="dbOffset">读取的起始偏移量</param>
        /// <param name="value">float类型的值</param>
        /// <param name="errText">执行结果信息</param>
        public int ReadFloat(
            SiemensRegisterType dbType,
            int dbNumber,
            int dbOffset,
            ref float value,
            out string errText)
        {
            if (!isConnected)
            {
                isConnected = Connect();
            }

            OperateResult<float> resNo;
            lock (_lockObjet)
            {
                string addr = $"{dbType.ToString()}{dbNumber}.{dbOffset}";
                resNo = siemensTcpNet.ReadFloat(addr);
            }

            if (!resNo.IsSuccess)
            {
                errText = resNo.ToMessageShowString();
                _log.Error(
                    $"PLC:[{IPAddress}]读取[{dbType}]" +
                    $"[{dbNumber}]失败，失败信息：" +
                    $"[Message:{errText}");
            }
            else
            {
                value = resNo.Content;
                errText = "读取正常";
            }

            return resNo.ErrorCode;
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

            OperateResult rlt = new OperateResult();
            try
            {
                lock (_lockObjet)
                {
                    string address =
                        $"{dbType.ToString()}{dbNumber}.{tag.DB_Offset}";
                    if (tag is SiemensBoolOfTag)
                    {
                        SiemensBoolOfTag ltag = tag as SiemensBoolOfTag;

                        address += $".{ltag.Position}";
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        if (!ltag.Name.Contains("MES_Heart_Beat"))
                        {
                            _log.Debug(
                              $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                              $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                        }
                    }
                    else if (tag is SiemensByteOfTag)
                    {
                        SiemensByteOfTag ltag = tag as SiemensByteOfTag;
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensWordOfTag)
                    {
                        SiemensWordOfTag ltag = tag as SiemensWordOfTag;
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensIntOfTag)
                    {
                        SiemensIntOfTag ltag = tag as SiemensIntOfTag;
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensDWordOfTag)
                    {
                        SiemensDWordOfTag ltag = tag as SiemensDWordOfTag;
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensRealOfTag)
                    {
                        SiemensRealOfTag ltag = tag as SiemensRealOfTag;
                        rlt = siemensTcpNet.Write(address, ltag.Value);
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                    else if (tag is SiemensArrayCharOfTag)
                    {
                        SiemensArrayCharOfTag ltag = tag as SiemensArrayCharOfTag;
                        rlt =
                            siemensTcpNet.Write(
                                address,
                                Encoding.ASCII.GetBytes(ltag.Value));
                        _log.Debug(
                            $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                            $"Offset:[{tag.DB_Offset}]，写入:[{ltag.Value}]");
                    }
                }
            }
            catch (Exception error)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入时发生错误，{error.Message}");
            }

            if (rlt.ErrorCode != 0)
            {
                throw new Exception(
                    $"PLC:[{IPAddress}]:数据块[{dbNumber}]:Tag[{tag.Name}]:" +
                    $"Offset:[{tag.DB_Offset}]写入失败，错误提示:[{rlt.ErrorCode}]" +
                    $"[{rlt.Message}]");
            }
        }
    }
}