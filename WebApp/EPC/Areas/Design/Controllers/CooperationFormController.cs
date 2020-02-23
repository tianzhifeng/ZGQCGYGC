using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using Workflow.Logic.Domain;

namespace EPC.Areas.Design.Controllers
{
    public class CooperationFormController : EPCFormContorllor<S_E_MajorPutInfo>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);
                }
            }
            return base.PageView();
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (String.IsNullOrEmpty(dic.GetValue("Receiver")))
            {
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(dic.GetValue("EngineeringInfoID"));
                if (engineeringInfo != null)
                {
                    string majorPrinciple = "MajorPrinciple";
                    var inMajor = dic.GetValue("InMajorValue");
                    var users = this.EPCEntites.Set<S_I_OBS_User>().Where(c => c.EngineeringInfoID == engineeringInfo.ID &&
                       inMajor.Contains(c.MajorCode) && c.RoleCode == majorPrinciple).Select(c => new { userID = c.UserID, userName = c.UserName }).ToList();
                    var reciviers = String.Join(",", users.Select(d => d.userID).ToList());
                    var recivierNames = String.Join(",", users.Select(d => d.userName).ToList());
                    dic.SetValue("Receiver", reciviers.TrimEnd(','));
                    dic.SetValue("ReceiverName", recivierNames.TrimEnd(','));
                }
            }
        }

        protected override void OnFlowEnd(S_E_MajorPutInfo entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
