using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_T_EngineeringSpaceRes
    {

        List<S_T_EngineeringSpaceRes> _allchildren;
        /// <summary>
        /// 获取所有子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_EngineeringSpaceRes> AllChildren
        {
            get
            {
                _allchildren = this.GetDbContext<BaseConfigEntities>().S_T_EngineeringSpaceRes.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                return _allchildren;
            }
        }

        /// <summary>
        /// 权限类别(只读或完全控制)
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public string AuthType
        {
            get;
            set;
        }

        public void AddChild(S_T_EngineeringSpaceRes child)
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            if (String.IsNullOrEmpty(child.ID))
                child.ID = Formula.FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            entities.Set<S_T_EngineeringSpaceRes>().Add(child);
        }

        public void Delete()
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            entities.Set<S_T_EngineeringSpaceRes>().Delete(d => d.FullID.StartsWith(this.FullID));
        }


        /// <summary>
        /// 清除所有的关联权限配置
        /// </summary>
        /// <param name="includeChildren">清除权限是否包含下层所有的子节点</param>
        public void ClearAuth(bool includeChildren = true)
        {
            this.GetDbContext<BaseConfigEntities>().S_T_EngineeringSpaceAuth.Delete(d => d.EngineeringSpaceResID == this.ID);
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
            this.GetDbContext<BaseConfigEntities>().S_T_EngineeringSpaceAuth.Delete(d => d.EngineeringSpaceResID == this.ID && d.RoleCode == roleCode);
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
        public void SetProjectRole(string roleCode, string authType, bool includeChildren = true)
        {
            //if (this.Root == null) throw new Formula.Exceptions.BusinessException("节点未挂到到空间定义，无法设置权限");
            var auth = this.S_T_EngineeringSpaceAuth.FirstOrDefault(d => d.RoleCode == roleCode
                     && d.RoleType == RoleType.ProjectRoleType.ToString());
            if (auth == null)
            {
                auth = new S_T_EngineeringSpaceAuth();
                auth.ID = FormulaHelper.CreateGuid();
                this.S_T_EngineeringSpaceAuth.Add(auth);
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
                var parent = _entities.Set<S_T_EngineeringSpaceRes>().FirstOrDefault(d => d.ID == this.ParentID);
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
        /// <param name="authType">权限级别（默认完全控制）</param>
        /// <param name="includeChildren">子节点是否继承</param>
        public void SetSysRole(string roleCode, string authType, bool includeChildren = true)
        {
            var auth = this.S_T_EngineeringSpaceAuth.FirstOrDefault(d => d.RoleCode == roleCode
                   && d.RoleType == RoleType.SysRoleType.ToString());
            if (auth == null)
            {
                auth = new S_T_EngineeringSpaceAuth();
                auth.ID = FormulaHelper.CreateGuid();
                this.S_T_EngineeringSpaceAuth.Add(auth);
            }
            auth.RoleCode = roleCode;
            auth.RoleType = RoleType.SysRoleType.ToString();
            auth.AuthType = authType.ToString();
            if (includeChildren)
            {
                foreach (var item in this.AllChildren)
                    item.SetSysRole(roleCode, authType, false);
            }
            //上层节点自动授权 
            var _entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            if (!String.IsNullOrEmpty(this.ParentID))
            {
                var parent = _entities.Set<S_T_EngineeringSpaceRes>().FirstOrDefault(d => d.ID == this.ParentID);
                parent.SetSysRole(roleCode, authType, false);
            }
        }
    }
}
