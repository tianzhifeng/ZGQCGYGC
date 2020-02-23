using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    /// <summary>
    /// OBS节点类型
    /// </summary>
    public enum OBSNodeType
    {
        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        Project,

        /// <summary>
        /// WBS
        /// </summary>
        [Description("WBS")]
        WBS,

        /// <summary>
        /// 自定义分组
        /// </summary>
        [Description("自定义分组")]
        Group,

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        Role

    }

}
