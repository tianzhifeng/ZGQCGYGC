using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using MvcAdapter;
using Project.Logic;
using Formula.Helper;
using Formula;
using Formula.DynConditionObject;
using Config;
using System.Data;
using Config.Logic;

namespace Project.Areas.Gantt.Controllers
{
    public class ProjectGroupMileStoneGanttController : ProjectController<S_P_MileStone>
    {
        public JsonResult GetMileStoneList(QueryBuilder qb)
        {
            var result = new List<Dictionary<string, object>>();
            var mainSql = this.GetProjectSql("*", qb);
            var mileStoneSql = this.GetMileStoneSql(qb);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var projectDt = sqlHelper.ExecuteDataTable(mainSql, qb);
            var mileStoneDt = sqlHelper.ExecuteDataTable(mileStoneSql);
            foreach (DataRow row in projectDt.Rows)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("UID", row["ID"]);
                dic.SetValue("Name", row["Name"]);
                dic.SetValue("Code", row["Code"]);
                dic.SetValue("ChargeUserName", row["ChargeUserName"]);
                var mileStones = mileStoneDt.Select(" ProjectInfoID='" + row["ID"] + "'");
                var mileStoneList = new List<Dictionary<string, object>>();
                foreach (DataRow mileStoneRow in mileStones)
                {
                    var node = new Dictionary<string, object>();
                    string name = mileStoneRow["Name"].ToString();
                    node.SetValue("UID", mileStoneRow["ID"]);
                    node.SetValue("Name", name.Length > 5 ? name.Substring(0, 5)+"..." : name);
                    node.SetValue("DisplayName", name);
                    node.SetValue("Start", Convert.ToDateTime(mileStoneRow["PlanFinishDate"]));
                    node.SetValue("Finish", Convert.ToDateTime(mileStoneRow["PlanFinishDate"]).AddDays(5));
                    var planDate = Convert.ToDateTime(mileStoneRow["PlanFinishDate"]);
                    if (mileStoneRow["ShowStatus"].ToString() == "NormalFinish")
                        node.SetValue("PercentComplete", "0");
                    else if (mileStoneRow["ShowStatus"].ToString() == "DelayFinish")
                        node.SetValue("PercentComplete", "50");
                    else if (mileStoneRow["ShowStatus"].ToString() == "NormalUndone")
                        node.SetValue("PercentComplete", "10");
                    else
                        node.SetValue("PercentComplete", "100");
                    node.SetValue("State", mileStoneRow["State"]);
                    node.SetValue("FactFinishDate", mileStoneRow["FactFinishDate"]);
                    node.SetValue("ProjectInfoName", row["Name"]);
                    mileStoneList.Add(node);
                    dic.SetValue("Tasks", mileStoneList);
                }
                result.Add(dic);
            }
            var data = new Dictionary<string, object>();
            data.SetValue("Total", qb.TotolCount);
            data.SetValue("CurrentPage", qb.PageIndex);
            data.SetValue("data", result);
            data.SetValue("DelayFinishCount", mileStoneDt.Compute(" Count(ID) ", " ShowStatus='" + MileStoneShowStatus.DelayFinish.ToString() + "' "));
            data.SetValue("DelayUndoneCount", mileStoneDt.Compute(" Count(ID) ", " ShowStatus='" + MileStoneShowStatus.DelayUndone.ToString() + "' "));
            data.SetValue("NormalFinishCount", mileStoneDt.Compute(" Count(ID) ", " ShowStatus='" + MileStoneShowStatus.NormalFinish.ToString() + "' "));
            data.SetValue("NormalUndoneCount", mileStoneDt.Compute(" Count(ID) ", " ShowStatus='" + MileStoneShowStatus.NormalUndone.ToString() + "' "));
            return Json(data);
        }

        private string GetProjectSql(string fileds, QueryBuilder qb)
        {
            string result = @"select " + fileds + " from S_I_ProjectInfo where 1=1 ";
            string whereStr = ((SearchCondition)qb).GetWhereString(false);
            if (!String.IsNullOrEmpty(whereStr)) result += "   " + whereStr;
            var queryData = JsonHelper.ToObject(this.Request["queryFormData"]);
            if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart")) || !String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateEnd")))
            {
                var subwhereString = "and (select Count(0) from S_P_MileStone where ProjectInfoID=S_I_ProjectInfo.ID {0})>0";
                if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart"))
                    && !String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateEnd")))
                    subwhereString = String.Format(subwhereString, " and PlanFinishDate>='" + queryData.GetValue("MileStonePlanDateStart")
                        + "' and PlanFinishDate<='" + queryData.GetValue("MileStonePlanDateEnd") + "'");
                else if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart")))
                    subwhereString = String.Format(subwhereString, " and PlanFinishDate>='" + queryData.GetValue("MileStonePlanDateStart") + "' ");
                else
                    subwhereString = String.Format(subwhereString, " and PlanFinishDate<='" + queryData.GetValue("MileStonePlanDateEnd") + "' ");
                result += subwhereString;
            }
            return result;
        }

        private string GetMileStoneSql(QueryBuilder qb)
        {
            var queryData = JsonHelper.ToObject(this.Request["queryFormData"]);
            string mainSql = this.GetProjectSql("ID", qb);
            string result = @"select * ,Case when State='Finish' and FactFinishDate<=PlanFinishDate then 'NormalFinish' 
when State='Finish' and FactFinishDate>=PlanFinishDate then 'DelayFinish' 
when State!='Finish' and getdate()<=PlanFinishDate then 'NormalUndone' 
when State!='Finish' and getdate()>PlanFinishDate then 'DelayUndone' 
else 'NormalUndone' end as ShowStatus from S_P_MileStone where MileStoneType='" + MileStoneType.Normal.ToString() + "' and  ProjectInfoID in (" + mainSql + ") and PlanFinishDate is not null";
            if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart")) || !String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateEnd")))
            {
                if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart")) && !String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateEnd")))
                {
                    result += " and PlanFinishDate>='" + queryData.GetValue("MileStonePlanDateStart") + "' and PlanFinishDate<='" + queryData.GetValue("MileStonePlanDateEnd") + "'";
                }
                else if (!String.IsNullOrEmpty(queryData.GetValue("MileStonePlanDateStart")))
                {
                    result += " and PlanFinishDate>='" + queryData.GetValue("MileStonePlanDateStart") + "' ";
                }
                else
                {
                    result += " and PlanFinishDate<='" + queryData.GetValue("MileStonePlanDateEnd") + "' ";
                }
            }
            return result;
        }
    }
}
