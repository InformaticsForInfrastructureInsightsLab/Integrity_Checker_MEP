using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrity_Checker_MEP
{
    public partial class form_ImageOption : Form
    {
        public form_ImageOption()
        {
            InitializeComponent();
            // 값을 추가
            for (int i = 1500; i <= 10000; i += 500)
            {
                this.cb_magnification.Items.Add(i.ToString());
            }
            this.cb_magnification.SelectedIndex = 0; // 첫 번째 항목을 기본 선택으로 설정
        }
        public bool background;
        public bool transparant;
        public bool start = false;
        public int transparancy = 80;
        public double magnification = 1500.0;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(cb_Background.Checked)
            {
                background = true;
            }
            else
            {
                background = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_Transparancy.Checked)
            {
                transparant = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                transparant = false;
                numericUpDown1.Enabled = false;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            transparancy = (int)numericUpDown1.Value;
        }

        private void cb_magnification_SelectIndexChanged(object sender, EventArgs e)
        {
            if(cb_magnification.SelectedIndex != -1)
            {
                string selectedItem = cb_magnification.Items[cb_magnification.SelectedIndex].ToString();
                magnification = double.Parse(selectedItem);
            }
        }
    }
}
