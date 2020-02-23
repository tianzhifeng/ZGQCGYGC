using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using Formula;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class OrgController : BaseController
    {
        #region Selector

        public ActionResult Selector()
        {
            return View();
        }

        public JsonResult GetTree()
        {
            string fullID = Request["RootFullID"];
            if (fullID == null)
                fullID = "";
            fullID = fullID.Trim(',', ' ');
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string sql = string.Format(@"select ID,'' as FullName,Code,case when '{1}'='EN' then isnull(NameEN,Name) else Name end as Name,
            ParentID,FullID,Type,SortIndex,Description from S_A_Org where  FullID like '{0}%' and IsDeleted='0'", fullID, FormulaHelper.GetCurrentLGID());

            if (!string.IsNullOrEmpty(Request["OrgType"]))
                sql += string.Format(" and Type in ('{0}')", Request["OrgType"].Replace(",", "','"));

            SearchCondition cnd = new SearchCondition();
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                cnd.Add("FullID", Formula.QueryMethod.InLike, Request["CorpID"]);

            sql += " order by ParentID,SortIndex";
            var dt = sqlHelper.ExecuteDataTable(sql, cnd);
            //数据量大后加载非常缓慢，故注释此处 by PengPai
            //foreach (DataRow item in dt.Rows)
            //{
            //    var itemFullID = item["FullID"].ToString();
            //    var itemFullIDList = itemFullID.Split('.');
            //    if (itemFullIDList.Length > 1)
            //    {
            //        var ancestorIDs = String.Join(",", itemFullIDList);
            //        var ancestorOrg = dt.Select("ID in ('" + ancestorIDs.Replace(",", "','") + "')", " FullID asc");
            //        var fullName = "";
            //        foreach (DataRow ancestor in ancestorOrg)
            //        {
            //            fullName += ancestor["Name"].ToString() + ".";
            //        }
            //        item["FullName"] = fullName.Trim().TrimEnd('.');
            //    }
            //}
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}
