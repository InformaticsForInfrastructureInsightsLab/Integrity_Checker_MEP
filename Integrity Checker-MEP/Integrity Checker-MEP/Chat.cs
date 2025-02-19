using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Autodesk.Navisworks.Api;
using System.Windows.Controls;
using ClashTest2;

namespace Integrity_Checker_MEP
{
    internal class Chat
    {
        #region delegate
        // C++ 콜백 함수 델리게이트 정의
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void CallbackDelegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void GUIDDelegate([MarshalAs(UnmanagedType.LPWStr)] string guid1, [MarshalAs(UnmanagedType.LPWStr)] string guid2);
        #endregion

        #region dllimport
        // DLL 함수 선언
        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void RegisterCallback(CallbackDelegate callback);
        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void RegisterGUIDExportFunc(GUIDDelegate guidExport);

        [DllImport("Dll1.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ForwardQuestion();

        [DllImport("Dll1.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ForwardPrevContext();

        [DllImport("Dll1.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void ForwardAnswer([MarshalAs(UnmanagedType.LPWStr)] string result, [MarshalAs(UnmanagedType.LPWStr)] string schema);

        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void ShowMyWindow();
        #endregion

        CallbackDelegate callback;
        GUIDDelegate guidExport;

        public Chat()
        {   // 콜백 등록
            callback = new CallbackDelegate(ReceiveMessageFromCpp);
            RegisterCallback(callback);

            guidExport = new GUIDDelegate(FindElement);
            RegisterGUIDExportFunc(guidExport);
        }

        private void ReceiveMessageFromCpp()
        {
            IntPtr LPmessage = ForwardQuestion();
            IntPtr LPprevContext = ForwardPrevContext();
            string message = Marshal.PtrToStringUni(LPmessage);
            string prevContext = Marshal.PtrToStringUni(LPprevContext);
            Question(message, prevContext);
        }

        private async void Question(string message, string prevContext)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "http://117.17.196.59:1131/question";

                var data = new { user_question = message, prev_context = prevContext == "" ? "None" : prevContext };
                string jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("success", "notify", MessageBoxButtons.OK);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Response answer = JsonConvert.DeserializeObject<Response>(responseBody);

                    string context = answer.context;
                    context = context.Replace("\n", ",");
                    context = "["+context+"]";

                    ForwardAnswer(answer.result.Replace("\n", "\r\n"), context);
                }
                else
                {
                    string context = File.ReadAllText("C://objectinfo/context.json");
                    ForwardAnswer($"Error: {response.StatusCode}", null);
                }
            }
        }

        private void FindElement([MarshalAs(UnmanagedType.LPWStr)] string guid1, [MarshalAs(UnmanagedType.LPWStr)] string guid2)
        {
            MessageBox.Show(guid1+"/"+guid2, "guid", MessageBoxButtons.OK);
            form_ResultViewer rv = new form_ResultViewer();
            rv.trans = true;
            rv.SelectClash(guid1, guid2);
        }
    }

    class Response
    {
        public string result;
        public string context;
    }
}
