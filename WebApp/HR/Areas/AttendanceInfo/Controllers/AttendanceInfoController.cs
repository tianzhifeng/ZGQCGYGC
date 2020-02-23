using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using MvcAdapter;
using Formula;
using HR.Logic;
using Config;
using Config.Logic;
using Formula.Helper;
using Newtonsoft.Json;
using Formula.ImportExport;
using Base.Logic.Domain;
using System.Text;
using Workflow.Logic;

namespace HR.Areas.AttendanceInfo.Controllers
{
    public class AttendanceInfoController : BaseController<S_W_AttendanceInfo>
    {
        public override ActionResult Edit()
        {
            var entity = entities.Set<S_W_AttendanceConfig>().FirstOrDefault();
            if (entity != null)
            {
                ViewBag.AmCheckDate = entity.AmCheckDate.Value.ToShortTimeString();
                ViewBag.AmLateCheckDate = entity.AmLateCheckDate.Value.ToShortTimeString();
                ViewBag.PmCheckDate = entity.PmCheckDate.Value.ToShortTimeString();
                ViewBag.PmEarlyDate = entity.PmEarlyDate.Value.ToShortTimeString();
                ViewBag.Description = entity.Description.Replace("\n", "<br/>");
            }
            else
            {
                ViewBag.AmCheckDate = "9:00";
                ViewBag.AmLateCheckDate = "9:30";
                ViewBag.PmCheckDate = "18:00";
                ViewBag.PmEarlyDate = "17:50";
            }

            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            return base.JsonGetList<V_S_W_AttendanceInfo>(qb);
        }

        public void Sign(string type)
        {
            var user = FormulaHelper.GetUserInfo();
            var dayStartTime = 0;
            var config = this.entities.Set<S_W_AttendanceConfig>().FirstOrDefault();
            if (config != null)
                dayStartTime = ((DateTime)config.PmEndDate).Hour;
            var clickTime = DateTime.Now;

            var Shortdate = clickTime.Date;
            if (clickTime.Hour < dayStartTime)
            {
                Shortdate = clickTime.AddDays(-1).Date;
                if (type == CheckType.SignIn.ToString())
                    return;
            }

            var checkentity = this.entities.Set<V_S_W_AttendanceInfo>().FirstOrDefault(s => s.UserID == user.UserID && s.WorkDay == Shortdate && s.CheckType == type);
            if (checkentity == null)
            {
                var entity = new S_W_AttendanceInfo();
                entity.ID = FormulaHelper.CreateGuid();
                entity.UserID = user.UserID;
                entity.UserName = user.UserName;
                entity.WorkNo = user.WorkNo;
                entity.WorkDay = Shortdate;
                entity.CheckDate = clickTime;
                entity.CheckType = type;
                entity.OrgID = user.UserOrgID;
                entity.OrgName = user.UserOrgName;
                entity.CreateDate = clickTime;
                entity.CreateUserID = user.UserID;
                entity.CreateUserName = user.UserName;
                this.entities.Set<S_W_AttendanceInfo>().Add(entity);
            }
            else
            {
                if (type == CheckType.SignOut.ToString())
                {
                    var entity = this.entities.Set<S_W_AttendanceInfo>().FirstOrDefault(s => s.ID == checkentity.ID);
                    entity.CheckDate = clickTime;
                }
            }
            entities.SaveChanges();
        }

        public JsonResult CheckReport(int year, int month)
        {
            var HasData = false;
            var exsit = this.entities.Set<R_W_AttendanceInfo>().Any(a => a.Year == year && a.Month == month);
            if (exsit)
                HasData = true;
            return Json(new { HasData });
        }

        public JsonResult SaveReport(int year, int month)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();  

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR.ToString());
            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            AttendanceFO fo = FormulaHelper.CreateFO<AttendanceFO>();
            List<DateTime?> shouldAttendanceDays = fo.GetShouldAttendanceDays(year, month); //应出勤日期列表
            if (shouldAttendanceDays == null || shouldAttendanceDays.Count == 0)
                throw new Formula.Exceptions.BusinessException("汇总的年月超过当前年月，无法汇总。");
            DateTime? monthStartDay = Convert.ToDateTime(year + "-" + month.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(2, '0') + "-01");
            DateTime? monthEndDay = Convert.ToDateTime(monthStartDay.Value.AddMonths(1).AddDays(-1).ToShortDateString() + " 23:59:59");

