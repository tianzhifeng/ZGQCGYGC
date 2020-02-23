using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Areas.Contract.Controllers
{
    public class PlanReceiptController : EPCFormContorllor<S_M_ContractInfo_ReceiptObj_PlanReceipt>
    {
        public ActionResult List()
        {
            var tab = new Tab();
            var planStateCategory = CategoryFactory.GetCategory(typeof(PlanReceiptState), "State");
            planStateCategory.SetDefaultItem(PlanReceiptState.UnReceipt.ToString());
            tab.Categories.Add(planStateCategory);

            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 2);
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(yearCategory);

            var quarterCategory = CategoryFactory.GetQuarterCategory("BelongQuarter");
            quarterCategory.SetDefaultItem(((DateTime.Now.Month - 1) / 3 + 1).ToString());
            tab.Categories.Add(quarterCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth");
            tab.Categories.Add(monthCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            ViewBag.DefaultState = PlanReceiptState.UnReceipt.ToString();
            ViewBag.DefaultYear = DateTime.Now.Year.ToString();
            ViewBag.DefaultQuarter = ((DateTime.Now.Month - 1) / 3 + 1).ToString();
            ViewBag.DefaultMonth = DateTime.Now.Month.ToString();
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            if (!String.IsNullOrEmpty(engineeringInfoID))
            {
                qb.Add("ProjectInfo", QueryMethod.Equal, engineeringInfoID);
            }
            DateTime currentMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            string sql = @" select * from (select S_M_ContractInfo_ReceiptObj_PlanReceipt.*,S_M_ContractInfo_ReceiptObj.FactInvoiceValue,SerialNumber,
S_M_ContractInfo.ProjectInfo,
case when PlanDate< '" + currentMonthFirstDay.ToShortDateString() + "' and S_M_ContractInfo_ReceiptObj_PlanReceipt.State='" + PlanReceiptState.UnReceipt + @"' then 'T' else 'F' end as IsDelay 
from S_M_ContractInfo_ReceiptObj_PlanReceipt  left join S_M_ContractInfo_ReceiptObj 
on S_M_ContractInfo_ReceiptObj_PlanReceipt.S_M_ContractInfo_ReceiptObjID=S_M_ContractInfo_ReceiptObj.ID
left join S_M_ContractInfo on S_M_ContractInfo.ID=S_M_ContractInfo_ReceiptObj_PlanReceipt.ContractInfoID ) planDataInfo ";
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            string sumSql = @"select Sum(PlanValue) as PlanValue,Sum(FactValue) as FactValue,
Sum(BadDebtValue) as BadDebtValue  from (" + sql + qb.GetWhereString() + ") PlanData ";
            var sumDt = this.EPCSQLDB.ExecuteDataTable(sumSql);
            if (sumDt != null && sumDt.Rows.Count > 0)
            {
                data.sumData.SetValue("PlanValue", sumDt.Rows[0]["PlanValue"]);
                data.sumData.SetValue("FactValue", sumDt.Rows[0]["FactValue"]);
                data.sumData.SetValue("BadDebtValue", sumDt.Rows[0]["BadDebtValue"]);
            }
            return Json(data);
        }

        public JsonResult GetModel(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var entity = this.GetEntityByID(id);
                if (entity == null)
                    throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + id + "】的计划收款对象");
                var dic = FormulaHelper.ModelToDic<S_M_ContractInfo_ReceiptObj_PlanReceipt>(entity);
                return Json(dic);
            }
            else
                return Json("");
        }

        public void SavePlanReceipt(string planReceiptData)
        {
            var planDic = JsonHelper.ToObject(planReceiptData);
            var planReceipt = this.GetEntityByID(planDic.GetValue("ID"));
            if (planReceipt == null)
                throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + planDic.GetValue("ID") + "】的计划收款对象，修改计划收款失败");
            this.UpdateEntity<S_M_ContractInfo_ReceiptObj_PlanReceipt>(planReceipt, planDic);
            planReceipt.Save();
            this.EPCEntites.SaveChanges();
        }

        public void DelayPlan(string PlanReceiptData, string NewPlanDate)
        {
            var list = JsonHelper.ToList(PlanReceiptData);
            if (String.IsNullOrEmpty(NewPlanDate.Trim('\"'))) throw new Formula.Exceptions.BusinessValidationException("必须指定延迟日期");
            foreach (var item in list)
            {
                var plan = this.GetEntityByID(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + item.GetValue("ID") + "】的计划收款信息，无法延迟操作");
                var date = Convert.ToDateTime(NewPlanDate.Trim('\"'));
                plan.Delay(date);
            }
            this.EPCEntites.SaveChanges();
        }

        public void SplitPlanReceipt(string PlanReceiptID, string SplitData)
        {
            var planReceipt = this.GetEntityByID(PlanReceiptID);
            if (planReceipt == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + PlanReceiptID + "】的计划收款信息，无法拆分操作");
            var data = JsonHelper.ToObject(SplitData);
            var splitValue = data.GetValue("SplitValue");
            if (String.IsNullOrEmpty(splitValue)) throw new Formula.Exceptions.BusinessValidationException("必须指定拆分金额才能对计划收款进行拆分");
            DateTime? newPlanDate = null;
            if (!String.IsNullOrEmpty(data.GetValue("NewPlanDate")))
                newPlanDate = Convert.ToDateTime(data.GetValue("NewPlanDate"));
            var newPlan = planReceipt.Split(Convert.ToDecimal(splitValue), newPlanDate);
            newPlan.Name = data.GetValue("Name");
            this.EPCEntites.SaveChanges();
        }

        public void CombinePlanReceipt(string PlanReceiptData)
        {
            var list = JsonHelper.ToList(PlanReceiptData);
            var planReceiptList = new List<S_M_ContractInfo_ReceiptObj_PlanReceipt>();
            foreach (var item in list)
            {
                var plan = this.GetEntityByID(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + item.GetValue("ID") + "】的计划收款信息，无法拆分操作");
                planReceiptList.Add(plan);
            }
            var targetPlan = planReceiptList.OrderBy(d => d.ID).ThenBy(d => d.PlanDate).FirstOrDefault(d => d.PlanDate != null);
            targetPlan.Combine(planReceiptList);
            EPCEntites.SaveChanges();
        }

        public void ValidateOperation(string PlanReceiptData)
        {
            var list = JsonHelper.ToList(PlanReceiptData);
            foreach (var item in list)
            {
                var planReceipt = this.GetEntityByID(item.GetValue("ID"));
                if (planReceipt == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + item.GetValue("ID") + "】的计划收款信息，无法延迟操作");
                if (planReceipt.State != PlanReceiptState.UnReceipt.ToString())
                    throw new Formula.Exceptions.BusinessValidationException("【" + planReceipt.Name + "】不是未到款状态，无法进行操作");
            }
        }

        public void SetBadDebt(string PlanReceiptData)
        {
            var list = JsonHelper.ToList(PlanReceiptData);
            foreach (var item in list)
            {
                var plan = this.GetEntityByID(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + item.GetValue("ID") + "】的计划收款信息");
                plan.BadDebt();
            }
            this.EPCEntites.SaveChanges();

        }

        public void RevertBadDebt(string PlanReceiptData)
        {
            var list = JsonHelper.ToList(PlanReceiptData);
            foreach (var item in list)
            {
                var plan = this.GetEntityByID(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + item.GetValue("ID") + "】的计划收款信息");
                plan.RevertBadDebt();
            }
            this.EPCEntites.SaveChanges();
        }

        public ActionResult ContractPlanReceiptList()
        {
            var tab = new Tab();
            var planStateCategory = CategoryFactory.GetCategory(typeof(PlanReceiptState), "State");
            planStateCategory.SetDefaultItem(PlanReceiptState.UnReceipt.ToString());
            tab.IsDisplay = true;
            tab.Categories.Add(planStateCategory);
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetPlanReceiptList(QueryBuilder qb, string ContractInfo)
        {
            string sql = @"select a.*,b.FactInvoiceValue from S_M_ContractInfo_ReceiptObj_PlanReceipt a 
left join  S_M_ContractInfo_ReceiptObj b on a.S_M_ContractInfo_ReceiptObjID=b.ID 
where a.ContractInfoID='" + ContractInfo + "'";
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }
    }
}
