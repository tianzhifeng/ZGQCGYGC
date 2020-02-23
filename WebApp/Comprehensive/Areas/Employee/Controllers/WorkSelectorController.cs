using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Base.Logic.Domain;
using Config;
using Config.Logic;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Comprehensive;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class WorkSelectorController : ComprehensiveController
    {
        public ViewResult WorkSelector()
        {
            string multiSelect = "true";
            if (string.IsNullOrEmpty(this.GetQueryString("isMulti")) || this.GetQueryString("isMulti") != "T")
                multiSelect = "false";
            ViewBag.MultiSelect = multiSelect;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb, string WorkHourType = "Production")
        {
            var productionType = "Production"; var marketType = "Production";
            if (WorkHourType == productionType)
            {
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                var sql = @"select * from (select  '" + WorkHourType + @"' as WorkHourType,ID as Project, Name as ProjectName,SerialNumber as ProjectCode,
ChargerUser as ProjectChargerUser,ChargerUserName as ProjectChargerUserName,
ChargerDept as ProjectDept,ChargerDeptName as ProjectDeptName,CreateDate
 from S_I_Engineering) tableInfo ";
                var data = db.ExecuteGridData(sql, qb);
                return Json(data);
            }
            else if (WorkHourType == marketType)
            {
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                var sql = @"select * from (select  '" + WorkHourType + @"' as WorkHourType,ID as Project, Name as ProjectName,SerialNumber as ProjectCode,
ChargerUser as ProjectChargerUser,ChargerUserName as ProjectChargerUserName,
ChargerDept as ProjectDept,ChargerDeptName as ProjectDeptName,CreateDate
 from S_I_Engineering) tableInfo ";
                var data = db.ExecuteGridData(sql, qb);
                return Json(data);
            }
            else
            {
                qb.Add("WorkHourType", QueryMethod.Equal, WorkHourType);
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
                var sql = @"select ID as Project,ProjectName,ProjectCode,ChargerDept as ProjectDept,ChargerDeptName as ProjectDeptName,
ChargerUser as ProjectChargerUser,ChargerUserName as ProjectChargerUserName,WorkHourType,CreateDate from S_W_ProjectInfo ";
                var data = db.ExecuteGridData(sql, qb);
                return Json(data);
            }
        }

        public JsonResult GetUserDefaultList(QueryBuilder qb)
        {
            string sql = @"select top 10 * from (
select distinct Project,ProjectName,ProjectCode,ProjectDept,ProjectDeptName,
ProjectChargerUser,ProjectChargerUserName--,SubProjectName,SubProjectCode,TaskWorkCode,TaskWorkName
,MajorName,MajorCode,WorkHourType,Max(WorkHourDate) as MaxWorkHourDate
from S_W_UserWorkHour where UserID='" + this.CurrentUserInfo.UserID + @"'
group by  Project,ProjectName,ProjectCode,ProjectChargerUserName,SubProjectName,SubProjectCode
,MajorName,MajorCode,TaskWorkCode,TaskWorkName,WorkHourType,
ProjectDept,ProjectDeptName,
ProjectChargerUser) tableInfo";
            qb.SortField = "MaxWorkHourDate";
            qb.PageSize = 0;
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }
    }
}
