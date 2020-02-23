using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using Project.Logic;
using Config;
using Config.Logic;
using Formula;
using MvcAdapter;
using System.Collections.Specialized;
using Formula.Helper;
using Formula.DynConditionObject;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class MileStoneTrackController : ProjectController
    {

        public ActionResult MileStoneTrackTab()
        {
            var configentities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var list = configentities.S_T_ProjectMode.ToList();
            ViewBag.Modes = list;
            if (list.Count > 0)
                ViewBag.DefaultModeCode = list.FirstOrDefault().ModeCode;
            return View();
        }

        public ActionResult MileStoneTrack()
        {
            var configentities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var ProjectModeCode = GetQueryString("ProjectModeCode");

            S_T_ProjectMode pm;
            if (string.IsNullOrEmpty(ProjectModeCode))
            {
                pm = configentities.Set<S_T_ProjectMode>().FirstOrDefault();
            }
            else
            {
                pm = BaseConfigFO.GetMode(ProjectModeCode);
            }
            if (pm == null)
                throw new Formula.Exceptions.BusinessException("找不到编号为【" + ProjectModeCode + "】的项目模式");

            ViewBag.ProjectMode = pm;

            var phaseInfo = pm.S_T_MileStone.Where(d => d.MileStoneType == MileStoneType.Normal.ToString())
                .Select(d => d.PhaseValue).ToList();

            var phaseList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Phase).Where(d => phaseInfo.Count(c=>c.Contains(d.Code))>0).
                OrderBy(d => d.SortIndex).ToList();

            var MileStoneList = pm.S_T_MileStone.Where(d => d.MileStoneType == MileStoneType.Normal.ToString()).
                OrderBy(d => d.SortIndex).ToList();

            ViewBag.MileStoneList = JsonHelper.ToJson(MileStoneList);
            var phaseArray = phaseList.Select(d => new { value = d.Code, text = d.Name }).ToList();
            ViewBag.PhaseArray = JsonHelper.ToJson(phaseArray);
            ViewBag.PhaseList = JsonHelper.ToJson(phaseList);
            var defaultPhase = phaseList.FirstOrDefault();
            if (defaultPhase != null)
                ViewBag.PhaseValue = defaultPhase.Code;
            else
                ViewBag.PhaseValue = "";
            ViewBag.PhaseMultiSelect = true;
            if (this.GetQueryString("PhaseMultiSelect") == false.ToString().ToLower())
                ViewBag.PhaseMultiSelect = false;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var ProjectModeCode = GetQueryString("ProjectModeCode");
            var pm = BaseConfigFO.GetMode(ProjectModeCode);
            if (pm == null)
                throw new Formula.Exceptions.BusinessException("找不到编号为【" + ProjectModeCode + "】的项目模式");
            var mileStoneDefines = pm.S_T_MileStone.Where(d => d.MileStoneType == MileStoneType.Normal.ToString()).ToList();

            var phaseValues = qb.Items.FirstOrDefault(d => d.Field == "PhaseValues");
            qb.Items.RemoveWhere(d => d.Field == "PhaseValues");
            var mileStoneDefineList = new List<S_T_MileStone>();

            if (phaseValues != null)
            {
                var phaseValue = phaseValues.Value.ToString();
                foreach (var item in mileStoneDefines)
                {
                    var definePhases = item.PhaseValue.Split(',');
                    var phases = phaseValue.Split(',');
                    foreach (var defPhase in definePhases)
                    {
                        if (phases.Contains(defPhase))
                        {
                            mileStoneDefineList.Add(item);
                            break;
                        }
                    }

                }
                //mileStoneDefines = mileStoneDefines.Where(d => phaseValue.Contains(d.PhaseValue)).ToList();
                var phaseValueList = phaseValue.Split(',');
                for (int i = 0; i < phaseValueList.Length; i++)
                {
                    qb.Add("PhaseValue", QueryMethod.InLike, phaseValueList[i], "quickSearchGroup" ,"");
                }
            }
            var mileStoneDefineIDs = mileStoneDefineList.Select(d => d.ID).ToList();

            var items = qb.Items.Where(d => d.Field == "PlanFinishDate").ToList();
            qb.Items.RemoveWhere(d => d.Field == "PlanFinishDate");

            //Count数据
            var normal = MileStoneType.Normal.ToString();
            var projectsquery = this.entities.Set<S_I_ProjectInfo>().Include("S_P_MileStone").Where(c => c.ModeCode == ProjectModeCode)
                .Where(qb);

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (item.Value == null) continue;
                    var date = Convert.ToDateTime(item.Value);
                    if (item.Method == QueryMethod.GreaterThanOrEqual)
                        projectsquery = projectsquery.Where(d => d.S_P_MileStone.Where(p => p.PlanFinishDate >= date).Count() > 0);
                    else
                        projectsquery = projectsquery.Where(d => d.S_P_MileStone.Where(p => p.PlanFinishDate <= date).Count() > 0);
                }
            }

            var countQuery = projectsquery
                .Select(c => c.S_P_MileStone.Where(x => x.MileStoneType == normal && mileStoneDefineIDs.Contains(x.TemplateID)))
                .AsEnumerable()
                .SelectMany((x, y) => x);
            var finish = ProjectCommoneState.Finish.ToString();
            var dateNow = DateTime.Now.Date;

            var countlist = new Dictionary<string, int>();
            countlist.Add(MileStoneShowStatus.DelayFinish.ToString(), countQuery.Where(c =>
                        c.State == finish && c.MileStoneType == "Normal" && c.PlanFinishDate.HasValue
                        && c.FactFinishDate.HasValue
                        && c.FactFinishDate.Value > c.PlanFinishDate.Value).Count()
                );
            countlist.Add(MileStoneShowStatus.NormalFinish.ToString(), countQuery.Where(c =>
                       c.State == finish && c.MileStoneType == "Normal" && (!c.PlanFinishDate.HasValue
                       || !c.FactFinishDate.HasValue
                       || c.FactFinishDate.Value <= c.PlanFinishDate.Value)).Count()
                );
            countlist.Add(MileStoneShowStatus.DelayUndone.ToString(), countQuery.Where(c =>
                       c.State != finish && c.MileStoneType == "Normal" &&
                       c.PlanFinishDate.HasValue &&
                       dateNow > c.PlanFinishDate.Value).Count()
                );
            countlist.Add(MileStoneShowStatus.NormalUndone.ToString(), countQuery.Where(c =>
                       c.State != finish && c.MileStoneType == "Normal" &&
                       c.PlanFinishDate.HasValue &&
                       (dateNow <= c.PlanFinishDate.Value)).Count()
                );

            //列表数据
            var result = new List<Dictionary<string, object>>();
            foreach (var project in projectsquery.ToList())
            {
                var projectdic = project.ToDic("ID,Name,Code,ChargeUserName,ChargeDeptName,ModeCode,ProjectClass");
                var mileStoneList = project.S_P_MileStone.Where(d => d.MileStoneType == normal && mileStoneDefineIDs.Contains(d.TemplateID));
                foreach (var define in mileStoneDefines)
                {
                    var list = new List<Dictionary<string, object>>();
                    var mileStones = mileStoneList.Where(d => d.TemplateID == define.ID).ToList();
                    foreach (var mileStone in mileStones)
                    {
                        var mileStoneInfo = new Dictionary<string, object>();
                        var wbs = project.S_W_WBS.FirstOrDefault(d => d.ID == mileStone.WBSID);
                        if (wbs == null)
                            continue;
                        mileStoneInfo.SetValue("WBSName", wbs.Name);
                        mileStoneInfo.SetValue("WBSType", wbs.StructNodeInfo.Name);
                        mileStoneInfo.SetValue("ShowStatus", mileStone.ShowStatus);
                        mileStoneInfo.SetValue("MileStoneID", mileStone.ID);
                        mileStoneInfo.SetValue("FactFinishDate", mileStone.FactFinishDate);
                        mileStoneInfo.SetValue("PlanFinishDate", mileStone.PlanFinishDate);
                        mileStoneInfo.SetValue("State", mileStone.State);
                        list.Add(mileStoneInfo);
                    }
                    projectdic.SetValue("Template" + define.ID, list);
                }
                result.Add(projectdic);
            }

            GridData gridData = new GridData(result);
            gridData.total = qb.TotolCount;
            var dic = FormulaHelper.ModelToDic(gridData);

            dic.SetValue("ShowStatusCount", countlist);

            return Json(dic);
        }
    }
}
