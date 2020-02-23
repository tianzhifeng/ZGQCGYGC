using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using Config;
using Config.Logic;
using Formula.Helper;
using Base.Logic.BusinessFacade;
using System.Web;
using Formula;

namespace Base.Logic
{
    public class StandardPie : BaseComponent
    {
        public StandardPie(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var result = this.BlockDef;
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
            if (String.IsNullOrEmpty(result.GetValue("chartHeight")))
                result.SetValue("chartHeight", "170px");
            this.setStyle(result);

            #region 设置图表JSON
            var chartData = new Dictionary<string, object>();
            var dataSourceName = HttpContext.Current.Request["DataSourceName"];
            DataTable dataSource = new DataTable();
            if (String.IsNullOrEmpty(dataSourceName))
            {
                dataSource = this.DataSource.FirstOrDefault().Value;
            }
            else
            {
                dataSource = this.DataSource.GetValue<DataTable>(dataSourceName);
            }
            if (dataSource == null) throw new Exception("没有找到指定的数据源");
            if (!dataSource.Columns.Contains("name") || !dataSource.Columns.Contains("value"))
            {
                throw new Exception("数据源必须有name 和 value 列");
            }
            var tooltip = new Dictionary<string, object>();
            tooltip.SetValue("headerFormat", "{point.name}<br>");
            tooltip.SetValue("pointFormat", "{point.name}: <b>{point." + this.BlockDef.GetValue("tooltipValue") + ":.1f}" + this.BlockDef.GetValue("tooltipValueUnit") + "</b>");
            chartData.SetValue("tooltip", tooltip);

            var plotOptions = new Dictionary<string, object>();
            var pieOpt = new Dictionary<string, object>();
            var dataLabels = new Dictionary<string, object>();
            dataLabels.SetValue("format", "<b>{point.name}</b>: {point." + this.BlockDef.GetValue("plotValue") + ":.1f} " + this.BlockDef.GetValue("plotValueUnit") + "");
            if (this.BlockDef.GetValue("showInLegend").ToLower() == true.ToString().ToLower())
            {
                dataLabels.SetValue("enabled", false);
                pieOpt.SetValue("showInLegend", true);
                var legend = new Dictionary<string, object>();
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("layout")))
                    legend.SetValue("layout", this.BlockDef.GetValue("layout"));
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("align")))
                    legend.SetValue("align", this.BlockDef.GetValue("align"));
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("verticalAlign")))
                    legend.SetValue("verticalAlign", this.BlockDef.GetValue("verticalAlign"));
                chartData.SetValue("legend", legend);
            }
            pieOpt.SetValue("dataLabels", dataLabels);
            plotOptions.SetValue("pie", pieOpt);
            chartData.SetValue("plotOptions", plotOptions);
            var dataList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dataSource.Rows)
            {
                var dic = Formula.FormulaHelper.DataRowToDic(row);
                dic.SetValue("y", row["value"]);
                dataList.Add(dic);
            }

            #region 设置过滤
            var filters = JsonHelper.ToList(this.BlockDef.GetValue("FilterInfo"));
            foreach (var filter in filters)
            {
                var enumData = filter.GetValue("EnumData");
                if (!String.IsNullOrEmpty(enumData))
                {
                    if (enumData.IndexOf("{") >= 0)
                    {
                        var enumList = JsonHelper.ToList(enumData);
                        foreach (var item in enumList)
                        {
                            if (String.IsNullOrEmpty(item.GetValue("IsDefault")))
                            {
                                item.SetValue("IsDefault", "false");
                            }
                        }
                        filter.SetValue("EnumData", enumList);
                    }
                    else
                    {
                        IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                        var enumList = JsonHelper.ToList(JsonHelper.ToJson(enumService.GetEnumDataSource(enumData.ToString())));
                        foreach (var item in enumList)
                        {
                            item.SetValue("IsDefault", "false");
                        }
                        filter.SetValue("EnumData", enumList);
                    }
                }
            }
            result.SetValue("FilterInfo", filters);
            #endregion

            var seriesList = new List<Dictionary<string, object>>();
            var series = new Dictionary<string, object>();
            series.SetValue("data", dataList);
            seriesList.Add(series);
            result.SetValue("ID", this.ID);
            chartData.SetValue("series", seriesList);
            result.SetValue("chartData", chartData);
            result.SetValue("chartId", this.ID + "_PieChart");
            #endregion
            return result;
        }
    }
}
