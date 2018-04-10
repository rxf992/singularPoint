namespace stepperMatrix
{
    partial class MainForm
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_RTCN = new System.Windows.Forms.TextBox();
            this.showMap = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBoxCoordinate = new System.Windows.Forms.GroupBox();
            this.textBoxCol = new System.Windows.Forms.TextBox();
            this.labelCol = new System.Windows.Forms.Label();
            this.labelRow = new System.Windows.Forms.Label();
            this.textBoxRow = new System.Windows.Forms.TextBox();
            this.checkMatrix = new System.Windows.Forms.CheckBox();
            this.checkRow = new System.Windows.Forms.CheckBox();
            this.groupBoxCommand = new System.Windows.Forms.GroupBox();
            this.btnLoadMatrix = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnSpeedDown = new System.Windows.Forms.Button();
            this.btnSpeedUp = new System.Windows.Forms.Button();
            this.btnSetAcc = new System.Windows.Forms.Button();
            this.textBoxACC = new System.Windows.Forms.TextBox();
            this.btnSetSpeed = new System.Windows.Forms.Button();
            this.textBoxSpeed = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.groupBoxCoordinate.SuspendLayout();
            this.groupBoxCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_RTCN
            // 
            this.textBox_RTCN.Location = new System.Drawing.Point(12, 129);
            this.textBox_RTCN.Multiline = true;
            this.textBox_RTCN.Name = "textBox_RTCN";
            this.textBox_RTCN.Size = new System.Drawing.Size(761, 236);
            this.textBox_RTCN.TabIndex = 3;
            // 
            // showMap
            // 
            this.showMap.Location = new System.Drawing.Point(454, 59);
            this.showMap.Name = "showMap";
            this.showMap.Size = new System.Drawing.Size(75, 23);
            this.showMap.TabIndex = 5;
            this.showMap.Text = "保存矩阵";
            this.showMap.UseVisualStyleBackColor = true;
            this.showMap.Click += new System.EventHandler(this.showMap_Click);
            // 
            // groupBoxCoordinate
            // 
            this.groupBoxCoordinate.Controls.Add(this.textBoxCol);
            this.groupBoxCoordinate.Controls.Add(this.labelCol);
            this.groupBoxCoordinate.Controls.Add(this.labelRow);
            this.groupBoxCoordinate.Controls.Add(this.textBoxRow);
            this.groupBoxCoordinate.Controls.Add(this.checkMatrix);
            this.groupBoxCoordinate.Controls.Add(this.checkRow);
            this.groupBoxCoordinate.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCoordinate.Name = "groupBoxCoordinate";
            this.groupBoxCoordinate.Size = new System.Drawing.Size(200, 100);
            this.groupBoxCoordinate.TabIndex = 7;
            this.groupBoxCoordinate.TabStop = false;
            this.groupBoxCoordinate.Text = "坐标选择";
            // 
            // textBoxCol
            // 
            this.textBoxCol.Location = new System.Drawing.Point(47, 61);
            this.textBoxCol.MaxLength = 6;
            this.textBoxCol.Name = "textBoxCol";
            this.textBoxCol.Size = new System.Drawing.Size(48, 21);
            this.textBoxCol.TabIndex = 5;
            this.textBoxCol.Text = "1";
            this.textBoxCol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxCol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCol_KeyPress);
            // 
            // labelCol
            // 
            this.labelCol.AutoSize = true;
            this.labelCol.Location = new System.Drawing.Point(17, 66);
            this.labelCol.Name = "labelCol";
            this.labelCol.Size = new System.Drawing.Size(23, 12);
            this.labelCol.TabIndex = 4;
            this.labelCol.Text = "列:";
            // 
            // labelRow
            // 
            this.labelRow.AutoSize = true;
            this.labelRow.Location = new System.Drawing.Point(17, 35);
            this.labelRow.Name = "labelRow";
            this.labelRow.Size = new System.Drawing.Size(23, 12);
            this.labelRow.TabIndex = 3;
            this.labelRow.Text = "行:";
            // 
            // textBoxRow
            // 
            this.textBoxRow.Location = new System.Drawing.Point(47, 31);
            this.textBoxRow.MaxLength = 6;
            this.textBoxRow.Name = "textBoxRow";
            this.textBoxRow.Size = new System.Drawing.Size(48, 21);
            this.textBoxRow.TabIndex = 2;
            this.textBoxRow.Text = "1";
            this.textBoxRow.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxRow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRow_KeyPress);
            // 
            // checkMatrix
            // 
            this.checkMatrix.AutoSize = true;
            this.checkMatrix.Location = new System.Drawing.Point(113, 33);
            this.checkMatrix.Name = "checkMatrix";
            this.checkMatrix.Size = new System.Drawing.Size(72, 16);
            this.checkMatrix.TabIndex = 1;
            this.checkMatrix.Text = "整个矩阵";
            this.checkMatrix.UseVisualStyleBackColor = true;
            this.checkMatrix.CheckedChanged += new System.EventHandler(this.checkMatrix_CheckedChanged);
            // 
            // checkRow
            // 
            this.checkRow.AutoSize = true;
            this.checkRow.Location = new System.Drawing.Point(113, 66);
            this.checkRow.Name = "checkRow";
            this.checkRow.Size = new System.Drawing.Size(48, 16);
            this.checkRow.TabIndex = 0;
            this.checkRow.Text = "整行";
            this.checkRow.UseVisualStyleBackColor = true;
            this.checkRow.CheckedChanged += new System.EventHandler(this.checkRow_CheckedChanged);
            // 
            // groupBoxCommand
            // 
            this.groupBoxCommand.Controls.Add(this.btnLoadMatrix);
            this.groupBoxCommand.Controls.Add(this.btnPlay);
            this.groupBoxCommand.Controls.Add(this.btnSync);
            this.groupBoxCommand.Controls.Add(this.btnSpeedDown);
            this.groupBoxCommand.Controls.Add(this.showMap);
            this.groupBoxCommand.Controls.Add(this.btnSpeedUp);
            this.groupBoxCommand.Controls.Add(this.btnSetAcc);
            this.groupBoxCommand.Controls.Add(this.textBoxACC);
            this.groupBoxCommand.Controls.Add(this.btnSetSpeed);
            this.groupBoxCommand.Controls.Add(this.textBoxSpeed);
            this.groupBoxCommand.Controls.Add(this.btnClose);
            this.groupBoxCommand.Controls.Add(this.btnEnable);
            this.groupBoxCommand.Location = new System.Drawing.Point(227, 12);
            this.groupBoxCommand.Name = "groupBoxCommand";
            this.groupBoxCommand.Size = new System.Drawing.Size(546, 100);
            this.groupBoxCommand.TabIndex = 8;
            this.groupBoxCommand.TabStop = false;
            this.groupBoxCommand.Text = "命令";
            // 
            // btnLoadMatrix
            // 
            this.btnLoadMatrix.Location = new System.Drawing.Point(360, 59);
            this.btnLoadMatrix.Name = "btnLoadMatrix";
            this.btnLoadMatrix.Size = new System.Drawing.Size(75, 23);
            this.btnLoadMatrix.TabIndex = 7;
            this.btnLoadMatrix.Text = "加载矩阵";
            this.btnLoadMatrix.UseVisualStyleBackColor = true;
            this.btnLoadMatrix.Click += new System.EventHandler(this.btnLoadMatrix_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(454, 21);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 6;
            this.btnPlay.Text = "播放默认";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(278, 59);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 5;
            this.btnSync.Text = "同步";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnSpeedDown
            // 
            this.btnSpeedDown.Location = new System.Drawing.Point(187, 59);
            this.btnSpeedDown.Name = "btnSpeedDown";
            this.btnSpeedDown.Size = new System.Drawing.Size(75, 23);
            this.btnSpeedDown.TabIndex = 4;
            this.btnSpeedDown.Text = "减速1000";
            this.btnSpeedDown.UseVisualStyleBackColor = true;
            this.btnSpeedDown.Click += new System.EventHandler(this.btnSpeedDown_Click);
            // 
            // btnSpeedUp
            // 
            this.btnSpeedUp.Location = new System.Drawing.Point(106, 59);
            this.btnSpeedUp.Name = "btnSpeedUp";
            this.btnSpeedUp.Size = new System.Drawing.Size(75, 23);
            this.btnSpeedUp.TabIndex = 4;
            this.btnSpeedUp.Text = "加速1000";
            this.btnSpeedUp.UseVisualStyleBackColor = true;
            this.btnSpeedUp.Click += new System.EventHandler(this.btnSpeedUp_Click);
            // 
            // btnSetAcc
            // 
            this.btnSetAcc.Location = new System.Drawing.Point(359, 20);
            this.btnSetAcc.Name = "btnSetAcc";
            this.btnSetAcc.Size = new System.Drawing.Size(75, 23);
            this.btnSetAcc.TabIndex = 3;
            this.btnSetAcc.Text = "设置加速度";
            this.btnSetAcc.UseVisualStyleBackColor = true;
            this.btnSetAcc.Click += new System.EventHandler(this.btnSetAcc_Click);
            // 
            // textBoxACC
            // 
            this.textBoxACC.Location = new System.Drawing.Point(278, 21);
            this.textBoxACC.MaxLength = 6;
            this.textBoxACC.Name = "textBoxACC";
            this.textBoxACC.Size = new System.Drawing.Size(75, 21);
            this.textBoxACC.TabIndex = 2;
            this.textBoxACC.Text = "0";
            this.textBoxACC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxACC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxACC_KeyPress);
            // 
            // btnSetSpeed
            // 
            this.btnSetSpeed.Location = new System.Drawing.Point(187, 21);
            this.btnSetSpeed.Name = "btnSetSpeed";
            this.btnSetSpeed.Size = new System.Drawing.Size(75, 23);
            this.btnSetSpeed.TabIndex = 3;
            this.btnSetSpeed.Text = "设置速度";
            this.btnSetSpeed.UseVisualStyleBackColor = true;
            this.btnSetSpeed.Click += new System.EventHandler(this.btnSetSpeed_Click);
            // 
            // textBoxSpeed
            // 
            this.textBoxSpeed.Location = new System.Drawing.Point(106, 21);
            this.textBoxSpeed.MaxLength = 6;
            this.textBoxSpeed.Name = "textBoxSpeed";
            this.textBoxSpeed.Size = new System.Drawing.Size(75, 21);
            this.textBoxSpeed.TabIndex = 2;
            this.textBoxSpeed.Text = "0";
            this.textBoxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSpeed_KeyPress);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(14, 59);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnEnable
            // 
            this.btnEnable.Location = new System.Drawing.Point(14, 21);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(75, 23);
            this.btnEnable.TabIndex = 0;
            this.btnEnable.Text = "打开";
            this.btnEnable.UseVisualStyleBackColor = true;
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 379);
            this.Controls.Add(this.groupBoxCommand);
            this.Controls.Add(this.groupBoxCoordinate);
            this.Controls.Add(this.textBox_RTCN);
            this.Name = "MainForm";
            this.Text = "AMControlPannel@RealField";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxCoordinate.ResumeLayout(false);
            this.groupBoxCoordinate.PerformLayout();
            this.groupBoxCommand.ResumeLayout(false);
            this.groupBoxCommand.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_RTCN;
        private System.Windows.Forms.Button showMap;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBoxCoordinate;
        private System.Windows.Forms.Label labelRow;
        private System.Windows.Forms.TextBox textBoxRow;
        private System.Windows.Forms.CheckBox checkMatrix;
        private System.Windows.Forms.CheckBox checkRow;
        private System.Windows.Forms.TextBox textBoxCol;
        private System.Windows.Forms.Label labelCol;
        private System.Windows.Forms.GroupBox groupBoxCommand;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSetSpeed;
        private System.Windows.Forms.TextBox textBoxSpeed;
        private System.Windows.Forms.Button btnSpeedDown;
        private System.Windows.Forms.Button btnSpeedUp;
        private System.Windows.Forms.Button btnSetAcc;
        private System.Windows.Forms.TextBox textBoxACC;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnLoadMatrix;
    }
}

