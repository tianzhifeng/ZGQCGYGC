using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using MvcAdapter;
using Formula;
using System.Data;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class CertificateBorrowController : ComprehensiveFormController<T_C_CertificateBorrow>
    {
        protected override void OnFlowEnd(T_C_CertificateBorrow entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var borrowIDList = entity.T_C_CertificateBorrow_ApplyContent.Select(d => d.Certificate).ToList();
            var borrowIDArray = "'" + string.Join("','", borrowIDList) + "'";

            var sql = string.Format(" update S_C_Certificate set CertificateState='已借出',BorrowUser='{0}',BorrowUserName='{1}' where ID in ({2})",
                                    entity.ApplyUser, entity.ApplyUserName, borrowIDArray);
            this.ComprehensiveSQLDB.ExecuteNonQuery(sql);

            //新增证书借用记录
            foreach (var row in entity.T_C_CertificateBorrow_ApplyContent)
            {
                var applyLog = new S_C_Certificate_ApplyLog();
                applyLog.ID = FormulaHelper.CreateGuid();
                EntityCreateLogic<S_C_Certificate_ApplyLog>(applyLog);
                applyLog.Certificate = row.Certificate;
                applyLog.CertificateName = row.CertificateName;
                applyLog.Code = row.CertificateCode;
                applyLog.BorrowUser = entity.ApplyUser;
                applyLog.BorrowUserName = entity.ApplyUserName;
                applyLog.BorrowDept = entity.ApplyDept;
                applyLog.BorrowDeptName = entity.ApplyDeptName;
                applyLog.ApplyDate = entity.ApplyDate;
                applyLog.BorrowDate = entity.BorrowDate;
                applyLog.PlanReturnDate = entity.PlanReturnDate;
                applyLog.CertificatePurpose = entity.CertificatePurpose;
                applyLog.IsReturn = "否";
                applyLog.CertificateBorrowID = entity.ID;
                this.ComprehensiveDbContext.Set<S_C_Certificate_ApplyLog>().Add(applyLog);
            }
            this.ComprehensiveDbContext.SaveChanges();
            base.OnFlowEnd(entity, taskExec, routing);
        }

        public void ReturnCertificate(string ids)
        {
            string rowIDs = "'" + ids.Replace(",", "','") + "'";
            string checkSql = string.Format("select * from S_C_Certificate where ID in ({0})", rowIDs);
            var dbInfo = this.ComprehensiveSQLDB.ExecuteDataTable(checkSql);
            foreach (DataRow row in dbInfo.Rows)
            {
                if (row["CertificateState"].ToString() == "正常") throw new Formula.Exceptions.BusinessValidationException("资质已归还，无需再次进行归还操作！");
                if (row["CertificateState"].ToString() != "已借出") throw new Formula.Exceptions.BusinessValidationException("借出状态为【已借出】的资质才可以进行归还操作！");
            }

            string sql = string.Format(@"update S_C_Certificate set CertificateState='正常',BorrowUser='',BorrowUserName=''
where ID in ({0}) and CertificateState!='已作废'and CertificateState!='已遗失'

            update S_C_Certificate_ApplyLog
set ReturnDate='{1}',IsReturn = '是'
where ID in
(select ID from S_C_Certificate_ApplyLog 
where Certificate in ({0}) and (ReturnDate is null or ReturnDate='')) ", rowIDs, DateTime.Now);

            this.ComprehensiveSQLDB.ExecuteNonQuery(sql);
        }
    }
}
