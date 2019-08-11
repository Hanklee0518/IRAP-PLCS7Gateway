namespace IRAP.MESGateway.Tools.Controls
{
    partial class UCProperties
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
            this.propertyGrid = new DevExpress.DXperience.Demos.XtraPropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Appearance.Options.UseFont = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(270, 546);
            this.propertyGrid.TabIndex = 4;
            // 
            // UCProperties
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.propertyGrid);
            this.Name = "UCProperties";
            this.Size = new System.Drawing.Size(270, 546);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.DXperience.Demos.XtraPropertyGrid propertyGrid;
    }
}
