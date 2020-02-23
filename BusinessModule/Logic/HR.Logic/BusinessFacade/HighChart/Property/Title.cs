using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace HR.Logic
{
    public class Title
    {
        public string Text
        { get; set; }

        public string Align
        { get; set; }

        public bool Foating
        { get; set; }

        public int x
        { get; set; }

        int _margin = 15;
        public int Margin
        { get { return _margin; } set { _margin = value; } }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            result.SetValue("margin", Margin);
            result.SetValue("foating", this.Foating);
            result.SetValue("text", Text);
            if (!String.IsNullOrEmpty(Align))
                result.SetValue("align", Align);
            return result;
        }
    }
}
