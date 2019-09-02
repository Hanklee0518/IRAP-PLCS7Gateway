namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCOptions
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.edtProjectPath = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.edtCommunityID = new DevExpress.XtraEditors.TextEdit();
            this.btnCancelOptionsChanged = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveOptionsChanged = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.edtWebAPIUrl = new DevExpress.XtraEditors.TextEdit();
            this.edtConnectionStringWithMongoDB = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.edtProjectPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCommunityID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtWebAPIUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtConnectionStringWithMongoDB.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("新宋体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl2.Location = new System.Drawing.Point(0, 0);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(612, 48);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "选项";
            // 
            // edtProjectPath
            // 
            this.edtProjectPath.Location = new System.Drawing.Point(17, 79);
            this.edtProjectPath.MaximumSize = new System.Drawing.Size(480, 0);
            this.edtProjectPath.MinimumSize = new System.Drawing.Size(120, 0);
            this.edtProjectPath.Name = "edtProjectPath";
            editorButtonImageOptions1.Image = global::IRAP.MESGateway.Tools.Properties.Resources.projectdirectory_16x16;
            this.edtProjectPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.edtProjectPath.Size = new System.Drawing.Size(480, 22);
            this.edtProjectPath.TabIndex = 4;
            this.edtProjectPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.edtProjectPath_ButtonClick);
            this.edtProjectPath.Validating += new System.ComponentModel.CancelEventHandler(this.edtProjectPath_Validating);
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("新宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(17, 54);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(250, 19);
            this.labelControl5.TabIndex = 3;
            this.labelControl5.Text = "DCSGateway 服务安装目录：";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("新宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(17, 120);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(100, 19);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "社区标识：";
            // 
            // edtCommunityID
            // 
            this.edtCommunityID.Location = new System.Drawing.Point(17, 145);
            this.edtCommunityID.Name = "edtCommunityID";
            this.edtCommunityID.Size = new System.Drawing.Size(189, 22);
            this.edtCommunityID.TabIndex = 6;
            this.edtCommunityID.Validating += new System.ComponentModel.CancelEventHandler(this.edtCommunityID_Validating);
            // 
            // btnCancelOptionsChanged
            // 
            this.btnCancelOptionsChanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelOptionsChanged.Location = new System.Drawing.Point(131, 710);
            this.btnCancelOptionsChanged.Name = "btnCancelOptionsChanged";
            this.btnCancelOptionsChanged.Size = new System.Drawing.Size(93, 30);
            this.btnCancelOptionsChanged.TabIndex = 8;
            this.btnCancelOptionsChanged.Text = "取消";
            this.btnCancelOptionsChanged.Click += new System.EventHandler(this.btnCancelOptionsChanged_Click);
            // 
            // btnSaveOptionsChanged
            // 
            this.btnSaveOptionsChanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveOptionsChanged.Location = new System.Drawing.Point(17, 710);
            this.btnSaveOptionsChanged.Name = "btnSaveOptionsChanged";
            this.btnSaveOptionsChanged.Size = new System.Drawing.Size(93, 30);
            this.btnSaveOptionsChanged.TabIndex = 7;
            this.btnSaveOptionsChanged.Text = "保存";
            this.btnSaveOptionsChanged.Click += new System.EventHandler(this.btnSaveOptionsChanged_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("新宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(17, 195);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(120, 19);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "WebAPI路径：";
            // 
            // edtWebAPIUrl
            // 
            this.edtWebAPIUrl.Location = new System.Drawing.Point(17, 220);
            this.edtWebAPIUrl.Name = "edtWebAPIUrl";
            this.edtWebAPIUrl.Size = new System.Drawing.Size(433, 22);
            this.edtWebAPIUrl.TabIndex = 10;
            // 
            // edtConnectionStringWithMongoDB
            // 
            this.edtConnectionStringWithMongoDB.Location = new System.Drawing.Point(17, 294);
            this.edtConnectionStringWithMongoDB.Name = "edtConnectionStringWithMongoDB";
            this.edtConnectionStringWithMongoDB.Size = new System.Drawing.Size(433, 22);
            this.edtConnectionStringWithMongoDB.TabIndex = 12;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("新宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(17, 269);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(200, 19);
            this.labelControl4.TabIndex = 11;
            this.labelControl4.Text = "MongoDB 连接字符串：";
            // 
            // UCOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.Controls.Add(this.edtConnectionStringWithMongoDB);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.edtWebAPIUrl);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.btnCancelOptionsChanged);
            this.Controls.Add(this.btnSaveOptionsChanged);
            this.Controls.Add(this.edtCommunityID);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.edtProjectPath);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl2);
            this.Name = "UCOptions";
            this.Size = new System.Drawing.Size(612, 758);
            ((System.ComponentModel.ISupportInitialize)(this.edtProjectPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCommunityID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtWebAPIUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtConnectionStringWithMongoDB.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ButtonEdit edtProjectPath;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit edtCommunityID;
        private DevExpress.XtraEditors.SimpleButton btnCancelOptionsChanged;
        private DevExpress.XtraEditors.SimpleButton btnSaveOptionsChanged;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit edtWebAPIUrl;
        private DevExpress.XtraEditors.TextEdit edtConnectionStringWithMongoDB;
        private DevExpress.XtraEditors.LabelControl labelControl4;
    }
}
