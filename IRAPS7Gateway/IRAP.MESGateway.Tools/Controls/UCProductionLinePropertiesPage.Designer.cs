namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCProductionLinePropertiesPage
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.grdDevices = new DevExpress.XtraGrid.GridControl();
            this.grdvDevices = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdclmnName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnPLCType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnIPAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grclmnDBType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnDBNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnCycleReadMode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnSplitterTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdclmnUpgrade = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdvDevices)).BeginInit();
            this.SuspendLayout();
            // 
            // grdDevices
            // 
            this.grdDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDevices.Location = new System.Drawing.Point(0, 0);
            this.grdDevices.MainView = this.grdvDevices;
            this.grdDevices.Name = "grdDevices";
            this.grdDevices.Size = new System.Drawing.Size(969, 641);
            this.grdDevices.TabIndex = 0;
            this.grdDevices.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdvDevices});
            // 
            // grdvDevices
            // 
            this.grdvDevices.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grdvDevices.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdvDevices.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.grdvDevices.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdclmnName,
            this.grdclmnPLCType,
            this.grdclmnIPAddress,
            this.grclmnDBType,
            this.grdclmnDBNumber,
            this.grdclmnCycleReadMode,
            this.grdclmnSplitterTime,
            this.grdclmnUpgrade});
            this.grdvDevices.GridControl = this.grdDevices;
            this.grdvDevices.Name = "grdvDevices";
            this.grdvDevices.OptionsBehavior.Editable = false;
            this.grdvDevices.OptionsView.ColumnAutoWidth = false;
            this.grdvDevices.OptionsView.ShowGroupPanel = false;
            this.grdvDevices.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grdvDevices_FocusedRowChanged);
            // 
            // grdclmnName
            // 
            this.grdclmnName.Caption = "设备名称";
            this.grdclmnName.FieldName = "Name";
            this.grdclmnName.Name = "grdclmnName";
            this.grdclmnName.Visible = true;
            this.grdclmnName.VisibleIndex = 0;
            // 
            // grdclmnPLCType
            // 
            this.grdclmnPLCType.Caption = "PLC类型";
            this.grdclmnPLCType.FieldName = "PLCType";
            this.grdclmnPLCType.Name = "grdclmnPLCType";
            this.grdclmnPLCType.Visible = true;
            this.grdclmnPLCType.VisibleIndex = 1;
            // 
            // grdclmnIPAddress
            // 
            this.grdclmnIPAddress.Caption = "IP地址";
            this.grdclmnIPAddress.FieldName = "BelongPLC.IPAddress";
            this.grdclmnIPAddress.Name = "grdclmnIPAddress";
            this.grdclmnIPAddress.Visible = true;
            this.grdclmnIPAddress.VisibleIndex = 2;
            // 
            // grclmnDBType
            // 
            this.grclmnDBType.Caption = "数据块类型";
            this.grclmnDBType.FieldName = "DBType";
            this.grclmnDBType.Name = "grclmnDBType";
            this.grclmnDBType.Visible = true;
            this.grclmnDBType.VisibleIndex = 3;
            // 
            // grdclmnDBNumber
            // 
            this.grdclmnDBNumber.Caption = "数据块标识号";
            this.grdclmnDBNumber.FieldName = "DBNumber";
            this.grdclmnDBNumber.Name = "grdclmnDBNumber";
            this.grdclmnDBNumber.Visible = true;
            this.grdclmnDBNumber.VisibleIndex = 4;
            // 
            // grdclmnCycleReadMode
            // 
            this.grdclmnCycleReadMode.Caption = "监控模式";
            this.grdclmnCycleReadMode.FieldName = "CycleReadMode";
            this.grdclmnCycleReadMode.Name = "grdclmnCycleReadMode";
            this.grdclmnCycleReadMode.Visible = true;
            this.grdclmnCycleReadMode.VisibleIndex = 5;
            // 
            // grdclmnSplitterTime
            // 
            this.grdclmnSplitterTime.Caption = "读取间隔时间(ms)";
            this.grdclmnSplitterTime.FieldName = "SplitterTime";
            this.grdclmnSplitterTime.Name = "grdclmnSplitterTime";
            this.grdclmnSplitterTime.Visible = true;
            this.grdclmnSplitterTime.VisibleIndex = 6;
            // 
            // grdclmnUpgrade
            // 
            this.grdclmnUpgrade.Caption = "版本更新状态";
            this.grdclmnUpgrade.FieldName = "UpgradeStatus";
            this.grdclmnUpgrade.Name = "grdclmnUpgrade";
            this.grdclmnUpgrade.Visible = true;
            this.grdclmnUpgrade.VisibleIndex = 7;
            // 
            // UCProductionLinePropertiesPage
            // 
            this.Controls.Add(this.grdDevices);
            this.Name = "UCProductionLinePropertiesPage";
            this.Size = new System.Drawing.Size(969, 641);
            this.Enter += new System.EventHandler(this.UCProductionLinePropertiesPage_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdvDevices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdDevices;
        private DevExpress.XtraGrid.Views.Grid.GridView grdvDevices;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnName;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnPLCType;
        private DevExpress.XtraGrid.Columns.GridColumn grclmnDBType;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnDBNumber;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnCycleReadMode;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnSplitterTime;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnIPAddress;
        private DevExpress.XtraGrid.Columns.GridColumn grdclmnUpgrade;
    }
}
