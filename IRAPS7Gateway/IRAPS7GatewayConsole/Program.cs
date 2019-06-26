//using IRAP.BL.S7Gateway;
//using IRAP.BL.S7Gateway.Entities;
using IRAP.BL.PLCGateway.Siemens.Test;
using Logrila.Logging;
using Logrila.Logging.NLogIntegration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPS7GatewayConsole
{
    class Program
    {
        private static ILog _log;
        private static long plcHandle;

        static void Main(string[] args)
        {
            NLogLogger.Use();
            _log = Logger.Get<Program>();

            //SiemensPLCConnectionTest();

            S7GatewayTest();

            //ReadBufferInThread();
        }

        private static void SiemensPLCConnectionTest()
        {
            SiemensPLC_Test plc = new SiemensPLC_Test()
            {
                //IPAddress = "192.168.11.2",
                //IPAddress = "192.168.87.10",
                IPAddress = "192.168.0.3",
            };
            //plc.AddDevice(new HMETurnTable());
            //plc.AddDevice(new HMEFlash());
            plc.AddDevice(new M0100());

            plc.RackNumber = 0;
            plc.SlotNumber = 0;

            plc.Start();
            Console.ReadLine();
            plc.Stop();
            //int errCode = 12582912;
            //Console.WriteLine($"ErrCode={errCode}|ErrText={plc.GetErrorMsg(errCode)}");
            //Console.ReadLine();
        }

        private static void S7GatewayTest()
        {
            string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}Devices.xml";

            try
            {
                if (IRAP.BL.S7Gateway.PLCContainer.Instance.LoadFromXml(fileName))
                {
                    IRAP.BL.S7Gateway.PLCContainer.Instance.Run();
                }
                Console.ReadLine();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                Console.ReadLine();
            }
        }

        private static void ReadBufferInThread()
        {
            plcHandle = CS7TcpClient.CreatePlc();

            int rlt =
                CS7TcpClient.ConnectPlc(
                    plcHandle,
                    Encoding.ASCII.GetBytes("192.168.0.3"),
                    0,
                    0,
                    false,
                    0,
                    0);
            if (rlt != 0)
            {
                _log.Error(GetErrorMsg(rlt));
                Console.ReadLine();
                return;
            }

            List<ReadBufferInThread> reads = new List<ReadBufferInThread>();
            reads.Add(
                new ReadBufferInThread()
                {
                    //PLCHandle = plcHandle,
                    Name = "29",
                    DBNumber = 29,
                    Offset = 46,
                    Length = 3,
                    SplitterTime = 1000,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    //PLCHandle = plcHandle,
                    Name = "30",
                    DBNumber = 30,
                    Offset = 6,
                    Length = 10,
                    SplitterTime = 1000,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    //PLCHandle = plcHandle,
                    Name = "31",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 1000,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "33",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "32",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "34",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "35",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "36",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "37",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "38",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "39",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });
            reads.Add(
                new IRAPS7GatewayConsole.ReadBufferInThread()
                {
                    Name = "40",
                    DBNumber = 31,
                    Offset = 144,
                    Length = 20,
                    SplitterTime = 100,
                });

            foreach (ReadBufferInThread read in reads)
            {
                read.Do();
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 根据错误代码获取错误提示文本
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        private static string GetErrorMsg(int errorCode)
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

        private static bool Connect()
        {
            int iConnected = 0;
            try
            {
                iConnected =
                    CS7TcpClient.ConnectPlc(
                        plcHandle,
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
                    _log.Error(GetErrorMsg(iConnected));
                    return false;
                }
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
                return false;
            }
        }

        private static void Disconnect()
        {
            CS7TcpClient.DisconnectPlc(plcHandle);
        }
    }
}
