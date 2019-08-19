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
            MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveProductionLine"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewDevice"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveDevice"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiImportDeviceConfigParams"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewTagGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTagGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewTagSubGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTagSubGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTag"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiDeployGatewayService"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiUpdateDeviceTags"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiUninstallGatewayService"].Enabled = false;

            if (obj is ProductionLineEntity)
            {
                MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiRemoveProductionLine"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiNewDevice"].Enabled = true;
            }
            else if (obj is DeviceEntity)
            {
                MenuItemHelper.Instance.Buttons["bbiNewDevice"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiRemoveDevice"].Enabled = true;
            }
            else if (obj is GroupEntity)
            {
                MenuItemHelper.Instance.Buttons["bbiNewTagGroup"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiRemoveTagGroup"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiNewTagSubGroup"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = true;
            }
            else if (obj is SubGroupEntity)
            {
                MenuItemHelper.Instance.Buttons["bbiNewTagSubGroup"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiRemoveTagSubGroup"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = true;
            }
            else if (obj is TagEntity)
            {
                MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = true;
                MenuItemHelper.Instance.Buttons["bbiRemoveTag"].Enabled = true;
            }
        }

        public void ShowProperties(object obj)
        {
            propertyGrid.PropertyGrid.SelectedObject = obj;
            propertyGrid.PropertyGrid.OptionsBehavior.Editable = true;
            propertyGrid.PropertyGrid.FocusFirst();
        }

        private void propertyGrid_Enter(object sender, EventArgs e)
        {
            ShowMenuItemEnabled(propertyGrid.PropertyGrid.SelectedObject);
        }
    }
}
