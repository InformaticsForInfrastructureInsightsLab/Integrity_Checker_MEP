using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    internal class DashboardUtils
    {
        public static int MapModel(string model)
        {
            switch (model)
            {
                case "Arch":
                    return 0;
                case "COMM":
                    return 1;
                case "ELEC":
                    return 2;
                case "FIRE":
                    return 3;
                case "MECH":
                    return 4;
                case "Str":
                    return 5;
            }
            return -1;
        }

        public static int MapSeverity(string severity)
        {
            switch (severity)
            {
                case "Minor":
                    return 0;
                case "Medium":
                    return 1;
                case "Major":
                    return 2;
            }
            return -1;
        }
    }
}
