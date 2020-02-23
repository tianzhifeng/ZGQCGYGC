using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Formula;
using MvcAdapter;
using Config.Logic;

namespace DocInstance.Logic
{
    public class DocInstanceHepler
    {
        public static string GetBtnHTML(string canBorrow, string canDownload)
        {
            string returnHTML = string.Empty;
            string borrowHTML = "<td style=\"width:100px;\" id=\"BorrowCar\">" +
        "<a class=\"mini-button\" onclick=\"openBorrowCar\" iconcls=\"icon-extend-car\" style=\"width: 100px;\"  plain=\"true\">" +
                    "借阅车(<span id=\"borrowNo\" style=\"color:Red\"></span>)</a></td>";

            string downloadHTML = "<td style=\"width:100px;\" id=\"DownCar\">" +
        "<a class=\"mini-button\" onclick=\"openDownloadCar\" iconcls=\"icon-extend-car\" style=\"width: 100px;\"  plain=\"true\">" +
                    "下载车(<span id=\"downNo\" style=\"color:Red\"></span>)</a></td>";
            if (canBorrow == "True")
                returnHTML += borrowHTML;
            if (canDownload == "True")
                returnHTML += downloadHTML;
            return returnHTML;
        }
        
        //高级查询
        public static void QueryBuilderExtend(QueryBuilder qb)
        {
            var rtn = new QueryBuilder();

            //逗号为或，空格为与
            foreach (var item in qb.Items)
            {
                if (item.Method == QueryMethod.InLike && item.Method == QueryMethod.In)
                {
                    rtn.Add(item.Field, item.Method, item.Value, item.OrGroup, item.BaseOrGroup);
                    continue;
                }
                var value = item.Value.ToString().Trim().Replace('，', ',');
                if (value.Contains(','))
                {
                    foreach (var val in value.Split(','))
                    {
                        rtn.Add(item.Field, item.Method, val.Trim(), item.OrGroup + "_" + item.Field, item.BaseOrGroup);
                    }
                }
                else if (value.Contains(' '))
                {
                    for (int i = 0; i < value.Split(' ').Length; i++)
                    {
                        var val = value.Split(' ')[i];
                        rtn.Add(item.Field, item.Method, val.Trim(), item.OrGroup + "_" + item.Field + "_" + i.ToString(), item.BaseOrGroup);
                    }
                }
                else
                    rtn.Add(item.Field, item.Method, item.Value, item.OrGroup, item.BaseOrGroup);
            }
            qb.Items.Clear();
            qb.Items.AddRange(rtn.Items);
        }

        public static void QueryBuilderExtend(QueryBuilder qb, string queryNode, List<Dictionary<string, object>> queryList)
        {
            foreach (var item in queryList)
            {
                var value = item.GetValue("Value");
                if (!String.IsNullOrEmpty(value))
                {
                    var typeValue = item.GetValue("TypeValue");
                    var method = item.GetValue("Method");
                    method = SearchMethodAdapter(method);
                    var attrField = item.GetValue("AttrField");
                    var type = item.GetValue("Type");
                    var group = item.GetValue("Group");
                    var configType = item.GetValue("AttrType");
                    var key = "";
                    if (string.IsNullOrEmpty(queryNode))
                    {
                        if (configType == DocSystem.Logic.Domain.ListConfigType.File.ToString())
                            key = attrField;
                        else
                            key = type + "_" + attrField;
                    }
                    else
                    {
                        if (queryNode == typeValue)
                            key = attrField;
                        else
                            key = type + "_" + attrField;
                    }
                    if (method == "GreaterThan" || method == "GreaterThanOrEqual")
                    {
                        DateTime time;
                        float f;
                        if (DateTime.TryParse(value, out time) && float.TryParse(value, out f) == false)  //例如3.1会被认为是3月1号
                        {
                            value = time.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }

                    if (method == "LessThan" || method == "LessThanOrEqual")
                    {
                        DateTime time;
                        float f;
                        if (DateTime.TryParse(value, out time) && float.TryParse(value, out f) == false)  //例如3.1会被认为是3月1号
                        {
                            value = time.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    qb.Add(key, (QueryMethod)Enum.Parse(typeof(QueryMethod), method), value, "", group);
                }
            }
            QueryBuilderExtend(qb);
        }

        private static string SearchMethodAdapter(string method)
        {
            string match = "";
            switch (method.ToUpper())
            {
                case "EQ"://等于
                    match = "Equal";
                    break;
                case "UE"://不等于
                    match = "NotEqual";
                    break;
                case "GT"://大于
                    match = "GreaterThan";
                    break;
                case "LT"://小于
                    match = "LessThan";
                    break;
                case "IN"://in
                    match = "In";
                    break;
                case "FR"://大于等于
                    match = "GreaterThanOrEqual";
                    break;
                case "TO"://小于等于
                    match = "LessThanOrEqual";
                    break;
                case "LK"://like
                    match = "Like";
                    break;
                case "IL":
                    match = "InLike";
                    break;
                case "SW"://以....开始
                    match = "StartsWith";
                    break;
                case "EW"://以....结束
                    match = "EndsWith";
                    break;
                case "IGNORE":
                    match = "";
                    break;
            }
            return match;
        }
    }
}
