using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula.Helper;
using Config;

namespace Base.Areas.PortalBlock.Controllers
{
    public class FeedbackController : BaseController<S_H_Feedback>
    {
        public override JsonResult GetModel(string id)
        {
            var model = base.GetEntity<S_H_Feedback>(id);
            if (string.IsNullOrEmpty(model.Level))
                model.Level = "一般";

            var url = Request["url"];
            if (string.IsNullOrEmpty(model.Module) && !string.IsNullOrEmpty(url))
            {
                url = url.Substring("http://".Length);
                url = url.Substring(url.IndexOf('/'));

                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                string sql = string.Format("select FullID from S_A_Res where url<>'' and Url is not null and '{0}' like Url+'%'", url);
                var obj = baseSqlHelper.ExecuteScalar(sql);
                if (obj != null)
                {
                    string fullID = obj.ToString();
                    if (fullID.Length > 72)
                    {
                        string resID = fullID.Split('.')[1];
                        sql = string.Format("select Name from S_A_Res where ID='{0}'", resID);
                        model.Module = baseSqlHelper.ExecuteScalar(sql).ToString();
                    }
                }
            }
            return Json(model);
        }

        public JsonResult GetMyFeedbackList(MvcAdapter.QueryBuilder qb)
        {
            var data = entities.Set<S_H_Feedback>().Where(p => p.CreateUserID == Formula.FormulaHelper.UserID).WhereToGridData(qb);
            return Json(data);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var entity = base.UpdateEntity<S_H_Feedback>();

            if (string.IsNullOrEmpty(entity.ProjectName))
                entity.ProjectName = System.Configuration.ConfigurationManager.AppSettings["FeedbackProjectName"];
            if (string.IsNullOrEmpty(entity.ProjectDept))
                entity.ProjectDept = System.Configuration.ConfigurationManager.AppSettings["FeedbackProjectDept"];
            if (string.IsNullOrEmpty(entity.ProjectPrincipal))
                entity.ProjectPrincipal = System.Configuration.ConfigurationManager.AppSettings["FeedbackProjectPrincipal"];

            string img = entity.Attachment;
            if (img.Contains("<img"))
            {
                img = img.Substring(img.IndexOf("<img")) + " ";
                img = img.Remove(img.IndexOf(">") + 1);
            }
            entity.Attachment = img;
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult SavePatch()
        {
            var ids = Request["IDs"];
            var dic = UpdateEntity<S_H_Feedback>();
            entities.Set<S_H_Feedback>().Remove(dic);

            var etys = entities.Set<S_H_Feedback>().Where(c => ids.Contains(c.ID));
            foreach (var id in ids.Split(','))
            {
                var ety = etys.SingleOrDefault(c => c.ID == id);
                if (ety == null) continue;

                if (!string.IsNullOrEmpty(dic.ProjectPrincipal))
                    ety.ProjectPrincipal = dic.ProjectPrincipal;
                if (!string.IsNullOrEmpty(dic.ProjectDept))
                    ety.ProjectDept = dic.ProjectDept;
                if (!string.IsNullOrEmpty(dic.DealUserName))
                    ety.DealUserName = dic.DealUserName;
                if (!string.IsNullOrEmpty(dic.DealStatus))
                    ety.DealStatus = dic.DealStatus;
                if (!string.IsNullOrEmpty(dic.DealResult))
                    ety.DealResult = dic.DealResult;
                if (!string.IsNullOrEmpty(dic.ProblemNature))
                    ety.ProblemNature = dic.ProblemNature;
                if (!string.IsNullOrEmpty(dic.Module))
                    ety.Module = dic.Module;
                if (dic.ExpectedCompleteTime != null)
                    ety.ExpectedCompleteTime = dic.ExpectedCompleteTime;
                if (dic.ConfirmProblemsTime != null)
                    ety.ConfirmProblemsTime = dic.ConfirmProblemsTime;
                if (dic.PlanCompleteTime != null)
                    ety.PlanCompleteTime = dic.PlanCompleteTime;
                if (dic.ActualCompleteTime != null)
                    ety.ActualCompleteTime = dic.ActualCompleteTime;
                if (dic.ConfirmCompleteTime != null)
                    ety.ConfirmCompleteTime = dic.ConfirmCompleteTime;
                if (!string.IsNullOrEmpty(dic.ConfirmProblemsUserID))
                    ety.ConfirmProblemsUserID = dic.ConfirmProblemsUserID;
                if (!string.IsNullOrEmpty(dic.ConfirmProblemsUserName))
                    ety.ConfirmProblemsUserName = dic.ConfirmProblemsUserName;
                if (!string.IsNullOrEmpty(dic.ConfirmCompleteUserID))
                    ety.ConfirmCompleteUserID = dic.ConfirmCompleteUserID;
                if (!string.IsNullOrEmpty(dic.ConfirmCompleteUserName))
                    ety.ConfirmCompleteUserName = dic.ConfirmCompleteUserName;
            }

            entities.SaveChanges();
            return Json("");
        }

        public void PatchConfirmComplete(string listIDs)
        {
            var etys = entities.Set<S_H_Feedback>().Where(c => listIDs.Contains(c.ID));
            foreach (var item in etys)
            {
                item.ConfirmCompleteTime = DateTime.Now;
                item.ConfirmCompleteUserID = item.CreateUserID;
                item.ConfirmCompleteUserName = item.CreateUserName;
                item.DealStatus = "确认完成";
            }
            entities.SaveChanges();
        }


    }
}
