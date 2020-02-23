using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;
using Formula.Exceptions;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeProbationaryperiodController : HRFormContorllor<T_Employee_Probationaryperiod>
    {
        // GET: /AutoForm/EmployeeProbationaryperiod/

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //人力资源部意见为延长试用期时，延长时间必填，并将延长后的试用期时间反写至员工合同信息中。
            var id = dic.GetValue("ID");
            var probation = BusinessEntities.Set<T_Employee_Probationaryperiod>().Find(id);
            if (probation.Probationextension != null && probation.Probationextension != 0)
            {
                string employeeID = dic.GetValue("EmployeeID");
                var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);

                if (employee != null)
                {
                    var contract = BusinessEntities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == employee.ID && c.IsUseful == "1").FirstOrDefault();
                    if (contract != null)
                    {
                        contract.PeriodEndDate = ((DateTime)contract.PeriodEndDate).AddMonths((int)(probation.Probationextension));
                        this.BusinessEntities.SaveChanges();
                    }
                }
            }
        }
    }
}
