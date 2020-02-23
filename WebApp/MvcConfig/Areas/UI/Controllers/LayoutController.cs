using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Base.Logic.Domain;
using System.Collections;
using Newtonsoft.Json;
using Config;
using System.Text;
using Formula.Helper;
using Workflow.Logic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Data;
using System.Web.Script.Serialization;
using Base.Logic.BusinessFacade;
using MvcAdapter;
using Formula.Exceptions;
using Workflow.Logic.BusinessFacade;
using System.IO;


namespace MvcConfig.Areas.UI.Controllers
{
    public class LayoutController : BaseController
    {
        public ActionResult PageView()
        {
            return View();
        }


        /// <summary>
        /// DataTable分页
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="PageIndex">页索引,注意：从1开始</param>
        /// <param name="PageSize">每页大小</param>
        /// <returns>分好页的DataTable数据</returns>       第1页        每页10条
        private DataTable GetPagedTable(DataTable dt, int pageIndex, int pageSize)
        {
            if (pageIndex == 0) { return dt; }
            DataTable newdt = dt.Copy();
            newdt.Clear();
            int rowbegin = (pageIndex - 1) * pageSize;
            int rowend = pageIndex * pageSize;
            if (rowbegin >= dt.Rows.Count)
            { return newdt; }
            if (rowend > dt.Rows.Count)
            { rowend = dt.Rows.Count; }
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }
        /// <summary>
        /// 返回分页的页数
        /// </summary>
        /// <param name="count">总条数</param>
        /// <param name="pageye">每页显示多少条</param>
        /// <returns>如果 结尾为0：则返回1</returns>
        private int PageCount(int count, int pageye)
        {
            int page = 0;
            int sesepage = pageye;
            if (count % sesepage == 0) { page = count / sesepage; }
            else { page = (count / sesepage) + 1; }
            if (page == 0) { page += 1; }
            return page;
        }


 
        public static string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
        public static string Encode(object o)
        {
            if (o == null || o.ToString() == "null") return null;

            if (o != null && (o.GetType() == typeof(String) || o.GetType() == typeof(string)))
            {
                return o.ToString();
            }
            IsoDateTimeConverter dt = new IsoDateTimeConverter();
            dt.DateTimeFormat = DateTimeFormat;
            return JsonConvert.SerializeObject(o, dt);
        }

        public static object Decode(string json)
        {
            if (String.IsNullOrEmpty(json)) return "";
            object o = JsonConvert.DeserializeObject(json);
            if (o.GetType() == typeof(String) || o.GetType() == typeof(string))
            {
                o = JsonConvert.DeserializeObject(o.ToString());
            }
            object v = toObject(o);
            return v;
        }

        private static object toObject(object o)
        {
            if (o == null) return null;

            if (o.GetType() == typeof(string))
            {
                //判断是否符合2010-09-02T10:00:00的格式
                string s = o.ToString();
                if (s.Length == 19 && s[10] == 'T' && s[4] == '-' && s[13] == ':')
                {
                    o = System.Convert.ToDateTime(o);
                }
            }
            else if (o is JObject)
            {
                JObject jo = o as JObject;

                Hashtable h = new Hashtable();

                foreach (KeyValuePair<string, JToken> entry in jo)
                {
                    h[entry.Key] = toObject(entry.Value);
                }

                o = h;
            }
            else if (o is IList)
            {

                ArrayList list = new ArrayList();
                list.AddRange((o as IList));
                int i = 0, l = list.Count;
                for (; i < l; i++)
                {
                    list[i] = toObject(list[i]);
                }
                o = list;

            }
            else if (typeof(JValue) == o.GetType())
            {
                JValue v = (JValue)o;
                o = toObject(v.Value);
            }
            else
            {
            }
            return o;
        }




