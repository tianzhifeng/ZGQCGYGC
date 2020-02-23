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
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class MileStoneExecuteController : ProjectController<S_P_MileStone>
    {

        #region 专业级里程碑
        public ActionResult MajorMileStoneExecuteList()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑填报");
            var majors = projectInfo.GetMajors();
            if (majors.Count == 0) throw new Formula.Exceptions.BusinessException("未进行专业策划的项目，无法填报专业里程碑");
            ViewBag.ProjectInfoID = projectInfoID;
            ViewBag.DefaultMajorCode = majors.FirstOrDefault().GetValue("Value");
            ViewBag.Majors = majors;
            ViewBag.RootNode = JsonHelper.ToJson(projectInfo.WBSRoot);
            ViewBag.FinishState = ProjectCommoneState.Finish.ToString();
            ViewBag.ExecuteState = ProjectCommoneState.Execute.ToString();
            return View();
        }
        public JsonResult GetMajorMileStoneList(QueryBuilder qb, string MajorCode, string ProjectInfoID)
        {
            qb.Add("MajorValue", QueryMethod.Equal, MajorCode);
            return base.GetList(qb);
        }
        #endregion

        #region 项目里程碑

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
        
        public ActionResult MileStoneExecuteList()
        {
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.FinishState = ProjectCommoneState.Finish.ToString();
            ViewBag.ExecuteState = ProjectCommoneState.Execute.ToString();
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息");
            ViewBag.ProjectInfo = JsonHelper.ToJson(projectInfo);
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.GetList(qb);
        }

        public JsonResult GetSubProjectList(QueryBuilder qb)
        {
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var wbsType = WBSNodeType.SubProject.ToString();
            var list = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == wbsType).OrderBy(d => d
                .SortIndex).ToList();
            return Json(list);
        }

        //public JsonResult BindingReceiptObject(string ProjectInfoID,string mileStoneList,string receiptObjList)
        //{
        //    //var marketEntities = FormulaHelper.GetEntities<MarketEntities>();
        //    //var projectInfo= this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
        //    //if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息，无法绑定");
         
        //    //var selectMileStones= JsonHelper.ToList(mileStoneList);
        //    //var selectReceiptObjects = JsonHelper.ToList(receiptObjList);

        //    //string selectRecpObjIDs = String.Join(",", selectReceiptObjects.Select(d => d.GetValue("ID")).ToList());
        //    //string selectRecpObjNames = String.Join(",", selectReceiptObjects.Select(d => d.GetValue("Name")).ToList());
        //    //var selectMileStoneIDs = String.Join(",", selectMileStones.Select(d => d.GetValue("ID")).ToList());
        //    //var selectMileStoneNames = String.Join(",", selectMileStones.Select(d => d.GetValue("Name")).ToList());
        //    //var _mileStoneList = new List<S_P_MileStone>();
        //    //var _delReceiptObjList = new List<S_C_ManageContract_ReceiptObj>();
        //    //foreach (var item in selectMileStones)
        //    //{
        //    //    string mileStoneID = item.GetValue("ID");
        //    //    var mileStone = this.GetEntityByID(mileStoneID);
        //    //    //如果里程碑已经关联了其他收款项，记录下删除关联关系的收款项
        //    //    GetDelReceiptObjList(_delReceiptObjList, mileStoneID, selectRecpObjIDs, marketEntities);
        //    //    //重新关联所选收款项
        //    //    mileStone.BindReceiptObjID = selectRecpObjIDs.TrimEnd(',');
        //    //    mileStone.BindReceiptObjName = selectRecpObjNames.TrimEnd(',');
        //    //    if (!_mileStoneList.Contains(mileStone))
        //    //        _mileStoneList.Add(mileStone);
        //    //}
        //    //this.entities.SaveChanges(); 
        //    ////同步每个里程碑关联的收款项的里程碑信息
        //    //foreach (var item in _mileStoneList)
        //    //{
        //    //    SyncReceiptObj(item);
        //    //}
        //    ////删除关联关系的收款项重新同步里程碑信息
        //    //foreach (var item in _delReceiptObjList)
        //    //{
        //    //    SyncMileStoneInfo(item);
        //    //}
        //    //marketEntities.SaveChanges();
            
        //    return Json("");
        //}
        ////更新收款项 根据 删除的里程碑ID
        //public void UpdateReceiptObjByDelMeliStoneID(string mileStoneID)
        //{ 
        //    //if (!string.IsNullOrEmpty(mileStoneID))
        //    //{ 
        //    //    var receiptObjList = FormulaHelper.GetEntities<MarketEntities>().Set<S_C_ManageContract_ReceiptObj>().Where(c => c.MileStoneID.Contains(mileStoneID)).ToList();
        //    //    foreach (var receiptObj in receiptObjList)
        //    //    {
        //    //        RemoveReceiptObjMileStoneInfo(receiptObj, mileStoneID);
        //    //        SyncMileStoneInfo(receiptObj);
        //    //    } 
        //    //}
        //}
        ////重新赋值收款项的里程碑ID、Name，去除当前里程碑
        //public void RemoveReceiptObjMileStoneInfo(S_C_ManageContract_ReceiptObj receiptObj,string removeMileStoneID)
        //{ 
        //    string[] receiptObjMileStoneIDList = receiptObj.MileStoneID.Split(',');
        //    string[] receiptObjMileStoneNameList = receiptObj.MileStoneName.Split(',');
        //    string newReceiptObjMileStoneIDs = string.Empty;
        //    string newReceiptObjMileStoneNames = string.Empty;
        //    for (int i = 0; i < receiptObjMileStoneIDList.Length; i++)
        //    {
        //        if (receiptObjMileStoneIDList[i] != removeMileStoneID && receiptObjMileStoneIDList[i] != "")
        //        {
        //            newReceiptObjMileStoneIDs += receiptObjMileStoneIDList[i] + ",";
        //            newReceiptObjMileStoneNames += receiptObjMileStoneNameList[i] + ",";
        //        }
        //    }
        //    receiptObj.MileStoneID = newReceiptObjMileStoneIDs.TrimEnd(',');
        //    receiptObj.MileStoneName = newReceiptObjMileStoneNames.TrimEnd(',');
        //}
        ////获取需要删除的里程碑 收款项
        //public void GetDelReceiptObjList(List<S_C_ManageContract_ReceiptObj> delReceiptObjList,string mileStoneID,string selectRecpObjIDs, MarketEntities marketEntities)
        //{
        //    if (!string.IsNullOrEmpty(mileStoneID))
        //    {
        //        //获得当前里程碑关联的收款项，但是本次没有选中的收款项，即需要删除和当前里程碑关联关系的收款项
        //        var tempDelReceiptObjList = marketEntities.Set<S_C_ManageContract_ReceiptObj>().Where(c => c.MileStoneID.Contains(mileStoneID) && !selectRecpObjIDs.Contains(c.ID)).ToList();
        //        foreach (var receiptObj in tempDelReceiptObjList)
        //        {
        //            if (string.IsNullOrEmpty(receiptObj.MileStoneID))
        //            {
        //                continue;
        //            }
        //            //重新赋值收款项的里程碑ID、Name，去除当前里程碑
        //            RemoveReceiptObjMileStoneInfo(receiptObj, mileStoneID);
        //            if (!delReceiptObjList.Contains(receiptObj))
        //            {
        //                delReceiptObjList.Add(receiptObj);
        //            }
        //        }
        //    }
        //}
        ////同步里程碑的计划时间、完成时间给关联的收款项
        //public void SyncReceiptObj(S_P_MileStone mileStone)
        //{
        //    if (mileStone==null)
        //    {
        //        return;
        //    }
        //    var marketEntities = FormulaHelper.GetEntities<MarketEntities>();
        //    if (mileStone.S_I_ProjectInfo == null)
        //        throw new Formula.Exceptions.BusinessException("mileStone.S_I_ProjectInfo对象为null");
        //    var marketProject = marketEntities.S_I_Project.FirstOrDefault(d => d.ID == mileStone.S_I_ProjectInfo.MarketProjectInfoID);
        //    if (marketProject == null) throw new Formula.Exceptions.BusinessException("没有找到相关联的项目信息，无法绑定收款项");
        //    if (string.IsNullOrEmpty(mileStone.BindReceiptObjID))
        //    {
        //        return;
        //    }
        //    var receiptObjIds = mileStone.BindReceiptObjID.Split(',');
        //    var receiptObjectList = marketEntities.Set<S_C_ManageContract_ReceiptObj>().Where(d => receiptObjIds.Contains(d.ID)).ToList();
        //    foreach (var item in receiptObjectList)
        //    {
        //        //bug 收款项已经关联的里程碑 会被新的里程碑覆盖，应该是追加
        //        if (string.IsNullOrEmpty(item.MileStoneID))
        //        {
        //            item.MileStoneID = mileStone.ID;
        //            item.MileStoneName = mileStone.Name;
        //        }
        //        else
        //        {
        //            if (!item.MileStoneID.Contains(mileStone.ID))
        //            {
        //                item.MileStoneID += "," + mileStone.ID;
        //                item.MileStoneName += "," + mileStone.Name;
        //            }
        //        }
        //        //选中的收款项可能没有绑定项目，此时关联里程碑自动绑定项目
        //        item.ProjectInfo = marketProject.ID;
        //        item.ProjectInfoName = marketProject.Name;
        //        SyncMileStoneInfo(item);
        //    }
        //}

        ////收款项根据关联的里程碑同步里程碑信息
        //public void SyncMileStoneInfo(S_C_ManageContract_ReceiptObj receiptObj)
        //{
        //    var mileStoneIDs = receiptObj.MileStoneID;
        //    //关联的所有里程碑已经完成，则默认将收款项变成可收款状态
        //    if (this.entities.Set<S_P_MileStone>().Count(d => mileStoneIDs.Contains(d.ID) && !d.FactFinishDate.HasValue) == 0)
        //        receiptObj.MileStoneState = true.ToString();
        //    else
        //        receiptObj.MileStoneState = false.ToString();
        //    //将关联的所有里程碑的最大时间作为计划、实际完成时间
        //    receiptObj.MileStonePlanEndDate = this.entities.Set<S_P_MileStone>().Where(d => mileStoneIDs.Contains(d.ID)).Max(c => c.PlanFinishDate);
        //    receiptObj.MileStoneFactEndDate = this.entities.Set<S_P_MileStone>().Where(d => mileStoneIDs.Contains(d.ID)).Max(c => c.FactFinishDate);
        //    receiptObj.SyncSupplementary();
        //}
        #endregion

        #region 设置里程碑状态
        public JsonResult ValidateMileStone(string ValidateType, string MileStoneID)
        {
            var mileStone = this.GetEntityByID<S_P_MileStone>(MileStoneID);
            if (mileStone == null) throw new Formula.Exceptions.BusinessException("未能找到指定的里程碑，无法进行操作");
            if (ValidateType == "Execute")
            {
                if (!mileStone.FactFinishDate.HasValue) {
                    throw new Formula.Exceptions.BusinessException("未完成的里程碑，不能撤销完成");
                }
            }
            else
            {
                if (mileStone.FactFinishDate.HasValue)
                {
                    throw new Formula.Exceptions.BusinessException("已完成的里程碑，不能重复设置完成");
                }
            }
            return Json("");
        }

        public JsonResult SetFinish(string MileStoneData)
        {
            var data = JsonHelper.ToObject(MileStoneData);
            var mileStone = this.GetEntityByID<S_P_MileStone>(data.GetValue("ID"));
            if (mileStone == null) throw new Formula.Exceptions.BusinessException("未能找到指定的里程碑，无法进行操作");
            var date = String.IsNullOrEmpty(data.GetValue("FactFinishDate")) ? DateTime.Now : Convert.ToDateTime(data.GetValue("FactFinishDate"));
            this.UpdateEntity<S_P_MileStone>(mileStone, data);
            mileStone.Finish(date);
            this.entities.SaveChanges();
            //同步收款项信息
           // SyncReceiptObj(mileStone);
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }

        public JsonResult SetExecute(string ID)
        {
            var mileStone = this.GetEntityByID<S_P_MileStone>(ID);
            if (mileStone == null) throw new Formula.Exceptions.BusinessException("未能找到指定的里程碑，无法进行操作");
            mileStone.Revert();
            this.entities.SaveChanges();
            //SyncReceiptObj(mileStone);
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }
        #endregion
                
        #region 里程碑变更
        public JsonResult GetListForChange()
        {
            string ProjectInfoID = GetQueryString("ProjectInfoID");
            string IDs = GetQueryString("IDs");
            string sql = @"Select Name,OrlPlanFinishDate,PlanFinishDate as PreviosPlanFinishDate,PlanFinishDate,Weight,State
                ,Code,MileStoneValue,MileStoneType,MajorValue,MileStoneLevel,Necessity
                ,OutMajorValue,PlanStartDate,OrlPlanStartDate,FactStartDate,FactFinishDate
                ,ID as MileStoneID 
                from S_P_MileStone 
                Where ProjectInfoID = '{0}' and ID in ('{1}')";
            sql = string.Format(sql, ProjectInfoID, IDs.Replace(",", "','"));

            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult ChangeMileStone(string ProjectInfoID, string ChangeMileStoneData)
        {
            var data = UpdateList<S_P_MileStoneHistory>(ChangeMileStoneData);
            string mileStoneIDs = string.Empty;
            data.ForEach(c =>
            {
                c.ChangeTime = DateTime.Now;
                var mileStone = GetEntityByID<S_P_MileStone>(c.MileStoneID);
                mileStone.PlanFinishDate = c.PlanFinishDate;
                //SyncReceiptObj(mileStone);
            });

            entities.SaveChanges();
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }

        public JsonResult GetChangeModel(string id)
        {
            var mileStoneID = GetQueryString("MileStoneID");
            S_P_MileStoneHistory entity;
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(mileStoneID))
            {
                var mileStone = GetEntityByID<S_P_MileStone>(mileStoneID);
                if (mileStone == null)
                    throw new Formula.Exceptions.BusinessException("找不到ID为【" + mileStoneID + "】的里程碑");

                entity = new S_P_MileStoneHistory()
                {
                    MileStoneID = mileStoneID,
                    Name = mileStone.Name,
                    Code = mileStone.Code,
                    ProjectInfoID = mileStone.ProjectInfoID,
                    WBSID = mileStone.WBSID,
                    OrlPlanFinishDate = mileStone.OrlPlanFinishDate,
                    PlanFinishDate = mileStone.PlanFinishDate,
                    PreviosPlanFinishDate = mileStone.PlanFinishDate,
                    MileStoneType = mileStone.MileStoneType,
                    Weight = mileStone.Weight,
                };

            }

            else
                entity = this.GetEntityByID<S_P_MileStoneHistory>(id);

            return Json(entity);
        }

        public JsonResult SaveChange()
        {
            var entity = UpdateEntity<S_P_MileStoneHistory>();
            entity.ChangeTime = DateTime.Now;
            var mileStone = GetEntityByID<S_P_MileStone>(entity.MileStoneID);
            if (mileStone == null)
                throw new Formula.Exceptions.BusinessException("找不到ID为【" + entity.MileStoneID + "】的里程碑");
            mileStone.Weight = entity.Weight;
            mileStone.PlanFinishDate = entity.PlanFinishDate;
            if (mileStone.MileStoneType == Project.Logic.MileStoneType.Cooperation.ToString())
            {
                var cooperationPlan = this.entities.Set<S_P_CooperationPlan>().FirstOrDefault(d => d.MileStoneID == mileStone.ID);
                if (cooperationPlan != null)
                {
                    cooperationPlan.PlanFinishDate = mileStone.PlanFinishDate;
                    var wbs = this.GetEntityByID<S_W_WBS>(cooperationPlan.WBSID);
                    if (wbs != null)
                        wbs.PlanEndDate = mileStone.PlanFinishDate;
                }
            }
            entities.SaveChanges();

            //SyncReceiptObj(mileStone);
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult GetChangeList(string MileStoneID)
        {
            var list = entities.Set<S_P_MileStoneHistory>().Where(c => c.MileStoneID == MileStoneID).OrderBy(c => c.ID);
            return Json(list);
        }

        #endregion

        #region 里程碑完成申请
        public override JsonResult GetModel(string id)
        {
            var entity = this.GetEntityByID(id);
            var dic = new Dictionary<string, object>();
            if (entity != null)
            {
                if (String.IsNullOrEmpty(entity.ApplyApprovalUserID))
                {
                    entity.ApplyApprovalUserID = this.CurrentUserInfo.UserID;
                    entity.ApplyApprovalUserName = this.CurrentUserInfo.UserName;
                    entity.ApplyApprovalDate = DateTime.Now;
                }
                dic = FormulaHelper.ModelToDic<S_P_MileStone>(entity);  
                dic.SetValue("ProjectInfoName", entity.S_I_ProjectInfo.Name);

            }
            else
            {
                entity = this.UpdateEntity<S_P_MileStone>(id);
                dic = FormulaHelper.ModelToDic<S_P_MileStone>(entity); 
                dic.SetValue("ApplyApprovalUserID", this.CurrentUserInfo.UserID);
                dic.SetValue("ApplyApprovalUserName", this.CurrentUserInfo.UserName);
                dic.SetValue("ApplyApprovalDate", DateTime.Now);
            }
            return Json(dic);
        }

        public ActionResult MileStoneFinishApplyTab()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑策划");
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.MileStoneLevel = level;
            ViewBag.ProjectInfoID = projectInfoID;
            return View();
        }

        public ActionResult MileStoneFinishApplyList()
        {
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.MileStoneLevel = level;
            ViewBag.FinishState = ProjectCommoneState.Finish.ToString();
            ViewBag.ExecuteState = ProjectCommoneState.Execute.ToString();
            return View();
        }

        public JsonResult GetApplyModel(string ids)
        {
            string strIDs = GetQueryString("IDs");
            string[] arrIDs = strIDs.Split(',');
            var entity = GetEntityByID<S_P_MileStone>(arrIDs[0]);
            if (entity == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + arrIDs[0] + "】的里程碑对象，无法完成里程碑");
            return Json(entity);
        }

        /// <summary>
        /// 申请完成
        /// </summary>
        /// <param name="MileStoneData"></param>
        /// <returns></returns>
        public JsonResult FinishApply()
        {
            string strIDs = GetQueryString("IDs");
            string[] arrIDs = strIDs.Split(',');

            for (int i = 0; i < arrIDs.Length; i++)
            {
                var entity = GetEntityByID<S_P_MileStone>(arrIDs[i]);
                if (entity.ApplyState == "已完成")
                {
                    throw new Formula.Exceptions.BusinessException(entity.Name + "已完成，不能再次申请!");
                }
            }
            for (int i = 0; i < arrIDs.Length; i++)
            {
                var entity = UpdateEntity<S_P_MileStone>(arrIDs[i]);
                if (entity == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + arrIDs[i] + "】的里程碑对象，无法完成里程碑");
                entity.ApplyState = "已申请";
                this.entities.SaveChanges();
                //SyncReceiptObj(entity);
            }
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }

        /// <summary>
        /// 专业里程碑完成申报
        /// </summary>
        /// <returns></returns>
        public ActionResult MajorMileStoneFinishApplyList()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑填报");
            var majors = projectInfo.GetMajors();
            if (majors.Count == 0) throw new Formula.Exceptions.BusinessException("未进行专业策划的项目，无法填报专业里程碑");
            ViewBag.ProjectInfoID = projectInfoID;
            ViewBag.DefaultMajorCode = majors.FirstOrDefault().GetValue("Value");
            ViewBag.Majors = majors;
            ViewBag.RootNode = JsonHelper.ToJson(projectInfo.WBSRoot);
            ViewBag.FinishState = ProjectCommoneState.Finish.ToString();
            ViewBag.ExecuteState = ProjectCommoneState.Execute.ToString();
            return View();
        }

        /// <summary>
        /// 确认完成里程碑
        /// </summary>
        /// <returns></returns>
        public JsonResult FinishMileStone()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法确认完成里程碑。");
            string IDs = this.Request["IDs"];
            string[] arrIDs = IDs.Split(',');

            for (int i = 0; i < arrIDs.Length; i++)
            {
                var entity = GetEntityByID<S_P_MileStone>(arrIDs[i]);
                entity.State = "Finish";
                entity.FactFinishDate = DateTime.Now;
                //SyncReceiptObj(entity);
            }
            this.entities.SaveChanges();
            //FormulaHelper.GetEntities<MarketEntities>().SaveChanges();
            return Json("");
        }
        #endregion
    }
}
