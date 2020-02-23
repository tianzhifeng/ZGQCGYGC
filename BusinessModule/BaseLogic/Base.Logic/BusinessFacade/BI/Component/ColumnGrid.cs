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
using System.Text.RegularExpressions;


namespace Base.Logic
{
    public class ColumnGrid : BaseComponent
    {
        public ColumnGrid(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            var enumSev = Formula.FormulaHelper.GetService<IEnumService>();
            this.FillDataSource(parameters);
            var result = this.BlockDef;
            result.SetValue("ID", this.ID);
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
            result.SetValue("showSummaryRow", this.BlockDef.GetValue("showSummaryRow"));
            if (String.IsNullOrEmpty(result.GetValue("chartHeight")))
                result.SetValue("chartHeight", "170px");
            this.setStyle(result);

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

            var dataSourceDefList = JsonHelper.ToList(this.BlockDef.GetValue("dataSource"));

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
            dataSource = this.DataSource.GetValue<DataTable>(dataSourceName.Trim());
            if (dataSource == null) throw new Exception("没有找到指定的数据源");
            var titleDic = new Dictionary<string, object>();
            titleDic.SetValue("Text", "");
            chartData.SetValue("title", titleDic);

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
            //判定是否是固定的自定义X轴
            if (this.BlockDef.GetValue("diyCategory").Trim() == true.ToString().ToLower())
            {
                //根据枚举来固定X轴
                if (this.BlockDef.GetValue("CategorySource") == "CategoryEnum")
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
                }
                else if (this.BlockDef.GetValue("CategorySource") == "Categories") //根据自定义的数组绘制X轴
                {
                    var Categories = this.BlockDef.GetValue("Categories").Replace("，", ",").Split(',');
                    foreach (var category in Categories)
                    {
                        if (String.IsNullOrEmpty(category)) continue;
                        xAxisData.Add(category);
                    }
                }
                else
                {
                    //否则根据数据源定义的列来绘制X轴
                    var dsDefine = dataSourceDefList.FirstOrDefault(c => c.GetValue("Code") == dataSourceName);
                    if (dsDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + dataSourceName + "】的数据源，无法根据数据源列定义绘制列表");
                    var colDefines = JsonHelper.ToList(dsDefine.GetValue("columnDefine"));
                    foreach (var item in colDefines)
                    {
                        if (item.GetValue("ShowInX") == true.ToString().ToLower())
                            xAxisData.Add(item.GetValue("Title"));
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

            #region 图表数据
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
                        var data = new ArrayList();
                        foreach (var xAxisValue in xAxisData)
                        {
                            var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "'" + " and " + whereStr);
                            if (dataRows.Length == 0)
                            {
                                data.Add(0);
                            }
                            else
                            {
                                var value = dataRows.FirstOrDefault()[valueField] == null || dataRows.FirstOrDefault()[valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows.FirstOrDefault()[valueField]);
                                data.Add(value);
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
                            var data = new ArrayList();
                            var rows = dataSource.Select(groupField + "='" + groupItem.Value + "'");
                            if (rows.Length == 0)
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    data.Add(0);
                                }
                            }
                            else
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "' and " + groupField + "='" + groupItem.Value + "'" + " and " + whereStr);
                                    if (dataRows.Length == 0)
                                    {
                                        data.Add(0);
                                    }
                                    else
                                    {
                                        var value = dataRows[0][valueField] == null || dataRows[0][valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows[0][valueField]);
                                        data.Add(value);
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
                        var data = new ArrayList();
                        foreach (var xAxisValue in xAxisData)
                        {
                            var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "'");
                            if (dataRows.Length == 0)
                            {
                                data.Add(0);
                            }
                            else
                            {
                                var value = dataRows.FirstOrDefault()[valueField] == null || dataRows.FirstOrDefault()[valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows.FirstOrDefault()[valueField]);
                                data.Add(value);
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
                            var data = new ArrayList();
                            var rows = dataSource.Select(groupField + "='" + groupItem.Value + "'");
                            if (rows.Length == 0)
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    data.Add(0);
                                }
                            }
                            else
                            {
                                foreach (var xAxisValue in xAxisData)
                                {
                                    var dataRows = dataSource.Select(categoryField + "='" + xAxisValue + "' and " + groupField + "='" + groupItem.Value + "'");
                                    if (dataRows.Length == 0)
                                    {
                                        data.Add(0);
                                    }
                                    else
                                    {
                                        var value = dataRows[0][valueField] == null || dataRows[0][valueField] == DBNull.Value ? 0m : Convert.ToDecimal(dataRows[0][valueField]);
                                        data.Add(value);
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

            #region 设置列表JSON
            var resultDt = new DataTable();

            var columnDefines = JsonHelper.ToList(this.BlockDef.GetValue("ColumnDefine"));
            #region 列定义
            if (this.BlockDef.GetValue("gridDynColumn") == true.ToString().ToLower())
            {
                if (this.BlockDef.GetValue("independentDataSource").ToLower() == true.ToString().ToLower())
                {
                    dataSourceName = this.BlockDef.GetValue("independentDataSourceName");
                }
                var dsDefine = dataSourceDefList.FirstOrDefault(c => c.GetValue("Code") == dataSourceName);
                if (dsDefine == null) throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + dataSourceName + "】的数据源，无法根据数据源列定义绘制列表");
                columnDefines = JsonHelper.ToList(dsDefine.GetValue("columnDefine"));
            }
            foreach (var item in columnDefines)
            {
                if (String.IsNullOrEmpty(item.GetValue("Width")))
                {
                    item.SetValue("Width", Math.Round(100m / columnDefines.Count, 2) + "%");
                }
                else if (item.GetValue("Width").ToLower().Trim() == "auto" || item.GetValue("Width").ToLower().Trim() == "*")
                {

                }
                else if (!item.GetValue("Width").ToLower().Trim().EndsWith("px") && !item.GetValue("Width").ToLower().Trim().EndsWith("%"))
                {
                    item.SetValue("Width", item.GetValue("Width") + "px");
                }
                if (String.IsNullOrEmpty(item.GetValue("Align")))
                {
                    item.SetValue("Align", "center");
                }
                resultDt.Columns.Add(item.GetValue("Field"));
            }
            this.BlockDef.SetValue("ColumnDefine", columnDefines);
            #endregion

            var autoSumRow = false; var sumType = "Sum";

            //判定是否是独立数据源，如果是独立数据源，则不能与图表同步进行数据源切换
            if (this.BlockDef.GetValue("independentDataSource").ToLower() == true.ToString().ToLower())
            {
                //独立数据源，必须指定数据源名称
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("independentDataSourceName")))
                {
                    dataSourceName = this.BlockDef.GetValue("independentDataSourceName");
                    var dsDefine = dataSourceDefList.FirstOrDefault(c => c.GetValue("Code") == dataSourceName);
                    var gridDataSource = this.DataSource.GetValue<DataTable>(dataSourceName);
                    if (gridDataSource != null && dsDefine != null)
                    {
                        if (dsDefine.GetValue("AutoSumRow").Trim().ToLower() == true.ToString().ToLower()
                            && !String.IsNullOrEmpty(dsDefine.GetValue("SumType").Trim()))
                        {
                            autoSumRow = true;
                            sumType = dsDefine.GetValue("SumType").Trim();
                        }
                        resultDt = getGridTable(gridDataSource, autoSumRow, sumType);
                    }
                }
            }
            else
            {
                var dsDefine = dataSourceDefList.FirstOrDefault(c => c.GetValue("Code").Trim() == dataSourceName.Trim());
                if (dsDefine != null && dsDefine.GetValue("AutoSumRow").Trim().ToLower() == true.ToString().ToLower()
                    && !String.IsNullOrEmpty(dsDefine.GetValue("SumType").Trim()))
                {
                    autoSumRow = true;
                    sumType = dsDefine.GetValue("SumType").Trim();
                }
                resultDt = getGridTable(dataSource, autoSumRow, sumType);
            }
            result.SetValue("gridHeight", String.IsNullOrEmpty(this.BlockDef.GetValue("gridHeight")) ? "100%" : this.BlockDef.GetValue("gridHeight"));
            var columnSumData = getColumnSumTable(resultDt, columnDefines);
            var gridData = new DataTable();
            result.SetValue("GridData", gridData);
            result.SetValue("ColumnSumData", columnSumData);
            result.SetValue("GridID", this.ID + "_DataGrid");
            #endregion

            return result;
        }

