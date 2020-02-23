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
    public class Tab : BaseComponent
    {
        public Tab(string BlockDefJson)
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
            var defaultUrl = ""; var defaultHeight = "";
            if (!String.IsNullOrEmpty(tagDef))
            {
                var TagList = new List<Dictionary<string, object>>();
                var tagDefList = JsonHelper.ToList(tagDef);
                for (int i = 0; i < tagDefList.Count; i++)
                {
                    var tagDefItem = tagDefList[i];
                    var tagValueDef = tagDefItem.GetValue("Value").Trim();
                    var tag = new Dictionary<string, object>();
                    tag.SetValue("Title", tagDefItem.GetValue("Title").Trim());
                    tag.SetValue("Unit", tagDefItem.GetValue("Unit").Trim());
                    if (!String.IsNullOrEmpty(tagDefItem.GetValue("LinkUrl").Trim()))
                    {
                        tag.SetValue("LinkUrl", fo.GetDefaultValue(String.Empty, tagDefItem.GetValue("LinkUrl").Trim(), this.DataSource));
                        tag.SetValue("HasLinkUrl", "1");
                        tag.SetValue("width", tagDefItem.GetValue("width").Trim());
                        tag.SetValue("height", tagDefItem.GetValue("height").Trim());
                    }
                    if (i == 0)
                    {
                        tag.SetValue("className", "active");
                        defaultUrl = tag.GetValue("LinkUrl");
                        defaultHeight = String.IsNullOrEmpty(tag.GetValue("height")) ? "400px" : tag.GetValue("height");
                    }
                    tag.SetValue("divClassName", this.getBorderStyle(this.BlockDef.GetValue("Style")));
                    TagList.Add(tag);
                }
                result.SetValue("DefaultUrl", defaultUrl);
                result.SetValue("DefaultHeight", defaultHeight);
                result.SetValue("Tag", TagList);
            }
            return result;
        }

        string getBorderStyle(string style)
        {
            var linecolor = "border-blue";
            switch (style.ToLower())
            {
                case "lineblue":
                    linecolor = "border-blue";
                    break;
                case "skyblue":
                    linecolor = "border-blue";
                    break;
                case "linered":
                    linecolor = "border-red";
                    break;
                case "linegreen":
                    linecolor = "border-green";
                    break;
                case "linepurple":
                    linecolor = "border-purple";
                    break;
                case "lineorange":
                    linecolor = "border-orange";
                    break;
            }
            return linecolor;
        }
    }
}
