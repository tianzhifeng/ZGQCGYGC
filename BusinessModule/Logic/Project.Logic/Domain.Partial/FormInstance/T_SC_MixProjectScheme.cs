using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula.Helper;
using Formula;
using Config.Logic;

namespace Project.Logic.Domain
{
    public partial class T_SC_MixProjectScheme
    {
        /// <summary>
        /// 流程结束反写业务逻辑
        /// </summary>
        public void Push(Dictionary<string, string> dic)
        {

            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            //获取项目信息
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            var rootWBS = projectInfo.WBSRoot;
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            List<Dictionary<string, object>> majorUserList = null;

            if (this.PlanStartDate.HasValue)
                projectInfo.PlanStartDate = this.PlanStartDate;
            if (this.PlanFinishDate.HasValue)
                projectInfo.PlanFinishDate = this.PlanFinishDate;

            if (!String.IsNullOrEmpty(this.Code))
                projectInfo.Code = this.Code;

            //此处批量清空专业人员信息；
            //string[] roles = new string[] { ProjectRole.ProjectManager.ToString(),
            //     ProjectRole.MajorPrinciple.ToString(),ProjectRole.MajorEngineer.ToString(),
            //ProjectRole.Designer.ToString(),ProjectRole.Collactor.ToString(),
            //ProjectRole.Auditor.ToString(),ProjectRole.Approver.ToString()};
            //projectEntities.Set<S_W_RBS>().Delete(a => roles.Contains(a.RoleCode) && a.ProjectInfoID == projectInfo.ID);
            projectEntities.Set<S_W_RBS>().Delete(a => a.ProjectInfoID == projectInfo.ID);


            //将项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
            this.SynchRBSUser(dic, rootWBS);

            if (!string.IsNullOrEmpty(this.MajorList))
                majorUserList = JsonHelper.ToList(this.MajorList);
            projectInfo.SynchMajorData(this.Major);
            //从RBS同步人员到obs
            projectInfo.ResetOBSUserFromRBS();
            //根据子项信息和专业信息同步WBS节点
            this.SynchSubProjectWBSNode(dic, rootWBS);
            projectInfo.State = ProjectCommoneState.Execute.ToString();

        }

