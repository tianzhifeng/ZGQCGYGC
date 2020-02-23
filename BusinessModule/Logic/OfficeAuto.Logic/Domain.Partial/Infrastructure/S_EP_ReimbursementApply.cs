using Config;
using Config.Logic;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeAuto.Logic.Domain
{
    public partial class S_EP_ReimbursementApply
    {
        public void Push()
        {
            //处理借款信息
            var officeAutoEntities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            var loanInfosOfRF = this.S_EP_ReimbursementApply_LoanInfo.ToList();
            var loanInfoIDs = loanInfosOfRF.Select(c => c.LoanApplyID).ToArray();
            var loanInfos = officeAutoEntities.Set<S_BM_LoanApply>().Where(c => loanInfoIDs.Contains(c.ID)).ToList();
            foreach (var loanInfoOfRF in loanInfosOfRF)
            {
                var loanInfo = loanInfos.FirstOrDefault(c => c.ID == loanInfoOfRF.LoanApplyID);
                if (loanInfo != null)
                    loanInfo.Repayment(loanInfoOfRF.ThisReversalAmount);
            }

            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var officAutoDB = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
            //费用报销费用明细
            foreach (var detailInfo in this.S_EP_ReimbursementApply_Details.ToList())
            {
                var cbsCode = detailInfo.TwoExpenseScourseCode;
                var cbsName = detailInfo.TwoExpenseScourse;
                if (!string.IsNullOrWhiteSpace(detailInfo.ThreeExpenseScourseCode))
                {
                    cbsCode = detailInfo.ThreeExpenseScourseCode;
                    cbsName = detailInfo.ThreeExpenseScourse;
                }
                if (string.IsNullOrWhiteSpace(cbsCode))
                {
                    cbsCode = this.ExpenseScourse;
                    cbsName = this.ExpenseScourseName;
                }

                var cost = officeAutoEntities.S_FC_CostInfo.Create();
                cost.ID = FormulaHelper.CreateGuid();
                cost.CostDate = this.ReimbursementDate.HasValue ? this.ReimbursementDate.Value : DateTime.Now;
                cost.BelongMonth = cost.CostDate.Month;
                cost.BelongQuarter = (cost.CostDate.Month - 1) / 3 + 1;
                cost.BelongYear = cost.CostDate.Year;
                cost.CostUserDeptID = this.AdvertiserDept;
                cost.CostUserDeptName = this.AdvertiserDeptName;
                cost.CostUserID = this.Advertiser;
                cost.CostUserName = this.AdvertiserName;
                if (string.IsNullOrWhiteSpace(cost.CostUserID))
                {
                    cost.CostUserDeptID = this.ApplyDept;
                    cost.CostUserDeptName = this.ApplyDeptName;
                    cost.CostUserID = this.ApplyUser;
                    cost.CostUserName = this.ApplyUserName;
                }
                cost.CostValue = detailInfo.TranslatedRMBAmount.HasValue ? detailInfo.TranslatedRMBAmount.Value : 0;
                var sql = "select SerialNumber as Code,ChargerDept,ChargerDeptName,ProjectClass  from S_I_Engineering where ID='{0}'";
                var dt = marketDB.ExecuteDataTable(String.Format(sql, this.ProjectName));
                if (dt.Rows.Count > 0)
                {
                    cost.ProjectClass = dt.Rows[0]["ProjectClass"].ToString();
                    cost.ProjectCode = dt.Rows[0]["Code"].ToString();
                    cost.ProjectDeptID = dt.Rows[0]["ChargerDept"].ToString();
                    cost.ProjectDeptName = dt.Rows[0]["ChargerDeptName"].ToString();
                }
                cost.ProjectID = this.ProjectName;
                cost.ProjectName = this.ProjectNameName;

                cost.ProjectType = this.ProjectType;
                cost.Quantity = 0;
                cost.SubjectCode = cbsCode;
                cost.SubjectName = cbsName;
                cost.UnitPrice = 0;
                cost.RelateID = detailInfo.ID;
                cost.FormID = this.ID;
                officeAutoEntities.S_FC_CostInfo.Add(cost);

            }

            var newLaborsInfos = this.S_EP_ReimbursementApply_Details.Where(c => c.IsNewPerson == "1").ToList();

            var IdCrads = newLaborsInfos.Select(c => c.IDCard).ToList();
            var existedLaborsInfos = officeAutoEntities.Set<E_BDS_LaborsInfo>().Where(c => loanInfoIDs.Contains(c.IDCard)).ToList();

            foreach (var newLaborsInfo in newLaborsInfos)
            {
                var laborsInfo = existedLaborsInfos.FirstOrDefault(c => c.IDCard == newLaborsInfo.IDCard);
                if (laborsInfo == null)
                {
                    laborsInfo = officeAutoEntities.Set<E_BDS_LaborsInfo>().Create();
                    laborsInfo.ID = FormulaHelper.CreateGuid();
                    laborsInfo.UserName = newLaborsInfo.UserName;
                    laborsInfo.CertificateType = newLaborsInfo.CertificateType;
                    laborsInfo.IDCard = newLaborsInfo.IDCard;
                    laborsInfo.UserPhone = newLaborsInfo.UserPhone;
                    laborsInfo.HiringTime = newLaborsInfo.HiringTime;
                    laborsInfo.AccountName = newLaborsInfo.AccountName;
                    laborsInfo.OpenBank = newLaborsInfo.OpenBank;
                    laborsInfo.CardNumber = newLaborsInfo.CardNumber;
                    officeAutoEntities.Set<E_BDS_LaborsInfo>().Add(laborsInfo);
                }
            }
        }
    }
}
