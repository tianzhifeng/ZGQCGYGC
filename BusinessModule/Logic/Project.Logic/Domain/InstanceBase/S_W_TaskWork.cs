using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;

namespace Project.Logic.Domain
{
    public partial class S_W_TaskWork
    {
        /// <summary>
        /// 发布工作任务
        /// </summary>
        public void Publish()
        {
            if (this.State == TaskWorkState.Finish.ToString()) return;
            var entities = this.GetDbContext<ProjectEntities>();
            if (String.IsNullOrEmpty(this.DesignerUserID))
                throw new Formula.Exceptions.BusinessException("未指定设计人，无法发布");
            this.State = TaskWorkState.Execute.ToString();
            this.S_W_WBS.State = TaskWorkState.Execute.ToString();

            //20190412  去除生成Activety 信息，个人门户页面以 代办事项 替代 设计任务 窗口 
            //this.CreateActivity();
        }

        /// <summary>
        /// 撤销发布工作任务
        /// </summary>
        public void UnPublish()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (this.S_W_WBS.S_E_Product.Count > 0)
                throw new Formula.Exceptions.BusinessException("已经上传成功的工作任务，不能撤销发布");
            this.State = TaskWorkState.Plan.ToString();
            entities.S_W_Activity.Delete(d => d.BusniessID == this.ID);
        }

