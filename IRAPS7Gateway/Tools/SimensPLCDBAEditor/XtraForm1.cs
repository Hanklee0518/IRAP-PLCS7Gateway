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

namespace SimensPLCDBAEditor
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        public XtraForm1()
        {
            InitializeComponent();
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController serviceController in services)
            {
                imageListBoxControl1.Items.Add(
                    $"{serviceController.ServiceName} {serviceController.Status}",
                    0);
            }
        }
    }
}