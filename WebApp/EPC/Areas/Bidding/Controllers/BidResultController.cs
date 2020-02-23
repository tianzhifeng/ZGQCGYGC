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
    public class BidResultController : EPCFormContorllor<S_M_BidResult>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            S_I_Engineering engineering = EPCEntites.Set<S_I_Engineering>().Find(dic.GetValue("EngineeringInfoID"));
            if (engineering.State != ProjectState.Bid.ToString())
            { throw new Formula.Exceptions.BusinessValidationException("已经完成立项的项目不能修改投标结果"); }
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dic, isNew, upperVersionID);
            var eID = dic.GetValue("EngineeringInfoID");
            S_M_BidResult result = EPCEntites.Set<S_M_BidResult>().FirstOrDefault(a => a.EngineeringInfoID == eID);
            if(result == null)
                throw new Formula.Exceptions.BusinessValidationException("找不到该投标结果登记");
            var sourceDic = result.ToDic();
            foreach (var tmp in sourceDic)
            {
                dic.SetValue(tmp.Key, sourceDic.GetValue(tmp.Key));
            }
        }
    }
}
