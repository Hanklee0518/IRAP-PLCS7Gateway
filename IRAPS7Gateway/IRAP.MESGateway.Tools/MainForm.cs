﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using IRAP.MESGateway.Tools.Forms;
using IRAP.MESGateway.Tools.Entities;
using System.IO;

namespace IRAP.MESGateway.Tools
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private ModulesNavigator modulesNavigator;
        private string dbPath = "";

        public MainForm()
        {
            dbPath = $"{AppDomain.CurrentDomain.BaseDirectory}Datas.xml";
            DataHelper.Instance.Load(dbPath);

            InitializeComponent();

            Text = ParamHelper.AppTitle;
            lblAboutAppTitle.Text = $"{ParamHelper.AppTitle}\n\nVersion:1.0.0.0";

            InitNavBarGroups();
            RibbonButtonsInitialize();

            modulesNavigator = new ModulesNavigator(ribbon, pcMain);
            modulesNavigator.ChangeGroup(
                navBarControl1.ActiveGroup,
                GetModuleData(
                    (NavBarGroupTagObject)navBarControl1.ActiveGroup.Tag));
        }

        private void RibbonButtonsInitialize()
        {
            InitBarButtonItem(bbiNewProductionLine, null, "");
            InitBarButtonItem(bbiNewDevice, null, "");
            InitBarButtonItem(bbiNewTagGroup, null, "");
            InitBarButtonItem(bbiNewTagSubGroup, null, "");
            InitBarButtonItem(bbiNewTag, null, "");
            InitBarButtonItem(bbiRemoveProductionLine, null, "");
            InitBarButtonItem(bbiRemoveDevice, null, "");
            InitBarButtonItem(bbiRemoveTagGroup, null, "");
            InitBarButtonItem(bbiRemoveTagSubGroup, null, "");
            InitBarButtonItem(bbiRemoveTag, null, "");
            InitBarButtonItem(bbiImportDeviceConfigParams, null, "");
        }

        private void InitBarButtonItem(BarButtonItem item, object tag, string description)
        {
            MenuItemHelper.Instance.Buttons.Add(item);
            item.ItemClick += new ItemClickEventHandler(bbiGeneralItemClick);
            item.Hint = description;
            item.Tag = tag;
        }

        private void bbiGeneralItemClick(object sender, ItemClickEventArgs e)
        {
            modulesNavigator.CurrentModule.ButtonClick(e.Item.Name);
        }

        private void InitNavBarGroups()
        {
            nbgDevices.Tag = new NavBarGroupTagObject("Devices", typeof(DeviceTags));
            nbgServices.Tag = new NavBarGroupTagObject("Services", typeof(GatewayServices));
        }

        private object GetModuleData(NavBarGroupTagObject tag)
        {
            if (tag == null) return null;
            if (tag.ModuleType == typeof(DeviceTags)) return ucDevices;
            if (tag.ModuleType == typeof(GatewayServices)) return nbgServices;
            return null;
        }

        private void bbiServicesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (Form frm in MdiChildren)
            {
                if (frm is GatewayServicesForm)
                {
                    frm.Show();
                    return;
                }
            }

            GatewayServicesForm form = new GatewayServicesForm
            {
                MdiParent = this,
            };
            form.Show();
            form.Enabled = true;
        }

        private void navBarControl1_ActiveGroupChanged(object sender, DevExpress.XtraNavBar.NavBarGroupEventArgs e)
        {
            object data = GetModuleData((NavBarGroupTagObject)e.Group.Tag);
            modulesNavigator.ChangeGroup(e.Group, data);
        }

        private void ucDevices_OnDataSourceChanged(object sender, ref DataSourceChangedEventArgs e)
        {
            modulesNavigator.CurrentModule.ShowDataChanged(e);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataHelper.Instance.Save(dbPath);
        }

        private void bvbiQuit_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            Close();
        }

        private void bvControl_Showing(object sender, EventArgs e)
        {
            bvControl.SelectedTabIndex = 0;

            ucOptions.InitOptionItems();
        }

        private void bvtiOptions_ItemPressed(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e)
        {
            ucOptions.InitOptionItems();
        }
    }
}