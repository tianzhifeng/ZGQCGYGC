using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Formula;
using Base.Logic.Domain;
using Config;
using Config.Logic;

namespace OfficeAuto.Areas.OfficialDocument.Controllers
{
    public class IncomingController : OfficeAutoFormContorllor<S_D_Incoming>
    {
        // GET: /OfficialDocument/Incoming/

        public JsonResult GetOneStep()
        {
            var isTwo = "0";
            var dept = "";


            var currentUser = FormulaHelper.GetUserInfo();

            var currentOrg = currentUser.UserOrgID;

            var BaseEntities = FormulaHelper.GetEntities<BaseEntities>();

            S_A_Org finalOrg = new S_A_Org();

            var org = BaseEntities.Set<S_A_Org>().Where(p => p.ID == currentOrg).FirstOrDefault();

            if (org == null )
            {
                throw new Formula.Exceptions.BusinessException("不存在的组织");
            }

            if (org.Character == "分支机构")
            {
                isTwo = "1";
                finalOrg = org;
            }

            if (isTwo != "1")
            {
                var parentOrg = BaseEntities.Set<S_A_Org>().Where(p => p.ID == org.ParentID).FirstOrDefault();

                if (parentOrg != null && parentOrg.Character == "分支机构")
                {
                    isTwo = "1";
                    finalOrg = parentOrg;
                }
            }


            if (isTwo == "1")
            {
                var o = BaseEntities.Set<S_A_Org>().Where(p => p.ParentID == finalOrg.ID && p.Code == "ZHB").FirstOrDefault();
                dept = o.ID;
            }

            var dic = new Dictionary<string, object>();
            dic.SetValue("IsTwo", isTwo);
            dic.SetValue("Dept", dept);

            return Json(dic);
        }


    }
}
