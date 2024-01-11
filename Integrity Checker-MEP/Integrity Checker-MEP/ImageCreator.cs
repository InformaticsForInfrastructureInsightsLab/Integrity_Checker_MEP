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

namespace Integrity_Checker_MEP
{
    public class ImageCreator
    {

        private int width = 500;
        private int height = 500;

        public void CreateAndFillImages(string directoryPath)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;
            string[] clashtypes = { "Arch-Arch_Duplicate", "Str-Str_Duplicate", "Arch-Arch_Clearance", "Arch-MECH_Clearance", "Arch-Str_Clearance", "Str-MECH_Clearance", "Str-Str_Clearance" };
            //hide all
            HideAllItems(doc);
            foreach (ClashTest test in oDCT.Tests)
            {
                if (clashtypes.Contains(test.DisplayName)){
                    List<ClashResult> outedResults = new List<ClashResult>();
                    RecurseFillResults(test, ref outedResults);
                    if (outedResults != null && outedResults.Count > 0 && !string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                    {

                        foreach (ClashResult r in outedResults)
                        {
                            CreateAndFillImage(doc, r, directoryPath, test.DisplayName);
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
                    items.Add(item1);
                    items.Add(item2);
                    doc.CurrentSelection.Clear();

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
                    doc.ActiveView.FocusOnCurrentSelection();
                    doc.CurrentSelection.Clear();

                    // Adjust the camera, lighting, and paint the clashing elements in Red and Green respectively
                    Viewpoint copy = viewpoint.CreateCopy();
                    copy.Lighting = 0;

                    doc.Models.ResetAllPermanentMaterials();
                    doc.CurrentViewpoint.CopyFrom(copy);

                    Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
                    Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

                    if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(1), null))
                        doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(1) }, GREEN);

                    doc.ActiveView.LookFromFrontRightTop();

                    doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

                    // Save the Clash image
                    using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene,width, height))
                    {
                        string clashResultImageName = Path.Combine(directoryPath, $"{testName}_{clResult.DisplayName}.png");

                        clashImage.Save(clashResultImageName, ImageFormat.Png);
                    }
                    //Hide clashelements
                    doc.Models.SetHidden(modelItemsToShow, true);
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
