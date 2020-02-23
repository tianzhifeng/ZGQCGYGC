using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;
using MvcAdapter;
using System.Data;
using Formula.Helper;
using Config.Logic;
using Config;


namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_DeptRequirementReportController : HRFormContorllor<T_Recruitment_DeptRequirementReport>
    {
        // GET: /AutoForm/Recruitment_DeptRequirementReport/
        protected override void OnFlowEnd(T_Recruitment_DeptRequirementReport entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var year = entity.Year;
            var type = entity.RecruitmentType;
            var id = entity.ID;
            var currentUser = FormulaHelper.GetUserInfo();

            if (type == "校招")
            {
                var xz = BusinessEntities.Set<T_Recruitment_DeptRequirementReport_XZRCXQJH>().Where(p => p.T_Recruitment_DeptRequirementReportID == id);

                foreach (var item in xz)
                {
                    var sentity = new S_E_Recruitment_DeptRequirementReport()
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Major = item.Major,
                        Year = year,
                        Dept = entity.RequireDept,
                        Recruitment_DeptRequirementReportID = id,
                        Number = item.Number,
                        CreateUserID = currentUser.UserID,
                        CreateUser = currentUser.UserName,
                        CreateDate = DateTime.Now,
                        DeptName = entity.RequireDeptName,
                        Type = "Normal",
                    };

                    BusinessEntities.Set<S_E_Recruitment_DeptRequirementReport>().Add(sentity);
                }

                BusinessEntities.SaveChanges();
            }

            if (type == "社会招聘")
            {
                var sz = BusinessEntities.Set<T_Recruitment_DeptRequirementReport_SZRCXQJH>().Where(p => p.T_Recruitment_DeptRequirementReportID == id);

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
                        Dept = entity.RequireDept,
                        DeptName = entity.RequireDeptName,
                        Year = year,
                        Type = "Normal",
                    };

                    BusinessEntities.Set<S_E_Recruitment_DeptRequirementReport_SZRCXQJH>().Add(sentity);
                }
            }
        }


        
    }
}
