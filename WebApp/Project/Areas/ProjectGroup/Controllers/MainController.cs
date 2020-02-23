using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using Formula.ImportExport;
using MvcAdapter;
using Project.Areas.ProjectGroup.Models;
using Project.Logic;
using Project.Logic.Domain;
using Base.Logic.Domain;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class MainController : ProjectController
    {
        public ActionResult Index()
        {
            var manDepts = EnumBaseHelper.GetEnumDef("System.ManufactureDept").EnumItem;
            var planState = ProjectCommoneState.Plan.ToString();
            var executeState = ProjectCommoneState.Execute.ToString();
            var projectList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.State == planState || d.State == executeState);
            var delayProjectList = projectList.Where(d => d.S_P_MileStone.Count(c => c.FactFinishDate == null && c.PlanFinishDate < DateTime.Now) > 0);
            var deptList = new List<Dictionary<string, object>>();
            foreach (var item in manDepts)
            {
                var result = new Dictionary<string, object>();
                result.SetValue("ID", item.Code);
                result.SetValue("Name", item.Name);
                if (item.Code == Config.Constant.OrgRootID)
                {
                    result.SetValue("Type", OrgType.Company.ToString());
                    result.SetValue("LinkUrl", "MainPage");
                    result.SetValue("ProjectCount", projectList.Count());
                    result.SetValue("DelayCount", delayProjectList.Count());
                }
                else
                {
                    result.SetValue("Type", OrgType.ManufactureDept.ToString());
                    result.SetValue("LinkUrl", "MainPageDept?DeptID=" + item.Code);
                    result.SetValue("ProjectCount", projectList.Count(d => d.ChargeDeptID == item.Code));
                    result.SetValue("DelayCount", delayProjectList.Count(d => d.ChargeDeptID == item.Code));
                }
                deptList.Add(result);
            }
            ViewBag.DeptInfoList = deptList;
            return View();
        }

        public ActionResult MainPage()
        {
            var planState = ProjectCommoneState.Plan.ToString();
            var executeState = ProjectCommoneState.Execute.ToString();
            var projectList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.State == planState || d.State == executeState);
            var businessType = EnumBaseHelper.GetEnumDef("Base.ProjectClass").EnumItem;
            var businessList = new List<Dictionary<string, object>>();
            foreach (var item in businessType)
            {
                var result = new Dictionary<string, object>();
                result.SetValue("ID", item.Code);
                result.SetValue("Name", item.Name);
                result.SetValue("ProjectCount", projectList.Count(d => d.ProjectClass == item.Code));
                businessList.Add(result);
            }
            ViewBag.BusinessList = businessList;


            #region 项目数量统计
            var sql = @"select  Count(0) as DataValue,Year(CreateDate) as BelongYear,Month(CreateDate) as BelongMonth,[State],SignContractCount 
from (select S_I_Project.ID,S_I_Project.State,S_I_Project.CreateDate,S_I_Project.Name,Code,
isnull(SignContractCount,0) as SignContractCount,
isnull(ProjectValue,0) as ProjectValue from S_I_Project
left join (select sum(case when IsSigned='Signed' then 1 else 0 end) SignContractCount,Sum(ProjectValue) as ProjectValue,ProjectID from S_C_ManageContract_ProjectRelation r
left join S_C_ManageContract c on r.S_C_ManageContractID=c.ID
group by ProjectID) ProjectSignInfo on S_I_Project.ID=ProjectSignInfo.ProjectID) tableInfo
group by Year(CreateDate),Month(CreateDate),[State],SignContractCount ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var projectDt = db.ExecuteDataTable(sql);

            var obj = projectDt.Compute("Sum(DataValue)", " State in ('" + ProjectCommoneState.Plan.ToString() + "','" + ProjectCommoneState.Execute.ToString() + "') ");
            var projectCount = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj)) : 0;
            var projectDic = new Dictionary<string, object>();
            projectDic.SetValue("ProjectCount", projectCount);


            obj = projectDt.Compute("Sum(DataValue)", " SignContractCount>0 ");
            var signCount = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj)) : 0;
            projectDic.SetValue("SignCount", signCount);
            var signScale = projectCount == 0 ? 0 : Math.Round(Convert.ToDecimal(signCount / projectCount * 100), 2);
            projectDic.SetValue("SignScale", signScale);

            ViewBag.ProjectDic = projectDic;
            #endregion

            #region 在建合同统计
            sql = @"select isnull(ContractRMBAmount,0)-isnull(SumReceiptValue,0) as DataValue ,
isnull(SumInvoiceValue,0)-isnull(SumReceiptValue,0) as InvoiceValue
from dbo.S_C_ManageContract where IsSigned='{0}'";
            var dt = db.ExecuteDataTable(String.Format(sql, ""));
            var contractDic = new Dictionary<string, object>();

            obj = dt.Compute("Sum(DataValue)", "  ");
            var contractValue = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj) / 10000, 0) : 0;
            contractDic.SetValue("ContractValue", contractValue);

            obj = db.ExecuteScalar(String.Format(@"select Sum(Amount) as DataValue from S_C_Receipt where BelongYear={0}", DateTime.Now.Year));
            var receiptValue = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj) / 10000, 0) : 0;
            contractDic.SetValue("ReceiptValue", receiptValue);

            obj = db.ExecuteScalar(String.Format(@"select Sum(isnull(ReceiptValue,0)- isnull(FactReceiptValue,0)) as DataValue from S_C_ManageContract_ReceiptObj
where MileStoneState='{0}'", true.ToString()));
            var InvoiceValue = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj) / 10000, 0) : 0;
            contractDic.SetValue("InvoiceValue", InvoiceValue);


            sql = @"select isnull(Sum(CostValue),0)/10000 as CostValue,CostType from S_FC_CostInfo
