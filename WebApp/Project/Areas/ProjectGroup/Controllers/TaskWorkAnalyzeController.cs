using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using Project.Logic;
using Config;
using Config.Logic;
using Formula;
using MvcAdapter;
using System.Collections.Specialized;
using Formula.Helper;
using System.Data;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class TaskWorkAnalyzeController : ProjectController<S_W_TaskWork>
    {
        public ActionResult TaskWorkAnalyze()
        {
            var tab = new Tab();

            var category = CategoryFactory.GetYearCategory("Year");
            category.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(category);

            category = CategoryFactory.GetMonthCategory("Month");
            category.SetDefaultItem(DateTime.Now.Month.ToString());
            tab.Categories.Add(category);

            category = CategoryFactory.GetQuarterCategory("Quarter");
            category.SetDefaultItem();
            tab.Categories.Add(category);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetPersonList(QueryBuilder qb)
        {
            var sql = @"
select u.ID,u.Name,u.Code,u.WorkNo,u.DeptID,u.DeptName,u.DeptFullID,
(select COUNT(*) from S_W_TaskWork where DesignerUserID = u.ID {1} {2}) Designer,
(select COUNT(*) from S_W_TaskWork where CollactorUserID = u.ID {1} {2}) Collactor,
(select COUNT(*) from S_W_TaskWork where AuditorUserID = u.ID {1} {2}) Auditor,
(select COUNT(*) from S_W_TaskWork where ApproverUserID = u.ID {1} {2}) Approver,
(select COUNT(*) from S_W_TaskWork where MapperUserID = u.ID {1} {2}) Mapper,
(select COUNT(*) from S_W_TaskWork where (DesignerUserID = u.ID or CollactorUserID = u.ID
or AuditorUserID = u.ID or ApproverUserID = u.ID or MapperUserID = u.ID){1} {2}) Summary
from {0}..S_A_User u";

            var misQb = new QueryBuilder();
            var subWhereStr = "";
            foreach (var condition in qb.Items.Where(a => a.Field == "FactEndDate" || a.Field == "Year" || a.Field == "Month" || a.Field == "Quarter"))
            {
                if (condition.Field == "FactEndDate")
                    misQb.Add("FactEndDate", condition.Method, condition.Value, condition.OrGroup, condition.BaseOrGroup);
                else
                    subWhereStr += "and datepart(" + condition.Field + ",CreateDate) in (" + condition.Value + ")";
            }
            qb.Items.RemoveWhere(a => a.Field == "FactEndDate");
            qb.Items.RemoveWhere(a => a.Field == "Year");
            qb.Items.RemoveWhere(a => a.Field == "Month");
            qb.Items.RemoveWhere(a => a.Field == "Quarter");
            string whereStr = misQb.GetWhereString(false);

            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sql = String.Format(sql, baseDB.DbName, whereStr, subWhereStr);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }
    }
}
