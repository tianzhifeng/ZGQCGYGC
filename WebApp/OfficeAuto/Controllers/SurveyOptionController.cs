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
using Formula.Helper;

namespace OfficeAuto.Controllers
{
    public class SurveyOptionController : BaseController<S_Survey_Option>
    {
        SQLHelper sh = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
        public override JsonResult Save()
        {
            string pid=GetQueryString("QuestionID");
            string fodata = Request.Form["FormData"];
            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(fodata);
            string Sub = formDic["Sub"].ToString();
            var list = UpdateList<S_Survey_Option>(Sub);
            foreach (S_Survey_Option invo in list)
            {
                invo.QuestionID = pid;
            }

            entities.SaveChanges();     //保存操作
            return Json(new { ID = "" });
        }

        public JsonResult GetOptionList(QueryBuilder qb)
        {  
            string pid = GetQueryString("QuestionID");
            string strSql = "Select * From S_Survey_Option Where QuestionID='" + pid + "'";
            GridData data = sh.ExecuteGridData(strSql, qb);
            return Json(data);
        }

        public JsonResult DeleteOption(String ID)
        {
            string sql1 = "delete from S_Survey_Option where ID='" + ID + "'";
            string sql2 = "delete from S_Survey_Vote where OptionID='" + ID + "'";
            sh.ExecuteNonQuery(sql2);
            sh.ExecuteNonQuery(sql1);

            return Json("");
        }
    }
}
