using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Api.ComApi;
using System.Security.Claims;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace Integrity_Checker_MEP
{
    
    public class ImageCreator
    {
        // ClashChecker에서 받아오는 이미지 추출할 모든 테스트의 간섭을 담은 dictionary
        Dictionary<string, List<string>> clashResultDict = new Dictionary<string,List<string>>();

        // 너비와 높이 수동 조정
        private int width = 500;
        private int height = 500;

        // 벽 배경을 표시할 것인지 결정
        // true: 표시, false: 표시 안함 -> form_imageOption에서 선택할 수 있게 바꿈
        public bool setBackground = true;

        // 투명도 적용 할 것인지 결정
        // true: 반투명, false: 불투명
        public bool isTransparant = true;

        // 투명도
        public int transparancy;

        // 카메라 배율
        public double setMagnification = 1500.0;
        private form_ImageOption form_imageOption = new form_ImageOption();

        public Dictionary<Guid, List<ModelItem>> guidDictionary = new Dictionary<Guid, List<ModelItem>>();


        /// <summary>
        /// ClashChecker 클래스에서 호출해서 이미지로 추출할 간섭을 clashResultDict에 저장하는 함수 
        /// </summary>
        /// <param name="names">Key: Name of Test(ex: Arch-Arch_Clearance), 
        ///                     Value: List of Clash name(num) (ex: 간섭1)</param>
        public void getResultName(Dictionary<string, List<string>> names)
        {
            clashResultDict = names;
        }
        
        /// <summary>
        /// 현재 불러올 수 있는 나비스웍스 document를 불러오고 
        /// 테스트와 dictionary를 비교하고 받아온 경로로 이미지를 추출하는 함수
        /// Simple(단순)이미지를 뽑아내는 함수.
        /// </summary>
        /// <param name="directoryPath"> 추출될 경로 -> "c:\objectinfo\resultImage" </param>
        public void SimpleCreateAndFillImages(string directoryPath, From_Log log)
        {
            try
            {
                // 현재 나비스에 떠 있는 테스트 결과를 가져온다
                Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                DocumentClash documentClash = doc.GetClash();
                DocumentClashTests oDCT = documentClash.TestsData;

                // 추출되기 원하는 test의 이름
                // 만약 특정한 test만 추출되길 원한다면 주석 해제하고 직접 특정 테스트 이름을 추가한다
                // 대소문자 주의

                //string[] clashtypes = { "Arch-Arch_Duplicate", "Str-Str_Duplicate", "Arch-Arch_Clearance", "Arch-MECH_Clearance", "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance", "MECH-MECH_Clearance" };
                //string[] clashtypes = { "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance" };
                //string[] clashtypes = { "MECH-MECH_Clearance" };

                // 현재 나비스에서 표시된 테스트를 순회하면서 추출할 것 인지 dictionary와 clashtypes 배열과 비교해서 이미지 추출
                foreach (ClashTest test in oDCT.Tests)
                {
                    //if (clashtypes.Contains(test.DisplayName) && clashResultDict.ContainsKey(test.DisplayName))
                    if (clashResultDict.ContainsKey(test.DisplayName))
                    {

                        List<string> resultNames = new List<string>();
                        resultNames = clashResultDict[test.DisplayName];

                        List<ClashResult> outedResults = new List<ClashResult>();
                        RecurseFillResults(test, ref outedResults);
                        if (outedResults != null && outedResults.Count > 0 && !string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                        {

                            foreach (ClashResult r in outedResults)
                            {
                                if (resultNames.Contains(r.DisplayName))
                                {
                                    SimpleCreateAndFillImage(doc, r, directoryPath, test.DisplayName, log);
                                }
                            }
                        }
                    }
                }
                //show all
                doc.Models.ResetAllHidden();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void SimpleCreateAndFillImage(Document doc, ClashResult clResult, string directoryPath, string testName, From_Log log)
        {
            if (clResult != null)
            {
                // item 숨김 초기화
                doc.Models.ResetAllHidden();
                // Get the 2 clashing elements from the ClashResult
                ModelItem item1 = clResult.Item1;
                ModelItem item2 = clResult.Item2;

                // 디렉토리의 파일 수가 19992개 이상이면 추출 멈추기
                string dPath = Path.Combine(@"C:\objectinfo\ResultImage\단순-다각도이미지\", testName);
                if (Directory.Exists(dPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(dPath);
                        if (files.Length >= 19992)
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }

                if (item1 != null && item2 != null)
                {
                    ModelItemCollection items = new ModelItemCollection();
                    items.Add(item1);
                    items.Add(item2);

                    // 추출하고 싶은 특정한 특성들만 추출하는 코드

                     /*string item1class = Getinfo(item1, "요소", "IfcClass");
                     string item2class = Getinfo(item2, "요소", "IfcClass");
                    bool clashImageCondition = item1class.Equals("IfcWall") || item1class.Equals("IfcSlab")
                        || item1class.Equals("IfcPipeSegment") || item1class.Equals("IfcDuctSegment")
                        || item1class.Equals("IfcDuctFitting") || item1class.Equals("IfcPipeFitting")
                        || item1class.Equals("IfcCableSegment") || item1class.Equals("IfcCableFitting")
                        && item2class.Equals("IfcWall") || item2class.Equals("IfcSlab")
                        || item2class.Equals("IfcPipeSegment") || item2class.Equals("IfcDuctSegment")
                        || item2class.Equals("IfcDuctFitting") || item2class.Equals("IfcPipeFitting")
                        || item2class.Equals("IfcCableSegment") || item2class.Equals("IfcCableFitting");

                     if (!clashImageCondition)
                     {
                         return;
                     }*/



                    doc.CurrentSelection.Clear();
                    
                    doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);


                    // 이미지에 보일 item들, 안 보일 item들, 투명하게 보일 item들을 담는 객체 생성
                    ModelItemCollection modelItemsToShow = new ModelItemCollection();
                    ModelItemCollection modelItemsToHide = new ModelItemCollection();
                    ModelItemCollection modelItemToTransparant = new ModelItemCollection();


                    // 간섭된 항목들은 보일 item에 추가
                    foreach (ModelItem item in items)
                    {
                        if(item.DescendantsAndSelf != null)
                            modelItemsToShow.AddRange(item.DescendantsAndSelf);
                    }

                    // 첫번째 부재의 층 정보를 갖고 오기
                    string levelInfo1 = Getinfo(item1, "요소", "IfcSpatialContainer");
                    string levelInfo2 = Getinfo(item1, "Constraints", "Level");
                    string systemInfo1 = Getinfo(item1, "요소", "IfcSystem");
                    string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");

                    // 배경 보이게 설정 했을시에 보일 item에 원하는 유형의 item 추가
                    // IfcWall과 IfcCurtainWall 추가 (대소문자 맞추기)
                    if(setBackground == true)
                    {
                        AddToItemsToShow(doc, new string[] {levelInfo1, levelInfo2 }, 
                            modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] {systemInfo1, systemInfo2});
                        AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 }, 
                            modelItemsToShow, modelItemToTransparant, "IfcCurtainWall", new string[] {systemInfo1, systemInfo2});
                    }

                    // 안 보일 item을 모두 숨기기
                    modelItemsToHide.CopyFrom(modelItemsToShow);
                    doc.CurrentSelection.CopyFrom(modelItemsToShow);
                    modelItemsToHide.Invert(doc); // invert 함수는 현재 선택된 item들을 반전
                    doc.Models.SetHidden(modelItemsToHide, true);


                    doc.CurrentSelection.Clear();

                    // 보이는 부재들 외의 배경을 흰색으로 통일 (주석 처리하면 나비스에서와 동일)
                    doc.SetPlainBackground(Autodesk.Navisworks.Api.Color.White);

                    // 간섭 부재에 색과 투명도 적용 

                    Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
                    Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;
                    
                    // 첫번째 item은 RED, 두번째 item은 GREEN으로 색 적용
                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(1), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(1) }, GREEN);

                    // 간섭 부재 자체는 투명도 적용하지 않기
                    modelItemToTransparant.Remove(items.ElementAtOrDefault(0));
                    modelItemToTransparant.Remove(items.ElementAtOrDefault(1));

                    // Adjust transparancy (false일 경우엔 투명도를 적용하지 않음)
                    if(isTransparant == true)
                    {
                        doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
                    }


                    // 폴더의 경로 설정 및 ReadOnly 해제

                    // 관측점 오른쪽 위에서 바라보는 나비스 기본 옵션 이미지 추출을 원하면 밑의 주석 해제

                    string testsimpleNamePath = Path.Combine(directoryPath, "단순-단순이미지", $"{testName}");
                    string testsideNamePath = Path.Combine(directoryPath, "단순-다각도이미지", $"{testName}");


                    /*DirectoryInfo diSimple = new DirectoryInfo(testsimpleNamePath);

                    if (!diSimple.Exists)
                    {
                        diSimple.Create();
                        var directorySecurity = diSimple.GetAccessControl();
                        var currentUserIdentity = WindowsIdentity.GetCurrent();
                        var fileSystemRule = new FileSystemAccessRule(currentUserIdentity.Name,
                                                                      FileSystemRights.Read,
                                                                      InheritanceFlags.ObjectInherit |
                                                                      InheritanceFlags.ContainerInherit,
                                                                      PropagationFlags.None,
                                                                      AccessControlType.Allow);

                        directorySecurity.AddAccessRule(fileSystemRule);
                        diSimple.SetAccessControl(directorySecurity);
                    }*/

                    DirectoryInfo diSide = new DirectoryInfo(testsideNamePath);

                    if (!diSide.Exists)
                    {
                        diSide.Create();
                        var directorySecurity = diSide.GetAccessControl();
                        var currentUserIdentity = WindowsIdentity.GetCurrent();
                        var fileSystemRule = new FileSystemAccessRule(currentUserIdentity.Name,
                                                                      FileSystemRights.Read,
                                                                      InheritanceFlags.ObjectInherit |
                                                                      InheritanceFlags.ContainerInherit,
                                                                      PropagationFlags.None,
                                                                      AccessControlType.Allow);

                        directorySecurity.AddAccessRule(fileSystemRule);
                        diSide.SetAccessControl(directorySecurity);
                    }

                    // 카메라 위치 조정 및 폴더에 이미지 저장
                    for (int i = 0; i < 12; i++)
                    {
                        const double pi = 3.14159265358979;
                        double newAngle = (i * 30) * (pi / 180);

                        UnitVector3D newAxis = new UnitVector3D(0, 0, 1);
                        Rotation3D newRotation = new Rotation3D(newAxis, newAngle);
                        ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
                        Viewpoint copy = doc.CurrentViewpoint.CreateCopy();

                        // 원근법 무시하려면 주석 해제
                        //copy.Projection = ViewpointProjection.Orthographic;

                        BoundingBox3D box = items.BoundingBox();
                        copy.PivotPoint = box.Center;

                        Rotation3D res = new Rotation3D(newRotation.D * copy.Rotation.A + newRotation.A * copy.Rotation.D + newRotation.B * copy.Rotation.C - newRotation.C * copy.Rotation.B,
                            newRotation.D * copy.Rotation.B + newRotation.B * copy.Rotation.D + newRotation.C * copy.Rotation.A - newRotation.A * copy.Rotation.C,
                            newRotation.D * copy.Rotation.C + newRotation.C * copy.Rotation.D + newRotation.A * copy.Rotation.B - newRotation.B * copy.Rotation.A,
                            newRotation.D * copy.Rotation.D - newRotation.A * copy.Rotation.A - newRotation.B * copy.Rotation.B - newRotation.C * copy.Rotation.C);

                        copy.Rotation = res;
                        copy.ZoomBox(items.BoundingBox());
                        
                        //@@
                        doc.CurrentSelection.Clear();
                        copy.Lighting = 0;

                        doc.CurrentViewpoint.CopyFrom(copy);


                        using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                        {
                            clashImage.Save(Path.Combine(testsideNamePath, $"{item1.InstanceHashCode.ToString()}+{item1.InstanceHashCode.ToString()}_{i+1}.png"), ImageFormat.Png);
                        }

                    }

                    // 주석 해제하면 오른쪽 위에서 바라본 단순이미지도 추출
                    /*((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
                    Viewpoint copy = doc.CurrentViewpoint.CreateCopy();
                    BoundingBox3D box = items.BoundingBox();
                    copy.ZoomBox(box);
                    doc.CurrentViewpoint.CopyFrom(copy);

                    using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                    {
                        clashImage.Save(Path.Combine(testsimpleNamePath, $"{clResult.DisplayName.Substring(2)}.png"), ImageFormat.Png);
                    }*/


                    ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT);

                    // 색 초기화
                    doc.Models.ResetAllPermanentMaterials();


                    //Hide all
                    doc.Models.SetHidden(modelItemsToShow, true);

                    // temp폴더의 파일 용량 삭제
                    deleteTexture();
                }
            }
        }
        
        /// <summary>
        /// 현재 불러올 수 있는 나비스웍스 document를 불러오고 
        /// 테스트와 dictionary를 비교하고 받아온 경로로 이미지를 추출하는 함수
        /// Complex(복합)이미지를 뽑아내는 함수.
        /// </summary>
        /// <param name="directoryPath"> 추출될 경로 -> "c:\objectinfo\resultImage" </param>
        public void ComplexCreateAndFillImages(string directoryPath)
        {
            try
            {
                // 현재 나비스에 떠 있는 테스트 결과를 가져온다
                Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                DocumentClash documentClash = doc.GetClash();
                DocumentClashTests oDCT = documentClash.TestsData;

                // 추출되기 원하는 test의 이름
                // 만약 특정한 test만 추출되길 원한다면 주석 해제하고 직접 특정 테스트 이름을 추가한다
                // 대소문자 주의

                //string[] clashtypes = { "Arch-Arch_Duplicate", "Str-Str_Duplicate", "Arch-Arch_Clearance", "Arch-MECH_Clearance", "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance", "MECH-MECH_Clearance" };
                //string[] clashtypes = { "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance" };
                string[] clashtypes = { "Arch-MECH_Clearance", "Str-ELEC_Clearance", "COMM-FIRE_Clearance" };
                //string[] clashtypes = { "Arch-MECH_Clearance", "Arch-Str_Clearance" };
                //string[] clashtypes = { "Arch-MECH_Clearance", "Str-MECH_Clearance"
                //"Str-MECH_Clearance"
                //};

                // 현재 나비스에서 표시된 테스트를 순회하면서 추출할 것 인지 dictionary와 clashtypes 배열과 비교해서 이미지 추출
                foreach (ClashTest test in oDCT.Tests)
                {
                    if (clashtypes.Contains(test.DisplayName) && clashResultDict.ContainsKey(test.DisplayName))
                    //if (clashResultDict.ContainsKey(test.DisplayName))
                    {

                        List<string> resultNames = new List<string>();
                        resultNames = clashResultDict[test.DisplayName];

                        List<ClashResult> outedResults = new List<ClashResult>();
                        RecurseFillResults(test, ref outedResults);
                        if (outedResults != null && outedResults.Count > 0 && !string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                        {
                            foreach (ClashResult r in outedResults)
                            {
                                if (!(r.Distance > 0))
                                {
                                    if (!guidDictionary.ContainsKey(r.Item1.InstanceGuid))
                                    {
                                        guidDictionary[r.Item1.InstanceGuid] = new List<ModelItem>();
                                    }

                                    guidDictionary[r.Item1.InstanceGuid].Add(r.Item2);
                                }
                            }
                            List<Guid> keysToRemove = new List<Guid>();

                            // Dictionary를 순회하며 value의 개수가 1개인 항목을 수집
                            foreach (var entry in guidDictionary)
                            {
                                if (entry.Value.Count == 1)
                                {
                                    keysToRemove.Add(entry.Key);
                                }
                            }

                            // 수집된 항목을 Dictionary에서 삭제
                            foreach (var key in keysToRemove)
                            {
                                guidDictionary.Remove(key);
                            }

                            foreach (ClashResult r in outedResults)
                            {
                                if (resultNames.Contains(r.DisplayName))
                                {
                                    ComplexCreateAndFillImage(doc, r, directoryPath, test.DisplayName);
                                }
                            }
                        }
                    }
                }
                //show all
                doc.Models.ResetAllHidden();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ComplexCreateAndFillImage(Document doc, ClashResult clResult, string directoryPath, string testName)
        {
            // item 숨김 초기화
            doc.Models.ResetAllHidden();
            // Get the 2 clashing elements from the ClashResult
            ModelItem item1 = clResult.Item1;
            //ModelItem item2 = clResult.Item2;

            // 디렉토리의 파일 수가 99999 이상이면 추출 멈추기
            string dPath = Path.Combine(@"C:\objectinfo\ResultImage\복합-다각도이미지\", testName);
            if (Directory.Exists(dPath))
            {
                try
                {
                    string[] files = Directory.GetFiles(dPath);
                    if (files.Length >= 99999)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            if (item1 != null)
            {
                ModelItemCollection items = new ModelItemCollection();
                items.Add(clResult.Item1);
                //items.Add(clResult.Item2);

                if (guidDictionary.ContainsKey(item1.InstanceGuid))
                {
                    foreach (ModelItem item in guidDictionary[item1.InstanceGuid])
                    {
                        items.Add(item);
                        guidDictionary.Remove(item1.InstanceGuid);
                    }
                }

                // 추출하고 싶은 특정한 특성들만 추출하는 코드

                //string item1class = Getinfo(item1, "요소", "IfcClass");
                //string item2class = Getinfo(item2, "요소", "IfcClass");
                //bool clashImageCondition = item1class.Equals("IfcWall") || item1class.Equals("IfcSlab")
                //    || item1class.Equals("IfcPipeSegment") || item1class.Equals("IfcDuctSegment")
                //    || item1class.Equals("IfcDuctFitting") 
                //    || item1class.Equals("IfcCableSegment") || item1class.Equals("IfcCableFitting")
                //    && item2class.Equals("IfcWall") || item2class.Equals("IfcSlab")
                //    || item2class.Equals("IfcPipeSegment") || item2class.Equals("IfcDuctSegment")
                //    || item2class.Equals("IfcDuctFitting") 
                //    || item2class.Equals("IfcCableSegment") || item2class.Equals("IfcCableFitting");

                string item1class = Getinfo(item1, "요소", "IfcClass");
                //string item2class = Getinfo(item2, "요소", "IfcClass");
                List<String> itemclass = new List<String>();

                if (guidDictionary.ContainsKey(item1.InstanceGuid))
                {
                    foreach (ModelItem item in guidDictionary[item1.InstanceGuid])
                    {
                        itemclass.Add(Getinfo(item, "요소", "IfcClass"));
                    }
                }
                //string item2class = Getinfo(item2, "요소", "IfcClass");
                //bool clashImageCondition = item1class.Equals("IfcWall") || item1class.Equals("IfcSlab")
                //    && item2class.Equals("IfcPipeSegment") || item2class.Equals("IfcDuctSegment")
                //    //|| item2class.Equals("IfcDuctFitting")
                //    ;


                //if (!clashImageCondition)
                 //{
               //      return;
                 //}



                doc.CurrentSelection.Clear();
                
                doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);


                // 이미지에 보일 item들, 안 보일 item들, 투명하게 보일 item들을 담는 객체 생성
                ModelItemCollection modelItemsToShow = new ModelItemCollection();
                ModelItemCollection modelItemsToHide = new ModelItemCollection();
                ModelItemCollection modelItemToTransparant = new ModelItemCollection();


                // 간섭된 항목들은 보일 item에 추가
                foreach (ModelItem item in items)
                {
                    if(item.DescendantsAndSelf != null)
                        modelItemsToShow.AddRange(item.DescendantsAndSelf);
                }

                // 두 번째 부재의 층 정보를 갖고 오기
                string levelInfo1 = Getinfo(item1, "요소", "IfcSpatialContainer");
                string levelInfo2 = Getinfo(item1, "Constraints", "Level");
                string systemInfo1 = Getinfo(item1, "요소", "IfcSystem");
                //string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");
                //string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");
                List<string> systemInfo2 = new List<string>();
                if (guidDictionary.ContainsKey(item1.InstanceGuid))
                {
                    foreach (ModelItem item in guidDictionary[item1.InstanceGuid])
                    {
                        systemInfo2.Add(Getinfo(item, "요소", "IfcSystem"));
                    }
                }
                // 배경 보이게 설정 했을시에 보일 item에 원하는 유형의 item 추가
                // IfcWall과 IfcCurtainWall 추가 (대소문자 맞추기)
                if (setBackground == true)
                {
                    //AddToItemsToShow(doc, new string[] {levelInfo1, levelInfo2 }, 
                    //   modelItemsToShow, modelItemToTransparant, "IfcPipeSegment");
                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 }, 
                    //    modelItemsToShow, modelItemToTransparant, "IfcPipeFitting");
                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    //    modelItemsToShow, modelItemToTransparant, "IfcDuctSegment");
                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    //    modelItemsToShow, modelItemToTransparant, "IfcDuctFitting");
                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    //    modelItemsToShow, modelItemToTransparant, "IfcCableCarrierSegment");
                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    //    modelItemsToShow, modelItemToTransparant, "IfcCableCarrierFitting");
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] { systemInfo1 }.Concat(systemInfo2).ToArray());
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcCurtainWall", new string[] { "a", "b" });
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcSlab", new string[] { "a", "b" });
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcDoor", new string[] { "a", "b" });
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcColumn", new string[] { "a", "b" });
                    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcWindow", new string[] { "a", "b" });

                    //AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    //modelItemsToShow, modelItemToTransparant, "IfcFurniture");
                }

                // 안 보일 item을 모두 숨기기
                modelItemsToHide.CopyFrom(modelItemsToShow);
                doc.CurrentSelection.CopyFrom(modelItemsToShow);
                modelItemsToHide.Invert(doc); // invert 함수는 현재 선택된 item들을 반전
                doc.Models.SetHidden(modelItemsToHide, true);


                doc.CurrentSelection.Clear();

                // 보이는 부재들 외의 배경을 흰색으로 통일 (주석 처리하면 나비스에서와 동일)
                //doc.SetPlainBackground(Autodesk.Navisworks.Api.Color.White);

                // 간섭 부재에 색과 투명도 적용 

                Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
                Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

                // 첫번째 item은 RED, 두번째 item은 GREEN으로 색 적용
                if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                    doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

                for (int i = 1; i < items.Count; i++)
                {
                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(i), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(i) }, GREEN);
                }

                //if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(1), null))
                //    doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(1) }, GREEN);

                // 간섭 부재 자체는 투명도 적용하지 않기
                modelItemToTransparant.Remove(items.ElementAtOrDefault(0));
                for (int i = 1; i < items.Count; i++)
                {
                    modelItemToTransparant.Remove(items.ElementAtOrDefault(i));
                }
                //modelItemToTransparant.Remove(items.ElementAtOrDefault(1));

                // Adjust transparancy (false일 경우엔 투명도를 적용하지 않음)
                if(isTransparant == true)
                {
                    doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
                }


                // 폴더의 경로 설정 및 ReadOnly 해제

                // 관측점 오른쪽 위에서 바라보는 나비스 기본 옵션 이미지 추출을 원하면 밑의 주석 해제

                string testsimpleNamePath = Path.Combine(directoryPath, "복합-단순이미지", $"{testName}");
                string testsideNamePath = Path.Combine(directoryPath, "복합-다각도이미지", $"{testName}");


                /*DirectoryInfo diSimple = new DirectoryInfo(testsimpleNamePath);

                if (!diSimple.Exists)
                {
                    diSimple.Create();
                    var directorySecurity = diSimple.GetAccessControl();
                    var currentUserIdentity = WindowsIdentity.GetCurrent();
                    var fileSystemRule = new FileSystemAccessRule(currentUserIdentity.Name,
                                                                  FileSystemRights.Read,
                                                                  InheritanceFlags.ObjectInherit |
                                                                  InheritanceFlags.ContainerInherit,
                                                                  PropagationFlags.None,
                                                                  AccessControlType.Allow);

                    directorySecurity.AddAccessRule(fileSystemRule);
                    diSimple.SetAccessControl(directorySecurity);
                }*/

                DirectoryInfo diSide = new DirectoryInfo(testsideNamePath);

                if (!diSide.Exists)
                {
                    diSide.Create();
                    var directorySecurity = diSide.GetAccessControl();
                    var currentUserIdentity = WindowsIdentity.GetCurrent();modelItemToTransparant.Remove(items.ElementAtOrDefault(1));
                    var fileSystemRule = new FileSystemAccessRule(currentUserIdentity.Name,
                                                                  FileSystemRights.Read,
                                                                  InheritanceFlags.ObjectInherit |
                                                                  InheritanceFlags.ContainerInherit,
                                                                  PropagationFlags.None,
                                                                  AccessControlType.Allow);

                    directorySecurity.AddAccessRule(fileSystemRule);
                    diSide.SetAccessControl(directorySecurity);
                }
                BoundingBox3D box = items.BoundingBox();
                
                // 카메라 위치 조정 및 폴더에 이미지 저장
                if (items.Count != 1)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i >= 0 && i <= 7)
                        {
                            const double pi = 3.14159265358979;
                            double newAngle = (i * 45) * (pi / 180);

                            UnitVector3D newAxis = new UnitVector3D(0, 0, 1);
                            Rotation3D newRotation = new Rotation3D(newAxis, newAngle);
                            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT);
                            Viewpoint copy = doc.CurrentViewpoint.CreateCopy();
                            copy.Lighting = ViewpointLighting.FullLights;

                            // 원근법 무시하려면 주석 해제s
                            //copy.Projection = ViewpointProjection.Orthographic;

                            // form_imageoption에서 설정한 배율에 따라
                            setMagnification = form_imageOption.magnification;
                            setMagnification = 0;
                            // 새로운 최소 및 최대 포인트를 계산합니다.
                            Point3D newMin = new Point3D(box.Min.X - setMagnification, box.Min.Y - setMagnification, box.Min.Z - setMagnification);
                            Point3D newMax = new Point3D(box.Max.X + setMagnification, box.Max.Y + setMagnification, box.Max.Z + setMagnification);

                            // 새로운 바운딩 박스를 생성합니다.
                            BoundingBox3D expandedBox = new BoundingBox3D(newMin, newMax);
                            //BoundingBox3D box = items.BoundingBox();
                            copy.PivotPoint = expandedBox.Center;

                            Rotation3D res = new Rotation3D(
                                newRotation.D * copy.Rotation.A + newRotation.A * copy.Rotation.D + newRotation.B * copy.Rotation.C - newRotation.C * copy.Rotation.B,
                                newRotation.D * copy.Rotation.B + newRotation.B * copy.Rotation.D + newRotation.C * copy.Rotation.A - newRotation.A * copy.Rotation.C,
                                newRotation.D * copy.Rotation.C + newRotation.C * copy.Rotation.D + newRotation.A * copy.Rotation.B - newRotation.B * copy.Rotation.A,
                                newRotation.D * copy.Rotation.D - newRotation.A * copy.Rotation.A - newRotation.B * copy.Rotation.B - newRotation.C * copy.Rotation.C
                                );

                            copy.Rotation = res;

                            Point3D currentPosition = copy.Position;
                            Point3D newPosition = new Point3D(currentPosition.X, currentPosition.Y, currentPosition.Z);
                            copy.Position = newPosition;

                            copy.ZoomBox(expandedBox);

                            //@@
                            doc.CurrentSelection.Clear();
                            //copy.Lighting = 0;

                            doc.CurrentViewpoint.CopyFrom(copy);
                        }

                        if (i==8)
                        {

                            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eTOP);
                            Viewpoint copy = doc.CurrentViewpoint.CreateCopy();

                            // 원근법 무시하려면 주석 해제
                            //copy.Projection = ViewpointProjection.Orthographic;

                            //BoundingBox3D box = items.BoundingBox();
                            copy.PivotPoint = box.Center;

                            copy.ZoomBox(items.BoundingBox());

                            //@@
                            doc.CurrentSelection.Clear();
                            //copy.Lighting = 0;

                            doc.CurrentViewpoint.CopyFrom(copy);
                        }
                        if (i == 9)
                        {

                            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eBOTTOM);
                            Viewpoint copy = doc.CurrentViewpoint.CreateCopy();

                            // 원근법 무시하려면 주석 해제
                            //copy.Projection = ViewpointProjection.Orthographic;

                            //BoundingBox3D box = items.BoundingBox();
                            copy.PivotPoint = box.Center;

                            copy.ZoomBox(items.BoundingBox());

                            //@@
                            doc.CurrentSelection.Clear();
                            //copy.Lighting = 0;

                            doc.CurrentViewpoint.CopyFrom(copy);
                        }
                                            
                        using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                        {
                            clashImage.Save(Path.Combine(testsideNamePath, $"{clResult.DisplayName.Substring(2)}_{i+1}.png"), ImageFormat.Png);
                        }
                    }
                }

                // 주석 해제하면 오른쪽 위에서 바라본 단순이미지도 추출
                /*((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
                Viewpoint copy = doc.CurrentViewpoint.CreateCopy();
                BoundingBox3D box = items.BoundingBox();
                copy.ZoomBox(box);
                doc.CurrentViewpoint.CopyFrom(copy);

                using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                {
                    clashImage.Save(Path.Combine(testsimpleNamePath, $"{clResult.DisplayName.Substring(2)}.png"), ImageFormat.Png);
                }*/


                ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);

                // 색 초기화
                doc.Models.ResetAllPermanentMaterials();


                //Hide all
                doc.Models.SetHidden(modelItemsToShow, true);

                // temp폴더의 파일 용량 삭제
                deleteTexture();
            }
            ////
        }

        private void GetAllItems(IEnumerable<ModelItem> items, ref ModelItemCollection allItems)
        {
            if (items != null)
            {
                foreach (ModelItem item in items)
                {
                    allItems.Add(item);
                    if (item.Children != null)
                    {
                        GetAllItems(item.Children, ref allItems);
                    }

                }
            }
        }

        private void RecurseFillResults(GroupItem group, ref List<ClashResult> outedResults)
        {

            foreach (SavedItem child in group.Children)
            {
                /* If we only wanted to access first-level children
                 * without reference to whether they were groups or results
                 * then we could:
                 *
                 * // access groups and results via shared interface
                 * IClashResult result = child as IClashResult;
                 * 
                 * operate on that and not recurse further. */

                // GroupItem is the base-class of ClashResultGroup which defines
                // group-like behaviour, if we needed to access ay ClashResultGroup properties
                // we could cast to that equivalently... 
                GroupItem child_group = child as GroupItem;

                // is this a group?
                if (child_group != null)
                {
                    // operate on the group's children
                    RecurseFillResults(child_group, ref outedResults);
                }
                else
                {
                    // Not a group, so must be a result.
                    ClashResult result = child as ClashResult;
                    if (outedResults != null)
                    {
                        outedResults.Add(result);
                    }
                }
            }
        }

        /// <summary>
        /// 해당 객체의 특성 정보를 갖고오는 함수
        /// </summary>
        /// <param name="item"> 특성 정보를 가져올 부재 </param>
        /// <param name="category"> 카테고리 정보 ex)요소 </param>
        /// <param name="property"> 프로퍼티 정보 ex)IfcClass </param>
        /// <returns> 프로퍼티 정보의 값 ex)IfcWall </returns>
        string Getinfo(ModelItem item, string category, string property)
        {
            string info = "";
            try
            {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString();
            }
            catch
            {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString();
            }
            if (string.IsNullOrEmpty(info))
            {
                try
                {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToDisplayString();
                }
                catch
                {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.FindPropertyByDisplayName(property)?.Value.ToString();
                }
            }

            return info;
        }

        /// <summary>
        /// 배경으로 보일 특정 부재들을 추가한다
        /// 이때 첫번째 부재의 층 정보를 가져와서 같은 층인 것들만 표시하도록 검색을 진행한다
        /// </summary>
        /// <param name="doc"> 현재 작업중인 나비스웍스 document </param>
        /// <param name="levelInfo"> 층 정보를 담고있는 배열 </param>
        /// <param name="modelItemsToShow"> 표시될 item을 담고있는 ModelItemCollection </param>
        /// <param name="modelItemToTransparant"> 투명하게 표시될 item을 담고있는 ModelItemCollection <param>
        /// <param name="className"> 표시할 IfcClass의 이름 </param>
        private void AddToItemsToShow(Document doc, string[] levelInfo, ModelItemCollection modelItemsToShow, ModelItemCollection modelItemToTransparant, string className, string[] systeminfo)
        {
            if (levelInfo != null)
            {
                // 검색 객체 생성
                Search search = new Search();
                Search search2 = new Search();
                Search search3 = new Search();
                // 검색 범위 지정
                search.Selection.SelectAll();
                search2.Selection.SelectAll();
                search3.Selection.SelectAll();
                // 검색 조건 생성
                SearchCondition classcondition = SearchCondition.HasPropertyByDisplayName("요소", "IfcClass").EqualValue(new VariantData(className));
                SearchCondition levelcondition1 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSpatialContainer").EqualValue(new VariantData(levelInfo[0]));
                SearchCondition levelcondition2 = SearchCondition.HasPropertyByDisplayName("Constraints", "Base Constraint").EqualValue(new VariantData(levelInfo[1]));
                SearchCondition systemcondition1 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[0]));
                SearchCondition systemcondition2 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[1]));

                // 검색 조건 적용
                List<SearchCondition> oG1 = new List<SearchCondition>();
                List<SearchCondition> oG2 = new List<SearchCondition>();
                List<SearchCondition> oG3 = new List<SearchCondition>();
                List<SearchCondition> oG4 = new List<SearchCondition>();

                oG1.Add(levelcondition1);
                oG1.Add(classcondition);
                oG2.Add(levelcondition2);
                oG2.Add(classcondition);
                oG3.Add(systemcondition1);
                oG4.Add(systemcondition2);

                search.SearchConditions.AddGroup(oG1);
                search.SearchConditions.AddGroup(oG2);
                search2.SearchConditions.AddGroup(oG3);
                search3.SearchConditions.AddGroup(oG4);

                Autodesk.Navisworks.Api.Color BLUE = Autodesk.Navisworks.Api.Color.Blue;
                Autodesk.Navisworks.Api.Color ORANGE = Autodesk.Navisworks.Api.Color.FromByteRGB(255, 128, 64);

                // spatialcontainer에 적힌 level 또는 Constraints의 Base Constraint에 적힌 level로 같은 층의 wall/curtainwall 검색
                ModelItemCollection wallItems = search.FindAll(doc, false);
                foreach (ModelItem item in wallItems)
                {
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                }

                ModelItemCollection mepItems = search2.FindAll(doc, false);
                foreach (ModelItem item in mepItems)
                {
                    doc.Models.OverridePermanentColor(new ModelItem[1] { item }, BLUE);
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                }

                ModelItemCollection mepItems2 = search3.FindAll(doc, false);
                foreach (ModelItem item in mepItems2)
                {
                    doc.Models.OverridePermanentColor(new ModelItem[1] { item }, ORANGE);
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                }
            }
        }

        /// <summary>
        /// 이미지 추출 진행동안 temp 파일에 용량이 너무 커져서 저장공간 부족 오류가 나타나는걸 방지하기 위해 코드상으로 temp파일의 texture를 삭제하는 함수
        /// </summary>
        private void deleteTexture()
        {
            try
            {
                string tempFolderPath = Path.GetTempPath(); // Get temp folder directory
                string ogsFolderPath = Path.Combine(tempFolderPath, "ogs");

                // Get first folder directory from ogs folder
                string[] ogsSubDirectories = Directory.GetDirectories(ogsFolderPath);
                if (ogsSubDirectories.Length > 0)
                {
                    string firstSubDirectory = ogsSubDirectories[0];

                    // Get first folder directory from subdirectory
                    string[] foldersInFirstSubDirectory = Directory.GetDirectories(firstSubDirectory);
                    if (foldersInFirstSubDirectory.Length > 0)
                    {
                        string firstFolderPath = foldersInFirstSubDirectory[0];

                        // For Debugging (Check if directory is temp/ogs/(number)/(number)_TextureCache
                        /*if (pathDebug)
                        {
                            MessageBox.Show(firstFolderPath);
                            pathDebug = false;
                        }*/

                        // Get file size info
                        DirectoryInfo directoryInfo = new DirectoryInfo(firstFolderPath);
                        FileInfo[] files = directoryInfo.GetFiles();    
                        long fileSizeInBytes = 0;
                        foreach (FileInfo file in files)
                        {
                            fileSizeInBytes += file.Length;
                        }
    
                        long fileSizeInGB = fileSizeInBytes / (1024 * 1024 * 1024); // 크기를 기가바이트로 변환

                        // Delete TextureCache folder if size is more than 5GB
                        if (fileSizeInGB > 5)
                        {
                            Directory.Delete(firstFolderPath, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No folder in subdirectory");
                    }
                }
                else
                {
                    MessageBox.Show("No folder in ogs folder");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
