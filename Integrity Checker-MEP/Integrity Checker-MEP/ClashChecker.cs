using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Newtonsoft.Json;
using wf = System.Windows.Forms;

[Serializable]
public class ClashTestCls {
    public string DisplayName { get; set; }
    public ClashResultCls[] ClashResults { get; set; }
    public ClashTestType testType { get; set; }
}

[Serializable]
public class ClashResultCls {
    public string DisplayName { get; set; }
    public string Status { get; set; }
    public string Namespace1 { get; set; }
    public string Namespace2 { get; set; }
    public string path1ID { get; set; }
    public string path2ID { get; set; }
    public double distance { get; set; }
    public double[] CenterPt { get; set; }
    public ModelItem Item1 { get; set; }
    public ModelItem Item2 { get; set; }
    public string SourceFile { get; set; }
    public string IfcSystem1 { get; set; }
    public string IfcSystem2 { get; set; }
}

class customTest {
    string name;
    float toler;
    public customTest(string name = "테스트 1", float toler = 0.01f) {
        this.toler = toler;
        this.name = name;
    }

    public string GetName() { return name; }
    public float GetToler() { return toler; }
}

class IfcSystemData {
    public ClashResult result;
    public string ns;
    public string ifc1;
    public string ifc2;

    public IfcSystemData(ClashResult result, string ifc1, string ifc2, string ns) {
        this.result = result;
        this.ifc1 = ifc1;
        this.ifc2 = ifc2;
        this.ns = ns;
    }
}

public class ExtendedWebClient : WebClient
{
    public int Timeout { get; set; }
    public new bool AllowWriteStreamBuffering { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        try
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
                var httpRequest = request as HttpWebRequest;
                if (httpRequest != null)
                {
                    httpRequest.KeepAlive = true;
                    httpRequest.AllowWriteStreamBuffering = AllowWriteStreamBuffering;
                }
            }

            return request;
        }
        catch (Exception e) {
            Console.WriteLine($"An exception occurred: {e.Message}");
            throw;
        }
    }

    public ExtendedWebClient()
    {
        Timeout = System.Threading.Timeout.Infinite;
    }
}

namespace Integrity_Checker_MEP {
    public class ClashChecker {
        bool show_Setting = false;
        
