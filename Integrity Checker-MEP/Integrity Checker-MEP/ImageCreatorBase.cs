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
using Autodesk.Navisworks.Api.DocumentParts;
using detail;

namespace detail
{
    class pair<T, U>
    {
        public T first;
        public U second;

        public pair(T first, U second)
        {
            this.first = first;
            this.second = second;
        }
    }
}


namespace Integrity_Checker_MEP
{
    using clashResultDictType = Dictionary<string, List<string>>;
    abstract class ImageCreatorBase
    {
        protected From_Log form_log;

        public static List<pair<string, ClashResult>> outed_results_list = new List<pair<string, ClashResult>>();

        protected int width = 500;
        protected int height = 500;

        public abstract void make_image(ClashResult cl_result, string directoryPath, string testName);

        public static void compare_tests_with_json(clashResultDictType names, ImageCreatorComplex complex)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            Parallel.ForEach(oDCT.Tests.Cast<ClashTest>(), (ClashTest test) =>
            { 
                if (names.ContainsKey(test.DisplayName))
                {
                    List<ClashResult> outedResults = new List<ClashResult>();
                    extract(test, outedResults);

                    Parallel.ForEach(outedResults, (ClashResult r) =>
                    {
                        List<string> results = names[test.DisplayName];
                        if (results.Contains(r.DisplayName))
                            outed_results_list.Add(new pair<string, ClashResult>(test.DisplayName, r));

                        lock (complex.guid_dictionary)
                        {
                            if (!(r.Distance > 0))
                            {
                                if (!complex.guid_dictionary.ContainsKey(r.Item1.InstanceGuid))
                                {
                                    complex.guid_dictionary[r.Item1.InstanceGuid] = new List<ModelItem>();
                                }
                                complex.guid_dictionary[r.Item1.InstanceGuid].Add(r.Item2);
                            }
                        }
                        
                    });
                }
            });
        }

        private static void extract(GroupItem group, List<ClashResult> outedResults)
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
                    extract(child_group, outedResults);
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

