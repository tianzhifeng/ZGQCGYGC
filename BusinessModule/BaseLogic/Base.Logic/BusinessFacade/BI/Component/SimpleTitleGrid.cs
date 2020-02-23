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
    public class SimpleTitleGrid : BaseComponent
    {
        public SimpleTitleGrid(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var result = this.BlockDef;
            result.SetValue("Title", this.BlockDef.GetValue("MainTitle"));
            result.SetValue("ShowTitle", this.BlockDef.GetValue("ShowTitle"));
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
                    tag.SetValue("RowHeight", String.IsNullOrEmpty(tagDefItem.GetValue("rowheight").Trim()) ? "44px"
                        : tagDefItem.GetValue("rowheight").Trim().TrimEnd('x', 'p').TrimEnd('t', 'p') + "px");
                    if (!String.IsNullOrEmpty(tagDefItem.GetValue("LinkUrl").Trim()) || (IsMobile && !String.IsNullOrEmpty(tagDefItem.GetValue("MobileLinkUrl").Trim())))
                    {
                        tag.SetValue("LinkUrl", fo.GetDefaultValue(String.Empty, IsMobile ? tagDefItem.GetValue("MobileLinkUrl").Trim() : tagDefItem.GetValue("LinkUrl").Trim(), this.DataSource));
                        tag.SetValue("HasLinkUrl", "1");
                        tag.SetValue("width", tagDefItem.GetValue("width").Trim());
                        tag.SetValue("height", tagDefItem.GetValue("height").Trim());
                    }
                    TagList.Add(tag);
                }
                result.SetValue("Tag", TagList);
            }
            return result;
        }
    }
}
