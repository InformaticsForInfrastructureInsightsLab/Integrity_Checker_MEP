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
        protected int[,,] clashMatrix = new int[3, 6, 6];
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

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    if (clashMatrix[0, i, j] + clashMatrix[1, i, j] + clashMatrix[2, i, j] == 0)
                        continue;
                    categoryAxis.Labels.Add(modelNames[i] + "-" + modelNames[j]);

                    minor.Add(new BarItem { Value = clashMatrix[0, i, j] });
                    medium.Add(new BarItem { Value = clashMatrix[1, i, j] });
                    major.Add(new BarItem { Value = clashMatrix[2, i, j] });
                }
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

            var minorSeries = new BarSeries { Title = "minor", IsStacked = true, FillColor = OxyColors.Blue };
            var mediumSeries = new BarSeries { Title = "medium", IsStacked = true, FillColor = OxyColors.Green };
            var majorSeries = new BarSeries { Title = "major", IsStacked = true, FillColor = OxyColors.Red };

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
                string[] model = cd.ClashType.Split('-');
                Array.Sort(model);

                int model1 = DashboardUtils.MapModel(model[0]);
                int model2 = DashboardUtils.MapModel(model[1]);
                if (model1 == -1 || model2 == -1)
                    continue;

                int severity = DashboardUtils.MapSeverity(cd.Adjusted_Severity);
                if (severity == -1)
                    continue;

                clashMatrix[severity, model1, model2]++;
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
                string[] model = cd.ClashType.Split('-');
                Array.Sort(model);

                int model1 = DashboardUtils.MapModel(model[0]);
                int model2 = DashboardUtils.MapModel(model[1]);
                if (model1 == -1 || model2 == -1)
                    continue;

                int severity = DashboardUtils.MapSeverity(cd.Severity);
                if (severity == -1)
                    continue;

                clashMatrix[severity, model1, model2]++;
            }
        }
    }
}
