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
    public class EmployeeJobController : HRFormContorllor<T_EmployeeJob>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //base.BeforeSave(dic, formInfo, isNew);
            if (dic.GetValue("IsMain") == "1")
            {
                var id = dic.GetValue("ID");
                var employeeID = dic.GetValue("EmployeeID");
                var list = BusinessEntities.Set<T_EmployeeJob>().Where(c => c.IsMain == "1" && c.ID != id && c.EmployeeID == employeeID).ToList();
                if (list.Count > 0)
                    throw new BusinessException(string.Format("已存在主责部门职务【{0}:{1}】！", list.FirstOrDefault().DeptIDName, list.FirstOrDefault().JobName));
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //base.AfterSave(dic, formInfo, isNew);
            var updateEmployeeBaseDept = GetQueryString("UpdateEmployeeBaseDept");
            if (updateEmployeeBaseDept == "1")
            {
                var employeeID = dic.GetValue("EmployeeID");
                var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
                if (employee != null)
                    employee.UpdateDeptByJob();
                this.BusinessEntities.SaveChanges();
            }
        }

        public override JsonResult Delete()
        {
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            var updateEmployeeBaseDept = GetQueryString("UpdateEmployeeBaseDept");
            if (updateEmployeeBaseDept == "1")
            {
                string employeeID = GetQueryString("EmployeeID");
                var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
                if (employee != null)
                    employee.UpdateDeptByJob();
                this.BusinessEntities.SaveChanges();
            }
            return Json("");
        }
    }
}
