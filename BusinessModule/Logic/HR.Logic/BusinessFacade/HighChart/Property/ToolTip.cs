using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace HR.Logic
{
    public class ToolTip
    {
        public string _pointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>";
        public string PointFormat
        {
            get { return _pointFormat; }
            set { _pointFormat = value; }
        }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            result.SetValue("pointFormat", PointFormat);
            return result;
        }
    }
}
