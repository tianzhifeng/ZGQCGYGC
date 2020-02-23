using Config;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;
using System.Data;
using Formula.Helper;
using Base.Logic.Domain;
using OfficeAuto.Logic;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class ReimbursementFormController : OfficeAutoFormContorllor<S_EP_ReimbursementApply>
    {

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (isNew == true)
            {
                var row = dt.Rows[0];
                var userID = row["Advertiser"];
                var reimbursementEffectiveDateStart = row["ReimbursementEffectiveDateStart"];
                if (reimbursementEffectiveDateStart == null)
                    reimbursementEffectiveDateStart = DateTime.Now.AddYears(-2);
                var reimbursementEffectiveDateEnd = row["ReimbursementEffectiveDateEnd"];
                var sql = @"select rf.ReimbursementFormCode,rf.Advertiser,rf.AdvertiserName,GNFYJTF.StartingDate,GNFYJTF.ArrivedDate from S_EP_ReimbursementApply_GNFYJTF GNFYJTF
left join S_EP_ReimbursementApply rf on rf.ID = GNFYJTF.S_EP_ReimbursementApplyID
where rf.Advertiser = '{0}'  and GNFYJTF.StartingDate>='{1}' and GNFYJTF.StartingDate<='{2}'";
                sql = string.Format(sql, userID, reimbursementEffectiveDateStart, reimbursementEffectiveDateEnd);
                var BXRYBXCLFRQdt = this.SQLDB.ExecuteDataTable(sql);
                dt.Rows[0]["BXRYBXCLFRQ"] = JsonHelper.ToJson(BXRYBXCLFRQdt);

            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            var isValidate = GetQueryString("IsValidate");
            if (string.IsNullOrWhiteSpace(isValidate) || isValidate.ToLower() != "false")
            {
                //非正式员工报销单需处理的业务逻辑
                if (dic.ContainsKey("Details"))
                {
                    var strDetailsInfo = dic.GetValue("Details");
                    if (!string.IsNullOrWhiteSpace(strDetailsInfo))
                    {
                        var detailsList = JsonHelper.ToObject<List<Dictionary<string, string>>>(strDetailsInfo);
                        var idCrads = detailsList.Select(c => c.GetValue("IDCard"));
                        if (idCrads != null && idCrads.Count() > 0)
                            dic = UpdateLaborInfo(dic);
                    }
                }
                Validate(dic, formInfo, isNew);

            }
        }

        protected override void OnFlowEnd(S_EP_ReimbursementApply entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            entity.Push();
            this.BusinessEntities.SaveChanges();
        }

        public JsonResult GetUserInfoAndBXRYBXCLFRQ(string userID, string reimbursementEffectiveDateStart, string reimbursementEffectiveDateEnd, string formID)
        {
            if (!string.IsNullOrWhiteSpace(userID))
                userID = userID.Replace(",", "','");

            if (!string.IsNullOrWhiteSpace(reimbursementEffectiveDateStart))
                reimbursementEffectiveDateStart = reimbursementEffectiveDateStart.Replace("\"", "");

            if (!string.IsNullOrWhiteSpace(reimbursementEffectiveDateEnd))
                reimbursementEffectiveDateEnd = reimbursementEffectiveDateEnd.Replace("\"", "");

            var returnDic = new Dictionary<string, object>();
            var HRSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var BaseSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var userSql = @"SELECT e.UserID  as ID,e.[Code] ,e.[Name],e.[Sex],e.[MobilePhone],e.[OfficePhone],e.[HomePhone],e.[Email],e.[Address],e.[Birthday],e.[IdentityCardCode],e.[JoinWorkDate],e.[JoinCompanyDate],e.[EmploymentWay],e.[EmployeeSource],e.[EmployeeBigType]
,e.[EmployeeSmallType],e.[Post],e.[PostLevel],e.[PositionalTitles],e.[Educational],e.[EngageMajor],e.[EducationalMajor]
,e.[DeptID],e.[DeptName],e.[EmployeeState],e.[IsDeleted],e.[DeleteTime],e.[IsHaveAccount],e.[PostTemplateID],e.[Employeelevel] 
,o.ID as UserOrgID,o.Name as UserOrgID,o.[Type] as UserOrgType
, po.ID as UserParentOrgID,po.Name as   UserParentOrgName,po.Type as UserParentOrgType
FROM [T_Employee] e
left join {1}.dbo.S_A_User u on u.ID = e.UserID
left join {1}.dbo.S_A_Org o on o.ID = u.DeptID
left join {1}.dbo.S_A_Org po on po.ID = o.ParentID
 where UserID = '{0}'";
            userSql = string.Format(userSql, userID, BaseSQLHelper.DbName);
            var dt = HRSQLHelper.ExecuteDataTable(userSql);

            var sql = @"
select rf.ReimbursementFormCode,rf.Advertiser,rf.AdvertiserName,GNFYJTF.StartingDate,GNFYJTF.ArrivedDate 
from S_EP_ReimbursementApply_GNFYJTF GNFYJTF
left join S_EP_ReimbursementApply rf on rf.ID = GNFYJTF.S_EP_ReimbursementApplyID
where rf.Advertiser in ('{0}') and GNFYJTF.StartingDate>='{1}' and GNFYJTF.ArrivedDate<='{2}' and rf.ID <> '{3}'
union
select rf.ReimbursementFormCode,GWFYMX.TravelUser,GWFYMX.TravelUserName,GWFYMX.DepartureDate,GWFYMX.LeftDate 
from S_EP_ReimbursementApply_GWFYMX GWFYMX
left join S_EP_ReimbursementApply rf on rf.ID = GWFYMX.S_EP_ReimbursementApplyID
where rf.Advertiser in ('{0}') and GWFYMX.DepartureDate>='{1}' and GWFYMX.LeftDate<='{2}' and rf.ID <> '{3}'";
            sql = string.Format(sql, userID, reimbursementEffectiveDateStart, reimbursementEffectiveDateEnd, formID);
            var BXRYBXCLFRQdt = this.SQLDB.ExecuteDataTable(sql);
            returnDic.SetValue("UserInfo", Json(dt));
            returnDic.SetValue("BXRYBXCLFRQ", Json(BXRYBXCLFRQdt));

            //户名
            sql = @"select AccountName as value,AccountName as text,OpenBank,CardNumber from E_BDS_MyBankCardInfo where CreateUserID  in ('{0}') or CreateUserID in ('{0}') ";
            sql = string.Format(sql, userID);
            var accountNameDt = this.SQLDB.ExecuteDataTable(sql);
            returnDic.SetValue("AccountName", Json(accountNameDt));

            return Json(returnDic);
        }

        /// <summary>
        /// 获取党费或工会经费预算信息
        /// </summary>
        /// <param name="useBudgetYear">使用预算年份</param>
        /// <param name="costIncludedBranch">费用计入支部</param>
        /// <param name="budgetType">预算类型</param>
        /// <param name="reimbursementFormCode">表单编号</param>
        /// <param name="thisFormID">本次表单ID</param>
        /// <returns></returns>
        public JsonResult GetPartyOrUnionBudgetInfo(string useBudgetYear, string costIncludedBranch, string budgetType, string reimbursementFormCode, string thisFormID)
        {
            string sql = @"select paub.ID,paub.Name,paub.Budget,paub.BudgetType,paub.BelongYear,UsedBudgetAmount,ProcessExpensesAmount
,(ISNULL(Budget,0)-ISNULL(UsedBudgetAmount,0)-ISNULL(ProcessExpensesAmount,0)) as RemainingBudgetAmount 
from E_BDS_PartyAndUnionBudget paub  
left join 
(
	select CostIncludedBranch,SUM(ISNULL(ReimbursementAmount,0)) as UsedBudgetAmount from S_EP_ReimbursementApply 
	where ReimbursementFormCode = '{1}' and FlowPhase = 'End'  and ID <> '{4}'
	group by CostIncludedBranch
)uba on uba.CostIncludedBranch = paub.ID
left join 
(
	select CostIncludedBranch,SUM(ISNULL(ReimbursementAmount,0)) as ProcessExpensesAmount from S_EP_ReimbursementApply 
	where ReimbursementFormCode = '{1}' and FlowPhase <> 'End'  and ID <> '{4}'
	group by CostIncludedBranch
)pea on pea.CostIncludedBranch = paub.ID
where paub.BudgetType='{0}' and paub.BelongYear = '{2}' and paub.ID='{3}'";
            sql = string.Format(sql, budgetType, reimbursementFormCode, useBudgetYear, costIncludedBranch, thisFormID);
            var dt = this.SQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }

        /// <summary>
        /// 获取外币外汇牌价中间价汇率
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="expenseDate">日期</param>
        /// <returns></returns>
        public JsonResult GetMiddlePriceRate(string currency, string date)
        {
            var returnDic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(currency) || string.IsNullOrWhiteSpace(date))
                return Json("");
            date = date.Replace("\"", "");
            var expenseDate = Convert.ToDateTime(date);
            var bankForeignExchangeRate = this.BusinessEntities.Set<E_BDS_BankForeignExchangeRate>().FirstOrDefault(c => c.ForeignCurrency == currency && c.DataUpdatetTime == expenseDate);
            if (bankForeignExchangeRate == null)
                return Json("");
            returnDic.SetValue("ForeignCurrency", currency);
            returnDic.SetValue("ExpenseDate", date);
            returnDic.SetValue("MiddlePriceRate", bankForeignExchangeRate.MiddlePriceRate);
            return Json(returnDic);
        }

        public JsonResult GetGNRelatedStandardInfo(string employeeLevel, string startDate)
        {
            var returnDic = new Dictionary<string, object>();
            string domesticaccommodationstandardID = string.Empty;

            if (!string.IsNullOrWhiteSpace(startDate))
                startDate = startDate.Replace("\"", "");
            //市内交通工具标准
            string sql = @"select ID,  'JTGJBZ' as StandardType, Transportation as StandardAmount,PersonLevel,StartDate from E_BDS_Transportationstandards
where PersonLevel ='{0}' and StartDate <='{1}'
union
select ID, 'GNYPZSFBZ' as StandardType, cast(StandardAmount as nvarchar(500)) as StandardAmount,PersonLevel,StartDate from E_BDS_Domesticaccommodationstandard
where PersonLevel ='{0}' and StartDate <='{1}'
union
select ID, 'GNWPZSJLBZY' as StandardType, cast(Reward as nvarchar(500)) as StandardAmount,'' as PersonLevel,StartDate from E_BDS_AccommodationSavingEaward
where StartDate <='{1}'
union
select ID, 'SNJTFBZ' as StandardType,cast(CityTransfeeStandard as nvarchar(500)) as StandardAmount, PersonLevel,StartDate from E_BDS_Citytransfeestandard
where PersonLevel ='{0}' and StartDate <='{1}'
union
select ID,'HSBTBZ' as StandardType,cast(Foods as nvarchar(500)) as StandardAmount,'' as  PersonLevel,StartDate from E_BDS_DomesticFoodCostStandard
where  StartDate <='{1}'";
            sql = string.Format(sql, employeeLevel, startDate);
            var dt = this.SQLDB.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var JTGJBZRows = dt.Select("StandardType = 'JTGJBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (JTGJBZRows != null && JTGJBZRows.Count() > 0)
                    returnDic.SetValue("TransportationStandard", JTGJBZRows[0]["StandardAmount"]);

                var GNYPZSFBZRows = dt.Select("StandardType = 'GNYPZSFBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GNYPZSFBZRows != null && GNYPZSFBZRows.Count() > 0)
                {
                    returnDic.SetValue("GNZSBZY", GNYPZSFBZRows[0]["StandardAmount"]);
                    domesticaccommodationstandardID = GNYPZSFBZRows[0]["ID"].ToString();
                }

                var GNWPZSJLBZYRows = dt.Select("StandardType = 'GNWPZSJLBZY'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GNWPZSJLBZYRows != null && GNWPZSJLBZYRows.Count() > 0)
                    returnDic.SetValue("GNWPZSJLBZY", GNWPZSJLBZYRows[0]["StandardAmount"]);

                var SNJTFBZRows = dt.Select("StandardType = 'SNJTFBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (SNJTFBZRows != null && SNJTFBZRows.Count() > 0)
                    returnDic.SetValue("SNJTBZY", SNJTFBZRows[0]["StandardAmount"]);

                var HSBTBZRows = dt.Select("StandardType = 'HSBTBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (HSBTBZRows != null && HSBTBZRows.Count() > 0)
                    returnDic.SetValue("HSBTBZY", HSBTBZRows[0]["StandardAmount"]);
            }

            //国内有票住宿费节约奖励
            sql = @"select * from E_BDS_Domesticaccommodationstandard_Reward where E_BDS_DomesticaccommodationstandardID  ='{0}' order by PJRJYJLEZ desc";
            sql = string.Format(sql, domesticaccommodationstandardID);
            dt = this.SQLDB.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
                returnDic.SetValue("GNYPZSFSRewardStandard", Json(dt));
            return Json(returnDic);
        }

        protected Dictionary<string, string> UpdateLaborInfo(Dictionary<string, string> dic)
        {

            var strDetailsInfo = dic.GetValue("Details");
            var strExpenseDateStart = dic.GetValue("ExpenseDateStart");
            var formID = dic.GetValue("ID");
            if (!string.IsNullOrWhiteSpace(strDetailsInfo) && !string.IsNullOrWhiteSpace(strExpenseDateStart))
            {
                var expenseStartDate = Convert.ToDateTime(strExpenseDateStart);
                var belongYear = expenseStartDate.Year.ToString();
                var belongMonth = expenseStartDate.Month.ToString();
                var detailsList = JsonHelper.ToObject<List<Dictionary<string, string>>>(strDetailsInfo);
                var idCrads = detailsList.Select(c => c.GetValue("IDCard"));
                if (idCrads != null && idCrads.Count() > 0)
                {
                    var strIdsCards = string.Join("','", idCrads);
                    string sql = @"select COUNT(ID) as LaborExpenseCount,IDCard from 
(
	select rad.UserName,rad.CertificateType,rad.IDCard,ra.ID,DateName(year,ra.ExpenseDateStart) as BelongYear,DateName(MONTH,ra.ExpenseDateStart) as BelongMonth from S_EP_ReimbursementApply_Details rad
	left join S_EP_ReimbursementApply ra on ra.ID = rad.S_EP_ReimbursementApplyID
	where rad.IDCard in ('{0}') and ra.ID <> '{1}' 
) aa where BelongYear = '{2}' and BelongMonth = '{3}'
group by IDCard";
                    sql = string.Format(sql, idCrads, formID, belongYear, belongMonth);
                    var dt = this.SQLDB.ExecuteDataTable(sql);
                    foreach (var itdetailDic in detailsList)
                    {
                        var idCard = itdetailDic.GetValue("IDCard");
                        var rowsOfSingleIDCard = dt.Select("IDCard='" + idCard + "'");
                        var laborExpenseCount = 1;
                        if (rowsOfSingleIDCard != null && rowsOfSingleIDCard.Count() > 0)
                        {
                            var rowOfSingleIDCard = rowsOfSingleIDCard[0];
                            if (rowOfSingleIDCard["LaborExpenseCount"] != null)
                            {
                                var strLaborExpenseCount = rowOfSingleIDCard["LaborExpenseCount"].ToString();
                                laborExpenseCount = laborExpenseCount + Convert.ToInt32(strLaborExpenseCount);
                            }
                        }
                        itdetailDic.SetValue("CurrentMonthReimburseCount", laborExpenseCount.ToString());
                    }
                    dic.SetValue("Details", JsonHelper.ToJson(detailsList));
                }
            }

            return dic;
        }

        /// <summary>
        /// 验证报销单逻辑
        /// </summary>
        /// <param name="formDic">表单数据</param>
        /// <param name="formInfo">表单配置信息</param>
        /// <param name="isNew">是否新增数据</param>
        protected void Validate(Dictionary<string, string> formDic, S_UI_Form formInfo, bool isNew)
        {
            var advertiser = formDic.GetValue("Advertiser");
            var advertiserName = formDic.GetValue("AdvertiserName");
            var ID = formDic.GetValue("ID");
            ValidateGNFYJTF(ID, advertiser, advertiserName, formDic);
            ValidateLoanInfo(ID, advertiser, advertiserName, formDic);
            ValidateElectronicTicket(formDic);
            ValidateBudget(formDic);
        }

        /// <summary>
        /// 验证差旅费日期
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <param name="advertiser">报销人ID</param>
        /// <param name="advertiserName">报销人Name</param>
        /// <param name="formDic">表单数据</param>
        protected void ValidateGNFYJTF(string formID, string advertiser, string advertiserName, Dictionary<string, string> formDic)
        {
            var reimbursementFormIds = this.BusinessEntities.Set<S_EP_ReimbursementApply>().Where(c => c.Advertiser == advertiser && c.ID != formID).Select(c => c.ID).ToArray();
            var GNFYJTFListOfAdvertiser = this.BusinessEntities.Set<S_EP_ReimbursementApply_GNFYJTF>().Where(c => reimbursementFormIds.Contains(c.S_EP_ReimbursementApplyID)).ToList();
            if (formDic.ContainsKey("GNFYJTF"))
            {
                var GNFYJTFDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(formDic["GNFYJTF"]);
                foreach (var item in GNFYJTFDataList)
                {
                    var strStartingDate = item.GetValue("StartingDate");
                    var strArrivedDate = item.GetValue("ArrivedDate");
                    var startingDate = Convert.ToDateTime(strStartingDate);
                    var arrivedDate = Convert.ToDateTime(strArrivedDate);

                    var isEffectiveDate = GNFYJTFListOfAdvertiser.Exists(c => c.StartingDate <= startingDate && c.ArrivedDate >= startingDate);
                    if (isEffectiveDate)
                        throw new Formula.Exceptions.BusinessException("报销人【" + advertiserName + "】开始时间为【" + startingDate + "】的交通费明细已经报过差旅费了，请修改！");
                    isEffectiveDate = GNFYJTFListOfAdvertiser.Exists(c => c.StartingDate <= arrivedDate && c.ArrivedDate >= arrivedDate);
                    if (isEffectiveDate)
                        throw new Formula.Exceptions.BusinessException("报销人【" + advertiserName + "】结束时间为【" + arrivedDate + "】的交通费明细已经报过差旅费了，请修改！");
                }
            }
        }

        /// <summary>
        /// 验证借款信息
        /// </summary>
        /// <param name="formID">表单ID</param>
        /// <param name="advertiser">报销人ID</param>
        /// <param name="advertiserName">报销人Name</param>
        /// <param name="formDic">表单数据</param>
        protected void ValidateLoanInfo(string formID, string advertiser, string advertiserName, Dictionary<string, string> formDic)
        {
            if (formDic.ContainsKey("LoanInfo"))
            {
                var strLoanInfoGrid = formDic.GetValue("LoanInfo");
                if (!string.IsNullOrWhiteSpace(strLoanInfoGrid))
                {
                    var loanInfoList = JsonHelper.ToObject<List<Dictionary<string, string>>>(strLoanInfoGrid);
                    var loanApplyIDs = loanInfoList.Select(c => c.GetValue("LoanApplyID"));
                    var loanInfoIds = loanInfoList.Select(c => c.GetValue("ID"));
                    var loanApplyListOfAdvertiser = this.BusinessEntities.Set<S_EP_ReimbursementApply_LoanInfo>().Where(c => loanApplyIDs.Contains(c.LoanApplyID) && !loanInfoIds.Contains(c.ID)).ToList();
                    var loanApplyListOfRepaymentForm = this.BusinessEntities.Set<T_BM_RepaymentForm_LoanInfo>().Where(c => loanApplyIDs.Contains(c.LoanApplyID)).ToList();

                    foreach (var loanInfo in loanInfoList)
                    {
                        var loanApplyID = loanInfo.GetValue("LoanApplyID");
                        var strBorrowFormNumber = loanInfo.GetValue("BorrowFormNumber");
                        var strThisReversalAmount = loanInfo.GetValue("ThisReversalAmount");
                        var strActualLoanAmount = loanInfo.GetValue("ActualLoanAmount");
                        var actualLoanAmount = 0.0M;
                        var thisReversalAmount = 0.0M;
                        if (!string.IsNullOrWhiteSpace(strActualLoanAmount))
                            actualLoanAmount = Convert.ToDecimal(strActualLoanAmount);

                        if (!string.IsNullOrWhiteSpace(strThisReversalAmount))
                            thisReversalAmount = Convert.ToDecimal(strThisReversalAmount);

                        var loanApplyList = loanApplyListOfAdvertiser.Where(c => c.LoanApplyID == loanApplyID).ToList();
                        var loanApplyListOfSingleLoan = loanApplyListOfRepaymentForm.Where(c => c.LoanApplyID == loanApplyID).ToList();
                        var sumReversalAmount = loanApplyList.Sum(c => c.ThisReversalAmount);
                        if (loanApplyListOfSingleLoan != null && loanApplyListOfSingleLoan.Count > 0)
                        {
                            var tempSumReversalAmount = loanApplyListOfSingleLoan.Sum(c => c.ThisTimeAmount);
                            if (tempSumReversalAmount != null)
                                sumReversalAmount += tempSumReversalAmount;
                        }
                        var diffValue = (sumReversalAmount + thisReversalAmount) - actualLoanAmount;

                        if (diffValue > 0)
                            throw new Formula.Exceptions.BusinessException("借款流水号为【" + strBorrowFormNumber + "】的借款申请单总体还款金额已超实际借款金额【" + diffValue + "】，请修改本次冲销金额！");
                    }

                }
            }
        }

        /// <summary>
        /// 验证电子发票
        /// </summary>
        /// <param name="formDic">表单信息</param>
        protected void ValidateElectronicTicket(Dictionary<string, string> formDic)
        {

            if (formDic.ContainsKey("ElectronicTicket"))
            {
                var strElectronicTicketGrid = formDic.GetValue("ElectronicTicket");
                if (!string.IsNullOrWhiteSpace(strElectronicTicketGrid))
                {
                    var formID = formDic.GetValue("ID");
                    var electronicTicketList = JsonHelper.ToObject<List<Dictionary<string, string>>>(strElectronicTicketGrid);
                    var electronicTicketNumbers = electronicTicketList.Select(c => c.GetValue("ElectronicInvoiceNumber"));
                    var electronicTicketListOfAdvertiser = this.BusinessEntities.Set<S_EP_ReimbursementApply_ElectronicTicket>().Where(c => electronicTicketNumbers.Contains(c.ElectronicInvoiceNumber) && c.S_EP_ReimbursementApplyID != formID).ToList();
                    foreach (var electronicTicket in electronicTicketList)
                    {
                        var ID = electronicTicket.GetValue("ID");
                        var strElectronicInvoiceNumber = electronicTicket.GetValue("ElectronicInvoiceNumber");
                        var existedElectronicInvoiceNumber = electronicTicketListOfAdvertiser.Where(c => c.ElectronicInvoiceNumber == strElectronicInvoiceNumber && c.ID != ID).ToList();
                        if (existedElectronicInvoiceNumber != null && existedElectronicInvoiceNumber.Count > 0)
                            throw new Formula.Exceptions.BusinessException("电子发票号码【" + strElectronicInvoiceNumber + "】已经存在，请修改！");
                    }

                }
            }

        }

        /// <summary>
        /// 验证预算
        /// </summary>
        /// <param name="formDic"></param>
        protected void ValidateBudget(Dictionary<string, string> formDic)
        {
            if (formDic.ContainsKey("ReimbursementFormCode") && formDic.ContainsKey("ReimbursementAmount"))
            {
                var strReimbursementFormCode = formDic.GetValue("ReimbursementFormCode");
                if ("RM_Partyexpenses" == strReimbursementFormCode || "RM_Unionexpense" == strReimbursementFormCode)
                    ValidatePartyAndUnionBudget(formDic, strReimbursementFormCode);
            }
        }

        /// <summary>
        /// 验证党费及工会经费预算
        /// </summary>
        /// <param name="formDic"></param>
        /// <param name="reimbursementFormCode"></param>
        protected void ValidatePartyAndUnionBudget(Dictionary<string, string> formDic, string reimbursementFormCode)
        {

            var strUseBudgetYear = formDic.GetValue("UseBudgetYear");
            var strCostIncludedBranch = formDic.GetValue("CostIncludedBranch");
            var strID = formDic.GetValue("ID");
            string budgetType = string.Empty;
            if ("RM_Partyexpenses" == reimbursementFormCode)
                budgetType = BudgetType.Party.ToString();
            else if ("RM_Unionexpense" == reimbursementFormCode)
                budgetType = BudgetType.Union.ToString();
            var jsonBudgetInfo = GetPartyOrUnionBudgetInfo(strUseBudgetYear, strCostIncludedBranch, budgetType, reimbursementFormCode, strID);
            var budgetInfoDt = (System.Data.DataTable)(jsonBudgetInfo.Data);
            if (budgetInfoDt != null && budgetInfoDt.Rows.Count > 0)
            {
                var singleRow = budgetInfoDt.Rows[0];
                var strRemainingBudgetAmount = singleRow["RemainingBudgetAmount"];
                if (strRemainingBudgetAmount != null)
                {
                    var strReimbursementAmount = formDic.GetValue("ReimbursementAmount");
                    var dRemainingBudgetAmount = Convert.ToDecimal(strRemainingBudgetAmount);

                    var dReimbursementAmount = Convert.ToDecimal(strReimbursementAmount);
                    var diffValue = dReimbursementAmount - dRemainingBudgetAmount;
                    if (diffValue > 0)
                        throw new Formula.Exceptions.BusinessException("本次报销金额超出剩余预算金额，流程无法提交，超出金额为【" + diffValue + "】，请修改!");
                }
            }
        }
    }
}
