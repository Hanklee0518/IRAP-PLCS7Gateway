using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using IRAP.MESGateway.Tools.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCDevicePropertiesPage : BaseControl
    {
        private UCProperties _objectPropertiesView = null;

        public UCDevicePropertiesPage()
        {
            InitializeComponent();
        }
        public UCDevicePropertiesPage(UCProperties properties) : this()
        {
            _objectPropertiesView = properties;
        }

        private void RaiseDataSourceChanged(BaseEntity entity)
        {
            if (_objectPropertiesView != null)
            {
                _objectPropertiesView.ShowProperties(entity);
            }

            MenuItemHelper.Instance.Buttons[MenuItem.NewProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveProductionLine].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewDevice].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.RemoveDevice].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.ImportDeviceConfigParams].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.NewTagGroup].Enabled = true;
            MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceDeploy].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.UpdateDeviceTagsToService].Enabled = false;
            MenuItemHelper.Instance.Buttons[MenuItem.GatewayServiceUninstall].Enabled = false;
            if (entity == null)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
            }
            else if (entity is GroupEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
            }
            else if (entity is SubGroupEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = false;
            }
            else if (entity is TagEntity)
            {
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewSubTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveSubTagGroup].Enabled = false;
                MenuItemHelper.Instance.Buttons[MenuItem.NewTag].Enabled = true;
                MenuItemHelper.Instance.Buttons[MenuItem.RemoveTag].Enabled = true;
            }
        }

        internal void ViewTagsInDevice(DeviceEntity device)
        {
            if (SplashScreenManager.Default == null)
            {
                SplashScreenManager.ShowForm(
                    FindForm(),
                    typeof(Forms.wfMain),
                    false,
                    true);
            }

            tlTags.Nodes.Clear();
            tlTags.BeginUpdate();
            foreach (GroupEntity group in device.Groups)
            {
                TreeListNode parent = AppendNode(group, null);
                foreach (TagEntity tag in group.Tags)
                {
                    AppendNode(tag, parent);
                }
                foreach (SubGroupEntity sgroup in group.SubGroups)
                {
                    TreeListNode sparent = AppendNode(sgroup, parent);
                    foreach (TagEntity tag in sgroup.Tags)
                    {
                        AppendNode(tag, sparent);
                    }
                }
            }
            tlTags.ExpandAll();
            tlTags.BestFitColumns();
            if (tlTags.Nodes.Count > 0)
                tlTags.FocusedNode = tlTags.Nodes[0];
            tlTags.EndUpdate();

            if (SplashScreenManager.Default != null)
            {
                SplashScreenManager.CloseForm();
            }
        }

        internal TreeListNode AppendNode(BaseEntity entity, TreeListNode parent)
        {
            TreeListNode newNode = null;
            if (entity is GroupEntity group)
            {
                newNode =
                  tlTags.AppendNode(
                      new object[]
                      {
                        $"{group.Name}",
                      },
                      parent);
                newNode.Tag = group.ID;
                group.Node = newNode;
            }
            else if (entity is SubGroupEntity sgroup)
            {
                newNode =
                    tlTags.AppendNode(
                        new object[]
                        {
                            sgroup.Prefix,
                        },
                        parent);
                newNode.Tag = sgroup.ID;
                sgroup.Node = newNode;
            }
            else if (entity is TagEntity tag)
            {
                newNode =
                    tlTags.AppendNode(
                        new object[]
                        {
                            $"{tag.Name}",
                            "",
                            EnumHelper.GetEnumDescription(tag.DataType),
                            EnumHelper.GetEnumDescription(tag.Type),
                            tag.Offset,
                            tag.Length,
                        },
                        parent);
                newNode.Tag = tag.ID;
                tag.Node = newNode;
            }

            tlTags.FocusedNode = newNode;
            return newNode;
        }

        internal void RemoveCurrentNode()
        {
            if (tlTags.FocusedNode != null)
            {
                tlTags.Nodes.Remove(tlTags.FocusedNode);
            }
        }

        internal TreeListNode CurrentNode()
        {
            return tlTags.FocusedNode;
        }

        private void tlTags_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            BaseEntity entity = null;
            if (e.Node == null)
            {
                return;
            }

            if (e.Node.Tag is Guid id)
            {
                entity = DataHelper.Instance.AllEntities[id];
                RaiseDataSourceChanged(entity);
            }
        }

        private void UCDevicePropertiesPage_Enter(object sender, EventArgs e)
        {
            if (tlTags.FocusedNode != null && tlTags.FocusedNode.Tag is Guid id)
            {
                RaiseDataSourceChanged(DataHelper.Instance.AllEntities[id]);
            }
            else
            {
                RaiseDataSourceChanged(null);
            }
        }
    }
}
