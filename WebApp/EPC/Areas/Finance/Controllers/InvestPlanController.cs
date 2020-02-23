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
using Base.Logic.Domain;
using System.Data;

namespace EPC.Areas.Finance.Controllers
{
    public class InvestPlanController : EPCFormContorllor<S_F_InvestPlan>
    {
        private string prePlanMonths = "colMonth_";

        public override ActionResult PageView()
        {
            var id = Request["ID"];
            var makeYear = DateTime.Now.Year;
            var makeMonth = DateTime.Now.Month;
            var makeDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(id))
            {
                makeYear = Convert.ToInt32(Request["MakeYear"]);
                makeMonth = Convert.ToInt32(Request["MakeMonth"]);
                makeDate = Convert.ToDateTime(string.Format("{0}-{1}-01", makeYear, makeMonth));
            }
            else
            {
                var sql = string.Format(@"select * from S_F_InvestPlan where ID='{0}' ", id);
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("此单不存在！");
                }
                makeDate = Convert.ToDateTime(string.Format("{0}-{1}-01", dt.Rows[0]["MakeYear"], dt.Rows[0]["MakeMonth"]));
            }

            var planMonths = GetPlanMonths(makeDate);
            ViewBag.planMonths = JsonHelper.ToJson(planMonths);


            return base.PageView();
        }
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var maleYear = Convert.ToInt32(Request["MakeYear"]);
                var makeMonth = Convert.ToInt32(Request["MakeMonth"]);
                var chargerDept = Request["ChargerDept"];
                var chargerDeptName = Request["ChargerDeptName"];

                dic.SetValue("MakeDate", string.Format("{0}-{1}-01", maleYear, makeMonth));
                dic.SetValue("MakeYear", maleYear);
                dic.SetValue("MakeMonth", makeMonth);
                dic.SetValue("ChargerDept", chargerDept);
                dic.SetValue("ChargerDeptName", chargerDeptName);
                dic.SetValue("VersionNumber", 1);

            }

        }
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var sql = string.Format(@"select * from S_F_InvestPlan where MakeYear={0} and MakeMonth={1} and ChargerDept='{2}'  order by VersionNumber desc",
                     dic.GetValue("MakeYear"), dic.GetValue("MakeMonth"), dic.GetValue("ChargerDept"));
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    dic.SetValue("VersionNumber", "1");
                }
                else
                {
                    var list = FormulaHelper.DataTableToListDic(dt);
                    //if (list.Exists(d => d.GetValue("FlowPhase") != "End"))
                    //{
                    //    throw new Formula.Exceptions.BusinessValidationException(string.Format("部门【{0}】【{1}年{2}月】有正在审核中的计划编制，无法提交！", dic.GetValue("ChargerDeptName"), dic.GetValue("BelongYear"), dic.GetValue("BelongMonth")));
                    //}

                    var newVersionNumber = Convert.ToInt32(list[0].GetValue("VersionNumber")) + 1;
                    dic.SetValue("VersionNumber", newVersionNumber.ToString());

                }

            }

        }
        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var flowPhase = dic.GetValue("FlowPhase");
            if (string.IsNullOrWhiteSpace(flowPhase) || flowPhase == "Start")
            {
                var list = JsonHelper.ToList(dic.GetValue("dataGrid"));
                var makeDate = Convert.ToDateTime(dic.GetValue("MakeDate"));
                var planMonths = GetPlanMonths(makeDate);

                var sql = string.Empty;
                if (!isNew)
                {
                    sql = string.Format(@"delete from S_F_InvestPlan_Detail where S_F_InvestPlanID='{0}'  ", dic.GetValue("ID"));
                }

                foreach (var item in list)
                {
                    item.SetValue("S_F_InvestPlanID", dic.GetValue("ID"));
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

                            sql += item.CreateInsertSql(EPCSQLDB, "S_F_InvestPlan_Detail", FormulaHelper.CreateGuid());
                        }

                    }

                }
                EPCSQLDB.ExecuteNonQuery(sql);
            }


        }

        private List<Dictionary<string, object>> GetPlanMonths(DateTime makeDate)
        {
            var planMonths = new List<Dictionary<string, object>>();
            var planDate = makeDate;
            for (int i = 1; i <= 6; i++)
            {
                planDate = makeDate.AddMonths(i);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("ColumnName", string.Format("{0}{1}_{2}", prePlanMonths, planDate.Year, planDate.Month));
                dic.SetValue("Year", planDate.Year);
                dic.SetValue("Month", planDate.Month);
                planMonths.Add(dic);
            }
            return planMonths;
        }

        public JsonResult GetDetailList()
        {
            var investPlanID = Request["InvestPlanID"];
            var planMonths = JsonHelper.ToList(Request["planMonths"]);

            var sql = string.Format(@"select * from S_F_InvestPlan_Detail where S_F_InvestPlanID='{0}' ", investPlanID);
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            var InvestPlanList = FormulaHelper.DataTableToListDic(dt);

            var defaultRows = from t in dt.AsEnumerable()
                              group t by new
                              {
                                  EngineeringInfo = t.Field<string>("EngineeringInfo"),
                                  EngineeringInfoName = t.Field<string>("EngineeringInfoName"),
                                  FundType = t.Field<string>("FundType")
                              } into m
                              orderby m.Key.EngineeringInfoName ascending, m.Key.FundType descending
                              select new
                              {
                                  EngineeringInfo = m.Key.EngineeringInfo,
                                  EngineeringInfoName = m.Key.EngineeringInfoName,
                                  FundType = m.Key.FundType
                              };

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var defaultRow in defaultRows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("EngineeringInfo", defaultRow.EngineeringInfo);
                dic.SetValue("EngineeringInfoName", defaultRow.EngineeringInfoName);
                dic.SetValue("FundType", defaultRow.FundType);

                //月份列
                foreach (var planMonth in planMonths)
                {
                    if (InvestPlanList.Count == 0)
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), 0m);
                        continue;
                    }
                    var InvestPlan = InvestPlanList.Find(p => p.GetValue("EngineeringInfo") == defaultRow.EngineeringInfo
                    && p.GetValue("FundType") == defaultRow.FundType
                    && p.GetValue("PlanYear") == planMonth.GetValue("Year")
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

            sql = string.Format(@"select * from S_F_InvestPlan where ID='{0}' ", investPlanID);
            dt = this.EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("此单不存在！");
            }
            var chargerDept = dt.Rows[0]["ChargerDept"].ToString();
            var makeYear = Convert.ToInt32(dt.Rows[0]["MakeYear"]);
            var makeMonth = Convert.ToInt32(dt.Rows[0]["MakeMonth"]);
            var latestPlan = LoadLatestPlan(chargerDept, makeYear, makeMonth, planMonths);

            return Json(new { DetailList = result, LatestPlan = latestPlan });
        }
        public JsonResult GetLatestPlan()
        {
            var chargerDept = Request["ChargerDept"];
            var makeYear = Convert.ToInt32(Request["MakeYear"]);
            var makeMonth = Convert.ToInt32(Request["MakeMonth"]);
            var planMonths = JsonHelper.ToList(Request["planMonths"]);
            var versionNumber = Request["VersionNumber"];

            var result = LoadLatestPlan(chargerDept, makeYear, makeMonth, planMonths);

            return Json(result);
        }
        private List<Dictionary<string, object>> LoadLatestPlan(string chargerDept, int makeYear, int makeMonth, List<Dictionary<string, object>> planMonths)
        {
            var sql = string.Format(@"select * from S_F_EngineeringInvestPlan where ChargerDept='{0}' and MakeYear={1} and MakeMonth={2} ",
                            chargerDept, makeYear, makeMonth);
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            var InvestPlanList = FormulaHelper.DataTableToListDic(dt);

            var defaultRows = from t in dt.AsEnumerable()
                              group t by new
                              {
                                  EngineeringInfo = t.Field<string>("EngineeringInfo"),
                                  EngineeringInfoName = t.Field<string>("EngineeringInfoName"),
                                  FundType = t.Field<string>("FundType")
                              } into m
                              orderby m.Key.EngineeringInfoName ascending, m.Key.FundType descending
                              select new
                              {
                                  EngineeringInfo = m.Key.EngineeringInfo,
                                  EngineeringInfoName = m.Key.EngineeringInfoName,
                                  FundType = m.Key.FundType
                              };

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var defaultRow in defaultRows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("EngineeringInfo", defaultRow.EngineeringInfo);
                dic.SetValue("EngineeringInfoName", defaultRow.EngineeringInfoName);
                dic.SetValue("FundType", defaultRow.FundType);

                //月份列
                foreach (var planMonth in planMonths)
                {
                    if (InvestPlanList.Count == 0)
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), 0m);
                        continue;
                    }
                    var InvestPlan = InvestPlanList.Find(p => p.GetValue("EngineeringInfo") == defaultRow.EngineeringInfo
                    && p.GetValue("FundType") == defaultRow.FundType
                    && p.GetValue("PlanYear") == planMonth.GetValue("Year")
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
            return result;

        }

        #region 投资类报表（财务部）
        public ActionResult Report()
        {
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

            var deptCategory = CategoryFactory.GetCategory("EPC.FundPlanDept", "部门", "ChargerDept", true);
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            var planMonths = GetPlanMonths(dateNow);
            ViewBag.planMonths = JsonHelper.ToJson(planMonths);

            return View();
        }
        public JsonResult GetAllPlan(QueryBuilder qb)
        {

            var chargerDept = qb.Items.Find(q => q.Field == "ChargerDept");
            var makeYear = Convert.ToInt32(qb.Items.Find(q => q.Field == "MakeYear").Value);
            var makeMonth = Convert.ToInt32(qb.Items.Find(q => q.Field == "MakeMonth").Value);
            var planMonths = JsonHelper.ToList(Request["planMonths"]);

            var sql = string.Format(@"select InvestPlan.ChargerDept,InvestPlan.ChargerDeptName,InvestPlan.MakeYear,InvestPlan.MakeMonth,
Detail.* from (
select ChargerDept,MakeYear,MakeMonth,max(VersionNumber) as VersionNumber 
from S_F_InvestPlan where FlowPhase='End' and MakeYear={0} and MakeMonth={1} group by ChargerDept,MakeYear,MakeMonth 
) LatestPlan
inner join S_F_InvestPlan InvestPlan on LatestPlan.ChargerDept=InvestPlan.ChargerDept 
and LatestPlan.MakeYear=InvestPlan.MakeYear and LatestPlan.MakeMonth=InvestPlan.MakeMonth 
and LatestPlan.VersionNumber=InvestPlan.VersionNumber
inner join S_F_InvestPlan_Detail Detail on InvestPlan.ID=Detail.S_F_InvestPlanID ",makeYear, makeMonth);

            if (chargerDept !=null && !string.IsNullOrWhiteSpace(chargerDept.Value.ToString()))
            {
                sql += string.Format(" where InvestPlan.ChargerDept='{0}' ", chargerDept.Value.ToString());
            }
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            var InvestPlanList = FormulaHelper.DataTableToListDic(dt);

            var defaultRows = from t in dt.AsEnumerable()
                              group t by new
                              {
                                  ChargerDept = t.Field<string>("ChargerDept"),
                                  ChargerDeptName = t.Field<string>("ChargerDeptName"),
                                  EngineeringInfo = t.Field<string>("EngineeringInfo"),
                                  EngineeringInfoName = t.Field<string>("EngineeringInfoName"),
                                  FundType = t.Field<string>("FundType")
                              } into m
                              orderby m.Key.ChargerDeptName ascending, m.Key.EngineeringInfoName ascending, m.Key.FundType descending
                              select new
                              {
                                  ChargerDept = m.Key.ChargerDept,
                                  ChargerDeptName = m.Key.ChargerDeptName,
                                  EngineeringInfo = m.Key.EngineeringInfo,
                                  EngineeringInfoName = m.Key.EngineeringInfoName,
                                  FundType = m.Key.FundType
                              };

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var defaultRow in defaultRows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("ChargerDept", defaultRow.ChargerDept);
                dic.SetValue("ChargerDeptName", defaultRow.ChargerDeptName);
                dic.SetValue("EngineeringInfo", defaultRow.EngineeringInfo);
                dic.SetValue("EngineeringInfoName", defaultRow.EngineeringInfoName);
                dic.SetValue("FundType", defaultRow.FundType);

                //月份列
                foreach (var planMonth in planMonths)
                {
                    if (InvestPlanList.Count == 0)
                    {
                        dic.SetValue(planMonth.GetValue("ColumnName"), 0m);
                        continue;
                    }
                    var InvestPlan = InvestPlanList.Find(p => p.GetValue("ChargerDept") == defaultRow.ChargerDept
                    && p.GetValue("EngineeringInfo") == defaultRow.EngineeringInfo
                    && p.GetValue("FundType") == defaultRow.FundType
                    && p.GetValue("PlanYear") == planMonth.GetValue("Year")
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

        #endregion
    }
}
