using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Config;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class SealTransferController : OfficeAutoFormContorllor<T_Seal_Transfer>
    {
        protected override void OnFlowEnd(T_Seal_Transfer entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format(@"update T_Seal_SealInfo 
    set DeptID=( select DeptID from {0}.dbo.S_A_User where ID='{1}'),
        DeptIDName=(select DeptName from {0}.dbo.S_A_User where ID='{1}'),
        KeeperID='{1}',
        KeeperIDName='{2}' 
where ID='{3}';", baseHelper.DbName, entity.ReceivePersonID, entity.ReceivePersonIDName, entity.SealInfoID);
            
            this.SQLDB.ExecuteNonQuery(sql);

            base.OnFlowEnd(entity, taskExec, routing);
        }
    }
}
