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
    public class EngineeringFundPlanController : EPCFormContorllor<S_F_EngineeringFundPlan>
    {
        public ActionResult List()
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(Request["EngineeringInfoID"]);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("工程信息不存在！");

            var sql = string.Format(@"select Top 1  ID,Name,SerialNumber,PartyA,PartyAName from S_M_ContractInfo where ProjectInfo='{0}' and ContractState='Sign' ", engineeringInfo.ID);
            var contractInfo = EPCSQLDB.ExecuteDataTable(sql);
            ViewBag.ContractInfo = JsonHelper.ToJson(contractInfo);

            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 2, false, "年份");
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth", false, "月份");
            monthCategory.SetDefaultItem(DateTime.Now.Month.ToString());
            monthCategory.Multi = false;
            tab.Categories.Add(monthCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var engineeringInfo = GetQueryString("EngineeringInfoID");
            var belongYear = Convert.ToInt32(qb.Items.Find(q => q.Field == "BelongYear").Value);
            var belongMonth = Convert.ToInt32(qb.Items.Find(q => q.Field == "BelongMonth").Value);
            var sql = string.Format(@"select * from S_F_EngineeringFundPlan where EngineeringInfo='{0}' and BelongYear={1} and BelongMonth={2} order by SortIndex",
                engineeringInfo, belongYear, belongMonth);
            var data = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(data);
        }

        public JsonResult SaveList()
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(Request["EngineeringInfoID"]);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("工程信息不存在！");

            var list = JsonHelper.ToList(Request["List"]);
            var belongYear = Convert.ToInt32(Request["BelongYear"]);
            var belongMonth = Convert.ToInt32(Request["BelongMonth"]);

            var userInfo = FormulaHelper.GetUserInfo();
            var sql = string.Format(@"delete from S_F_EngineeringFundPlan where EngineeringInfo='{0}' and BelongYear={1} and BelongMonth={2}  ",
                engineeringInfo.ID, belongYear, belongMonth);
            var index = 0;
            foreach (var item in list)
            {
                item.SetValue("SortIndex", index);
                index++;
                item.SetValue("EngineeringInfo", engineeringInfo.ID);
                item.SetValue("EngineeringInfoName", engineeringInfo.Name);
                item.SetValue("BelongDate", Convert.ToDateTime(string.Format("{0}-{1}-01", belongYear, belongMonth)));
                item.SetValue("BelongYear", belongYear);
                item.SetValue("BelongMonth", belongMonth);
                item.SetValue("ChargerDept", engineeringInfo.ChargerDept);
                item.SetValue("ChargerDeptName", engineeringInfo.ChargerDeptName);
                if (string.IsNullOrWhiteSpace(item.GetValue("CreateUserID")))
                {
                    item.SetValue("CreateUserID", userInfo.UserID);
                    item.SetValue("CreateUser", userInfo.UserName);
                    item.SetValue("CreateDate", DateTime.Now);
                }
                item.SetValue("ModifyUserID", userInfo.UserID);
                item.SetValue("ModifyUser", userInfo.UserName);
                item.SetValue("ModifyDate", DateTime.Now);
                item.SetValue("MakeYear", DateTime.Now.Year);
                item.SetValue("MakeMonth", DateTime.Now.Month);
                if (string.IsNullOrWhiteSpace(item.GetValue("ID")))
                {
                    item.SetValue("ID", FormulaHelper.CreateGuid());
                }
                sql += item.CreateInsertSql(EPCSQLDB, "S_F_EngineeringFundPlan", item.GetValue("ID"));
            }
            EPCSQLDB.ExecuteNonQuery(sql);

            return Json("");
        }

    }
}
