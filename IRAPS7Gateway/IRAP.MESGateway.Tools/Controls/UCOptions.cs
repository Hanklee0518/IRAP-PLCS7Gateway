using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraBars.Ribbon;

namespace IRAP.MESGateway.Tools.Controls
{
    public partial class UCOptions : DevExpress.XtraEditors.XtraUserControl
    {
        public UCOptions()
        {
            InitializeComponent();
        }

        private void CloseParent()
        {
            if (Parent is BackstageViewClientControl bvccParent)
            {
                if (bvccParent.Parent is BackstageViewControl bvcParent)
                {
                    bvcParent.Close();
                }
            }
        }

        public void InitOptionItems()
        {
            edtProjectPath.Text = ParamHelper.ProjectBasePath;
            edtCommunityID.Text = ParamHelper.CommunityID.ToString();
        }

        private void edtProjectPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            switch (e.Button.Index)
            {
                case 0:
                    using (FolderBrowserDialog folder = new FolderBrowserDialog())
                    {
                        folder.Description = "选择 DCSGateway for PLC 的工作文件夹：";
                        folder.ShowNewFolderButton = true;

                        if (folder.ShowDialog() == DialogResult.OK)
                        {
                            edtProjectPath.Text = folder.SelectedPath;
                        }
                    }
                    break;
            }
        }

        private void edtProjectPath_Validating(object sender, CancelEventArgs e)
        {
            if (!Directory.Exists(edtProjectPath.Text) && edtProjectPath.Text != "")
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void btnSaveOptionsChanged_Click(object sender, EventArgs e)
        {
            ParamHelper.ProjectBasePath = edtProjectPath.Text;
            if (int.TryParse(edtCommunityID.Text, out int value))
            {
                ParamHelper.CommunityID = value;
            }

            CloseParent();
        }

        private void btnCancelOptionsChanged_Click(object sender, EventArgs e)
        {
            InitOptionItems();

            CloseParent();
        }

        private void edtCommunityID_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(edtCommunityID.Text, out int value))
            {
                e.Cancel = true;
                XtraMessageBox.Show(
                    "社区标识必须是整型数字",
                    "信息",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
