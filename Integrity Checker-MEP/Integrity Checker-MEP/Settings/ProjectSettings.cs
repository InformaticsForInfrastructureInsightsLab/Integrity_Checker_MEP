using System.Collections.Generic;

namespace Integrity_Checker_MEP
{
    public static class ProjectSettings
    {
        public const string ModelFolderPath = @"C:\models\";
        public const string OutputFolderPath = @"C:/objectinfo";
        public const string ResultImageFolderPath = OutputFolderPath + "/ResultImage";
        public const string SideImageFolderName = "다각도이미지";
        public const string TestsDictFileName = "tests_dict.json";
        public const string LogFileName = "log.txt";
        public const string AllInOneReportName = "All_in_One.csv";
        public const string SameIfcSystemReportName = "SameIfcSystem.csv";
        public const string UselessClashesReportName = "Useless Clashes.csv";
        public const string PropertiesReportName = "Properties.csv";
        public const string CompressedFolderName = "compressed";
        public const string CompressedZipFileName = "compressed.zip";
        public const string UsedModelInfoFileName = "used_model.txt";

        public const string ServerUploadUrl = "http://117.17.196.59:3116/upload";

        public static readonly string[] ModelNames = { "Arch", "COMM", "ELEC", "FIRE", "MECH", "Str" };
    }
}