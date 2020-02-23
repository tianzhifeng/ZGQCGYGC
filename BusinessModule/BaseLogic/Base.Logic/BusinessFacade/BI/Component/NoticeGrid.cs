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
    public class NoticeGrid : BaseComponent
    {
        public NoticeGrid(string BlockDefJson)
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
            if (this.DataSource.ContainsKey(dataSourceName))
            {
                if (this.DataSource[dataSourceName] == null)
                {
                    throw new Exception("数据源为空，无法加载数据");
                }
                var dt = this.DataSource[dataSourceName] as DataTable;
                var list = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var rowItem = Formula.FormulaHelper.DataRowToDic(row);
                    var item = Formula.FormulaHelper.DataRowToDic(row);
                    item.SetValue("ID", rowItem.GetValue("ID"));
                    item.SetValue("Title", this.ReplaceRegString(this.BlockDef.GetValue("Title"), rowItem, null));
                    item.SetValue("DateTime", this.ReplaceRegString(this.BlockDef.GetValue("DateTimeField"), rowItem, null));
                    var tagDef = this.BlockDef.GetValue("tagDefine");
                    if (!String.IsNullOrEmpty(tagDef))
                    {
                        var TagList = new List<Dictionary<string, object>>();
                        var tagDefList = JsonHelper.ToList(tagDef);
                        foreach (var tagDefItem in tagDefList)
                        {
                            var tagValueDef = tagDefItem.GetValue("Value");
                            var tag = new Dictionary<string, object>();
                            tag.SetValue("Value", this.ReplaceRegString(tagValueDef, item, null));
                            TagList.Add(tag);
                        }
                        item.SetValue("Tag", TagList);
                    }

                    if (!String.IsNullOrEmpty(this.BlockDef.GetValue("FileIDField")))
                    {
                        item.SetValue("FileID", item.GetValue(this.BlockDef.GetValue("FileIDField")));
                    }

                    if (!String.IsNullOrEmpty(this.BlockDef.GetValue("LinkUrl").Trim()) || (IsMobile && !String.IsNullOrEmpty(this.BlockDef.GetValue("MobileLinkUrl").Trim())))
                    {
                        var url = fo.ReplaceString(IsMobile ? this.BlockDef.GetValue("MobileLinkUrl").Trim() : this.BlockDef.GetValue("LinkUrl").Trim(), dt.Rows[0]);
                        item.SetValue("LinkUrl", "openUrl('" + url + "')");
                    }
                    list.Add(item);
                }
                result.SetValue("hasdata", "0");
                if (list.Count > 0)
                {
                    result.SetValue("hasdata", "1");
                }
                result.SetValue("List", list);
                if (!String.IsNullOrEmpty(this.BlockDef.GetValue("showUrl")))
                {
                    result.SetValue("showmore", 1);
                    result.SetValue("moreUrl", this.BlockDef.GetValue("showUrl"));
                }
                else
                {
                    result.SetValue("showmore", 0);
                }

            }
            return result;
        }
    }
}
