using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Comprehensive.Logic.BusinessFacade;
using Config;
using Config.Logic;
using System.Data;
using Formula.Helper;
using MvcAdapter;
using Formula;
using Newtonsoft.Json;
using Formula.ImportExport;
using System.Text.RegularExpressions;

namespace Comprehensive.Areas.WorkHour.Controllers
{
    public class WorkHourApproveAnalyzeController : ComprehensiveController
    {
        //工时填报与结算类型；Hour：按小时填报按小时结算（默认），HD：按小时填报按天结算，Day：按天填报按天结算
        string workHourType = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkHourType"]) ? WorkHourSaveType.Hour.ToString() :
            System.Configuration.ConfigurationManager.AppSettings["WorkHourType"];

        public ActionResult UserList()
        {
            var QueryStartDate = GetQueryString("QueryStartDate");
            var QueryEndDate = GetQueryString("QueryEndDate");
            if (string.IsNullOrEmpty(QueryStartDate))
                QueryStartDate = DateTime.Now.Date.ToString("yyyy-MM") + "-01";
            if (string.IsNullOrEmpty(QueryEndDate))
                QueryEndDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewBag.QueryStartDate = QueryStartDate;
            ViewBag.QueryEndDate = QueryEndDate;
            var dt = EnumBaseHelper.GetEnumTable("Comprehensive.WorkHourState");
            ViewBag.WorkHourState = dt.Select("value <>'Create'", "sortindex asc");
            ViewBag.WorkHourType = (workHourType == WorkHourSaveType.Day.ToString() ? "天" : "小时");
            return View();
        }

        public ActionResult DeptList()
        {
            var QueryStartDate = GetQueryString("QueryStartDate");
            var QueryEndDate = GetQueryString("QueryEndDate");
            if (string.IsNullOrEmpty(QueryStartDate))
                QueryStartDate = DateTime.Now.Date.ToString("yyyy-MM") + "-01";
            if (string.IsNullOrEmpty(QueryEndDate))
                QueryEndDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewBag.QueryStartDate = QueryStartDate;
            ViewBag.QueryEndDate = QueryEndDate;
            var dt = EnumBaseHelper.GetEnumTable("Comprehensive.WorkHourState");
            ViewBag.WorkHourState = dt.Select("value <>'Create'", "sortindex asc");
            ViewBag.WorkHourType = (workHourType == WorkHourSaveType.Day.ToString() ? "天" : "小时");
            return View();
        }

        public JsonResult GetUserList(QueryBuilder qb)
        {
            var queryStartDate = "";
            var queryEndDate = "";
            foreach (var item in qb.Items)
            {
                if (item.Field == "WorkHourDate")
                {
                    if (item.Method == QueryMethod.GreaterThanOrEqual)
                        queryStartDate = item.Value.ToString();
                    else if (item.Method == QueryMethod.LessThanOrEqual)
                        queryEndDate = item.Value.ToString();
                }
            }
            qb.Items.RemoveAll(a => a.Field == "WorkHourDate");
            if (string.IsNullOrEmpty(queryEndDate))
                queryEndDate = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            var deptID = GetQueryString("UserDeptID");
            if (!string.IsNullOrEmpty(deptID))
                qb.Add("UserDeptID", QueryMethod.Equal, deptID);

            var sql = @"select (case when ApplyValue!=0 then Step1Value/ApplyValue else null end) Step1Rate,
(case when ApplyValue!=0 then Step2Value/ApplyValue else null end) Step2Rate,
(case when ApplyValue!=0 then LockedValue/ApplyValue else null end) LockedRate,
(case when {1} !=0 then ApplyValue/{1} else null end) ApplyRate,
{1} NeedApply,* from (
	select UserID,UserName,UserDeptID,UserDeptName,
	sum(WorkHour{0}) ApplyValue,COUNT(1) ApplyCount,
	sum(Step1{0}) Step1Value,sum(Step2{0}) Step2Value,sum(Confirm{0}) LockedValue
	from S_W_UserWorkHour where WorkHourDate>='" + queryStartDate + "' and WorkHourDate<='" + queryEndDate + @"'
	group by UserID,UserName,UserDeptID,UserDeptName
) tmp";

            //计算应填天数
            var needApply = new WorkHourFO().GetWorkValue(queryStartDate, queryEndDate);

            if (workHourType == WorkHourSaveType.Day.ToString())
                sql = string.Format(sql, "Day", needApply.ToString());
            else
                sql = string.Format(sql, "Value", needApply.ToString());

            var result = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(result);
        }

        public JsonResult GetDeptList(QueryBuilder qb)
        {
            var queryStartDate = "";
            var queryEndDate = "";
            foreach (var item in qb.Items)
            {
                if (item.Field == "WorkHourDate")
                {
                    if (item.Method == QueryMethod.GreaterThanOrEqual)
                        queryStartDate = item.Value.ToString();
                    else if (item.Method == QueryMethod.LessThanOrEqual)
                        queryEndDate = item.Value.ToString();
                }
            }
            qb.Items.RemoveAll(a => a.Field == "WorkHourDate");
            if (string.IsNullOrEmpty(queryEndDate))
                queryEndDate = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";

            var sql = @"select (case when ApplyValue!=0 then Step1Value/ApplyValue else null end) Step1Rate,
(case when ApplyValue!=0 then Step2Value/ApplyValue else null end) Step2Rate,
(case when ApplyValue!=0 then LockedValue/ApplyValue else null end) LockedRate,
(case when {1} !=0 then ApplyValue/({1}*UserCount) else null end) ApplyRate,
({1}*UserCount) NeedApply,* from (
	select UserDeptID,UserDeptName,
	sum(WorkHour{0}) ApplyValue,COUNT(1) ApplyCount,COUNT(distinct UserID) UserCount,
	sum(Step1{0}) Step1Value,sum(Step2{0}) Step2Value,sum(Confirm{0}) LockedValue
	from S_W_UserWorkHour where WorkHourDate>='" + queryStartDate + "' and WorkHourDate<='" + queryEndDate + @"'
	group by UserDeptID,UserDeptName
) tmp";

            //计算应填天数
            var needApply = new WorkHourFO().GetWorkValue(queryStartDate, queryEndDate);

            if (workHourType == WorkHourSaveType.Day.ToString())
                sql = string.Format(sql, "Day", needApply.ToString());
            else
                sql = string.Format(sql, "Value", needApply.ToString());

            var result = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(result);
        }

    }
}