        private DataTable getGridTable(DataTable gridDataSource, bool autoSumRow = false, string sumType = "Sum")
        {
            var resultDt = new DataTable();
            if (!String.IsNullOrEmpty(this.BlockDef.GetValue("rowGroupField")) && !String.IsNullOrEmpty(this.BlockDef.GetValue("columnGroupField"))
                            && !String.IsNullOrEmpty(this.BlockDef.GetValue("groupValueField")))
            {
                var rowGroupField = this.BlockDef.GetValue("rowGroupField");
                var columnGroupField = this.BlockDef.GetValue("columnGroupField");
                var groupValueField = this.BlockDef.GetValue("groupValueField");
                if (!resultDt.Columns.Contains(rowGroupField))
                {
                    resultDt.Columns.Add(rowGroupField);
                }
                var rowGouprInfo = gridDataSource.AsEnumerable().Select(c => new { Value = c.Field<object>(rowGroupField) }).Where(c => c.Value != null && c.Value != DBNull.Value && !String.IsNullOrEmpty(c.Value.ToString())).OrderBy(c => c.Value).Distinct().ToList();
                var columnGouprInfo = gridDataSource.AsEnumerable().Select(c => new { Value = c.Field<object>(columnGroupField) }).Where(c => c.Value != null && c.Value != DBNull.Value && !String.IsNullOrEmpty(c.Value.ToString())).OrderBy(c => c.Value).Distinct().ToList();
                foreach (var columnGroupValue in columnGouprInfo)
                {
                    if (!resultDt.Columns.Contains(columnGroupValue.Value.ToString()))
                    {
                        resultDt.Columns.Add(columnGroupValue.Value.ToString());
                    }
                }
                foreach (var rowGroupValue in rowGouprInfo)
                {
                    var row = resultDt.NewRow();
                    row[rowGroupField] = rowGroupValue.Value;
                    foreach (var columnGroupValue in columnGouprInfo)
                    {
                        var value = gridDataSource.Compute("Sum(" + groupValueField + ")", rowGroupField + " ='" + rowGroupValue.Value.ToString()
                            + "' and " + columnGroupField + "='" + columnGroupValue.Value.ToString() + "'");
                        row[columnGroupValue.Value.ToString()] = value;
                    }
                    if (autoSumRow)
                    {
                        if (!String.IsNullOrEmpty(sumType))
                        {
                            var rowSum = gridDataSource.Compute(sumType + "(" + groupValueField + ")", rowGroupField + " ='" + rowGroupValue.Value.ToString() + "'");
                            if (!resultDt.Columns.Contains("RowTotal"))
                            {
                                resultDt.Columns.Add("RowTotal");
                            }
                            row["RowTotal"] = rowSum;
                        }
                    }
                    resultDt.Rows.Add(row);
                }
            }
            else
            {
                resultDt = gridDataSource;
            }
            return resultDt;
        }

