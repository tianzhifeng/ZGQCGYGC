using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;
using System.Data;
/**
 * 公司级部门培训计划汇总审批
 * **/
namespace HR.Areas.AutoForm.Controllers
{
    public class TrainplanApprovalController : HRFormContorllor<T_Trainmanagement_TrainplanApproval>
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


        protected override void OnFlowEnd(T_Trainmanagement_TrainplanApproval entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {

            if (entity == null)
                return;
            if (string.IsNullOrEmpty(entity.RelateID))
                entity.RelateID = entity.ID;


            var detailList = entity.T_Trainmanagement_TrainplanApproval_Trainplan.ToList();
            if (detailList.Count == 0)
                return;

            HRSQLDB.ExecuteNonQuery("delete from S_Train_CompPlan where FormID ='" + entity.RelateID + "'");

            foreach (var item in detailList)
            {
                S_Train_CompPlan plan = new S_Train_CompPlan();
                plan.ID = Formula.FormulaHelper.CreateGuid();
                plan.FormID = entity.RelateID;
                BusinessEntities.Set<S_Train_CompPlan>().Add(plan);
                plan.InOrOut = "In";

                //将表单主表数据更新至结果表
                UpdateEntity<S_Train_CompPlan>(plan, entity.ToDic());
                //将表单子表数据更新至结果表
                UpdateEntity<S_Train_CompPlan>(plan, item.ToDic());
            }
            entity.IsNewVersion = "True";
            string sql = string.Format("update T_Trainmanagement_TrainplanApproval set IsNewVersion = 'False' where RelateID='{0}' and ID <>'{1}'", entity.RelateID, entity.ID);
            HRSQLDB.ExecuteNonQuery(sql);

            BusinessEntities.SaveChanges();

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
            if (plan == null) throw new Formula.Exceptions.BusinessException("没有找到指定对的培训计划，不能进行变更操作");
            if (plan.FlowPhase != "End" || String.IsNullOrEmpty(plan.RelateID))
                throw new Formula.Exceptions.BusinessException("不能对没有审批完成的培训计划进行变更操作");

            string FlowVersionNumberStart = "1";
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];

            string sql = string.Format("select ID,VersionNumber,FlowPhase from {0} where 1=1 and RelateID='{1}'", "T_Trainmanagement_TrainplanApproval", plan.RelateID);
            sql += " order by convert(int,VersionNumber) desc";
            var dt = HRSQLDB.ExecuteDataTable(sql);

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
