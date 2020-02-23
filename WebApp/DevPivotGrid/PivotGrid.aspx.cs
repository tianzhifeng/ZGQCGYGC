using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Data.PivotGrid;
using DevExpress.Web.ASPxGridView;
using System.Data;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Config;

namespace DevPivotGrid
{
    public partial class PivotGrid : System.Web.UI.Page
    {
        #region 基本

        protected List<Dictionary<string, string>> userFilterItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> userDataItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> userRowItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> userColumnItems = new List<Dictionary<string, string>>();

        protected List<Dictionary<string, string>> filterItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> dataItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> rowItems = new List<Dictionary<string, string>>();
        protected List<Dictionary<string, string>> columnItems = new List<Dictionary<string, string>>();

        protected string enumHtml = "";

        protected string queryHtml = "";

        private bool isFreePivot = false;//是否自由透视表，地址栏传递ID时为自由透视表，传递TmplCode时为透视表

        protected void Page_Init(object sender, EventArgs e)
        {
            ChineseLanguage.Chinese ch = new ChineseLanguage.Chinese();

            if (!string.IsNullOrEmpty(Request["ID"]) && string.IsNullOrEmpty(Request["TmplCode"]))
                isFreePivot = true;


            string method = Request["method"];
            if (!string.IsNullOrEmpty(method))
            {
                string result = "";
                switch (method)
                {
                    case "doQuery":
                        result = doQuery();
                        break;
                    case "reset":
                        result = doReset();
                        break;
                    case "getDetail":
                        BindData();//getDetail需要先绑定数据
                        result = getDetail();
                        break;
                }
                Response.Clear();
                Response.Write(result);
                Response.End();
            }
            //正常绑定数据          
            BindData();
        }

