namespace Integrity_Checker_MEP
{
    partial class form_ImageOption
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
            this.cb_Background = new System.Windows.Forms.CheckBox();
            this.cb_Transparancy = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.cb_magnification = new System.Windows.Forms.ComboBox();
            this.lbl_magnification = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_Background
            // 
            this.cb_Background.AutoSize = true;
            this.cb_Background.Location = new System.Drawing.Point(12, 17);
            this.cb_Background.Name = "cb_Background";
            this.cb_Background.Size = new System.Drawing.Size(127, 16);
            this.cb_Background.TabIndex = 0;
            this.cb_Background.Text = "Show Background";
            this.cb_Background.UseVisualStyleBackColor = true;
            this.cb_Background.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cb_Transparancy
            // 
            this.cb_Transparancy.AutoSize = true;
            this.cb_Transparancy.Location = new System.Drawing.Point(12, 39);
            this.cb_Transparancy.Name = "cb_Transparancy";
            this.cb_Transparancy.Size = new System.Drawing.Size(125, 16);
            this.cb_Transparancy.TabIndex = 1;
            this.cb_Transparancy.Text = "Set Transparancy";
            this.cb_Transparancy.UseVisualStyleBackColor = true;
            this.cb_Transparancy.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 105);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(118, 105);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 33);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(143, 38);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(38, 21);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // cb_magnification
            // 
            this.cb_magnification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_magnification.FormattingEnabled = true;
            this.cb_magnification.Location = new System.Drawing.Point(12, 78);
            this.cb_magnification.Name = "cb_magnification";
            this.cb_magnification.Size = new System.Drawing.Size(163, 20);
            this.cb_magnification.TabIndex = 5;
            // 
            // lbl_magnification
            // 
            this.lbl_magnification.AutoSize = true;
            this.lbl_magnification.Location = new System.Drawing.Point(30, 61);
            this.lbl_magnification.Name = "lbl_magnification";
            this.lbl_magnification.Size = new System.Drawing.Size(80, 12);
            this.lbl_magnification.TabIndex = 6;
            this.lbl_magnification.Text = "Magnification";
            // 
            // form_ImageOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 150);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cb_Transparancy);
            this.Controls.Add(this.cb_Background);
            this.Controls.Add(this.cb_magnification);
            this.Controls.Add(this.lbl_magnification);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_ImageOption";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Image Setting";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_Background;
        private System.Windows.Forms.CheckBox cb_Transparancy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ComboBox cb_magnification;
        private System.Windows.Forms.Label lbl_magnification;
    }
}