where BelongYear={0} group by CostType";
            var costDt = db.ExecuteDataTable(String.Format(sql, DateTime.Now.Year));

            var costDic = new Dictionary<string, object>();

            obj = costDt.Compute("Sum(CostValue)", " CostType='UserCost'");
            var userCost = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj), 2) : 0;
            costDic.SetValue("UserCost", userCost);

            obj = costDt.Compute("Sum(CostValue)", " CostType='Production'");
            var production = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj), 2) : 0;
            costDic.SetValue("Production", production);

            obj = costDt.Compute("Sum(CostValue)", " CostType='Payment'");
            var payment = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj), 2) : 0;
            costDic.SetValue("Payment", payment);

            ViewBag.ContractDic = contractDic;
            ViewBag.CostDic = costDic;
            #endregion

            return View();
        }

        #region 项目概览

        public ActionResult OverViewMain()
        {
            var depts = EnumBaseHelper.GetEnumDef("System.ManDept").EnumItem;
            var deptsIds = depts.Select(d => d.Code).ToList();
            var baseDb = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var manUserDt = baseDb.ExecuteDataTable(String.Format(@"select count(0) as DataValue from S_A_User where IsDeleted='0' and DeptID in ('{0}')", String.Join(",", deptsIds).Replace(",", "','")));
            var userDt = baseDb.ExecuteDataTable(@"select count(0) as DataValue from S_A_User where IsDeleted='0' ");
            var obj = manUserDt.Compute("Sum(DataValue)", "");
            var manUserCount = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
            obj = userDt.Compute("Sum(DataValue)", "");
            var userCount = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
            ViewBag.ManUserCount = manUserCount;
            ViewBag.UserCount = userCount;
            ViewBag.ManUserScale = userCount == 0 ? 0m : Math.Round(Convert.ToDecimal(manUserCount) / Convert.ToDecimal(userCount) * 100);

            var planState = ProjectCommoneState.Plan.ToString();
            var executeState = ProjectCommoneState.Execute.ToString();
            var projectCount = this.entities.Set<S_I_ProjectInfo>().Where(d => d.State == planState || d.State == executeState).Count();
            var manUserProjectCount = manUserCount == 0 ? 0m : Math.Round(Convert.ToDecimal(projectCount) / Convert.ToDecimal(manUserCount), 2);
            var currentYearReceiptValue = 0m;
                // FormulaHelper.GetEntities<MarketEntities>().S_C_Receipt.Count(d => d.BelongYear == DateTime.Now.Year) > 0 ?
                //FormulaHelper.GetEntities<MarketEntities>().S_C_Receipt.Where(d => d.BelongYear == DateTime.Now.Year).Sum
                //(d => d.Amount) : 0m;
            var manUserReceipt = manUserCount == 0 ? 0m : Math.Round(Convert.ToDecimal(currentYearReceiptValue) / Convert.ToDecimal(manUserCount) / 10000, 2);

            ViewBag.ManUserReceipt = manUserReceipt;
            ViewBag.ManUserProjectCount = manUserProjectCount;

            return View();
        }

        public JsonResult LoadData(string belongYear)
        {
            var chartDt = this._getChartDt(belongYear);
            var result = new Dictionary<string, object>();

            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "金额");
            y1.Lable.SetValue("format", "{value}元");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "成本率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();

            var ReceiptValueSer = new Series { Name = "已收款", Field = "ReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(ReceiptValueSer);

            var CanReceiptValueSer = new Series { Name = "经营应收款", Field = "CanReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            CanReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(CanReceiptValueSer);

            var costSer = new Series { Name = "实际成本", Field = "CostValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            costSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(costSer);

            var costRateSer = new Series { Name = "成本率%", Field = "CostRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            costRateSer.Tooltip.SetValue("valueSuffix", "%");
            serDefines.Add(costRateSer);

            string title = belongYear + "各部门费用概览";
            var chart = HighChartHelper.CreateColumnXYChart(title, "", chartDt, "text", yAxies, serDefines, null);
            result.SetValue("chart", chart);
            #endregion

            result["progress"] = _getProgressLis(belongYear);
            result["quality"] = _getQualityLis(belongYear);
            result["resource"] = _getResourceLis(belongYear);
            return Json(result);
        }

        private List<Dictionary<string, object>> _getProgressLis(string belongYear)
        {
            var result = _createDefaultList();
            var planState = ProjectCommoneState.Plan.ToString();
            var executeState = ProjectCommoneState.Execute.ToString();
            var finishState = ProjectCommoneState.Finish.ToString();
            //var projectList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.State == planState || d.State == executeState);
            //var delayProjectList = projectList.Where(d => d.S_P_MileStone.Count(c => c.FactFinishDate == null && d.PlanFinishDate < DateTime.Now) > 0);
            var currentYearFinishProject = this.entities.Set<S_I_ProjectInfo>().Where(d => d.State == finishState && d.FactFinishDate.HasValue
                && d.FactFinishDate.Value.Year == DateTime.Now.Year);

            var projectDt = this.SqlHelper.ExecuteDataTable(@"select isnull(DelayCount,0) as DelayCount,S_I_ProjectInfo.* from S_I_ProjectInfo
left join (select count(0) as DelayCount, ProjectInfoID from S_P_MileStone
where FactFinishDate is null
and PlanFinishDate<getdate() and PlanFinishDate is not null
group by ProjectInfoID) DelayProjectInfo
on S_I_ProjectInfo.ID=DelayProjectInfo.ProjectInfoID
where S_I_ProjectInfo.State = 'Plan' or S_I_ProjectInfo.State = 'Execute'");

            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var receiptDt = db.ExecuteDataTable(String.Format(@"select Sum(Amount) as DataValue,ProductUnit as ChargeDeptID,ProductUnitName as ChargeDeptName from S_C_Receipt 
where BelongYear='{0}' group by ProductUnit,ProductUnitName", DateTime.Now.Year));

            var contractDt = db.ExecuteDataTable(String.Format(@"select Sum(isnull(ContractRMBAmount,0)-isnull(SumReceiptValue,0)) as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
 from S_C_ManageContract where IsSigned='{0}' group by ProductionDept,ProductionDeptName ", "", DateTime.Now.Year));

            var canReceiptDt = db.ExecuteDataTable(String.Format(@"select isnull(Sum(isnull(ReceiptValue,0)-isnull(FactReceiptValue,0)),0)  as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
from S_C_ManageContract_ReceiptObj left join S_C_ManageContract on  S_C_ManageContract_ReceiptObj.S_C_ManageContractID=S_C_ManageContract.ID
where MileStoneState='{0}' group by ProductionDept,ProductionDeptName", true.ToString()));

            foreach (var item in result)
            {
                var deptID = item.GetValue("DeptID");
                item.SetValue("NormalCount", projectDt.Compute("Count(ID)", "ChargeDeptID='" + deptID + "' and DelayCount=0"));
                item.SetValue("DelayCount", projectDt.Compute("Count(ID)", "ChargeDeptID='" + deptID + "' and DelayCount>0"));
                item.SetValue("MonthFinishCount", currentYearFinishProject.Where(d => d.ChargeDeptID == deptID && d.FactFinishDate.Value.Month == DateTime.Now.Month).Count());
                item.SetValue("SeasonFinishCount", currentYearFinishProject.Where(d => d.ChargeDeptID == deptID &&
                   ((d.FactFinishDate.Value.Month - 1) / 3 + 1) == (DateTime.Now.Month - 1) / 3 + 1).Count());
                item.SetValue("YearFinishCount", currentYearFinishProject.Where(d => d.ChargeDeptID == deptID).Count());
                item.SetValue("CanReceiptValue", canReceiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + deptID + "'"));
                item.SetValue("ReceiptValue", receiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + deptID + "'"));
                item.SetValue("RemainReceiptValue", contractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + deptID + "'"));
            }
            return result;
        }

        private List<Dictionary<string, object>> _getQualityLis(string belongYear)
        {
            var result = _createDefaultList();
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            string sql = @"select count(0) as DataValue,DeptID as ChargerDeptID,DeptName as ChargerDeptName,MistakeLevel from S_AE_Mistake
where Year(CreateDate)={0}
group by DeptID,DeptName,MistakeLevel";
            var deptsIds = result.Select(d => d.GetValue("DeptID")).ToList();
            var baseDb = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var manUserDt = baseDb.ExecuteDataTable(String.Format(@"select count(0) as DataValue,DeptID from S_A_User where IsDeleted='0' and DeptID in ('{0}') group by DeptID", String.Join(",", deptsIds).Replace(",", "','")));

            var mistakeDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, DateTime.Now.Year));
            foreach (var item in result)
            {
                var deptID = item.GetValue("DeptID");
                var mistakeSum = 0m;
                var obj = manUserDt.Compute("Sum(DataValue)", "DeptID='" + deptID + "'");
                var userCount = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                foreach (var mistakeLevel in mistakeLevels)
                {
                    obj = mistakeDt.Compute("Sum(DataValue)", " ChargerDeptID='" + deptID + "' and MistakeLevel='" + mistakeLevel.Code + "'");
                    var dataValue = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                    item.SetValue(mistakeLevel.Code, dataValue);
                    mistakeSum += dataValue;
                }
                var avg = 0m;
                if (userCount > 0)
                    avg = Math.Round(Convert.ToDecimal(mistakeSum / userCount), 2);
                item.SetValue("Summary", mistakeSum);
                item.SetValue("Avg", avg);
            }
            return result;
        }

        private List<Dictionary<string, object>> _getResourceLis(string belongYear)
        {
            var result = _createDefaultList();
            var baseDb = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var userDt = baseDb.ExecuteDataTable("select count(0) as DataValue,DeptID as ChargerDeptID from S_A_User where IsDeleted='0' group by DeptID");
            string sql = "select Sum(Amount) as DataValue,ProductUnit as ChargerDeptID,ProductUnitName as ChargerDeptName from S_C_Receipt where BelongYear='{0}' group by ProductUnit,ProductUnitName";
            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var receiptDt = marketDB.ExecuteDataTable(String.Format(sql, DateTime.Now.Year));
            var projectDt = this.SqlHelper.ExecuteDataTable("select count(0) as DataValue,ChargeDeptID as ChargerDeptID from S_I_ProjectInfo where State in ('Plan','Execute') group by ChargeDeptID");

            var workHourDt = SQLHelper.CreateSqlHelper(ConnEnum.HR).ExecuteDataTable(String.Format(@"select Sum(WorkHourValue) as DataValue,UserDeptID as ChargerDeptID,UserDeptName as ChargerDeptName,
BelongYear,BelongMonth,BelongQuarter from dbo.S_W_UserWorkHour where BelongYear='{0}' and WorkHourType='{1}'
group by UserDeptID,UserDeptName,BelongYear,BelongMonth,BelongQuarter", DateTime.Now.Year, "Production"));


            foreach (var item in result)
            {
                DateTime dt = DateTime.Now;  //当前时间  
                var monthFirstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var monthStandardWork = this.getStandardWorkHour(monthFirstDate, monthFirstDate.AddDays(1).AddDays(-1));

                DateTime startQuarter = dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day);
                DateTime endQuarter = startQuarter.AddMonths(3).AddDays(-1);
                var seasonStandardWork = this.getStandardWorkHour(startQuarter, endQuarter);

                DateTime startYear = new DateTime(dt.Year, 1, 1);
                DateTime endYear = new DateTime(dt.Year, 12, 31);
                var yearStandardWork = this.getStandardWorkHour(startYear, endYear);

                var obj = receiptDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "'");
                var receiptValue = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                obj = userDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "'");
                var userCount = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                obj = projectDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "'");
                var prjCount = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);

                obj = workHourDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "' and BelongMonth='" + DateTime.Now.Month + "'");
                var monthWorkhour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);

                obj = workHourDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "' and BelongQuarter='" + (DateTime.Now.Month - 1) / 3 + 1 + "'");
                var seasonWorkhour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);


                obj = workHourDt.Compute("Sum(DataValue)", "ChargerDeptID='" + item.GetValue("DeptID") + "' ");
                var yearWorkhour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);


                var userProjectCount = userCount == 0 ? 0m : Math.Round(prjCount / userCount, 2);
                var userReceiptValue = userCount == 0 ? 0m : Math.Round(receiptValue / userCount / 10000, 2);

                var monthScale = monthStandardWork == 0 ? 0m : Math.Round(monthWorkhour / monthStandardWork, 4);
                var seasonScale = seasonStandardWork == 0 ? 0m : Math.Round(seasonWorkhour / seasonStandardWork, 4);
                var yearScale = yearStandardWork == 0 ? 0m : Math.Round(yearWorkhour / yearStandardWork, 4);

                item.SetValue("UserCount", userCount);
                item.SetValue("UserProjectCount", userProjectCount);
                item.SetValue("UserReceiptValue", userReceiptValue);
                item.SetValue("MonthStandardWorkHour", monthStandardWork);
                item.SetValue("MonthScale", monthScale);
                item.SetValue("SeasonStandardWorkHour", seasonStandardWork);
                item.SetValue("SeasonScale", seasonScale);
                item.SetValue("YearStandardWorkHour", yearStandardWork);
                item.SetValue("YearScale", yearScale);
            }
            return result;
        }

        private DataTable _getChartDt(string belongYear)
        {
            string sql = @"select * from (select ID as ChargerDept,Name as ChargerDeptName,SortIndex,
0 as ReceiptValue,0 as RemainContractValue,0 as CostValue,0 as CanReceiptValue,0 as RemainValue,
0 as CostRate from S_A_Org where  Type='{0}' ) TableInfo ";

            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            dt.Columns.Add("ReceiptValue", typeof(decimal));
            dt.Columns.Add("RemainContractValue", typeof(decimal));
            dt.Columns.Add("CostValue", typeof(decimal));
            dt.Columns.Add("CanReceiptValue", typeof(decimal));
            dt.Columns.Add("RemainValue", typeof(decimal));
            dt.Columns.Add("CostRate", typeof(decimal));

            var marketDb = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            if (String.IsNullOrEmpty(belongYear))
                belongYear = DateTime.Now.Year.ToString();
            sql = @"select Sum(Amount) as DataValue,ProductUnit as ChargeDeptID,ProductUnitName as ChargeDeptName from S_C_Receipt  where BelongYear='{0}' group by ProductUnit,ProductUnitName ";
            var receiptDt = marketDb.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"select Sum(CostValue) as DataValue,ProjectDeptID as ChargeDeptID,ProjectDeptName as ChargeDeptName  from dbo.S_FC_CostInfo where BelongYear='{0}' group by ProjectDeptID,ProjectDeptName ";
            var costDt = marketDb.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"select isnull(Sum(isnull(ReceiptValue,0)-isnull(FactReceiptValue,0)),0)  as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName from S_C_ManageContract_ReceiptObj left join S_C_ManageContract
