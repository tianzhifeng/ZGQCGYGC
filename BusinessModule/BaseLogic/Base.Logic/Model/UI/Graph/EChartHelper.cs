using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base.Logic.Domain;
using Formula;
using System.Data;
using Formula.Helper;
using Config.Logic;

namespace Base.Logic.Model.UI.Graph
{
    public class EChartHelper
    {
        /// <summary>
        /// 折线图/柱形图创建接口
        /// </summary>
        /// <param name="type">图表类型</param>
        /// <param name="title">图表标题</param>
        /// <param name="subTitle">图表副标题</param>
        /// <param name="legend">图例</param>
        /// <param name="xAxis">X轴字段集合</param>
        /// <param name="yAxis">Y轴值集合</param>
        /// <param name="yAxisIndex">Y轴值序列</param>
        /// <param name="graphType">图表类型</param>
        /// <param name="lineFields">折线图例名数组集合</param>
        /// <param name="lineNames">轴线说明</param>
        /// <param name="startValue">X轴默认第一个值</param>
        /// <param name="direction">是否纵向柱形图
        /// <remarks>true:纵向柱形图；false：横向柱形图</remarks>
        /// </param>
        /// <param name="min">值轴坐标的最小值</param>
        /// <param name="max">值轴坐标的最大值</param>
        /// <param name="onZero">X轴坐标是否从零开始</param>
        /// <returns>折线图/柱形图Json</returns>
        public static string CreateLineBarChartOption(string type, string title, string subTitle, bool isDetail, string legend, string xAxis, Dictionary<string, string> yAxis,
             string graphType, string[] lineFields, string[] lineNames, Dictionary<string, int> yAxisIndex = null, string startValue = "", bool direction = true, string min = "", string max = "", bool onZero = true)
        {
            // 如果图表类型不为柱状图，参数direction设置无效，赋默认值true
            if (graphType.Split('-')[0] != "bar")
                direction = true;

            Dictionary<string, double[]> yAxisDouble = new Dictionary<string, double[]>();
            Dictionary<int, string> lineNameDic = new Dictionary<int, string>();
            foreach (var item in yAxis)
            {
                var itemArr = item.Value.Split(',');
                var doubleArr = new double[itemArr.Count()];
                for (int i = 0; i < itemArr.Count(); i++)
                {
                    doubleArr[i] = Double.Parse(itemArr[i]);
                }
                yAxisDouble.Add(item.Key, doubleArr);
            }
            for (var i = 0; i < lineNames.Length; i++)
            {
                lineNameDic.Add(i, lineNames[i]);
            }

            if (direction)
            {
                #region 纵向柱形图或折线图或折线图/柱形混合图
                var option = new
                {
                    tooltip = new
                    {
                        trigger = "axis",
                        confine = true
                    },
                    title = new
                    {
                        //show = isDetail,
                        text = title,
                        subtext = subTitle,
                        left = isDetail ? "center" : "left",
                        top = isDetail ? 20 : 5,
                        textStyle = new
                        {
                            color = "#ccc",
                            fontSize = isDetail ? 18 : 15,
                        },
                        subtextStyle = new
                        {
                            fontWeight = "lighter",
                            fontSize = isDetail ? 15 : 10,
                        },
                        itemGap = 5
                    },
                    toolbox = new
                    {
                        show = false,
                        right = 30,
                        feature = new
                        {
                            dataZoom = new
                            {
                                yAxisIndex = "none"
                            },
                            restore = new { },
                            saveAsImage = new { }
                        }
                    },
                    grid = new
                    {
                        left = "15%",
                        top = "30%",
                        bottom = "20%"
                    },
                    //dataZoom = new[]{
                    //            new {
                    //                type = "slider",
                    //                startValue = startValue 
                    //            }, 
                    //            new {
                    //                type = "inside",
                    //                startValue = "" 
                    //            }
                    //        },
                    legend = new
                    {
                        show = isDetail,
                        left = "center",
                        top = "8%",
                        textStyle = new
                        {
                            color = "lightgray"
                        },
                        data = legend.Split(',')
                    },
                    xAxis = new
                    {
                        type = "category",
                        boundaryGap = graphType.Split('-')[0] == "bar" ? true : false,
                        alignWithLabel = true,
                        splitLine = new
                        {
                            show = false
                        },
                        axisLine = new
                        {
                            onZero = onZero ? true : false,
                            lineStyle = new { color = "lightgray" }
                        },
                        axisTick = new
                        {
                            lineStyle = new { color = "lightgray" }
                        },
                        axisLabel = new
                        {
                            //interval = 0,
                            textStyle = new { color = "lightgray" }
                        },
                        axisPointer = new
                        {
                            type = "shadow"
                        },
                        data = xAxis.Split(',')
                    },
                    yAxis = from item in lineNameDic
                            select new
                            {
                                type = "value",
                                name = item.Value,
                                splitLine = new
                                {
                                    show = true,
                                    lineStyle = new
                                    {
                                        color = "lightgray",
                                        type = "dotted"
                                    }
                                },
                                axisLine = new
                                {
                                    show = false,
                                    lineStyle = new { color = "lightgray" }
                                },
                                axisTick = new
                                {
                                    show = false,
                                    lineStyle = new { color = "lightgray" }
                                },
                                axisLabel = new
                                {
                                    textStyle = new { color = "lightgray" }
                                }
                                //,min = min
                            },
                    series = from item in yAxisDouble
                             select new
                             {
                                 name = item.Key,
                                 type = lineFields.Any(d => d == item.Key) && !string.IsNullOrEmpty(item.Key) ? "line" : graphType,
                                 yAxisIndex = yAxisIndex == null ? 0 : yAxisIndex.GetValue(item.Key),
                                 smooth = true,
                                 data = item.Value
                             },
                    color = new string[] { "#ef8691", "#f2b33f", "#e6f490", "#a2f0bc", "#8ad1ef", "#3fc8f2", "#90abf3", "#c9a9db" }
                };

                var jsonStr = JsonHelper.ToJson<object>(option);
                string minOrMax = "";
                if (!string.IsNullOrEmpty(min))
                    minOrMax += "\"min\":" + min + ",";
                if (!string.IsNullOrEmpty(max))
                    minOrMax += "\"max\":" + max + ",";
                jsonStr = jsonStr.Replace("\"yAxis\":{", "\"yAxis\":{" + minOrMax);
                return jsonStr;

                #endregion
            }
            else
            {
                #region 横向柱形图

                var option = new
                {
                    //show = isDetail,
                    tooltip = new
                    {
                        trigger = "axis",
                        confine = true
                    },
                    title = new
                    {
                        text = title,
                        subtext = subTitle,
                        left = isDetail ? "center" : "left",
                        top = isDetail ? 20 : 5,
                        textStyle = new
                        {
                            color = "#ccc",
                            fontSize = isDetail ? 18 : 15,
                        },
                        subtextStyle = new
                        {
                            fontWeight = "lighter",
                            fontSize = isDetail ? 15 : 10,
                        },
                        itemGap = 5
                    },
                    toolbox = new
                    {
                        show = false,
                        right = 30,
                        feature = new
                        {
                            dataZoom = new
                            {
                                yAxisIndex = "none"
                            },
                            restore = new { },
                            saveAsImage = new { }
                        }
                    },
                    grid = new
                    {
                        top = "30%",
                        bottom = "20%",
                        left = "20%"
                    },
                    //dataZoom = new[]
                    //{
                    //    new {
                    //        type = "slider",
                    //        startValue = startValue 
                    //    }, 
                    //    new {
                    //        type = "inside",
                    //        startValue = "" 
                    //    }
                    //},
                    legend = new
                    {
                        show = isDetail,
                        left = "center",
                        top = "8%",
                        textStyle = new
                        {
                            color = "lightgray"
                        },
                        data = legend.Split(',')
                    },
                    xAxis = new
                    {
                        type = "value",
                        name = lineNameDic[0],
                        boundaryGap = graphType.Split('-')[0] == "bar" ? true : false,
                        splitLine = new
                        {
                            show = true,
                            lineStyle = new
                            {
                                color = "lightgray",
                                type = "dotted"
                            }
                        },
                        axisLine = new
                        {
                            show = true,
                            lineStyle = new { color = "lightgray" }
                        },
                        axisTick = new
                        {
                            lineStyle = new { color = "lightgray" }
                        },
                        axisLabel = new
                        {
                            textStyle = new { color = "lightgray" }
                        }
                        //min = "null"
                    },
                    yAxis = new
                    {
                        type = "category",
                        splitLine = new { show = false },
                        data = xAxis.Split(','),
                        axisLine = new
                        {
                            show = true,
                            onZero = onZero ? true : false,
                            lineStyle = new { color = "lightgray" }
                        },
                        axisTick = new
                        {
                            lineStyle = new { color = "lightgray" }
                        },
                        axisLabel = new
                        {
                            //interval = 0,
                            textStyle = new { color = "lightgray" },
                            rotate = 30
                        }
                    },
                    series = from item in yAxisDouble
                             select new
                             {
                                 name = item.Key,
                                 type = lineFields.Any(d => d == item.Key) ? "line" : graphType,
                                 smooth = true,
                                 data = item.Value
                             },
                    color = new string[] { "#ef8691", "#f2b33f", "#e6f490", "#a2f0bc", "#8ad1ef", "#3fc8f2", "#90abf3", "#c9a9db" }
                };

                var jsonStr = JsonHelper.ToJson<object>(option);
                string minOrMax = "";
                if (!string.IsNullOrEmpty(min))
                    minOrMax += "\"min\":" + min + ",";
                if (!string.IsNullOrEmpty(max))
                    minOrMax += "\"max\":" + max + ",";
                jsonStr = jsonStr.Replace("\"xAxis\":{", "\"xAxis\":{" + minOrMax);
                return jsonStr;

                #endregion
            }
        }

