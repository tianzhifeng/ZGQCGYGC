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
    public class ComplexTextBlock : BaseComponent
    {
        public ComplexTextBlock(string BlockDefJson)
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
                var tagDefList = JsonHelper.ToList(tagDef);
                var defCount = tagDefList.Count > 4 ? 4 : tagDefList.Count;
                for (int i = 0; i < defCount; i++)
                {
                    var tagDefItem = tagDefList[i];
                    var tagValueDef = tagDefItem.GetValue("Value").Trim();
                    var tag = new Dictionary<string, object>();
                    result.SetValue("Title" + (i + 1), tagDefItem.GetValue("Title").Trim());
                    result.SetValue("Prefix" + (i + 1), tagDefItem.GetValue("Prefix").Trim());
                    result.SetValue("Value" + (i + 1), fo.GetDefaultValue(String.Empty, tagValueDef, this.DataSource));
                    result.SetValue("Unit" + (i + 1), tagDefItem.GetValue("Unit").Trim());
                    result.SetValue("LinkUrl" + (i + 1), fo.GetDefaultValue(String.Empty, (IsMobile ? tagDefItem.GetValue("MobileLinkUrl").Trim() : tagDefItem.GetValue("LinkUrl").Trim()), this.DataSource));
                }
            }
            return result;
        }
    }
}
