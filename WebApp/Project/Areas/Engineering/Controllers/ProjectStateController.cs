using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Helper;
using Project.Logic.Domain;
using Project.Logic;
using Formula;
using Config.Logic;
using Config;
using MvcAdapter;

namespace Project.Areas.Engineering.Controllers
{
    public class ProjectStateController : ProjectController<S_I_ProjectInfo>
    {
        public override ActionResult List()
        {
            var tab = new Tab();
            var stateCategory = CategoryFactory.GetCategory(typeof(ProjectCommoneState), "State");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;
            tab.Categories.Add(stateCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override  JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var groupInfoID = Request["GroupInfoID"];
            var group = this.GetEntityByID<S_I_ProjectGroup>(groupInfoID);
            if (group == null)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            }
            string sql = @"select * from S_I_ProjectInfo where GroupRootID='{0}'";
            sql = string.Format(sql, groupInfoID);
            SQLHelper shProject = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            GridData data = shProject.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult Pause(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
           // var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.GetEntityByID(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经完工，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Pause.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经暂停，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Terminate.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经终止，无法进行操作");
                projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Pause.ToString();
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

               // var marketProjectInfo = marketEntities.Set<Market.Logic.Domain.S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
               // if (marketProjectInfo != null)
                 //   marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Terminate(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.GetEntityByID(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经完工，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Terminate.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经终止，无法进行操作");
                if (projectInfo.State != ProjectCommoneState.Pause.ToString())
                    projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Terminate.ToString();
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Restart(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.GetEntityByID(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State != ProjectCommoneState.Pause.ToString() &&
                 projectInfo.State != ProjectCommoneState.Terminate.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】不是暂停或终止状态，无法进行操作");
                projectInfo.State = projectInfo.Status;
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Finish(string ListData, string factFinishDate)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作！");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已完工，无法进行操作！");
                projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Finish.ToString();
                projectInfo.FactFinishDate = DateTime.Parse(factFinishDate);
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
