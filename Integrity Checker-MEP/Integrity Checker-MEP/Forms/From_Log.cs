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

        // 로그 폼 초기화
        public From_Log() {
            InitializeComponent();
        }

        List<DateTime> times = new List<DateTime>();
        // 새로운 로그 추가
        public void UpdateLog(string lasttask) {
            lst_progress.Update();
            DateTime fin = DateTime.Now;
            TimeSpan ts;
            // 소요 시간 계산
            if (times.Count > 0) {
                ts = fin - times[times.Count - 1];
            }
            else {
                ts = fin - fin;
            }
            times.Add(fin);
            // [실행된 시간 / 소요된 시간] 로그
            lst_progress.Items.Add($"[{fin.ToString("H:mm:ss")} / {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}] {lasttask}");
            // 커서를 마지막으로 옮겨 마지막 로그가 보일 수 있도록 함
            lst_progress.SelectedItem = lst_progress.Items[lst_progress.Items.Count - 1];
        }

        /// <summary>
        /// 로그를 objectinfo/log.txt로 저장함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