        public void PushMileStoneList()
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            var rootWBS = projectInfo.WBSRoot;
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            var detailMileStoneList=  this.T_SC_MixProjectScheme_MileStoneList.ToList();
            #region 删除里程碑
            var selectCodes = detailMileStoneList.Select(a => a.Code).ToList();
            var deleteMileStoneList = projectInfo.S_P_MileStone.Where(a => !selectCodes.Contains(a.Code) && a.WBSID == projectInfo.WBSRoot.ID).ToList();
            foreach (var item in deleteMileStoneList)
                item.Delete();
            #endregion
            #region 更新进度计划
            for (int i = 0; i < detailMileStoneList.Count; i++)
            {
                var item = detailMileStoneList.ToList()[i];
                var mileStone = projectEntities.S_P_MileStone.FirstOrDefault(d => d.Code == item.Code && d.WBSID == projectInfo.WBSRoot.ID &&
                  d.ProjectInfoID == this.ProjectInfoID);
                if (mileStone == null)
                {
                    mileStone = new S_P_MileStone();
                    mileStone.ID = FormulaHelper.CreateGuid();
                    mileStone.Name = item.Name;
                    mileStone.Code = item.Code;
                    mileStone.MileStoneValue = item.Code;
                    mileStone.WBSID = projectInfo.WBSRoot.ID;
                    mileStone.ProjectInfoID = projectInfo.ID;
                    mileStone.OrlPlanFinishDate = item.PlanEndDate;
                    mileStone.S_I_ProjectInfo = projectInfo;
                }
                if (mileStone.State == ProjectCommoneState.Finish.ToString())
                    continue;
                mileStone.PlanFinishDate = item.PlanEndDate;
                mileStone.Weight = item.Weight;
                mileStone.MajorValue = item.Major;
                mileStone.TemplateID = item.TemplateID;
                mileStone.MileStoneType = item.MileStoneType;
                mileStone.Name = item.Name;
                if (projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_MsDataIsFromLastVertion") == TrueOrFalse.True.ToString())
                    mileStone.SortIndex = Convert.ToInt32(item.SortIndex.HasValue ? Convert.ToDecimal(item.SortIndex) * 100 : i);
                else
                {
                    var template = projectInfo.ProjectMode.S_T_MileStone.FirstOrDefault(d => d.ID == item.TemplateID);
                    if (template != null)
                    {
                        mileStone.SortIndex = template.SortIndex;
                    }
                    if (!mileStone.SortIndex.HasValue)
                    {
                        mileStone.SortIndex = item.SortIndex.HasValue ? Convert.ToInt32(item.SortIndex) : i;
                    }
                }
                mileStone.Description = item.Remark;
                mileStone.Save();
                if (mileStone.MileStoneType == MileStoneType.Cooperation.ToString())
                {
                    mileStone.OutMajorValue = item.InMajor;
                    var cooperationPlan = projectEntities.S_P_CooperationPlan.FirstOrDefault(d => d.SchemeWBSID == projectInfo.WBSRoot.ID
                       && d.CooperationValue == mileStone.MileStoneValue);
                    if (cooperationPlan == null)
                    {
                        cooperationPlan = new S_P_CooperationPlan();
                        cooperationPlan.InMajorValue =  String.Join(",", item.InMajor.Split(',').Where(d => this.Major.Split(',').Contains(d)));
                        cooperationPlan.OutMajorValue = item.OutMajor;
                        cooperationPlan.MileStoneID = mileStone.ID;
                        cooperationPlan.ID = FormulaHelper.CreateGuid();
                        cooperationPlan.CooperationContent = mileStone.Name;
                        cooperationPlan.CooperationValue = mileStone.MileStoneValue;
                        cooperationPlan.OrPlanFinishDate = item.PlanEndDate;
                        cooperationPlan.PlanFinishDate = item.PlanEndDate;
                    }
                    if (!cooperationPlan.OrPlanFinishDate.HasValue)
                        cooperationPlan.OrPlanFinishDate = item.PlanEndDate;
                    cooperationPlan.PlanFinishDate = item.PlanEndDate;
                    if (projectInfo.WBSRoot != null)
                        projectInfo.WBSRoot.SaveCooperationPlan(cooperationPlan);
                }
            }
            #endregion
        }

        /// <summary>
        /// 同步项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
        /// </summary>
        public void SynchRBSUser(Dictionary<string, string> dic, S_W_WBS rootWBS = null)
        {
            if (rootWBS == null)
            {
                var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
                //获取项目信息
                var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
                rootWBS = projectInfo.WBSRoot;
                if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            }
            if (rootWBS.WBSType != WBSNodeType.Project.ToString()) throw new Formula.Exceptions.BusinessException("非根节点WBS，不可将项目主要干系人信息同步至RBS");
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            //将项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
            rootWBS.SetUsers(ProjectRole.ProjectManager.ToString(), this.ChargeUser.Split(','), true, true);//项目负责人
            //c_hua 2018/10/08 根据角色定义与表单字段动态写入RBS
            var roleList = BaseConfigFO.GetRoleDefineList().Where(a => dic.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(dic.GetValue(a.RoleCode)));
            foreach (var roleDef in roleList)
                rootWBS.SetUsers(roleDef.RoleCode, dic.GetValue(roleDef.RoleCode).Split(','), true, true);
        }

