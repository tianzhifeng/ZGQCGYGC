using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Exceptions;
using Formula.Helper;

namespace EPC.Areas.Construction.Controllers
{
    public class FbContractChangeController : EPCFormContorllor<S_P_FbContractChange>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dic, isNew, upperVersionID);
            string ContractID = dic.GetValue("ContractID");
            if (isNew)
            {
                ContractID = GetQueryString("ContractID");
            }

            var lastVersionID = dic.GetValue("LastVersionID");
            if (string.IsNullOrEmpty(lastVersionID))
            {
                var contract = EPCEntites.Set<S_P_ContractInfo>().Find(ContractID);
                dic.SetValue("LastVersionData", JsonHelper.ToJson(contract));
            }
            else
            {
                string LastVersionID = dic.GetValue("LastVersionID");
                var change = EPCEntites.Set<S_P_FbContractChange>().Find(LastVersionID);
                if (change != null)
                {
                    S_P_ContractInfo contract = new S_P_ContractInfo();
                    FormulaHelper.UpdateEntity(contract, change.ToDic());//只带合同信息,去掉审批字段数据
                    dic.SetValue("LastVersionData", JsonHelper.ToJson(contract));
                }
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            decimal contractAmount = 0;
            decimal.TryParse(dic.GetValue("ContractAmount"), out contractAmount);
            string contractInfoID = dic.GetValue("ContractID");
            var lastVersion = EPCEntites.Set<S_C_BOQ_Version>().Where(a => a.ContractInfoID == contractInfoID).OrderByDescending(a => a.ID).FirstOrDefault();
            if (lastVersion != null)
            {
                decimal boqTotalPrice = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == lastVersion.ID).ToList().Sum(a => a.Price ?? 0);

                if (contractAmount < boqTotalPrice)
                {
                    throw new Formula.Exceptions.BusinessValidationException("合同人民币金额不能小于清单总价" + boqTotalPrice + "元");
                }
            }
        }

        protected override void OnFlowEnd(S_P_FbContractChange entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            var contract = EPCEntites.Set<S_P_ContractInfo>().Find(entity.ContractID);

            //如果首次变更，必须将变更前的合同信息插入到变更表中。
            if (string.IsNullOrEmpty(entity.LastVersionID))
            {
                S_P_FbContractChange firstChange = new S_P_FbContractChange();
                this.UpdateEntity(firstChange, contract.ToDic());
                firstChange.ID = FormulaHelper.CreateGuid();
                firstChange.ContractID = contract.ID;
                firstChange.FlowPhase = "End";
                firstChange.FirstOne = "true";                

                entity.LastVersionID = firstChange.ID;
                EPCEntites.Set<S_P_FbContractChange>().Add(firstChange);
            }

            //开始更新合同信息
            this.UpdateEntity(contract, entity.ToDic());
            this.EPCEntites.SaveChanges();
        }
    }
}
