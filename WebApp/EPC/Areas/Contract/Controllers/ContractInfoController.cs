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
using Formula.Exceptions;
using MvcAdapter;
using Workflow.Logic.Domain;

namespace EPC.Areas.Contract.Controllers
{
    public class ContractInfoController : EPCFormContorllor<T_M_ContractInfo>
    {
        public ActionResult List()
        {
            return View();
        }
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //if (!string.IsNullOrEmpty(GetQueryString("State")))
            //{
            //    dic.SetValue("ContractState", GetQueryString("State"));
            //}
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
          
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "ReceiptObj")
            {
                detail.SetValue("OriginalPlanFinishDate", detail.GetValue("PlanFinishDate"));
                var ReceiptValue = 0.0m;
                var FactReceiptValue = 0.0m;
                //var FactBadValue = 0.0m;
                decimal.TryParse(detail.GetValue("ReceiptValue"), out ReceiptValue);
                decimal.TryParse(detail.GetValue("FactReceiptValue"), out FactReceiptValue);
                //decimal.TryParse(detail.GetValue("FactBadValue"), out FactBadValue);
                //var tmp = Math.Max(FactReceiptValue,FactBadValue);
                if (ReceiptValue < FactReceiptValue)
                {
                    throw new Formula.Exceptions.BusinessValidationException("收款项【" + detail.GetValue("Name") + "】的金额不能小于已收款金额【" + FactReceiptValue + "元】");
                }

                //if ((FactReceiptValue < FactBadValue) && ReceiptValue < FactBadValue)
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("收款项【" + detail.GetValue("Name") + "】的金额不能小于坏账金额【" + tmp + "元】");
                //}
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "ReceiptObj")
            {
                var sumReceiptObjValue = 0m;
                foreach (var item in detailList)
                {
                    sumReceiptObjValue += String.IsNullOrEmpty(item.GetValue("ReceiptValue")) ? 0m : Convert.ToDecimal(item.GetValue("ReceiptValue"));
                  
                }
                var contractRMBValue = String.IsNullOrEmpty(dic.GetValue("ContractRMBValue")) ? 0m : Convert.ToDecimal(dic.GetValue("ContractRMBValue"));
                if (sumReceiptObjValue > contractRMBValue)
                {
                    throw new Formula.Exceptions.BusinessValidationException("收款项金额总计不能大于合同金额");
                }
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //var contract = this.GetEntityByID(dic["ID"]);
            //if (contract == null) contract = this.entities.Set<S_M_ContractInfo>().Create();
            //this.UpdateEntity(contract, dic);
            //contract.SynchContractProperties();
            //contract.SummaryInvoiceData();
            //contract.SummaryReceiptData();
            //contract.SummaryBadDebtData();
            //foreach (var receiptObj in contract.S_M_ContractInfo_ReceiptObj.ToList())
            //{
            //    receiptObj.SummaryReceiptValue();
            //    receiptObj.ResetPlan();
            //}
            //this.EPCEntites.SaveChanges();
        }

        protected override void BeforeDelete(string[] Ids)
        {
            //foreach (var Id in Ids)
            //{
            //    var contract = this.GetEntityByID(Id);
            //    contract.ValidateDelete();
            //}
        }

        public JsonResult RemoveReceiptObj(string ReceiptData)
        {
            var list = JsonHelper.ToList(ReceiptData);
            foreach (var item in list)
            {
                var receiptObject = this.GetEntityByID<S_M_ContractInfo_ReceiptObj>(item.GetValue("ID"));
                if (receiptObject != null)
                {
                    receiptObject.Delete();
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }


        public JsonResult GetDataList(QueryBuilder qb) {
            string sql = @"select cinfo.*,isnull((select top 1 FlowPhase from T_M_ContractInfoChange tcinfo 
where tcinfo.CustomerID = cinfo.ID and VersionNumber> 0
order by ModifyDate desc),'UnChange') ChangeState from T_M_ContractInfo cinfo";
            var data= this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }
        public JsonResult GetDataListDetail(QueryBuilder qb,string T_M_ContractInfo)
        {
            //QueryBuilder qb = new QueryBuilder();
            foreach (ConditionItem item in qb.Items) {
                if (item.Method == QueryMethod.IsEmpty) {
                    item.Method = QueryMethod.Equal;
                    item.Value = "";
                }
            }
            qb.Items.Add(new ConditionItem() { Field = "T_M_ContractInfo", Value = T_M_ContractInfo,Method=0 });
            EPCEntities epc = FormulaHelper.GetEntities<EPCEntities>();
            var data = epc.Set<S_M_ContractInfo_Supplementary>().WhereToGridData<S_M_ContractInfo_Supplementary>(qb);
            return Json(data);
        }

        protected override void OnFlowEnd(T_M_ContractInfo entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var project = EPCEntites.Set<T_I_EngineeringBuild>().FirstOrDefault(c => c.ID == entity.ProjectInfo);
            if (project != null)
            {
                project.Programme = entity.Programme;
                project.ConstructionFeatures = entity.ConstructionFeatures;
                project.ConstructionSite = entity.ConstructionSite;
            }
            EPCEntites.SaveChanges();
        }

    }
}