            var workBase = new WorkBase();

            //所有员工条件日期内的每天数据
            var datalist = new List<R_W_AttendanceInfo>();
            //由于 直接执行几千条insert语句在虚拟机服务器执行速度过慢，所以采用插入临表，再复制的方式
            #region sql模板
            string deleteSql = "delete from R_W_AttendanceInfo where year='" + year + "' and month='" + month + "'";
            string sql = deleteSql + @"
--删除临时表
if object_id('tempdb..#R_W_AttendanceInfo') is not null Begin
    drop table #R_W_AttendanceInfo
End

--创建临时表
select top 1 * into #R_W_AttendanceInfo from R_W_AttendanceInfo
delete from #R_W_AttendanceInfo

--将数据插入临时表
begin
print ''
{0}
end

--从临时表复制数据到表
insert into R_W_AttendanceInfo select * from #R_W_AttendanceInfo

--删除临时表
if object_id('tempdb..#R_W_AttendanceInfo') is not null Begin
    drop table #R_W_AttendanceInfo
End
";
            #endregion

            StringBuilder sb = new StringBuilder();

            var userlist = baseEntities.Set<S_A_User>().Where(a => a.IsDeleted == "0").ToList();
            #region 初始化所有人的考勤数据
            //根据日历和传入的起止日期添加所有人的考勤记录  都为缺勤
            //var userIds = this.entities.Set<T_Employee>().Where(a => a.IsDeleted == "0").Select(a=>a.UserID).Distinct().ToList();
            //var userlist = baseEntities.Set<S_A_User>().Where(a => userIds.Contains(a.ID)).ToList();
            //需要只显示入职以后的考勤数据
            var employeeList = this.entities.Set<T_Employee>().Where(a => a.IsDeleted == "0").ToList();
            //考勤列表
            var userInfo = FormulaHelper.GetUserInfo();
            foreach (var user in userlist)
            {
                //需要只显示入职以后的考勤数据
                var employee = employeeList.FirstOrDefault(a => a.UserID == user.ID);
                DateTime? joinDate = null;//入职时间
                if (employee != null && employee.JoinCompanyDate.HasValue)
                    joinDate = employee.JoinCompanyDate.Value;
                var _shouldAttendanceDays = shouldAttendanceDays;
                if (joinDate.HasValue)
                    _shouldAttendanceDays = shouldAttendanceDays.Where(a => a.Value >= joinDate.Value).ToList();
                foreach (var item in _shouldAttendanceDays)
                {
                    var attendanceInfo = new R_W_AttendanceInfo();
                    attendanceInfo.ID = FormulaHelper.CreateGuid();
                    attendanceInfo.CreateDate = DateTime.Now;
                    attendanceInfo.CreateUser = userInfo.UserName;
                    attendanceInfo.CreateUserID = userInfo.UserID;
                    attendanceInfo.OrgID = userInfo.UserOrgID;
                    attendanceInfo.CompanyID = userInfo.UserCompanyID;

                    attendanceInfo.UserID = user.ID;
                    attendanceInfo.UserIDName = user.Name;

                    attendanceInfo.Year = item.Value.Year;
                    attendanceInfo.Month = item.Value.Month;
                    attendanceInfo.Date = item;

                    attendanceInfo.Morning = AttendenceState.Absence.ToString();
                    attendanceInfo.Afternoon = AttendenceState.Absence.ToString();
                    //this.entities.Set<R_W_AttendanceInfo>().Add(attendanceInfo);
                    datalist.Add(attendanceInfo);

                }
            }
            #endregion

