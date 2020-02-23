using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Logic;
using System.Text.RegularExpressions;
using Formula;

namespace Project.Logic
{
    public  class Function
    {
        public static string ReplaceRegString(string regString, Dictionary<string, object> data, List<Dictionary<string, object>> EnumDefine)
        {
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(regString, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var sValue = data.GetValue(value);
                var define = EnumDefine.FirstOrDefault(d => d["FieldName"].ToString() == value);
                if (define != null)
                {
                    var enumValue = "";
                    var EnumService = FormulaHelper.GetService<IEnumService>();
                    foreach (var item in sValue.Split(','))
                    {
                        enumValue += EnumService.GetEnumText(define.GetValue("EnumKey"), item) + ",";
                    }
                    return enumValue.TrimEnd(',');
                }
                return sValue;
            });
            return result;
        }

    }
}
