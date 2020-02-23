using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum CapitalPlanType
    {
        /// <summary>
        /// 初始
        /// </summary>
        [Description("初始")]
        Initial = 1,
        /// <summary>
        /// 流入
        /// </summary>
        [Description("流入")]
        In,
        /// <summary>
        /// 支出
        /// </summary>
        [Description("支出")]
        Out,        
        /// <summary>
        /// 结余
        /// </summary>
        [Description("汇总")]
        Total
    }
}
