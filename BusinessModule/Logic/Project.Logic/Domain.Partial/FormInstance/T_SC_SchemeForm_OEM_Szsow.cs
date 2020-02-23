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
    public partial class T_SC_SchemeForm_OEM_Szsow
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
            //根据阶段、子项信息、专业信息同步WBS节点
            this.SynchWBSNode(dic, rootWBS);
            projectInfo.State = ProjectCommoneState.Execute.ToString();
            projectInfo.ModifyDate = DateTime.Now;
            projectInfo.ModifyUser = this.CreateUser;
            projectInfo.ModifyUserID = this.CreateUserID;

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
        /// 根据阶段、子项信息、专业信息同步WBS节点
        /// </summary>
        public void SynchWBSNode(Dictionary<string, string> dic, S_W_WBS rootWBS = null)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();
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
            var detailList = this.T_SC_SchemeForm_OEM_Szsow_SubProjectList.OrderBy(d => d.SortIndex).ToList();

            var roleDefineList = BaseConfigFO.GetRoleDefineList();
            var phaseWBSes = rootWBS.Children.ToList();
            var phaseAttrs = BaseConfigFO.GetWBSAttrList(WBSNodeType.Phase);
            foreach (var phaseValue in projectInfo.PhaseValue.Split(','))
            {
                #region 阶段节点

                var phaseName = enumService.GetEnumText("Project.Phase", phaseValue);
                var attr = phaseAttrs.FirstOrDefault(a => a.Code == phaseValue);
                if (attr == null) throw new Formula.Exceptions.BusinessException("未定义阶段【" + phaseValue + "】" + phaseName + "，操作失败");
                var phaseWBSNode = phaseWBSes.FirstOrDefault(d => d.WBSValue == phaseValue
                    && d.WBSType == WBSNodeType.Phase.ToString());
                if (phaseWBSNode == null)
                {
                    phaseWBSNode = new S_W_WBS();
                    phaseWBSNode.ID = FormulaHelper.CreateGuid();
                    phaseWBSNode.WBSType = WBSNodeType.Phase.ToString();
                    phaseWBSNode.Name = phaseName;
                    phaseWBSNode.Code = attr.WBSCode;
                    phaseWBSNode.WBSValue = attr.Code;
                    rootWBS.AddChild(phaseWBSNode);
                }
                else
                {
                    phaseWBSNode.Name = phaseName;
                    phaseWBSNode.Code = attr.WBSCode;
                    phaseWBSNode.WBSValue = attr.Code;
                    phaseWBSNode.Save();
                    phaseWBSes.Remove(phaseWBSNode);
                }
                #endregion

                #region 子项节点
                var subwbses = phaseWBSNode.Children.ToList();
                foreach (var subProject in detailList.Where(a => a.PhaseValue.Split(',').Contains(phaseValue)).ToList())
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
                        subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                        subWBSNode.ExtField2 = subProject.Unit;
                        phaseWBSNode.AddChild(subWBSNode);
                    }
                    else
                    {
                        subWBSNode.Name = subProject.Name;
                        subWBSNode.WBSValue = subProject.Code;
                        subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                        subWBSNode.ExtField2 = subProject.Unit;
                        subWBSNode.Save();
                        subwbses.Remove(subWBSNode);
                    }
                    subProject.PhaseWBSID = phaseWBSNode.ID;
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

                    //增加名称判断逻辑
                    if (string.IsNullOrEmpty(subProject.Name))
                    {                       
                        //将空名子项的专业挂到阶段下
                        SynchMajorWBSNode(roleDefineList,subWBSNode.Parent, subProject.RBSJson);
                        //删除空名子项
                        subWBSNode.Delete();

                        //排除在阶段下的专业
                        var rbsDicList = JsonHelper.ToList(subProject.RBSJson ?? MajorList);
                        subwbses.RemoveAll(a => rbsDicList.Any(b => a.WBSValue == b.GetValue("MajorCode")));
                    }
                    else
                    {
                        subProject.SubProjectWBSID = subWBSNode.ID;
                        SynchMajorWBSNode(roleDefineList,subWBSNode, subProject.RBSJson);
                    }
                    //this.InitMileStone(subWBSNode);
                }
                #endregion
                subwbses.ForEach(t => t.Delete());  //删除多余的子项

            }
            phaseWBSes.ForEach(t => t.Delete());  //删除多余的阶段

            this.SubProjectList = JsonHelper.ToJson<List<T_SC_SchemeForm_OEM_Szsow_SubProjectList>>(detailList);
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
            //sll edit 2018-8-8 
            //改前var delMajors = parentNode.Children.Where(d => !this.Major.Contains(d.WBSValue)).ToList();
            var delMajors = parentNode.Children.Where(d => !this.Major.Contains(d.WBSValue) && d.WBSType == WBSNodeType.Major.ToString()).ToList();
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
