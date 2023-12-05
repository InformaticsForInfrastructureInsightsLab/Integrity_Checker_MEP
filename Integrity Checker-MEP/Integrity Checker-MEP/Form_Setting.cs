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
    public partial class Form_Setting : Form {
        public Form_Setting() {
            InitializeComponent();
        }

        public bool show_fromparent = false; //부재의 GUID가 부모의 GUID를 가져온 것인지 표시(접두사 fromparent_ 추가)
        public bool export_Result = true; //결과 파일 추출
        public bool export_AllinOne = true; //모든 결과를 한 파일로 통합하여 추출
        public bool export_UselessClash = true; //불필요한 간섭 결과를 나열한 파일 추출
        public bool export_SameIfcSystem = true; //동일한 IfcSystem을 가지는 MEP부재를 나열한 파일 추출
        public bool export_Properties = true; //Properties파일 추출
        public bool cut_Properties = false; //Properties파일 분할
        public bool Make_ZipFile = false; //압축파일 만들기
        public bool Send_toServer = false; //서버로 보내기
        public bool Save_log = false; //로그 저장

        public bool start = false; //시작버튼 클릭 여부

        //부모 GUID 획득 표시
        private void cb_parent_CheckedChanged(object sender, EventArgs e) {
            if (cb_parent.Checked) show_fromparent = true;
            else show_fromparent = false;
        }
        //간섭결과 출력
        private void cb_exportResult_CheckedChanged(object sender, EventArgs e) {
            if (cb_exportResult.Checked) export_Result = true;
            else export_Result = false;
        }
        //All In One 출력
        private void cb_allInOne_CheckedChanged(object sender, EventArgs e) {
            if (cb_allInOne.Checked) export_AllinOne = true;
            else export_AllinOne = false;
        }
        //불필요 간섭출력
        private void cb_useless_CheckedChanged(object sender, EventArgs e) {
            if (cb_useless.Checked) export_UselessClash = true;
            else export_UselessClash = false;
        }
        //동일 IfcSystem출력
        private void cb_sameSystem_CheckedChanged(object sender, EventArgs e) {
            if (cb_sameSystem.Checked) export_SameIfcSystem = true;
            else export_SameIfcSystem = false;
        }
        //Properties 출력
        private void cb_properties_CheckedChanged(object sender, EventArgs e) {
            if (cb_properties.Checked) export_Properties = true;
            else export_Properties = false;
        }
        //서버전송
        private void cb_server_CheckedChanged(object sender, EventArgs e) {
            if (cb_server.Checked) {
                Make_ZipFile = true;
                Send_toServer = true;
            }
            else {
                Make_ZipFile = false;
                Send_toServer = false;
            }
        }
        //간섭검토 시작
        private void btn_start_Click(object sender, EventArgs e) {
            start = true;
            this.Close();
        }
        //취소
        private void btn_cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cb_savelog_CheckedChanged(object sender, EventArgs e) {
            if (cb_savelog.Checked) Save_log = true;
            else Save_log = false;
        }
    }
}
