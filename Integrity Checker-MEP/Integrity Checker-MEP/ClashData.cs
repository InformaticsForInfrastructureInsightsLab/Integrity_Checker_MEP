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
        public string Adjusted_Severity;
        public string Clearance;
        public string MovabilityResult;
        public string MovableSpace;
        public string MovableDistance;
        public string Offset;
        public string Penetration;
        public string ABS_Volume_Diff;
        public string ABS_Volume_SUM;
        public string ClashVolume;
        public string MovabilityValue_X_P;
        public string MovabilityValue_X_N;
        public string MovabilityValue_Y_P;
        public string MovabilityValue_Y_N;
        public string MovabilityValue_Z_P;
        public string MovabilityValue_Z_N;
        public string Element1Type;
        public string Element2Type;

        public ClashData(string[] stringdata) {
            Element1Guid = stringdata[0];
            Element2Guid = stringdata[1];
            Type = stringdata[2];
            Topology = stringdata[3];
            HardClashType = stringdata[4];
            SoftClashType = stringdata[5];
            Adjusted_Severity = stringdata[6];
            Severity = stringdata[7];
            Clearance = stringdata[8];
            Offset = stringdata[9];
            Penetration = stringdata[10];
            ABS_Volume_Diff = stringdata[11];
            ABS_Volume_SUM = stringdata[12];
            ClashVolume = stringdata[13];
            MovabilityResult = stringdata[14];
            MovableSpace = stringdata[15];
            MovableDistance = stringdata[16];
            MovabilityValue_X_P = stringdata[17];
            MovabilityValue_X_N = stringdata[18];
            MovabilityValue_Y_P = stringdata[19];
            MovabilityValue_Y_N = stringdata[20];
            MovabilityValue_Z_P = stringdata[21];
            MovabilityValue_Z_N = stringdata[22];
            Element1Type = stringdata[23];
            Element2Type = stringdata[24];
        }
    }
}
