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
using Comprehensive.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class MemberShcmeaController : EPCFormContorllor<T_I_MemberShcmea>
    {
        protected override void OnFlowEnd(T_I_MemberShcmea entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();

                //SetWorkPerformance(entity);
            }
        }

        public void SetWorkPerformance(T_I_MemberShcmea entity) {
            var epcent=FormulaHelper.GetEntities<EPCEntities>();
            var hrent = FormulaHelper.GetEntities<ComprehensiveEntities>();
            var projectent = epcent.Set<S_I_Engineering>().FirstOrDefault(c=>c.ID==entity.EngineeringInfoID);
            var entlist = epcent.Set<T_I_MemberShcmea_MemberInfo>().Where(c => c.T_I_MemberShcmeaID == entity.ID).ToList();
            if (entlist.Count > 0) {
                foreach (T_I_MemberShcmea_MemberInfo item in entlist) {
                    string employeeID = "";
                    string employeeName = "";
                    var employee = hrent.Set<S_HR_Employee>().FirstOrDefault(c => c.UserID == item.UserInfo);
                    if (employee != null)
                    {
                        employeeID = employee.ID;
                        employeeName = employee.Name;
                    }
                    S_HR_EmployeeWorkPerformance ewp = new S_HR_EmployeeWorkPerformance();
                    ewp.ID = FormulaHelper.CreateGuid();
                    ewp.EmployeeID = employeeID;
                    ewp.Code = entity.EngineerInfoCode;
                    ewp.Name = entity.EngineerInfoName;
                    ewp.ProjectClass = projectent.ProjectClass;
                    ewp.ProjectLevel = projectent.ProjectScale;
                    ewp.PlanStartDate = item.PlanInDate;
                    ewp.ProjectState = projectent.State;
                    ewp.ProjectRole = item.Postion;
                    hrent.Set<S_HR_EmployeeWorkPerformance>().Add(ewp);
                    hrent.SaveChanges();
                }
            }
        }
    }
}
