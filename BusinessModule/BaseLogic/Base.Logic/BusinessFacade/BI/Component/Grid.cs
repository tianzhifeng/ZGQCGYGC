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
using MvcAdapter;

namespace Base.Logic
{
    public class Grid : BaseComponent
    {
        public Grid(string BlockDefJson)
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

            #region 设置列表JSON
            var resultDt = new DataTable();
            #region 列定义
            var columnDefines = JsonHelper.ToList(this.BlockDef.GetValue("ColumnDefine"));
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
            }
            this.BlockDef.SetValue("ColumnDefine", columnDefines);
            #endregion

            var autoSumRow = false; var sumType = "Sum";
            var dataSourceDefList = JsonHelper.ToList(this.BlockDef.GetValue("dataSource"));
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
            result.SetValue("GridData", new DataTable());
            result.SetValue("ColumnSumData", getColumnSumTable(resultDt, columnDefines));
            result.SetValue("GridID", this.ID + "_DataGrid");
            #endregion

            result.SetValue("chartData", resultDt);

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
                var rowGouprInfo = gridDataSource.AsEnumerable().Select(c => new { Value = c.Field<object>(rowGroupField) }).Where(c => c.Value != null && c.Value != DBNull.Value).OrderBy(c => c.Value).Distinct().ToList();
                var columnGouprInfo = gridDataSource.AsEnumerable().Select(c => new { Value = c.Field<object>(columnGroupField) }).Where(c => c.Value != null && c.Value != DBNull.Value).OrderBy(c => c.Value).Distinct().ToList();
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
