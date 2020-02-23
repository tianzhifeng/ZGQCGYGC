using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Config;
using Config.Logic;
using Formula.Helper;
using Base.Logic.BusinessFacade;

namespace Base.Logic
{
    public class Navigation : BaseComponent
    {
        public Navigation(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        //string sql = "select max(ID) from {TableName} where EngineeringInfoID='{EngineeringInfoID}'";

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            var result = new Dictionary<string, object>();
            var categoryDefList = JsonHelper.ToList(this.BlockDef.GetValue("tagDefine"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));

            #region 计算有多少列并计算每列的列宽
            var columnCount = 0m; var unitColumnwidth = 0m; decimal minWidth = 0;

            var columnList = categoryDefList.Select(c => c["ColumnCount"].ToString()).ToList();

            foreach (var column in columnList)
            {
                if (String.IsNullOrEmpty(column))
                {
                    columnCount += 1; continue;
                }
                columnCount += Convert.ToDecimal(column);
            }

            unitColumnwidth = Math.Round(Convert.ToDecimal(100m / columnCount), 2);
            minWidth = columnCount * 200;
            #endregion

            //定义的分类分组节点
            var categoryList = new List<Dictionary<string, object>>();
            for (int i = 0; i < categoryDefList.Count; i++)
            {
                var categoryDef = categoryDefList[i];
                var categoryItem = new Dictionary<string, object>();
                var categoryColumnCount = String.IsNullOrEmpty(categoryDef.GetValue("ColumnCount")) ? 1 : Convert.ToInt32(categoryDef.GetValue("ColumnCount"));
                var width = (unitColumnwidth * categoryColumnCount).ToString() + "%";
                if (i == categoryDefList.Count - 1)
                {
                    categoryItem.SetValue("width", Math.Round((100 - (unitColumnwidth * (columnCount - categoryColumnCount))), 2) + "%");
                }
                else
                {
                    categoryItem.SetValue("width", width);
                }
                categoryItem.SetValue("Title", categoryDef.GetValue("Title"));
                if (i == categoryDefList.Count - 1)
                {
                    categoryItem.SetValue("IsLast", "1");
                }
                else
                {
                    categoryItem.SetValue("IsLast", "0");
                }

                //定义分组有多少列，有多少列，则根据item数量自动平铺排列
                var itemColumnList = new List<Dictionary<string, object>>();
                //var columnsItemDic = new Dictionary<string, object>();
                for (int m = 0; m < categoryColumnCount; m++)
                {
                    var categoryColumn = new Dictionary<string, object>();
                    categoryColumn.SetValue("width", Math.Round(Convert.ToDecimal(100 / categoryColumnCount), 2) + "%");
                    categoryColumn.SetValue("ColumnIndex", m + 1);
                    categoryColumn.SetValue("Items", new List<Dictionary<string, object>>());
                    itemColumnList.Add(categoryColumn);
                }
                var itemList = JsonHelper.ToList(categoryDef.GetValue("Item"));
                for (int m = 0; m < itemList.Count; m++)
                {
                    var columnIndex = (m + 1) % categoryColumnCount;
                    if (columnIndex == 0) columnIndex = categoryColumnCount;
                    var items = itemColumnList.FirstOrDefault(c => Convert.ToInt32(c["ColumnIndex"]) == columnIndex).GetObject("Items") as List<Dictionary<string, object>>;
                    var item = itemList[m];
                    var connName = item.GetValue("ConnName");
                    var sql = item.GetValue("SQL");
                    if (!String.IsNullOrEmpty(sql) && !String.IsNullOrEmpty(connName))
                    {
                        var db = SQLHelper.CreateSqlHelper(connName);
                        sql = fo.ReplaceString(sql);
                        var dt = db.ExecuteDataTable(sql);
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            item.SetValue("State", "0");
                        }
                        else
                        {
                            item.SetValue("State", "1");
                            var url = fo.ReplaceString(IsMobile ? item.GetValue("MobileLinkUrl").Trim() : item.GetValue("LinkUrl").Trim(), dt.Rows[0]);
                            item.SetValue("LinkUrl", "openUrl('" + url + "')");
                            if (dt.Columns.Contains("CreateDate"))
                            {
                                item.SetValue("Date", Convert.ToDateTime(dt.Rows[0]["CreateDate"]).ToShortDateString());
                            }
                        }
                    }
                    else
                    {
                        item.SetValue("State", "0");
                    }
                    items.Add(item);
                }
                categoryItem.SetValue("Columns", itemColumnList);
                categoryList.Add(categoryItem);
            }
            this.setStyle(result);
            result.SetValue("minWidth", minWidth.ToString() + "px");
            result.SetValue("ID", this.ID);
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("Tag", categoryList);
            return result;
        }
    }
}
