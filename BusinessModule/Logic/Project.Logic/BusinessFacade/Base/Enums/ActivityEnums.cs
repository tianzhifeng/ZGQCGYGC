using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    [Description("活动类别")]
    public enum ActivityType
    {
        /// <summary>
        /// 设计任务（和校审流程中的设计提交做区分）
        /// </summary>
        [Description("设计任务")]
        DesignTask,

        #region 校审流程环节
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
        /// 批准
        /// </summary>
        [Description("批准")]
        Agree,
        #endregion

        /// <summary>
        /// 资料提出
        /// </summary>
        [Description("资料提出")]
        Put,

        /// <summary>
        /// 资料接收
        /// </summary>
        [Description("资料接收")]
        Receive,

        /// <summary>
        /// 会签
        /// </summary>
        [Description("会签")]
        CounterSign
    }

}
