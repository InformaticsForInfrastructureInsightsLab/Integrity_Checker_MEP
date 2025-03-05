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
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Integrity_Checker_MEP.Forms
{
    public partial class Form_Dashboard : Form
    {
        int[,,] clashMatrix = new int[3,6,6];

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

            FillMatrix();
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
            pieSeries.Slices.Add(new PieSlice("Hard", rv.major_hard + rv.major_soft) { Fill = OxyColors.Red }); //하드
            pieSeries.Slices.Add(new PieSlice("Medium", rv.medium_hard + rv.medium_soft) { Fill = OxyColors.Yellow }); //미디움
            pieSeries.Slices.Add(new PieSlice("Minor", rv.minor_hard + rv.minor_soft) { Fill = OxyColors.Purple }); //마이너

            model.Series.Add(pieSeries);
            return model;
        }

        private void FillMatrix()
        {
            foreach(ClashData cd in rv.dataList)
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

                clashMatrix[severity, model1, model2]++;
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

            string[] modelNames = { "Arch", "COMM", "ELEC", "FIRE", "MECH", "Str" };

            // X축
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    if (clashMatrix[0, i, j] + clashMatrix[1, i, j] + clashMatrix[2, i, j] == 0)
                        continue;
                    categoryAxis.Labels.Add(modelNames[i] + "-" + modelNames[j]);
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

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    var series = new BarSeries { Title = modelNames[i] + "-" + modelNames[j], IsStacked = true };
                    series.Items.Add(new BarItem { Value = clashMatrix[2, i, j], Color = OxyColors.Blue });
                    series.Items.Add(new BarItem { Value = clashMatrix[1, i, j], Color = OxyColors.Green });
                    series.Items.Add(new BarItem { Value = clashMatrix[0, i, j], Color = OxyColors.Red });
                    model.Series.Add(series);
                }
            }

            return model;
        }
    }
}
