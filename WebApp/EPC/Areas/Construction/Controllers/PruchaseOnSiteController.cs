using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class PruchaseOnSiteController : EPCFormContorllor<T_C_PurchaseOnSite>
    {
        /// <summary>
        ///  验证是否有采购明细
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="formInfo"></param>
        /// <param name="isNew"></param>
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var pd = dic.GetValue("PurchaseDetail");
            if (string.IsNullOrEmpty(pd) || pd == "[]")
            {
                throw new Formula.Exceptions.BusinessValidationException("请填写采购明细！");
            }
        }

    }
}
