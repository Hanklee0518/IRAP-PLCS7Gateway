namespace SiemensPLCMonitor
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.edtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.edtIPAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.edtText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.edtDBNumber = new System.Windows.Forms.TextBox();
            this.edtOffset = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.edtLength = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnReadBool = new System.Windows.Forms.Button();
            this.btnWriteBool = new System.Windows.Forms.Button();
            this.btnWriteByte = new System.Windows.Forms.Button();
            this.btnReadByte = new System.Windows.Forms.Button();
            this.btnWriteWord = new System.Windows.Forms.Button();
            this.btnReadWord = new System.Windows.Forms.Button();
            this.btnWriteInt = new System.Windows.Forms.Button();
            this.btnReadint = new System.Windows.Forms.Button();
            this.btnWriteDWord = new System.Windows.Forms.Button();
            this.btnReadDWord = new System.Windows.Forms.Button();
            this.btnWriteReal = new System.Windows.Forms.Button();
            this.btnReadReal = new System.Windows.Forms.Button();
            this.btnWriteChars = new System.Windows.Forms.Button();
            this.btnReadChars = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.edtPort);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.edtIPAddress);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 47);
            this.panel1.TabIndex = 0;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(781, 11);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 26);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "断开连接";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(691, 11);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(84, 26);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // edtPort
            // 
            this.edtPort.Location = new System.Drawing.Point(403, 13);
            this.edtPort.Name = "edtPort";
            this.edtPort.Size = new System.Drawing.Size(73, 26);
            this.edtPort.TabIndex = 3;
            this.edtPort.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(325, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口号：";
            this.label2.Visible = false;
            // 
            // edtIPAddress
            // 
            this.edtIPAddress.Location = new System.Drawing.Point(94, 13);
            this.edtIPAddress.Name = "edtIPAddress";
            this.edtIPAddress.Size = new System.Drawing.Size(225, 26);
            this.edtIPAddress.TabIndex = 1;
            this.edtIPAddress.Text = "192.168.0.3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址：";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.edtText);
            this.panel2.Location = new System.Drawing.Point(12, 103);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(677, 217);
            this.panel2.TabIndex = 1;
            // 
            // edtText
            // 
            this.edtText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edtText.Location = new System.Drawing.Point(5, 5);
            this.edtText.Multiline = true;
            this.edtText.Name = "edtText";
            this.edtText.Size = new System.Drawing.Size(665, 205);
            this.edtText.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "DB 块标识号：";
            // 
            // edtDBNumber
            // 
            this.edtDBNumber.Location = new System.Drawing.Point(147, 68);
            this.edtDBNumber.Name = "edtDBNumber";
            this.edtDBNumber.Size = new System.Drawing.Size(123, 26);
            this.edtDBNumber.TabIndex = 4;
            // 
            // edtOffset
            // 
            this.edtOffset.Location = new System.Drawing.Point(381, 68);
            this.edtOffset.Name = "edtOffset";
            this.edtOffset.Size = new System.Drawing.Size(123, 26);
            this.edtOffset.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(287, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "起始地址：";
            // 
            // edtLength
            // 
            this.edtLength.Location = new System.Drawing.Point(604, 68);
            this.edtLength.Name = "edtLength";
            this.edtLength.Size = new System.Drawing.Size(85, 26);
            this.edtLength.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(510, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "读取长度：";
            // 
            // btnReadBool
            // 
            this.btnReadBool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadBool.Location = new System.Drawing.Point(699, 103);
            this.btnReadBool.Name = "btnReadBool";
            this.btnReadBool.Size = new System.Drawing.Size(100, 26);
            this.btnReadBool.TabIndex = 9;
            this.btnReadBool.Text = "读Bool值";
            this.btnReadBool.UseVisualStyleBackColor = true;
            this.btnReadBool.Click += new System.EventHandler(this.btnReadBool_Click);
            // 
            // btnWriteBool
            // 
            this.btnWriteBool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteBool.Location = new System.Drawing.Point(805, 103);
            this.btnWriteBool.Name = "btnWriteBool";
            this.btnWriteBool.Size = new System.Drawing.Size(100, 26);
            this.btnWriteBool.TabIndex = 10;
            this.btnWriteBool.Text = "写Bool值";
            this.btnWriteBool.UseVisualStyleBackColor = true;
            this.btnWriteBool.Click += new System.EventHandler(this.btnWriteBool_Click);
            // 
            // btnWriteByte
            // 
            this.btnWriteByte.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteByte.Location = new System.Drawing.Point(805, 135);
            this.btnWriteByte.Name = "btnWriteByte";
            this.btnWriteByte.Size = new System.Drawing.Size(100, 26);
            this.btnWriteByte.TabIndex = 12;
            this.btnWriteByte.Text = "写Byte值";
            this.btnWriteByte.UseVisualStyleBackColor = true;
            this.btnWriteByte.Click += new System.EventHandler(this.btnWriteByte_Click);
            // 
            // btnReadByte
            // 
            this.btnReadByte.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadByte.Location = new System.Drawing.Point(699, 135);
            this.btnReadByte.Name = "btnReadByte";
            this.btnReadByte.Size = new System.Drawing.Size(100, 26);
            this.btnReadByte.TabIndex = 11;
            this.btnReadByte.Text = "读Byte值";
            this.btnReadByte.UseVisualStyleBackColor = true;
            this.btnReadByte.Click += new System.EventHandler(this.btnReadByte_Click);
            // 
            // btnWriteWord
            // 
            this.btnWriteWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteWord.Location = new System.Drawing.Point(805, 167);
            this.btnWriteWord.Name = "btnWriteWord";
            this.btnWriteWord.Size = new System.Drawing.Size(100, 26);
            this.btnWriteWord.TabIndex = 14;
            this.btnWriteWord.Text = "写Word值";
            this.btnWriteWord.UseVisualStyleBackColor = true;
            this.btnWriteWord.Click += new System.EventHandler(this.btnWriteWord_Click);
            // 
            // btnReadWord
            // 
            this.btnReadWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadWord.Location = new System.Drawing.Point(699, 167);
            this.btnReadWord.Name = "btnReadWord";
            this.btnReadWord.Size = new System.Drawing.Size(100, 26);
            this.btnReadWord.TabIndex = 13;
            this.btnReadWord.Text = "读Word值";
            this.btnReadWord.UseVisualStyleBackColor = true;
            this.btnReadWord.Click += new System.EventHandler(this.btnReadWord_Click);
            // 
            // btnWriteInt
            // 
            this.btnWriteInt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteInt.Location = new System.Drawing.Point(805, 199);
            this.btnWriteInt.Name = "btnWriteInt";
            this.btnWriteInt.Size = new System.Drawing.Size(100, 26);
            this.btnWriteInt.TabIndex = 16;
            this.btnWriteInt.Text = "写Int值";
            this.btnWriteInt.UseVisualStyleBackColor = true;
            this.btnWriteInt.Click += new System.EventHandler(this.btnWriteInt_Click);
            // 
            // btnReadint
            // 
            this.btnReadint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadint.Location = new System.Drawing.Point(699, 199);
            this.btnReadint.Name = "btnReadint";
            this.btnReadint.Size = new System.Drawing.Size(100, 26);
            this.btnReadint.TabIndex = 15;
            this.btnReadint.Text = "读Int值";
            this.btnReadint.UseVisualStyleBackColor = true;
            this.btnReadint.Click += new System.EventHandler(this.btnReadint_Click);
            // 
            // btnWriteDWord
            // 
            this.btnWriteDWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteDWord.Location = new System.Drawing.Point(805, 231);
            this.btnWriteDWord.Name = "btnWriteDWord";
            this.btnWriteDWord.Size = new System.Drawing.Size(100, 26);
            this.btnWriteDWord.TabIndex = 18;
            this.btnWriteDWord.Text = "写DWord值";
            this.btnWriteDWord.UseVisualStyleBackColor = true;
            this.btnWriteDWord.Click += new System.EventHandler(this.btnWriteDWord_Click);
            // 
            // btnReadDWord
            // 
            this.btnReadDWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadDWord.Location = new System.Drawing.Point(699, 231);
            this.btnReadDWord.Name = "btnReadDWord";
            this.btnReadDWord.Size = new System.Drawing.Size(100, 26);
            this.btnReadDWord.TabIndex = 17;
            this.btnReadDWord.Text = "读DWord值";
            this.btnReadDWord.UseVisualStyleBackColor = true;
            this.btnReadDWord.Click += new System.EventHandler(this.btnReadDWord_Click);
            // 
            // btnWriteReal
            // 
            this.btnWriteReal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteReal.Location = new System.Drawing.Point(801, 263);
            this.btnWriteReal.Name = "btnWriteReal";
            this.btnWriteReal.Size = new System.Drawing.Size(100, 26);
            this.btnWriteReal.TabIndex = 22;
            this.btnWriteReal.Text = "写Real值";
            this.btnWriteReal.UseVisualStyleBackColor = true;
            this.btnWriteReal.Click += new System.EventHandler(this.btnWriteReal_Click);
            // 
            // btnReadReal
            // 
            this.btnReadReal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadReal.Location = new System.Drawing.Point(695, 263);
            this.btnReadReal.Name = "btnReadReal";
            this.btnReadReal.Size = new System.Drawing.Size(100, 26);
            this.btnReadReal.TabIndex = 21;
            this.btnReadReal.Text = "读Real值";
            this.btnReadReal.UseVisualStyleBackColor = true;
            this.btnReadReal.Click += new System.EventHandler(this.btnReadReal_Click);
            // 
            // btnWriteChars
            // 
            this.btnWriteChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWriteChars.Location = new System.Drawing.Point(801, 295);
            this.btnWriteChars.Name = "btnWriteChars";
            this.btnWriteChars.Size = new System.Drawing.Size(100, 26);
            this.btnWriteChars.TabIndex = 24;
            this.btnWriteChars.Text = "写Chars值";
            this.btnWriteChars.UseVisualStyleBackColor = true;
            this.btnWriteChars.Click += new System.EventHandler(this.btnWriteChars_Click);
            // 
            // btnReadChars
            // 
            this.btnReadChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadChars.Location = new System.Drawing.Point(695, 295);
            this.btnReadChars.Name = "btnReadChars";
            this.btnReadChars.Size = new System.Drawing.Size(100, 26);
            this.btnReadChars.TabIndex = 23;
            this.btnReadChars.Text = "读Chars值";
            this.btnReadChars.UseVisualStyleBackColor = true;
            this.btnReadChars.Click += new System.EventHandler(this.btnReadChars_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(917, 331);
            this.Controls.Add(this.btnWriteChars);
            this.Controls.Add(this.btnReadChars);
            this.Controls.Add(this.btnWriteReal);
            this.Controls.Add(this.btnReadReal);
            this.Controls.Add(this.btnWriteDWord);
            this.Controls.Add(this.btnReadDWord);
            this.Controls.Add(this.btnWriteInt);
            this.Controls.Add(this.btnReadint);
            this.Controls.Add(this.btnWriteWord);
            this.Controls.Add(this.btnReadWord);
            this.Controls.Add(this.btnWriteByte);
            this.Controls.Add(this.btnReadByte);
            this.Controls.Add(this.btnWriteBool);
            this.Controls.Add(this.btnReadBool);
            this.Controls.Add(this.edtLength);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.edtOffset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.edtDBNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("新宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox edtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edtIPAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox edtDBNumber;
        private System.Windows.Forms.TextBox edtOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox edtLength;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnReadBool;
        private System.Windows.Forms.Button btnWriteBool;
        private System.Windows.Forms.Button btnWriteByte;
        private System.Windows.Forms.Button btnReadByte;
        private System.Windows.Forms.Button btnWriteWord;
        private System.Windows.Forms.Button btnReadWord;
        private System.Windows.Forms.Button btnWriteInt;
        private System.Windows.Forms.Button btnReadint;
        private System.Windows.Forms.Button btnWriteDWord;
        private System.Windows.Forms.Button btnReadDWord;
        private System.Windows.Forms.Button btnWriteReal;
        private System.Windows.Forms.Button btnReadReal;
        private System.Windows.Forms.Button btnWriteChars;
        private System.Windows.Forms.Button btnReadChars;
        private System.Windows.Forms.TextBox edtText;
    }
}

