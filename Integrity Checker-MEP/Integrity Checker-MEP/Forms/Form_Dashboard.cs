using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClashTest2;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Integrity_Checker_MEP.Forms
{
    public partial class Form_Dashboard : Form
    {
        int[,,] clashMatrixAdjusted = new int[3,6,6];
        int[,,] clashMatrixOrigin = new int[3,6,6];
        form_ResultViewer rv = MainClass.rv;

        public Form_Dashboard()
        {
            InitializeComponent();

            // TableLayoutPanel 설정 (2개 열, 1개 행)
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 왼쪽 50%
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 오른쪽 50%

            var piePlot = new PlotView
            {
                Model = CreatePieChart(),
                Dock = DockStyle.Fill
            };

            FillMatrixAdjusted();
            var barPlot = new PlotView
            {
                Model = CreateBarChart(),
                Dock = DockStyle.Fill
            };

            tableLayout.Controls.Add(piePlot, 0, 0);
            tableLayout.Controls.Add(barPlot, 1, 0);

            this.Controls.Add(tableLayout);
        }

        private PlotModel CreatePieChart()
        {
            var model = new PlotModel { Title = "Clash Severity" };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 1.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
                OutsideLabelFormat = "{1}: {0:N0}", // "{value}: {percentage}"
                InsideLabelFormat = "{0:N0}", // "{value}"
            };

            // 데이터 추가
            pieSeries.Slices.Add(new PieSlice("Major", rv.major_hard + rv.major_soft) { Fill = OxyColors.Red }); //하드
            pieSeries.Slices.Add(new PieSlice("Medium", rv.medium_hard + rv.medium_soft) { Fill = OxyColors.Green }); //미디움
            pieSeries.Slices.Add(new PieSlice("Minor", rv.minor_hard + rv.minor_soft) { Fill = OxyColors.Blue }); //마이너

            model.Series.Add(pieSeries);
            return model;
        }

        private void FillMatrixAdjusted()
        {
            foreach(ClashData cd in rv.dataList)
            {
                string[] model = cd.HardClashType.Split('-');
                Array.Sort(model);

                int model1 = MapModel(model[0]);
                int model2 = MapModel(model[1]);
                if (model1 == -1 || model2 == -1) 
                    continue;

                int severity = MapSeverity(cd.Adjusted_Severity);
                if (severity == -1)
                    continue;

                clashMatrixAdjusted[severity, model1, model2]++;
            }
        }

        private void FillMatrixOrigin()
        {
            foreach (ClashData cd in rv.dataList)
            {
                string[] model = cd.HardClashType.Split('-');
                Array.Sort(model);

                int model1 = MapModel(model[0]);
                int model2 = MapModel(model[1]);
                if (model1 == -1 || model2 == -1)
                    continue;

                int severity = MapSeverity(cd.Severity);
                if (severity == -1)
                    continue;

                clashMatrixOrigin[severity, model1, model2]++;
            }
        }

        private int MapModel(string model)
        {
            switch (model)
            {
                case "Arch":
                    return 0;
                case "COMM":
                    return 1;
                case "ELEC":
                    return 2;
                case "FIRE":
                    return 3;
                case "MECH":
                    return 4;
                case "Str":
                    return 5;
            }
            return -1;
        }

        private int MapSeverity(string severity)
        {
            switch (severity)
            {
                case "Minor":
                    return 0;
                case "Medium":
                    return 1;
                case "Major":
                    return 2;
            }
            return -1;
        }

        private PlotModel CreateBarChart()
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
                    if (clashMatrixAdjusted[0, i, j] + clashMatrixAdjusted[1, i, j] + clashMatrixAdjusted[2, i, j] == 0)
                        continue;
                    categoryAxis.Labels.Add(modelNames[i] + "-" + modelNames[j]);

                    minor.Add(new BarItem { Value = clashMatrixAdjusted[0, i, j] });
                    medium.Add(new BarItem { Value = clashMatrixAdjusted[1, i, j] });
                    major.Add(new BarItem { Value = clashMatrixAdjusted[2, i, j] });
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

            var minorSeries = new BarSeries {  Title = "minor", IsStacked = true, FillColor = OxyColors.Blue };
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
}
