using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{

    /// <summary>
    /// 空间定义类别
    /// </summary>
    [Description("空间定义类别")]
    public enum SpaceDefineType
    {
        /// <summary>
        /// 静态值
        /// </summary>
        [Description("静态")]
        Static,

        /// <summary>
        /// 动态值
        /// </summary>
        [Description("动态")]
        Dynamic
    }

    public enum SpaceDataSourceType
    {
        [Description("表字段")]
        Table,
        [Description("SQL数据源")]
        SQL
    }

    [Description("空间节点类别")]
    public enum SpaceNodeType
    {
        /// <summary>
        /// 根节点
        /// </summary>
        [Description("空间")]
        Root,

        /// <summary>
        /// 根节点
        /// </summary>
        [Description("空间")]
        Space,

        /// <summary>
        /// 分组节点
        /// </summary>
        [Description("菜单")]
        Meun
    }

    /// <summary>
    /// 空间授权权限类别
    /// </summary>
    [Description("空间授权权限类别")]
    public enum SpaceAuthType
    {
        /// <summary>
        /// 完全控制
        /// </summary>
        [Description("完全控制")]
        FullControl,

        /// <summary>
        /// 当前完全控制
        /// </summary>
        [Description("当前完全控制")]
        CurrentFullControl,

        /// <summary>
        /// 只读
        /// </summary>
        [Description("只读")]
        View
    }


}
