using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.Logic;
using Formula;

namespace HR.Logic.Domain
{
    public partial class S_W_UserWorkHour
    {
        public void ImportToCost()
        {
            var oaDb = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var projectDb = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = oaDb.ExecuteDataTable("select * from S_FC_CostInfo where RelateID='" + this.ID + "'");
          
            var dic = new Dictionary<string, object>();
            var costValue = 0m;
            var userUnitPrice = this._getUserUnitPrice();
            if (this.WorkHourDay.HasValue)
            {
                costValue = this.WorkHourDay.Value * userUnitPrice;
            }
            if (dt.Rows.Count == 0)
            {
                var projectDt = projectDb.ExecuteDataTable("select * from S_I_ProjectInfo where ID='" + this.ProjectID + "'");

                dic.SetValue("ID", FormulaHelper.CreateGuid());
                dic.SetValue("SubjectName", "人工成本");
                dic.SetValue("SubjectCode", "UserCost");
                dic.SetValue("ProjectID", this.ProjectID);
                dic.SetValue("ProjectName", this.ProjectName);
                dic.SetValue("ProjectCode", this.ProjectCode);
                dic.SetValue("CostType", "UserCost");
                dic.SetValue("ProjectType", this.WorkHourType);               
                dic.SetValue("CostValue", costValue);
                dic.SetValue("CostUserID", this.UserID);
                dic.SetValue("CostUserName", this.UserName);
                dic.SetValue("CostUserDeptID", this.UserDeptID);
                dic.SetValue("CostUserDeptName", this.UserDeptName);
                dic.SetValue("ProjectDeptID", this.ProjectDept);
                dic.SetValue("ProjectDeptName", this.ProjectDeptName);
                dic.SetValue("ProjectChargerUserID", this.ProjectChargerUser);
                dic.SetValue("ProjectChargerUserName", this.ProjectChargerUserName);
                if (projectDt.Rows.Count > 0) {
                    dic.SetValue("ProjectClass", projectDt.Rows[0]["ProjectClass"]);
                    dic.SetValue("ProjectID", projectDt.Rows[0]["MarketProjectInfoID"]);
                }       
                dic.SetValue("UnitPrice", userUnitPrice);
                dic.SetValue("Quantity", this.WorkHourDay.HasValue ? this.WorkHourDay.Value : 0);
                dic.SetValue("CostDate", this.WorkHourDate);
                dic.SetValue("BelongYear", this.WorkHourDate.Year);
                dic.SetValue("BelongMonth", this.WorkHourDate.Month);
                dic.SetValue("BelongQuarter", this.WorkHourDate.Month);
                dic.SetValue("RelateID", this.ID);
                dic.SetValue("FormID", "");
                dic.InsertDB(oaDb, "S_FC_CostInfo", dic.GetValue("ID"));
                dic.InsertDB(marketDB, "S_FC_CostInfo", dic.GetValue("ID"));
            }
            else
            {
                Formula.FormulaHelper.DataRowToDic(dt.Rows[0], dic);
                dic.SetValue("CostValue", costValue);
                dic.SetValue("CostUserID", this.UserID);
                dic.SetValue("CostUserName", this.UserName);
                dic.SetValue("CostUserDeptID", this.UserDeptID);
                dic.SetValue("CostUserDeptName", this.UserDeptName);
                dic.SetValue("ProjectDeptID", this.ProjectDept);
                dic.SetValue("ProjectDeptName", this.ProjectDeptName);
                dic.SetValue("ProjectChargerUserID", this.ProjectChargerUser);
                dic.SetValue("ProjectChargerUserName", this.ProjectChargerUserName);
                dic.UpdateDB(oaDb, "S_FC_CostInfo", dic.GetValue("ID"));
                dic.UpdateDB(marketDB, "S_FC_CostInfo", dic.GetValue("ID"));
            }
        }

        public void RevertCost()
        {
            var oaDb = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Market);

            oaDb.ExecuteNonQuery("delete from S_FC_CostInfo where RelateID='" + this.ID + "'");
            marketDB.ExecuteNonQuery("delete from S_FC_CostInfo where RelateID='" + this.ID + "'");
        }

        decimal _getUserUnitPrice()
        {
            var dbContext = FormulaHelper.GetEntities<HREntities>();
            var unitPrice = dbContext.S_W_UserUnitPrice.Where(d => d.StartDate <= DateTime.Now).OrderByDescending(d => d.StartDate).FirstOrDefault();
            if (unitPrice == null)
                return 0m;
            else
            {
                return unitPrice.UnitPrice;
            }
        }

    }
}
