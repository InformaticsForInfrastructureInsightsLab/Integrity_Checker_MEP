using ClashTest2;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP
{
    abstract class DashboardPieChart_Base
    {
        protected PlotModel model;

        public abstract PlotModel getModel(form_ResultViewer rv);
    }

    class DashboardPieChart_Adj : DashboardPieChart_Base
    {
        public override PlotModel getModel(form_ResultViewer rv)
        {
            model = new PlotModel { Title = "Clash Severity" };
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
    }

    class DashboardPieChart_Origin : DashboardPieChart_Base
    {
        public override PlotModel getModel(form_ResultViewer rv)
        {
            model = new PlotModel { Title = "Clash Severity" };
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
            pieSeries.Slices.Add(new PieSlice("Major", rv.major_hard_origin + rv.major_soft_origin) { Fill = OxyColors.Red }); //하드
            pieSeries.Slices.Add(new PieSlice("Medium", rv.medium_hard_origin + rv.medium_soft_origin) { Fill = OxyColors.Green }); //미디움
            pieSeries.Slices.Add(new PieSlice("Minor", rv.minor_hard_origin + rv.minor_soft_origin) { Fill = OxyColors.Blue }); //마이너

            model.Series.Add(pieSeries);
            return model;
        }    
    }
}
