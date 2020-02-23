using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;

namespace EPC.Areas.Procurement.Controllers
{
    public class PlanSubmitController : EPCFormContorllor<S_P_Plan>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            var engineeringInfoID = dic.GetValue("EngineeringInfoID"); ;
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程项目信息，无法编制采购计划");
            bool isFirstPlan = false;
            bool isFlowEnd = false;
            //先确定是否是首次编制采购 计划
            if (engineeringInfo.S_P_Plan.Count == 0)
            {
                isFirstPlan = true;
                ViewBag.PlanID = "";
            }
            else
            {
                //判定是否有在编辑中的采购计划版本
                var plan = engineeringInfo.S_P_Plan.Where(c => c.FlowPhase != "End").FirstOrDefault();
                if (plan == null)
                {
                    plan = engineeringInfo.S_P_Plan.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
                    isFlowEnd = true;
                }
                ViewBag.PlanID = plan.ID;
            }
            ViewBag.IsFlowEnd = isFlowEnd;
            ViewBag.IsFirstPlan = isFirstPlan;
            dic.SetValue("ShowType", "Diff"); //默认只显示差异内容
            var list = engineeringInfo.PBSRoot.Children.OrderBy(c => c.SortIndex).ToList();
            var belongEnum = new List<Dictionary<string, object>>();
            foreach (var pbs in list)
            {
                var enumDic = new Dictionary<string, object>();
                enumDic.SetValue("text", pbs.Name);
                enumDic.SetValue("value", pbs.ID);
                belongEnum.Add(enumDic);
            }
            ViewBag.PBSRelation = JsonHelper.ToJson(belongEnum);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetPackageList(string planID, string ShowType)
        {
            var list = new List<S_P_Plan_Package>();
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                list = this.EPCEntites.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID && c.ModifyState != "Normal").OrderBy(c => c.SortIndex).ToList();
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不显示已经删除的数据
                list = this.EPCEntites.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID && c.ModifyState != "Remove").OrderBy(c => c.SortIndex).ToList();
            }
            else
            {
                //显示全部数据，体现差异
                list = this.EPCEntites.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID).OrderBy(c => c.SortIndex).ToList();
            }
            return Json(list);
        }

        public JsonResult GetItemList(string PackageID, string ShowType)
        {
            string sql = @"select dbo.S_P_Plan_Package_Item.ID as RelationID,S_P_Plan_Package_Item.VersionNo as PlanVersion,
 S_P_Bom.*,ItemQuantity,ModifyState from S_P_Plan_Package_Item
left join S_P_Bom on S_P_Plan_Package_Item.BomID=
S_P_Bom.ID  where PackageID='{0}'";
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                sql += " and S_P_Plan_Package_Item.ModifyState!='" + BomVersionModifyState.Normal.ToString() + "'";
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不显示已经删除的数据
                sql += " and S_P_Plan_Package_Item.ModifyState!='" + BomVersionModifyState.Remove.ToString() + "'";
            }
            sql += " order by BomID ";
            var dt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, PackageID));
            return Json(dt);
        }

        protected override void OnFlowEnd(S_P_Plan entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                if (entity.S_I_Engineering == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有关联任何工程的采购计划无法进行发布");
                }
                entity.Push();
                this.EPCEntites.SaveChanges();
                entity.S_I_Engineering.ProcurementPlanToPBom();
                this.EPCEntites.SaveChanges();
            }          
        }
    }
}
