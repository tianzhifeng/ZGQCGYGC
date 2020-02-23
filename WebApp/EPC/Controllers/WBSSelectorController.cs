using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Controllers
{
    public class WBSSelectorController : EPCController
    {
        public JsonResult GetWBSTree(string EngineeringInfoID, string ScheduleCode)
        {
            string sql = @"select * from (select ID,ParentID,FullID,EngineeringInfoID,StructInfoID,
                         Name,Code,Value,NodeType, Extra5Field as ProcurementMethod,
                         NodeType as TaskType,NodeTypeName as TaskTypeName, SortIndex,DeviceInfo from S_I_WBS
                         union select ID,ParentID,WBSFullID as FullID,EngineeringInfoID,StructInfoID,
                         Name,Code,Name as Value,'Task' as NodeType, Extra5Field as ProcurementMethod,
                         TaskType,TaskTypeName,SortIndex, 
                         DeviceInfo from S_I_WBS_Task ) TableInfo
                         inner join (select StructInfoID as DefineID,{2}..S_C_ScheduleDefine_Nodes.SortIndex as DSortIndex,
                         case when Visible is null then 0 else Visible end as Visible
                         from {2}..S_C_ScheduleDefine_Nodes inner join {2}..S_C_ScheduleDefine on
                         {2}..S_C_ScheduleDefine.ID = {2}..S_C_ScheduleDefine_Nodes.DefineID
                          where {2}..S_C_ScheduleDefine.Code ='{1}') DefineSortIndex
                          on DefineSortIndex.DefineID=TableInfo.StructInfoID  where EngineeringInfoID = '{0}'   and Visible=1 
                          order by DSortIndex,SortIndex";
            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, ScheduleCode, infDb.DbName));
            return Json(dt);
        }

        public JsonResult GetTreeList(string EngineeringInfoID, string ScheduleCode)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程，无法加载WBS");
            var define = engineeringInfo.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == ScheduleCode);
            if (define == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + ScheduleCode + "】的计划定义，无法加载WBS");
            }
            string sql = @"  select * from (select ID,ParentID,FullID,EngineeringInfoID,StructInfoID,
Name,Code,Value,NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
NodeType as TaskType,NodeTypeName as TaskTypeName,
[Weight], SortIndex,'' as PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo from S_I_WBS with(nolock)
union select ID,ParentID,WBSFullID as FullID,EngineeringInfoID,StructInfoID,
Name,Code,Name as Value,TaskType as NodeType,ChargerUser,ChargerUserName,
ChargerDept,ChargerDeptName,BasePlanStartDate,BasePlanEndDate,PlanStartDate,PlanEndDate,
CalPlanStartDate,CalPlanEndDate,PlanDuration,StrandardDuration,FactStartDate,FactEndDate,CalProgress,Progress,
TaskType,TaskTypeName,[Weight],SortIndex,PredecessorLink,
ExtraField,ExtraFieldName,Extra1Field,Extra1FieldName,Extra2Field,Extra2FieldName,Extra3Field,Extra3FieldName,Extra4Field,Extra4FieldName,UserResource,
DeviceInfo,BOQInfo,ContractInfo,QBSInfo,DocumentInfo
 from S_I_WBS_Task with(nolock) ) TableInfo  
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
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, define.ID, infDb.DbName));
            return Json(dt);
        }
    }
}
