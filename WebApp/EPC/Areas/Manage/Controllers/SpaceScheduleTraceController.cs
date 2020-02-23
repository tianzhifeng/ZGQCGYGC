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
    public class SpaceScheduleTraceController : EPCController
    {
        public ActionResult ModeTab()
        {
            var infra = FormulaHelper.GetEntities<InfrastructureEntities>();
            var modeList = infra.S_C_Mode.ToList();
            ViewBag.ModeList = modeList;
            return View();
        }
        public ActionResult MainTab()
        {
            var modeCode = GetQueryString("ModeCode");
            var infra = FormulaHelper.GetEntities<InfrastructureEntities>();
            var mode = infra.S_C_Mode.FirstOrDefault(a => a.Code == modeCode);
            if (mode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的模式");
            var defines = mode.S_C_ScheduleDefine.OrderBy(c => c.SortIndex).ToList();
            if (defines.Count == 0) throw new Formula.Exceptions.BusinessValidationException("该模式没有配置计划");
            ViewBag.ScheduleDefines = defines;
            return View();
        }

        public ActionResult Gantte()
        {
            string defineID = this.GetQueryString("DefineID");
            var engineeringInfos = entities.Set<S_I_Engineering>().ToList();
            ViewBag.DefineID = defineID;

            var infra = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = infra.Set<S_C_ScheduleDefine>().Find(defineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("未找到【" + defineID + "】计划");

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
            ViewBag.MonthFirstDay = DateTime.Now.ToString("yyyy-MM");
            ViewBag.MonthLastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            return View();
        }

        public JsonResult GetGantteTree(string DefineID)
        {
            string beginDate = GetQueryString("beginDate");
            string endDate = GetQueryString("endDate");
            string engineeringCode = GetQueryString("engineeringCode");
            string engineeringName = GetQueryString("engineeringName");
            string projectClass = GetQueryString("projectClass");
            string projManagerName = GetQueryString("projManagerName");

            string sql = @"select *
from (select ID as TID, ID as [UID],ParentID,ParentID as ParentTaskUID,FullID,EngineeringInfoID,StructInfoID,
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
from {1}..S_C_ScheduleDefine_Nodes
 where DefineID='{0}') DefineSortIndex
 on DefineSortIndex.DefineID=TableInfo.StructInfoID 
 left join S_I_Engineering on EngineeringInfoID = S_I_Engineering.ID
 {2}
 order by DSortIndex,SortIndex";

            string whereSql = "where  1=1 ";
            if (!string.IsNullOrEmpty(beginDate) && string.IsNullOrEmpty(endDate))
            {
                DateTime tmp = DateTime.Now;
                DateTime.TryParse(beginDate, out tmp);
                whereSql += string.Format("and Start is not null and cast('{0}' as datetime) <= Finish", beginDate);
            }
            else if (string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime tmp = DateTime.Now;
                DateTime.TryParse(endDate, out tmp);
                whereSql += string.Format("and Finish is not null and cast('{0}' as datetime) >= Start", endDate);
            }
            else if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
            {
                whereSql += string.Format(@"and Start is not null and Finish is not null
                and ((cast('{0}' as datetime) >= Start and cast('{0}' as datetime) <= Finish) or (cast('{0}' as datetime) <= Start and cast('{1}' as datetime) >= Start))", beginDate, endDate);
            }

            if(!string.IsNullOrEmpty(engineeringCode))
            {
                whereSql += " and S_I_Engineering.SerialNumber like '%" + engineeringCode + "%'";
            }
            if (!string.IsNullOrEmpty(engineeringName))
            {
                whereSql += " and S_I_Engineering.Name like '%" + engineeringName + "%'";
            }
            if (!string.IsNullOrEmpty(projectClass))
            {
                whereSql += " and S_I_Engineering.ProjectClass = '" + projectClass + "'";
            }
            if (!string.IsNullOrEmpty(projManagerName))
            {
                whereSql += " and S_I_Engineering.ChargerUserName = '" + projManagerName + "'";
            }

            var infDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, DefineID, infDb.DbName, whereSql));
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
