using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config.Logic;
using EPC.Logic.Domain;
using Formula.Helper;
using System.Data;

namespace EPC.Areas.Procurement.Controllers
{
    public class ComparisonResultController : EPCFormContorllor<T_P_ComparisonResult>
    {
        public JsonResult GetPBom(string PackageIDs)
        {

            var sql = @"select * from S_I_WBS_Version_Task
where VersionID in (
select Max(VersionID) from S_I_WBS_Version_Task
left join S_I_WBS_Version on S_I_WBS_Version_Task.VersionID=S_I_WBS_Version.ID
 where TaskID in( '{0}')
 and S_I_WBS_Version.FlowPhase='End')
 and TaskID in ('{0}')";

            var bomList = new List<Dictionary<string, object>>();
            var fileList = new List<Dictionary<string, object>>();
            var dt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, PackageIDs.Replace(",", "','")));
            foreach (DataRow row in dt.Rows)
            {
                var deviceInfo = row["DeviceInfo"] == null || row["DeviceInfo"] == DBNull.Value ? "" : row["DeviceInfo"].ToString();
                if (String.IsNullOrEmpty(deviceInfo))
                {
                    continue;
                }
                var list = JsonHelper.ToList(deviceInfo);
                foreach (var item in list)
                {
                    var bom = this.EPCEntites.Set<S_P_Bom>().Find(item.GetValue("ID"));
                    if (bom == null) continue;
                    var bomDic = Formula.FormulaHelper.ModelToDic<S_P_Bom>(bom);
                    bomDic.SetValue("ID", "");
                    bomDic.SetValue("PackageName", row["Name"]);
                    bomDic.SetValue("PackageCode", row["Code"]);
                    bomDic.SetValue("PBomName", bom.Name);
                    bomDic.SetValue("PBomCode", bom.Code);
                    bomDic.SetValue("PBomID", bom.ID);
                    var quantity = bom.Quantity.HasValue ? bom.Quantity.Value : 0m;
                    var invitationQuantity = bom.InvitationQuantity.HasValue ? bom.InvitationQuantity.Value : 0m;
                    bomDic.SetValue("InvitationQuantity", quantity - invitationQuantity);
                    bomDic.SetValue("CanQuantity", quantity - invitationQuantity);
                    bomDic.Remove("S_I_Engineering");
                    bomList.Add(bomDic);
                    if (!String.IsNullOrEmpty(bom.Attachment))
                    {
                        var fileDic = new Dictionary<string, object>();
                        fileDic.SetValue("Files", bom.Attachment);
                        fileDic.SetValue("FilesName", bom.AttachmentName);
                        fileList.Add(fileDic);
                    }
                }
            }
            var result = new Dictionary<string, object>();
            result.SetValue("DeviceList", bomList);
            result.SetValue("TechFiles", fileList);
            return Json(result);
        }

        public JsonResult GetPBomFromApplyInfo(string ApplyID)
        {
            string sql = @"select S_P_ProcurementApply_PBom.ID as ApplyDetailID,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ApplyQuantity,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(InvitationApply.InvitationQuantity,0)-isnull(ComparisonQuantity,0) as CanQuantity,
isnull(S_I_CBS_Budget.UnitPrice,0) as UnitPrice
from S_P_ProcurementApply_PBom
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as InvitationQuantity
from dbo.T_P_InvitationApply_DeviceList
group by ApplyDetailID) InvitationApply
on InvitationApply.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as ComparisonQuantity
from T_P_ComparisonResult_DeviceInfo
group by ApplyDetailID) ComparisonInfo 
on ComparisonInfo.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join S_I_CBS_Budget on S_P_Bom.ID=S_I_CBS_Budget.BomID
where S_P_ProcurementApply_PBom.S_P_ProcurementApplyID in ('" + ApplyID.Replace(",", "','") + "')";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            var bomList = new List<Dictionary<string, object>>();
            var fileList = new List<Dictionary<string, object>>();
            var result = new Dictionary<string, object>();
            result.SetValue("DeviceList", bomList);
            result.SetValue("TechFiles", fileList);
            foreach (DataRow row in dt.Rows)
            {
                var bomDic = Formula.FormulaHelper.DataRowToDic(row);
                bomDic.SetValue("ID", "");
                bomDic.SetValue("PackageName", "");
                bomDic.SetValue("PackageCode", "");
                bomDic.SetValue("PBomName", row["Name"]);
                bomDic.SetValue("PBomCode", row["Code"]);
                bomDic.SetValue("PBomID", row["ID"].ToString());
                bomDic.SetValue("ApplyDetailID", row["ApplyDetailID"].ToString());
                bomDic.SetValue("InvitationQuantity", row["CanQuantity"]);
                var price = (row["UnitPrice"] == null || row["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(row["UnitPrice"])) * Convert.ToDecimal(row["CanQuantity"]);
                bomList.Add(bomDic);
                if (row["Attachment"] != null && row["Attachment"] != DBNull.Value && !String.IsNullOrEmpty(row["Attachment"].ToString()))
                {
                    var fileDic = new Dictionary<string, object>();
                    fileDic.SetValue("Files", row["Attachment"].ToString());
                    fileDic.SetValue("FilesName", row["AttachmentName"].ToString());
                    fileList.Add(fileDic);
                }
            }
            return Json(result);
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "DeviceInfo")
            {
                var detailJson = JsonHelper.ToJson(detail);
                detail.SetValue("Data", detailJson);
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);
            #region 判定采购申请的数量
            //采购申请
            if (subTableName == "DeviceInfo")
            {
                var applyDetailIDList = detailList.Select(c => c.GetValue("ApplyDetailID")).Distinct().ToList();
                foreach (var applyDetailID in applyDetailIDList)
                {
                    var applyDetail = EPCEntites.Set<S_P_ProcurementApply_PBom>().FirstOrDefault(c => c.ID == applyDetailID);
                    if (applyDetail == null)
                        continue;

                    var obj = this.EPCSQLDB.ExecuteScalar(String.Format("select Sum(InvitationQuantity) from T_P_ComparisonResult_DeviceInfo where ApplyDetailID='{0}' and T_P_ComparisonResultID!='{1}'",
                        applyDetailID, dic.GetValue("ID")));
                    var otherInvitationQuantity = 0m;
                    if (obj != null && obj != DBNull.Value)
                    {
                        otherInvitationQuantity = Convert.ToDecimal(obj);
                    }

                    var quantity = 0m;
                    var list = detailList.Where(c => c.GetValue("ApplyDetailID") == applyDetailID).ToList();
                    foreach (var applyDetailInfo in list)
                    {
                        var invitationQuantity = 0m;
                        if (!String.IsNullOrEmpty(applyDetailInfo.GetValue("InvitationQuantity")))
                            invitationQuantity = Convert.ToDecimal(applyDetailInfo.GetValue("InvitationQuantity"));
                        quantity += invitationQuantity;
                    }
                    if (applyDetail.Quantity < (otherInvitationQuantity + quantity))//TODO 少扣了 合同量及招标申请量
                    {
                        throw new Exception("设备材料【" + applyDetail.Name + "】合计不能大于采购申请数量【" + applyDetail.Quantity + "】");
                    }
                }
            }            

            #endregion
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var entity = this.GetEntityByID(dic.GetValue("ID"));
                if (entity != null)
                {
                    var invitation = entity.ToInvitation();
                    this.EPCEntites.SaveChanges();
                    string sql = @"update S_P_Bom
set InvitationQuantity = (select Sum(InvitationQuantity) from S_P_Invitation_Device
where PBomID = S_P_Bom.ID)
where ID in ('{0}') ";
                    var bomIDs = String.Join(",", invitation.S_P_Invitation_Device.Select(c => c.PBomID).ToList());
                    this.EPCSQLDB.ExecuteNonQuery(String.Format(sql, bomIDs.Replace(",", "','")));
                }
            }
        }

        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                var invitation = this.GetEntityByID<S_P_Invitation>(Id);
                if (invitation == null) continue;
                if (this.EPCEntites.Set<S_P_ContractInfo>().Count(c => c.Invitation.Contains(invitation.ID)) > 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("已经关联了合同的招标信息，无法删除");
                }
            }
        }

        protected override void OnFlowEnd(T_P_ComparisonResult entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
