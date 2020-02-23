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
using MvcAdapter;
using System.Data;

namespace EPC.Controllers
{
    public class ProcurementApplySelectorController : EPCController
    {
        public JsonResult GetTreeList(string EngineeringInfoID, string ContractID, string InvitationID)
        {
            string sql = @"select * from (select S_P_ProcurementApply.*,SelectOrder,DeviceNames from S_P_ProcurementApply
left join (select DeviceNames=STUFF((SELECT ','+Name FROM S_P_ProcurementApply_PBom A 
WHERE A.S_P_ProcurementApplyID=S_P_ProcurementApply_PBom.S_P_ProcurementApplyID FOR XML PATH('')), 1, 1, '')
,S_P_ProcurementApplyID from dbo.S_P_ProcurementApply_PBom
group by S_P_ProcurementApplyID) BomDetail
on BomDetail.S_P_ProcurementApplyID=S_P_ProcurementApply.ID
left join S_P_ContractInfo on S_P_ProcurementApply.ID=S_P_ContractInfo.SelectOrder) TableInfo where  FlowPhase='End' and EngineeringInfoID='{0}'";
            var mainDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID));

            sql = @"select S_P_ProcurementApply_PBom.ID as ApplyDetailID,S_P_ProcurementApplyID,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ApplyQuantity,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(ContentInfo.ContractQuantity,0) as CanContractQuantity
 from S_P_ProcurementApply_PBom
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyBomID,isnull(Sum(ContractQuantity),0) as ContractQuantity
from dbo.S_P_ContractInfo_Content where S_P_ContractInfoID!='" + ContractID + @"'
group by ApplyBomID) ContentInfo
on ContentInfo.ApplyBomID=S_P_ProcurementApply_PBom.ID where EngineeringInfoID='{0}'";
            var subDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID));
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in mainDt.Rows)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("ID", row["ID"].ToString());
                dic.SetValue("SerialNumber", row["SerialNumber"].ToString());
                dic.SetValue("ParentID", "");
                dic.SetValue("Name", row["DeviceNames"].ToString());
                dic.SetValue("ApplyUserName", row["ApplyUserName"].ToString());
                dic.SetValue("ApplyDate", row["ApplyDate"].ToString());
                dic.SetValue("ApplyUser", row["ApplyUser"].ToString());
                dic.SetValue("Type", "Form");
                list.Add(dic);
                var rows = subDt.Select("S_P_ProcurementApplyID='" + row["ID"].ToString() + "'");
                foreach (var subRow in rows)
                {
                    var subDic = new Dictionary<string, object>();
                    subDic.SetValue("ID", subRow["ID"].ToString());
                    subDic.SetValue("SerialNumber", "");
                    subDic.SetValue("ParentID", row["ID"].ToString());
                    subDic.SetValue("Name", subRow["Name"].ToString());
                    subDic.SetValue("Model", subRow["Model"].ToString());
                    subDic.SetValue("Quantity", subRow["Quantity"].ToString());
                    subDic.SetValue("CanContractQuantity", subRow["CanContractQuantity"].ToString());
                    subDic.SetValue("CanInvitationQuantity", subRow["Quantity"].ToString());
                    subDic.SetValue("PBomID", subRow["ID"].ToString());
                    subDic.SetValue("Type", "PBom");
                    list.Add(subDic);
                }

            }
            return Json(list);
        }

        public JsonResult GetContractApplyDetailList(QueryBuilder qb, string ProcurementType, string EngineeringInfoID, string RelateID)
        {
            string sql = @"select * from (select S_P_ProcurementApply_PBom.ID as ApplyDetailID,S_P_ProcurementApplyID,
S_P_ProcurementApply_PBom.CBSInfo,S_P_ProcurementApply_PBom.CBSInfoName,
S_P_ProcurementApply.SerialNumber,S_P_ProcurementApply.ApplyUser,S_P_ProcurementApply.ProcurementType,ApplyUserName,ApplyDate,
--S_P_ProcurementApply.Type,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ProcurementApplyQuantity,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(ContentInfo.ContractQuantity,0)-isnull(Result.ResultQuantity,0)-isnull(Apply.ApplyQuantity,0) as CanContractQuantity
 from S_P_ProcurementApply_PBom
left join S_P_ProcurementApply on S_P_ProcurementApply.ID=S_P_ProcurementApplyID
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyDetailID,isnull(Sum(ContractQuantity),0) as ContractQuantity
from dbo.S_P_ContractInfo_Content where S_P_ContractInfoID!='{0}'
group by ApplyDetailID) ContentInfo
on ContentInfo.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join ( select sum(isnull(ContractQuantity,0)) ResultQuantity,ProcurementDetailBomID from (select T_P_ComparisonResult_DeviceInfo.ApplyDetailID as ProcurementDetailBomID,S_P_ContractInfo_Content.* from T_P_ComparisonResult_DeviceInfo
left join S_P_ContractInfo_Content on S_P_ContractInfo_Content.ApplyDetailID = T_P_ComparisonResult_DeviceInfo.id) tmp group by ProcurementDetailBomID) Result
on Result.ProcurementDetailBomID = S_P_ProcurementApply_PBom.ID
left join ( select sum(isnull(ContractQuantity,0)) ApplyQuantity,ProcurementDetailBomID from (select T_P_InvitationApply_DeviceList.ApplyDetailID as ProcurementDetailBomID,S_P_ContractInfo_Content.* from T_P_InvitationApply_DeviceList
left join S_P_ContractInfo_Content on S_P_ContractInfo_Content.ApplyDetailID = T_P_InvitationApply_DeviceList.id) tmp group by ProcurementDetailBomID) Apply
on Apply.ProcurementDetailBomID = S_P_ProcurementApply_PBom.ID
) tmp where tmp.CanContractQuantity > 0 and EngineeringInfoID='{1}'";
            sql = String.Format(sql, RelateID, EngineeringInfoID);
            if (!String.IsNullOrEmpty(ProcurementType))
            {
                sql += " and ProcurementType='" + ProcurementType + "'";
            }
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetComparisonDetailList(QueryBuilder qb, string EngineeringInfoID, string RelateID)
        {
            string sql = @"select * from (select S_P_ProcurementApply_PBom.ID as ApplyDetailID, EngineeringInfoName,S_P_ProcurementApplyID,
S_P_ProcurementApply.SerialNumber,S_P_ProcurementApply.ApplyUser,ApplyUserName,ApplyDate,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ProcurementApplyQuantity,
s_i_cbs_budget.UnitPrice,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(Result.InvitationQuantity,0) -isnull(ContractContent.ContractQuantity,0) -isnull(InvoiceApply.InvitationQuantity,0) as RemainInvitationQuantity
 from S_P_ProcurementApply_PBom
left join S_P_ProcurementApply on S_P_ProcurementApply.ID=S_P_ProcurementApplyID
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyDetailID,isnull(Sum(ContractQuantity),0) as ContractQuantity
from dbo.S_P_ContractInfo_Content
group by ApplyDetailID) ContractContent
on ContractContent.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as InvitationQuantity
from dbo.T_P_InvitationApply_DeviceList
group by ApplyDetailID) InvoiceApply
on InvoiceApply.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as InvitationQuantity
from dbo.T_P_ComparisonResult_DeviceInfo where T_P_ComparisonResultID!='{0}'
group by ApplyDetailID) Result
on Result.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join s_i_cbs_budget on s_i_cbs_budget.bomid = S_P_ProcurementApply_PBom.bomid
) tmp where tmp.RemainInvitationQuantity> 0 and EngineeringInfoID='{1}'";
            sql = String.Format(sql, RelateID, EngineeringInfoID);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);

        }

        public JsonResult GetApplyDetailList(QueryBuilder qb, string EngineeringInfoID, string RelateID)
        {
            string sql = @"select * from (select S_P_ProcurementApply_PBom.ID as ApplyDetailID, EngineeringInfoName,S_P_ProcurementApplyID,
S_P_ProcurementApply.SerialNumber,S_P_ProcurementApply.ApplyUser,S_P_ProcurementApply.FlowPhase,ApplyUserName,ApplyDate,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ProcurementApplyQuantity,
s_i_cbs_budget.UnitPrice,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(InvoiceApply.InvitationQuantity,0)-isnull(ContractContent.ContractQuantity,0)-isnull(Result.InvitationQuantity,0)  as RemainInvitationQuantity
 from S_P_ProcurementApply_PBom
left join S_P_ProcurementApply on S_P_ProcurementApply.ID=S_P_ProcurementApplyID
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyDetailID,isnull(Sum(ContractQuantity),0) as ContractQuantity
from dbo.S_P_ContractInfo_Content
group by ApplyDetailID) ContractContent
on ContractContent.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as InvitationQuantity
from dbo.T_P_ComparisonResult_DeviceInfo
group by ApplyDetailID) Result
on Result.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join (select ApplyDetailID,isnull(Sum(InvitationQuantity),0) as InvitationQuantity
from dbo.T_P_InvitationApply_DeviceList  where T_P_InvitationApplyID!='{0}'
group by ApplyDetailID) InvoiceApply
on InvoiceApply.ApplyDetailID=S_P_ProcurementApply_PBom.ID
left join s_i_cbs_budget on s_i_cbs_budget.bomid = S_P_ProcurementApply_PBom.bomid
) tmp where tmp.RemainInvitationQuantity> 0 and EngineeringInfoID='{1}'";
            sql = String.Format(sql, RelateID, EngineeringInfoID);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }
    }
}
