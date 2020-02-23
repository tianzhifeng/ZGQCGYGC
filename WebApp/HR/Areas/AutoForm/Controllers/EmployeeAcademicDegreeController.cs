using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeAcademicDegreeController : HRFormContorllor<T_EmployeeAcademicDegree>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            string employeeID = dic.GetValue("EmployeeID");
            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var ad = BusinessEntities.Set<T_EmployeeAcademicDegree>().Where(c => c.EmployeeID == employee.ID).OrderBy("GraduationDate", false).FirstOrDefault();
                if (ad != null)
                {
                    if (DateTime.Compare((DateTime)ad.GraduationDate, Convert.ToDateTime(dic.GetValue("GraduationDate"))) > 0)
                    {
                        employee.Educational = ad.Education;
                        employee.EducationalMajor = ad.FirstProfession;
                    }
                    else
                    {
                        employee.Educational = dic.GetValue("Education");
                        employee.EducationalMajor = dic.GetValue("FirstProfession");
                    }
                }
                else
                {
                    employee.Educational = null;
                    employee.EducationalMajor = null;
                }
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
                var ad = BusinessEntities.Set<T_EmployeeAcademicDegree>().Where(c => c.EmployeeID == employee.ID).OrderBy("GraduationDate", false).FirstOrDefault();
                if (ad != null)
                {
                    employee.Educational = ad.Education;
                    employee.EducationalMajor = ad.FirstProfession;
                }
                else
                {
                    employee.Educational = null;
                    employee.EducationalMajor = null;
                }
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }
    }
}
