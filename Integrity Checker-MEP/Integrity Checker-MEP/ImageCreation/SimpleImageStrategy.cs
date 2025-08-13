using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Integrity_Checker_MEP.ImageCreation
{
    public class SimpleImageStrategy : ImageCreationStrategyBase
    {
        public SimpleImageStrategy(ILogger logger) : base(logger)
        {
            // Specific settings for SimpleImageStrategy, if any
        }

        public override void MakeImage(ClashResult clResult, string directoryPath, string testName)
        {
            if (clResult == null) return;

            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            
            doc.Models.ResetAllHidden();
            ModelItem item1 = clResult.Item1;
            ModelItem item2 = clResult.Item2;

            if (item1 == null || item2 == null) return;

            // Check file count, moved from original ImageCreator.cs
            string d_path = Path.Combine(ProjectSettings.ResultImageFolderPath, "단순-다각도이미지", testName); // Using ProjectSettings
            if (Directory.Exists(d_path))
            {
                try
                {
                    string[] files = Directory.GetFiles(d_path);
                    if (files.Length >= 19992) // Magic number, consider moving to ProjectSettings
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
            items.Add(item1);
            items.Add(item2);

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
            string systemInfo2 = GetInfo(item2, "요소", "IfcSystem");

            if (SetBackground) // Using property from base class
            {
                AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    modelItemsToShow, modelItemToTransparant, "IfcWall", new string[] { systemInfo1, systemInfo2 });
                AddToItemsToShow(doc, new string[] { levelInfo1, levelInfo2 },
                    modelItemsToShow, modelItemToTransparant, "IfcCurtainWall", new string[] { systemInfo1, systemInfo2 });
            }

            modelItemsToHide.CopyFrom(modelItemsToShow);
            doc.CurrentSelection.CopyFrom(modelItemsToShow);
            modelItemsToHide.Invert(doc);
            doc.Models.SetHidden(modelItemsToHide, true);

            doc.CurrentSelection.Clear();
            doc.SetPlainBackground(Autodesk.Navisworks.Api.Color.White);

            Autodesk.Navisworks.Api.Color RED = Autodesk.Navisworks.Api.Color.Red;
            Autodesk.Navisworks.Api.Color GREEN = Autodesk.Navisworks.Api.Color.Green;

            if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(0), null))
                doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(0) }, RED);

            if (!NativeHandle.ReferenceEquals(items.ElementAtOrDefault(1), null))
                doc.Models.OverridePermanentColor(new ModelItem[1] { items.ElementAtOrDefault(1) }, GREEN);

            modelItemToTransparant.Remove(items.ElementAtOrDefault(0));
            modelItemToTransparant.Remove(items.ElementAtOrDefault(1));

            if (IsTransparant) // Using property from base class
            {
                doc.Models.OverridePermanentTransparency(modelItemToTransparant, Transparancy);
            }

            string testsideNamePath = Path.Combine(directoryPath, "단순-다각도이미지", testName); // Using ProjectSettings

            for (int i = 0; i < 12; i++)
            {
                const double pi = 3.14159265358979;
                double newAngle = (i * 30) * (pi / 180);

                UnitVector3D newAxis = new UnitVector3D(0, 0, 1);
                Rotation3D newRotation = new Rotation3D(newAxis, newAngle);
                ((LcOwViewer)doc.ActiveView.Viewer).LookFrom(LcOaPartitionViewDirection.eFRONT_RIGHT_TOP);
                Viewpoint copy = doc.CurrentViewpoint.CreateCopy();

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

                SaveImageToFile(doc, items, testsideNamePath, testName, clResult, i + 1); // Using base method
            }

            ResetViewAndMaterials(doc, modelItemsToShow);
        }
    }
}
