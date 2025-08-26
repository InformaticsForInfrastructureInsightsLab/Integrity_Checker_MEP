using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.DocumentParts;
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
                        // IfcSystem이 동일한 MEP 간의 간섭은 간섭으로 판단 하지 않음
                        
                        //두 부재가 mep일 때
                        if ((oEachResult.Namespace1.Contains("COMM") && oEachResult.Namespace2.Contains("COMM"))
                            || (oEachResult.Namespace1.Contains("COMM") && oEachResult.Namespace2.Contains("ELEC"))
                            || (oEachResult.Namespace1.Contains("COMM") && oEachResult.Namespace2.Contains("FIRE"))
                            || (oEachResult.Namespace1.Contains("COMM") && oEachResult.Namespace2.Contains("MECH"))
                            || (oEachResult.Namespace1.Contains("ELEC") && oEachResult.Namespace2.Contains("ELEC"))
                            || (oEachResult.Namespace1.Contains("ELEC") && oEachResult.Namespace2.Contains("FIRE"))
                            || (oEachResult.Namespace1.Contains("ELEC") && oEachResult.Namespace2.Contains("MECH"))
                            || (oEachResult.Namespace1.Contains("FIRE") && oEachResult.Namespace2.Contains("FIRE"))
                            || (oEachResult.Namespace1.Contains("FIRE") && oEachResult.Namespace2.Contains("MECH"))
                            || (oEachResult.Namespace1.Contains("MECH") && oEachResult.Namespace2.Contains("MECH"))) {
                            oEachResult.IfcSystem1 = "";
                            oEachResult.IfcSystem2 = "";
                            try {
                                oEachResult.IfcSystem1 = $"{GetInfo(nwissue.Item1, "요소", "IfcSystem")}";
                            } 
                            catch { // 가끔 간섭을 일으킨 부재가 IfcSystem 속성을 가지지 않을 때가 있으므로 해당 부재의 부모의 IfcSystem 확인
                                try {
                                    oEachResult.IfcSystem1 = $"{GetInfo(nwissue.Item1.FindFirstObjectAncestor(), "요소", "IfcSystem")}";
                                    if (string.IsNullOrEmpty(oEachResult.IfcSystem1)) oEachResult.IfcSystem1 = "";
                                }
                                catch {
                                    oEachResult.IfcSystem1 = "";
                                }
                            }
                            try {
                                oEachResult.IfcSystem2 = $"{GetInfo(nwissue.Item2, "요소", "IfcSystem")}";
                                if (string.IsNullOrEmpty(oEachResult.IfcSystem2)) oEachResult.IfcSystem2 = "";
                            }
                            catch {
                                try {
                                    oEachResult.IfcSystem2 = $"{GetInfo(nwissue.Item2.FindFirstObjectAncestor(), "요소", "IfcSystem")}";
                                }
                                catch {
                                    oEachResult.IfcSystem2 = "";
                                }
                            }

                            //둘이 빈칸이 아니고 같으면 제거
                            if (!oEachResult.IfcSystem1.Equals("")) {
                                if (string.Compare(oEachResult.IfcSystem1, oEachResult.IfcSystem2) == 0) {
                                    if (_settings.export_SameIfcSystem) {
                                        _listSameIfcSystem.Add(new IfcSystemData(nwissue, oEachResult.IfcSystem1, oEachResult.IfcSystem2, oEachResult.Namespace1));
                                    }
                                    continue;
                                }
                            }
                        }
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

                #region 중복제거 : 최초 등장 간섭 외 제거(간섭거리가 가장 작은 간섭 외 제거)
                //CurtainWall은 CurtainWall끼리만 비교
                StringBuilder sb_multi = new StringBuilder();
                sb_multi.Append(temp[1]).Append(temp[2]).Append(temp[4]).Append(temp[5]);

                StringBuilder sb_iscur = new StringBuilder();
                sb_iscur.Append(temp[3]).Append(temp[6]);

                if (sb_iscur.ToString().Contains("IfcCurtainWall"))
                {
                    if (multiObjects_Cur.Contains(sb_multi.ToString()))
                    {
                        if (_settings.export_UselessClash)
                        {
                            _listUselessClashes2.Add(result);
                            _listUselessReasons2.Add("동일 간섭 존재(CurtainWall)");
                            continue;
                        }
                    }
                    else
                    {
                        multiObjects_Cur.Add(sb_multi.ToString());
                    }
                }
                else
                {
                    if (multiObjects.Contains(sb_multi.ToString()))
                    {
                        if (_settings.export_UselessClash)
                        {
                            _listUselessClashes2.Add(result);
                            _listUselessReasons2.Add("동일 간섭 존재");
                            continue;
                        }
                    }
                    else
                    {
                        multiObjects.Add(sb_multi.ToString());
                    }
                }
                #endregion

                #region str-mep가 아닌 간섭에서 distance가 0이하 ~ -0.01m초과인 간섭 제외
                double dis = result.distance;
                if (temp[1].Contains("Str"))
                {
                    // str-str 간섭
                    if (temp[4].Contains("Str"))
                    {
                        if (Math.Round(dis, 3) <= 0 && dis > -10)
                        {
                            if (dis == 0)
                            {
                                StringBuilder _sb = new StringBuilder();
                                _sb.Append(temp[2]).Append(temp[5]);
                                if (_lstStrDupl.Contains(_sb.ToString()))
                                { // Duplicate에 존재 시 제거하지 않음
                                    temp[7] = "Hard";
                                }
                                else
                                {
                                    _listUselessClashes2.Add(result);
                                    _listUselessReasons2.Add("거리 0 간섭");
                                    continue;
                                }
                            }
                            else
                            {
                                _listUselessClashes2.Add(result);
                                _listUselessReasons2.Add("공차 내 간섭");
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (temp[1] == "Arch" && temp[4] == "Arch")
                    {
                        if (Math.Round(dis, 3) <= 0 && dis > -10)
                        {
                            if (dis == 0)
                            {
                                StringBuilder _sb = new StringBuilder();
                                _sb.Append(temp[2]).Append(temp[5]);
                                if (_lstArchDupl.Contains(_sb.ToString()))
                                {// Duplicate에 존재 시 제거하지 않음
                                    temp[7] = "Hard";
                                }
                                else
                                {
                                    _listUselessClashes2.Add(result);
                                    _listUselessReasons2.Add("거리 0 간섭");
                                    continue;
                                }
                            }
                            else
                            {
                                _listUselessClashes2.Add(result);
                                _listUselessReasons2.Add("공차 내 간섭");
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (Math.Round(dis, 3) <= 0 && dis > -10)
                        {
                            if (dis == 0)
                            {
                                _listUselessClashes2.Add(result);
                                _listUselessReasons2.Add("거리 0 간섭");
                                continue;
                            }
                            else
                            {
                                _listUselessClashes2.Add(result);
                                _listUselessReasons2.Add("공차 내 간섭");
                                continue;
                            }
                        }
                    }
                }
                #endregion

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

        public string GeneratePropertiesFile()
        {
            _logger.Log("층간 높이 측정 시작");
            Document doc = _document;
            ModelItemCollection itemCollection = new ModelItemCollection();
            DocumentModels models = doc.Models;

            if (models.Count == 0)
            {
                _logger.Log("(경고) 불러온 모델이 없습니다.");
                return null;
            }

            Dictionary<string, double> heights = new Dictionary<string, double>();
            List<string[]> props = new List<string[]>();

            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].FileName.ToUpper().Contains("COMM")
                    || models[i].FileName.ToUpper().Contains("ELEC")
                    || models[i].FileName.ToUpper().Contains("FIRE")
                    || models[i].FileName.ToUpper().Contains("MECH")) continue;
                itemCollection.AddRange(ItemsFromRoot(models[i]));
            }

            for (int i = 0; i < itemCollection.Count; i++)
            {
                string type = GetInfo(itemCollection[i], "항목", "유형");
                if (type.Equals("IfcDistributionPort") || type.Equals("IfcDistributionSystem") || type.Equals("IfcValve") || type.Equals("IfcPipeFitting"))
                {
                    itemCollection.Remove(itemCollection[i]);
                    i--; 
                }
            }

            _logger.Log("    딕셔너리 생성 시작");
            ModelItem item = null;
            for (int i = 0; i < itemCollection.Count; i++)
            {
                item = itemCollection[i];
                try
                {
                    string[] temp = new string[3];
                    temp[0] = GetElementID(item);
                    temp[1] = GetInfo(item, "요소", "IfcClass");
                    if (temp[1].Equals("IfcDistributionPort") || temp[1].Equals("IfcDistributionSystem") || temp[1].Equals("IfcOpeningElement")) continue;

                    temp[2] = GetInfo(item, "요소", "IfcSpatialContainer");
                    if (string.IsNullOrEmpty(temp[2]))
                    {
                        temp[2] = GetInfo(item, "항목", "도면층");
                    }
                    if (!string.IsNullOrEmpty(temp[2]) && !heights.ContainsKey(temp[2]))
                    {
                        heights.Add(temp[2], 0f);
                    }
                    props.Add(temp);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.ToString());
                }
            }
            _logger.Log("    딕셔너리 생성 완료");

            Dictionary<string, double> newHeights = new Dictionary<string, double>();
            _logger.Log("    층간 최대값 찾기 시작");
            try
            {
                foreach (var level in heights.Keys)
                {
                    HashSet<double> height_walls = new HashSet<double>();
                    for (int i = 0; i < itemCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(GetInfo(itemCollection[i], "요소", "IfcSpatialContainer")) && string.Compare(GetInfo(itemCollection[i], "요소", "IfcSpatialContainer"), level) == 0)
                        {
                            if (!string.IsNullOrEmpty(GetInfo(itemCollection[i], "요소", "IfcSpatialContainer")) && string.Compare(GetInfo(itemCollection[i], "요소", "IfcClass"), "IfcWall") == 0)
                            {
                                try
                                {
                                    double h = double.Parse(GetInfo(itemCollection[i], "Constraints", "Unconnected Height").Split(':')[1]);
                                    double offsetValue = 0;
                                    string s_offset = GetInfo(itemCollection[i], "Constraints", "Base Offset");
                                    if (!string.IsNullOrEmpty(s_offset))
                                    {
                                        offsetValue = double.Parse(s_offset.Split(':')[1]);
                                    }
                                    height_walls.Add(Math.Abs(h + offsetValue));
                                }
                                catch (Exception ex)
                                {
                                    _logger.Log(ex.ToString());
                                    return null;
                                }
                            }
                        }
                    }
                    if (height_walls.Count != 0)
                    {
                        newHeights.Add(level, height_walls.Max());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
            }

            List<string[]> newprops = new List<string[]>();
            try
            {
                for (int i = 0; i < props.Count; i++)
                {
                    if (props[i] != null)
                    {
                        if (newHeights.ContainsKey(props[i][2]))
                        {
                            props[i][2] = (newHeights[props[i][2]] * _offset).ToString("F3");
                            newprops.Add(props[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
            }
            _logger.Log("    층간 최대값 찾기 완료");

            List<string[]> ifcspace = new List<string[]>();
            for (int i = 0; i < newprops.Count; i++)
            {
                if (string.Compare(newprops[i][1], "IfcSpace") == 0)
                {
                    ifcspace.Add(newprops[i]);
                }
            }

            string[] header2 = new string[3];
            header2[0] = "Guid";
            header2[1] = "Type";
            header2[2] = "Level_height";
            ifcspace.Insert(0, header2);

            _reportGenerator.SaveCsvFile("Properties", ifcspace);
            _logger.Log("층간 높이 측정 완료");
            return "Properties.csv";
        }

        public IEnumerable<ModelItem> ItemsFromRoot(Model model)
        {
            return model.RootItem.Descendants.Where(x => x.HasGeometry);
        }
    }
}