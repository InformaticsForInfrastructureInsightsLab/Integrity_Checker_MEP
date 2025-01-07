using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrity_Checker_MEP
{
    public partial class IFCLoad : Form
    {
        public bool load;
        public const string path = @"C:\models\";

        public List<string> usedmodel = new List<string>();

        public IFCLoad()
        {
            InitializeComponent();
            LoadFiles();
            load = false;
        }

        public void LoadFiles()
        {
            if (Directory.Exists(path))
            {

                string[] files = Directory.GetFiles(path);
                cb_panel.Controls.Clear();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(".ifc") && (!files[i].Contains(".ifc.")))
                    {
                        CheckBox cb = new CheckBox();
                        cb.Text = Path.GetFileName(files[i]);
                        cb_panel.Controls.Add(cb);
                    }
                }
            }
            else
            {
                MessageBox.Show("path does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            foreach (Control control in cb_panel.Controls)
            {
                if (control is CheckBox checkBox && checkBox.Checked)
                {
                    doc.AppendFile(Path.Combine(path, checkBox.Text));
                    usedmodel.Add(checkBox.Text);
                }
            }

            if (doc.Models.Count == 0)
            {
                MessageBox.Show("no file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            load = true;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