        //绑定数据，包括设置pivotGrid的行列标签和设置其数据源
        protected void BindData()
        {
            string sql = string.Format("select * from S_UI_Pivot where Code='{0}'", Request["TmplCode"]);
            if (isFreePivot)//自由透视表
                sql = string.Format("select * from S_UI_FreePivotInstance where ID='{0}'", Request["ID"]);

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");

            ASPxPivotGrid1.ClientSideEvents.CellClick = "showDetail";

            var dt = baseSqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return;
            var defRow = dt.Rows[0];
            filterItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["FilterItems"].ToString());
            dataItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["DataItems"].ToString());
            rowItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["RowItems"].ToString());
            columnItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(defRow["ColumnItems"].ToString());

            if (filterItems == null)
                filterItems = new List<Dictionary<string, string>>();
            if (dataItems == null)
                dataItems = new List<Dictionary<string, string>>();
            if (rowItems == null)
                rowItems = new List<Dictionary<string, string>>();
            if (columnItems == null)
                columnItems = new List<Dictionary<string, string>>();


            sql = string.Format("select * from S_UI_PivotUser where PivotID='{0}' and UserID='{1}'", defRow["ID"], HttpContext.Current.User.Identity.Name);
            if (isFreePivot)//自由透视表
                sql = string.Format("select * from S_UI_FreePivotInstanceUser where FreePivotInstanceID='{0}' and UserID='{1}'", defRow["ID"], HttpContext.Current.User.Identity.Name);

            var dtUser = baseSqlHelper.ExecuteDataTable(sql);


            if (dtUser.Rows.Count > 0)
            {
                var row = dtUser.Rows[0];
                userFilterItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(row["FilterItems"].ToString());
                userDataItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(row["DataItems"].ToString());
                userRowItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(row["RowItems"].ToString());
                userColumnItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(row["ColumnItems"].ToString());
            }
            else
            {
                userFilterItems = filterItems.Where(c => c["Visible"] == "1").ToList();
                userDataItems = dataItems.Where(c => c["Visible"] == "1").ToList();
                userRowItems = rowItems.Where(c => c["Visible"] == "1").ToList();
                userColumnItems = columnItems.Where(c => c["Visible"] == "1").ToList();
            }

            if (userFilterItems == null)
                userFilterItems = new List<Dictionary<string, string>>();
            if (userDataItems == null)
                userDataItems = new List<Dictionary<string, string>>();
            if (userRowItems == null)
                userRowItems = new List<Dictionary<string, string>>();
            if (userColumnItems == null)
                userColumnItems = new List<Dictionary<string, string>>();


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table width='100%'>");
            for (int i = 0; i < userFilterItems.Count; i++)
            {
                string ctrl = "";

                string fieldName = userFilterItems[i]["FieldName"];
                string fieldValue = "";
                if (userFilterItems[i].ContainsKey("FieldValue"))
                    fieldValue = userFilterItems[i]["FieldValue"];

                string filterType = "";
                if (userFilterItems[i]["CtrlType"] == "text")
                {
                    ctrl = string.Format("<input id='v_{0}' class='mini-textbox' style='width: 100%' value='{1}'/>", i, fieldValue);
                    filterType = string.Format("<input id='f_{0}' value='{1}' class='mini-combobox' style='width: 100%' multiselect='false' valuefield='value' textfield='text' data='textQueryType'/>", i, userFilterItems[i]["QueryType"]);
                }
                else if (userFilterItems[i]["CtrlType"] == "date")
                {
                    string v1 = "";
                    string v2 = "";
                    if (string.IsNullOrEmpty(fieldValue) || !fieldValue.Contains(','))
                    {
                        v1 = fieldValue;
                        v2 = fieldValue;
                    }
                    else
                    {
                        v1 = fieldValue.Split(',')[0];
                        v2 = fieldValue.Split(',')[1];
                    }

                    ctrl = string.Format(@"
<input id='v_{0}' class='mini-datepicker' style='width: 45%' value='{1}'/>
->
<input id='v1_{0}' class='mini-datepicker' style='width: 45%' value='{2}'/>
", i, v1, v2);
                    filterType = string.Format("<input id='f_{0}' value='{1}' class='mini-combobox' style='width: 100%' multiselect='false' valuefield='value' textfield='text' data='dateQueryType' onvaluechanged='onFilterTypeChanged(this);'/>", i, userFilterItems[i]["QueryType"]);
                }
                else if (userFilterItems[i]["CtrlType"] == "dropdown")
                {
                    ctrl = string.Format("<input id='v_{0}' class='mini-combobox' multiselect='true' valuefield='value' textfield='text' style='width: 100%' value='{1}' data='enum_{2}'/>", i, fieldValue, fieldName);
                    filterType = string.Format("<input id='f_{0}' value='{1}' class='mini-combobox' style='width: 100%' multiselect='false' valuefield='value' textfield='text' data='dropdownQueryType'/>", i, userFilterItems[i]["QueryType"]);
                }

                if (i % 2 == 0)
                    sb.AppendFormat("\n<tr>");
                sb.AppendFormat(@"
<td width='10%' align='right'>{0}</td>
<td width='10%'>{1}</td>
<td width='30%'>{2}</td>
", userFilterItems[i]["Caption"]
 , filterType
 , ctrl
 );
                if (i % 2 == 0 && i == userFilterItems.Count - 1)
                {
                    sb.AppendFormat("<td width='50%' colspan='3'></td>");
                    sb.AppendFormat("</tr>");
                }
                else if (i % 2 == 1)
                {
                    sb.AppendFormat("</tr>");
                }
            }
            sb.AppendFormat("</table>");
            queryHtml = sb.ToString();

            setFields(userFilterItems, userDataItems, userRowItems, userColumnItems);
            LoadData(userFilterItems);
        }

        private void LoadData(List<Dictionary<string, string>> filters)
        {
            string sql = string.Format("select * from S_UI_Pivot where Code='{0}'", Request["TmplCode"]);
            if (isFreePivot)//自由透视表
                sql = string.Format("select S_UI_FreePivotInstance.*,ConnName from S_UI_FreePivotInstance join S_UI_FreePivot on S_UI_FreePivot.ID=FreePivotID where S_UI_FreePivotInstance.ID='{0}'", Request["ID"]);

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            var dt = baseSqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0) //找不到定义
                return;
            var row = dt.Rows[0];
            var sqlHelper = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());
            sql = row["SQL"].ToString();

            string str = "";
            foreach (var item in filters)
            {
                string value = item.Keys.Contains("FieldValue") ? item["FieldValue"].Replace("'", "") : "";
                string queryType = item.Keys.Contains("QueryType") && !string.IsNullOrWhiteSpace(item["QueryType"]) ? item["QueryType"] : "in";
                if (!string.IsNullOrEmpty(value))
                {
                    if (queryType == "in")
                        str += string.Format(" and {0} in('{1}')", item["FieldName"], value.Replace(",", "','"));
                    else if (queryType == "like")
                        str += string.Format(" and {0} like '%{1}%'", item["FieldName"], value);
                    else if (queryType.StartsWith("between"))
                    {
                        if (!value.Contains(','))
                            value += ",";
                        string v1 = value.Split(',')[0];
                        string v2 = value.Split(',')[1];
                        DateTime t1 = new DateTime(1900, 1, 1);
                        DateTime t2 = new DateTime(9999, 12, 31);
                        if (DateTime.TryParse(v1, out t1) || DateTime.TryParse(v2, out t2))
                            str += string.Format(" and {0} between '{1}' and '{2} 23:59:59'", item["FieldName"], t1.ToString("yyyy-MM-dd"), t2.ToString("yyyy-MM-dd"));
                    }
                    else
                        str += string.Format(" and {0} {1} '{2}'", item["FieldName"], item["QueryType"], value);
                }
            }
            if (Config.Constant.IsOracleDb)
                sql = string.Format("select * from ({0}) tb where 1=1 {1}", sql, str);
            else
                sql = string.Format("select * from ({0}) as tb where 1=1 {1}", sql, str);


            DataTable result = sqlHelper.ExecuteDataTable(sql);

            enumHtml = "<script type='text/javascript'>";


            List<Dictionary<string, string>> fields = new List<Dictionary<string, string>>();
            fields.AddRange(userRowItems);
            fields.AddRange(userColumnItems);
            fields.AddRange(userDataItems);
            fields.AddRange(userFilterItems);
            foreach (var item in fields)
            {
                if (!item.ContainsKey("Enum"))
                    continue;

                if (string.IsNullOrEmpty(item["Enum"]))
                    continue;

                string strEnum = "";
                if (item["Enum"].StartsWith("["))
                    strEnum = item["Enum"];
                else
                {
                    dt = baseSqlHelper.ExecuteDataTable(
                          string.Format("select S_M_EnumItem.Code as value,S_M_EnumItem.Name as text from S_M_EnumItem join S_M_EnumDef on S_M_EnumDef.ID=EnumDefID where S_M_EnumDef.Code='{0}'", item["Enum"]));
                    strEnum = JsonHelper.ToJson(dt);
                }

                enumHtml += string.Format(@"
var enum_{0} = {1};
", item["FieldName"], strEnum);


                if (item["Enum"] == "[]")
                    continue;

                var enumList = JsonHelper.ToList(strEnum);

                result.Columns.Add(item["FieldName"] + "_");
                foreach (DataRow row1 in result.Rows)
                {
                    row1[item["FieldName"] + "_"] = GetEnumText(enumList, row1[item["FieldName"]].ToString());
                }
                result.Columns.Remove(item["FieldName"]);
                result.Columns[item["FieldName"] + "_"].ColumnName = item["FieldName"];
            }

            enumHtml += @"
</script>";

            ASPxPivotGrid1.DataSource = result;
            ASPxPivotGrid1.DataBind();
        }

        private DataTable caculateDT = new DataTable();//用于表达式计算
        private string GetEnumText(List<Dictionary<string, object>> enumList, string value)
        {
            List<string> result = new List<string>();
            foreach (var item in enumList)
            {
                var enumItemValue = item["value"].ToString();
                if (enumItemValue.Contains("{0}") && caculateDT.Compute(string.Format(enumItemValue, value), "").ToString() == "True")
                {
                    return item["text"].ToString();
                }
                else if(value.Split(',').Contains(enumItemValue))
                {
                    result.Add(item["text"].ToString());
                }
            }

            if (result.Count == 0)
                return value;
            else
                return string.Join(",", result);
        }

        protected void setFields(List<Dictionary<string, string>> FilterItems, List<Dictionary<string, string>> DataItems, List<Dictionary<string, string>> RowItems, List<Dictionary<string, string>> ColumnItems)
        {
            ASPxPivotGrid1.Fields.Clear();
            foreach (var item in FilterItems)
            {
                var field = ASPxPivotGrid1.Fields.Add();
                field.Area = PivotArea.FilterArea;
                field.FieldName = item["FieldName"];
                field.Caption = item["Caption"];
            }

            foreach (var item in RowItems)
            {
                var field = ASPxPivotGrid1.Fields.Add();
                field.Area = PivotArea.RowArea;
                field.FieldName = item["FieldName"];
                field.Caption = item["Caption"];
            }
            foreach (var item in ColumnItems)
            {
                var field = ASPxPivotGrid1.Fields.Add();
                field.Area = PivotArea.ColumnArea;
                field.FieldName = item["FieldName"];
                field.Caption = item["Caption"];
            }

            foreach (var item in DataItems)
            {
                var field = ASPxPivotGrid1.Fields.Add();
                field.Area = PivotArea.DataArea;
                field.FieldName = item["FieldName"];
                field.Caption = item["Caption"];
                field.AllowedAreas = PivotGridAllowedAreas.DataArea;
                field.SummaryType = (PivotSummaryType)Enum.Parse(typeof(PivotSummaryType), item["SumType"].ToString(), true);
                field.CellFormat.FormatString = "{0}";
            }

        }


        protected string doQuery()
        {
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            var dt = baseSqlHelper.ExecuteDataTable(string.Format("select * from S_UI_Pivot where Code='{0}'", Request["TmplCode"]));
            if (isFreePivot)//自由透视表
                dt = baseSqlHelper.ExecuteDataTable(string.Format("select * from S_UI_FreePivotInstance where ID='{0}'", Request["ID"]));
            if (dt.Rows.Count == 0)
                return "";
            var row = dt.Rows[0];
            if (Config.Constant.IsOracleDb)
            {
                if (isFreePivot)
                {
                    baseSqlHelper.ExecuteNonQuery(string.Format("delete S_UI_FreePivotInstanceUser where FreePivotInstanceID='{0}' and UserID='{1}'", row["ID"], HttpContext.Current.User.Identity.Name));
                    string sql = "insert into S_UI_FreePivotInstanceUser(ID,FreePivotInstanceID,UserID,RowItems,ColumnItems,DataItems,FilterItems) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                    sql = string.Format(sql, GuidHelper.CreateGuid(), row["ID"], HttpContext.Current.User.Identity.Name, Request["RowItems"], Request["ColumnItems"], Request["DataItems"], Request["FilterItems"]);
                    baseSqlHelper.ExecuteNonQuery(sql);
                }
                else
                {
                    baseSqlHelper.ExecuteNonQuery(string.Format("delete S_UI_PivotUser where PivotID='{0}' and UserID='{1}'", row["ID"], HttpContext.Current.User.Identity.Name));
                    string sql = "insert into S_UI_PivotUser(ID,PivotID,UserID,RowItems,ColumnItems,DataItems,FilterItems) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                    sql = string.Format(sql, GuidHelper.CreateGuid(), row["ID"], HttpContext.Current.User.Identity.Name, Request["RowItems"], Request["ColumnItems"], Request["DataItems"], Request["FilterItems"]);
                    baseSqlHelper.ExecuteNonQuery(sql);
                }
            }
            else
            {
                if (isFreePivot)
                {
                    string sql = @"
if EXISTS(select * from S_UI_FreePivotInstanceUser where FreePivotInstanceID='{1}' and UserID='{2}')
    update S_UI_FreePivotInstanceUser set RowItems='{3}',ColumnItems='{4}',DataItems='{5}',FilterItems='{6}' where FreePivotInstanceID='{1}' and UserID='{2}'
else
    insert into S_UI_FreePivotInstanceUser(ID,FreePivotInstanceID,UserID,RowItems,ColumnItems,DataItems,FilterItems) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')
";
                    sql = string.Format(sql, GuidHelper.CreateGuid(), row["ID"], HttpContext.Current.User.Identity.Name, Request["RowItems"], Request["ColumnItems"], Request["DataItems"], Request["FilterItems"]);
                    baseSqlHelper.ExecuteNonQuery(sql);
                }
                else
                {
                    string sql = @"
if EXISTS(select * from S_UI_PivotUser where PivotID='{1}' and UserID='{2}')
    update S_UI_PivotUser set RowItems='{3}',ColumnItems='{4}',DataItems='{5}',FilterItems='{6}' where PivotID='{1}' and UserID='{2}'
else
    insert into S_UI_PivotUser(ID,PivotID,UserID,RowItems,ColumnItems,DataItems,FilterItems) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')
";
                    sql = string.Format(sql, GuidHelper.CreateGuid(), row["ID"], HttpContext.Current.User.Identity.Name, Request["RowItems"], Request["ColumnItems"], Request["DataItems"], Request["FilterItems"]);
                    baseSqlHelper.ExecuteNonQuery(sql);
                }
            }
            return "ok";
        }

        protected string doReset()
        {
            if (isFreePivot)//自由透视表
            {
                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
                var dt = baseSqlHelper.ExecuteDataTable(string.Format("select * from S_UI_FreePivotInstance where ID='{0}'", Request["ID"]));
                if (dt.Rows.Count == 0)
                    return "";
                var row = dt.Rows[0];
                string sql = @"delete from S_UI_FreePivotInstanceUser where FreePivotInstanceID='{0}' and UserID='{1}'";
                sql = string.Format(sql, row["ID"], HttpContext.Current.User.Identity.Name);
                baseSqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
                var dt = baseSqlHelper.ExecuteDataTable(string.Format("select * from S_UI_Pivot where Code='{0}'", Request["TmplCode"]));
                if (dt.Rows.Count == 0)
                    return "";
                var row = dt.Rows[0];
                string sql = @"delete from S_UI_PivotUser where PivotID='{0}' and UserID='{1}'";
                sql = string.Format(sql, row["ID"], HttpContext.Current.User.Identity.Name);
                baseSqlHelper.ExecuteNonQuery(sql);
            }
            return "ok";
        }

        //获取点击单元格的明细数据
        protected string getDetail()
        {
            int rowIndex = int.Parse(Request["rowIndex"]);
            int colIndex = int.Parse(Request["colIndex"]);
            int pageSize = int.Parse(Request["pageSize"]);
            int pageIndex = int.Parse(Request["pageIndex"]);
            string sortField = Request["sortField"];
            string sortOrder = Request["SortOrder"];

            DataTable dt = ASPxPivotGrid1.DataSource as DataTable;
            PivotDrillDownDataSource source = ASPxPivotGrid1.CreateDrillDownDataSource(colIndex, rowIndex);

            List<int> list = new List<int>();
            for (int i = 0; i < source.RowCount; i++)
            {
                list.Add(source[i].ListSourceRowIndex);
            }

            var result = dt.Copy();
            for (int i = result.Rows.Count - 1; i >= 0; i--)
            {
                if (list.Contains(i) == false)
                    result.Rows.RemoveAt(i);
            }

            if (!string.IsNullOrEmpty(sortField))
            {
                if (sortOrder == "asc")
                {
                    result = result.AsEnumerable().OrderBy(c => c[sortField]).CopyToDataTable();
                }
                else
                {
                    result = result.AsEnumerable().OrderByDescending(c => c[sortField]).CopyToDataTable();
                }
            }

            int total = result.Rows.Count;
            for (int i = result.Rows.Count - 1; i >= 0; i--)
            {
                if (i < pageIndex * pageSize || i >= (pageIndex + 1) * pageSize)
                    result.Rows.RemoveAt(i);
            }

            IsoDateTimeConverter datetype = new IsoDateTimeConverter();
            datetype.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

            var objResult = new { total = total, data = result };
            return JsonConvert.SerializeObject(objResult, datetype);
        }

        #endregion

        protected override void OnError(EventArgs e)
        {
            //报错时清空个性化设置
            doReset();

            base.OnError(e);
        }
    }
}