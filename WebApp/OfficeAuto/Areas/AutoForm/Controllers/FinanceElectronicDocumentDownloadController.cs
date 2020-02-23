using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Formula.Helper;
using Workflow.Logic.Domain;
using Formula.Exceptions;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class FinanceElectronicDocumentDownloadController : OfficeAutoFormContorllor<T_FDM_FinanceElectronicDocumentDownload>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (isNew == true)
            {
                var FEDIDs = GetQueryString("FEDIDs");
                string sql = @"select ID as FinanceElectronicDocumentInfoID,SerialNumber,Title,FileType,FileSecondType,FileThirdType,IsSpecial,BriefDescription,RelatedEnclosure from T_FDM_FinanceElectronicDocumentUpload where ID in ('{0}')";
                sql = string.Format(sql, FEDIDs.Replace(",", "','"));
                var FEDdt = this.SQLDB.ExecuteDataTable(sql);
                dt.Rows[0]["Detail"] = JsonHelper.ToJson(FEDdt);
                dt.AcceptChanges();
            }
        }

        public override bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            if (routing.Code == "End")
            {
                var formInstanceID = taskExec.S_WF_InsFlow.FormInstanceID;
                var financeElectronicDocumentDownloadEntity = this.BusinessEntities.Set<T_FDM_FinanceElectronicDocumentDownload>().Find(formInstanceID);
                if (financeElectronicDocumentDownloadEntity == null)
                    throw new BusinessException("获取当前下载申请单失败，请联系管理员！");
                var deitals = financeElectronicDocumentDownloadEntity.T_FDM_FinanceElectronicDocumentDownload_Detail.ToList();
                if (deitals != null && deitals.Count > 0)
                {
                    var fedIds = deitals.Select(c => c.FinanceElectronicDocumentInfoID).ToArray();
                    var financeElectronicDocumentEntiys = this.BusinessEntities.Set<T_FDM_FinanceElectronicDocumentUpload>()
                        .Where(c => fedIds.Contains(c.ID)).ToList();
                    financeElectronicDocumentEntiys.Update(c => c.DownLoadCount = c.DownLoadCount + 1);
                    this.BusinessEntities.SaveChanges();
                }

            }
            return base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
        }

    }
}
