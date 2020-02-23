using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using System.Data;
using Workflow.Logic.Domain;
using Config;
using Config.Logic;
using System.Reflection;
namespace Project.Logic.Domain
{
    public partial class T_EXE_ChangeAudit
    {

        public S_W_Activity CreateAcitivity(string key, string name, string displayName)
        {
            UserInfo user = FormulaHelper.GetUserInfo();
            var activity = new S_W_Activity();
            activity.ActvityName = name;
            activity.ActivityKey = key;
            activity.DisplayName = displayName;
            activity.CreateUser = user.UserName;
            activity.CreateUserID = user.UserID;
            activity.CreateDate = DateTime.Now;
            activity.AuditPatchID = this.ID;
            activity.BusniessID = this.ID;
            activity.ID = FormulaHelper.CreateGuid();
            activity.LinkUrl = "/Project/AutoUI/ChangeAuditView/PageView?TmplCode=ProjectExecutive_ChangeAudit&ID=" + this.ID;
            activity.OwnerUserID = user.UserID;
            activity.OwnerUserName = user.UserName;
            activity.ProjectInfoID = this.ProjectInfoID;
            activity.State = ProjectCommoneState.Finish.ToString();
            activity.FinishDate = DateTime.Now;
            activity.WBSID = this.WBSID;
            var projectEntities = this.GetDbContext<ProjectEntities>();
            projectEntities.S_W_Activity.Add(activity);
            return activity;
        }

        public void Publish()
        {
            //将该校审单对饮的Activity的状态之Finish
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var activities = projectEntities.Set<S_W_Activity>().Where(c => c.AuditPatchID == this.ID).ToList();
            var state = ProjectCommoneState.Finish.ToString();
            activities.Update(c => c.State = state);
            //this.State = AuditState.Pass.ToString();
            //this.CurrentTask = AuditState.Pass.ToString();
            //将关联的成果的状态置为校审完成
            this.SynchProductChangeAuditState(AuditState.Pass.ToString(), true);
            //反写成果的设校审人员信息
            this.SynchProductAuditUser();
            //将Mistate反写到S_W_Mistake表
            this.SynchMistake();
            //将新增成果反写值S_E_Product表
            //this.SynchCreateProduct();
            //设置工作包完成时间
            var wbs = projectEntities.S_W_WBS.FirstOrDefault(a => a.ID == this.WBSID);
            if (wbs != null && wbs.WBSType == WBSNodeType.Work.ToString())
            {
                foreach (var work in wbs.S_W_TaskWork.ToList())
                {
                    if (work.State != TaskWorkState.Finish.ToString())
                        work.Finish();
                    work.ChangeState = TaskWorkChangeState.AuditFinish.ToString();
                }
            }
            //自动归集成果资料
            var mapType = DBSType.Mapping.ToString();
            var productType = DBSMappingType.Product.ToString();
            var productFolders = projectEntities.Set<S_D_DBS>().Where(d => d.ProjectInfoID == ProjectInfoID && d.DBSType == mapType
                && d.MappingType == productType).ToList();
            foreach (var folder in productFolders)
            {
                folder.GatherProducts();
            }
        }

        public void SynchProductChangeAuditState(string auditState, bool isSynchAuditCompleteDate = false)
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var products = projectEntities.S_E_Product.Where(d => d.ChangeAuditID == this.ID).ToList();
            foreach (var item in products)
            {
                if (isSynchAuditCompleteDate)
                {
                    item.AuditPassDate = DateTime.Now;
                }
                item.AuditState = auditState;
                item.ChangeAuditID = this.ID;
                item.UpdateVersison();
            }
        }

        /// <summary>
        /// 设置设校审人信息
        /// </summary>
        public void SynchProductAuditUser()
        {
            var list = this.GetAuditSignUser();
            var strList = JsonHelper.ToJson(list);
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            //projectEntities.S_E_Product.Where(d => d.ChangeAuditID == this.ID).Update(d => d.AuditSignUser = strList);
            var designers = list.Where(a => a.ActivityKey == Project.Logic.AuditState.Design.ToString()
                || a.ActivityKey == Project.Logic.AuditState.Designer.ToString());
            var collactors = list.Where(a => a.ActivityKey == Project.Logic.AuditState.Collact.ToString()
                || a.ActivityKey == Project.Logic.AuditState.Collactor.ToString());
            var auditors = list.Where(a => a.ActivityKey == Project.Logic.AuditState.Audit.ToString()
                || a.ActivityKey == Project.Logic.AuditState.Auditor.ToString());
            var approvers = list.Where(a => a.ActivityKey == Project.Logic.AuditState.Approve.ToString()
                || a.ActivityKey == Project.Logic.AuditState.Approver.ToString());
            var products = projectEntities.S_E_Product.Where(d => d.ChangeAuditID == this.ID).ToList();
            foreach (var item in products)
            {
                item.AuditSignUser = strList;
                item.Designer = string.Join(",", designers.Select(a => a.UserID).Distinct());
                item.DesignerName = string.Join(",", designers.Select(a => a.UserName).Distinct());
                item.Collactor = string.Join(",", collactors.Select(a => a.UserID).Distinct());
                item.CollactorName = string.Join(",", collactors.Select(a => a.UserName).Distinct());
                item.Auditor = string.Join(",", auditors.Select(a => a.UserID).Distinct());
                item.AuditorName = string.Join(",", auditors.Select(a => a.UserName).Distinct());
                item.Approver = string.Join(",", approvers.Select(a => a.UserID).Distinct());
                item.ApproverName = string.Join(",", approvers.Select(a => a.UserName).Distinct());
                item.UpdateVersison();
            }
        }

