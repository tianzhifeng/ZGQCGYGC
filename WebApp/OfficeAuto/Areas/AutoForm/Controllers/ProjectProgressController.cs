using Config.Logic;
using Formula.Helper;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    /// <summary>
    /// 课题进度
    /// </summary>
    public class ProjectProgressController : OfficeAutoFormContorllor<T_Technology_ProjectProgress>
    {
        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                if (String.IsNullOrEmpty(upperVersionID))
                {
                    //如果第一次下任务单给予一个初始的版本号
                    string FlowVersionNumberStart = "1";
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                        FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];
                    dt.Rows[0]["VersionNumber"] = FlowVersionNumberStart;
                }
            }

        }



        protected override void OnFlowEnd(T_Technology_ProjectProgress entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                if (string.IsNullOrEmpty(entity.RelateID))
                    entity.RelateID = entity.ID;


                BusinessEntities.SaveChanges();

            }

            base.OnFlowEnd(entity, taskExec, routing);

        }

        /// <summary>
        /// 变更升版验证
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public JsonResult ValidateUpVersion(string Data)
        {
            var data = JsonHelper.ToObject(Data);
            var plan = this.GetEntityByID(data.GetValue("ID"));
            if (plan == null) throw new Formula.Exceptions.BusinessException("没有找到指定对的课题进度，不能进行升版操作");
            if (plan.FlowPhase != "End" || String.IsNullOrEmpty(plan.RelateID))
                throw new Formula.Exceptions.BusinessException("不能对没有审批完成的课题进度进行升版操作");

            string FlowVersionNumberStart = "1";
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];

            string sql = string.Format("select ID,VersionNumber,FlowPhase from {0} where 1=1 and RelateID='{1}'", "T_Technology_ProjectProgress", plan.RelateID);
            sql += " order by convert(int,VersionNumber) desc";
            var dt = SQLDB.ExecuteDataTable(sql);
            var versions = string.Join(",", dt.AsEnumerable().Select(c => c["VersionNumber"].ToString()));
            return Json(new
            {
                ID = plan.ID,
                RelateID = plan.RelateID,
                VersionNumber = versions[0],
                Versions = versions
            });
        }

    }
}