    //********************************************************新定义布局兼容以前项目**********************************************************************************//
        public ActionResult LayoutView(string tmplCode)
        {
            var listDef = entities.Set<S_UI_Layout>().Where(c => c.Code == tmplCode);
            LayoutUIFO uiFO = FormulaHelper.CreateFO<LayoutUIFO>();
            ViewBag.ListHtml = uiFO.LayoutHtml(tmplCode);
            ViewBag.Script = uiFO.CreateScript(tmplCode);
            ViewBag.FixedFields = string.Format("var FixedFields={0};", Newtonsoft.Json.JsonConvert.SerializeObject(uiFO.GetFixedWidthFields(tmplCode)));
            ViewBag.Title = listDef.First().Name;


            var fields = JsonHelper.ToList(listDef.First().LayoutField);
            var tab = new Tab();
            foreach (var field in fields)
            {
                if (!field.ContainsKey("Settings"))
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());

                if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                    continue;

                if (!settings.ContainsKey("TabSearchName") || settings["TabSearchName"].ToString() == "")
                    continue;


                Category category = null;
                string enumKey = settings["EnumKey"].ToString();
                if (enumKey.StartsWith("["))
                    category = CategoryFactory.GetCategoryByString(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString());
                else
                    category = CategoryFactory.GetCategory(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString());

                //默认值
                if (settings.ContainsKey("TabSearchDefault") && settings["TabSearchDefault"].ToString() != "")
                    category.SetDefaultItem(settings["TabSearchDefault"].ToString());
                else
                    category.SetDefaultItem();

                //多选或单选
                if (settings.ContainsKey("TabSearchMulti") && settings["TabSearchMulti"].ToString() == "false")
                    category.Multi = false;
                else
                    category.Multi = true;

                tab.Categories.Add(category);
            }

            if (tab.Categories.Count > 0)
            {
                tab.IsDisplay = true;
                ViewBag.Tab = tab;
                ViewBag.Layout = "~/Views/Shared/_AutoListLayoutTab.cshtml";
            }
            else
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            }

