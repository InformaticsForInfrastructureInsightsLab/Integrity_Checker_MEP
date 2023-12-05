namespace ClashTest2
{
    partial class form_ResultViewer
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("MAJOR", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("MEDIUM", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("MINOR", System.Windows.Forms.HorizontalAlignment.Left);
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ID_GUID1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.Download = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.collapsibleListView1 = new ListviewTest.CollapsibleListView();
            this.majorHard = new System.Windows.Forms.Label();
            this.majorSoft = new System.Windows.Forms.Label();
            this.mediumHard = new System.Windows.Forms.Label();
            this.mediumSoft = new System.Windows.Forms.Label();
            this.minorHard = new System.Windows.Forms.Label();
            this.minorSoft = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(591, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 426);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 2;
            // 
            // ID_GUID1
            // 
            this.ID_GUID1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ID_GUID1.AutoSize = true;
            this.ID_GUID1.Location = new System.Drawing.Point(12, 444);
            this.ID_GUID1.Name = "ID_GUID1";
            this.ID_GUID1.Size = new System.Drawing.Size(257, 12);
            this.ID_GUID1.TabIndex = 4;
            this.ID_GUID1.Text = "Guid1:                                             Guid2:";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(591, 70);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(47, 22);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Hard";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox2.Enabled = false;
            this.checkBox2.Location = new System.Drawing.Point(638, 70);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(47, 22);
            this.checkBox2.TabIndex = 7;
            this.checkBox2.Text = "Soft";
            this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // Download
            // 
            this.Download.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Download.Location = new System.Drawing.Point(591, 12);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(94, 23);
            this.Download.TabIndex = 8;
            this.Download.Text = "Download";
            this.Download.UseVisualStyleBackColor = true;
            this.Download.Click += new System.EventHandler(this.button2_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(591, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Select header";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(12, 420);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Item1";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(93, 420);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Item2";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(591, 127);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(104, 16);
            this.checkBox3.TabIndex = 12;
            this.checkBox3.Text = "선택 외 투명화";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // collapsibleListView1
            // 
            this.collapsibleListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.collapsibleListView1.FullRowSelect = true;
            this.collapsibleListView1.GridLines = true;
            listViewGroup1.Header = "MAJOR";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "MEDIUM";
            listViewGroup2.Name = "listViewGroup2";
            listViewGroup3.Header = "MINOR";
            listViewGroup3.Name = "listViewGroup3";
            this.collapsibleListView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.collapsibleListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.collapsibleListView1.HideSelection = false;
            this.collapsibleListView1.Location = new System.Drawing.Point(12, 12);
            this.collapsibleListView1.Name = "collapsibleListView1";
            this.collapsibleListView1.Size = new System.Drawing.Size(560, 402);
            this.collapsibleListView1.TabIndex = 5;
            this.collapsibleListView1.UseCompatibleStateImageBehavior = false;
            this.collapsibleListView1.View = System.Windows.Forms.View.Details;
            this.collapsibleListView1.SelectedIndexChanged += new System.EventHandler(this.collapsibleListView1_SelectedIndexChanged);
            // 
            // majorHard
            // 
            this.majorHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.majorHard.AutoSize = true;
            this.majorHard.Location = new System.Drawing.Point(589, 157);
            this.majorHard.Name = "majorHard";
            this.majorHard.Size = new System.Drawing.Size(89, 12);
            this.majorHard.TabIndex = 13;
            this.majorHard.Text = "MAJOR_HARD:";
            this.majorHard.Visible = false;
            // 
            // majorSoft
            // 
            this.majorSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.majorSoft.AutoSize = true;
            this.majorSoft.Location = new System.Drawing.Point(589, 177);
            this.majorSoft.Name = "majorSoft";
            this.majorSoft.Size = new System.Drawing.Size(89, 12);
            this.majorSoft.TabIndex = 14;
            this.majorSoft.Text = "MAJOR_SOFT:\r\n";
            this.majorSoft.Visible = false;
            // 
            // mediumHard
            // 
            this.mediumHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mediumHard.AutoSize = true;
            this.mediumHard.Location = new System.Drawing.Point(589, 197);
            this.mediumHard.Name = "mediumHard";
            this.mediumHard.Size = new System.Drawing.Size(96, 12);
            this.mediumHard.TabIndex = 15;
            this.mediumHard.Text = "MEDIUM_HARD:";
            this.mediumHard.Visible = false;
            // 
            // mediumSoft
            // 
            this.mediumSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mediumSoft.AutoSize = true;
            this.mediumSoft.Location = new System.Drawing.Point(589, 217);
            this.mediumSoft.Name = "mediumSoft";
            this.mediumSoft.Size = new System.Drawing.Size(96, 12);
            this.mediumSoft.TabIndex = 16;
            this.mediumSoft.Text = "MEDIUM_SOFT:";
            this.mediumSoft.Visible = false;
            // 
            // minorHard
            // 
            this.minorHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minorHard.AutoSize = true;
            this.minorHard.Location = new System.Drawing.Point(589, 237);
            this.minorHard.Name = "minorHard";
            this.minorHard.Size = new System.Drawing.Size(87, 12);
            this.minorHard.TabIndex = 17;
            this.minorHard.Text = "MINOR_HARD:";
            this.minorHard.Visible = false;
            // 
            // minorSoft
            // 
            this.minorSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minorSoft.AutoSize = true;
            this.minorSoft.Location = new System.Drawing.Point(589, 257);
            this.minorSoft.Name = "minorSoft";
            this.minorSoft.Size = new System.Drawing.Size(87, 12);
            this.minorSoft.TabIndex = 18;
            this.minorSoft.Text = "MINOR_SOFT:";
            this.minorSoft.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 466);
            this.Controls.Add(this.minorSoft);
            this.Controls.Add(this.minorHard);
            this.Controls.Add(this.mediumSoft);
            this.Controls.Add(this.mediumHard);
            this.Controls.Add(this.majorSoft);
            this.Controls.Add(this.majorHard);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Download);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.collapsibleListView1);
            this.Controls.Add(this.ID_GUID1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Integrity Checker-MEP Response";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ID_GUID1;
        private ListviewTest.CollapsibleListView collapsibleListView1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label majorHard;
        private System.Windows.Forms.Label majorSoft;
        private System.Windows.Forms.Label mediumHard;
        private System.Windows.Forms.Label mediumSoft;
        private System.Windows.Forms.Label minorHard;
        private System.Windows.Forms.Label minorSoft;
    }
}

