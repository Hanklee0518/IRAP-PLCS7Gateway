namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCDevicesTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDevicesTree));
            this.tlTrees = new DevExpress.XtraTreeList.TreeList();
            this.tlcName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.imageCollection = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlTrees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection)).BeginInit();
            this.SuspendLayout();
            // 
            // tlTrees
            // 
            this.tlTrees.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tlTrees.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlcName});
            this.tlTrees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlTrees.Location = new System.Drawing.Point(0, 0);
            this.tlTrees.Name = "tlTrees";
            this.tlTrees.OptionsBehavior.Editable = false;
            this.tlTrees.OptionsView.ShowColumns = false;
            this.tlTrees.OptionsView.ShowHorzLines = false;
            this.tlTrees.OptionsView.ShowIndentAsRowStyle = true;
            this.tlTrees.OptionsView.ShowIndicator = false;
            this.tlTrees.OptionsView.ShowVertLines = false;
            this.tlTrees.SelectImageList = this.imageCollection;
            this.tlTrees.Size = new System.Drawing.Size(300, 507);
            this.tlTrees.TabIndex = 0;
            this.tlTrees.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlTrees_FocusedNodeChanged);
            // 
            // tlcName
            // 
            this.tlcName.Caption = "产线名称";
            this.tlcName.FieldName = "tlcName";
            this.tlcName.Name = "tlcName";
            this.tlcName.Visible = true;
            this.tlcName.VisibleIndex = 0;
            // 
            // imageCollection
            // 
            this.imageCollection.ImageSize = new System.Drawing.Size(24, 24);
            this.imageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection.ImageStream")));
            this.imageCollection.Images.SetKeyName(0, "Line_flat_circle_24px_1113572_easyicon.net.png");
            this.imageCollection.Images.SetKeyName(1, "machine_24px_27273_easyicon.net.png");
            // 
            // UCDevicesTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlTrees);
            this.Name = "UCDevicesTree";
            this.Size = new System.Drawing.Size(300, 507);
            this.Enter += new System.EventHandler(this.UCDevicesTree_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.tlTrees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlTrees;
        private DevExpress.Utils.ImageCollection imageCollection;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcName;
    }
}
