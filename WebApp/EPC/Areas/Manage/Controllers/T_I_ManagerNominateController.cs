using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;

namespace EPC.Areas.Manage.Controllers
{
    public class ManagerNominateController : EPCFormContorllor<T_I_ManagerNominate>
    {
        protected override void OnFlowEnd(T_I_ManagerNominate entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();

                #region 生成项目仓库
                var engineeringInfo = EPCEntites.Set<S_I_Engineering>().Find(entity.EngineeringInfoID);
                if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息");
                var storage = EPCEntites.Set<S_W_StorageInfo>().FirstOrDefault(a => a.Engineering == engineeringInfo.ID);
                if (storage == null)
                {
                    storage = new S_W_StorageInfo
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Name = engineeringInfo.Name,
                        Type = StorageType.Project.ToString(),
                        Engineering = engineeringInfo.ID,
                        EngineeringName = engineeringInfo.Name
                    };
                    EntityCreateLogic<S_W_StorageInfo>(storage);
                    EPCEntites.Set<S_W_StorageInfo>().Add(storage);
                }
                #endregion 

                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult ValidateNominate(string ID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(ID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的项目信息，无法立项");
            var result = new Dictionary<string, object>();
            var form=this.EPCEntites.Set<T_I_ManagerNominate>().FirstOrDefault(d=>d.EngineeringInfoID==ID);
            if (form!=null)
           {
               result.SetValue("FormID", form.ID);
           }
            else
            {
                result.SetValue("FormID", "");
            }
            return Json(result);
        }
    }
}
