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

namespace Comprehensive.Areas.WorkHour.Controllers
{
    public class WorkSelectorController : ComprehensiveController
    {
        public JsonResult GetList(QueryBuilder qb, string WorkHourType)
        {
            qb.Add("WorkHourType", QueryMethod.Equal, WorkHourType);
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
            var sql = @"select ID as ProjectID,ProjectName,ProjectCode,
ChargerDept ProjectDept,ChargerDeptName ProjectDeptName,
ChargerUser ProjectChargerUser,ChargerUserName ProjectChargerUserName,WorkHourType,CreateDate from S_W_ProjectInfo ";
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetProductionList(QueryBuilder qb)
        {
            string WorkHourType = "Production";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var sql = @"select * from (select  '" + WorkHourType + @"' as WorkHourType,ID as ProjectID, Name as ProjectName,Serialnumber as ProjectCode,
ChargerUser as ProjectChargerUser,ChargerUserName as ProjectChargerUserName,
ChargerDept as ProjectDept,ChargerDeptName as ProjectDeptName,CreateDate,
 [UserIDs]=STUFF((SELECT ','+UserID FROM S_I_OBS_User A WHERE A.ProjectInfoID=S_I_Engineering.ID FOR XML PATH('')), 1, 1, '')
 from S_I_Engineering) tableInfo";
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetUserDefaultList(QueryBuilder qb)
        {
            string sql = @"select top 10 * from (
select distinct ProjectID,ProjectName,ProjectCode,ProjectDept,ProjectDeptName,
ProjectChargerUser,ProjectChargerUserName,SubProjectName,SubProjectCode,WorkTimeMajor
,MajorName,MajorCode,TaskWorkCode,TaskWorkName,WorkHourType,Max(WorkHourDate) as MaxWorkHourDate
from S_W_UserWorkHour where UserID='" + this.CurrentUserInfo.UserID + @"'
group by  ProjectID,ProjectName,ProjectCode,ProjectChargerUserName,SubProjectName,SubProjectCode,WorkTimeMajor
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
