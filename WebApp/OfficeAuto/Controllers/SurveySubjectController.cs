using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula;
using System.Reflection;
using Formula.DynConditionObject;
using Config;
using System.Data;
using OfficeAuto.Logic.Domain;
using Formula.Exceptions;
using Base.Logic.Domain;

namespace OfficeAuto.Controllers
{
    public class SurveySubjectController : BaseController<S_Survey_Subject>
    {
        protected override void EntityCreateLogic<TEntity>(TEntity entity)
        {
            base.EntityCreateLogic(entity);
            if (entity is S_Survey_Subject)
            {
                var obj = entity as S_Survey_Subject;
                obj.IssueID = obj.CreateUserID;
                obj.IssueName = obj.CreateUserName;
                obj.IssueDate = obj.CreateDate;
            }
        }

        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_Survey_Subject>();
            if (string.IsNullOrEmpty(entity.QueryUserIDs) && string.IsNullOrEmpty(entity.QueryDeptIDs))
                throw new Formula.Exceptions.BusinessException("请选择投票人员或者投票部门！");
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        SQLHelper sh = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
        public override JsonResult Delete()
        {
            string id = Request["ListIDs"];
            object obj = entities.Set<S_Survey_Subject>().Where(c => c.State == "T" && c.ID == id).FirstOrDefault();
            if (obj != null)
                throw new BusinessException("投票已启动，无法删除");

            string sql = "delete from S_Survey_Subject where ID='" + id + "'";//删主题表
            sh.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SetState(string id)
        {
            //查找是否已维护问题信息
            var quality = entities.Set<S_Survey_Question>().Where(c => c.SurveyID == id);
            if (quality.Count() == 0)
                throw new Formula.Exceptions.BusinessException("请先维护问题再启动投票！");
            //校验问题的是否已维护枚举
            var list = quality.Where(t => t.QuestionType == "单选" || t.QuestionType == "多选").ToList();
            foreach (var item in list)
            {
                var OptionList = item.S_Survey_Option.ToList();
                if (OptionList == null || OptionList.Count == 0)
                    throw new Formula.Exceptions.BusinessException("请先维护问题标题为【" + item.QuestionName + "】的选项再启动投票！");
            }
            var subject = this.GetEntity<S_Survey_Subject>(id);
            subject.State = "T";
            this.entities.SaveChanges();

            string link = "/OfficeAuto/Survey/SurveyList?ID=" + id + "&FuncType=vote";
            string content = "您有新的问卷调查，请尽快对【" + subject.SurveyName + "】问卷进行投票！";

            if (!string.IsNullOrEmpty(subject.QueryUserIDs))
            {
                FormulaHelper.GetService<IMessageService>().SendMsg("问卷调查:" + subject.SurveyName, content, link,
                    string.Empty, subject.QueryUserIDs, subject.QueryUserNames);
            }
            if (!string.IsNullOrEmpty(subject.QueryDeptIDs))
            {
                FormulaHelper.GetService<IMessageService>().SendMsg("问卷调查:" + subject.SurveyName, content, link,
                    string.Empty, subject.QueryDeptIDs, subject.QueryDeptNames, MsgReceiverType.PropertyType);
            }

            return Json("");
        }
    }
}
