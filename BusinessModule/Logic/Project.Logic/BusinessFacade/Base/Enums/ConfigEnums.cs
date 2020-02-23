using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    /// <summary>
    /// 
    /// </summary>
    [Description("项目角色")]
    public enum ProjectRole
    {
        /// <summary>
        /// 设计人
        /// </summary>
        [Description("设计人")]
        Designer,

        /// <summary>
        /// 校核人
        /// </summary>
        [Description("校核人")]
        Collactor,

        /// <summary>
        /// 审核人
        /// </summary>
        [Description("审核人")]
        Auditor,

        /// <summary>
        /// 审定人
        /// </summary>
        [Description("审定人")]
        Approver,

        /// <summary>
        /// 专业负责人
        /// </summary>
        [Description("专业负责人")]
        MajorPrinciple,

        /// <summary>
        /// 专业总工程师
        /// </summary>
        [Description("专业总工程师")]
        MajorEngineer,

        /// <summary>
        /// 项目负责人
        /// </summary>
        [Description("项目负责人")]
        ProjectManager,

        /// <summary>
        /// 制图人
        /// </summary>
        [Description("制图人")]
        Mapper,

        /// <summary>
        /// 项目副经理
        /// </summary>
        [Description("设总")]
        DesignManager,

        /// <summary>
        /// 协作云项目经理
        /// </summary>
        [Description("协作云项目经理")]
        CloudManager,

        /// <summary>
        /// 协作云用户
        /// </summary>
        [Description("协作云用户")]
        CloudUser
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

    /// <summary>
    /// 空间定义类别
    /// </summary>
    [Description("空间定义类别")]
    public enum SpaceDefineType
    {
        /// <summary>
        /// 静态值
        /// </summary>
        [Description("静态值")]
        Static,

        /// <summary>
        /// 动态值
        /// </summary>
        [Description("动态值")]
        Dynamic
    }

    [Description("空间节点类别")]
    public enum SpaceNodeType
    {
        /// <summary>
        /// 根节点
        /// </summary>
        [Description("根节点")]
        Root,

        /// <summary>
        /// 分组节点
        /// </summary>
        [Description("分组节点")]
        Catalog,

        /// <summary>
        /// 功能节点
        /// </summary>
        [Description("功能节点")]
        Feature
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

    /// <summary>
    /// WBS节点类型
    /// </summary>
    [Description("WBS节点类型")]
    public enum WBSNodeType
    {
        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        Project,

        /// <summary>
        /// 子项
        /// </summary>
        [Description("子项")]
        SubProject,

        /// <summary>
        /// 装置
        /// </summary>
        [Description("装置")]
        Device,

        /// <summary>
        /// 区域
        /// </summary>
        [Description("区域")]
        Area,

        /// <summary>
        /// 单体/卷
        /// </summary>
        [Description("单体/卷")]
        Entity,

        /// <summary>
        /// 阶段
        /// </summary>
        [Description("阶段")]
        Phase,

        /// <summary>
        /// 专业
        /// </summary>
        [Description("专业")]
        Major,

        /// <summary>
        /// 工作包/卷册/册
        /// </summary>
        [Description("工作包/卷册/册")]
        Work,
        /// <summary>
        /// 提资包
        /// </summary>
        [Description("提资包")]
        CooperationPackage,
        /// <summary>
        /// 工作类型
        /// </summary>
        [Description("工作类型")]
        PackageType,

        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other
    }

    /// <summary>
    /// DBS节点类型
    /// </summary>
    [Description("DBS节点类型")]
    public enum DBSType
    {
        /// <summary>
        /// 根目录
        /// </summary>
        [Description("根目录")]
        Root,

        /// <summary>
        /// 子目录
        /// </summary>
        [Description("子目录")]
        Folder,

        /// <summary>
        /// 映射目录
        /// </summary>
        [Description("映射目录")]
        Mapping,

        /// <summary>
        /// 映射目录
        /// </summary>
        [Description("OEM映射目录")]
        OEMMapping,
        
        /// <summary>
        /// 外部协作目录
        /// </summary>
        [Description("外部协作目录")]
        Cloud
    }

    /// <summary>
    /// DBS映射类型
    /// </summary>
    [Description("DBS映射类型")]
    public enum DBSMappingType
    {
        /// <summary>
        /// 成果资料
        /// </summary>
        [Description("成果资料")]
        Product,

        ///// <summary>
        ///// 互提资料
        ///// </summary>
        //[Description("互提资料")]
        //Cooperation,

        /// <summary>
        /// ISO表单
        /// </summary>
        [Description("ISO表单")]
        ISO,

        /// <summary>
        /// 设计输入资料
        /// </summary>
        [Description("设计输入资料")]
        DesignInput
    }

    /// <summary>
    /// 目录权限类别
    /// </summary>
    public enum FolderAuthType
    {
        /// <summary>
        /// 完全控制
        /// </summary>
        [Description("完全控制")]
        FullControl = 0,

        /// <summary>
        /// 读写权限
        /// </summary>
        [Description("读写权限")]
        ReadAndWrite,

        /// <summary>
        /// 只读权限
        /// </summary>
        [Description("只读权限")]
        ReadOnly,

        /// <summary>
        /// 无权限
        /// </summary>
        [Description("无权限")]
        None
    }

    /// <summary>
    /// 项目模式
    /// </summary>
    public enum ProjectMode
    {
        [Description("标准简化版")]
        FirstMode,
        [Description("标准管理模式")]
        Standard,
        [Description("综合表单策划")]
        SecondMode,
        [Description("分级策划表单")]
        ThreeMode,
        [Description("电力院单阶段管理模式")]
        ElectricPower
    }

    /// <summary>
    /// 工程层级
    /// </summary>
    public enum EngineeringLevel
    {
        [Description("工程")]
        Engineering,
        [Description("业务")]
        Business,
        [Description("子项")]
        SubProject,
        [Description("阶段")]
        TaskNotice
    }

    public enum FlowTraceDefineNodeType
    {
        [Description("分类")]
        Catagory,
        [Description("流程")]
        FlowNode
    }

    public enum QBSNodeType
    {
        [Description("分类")]
        Catagory,
        [Description("质量节点")]
        QBS
    }

    public enum QBSType
    {
        /// <summary>
        /// 定性
        /// </summary>
        [Description("定性")]
        Qualitative,
        /// <summary>
        /// 定量
        /// </summary>
        [Description("定量")]
        Quantify
    }

    [Description("CAD工作区树权限")]
    public enum CADAreaAuthType
    {
        /// <summary>
        /// 个人
        /// </summary>
        [Description("个人")]
        User,

        /// <summary>
        /// 专业
        /// </summary>
        [Description("专业")]
        Major,

        /// <summary>
        /// 项目
        /// </summary>
        [Description("项目")]
        Project
    }
}
