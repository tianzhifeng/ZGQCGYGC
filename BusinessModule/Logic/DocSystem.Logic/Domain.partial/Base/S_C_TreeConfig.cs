using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Config.Logic;
using Formula;
using Formula.Helper;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_TreeConfig
    {

        public string GetDisplayStr()
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(this.TreeDisplay))
                result = " * ," + this.TreeDisplay + " as DisplayName";
            else
                result = " * ,Name as DisplayName";
            return result;
        }

        public string GetOrderByStr()
        {
            string result = " order by ";
            if (String.IsNullOrEmpty(this.TreeSort))
                result += "  ID asc ";
            else
            {
                var arrayList = JsonHelper.ToList(this.TreeSort); 
                foreach (var item in arrayList)
                {
                    string field = item.GetValue("SortField");
                    string dir = item.GetValue("SortDir");
                    result += " " + field + " " + dir + " ,";
                }
                result = result.TrimEnd(',');
            }
            return result;
        }
    }
}
