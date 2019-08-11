using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCProperties : XtraUserControl
    {
        public UCProperties()
        {
            InitializeComponent();

            propertyGrid.PropertyGrid.AutoGenerateRows = true;
            propertyGrid.PropertyGrid.SelectedObject = null;
        }

        public void ShowProperties(object obj)
        {
            propertyGrid.PropertyGrid.SelectedObject = obj;
            propertyGrid.PropertyGrid.OptionsBehavior.Editable = true;
        }
    }
}
