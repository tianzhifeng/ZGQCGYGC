using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// ReSharper disable InconsistentNaming

namespace Project.Areas.ProjectGroup.Models
{
    public class ChartDataModel
    {
        public string name { get; set; }

        public decimal y { get; set; }

        public string color { get; set; }
    }
}