using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    /// <summary>
    /// 项目管理通用状态
    /// </summary>
    [Description("项目状态")]
    public enum ProjectCommoneState
    {
        /// <summary>
        /// 未下达
        /// </summary>
        [Description("未下达")]
        Create,

        /// <summary>
        /// 策划中
        /// </summary>
        [Description("策划中")]
        Plan,

        /// <summary>
        /// 设计中
        /// </summary>
        [Description("设计中")]
        Execute,

        /// <summary>
        /// 已完工
        /// </summary>
        [Description("已完工")]
        Finish,

        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("已暂停")]
        Pause,

        /// <summary>
        /// 已终止
        /// </summary>
        [Description("已终止")]
        Terminate
    }

    /// <summary>
    /// 项目管理流程状态
    /// </summary>
    [Description("项目管理流程状态")]
    public enum ProjectFlowState
    {
        /// <summary>
        /// 执行
        /// </summary>
        [Description("审批中")]
        InFlow,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish,

        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回")]
        Reject
    }

    public enum TrueOrFalse
    {
        [Description("是")]
        True,
        [Description("否")]
        False
    }

    /// <summary>
    /// ISO单数量类型
    /// </summary>
    [Description("ISO单数量类型")]
    public enum ISONumberType
    {
        /// <summary>
        /// 单张
        /// </summary>
        [Description("单张")]
        Single,
        /// <summary>
        /// 多张
        /// </summary>
        [Description("多张")]
        Multi,

    }

    /// <summary>
    /// 卷册工时状态
    /// </summary>
    [Description("卷册工时状态")]
    public enum TaskWorkState
    {
        /// <summary>
        /// 未策划
        /// </summary>
        [Description("未策划")]
        Create,
        /// <summary>
        /// 策划中
        /// </summary>
        [Description("策划中")]
        Plan,
        /// <summary>
        /// 下达中
        /// </summary>
        [Description("下达中")]
        Publish,
        /// <summary>
        /// 变更中
        /// </summary>
        [Description("变更中")]
        Change,
        /// <summary>
        /// 设计中
        /// </summary>
        [Description("设计中")]
        Execute,
        /// <summary>
        /// 已完工
        /// </summary>
        [Description("已完工")]
        Finish
    }


    /// <summary>
    /// 卷册变更状态
    /// </summary>
    [Description("卷册变更状态")]
    public enum TaskWorkChangeState
    {
        /// <summary>
        ///变更申请中
        /// </summary>
        [Description("变更申请中")]
        ApplyStart,
        /// <summary>
        /// 变更申请结束
        /// </summary>
        [Description("变更申请结束")]
        ApplyFinish,
        /// <summary>
        /// 变更通知首环节
        /// </summary>
        [Description("变更通知首环节")]
        AuditStart,
        /// <summary>
        /// 变更通知其他环节
        /// </summary>
        [Description("变更通知其他环节")]
        AuditApprove,
        /// <summary>
        /// 变更通知结束
        /// </summary>
        [Description("变更通知结束")]
        AuditFinish,

    }

    /// <summary>
    /// 项目消息类别
    /// </summary>
    public enum ProjectNoticeType
    {
        /// <summary>
        ///项目
        /// </summary>
        [Description("项目")]
        Project,
        /// <summary>
        ///专业
        /// </summary>
        [Description("专业")]
        Major,
        /// <summary>
        ///个人
        /// </summary>
        [Description("个人")]
        User
    }

    /// <summary>
    /// 共享区文件来源
    /// </summary>
    public enum ShareInfoSourceType
    {
        /// <summary>
        /// 手动添加
        /// </summary>
        [Description("手动添加")]
        ShareAdd,
        /// <summary>
        /// 表单添加
        /// </summary>
        [Description("表单添加")]
        FormAdd,
        /// <summary>
        ///表单选择共享文件
        /// </summary>
        [Description("表单选择共享文件")]
        FormShareSelect,
        /// <summary>
        ///表单选择成果文件
        /// </summary>
        [Description("表单选择成果文件")]
        FormProductSelect
    }
}
