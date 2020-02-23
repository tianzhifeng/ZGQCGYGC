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

namespace Project.Areas.Basic.Controllers
{
    public class SpaceController : ProjectController
    {
        public ActionResult Index()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectFO = Formula.FormulaHelper.CreateFO<Project.Logic.ProjectInfoFO>();
            if (String.IsNullOrEmpty(projectInfoID))
                projectInfoID = projectFO.GetDefaultProjectID(this.CurrentUserInfo.UserID);
            this.ViewBag.CurrentUserID = this.CurrentUserInfo.UserID;
            this.ViewBag.DefaultCode = "";
            this.ViewBag.DefaultSpaceDefineID = "";
            this.ViewBag.ShowSpace = "False";
            this.ViewBag.DefaultUrl = "";
            if (String.IsNullOrEmpty(projectInfoID))
            {
                var entraceList = new List<Entrance>();
                this.ViewBag.Entrace = entraceList;
                this.ViewBag.DisplayCount = 0;
                this.ViewBag.ProjectInfoID = projectInfoID;
                this.ViewBag.ProjectJson = "";
                this.ViewBag.NeedMenu = false;
                this.ViewBag.ShowSpace = false.ToString();
                this.ViewBag.ProjectJson = "{}";
            }
            // if (String.IsNullOrEmpty(projectInfoID)) throw new Formula.Exceptions.BusinessException("您没有参与设计项目，无法进入项目空间");
            else
            {
                
                string userID = this.ViewBag.CurrentUserID;
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                var entraceList = projectFO.GetEntrance(userID, projectInfoID);
                bool needMenu = entraceList.Count > 8 ? true : false;
                int displayCount = entraceList.Count > 8 ? 8 : entraceList.Count;
                if (entraceList.Count > 0)
                    this.ViewBag.ShowSpace = "True";
                foreach (var item in entraceList)
                {
                    if (item.IsDefault)
                    {
                        this.ViewBag.DefaultItem = item;
                    }
                }
                this.ViewBag.DisplayCount = displayCount;
                this.ViewBag.ProjectInfoID = projectInfo.ID;
                this.ViewBag.Entrace = entraceList;
                this.ViewBag.NeedMenu = needMenu;
                this.ViewBag.ProjectJson = JsonHelper.ToJson(projectInfo);               
            }
            var userProjectTable = projectFO.GetUserProjectList(this.CurrentUserInfo.UserID);
            this.ViewBag.RelationProject = JsonHelper.ToJson(userProjectTable);
            return View();
        }


        public JsonResult GetSpaceDefine(string DefineID, string SpaceCode, string ProjectInfoID)
        {
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            var result = projectFO.GetSpaceDefine(this.CurrentUserInfo.UserID, ProjectInfoID, DefineID, SpaceCode);
            return Json(result);
        }

        public JsonResult GetMyProjectInfo(QueryBuilder qb)
        {

            string sql = @"select distinct S_I_ProjectInfo.* from dbo.S_I_ProjectInfo
            left join S_I_UserFocusProjectInfo on ProjectInfoID = S_I_ProjectInfo.ID where UserID='" + this.CurrentUserInfo.UserID + "' ";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);

        }

        /// <summary>
        ///获取有权限的项目信息
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public JsonResult GetAllProjectInfo(QueryBuilder qb)
        {
            string sql = @"SELECT ID,Name,Code,[State],PhaseValue,ChargeDeptID,ChargeDeptName,
OtherDeptID,OtherDeptName,ChargeUserID,ChargeUserName,CreateDate,
[UserIDs]=STUFF((SELECT ','+UserID FROM V_I_ProjectUserInfo A WHERE A.ID=V_I_ProjectUserInfo.ID FOR XML PATH('')), 1, 1, '')
FROM V_I_ProjectUserInfo with(nolock)
GROUP BY ID,Name,Code,[State],PhaseValue,ChargeDeptID,ChargeDeptName,
OtherDeptID,OtherDeptName,ChargeUserID,ChargeUserName,CreateDate";
            //where UserID='" + this.CurrentUserInfo.UserID + "' 
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult SetFocusProject()
        {
            var projectInfoID = GetQueryString("ProjectInfoID");
            entities.Set<S_I_UserFocusProjectInfo>().Add(
                new S_I_UserFocusProjectInfo()
                {
                    ID = FormulaHelper.CreateGuid(),
                    ProjectInfoID = projectInfoID,
                    UserID = FormulaHelper.GetUserInfo().UserID,
                });
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult CancelFocusProject(string ProjectInfoID)
        {
            string sql = @"delete from S_I_UserFocusProjectInfo where id = (select top 1 id from S_I_UserFocusProjectInfo with(nolock) where UserID='{0}' and ProjectInfoID = '{1}')";
            this.SqlHelper.ExecuteNonQuery(string.Format(sql,this.CurrentUserInfo.UserID,ProjectInfoID));
            return Json("");
        }

    }

}
