using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using Workflow.Logic.Domain;
using Workflow.Logic;
using System.Text;

namespace Project.Areas.AutoUI.Controllers
{
    public class TaskWorkPublishController : ProjectFormContorllor<T_EXE_TaskWorkPublish>
    {
        protected override void OnFlowEnd(T_EXE_TaskWorkPublish entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var baseconfig = FormulaHelper.GetEntities<BaseConfigEntities>();
            var taskWorks = entity.T_EXE_TaskWorkPublish_TaskWork;
            var RoleNames = baseconfig.Set<S_D_RoleDefine>().ToList();
            foreach (var taskWork in taskWorks)
            {
                var cbs = this.BusinessEntities.Set<S_C_CBS>().FirstOrDefault(a => a.ProjectInfoID == entity.ProjectInfoID
                    && a.Code == taskWork.Major);
                var budget = this.BusinessEntities.Set<S_C_CBS_Budget>().FirstOrDefault(a => a.ID == taskWork.TaskWorkID);
                if (budget == null)
                {
                    budget = new S_C_CBS_Budget();
                    budget.ID = taskWork.TaskWorkID;
                    budget.Name = taskWork.Name;
                    budget.Code = taskWork.Code;
                    cbs.AddBudget(budget);
                }
                budget.Quantity = taskWork.Workload;
                if (budget.Quantity.HasValue && budget.UnitPrice.HasValue)
                    budget.TotalValue = budget.Quantity.Value * budget.UnitPrice.Value;
                if (!string.IsNullOrEmpty(taskWork.RoleRate))
                {
                    var roles = JsonHelper.ToList(taskWork.RoleRate);
                    foreach (var role in roles)
                    {
                        var roleRateID = role.GetValue("ID");
                        var subBudget = this.BusinessEntities.Set<S_C_CBS_Budget>().FirstOrDefault(a => a.ID == roleRateID);
                        if (subBudget == null)
                        {
                            subBudget = new S_C_CBS_Budget();
                            subBudget.ID = roleRateID;
                            subBudget.Code = role.GetValue("Role");
                            subBudget.Name = RoleNames.FirstOrDefault(a => a.RoleCode == subBudget.Code).RoleName;
                            budget.AddChild(subBudget);
                        }
                        subBudget.Quantity = string.IsNullOrEmpty(role.GetValue("Workload")) ? 0m : Convert.ToDecimal(role.GetValue("Workload"));
                        if (subBudget.Quantity.HasValue && subBudget.UnitPrice.HasValue)
                            subBudget.TotalValue = subBudget.Quantity.Value * subBudget.UnitPrice.Value;
                    }
                }
                var task = this.BusinessEntities.Set<S_W_TaskWork>().FirstOrDefault(a => a.ID == taskWork.TaskWorkID);
                if (task != null)
                {
                    task.State = TaskWorkState.Execute.ToString();
                    if (taskWork.IsChanged == "true")
                    {
                        task.Workload = taskWork.Workload;
                        task.PlanEndDate = taskWork.PlanEndDate;
                        if (!string.IsNullOrEmpty(taskWork.RoleRate))
                        {
                            var role = JsonHelper.ToList(taskWork.RoleRate);
                            foreach (var r in role)
                            {
                                var roleRate = this.GetEntityByID<S_W_TaskWork_RoleRate>(r.GetValue("ID"));

                                roleRate.Workload = string.IsNullOrEmpty(r.GetValue("Workload")) ? 0m : Convert.ToDecimal(r.GetValue("Workload"));
                                roleRate.Rate = string.IsNullOrEmpty(r.GetValue("Rate")) ? 0m : Convert.ToDecimal(r.GetValue("Rate"));
                            }
                        }
                    }
                    task.Publish();
                }
                this.BusinessEntities.SaveChanges();
                cbs.Ansestors.FirstOrDefault(a => a.NodeType == CBSNodeType.Root.ToString()).SummaryBudget();
            }
            this.BusinessEntities.SaveChanges();
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var majors = detailList.Select(a => a.GetValue("Major")).Distinct().ToList();
            foreach (var major in majors)
            {
                var cbs = this.BusinessEntities.Set<S_C_CBS>().FirstOrDefault(a => a.ProjectInfoID == projectInfoID
                    && a.Code == major);
                var budgetSum = cbs.S_C_CBS_Budget.Where(a => string.IsNullOrEmpty(a.ParentID)).Select(a => a.Quantity).Sum();
                decimal thisWorkloadSum = 0;
                foreach (var detail in detailList.Where(a => a.GetValue("Major") == major))
                {
                    thisWorkloadSum += string.IsNullOrEmpty(detail.GetValue("Workload")) ? 0m : Convert.ToDecimal(detail.GetValue("Workload"));
                }
                if (cbs.Quantity < budgetSum + thisWorkloadSum)
                    throw new Formula.Exceptions.BusinessException("要下达的工时总预算大于专业的工时，请重新策划。");
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var detailList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["TaskWork"]);
            var taskWorkIDs = detailList.Select(a => a.GetValue("TaskWorkID"));
            var taskWorkList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => taskWorkIDs.Contains(a.ID)).ToList();
            if (taskWorkList.Count != detailList.Count)
                throw new Formula.Exceptions.BusinessException("S_W_TaskWork数据不匹配，请联系管理员");
            foreach (var item in taskWorkList)
                    item.PublishID = dic.GetValue("ID");
        }

        public override JsonResult Delete()
        {
            var sql = @"update S_W_TaskWork set State = 'Plan',PublishID = '' where ID in (
select TaskWorkID from T_EXE_TaskWorkPublish_TaskWork where 
T_EXE_TaskWorkPublishID in ('{0}'))";
            sql = String.Format(sql, String.Join("','", Request["ID"]));
            this.ProjectSQLDB.ExecuteNonQuery(sql);
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            return Json("");
        }
    }
}