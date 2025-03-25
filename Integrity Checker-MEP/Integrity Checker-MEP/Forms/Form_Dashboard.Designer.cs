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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tabAdjustment = new System.Windows.Forms.TabPage();
            this.tabControlMain.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // radio_adj
            // 
            this.radio_adj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radio_adj.AutoSize = true;
            this.radio_adj.Checked = true;
            this.radio_adj.Location = new System.Drawing.Point(10, 10);
            this.radio_adj.Name = "radio_adj";
            this.radio_adj.Size = new System.Drawing.Size(99, 16);
            this.radio_adj.Text = "재조정 심각도";
            this.radio_adj.UseVisualStyleBackColor = true;
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
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabDashboard);
            this.tabControlMain.Controls.Add(this.tabAdjustment);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1200, 600);
            this.tabControlMain.TabIndex = 0;
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout.Controls.Add(this.radio_adj, 1, 0);
            this.tableLayout.Controls.Add(this.radio_origin, 0, 0);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout.Size = new System.Drawing.Size(1192, 574);
            this.tableLayout.TabIndex = 23;
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.radio_adj);
            this.tabDashboard.Controls.Add(this.radio_origin);
            this.tabDashboard.Location = new System.Drawing.Point(4, 22);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Size = new System.Drawing.Size(1192, 574);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "심각도 대시보드";
            // 
            // tabAdjustment
            // 
            this.tabAdjustment.Location = new System.Drawing.Point(4, 22);
            this.tabAdjustment.Name = "tabAdjustment";
            this.tabAdjustment.Size = new System.Drawing.Size(192, 74);
            this.tabAdjustment.TabIndex = 1;
            this.tabAdjustment.Text = "심각도 조정 종류";
            // 
            // Form_Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.tabControlMain);
            this.Name = "Form_Dashboard";
            this.Text = "Form_Dashboard";
            this.tabControlMain.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabDashboard;
        private System.Windows.Forms.TabPage tabAdjustment;
        private System.Windows.Forms.RadioButton radio_adj;
        private System.Windows.Forms.RadioButton radio_origin;
        private TableLayoutPanel tableLayout;
    }
}