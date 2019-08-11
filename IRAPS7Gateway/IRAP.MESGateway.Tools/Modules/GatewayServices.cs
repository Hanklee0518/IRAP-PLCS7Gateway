using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools
{
    public partial class GatewayServices : BaseModule
    {
        private DataTable dtServices = new DataTable();

        public GatewayServices()
        {
            InitializeComponent();

            InitDTServices();

            tlServices.DataSource = dtServices;
            GetCurrentDCSGatewayServices();
        }

        private void InitDTServices()
        {
            dtServices.Columns.Clear();
            dtServices.Columns.Add("ServiceName", typeof(string));
            dtServices.Columns.Add("ServiceStatus", typeof(int));
        }

        internal void GetCurrentDCSGatewayServices()
        {
            dtServices.Rows.Clear();
            ServiceController[] sControllers = ServiceController.GetServices();
            foreach (ServiceController sController in sControllers)
            {
                if (sController.ServiceName.ToLower().Contains("irap dcsgateway for s7plc"))
                {
                    DataRow dr = dtServices.NewRow();
                    dr["ServiceName"] = sController.ServiceName;
                    dr["ServiceStatus"] = sController.Status;
                    dtServices.Rows.Add(dr);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            tlServices.BestFitColumns();
        }
    }

    internal class DCSGatewayService
    {
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
    }
}
