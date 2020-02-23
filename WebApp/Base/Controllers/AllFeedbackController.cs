using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Base.Logic.Domain;
using Formula;
using System.Web.Http;
using System.Web;

namespace dcwj.Controllers
{
    public class AllFeedbackController : ApiController
    {
        // POST api/<controller>   
        [ActionName("Save")]
        [HttpPost]
        public void Post([FromBody]S_H_AllFeedback feedback)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            var entity = entities.Set<S_H_AllFeedback>().SingleOrDefault(c => c.ID == feedback.ID);

            if (entity == null)
            {
                feedback.CreateTime = DateTime.Now;
                entities.Set<S_H_AllFeedback>().Add(feedback);
            }
            else
            {
                entity.ProjectPrincipal = feedback.ProjectPrincipal;
                entity.ProjectDept = feedback.ProjectDept;
                entity.DealUserName = feedback.DealUserName;
                entity.DealStatus = feedback.DealStatus;
                entity.DealResult = feedback.DealResult;
                entity.ExpectedCompleteTime = feedback.ExpectedCompleteTime;
                entity.Module = feedback.Module;
                entity.ProblemNature = feedback.ProblemNature;
                entity.ConfirmProblemsTime = feedback.ConfirmProblemsTime;
                entity.PlanCompleteTime = feedback.PlanCompleteTime;
                entity.ActualCompleteTime = feedback.ActualCompleteTime;
                entity.ConfirmCompleteTime = feedback.ConfirmCompleteTime;
                entity.ConfirmProblemsUserID = feedback.ConfirmProblemsUserID;
                entity.ConfirmProblemsUserName = feedback.ConfirmProblemsUserName;
                entity.ConfirmCompleteUserID = feedback.ConfirmCompleteUserID;
                entity.ConfirmCompleteUserName = feedback.ConfirmCompleteUserName;
            }

            entities.SaveChanges();
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete(string id)
        {
            SQLHelper _sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("delete from S_H_AllFeedback where ID in('{0}')", id.Replace(",", "','"));
            _sqlHelper.ExecuteNonQuery(sql);
        }

        [ActionName("PatchSave")]
        [HttpPost]
        public void _patchSave(string id, [FromBody]S_H_AllFeedback dic)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            var etys = entities.Set<S_H_AllFeedback>().Where(c => id.Contains(c.ID));
            foreach (var ID in id.Split(','))
            {
                var ety = etys.SingleOrDefault(c => c.ID == ID);
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
        }

        [ActionName("PatchConfirmComplete")]
        [HttpPost]
        public void _patchConfirmComplete(string id)
        {           
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            var etys = entities.Set<S_H_AllFeedback>().Where(c => id.Contains(c.ID));
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

