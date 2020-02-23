using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Config.Logic;
using Config;
using HR.Logic.Domain;
using HR.Logic.BusinessFacade;
using Project.Logic;
using Project.Logic.Domain;

namespace HR.Areas.Selector.Controllers
{
    public class ContractProjectSelectController : HRController
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            var projectSql = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var hrSql = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var sql = string.Format(@"select prj.ID,prj.Name,prj.Code,prj.CustomerID,prj.CustomerName,prj.PhaseValue,prj.PhaseName
	,prj.ChargeUserID,prj.ChargeUserName,prj.ChargeDeptID,prj.ChargeDeptName,prj.Country,prj.Province,prj.City
	,prj.PlanStartDate,prj.FactFinishDate,prj.State as ProjectState,prj.ProjectClass,prj.ProjectLevel,pr.Industry,pr.BuildAddress
	,mc.ID as ContractID,mc.Name as ContractName,mc.SignDate,mc.BelongYear,mc.BelongMonth,mc.ContractRMBAmount,mc.StartDate,mc.EndDate
	,cast(mc.BelongYear as varchar)+right('00'+cast(mc.BelongMonth as varchar),2) as ContractBelongYearMonth
from {0}..S_I_ProjectInfo prj
left join S_I_Project pr
on prj.MarketProjectInfoID=pr.ID
left join S_C_ManageContract_ProjectRelation cpr
on pr.ID=cpr.ProjectID
left join S_C_ManageContract mc
on cpr.S_C_ManageContractID=mc.ID
where prj.ID not in (
	select distinct ProjectInfoID from {1}..S_P_Performance 
	where (mc.ID=ContractID and ContractID is not null) or (ContractID is null or ContractID =''))", projectSql.DbName, hrSql.DbName);

            var marketSql = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var db = marketSql.ExecuteGridData(sql, qb);
            return Json(db);
        }

        public string AddPerformanceInfo(string row)
        {
            var rowData = JsonHelper.ToObject(row);
            var performance = new S_P_Performance();
            performance.ID = FormulaHelper.CreateGuid();
            this.UpdateEntity<S_P_Performance>(performance, rowData);

            performance.ProjectInfoID = rowData.GetValue("ID");
            performance.ChargeUser = rowData.GetValue("ChargeUserID");

            this.entities.Set<S_P_Performance>().Add(performance);
            this.SynchEmpPerformance(performance.ChargeUser, performance, AuditRoles.ProjectManager.ToString());

            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var rbsUser = projectEntities.Set<S_W_RBS>().Where(d => d.ProjectInfoID == performance.ProjectInfoID && d.WBSType == "Major").ToList();
            var majorList = rbsUser.Select(d => d.MajorValue).Distinct();

            foreach (var major in majorList)
            {
                var rbsInfo = new S_P_Performance_JoinPeople();
                rbsInfo.ID = FormulaHelper.CreateGuid();
                rbsInfo.S_P_PerformanceID = performance.ID;
                rbsInfo.Major = major;
                var rbsList = rbsUser.Where(d => d.MajorValue == major).ToList();

                #region 人员添加

                //专业负责人
                var role = AuditRoles.MajorPrinciple.ToString();
                var userList = rbsList.Where(d => d.RoleCode == role).ToList();
                var userIDs = userList.Select(d => d.UserID).Distinct().ToList();
                rbsInfo.MajorPrinciple = string.Join(",", userIDs);
                rbsInfo.MajorPrincipleName = string.Join(",", userList.Select(d => d.UserName).Distinct().ToList());
                if (userIDs.Count != 0)
                    foreach (var userID in userIDs)
                        this.SynchEmpPerformance(userID, performance, role);

                //设计人
                role = AuditRoles.Designer.ToString();
                userList = rbsList.Where(d => d.RoleCode == role).ToList();
                userIDs = userList.Select(d => d.UserID).Distinct().ToList();
                rbsInfo.Designer = string.Join(",", userIDs);
                rbsInfo.DesignerName = string.Join(",", userList.Select(d => d.UserName).Distinct().ToList());
                if (userIDs.Count != 0)
                    foreach (var userID in userIDs)
                        this.SynchEmpPerformance(userID, performance, role);

                //校核人
                role = AuditRoles.Collactor.ToString();
                userList = rbsList.Where(d => d.RoleCode == role).ToList();
                userIDs = userList.Select(d => d.UserID).Distinct().ToList();
                rbsInfo.Collactor = string.Join(",", userIDs);
                rbsInfo.CollactorName = string.Join(",", userList.Select(d => d.UserName).Distinct().ToList());
                if (userIDs.Count != 0)
                    foreach (var userID in userIDs)
                        this.SynchEmpPerformance(userID, performance, role);

                //审核人
                role = AuditRoles.Auditor.ToString();
                userList = rbsList.Where(d => d.RoleCode == role).ToList();
                userIDs = userList.Select(d => d.UserID).Distinct().ToList();
                rbsInfo.Auditor = string.Join(",", userIDs);
                rbsInfo.AuditorName = string.Join(",", userList.Select(d => d.UserName).Distinct().ToList());
                if (userIDs.Count != 0)
                    foreach (var userID in userIDs)
                        this.SynchEmpPerformance(userID, performance, role);

                //审定人
                role = AuditRoles.Approver.ToString();
                userList = rbsList.Where(d => d.RoleCode == role).ToList();
                userIDs = userList.Select(d => d.UserID).Distinct().ToList();
                rbsInfo.Approver = string.Join(",", userIDs);
                rbsInfo.ApproverName = string.Join(",", userList.Select(d => d.UserName).Distinct().ToList());
                if (userIDs.Count != 0)
                    foreach (var userID in userIDs)
                        this.SynchEmpPerformance(userID, performance, role);

                #endregion

                this.entities.Set<S_P_Performance_JoinPeople>().Add(rbsInfo);
            }

            this.entities.SaveChanges();

            return performance.ID;
        }

        private void SynchEmpPerformance(string userID, S_P_Performance performance, string role)
        {
            var workPerformance = this.entities.Set<T_EmployeeWorkPerformance>().FirstOrDefault(d => d.RelateID == performance.ID && d.UserID == userID);
            if (workPerformance == null)
            {
                workPerformance = new T_EmployeeWorkPerformance();
                workPerformance.SynchEmpPerformance(userID, performance, role);
                this.entities.Set<T_EmployeeWorkPerformance>().Add(workPerformance);
            }
            else
                workPerformance.ProjectRole += "," + role;

            this.entities.SaveChanges();
        }
    }
}
