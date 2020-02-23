using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic;
using OfficeAuto.Logic.Domain;
using Config;

namespace OfficeAuto.Areas.OfficialDoc.Controllers
{
    public class SendFaxController : BaseFlowController<T_B_SendFaxManagement>
    {
        protected override void EntityCreateLogic<TEntity>(TEntity entity)
        {
            base.EntityCreateLogic(entity);
            if (entity is T_B_SendFaxManagement)
            {
                var obj = entity as T_B_SendFaxManagement;
                obj.FlowPhase = SendFaxStatus.Register.ToString();
            }
        }
        public override JsonResult GetModel(string id)
        {
            T_B_SendFaxManagement model = GetEntity<T_B_SendFaxManagement>(id);
            if (string.IsNullOrEmpty(id))
            {
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.SendUserName = user.UserName;
                model.SendUserID = user.UserID;
                model.SendDept = user.UserOrgName;
                model.SendDeptID = user.UserOrgID;
            }
            return Json(model);
        }
    }
}
