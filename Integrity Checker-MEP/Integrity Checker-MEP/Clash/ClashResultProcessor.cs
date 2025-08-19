using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Integrity_Checker_MEP
{
    public class ClashResultProcessor
    {
        private readonly ILogger _logger;
        private readonly Form_Setting _settings;
        private readonly Document _document;
        private readonly ReportGenerator _reportGenerator;
        public readonly double _offset;
        private readonly double _tolerOffset;

        private List<string> _lstArchDupl = new List<string>();
        private List<string> _lstStrDupl = new List<string>();
        private List<IfcSystemData> _listSameIfcSystem = new List<IfcSystemData>();
        private List<ClashResult> _listUselessClashes = new List<ClashResult>();
        private List<string> _listUselessReasons = new List<string>();
        private List<ClashResultCls> _listUselessClashes2 = new List<ClashResultCls>();
        private List<string> _listUselessReasons2 = new List<string>();

        public ClashResultProcessor(ILogger logger, Form_Setting settings, Document document, ReportGenerator reportGenerator)
        {
            _logger = logger;
            _settings = settings;
            _document = document;
            _reportGenerator = reportGenerator;

            if (_document.Models.Count > 0)
            {
                _offset = UnitConversion.ScaleFactor(_document.Models[0].Units, Units.Millimeters);
                _tolerOffset = UnitConversion.ScaleFactor(Units.Millimeters, _document.Models[0].Units);
            }
            else
            {
                _offset = 1.0;
                _tolerOffset = 1.0;
            }
        }

        public void ProcessAndReport(string clashtestName = null)
        {
            _logger.Log("테스트 결과 처리 및 보고 시작");
            try
            {
                DocumentClash documentClash = _document.GetClash();
                DocumentClashTests oDCT = documentClash.TestsData;
                List<ClashTestCls> tests_array = new List<ClashTestCls>();
                Dictionary<string, List<string>> tests_dict = new Dictionary<string, List<string>>();

                #region 각 테스트 별 충돌 오브젝트 정보 가져오기
                foreach (ClashTest test in oDCT.Tests)
                {
                    _logger.Log($"    결과 읽기 시작 : {test.DisplayName}");
                    ClashTestCls oEachTest = new ClashTestCls
                    {
                        DisplayName = test.DisplayName,
                        testType = test.TestType
                    };

                    if (test.TestType == ClashTestType.Duplicate)
                    {
                        ProcessDuplicateTest(test);
                        continue;
                    }

                    List<ClashResultCls> test_results_array = ProcessClashResults(test);
                    oEachTest.ClashResults = test_results_array.ToArray();
                    tests_array.Add(oEachTest);
                    _logger.Log($"    결과 읽기 종료 : {test.DisplayName}");
                }

                if (_settings.export_SameIfcSystem)
                {
                    _reportGenerator.ExportSameIfcSystem(_listSameIfcSystem, GetElementID, _offset);
                }
                #endregion

                #region 충돌 보고서 내보내기
                StringBuilder allInOneSb = new StringBuilder();
                string[] header = { "No,", "NameSpace1,", "Guid1,", "IfcClass1,", "NameSpace2,", "Guid2,", "IfcClass2,", "Type,", "Clashpoint,", "Distance,", "IfcSystem1,", "IfcSystem2" };
                allInOneSb.AppendLine(string.Join("", header));

                _logger.Log("결과 파일 생성 시작");
                foreach (var testArray in tests_array)
                {
                    _logger.Log($"    결과 쓰기 시작 : {testArray.DisplayName}");
                    var (csvData, clashNumList) = ProcessTestArrayForCsv(testArray);
                    tests_dict.Add(testArray.DisplayName, clashNumList);

                    if (_settings.export_AllinOne)
                    {
                        allInOneSb.Append(_reportGenerator.SaveCsvFile(testArray.DisplayName, csvData));
                    }
                    else
                    {
                        _reportGenerator.SaveCsvFile(testArray.DisplayName, csvData);
                    }
                    _logger.Log($"    결과 쓰기 종료 : {testArray.DisplayName}");
                }

                string json = JsonConvert.SerializeObject(tests_dict, Formatting.Indented);
                File.WriteAllText(Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.TestsDictFileName), json);
                _logger.Log("결과 파일 생성 완료");

                if (_settings.export_UselessClash)
                {
                    _reportGenerator.ExportUselessClashes(_listUselessClashes, _listUselessReasons, _listUselessClashes2, _listUselessReasons2, GetElementID, GetInfo, _offset);
                }

                if (_settings.export_AllinOne)
                {
                    _logger.Log("파일 출력 시작 : All in One");
                    string allInOnePath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.AllInOneReportName);
                    File.WriteAllText(allInOnePath, allInOneSb.ToString(), Encoding.UTF8);
                    _logger.Log("파일 출력 완료 : AllinOne");
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
            }
            _logger.Log("테스트 결과 처리 및 보고 완료");
        }
        
        private void ProcessDuplicateTest(ClashTest test)
        {
            foreach (ClashResult nwissue in test.Children)
            {
                if (Math.Round(nwissue.Distance, 3) == 0)
                {
                    StringBuilder _sb = new StringBuilder();
                    _sb.Append(GetElementID(nwissue.Item1)).Append(GetElementID(nwissue.Item2));
                    if (test.DisplayName.Contains("Arch")) _lstArchDupl.Add(_sb.ToString());
                    if (test.DisplayName.Contains("Str")) _lstStrDupl.Add(_sb.ToString());
                }
            }
            _logger.Log($"    Duplicates : {test.DisplayName}");
            if (test.DisplayName.Contains("Arch")) _lstArchDupl.ForEach(k => _logger.Log($"    {k}"));
            if (test.DisplayName.Contains("Str")) _lstStrDupl.ForEach(k => _logger.Log($"    {k}"));
        }

        private List<ClashResultCls> ProcessClashResults(ClashTest test)
        {
            var test_results_array = new List<ClashResultCls>();
            foreach (ClashResult nwissue in test.Children)
            {
                #region 불필요 부재 제거
                string temp1 = GetInfo(nwissue.Item1, "항목", "유형");
                if (string.Compare(temp1, "IfcDistributionPort") == 0 || temp1.Contains("IfcDistributionSystem") || temp1.Contains("IfcPipeFitting") || temp1.Contains("IfcValve"))
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("[1]" + temp1); }
                    continue;
                }
                string temp2 = GetInfo(nwissue.Item2, "항목", "유형");
                if (string.Compare(temp2, "IfcDistributionPort") == 0 || temp2.Contains("IfcDistributionSystem") || temp2.Contains("IfcPipeFitting") || temp2.Contains("IfcValve"))
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("[2]" + temp2); }
                    continue;
                }
                string temp11 = ""; try { temp11 = GetInfo(nwissue.Item1.FindFirstObjectAncestor(), "항목", "유형"); } catch { }
                if (temp11.Contains("IfcPipeFitting") || temp11.Contains("IfcValve"))
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("[1]" + temp11); }
                    continue;
                }
                string temp22 = ""; try { temp22 = GetInfo(nwissue.Item2.FindFirstObjectAncestor(), "항목", "유형"); } catch { }
                if (temp22.Contains("IfcPipeFitting") || temp22.Contains("IfcValve"))
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("[2]" + temp22); }
                    continue;
                }
                if (temp11.Contains("IfcStair") && temp22.Contains("IfcStair"))
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("복합부재-IfcStair"); }
                    continue;
                }
                #endregion

                ClashResultCls oEachResult = new ClashResultCls
                {
                    DisplayName = nwissue.DisplayName,
                    Item1 = nwissue.Item1,
                    Item2 = nwissue.Item2,
                    Status = Enum.GetName(typeof(ClashResultStatus), nwissue.Status),
                    CenterPt = new double[3] { nwissue.Center.X, nwissue.Center.Y, nwissue.Center.Z },
                    distance = nwissue.Distance,
                    Namespace1 = GetInfo(nwissue.Item1, "항목", "소스 파일").Replace(".ifc", ""),
                    Namespace2 = GetInfo(nwissue.Item2, "항목", "소스 파일").Replace(".ifc", ""),
                    path1ID = GetInfo(nwissue.Item1, "요소", "IfcGUID"),
                    path2ID = GetInfo(nwissue.Item2, "요소", "IfcGUID")
                };

                #region IfcSystem이 동일한 MEP부재 모으기
                // ... (omitted for brevity, logic is complex)
                #endregion

                if (string.Compare(oEachResult.path1ID, oEachResult.path2ID) == 0)
                {
                    if (_settings.export_UselessClash) { _listUselessClashes.Add(nwissue); _listUselessReasons.Add("GUID1과 GUID2 같음"); }
                    continue;
                }

                test_results_array.Add(oEachResult);
            }
            return test_results_array;
        }

        private (List<string[]> csvData, List<string> clashNumList) ProcessTestArrayForCsv(ClashTestCls testArray)
        {
            var csvData = new List<string[]>();
            var clashNumList = new List<string>();
            var multiObjects = new HashSet<string>();
            var multiObjects_Cur = new HashSet<string>();

            foreach (var result in testArray.ClashResults)
            {
                string[] temp = new string[12];
                temp[0] = result.DisplayName;
                temp[1] = result.Namespace1;
                temp[2] = result.path1ID;
                temp[3] = GetInfo(result.Item1, "요소", "IfcClass");
                if (string.IsNullOrEmpty(temp[3])) temp[3] = GetInfo(result.Item1.FindFirstObjectAncestor(), "요소", "IfcClass");
                temp[4] = result.Namespace2;
                temp[5] = result.path2ID;
                temp[6] = GetInfo(result.Item2, "요소", "IfcClass");
                if (string.IsNullOrEmpty(temp[6])) temp[6] = GetInfo(result.Item2.FindFirstObjectAncestor(), "요소", "IfcClass");
                temp[7] = result.distance < 0 ? "Hard" : testArray.testType.ToString();
                temp[8] = $"\"{result.CenterPt[0] * _offset:F3},{result.CenterPt[1] * _offset:F3},{result.CenterPt[2] * _offset:F3}\"";
                temp[9] = (result.distance * _offset).ToString("F3");
                temp[10] = result.IfcSystem1;
                temp[11] = result.IfcSystem2;

                // ... Filtering logic for duplicates and distance ...

                csvData.Add(temp);
                if (temp[7] == "Hard") clashNumList.Add(temp[0]);
            }
            return (csvData, clashNumList);
        }

        public string GetElementID(ModelItem item)
        {
            try
            {
                switch (GetInfo(item, "요소", "IfcClass"))
                {
                    case "IfcMember":
                    case "IfcPlate":
                        if (GetInfo(item.FindFirstObjectAncestor(), "요소", "IfcClass").Equals("IfcCurtainWall"))
                        {
                            return GetElementID(item.FindFirstObjectAncestor());
                        }
                        break;
                }
                DataProperty oDP = item.PropertyCategories.FindCategoryByName("LcRevitData_Element")?.Properties.FindPropertyByDisplayName("IfcGUID");
                if (oDP == null) oDP = item.PropertyCategories.FindCategoryByDisplayName("요소")?.Properties.FindPropertyByDisplayName("IfcGUID");
                if (oDP != null) return oDP.Value.ToDisplayString();
            }
            catch
            {
                try
                {
                    DataProperty oDP = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByName("LcRevitData_Element").Properties.FindPropertyByDisplayName("IfcGUID");
                    if (oDP == null) oDP = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName("요소").Properties.FindPropertyByDisplayName("IfcGUID");
                    if (oDP != null) return _settings.show_fromparent ? "fromparent_" + oDP.Value.ToDisplayString() : oDP.Value.ToDisplayString();
                }
                catch { return "[                  ]"; }
            }
            return "[                  ]";
        }

        public string GetInfo(ModelItem item, string category, string property)
        {
            string info = "";
            try { info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString(); }
            catch { info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString(); }
            if (string.IsNullOrEmpty(info))
            {
                try { info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString(); }
                catch { info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString(); }
            }
            return info;
        }
    }
}