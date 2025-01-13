using System.Windows.Controls;
using System.Windows.Forms;

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
            this.send = new System.Windows.Forms.Button();
            this.model_answer = new System.Windows.Forms.ListBox();
            this.input_text = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(835, 462);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(103, 126);
            this.send.TabIndex = 0;
            this.send.Text = "Send";
            // 
            // model_answer
            // 
            this.model_answer.ItemHeight = 12;
            this.model_answer.Location = new System.Drawing.Point(12, 380);
            this.model_answer.Name = "model_answer";
            this.model_answer.Size = new System.Drawing.Size(926, 76);
            this.model_answer.TabIndex = 0;
            // 
            // input_text
            // 
            this.input_text.Location = new System.Drawing.Point(12, 462);
            this.input_text.Multiline = true;
            this.input_text.Name = "input_text";
            this.input_text.Size = new System.Drawing.Size(817, 126);
            this.input_text.TabIndex = 1;
            // 
            // Form_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Controls.Add(this.send);
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
    }
}