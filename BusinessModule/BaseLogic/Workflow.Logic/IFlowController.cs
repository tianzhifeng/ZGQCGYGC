using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Workflow.Logic.Domain;

namespace Workflow.Logic
{
    public interface IFlowController
    {       
        void SetFormFlowPhase(string id, string flowPhase, string stepName);

        void SetFormFlowInfo(string id, string flowInfo);

        void DeleteForm(string ids);
        bool ExistForm(string id);

        JsonResult SetSerialNumber(string id);

        JsonResult Save();
        JsonResult SaveBA(string tmplCode);
        void UnExecTaskExec(string taskExecID);
        bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment);
        bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment,string code);

        JsonResult GetTaskExec(string taskexecID);
    }
}
