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
using System.Collections;
using Formula;

namespace Base.Logic
{
    public class StandardColumn : BaseComponent
    {
        public StandardColumn(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            var enumSev = Formula.FormulaHelper.GetService<IEnumService>();
            this.FillDataSource(parameters);
            var result = this.BlockDef;
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
            if (String.IsNullOrEmpty(result.GetValue("chartHeight")))
                result.SetValue("chartHeight", "170px");
            this.setStyle(result);

            #region 设置图表JSON
            var chartData = new Dictionary<string, object>();
            var dataSourceName = String.IsNullOrEmpty(HttpContext.Current.Request["DataSourceName"]) ? this.BlockDef.GetValue("DefaultDataSource") : HttpContext.Current.Request["DataSourceName"];
            if (!string.IsNullOrEmpty(parameters))
            {
                var paras = JsonHelper.ToObject<Dictionary<string, string>>(parameters);
                var ds = paras.GetValue("DataSourceName");
                if (!string.IsNullOrEmpty(ds))
                    dataSourceName = ds;
            }
            DataTable dataSource = new DataTable();
            dataSource = this.DataSource.GetValue<DataTable>(dataSourceName);
            if (dataSource == null) throw new Exception("没有找到指定的数据源");
            var titleDic = new Dictionary<string, object>();
            titleDic.SetValue("Text", "");
            chartData.SetValue("title", titleDic);
            chartData.SetValue("DataSource", JsonHelper.ToJson(dataSource));

            if (this.BlockDef.GetValue("AllowChangeDataSource").ToLower().Trim() == true.ToString().ToLower())
            {
                result.SetValue("defaultDataSource", dataSourceName);
                var enumData = new List<Dictionary<string, object>>();
                var dataSourceList = JsonHelper.ToList(this.BlockDef.GetValue("dataSource"));
                foreach (var item in dataSourceList)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("text", item.GetValue("Name"));
                    dic.SetValue("value", item.GetValue("Code"));
                    enumData.Add(dic);
                }
                result.SetValue("defaultDataSource", dataSourceName);
                result.SetValue("dataSourceEnum", enumData);
            }

            #region 设置X轴
            var categoryField = this.BlockDef.GetValue("categoryField");
            var xAxisList = new List<Dictionary<string, object>>();
            var xAxis = new Dictionary<string, object>();
            var xAxisData = new ArrayList();
            if (this.BlockDef.GetValue("diyCategory").Trim() == true.ToString().ToLower())
            {
                var categoryEnumKey = this.BlockDef.GetValue("categoryEnumKey");
                if (!String.IsNullOrEmpty(categoryEnumKey))
                {
                    var enumDef = EnumBaseHelper.GetEnumDef(categoryEnumKey);
                    if (enumDef == null)
                        throw new Formula.Exceptions.BusinessValidationException("没有找到指定的枚举【" + categoryEnumKey + "】");
                    foreach (var enumItem in enumDef.EnumItem)
                    {
                        if (String.IsNullOrEmpty(enumItem.Name)) continue;
                        xAxisData.Add(enumItem.Name);
                    }
                }
                else
                {
                    var Categories = this.BlockDef.GetValue("Categories").Replace("，", ",").Split(',');
                    foreach (var category in Categories)
                    {
                        if (String.IsNullOrEmpty(category)) continue;
                        xAxisData.Add(category);
                    }
                }
                xAxis.SetValue("categories", xAxisData);
                xAxis.SetValue("crosshair", true);
            }
            else
            {
                if (!dataSource.Columns.Contains(categoryField))
                    throw new Formula.Exceptions.BusinessValidationException("没有找到categoryField 字段【" + categoryField + "】");
                var categoryEnumKey = this.BlockDef.GetValue("categoryEnumKey");
                var list = dataSource.AsEnumerable().Select(p => new { Value = p.Field<object>(categoryField) }).Distinct().ToList();
                if (!String.IsNullOrEmpty(categoryEnumKey))
                {
                    foreach (var category in list)
                    {
                        xAxisData.Add(enumSev.GetEnumText(categoryEnumKey, category.Value.ToString()));
                    }
                }
                else
                {
                    foreach (var category in list)
                    {
                        xAxisData.Add(category.Value);
                    }
                }
                xAxis.SetValue("categories", xAxisData);
                xAxis.SetValue("crosshair", true);
            }
            xAxisList.Add(xAxis);
            chartData.SetValue("xAxis", xAxisList);
            #endregion

            #region 设置Y轴
            var yAxisList = new List<Dictionary<string, object>>();
            var yAxisDefine = JsonHelper.ToList(this.BlockDef.GetValue("yAxis"));

