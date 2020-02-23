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

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementSmartApplyController : EPCFormContorllor<S_P_ProcurementSmartApply>
    {
        protected override void OnFlowEnd(S_P_ProcurementSmartApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult GetProcurementSmartApplyDetail(string applyID)
        {
            string sql = @"select S_P_ProcurementSmartApply_PBom.*
                         ,isnull(S_P_Bom.Quantity,0)-isnull(ContractQuantity,0) as RemainContractQuantity
                          from S_P_Bom inner join S_P_ProcurementSmartApply_PBom on S_P_Bom.ID = S_P_ProcurementSmartApply_PBom.PBomID 
                          where S_P_ProcurementSmartApply_PBom.S_P_ProcurementSmartApplyID = '" + applyID + "'";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Engineering).ExecuteDataTable(sql);           
            return Json(dt);
        }
    }
}