        /// <summary>
        /// 饼图创建接口
        /// </summary>
        /// <param name="type">图表类型</param>
        /// <param name="title">图表标题</param>
        /// <param name="subTitle">图表副标题</param>
        /// <param name="seriesName">图例名称</param>
        /// <param name="data">图表数据</param>
        /// <returns>饼图Json</returns>
        public static string CreatePieChartOption(string type, string title, string subTitle, bool isDetail, string seriesName, Dictionary<string, double> data)
        {
            var rtn = new
            {
                title = new
                {
                    //show = isDetail,
                    text = title,
                    subtext = subTitle,
                    left = isDetail ? "center" : "left",
                    top = isDetail ? 20 : 5,
                    textStyle = new
                    {
                        color = "#ccc",
                        fontSize = isDetail ? 18 : 15,
                    },
                    subtextStyle = new
                    {
                        fontWeight = "lighter",
                        fontSize = isDetail ? 15 : 10,
                    }
                },
                toolbox = new
                {
                    show = false,
                    right = 30,
                    feature = new
                    {
                        saveAsImage = new { }
                    }
                },
                tooltip = new
                {
                    trigger = "item",
                    formatter = "{a} <br/>{b} : {c} ({d}%)",
                    confine = true
                },
                series = new[]
                {
                    new{
                        name = seriesName,
                        type = type,
                        radius = "70%",
                        center = new string[] { "50%", "55%" },
                        label = new {
                            normal = new {
                                show = true,
                                position = "outside",
                                textStyle = new {
                                    color = "#FFFFFF",
                                    fontSize = 14
                                }
                            }
                        },
                        data = from item in data
                            select new {
                                value = item.Value,
                                name = item.Key
                            },
                        itemStyle=new {
                            emphasis=new {
                                shadowBlur= 10,
                                shadowOffsetX= 0,
                                shadowColor= "rgba(0, 0, 0, 0.5)"
                            }
                        },
                        animationType = "scale",
                        animationEasing = "elasticOut",
                        animationDelay = 71
                    }
                },
                //color = new string[] { "#c23531", "#61a0a8", "#d48265", "#91c7ae", "#749f83", "#ca8622", "#bda29a", "#6e7074", "#2f4554", "#546570", "#c4ccd3" }
                //color = new string[] { "#ef8691", "#f2b33f", "#e6f490", "#a2f0bc", "#8ad1ef", "#3fc8f2", "#90abf3", "#c9a9db" }
                color = new string[] { "#c33632", "#2e4354", "#d58364", "#62a1a9", "#91c9b0", "#749f83", "#cb8723", "#bda49c", "#6f7174", "#546470", "#c6ced3", "#c43430", "#2e4455", "#60a2aa", "#d68364" }
            };

            var jsonStr = JsonHelper.ToJson<object>(rtn);
            return jsonStr;
        }

