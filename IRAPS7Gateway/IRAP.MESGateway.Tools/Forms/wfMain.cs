using DevExpress.Utils;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools.Forms
{
    public partial class wfMain : DemoWaitForm
    {
        public wfMain()
        {
            LocalizationHelper.SetCurrentCulture(ParamHelper.ApplicationArguments);

            InitializeComponent();

            ProgressPanel.Caption = Tools.Properties.Resources.ProgressPanelCaption;
            ProgressPanel.Description = Tools.Properties.Resources.ProgressPanelDescription;
        }
    }
}
