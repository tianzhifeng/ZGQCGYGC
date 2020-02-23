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
    public class ProcurementPaymentApplyController : EPCFormContorllor<T_P_PaymentApply>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var id = dic.GetValue("ID");
            var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
            if (contract == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的合同信息");
            //付款不能超过合同金额
            var paymentValue = String.IsNullOrEmpty(dic.GetValue("ApplyValue")) ? 0m : Convert.ToDecimal(dic.GetValue("ApplyValue"));
            var remainValue = Convert.ToDecimal(contract.S_P_Payment.Select(a => a.PaymentValue).Sum());
            var contractValue = Convert.ToDecimal(contract.ContractAmount);
            var otherPaymentApplyValue = EPCEntites.Set<T_P_PaymentApply>().Where(a => a.ID != id && a.ContractInfo == contract.ID && a.FlowPhase != "End").ToList().Sum(a => a.ApplyValue);//其他流程
            if (otherPaymentApplyValue + paymentValue + remainValue > contractValue)
                throw new Formula.Exceptions.BusinessValidationException("累计申请付款金额不能超过合同总金额【" + contractValue + "元】");
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PaymentObjRelation")
            {
                var planValue = String.IsNullOrEmpty(detail.GetValue("PlanValue")) ? 0m : Convert.ToDecimal(detail.GetValue("PlanValue"));
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                if (relationValue > planValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("PlanName") + "】计划付款关联金额不能大于计划金额【" + planValue + "】");
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PaymentObjRelation")
            {
                var sumRelationValue = detailList.Select(a => String.IsNullOrEmpty(a.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(a.GetValue("RelationValue"))).Sum();
                var receiptValue = String.IsNullOrEmpty(dic.GetValue("ApplyValue")) ? 0m : Convert.ToDecimal(dic.GetValue("ApplyValue"));
                if (sumRelationValue > receiptValue)
                    throw new Formula.Exceptions.BusinessValidationException("付款项或计划关联总金额不能大于申请金额【" + receiptValue + "】");
            }
            else if (subTableName == "DetailInfo")
            {
                if (detailList.Count > 0)
                {
                    var sumRelationValue = detailList.Select(a => String.IsNullOrEmpty(a.GetValue("PaymentValue")) ? 0m : Convert.ToDecimal(a.GetValue("PaymentValue"))).Sum();
                    var receiptValue = String.IsNullOrEmpty(dic.GetValue("ApplyValue")) ? 0m : Convert.ToDecimal(dic.GetValue("ApplyValue"));
                    if (sumRelationValue != receiptValue)
                        throw new Formula.Exceptions.BusinessValidationException("合同明细关联金额总计必须等于申请金额【" + receiptValue + "】");
                }
            }
        }

        protected override void OnFlowEnd(T_P_PaymentApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var payment = entity.Push();

                payment.SetRelateInfo();

                //重新汇总合同上的实际付款数据
                var contract = this.GetEntityByID<S_P_ContractInfo>(entity.ContractInfo);
                contract.SummaryPaymentData();

                //重新汇总合同下各付款项的实际付款数据
                foreach (var item in contract.S_P_ContractInfo_PaymentObj.ToList())
                {
                    item.SummaryPaymentValue();
                }

                payment.ToCost(S_T_DefineParams.Params.GetValue("PaymentAutoSettle").ToLower() == "true");

                var entryInfoIDs = entity.EntryInfo;
                var entryInfos = this.EPCEntites.Set<T_W_WarehouseEntry>().Where(a => entryInfoIDs.Contains(a.ID)).ToList();
                var equipmentAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => entryInfoIDs.Contains(a.FormID)).ToList();
                foreach (var entryInfo in entryInfos)
                {
                    foreach (var equipment in entryInfo.T_W_WarehouseEntry_EquipmentDetail.ToList())
                    {
                        equipment.RMBTotalPrice = equipment.TotalPrice * entity.ExchangeRate;
                        equipment.RMBTaxPrice = equipment.RMBTaxPrice / entryInfo.ExchangeRate * entity.ExchangeRate;
                        equipment.RMBTaxOutPrice = equipment.RMBTotalPrice - equipment.RMBTaxPrice;
                    }
                    entryInfo.ExchangeRate = entity.ExchangeRate;
                }
                foreach (var equipmentAccount in equipmentAccounts)
                {
                    var allAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.RootID == equipmentAccount.ID);
                    foreach (var account in allAccounts)
                    {
                        account.RMBTotalPrice = account.RMBTotalPrice / account.ExchangeRate * entity.ExchangeRate;
                        account.RMBTaxPrice = account.RMBTaxPrice / account.ExchangeRate * entity.ExchangeRate;
                        account.RMBTaxOutPrice = account.RMBTotalPrice - account.RMBTaxPrice;
                        account.ExchangeRate = entity.ExchangeRate;
                    }
                }

                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult GetEntryDetail(string EntryIDs)
        {
            var sql = string.Format("select * from T_W_WarehouseEntry_EquipmentDetail where T_W_WarehouseEntryID in ('{0}')", EntryIDs.Replace(",", "','"));
            var data = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(data);
        }
    }
}