        /// <summary>
        /// 仪表盘创建接口
        /// </summary>
        /// <param name="type">图表类型</param>
        /// <param name="seriesName">图例名称</param>
        /// <param name="gaugeValue">比例数值</param>
        /// <param name="gaugeText">比例说明文本</param>
        /// <param name="title">图表标题</param>
        /// <param name="subTitle">图表副标题</param>
        /// <param name="lineArray">轴线分段</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="isDetail">是否详情页</param>
        /// <returns></returns>
        public static string CreateGaugeChartOption(string type, string seriesName, string gaugeValue, string gaugeText, string title, string subTitle, string lineArray, double minValue, double maxValue, bool isDetail)
        {
            var arr = lineArray.Split(';');
            List<object[]> list = new List<object[]>();
            for (var i = 0; i < arr.Length; i++)
            {
                var item = arr[i].Split(',');
                list.Add(new object[] { double.Parse(item[0]), item[1] });
            }

            var option = new
            {
                title = new
                {
                    //show = isDetail,
                    text = title,
                    subtext = subTitle,
                    left = isDetail ? "center" : "left",
                    top = isDetail ? 20 : 5,
                    textStyle = new
                    {
                        color = "#ccc",
                        fontSize = isDetail ? 18 : 15,
                    },
                    subtextStyle = new
                    {
                        fontWeight = "lighter",
                        fontSize = isDetail ? 15 : 10,
                    }
                },
                //grid = new
                //{
                //    top = "30%",
                //    bottom = "20%"
                //},
                tooltip = new
                {
                    formatter = "{a} <br/>{b} : {c}%",
                    confine = true
                },
                toolbox = new
                {
                    show = false,
                    feature = new
                    {
                        restore = new { },
                        saveAsImage = new { }
                    }
                },
                series = new[]
                {
                    new{
                        name = seriesName ,
                        type = type,
                        min = minValue,
                        max = maxValue,
                        center = new string[]{"center", "75%"},
                        startAngle = 180,
                        endAngle = 0,
                        radius = "100%",
                        title = new {
                            offsetCenter = new object[]{0, "-40%"},
                            textStyle = new {
                                color = "ligntgray",
                                fontSize = 8
                            }
                        },
                        detail = new {
                            offsetCenter = new object[]{0,"15%"},
                            formatter = "{value}%",
                            textStyle = new {fontSize = 15},
                            height = 10
                        },
                        axisLine = new {
                            show = isDetail,
                            lineStyle = new {
                                color = list,
                                width = 25
                            }
                        },
                        axisTick = new{
                            splitNumber = isDetail ? 20 : 5
                        },
                        axisLabel = new{
                            show = isDetail
                        },
                        pointer = new
                        {
                            width = 6
                        },
                        data = new []
                        {
                            new{
                                value=  gaugeValue ,
                                name=  gaugeText
                            }
                        }
                    }
                }
            };

            var jsonStr = JsonHelper.ToJson<object>(option);
            return jsonStr;
        }

