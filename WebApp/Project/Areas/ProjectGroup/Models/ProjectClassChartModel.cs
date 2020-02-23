using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.ProjectGroup.Models
{
    public class ProjectClassChartModel
    {
        public List<ChartDataModel> ClassData { get; set; }

        public List<ChartDataModel> AreaData { get; set; }

        public decimal ProjectsCount { get; set; }
        public string ClassUrl { get; set; }
        public string AreaUrl { get; set; }
    }
}