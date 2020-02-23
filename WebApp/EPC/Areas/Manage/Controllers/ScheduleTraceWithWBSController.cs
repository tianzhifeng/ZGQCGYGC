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

namespace EPC.Areas.Manage.Controllers
{
    public class ScheduleTraceWithWBSController : EPCController
    {
        public ActionResult MainTab()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的项目信息，无法跟踪计划");
            var defines = engineeringInfo.Mode.S_C_ScheduleDefine.OrderBy(c => c.SortIndex).ToList();
            ViewBag.ScheduleDefines = defines;
            ViewBag.EngineeringInfo = engineeringInfo;
            return View();
        }

        public ActionResult TraceList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            if (engineeringInfo.Mode == null) { throw new Formula.Exceptions.BusinessValidationException("工程未关联任何管理模式，请联系管理员"); }
            string Code = this.GetQueryString("ScheduleCode");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var version = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code && c.FlowPhase == "End").
                OrderByDescending(c => c.ID).FirstOrDefault();
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            var attrDefine = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(define.AttrDefine)) attrDefine = JsonHelper.ToList(define.AttrDefine);
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            ViewBag.Define = define;

            if (version == null)
            {
                ViewBag.FlowPhase = "";
                ViewBag.VersionNo = "0";
                ViewBag.VersionID = "";
            }
            else
            {
                ViewBag.FlowPhase = version.FlowPhase;
                ViewBag.VersionNo = version.VersionNumber;
                ViewBag.VersionID = version.ID;
            }
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;
            ViewBag.DefineID = define.ID;
            var nodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
            if (nodeTypeEnum == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【Base.WBSType】的枚举"); }
            var nodeTypeList = define.S_C_ScheduleDefine_Nodes.Where(c => c.NodeType != WBSConst.taskNodeType && c.Visible == "1").Select(c => c.NodeType).Distinct().ToList();
            var list = nodeTypeEnum.EnumItem.Where(c => nodeTypeList.Contains(c.Code)).OrderBy(c => c.SortIndex).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i + 1);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 1)
                {
                    ViewBag.ExpandLevel = i + 1;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);
            return View();
        }

        public ActionResult Gantte()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            if (engineeringInfo.Mode == null) { throw new Formula.Exceptions.BusinessValidationException("工程未关联任何管理模式，请联系管理员"); }
            string versionID = this.GetQueryString("VersionID");
            string Code = this.GetQueryString("ScheduleCode");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var version = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code && c.FlowPhase == "End").
            OrderByDescending(c => c.ID).FirstOrDefault();
            bool flowEnd = true;
            if (version == null)
            {
                ViewBag.VersionID = "";
                ViewBag.FlowPhase = "";
                ViewBag.VersionNo = "0";
            }
            else
            {
                if (version.FlowPhase != "End")
                    flowEnd = false;
                ViewBag.FlowPhase = version.FlowPhase;
                ViewBag.VersionID = version.ID;
                ViewBag.VersionNo = version.VersionNumber;
            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.ScheduleCode = Code;
            ViewBag.DefineID = define.ID;

            var nodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
            if (nodeTypeEnum == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【Base.WBSType】的枚举"); }
            var nodeTypeList = define.S_C_ScheduleDefine_Nodes.Where(c => c.NodeType != WBSConst.taskNodeType && c.Visible == "1").Select(c => c.NodeType).Distinct().ToList();
            var list = nodeTypeEnum.EnumItem.Where(c => nodeTypeList.Contains(c.Code)).OrderBy(c => c.SortIndex).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i + 1);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 1)
                {
                    ViewBag.ExpandLevel = i + 1;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);
            ViewBag.HolidayTable = JsonHelper.ToJson(HolidayHelper.GetHolidayTable());
            return View();
        }

        public JsonResult GetTreeList(string EngineeringInfoID, string DefineID)
        {
            string sql = @"  select * from (select ID,ParentID,FullID,EngineeringInfoID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight], SortIndex,'' as PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo from S_I_WBS
union select ID,ParentID,WBSFullID as FullID,EngineeringInfoID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
 from S_I_WBS_Task ) TableInfo  
 inner join (select StructInfoID as DefineID,SortIndex as DSortIndex,
 case when IsLocked is null then 0 else IsLocked end as IsLocked,
 case when Visible is null then 0 else Visible end as Visible,
 case when CanAdd is null then 0 else CanAdd end as CanAdd,
 case when CanEdit is null then 0 else CanEdit end as CanEdit,
 case when CanDelete is null then 0 else CanDelete end as CanDelete,
case when IsEnum is null then 0 else CanDelete end as IsEnum
from {2}..S_C_ScheduleDefine_Nodes
 where DefineID='{1}') DefineSortIndex
 on DefineSortIndex.DefineID=TableInfo.StructInfoID  where EngineeringInfoID = '{0}'   and Visible=1 
 order by DSortIndex,SortIndex";
            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, DefineID, infDb.DbName));
            return Json(dt);
        }

        public JsonResult GetGantteTree(string EngineeringInfoID, string DefineID)
        {
            string sql = @"  select * from (select ID as TID, ID as [UID],ParentID,ParentID as ParentTaskUID,FullID,EngineeringInfoID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate as Start,PlanEndDate as Finish,
CalPlanStartDate,CalPlanEndDate,PlanDuration as Duration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight], SortIndex,'' as PredecessorLink,0  as Milestone from S_I_WBS
union select ID as TID, ID as [UID],ParentID,ParentID as ParentTaskUID,WBSFullID as FullID,EngineeringInfoID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate as Start,PlanEndDate as Finish,
CalPlanStartDate,CalPlanEndDate,PlanDuration as Duration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,PredecessorLink,case when TaskType='MileStone' then 1 else 0 end as Milestone
 from S_I_WBS_Task ) TableInfo  
 inner join (select StructInfoID as DefineID,SortIndex as DSortIndex,
 case when IsLocked is null then 0 else IsLocked end as IsLocked,
 case when Visible is null then 0 else Visible end as Visible,
 case when CanAdd is null then 0 else CanAdd end as CanAdd,
 case when CanEdit is null then 0 else CanEdit end as CanEdit,
 case when CanDelete is null then 0 else CanDelete end as CanDelete,
case when IsEnum is null then 0 else CanDelete end as IsEnum
from {2}..S_C_ScheduleDefine_Nodes
 where DefineID='{1}') DefineSortIndex
 on DefineSortIndex.DefineID=TableInfo.StructInfoID  where EngineeringInfoID = '{0}'  
 order by DSortIndex,SortIndex";
            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, DefineID, infDb.DbName));
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(row);
                var list = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(row["PredecessorLink"].ToString()))
                {
                    list = JsonHelper.ToList(row["PredecessorLink"].ToString());
                }
                dic.SetValue("PredecessorLink", list);
                result.Add(dic);
            }
            return Json(result);
        }
    }
}
