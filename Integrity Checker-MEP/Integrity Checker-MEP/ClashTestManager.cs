using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Integrity_Checker_MEP
{
    public class ClashTestManager
    {
        private readonly ILogger _logger;
        private readonly Document _document;
        private readonly double _toleranceOffset;

        public ClashTestManager(ILogger logger, Document document)
        {
            _logger = logger;
            _document = document;
            // Calculate tolerance offset once. Assumes model at index 0 is the reference.
            _toleranceOffset = _document.Models.Count > 0 
                ? UnitConversion.ScaleFactor(Units.Millimeters, _document.Models[0].Units) 
                : 1.0;
        }

        public void RunAllTests()
        {
            CreateClashTests();
            ExecuteAllClashTests();
        }

        private void CreateClashTests()
        {
            var availableModels = _document.Models
                .Select((model, index) => new {
                    Name = Path.GetFileNameWithoutExtension(model.RootItem.DisplayName),
                    Index = index
                })
                .Where(x => ProjectSettings.ModelNames.Contains(x.Name))
                .ToList();

            var modelDict = availableModels.ToDictionary(x => x.Name, x => x.Index);

            // Duplicate Tests
            CreateTestPairs("Duplicate", ClashTestType.Duplicate, modelDict, new (string, string)[] {
                ("Arch", "Arch"),
                ("Str", "Str")
            });

            // Clearance Tests
            CreateTestPairs("Clearance", ClashTestType.Clearance, modelDict, new (string, string)[] {
                ("Arch", "Arch"), ("Arch", "COMM"), ("Arch", "ELEC"), ("Arch", "FIRE"), ("Arch", "MECH"), ("Arch", "Str"),
                ("COMM", "COMM"), ("COMM", "ELEC"), ("COMM", "FIRE"), ("COMM", "MECH"),
                ("ELEC", "ELEC"), ("ELEC", "FIRE"), ("ELEC", "MECH"),
                ("FIRE", "FIRE"), ("FIRE", "MECH"),
                ("MECH", "MECH"),
                ("Str", "COMM"), ("Str", "ELEC"), ("Str", "FIRE"), ("Str", "MECH"), ("Str", "Str")
            });
        }

        private void CreateTestPairs(string testTypeName, ClashTestType testType, Dictionary<string, int> modelDict, (string, string)[] pairs)
        {
            foreach (var (nameA, nameB) in pairs)
            {
                if (modelDict.ContainsKey(nameA) && modelDict.ContainsKey(nameB))
                {
                    int i = modelDict[nameA];
                    int j = modelDict[nameB];
                    string testName = $"{nameA}-{nameB}_{testTypeName}";
                    
                    // Determine tolerance
                    double tolerance = ((nameA == "Arch" && nameB == "Arch") || (nameA == "Arch" && nameB == "Str") || (nameA == "Str" && nameB == "Str")) ? 0.0 : 50.0;

                    CreateNewClashTest(testName, tolerance, testType, i, j);
                }
            }
        }

        private void CreateNewClashTest(string clashtestName, double tolerance, ClashTestType type, int modelIndexA, int modelIndexB)
        {
            DocumentClash documentClash = _document.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            if (oDCT.Tests.Any(x => x.DisplayName == clashtestName))
            {
                _logger.Log($"(경고)테스트 존재 {clashtestName}, {tolerance}mm, {type}");
                return;
            }

            ClashTest ct = new ClashTest
            {
                Tolerance = tolerance * _toleranceOffset,
                DisplayName = clashtestName,
                CustomTestName = clashtestName,
                TestType = type,
                MergeComposites = true
            };

            ModelItemCollection oSelA = new ModelItemCollection { _document.Models[modelIndexA].RootItem };
            ModelItemCollection oSelB = new ModelItemCollection { _document.Models[modelIndexB].RootItem };

            ct.SelectionA.Selection.CopyFrom(oSelA);
            ct.SelectionB.Selection.CopyFrom(oSelB);

            oDCT.TestsAddCopy(ct);
            _logger.Log($"테스트 생성 {clashtestName}, {tolerance}mm, {type}");
        }

        private void ExecuteAllClashTests()
        {
            foreach (var test in _document.GetClash().TestsData.Tests)
            {
                ExecuteClashTest(test.DisplayName);
            }
        }

        private void ExecuteClashTest(string clashtestName)
        {
            _logger.Log($"테스트 시작 : {clashtestName}");
            DocumentClash documentClash = _document.GetClash();
            if (documentClash.TestsData.Tests.FirstOrDefault(x => x.DisplayName == clashtestName) is ClashTest clashTest)
            {
                if (clashTest.SelectionA == null || clashTest.SelectionB == null)
                {
                    _logger.Log("(경고)각 선택에서 하나 이상의 모델을 선택하세요");
                    return;
                }
                documentClash.TestsData.TestsRunTest(clashTest);
                _logger.Log($"테스트 종료 : {clashtestName}");
            }
            else
            {
                _logger.Log($"(경고){clashtestName}이라는 이름을 가진 테스트가 없습니다.");
            }
        }
    }
}
