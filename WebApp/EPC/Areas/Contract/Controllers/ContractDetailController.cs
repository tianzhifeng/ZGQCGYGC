using Base.Logic.Domain;
using EPC.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Contract.Controllers
{
    public class ContractDetailController : EPCFormContorllor<S_M_ContractInfo_Supplementary>
    {
        //
        // GET: /Contract/ContractDetail/
        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (dic.ContainsKey("S_M_ContractInfo"))
            {
                string S_M_ContractInfoID = dic["S_M_ContractInfo"];
                if (!string.IsNullOrEmpty(S_M_ContractInfoID))
                {
                    S_M_ContractInfo contractinfo = this.EPCEntites.Set<S_M_ContractInfo>().Find(S_M_ContractInfoID);
                    List<S_M_ContractInfo_Supplementary> ContractDetails = this.EPCEntites.Set<S_M_ContractInfo_Supplementary>().Where(c => c.S_M_ContractInfo == S_M_ContractInfoID).ToList();
                    if (contractinfo != null)
                    {
                        contractinfo.ContractMoney = ContractDetails.Sum(c => c.ContractMoney);
                        contractinfo.ContractReceivableMoney = ContractDetails.Sum(c => c.ContractReceivableMoney);
                        contractinfo.ForeignCurrencyContractMoney = ContractDetails.Sum(c => c.ForeignCurrencyContractMoney);
                        contractinfo.DesignContractMoney = ContractDetails.Sum(c => c.DesignContractMoney);
                        contractinfo.CBMoney = ContractDetails.Sum(c => c.CBMoney);
                        contractinfo.ProjectManageContractMoney = ContractDetails.Sum(c => c.ProjectManageContractMoney);
                        contractinfo.ReceiptLimitMoney = ContractDetails.Sum(c => c.ReceiptLimitMoney);
                        contractinfo.HTBHSJY = ContractDetails.Sum(c => c.HTBHSJY);
                        contractinfo.HTXXSEY = ContractDetails.Sum(c => c.HTXXSEY);
                        //ForeignCurrencyType ContractDifferReason 
                        //HaveSubcontract  FBHTJEY 
                        var gcd = ContractDetails.GroupBy(c => c.ForeignCurrencyType);
                        if (gcd.Count() > 1) { throw new Exception("合同明细存在不同币种请修改！"); }
                        contractinfo.ForeignCurrencyType = gcd.FirstOrDefault().Key;
                        contractinfo.ContractDifferReason = String.Join("/n/r", ContractDetails.Select(c => c.ContractDifferReason).ToArray());
                    }
                }
            }
            if (dic.ContainsKey("T_M_ContractInfoID"))
            {
                string T_M_ContractInfoID = dic["T_M_ContractInfoID"];
                if (!string.IsNullOrEmpty(T_M_ContractInfoID))
                {
                    T_M_ContractInfo contractinfo = this.EPCEntites.Set<T_M_ContractInfo>().Find(T_M_ContractInfoID);
                    List<S_M_ContractInfo_Supplementary> ContractDetails = this.EPCEntites.Set<S_M_ContractInfo_Supplementary>().Where(c => c.T_M_ContractInfo == T_M_ContractInfoID).ToList();
                    if (contractinfo != null)
                    {
                        contractinfo.ContractMoney = ContractDetails.Sum(c => c.ContractMoney);
                        contractinfo.ContractReceivableMoney = ContractDetails.Sum(c => c.ContractReceivableMoney);
                        contractinfo.ForeignCurrencyContractMoney = ContractDetails.Sum(c => c.ForeignCurrencyContractMoney);
                        contractinfo.DesignContractMoney = ContractDetails.Sum(c => c.DesignContractMoney);
                        contractinfo.CBMoney = ContractDetails.Sum(c => c.CBMoney);
                        contractinfo.ProjectManageContractMoney = ContractDetails.Sum(c => c.ProjectManageContractMoney);
                        contractinfo.ReceiptLimitMoney = ContractDetails.Sum(c => c.ReceiptLimitMoney);
                        contractinfo.HTBHSJY = ContractDetails.Sum(c => c.HTBHSJY);
                        contractinfo.HTXXSEY = ContractDetails.Sum(c => c.HTXXSEY);
                        //ForeignCurrencyType ContractDifferReason 
                        //HaveSubcontract  FBHTJEY 
                        var gcd = ContractDetails.GroupBy(c => c.ForeignCurrencyType);
                        if (gcd.Count() > 1) { throw new Exception("合同明细存在不同币种请修改！"); }
                        contractinfo.ForeignCurrencyType = gcd.FirstOrDefault().Key;
                        contractinfo.ContractDifferReason = String.Join("/n/r", ContractDetails.Select(c => c.ContractDifferReason).ToArray());
                    }
                }
            }

        }
    }
}
