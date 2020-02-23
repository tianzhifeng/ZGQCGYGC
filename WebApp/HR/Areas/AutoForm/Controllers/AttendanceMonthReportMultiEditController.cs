using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;
using Formula;
using HR.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class AttendanceMonthReportMultiEditController : HRFormContorllor<S_W_AttendanceInfoMultiEdit>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var fo = new AttendanceFO();
            var users = dic.GetValue("UserIDs");
            var dates = dic.GetValue("StartDate");
            var datee = dic.GetValue("EndDate");
            if (string.IsNullOrEmpty(dates) || string.IsNullOrEmpty(datee))
                throw new Formula.Exceptions.BusinessException("请输入日期范围");
            var startDate = Convert.ToDateTime(dates);
            var endDate = Convert.ToDateTime(datee);
            var morning = dic.GetValue("Morning");
            var morningType = dic.GetValue("MorningType");
            var afternoon = dic.GetValue("Afternoon");
            var afternoonType = dic.GetValue("AfternoonType");
            var userIDs = users.Split(',');
            //需要只插入入职以后的考勤数据
            var employeeList = this.BusinessEntities.Set<T_Employee>().Where(a => a.IsDeleted == "0" && userIDs.Contains(a.UserID)).ToList();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (!fo.IsWorkDay(date))
                    continue;
                foreach (var userID in userIDs)
                {
                    //需要只插入入职以后的考勤数据
                    var employee = employeeList.FirstOrDefault(a => a.UserID == userID);
                    DateTime? joinDate = null;//入职时间
                    if (employee != null && employee.JoinCompanyDate.HasValue)
                        joinDate = employee.JoinCompanyDate.Value;
                    if (joinDate.HasValue && date < joinDate)
                        continue;
                    var rdata = this.BusinessEntities.Set<R_W_AttendanceInfo>().FirstOrDefault(a => a.UserID == userID && a.Date.HasValue && a.Date.Value == date);
                    if (rdata == null)
                    {
                        rdata = new R_W_AttendanceInfo();
                        rdata.CreateDate = DateTime.Now;
                        rdata.CreateUser = this.CurrentUserInfo.UserName;
                        rdata.CreateUserID = this.CurrentUserInfo.UserID;
                        rdata.OrgID = this.CurrentUserInfo.UserOrgID;
                        rdata.CompanyID = this.CurrentUserInfo.UserCompanyID;
                        rdata.ID = FormulaHelper.CreateGuid();
                        rdata.UserID = userID;
                        rdata.UserIDName = FormulaHelper.GetUserInfoByID(userID).UserName;
                        rdata.Year = date.Year;
                        rdata.Month = date.Month;
                        this.BusinessEntities.Set<R_W_AttendanceInfo>().Add(rdata);
                    }
                    else
                    {
                        rdata.ModifyDate = DateTime.Now;
                        rdata.ModifyUser = this.CurrentUserInfo.UserName;
                        rdata.ModifyUserID = this.CurrentUserInfo.UserID;
                    }
                    rdata.Date = date;
                    rdata.Morning = morning;
                    rdata.MorningType = morningType;
                    rdata.Afternoon = afternoon;
                    rdata.AfternoonType = afternoonType;
                }
            }
        }
    }
}
