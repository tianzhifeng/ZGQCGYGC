using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using System.Data;
using MvcAdapter;
using System.Data.Entity.Infrastructure;
using Formula;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Base.Areas.Meta.Controllers
{
    public class EnumController : BaseController
    {
        public FileResult ExportSql(string ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var id in ids.Split(','))
            {
                string sql = string.Format("select * from S_M_EnumDef where ID= '{0}'", id);
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                DataTable dtDef = sqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_M_EnumItem where EnumDefID='{0}'", id);
                DataTable dtItem = sqlHelper.ExecuteDataTable(sql);
                sb.AppendLine(string.Format("delete from S_M_EnumDef where ID='{0}'", id));
                sb.AppendLine(SQLHelper.CreateInsertSql("S_M_EnumDef", dtDef));
                sb.AppendLine(SQLHelper.CreateInsertSql("S_M_EnumItem", dtItem));
            }
            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "enums.sql");
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult DefEdit()
        {
            return View();
        }

        public ActionResult ItemEdit()
        {
            return View();
        }


        public JsonResult GetTree()
        {
            if (Request["listType"] == "subCompany")
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name,CategoryCode,IconClass from S_M_Category where FullID like '{0}%'", "0"));

                var companyId = Formula.FormulaHelper.GetUserInfo().AdminCompanyID;
                SQLHelper flowSqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                var dtFlow = flowSqlHelper.ExecuteDataTable(string.Format("select distinct CategoryID from S_WF_DefFlow where CompanyID='{0}'", companyId));
                var _dt = sqlHelper.ExecuteDataTable(string.Format(@"
            select distinct CategoryID from S_UI_Form where CompanyID='{0}'
            union 
            select distinct CategoryID from S_UI_List where CompanyID='{0}'
            union 
            select distinct CategoryID from S_UI_Word where CompanyID='{0}'
            union 
            select distinct CategoryID from S_M_EnumDef where CompanyID='{0}'", companyId));
                var list = new List<string>();
                foreach (DataRow row in dtFlow.Rows)
                {
                    var categoryId = row["CategoryID"].ToString();
                    if (list.Contains(categoryId) == false)
                        list.Add(categoryId);
                }

                foreach (DataRow row in _dt.Rows)
                {
                    var categoryId = row["CategoryID"].ToString();
                    if (list.Contains(categoryId) == false)
                        list.Add(categoryId);
                }

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    string parentId = dt.Rows[i]["ParentID"].ToString();
                    if (parentId != "" && parentId != "0")
                    {
                        dt.Rows.RemoveAt(i);
                        continue;
                    }
                    var id = dt.Rows[i]["ID"].ToString();
                    if (dt.AsEnumerable().Count(c => c["ParentID"].ToString() == id) > 0)
                        continue;
                    if (list.Contains(id))
                        continue;
                    dt.Rows.RemoveAt(i);
                }

                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(entities.Set<S_M_Category>().Where(c => string.IsNullOrEmpty(c.ParentID) || c.ParentID == "0"));
            }
        }

        public JsonResult GetDefList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                qb.Add("CategoryID", QueryMethod.Equal, Request["CategoryID"]);

            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"].ToLower() == "true"
                && Request["listType"] == "subCompany")
                qb.Add("CompanyID", QueryMethod.Equal, FormulaHelper.GetUserInfo().AdminCompanyID);

            var list = entities.Set<S_M_EnumDef>().WhereToGridData(qb);

            return Json(list);
        }

        #region 获取枚举项

        public JsonResult GetItemList(QueryBuilder qb)
        {
            string enumType = Request["EnumType"];
            if (enumType == Base.Logic.EnumType.Table.ToString())
            {
                string enumDefID = Request["EnumDefID"];
                var def = entities.Set<S_M_EnumDef>().Where(c => c.ID == enumDefID).SingleOrDefault();
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(def.ConnName);


                string sql = def.Sql;
                string orderby = "order by text asc";
                if (!string.IsNullOrEmpty(def.Orderby))
                    orderby = def.Orderby;

                var user = FormulaHelper.GetUserInfo();

                //表枚举支持当前公司ID参数和地址栏参数
                Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
                sql = reg.Replace(sql, (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');
                    switch (value)
                    {
                        case "CurrentUserCompanyID":
                            return user.UserCompanyID;
                    }
                    //TODO：地址栏参数，管理页地址栏无参数
                    return "";
                });


                orderby = orderby.ToLower().Replace("order", "").Replace("by", "").Trim().Split(',')[0];

                qb.SortField = orderby.Split(' ')[0];
                qb.SortOrder = orderby.Split(' ').Length > 1 ? orderby.Split(' ')[1] : "asc";

                DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
                dt.Columns["value"].ColumnName = "Code";
                dt.Columns["text"].ColumnName = "Name";
                GridData gridData = new GridData(dt);
                gridData.total = qb.TotolCount;

                return Json(gridData);
            }
            else
            {
                qb.SortField = "SortIndex";
                qb.SortOrder = "asc";

                return base.JsonGetList<S_M_EnumItem>(qb);
            }
        }

        public JsonResult GetItemListByPinYin(string enumCode, string key)
        {
            if (key != null)
                key = key.Trim();
            var def = entities.Set<S_M_EnumDef>().Where(c => c.Code == enumCode).SingleOrDefault();
            if (def != null)
            {
                if (def.Type == Base.Logic.EnumType.Table.ToString())
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(def.ConnName);

                    string sql = def.Sql;
                    string orderby = "order by text asc";
                    if (!string.IsNullOrEmpty(def.Orderby))
                        orderby = def.Orderby;

                    orderby = orderby.ToLower().Replace("order", "").Replace("by", "").Trim().Split(',')[0];

                    QueryBuilder qb = new QueryBuilder();
                    SetPinYinQueryBuilder(key, "text", ref qb);
                    qb.PageSize = 0;
                    qb.SortField = orderby.Split(' ')[0];
                    qb.SortOrder = orderby.Split(' ').Length > 1 ? orderby.Split(' ')[1] : "asc";

                    DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
                    return Json(dt);
                }
                else
                {
                    string sql = "select * from S_M_EnumItem";
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    QueryBuilder qb = new QueryBuilder();
                    qb.Add("EnumDefID", QueryMethod.Equal, def.ID);
                    SetPinYinQueryBuilder(key, "Name", ref qb);
                    qb.PageSize = 0;
                    qb.SortField = "SortIndex";
                    qb.SortOrder = "asc";
                    var dt = sqlHelper.ExecuteDataTable(sql, qb);
                    var list = from c in dt.AsEnumerable()
                               select new { text = c["Name"], value = c["Code"] };
                    return Json(list);
                }
            }
            else
            {
                return Json(string.Empty);
            }
        }

        #endregion

        public JsonResult GetDef(string id)
        {
            return base.JsonGetModel<S_M_EnumDef>(id);
        }

        public JsonResult GetItem(string id)
        {
            var model = GetEntity<S_M_EnumItem>(id);

            if (model._state == EntityStatus.added.ToString())
            {
                string EnumDefID = Request["EnumDefID"];
                var lastModel = entities.Set<S_M_EnumItem>().Where(c => c.EnumDefID == EnumDefID).OrderByDescending(c => c.SortIndex).FirstOrDefault();
                if (lastModel == null)
                    model.SortIndex = 0;
                else if (lastModel.SortIndex == null)
                    model.SortIndex = 0;
                else
                    model.SortIndex = lastModel.SortIndex + 1;
            }

            return Json(model);
        }

        public JsonResult SaveDef()
        {
            return base.JsonSave<S_M_EnumDef>();
        }

        public JsonResult SaveItem()
        {
            var item = UpdateEntity<S_M_EnumItem>();
            if (entities.Set<S_M_EnumItem>().SingleOrDefault(c => c.EnumDefID == item.EnumDefID && c.Code == item.Code && c.ID != item.ID) != null)
                throw new Exception("枚举编号不能重复");
            var def = entities.Set<S_M_EnumDef>().SingleOrDefault(c => c.ID == item.EnumDefID);
            def.ModifyTime = DateTime.Now;
            entities.SaveChanges();
            return Json(new { ID = item.ID });
        }

        public JsonResult DeleteDef(string listIDs)
        {
            return base.JsonDelete<S_M_EnumDef>(listIDs);
        }

        public JsonResult DeleteItem(string listIDs)
        {
            return base.JsonDelete<S_M_EnumItem>(listIDs);
        }

        private void SetPinYinQueryBuilder(string key, string columnName, ref QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int firstCode = (int)key[0];
                //中文或数字
                if (firstCode >= 255 || 48 <= firstCode && firstCode <= 57)
                {
                    qb.Add(columnName, QueryMethod.Like, key);
                }
                else if ((65 <= firstCode && firstCode <= 90) || (97 <= firstCode && firstCode <= 122))	//字母
                {
                    string[,] hz = GetHanziScope(key);
                    for (int i = 0; i < hz.GetLength(0); i++)
                    {
                        if (Config.Constant.IsOracleDb)
                        {
                            qb.Add("nlssort(SUBSTR(\"" + columnName + "\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M')", QueryMethod.GreaterThanOrEqual, "nlssort('" + hz[i, 0] + "','NLS_SORT=SCHINESE_PINYIN_M')");
                            qb.Add("nlssort(SUBSTR(\"" + columnName + "\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M')", QueryMethod.LessThanOrEqual, "nlssort('" + hz[i, 1] + "','NLS_SORT=SCHINESE_PINYIN_M')");
                        }
                        else
                        {
                            qb.Add("SUBSTRING(" + columnName + ", " + (i + 1) + ", 1)", QueryMethod.GreaterThanOrEqual, hz[i, 0]);
                            qb.Add("SUBSTRING(" + columnName + ", " + (i + 1) + ", 1)", QueryMethod.LessThanOrEqual, hz[i, 1]);
                        }
                    }
                }
            }
        }

        private string[,] GetHanziScope(string pinyinIndex)
        {
            pinyinIndex = pinyinIndex.ToLower();
            string[,] hz = new string[pinyinIndex.Length, 2];
            for (int i = 0; i < pinyinIndex.Length; i++)
            {
                string index = pinyinIndex.Substring(i, 1);
                if (index == "a") { hz[i, 0] = "吖"; hz[i, 1] = "驁"; }
                else if (index == "b") { hz[i, 0] = "八"; hz[i, 1] = "簿"; }
                else if (index == "c") { hz[i, 0] = "嚓"; hz[i, 1] = "錯"; }
                else if (index == "d") { hz[i, 0] = "咑"; hz[i, 1] = "鵽"; }
                else if (index == "e") { hz[i, 0] = "妸"; hz[i, 1] = "樲"; }
                else if (index == "f") { hz[i, 0] = "发"; hz[i, 1] = "猤"; }
                else if (index == "g") { hz[i, 0] = "旮"; hz[i, 1] = "腂"; }
                else if (index == "h") { hz[i, 0] = "妎"; hz[i, 1] = "夻"; }
                else if (index == "j") { hz[i, 0] = "丌"; hz[i, 1] = "攈"; }
                else if (index == "k") { hz[i, 0] = "咔"; hz[i, 1] = "穒"; }
                else if (index == "l") { hz[i, 0] = "垃"; hz[i, 1] = "鱳"; }
                else if (index == "m") { hz[i, 0] = "嘸"; hz[i, 1] = "椧"; }
                else if (index == "n") { hz[i, 0] = "拏"; hz[i, 1] = "桛"; }
                else if (index == "o") { hz[i, 0] = "噢"; hz[i, 1] = "漚"; }
                else if (index == "p") { hz[i, 0] = "妑"; hz[i, 1] = "曝"; }
                else if (index == "q") { hz[i, 0] = "七"; hz[i, 1] = "裠"; }
                else if (index == "r") { hz[i, 0] = "亽"; hz[i, 1] = "鶸"; }
                else if (index == "s") { hz[i, 0] = "仨"; hz[i, 1] = "蜶"; }
                else if (index == "t") { hz[i, 0] = "他"; hz[i, 1] = "籜"; }
                else if (index == "w") { hz[i, 0] = "屲"; hz[i, 1] = "鶩"; }
                else if (index == "x") { hz[i, 0] = "夕"; hz[i, 1] = "鑂"; }
                else if (index == "y") { hz[i, 0] = "丫"; hz[i, 1] = "韻"; }
                else if (index == "z") { hz[i, 0] = "帀"; hz[i, 1] = "咗"; }
                else { hz[i, 0] = index; hz[i, 1] = index; }
            }
            return hz;
        }

    }
}
