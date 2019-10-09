//using IRAP.BL.S7Gateway;
//using IRAP.BL.S7Gateway.Entities;
using Logrila.Logging;
using Logrila.Logging.NLogIntegration;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
