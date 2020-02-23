using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;
using System.Collections;

namespace Project.Logic
{
    public class Series
    {

        public string Color
        { get; set; }

        public string Name
        { get; set; }

        public string Field
        { get; set; }

        public string Type
        { get; set; }

        public int? yAxis
        { get; set; }

        ArrayList _data;
        public ArrayList Data
        {
            get
            {
                if (_data == null)
                    _data = new ArrayList();
                return _data;
            }
            set {
                _data = value;
            }
        }

        public Dictionary<string, object> Tooltip
        {
            get;
            set;
        }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(Name))
                result.SetValue("name", Name);
            if (!String.IsNullOrEmpty(Color))
                result.SetValue("color", Color);
            if (!String.IsNullOrEmpty(Type))
                result.SetValue("type", Type);
            if (yAxis != null)
                result.SetValue("yAxis", yAxis);
            result.SetValue("data", Data);
            if (Tooltip != null)
                result.SetValue("tooltip", Tooltip);
            return result;
        }
    }
}