            //符合条件日期内的考勤、请假、出差数据
            //V_S_W_AttendanceInfo视图会合并修改后的日期
            var kqlist = entities.Set<V_S_W_AttendanceInfo>().Where(a => a.WorkDay >= monthStartDay && a.WorkDay <= monthEndDay).ToList();
            var leavelist = entities.Set<T_AttendanceLeaveApply>().Where(a =>
                !(a.StartDate < monthStartDay && a.EndDate < monthStartDay)
                && !(a.StartDate > monthEndDay && a.EndDate > monthEndDay) && a.FlowPhase == "End").ToList();
            var outlist = entities.Set<T_AttendanceBusinessApply>().Where(a =>
                !(a.StartDate < monthStartDay && a.EndDate < monthStartDay)
                && !(a.StartDate > monthEndDay && a.EndDate > monthEndDay) && a.FlowPhase == "End").ToList();
            #region 将节假日考勤的记录加入 初始化数据集合，需要统计节假日加班数据
            //将节假日考勤的记录加入 初始化数据集合，需要统计节假日加班数据
            foreach (var item in kqlist.Where(a => !shouldAttendanceDays.Contains(a.WorkDay)))
            {
                var attendanceInfo = new R_W_AttendanceInfo();
                attendanceInfo.ID = FormulaHelper.CreateGuid();
                attendanceInfo.CreateDate = DateTime.Now;
                attendanceInfo.CreateUser = userInfo.UserName;
                attendanceInfo.CreateUserID = userInfo.UserID;
                attendanceInfo.OrgID = userInfo.UserOrgID;
                attendanceInfo.CompanyID = userInfo.UserCompanyID;

                attendanceInfo.UserID = item.UserID;
                attendanceInfo.UserIDName = item.UserName;

                attendanceInfo.Year = item.CheckDate.Value.Year;
                attendanceInfo.Month = item.CheckDate.Value.Month;
                attendanceInfo.Date = item.CheckDate;

                //attendanceInfo.Morning = AttendenceState.Absence.ToString();
                //attendanceInfo.Afternoon = AttendenceState.Absence.ToString();
                //this.entities.Set<R_W_AttendanceInfo>().Add(attendanceInfo);
                datalist.Add(attendanceInfo);
            }
            #endregion

            //判断有没有对应的枚举（普通枚举），如果删除了对应的枚举（如迟到、早退），则不判断对应逻辑（迟到早退当出勤）；
            var cusEnums = EnumBaseHelper.GetEnumDef("HR.AttendanceState");
            if (cusEnums == null)
                throw new Formula.Exceptions.BusinessException("请先维护HR.AttendanceState枚举。");
            var cusEnumItems = cusEnums.EnumItem.ToList();
            var hasComeLate = cusEnumItems.Any(a => a.Code == AttendenceState.ComeLate.ToString());
            var hasLeaveEarly = cusEnumItems.Any(a => a.Code == AttendenceState.LeaveEarly.ToString());
            var hasBusiness = cusEnumItems.Any(a => a.Code == AttendenceState.Business.ToString());
            var hasLeave = cusEnumItems.Any(a => a.Code == AttendenceState.Leave.ToString());
            var hasError = cusEnumItems.Any(a => a.Code == AttendenceState.Error.ToString());

