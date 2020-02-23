using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Formula.Helper;
using Project.Logic;
using System.Data;
using MvcAdapter;
using Config;
using Config.Logic;
using Base.Logic.Domain;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class ResourceAnalyzeController : ProjectController
    {
        public JsonResult GetList(string StartDate, string EndDate)
        {
           
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var HrDB = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var resultDt = new DataTable();
            resultDt.Columns.Add("DeptName");
            resultDt.Columns.Add("DeptID");
            resultDt.Columns.Add("UserCount", typeof(int));
            resultDt.Columns.Add("BaseWorkHour", typeof(decimal));
            resultDt.Columns.Add("Production", typeof(decimal));
            resultDt.Columns.Add("Other", typeof(decimal));
            resultDt.Columns.Add("SumWorkHour", typeof(decimal));
            resultDt.Columns.Add("AvgSumWorkHour", typeof(decimal));
            resultDt.Columns.Add("AvgProduction", typeof(decimal));
            resultDt.Columns.Add("ProductionScale", typeof(decimal));

            var deptSQL = @"select ID,Name,UserCount,SortIndex from S_A_Org
left join (select Count(0) as UserCount,DeptID from dbo.S_A_User
where IsDeleted='0' group by DeptID) UserDeptInfo on S_A_Org.ID=UserDeptInfo.DeptID
where Type in ('ManufactureDept') order by SortIndex ";
            var mainDeptDt = baseDB.ExecuteDataTable(deptSQL);

            string sql = @"select * from (select UserDeptID,UserDeptName,BelongYear,BelongQuarter,BelongMonth,
isnull(Max(Production),0) as ProductionWorkHour,
isnull(Max(Other),0) as OtherWorkHour
from  (select  Sum(WorkHourValue) as WorkHourValue,DeptWorkHourType,
UserDeptID,UserDeptName,BelongYear,BelongMonth,BelongQuarter 
from (select case when WorkHourType='Production' then 'Production' else 'Other' end as DeptWorkHourType,* 
from S_W_UserWorkHour where 1=1 {0} ) S_W_UserWorkHour
group by  UserDeptID,UserDeptName,DeptWorkHourType,BelongYear,BelongMonth,BelongQuarter 
) as DeptWorkHourInfo
pivot(avg(WorkHourValue) for DeptWorkHourType in (Production,Other)) tableInfo
group by UserDeptID,UserDeptName,BelongYear,BelongQuarter,BelongMonth) TableInfo  ";
            if (String.IsNullOrEmpty(EndDate)) EndDate = DateTime.Now.ToShortDateString();
            if (String.IsNullOrEmpty(StartDate))
            {
                StartDate = DateTime.Now.AddMonths(-1).ToShortDateString();
            }

            string whereStr = " and WorkHourDate>='" + StartDate + "' and WorkHourDate <= '" + EndDate + "'";
            var workHourDt = HrDB.ExecuteDataTable(String.Format(sql,whereStr));
            var standardWork = this.getStandardWorkHour(Convert.ToDateTime(StartDate), Convert.ToDateTime(EndDate));
            var result = new Dictionary<string, object>();

            foreach (DataRow dept in mainDeptDt.Rows)
            {
                var row = resultDt.NewRow();
                row["DeptName"] = dept["Name"];
                row["DeptID"] = dept["ID"];
                var userCount = dept["UserCount"] == null || dept["UserCount"] == DBNull.Value ? 0 : Convert.ToInt32(dept["UserCount"]);
                row["UserCount"] = userCount;
                var standardWorkHour = dept["UserCount"] == null || dept["UserCount"] == DBNull.Value ? 0 : Convert.ToInt32(dept["UserCount"]) * standardWork;
                row["BaseWorkHour"] = standardWorkHour;
                var obj = workHourDt.Compute("Sum(ProductionWorkHour)", " UserDeptID='" + dept["ID"] + "'");
                var productionValue = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                obj = workHourDt.Compute("Sum(OtherWorkHour)", " UserDeptID='" + dept["ID"] + "'");
                var otherValue = obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
                var sumWorkHour = productionValue + otherValue;
                row["Production"] = productionValue;
                row["Other"] = otherValue;
                row["SumWorkHour"] = productionValue + otherValue;
                row["AvgSumWorkHour"] = userCount == 0 ? 0m : Math.Round((productionValue + otherValue) / userCount, 2);
                row["AvgProduction"] = userCount == 0 ? 0m : Math.Round(productionValue / userCount, 2);
                row["ProductionScale"] = standardWorkHour == 0 ? 0 : Math.Round(productionValue*100 / standardWorkHour , 2);
                resultDt.Rows.Add(row);
            }
            result.SetValue("data", resultDt);

            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "工时");
            y1.Lable.SetValue("format", "{value}");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "项目负荷率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();
            var productionSer = new Series { Name = "项目工时", Field = "Production", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var otherer = new Series { Name = "非项目工时", Field = "Other", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var productionScaleSer = new Series { Name = "项目负荷率", Field = "ProductionScale", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            productionScaleSer.Tooltip.SetValue("valueSuffix", "%");
         
            serDefines.Add(productionSer);
            serDefines.Add(otherer);
            serDefines.Add(productionScaleSer);
            var chart = HighChartHelper.CreateColumnXYChart("", "", resultDt, "DeptName", yAxies, serDefines, null);
            result.SetValue("chart", chart);
            return Json(result);
        }

        public Dictionary<string,object> GetChart()
        {
            return null;
        }

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
