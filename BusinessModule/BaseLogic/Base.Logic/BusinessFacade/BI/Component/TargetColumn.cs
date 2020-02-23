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
    public class TargetColumn : BaseComponent
    {
        public TargetColumn(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var result = this.BlockDef;
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
            var dataSourceName = this.BlockDef.GetValue("InUserDataSource");
            var targetSourceName = this.BlockDef.GetValue("TargetDataSource");
            var dataSource = this.DataSource.GetValue<DataTable>(dataSourceName);
            var targetDataSource = this.DataSource.GetValue<DataTable>(targetSourceName);
            var fieldName = this.BlockDef.GetValue("FieldName").Trim();
            var targetFieldName = this.BlockDef.GetValue("TargetFieldName").Trim();
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
            if (targetDataSource.Rows.Count > 0)
            {
                targetValue = targetDataSource.Rows[0][targetFieldName] == null || targetDataSource.Rows[0][targetFieldName] == DBNull.Value ? 0m :
                    Convert.ToDecimal(targetDataSource.Rows[0][targetFieldName]);
            }
            var Rate = targetValue == 0 ? 0 : Math.Round(value / targetValue * 100);
            result.SetValue("Rate", Rate);
            this.setStyle(result);

            var tagDef = this.BlockDef.GetValue("tagDefine");
            if (!String.IsNullOrEmpty(tagDef))
            {
                var TagList = new List<Dictionary<string, object>>();
                var tagDefList = JsonHelper.ToList(tagDef);
                foreach (var tagDefItem in tagDefList)
                {
                    var tagValueDef = tagDefItem.GetValue("Value").Trim();
                    var tag = new Dictionary<string, object>();
                    tag.SetValue("Title", tagDefItem.GetValue("Title").Trim());
                    tag.SetValue("Value", fo.GetDefaultValue(String.Empty, tagValueDef, this.DataSource));
                    tag.SetValue("Unit", tagDefItem.GetValue("Unit").Trim());
                    tag.SetValue("Prefix", tagDefItem.GetValue("Prefix").Trim());
                    tag.SetValue("Image", tagDefItem.GetValue("Image").Trim());

                    if (!String.IsNullOrEmpty(tagDefItem.GetValue("LinkUrl").Trim()) || (IsMobile && !String.IsNullOrEmpty(tagDefItem.GetValue("MobileLinkUrl").Trim())))
                    {
                        tag.SetValue("LinkUrl", fo.GetDefaultValue(String.Empty, IsMobile ? tagDefItem.GetValue("MobileLinkUrl").Trim() : tagDefItem.GetValue("LinkUrl").Trim(), this.DataSource));
                        tag.SetValue("HasLinkUrl", "1");
                        tag.SetValue("width", tagDefItem.GetValue("width").Trim());
                        tag.SetValue("height", tagDefItem.GetValue("height").Trim());
                    }

                    if (!String.IsNullOrEmpty(tagDefItem.GetValue("SourceField")) && !String.IsNullOrEmpty(tagDefItem.GetValue("CompareField")))
                    {
                        var sourceValue = fo.GetDefaultValue(String.Empty, tagDefItem.GetValue("SourceField").Trim(), this.DataSource);
                        var compareValue = fo.GetDefaultValue(String.Empty, tagDefItem.GetValue("CompareField").Trim(), this.DataSource);
                        System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.
                   Regex(@"^(-?\d+)(\.\d+)?$");
                        if (!reg1.IsMatch(sourceValue)) throw new Exception("对比比较的源数据必须是数字");
                        if (!reg1.IsMatch(compareValue)) throw new Exception("对比比较的源数据必须是数字");
                        var diff = Math.Round(Convert.ToDecimal(sourceValue) - Convert.ToDecimal(compareValue), 2);
                        tag.SetValue("hasArrow", "1");
                        tag.SetValue("Arrow", diff);
                    }
                    TagList.Add(tag);
                }
                result.SetValue("Tag", TagList);
            }
            return result;
        }
    }
}
