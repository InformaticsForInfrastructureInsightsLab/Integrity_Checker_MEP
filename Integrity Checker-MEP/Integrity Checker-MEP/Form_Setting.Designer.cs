namespace Integrity_Checker_MEP {
    partial class Form_Setting {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.cb_parent = new System.Windows.Forms.CheckBox();
            this.cb_exportResult = new System.Windows.Forms.CheckBox();
            this.cb_allInOne = new System.Windows.Forms.CheckBox();
            this.cb_properties = new System.Windows.Forms.CheckBox();
            this.cb_saveimage = new System.Windows.Forms.CheckBox();
            this.cb_server = new System.Windows.Forms.CheckBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.cb_savelog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cb_parent
            // 
            this.cb_parent.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_parent.AutoSize = true;
            this.cb_parent.Location = new System.Drawing.Point(58, 37);
            this.cb_parent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cb_parent.Name = "cb_parent";
            this.cb_parent.Size = new System.Drawing.Size(128, 16);
            this.cb_parent.TabIndex = 0;
            this.cb_parent.Text = "부모GUID획득 표시";
            this.cb_parent.UseVisualStyleBackColor = true;
            this.cb_parent.CheckedChanged += new System.EventHandler(this.cb_parent_CheckedChanged);
            // 
            // cb_exportResult
            // 
            this.cb_exportResult.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_exportResult.AutoSize = true;
            this.cb_exportResult.Checked = true;
            this.cb_exportResult.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_exportResult.Location = new System.Drawing.Point(58, 59);
            this.cb_exportResult.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cb_exportResult.Name = "cb_exportResult";
            this.cb_exportResult.Size = new System.Drawing.Size(100, 16);
            this.cb_exportResult.TabIndex = 1;
            this.cb_exportResult.Text = "간섭결과 출력";
            this.cb_exportResult.UseVisualStyleBackColor = true;
            this.cb_exportResult.CheckedChanged += new System.EventHandler(this.cb_exportResult_CheckedChanged);
            // 
            // cb_allInOne
            // 
            this.cb_allInOne.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_allInOne.AutoSize = true;
            this.cb_allInOne.Checked = true;
            this.cb_allInOne.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_allInOne.Location = new System.Drawing.Point(58, 81);
            this.cb_allInOne.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cb_allInOne.Name = "cb_allInOne";
            this.cb_allInOne.Size = new System.Drawing.Size(107, 16);
            this.cb_allInOne.TabIndex = 2;
            this.cb_allInOne.Text = "All In One 출력";
            this.cb_allInOne.UseVisualStyleBackColor = true;
            this.cb_allInOne.CheckedChanged += new System.EventHandler(this.cb_allInOne_CheckedChanged);
            // 
            // cb_properties
            // 
            this.cb_properties.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_properties.AutoSize = true;
            this.cb_properties.Checked = true;
            this.cb_properties.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_properties.Location = new System.Drawing.Point(58, 103);
            this.cb_properties.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cb_properties.Name = "cb_properties";
            this.cb_properties.Size = new System.Drawing.Size(109, 16);
            this.cb_properties.TabIndex = 5;
            this.cb_properties.Text = "Properites 출력";
            this.cb_properties.UseVisualStyleBackColor = true;
            this.cb_properties.CheckedChanged += new System.EventHandler(this.cb_properties_CheckedChanged);
            // 
            // cb_saveimage
            // 
            this.cb_saveimage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_saveimage.AutoSize = true;
            this.cb_saveimage.Location = new System.Drawing.Point(58, 125);
            this.cb_saveimage.Name = "cb_saveimage";
            this.cb_saveimage.Size = new System.Drawing.Size(88, 16);
            this.cb_saveimage.TabIndex = 11;
            this.cb_saveimage.Text = "이미지 저장";
            this.cb_saveimage.UseVisualStyleBackColor = true;
            this.cb_saveimage.CheckedChanged += new System.EventHandler(this.cb_saveimage_CheckedChanged);
            // 
            // cb_server
            // 
            this.cb_server.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cb_server.AutoSize = true;
            this.cb_server.Checked = true;
            this.cb_server.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_server.Location = new System.Drawing.Point(58, 147);
            this.cb_server.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cb_server.Name = "cb_server";
            this.cb_server.Size = new System.Drawing.Size(76, 16);
            this.cb_server.TabIndex = 6;
            this.cb_server.Text = "서버 전송";
            this.cb_server.UseVisualStyleBackColor = true;
            this.cb_server.CheckedChanged += new System.EventHandler(this.cb_server_CheckedChanged);
            // 
            // btn_start
            // 
            this.btn_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_start.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_start.Location = new System.Drawing.Point(11, 219);
            this.btn_start.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(143, 38);
            this.btn_start.TabIndex = 8;
            this.btn_start.Text = "간섭검토 시작";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_cancel.Location = new System.Drawing.Point(160, 219);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(62, 38);
            this.btn_cancel.TabIndex = 9;
            this.btn_cancel.Text = "취소";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // cb_savelog
            // 
            this.cb_savelog.AutoSize = true;
            this.cb_savelog.Location = new System.Drawing.Point(58, 169);
            this.cb_savelog.Name = "cb_savelog";
            this.cb_savelog.Size = new System.Drawing.Size(69, 16);
            this.cb_savelog.TabIndex = 10;
            this.cb_savelog.Text = "Log저장";
            this.cb_savelog.UseVisualStyleBackColor = true;
            this.cb_savelog.CheckedChanged += new System.EventHandler(this.cb_savelog_CheckedChanged);
            // 
            // Form_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(234, 269);
            this.ControlBox = false;
            this.Controls.Add(this.cb_saveimage);
            this.Controls.Add(this.cb_savelog);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.cb_server);
            this.Controls.Add(this.cb_properties);
            this.Controls.Add(this.cb_allInOne);
            this.Controls.Add(this.cb_exportResult);
            this.Controls.Add(this.cb_parent);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "Form_Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Integrity Checker-MEP Request";
            this.Load += new System.EventHandler(this.Form_Setting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_parent;
        private System.Windows.Forms.CheckBox cb_exportResult;
        private System.Windows.Forms.CheckBox cb_allInOne;
        private System.Windows.Forms.CheckBox cb_properties;
        private System.Windows.Forms.CheckBox cb_saveimage;
        private System.Windows.Forms.CheckBox cb_server;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.CheckBox cb_savelog;
       
    }
}