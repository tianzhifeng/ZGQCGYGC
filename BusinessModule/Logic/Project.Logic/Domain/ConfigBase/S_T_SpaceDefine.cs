using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    public partial class S_T_SpaceDefine
    {
        #region 公开属性
        /// <summary>
        /// 根节点定义
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_SpaceDefine Root
        {
            get
            {
                if (String.IsNullOrEmpty(this.ModeID)) throw new Formula.Exceptions.BusinessException("未指定对象的ModeID无法获取管理模式对象，获取SpaceDefine根节点失败");
                if (this.S_T_ProjectMode == null)
                    this.S_T_ProjectMode = this.GetDbContext<BaseConfigEntities>().S_T_ProjectMode.FirstOrDefault(d => d.ID == this.ModeID);
                if (this.S_T_ProjectMode == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【】的管理模式对象，获取SpaceDefine根节点失败");
                return this.S_T_ProjectMode.S_T_SpaceDefine.FirstOrDefault(d => this.FullID.StartsWith(d.FullID) && String.IsNullOrEmpty(d.ParentID));
            }
        }

        List<S_T_SpaceDefine> _children;
        /// <summary>
        /// 获得下层子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_SpaceDefine> Children
        {
            get
            {
                //if (_children == null)
                    _children = this.GetDbContext<BaseConfigEntities>().S_T_SpaceDefine.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }


        List<S_T_SpaceDefine> _allchildren;
        /// <summary>
        /// 获取所有子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_SpaceDefine> AllChildren
        {
            get
            {
                //if (_allchildren == null)
                    _allchildren = this.GetDbContext<BaseConfigEntities>().S_T_SpaceDefine.Where(d => d.FullID.StartsWith(this.FullID)).ToList();
                return _allchildren;
            }
        }

        /// <summary>
        /// 权限类别(只读或完全控制)
        /// </summary>
        [NotMapped]
        public string AuthType
        {
            get;
            set;
        }

        #endregion

        #region 公开的实例方法

        /// <summary>
        /// 增加子编目节点
        /// </summary>
        /// <param name="child">子节点对象</param>
        public void AddChild(S_T_SpaceDefine child)
        {
            child.ValidateEntity();
            if (String.IsNullOrEmpty(child.ID)) child.ID = FormulaHelper.CreateGuid();
            child.Type = SpaceNodeType.Catalog.ToString();
            child.FullID = this.FullID + "." + child.ID;
            child.ModeID = this.ModeID;
            if (this.S_T_ProjectMode == null) throw new Formula.Exceptions.BusinessException("没有找到指定的管理模型对象无法新增SpaceDefine子节点");
            this.S_T_ProjectMode.S_T_SpaceDefine.Add(child);
        }

        /// <summary>
        /// 增加功能节点
        /// </summary>
        /// <param name="feature">功能注册对象</param>
        public void AddFeature(S_D_Feature feature)
        {
            if (this.Children.Exists(d => d.Type == SpaceNodeType.Feature.ToString() && d.Code == feature.Code))
                throw new Formula.Exceptions.BusinessException("已经存在的功能【" + feature.Name + "】不能重复增加");
            var child = new S_T_SpaceDefine();
            if (String.IsNullOrEmpty(child.ID)) child.ID = FormulaHelper.CreateGuid();
            child.Type = SpaceNodeType.Feature.ToString();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.ModeID = this.ModeID;
            child.Name = feature.Name;
            child.Code = feature.Code;
            child.LinkUrl = feature.LinkUrl;
            child.FeatureID = feature.ID;
            child.RelateWBSAttrCode = feature.RelateWBSAttrCode;
            child.DefineType = SpaceDefineType.Static.ToString();
            if (this.S_T_ProjectMode == null) throw new Formula.Exceptions.BusinessException("没有找到指定的管理模型对象无法新增Feature对象");
            this.S_T_ProjectMode.S_T_SpaceDefine.Add(child);
        }

        /// <summary>
        /// 删除空间定义
        /// </summary>
        public void Delete()
        {
            var allChildren = this.AllChildren;
            foreach (var item in allChildren)
                item.ClearAuth();
            this.GetDbContext<BaseConfigEntities>().S_T_SpaceDefine.Delete(d => d.FullID.StartsWith(this.FullID));
        }

        /// <summary>
        /// 清除所有的关联权限配置
        /// </summary>
        /// <param name="includeChildren">清除权限是否包含下层所有的子节点</param>
        public void ClearAuth(bool includeChildren = true)
        {
            this.GetDbContext<BaseConfigEntities>().S_T_SpaceAuth.Delete(d => d.SpaceID == this.ID);
            if (includeChildren)
            {
                foreach (var item in this.AllChildren)
                    item.ClearAuth(false);
            }
        }

        /// <summary>
        /// 清除所有指定角色的关联权限配置
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="includeChildren">清除权限是否包含下层所有的子节点</param>
        public void ClearAuth(string roleCode, bool includeChildren = true)
        {
            this.GetDbContext<BaseConfigEntities>().S_T_SpaceAuth.Delete(d => d.SpaceID == this.ID && d.RoleCode == roleCode);
            if (includeChildren)
            {
                foreach (var item in this.AllChildren)
                    item.ClearAuth(roleCode, false);
            }
        }

        /// <summary>
        /// 为空间设置项目角色的权限
        /// </summary>
        /// <param name="roleCode">项目角色编码</param>
        /// <param name="authType">权限级别（默认完全控制）</param>
        /// <param name="dynValue">动态值类别（默认为全部）</param>
        /// <param name="includeChildren">子节点是否继承</param>
        public void SetProjectRole(string roleCode, string authType,bool includeChildren = true)
        {
            if (this.Root == null) throw new Formula.Exceptions.BusinessException("节点未挂到到空间定义，无法设置权限");
            var auth = this.S_T_SpaceAuth.FirstOrDefault(d => d.RoleCode == roleCode
                     && d.RoleType == RoleType.ProjectRoleType.ToString());
            if (auth == null)
            {
                auth = new S_T_SpaceAuth();
                auth.ID = FormulaHelper.CreateGuid();
                this.S_T_SpaceAuth.Add(auth);
            }
            auth.RoleCode = roleCode;
            auth.RoleType = RoleType.ProjectRoleType.ToString();
            auth.AuthType = authType.ToString();
            if (includeChildren)
            {
                foreach (var item in this.AllChildren)
                    item.SetProjectRole(roleCode, authType, false);
            }
            //上层节点自动授权 
            var _entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            if (!String.IsNullOrEmpty(this.ParentID))
            {
                var parent = _entities.Set<S_T_SpaceDefine>().FirstOrDefault(d => d.ID == this.ParentID);
                parent.SetProjectRole(roleCode, authType, false);
            }
        }

        /// <summary>
        /// 为空间设置项目角色的权限
        /// </summary>
        /// <param name="roleDefine">项目角色定义对象</param>
        /// <param name="authType">权限级别（默认完全控制）</param>
        /// <param name="dynValue">动态值类别（默认为全部）</param>
        /// <param name="includeChildren">子节点是否继承</param>
        public void SetProjectRole(S_D_RoleDefine roleDefine, string authType, bool includeChildren = true)
        {
            this.SetProjectRole(roleDefine.RoleCode, authType, includeChildren);
        }

        /// <summary>
        /// 为空间设置系统角色的权限
        /// </summary>
        /// <param name="roleCode">系统角色编码</param>
        /// <param name="roleType">角色类别：组织角色、系统角色</param>
        /// <param name="authType">权限级别（默认完全控制）</param>
        /// <param name="includeChildren">子节点是否继承</param>
        public void SetSysRole(string roleCode,string roleType, string authType, bool includeChildren = true)
        {
            if (this.Root == null) throw new Formula.Exceptions.BusinessException("节点未挂到到空间定义，无法设置权限");
            var auth = this.S_T_SpaceAuth.FirstOrDefault(d => d.RoleCode == roleCode
                   && d.RoleType == roleType);
            if (auth == null)
            {
                auth = new S_T_SpaceAuth();
                auth.ID = FormulaHelper.CreateGuid();
                this.S_T_SpaceAuth.Add(auth);
            }
            auth.RoleCode = roleCode;
            auth.RoleType = roleType;// RoleType.SysRoleType.ToString();
            auth.AuthType = authType.ToString();
            if (includeChildren)
            {
                foreach (var item in this.AllChildren)
                    item.SetSysRole(roleCode, roleType, authType, false);
            }
            //上层节点自动授权 
            var _entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            if (!String.IsNullOrEmpty(this.ParentID))
            {
                var parent = _entities.Set<S_T_SpaceDefine>().FirstOrDefault(d => d.ID == this.ParentID);
                parent.SetSysRole(roleCode, roleType, authType, false);
            }
        }

        /// <summary>
        /// 验证是否拥有权限
        /// </summary>
        /// <param name="roleCode">角色名称</param>
        /// <param name="roleType">角色类别</param>
        /// <returns>是否拥有权限</returns>
        public bool ValidateRole(string roleCode,string roleType)
        {
            var result = this.S_T_SpaceAuth.Where(d => d.RoleCode == roleCode && d.RoleType == roleType).Count() == 0 ? false : true;
            return result;
        }

        #endregion

        #region 公开的静态方法

        /// <summary>
        /// 增加根节点
        /// </summary>
        /// <param name="root">根节点对象</param>
        public static void AddRoot(S_T_SpaceDefine root)
        {
            root.ValidateEntity();
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var projectMode = entities.S_T_ProjectMode.FirstOrDefault(d => d.ID == root.ModeID);
            if (projectMode == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【】的管理模式对象，无法增加SpaceDefine根");
            var spaceDefine = projectMode.S_T_SpaceDefine.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID) && d.Code == root.Code && d.ID != root.ID);
            if (spaceDefine != null) throw new Formula.Exceptions.BusinessException("【" + projectMode.Name + "】的管理对象已经拥有编号为【" + root.Code + "】的SpaceDefine根，不能重复增加");
            if (String.IsNullOrEmpty(root.ID))
                root.ID = FormulaHelper.CreateGuid();
            root.FullID = root.ID;
            root.Type = "Root";
            projectMode.S_T_SpaceDefine.Add(root);
        }

        #endregion

        #region 私有实例方法

        /// <summary>
        /// 校验对象属性的合法性
        /// </summary>
        private void ValidateEntity()
        {
            if (String.IsNullOrEmpty(this.ModeID)) throw new Formula.Exceptions.BusinessException("必须为空间定义根节点指定ModeID");
            if (String.IsNullOrEmpty(this.Name)) throw new Formula.Exceptions.BusinessException("必须为空间定义根节点指定名称");
            if (String.IsNullOrEmpty(this.DefineType)) throw new Formula.Exceptions.BusinessException("必须为空间定义根节点指定类型");
            if (String.IsNullOrEmpty(this.Code)) throw new Formula.Exceptions.BusinessException("必须为空间定义根节点指定编码");
        }

        #endregion
    }
}
