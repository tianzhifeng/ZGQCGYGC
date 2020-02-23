using Config.Logic;
using EPC.Logic.Domain;
using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class EngineeringChangeController : EPCFormContorllor<T_I_EngineeringBuildChange>
    {
        //
        // GET: /Manage/EngineeringChange/

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string EngineeringID = GetQueryString("EngineeringID");
                if (!string.IsNullOrEmpty(EngineeringID))
                {
                    int maxNumber = 0;
                    EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
                    var EngineeringInfo = epcentity.Set<T_I_EngineeringBuild>().FirstOrDefault(a => a.ID == EngineeringID);
                    var Engineerings = epcentity.Set<T_I_EngineeringBuildChange>().Where(c => c.EngineeringID == EngineeringID);
                    if (Engineerings != null && Engineerings.Count() > 0)
                    {
                        maxNumber = Engineerings.ToList().Max(c => Convert.ToInt32(c.VersionNumber));
                    }
                    maxNumber++;
                    if (EngineeringInfo != null)
                    {
                        Type tp = EngineeringInfo.GetType();
                        var ps = tp.GetProperties();
                        DataRow dtr = dt.Rows[0];
                        foreach (PropertyInfo item in ps)
                        {
                            string cname = item.Name;
                            string type = item.PropertyType.Name;
                            if (type != "ICollection`1" && cname != "EngineeringID" && cname != "VersionNumber" && dt.Columns.Contains(cname))
                            {
                                object value = item.GetValue(EngineeringInfo, null);
                                if (cname == "ID")
                                {
                                    dtr.SetField("EngineeringID", value);
                                }
                                else
                                {
                                    dtr.SetField(cname, value);
                                }
                            }
                            dtr.SetField("VersionNumber", maxNumber);
                        }
                    }
                }
            }
        }
        protected override void OnFlowEnd(T_I_EngineeringBuildChange EngineeringInfo, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
            string EngineeringID = EngineeringInfo.EngineeringID;
            var model = epcentity.Set<T_I_EngineeringBuild>().FirstOrDefault(a => a.ID == EngineeringID);
            if (model != null && CurrentUserInfo != null)
            {
                model.Name = EngineeringInfo.Name;
                model.CustomerInfo = EngineeringInfo.CustomerInfo;
                model.CustomerInfoName = EngineeringInfo.CustomerInfoName;
                model.SerialNumber = EngineeringInfo.SerialNumber;
                model.ProjectClass = EngineeringInfo.ProjectClass;
                model.BusinessSector = EngineeringInfo.BusinessSector;
                model.ProjectStatus = EngineeringInfo.ProjectStatus;
                model.MarketingMinorsReason = EngineeringInfo.MarketingMinorsReason;
                model.ProjectLevelDesign = EngineeringInfo.ProjectLevelDesign;
                model.ProjectLevelContract = EngineeringInfo.ProjectLevelContract;
                model.ProjectManagementCategory = EngineeringInfo.ProjectManagementCategory;
                model.Country = EngineeringInfo.Country;
                model.Province = EngineeringInfo.Province;
                model.City = EngineeringInfo.City;
                model.ConstructionSite = EngineeringInfo.ConstructionSite;
                model.Country = EngineeringInfo.Country;
                model.BoxCategory = EngineeringInfo.BoxCategory;
                model.Province = EngineeringInfo.Province;
                model.City = EngineeringInfo.City;
                model.IsNeedBid = EngineeringInfo.IsNeedBid;
                model.ExpectedTenderDate = EngineeringInfo.ExpectedTenderDate;
                model.IsNewEnergy = EngineeringInfo.IsNewEnergy;
                model.NewEnergyType = EngineeringInfo.NewEnergyType;
                model.AllEstimateInvestmentMoney = EngineeringInfo.AllEstimateInvestmentMoney;
                model.EstimatedContractMoney = EngineeringInfo.EstimatedContractMoney;
                model.CoverageArea = EngineeringInfo.CoverageArea;
                model.ConstructionArea = EngineeringInfo.ConstructionArea;
                model.ConstructionFeatures = EngineeringInfo.ConstructionFeatures;
                model.WorkshopComposition = EngineeringInfo.WorkshopComposition;
                model.ConstructionStandard = EngineeringInfo.ConstructionStandard;
                model.OrganizationResources = EngineeringInfo.OrganizationResources;
                model.ConstructionMode = EngineeringInfo.ConstructionMode;
                model.Programme = EngineeringInfo.Programme;
                model.ProductOrModels = EngineeringInfo.ProductOrModels;
                model.Remark = EngineeringInfo.Remark;
                model.ProjectManagerLeader = EngineeringInfo.ProjectManagerLeader;
               // model.IndustryType = EngineeringInfo.IndustryType;
                model.Code = EngineeringInfo.Code;
                model.CodeName = EngineeringInfo.CodeName;
                model.IndustryOne = EngineeringInfo.IndustryOne;
                model.IndustryTwo = EngineeringInfo.IndustryTwo;
                model.IndustryThree= EngineeringInfo.IndustryThree;
                model.IndustryFour = EngineeringInfo.IndustryFour;
                model.IndustryFive = EngineeringInfo.IndustryFive;
                model.IsImpoartProject = EngineeringInfo.IsImpoartProject;
                model.ProjectStartTime = EngineeringInfo.ProjectStartTime;
                model.ProjectEndTime = EngineeringInfo.ProjectEndTime;
                model.ProjectUpdateTime = EngineeringInfo.ProjectUpdateTime;
                epcentity.SaveChanges();
            }
        }

        public JsonResult ValidateChange(string EngineeringID)
        {
            var EngineeringInfo = this.GetEntityByID<T_I_EngineeringBuild>(EngineeringID);
            if (EngineeringInfo.FlowPhase!="End")
            {
                throw new Formula.Exceptions.BusinessValidationException("项目正在审批中，无法启动变更，请在审批结束后再启动变更");
            }
            var result = new Dictionary<string, object>();
            string sql = String.Format("select ID from T_I_EngineeringBuildChange where EngineeringID='{0}' and FlowPhase='{1}'", EngineeringID, "Start");
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                result.SetValue("ID", dt.Rows[0]["ID"]);
                return Json(result);
            }
            sql = String.Format("select Count(ID) from T_I_EngineeringBuildChange where EngineeringID='{0}' and FlowPhase='{1}'", EngineeringID, "Processing");
            var obj = Convert.ToInt32(this.EPCSQLDB.ExecuteScalar(sql));
            if (obj > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("项目正在变更中，无法重复启动变更，请在变更结束后再启动变更");
            }
            return Json(result);
        }


    }
}
