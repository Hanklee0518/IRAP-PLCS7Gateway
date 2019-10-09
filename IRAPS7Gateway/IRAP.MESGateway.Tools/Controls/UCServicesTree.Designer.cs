namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCServicesTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCServicesTree));
            this.tlServices = new DevExpress.XtraTreeList.TreeList();
            this.tlclmServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.imageCollection = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlServices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection)).BeginInit();
            this.SuspendLayout();
            // 
            // tlServices
            // 
            this.tlServices.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tlServices.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlclmServiceName});
            this.tlServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlServices.HtmlImages = this.imageCollection;
            this.tlServices.Location = new System.Drawing.Point(0, 0);
            this.tlServices.Name = "tlServices";
            this.tlServices.OptionsBehavior.Editable = false;
            this.tlServices.OptionsView.ShowColumns = false;
            this.tlServices.OptionsView.ShowHorzLines = false;
            this.tlServices.OptionsView.ShowIndentAsRowStyle = true;
            this.tlServices.OptionsView.ShowIndicator = false;
            this.tlServices.OptionsView.ShowVertLines = false;
            this.tlServices.Padding = new System.Windows.Forms.Padding(10);
            this.tlServices.SelectImageList = this.imageCollection;
            this.tlServices.Size = new System.Drawing.Size(254, 441);
            this.tlServices.StateImageList = this.imageCollection;
            this.tlServices.TabIndex = 2;
            this.tlServices.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlServices_FocusedNodeChanged);
            // 
            // tlclmServiceName
            // 
            this.tlclmServiceName.Caption = "服务名称";
            this.tlclmServiceName.FieldName = "ServiceName";
            this.tlclmServiceName.Name = "tlclmServiceName";
            this.tlclmServiceName.Visible = true;
            this.tlclmServiceName.VisibleIndex = 0;
            // 
            // imageCollection
            // 
            this.imageCollection.ImageSize = new System.Drawing.Size(24, 24);
            this.imageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection.ImageStream")));
            this.imageCollection.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.technology_32x32, "technology_32x32", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 0);
            this.imageCollection.Images.SetKeyName(0, "technology_32x32");
            this.imageCollection.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.next_32x321, "next_32x321", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 1);
            this.imageCollection.Images.SetKeyName(1, "next_32x321");
            this.imageCollection.InsertImage(global::IRAP.MESGateway.Tools.Properties.Resources.selectall_32x321, "selectall_32x321", typeof(global::IRAP.MESGateway.Tools.Properties.Resources), 2);
            this.imageCollection.Images.SetKeyName(2, "selectall_32x321");
            // 
            // UCServicesTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlServices);
            this.Name = "UCServicesTree";
            this.Size = new System.Drawing.Size(254, 441);
            this.Enter += new System.EventHandler(this.UCServicesTree_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.tlServices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlServices;
        private DevExpress.Utils.ImageCollection imageCollection;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlclmServiceName;
    }
}
