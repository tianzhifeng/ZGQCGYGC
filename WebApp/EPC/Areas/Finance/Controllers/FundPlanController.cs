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

namespace EPC.Areas.Finance.Controllers
{
    public class FundPlanController : EPCFormContorllor<S_F_DeptFundPlan>
    {

        public JsonResult ValidateStart()
        {
            var belongYear = Convert.ToInt32(Request["BelongYear"]);
            var belongMonth = Convert.ToInt32(Request["BelongMonth"]);
            var chargerDept = Request["ChargerDept"];

            var sql = string.Format(@"select * from S_F_DeptFundPlan where BelongYear={0} and BelongMonth={1} and ChargerDept='{2}' and FlowPhase='Start'  order by VersionNumber desc",
                 belongYear, belongMonth, chargerDept);
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                return Json(new { ID = dt.Rows[0]["ID"] });
            }

            return Json("");
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var belongYear = Convert.ToInt32(Request["BelongYear"]);
                var belongMonth = Convert.ToInt32(Request["BelongMonth"]);
                var chargerDept = Request["ChargerDept"];
                var chargerDeptName = Request["ChargerDeptName"];

                dic.SetValue("BelongDate", string.Format("{0}-{1}-01", belongYear, belongMonth));
                dic.SetValue("BelongYear", belongYear);
                dic.SetValue("BelongMonth", belongMonth);
                dic.SetValue("ChargerDept", chargerDept);
                dic.SetValue("ChargerDeptName", chargerDeptName);
                dic.SetValue("VersionNumber", 1);

            }
            else
            {
                var sql = string.Format(@"select * from S_F_EngineeringFundPlan where BelongYear={0} and BelongMonth={1} and ChargerDept='{2}' order by SortIndex ",
        dic.GetValue("BelongYear"), dic.GetValue("BelongMonth"), dic.GetValue("ChargerDept"));
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                dic.SetValue("LatestPlan", JsonHelper.ToJson(dt));
            }


        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var sql = string.Format(@"select * from S_F_DeptFundPlan where BelongYear={0} and BelongMonth={1} and ChargerDept='{2}'  order by VersionNumber desc",
                     dic.GetValue("BelongYear"), dic.GetValue("BelongMonth"), dic.GetValue("ChargerDept"));
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

            if (string.IsNullOrWhiteSpace(dic.GetValue("Detail")))
            {
                dic.SetValue("ReceiveValue", "0");
                dic.SetValue("PaymentValue", "0");
            }
            else
            {
                var list = JsonHelper.ToList(dic.GetValue("Detail"));
                var receiveValue = 0m;
                var paymentValue = 0m;
                var amount = 0m;
                foreach (var item in list)
                {
                    amount = Convert.ToDecimal(item.GetValue("Amount"));
                    switch (item.GetValue("FundType"))
                    {
                        case "Receive":
                            receiveValue += amount;
                            break;

                        case "Payment":
                            paymentValue += amount;
                            break;

                        default:
                            break;
                    }
                }
                receiveValue = Math.Round(receiveValue, 2);
                paymentValue = Math.Round(paymentValue, 2);
                dic.SetValue("ReceiveValue", receiveValue.ToString());
                dic.SetValue("PaymentValue", paymentValue.ToString());

            }
        }

        public JsonResult GetLatestPlan()
        {
            var belongYear = Convert.ToInt32(Request["BelongYear"]);
            var belongMonth = Convert.ToInt32(Request["BelongMonth"]);
            var chargerDept = Request["ChargerDept"];
            var versionNumber = Request["VersionNumber"];

            var sql = string.Format(@"select * from S_F_EngineeringFundPlan where BelongYear={0} and BelongMonth={1} and ChargerDept='{2}' order by SortIndex ",
                belongYear, belongMonth, chargerDept);
            var data = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(data);
        }


    }
}
