using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Formula.Helper;
using Comprehensive.Logic.Domain;
using MvcAdapter;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class GoodsStockReportController : ComprehensiveFormController<R_G_GoodsReport>
    {
        public ActionResult List()
        {
            var items = EnumBaseHelper.GetEnumDef("System.DeptOrg").EnumItem.ToList();
            ViewBag.DeptOrg = items;

            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 1, false);
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth", false);
            monthCategory.SetDefaultItem(DateTime.Now.Month.ToString());
            monthCategory.Multi = false;
            tab.Categories.Add(monthCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var belongYear = qb.Items.FirstOrDefault(d => d.Field == "BelongYear");
            qb.Items.Remove(belongYear);
            var belongMonth = qb.Items.FirstOrDefault(d => d.Field == "BelongMonth");
            qb.Items.Remove(belongMonth);

            int year = 0;
            int month = 0;

            if (belongYear == null)
                year = int.Parse(this.GetQueryString("BelongYear"));
            else
                year = int.Parse(belongYear.Value.ToString());

            if (belongMonth == null)
                month = int.Parse(this.GetQueryString("BelongMonth"));
            else
                month = int.Parse(belongMonth.Value.ToString());

            var result = new List<Dictionary<string, object>>();
            var deptOrgList = EnumBaseHelper.GetEnumDef("System.DeptOrg").EnumItem.ToList();
            var goodsInfoList = this.ComprehensiveDbContext.Set<R_G_GoodsReport>().Where(qb).Where(d => d.BelongYear == year && d.BelongMonth == month).ToList();
            if (goodsInfoList.Count == 0)
            {
                var goodsList = this.ComprehensiveDbContext.Set<S_G_GoodsInfo>().ToList();
                foreach (var goods in goodsList)
                {
                    var goodsInfo = new Dictionary<string, object>();
                    goodsInfo.Add("GoodsID", goods.ID);
                    goodsInfo.Add("Name", goods.Name);
                    goodsInfo.Add("Model", goods.Model);
                    goodsInfo.Add("Unit", goods.Unit);
                    goodsInfo.Add("Remark", goods.Remark);

                    goodsInfo.Add("InitialCount", 0);
                    goodsInfo.Add("StockCount", 0);

                    foreach (var deptOrg in deptOrgList)
                    {
                        goodsInfo.Add(deptOrg.Code + "Quantity", 0);
                    }
                    result.Add(goodsInfo);
                }
            }
            else
            {
                foreach (var goods in goodsInfoList)
                {
                    var goodsInfo = new Dictionary<string, object>();
                    goodsInfo.Add("ID", goods.ID);
                    goodsInfo.Add("GoodsID", goods.GoodsID);
                    goodsInfo.Add("Name", goods.Name);
                    goodsInfo.Add("Model", goods.Model);
                    goodsInfo.Add("Unit", goods.Unit);
                    goodsInfo.Add("Remark", goods.Remark);
                    goodsInfo.Add("InitialCount", goods.InitialCount);
                    goodsInfo.Add("StockCount", goods.StockCount);

                    foreach (var deptOrg in deptOrgList)
                    {
                        var applyInfo = this.ComprehensiveDbContext.Set<R_G_GoodsReport_ApplyQuantity>().FirstOrDefault(d => d.R_G_GoodsReportID == goods.ID && d.ApplyDept == deptOrg.Code);
                        int? applyQuantity = null;
                        if (applyInfo != null)
                            applyQuantity = applyInfo.ApplyQuantity;
                        goodsInfo.Add(deptOrg.Code + "Quantity", applyQuantity);
                    }
                    result.Add(goodsInfo);
                }
            }

            GridData gridData = new GridData(result);
            gridData.total = qb.TotolCount;

            return Json(gridData);
        }

        public string ValidateReportExist(int belongYear, int belongMonth)
        {
            string res = "true";

            var goodsInfoList = this.ComprehensiveDbContext.Set<R_G_GoodsReport>().FirstOrDefault(d => d.BelongYear == belongYear && d.BelongMonth == belongMonth);
            if (goodsInfoList == null)
            {
                res = "false";
            }

            return res;
        }

        public void CreateNewReport(string isReset, int belongYear, int belongMonth)
        {
            if (isReset == "T")
            {
                this.ComprehensiveDbContext.Set<R_G_GoodsReport>().Delete(d => d.BelongYear == belongYear && d.BelongMonth == belongMonth);
            }

            var deptOrgList = EnumBaseHelper.GetEnumDef("System.DeptOrg").EnumItem.ToList();
            var goodsInfoList = this.ComprehensiveDbContext.Set<S_G_GoodsInfo>().ToList();
            int lastYear = belongYear;
            int lastMonth = belongMonth;
            if (belongMonth == 1)
            {
                lastYear = belongYear - 1;
                lastMonth = 12;
            }

            //上月领用单IDs
            var appliedList = this.ComprehensiveDbContext.Set<T_G_GoodsApply>()
                                .Where(d => d.ApplyDate.Value.Year == lastYear
                                        && d.ApplyDate.Value.Month == lastMonth).ToList();
            var appliedIDs = appliedList.Select(d => d.ID).ToList();

            //当月领用单IDs
            var applyList = this.ComprehensiveDbContext.Set<T_G_GoodsApply>()
                                .Where(d => d.ApplyDate.Value.Year == belongYear
                                        && d.ApplyDate.Value.Month == belongMonth).ToList();
            var applyIDs = applyList.Select(d => d.ID).ToList();

            foreach (var goodsInfo in goodsInfoList)
            {
                var goodsReport = new R_G_GoodsReport();
                goodsReport.ID = FormulaHelper.CreateGuid();
                goodsReport.Name = goodsInfo.Name;
                goodsReport.Model = goodsInfo.Model;
                goodsReport.Unit = goodsInfo.Unit;
                goodsReport.Remark = goodsInfo.Remark;
                goodsReport.GoodsID = goodsInfo.ID;
                goodsReport.BelongYear = belongYear;
                goodsReport.BelongMonth = belongMonth;

                //期初数
                int initialCount = 0;
                var addedGoodsInfo = this.ComprehensiveDbContext.Set<S_G_GoodsAdditional>().Where(d => d.AdditionalData.Value.Year == lastYear
                                                                                   && d.AdditionalData.Value.Month == lastMonth
                                                                                   && d.Goods == goodsInfo.ID).ToList();
                var addedGoodsSum = addedGoodsInfo.Sum(d => d.Quantity.HasValue ? d.Quantity.Value : 0);
                var appliedGoodsInfo = this.ComprehensiveDbContext.Set<T_G_GoodsApply_ApplyDetail>().Where(d => appliedIDs.Contains(d.T_G_GoodsApplyID) && d.Goods == goodsInfo.ID).ToList();
                var appliedGoodsSum = appliedGoodsInfo.Sum(d => d.Quantity.HasValue ? d.Quantity.Value : 0);
                initialCount = goodsInfo.Quantity.Value + addedGoodsSum - appliedGoodsSum;
                goodsReport.InitialCount = initialCount;

                //库存数
                int stockCount = 0;
                var addGoodsInfo = this.ComprehensiveDbContext.Set<S_G_GoodsAdditional>().Where(d => d.AdditionalData.Value.Year == belongYear
                                                                                   && d.AdditionalData.Value.Month == belongMonth
                                                                                   && d.Goods == goodsInfo.ID).ToList();
                var addGoodsSum = addGoodsInfo.Sum(d => d.Quantity.HasValue ? d.Quantity.Value : 0);
                var applyGoodsList = this.ComprehensiveDbContext.Set<T_G_GoodsApply_ApplyDetail>().Where(d => applyIDs.Contains(d.T_G_GoodsApplyID) && d.Goods == goodsInfo.ID).ToList();
                var applyGoodsSum = applyGoodsList.Sum(d => d.Quantity.HasValue ? d.Quantity.Value : 0);
                stockCount = goodsInfo.Quantity.Value + addGoodsSum - applyGoodsSum;
                goodsReport.StockCount = stockCount;

                this.ComprehensiveDbContext.Set<R_G_GoodsReport>().Add(goodsReport);

                //各部门领用数
                foreach (var deptOrg in deptOrgList)
                {
                    var applyDetailInfo = new R_G_GoodsReport_ApplyQuantity();
                    applyDetailInfo.ID = FormulaHelper.CreateGuid();
                    applyDetailInfo.ApplyDept = deptOrg.Code;
                    applyDetailInfo.ApplyDeptName = deptOrg.Name;
                    applyDetailInfo.R_G_GoodsReportID = goodsReport.ID;

                    var applyListByDept = this.ComprehensiveDbContext.Set<T_G_GoodsApply>()
                                .Where(d => d.ApplyDate.Value.Year == belongYear
                                        && d.ApplyDate.Value.Month == belongMonth
                                        && d.ApplyDept == deptOrg.Code).ToList();
                    var applyIDsByDept = applyListByDept.Select(d => d.ID).ToList();
                    var applyGoodsInfoByDept = this.ComprehensiveDbContext.Set<T_G_GoodsApply_ApplyDetail>()
                                                .Where(d => applyIDsByDept.Contains(d.T_G_GoodsApplyID) && d.Goods == goodsInfo.ID).ToList();
                    var applyGoodsSumByDept = applyGoodsInfoByDept.Sum(d => d.Quantity);
                    applyDetailInfo.ApplyQuantity = applyGoodsSumByDept;
                    this.ComprehensiveDbContext.Set<R_G_GoodsReport_ApplyQuantity>().Add(applyDetailInfo);
                }
            }
            this.ComprehensiveDbContext.SaveChanges();
        }
    }
}
