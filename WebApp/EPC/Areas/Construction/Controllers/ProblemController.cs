using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using Workflow.Logic.Domain;
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class ProblemController : EPCFormContorllor<S_C_RectifySheet_RectifyProblems>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                dic.Remove("RectifyDate");
                dic.Remove("CloseDate");
            }
            else
            {
                string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", dic.GetValue("ID"));
                var problem = EPCSQLDB.ExecuteDataTable(sql); 
                if (problem.Rows.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");

                var currentState = problem.Rows[0]["RectifyState"] == null ? string.Empty: problem.Rows[0]["RectifyState"].ToString();
                if (string.IsNullOrEmpty(currentState))
                    throw new Formula.Exceptions.BusinessValidationException("此单的状态为空，请确认！");

                string actionType = GetQueryString("actionType");
                switch (actionType)
                {
                    case "Rectify"://整改
                        if (currentState != "Register")
                            throw new Formula.Exceptions.BusinessValidationException("此单已处理，不能重复处理，请确认！");

                        dic.SetValue("RectifyState", "Rectify");//状态-->“已整改”
                        if (string.IsNullOrEmpty(dic.GetValue("RectifyDate")))
                            throw new Formula.Exceptions.BusinessValidationException("请选择【整改时间】！");
                        break;

                    case "Close"://关闭
                        if (currentState != "Rectify")
                            throw new Formula.Exceptions.BusinessValidationException("此单已关闭，不能重复关闭，请确认！");

                        dic.SetValue("RectifyState", "Closed");//状态-- >“已关闭”
                        if (string.IsNullOrEmpty(dic.GetValue("CloseDate")))
                            throw new Formula.Exceptions.BusinessValidationException("请选择【关闭时间】！");
                        break;

                    case "Reject"://重新整改
                        if (currentState != "Rectify")
                            throw new Formula.Exceptions.BusinessValidationException("此单已关闭，不能重复关闭，请确认！");

                        dic.SetValue("RectifyState", "Register");//状态-- >“待整改”
                        dic.SetValue("RectifyDate", string.Empty);//整改时间
                        break;
                }


            }
        }

    }
}
