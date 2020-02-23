using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using MvcAdapter;

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementContractPaymentPlanController : EPCFormContorllor<S_P_ContractInfo_PaymentObj_PaymentPlan>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (!isNew)
            {
                var plan = this.GetEntityByID(dic.GetValue("ID"));
                if (plan.FactValue.HasValue && plan.FactValue.Value > 0)
                    throw new Formula.Exceptions.BusinessValidationException("计划【" + plan.PaymentObjName + "】已经有付款，无法修改。");
                if (plan.State == PlanPaymentState.UnFinished.ToString())
                    throw new Formula.Exceptions.BusinessValidationException("计划【" + plan.PaymentObjName + "】未完成，无法修改。");
            }
            //base.BeforeSave(dic, formInfo, isNew);
            var date = DateTime.Now;
            DateTime.TryParse(dic.GetValue("PlanDate"), out date);

            dic.SetValue("BelongYear", date.Year.ToString());
            dic.SetValue("BelongMonth", date.Month.ToString());
            dic.SetValue("BelongQuarter", ((date.Month - 1) / 3 + 1).ToString());
            //计划付款款不能大于可付款（付款项-该付款项实际付款合计）
            var planValue = String.IsNullOrEmpty(dic.GetValue("PlanValue")) ? 0m : Convert.ToDecimal(dic.GetValue("PlanValue"));
            var remainValue = 0m;
            var paymentObj = this.GetEntityByID<S_P_ContractInfo_PaymentObj>(dic.GetValue("PaymentObj"));
            if (paymentObj != null)
            {
                var paymentValue =Convert.ToDecimal(paymentObj.PlanPaymentValue);
                remainValue = paymentValue - Convert.ToDecimal(paymentObj.S_P_ContractInfo_PaymentObj_PaymentPlan.Where(a => a.ID != dic.GetValue("ID")).Sum(a => a.FactValue));
                if (planValue > remainValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + paymentObj.Name + "】计划金额不能大于可付款金额【" + remainValue + "】");
            }
        }

        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                //有实际付款的 不能删除
                var plan = this.GetEntityByID(Id);
                if (plan != null)
                {
                    if (plan.FactValue.HasValue && plan.FactValue.Value > 0)
                        throw new Formula.Exceptions.BusinessValidationException("计划【" + plan.PaymentObjName + "】已经有付款，无法删除。");
                }
            }
        }

        public ActionResult MultiContractPaymentObjAdd()
        {
            ViewBag.EngineeringInfoID = this.GetQueryString("EngineeringInfoID");
            return View();
        }

        public JsonResult SavePlanList(string PlanList)
        {
            var list = JsonHelper.ToList(PlanList);
            foreach (var dic in list)
            {
                var date = DateTime.Now;
                DateTime.TryParse(dic.GetValue("PlanDate"), out date);

                dic.SetValue("BelongYear", date.Year.ToString());
                dic.SetValue("BelongMonth", date.Month.ToString());
                dic.SetValue("BelongQuarter", ((date.Month - 1) / 3 + 1).ToString());
                //计划付款款不能大于可付款（付款项-该付款项实际付款合计）
                var planValue = String.IsNullOrEmpty(dic.GetValue("PlanValue")) ? 0m : Convert.ToDecimal(dic.GetValue("PlanValue"));
                var remainValue = 0m;
                var paymentObj = this.GetEntityByID<S_P_ContractInfo_PaymentObj>(dic.GetValue("PaymentObj"));
                if (paymentObj != null)
                {
                    var state = PlanPaymentState.UnPayment.ToString();
                    var unFinish = PlanPaymentState.UnFinished.ToString();
                    var otherList = paymentObj.S_P_ContractInfo_PaymentObj_PaymentPlan.Where(a => a.State == state).ToList();
                    if(otherList.Count>0)
                        otherList.Update(a => a.State = unFinish); //每个付款项至多只有一条未付计划
                    var paymentValue = Convert.ToDecimal(paymentObj.PlanPaymentValue);
                    remainValue = paymentValue - Convert.ToDecimal(paymentObj.S_P_Payment_PaymentObjRelation.Sum(d => d.RelationValue));
                    if (planValue > remainValue)
                        throw new Formula.Exceptions.BusinessValidationException("【" + paymentObj.Name + "】计划金额不能大于可付款金额【" + remainValue + "】");
                }
                var plan = new S_P_ContractInfo_PaymentObj_PaymentPlan();
                plan.ID = FormulaHelper.CreateGuid();
                plan.State = PlanPaymentState.UnPayment.ToString();
                plan.CompanyID = CurrentUserInfo.UserCompanyID;
                EntityCreateLogic<S_P_ContractInfo_PaymentObj_PaymentPlan>(plan);
                UpdateEntity<S_P_ContractInfo_PaymentObj_PaymentPlan>(plan, dic);
                this.EPCEntites.Set<S_P_ContractInfo_PaymentObj_PaymentPlan>().Add(plan);
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        //获取合同列表
        public JsonResult GetContractList(QueryBuilder qb, string PartyB, string EngineeringInfoID)
        {
            string sql = @"select * from (
select con.*,isnull(ContractAmount,0)-isnull(SumInvoiceValue,0) as NoInvoiceAmount
,isnull(ContractAmount,0)-isnull(SumPaymentValue,0) as NoPaymentAmount from S_P_ContractInfo con
left join S_I_Engineering eng on eng.ID=con.EngineeringInfoID) tmp";
            qb.Add("ContractState", QueryMethod.Equal, "Sign");
            qb.Add("ContractProperty", QueryMethod.Equal, "Procurement");
            if (!String.IsNullOrEmpty(EngineeringInfoID))
                qb.Add("EngineeringInfoID", QueryMethod.Equal, EngineeringInfoID);
            if (!String.IsNullOrEmpty(PartyB))
                qb.Add("PartyB", QueryMethod.Equal, PartyB);
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetPaymentObjectList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @"select *,(isnull(PlanPaymentValue,0)-isnull(SumInvoiceValue,0)) as RemainInvoiceValue,
(isnull(PlanPaymentValue,0)-isnull(SumPaymentValue,0)) as RemainPaymentValue 
from dbo.S_P_ContractInfo_PaymentObj where S_P_ContractInfoID ='{0}' {1} order by SortIndex ";
            var whereStr = qb.GetWhereString(false);
            sql = String.Format(sql, ContractInfoID, whereStr);
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }

    }
}
