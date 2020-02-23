using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Config;
using System.Data;
using Formula.Helper;
using System.Reflection;
using Config.Logic;

namespace Project.Areas.Monitor.Controllers
{
    public class MileStoneTrackController : ProjectController<S_P_MileStone>
    {

        public ActionResult MileStoneTrackTab()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var wbsID = this.GetQueryString("WBSID");
            if (String.IsNullOrEmpty(wbsID))
            {
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
                wbsID = projectInfo.WBSRoot.ID;
            }
            ViewBag.WBSID = wbsID;
            return View();
        }

        #region 里程碑跟踪
        public ActionResult MainTab()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var subProjectType = WBSNodeType.SubProject.ToString();
            var subProjectList = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == subProjectType).ToList();
            ViewBag.SubProjectList = subProjectList;
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            return View();
        }

        public ActionResult MileStoneTab()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var subProjectType = WBSNodeType.SubProject.ToString();
            var subProjectList = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == subProjectType).ToList();
            ViewBag.SubProjectList = subProjectList;
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            return View();
        }

        public ActionResult CooperationTab()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var subProjectType = WBSNodeType.SubProject.ToString();
            var subProjectList = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == subProjectType).ToList();
            ViewBag.SubProjectList = subProjectList;
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            return View();
        }

        public ActionResult CooperationTrackTab()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var wbsID = this.GetQueryString("WBSID");
            if (String.IsNullOrEmpty(wbsID))
            {
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
                wbsID = projectInfo.WBSRoot.ID;
            }
            ViewBag.WBSID = wbsID;
            return View();
        }

        public JsonResult GetSubProjectList(QueryBuilder qb)
        {
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var wbsType = WBSNodeType.SubProject.ToString();
            var list = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == wbsType).OrderBy(d => d
                .SortIndex).ToList();
            return Json(list);
        }

        public ActionResult MileStoneTrackList()
        {
            return View();
        }

        public JsonResult GetTrackList(QueryBuilder qb)
        {
            string sql = @"select S_W_WBS.Name as SubProjectName, S_P_MileStone.*,
case when FactFinishDate is not null and PlanFinishDate is not null
and FactFinishDate>PlanFinishDate then 'DelayFinish'
when FactFinishDate is not null and PlanFinishDate is not null
and FactFinishDate<=PlanFinishDate then 'NormalFinish'
when PlanFinishDate is null then 'NotPlan'
when FactFinishDate is null and PlanFinishDate<getdate() then 'DelayUndone'
else  'NormalUndone' end as ShowStatus
 from dbo.S_P_MileStone
left join S_W_WBS on S_P_MileStone.WBSID= S_W_WBS.ID";

            qb.PageSize = 0;
            string ProjectInfoID = GetQueryString("ProjectInfoID");
            qb.Add("ProjectInfoID", QueryMethod.Equal, ProjectInfoID);
            string WBSID = this.GetQueryString("WBSID");
            if(!String.IsNullOrEmpty(WBSID))
                qb.Add("WBSID", QueryMethod.Equal, WBSID);
            qb.SetSort("SortIndex","asc");
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            var dic = FormulaHelper.ModelToDic(data);

            var list = new Dictionary<string, int>();
            list.Add(MileStoneShowStatus.DelayFinish.ToString(), Convert.ToInt32(dt.Compute("count(ID)", " ShowStatus='" + MileStoneShowStatus.DelayFinish.ToString() + "'")));
            list.Add(MileStoneShowStatus.NormalFinish.ToString(), Convert.ToInt32(dt.Compute("Count(ID)", " ShowStatus='" + MileStoneShowStatus.NormalFinish.ToString() + "'")));
            list.Add(MileStoneShowStatus.DelayUndone.ToString(), Convert.ToInt32(dt.Compute("Count(ID)", " ShowStatus='" + MileStoneShowStatus.DelayUndone.ToString() + "'")));
            list.Add(MileStoneShowStatus.NormalUndone.ToString(), Convert.ToInt32(dt.Compute("Count(ID)", " ShowStatus='" + MileStoneShowStatus.NormalUndone.ToString() + "'")));

            dic.SetValue("ShowStatusCount", list);
            return Json(dic);
        }

        public JsonResult GetChangeList(string MileStoneID)
        {
            var list = entities.Set<S_P_MileStoneHistory>().Where(c => c.MileStoneID == MileStoneID).OrderBy(c => c.ID);
            return Json(list);
        }

        public JsonResult GetStateNumberList(string ProjectInfoID)
        {
            var normal = MileStoneType.Normal.ToString();
            var list = this.entities.Set<S_P_MileStone>()
                .Where(c => c.ProjectInfoID == ProjectInfoID && c.MileStoneType == normal)
                .ToList()
               .GroupBy(c => c.ShowStatus)
               .Select(
                   c => new KeyValuePair<string, int>(c.Key, c.Count())
               );

            return Json(list);
        }
        #endregion

        #region 互提矩阵表跟踪
        public ActionResult CooperationTrack()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            string parentWBSID = GetQueryString("WBSID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) { throw new Formula.Exceptions.BusinessException("未能找到指定的项目细信息"); }
            var subProject = this.GetEntityByID<S_W_WBS>(parentWBSID);
            var subProjectMajors = this.entities.Set<S_W_WBS>().Where(a => a.ParentID == parentWBSID).Select(a => new { a.Name, a.WBSValue }).ToList();
            if (string.IsNullOrEmpty(parentWBSID) || subProject.WBSType == WBSNodeType.Project.ToString())
            {
                subProjectMajors = this.entities.Set<S_W_WBS>().Where(a => a.ProjectInfoID == projectInfo.ID && a.WBSType == "Major").Select(a => new { a.Name, a.WBSValue }).Distinct().ToList();
            }
            var list2 = new List<Dictionary<string, object>>();
            foreach (var item in subProjectMajors)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("text", item.Name);
                dic.SetValue("value", item.WBSValue);
                list2.Add(dic);
            }
            ViewBag.SubProjectMajor = JsonHelper.ToJson(list2);
            ViewBag.SubProjectMajorEnum = list2;
            ViewBag.RootWBSID = projectInfo.WBSRoot.ID;
            return View();
        }

        public JsonResult GetCooperationTrackList(string WBSID)
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目");
            var subProject = this.GetEntityByID<S_W_WBS>(WBSID);
            var mileStoneType = MileStoneType.Cooperation.ToString();
            var mileStones = new List<S_P_MileStone>();
            var majors = new List<S_W_WBS>().Select(a => new { a.WBSValue, a.Name });
            if (subProject == null)
            {
                mileStones = this.entities.Set<S_P_MileStone>().Where(d => d.MileStoneType == mileStoneType && d.ProjectInfoID == projectInfo.ID).ToList();
                majors = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()).OrderBy(d => d.SortIndex).ToList().Select(a => new { a.WBSValue, a.Name }).Distinct();
            }
            else
            {
                mileStones = this.entities.Set<S_P_MileStone>().Where(d => d.MileStoneType == mileStoneType && d.WBSID == subProject.ID).ToList();
                if (subProject.WBSType == WBSNodeType.Project.ToString())
                    majors = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()).OrderBy(d => d.SortIndex).ToList().Select(a => new { a.WBSValue, a.Name }).Distinct();
                else
                    majors = subProject.Children.OrderBy(d => d.SortIndex).ToList().Select(a => new { a.WBSValue, a.Name }).Distinct();
            }
            var list = new List<Dictionary<string, object>>();

            var countlist = EnumBaseHelper.GetEnumDef(typeof(MileStoneShowStatus)).EnumItem.ToDictionary(c => c.Code, c => 0);

            foreach (var major in majors)
            {
                var item = new Dictionary<string, object>();
                item.SetValue("Name", major.Name);
                item.SetValue("MajorValue", major.WBSValue);
                var cooperationMileStones = mileStones.Where(d => d.MajorValue == major.WBSValue).ToList();
                foreach (var mileStone in cooperationMileStones)
                {
                    var inMajors = mileStone.OutMajorValue.Split(',');
                    string status = mileStone.ShowStatus;
                    foreach (var inMajorCode in inMajors)
                    {

                        var field = inMajorCode + "_FinishDate";
                        var finishDate = item.GetValue(field);
                        if (mileStone.PlanFinishDate.HasValue)
                        {
                            finishDate += "," + status + "_" + mileStone.ID + "_" + mileStone.PlanFinishDate.Value.ToShortDateString();
                            if (mileStone.FactFinishDate.HasValue)
                                finishDate += "_" + mileStone.FactFinishDate.Value.ToShortDateString() + "";
                        }
                        item.SetValue(field, finishDate.TrimStart(',').TrimEnd(','));
                    }
                    var count = countlist.GetValue(status);
                    countlist.SetValue(status, count + 1);
                }
                list.Add(item);
            }
            return Json(new
            {
                data = list,
                ShowStatusCount = countlist,
            });
           
        }

        #endregion

        #region 互提计划明细跟踪



        public ActionResult CooperationDetailPlanTrack()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行互提策划");
            var list = projectInfo.GetMajorList().Select(d => new { text = d.Name, value = d.WBSValue }).Distinct().ToList();
            ViewBag.Major = JsonHelper.ToJson(list);
            return View();
        }

        public JsonResult GetCooperationPlanTrackList(QueryBuilder qb, string WBSID)
        {
            qb.PageSize = 0;
            var data = this.entities.Set<S_P_CooperationPlan>().Where(d => d.SchemeWBSID == WBSID).WhereToGridData(qb);
            return Json(data);
        }

        #endregion

    }
}
