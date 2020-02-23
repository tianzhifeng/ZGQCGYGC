using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using Formula.ImportExport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.HSE.Controllers
{
    public class EnvironmentalFactorsIdentificationController : EPCFormContorllor<T_HSE_EnvironmentalFactorsIdentification>
    {
        public JsonResult ImportNodeTemplate()
        {
            var engineeringInfoID = GetQueryString("EngineeringInfoID");
            if (string.IsNullOrEmpty(engineeringInfoID))
            {
                throw new Formula.Exceptions.BusinessValidationException("engineeringInfoID参数不能为空");
            }

            var nodeIDs = GetQueryString("NodeIDs");
            if (string.IsNullOrEmpty(nodeIDs))
            {
                throw new Formula.Exceptions.BusinessValidationException("节点id(NodeIDs)参数不能为空,且格式为id,id,id");
            }

            var nodeIDList = nodeIDs.Split(',');
            foreach (var nodeID in nodeIDList)
            {
                T_HSE_EnvironmentalFactorsIdentification_Detail identity = new T_HSE_EnvironmentalFactorsIdentification_Detail();
                var temple = EPCEntites.Set<T_HSE_EnvironmentalFactorsIdentification_Temple>().Find(nodeID);
                if (temple == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("id为" + nodeID + "的T_HSE_EnvironmentalFactorsIdentification_Temple数据不存在");
                }
                identity.WorkArea = temple.WorkArea;
                identity.TotalValue = temple.TotalValue;
                identity.Tense = temple.Tense;
                identity.State = temple.State;
                identity.RuleValue = temple.RuleValue;
                identity.Remark = temple.Remark;
                identity.RateValue = temple.RateValue;
                identity.OrgID = temple.OrgID;
                identity.IncidenceValue = temple.IncidenceValue;
                identity.ImportEnvironmentalFactor = temple.ImportEnvironmentalFactor;
                identity.ImpactValue = temple.ImpactValue;
                identity.ID = FormulaHelper.CreateGuid();
                identity.EnvironmentalFactor = temple.EnvironmentalFactor;
                identity.EnvironmentalEffect = temple.EnvironmentalEffect;
                identity.CreateUserID = CurrentUserInfo.UserID;
                identity.CreateUser = CurrentUserInfo.UserName;
                identity.CreateDate = DateTime.Now;
                identity.ConcernValue = temple.ConcernValue;
                identity.CompanyID = temple.CompanyID;
                identity.EngineeringInfoID = engineeringInfoID;
                EPCEntites.Set<T_HSE_EnvironmentalFactorsIdentification_Detail>().Add(identity);
            }
            EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "WorkArea" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "不能为空";
                }
                else if (e.FieldName == "EnvironmentalFactor" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "不能为空";
                }
                else if (e.FieldName == "Tense")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else
                    {
                        string json = FormulaHelper.GetService<IEnumService>().GetEnumJson("EPC.EnvironmentalFactorsIdentification.TenseType");
                        List<DicItem> dic = JsonConvert.DeserializeObject<List<DicItem>>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
                        if (!dic.Any(a=>a.Text == e.Value))
                        {
                            e.IsValid = false;
                            string tmp = "";
                            foreach (var d in dic)
                            {
                                tmp += "【" + d.Value + "】";
                            }
                            e.ErrorText = "只能是" + tmp + "中的一个";
                        }
                    }
                }
                else if (e.FieldName == "State")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else
                    {
                        string json = FormulaHelper.GetService<IEnumService>().GetEnumJson("EPC.EnvironmentalFactorsIdentification.StateType");
                        List<DicItem> dic = JsonConvert.DeserializeObject<List<DicItem>>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
                        if (!dic.Any(a => a.Text == e.Value))
                        {
                            e.IsValid = false;
                            string tmp = "";
                            foreach (var d in dic)
                            {
                                tmp += "【" + d.Value + "】";
                            }
                            e.ErrorText = "只能是" + tmp + "中的一个";
                        }
                    }
                }
                else if (e.FieldName == "EnvironmentalEffect" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "不能为空";
                }
                else if (e.FieldName == "IncidenceValue"
                    || e.FieldName == "ImpactValue"
                    || e.FieldName == "RateValue"
                    || e.FieldName == "RuleValue"
                    || e.FieldName == "ConcernValue"
                    )
                {
                    double tmp = 0;
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else if (!double.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "必须为数字";
                    }
                }
                //else if (e.FieldName == "TotalValue")
                //{
                //    double tmp = 0;
                //    if (String.IsNullOrEmpty(e.Value))
                //    {
                //        e.IsValid = false;
                //        e.ErrorText = "不能为空";
                //    }
                //    else if (!double.TryParse(e.Value, out tmp))
                //    {
                //        e.IsValid = false;
                //        e.ErrorText = "必须为数字";
                //    }
                //}
                else if (e.FieldName == "ImportEnvironmentalFactor")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else if (e.Value != "是" && e.Value != "否")
                    {
                        e.IsValid = false;
                        e.ErrorText = "只能为【是】或者【否】";
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

            foreach (var item in list)
            {
                T_HSE_EnvironmentalFactorsIdentification_Temple tmp = new T_HSE_EnvironmentalFactorsIdentification_Temple();
                tmp.ID = FormulaHelper.CreateGuid();
                tmp.CreateDate = DateTime.Now;
                tmp.ModifyDate = DateTime.Now;
                tmp.CreateUserID = CurrentUserInfo.UserID;
                tmp.CreateUser = CurrentUserInfo.UserName;
                tmp.ModifyUserID = CurrentUserInfo.UserID;
                tmp.ModifyUser = CurrentUserInfo.UserName;
                tmp.OrgID = CurrentUserInfo.UserOrgID;
                tmp.CompanyID = CurrentUserInfo.UserCompanyID;
                tmp.WorkArea = item.GetValue("WorkArea");
                tmp.EnvironmentalFactor = item.GetValue("EnvironmentalFactor");
                tmp.Tense = item.GetValue("Tense");
                tmp.State = item.GetValue("State");
                tmp.EnvironmentalEffect = item.GetValue("EnvironmentalEffect");
                tmp.IncidenceValue = Convert.ToDouble(item.GetValue("IncidenceValue"));
                tmp.ImpactValue = Convert.ToDouble(item.GetValue("ImpactValue"));
                tmp.RateValue = Convert.ToDouble(item.GetValue("RateValue"));
                tmp.RuleValue = Convert.ToDouble(item.GetValue("RuleValue"));
                tmp.ConcernValue = Convert.ToDouble(item.GetValue("ConcernValue"));
                tmp.TotalValue = tmp.IncidenceValue + tmp.ImpactValue + tmp.RateValue + tmp.RuleValue + tmp.ConcernValue;
                tmp.ImportEnvironmentalFactor = item.GetValue("ImportEnvironmentalFactor") == "是" ? "1" : "0";
                tmp.Remark = item.GetValue("Remark");

                EPCEntites.Set<T_HSE_EnvironmentalFactorsIdentification_Temple>().Add(tmp);
            }

            EPCEntites.SaveChanges();
            return Json("Success");
        }
    }
}
