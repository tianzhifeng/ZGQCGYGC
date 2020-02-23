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
using System.Data;
using System.Text.RegularExpressions;
using Config.Logic;

namespace Project.Logic.Domain
{
    public partial class S_W_WBS
    {
        #region 公共属性

        /// <summary>
        /// 管理模式对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_ProjectMode ProjectMode
        {
            get
            {
                if (this.S_I_ProjectInfo == null)
                    throw new Formula.Exceptions.BusinessException("WBS必须指定所属的项目对象，否则无法获取管理模式对象");
                return this.S_I_ProjectInfo.ProjectMode;
            }
        }

        /// <summary>
        /// 是否为枚举型节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsEnumNode
        {
            get
            {
                if (this.StructNodeInfo == null) return false;
                return this.StructNodeInfo.IsDefineNode;
            }
        }

        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_WBSStructInfo StructNodeInfo
        {
            get
            {
                return this.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == this.WBSStructCode);
            }
        }

        S_W_WBS _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_W_WBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_ProjectInfo.S_W_WBS.FirstOrDefault(d => d.ID == this.ParentID && d.ProjectInfoID == this.ProjectInfoID);
                }
                return _parent;
            }
        }

        List<S_W_WBS> _seniorities;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_W_WBS> Seniorities
        {
            get
            {
                if (_seniorities == null)
                {
                    _seniorities = this.S_I_ProjectInfo.S_W_WBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                }
                return _seniorities;
            }
        }

        /// <summary>
        /// 根节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_W_WBS RootNode
        {
            get
            {
                return this.S_I_ProjectInfo.WBSRoot;
            }
        }

        List<S_W_WBS> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_W_WBS> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_I_ProjectInfo.S_W_WBS.Where(d => d.ParentID == this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _children;
            }
        }

        List<S_W_WBS> _allchildren;
        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_W_WBS> AllChildren
        {
            get
            {
                if (this._allchildren == null || this._allchildren.Count == 0)
                {
                    this._allchildren = this.S_I_ProjectInfo.S_W_WBS.Where(w => w.FullID.StartsWith(this.FullID) && w.ID != this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _allchildren;
            }
        }

        List<S_P_MileStone> _mileStones;
        /// <summary>
        /// 里程碑清单
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_P_MileStone> MileStones
        {
            get
            {
                if (_mileStones == null)
                {
                    _mileStones = this.GetDbContext<ProjectEntities>().S_P_MileStone.Where(d => d.WBSID == this.ID).ToList();
                }
                return _mileStones;
            }
        }

        List<S_W_Activity> _activityList;
        /// <summary>
        /// 活动列表
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_W_Activity> ActivityList
        {
            get
            {
                if (_activityList == null)
                {
                    var entities = FormulaHelper.GetEntities<ProjectEntities>();
                    _activityList = entities.S_W_Activity.Where(d => d.WBSID == this.ID).ToList();
                }
                return _activityList;
            }
        }

        #endregion

        #region 公共实例方法


        /// <summary>
        /// 保存工作包信息
        /// </summary>
        /// <param name="mileStone">里程碑对象</param>
        public void SaveProduct(S_E_Product product)
        {
            product.WBSID = this.ID;
            product.S_W_WBS = this;
            product.Save();
        }

        /// <summary>
        /// 根据里程碑定义新增里程碑信息
        /// </summary>
        /// <param name="mileStoneDefine">里程碑定义</param>
        public void ImportMileStoneDefine(S_T_MileStone mileStoneDefine, string majorValue = "")
        {
            if (this.MileStones.Exists(d => d.Code == mileStoneDefine.MileStoneCode && d.MajorValue == majorValue))
                return;
            var mileStone = new S_P_MileStone();
            mileStone.ID = FormulaHelper.CreateGuid();
            mileStone.MajorValue = majorValue;
            if (this.WBSType == WBSNodeType.Major.ToString() && String.IsNullOrEmpty(mileStone.MajorValue))
                mileStone.MajorValue = this.WBSValue;
            mileStone.Name = mileStoneDefine.MileStoneName;
            mileStone.Code = mileStoneDefine.MileStoneCode;
            mileStone.MileStoneType = mileStoneDefine.MileStoneType;
            mileStone.Weight = mileStoneDefine.Weight;
            mileStone.Necessity = mileStoneDefine.Necessity;
            mileStone.SortIndex = mileStoneDefine.SortIndex;
            if (this.PlanStartDate != null && mileStoneDefine.DefaultTimeSpan != null)
            {
                mileStone.PlanFinishDate = Convert.ToDateTime(this.PlanStartDate).AddDays(Convert.ToDouble(mileStoneDefine.DefaultTimeSpan));
                mileStone.OrlPlanFinishDate = mileStone.PlanFinishDate;
            }
            mileStone.WBSID = this.ID;
            mileStone.ProjectInfoID = this.ProjectInfoID;
            mileStone.State = ProjectCommoneState.Plan.ToString();
            this.S_I_ProjectInfo.AddMileStone(mileStone);
            mileStone.Save();
        }

        /// <summary>
        /// 保存里程碑信息
        /// </summary>
        /// <param name="mileStone">里程碑对象</param>
        public void SaveMileStone(S_P_MileStone mileStone)
        {
            var entites = this.GetDbContext<ProjectEntities>();
            if (String.IsNullOrEmpty(mileStone.ID)) mileStone.ID = FormulaHelper.CreateGuid();
            mileStone.WBSID = this.ID;
            mileStone.ProjectInfoID = this.ProjectInfoID;
            mileStone.S_I_ProjectInfo = this.S_I_ProjectInfo;
            if (String.IsNullOrEmpty(mileStone.State)) mileStone.State = ProjectCommoneState.Plan.ToString();
            mileStone.Save();
        }

        /// <summary>
        /// 保存互提计划
        /// </summary>
        /// <param name="cooperationPlan">互提计划对象</param>
        public void SaveCooperationPlan(S_P_CooperationPlan cooperationPlan, bool createMileStone = false, bool linkRoot = false)
        {
            var entites = this.GetDbContext<ProjectEntities>();
            var majorNode = this.AllChildren.Where(c => c.WBSType == WBSNodeType.Major.ToString() && c.WBSValue == cooperationPlan.OutMajorValue).FirstOrDefault();
            var cooperationNode = entites.Set<S_W_WBS>().FirstOrDefault(c => c.ID == cooperationPlan.WBSID);
            if (cooperationNode != null)
            {
                if (!linkRoot)
                {
                    cooperationNode.Name = cooperationPlan.CooperationContent;
                    cooperationNode.PlanEndDate = cooperationPlan.PlanFinishDate;
                    //先放接受专业的编码
                    cooperationNode.WBSValue = cooperationPlan.InMajorValue;
                }
            }
            else
            {
                cooperationNode = new S_W_WBS();
                cooperationNode.ID = FormulaHelper.CreateGuid();
                cooperationNode.Name = cooperationPlan.CooperationContent;
                cooperationNode.PlanEndDate = cooperationPlan.PlanFinishDate;
                cooperationNode.WBSValue = cooperationPlan.InMajorValue;
                cooperationNode.WBSType = WBSNodeType.CooperationPackage.ToString();

                majorNode.AddChild(cooperationNode, false);

            }

            if (entites.Entry<S_P_CooperationPlan>(cooperationPlan).State == EntityState.Detached
                || entites.Entry<S_P_CooperationPlan>(cooperationPlan).State == EntityState.Added)
            {
                cooperationPlan.ID = FormulaHelper.CreateGuid();
                cooperationPlan.WBSID = cooperationNode.ID;
                cooperationPlan.WBSFullID = cooperationNode.FullID;
                cooperationPlan.ProjectInfoID = this.ProjectInfoID;
                cooperationPlan.SchemeWBSID = this.ID;
                cooperationPlan.SchemeWBSName = this.Name;
                cooperationPlan.CreateUser = FormulaHelper.GetUserInfo().UserName;
                cooperationPlan.CreateUserID = FormulaHelper.GetUserInfo().UserID;
                cooperationPlan.CreateDate = DateTime.Now;
                cooperationPlan.ModifyUser = cooperationPlan.CreateUser;
                cooperationPlan.ModifyUserID = cooperationPlan.CreateUserID;
                cooperationPlan.ModifyDate = DateTime.Now;
                entites.S_P_CooperationPlan.Add(cooperationPlan);
            }
            else
            {
                cooperationPlan.ModifyUser = FormulaHelper.GetUserInfo().UserName;
                cooperationPlan.ModifyUserID = FormulaHelper.GetUserInfo().UserID;
                cooperationPlan.ModifyDate = DateTime.Now;
            }
            cooperationPlan.Save(createMileStone);
        }

        /// <summary>
        /// 在WBS下增加工作任务对象（同步生成WBS工作包节点）
        /// </summary>
        /// <param name="Task">工作任务对象</param>
        /// <param name="validateMode">是否进行WBS结构校验</param>
        public S_W_WBS AddTaskWork(S_W_TaskWork Task, bool validateMode = true)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_W_TaskWork>(Task).State != System.Data.EntityState.Detached && entities.Entry<S_W_TaskWork>(Task).State != EntityState.Added)
                throw new Formula.Exceptions.BusinessException("非新增状态的Task对象，无法调用AddTaskWork方法");

            var wbs = new S_W_WBS();
            wbs.ID = FormulaHelper.CreateGuid();
            wbs.Name = Task.Name;
            wbs.Code = Task.Code;
            wbs.ChargeUserID = Task.ChargeUserID;
            wbs.ChargeUserName = Task.ChargeUserName;
            wbs.WBSType = WBSNodeType.Work.ToString();
            wbs.PlanStartDate = Task.PlanStartDate;
            wbs.PlanEndDate = Task.PlanEndDate;
            this.AddChildWBS(wbs);
            if (String.IsNullOrEmpty(Task.ID))
                Task.ID = FormulaHelper.CreateGuid();
            Task.ProjectInfoID = this.ProjectInfoID;
            Task.WBSID = wbs.ID;
            Task.S_W_WBS = wbs;
            Task.WBSFullID = wbs.FullID;
            Task.ChargeDeptID = wbs.WBSDeptID;
            Task.ChargeDeptName = wbs.WBSDeptName;
            Task.State = ProjectCommoneState.Plan.ToString();
            if (Task.PlanEndDate != null)
            {
                Task.PlanYear = Task.PlanEndDate.Value.Year;
                Task.PlanSeason = (Task.PlanEndDate.Value.Month + 2) / 3;
                Task.PlanMonth = Task.PlanEndDate.Value.Month;
            }

            Task.CreateDate = DateTime.Now;
            Task.CreateUser = user.UserName;
            Task.CreateUserID = user.UserID;
            Task.ModifyDate = DateTime.Now;
            Task.ModifyUser = user.UserName;
            Task.ModifyUserID = user.UserID;

            wbs.S_W_TaskWork.Add(Task);

            //设置工作包的默认专业，阶段，或子项属性
            if (String.IsNullOrEmpty(Task.MajorValue))
            {
                var majorNode = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
                if (majorNode != null) Task.MajorValue = majorNode.WBSValue;
            }

            if (String.IsNullOrEmpty(Task.PhaseValue))
            {
                var phaseNode = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Phase.ToString());
                if (phaseNode != null) Task.PhaseValue = phaseNode.WBSValue;
            }

            if (String.IsNullOrEmpty(Task.SubProjectCode))
            {
                var subProjectNode = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.SubProject.ToString());
                if (subProjectNode != null) Task.SubProjectCode = subProjectNode.WBSValue;
            }

            //为WBS节点设定设计人
            if (!String.IsNullOrEmpty(Task.DesignerUserID))
                wbs.SetUsers(ProjectRole.Designer.ToString(), Task.DesignerUserID.Split(','), false, true, true, true);

            //为WBS节点设定校核人
            if (!String.IsNullOrEmpty(Task.CollactorUserID))
                wbs.SetUsers(ProjectRole.Collactor.ToString(), Task.CollactorUserID.Split(','), false, true, true, true);

            //为WBS节点设定制图人
            if (!String.IsNullOrEmpty(Task.MapperUserID))
                wbs.SetUsers(ProjectRole.Mapper.ToString(), Task.MapperUserID.Split(','), false, true, true, true);

            //为WBS节点设定审定人
            if (!String.IsNullOrEmpty(Task.ApproverUserID))
                wbs.SetUsers(ProjectRole.Approver.ToString(), Task.ApproverUserID.Split(','), false, true, true, true);

            //为WBS节点设定审核人
            if (!String.IsNullOrEmpty(Task.AuditorUserID))
                wbs.SetUsers(ProjectRole.Auditor.ToString(), Task.AuditorUserID.Split(','), false, true, true, true);

            if (Task.PlanYear == null && Task.PlanEndDate != null)
                Task.PlanYear = Task.PlanEndDate.Value.Year;
            if (Task.PlanMonth == null && Task.PlanEndDate != null)
                Task.PlanMonth = Task.PlanEndDate.Value.Month;

            onAddTaskWork();
            return wbs;
        }

        /// <summary>
        /// 在WBS下增加工作任务对象（同步生成WBS工作包节点）
        /// </summary>
        /// <param name="taskDic">工作包词典定义对象</param>
        /// <param name="ignroRepeat">是否忽略重复</param>
        public S_W_WBS AddTaskWork(S_D_PackageDic taskDic, bool ignroRepeat = true)
        {
            var task = new S_W_TaskWork();
            task.Name = taskDic.Name;
            task.Code = taskDic.Code;
            task.PhaseValue = taskDic.PhaseCode;
            task.MajorValue = taskDic.MajorCode;
            task.Workload = taskDic.WorkLoad;
            if (!ignroRepeat)
            {
                if (this.Children.Exists(d => d.WBSValue == taskDic.Code))
                    throw new Formula.Exceptions.BusinessException("已经存在编号为【" + taskDic.Code + "】的工作包，无法进行重复添加");
            }
            return this.AddTaskWork(task);
        }

        /// <summary>
        /// 增加WBS子节点
        /// </summary>
        /// <param name="child">子节点对象</param>
        /// <param name="validateMode">是否根据配置定义结构进行校验</param>
        public S_W_WBS AddChild(S_W_WBS child, bool validateMode = true, bool sychToTaskWork = false)
        {
            if (!sychToTaskWork && child.WBSType == WBSNodeType.Work.ToString())
                throw new Formula.Exceptions.BusinessException("请通过AddTask方法增加工作包，或将API中的sychToTaskWork参数设置为True");
            return this.AddChildWBS(child, validateMode, sychToTaskWork);
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="activity">活动对象</param>
        public void AddActivity(S_W_Activity activity)
        {
            if (String.IsNullOrEmpty(activity.ActivityKey) || String.IsNullOrEmpty(activity.ActvityName))
                throw new Formula.Exceptions.BusinessException("必须指定活动KEY和活动名称");
            activity.ProjectInfoID = this.ProjectInfoID;
            activity.WBSID = this.ID;
            activity.CreateDate = DateTime.Now;
            var userInfo = FormulaHelper.GetUserInfo();
            activity.CreateUser = userInfo.UserName;
            activity.CreateUserID = userInfo.UserID;
            if (String.IsNullOrEmpty(activity.ID))
                activity.ID = FormulaHelper.CreateGuid();
            if (String.IsNullOrEmpty(activity.DisplayName))
                activity.DisplayName = activity.ActvityName;
            activity.SetParam("ProjectInfoID", this.ProjectInfoID);
            activity.SetParam("WBSID", this.ID);
            if (this.S_W_TaskWork.Count > 0)
                activity.SetParam("TaskWorkID", this.S_W_TaskWork.FirstOrDefault().ID);
            if (String.IsNullOrEmpty(activity.State))
                activity.State = AuditState.Create.ToString();
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            entities.S_W_Activity.Add(activity);
            this.ActivityList.Add(activity);
        }

        /// <summary>
        /// 清除所有活动
        /// </summary>
        /// <param name="igonFinish">是否删除已经完成的活动（默认false）</param>
        public void ClearActivity(bool igonFinish = false)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (igonFinish)
            {
                entities.S_W_Activity.Delete(d => d.WBSID == this.ID);
                this.ActivityList.Clear();
            }
            else
            {
                entities.S_W_Activity.Delete(d => d.WBSID == this.ID && d.State == ProjectCommoneState.Create.ToString());
                this.ActivityList.RemoveWhere(d => d.State == ProjectCommoneState.Create.ToString());
            }
        }

        /// <summary>
        /// 根据角色编码追加人员
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="userIds">人员ID字符串（多人时，以逗号分隔）</param>
        public void AppendUsers(string roleCode, string userIds)
        {
            foreach (var userID in userIds.Split(','))
            {
                var userInfo = FormulaHelper.GetUserInfoByID(userID);
                this.AppendUsers(roleCode, userInfo);
            }
        }

        /// <summary>
        /// 根据角色编码追加人员
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="userInfo">人员信息对象</param>
        public void AppendUsers(string roleCode, UserInfo userInfo)
        {
            if (this.S_W_RBS.Count(d => d.UserID == userInfo.UserID && d.RoleCode == roleCode) > 0)
                return;
            this.SetUser(roleCode, userInfo, false, false);
        }

        /// <summary>
        /// 为WBS设置人员
        /// </summary>
        /// <param name="roleCode">岗位角色编码</param>
        /// <param name="userID">用户ID</param>
        /// <param name="obsID">"OBS岗位ID"</param>
        /// <param name="SychChargeUser">是否同步至WBS负责人</param>
        /// <param name="isReset">是否重置人员后再进行人员设置</param>
        /// <param name="validateMode">是否根据模板验证</param>
        public void SetUser(string roleCode, string userID, bool isResetOBS = false, bool isReset = true, bool validateMode = true, bool ignoreValidateError = false)
        {
            if (String.IsNullOrEmpty(userID)) return;
            var userInfo = FormulaHelper.GetUserInfoByID(userID);
            if (userInfo == null) throw new Formula.Exceptions.BusinessException("当前指定的用户ID【" + userID + "】未能找到用户信息，无法设置人员");
            this.SetUser(roleCode, userInfo, isResetOBS, isReset, validateMode, ignoreValidateError);
        }

        /// <summary>
        /// 为WBS设置人员
        /// </summary>
        /// <param name="roleCode">岗位角色编码</param>
        /// <param name="user">用户对象</param>
        /// <param name="obsID">"OBS岗位ID"</param>
        /// <param name="isReset">是否重置人员后再进行人员设置</param>
        /// <param name="validateMode">是否根据模板验证</param>
        /// <param name="ignoreValidateError">是否忽略验证错误(如果忽略则直接返回，不抛出异常信息)</param>
        public void SetUser(string roleCode, UserInfo user, bool isResetOBS = false, bool isReset = true, bool validateMode = true,
            bool ignoreValidateError = false)
        {
            if (isReset)
                this.RemoveUser(roleCode);
            var configFO = FormulaHelper.CreateFO<BaseConfigFO>();
            var roleDefine = configFO.GetRoleDefine(roleCode);
            if (roleDefine == null) throw new Formula.Exceptions.BusinessException("未能找到定义为【" + roleCode + "】的岗位角色定义信息，无法设置WBS角色");
            var rbsUser = this.S_W_RBS.FirstOrDefault(d => d.RoleCode == roleCode && d.UserID == user.UserID);
            if (rbsUser != null)
                return;
            rbsUser = new S_W_RBS();
            rbsUser.UserID = user.UserID;
            rbsUser.UserName = user.UserName;
            rbsUser.RoleCode = roleCode;
            rbsUser.WBSType = this.WBSType;
            rbsUser.RoleName = roleDefine.RoleName;
            rbsUser.ProjectInfoID = this.ProjectInfoID;
            rbsUser.OBSID = "";
            rbsUser.WBSCode = this.WBSValue;
            rbsUser.UserDeptID = user.UserOrgID;
            rbsUser.UserDeptName = user.UserOrgName;
            this.S_W_RBS.Add(rbsUser);
            
            rbsUser.MajorValue = this.MajorCode;
            //if (this.WBSType == WBSNodeType.Major.ToString())
            //    rbsUser.MajorValue = this.WBSValue;
            //else if (this.WBSType == WBSNodeType.Work.ToString())
            //{
            //    if (this.S_W_TaskWork.Count == 0) throw new Formula.Exceptions.BusinessException("工作包节点必须拥有工作包TaskWork对象才能设置人员角色");
            //    rbsUser.MajorValue = this.S_W_TaskWork.FirstOrDefault().MajorValue;
            //}

            if (validateMode)
            {
                if (!this.StructNodeInfo.ValidateRoleDefine(roleCode))
                {
                    if (ignoreValidateError)
                        return;
                    else
                        throw new Formula.Exceptions.BusinessException("管理模式定义中，【" + this.WBSStructCode + "】节点不存在【" + roleCode + "】的角色定义，无法设置人员");
                }
                var structRodeDefine = this.StructNodeInfo.S_T_WBSStructRole.FirstOrDefault(d => d.RoleCode == roleCode);

                //如果定义中配置了同步WBS字段，则同步人员信息至WBS的字段上
                if (structRodeDefine.SychWBS == true.ToString())
                {
                    string wbsSycField = String.IsNullOrEmpty(structRodeDefine.SychWBSField) ? "ChargeUserID" : structRodeDefine.SychWBSField;
                    string wbsSycNameFiled = wbsSycField.Replace("ID", "Name");
                    this.SetProperty(wbsSycField, user.UserID);
                    this.SetProperty(wbsSycNameFiled, user.UserName);
                }
                //如果定义中配置了同步ProjectInfo字段，则同步人员信息至ProjectInfo的字段上
                if (structRodeDefine.SychProject == true.ToString())
                {
                    string projectSycField = String.IsNullOrEmpty(structRodeDefine.SychProjectField) ? "ChargeUserID" : structRodeDefine.SychProjectField;
                    string projectSycNameFiled = projectSycField.Replace("ID", "Name");
                    this.S_I_ProjectInfo.SetProperty(projectSycField, user.UserID);
                    this.S_I_ProjectInfo.SetProperty(projectSycNameFiled, user.UserName);
                }
            }
            this.FiilToOBS(user, roleCode, isResetOBS);
            if (this.WBSType == WBSNodeType.Work.ToString())
            {
                var userRbsList = this.S_W_RBS.Where(d => d.RoleCode == roleCode);
                var userIDs = string.Empty; var userNames = string.Empty;
                foreach (var userRbs in userRbsList)
                {
                    userIDs += userRbs.UserID + ",";
                    userNames += userRbs.UserName + ",";
                }
                FillToTask(userIDs.TrimEnd(','), userNames.TrimEnd(','), roleCode);
            }
            this.onSetUser();
        }

        /// <summary>
        /// 为WBS设置人员
        /// </summary>
        /// <param name="roleCode">岗位角色编码</param>
        /// <param name="userIDs">用户ID数组</param>
        /// <param name="obsID">"OBS岗位ID"</param>
        /// <param name="isReset">是否重置人员后再进行人员设置</param>
        /// <param name="validateMode">是否根据模板验证</param>
        public void SetUsers(string roleCode, string[] userIDs, bool isResetOBS = false, bool isReset = true, bool validateMode = true, bool ignoreValidateError = false)
        {
            if (isReset)
                this.RemoveUser(roleCode);
            List<UserInfo> users = new List<UserInfo>();
            foreach (var userID in userIDs)
            {
                if (String.IsNullOrEmpty(userID)) continue;
                var userInfo = FormulaHelper.GetUserInfoByID(userID);
                if (userInfo == null) continue;
                users.Add(userInfo);
            }
            SetUsers(roleCode, users, isResetOBS, false, validateMode, ignoreValidateError);
        }

        /// <summary>
        /// 为WBS设置人员
        /// </summary>
        /// <param name="roleCode">岗位角色编码</param>
        /// <param name="users">用户对象集合</param>
        /// <param name="obsID">"OBS岗位ID"</param>
        /// <param name="SychChargeUser">是否同步至WBS负责人</param>
        /// <param name="isReset">是否重置人员后再进行人员设置</param>
        /// <param name="validateMode">是否根据模板验证</param>
        public void SetUsers(string roleCode, List<UserInfo> users, bool isResetOBS = true, bool isReset = true, bool validateMode = true, bool ignoreValidateError = false)
        {
            var currentUser = FormulaHelper.GetUserInfo();
            if (isReset)
                this.RemoveUser(roleCode);
            string userIDs = "";
            string userNames = "";
            var configFO = FormulaHelper.CreateFO<BaseConfigFO>();
            var roleDefine = configFO.GetRoleDefine(roleCode);
            foreach (var user in users)
            {
                userIDs += user.UserID + ",";
                userNames += user.UserName + ",";
                this.SetUser(roleCode, user, isResetOBS, isReset, validateMode, ignoreValidateError);
            }

            if (validateMode)
            {
                if (!this.StructNodeInfo.ValidateRoleDefine(roleCode))
                {
                    if (ignoreValidateError)
                        return;
                    else
                        throw new Formula.Exceptions.BusinessException("管理模式定义中，【" + this.WBSStructCode + "】节点不存在【" + roleCode + "】的角色定义，无法设置人员");
                }
                var structRodeDefine = this.StructNodeInfo.S_T_WBSStructRole.FirstOrDefault(d => d.RoleCode == roleCode);

                //如果定义中配置了同步WBS字段，则同步人员信息至WBS的字段上
                if (structRodeDefine.SychWBS == true.ToString())
                {
                    string wbsSycField = String.IsNullOrEmpty(structRodeDefine.SychWBSField) ? "ChargeUserID" : structRodeDefine.SychWBSField;
                    string wbsSycNameFiled = wbsSycField.Replace("ID", "Name");
                    this.SetProperty(wbsSycField, userIDs.TrimEnd(','));
                    this.SetProperty(wbsSycNameFiled, userNames.TrimEnd(','));
                }
                //如果定义中配置了同步ProjectInfo字段，则同步人员信息至ProjectInfo的字段上
                if (structRodeDefine.SychProject == true.ToString())
                {
                    string projectSycField = String.IsNullOrEmpty(structRodeDefine.SychProjectField) ? "ChargeUserID" : structRodeDefine.SychProjectField;
                    string projectSycNameFiled = projectSycField.Replace("ID", "Name");
                    this.S_I_ProjectInfo.SetProperty(projectSycField, userIDs.TrimEnd(','));
                    this.S_I_ProjectInfo.SetProperty(projectSycNameFiled, userNames.TrimEnd(','));
                }
            }
            this.ModifyDate = DateTime.Now;
            this.ModifyUser = currentUser.UserName;
            this.ModifyUserID = currentUser.UserID;
        }

        /// <summary>
        /// 根据人员ID，获取该人员在WBS节点上的所有角色
        /// </summary>
        /// <param name="userID">人员ID</param>
        /// <returns>角色集合</returns>
        public List<S_W_RBS> GetUserRole(string userID)
        {
            return this.S_W_RBS.Where(d => d.UserID == userID).ToList();
        }

        /// <summary>
        /// 根据岗位角色编码获得RBS对象
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>RBS人员岗位信息</returns>
        public S_W_RBS GetUser(string roleCode)
        {
            return this.S_W_RBS.FirstOrDefault(d => d.RoleCode == roleCode);
        }

        /// <summary>
        /// 判定用户是否在WBS节点上担当任意角色
        /// </summary>
        /// <param name="userID">人员ID</param>
        /// <returns>是否在WBS中担当角色</returns>
        public bool IsInWBS(string userID)
        {
            return this.S_W_RBS.Count(d => d.UserID == userID) > 0;
        }

        /// <summary>
        /// 移除指定角色的指定用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="userID">用户ID</param>
        public void RemoveUser(string roleCode, string userID)
        {
            this.GetDbContext<ProjectEntities>().S_W_RBS.Delete(d => d.WBSID == this.ID && d.RoleCode == roleCode
                && d.UserID == userID);
        }

        /// <summary>
        /// 移除指定角色的所有用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        public void RemoveUser(string roleCode)
        {
            this.GetDbContext<ProjectEntities>().S_W_RBS.Delete(d => d.WBSID == this.ID && d.RoleCode == roleCode);
        }

        /// <summary>
        /// 移除指定用户的所有角色
        /// </summary>
        /// <param name="userID">用户ID</param>
        public void RemoveUserByUserID(string userID)
        {
            this.GetDbContext<ProjectEntities>().S_W_RBS.Delete(d => d.WBSID == this.ID && d.UserID == userID);
        }

        /// <summary>
        /// 清除WBS节点上的所有人员角色
        /// </summary>
        public void ClearRBS()
        {
            var entites = this.GetDbContext<ProjectEntities>();
            foreach (var item in this.S_W_RBS.ToList())
                entites.S_W_RBS.Delete(d => d.ID == item.ID);
        }

        /// <summary>
        /// 删除WBS节点
        /// </summary>
        public void Delete(bool validateMode = true)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry(this).State == EntityState.Deleted) return;
            foreach (var item in this.AllChildren)
                item.DeleteSelf(validateMode);
            this.DeleteSelf(validateMode);
        }

        /// <summary>
        /// 保存WBS信息
        /// </summary>
        public void Save(bool sychUser = true)
        {
            if (sychUser)
            {
                if (!String.IsNullOrEmpty(this.ChargeUserID))
                {
                    //如果定义中存在需要同步ChargeUserID的配置定义，则将ChargeUserID 填写到RBS表中
                    var structRoleDefine = this.StructNodeInfo.S_T_WBSStructRole.FirstOrDefault(d => d.SychWBS == "True");
                    if (structRoleDefine != null)
                        this.SetUser(structRoleDefine.RoleCode, this.ChargeUserID);
                }
            }

            //如果当前节点没有节点定义暨非枚举型节点，则将名称作为WBSValue
            if (!this.IsEnumNode && String.IsNullOrEmpty(this.WBSValue))
                this.WBSValue = this.Name;

            //将WBSValue 更新到下层子节点的类型编码字段上
            string filedName = this.WBSType + "Code";
            foreach (var item in this.AllChildren)
            {
                var value = this.WBSValue.Trim();
                if (!String.IsNullOrEmpty(value))
                    item.SetProperty(filedName, value);
            }
            if (!String.IsNullOrEmpty(this.WBSValue.Trim()))
                this.SetProperty(filedName, this.WBSValue.Trim());

            if (this.S_W_TaskWork.Count > 0)
            {
                this.S_W_TaskWork.Update(d => d.PlanStartDate = this.PlanStartDate);
                this.S_W_TaskWork.Update(d => d.PlanEndDate = this.PlanEndDate);
                this.S_W_TaskWork.Update(d => d.Name = this.Name);
                if (!String.IsNullOrEmpty(this.Code))
                {
                    this.S_W_TaskWork.Update(d => d.Code = this.Code);
                }
            }
            if (!String.IsNullOrEmpty(this.PhaseCode))
            {
                foreach (var item in this.AllChildren)
                {
                    item.PhaseCode = this.PhaseCode;
                }
            }
            var user = FormulaHelper.GetUserInfo();
            this.ModifyDate = DateTime.Now;
            this.ModifyUser = user.UserName;
            this.ModifyUserID = user.UserID;
        }

        /// <summary>
        /// 复制WBS节点至指定的WBS
        /// </summary>
        /// <param name="targetNode">需要复制到的目标节点对象</param>
        public void CopyTo(S_W_WBS targetNode, bool includeRbs = true, bool includeChildren = true)
        {
            if (this.WBSType == WBSNodeType.Work.ToString())
            {
                var task = this.S_W_TaskWork.FirstOrDefault();
                if (task == null) return;
                var newTask = task.Clone<S_W_TaskWork>();
                newTask.PlanEndDate = null;
                newTask.PlanMonth = null;
                newTask.PlanSeason = null;
                newTask.PlanYear = null;
                newTask.State = ProjectCommoneState.Plan.ToString();
                targetNode.AddTaskWork(newTask);
                foreach (var child in this.Children)
                    child.CopyTo(newTask.S_W_WBS, includeRbs, includeChildren);
            }
            else
            {
                var wbs = this.Clone();
                wbs.State = ProjectCommoneState.Plan.ToString();
                var node = targetNode.AddChild(wbs, true);
                if (includeRbs)
                {
                    foreach (var rbs in this.S_W_RBS.ToList())
                    {
                        wbs.SetUser(rbs.RoleCode, rbs.UserID);
                    }
                }
                if (includeChildren)
                {
                    foreach (var child in this.Children)
                        child.CopyTo(node, includeRbs, includeChildren);
                }
            }
        }

        /// <summary>
        /// 根据WBS标准模板导入节点
        /// </summary>
        /// <param name="templateNode">模板节点对象</param>
        /// <param name="includeChildren">是否包括模板的子节点</param>
        public void ImportTemplateNode(S_D_WBSTemplateNode templateNode, bool includeChildren = true)
        {
            if (templateNode.WBSType == WBSNodeType.Work.ToString())
            {
                var newTask = new S_W_TaskWork();
                newTask.Name = templateNode.Name;
                newTask.Code = templateNode.Code;
                newTask.ChargeDeptID = templateNode.WBSDeptID;
                newTask.ChargeDeptName = templateNode.WBSDeptName;
                newTask.Workload = templateNode.WorkLoad;
                newTask.PlanEndDate = null;
                newTask.PlanMonth = null;
                newTask.PlanSeason = null;
                newTask.PlanYear = null;
                newTask.State = ProjectCommoneState.Plan.ToString();
                var wbs = this.AddTaskWork(newTask);
                foreach (var child in templateNode.Children)
                {
                    wbs.ImportTemplateNode(child, includeChildren);
                }
            }
            else
            {
                var wbs = new S_W_WBS();
                wbs.WBSValue = templateNode.WBSValue;
                wbs.Code = templateNode.Code;
                wbs.Name = templateNode.Name;
                wbs.WBSType = templateNode.WBSType;
                wbs.WBSDeptID = templateNode.WBSDeptID;
                wbs.WBSDeptName = templateNode.WBSDeptName;
                wbs.WBSStructCode = templateNode.WBSType;
                wbs.WorkLoad = templateNode.WorkLoad;
                this.AddChildWBS(wbs);
                if (includeChildren)
                {
                    foreach (var child in templateNode.Children)
                        wbs.ImportTemplateNode(child, includeChildren);
                }
            }

        }


        /// <summary>
        /// 克隆一个WBS节点
        /// </summary>
        /// <returns>返回克隆后的WBS对象（ID，ParentID,FullID,ProjectInfoID 为空）</returns>
        public S_W_WBS Clone()
        {
            var result = this.Clone<S_W_WBS>();
            result.ProjectInfoID = "";
            result.ParentID = "";
            result.FullID = "";
            if (!this.IsEnumNode)
                result.SortIndex = 0;
            return result;
        }

        #endregion

        #region 公共静态方法

        public static S_W_WBS GetWBSByID(string wbsID, bool canReturnNull = false)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var result = entities.S_W_WBS.FirstOrDefault(d => d.ID == wbsID);
            if (!canReturnNull)
                if (result == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
            return result;
        }

        #endregion

        #region 内部方法

        string getSerialCode()
        {
            var result = "";
            if (this.StructNodeInfo != null && !String.IsNullOrEmpty(this.StructNodeInfo.CodeDefine))
            {
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                int number = 1;
                if (this.Parent.Children.Count > 0)
                {
                    var maxCodeIndex = this.Parent.Children.Max(c => c.CodeIndex);
                    if (maxCodeIndex.HasValue)
                    {
                        number = maxCodeIndex.Value + 1;
                    }
                }
                //Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
                Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
                result = reg.Replace(this.StructNodeInfo.CodeDefine, (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');
                    if (value.IndexOf(".") > 0)
                    {
                        var array = value.Split('.');
                        if (array[0].ToLower() == "parent" && this.Parent != null && this.Parent.GetProperty(array[1], true) != null)
                        {
                            return this.Parent.GetPropertyString(array[1]);
                        }
                        else if (array[0].ToLower() == "project" && this.S_I_ProjectInfo != null && this.S_I_ProjectInfo.GetProperty(array[1], true) != null)
                        {
                            return this.S_I_ProjectInfo.GetPropertyString(array[1]);
                        }
                    }
                    else if (this.GetProperty(value, true) != null)
                    {
                        return this.GetPropertyString(value);
                    }
                    if (value.Replace('N', ' ').Trim() == "") //顺序号
                    {
                        var _rtn = number.ToString("D" + value.Length);
                        if (string.IsNullOrEmpty(this.WBSValue))
                            this.WBSValue = _rtn;
                        return _rtn;
                    }
                    switch (value)
                    {
                        case "yyyy":
                        case "YYYY":
                            return DateTime.Now.ToString("yyyy");
                        case "yy":
                        case "YY":
                            return DateTime.Now.ToString("yy");
                        case "mm":
                        case "MM":
                            return DateTime.Now.ToString("MM");
                        case "dd":
                        case "DD":
                            return DateTime.Now.ToString("dd");
                    }
                    return m.Value;
                });

                //自动补全
                var padLength = this.StructNodeInfo.PadLength;
                if (padLength.HasValue && padLength > 0)
                {
                    if (result.Length < padLength)
                    {
                        if (!string.IsNullOrEmpty(this.StructNodeInfo.PadChar))
                        {
                            var padChar = this.StructNodeInfo.PadChar[0];
                            if (this.StructNodeInfo.PadType == "left")
                                result = result.PadLeft(padLength.Value, padChar);
                            else
                                result = result.PadRight(padLength.Value, padChar);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="validateMode"></param>
        /// <param name="sychToTaskWork"></param>
        internal S_W_WBS AddChildWBS(S_W_WBS child, bool validateMode = true, bool sychToTaskWork = false)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (String.IsNullOrEmpty(child.WBSType))
                throw new Formula.Exceptions.BusinessException("必须指定WBS节点的类型");
            if (entities.Entry(child).State != System.Data.EntityState.Detached && entities.Entry(child).State != System.Data.EntityState.Added)
                throw new Formula.Exceptions.BusinessException("非新增状态的WBS对象，无法调用AddChild方法");
            child.ProjectInfoID = this.ProjectInfoID;
            child.S_I_ProjectInfo = this.S_I_ProjectInfo;
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.Level = this.Level + 1;
            child.WBSStructCode = child.WBSType;

            if (child.Parent == null || child.Parent.Children.Count == 0)
            {
                child.CodeIndex = 1;
            }
            else
            {
                var maxCodeIndex = child.Parent.Children.Max(c => c.CodeIndex);
                if (maxCodeIndex.HasValue)
                    child.CodeIndex = maxCodeIndex.Value + 1;
                else
                {
                    child.CodeIndex = 1;
                }
            }

            if (String.IsNullOrEmpty(child.State))
                child.State = ProjectCommoneState.Plan.ToString();
            if (child.SortIndex <= 0)
                child.SortIndex = child.Level * 1000 + this.Children.Count * 10;
            if (String.IsNullOrEmpty(child.WBSValue) && child.IsEnumNode)
                throw new Formula.Exceptions.BusinessException("枚举类WBS节点，必须指定WBSValue");

            child.SetDefaultValue();

            if (validateMode)
            {
                if (this.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("【" + this.Name + "】未获取WBS结构定义对象，无法新增子节点");
                if (!this.StructNodeInfo.ValidateChildren(child.WBSType))
                    throw new Formula.Exceptions.BusinessException("【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), this.WBSType)
                        + "】节点下不包含【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), child.WBSType) + "】的子节点定义，无法新增子节点");

                //如果定义中存在需要同步ChargeUserID的配置定义，则将ChargeUserID 填写到RBS表中
                if (child.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("模板定义中未能找到【" + child.WBSStructCode + "】定义，无法增加子节点");
                var structRoleDefine = child.StructNodeInfo.S_T_WBSStructRole.FirstOrDefault(d => d.SychWBS == "True");
                if (structRoleDefine != null && !string.IsNullOrEmpty(child.ChargeUserID))
                    child.SetUsers(structRoleDefine.RoleCode, child.ChargeUserID.Split(','));

                if (String.IsNullOrEmpty(child.Code) && child.StructNodeInfo != null && !String.IsNullOrEmpty(child.StructNodeInfo.CodeDefine))
                {
                    #region 自动填充编号
                    child.Code = child.getSerialCode();
                    #endregion
                }
            }
            
            if (String.IsNullOrEmpty(child.WBSValue))
                child.WBSValue = child.Code;
            if (String.IsNullOrEmpty(child.WBSValue))
                child.WBSValue = child.Name;
            child.CreateDate = DateTime.Now;
            child.CreateUser = user.UserName;
            child.CreateUserID = user.UserID;

            //表示需要将WBS节点同步增加到工作包数据表内
            if (sychToTaskWork && child.WBSType == WBSNodeType.Work.ToString())
            {
                if (child.S_W_TaskWork.Count == 0)
                {
                    var taskWork = new S_W_TaskWork();
                    taskWork.ID = FormulaHelper.CreateGuid();
                    taskWork.WBSID = child.ID;
                    taskWork.WBSFullID = child.FullID;
                    taskWork.ProjectInfoID = child.ProjectInfoID;
                    taskWork.ChargeDeptID = child.WBSDeptID;
                    taskWork.ChargeDeptName = child.WBSDeptName;
                    taskWork.Name = child.Name;
                    taskWork.Code = child.Code;
                    taskWork.PlanEndDate = child.PlanEndDate;
                    taskWork.PlanStartDate = child.PlanStartDate;
                    taskWork.CreateDate = DateTime.Now;
                    taskWork.CreateUser = user.UserName;
                    taskWork.CreateUserID = user.UserID;
                    taskWork.FillWBSUser(child);
                    //设置工作包的默认专业，阶段，或子项属性
                    if (String.IsNullOrEmpty(taskWork.MajorValue))
                    {
                        var majorNode = child.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
                        if (majorNode != null) taskWork.MajorValue = majorNode.WBSValue;
                    }

                    if (String.IsNullOrEmpty(taskWork.PhaseValue))
                    {
                        var phaseNode = child.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Phase.ToString());
                        if (phaseNode != null) taskWork.PhaseValue = phaseNode.WBSValue;
                    }

                    if (String.IsNullOrEmpty(taskWork.SubProjectCode))
                    {
                        var subProjectNode = child.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.SubProject.ToString());
                        if (subProjectNode != null) taskWork.SubProjectCode = subProjectNode.WBSValue;
                    }

                    if (String.IsNullOrEmpty(taskWork.AreaCode))
                    {
                        var areaNode = child.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Area.ToString());
                        if (areaNode != null) taskWork.AreaCode = areaNode.WBSValue;
                    }

                    if (String.IsNullOrEmpty(taskWork.DeviceCode))
                    {
                        var deviceNode = child.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Device.ToString());
                        if (deviceNode != null) taskWork.DeviceCode = deviceNode.WBSValue;
                    }

                    if (string.IsNullOrEmpty(taskWork.DossierCode))
                    {
                        var entityNode = child.Seniorities.FirstOrDefault(a => a.WBSType == WBSNodeType.Entity.ToString());
                        if (entityNode != null)
                        {
                            taskWork.DossierCode = entityNode.WBSValue;
                            taskWork.DossierName = entityNode.Name;
                        }
                    }

                    child.S_W_TaskWork.Add(taskWork);
                }
            }

            if (this.S_I_ProjectInfo.S_W_WBS.Count(d => d.ParentID == this.ID && d.WBSValue == child.WBSValue) > 0 && child.IsEnumNode)
                return this.S_I_ProjectInfo.S_W_WBS.FirstOrDefault(d => d.ParentID == this.ID && d.WBSValue == child.WBSValue);

            child.SetWBSCodes();
            child.ResetWBSDeptInfo();

            this.S_I_ProjectInfo.S_W_WBS.Add(child);
            this.Children.Add(child);

            if (String.IsNullOrEmpty(child.PhaseCode) && !String.IsNullOrEmpty(this.PhaseCode))
            {
                child.PhaseCode = this.PhaseCode;
            }
            this.onAddChild();
            return child;
        }

        public void SetDefaultValue()
        {
                if (this.StructNodeInfo != null && !string.IsNullOrEmpty(this.StructNodeInfo.DefaultValueJson))
                {
                    #region 设置默认值
                    var list = JsonHelper.ToList(this.StructNodeInfo.DefaultValueJson);
                    foreach (var item in list)
                    {
                        var field = item.GetValue("Field");
                        var value = item.GetValue("Value");
                        if (string.IsNullOrEmpty(value)) continue;
                        if (GetProperty(field, true) != null && !string.IsNullOrEmpty(this.GetPropertyString(field))) continue;

                        Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
                        var result = reg.Replace(value, (Match m) =>
                        {
                            string _value = m.Value.Trim('{', '}');
                            if (_value.IndexOf(".") > 0)
                            {
                                var array = _value.Split('.');
                                if (array[0].ToLower() == "parent" && this.Parent != null && this.Parent.GetProperty(array[1], true) != null)
                                {
                                    return this.Parent.GetPropertyString(array[1]);
                                }
                                else if (array[0].ToLower() == "project" && this.S_I_ProjectInfo != null && this.S_I_ProjectInfo.GetProperty(array[1], true) != null)
                                {
                                    return this.S_I_ProjectInfo.GetPropertyString(array[1]);
                                }
                            }
                            else if (this.GetProperty(_value, true) != null)
                            {
                                return this.GetPropertyString(_value);
                            }
                            return m.Value;
                        });

                        this.SetProperty(field, result);
                    }
                    #endregion
                }
        }

        /// <summary>
        /// 删除节点自身
        /// </summary>
        /// <param name="validateMode"></param>
        internal void DeleteSelf(bool validateMode = true)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (validateMode)
            {
                if (this.S_E_Product.Count > 0)
                    throw new Formula.Exceptions.BusinessException("已经有成果的WBS节点不允许删除");
            }
            this.ClearRBS();
            entities.S_W_TaskWork.Delete(d => d.WBSID == this.ID);
            foreach (var product in this.S_E_Product.ToList())
                product.Delete(validateMode);
            entities.S_P_MileStone.Delete(d => d.WBSID == this.ID);
            entities.S_W_WBS.Delete(d => d.ID == this.ID);
            entities.S_P_CooperationPlan.Delete(d => d.WBSID == this.ID);
            entities.S_W_Activity.Delete(d => d.WBSID == this.ID);
        }
        /// <summary>
        /// 该节点下是否有成果
        /// </summary>
        /// <returns></returns>
        public bool HasProducts()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            return entities.S_E_Product.Any(t => t.WBSFullID.StartsWith(this.FullID));
        }

        /// <summary>
        /// 默认设置WBS部门
        /// </summary>
        internal void ResetWBSDeptInfo()
        {
            var configEntiteies = this.GetDbContext<BaseConfigEntities>();
            var defAttr = configEntiteies.S_D_WBSAttrDefine.FirstOrDefault(d => d.Code == this.WBSValue);
            if (defAttr != null)
            {
                var relationDeptInfo = defAttr.S_D_WBSAttrDeptInfo.FirstOrDefault();
                if (relationDeptInfo != null)
                {
                    this.WBSDeptID = relationDeptInfo.DeptID;
                    this.WBSDeptName = relationDeptInfo.DeptName;
                }
            }
            else if (String.IsNullOrEmpty(this.WBSDeptID) && this.Parent != null)
            {
                this.WBSDeptID = Parent.WBSDeptID;
                this.WBSDeptName = Parent.WBSDeptName;
            }
        }

        /// <summary>
        /// 设置WBS分类码
        /// </summary>
        internal void SetWBSCodes()
        {
            //获取所有的上级节点，填充WBS类别属性字段
            foreach (var seniority in this.Seniorities)
            {
                var fieldName = seniority.WBSType + "Code";
                this.SetProperty(fieldName, seniority.WBSValue);//c_hua 将上层节点的WBSValue 赋于本节点的分类Code
            }
            //填充WBS自身的类别属性字段
            this.SetProperty(this.WBSType + "Code", this.WBSValue);
        }

        #endregion

        #region 私有方法


        /// <summary>
        /// 在设置人员时，将人员填充至OBS体系内
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="role">岗位角色编码</param>
        void FiilToOBS(string userID, ProjectRole role, bool resetOBSFromRBS = false)
        {
            var userInfo = FormulaHelper.GetUserInfoByID(userID);
            this.FiilToOBS(userInfo, role.ToString(), resetOBSFromRBS);
        }

        /// <summary>
        /// 在设置人员时，将人员填充至OBS体系内
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roleCode">岗位角色编码</param>
        void FiilToOBS(string userID, string roleCode, bool resetOBSFromRBS = false)
        {
            var userInfo = FormulaHelper.GetUserInfoByID(userID);
            this.FiilToOBS(userInfo, roleCode, resetOBSFromRBS);
        }

        /// <summary>
        /// 在设置人员时，将人员填充至OBS体系内
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roleCode">岗位角色编码</param>
        void FiilToOBS(UserInfo user, string roleCode, bool resetOBSFromRBS = false)
        {
            string majorValue = String.Empty;
            string majorName = String.Empty;
            if (this.WBSType == WBSNodeType.Major.ToString())
            {
                majorValue = this.WBSValue;
                majorName = this.Name;
            }
            else
            {
                var major = this.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
                if (major != null)
                {
                    majorValue = major.WBSValue;
                    majorName = major.Name;
                }
            }
            this.S_I_ProjectInfo.SetOBSUser(user, roleCode, majorValue, majorName, resetOBSFromRBS);
        }

        void FillToTask(string userID, string userName, string roleCode)
        {
            var idKey = roleCode + "UserID";
            var nameKey = roleCode + "UserName";
            var task = this.S_W_TaskWork.FirstOrDefault();
            var taskUserID = task.GetPropertyString(idKey);
            if (userID != taskUserID)
            {
                task.SetProperty(idKey, userID);
                task.SetProperty(nameKey, userName);
            }
        }

        #endregion

        #region 分布扩展方法
        /// <summary>
        /// 当增加工作包后执行
        /// </summary>
        partial void onAddTaskWork();

        /// <summary>
        /// 当WBS增加完子节点后执行
        /// </summary>
        partial void onAddChild();

        /// <summary>
        /// 当WBS被删除后执行
        /// </summary>
        partial void onDelete();

        /// <summary>
        /// 设置完人员后执行
        /// </summary>
        partial void onSetUser();
        #endregion
    }
}
