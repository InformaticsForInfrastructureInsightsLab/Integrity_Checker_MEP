using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text; // For StringBuilder

namespace Integrity_Checker_MEP.ImageCreation
{
    public abstract class ImageCreationStrategyBase : IImageCreationStrategy
    {
        protected readonly ILogger _logger;
        protected int width = 500;
        protected int height = 500;

        public bool SetBackground { get; set; } = true;
        public bool IsTransparant { get; set; } = true;
        public int Transparancy { get; set; } = 128;
        public double SetMagnification { get; set; } = 1500.0;

        protected ImageCreationStrategyBase(ILogger logger)
        {
            _logger = logger;
        }

        public abstract void MakeImage(ClashResult clashResult, string directoryPath, string testName);

        protected string GetInfo(ModelItem item, string category, string property)
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

        protected void AddToItemsToShow(Document doc, string[] levelInfo, ModelItemCollection modelItemsToShow,
            ModelItemCollection modelItemToTransparant, string className, string[] systeminfo)
        {
            if (levelInfo == null) return;

            Search search = new Search();
            Search search2 = new Search();
            Search search3 = new Search();

            Autodesk.Navisworks.Api.Color BLUE = Autodesk.Navisworks.Api.Color.Blue;
            Autodesk.Navisworks.Api.Color ORANGE = Autodesk.Navisworks.Api.Color.FromByteRGB(255, 128, 64);

            // Original code used Threads, which can be problematic with Navisworks API.
            // I'll convert this to sequential calls for now, or use Task.Run if threading is truly needed and safe.
            // For simplicity and safety, I'll make them sequential.

            // Search 1: Level and Class
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

            ModelItemCollection wallItems = search.FindAll(doc, false);
            foreach (ModelItem item in wallItems)
            {
                modelItemsToShow.Add(item);
                modelItemToTransparant.Add(item);
            }

            // Search 2: System Info 1
            search2.Selection.SelectAll();
            SearchCondition systemcondition1 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[0]));
            List<SearchCondition> l3 = new List<SearchCondition>();
            l3.Add(systemcondition1);
            search2.SearchConditions.AddGroup(l3);

            ModelItemCollection mepItems = search2.FindAll(doc, false);
            foreach (ModelItem item in mepItems)
            {
                doc.Models.OverridePermanentColor(new ModelItem[1] { item }, BLUE);
                modelItemsToShow.Add(item);
                modelItemToTransparant.Add(item);
            }

            // Search 3: System Info 2
            search3.Selection.SelectAll();
            SearchCondition systemcondition2 = SearchCondition.HasPropertyByDisplayName("요소", "IfcSystem").EqualValue(new VariantData(systeminfo[1]));
            List<SearchCondition> l4 = new List<SearchCondition>();
            l4.Add(systemcondition2);
            search3.SearchConditions.AddGroup(l4);

            ModelItemCollection mepItems2 = search3.FindAll(doc, false);
            foreach (ModelItem item in mepItems2)
            {
                doc.Models.OverridePermanentColor(new ModelItem[1] { item }, ORANGE);
                modelItemsToShow.Add(item);
                modelItemToTransparant.Add(item);
            }
        }

        protected void SaveImageToFile(Document doc, ModelItemCollection items, string filePath, string testName, ClashResult clResult, int imageIndex = 0)
        {
            // Ensure directory exists and has write permissions
            DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
                // Add access control if needed, similar to original code
                var directorySecurity = dirInfo.GetAccessControl();
                var currentUserIdentity = WindowsIdentity.GetCurrent();
                var fileSystemRule = new FileSystemAccessRule(currentUserIdentity.Name,
                                                              FileSystemRights.Read | FileSystemRights.Write, // Added Write
                                                              InheritanceFlags.ObjectInherit |
                                                              InheritanceFlags.ContainerInherit,
                                                              PropagationFlags.None,
                                                              AccessControlType.Allow);
                directorySecurity.AddAccessRule(fileSystemRule);
                dirInfo.SetAccessControl(directorySecurity);
            }

            using (Bitmap clashImage = doc.ActiveView.GenerateImage(ImageGenerationStyle.Scene, width, height))
            {
                string item1_ifcguid = GetInfo(items.ElementAtOrDefault(0), "요소", "IfcGUID");
                string item2_ifcguid = GetInfo(items.ElementAtOrDefault(1), "요소", "IfcGUID");
                string fileName = $"{item1_ifcguid}+{item2_ifcguid}_{imageIndex}.png";
                string fullPath = Path.Combine(filePath, fileName);
                clashImage.Save(fullPath, ImageFormat.Png);
            }
        }

        protected void ResetViewAndMaterials(Document doc, ModelItemCollection modelItemsToShow)
        {
            ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
            doc.Models.ResetAllPermanentMaterials();
            doc.Models.SetHidden(modelItemsToShow, true);
        }
    }
}
