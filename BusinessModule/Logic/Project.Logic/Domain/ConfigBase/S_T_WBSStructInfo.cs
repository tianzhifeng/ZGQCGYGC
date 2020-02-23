using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Formula.Helper;
using Formula;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    public partial class S_T_WBSStructInfo 
    {
       [NotMapped]
       [JsonIgnore]
        public bool IsDefineNode
        {
            get
            {
                if (this.GetDbContext<BaseConfigEntities>().S_D_WBSAttrDefine.Count(d => d.Type == this.Code) > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 获取可增加的下层子节点类型
        /// </summary>
        /// <returns>枚举DataTable</returns>
        public DataTable GetChildWBSType()
        {
            var result = new DataTable();
            result.Columns.Add("text");
            result.Columns.Add("value");
            var children = this.ChildCode.Split(',');
            foreach (var item in children)
            {
                var row = result.NewRow();
                row["value"] = item;
                row["text"] = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), item);
                result.Rows.Add(row);
            }
            return result;
        }

        /// <summary>
        /// 增加角色定义
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        public void AddRoleInfo(string roleCode)
        {
            var roleDefine = this.GetDbContext<BaseConfigEntities>().S_D_RoleDefine.FirstOrDefault(d => d.RoleCode == roleCode);
            if (roleDefine == null) throw new Formula.Exceptions.BusinessException("未能找到编号为【"+roleCode+"】的角色定义，无法关联WBS结构");
            this.AddRoleInfo(roleDefine);
        }

        /// <summary>
        /// 增加角色定义
        /// </summary>
        /// <param name="roleDefine">角色定义对象</param>
        public void AddRoleInfo(S_D_RoleDefine roleDefine)
        {
            if (this.S_T_WBSStructRole.FirstOrDefault(d => d.RoleCode == roleDefine.RoleCode) != null) return;
            var roleInfo = new S_T_WBSStructRole();
            roleInfo.ID = FormulaHelper.CreateGuid();
            roleInfo.RoleCode = roleDefine.RoleCode;
            roleInfo.RoleName = roleDefine.RoleName;
            roleInfo.RoleRelation = roleDefine.RoleRelation;
            roleInfo.RoleType = roleDefine.RoleType;
            roleInfo.SychProject = false.ToString();
            roleInfo.SychWBS = false.ToString();
            roleInfo.Multi = true.ToString();
            roleInfo.SortIndex = this.S_T_WBSStructRole.Count + 1;
            this.S_T_WBSStructRole.Add(roleInfo);
        }

        /// <summary>
        /// 验证WBS结构下是否可增加子节点
        /// </summary>
        /// <param name="childWBSType">子节点类型</param>
        /// <returns>boolean值</returns>
        public bool ValidateChildren(WBSNodeType childWBSType)
        {
            var childCode = this.ChildCode.Split(',').FirstOrDefault(s => s == childWBSType.ToString());
            if (String.IsNullOrEmpty(childCode))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 验证WBS结构下是否可增加子节点
        /// </summary>
        /// <param name="childWBSType">子节点类型</param>
        /// <returns>boolean值</returns>
        public bool ValidateChildren(string childWBSType)
        {
            var childCode = this.ChildCode.Split(',').FirstOrDefault(s => s == childWBSType);
            if (String.IsNullOrEmpty(childCode))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 验证WBS结构下是否可增加角色
        /// </summary>
        /// <param name="roleDefine">岗位角色定义</param>
        /// <returns>boolean值</returns>
        public bool ValidateRoleDefine(S_D_RoleDefine roleDefine)
        {
            if (this.S_T_WBSStructRole.FirstOrDefault(d => d.WBSStructID == this.ID &&
                d.RoleCode == roleDefine.RoleCode) == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 验证WBS结构下是否可增加角色
        /// </summary>
        /// <param name="roleDefineCode">岗位角色定义编号</param>
        /// <returns>boolean值</returns>
        public bool ValidateRoleDefine(string roleDefineCode)
        {
            if (this.S_T_WBSStructRole.FirstOrDefault(d => d.WBSStructID == this.ID &&
                d.RoleCode == roleDefineCode) == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 删除WBS结构定义
        /// </summary>
        public void Delete()
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            context.S_T_WBSStructRole.Delete(d => d.WBSStructID == this.ID);
            context.S_T_WBSStructInfo.Delete(d => d.ID == this.ID);
        }

        /// <summary>
        /// 清除所有的关联角色定义
        /// </summary>
        public void ClearRoleDefine()
        {
            this.GetDbContext<BaseConfigEntities>().S_T_WBSStructRole.Delete(d => d.WBSStructID == this.ID);
        }

        /// <summary>
        /// 删除指定角色定义
        /// </summary>
        /// <param name="roleCode">角色定义编号</param>
        public void DeleteRoleDefine(string roleCode)
        {
            this.GetDbContext<BaseConfigEntities>().S_T_WBSStructRole.Delete(d => d.WBSStructID == this.ID && d.RoleCode == roleCode);
        }
    }
}
