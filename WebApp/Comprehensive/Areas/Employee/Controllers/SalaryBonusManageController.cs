using Comprehensive.Logic.Domain;
using Formula;
using Formula.ImportExport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Comprehensive.Areas.Employee.Controllers
{
    /// <summary>
    /// 工资奖金管理_人员工资管理
    /// </summary>
    public class SalaryBonusManageController : ComprehensiveFormController<T_HR_SalaryManage>
    {

        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
            var orgService = FormulaHelper.GetService<IOrgService>();
            var orgs = orgService.GetOrgs();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Code")
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var user = ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(d => d.Code == e.Value);
                        if (user == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = "该员工在基本信息表中不存在";
                        }
                        if (user.IsDeleted=="0")
                        {
                            e.IsValid = false;
                            e.ErrorText = "该员工已删除";
                        }
                    }
                    else
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("员工编号不能为空", e.Value);
                    }
                }
                //if (e.FieldName == "Employee" && string.IsNullOrWhiteSpace(e.Value))
                //{
                //    e.IsValid = false;
                //    e.ErrorText = string.Format("员工姓名不能为空", e.Value);
                //}
                //if (e.FieldName == "DeptName")
                //{
                //    if (!string.IsNullOrWhiteSpace(e.Value))
                //    {
                //        var dept = orgs.FirstOrDefault(o => o.Name == e.Value);
                //        if (dept == null)
                //        {
                //            e.IsValid = false;
                //            e.ErrorText = string.Format("部门（{0}）不存在！", e.Value);
                //        }
                //    }
                //    else
                //    {
                //        e.IsValid = false;
                //        e.ErrorText = "部门字段不能为空！";
                //    }
                //}
                if (e.FieldName == "Year" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("年份不能为空", e.Value);
                }
                if (e.FieldName == "Month" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("月份不能为空", e.Value);
                }
            });
            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tempdata["data"]);
            //var currentUser = FormulaHelper.GetUserInfo();
            //var orgService = FormulaHelper.GetService<IOrgService>();
            //var orgs = orgService.GetOrgs();
            foreach (var salaryDic in list)
            {
                var salary = new T_HR_SalaryManage();
                UpdateEntity<T_HR_SalaryManage>(salary, salaryDic);
                if (salary.ID == null)
                {
                    salary.ID = FormulaHelper.CreateGuid();
                }
                EntityCreateLogic<T_HR_SalaryManage>(salary);
                var code = salaryDic["Code"];
                var currentInfo = ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(d => d.Code == code && d.IsDeleted == "0");
                if (currentInfo != null)
                {
                    salary.EmployeeName = currentInfo.Name;
                    salary.Employee = currentInfo.ID;
                    salary.DeptName = currentInfo.DeptName;
                    salary.Dept = currentInfo.Dept;
                    salary.Position = currentInfo.Post;
                }

                //if (!String.IsNullOrEmpty(salary.DeptName))
                //{
                //    var dept = orgs.FirstOrDefault(d => d.Name == salary.DeptName);
                //    if (dept != null)
                //    {
                //        salary.Dept = dept.ID;
                //        salary.DeptName = dept.Name;
                //    }
                //}
                //if (string.IsNullOrWhiteSpace(salary.EmployeeName))
                //{
                //    salary.EmployeeName = salary.Employee;
                //}
                this.ComprehensiveDbContext.Set<T_HR_SalaryManage>().Add(salary);
                ComprehensiveDbContext.SaveChanges();
            }
            return Json("Success");
        }

        #endregion
    }
}
