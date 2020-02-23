using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.ComponentModel.DataAnnotations;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Config;
using Formula.Helper;
using Formula.DynConditionObject;

namespace Project.Logic.Domain
{
    public partial class S_I_ProjectGroup
    {
        S_T_EngineeringSpace _space;
        [NotMapped]
        [JsonIgnore]
        public S_T_EngineeringSpace EngineeringSpace
        {
            get
            {
                if (_space == null)
                {
                    _space = BaseConfigFO.GetEngineeringSpace(this.EngineeringSpaceCode);
                }
                return _space;
            }
        }

        S_I_ProjectGroup _root;
        /// <summary>
        /// 根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_ProjectGroup Root
        {
            get
            {
                if (_root == null)
                {
                    var entities = this.GetDbContext<ProjectEntities>();
                    _root = entities.S_I_ProjectGroup.FirstOrDefault(d => d.RootID == this.ID);
                }
                return _root;
            }
        }

        List<S_I_ProjectGroup> _children;
        /// <summary>
        /// 所有子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_ProjectGroup> Children
        {
            get
            {
                if (_children == null)
                {
                    var entities = this.GetDbContext<ProjectEntities>();
                    _children = entities.S_I_ProjectGroup.Where(d => d.ParentID == this.ID).ToList();
                }
                return _children;
            }
        }

