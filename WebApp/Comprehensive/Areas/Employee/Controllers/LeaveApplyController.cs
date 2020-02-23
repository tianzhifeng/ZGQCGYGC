using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Formula;
using System.Drawing.Imaging;
using System.Drawing;
using Base.Logic.Domain;
using System.Web.Security;
using Comprehensive.Logic.BusinessFacade;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class LeaveApplyController : ComprehensiveFormController<T_LeaveManage_LeaveApply>
    {
        //此流程结束逻辑用于考勤导入功能
        protected override void OnFlowEnd(T_LeaveManage_LeaveApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == entity.ApplyUser);
            if (user == null) throw new Formula.Exceptions.BusinessValidationException("无法找到申请人【" + entity.ApplyUserName + "】");
            var startDate = DateTime.Parse(entity.StartTime.ToString());
            var endDate = DateTime.Parse(entity.EndTime.ToString());
            var thisDate = startDate;
            var holidayList = baseEntities.Set<S_C_Holiday>().ToList();
            var leave = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "请假").Name;
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
                    //开始日期时间段不是全天时
                    if (thisDate == startDate && entity.StartTimeSlot != "全天")
                    {
                        if (entity.StartTimeSlot == "上午")
                        {
                            attendanceInfo.Morning = leave;
                            attendanceInfo.MorningType = entity.LeaveCategory;
                        }
                        else
                        {
                            attendanceInfo.Afternoon = leave;
                            attendanceInfo.AfternoonType = entity.LeaveCategory;
                        }
                    }
                    //结束日期时间段不是全天时
                    else if (thisDate == endDate && entity.EndTimeSlot != "全天" && entity.EndTimeSlot != "")
                    {
                        if (entity.EndTimeSlot == "上午")
                        {
                            attendanceInfo.Morning = leave;
                            attendanceInfo.MorningType = entity.LeaveCategory;
                        }
                        else
                        {
                            attendanceInfo.Afternoon = leave;
                            attendanceInfo.AfternoonType = entity.LeaveCategory;
                        }
                    }
                    else
                    {
                        attendanceInfo.Morning = leave;
                        attendanceInfo.MorningType = entity.LeaveCategory;
                        attendanceInfo.Afternoon = leave;
                        attendanceInfo.AfternoonType = entity.LeaveCategory;
                    }
                }
                thisDate = thisDate.AddDays(1);
            }
            this.ComprehensiveDbContext.SaveChanges();
        }
    }
}
