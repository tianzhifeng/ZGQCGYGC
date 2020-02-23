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
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Bidding.Controllers
{
    public class BidReviewController : EPCFormContorllor<T_M_BidReview>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            S_I_Engineering engineering = EPCEntites.Set<S_I_Engineering>().Find(dic.GetValue("EngineeringInfoID"));
            if (engineering.State != ProjectState.Bid.ToString())
            { throw new Formula.Exceptions.BusinessValidationException("已经完成立项的项目不能再进行投标文件评审"); }
        }
    }
}
