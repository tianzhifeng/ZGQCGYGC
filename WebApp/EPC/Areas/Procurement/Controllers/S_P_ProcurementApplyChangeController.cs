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

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementApplyChangeController : EPCFormContorllor<S_P_ProcurementApplyChange>
    {
        protected override void OnFlowEnd(S_P_ProcurementApplyChange entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            //撤销
            if (entity.Operation == "Cancel")
            {
                var bomIds = entity.S_P_ProcurementApplyChange_PBom.GroupBy(a => a.BomID).Select(a => a.Key).ToList();
                foreach (var bomId in bomIds)
                {
                    var pbom = this.EPCEntites.Set<S_P_Bom>().FirstOrDefault(c => c.ID == bomId);
                    pbom.ApplyQuantity = this.EPCEntites.Set<S_P_ProcurementApply_PBom>().Where(a => a.BomID == pbom.ID && a.S_P_ProcurementApplyID != entity.S_P_ProcurementApplyID).ToList().Sum(a => a.Quantity);
                }
                EPCEntites.Set<S_P_ProcurementApply>().Delete(a => a.ID == entity.S_P_ProcurementApplyID);
            }
            else if (entity.Operation == "Change")
            {
                var apply = EPCEntites.Set<S_P_ProcurementApply>().Find(entity.S_P_ProcurementApplyID);
                //如果第一次进行变更
                if (string.IsNullOrEmpty(entity.LastVersionId))
                {
                    S_P_ProcurementApplyChange firstChange = new S_P_ProcurementApplyChange();
                    this.UpdateEntity(firstChange, apply.ToDic());
                    firstChange.ID = FormulaHelper.CreateGuid();
                    firstChange.S_P_ProcurementApplyID = apply.ID;
                    firstChange.FlowPhase = "End";
                    firstChange.Operation = "Initial";//原数据
                    foreach(var tmp in apply.S_P_ProcurementApply_PBom)
                    {
                        S_P_ProcurementApplyChange_PBom pbom = new S_P_ProcurementApplyChange_PBom();
                        pbom.ID = FormulaHelper.CreateGuid();
                        this.UpdateEntity(pbom, tmp.ToDic());
                        pbom.S_P_ProcurementApplyChangeID = firstChange.ID;
                        pbom.OrlID = tmp.ID;
                        firstChange.S_P_ProcurementApplyChange_PBom.Add(pbom);
                    }

                    entity.LastVersionId = firstChange.ID;
                    EPCEntites.Set<S_P_ProcurementApplyChange>().Add(firstChange);
                }

                var operaApplyBoms = entity.S_P_ProcurementApplyChange_PBom.ToList();
                var newQuantityApplyBoms = new List<S_P_ProcurementApply_PBom>();
                foreach (var op in operaApplyBoms)
                {
                    var old = EPCEntites.Set<S_P_ProcurementApply_PBom>().SingleOrDefault(a => a.ID == op.OrlID);                    
                    old.Quantity = op.Quantity;//更新数量
                    newQuantityApplyBoms.Add(old);
                }

                //更新bom
                var dictAppBoms = newQuantityApplyBoms.GroupBy(a => a.BomID).Select(a => new { BomId = a.Key, SumQuantity = a.Sum(b => b.Quantity) }).ToList();
                foreach (var appBom in dictAppBoms)
                {
                    var pbom = this.EPCEntites.Set<S_P_Bom>().FirstOrDefault(c => c.ID == appBom.BomId);
                    pbom.ApplyQuantity = this.EPCEntites.Set<S_P_ProcurementApply_PBom>().Where(a => a.BomID == pbom.ID && a.S_P_ProcurementApplyID != entity.S_P_ProcurementApplyID).ToList().Sum(a => a.Quantity);
                    pbom.ApplyQuantity += appBom.SumQuantity;
                }
            }
            this.EPCEntites.SaveChanges();
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var apply = EPCEntites.Set<S_P_ProcurementApply>().Find(dic.GetValue("S_P_ProcurementApplyID"));
            if (apply != null)
            {
                var operation = dic.GetValue("Operation");
                if (operation == "Cancel")
                {
                    if (apply.FlowPhase != "End")
                    {
                        throw new BusinessException("该采购申请流程还未结束，不能被撤销");
                    }

                    var content = EPCEntites.Set<S_P_ContractInfo_Content>().FirstOrDefault(a => a.ApplyFormID == apply.ID);
                    if (content != null)
                    {
                        throw new BusinessException("该采购申请的设备已为采购合同【" + content.S_P_ContractInfo.SerialNumber + "】的采购设备，不能被撤销");
                    }
                }
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);

            if (dic.GetValue("Operation") == "Change" && subTableName == "PBom")
            {
                var bomIdDicList = detailList.GroupBy(a => a.GetValue("BomID")).Select(a => a.Key);
                foreach (var bomId in bomIdDicList)
                {
                    var quantity = 0.0m;
                    foreach (var detail in detailList.Where(a => a.GetValue("BomID") == bomId))
                    {
                        var tmp = 0.0m;
                        decimal.TryParse(detail.GetValue("Quantity"), out tmp);
                        quantity += tmp;
                    }
                    var bom = EPCEntites.Set<S_P_Bom>().Find(bomId);
                    var applyID = dic.GetValue("S_P_ProcurementApplyID");
                    var otherQuantity = EPCEntites.Set<S_P_ProcurementApply_PBom>().Where(a => a.BomID == bomId && a.S_P_ProcurementApplyID != applyID).ToList().Sum(a => a.Quantity ?? 0);
                    var maxQuantity = bom.Quantity - otherQuantity;
                    if (quantity > maxQuantity)
                    {
                        throw new BusinessException("设备【" + bom.Name + "】的数量不能超过" + maxQuantity);
                    }
                }

                foreach (var detail in detailList)
                {
                    var detailId = detail.GetValue("ID");
                    var name = detail.GetValue("Name");
                    var quantity = 0.0m;
                    decimal.TryParse(detail.GetValue("Quantity"), out quantity);
                    var minQuantity = 0.0m;
                    //minQuantity += EPCEntites.Set<T_P_InvitationApply_DeviceList>().Where(a => a.ApplyDetailID == detailId).ToList().Sum(a => a.InvitationQuantity ?? 0);
                    //minQuantity += EPCEntites.Set<T_P_ComparisonResult_DeviceInfo>().Where(a => a.ApplyDetailID == detailId).ToList().Sum(a => a.InvitationQuantity ?? 0);
                    //minQuantity += EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.ApplyDetailID == detailId).ToList().Sum(a => a.ContractQuantity ?? 0);
                    //if (quantity < minQuantity)
                    //{
                    //    throw new BusinessException("明细【" + name + "】的数量不能少于已用于招标、比价、订单的数量之和" + minQuantity);
                   // }

                    minQuantity += EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.ApplyBomID == detailId).ToList().Sum(a => a.ContractQuantity ?? 0);
                    if (quantity < minQuantity)
                    {
                        throw new BusinessException("明细【" + name + "】的数量不能少于该申请已签订数量" + minQuantity);
                    }
                }
            }
        }
    }
}
