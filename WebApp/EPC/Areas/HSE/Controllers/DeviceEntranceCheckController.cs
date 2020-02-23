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
    public class DeviceEntranceCheckController : EPCController
    {
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
                if (e.FieldName == "DeviceName" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "设备名称不能为空";
                }
                else if (e.FieldName == "DeviceType" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "类型不能为空";
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
                else if (e.FieldName == "Count" && String.IsNullOrEmpty(e.Value))
                {
                    int count = 0;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "现场数量不能为空";
                    }
                    else if (!Int32.TryParse(e.Value, out count))
                    {
                        e.IsValid = false;
                        e.ErrorText = "数量格式不正确";
                    }
                }
                else if (e.FieldName == "EntryDate")
                {
                    DateTime tmpDate = DateTime.Now;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "进场时间不能为空";
                    }
                    else if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "进场时间格式不正确";
                    }
                }
                else if (e.FieldName == "CheckDate")
                {
                    DateTime tmpDate = DateTime.Now;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "检验时间不能为空";
                    }
                    else if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "检验时间格式不正确";
                    }
                }
                else if (e.FieldName == "ExpiryDate")
                {
                    DateTime tmpDate = DateTime.Now;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "有效期不能为空";
                    }
                    else if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "有效期格式不正确";
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
                T_HSE_DeviceEntranceCheck check = new T_HSE_DeviceEntranceCheck();
                check.ID = FormulaHelper.CreateGuid();
                check.EngineeringInfoID = eID;
                check.DeviceNum = item.GetValue("DeviceNum");
                check.DeviceName = item.GetValue("DeviceName");
                check.SpecType = item.GetValue("SpecType");
                check.Manufacturer = item.GetValue("Manufacturer");
                check.ManufactureDate = item.GetValue("ManufactureDate");
                check.DeviceType = item.GetValue("DeviceType");

                var tmpQuery = entities.Set<S_P_ContractInfo>().Where(a => a.EngineeringInfoID == eID
                            && a.ContractProperty == "Construction"
                            && a.ContractState == "Sign");

                string tmpStr = item.GetValue("SubcontractingUnit").Trim();
                var sing = tmpQuery.FirstOrDefault(a => a.PartyBName == tmpStr);
                if (sing != null)
                {
                    check.SubcontractingUnit = sing.PartyB;
                    check.SubcontractingUnitName = sing.PartyBName;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException("已签约分包合同中未找到" + tmpStr);
                }
               
                check.Count = Convert.ToInt32(item.GetValue("Count"));
                check.EntryDate = Convert.ToDateTime(item.GetValue("EntryDate"));
                check.CheckDate = Convert.ToDateTime(item.GetValue("CheckDate"));
                check.ExpiryDate = Convert.ToDateTime(item.GetValue("ExpiryDate"));

                check.CreateDate = DateTime.Now;
                check.CreateUserID = CurrentUserInfo.UserID;
                check.CreateUser = CurrentUserInfo.UserName;
                check.CompanyID = CurrentUserInfo.UserCompanyID;
                check.OrgID = CurrentUserInfo.UserOrgID;

                entities.Set<T_HSE_DeviceEntranceCheck>().Add(check);
            }

            entities.SaveChanges();
            return Json("Success");
        }

    }
}