        protected string Getinfo(ModelItem item, string category, string property)
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
        protected void AddToItemsToShow(Document doc, string[] levelInfo, ModelItemCollection modelItemsToShow,
            ModelItemCollection modelItemToTransparant, string className, string[] systeminfo)
        {
            if (levelInfo == null) return;

            Search search = new Search();
            Search search2 = new Search();
            Search search3 = new Search();

            Autodesk.Navisworks.Api.Color BLUE = Autodesk.Navisworks.Api.Color.Blue;
            Autodesk.Navisworks.Api.Color ORANGE = Autodesk.Navisworks.Api.Color.FromByteRGB(255, 128, 64);

            Thread t1 = new Thread(() =>
            {
                search.Selection.SelectAll();

                SearchCondition classcondition = SearchCondition.HasPropertyByDisplayName("요소", "IfcClass").EqualValue(new VariantData(className));
                SearchCondition levelcondition1 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSpatialContainer").EqualValue(new VariantData(levelInfo[0]));
                SearchCondition levelcondition2 = SearchCondition.HasPropertyByDisplayName("Constraints", "Base Constraint").EqualValue(new VariantData(levelInfo[1]));

                List<SearchCondition> l1 = new List<SearchCondition>();
                l1.Add(levelcondition1);
                l1.Add(classcondition);
                List<SearchCondition> l2 = new List<SearchCondition>();
                l2.Add(levelcondition2);
                l2.Add(classcondition);

                search.SearchConditions.AddGroup(l1);
                search.SearchConditions.AddGroup(l2);

                // spatialcontainer에 적힌 level 또는 Constraints의 Base Constraint에 적힌 level로 같은 층의 wall/curtainwall 검색
                ModelItemCollection wallItems = search.FindAll(doc, false);
                Parallel.ForEach(wallItems.Cast<ModelItem>(), (ModelItem item) => {
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                });
            });
            Thread t2 = new Thread(() =>
            {
                search2.Selection.SelectAll();
                SearchCondition systemcondition1 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[0]));
                List<SearchCondition> l = new List<SearchCondition>();
                l.Add(systemcondition1);
                search2.SearchConditions.AddGroup(l);

                // spatialcontainer에 적힌 level 또는 Constraints의 Base Constraint에 적힌 level로 같은 층의 wall/curtainwall 검색
                ModelItemCollection mepItems = search2.FindAll(doc, false);
                foreach (ModelItem item in mepItems)
                {
                    lock (doc)
                    {
                        doc.Models.OverridePermanentColor(new ModelItem[1] { item }, BLUE);
                    }
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                }
            });
            Thread t3 = new Thread(() => {
                search3.Selection.SelectAll();
                SearchCondition systemcondition2 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[1]));
                List<SearchCondition> l = new List<SearchCondition>();
                l.Add(systemcondition2);
                search3.SearchConditions.AddGroup(l);

                ModelItemCollection mepItems2 = search3.FindAll(doc, false);
                Parallel.ForEach(mepItems2.Cast<ModelItem>(), (ModelItem item) => {
                    lock (doc)
                    {
                        doc.Models.OverridePermanentColor(new ModelItem[1] { item }, ORANGE);
                    }
                    modelItemsToShow.Add(item);
                    modelItemToTransparant.Add(item);
                });
            });

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();
        }
    }



    class ImageCreatorSimple : ImageCreatorBase
    {
        public ImageCreatorSimple(From_Log log)
        {
            form_log = log;
        }

        public override void make_image(ClashResult cl_result, string directory_path, string test_name)
        {
            if (cl_result == null) return;

            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            // item 숨김 초기화
            doc.Models.ResetAllHidden();
            ModelItem item1 = cl_result.Item1;
            ModelItem item2 = cl_result.Item2;

            if (item1 == null || item2 == null) return;
            string d_path = Path.Combine(@"C:\objectinfo\ResultImage\단순-다각도이미지\", test_name);
            if (Directory.Exists(d_path))
            {
                try
                {
                    string[] files = Directory.GetFiles(d_path);
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

            ModelItemCollection items = new ModelItemCollection();
            items.Add(item1);
            items.Add(item2);

            doc.CurrentSelection.Clear();
            doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

            // 이미지에 보일 item들, 안 보일 item들, 투명하게 보일 item들을 담는 객체 생성
            ModelItemCollection modelItemsToShow = new ModelItemCollection();
            ModelItemCollection modelItemsToHide = new ModelItemCollection();
            ModelItemCollection modelItemToTransparant = new ModelItemCollection();

            // 간섭된 항목들은 보일 item에 추가
            foreach (ModelItem item in items)
            {
                if (item.DescendantsAndSelf != null)
                    modelItemsToShow.AddRange(item.DescendantsAndSelf);
            }

            // 첫번째 부재의 층 정보를 갖고 오기
            string levelInfo1 = Getinfo(item1, "요소", "IfcSpatialContainer");
            string levelInfo2 = Getinfo(item1, "Constraints", "Level");
            string systemInfo1 = Getinfo(item1, "요소", "IfcSystem");
            string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");

            // 배경 보이게 설정 했을시에 보일 item에 원하는 유형의 item 추가
            // IfcWall과 IfcCurtainWall 추가 (대소문자 맞추기)
/*            if (background == true)
            {
                AddToItemsToShow(doc, new string[] {levelInfo1, levelInfo2},
                    modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] {systemInfo1, systemInfo2});
                AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    modelItemsToShow, modelItemToTransparant, "IfcCurtainWall", new string[] { systemInfo1, systemInfo2 });
            }*/

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
/*            if (transparent == true)
            {
                doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
            }*/

            string testsimpleNamePath = Path.Combine(directory_path, "단순-단순이미지", $"{test_name}");
            string testsideNamePath = Path.Combine(directory_path, "단순-다각도이미지", $"{test_name}");

            // 이미지 생성, 저장
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

                doc.CurrentSelection.Clear();
                copy.Lighting = 0;

                doc.CurrentViewpoint.CopyFrom(copy);

                // 폴더 생성 및 쓰기권한 얻기
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

                using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                {
                    string path = Path.Combine(testsideNamePath, $"{items[0].InstanceGuid.ToString()}+{items[1].InstanceGuid.ToString()}_{i + 1}.png");
                    clashImage.Save(path, ImageFormat.Png);
                }
                
            }
            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
            // 색 초기화
            doc.Models.ResetAllPermanentMaterials();
            //Hide all
            doc.Models.SetHidden(modelItemsToShow, true);
        }
    }

    class ImageCreatorComplex : ImageCreatorBase
    {
        public Dictionary<Guid, List<ModelItem>> guid_dictionary;

        public ImageCreatorComplex(From_Log log)
        {
            form_log = log;

            guid_dictionary = new Dictionary<Guid, List<ModelItem>>();
        }

        public override void make_image(ClashResult cl_result, string directoryPath, string test_name)
        {
            if (cl_result == null) return;

            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            // item 숨김 초기화
            doc.Models.ResetAllHidden();
            ModelItem item1 = cl_result.Item1;

            if (item1 == null) return;
            string d_path = Path.Combine(@"C:\objectinfo\ResultImage\단순-다각도이미지\", test_name);
            if (Directory.Exists(d_path))
            {
                try
                {
                    string[] files = Directory.GetFiles(d_path);
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

            ModelItemCollection items = new ModelItemCollection();
            items.Add(cl_result.Item1);
            //items.Add(clResult.Item2);

            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    items.Add(item);
                    guid_dictionary.Remove(item1.InstanceGuid);
                }
            }

            string item1class = Getinfo(item1, "요소", "IfcClass");
            List<String> itemclass = new List<String>();
            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    itemclass.Add(Getinfo(item, "요소", "IfcClass"));
                }
            }

            doc.CurrentSelection.Clear();
            doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

            // 이미지에 보일 item들, 안 보일 item들, 투명하게 보일 item들을 담는 객체 생성
            ModelItemCollection modelItemsToShow = new ModelItemCollection();
            ModelItemCollection modelItemsToHide = new ModelItemCollection();
            ModelItemCollection modelItemToTransparant = new ModelItemCollection();


            // 간섭된 항목들은 보일 item에 추가
            foreach (ModelItem item in items)
            {
                if (item.DescendantsAndSelf != null)
                    modelItemsToShow.AddRange(item.DescendantsAndSelf);
            }

            string levelInfo1 = Getinfo(item1, "요소", "IfcSpatialContainer");
            string levelInfo2 = Getinfo(item1, "Constraints", "Level");
            string systemInfo1 = Getinfo(item1, "요소", "IfcSystem");
            //string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");
            //string systemInfo2 = Getinfo(item2, "요소", "IfcSystem");
            List<string> systemInfo2 = new List<string>();
            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    systemInfo2.Add(Getinfo(item, "요소", "IfcSystem"));
                }
            }

/*            if (background)
            {
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
            }*/

            modelItemsToHide.CopyFrom(modelItemsToShow);
            doc.CurrentSelection.CopyFrom(modelItemsToShow);
            modelItemsToHide.Invert(doc); // invert 함수는 현재 선택된 item들을 반전
            doc.Models.SetHidden(modelItemsToHide, true);

            doc.CurrentSelection.Clear();

            // 간섭 부재에 색과 투명도 적용 
            Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
            Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

            if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

            for (int i = 1; i < items.Count; i++)
            {
                if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(i), null))
                    doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(i) }, GREEN);
            }

            // 간섭 부재 자체는 투명도 적용하지 않기
            modelItemToTransparant.Remove(items.ElementAtOrDefault(0));
            for (int i = 1; i < items.Count; i++)
            {
                modelItemToTransparant.Remove(items.ElementAtOrDefault(i));
            }
            //modelItemToTransparant.Remove(items.ElementAtOrDefault(1));
            // Adjust transparancy (false일 경우엔 투명도를 적용하지 않음)
