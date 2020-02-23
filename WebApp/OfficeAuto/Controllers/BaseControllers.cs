using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Formula;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Controllers
{
    public class BaseController : MvcAdapter.BaseController
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseController<T> : MvcAdapter.BaseController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseFlowController<T> : MvcAdapter.BaseFlowController<T> where T : class,new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }

        //protected override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        //{

        //    bool isSucc = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);

        //    #region  发送Rtx
        //    //发送Rtx
        //    if (!isSucc)
        //    {
        //        //是否启动RTX
        //        var IsEnabledRtx = RxtLib.ConfigHelper.GetAppSection("IsEnabledRtx");
        //        if (IsEnabledRtx == "T")
        //        {
        //            if (!string.IsNullOrEmpty(nextExecUserIDs))
        //            {
        //                var nextTask = taskExec.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.EndID).OrderBy(c => c.ID).LastOrDefault(); //flowEntities.Set<S_WF_InsTask>().Where(c => c.InsDefStepID == endID).OrderBy(c => c.ID).LastOrDefault();
        //                var nextTaskExecs = nextTask.S_WF_InsTaskExec;


        //                string url = RxtLib.ConfigHelper.GetAppSection("RtxSSOAddress");
        //                string[] userIDs = nextExecUserIDs.Split(',');
        //                foreach (var userID in userIDs)
        //                {
        //                    var nextTaskExecId = nextTaskExecs.SingleOrDefault(c => c.TaskUserID == userID).ID;

        //                    string systemName = RxtLib.RtxService.GetUser(userID);
        //                    string title = "您有新任务";
        //                    url += "?SystemName=" + systemName + "%26Key=FLOW%26ID=" + nextTaskExecId;
        //                    string execUrl = "[点击处理:|" + url + "]";
        //                    RxtLib.RtxService.SendNotify(systemName, title, execUrl);
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    return isSucc;
        //}
    }
}