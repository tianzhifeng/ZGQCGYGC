using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Formula.Exceptions;
using Formula;

namespace HR.Areas.AutoForm.Controllers
{
    public class ForeignGoabroadpersonalController : HRFormContorllor<T_Foreign_Goabroadpersonal>
    {

        // GET: /AutoForm/ForeignGoabroadpersonal/

        public JsonResult GetUserInfo(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                throw new BusinessException("用户不存在");
            }

            var employee = BusinessEntities.Set<T_Employee>().Where(c => c.UserID == UserId).FirstOrDefault();
            if (employee == null)
            {
                throw new BusinessException("用户不存在");
            }

            return Json(employee);
        }

        protected override void OnFlowEnd(T_Foreign_Goabroadpersonal entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {

            var mod = new S_E_Peerlist();
            mod.ID = FormulaHelper.CreateGuid();
            mod.ApplyDate = entity.Applytime;
            mod.ApplyType = "因私出国";
            mod.T_Foreign_GoabroadapplybusinessID = entity.ID;
            mod.TaskName = entity.Position;
            mod.UserID = entity.ApplyUser;
            mod.UserName = entity.ApplyUserName;
            mod.DeptID = entity.Appledept;
            mod.DeptName = entity.AppledeptName;
            mod.Code = entity.ApplyCode;
            mod.Gocountry = entity.Country;

            BusinessEntities.Set<S_E_Peerlist>().Add(mod);

            BusinessEntities.SaveChanges();
        }

    }
}