            var length = yAxisDefine.Count > 2 ? 2 : yAxisDefine.Count;
            for (int i = 0; i < length; i++)
            {
                var item = yAxisDefine[i];
                var yAxis = new Dictionary<string, object>();
                var lables = new Dictionary<string, object>();
                var style = new Dictionary<string, object>();
                style.SetValue("color", "Highcharts.getOptions().colors[1]");
                lables.SetValue("format", "{value}" + item.GetValue("Unit"));
                lables.SetValue("style", style);

                var title = new Dictionary<string, object>();
                title.SetValue("text", item.GetValue("Title"));
                title.SetValue("style", style);
                yAxis.SetValue("labels", lables);
                yAxis.SetValue("title", title);
                if (i != 0)
                {
                    yAxis.SetValue("opposite", true);
                }
                yAxisList.Add(yAxis);
            }
            chartData.SetValue("yAxis", yAxisList);
            #endregion

            #region 图例设置
            if (this.BlockDef.GetValue("showInLegend").Trim().ToLower() == true.ToString().ToLower())
            {
                var legend = new Dictionary<string, object>();
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("layout")))
                    legend.SetValue("layout", this.BlockDef.GetValue("layout"));
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("align")))
                    legend.SetValue("align", this.BlockDef.GetValue("align"));
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("verticalAlign")))
                    legend.SetValue("verticalAlign", this.BlockDef.GetValue("verticalAlign"));
                chartData.SetValue("legend", legend);
            }
            else
            {
                var legend = new Dictionary<string, object>();
                legend.SetValue("enabled", false);
                chartData.SetValue("legend", legend);
            }
            #endregion

            #region 数据
            var seriesList = new List<Dictionary<string, object>>();
            var graphicDefine = JsonHelper.ToList(this.BlockDef.GetValue("Series"));
            foreach (var item in graphicDefine)
            {
                if (!String.IsNullOrEmpty(item.GetValue("whereString")))
                {
                    string whereStr = this.fo.ReplaceString(item.GetValue("whereString"));
                    var groupField = item.GetValue("groupField");
                    var valueField = item.GetValue("valueField");
                    if (String.IsNullOrEmpty(groupField))
                    {
                        #region
                        var series = new Dictionary<string, object>();
                        series.SetValue("name", item.GetValue("name"));
                        var data = new List<Dictionary<string, object>>();
                        foreach (var xAxisValue in xAxisData)
                        {
                            var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "'" + " and " + whereStr);
                            if (dataRows.Length == 0)
                            {
                                var dic = new Dictionary<string, object>();
                                dic.SetValue("y", 0);
                                data.Add(dic);
                            }
                            else
                            {
                                var value = dataRows.FirstOrDefault()[valueField] == null || dataRows.FirstOrDefault()[valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows.FirstOrDefault()[valueField]);
                                var dic = Formula.FormulaHelper.DataRowToDic(dataRows.FirstOrDefault());
                                dic.SetValue("y", value);
                                data.Add(dic);
                            }
                        }
                        series.SetValue("data", data);
                        series.SetValue("type", item.GetValue("type"));
                        if (!String.IsNullOrEmpty(item.GetValue("yAxis")))
                        {
                            series.SetValue("yAxis", Convert.ToInt32(item.GetValue("yAxis")));
                        }
                        if (!String.IsNullOrEmpty(item.GetValue("Unit")))
                        {
                            var tooltip = new Dictionary<string, object>();
                            tooltip.SetValue("valueSuffix", item.GetValue("valueSuffix"));
                            series.SetValue("tooltip", tooltip);
                        }
                        seriesList.Add(series);
                        #endregion
                    }
                    else
                    {
                        #region
                        var groupData = dataSource.Select(whereStr).AsEnumerable().Select(p => new { Value = p.Field<object>(groupField) }).Distinct().ToList();
                        foreach (var groupItem in groupData)
                        {
                            var series = new Dictionary<string, object>();
                            series.SetValue("name", groupItem.Value);
                            var data = new List<Dictionary<string, object>>();
                            var rows = dataSource.Select(groupField + "='" + groupItem.Value + "'");
                            if (rows.Length == 0)
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dic = new Dictionary<string, object>();
                                    dic.SetValue("y", 0);
                                    data.Add(dic);
                                }
                            }
                            else
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "' and " + groupField + "='" + groupItem.Value + "'" + " and " + whereStr);
                                    if (dataRows.Length == 0)
                                    {
                                        var dic = new Dictionary<string, object>();
                                        dic.SetValue("y", 0);
                                        data.Add(dic);
                                    }
                                    else
                                    {
                                        var value = dataRows[0][valueField] == null || dataRows[0][valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows[0][valueField]);
                                        var dic = Formula.FormulaHelper.DataRowToDic(dataRows.FirstOrDefault());
                                        dic.SetValue("y", value);
                                        data.Add(dic);
                                    }
                                }
                            }
                            series.SetValue("data", data);
                            series.SetValue("type", item.GetValue("type"));
                            if (!String.IsNullOrEmpty(item.GetValue("yAxis")))
                            {
                                series.SetValue("yAxis", Convert.ToInt32(item.GetValue("yAxis")));
                            }
                            if (!String.IsNullOrEmpty(item.GetValue("Unit")))
                            {
                                var tooltip = new Dictionary<string, object>();
                                tooltip.SetValue("valueSuffix", item.GetValue("valueSuffix"));
                                series.SetValue("tooltip", tooltip);
                            }
                            seriesList.Add(series);
                        }
                        #endregion
                    }
                }
                else
                {
                    var groupField = item.GetValue("groupField");
                    var valueField = item.GetValue("valueField");
                    if (String.IsNullOrEmpty(groupField))
                    {
                        #region
                        var series = new Dictionary<string, object>();
                        series.SetValue("name", item.GetValue("name"));
                        var data = new List<Dictionary<string, object>>();
                        foreach (var xAxisValue in xAxisData)
                        {
                            var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "'");
                            if (dataRows.Length == 0)
                            {
                                var dic = new Dictionary<string, object>();
                                dic.SetValue("y", "0");
                                data.Add(dic);
                            }
                            else
                            {
                                var value = dataRows.FirstOrDefault()[valueField] == null || dataRows.FirstOrDefault()[valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows.FirstOrDefault()[valueField]);
                                var dic = Formula.FormulaHelper.DataRowToDic(dataRows.FirstOrDefault());
                                dic.SetValue("y", value);
                                data.Add(dic);
                            }
                        }
                        series.SetValue("data", data);
                        series.SetValue("type", item.GetValue("type"));
                        if (!String.IsNullOrEmpty(item.GetValue("yAxis")))
                        {
                            series.SetValue("yAxis", Convert.ToInt32(item.GetValue("yAxis")));
                        }
                        if (!String.IsNullOrEmpty(item.GetValue("Unit")))
                        {
                            var tooltip = new Dictionary<string, object>();
                            tooltip.SetValue("valueSuffix", item.GetValue("valueSuffix"));
                            series.SetValue("tooltip", tooltip);
                        }
                        seriesList.Add(series);
                        #endregion
                    }
                    else
                    {
                        #region
                        var groupData = dataSource.AsEnumerable().Select(p => new { Value = p.Field<object>(groupField) }).Distinct().ToList();
                        foreach (var groupItem in groupData)
                        {
                            var series = new Dictionary<string, object>();
                            series.SetValue("name", groupItem.Value);
                            var data = new List<Dictionary<string, object>>();
                            var rows = dataSource.Select(groupField + "='" + groupItem.Value + "'");
                            if (rows.Length == 0)
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dic = new Dictionary<string, object>();
                                    dic.SetValue("y", 0);
                                    data.Add(dic);
                                }
                            }
                            else
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "' and " + groupField + "='" + groupItem.Value + "'");
                                    if (dataRows.Length == 0)
                                    {
                                        var dic = new Dictionary<string, object>();
                                        dic.SetValue("y", 0);
                                        data.Add(dic);
                                    }
                                    else
                                    {
                                        var value = dataRows[0][valueField] == null || dataRows[0][valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows[0][valueField]);
                                        var dic = Formula.FormulaHelper.DataRowToDic(dataRows.FirstOrDefault());
                                        dic.SetValue("y", value);
                                        data.Add(dic);
                                    }
                                }
                            }
                            series.SetValue("data", data);
                            series.SetValue("type", item.GetValue("type"));
                            if (!String.IsNullOrEmpty(item.GetValue("yAxis")))
                            {
                                series.SetValue("yAxis", Convert.ToInt32(item.GetValue("yAxis")));
                            }
                            if (!String.IsNullOrEmpty(item.GetValue("Unit")))
                            {
                                var tooltip = new Dictionary<string, object>();
                                tooltip.SetValue("valueSuffix", item.GetValue("valueSuffix"));
                                series.SetValue("tooltip", tooltip);
                            }
                            seriesList.Add(series);
                        }
                        #endregion
                    }
                }
            }
            #endregion
            chartData.SetValue("series", seriesList);
            result.SetValue("chartData", chartData);
            result.SetValue("chartId", this.ID + "_ColumnChart");
            #endregion
            return result;
        }
    }
}
