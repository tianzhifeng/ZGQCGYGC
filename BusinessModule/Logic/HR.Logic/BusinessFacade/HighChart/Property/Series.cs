using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config.Logic;
using Config;
using System.Collections;

namespace HR.Logic
{
    public class Series
    {
        public string Name
        { get; set; }

        public string Type
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


        public Dictionary<string, object> ToDic()
        {
            var result = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(Name))
                result.SetValue("name", Name);
            if (!String.IsNullOrEmpty(Type))
                result.SetValue("type", Type);
            result.SetValue("data", Data);
            return result;
        }
    }
}
