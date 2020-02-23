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
    public class ProjectStateController : ProjectController<S_I_ProjectInfo>
    {
        public override ActionResult List()
        {
            ViewBag.ColumnEnums = new Dictionary<string, string>();
            var dt = BaseConfigFO.GetWBSEnum(WBSNodeType.Phase.ToString());

            var tab = new Tab();
            var stateCategory = CategoryFactory.GetCategory(typeof(ProjectCommoneState), "State");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;
            tab.Categories.Add(stateCategory);

            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "部门", "ChargeDeptID");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult Pause(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
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

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
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

                //var marketProjectInfo = marketEntities.Set<Market.Logic.Domain.S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
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
