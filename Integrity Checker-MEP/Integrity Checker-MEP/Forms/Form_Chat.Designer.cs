using System.Windows.Controls;
using System.Windows.Forms;
using OxyPlot.WindowsForms;

namespace Integrity_Checker_MEP
{
    partial class Form_Chat
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
            this.plot_view = new OxyPlot.WindowsForms.PlotView();
            this.model_answer = new System.Windows.Forms.ListBox();
            this.input_text = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // plot_view
            // 
            this.plot_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plot_view.Location = new System.Drawing.Point(0, 0);
            this.plot_view.Name = "plot_view";
            this.plot_view.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plot_view.Size = new System.Drawing.Size(800, 450);
            this.plot_view.TabIndex = 0;
            this.plot_view.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plot_view.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plot_view.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // model_answer
            // 
            this.model_answer.ItemHeight = 12;
            this.model_answer.Location = new System.Drawing.Point(0, 380);
            this.model_answer.Name = "model_answer";
            this.model_answer.Size = new System.Drawing.Size(950, 76);
            this.model_answer.TabIndex = 0;
            // 
            // input_text
            // 
            this.input_text.Location = new System.Drawing.Point(0, 462);
            this.input_text.Multiline = true;
            this.input_text.Name = "input_text";
            this.input_text.Size = new System.Drawing.Size(950, 138);
            this.input_text.TabIndex = 1;
            this.input_text.TextChanged += new System.EventHandler(this.input_text_TextChanged);
            // 
            // Form_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Controls.Add(this.model_answer);
            this.Controls.Add(this.input_text);
            this.Name = "Form_Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox model_answer;
        private System.Windows.Forms.TextBox input_text;
        private System.Windows.Forms.Button send;
        private PlotView plot_view;
    }
}