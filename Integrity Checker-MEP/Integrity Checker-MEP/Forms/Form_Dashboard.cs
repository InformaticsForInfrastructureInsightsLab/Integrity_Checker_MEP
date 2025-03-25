using System;
using System.Windows.Forms;
using ClashTest2;
using OxyPlot.WindowsForms;

namespace Integrity_Checker_MEP.Forms
{
    public partial class Form_Dashboard : Form
    {
        form_ResultViewer rv = MainClass.rv;

        DashboardBarchart_Adj bar_adj;
        DashboardBarchart_Origin bar_origin;

        DashboardPieChart_Adj pie_adj;
        DashboardPieChart_Origin pie_origin;

        public Form_Dashboard()
        {
            InitializeComponent();
            DashboardTab();
            AdjustmentTab();
        }

        private void DashboardTab()
        {
            bar_adj = new DashboardBarchart_Adj(rv);
            bar_origin = new DashboardBarchart_Origin(rv);
            pie_adj = new DashboardPieChart_Adj();
            pie_origin = new DashboardPieChart_Origin();

            var piePlot = new PlotView
            {
                Model = pie_adj.getModel(rv),
                Dock = DockStyle.Fill
            };

            var barPlot = new PlotView
            {
                Model = bar_adj.getModel(),
                Dock = DockStyle.Fill
            };

            tableLayout.Controls.Add(piePlot, 0, 0);
            tableLayout.Controls.Add(barPlot, 1, 0);

            this.tabDashboard.Controls.Add(tableLayout);
        }

        private void RadioChanged(object sender, EventArgs e)
        {
            tableLayout.Controls.Clear();
            if (radio_origin.Checked)
            {
                var piePlot = new PlotView
                {
                    Model = pie_origin.getModel(rv),
                    Dock = DockStyle.Fill
                };
                var barPlot = new PlotView
                {
                    Model = bar_origin.getModel(),
                    Dock = DockStyle.Fill
                };
                tableLayout.Controls.Add(piePlot, 0, 0);
                tableLayout.Controls.Add(barPlot, 1, 0);
            }
            else
            {
                var piePlot = new PlotView
                {
                    Model = pie_adj.getModel(rv),
                    Dock = DockStyle.Fill
                };
                var barPlot = new PlotView
                {
                    Model = bar_adj.getModel(),
                    Dock = DockStyle.Fill
                };
                tableLayout.Controls.Add(piePlot, 0, 0);
                tableLayout.Controls.Add(barPlot, 1, 0);
            }
        }

        private void AdjustmentTab()
        {
            folv.SetObjects(MainClass.rv.dataList);
        }
    }
}
