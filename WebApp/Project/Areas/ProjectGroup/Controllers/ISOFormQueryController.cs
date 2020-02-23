using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcAdapter;
using Formula;
using Project.Logic.Domain;
using System.Data;
using Project.Logic;
using Formula.Helper;
using Config.Logic;
using Config;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class ISOFormQueryController : ProjectController
    {
        public ActionResult Tabs()
        {
            var configentities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var list = configentities.S_T_ProjectMode.ToList();
            ViewBag.Modes = list;
            if (list.Count > 0)
                ViewBag.DefaultModeCode = list.FirstOrDefault().ModeCode;
            return View();
        }

        public ActionResult List()
        {
            string modeCode = this.GetQueryString("ProjectModeCode");
            var mode = Project.Logic.BaseConfigFO.GetMode(modeCode);
            if (mode == null)
            {
                throw new Formula.Exceptions.BusinessException("未能找到编号为【"+modeCode+"】的管理模式");
            }
            var tab = new Tab();
            var list = mode.S_T_ISODefine.Select(d => new { text = d.Name, value = d.ID, sortIndex = d.SortIndex }).OrderBy(p => p.sortIndex).ToList();
            var catagory = new Formula.Category();
            catagory.Name = "表单类型";
            catagory.DisplayName = "表单类型";
            catagory.Key = "ISODefineType";
            catagory.QueryField = "ISODefineType";
            catagory.Multi = false;
            foreach (var define in list)
            {
                var item = new CategroyItem();
                item.Name = define.text;
                item.Value = define.value;
                item.SortIndex = define.sortIndex.HasValue ? define.sortIndex.Value : 0;
                catagory.Items.Add(item);
            }
            catagory.SetDefaultItem(list.FirstOrDefault().value);
            tab.Categories.Add(catagory);


            var deptCategory = CategoryFactory.GetCategory("Market.ManDept", "责任部门", "ProjectChargeDeptID");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var phaseCategory = CategoryFactory.GetCategory("Project.Phase", "设计阶段", "ProjectInfoPhaseValue");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = false;
            tab.Categories.Add(phaseCategory);

            var projectClassCategory = CategoryFactory.GetCategory("Base.ProjectClass", "业务类型", "ProjectInfoClass");
            projectClassCategory.SetDefaultItem();
            projectClassCategory.Multi = false;
            tab.Categories.Add(projectClassCategory);

            var stateCategory = CategoryFactory.GetCategory(typeof(Project.Logic.ProjectCommoneState), "项目状态", "ProjectInfoState");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;
            tab.Categories.Add(stateCategory);

            var flowCategory = CategoryFactory.GetCategory("FlowPhase", "流程状态", "FlowPhase");
            flowCategory.SetDefaultItem();
            flowCategory.Multi = false;
            tab.Categories.Add(flowCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();

        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string modeCode = this.GetQueryString("ModeCode");
            var mode = BaseConfigFO.GetMode(modeCode);
            if (mode == null) throw new Formula.Exceptions.BusinessException("没有找到编号为【" + modeCode + "】的管理模式，无法查询表单");
            string defineInfo = this.GetQueryString("DefineInfo");
            var isoDefine = mode.S_T_ISODefine.FirstOrDefault(d => d.ID == defineInfo);

            if (isoDefine == null) throw new Formula.Exceptions.BusinessException("没有找到表单类型定义，无法查询表单");
            string sql = @"select '{2}' as FormName,'{3}' as LinkUrl,ISOTableInfo.ID,ISOTableInfo.FlowPhase,case when ISOTableInfo.VersionNumber is null or ISOTableInfo.VersionNumber =''
then 1 else ISOTableInfo.VersionNumber end as VersionNumber,
ISOTableInfo.SerialNumber,ISOTableInfo.CreateUser,ISOTableInfo.CreateDate,
S_I_ProjectInfo.Name as ProjectInfoName,S_I_ProjectInfo.Code as ProjectInfoCode,
S_I_ProjectInfo.ProjectClass as ProjectInfoClass,S_I_ProjectInfo.ChargeDeptID as ProjectChargeDeptID,S_I_ProjectInfo.ChargeDeptName as ProjectChargeDeptName,
S_I_ProjectInfo.ChargeUserID as ProjectManager,
S_I_ProjectInfo.ChargeUserName as ProjectManagerName,S_I_ProjectInfo.PhaseValue as ProjectInfoPhaseValue,S_I_ProjectInfo.PhaseName as ProjectInfoPhaseName,
S_I_ProjectInfo.State as ProjectInfoState
from {0} as ISOTableInfo left join S_I_ProjectInfo on  ISOTableInfo.ProjectInfoID=S_I_ProjectInfo.ID where S_I_ProjectInfo.ModeCode='{1}'";
            var data = this.SqlHelper.ExecuteGridData(String.Format(sql, isoDefine.TableName, modeCode, isoDefine.Name, isoDefine.LinkViewUrl), qb);
            return Json(data);
        }
    }
}
