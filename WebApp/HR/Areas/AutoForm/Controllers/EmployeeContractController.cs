using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeContractController : HRFormContorllor<T_EmployeeAcademicDegree>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            string employeeID = dic.GetValue("EmployeeID");
            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var lastContract = BusinessEntities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == employee.ID).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (lastContract != null)
                {
                    employee.ContractType = lastContract.ContractCategory;
                    employee.DeterminePostsDate = lastContract.PostDate;
                }
            }

            //只有一个合同生效
            var contractList = BusinessEntities.Set<T_EmployeeContract>().Where(p => p.EmployeeID == employeeID);

            var contractId = dic.GetValue("ID");
            var contract = BusinessEntities.Set<T_EmployeeContract>().Find(contractId);

            foreach (var item in contractList)
            {
                if (item.ID != contractId)
                {
                    item.IsUseful = "0";
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
                var lastContract = BusinessEntities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == employee.ID).OrderBy("ContractStartDate", false).FirstOrDefault();
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
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }
    }
}
