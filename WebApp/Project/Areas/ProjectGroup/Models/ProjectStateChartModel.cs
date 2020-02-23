using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Formula.Helper;

namespace Project.Areas.ProjectGroup.Models
{
    public class ProjectStateChartModel
    {
        public string Name { get; set; }

        public List<ChartDataModel>  Data { get; set; }

        public string Json
        {
            get { return JsonHelper.ToJson(Data); }
        }

        public decimal Count
        {
            get { return Data.Sum(t => t.y); }
        }

        public string Url { get; set; }
    }
}