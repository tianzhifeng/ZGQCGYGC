using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Comprehensive.Logic.BusinessFacade;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeRetiredController : ComprehensiveFormController<S_HR_EmployeeRetired>
    {
        public override JsonResult Save()
        {
            S_HR_EmployeeRetired model = UpdateEntity<S_HR_EmployeeRetired>();
            var employee =  this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(model.EmployeeID);
            if (employee == null)
                throw new Formula.Exceptions.BusinessValidationException("找不到该员工！");
            employee.IsDeleted = "1";
            if (model.Type == "退休")
                employee.EmployeeState = EmployeeState.Retire.ToString();
            else if (employee.EmployeeState == EmployeeState.ReEmploy.ToString())
                employee.EmployeeState = EmployeeState.ReEmployDimission.ToString();
            else
                employee.EmployeeState = EmployeeState.Dimission.ToString();
            employee.DeleteTime = DateTime.Now;
             this.ComprehensiveDbContext.SaveChanges();
            //同步到系统用户表
             EmployeeFo fo = new EmployeeFo();
             fo.EmployeeDeleteToUser(employee);
            return Json(new { ID = model.ID });
            //return base.Save();
        }

        /// <summary>
        /// 撤销离职
        /// </summary>
        /// <returns></returns>
        public JsonResult CancelRetire()
        {

            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            foreach (string id in idArray)
            {
                S_HR_Employee model =  this.ComprehensiveDbContext.Set<S_HR_Employee>().Find(id);
                if (model.EmployeeState == EmployeeState.ReEmployDimission.ToString())
                    model.EmployeeState = EmployeeState.ReEmploy.ToString();
                else
                    model.EmployeeState = EmployeeState.Incumbency.ToString();

                model.IsDeleted = "0";
                model.DeleteTime = null;
            }
             this.ComprehensiveDbContext.SaveChanges();
             //恢复系统账号状态
             EmployeeFo fo = new EmployeeFo();
             fo.ResetSysUserState(listIDs);
            return Json("");
        }

        /// <summary>
        /// 返聘
        /// </summary>
        /// <returns></returns>
        public JsonResult ReEmployee()
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            string reEmploy = EmployeeState.ReEmploy.ToString();
             this.ComprehensiveDbContext.Set<S_HR_Employee>().Where(c => idArray.Contains(c.ID)).Update(c =>
            {
                c.IsDeleted = "0";
                c.EmployeeState = reEmploy;
                c.DeleteTime = null;
            });
             this.ComprehensiveDbContext.SaveChanges();
             //恢复系统账号状态
             EmployeeFo fo = new EmployeeFo();
             fo.ResetSysUserState(listIDs);
            return Json("");
        }

    }
}
