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
    public class SafetyEducationTrainingController : EPCController
    {
        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            //var spID = GetQueryString("SpecOperaPersonID");
            //T_HSE_SpecOperaPerson operaPerson = entities.Set<T_HSE_SpecOperaPerson>().Find(spID);
            //if (operaPerson == null)
            //{
            //    throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + spID + "的T_HSE_SpecOperaPerson");
            //}

            var errors = excelData.Vaildate(e =>
            {
                //if (e.FieldName == "SpecOperaPersonName" && String.IsNullOrEmpty(e.Value))
                //{
                //    if (String.IsNullOrEmpty(e.Value))
                //    {
                //        e.IsValid = false;
                //        e.ErrorText = "作业人员不能为空";
                //    }
                //    else
                //    {
                //        var res = entities.Set<T_HSE_SpecOperaPerson>().SingleOrDefault(a => a.Name == e.Value.Trim());
                //        if (res == null)
                //        {
                //            e.IsValid = false;
                //            e.ErrorText = e.Value + "的作业人员不存在";
                //        }
                //    }
                //}
                if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "培训名称不能为空";
                }
                else if (e.FieldName == "TrainingType" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "培训类型不能为空";
                }
                else if (e.FieldName == "BeginDate")
                {
                    DateTime tmpDate = DateTime.Now;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "开始时间不能为空";
                    }
                    else if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "开始时间格式不正确";
                    }
                }
                else if (e.FieldName == "EndDate")
                {
                    DateTime tmpDate = DateTime.Now;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "结束时间不能为空";
                    }
                    else if (!DateTime.TryParse(e.Value, out tmpDate))
                    {
                        e.IsValid = false;
                        e.ErrorText = "结束时间格式不正确";
                    }
                }
                else if (e.FieldName == "Hours" && !String.IsNullOrEmpty(e.Value))
                {
                    int hours = 0;
                    if (!int.TryParse(e.Value, out hours))
                    {
                        e.IsValid = false;
                        e.ErrorText = "学时格式不正确";
                    }
                }
                else if (e.FieldName == "NumOfPeople" && !String.IsNullOrEmpty(e.Value))
                {
                    int hours = 0;
                    if (!int.TryParse(e.Value, out hours))
                    {
                        e.IsValid = false;
                        e.ErrorText = "人数格式不正确";
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

            //var qID = GetQueryString("SpecOperaPersonID");
            //T_HSE_SpecOperaPerson quantity = entities.Set<T_HSE_SpecOperaPerson>().Find(qID);
            //if (quantity == null)
            //{
            //    throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + qID + "的T_HSE_SpecOperaPerson");
            //}

            foreach (var item in list)
            {
                T_HSE_SafetyEducationTraining detail = new T_HSE_SafetyEducationTraining();
                detail.ID = FormulaHelper.CreateGuid();

                //var res = entities.Set<T_HSE_SpecOperaPerson>().SingleOrDefault(a => a.Name == item.GetValue("SpecOperaPersonName").Trim());
                //if (res == null)
                //{
                //    throw new Formula.Exceptions.BusinessValidationException(item.GetValue("SpecOperaPersonName").Trim() + "的作业人员不存在");
                //}

                //detail.SpecOperaPersonID = res.ID;
                detail.Name = item.GetValue("Name");
                detail.TrainingType = item.GetValue("TrainingType");
                detail.BeginDate = Convert.ToDateTime(item.GetValue("BeginDate"));
                detail.EndDate = Convert.ToDateTime(item.GetValue("EndDate"));

                detail.Lecturer = item.GetValue("Lecturer");
                detail.Location = item.GetValue("Location");
                if (!string.IsNullOrEmpty(item.GetValue("Hours")))
                    detail.Hours = Convert.ToDouble(item.GetValue("Hours"));
                if (!string.IsNullOrEmpty(item.GetValue("NumOfPeople")))
                    detail.NumOfPeople = Convert.ToInt32(item.GetValue("NumOfPeople"));

                detail.CreateDate = DateTime.Now;
                detail.CreateUserID = CurrentUserInfo.UserID;
                detail.CreateUser = CurrentUserInfo.UserName;
                detail.CompanyID = CurrentUserInfo.UserCompanyID;
                detail.OrgID = CurrentUserInfo.UserOrgID;

                entities.Set<T_HSE_SafetyEducationTraining>().Add(detail);
            }

            entities.SaveChanges();
            return Json("Success");
        }
    }
}
