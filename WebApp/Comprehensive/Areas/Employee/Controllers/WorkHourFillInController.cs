using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Comprehensive.Logic.Domain;
using Comprehensive.Logic;
using Formula;
using Config.Logic;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class WorkHourFillInController : ComprehensiveFormController<S_W_UserWorkHour>
    {
        decimal maxNormaValue = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]) ? 8 :
            Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);

        decimal maxEtraValue = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]) ? 0 :
               Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]);

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var userID = dic.GetValue("UserID");
            var workHourID = dic.GetValue("ID");
            DateTime workHourDate = DateTime.Parse(dic.GetValue("WorkHourDate"));
            var workHourList = this.ComprehensiveDbContext.Set<S_W_UserWorkHour>().Where(d => d.ID != workHourID && d.UserID == userID && d.WorkHourDate == workHourDate).ToList();

            var normalValue = String.IsNullOrEmpty(dic.GetValue("NormalValue")) ? 0 : Convert.ToDecimal(dic.GetValue("NormalValue"));
            var extraValue = String.IsNullOrEmpty(dic.GetValue("AdditionalValue")) ? 0 : Convert.ToDecimal(dic.GetValue("AdditionalValue"));

            foreach (var item in workHourList)
            {
                normalValue += item.NormalValue.HasValue ? item.NormalValue.Value : 0;
                extraValue += item.AdditionalValue.HasValue ? item.AdditionalValue.Value : 0;
            }

            if (normalValue > maxNormaValue)
                throw new Formula.Exceptions.BusinessValidationException("【" + workHourDate.ToShortDateString() + "】的正班工时不能大于" + maxNormaValue.ToString());
            if (extraValue > maxEtraValue)
                throw new Formula.Exceptions.BusinessValidationException("【" + workHourDate.ToShortDateString() + "】的加班工时不能大于" + maxEtraValue.ToString());

            var userInfo = FormulaHelper.GetUserInfoByID(userID);
            if (String.IsNullOrEmpty(dic.GetValue("UserCode")))
                dic.SetValue("UserCode", userInfo.Code);
            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(d => d.UserID == userID);
            if (employee != null)
                dic.SetValue("EmployeeID", employee.ID);

            this.ComprehensiveDbContext.SaveChanges();
            base.BeforeSave(dic, formInfo, isNew);
        }
    }
}
