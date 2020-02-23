using EPC.Logic.Domain;
using Config.Logic;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic;
using Formula;
using Config;

namespace EPC.Areas.Finance.Controllers
{
    public class CapitalCompanyPlanController : EPCFormContorllor<S_F_CapitalCompanyPlan>
    {
        private string yearAndMonthColPre = "colYearAndMonth";

        public ActionResult List()
        {
            var yearAndMonthCol = new List<Dictionary<string, object>>();
            string templateCode = GetQueryString("templateCode");

            //是否还有暂存但是未提交审批的计划
            bool flowEnd = true;
            bool justAdd = true;//仅仅增加了version 没有任何detail
            var plans = EPCEntites.Set<S_F_CapitalCompanyPlan>().Where(a => a.TemplateCode == templateCode)
                .OrderByDescending(a => a.ID);

            int count = plans.Count();
            ViewBag.PlanCount = count;

            //没发布任何计划
            if (count == 0)
            {
                ViewBag.LatestPlanId = "";
                flowEnd = true;
            }
            else
            {
                var plan = plans.FirstOrDefault();
                if (plan.FlowPhase != "End")
                    flowEnd = false;
                ViewBag.FlowPhase = plan.FlowPhase;
                ViewBag.LatestPlanId = plan.ID;

                var budgeDateList = EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>()
                    .Where(a => a.S_F_BudgetProjectID == plan.ID).ToList()
                    .GroupBy(a => a.BudgetDate).Select(a => a.First()).Select(a => a.BudgetDate).OrderBy(a => a.Value);


                foreach (var date in budgeDateList)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.SetValue("colName", yearAndMonthColPre + date.Value.Year + "_" + date.Value.Month);
                    dic.SetValue("year", date.Value.Year);
                    dic.SetValue("month", date.Value.Month);
                    yearAndMonthCol.Add(dic);
                }
            }

            justAdd = yearAndMonthCol.Count == 0;
            if (!flowEnd && justAdd)
            {
                //默认月数
                int monthCount = 3;
                DateTime now = DateTime.Now;
                for (int i = 0; i < monthCount; i++)
                {
                    DateTime next = now.AddMonths(i);
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.SetValue("colName", yearAndMonthColPre + next.Year + "_" + next.Month);
                    dic.SetValue("year", next.Year);
                    dic.SetValue("month", next.Month);
                    yearAndMonthCol.Add(dic);
                }
            }

            ViewBag.JustAdd = justAdd;
            ViewBag.FlowEnd = flowEnd;
            ViewBag.YearAndMonthCol = JsonHelper.ToJson(yearAndMonthCol);
            ViewBag.YearAndMonthColPre = yearAndMonthColPre;
            return View();
        }

        public override ActionResult PageView()
        {
            string sumID = GetQueryString("ID");
            var yearAndMonthCol = new List<Dictionary<string, object>>();

            var budgeDateList = EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>()
                    .Where(a => a.S_F_BudgetProjectID == sumID).ToList()
                    .GroupBy(a => a.BudgetDate).Select(a => a.First()).Select(a => a.BudgetDate).OrderBy(a => a.Value);

            foreach (var date in budgeDateList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("colName", yearAndMonthColPre + date.Value.Year + "_" + date.Value.Month);
                dic.SetValue("year", date.Value.Year);
                dic.SetValue("month", date.Value.Month);
                yearAndMonthCol.Add(dic);
            }

            ViewBag.YearAndMonthCol = JsonHelper.ToJson(yearAndMonthCol);
            return base.PageView();
        }

