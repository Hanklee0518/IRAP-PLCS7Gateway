using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ServiceProcess;

namespace IRAP.MESGateway.Tools.Forms
{
    public partial class GatewayServicesForm : XtraForm
    {
        private DataTable dtServices = new DataTable();

        public GatewayServicesForm()
        {
            InitializeComponent();

            InitDTServices();
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

        private void GatewayServicesForm_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            tlServices.DataSource = dtServices;
            GetCurrentDCSGatewayServices();
            tlServices.BestFitColumns(false);
        }
    }

    internal class DCSGatewayService
    {
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
    }
}