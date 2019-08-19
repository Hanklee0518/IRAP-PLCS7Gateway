using IRAP.MESGateway.Tools.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCProductionLinePropertiesPage : BaseControl
    {
        private UCProperties _objectPropertiesView = null;
        private List<DeviceEntity> datas = null;

        public UCProductionLinePropertiesPage()
        {
            InitializeComponent();
        }
        public UCProductionLinePropertiesPage(UCProperties properties) : this()
        {
            _objectPropertiesView = properties;
        }

        private void RaiseDataSourceChanged(BaseEntity entity)
        {
            if (_objectPropertiesView != null)
            {
                _objectPropertiesView.ShowProperties(entity);
            }
        }

        internal void ViewDevices(List<DeviceEntity> datas)
        {
            this.datas = datas;
            grdDevices.DataSource = this.datas;
            grdvDevices.BestFitColumns();
        }

        private void UCProductionLinePropertiesPage_Enter(object sender, EventArgs e)
        {
            MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveProductionLine"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewDevice"].Enabled = true;
            MenuItemHelper.Instance.Buttons["bbiRemoveDevice"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiImportDeviceConfigParams"].Enabled = true;
            MenuItemHelper.Instance.Buttons["bbiNewTagGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTagGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewTagSubGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTagSubGroup"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiRemoveTag"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiDeployGatewayService"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiUpdateDeviceTags"].Enabled = false;
            MenuItemHelper.Instance.Buttons["bbiUninstallGatewayService"].Enabled = false;

            int index = grdvDevices.GetFocusedDataSourceRowIndex();
            if (index >= 0)
            {
                RaiseDataSourceChanged(datas[index]);
            }
            else
            {
                RaiseDataSourceChanged(null);
            }
        }

        private void grdvDevices_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                RaiseDataSourceChanged(datas[e.FocusedRowHandle]);
            }
            else
            {
                RaiseDataSourceChanged(null);
            }
        }
    }
}
