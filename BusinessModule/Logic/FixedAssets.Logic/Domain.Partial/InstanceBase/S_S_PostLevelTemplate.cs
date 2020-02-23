using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Config.Logic;

namespace FixedAssets.Logic.Domain
{
    public partial class S_S_PostLevelTemplate
    {

        //public void Validate(Dictionary<string, string> detailItem)
        //{
        //    var postCode = detailItem.GetValue("PostCode");
        //    var postLevelCode = detailItem.GetValue("PostLevelCode");
        //    var belongYear = detailItem.GetValue("BelongYear");
        //    var belongMonth = detailItem.GetValue("BelongMonth");
        //    var HrEntities = FormulaHelper.GetEntities<HREntities>();
        //    var existedForms = HrEntities.Set<S_S_PostLevelTemplate>().Where(c => c.TemplateCode == this.TemplateCode && c.ID != this.ID).ToList();
        //    if (existedForms != null && existedForms.Count > 0) throw new Formula.Exceptions.BusinessException("编号为【" + this.TemplateCode + "】的模板已经存在，请修改。");
        //    /* var postList = HrEntities.Set<S_S_PostLevelTemplate_PostList>().Where(c => c.S_S_PostLevelTemplateID != this.ID && c.PostCode == postCode
        //         && c.PostLevelCode == postLevelCode && c.BelongYear == belongYear && c.BelongMonth == belongMonth).ToList();
        //     if (postList != null && postList.Count > 0)
        //     {
        //         var singleItem = postList[0];
        //         var existedPostLevelTemplate = HrEntities.Set<S_S_PostLevelTemplate>().FirstOrDefault(c => c.ID == singleItem.S_S_PostLevelTemplateID);
        //         if (existedPostLevelTemplate == null) throw new Formula.Exceptions.BusinessException("岗位岗级模板表中存在垃圾数据，请联系管理员");
        //         throw new Formula.Exceptions.BusinessException("岗位为【" + detailItem.GetValue("PostName") + "】且岗级为【" + detailItem.GetValue("PostLevelName")
        //             + "】的记录已经存在于名称为【" + existedPostLevelTemplate.TemplateName + "】的模板中");

        //     }*/
        //}

    }
}
