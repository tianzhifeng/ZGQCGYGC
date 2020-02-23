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

namespace OfficeAuto.Controllers
{
    public class SurveyQuestionController : BaseController<S_Survey_Question>
    {
        SQLHelper sh = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
        public override JsonResult Save()
        {
            
            S_Survey_Question entity = UpdateEntity<S_Survey_Question>();           //根据前台传递过来的值给实体类赋值
            entity.SurveyID = GetQueryString("SurveyID");
            entities.SaveChanges();     //保存操作
            PropertyInfo pi = typeof(S_Survey_Question).GetProperty("ID");
            if (pi != null)
                return Json(new { ID = pi.GetValue(entity, null) });
            return Json(new { ID = "" });
        }

        public override JsonResult Delete()
        {
            string id = Request["ListIDs"];
            string sql1 = "delete from S_Survey_Vote where OptionID in(select ID from S_Survey_Option where QuestionID='" + id + "')";//删投票表
            string sql2 = "delete from S_Survey_Option where QuestionID='" + id + "'";//删选项表
            string sql3 = "delete from dbo.S_Survey_QuestionComment where QuestionID='" + id + "'";//删问题评论表
            string sql4 = "delete from S_Survey_Question where ID='" + id + "'";//删问题表

            sh.ExecuteNonQuery(sql1);
            sh.ExecuteNonQuery(sql2);
            sh.ExecuteNonQuery(sql3);
            sh.ExecuteNonQuery(sql4);

            return Json("");
        }
    }
}
