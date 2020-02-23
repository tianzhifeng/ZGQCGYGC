using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.ImportExport;

namespace EPC.Areas.HSE.Controllers
{
    public class HazardSourceIdentityController : EPCFormContorllor<T_HSE_HazardSourceIdentity>
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
                T_HSE_HazardSourceIdentity_Detail identity = new T_HSE_HazardSourceIdentity_Detail();
                var temple = EPCEntites.Set<T_HSE_HazardSourceIdentity_Temple>().Find(nodeID);
                if (temple == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("id为" + nodeID + "的T_HSE_HazardSourceIdentity_Temple数据不存在");
                }
                identity.WorkArea = temple.WorkArea;
                identity.Seriousness = temple.Seriousness;
                identity.RiskDegree = temple.RiskDegree;
                identity.Remark = temple.Remark;
                identity.Probably = temple.Probably;
                identity.ProbableEvent = temple.ProbableEvent;
                identity.OrgID = temple.OrgID;
                identity.MajorRisk = temple.MajorRisk;
                identity.ID = FormulaHelper.CreateGuid();
                identity.HazardSource = temple.HazardSource;
                identity.EngineeringInfoID = engineeringInfoID;
                identity.CreateUserID = CurrentUserInfo.UserID;
                identity.CreateUser = CurrentUserInfo.UserName;
                identity.CreateDate = DateTime.Now;
                identity.ControlMeasure = temple.ControlMeasure;
                identity.CompanyID = temple.CompanyID;
                EPCEntites.Set<T_HSE_HazardSourceIdentity_Detail>().Add(identity);
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
                else if (e.FieldName == "HazardSource" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "不能为空";
                }
                else if (e.FieldName == "ProbableEvent" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "不能为空";
                }
                else if (e.FieldName == "Probably")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else
                    {
                        string json = FormulaHelper.GetService<IEnumService>().GetEnumJson("EPC.HazardSourceIdentity.ProbablyType");
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
                else if (e.FieldName == "Seriousness")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else
                    {
                        string json = FormulaHelper.GetService<IEnumService>().GetEnumJson("EPC.HazardSourceIdentity.SeriousnessType");
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
                else if (e.FieldName == "MajorRisk")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "不能为空";
                    }
                    else
                    {
                        string json = FormulaHelper.GetService<IEnumService>().GetEnumJson("System.TrueOrFalse");
                        List<DicItem> dic = JsonConvert.DeserializeObject<List<DicItem>>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
                        if (!dic.Any(a => a.Text == e.Value))
                        {
                            e.IsValid = false;
                            string tmp = "";
                            foreach (var d in dic)
                            {
                                tmp += "【" + d.Text + "】";
                            }
                            e.ErrorText = "只能是" + tmp + "中的一个";
                        }
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
                T_HSE_HazardSourceIdentity_Temple tmp = new T_HSE_HazardSourceIdentity_Temple();
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
                tmp.HazardSource = item.GetValue("HazardSource");
                tmp.ProbableEvent = item.GetValue("ProbableEvent");
                tmp.Probably = Convert.ToInt32(item.GetValue("Probably"));
                tmp.Seriousness = Convert.ToInt32(item.GetValue("Seriousness"));
                tmp.RiskDegree = (tmp.Probably * tmp.Seriousness).ToString();
                tmp.MajorRisk = item.GetValue("MajorRisk") == "是" ? "1" : "0";
                tmp.ControlMeasure = item.GetValue("ControlMeasure");
                tmp.Remark = item.GetValue("Remark");

                EPCEntites.Set<T_HSE_HazardSourceIdentity_Temple>().Add(tmp);
            }

            EPCEntites.SaveChanges();
            return Json("Success");
        }
    }
}