        protected override void OnFlowEnd(S_F_CapitalCompanyPlan entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var detailList = EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().Where(a => a.S_F_BudgetProjectID == entity.ID).ToList();

            foreach (var detail in detailList)
            {
                //唯一数据
                S_F_CapitalAnalysis_Detail onlyOneDetail = EPCEntites.Set<S_F_CapitalAnalysis_Detail>().SingleOrDefault(
                    a => a.TemplateID == detail.TemplateID
                        && a.TemplateCode == entity.TemplateCode
                        && a.BudgetDate == detail.BudgetDate);

                //add
                if (onlyOneDetail == null)
                {
                    onlyOneDetail = new S_F_CapitalAnalysis_Detail();
                    onlyOneDetail.ID = FormulaHelper.CreateGuid();
                    onlyOneDetail.TemplateCode = entity.TemplateCode;
                    onlyOneDetail.TemplateID = detail.TemplateID;
                    onlyOneDetail.ParentTemplateID = detail.ParentTemplateID;
                    onlyOneDetail.FullTemplateID = detail.FullTemplateID;
                    onlyOneDetail.IsReadOnly = detail.IsReadOnly;
                    onlyOneDetail.SortIndex = detail.SortIndex;
                    onlyOneDetail.Name = detail.Name;
                    onlyOneDetail.BudgetDate = detail.BudgetDate;
                    onlyOneDetail.Year = detail.Year;
                    onlyOneDetail.Month = detail.Month;
                    onlyOneDetail.CapitalPlanType = detail.CapitalPlanType;
                    onlyOneDetail.SourceSQL = detail.SourceSQL;
                    onlyOneDetail.RealSourceSQL = detail.RealSourceSQL;
                    onlyOneDetail.SourceLink = detail.SourceLink;
                    onlyOneDetail.RealSourceLink = detail.RealSourceLink;
                    EPCEntites.Set<S_F_CapitalAnalysis_Detail>().Add(onlyOneDetail);
                }

                onlyOneDetail.BudgetValue = detail.BudgetValue;
            }
        }

        public JsonResult SaveList(string listData, string latestPlanID, string beginDate, string endDate)
        {
            var detailList = JsonHelper.ToList(listData);
            var plan = EPCEntites.Set<S_F_CapitalCompanyPlan>().Find(latestPlanID);
            if (plan == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到公司资金计划");
            }
            plan.BeginDate = Convert.ToDateTime(beginDate);
            plan.EndDate = Convert.ToDateTime(endDate);
            plan.Compiler = CurrentUserInfo.UserID;
            plan.CompilerName = CurrentUserInfo.UserName;
            plan.CreateDate = DateTime.Now;
            plan.CreateUserID = CurrentUserInfo.UserID;
            plan.CreateUser = CurrentUserInfo.UserName;
            plan.CompileDate = DateTime.Now;

            if (detailList.Count > 0)
            {
                AddToDetail(detailList, latestPlanID);
                //界面没有的数据库有的数据,则数据库删(保持与界面一致)
                List<DateTime> deleteDates = new List<DateTime>();
                foreach (var key in detailList[0].Keys)
                {
                    if (!key.Contains(yearAndMonthColPre))
                        continue;

                    DateTime bDate = Convert.ToDateTime(key.Replace(yearAndMonthColPre, "").Replace("_", "-"));
                    deleteDates.Add(bDate);
                }
                EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().Delete(a => a.S_F_BudgetProjectID == latestPlanID && !deleteDates.Contains(a.BudgetDate.Value));
            }

            EPCEntites.SaveChanges();
            return Json("");
        }

        //protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        //{
        //    base.BeforeSave(dic, formInfo, isNew);

        //    if (!isNew)
        //    {
        //        return;
        //    }

        //    //保存子表
        //    var detailList = JsonHelper.ToList(dic.GetValue("Detail"));
        //    AddToDetail(detailList, dic.GetValue("ID"));
        //}

        public JsonResult MakePlan(string templateCode)
        {
            var infrastructureEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            if (!infrastructureEntities.Set<S_F_CapitalPlanTemplate>().Any(a => a.SerialNumber == templateCode))
            {
                throw new Exception("未找到编号为【" + templateCode + "】的资金计划模板");
            }

            var userInfo = FormulaHelper.GetUserInfo();

            var newPlan = new S_F_CapitalCompanyPlan();
            newPlan.ID = FormulaHelper.CreateGuid();
            newPlan.TemplateCode = templateCode;
            newPlan.CreateDate = DateTime.Now;
            newPlan.CreateUser = userInfo.UserName;
            newPlan.CreateUserID = userInfo.UserID;
            newPlan.FlowPhase = "Start";
            newPlan.FlowInfo = "";
            EPCEntites.Set<S_F_CapitalCompanyPlan>().Add(newPlan);
            EPCEntites.SaveChanges();
            return Json("");
        }

