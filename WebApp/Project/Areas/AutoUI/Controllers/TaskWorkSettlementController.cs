using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config.Logic;
using Formula;

namespace Project.Areas.AutoUI.Controllers
{
    public class TaskWorkSettlementController : ProjectFormContorllor<T_EXE_TaskWorkSettlement>
    {

        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            #region 卷册已结算数据
            if (dt.Rows.Count > 0 && dt.Columns.Contains("TaskWorkList")
                && dt.Rows[0]["TaskWorkList"] != DBNull.Value && dt.Rows[0]["TaskWorkList"] != null
                && !string.IsNullOrEmpty(dt.Rows[0]["TaskWorkList"].ToString()))
            {
                var detailList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["TaskWorkList"].ToString());
                var budgetids = detailList.Select(a => a.GetValue("BudgetID"));
                var budgetList = this.BusinessEntities.Set<S_C_CBS_Budget>().Where(a => budgetids.Contains(a.ID)).ToList();
                foreach (var detailDic in detailList)
                {
                    decimal? SummaryCostQuantity = null;
                    var budget = budgetList.FirstOrDefault(a => a.ID == detailDic.GetValue("BudgetID"));
                    if (budget != null)
                        SummaryCostQuantity = budget.SummaryCostQuantity;
                    detailDic.SetValue("SummarySettlement", SummaryCostQuantity);
                }
                if (dt.Rows.Count > 0 && dt.Columns.Contains("TaskWorkList"))
                    dt.Rows[0]["TaskWorkList"] = JsonHelper.ToJson(detailList);
            }
            #endregion
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //base.BeforeSave(dic, formInfo, isNew);
            var detailList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["TaskWorkList"]);
            var budgetids = detailList.Select(a => a.GetValue("BudgetID"));
            var budgetList = this.BusinessEntities.Set<S_C_CBS_Budget>().Where(a => budgetids.Contains(a.ID)).ToList();
            if (budgetList.Count != detailList.Count)
                throw new Formula.Exceptions.BusinessException("S_C_CBS_Budget数据不匹配，请联系管理员");
            var taskList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => budgetids.Contains(a.ID)).ToList();
            if (budgetList.Count != detailList.Count)
                throw new Formula.Exceptions.BusinessException("S_C_CBS_Budget数据与S_W_TaskWork不匹配，请联系管理员");
            foreach (var item in taskList)
                item.SettlementID = dic.GetValue("ID");
            string msgTaskWorks = "";
            foreach (var item in detailList)
            {
                var budget = budgetList.FirstOrDefault(a => a.ID == item.GetValue("BudgetID"));
                var settlement = 0m;
                if (!string.IsNullOrEmpty(item.GetValue("Settlement")))
                    settlement = Convert.ToDecimal(item.GetValue("Settlement"));
                decimal remain = Convert.ToDecimal(budget.Quantity) - Convert.ToDecimal(budget.SummaryCostQuantity);
                if (settlement > remain)
                    msgTaskWorks += item["Name"].ToString() + ",";
            }
            if (!string.IsNullOrEmpty(msgTaskWorks))
                throw new Formula.Exceptions.BusinessException("卷册【" + msgTaskWorks.TrimEnd(',') + "】的本次结算工时不能大于可结算工时");
        }

        protected override void OnFlowEnd(T_EXE_TaskWorkSettlement entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();
            //base.OnFlowEnd(entity, taskExec, routing);
            var detaillist = entity.T_EXE_TaskWorkSettlement_TaskWorkList.ToList();
            var budgetids = detaillist.Select(a => a.BudgetID).ToList();
            var budgetList = this.BusinessEntities.Set<S_C_CBS_Budget>().Where(a => budgetids.Contains(a.ID)).ToList();
            var taskList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => budgetids.Contains(a.ID)).ToList();

            var nodeType = CBSNodeType.Root.ToString();
            var rootCBS = this.BusinessEntities.Set<S_C_CBS>().FirstOrDefault(a => a.ProjectInfoID == entity.ProjectInfo && a.NodeType == nodeType);

            foreach (var detail in detaillist)
            {
                var budget = budgetList.FirstOrDefault(a => a.ID == detail.BudgetID);
                var task = taskList.FirstOrDefault(a => a.ID == budget.ID);
                var taskRoleRateList = task.S_W_TaskWork_RoleRate.ToList();
                //校验预算不能超过
                var settlement = Convert.ToDecimal(detail.Settlement);
                decimal remain = Convert.ToDecimal(budget.Quantity) - Convert.ToDecimal(budget.SummaryCostQuantity);
                if (settlement > remain)
                    throw new Formula.Exceptions.BusinessException("卷册【" + budget.Name + "】的本次结算工时不能大于可结算工时");
                //分配人员工时
                var n = 0;
                var sumQuantity = 0m;
                var roleBudgets = budget.Children.Where(a => a.Quantity.HasValue && a.Quantity.Value != 0).ToList();
                foreach (var roleBudget in roleBudgets)
                {
                    n++;
                    var cost = new S_C_CBS_Cost();
                    cost.Code = roleBudget.Code;
                    cost.Name = roleBudget.Name;
                    //获取角色人员
                    var rbs = task.S_W_WBS.GetUser(roleBudget.Code);
                    if (rbs != null)
                    {
                        cost.CostUser = rbs.UserID;
                        cost.CostUserName = rbs.UserName;
                        var userinfo = FormulaHelper.GetUserInfoByID(cost.CostUser);
                        if (userinfo != null)
                        {
                            cost.UserDept = userinfo.UserOrgID;
                            cost.UserDeptName = userinfo.UserOrgName;
                        }
                    }
                    //获取角色比例，计算人员结算工时
                    if (n == roleBudgets.Count)
                    {
                        cost.Quantity = Convert.ToDecimal(detail.Settlement) - sumQuantity;
                    }
                    else
                    {
                        var rate = 0m;
                        var roleRate = taskRoleRateList.FirstOrDefault(a => a.Role == roleBudget.Code);
                        if (roleRate != null && roleRate.Rate.HasValue)
                            rate = roleRate.Rate.Value;
                        cost.Quantity = Math.Round(Convert.ToDecimal(detail.Settlement) * rate / 100, 2);
                        sumQuantity += cost.Quantity.Value;
                    }
                    cost.BelongDept = entity.BelongDept;
                    cost.BelongDeptName = entity.BelongDeptName;
                    var costDate = DateTime.Now;
                    if (entity.BelongDate.HasValue)
                        costDate = entity.BelongDate.Value;
                    cost.BelongYear = costDate.Year;
                    cost.BelongQuarter = (costDate.Month - 1) / 3 + 1;
                    cost.BelongMonth = costDate.Month;
                    cost.CostDate = costDate;
                    cost.MajorCode = detail.Major;
                    cost.MajorName = enumService.GetEnumText("Project.Major", detail.Major);
                    cost.TaskWorkCode = budget.Code;
                    cost.TaskWorkName = budget.Name;
                    cost.RoleCode = roleBudget.Code;
                    cost.RoleName = roleBudget.Name;
                    cost.FormID = entity.ID;
                    if (cost.Quantity.HasValue && cost.UnitPrice.HasValue)
                        cost.TotalValue = cost.Quantity.Value * cost.UnitPrice.Value;
                    roleBudget.AddCost(cost);
                }
                //同步卷册分配数据
                budget.SummaryCost();
                task.WorkloadFinish = budget.SummaryCostQuantity;
                //卷册完成
                if (task.WorkloadFinish >= task.Workload)
                    task.Finish();
            }

            this.BusinessEntities.SaveChanges();
            rootCBS.Ansestors.FirstOrDefault(a => a.NodeType == CBSNodeType.Root.ToString()).SummaryCost();
            this.BusinessEntities.SaveChanges();
        }
    }
}
