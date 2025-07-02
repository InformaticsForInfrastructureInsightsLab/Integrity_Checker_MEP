using ClashTest2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Integrity_Checker_MEP
{
    abstract class DashboardBarchart_Base
    {
        protected int[,,] hardClashMatrix = new int[3, 6, 6];
        protected Dictionary<string, List<int>> softClashDict = new Dictionary<string, List<int>>
        {
            { "UnnecessaryMEPElement", new List<int>{ 0,0,0 } },
            { "DoorClearance",  new List<int>{ 0,0,0 } },
            { "CeilingHeightCompliance", new List < int > { 0, 0, 0 } },
            { "MEPPassageAccess", new List < int > { 0, 0, 0 } },
            { "PipeCeilingClearance", new List < int > { 0, 0, 0 } },
            { "MEPBelowCeiling", new List < int > { 0, 0, 0 } },
            { "BeamDuctClearance", new List < int > { 0, 0, 0 } },
            { "DuctClearance", new List < int > { 0, 0, 0 } },
            { "DuctPipeClearance", new List < int > { 0, 0, 0 } },
            { "PipeClearance", new List < int > { 0, 0, 0 } },
            { "FittingOmission", new List < int > { 0, 0, 0 } },
        };
        protected Model model;

        public abstract void FillMatrix(form_ResultViewer rv);
        public PlotModel getModel()
        {
            var model = new PlotModel { Title = "Clash Status" };
            // 범례 객체 생성 및 설정
            var legend = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopRight,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 1
            };
            model.Legends.Add(legend);

            string[] modelNames = { "Arch", "COMM", "ELEC", "FIRE", "MECH", "Str" };

            // X축
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };

            List<BarItem> minor = new List<BarItem>();
            List<BarItem> medium = new List<BarItem>();
            List<BarItem> major = new List<BarItem>();

            // for hard clash
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    if (hardClashMatrix[0, i, j] + hardClashMatrix[1, i, j] + hardClashMatrix[2, i, j] == 0)
                        continue;
                    categoryAxis.Labels.Add(modelNames[i] + "-" + modelNames[j]);

                    minor.Add(new BarItem { Value = hardClashMatrix[0, i, j] });
                    medium.Add(new BarItem { Value = hardClashMatrix[1, i, j] });
                    major.Add(new BarItem { Value = hardClashMatrix[2, i, j] });
                }
            }

            foreach (var soft in softClashDict)
            {
                categoryAxis.Labels.Add(soft.Key);
                minor.Add(new BarItem { Value = soft.Value[0] });
                medium.Add(new BarItem { Value = soft.Value[1] });
                major.Add(new BarItem { Value = soft.Value[2] });
            }

            model.Axes.Add(categoryAxis);

            // Y축
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0,
                MaximumPadding = 0.1
            };
            model.Axes.Add(valueAxis);

            var minorSeries = new BarSeries
            {
                Title = "minor",
                IsStacked = true,
                FillColor = OxyColors.Blue,
                LabelFormatString = "{0:0}", 
                LabelPlacement = LabelPlacement.Inside,
                TextColor = OxyColors.Black
            };
            var mediumSeries = new BarSeries
            {
                Title = "medium",
                IsStacked = true,
                FillColor = OxyColors.Green,
                LabelFormatString = "{0:0}",
                LabelPlacement = LabelPlacement.Inside,
                TextColor = OxyColors.Black
            };
            var majorSeries = new BarSeries
            {
                Title = "major",
                IsStacked = true,
                FillColor = OxyColors.Red,
                LabelFormatString = "{0:0}",
                LabelPlacement = LabelPlacement.Inside,
                TextColor = OxyColors.Black
            };

            minorSeries.Items.AddRange(minor);
            mediumSeries.Items.AddRange(medium);
            majorSeries.Items.AddRange(major);

            model.Series.Add(majorSeries);
            model.Series.Add(mediumSeries);
            model.Series.Add(minorSeries);

            return model;
        }
    }

    class DashboardBarchart_Adj : DashboardBarchart_Base
    {

        public DashboardBarchart_Adj(form_ResultViewer rv) 
        {
            FillMatrix(rv);
        }

        public override void FillMatrix(form_ResultViewer rv)
        {
            foreach (ClashData cd in rv.dataList)
            {
                if (cd.Type == "Hard")
                {
                    string[] model = cd.ClashType.Split('-');
                    Array.Sort(model);
                    int model1 = DashboardUtils.MapModel(model[0]);
                    int model2 = DashboardUtils.MapModel(model[1]);
                    if (model1 == -1 || model2 == -1)
                        continue;

                    int severity = DashboardUtils.MapSeverity(cd.Adjusted_Severity);
                    if (severity == -1)
                        continue;

                    hardClashMatrix[severity, model1, model2]++;
                }
                else
                {
                    int severity = DashboardUtils.MapSeverity(cd.Adjusted_Severity);
                    try
                    {
                        softClashDict[cd.ClashType][severity]++;
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }

                }

            }
        }
    }

    class DashboardBarchart_Origin : DashboardBarchart_Base
    {
        public DashboardBarchart_Origin(form_ResultViewer rv)
        {
            FillMatrix(rv);
        }

        public override void FillMatrix(form_ResultViewer rv)
        {
            foreach (ClashData cd in rv.dataList)
            {
                if (cd.Type == "Hard")
                {
                    string[] model = cd.ClashType.Split('-');
                    Array.Sort(model);
                    
                    int model1 = DashboardUtils.MapModel(model[0]);
                    int model2 = DashboardUtils.MapModel(model[1]);
                    if (model1 == -1 || model2 == -1)
                        continue;

                    int severity = DashboardUtils.MapSeverity(cd.Severity);
                    if (severity == -1)
                        continue;

                    hardClashMatrix[severity, model1, model2]++;
                }
                else
                {
                    int severity = DashboardUtils.MapSeverity(cd.Severity);
                    try
                    {
                        softClashDict[cd.ClashType][severity]++;
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }
                
            }
        }
    }
}
