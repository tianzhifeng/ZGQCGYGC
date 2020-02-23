using EPC.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using EPC.Logic;
using System.Text.RegularExpressions;
using Formula;

namespace EPC.Areas.Construction.Controllers
{
    public class ContractInfoBOQPayCheckController : EPCFormContorllor<S_P_ContractInfo_Fb_BOQPayCheck>
    {
        protected override void OnFlowEnd(Logic.Domain.S_P_ContractInfo_Fb_BOQPayCheck entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            S_P_ContractInfo contract = EPCEntites.Set<S_P_ContractInfo>().Find(entity.ContractInfoID);
            if (contract == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("ID为" + entity.ContractInfoID + "的合同已不存在");
            }
            var detailSummary = EPCEntites.Set<S_P_ContractInfo_Fb_BOQPayCheck_Detail>()
                .SingleOrDefault(a => a.S_P_ContractInfo_Fb_BOQPayCheckID == entity.ID && a.PayCheckDetailType == (int)PayCheckDetailTemplateType.Summary);

            if (detailSummary == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("找不到工程价款项");
            }
            //合同付款项
            S_P_ContractInfo_PaymentObj paymentObj = new S_P_ContractInfo_PaymentObj();
            paymentObj.ContractFullID = entity.ContractInfoID;
            paymentObj.ID = FormulaHelper.CreateGuid();
            paymentObj.Name = "预付";
            paymentObj.PayType = "预付";
            paymentObj.PlanPaymentValue = detailSummary.ConfirmPrice;
            paymentObj.S_P_ContractInfoID = entity.ContractInfoID;
            paymentObj.Scale = 100;
            EPCEntites.Set<S_P_ContractInfo_PaymentObj>().Add(paymentObj);

            //付款数据
            var payment = EPCEntites.Set<S_P_Payment>().Find(entity.ID);
            if (payment == null)
            {
                payment = new S_P_Payment();
                payment.ID = entity.ID;
                payment.State = PaymentState.Normal.ToString();
                EPCEntites.Set<S_P_Payment>().Add(payment);
            }

            payment.OrgID = entity.OrgID;
            payment.CompanyID = entity.CompanyID;
            payment.ContractInfo = entity.ContractInfoID;
            payment.ContractInfoName = entity.ContractInfoName;
            payment.SupplierInfo = contract.PartyB;
            payment.SupplierInfoName = contract.PartyBName;
            payment.EngineeringInfoID = contract.EngineeringInfoID;
            payment.CreateDate = DateTime.Now;
            payment.CreateUser = CurrentUserInfo.UserName;
            payment.CreateUserID = CurrentUserInfo.UserID;
            payment.OrgID = CurrentUserInfo.UserOrgID;
                      
            payment.PaymentValue = detailSummary.ConfirmPrice;
            payment.PaymentValueDX = ConvertToChinese(detailSummary.ConfirmPrice ?? 0);
            payment.PaymentRMBAmount = detailSummary.ConfirmPrice;
            payment.Register = entity.CreateUserID;
            payment.RegisterName = entity.CreateUser;
            payment.PaymentDate = DateTime.Now;
            payment.S_P_ContractInfo = contract;
            contract.S_P_Payment.Add(payment);

            //付款数据关联合同付款项
            var detail = new S_P_Payment_PaymentObjRelation();
            detail.S_P_ContractInfo_PaymentObj = paymentObj;
            detail.ID = FormulaHelper.CreateGuid();
            detail.PaymentObjID = paymentObj.ID;
            detail.S_P_PaymentID = payment.ID;
            detail.RelationValue = paymentObj.PlanPaymentValue;
            EPCEntites.Set<S_P_Payment_PaymentObjRelation>().Add(detail);

            contract.SumPaymentValue = (contract.SumPaymentValue ?? 0) + detailSummary.ConfirmPrice;
            payment.ToCost(S_T_DefineParams.Params.GetValue("BOQPayAutoSettle").ToLower() == "true");
            EPCEntites.SaveChanges();
        }

        public JsonResult CheckSpecialDetailTempleCanAdd(string tempTypeValue)
        {
            string id = GetQueryString("ID");
            int templateType = Convert.ToInt32(tempTypeValue);
            bool exist = EPCEntites.Set<S_P_ContractInfo_Fb_BOQPayCheck_DetailTemplate>().Any(a => a.PayCheckDetailType == templateType && a.ID != id);
            return Json(!exist);
        }

        // 把阿拉伯数字的金额转换为中文大写数字
        private  string ConvertToChinese(decimal x)
        {
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", delegate(Match m) { return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString(); });
        }
    }
}
