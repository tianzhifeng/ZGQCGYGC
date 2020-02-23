using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Project.Logic.Domain;
using Config;
using System.Text;
using Formula.Helper;
using System.Data;
using Formula;
using Config.Logic;
using Project.Logic;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class WorkHourController : ProjectController
    {
        #region 项目工时统计
        public ActionResult ProjectHourRpt()
        {
            var items =  EnumBaseHelper.GetEnumDef("System.WorkHourType").EnumItem.ToList();
            ViewBag.WorkHourTypeInfo = items;

            var tab = new Tab();
            var stateCategory = CategoryFactory.GetCategory(typeof(ProjectCommoneState), "State");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;
            tab.Categories.Add(stateCategory);

            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "负责部门", "ChargeDeptID");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var workHourCategory = CategoryFactory.GetCategory("System.DateFilder", "工时日期", "WorkHourDate");
            workHourCategory.SetDefaultItem();
            workHourCategory.Multi = false;
            tab.Categories.Add(workHourCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public ActionResult ProjectPersonHourView()
        {
            ViewBag.ColumnHearder = GetWorkHourCategoryColumnHeader();
            return View();
        }

        public JsonResult GetProjectHourList(QueryBuilder qb)
        {
            qb.IsOrRelateion = false;
            qb.Items.RemoveWhere(d => d.Field == "WorkHourDate");
            string startTime = GetQueryString("WorkHourStart");
            string endTime = GetQueryString("WorkHourEnd");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            SQLHelper shProject = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            SQLHelper shHrTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);

            string fields = @"--项目信息
pri.ID ,pri.Code,pri.Name,pri.ChargeDeptID,pri.ChargeDeptName,pri.State,
--项目经理
pmrbs.userID as ProjectManagerID,pmrbs.userName as ProjectManager,
--参与人数
isnull(whUser.PersonCount,0) as PersonCount,
--工时
isnull(wh.NormalValue,0) as NormalValue,isnull(wh.AdditionalValue,0) as AdditionalValue,
isnull(wh.WorkHourValue,0) as WorkHourValue";

            string sql = "select {0} From dbo.S_I_ProjectInfo pri {1}";

            //查询某项目项目经理，工时填报人数，工时填报统计
            string middleSql = string.Format(@"
LEFT JOIN (
	Select ProjectInfoID,userID,userName,UserDeptID,UserDeptName from dbo.S_W_RBS 
	where RoleCode='ProjectManager' 
	)pmrbs ON pri.ID=pmrbs.ProjectInfoID
LEFT JOIN (
	select ProjectID,count(*) PersonCount from (Select distinct ProjectID,UserID From {0}.dbo.S_W_UserWorkHour  where 1=1 {1}) whi
	group by ProjectID
	)whUser ON pri.ID=whUser.ProjectID
LEFT JOIN (
	select ProjectID,sum(NormalValue) as NormalValue,sum(AdditionalValue) as AdditionalValue,sum(WorkHourValue) as WorkHourValue from {0}.dbo.S_W_UserWorkHour  where 1=1 {1}
	group by ProjectID
	) as wh ON wh.ProjectID=pri.ID", shHrTime.DbName, where);

            //员工按工时类别分类工时合计
            foreach (var item in new string[] { "Market", "Production" })
            {
                middleSql += string.Format(@" Left JOIN (
	Select ProjectID,isnull(sum(WorkHourValue),0) as {0}Hours 
	from {1}.dbo.S_W_UserWorkHour where WorkHourType='{0}' {2} group by ProjectID ) wh{0} On pri.ID= wh{0}.ProjectID ", item, shHrTime.DbName, where);
                fields += string.Format(",isnull({0}Hours,0) {0}Hours", item);
            }

            sql = string.Format(sql, fields, middleSql);

            GridData data = shProject.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetEmployeeByProjectInfoID(QueryBuilder qb)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            string projectInfoID = GetQueryString("ProjectInfoID");
            SQLHelper shProject = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            SQLHelper shHrTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string manufactureDept = OrgType.ManufactureDept.ToString();//生产部门
            //string dept = OrgType.Dept.ToString();

            string field = "wh.UserDeptName,wh.UserDeptID,wh.UserName,wh.UserID as ID,isnull(TotalNormalValue,0) TotalNormalValue,isnull(TotalAdditionalValue,0) TotalAdditionalValue,isnull(TotalWorkHourValue,0) TotalWorkHourValue";
            string sql = @"Select {0} From {2} ";

            //获取员工部门信息，员工工时合计
            string middleSql = string.Format(@" (
    Select UserDeptName,UserDeptID,UserName,UserID From {0}.dbo.S_W_UserWorkHour 
    where ProjectID='{1}' {2} group by userID,UserdeptID,UserdeptName,username
    ) wh 
Left JOIN(
	Select UserID,UserDeptID,sum(NormalValue) as TotalNormalValue,sum(AdditionalValue) as TotalAdditionalValue,sum(WorkHourValue) as TotalWorkHourValue
	from {0}.dbo.S_W_UserWorkHour where ProjectID='{1}'  {2} group by UserID,UserDeptID
	) whTotal ON whTotal.UserID=wh.UserID and whTotal.UserDeptID=wh.UserDeptID ", shHrTime.DbName, projectInfoID, where);

            //员工按工时类别分类工时合计
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");
            foreach (var item in whCategory.EnumItem)
            {
                middleSql += string.Format(@" Left JOIN (
	Select UserID,UserDeptID,isnull(sum(WorkHourValue),0) as {0}Hours 
	from {1}.dbo.S_W_UserWorkHour where WorkHourType='{0}' and ProjectID='{2}' {3} group by UserID,UserDeptID ) wh{0} On whTotal.UserID= wh{0}.UserID and whTotal.UserDeptID=wh{0}.UserDeptID ", item.Code, shHrTime.DbName, projectInfoID, where);
                field += string.Format(",isnull({0}Hours,0) {0}Hours", item.Code);
            }

            sql = string.Format(sql, field, projectInfoID, middleSql);

            GridData data = shProject.ExecuteGridData(sql, qb);

            return Json(data);
        }

        public JsonResult GetProjectWorkHourList(QueryBuilder qb, string userID, string deptID)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));


            string projectInfoID = GetQueryString("ProjectInfoID");
            //获取工时记录
            string sql = string.Format(@"Select ID, ProjectID, ProjectCode, ProjectName, WorkContent, WorkHourDate, 
UserID, UserName, isnull(NormalValue,0) as NormalValue, isnull(AdditionalValue,0) as AdditionalValue, isnull(WorkHourValue,0) as WorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and UserDeptID='{3}' and ProjectID='{1}' {2}  ", userID, projectInfoID, where, deptID);
            //获取工时合计
            string sumSql = string.Format("Select isnull(sum(NormalValue),0) as TotalNormalValue,isnull(sum(AdditionalValue),0) as TotalAdditionalValue,isnull(sum(WorkHourValue),0) as TotalWorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and UserDeptID='{3}' and ProjectID='{1}' {2}  ", userID, projectInfoID, where, deptID);

            SQLHelper shWorkTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            GridData data = shWorkTime.ExecuteGridData(sql, qb);

            return Json(data);
        }

        #endregion

        #region 部门工时统计

        public ActionResult DeptHourRpt()
        {
            var items = EnumBaseHelper.GetEnumDef("System.WorkHourType").EnumItem.ToList();
            ViewBag.WorkHourTypeInfo = items;

            var tab = new Tab();
            var workHourCategory = CategoryFactory.GetCategory("System.DateFilder", "工时日期", "WorkHourDate");
            workHourCategory.SetDefaultItem();
            workHourCategory.Multi = false;
            tab.Categories.Add(workHourCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public ActionResult DeptPersonHourView()
        {
            ViewBag.ColumnHearder = GetWorkHourCategoryColumnHeader();
            return View();
        }

        public JsonResult GetDeptHourList(QueryBuilder qb)
        {
            qb.IsOrRelateion = false;
            qb.Items.RemoveWhere(d => d.Field == "WorkHourDate");
            string startTime = GetQueryString("WorkHourStart");
            string endTime = GetQueryString("WorkHourEnd");
            string where = "";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper shHR = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            string post = OrgType.Post.ToString();
            //int normalHours = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);
            var calendarService = FormulaHelper.GetService<ICalendarService>();
            //int workDaysCount = (int)calendarService.GetWorkDayCount(DateTime.Parse(startTime), DateTime.Parse(endTime)) + 1;

            string fields = @"--部门信息
org.ID ,org.Name as DeptName,
--部门人数
isnull(whUser.PersonCount,0) as PersonCount,
--计划总工时（部门人数*工作日*每日规定工作时间）
--isnull(isnull(whUser.PersonCount,0)*{0}*{1},0) as PlanWorkHourValue,
--正班工时，加班工时
isnull(wh.NormalValue,0) as NormalValue,isnull(wh.AdditionalValue,0) as AdditionalValue,
isnull(wh.WorkHourValue,0) as WorkHourValue,
--人均工时
isnull(case PersonCount when 0 then 0
else (isnull(wh.NormalValue,0)+isnull(wh.AdditionalValue,0))/PersonCount 
end ,0) as AvgHours";//, workDaysCount, normalHours


            string sql = "SELECT {0} From {1}";
            //查询某项目项目经理，工时填报人数，工时填报统计
            string middleSql = string.Format(@"(select * from dbo.S_A_Org where  Type != '{0}'  and IsDeleted='0') org
LEFT JOIN (
	select UserDeptID,count(*) PersonCount from (Select distinct UserDeptID,UserID From {1}.dbo.S_W_UserWorkHour  where 1=1 {2}) whi
	group by UserDeptID
	)whUser ON org.ID=whUser.UserDeptID
LEFT JOIN (
	select UserDeptID,sum(NormalValue) as NormalValue,sum(AdditionalValue) as AdditionalValue,sum(WorkHourValue) as WorkHourValue from {1}.dbo.S_W_UserWorkHour  where 1=1 {2}
	group by UserDeptID
	) as wh ON wh.UserDeptID=org.ID", post, shHR.DbName, where);

            //员工按工时类别分类工时合计
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");
            foreach (var item in whCategory.EnumItem)
            {
                middleSql += string.Format(@" Left JOIN (
	Select UserDeptID,isnull(sum(WorkHourValue),0) as {0}Hours 
	from {1}.dbo.S_W_UserWorkHour where WorkHourType='{0}' {2} group by UserDeptID ) wh{0} On org.ID= wh{0}.UserDeptID ", item.Code, shHR.DbName, where);
                fields += string.Format(",isnull({0}Hours,0) {0}Hours", item.Code);
            }
            sql = string.Format(sql, fields, middleSql);

            GridData data = shBase.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetEmployeeByDeptID(QueryBuilder qb)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            string deptID = GetQueryString("DeptID");
            SQLHelper shWorkTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string manufactureDept = OrgType.ManufactureDept.ToString();//生产部门

            string field = "whTotal.UserName,whTotal.UserID as ID,isnull(TotalNormalValue,0) TotalNormalValue,isnull(TotalAdditionalValue,0) TotalAdditionalValue,isnull(TotalWorkHourValue,0) TotalWorkHourValue";
            string sql = @"Select {0} From  {1} ";

            //获取员工部门信息，员工工时合计
            string middleSql = string.Format(@"(
    Select UserID,UserName,sum(NormalValue) as TotalNormalValue,sum(AdditionalValue) as TotalAdditionalValue ,sum(WorkHourValue) as TotalWorkHourValue
    from {0}.dbo.S_W_UserWorkHour where UserDeptID='{1}' {2} group by UserID,UserName
) whTotal ", shWorkTime.DbName, deptID, where);

            //员工按工时类别分类工时合计
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");
            foreach (var item in whCategory.EnumItem)
            {
                middleSql += string.Format(@" Left JOIN (
	Select UserID,isnull(sum(WorkHourValue),0) as {1}Hours 
	from {0}.dbo.S_W_UserWorkHour where WorkHourType='{1}' and UserDeptID='{2}' {3} group by UserID ) wh{1} On whTotal.UserID= wh{1}.UserID ", shWorkTime.DbName, item.Code, deptID, where);
                field += string.Format(",isnull({0}Hours,0) {0}Hours", item.Code);
            }

            sql = string.Format(sql, field, middleSql);

            GridData data = shBase.ExecuteGridData(sql, qb);

            return Json(data);
        }

        public JsonResult GetDeptWorkHourList(QueryBuilder qb, string userID)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));


            string deptID = GetQueryString("DeptID");
            //获取工时记录
            string sql = string.Format(@"Select ID, ProjectID, ProjectCode, ProjectName, WorkContent, WorkHourDate, 
UserID, UserName, isnull(NormalValue,0) as NormalValue, isnull(AdditionalValue,0) as AdditionalValue, isnull(WorkHourValue,0) as WorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and UserDeptID='{1}' {2} ", userID, deptID, where);
            //获取工时合计
            string sumSql = string.Format("Select isnull(sum(NormalValue),0) as TotalNormalValue,isnull(sum(AdditionalValue),0) as TotalAdditionalValue,isnull(sum(WorkHourValue),0) as TotalWorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and UserDeptID='{1}' {2} ", userID, deptID, where);

            SQLHelper shWorkTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            GridData data = shWorkTime.ExecuteGridData(sql, qb);

            return Json(data);
        }
        #endregion

        #region 人员工时统计

        public ActionResult PersonHourRpt()
        {
            var items = EnumBaseHelper.GetEnumDef("System.WorkHourType").EnumItem.ToList();
            ViewBag.WorkHourTypeInfo = items;

            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "所属部门", "DeptID");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var workHourCategory = CategoryFactory.GetCategory("System.DateFilder", "工时日期", "WorkHourDate");
            workHourCategory.SetDefaultItem();
            workHourCategory.Multi = false;
            tab.Categories.Add(workHourCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public ActionResult PersonProjectHourView()
        {
            ViewBag.ColumnHearder = GetWorkHourCategoryColumnHeader();
            return View();
        }

        public JsonResult GetPersonHourList(QueryBuilder qb)
        {
            qb.IsOrRelateion = false;
            qb.Items.RemoveWhere(d => d.Field == "WorkHourDate");
            string startTime = GetQueryString("WorkHourStart");
            string endTime = GetQueryString("WorkHourEnd");
            string where = "";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper shHR = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            int normalHours = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);
            var calendarService = FormulaHelper.GetService<ICalendarService>();

            string fields = @"--人员部门信息
uo.ID,uo.UserCode,uo.UserName,uo.DeptID,uo.DeptName,
--正班工时，加班工时
isnull(wh.NormalValue,0) as NormalValue,isnull(wh.AdditionalValue,0) as AdditionalValue,isnull(wh.WorkHourValue,0) as WorkHourValue";


            string sql = "SELECT {0} From {1}";
            //查询人员部门信息，工时填报统计
            string middleSql = string.Format(@"(select distinct ID,Code as UserCode,Name as UserName,DeptID,DeptName from dbo.S_A_User
where IsDeleted='0'
union
select distinct UserID as ID,UserCode,UserName,u.DeptID,u.DeptName
from {0}.dbo.S_W_UserWorkHour wh
left join S_A_User u
on wh.UserID=u.ID
where u.IsDeleted='1' {1}) uo
LEFT JOIN (
	select UserID,sum(NormalValue) as NormalValue,sum(AdditionalValue) as AdditionalValue,sum(WorkHourValue) as WorkHourValue from {0}.dbo.S_W_UserWorkHour  where 1=1 {1}
	group by UserID
	) as wh ON wh.UserID=uo.ID", shHR.DbName, where);

            //员工按工时类别分类工时合计
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");
            foreach (var item in whCategory.EnumItem)
            {
                middleSql += string.Format(@" Left JOIN (
	Select UserID,isnull(sum(WorkHourValue),0) as {0}Hours 
	from {1}.dbo.S_W_UserWorkHour where WorkHourType='{0}' {2} group by UserID ) wh{0} On uo.ID= wh{0}.UserID ", item.Code, shHR.DbName, where);
                fields += string.Format(",isnull({0}Hours,0) {0}Hours", item.Code);
            }
            sql = string.Format(sql, fields, middleSql);

            GridData data = shBase.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetProjectByUserID(QueryBuilder qb)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            string userID = GetQueryString("UserID");
            SQLHelper shWorkTime = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper shProject = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            SQLHelper shMarket = SQLHelper.CreateSqlHelper(ConnEnum.Market);

            string field = "whTotal.Name,whTotal.ProjectID as ID,whTotal.Code,whTotal.ChargeUserID,whTotal.ChargeUserName,isnull(TotalNormalValue,0) TotalNormalValue,isnull(TotalAdditionalValue,0) TotalAdditionalValue,isnull(TotalWorkHourValue,0) TotalWorkHourValue";
            string sql = @"Select {0} From  {1} ";

            //获取项目信息，项目工时合计
            string middleSql = string.Format(@"(select total.*,prj.Name,prj.Code,prj.ChargeUserID,prj.ChargeUserName from (
    Select ProjectID,sum(NormalValue) as TotalNormalValue,sum(AdditionalValue) as TotalAdditionalValue,sum(WorkHourValue) as TotalWorkHourValue
    from {0}.dbo.S_W_UserWorkHour where UserID='{1}' {2} group by ProjectID
) total left join (select ID,Name,Code,ChargeUserID,ChargeUserName from {3}.dbo.S_I_ProjectInfo 
 union all select ID,ProjectName as Name,ProjectCode as Code,ChargeUserID='',ChargeUserName='' from {0}.dbo.S_W_ProjectInfo
 union all select ID,Name as Name,SerialNumber as Code,BusinessManager as ChargerUser,BusinessManagerName as ChargerUserName from {4}.dbo.S_P_MarketClue) prj
on total.ProjectID=prj.ID ) whTotal", shWorkTime.DbName, userID, where, shProject.DbName, shMarket.DbName);

            //员工按工时类别分类工时合计
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");
            foreach (var item in whCategory.EnumItem)
            {
                middleSql += string.Format(@" Left JOIN (
	Select ProjectID,isnull(sum(WorkHourValue),0) as {1}Hours 
	from {0}.dbo.S_W_UserWorkHour where WorkHourType='{1}' and UserID='{2}' {3} group by ProjectID ) wh{1} On whTotal.ProjectID= wh{1}.ProjectID ", shWorkTime.DbName, item.Code, userID, where);
                field += string.Format(",isnull({0}Hours,0) {0}Hours", item.Code);
            }

            sql = string.Format(sql, field, middleSql);

            GridData data = shBase.ExecuteGridData(sql, qb);

            return Json(data);
        }

        public JsonResult GetPersonWorkHourList(QueryBuilder qb, string userID, string projectID)
        {
            string startTime = GetQueryString("StartTime");
            string endTime = GetQueryString("EndTime");
            string where = " ";
            if (!string.IsNullOrEmpty(startTime))
                where += string.Format(" and WorkHourDate>='{0}' ", DateTime.Parse(startTime));
            if (!string.IsNullOrEmpty(endTime))
                where += string.Format(" and WorkHourDate<='{0}' ", DateTime.Parse(endTime));

            //获取工时记录
            string sql = string.Format(@"Select ID, ProjectID, ProjectCode, ProjectName, WorkContent, WorkHourDate, 
UserID, UserName, isnull(NormalValue,0) as NormalValue, isnull(AdditionalValue,0) as AdditionalValue, isnull(WorkHourValue,0) as WorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and ProjectID='{2}' {1} ", userID, where, projectID);
            //获取工时合计
            string sumSql = string.Format("Select isnull(sum(NormalValue),0) as TotalNormalValue,isnull(sum(AdditionalValue),0) as TotalAdditionalValue,isnull(sum(WorkHourValue),0) as TotalWorkHourValue from dbo.S_W_UserWorkHour where UserID='{0}' and ProjectID='{2}' {1} ", userID, where, projectID);

            SQLHelper shHR = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            GridData data = shHR.ExecuteGridData(sql, qb);

            return Json(data);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 动态生成工时类别表头
        /// </summary>
        /// <returns></returns>
        private string GetWorkHourCategoryColumnHeader()
        {
            StringBuilder columnHearder = new StringBuilder(@"<div header='工时类别' headeralign='center'><div property='columns'>");
            var whCategory = EnumBaseHelper.GetEnumDef("System.WorkHourType");

            //动态生成工时分类表头
            foreach (var item in whCategory.EnumItem)
            {
                columnHearder.AppendFormat(@" <div field='{0}Hours' headeralign='center' width='70' allowsort='true' align='right'
                                decimalPlaces='1' datatype='float' summarytype='sum'>
                               {1}</div>", item.Code, item.Name);
            }

            columnHearder.Append("</div></div>");

            return columnHearder.ToString();
        }

        #endregion
    }
}
