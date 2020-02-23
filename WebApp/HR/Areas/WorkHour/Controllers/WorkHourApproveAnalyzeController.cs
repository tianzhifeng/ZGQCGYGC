using Formula.Helper;
using HR.Logic;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using Formula;
using Base.Logic.Domain;

namespace HR.Areas.WorkHour.Controllers
{
    public class WorkHourApproveAnalyzeController : HRController
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
            var dt = EnumBaseHelper.GetEnumTable("HR.WorkHourState");
            ViewBag.WorkHourState = dt.Select("value <>'Create'", "sortindex asc");
            ViewBag.WorkHourType = (workHourType == WorkHourSaveType.Day.ToString() ? "天" : "小时");
            ViewBag.NeedAdditional = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]) ? true :
               Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]);
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
            var dt = EnumBaseHelper.GetEnumTable("HR.WorkHourState");
            ViewBag.WorkHourState = dt.Select("value <>'Create'", "sortindex asc");
            ViewBag.WorkHourType = (workHourType == WorkHourSaveType.Day.ToString() ? "天" : "小时");
            ViewBag.NeedAdditional = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]) ? true :
               Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]);
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
(case when {1} !=0 then NormalValue/{1} else null end) ApplyRate,
{1} NeedApply,e.UserID,e.Name UserName,e.DeptID UserDeptID,e.DeptName UserDeptName,tmp.* from T_Employee e left join (
	select EmployeeID,sum(WorkHour{0}) ApplyValue,COUNT(1) ApplyCount,sum(NormalValue) NormalValue,sum(AdditionalValue) AdditionalValue,
	sum(Step1{0}) Step1Value,sum(Step2{0}) Step2Value,sum(Confirm{0}) LockedValue
	from S_W_UserWorkHour where WorkHourDate>='" + queryStartDate + "' and WorkHourDate<='" + queryEndDate + @"'
	group by EmployeeID
) tmp on e.ID = tmp.EmployeeID where e.IsDeleted='0'";

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
            var depts = EnumBaseHelper.GetEnumDef("HR.WorkHourDept").EnumItem.ToList();
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
(case when {1} !=0 then NormalValue/({1}*UserCount) else null end) ApplyRate,
({1}*UserCount) NeedApply,* from (
    select DeptID UserDeptID,DeptName UserDeptName,COUNT(distinct UserID) UserCount,sum(ApplyValue) ApplyValue,sum(ApplyCount) ApplyCount,sum(NormalValue) NormalValue,sum(AdditionalValue) AdditionalValue,
    sum(Step1Value) Step1Value,sum(Step2Value) Step2Value,sum(LockedValue) LockedValue from T_Employee e
    left join (select EmployeeID,sum(WorkHour{0}) ApplyValue,COUNT(1) ApplyCount,sum(NormalValue) NormalValue,sum(AdditionalValue) AdditionalValue,
	sum(Step1{0}) Step1Value,sum(Step2{0}) Step2Value,sum(Confirm{0}) LockedValue
	from S_W_UserWorkHour where WorkHourDate>='" + queryStartDate + "' and WorkHourDate<='" + queryEndDate + @"'
	group by EmployeeID) w on w.EmployeeID = e.ID
    where IsDeleted='0'
    group by DeptID,DeptName
) tmp";

            //计算应填天数
            var needApply = new WorkHourFO().GetWorkValue(queryStartDate, queryEndDate);

            if (workHourType == WorkHourSaveType.Day.ToString())
                sql = string.Format(sql, "Day", needApply.ToString());
            else
                sql = string.Format(sql, "Value", needApply.ToString());

            var data = this.SqlHelper.ExecuteDataTable(sql, qb);
            var result = data.Clone();
            foreach (var dept in depts)
            {
                var d = data.Select("UserDeptID = '" + dept.Code + "' and UserDeptName = '" + dept.Name + "'").FirstOrDefault();
                if (d == null)
                {
                    d = result.NewRow();
                    d["UserDeptID"] = dept.Code;
                    d["UserDeptName"] = dept.Name;
                }
                result.Rows.Add(d.ItemArray);
            }

            return Json(result);
        }

    }
}
