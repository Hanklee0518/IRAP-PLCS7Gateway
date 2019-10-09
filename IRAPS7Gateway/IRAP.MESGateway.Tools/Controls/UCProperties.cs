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
using IRAP.MESGateway.Tools.Entities;

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

        private void ShowMenuItemEnabled(object obj)
        {
            MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.ImportDeviceConfigParams].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = false;

            if (obj is ProductionLineEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = true;
            }
            else if (obj is DeviceEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = true;
            }
            else if (obj is GroupEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
            }
            else if (obj is SubGroupEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
            }
            else if (obj is TagEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = true;
            }
        }

        public void ShowProperties(object obj)
        {
            propertyGrid.PropertyGrid.SelectedObject = obj;
            propertyGrid.PropertyGrid.OptionsBehavior.Editable = true;
            propertyGrid.PropertyGrid.FocusFirst();
            propertyGrid.PropertyGrid.ExpandAllRows();
        }

        private void propertyGrid_Enter(object sender, EventArgs e)
        {
            ShowMenuItemEnabled(propertyGrid.PropertyGrid.SelectedObject);
        }
    }
}
