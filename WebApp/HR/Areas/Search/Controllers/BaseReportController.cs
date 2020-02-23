using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula.Helper;
using System.Text;
using System.Data;
using Formula.ImportExport;
using MvcAdapter.ImportExport;
using Newtonsoft.Json;
using System.IO;
using Aspose.Cells;
using Formula.Exceptions;

namespace HR.Areas.Search.Controllers
{
    public abstract class BaseReportController : Controller
    {
        protected abstract string title { get; }
        protected abstract SQLHelper sqlHelper { get; }
        protected abstract string mainTable { get; }
        protected abstract string fk { get; }

        #region ReportList

        public virtual ActionResult ReportList()
        {
            return View();
        }

        public virtual ActionResult ReportSeacher()
        {
            return View();
        }

        public virtual ActionResult ReportSettings()
        {
            return View();
        }

        public virtual string GetReportList(QueryBuilder qb)
        {
            var dt = getReportData(qb);
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            var json = JsonHelper.ToJson(data);
            return json;
        }

        public virtual ActionResult ExportExcel(string jsonColumns, string reportSettings, QueryBuilder qb)
        {
            var columns = JsonConvert.DeserializeObject<List<ColumnInfo>>(jsonColumns);
            HttpContext.Items["__ColumnInfo"] = columns;
            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = exporter.ParseTemplate(columns, mainTable, title);

            qb.PageSize = int.MaxValue;
            var dt = getReportData(qb);
            dt.TableName = mainTable;

            var buffer = exporter.Export(dt, templateBuffer);

            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");

        }

        #endregion

        #region GetEnumData

        public virtual string GetEnumData(string tableName, string fieldCode)
        {
            string sql = string.Format("select distinct {1} as value,{1} as text from {0}", tableName, fieldCode);

            SQLHelper sqlhelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            DataTable data = sqlhelper.ExecuteDataTable(sql);

            foreach (DataRow row in data.Rows)
            {
                if (row["text"] is DBNull)
                {
                    row["text"] = "NULL";
                    continue;
                }
                else if (row["text"].ToString() == "")
                {
                    row["text"] = "空";
                    continue;
                }

                string text = row["text"].ToString();
                string result = "";

                bool flag = text[0] > '0' && text[0] <= '9';
                for (int i = 0; i < text.Length; i++)
                {
                    var c = text[i];
                    if (c == '.')
                    {
                        result += "点";
                    }
                    else if (",".Contains(c))
                    {
                        throw new BusinessException("分组字段中不能包含逗号！");
                    }
                    else if ("()（）。、；,;".Contains(c))
                    {
                    }
                    else
                    {
                        if (flag)
                        {
                            switch (c)
                            {
                                case '0':
                                    result += "零";
                                    break;
                                case '1':
                                    result += "一";
                                    break;
                                case '2':
                                    result += "二";
                                    break;
                                case '3':
                                    result += "三";
                                    break;
                                case '4':
                                    result += "四";
                                    break;
                                case '5':
                                    result += "五";
                                    break;
                                case '6':
                                    result += "六";
                                    break;
                                case '7':
                                    result += "七";
                                    break;
                                case '8':
                                    result = "八";
                                    break;
                                case '9':
                                    result += "九";
                                    break;
                                default:
                                    result += c;
                                    break;
                            }
                        }
                        else
                        {
                            result += c;
                        }
                    }
                }
                row["text"] = result.Replace(" ", "");
            }

            var json = JsonHelper.ToJson(data);
            return json;
        }

        #endregion

        #region 私有方法

