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

namespace Integrity_Checker_MEP
{
    public class ImageCreator
    {
        Dictionary<string, List<string>> clashResultName = new Dictionary<string,List<string>>();

        private int width = 1000;
        private int height = 500;

        public void getResultName(Dictionary<string, List<string>> names)
        {
            clashResultName = names;
        }


        public void CreateAndFillImages(string directoryPath)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;
            //string[] clashtypes = { "Arch-Arch_Duplicate", "Str-Str_Duplicate", "Arch-Arch_Clearance", "Arch-MECH_Clearance", "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance" };
            //string[] clashtypes = { "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance" };
            string[] clashtypes = { "Str-MECH_Clearance" };
            
            //hide all
            HideAllItems(doc);

            foreach (ClashTest test in oDCT.Tests)
            {
                //for debugging
                if (clashtypes.Contains(test.DisplayName) && clashResultName.ContainsKey(test.DisplayName))
                //if (clashResultName.ContainsKey(test.DisplayName))
                {

                    List<string> resultNames = new List<string>();
                    resultNames = clashResultName[test.DisplayName];

                    List<ClashResult> outedResults = new List<ClashResult>();
                    RecurseFillResults(test, ref outedResults);
                    if (outedResults != null && outedResults.Count > 0 && !string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                    {

                        foreach (ClashResult r in outedResults)
                        {
                            if (resultNames.Contains(r.DisplayName))
                            {
                                CreateAndFillImage(doc, r, directoryPath, test.DisplayName);
                            }
                        }
                    }
                }
            }
            //show all
            doc.Models.ResetAllHidden();
        }


        private void CreateAndFillImage(Document doc, ClashResult clResult, string directoryPath, string testName)
        {
            if (clResult != null)
            {
                
                Viewpoint viewpoint = doc.CurrentViewpoint.Value;
                // Get the 2 clashing elements from the ClashResult
                ModelItem item1 = clResult.Item1;
                ModelItem item2 = clResult.Item2;
                if (item1 != null && item2 != null)
                {
                    ModelItemCollection items = new ModelItemCollection();
                    ModelItemCollection invertItems = new ModelItemCollection();
                    items.Add(item1);
                    items.Add(item2);
                    invertItems.Add(item1);
                    invertItems.Add(item2);
                    invertItems.Invert(doc);
                    doc.CurrentSelection.Clear();
                    doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);


                    //Show clash items
                    ModelItemCollection modelItemsToShow = new ModelItemCollection();
                    foreach (ModelItem item in items)
                    {
                        if (item.AncestorsAndSelf != null)
                            modelItemsToShow.AddRange(item.AncestorsAndSelf);

                        if (item.Descendants != null)
                            modelItemsToShow.AddRange(item.Descendants);
                    }
                    

                    doc.Models.SetHidden(modelItemsToShow, false);
                    // Select the 2 clashing elements
                    doc.CurrentSelection.Clear();
                    doc.CurrentSelection.CopyFrom(items);

                    // Adjust the camera, lighting, and paint the clashing elements in Red and Green respectively
                    Viewpoint copy = viewpoint.CreateCopy();
                    copy.Projection = ViewpointProjection.Orthographic;
                    copy.AlignDirection(new Vector3D(0, 0, -1));
                    copy.Rotation = new Rotation3D(0, 0, 0, 1);
                    copy.ZoomBox(items.BoundingBox());


                    doc.CurrentSelection.Clear();
                    copy.Lighting = 0;


                    doc.CurrentViewpoint.CopyFrom(copy);


                    // Set Selection box
                    /*var viewPointValue = doc.CurrentViewpoint.Value;
                    var planes = viewPointValue.InternalClipPlanes;
                    planes.SetMode(LcOaClipPlaneSetMode.eMODE_BOX);
                    planes.SetBox(items.BoundingBox());
                    planes.SetEnabled(true);*/


                    // Make other items transparent
                    /*ModelItemCollection itemsToTransparant = new ModelItemCollection();

                    foreach(ModelItem item in invertItems)
                    {
                        if (items.BoundingBox().Intersects(item.BoundingBox()))
                        {
                            itemsToTransparant.Add(item);
                        }
                    }
                    doc.Models.SetHidden(itemsToTransparant, false);
                    doc.Models.OverrideTemporaryTransparency(itemsToTransparant, 100);*/

                    //doc.Models.SetHidden(invertItems, false);
                    //doc.Models.OverridePermanentTransparency(invertItems, 100);

                    Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
                    Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(1), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(1) }, GREEN);
                    


                    string testsimpleNamePath = Path.Combine(directoryPath, "단순이미지", $"{testName}");
                    string testsideNamePath = Path.Combine(directoryPath, "다각도이미지", $"{testName}");


                    DirectoryInfo diSimple = new DirectoryInfo(testsimpleNamePath);

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
                    }

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
                    doc.SetPlainBackground(Autodesk.Navisworks.Api.Color.White);
                    

                    //((LcOwViewer)doc.ActiveView.Viewer).LookFrom((Autodesk.Navisworks.Api.Interop.LcOaPartitionViewDirection)2);
                    using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                    {
                        clashImage.Save(Path.Combine(testsimpleNamePath, $"{clResult.DisplayName.Substring(2)}.png"), ImageFormat.Png);
                    }
                    /*for (int i = 0; i< 7; i++)
                    {
                        //0~6 앞 뒤 위 아래 왼쪽 오른쪽
                        ((LcOwViewer)doc.ActiveView.Viewer).LookFrom((Autodesk.Navisworks.Api.Interop.LcOaPartitionViewDirection)i);
                        
                        
                        doc.SetPlainBackground(Autodesk.Navisworks.Api.Color.White);
                        doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

                        // Save the Clash image
                        using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
                        {
                            string clashResultImageName;
                            if((Autodesk.Navisworks.Api.Interop.LcOaPartitionViewDirection)i == LcOaPartitionViewDirection.eFRONT_RIGHT_TOP)
                            {
                                clashResultImageName = Path.Combine(testsimpleNamePath, $"{clResult.DisplayName.Substring(2)}.png");

                            }
                            else
                            {
                                clashResultImageName = Path.Combine(testsideNamePath, $"{clResult.DisplayName.Substring(2)}_{i+1}.png");
                            }

                            clashImage.Save(clashResultImageName, ImageFormat.Png);
                        }
                    }*/


                    doc.Models.ResetAllPermanentMaterials();


                    //Hide all
                    doc.Models.SetHidden(modelItemsToShow, true);
                    //doc.Models.SetHidden(invertItems, true);
                    //doc.Models.SetHidden(itemsToTransparant, true);
                }
            }
        }

        private void HideAllItems(Document doc)
        {
            ModelItemCollection rootItems = new ModelItemCollection();
            ModelItemCollection allitems = new ModelItemCollection();
            foreach (var m in doc.Models)
            {
                if (m.RootItem != null)
                {
                    rootItems.Add(m.RootItem);
                }
            }
            GetAllItems(rootItems, ref allitems);
            if (allitems != null && allitems.Count > 0)
            {
                doc.Models.SetHidden(allitems, true);
            }
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
    }
    
}