        /// <summary>
        /// 生成列表数据
        /// </summary>
        /// <param name="data">列表数据</param>
        /// <param name="headerData">表头相关数据</param>
        /// <param name="headField">表头字段</param>
        /// <param name="title">图表标题</param>
        /// <param name="subTitle">图表副标题</param>
        /// <param name="isDetail">是否详情页</param>
        /// <returns>列表Json</returns>
        public static string CreateTableOption(DataTable data, List<Dictionary<string, object>> headerData, string[] headField, string title, string subTitle, bool isDetail)
        {
            //汇总类型
            Dictionary<string, string> summaryType = new Dictionary<string, string>();
            //数字格式
            Dictionary<string, string> numberformat = new Dictionary<string, string>();
            //枚举
            Dictionary<string, object> enumDic = new Dictionary<string, object>();
            string[] headerFormatList = new string[headerData.Count];
            int colIndex = 0;
            foreach (var item in headerData)
            {
                if (item.GetValue("Visible").ToLower() == "true")
                {
                    if (item.ContainsKey("SummaryType") && !string.IsNullOrEmpty(item.GetValue("SummaryType")))
                        summaryType.Add(colIndex.ToString(), item.GetValue("SummaryType"));
                    if (item.ContainsKey("NumberFormat") && !string.IsNullOrEmpty(item.GetValue("NumberFormat")))
                        numberformat.Add(colIndex.ToString(), item.GetValue("NumberFormat"));

                    //列属性
                    string headerFormat = "";
                    //宽度
                    if (item.ContainsKey("width") && !string.IsNullOrEmpty(item.GetValue("width")))
                        headerFormat += "width:" + item.GetValue("width") + "px;";
                    //对齐
                    if (item.ContainsKey("align") && !string.IsNullOrEmpty(item.GetValue("align")))
                        headerFormat += "text-align:" + item.GetValue("align") + ";";

                    headerFormatList[colIndex] = headerFormat;
                    colIndex++;

                    #region 枚举处理
                    var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
                    var enumDefCode = item.GetValue("data");
                    if (string.IsNullOrEmpty(enumDefCode)) continue;
                    if (!enumDefCode.StartsWith("["))
                    {
                        var enumDefInfo = baseEntities.Set<S_M_EnumDef>().FirstOrDefault(d => d.Code == enumDefCode);
                        if (enumDefInfo == null)
                            throw new Exception("编号为【" + enumDefCode + "】的枚举不存在！");
                        var enumItemList = baseEntities.Set<S_M_EnumItem>().Where(d => d.EnumDefID == enumDefInfo.ID).OrderBy(d => d.SortIndex).
                                            Select(d => new { Code = d.Code, Name = d.Name }).ToList();
                        enumDic.Add(item.GetValue("Code").ToUpper(), JsonHelper.ToJson<object>(enumItemList));
                    }
                    else
                    {
                        var enumItemList = JsonHelper.ToObject<List<Dictionary<string, object>>>(enumDefCode);
                        var enumItemJson = from enumItem in enumItemList
                                           select new
                                           {
                                               Code = enumItem.GetValue("value"),
                                               Name = enumItem.GetValue("text")
                                           };
                        enumDic.Add(item.GetValue("Code").ToUpper(), JsonHelper.ToJson<object>(enumItemJson));
                    }
                    #endregion
                }
            }

            #region 数据处理

            int colNum = data.Columns.Count;
            int rowNum = data.Rows.Count;
            object[][] dataRows = new object[rowNum][];
            for (var i = 0; i < rowNum; i++)
            {
                var row = data.Rows[i];
                dataRows[i] = new object[colNum];
                for (var j = 0; j < colNum; j++)
                {
                    if (numberformat.Keys.Contains(j.ToString()))
                    {
                        var format = numberformat[j.ToString()];
                        dataRows[i][j] = FormatNumber(row[j].ToString(), format);
                    }
                    else
                        dataRows[i][j] = GetEnumText(data.Columns[j].ColumnName, row[j].ToString(), enumDic);
                }
            }

            #endregion

            var option = new
            {
                dataSources = new
                {
                    title = new
                    {
                        //show = isDetail,
                        text = title,
                        subtext = subTitle,
                        textStyle = new
                        {
                            color = "#ccc",
                            fontSize = isDetail ? 18 : 15,
                        },
                        subtextStyle = new
                        {
                            fontWeight = "lighter",
                            fontSize = isDetail ? 15 : 10,
                        }
                    },
                    type = "table",
                    theads = headField,
                    summaryType = summaryType,
                    numberformat = numberformat,
                    tbodys = dataRows
                },
                blockFlexStyle = headerFormatList
            };

            var jsonStr = JsonHelper.ToJson<object>(option);
            return jsonStr;
        }

