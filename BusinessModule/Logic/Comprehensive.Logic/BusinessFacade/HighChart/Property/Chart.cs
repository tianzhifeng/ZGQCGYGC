using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;

namespace Comprehensive.Logic
{
    public class Chart
    {
        public string Type
        { get; set; }

        bool _is3D = true;
        public bool Is3D
        { get { return _is3D; } set { _is3D = value; } }

        Dictionary<string, object> _options3D;
        public Dictionary<string, object> options3D
        {
            get
            {
                if (_options3D == null)
                {
                    _options3D = new Dictionary<string, object>();
                    _options3D.SetValue("enabled", true);
                    _options3D.SetValue("alpha", 45);
                    _options3D.SetValue("beta", 0);
                }
                return _options3D;
            }
        }

        public string BackgroundColor
        { get; set; }

        public string BorderColor
        { get; set; }

        public int BorderWidth
        { get; set; }

        public string className
        { get; set; }

        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            result.SetValue("type", Type);
            if (!String.IsNullOrEmpty(BackgroundColor))
                result.SetValue("backgroundColor", BackgroundColor);
            if (!String.IsNullOrEmpty(className))
                result.SetValue("className", className);
            if (!String.IsNullOrEmpty(BorderColor))
                result.SetValue("borderColor", BorderColor);
            if (this.Is3D)
                result.SetValue("options3d", this.options3D);
            return result;
        }
    }
}
