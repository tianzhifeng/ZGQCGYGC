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
    public class EmployeeAcademicDegreeController : ComprehensiveFormController<S_HR_EmployeeAcademicDegree>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(dic.GetValue("EmployeeID"));
            if (employee != null)
            {
                var isMain = dic.GetValue("IsMain");
                var ID = dic.GetValue("ID");
                if (isMain == "1")
                {
                    var otherDegreeList = this.ComprehensiveDbContext.Set<S_HR_EmployeeAcademicDegree>().Where(a => a.EmployeeID == employee.ID && a.ID != ID).ToList();
                    foreach (var degree in otherDegreeList)
                    {
                        degree.IsMain = "0";
                    }
                    employee.Educational = dic.GetValue("Education");
                    employee.EducationalMajor = dic.GetValue("FirstProfession");
                    this.ComprehensiveDbContext.SaveChanges();
                }
            }
            base.AfterSave(dic, formInfo, isNew);
        }

        public override JsonResult Delete()
        {
            string[] idArray = Request["ListIDs"].Split(',');
            string employeeID = GetQueryString("EmployeeID");
            this.ComprehensiveDbContext.Set<S_HR_EmployeeAcademicDegree>().Delete(c => idArray.Contains(c.ID));
            this.ComprehensiveDbContext.SaveChanges();

            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.Educational = null;
                employee.EducationalMajor = null;

                this.ComprehensiveDbContext.SaveChanges();
            }
            return Json("");
        }
    }
}
