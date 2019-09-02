﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IRAP.MESGateway.Tools.Controls;
using DevExpress.XtraEditors;
using IRAP.MESGateway.Tools.Entities;
using DevExpress.Utils.Menu;
using DevExpress.XtraNavBar;
using DevExpress.XtraTreeList.Nodes;
using System.Xml;
using System.IO;
using IRAP.MESGateway.Tools.Utils;
using DevExpress.XtraSplashScreen;

namespace IRAP.MESGateway.Tools
{
    public partial class DeviceTags : BaseModule
    {
        private UCDevicesTree trees = null;
        private UCProductionLinePropertiesPage lineProp = null;
        private UCDevicePropertiesPage deviceProp = null;

        public DeviceTags()
        {
            InitializeComponent();
        }

        internal override void InitModule(IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            if (trees == null)
            {
                trees = data as UCDevicesTree;
            }
        }

        protected internal override void ButtonClick(MenuItem menuItemName)
        {
            base.ButtonClick(menuItemName);

            try
            {
                switch (menuItemName)
                {
                    case MenuItem.NewProductionLine:
                        NewProductionLine();
                        break;
                    case MenuItem.RemoveProductionLine:
                        RemoveProductionLine();
                        break;
                    case MenuItem.NewDevice:
                        NewDevice();
                        break;
                    case MenuItem.RemoveDevice:
                        RemoveDevice();
                        break;
                    case MenuItem.ImportDeviceConfigParams:
                        ImportDeviceParams();
                        break;
                    case MenuItem.NewTagGroup:
                        NewTagGroup();
                        break;
                    case MenuItem.RemoveTagGroup:
                        RemoveTagGroup();
                        break;
                    case MenuItem.NewSubTagGroup:
                        NewSubTagGroup();
                        break;
                    case MenuItem.RemoveSubTagGroup:
                        RemoveSubTagGroup();
                        break;
                    case MenuItem.NewTag:
                        NewTag();
                        break;
                    case MenuItem.RemoveTag:
                        RemoveTag();
                        break;
                    case MenuItem.GatewayServiceDeploy:
                        RegisterService();
                        break;
                    case MenuItem.GatewayServiceUninstall:
                        UnregisterService();
                        break;
                    case MenuItem.UpdateDeviceTagsToService:
                        if (SplashScreenManager.Default == null)
                        {
                            SplashScreenManager.ShowForm(
                                ParentForm.FindForm(),
                                typeof(Forms.wfMain),
                                false,
                                true);
                        }

                        UpdateDeviceTagsToService();
                        break;
                    case MenuItem.UpdateServiceFile:
                        if (SplashScreenManager.Default == null)
                        {
                            SplashScreenManager.ShowForm(
                                ParentForm.FindForm(),
                                typeof(Forms.wfMain),
                                false,
                                true);
                        }

                        UpdateServiceFile();
                        break;
                }
            }
            finally
            {
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        private void ImportDeviceParams()
        {
            #region 获取设备导入的文件名
            OpenFileDialog dialogOpenFile = new OpenFileDialog()
            {
                Title = "选择导入设备PLC的配置文件",
                Filter = "设备PLC配置文件(*.xml)|*.xml|所有文件(*.*)|*.*",
                CheckFileExists = true,
            };
            string importPath = "";
            if (dialogOpenFile.ShowDialog(FindForm()) == DialogResult.OK)
            {
                importPath = dialogOpenFile.FileName;
            }
            #endregion

            if (File.Exists(importPath))
            {
                TreeListNode node = trees.CurrentNode();
                ProductionLineEntity parent = null;

                if (node.Tag is Guid id)
                {
                    BaseEntity entity = DataHelper.Instance.AllEntities[id];
                    if (entity is ProductionLineEntity line)
                    {
                        parent = line;
                    }
                }

                if (parent != null)
                {
                    List<DeviceEntity> devices = parent.ImportDevice(importPath);
                    foreach (DeviceEntity device in devices)
                    {
                        device.Node = trees.AppendNode(device.Name, device.ID, node);
                    }
                }
            }
        }

        protected internal override void ShowDataChanged(DeviceTreeDataSourceChangedEventArgs args)
        {
            base.ShowDataChanged(args);

            if (args.EntityID != Guid.Empty)
            {
                Tag = args.EntityID;
                SetObjectProperties(args.EntityID);
            }
            else
            {
                SetObjectProperties(new Guid());
            }
        }

        private void SetObjectProperties(Guid id)
        {
            BaseEntity entity = DataHelper.Instance.AllEntities[id];
            ucProperties.ShowProperties(DataHelper.Instance.AllEntities[id]);

            if (entity is ProductionLineEntity line)
            {
                if (lineProp == null)
                {
                    lineProp = new UCProductionLinePropertiesPage(ucProperties);
                }
                ShowDetailObjectsPage(lineProp);

                lineProp.ViewDevices(line.Devices.ToList());
            }
            else if (entity is DeviceEntity device)
            {
                if (deviceProp == null)
                {
                    deviceProp = new UCDevicePropertiesPage(ucProperties);
                }
                ShowDetailObjectsPage(deviceProp);

                deviceProp.ViewTagsInDevice(device);
            }
        }

        private void ShowDetailObjectsPage(BaseControl page)
        {
            pcDevicePage.Controls.Clear();
            pcDevicePage.Controls.Add(page);
            page.Dock = DockStyle.Fill;
        }

        private void NewProductionLine()
        {
            ProductionLineEntity line = new ProductionLineEntity()
            {
                Name = $"Known[{Guid.NewGuid().ToString("N")}]",
            };
            DataHelper.Instance.AllEntities.Add(line);
            DataHelper.Instance.Lines.Add(line, null);

            line.Node = trees.AppendNode(line.Name, line.ID, null);
        }

        private void RemoveProductionLine()
        {
            TreeListNode node = trees.CurrentNode();
            if (node.Tag is Guid id)
            {
                if (DataHelper.Instance.AllEntities[id] is ProductionLineEntity line)
                {
                    if (XtraMessageBox.Show(
                        $"是否要删除产线[{line.Name}]？",
                        "提问",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        DataHelper.Instance.AllEntities.Remove(id);
                        DataHelper.Instance.Lines.Remove(line);
                        trees.RemoveNode(node);
                    }
                }
            }
        }

        private void NewDevice()
        {
            TreeListNode node = trees.CurrentNode();
            TreeListNode parentNode = null;
            ProductionLineEntity parent = null;

            if (node.Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is ProductionLineEntity line)
                {
                    parentNode = node;
                    parent = line;
                }
                else if (entity is DeviceEntity device)
                {
                    parentNode = node.ParentNode;
                    parent = device.Parent;
                }
            }

            if (parentNode != null && parent != null)
            {
                DeviceEntity newDevice = new DeviceEntity(parent);
                parent.Devices.Add(newDevice, null);
                DataHelper.Instance.AllEntities.Add(newDevice);

                newDevice.Node = trees.AppendNode(newDevice.Name, newDevice.ID, parentNode);
            }
        }

        private void RemoveDevice()
        {
            TreeListNode node = trees.CurrentNode();
            if (node.Tag is Guid id)
            {
                if (DataHelper.Instance.AllEntities[id] is DeviceEntity device)
                {
                    if (XtraMessageBox.Show(
                        $"是否要删除设备[{device.Name}]？",
                        "提问",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        device.RemoveChildren();

                        DataHelper.Instance.AllEntities.Remove(id);
                        device.Parent.Devices.Remove(device);
                        trees.RemoveNode(node);
                    }
                }
            }
        }

        private void NewTagGroup()
        {
            TreeListNode node = trees.CurrentNode();
            if (node.Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is DeviceEntity device)
                {
                    GroupEntity newGroup = new GroupEntity(device);
                    device.Groups.Add(newGroup);
                    DataHelper.Instance.AllEntities.Add(newGroup);

                    deviceProp.AppendNode(newGroup, null);
                }
            }
        }

        private void RemoveTagGroup()
        {
            TreeListNode node = deviceProp.CurrentNode();
            BaseEntity entity = null;
            if (node != null)
            {
                if (node.Tag is Guid id)
                {
                    entity = DataHelper.Instance.AllEntities[id];
                }
            }

            if (entity is GroupEntity group)
            {
                if (XtraMessageBox.Show(
                    $"是否要删除[{group.Name}]标记组？",
                    "提问",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    DeviceEntity device = group.Parent;
                    device.Groups.Remove(group);

                    deviceProp.RemoveCurrentNode();
                }
            }
        }

        private void NewSubTagGroup()
        {
            TreeListNode node = deviceProp.CurrentNode();
            if (node != null && node.Tag is Guid id)
            {
                if (DataHelper.Instance.AllEntities[id] is GroupEntity group)
                {
                    SubGroupEntity sgroup = new SubGroupEntity(group);
                    group.SubGroups.Add(sgroup);
                    deviceProp.AppendNode(sgroup, node);
                }
            }
        }

        private void RemoveSubTagGroup()
        {
            TreeListNode node = deviceProp.CurrentNode();
            if (node != null && node.Tag is Guid id)
            {
                if (DataHelper.Instance.AllEntities[id] is SubGroupEntity sgroup)
                {
                    if (XtraMessageBox.Show(
                        $"是否要删除[{sgroup.Prefix}]标记子组？",
                        "提问",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        sgroup.Parent.SubGroups.Remove(sgroup);
                        deviceProp.RemoveCurrentNode();
                    }
                }
            }
        }

        private void NewTag()
        {
            TreeListNode node = deviceProp.CurrentNode();
            if (node != null && node.Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];

                TagEntity newTag = null;
                if (entity is GroupEntity group)
                {
                    newTag = new TagEntity(group);
                    group.Tags.Add(newTag);
                }
                else if (entity is SubGroupEntity sgroup)
                {
                    newTag = new TagEntity(sgroup);
                    sgroup.Tags.Add(newTag);
                }
                else if (entity is TagEntity tag)
                {
                    node = node.ParentNode;
                    if (tag.GroupParent != null)
                    {
                        newTag = new TagEntity(tag.GroupParent);
                        tag.GroupParent.Tags.Add(newTag);
                    }
                    else if (tag.SubGroupParent != null)
                    {
                        newTag = new TagEntity(tag.SubGroupParent);
                        tag.SubGroupParent.Tags.Add(newTag);
                    }
                }

                if (newTag != null)
                {
                    deviceProp.AppendNode(newTag, node);
                }
            }
        }

        private void RemoveTag()
        {
            TreeListNode node = deviceProp.CurrentNode();
            if (node != null && node.Tag is Guid id)
            {
                if (DataHelper.Instance.AllEntities[id] is TagEntity tag)
                {
                    if (XtraMessageBox.Show(
                        $"是否要删除[{tag.Name}]标记？",
                        "提问",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (tag.GroupParent != null)
                        {
                            tag.GroupParent.Tags.Remove(tag);
                        }
                        else
                        {
                            tag.SubGroupParent.Tags.Remove(tag);
                        }

                        deviceProp.RemoveCurrentNode();
                    }
                }
            }
        }

        private void RegisterService()
        {
            DeviceEntity device = null;
            DCSGatewayServiceController service = null;

            if (trees.CurrentNode().Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is DeviceEntity deviceEntity)
                {
                    device = deviceEntity;
                    service = deviceEntity.Service;
                }
                else if (entity is ProductionLineEntity line)
                {
                    if (lineProp == null)
                    {
                        return;
                    }

                    if (lineProp.FocusedDevice == null)
                    {
                        return;
                    }

                    device = lineProp.FocusedDevice;
                    service = lineProp.FocusedDevice.Service;
                }

                if (service != null)
                {
                    try
                    {
                        service.ResetServName();
                        service.Deploy();

                        trees.RefreshTreeNodeStatue();

                        XtraMessageBox.Show(
                            $"设备[{device.Name}]的DCS网关部署成功",
                            "提示",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        trees.ResetMenuItemEnable();
                    }
                    catch (Exception error)
                    {
                        XtraMessageBox.Show(
                            $"部署网关的时候发生错误：{error.Message}",
                            "出错啦",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UnregisterService()
        {
            DeviceEntity device = null;
            DCSGatewayServiceController service = null;

            if (trees.CurrentNode().Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is DeviceEntity deviceEntity)
                {
                    device = deviceEntity;
                    service = device.Service;
                }
                else if (entity is ProductionLineEntity line)
                {
                    if (lineProp == null)
                    {
                        return;
                    }

                    if (lineProp.FocusedDevice == null)
                    {
                        return;
                    }

                    device = lineProp.FocusedDevice;
                    service = lineProp.FocusedDevice.Service;
                }

                if (service != null)
                {
                    try
                    {
                        service.Unregist();
                        trees.RefreshTreeNodeStatue();

                        XtraMessageBox.Show(
                            $"设备[{device.Name}]的DCS网关卸载成功",
                            "提示",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        trees.ResetMenuItemEnable();
                    }
                    catch (Exception error)
                    {
                        XtraMessageBox.Show(
                            $"卸载网关的时候发生错误：{error.Message}",
                            "出错啦",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateDeviceTagsToService()
        {
            DeviceEntity device = null;
            DCSGatewayServiceController service = null;

            if (trees.CurrentNode().Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is DeviceEntity deviceEntity)
                {
                    device = deviceEntity;
                    service = deviceEntity.Service;
                }
                else if (entity is ProductionLineEntity line)
                {
                    if (lineProp == null)
                    {
                        return;
                    }

                    if (lineProp.FocusedDevice == null)
                    {
                        return;
                    }

                    device = lineProp.FocusedDevice;
                    service = lineProp.FocusedDevice.Service;
                }

                if (service != null)
                {
                    try
                    {
                        if (ServiceHelper.Instance.StopService(service.ServName))
                        {
                            service.UpdateDeviceTagsParam();
                        }
                        else
                        {
                            XtraMessageBox.Show(
                                $"无法停止DCS网关服务[{service.ServName}]运行",
                                "提示",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            trees.ResetMenuItemEnable();
                        }

                        ServiceHelper.Instance.StartService(service.ServName);
                    }
                    catch (Exception error)
                    {
                        XtraMessageBox.Show(
                            $"更新设备标签配置文件的时候发生错误：{error.Message}",
                            "出错啦",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateServiceFile()
        {
            DeviceEntity device = null;
            DCSGatewayServiceController service = null;

            if (trees.CurrentNode().Tag is Guid id)
            {
                BaseEntity entity = DataHelper.Instance.AllEntities[id];
                if (entity is DeviceEntity deviceEntity)
                {
                    device = deviceEntity;
                    service = deviceEntity.Service;
                }
                else if (entity is ProductionLineEntity line)
                {
                    if (lineProp == null)
                    {
                        return;
                    }

                    if (lineProp.FocusedDevice == null)
                    {
                        return;
                    }

                    device = lineProp.FocusedDevice;
                    service = lineProp.FocusedDevice.Service;
                }

                if (service != null)
                {
                    try
                    {
                        if (ServiceHelper.Instance.StopService(service.ServName))
                        {
                            service.ResetServName();
                            service.CopyDCSGatewayServiceFile();
                        }
                        else
                        {
                            XtraMessageBox.Show(
                                $"无法停止DCS网关服务[{service.ServName}]运行",
                                "提示",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            trees.ResetMenuItemEnable();
                        }

                        ServiceHelper.Instance.StartService(service.ServName);

                        trees.RefreshTreeNodeStatue();
                    }
                    catch (Exception error)
                    {
                        XtraMessageBox.Show(
                            $"更新DCS网关服务程序文件的时候发生错误：{error.Message}",
                            "出错啦",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

