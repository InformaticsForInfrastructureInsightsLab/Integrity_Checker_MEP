using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Integrity_Checker_MEP
{
    public class ReportGenerator
    {
        private readonly ILogger _logger;

        public ReportGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public void ExportSameIfcSystem(List<IfcSystemData> sameIfcSystemList, Func<ModelItem, string> getElementIdFunc, double offset)
        {
            _logger.Log("파일 출력 시작 : SameIfcSystem");
            StringBuilder sb = new StringBuilder();
            if (sameIfcSystemList.Count == 0)
            {
                sb.Append("no same ifcsystem");
            }
            else
            {
                sb.AppendLine("NO,Item1,Item2,Distance,System1,System2");
                foreach (var item in sameIfcSystemList)
                {
                    sb.AppendLine($"{item.result.DisplayName},{getElementIdFunc(item.result.Item1)},{getElementIdFunc(item.result.Item2)},{(item.result.Distance * offset):F3},{item.ifc1},{item.ifc2}");
                }
            }
            
            string filePath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.SameIfcSystemReportName);
            WriteToFile(filePath, sb.ToString());
            _logger.Log("파일 출력 완료 : SameIfcSystem");
        }

        public void ExportUselessClashes(List<ClashResult> uselessClashes, List<string> uselessReasons, List<ClashResultCls> uselessClashes2, List<string> uselessReasons2, Func<ModelItem, string> getElementIdFunc, Func<ModelItem, string, string, string> getInfoFunc, double offset)
        {
            _logger.Log("파일 출력 시작 : UselessClashes");
            if (uselessClashes.Count == 0 && uselessClashes2.Count == 0) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Useless Clashes [{uselessClashes.Count + uselessClashes2.Count}]");
            sb.AppendLine("간섭번호,NameSpace1,GUID1,Class1,NameSpace2,GUID2,Class2,거리,제외사유");

            for (int k = 0; k < uselessClashes.Count; k++)
            {
                var i = uselessClashes[k];
                var j = uselessReasons[k];
                sb.Append($"{i.DisplayName},");
                sb.Append($"{getInfoFunc(i.Item1, "항목", "소스 파일").Replace(".ifc", "").ToUpper()},");
                sb.Append($"{getElementIdFunc(i.Item1)},");
                sb.Append($"{getInfoFunc(i.Item1, "요소", "IfcClass")},");
                sb.Append($"{getInfoFunc(i.Item2, "항목", "소스 파일").Replace(".ifc", "").ToUpper()},");
                sb.Append($"{getElementIdFunc(i.Item2)},");
                sb.Append($"{getInfoFunc(i.Item2, "요소", "IfcClass")},");
                sb.Append($"{(i.Distance * offset):F3},");
                sb.AppendLine(j);
            }

            for (int k = 0; k < uselessClashes2.Count; k++)
            {
                var i = uselessClashes2[k];
                var j = uselessReasons2[k];
                sb.Append($"{i.DisplayName},");
                sb.Append($"{i.Namespace1},");
                sb.Append($"{i.path1ID},");
                sb.Append($"{getInfoFunc(i.Item1, "요소", "IfcClass")},");
                sb.Append($"{i.Namespace2},");
                sb.Append($"{i.path2ID},");
                sb.Append($"{getInfoFunc(i.Item2, "요소", "IfcClass")},");
                sb.Append($"{i.distance:F3},");
                sb.AppendLine(j);
            }

            string filePath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.UselessClashesReportName);
            WriteToFile(filePath, sb.ToString());
            _logger.Log("파일 출력 완료 : UselessClashes");
        }

        public StringBuilder SaveCsvFile(string name, List<string[]> data)
        {
            string[][] output = data.Select(l => l.ToArray()).ToArray();
            int length = output.GetLength(0);
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.AppendLine(string.Join(delimiter, output[i]));
            }
            
            string filePath = Path.Combine(ProjectSettings.OutputFolderPath, name + ".csv");
            WriteToFile(filePath, sb.ToString());
            return sb;
        }

        private void WriteToFile(string filePath, string content)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.Log($"파일 쓰기 오류 ({filePath}): {ex.Message}");
                throw;
            }
        }
    }
}