        /// <summary>
        /// 根据子项和专业列表同步WBS表
        /// </summary>
        public void SynchSubProjectWBSNode(Dictionary<string, string> dic, S_W_WBS rootWBS = null)
        {
            var userInfo = FormulaHelper.GetUserInfo();
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            if (rootWBS == null)
            {
                //获取项目信息             
                rootWBS = projectInfo.WBSRoot;
                if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            }

            var roleDefineList = BaseConfigFO.GetRoleDefineList();
            var enumSubProject = WBSNodeType.SubProject.ToString();
            var subwbses = rootWBS.Children.ToList();
            if (this.T_SC_MixProjectScheme_SubProjectList.ToList().Count == 0)   //如果没有策划任何子项，则默认建立一个子项，子项名称为项目阶段
            {
                SynchMajorWBSNode(roleDefineList, rootWBS, "");
                /*
                foreach (var item in projectInfo.PhaseValue.Split(','))
                {

                }
                var subProjectListItem = new T_SC_MixProjectScheme_SubProjectList();
                subProjectListItem.ID = FormulaHelper.CreateGuid(); ;
                subProjectListItem.Name = projectInfo.PhaseName;
                subProjectListItem.Code = projectInfo.PhaseValue;
                subProjectListItem.Unit = "";
                subProjectListItem.PhaseValue = projectInfo.PhaseValue;
                this.T_SC_MixProjectScheme_SubProjectList.Add(subProjectListItem);
                this.SubProjectList = JsonHelper.ToJson(this.T_SC_MixProjectScheme_SubProjectList);
                 */
            }

            foreach (var subProject in this.T_SC_MixProjectScheme_SubProjectList.OrderBy(d => d.SortIndex).ToList())
            {
                var subWBSNode = subwbses.FirstOrDefault(d => d.WBSValue == subProject.Code && d.WBSType == WBSNodeType.SubProject.ToString());
                if (subWBSNode == null)
                {
                    subWBSNode = new S_W_WBS();
                    subWBSNode.ID = FormulaHelper.CreateGuid();
                    subWBSNode.WBSType = WBSNodeType.SubProject.ToString();
                    subWBSNode.Name = subProject.Name;
                    subWBSNode.Code = "";
                    subWBSNode.WBSValue = subProject.Code;
                    subWBSNode.PhaseCode = subProject.PhaseValue;
                    subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                    subWBSNode.ExtField2 = subProject.Unit;
                    rootWBS.AddChild(subWBSNode);
                }
                else
                {
                    subWBSNode.Name = subProject.Name;
                    subWBSNode.PhaseCode = subProject.PhaseValue;
                    subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                    subWBSNode.ExtField2 = subProject.Unit;
                    subWBSNode.Save();
                    subwbses.Remove(subWBSNode);
                }
                #region 新增单体
                var wonomerList = subWBSNode.S_W_Monomer.ToList();
                if (String.IsNullOrEmpty(subProject.Unit))
                {
                    var wonomer = wonomerList.FirstOrDefault(d => d.Name == subProject.Name);
                    if (wonomer == null)
                    {
                        wonomer = new S_W_Monomer();
                        wonomer.ID = FormulaHelper.CreateGuid();
                        wonomer.Name = subProject.Name;
                        wonomer.ProjectInfoID = this.ProjectInfoID;
                        wonomer.Code = "";
                        wonomer.CreateDate = DateTime.Now;
                        wonomer.CreateUser = userInfo.UserName;
                        wonomer.CreateUserID = userInfo.UserID;
                        wonomer.SchemeFormSubID = this.ID;
                        subWBSNode.S_W_Monomer.Add(wonomer);
                    }
                }
                else
                {

                    foreach (var item in subProject.Unit.Replace("，", ",").Split(','))
                    {
                        var wonomer = subWBSNode.S_W_Monomer.FirstOrDefault(d => d.Name == item);
                        if (wonomer == null)
                        {
                            wonomer = new S_W_Monomer();
                            wonomer.ID = FormulaHelper.CreateGuid();
                            wonomer.Name = item;
                            wonomer.ProjectInfoID = this.ProjectInfoID;
                            wonomer.Code = "";
                            wonomer.CreateDate = DateTime.Now;
                            wonomer.CreateUser = userInfo.UserName;
                            wonomer.CreateUserID = userInfo.UserID;
                            wonomer.SchemeFormSubID = this.ID;
                            subWBSNode.S_W_Monomer.Add(wonomer);
                        }
                        else
                        {
                            wonomerList.Remove(wonomer);
                        }

                    }
                }
                foreach (var delWonomer in wonomerList)   //移除不需要的单体
                {
                    projectEntities.S_W_Monomer.Remove(delWonomer);
                }

                #endregion

                SynchMajorWBSNode(roleDefineList, subWBSNode, subProject.RBSJson);
                //this.InitMileStone(subWBSNode);
            }
            subwbses.ForEach(t => t.Delete());  //删除多余的子项
        }

