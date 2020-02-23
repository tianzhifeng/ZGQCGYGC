using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Config;
using Config.Logic;
using System.Data;
using Formula;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class PersonalLoanUnReturnNoticeController : OfficeAutoFormContorllor<T_BM_PersonalLoanUnReturnNotice>
    {

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (isNew == true)
            {
                var newRow = dt.Rows[0];
                var abUserSql = @"select ActualBorrower,ActualBorrowerName,DeptID as ActualBorrowerDeptID,DeptName as ActualBorrowerDeptName
,u.MobilePhone as ActualBorrowerMobilePhone,SUM(ISNULL(UnReturnAmount,0)) as UnReturnAmount from
(
	SELECT SerialNumber, ActualBorrower as ID,ActualBorrower,ActualBorrowerName
	,ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) as UnReturnAmount
	from S_BM_LoanApply where FlowPhase = 'End' and IsBranchLoan = 'False' and ISNULL(LoanAmount,0) <> ISNULL(AlreadyReturnAmount,0)
)ura left join {0}.dbo.S_A_User u on ura.ActualBorrower = u.ID
group by ActualBorrower,ActualBorrowerName,DeptID,DeptName,MobilePhone";
                var baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                abUserSql = string.Format(abUserSql, baseSqlHelper.DbName);
                var abUserdt = this.SQLDB.ExecuteDataTable(abUserSql);
                var actualBorrowerIds = abUserdt.AsEnumerable().Select(c => c.Field<string>("ActualBorrower"));
                var actualBorrowerNames = abUserdt.AsEnumerable().Select(c => c.Field<string>("ActualBorrowerName"));
                var strActualBorrowerIds = string.Join(",", actualBorrowerIds);
                var strActualBorrowerNames = string.Join(",", actualBorrowerNames);
                newRow["ABUser"] = strActualBorrowerIds;
                newRow["ABUserName"] = strActualBorrowerNames;
                dt.AcceptChanges();
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var ID = dic.GetValue("ID");
            var actualBorrowerIds = dic.GetValue("ABUser");     //需发消息用户IDs
            var actualBorrowerNames = dic.GetValue("ABUserName");   //需发消息用户Names
            var strTJJZRQ = dic.GetValue("TJJZRQ");    //统计截至日期
            var strHKJZRQ = dic.GetValue("HKJZRQ");    //还款截至日期
            if (string.IsNullOrWhiteSpace(strTJJZRQ) || string.IsNullOrWhiteSpace(strHKJZRQ))
                throw new Formula.Exceptions.BusinessException("统计截至日期或者还款截至日期字段为空，请联系管理员！");
            var TJJZRQ = Convert.ToDateTime(strTJJZRQ).ToLongDateString();
            var HKJZRQ = Convert.ToDateTime(strHKJZRQ).ToLongDateString();


            string loanInfoSql = @"
	SELECT ID,SerialNumber, ActualBorrower as ID,ActualBorrower,ActualBorrowerName
	,ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) as UnReturnAmount
	from S_BM_LoanApply where FlowPhase = 'End' and IsBranchLoan = 'False' and ISNULL(LoanAmount,0) <> ISNULL(AlreadyReturnAmount,0)";
            var loanInfoDt = this.SQLDB.ExecuteDataTable(loanInfoSql);

            var abUserSql = @"select ActualBorrower,ActualBorrowerName,DeptID as ActualBorrowerDeptID,DeptName as ActualBorrowerDeptName
,u.MobilePhone as ActualBorrowerMobilePhone,SUM(ISNULL(UnReturnAmount,0)) as UnReturnAmount from
(
	SELECT SerialNumber, ActualBorrower as ID,ActualBorrower,ActualBorrowerName
	,ISNULL(LoanAmount,0)-ISNULL(AlreadyReturnAmount,0) as UnReturnAmount
	from S_BM_LoanApply where FlowPhase = 'End' and IsBranchLoan = 'False' and ISNULL(LoanAmount,0) <> ISNULL(AlreadyReturnAmount,0)
)ura left join {0}.dbo.S_A_User u on ura.ActualBorrower = u.ID
group by ActualBorrower,ActualBorrowerName,DeptID,DeptName,MobilePhone";
            var baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            abUserSql = string.Format(abUserSql, baseSqlHelper.DbName);
            var abUserdt = this.SQLDB.ExecuteDataTable(abUserSql);
            if (!string.IsNullOrWhiteSpace(actualBorrowerIds))
            {
                var aActualBorrowerIds = actualBorrowerIds.Split(',');
                var messageService = FormulaHelper.GetService<IMessageService>();
                var iActualBorrowerIdsCount = aActualBorrowerIds.Count();
                var linkUrl = @"/MvcConfig/UI/List/PageView?TmplCode=BM_Personalloanstatement&PersonalLoanUnReturnNoticeID=" + ID;
                for (int i = 0; i < iActualBorrowerIdsCount; i++)
                {
                    var abUserRows = abUserdt.Select("ActualBorrower='" + aActualBorrowerIds[i] + "'");
                    if (abUserRows != null && abUserRows.Count() > 0)
                    {
                        //发消息给欠款人
                        var abUserRow = abUserRows[0];
                        var actualBorrowerName = abUserRow["ActualBorrowerName"].ToString();
                        var actualBorrowerDeptName = abUserRow["ActualBorrowerDeptName"] == null ? "" : abUserRow["ActualBorrowerDeptName"].ToString();
                        var dUnReturnAmount = 0.0M;
                        var unReturnAmount = abUserRow["UnReturnAmount"] == null ? "" : abUserRow["UnReturnAmount"].ToString();
                        if (!string.IsNullOrWhiteSpace(unReturnAmount))
                            dUnReturnAmount = Convert.ToDecimal(unReturnAmount);
                        var title = DateTime.Now.Year.ToString() + "年个人借款催款通知";
                        var strSendNoticeDate = DateTime.Now.ToLongDateString();
                        var translateUnReturnAmount = dUnReturnAmount.ToString("N");
                        var content = "<p style=\"white-space: normal;\">{0}{1}：</p><p style=\"white-space: normal;\">&nbsp; &nbsp;您好，截至{2}，你未还款的个人借款总额为{3}元，按照公司财[2018]6号《关于清理个人借款的通知》的规定，请您于{4}前归还个人借款或履行报销程序！点击【相关链接】可查看个人欠款详细信息！</p><p style=\"white-space: normal;\">&nbsp; &nbsp; 特此通知&nbsp;</p><p style=\"white-space: normal;\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 资产财务部</p><p style=\"white-space: normal;\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{5}</p>";
                        content = string.Format(content, actualBorrowerDeptName, actualBorrowerName, TJJZRQ, translateUnReturnAmount, HKJZRQ, strSendNoticeDate);
                        messageService.SendMsg(title, content, linkUrl, "", aActualBorrowerIds[i], actualBorrowerName);
                    }
                }
            }

            //记录借款催款通知下达日期
            var Ids = loanInfoDt.AsEnumerable().Select(c => c.Field<string>("ID"));
            var strIds = string.Join("','", Ids);
            var updateSql = @"update S_BM_LoanApply set UnReturnNoticeDate='{0}' where ID in ('{1}')";
            updateSql = string.Format(updateSql, DateTime.Now, strIds);
            this.SQLDB.ExecuteNonQuery(updateSql);


        }
    }
}
