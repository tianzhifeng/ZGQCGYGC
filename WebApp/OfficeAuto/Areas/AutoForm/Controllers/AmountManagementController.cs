using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Config;
using MvcAdapter;
using Formula;
using OfficeAuto.Logic;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class AmountManagementController : OfficeAutoFormContorllor<S_FC_UserLoanAccount>
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = string.Format(@" select * from (select UserID as ID,UserName,u.DeptID,u.DeptName,isnull(Sum(AccountValue),0) as LoanValue
from dbo.S_FC_UserLoanAccount ula
left join {0}.dbo.S_A_User u
on ula.UserID=u.ID
group by UserID,UserName,u.DeptID,u.DeptName) as UserAmont
where LoanValue>0", baseHelper.DbName);
            var db = this.SQLDB.ExecuteGridData(sql, qb);
            return Json(db);
        }

        public JsonResult GetDetailList(QueryBuilder qb, string userID)
        {
            var sql = string.Format(@"select *,
	case when RelateFormID is not null and AccountType='{1}' then '查看借款单' 
	when RelateFormID is not null and AccountType='{2}' then '查看报销单' 
	else '' end as ShowForm
	 from dbo.S_FC_UserLoanAccount
where UserID='{0}'", userID, LoanAccountType.借款.ToString(), LoanAccountType.还款.ToString());
            var db = this.SQLDB.ExecuteGridData(sql, qb);
            return Json(db);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
        }

        public void CancelReturnAmount(string rowIDs)
        {
            var idArr = rowIDs.Trim(',').Split(',');
            this.BusinessEntities.Set<S_FC_UserLoanAccount>().Delete(d => idArr.Contains(d.ID));
            this.BusinessEntities.SaveChanges();
        }
    }
}