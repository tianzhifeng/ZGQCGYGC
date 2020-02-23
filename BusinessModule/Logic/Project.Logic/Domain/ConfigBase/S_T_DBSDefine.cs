using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Formula;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    public partial class S_T_DBSDefine 
    {
        /// <summary>
        /// 根节点定义
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_DBSDefine Root
        {
            get
            {
                return this.S_T_ProjectMode.S_T_DBSDefine.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
            }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_DBSDefine Parent
        {
            get {
                return this.S_T_ProjectMode.S_T_DBSDefine.SingleOrDefault(d => d.ID == this.ParentID);
            }
        }

        List<S_T_DBSDefine> _children;
        /// <summary>
        /// 子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_DBSDefine> Children
        {
            get
            {
                if (_children == null)
                    _children = this.S_T_ProjectMode.S_T_DBSDefine.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }

        List<S_T_DBSDefine> _allchildren;
        /// <summary>
        /// 所有子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_DBSDefine> AllChildren
        {
            get
            {
                if (_allchildren == null)
                    _allchildren = this.S_T_ProjectMode.S_T_DBSDefine.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                return _allchildren;
            }
        }



        /// <summary>
        /// 增加DBS定义子节点
        /// </summary>
        /// <param name="child">DBS定义子节点</param>
        public void AddChild(S_T_DBSDefine child)
        {
            if (this.S_T_ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.DBSCode == child.DBSCode && d.ID != child.ID) != null)
                throw new Formula.Exceptions.BusinessException("已经存在编号为【" + child.DBSCode + "】的DBS节点，不能重复添加");
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.Level = child.FullID.Split('.').Length;
            if(String.IsNullOrEmpty(child.DBSType))
            child.DBSType = Project.Logic.DBSType.Folder.ToString();           
            this.S_T_ProjectMode.S_T_DBSDefine.Add(child);
            child.InhertParentAuth();
        }

        /// <summary>
        /// 根据角色来设置DBS对象的权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="authType">权限类别</param>
        public void SetAuth(string roleCode, string roleName, FolderAuthType authType, string roleType)
        {
            this.SetAuth(roleCode, roleName, authType.ToString(), roleType);
        }

        /// <summary>
        /// 根据角色来设置DBS对象的权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="authType">权限类别</param>
        public void SetAuth(string roleCode, string roleName, string authType, string roleType)
        {
            var security = this.S_T_DBSSecurity.FirstOrDefault(d => d.RoleCode == roleCode);
            if (security == null)
            {
                security = new S_T_DBSSecurity();
                security.ID = FormulaHelper.CreateGuid();
                security.RoleCode = roleCode;
                security.RoleName = roleName;
                security.RoleType = roleType;
                this.S_T_DBSSecurity.Add(security);
            }
            security.AuthType = authType;
            this.InheritAuth = false.ToString();
            foreach (var child in this.AllChildren.Where(d => d.InheritAuth == true.ToString()).ToList())
                child.InheritNodeAuth(this);
        }

        /// <summary>
        /// 重新集成上级权限
        /// </summary>
        public void InhertParentAuth()
        {
            if (this.Parent == null) return;
            this.InheritNodeAuth(this.Parent);
            foreach (var child in this.AllChildren.Where(d => d.InheritAuth == true.ToString()).ToList())
                child.InheritNodeAuth(this);
        }

        /// <summary>
        /// 集成指定目录的权限
        /// </summary>
        /// <param name="dbsDefine">指定的DBS目录</param>
        public void InheritNodeAuth(S_T_DBSDefine dbsDefine)
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            entities.S_T_DBSSecurity.Delete(d => d.DBSDefineID == this.ID);
            foreach (var security in dbsDefine.S_T_DBSSecurity.ToList())
            {
                var sec = new S_T_DBSSecurity();
                sec.ID = FormulaHelper.CreateGuid();
                sec.RoleCode = security.RoleCode;
                sec.RoleName = security.RoleName;
                sec.AuthType = security.AuthType;
                sec.RoleType = security.RoleType;
                sec.DBSDefineID = this.ID;
                this.S_T_DBSSecurity.Add(sec);
                sec.S_T_DBSDefine = this;
            }
            this.InheritAuth = true.ToString();
        }

        /// <summary>
        /// 移除DBS权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        public void RemoveAuth(string roleCode)
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            entities.S_T_DBSSecurity.Delete(d => d.DBSDefineID == this.ID && d.RoleCode == roleCode);
        }

        /// <summary>
        /// 清空目录访问权限
        /// </summary>
        public void ClearAuth()
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            entities.S_T_DBSSecurity.Delete(d => d.DBSDefineID == this.ID);
        }

        /// <summary>
        /// 获取下层子节点
        /// </summary>
        /// <returns>返回DBS定义集合</returns>
        public List<S_T_DBSDefine> GetChildren()
        {
            return this.S_T_ProjectMode.S_T_DBSDefine.Where(d => d.ParentID == this.ID).ToList();
        }

        /// <summary>
        /// 获得所有子节点
        /// </summary>
        /// <returns>返回DBS定义集合</returns>
        public List<S_T_DBSDefine> GetAllChildren()
        {
            return this.S_T_ProjectMode.S_T_DBSDefine.Where(d => d.FullID.StartsWith(this.FullID)).ToList();
        }

        /// <summary>
        /// 删除DBS定义节点
        /// </summary>
        public void Delete()
        {
            foreach (var item in this.AllChildren)
                item.DeleteSelf();
            this.DeleteSelf();
        }

        internal void DeleteSelf()
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            entities.S_T_DBSSecurity.Delete(d => d.DBSDefineID == this.ID);
            entities.S_T_DBSDefine.Delete(d => d.ID == this.ID);
        }
    }
}
