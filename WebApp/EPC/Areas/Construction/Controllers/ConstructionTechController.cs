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
    public class ConstructionTechController : EPCFormContorllor<T_C_ConstructionTech>
    {
        /// <summary>
        ///  判断子表是否有数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="formInfo"></param>
        /// <param name="isNew"></param>
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var cp = dic.GetValue("ComPersonnel");
            var rr = dic.GetValue("ReviewRecords");
            if (string.IsNullOrEmpty(cp)|| cp=="[]")
            {
                throw new Formula.Exceptions.BusinessValidationException("请填写参加单位及人员！");
            }
            if (string.IsNullOrEmpty(rr) || rr == "[]")
            {
                throw new Formula.Exceptions.BusinessValidationException("请填写会审记录！");
            }
        }

    }
}
