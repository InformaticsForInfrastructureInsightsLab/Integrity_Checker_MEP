using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    public class ClashData
    {
        public Guid Element1Guid;
        public Guid Element2Guid;
        public string Type;
        public string MovabilityValue;
        public string Topology;
        public string HardClashType;
        public string Severity;
        public string Clearance;
        public string MovabilityResult;
        public string Offset;

        public ClashData(string[] stringdata) {
            Element1Guid = Guid.Parse(stringdata[0]);
            Element2Guid = Guid.Parse(stringdata[1]);
            Type = stringdata[2];
            MovabilityValue = stringdata[3];
            Topology = stringdata[4];
            HardClashType = stringdata[5];
            Severity = stringdata[6];
            Clearance = stringdata[7];
            MovabilityResult = stringdata[8];
            Offset = stringdata[9];
        }
    }
}
