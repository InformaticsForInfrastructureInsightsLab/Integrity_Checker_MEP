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
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using Autodesk.Navisworks.Internal.ApiImplementation;


namespace Integrity_Checker_MEP
{
    using static System.Net.Mime.MediaTypeNames;
    using clashResultDictType = Dictionary<string, List<string>>;
    abstract class ImageCreatorBase
    {
        protected static Mutex mutex = new Mutex();
        protected From_Log form_log;

        protected bool background, transparent;
        protected int transparancy;

        protected int width = 500;
        protected int height = 500;

        bool set_background;
        public abstract void CreateAndFillImage(string path, clashResultDictType clash_result_dict);
        public abstract void image_thread(Document doc, ClashResult clResult, string directoryPath, string testName);

        protected void extract(GroupItem group, ref List<ClashResult> outedResults)
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
                    extract(child_group, ref outedResults);
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
    }

    class ImageCreatorSimple : ImageCreatorBase
    {
        public ImageCreatorSimple(From_Log log, form_ImageOption image_option)
        {
            form_log = log;
            background = image_option.background;
            transparent = image_option.transparant;
            transparancy = image_option.transparancy;
        }

        override public void CreateAndFillImage(string path, clashResultDictType clash_result_dict)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            Parallel.ForEach(oDCT.Tests.Cast<ClashTest>(), (ClashTest test) =>
            {
                // 없으면 패스
                if (!clash_result_dict.ContainsKey(test.DisplayName))
                    return;

                List<ClashResult> outed_results = new List<ClashResult>();
                extract(test, ref outed_results);

                foreach (ClashResult result in outed_results)
                {
                    if (!clash_result_dict[test.DisplayName].Contains(result.DisplayName))
                        return;

                    form_log.Invoke(new Action(() =>
                    {
                        form_log.UpdateLog($"{result.DisplayName} 이미지 추출");
                    }));
                    // 이미지 추출 로직
                }

            });
        }

        public override async void image_thread(Document doc, ClashResult clResult, string directoryPath, string testName)
        {
            if (clResult == null)
                return;

            // 저장경로 확인
            string dPath = Path.Combine(@"C:\objectinfo\ResultImage\다각도이미지\", testName);
            if (Directory.Exists(dPath))
            {
                try
                {
                    string[] files = Directory.GetFiles(dPath);
                    if (files.Length >= 99999) return;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            doc.Models.ResetAllHidden();

            ModelItemCollection items = new ModelItemCollection();
            items.Add(clResult.Item1);
            items.Add(clResult.Item2);

            ModelItem item1 = clResult.Item1;
            ModelItem item2 = clResult.Item2;
            if (item1 == null) return;

            string item1class = Getinfo(item1, "요소", "IfcClass");
            string item2class = Getinfo(item2, "요소", "IfcClass");

            bool clashImageCondition = item1class.Equals("IfcWall") || item1class.Equals("IfcSlab")
                        && item2class.Equals("IfcPipeSegment") || item2class.Equals("IfcDuctSegment");
            if (!clashImageCondition) return;

            doc.CurrentSelection.Clear();
            doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

            ModelItemCollection modelItemsToShow = new ModelItemCollection();
            ModelItemCollection modelItemsToHide = new ModelItemCollection();
            ModelItemCollection modelItemToTransparant = new ModelItemCollection();

            // 간섭된 항목들은 보일 item에 추가
            foreach (ModelItem item in items)
            {
                if (item.DescendantsAndSelf != null)
                    modelItemsToShow.AddRange(item.DescendantsAndSelf);
            }

            // 두 번째 부재의 층 정보를 갖고 오기
            string levelInfo1 = Getinfo(item1, "요소", "IfcSpatialContainer");
            string levelInfo2 = Getinfo(item1, "Constraints", "Level");
            string systemInfo1 = Getinfo(item1, "요소", "IfcSystem");
            string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");

            AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                        modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] { systemInfo1, systemInfo2 });
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

            // 안 보일 item을 모두 숨기기
            modelItemsToHide.CopyFrom(modelItemsToShow);
            doc.CurrentSelection.CopyFrom(modelItemsToShow);
            modelItemsToHide.Invert(doc); // invert 함수는 현재 선택된 item들을 반전
            doc.Models.SetHidden(modelItemsToHide, true);

            doc.CurrentSelection.Clear();

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
            if (transparent)
            {
                doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
            }

            string testsimpleNamePath = Path.Combine(directoryPath, "단순이미지", $"{testName}");
            string testsideNamePath = Path.Combine(directoryPath, "다각도이미지", $"{testName}");

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

            // 이미지 추출 병렬처리
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 12; i++)
            {
                tasks.Add(capture_image(items, clResult, doc, testsideNamePath, i));
            }
            foreach (Task task in tasks)
            {
                await task;
            }

            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);

            // 색 초기화
            doc.Models.ResetAllPermanentMaterials();
            //Hide all
            doc.Models.SetHidden(modelItemsToShow, true);

            form_log.UpdateLog($"{clResult.DisplayName} 이미지 저장 완료");
        }

        private string Getinfo(ModelItem item, string category, string property)
        {
            string info = "";
            try
            {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.
                    FindPropertyByDisplayName(property)?.Value.ToDisplayString();
            }
            catch
            {
                info = item.PropertyCategories.FindCategoryByDisplayName(category)?.Properties.
                    FindPropertyByDisplayName(property)?.Value.ToString();
            }

            if (string.IsNullOrEmpty(info))
            {
                try
                {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.
                        FindPropertyByDisplayName(property)?.Value.ToDisplayString();
                }
                catch
                {
                    info = item.FindFirstObjectAncestor().PropertyCategories.FindCategoryByDisplayName(category)?.Properties.
                        FindPropertyByDisplayName(property)?.Value.ToString();
                }
            }

            return info;
        }

        private void AddToItemsToShow(Document doc, string[] levelInfo, ModelItemCollection modelItemsToShow,
            ModelItemCollection modelItemToTransparant, string className, string[] systeminfo)
        {
            if (levelInfo == null) return;
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

        private async Task capture_image(ModelItemCollection items, ClashResult result, Document doc, string path, int i)
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
                clashImage.Save(Path.Combine(path, $"{result.DisplayName.Substring(2)}_{i + 1}.png"), ImageFormat.Png);
            }

        }

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

        class ImageCreatorComplex : ImageCreatorBase
        {
            public ImageCreatorComplex(From_Log log, form_ImageOption image_option)
            {
                form_log = log;
                background = image_option.background;
                transparent = image_option.transparant;
                transparancy = image_option.transparancy;
            }

            public override void CreateAndFillImage(string path, clashResultDictType clash_result_dict)
            {
                throw new NotImplementedException();
            }

            public override void image_thread(Document doc, ClashResult clResult, string directoryPath, string testName)
            {
                throw new NotImplementedException();
            }
        }
    }
}//namespace Integrity_Checker_MEP