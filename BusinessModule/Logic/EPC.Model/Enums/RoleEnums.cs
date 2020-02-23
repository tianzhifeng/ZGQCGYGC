using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum ProjectRoleType
    {
        [Description("前期")]
        前期,
        [Description("设计")]
        设计,
        [Description("采购")]
        采购,
        [Description("施工")]
        施工,
        [Description("管理")]
        管理
    }

    /// <summary>
    /// 角色类型
    /// </summary>
    [Description("角色类型")]
    public enum RoleType
    {
        [Description("项目角色")]
        ProjectRoleType,
        [Description("岗位")]
        OrgRoleType,
        [Description("系统角色")]
        SysRoleType
    }

    public enum CoopertationRoleType
    {
        [Description("总包方")]
        EPC,
        [Description("客户")]
        Customer,
        [Description("设备供应商")]
        Supplier,
        [Description("施工分包商")]
        Consturction,
        [Description("监理")]
        Supervisor
    }
}
