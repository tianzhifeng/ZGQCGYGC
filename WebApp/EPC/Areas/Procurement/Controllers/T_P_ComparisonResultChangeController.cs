﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config.Logic;
using EPC.Logic.Domain;
using Formula.Helper;
using System.Data;
using Formula.Exceptions;
using Formula;

namespace EPC.Areas.Procurement.Controllers
{
    public class ComparisonResultChangeController : EPCFormContorllor<T_P_ComparisonResultChange>
    {
        protected override void OnFlowEnd(T_P_ComparisonResultChange entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            //撤销
            if (entity.Operation == "Cancel")
            {
                EPCEntites.Set<T_P_ComparisonResult>().Delete(a => a.ID == entity.T_P_ComparisonResultID);
                var bomIds = EPCEntites.Set<S_P_Invitation_Device>().Where(a => a.S_P_InvitationID == entity.T_P_ComparisonResultID).Select(c => c.PBomID).ToList();
                EPCEntites.Set<S_P_Invitation>().Delete(a => a.ID == entity.T_P_ComparisonResultID);
                EPCEntites.SaveChanges();
                string sql = @"update S_P_Bom
set InvitationQuantity = (select Sum(InvitationQuantity) from S_P_Invitation_Device
where PBomID = S_P_Bom.ID)
where ID in ('{0}') ";
                var bomIDs = String.Join(",", bomIds);
                this.EPCSQLDB.ExecuteNonQuery(String.Format(sql, bomIDs.Replace(",", "','")));
            }
            else if (entity.Operation == "Change")
            {
                var apply = EPCEntites.Set<T_P_ComparisonResult>().Find(entity.T_P_ComparisonResultID);

                //如果第一次进行变更
                if (string.IsNullOrEmpty(entity.LastVersionId))
                {
                    T_P_ComparisonResultChange firstChange = new T_P_ComparisonResultChange();
                    this.UpdateEntity(firstChange, apply.ToDic());
                    firstChange.ID = FormulaHelper.CreateGuid();
                    firstChange.T_P_ComparisonResultID = apply.ID;
                    firstChange.FlowPhase = "End";
                    firstChange.Operation = "Initial";//原数据
                    foreach (var tmp in apply.T_P_ComparisonResult_DeviceInfo)
                    {
                        T_P_ComparisonResultChange_DeviceInfo pbom = new T_P_ComparisonResultChange_DeviceInfo();
                        pbom.ID = FormulaHelper.CreateGuid();
                        this.UpdateEntity(pbom, tmp.ToDic());
                        pbom.T_P_ComparisonResultChangeID = firstChange.ID;
                        pbom.OrlID = tmp.ID;
                        firstChange.T_P_ComparisonResultChange_DeviceInfo.Add(pbom);
                    }

                    foreach (var tmp in apply.T_P_ComparisonResult_RequireInfo)
                    {
                        T_P_ComparisonResultChange_RequireInfo eng = new T_P_ComparisonResultChange_RequireInfo();
                        eng.ID = FormulaHelper.CreateGuid();
                        this.UpdateEntity(eng, tmp.ToDic());
                        eng.T_P_ComparisonResultChangeID = firstChange.ID;
                        //pbom.OrlID = tmp.ID;
                        firstChange.T_P_ComparisonResultChange_RequireInfo.Add(eng);
                    }

                    foreach (var tmp in apply.T_P_ComparisonResult_SupplierInfo)
                    {
                        T_P_ComparisonResultChange_SupplierInfo eng = new T_P_ComparisonResultChange_SupplierInfo();
                        eng.ID = FormulaHelper.CreateGuid();
                        this.UpdateEntity(eng, tmp.ToDic());
                        eng.T_P_ComparisonResultChangeID = firstChange.ID;
                        //pbom.OrlID = tmp.ID;
                        firstChange.T_P_ComparisonResultChange_SupplierInfo.Add(eng);
                    }
                    
                    entity.LastVersionId = firstChange.ID;
                    EPCEntites.Set<T_P_ComparisonResultChange>().Add(firstChange);
                }

                var operaApplyBoms = entity.T_P_ComparisonResultChange_DeviceInfo.ToList();
                var newQuantityApplyBomIds = new List<string>();
                foreach (var op in operaApplyBoms)
                {
                    var old = EPCEntites.Set<T_P_ComparisonResult_DeviceInfo>().SingleOrDefault(a => a.ID == op.OrlID);
                    //值交换，变更记录保存旧数据，旧数据更新新数据
                    var tmp = op.InvitationQuantity;
                    op.InvitationQuantity = old.InvitationQuantity;//变更记录保存旧数量
                    old.InvitationQuantity = tmp;//更新数量

                    var oldSDevice = EPCEntites.Set<S_P_Invitation_Device>().SingleOrDefault(a => a.ID == op.OrlID);
                    if (oldSDevice != null)
                    {
                        oldSDevice.InvitationQuantity = tmp;
                    }

                    newQuantityApplyBomIds.Add(old.PBomID);
                }
                EPCEntites.SaveChanges();

                string sql = @"update S_P_Bom
set InvitationQuantity = (select Sum(InvitationQuantity) from S_P_Invitation_Device
where PBomID = S_P_Bom.ID)
where ID in ('{0}') ";
                var bomIDs = String.Join(",", newQuantityApplyBomIds.ToList());
                this.EPCSQLDB.ExecuteNonQuery(String.Format(sql, bomIDs.Replace(",", "','")));
            }            
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            var resultID = dic.GetValue("T_P_ComparisonResultID");
            var apply = EPCEntites.Set<T_P_ComparisonResult>().Find(resultID);
            if (apply != null)
            {
                var operation = dic.GetValue("Operation");
                if (operation == "Cancel")
                {
                    if (apply.FlowPhase != "End")
                    {
                        throw new BusinessException("该比价审批流程还未结束，不能被撤销");
                    }

                    var content = EPCEntites.Set<S_P_ContractInfo_Content>().FirstOrDefault(a => a.ApplyFormID == apply.ID);
                    if (content != null)
                    {
                        throw new BusinessException("该比价审批的设备已为采购合同【" + content.S_P_ContractInfo.SerialNumber + "】的采购设备，不能被撤销");
                    }
                }
            }

            if (isNew)
            {
                //id设置为空，不然记录无法保存
                var deviceInfo = dic.GetValue("DeviceInfo");
                var deviceInfoDic = JsonHelper.ToList(deviceInfo);
                foreach (var tmp in deviceInfoDic)
                {
                    tmp.SetValue("ID", "");
                }
                dic.SetValue("DeviceInfo", JsonHelper.ToJson(deviceInfoDic));
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);
            if (dic.GetValue("Operation") == "Change" && subTableName == "DeviceInfo")
            {
                foreach (var detail in detailList)
                {
                    var applyID = dic.GetValue("T_P_ComparisonResultID");
                    var bomId = detail.GetValue("PBomID");
                    var applyDetailID = detail.GetValue("ApplyDetailID");
                    var orlID = detail.GetValue("OrlID");
                    var quantity = 0.0m;
                    decimal.TryParse(detail.GetValue("InvitationQuantity"), out quantity);

                    var otherQuantity = EPCEntites.Set<T_P_ComparisonResult_DeviceInfo>().Where(a => a.PBomID == bomId && a.T_P_ComparisonResultID != applyID).ToList().Sum(a => a.InvitationQuantity ?? 0);
                    var procurementApplyBomQuantity = EPCEntites.Set<S_P_ProcurementApply_PBom>().Where(a => a.ID == applyDetailID).Sum(a => a.Quantity);

                    var procurementApplyBomContractQuantity = EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.ApplyDetailID == applyDetailID).Sum(a => a.ContractQuantity);
                    var procurementApplyBomResultQuantity = EPCEntites.Set<T_P_InvitationApply_DeviceList>().Where(a => a.ApplyDetailID == applyDetailID).Sum(a => a.InvitationQuantity);

                    var maxQuantity = procurementApplyBomQuantity - procurementApplyBomContractQuantity - procurementApplyBomResultQuantity - otherQuantity;
                    var minQuantity = EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.ApplyDetailID == orlID).Sum(a => a.ContractQuantity);

                    if (quantity > maxQuantity)
                    {
                        throw new BusinessException("【" + detail.GetValue("PBomName") + "】的数量不能超过" + maxQuantity);
                    }
                    if (quantity < minQuantity)
                    {
                        throw new BusinessException("【" + detail.GetValue("PBomName") + "】的数量不能少于采购合同设备数量" + minQuantity);
                    }
                }
            }
        }
    }
}
