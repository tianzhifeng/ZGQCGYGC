using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Reflection;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;
using Project.Logic;

namespace Project.Areas.Monitor.Controllers
{
    public class QualityInfoController : ProjectController<S_AE_Mistake>
    {
        public ActionResult QualityInfoList()
        {
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            ViewBag.MistakeLevel = mistakeLevels;

            string projectInfoID = this.Request["ProjectInfoID"];
            var list = this.entities.Set<S_AE_Mistake>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var majors = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == "Major").
                Select(c => new { MajorName = c.Name, WBSValue = c.WBSValue, SortIndex = c.SortIndex }).Distinct().OrderBy(d => d.SortIndex).ToList();

            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var list = this.entities.Set<S_AE_Mistake>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var result = new List<Dictionary<string, object>>();
            var mistakeLevels = EnumBaseHelper.GetEnumDef("Project.MistakeLevel").EnumItem;
            string fields = String.Empty;
            string seriesNames = String.Empty;
            string seriesFields = String.Empty;
            foreach (var item in mistakeLevels)
            {
                fields += ", 0 as " + item.Code;
                seriesFields += item.Code + ",";
                seriesNames += item.Name + ",";
            }

            //柱状图sql模型
            var sql = @"select distinct Name as MajorName,WBSValue,SortIndex {1},0 as Summary
from S_W_WBS wbs
where ProjectInfoID='{0}'
and WBSType = 'Major'
order by SortIndex";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, projectInfoID, fields));

            foreach (DataRow row in dt.Rows)
            {
                var wbsValue = row["WBSValue"].ToString();
                var majorName = row["MajorName"].ToString();
                var dic = new Dictionary<string, object>();
                dic.SetValue("MajorValue", wbsValue);
                dic.SetValue("MajorName", majorName);
                double mistakeSum = 0;
                foreach (var level in mistakeLevels)
                {
                    var mistakeCount = list.Count(d => d.MajorCode == wbsValue && d.MistakeLevel == level.Code);
                    dic.SetValue(level.Code, mistakeCount);
                    mistakeSum += mistakeCount;
                    row[level.Code] = mistakeCount;
                }
                dic.SetValue("Summary", mistakeSum);
                result.Add(dic);
            }

            var data = new Dictionary<string, object>();
            data.SetValue("data", result);

            #region 创建柱状图

            var columnChart = HighChartHelper.CreateColumnChart("专业质量问题分析", dt, "MajorName", seriesNames.TrimEnd(',').Split(',')
                , seriesFields.TrimEnd(',').Split(','));
            data.SetValue("ColumnChart", columnChart.Render());

            #endregion

            #region 创建饼图
            sql = @"select item.Code,item.Name,item.SortIndex,
	MisNum=(select count(0) from S_AE_Mistake mis 
			where mis.ProjectInfoID='{0}' and mis.MistakeLevel=item.Code)
from {1}.dbo.S_M_EnumDef def
left join {1}.dbo.S_M_EnumItem item
on def.ID=item.EnumDefID
where def.Code='Project.MistakeLevel'
order by item.SortIndex";
            dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, projectInfoID, SQLHelper.CreateSqlHelper(ConnEnum.Base).DbName));
            var pieChart = HighChartHelper.CreatePieChart("质量问题分类分析", "错误", dt, "Name", "MisNum");

            var chartOption = pieChart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartOption.SetValue("credits", credits);
            #endregion
            data.SetValue("PieChart", chartOption);
            return Json(data);
        }

        public JsonResult GetDetailInfo(QueryBuilder qb)
        {
            var mistakeType = this.GetQueryString("MistakeType");
            var query = this.entities.Set<S_AE_Mistake>().Where((SearchCondition)qb);
            var list = query.GroupBy(d => new { UserName = d.Designer, UserID = d.DesignerID, MistakeLevel = d.MistakeLevel })
                .Select(d => new { UserName = d.Key.UserName, UserID = d.Key.UserID, MistakeLevel = d.Key.MistakeLevel, MistakeCount = d.Count() }).ToList()
                .Where(d => d.MistakeLevel == mistakeType).OrderByDescending(d => d.MistakeCount).ToList();
            var dataGrid = new GridData(list);
            return Json(dataGrid);
        }

        public JsonResult GetAuditList(QueryBuilder qb)
        {
            var mistakeType = this.GetQueryString("MistakeType");
            var userID = this.GetQueryString("UserID");
            var majorCode = this.GetQueryString("MajorCode");
            var projectInfoID = this.GetQueryString("ProjectInfoID");

            var mistakes = this.entities.Set<S_AE_Mistake>().Where(d => d.ProjectInfoID == projectInfoID
                        && d.MajorCode == majorCode && d.DesignerID == userID && d.MistakeLevel == mistakeType).ToList();
            var auditIDs = mistakes.Select(d => d.AuditID).ToList();
            var auditList = this.entities.Set<T_EXE_Audit>().Where(d => auditIDs.Contains(d.ID));

            return Json(auditList);
        }

    }
}
