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
using Config;

namespace Project.Areas.AutoUI.Controllers
{
    public class ManageWorkloadSettlementController : ProjectFormContorllor<T_EXE_ManageWorkloadSettlement>
    {
        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (dt.Rows.Count > 0 && dt.Columns.Contains("ManageWorkloadList"))
            {
                if (dt.Rows[0]["ManageWorkloadList"] == DBNull.Value || dt.Rows[0]["ManageWorkloadList"] == null
                || string.IsNullOrEmpty(dt.Rows[0]["ManageWorkloadList"].ToString()))
                {
                    var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                    //默认值
                    string sql = @"select Quantity as Workload,
SummaryCostQuantity as SummarySettlement,ID as CBSID,Name,Code from s_c_cbs
where ParentID=(
	select ID from s_c_cbs 
	where ProjectInfoID='{0}'
	and NodeType='Category' and Code='Manage')";
                    sql = string.Format(sql, dt.Rows[0]["ProjectInfo"].ToString());
                   var detailDt= db.ExecuteDataTable(sql);
                   dt.Rows[0]["ManageWorkloadList"] = JsonHelper.ToJson(detailDt);
                }
                #region 管理工时已结算数据

                var detailList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["ManageWorkloadList"].ToString());
                var cbsids = detailList.Select(a => a.GetValue("CBSID"));
                var cbsList = this.BusinessEntities.Set<S_C_CBS>().Where(a => cbsids.Contains(a.ID)).ToList();
                foreach (var detailDic in detailList)
                {
                    decimal? SummaryCostQuantity = null;
                    var cbs = cbsList.FirstOrDefault(a => a.ID == detailDic.GetValue("CBSID"));
                    if (cbs != null)
                    {
                        SummaryCostQuantity = cbs.SummaryCostQuantity;
                        if (cbs.SummaryCostQuantity.HasValue && cbs.SummaryCostQuantity.Value > 0)
                        {
                            var historyTexts = string.Empty;
                            var userCostList = cbs.S_C_CBS_Cost.GroupBy(a => a.CostUserName).Select(a => new { a.Key, SummaryCostQuantity = a.Sum(g => g.Quantity) }).ToList();
                            foreach (var userCost in userCostList)
                            {
                                historyTexts += userCost.Key + "(" + (userCost.SummaryCostQuantity ?? 0).ToString() + "),";
                            }
                            if (!string.IsNullOrEmpty(historyTexts))
                                detailDic.SetValue("DetailTextHistory", historyTexts.TrimEnd(','));
                        }
                    }
                    detailDic.SetValue("SummarySettlement", SummaryCostQuantity);

                }
                dt.Rows[0]["ManageWorkloadList"] = JsonHelper.ToJson(detailList);
                #endregion
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var detailList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["ManageWorkloadList"]);
            var cbsids = detailList.Select(a => a.GetValue("CBSID"));
            var cbsList = this.BusinessEntities.Set<S_C_CBS>().Where(a => cbsids.Contains(a.ID)).ToList();
            if (cbsList.Count != detailList.Count)
                throw new Formula.Exceptions.BusinessException("S_C_CBS数据不匹配，请联系管理员");
            string msgCBS = "";
            foreach (var item in detailList)
            {
                var cbs = cbsList.FirstOrDefault(a => a.ID == item.GetValue("CBSID"));
                var settlement = 0m;
                if (!string.IsNullOrEmpty(item.GetValue("Settlement")))
                    settlement = Convert.ToDecimal(item.GetValue("Settlement"));
                decimal remain = Convert.ToDecimal(cbs.Quantity) - Convert.ToDecimal(cbs.SummaryCostQuantity);
                if (settlement > remain)
                    msgCBS += item["Name"].ToString() + ",";
            }
            if (!string.IsNullOrEmpty(msgCBS))
                throw new Formula.Exceptions.BusinessException("管理工时【" + msgCBS.TrimEnd(',') + "】的本次结算工时不能大于可结算工时");
        }

        protected override void OnFlowEnd(T_EXE_ManageWorkloadSettlement entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var detaillist = entity.T_EXE_ManageWorkloadSettlement_ManageWorkloadList.ToList();
            var cbsids = detaillist.Select(a => a.CBSID).ToList();
            var cbsList = this.BusinessEntities.Set<S_C_CBS>().Where(a => cbsids.Contains(a.ID)).ToList();
            var nodeType = CBSNodeType.Root.ToString();
            var rootCBS = this.BusinessEntities.Set<S_C_CBS>().FirstOrDefault(a => a.ProjectInfoID == entity.ProjectInfo && a.NodeType == nodeType);
            var codes = detaillist.Select(a => a.Code).ToList();
            var obsList = this.BusinessEntities.Set<S_W_OBSUser>().Where(a => codes.Contains(a.RoleCode)).Select(a => new { a.RoleCode, a.RoleName, a.UserID }).Distinct().ToList();

            foreach (var detail in detaillist)
            {
                var cbs = cbsList.FirstOrDefault(a => a.ID == detail.CBSID);
                //结算人员的管理工时
                if (string.IsNullOrEmpty(detail.DetailJson))
                    continue;
                if (Convert.ToDecimal(detail.Settlement) == 0)
                    continue;
                var userList = JsonHelper.ToObject<List<Dictionary<string, string>>>(detail.DetailJson);
                foreach (var userItem in userList)
                {
                    var settlement = 0m;
                    if (!string.IsNullOrEmpty(userItem.GetValue("UserWorkload")))
                        settlement = Convert.ToDecimal(userItem.GetValue("UserWorkload"));
                    if (settlement == 0)
                        continue;
                    var cost = new S_C_CBS_Cost();
                    cost.Code = detail.Code;
                    cost.Name = detail.Name;
                    cost.Quantity = settlement;
                    
                    cost.CostUser = userItem.GetValue("UserID");
                    cost.CostUserName =userItem.GetValue("UserName") ;
                    var userinfo = FormulaHelper.GetUserInfoByID(cost.CostUser);
                    if (userinfo != null)
                    {
                        cost.UserDept = userinfo.UserOrgID;
                        cost.UserDeptName = userinfo.UserOrgName;
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
                    var obs = obsList.FirstOrDefault(a => a.UserID == cost.CostUser && a.RoleCode == cost.Code);
                    if (obs != null)
                    {
                        cost.RoleCode = obs.RoleCode;
                        cost.RoleName = obs.RoleName;
                    }
                    cost.FormID = entity.ID;
                    cbs.AddCost(cost);
                    if (cost.Quantity.HasValue && cost.UnitPrice.HasValue)
                        cost.TotalValue = cost.Quantity.Value * cost.UnitPrice.Value;
                }
            }

            this.BusinessEntities.SaveChanges();
            rootCBS.Ansestors.FirstOrDefault(a => a.NodeType == CBSNodeType.Root.ToString()).SummaryCost();
            this.BusinessEntities.SaveChanges();
        } 

    }
}
