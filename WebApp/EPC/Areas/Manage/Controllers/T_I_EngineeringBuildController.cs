using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;

namespace EPC.Areas.Manage.Controllers
{
    public class EngineeringBuildController : EPCFormContorllor<T_I_EngineeringBuild>
    {
        protected override void OnFlowEnd(T_I_EngineeringBuild entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult GetTaskNoticeList(string EngineeringInfoID)
        {
            string sql = String.Format("select * from T_CP_TaskNotice where EngineeringID='{0}'", EngineeringInfoID);
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = db.ExecuteDataTable(sql);
            return Json(dt);
        }
    }
}