        private DataTable getColumnSumTable(DataTable gridDataSource, List<Dictionary<string, object>> columnDefines)
        {
            var resultDt = gridDataSource.Clone();
            if (columnDefines.Any(a => !string.IsNullOrEmpty(a.GetValue("SummaryType"))))
            {
                var row = resultDt.NewRow();
                foreach (var define in columnDefines.Where(a => !string.IsNullOrEmpty(a.GetValue("SummaryType"))))
                {
                    //Sum，Max，Min，Avg
                    var summaryType = define.GetValue("SummaryType");
                    var field = define.GetValue("Field");
                    if (string.IsNullOrEmpty(field)) continue;
                    if (!gridDataSource.Columns.Contains(field)) continue;
                    decimal totalValue = 0; var first = true;
                    foreach (DataRow item in gridDataSource.Rows)
                    {
                        if (item[field] == DBNull.Value || item[field] == null || string.IsNullOrEmpty(item[field].ToString())) continue;
                        decimal rowValue = 0;
                        var error = decimal.TryParse(item[field].ToString(), out rowValue);
                        if (!error) throw new Formula.Exceptions.BusinessException("列【" + field + "】的值【" + item[field].ToString() + "】无法转为数值，汇总失败");
                        if (summaryType.ToLower() == "sum")
                            totalValue += rowValue;
                        else if (summaryType.ToLower() == "avg")
                            totalValue += rowValue;
                        else if (summaryType.ToLower() == "max")
                        {
                            if (first)
                                totalValue = rowValue;
                            else if (rowValue > totalValue)
                                totalValue = rowValue;
                        }
                        else if (summaryType.ToLower() == "min")
                        {
                            if (first)
                                totalValue = rowValue;
                            else if (rowValue < totalValue)
                                totalValue = rowValue;
                        }
                        first = false;
                    }
                    if (summaryType.ToLower() == "avg" && gridDataSource.Rows.Count > 0)
                        totalValue = totalValue / Convert.ToDecimal(gridDataSource.Rows.Count);
                    row[field] = totalValue;
                }
                resultDt.Rows.Add(row);
            }
            return resultDt;
        }
    }
}