        /// <summary>
        /// 同步错误信息
        /// </summary>
        public void SynchMistake()
        {
            var projectEntities = this.GetDbContext<ProjectEntities>();
            var mistakes = this.T_EXE_ChangeAudit_AdviceDetail;
            var majorEnum = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major).ToList().FirstOrDefault(d => d.Code == this.MajorCode);
            var projectInfo = projectEntities.S_I_ProjectInfo.Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("");
            if (mistakes != null && mistakes.Count() > 0)
            {
                foreach (var mistake in mistakes)
                {
                    var entity = projectEntities.Set<S_AE_Mistake>().Create();
                    if (string.IsNullOrEmpty(entity.ID))
                        entity.ID = FormulaHelper.CreateGuid();
                    entity.ProjectInfoID = this.ProjectInfoID;
                    entity.ChangeAuditID = this.ID;
                    entity.MistakeContent = mistake.MsitakeContent;
                    entity.AuditActivityType = mistake.Step;
                    entity.MistakeLevel = mistake.MistakeType;
                    if (mistake.MistakeType == "" || mistake.MistakeType == null)
                        entity.MistakeLevel = "OtherForNull";
                    entity.MajorCode = this.MajorCode;
                    if (majorEnum != null)
                        entity.MajorName = majorEnum.Name;
                    entity.DesignerID = this.Designer;
                    entity.Designer = this.DesignerName;
                    entity.CreateUserID = mistake.CreateUser;
                    entity.CreateUser = mistake.CreateUserName;
                    entity.CreateDate = mistake.CreateDate == null ? DateTime.Now : Convert.ToDateTime(mistake.CreateDate);
                    entity.MistakeYear = mistake.CreateDate.Value.Year;
                    entity.MistakeMonth = mistake.CreateDate.Value.Month;
                    entity.MistakeSeason = (mistake.CreateDate.Value.Month + 2) / 3;
                    entity.Measure = mistake.ResponseContent;
                    entity.DrawingNO = mistake.ProductCode;
                    entity.DeptID = projectInfo.ChargeDeptID;
                    entity.DeptName = projectInfo.ChargeDeptName;
                    //图号等信息
                    projectEntities.Set<S_AE_Mistake>().Add(entity);
                }
            }
        }


        public List<AuditUserInfo> GetAuditSignUser()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            IUserService service = FormulaHelper.GetService<IUserService>();
            var auditStateArray = Enum.GetNames(typeof(Project.Logic.AuditState));
            var activityList = entities.S_W_Activity.Where(d => d.BusniessID == this.ID).ToList();
            List<AuditUserInfo> list = new List<AuditUserInfo>();

            var dic = this.ToDic();
            foreach (var item in auditStateArray)
            {
                if (this.AuditSignSource == "0")//从表单上取
                {
                    var value = dic.GetValue(item);
                    if (!string.IsNullOrEmpty(value))
                    {
                        var users = value.Split(',');
                        foreach (var user in users)
                        {
                            var u = service.GetUserInfoByID(user);
                            if (u != null)
                            {
                                AuditUserInfo userInfo = new AuditUserInfo(item, user, u.UserName);
                                userInfo.SignDate = DateTime.Now;
                                list.Add(userInfo);
                            }
                        }
                    }
                }
                else//从流程中取
                {
                    //项目负责人需要按照策划的顺序进行签名
                    if (item == "ProjectManager" && !string.IsNullOrEmpty(this.ProjectManager))
                    {
                        var pms = this.ProjectManager.Split(',');
                        foreach (var pmID in pms)
                        {
                            var activity = activityList.FirstOrDefault(a => a.ActivityKey == item && a.OwnerUserID == pmID);
                            if (activity != null)
                            {
                                AuditUserInfo userInfo = new AuditUserInfo(item, activity.OwnerUserID, activity.OwnerUserName);
                                userInfo.SignDate = activityList.Where(d => d.ActivityKey == item).Max(d => d.CreateDate);
                                list.Add(userInfo);
                            }
                        }
                    }
                    else
                    {
                        foreach (var activity in activityList.Where(d => d.ActivityKey == item).ToList())
                        {
                            if (list.Any(a => a.ActivityKey == item && a.UserID == activity.OwnerUserID))
                                continue;
                            AuditUserInfo userInfo = new AuditUserInfo(item, activity.OwnerUserID, activity.OwnerUserName);
                            userInfo.SignDate = activityList.Where(d => d.ActivityKey == item).Max(d => d.CreateDate);
                            list.Add(userInfo);
                        }
                    }
                }
            }

            return list;
        }

    }

}