            return View();
        }


        public JsonResult GetList(string tmplCode, QueryBuilder qb)
        {
            var listDef = entities.Set<S_UI_Layout>().Where(c => c.Code == tmplCode).First();
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            string sql = uiFO.ReplaceString(listDef.SQL);

            #region 地址栏过滤

            //            string sqlTmpl = string.Format(@"SELECT  fieldCode= a.name , description= isnull(g.[value],''),fieldType=b.name,sortIndex=a.column_id
            //FROM  sys.columns a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = a.column_id)
            //left join systypes b on a.user_type_id=b.xusertype  
            //WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name in('{0}'))", listDef.TableNames.Replace(",", "','"));
            //            if (Config.Constant.IsOracleDb)
            //            {
            //                sqlTmpl = string.Format(@"select column_name as fieldCode from user_tab_columns where table_name in('{0}')", listDef.TableNames.Replace(",", "','"));
            //            }

            //            DataTable dtTmpl = sqlHeler.ExecuteDataTable(sqlTmpl);

            //            //地址栏参数作为查询条件
            //            var queryDtTmpl = dtTmpl.AsEnumerable();
            //            foreach (string key in Request.QueryString.Keys)
            //            {
            //                if (string.IsNullOrEmpty(key))
            //                    continue;
            //                if ("ID,FullID,FULLID,TmplCode,IsPreView,_winid,_t".Split(',').Contains(key) || key.StartsWith("$"))
            //                    continue;
            //                if (queryDtTmpl.Count(c => c["fieldCode"].ToString().ToLower() == key.ToLower()) > 0)
            //                    qb.Add(key, QueryMethod.In, Request[key]);
            //            }


            DataTable dtTmpl = sqlHeler.ExecuteDataTable(sql, qb);
            foreach (string key in Request.QueryString.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                if ("ID,FullID,FULLID,TmplCode,IsPreView,_winid,_t".Split(',').Contains(key) || key.StartsWith("$"))
                    continue;
                if (dtTmpl.Columns.Contains(key))
                    qb.Add(key, QueryMethod.In, Request[key]); ;
            }


            #endregion

            GridData data = null;
            if (listDef.LayoutGrid.Contains("\"showPager\":\"false\""))
            {
                data = new GridData(sqlHeler.ExecuteDataTable(sql, (SearchCondition)qb, listDef.OrderBy));
            }
            else
            {
                data = sqlHeler.ExecuteGridData(sql, qb, listDef.OrderBy);
            }

            #region 计算汇总

            var fields = JsonHelper.ToList(listDef.LayoutField);
            StringBuilder sb = new StringBuilder();
            foreach (var field in fields)
            {
                if (field.ContainsKey("Settings") == false)
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("Collect") == false || settings["Collect"].ToString() == "")
                    continue;
                if (Config.Constant.IsOracleDb)
                    sb.AppendFormat(",{1}({0}) as {0}", field["field"], settings["Collect"]);
                else
                    sb.AppendFormat(",{0}={1}({0})", field["field"], settings["Collect"]);

                if (settings["Collect"].ToString() == "sum")
                    data.sumData.Add(field["field"].ToString(), null);
                else
                    data.avgData.Add(field["field"].ToString(), null);
            }
            if (sb.Length > 0)
            {
                string companyAuth = "";
                if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"] == "True")
                {
                    var dt = sqlHeler.ExecuteDataTable(string.Format("select * from ({0}) tempDt1 where 1=2", sql));
                    if (dt.Columns.Contains("CompanyID"))
                    {
                        companyAuth = string.Format(" and CompanyID='{0}'", FormulaHelper.GetUserInfo().UserCompanyID);
                    }
                }

                string collectSql = "";
                if (Config.Constant.IsOracleDb)
                {
                    collectSql = string.Format("select {0} from (select * from ({1}) tb where 1=1 {2}) T"
                        , sb.ToString().Trim(',')
                        , sql
                        , qb.GetWhereString(false) + FormulaHelper.CreateAuthDataFilter().GetWhereString(false) + companyAuth
                        );
                }
                else
                {
                    collectSql = string.Format("select {0} from (select * from ({1}) as tb where 1=1 {2}) as T"
                       , sb.ToString().Trim(',')
                       , sql
                       , qb.GetWhereString(false) + FormulaHelper.CreateAuthDataFilter().GetWhereString(false) + companyAuth
                       );
                }
                DataTable dtCollect = sqlHeler.ExecuteDataTable(collectSql);

                foreach (DataColumn col in dtCollect.Columns)
                {
                    if (data.sumData.ContainsKey(col.ColumnName))
                        data.sumData[col.ColumnName] = dtCollect.Rows[0][col] is DBNull ? 0 : dtCollect.Rows[0][col];
                    else
                        data.avgData[col.ColumnName] = dtCollect.Rows[0][col] is DBNull ? 0 : dtCollect.Rows[0][col];
                }
            }

            //汇总数据小数点
            foreach (var field in fields)
            {
                if (field.ContainsKey("Settings") == false)
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("Collect") == false || settings["Collect"].ToString() == "")
                    continue;
                int decimalPlaces = 2;
                if (settings.ContainsKey("decimalPlaces") && settings["decimalPlaces"].ToString() != "")
                    decimalPlaces = Convert.ToInt32(settings["decimalPlaces"]);

                string fieldCode = field["field"].ToString();

                if (data.sumData.ContainsKey(fieldCode))
                {
                    data.sumData[fieldCode] = string.Format("{0:F" + decimalPlaces + "}", Convert.ToDouble(data.sumData[fieldCode]));
                }
                else
                {
                    data.avgData[fieldCode] = string.Format("{0:F" + decimalPlaces + "}", Convert.ToDouble(data.avgData[fieldCode]));
                }
            }

            #endregion

            return Json(data);
        }

        public JsonResult GetTree(string tmplCode, QueryBuilder qb)
        {
            var listDef = entities.Set<S_UI_Layout>().Where(c => c.Code == tmplCode).First();
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            string sql = uiFO.ReplaceString(listDef.SQL);
            return Json(sqlHeler.ExecuteDataTable(sql));
        }

        public JsonResult Delete(string tmplCode, string listIDs)
        {
            var listDef = entities.Set<S_UI_Layout>().SingleOrDefault(c => c.Code == tmplCode);

            if (listDef.DenyDeleteFlow == "1" || listDef.DenyDeleteFlow == "true")
            {
                SQLHelper flowHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                string flowSql = string.Format("select count(1) from S_WF_InsFlow where FormInstanceID in('{0}')", listIDs.Replace(",", "','"));
                var obj = flowHelper.ExecuteScalar(flowSql);
                if (obj.ToString() != "0")
                    throw new BusinessException("已经启动流程的表单不能删除！");
            }

            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);
            string sql = string.Format("delete from {0} where ID in('{1}')", listDef.TableNames.Split(',')[0], listIDs.Replace(",", "','"));
            sqlHeler.ExecuteNonQuery(sql);

            FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
            foreach (string id in listIDs.Split(','))
            {
                flowFO.DeleteFlowByFormInstanceID(id);
            }

            return Json("");
        }

        #region 导出HTML

        public FileResult ExportHtml()
        {
            string tmplCode = Request["TmplCode"];
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            StringBuilder html = new StringBuilder();
            html.Append(uiFO.CreateListHtml(tmplCode));
            html.AppendLine();
            html.Append("<script type='text/javascript'>");
            html.Append(uiFO.CreateListScript(tmplCode, true));
            html.AppendLine();
            html.Append("</script>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(html.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", Request["TmplCode"] + ".cshtml");

        }





        #endregion


        #region entities

        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                return FormulaHelper.GetEntities<BaseEntities>();
            }
        }

        #endregion


    }
    public class DateObject
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
