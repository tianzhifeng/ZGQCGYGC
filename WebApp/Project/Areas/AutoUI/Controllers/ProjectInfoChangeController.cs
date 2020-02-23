using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;

namespace Project.Areas.AutoUI.Controllers
{
    public class ProjectInfoChangeController : ProjectFormContorllor<T_CP_ProjectInfoChange>
    {
        protected override void OnFlowEnd(T_CP_ProjectInfoChange entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息");
                projectInfo.WBSRoot.SetUsers(ProjectRole.ProjectManager.ToString(), entity.ProjectManager.Split(','));
                projectInfo.ChargeUserID = entity.ProjectManager;
                projectInfo.ChargeUserName = entity.ProjectManagerName;
                var projectGroup = this.GetEntityByID<S_I_ProjectGroup>(projectInfo.GroupID);
                if (projectGroup == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息");
                projectGroup.ChargeUser = entity.ProjectManager;
                projectGroup.ChargeUserName = entity.ProjectManagerName;

                projectInfo.ModifyDate = DateTime.Now;
            }
            this.BusinessEntities.SaveChanges();
        }
    }
}
