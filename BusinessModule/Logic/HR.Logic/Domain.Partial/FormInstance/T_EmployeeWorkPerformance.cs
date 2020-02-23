using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace HR.Logic.Domain
{
    public partial class T_EmployeeWorkPerformance
    {
        public void SynchEmpPerformance(string userID, S_P_Performance performance, string role)
        {
            this.ID = FormulaHelper.CreateGuid();
            this.Name = performance.Name;
            this.Code = performance.Code;
            this.ProjectClass = performance.ProjectClass;
            this.ProjectLevel = performance.ProjectLevel;
            this.ProjectState = performance.ProjectState;
            this.PlanStartDate = performance.PlanStartDate;
            this.FactFinishDate = performance.FactFinishDate;
            this.UserID = userID;
            this.RelateID = performance.ID;
            this.ProjectRole = role;

            var hrEntity = FormulaHelper.GetEntities<HREntities>();
            var empInfo = hrEntity.Set<T_Employee>().FirstOrDefault(d => d.UserID == userID && d.IsDeleted == "0");
            if (empInfo != null)
                this.EmployeeID = empInfo.ID;
        }
    }
}
