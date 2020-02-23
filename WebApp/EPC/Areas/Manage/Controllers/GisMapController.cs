using Config;
using EPC;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Manage.Controllers
{
    public class GisMapController : EPCController
    {
        public ActionResult Index()
        {
            string strViewPt = System.Configuration.ConfigurationManager.AppSettings["InitialViewPt"];
            var arr = (strViewPt ?? "error").Split(',');
            if (arr.Length > 1)
            {
                ViewBag.Long = arr[0];
                ViewBag.Lat = arr[1];
            }

            ViewBag.InitialZoomVal = System.Configuration.ConfigurationManager.AppSettings["initialZoomVal"];
            var list = FormulaHelper.GetEntities<InfrastructureEntities>().Set<S_T_HotRangeStatisticsConfig>().Where(a => a.Enable == "1").ToList();
            var projZoomVal = (list.Max(a => a.MaxZoomVal) ?? 0) + 1;
            ViewBag.ProjZoomVal = projZoomVal;
            ViewBag.RangeConfig = JsonHelper.ToJson(FormulaHelper.GetEntities<InfrastructureEntities>().Set<S_T_HotRangeStatisticsConfig>().Where(a => a.Enable == "1").ToList());
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            if (qb.Items != null && qb.Items.Count != 0)
            {
                return Json(entities.Set<S_I_Engineering>().Where(a => a.Long != null && a.Lat != null).Where(qb).ToList());
            }
            else
            {
                var ff = entities.Set<S_I_Engineering>().Where(a => a.Long != null && a.Lat != null).ToList();
                return Json(entities.Set<S_I_Engineering>().Where(a => a.Long != null && a.Lat != null));
            }            
        }

        public JsonResult GetRangeCount(QueryBuilder qb)
        {
            string rangeSubItems = GetQueryString("rangeSubItems");
            string name = GetQueryString("name");
            string field = GetQueryString("field");
            string subField = GetQueryString("subField");
            
            string condition = null;
            if (!string.IsNullOrEmpty(field) && field.ToLower() != "null")
            {
                condition += string.Format(" (S_I_Engineering.{0} = '{1}') ", field, name);
            }

            if (!string.IsNullOrEmpty(subField) && subField.ToLower() != "null")
            {
                if (string.IsNullOrEmpty(condition))
                {
                    condition = "";
                    condition += string.Format(" (CHARINDEX(S_I_Engineering.{0} ,'{1}') > 0) ", subField, rangeSubItems);
                }
                else
                {
                    condition += string.Format(" or (CHARINDEX(S_I_Engineering.{0} ,'{1}') > 0) ", subField, rangeSubItems);
                }
                
            }

            string sql = "select * from S_I_Engineering  where (Long is not null and Lat is not null) and (" + (condition ?? "1=1") + ")";
            int count = 0;
            if (qb.Items != null && qb.Items.Count != 0)
            {
                count = SqlHelper.ExecuteList<S_I_Engineering>(sql).AsQueryable().Where(qb).Count();
            }
            else
            {
                count = SqlHelper.ExecuteList<S_I_Engineering>(sql).AsQueryable().Count();
            }
            return Json(count);
        }
        
        public JsonResult GetProjProperties(string projID)
        {           
            return Json("");
        }
    }
}
