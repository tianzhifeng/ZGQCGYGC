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
using System.Web;


namespace Base.Logic
{
    public class FileGrid : BaseComponent
    {
        public FileGrid(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var result = new Dictionary<string, object>();
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
            var dataSourceName = this.BlockDef.GetValue("InUserDataSource");
            result.SetValue("hasimg", String.IsNullOrEmpty(this.BlockDef.GetValue("FileTypeField")) ? 0 : 1);
            if (this.DataSource.ContainsKey(dataSourceName))
            {
                if (this.DataSource[dataSourceName] == null)
                {
                    throw new Exception("数据源为空，无法加载数据");
                }
                var dt = this.DataSource[dataSourceName] as DataTable;
                var list = GetData(dt, IsMobile);
                result.SetValue("hasdata", "0");
                if (list.Count > 0)
                {
                    result.SetValue("hasdata", "1");
                }
                result.SetValue("Style", !string.IsNullOrEmpty(this.BlockDef.GetValue("Height")) ? string.Format("height:{0};overflow:auto;", this.BlockDef.GetValue("Height")) : "");
                result.SetValue("List", list);
                result.SetValue("chartId", this.ID + "_FileGrid");
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("showUrl")))
                {
                    result.SetValue("showmore", 1);
                    result.SetValue("moreUrl", this.BlockDef.GetValue("showUrl"));
                }
                else
                {
                    result.SetValue("showmore", 0);
                }

                #region 过滤
                var groupList = new List<Dictionary<string, object>>();
                var groupField = this.BlockDef.GetValue("GroupField");
                if (!String.IsNullOrEmpty(groupField) && groupField.IndexOf("[]") < 0)
                {
                    var filters = JsonHelper.ToList(this.BlockDef.GetValue("GroupField"));
                    if (filters.Count > 0)
                    {
                        Dictionary<string, object> noGroup = new Dictionary<string, object>();
                        noGroup.Add("value", "none");
                        noGroup.Add("text", "不分组");
                        filters.Add(noGroup);
                    }
                    foreach (var filter in filters)
                    {
                        if (String.IsNullOrEmpty(filter.GetValue("IsDefault")))
                        {
                            filter.SetValue("IsDefault", "false");
                        }
                    }
                    var group = HttpContext.Current.Request["Group"];
                    if (!string.IsNullOrEmpty(parameters) && IsMobile)
                    {
                        var paras = JsonHelper.ToObject<Dictionary<string, string>>(parameters);
                        group = paras.GetValue("Group");
                    }
                    if (string.IsNullOrEmpty(group))
                    {
                        foreach (var filter in filters)
                        {
                            if (!String.IsNullOrEmpty(filter.GetValue("IsDefault")) && Convert.ToBoolean(filter.GetValue("IsDefault")))
                            {
                                var filterKey = filter.GetValue("value");
                                groupList = Filter(dt, filterKey, IsMobile);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var filter in filters)
                        {
                            var value = filter.GetValue("value");
                            if (value == group)
                            {
                                filter.SetValue("IsDefault", "true");
                            }
                            else
                            {
                                filter.SetValue("IsDefault", "false");
                            }
                        }
                        try
                        {
                            groupList = Filter(dt, group, IsMobile);
                        }
                        catch (Exception)
                        {
                        }

                    }
                    if (groupList.Count > 0)
                        result.SetValue("GroupData", groupList);
                    result.SetValue("GroupFilters", filters);
                }
                #endregion

            }
            return result;
        }

