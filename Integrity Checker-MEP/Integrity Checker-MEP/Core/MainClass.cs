using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Plugins;
using ClashTest2;

namespace Integrity_Checker_MEP
{
    [Plugin("Integrity Checker-MEP", "LC", DisplayName = "Integrity Checker-MEP")]
    [RibbonLayout("Integrity_Checker_MEP.xaml")]
    [RibbonTab("IntegrityCheckerTab")]
    [Command("Clash_Checker", Icon = "1_16.png", LargeIcon = "1_32.png")]
    [Command("Result_Receiver", Icon = "2_16.png", LargeIcon = "2_32.png")]
    public class MainClass : CommandHandlerPlugin
    {
        public static ClashChecker clashChecker;
        public static form_ResultViewer rv;

        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "Clash_Checker":
                    //if(mainFunc == null)
                    //{
                        clashChecker = new ClashChecker();
                        clashChecker.Execute();

                        //form1 = new Form1();
                        //form1.Show();
                        //form1.FormClosed += (s, args) => form1 = null;
                    //}
                    //else
                    //{
                    //    //todo 닫기 구현
                    //    //form1.Close();
                    //    //form1 = null;
                    //    mainFunc = null;
                    //}
                    break;
                case "Result_Receiver":
                    if (rv == null) {
                        rv = new form_ResultViewer();
                        rv.Show();

                        //form2 = new Form2();
                        //form2.Show();
                        //form2.FormClosed += (s, args) => form2 = null;
                    }
                    else {
                        //todo 닫기 구현
                        //form2.Close();
                        //form2 = null;

                        rv = null;
                    }
                    break;
            }

            return 0;
        }
    }
}
