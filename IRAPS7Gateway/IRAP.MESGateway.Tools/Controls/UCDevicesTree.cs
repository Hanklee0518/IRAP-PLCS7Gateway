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
using DevExpress.Utils.Design;
using IRAP.MESGateway.Tools.Entities;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraTreeList;
using IRAP.MESGateway.Tools.Utils;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCDevicesTree : BaseControl
    {
        private bool allowDataSourceChanged = false;

        public UCDevicesTree()
        {
            InitializeComponent();

            InitData();

            tlTrees.RowHeight =
                Math.Max(
                    Convert.ToInt32(tlTrees.Font.GetHeight()),
                    imageCollection.ImageSize.Height) + 4;
        }

        public event DeviceTreeDataSourceChangedEventHandler OnDataSourceChanged;

        private void InitData()
        {
            tlTrees.Nodes.Clear();
            DataHelper.Instance.Lines.Sort();
            foreach (ProductionLineEntity pline in DataHelper.Instance.Lines)
            {
                TreeListNode tlnLine =
                    tlTrees.AppendNode(
                        new object[]
                        {
                            $"{pline.Name}",
                        },
                        null);
                tlnLine.Tag = pline.ID;
                tlnLine.ImageIndex = (int)DeviceTreeNodeImage.ProductionLine;
                tlnLine.SelectImageIndex = (int)DeviceTreeNodeImage.ProductionLine;
                pline.Node = tlnLine;

                pline.Devices.Sort();
                foreach (DeviceEntity device in pline.Devices)
                {
                    TreeListNode tlnDevice =
                        tlTrees.AppendNode(
                            new object[]
                            {
                                $"{device.Name}",
                            },
                            tlnLine);
                    tlnDevice.Tag = device.ID;
                    tlnDevice.ImageIndex = (int)DeviceTreeNodeImage.Device;
                    tlnDevice.SelectImageIndex = (int)DeviceTreeNodeImage.Device;
                    device.Node = tlnDevice;
                }
            }

            tlTrees.ExpandAll();
            if (!DesignTimeTools.IsDesignMode)
            {

            }
            allowDataSourceChanged = true;
        }

        private void SetFocusedColor(Skin skin)
        {
            if ("|Office 2016 Colorful|".IndexOf(skin.Name) > 0)
            {
                tlTrees.Appearance.FocusedRow.BackColor =
                    skin.Colors.GetColor("HideSelection");
            }
            else
            {
                tlTrees.Appearance.FocusedRow.BackColor =
                    skin.Colors.GetColor("Highlight");
            }
        }

        private void RefreshMenuItem(TreeListNode node, DeviceTreeDataSourceChangedEventArgs args)
        {
            if (node == null)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = true;
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
                MenuItemHelper.Instance.Buttons[MenuItem.UpdateServiceFile].Enabled = false;
            }
            else
            {
                if (DataHelper.Instance.AllEntities[args.EntityID] is ProductionLineEntity)
                {
                    MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = !node.HasChildren;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.ImportDeviceConfigParams].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.UpdateServiceFile].Enabled = false;
                }
                else if (DataHelper.Instance.AllEntities[args.EntityID] is DeviceEntity entity)
                {
                    MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = entity.Service.CanDeploy;
                    MenuItemHelper.Instance.Buttons[MenuItem.ImportDeviceConfigParams].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = true;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
                    MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = entity.Service.CanDeploy;
                    MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = entity.Service.CanUpdateParams();
                    MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = !entity.Service.CanDeploy;
                    MenuItemHelper.Instance.Buttons[MenuItem.UpdateServiceFile].Enabled = entity.Service.CanUpdateParams();
                }
            }

            RefreshTreeNodeStatue();
        }

        private void RaiseDataSourceChanged(TreeListNode node)
        {
            if (OnDataSourceChanged != null && allowDataSourceChanged)
            {
                DeviceTreeDataSourceChangedEventArgs args = new DeviceTreeDataSourceChangedEventArgs(node.Tag);

                OnDataSourceChanged(tlTrees, ref args);
                RefreshMenuItem(node, args);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignTimeTools.IsDesignMode)
            {
                if (tlTrees.Nodes.Count > 0)
                    tlTrees.FocusedNode = tlTrees.Nodes[0];
            }
        }

        protected override void LookAndFeelStyleChanged()
        {
            base.LookAndFeelStyleChanged();

            Color controlColor =
                CommonSkins.GetSkin(UserLookAndFeel.Default)
                .Colors.GetColor("Control");
            tlTrees.Appearance.Empty.BackColor = controlColor;
            tlTrees.Appearance.Row.BackColor = controlColor;
            SetFocusedColor(CommonSkins.GetSkin(UserLookAndFeel.Default));
        }

        private void tlTrees_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            RaiseDataSourceChanged(e.Node);
        }

        public void UpdateCurrentNodeText(string nodeText)
        {
            if (tlTrees.FocusedNode != null)
            {
                tlTrees.FocusedNode.SetValue(0, nodeText);
            }
        }

        public TreeListNode AppendNode(string nodeText, Guid tag, TreeListNode parent)
        {
            TreeListNode newNode =
                tlTrees.AppendNode(
                    new object[]
                    {
                        nodeText,
                    },
                    parent);
            newNode.Tag = tag;
            BaseEntity entity = DataHelper.Instance.AllEntities[tag];
            if (entity is ProductionLineEntity)
            {
                newNode.ImageIndex = 0;
                newNode.SelectImageIndex = 0;
            }
            else if (entity is DeviceEntity)
            {
                newNode.ImageIndex = 1;
                newNode.SelectImageIndex = 1;
            }

            tlTrees.FocusedNode = newNode;

            return newNode;
        }

        public void RemoveNode(TreeListNode node)
        {
            tlTrees.Nodes.Remove(node);
        }

        public TreeListNode CurrentNode()
        {
            return tlTrees.FocusedNode;
        }

        public void ResetMenuItemEnable()
        {
            RefreshMenuItem(
                tlTrees.FocusedNode,
                new DeviceTreeDataSourceChangedEventArgs(tlTrees.FocusedNode.Tag));
        }

        public void RefreshTreeNodeStatue()
        {
            foreach (TreeListNode node in tlTrees.Nodes)
            {
                if (node.HasChildren)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        if (childNode.Tag is Guid guid)
                        {
                            if (DataHelper.Instance.AllEntities[guid] is DeviceEntity device)
                            {
                                if (device.Service.CanDeploy)
                                {
                                    childNode.StateImageIndex = 
                                        (int)DeviceTreeNodeStateImage.Uninstalled;
                                }
                                else
                                {
                                    if (device.Service.NeedUpgradeServiceExecute())
                                    {
                                        childNode.StateImageIndex =
                                            (int)DeviceTreeNodeStateImage.NeedUpgrade;
                                    }
                                    else
                                    {
                                        childNode.StateImageIndex = 
                                            (int)DeviceTreeNodeStateImage.Installed;
                                    }
                                }
                            }
                            else
                            {
                                childNode.StateImageIndex = 
                                    (int)DeviceTreeNodeStateImage.Uninstalled;
                            }
                        }
                        else
                        {
                            childNode.StateImageIndex = 
                                (int)DeviceTreeNodeStateImage.Uninstalled;
                        }
                    }
                }
            }
        }

        private void UCDevicesTree_Enter(object sender, EventArgs e)
        {
            RaiseDataSourceChanged(tlTrees.FocusedNode);
        }

        private void tlTrees_Enter(object sender, EventArgs e)
        {
            RefreshTreeNodeStatue();
        }
    }

    enum DeviceTreeNodeImage
    {
        ProductionLine = 0,
        Device,
    }

    enum DeviceTreeNodeStateImage
    {
        Uninstalled = 2,
        Installed = 3,
        NeedUpgrade = 4,
    }
}
