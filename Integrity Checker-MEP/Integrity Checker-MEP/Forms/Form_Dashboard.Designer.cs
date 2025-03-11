using System.Windows.Controls;
using System.Windows.Forms;

namespace Integrity_Checker_MEP.Forms
{
    partial class Form_Dashboard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radio_adj = new System.Windows.Forms.RadioButton();
            this.radio_origin = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radio_adj
            // 
            this.radio_adj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radio_adj.AutoSize = true;
            this.radio_adj.Location = new System.Drawing.Point(10, 10);
            this.radio_adj.Name = "radio_adj";
            this.radio_adj.Size = new System.Drawing.Size(99, 16);
            this.radio_adj.TabIndex = 22;
            this.radio_adj.Text = "재조정 심각도";
            this.radio_adj.UseVisualStyleBackColor = true;
            this.radio_adj.Checked = true;
            this.radio_origin.CheckedChanged += new System.EventHandler(this.RadioChanged);
            // 
            // radio_origin
            // 
            this.radio_origin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radio_origin.AutoSize = true;
            this.radio_origin.Location = new System.Drawing.Point(10, 30);
            this.radio_origin.Name = "radio_origin";
            this.radio_origin.Size = new System.Drawing.Size(139, 16);
            this.radio_origin.TabIndex = 21;
            this.radio_origin.Text = "조정되지 않은 심각도";
            this.radio_origin.UseVisualStyleBackColor = true;
            this.radio_origin.CheckedChanged += new System.EventHandler(this.RadioChanged);
            // 
            // tableLayout
            // 
            tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            // 
            // Form_Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.radio_adj);
            this.Controls.Add(this.radio_origin);
            this.Name = "Form_Dashboard";
            this.Text = "Form_Dashboard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.RadioButton radio_adj;
        private System.Windows.Forms.RadioButton radio_origin;
        private TableLayoutPanel tableLayout;
    }
}