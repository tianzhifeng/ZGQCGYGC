using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Config;

namespace DevPivotGrid
{
    public partial class PivotDetail : System.Web.UI.Page
    {
        private bool isFreePivot = false;//是否自由透视表，地址栏传递ID时为自由透视表，传递TmplCode时为透视表

        protected string ColumnsString = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["ID"]) && string.IsNullOrEmpty(Request["TmplCode"]))
                isFreePivot = true;

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = string.Format("select * from S_UI_Pivot where Code='{0}'", Request["TmplCode"]);
            if(isFreePivot)//自由透视表
                sql = string.Format("select * from S_UI_FreePivotInstance where ID='{0}'", Request["ID"]);
            var dt = baseSqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return;
            var defRow = dt.Rows[0];
            var RowItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["RowItems"].ToString());
            var ColumnItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["ColumnItems"].ToString());
            var DataItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["DataItems"].ToString());
            var FilterItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["FilterItems"].ToString() == "" ? "[]" : defRow["FilterItems"].ToString());

            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var item in RowItems)
            {
                if (list.Contains(item["FieldName"]) == false)
                {
                    string align = "left";
                    if (item.ContainsKey("Enum") && item["Enum"] != "")
                        align = "center";
                    sb.AppendFormat("\n<div field='{0}' allowsort='true' headerAlign='center' align='{2}' dateFormat='yyyy-MM-dd' visible='{3}'>{1}</div>"
                        , item["FieldName"], item["Caption"], align, "true");
                    list.Add(item["FieldName"]);
                }
            }

            foreach (var item in ColumnItems)
            {
                if (list.Contains(item["FieldName"]) == false)
                {
                    string align = "left";
                    if (item.ContainsKey("Enum") && item["Enum"] != "")
                        align = "center";
                    sb.AppendFormat("\n<div field='{0}' allowsort='true' headerAlign='center' align='{2}' dateFormat='yyyy-MM-dd' visible='{3}'>{1}</div>"
                        , item["FieldName"], item["Caption"], align, "true");
                    list.Add(item["FieldName"]);
                }
            }

            foreach (var item in DataItems)
            {
                if (list.Contains(item["FieldName"]) == false)
                {
                    string align = "right";
                    if (item["SumType"] == "Count")
                        align = "left";
                    sb.AppendFormat("\n<div field='{0}' allowsort='true' headerAlign='center' align='{2}' dateFormat='yyyy-MM-dd' visible='{3}'>{1}</div>"
                        , item["FieldName"], item["Caption"], align, "true");
                    list.Add(item["FieldName"]);
                }
            }

            foreach (var item in FilterItems)
            {
                if (list.Contains(item["FieldName"]) == false)
                {
                    string align = "left";
                    if (item["CtrlType"] == "date" || item["CtrlType"] == "dropdown")
                        align = "center";
                    else if (item["QueryType"] == ">" || item["QueryType"] == ">=" || item["QueryType"] == "<" || item["QueryType"] == "<=")
                        align = "right";

                    sb.AppendFormat("\n<div field='{0}' allowsort='true' headerAlign='center' align='{2}' dateFormat='yyyy-MM-dd' visible='{3}'>{1}</div>"
                        , item["FieldName"], item["Caption"], align, "true");
                    list.Add(item["FieldName"]);
                }
            }

            ColumnsString = sb.ToString();
        }
    }
}