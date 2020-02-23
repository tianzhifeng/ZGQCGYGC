using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using Project.Logic;
using Config;
using Config.Logic;
using Formula;
using MvcAdapter;
using System.Collections.Specialized;
using Formula.Helper;
using System.Data;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class QualityAnalyzeController : ProjectController<S_AE_Mistake>
    {

        public ActionResult DeptAnalyze()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;
            return View();
        }

        public ActionResult UserSummaryList()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;
            return View();
        }

        public ActionResult MajorDetailList()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoFO");
            return View();
        }

        public ActionResult ProjectAnalyze()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;

            var tab = new Tab();
            //var levelCategory = CategoryFactory.GetCategory("Market.CustomerLevel", "项目等级", "CustomerLevel");
            //levelCategory.SetDefaultItem();
            //tab.Categories.Add(levelCategory);

            //业务类型
            var category = CategoryFactory.GetCategory("Base.ProjectClass", "业务类型", "ProjectClass");
            category.SetDefaultItem();
            tab.Categories.Add(category);
            //主责部门
            category = CategoryFactory.GetCategory("Market.ManDept", "主责部门", "ChargeDeptID");
            category.SetDefaultItem();
            tab.Categories.Add(category);
            //设计阶段
            category = CategoryFactory.GetCategory("Project.Phase", "设计阶段", "PhaseValue");
            category.SetDefaultItem();
            tab.Categories.Add(category);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult PersonalAnalyze()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;
            var tab = new Tab();

            //var category = CategoryFactory.GetCategory("Market.ManDept", "部门", "DeptID");
            //category.SetDefaultItem();
            //tab.Categories.Add(category);

            var category = CategoryFactory.GetYearCategory("MistakeYear");
            category.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(category);

            category = CategoryFactory.GetMonthCategory("MistakeMonth");
            category.SetDefaultItem(DateTime.Now.Month.ToString());
            tab.Categories.Add(category);

            category = CategoryFactory.GetQuarterCategory("MistakeSeason");
            category.SetDefaultItem();
            tab.Categories.Add(category);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetPersonList(QueryBuilder qb)
        {
            var sql = @"
select u.ID,u.Name,u.Code,u.WorkNo,u.DeptID,u.DeptName,u.DeptFullID,mistake.*,summary.Summary
from {0}..S_A_User u
left join (
	select * from(
	select COUNT(*) MistakeCount,MistakeLevel,DesignerID from S_AE_Mistake
	where 1=1 {1}
	group by MistakeLevel,DesignerID) tmp pivot 
	(max(MistakeCount) for MistakeLevel in ({2}))t
) mistake 
on mistake.DesignerID = u.ID
left join (
	select COUNT(*) Summary,DesignerID from S_AE_Mistake
	where 1=1 {1}
	group by DesignerID
) summary
on summary.DesignerID = u.ID";

            var misQb = new QueryBuilder();
            foreach (var condition in qb.Items.Where(a => a.Field == "MistakeDate" || a.Field == "MistakeYear" || a.Field == "MistakeMonth" || a.Field == "MistakeSeason"))
            {
                if (condition.Field == "MistakeDate")
                    misQb.Add("CreateDate", condition.Method, condition.Value, condition.OrGroup, condition.BaseOrGroup);
                else
                    misQb.Add(condition.Field, condition.Method, condition.Value, condition.OrGroup, condition.BaseOrGroup);
            }
            qb.Items.RemoveWhere(a => a.Field == "MistakeDate");
            qb.Items.RemoveWhere(a => a.Field == "MistakeYear");
            qb.Items.RemoveWhere(a => a.Field == "MistakeMonth");
            qb.Items.RemoveWhere(a => a.Field == "MistakeSeason");
            string whereStr = misQb.GetWhereString(false);

            var mistakeLevels = String.Join(",", EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.Select(a => a.Code));
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sql = String.Format(sql, baseDB.DbName, whereStr, mistakeLevels);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            var misQb = new QueryBuilder();
            foreach (var condition in qb.Items.Where(a => a.Field == "MistakeDate"))
            {
                misQb.Add("CreateDate", condition.Method, condition.Value, condition.OrGroup, condition.BaseOrGroup);
            }
            qb.Items.RemoveWhere(a => a.Field == "MistakeDate");
            string misWhereStr = misQb.GetWhereString(false);
            string joinType = "left";
            if (!string.IsNullOrEmpty(misWhereStr))
            {
                joinType = "inner";
            }

            string sql = @"select prj.*,{1} from s_i_projectinfo prj
 {3} join (SELECT ProjectInfoID ,{0}{2} FROM 
(select ProjectInfoID,MistakeLevel,Count(1) MistakeCount from S_AE_Mistake
where 1=1 {4}
group by ProjectInfoID,MistakeLevel) tmp
PIVOT (Sum(MistakeCount) FOR MistakeLevel IN ({0}) ) AS tmp 
) mis on prj.id = mis.ProjectInfoID ";
            string fields = string.Empty, rowSumStr = string.Empty, filedsStr = string.Empty;
            string columnSumStr = string.Empty;
            var mistakeLevelEnum = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.ToList();
            foreach (var item in mistakeLevelEnum)
            {
                fields += item.Code + ",";
                filedsStr += "isnull(" + item.Code + ",0) " + item.Code + ",";
                rowSumStr += "isnull(" + item.Code + ",0)+";
                columnSumStr += "isnull(Sum(" + item.Code + "),0) " + item.Code + ",";
            }
            filedsStr += "isnull(Summary,0) Summary,";
            rowSumStr = ",(" + rowSumStr.TrimEnd('+') + ") Summary";
            columnSumStr += "isnull(Sum(Summary),0) Summary,";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            sql = String.Format(sql, fields.TrimEnd(','), filedsStr.TrimEnd(','), rowSumStr, joinType, misWhereStr);
            var result = db.ExecuteGridData(sql, qb);

            //合计
            string sumSql = @"select " + columnSumStr.TrimEnd(',') + " from (" + sql + qb.GetWhereString() + ") sumTableInfo ";
            var sumDt = this.SqlHelper.ExecuteDataTable(sumSql);
            if (sumDt.Rows.Count > 0)
            {
                var sumRow = sumDt.Rows[0];
                foreach (var item in mistakeLevelEnum)
                {
                    result.sumData.SetValue(item.Code, Convert.ToDecimal(sumRow[item.Code]));
                }
                result.sumData.SetValue("Summary", Convert.ToDecimal(sumRow["Summary"]));
            }
            return Json(result);
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string sql = @"select * from (select ID as ID,Name as DeptName,SortIndex,{1},0 as Summary,0 as Avg
from S_A_Org where  Type='{0}' ) TableInfo ";
            string fields = String.Empty;
            string seriesNames = String.Empty;
            string seriesFields = String.Empty;
            qb.PageSize = 0;
            var orgService = FormulaHelper.GetService<IOrgService>();
            var mistakeLevelEnum = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.ToList();
            foreach (var item in mistakeLevelEnum)
            {
                fields += " 0 as " + item.Code + ",";
                seriesFields += item.Code + ",";
                seriesNames += item.Name + ",";
            }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = db.ExecuteDataTable(String.Format(sql, OrgType.ManufactureDept.ToString(), fields.TrimEnd(',')));
            var mistakeList = this.entities.Set<S_AE_Mistake>().Where(qb).ToList();
            foreach (DataRow deptRow in dt.Rows)
            {
                double mistakeSum = 0;
                var deptID = deptRow["ID"].ToString();
                foreach (var item in mistakeLevelEnum)
                {
                    var count = mistakeList.Count(d => d.DeptID == deptID && d.MistakeLevel == item.Code);
                    mistakeSum += count;
                    deptRow[item.Code] = count;
                }
                deptRow["Summary"] = mistakeSum;
                double userCount = orgService.GetOrgUsers(deptRow["ID"].ToString()).Count;
                double avg = 0;
                if (userCount > 0)
                    avg = Math.Round(Convert.ToDouble(mistakeSum / userCount), 2);
                deptRow["Avg"] = avg;
            }
            var result = new List<Dictionary<string, object>>();

            var data = new Dictionary<string, object>();
            data.SetValue("data", dt);
            var pieChart = HighChartHelper.CreatePieChart("部门质量问题分析", "错误", dt, "DeptName", "Summary");
            data.SetValue("PieChart", pieChart.Render());
            var columnChart = HighChartHelper.CreateColumnChart("部门质量问题分类分析", dt, "DeptName", seriesNames.TrimEnd(',').Split(',')
                , seriesFields.TrimEnd(',').Split(','));
            data.SetValue("ColumnChart", columnChart.Render());
            return Json(data);
        }


        public Dictionary<string, object> CreateColumnChart(IList<Config.Org> orgList, List<S_AE_Mistake> mistakeList)
        {
            var columnChart = new Dictionary<string, object>();
            var chartPty = new Dictionary<string, object>();
            chartPty.SetValue("caption", "部门质量问题分类分析");
            chartPty.SetValue("rotatevalues", "1");
            chartPty.SetValue("placevaluesinside", "1");
            chartPty.SetValue("showvalues", "0");
            chartPty.SetValue("showlabels", "1");
            chartPty.SetValue("bgcolor", "FFFFFF");
            chartPty.SetValue("showBorder", "0");
            columnChart.SetValue("chart", chartPty);
            var mistakeLevelEnum = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.ToList();
            var categories = new List<Dictionary<string, object>>();
            var category = new Dictionary<string, object>();
            var lables = new List<Dictionary<string, object>>();
            var dataSet = new List<Dictionary<string, object>>();
            foreach (var org in orgList)
            {
                var label = new Dictionary<string, object>();
                label.SetValue("label", org.Name);
                lables.Add(label);
            }
            category.SetValue("category", lables);
            categories.Add(category);
            columnChart.SetValue("categories", categories);
            for (int i = 0; i < mistakeLevelEnum.Count; i++)
            {
                var mistakeLevel = mistakeLevelEnum[i];
                var data = new Dictionary<string, object>();
                var mistakeLevelList = new List<Dictionary<string, object>>();
                foreach (var org in orgList)
                {
                    var mistake = new Dictionary<string, object>();
                    var count = mistakeList.Count(d => d.DeptID == org.ID && d.MistakeLevel == mistakeLevel.Code);
                    mistake.SetValue("value", count);
                    mistakeLevelList.Add(mistake);
                }
                data.SetValue("color", colors[i]);
                data.SetValue("seriesname", mistakeLevel.Name);
                data.SetValue("data", mistakeLevelList);
                dataSet.Add(data);
            }
            columnChart.SetValue("dataset", dataSet);
            return columnChart;
        }


        public JsonResult GetDetailInfo(QueryBuilder qb)
        {
            var mistakeType = this.GetQueryString("MistakeType");
            qb.PageSize = 0;
            var query = this.entities.Set<S_AE_Mistake>().Where((SearchCondition)qb);

            var list = query.GroupBy(d => new { UserName = d.Designer, UserID = d.DesignerID, MistakeLevel = d.MistakeLevel })
                .Select(d => new { UserName = d.Key.UserName, UserID = d.Key.UserID, MistakeLevel = d.Key.MistakeLevel, MistakeCount = d.Count() }).ToList()
                .Where(d => d.MistakeLevel == mistakeType).OrderByDescending(d => d.MistakeCount).ToList();
            var dataGrid = new GridData(list);
            return Json(dataGrid);
        }

        public JsonResult GetMajorInfo(string ProjectInfoID, QueryBuilder qb)
        {
            qb.Items.Clear();
            string sql = @"select major.*,{1} from (select distinct wbsvalue  from s_w_wbs
where wbstype='{5}' and  ProjectInfoID='{3}') major
inner join (SELECT MajorCode ,{0}{2} FROM 
(select MajorCode,MistakeLevel,Count(1) MistakeCount from S_AE_Mistake
where ProjectInfoID='{3}' {4}
group by MajorCode,MistakeLevel) tmp
PIVOT (Sum(MistakeCount) FOR MistakeLevel IN ({0}) ) AS tmp 
) mis on major.wbsvalue = mis.MajorCode ";
            string fields = string.Empty, rowSumStr = string.Empty, filedsStr = string.Empty;
            string columnSumStr = string.Empty;
            var mistakeLevelEnum = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.ToList();
            foreach (var item in mistakeLevelEnum)
            {
                fields += item.Code + ",";
                filedsStr += "isnull(" + item.Code + ",0) " + item.Code + ",";
                rowSumStr += "isnull(" + item.Code + ",0)+";
                columnSumStr += "isnull(Sum(" + item.Code + "),0) " + item.Code + ",";
            }
            filedsStr += "isnull(Summary,0) Summary,";
            rowSumStr = ",(" + rowSumStr.TrimEnd('+') + ") Summary";
            columnSumStr += "isnull(Sum(Summary),0) Summary,";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            sql = String.Format(sql, fields.TrimEnd(','), filedsStr.TrimEnd(','), rowSumStr, ProjectInfoID, "", WBSNodeType.Major.ToString());
            var result = db.ExecuteGridData(sql, qb);

            //合计
            string sumSql = @"select " + columnSumStr.TrimEnd(',') + " from (" + sql + qb.GetWhereString() + ") sumTableInfo ";
            var sumDt = this.SqlHelper.ExecuteDataTable(sumSql);
            if (sumDt.Rows.Count > 0)
            {
                var sumRow = sumDt.Rows[0];
                foreach (var item in mistakeLevelEnum)
                {
                    result.sumData.SetValue(item.Code, Convert.ToDecimal(sumRow[item.Code]));
                }
                result.sumData.SetValue("Summary", Convert.ToDecimal(sumRow["Summary"]));
            }
            return Json(result);
        }

        public JsonResult GetUserSummaryInfo(QueryBuilder qb)
        {
            var list = this.entities.Set<S_AE_Mistake>().Where((SearchCondition)qb).ToList();
            var mistakeLevelEnum = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem.ToList();
            var userSummaryList = list.GroupBy(d => new { UserName = d.Designer, UserID = d.DesignerID }).Select(d => new { UserName = d.Key.UserName, UserID = d.Key.UserID, Summary = d.Count() }).ToList().OrderByDescending(d => d.Summary).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in userSummaryList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("UserID", item.UserID);
                dic.SetValue("UserName", item.UserName);
                foreach (var mistakeItem in mistakeLevelEnum)
                {
                    var count = list.Count(d => d.DesignerID == item.UserID && d.MistakeLevel == mistakeItem.Code);
                    dic.SetValue(mistakeItem.Code, count);
                }
                dic.SetValue("Summary", item.Summary);
                result.Add(dic);
            }
            var gridData = new GridData(result);
            return Json(gridData);
        }
        string[] colors = new string[] { "F6BD0F", "AFD8F8", "8BBA00" };
    }
}