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
    public class ScheduleVersionController : EPCFormContorllor<S_I_WBS_Version>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.VersionID = dic.GetValue("ID");
            ViewBag.ScheduleCode = dic.GetValue("ScheduleCode");
            var code = dic.GetValue("ScheduleCode");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == code);
            ViewBag.Define = define;
            var attrDefine = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(define.AttrDefine)) attrDefine = JsonHelper.ToList(define.AttrDefine);
            ViewBag.AttrDefine = JsonHelper.ToJson(attrDefine);
            dic.SetValue("ShowType", "Diff");

            #region 是否启用虚加载

            int wbsNodeCount = 0, wbsTaskCount = 0;
            var wbsNodeCountObj = this.EPCSQLDB.ExecuteScalar("select count(ID) from S_I_WBS_Version_Node with(nolock) WHERE VersionID='" + dic.GetValue("ID") + "'");
            var wbsTaskCountObj = this.EPCSQLDB.ExecuteScalar("select count(ID) from S_I_WBS_Version_Task with(nolock) WHERE VersionID='" + dic.GetValue("ID") + "'");
            int.TryParse((wbsNodeCountObj ?? "").ToString(), out wbsNodeCount);
            int.TryParse((wbsTaskCountObj ?? "").ToString(), out wbsTaskCount);
            ViewBag.VirtualScroll = "false";
            if ((wbsTaskCount + wbsNodeCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }
            #endregion
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetVersionTreeList(string VersionID, string ShowType)
        {
            string sql = @"  select * from (select ID,WBSID,ParentID,FullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'0' as IsEnum,IsLocked,
case when IsLocked ='True' then PlanStartDate else null end as BaseStart,
case when IsLocked ='True' then PlanEndDate else null end as BaseFinish,'' as PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,Extra5Field,Extra5FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
from S_I_WBS_Version_Node
union select ID,TaskID as WBSID,ParentID as WBSParentID,WBSFullID,EngineeringInfoID,VersionID,StructInfoID,
Name,Code,Name as Value,'Task' as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,ModifyState,'1' as Visible,'1' as CanAdd,'1' as CanEdit, '1' as CanDelete,'0' as IsEnum,'False' as IsLocked,
null as BaseStart, null as BaseFinish,PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,Extra5Field,Extra5FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
 from S_I_WBS_Version_Task ) TableInfo  where VersionID = '{0}' ";
            if (ShowType.ToLower() == "diff")
            {
                sql += " and ModifyState!='" + BomVersionModifyState.Normal.ToString() + "'";
            }
            else if (ShowType.ToLower() == "new")
            {
                sql += " and ModifyState!='" + BomVersionModifyState.Remove.ToString() + "'";
            }
            sql += " order by SortIndex";
            var dt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, VersionID, BomVersionModifyState.Remove.ToString()));

            var version = this.GetEntityByID<S_I_WBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");
            var defineNodes = version.ScheduleDefine.S_C_ScheduleDefine_Nodes.ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(row);

                var structInfoID = row["StructInfoID"] == null || row["StructInfoID"] == DBNull.Value ? "" : row["StructInfoID"].ToString();
                var defineNode = defineNodes.FirstOrDefault(c => c.StructInfoID == structInfoID);
                if (defineNode != null)
                {
                    if (defineNode.Visible != "1")
                    {
                        continue;
                    }
                }
                result.Add(dic);
            }
            return Json(result);
        }

        protected override void OnFlowEnd(S_I_WBS_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
