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

        [DllImport("Dll1.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern void ForwardGraphKey(string result);

        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void ShowMyWindow();
        #endregion

        #region cookie
        private static readonly HttpClientHandler handler = new HttpClientHandler
        {
            CookieContainer = new System.Net.CookieContainer(),
            UseCookies = true
        };
        private static readonly HttpClient client = new HttpClient(handler);
        #endregion

        CallbackDelegate callback;
        GUIDDelegate guidExport;

        public Chat()
        {
            // 콜백 등록
            callback = new CallbackDelegate(ReceiveMessageFromCpp);
            RegisterCallback(callback);

            guidExport = new GUIDDelegate(FindElement);
            RegisterGUIDExportFunc(guidExport);

            GetGraphKeys();
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
            {
                string url = "http://117.17.196.59:1131/question";

                var data = new { user_question = message };
                string jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        Response answer = JsonConvert.DeserializeObject<Response>(responseBody);
                        File.WriteAllText("C://objectinfo/context.json", answer.context);
                        MessageBox.Show("success", "notify", MessageBoxButtons.OK);
                        ForwardAnswer(answer.result.Replace("\n", "\r\n"), String.Copy(answer.context));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    try
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        BadResponse answer = JsonConvert.DeserializeObject<BadResponse>(responseBody);
                        ForwardAnswer(answer.error, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private async void GetGraphKeys()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "http://117.17.196.59:3116/graph_keys";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        ForwardGraphKey(String.Copy(responseBody));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(responseBody, "error", MessageBoxButtons.OK);
                }
            }
        }

        private void FindElement([MarshalAs(UnmanagedType.LPWStr)] string guid1, [MarshalAs(UnmanagedType.LPWStr)] string guid2)
        {
            MessageBox.Show( (guid1==guid2 ? guid1 : guid1+"/"+guid2),
                "info", MessageBoxButtons.OK);
            form_ResultViewer rv = MainClass.rv;
            rv.trans = true;
            rv.SelectClash(guid1, guid2);
        }
    }

    class Response
    {
        public string result;
        public string context;
    }

    class BadResponse
    {
        public string error;
    }
}