            foreach (var item in datalist)
            {
                #region 每天的状态逻辑
                //当天考勤
                var userKqList = kqlist.Where(a => a.UserID == item.UserID && item.Date.Value.Date == a.WorkDay.Value.Date).ToList();
                var firstSignInkq = userKqList.Where(s => s.CheckType == CheckType.SignIn.ToString()).OrderBy(a => a.CheckDate).FirstOrDefault();
                var lastSignOutkq = userKqList.Where(s => s.CheckType == CheckType.SignOut.ToString()).OrderByDescending(a => a.CheckDate).FirstOrDefault();
                //当天请假
                var userLeaveList = leavelist.Where(a => a.ApplyUser == item.UserID && item.Date >= a.StartDate && item.Date <= a.EndDate).ToList();
                //当天外出
                var userOutList = outlist.Where(a => a.ApplyUser == item.UserID && item.Date >= a.StartDate && item.Date <= a.EndDate).ToList();

                if (workBase.UseAttendanceInfo)
                {
                    var noonTime = DateTime.Parse(item.Date.Value.ToShortDateString() + " 12:00:00");

                    //迟到、早退
                    if (firstSignInkq != null)
                    {
                        item.PostTime = firstSignInkq.CheckDate;
                        //下午签到 :下午正常，不判断迟到
                        if (firstSignInkq.CheckDate.Value >= noonTime)
                        {
                            item.Afternoon = AttendenceState.Normal.ToString();
                        }
                        else
                        {
                            if (hasError && AttendanceFO.IsError(workBase, firstSignInkq.CheckDate))
                                item.Morning = AttendenceState.Error.ToString();
                            else if (hasComeLate && AttendanceFO.IsLate(workBase, firstSignInkq.CheckDate))
                                item.Morning = AttendenceState.ComeLate.ToString();
                            else
                                item.Morning = AttendenceState.Normal.ToString();
                        }
                    }
                    if (lastSignOutkq != null)
                    {
                        item.LeaveTime = lastSignOutkq.CheckDate;
                        //上午签退 :上午正常，不判断早退
                        if (lastSignOutkq.CheckDate.Value < noonTime)
                        {
                            item.Morning = AttendenceState.Normal.ToString();
                        }
                        else
                        {
                            if (hasError && AttendanceFO.IsError(workBase, lastSignOutkq.CheckDate))
                                item.Afternoon = AttendenceState.Error.ToString();
                            else if (hasLeaveEarly && AttendanceFO.IsLeaveEarly(workBase, lastSignOutkq.CheckDate))
                                item.Afternoon = AttendenceState.LeaveEarly.ToString();
                            else
                                item.Afternoon = AttendenceState.Normal.ToString();
                        }
                    }
                }
                else
                {
                    item.Morning = AttendenceState.Normal.ToString();
                    item.Afternoon = AttendenceState.Normal.ToString();
                }

                if (userLeaveList.Count > 0 && hasLeave)
                {
                    //上午请假
                    var amLeave = userLeaveList.FirstOrDefault(a => a.EndDate == item.Date && a.EndFlag == DayFlag.AM.ToString());
                    if (amLeave != null)
                    {
                        item.Morning = AttendenceState.Leave.ToString();
                        item.MorningType = amLeave.LeaveType;
                    }
                    //下午请假
                    var pmLeave = userLeaveList.FirstOrDefault(a => a.StartDate == item.Date && a.StartFlag == DayFlag.PM.ToString());
                    if (pmLeave != null)
                    {
                        item.Afternoon = AttendenceState.Leave.ToString();
                        item.AfternoonType = pmLeave.LeaveType;
                    }
                    //全天请假的情况
                    if (amLeave == null && pmLeave == null)
                    {
                        var dayLeave = userLeaveList.FirstOrDefault();
                        item.Morning = AttendenceState.Leave.ToString();
                        item.MorningType = dayLeave.LeaveType;
                        item.Afternoon = AttendenceState.Leave.ToString();
                        item.AfternoonType = dayLeave.LeaveType;
                    }
                }
                else if (userOutList.Count > 0 && hasBusiness)
                {
                    //上午外出
                    var amOut = userOutList.FirstOrDefault(a => a.EndDate == item.Date && a.EndFlag == DayFlag.AM.ToString());
                    if (amOut != null)
                    {
                        item.Morning = AttendenceState.Business.ToString();
                    }
                    //下午外出
                    var pmOut = userOutList.FirstOrDefault(a => a.StartDate == item.Date && a.StartFlag == DayFlag.PM.ToString());
                    if (pmOut != null)
                    {
                        item.Afternoon = AttendenceState.Business.ToString();
                    }
                    //全天外出的情况
                    if (amOut == null && pmOut == null)
                    {
                        item.Morning = AttendenceState.Business.ToString();
                        item.Afternoon = AttendenceState.Business.ToString();
                    }
                }

                #endregion
                //生成每条考勤insertSql
                var dicStr = CreateInsertSql(item);
                dicStr = dicStr.Replace("R_W_AttendanceInfo", "#R_W_AttendanceInfo");
                sb.AppendLine(dicStr);
            }

            //sw.Stop();
            //TimeSpan ts2 = sw.Elapsed;
            //throw new Formula.Exceptions.BusinessException(string.Format("Stopwatch总共花费{0}s.", (ts2.TotalMilliseconds / 1000))); 
            sql = string.Format(sql, sb.ToString());
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        private string CreateInsertSql(R_W_AttendanceInfo item)
        {
            string sql = string.Empty;
            sql = @"insert into R_W_AttendanceInfo (ID,CreateDate,CreateUserID,CreateUser,OrgID,CompanyID,UserID,UserIDName,Year,Month,
                        Date,Morning,MorningType,PostTime,Afternoon,AfternoonType,LeaveTime) 
                        Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}',{16})";
            sql = string.Format(sql, item.ID,
                item.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                item.CreateUserID, item.CreateUser, item.CompanyID, item.OrgID, item.UserID, item.UserIDName.Replace("'", "''"),
                item.Year.ToString(), item.Month.ToString(), item.Date.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Morning, item.MorningType, item.PostTime.HasValue ? "'" + item.PostTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                item.Afternoon, item.AfternoonType, item.LeaveTime.HasValue ? "'" + item.LeaveTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL"
                );
            return sql;
        }


    }
}
