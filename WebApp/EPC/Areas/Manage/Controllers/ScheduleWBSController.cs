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
using System.Text.RegularExpressions;

namespace EPC.Areas.Manage.Controllers
{
    public class ScheduleWBSController : EPCController
    {
        public ActionResult Tab()
        {
            string codes = this.GetQueryString("ScheduleCodes");

            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var defines = engineeringInfo.Mode.S_C_ScheduleDefine.Where(c => codes.Contains(c.Code)).ToList();
            ViewBag.ScheduleDefines = defines;
            ViewBag.EngineeringInfo = engineeringInfo;
            return View();
        }

        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            if (engineeringInfo.Mode == null) { throw new Formula.Exceptions.BusinessValidationException("工程未关联任何管理模式，请联系管理员"); }
            string Code = this.GetQueryString("ScheduleCode");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            bool flowEnd = engineeringInfo.S_I_WBS_Version.Count(c => c.ScheduleCode == Code && c.FlowPhase != "End") == 0 ? true : false;

            bool isFirst = false;
            var attrDefine = new List<Dictionary<string, object>>();
            var readOnlyFields = string.Empty;
            if (!String.IsNullOrEmpty(define.ColDefine))
            {
                attrDefine = JsonHelper.ToList(define.ColDefine);
                attrDefine = attrDefine.OrderBy(c => Convert.ToInt32(c["sortIndex"])).ToList();
                foreach (var item in attrDefine)
                {
                    if (item.GetValue("editable") != "true")
                    {
                        readOnlyFields += item.GetValue("fieldName") + ",";
                    }
                }
            }
            var verion = engineeringInfo.S_I_WBS_Version.OrderByDescending(c => c.ID).FirstOrDefault();
            if (verion == null)
            {
                ViewBag.VersionID = "";
                ViewBag.VersionNo = "0";
            }
            else
            {
                ViewBag.VersionID = verion.ID;
                ViewBag.VersionNo = verion.VersionNumber;
            }

