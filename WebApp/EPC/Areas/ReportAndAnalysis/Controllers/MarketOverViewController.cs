using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EPC.Logic;
using EPC.Logic.Domain;
using Formula.Helper;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class MarketOverViewController : EPCController
    {
        public ActionResult MainPage()
        {
            int belongYear = String.IsNullOrEmpty(this.GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(this.GetQueryString("BelongYear"));
         
            int belongMonth = DateTime.Now.Month;
            var lastMonth = belongMonth == 12 ? 1 : belongMonth - 1;
            var lastYear = belongMonth == 12 ? belongYear - 1 : belongYear;

            string sql = @"select isnull(Sum(ReceiptValue),0) as DataValue,
Year(ReceiptDate) as BelongYear,(Month(ReceiptDate)-1)/3+1 as BelongQuarter,Month(ReceiptDate) as BelongMonth 
from S_M_Receipt group by Year(ReceiptDate),(Month(ReceiptDate)-1)/3+1,Month(ReceiptDate)  ";
            var receiptDt = this.SqlHelper.ExecuteDataTable(sql);

            sql = @"select * from S_M_KPICompany where KPIClass = 'Year' and BelongYear= '{0}'";
            var indicatorDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));
            var RecepitBlock = this._getViewBlock(indicatorDt, receiptDt, belongYear, "Receipt", "收款");
            RecepitBlock.MainUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProjectYearReceiptView&BelongYear=" + belongYear;
            RecepitBlock.SubViewBlockList[0].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProjectMonthReceiptView&BelongYear=" + belongYear + "&BelongQuarter=" + DateTimeHelper.GetQuarter(DateTime.Now);
            RecepitBlock.SubViewBlockList[1].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProjectMonthReceiptView&BelongYear=" + belongYear + "&BelongMonth=" + belongMonth;
            RecepitBlock.SubViewBlockList[2].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProjectMonthReceiptView&BelongYear=" + lastYear + "&BelongMonth=" + lastMonth;
            ViewBag.RecepitBlock = RecepitBlock;

            sql = @"select isnull(Sum(ContractRMBValue),0) as DataValue,Year(SignDate) as BelongYear,
(Month(SignDate)-1)/3+1 as BelongQuarter,Month(SignDate) as BelongMonth from S_M_ContractInfo
where SignDate is not null  group by Year(SignDate),(Month(SignDate)-1)/3+1,Month(SignDate) ";
            var contractDt = this.SqlHelper.ExecuteDataTable(sql);
            var ContractBlock = this._getViewBlock(indicatorDt, contractDt, belongYear, "Contract", "签订");
            ContractBlock.MainUrl = "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&IsSigned=Signed&BelongYear=" + belongYear;
            ContractBlock.SubViewBlockList[0].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&BelongYear=" + belongYear + "&BelongQuarter=" + DateTimeHelper.GetQuarter(DateTime.Now);
            ContractBlock.SubViewBlockList[1].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&BelongYear=" + belongYear + "&BelongMonth=" + belongMonth;
            ContractBlock.SubViewBlockList[2].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&BelongYear=" + lastYear + "&BelongMonth=" + lastMonth;
            ViewBag.ContractBlock = ContractBlock;

            sql = @"select Sum(DataValue) as DataValue,BelongYear,BelongQuarter,BelongMonth
from (select ID,DataValue,Year(FinishDate) as BelongYear,(Month(FinishDate)-1)/3+1 as BelongQuarter,Month(FinishDate) as BelongMonth
 from S_I_Engineering
 left join  (select Sum(ContractRMBValue) as DataValue ,ProjectInfo from dbo.S_M_ContractInfo
 group by ProjectInfo) ContractInfo
 on S_I_Engineering.ID=ContractInfo.ProjectInfo
 where FinishDate is not null) tableInfo
 group by BelongYear,BelongQuarter,BelongMonth";
            var productionDt = this.SqlHelper.ExecuteDataTable(sql);
            var productionBlock = this._getViewBlock(indicatorDt, productionDt, belongYear, "Production", "收入");
            productionBlock.MainUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProductionQuery&BelongYear=" + belongYear;
            productionBlock.SubViewBlockList[0].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProductionQuery&BelongYear=" + belongYear + "&BelongQuarter=" + DateTimeHelper.GetQuarter(DateTime.Now);
            productionBlock.SubViewBlockList[1].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProductionQuery&BelongYear=" + belongYear + "&BelongMonth=" + belongMonth;
            productionBlock.SubViewBlockList[2].LinkUrl = "/MvcConfig/UI/List/PageView?TmplCode=ProductionQuery&BelongYear=" + lastYear + "&BelongMonth=" + lastMonth;
            ViewBag.ProductionBlock = productionBlock;
            ViewBag.SubViewList = this._getSubViewBlockList().OrderBy(d => d.SortIndex).ToList();
            return View();
        }

        ViewBlock _getViewBlock(DataTable indicatorDt, DataTable dataSource, int belongYear, string kpiClass, string title = "", bool formatCurreny = true)
        {
            var result = new ViewBlock();
            var obj = dataSource.Compute("Sum(DataValue)", " BelongYear = '" + belongYear + "' and BelongMonth<='" + DateTime.Now.Month + "'");
            result.Main = obj != null && obj != DBNull.Value ? Math.Round(Convert.ToDecimal(obj), 2) : 0;
            if (formatCurreny)
            {
                result.Main = Math.Round(result.Main / 10000, 2);
            }
            var lastYear = belongYear - 1;
            obj = dataSource.Compute("Sum(DataValue)", " BelongYear = '" + lastYear + "' and BelongMonth<='" + DateTime.Now.Month + "'");
            var lastYearValue = obj != null && obj != DBNull.Value ? Convert.ToDecimal(obj) : 0;
            if (formatCurreny)
            {
                lastYearValue = Math.Round(lastYearValue / 10000, 2);
            }
            result.SubAreaTip = "去年同期：{0}；同比{1}：{2}";
            var scale = lastYearValue == 0 ? 100 : Math.Round((result.Main - lastYearValue) / lastYearValue * 100, 0);
            result.Sub = Math.Round(scale);
            if (result.Sub > 0)
            {
                result.SubAreaTip = String.Format(result.SubAreaTip, lastYearValue, "上升", Math.Abs(scale) + "%");
            }
            else
            {
                result.SubAreaTip = String.Format(result.SubAreaTip, lastYearValue, "下降", Math.Abs(scale) + "%");
            }
            var kpiRows = indicatorDt.Select("KPIType='" + kpiClass + "'");
            if (kpiRows.Length > 0)
            {
                result.SubRight = kpiRows[0]["KPIValue"] == null || kpiRows[0]["KPIValue"] == DBNull.Value ? 0 :
                Math.Round(Convert.ToDecimal(kpiRows[0]["KPIValue"]), 0);
            }
            else
            {
                result.SubRight = 0;
            }
            if (formatCurreny)
            {
                result.SubRight = result.SubRight / 10000;
            }

            result.progressMain = result.SubRight == 0 ? 0 : Math.Round(result.Main / result.SubRight * 100);
            result.progressSub = Math.Round(Convert.ToDecimal(DateTime.Now.DayOfYear) / 365 * 100);
            obj = dataSource.Compute("Sum(DataValue)", " BelongYear = '" + belongYear + "' and BelongQuarter='" + DateTimeHelper.GetQuarter(DateTime.Now) + "'");
            var subValue = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj), 2);
            if (formatCurreny) subValue = Math.Round(subValue / 10000, 2);
            var quater = this._createSubViewBlock(subValue.ToString(), "本季度" + title, "万元", 100);
            result.SubViewBlockList.Add(this._createSubViewBlock(subValue.ToString(), "本季度" + title, "万元", 100));

            obj = dataSource.Compute("Sum(DataValue)", " BelongYear = '" + belongYear + "' and BelongMonth='" + DateTime.Now.Month + "'");
            subValue = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj), 2);
            if (formatCurreny) subValue = Math.Round(subValue / 10000, 2);
            result.SubViewBlockList.Add(this._createSubViewBlock(subValue.ToString(), "本月" + title, "万元", 100));

            var lastMonth = DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month - 1;
            obj = dataSource.Compute("Sum(DataValue)", " BelongYear = '" + belongYear + "' and BelongMonth='" + lastMonth + "'");
            subValue = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj), 2);
            if (formatCurreny) subValue = Math.Round(subValue / 10000, 2);
            result.SubViewBlockList.Add(this._createSubViewBlock(subValue.ToString(), "上月" + title, "万元", 100));
            return result;
        }

        List<SubViewBlock> _getSubViewBlockList()
        {
            var result = new List<SubViewBlock>();
            string sql = @"select isnull(Sum(isnull(SumInvoiceValue,0)-isnull(SumReceiptValue,0)-isnull(SumBadDebtValue,0)),0) as  DataValue from S_M_ContractInfo where SignDate is not null ";
            var obj = this.SqlHelper.ExecuteScalar(sql);
            var value = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
            result.Add(_createSubViewBlock(value.ToString(), "应收款", "万元", 100, "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&IsSigned=Signed"));

            var nextMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            nextMonthDate = nextMonthDate.AddMonths(2).AddDays(-1);
            sql = @"select Sum(PlanValue) as DataValue from S_M_ContractInfo_ReceiptObj_PlanReceipt where State='UnReceipt' and BelongYear='" + nextMonthDate.Year + "' and BelongMonth='" + nextMonthDate.Month + "'";
            obj = this.SqlHelper.ExecuteScalar(sql);
            value = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
            result.Add(_createSubViewBlock(value.ToString(), "下月计划收款", "万元", 200, "/MvcConfig/UI/List/PageView?TmplCode=PlanReceiptQuery&BelongYear=" + nextMonthDate.Year + "&BelongMonth=" + nextMonthDate.Month));

            sql = @"select isnull(Sum(ContractRMBValue-SumReceiptValue-SumBadDebtValue),0) as  DataValue from S_M_ContractInfo where SignDate is not null ";
            obj = this.SqlHelper.ExecuteScalar(sql);
            value = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
            result.Add(_createSubViewBlock(value.ToString(), "当前剩余合同额", "万元", 300, "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&IsSigned=Signed&HasRemainContract=true"));

            sql = @"select isnull(Sum(ContractRMBValue),0) as DataValue from S_M_ContractInfo where SignDate  is null ";
            obj = this.SqlHelper.ExecuteScalar(sql);
            value = obj == null || obj == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(obj) / 10000, 2);
            result.Add(_createSubViewBlock(value.ToString(), "待签合同额", "万元", 400, "/MvcConfig/UI/List/PageView?TmplCode=ContractInfoView&IsSigned=NotSigned"));
            return result;
        }

        SubViewBlock _createSubViewBlock(string mainValue, string title, string Unit, int sortIndex, string linkUrl = "")
        {
            var result = new SubViewBlock();
            result.MainValue = mainValue;
            result.SortIndex = sortIndex;
            result.Title = title;
            result.Unit = Unit;
            result.LinkUrl = linkUrl;
            return result;
        }

        List<ReportMenu> GetMenuList()
        {
            return null;
        }
    }

    public class ViewBlock
    {
        public decimal Main { get; set; }

        public string MainUrl { get; set; }

        public decimal Sub { get; set; }

        public string SubAreaTip { get; set; }

        public decimal SubRight { get; set; }

        public decimal progressMain { get; set; }

        public decimal progressSub { get; set; }

        List<SubViewBlock> _SubViewBlockList;
        public List<SubViewBlock> SubViewBlockList
        {
            get
            {
                if (_SubViewBlockList == null)
                    _SubViewBlockList = new List<SubViewBlock>();
                return _SubViewBlockList;
            }
        }
    }

    public class SubViewBlock
    {

        public string MainValue { get; set; }

        public string Title { get; set; }

        public string Unit { get; set; }

        public int SortIndex { get; set; }

        public string LinkUrl { get; set; }
    }

    public class ReportMenu
    {
        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string Img { get; set; }
    }
}
