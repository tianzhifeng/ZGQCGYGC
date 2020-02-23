using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Formula.Helper;
using Formula;
using Config.Logic;

namespace Project.Logic.Domain
{
    public partial class T_SC_SingleProjectScheme
    {
        public void Push(Dictionary<string, string> dic)
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            var rootWBS = projectInfo.WBSRoot;
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            if (!String.IsNullOrEmpty(this.Code))
                projectInfo.Code = this.Code;

            if (this.PlanStartDate.HasValue)
                projectInfo.PlanStartDate = this.PlanStartDate;
            if (this.PlanFinishDate.HasValue)
                projectInfo.PlanFinishDate = this.PlanFinishDate;
            
            this.SynchRBSUser(dic);
            var selectMajorList = this.Major.Split(',');

            #region 回写专业信息
            //移除未选中的专业
            var removeMajorList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()
                && !selectMajorList.Contains(d.WBSValue)).ToList();
            foreach (var item in removeMajorList)
            {
                item.Delete();
            }

            var roleDefineList = BaseConfigFO.GetRoleDefineList();
            var majorAttrList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);

            if (!string.IsNullOrEmpty(this.MajorList))
            {
                var majorNodes = JsonHelper.ToList(this.MajorList);
                foreach (var item in majorNodes)
                {
                    var majorCode = item.GetValue("MajorCode");
                    var attr = majorAttrList.FirstOrDefault(d => d.Code == majorCode);
                    if (attr == null) throw new Formula.Exceptions.BusinessException("未定义专业【" + majorCode + "】" +item.GetValue("MajorName") + "，操作失败");
                    var major = projectInfo.S_W_WBS.FirstOrDefault(d => d.WBSValue == majorCode && d.WBSType == WBSNodeType.Major.ToString());
                    if (major == null)
                    {
                        major = new S_W_WBS();
                        major.Name = attr.Name;
                        major.Code = attr.WBSCode;
                        major.WBSValue = attr.Code;
                        major.WBSType = WBSNodeType.Major.ToString();
                        major.SortIndex = attr.SortIndex;
                        projectInfo.WBSRoot.AddChild(major);
                    }
                    else
                        major.PhaseCode = projectInfo.PhaseValue;
                    //if (!string.IsNullOrEmpty(item.MajorPrinciple))
                    //    major.SetUsers(ProjectRole.MajorPrinciple.ToString(), item.MajorPrinciple.Split(','), true, true, true, true);
                    //if (!string.IsNullOrEmpty(item.MajorEngineer))
                    //    major.SetUsers(ProjectRole.MajorEngineer.ToString(), item.MajorEngineer.Split(','), true, true, true, true);
                    //if (!string.IsNullOrEmpty(item.Designer))
                    //    major.SetUsers(ProjectRole.Designer.ToString(), item.Designer.Split(','), true, true, true, true);
                    //if (!string.IsNullOrEmpty(item.Collactor))
                    //    major.SetUsers(ProjectRole.Collactor.ToString(), item.Collactor.Split(','), true, true, true, true);
                    //if (!string.IsNullOrEmpty(item.Auditor))
                    //    major.SetUsers(ProjectRole.Auditor.ToString(), item.Auditor.Split(','), true, true, true, true);
                    //if (!string.IsNullOrEmpty(item.Approver))
                    //    major.SetUsers(ProjectRole.Approver.ToString(), item.Approver.Split(','), true, true, true, true);
                    var roleList = roleDefineList.Where(a => item.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(item.GetValue(a.RoleCode)));
                    foreach (var roleDef in roleList)
                        major.SetUsers(roleDef.RoleCode, item.GetValue(roleDef.RoleCode).Split(','), true, true, true, true);
                }
            }
            var wbsMajorList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()).
                Select(d => new { Name = d.Name, Value = d.WBSValue, SortIndex = d.SortIndex }).OrderBy(c => c.SortIndex).ToList();
            projectInfo.Major = JsonHelper.ToJson(wbsMajorList);
            var wonomerList = projectInfo.WBSRoot.S_W_Monomer.ToList();
            var userInfo = FormulaHelper.GetUserInfo();
            if (wonomerList.Count == 0)
            {
                var wonomer = new S_W_Monomer();
                wonomer.ID = FormulaHelper.CreateGuid();
                wonomer.Name = projectInfo.Name;
                wonomer.ProjectInfoID = this.ProjectInfoID;
                wonomer.Code = "";
                wonomer.CreateDate = DateTime.Now;
                wonomer.CreateUser = userInfo.UserName;
                wonomer.CreateUserID = userInfo.UserID;
                wonomer.SchemeFormSubID = this.ID;
                projectInfo.WBSRoot.S_W_Monomer.Add(wonomer);
            }
            #endregion

            var detailMileStoneList = this.T_SC_SingleProjectScheme_MileStoneList.ToList();
            #region 删除里程碑
            var selectCodes = detailMileStoneList.Select(a => a.Code).ToList();
            var deleteMileStoneList = projectInfo.S_P_MileStone.Where(a => !selectCodes.Contains(a.Code)).ToList();
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
                        cooperationPlan.InMajorValue = item.InMajor;
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
            //根据RBS更新OBS
            projectInfo.ResetOBSUserFromRBS();
            projectInfo.State = ProjectCommoneState.Execute.ToString();
        }

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
            rootWBS.SetUsers(ProjectRole.ProjectManager.ToString(), this.ChargeUser.Split(','), true, true);//项目负责人
            //c_hua 2018/10/08 根据角色定义与表单字段动态写入RBS
            var roleList = BaseConfigFO.GetRoleDefineList().Where(a => dic.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(dic.GetValue(a.RoleCode)));
            foreach (var roleDef in roleList)
                rootWBS.SetUsers(roleDef.RoleCode, dic.GetValue(roleDef.RoleCode).Split(','), true, true);
        }
    }
}
