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
    [Command("Button_One", Icon = "1_16.png", LargeIcon = "1_32.png")]
    [Command("Button_Two", Icon = "2_16.png", LargeIcon = "2_32.png")]
    [Command("Split_Button_1")]
    [Command("Button_Three", Icon = "3_16.png", LargeIcon = "3_32.png")]
    [Command("Button_Four", Icon = "4_16.png", LargeIcon = "4_32.png")]
    public class MainClass : CommandHandlerPlugin
    {
        private Form1 form1;
        private Form2 form2;

        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "Button_One":
                    if(form1 == null)
                    {
                        form1 = new Form1();
                        form1.Show();
                        form1.FormClosed += (s, args) => form1 = null;
                    }
                    else
                    {
                        form1.Close();
                        form1 = null;
                    }
                    break;
                case "Button_Two":
                    if (form2 == null)
                    {
                        form2 = new Form2();
                        form2.Show();
                        form2.FormClosed += (s, args) => form2 = null;

                    }
                    else
                    {
                        form2.Close();
                        form2 = null;
                    }
                    break;
                /*case "Button_Three":
                    MessageBox.Show("In three words I can sum up everything I've learned" +
                        " about life: it goes on.\n-Robert Frost",
                        "Sample - Button Three");
                    break;
                case "Button_Four":
                    MessageBox.Show("Four things for success: work and pray, " +
                        "think and believe.\n-Norman Vincent Peale", "Sample - Button Four");
                    break;*/
            }

            return 0;
        }
    }
}
