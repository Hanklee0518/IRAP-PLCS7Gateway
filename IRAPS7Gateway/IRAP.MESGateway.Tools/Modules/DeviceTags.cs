using System;
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

        protected internal override void ButtonClick(string menuItemName)
        {
            base.ButtonClick(menuItemName);

            switch (menuItemName)
            {
                case "bbiNewProductionLine":
                    NewProductionLine();
                    break;
                case "bbiRemoveProductionLine":
                    RemoveProductionLine();
                    break;
                case "bbiNewDevice":
                    NewDevice();
                    break;
                case "bbiRemoveDevice":
                    RemoveDevice();
                    break;
                case "bbiNewTagGroup":
                    NewTagGroup();
                    break;
                case "bbiRemoveTagGroup":
                    RemoveTagGroup();
                    break;
                case "bbiNewTagSubGroup":
                    NewSubTagGroup();
                    break;
                case "bbiRemoveTagSubGroup":
                    RemoveSubTagGroup();
                    break;
                case "bbiNewTag":
                    NewTag();
                    break;
                case "bbiRemoveTag":
                    RemoveTag();
                    break;
            }
        }

        protected internal override void ShowDataChanged(DataSourceChangedEventArgs args)
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
                        MessageBoxDefaultButton.Button2)== DialogResult.Yes)
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
    }
}
