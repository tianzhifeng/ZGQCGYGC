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


namespace EPC.Areas.Quality.Controllers
{
    public class QBSAcceptanceController : EPCFormContorllor<T_Q_QBSAcceptance>
    {
        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "Detail")
            {
                var qbsID = detail.GetValue("QBSID");
                var qbs = this.GetEntityByID<S_Q_QBS>(qbsID);
                if (!String.IsNullOrEmpty(dic.GetValue("Certificate")))
                {
                    qbs.Certificate = dic.GetValue("Certificate");
                    qbs.CertificateName = dic.GetValue("CertificateName");
                }
            }
        }

        protected override void OnFlowEnd(T_Q_QBSAcceptance entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var details = entity.T_Q_QBSAcceptance_Detail.ToList();
                foreach (var item in details)
                {
                    var qbs = this.GetEntityByID<S_Q_QBS>(item.QBSID);
                    if (qbs != null)
                    {
                        if (!String.IsNullOrEmpty(item.Certificate))
                        {
                            qbs.Certificate = item.Certificate;
                            qbs.CertificateName = item.CertificateName;
                        }

                        qbs.Result = String.IsNullOrEmpty(item.Result) ? 0m : Convert.ToDecimal(item.Result);
                        if (qbs.Result >= 60)
                        {
                            qbs.State = ProjectState.Finish.ToString();
                            if (!qbs.CheckFinishDate.HasValue)
                            {
                                qbs.CheckFinishDate = entity.CheckDate == null ? System.DateTime.Now : entity.CheckDate;
                            }
                        }
                        qbs.CheckFinishUser = entity.SubmitUser;
                        qbs.CheckFinishUserName = entity.SubmitUserName;
                        qbs.CheckCount = qbs.CheckCount == null ? 1 : qbs.CheckCount + 1;
                    }
                }
                this.EPCEntites.SaveChanges();
            }
        }

    }
}
