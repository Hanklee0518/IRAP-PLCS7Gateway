namespace IRAP.MESGateway.Tools
{
    partial class DeviceTags
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pcDevicePage = new DevExpress.XtraEditors.PanelControl();
            this.ucProperties = new IRAP.MESGateway.Tools.Controls.UCProperties();
            ((System.ComponentModel.ISupportInitialize)(this.pcDevicePage)).BeginInit();
            this.pcDevicePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pcDevicePage
            // 
            this.pcDevicePage.Controls.Add(this.ucProperties);
            this.pcDevicePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcDevicePage.Location = new System.Drawing.Point(0, 0);
            this.pcDevicePage.Name = "pcDevicePage";
            this.pcDevicePage.Size = new System.Drawing.Size(1069, 681);
            this.pcDevicePage.TabIndex = 0;
            // 
            // ucProperties
            // 
            this.ucProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.ucProperties.Location = new System.Drawing.Point(711, 2);
            this.ucProperties.Name = "ucProperties";
            this.ucProperties.Size = new System.Drawing.Size(356, 677);
            this.ucProperties.TabIndex = 0;
            this.ucProperties.Validated += new System.EventHandler(this.ucProperties_Validated);
            // 
            // DeviceTags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pcDevicePage);
            this.Name = "DeviceTags";
            this.Size = new System.Drawing.Size(1069, 681);
            ((System.ComponentModel.ISupportInitialize)(this.pcDevicePage)).EndInit();
            this.pcDevicePage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pcDevicePage;
        private Controls.UCProperties ucProperties;
    }
}