        private List<Dictionary<string, object>> GetData(DataTable dt, bool IsMobile = false)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var rowItem = Formula.FormulaHelper.DataRowToDic(row);
                var item = new Dictionary<string, object>();
                item.SetValue("Title", this.ReplaceRegString(this.BlockDef.GetValue("Title"), rowItem, null));
                item.SetValue("Content", this.ReplaceRegString(this.BlockDef.GetValue("Content"), rowItem, null));
                item.SetValue("SubTitle", this.ReplaceRegString(this.BlockDef.GetValue("Tag"), rowItem, null));
                var openUrl = IsMobile ? this.BlockDef.GetValue("MobileLinkUrl") : this.BlockDef.GetValue("LinkUrl");
                if (!String.IsNullOrEmpty(openUrl) || IsMobile)
                {
                    openUrl = this.ReplaceRegString(openUrl, rowItem, null);
                    item.SetValue("onclick", "openUrl('" + openUrl + "','" + rowItem.GetValue("width").Trim() + "','" + rowItem.GetValue("height").Trim() + "')");
                }
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("FileIDField")))
                {
                    item.SetValue("FileID", rowItem.GetValue(this.BlockDef.GetValue("FileIDField")));
                    if (!String.IsNullOrEmpty(item.GetValue("FileID")))
                    {
                        item.SetValue("onclick", "downLoadFiles('" + item.GetValue("FileID") + "')");
                    }
                }
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("FileTypeField")))
                {
                    var value = rowItem.GetValue(this.BlockDef.GetValue("FileTypeField"));
                    var IMG = "";
                    if (!string.IsNullOrEmpty(value) && value.ToLower().IndexOf(".png") >= 0)
                    {
                        IMG = value.ToLower();
                    }
                    else
                    {
                        switch (value.ToLower())
                        {
                            case "xls":
                            case "xlsx":
                                IMG = "/MVCConfig/Scripts/BI/img/format/EXCEL.png";
                                break;
                            case "doc":
                            case "docx":
                                IMG = "/MVCConfig/Scripts/BI/img/format/WORD.png";
                                break;
                            case "pdf":
                                IMG = "/MVCConfig/Scripts/BI/img/format/PDF.png";
                                break;
                            case "jpg":
                            case "png":
                            case "bmp":
                            case "gif":
                                IMG = "/MVCConfig/Scripts/BI/img/format/IMG.png";
                                break;
                            case "dwg":
                                IMG = "/MVCConfig/Scripts/BI/img/format/DWG.png";
                                break;
                            default:
                                IMG = "/MVCConfig/Scripts/BI/img/format/Ohter.png";
                                break;
                        }
                    }
                    item.SetValue("IMG", IMG);
                }
                list.Add(item);
            }
            return list;
        }

        private List<Dictionary<string, object>> Filter(DataTable dt, string filterKey, bool IsMobile = false)
        {
            var tagKeys = new List<string>();
            var groupList = new List<Dictionary<string, object>>();
            Regex reg = new Regex(@"(?<=\{)[^}]*(?=\})");
            foreach (Match m in reg.Matches(this.BlockDef.GetValue("GroupTag")))
            {
                string key = m.Value.Trim('{', '}');
                tagKeys.Add(key);
            }

            var query = from t in dt.AsEnumerable()
                        group t by new { filterKey = t.Field<string>(filterKey) } into m
                        select new
                        {
                            groupName = m.Key.filterKey,
                            info = m,
                            rowCount = m.Count()
                        };

            foreach (var item in query)
            {
                List<DataRow> dataRows = item.info.ToList();

                var rowItem = new Dictionary<string, object>();
                var groupRow = new Dictionary<string, object>();
                foreach (var tagKey in tagKeys)
                {
                    var value = "";
                    var op = tagKey.Split(':').Length > 1 ? tagKey.Split(':')[0] : "SUM";
                    var field = tagKey.Split(':').Length > 1 ? tagKey.Split(':')[1] : tagKey;
                    switch (op.ToUpper())
                    {
                        case "MAX":
                            value = Convert.ToString(Decimal.Round(dataRows.Max(p => Convert.ToDecimal(p.Field<decimal>(field))), 2));
                            break;
                        case "MIN":
                            value = Convert.ToString(Decimal.Round(dataRows.Min(p => Convert.ToDecimal(p.Field<decimal>(field))), 2));
                            break;
                        case "COUNT":
                            value = item.rowCount.ToString();
                            break;
                        default:
                            value = Convert.ToString(Decimal.Round(dataRows.Sum(p => Convert.ToDecimal(p.Field<decimal>(field))), 2));
                            break;
                    }
                    rowItem.Add(tagKey, value);
                }
                var tag = this.ReplaceRegString(this.BlockDef.GetValue("GroupTag"), rowItem, null);
                groupRow.Add("GroupName", item.groupName);
                groupRow.Add("GroupCount", item.rowCount.ToString());
                groupRow.Add("GroupTag", tag);
                var newDataTable = dt.Clone();
                foreach (var dr in dataRows)
                {
                    newDataTable.ImportRow(dr);
                }
                var groupDataList = GetData(newDataTable, IsMobile);
                groupRow.Add("GroupDataList", groupDataList);
                groupList.Add(groupRow);
            }
            return groupList;
        }
    }
}