on S_C_ManageContract.ID =S_C_ManageContract_ReceiptObj.S_C_ManageContractID  where MileStoneState='{0}' group by ProductionDept,ProductionDeptName";
            var canReceiptDt = marketDb.ExecuteDataTable(String.Format(sql, true.ToString()));

            sql = @"select Sum(isnull(ContractRMBAmount,0)-isnull(SumReceiptValue,0)) as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
 from S_C_ManageContract where IsSigned='{0}' group by ProductionDept,ProductionDeptName";
            var remainContractDt = marketDb.ExecuteDataTable(String.Format(sql, ""));

            foreach (DataRow row in dt.Rows)
            {
                var obj = receiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var receiptValue = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ReceiptValue"] = receiptValue;

                obj = canReceiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");

                row["CanReceiptValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);

                obj = remainContractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["RemainContractValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);

                obj = costDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var costValue = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["CostValue"] = costValue;

                if (receiptValue > 0)
                {
                    row["CostRate"] = Math.Round(costValue / receiptValue * 100);
                }
                else if (costValue > 0)
                    row["CostRate"] = 100;
                else
                    row["CostRate"] = 0;
            }
            return dt;
        }

        private List<Dictionary<string, object>> _createDefaultList()
        {
            var list = new List<Dictionary<string, object>>();
            var depts = EnumBaseHelper.GetEnumDef("System.ManDept").EnumItem;
            foreach (var item in depts)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("DeptID", item.Code);
                dic.SetValue("DeptName", item.Name);
                list.Add(dic);
            }
            return list;
        }
        #endregion

        #region 部门概览

        public ActionResult MainPageDept()
        {
            string deptID = this.GetQueryString("DeptID");
            ViewBag.DeptID = deptID;
            string sql = @"select OrgID,OrgName,ContractValue,ReceiptValue from dbo.S_KPI_IndicatorOrg
where BelongYear='{0}' and OrgID='{1}'
and IndicatorType='YearIndicator'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, DateTime.Now.Year, deptID));
            var contractKPI = 0m;
            var receiptKPI = 0m;
            var receiptValue = 0m;
            var contractValue = 0m;
            if (dt.Rows.Count > 0)
            {
                contractKPI =
                    dt.Rows[0]["ContractValue"] == null || dt.Rows[0]["ContractValue"] == DBNull.Value ? 0 :
                    Math.Round(Convert.ToDecimal(dt.Rows[0]["ContractValue"]) / 10000);
                receiptKPI = dt.Rows[0]["ReceiptValue"] == null || dt.Rows[0]["ReceiptValue"] == DBNull.Value
                    ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[0]["ReceiptValue"]) / 10000);
            }
            ViewBag.ContractKPI = contractKPI;
            ViewBag.ReceiptKPI = receiptKPI;
            sql = "select isnull(Sum(Amount),0) as DataValue from S_C_Receipt where BelongYear='{0}' and ProductUnit='{1}'";
            receiptValue = Math.Round(Convert.ToDecimal(db.ExecuteScalar(String.Format(sql, DateTime.Now.Year, deptID))) / 10000);
            sql = "select isnull(Sum(ContractRMBAmount),0) as DataValue from S_C_ManageContract where BelongYear='{0}' and ProductionDept='{1}'";
            contractValue = Math.Round(Convert.ToDecimal(db.ExecuteScalar(String.Format(sql, DateTime.Now.Year, deptID))) / 10000);

            ViewBag.ContractValue = contractValue;
            ViewBag.ReceiptValue = receiptValue;
            ViewBag.ReceiptScale = receiptKPI == 0 ? 0m : Math.Round(receiptValue / receiptKPI * 100);
            ViewBag.ContractScale = contractKPI == 0 ? 0m : Math.Round(contractValue / contractKPI * 100);
            return View();
        }

        public ActionResult OverViewDept()
        {

            return View();
        }

        #region 费用与成本分析
        public JsonResult GetCostInfo(string belongYear, string DeptID, string AnalysisType)
        {
            var result = new Dictionary<string, object>();
            if (String.IsNullOrEmpty(belongYear)) belongYear = DateTime.Now.Year.ToString();

            var costDt = createCostDt(belongYear, DeptID, AnalysisType);
            var orgService = FormulaHelper.GetService<IOrgService>();
            var dept = orgService.GetOrgs(DeptID).FirstOrDefault(d => d.ID == DeptID);
            if (dept == null) throw new Formula.Exceptions.BusinessException("");

            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "金额");
            y1.Lable.SetValue("format", "{value}万元");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "成本率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();

            var ReceiptValueSer = new Series { Name = "收款", Field = "ReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptValueSer.Tooltip.SetValue("valueSuffix", "万元");
            serDefines.Add(ReceiptValueSer);

            var CanReceiptValueSer = new Series { Name = "成本", Field = "CostValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            CanReceiptValueSer.Tooltip.SetValue("valueSuffix", "万元");
            serDefines.Add(CanReceiptValueSer);

            var costRateSer = new Series { Name = "成本率%", Field = "CostRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            costRateSer.Tooltip.SetValue("valueSuffix", "%");
            serDefines.Add(costRateSer);

            var chartDt = createCostChartDt(belongYear, DeptID, AnalysisType);
            string title = dept.Name + "费用概览";
            var chart = HighChartHelper.CreateColumnXYChart(title, "", chartDt, "Month", yAxies, serDefines, null);
            result.SetValue("costChart", chart);

            #endregion

            result.SetValue("data", costDt);
            return Json(result);
        }

        private DataTable createCostChartDt(string belongYear, string DeptID, string AnalysisType)
        {
            string sql = @"select Sum(Amount) as DataValue,ProductUnit as ChargerDeptID,
ProductUnitName as ChargerDeptName,BelongYear,BelongMonth from S_C_Receipt
where ProductUnit='{0}' and BelongYear={1} group by ProductUnit,ProductUnitName,BelongYear,BelongMonth";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var receiptDt = db.ExecuteDataTable(String.Format(sql, DeptID, belongYear));

            sql = @"select Sum(CostValue) as DataValue,ProjectDeptID as ChargerDeptID,
ProjectDeptName as ChargerDeptName,BelongYear,BelongMonth from S_FC_CostInfo
where ProjectDeptID='{0}' and BelongYear={1} group by ProjectDeptID,ProjectDeptName,BelongYear,BelongMonth";
            var costDt = db.ExecuteDataTable(String.Format(sql, DeptID, belongYear));

            var dt = new DataTable();
            dt.Columns.Add("Month");
            dt.Columns.Add("ReceiptValue", typeof(decimal));
            dt.Columns.Add("CostValue", typeof(decimal));
            dt.Columns.Add("CostRate", typeof(decimal));

            for (int i = 1; i <= 12; i++)
            {
                var row = dt.NewRow();
                row["Month"] = i + "月";
                var receiptValue = 0m;
                var costValue = 0m;
                var scale = 0m;

                if (i > DateTime.Now.Month)
                {
                    row["ReceiptValue"] = DBNull.Value;
                    row["CostValue"] = DBNull.Value;
                    row["CostRate"] = DBNull.Value;
                    dt.Rows.Add(row); continue;
                }

                if (AnalysisType == "Total")
                {
                    var obj = receiptDt.Compute("Sum(DataValue)", "BelongMonth<=" + i + "");
                    receiptValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                    obj = costDt.Compute("Sum(DataValue)", "BelongMonth<=" + i + "");
                    costValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                }
                else
                {
                    var obj = receiptDt.Compute("Sum(DataValue)", "BelongMonth='" + i + "'");
                    receiptValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                    obj = costDt.Compute("Sum(DataValue)", "BelongMonth='" + i + "'");
                    costValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                }

                if (receiptValue == 0)
                {
                    if (costValue > 0)
                        scale = 100;
                }
                else
                {
                    scale = Math.Round(costValue / receiptValue * 100, 2);
                }
                row["ReceiptValue"] = receiptValue;
                row["CostValue"] = costValue;
                row["CostRate"] = scale;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable createCostDt(string belongYear, string DeptID, string AnalysisType)
        {
            string sql = @"select Sum(Amount) as DataValue,ProductUnit as ChargerDeptID,
ProductUnitName as ChargerDeptName,BelongYear,BelongMonth from S_C_Receipt
where ProductUnit='{0}' and BelongYear={1} group by ProductUnit,ProductUnitName,BelongYear,BelongMonth";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var receiptDt = db.ExecuteDataTable(String.Format(sql, DeptID, belongYear));

            sql = @"select Sum(CostValue) as DataValue,ProjectDeptID as ChargerDeptID,
ProjectDeptName as ChargerDeptName,BelongYear,BelongMonth from S_FC_CostInfo
where ProjectDeptID='{0}' and BelongYear={1} group by ProjectDeptID,ProjectDeptName,BelongYear,BelongMonth";
            var costDt = db.ExecuteDataTable(String.Format(sql, DeptID, belongYear));

            var dt = _createDt();
            var costRow = dt.NewRow();
            var receiptRow = dt.NewRow();
            var scaleRow = dt.NewRow();
            costRow["DataType"] = "成本（万元）";
            receiptRow["DataType"] = "收款（万元）";
            scaleRow["DataType"] = "成本率";

            var sumReceiptValue = 0m;
            var sumCostValue = 0m;

            for (int i = 1; i <= 12; i++)
            {
                var field = i + "_Month";
                var costValue = 0m;
                var receiptValue = 0m;
                var scale = 0m;
                if (i > DateTime.Now.Month)
                {
                    costRow[field] = DBNull.Value;
                    receiptRow[field] = DBNull.Value;
                    scaleRow[field] = DBNull.Value;
                }
                else
                {
                    if (AnalysisType == "Total")
                    {
                        var obj = receiptDt.Compute("sum(DataValue)", "BelongMonth<=" + i + "");
                        receiptValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        obj = costDt.Compute("sum(DataValue)", "BelongMonth<=" + i + "");
                        costValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        sumReceiptValue = receiptValue;
                        sumCostValue = costValue;
                    }
                    else
                    {
                        var obj = receiptDt.Compute("sum(DataValue)", "BelongMonth='" + i + "'");
                        receiptValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        obj = costDt.Compute("sum(DataValue)", "BelongMonth='" + i + "'");
                        costValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        sumReceiptValue += receiptValue;
                        sumCostValue += costValue;
                    }
                    costRow[field] = costValue;
                    receiptRow[field] = receiptValue;
                    if (receiptValue == 0)
                    {
                        if (costValue > 0)
                            scale = 100;
                    }
                    else
                    {
                        scale = Math.Round(costValue / receiptValue * 100, 2);
                    }
                    scaleRow[field] = scale;
                }
            }
            receiptRow["Total"] = sumReceiptValue;
            costRow["Total"] = sumCostValue;
            var sumScale = 0m;
            if (sumReceiptValue > 0)
            {
                sumScale = Math.Round(sumCostValue / sumReceiptValue * 100, 2);
            }
            else if (sumCostValue > 0)
            {
                sumScale = 100;
            }
            scaleRow["Total"] = sumScale;
            dt.Rows.Add(receiptRow);
            dt.Rows.Add(costRow);
            dt.Rows.Add(scaleRow);
            return dt;

        }

        private DataTable _createDt()
        {
            var result = new DataTable();
            result.Columns.Add("DataType");
            for (int i = 1; i <= 12; i++)
            {
                var field = i + "_Month";
                result.Columns.Add(field, typeof(decimal));
            }
            result.Columns.Add("Total", typeof(decimal));
            return result;
        }
        #endregion

        #region 合同情况分析
        public JsonResult GetContractInfo(string lastYear, string DeptID, string AnalysisType)
        {
            var lastYearNo = String.IsNullOrEmpty(lastYear) ? 3 : Convert.ToInt32(lastYear);
            var startYear = DateTime.Now.Year - lastYearNo + 1;
            var endYear = DateTime.Now.Year;
            string sql = @"select Sum({1}) as Value,BelongYear,BelongMonth from {0}
where BelongYear>='" + startYear + "' and BelongYear<='" + endYear + "' {2} group by BelongYear,BelongMonth";
            sql = String.Format(sql, "S_C_ManageContract", "ContractRMBAmount", " and IsSigned='Signed' and ProductionDept='" + DeptID + "'");
            var dt = _CreateContractTable(startYear, endYear);
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var data = db.ExecuteDataTable(sql);
            foreach (DataRow item in dt.Rows)
            {
                var year = Convert.ToInt32(item["Year"].ToString());
                var total = 0M;
                for (int i = 1; i <= 12; i++)
                {
                    if (year == DateTime.Now.Year && i > DateTime.Now.Month)
                    {
                        item[i + "Month"] = DBNull.Value; continue;
                    }
                    var contractValue = 0m;
                    if (AnalysisType == "Total")
                    {
                        var obj = data.Compute("Sum(Value)", "BelongYear='" + year + "' and BelongMonth<=" + i);
                        contractValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        total = contractValue;
                    }
                    else
                    {
                        var obj = data.Compute("Sum(Value)", "BelongYear='" + year + "' and BelongMonth=" + i);
                        contractValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                        total += contractValue;
                    }
                    item[i + "Month"] = contractValue;
                }
                item["Total"] = total;
            }

            #region 计算同比增长
            var rateRow = dt.NewRow();
            rateRow["Year"] = "同比增长率";
            var currentYearRow = dt.Select("Year='" + DateTime.Now.Year + "'").FirstOrDefault();
            var lastYearRow = dt.Select("Year='" + (DateTime.Now.Year - 1) + "'").FirstOrDefault();
            for (int i = 1; i <= 12; i++)
            {
                if (i > DateTime.Now.Month)
                {
                    rateRow[i + "Month"] = DBNull.Value; continue;
                }
                var currentValue = currentYearRow[i + "Month"] == DBNull.Value ? 0 : Convert.ToDecimal(currentYearRow[i + "Month"]);
                var lastValue = lastYearRow[i + "Month"] == DBNull.Value ? 0 : Convert.ToDecimal(lastYearRow[i + "Month"]);
                if (currentValue == 0 && lastValue > 0)
                {
                    rateRow[i + "Month"] = -100;
                }
                else if (lastValue == 0 && currentValue > 0)
                {
                    rateRow[i + "Month"] = 100;
                }
                else if (lastValue == 0 && currentValue == 0)
                {
                    rateRow[i + "Month"] = 0;
                }
                else
                {
                    rateRow[i + "Month"] = Math.Round((currentValue - lastValue) / lastValue * 100, 2);
                }
            }

            dt.Rows.Add(rateRow);
            #endregion

            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            var chart = _CreateChartTable(lastYearNo, DeptID, AnalysisType);
            result.SetValue("Chart", chart);
            return Json(result);
        }

        private Dictionary<string, object> _CreateChartTable(int lastYear, string DeptID, string AnalysisType)
        {
            string sql = "select Sum(ContractRMBAmount) as Value,BelongYear,BelongMonth from S_C_ManageContract where IsSigned='Signed' and BelongYear>={0} and BelongYear<={1} and ProductionDept='{2}' group by BelongYear,BelongMonth";
            var startYear = DateTime.Now.Year - lastYear + 1;
            var endYear = DateTime.Now.Year;
            string series = string.Empty;
            string serieFields = string.Empty;
            var dataSource = new DataTable();
            dataSource.Columns.Add("Month", typeof(string));
            for (int i = startYear; i <= endYear; i++)
            {
                series += i.ToString() + "年,";
                serieFields += i.ToString() + ",";
                dataSource.Columns.Add(i.ToString(), typeof(decimal));
            }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, startYear, endYear, DeptID));
            for (int i = 1; i <= 12; i++)
            {
                var row = dataSource.NewRow();
                row["Month"] = i + "月";
                for (int j = startYear; j <= endYear; j++)
                {
                    var contractValue = 0m;
                    if (j == DateTime.Now.Year && i > DateTime.Now.Month)
                    {
                        row[j.ToString()] = DBNull.Value; continue;
                    }
                    if (AnalysisType == "Total")
                    {
                        var obj = dt.Compute("Sum(Value)", "BelongYear='" + j + "' and BelongMonth<=" + i);
                        contractValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                    }
                    else
                    {
                        var obj = dt.Compute("Sum(Value)", "BelongYear='" + j + "' and BelongMonth=" + i);
                        contractValue = obj == null || obj == DBNull.Value ? 0m : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
                    }
                    row[j.ToString()] = contractValue;
                }
                dataSource.Rows.Add(row);
            }
            series = series.TrimEnd(',');
            serieFields = serieFields.TrimEnd(',');
            var columChart = HighChartHelper.CreateColumnChart("", dataSource, "Month", series.Split(','), serieFields.Split(','));
            return columChart.Render();
        }

        private DataTable _CreateContractTable(int startYear, int endYear)
        {
            var dt = new DataTable();
            dt.Columns.Add("Year");
            dt.Columns.Add("1Month", typeof(decimal));
            dt.Columns.Add("2Month", typeof(decimal));
            dt.Columns.Add("3Month", typeof(decimal));
            dt.Columns.Add("4Month", typeof(decimal));
            dt.Columns.Add("5Month", typeof(decimal));
            dt.Columns.Add("6Month", typeof(decimal));
            dt.Columns.Add("7Month", typeof(decimal));
            dt.Columns.Add("8Month", typeof(decimal));
            dt.Columns.Add("9Month", typeof(decimal));
            dt.Columns.Add("10Month", typeof(decimal));
            dt.Columns.Add("11Month", typeof(decimal));
            dt.Columns.Add("12Month", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            for (int i = startYear; i <= endYear; i++)
            {
                var row = dt.NewRow();
                row["Year"] = i.ToString();
                row["1Month"] = 0;
                row["2Month"] = 0;
                row["3Month"] = 0;
                row["4Month"] = 0;
                row["5Month"] = 0;
                row["6Month"] = 0;
                row["7Month"] = 0;
                row["8Month"] = 0;
                row["9Month"] = 0;
                row["10Month"] = 0;
                row["11Month"] = 0;
                row["12Month"] = 0;
                row["Total"] = 0;
                dt.Rows.Add(row);
            }
            return dt;
        }

        #endregion

        #region 项目分析

        public ActionResult DetailDeptProject()
        {
            string deptID = this.GetQueryString("DeptID");
            ViewBag.DeptID = deptID;
            string sql = @"select count(0) as DataValue,State,ChargeDeptID,ChargeDeptName from S_I_ProjectInfo
where ChargeDeptID='{0}' group by State,ChargeDeptID,ChargeDeptName";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, deptID));
            var obj = dt.Compute("Sum(DataValue)", "State in ('Create','Plan','Execute')");
            var executeCount = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);

            obj = dt.Compute("Sum(DataValue)", "State in ('Finish')");
            var finishCount = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
            obj = dt.Compute("Sum(DataValue)", "State in ('Terminate','Pause')");
            var pauseCount = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);

            ViewBag.ExeCount = executeCount;
            ViewBag.FinishCount = finishCount;
            ViewBag.PauseCount = pauseCount;
            return View();
        }

        public JsonResult GetProjectDetailList(QueryBuilder qb, string ChargerDept, string State)
        {
            string sql = @"select S_I_Project.ID,S_I_Project.Name,S_I_Project.Code,S_I_Project.CreateDate,S_I_Project.State,CompletePercent,
S_I_Project.ChargerDept,
Round(isnull(ProjectContractValue,0)/10000,2) as ProjectContractValue,
Round(isnull(ProjectReceiptValue,0)/10000,2) as ProjectReceiptValue,
Round(isnull(PaymentCost,0)/10000,2) as Payment,
Round(isnull(Production,0)/10000,2) as Production,
Round(isnull(UserCost,0)/10000,2) as UserCost,
Round(isnull(SumProjectCostValue,0)/10000,2) as SumProjectCostValue,
Round((isnull(ProjectReceiptValue,0) - isnull(SumProjectCostValue,0))/10000,2) as Profit,
isnull(CurrentMileStoneName,'-') as CurrentMileStoneName,
isnull(MistakeCount,0) as MistakeCount,
case when isnull(ProjectReceiptValue,0)=0 then 0 else 
Round( (isnull(ProjectReceiptValue,0) - isnull(SumProjectCostValue,0))/isnull(ProjectReceiptValue,0),4) 
end as ProfitRate,
case when isnull(ProjectContractValue,0)=0 then 0 else 
Round(isnull(ProjectReceiptValue,0)/isnull(ProjectContractValue,0),4) 
end as ReceiptScale,S_I_ProjectInfo.ID as ProjectInfoID 
from S_I_Project
left join (select Sum(ProjectValue) as ProjectContractValue,ProjectID
from S_C_ManageContract_ProjectRelation 
where ProjectID is not null group by ProjectID ) ProjectContract 
on S_I_Project.ID=ProjectContract.ProjectID
left join (select 
Sum(FactReceiptValue) as ProjectReceiptValue,
Sum(FactInvoiceValue) as ProjectInvoiceValue,
Sum(FactBadValue) as ProjectBadDebtValue,ProjectInfo from S_C_ManageContract_ReceiptObj 
where ProjectInfo is not null group by ProjectInfo) ProjectValueInfo 
on S_I_Project.ID=ProjectValueInfo.ProjectInfo
left join (
select ProjectID,
isnull(Max(Payment),0) as PaymentCost,
isnull(Max(Production),0) as Production,
isnull(Max(UserCost),0) as UserCost,
isnull(Max(Payment),0)+isnull(Max(Production),0)+isnull(Max(UserCost),0) as SumProjectCostValue
from (select Sum(CostValue) as SumCostValue,CostType,ProjectID from s_fc_costinfo
where ProjectID is not null and ProjectID !=''
group by ProjectID,CostType) as CostInfo
pivot(avg(SumCostValue) for CostType in (Payment,Production,UserCost)
) as ProjectCostInfo
group by ProjectID) ProjectCostTable
on S_I_Project.ID = ProjectCostTable.ProjectID
left join {0}..S_I_ProjectInfo on S_I_Project.ID=S_I_ProjectInfo.MarketProjectInfoID
left join (
select count(0) as MistakeCount,ProjectInfoID from {0}..S_AE_Mistake
group by ProjectInfoID) MistakeInfo on S_I_ProjectInfo.ID=MistakeInfo.ProjectInfoID
left join (
select Name as CurrentMileStoneName,S_P_MileStone.ProjectInfoID from {0}..S_P_MileStone
inner join (
select Min(ID) as MinMileStoneID,ProjectInfoID from {0}..S_P_MileStone
where MileStoneType='Normal' and FactFinishDate is null 
group by ProjectInfoID) MinMileStone
on MinMileStone.MinMileStoneID = S_P_MileStone.ID) MilestoneInfo
on MilestoneInfo.ProjectInfoID = S_I_ProjectInfo.ID
";
            qb.Add("ChargerDept", QueryMethod.Equal, ChargerDept);
            qb.Add("State", QueryMethod.In, State);

            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            sql = String.Format(sql, this.SqlHelper.DbName);
            qb.PageSize = 0;
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);

        }
        #endregion

        #region 资源分析

        public ActionResult DetailDeptResource()
        {
            string deptID = this.GetQueryString("DeptID");
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select count(1) from S_A_User
where DeptID='{0}' and IsDeleted='0'";
            var obj = baseDB.ExecuteScalar(String.Format(sql, deptID));
            ViewBag.UserCount = Convert.ToInt32(obj);
            return View();
        }

        public JsonResult GetResourceDetailList(string belongYear, string DeptID)
        {
            if (String.IsNullOrEmpty(belongYear))
                belongYear = DateTime.Now.Year.ToString();
            var HrDB = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = baseDB.ExecuteDataTable(@"select ID,Name as UserName,WorkNo,0.00 as LastMonthStandard,0.00 as CurrentMonthStandard,0.00 as SeasonStandard,
0.00 as YearStandard,0.00as LastMonth,0.00 as CurrentMonth, 0.00 as Seaon,0.00 as Year,
0 as [Plan],0 as [Execute],0 as ExecuteTotal,0 as Finish,0 as Pause,0 as Terminate,0 as Total from S_A_User where DeptID='" + DeptID + "'  and IsDeleted='0'");
            DateTime dt = DateTime.Now;
            DateTime startQuarter = dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day);  //本季度初  
            DateTime endQuarter = startQuarter.AddMonths(3).AddDays(-1);  //本季度末  
            var seasonStandard = getStandardWorkHour(startQuarter, endQuarter) ;
            var lastMonthStandard = getStandardWorkHour(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1),
              DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1))  ;
            var monthStandard = getStandardWorkHour(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")),
                DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1)) ;
            var yearStandard = getStandardWorkHour(DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")),
                DateTime.Parse(DateTime.Now.ToString("yyyy-12-31"))) ;

            var yearDt = HrDB.ExecuteDataTable(String.Format(@"select UserID,Sum(WorkHourValue) as WorkHourValue,
case when WorkHourType='Production' then 'Production' else 'Other' end as DeptWorkHourType
from S_W_UserWorkHour where BelongYear={0} and UserDeptID='{1}'
group by UserID,WorkHourType", belongYear, DeptID));

            var seasonDt = HrDB.ExecuteDataTable(String.Format(@"select UserID,Sum(WorkHourValue) as WorkHourValue,
case when WorkHourType='Production' then 'Production' else 'Other' end as DeptWorkHourType
from S_W_UserWorkHour
where BelongYear={0} and BelongQuarter={1} and UserDeptID='{2}'
group by UserID,WorkHourType", belongYear, (dt.Month - 1) / 3 + 1, DeptID));

            var monthDt = HrDB.ExecuteDataTable(String.Format(@"select UserID,Sum(WorkHourValue) as WorkHourValue,
case when WorkHourType='Production' then 'Production' else 'Other' end as DeptWorkHourType
from S_W_UserWorkHour
where WorkHourDate>='{0}' and WorkHourDate<='{1}' and UserDeptID='{2}'
group by UserID,WorkHourType", DateTime.Now.ToString("yyyy-MM-01"),
                             DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToShortDateString()
                             , DeptID));

            var lastMonthDt = HrDB.ExecuteDataTable(String.Format(@"select UserID,Sum(WorkHourValue) as WorkHourValue,
case when WorkHourType='Production' then 'Production' else 'Other' end as DeptWorkHourType
from S_W_UserWorkHour
where WorkHourDate>='{0}' and WorkHourDate<='{1}' and UserDeptID='{2}'
group by UserID,WorkHourType", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1).ToShortDateString(),
                              DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToShortDateString()
                             , DeptID));

            var projectDt = this.SqlHelper.ExecuteDataTable(String.Format(@"select * from (
select distinct UserID,DeptID,DeptName,ProjectInfoID from S_W_OBSUser
where DeptID='{0}') OBSUser
left join S_I_ProjectInfo on OBSUser.ProjectInfoID=S_I_ProjectInfo.ID", DeptID));

            foreach (DataRow item in data.Rows)
            {
                var obj = lastMonthDt.Compute("Sum(WorkHourValue)", "UserID='" + item["ID"] + "'");
                var lastMonthWorkHour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                item["LastMonth"] = lastMonthStandard == 0 ? 0 : Math.Round(lastMonthWorkHour / lastMonthStandard, 4);

                obj = monthDt.Compute("Sum(WorkHourValue)", "UserID='" + item["ID"] + "'");
                var currentMonthWorkHour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                item["CurrentMonth"] = monthStandard == 0 ? 0 : Math.Round(currentMonthWorkHour / monthStandard, 4);

                obj = seasonDt.Compute("Sum(WorkHourValue)", "UserID='" + item["ID"] + "'");
                var seaonWorkHour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                item["Seaon"] = seasonStandard == 0 ? 0 : Math.Round(seaonWorkHour / seasonStandard, 4);

                obj = yearDt.Compute("Sum(WorkHourValue)", "UserID='" + item["ID"] + "'");
                var yearWorkHour = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                item["Year"] = yearStandard == 0 ? 0 : Math.Round(yearWorkHour / yearStandard, 4);

                item["Plan"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State='" + ProjectCommoneState.Plan.ToString() + "'"));
                item["Execute"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State='" + ProjectCommoneState.Execute.ToString() + "'"));
                item["ExecuteTotal"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State in ('" + ProjectCommoneState.Execute.ToString() + "','" + ProjectCommoneState.Plan.ToString() + "')"));
                item["Finish"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State='" + ProjectCommoneState.Finish.ToString() + "'"));
                item["Pause"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State='" + ProjectCommoneState.Pause.ToString() + "'"));
                item["Terminate"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "' AND State='" + ProjectCommoneState.Terminate.ToString() + "'"));
                item["Total"] = Convert.ToDecimal(projectDt.Compute("count(ID)", "UserID='" + item["ID"] + "'"));
            }

            return Json(data);
        }

        #endregion

        #endregion

        #region 计算标准工时
        string workHourType = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkHourType"]) ? "Hour" :
          System.Configuration.ConfigurationManager.AppSettings["WorkHourType"];

        decimal NormalHoursMax = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]) ? 8 :
           Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);

        decimal maxExtraHour = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]) ? 0 :
               Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]);
        decimal getStandardWorkHour(DateTime startDate, DateTime endDate)
        {
            var baseEntities = FormulaHelper.GetEntities<Base.Logic.Domain.BaseEntities>();
            var holidayConfig = baseEntities.Set<S_C_Holiday>().Where(d => d.Year >= startDate.Year && d.Year <= endDate.Year).ToList();
            TimeSpan sp = endDate.Subtract(startDate);
            var day = Convert.ToDecimal(sp.Days) + 1;
            var holiday = 0;
            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                bool isholiday = false;
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                    isholiday = true;
                var config = holidayConfig.FirstOrDefault(d => d.Date == i);
                if (config != null)
                {
                    if (config.IsHoliday == "0")
                        isholiday = false;
                    else
                        isholiday = true;
                }
                if (isholiday)
                    holiday++;
            }
            if (workHourType == "Hour")
            {
                day = (day - holiday) * NormalHoursMax;
            }
            else
            {
                day = (day - holiday);
            }
            return day;
        }
        #endregion
    }
}
