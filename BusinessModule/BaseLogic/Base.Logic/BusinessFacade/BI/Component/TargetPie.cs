using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Config;
using Config.Logic;
using Formula.Helper;
using Base.Logic.BusinessFacade;
using System.Text.RegularExpressions;
using System.Collections;
using Formula;

namespace Base.Logic
{
    public class TargetPie : BaseComponent
    {
        public TargetPie(string BlockDefJson)
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
                result.SetValue("chartHeight", "120px");
            var dataSourceName = this.BlockDef.GetValue("InUserDataSource");
            var targetSourceName = this.BlockDef.GetValue("TargetDataSource");
            var dataSource = this.DataSource.GetValue<DataTable>(dataSourceName);
            var targetDataSource = this.DataSource.GetValue<DataTable>(targetSourceName);
            var fieldName = this.BlockDef.GetValue("FieldName").Trim();
            var targetFieldName = this.BlockDef.GetValue("TargetFieldName").Trim();
            var chartUnit = this.BlockDef.GetValue("chartUnit");
            var value = 0m; var targetValue = 0m;
            if (!dataSource.Columns.Contains(fieldName))
                throw new Exception("数据源中没有【" + fieldName + "】字段");
            if (!targetDataSource.Columns.Contains(targetFieldName))
                throw new Exception("目标数据源中没有【" + targetFieldName + "】字段");
            if (dataSource.Rows.Count > 0)
            {
                value = dataSource.Rows[0][fieldName] == null || dataSource.Rows[0][fieldName] == DBNull.Value ? 0m :
                    Convert.ToDecimal(dataSource.Rows[0][fieldName]);
            }
            if (targetDataSource != null && targetDataSource.Columns.Contains(targetFieldName) && targetDataSource.Rows.Count > 0)
            {
                targetValue = targetDataSource.Rows[0][targetFieldName] == null || targetDataSource.Rows[0][targetFieldName] == DBNull.Value ? 0m :
                    Convert.ToDecimal(targetDataSource.Rows[0][targetFieldName]);
            }
            if (targetValue <= 0) targetValue = value + 100;
            result.SetValue("ActualValue", value);
            result.SetValue("ID", this.ID);
            result.SetValue("TargetValue", targetValue);
            var Rate = targetValue == 0 ? 0 : Math.Round(value / targetValue * 100);
            result.SetValue("CompletionAmount", Rate);
            var colorClass = "color4";
            if (Rate > 0 && Rate < 25)
                colorClass = "color1";
            else if (Rate >= 25 && Rate < 50)
                colorClass = "color2";
            else if (Rate >= 50 && Rate < 75)
                colorClass = "color3";

            result.SetValue("ProgressColor", colorClass);

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

            var content = this.BlockDef.GetValue("Content");
            var rowItem = new Dictionary<string, object>();
            Regex reg = new Regex(@"(?<=\{)[^}]*(?=\})");
            foreach (Match m in reg.Matches(content))
            {
                string key = m.Value.Trim('{', '}');
                if (dataSource.Rows.Count > 0 && dataSource.Columns.Contains(key))
                {
                    rowItem.Add(key, dataSource.Rows[0][key]);
                }
            }
            result.SetValue("Content", this.ReplaceRegString(content, rowItem, null));

            this.setStyle(result);

            #region 设置图表JSON
            var chartData = new Dictionary<string, object>();
            var seriesList = new List<Dictionary<string, object>>();
            var series = new Dictionary<string, object>();
            var yAxis = new Dictionary<string, object>();
            yAxis.SetValue("min", 0);
            yAxis.SetValue("max", targetValue);
            var yAxisTitle = new Dictionary<string, object>();
            yAxisTitle.SetValue("y", 15);
            yAxisTitle.SetValue("text", result.GetValue("Content"));
            yAxis.SetValue("title", yAxisTitle);
            chartData.SetValue("yAxis", yAxis);

            var dataArray = new ArrayList();
            dataArray.Add(value);
            //var titleDic = new Dictionary<string, object>();
            //titleDic.SetValue("text", Rate + "%");
            series.SetValue("data", dataArray);
            var dataLabels = new Dictionary<string, object>();
            dataLabels.SetValue("inside", "true");
            dataLabels.SetValue("format", "<div style=\"text-align:top\"><span style=\"font-family:Microsoft YaHei;font-size:25px;color:black\">{y}</span><span style=\"font-size:12px;color:silver\">" + chartUnit + "</span></div>");
            series.SetValue("dataLabels", dataLabels);
            seriesList.Add(series);
            chartData.SetValue("series", seriesList);
            result.SetValue("chartData", chartData);
            result.SetValue("chartId", this.ID + "_PieChart");
            #endregion

            var tagDef = this.BlockDef.GetValue("tagDefine");
            if (!String.IsNullOrEmpty(tagDef))
            {
                var TagList = new List<Dictionary<string, object>>();
                var tagDefList = JsonHelper.ToList(tagDef);
                int index = 1;
                foreach (var tagDefItem in tagDefList)
                {
                    var tagValueDef = tagDefItem.GetValue("Value").Trim();
                    var tag = new Dictionary<string, object>();
                    tag.SetValue("TargetID", "target_" + index);
                    tag.SetValue("Title", tagDefItem.GetValue("Title").Trim());
                    tag.SetValue("Value", fo.GetDefaultValue(String.Empty, tagValueDef, this.DataSource));
                    tag.SetValue("Unit", tagDefItem.GetValue("Unit").Trim());
                    tag.SetValue("Prefix", tagDefItem.GetValue("Prefix").Trim());
                    tag.SetValue("Image", tagDefItem.GetValue("Image").Trim());
                    tag.SetValue("HasLinkUrl", "0");
                    if (!String.IsNullOrEmpty(tagDefItem.GetValue("LinkUrl").Trim()) || (IsMobile && !String.IsNullOrEmpty(tagDefItem.GetValue("MobileLinkUrl").Trim())))
                    {
                        tag.SetValue("LinkUrl", fo.GetDefaultValue(String.Empty, IsMobile ? tagDefItem.GetValue("MobileLinkUrl").Trim() : tagDefItem.GetValue("LinkUrl").Trim(), this.DataSource));
                        tag.SetValue("HasLinkUrl", "1");
                        tag.SetValue("width", tagDefItem.GetValue("width").Trim());
                        tag.SetValue("height", tagDefItem.GetValue("height").Trim());
                    }
                    TagList.Add(tag);
                    index++;
                }
                result.SetValue("Tag", TagList);
            }
            return result;
        }
    }
}
