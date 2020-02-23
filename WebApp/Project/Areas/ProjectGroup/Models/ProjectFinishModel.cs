using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.ProjectGroup.Models
{
    public class ProjectFinishModel
    {
        public string ProjectClass { get; set; }

        public int ThisMonth { get; set; }

        public int ThisSeason { get; set; }
        public int ThisYear { get; set; }

        public int Total { get; set; }


    }
}