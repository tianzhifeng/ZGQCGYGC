using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace Project.Logic
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

        public Dictionary<string, object> Lable
        {
            get;
            set;
        }

        bool _opposite = false;
        public bool opposite
        { get { return _opposite; } set { _opposite = value; } }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            if (this.TitleInfo != null)
                result.SetValue("title", this.TitleInfo);
            if (this.Lable != null)
                result.SetValue("labels", this.Lable);
            result.SetValue("min", this.MiniValue);
            result.SetValue("opposite", this.opposite);
            return result;
        }
    }
}
