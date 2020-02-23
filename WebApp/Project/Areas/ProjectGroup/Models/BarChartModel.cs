using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.ProjectGroup.Models
{
    public class BarChartModel
    {
        public string Title { get; set; }

        public Dictionary<string,object> MainData { get; set; }

        public Dictionary<string, object> ForceData { get; set; }

        public string Url { get; set; }

    }
}