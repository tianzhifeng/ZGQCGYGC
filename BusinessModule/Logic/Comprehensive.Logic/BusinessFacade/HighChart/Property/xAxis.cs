using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace Comprehensive.Logic
{
    public class xAxis
    {
        public string[] Categories
        {
            get;
            set;
        }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            result.SetValue("categories", Categories);
            return result;
        }
    }
}
