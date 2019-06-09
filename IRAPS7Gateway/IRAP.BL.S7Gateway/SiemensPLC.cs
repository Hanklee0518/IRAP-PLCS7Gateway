using IRAP.BL.S7Gateway.Entities;
using IRAP.BL.S7Gateway.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace IRAP.BL.S7Gateway
{
    public class SiemensPLC
    {
        private static long plcHandle = 0;

        private byte[] hashBytes = null;
        private byte[] buffer = null;

        private Thread thread = null;
        private bool thread_terminated_single = false;
        private List<TCustomTKDevice> devices = new List<TCustomTKDevice>();
        /// <summary>
        /// 是否连接到 PLC
        /// </summary>
        private bool plc_Connected = false;

        /// <summary>
        /// 当地址块中的内容发生变化时触发
        /// </summary>
        public event DBDataChangedHandler OnAreaDataChanged;

        public SiemensPLC()
        {
            plcHandle = CS7TcpClient.CreatePlc();
        }

        ~SiemensPLC()
        {
            CS7TcpClient.DestoryPlc(ref plcHandle);
        }

        /// <summary>
        /// PLC 的机架号
        /// </summary>
        public int RackNumber { get; set; } = 0;

        /// <summary>
        /// PLC 的插槽号
        /// </summary>
        public int SlotNumber { get; set; } = 0;

        /// <summary>
        /// PLC 的 IP 地址
        /// </summary>
        public string IPAddress { get; set; } = "";

        private byte[] CalculateHash(byte[] buffer)
        {
            HashAlgorithm hash = HashAlgorithm.Create();
            return hash.ComputeHash(buffer);
        }

        private bool Connect()
        {
            int iConnected = 0;
            try
            {
                iConnected =
                    CS7TcpClient.ConnectPlc(
                        plcHandle,
                        Encoding.Default.GetBytes(IPAddress),
                        RackNumber,
                        SlotNumber,
                        false,
                        0,
                        0);

                if (iConnected == 0)
                {
                    plc_Connected = true;
                    return true;
                }
                else
                {
                    plc_Connected = false;
                    return false;
                }
            }
            catch (Exception error)
            {
                plc_Connected = false;
                Console.WriteLine(error.Message);
                return false;
            }
        }

        private void Disconnect()
        {
            CS7TcpClient.DisconnectPlc(plcHandle);
        }

        /// <summary>
        /// 回写到 PLC 中
        /// </summary>
        private void WritebackToPLC(TCustomTKDevice device, CustomTag tag)
        {
            if (tag is BoolOfTag)
            {
                BoolOfTag ltag = tag as BoolOfTag;

                CS7TcpClient.WriteBool(
                    plcHandle,
                    0x84,
                    device.DBNumber,
                    ltag.DB_Offset,
                    ltag.Position,
                    Convert.ToUInt16(ltag.Value));
            }
            else if (tag is ByteOfTag)
            {
                ByteOfTag ltag = tag as ByteOfTag;

                CS7TcpClient.WriteByte(
                    plcHandle,
                    0x84,
                    device.DBNumber,
                    ltag.DB_Offset,
                    ltag.Value);
            }
            else if (tag is WordOfTag)
            {
                WordOfTag ltag = tag as WordOfTag;

                CS7TcpClient.WriteWord(
                    plcHandle,
                    0x84,
                    device.DBNumber,
                    ltag.DB_Offset,
                    (uint)ltag.Value);
            }
            else if (tag is DWordOfTag)
            {
                DWordOfTag ltag = tag as DWordOfTag;

                CS7TcpClient.WriteDWord(
                    plcHandle,
                    0x84,
                    device.DBNumber,
                    ltag.DB_Offset,
                    (uint)ltag.Value);
            }
            else if (tag is ArrayCharOfTag)
            {
                ArrayCharOfTag ltag = tag as ArrayCharOfTag;

                string temp = ltag.Value.PadRight(ltag.Length, ' ').Substring(0, ltag.Length);
                CS7TcpClient.WriteBlockAsByte(
                    plcHandle,
                    0x84,
                    device.DBNumber,
                    ltag.DB_Offset,
                    ltag.Length,
                    Encoding.Default.GetBytes(temp));
            }
        }

        protected string ReadString(TCustomTKDevice device, ArrayCharOfTag tag)
        {
            byte[] buffer = new byte[tag.Length];
            int resNo =
                CS7TcpClient.ReadString(
                    plcHandle,
                    (int)tag.DB_Type,
                    device.DBNumber,
                    tag.DB_Offset,
                    (short)tag.Length,
                    buffer);

            return Encoding.Default.GetString(buffer);
        }

        private void Do()
        {
            if (!Connect())
            {
                return;
            }

            while (!thread_terminated_single)
            {
                int resNo = 0;

                for (int i = 0; i < devices.Count; i++)
                {
                    buffer = new byte[devices[i].BufferSize];

                    try
                    {
                        resNo =
                            CS7TcpClient.ReadBlockAsByte(
                                plcHandle,
                                0x84,
                                devices[i].DBNumber,
                                0,
                                buffer.Length,
                                buffer);

                        devices[i].DoSomething(buffer);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.Message);
                    }

                    if (devices[i].COMM.MES_Heart_Beat.Used)
                    {
                        DateTime now = DateTime.Now;
                        DateTime lastCallTime = devices[i].LastMESHeartBeatTime;
                        if ((now - lastCallTime).TotalMilliseconds >= 2000)
                        {
                            devices[i].LastMESHeartBeatTime = now;
                            //devices[i].COMM.MES_Heart_Beat.Value =
                            //    !(devices[i].COMM.MES_Heart_Beat.Value);
                            CS7TcpClient.WriteBool(
                                plcHandle,
                                0x84,
                                devices[i].DBNumber,
                                devices[i].COMM.MES_Heart_Beat.DB_Offset,
                                devices[i].COMM.MES_Heart_Beat.Position,
                                Convert.ToUInt16(!devices[i].COMM.MES_Heart_Beat.Value));
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void Start()
        {
            if (thread == null)
                thread = new Thread(Do);
            thread.Start();
        }

        public void Stop()
        {
            thread_terminated_single = true;
        }

        public void AddDevice(TCustomTKDevice device)
        {
            device.OnWriteback += new S7WriteBackHandler(WritebackToPLC);
            devices.Add(device);
        }
    }
}