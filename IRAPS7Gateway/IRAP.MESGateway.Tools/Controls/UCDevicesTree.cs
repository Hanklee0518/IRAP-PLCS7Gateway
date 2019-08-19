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

        public event DataSourceChangedEventHandler OnDataSourceChanged;

        private void InitData()
        {
            tlTrees.Nodes.Clear();
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
                tlnLine.ImageIndex = 0;
                tlnLine.SelectImageIndex = 0;
                pline.Node = tlnLine;

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
                    tlnDevice.ImageIndex = 1;
                    tlnDevice.SelectImageIndex = 1;
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

        private void RefreshMenuItem(TreeListNode node, DataSourceChangedEventArgs args)
        {
            if (node == null)
            {
                MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = true;
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
            }
            else
            {
                if (DataHelper.Instance.AllEntities[args.EntityID] is ProductionLineEntity)
                {
                    MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = true;
                    MenuItemHelper.Instance.Buttons["bbiRemoveProductionLine"].Enabled = !node.HasChildren;
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
                }
                else if (DataHelper.Instance.AllEntities[args.EntityID] is DeviceEntity entity)
                {
                    MenuItemHelper.Instance.Buttons["bbiNewProductionLine"].Enabled = true;
                    MenuItemHelper.Instance.Buttons["bbiRemoveProductionLine"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiNewDevice"].Enabled = true;
                    MenuItemHelper.Instance.Buttons["bbiRemoveDevice"].Enabled = entity.Service.CanDeploy();
                    MenuItemHelper.Instance.Buttons["bbiImportDeviceConfigParams"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiNewTagGroup"].Enabled = true;
                    MenuItemHelper.Instance.Buttons["bbiRemoveTagGroup"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiNewTagSubGroup"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiRemoveTagSubGroup"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiNewTag"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiRemoveTag"].Enabled = false;
                    MenuItemHelper.Instance.Buttons["bbiDeployGatewayService"].Enabled = entity.Service.CanDeploy();
                    MenuItemHelper.Instance.Buttons["bbiUpdateDeviceTags"].Enabled = entity.Service.CanUpdateParams();
                    MenuItemHelper.Instance.Buttons["bbiUninstallGatewayService"].Enabled = !entity.Service.CanDeploy();
                }
            }
        }

        private void RaiseDataSourceChanged(TreeListNode node)
        {
            if (OnDataSourceChanged != null && allowDataSourceChanged)
            {
                DataSourceChangedEventArgs args = new DataSourceChangedEventArgs(node.Tag);

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
                new DataSourceChangedEventArgs(tlTrees.FocusedNode.Tag));
        }

        private void UCDevicesTree_Enter(object sender, EventArgs e)
        {
            RaiseDataSourceChanged(tlTrees.FocusedNode);
        }
    }
}
