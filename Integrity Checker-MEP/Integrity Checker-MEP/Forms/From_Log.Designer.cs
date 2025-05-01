namespace Integrity_Checker_MEP {
    partial class From_Log {
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
            this.pgb_total = new System.Windows.Forms.ProgressBar();
            this.pgb_current = new System.Windows.Forms.ProgressBar();
            this.lst_progress = new System.Windows.Forms.ListBox();
            this.btn_savelog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pgb_total
            // 
            this.pgb_total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgb_total.Location = new System.Drawing.Point(12, 12);
            this.pgb_total.Name = "pgb_total";
            this.pgb_total.Size = new System.Drawing.Size(410, 23);
            this.pgb_total.TabIndex = 0;
            // 
            // pgb_current
            // 
            this.pgb_current.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgb_current.Location = new System.Drawing.Point(12, 41);
            this.pgb_current.Name = "pgb_current";
            this.pgb_current.Size = new System.Drawing.Size(410, 23);
            this.pgb_current.TabIndex = 1;
            // 
            // lst_progress
            // 
            this.lst_progress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lst_progress.FormattingEnabled = true;
            this.lst_progress.ItemHeight = 12;
            this.lst_progress.Location = new System.Drawing.Point(11, 78);
            this.lst_progress.Name = "lst_progress";
            this.lst_progress.Size = new System.Drawing.Size(411, 736);
            this.lst_progress.TabIndex = 2;
            // 
            // btn_savelog
            // 
            this.btn_savelog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_savelog.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_savelog.Location = new System.Drawing.Point(338, 823);
            this.btn_savelog.Name = "btn_savelog";
            this.btn_savelog.Size = new System.Drawing.Size(83, 26);
            this.btn_savelog.TabIndex = 3;
            this.btn_savelog.Text = "save log";
            this.btn_savelog.UseVisualStyleBackColor = true;
            this.btn_savelog.Click += new System.EventHandler(this.btn_savelog_Click);
            // 
            // Form_Progress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 854);
            this.Controls.Add(this.btn_savelog);
            this.Controls.Add(this.lst_progress);
            this.Controls.Add(this.pgb_current);
            this.Controls.Add(this.pgb_total);
            this.Name = "Form_Progress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Log";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgb_total;
        private System.Windows.Forms.ProgressBar pgb_current;
        public System.Windows.Forms.ListBox lst_progress;
        private System.Windows.Forms.Button btn_savelog;
    }
}