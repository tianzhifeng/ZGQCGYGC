using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula;
using Config;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class MyDelegateTaskController : BaseController
    {
        public JsonResult GetDoneList(QueryBuilder qb)
        {
            string sql = string.Format("select S_WF_InsTaskExec.*,TaskName from S_WF_InsTask join S_WF_InsTaskExec on InsTaskID=S_WF_InsTask.ID and TaskUserID='{0}' and ExecUserID<>'{0}' and ExecTime is not null", FormulaHelper.UserID);

            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            var result = sqlHelper.ExecuteGridData(sql, qb);

            return Json(result);
        }

        public JsonResult GetUnDoList(QueryBuilder qb)
        {
            string sql = string.Format("select S_WF_InsTaskExec.*,TaskName from S_WF_InsTask join S_WF_InsTaskExec on InsTaskID=S_WF_InsTask.ID and TaskUserID='{0}' and ExecUserID<>'{0}' and ExecTime is null", FormulaHelper.UserID);

            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            var result = sqlHelper.ExecuteGridData(sql, qb);

            return Json(result);
        }

        public JsonResult WithdrawDelegateTask(string ListIDs)
        {
            string sql = string.Format("update S_WF_InsTaskExec set ExecUserID=TaskUserID,ExecUserName=TaskUserName where ID in('{0}')", ListIDs.Replace(",", "','"));

            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            sqlHelper.ExecuteNonQuery(sql);

            return Json("");
        }
    }
}
