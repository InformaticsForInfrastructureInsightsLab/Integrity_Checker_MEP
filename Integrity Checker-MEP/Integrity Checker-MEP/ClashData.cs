using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    public class ClashData
    {
        public string Element1Guid;
        public string Element2Guid;
        public string Type;
        public string MovabilityValue;
        public string Topology;
        public string HardClashType;
        public string SoftClashType;
        public string Severity;
        public string Clearance;
        public string MovabilityResult;
        public string MovableSpace;
        public string MovableDistance;
        public string Offset;
        public string Penetration;
        public string ABS_Volume_Diff;
        public string ABS_Volume_SUM;
        public string ClashVolume;


        public ClashData(string[] stringdata) {
            Element1Guid = stringdata[0];
            Element2Guid = stringdata[1];
            Type = stringdata[2];
            MovabilityValue = stringdata[3];
            Topology = stringdata[4];
            HardClashType = stringdata[5];
            SoftClashType = stringdata[6];
            Severity = stringdata[7];
            Clearance = stringdata[8];
            MovabilityResult = stringdata[9];
            MovableSpace = stringdata[10];
            MovableDistance = stringdata[11];
            Offset = stringdata[12];
            Penetration = stringdata[13];
            ABS_Volume_Diff = stringdata[14];
            ABS_Volume_SUM = stringdata[15];
            ClashVolume = stringdata[16];
        }
    }
}