        Form_Setting form_setting; // 초기 세팅 폼 (디버그용)
        From_Log form_log; // 로그 출력 폼
        ImageCreator_new imgCreator; // 스크린샷 저장 클래스
        IFCLoad ifcLoader;
        public int Execute(params string[] parameters) {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            doc.Clear();
            try {
                ifcLoader = new IFCLoad();
                ifcLoader.ShowDialog();

                if (!ifcLoader.load) return 0;

                form_setting = new Form_Setting();
                

                form_setting.ShowDialog();

                form_log = new From_Log();
                form_log.Show();


                if (!form_setting.start) return 0; // 세팅폼에서 취소를 누를 시 종료
                
                if (form_setting.Save_image)
                {
                    imgCreator = new ImageCreator_new(form_log);
                }

                
                if (!GetModelFromFolder(@"C:\models\")) return 0; // 모델을 불러오지 못했을 경우 종료

                if (form_setting.export_Result) {
                    MakeClashTest(); // 테스트 생성
                    TestClashTest(); // 테스트 실행
                    GetClashResult(); // 테스트 결과 받기
                }
                if(form_setting.Save_image)
                {
                    SaveImage(); // 이미지 추출
                }
                if (form_setting.export_Properties) {
                    GetSpaceHeights(); // 공간별 높이 구하기
                }
                if (form_setting.Make_ZipFile) {
                    MakeZipFile(ClashFile, InfoFile); // 압축파일 만들기
                }
                if (form_setting.Send_toServer) {
                    try
                    {
                        SendtoServer("C:/objectinfo/compressed.zip"); // 서버로 압축파일 전송

                    }
                    catch (Exception e)
                    {
                        form_log.UpdateLog("서버 전송 실패");
                        form_log.UpdateLog(e.ToString());
                    }
                }
                if (form_setting.Save_log) {
                    SaveLog(); // 로그를 파일로 저장
                }
                form_log.UpdateLog("종료");
            }
            catch (Exception ex) {
                form_log.UpdateLog(ex.ToString());
                //ShowMessage(ex.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 이미지를 C:/objectinfo/ResultImage 경로에 저장하는 함수
        /// </summary>
        void SaveImage()
        {
            form_log.UpdateLog("이미지 추출 작업 시작");


            // 이미지가 저장될 폴더를 만들어 준다
            string resultImagePath = @"C:/objectinfo/ResultImage";
            if (!Directory.Exists(resultImagePath))
            {
                
                Directory.CreateDirectory(resultImagePath);
            }

            // simple -> 단순 (오른쪽 위), side -> 다각도
            // 단순이 필요하면 주석 제외

            //string simpleImagePath = Path.Combine(resultImagePath, "단순이미지");
            string sideImagePath = Path.Combine(resultImagePath, "다각도이미지");

            /*if (!Directory.Exists(simpleImagePath))
            {
                
                Directory.CreateDirectory(simpleImagePath);
            }*/
            if (!Directory.Exists(sideImagePath))
            {
                
                Directory.CreateDirectory(sideImagePath);
            }


            // 이미지로 추출할 간섭 결과들을 갖고있는 json파일을 불러오고 imgCreator에 넘겨준다
            Dictionary<string, List<string>> tests_dict;
            try
            {
                tests_dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(@"c:\objectinfo\tests_dict.json"));
                imgCreator.save_image(tests_dict, sideImagePath);
            }
            catch(Exception ex)
            {
                form_log.UpdateLog(ex.ToString());
            }

            form_log.UpdateLog("이미지 추출 작업 종료");

        }

        //로그 저장
        void SaveLog() {
            StringBuilder sb = new StringBuilder();
            string path = @"C:/objectinfo/log.txt";
            foreach (var item in form_log.lst_progress.Items) {
                sb.AppendLine(item.ToString());
            }
            System.IO.File.WriteAllText(path, sb.ToString());
        }

        double offset;
        double toleroffset;
        /// <summary>
        /// "path" 폴더 안에있는 .ifc파일을 현재워크시트에 불러옵니다.
        /// 변경: 대화상자에서 선택한 모델만을 불러옵니다.
        /// </summary>
        /// <param name="path"> 모델 파일 경로(폴더)</param>
        bool GetModelFromFolder(string path = IFCLoad.path) {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            //if(doc.Models.Count > 0)
            //{
            //    return true;
            //}
            ////경로에서 .ifc파일만 찾아 불러오기
            //for (int i = 0; i < Directory.GetFiles(path).Length; i++) {
            //    // .ifc만 불러옴 .ifc.log 필터링
            //    if (Directory.GetFiles(path)[i].Contains(".ifc") && Directory.GetFiles(path)[i].Contains(".ifc.")) {
            //        doc.AppendFile(Directory.GetFiles(path)[i]);
            //        form_log.UpdateLog($"ifc 불러오기 : {Directory.GetFiles(path)[i]}");
            //    }
            //}

            //if (doc.Models.Count == 0) {
            //    ShowMessage("불러온 모델이 없습니다.");
            //    return false;
            //}

            //경로에서 .ifc파일만 찾아 불러오기
            for (int i = 0; i < doc.Models.Count; i++)
            {
                form_log.UpdateLog($"ifc 불러오기 : {doc.Models.ToString()}");
            }

            //모델별 단위 통합
            toleroffset = UnitConversion.ScaleFactor(Units.Millimeters, doc.Models[0].Units);
            offset = UnitConversion.ScaleFactor(doc.Models[0].Units, Units.Millimeters);

            //설정값 보이기
            if (show_Setting) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"모델 단위 보정값 : {offset}");
                sb.AppendLine($"공차 단위 보정값 : {toleroffset}");
                sb.AppendLine($"부모 GUID 획득 표시 : {form_setting.show_fromparent}");
                sb.AppendLine($"간섭 결과 파일 출력 : {form_setting.export_Result}");
                sb.AppendLine($"All in One 파일 출력 : {form_setting.export_AllinOne}");
                sb.AppendLine($"불필요간섭결과 파일 출력 : {form_setting.export_UselessClash}");
                sb.AppendLine($"동일 IfcSystem 파일 출력 : {form_setting.export_SameIfcSystem}");
                sb.AppendLine($"Properties 파일 출력 : {form_setting.export_Properties}");
                sb.AppendLine($"Properties 파일 분할 : {form_setting.cut_Properties}");
                sb.AppendLine($"Zip 파일 생성 : {form_setting.Make_ZipFile}");
                sb.AppendLine($"Zip 파일 서버 전송 : {form_setting.Send_toServer}");
                sb.AppendLine($"로그 저장 : {form_setting.Save_log}");
                ShowMessage(sb.ToString());
            }
            return true;
        }

        void MakeClashTest() {
            // 모델 이름 배열 (순서가 중요)
            string[] models = { "Arch", "COMM", "ELEC", "FIRE", "MECH", "Str" };

            // 모델이 실제로 존재하는지 확인
            Document document = Autodesk.Navisworks.Api.Application.ActiveDocument;
            var availableModels = document.Models
                .Select((model, index) => new {
                    Model = model,
                    Name = Path.GetFileNameWithoutExtension(model.RootItem.DisplayName),
                    Index = index
                })
                .Where(x => models.Contains(x.Name))  // 확장자를 제거한 이름만 선택
                .ToList();

            // 모델 이름과 인덱스를 딕셔너리에 저장
            var modelDict = availableModels.ToDictionary(x => x.Name, x => x.Index);

            // Duplicate Tests
            CreateTest("Duplicate", ClashTestType.Duplicate, modelDict, new (string, string)[] {
                ("Arch", "Arch"), // Arch-Arch
                ("Str", "Str")    // Str-Str
            });

            // Clearance Tests
            CreateTest("Clearance", ClashTestType.Clearance, modelDict, new (string, string)[] {
                ("Arch", "Arch"), // Arch-Arch
                ("Arch", "COMM"), // Arch-COMM
                ("Arch", "ELEC"), // Arch-ELEC
                ("Arch", "FIRE"), // Arch-FIRE
                ("Arch", "MECH"), // Arch-MECH
                ("Arch", "Str"),  // Arch-Str
                ("COMM", "COMM"), // COMM-COMM
                ("COMM", "ELEC"), // COMM-ELEC
                ("COMM", "FIRE"), // COMM-FIRE
                ("COMM", "MECH"), // COMM-MECH
                ("ELEC", "ELEC"), // ELEC-ELEC
                ("ELEC", "FIRE"), // ELEC-FIRE
                ("ELEC", "MECH"), // ELEC-MECH
                ("FIRE", "FIRE"), // FIRE-FIRE
                ("FIRE", "MECH"), // FIRE-MECH
                ("MECH", "MECH"), // MECH-MECH
                ("Str", "COMM"),  // Str-COMM
                ("Str", "ELEC"),  // Str-ELEC
                ("Str", "FIRE"),  // Str-FIRE
                ("Str", "MECH"),  // Str-MECH
                ("Str", "Str")    // Str-Str
            });
        }

        void CreateTest(string testTypeName, ClashTestType testType, Dictionary<string, int> modelDict, (string, string)[] pairs) {
            foreach (var (nameA, nameB) in pairs) {
                if (modelDict.ContainsKey(nameA) && modelDict.ContainsKey(nameB)) {
                    int i = modelDict[nameA];
                    int j = modelDict[nameB];
                    string testName = $"{nameA}-{nameB}_{testTypeName}";
                    NewClashTest(testName, 50f, testType, i, j);
                }
            }
        }

        void NewClashTest(string ClashtestName, double toler, ClashTestType type, int i, int j) {
            // 현재 Navisworks에 접근
            DocumentClash documentClash = Autodesk.Navisworks.Api.Application.ActiveDocument.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;
            ClashTest ct = documentClash.TestsData.Tests.Where(x => string.Compare(x.DisplayName, ClashtestName) == 0).FirstOrDefault() as ClashTest;
            if (ct == null) {
                ct = new ClashTest();
                ct.Tolerance = toler * toleroffset;//모델 단위 보정 적용 공차
                ct.DisplayName = ClashtestName;
                ct.CustomTestName = ClashtestName;
                ct.TestType = type;
                ct.MergeComposites = true;

                //선택A와 선택B 설정
                ModelItemCollection oSelA = new ModelItemCollection();
                ModelItemCollection oSelB = new ModelItemCollection();

                // 모델 선택
                Document document = Autodesk.Navisworks.Api.Application.ActiveDocument;
                oSelA.Add(document.Models[i].RootItem);
                oSelB.Add(document.Models[j].RootItem);
                ct.SelectionA.Selection.CopyFrom(oSelA);
                ct.SelectionB.Selection.CopyFrom(oSelB);
                oDCT.TestsAddCopy(ct);
                form_log.UpdateLog($"테스트 생성 {ClashtestName}, {toler}mm, {type}");
            }
            else {
                form_log.UpdateLog($"(경고)테스트 존재 {ClashtestName}, {toler}mm, {type}");
            }
        }

        /// <summary>
        /// 지정한 형식으로 생성된 테스트 실행
        /// </summary>
        void TestClashTest() {
            foreach (var name in Application.ActiveDocument.GetClash().TestsData.Tests) {
                TestClashDetective(name.DisplayName);
            }
        }

        /// <summary>
        /// DisplayName이 'ClashtestName'인 테스트의 간섭검토 실행
        /// </summary>
        /// <param name="ClashtestName"> 실행 하고자하는 테스트의 DisplayName</param>
        void TestClashDetective(string ClashtestName) {
            form_log.UpdateLog($"테스트 시작 : {ClashtestName}");
            DocumentClash documentClash = Application.ActiveDocument.GetClash();
            ClashTest clashTestsData = documentClash.TestsData.Tests.Where(x => string.Compare(x.DisplayName, ClashtestName) == 0).FirstOrDefault() as ClashTest;
            if (clashTestsData == null) {
                form_log.UpdateLog($"(경고){ClashtestName}이라는 이름을 가진 테스트가 없습니다.");
                //ShowMessage($"{ClashtestName}이라는 이름을 가진 테스트가 없습니다.");
                return;
            }
            if (clashTestsData.SelectionA == null | clashTestsData.SelectionB == null) {
                form_log.UpdateLog($"(경고)각 선택에서 하나 이상의 모델을 선택하세요");
                //ShowMessage("각 선택에서 하나 이상의 모델을 선택하세요");
                return;
            }
            documentClash.TestsData.TestsRunTest(clashTestsData);
            form_log.UpdateLog($"테스트 종료 : {ClashtestName}");
        }


        string ClashFile = "";
        List<string> lst_ArchDupl = new List<string>();
        List<string> lst_StrDupl = new List<string>();

        /// <summary>
        /// 전체/지정된 테스트의 간섭검토 결과를 CSV파일로 export
        /// </summary>
        /// <param name="ClashtestName"> 기본값 = null, 테스트 이름 설정시 해당 테스트의 결과만 접근(미설정:모든 테스트)</param>
        void GetClashResult(string ClashtestName = null) {
            form_log.UpdateLog("테스트 결과 출력 시작");
            try {
                // 현재 Navisworks 접근
                Document document = Application.ActiveDocument;
                DocumentClash documentClash = document.GetClash();
                DocumentClashTests oDCT = documentClash.TestsData;
                List<ClashTestCls> tests_array = new List<ClashTestCls>();

                // imgCreator 객체에 넘겨줄 dictionary
                Dictionary<string, List<string>> tests_dict = new Dictionary<string, List<string>>();

                #region 각 테스트 별 충돌 오브젝트 정보 가져오기
                for (int i = 0; i < oDCT.Tests.Count; i++) {
                    form_log.UpdateLog($"    결과 읽기 시작 : {oDCT.Tests[i].DisplayName}");
                    ClashTest test = oDCT.Tests[i] as ClashTest;
                    ClashTestCls oEachTest = new ClashTestCls();
                    oEachTest.DisplayName = test.DisplayName;
                    oEachTest.testType = test.TestType;

                    // 테스트 타입이 Duplicate
                    if (test.TestType == ClashTestType.Duplicate) { // guid가 다른 똑같이 생긴 부재가 겹쳐있는 경우 판별용
                        foreach (var c in test.Children) {
                            ClashResult nwissue = (ClashResult)c;
                            if (Math.Round(nwissue.Distance, 3) == 0) {
                                StringBuilder _sb = new StringBuilder();
                                _sb.Append(getElementID(nwissue.Item1)).Append(getElementID(nwissue.Item2));
                                if (test.DisplayName.Contains("Arch")) {
                                    lst_ArchDupl.Add(_sb.ToString());
                                }
                                if (test.DisplayName.Contains("Str")) {
                                    lst_StrDupl.Add(_sb.ToString());
                                }
                            }
                        }
                        form_log.UpdateLog($"    Duplicates : {test.DisplayName}");
                        if (test.DisplayName.Contains("Arch")) {
                            foreach (var k in lst_ArchDupl) {
                                form_log.UpdateLog($"    {k.ToString()}");
                            }
                        }
                        if (test.DisplayName.Contains("Str")) {
                            foreach (var k in lst_StrDupl) {
                                form_log.UpdateLog($"    {k.ToString()}");
                            }
                        }
                        continue;
                    }

                    List<ClashResultCls> test_results_array = new List<ClashResultCls>();

                    for (int j = 0; j < test.Children.Count; j++) {
                        ClashResult nwissue = (ClashResult)test.Children[j];
                        ClashResultCls oEachResult = new ClashResultCls();

                        #region 불필요 부재 제거

                        string temp1 = Getinfo(nwissue.Item1, "항목", "유형");
                        if (string.Compare(temp1, "IfcDistributionPort") == 0) { //레빗에서 ifc파일로 만드는 과정에서 생기는 화살표(실재 오브젝트가 아님)
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[1]IfcDistributionPort");
                            }
                            continue;
                        }
                        if (temp1.Contains("IfcDistributionSystem")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[1]IfcDistributionSystem");
                            }
                            continue;
                        }
                        //if (temp1.Contains("IfcSpace")) { //물리적 간섭만 고려
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[1]IfcSpace");
                        //    }
                        //    continue;
                        //}
                        //if (temp1.Contains("IfcPipeFitting")) {
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[1]IfcPipeFitting");
                        //    }
                        //    continue;
                        //}
                        if (temp1.Contains("IfcValve")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[1]IfcValve");
                            }
                            continue;
                        }
                        // 계단과 같은 복합 부재 일 경우 제외
                        string temp11 = "";
                        try {
                            temp11 = Getinfo(nwissue.Item1.FindFirstObjectAncestor(), "항목", "유형");
                        }
                        catch {
                            temp11 = "";
                        }
                        if (temp11.Contains("IfcStair")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[1]복합부재-IfcStair");
                            }
                            continue;
                        }
                        //if (temp11.Contains("IfcPipeFitting")) {
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[1]IfcPipeFitting");
                        //    }
                        //    continue;
                        //}
                        if (temp11.Contains("IfcValve")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[1]IfcValve");
                            }
                            continue;
                        }

