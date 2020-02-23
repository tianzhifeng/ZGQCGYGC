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
    public class EmployeeContractController : ComprehensiveFormController<S_HR_EmployeeContract>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(dic.GetValue("EmployeeID"));
            if (employee != null)
            {
                var lastContract = this.ComprehensiveDbContext.Set<S_HR_EmployeeContract>().Where(c => c.EmployeeID == employee.ID).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (lastContract != null)
                {
                    employee.ContractType = lastContract.ContractCategory;
                    employee.DeterminePostsDate = lastContract.PostDate;
                    this.ComprehensiveDbContext.SaveChanges();
                }
            }
            base.AfterSave(dic, formInfo, isNew);
        }

        public override JsonResult Delete()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            this.ComprehensiveDbContext.Set<S_HR_EmployeeContract>().Delete(c => idArray.Contains(c.ID));
            this.ComprehensiveDbContext.SaveChanges();

            var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(employeeID);
            if (employee != null)
            {
                var lastContract = this.ComprehensiveDbContext.Set<S_HR_EmployeeContract>().Where(c => c.EmployeeID == employee.ID && !idArray.Contains(c.ID)).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (lastContract != null)
                {
                    employee.ContractType = lastContract.ContractCategory;
                    employee.DeterminePostsDate = lastContract.PostDate;
                }
                else
                {
                    employee.ContractType = "";
                    employee.DeterminePostsDate = null;
                }
                this.ComprehensiveDbContext.SaveChanges();
            }

            return Json("");
        }
    }
}
