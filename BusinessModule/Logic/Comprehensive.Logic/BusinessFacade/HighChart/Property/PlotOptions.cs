using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace Comprehensive.Logic
{
    public class PlotOptions
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        public void SetValue(string key,object value)
        {
            result.SetValue(key, value);
        }

        public Dictionary<string, object> ToDic()
        {
            return result;
        }
    }
}
