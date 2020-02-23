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
using Newtonsoft.Json;
using Formula.ImportExport;


namespace EPC.Areas.ExpenseControl.Controllers
{
    public class SettlementController : EPCController<S_I_CBS_Cost>
    {
        public ActionResult InSpaceTab()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfo = engineeringInfo;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string sql = @"select S_I_CBS_Cost.*,S_I_Engineering.Name as EngineeringInfoName,
S_I_Engineering.SerialNumber as EngineeringInfoCode,ChargerUserName,ChargerUser from S_I_CBS_Cost
left join S_I_Engineering on S_I_CBS_Cost.EngineeringInfoID=S_I_Engineering.ID";
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            if (!String.IsNullOrEmpty(engineeringInfoID))
            {
                qb.Add("EngineeringInfoID", QueryMethod.Equal, engineeringInfoID);
            }
            qb.Add("State", QueryMethod.Equal, "Finish");
            var data = this.SqlHelper.ExecuteDataTable(sql, qb);
            return Json(data);
        }

        public JsonResult GetWaitList(QueryBuilder qb)
        {
            string sql = @"select S_I_CBS_Cost.*,S_I_Engineering.Name as EngineeringInfoName,
S_I_Engineering.SerialNumber as EngineeringInfoCode,ChargerUserName,ChargerUser from S_I_CBS_Cost
left join S_I_Engineering on S_I_CBS_Cost.EngineeringInfoID=S_I_Engineering.ID";
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            if (!String.IsNullOrEmpty(engineeringInfoID))
            {
                qb.Add("EngineeringInfoID", QueryMethod.Equal, engineeringInfoID);
            }
            qb.Add("State", QueryMethod.Equal, "Create");
            var data = this.SqlHelper.ExecuteDataTable(sql, qb);
            return Json(data);
        }

