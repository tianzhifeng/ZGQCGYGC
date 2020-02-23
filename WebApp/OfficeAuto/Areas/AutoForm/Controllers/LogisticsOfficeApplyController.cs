using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Formula;
using Config;
using Formula.Exceptions;
using MvcAdapter;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class LogisticsOfficeApplyController : OfficeAutoFormContorllor<T_Logistics_OfficeApply>
    {
        //
        // GET: /OfficeAuto/AutoForm/LogisticsOfficeApply/


        protected override void OnFlowEnd(T_Logistics_OfficeApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var dept = entity.ApplyDept;
            var deptName = entity.ApplyDeptName;

            var type = entity.ApplyType;

            if (type == "Add")
            {
                var office = entity.NewOfficeName;

                var officeEntity = BusinessEntities.Set<T_Logistics_Office>().Find(office);

                if (officeEntity != null)
                {
                    officeEntity.CurrentUseDept = dept;
                    officeEntity.CurrentUseDeptName = deptName;
                    officeEntity.UseStatus = "Use";

                    //新增记录
                    var record = new S_E_Logistics_OfficeRecord()
                    {
                        ID = FormulaHelper.CreateGuid(),
                        CreateDate = DateTime.Now,
                        CreateUser = CurrentUserInfo.UserName,
                        CreateUserID = CurrentUserInfo.UserID,
                        CompanyID = CurrentUserInfo.UserCompanyID,
                        OrgID = CurrentUserInfo.UserOrgID,

                        Dept = dept,
                        DeptName = deptName,
                        Office = officeEntity.ID,
                        OfficeName = officeEntity.Name,
                        Pos = officeEntity.Pos,
                        PosName = officeEntity.PosName,
                        Area = officeEntity.Area,
                        Number = officeEntity.Number,
                        StartDate = entity.StartDate
                    };

                    BusinessEntities.Set<S_E_Logistics_OfficeRecord>().Add(record);
                    BusinessEntities.SaveChanges();
                }
            }


            if (type == "Back")
            {
                var office = entity.BackOfficeName;
                var officeEntity = BusinessEntities.Set<T_Logistics_Office>().Find(office);

                if (officeEntity != null)
                {
                    officeEntity.CurrentUseDept = string.Empty;
                    officeEntity.CurrentUseDeptName = string.Empty;
                    officeEntity.UseStatus = "NoUse";


                    var record = BusinessEntities.Set<S_E_Logistics_OfficeRecord>().Where(p => p.Dept == dept && p.EndDate == null && p.Office == officeEntity.ID).FirstOrDefault();

                    record.EndDate = entity.BackDate;

                    BusinessEntities.SaveChanges();
                }
            }

            if (type == "Change")
            {
                var oldOffice = entity.Change_OldOfficeName;
                var newOffice = entity.Change_NewOfficeName;
                //退
                var oldEntity = BusinessEntities.Set<T_Logistics_Office>().Find(oldOffice);
                if (oldEntity != null)
                {
                    oldEntity.CurrentUseDept = string.Empty;
                    oldEntity.CurrentUseDeptName = string.Empty;
                    oldEntity.UseStatus = "NoUse";

                    var record = BusinessEntities.Set<S_E_Logistics_OfficeRecord>().Where(p => p.Dept == dept && p.EndDate == null && p.Office == oldEntity.ID).FirstOrDefault();
                    record.EndDate = entity.BackDate;

                }
                //增
                var newEntity = BusinessEntities.Set<T_Logistics_Office>().Find(newOffice);
                if (newEntity != null)
                {
                    newEntity.CurrentUseDept = dept;
                    newEntity.CurrentUseDeptName = deptName;
                    newEntity.UseStatus = "Use";

                    //新增记录
                    var record = new S_E_Logistics_OfficeRecord()
                    {
                        ID = FormulaHelper.CreateGuid(),
                        CreateDate = DateTime.Now,
                        CreateUser = CurrentUserInfo.UserName,
                        CreateUserID = CurrentUserInfo.UserID,
                        OrgID = CurrentUserInfo.UserOrgID,

                        Dept = dept,
                        DeptName = deptName,
                        Office = newEntity.ID,
                        OfficeName = newEntity.Name,
                        Pos = newEntity.Pos,
                        PosName = newEntity.PosName,
                        Area = newEntity.Area,
                        Number = newEntity.Number,
                        StartDate = entity.StartDate
                    };

                    BusinessEntities.Set<S_E_Logistics_OfficeRecord>().Add(record);
                }

                BusinessEntities.SaveChanges();
            }

        }
    }
}
