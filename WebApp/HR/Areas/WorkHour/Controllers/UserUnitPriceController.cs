using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using HR.Logic.Domain;
using Config;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Newtonsoft.Json;
using Formula.ImportExport;
using System.Text.RegularExpressions;

namespace HR.Areas.WorkHour.Controllers
{
    public class UserUnitPriceController : HRController<S_W_UserCostInfo>
    {
        protected override void AfterGetMode(Dictionary<string, object> entityDic, S_W_UserCostInfo entity, bool isNew)
        {
            if (isNew)
            {
                entityDic.SetValue("StartDate", DateTime.Now);
            }
        }

        public JsonResult GetDetailList(QueryBuilder qb,string CostInfoID)
        {
            string sql = @"select T_Employee.ID,T_Employee.UserID,Name as UserName,DeptName,UnitPrice,UserLevel,CostInfoID from T_Employee
left join (select * from S_W_UserUnitPrice where CostInfoID='{0}') S_W_UserUnitPrice on T_Employee.UserID=S_W_UserUnitPrice.UserID
where IsDeleted = '0' ";
            var data = this.SqlHelper.ExecuteGridData(String.Format(sql,CostInfoID),qb);
            return Json(data);
        }

        public JsonResult SaveUserPrice(string MainID, string Data)
        {
            var list = JsonHelper.ToList(Data);
            var costInfo = this.GetEntityByID(MainID);
            if (costInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的内容，无法添加个人单价");
            foreach (var item in list)
            {
                var userID = item.GetValue("UserID");
                var entity = costInfo.S_W_UserUnitPrice.FirstOrDefault(d => d.UserID == userID);
                if (entity == null)
                {
                    entity = new S_W_UserUnitPrice();
                    entity.ID = FormulaHelper.CreateGuid();
                    entity.UserID = item.GetValue("UserID");
                    entity.UserName = item.GetValue("UserName");
                    costInfo.S_W_UserUnitPrice.Add(entity);
                }
                entity.UnitPrice = String.IsNullOrEmpty(item.GetValue("UnitPrice")) ? 0m : Convert.ToDecimal(item.GetValue("UnitPrice"));
                entity.UserLevel = item.GetValue("UserLevel");
            }
            costInfo.ModifyDate = DateTime.Now;
            costInfo.ModifyUser = this.CurrentUserInfo.UserName;
            costInfo.ModifyUserID = this.CurrentUserInfo.UserID;
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateExcel()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "UserName" && string.IsNullOrWhiteSpace(e.Value))
                {
                    if (string.IsNullOrWhiteSpace(e.Value)) {
                        e.IsValid = false;
                        e.ErrorText = string.Format("人员姓名不能为空", e.Value);
                    }
                    else
                    {
                       var employee =  this.entities.Set<T_Employee>().FirstOrDefault(d => d.Name == e.Value);
                       if (employee == null)
                       {
                           e.IsValid = false;
                           e.ErrorText = string.Format("没有找到该人员，请核实人员姓名", e.Value);
                       }
                    }
                  
                }
                if (e.FieldName == "UnitPrice")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("人员工时单价不能为空", e.Value);
                    }
                    else
                    {
                        if (!Regex.IsMatch(e.Value, @"^[-]?\d+[.]?\d*$"))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("人员工时单价只能输入数字", e.Value);
                        }
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcel()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_W_UserUnitPrice>>(tempdata["data"]);
            var currentUser = FormulaHelper.GetUserInfo();
            string costInfoID = this.GetQueryString("CostInfoID");
            var costInfo = this.GetEntityByID(costInfoID);
            if (costInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的内容，无法添加个人单价");
            foreach (var item in list)
            {
                var employee = this.entities.Set<T_Employee>().FirstOrDefault(d => d.Name == item.UserName);
                if (employee == null) continue;
                var userID = employee.UserID;
                var entity = costInfo.S_W_UserUnitPrice.FirstOrDefault(d => d.UserID == userID);
                if (entity == null)
                {
                    entity = new S_W_UserUnitPrice();
                    entity.ID = FormulaHelper.CreateGuid();
                    entity.UserID = employee.UserID;
                    entity.UserName = employee.Name;
                    costInfo.S_W_UserUnitPrice.Add(entity);
                }
                entity.UnitPrice = item.UnitPrice;  
                entity.UserLevel = item.UserLevel;
                this.entities.SaveChanges();
            }
            costInfo.ModifyDate = DateTime.Now;
            costInfo.ModifyUser = currentUser.UserName;
            costInfo.ModifyUserID = currentUser.UserID;
            return Json("Success");
        }
    }
}