        /// <summary>
        /// 生成静态块数据
        /// </summary>
        /// <param name="text">标题</param>
        /// <param name="value">值</param>
        /// <param name="unit">单位</param>
        /// <param name="isDetail">是否详情页</param>
        /// <returns>静态块Json</returns>
        public static string CreateBlockChartOption(string text, string value, string unit, bool isDetail)
        {
            var option = new
            {
                dataSources = new
                {
                    type = "staticsBlock",
                    title = text,
                    content = value,
                    unit = unit
                },
                blockFlexStyle = new { }
            };

            var jsonStr = JsonHelper.ToJson<object>(option);
            return jsonStr;
        }

        /// <summary>
        /// 生成文本块数据
        /// </summary>
        /// <param name="curGraphInfo">文本信息</param>
        /// <param name="content">文本内容</param>
        /// <returns>文本块Json</returns>
        public static string CreateTextBlockOption(Dictionary<string, object> curGraphInfo, string content, string title)
        {
            int titleFontSize = int.Parse(curGraphInfo.GetValue("titleFontSize"));
            string titleAlignX = curGraphInfo.GetValue("titleAlignX");
            int fontSize = int.Parse(curGraphInfo.GetValue("fontSize"));
            string alignX = curGraphInfo.GetValue("alignX");
            string alignY = curGraphInfo.GetValue("alignY");

            var option = new
            {
                dataSources = new
                {
                    type = "textBlock",
                    title = title,
                    titleFontSize = titleFontSize,
                    titleAlignX = titleAlignX,
                    content = content,
                    fontSize = fontSize,
                    alignX = alignX,
                    alignY = alignY
                },
                blockFlexStyle = new { }
            };

            var jsonStr = JsonHelper.ToJson<object>(option);
            return jsonStr;
        }

        private static string GetEnumText(string fieldName, string value, Dictionary<string, object> enumDic)
        {
            string res = value;
            if (enumDic.Keys.Contains(fieldName))
            {
                var enumStr = JsonHelper.ToObject<List<Dictionary<string, string>>>(enumDic[fieldName].ToString());

                foreach (var item in enumStr)
                {
                    if (item["Code"] == value)
                    {
                        res = item["Name"].ToString();
                        break;
                    }
                }
                return res;
            }
            else
                return value;
        }

        private static string FormatNumber(string number, string format)
        {
            double num = 0.00;
            try
            {
                num = double.Parse(number);
            }
            catch (Exception)
            {
                throw new Exception("值【" + number + "】不是数值型!");
            }

            switch (format)
            {
                case "thousands":
                    return num.ToString("N");
                case "currency":
                    return num.ToString("C");
                case "percentile":
                    return num.ToString("P");
                default:
                    return number;
            }

        }
    }
}
