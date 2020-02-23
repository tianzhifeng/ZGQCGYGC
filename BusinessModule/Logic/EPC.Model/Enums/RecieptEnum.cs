using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    /// <summary>
    /// 收款计划状态
    /// </summary>
    [Description("收款计划状态")]
    public enum PlanReceiptState
    {

        /// <summary>
        /// 未到款
        /// </summary>
        [Description("部分到款")]
        PartReceipted,

        /// <summary>
        /// 未完成
        /// </summary>
        [Description("未完成")]
        UnFinished,

        /// <summary>
        /// 未到款
        /// </summary>
        [Description("未到款")]
        UnReceipt,

        /// <summary>
        /// 坏账
        /// </summary>
        [Description("坏账")]
        BadDebt,

        /// <summary>
        /// 已到款
        /// </summary>
        [Description("已到款")]
        Receipted
    }
}
