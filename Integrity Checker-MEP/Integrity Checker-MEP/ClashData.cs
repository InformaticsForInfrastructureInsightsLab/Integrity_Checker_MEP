using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    public class ClashData
    {
        public ClashData() { }
        public string ClashType { get; set; }
        public string HardClashType { get; set; }
        public string SoftClashType { get; set; }
        public string Severity { get; set; }
        public string Element1discipline { get; set; }
        public string Element1GUID { get; set; }
        public string Element1Type { get; set; }
        public string Element2discipline { get; set; }
        public string Element2GUID { get; set; }
        public string Element2Type { get; set; }
        public string ClashDistance { get; set; }
        public string Clearance { get; set; }
        public string ClashPoint { get; set; }
        public string ClashVolume { get; set; }
        public string Topology { get; set; }
        public string Offset { get; set; }
    }
}
