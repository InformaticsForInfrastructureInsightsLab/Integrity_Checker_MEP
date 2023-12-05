using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrity_Checker_MEP {
    public partial class From_Log : Form {

        public From_Log() {
            InitializeComponent();
        }
        List<DateTime> times = new List<DateTime>();
        public void UpdateLog(string lasttask) {
            lst_progress.Update();
            DateTime fin = DateTime.Now;
            TimeSpan ts;
            if (times.Count > 0) {
                ts = fin - times[times.Count - 1];
            }
            else {
                ts = fin - fin;
            }
            times.Add(fin);
            lst_progress.Items.Add($"[{fin.ToString("H:mm:ss")} / {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}] {lasttask}");
            lst_progress.SelectedItem = lst_progress.Items[lst_progress.Items.Count - 1];
        }

        private void btn_savelog_Click(object sender, EventArgs e) {
            try {
                StringBuilder sb = new StringBuilder();
                string path = @"C:/objectinfo/log.txt";
                foreach (var item in lst_progress.Items) {
                    sb.AppendLine(item.ToString());
                }
                System.IO.File.WriteAllText(path, sb.ToString());
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

    }
}
