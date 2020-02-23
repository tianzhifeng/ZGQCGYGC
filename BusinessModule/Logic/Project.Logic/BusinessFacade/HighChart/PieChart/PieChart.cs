using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;

namespace Project.Logic
{
    public class PieChart:BaseChart
    {

        PlotOptions _plotOption;
        public override PlotOptions PlotOption
        {
            get
            {
                if (_plotOption == null)
                    _plotOption = new PlotOptions();
                var pieOptions = new Dictionary<string, object>();
                pieOptions.SetValue("allowPointSelect", true);
                pieOptions.SetValue("cursor", "pointer");
                pieOptions.SetValue("depth", 35);

                var dataLabels = new Dictionary<string, object>();
                dataLabels.SetValue("enabled", true);
                dataLabels.SetValue("format", "{point.name}");
                pieOptions.SetValue("dataLabels", dataLabels);

                _plotOption.SetValue("pie", pieOptions);
                return _plotOption;
            }
        }

        public override  Dictionary<string, object> Render()
        {
            var result = new Dictionary<string, object>();
            this.Chart.Type = "pie";
            result.SetValue("chart", this.Chart.ToDic());
            result.SetValue("title", TitleInfo.ToDic());
            if (this.SubTitleInfo != null)
                result.SetValue("subtitle", SubTitleInfo.ToDic());
            var seriesInfos = new List<Dictionary<string, object>>();
            foreach (var item in this.SeriesList)
                seriesInfos.Add(item.ToDic());
            result.SetValue("plotOptions", this.PlotOption.ToDic());
            result.SetValue("series", seriesInfos);
            result.SetValue("credits", new { enabled = false });
            return result;
        }
    }
}