/*            if (transparent)
            {
                doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
            }*/

            string testsimpleNamePath = Path.Combine(directoryPath, "복합-단순이미지", $"{test_name}");
            string testsideNamePath = Path.Combine(directoryPath, "복합-다각도이미지", $"{test_name}");

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
            BoundingBox3D box = items.BoundingBox();

            if (items.Count == 1) return;

            for (int i = 0; i < 10; i++)
            {
                Viewpoint copy = doc.CurrentViewpoint.CreateCopy();
                switch (i)
                {
                    case 8:
                        ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eTOP);
                        copy.PivotPoint = box.Center;
                        copy.ZoomBox(items.BoundingBox());
                        break;
                    case 9:
                        ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eBOTTOM);
                        copy.PivotPoint = box.Center;
                        copy.ZoomBox(items.BoundingBox());
                        break;
                    default:
                        double newAngle = (i * 45) * (Math.PI / 180);
                        UnitVector3D newAxis = new UnitVector3D(0, 0, 1);
                        Rotation3D newRotation = new Rotation3D(newAxis, newAngle);
                        ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT);
                        copy.Lighting = ViewpointLighting.FullLights;

                        Point3D newMin = new Point3D(box.Min.X, box.Min.Y, box.Min.Z);
                        Point3D newMax = new Point3D(box.Max.X, box.Max.Y, box.Max.Z);

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
                        break;
                }

                doc.CurrentSelection.Clear();
                doc.CurrentViewpoint.CopyFrom(copy);

                using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                {
                    clashImage.Save(Path.Combine(testsideNamePath, $"{cl_result.DisplayName.Substring(2)}_{i + 1}.png"), ImageFormat.Png);
                }
            }

            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
            // 색 초기화
            doc.Models.ResetAllPermanentMaterials();
            //Hide all
            doc.Models.SetHidden(modelItemsToShow, true);
        }
    }

}//namespace Integrity_Checker_MEP