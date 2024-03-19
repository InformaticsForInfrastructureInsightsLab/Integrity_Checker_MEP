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
        }
        public bool background;
        public bool transparant;
        public bool start = false;
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
            }
            else
            {
                transparant = false;
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
    }
}
