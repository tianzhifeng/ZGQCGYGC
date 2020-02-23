using EPC;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class CompletedController : EPCFormContorllor<T_C_CompletedReturn>
    {
        //
        // GET: /Procurement/Completed/
        //public override bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        //{
        //    var entity = this.GetEntityByID<T_C_CompletedReturn>(taskExec.S_WF_InsFlow.FormInstanceID);
        //    if (routing.Code == "End")
        //    {
        //      var newprojectid=CreateNewProject(entity.Project);
        //        CreateNewOBS(entity.Project, newprojectid,entity.ChargeUser);
        //    }
        //    return base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
        //}
        protected override void OnFlowEnd(T_C_CompletedReturn entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            //if (entity != null) {
            //    var newprojectid = CreateNewProject(entity.Project);
            //    if (newprojectid != "")
            //    {
            //        var engineeringInfo = FormulaHelper.GetEntities<EPCEntities>().Set<S_I_Engineering>().Find(newprojectid);
            //            //entities.S_I_Engineering.Find(newprojectid);
            //        engineeringInfo.SetOBSUser("ProjectManager", entity.ChargeUser);
            //        engineeringInfo.ChargerUser = entity.ChargeUser;
            //        engineeringInfo.ChargerUserName = entity.ChargeUserName;
            //        engineeringInfo.initModeCode();
            //        engineeringInfo.InitWBS();
            //        engineeringInfo.InitFolderTemplate();
            //        CreadPBSRoot(newprojectid);
            //        //CreateNewOBS(entity.Project, newprojectid, entity.ChargeUser);
            //    }
            //}
            base.OnFlowEnd(entity, taskExec, routing);
        }


        public string CreateNewProject(string EngineeringID) {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            var isOver = entity.Set<S_I_Engineering>().Where(c => c.OldEngineeringID == EngineeringID).ToList();
            var newid = "";
            if (isOver.Count == 0)
            {
                S_I_Engineering oldProject = entity.Set<S_I_Engineering>().FirstOrDefault(c => c.ID == EngineeringID);
                S_I_Engineering newProject = new S_I_Engineering();
                 newid = FormulaHelper.CreateGuid();
                newProject.ID = newid;
                newProject.OldEngineeringID = EngineeringID;
                newProject.ContractMode = "AfterSale";
                newProject.CreateDate = DateTime.Now;
                newProject.CreateUser = FormulaHelper.GetUserInfo().UserName;
                newProject.CreateUserID = FormulaHelper.GetUserInfo().UserID;

                newProject.Name = oldProject.Name+"(售后项目)";
                newProject.OrgID = oldProject.OrgID;
                newProject.OtherDept = oldProject.OtherDept;
                newProject.OtherDeptName = oldProject.OtherDeptName;
                //newProject.PBSRoot = oldProject.PBSRoot;
                newProject.PhaseValue = oldProject.PhaseValue;
                newProject.ProductionValue = oldProject.ProductionValue;
                newProject.ProjectClass = oldProject.ProjectClass;
                newProject.ProjectContent = oldProject.ProjectContent;
                newProject.ProjectPhase = oldProject.ProjectPhase;
                newProject.ProjectPreiod = oldProject.ProjectPreiod;
                newProject.ProjectScale = oldProject.ProjectScale;
                newProject.Remark = oldProject.Remark;
                newProject.SalePeriod = oldProject.SalePeriod;
                newProject.SerialNumber = oldProject.SerialNumber;
                newProject.State = oldProject.State;
                newProject.Status = oldProject.Status;
                newProject.StepName = oldProject.StepName;
                newProject.SuccessRate = oldProject.SuccessRate;
                newProject.Advantage = oldProject.Advantage;
                newProject.Attachment = oldProject.Attachment;
                newProject.BiddingCharger = oldProject.BiddingCharger;
                newProject.BiddingChargerName = oldProject.BiddingChargerName;
                newProject.BuildFormID = oldProject.BuildFormID;
                newProject.BuildType = oldProject.BuildType;
                newProject.BusinessCharger = oldProject.BusinessCharger;
                newProject.BusinessChargerName = oldProject.BusinessChargerName;
                //newProject.CBSRoot = oldProject.CBSRoot;
                newProject.ChargerDept = oldProject.ChargerDept;
                newProject.ChargerDeptName = oldProject.ChargerDeptName;
                newProject.ChargerUser = oldProject.ChargerUser;
                newProject.ChargerUserName = oldProject.ChargerUserName;
                newProject.CompanyID = oldProject.CompanyID;
                newProject.Competition = oldProject.Competition;
                //newProject.ConstructionRoot = oldProject.ConstructionRoot;

                newProject.CurrentPhase = oldProject.CurrentPhase;
                newProject.CustomerAddress = oldProject.CustomerAddress;
                newProject.CustomerCode = oldProject.CustomerCode;
                newProject.CustomerInfo = oldProject.CustomerInfo;
                newProject.CustomerInfoName = oldProject.CustomerInfoName;
                //newProject.DeviceRoot = oldProject.DeviceRoot;
                newProject.ExaminePerformance = oldProject.ExaminePerformance;
                newProject.FlowInfo = oldProject.FlowInfo;
                newProject.FlowPhase = oldProject.FlowPhase;
                newProject.Investment = oldProject.Investment;
                newProject.IsInvestigate = oldProject.IsInvestigate;
                newProject.IsInvestigateDate = oldProject.IsInvestigateDate;
                newProject.IsInvestigateProject = oldProject.IsInvestigateProject;
                //newProject.LaborRoot = oldProject.LaborRoot;
                //newProject.Mode = oldProject.Mode;
                newProject.ModeCode = "EPCAfterSale";
                newProject.ModifyDate = oldProject.ModifyDate;
                newProject.ModifyUser = oldProject.ModifyUser;
                newProject.ModifyUserID = oldProject.ModifyUserID;
                entity.Set<S_I_Engineering>().Add(newProject);
                entity.SaveChanges();
            }
            return newid;
        }

        public void CreateNewOBS(string EngineeringID,string NewEngineeringID,string userid) {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            var oldOBS = entity.Set<S_I_OBS>().FirstOrDefault(c => c.EngineeringInfoID == EngineeringID&&c.ParentID=="");
              CreadOBS(oldOBS, "", "",NewEngineeringID);
            CreadOBSUser(NewEngineeringID, userid);
        }
        public void CreadOBS(S_I_OBS oldOBS,string ParentID,string FullID,string NewEngineeringID) {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            var newobs = new S_I_OBS();
            var newid = FormulaHelper.CreateGuid();
            newobs.ID = newid;
            newobs.EngineeringInfoID = NewEngineeringID;
            newobs.Name = oldOBS.Name;
            newobs.NodeType = oldOBS.NodeType;
            newobs.OBSLevel = oldOBS.OBSLevel;
            newobs.ParentID = ParentID;
            newobs.Scope = oldOBS.Scope;
            newobs.SortIndex = oldOBS.SortIndex;
            newobs.Code = oldOBS.Code;
            newobs.FullID = FullID==""?newid: FullID+","+ newid;
            entity.Set<S_I_OBS>().Add(newobs);
            var ob = entity.Set<S_I_OBS>().Where(c => c.ParentID == oldOBS.ID).ToList();
            if (ob.Count > 0)
            {
                foreach (S_I_OBS item in ob)
                {
                    CreadOBS(item, newobs.ID, newobs.FullID, NewEngineeringID);
                }
            }
            entity.SaveChanges();
        }

        public void CreadOBSUser(string NewEngineeringID,string UserID) {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            var OBSmodel = entity.Set<S_I_OBS>().FirstOrDefault(c => c.EngineeringInfoID == NewEngineeringID && c.Name == "项目经理");
            var userInfo = FormulaHelper.GetUserInfoByID(UserID);
            S_I_OBS_User newOBSUser = new S_I_OBS_User();
            newOBSUser.ID = FormulaHelper.CreateGuid();
            newOBSUser.OBSID = OBSmodel.ID;
            newOBSUser.EngineeringInfoID = NewEngineeringID;
            newOBSUser.ProjectInfoID = NewEngineeringID;
            newOBSUser.UserID = UserID;
            newOBSUser.UserName = userInfo.UserName;
            newOBSUser.RoleCode = "ProjectManager";
            newOBSUser.RoleName = "项目经理";
            newOBSUser.DeptName = userInfo.UserOrgName;
            newOBSUser.DeptInfo = userInfo.UserOrgID;
            newOBSUser.InTime = DateTime.Now;
            entity.Set<S_I_OBS_User>().Add(newOBSUser);
            entity.SaveChanges();
        }

        public void CreadPBSRoot(string NewProjectID) {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            var pbs=entity.Set<S_I_PBS>();
            var project = entity.Set<S_I_Engineering>().Find(NewProjectID);
            var modelid= FormulaHelper.GetEntities<InfrastructureEntities>().Set<S_C_Mode>().FirstOrDefault(c => c.Code== "EPCAfterSale").ID;
            if (NewProjectID != "") {
                S_I_PBS modelp = new S_I_PBS();
                modelp.ID = FormulaHelper.CreateGuid();
                modelp.EngineeringInfoID = NewProjectID;
                modelp.Name = project.Name;
                modelp.Code = project.SerialNumber;
                modelp.NodeType = "Root";
                modelp.ParentID = "";
                modelp.FullID = modelp.ID;
                modelp.ToWBS = "false";
                modelp.StructNodeID = FormulaHelper.GetEntities<InfrastructureEntities>().Set<S_C_PBSStruct>().FirstOrDefault(c=>c.Code=="Root"&&c.ModeID==modelid).ID;
                modelp.SortIndex = null;
                modelp.RelateID = null;
                modelp.Remark = null;
                pbs.Add(modelp);
                entity.SaveChanges();
            }
            
        }
    }
}
