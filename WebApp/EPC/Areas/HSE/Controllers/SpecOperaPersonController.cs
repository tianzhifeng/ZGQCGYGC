using EPC.Logic.Domain;
using Config.Logic;
using Formula.ImportExport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;

namespace EPC.Areas.HSE.Controllers
{
    public class SpecOperaPersonController : EPCController
    {
        public ActionResult BatchExit()
        {
            return View();
        }

        public JsonResult GetModel()
        {
            return Json("{}");
        }

        public JsonResult SaveNode()
        {
            var ids = GetQueryString("data");
            List<string> idList = JsonConvert.DeserializeObject<List<string>>(ids);
            var formDataCls = new { ID = "", ParentID = "", FullID = "", Type = "", ExitDate = DateTime.Now };
            var formData = JsonConvert.DeserializeAnonymousType(Request.Form["FormData"], formDataCls);
            DateTime exitDate = formData.ExitDate;
            entities.Set<T_HSE_SpecOperaPerson>().Where(a => idList.Contains(a.ID)).Update(a => a.ExitDate = exitDate);
            entities.SaveChanges();
            return Json("{}");
        }
        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var eID = GetQueryString("EngineeringInfoID");
            S_I_Engineering engin = entities.Set<S_I_Engineering>().Find(eID);
            if (engin == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + eID + "的S_I_Engineering");
            }

            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "姓名不能为空";
                }
                else if (e.FieldName == "SubcontractingUnit")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "分包单位不能为空";
                    }
                    else
                    {
                        var tmpQuery = entities.Set<S_P_ContractInfo>().Where(a => a.EngineeringInfoID == eID 
                            && a.ContractProperty == "Construction"
                            && a.ContractState == "Sign");

                        if (!tmpQuery.Any(a => a.PartyBName == e.Value.Trim()))
                        {
                            e.IsValid = false;
                            e.ErrorText = "已签约分包合同中未找到该乙方名称";
                        }
                    }
                }
                else if (e.FieldName == "ExpiryDate" && !String.IsNullOrEmpty(e.Value))
                {
                    DateTime tmpDate = DateTime.Now;
                    if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "有效期限格式不正确";
                    }
                }
                else if (e.FieldName == "EntryDate" && !String.IsNullOrEmpty(e.Value))
                {
                    DateTime tmpDate = DateTime.Now;
                    if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "进场时间格式不正确";
                    }
                }
                else if (e.FieldName == "ExitDate" && !String.IsNullOrEmpty(e.Value))
                {
                    DateTime tmpDate = DateTime.Now;
                    if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "退场时间格式不正确";
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);

            var eID = GetQueryString("EngineeringInfoID");
            S_I_Engineering engin = entities.Set<S_I_Engineering>().Find(eID);
            if (engin == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + eID + "的S_I_Engineering");
            }

            foreach (var item in list)
            {
                T_HSE_SpecOperaPerson detail = new T_HSE_SpecOperaPerson();
                detail.ID = FormulaHelper.CreateGuid();
                detail.EngineeringInfoID = eID;
                detail.Name = item.GetValue("Name");

                var tmpQuery = entities.Set<S_P_ContractInfo>().Where(a => a.EngineeringInfoID == eID
                            && a.ContractProperty == "Construction"
                            && a.ContractState == "Sign");

                string tmpStr = item.GetValue("SubcontractingUnit").Trim();
                var sing = tmpQuery.FirstOrDefault(a => a.PartyBName == tmpStr);
                if (sing != null)
                {
                    detail.SubcontractingUnit = sing.PartyB;
                    detail.SubcontractingUnitName = sing.PartyBName;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException("已签约分包合同中未找到" + tmpStr);
                }
                             
                detail.Sex = item.GetValue("Sex");
                detail.Phone = item.GetValue("Phone");
                detail.IDCard = item.GetValue("IDCard");
                detail.WorkType = item.GetValue("WorkType");
                detail.OperaCerfNum = item.GetValue("OperaCerfNum");
                detail.LicOrg = item.GetValue("LicOrg");

                detail.CreateDate = DateTime.Now;
                detail.CreateUserID = CurrentUserInfo.UserID;
                detail.CreateUser = CurrentUserInfo.UserName;
                detail.CompanyID = CurrentUserInfo.UserCompanyID;
                detail.OrgID = CurrentUserInfo.UserOrgID;

                if (!string.IsNullOrEmpty(item.GetValue("ExpiryDate")))
                    detail.ExpiryDate = Convert.ToDateTime(item.GetValue("ExpiryDate"));
                if (!string.IsNullOrEmpty(item.GetValue("EntryDate")))
                    detail.EntryDate = Convert.ToDateTime(item.GetValue("EntryDate"));
                if (!string.IsNullOrEmpty(item.GetValue("ExitDate")))
                    detail.ExitDate = Convert.ToDateTime(item.GetValue("ExitDate"));

                entities.Set<T_HSE_SpecOperaPerson>().Add(detail);
            }

            entities.SaveChanges();
            return Json("Success");
        }

    }
}
