namespace IRAP.MESGateway.Tools
{
    partial class ServiceInfo
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
            this.edtLog = new DevExpress.XtraEditors.MemoEdit();
            this.tcViewLog = new System.Windows.Forms.TabControl();
            this.tpLog = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.edtLog.Properties)).BeginInit();
            this.tcViewLog.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // edtLog
            // 
            this.edtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edtLog.Location = new System.Drawing.Point(3, 3);
            this.edtLog.Name = "edtLog";
            this.edtLog.Properties.AcceptsTab = true;
            this.edtLog.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.edtLog.Properties.Appearance.Options.UseBackColor = true;
            this.edtLog.Properties.ReadOnly = true;
            this.edtLog.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.edtLog.Properties.WordWrap = false;
            this.edtLog.Size = new System.Drawing.Size(992, 641);
            this.edtLog.TabIndex = 0;
            // 
            // tcViewLog
            // 
            this.tcViewLog.Controls.Add(this.tpLog);
            this.tcViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcViewLog.Location = new System.Drawing.Point(10, 10);
            this.tcViewLog.Name = "tcViewLog";
            this.tcViewLog.SelectedIndex = 0;
            this.tcViewLog.Size = new System.Drawing.Size(1006, 677);
            this.tcViewLog.TabIndex = 1;
            this.tcViewLog.TabStop = false;
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.edtLog);
            this.tpLog.Location = new System.Drawing.Point(4, 26);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(998, 647);
            this.tpLog.TabIndex = 0;
            this.tpLog.UseVisualStyleBackColor = true;
            // 
            // ServiceInfo
            // 
            this.Controls.Add(this.tcViewLog);
            this.Name = "ServiceInfo";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1026, 697);
            ((System.ComponentModel.ISupportInitialize)(this.edtLog.Properties)).EndInit();
            this.tcViewLog.ResumeLayout(false);
            this.tpLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.MemoEdit edtLog;
        private System.Windows.Forms.TabControl tcViewLog;
        private System.Windows.Forms.TabPage tpLog;
    }
}