        //获取子表数据
        public JsonResult GetBudgetList(string id, string monthColArr)
        {
            if (string.IsNullOrEmpty(id))
                return Json("");

            var sourceDicList = JsonHelper.ToList(monthColArr);
            //预算明细
            var detailList = EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().Where(a => a.S_F_BudgetProjectID == id).ToList();
            //预算明细对应的模板id列表
            var templateIDList = detailList.GroupBy(a => a.TemplateID).Select(a => a.First()).Select(a => a.TemplateID);

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            foreach (var templateID in templateIDList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var sameTemplateIDDetailList = detailList.Where(a => a.TemplateID == templateID);
                var tmp = sameTemplateIDDetailList.FirstOrDefault();

                dic.SetValue("Name", tmp.Name);
                dic.SetValue("ID", templateID);
                dic.SetValue("ParentID", tmp.ParentTemplateID);
                dic.SetValue("FullID", tmp.FullTemplateID);
                dic.SetValue("SortIndex", tmp.SortIndex);
                dic.SetValue("CapitalPlanType", tmp.CapitalPlanType);
                dic.SetValue("AddOrSub", GetAddOrSub(tmp.CapitalPlanType));
                dic.SetValue("SourceSQL", tmp.SourceSQL);
                dic.SetValue("RealSourceSQL", tmp.RealSourceSQL);
                dic.SetValue("SourceLink", tmp.SourceLink);
                dic.SetValue("RealSourceLink", tmp.RealSourceLink);
                dic.SetValue("IsReadOnly", tmp.IsReadOnly);
                dic.SetValue("HaveSource", !string.IsNullOrEmpty(tmp.SourceSQL));
                dic.SetValue("HaveLink", !string.IsNullOrEmpty(tmp.SourceLink));

                //月份列
                foreach (var sourceDic in sourceDicList)
                {
                    DateTime budgetDate = Convert.ToDateTime(sourceDic.GetValue("year") + "-" + sourceDic.GetValue("month"));
                    var cell = sameTemplateIDDetailList.SingleOrDefault(a => a.TemplateID == templateID && a.BudgetDate == budgetDate);
                    dic.SetValue(sourceDic.GetValue("colName"), cell != null ? cell.BudgetValue : 0);
                }
                result.Add(dic);
            }

            return Json(result.OrderBy(a => a.GetValue("SortIndex")));
        }

        public JsonResult GetTemplateList(string monthColArr, string templateCode)
        {
            var sourceDicList = JsonHelper.ToList(monthColArr);

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var infrastructureEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var planTemplate = infrastructureEntities.Set<S_F_CapitalPlanTemplate>().SingleOrDefault(a => a.SerialNumber == templateCode);
            if (planTemplate == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到编号为【" + templateCode + "】的公司资金计划模板");
            }
            FillChildrenDic("", "", planTemplate, ref result, sourceDicList);
            //FillSum(result, sourceDicList);

            return Json(result);
        }

