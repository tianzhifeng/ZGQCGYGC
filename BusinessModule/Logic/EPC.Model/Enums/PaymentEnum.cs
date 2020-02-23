using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    /// <summary>
    ///付款计划状态
    /// </summary>
    [Description("付款计划状态")]
    public enum PlanPaymentState
    {
        /// <summary>
        /// 部分付款
        /// </summary>
        [Description("部分付款")]
        PartPaymented,

        /// <summary>
        /// 未完成
        /// </summary>
        [Description("未完成")]
        UnFinished,

        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        UnPayment,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paymented
    }

    /// <summary>
    ///付款状态
    /// </summary>
    [Description("付款状态")]
    public enum PaymentState
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal
    }
}
