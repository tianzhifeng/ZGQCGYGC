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
    public class TitleBlock : BaseComponent
    {
        public TitleBlock(string BlockDefJson)
            : base(BlockDefJson)
        {
        }

        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var titleInfo = new Dictionary<string, object>();
            var titleDef = this.BlockDef.GetValue("Title");
            var codeDef = this.BlockDef.GetValue("Code");
            var SubTitleDef = this.BlockDef.GetValue("SubTitle");
            if (this.DataSource != null)
            {
                titleInfo.SetValue("Title", fo.GetDefaultValue("", titleDef, this.DataSource));
                titleInfo.SetValue("Code", fo.GetDefaultValue("", codeDef, this.DataSource));
                titleInfo.SetValue("SubTitle", fo.GetDefaultValue("", SubTitleDef, this.DataSource));
            }
            else
            {
                titleInfo.SetValue("Title", titleInfo);
                titleInfo.SetValue("Code", codeDef);
                titleInfo.SetValue("SubTitle", SubTitleDef);
            }

            var tagDef = this.BlockDef.GetValue("tagDefine");
            if (!String.IsNullOrEmpty(tagDef))
            {
                var TagList = new List<Dictionary<string, object>>();
                var tagDefList = JsonHelper.ToList(tagDef);
                foreach (var tagDefItem in tagDefList)
                {
                    var tagValueDef = tagDefItem.GetValue("Value").Trim();
                    var tag = new Dictionary<string, object>();
                    tag.SetValue("Value", fo.GetDefaultValue(String.Empty, tagValueDef, this.DataSource));
                    TagList.Add(tag);
                }
                titleInfo.SetValue("Tag", TagList);
            }

            var attrDefine = this.BlockDef.GetValue("attrDefine");
            if (!String.IsNullOrEmpty(attrDefine))
            {
                var attrList = new List<Dictionary<string, object>>();
                var attrDefList = JsonHelper.ToList(attrDefine);
                foreach (var attrDefItem in attrDefList)
                {
                    var attrValueDef = attrDefItem.GetValue("Value").Trim();
                    var attr = new Dictionary<string, object>();
                    attr.SetValue("Name", attrDefItem.GetValue("Name"));
                    attr.SetValue("Value", fo.GetDefaultValue(String.Empty, attrValueDef, this.DataSource));
                    attrList.Add(attr);
                }
                titleInfo.SetValue("Attrs", attrList);
            }

            var buttonDefine = this.BlockDef.GetValue("buttonDefine");
            if (!String.IsNullOrEmpty(buttonDefine))
            {
                var buttonList = new List<Dictionary<string, object>>();
                var buttonDefList = JsonHelper.ToList(buttonDefine);
                foreach (var buttonDefItem in buttonDefList)
                {
                    var buttonValueDef = buttonDefItem.GetValue("Value").Trim();
                    var button = new Dictionary<string, object>();
                    button.SetValue("ID", buttonDefItem.GetValue("ID"));
                    button.SetValue("Name", buttonDefItem.GetValue("Name"));
                    button.SetValue("OnClick", buttonDefItem.GetValue("OnClick"));
                    buttonDefList.Add(button);
                }
                titleInfo.SetValue("Button", buttonDefList);
            }
            return titleInfo;
        }
    }
}
