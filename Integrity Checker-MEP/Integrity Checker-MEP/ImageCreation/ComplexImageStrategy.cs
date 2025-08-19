using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Integrity_Checker_MEP.ImageCreation
{
    public class ComplexImageStrategy : ImageCreationStrategyBase
    {
        public Dictionary<Guid, List<ModelItem>> guid_dictionary { get; set; } // Made public for external access

        public ComplexImageStrategy(ILogger logger) : base(logger)
        {
            guid_dictionary = new Dictionary<Guid, List<ModelItem>>();
        }

        public override void MakeImage(ClashResult clResult, string directoryPath, string testName)
        {
            if (clResult == null) return;

            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            
            doc.Models.ResetAllHidden();
            ModelItem item1 = clResult.Item1;

            if (item1 == null) return;

            // Check file count, moved from original ImageCreator.cs
            string d_path = Path.Combine(ProjectSettings.ResultImageFolderPath, "복합-다각도이미지", testName); // Using ProjectSettings
            if (Directory.Exists(d_path))
            {
                try
                {
                    string[] files = Directory.GetFiles(d_path);
                    if (files.Length >= 99999) // Magic number, consider moving to ProjectSettings
                    {
                        _logger.Log($"Skipping image creation for {testName} due to file count limit.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    _logger.Log($"Error checking file count for {d_path}: {e.ToString()}");
                }
            }

            ModelItemCollection items = new ModelItemCollection();
            items.Add(clResult.Item1);

            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    items.Add(item);
                    guid_dictionary.Remove(item1.InstanceGuid);
                }
            }

            string item1class = GetInfo(item1, "요소", "IfcClass");
            List<String> itemclass = new List<String>();
            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    itemclass.Add(GetInfo(item, "요소", "IfcClass"));
                }
            }

            doc.CurrentSelection.Clear();
            doc.ActiveView.RequestDelayedRedraw((ViewRedrawRequests)3);

            ModelItemCollection modelItemsToShow = new ModelItemCollection();
            ModelItemCollection modelItemsToHide = new ModelItemCollection();
            ModelItemCollection modelItemToTransparant = new ModelItemCollection();

            foreach (ModelItem item in items)
            {
                if (item.DescendantsAndSelf != null)
                    modelItemsToShow.AddRange(item.DescendantsAndSelf);
            }

            string levelInfo1 = GetInfo(item1, "요소", "IfcSpatialContainer");
            string levelInfo2 = GetInfo(item1, "Constraints", "Level");
            string systemInfo1 = GetInfo(item1, "요소", "IfcSystem");
            List<string> systemInfo2 = new List<string>();
            if (guid_dictionary.ContainsKey(item1.InstanceGuid))
            {
                foreach (ModelItem item in guid_dictionary[item1.InstanceGuid])
                {
                    systemInfo2.Add(GetInfo(item, "요소", "IfcSystem"));
                }
            }

            //if (SetBackground) // Using property from base class
            //{
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] { systemInfo1 }.Concat(systemInfo2).ToArray());
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcCurtainWall", new string[] { "a", "b" }); // Hardcoded systeminfo, needs review
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcSlab", new string[] { "a", "b" }); // Hardcoded systeminfo, needs review
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcDoor", new string[] { "a", "b" }); // Hardcoded systeminfo, needs review
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcColumn", new string[] { "a", "b" }); // Hardcoded systeminfo, needs review
            //    AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
            //        modelItemsToShow, modelItemToTransparant, "IfcWindow", new string[] { "a", "b" }); // Hardcoded systeminfo, needs review
            //}

            modelItemsToHide.CopyFrom(modelItemsToShow);
            doc.CurrentSelection.CopyFrom(modelItemsToShow);
            modelItemsToHide.Invert(doc);
            doc.Models.SetHidden(modelItemsToHide, true);

            doc.CurrentSelection.Clear();

            Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
            Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

            if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

            for (int i = 1; i < items.Count; i++)
            {
                if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(i), null))
                    doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(i) }, GREEN);
            }

            modelItemToTransparant.Remove(items.ElementAtOrDefault(0));
            for (int i = 1; i < items.Count; i++)
            {
                modelItemToTransparant.Remove(items.ElementAtOrDefault(i));
            }

            // modelItemToTransparant.Remove(items.ElementAtOrDefault(1));
            // Adjust transparancy(false일 경우엔 투명도를 적용하지 않음)
            //if (transparent)
            //{
            //    doc.Models.OverridePermanentTransparency(modelItemToTransparant, transparancy);
            //}

            string testsideNamePath = Path.Combine(directoryPath, "복합-다각도이미지", testName); // Using ProjectSettings

            BoundingBox3D box = items.BoundingBox();

            if (items.Count == 1) return; // Original code had this check

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

                        BoundingBox3D expandedBox = new BoundingBox3D(newMin, newMax);
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

                SaveImageToFile(doc, items, testsideNamePath, testName, clResult, i + 1); // Using base method
            }

            ResetViewAndMaterials(doc, modelItemsToShow);
        }
    }
}
