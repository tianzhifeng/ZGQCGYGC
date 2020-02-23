using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    /// <summary>
    /// 里程碑类别
    /// </summary>
    [Description("里程碑类别")]
    public enum MileStoneType
    {
        /// <summary>
        /// 项目里程碑
        /// </summary>
        [Description("项目里程碑")]
        Normal,

        /// <summary>
        /// 专业里程碑
        /// </summary>
        [Description("专业里程碑")]
        Major,

        /// <summary>
        /// 提资里程碑
        /// </summary>
        [Description("提资计划")]
        Cooperation

    }

    public enum MileStoneChangeType
    {
        [Description("正常变更")]
        Normal,
        [Description("责任变更")]
        Delay
    }


    [Description("里程碑状态")]
    public enum MileStoneState
    {
        /// <summary>
        /// 计划
        /// </summary>
        [Description("计划")]
        Plan,

        /// <summary>
        /// 完成申请中
        /// </summary>
        [Description("完成申请中")]
        Execute,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish,

        /// <summary>
        /// 未完成
        /// </summary>
        [Description("未完成")]
        UnFinish
    }

    public enum MileStoneShowStatus
    {
        /// <summary>
        /// 未策划
        /// </summary>
        [Description("未策划")]
        NotPlan,

        /// <summary>
        /// 正常完成
        /// </summary>
        [Description("正常完成")]
        NormalFinish,

        /// <summary>
        /// 正常进行
        /// </summary>
        [Description("正常进行")]
        NormalUndone,

        /// <summary>
        /// 延期完成
        /// </summary>
        [Description("延期完成")]
        DelayFinish,

        /// <summary>
        /// 延期未完成
        /// </summary>
        [Description("延期未完成")]
        DelayUndone,
    }

}
