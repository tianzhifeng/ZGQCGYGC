using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using Config;
using Config.Logic;
using Formula;
using OfficeAuto.Logic.Domain;
using Base.Logic.Domain;
using Workflow.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class LoanApplyController : OfficeAutoFormContorllor<S_BM_LoanApply>
    {

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (isNew == true)
            {
                var row = dt.Rows[0];
                var loanApplyListOfCurrentUserInfo = this.BusinessEntities.Set<S_BM_LoanApply>().Where(c => c.ActualBorrower == this.CurrentUserInfo.UserID).ToList();
                var unReturnAmount = loanApplyListOfCurrentUserInfo.Sum(c => c.LoanAmount) - loanApplyListOfCurrentUserInfo.Sum(c => c.AlreadyReturnAmount);
                row["JZMQWZSJJKRZWHKJE"] = unReturnAmount;
                dt.AcceptChanges();
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            if (isNew == true)
            {
                dic.SetValue("BelongYear", DateTime.Now.Year.ToString());
                dic.SetValue("BelongMonth", DateTime.Now.Month.ToString());
                dic.SetValue("BelongQuarter", (((DateTime.Now.Month) + 2) / 3).ToString());
            }
        }

        public JsonResult GetLoanUserJZMQWZSJJKRZWHKJEInfo(string loanUserID)
        {
            var returnDic = new Dictionary<string, object>();
            string sql = @"select ActualBorrower,SUM(JZMQWZSJJKRZWHKJE) as JZMQWZSJJKRZWHKJE,ual.IsAllowLoan from 
(
	select ISNULL((ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0)),0) as JZMQWZSJJKRZWHKJE,ActualBorrower
	from S_BM_LoanApply where ActualBorrower = '{0}'
) la left join E_BDS_UserAllowLoan ual on ual.UserID = la.ActualBorrower
where la.ActualBorrower = '{0}'
group by ActualBorrower,IsAllowLoan";
            sql = string.Format(sql, loanUserID);
            var dt = this.SQLDB.ExecuteDataTable(sql);
            var JZMQWZSJJKRZWHKJE = string.Empty;
            var isAllowBorrow = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                JZMQWZSJJKRZWHKJE = row["JZMQWZSJJKRZWHKJE"] == null ? "" : row["JZMQWZSJJKRZWHKJE"].ToString();
                isAllowBorrow = row["IsAllowLoan"] == null ? "" : row["IsAllowLoan"].ToString();
            }
            returnDic.SetValue("JZMQWZSJJKRZWHKJE", JZMQWZSJJKRZWHKJE);
            returnDic.SetValue("IsAllowBorrow", isAllowBorrow);

            return Json(returnDic);
        }
        /// <summary>
        /// 获取单个借款人的借款明细
        /// </summary>
        /// <param name="abUserID">实际借款人ID</param>
        /// <returns></returns>
        public JsonResult GetPersonalLoanStatement(string abUserID)
        {
            var baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = @" 
select aa.ID,ActualBorrower,ActualBorrowerName,TaskNature,SUM(LoanAmount) as SumLoanAmount,SUM(AlreadyReturnAmount) as SumAlreadyReturnAmount
,SUM(UnReturnAmount) as SumUnReturnAmount,SUM(OneYearUnReturnAmount) as SumOneYearUnReturnAmount
,SUM(TwoYearUnReturnAmount) as SumTwoYearUnReturnAmount,SUM(ThreeYearUnReturnAmount) as SumThreeYearUnReturnAmount
,SUM(FourYearUnReturnAmount) as SumFourYearUnReturnAmount,SUM(FiveYearUnReturnAmount) as SumFiveYearUnReturnAmount
,SUM(MoreFiveYearUnReturnAmount) as SumMoreFiveYearUnReturnAmount,MobilePhone,DeptID,DeptName from
(
	SELECT ActualBorrower as ID,ActualBorrower,ActualBorrowerName,TaskNature,ISNULL(LoanAmount,0) as LoanAmount,ISNULL(AlreadyReturnAmount,0) as AlreadyReturnAmount
	,ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) as UnReturnAmount
	,BelongYear--,(case when (ISNULL(LoanAmount,0) <> ISNULL(AlreadyReturnAmount,0)) then 'true' else 'false' end)
	,(case when (DATEPART(yyyy,getdate())-BelongYear) = 0 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as OneYearUnReturnAmount
	,(case when (DATEPART(yyyy,getdate())-BelongYear) = 1 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as TwoYearUnReturnAmount
	,(case when (DATEPART(yyyy,getdate())-BelongYear) = 2 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as ThreeYearUnReturnAmount
	,(case when (DATEPART(yyyy,getdate())-BelongYear) = 3 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as FourYearUnReturnAmount
	,(case when (DATEPART(yyyy,getdate())-BelongYear) = 4 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as FiveYearUnReturnAmount
	,(case when (DATEPART(yyyy,getdate())-BelongYear) >= 5 then ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) else 0 end ) as MoreFiveYearUnReturnAmount
	
	from S_BM_LoanApply where FlowPhase = 'End' and IsBranchLoan = 'False' and ISNULL(LoanAmount,0) <> ISNULL(AlreadyReturnAmount,0) and ActualBorrower='{1}'
)aa 
left join {0}.dbo.S_A_User u on u.ID = aa.ActualBorrower
group by aa.ID,ActualBorrower,ActualBorrowerName,TaskNature,MobilePhone,DeptID,DeptName";
            sql = string.Format(sql, baseSqlHelper.DbName, abUserID);
            var dt = this.SQLDB.ExecuteDataTable(sql);
            return Json(dt);

        }

        public JsonResult SetUserIsAllowLoan(string actualBorrower, string isAllowLoan)
        {
            var userInfo = FormulaHelper.GetUserInfoByID(actualBorrower);
            if (userInfo == null)
                throw new Formula.Exceptions.BusinessException("获取用户ID为【】的信息失败，请联系管理员");
            var userAllowLoanInfo = this.BusinessEntities.Set<E_BDS_UserAllowLoan>().FirstOrDefault(c=>c.UserID == actualBorrower);
            if (userAllowLoanInfo == null)
            {
                userAllowLoanInfo = this.BusinessEntities.Set<E_BDS_UserAllowLoan>().Create();
                userAllowLoanInfo.ID = FormulaHelper.CreateGuid();
                userAllowLoanInfo.UserID = userInfo.UserID;
                userAllowLoanInfo.UserName = userInfo.UserName;
                userAllowLoanInfo.UserCode = userInfo.Code;
                this.BusinessEntities.Set<E_BDS_UserAllowLoan>().Add(userAllowLoanInfo);
            }
            userAllowLoanInfo.IsAllowLoan = isAllowLoan;

            this.BusinessEntities.SaveChanges();
            return Json("");

        }

        protected override void OnFlowEnd(S_BM_LoanApply entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            entity.Push();
            this.BusinessEntities.SaveChanges();
        }

    }
}