        /// <summary>
        /// 根据专业同步WBS
        /// </summary>
        /// <param name="parentNode"></param>
        public void SynchMajorWBSNode(List<S_D_RoleDefine> roleDefineList, S_W_WBS parentNode, string rbsList = "")
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            if (parentNode == null) throw new Formula.Exceptions.BusinessException("传入的WBS节点为空。");
            var majorList = "";
            if (!string.IsNullOrEmpty(rbsList))
                majorList = rbsList;
            else
                majorList = this.MajorList;

            #region 删除不需要的专业
            var delMajors = parentNode.Children.Where(d => !this.Major.Contains(d.WBSValue)).ToList();
            foreach (var item in delMajors)
            {
                item.Delete();
                parentNode.Children.Remove(item);
            }
            #endregion

            if (!string.IsNullOrEmpty(majorList))
            {
                var majorNodes = JsonHelper.ToList(majorList);
                var majorAttrList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
                var majorWBSType = WBSNodeType.Major.ToString();
                foreach (var majorNode in majorNodes)
                {
                    if (majorNode.GetValue("Valid") == "0")
                    {
                        var thisMajorValue = majorNode.GetValue("MajorCode");
                        var thisMajorNode = parentNode.Children.SingleOrDefault(c => c.WBSValue == thisMajorValue);
                        if (thisMajorNode != null)
                            thisMajorNode.Delete();
                        continue;
                    }
                    var value = majorNode.GetValue("MajorCode");
                    var majorAttr = majorAttrList.SingleOrDefault(c => c.Code == value);
                    if (majorAttr == null) continue;
                    var childNode = parentNode.Children.SingleOrDefault(c => c.WBSValue == value);
                    if (!this.Major.Split(',').Contains(value))
                    {
                        if (childNode != null)
                            childNode.Delete();
                        continue;
                    }
                    if (childNode == null)
                    {
                        childNode = projectEntities.Set<S_W_WBS>().Create();
                        if (string.IsNullOrEmpty(childNode.ID))
                            childNode.ID = FormulaHelper.CreateGuid();
                        childNode._state = EntityState.Added.ToString();
                        childNode.WBSType = majorWBSType;
                        childNode.Name = majorAttr.Name;
                        childNode.WBSValue = majorAttr.Code;
                        childNode.Code = majorAttr.WBSCode;
                    }
                    childNode.SortIndex = majorAttr.SortIndex;
                    childNode.ChargeUserID = majorNode.GetValue("MajorPrinciple");
                    childNode.ChargeUserName = majorNode.GetValue("MajorPrincipleName");
                    if (childNode._state == EntityState.Added.ToString())
                        parentNode.AddChild(childNode);

                    #region 同步设校审人员信息
                    /*
                    //同步设置专业总工
                    var userIDs = majorNode.GetValue("MajorEngineer");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.MajorEngineer.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定专业负责人
                    userIDs = majorNode.GetValue("MajorPrinciple");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.MajorPrinciple.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定设计人
                    userIDs = majorNode.GetValue("Designer");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Designer.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定校核人
                    userIDs = majorNode.GetValue("Collactor");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Collactor.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定审核人
                    userIDs = majorNode.GetValue("Auditor");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Auditor.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定审定人
                    userIDs = majorNode.GetValue("Approver");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Approver.ToString(), userIDs.Split(','), true, false, true, true);
                     */
                    var roleList = roleDefineList.Where(a => majorNode.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(majorNode.GetValue(a.RoleCode)));
                    foreach (var roleDef in roleList)
                        childNode.SetUsers(roleDef.RoleCode, majorNode.GetValue(roleDef.RoleCode).Split(','), true, false, true, true);
                    #endregion
                }
            }
        }
    }
}
