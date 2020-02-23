using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace HR.Logic
{
    public class yAxis
    {
        public int MiniValue
        {
            get;
            set;
        }

        public Dictionary<string, object> TitleInfo
        {
            get;
            set;
        }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            result.SetValue("title",this.TitleInfo);
            result.SetValue("min", this.MiniValue);
            return result;
        }
    }
}
