using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace Integrity_Checker_MEP
{
    public partial class Form_Chat : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public Form_Chat()
        {
            InitializeComponent();
        }

        private async void on_click_send_button(object sender, EventArgs e)
        {
            string user_request = input_text.Text;
            try
            {
                // 서버로 데이터 전송
                string response = await send_server(user_request);

                // 서버 응답 출력
                model_answer.Text = response;
            }
            catch (Exception ex)
            {
                // 에러 처리
                MessageBox.Show("에러 발생: " + ex.Message);
            }
        }

        private async Task<string> send_server(string data)
        {
            // JSON 데이터 생성
            var jsonData = new StringContent(
                $"{{ \"data\": \"{data}\" }}",
                Encoding.UTF8,
                "application/json"
            );

            // HTTP POST 요청
            HttpResponseMessage response = await client.PostAsync("https://example.com/api", jsonData);

            // 요청 결과 확인
            response.EnsureSuccessStatusCode();

            // 응답 데이터 읽기
            return await response.Content.ReadAsStringAsync();
        }
    }
}
