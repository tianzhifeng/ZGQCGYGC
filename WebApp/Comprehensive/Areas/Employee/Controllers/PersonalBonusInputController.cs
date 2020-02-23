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
    /// 个人奖金录入
    /// </summary>
    public class PersonalBonusInputController : ComprehensiveFormController<T_HR_PersonalBonusInput>
    {
        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
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
                        if (user.IsDeleted == "0")
                        {
                            e.IsValid = false;
                            e.ErrorText = "该员工已被删除";
                        }
                    }
                    else
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("员工编号不能为空", e.Value);
                    }
                }
                if (e.FieldName == "SendOutDate" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("发放日期不能为空", e.Value);
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
            foreach (var salaryDic in list)
            {
                var bonus = new T_HR_PersonalBonusInput();
                UpdateEntity<T_HR_PersonalBonusInput>(bonus, salaryDic);
                if (bonus.ID == null)
                {
                    bonus.ID = FormulaHelper.CreateGuid();
                }
                EntityCreateLogic<T_HR_PersonalBonusInput>(bonus);
                var code = salaryDic["Code"];
                var currentInfo = ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(d => d.Code == code && d.IsDeleted == "0");
                if (currentInfo != null)
                {
                    bonus.EmployeeName = currentInfo.Name;
                    bonus.Employee = currentInfo.ID;
                    bonus.DeptName = currentInfo.DeptName;
                    bonus.Dept = currentInfo.Dept;
                    if (string.IsNullOrWhiteSpace(bonus.SendOutDate.ToString()))
                    {
                        DateTime dt = (DateTime)bonus.SendOutDate;
                        bonus.Year = dt.Year.ToString();
                        bonus.Month = dt.Month.ToString();
                    }

                }
                this.ComprehensiveDbContext.Set<T_HR_PersonalBonusInput>().Add(bonus);
                ComprehensiveDbContext.SaveChanges();
            }
            return Json("Success");
        }

        #endregion

    }
}
