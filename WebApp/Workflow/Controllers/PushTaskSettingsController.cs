using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using MvcAdapter;
using Config;

namespace Workflow.Web.Controllers
{
    public class PushTaskSettingsController : BaseController
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            string sql = string.Format(@"
select FlowID=S_WF_DefFLow.ID
,FlowCode=S_WF_DefFLow.Code
, FlowName=S_WF_DefFlow.Name
,S_wF_DefStep.Code
,S_wF_DefStep.Name
,Phase
,AllowToMobile
,ID=S_WF_DefStep.ID
from S_WF_DefFlow join S_wF_DefStep on DefFLowID=S_WF_DefFlow.ID
where Type = 'Normal' order by FlowName,Name
");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult AllowPush(string listIDs)
        {
            string sql = string.Format("update S_WF_DefStep set AllowToMobile='1' where ID in('{0}')", listIDs.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult DenyPush(string listIDs)
        {
            string sql = string.Format("update S_WF_DefStep set AllowToMobile='0' where ID in('{0}')", listIDs.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }
        public string GetTaskId(string id)
        {
            SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            return Convert.ToString(helper.ExecuteScalar(string.Format("Select a.ID From S_WF_InsTaskExec a with(nolock) Left Join S_WF_InsFlow b with(nolock) On a.InsFlowID=b.ID Where b.FormInstanceID='{0}' And ExecTime Is Null", id)));
        }
    }
}
