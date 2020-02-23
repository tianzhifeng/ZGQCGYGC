using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.Logic;
using Formula;

namespace Comprehensive.Logic.Domain
{
    public partial class S_W_UserWorkHour
    {
        string HrCBSCode = "Labor";
        public void ImportToCost(string State = "Create")
        {
            var userUnitPrice = this._getUserUnitPrice();
            var costValue = 0m;
            var workValue = 0m;
            if (this.WorkHourValue.HasValue)
                workValue += this.WorkHourValue.Value;         
            costValue = workValue * userUnitPrice;

            var epcDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var dic = new Dictionary<string, object>();
            var dt = epcDB.ExecuteDataTable("select * from S_I_CBS_Cost where RelateID='" + this.ID + "'");
            if (dt.Rows.Count == 0)
            {
                var engineeringInfoDt = epcDB.ExecuteDataTable("select * from S_I_CBS_Cost where RelateID='" + this.ProjectID + "'");
                dic.SetValue("ID", FormulaHelper.CreateGuid());
                dic.SetValue("EngineeringInfoID", this.ProjectID);
                var cbsNode = epcDB.ExecuteDataTable(@"select * from S_I_CBS where Code = '" + HrCBSCode + @"' 
                and EngineeringInfoID = '" + this.ProjectID + "' order by FullID,SortIndex");
                if (cbsNode.Rows.Count > 0)
                {
                    dic.SetValue("CBSID", cbsNode.Rows[0]["ID"]);
                    dic.SetValue("CBSFullID", cbsNode.Rows[0]["FullID"]);
                    dic.SetValue("CBSCode", cbsNode.Rows[0]["Code"]);
                    dic.SetValue("CBSName", cbsNode.Rows[0]["Name"]);
                }
                dic.SetValue("FullID", dic.GetValue("ID"));
                dic.SetValue("Name", this.UserName);
                dic.SetValue("Code", this.UserID);
                dic.SetValue("Quantity", this.WorkHourValue);
                dic.SetValue("UnitPrice", userUnitPrice);
                dic.SetValue("TotalValue", costValue);
                dic.SetValue("CostDate", this.WorkHourDate);
                dic.SetValue("BelongYear", this.WorkHourDate.Year);
                dic.SetValue("BelongMonth", this.WorkHourDate.Month);
                dic.SetValue("UserDept", this.UserDeptID);
                dic.SetValue("UserDeptName", this.UserDeptName);
                dic.SetValue("State", State);
                dic.SetValue("RelateFormID", this.ID);
                dic.SetValue("RelateBusinessID", this.ID);
                dic.SetValue("RelateID", this.ID);
                dic.InsertDB(epcDB, "S_I_CBS_Cost", dic.GetValue("ID"));
            }
            else
            {
                Formula.FormulaHelper.DataRowToDic(dt.Rows[0], dic);
                dic.SetValue("Quantity", this.WorkHourValue);
                dic.SetValue("UnitPrice", userUnitPrice);
                dic.SetValue("TotalValue", costValue);
                dic.SetValue("CostDate", this.WorkHourDate);
                dic.SetValue("BelongYear", this.WorkHourDate.Year);
                dic.SetValue("BelongMonth", this.WorkHourDate.Month);
                dic.SetValue("UserDept", this.UserDeptID);
                dic.SetValue("UserDeptName", this.UserDeptName);
                dic.UpdateDB(epcDB, "S_I_CBS_Cost", dic.GetValue("ID"));
            }
        }

        public void RevertCost()
        {
            //var oaDb = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
            //var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);

            //oaDb.ExecuteNonQuery("delete from S_FC_CostInfo where RelateID='" + this.ID + "'");
            //marketDB.ExecuteNonQuery("delete from S_FC_CostInfo where RelateID='" + this.ID + "'");
        }

        decimal _getUserUnitPrice()
        {
            var dbContext = FormulaHelper.GetEntities<ComprehensiveEntities>();
            var costInfo = dbContext.S_HR_UserCostInfo.Where(d => d.StartDate <= DateTime.Now).OrderByDescending(d => d.StartDate).FirstOrDefault();
            if (costInfo == null)
                return 0m;
            var unitPrice = costInfo.S_HR_UserUnitPrice.FirstOrDefault(d => d.UserID == this.UserID);
            if (unitPrice == null)
                return 0m;
            else
            {
                return unitPrice.UnitPrice;
            }
        }

    }
}
