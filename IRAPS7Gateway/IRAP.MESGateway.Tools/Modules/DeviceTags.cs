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
        private UCPLinePropertiesPage pagePLine = null;
        private UCDevicePropertiesPage pageDevice = null;
        private UCDevicesTree trees = null;

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
                    break;
                case "bbiRemoveDevice":
                    break;
                case "bbiNewTagGroup":
                    break;
                case "bbiRemoveTagGroup":
                    break;
                case "bbiNewTagSubGroup":
                    break;
                case "bbiRemoveTagSubGroup":
                    break;
                case "bbiNewTag":
                    break;
                case "bbiRemoveTag":
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
        }

        private void SetObjectProperties(Guid id)
        {
            ucProperties.ShowProperties(DataHelper.Instance.AllEntities[id]);
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
                        for (int i = 0; i < DataHelper.Instance.Lines.Count; i++)
                        {
                            if (DataHelper.Instance.Lines[i].ID == id)
                            {
                                DataHelper.Instance.Lines.Remove(id);
                            }
                        }
                        trees.RemoveNode(node);
                    }
                }
            }
        }

        private void CreatePLinePropertiesPage()
        {
            pagePLine = new UCPLinePropertiesPage();
        }

        private void CreateDevicePropertiesPage()
        {
            pageDevice = new UCDevicePropertiesPage();
        }

        private void ucProperties_Validated(object sender, EventArgs e)
        {
            if (Tag is ProductionLineEntity line)
            {

            }
        }
    }
}
