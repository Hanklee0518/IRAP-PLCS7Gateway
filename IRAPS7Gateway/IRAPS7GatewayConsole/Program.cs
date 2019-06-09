using IRAP.BL.S7Gateway;
using IRAP.BL.S7Gateway.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPS7GatewayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SiemensPLC plc = new SiemensPLC()
            {
                IPAddress = "192.168.0.3",
            };
            plc.AddDevice(new HMETurnTable());
            plc.AddDevice(new HMEFlash());

            plc.Start();
            Console.ReadLine();
            plc.Stop();
        }
    }
}
