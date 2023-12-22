using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Plugins;

namespace Integrity_Checker_MEP
{
    [Plugin("Integrity Checker-MEP", "LC", DisplayName = "Integrity Checker-MEP")]
    [RibbonLayout("Integrity_Checker_MEP.xaml")]
    [RibbonTab("IntegrityCheckerTab")]
    [Command("Clash_Checker", Icon = "1_16.png", LargeIcon = "1_32.png")]
    [Command("Result_Receiver", Icon = "2_16.png", LargeIcon = "2_32.png")]
    [Command("Split_Button")]
    [Command("Clash_Checker_split", Icon = "3_16.png", LargeIcon = "3_32.png")]
    [Command("Result_Receiver_split", Icon = "4_16.png", LargeIcon = "4_32.png")]
    public class MainClass : CommandHandlerPlugin
    {
        private ClashChecker clashChecker;
        private ResultViewer resultViewer;
    
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
                    if (resultViewer == null) {
                        resultViewer = new ResultViewer();
                        resultViewer.Execute();

                        //form2 = new Form2();
                        //form2.Show();
                        //form2.FormClosed += (s, args) => form2 = null;
                    }
                    else {
                        //todo 닫기 구현
                        //form2.Close();
                        //form2 = null;

                        resultViewer = null;
                    }
                    break;
                case "Clash_Checker_split":
                    clashChecker = new ClashChecker();
                    clashChecker.Execute();

                    //MessageBox.Show("In three words I can sum up everything I've learned" +
                    //    " about life: it goes on.\n-Robert Frost",
                    //    "Sample - Button Three");
                    break;
                case "Result_Receiver_split":
                    resultViewer = new ResultViewer();
                    resultViewer.Execute();

                    //MessageBox.Show("Four things for success: work and pray, " +
                    //    "think and believe.\n-Norman Vincent Peale", "Sample - Button Four");
                    break;
            }

            return 0;
        }
    }
}