        protected DataTable getListData(string sql, QueryBuilder qb)
        {
            string queryData = Request["queryData"];


            if (!string.IsNullOrEmpty(queryData))
            {
                var list = JsonHelper.ToList(queryData);
                var tableNames = list.Where(c => c["table"].ToString() != mainTable).Select(c => c["table"].ToString()).Distinct().ToArray();
                StringBuilder sb = new StringBuilder();
                sb.Append(sql);
                sb.AppendLine();
                foreach (var tableName in tableNames)
                {
                    string tableSql = tableName;
                    string tName = tableName.Trim();
                    if (tableName.Contains(' '))
                        tName = tableName.Split(' ').Last();

                    sb.AppendFormat(" join {0} on {2}.ID={1}.{3}", tableName, tName, mainTable.Split(' ').Last(), fk);
                    var searchList = list.Where(c => c["table"].ToString() == tableName).ToArray();
                    foreach (var item in searchList)
                    {
                        string fValue = "";
                        if (item.ContainsKey("fieldValue"))
                            fValue = item["fieldValue"].ToString();

                        if (item["queryType"].ToString() == "like")
                            sb.AppendFormat(" and {0}.{1} {2} '%{3}%'", tName, item["fieldCode"], item["queryType"], fValue);
                        else if (item["queryType"].ToString() != "in")
                            sb.AppendFormat(" and {0}.{1} {2} '{3}'", tName, item["fieldCode"], item["queryType"], fValue);
                        else
                            sb.AppendFormat(" and {0}.{1} in('{2}')", tName, item["fieldCode"], fValue.ToString().Replace(",", "','"));
                    }
                    sb.AppendLine();
                }

                sb.AppendFormat(" where 1=1");
                var employeeSearchList = list.Where(c => c["table"].ToString() == mainTable);
                foreach (var item in employeeSearchList)
                {
                    string fValue = "";
                    if (item.ContainsKey("fieldValue"))
                        fValue = item["fieldValue"].ToString();
                    if (string.IsNullOrEmpty(fValue) && item.ContainsKey("fieldValueText"))
                        fValue = item["fieldValueText"].ToString();

                    if (item["queryType"].ToString() == "like")
                        sb.AppendFormat(" and {3}.{0} {1} '%{2}%'", item["fieldCode"], item["queryType"], fValue, mainTable);
                    else if (item["queryType"].ToString() != "in")
                        sb.AppendFormat(" and {3}.{0} {1} '{2}'", item["fieldCode"], item["queryType"], fValue, mainTable);
                    else
                        sb.AppendFormat(" and {2}.{0} in('{1}')", item["fieldCode"], fValue.ToString().Replace(",", "','"), mainTable);
                }

                sql = sb.ToString();
            }

            return sqlHelper.ExecuteDataTable(sql, qb);
        }

