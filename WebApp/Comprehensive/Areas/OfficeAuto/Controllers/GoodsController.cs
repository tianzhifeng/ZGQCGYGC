using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Comprehensive.Logic.Domain;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class GoodsController : ComprehensiveFormController<S_G_GoodsInfo>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (!isNew)
            {
                var id = dic["ID"];
                //修改后初始数量
                int newQuantity = int.Parse(dic["Quantity"]);
                //追加数量
                var addGoodsInfo = this.ComprehensiveDbContext.Set<S_G_GoodsAdditional>().Where(d => d.Goods == id).ToList();
                var additionalQuantity = addGoodsInfo.Sum(d => d.Quantity);
                additionalQuantity = additionalQuantity.HasValue ? additionalQuantity.Value : 0;
                //领用数量
                var applyInfo = this.ComprehensiveDbContext.Set<T_G_GoodsApply_ApplyDetail>().Where(d => d.Goods == id).ToList();
                var applyQuantity = applyInfo.Sum(d => d.Quantity);
                applyQuantity = applyQuantity.HasValue ? applyQuantity.Value : 0;

                if (newQuantity + additionalQuantity - applyQuantity < 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("编辑数量不可低于已领取数量【" + applyQuantity + "】！");
                }
            }
        }
    }
}
