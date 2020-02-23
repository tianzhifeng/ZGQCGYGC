using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Formula;
using Base.Logic.Domain;
using HR.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class TravelRecordPersonController : OfficeAutoFormContorllor<T_TravelRecord_Person>
    {
        //
        // GET: /OfficeAuto/AutoForm/TravelRecordPerson/

        /// <summary>
        /// 是否第六审批
        /// </summary>
        /// <returns></returns>
        public JsonResult IsSixSign()
        {
            var dic = new Dictionary<string, object>();

            dic["isSixSign"] = "No";

            var deptID = CurrentUserInfo.UserOrgID;

            var baseDB = FormulaHelper.GetEntities<BaseEntities>();

            var dept = baseDB.Set<S_A_Org>().Find(deptID);

            if (dept != null)
            {
                if (dept.Name == "公司领导" || dept.Name == "总经理助理")
                {
                    dic["isSixSign"] = "Yes";
                }
            }

            var hrDB = FormulaHelper.GetEntities<HREntities>();
            var employee = hrDB.Set<T_Employee>().Where(p => p.UserID == CurrentUserInfo.UserID).FirstOrDefault();
            if (employee != null)
            {
                if (employee.Employeelevel == "二级")
                {
                    dic["isSixSign"] = "Yes";
                }
            }

            return Json(dic);
        }


    }
}
