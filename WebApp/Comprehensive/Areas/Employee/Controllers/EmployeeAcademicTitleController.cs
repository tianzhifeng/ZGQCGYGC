using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Comprehensive.Logic.BusinessFacade;
using Config;
using Config.Logic;
using System.Data;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeAcademicTitleController : ComprehensiveFormController<S_HR_EmployeeAcademicTitle>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(dic.GetValue("EmployeeID"));
            if (employee != null)
            {
                var isMain = dic.GetValue("IsMain");
                var ID = dic.GetValue("ID");
                var otherTitleList = this.ComprehensiveDbContext.Set<S_HR_EmployeeAcademicTitle>().Where(a => a.EmployeeID == employee.ID && a.ID != ID).ToList();
                foreach (var item in otherTitleList)
                {
                    item.IsMain = "0";
                }
                employee.PositionalTitles = dic.GetValue("Title");
                this.ComprehensiveDbContext.SaveChanges();
            }
            base.AfterSave(dic, formInfo, isNew);
        }

        public override JsonResult Delete()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            this.ComprehensiveDbContext.Set<S_HR_EmployeeAcademicTitle>().Delete(c => idArray.Contains(c.ID));
            this.ComprehensiveDbContext.SaveChanges();

            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.PositionalTitles = "";
                this.ComprehensiveDbContext.SaveChanges();
            }
            return Json("");
        }
    }
}