        private void AddToDetail(List<Dictionary<string, object>> childrenDics, string budgetProjectID)
        {
            foreach (var child in childrenDics)
            {
                //数据库没有界面的数据则数据库补,有的部分更新
                foreach (var key in child.Keys)
                {
                    if (!key.Contains(yearAndMonthColPre))
                        continue;

                    DateTime bDate = Convert.ToDateTime(key.Replace(yearAndMonthColPre, "").Replace("_", "-"));
                    string templateID = child.GetValue("ID");
                    var detail = EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().SingleOrDefault(a => a.TemplateID == templateID && a.BudgetDate == bDate && a.S_F_BudgetProjectID == budgetProjectID);
                    //增加
                    if (detail == null)
                    {
                        detail = new S_F_CapitalCompanyPlan_Detail();
                        detail.ID = FormulaHelper.CreateGuid();
                        detail.S_F_BudgetProjectID = budgetProjectID;
                        detail.SortIndex = Convert.ToDouble(child.GetValue("SortIndex"));
                        detail.TemplateID = child.GetValue("ID");
                        detail.Name = child.GetValue("Name");
                        detail.ParentTemplateID = child.GetValue("ParentID");
                        detail.FullTemplateID = child.GetValue("FullID");
                        detail.IsReadOnly = Convert.ToBoolean(child.GetValue("IsReadOnly"));
                        detail.BudgetDate = bDate;
                        detail.Year = detail.BudgetDate.Value.Year.ToString();
                        detail.Month = detail.BudgetDate.Value.Month.ToString();
                        detail.BudgetValue = Convert.ToDecimal(child.GetValue(key));
                        detail.CapitalPlanType = child.GetValue("CapitalPlanType");
                        detail.SourceSQL = child.GetValue("SourceSQL");
                        detail.RealSourceSQL = child.GetValue("RealSourceSQL");
                        detail.SourceLink = child.GetValue("SourceLink");
                        detail.RealSourceLink = child.GetValue("RealSourceLink");
                        EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().Add(detail);
                    }
                    detail.BudgetValue = Convert.ToDecimal(child.GetValue(key));
                }

                var childChildrenDics = JsonHelper.ToList(child.GetValue("children"));
                AddToDetail(childChildrenDics, budgetProjectID);
            }
        }

        private void FillChildrenDic(string parentTemplateId, string parentFullId, S_F_CapitalPlanTemplate plan, ref List<Dictionary<string, object>> result, List<Dictionary<string, object>> sourceDicList)
        {
            var infrastructureEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var tmpList = infrastructureEntities.Set<S_F_CapitalPlanTemplate_Detail>().Where(a => a.ParentID == parentTemplateId && a.S_F_CapitalPlanTemplateID == plan.ID).OrderBy(a => a.SortIndex).ToList();
            foreach (var tmp in tmpList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string newDicId = FormulaHelper.CreateGuid();
                dic.SetValue("Name", tmp.Name);
                string newID = FormulaHelper.CreateGuid();
                dic.SetValue("ID", tmp.ID);
                dic.SetValue("ParentID", tmp.ParentID);
                dic.SetValue("FullID", tmp.FullID);
                dic.SetValue("SortIndex", tmp.SortIndex);
                dic.SetValue("CapitalPlanType", tmp.CapitalPlanType);
                dic.SetValue("AddOrSub", GetAddOrSub(tmp.CapitalPlanType));
                dic.SetValue("SourceSQL", tmp.SourceSQL);
                dic.SetValue("RealSourceSQL", tmp.RealSourceSQL);
                dic.SetValue("SourceLink", tmp.SourceLink);
                dic.SetValue("RealSourceLink", tmp.RealSourceLink);
                dic.SetValue("IsReadOnly", tmp.IsReadOnly);
                dic.SetValue("HaveSource", !string.IsNullOrEmpty(tmp.SourceSQL));
                dic.SetValue("HaveLink", !string.IsNullOrEmpty(tmp.SourceLink));

                //月份列
                foreach (var sourceDic in sourceDicList)
                {
                    //string sql = tmp.SourceSQL;
                    //if (!string.IsNullOrEmpty(sql))
                    //{
                    //    DateTime budgetDate = new DateTime(Convert.ToInt32(sourceDic.GetValue("year")), Convert.ToInt32(sourceDic.GetValue("month")), 1);
                    //    sql = sql.Replace("{CapitalPlanDate}", budgetDate.ToString("yyyy-MM-dd"));

                    //    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                    //    object obj = sqlHelper.ExecuteScalar(sql);
                    //    decimal val = 0;
                    //    if (obj != null && decimal.TryParse(obj.ToString(), out val))
                    //    {
                    //        dic.SetValue(sourceDic.GetValue("colName"), val);
                    //        continue;
                    //    }
                    //}

                    dic.SetValue(sourceDic.GetValue("colName"), 0);
                }

                result.Add(dic);
                FillChildrenDic(tmp.ID, tmp.FullID, plan, ref result, sourceDicList);
            }
        }

