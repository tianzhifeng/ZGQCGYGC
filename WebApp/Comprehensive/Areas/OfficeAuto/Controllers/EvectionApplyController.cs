using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using MvcAdapter;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class EvectionApplyController : ComprehensiveFormController<T_Evection_EvectionApply>
    {
        //此流程结束逻辑用于考勤导入功能
        protected override void OnFlowEnd(T_Evection_EvectionApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == entity.ApplyUser);
            if (user == null) throw new Formula.Exceptions.BusinessValidationException("无法找到申请人【" + entity.ApplyUserName + "】");
            var startDate = DateTime.Parse(entity.StartTime.ToString());
            var endDate = DateTime.Parse(entity.EndTime.ToString());
            var thisDate = startDate;
            var holidayList = baseEntities.Set<S_C_Holiday>().ToList();
            var evection = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "出差").Name;
            while (thisDate <= endDate)
            {
                var holiday = holidayList.FirstOrDefault(a => a.Date == thisDate);
                if ((holiday != null && holiday.IsHoliday == "0") ||
                    (holiday == null && thisDate.DayOfWeek != DayOfWeek.Saturday && thisDate.DayOfWeek != DayOfWeek.Sunday))
                {
                    var attendanceInfo = this.ComprehensiveDbContext.Set<S_A_AttendanceInfo>().FirstOrDefault(a => a.Person == user.ID && a.Date == thisDate);
                    if (attendanceInfo == null)
                    {
                        attendanceInfo = new S_A_AttendanceInfo();
                        attendanceInfo.ID = FormulaHelper.CreateGuid();
                        EntityCreateLogic<S_A_AttendanceInfo>(attendanceInfo);
                        attendanceInfo.Person = user.ID;
                        attendanceInfo.PersonName = user.Name;
                        attendanceInfo.Dept = user.DeptID;
                        attendanceInfo.DeptName = user.DeptName;

                        attendanceInfo.Year = thisDate.Year;
                        attendanceInfo.Month = thisDate.Month;
                        attendanceInfo.Date = thisDate.Date;

                        this.ComprehensiveDbContext.Set<S_A_AttendanceInfo>().Add(attendanceInfo);
                    }
                    attendanceInfo.Morning = evection;
                    attendanceInfo.Afternoon = evection;
                }
                thisDate = thisDate.AddDays(1);
            }
            this.ComprehensiveDbContext.SaveChanges();
        }
    }
}
