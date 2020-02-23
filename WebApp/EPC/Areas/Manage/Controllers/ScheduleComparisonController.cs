using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;

namespace EPC.Areas.Manage.Controllers
{
    public class ScheduleComparisonController : EPCController
    {
        public ActionResult List()
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(this.GetQueryString("EngineeringInfoID"));
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没能找到指定的工程信息");
            var scheduleDefines = engineeringInfo.Mode.S_C_ScheduleDefine.OrderBy(c => c.SortIndex).ToList();
            var scheduleDefinesEnum = new List<Dictionary<string, object>>();
            foreach (var item in scheduleDefines)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("text", item.Name);
                dic.SetValue("value", item.Code);
                scheduleDefinesEnum.Add(dic);
            }
            ViewBag.ScheduleDefine = JsonHelper.ToJson(scheduleDefinesEnum);
            return View();
        }

        public JsonResult GetVersionList(string EngineeringInfoID, string ScheduleCode)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("");
            var versinList = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == ScheduleCode && c.FlowPhase == "End").OrderBy(c => c.ID).ToList();
            var result = versinList.Select(c => new { text = "第【" + c.VersionNumber + "】版", value = c.ID }).ToList();
            return Json(result);
        }

        public JsonResult GetComparisonList(string EngineeringInfoID, string SourceID, string TargetID, string ShowType)
        {


            var result = new DataTable();
            result.Columns.Add("ID");
            result.Columns.Add("ParentID");
            result.Columns.Add("Name");
            result.Columns.Add("NodeType");
            result.Columns.Add("SortIndex");
            result.Columns.Add("PlanDuration", typeof(decimal));
            result.Columns.Add("PlanStartDate", typeof(DateTime));
            result.Columns.Add("PlanEndDate", typeof(DateTime));
            result.Columns.Add("TargetPlanDuration", typeof(decimal));
            result.Columns.Add("TargetPlanStartDate", typeof(DateTime));
            result.Columns.Add("TargetPlanEndDate", typeof(DateTime));
            result.Columns.Add("Diff");
            result.Columns.Add("DiffField");

            string sql = @"select * from (
select * from (select WBSID as ID,ParentID,Name,NodeType,SortIndex,[Weight],PlanDuration,
PlanStartDate,PlanEndDate,VersionID,StructInfoID from S_I_WBS_Version_Node  where ModifyState!='{1}') NodeInfo
union select * from (
select TaskID as ID,ParentID,Name,'Task' as NodeType,SortIndex,[Weight],PlanDuration,
PlanStartDate,PlanEndDate,VersionID,StructInfoID from S_I_WBS_Version_Task  where ModifyState!='{1}') TaskInfo
) TableInfo where VersionID in ('{0}')";

            var dataTable = this.SqlHelper.ExecuteDataTable(String.Format(sql, SourceID + "','" + TargetID, BomVersionModifyState.Remove.ToString()));

            var version = this.GetEntityByID<S_I_WBS_Version>(SourceID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本信息，无法进行比较");
            var definesNodes = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.ToList();

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["VersionID"] == null || row["VersionID"] == DBNull.Value || String.IsNullOrEmpty(row["VersionID"].ToString()))
                    continue;
                if (row["StructInfoID"] == null || row["StructInfoID"] == DBNull.Value || String.IsNullOrEmpty(row["StructInfoID"].ToString()))
                    continue;
                var versionID = row["VersionID"].ToString();
                var structInfoID = row["StructInfoID"].ToString();
                var defineNode = definesNodes.FirstOrDefault(c => c.StructInfoID == structInfoID);
                if (defineNode.Visible != "1") continue;
                var ID = row["ID"].ToString();
                var resultRows = result.Select("ID='" + row["ID"] + "'");
                var key = "";
                if (versionID == TargetID)
                {
                    key = "Target";
                }
                if (resultRows.Length == 0)
                {
                    var resultRow = result.NewRow();
                    resultRow["ID"] = row["ID"];
                    resultRow["ParentID"] = row["ParentID"];
                    resultRow["Name"] = row["Name"];
                    resultRow["NodeType"] = row["NodeType"];
                    resultRow["SortIndex"] = row["SortIndex"];
                    resultRow["Diff"] = false.ToString();
                    resultRow[key + "PlanDuration"] = row["PlanDuration"];
                    resultRow[key + "PlanStartDate"] = row["PlanStartDate"];
                    resultRow[key + "PlanEndDate"] = row["PlanEndDate"];
                    result.Rows.Add(resultRow);
                }
                else
                {
                    var resultRow = resultRows.FirstOrDefault();
                    resultRow[key + "PlanDuration"] = row["PlanDuration"];
                    resultRow[key + "PlanStartDate"] = row["PlanStartDate"];
                    resultRow[key + "PlanEndDate"] = row["PlanEndDate"];
                }
            }

            foreach (DataRow item in result.Rows)
            {
                var diffField = "";
                var PlanDuration = item["PlanDuration"] == null || item["PlanDuration"] == DBNull.Value ? 0 : Convert.ToDecimal(item["PlanDuration"]);
                var TargetPlanDuration = item["TargetPlanDuration"] == null || item["TargetPlanDuration"] == DBNull.Value ? 0 : Convert.ToDecimal(item["TargetPlanDuration"]);
                if (PlanDuration != TargetPlanDuration)
                {
                    item["Diff"] = true.ToString();
                    diffField += "PlanDuration,TargetPlanDuration,";
                }
                var PlanStartDate = item["PlanStartDate"] == null || item["PlanStartDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(item["PlanStartDate"]);
                var TargetPlanStartDate = item["TargetPlanStartDate"] == null || item["TargetPlanStartDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(item["TargetPlanStartDate"]);
                if (PlanStartDate != TargetPlanStartDate)
                {
                    item["Diff"] = true.ToString();
                    diffField += "PlanStartDate,TargetPlanStartDate,";
                }

                var PlanEndDate = item["PlanEndDate"] == null || item["PlanEndDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(item["PlanEndDate"]);
                var TargetPlanEndDate = item["TargetPlanEndDate"] == null || item["TargetPlanEndDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(item["TargetPlanEndDate"]);
                if (PlanEndDate != TargetPlanEndDate)
                {
                    item["Diff"] = true.ToString();
                    diffField += "PlanEndDate,TargetPlanEndDate,";
                }
                item["DiffField"] = diffField.TrimEnd(',');
            }
            return Json(result);
        }
    }
}
