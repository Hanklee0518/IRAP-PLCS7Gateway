namespace IRAP.MESGateway.Tools.Forms
{
    partial class GatewayServicesForm
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
            this.tlServices = new DevExpress.XtraTreeList.TreeList();
            this.tlclmServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlclmServiceStatus = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tlServices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tlServices
            // 
            this.tlServices.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.tlServices.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlServices.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.tlServices.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlclmServiceName,
            this.tlclmServiceStatus});
            this.tlServices.Cursor = System.Windows.Forms.Cursors.Default;
            this.tlServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlServices.Location = new System.Drawing.Point(0, 0);
            this.tlServices.Name = "tlServices";
            this.tlServices.OptionsView.AutoWidth = false;
            this.tlServices.OptionsView.BestFitMode = DevExpress.XtraTreeList.TreeListBestFitMode.Full;
            this.tlServices.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.All;
            this.tlServices.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemImageComboBox1});
            this.tlServices.Size = new System.Drawing.Size(870, 599);
            this.tlServices.TabIndex = 1;
            // 
            // tlclmServiceName
            // 
            this.tlclmServiceName.Caption = "服务名称";
            this.tlclmServiceName.FieldName = "ServiceName";
            this.tlclmServiceName.Name = "tlclmServiceName";
            this.tlclmServiceName.Visible = true;
            this.tlclmServiceName.VisibleIndex = 0;
            // 
            // tlclmServiceStatus
            // 
            this.tlclmServiceStatus.AppearanceCell.Options.UseTextOptions = true;
            this.tlclmServiceStatus.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlclmServiceStatus.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.tlclmServiceStatus.Caption = "状态";
            this.tlclmServiceStatus.ColumnEdit = this.repositoryItemImageComboBox1;
            this.tlclmServiceStatus.FieldName = "ServiceStatus";
            this.tlclmServiceStatus.Name = "tlclmServiceStatus";
            this.tlclmServiceStatus.Visible = true;
            this.tlclmServiceStatus.VisibleIndex = 1;
            // 
            // repositoryItemImageComboBox1
            // 
            this.repositoryItemImageComboBox1.AutoHeight = false;
            this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageComboBox1.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务未运行", 1, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务正在启动", 2, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务正在停止", 3, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务正在运行", 4, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务即将继续", 5, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务即将暂停", 6, -1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("服务已暂停", 7, -1)});
            this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
            // 
            // GatewayServicesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 599);
            this.Controls.Add(this.tlServices);
            this.Name = "GatewayServicesForm";
            this.Text = "DCSGateway服务";
            this.Shown += new System.EventHandler(this.GatewayServicesForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tlServices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlServices;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmServiceStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBox1;
    }
}