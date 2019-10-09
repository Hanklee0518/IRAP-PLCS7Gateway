namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCDevicePropertiesPage
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
            this.tlTags = new DevExpress.XtraTreeList.TreeList();
            this.tlclmnName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmnPrefix = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmnDataType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmnType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmnOffset = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmnLength = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.tlTags)).BeginInit();
            this.SuspendLayout();
            // 
            // tlTags
            // 
            this.tlTags.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tlTags.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlclmnName,
            this.tlclmnPrefix,
            this.tlclmnDataType,
            this.tlclmnType,
            this.tlclmnOffset,
            this.tlclmnLength});
            this.tlTags.Cursor = System.Windows.Forms.Cursors.Default;
            this.tlTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlTags.Location = new System.Drawing.Point(0, 0);
            this.tlTags.Name = "tlTags";
            this.tlTags.OptionsBehavior.Editable = false;
            this.tlTags.OptionsView.AutoWidth = false;
            this.tlTags.OptionsView.BestFitMode = DevExpress.XtraTreeList.TreeListBestFitMode.Fast;
            this.tlTags.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.All;
            this.tlTags.OptionsView.ShowHorzLines = false;
            this.tlTags.OptionsView.ShowTreeLines = DevExpress.Utils.DefaultBoolean.True;
            this.tlTags.OptionsView.ShowVertLines = false;
            this.tlTags.Size = new System.Drawing.Size(904, 663);
            this.tlTags.TabIndex = 0;
            this.tlTags.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlTags_FocusedNodeChanged);
            // 
            // tlclmnName
            // 
            this.tlclmnName.Caption = "名称";
            this.tlclmnName.FieldName = "Name";
            this.tlclmnName.Name = "tlclmnName";
            this.tlclmnName.Visible = true;
            this.tlclmnName.VisibleIndex = 0;
            // 
            // tlclmnPrefix
            // 
            this.tlclmnPrefix.Caption = "前缀";
            this.tlclmnPrefix.FieldName = "Prefix";
            this.tlclmnPrefix.Name = "tlclmnPrefix";
            // 
            // tlclmnDataType
            // 
            this.tlclmnDataType.Caption = "数据类型";
            this.tlclmnDataType.FieldName = "TagDataType";
            this.tlclmnDataType.Name = "tlclmnDataType";
            this.tlclmnDataType.Visible = true;
            this.tlclmnDataType.VisibleIndex = 1;
            // 
            // tlclmnType
            // 
            this.tlclmnType.Caption = "标记类型";
            this.tlclmnType.FieldName = "TagType";
            this.tlclmnType.Name = "tlclmnType";
            this.tlclmnType.Visible = true;
            this.tlclmnType.VisibleIndex = 2;
            // 
            // tlclmnOffset
            // 
            this.tlclmnOffset.Caption = "偏移量";
            this.tlclmnOffset.FieldName = "Offset";
            this.tlclmnOffset.Name = "tlclmnOffset";
            this.tlclmnOffset.Visible = true;
            this.tlclmnOffset.VisibleIndex = 3;
            // 
            // tlclmnLength
            // 
            this.tlclmnLength.Caption = "长度";
            this.tlclmnLength.FieldName = "Length";
            this.tlclmnLength.Name = "tlclmnLength";
            this.tlclmnLength.Visible = true;
            this.tlclmnLength.VisibleIndex = 4;
            // 
            // UCDevicePropertiesPage
            // 
            this.Controls.Add(this.tlTags);
            this.Name = "UCDevicePropertiesPage";
            this.Size = new System.Drawing.Size(904, 663);
            this.Enter += new System.EventHandler(this.UCDevicePropertiesPage_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.tlTags)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlTags;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnPrefix;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnDataType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnOffset;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmnLength;
    }
}
