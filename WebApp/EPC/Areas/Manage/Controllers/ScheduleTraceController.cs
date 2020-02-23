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
    public class ScheduleTraceController : EPCController
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
            bool flowEnd = true; bool isFirst = false;
            var attrDefine = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(define.AttrDefine)) attrDefine = JsonHelper.ToList(define.AttrDefine);
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            ViewBag.Define = define;

            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
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
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;

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
            S_I_WBS_Version version = null;
            string Code = this.GetQueryString("ScheduleCode");
            if (String.IsNullOrEmpty(versionID))
            {
                version = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code && c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
            }
            else
            {
                version = this.entities.Set<S_I_WBS_Version>().FirstOrDefault(c => c.ID == versionID);
                Code = version.ScheduleCode;
            }
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            bool flowEnd = true;
            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
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
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;

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

        public JsonResult GetVersionTreeList(string VersionID, string ShowType)
        {
            string sql = @" select * from (select ID,WBSID,ParentID,FullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight], SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'0' as IsEnum,IsLocked,
case when IsLocked ='True' then PlanStartDate else null end as BaseStart,
case when IsLocked ='True' then PlanEndDate else null end as BaseFinish,'' as PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
from S_I_WBS_Version_Node
union select ID,TaskID as WBSID,ParentID as WBSParentID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'0' as IsEnum,'False' as IsLocked,
null as BaseStart, null as BaseFinish,PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
 from S_I_WBS_Version_Task ) TableInfo  
 left join (select ID as DefineID,SortIndex as DSortIndex from {2}..S_C_WBSStruct) DefineSortIndex
 on DefineSortIndex.DefineID=TableInfo.StructInfoID  where VersionID = '{0}' and ModifyState!='{1}' order by DSortIndex,SortIndex";
            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, VersionID, BomVersionModifyState.Remove.ToString(), infDb.DbName));
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");
            var defineNodes = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var structInfoID = row["StructInfoID"] == null || row["StructInfoID"] == DBNull.Value ? "" : row["StructInfoID"].ToString();
                var defineNode = defineNodes.FirstOrDefault(c => c.StructInfoID == structInfoID);
                if (defineNode != null)
                {
                    if (defineNode.Visible != "1")
                    {
                        continue;
                    }
                    row["CanDelete"] = String.IsNullOrEmpty(defineNode.CanDelete) ? "0" : defineNode.CanDelete;
                    row["CanEdit"] = String.IsNullOrEmpty(defineNode.CanEdit) ? "0" : defineNode.CanEdit;
                    row["IsEnum"] = String.IsNullOrEmpty(defineNode.IsEnum) ? "0" : defineNode.IsEnum;
                }
                var dic = FormulaHelper.DataRowToDic(row);
                result.Add(dic);
            }
            return Json(result);
        }

        public JsonResult GetGantteTree(string VersionID, string ShowType)
        {
            string sql = @"  select * from (select ID as TID,WBSID as [UID], WBSID,ParentID,ParentID as ParentTaskUID,FullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate as Start,PlanEndDate as Finish,
CalPlanStartDate,CalPlanEndDate,PlanDuration as Duration,StrandardDuration,FactStartDate as FactStart,FactEndDate as FactFinish,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
0  as Milestone,
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,IsLocked,
case when IsLocked ='True' then PlanStartDate else null end as BaseStart,
case when IsLocked ='True' then PlanEndDate else null end as BaseFinish,'' as PredecessorLink
from S_I_WBS_Version_Node
union select ID as TID,TaskID as WBSID,TaskID as [UID],ParentID,ParentID as ParentTaskUID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate as Start,PlanEndDate as Finish,
CalPlanStartDate,CalPlanEndDate,PlanDuration as Duration,StrandardDuration,FactStartDate as FactStart,FactEndDate as FactFinish,CalProgress,Progress,
TaskType,TaskTypeName,
case when TaskType='MileStone' then 1 else 0 end as Milestone,
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'False' as IsLocked,
null as BaseStart, null as BaseFinish,PredecessorLink
 from S_I_WBS_Version_Task ) TableInfo   
 left join (select ID as DefineID,SortIndex as DSortIndex from {2}..S_C_WBSStruct) DefineSortIndex
 on DefineSortIndex.DefineID=TableInfo.StructInfoID 
where VersionID = '{0}' and ModifyState!='{1}' order by DSortIndex,SortIndex";
            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, VersionID, BomVersionModifyState.Remove.ToString(), infDb.DbName));
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");
            var defineNodes = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var structInfoID = row["StructInfoID"] == null || row["StructInfoID"] == DBNull.Value ? "" : row["StructInfoID"].ToString();
                var defineNode = defineNodes.FirstOrDefault(c => c.StructInfoID == structInfoID);
                if (defineNode != null)
                {
                    row["Visible"] = String.IsNullOrEmpty(defineNode.Visible) ? "0" : defineNode.Visible;
                    row["CanDelete"] = String.IsNullOrEmpty(defineNode.CanDelete) ? "0" : defineNode.CanDelete;
                    row["CanEdit"] = String.IsNullOrEmpty(defineNode.CanEdit) ? "0" : defineNode.CanEdit;
                }
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