        public JsonResult GetCBSList()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) return Json("");
            var list = engineeringInfo.S_I_CBS.OrderBy(c => c.SortIndex).ToList();
            return Json(list);
        }

        public override JsonResult SaveList()
        {
            var listData = this.Request["ListData"];
            var list = JsonHelper.ToList(listData);
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") { continue; }
                var costInfo = this.GetEntityByID<S_I_CBS_Cost>(item.GetValue("ID"));
                if (costInfo == null)
                {
                    costInfo = new S_I_CBS_Cost();
                    costInfo.ID = FormulaHelper.CreateGuid();
                    this.UpdateEntity<S_I_CBS_Cost>(costInfo, item);
                    costInfo.State = "Create";
                    costInfo.FullID = costInfo.ID;
                    costInfo.CreateDate = DateTime.Now;
                    costInfo.CreateUser = this.CurrentUserInfo.UserName;
                    costInfo.CreateUserID = this.CurrentUserInfo.UserID;
                    costInfo.CostUser = this.CurrentUserInfo.UserID;
                    costInfo.CostUserName = this.CurrentUserInfo.UserName;
                    this.entities.Set<S_I_CBS_Cost>().Add(costInfo);
                }
                else
                {
                    this.UpdateEntity<S_I_CBS_Cost>(costInfo, item);
                }
                if (String.IsNullOrEmpty(costInfo.EngineeringInfoID))
                {
                    throw new Formula.Exceptions.BusinessValidationException("费用【" + item.GetValue("Name") + "】未指定工程信息，无法保存");
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ConfirmData(string ListData, string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            var list = JsonHelper.ToList(ListData);

            var engineerIDList = new List<string>();
            if (list.Count == 0)
            {
                if (engineeringInfo != null)
                {
                    //只更新确认指定项目的所有未确认COST数据
                    this.entities.Set<S_I_CBS_Cost>().Where(c => c.EngineeringInfoID == EngineeringInfoID
                        && !String.IsNullOrEmpty(c.CBSID) && c.State == "Create").Update(c => c.State = "Finish");
                }
                else
                {
                    engineerIDList = this.entities.Set<S_I_CBS_Cost>().Where(c => !String.IsNullOrEmpty(c.CBSID)
                      && !String.IsNullOrEmpty(c.EngineeringInfoID) && c.State == "Create").Select(c => c.EngineeringInfoID).Distinct().ToList();

                    //更新确认所有的未确认COST结算数据
                    this.entities.Set<S_I_CBS_Cost>().Where(c => !String.IsNullOrEmpty(c.CBSID)
                         && !String.IsNullOrEmpty(c.EngineeringInfoID)
                      && c.State == "Create").Update(c => c.State = "Finish");
                }
            }
            else
            {
                var costIDList = new List<string>();
                foreach (var item in list)
                {
                    costIDList.Add(item.GetValue("ID"));
                    if (String.IsNullOrEmpty(item.GetValue("CBSID")))
                    { throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】没有绑定至费用科目，请先指定费用科目后再进行结算"); }
                    if (!engineerIDList.Contains(item.GetValue("EngineeringInfoID")))
                        engineerIDList.Add(item.GetValue("EngineeringInfoID"));
                }

                //更新确认选中的COST结算数据
                this.entities.Set<S_I_CBS_Cost>().Where(c => !String.IsNullOrEmpty(c.CBSID) && costIDList.Contains(c.ID)
                    && c.State == "Create").Update(c => c.State = "Finish");
            }

            this.entities.SaveChanges();

            //汇总重新计算CBS节点上的数据
            if (engineeringInfo != null)
            {
                engineeringInfo.SumCBSCost();
            }
            else
            {
                foreach (var item in engineerIDList)
                {
                    var engineering = this.GetEntityByID<S_I_Engineering>(item);
                    if (engineering != null)
                    {
                        engineering.SumCBSCost();
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertData(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var ids = new List<string>();
            var engineeringIds = new List<string>();
            foreach (var item in list)
            {
                ids.Add(item.GetValue("ID"));
                if (!engineeringIds.Contains(item.GetValue("EngineeringInfoID")))
                    engineeringIds.Add(item.GetValue("EngineeringInfoID"));
            }
            this.entities.Set<S_I_CBS_Cost>().Where(c => ids.Contains(c.ID)).Update(c => c.State = "Create");
            this.entities.SaveChanges();

            foreach (var item in engineeringIds)
            {
                var engineering = this.GetEntityByID<S_I_Engineering>(item);
                if (engineering != null)
                {
                    engineering.SumCBSCost();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SummarySettlementData(string EngineeringInfoID)
        {
            var paymentDt = new DataTable();
            var hrDt = new DataTable();
            var reimbursementDt = new DataTable();

            string hrSQL = @"select WorkHourDt.*,isnull(UnitPrice,0) as UnitPrice from (
select ID,ProjectID,ProjectName,UserID,UserName,WorkHourValue,WorkHourDate,UserDeptID,UserDeptName from S_W_UserWorkHour
where ID not in (select distinct RelateBusinessID from {0}..S_I_CBS_Cost where RelateBusinessID is not null
and RelateBusinessID !='') and State='Locked') WorkHourDt
left join (select UserID,UserName,UnitPrice from S_HR_UserUnitPrice
where CostInfoID=(select ID from S_HR_UserCostInfo
where StartDate=(select Max(StartDate) from S_HR_UserCostInfo
where StartDate<getdate()))) UnitPriceDt
on WorkHourDt.UserID=UnitPriceDt.UserID";

            var paymentSQL = @"select * from S_P_Payment where ID not in (select distinct RelateBusinessID from S_I_CBS_Cost
where RelateBusinessID is not null and RelateBusinessID !='') ";

            var reimbursementSQL = @"select S_F_ProjectReimbursement_Contents.*,Project,ProjectName,ApplyDate,Applier,ApplierName from S_F_ProjectReimbursement_Contents
left join S_F_ProjectReimbursement on S_F_ProjectReimbursement.ID=S_F_ProjectReimbursement_Contents.S_F_ProjectReimbursementID
where S_F_ProjectReimbursement_Contents.ID not in (select distinct RelateBusinessID from S_I_CBS_Cost where RelateBusinessID is not null and RelateBusinessID !='')";

            if (!String.IsNullOrEmpty(EngineeringInfoID))
            {
                hrSQL += "where ProjectID='" + EngineeringInfoID + "'";
                paymentSQL += " and EngineeringInfoID='" + EngineeringInfoID + "'";
                reimbursementSQL += " and Project='" + EngineeringInfoID + "'";
            }
            var comprehensiveDB = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
            var epcDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            hrDt = comprehensiveDB.ExecuteDataTable(String.Format(hrSQL, epcDB.DbName));
            paymentDt = epcDB.ExecuteDataTable(paymentSQL);
            reimbursementDt = epcDB.ExecuteDataTable(reimbursementSQL);

            foreach (DataRow row in paymentDt.Rows)
            {
                //归集付款成本
                var payment = this.GetEntityByID<S_P_Payment>(row["ID"].ToString());
                payment.ToCost();
            }
            var dbContext = FormulaHelper.GetEntities<EPCEntities>();
            var infrasDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var financeList = infrasDbContext.S_T_FinanceSubject.ToList();
            foreach (DataRow row in reimbursementDt.Rows)
            {
                #region 归集报销费用成本
                var cost = new S_I_CBS_Cost();
                cost.ID = FormulaHelper.CreateGuid();
                var engineeringInfoID = row["Project"].ToString();
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
                if (engineeringInfo == null) continue;
                cost.EngineeringInfoID = engineeringInfo.ID;

                var subject = financeList.FirstOrDefault(c => c.Code == row["Type"].ToString());
                if (subject != null)
                {
                    cost.Name = subject.Name;
                    cost.Code = subject.Code;
                    var cbs = engineeringInfo.S_I_CBS.FirstOrDefault(c => c.Code == subject.RelateCBSCode);
                    if (cbs != null)
                    {
                        cost.CBSID = cbs.ID;
                        cost.CBSFullID = cbs.FullID;
                        cost.CBSName = cbs.Name;
                        cost.CBSCode = cbs.Code;
                    }
                    cost.FinaceSubjectCode = subject.Code;
                    cost.FinaceSubjectName = subject.Name;
                    cost.Name = subject.Code;
                    cost.Code = subject.Name;
                }
                else
                {
                    cost.Name = row["Type"].ToString();
                    cost.Code = row["Type"].ToString();
                }
                cost.FullID = cost.ID;
                cost.CostDate = row["ApplyDate"] == null || row["ApplyDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["ApplyDate"]);  //row["ApplyDate"].ToString() ;
                cost.CostUser = row["Applier"].ToString();
                cost.CostUserName = row["ApplierName"].ToString();
                cost.CreateDate = DateTime.Now;
                cost.CreateUser = row["Applier"].ToString();
                cost.CreateUserID = row["ApplierName"].ToString();
                cost.TotalValue = row["Cost"] == null || row["Cost"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Cost"]);
                cost.RelateID = row["ID"].ToString();
                cost.RelateFormID = row["S_F_ProjectReimbursementID"].ToString();
                cost.RelateBusinessID = row["ID"].ToString();
                cost.State = "Create";
                dbContext.S_I_CBS_Cost.Add(cost);
                #endregion
            }

            foreach (DataRow row in hrDt.Rows)
            {
                #region 归集工日费用成本
                var cost = new S_I_CBS_Cost();
                cost.ID = FormulaHelper.CreateGuid();
                var engineeringInfoID = row["ProjectID"].ToString();
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
                if (engineeringInfo == null) continue;
                cost.EngineeringInfoID = engineeringInfo.ID;
                var cbs = engineeringInfo.S_I_CBS.Where(c => c.CBSType == "Labor").OrderBy(c => c.FullID).ThenBy(c => c.SortIndex).FirstOrDefault();
                if (cbs != null)
                {
                    cost.CBSID = cbs.ID;
                    cost.CBSFullID = cbs.FullID;
                    cost.CBSName = cbs.Name;
                    cost.CBSCode = cbs.Code;
                }
                cost.FullID = cost.ID;
                cost.Name = row["UserName"] == null || row["UserName"] == DBNull.Value ? "" : row["UserName"].ToString();
                cost.Code = row["UserID"] == null || row["UserID"] == DBNull.Value ? "" : row["UserID"].ToString();
                cost.Quantity = row["WorkHourValue"] == null || row["WorkHourValue"] == DBNull.Value ? 0m : Convert.ToDecimal(row["WorkHourValue"]);
                cost.UnitPrice = row["UnitPrice"] == null || row["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(row["UnitPrice"]);
                cost.TotalValue = cost.Quantity * cost.UnitPrice;
                cost.CostDate = row["WorkHourDate"] == null || row["WorkHourDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime( row["WorkHourDate"]);
                cost.BelongYear = cost.CostDate.Value.Year;
                cost.BelongMonth = cost.CostDate.Value.Month;
                cost.UserDept = row["UserDeptID"] == null || row["UserDeptID"] == DBNull.Value ? "" : row["UserDeptID"].ToString();
                cost.UserDeptName = row["UserDeptName"] == null || row["UserDeptName"] == DBNull.Value ? "" : row["UserDeptName"].ToString();
                cost.RelateFormID = row["ID"].ToString();
                cost.RelateBusinessID = row["ID"].ToString();
                cost.RelateID = row["ID"].ToString();
                cost.State = "Create";
                dbContext.S_I_CBS_Cost.Add(cost);
                #endregion
            }
            dbContext.SaveChanges();
            return Json("");
        }
    }
}
