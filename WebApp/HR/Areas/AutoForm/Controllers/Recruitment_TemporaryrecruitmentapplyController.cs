using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;

namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_TemporaryrecruitmentapplyController : HRFormContorllor<T_Recruitment_Temporaryrecruitmentapply>
    {
        // GET: /AutoForm/Recruitment_Temporaryrecruitmentapply/

        protected override void OnFlowEnd(T_Recruitment_Temporaryrecruitmentapply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            //写入到核心表中
            var year = entity.year;
            var type = entity.Recruitmenttype;
            var id = entity.ID;
            var currentUser = FormulaHelper.GetUserInfo();

            if (type == "校招")
            {
                var xz = BusinessEntities.Set<T_Recruitment_Temporaryrecruitmentapply_Schoolrecruitmentdemandplan>().Where(p => p.T_Recruitment_TemporaryrecruitmentapplyID == id);

                foreach (var item in xz)
                {
                    var sentity = new S_E_Recruitment_DeptRequirementReport()
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Major = item.Major,
                        Year = year,
                        Dept = entity.Needdepartment,
                        Recruitment_DeptRequirementReportID = id,
                        Number = item.Number,
                        CreateUserID = currentUser.UserID,
                        CreateUser = currentUser.UserName,
                        CreateDate = DateTime.Now,
                        DeptName = entity.NeeddepartmentName,
                        Type = "Temp",
                    };

                    BusinessEntities.Set<S_E_Recruitment_DeptRequirementReport>().Add(sentity);
                }

                BusinessEntities.SaveChanges();
            }

            if (type == "社会招聘")
            {
                var sz = BusinessEntities.Set<T_Recruitment_Temporaryrecruitmentapply_Socialrecruitmentdemandplan>().Where(p => p.T_Recruitment_TemporaryrecruitmentapplyID == id);

                foreach (var item in sz)
                {
                    var sentity = new S_E_Recruitment_DeptRequirementReport_SZRCXQJH()
                    {
                        ID = FormulaHelper.CreateGuid(),
                        PostName = item.PostName,
                        Major = item.Mjor,
                        Number = item.Number,
                        Responsibilities = item.Responsibilities,
                        Qualification = item.Qualification,
                        SalaryRange = item.SalaryRange,
                        TargetUnit = item.TargetUnit,
                        TargetUnitName = item.TargetUnitName,
                        Remarks = item.Remarks,
                        T_Recruitment_DeptRequirementReportID = id,
                        CreateUserID = currentUser.UserID,
                        CreateUser = currentUser.UserName,
                        CreateDate = DateTime.Now,
                        Dept = entity.Needdepartment,
                        DeptName = entity.NeeddepartmentName,
                        Year = year,
                        Type = "Temp"
                    };

                    BusinessEntities.Set<S_E_Recruitment_DeptRequirementReport_SZRCXQJH>().Add(sentity);
                }
            }
        }
      

    }
}
