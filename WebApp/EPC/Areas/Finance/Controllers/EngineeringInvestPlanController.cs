using Config;
using Config.Logic;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Finance.Controllers
{
    public class EngineeringInvestPlanController : EPCFormContorllor<S_F_EngineeringInvestPlan>
    {
        private string prePlanMonths = "colMonth_";

        public ActionResult List()
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(Request["EngineeringInfoID"]);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("工程信息不存在！");

            var dateNow = DateTime.Now;

            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("MakeYear", 5, 2, false, "年份");
            yearCategory.SetDefaultItem(dateNow.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("MakeMonth", false, "月份");
            monthCategory.SetDefaultItem(dateNow.Month.ToString());
            monthCategory.Multi = false;
            tab.Categories.Add(monthCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            var planMonths = new List<Dictionary<string, object>>();
            var planDate = dateNow;
            for (int i = 1; i <=6; i++)
            {
                planDate = dateNow.AddMonths(i);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("ColumnName", string.Format("{0}{1}_{2}", prePlanMonths, planDate.Year, planDate.Month) );
                dic.SetValue("Year", planDate.Year);
                dic.SetValue("Month", planDate.Month);
                planMonths.Add(dic);
            }

            ViewBag.planMonths = JsonHelper.ToJson(planMonths);
            ViewBag.prePlanMonths = prePlanMonths;


            return View();
        }
        public JsonResult GetList(QueryBuilder qb)
        {
            var engineeringInfo = GetQueryString("EngineeringInfoID");
            var makeYear = Convert.ToInt32(qb.Items.Find(q => q.Field == "MakeYear").Value);
            var makeMonth = Convert.ToInt32(qb.Items.Find(q => q.Field == "MakeMonth").Value);
            var planMonths = JsonHelper.ToList(Request["planMonths"]);

            var sql = string.Format(@"select * from S_F_EngineeringInvestPlan where EngineeringInfo='{0}' and MakeYear={1} and MakeMonth={2} ",
                engineeringInfo, makeYear, makeMonth);
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            var InvestPlanList = FormulaHelper.DataTableToListDic(dt);

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var defaultRows = new string[] { "Receive", "Payment" };
            foreach (var defaultRow in defaultRows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("FundType", defaultRow);

                //月份列
                foreach (var planMonth in planMonths)
                {
                    if (InvestPlanList.Count == 0)
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), 0m);
                        continue;
                    }
                    var InvestPlan = InvestPlanList.Find(p => p.GetValue("FundType") == defaultRow && p.GetValue("PlanYear") == planMonth.GetValue("Year") 
                    && p.GetValue("PlanMonth") == planMonth.GetValue("Month"));
                    if (InvestPlan == null)
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), 0m);
                    }
                    else
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), InvestPlan.GetValue("Amount"));
                    }
                }
                result.Add(dic);
            }

            return Json(result);

        }

        public JsonResult SaveList()
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(Request["EngineeringInfoID"]);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("工程信息不存在！");

            var list = JsonHelper.ToList(Request["List"]);
            var makeYear = Convert.ToInt32(Request["MakeYear"]);
            var makeMonth = Convert.ToInt32(Request["MakeMonth"]);
            var planMonths = JsonHelper.ToList(Request["planMonths"]);

            var userInfo = FormulaHelper.GetUserInfo();
            var sql = string.Format(@"delete from S_F_EngineeringInvestPlan where EngineeringInfo='{0}' and MakeYear={1} and MakeMonth={2}  ",
                engineeringInfo.ID, makeYear, makeMonth);
            foreach (var item in list)
            {
                item.SetValue("EngineeringInfo", engineeringInfo.ID);
                item.SetValue("EngineeringInfoName", engineeringInfo.Name);
                item.SetValue("MakeDate", Convert.ToDateTime(string.Format("{0}-{1}-01", makeYear, makeMonth)));
                item.SetValue("MakeYear", makeYear);
                item.SetValue("MakeMonth", makeMonth);
                item.SetValue("ChargerDept", engineeringInfo.ChargerDept);
                item.SetValue("ChargerDeptName", engineeringInfo.ChargerDeptName);
                item.SetValue("CreateUserID", userInfo.UserID);
                item.SetValue("CreateUser", userInfo.UserName);
                item.SetValue("CreateDate", DateTime.Now);
                item.SetValue("ModifyUserID", userInfo.UserID);
                item.SetValue("ModifyUser", userInfo.UserName);
                item.SetValue("ModifyDate", DateTime.Now);
                var strMonthValue = string.Empty;
                var monthValue = 0m;
                foreach (var planMonth in planMonths)
                {
                    strMonthValue = item.GetValue(planMonth.GetValue("ColumnName"));
                    if (string.IsNullOrWhiteSpace(strMonthValue))
                    {
                        continue;
                    }
                    if (decimal.TryParse(strMonthValue, out monthValue))
                    {
                        item.SetValue("PlanDate", Convert.ToDateTime(string.Format("{0}-{1}-01", planMonth.GetValue("Year"), planMonth.GetValue("Month"))));
                        item.SetValue("PlanYear", planMonth.GetValue("Year"));
                        item.SetValue("PlanMonth", planMonth.GetValue("Month")); 
                        item.SetValue("Amount", monthValue);

                        sql += item.CreateInsertSql(EPCSQLDB, "S_F_EngineeringInvestPlan", FormulaHelper.CreateGuid());
                    }

                }

            }
            EPCSQLDB.ExecuteNonQuery(sql);

            return Json("");
        }

    }
}
