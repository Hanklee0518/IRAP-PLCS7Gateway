using Logrila.Logging.NLogIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.DCSGateway.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            NLogLogger.Use();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new servDCSGateway()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
