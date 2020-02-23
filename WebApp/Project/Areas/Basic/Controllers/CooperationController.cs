using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class CooperationController : ProjectController
    {
        #region 提资计划相关

        public ActionResult ListTab()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            var phaseValue = projectInfo.PhaseValue.Split(',');
            var phaseEnumDt = EnumBaseHelper.GetEnumTable("Project.Phase");
            var phaseInfo = new List<Dictionary<string, object>>();
            foreach (var item in phaseValue)
            {
                var rows = phaseEnumDt.Select("value='" + item + "'");
                if (rows.Length == 0) continue;
                var di = new Dictionary<string, object>();
                di.SetValue("PhaseValue", item);
                di.SetValue("PhaseName", rows[0]["text"].ToString());
                phaseInfo.Add(di);
            }
            ViewBag.Phase = phaseInfo;
            ViewBag.ProjectInfoID = projectInfoID;
            ViewBag.SpaceCode = this.Request["SpaceCode"];
            ViewBag.AuthType = this.Request["AuthType"];
            return View();

        }

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

        public ActionResult Plan()
        {
            var def = EnumBaseHelper.GetEnumDef("Project.Major");
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) { throw new Formula.Exceptions.BusinessException("未能找到指定的项目细信息"); }
            var majorType = WBSNodeType.Major.ToString();
            var wbsMajors = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == majorType).Select(d => d.WBSValue).Distinct().ToList();
            var majorEnumList = def.EnumItem.Where(d => wbsMajors.Contains(d.Code)).OrderBy(d => d.SortIndex).ToList();
            var list = new List<Dictionary<string, object>>();
            foreach (var item in majorEnumList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("text", item.Name);
                dic.SetValue("value", item.Code);
                list.Add(dic);
            }
            ViewBag.MajorEnum = JsonHelper.ToJson(list);
            ViewBag.Majors = majorEnumList;

            var subProjectWBSID = GetQueryString("SubProjectID");
            var subProjectMajors = projectInfo.S_W_WBS.Where(a => a.ParentID == subProjectWBSID).Select(a => new { a.Name, a.WBSValue }).ToList();
            if (string.IsNullOrEmpty(subProjectWBSID))
            {
                subProjectMajors = projectInfo.S_W_WBS.Where(a => a.ProjectInfoID == projectInfo.ID && a.WBSType == "Major").Select(a => new { a.Name, a.WBSValue }).Distinct().ToList();
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

        public JsonResult GetList(QueryBuilder qb)
        {
            string SchemeWBSID = this.Request["SchemeWBSID"];
            string ProjectInfoID = this.Request["ProjectInfoID"];
            if (!String.IsNullOrEmpty(SchemeWBSID))
            {
                qb.Add("SchemeWBSID", QueryMethod.Equal, SchemeWBSID);
            }
            qb.Add("ProjectInfoID", QueryMethod.Equal, ProjectInfoID);
            var data = this.entities.Set<S_P_CooperationPlan>().WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetSubProjectList(QueryBuilder qb)
        {
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var wbsType = WBSNodeType.SubProject.ToString();
            var list = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == wbsType).OrderBy(d => d
                .SortIndex).ToList();
            return Json(list);
        }

        public JsonResult ValidCoopPlan(string List)
        {
            var projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var relateWBSs = projectInfo.S_W_WBS.Where(a => !string.IsNullOrEmpty(a.RelateMileStone)).ToList();
            var CoopPlanList = UpdateList<S_P_CooperationPlan>(List);
            var showContent = "";
            foreach (var item in CoopPlanList)
            {
                if (!string.IsNullOrEmpty(item.MileStoneID))
                {
                    //关联了这个提资计划的WBS
                    var relateWBSByThiss = relateWBSs.Where(a => a.RelateMileStone.Contains(item.MileStoneID));
                    foreach (var w in relateWBSByThiss)
                    {
                        var type = "";
                        if (w.PlanStartDate != null && item.PlanStartDate != null)
                            if (w.PlanStartDate < item.PlanStartDate)
                                type = "计划开始时间";
                        if (w.PlanEndDate != null && item.PlanFinishDate != null)
                            if (w.PlanEndDate > item.PlanFinishDate)
                                type += "、计划结束时间";
                        if (!string.IsNullOrEmpty(type))
                            showContent += "【" + item.CooperationContent + "】关联了WBS节点【" + w.Name + "】，将同步修改WBS节点的" + type.TrimStart('、') + "；\n";
                    }
                }
            }
            return Json(new { IsShow = !string.IsNullOrEmpty(showContent), Content = showContent });
        }

        public JsonResult SaveCoopPlan(string List)
        {
            var projectInfoID = this.Request["ProjectInfoID"];
            var subProjectID = this.Request["SchemeWBSID"];
            var linkRoot = false;
            if (this.Request["LinkRoot"] != null)
                linkRoot = this.Request["LinkRoot"].ToLower() == bool.TrueString.ToLower();
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法进行互提资料策划");
            var relateWBSs = projectInfo.S_W_WBS.Where(a => !string.IsNullOrEmpty(a.RelateMileStone)).ToList();
            var wbs = entities.Set<S_W_WBS>().FirstOrDefault(d => d.ID == subProjectID);
            if (wbs == null) wbs = projectInfo.WBSRoot;
            var CoopPlanList = UpdateList<S_P_CooperationPlan>(List);
            foreach (var item in CoopPlanList)
            {
                if (linkRoot && string.IsNullOrEmpty(item.WBSID))
                {
                    item.WBSID = projectInfo.WBSRoot.ID;
                }
                wbs.SaveCooperationPlan(item, true, linkRoot);
                //关联了这个提资计划的WBS
                var relateWBSByThiss = relateWBSs.Where(a => a.RelateMileStone.Contains(item.MileStoneID));
                foreach (var w in relateWBSByThiss)
                {
                    var relateMileStone = JsonHelper.ToList(w.RelateMileStone);
                    var m = relateMileStone.FirstOrDefault(a => a.GetValue("ID") == item.MileStoneID);
                    if (m != null)
                    {
                        m["Name"] = item.CooperationContent;
                        m["PlanStart"] = item.PlanStartDate;
                        m["PlanFinish"] = item.PlanFinishDate;
                    }
                    w.RelateMileStone = JsonHelper.ToJson(relateMileStone);
                    if (item.PlanStartDate != null)
                    {
                        if (w.BasePlanStartDate == null)
                            w.BasePlanStartDate = item.PlanStartDate;
                        else if (w.BasePlanStartDate < item.PlanStartDate)
                            w.BasePlanStartDate = item.PlanStartDate;
                        if (w.PlanStartDate == null)
                            w.PlanStartDate = item.PlanStartDate;
                        else if (w.PlanStartDate < item.PlanStartDate)
                            w.PlanStartDate = item.PlanStartDate;
                    }
                    if (item.PlanFinishDate != null)
                    {
                        if (w.BasePlanEndDate == null)
                            w.BasePlanEndDate = item.PlanFinishDate;
                        else if (w.BasePlanEndDate > item.PlanFinishDate)
                            w.BasePlanEndDate = item.PlanFinishDate;
                        if (w.PlanEndDate == null)
                            w.PlanEndDate = item.PlanFinishDate;
                        else if (w.PlanEndDate > item.PlanFinishDate)
                            w.PlanEndDate = item.PlanFinishDate;
                    }
                }
            }
            entities.SaveChanges();

            //同步关联的收款项的里程碑信息：时间、状态
            //var fo = new Basic.Controllers.MileStoneExecuteController();
            //foreach (var item in projectInfo.S_P_MileStone.ToList())
            //{
            //    fo.SyncReceiptObj(item);
            //}
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }

        public JsonResult Delete()
        {
            var projectInfoID = this.Request["ProjectInfoID"];
            var listIDs = this.Request["ListIDs"];
            var linkRoot = false;
            if (this.Request["LinkRoot"] != null)
                linkRoot = this.Request["LinkRoot"].ToLower() == bool.TrueString.ToLower();
            var relateWBSs = this.entities.Set<S_W_WBS>().Where(a => a.ProjectInfoID == projectInfoID && !string.IsNullOrEmpty(a.RelateMileStone)).ToList();

            //同步关联的收款项的里程碑信息：时间、状态
            var fo = new Basic.Controllers.MileStoneExecuteController();

            foreach (var item in listIDs.Split(','))
            {
                var entity = this.GetEntityByID<S_P_CooperationPlan>(item);
                if (entity == null) continue;
                entity.Delete(false, linkRoot);
                var relateWBSByThiss = relateWBSs.Where(a => a.RelateMileStone.Contains(entity.MileStoneID));

                //fo.UpdateReceiptObjByDelMeliStoneID(entity.MileStoneID);

                foreach (var w in relateWBSByThiss)
                {
                    var relateMileStone = JsonHelper.ToList(w.RelateMileStone);
                    var deleteItem = relateMileStone.FirstOrDefault(a => a.GetValue("ID") == entity.MileStoneID);
                    if (deleteItem != null)
                        relateMileStone.Remove(deleteItem);
                    if (relateMileStone.Count > 0)
                    {
                        DateTime? bStart = DateTime.MinValue, bFinish = DateTime.MaxValue;
                        foreach (var mileS in relateMileStone)
                        {
                            if (mileS["PlanStart"] != null && bStart < (DateTime)mileS["PlanStart"])
                                bStart = (DateTime)mileS["PlanStart"];
                            if (mileS["PlanFinish"] != null && bFinish > (DateTime)mileS["PlanFinish"])
                                bFinish = (DateTime)mileS["PlanFinish"];
                        }
                        if (bStart != DateTime.MinValue)
                            w.BasePlanStartDate = bStart;
                        if (bFinish != DateTime.MaxValue)
                            w.BasePlanEndDate = bFinish;
                    }
                    else
                    {
                        w.BasePlanStartDate = null;
                        w.BasePlanEndDate = null;
                    }
                    w.RelateMileStone = JsonHelper.ToJson(relateMileStone);
                }
            }
            this.entities.SaveChanges();
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }

        public JsonResult GetMileStoneList(string ProjectInfoID, string SchemeWBSID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目");
            var subProject = this.GetEntityByID<S_W_WBS>(SchemeWBSID);
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
            foreach (var major in majors)
            {
                var item = new Dictionary<string, object>();
                item.SetValue("MajorName", major.Name);
                item.SetValue("MajorValue", major.WBSValue);
                var cooperationMileStones = mileStones.Where(d => d.MajorValue == major.WBSValue).ToList();
                foreach (var mileStone in cooperationMileStones)
                {
                    var inMajors = mileStone.OutMajorValue.Split(',');
                    foreach (var inMajorCode in inMajors)
                    {
                        var field = inMajorCode + "_PlanFinishDate";
                        var planFinishDate = item.GetValue(field);
                        if (mileStone.PlanFinishDate.HasValue)
                            planFinishDate += "," + mileStone.ID + "_" + mileStone.PlanFinishDate.Value.ToShortDateString();
                        item.SetValue(field, planFinishDate.TrimStart(',').TrimEnd(','));
                    }
                }
                list.Add(item);
            }
            return Json(list);
        }

        #endregion

        #region 提资执行相关

        public ActionResult Execute()
        {
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息，无法提资");
            ViewBag.RootNode = projectInfo.WBSRoot;
            var majorNode = projectInfo.GetMajorList().FirstOrDefault();
            var majorParentNodeType = "";
            if (majorNode != null)
            {
                majorParentNodeType = majorNode.Parent.WBSType;
            }
            ViewBag.MajorParentNode = majorParentNodeType;
            ViewBag.ModeCode = projectInfo.ModeCode;
            return View();
        }

        public JsonResult GetSubProjectTree()
        {
            string ProjectInfoID = this.Request["ProjectInfoID"];
            var treeViewType = WBSNodeType.Major.ToString();
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目信息对象，无法获得WBS结构");
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            var wbsFO = FormulaHelper.CreateFO<WBSFO>();
            var spaceCode = this.Request["SpaceCode"];
            var result = new List<Dictionary<string, object>>();
            result = wbsFO.CreateWBSTree(ProjectInfoID, treeViewType, false, spaceCode, "", false, false);
            return Json(result);
        }

        public JsonResult GetPlanList(QueryBuilder qb, string WBSID, string SpaceCode, string ProjectInfoID)
        {
            var type = WBSNodeType.CooperationPackage.ToString();
            var sql = string.Format(@"select S_W_WBS.*,S_P_CooperationPlan.ID CooperationPlanID from S_W_WBS with(nolock)
left join S_P_CooperationPlan on S_W_WBS.ID = S_P_CooperationPlan.WBSID
where S_W_WBS.ProjectInfoID='{0}' and S_W_WBS.MajorCode like '%{1}%'
and S_W_WBS.FullID like '%{2}%' and S_W_WBS.WBSType='{3}'", ProjectInfoID, SpaceCode, WBSID, type);
            var coopPackage = this.SqlHelper.ExecuteDataTable(sql);
            return Json(coopPackage);
        }

        public JsonResult GetMajorInputList(QueryBuilder qb, string WBSID, string SpaceCode)
        {
            var sql = string.Format(@"select T_EXE_MajorPutInfo.* from T_EXE_MajorPutInfo
left join S_W_WBS on T_EXE_MajorPutInfo.WBSID = S_W_WBS.ID
where (CoopPlanID = '' or CoopPlanID is null) 
and S_W_WBS.FullID like '%{0}%' and OutMajorValue = '{1}'", WBSID, SpaceCode);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetReceiveList(QueryBuilder qb, string WBSID, string SpaceCode, string ProjectInfoID)
        {
            var wbsDT = this.SqlHelper.ExecuteDataTable("select * from S_W_WBS with(nolock) where ProjectInfoID = '" + ProjectInfoID + "'");
            var wbs = wbsDT.AsEnumerable().FirstOrDefault(a => a["ID"].ToString() == WBSID); 
            if (wbs != null && wbs["WBSType"].ToString() == WBSNodeType.Major.ToString())
            {
                var parentWBS = wbsDT.AsEnumerable().FirstOrDefault(a => a["ID"].ToString() == wbs["ParentID"].ToString());
                if (parentWBS != null)
                    WBSID = parentWBS["ID"].ToString();
            }
            string sql = @"select S_W_CooperationExe.*,case when RelatePlanID is null or RelatePlanID ='' then '计划外提资'
else '计划内提资' end as CooperationType,ff.ID as FlowID from dbo.S_W_CooperationExe
left join S_W_WBS on S_W_CooperationExe.RelateWBSID = S_W_WBS.ID
left join T_EXE_MajorPutInfo ff on ff.id = S_W_CooperationExe.BatchID
where FullID like '%" + WBSID + "%'";
            qb.Add("InMajor", QueryMethod.Equal, SpaceCode);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetMajorShareInfoList(QueryBuilder qb, string WBSID, string ProjectInfoID)
        {
            var sql = string.Format(
                @"select S_D_ShareInfo.* from S_D_ShareInfo left join S_W_WBS on S_D_ShareInfo.SubProjectWBSID = S_W_WBS.ID
where SourceType='{1}' and  FullID like '%{0}%'", WBSID, ShareInfoSourceType.ShareAdd.ToString());
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetShareInfoList(QueryBuilder qb, string WBSID, string CooperationPlan)
        {
            if (string.IsNullOrEmpty(WBSID))
            {
                var data = this.entities.Set<S_D_ShareInfo>().Where(d => d.CooperationPlan == CooperationPlan).WhereToGridData(qb);
                return Json(data);
            }
            else
            {
                var wbs = this.entities.Set<S_W_WBS>().FirstOrDefault(a => a.ID == WBSID);
                if (wbs != null && wbs.WBSType == WBSNodeType.Major.ToString())
                {
                    var sql = string.Format(
                        @"select S_D_ShareInfo.* from S_D_ShareInfo left join S_W_WBS on S_D_ShareInfo.SubProjectWBSID = S_W_WBS.ID
where FullID like '%{0}%'", wbs.ParentID);
                    var data = this.SqlHelper.ExecuteGridData(sql, qb);
                    return Json(data);
                }
                else
                {
                    var sql = string.Format(
                        @"select S_D_ShareInfo.* from S_D_ShareInfo left join S_W_WBS on S_D_ShareInfo.SubProjectWBSID = S_W_WBS.ID
where FullID like '%{0}%'", WBSID);
                    var data = this.SqlHelper.ExecuteGridData(sql, qb);
                    return Json(data);
                }
            }
        }

        public JsonResult GetExecuteList(string WBSID, string CoopPlanID)
        {
            if (!string.IsNullOrEmpty(WBSID))
            {
                var list = this.entities.Set<T_EXE_MajorPutInfo>().Where(d => d.WBSID == WBSID).OrderByDescending(c => c.ID).ToList();
                return Json(list);
            }
            else
            {
                var list = this.entities.Set<T_EXE_MajorPutInfo>().Where(d => d.CoopPlanID == CoopPlanID).OrderByDescending(c => c.ID).ToList();
                return Json(list);
            }
        }

        public JsonResult RefuseRecevie(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_W_CooperationExe>(item.GetValue("ID"));
                if (entity == null) continue;
                entity.Refuse();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ConfirmRecevie(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_W_CooperationExe>(item.GetValue("ID"));
                if (entity == null) continue;
                entity.Confirm();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DelShareInfo(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_D_ShareInfo>(item.GetValue("ID"));
                if (entity == null) continue;
                if (entity.Source == "CAD")
                {
                    throw new Formula.Exceptions.BusinessException("不能删除CAD上传的图纸文件");
                }
                this.entities.Set<S_D_ShareInfo>().Remove(entity);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        #endregion
    }
}
