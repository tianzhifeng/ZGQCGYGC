using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config.Logic;
using Base.Logic.Domain;
using Formula;
namespace HR.Areas.AutoForm.Controllers
{
    public class Monthlyreport_MonthlyreportfillController : HRFormContorllor<T_Monthlyreport_Monthlyreportfill>
    {
        //
        // GET: /AutoForm/Monthlyreport_Monthlyreportfill/

        public ActionResult Index()
        {
            return View();
        }
        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var row = dt.Rows[0];
                var Filldate = Convert.ToDateTime(row["Filldate"]);
                if (Filldate.Day > 6)
                    row["Overdue"] = "1";
                else
                    row["Overdue"] = "0";

            }
        }

        protected override void OnFlowEnd(T_Monthlyreport_Monthlyreportfill entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var list = entity.T_Monthlyreport_Monthlyreportfill_Advice.ToList();
            foreach (var item in list)
            {
                var S_E_MonthAdvice = new S_E_MonthAdvice();
                S_E_MonthAdvice.ID = FormulaHelper.CreateGuid();
                S_E_MonthAdvice.Advice = item.Advice;
                S_E_MonthAdvice.BelongMonth = Convert.ToDateTime(entity.Filldate).Month;
                S_E_MonthAdvice.BelongYear = Convert.ToDateTime(entity.Filldate).Year;
                S_E_MonthAdvice.ContentOutline = item.Content;
                S_E_MonthAdvice.Dept = item.Dept;
                S_E_MonthAdvice.DeptName = item.DeptName;
                S_E_MonthAdvice.Filldate = entity.Filldate;
                S_E_MonthAdvice.T_Monthlyreport_MonthlyreportfillID = item.T_Monthlyreport_MonthlyreportfillID;
                S_E_MonthAdvice.FillUserID = entity.Fillname;
                S_E_MonthAdvice.FillUserName = entity.FillnameName;
                BusinessEntities.Set<S_E_MonthAdvice>().Add(S_E_MonthAdvice);

            }
            BusinessEntities.SaveChanges();
        }
    }
}
