using BrightIdeasSoftware;
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
            this.tabAdjustment = new System.Windows.Forms.TabPage();
            this.folv = new BrightIdeasSoftware.FastObjectListView();
            this.Elem1Type = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Elem2Type = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Elem1Guid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Elem2Guid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Severity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Adjusted_Severity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Penetration = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.MovabilityResult = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlMain.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tabAdjustment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folv)).BeginInit();
            this.SuspendLayout();
            // 
            // radio_adj
            // 
            this.radio_adj.AutoSize = true;
            this.radio_adj.Checked = true;
            this.radio_adj.Location = new System.Drawing.Point(3, 25);
            this.radio_adj.Name = "radio_adj";
            this.radio_adj.Size = new System.Drawing.Size(99, 16);
            this.radio_adj.TabIndex = 0;
            this.radio_adj.TabStop = true;
            this.radio_adj.Text = "재조정 심각도";
            this.radio_adj.UseVisualStyleBackColor = true;
            // 
            // radio_origin
            // 
            this.radio_origin.AutoSize = true;
            this.radio_origin.Location = new System.Drawing.Point(3, 3);
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
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.radio_origin);
            this.tabDashboard.Controls.Add(this.radio_adj);
            this.tabDashboard.Location = new System.Drawing.Point(4, 22);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Size = new System.Drawing.Size(1192, 574);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "심각도 대시보드";
            // 
            // tabAdjustment
            // 
            this.tabAdjustment.Controls.Add(this.folv);
            this.tabAdjustment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabAdjustment.Location = new System.Drawing.Point(4, 22);
            this.tabAdjustment.Name = "tabAdjustment";
            this.tabAdjustment.Size = new System.Drawing.Size(1192, 574);
            this.tabAdjustment.TabIndex = 1;
            this.tabAdjustment.Text = "심각도 조정 리스트";
            // 
            // folv
            // 
            this.folv.AllColumns.Add(this.Elem1Type);
            this.folv.AllColumns.Add(this.Elem2Type);
            this.folv.AllColumns.Add(this.Elem1Guid);
            this.folv.AllColumns.Add(this.Elem2Guid);
            this.folv.AllColumns.Add(this.Severity);
            this.folv.AllColumns.Add(this.Adjusted_Severity);
            this.folv.AllColumns.Add(this.Penetration);
            this.folv.AllColumns.Add(this.MovabilityResult);
            this.folv.CellEditUseWholeCell = false;
            this.folv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Elem1Type,
            this.Elem2Type,
            this.Elem1Guid,
            this.Elem2Guid,
            this.Severity,
            this.Adjusted_Severity,
            this.Penetration,
            this.MovabilityResult});
            this.folv.Cursor = System.Windows.Forms.Cursors.Default;
            this.folv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folv.HideSelection = false;
            this.folv.Location = new System.Drawing.Point(0, 0);
            this.folv.Name = "folv";
            this.folv.ShowGroups = false;
            this.folv.Size = new System.Drawing.Size(1192, 574);
            this.folv.TabIndex = 0;
            this.folv.UseCellFormatEvents = true;
            this.folv.UseCompatibleStateImageBehavior = false;
            this.folv.UseFiltering = true;
            this.folv.View = System.Windows.Forms.View.Details;
            this.folv.VirtualMode = true;
            // 
            // Elem1Type
            // 
            this.Elem1Type.AspectName = "Element1Type";
            this.Elem1Type.Text = "Element1 Type";
            this.Elem1Type.Width = 100;
            // 
            // Elem2Type
            // 
            this.Elem2Type.AspectName = "Element2Type";
            this.Elem2Type.Text = "Element2 Type";
            this.Elem2Type.Width = 100;
            // 
            // Elem1Guid
            // 
            this.Elem1Guid.AspectName = "Element1Guid";
            this.Elem1Guid.Text = "Element1 GUID";
            this.Elem1Guid.Width = 100;
            // 
            // Elem2Guid
            // 
            this.Elem2Guid.AspectName = "Element2Guid";
            this.Elem2Guid.Text = "Element2 GUID";
            this.Elem2Guid.Width = 100;
            // 
            // Severity
            // 
            this.Severity.AspectName = "Severity";
            this.Severity.Text = "Severity";
            this.Severity.Width = 100;
            // 
            // Adjusted_Severity
            // 
            this.Adjusted_Severity.AspectName = "Adjusted_Severity";
            this.Adjusted_Severity.Text = "Adjusted Severity";
            this.Adjusted_Severity.Width = 100;
            // 
            // Penetration
            // 
            this.Penetration.AspectName = "Penetration";
            this.Penetration.Text = "Penetration";
            this.Penetration.Width = 80;
            // 
            // MovabilityResult
            // 
            this.MovabilityResult.AspectName = "MovabilityResult";
            this.MovabilityResult.Text = "MovabilityResult";
            this.MovabilityResult.Width = 125;
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout.Size = new System.Drawing.Size(1192, 574);
            this.tableLayout.TabIndex = 23;
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
            this.tabDashboard.PerformLayout();
            this.tabAdjustment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.folv)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabDashboard;
        private System.Windows.Forms.TabPage tabAdjustment;
        private System.Windows.Forms.RadioButton radio_adj;
        private System.Windows.Forms.RadioButton radio_origin;
        private TableLayoutPanel tableLayout;

        private FastObjectListView folv;
        private BrightIdeasSoftware.OLVColumn Elem1Type;
        private BrightIdeasSoftware.OLVColumn Elem2Type;
        private BrightIdeasSoftware.OLVColumn Elem1Guid;
        private BrightIdeasSoftware.OLVColumn Elem2Guid;
        private BrightIdeasSoftware.OLVColumn Severity;
        private BrightIdeasSoftware.OLVColumn Adjusted_Severity;
        private OLVColumn Penetration;
        private OLVColumn MovabilityResult;
    }
}