                        string temp2 = Getinfo(nwissue.Item2, "항목", "유형");
                        if (string.Compare(temp2, "IfcDistributionPort") == 0) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[2]IfcDistributionPort");
                            }
                            continue;
                        }
                        if (temp2.Contains("IfcDistributionSystem")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[2]IfcDistributionSystem");
                            }
                            continue;
                        }
                        //if (temp2.Contains("IfcSpace")) {
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[2]IfcSpace");
                        //    }
                        //    continue;
                        //}
                        //if (temp2.Contains("IfcPipeFitting")) {
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[2]IfcPipeFitting");
                        //    }
                        //    continue;
                        //}
                        if (temp2.Contains("IfcValve")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[2]IfcValve");
                            }
                            continue;
                        }
                        string temp22 = "";
                        try {
                            temp22 = Getinfo(nwissue.Item2.FindFirstObjectAncestor(), "항목", "유형");
                        }
                        catch {
                            temp22 = "";
                        }
                        if (temp22.Contains("IfcStair")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add((nwissue));
                                List_uselessReasons.Add("[2]복합부재-IfcStair");
                            }
                            continue;
                        }
                        //if (temp22.Contains("IfcPipeFitting")) {
                        //    if (form_setting.export_UselessClash) {
                        //        List_uselessClashes.Add(nwissue);
                        //        List_uselessReasons.Add("[2]IfcPipeFitting");
                        //    }
                        //    continue;
                        //}
                        if (temp22.Contains("IfcValve")) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("[2]IfcValve");
                            }
                            continue;
                        }

                        #endregion

                        #region IfcSystem이 동일한 MEP부재 모으기
                        // IfcSystem이 동일한 MEP 간의 간섭은 간섭으로 판단 하지 않음
                        
                        //출처 파일 가져오기
                        oEachResult.Namespace1 = Getinfo(nwissue.Item1, "항목", "소스 파일").Replace(".ifc", "");
                        oEachResult.Namespace2 = Getinfo(nwissue.Item2, "항목", "소스 파일").Replace(".ifc", "");
                        
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
                                oEachResult.IfcSystem1 = $"\"{Getinfo(nwissue.Item1, "요소", "IfcSystem")}\"";
                            }
                            catch { // 가끔 간섭을 일으킨 부재가 IfcSystem 속성을 가지지 않을 때가 있으므로 해당 부재의 부모의 IfcSystem 확인
                                try {
                                    oEachResult.IfcSystem1 = $"\"{Getinfo(nwissue.Item1.FindFirstObjectAncestor(), "요소", "IfcSystem")}\"";
                                    if (string.IsNullOrEmpty(oEachResult.IfcSystem1)) oEachResult.IfcSystem1 = "";
                                }
                                catch {
                                    oEachResult.IfcSystem1 = "";
                                }
                            }
                            try {
                                oEachResult.IfcSystem2 = $"\"{Getinfo(nwissue.Item2, "요소", "IfcSystem")}\"";
                                if (string.IsNullOrEmpty(oEachResult.IfcSystem2)) oEachResult.IfcSystem2 = "";
                            }
                            catch {
                                try {
                                    oEachResult.IfcSystem2 = $"\"{Getinfo(nwissue.Item2.FindFirstObjectAncestor(), "요소", "IfcSystem")}\"";
                                }
                                catch {
                                    oEachResult.IfcSystem2 = "";
                                }
                            }

                            //둘이 빈칸이 아니고 같으면 제거
                            if (!oEachResult.IfcSystem1.Equals("\"\"")) {
                                if (string.Compare(oEachResult.IfcSystem1, oEachResult.IfcSystem2) == 0) {
                                    if (form_setting.export_SameIfcSystem) {
                                        List_sameIfcSystem.Add(new IfcSystemData(nwissue, oEachResult.IfcSystem1, oEachResult.IfcSystem2, oEachResult.Namespace1));
                                    }
                                    continue;
                                }
                            }
                        }
                        #endregion

                        #region 간섭결과 정보 가져오기
                        oEachResult.DisplayName = nwissue.DisplayName;
                        oEachResult.Item1 = nwissue.Item1;
                        oEachResult.Item2 = nwissue.Item2;
                        oEachResult.Status = Enum.GetName(typeof(ClashResultStatus), nwissue.Status);
                        oEachResult.CenterPt = new double[3] { nwissue.Center.X, nwissue.Center.Y, nwissue.Center.Z };
                        oEachResult.distance = nwissue.Distance;

                        oEachResult.path1ID = "[Not Assigned]";
                        oEachResult.path1ID = Getinfo(nwissue.Item1, "요소", "IfcGUID");

                        oEachResult.path2ID = "[Not Assigned]";
                        oEachResult.path2ID = Getinfo(nwissue.Item2, "요소", "IfcGUID");

                        #region 자신과의 충돌
                        // 드물게 존재하는 케이스
                        if (string.Compare(oEachResult.path1ID, oEachResult.path2ID) == 0) {
                            if (form_setting.export_UselessClash) {
                                List_uselessClashes.Add(nwissue);
                                List_uselessReasons.Add("GUID1과 GUID2 같음");
                            }
                            continue;
                        }
                        #endregion

                        #endregion
                        test_results_array.Add(oEachResult);
                    }
                    oEachTest.ClashResults = test_results_array.ToArray();
                    tests_array.Add(oEachTest);
                    form_log.UpdateLog($"    결과 읽기 종료 : {oDCT.Tests[i].DisplayName}");
                }
                if (form_setting.export_SameIfcSystem) {
                    ExportSameIfcSystem();
                }
                #endregion
                #region 결과 출력 오류
                if (ClashtestName != null) {
                    for (int j = 0; j < tests_array.Count; j++) {
                        if (string.Compare(tests_array[j].DisplayName, ClashtestName) != 0) {
                            tests_array.Remove(tests_array[j]);
                        }
                    }
                    if (tests_array.Count == 0) {//해당 이름을 가진 테스트가 없음
                        ShowMessage("No Test Clash named " + ClashtestName);
                        return;
                    }
                }
                else {//찾고자 하는 테스트가 없음
                    if (tests_array.Count == 0) {
                        ShowMessage("No Test Clash");
                        return;
                    }
                }

               

                #endregion

                #region 충돌 보고서 내보내기
                #region 헤더부분
                StringBuilder sb = new StringBuilder(); //All_in_One에 들어갈 내용들
                string[] header = new string[12];
                header[0] = "No,";
                header[1] = "NameSpace1,";
                header[2] = "Guid1,";
                header[3] = "IfcClass1,";
                header[4] = "NameSpace2,";
                header[5] = "Guid2,";
                header[6] = "IfcClass2,";
                header[7] = "Type,";
                header[8] = "Clashpoint,";
                header[9] = "Distance,";
                header[10] = "IfcSystem1,";
                header[11] = "IfcSystem2";
                for (int i = 0; i < header.Length; i++) {
                    sb.Append(header[i]);
                }
                sb.Append('\n');
                #endregion
                #region 간섭결과 부분
                form_log.UpdateLog("결과 파일 생성 시작");
                for (int i = 0; i < tests_array.Count; i++) {
                    form_log.UpdateLog($"    결과 쓰기 시작 : {tests_array[i].DisplayName}");
                    var csvData = new List<string[]>();
                    
                    // 조건을 만족하는 test의 간섭 결과 이름(name)을 모아둔 리스트 ex)간섭1, 간섭2,..
                    List<string> clashNumList = new List<string>();

                    HashSet<string> MultiObjects = new HashSet<string>();
                    HashSet<string> MultiObjects_Cur = new HashSet<string>();
                    for (int j = 0; j < tests_array[i].ClashResults.Length; j++) {
                        string[] temp = new string[12];
                        //간섭번호
                        temp[0] = tests_array[i].ClashResults[j].DisplayName;
                        //Item1 파일
                        temp[1] = tests_array[i].ClashResults[j].Namespace1;
                        //Item1 Guid
                        temp[2] = tests_array[i].ClashResults[j].path1ID;
                        //IfcClass
                        temp[3] = Getinfo(tests_array[i].ClashResults[j].Item1, "요소", "IfcClass");
                        if (string.IsNullOrEmpty(temp[3])) {
                            temp[3] = Getinfo(tests_array[i].ClashResults[j].Item1.FindFirstObjectAncestor(), "요소", "IfcClass");
                        }
                        //Item2 파일
                        temp[4] = tests_array[i].ClashResults[j].Namespace2;
                        //Item2 Guid
                        temp[5] = tests_array[i].ClashResults[j].path2ID;
                        //IfcClass
                        temp[6] = Getinfo(tests_array[i].ClashResults[j].Item2, "요소", "IfcClass");
                        if (string.IsNullOrEmpty(temp[6])) {
                            temp[6] = Getinfo(tests_array[i].ClashResults[j].Item2.FindFirstObjectAncestor(), "요소", "IfcClass");
                        }
                        //간섭 유형
                        temp[7] = tests_array[i].testType.ToString();
                        if (tests_array[i].ClashResults[j].distance< 0) //간섭거리가 음수인 경우 직접충돌로 Hard로 표시
                            temp[7] = "Hard";
                        //간섭지점
                        //xyz넣기 csv는 ,를 기준으로 항을 나누기 때문에 ""으로 묶어 ,로 항을 나누지 않게 함
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(("\""));
                        stringBuilder.Append((tests_array[i].ClashResults[j].CenterPt[0] * offset).ToString("F3"));//난 이 F3때문에 고생을 좀 했어
                        stringBuilder.Append(",");
                        stringBuilder.Append((tests_array[i].ClashResults[j].CenterPt[1] * offset).ToString("F3"));//처음에는 N3를 썻는데 N은 천자리 콤마를 찍더라...
                        stringBuilder.Append(",");
                        stringBuilder.Append((tests_array[i].ClashResults[j].CenterPt[2] * offset).ToString("F3"));
                        stringBuilder.Append(("\""));
                        temp[8] = stringBuilder.ToString();
                        //간섭거리
                        temp[9] = (tests_array[i].ClashResults[j].distance * offset).ToString("F3");

                        #region 부모의 IfcClass가 CurtainWall이면 자식의 IfcClass도 CurtainWall로 변경
                        //이 부분은 getElement와 같이 수정되어야 함
                        switch (temp[3]) {
                            case "IfcMember":
                            case "IfcPlate":
                                if (Getinfo(tests_array[i].ClashResults[j].Item1.FindFirstObjectAncestor(), "요소", "IfcClass").Equals("IfcCurtainWall")) {
                                    temp[3] = Getinfo(tests_array[i].ClashResults[j].Item1.FindFirstObjectAncestor(), "요소", "IfcClass");
                                }
                                break;
                        }
                        switch (temp[6]) {
                            case "IfcMember":
                            case "IfcPlate":
                                if (Getinfo(tests_array[i].ClashResults[j].Item2.FindFirstObjectAncestor(), "요소", "IfcClass").Equals("IfcCurtainWall")) {
                                    temp[6] = Getinfo(tests_array[i].ClashResults[j].Item2.FindFirstObjectAncestor(), "요소", "IfcClass");
                                }
                                break;
                        }
                        #endregion

                        #region mep부재 한정 ifcSystem 출력
                        //item1
                        try {
                            if (temp[1].Contains("COMM") || temp[1].Contains("ELEC") || temp[1].Contains("FIRE") || temp[1].Contains("MECH")) {
                                temp[10] = tests_array[i].ClashResults[j].IfcSystem1;
                            }
                            else {
                                temp[10] = "";
                            }
                        }
                        catch {
                            temp[10] = "";
                        }
                        //item2
                        try {
                            if (temp[4].Contains("COMM") || temp[4].Contains("ELEC") || temp[4].Contains("FIRE") || temp[4].Contains("MECH")) {
                                temp[11] = tests_array[i].ClashResults[j].IfcSystem2;
                            }
                            else {
                                temp[11] = "";
                            }
                        }
                        catch {
                            temp[11] = "";
                        }
                        #endregion

                        #region 중복제거 : 최초 등장 간섭 외 제거(간섭거리가 가장 작은 간섭 외 제거)
                        //CurtainWall은 CurtainWall끼리만 비교
                        StringBuilder sb_multi = new StringBuilder();
                        sb_multi.Append(temp[1]).Append(temp[2]).Append(temp[4]).Append(temp[5]);

                        StringBuilder sb_iscur = new StringBuilder();
                        sb_iscur.Append(temp[3]).Append(temp[6]);

                        if (sb_iscur.ToString().Contains("IfcCurtainWall")) {
                            if (MultiObjects_Cur.Contains(sb_multi.ToString())) {
                                if (form_setting.export_UselessClash) {
                                    List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                    List_uselessReasons2.Add("동일 간섭 존재(CurtainWall)");
                                    continue;
                                }
                            }
                            else {
                                //csvData.Add(temp);
                                MultiObjects_Cur.Add(sb_multi.ToString());
                            }
                        }
                        else {
                            if (MultiObjects.Contains(sb_multi.ToString())) {
                                if (form_setting.export_UselessClash) {
                                    List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                    List_uselessReasons2.Add("동일 간섭 존재");
                                    continue;
                                }
                            }
                            else {
                                //csvData.Add(temp);
                                MultiObjects.Add(sb_multi.ToString());
                            }
                        }
                        #endregion

                        #region str-mep가 아닌 간섭에서 distance가 0이하 ~ -0.01m초과인 간섭 제외
                        double dis = tests_array[i].ClashResults[j].distance;
                        if (temp[1].Contains("Str")) {
                            // str-str 간섭
                            if (temp[4].Contains("Str")) {
                                if(Math.Round(dis,3) <= 0 && dis > -10) {
                                    if (dis == 0) {
                                        StringBuilder _sb = new StringBuilder();
                                        _sb.Append(temp[2]).Append(temp[5]);
                                        //fp.UpdateLog($"    {tests_array[i].ClashResults[j].DisplayName} {_sb.ToString()}");
                                        if (lst_StrDupl.Contains(_sb.ToString())) { // Duplicate에 존재 시 제거하지 않음
                                            temp[7] = "Hard";
                                        }
                                        else {
                                            List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                            List_uselessReasons2.Add("거리 0 간섭");
                                            continue;
                                        }
                                    }
                                    else {
                                        List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                        List_uselessReasons2.Add("공차 내 간섭");
                                        continue;
                                    }
                                }
                            }
                        }
                        else {
                            if (temp[1] == "Arch" && temp[4] == "Arch") {
                                if (Math.Round(dis, 3) <= 0 && dis > -10) {
                                    if (dis == 0) {
                                        StringBuilder _sb = new StringBuilder();
                                        _sb.Append(temp[2]).Append(temp[5]);
                                        //fp.UpdateLog($"    {tests_array[i].ClashResults[j].DisplayName} {_sb.ToString()}");
                                        if (lst_ArchDupl.Contains(_sb.ToString())) {// Duplicate에 존재 시 제거하지 않음
                                            temp[7] = "Hard";
                                        }
                                        else {
                                            List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                            List_uselessReasons2.Add("거리 0 간섭");
                                            continue;
                                        }
                                    }
                                    else {
                                        List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                        List_uselessReasons2.Add("공차 내 간섭");
                                        continue;
                                    }
                                }
                            }
                            else {
                                if (Math.Round(dis, 3) <= 0 && dis > -10) {
                                    if (dis == 0) {
                                        List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                        List_uselessReasons2.Add("거리 0 간섭");
                                        continue;
                                    }
                                    else {
                                        List_uselessClashes2.Add(tests_array[i].ClashResults[j]);
                                        List_uselessReasons2.Add("공차 내 간섭");
                                        continue;
                                    }
                                }
                            }
                        }

                        //모든 조건을 만족하는 간섭항목을 리스트에 추가
                        csvData.Add(temp);

                        // hard일때만 이미지로 추출할 리스트에 추가
                        if (temp[7] == "Hard")
                            clashNumList.Add(temp[0]);
                        #endregion
                    }

                    // dictionary에 Key: 테스트의 이름(ex) Arch-Arch), value: 간섭 항목의 이름을 담고있는 list 
                    tests_dict.Add(tests_array[i].DisplayName, clashNumList);

                    if (form_setting.export_AllinOne) {
                        sb.Append(SaveCSVFile(tests_array[i].DisplayName, csvData)); //개별 파일로 저장 후, 해당 내용을 All_in_One에 추가
                    }
                    else {
                        SaveCSVFile(tests_array[i].DisplayName, csvData);
                    }
                    form_log.UpdateLog($"    결과 쓰기 종료 : {tests_array[i].DisplayName}");
                }

                

                string json = JsonConvert.SerializeObject(tests_dict, Formatting.Indented);
                File.WriteAllText(@"c:\objectinfo\tests_dict.json", json);


                form_log.UpdateLog("결과 파일 생성 완료");
                #endregion
                if (form_setting.export_UselessClash) {
                    ExportUselessClashes();
                }
                #region All_in_One 파일로 내보내기
                if (form_setting.export_AllinOne) {
                    form_log.UpdateLog("파일 출력 시작 : All in One");
                    string filepath = @"C:/objectinfo";
                    DirectoryInfo dir = new DirectoryInfo(filepath);
                    if (!dir.Exists) {
                        dir.Create();
                    }

                    string _filepath = filepath + "/All_in_One.csv";
                    ClashFile = _filepath;

                    FileInfo file = new FileInfo(_filepath);
                    if (file.Exists) {
                        file.Delete();
                    }

                    Stream fileStream = new FileStream(_filepath, FileMode.CreateNew, FileAccess.Write);
                    StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
                    outStream.WriteLine(sb);
                    outStream.Close();
                    form_log.UpdateLog("파일 출력 완료 : AllinOne");
                }
                #endregion
                #endregion
            }
            catch (Exception ex) {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                form_log.UpdateLog(ex.ToString());
            }
            form_log.UpdateLog("테스트 결과 출력 완료");
        }

        /// <summary>
        /// 'data'를 CSV로 저장
        /// </summary>
        /// <param name="name"> 파일 이름</param>
        /// <param name="data"> 저장할 데이터</param>
        StringBuilder SaveCSVFile(string name, List<string[]> data) {
            //form_log.UpdateLog($"    생성 시작 : {name}.csv");
            string[][] output = new string[data.Count][];
            for (int i = 0; i < output.Length; i++) {
                output[i] = data[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                sb.AppendLine(string.Join(delimiter, output[i]));
            }

            string filepath = @"C:/objectinfo";
            DirectoryInfo dir = new DirectoryInfo(filepath);
            if (!dir.Exists) {
                dir.Create();
            }

            string _filepath = filepath + "/" + name + ".csv";

            FileInfo file = new FileInfo(_filepath);
            if (file.Exists) {
                file.Delete();
            }

            Stream fileStream = new FileStream(_filepath, FileMode.CreateNew, FileAccess.Write);
            StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
            outStream.WriteLine(sb);
            outStream.Close();

            //form_log.UpdateLog($"    생성 완료 : {name}.csv");
            return sb;
        }

        /// <summary>
        /// 해당 오브젝트의 [IfcGUID]가져오기, IfcRailing, IfcMember, IfcStariFlight일 경우 부모의 GUID를 반환
        /// </summary>
        /// <param name="item"> IfcGUID를 찾고자 하는 오브젝트</param>
        /// <returns> IfcGUID </returns>
        string getElementID(ModelItem item) {
            try {
                // CurtainWall의 자식일 경우 IfcClass를 CurtainWall로 변경
                switch (Getinfo(item, "요소", "IfcClass")) {
                    case "IfcMember":
                    case "IfcPlate":
                        if (Getinfo(item.FindFirstObjectAncestor(), "요소", "IfcClass").Equals("IfcCurtainWall")) {
                            return getElementID(item.FindFirstObjectAncestor());
                        }
                        break;
                }
                //카테고리 이름으로 찾기
                //findcategorybyname이 null을 반환
                DataProperty oDP = item.PropertyCategories.FindCategoryByName("LcRevitData_Element")?.Properties.FindPropertyByDisplayName("IfcGUID");
                if (oDP == null)
                    //카테고리의 DisplayName으로 찾기
                    oDP = item.PropertyCategories.FindCategoryByDisplayName("요소")?.Properties.FindPropertyByDisplayName("IfcGUID");
                if (oDP != null)
                    //찾음
                    return oDP.Value.ToDisplayString();
            }
            catch {
                //item 내에서 [IfcGUID]를 찾지 못했을 때
                try {
                    //item의 첫번째 부모에서 [IfcGUID] 찾기
                    DataProperty oDP = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByName("LcRevitData_Element").Properties.FindPropertyByDisplayName("IfcGUID");
                    if (oDP == null)
                        oDP = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName("요소").Properties.FindPropertyByDisplayName("IfcGUID");
                    if (oDP != null) {
                        if (form_setting.show_fromparent) {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("fromparent_");
                            sb.Append(oDP.Value.ToDisplayString());
                            return sb.ToString();
                        }
                        else {
                            return oDP.Value.ToDisplayString();
                        }
                    }
                }
                catch {
                    //부모에서 못찾음
                    return "[                  ]";
                }
            }
            //결국 못찾음
            return "[                  ]";
        }


        //IfcSystem 동일 mep부재 리스트
        List<IfcSystemData> List_sameIfcSystem = new List<IfcSystemData>();
        /// <summary>
        /// IfcSystem이 같은 MEP부재를 모아서 출력
        /// </summary>
        void ExportSameIfcSystem() {
            form_log.UpdateLog($"파일 출력 시작 : SameIfcSystem");
            StringBuilder sb = new StringBuilder();
            if (List_sameIfcSystem.Count == 0) {
                sb.Append("no same ifcsystem");
            }
            else {
                sb.AppendLine($"NO, Item1, Item2, Distance, System1, System2");
                for (int i = 0; i < List_sameIfcSystem.Count; i++) {
                    sb.AppendLine($"{List_sameIfcSystem[i].result.DisplayName},{getElementID(List_sameIfcSystem[i].result.Item1)},{getElementID(List_sameIfcSystem[i].result.Item2)},{(List_sameIfcSystem[i].result.Distance * offset).ToString("F3")},{List_sameIfcSystem[i].ifc1},{List_sameIfcSystem[i].ifc2}");
                }
            }
            string filepath = @"C:/objectinfo";
            DirectoryInfo dir = new DirectoryInfo(filepath);
            if (!dir.Exists) {
                dir.Create();
            }

            string _filepath = filepath + "/" + "SameIfcSystem" + ".csv";

            FileInfo file = new FileInfo(_filepath);
            if (file.Exists) {
                file.Delete();
            }

            Stream fileStream = new FileStream(_filepath, FileMode.CreateNew, FileAccess.Write);
            StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
            outStream.WriteLine(sb);
            outStream.Close();
            List_sameIfcSystem.Clear();
            form_log.UpdateLog($"파일 출력 완료 : SameIfcSystem");
        }

        /// <summary>
        /// Wall의 높이를 이용해서 공간의 최대 높이를 구함
        /// </summary>
        string InfoFile = "";
        void GetSpaceHeights() {
            form_log.UpdateLog("층간 높이 측정 시작");
            // 현재 문서 가져오기
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            // 모델 안에 있는 모든 오브젝트 집합
            ModelItemCollection itemCollection = new ModelItemCollection();
            // 불러와진 모델(파일) 집합
            DocumentModels models = doc.Models;

            //모델이 없음
            if (models.Count == 0) {
                ShowMessage("No Loaded Model");
                return;
            }

            //키 : 도면층, 값 : 같은 도면층에 존재하는 IfcWall들의 높이 중 최대값
            Dictionary<string, double> heights = new Dictionary<string, double>();

            //오브젝트 정보 임시 저장
            List<string[]> props = new List<string[]>();
            //파일 번호
            //int index = 0;
            //불러와진 모델들의 최하위 오브젝트를 하나씩 콜랙션에 넣기
            for (int i = 0; i < models.Count; i++) {
                if (models[i].FileName.ToUpper().Contains("COMM") // MEP는 대상에서 제외
                    || models[i].FileName.ToUpper().Contains("ELEC")
                    || models[i].FileName.ToUpper().Contains("FIRE")
                    || models[i].FileName.ToUpper().Contains("MECH")) continue;
                itemCollection.AddRange(ItemsFromRoot(models[i]));
            }

            #region 불필요 오브젝트 제거
            int useless = 0;
            for (int i = 0; i < itemCollection.Count; i++) {
                string type = Getinfo(itemCollection[i], "항목", "유형");
                //ifc파일에만 존재하는 IfcDistributionPort제거(레빗의 화살표 모양)
                if (type.Equals("IfcDistributionPort")) {
                    useless++;
                    itemCollection.Remove(itemCollection[i]);
                    continue;
                }
                if (type.Equals("IfcDistributionSystem")) {
                    useless++;
                    itemCollection.Remove(itemCollection[i]);
                    continue;
                }
                if (type.Equals("IfcValve")) {
                    useless++;
                    itemCollection.Remove(itemCollection[i]);
                    continue;
                }
                if (type.Equals("IfcPipeFitting")) {
                    useless++;
                    itemCollection.Remove(itemCollection[i]);
                    continue;
                }
            }
            #endregion

            //모든 부재를 돌면서 도면층으로 딕셔너리 구성
            form_log.UpdateLog("    딕셔너리 생성 시작");
            ModelItem item = null;
            for (int i = 0; i < itemCollection.Count; i++) {
                item = itemCollection[i];

                //guid, ifcclass, ifcname, ifcspatialcontainer
                try {
                    string[] temp = new string[3];

                    temp[0] = getElementID(item);

                    temp[1] = Getinfo(item, "요소", "IfcClass");
                    if (temp[1].Equals("IfcDistributionPort")) continue;
                    if (temp[1].Equals("IfcDistributionSystem")) continue;
                    if (temp[1].Equals("IfcOpeningElement")) continue;

                    temp[2] = Getinfo(item, "요소", "IfcSpatialContainer");
                    if (string.IsNullOrEmpty(temp[2])) {
                        temp[2] = Getinfo(item, "항목", "도면층");
                    }
                    //기본값은 0
                    if (!string.IsNullOrEmpty(temp[2]) && !heights.ContainsKey(temp[2])) {
                        heights.Add(temp[2], 0f); 
                    }

                    props.Add(temp);
                }
                catch (Exception ex) {
                    //ShowMessage(ex.ToString());
                    form_log.UpdateLog(ex.ToString());
                }
            }
            form_log.UpdateLog("    딕셔너리 생성 완료");
            
            //각 도면층 별 최대값 찾기
            Dictionary<string, double> newHeights = new Dictionary<string, double>();
            form_log.UpdateLog("    층간 최대값 찾기 시작");
            try {
                foreach (var level in heights.Keys) {
                    //해당 도면층에 존재하는 모든 높이 값
                    HashSet<double> height_walls = new HashSet<double>();
                    for (int i = 0; i < itemCollection.Count; i++) {
                        if (!string.IsNullOrEmpty(Getinfo(itemCollection[i], "요소", "IfcSpatialContainer")) && string.Compare(Getinfo(itemCollection[i], "요소", "IfcSpatialContainer"), level) == 0) {
                            if (!string.IsNullOrEmpty(Getinfo(itemCollection[i], "요소", "IfcSpatialContainer")) && string.Compare(Getinfo(itemCollection[i], "요소", "IfcClass"), "IfcWall") == 0) {
                                try {
                                    double h = double.Parse(Getinfo(itemCollection[i], "Constraints", "Unconnected Height").Split(':')[1]); // : 뒤에 있는 실수값만 가져오기

                                    double offset = 0;
                                    string s_offset = Getinfo(itemCollection[i], "Constraints", "Base Offset");
                                    if (!string.IsNullOrEmpty(s_offset)) {
                                        offset = double.Parse(s_offset.Split(':')[1]);
                                    }
                                    // 절댓값(높이 + 보정값)
                                    height_walls.Add(Math.Abs(h + offset));
                                }
                                catch (Exception ex) {
                                    form_log.UpdateLog(ex.ToString());
                                    //ShowMessage(ex.ToString());
                                    return;
                                }
                            }
                        }
                    }
                    if (height_walls.Count != 0) {
                        //도면층 높이값 중 최대를 도면층의 값으로 함
                        newHeights.Add(level, height_walls.Max());
                    }
                }
            }
            catch (Exception ex) {
                form_log.UpdateLog(ex.ToString());
                //ShowMessage(ex.ToString());
            }

            //각 레벨의 높이 값 대입
            List<string[]> newprops = new List<string[]>();
            try {
                for (int i = 0; i < props.Count; i++) {
                    if (props[i] != null) {
                        if (newHeights.ContainsKey(props[i][2])) {
                            props[i][2] = (newHeights[props[i][2]] * offset).ToString("F3");
                            newprops.Add(props[i]);
                        }
                    }
                }
            }
            catch (Exception e) {
                form_log.UpdateLog(e.ToString());
                //ShowMessage(e.ToString());
            }
            form_log.UpdateLog("    층간 최대값 찾기 완료");

            //IfcSpace만 필터링
            List<string[]> ifcspace = new List<string[]>();
            for (int i = 0; i < newprops.Count; i++) {
                if (string.Compare(newprops[i][1], "IfcSpace") == 0) {
                    ifcspace.Add(newprops[i]);
                }
            }

            //결과 저장
            #region 헤더부분
            string[] header2 = new string[3];
            header2[0] = "Guid";
            header2[1] = "Type";
            header2[2] = "Level_height";
            ifcspace.Insert(0, header2);
            #endregion
            SaveCSVFile("Properties", ifcspace);
            InfoFile = "Properties.csv";

            form_log.UpdateLog("층간 높이 측정 완료");
        }

        /// <summary>
        /// 'model'의 최하위 오브젝트로 분리
        /// </summary>
        /// <param name="model"> 오브젝트 단위로 분리할 모델</param>
        /// <returns></returns>
        public IEnumerable<ModelItem> ItemsFromRoot(Model model) {
            // collect all descendants geometric items from a model 
            return model.RootItem.Descendants.Where(x => x.HasGeometry);
        }

        /// <summary>
        /// 'item'의 'category'에 있는 'property'의 값을 반환
        /// </summary>
        /// <param name="item">부재</param>
        /// <param name="category">카테고리의 DisplayName</param>
        /// <param name="property">속성의 DisplayName</param>
        /// <returns></returns>
        string Getinfo(ModelItem item, string category, string property) {
            string info = "";
            try {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString();
            }
            catch {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString();
            }
            if (string.IsNullOrEmpty(info)) {
                try {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString();
                }
                catch {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString();
                }
            }
            return info;
        }
        
        /// <summary>
        /// 지정된 원본 디렉터리에서 지정된 대상 디렉터리로 모든 파일과 하위 디렉터리를 복사합니다.
        /// </summary>
        /// <param name="sourceDirName">파일을 복사할 원본 디렉터리의 경로입니다.</param>
        /// <param name="destDirName">파일을 복사할 대상 디렉터리의 경로입니다.</param>
        /// <param name="copySubDirs">하위 디렉터리를 재귀적으로 복사하려면 true, 그렇지 않으면 false입니다.</param>
        /// <exception cref="DirectoryNotFoundException">원본 디렉터리가 존재하지 않거나 찾을 수 없는 경우 발생합니다.</exception>
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// compressed.zip파일 생성
        /// </summary>
        /// <param name="clashFile">C:\objectinfo\All_in_One.csv</param>
        /// <param name="infoFile">C:\objectinfo\Properties.csv</param>
        void MakeZipFile(string clashFile = null, string infoFile = null) {
            form_log.UpdateLog("압축파일 만들기 시작");
            try {
                //압축할 폴더 만들기
                string newdirpath = @"C:/objectinfo/compressed";
                if (File.Exists(newdirpath))
                    File.Delete(newdirpath);
                System.IO.Directory.CreateDirectory(newdirpath);

                //폴더에 파일 추가하기
                string filepath = @"C:/objectinfo";
                string modelpath = @"C:/models";
                string resultImagePath = @"C:/objectinfo/ResultImage";
                //결과파일 옮기기
                if (string.IsNullOrEmpty(clashFile)) ShowMessage("No Clash Result File");
                else File.Copy(clashFile, clashFile.Replace(filepath, newdirpath), true);
                //정보파일 옮기기
                if (string.IsNullOrEmpty(infoFile)) ShowMessage("No Object Infomation File");
                else File.Copy(System.IO.Path.Combine(filepath, infoFile), System.IO.Path.Combine(newdirpath, infoFile), true);
                //모델파일 옮기기
                string[] files = Directory.GetFiles(modelpath);
                for (int i = 0; i < files.Length; i++) {
                    if (files[i].Contains(".ifc")) {
                        if (files[i].Contains(".ifc."))
                            continue;
                        if (files[i].Contains("Arch")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "Arch.ifc"), true);
                        }
                        if (files[i].Contains("Str")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "Str.ifc"), true);
                        }
                        if (files[i].Contains("COMM")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "COMM.ifc"), true);
                        }
                        if (files[i].Contains("ELEC")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "ELEC.ifc"), true);
                        }
                        if (files[i].Contains("FIRE")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "FIRE.ifc"), true);
                        }
                        if (files[i].Contains("MECH")) {
                            File.Copy(files[i], Path.Combine(newdirpath, "MECH.ifc"), true);
                        }
                        //File.Copy(files[i], files[i].Replace(modelpath, newdirpath), true);
                    }
                }
                //이미지파일 옮기기
                if (form_setting.Save_image)
                {
                    if (Directory.Exists(resultImagePath)) DirectoryCopy(resultImagePath, 
                        Path.Combine(newdirpath, "ResultImage"), true);
                    else ShowMessage("No Result Image Folder");
                }

                //사용한 모델 정보 텍스트 파일
                using (StreamWriter writer = new StreamWriter(Path.Combine(newdirpath, "used_model.txt")))
                {
                    foreach (string model_name in ifcLoader.usedmodel)
                    {
                        writer.WriteLine(model_name);
                    }
                }

                //폴더 압축하기
                string zippath = @"C:/objectinfo/compressed.zip";
                if (File.Exists(zippath))
                    File.Delete(zippath);
                ZipFile.CreateFromDirectory(newdirpath, zippath);
            }
            catch (Exception ex) {
                form_log.UpdateLog(ex.ToString());
                //ShowMessage(ex.Message);
            }
            form_log.UpdateLog("압축파일 만들기 완료");
        }

        //서버로 보내기(http)
        void SendtoServer(string filepath) {
            form_log.UpdateLog("서버로 전송 시작");
            try {
                string serverIP = "http://117.17.196.59:3116/upload";
                ExtendedWebClient webClient = new ExtendedWebClient();
                //webClient.Timeout = Timeout.Infinite;
                webClient.AllowWriteStreamBuffering = false;
                
                byte[] re = webClient.UploadFile(serverIP,filepath);
                
                string response = webClient.Encoding.GetString(re);
                ShowMessage(response);
            }
            catch (Exception e) {
                form_log.UpdateLog(e.ToString());
                //ShowMessage(e.ToString());
            }
            
            form_log.UpdateLog("서버로 전송 완료");
        }

        List<ClashResult> List_uselessClashes = new List<ClashResult>();
        List<string> List_uselessReasons = new List<string>();
        List<ClashResultCls> List_uselessClashes2 = new List<ClashResultCls>();
        List<string> List_uselessReasons2 = new List<string>();
        /// <summary>
        /// [디버그용]
        /// 'test'의 간섭 검토 결과 중 무의미한 결과 export
        /// </summary>
        /// <param name="test"></param>
        void ExportUselessClashes() {
            form_log.UpdateLog("파일 출력 시작 : UselessClashes");
            if (List_uselessClashes.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("Useless Clashes [").Append(List_uselessClashes.Count()).Append("]\n");
            sb.AppendLine("간섭번호,NameSpace1,GUID1,Class1,NameSpace2,GUID2,Class2,거리,제외사유");
            for (int k = 0; k < List_uselessClashes.Count; k++) {
                var i = List_uselessClashes[k];
                var j = List_uselessReasons[k];
                sb.Append(i.DisplayName.ToString()).Append(",").Append(Getinfo(i.Item1, "항목", "소스 파일").Replace(".ifc", "").ToUpper()).Append(",").Append(getElementID(i.Item1)).Append(",").Append(Getinfo(i.Item1, "요소", "IfcClass")).Append(",").Append(Getinfo(i.Item2, "항목", "소스 파일").Replace(".ifc", "").ToUpper()).Append(",").Append(getElementID(i.Item2)).Append(",").Append(Getinfo(i.Item2, "요소", "IfcClass")).Append(",").Append((i.Distance * offset).ToString("F3")).Append(",").Append(j);
                sb.Append("\n");
            }
            for (int k = 0; k < List_uselessClashes2.Count; k++) {
                var i = List_uselessClashes2[k];
                var j = List_uselessReasons2[k];
                sb.Append(i.DisplayName.ToString()).Append(",").Append(i.Namespace1).Append(",").Append(i.path1ID).Append(",").Append(Getinfo(i.Item1, "요소", "IfcClass")).Append(",").Append(i.Namespace2).Append(",").Append(i.path2ID).Append(",").Append(Getinfo(i.Item2, "요소", "IfcClass")).Append(",").Append(i.distance).Append(",").Append(j);
                sb.Append("\n");
            }
            string filepath = @"C:/objectinfo";
            DirectoryInfo dir = new DirectoryInfo(filepath);
            if (!dir.Exists) {
                dir.Create();
                wf.MessageBox.Show("Create Folder");
            }

            string _filepath = filepath + "/" + "Useless Clashes" + ".csv";

            FileInfo file = new FileInfo(_filepath);
            if (file.Exists) {
                file.Delete();
            }

            Stream fileStream = new FileStream(_filepath, FileMode.CreateNew, FileAccess.Write);
            StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
            outStream.WriteLine(sb);
            outStream.Close();
            List_uselessClashes.Clear();
            List_uselessReasons.Clear();
            form_log.UpdateLog("파일 출력 완료 : UselessClashes");
        }

        void ShowMessage(string content) {
            wf.MessageBox.Show(content);
        }
    }
}