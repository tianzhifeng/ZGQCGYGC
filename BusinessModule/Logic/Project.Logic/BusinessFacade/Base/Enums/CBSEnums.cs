using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    [Description("CBS节点类型")]
    public enum CBSNodeType
    {
        [Description("根节点")]
        Root,
        [Description("分类节点")]
        Category,
        [Description("费用节点")]
        CBS,
        [Description("专业节点")]
        Major,
        [Description("其它")]
        Other
    }
    
    [Description("CBS工时节点分类")]
    public enum CBSCategoryType
    {
        [Description("预留工时")]
        Reserve,
        [Description("管理工时")]
        Manage,
        [Description("设计工时")]
        Product
    }

    /// <summary>
    /// CBS分类
    /// </summary>
    [Description("CBS分类")]
    public enum CBSType
    {
        /// <summary>
        /// 直接成本
        /// </summary>
        [Description("直接成本")]
        DirectExpense,


        /// <summary>
        /// 人工成本
        /// </summary>
        [Description("人工成本")]
        LabourExpense,


        /// <summary>
        /// 其它成本
        /// </summary>
        [Description("其它成本")]
        OtherExpense

    }
}
