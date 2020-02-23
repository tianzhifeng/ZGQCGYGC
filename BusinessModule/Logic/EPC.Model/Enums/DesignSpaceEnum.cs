using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    /// <summary>
    /// 成果设校审状态枚举
    /// </summary>
    [Description("成果设校审状态枚举")]
    public enum AuditState
    {
        /// <summary>
        /// 未提交
        /// </summary>
        [Description("未提交")]
        Create,

        /// <summary>
        /// 设计人提交
        /// </summary>
        [Description("设计人提交")]
        Design,

        /// <summary>
        /// 校核中
        /// </summary>
        [Description("校核中")]
        Collact,

        /// <summary>
        /// 审核中
        /// </summary>
        [Description("审核中")]
        Audit,

        /// <summary>
        /// 审定中
        /// </summary>
        [Description("审定中")]
        Approve,
        /// <summary>
        /// 批准中
        /// </summary>
        [Description("批准中")]
        Agree,
        /// <summary>
        /// 校审完成
        /// </summary>
        [Description("校审完成")]
        Pass
    }

    /// <summary>
    /// 校审意见状态
    /// </summary>
    [Description("校审意见状态")]
    public enum MistakeState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        Execute,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Finish,

        /// <summary>
        /// 无问题
        /// </summary>
        [Description("无问题")]
        Reject
    }

    /// <summary>
    /// 设计流程环节定义（需对应ActivityType中的枚举）
    /// </summary>
    public enum AuditType
    {
        /// <summary>
        /// 设计
        /// </summary>
        [Description("设计提交")]
        Design,

        /// <summary>
        /// 校核
        /// </summary>
        [Description("校核")]
        Collact,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        Audit,

        /// <summary>
        /// 审定
        /// </summary>
        [Description("审定")]
        Approve,
        /// <summary>
        /// 项目经理
        /// </summary>
        [Description("项目经理")]
        ProjectManager,
        /// <summary>
        /// 设总
        /// </summary>
        [Description("设总")]
        DesignManager,
        /// <summary>
        /// 制图人
        /// </summary>
        [Description("制图人")]
        Mapper,
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
        /// 部门负责人
        /// </summary>
        [Description("部门负责人")]
        DeptManager,
        /// <summary>
        /// 批准
        /// </summary>
        [Description("批准")]
        Agree,
        /// <summary>
        /// 会签
        /// </summary>
        [Description("会签")]
        CounterSign

    }

    /// <summary>
    /// 会签状态
    /// </summary>
    public enum CoSignState
    {
        /// <summary>
        /// 未会签
        /// </summary>
        [Description("未会签")]
        NoSign,
        /// <summary>
        /// 会签中
        /// </summary>
        [Description("会签中")]
        Sign,
        /// <summary>
        /// 会签完成
        /// </summary>
        [Description("会签完成")]
        SignComplete

    }
}
