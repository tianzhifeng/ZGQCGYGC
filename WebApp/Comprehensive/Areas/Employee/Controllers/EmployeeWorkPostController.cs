using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Config;
using Config.Logic;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeWorkPostController : ComprehensiveFormController<S_HR_EmployeeWorkPost>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var employee = ComprehensiveDbContext.Set<S_HR_Employee>().Find(dic.GetValue("EmployeeID"));
            if (employee != null)
            {
                //var isMain = dic.GetValue("IsMain");
                var ID = dic.GetValue("ID");
                var postList = ComprehensiveDbContext.Set<S_HR_EmployeeWorkPost>().Where(a => a.EmployeeID == employee.ID && a.ID != ID).ToList();
                foreach (var item in postList)
                {
                    item.IsMain = "0";
                }
                employee.Post = dic.GetValue("Post");
                employee.PostLevel = dic.GetValue("PostLevel");
                ComprehensiveDbContext.SaveChanges();
            }
        }

        public override JsonResult Delete()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            ComprehensiveDbContext.Set<S_HR_EmployeeWorkPost>().Delete(c => idArray.Contains(c.ID));
            ComprehensiveDbContext.SaveChanges();

            var employee = ComprehensiveDbContext.Set<S_HR_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.Post = null;
                employee.PostLevel = null;
                ComprehensiveDbContext.SaveChanges();
            }
            return Json("");
            //return base.Delete();
        }
    }
}
