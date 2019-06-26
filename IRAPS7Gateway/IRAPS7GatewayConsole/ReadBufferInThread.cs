using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRAPS7GatewayConsole
{
    internal class ReadBufferInThread
    {
        private ILog _log = Logger.Get<ReadBufferInThread>();

        public string Name { get; set; }
        public long PLCHandle { get; set; }
        public uint DBNumber { get; set; }
        public uint Offset { get; set; }
        public int Length { get; set; }
        public int SplitterTime { get; set; }
        public bool Ternimated { get; set; } = false;

        public ReadBufferInThread()
        {
            PLCHandle = CS7TcpClient.CreatePlc();
        }

        /// <summary>
        /// 根据错误代码获取错误提示文本
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        private string GetErrorMsg(int errorCode)
        {
            byte[] errText = new byte[1024];
            int resNo = CS7TcpClient.GetErrorMsg(errorCode, errText, errText.Length);
            if (resNo == 0)
            {
                return Encoding.Default.GetString(errText);
            }
            else
            {
                return $"ResNo={resNo}";
            }
        }

        private void ReadBuffer()
        {
            bool connected = false;
            try
            {
                byte[] buffer = new byte[Length];

                while (!Ternimated)
                {
                    if (!connected)
                    {
                        connected = Connect();
                    }

                    if (connected)
                    {
                        try
                        {
                            int resNo =
                                CS7TcpClient.ReadBlockAsByte(
                                    PLCHandle,
                                    0x84,
                                    DBNumber,
                                    Offset,
                                    Length,
                                    buffer);
                            if (resNo != 0)
                            {
                                _log.Error($"[{Name}][{resNo}]={GetErrorMsg(resNo)}");
                            }
                            else
                            {
                                _log.Trace($"[{Name}]读到数据：[{Encoding.ASCII.GetString(buffer)}]");
                            }
                        }
                        catch (Exception error)
                        {
                            _log.Error($"[{Name}]{error.Message}", error);
                        }
                    }

                    Thread.Sleep(SplitterTime);
                }
            }
            finally
            {
                if (connected)
                    Disconnect();
            }
        }

        private bool Connect()
        {
            int iConnected = 0;
            try
            {
                iConnected =
                    CS7TcpClient.ConnectPlc(
                        PLCHandle,
                        Encoding.Default.GetBytes("192.168.0.3"),
                        0,
                        0,
                        false,
                        0,
                        0);

                if (iConnected == 0)
                {
                    return true;
                }
                else
                {
                    _log.Error($"[{Name}]{GetErrorMsg(iConnected)}");
                    return false;
                }
            }
            catch (Exception error)
            {
                _log.Error($"[{Name}]{error.Message}", error);
                return false;
            }
        }

        private void Disconnect()
        {
            CS7TcpClient.DisconnectPlc(PLCHandle);
        }

        public void Do()
        {
            Thread thread = new Thread(ReadBuffer);
            thread.Start();
        }
    }
}
