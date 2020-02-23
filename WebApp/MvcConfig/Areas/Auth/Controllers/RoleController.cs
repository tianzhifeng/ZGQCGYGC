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
    public class RoleController : BaseController
    {
        public static string ProjectBaseConfig
        {
            get
            {
                var projectBaseConfig = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["ProjectBaseConfig"]);
                if (!string.IsNullOrWhiteSpace(projectBaseConfig))
                    return projectBaseConfig;
                else
                    return "";
            }
        }

        public static string Infrastructure
        {
            get
            {
                var infrastructure = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["Infrastructure"]);
                if (!string.IsNullOrWhiteSpace(infrastructure))
                    return infrastructure;
                else
                    return "";
            }
        }

        public ActionResult Selector()
        {
            ViewBag.ProjectBaseConfig = string.IsNullOrWhiteSpace(ProjectBaseConfig);
            ViewBag.Infrastructure = string.IsNullOrWhiteSpace(Infrastructure);
            return View();
        }

        public JsonResult GetOrgRoleList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                qb.Add("CorpID", Formula.QueryMethod.In, Request["CorpID"]);

            string sql = string.Format("select ID,Code,Name,Type,Description,CorpID from S_A_Role where GroupID='{0}' and Type='OrgRole'", Request["GroupID"]);
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = "select ID,Code,Name,Type,Description,CorpID from S_A_Role where Type='OrgRole'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb));
        }

        public JsonResult GetSysRoleList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                qb.Add("CorpID", Formula.QueryMethod.In, Request["CorpID"]);

            string sql = string.Format("select ID,Code,case when '{1}'='EN' then isnull(NameEN,Name) else Name end as Name,Type,Description,CorpID from S_A_Role where GroupID='{0}' and Type='SysRole'", Request["GroupID"], FormulaHelper.GetCurrentLGID());
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = string.Format("select ID,Code,case when '{0}'='EN' then isnull(NameEN,Name) else Name end as Name,Type,Description,CorpID from S_A_Role where Type='SysRole'", FormulaHelper.GetCurrentLGID());



            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb));
        }

        public JsonResult GetPrjRoleList(MvcAdapter.QueryBuilder qb)
        {
            if (string.IsNullOrEmpty(ProjectBaseConfig))
                return Json("");
            SQLHelper sqlHelpr = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            string sql = string.Format(@"select ID='{0}'+RoleCode,Code=RoleCode,Name=RoleName from S_D_RoleDefine", ConnEnum.InfrasBaseConfig.ToString());
            var dt = sqlHelpr.ExecuteDataTable(sql, (SearchCondition)qb);
            return Json(dt);
        }

        public JsonResult GetEngineeringRoleList(MvcAdapter.QueryBuilder qb)
        {
            if (string.IsNullOrEmpty(Infrastructure))
                return Json("");
            SQLHelper sqlHelpr = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            string sql = string.Format(@"select ID='{0}'+RoleCode,Code=RoleCode,Name=RoleName from S_T_RoleDefine", ConnEnum.Infrastructure.ToString());
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            var dt = sqlHelpr.ExecuteDataTable(sql, (SearchCondition)qb);
            return Json(dt);
        }
    }
}
