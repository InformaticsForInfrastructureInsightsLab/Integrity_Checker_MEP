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
            this.btn_Load = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ID_GUID1 = new System.Windows.Forms.Label();
            this.tog_Hard = new System.Windows.Forms.CheckBox();
            this.tog_Soft = new System.Windows.Forms.CheckBox();
            this.btn_Download = new System.Windows.Forms.Button();
            this.btn_SelectHeader = new System.Windows.Forms.Button();
            this.btn_Item1 = new System.Windows.Forms.Button();
            this.btn_item2 = new System.Windows.Forms.Button();
            this.majorHard = new System.Windows.Forms.Label();
            this.majorSoft = new System.Windows.Forms.Label();
            this.mediumHard = new System.Windows.Forms.Label();
            this.mediumSoft = new System.Windows.Forms.Label();
            this.minorHard = new System.Windows.Forms.Label();
            this.minorSoft = new System.Windows.Forms.Label();
            this.rdo_hide = new System.Windows.Forms.RadioButton();
            this.rdo_trans = new System.Windows.Forms.RadioButton();
            this.rdo_none = new System.Windows.Forms.RadioButton();
            this.btn_Screenshot = new System.Windows.Forms.Button();
            this.folv = new BrightIdeasSoftware.FastObjectListView();
            this.Element1Guid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Element2Guid = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Type = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.MovabilityValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Topology = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.HardClashType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.SoftClashType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Severity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Clearance = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.MovabilityResult = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Offset = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.folv)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Load
            // 
            this.btn_Load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Load.Location = new System.Drawing.Point(643, 41);
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Size = new System.Drawing.Size(94, 23);
            this.btn_Load.TabIndex = 1;
            this.btn_Load.Text = "Load";
            this.btn_Load.UseVisualStyleBackColor = true;
            this.btn_Load.Click += new System.EventHandler(this.click_btn_Load);
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
            this.ID_GUID1.Location = new System.Drawing.Point(12, 531);
            this.ID_GUID1.Name = "ID_GUID1";
            this.ID_GUID1.Size = new System.Drawing.Size(257, 12);
            this.ID_GUID1.TabIndex = 4;
            this.ID_GUID1.Text = "Guid1:                                             Guid2:";
            // 
            // tog_Hard
            // 
            this.tog_Hard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tog_Hard.Appearance = System.Windows.Forms.Appearance.Button;
            this.tog_Hard.Enabled = false;
            this.tog_Hard.Location = new System.Drawing.Point(643, 70);
            this.tog_Hard.Name = "tog_Hard";
            this.tog_Hard.Size = new System.Drawing.Size(47, 22);
            this.tog_Hard.TabIndex = 6;
            this.tog_Hard.Text = "Hard";
            this.tog_Hard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tog_Hard.UseVisualStyleBackColor = true;
            this.tog_Hard.CheckedChanged += new System.EventHandler(this.tog_Hard_CheckedChanged);
            // 
            // tog_Soft
            // 
            this.tog_Soft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tog_Soft.Appearance = System.Windows.Forms.Appearance.Button;
            this.tog_Soft.Enabled = false;
            this.tog_Soft.Location = new System.Drawing.Point(690, 70);
            this.tog_Soft.Name = "tog_Soft";
            this.tog_Soft.Size = new System.Drawing.Size(47, 22);
            this.tog_Soft.TabIndex = 7;
            this.tog_Soft.Text = "Soft";
            this.tog_Soft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tog_Soft.UseVisualStyleBackColor = true;
            this.tog_Soft.CheckedChanged += new System.EventHandler(this.tog_Soft_CheckedChanged);
            // 
            // btn_Download
            // 
            this.btn_Download.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Download.Location = new System.Drawing.Point(643, 12);
            this.btn_Download.Name = "btn_Download";
            this.btn_Download.Size = new System.Drawing.Size(94, 23);
            this.btn_Download.TabIndex = 8;
            this.btn_Download.Text = "Download";
            this.btn_Download.UseVisualStyleBackColor = true;
            this.btn_Download.Click += new System.EventHandler(this.click_btn_Download);
            // 
            // btn_SelectHeader
            // 
            this.btn_SelectHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SelectHeader.Location = new System.Drawing.Point(643, 98);
            this.btn_SelectHeader.Name = "btn_SelectHeader";
            this.btn_SelectHeader.Size = new System.Drawing.Size(94, 23);
            this.btn_SelectHeader.TabIndex = 9;
            this.btn_SelectHeader.Text = "Select header";
            this.btn_SelectHeader.UseVisualStyleBackColor = true;
            this.btn_SelectHeader.Click += new System.EventHandler(this.click_btn_SelectHeader);
            // 
            // btn_Item1
            // 
            this.btn_Item1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Item1.Location = new System.Drawing.Point(12, 507);
            this.btn_Item1.Name = "btn_Item1";
            this.btn_Item1.Size = new System.Drawing.Size(75, 23);
            this.btn_Item1.TabIndex = 10;
            this.btn_Item1.Text = "Item1";
            this.btn_Item1.UseVisualStyleBackColor = true;
            this.btn_Item1.Click += new System.EventHandler(this.click_btn_Item1);
            // 
            // btn_item2
            // 
            this.btn_item2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_item2.Location = new System.Drawing.Point(93, 507);
            this.btn_item2.Name = "btn_item2";
            this.btn_item2.Size = new System.Drawing.Size(75, 23);
            this.btn_item2.TabIndex = 11;
            this.btn_item2.Text = "Item2";
            this.btn_item2.UseVisualStyleBackColor = true;
            this.btn_item2.Click += new System.EventHandler(this.click_btn_Item2);
            // 
            // majorHard
            // 
            this.majorHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.majorHard.AutoSize = true;
            this.majorHard.Location = new System.Drawing.Point(641, 389);
            this.majorHard.Name = "majorHard";
            this.majorHard.Size = new System.Drawing.Size(89, 12);
            this.majorHard.TabIndex = 13;
            this.majorHard.Text = "MAJOR_HARD:";
            this.majorHard.Visible = false;
            // 
            // majorSoft
            // 
            this.majorSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.majorSoft.AutoSize = true;
            this.majorSoft.Location = new System.Drawing.Point(641, 409);
            this.majorSoft.Name = "majorSoft";
            this.majorSoft.Size = new System.Drawing.Size(89, 12);
            this.majorSoft.TabIndex = 14;
            this.majorSoft.Text = "MAJOR_SOFT:\r\n";
            this.majorSoft.Visible = false;
            // 
            // mediumHard
            // 
            this.mediumHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mediumHard.AutoSize = true;
            this.mediumHard.Location = new System.Drawing.Point(641, 429);
            this.mediumHard.Name = "mediumHard";
            this.mediumHard.Size = new System.Drawing.Size(96, 12);
            this.mediumHard.TabIndex = 15;
            this.mediumHard.Text = "MEDIUM_HARD:";
            this.mediumHard.Visible = false;
            // 
            // mediumSoft
            // 
            this.mediumSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mediumSoft.AutoSize = true;
            this.mediumSoft.Location = new System.Drawing.Point(641, 449);
            this.mediumSoft.Name = "mediumSoft";
            this.mediumSoft.Size = new System.Drawing.Size(96, 12);
            this.mediumSoft.TabIndex = 16;
            this.mediumSoft.Text = "MEDIUM_SOFT:";
            this.mediumSoft.Visible = false;
            // 
            // minorHard
            // 
            this.minorHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.minorHard.AutoSize = true;
            this.minorHard.Location = new System.Drawing.Point(641, 469);
            this.minorHard.Name = "minorHard";
            this.minorHard.Size = new System.Drawing.Size(87, 12);
            this.minorHard.TabIndex = 17;
            this.minorHard.Text = "MINOR_HARD:";
            this.minorHard.Visible = false;
            // 
            // minorSoft
            // 
            this.minorSoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.minorSoft.AutoSize = true;
            this.minorSoft.Location = new System.Drawing.Point(641, 489);
            this.minorSoft.Name = "minorSoft";
            this.minorSoft.Size = new System.Drawing.Size(87, 12);
            this.minorSoft.TabIndex = 18;
            this.minorSoft.Text = "MINOR_SOFT:";
            this.minorSoft.Visible = false;
            // 
            // rdo_hide
            // 
            this.rdo_hide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdo_hide.AutoSize = true;
            this.rdo_hide.Location = new System.Drawing.Point(643, 150);
            this.rdo_hide.Name = "rdo_hide";
            this.rdo_hide.Size = new System.Drawing.Size(103, 16);
            this.rdo_hide.TabIndex = 19;
            this.rdo_hide.Text = "선택 외 숨기기";
            this.rdo_hide.UseVisualStyleBackColor = true;
            this.rdo_hide.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
            // 
            // rdo_trans
            // 
            this.rdo_trans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdo_trans.AutoSize = true;
            this.rdo_trans.Location = new System.Drawing.Point(643, 173);
            this.rdo_trans.Name = "rdo_trans";
            this.rdo_trans.Size = new System.Drawing.Size(103, 16);
            this.rdo_trans.TabIndex = 20;
            this.rdo_trans.Text = "선택 외 투명화";
            this.rdo_trans.UseVisualStyleBackColor = true;
            this.rdo_trans.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
            // 
            // rdo_none
            // 
            this.rdo_none.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdo_none.AutoSize = true;
            this.rdo_none.Checked = true;
            this.rdo_none.Location = new System.Drawing.Point(643, 128);
            this.rdo_none.Name = "rdo_none";
            this.rdo_none.Size = new System.Drawing.Size(87, 16);
            this.rdo_none.TabIndex = 21;
            this.rdo_none.TabStop = true;
            this.rdo_none.Text = "숨기기 없음";
            this.rdo_none.UseVisualStyleBackColor = true;
            // 
            // btn_Screenshot
            // 
            this.btn_Screenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Screenshot.Location = new System.Drawing.Point(643, 513);
            this.btn_Screenshot.Name = "btn_Screenshot";
            this.btn_Screenshot.Size = new System.Drawing.Size(94, 23);
            this.btn_Screenshot.TabIndex = 22;
            this.btn_Screenshot.Text = "Screenshot";
            this.btn_Screenshot.UseVisualStyleBackColor = true;
            this.btn_Screenshot.Click += new System.EventHandler(this.btn_Screenshot_Click);
            // 
            // folv
            // 
            this.folv.AllColumns.Add(this.Element1Guid);
            this.folv.AllColumns.Add(this.Element2Guid);
            this.folv.AllColumns.Add(this.Type);
            this.folv.AllColumns.Add(this.MovabilityValue);
            this.folv.AllColumns.Add(this.Topology);
            this.folv.AllColumns.Add(this.HardClashType);
            this.folv.AllColumns.Add(this.SoftClashType);
            this.folv.AllColumns.Add(this.Severity);
            this.folv.AllColumns.Add(this.Clearance);
            this.folv.AllColumns.Add(this.MovabilityResult);
            this.folv.AllColumns.Add(this.Offset);
            this.folv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folv.CellEditUseWholeCell = false;
            this.folv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Element1Guid,
            this.Element2Guid,
            this.Type,
            this.MovabilityValue,
            this.Topology,
            this.HardClashType,
            this.SoftClashType,
            this.Severity,
            this.Clearance,
            this.MovabilityResult,
            this.Offset
            });
            this.folv.Cursor = System.Windows.Forms.Cursors.Default;
            this.folv.FullRowSelect = true;
            this.folv.HideSelection = false;
            this.folv.Location = new System.Drawing.Point(12, 12);
            this.folv.MultiSelect = false;
            this.folv.Name = "folv";
            this.folv.ShowGroups = false;
            this.folv.ShowItemCountOnGroups = true;
            this.folv.Size = new System.Drawing.Size(612, 489);
            this.folv.TabIndex = 23;
            this.folv.UseCompatibleStateImageBehavior = false;
            this.folv.View = System.Windows.Forms.View.Details;
            this.folv.VirtualMode = true;
            this.folv.SelectionChanged += new System.EventHandler(this.folv_SelectionChanged);
            // 
            // ClashTypeCol
            // 
            this.Element1Guid.AspectName = "Element1 Guid";
            this.Element1Guid.Text = "Element1 Guid";
            this.Element1Guid.Width = 81;
            // 
            // HardClashTypeCol
            // 
            this.Element2Guid.AspectName = "Element2 Guid";
            this.Element2Guid.Text = "Element2 Guid";
            this.Element2Guid.Width = 100;
            // 
            // SoftClashTypeCol
            // 
            this.Type.AspectName = "Type";
            this.Type.Text = "hard clash type";
            this.Type.Width = 93;
            // 
            // SeverityCol
            // 
            this.MovabilityValue.AspectName = "MovabilityValue";
            this.MovabilityValue.Text = "Movability value";
            // 
            // Elem1disciplineCol
            // 
            this.Topology.AspectName = "topology";
            this.Topology.Text = "topology(x+z+y)";
            this.Topology.Width = 127;
            // 
            // Elem1GUIDCol
            // 
            this.HardClashType.AspectName = "hard clash type";
            this.HardClashType.Text = "hard clash type";
            this.HardClashType.Width = 101;
            // 
            // Elem1TypeCol
            // 
            this.Severity.AspectName = "Severity";
            this.Severity.Text = "Severity";
            this.HardClashType.Width = 103;
            // 
            // Elem2disciplineCol
            // 
            this.Clearance.AspectName = "Clearance";
            this.Clearance.Text = "Clearance";
            this.Clearance.Width = 123;
            // 
            // Elem2GUIDCol
            // 
            this.MovabilityResult.AspectName = "MovabilityResult";
            this.MovabilityResult.Text = "Movability Result";
            this.MovabilityResult.Width = 98;
            // 
            // Elem2TypeCol
            // 
            this.Offset.AspectName = "Offset";
            this.Offset.Text = "Offset";
            this.Offset.Width = 98;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(643, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // form_ResultViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 553);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.folv);
            this.Controls.Add(this.btn_Screenshot);
            this.Controls.Add(this.rdo_none);
            this.Controls.Add(this.rdo_trans);
            this.Controls.Add(this.rdo_hide);
            this.Controls.Add(this.minorSoft);
            this.Controls.Add(this.minorHard);
            this.Controls.Add(this.mediumSoft);
            this.Controls.Add(this.mediumHard);
            this.Controls.Add(this.majorSoft);
            this.Controls.Add(this.majorHard);
            this.Controls.Add(this.btn_item2);
            this.Controls.Add(this.btn_Item1);
            this.Controls.Add(this.btn_SelectHeader);
            this.Controls.Add(this.btn_Download);
            this.Controls.Add(this.tog_Soft);
            this.Controls.Add(this.tog_Hard);
            this.Controls.Add(this.ID_GUID1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Load);
            this.Name = "form_ResultViewer";
            this.Text = "Integrity Checker-MEP Response";
            ((System.ComponentModel.ISupportInitialize)(this.folv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_Load;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ID_GUID1;
        private System.Windows.Forms.CheckBox tog_Hard;
        private System.Windows.Forms.CheckBox tog_Soft;
        private System.Windows.Forms.Button btn_Download;
        private System.Windows.Forms.Button btn_SelectHeader;
        private System.Windows.Forms.Button btn_Item1;
        private System.Windows.Forms.Button btn_item2;
        private System.Windows.Forms.Label majorHard;
        private System.Windows.Forms.Label majorSoft;
        private System.Windows.Forms.Label mediumHard;
        private System.Windows.Forms.Label mediumSoft;
        private System.Windows.Forms.Label minorHard;
        private System.Windows.Forms.Label minorSoft;
        private System.Windows.Forms.RadioButton rdo_hide;
        private System.Windows.Forms.RadioButton rdo_trans;
        private System.Windows.Forms.RadioButton rdo_none;
        private System.Windows.Forms.Button btn_Screenshot;
        private BrightIdeasSoftware.FastObjectListView folv;

        private BrightIdeasSoftware.OLVColumn Element1Guid;
        private BrightIdeasSoftware.OLVColumn Element2Guid;
        private BrightIdeasSoftware.OLVColumn Type;
        private BrightIdeasSoftware.OLVColumn MovabilityValue;
        private BrightIdeasSoftware.OLVColumn Topology;
        private BrightIdeasSoftware.OLVColumn HardClashType;
        private BrightIdeasSoftware.OLVColumn SoftClashType;
        private BrightIdeasSoftware.OLVColumn Severity;
        private BrightIdeasSoftware.OLVColumn Clearance;
        private BrightIdeasSoftware.OLVColumn MovabilityResult;
        private BrightIdeasSoftware.OLVColumn Offset;
        private System.Windows.Forms.Button button1;
    }
}

