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
    public partial class T_SC_ElectricalPowerProjectScheme
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

            #region 更新预留工时、管理工时CBS
            var cbsRoot = projectInfo.CBSRoot;
            var nodeType = CBSNodeType.Category.ToString();
            var categoryType = CBSCategoryType.Reserve.ToString();
            var enums = EnumBaseHelper.GetEnumDef(typeof(CBSCategoryType));
            //预留工时
            var reserveCBS = cbsRoot.Children.FirstOrDefault(d => d.NodeType == nodeType && d.Code == categoryType);
            if (reserveCBS == null)
            {
                reserveCBS = new S_C_CBS();
                reserveCBS.Name = enums.EnumItem.FirstOrDefault(a => a.Code == categoryType).Name;
                reserveCBS.Code = categoryType;
                reserveCBS.NodeType = nodeType;
                reserveCBS.CBSType = CBSType.LabourExpense.ToString();
                reserveCBS.SortIndex = 0;
                reserveCBS.CreateUserID = this.CreateUserID;
                reserveCBS.CreateUser = this.CreateUser;
                cbsRoot.AddChild(reserveCBS);
            }
            reserveCBS.Quantity = this.ReserveWorkload ?? 0;
            reserveCBS.TotalPrice = reserveCBS.Quantity.Value * reserveCBS.UnitPrice.Value;
            //管理工时
            categoryType = CBSCategoryType.Manage.ToString();
            var manageCBS = cbsRoot.Children.FirstOrDefault(d => d.NodeType == nodeType && d.Code == categoryType);
            if (manageCBS == null)
            {
                manageCBS = new S_C_CBS();
                manageCBS.Name = enums.EnumItem.FirstOrDefault(a => a.Code == categoryType).Name;
                manageCBS.Code = categoryType;
                manageCBS.NodeType = nodeType;
                manageCBS.CBSType = CBSType.LabourExpense.ToString();
                manageCBS.SortIndex = 1;
                manageCBS.CreateUserID = this.CreateUserID;
                manageCBS.CreateUser = this.CreateUser;
                cbsRoot.AddChild(manageCBS);
            }
            //删除不存在的
            var subManageWorkloadList = this.T_SC_ElectricalPowerProjectScheme_ManageWorkloadList.ToList();
            var removeManageList = manageCBS.Children.Where(d => !subManageWorkloadList.Select(a => a.Item).Contains(d.Code)).ToList();
            foreach (var item in removeManageList)
                item.Delete();
            foreach (var item in subManageWorkloadList)
            {
                var _mcbs = manageCBS.Children.FirstOrDefault(a => a.Code == item.Item);
                if (_mcbs == null)
                {
                    _mcbs = new S_C_CBS();
                    _mcbs.Name = item.ItemName;
                    _mcbs.Code = item.Item;
                    _mcbs.NodeType = CBSNodeType.CBS.ToString();
                    _mcbs.CBSType = CBSType.LabourExpense.ToString();
                    _mcbs.SortIndex = item.SortIndex;
                    _mcbs.CreateUserID = this.CreateUserID;
                    _mcbs.CreateUser = this.CreateUser;
                    manageCBS.AddChild(_mcbs);
                }
                else
                {
                    _mcbs.ModifyUserID = this.CreateUserID;
                    _mcbs.ModifyUser = this.CreateUser;
                    _mcbs.ModifyDate = DateTime.Now;
                }
                _mcbs.Quantity = item.ManageWorkload ?? 0;
                _mcbs.TotalPrice = _mcbs.Quantity.Value * _mcbs.UnitPrice.Value;
            }
            manageCBS.SummayQuantity();
            #endregion

            var ProjectTaskWorkList = new List<S_W_TaskWork>();
            var DetailTaskWorkList = this.T_SC_ElectricalPowerProjectScheme_TaskWorkList.ToList();
            #region 删除卷册
            if (!string.IsNullOrEmpty(this.TaskWorkList))
            {
                var selectids = DetailTaskWorkList.Select(a => a.TaskWorkID).ToList();
                ProjectTaskWorkList = projectEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfo.ID).ToList();
                var deleteList = ProjectTaskWorkList.Where(a => !selectids.Contains(a.ID)).ToList();
                foreach (var item in deleteList)
                {
                    item.Delete();
                    ProjectTaskWorkList.Remove(item);
                }
            }
            #endregion

            #region 回写专业信息，专业工时CBS
            //移除未选中的专业
            var removeMajorList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()
                && !selectMajorList.Contains(d.WBSValue)).ToList();
            foreach (var item in removeMajorList)
            {
                item.Delete();
            }

            #region 专业工时
            
            categoryType = CBSCategoryType.Product.ToString();
            var productCBS = cbsRoot.Children.FirstOrDefault(d => d.NodeType == nodeType && d.Code == categoryType);
            if (productCBS == null)
            {
                productCBS = new S_C_CBS();
                productCBS.Name = enums.EnumItem.FirstOrDefault(a => a.Code == categoryType).Name;
                productCBS.Code = categoryType;
                productCBS.NodeType = nodeType;
                productCBS.CBSType = CBSType.LabourExpense.ToString();
                productCBS.SortIndex = 2;
                productCBS.CreateUserID = this.CreateUserID;
                productCBS.CreateUser = this.CreateUser;
                cbsRoot.AddChild(productCBS);
            }
            //删除不存在的
            var removeMajorWorkloadList = productCBS.Children.Where(d => !selectMajorList.Contains(d.Code)).ToList();
            foreach (var item in removeMajorWorkloadList)
                item.Delete();
            #endregion

            var roleDefineList = BaseConfigFO.GetRoleDefineList();
            var majorAttrList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
            if (!string.IsNullOrEmpty(this.MajorList))
            {
                var majorNodes = JsonHelper.ToList(this.MajorList);
                foreach (var item in majorNodes)
                {
                    var majorCode = item.GetValue("MajorCode");
                    var majorName = item.GetValue("MajorName");
                    var attr = majorAttrList.FirstOrDefault(d => d.Code == majorCode);
                    if (attr == null) throw new Formula.Exceptions.BusinessException("未定义专业【" + majorCode + "】" + majorName + "，操作失败");
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

                    #region 卷册
                    if (!string.IsNullOrEmpty(this.TaskWorkList))
                    {
                        var list = DetailTaskWorkList.Where(a => a.Major == major.WBSValue).ToList();
                        foreach (var detail in list)
                        {
                            var task = ProjectTaskWorkList.FirstOrDefault(a => a.ID == detail.TaskWorkID);
                            if (task == null)
                            {
                                task = new S_W_TaskWork();
                                task.State = TaskWorkState.Plan.ToString();
                            }
                            task.Name = detail.Name;
                            task.Code = detail.Code;
                            task.MajorValue = detail.Major;
                            task.PhaseValue = detail.Phase;
                            if (string.IsNullOrEmpty(task.ID))
                            {
                                task.ID = detail.TaskWorkID;
                                //根据专业同步人员
                                task.FillWBSUser(major);
                                major.AddTaskWork(task);
                            }
                            else
                                task.Save();
                            task.DossierCode = detail.DossierCode;
                            task.DossierName = detail.DossierName;
                            task.S_W_WBS.PhaseCode = task.PhaseValue;
                            //修改成果的阶段、专业、卷号
                            foreach (var product in task.S_W_WBS.S_E_Product.ToList())
                            {
                                product.PhaseValue = task.PhaseValue;
                                product.MajorValue = task.MajorValue;
                                product.MonomerCode = task.DossierCode;
                                product.MonomerInfo = task.DossierName;
                                product.PackageCode = task.Code;
                                product.PackageName = task.Name;
                            }
                            if (task.Version == null)
                                task.Version = 1;
                        }
                    }
                    #endregion

                    #region 专业工时CBS数据
                    var _mcbs = productCBS.Children.FirstOrDefault(a => a.Code == majorCode);
                    if (_mcbs == null)
                    {
                        _mcbs = new S_C_CBS();
                        _mcbs.Name = majorName;
                        _mcbs.Code = majorCode;
                        _mcbs.NodeType = CBSNodeType.Major.ToString();
                        _mcbs.CBSType = CBSType.LabourExpense.ToString();
                        _mcbs.SortIndex = 0;
                        if (!string.IsNullOrEmpty(item.GetValue("SortIndex")))
                            _mcbs.SortIndex = Convert.ToDouble(item.GetValue("SortIndex"));
                        _mcbs.CreateUserID = this.CreateUserID;
                        _mcbs.CreateUser = this.CreateUser;
                        productCBS.AddChild(_mcbs);
                    }
                    else
                    {
                        _mcbs.ModifyUserID = this.CreateUserID;
                        _mcbs.ModifyUser = this.CreateUser;
                        _mcbs.ModifyDate = DateTime.Now;
                    }
                    _mcbs.Quantity = 0;
                    if (!string.IsNullOrEmpty(item.GetValue("MajorWorkload")))
                        _mcbs.Quantity = Convert.ToDecimal(item.GetValue("MajorWorkload"));
                    _mcbs.TotalPrice = _mcbs.Quantity.Value * _mcbs.UnitPrice.Value;

                    #endregion
                }
            }

            productCBS.SummayQuantity();

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

            var detailMileStoneList = this.T_SC_ElectricalPowerProjectScheme_MileStoneList.ToList();
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
