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
    public class ProcurementApplyController : EPCFormContorllor<S_P_ProcurementApply>
    {
        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);
            if (subTableName == "PBom")
            {
                var dicList = detailList.GroupBy(a => new { ID = a.GetValue("BomID"), Name = a.GetValue("Name") }).Select(a => a.Key);
                foreach (var detailDic in dicList)
                {
                    var sameIDList = detailList.Where(a => a.GetValue("BomID") == detailDic.ID);
                    decimal total = 0;
                    foreach (var sameIDItem in sameIDList)
                    {
                        decimal tmp = 0;
                        decimal.TryParse(sameIDItem.GetValue("Quantity"), out tmp);
                        total += tmp;
                    }
                    string sql = @"select isnull(S_P_Bom.Quantity,0)-isnull(InvitationQuantity,0)-isnull(procurementApply.ApplyQuantity,0) as RemainInvitationQuantity from S_P_Bom left join  (select sum(Quantity) ApplyQuantity,EngineeringInfoID,S_P_ProcurementApply_PBom.BomID from S_P_ProcurementApply inner join S_P_ProcurementApply_PBom on S_P_ProcurementApply.ID = S_P_ProcurementApply_PBom.S_P_ProcurementApplyID
                                   where S_P_ProcurementApply.ID <>  '" + dic.GetValue("ID") + @"' group by EngineeringInfoID,BomID) procurementApply on procurementApply.BomID = S_P_Bom.ID
                                   where id = '" + detailDic.ID + "'";

                    object val = this.EPCSQLDB.ExecuteScalar(sql);
                    if (val != null)
                    {
                        decimal storeTotal = 0;
                        decimal.TryParse(val.ToString(), out storeTotal);
                        if (total > storeTotal)
                        {
                            throw new BusinessException("设备【" + detailDic.Name + "】数量" + total + "超出了可招标数量" + storeTotal);
                        }
                    }
                }
            }
        }

        protected override void OnFlowEnd(S_P_ProcurementApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult GetProcurementApplyDetail(string applyID, string contractID)
        {
            string sql = @"
select S_P_ProcurementApply_PBom.ID as ApplyDetailID,
S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ApplyQuantity,
isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(ContentInfo.ContractQuantity,0) as CanContractQuantity
 from S_P_ProcurementApply_PBom
left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
left join (select ApplyBomID,isnull(Sum(ContractQuantity),0) as ContractQuantity
from dbo.S_P_ContractInfo_Content where S_P_ContractInfoID!='" + contractID + @"'
group by ApplyBomID) ContentInfo
on ContentInfo.ApplyBomID=S_P_ProcurementApply_PBom.ID
where S_P_ProcurementApply_PBom.S_P_ProcurementApplyID in ('" + applyID.Replace(",", "','") + "')";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }
    }
}
