using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeAcademicTitleController : HRFormContorllor<T_EmployeeAcademicTitle>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            string employeeID = dic.GetValue("EmployeeID");
            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var atiList = BusinessEntities.Set<T_EmployeeAcademicTitle>().Where(c => c.EmployeeID == employee.ID).ToList();
                 employee.PositionalTitles = string.Join(",", atiList.Select(a => a.Title).Distinct().ToArray());
            }
            this.BusinessEntities.SaveChanges();
        }

        public override JsonResult Delete()
        {
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            string employeeID = GetQueryString("EmployeeID");
            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var atiList = BusinessEntities.Set<T_EmployeeAcademicTitle>().Where(c => c.EmployeeID == employee.ID).ToList();
                employee.PositionalTitles = string.Join(",", atiList.Select(a => a.Title).Distinct().ToArray());
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }
    }
}
