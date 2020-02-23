using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Collections;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;


namespace HR.Logic
{
    public class HighChartHelper
    {
        public static PieChart CreatePieChart(string title,string seriesName, DataTable dataSource, string nameField = "nameField", string valueField = "valueField")
        {
            if (!dataSource.Columns.Contains(nameField)) throw new Formula.Exceptions.BusinessException("未找到指定的name字段");
            if (!dataSource.Columns.Contains(valueField)) throw new Formula.Exceptions.BusinessException("未找到指定的data字段");
            var piechart = new PieChart();
            piechart.TitleInfo.Text = title;
            
            var series = new Series();
            series.Name = seriesName;
            series.Type = "pie";
            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                var item = dataSource.Rows[i];
                var data = new Dictionary<string, object>();
                var name = item[nameField].ToString();
                if (String.IsNullOrEmpty(name)) continue;
                data.SetValue("name", name);
                data.SetValue("y", item[valueField]);
                if (i == 0)
                {
                    data.SetValue("sliced", true);
                    data.SetValue("selected", true);
                }
                series.Data.Add(data);
            }
            piechart.SeriesList.Add(series);
            return piechart;
        }


        public static ColumnChart CreateColumnChart(string title, DataTable dataSource, string categoryField, string[] seriesNames, string[] seriesFields)
        {
            foreach (var seriesField in seriesFields)
            {
                if (!dataSource.Columns.Contains(seriesField))
                    throw new Formula.Exceptions.BusinessException("数据源中不包含序列字段【" + seriesField + "】无法生成柱状图");
            }
            if (seriesNames.Length != seriesFields.Length)
                throw new Formula.Exceptions.BusinessException("序列字段数与序列名称数不符，无法生成柱状图");

            var columnChart = new ColumnChart();
            columnChart.TitleInfo.Text = title;
            columnChart.xAxisInfo = new xAxis();
            var categories = string.Empty;
            foreach (DataRow row in dataSource.Rows)
            {
                if (row[categoryField] == null || row[categoryField] == DBNull.Value) continue;
                categories += row[categoryField].ToString() + ",";
            }
            columnChart.xAxisInfo.Categories = categories.TrimEnd(',').Split(',');
            columnChart.yAxisInfo = new yAxis();
            columnChart.yAxisInfo.MiniValue = 0;
            columnChart.yAxisInfo.TitleInfo = new Dictionary<string, object>();
            columnChart.yAxisInfo.TitleInfo.SetValue("text", "");

            for (int i = 0; i < seriesNames.Length; i++)
            {
                var seriesName = seriesNames[i];
                var seriesField = seriesFields[i];
                var series = new Series();
                series.Name = seriesName;
                var data =new ArrayList();
                for (int j = 0; j < columnChart.xAxisInfo.Categories.Length; j++)
                {
                    var category = columnChart.xAxisInfo.Categories[j];
                    var row = dataSource.Select(categoryField + "='" + category + "'").FirstOrDefault();
                    if (row == null) 
                    {
                        data.Add(0); 
                    }
                    else
                    {
                        data.Add(row[seriesField]);
                    }
                }
                series.Data = data;
                columnChart.SeriesList.Add(series);
            }
            return columnChart;
        }

    }
}