        List<S_I_ProjectGroup> _allChildren;
        /// <summary>
        /// 所有下层节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_ProjectGroup> AllChildren
        {
            get
            {
                if (_allChildren == null)
                {
                    var entities = this.GetDbContext<ProjectEntities>();
                    _allChildren = entities.S_I_ProjectGroup.Where(d => d.RootID == this.RootID && d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _allChildren;
            }
        }

        List<S_I_ProjectGroup> _allParents;
        /// <summary>
        /// 所有上层节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_ProjectGroup> AllParents
        {
            get
            {
                if (_allParents == null)
                {
                    var entities = this.GetDbContext<ProjectEntities>();
                    _allParents = entities.S_I_ProjectGroup.Where(d => d.RootID == this.RootID && this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _allParents;
            }
        }


        List<S_I_ProjectInfo> _projectInfoList;
        /// <summary>
        /// 关联管理单元对象集合
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_ProjectInfo> ProjectInfoList
        {
            get
            {
                if (_projectInfoList == null)
                {
                    var entities = this.GetDbContext<ProjectEntities>();
                    _projectInfoList = entities.S_I_ProjectInfo.Where(d => d.GroupFullID.StartsWith(this.FullID)).ToList();
                }
                return _projectInfoList;
            }
        }

        /// <summary>
        /// 新增子节点
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(S_I_ProjectGroup child)
        {
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.RootID = this.RootID;
            child.CreateDate = DateTime.Now;
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_I_ProjectGroup>(child).State == EntityState.Detached)
                entities.S_I_ProjectGroup.Add(child);
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="destory">忽略校验，删除全部关联数据</param>
        public void Delete(bool destory = false)
        {
            foreach (var item in this.AllChildren)
                item.DestorySelf(destory);
            this.DestorySelf(destory);
        }

        /// <summary>
        /// 绑定管理单元
        /// </summary>
        /// <param name="projectInfo">管理单元项目对象</param>
        public void BindingProject(S_I_ProjectInfo projectInfo)
        {
            var baseConfigEntities = this.GetDbContext<BaseConfigEntities>();
            var projectEntities = this.GetDbContext<ProjectEntities>();

            if (!String.IsNullOrEmpty(projectInfo.GroupID))
            {
                var groupInfo = projectEntities.S_I_ProjectGroup.FirstOrDefault(d => d.ID == projectInfo.GroupID);
                if (groupInfo != null)
                    throw new Formula.Exceptions.BusinessException("已经绑定了EPS的项目不能进行重复绑定");
            }
            projectInfo.EngineeringInfoID = this.RelateID;

            var type = EngineeringSpaceType.UnderEngineering.ToString();
            var epsDefines = baseConfigEntities.Set<S_D_EPSDef>().Where(d => d.Type == type).OrderBy(d => d.FullID).ToList();
            if (this.Children.Count == 0 || epsDefines.Count == 0)
            {
                var project = new S_I_ProjectGroup();
                project.ID = FormulaHelper.CreateGuid();
                project.Name = projectInfo.Name;
                project.Code = projectInfo.Code;
                project.DeptID = projectInfo.ChargeDeptID;
                project.DeptName = projectInfo.ChargeDeptName;
                project.City = this.City;
                project.ProjectClass = string.IsNullOrEmpty(this.ProjectClass) ? projectInfo.ProjectClass : this.ProjectClass;
                project.Address = this.Address;
                project.Country = this.Country;
                project.Proportion = this.Proportion;
                project.Province = this.Province;
                project.RelateID = this.RelateID;
                project.Type = EngineeringSpaceType.Project.ToString();
                project.ChargeUserName = projectInfo.ChargeUserName;
                project.ChargeUser = projectInfo.ChargeUserID;
                project.PhaseContent = projectInfo.PhaseName;
                project.PhaseValue = projectInfo.PhaseValue;
                project.State = projectInfo.State;
                this.AddChild(project);
                projectInfo.GroupID = project.ID;
                projectInfo.GroupRootID = project.RootID;
                projectInfo.GroupFullID = project.FullID;
            }
            else
            {
                var list = new List<S_I_ProjectGroup>();
                foreach (var item in epsDefines)
                {
                    #region
                    var propertyInfoValue = projectInfo.GetPropertyString(item.GroupField);
                    var eps = projectEntities.S_I_ProjectGroup.FirstOrDefault(d => d.DefineID == item.ID && d.Name == propertyInfoValue && d.FullID.StartsWith(this.FullID));
                    if (eps == null)
                    {
                        eps = projectEntities.S_I_ProjectGroup.Create();
                        eps.Type = EngineeringSpaceType.UnderEngineering.ToString();
                        eps.ID = FormulaHelper.CreateGuid();
                        eps.Name = propertyInfoValue;
                        eps.DefineID = item.ID;
                        if (item.Parent == null) throw new Formula.Exceptions.BusinessException();
                        if (item.Parent.Type == EngineeringSpaceType.Engineering.ToString())
                        {
                            eps.RelateID = this.RelateID;
                            eps.Type = EngineeringSpaceType.UnderEngineering.ToString();
                            this.AddChild(eps);
                        }
                        else
                        {
                            var parentValue = projectInfo.GetPropertyString(item.Parent.GroupField);
                            var parentEps = list.FirstOrDefault(d => d.DefineID == item.ParentID && d.Name == parentValue);
                            if (parentEps != null)
                            {
                                parentEps.AddChild(eps);
                            }
                        }
                    }
                    list.Add(eps);
                    if (item.Children.Count == 0)
                    {
                        var project = new S_I_ProjectGroup();
                        project.ID = FormulaHelper.CreateGuid();
                        project.Name = projectInfo.Name;
                        project.Code = projectInfo.Code;
                        project.DeptID = projectInfo.ChargeDeptID;
                        project.DeptName = projectInfo.ChargeDeptName;
                        project.City = this.City;
                        project.ProjectClass = this.ProjectClass;
                        project.Address = this.Address;
                        project.Country = this.Country;
                        project.Proportion = this.Proportion;
                        project.Province = this.Province;
                        project.RelateID = this.RelateID;
                        project.Type = EngineeringSpaceType.Project.ToString();
                        project.ChargeUserName = project.ChargeUserName;
                        project.ChargeUserName = projectInfo.ChargeUserName;
                        project.ChargeUser = projectInfo.ChargeUserID;
                        project.PhaseValue = projectInfo.PhaseValue;
                        project.State = projectInfo.State;
                        eps.AddChild(project);
                        projectInfo.GroupID = project.ID;
                        projectInfo.GroupRootID = project.RootID;
                        projectInfo.GroupFullID = project.FullID;
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 获取工程下的所有人员OBS列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserTable()
        {
            string sql = @"select distinct RoleCode,RoleName,UserID,UserName,DeptID,DeptName from S_W_OBSUser
left join S_I_ProjectInfo on S_W_OBSUser.ProjectInfoID = S_I_ProjectInfo.ID
where GroupFullID like '" + this.FullID + "%'";
            var Db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = Db.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 获取工程下的主要人员OBS列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMainUserTable()
        {
            string sql = @"select distinct RoleCode,RoleName,UserID,UserName,ou.DeptID,ou.DeptName,u.MobilePhone from S_W_OBSUser ou
left join S_I_ProjectInfo on ou.ProjectInfoID = S_I_ProjectInfo.ID
left join {0}..S_A_User u on ou.UserID=u.ID
where GroupFullID like '" + this.FullID + "%' and  (RoleCode='MajorPrinciple' or MajorValue='' or MajorValue is null)";
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base).DbName;
            sql = string.Format(sql, baseDB);
            var Db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = Db.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 获取工程下的主要人员OBS列表(多角色对应一行)
        /// </summary>
        /// <returns></returns>
        public DataTable GetMainUserWithRolesTable()
        {
            string sql = @"select distinct UserID,UserName,ou.DeptID,ou.DeptName,u.MobilePhone,
	RoleName=stuff((select distinct ','+RoleName 
						from S_W_OBSUser
						left join S_I_ProjectInfo on ProjectInfoID = S_I_ProjectInfo.ID
						where UserID=ou.UserID 
							and GroupFullID like '{1}%' 
							and  (RoleCode='MajorPrinciple' or MajorValue='' or MajorValue is null)
						for xml path('')) ,1,1,'') 
from S_W_OBSUser ou
left join S_I_ProjectInfo on ou.ProjectInfoID = S_I_ProjectInfo.ID
left join {0}..S_A_User u on ou.UserID=u.ID
where GroupFullID like '{1}%' 
and  (RoleCode='MajorPrinciple' or MajorValue='' or MajorValue is null)";
            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base).DbName;
            sql = string.Format(sql, baseDB, this.FullID);
            var Db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = Db.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 获取工程下指定人员的OBS信息
        /// </summary>
        /// <param name="userID">指定人员ID</param>
        /// <returns></returns>
        public DataTable GetUserOBSTable(string userID)
        {
            string sql = @"select distinct RoleCode,RoleName,UserID,UserName from S_W_OBSUser
left join S_I_ProjectInfo on S_W_OBSUser.ProjectInfoID = S_I_ProjectInfo.ID
where GroupFullID like '" + this.FullID + "%' and UserID='" + userID + "'";
            var Db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = Db.ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RoleKeys"></param>
        /// <returns></returns>
        public List<S_T_EngineeringSpaceRes> GetSpaceDefineByRoleKeys(string RoleKeys)
        {
            var result = new List<S_T_EngineeringSpaceRes>();
            var list = this.EngineeringSpace.S_T_EngineeringSpaceRes.ToList();
            foreach (var item in list)
            {
                if (!String.IsNullOrEmpty(item.AuthType) && item.AuthType == SpaceAuthType.FullControl.ToString())
                {
                    var siItem = result.FirstOrDefault(d => d.ID == item.ID);
                    if (siItem == null) { result.Add(item); siItem = item; }
                    continue;
                }
                foreach (var role in RoleKeys.Split(','))
                {
                    var spaceAuth = item.S_T_EngineeringSpaceAuth.FirstOrDefault(d => d.RoleCode == role);
                    if (spaceAuth != null)
                    {
                        var siItem = result.FirstOrDefault(d => d.ID == item.ID);
                        if (siItem == null) { result.Add(item); siItem = item; }
                        if (spaceAuth.AuthType == SpaceAuthType.FullControl.ToString())
                        {
                            siItem.AuthType = SpaceAuthType.FullControl.ToString();
                            break;
                        }
                        else
                            siItem.AuthType = SpaceAuthType.View.ToString();
                    }
                }
            }
            return result;
        }


        #region 私有方法

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="destory">是否忽略校验，进行删除</param>
        void DestorySelf(bool destory = false)
        {
            if (!destory)
            {
                if (this.ProjectInfoList.Count > 0)
                    throw new Formula.Exceptions.BusinessException("已经拥有管理单元的组合对象不能进行删除操作");
            }
            foreach (var item in this.ProjectInfoList)
                item.Delete(destory);
        }

        #endregion
    }
}
