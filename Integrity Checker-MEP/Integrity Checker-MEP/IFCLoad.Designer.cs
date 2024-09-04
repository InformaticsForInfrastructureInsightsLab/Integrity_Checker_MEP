using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Integrity_Checker_MEP
{
    partial class IFCLoad
    {
        FlowLayoutPanel cb_panel;
        Button btn_save = new Button();
        Button btn_cancel = new Button();

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cb_panel
            // 
            this.cb_panel.AutoScroll = true;
            this.cb_panel.Location = new System.Drawing.Point(10, 10);
            this.cb_panel.Name = "cb_panel";
            this.cb_panel.Size = new System.Drawing.Size(200, 250);
            this.cb_panel.TabIndex = 0;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(12, 266);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(118, 38);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "선택 파일 로드";
            this.btn_save.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(136, 270);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(68, 30);
            this.btn_cancel.TabIndex = 2;
            this.btn_cancel.Text = "취소";
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // IFCLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 325);
            this.Controls.Add(this.cb_panel);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_cancel);
            this.Name = "IFCLoad";
            this.Text = "Model Loader";
            this.ResumeLayout(false);

        }
        #endregion
    }
}