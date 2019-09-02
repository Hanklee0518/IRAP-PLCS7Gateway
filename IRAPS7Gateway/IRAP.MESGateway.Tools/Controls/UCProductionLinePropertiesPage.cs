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

        internal DeviceEntity FocusedDevice
        {
            get
            {
                int index = grdvDevices.GetFocusedDataSourceRowIndex();
                if (index >=0 && index < datas.Count)
                {
                    return datas[index];
                }
                else
                {
                    return null;
                }
            }
        }

        private void RefreshServiceVersionStatus()
        {
            ParamHelper.Instance.RefreshBaseServiceVersion();

            grdvDevices.BeginDataUpdate();
            grdvDevices.RefreshData();
            grdvDevices.EndDataUpdate();
        }

        private void RaiseDataSourceChanged(BaseEntity entity)
        {
            if (_objectPropertiesView != null)
            {
                _objectPropertiesView.ShowProperties(entity);
            }

            RefreshServiceVersionStatus();

            if (entity == null)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.UpdateServiceFile].Enabled = false;
            }
            else
            {
                DeviceEntity device = entity as DeviceEntity;
                MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = device.Service.CanDeploy;
                MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = device.Service.CanUpdateParams();
                MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = !device.Service.CanDeploy;
                MenuItemHelper.Instance.Buttons[MenuItem.UpdateServiceFile].Enabled = device.Service.CanUpdateParams();
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
            MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = true;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.ImportDeviceConfigParams].Enabled = true;
            MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;

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