        /// <summary>
        /// 完成工作任务
        /// </summary>
        public void Finish()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            this.State = TaskWorkState.Finish.ToString();
            this.S_W_WBS.State = TaskWorkState.Finish.ToString();
            if (this.S_W_WBS.FactEndDate == null) this.S_W_WBS.FactEndDate = DateTime.Now;
            if (this.FactEndDate == null) this.FactEndDate = DateTime.Now;
            this.FactYear = this.FactEndDate.Value.Year;
            this.FactSeason = (this.FactEndDate.Value.Month + 2) / 3;
            this.FactMonth = this.FactEndDate.Value.Month;
        }

        /// <summary>
        /// 删除工作包
        /// </summary>
        public void Delete(bool validateMode = true)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (validateMode)
            {
                if (this.S_W_WBS.S_E_Product.Count > 0) throw new Formula.Exceptions.BusinessException("已经上传成果的工作包，不能删除");
                if (this.WorkloadFinish > 0) throw new Formula.Exceptions.BusinessException("已经结算过的工作包，不能删除");
            }
            this.S_W_WBS.Delete(validateMode);
            entities.S_W_TaskWork.Delete(d => d.ID == this.ID);
            var budget = entities.S_C_CBS_Budget.FirstOrDefault(a => a.ID == this.ID);
            if (budget != null)
                budget.Delete(!validateMode);

            //删除设计任务
            var state = TaskWorkState.Create.ToString();
            entities.S_W_Activity.Delete(c => c.BusniessID == this.ID && c.State == state);
            //删除标准工日
            entities.S_W_StandardWorkTime.Delete(c => c.TaskWorkID == this.ID);
        }

        /// <summary>
        /// 保存工作包
        /// </summary>
        /// <param name="withActivity">是否创建Activity活动</param>
        public void Save(bool withActivity = false)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry(this).State == System.Data.EntityState.Detached || entities.Entry(this).State == System.Data.EntityState.Added)
                throw new Formula.Exceptions.BusinessException("不能通过Save方法来直接新增工作任务对象，请通过WBS对象的AddTaskWork方法来新增任务对象");
            this.S_W_WBS.Name = this.Name;
            this.S_W_WBS.Code = this.Code;
            this.S_W_WBS.ChargeUserID = this.ChargeUserID;
            this.S_W_WBS.ChargeUserName = this.ChargeUserName;
            this.S_W_WBS.PlanStartDate = this.PlanStartDate;
            this.S_W_WBS.PlanEndDate = this.PlanEndDate;
            this.S_W_WBS.Save();
            if (this.PlanEndDate != null)
            {
                this.PlanYear = this.PlanEndDate.Value.Year;
                this.PlanSeason = (this.PlanEndDate.Value.Month + 2) / 3;
                this.PlanMonth = this.PlanEndDate.Value.Month;
            }

            var budget = entities.S_C_CBS_Budget.FirstOrDefault(a => a.ID == this.ID);
            if (budget != null)
            {
                budget.Name = this.Name;
                budget.Code = this.Code;
                budget.Quantity = this.Workload;
            }

            //为WBS节点设定设计人
            if (!String.IsNullOrEmpty(this.DesignerUserID))
                this.S_W_WBS.SetUsers(ProjectRole.Designer.ToString(), this.DesignerUserID.Split(','), false, true, true, true);
            //为WBS节点设定校核人
            if (!String.IsNullOrEmpty(this.CollactorUserID))
                this.S_W_WBS.SetUsers(ProjectRole.Collactor.ToString(), this.CollactorUserID.Split(','), false, true, true, true);
            //为WBS节点设定制图人
            if (!String.IsNullOrEmpty(this.MapperUserID))
                this.S_W_WBS.SetUsers(ProjectRole.Mapper.ToString(), this.MapperUserID.Split(','), false, true, true, true);
            //为WBS节点设定审定人
            if (!String.IsNullOrEmpty(this.ApproverUserID))
                this.S_W_WBS.SetUsers(ProjectRole.Approver.ToString(), this.ApproverUserID.Split(','), false, true, true, true);
            //为WBS节点设定审核人
            if (!String.IsNullOrEmpty(this.AuditorUserID))
                this.S_W_WBS.SetUsers(ProjectRole.Auditor.ToString(), this.AuditorUserID.Split(','), false, true, true, true);

            if (withActivity)
                this.CreateActivity();

            if (this.Workload.HasValue)
            {
                foreach (var role in this.S_W_TaskWork_RoleRate)
                {
                    //role.Member = item.GetValue(role.Role + "UserID");
                    //role.MemberName = item.GetValue(role.Role + "UserName");
                    role.Workload = Math.Round((decimal)(Convert.ToDecimal(role.Rate) * Convert.ToDecimal(this.Workload) / 100), 2);
                }
                this.RoleRate = JsonHelper.ToJson(this.S_W_TaskWork_RoleRate.OrderBy(a => a.SortIndex));
            }

            var user = FormulaHelper.GetUserInfo();
            this.ModifyDate = DateTime.Now;
            this.ModifyUser = user.UserName;
            this.ModifyUserID = user.UserID;
        }

        /// <summary>
        /// 将WBS节点上的设校审人员填充到工作包上（如有多人，默认获取第一个人）
        /// </summary>
        /// <param name="wbsNode">WBS节点</param>
        public void FillWBSUser(S_W_WBS wbsNode)
        {
            var designer = wbsNode.GetUser(ProjectRole.Designer.ToString());
            var collactor = wbsNode.GetUser(ProjectRole.Collactor.ToString());
            var auditor = wbsNode.GetUser(ProjectRole.Auditor.ToString());
            var approver = wbsNode.GetUser(ProjectRole.Approver.ToString());
            var mapper = wbsNode.GetUser(ProjectRole.Mapper.ToString());

            if (designer != null)
            {
                this.DesignerUserID = designer.UserID;
                this.DesignerUserName = designer.UserName;
                if (this.S_W_WBS != null)
                    this.S_W_WBS.SetUsers(ProjectRole.Designer.ToString(), this.DesignerUserID.Split(','), false, true, true, true);
            }
            if (collactor != null)
            {
                this.CollactorUserID = collactor.UserID;
                this.CollactorUserName = collactor.UserName;
                if (this.S_W_WBS != null)
                    this.S_W_WBS.SetUsers(ProjectRole.Collactor.ToString(), this.CollactorUserID.Split(','), false, true, true, true);
            }
            if (auditor != null)
            {
                this.AuditorUserID = auditor.UserID;
                this.AuditorUserName = auditor.UserName;
                if (this.S_W_WBS != null)
                    this.S_W_WBS.SetUsers(ProjectRole.Auditor.ToString(), this.AuditorUserID.Split(','), false, true, true, true);
            }
            if (approver != null)
            {
                this.ApproverUserID = approver.UserID;
                this.ApproverUserName = approver.UserName;
                if (this.S_W_WBS != null)
                    this.S_W_WBS.SetUsers(ProjectRole.Approver.ToString(), this.ApproverUserID.Split(','), false, true, true, true);
            }
            if (mapper != null)
            {
                this.MapperUserID = mapper.UserID;
                this.MapperUserName = mapper.UserName;
                if (this.S_W_WBS != null)
                    this.S_W_WBS.SetUsers(ProjectRole.Mapper.ToString(), this.MapperUserID.Split(','), false, true, true, true);
            }
        }

        /// <summary>
        /// 创建活动
        /// </summary>
        public void CreateActivity(ActivityType actType = ActivityType.DesignTask)
        {
            if (!String.IsNullOrEmpty(this.DesignerUserID))
            {
                //先删除已有活动
                var entities = this.GetDbContext<ProjectEntities>();
                entities.S_W_Activity.Delete(d => d.BusniessID == this.ID);
                var designerIds = this.DesignerUserID.Split(',');
                var designerNames = this.DesignerUserName.Split(',');
                //this.S_W_WBS.ClearActivity();
                for (int i = 0; i < designerIds.Length; i++)
                {
                    string designerID = designerIds[i];
                    string designerName = designerNames[i];
                    CreateAcitivityForSingleUser(designerID, designerName, actType);
                }
            }
        }

        /// <summary>
        /// 为单一用户创建活动
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">用户名称</param>
        /// <param name="actType">活动类别（默认是设计活动）</param>
        /// <returns>活动对象</returns>
        public S_W_Activity CreateAcitivityForSingleUser(string userID, string userName, ActivityType actType = ActivityType.DesignTask)
        {
            var activity = new S_W_Activity();
            var activityName = this.S_W_WBS.S_I_ProjectInfo.Name + "-";
            foreach (var item in this.S_W_WBS.Seniorities)
            {
                if (item.WBSType == WBSNodeType.SubProject.ToString())
                    activityName += item.Name + "-";
            }
            foreach (var item in this.S_W_WBS.Seniorities)
            {
                if (item.WBSType == WBSNodeType.Major.ToString())
                    activityName += item.Name + "-";
            }
            activity.DisplayName = activityName + "-" + this.Name;
            activity.ActvityName = EnumBaseHelper.GetEnumDescription(typeof(ActivityType), actType.ToString());
            activity.ActivityKey = actType.ToString();
            activity.BusniessID = this.ID;
            activity.OwnerUserID = userID;
            activity.OwnerUserName = userName;
            activity.State = ProjectCommoneState.Create.ToString();
            activity.LinkUrl = CommonConst.designSubmitActivityUrl;
            this.S_W_WBS.AddActivity(activity);
            return activity;
        }

        /// <summary>
        /// 同步标准工日信息
        /// </summary>
        public S_W_StandardWorkTime SynchStandardWorkTime(bool isValidateWorkDay = true)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            S_W_StandardWorkTime entity = entities.Set<S_W_StandardWorkTime>().SingleOrDefault(c => c.TaskWorkID == this.ID);
            if (entity == null)
            {
                entity = entities.Set<S_W_StandardWorkTime>().Create();
                entity.ID = FormulaHelper.CreateGuid();
                entities.Set<S_W_StandardWorkTime>().Add(entity);
                entity.CreateDate = DateTime.Now;
                if (user != null)
                {
                    entity.CreateUserID = user.UserID;
                    entity.CreateUser = user.UserName;
                }
            }
            else
            {
                entity.ModifyDate = DateTime.Now;
                if (user != null)
                {
                    entity.ModifyUserID = user.UserID;
                    entity.ModifyUser = user.UserName;
                }
            }
            entity.ProjectInfoID = this.ProjectInfoID;
            entity.ProjectInfoCode = this.S_W_WBS.S_I_ProjectInfo.Code;
            entity.ProjectInfoName = this.S_W_WBS.S_I_ProjectInfo.Name;
            entity.WBSID = this.WBSID;
            entity.TaskWorkID = this.ID;
            entity.WorkDay = this.StandartWorkDay;
            entity.MajorCode = this.S_W_WBS.Parent.WBSValue;
            entity.MajorName = this.S_W_WBS.Parent.Name;
            entity.DesignerUserID = this.DesignerUserID;
            entity.DesignerUserName = this.DesignerUserName;
            entity.CollactorUserID = this.CollactorUserID;
            entity.CollactorUserName = this.CollactorUserName;
            entity.AuditorUserID = this.AuditorUserID;
            entity.AuditorUserName = this.AuditorUserName;
            entity.ApproverUserID = this.ApproverUserID;
            entity.ApproverUserName = this.ApproverUserName;
            entity.MapperUserID = this.MapperUserID;
            entity.MapperUserName = this.MapperUserName;
            entity.TaskWorkCode = this.Code;

            //同步字表信息
            if (isValidateWorkDay == true)
                entity.ValidateStandardWorkDay();
            if (entity.S_W_StandardWorkTimeDetail.ToList().Count == 0)
                entity.CreateDetailInfo();
            return entity;
        }

        public void InitRoleRate()
        {
            if (this.S_W_WBS == null)
                throw new Formula.Exceptions.BusinessException("未找到卷册【" + this.Code + "】的WBS信息，无法初始化角色定额工时信息");
            var roleRateList = this.S_W_TaskWork_RoleRate.ToList();
            var roleDefine = this.S_W_WBS.StructNodeInfo.S_T_WBSStructRole.OrderBy(a => a.SortIndex).ToList();
            var baseConfigEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var package = baseConfigEntities.Set<S_D_PackageDic>().FirstOrDefault(a => a.Code == this.Code
                && a.MajorCode == this.MajorValue && this.S_W_WBS.S_I_ProjectInfo.PhaseValue.Contains(a.PhaseCode));
            var roleWorkloadDefines = baseConfigEntities.Set<S_C_RoleWorkloadDefine>().ToList();
            int sortIndex = 0;
            foreach (var role in roleDefine)
            {
                var roleRate = new S_W_TaskWork_RoleRate()
                {
                    ID = FormulaHelper.CreateGuid(),
                    S_W_TaskWorkID = this.ID,
                    SortIndex = sortIndex++,
                    Role = role.RoleCode,
                    //Member = item.GetValue(role.RoleCode + "UserID"),
                    //MemberName = item.GetValue(role.RoleCode + "UserName")
                };
                if (package != null && package.S_D_PackageDic_RoleRate.Count > 0)
                {
                    var roleD = package.S_D_PackageDic_RoleRate.FirstOrDefault(a => a.Role == role.RoleCode);
                    if (roleD != null)
                        roleRate.Rate = roleD.Rate;
                    else
                        roleRate.Rate = 0;
                }
                else
                {
                    var roleWorkloadDefine = roleWorkloadDefines.FirstOrDefault(a => a.RoleCode == role.RoleCode);
                    if (roleWorkloadDefine != null)
                        roleRate.Rate = roleWorkloadDefine.Rate;
                    else
                        roleRate.Rate = 0;
                }
                roleRate.Workload = Math.Round((Convert.ToDecimal(roleRate.Rate) * Convert.ToDecimal(this.Workload) / 100m), 2);
                this.S_W_TaskWork_RoleRate.Add(roleRate);
                roleRateList.Add(roleRate);
            }
            var sum = roleRateList.Sum(a => a.Rate).ToString().TrimEnd('0').TrimEnd('.') + "%";
            var d = roleRateList.FirstOrDefault(a => a.Role == "Designer");
            var c = roleRateList.FirstOrDefault(a => a.Role == "Collactor");
            var au = roleRateList.FirstOrDefault(a => a.Role == "Auditor");
            var ap = roleRateList.FirstOrDefault(a => a.Role == "Approver");
            var m = roleRateList.FirstOrDefault(a => a.Role == "Mapper");
            var designStr = d == null ? "" : "设(" + d.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var collactStr = c == null ? "" : "校(" + c.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var auditStr = au == null ? "" : "审(" + au.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var approveStr = ap == null ? "" : "定(" + ap.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var mapperStr = m == null ? "" : "制(" + m.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var sumString = sum + " :"
                + (designStr == "设(%);" ? "" : designStr)
                + (collactStr == "校(%);" ? "" : collactStr)
                + (auditStr == "审(%);" ? "" : auditStr)
                + (approveStr == "定(%);" ? "" : approveStr)
                + (mapperStr == "制(%);" ? "" : mapperStr);
            this.WorkloadDistribute = sumString;
            this.RoleRate = JsonHelper.ToJson(roleRateList.OrderBy(a => a.SortIndex));
        }
    }
}
