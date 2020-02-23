using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.ProjectGroup.Models
{
    public class ProjectReceivedModel
    {
        public decimal ReceivedRate { get; set; }

        public decimal ReceivedAmount { get; set; }

        public Dictionary<string,decimal> DataList { get; set; }
    }
}