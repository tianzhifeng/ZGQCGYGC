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
using Formula.ImportExport;
using Newtonsoft.Json;
using Comprehensive.Logic.Domain;
using System.Data.Entity;
using Base.Logic.Domain;
using System.Diagnostics;
using System.IO;
using Aspose.Tasks;
using Aspose.Tasks.Saving;
using Formula.Exceptions;

namespace EPC.Areas.Manage.Controllers
{
    public class ScheduleController : EPCController
    {
        private DbContext baseEntities = FormulaHelper.GetEntities<BaseEntities>();
        private DbContext infrastructureEntities = FormulaHelper.GetEntities<InfrastructureEntities>();

        public ActionResult Tab()
        {
            string codes = this.GetQueryString("ScheduleCodes");

            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var defines = engineeringInfo.Mode.S_C_ScheduleDefine.Where(c => codes.Contains(c.Code)).OrderBy(s => s.SortIndex).ToList();
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
            var version = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code).OrderByDescending(c => c.ID).FirstOrDefault();
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            bool flowEnd = true; bool isFirst = false;
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
            var extendFieldDefine = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(define.AttrDefine))
            {
                extendFieldDefine = JsonHelper.ToList(define.AttrDefine);
            }
            ViewBag.ExtendFieldDefine = JsonHelper.ToJson(extendFieldDefine);
            ViewBag.SelectorScript = define.GetSelectorScript();
            ViewBag.ReadonlyField = readOnlyFields.TrimEnd(',');
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            ViewBag.Define = define;
            ViewBag.ImportProject = define.ImportProject;
            ViewBag.ImportBOM = define.ImportBOM;
            ViewBag.ImportQBS = define.ImportQBS;
            ViewBag.ImportBid = define.ImportBid;
            ViewBag.ImportExcel = define.ImportExcel;
            ViewBag.ImportTaskTemplate = define.ImportTaskTemplate;
            if (!define.ImportProject && !define.ImportBOM && !define.ImportQBS && !define.ImportBid && !define.ImportTaskTemplate)
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

            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
                isFirst = true;
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
                if (!String.IsNullOrEmpty(version.PreScheduleCode))
                {
                    var preDefine = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == version.PreScheduleCode);
                    if (preDefine != null)
                    {
                        var preVersion = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == version.PreScheduleCode &&
    c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
                        ViewBag.PreVersionID = preVersion.ID;
                        ViewBag.PreScheduleCode = version.PreScheduleCode;
                        ViewBag.PreScheduleName = preDefine.Name;
                    }
                }
            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;

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
            if (version != null)
            {
                var wbsNodeCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_I_WBS_Version_Node with(nolock) WHERE VersionID='" + version.ID + "'");
                var wbsTaskCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_I_WBS_Version_Task with(nolock) WHERE VersionID='" + version.ID + "'");
                int.TryParse((wbsNodeCountObj ?? "").ToString(), out wbsNodeCount);
                int.TryParse((wbsTaskCountObj ?? "").ToString(), out wbsTaskCount);
            }

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
            string versionID = this.GetQueryString("VersionID");
            S_I_WBS_Version version = null;
            string Code = this.GetQueryString("ScheduleCode");
            if (String.IsNullOrEmpty(versionID))
            {
                version = engineeringInfo.S_I_WBS_Version.Where(c => c.ScheduleCode == Code).OrderByDescending(c => c.ID).FirstOrDefault();
            }
            else
            {
                version = this.entities.Set<S_I_WBS_Version>().FirstOrDefault(c => c.ID == versionID);
                Code = version.ScheduleCode;
            }
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + Code + "】的计划定义视图，请联系管理员"); }
            var pushCount = engineeringInfo.S_I_WBS_Version.Count(c => c.FlowPhase == "End" && c.ScheduleCode == Code);
            bool flowEnd = true; bool isFirst = false;
            ViewBag.ImportProject = define.ImportProject;
            ViewBag.ImportQBS = define.ImportQBS;
            ViewBag.ImportBOM = define.ImportBOM;
            ViewBag.ImportBid = define.ImportBid;
            ViewBag.ImportTaskTemplate = define.ImportTaskTemplate;
            if (!define.ImportProject && !define.ImportQBS && !define.ImportBOM && !define.ImportBid && !define.ImportTaskTemplate)
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
                    ViewBag.CanStart = false;
            }

            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
                isFirst = true;
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
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.ScheduleCode = Code;

            var nodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
            if (nodeTypeEnum == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【Base.WBSType】的枚举"); }
            var nodeTypeList = define.S_C_ScheduleDefine_Nodes.Where(c => c.NodeType != WBSConst.taskNodeType
                && c.Visible == "1").Select(c => c.NodeType).Distinct().ToList();
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

        public JsonResult GetMenu(string NodeID)
        {
            var wbs = this.GetEntityByID<S_I_WBS_Version_Node>(NodeID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + NodeID + "】的节点");
            if (wbs.StructNodeInfo == null) throw new Formula.Exceptions.BusinessValidationException("WBS节点未定义类别，无法显示菜单");
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var children = wbs.StructNodeInfo.Children.Where(c => c.CanAdd == "1" && c.IsDynmanic == "1").ToList();
            foreach (var item in children)
            {
                var menuItem = new Dictionary<string, object>();
                if (String.IsNullOrEmpty(item.NodeType)) continue;
                menuItem["name"] = item.NodeType;
                menuItem["text"] = "增加" + item.Name;
                menuItem["iconCls"] = "icon-add";
                menuItem["onClick"] = "addNode";
                if (item.IsEnum == "1")
                    menuItem["attrDefine"] = "true";
                else
                    menuItem["attrDefine"] = "false";
                result.Add(menuItem);
            }
            string json = JsonHelper.ToJson(result);
            return Json(result);
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
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,
Extra5Field,Extra5FieldName,Extra6Field,Extra6FieldName,Extra7Field,Extra7FieldName,
UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
from S_I_WBS_Version_Node
union select ID,TaskID as WBSID,ParentID as WBSParentID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'0' as IsEnum,'False' as IsLocked,
null as BaseStart, null as BaseFinish,PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,
Extra5Field,Extra5FieldName,Extra6Field,Extra6FieldName,Extra7Field,Extra7FieldName,
UserResource,
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
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,
case when IsLocked ='True' then 1 else 0 end as FixedDate,IsLocked,
case when IsLocked ='True' then PlanStartDate else null end as BaseStart,
case when IsLocked ='True' then PlanEndDate else null end as BaseFinish,'' as PredecessorLink
from S_I_WBS_Version_Node
union select ID as TID,TaskID as WBSID,TaskID as [UID],ParentID,ParentID as ParentTaskUID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate as Start,PlanEndDate as Finish,
CalPlanStartDate,CalPlanEndDate,PlanDuration as Duration,StrandardDuration,FactStartDate as FactStart,FactEndDate as FactFinish,CalProgress,Progress,
TaskType,TaskTypeName,
case when TaskType='MileStone' then 1 else 0 end as Milestone,
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,0 as FixedDate ,'False' as IsLocked,
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

        public JsonResult GetSelectorTaskTree(string VersionID, string ID)
        {
            string sql = @" select * from (select ID,WBSID,ParentID,FullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,IsLocked,'' as PredecessorLink
from S_I_WBS_Version_Node
union select ID,TaskID as WBSID,ParentID as WBSParentID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'False' as IsLocked,PredecessorLink
 from S_I_WBS_Version_Task ) TableInfo  where VersionID = '{0}' and ModifyState!='{1}'  order by SortIndex";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, VersionID, BomVersionModifyState.Remove.ToString(), ID));
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
                }
                var dic = FormulaHelper.DataRowToDic(row);
                result.Add(dic);
            }
            return Json(result);
        }

        public JsonResult GetDocumentList(string RelateID)
        {
            var sql = String.Format("SELECT * FROM S_I_CommonDocument WHERE RelateObjID='{0}'", RelateID);
            var data = this.SqlHelper.ExecuteDataTable(sql);
            return Json(data);
        }

        public JsonResult SaveDocument()
        {
            var orgService = FormulaHelper.GetService<IOrgService>();
            var rootOrg = orgService.GetOrgs().SingleOrDefault(c => c.ID == Config.Constant.OrgRootID);
            var entity = this.UpdateEntity<S_I_CommonDocument>();
            entity.ParticipationType = CoopertationRoleType.EPC.ToString();
            if (this.entities.Entry<S_I_CommonDocument>(entity).State == System.Data.EntityState.Detached
               || this.entities.Entry<S_I_CommonDocument>(entity).State == System.Data.EntityState.Added)
            {
                if (rootOrg != null)
                {
                    entity.ParticipationName = rootOrg.Name;
                    entity.Participation = rootOrg.ID;
                }
            }
            this.entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult GetDocumentData(string id, string RelateID)
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var entity = this.GetEntity<S_I_CommonDocument>(id);
            entity.ParticipationType = CoopertationRoleType.EPC.ToString();
            if (this.entities.Entry<S_I_CommonDocument>(entity).State == System.Data.EntityState.Detached
               || this.entities.Entry<S_I_CommonDocument>(entity).State == System.Data.EntityState.Added)
            {
                entity.RelateObjID = RelateID;
                entity.RelateObjType = "WBS";
            }
            return Json(entity);
        }

        public JsonResult DelDocument(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("ParticipationType") != EPC.Logic.CoopertationRoleType.EPC.ToString())
                {
                    //总包方不允许删除其他人创建的文件
                    throw new Formula.Exceptions.BusinessValidationException("您不能删除其他参建方所创建的文件");
                }
                item.DeleteDB(this.SqlHelper, "S_I_CommonDocument", item.GetValue("ID"));
            }
            return Json("");
        }

        public JsonResult UpgradSchedule(string EngineeringInfoID, string ScheduleCode)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法进行结构策划");
            var version = engineeringInfo.UpgradSchedule(ScheduleCode);
            version.PushVersion = true;
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddEmptyNode(string ParentNodeID, string VersionID)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本，无法新增节点");
            }
            var node = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.ID == ParentNodeID);
            if (node == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("作业下不能新增节点，只有WBS才能新增子节点");
            }
            var dic = node.AddEmptyChild();
            dic.SetValue("TID", dic.GetValue("ID"));

            this.entities.SaveChanges();
            var id = dic.GetValue("ID");
            var children = version.S_I_WBS_Version_Node.Where(c => c.ParentID == id).ToList();
            if (children.Count > 0)
            {
                var childList = new List<Dictionary<string, object>>();
                foreach (var item in children)
                {
                    var childItem = FormulaHelper.ModelToDic<S_I_WBS_Version_Node>(item);
                    childItem.SetValue("NodeType", item.NodeType);
                    childItem.SetValue("TaskType", item.NodeType);
                    childList.Add(childItem);
                }
                dic.SetValue("children", childList);
            }
            return Json(dic);
        }

        public JsonResult InsertEmptyNode(string NodeID, string VersionID, string NodeType)
        {
            var dic = new Dictionary<string, object>();
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本，无法新增节点");
            }
            if (NodeType == "Task")
            {
                var task = version.S_I_WBS_Version_Task.FirstOrDefault(c => c.ID == NodeID);
                if (task == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的作业界面，无法插入作业");
                }
                var parentNode = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.WBSID == task.ParentID);
                if (parentNode == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的父节点，无法插入节点");
                }
                dic = parentNode.AddEmptyChild(task.SortIndex);
                dic.SetValue("TID", dic.GetValue("ID"));
            }
            else
            {
                var node = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.ID == NodeID);
                if (node == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法插入节点");
                }
                var parentNode = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.WBSID == node.ParentID);
                if (parentNode == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到指定的父节点，无法插入节点");
                }
                dic = parentNode.AddEmptyChild(node.SortIndex);
                dic.SetValue("TID", dic.GetValue("ID"));
            }

            this.entities.SaveChanges();
            var id = dic.GetValue("ID");
            var children = version.S_I_WBS_Version_Node.Where(c => c.ParentID == id).ToList();
            if (children.Count > 0)
            {
                var childList = new List<Dictionary<string, object>>();
                foreach (var item in children)
                {
                    var childItem = FormulaHelper.ModelToDic<S_I_WBS_Version_Node>(item);
                    childItem.SetValue("NodeType", item.NodeType);
                    childItem.SetValue("TaskType", item.NodeType);
                    childList.Add(childItem);
                }
                dic.SetValue("children", childList);
            }
            return Json(dic);
        }

        public JsonResult SaveNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var enumListItem = EnumBaseHelper.GetEnumDef(typeof(EPC.Logic.TaskType)).EnumItem.Select(c => c.Code).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var detail = this.GetEntityByID<S_I_WBS_Version_Task>(item.GetValue("ID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Version_Task>(detail, item);
                        if (!enumListItem.Contains(detail.TaskType))
                        {
                            var node = detail.TransToWBS();
                            var dic = FormulaHelper.ModelToDic<S_I_WBS_Version_Node>(node);
                            dic.SetValue("NodeType", node.NodeType);
                            dic.SetValue("StructInfoID", node.StructInfoID);
                            result.Add(dic);
                        }
                        else
                        {
                            if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                                detail.ModifyState = BomVersionModifyState.Modify.ToString();
                            detail.SetRelation();
                            var dic = FormulaHelper.ModelToDic<S_I_WBS_Version_Task>(detail);
                            dic.SetValue("NodeType", "Task");
                            result.Add(dic);
                        }
                    }
                    detail.SynchronizeBomResource();
                }
                else
                {
                    var detail = this.GetEntityByID<S_I_WBS_Version_Node>(item.GetValue("ID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Version_Node>(detail, item);
                        if (enumListItem.Contains(item.GetValue("TaskType")))
                        {
                            var task = detail.TransToTask(item.GetValue("TaskType"));
                            var dic = FormulaHelper.ModelToDic<S_I_WBS_Version_Task>(task);
                            dic.SetValue("NodeType", "Task");
                            dic.SetValue("StructInfoID", task.StructInfoID);
                            result.Add(dic);
                        }
                        else
                        {
                            detail.NodeType = item.GetValue("TaskType");
                            detail.NodeTypeName = item.GetValue("TaskTypeName");
                            if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                                detail.ModifyState = BomVersionModifyState.Modify.ToString();
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
                            detail.SetRelation();
                            var dic = FormulaHelper.ModelToDic<S_I_WBS_Version_Node>(detail);
                            dic.SetValue("NodeType", detail.NodeType);
                            result.Add(dic);
                        }
                    }
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveTasks(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var enumListItem = EnumBaseHelper.GetEnumDef(typeof(EPC.Logic.TaskType)).EnumItem.Select(c => c.Code).ToList();
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var detail = this.GetEntityByID<S_I_WBS_Version_Task>(item.GetValue("TID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Version_Task>(detail, item);
                        if (!String.IsNullOrEmpty(item.GetValue("Start")))
                            detail.PlanStartDate = Convert.ToDateTime(item.GetValue("Start"));
                        if (!String.IsNullOrEmpty(item.GetValue("Finish")))
                            detail.PlanEndDate = Convert.ToDateTime(item.GetValue("Finish"));
                        if (!String.IsNullOrEmpty(item.GetValue("Duration")))
                            detail.PlanDuration = Convert.ToDecimal(item.GetValue("Duration"));
                        if (!enumListItem.Contains(detail.TaskType))
                        {
                            detail.TransToWBS();
                        }
                        else
                        {
                            if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                                detail.ModifyState = BomVersionModifyState.Modify.ToString();
                        }
                        if (!String.IsNullOrEmpty(detail.PredecessorLink))
                        {
                            var linkList = JsonHelper.ToList(item.GetValue("PredecessorLink"));
                            foreach (var link in linkList)
                            {
                                var preTaskID = link.GetValue("PredecessorUID");
                                var task = detail.S_I_WBS_Version.S_I_WBS_Version_Task.FirstOrDefault(c => c.TaskID == preTaskID);
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
                }
                else
                {
                    var detail = this.GetEntityByID<S_I_WBS_Version_Node>(item.GetValue("TID"));
                    if (detail != null)
                    {
                        this.UpdateEntity<S_I_WBS_Version_Node>(detail, item);
                        if (!String.IsNullOrEmpty(item.GetValue("Start")))
                            detail.PlanStartDate = Convert.ToDateTime(item.GetValue("Start"));
                        if (!String.IsNullOrEmpty(item.GetValue("Finish")))
                            detail.PlanEndDate = Convert.ToDateTime(item.GetValue("Finish"));
                        if (!String.IsNullOrEmpty(item.GetValue("Duration")))
                            detail.PlanDuration = Convert.ToDecimal(item.GetValue("Duration"));
                        if (enumListItem.Contains(item.GetValue("TaskType")))
                        {
                            detail.TransToTask(item.GetValue("TaskType"));
                        }
                        else
                        {
                            if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                                detail.ModifyState = BomVersionModifyState.Modify.ToString();
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

        public JsonResult SetNodePredecessorLink(string ListData, string NodeID, string Operation)
        {
            var task = this.GetEntityByID<S_I_WBS_Version_Task>(NodeID);
            if (task == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的作业节点，无法关联紧前作业");
            if (Operation == "Add")
            {
                var list = JsonHelper.ToList(task.PredecessorLink);
                var addList = JsonHelper.ToList(ListData);
                foreach (var item in addList)
                {
                    var taskID = item.GetValue("TaskID");
                    var preLinkTask = list.FirstOrDefault(c => c.GetValue("TaskID") == taskID);
                    if (preLinkTask == null)
                    {
                        var preTask = this.GetEntityByID<S_I_WBS_Version_Task>(item.GetValue("ID"));
                        if (preTask == null) continue;
                        preLinkTask = new Dictionary<string, object>();
                        preLinkTask.SetValue("PredecessorUID", preTask.TaskID);
                        preLinkTask.SetValue("Type", "1");
                        preLinkTask.SetValue("LinkLag", "0");
                        preLinkTask.SetValue("TaskUID", task.TaskID);
                        preLinkTask.SetValue("PredecessorTaskName", preTask.Name);
                        preLinkTask.SetValue("PredecessorTaskCode", preTask.Code);
                        preLinkTask.SetValue("Delay", 0);
                        list.Add(preLinkTask);
                    }
                }
                task.PredecessorLink = JsonHelper.ToJson(list);
            }
            else
            {
                var addList = JsonHelper.ToList(ListData);
                foreach (var item in addList)
                {
                    var taskID = item.GetValue("TaskID");
                    var preTask = this.GetEntityByID<S_I_WBS_Version_Task>(item.GetValue("ID"));
                    if (preTask == null) continue;
                    item.SetValue("PredecessorTaskName", preTask.Name);
                    item.SetValue("PredecessorTaskCode", preTask.Code);
                }
                task.PredecessorLink = JsonHelper.ToJson(addList);
            }
            this.entities.SaveChanges();
            return Json(JsonHelper.ToList(task.PredecessorLink));
        }

        public JsonResult SaveUserRole(string ListData, string NodeID)
        {
            var list = JsonHelper.ToList(ListData);
            var task = this.GetEntityByID<S_I_WBS_Version_Task>(NodeID);
            if (task == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的作业，无法保存人员角色");
            foreach (var item in list)
            {
                item.SetValue("TaskID", task.ID);
                item.SetValue("WBSID", task.ParentID);
                item.SetValue("EngineeringInfoID", task.EngineeringInfoID);
                item.SetValue("ResourceType", "UserRole");
            }
            task.UserResource = JsonHelper.ToJson(list);
            this.entities.SaveChanges();
            return Json(JsonHelper.ToList(task.UserResource));
        }

        public JsonResult DeleteNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var taskID = item.GetValue("ID");
                    var task = this.GetEntityByID<S_I_WBS_Version_Task>(taskID);
                    task.ValidateDelete();
                    if (task.ModifyState == BomVersionModifyState.Add.ToString())
                    {
                        this.entities.Set<S_I_WBS_Version_Task>().Remove(task);
                    }
                    else
                    {
                        task.ModifyState = BomVersionModifyState.Remove.ToString();
                    }
                }
                else
                {
                    var node = this.GetEntityByID<S_I_WBS_Version_Node>(item.GetValue("ID"));
                    if (node == null) continue;
                    if (node.NodeType == "Root") throw new Formula.Exceptions.BusinessValidationException("不允许删除根节点");
                    node.ValidateDelete();
                    if (node.ModifyState == BomVersionModifyState.Add.ToString())
                    {
                        this.entities.Set<S_I_WBS_Version_Node>().Delete(c => c.FullID.StartsWith(node.FullID));
                        this.entities.Set<S_I_WBS_Version_Task>().Delete(c => c.WBSFullID.StartsWith(node.FullID));
                    }
                    else
                    {
                        this.entities.Set<S_I_WBS_Version_Node>().Where(c => c.FullID.StartsWith(node.FullID)).
                            Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                        var addState = BomVersionModifyState.Add.ToString();
                        this.entities.Set<S_I_WBS_Version_Task>().Where(c => c.WBSFullID.StartsWith(node.FullID) && c.ModifyState != addState).
                            Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                        this.entities.Set<S_I_WBS_Version_Task>().Delete(c => c.WBSFullID.StartsWith(node.FullID) && c.ModifyState == addState);
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteTask(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == WBSConst.taskNodeType)
                {
                    var taskID = item.GetValue("TID");
                    var task = this.GetEntityByID<S_I_WBS_Version_Task>(taskID);
                    task.ValidateDelete();
                    if (task.ModifyState == BomVersionModifyState.Add.ToString())
                    {
                        this.entities.Set<S_I_WBS_Version_Task>().Remove(task);
                    }
                    else
                    {
                        task.ModifyState = BomVersionModifyState.Remove.ToString();
                    }
                }
                else
                {
                    var node = this.GetEntityByID<S_I_WBS_Version_Node>(item.GetValue("TID"));
                    if (node == null || node.NodeType == "Root")
                        continue;
                    node.ValidateDelete();
                    if (node.ModifyState == BomVersionModifyState.Add.ToString())
                    {
                        this.entities.Set<S_I_WBS_Version_Node>().Delete(c => c.FullID.StartsWith(node.FullID));
                        this.entities.Set<S_I_WBS_Version_Task>().Delete(c => c.WBSFullID.StartsWith(node.FullID));
                    }
                    else
                    {
                        this.entities.Set<S_I_WBS_Version_Node>().Where(c => c.FullID.StartsWith(node.FullID)).
                            Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                        var addState = BomVersionModifyState.Add.ToString();
                        this.entities.Set<S_I_WBS_Version_Task>().Where(c => c.WBSFullID.StartsWith(node.FullID) && c.ModifyState != addState).
                            Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                        this.entities.Set<S_I_WBS_Version_Task>().Delete(c => c.WBSFullID.StartsWith(node.FullID) && c.ModifyState == addState);
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_I_WBS_Version>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            this.entities.Set<S_I_WBS_Version>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetNodeType(string ParentID, string VersionID)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var node = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.WBSID == ParentID);
            if (node == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var structList = node.StructNodeInfo.Children.ToList();
            foreach (var item in structList)
            {
                if (item.CanAdd != "1") continue;
                if (item.NodeType == WBSConst.taskNodeType)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("value", item.TaskType);
                    dic.SetValue("text", EnumBaseHelper.GetEnumDescription(typeof(EPC.Logic.TaskType), item.TaskType));
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
            var wbs = this.GetEntityByID<S_I_WBS_Version_Node>(ID);
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

        public JsonResult ImportTemplate(string VersionID, string TemplateID)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            version.ImportTemplate(TemplateID);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportTaskTemplateDetail(string VersionID, string ListData)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            var infrasEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detailID = item.GetValue("ID");
                var detail = infrasEntities.S_T_TaskTemplate_Detail.Find(detailID);
                if (detail != null)
                {
                    version.ImportTaskTemplateDetail(detail);
                }
            }
            version.SetWBSFromTask();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportBidBom(string WBSNodeID, string VersionID, string ListData)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            var node = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.ID == WBSNodeID);
            if (node == null) throw new Formula.Exceptions.BusinessValidationException("必须选中一个节点才能导入");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detailID = item.GetValue("ID");
                var detail = this.entities.Set<S_M_BidOffer_CBS_Detail>().Find(detailID);
                if (detail != null)
                {
                    version.ImportBidBOM(detail, node);
                }
            }
            version.SetWBSFromTask();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPBom(string VersionID, string ListData)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detailID = item.GetValue("ID");
                var detail = this.entities.Set<S_P_Bom>().Find(detailID);
                if (detail != null)
                {
                    version.ImportPBOM(detail);
                }
            }
            version.SetWBSFromTask();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFromQBS(string QBSData, string VersionID, string NodeID)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本数据");
            var node = version.S_I_WBS_Version_Node.FirstOrDefault(c => c.ID == NodeID);
            if (node == null) throw new Formula.Exceptions.BusinessValidationException("必须选中一个节点才能进行QBS结构导入");
            var qbsList = JsonHelper.ToList(QBSData);
            foreach (var item in qbsList)
            {
                var qbs = this.GetEntityByID<S_Q_QBS>(item.GetValue("ID"));
                if (qbs == null) continue;
                version.ImportFromQBS(qbs, node);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction, string nodeType)
        {
            var result = new Dictionary<string, object>();
            if (nodeType == "Task")
            {
                var sourceNode = this.GetEntityByID<S_I_WBS_Version_Task>(sourceID);
                if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
                if (dragAction.ToLower() == "before")
                {
                    var target = this.GetEntityByID<S_I_WBS_Version_Task>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                    this.entities.Set<S_I_WBS_Version_Task>().Where(c => c.ParentID == target.ParentID && c.VersionID == sourceNode.VersionID
                       && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                    sourceNode.SortIndex = target.SortIndex - 0.001;
                }
                else if (dragAction.ToLower() == "after")
                {
                    var target = this.GetEntityByID<S_I_WBS_Version_Task>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                    this.entities.Set<S_I_WBS_Version_Task>().Where(c => c.ParentID == target.ParentID && c.VersionID == sourceNode.VersionID
                      && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                    sourceNode.SortIndex = target.SortIndex + 0.001;
                }
                result = FormulaHelper.ModelToDic<S_I_WBS_Version_Task>(sourceNode);
            }
            else
            {
                var sourceNode = this.GetEntityByID<S_I_WBS_Version_Node>(sourceID);
                if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
                if (dragAction.ToLower() == "before")
                {
                    var target = this.GetEntityByID<S_I_WBS_Version_Node>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                    this.entities.Set<S_I_WBS_Version_Node>().Where(c => c.ParentID == target.ParentID && c.VersionID == sourceNode.VersionID
                       && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                    sourceNode.SortIndex = target.SortIndex - 0.001;
                }
                else if (dragAction.ToLower() == "after")
                {
                    var target = this.GetEntityByID<S_I_WBS_Version_Node>(targetID);
                    if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                    this.entities.Set<S_I_WBS_Version_Node>().Where(c => c.ParentID == target.ParentID && c.VersionID == sourceNode.VersionID
                      && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                    sourceNode.SortIndex = target.SortIndex + 0.001;
                }
                result = FormulaHelper.ModelToDic<S_I_WBS_Version_Node>(sourceNode);
            }

            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult CalcProgress(string VersionID)
        {
            var parentIDList = this.entities.Set<S_I_WBS_Version_Task>().Where(c => c.VersionID == VersionID).GroupBy(a => a.ParentID).Select(a => a.Key).ToList();
            foreach (var parentID in parentIDList)
            {
                var taskList = this.entities.Set<S_I_WBS_Version_Task>().Where(a => a.ParentID == parentID && a.VersionID == VersionID).ToList();
                var weightProgress = 0m;
                foreach (var task in taskList)
                {
                    weightProgress += (task.Weight * task.Progress) / 100;
                }

                var node = this.entities.Set<S_I_WBS_Version_Node>().SingleOrDefault(a => a.WBSID == parentID && a.VersionID == VersionID);
                node.Progress = ((node.Weight ?? 0) * (decimal)weightProgress) / 100;
                UpCalcVersionNodeProgress(node.ParentID, VersionID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        private void UpCalcVersionNodeProgress(string parentID, string versionID)
        {
            var node = this.entities.Set<S_I_WBS_Version_Node>().SingleOrDefault(a => a.WBSID == parentID && a.VersionID == versionID);
            if (node == null)
                return;

            var nodeList = this.entities.Set<S_I_WBS_Version_Node>().Where(a => a.ParentID == parentID && a.VersionID == versionID).ToList();
            decimal weightProgress = 0;
            foreach (var tmpNode in nodeList)
            {
                weightProgress += (tmpNode.Weight ?? 0) * (tmpNode.Progress ?? 0) / 100;
            }


            node.Progress = ((node.Weight ?? 0) * weightProgress) / 100;
            UpCalcVersionNodeProgress(node.ParentID, versionID);
        }

        #region 计划导出excel

        [ValidateInput(false)]
        public ActionResult ExportExcel(string versionID, string jsonColumns)
        {
            var version = this.GetEntityByID<S_I_WBS_Version>(versionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");

            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "WBS_" + version.ScheduleCode;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            templatePath = Server.MapPath("/") + templatePath;

            if (System.IO.File.Exists(templatePath))
            {
                Formula.LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = Formula.ImportExport.FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, version.ScheduleName);
            }

            var dt = new DataTable();
            foreach (var item in columns)
            {
                if (dt.Columns.Contains(item.FieldName))
                {
                    continue;
                }
                dt.Columns.Add(item.FieldName);
            }
            JsonResult jr = GetVersionTreeList(versionID, "");
            IEnumerable<Dictionary<string, object>> dicList = jr.Data as List<Dictionary<string, object>>;
            FillDataTable(ref dt, dicList, columns);
            dt.TableName = excelKey;
            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(version.ScheduleName) + ".xls");

        }

        private void FillDataTable(ref DataTable dt, IEnumerable<Dictionary<string, object>> dicList, List<ColumnInfo> columns, bool top = true, int spaceCount = 0, string parentID = "")
        {
            List<Dictionary<string, object>> children = new List<Dictionary<string, object>>();
            if (top)
            {
                //fullid长度最小的肯定是最顶层项
                children = dicList.Where(a => a.GetValue("FullID").Length == dicList.Min(b => b.GetValue("FullID").Length)).OrderBy(a => a.GetValue("SortIndex")).ToList();
            }
            else
            {
                children = dicList.Where(a => a.GetValue("ParentID") == parentID).OrderBy(a => a.GetValue("SortIndex")).ToList();
            }

            foreach (var child in children)
            {
                var detailRow = dt.NewRow();

                foreach (var detailColumn in columns)
                {
                    if (detailColumn.FieldName == "Name")
                    {
                        string space = "";

                        for (int i = 0; i < spaceCount; i++)
                        {
                            space += "    ";
                        }
                        detailRow[detailColumn.FieldName] = child.GetValue(detailColumn.FieldName);//space + child.GetValue(detailColumn.FieldName);
                    }
                    else
                    {
                        string val = child.GetValue(detailColumn.FieldName);
                        DateTime date = DateTime.Now;
                        if (DateTime.TryParse(val, out date))
                        {
                            val = date.ToString("yyyy-MM-dd");
                        }
                        detailRow[detailColumn.FieldName] = val;
                    }
                }
                dt.Rows.Add(detailRow);
                FillDataTable(ref dt, dicList, columns, false, spaceCount + 1, child.GetValue("WBSID"));
            }
        }

        #endregion

        #region 计划导入excel

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var vID = GetQueryString("VersionID");
            S_I_WBS_Version version = entities.Set<S_I_WBS_Version>().Find(vID);
            if (version == null)
            {
                throw new Exception("未找到ID为" + vID + "的S_I_WBS_Version");
            }
            S_I_Engineering engineeringInfo = entities.Set<S_I_Engineering>().Find(version.EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Exception("未找到ID为" + engineeringInfo.ID + "的S_I_Engineering");
            }

            var attrDic = JsonHelper.ToList(version.ScheduleDefine.AttrDefine);
            //S_C_WBSStruct wbsStructItem = null;
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "RowType")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "类别不能为空";
                    }
                    //else
                    //{
                    //    if (e.Value.Contains("作业"))
                    //    {
                    //        if (!EnumBaseHelper.GetEnumDef(typeof(TaskType)).EnumItem.Any(a => a.Name == e.Value))
                    //        {
                    //            e.IsValid = false;
                    //            e.ErrorText = "TaskType未找到名为【" + e.Value + "】的作业名称";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var wbsEnum = EnumBaseHelper.GetEnumDef("Base.WBSType");
                    //        EnumItemInfo wbsType = wbsEnum.EnumItem.FirstOrDefault(a => a.Name == e.Value);
                    //        if (wbsType == null)
                    //        {
                    //            e.IsValid = false;
                    //            e.ErrorText = "WBSType未找到对应枚举值";
                    //        }
                    //        else
                    //        {
                    //            wbsStructItem = infrastructureEntities.Set<S_C_WBSStruct>().FirstOrDefault(a => a.NodeType == wbsType.Code &&
                    //            a.ModeID == engineeringInfo.Mode.ID);

                    //            if (wbsStructItem == null)
                    //            {
                    //                e.IsValid = false;
                    //                e.ErrorText = "未在S_C_WBSStruct中找到该类型";
                    //            }
                    //        }
                    //    }                        
                    //}
                }
                else if (e.FieldName == "RowName")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "名称不能为空";
                    }
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(wbsStructItem.EnumKey))
                    //    {
                    //        var wbsEnum = EnumBaseHelper.GetEnumDef(wbsStructItem.EnumKey);
                    //        if (wbsEnum != null && !wbsEnum.EnumItem.Any(a => a.Name == e.Value))
                    //        {
                    //            e.IsValid = false;
                    //            e.ErrorText = "未在" + wbsStructItem.EnumKey + "枚举中找到";
                    //        }
                    //    }
                    //}
                }
                else if (e.FieldName == "Weight")
                {
                    decimal tmp = 0;
                    if (!String.IsNullOrEmpty(e.Value) && !decimal.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "权重格式不对";
                    }
                }
                else if (e.FieldName == "PlanDuration")
                {
                    decimal tmp = 0;
                    if (!String.IsNullOrEmpty(e.Value) && !decimal.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "工期格式不对";
                    }
                }
                else if (e.FieldName == "PlanStartDate")
                {
                    DateTime tmp = DateTime.Now;
                    if (!String.IsNullOrEmpty(e.Value) && !DateTime.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "计划开始日期格式不对";
                    }
                }
                else if (e.FieldName == "PlanEndDate")
                {
                    DateTime tmp = DateTime.Now;
                    if (!String.IsNullOrEmpty(e.Value) && !DateTime.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "计划完成日期格式不对";
                    }
                }
                else
                {
                    var item = attrDic.SingleOrDefault(a => a.GetValue("AttrField") + "Name" == e.FieldName);
                    if (item != null && !String.IsNullOrEmpty(e.Value))
                    {
                        if (item.GetValue("RelateType") == "role")
                        {
                            var employee = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.Name == e.Value);
                            if (employee == null)
                            {
                                e.IsValid = false;
                                e.ErrorText = "未找到这个人";
                            }
                        }
                        else if (item.GetValue("RelateType") == "devicedate" || item.GetValue("RelateType") == "procdate")
                        {
                            DateTime date = DateTime.Now;
                            if (!DateTime.TryParse(e.Value, out date))
                            {
                                e.IsValid = false;
                                e.ErrorText = "日期格式不对";
                            }
                        }
                    }
                }
            });
            return Json(errors);
        }
        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);
            SaveFromList(list);
            return Json("Success");
        }
        public void SaveFromList(List<Dictionary<string, object>> list)
        {
            string vID = GetQueryString("VersionID");
            S_I_WBS_Version version = entities.Set<S_I_WBS_Version>().Find(vID);
            if (version == null)
            {
                throw new Exception("未找到ID为" + vID + "的S_I_WBS_Version");
            }

            if (list.Count > 0)
            {
                S_C_ScheduleDefine_Nodes defineNodes = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.Where(a => a.Visible == "1").ToList().OrderBy(a => a.FullID.Length).FirstOrDefault();
                if (defineNodes != null && defineNodes.Parent == null)
                {
                    var rootWBSBode = version.S_I_WBS_Version_Node.FirstOrDefault(a => a.StructInfoID == defineNodes.StructInfoID);
                    AddWBSNode(list, version, defineNodes.Children, rootWBSBode);
                    entities.SaveChanges();
                }
                else if (defineNodes != null && defineNodes.Parent != null)
                {
                    var children = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.Where(a => a.FullID.Length == defineNodes.FullID.Length).ToList();
                    var rootWBSBode = version.S_I_WBS_Version_Node.FirstOrDefault(a => a.StructInfoID == defineNodes.Parent.StructInfoID);
                    AddWBSNode(list, version, children, rootWBSBode);
                    entities.SaveChanges();
                }

            }
        }

        private void AddWBSNode(List<Dictionary<string, object>> list, S_I_WBS_Version version, List<S_C_ScheduleDefine_Nodes> childrenDefineNode, S_I_WBS_Version_Node parentWBSNode)
        {
            foreach (var structInfo in childrenDefineNode)
            {
                // S_C_WBSStruct wbsStruct = infrastructureEntities.Set<S_C_WBSStruct>().Find(structInfo.StructInfoID);
                //作业节点
                if (structInfo.NodeType == WBSConst.taskNodeType)
                {
                    string taskTypeName = EnumBaseHelper.GetEnumDescription(typeof(EPC.Logic.TaskType), structInfo.TaskType); ;
                    var sourceOfNodeTypeList = list.Where(a => a.GetValue("RowType") == taskTypeName).ToList();
                    AddTaskNodes(sourceOfNodeTypeList, version, structInfo, parentWBSNode);
                }
                else
                {
                    var wbsTypeDt = EnumBaseHelper.GetEnumTable("Base.WBSType");
                    var wbsTypeDicList = FormulaHelper.DataTableToListDic(wbsTypeDt);
                    var wbsTypeDic = wbsTypeDicList.FirstOrDefault(a => a.GetValue("value") == structInfo.NodeType);
                    if (wbsTypeDic == null) continue;

                    var sourceOfNodeTypeList = list.Where(a => a.GetValue("RowType") == wbsTypeDic.GetValue("text")).ToList();
                    for (int i = 0; i < sourceOfNodeTypeList.Count; i++)
                    {
                        var source = sourceOfNodeTypeList[i];
                        S_I_WBS_Version_Node wbsNode = null;
                        if (structInfo.CanAdd == "1")//能够自由增加的，则不做重复性校核。
                        {
                            wbsNode = new S_I_WBS_Version_Node();
                            wbsNode.ID = FormulaHelper.CreateGuid();
                            wbsNode.WBSID = wbsNode.ID;
                            wbsNode.Code = structInfo.Code;
                            wbsNode.BOQInfo = "";
                            wbsNode.BudgetValue = null;
                            wbsNode.ChargerDept = CurrentUserInfo.UserOrgID;
                            wbsNode.ChargerDeptName = CurrentUserInfo.UserOrgName;
                            wbsNode.ChargerUser = CurrentUserInfo.UserID;
                            wbsNode.ChargerUserName = CurrentUserInfo.UserName;

                            wbsNode.ContractInfo = "";
                            wbsNode.ContractValue = null;
                            wbsNode.CostValue = null;
                            wbsNode.CreateDate = DateTime.Now;
                            wbsNode.CreateUser = CurrentUserInfo.UserName;
                            wbsNode.CreateUserID = CurrentUserInfo.UserID;
                            wbsNode.DeviceInfo = "";
                            wbsNode.DocumentInfo = "";
                            wbsNode.EngineeringInfoID = version.EngineeringInfoID;
                            wbsNode.EstimateValue = null;

                            wbsNode.FullID = parentWBSNode == null ? "" : (parentWBSNode.FullID + "." + wbsNode.WBSID);
                            wbsNode.FullName = "";
                            wbsNode.Level = "";

                            var wbsType = EnumBaseHelper.GetEnumDef("Base.WBSType").EnumItem.SingleOrDefault(a => a.Code == structInfo.NodeType);
                            wbsNode.NodeTypeName = wbsType.Name;

                            wbsNode.ParentID = parentWBSNode == null ? "" : parentWBSNode.WBSID;
                            wbsNode.PredecessorLink = "";
                            wbsNode.Progress = null;
                            wbsNode.QBSInfo = "";
                            var sortIndex = version.S_I_WBS_Version_Node.Count == 0 ? 0 : version.S_I_WBS_Version_Node.Max(c => c.SortIndex);

                            wbsNode.SortIndex = sortIndex + 1;
                            wbsNode.StructInfoID = structInfo.StructInfoID;
                            wbsNode.UserResource = "";

                            wbsNode.VersionID = version.ID;
                            wbsNode.S_I_WBS_Version = version;
                            wbsNode.ModifyState = BomVersionModifyState.Add.ToString();
                            wbsNode.NodeType = structInfo.NodeType;
                            wbsNode.IsLocked = false.ToString();
                            if (structInfo.IsLocked == "1")
                            {
                                wbsNode.IsLocked = true.ToString();
                            }
                            wbsNode.ModifyDate = DateTime.Now;
                            wbsNode.ModifyUser = CurrentUserInfo.UserName;
                            wbsNode.ModifyUserID = CurrentUserInfo.UserID;
                            wbsNode.StrandardDuration = null;

                            wbsNode.BasePlanEndDate = null;
                            wbsNode.BasePlanStartDate = null;
                            wbsNode.CalPlanEndDate = null;
                            wbsNode.CalPlanStartDate = null;
                            wbsNode.CalProgress = null;

                            wbsNode.Name = source.GetValue("RowName");
                            wbsNode.Value = source.GetValue("RowName");
                            #region ExtraField
                            Dictionary<string, object> extra = FillExtraValue(version, source);
                            FormulaHelper.UpdateEntity(wbsNode, extra);
                            #endregion
                            //wbsNode.
                            wbsNode.S_I_WBS_Version.S_I_WBS_Version_Node.Add(wbsNode);
                            entities.Set<S_I_WBS_Version_Node>().Add(wbsNode);
                        }
                        //不能自由增加的，需要做存在性校核(这里不报错直接跳过)
                        else
                        {
                            wbsNode = version.S_I_WBS_Version_Node.FirstOrDefault(a => a.StructInfoID == structInfo.StructInfoID && a.Code == source.GetValue("Code"));
                            //尝试从名字匹配
                            if (wbsNode == null)
                            {
                                wbsNode = version.S_I_WBS_Version_Node.FirstOrDefault(a => a.StructInfoID == structInfo.StructInfoID && a.Name == source.GetValue("RowName"));
                            }

                            if (wbsNode == null)
                            {
                                continue;
                            }

                            #region ExtraField
                            Dictionary<string, object> extra = FillExtraValue(version, source);
                            FormulaHelper.UpdateEntity(wbsNode, extra);
                            #endregion
                        }
                        AddWBSNode(GetSubList(list, wbsTypeDic.GetValue("text"), i), version, structInfo.Children, wbsNode);
                    }
                }
            }
        }
        private void AddTaskNodes(List<Dictionary<string, object>> taskList, S_I_WBS_Version version, S_C_ScheduleDefine_Nodes defineNode, S_I_WBS_Version_Node parentWBSNode)
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                var task = taskList[i];

                if (defineNode.CanAdd == "1")
                {
                    S_I_WBS_Version_Task taskNode = new S_I_WBS_Version_Task();
                    taskNode.ID = FormulaHelper.CreateGuid();
                    taskNode.VersionID = version.ID;
                    taskNode.S_I_WBS_Version = version;
                    taskNode.TaskID = taskNode.ID;
                    taskNode.EngineeringInfoID = version.EngineeringInfoID;
                    taskNode.ParentID = parentWBSNode.WBSID;
                    taskNode.WBSFullID = parentWBSNode.FullID;
                    taskNode.ChargerDept = CurrentUserInfo.UserOrgID;
                    taskNode.ChargerDeptName = CurrentUserInfo.UserOrgName;
                    taskNode.ChargerUser = CurrentUserInfo.UserID;
                    taskNode.ChargerUserName = CurrentUserInfo.UserName;
                    taskNode.StructInfoID = defineNode.StructInfoID;
                    taskNode.TaskType = defineNode.TaskType;
                    taskNode.TaskTypeName = EnumBaseHelper.GetEnumDescription(typeof(EPC.Logic.TaskType), taskNode.TaskType);
                    taskNode.Progress = 0;
                    var sortIndex = version.S_I_WBS_Version_Task.Count == 0 ? 0 : version.S_I_WBS_Version_Task.Max(c => c.SortIndex);

                    taskNode.SortIndex = sortIndex + 1;

                    taskNode.RootName = taskNode.S_I_WBS_Version.RootNode.Name;
                    taskNode.RootValue = taskNode.S_I_WBS_Version.RootNode.Value;
                    var nameField = parentWBSNode.NodeType + "Name";
                    var valueField = parentWBSNode.NodeType + "Value";
                    taskNode.SetProperty(nameField, parentWBSNode.Name);
                    taskNode.SetProperty(valueField, parentWBSNode.Value);
                    foreach (var ances in parentWBSNode.Ancestor)
                    {
                        nameField = ances.NodeType + "Name";
                        valueField = ances.NodeType + "Value";
                        taskNode.SetProperty(nameField, ances.Name);
                        taskNode.SetProperty(valueField, ances.Value);
                    }
                    taskNode.ScheduleCode = version.ScheduleCode;
                    taskNode.ModifyState = BomVersionModifyState.Add.ToString();
                    //taskNode.TaskLevel		
                    //taskNode.CalProgress
                    //taskNode.BasePlanStartDate
                    //taskNode.BasePlanEndDate
                    //taskNode.CalPlanStartDate
                    //taskNode.CalPlanEndDate
                    //taskNode.StrandardDuration
                    //taskNode.FactStartDate
                    //taskNode.FactEndDate
                    //taskNode.PredecessorLink
                    //taskNode.UserResource
                    //taskNode.DeviceInfo
                    //taskNode.BOQInfo
                    //taskNode.ContractInfo
                    //taskNode.QBSInfo
                    //taskNode.DocumentInfo
                    //taskNode.Attachments
                    //taskNode.Evidence
                    //taskNode.EvidenceName

                    taskNode.Name = task.GetValue("RowName");
                    #region ExtraField
                    Dictionary<string, object> extra = FillExtraValue(version, task);
                    FormulaHelper.UpdateEntity(taskNode, extra);
                    #endregion
                    taskNode.S_I_WBS_Version.S_I_WBS_Version_Task.Add(taskNode);
                    entities.Set<S_I_WBS_Version_Task>().Add(taskNode);
                }
                else
                {
                    S_I_WBS_Version_Task taskNode = version.S_I_WBS_Version_Task.FirstOrDefault(a => a.Name == task.GetValue("RowName") && a.StructNodeInfo.ID == defineNode.ID);
                    if (taskNode == null)
                    {
                        continue;
                    }
                    #region ExtraField
                    Dictionary<string, object> extra = FillExtraValue(version, task);
                    FormulaHelper.UpdateEntity(taskNode, extra);
                    #endregion
                }
            }
        }

        private Dictionary<string, object> FillExtraValue(S_I_WBS_Version version, Dictionary<string, object> source)
        {
            var colDicList = JsonHelper.ToList(version.ScheduleDefine.ColDefine);
            Dictionary<string, object> extra = new Dictionary<string, object>();
            foreach (var colDic in colDicList)
            {
                var fieldName = colDic.GetValue("fieldName");
                var displayField = colDic.GetValue("displayField");
                if (source.ContainsKey(fieldName))
                {
                    extra.SetValue(fieldName, source.GetValue(fieldName));
                }

                if (source.ContainsKey(displayField))
                {
                    extra.SetValue(displayField, source.GetValue(displayField));//显示值

                    //实际值
                    if (colDic.GetValue("inputType") == "ButtonEdit")
                    {
                        string settings = colDic.GetValue("settings");
                        if (!string.IsNullOrEmpty(settings))
                        {
                            var settingsDic = JsonHelper.ToObject(settings);
                            string selectorKey = settingsDic.GetValue("SelectorKey");
                            if (string.IsNullOrEmpty(selectorKey)) continue;

                            if (selectorKey == "SystemUser")
                            {
                                string val = source.GetValue(displayField);
                                var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.Name == val);
                                if (user != null)
                                {
                                    extra.SetValue(fieldName, user.ID);
                                }
                            }
                            else if (selectorKey == "SystemOrg")
                            {
                                string val = source.GetValue(displayField);
                                var org = baseEntities.Set<S_A_Org>().FirstOrDefault(a => a.Name == val);
                                if (org != null)
                                {
                                    extra.SetValue(fieldName, org.ID);
                                }
                            }
                            else
                            {
                                throw new BusinessException("选择器只支持识别用户和部门");
                            }
                        }
                    }
                    else if (colDic.GetValue("inputType") == "combobox")
                    {
                        string settings = colDic.GetValue("settings");
                        if (!string.IsNullOrEmpty(settings))
                        {
                            string val = source.GetValue(displayField);

                            var settingsDic = JsonHelper.ToObject(settings);
                            string data = settingsDic.GetValue("data");
                            if (string.IsNullOrEmpty(data)) continue;

                            if (!data.Contains("["))
                            {
                                var itemList = EnumBaseHelper.GetEnumDef(data).EnumItem;
                                var item = itemList.FirstOrDefault(a => a.Name == val);
                                if (item != null)
                                {
                                    extra.SetValue(fieldName, item.Code);
                                }
                            }
                            else
                            {
                                var dicList = JsonHelper.ToList(data);
                                var dic = dicList.FirstOrDefault(a => a.GetValue("text") == val);
                                if (dic != null)
                                {
                                    extra.SetValue(fieldName, dic.GetValue("value"));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new BusinessException("导入控件实际值显示值设置仅支持弹出选择框和组合下拉框");
                    }
                }
            }
            return extra;
        }

        private List<Dictionary<string, object>> GetSubList(List<Dictionary<string, object>> list, string rowType, int i)
        {
            var subList = new List<Dictionary<string, object>>();
            var sourceOfNodeTypeList = list.Where(a => a.GetValue("RowType") == rowType).ToList();
            if (i < sourceOfNodeTypeList.Count)
            {
                var source = sourceOfNodeTypeList[i];

                int startIndex = list.IndexOf(source) + 1;
                int endIndex = 0;
                if (i == sourceOfNodeTypeList.Count - 1)
                {
                    endIndex = list.Count - 1;
                }
                else
                {
                    var nextSource = sourceOfNodeTypeList[i + 1];
                    endIndex = list.IndexOf(nextSource) - 1;
                }

                int count = endIndex - startIndex + 1;

                if (list.Count >= count && startIndex < list.Count)
                    subList = list.GetRange(startIndex, endIndex - startIndex + 1);
            }

            return subList;
        }


        #endregion 导入excel

        #region 用Aspose导出计划
        [ValidateInput(false)]
        public ActionResult UseAsposeExportProject()
        {
            string versionID = GetQueryString("VersionID");
            var version = this.GetEntityByID<S_I_WBS_Version>(versionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");
            string filePath = "";
            try
            {
                string templateName = "Aspose_WBS_" + version.ScheduleCode + "_New";
                string templatePath = Server.MapPath("/") + "/ExcelTemplate/" + templateName + ".mpt";
                if (!System.IO.File.Exists(templatePath))
                {
                    throw new Exception("找不到模板【" + templateName + "】");
                }


                JsonResult jr = GetVersionTreeList(versionID, "");
                var list = jr.Data as List<Dictionary<string, object>>;
                Aspose.Tasks.Project project = new Aspose.Tasks.Project(templatePath);

                Task rootTask = project.RootTask;
                int index = 1;

                ExtendedAttributeDefinition text1Define = new ExtendedAttributeDefinition();
                text1Define.FieldName = "Text1";
                text1Define.Alias = "任务类型";
                text1Define.FieldId = ((int)ExtendedAttributeTask.Text1).ToString();
                project.ExtendedAttributes.Add(text1Define);

                ExtendedAttributeDefinition text2Define = new ExtendedAttributeDefinition();
                text2Define.FieldName = "Text2";
                text2Define.Alias = "权重";
                text2Define.FieldId = ((int)ExtendedAttributeTask.Text2).ToString();
                project.ExtendedAttributes.Add(text2Define);

                ExtendedAttributeDefinition text3Define = new ExtendedAttributeDefinition();
                text3Define.FieldName = "Text3";
                text3Define.Alias = "工期(天)";
                text3Define.FieldId = ((int)ExtendedAttributeTask.Text3).ToString();
                project.ExtendedAttributes.Add(text3Define);

                UseAsposeFillTasks(rootTask.Children, project, list, ref index);

                filePath = Path.GetTempFileName();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                project.Save(filePath, SaveFileFormat.MPP);
            }
            catch (Exception ex)
            {
                throw new Exception("生成project文件出错," + ex.Message);
            }

            return File(filePath, "application/vnd.ms-project", version.ScheduleName + ".mpp");
        }
        private void UseAsposeFillTasks(TaskCollection tasks, Aspose.Tasks.Project project,
            IEnumerable<Dictionary<string, object>> dicList, ref int index,
            short level = 0,
            string parentID = "")
        {
            List<Dictionary<string, object>> children = new List<Dictionary<string, object>>();
            if (level == 0)
            {
                //fullid长度最小的肯定是最顶层项
                children = dicList.Where(a => a.GetValue("FullID").Length == dicList.Min(b => b.GetValue("FullID").Length)).OrderBy(a => a.GetValue("SortIndex")).ToList();
            }
            else
            {
                children = dicList.Where(a => a.GetValue("ParentID") == parentID).OrderBy(a => a.GetValue("SortIndex")).ToList();
            }

            level++;
            foreach (var item in children)
            {
                Task prjTask = tasks.Add(item.GetValue("Name"));

                index++;
                //prjTask.Set(Tsk.Id, index);
                //if (parentTask != null)
                //{
                //    //parentTask.TaskDependencies.Add(prjTask,MOIProject.PjTaskLinkType.pjFinishToStart,Type.Missing);
                //}
                prjTask.Set(Tsk.OutlineLevel, level);
                prjTask.Set(Tsk.IsManual, true);
                {
                    ExtendedAttribute attr = new ExtendedAttribute();
                    attr.FieldId = ((int)ExtendedAttributeTask.Text1).ToString();
                    attr.Value = item.GetValue("TaskTypeName");
                    prjTask.ExtendedAttributes.Add(attr);
                }
                {
                    ExtendedAttribute attr = new ExtendedAttribute();
                    attr.FieldId = ((int)ExtendedAttributeTask.Text2).ToString();
                    attr.Value = item.GetValue("Weight");
                    prjTask.ExtendedAttributes.Add(attr);
                }
                {
                    ExtendedAttribute attr = new ExtendedAttribute();
                    attr.FieldId = ((int)ExtendedAttributeTask.Text3).ToString();
                    attr.Value = item.GetValue("PlanDuration");
                    prjTask.ExtendedAttributes.Add(attr);
                }

                if (!string.IsNullOrEmpty(item.GetValue("PlanStartDate")))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(item.GetValue("PlanStartDate"), out dt))
                    {
                        prjTask.Set(Tsk.Start, dt);
                    }
                }

                if (!string.IsNullOrEmpty(item.GetValue("FactStartDate")))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(item.GetValue("FactStartDate"), out dt))
                    {
                        prjTask.Set(Tsk.ActualStart, dt);
                    }
                }

                if (!string.IsNullOrEmpty(item.GetValue("PlanEndDate")))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(item.GetValue("PlanEndDate"), out dt))
                    {
                        prjTask.Set(Tsk.Finish, dt);
                    }
                }

                if (!string.IsNullOrEmpty(item.GetValue("FactEndDate")))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(item.GetValue("FactEndDate"), out dt))
                    {
                        prjTask.Set(Tsk.ActualFinish, dt);
                    }
                }

                //prjTask.Text5 = item.GetValue("ExtraFieldName");
                //prjTask.Text6 = item.GetValue("Extra1FieldName");
                //prjTask.Text7 = item.GetValue("Extra2FieldName");

                UseAsposeFillTasks(prjTask.Children, project, dicList, ref index, level, item.GetValue("WBSID"));
            }
        }
        #endregion

        #region 用Aspose导入计划
        public ActionResult ImportProject(string result)
        {
            string scheduleCode = GetQueryString("scheduleCode");
            string successMsg = GetQueryString("SuccessMsg");
            ViewBag.ProjectKey = scheduleCode + "_Import";
            ViewBag.VersionID = GetQueryString("VersionID");
            ViewBag.ErrorMsg = GetQueryString("ErrorMsg");
            ViewBag.IsSuccess = !string.IsNullOrWhiteSpace(successMsg) ? bool.TrueString : bool.FalseString;
            return View();
        }

        public ActionResult DownloadProjectTemplate(string key)
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            if (path.StartsWith("/"))
                path = Server.MapPath(path);
            var tmplPath = path.EndsWith("\\") ? string.Format("{0}Aspose_WBS_{1}.mpt", path, key) : string.Format("{0}\\Aspose_WBS_{1}.mpt", path, key);
            if (!System.IO.File.Exists(tmplPath))
            {
                throw new Exception("找不到指点key的模板文件，key为：" + key);
            }
            var fileBuffer = FileHelper.GetFileBuffer(tmplPath);

            return File(fileBuffer, "application/vnd.ms-excel", "Aspose_WBS_" + key + ".mpt");
        }
        public ActionResult UseAsposeUploadProject(HttpPostedFileBase Fdata, string projectKey, string versionID)
        {
            if (Fdata == null || Fdata.InputStream == null)
                return RedirectToAction("ImportProject", new { scheduleCode = projectKey, VersionID = versionID, ErrorMsg = "数据文件没有上传，请上传要导入数据文件！" });

            try
            {
                Aspose.Tasks.Project project = new Aspose.Tasks.Project(Fdata.InputStream);
                ExtendedAttributeDefinition text1Define = new ExtendedAttributeDefinition();
                text1Define.FieldName = "Text1";
                text1Define.Alias = "任务类型";
                text1Define.FieldId = ((int)ExtendedAttributeTask.Text1).ToString();
                project.ExtendedAttributes.Add(text1Define);

                ExtendedAttributeDefinition text2Define = new ExtendedAttributeDefinition();
                text2Define.FieldName = "Text2";
                text2Define.Alias = "权重";
                text2Define.FieldId = ((int)ExtendedAttributeTask.Text2).ToString();
                project.ExtendedAttributes.Add(text2Define);

                //ExtendedAttributeDefinition text3Define = new ExtendedAttributeDefinition();
                //text3Define.FieldName = "Text3";
                //text3Define.Alias = "工期(天)";
                //text3Define.FieldId = ((int)ExtendedAttributeTask.Text3).ToString();
                //project.ExtendedAttributes.Add(text3Define);

                List<Task> taskList = new List<Task>();
                FillTaskList(ref taskList, project.RootTask);
                UseAsposePlanSaveProjectData(taskList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ImportProject", new { scheduleCode = projectKey, VersionID = versionID, ErrorMsg = "导入失败！" });
            }
            finally
            {
                Fdata.InputStream.Close();
            }

            return RedirectToAction("ImportProject", new { projectKey = projectKey, SuccessMsg = "导入成功！" });

        }

        public void UseAsposePlanSaveProjectData(List<Task> tasks)
        {
            List<Dictionary<string, object>> dicList = new List<Dictionary<string, object>>();
            foreach (Task task in tasks)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("RowName", task.Get(Tsk.Name));
                dic.SetValue("RowType", "");
                if (task.ExtendedAttributes.Count > 0)
                {
                    dic.SetValue("RowType", task.ExtendedAttributes[0].Value);
                }
                dic.SetValue("Weight", "");
                if (task.ExtendedAttributes.Count > 1)
                {
                    dic.SetValue("Weight", task.ExtendedAttributes[1].Value);
                }

                dic.SetValue("PlanStartDate", "");
                if (task.Get(Tsk.Start) != DateTime.MinValue)
                {
                    dic.SetValue("PlanStartDate", task.Get(Tsk.Start));
                }

                dic.SetValue("PlanEndDate", "");
                if (task.Get(Tsk.Finish) != DateTime.MinValue)
                {
                    dic.SetValue("PlanEndDate", task.Get(Tsk.Finish));
                }

                dic.SetValue("FactStartDate", "");
                if (task.Get(Tsk.ActualStart) != DateTime.MinValue)
                {
                    dic.SetValue("FactStartDate", task.Get(Tsk.ActualStart));
                }

                dic.SetValue("FactEndDate", "");
                if (task.Get(Tsk.ActualFinish) != DateTime.MinValue)
                {
                    dic.SetValue("FactEndDate", task.Get(Tsk.ActualFinish));
                }

                dic.SetValue("PlanDuration", "");
                if ((task.Get(Tsk.Start) != DateTime.MinValue) &&
                    (task.Get(Tsk.Finish) != DateTime.MinValue))
                {
                    TimeSpan span = task.Get(Tsk.Finish) - task.Get(Tsk.Start);
                    dic.SetValue("PlanDuration", span.Days);
                }

                //dic.SetValue("ExtraFieldName", task.Text5);
                //dic.SetValue("Extra1FieldName", task.Text6);
                //dic.SetValue("Extra2FieldName", task.Text7);
                dicList.Add(dic);
            }
            SaveFromList(dicList);
        }
        private void FillTaskList(ref List<Task> taskList, Task parentTask)
        {
            foreach (var cTask in parentTask.Children)
            {
                taskList.Add(cTask);
                FillTaskList(ref taskList, cTask);
            }
        }
        #endregion

    }
}