        public JsonResult GetSourceResult(string nodesHaveSource, string yearAndMonth)
        {
            var nodeList = JsonHelper.ToList(nodesHaveSource);
            var yearAndMonthDic = JsonHelper.ToObject(yearAndMonth);

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var node in nodeList)
            {
                Dictionary<string, object> singResult = new Dictionary<string, object>();
                singResult.SetValue("ID", node.GetValue("ID"));
                singResult.SetValue("ParentID", node.GetValue("ParentID"));
                singResult.SetValue("colName", yearAndMonthDic.GetValue("colName"));
                result.Add(singResult);

                string year = yearAndMonthDic.GetValue("year");
                string month = yearAndMonthDic.GetValue("month");

                //var template = EPCEntites.Set<S_F_CapitalPlanTemplate_Detail>().Find(node.GetValue("templateID"));
                //if (template != null)
                {
                    string sql = node.GetValue("SourceSQL");
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sql = sql.Replace("{year}", year).Replace("{month}", month);
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                        object obj = sqlHelper.ExecuteScalar(sql);
                        decimal val = 0;
                        if (obj != null && decimal.TryParse(obj.ToString(), out val))
                        {
                            singResult.SetValue("SourceVal", val);
                        }
                        else
                        {
                            singResult.SetValue("SourceVal", 0);
                        }
                    }
                }
                //else
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("模板不存在");
                //}
            }

            return Json(result);
        }

        public JsonResult Revert(string latestPlanID)
        {
            EPCEntites.Set<S_F_CapitalCompanyPlan>().Delete(a => a.ID == latestPlanID);
            EPCEntites.Set<S_F_CapitalCompanyPlan_Detail>().Delete(a => a.S_F_BudgetProjectID == latestPlanID);
            EPCEntites.SaveChanges();
            return Json("");
        }

        private void FillSum(List<Dictionary<string, object>> result, List<Dictionary<string, object>> monthDicList)
        {
            var parents = result.Where(a => a.GetValue("ParentID") == ""
               && a.GetValue("CapitalPlanType") != CapitalPlanType.Total.ToString());

            foreach (var p in parents)
            {
                foreach (var monthDic in monthDicList)
                {
                    var children = result.Where(a => a.GetValue("ParentID") == p.GetValue("ID"));
                    decimal val = 0;
                    foreach (var child in children)
                    {
                        decimal tmp = 0;
                        decimal.TryParse(child.GetValue(monthDic.GetValue("colName")), out tmp);
                        val += tmp;
                    }
                    p.SetValue(monthDic.GetValue("colName"), val);
                }
            }

            //total
            var totalItem = result.SingleOrDefault(a => a.GetValue("CapitalPlanType") == CapitalPlanType.Total.ToString());
            {
                foreach (var monthDic in monthDicList)
                {
                    var children = result.Where(a => a.GetValue("ParentID") == "");
                    decimal val = 0;
                    foreach (var child in children)
                    {
                        decimal tmp = 0;
                        decimal.TryParse(child.GetValue(monthDic.GetValue("colName")), out tmp);
                        val += GetAddOrSub(child.GetValue("CapitalPlanType")) * tmp;
                    }
                    totalItem.SetValue(monthDic.GetValue("colName"), val);
                }
            }
        }

        private decimal GetAddOrSub(string capitalPlanType)
        {
            if (capitalPlanType == CapitalPlanType.In.ToString())
            {
                return 1;
            }
            else if (capitalPlanType == CapitalPlanType.Out.ToString())
            {
                return -1;
            }
            else if (capitalPlanType == CapitalPlanType.Initial.ToString())
            {
                return 1;
            }
            else if (capitalPlanType == CapitalPlanType.Total.ToString())
            {
                //无意义
                return 9999;
            }
            else
            {
                return 1;
                //throw new Formula.Exceptions.BusinessValidationException(capitalPlanType + "未在枚举CapitalPlanType中找到对应值");
            }
        }
    }
}
