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

namespace EPC.Areas.Contract.Controllers
{
    public class ReceiptController : EPCFormContorllor<S_M_Receipt>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            var receiptID = dic.GetValue("ID");
            if (!isNew)
            {
                var contract = this.GetEntityByID<S_M_ContractInfo>(dic.GetValue("ContractInfo"));
                if (contract == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的合同信息");
                dic.SetValue("ContractValue", contract.ContractRMBValue);          
                var contractReceiptValue = Convert.ToDecimal(contract.S_M_Receipt.Where(d => d.ID != receiptID).Sum(d => d.ReceiptValue));
                dic.SetValue("ContractReceiptValue", contractReceiptValue);
                var remainReceiptValue = Convert.ToDecimal(contract.ContractRMBValue) - contractReceiptValue;
                if (remainReceiptValue < 0) remainReceiptValue = 0;
                dic.SetValue("RemainReceiptValue", remainReceiptValue);              

                string sql = @"select S_M_Receipt_InvoiceRelation.ID,
S_M_Invoice.InvoiceValue,S_M_Invoice.InvoiceDate, SumRelationInfo.SumRelateValue,
S_M_Receipt_InvoiceRelation.SortIndex
,S_M_Receipt_InvoiceRelation.InvoiceID,
S_M_Receipt_InvoiceRelation.RelationValue
 from S_M_Receipt_InvoiceRelation
left join S_M_Invoice on S_M_Invoice.ID=S_M_Receipt_InvoiceRelation.InvoiceID
left join (select Sum(RelationValue) as SumRelateValue,InvoiceID
from S_M_Receipt_InvoiceRelation where S_M_ReceiptID !='{0}'
group by InvoiceID) SumRelationInfo on  SumRelationInfo.InvoiceID = S_M_Invoice.ID
where S_M_ReceiptID='{0}' ";
                var invoiceRelateionDt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, dic.GetValue("ID")));
                dic.SetValue("InvoiceRelation", JsonHelper.ToJson(invoiceRelateionDt));
            }

            var register = this.GetEntityByID<S_M_ReceiptRegister>(dic.GetValue("RegisterID"));
            if (register != null)
            {
                dic.SetValue("ReceiptRegisterValue", register.ReceiptValue);
                var confirmValue = 0m;
                if (this.EPCEntites.Set<S_M_Receipt>().Count(d => d.ID != receiptID && d.RegisterID == register.ID) > 0)
                    confirmValue = Convert.ToDecimal(this.EPCEntites.Set<S_M_Receipt>().Where(d => d.ID != receiptID && d.RegisterID == register.ID).Sum(d => d.ReceiptValue));
                dic.SetValue("ConfirmValue", confirmValue);
                var remainValue = Convert.ToDecimal(register.ReceiptValue) - confirmValue;
                dic.SetValue("RemainValue", remainValue);
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //判定是否是认领收款，如果是认领收款则要做金额限制校验
            if (!String.IsNullOrEmpty(dic.GetValue("RegisterID")))
            {
                var receiptRegister = this.GetEntityByID<S_M_ReceiptRegister>(dic.GetValue("RegisterID"));
                if (receiptRegister == null) throw new Formula.Exceptions.BusinessValidationException("未能找到对应的到款登记记录，无法认领到款");
                var receiptID = dic.GetValue("ID");
                var confirmValue = this.EPCEntites.Set<S_M_Receipt>().Where(d => d.RegisterID == receiptRegister.ID && d.ID != receiptID).Sum(d => d.ReceiptValue);
                var sumConfirmValue = (confirmValue.HasValue ? confirmValue.Value : 0m) + (String.IsNullOrEmpty(dic.GetValue("ReceiptValue")) ? 0m :
                    Convert.ToDecimal(dic.GetValue("ReceiptValue")));
                //if (sumConfirmValue > receiptRegister.ReceiptValue)
                //    throw new Formula.Exceptions.BusinessValidationException("认领金额总和不能大于收款登记总金额【" + receiptRegister.ReceiptValue + "】");
            }
            if (isNew)
            {
                if (!String.IsNullOrEmpty(dic.GetValue("RegisterID")))
                {
                    dic.SetValue("State", "Create");
                }
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "InvoiceRelation")
            {
                var invoiceValue = String.IsNullOrEmpty(detail.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(detail.GetValue("InvoiceValue"));
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                var invoice = this.GetEntityByID<S_M_Invoice>(detail.GetValue("InvoiceID"));
                if (invoice == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的发票");
                var sumRelationValue = invoice.S_M_Receipt_InvoiceRelation.Where(d => d.ID != detail.GetValue("ID")).Sum(d => d.RelationValue);
                var canRelationValue = invoiceValue - sumRelationValue;
                if ((sumRelationValue + relationValue) > invoiceValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + invoice.InvoiceCode + "】发票冲销金额不能大于可冲销金额【" + canRelationValue + "】");
            }
            else if (subTableName == "PlanRelation")
            {
                var planValue = String.IsNullOrEmpty(detail.GetValue("PlanValue")) ? 0m : Convert.ToDecimal(detail.GetValue("PlanValue"));
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                if (relationValue > planValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("PlanName") + "】计划收款关联金额不能大于计划金额【" + planValue + "】");
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PlanRelation")
            {
                var sumRelationValue = detailList.Select(a => String.IsNullOrEmpty(a.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(a.GetValue("RelationValue"))).Sum();
                var receiptValue = String.IsNullOrEmpty(dic.GetValue("ReceiptValue")) ? 0m : Convert.ToDecimal(dic.GetValue("ReceiptValue"));
                if (sumRelationValue > receiptValue)
                    throw new Formula.Exceptions.BusinessValidationException("计划收款关联总金额不能大于收款金额【" + receiptValue + "】");
                var ids = detailList.Where(c => c.ContainsKey("ID") && !string.IsNullOrEmpty(c["ID"])).Select(c => c["ID"]).ToList();
                var receiptID = dic.GetValue("ID");
                var removeRelations = this.EPCEntites.Set<S_M_Receipt_PlanRelation>().Where(d => d.S_M_ReceiptID == receiptID 
                    && !ids.Contains(d.ID)).ToList();
                foreach (var item in removeRelations)
                {
                    var plan = item.S_M_ContractInfo_ReceiptObj_PlanReceipt;
                    this.EPCEntites.Set<S_M_Receipt_PlanRelation>().Remove(item);
                    if (plan != null)
                    {
                        plan.Reset();
                    }                       
                }
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic.GetValue("ID"));
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("保存失败，未能找到指定的收款记录");     
            //设置到款与发票和计划的关联关系
            entity.SetRelateInfo();

            //重新汇总合同上的实际到款数据
            var contract = this.GetEntityByID<S_M_ContractInfo>(dic.GetValue("ContractInfo"));
            contract.SummaryReceiptData();
            
            //重新汇总合同下各收款项的实际到款数据
            foreach (var item in contract.S_M_ContractInfo_ReceiptObj.ToList())
            {
                item.SummaryReceiptValue();
            }
            //如果是通过到款认领功能进行拆分登记，则需要汇总到款登记上的实际认领金额数据
            if (!String.IsNullOrEmpty(entity.RegisterID))
            {
                var receiptRegister = this.GetEntityByID<S_M_ReceiptRegister>(entity.RegisterID);
                receiptRegister.SumConfirmValue();
            }
            this.EPCEntites.SaveChanges();
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                var ids = Request["ListIDs"].Split(',');
                foreach (var Id in ids)
                {
                    var entity = this.GetEntityByID(Id);
                    if (entity == null) continue;
                    if (entity.Register != this.CurrentUserInfo.UserID && !String.IsNullOrEmpty(entity.RegisterID))
                        throw new Formula.Exceptions.BusinessValidationException("不能删除别人认领的收款信息");
                    entity.Delete();
                    this.EPCEntites.SaveChanges();
                    if (!String.IsNullOrEmpty(entity.RegisterID))
                    {
                        var receiptRegister = this.EPCEntites.Set<S_M_ReceiptRegister>().Find(entity.RegisterID);
                        if (receiptRegister != null)
                        {
                            receiptRegister.SumConfirmValue();
                        }
                    }
                }
                this.EPCEntites.SaveChanges();
            }
            return Json("");
        }

        public JsonResult ValidateModify(string ID)
        {
            var receipt = this.GetEntityByID(ID);
            if (receipt == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的收款记录，无法修订");
            if (!String.IsNullOrEmpty(receipt.RegisterID) && this.EPCEntites.Set<S_M_ReceiptRegister>().Count(d => d.ID == receipt.RegisterID) > 0
                && receipt.State == "Normal")
            {
                throw new Formula.Exceptions.BusinessValidationException("财务已经确认的认领记录不能编辑");
            }
            return Json("");
        }
    }
}