            var extendFieldDefine = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(define.AttrDefine))
            {
                extendFieldDefine = JsonHelper.ToList(define.AttrDefine);
            }
            ViewBag.ExtendFieldDefine = JsonHelper.ToJson(extendFieldDefine);
            ViewBag.ReadonlyField = readOnlyFields.TrimEnd(',');
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            ViewBag.Define = define;
            ViewBag.ImportProject = define.ImportProject;
            ViewBag.ImportExcel = define.ImportExcel;
            ViewBag.ImportBOM = define.ImportBOM;
            ViewBag.ImportQBS = define.ImportQBS;
            ViewBag.ImportBid = define.ImportBid;
            ViewBag.ImportTaskTemplate = define.ImportTaskTemplate;
            if (!define.ImportProject && !define.ImportBOM && !define.ImportQBS && !define.ImportBid
                && !define.ImportTaskTemplate && !define.ImportExcel)
            {
                ViewBag.ShowImportButton = false;
            }
            else
            {
                ViewBag.ShowImportButton = true;
            }
            ViewBag.CanStart = true;
            if (!String.IsNullOrEmpty(define.PreScheduleCode))
            {
                var preDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == define.PreScheduleCode);
                if (preDefine == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + define.PreScheduleCode + "】的计划定义视图，请联系管理员"); }
                if (engineeringInfo.S_I_WBS_Version.Count(c => c.ScheduleCode == preDefine.Code && c.FlowPhase == "End") == 0)
                {
                    ViewBag.CanStart = false;
                    ViewBag.ErrorMsg = "【" + preDefine.Name + "】计划尚未发布，无法编制本级计划";
                }

            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;
            ViewBag.DefineID = define.ID;

            var nodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
            if (nodeTypeEnum == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【Base.WBSType】的枚举"); }
            var nodeTypeList = define.S_C_ScheduleDefine_Nodes.Where(c => c.NodeType
                != WBSConst.taskNodeType && c.Visible == "1").Select(c => c.NodeType).Distinct().ToList();
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

            #region 是否启用虚加载

            int wbsNodeCount = 0, wbsTaskCount = 0;
            var wbsNodeCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_I_WBS with(nolock) WHERE EngineeringInfoID='" + engineeringInfo.ID + "'");
            var wbsTaskCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_I_WBS_Task with(nolock) WHERE EngineeringInfoID='" + engineeringInfo.ID + "'");
            int.TryParse((wbsNodeCountObj ?? "").ToString(), out wbsNodeCount);
            int.TryParse((wbsTaskCountObj ?? "").ToString(), out wbsTaskCount);
            ViewBag.VirtualScroll = "false";
            if ((wbsTaskCount + wbsNodeCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }
            #endregion
            return View();
        }

        public ActionResult Gantte()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            if (engineeringInfo.Mode == null) { throw new Formula.Exceptions.BusinessValidationException("工程未关联任何管理模式，请联系管理员"); }
            string Code = this.GetQueryString("ScheduleCode");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            bool flowEnd = engineeringInfo.S_I_WBS_Version.Count(c => c.ScheduleCode == Code && c.FlowPhase != "End") == 0 ? true : false;
            bool isFirst = false;
            var attrDefine = new List<Dictionary<string, object>>();
            var readOnlyFields = string.Empty;
            if (!String.IsNullOrEmpty(define.ColDefine))
            {
                attrDefine = JsonHelper.ToList(define.ColDefine);
                attrDefine = attrDefine.OrderBy(c => Convert.ToInt32(c["sortIndex"])).ToList();
                foreach (var item in attrDefine)
                {
                    if (item.GetValue("editable") != "true")
                    {
                        readOnlyFields += item.GetValue("fieldName") + ",";
                    }
                }
            }
            ViewBag.VersionNo = String.IsNullOrEmpty(engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code).Max(c => c.VersionNumber)) ? "0" :
                engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code).Max(c => c.VersionNumber);
            ViewBag.ReadonlyField = readOnlyFields.TrimEnd(',');
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            ViewBag.Define = define;
            ViewBag.ImportProject = define.ImportProject;
            ViewBag.ImportExcel = define.ImportExcel;
            ViewBag.ImportBOM = define.ImportBOM;
            ViewBag.ImportQBS = define.ImportQBS;
            ViewBag.ImportBid = define.ImportBid;
            ViewBag.ImportTaskTemplate = define.ImportTaskTemplate;
            if (!define.ImportProject && !define.ImportBOM && !define.ImportQBS && !define.ImportBid
                && !define.ImportTaskTemplate && !define.ImportExcel)
            {
                ViewBag.ShowImportButton = false;
            }
            else
            {
                ViewBag.ShowImportButton = true;
            }
            ViewBag.CanStart = true;
            if (!String.IsNullOrEmpty(define.PreScheduleCode))
            {
                var preDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == define.PreScheduleCode);
                if (preDefine == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + define.PreScheduleCode + "】的计划定义视图，请联系管理员"); }
                if (engineeringInfo.S_I_WBS_Version.Count(c => c.ScheduleCode == preDefine.Code && c.FlowPhase == "End") == 0)
                {
                    ViewBag.CanStart = false;
                    ViewBag.ErrorMsg = "【" + preDefine.Name + "】计划尚未发布，无法编制本级计划";
                }

            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;
            ViewBag.DefineID = define.ID;

            var nodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
            if (nodeTypeEnum == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【Base.WBSType】的枚举"); }
            var nodeTypeList = define.S_C_ScheduleDefine_Nodes.Where(c => c.NodeType
                != WBSConst.taskNodeType && c.Visible == "1").Select(c => c.NodeType).Distinct().ToList();
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
case when IsEnum is null then 0 else IsEnum end as IsEnum
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

        public JsonResult GetNodeType(string ParentID, string EngineeringInfoID, string DefineID)
        {
            var result = new List<Dictionary<string, object>>();
            var enginneeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (enginneeringInfo == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var node = enginneeringInfo.S_I_WBS.FirstOrDefault(c => c.ID == ParentID);
            if (node == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var define = enginneeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (define == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var parentDefineNode = define.S_C_ScheduleDefine_Nodes.FirstOrDefault(c => c.StructInfoID == node.StructInfoID);
            var structList = parentDefineNode.Children;
            foreach (var item in structList)
            {
                if (item.CanAdd != "1") continue;
                if (item.NodeType == WBSConst.taskNodeType)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("value", item.TaskType);
                    dic.SetValue("text", EnumBaseHelper.GetEnumDescription(typeof(TaskType), item.TaskType));
                    result.Add(dic);
                }
                else
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("value", item.NodeType);
                    dic.SetValue("text", item.Name);
                    result.Add(dic);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNodesInfo(string ID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(ID);
            if (wbs == null) { return Json("", JsonRequestBehavior.AllowGet); }
            string sql = @"select Name as text,Code as value,SortIndex from S_T_WBSAttrDefine
where TypeDefineID in (select ID from dbo.S_T_WBSTypeDefine
where Code='{0}')   order by SortIndex";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = db.ExecuteDataTable(String.Format(sql, wbs.NodeType));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTemplateList(QueryBuilder qb, string ScheduleCode)
        {
            var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var data = dbContext.Set<S_T_TaskTemplate>().Where(c => c.ScheduleCode.Contains(ScheduleCode)).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetTemplateDetail(QueryBuilder qb, string TemplateID)
        {
            var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            qb.PageSize = 0;
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            var data = dbContext.Set<S_T_TaskTemplate_Detail>().Where(c => c.TemplateID == TemplateID).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult ImportTemplate(string EngineeringInfoID, string DefineID, string TemplateID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            engineeringInfo.ImportTemplateToWBS(TemplateID, DefineID);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportTaskTemplateDetail(string EngineeringInfoID, string DefineID, string ListData)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法导入");
            var scheduleDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (scheduleDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定计划定义，无法导入");
            var infrasEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detailID = item.GetValue("ID");
                var detail = infrasEntities.S_T_TaskTemplate_Detail.Find(detailID);
                if (detail != null)
                {
                    engineeringInfo.ImportTaskTemplateDetailToWBS(detail, scheduleDefine);
                }
            }
            engineeringInfo.SetWBSFromTask(scheduleDefine);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPBom(string EngineeringInfoID, string ListData, string DefineID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法导入");
            var scheduleDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (scheduleDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定计划定义，无法导入");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detailID = item.GetValue("ID");
                var detail = this.entities.Set<S_P_Bom>().Find(detailID);
                if (detail != null)
                {
                    engineeringInfo.ImportPBOMToWBS(detail, scheduleDefine);
                }
            }
            engineeringInfo.SetWBSFromTask(scheduleDefine);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFromQBS(string QBSData, string EngineeringInfoID, string NodeID, string DefineID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法导入QBS");
            var wbs = engineeringInfo.S_I_WBS.FirstOrDefault(c => c.ID == NodeID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("必须选中一个WBS节点才能进行导入");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义，无法导入");
            var qbsList = JsonHelper.ToList(QBSData);
            foreach (var item in qbsList)
            {
                var qbs = this.GetEntityByID<S_Q_QBS>(item.GetValue("ID"));
                engineeringInfo.ImportFromQBSToWBS(qbs, wbs, define);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddEmptyNode(string ParentNodeID, string EngineeringInfoID, string DefineID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法新增节点");
            }
            var scheduleDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (scheduleDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义，无法新增节点");
            var parentNode = engineeringInfo.S_I_WBS.FirstOrDefault(c => c.ID == ParentNodeID);
            if (parentNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("作业下不能新增节点，只有WBS才能新增子节点");
            }
            var dic = parentNode.AddEmptyChild(scheduleDefine);
            dic.SetValue("TID", dic.GetValue("ID"));

            this.entities.SaveChanges();
            var id = dic.GetValue("ID");
            var defineStructNodeIDs = scheduleDefine.S_C_ScheduleDefine_Nodes.Select(c => c.StructInfoID).ToList();
            var children = engineeringInfo.S_I_WBS.Where(c => c.ParentID == id && defineStructNodeIDs.Contains(c.StructInfoID)).ToList();
            if (children.Count > 0)
            {
                var childList = new List<Dictionary<string, object>>();
                foreach (var item in children)
                {
                    var childItem = FormulaHelper.ModelToDic<S_I_WBS>(item);
                    childItem.SetValue("NodeType", item.NodeType);
                    childItem.SetValue("TaskType", item.NodeType);
                    childList.Add(childItem);
                }
                dic.SetValue("children", childList);
            }
            return Json(dic);
        }

        public JsonResult InsertEmptyNode(string NodeID, string EngineeringInfoID, string DefineID, string NodeType)
        {
            var dic = new Dictionary<string, object>();
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法新增节点");
            }
            var scheduleDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (scheduleDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义，无法新增节点");
            if (NodeType == "Task")
            {
                var task = engineeringInfo.S_I_WBS_Task.FirstOrDefault(c => c.ID == NodeID);
                if (task == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的作业界面，无法插入作业");
                }
                var parentNode = engineeringInfo.S_I_WBS.FirstOrDefault(c => c.ID == task.ParentID);
                if (parentNode == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的父节点，无法插入节点");
                }
                dic = parentNode.AddEmptyChild(scheduleDefine, task.SortIndex);
                dic.SetValue("TID", dic.GetValue("ID"));
            }
            else
            {
                var node = engineeringInfo.S_I_WBS.FirstOrDefault(c => c.ID == NodeID);
                if (node == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法插入节点");
                }
                if (node.Parent == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的父节点，无法插入节点");
                }
                dic = node.Parent.AddEmptyChild(scheduleDefine, node.SortIndex);
                dic.SetValue("TID", dic.GetValue("ID"));
            }

            this.entities.SaveChanges();
            var id = dic.GetValue("ID");
            var defineStructNodeIDs = scheduleDefine.S_C_ScheduleDefine_Nodes.Select(c => c.StructInfoID).ToList();
            var children = engineeringInfo.S_I_WBS.Where(c => c.ParentID == id && defineStructNodeIDs.Contains(c.StructInfoID)).ToList();
            if (children.Count > 0)
            {
                var childList = new List<Dictionary<string, object>>();
                foreach (var item in children)
                {
                    var childItem = FormulaHelper.ModelToDic<S_I_WBS>(item);
                    childItem.SetValue("NodeType", item.NodeType);
                    childItem.SetValue("TaskType", item.NodeType);
                    childList.Add(childItem);
                }
                dic.SetValue("children", childList);
            }
            return Json(dic);
        }

        public JsonResult SaveNodes(string ListData, string DefineID, string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程");
            var list = JsonHelper.ToList(ListData);
            var enumListItem = EnumBaseHelper.GetEnumDef(typeof(TaskType)).EnumItem.Select(c => c.Code).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var detail = this.GetEntityByID<S_I_WBS_Task>(item.GetValue("ID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Task>(detail, item);
                        if (!enumListItem.Contains(detail.TaskType))
                        {
                            var node = detail.TransToWBS(DefineID);
                            var dic = FormulaHelper.ModelToDic<S_I_WBS>(node);
                            dic.SetValue("NodeType", node.NodeType);
                            dic.SetValue("StructInfoID", node.StructInfoID);
                            node.FullName = node.Parent.FullName + "." + node.Name;
                            result.Add(dic);
                        }
                        else
                        {
                            detail.SetRelation(DefineID); ;
                            detail.ResetUserResource();
                        }
                    }
                    detail.SynchronizeBomResource();
                }
                else
                {
                    var detail = this.GetEntityByID<S_I_WBS>(item.GetValue("ID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS>(detail, item);
                        if (enumListItem.Contains(item.GetValue("TaskType")))
                        {
                            var task = detail.TransToTask(DefineID, item.GetValue("TaskType"));
                            var dic = FormulaHelper.ModelToDic<S_I_WBS_Task>(task);
                            dic.SetValue("NodeType", "Task");
                            dic.SetValue("StructInfoID", task.StructInfoID);
                            result.Add(dic);
                        }
                        else
                        {
                            detail.NodeType = item.GetValue("TaskType");
                            detail.NodeTypeName = item.GetValue("TaskTypeName");
                            if (String.IsNullOrEmpty(detail.Value) || detail.Value == detail.ID)
                            {
                                if (!String.IsNullOrEmpty(detail.Code))
                                {
                                    detail.Value = detail.Code;
                                }
                                else if (!String.IsNullOrEmpty(detail.Name))
                                {
                                    detail.Value = detail.Name;
                                }
                            }
                            detail.SetRelation(DefineID);
                            detail.ResetUserResource();
                        }
                    }
                }
            }
            engineeringInfo.SetWBSAuthWithUser();
            engineeringInfo.SetWBSFullName(DefineID);
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveTasks(string ListData, string DefineID)
        {
            var list = JsonHelper.ToList(ListData);
            var enumListItem = EnumBaseHelper.GetEnumDef(typeof(TaskType)).EnumItem.Select(c => c.Code).ToList();
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var detail = this.GetEntityByID<S_I_WBS_Task>(item.GetValue("TID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Task>(detail, item);
                        if (!String.IsNullOrEmpty(item.GetValue("Start")))
                            detail.PlanStartDate = Convert.ToDateTime(item.GetValue("Start"));
                        if (!String.IsNullOrEmpty(item.GetValue("Finish")))
                            detail.PlanEndDate = Convert.ToDateTime(item.GetValue("Finish"));
                        if (!String.IsNullOrEmpty(item.GetValue("Duration")))
                            detail.PlanDuration = Convert.ToDecimal(item.GetValue("Duration"));
                        if (!enumListItem.Contains(detail.TaskType))
                        {
                            detail.TransToWBS(DefineID);
                        }
                        if (!String.IsNullOrEmpty(detail.PredecessorLink))
                        {
                            var linkList = JsonHelper.ToList(item.GetValue("PredecessorLink"));
                            foreach (var link in linkList)
                            {
                                var preTaskID = link.GetValue("PredecessorUID");
                                var task = detail.S_I_Engineering.S_I_WBS_Task.FirstOrDefault(c => c.ID == preTaskID);
                                if (task == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到指定的作业，无法紧前关系"); }
                                link.SetValue("PredecessorTaskName", task.Name);
                                link.SetValue("PredecessorTaskCode", task.Code);
                                if (String.IsNullOrEmpty(link.GetValue("Delay")))
                                {
                                    link.SetValue("Delay", 0);
                                }
                            }
                            detail.PredecessorLink = JsonHelper.ToJson(linkList);
                        }
                    }
                    detail.SynchronizeBomResource();
                }
                else
                {
                    var detail = this.GetEntityByID<S_I_WBS>(item.GetValue("TID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS>(detail, item);
                        if (!String.IsNullOrEmpty(item.GetValue("Start")))
                            detail.PlanStartDate = Convert.ToDateTime(item.GetValue("Start"));
                        if (!String.IsNullOrEmpty(item.GetValue("Finish")))
                            detail.PlanEndDate = Convert.ToDateTime(item.GetValue("Finish"));
                        if (!String.IsNullOrEmpty(item.GetValue("Duration")))
                            detail.PlanDuration = Convert.ToDecimal(item.GetValue("Duration"));
                        if (enumListItem.Contains(item.GetValue("TaskType")))
                        {
                            detail.TransToTask(DefineID, item.GetValue("TaskType"));
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(detail.Code))
                            {
                                detail.Value = detail.Code;
                            }
                            else if (!String.IsNullOrEmpty(detail.Name))
                            {
                                detail.Value = detail.Name;
                            }
                        }
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteNodes(string ListData, string DefineID)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var taskID = item.GetValue("ID");
                    var task = this.GetEntityByID<S_I_WBS_Task>(taskID);
                    task.ValidateDelete(DefineID);
                    this.entities.Set<S_I_WBS_Task>().Remove(task);
                }
                else
                {
                    var node = this.GetEntityByID<S_I_WBS>(item.GetValue("ID"));
                    if (node == null) continue;
                    if (node.NodeType == "Root") throw new Formula.Exceptions.BusinessValidationException("不允许删除根节点");
                    node.ValidateDelete(DefineID);
                    node.Delete();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteTask(string ListData, string DefineID)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var taskID = item.GetValue("TID");
                    var task = this.GetEntityByID<S_I_WBS_Task>(taskID);
                    task.ValidateDelete(DefineID);
                    this.entities.Set<S_I_WBS_Task>().Remove(task);
                }
                else
                {
                    var node = this.GetEntityByID<S_I_WBS>(item.GetValue("ID"));
                    if (node == null) continue;
                    if (node.NodeType == "Root") throw new Formula.Exceptions.BusinessValidationException("不允许删除根节点");
                    node.ValidateDelete(DefineID);
                    node.Delete();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction, string nodeType)
        {
            var result = new Dictionary<string, object>();
            if (nodeType == "Task")
            {
                var sourceNode = this.GetEntityByID<S_I_WBS_Task>(sourceID);
                if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
                if (dragAction.ToLower() == "before")
                {
                    var target = this.GetEntityByID<S_I_WBS_Task>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                    this.entities.Set<S_I_WBS_Task>().Where(c => c.ParentID == target.ParentID && c.EngineeringInfoID == sourceNode.EngineeringInfoID
                       && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                    sourceNode.SortIndex = target.SortIndex - 0.001;
                }
                else if (dragAction.ToLower() == "after")
                {
                    var target = this.GetEntityByID<S_I_WBS_Task>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                    this.entities.Set<S_I_WBS_Task>().Where(c => c.ParentID == target.ParentID && c.EngineeringInfoID == sourceNode.EngineeringInfoID
                      && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                    sourceNode.SortIndex = target.SortIndex + 0.001;
                }
                result = FormulaHelper.ModelToDic<S_I_WBS_Task>(sourceNode);
            }
            else
            {
                var sourceNode = this.GetEntityByID<S_I_WBS>(sourceID);
                if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
                if (dragAction.ToLower() == "before")
                {
                    var target = this.GetEntityByID<S_I_WBS>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                    this.entities.Set<S_I_WBS>().Where(c => c.ParentID == target.ParentID && c.EngineeringInfoID == sourceNode.EngineeringInfoID
                       && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                    sourceNode.SortIndex = target.SortIndex - 0.001;
                }
                else if (dragAction.ToLower() == "after")
                {
                    var target = this.GetEntityByID<S_I_WBS>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                    this.entities.Set<S_I_WBS>().Where(c => c.ParentID == target.ParentID && c.EngineeringInfoID == sourceNode.EngineeringInfoID
                      && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                    sourceNode.SortIndex = target.SortIndex + 0.001;
                }
                result = FormulaHelper.ModelToDic<S_I_WBS>(sourceNode);
            }

            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult PublishSchedule(string EngineeringInfoID, string DefineID)
        {
            var enginneringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (enginneringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法发布计划");
            var define = enginneringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义，无法发布计划");
            //var lastVersion = enginneringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == define.Code && c.FlowPhase == "End").OrderByDescending(d => d.ID).FirstOrDefault();
            var version = enginneringInfo.S_I_WBS_Version.FirstOrDefault(c => c.ScheduleCode == define.Code && c.FlowPhase != "End");

            #region 创建WBS版本
            if (version != null)
            {
                //如果之前存在一个临时版本（流程未完成的版本），则删除该版本下所有节点
                var nodes = version.S_I_WBS_Version_Node.ToList();
                foreach (var node in nodes)
                {
                    this.entities.Set<S_I_WBS_Version_Node>().Remove(node);
                }
                var tasks = version.S_I_WBS_Version_Task.ToList();
                foreach (var task in tasks)
                {
                    this.entities.Set<S_I_WBS_Version_Task>().Remove(task);
                }
            }
            else
            {
                version = new S_I_WBS_Version();
                version.ID = FormulaHelper.CreateGuid();
                version.FlowPhase = "Start";
                version.EngineeringInfoID = enginneringInfo.ID;
                version.EngineeringInfoName = enginneringInfo.Name;
                version.EngineeringInfoCode = enginneringInfo.SerialNumber;
                version.S_I_Engineering = enginneringInfo;
                version.CreateDate = DateTime.Now;
                version.CreateUser = this.CurrentUserInfo.UserName;
                version.CreateUserID = this.CurrentUserInfo.UserID;
                version.ScheduleCode = define.Code;
                version.ScheduleName = define.Name;
                var maxVersion = "0";
                if (enginneringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == define.Code).Count() > 0)
                {
                    maxVersion = enginneringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == define.Code).Max(c => c.VersionNumber);
                }
                var versionNum = String.IsNullOrEmpty(maxVersion) || !Regex.IsMatch(maxVersion, @"[1-9]\d*$") ? 0 : Convert.ToInt32(maxVersion);
                version.VersionNumber = (versionNum + 1).ToString();
                enginneringInfo.S_I_WBS_Version.Add(version);
            }
            version.UpgradeFromWBS();
            #endregion
            version.PushVersion = false;
            this.entities.SaveChanges();
            return Json(new { FormID = version.ID });
        }

        public JsonResult CalcProgress(string EngineeringInfoID, string DefineID)
        {
            var enginneringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (enginneringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法计算");
            var define = enginneringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.ID == DefineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义，无法计算");
            var defineNodeIDs = define.S_C_ScheduleDefine_Nodes.Select(a => a.StructInfoID).ToList();

            var parentIDList = this.entities.Set<S_I_WBS_Task>().Where(c => c.EngineeringInfoID == EngineeringInfoID && defineNodeIDs.Contains(c.StructInfoID)).GroupBy(a => a.ParentID).Select(a => a.Key).ToList();
            foreach (var parentID in parentIDList)
            {
                var taskList = this.entities.Set<S_I_WBS_Task>().Where(a => a.ParentID == parentID && a.EngineeringInfoID == EngineeringInfoID && defineNodeIDs.Contains(a.StructInfoID)).ToList();
                var weightProgress = 0m;
                foreach (var task in taskList)
                {
                    weightProgress += (task.Weight * task.Progress) / 100;
                }

                var node = this.entities.Set<S_I_WBS>().SingleOrDefault(a => a.ID == parentID && a.EngineeringInfoID == EngineeringInfoID && defineNodeIDs.Contains(a.StructInfoID));
                node.Progress = ((node.Weight ?? 0) * (decimal)weightProgress) / 100;
                UpCalcVersionNodeProgress(node.ParentID, EngineeringInfoID, DefineID, defineNodeIDs);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        private void UpCalcVersionNodeProgress(string parentID, string EngineeringInfoID, string DefineID, List<string> defineNodeIDs)
        {
            var node = this.entities.Set<S_I_WBS>().SingleOrDefault(a => a.ID == parentID && a.EngineeringInfoID == EngineeringInfoID && defineNodeIDs.Contains(a.StructInfoID));
            if (node == null)
                return;

            var nodeList = this.entities.Set<S_I_WBS>().Where(a => a.ParentID == parentID && a.EngineeringInfoID == EngineeringInfoID && defineNodeIDs.Contains(a.StructInfoID)).ToList();
            decimal weightProgress = 0;
            foreach (var tmpNode in nodeList)
            {
                weightProgress += (tmpNode.Weight ?? 0) * (tmpNode.Progress ?? 0) / 100;
            }


            node.Progress = ((node.Weight ?? 0) * weightProgress) / 100;
            UpCalcVersionNodeProgress(node.ParentID, EngineeringInfoID, DefineID, defineNodeIDs);
        }
    }
}
