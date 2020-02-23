using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EPC.Logic.Domain;
using MvcAdapter;
using Formula.Helper;
using Config.Logic;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class TaskTemplateController : InfrastructureController<S_T_TaskTemplate>
    {
        public JsonResult GetTaskDetailList(QueryBuilder qb, string TemplateID)
        {
            qb.PageSize = 0;
            var data = this.entities.Set<S_T_TaskTemplate_Detail>().Where(c => c.TemplateID == TemplateID).WhereToGridData(qb);
            return Json(data);
        }

        public override JsonResult SaveList()
        {
            var listData = Request["ListData"];
            UpdateList<S_T_TaskTemplate_Detail>(listData);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteDetails(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_T_TaskTemplate_Detail>(item.GetValue("ID"));
                if (detail == null) continue;
                this.entities.Set<S_T_TaskTemplate_Detail>().Remove(detail);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult CopyTemplate(string TemplateID)
        {
            var template = this.entities.Set<S_T_TaskTemplate>().Find(TemplateID);
            if (template == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的作业模板");
            var newTempalte = template.Clone<S_T_TaskTemplate>();
            newTempalte.ID = Formula.FormulaHelper.CreateGuid();
            newTempalte.Name =template.Name+ "（副本）";
            newTempalte.CreateDate = DateTime.Now;
            newTempalte.CreateUser = this.CurrentUserInfo.UserName;
            newTempalte.CreateUserID = this.CurrentUserInfo.UserID;
            newTempalte.ModifyUser = this.CurrentUserInfo.UserName;
            newTempalte.ModifyUserID = this.CurrentUserInfo.UserID;
            newTempalte.ModifyDate = DateTime.Now;
            foreach (var item in template.S_T_TaskTemplate_Detail.ToList())
            {
                var detail = item.Clone<S_T_TaskTemplate_Detail>();
                newTempalte.S_T_TaskTemplate_Detail.Add(detail);
            }
            this.entities.Set<S_T_TaskTemplate>().Add(newTempalte);
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
