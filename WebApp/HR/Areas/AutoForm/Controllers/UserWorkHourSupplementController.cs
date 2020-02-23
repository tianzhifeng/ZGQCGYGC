using Formula;
using HR.Logic;
using HR.Logic.Domain;
using Config.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;

namespace HR.Areas.AutoForm.Controllers
{
    public class UserWorkHourSupplementController : HRFormContorllor<S_W_UserWorkHourSupplement>
    {
        //工时填报与结算类型；Hour：按小时填报按小时结算（默认），HD：按小时填报按天结算，Day：按天填报按天结算
        string workHourType = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkHourType"]) ? WorkHourSaveType.Hour.ToString() :
            System.Configuration.ConfigurationManager.AppSettings["WorkHourType"];

        decimal NormalHoursMax = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]) ? 8 :
            Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);

        decimal maxExtraHour = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]) ? 0 :
               Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]);

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var list = new List<Dictionary<string, object>>();
            var id = dic.GetValue("ID");
            var normalValue = 0m;
            if (!string.IsNullOrEmpty(dic.GetValue("NormalValue")))
                normalValue = Convert.ToDecimal(dic.GetValue("NormalValue"));
            var extraValue = 0m; 
            if (!string.IsNullOrEmpty(dic.GetValue("AdditionalValue")))
                extraValue = Convert.ToDecimal(dic.GetValue("AdditionalValue"));
            if ((normalValue == 0 && extraValue == 0))
            {
                throw new Formula.Exceptions.BusinessException("每日工时不能为0");
            }
            var user = FormulaHelper.GetUserInfoByID(dic.GetValue("UserID"));
            var employee = BusinessEntities.Set<T_Employee>().FirstOrDefault(d => d.UserID == user.UserID);
            var fo = FormulaHelper.CreateFO<WorkHourFO>();
            //根据日期段判断
            var startDate = dic.GetValue("WorkHourDateStart").Replace("T"," ");
            var endDate = dic.GetValue("WorkHourDateEnd").Replace("T", " ");
            var projectID = dic.GetValue("ProjectID");
            var SubProjectCode = dic.GetValue("SubProjectCode");
            var MajorCode = dic.GetValue("MajorCode");
            var WorkContent = dic.GetValue("WorkContent");
            var dateList = getWorkHourDate(startDate, endDate);
            //已存在工时数据
            var userID = dic.GetValue("UserID");
            var existList = this.BusinessEntities.Set<S_W_UserWorkHour>().Where(a => a.UserID == userID && dateList.Contains(a.WorkHourDate) ).ToList();
            foreach (var item in dateList)
            {
                var existNormalValue = existList.Where(a => a.WorkHourDate == item && (a.SupplementID != id || a.SupplementID == null)).Sum(a => a.NormalValue);
                var existExtraValue = existList.Where(a => a.WorkHourDate == item && (a.SupplementID != id || a.SupplementID == null)).Sum(a => a.AdditionalValue);
                if (!existNormalValue.HasValue) existNormalValue = 0;
                if (!existExtraValue.HasValue) existExtraValue = 0;
                //自动补全正班工日，加班工日不补全
                var _normalValue = normalValue;
                if ((_normalValue + existNormalValue) > NormalHoursMax)
                    _normalValue = NormalHoursMax - existNormalValue.Value;
                    //throw new Formula.Exceptions.BusinessException("【" + item.ToShortDateString() + "】正常工时不能大于" + NormalHoursMax.ToString());
                if ((extraValue+existExtraValue) > maxExtraHour)
                    throw new Formula.Exceptions.BusinessException("【" + item.ToShortDateString() + "】加班工时不能大于" + maxExtraHour.ToString());

                if ((_normalValue == 0 && extraValue == 0))
                    continue;

                var userWorkHour = existList.FirstOrDefault(d => d.SupplementID == id
                    && d.UserID == user.UserID && d.WorkHourDate == item);
                if (userWorkHour == null || BusinessEntities.Entry<S_W_UserWorkHour>(userWorkHour).State == System.Data.EntityState.Deleted)
                {
                    userWorkHour = BusinessEntities.Set<S_W_UserWorkHour>().Create();
                    userWorkHour.ID = FormulaHelper.CreateGuid();
                    userWorkHour.UserID = user.UserID;
                    userWorkHour.UserName = user.UserName;
                    userWorkHour.UserDeptID = user.UserOrgID;
                    userWorkHour.UserDeptName = user.UserOrgName;
                    userWorkHour.WorkHourDate = item;
                    userWorkHour.UserCode = user.Code;
                    if (employee != null)
                    {
                        userWorkHour.EmployeeID = employee.ID;
                    }
                    userWorkHour.State = "Create";
                    userWorkHour.IsConfirm = "0";
                    userWorkHour.IsStep1 = "0";
                    userWorkHour.IsStep2 = "0";
                    userWorkHour.CreateUser = CurrentUserInfo.UserName;
                    userWorkHour.CreateUserID = CurrentUserInfo.UserID;
                    userWorkHour.SupplementID = id;
                    userWorkHour.BelongMonth = item.Month;
                    userWorkHour.BelongYear = item.Year;
                    userWorkHour.BelongQuarter = ((item.Month - 1) / 3) + 1;
                    BusinessEntities.Set<S_W_UserWorkHour>().Add(userWorkHour);
                }
                if (userWorkHour.State != "Create")
                    throw new Formula.Exceptions.BusinessException("【" + userWorkHour.WorkHourDate.ToShortDateString() + "】补填的工时已经审批，无法修改");
                userWorkHour.ProjectChargerUser = dic.GetValue("ProjectChargerUser");
                userWorkHour.ProjectChargerUserName = dic.GetValue("ProjectChargerUserName");
                userWorkHour.ProjectCode = dic.GetValue("ProjectCode");
                userWorkHour.ProjectDept = dic.GetValue("ProjectDept");
                userWorkHour.ProjectDeptName = dic.GetValue("ProjectDeptName");
                userWorkHour.ProjectID = projectID;
                userWorkHour.ProjectName = dic.GetValue("ProjectName");
                userWorkHour.SubProjectCode = dic.GetValue("SubProjectCode");
                userWorkHour.SubProjectName = dic.GetValue("SubProjectName");
                userWorkHour.TaskWorkCode = dic.GetValue("TaskWorkCode");
                userWorkHour.TaskWorkName = dic.GetValue("TaskWorkName");
                userWorkHour.WorkContent = dic.GetValue("WorkContent");
                userWorkHour.MajorCode = dic.GetValue("MajorCode");
                userWorkHour.MajorName = dic.GetValue("MajorName");
                userWorkHour.WorkHourType = dic.GetValue("WorkHourType");
                userWorkHour.NormalValue = _normalValue;
                userWorkHour.AdditionalValue = extraValue;
                userWorkHour.WorkHourValue = _normalValue + extraValue;
                userWorkHour.WorkHourDay = fo.ConvertHourToDay(userWorkHour.WorkHourValue, workHourType, NormalHoursMax);
            }
        }

        private List<DateTime> getWorkHourDate(string start,string end)
        {
            var startDate = Convert.ToDateTime(start);
            var endDate = Convert.ToDateTime(end);
            var rtn = new List<DateTime>();
            var baseBusinessEntities = FormulaHelper.GetEntities<Base.Logic.Domain.BaseEntities>();
            var years = new List<int>() { startDate.Year };
            if (endDate.Year != startDate.Year)
                years.Add(endDate.Year);
            var holidayConfig = baseBusinessEntities.Set<S_C_Holiday>().Where(d => d.Year.HasValue && years.Contains(d.Year.Value)).ToList();
            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                bool isholiday = false;
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                    isholiday = true;
                var config = holidayConfig.FirstOrDefault(d => d.Date == i);
                if (config != null)
                {
                    if (config.IsHoliday == "0")
                        isholiday = false;
                    else
                        isholiday = true;
                }
                if (!isholiday)
                    rtn.Add(i);
            }
            return rtn;
        }

        protected override void BeforeDelete(string[] Ids)
        {
            var workourlist = this.BusinessEntities.Set<S_W_UserWorkHour>().Where(a => a.SupplementID != null && Ids.Contains(a.SupplementID)).ToList();
            foreach (var item in workourlist)
            {
                if (item.State != "Create")
                    throw new Formula.Exceptions.BusinessException("【" + item.WorkHourDate.ToShortDateString() + "】补填的工时已经审批，无法删除");
                this.BusinessEntities.Set<S_W_UserWorkHour>().Remove(item);
            }
            //base.BeforeDelete(Ids);
        }
    }
}
