using System.Windows.Controls;

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
            this.chat_list = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chat_list
            // 
            this.chat_list.ItemHeight = 12;
            this.chat_list.Location = new System.Drawing.Point(1, 416);
            this.chat_list.Name = "chat_list";
            this.chat_list.Size = new System.Drawing.Size(985, 172);
            this.chat_list.TabIndex = 0;
            // 
            // Form_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 585);
            this.Controls.Add(this.chat_list);
            this.Name = "Form_Chat";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox chat_list;
        private System.Windows.Forms.TextBox input_text;
        private System.Windows.Forms.Button send;
    }
}