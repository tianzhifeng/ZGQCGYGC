using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Project.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class PerformanceController : HRFormContorllor<S_P_Performance>
    {

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new S_P_Performance();
            this.UpdateEntity(entity, dic);
            if (!isNew)
            {
                this.BusinessEntities.Set<T_EmployeeWorkPerformance>().Delete(d => d.RelateID == entity.ID);
                this.BusinessEntities.SaveChanges();
            }
            this.SynchEmpPerformance(entity.ChargeUser, entity, AuditRoles.ProjectManager.ToString());
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new S_P_Performance();
            this.UpdateEntity(entity, dic);

            var role = AuditRoles.MajorPrinciple.ToString();
            this.SynchEmpPerformance(entity, role, detail);

            //设计人
            role = AuditRoles.Designer.ToString();
            this.SynchEmpPerformance(entity, role, detail);

            //校核人
            role = AuditRoles.Collactor.ToString();
            this.SynchEmpPerformance(entity, role, detail);

            //审核人
            role = AuditRoles.Auditor.ToString();
            this.SynchEmpPerformance(entity, role, detail);

            //审定人
            role = AuditRoles.Approver.ToString();
            this.SynchEmpPerformance(entity, role, detail);
        }


        private void SynchEmpPerformance(S_P_Performance performance, string role, Dictionary<string, string> detail)
        {
            if (detail.Keys.Contains(role) && !string.IsNullOrEmpty(detail[role]))
            {
                var userIDs = detail[role].Split(',').ToList();
                foreach (var userID in userIDs)
                    if (!string.IsNullOrEmpty(userID))
                        this.SynchEmpPerformance(userID, performance, role);
            }
        }

        private void SynchEmpPerformance(string userID, S_P_Performance performance, string role)
        {
            var workPerformance = this.BusinessEntities.Set<T_EmployeeWorkPerformance>().FirstOrDefault(d => d.RelateID == performance.ID && d.UserID == userID);
            if (workPerformance == null)
            {
                workPerformance = new T_EmployeeWorkPerformance();
                workPerformance.SynchEmpPerformance(userID, performance, role);
                this.BusinessEntities.Set<T_EmployeeWorkPerformance>().Add(workPerformance);
            }
            else
                workPerformance.ProjectRole += "," + role;

            this.BusinessEntities.SaveChanges();
        }

        public void DeletePerformance(string Ids)
        {
            var rowIDs = Ids.Split(',').ToList();
            this.BusinessEntities.Set<T_EmployeeWorkPerformance>().Delete(d => rowIDs.Contains(d.RelateID));
            this.BusinessEntities.SaveChanges();
        }
    }
}
