namespace IRAP.MESGateway.Tools
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.icSmall = new DevExpress.Utils.ImageCollection(this.components);
            this.skinDropDownButtonItem1 = new DevExpress.XtraBars.SkinDropDownButtonItem();
            this.bbiRefreshServiceList = new DevExpress.XtraBars.BarButtonItem();
            this.bbiStartService = new DevExpress.XtraBars.BarButtonItem();
            this.bbiStopService = new DevExpress.XtraBars.BarButtonItem();
            this.bbiServicesList = new DevExpress.XtraBars.BarButtonItem();
            this.skinPaletteRibbonGalleryBarItem1 = new DevExpress.XtraBars.SkinPaletteRibbonGalleryBarItem();
            this.bbiNewDevice = new DevExpress.XtraBars.BarButtonItem();
            this.bbiNewProductionLine = new DevExpress.XtraBars.BarButtonItem();
            this.bbiRemoveProductionLine = new DevExpress.XtraBars.BarButtonItem();
            this.bbiRemoveDevice = new DevExpress.XtraBars.BarButtonItem();
            this.bbiNewTagGroup = new DevExpress.XtraBars.BarButtonItem();
            this.bbiRemoveTagGroup = new DevExpress.XtraBars.BarButtonItem();
            this.bbiNewTagSubGroup = new DevExpress.XtraBars.BarButtonItem();
            this.bbiRemoveTagSubGroup = new DevExpress.XtraBars.BarButtonItem();
            this.bbiNewTag = new DevExpress.XtraBars.BarButtonItem();
            this.bbiRemoveTag = new DevExpress.XtraBars.BarButtonItem();
            this.icLarge = new DevExpress.Utils.ImageCollection(this.components);
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rgpPLine = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgDevice = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgTagGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.tpgTagSubGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgTag = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpServices = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgServiceList = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpView = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgAppearance = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.nbgDevices = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.ucDevices = new IRAP.MESGateway.Tools.Controls.UCDevicesTree();
            this.nbgServices = new DevExpress.XtraNavBar.NavBarGroup();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.pcMain = new DevExpress.XtraEditors.PanelControl();
            this.bbiImportDeviceConfigParams = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icSmall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icLarge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcMain)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Images = this.icSmall;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.skinDropDownButtonItem1,
            this.bbiRefreshServiceList,
            this.bbiStartService,
            this.bbiStopService,
            this.bbiServicesList,
            this.skinPaletteRibbonGalleryBarItem1,
            this.bbiNewDevice,
            this.bbiNewProductionLine,
            this.bbiRemoveProductionLine,
            this.bbiRemoveDevice,
            this.bbiNewTagGroup,
            this.bbiRemoveTagGroup,
            this.bbiNewTagSubGroup,
            this.bbiRemoveTagSubGroup,
            this.bbiNewTag,
            this.bbiRemoveTag,
            this.bbiImportDeviceConfigParams});
            this.ribbon.LargeImages = this.icLarge;
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 22;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1,
            this.rpServices,
            this.rpView});
            this.ribbon.QuickToolbarItemLinks.Add(this.skinDropDownButtonItem1);
            this.ribbon.Size = new System.Drawing.Size(976, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // icSmall
            // 
            this.icSmall.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("icSmall.ImageStream")));
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.add_16x16, "add_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 0);
            this.icSmall.Images.SetKeyName(0, "add_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.additem_16x16, "additem_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 1);
            this.icSmall.Images.SetKeyName(1, "additem_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.cancel_16x16, "cancel_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 2);
            this.icSmall.Images.SetKeyName(2, "cancel_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletelist_16x16, "deletelist_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 3);
            this.icSmall.Images.SetKeyName(3, "deletelist_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.insert_16x16, "insert_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 4);
            this.icSmall.Images.SetKeyName(4, "insert_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.removeitem_16x16, "removeitem_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 5);
            this.icSmall.Images.SetKeyName(5, "removeitem_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.addnewdatasource_16x16, "addnewdatasource_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 6);
            this.icSmall.Images.SetKeyName(6, "addnewdatasource_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletedatasource_16x16, "deletedatasource_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 7);
            this.icSmall.Images.SetKeyName(7, "deletedatasource_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.addgroupfooter_16x16, "addgroupfooter_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 8);
            this.icSmall.Images.SetKeyName(8, "addgroupfooter_16x16");
            this.icSmall.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletegroupfooter_16x16, "deletegroupfooter_16x16", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 9);
            this.icSmall.Images.SetKeyName(9, "deletegroupfooter_16x16");
            this.icSmall.Images.SetKeyName(10, "import1_16px_31024_easyicon.net.png");
            // 
            // skinDropDownButtonItem1
            // 
            this.skinDropDownButtonItem1.Id = 2;
            this.skinDropDownButtonItem1.Name = "skinDropDownButtonItem1";
            // 
            // bbiRefreshServiceList
            // 
            this.bbiRefreshServiceList.Caption = "刷新";
            this.bbiRefreshServiceList.Id = 4;
            this.bbiRefreshServiceList.ImageOptions.LargeImage = global::IRAP.MESGateway.Tools.Properties.Resources.refresh_32x32;
            this.bbiRefreshServiceList.Name = "bbiRefreshServiceList";
            // 
            // bbiStartService
            // 
            this.bbiStartService.Caption = "启动服务";
            this.bbiStartService.Id = 5;
            this.bbiStartService.ImageOptions.LargeImage = global::IRAP.MESGateway.Tools.Properties.Resources.next_32x32;
            this.bbiStartService.Name = "bbiStartService";
            // 
            // bbiStopService
            // 
            this.bbiStopService.Caption = "停止服务";
            this.bbiStopService.Id = 6;
            this.bbiStopService.ImageOptions.LargeImage = global::IRAP.MESGateway.Tools.Properties.Resources.selectall_32x32;
            this.bbiStopService.Name = "bbiStopService";
            // 
            // bbiServicesList
            // 
            this.bbiServicesList.Caption = "服务列表";
            this.bbiServicesList.Id = 7;
            this.bbiServicesList.ImageOptions.LargeImage = global::IRAP.MESGateway.Tools.Properties.Resources.version_32x32;
            this.bbiServicesList.Name = "bbiServicesList";
            this.bbiServicesList.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiServicesList_ItemClick);
            // 
            // skinPaletteRibbonGalleryBarItem1
            // 
            this.skinPaletteRibbonGalleryBarItem1.Caption = "skinPaletteRibbonGalleryBarItem1";
            this.skinPaletteRibbonGalleryBarItem1.Id = 10;
            this.skinPaletteRibbonGalleryBarItem1.Name = "skinPaletteRibbonGalleryBarItem1";
            // 
            // bbiNewDevice
            // 
            this.bbiNewDevice.Caption = "新设备";
            this.bbiNewDevice.Id = 11;
            this.bbiNewDevice.ImageOptions.ImageIndex = 1;
            this.bbiNewDevice.ImageOptions.LargeImage = global::IRAP.MESGateway.Tools.Properties.Resources.additem_32x32;
            this.bbiNewDevice.Name = "bbiNewDevice";
            // 
            // bbiNewProductionLine
            // 
            this.bbiNewProductionLine.Caption = "新产线";
            this.bbiNewProductionLine.Id = 12;
            this.bbiNewProductionLine.ImageOptions.ImageIndex = 0;
            this.bbiNewProductionLine.ImageOptions.LargeImageIndex = 7;
            this.bbiNewProductionLine.Name = "bbiNewProductionLine";
            // 
            // bbiRemoveProductionLine
            // 
            this.bbiRemoveProductionLine.Caption = "删除产线";
            this.bbiRemoveProductionLine.Id = 13;
            this.bbiRemoveProductionLine.ImageOptions.ImageIndex = 2;
            this.bbiRemoveProductionLine.ImageOptions.LargeImageIndex = 8;
            this.bbiRemoveProductionLine.Name = "bbiRemoveProductionLine";
            // 
            // bbiRemoveDevice
            // 
            this.bbiRemoveDevice.Caption = "删除设备";
            this.bbiRemoveDevice.Id = 14;
            this.bbiRemoveDevice.ImageOptions.ImageIndex = 3;
            this.bbiRemoveDevice.ImageOptions.LargeImageIndex = 6;
            this.bbiRemoveDevice.Name = "bbiRemoveDevice";
            // 
            // bbiNewTagGroup
            // 
            this.bbiNewTagGroup.Caption = "新标记组";
            this.bbiNewTagGroup.Id = 15;
            this.bbiNewTagGroup.ImageOptions.ImageIndex = 6;
            this.bbiNewTagGroup.ImageOptions.LargeImageIndex = 11;
            this.bbiNewTagGroup.Name = "bbiNewTagGroup";
            // 
            // bbiRemoveTagGroup
            // 
            this.bbiRemoveTagGroup.Caption = "删除标记组";
            this.bbiRemoveTagGroup.Id = 16;
            this.bbiRemoveTagGroup.ImageOptions.ImageIndex = 7;
            this.bbiRemoveTagGroup.ImageOptions.LargeImageIndex = 12;
            this.bbiRemoveTagGroup.Name = "bbiRemoveTagGroup";
            // 
            // bbiNewTagSubGroup
            // 
            this.bbiNewTagSubGroup.Caption = "新标记子组";
            this.bbiNewTagSubGroup.Id = 17;
            this.bbiNewTagSubGroup.ImageOptions.ImageIndex = 8;
            this.bbiNewTagSubGroup.ImageOptions.LargeImageIndex = 13;
            this.bbiNewTagSubGroup.Name = "bbiNewTagSubGroup";
            // 
            // bbiRemoveTagSubGroup
            // 
            this.bbiRemoveTagSubGroup.Caption = "删除标记子组";
            this.bbiRemoveTagSubGroup.Id = 18;
            this.bbiRemoveTagSubGroup.ImageOptions.ImageIndex = 9;
            this.bbiRemoveTagSubGroup.ImageOptions.LargeImageIndex = 14;
            this.bbiRemoveTagSubGroup.Name = "bbiRemoveTagSubGroup";
            // 
            // bbiNewTag
            // 
            this.bbiNewTag.Caption = "新标记";
            this.bbiNewTag.Id = 19;
            this.bbiNewTag.ImageOptions.ImageIndex = 4;
            this.bbiNewTag.ImageOptions.LargeImageIndex = 9;
            this.bbiNewTag.Name = "bbiNewTag";
            // 
            // bbiRemoveTag
            // 
            this.bbiRemoveTag.Caption = "删除标记";
            this.bbiRemoveTag.Id = 20;
            this.bbiRemoveTag.ImageOptions.ImageIndex = 5;
            this.bbiRemoveTag.ImageOptions.LargeImageIndex = 10;
            this.bbiRemoveTag.Name = "bbiRemoveTag";
            // 
            // icLarge
            // 
            this.icLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.icLarge.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("icLarge.ImageStream")));
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.next_32x32, "next_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 0);
            this.icLarge.Images.SetKeyName(0, "next_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.selectall_32x32, "selectall_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 1);
            this.icLarge.Images.SetKeyName(1, "selectall_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.convert_32x32, "convert_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 2);
            this.icLarge.Images.SetKeyName(2, "convert_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.refresh_32x32, "refresh_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 3);
            this.icLarge.Images.SetKeyName(3, "refresh_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.version_32x32, "version_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 4);
            this.icLarge.Images.SetKeyName(4, "version_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.additem_32x32, "additem_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 5);
            this.icLarge.Images.SetKeyName(5, "additem_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletelist_32x32, "deletelist_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 6);
            this.icLarge.Images.SetKeyName(6, "deletelist_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.add_32x32, "add_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 7);
            this.icLarge.Images.SetKeyName(7, "add_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.cancel_32x32, "cancel_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 8);
            this.icLarge.Images.SetKeyName(8, "cancel_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.insert_32x32, "insert_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 9);
            this.icLarge.Images.SetKeyName(9, "insert_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.removeitem_32x32, "removeitem_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 10);
            this.icLarge.Images.SetKeyName(10, "removeitem_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.addnewdatasource_32x32, "addnewdatasource_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 11);
            this.icLarge.Images.SetKeyName(11, "addnewdatasource_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletedatasource_32x32, "deletedatasource_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 12);
            this.icLarge.Images.SetKeyName(12, "deletedatasource_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.addgroupfooter_32x32, "addgroupfooter_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 13);
            this.icLarge.Images.SetKeyName(13, "addgroupfooter_32x32");
            this.icLarge.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.deletegroupfooter_32x32, "deletegroupfooter_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 14);
            this.icLarge.Images.SetKeyName(14, "deletegroupfooter_32x32");
            this.icLarge.Images.SetKeyName(15, "import1_32px_31024_easyicon.net.png");
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rgpPLine,
            this.rpgDevice,
            this.rpgTagGroup,
            this.tpgTagSubGroup,
            this.rpgTag});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Tag = "Devices";
            this.ribbonPage1.Text = "设备";
            // 
            // rgpPLine
            // 
            this.rgpPLine.ItemLinks.Add(this.bbiNewProductionLine);
            this.rgpPLine.ItemLinks.Add(this.bbiRemoveProductionLine);
            this.rgpPLine.Name = "rgpPLine";
            this.rgpPLine.Text = "产线";
            // 
            // rpgDevice
            // 
            this.rpgDevice.ItemLinks.Add(this.bbiNewDevice);
            this.rpgDevice.ItemLinks.Add(this.bbiRemoveDevice);
            this.rpgDevice.ItemLinks.Add(this.bbiImportDeviceConfigParams, true);
            this.rpgDevice.Name = "rpgDevice";
            this.rpgDevice.Text = "设备";
            // 
            // rpgTagGroup
            // 
            this.rpgTagGroup.ItemLinks.Add(this.bbiNewTagGroup);
            this.rpgTagGroup.ItemLinks.Add(this.bbiRemoveTagGroup);
            this.rpgTagGroup.Name = "rpgTagGroup";
            this.rpgTagGroup.Text = "标记组";
            // 
            // tpgTagSubGroup
            // 
            this.tpgTagSubGroup.ItemLinks.Add(this.bbiNewTagSubGroup);
            this.tpgTagSubGroup.ItemLinks.Add(this.bbiRemoveTagSubGroup);
            this.tpgTagSubGroup.Name = "tpgTagSubGroup";
            this.tpgTagSubGroup.Text = "标记子组";
            // 
            // rpgTag
            // 
            this.rpgTag.ItemLinks.Add(this.bbiNewTag);
            this.rpgTag.ItemLinks.Add(this.bbiRemoveTag);
            this.rpgTag.Name = "rpgTag";
            this.rpgTag.Text = "标记";
            // 
            // rpServices
            // 
            this.rpServices.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgServiceList,
            this.ribbonPageGroup3});
            this.rpServices.Name = "rpServices";
            this.rpServices.Tag = "Services";
            this.rpServices.Text = "服务";
            // 
            // rpgServiceList
            // 
            this.rpgServiceList.ItemLinks.Add(this.bbiServicesList);
            this.rpgServiceList.ItemLinks.Add(this.bbiRefreshServiceList);
            this.rpgServiceList.Name = "rpgServiceList";
            this.rpgServiceList.Text = "服务";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.bbiStartService);
            this.ribbonPageGroup3.ItemLinks.Add(this.bbiStopService);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "服务启停";
            // 
            // rpView
            // 
            this.rpView.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgAppearance});
            this.rpView.Name = "rpView";
            this.rpView.Text = "显示";
            // 
            // rpgAppearance
            // 
            this.rpgAppearance.ItemLinks.Add(this.skinDropDownButtonItem1);
            this.rpgAppearance.ItemLinks.Add(this.skinPaletteRibbonGalleryBarItem1);
            this.rpgAppearance.Name = "rpgAppearance";
            this.rpgAppearance.Text = "外观";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 767);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(976, 21);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.nbgDevices;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbgDevices,
            this.nbgServices});
            this.navBarControl1.Location = new System.Drawing.Point(0, 158);
            this.navBarControl1.MenuManager = this.ribbon;
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 298;
            this.navBarControl1.Size = new System.Drawing.Size(298, 609);
            this.navBarControl1.TabIndex = 3;
            this.navBarControl1.Text = "navBarControl1";
            this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.SkinNavigationPaneViewInfoRegistrator();
            this.navBarControl1.ActiveGroupChanged += new DevExpress.XtraNavBar.NavBarGroupEventHandler(this.navBarControl1_ActiveGroupChanged);
            // 
            // nbgDevices
            // 
            this.nbgDevices.Caption = "设备";
            this.nbgDevices.ControlContainer = this.navBarGroupControlContainer1;
            this.nbgDevices.DragDropFlags = DevExpress.XtraNavBar.NavBarDragDrop.None;
            this.nbgDevices.Expanded = true;
            this.nbgDevices.GroupCaptionUseImage = DevExpress.XtraNavBar.NavBarImage.Large;
            this.nbgDevices.GroupClientHeight = 463;
            this.nbgDevices.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.nbgDevices.Name = "nbgDevices";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.ucDevices);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(298, 490);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // ucDevices
            // 
            this.ucDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucDevices.Location = new System.Drawing.Point(0, 0);
            this.ucDevices.Name = "ucDevices";
            this.ucDevices.Size = new System.Drawing.Size(298, 490);
            this.ucDevices.TabIndex = 0;
            this.ucDevices.OnDataSourceChanged += new IRAP.MESGateway.Tools.DataSourceChangedEventHandler(this.ucDevices_OnDataSourceChanged);
            // 
            // nbgServices
            // 
            this.nbgServices.Caption = "服务";
            this.nbgServices.Name = "nbgServices";
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(298, 158);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(12, 609);
            this.splitterControl1.TabIndex = 6;
            this.splitterControl1.TabStop = false;
            this.splitterControl1.Tag = "";
            // 
            // pcMain
            // 
            this.pcMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcMain.Location = new System.Drawing.Point(310, 158);
            this.pcMain.Name = "pcMain";
            this.pcMain.Size = new System.Drawing.Size(666, 609);
            this.pcMain.TabIndex = 9;
            // 
            // bbiImportDeviceConfigParams
            // 
            this.bbiImportDeviceConfigParams.Caption = "导入...";
            this.bbiImportDeviceConfigParams.Id = 21;
            this.bbiImportDeviceConfigParams.ImageOptions.ImageIndex = 10;
            this.bbiImportDeviceConfigParams.ImageOptions.LargeImageIndex = 15;
            this.bbiImportDeviceConfigParams.Name = "bbiImportDeviceConfigParams";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 788);
            this.Controls.Add(this.pcMain);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.navBarControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Ribbon = this.ribbon;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "NewMainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icSmall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icLarge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rgpPLine;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.SkinDropDownButtonItem skinDropDownButtonItem1;
        private DevExpress.XtraBars.BarButtonItem bbiRefreshServiceList;
        private DevExpress.XtraBars.BarButtonItem bbiStartService;
        private DevExpress.XtraBars.BarButtonItem bbiStopService;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgServiceList;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpServices;
        private DevExpress.Utils.ImageCollection icLarge;
        private DevExpress.XtraBars.BarButtonItem bbiServicesList;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup nbgDevices;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraNavBar.NavBarGroup nbgServices;
        private Controls.UCDevicesTree ucDevices;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpView;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgAppearance;
        private DevExpress.XtraBars.SkinPaletteRibbonGalleryBarItem skinPaletteRibbonGalleryBarItem1;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraEditors.PanelControl pcMain;
        private DevExpress.XtraBars.BarButtonItem bbiNewDevice;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgDevice;
        private DevExpress.XtraBars.BarButtonItem bbiNewProductionLine;
        private DevExpress.XtraBars.BarButtonItem bbiRemoveProductionLine;
        private DevExpress.XtraBars.BarButtonItem bbiRemoveDevice;
        private DevExpress.XtraBars.BarButtonItem bbiNewTagGroup;
        private DevExpress.XtraBars.BarButtonItem bbiRemoveTagGroup;
        private DevExpress.XtraBars.BarButtonItem bbiNewTagSubGroup;
        private DevExpress.XtraBars.BarButtonItem bbiRemoveTagSubGroup;
        private DevExpress.XtraBars.BarButtonItem bbiNewTag;
        private DevExpress.XtraBars.BarButtonItem bbiRemoveTag;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgTagGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup tpgTagSubGroup;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgTag;
        private DevExpress.Utils.ImageCollection icSmall;
        private DevExpress.XtraBars.BarButtonItem bbiImportDeviceConfigParams;
    }
}