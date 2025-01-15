using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    internal class Chat
    {
        #region delegate
        // C++ 콜백 함수 델리게이트 정의
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void CallbackDelegate(string message);
        #endregion

        #region dllimport
        // DLL 함수 선언
        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void RegisterCallback(CallbackDelegate callback);

        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void TriggerEvent();

        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void ForwardAnswer(string answer);

        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void ShowMyWindow();
        #endregion

        public Chat()
        {   // 콜백 등록
            CallbackDelegate callback = new CallbackDelegate(ReceiveMessageFromCpp);
            RegisterCallback(callback);
        }

        private void ReceiveMessageFromCpp(string message)
        {
            Question(message);
        }

        private async void Question(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "http://117.17.196.59:1131/question";
                var data = new StringContent("{\"user_request\":" + message + "}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, data);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ForwardAnswer(responseBody);
                }
                else
                {
                    ForwardAnswer($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