        protected virtual DataTable getReportData(QueryBuilder qb)
        {

            string reportSettings = Request["reportSettings"];
            string sql = "";
            if (string.IsNullOrEmpty(reportSettings))
                throw new BusinessException("缺少参数reportSettings！");


            List<Dictionary<string, string>> list = JsonHelper.ToObject<List<Dictionary<string, string>>>(reportSettings);

            if (list.Where(c => c["table"] == mainTable).Count() > 0)
            {
                sql = string.Format("select distinct {1}.ID,{2} from {0} ", mainTable, mainTable.Split(' ').Last(), string.Join(",", list.Where(c => c["table"] == mainTable).Select(c => mainTable.Split(' ').Last() + "." + c["sumFieldCode"] + " as " + c["sumFieldName"]).Distinct()));
            }
            else
            {
                sql = string.Format("select distinct {1}.ID from {0} ", mainTable, mainTable.Split(' ').Last());
            }

            DataTable dtResult = getListData(sql, qb);
            list = list.Where(c => c["table"] != mainTable).ToList();

            foreach (var item in list)
            {
                string table = item["table"];
                string sumFieldCode = item["sumFieldCode"];
                string sumFieldName = item["sumFieldName"];
                string sumType = item["sumType"];
                string groupFieldCode = item.ContainsKey("groupFieldCode") ? item["groupFieldCode"] : "";
                string groupFieldValue = item.ContainsKey("groupFieldValue") ? item["groupFieldValue"] : "";
                string groupFieldText = item.ContainsKey("groupFieldText") ? item["groupFieldText"] : "";
                string extremumFieldCode = item.ContainsKey("extremumFieldCode") ? item["extremumFieldCode"] : "";

                if (sumType != "strSum")
                    if (!string.IsNullOrEmpty(groupFieldValue))
                        sql = string.Format("select {0},{1},{2}({3}) as {3} from {4} group by {0},{1}", fk, groupFieldCode, sumType, sumFieldCode, table);
                    else

                        sql = string.Format("select {0},{1}({2}) as {2} from {3} group by {0}", fk, sumType, sumFieldCode, table);
                else
                    if (!string.IsNullOrEmpty(groupFieldValue))
                        sql = string.Format("select {0},{1},stuff((select ','+{2} from {3} where {0}=tb.{0} and {1}=tb.{1} for xml path('')), 1, 1, '') as {2} from {4} tb group by {0},{1} ", fk, groupFieldCode, sumFieldCode, table, table.Contains(' ') ? table.Remove(table.LastIndexOf(' ')) : table);
                    else
                        sql = string.Format("select {0},stuff((select ','+{1} from {2} where {0}=tb.{0} for xml path('')), 1, 1, '') as {1} from {3} tb group by {0} ", fk, sumFieldCode, table, table.Contains(' ') ? table.Remove(table.LastIndexOf(' ')) : table);

                var sumDT = sqlHelper.ExecuteDataTable(sql);

                #region 处理极值显示字段
                if (!string.IsNullOrEmpty(extremumFieldCode))
                {
                    StringBuilder sbSql = new StringBuilder();
                    if (!string.IsNullOrEmpty(groupFieldValue))
                    {
                        sbSql.AppendFormat("select {0},{1},{2} from {3} where 1=2", fk, groupFieldCode, extremumFieldCode, table);
                        foreach (DataRow row in sumDT.Rows)
                        {
                            sbSql.AppendFormat(" or ({0}='{1}' and {2}='{3}' and {4}='{5}')", fk, row[fk], groupFieldCode, row[groupFieldCode], sumFieldCode, row[sumFieldCode]);
                        }
                    }
                    else
                    {
                        sbSql.AppendFormat("select {0},{1} from {2} where 1=2", fk, extremumFieldCode, table);
                        foreach (DataRow row in sumDT.Rows)
                        {
                            sbSql.AppendFormat(" or ({0}='{1}' and {2}='{3}')", fk, row[fk], sumFieldCode, row[sumFieldCode]);
                        }
                    }

                    var sumDtDetail = sqlHelper.ExecuteDataTable(sbSql.ToString());
                    sumDT.Columns.Remove(sumFieldCode);
                    sumDT.Columns.Add(sumFieldCode);
                    foreach (DataRow row in sumDT.Rows)
                    {
                        if (!string.IsNullOrEmpty(groupFieldValue))
                            row[sumFieldCode] = string.Join(",", sumDtDetail.AsEnumerable().Where(c => c[fk].ToString() == row[fk].ToString() && c[groupFieldCode].ToString() == row[groupFieldCode].ToString()).Select(c => c[extremumFieldCode].ToString()).ToArray());
                        else
                            row[sumFieldCode] = string.Join(",", sumDtDetail.AsEnumerable().Where(c => c[fk].ToString() == row[fk].ToString()).Select(c => c[extremumFieldCode].ToString()).ToArray());
                    }
                }
                #endregion

                #region 赋值到返回结果
                if (string.IsNullOrEmpty(groupFieldValue))
                {
                    if (dtResult.Columns.Contains(sumFieldName) == false)
                        dtResult.Columns.Add(sumFieldName);
                    foreach (DataRow row in dtResult.Rows)
                    {
                        row[sumFieldName] = string.Join(",", sumDT.AsEnumerable().Where(c => c[fk].ToString() == row["ID"].ToString()).Select(c => c[sumFieldCode].ToString()).ToArray());
                    }
                }
                else
                {
                    var arrGroupValue = groupFieldValue.Split(',');
                    var arrGroupText = groupFieldText.Split(',');
                    for (int i = 0; i < arrGroupValue.Length; i++)
                    {
                        if (dtResult.Columns.Contains(arrGroupText[i]) == false)
                            dtResult.Columns.Add(arrGroupText[i]);

                        foreach (DataRow row in dtResult.Rows)
                        {
                            row[arrGroupText[i]] = string.Join(",", sumDT.AsEnumerable().Where(c => c[fk].ToString() == row["ID"].ToString() && arrGroupValue[i] == c[groupFieldCode].ToString()).Select(c => c[sumFieldCode].ToString()).ToArray());
                        }
                    }
                }
                #endregion
            }

            //日期格式处理
            for (int i = dtResult.Columns.Count - 1; i >= 0; i--)
            {
                var col = dtResult.Columns[i];
                if (col.DataType == typeof(DateTime))
                {
                    var colNew = dtResult.Columns.Add(col.ColumnName + "_");
                    foreach (DataRow row in dtResult.Rows)
                    {
                        var str = row[col.ColumnName].ToString();
                        if (str != "")
                            row[col.ColumnName + "_"] = DateTime.Parse(str).ToString("yyyy-MM-dd");
                    }
                    dtResult.Columns.Remove(col);
                    colNew.ColumnName = col.ColumnName;
                }
            }

            return dtResult;
        }

        #endregion
    }
}
