using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;

namespace HR.Areas.AutoForm.Controllers
{
    public class UserAptitudeApplyController : HRFormContorllor<T_D_UserAptitudeApply>
    {
        protected override void OnFlowEnd(T_D_UserAptitudeApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            foreach (var row in entity.T_D_UserAptitudeApply_ApplyInfo)
            {
                var userInfo = FormulaHelper.GetUserInfoByID(row.UserID);
                var employee = this.BusinessEntities.Set<T_Employee>().FirstOrDefault(d => d.UserID == row.UserID);
                string employeeID = "";
                if (employee != null) employeeID = employee.ID;

                var userAptitudeInfo = new S_D_UserAptitudeApply()
                {
                    ID = FormulaHelper.CreateGuid(),
                    ProjectInfoID = entity.ProjectID,
                    ProjectInfoName = entity.ProjectIDName,
                    UserAptitudeApplyID = entity.ID,
                    UserID = row.UserID,
                    UserName = row.UserIDName,
                    DeptID = userInfo.UserOrgID,
                    DeptName = userInfo.UserOrgName,
                    Major = row.Major,
                    ProjectClass = row.ProjectClass,
                    Position = row.Position,
                    AptitudeLevel = row.AptitudeLevel ?? 0,
                    HREmployeeID = employeeID
                };
                this.BusinessEntities.Set<S_D_UserAptitudeApply>().Add(userAptitudeInfo);
            }

            this.BusinessEntities.SaveChanges();
            base.OnFlowEnd(entity, taskExec, routing);
        }

    }
}
