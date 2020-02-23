using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum PayCheckDetailTemplateType
    {
        /// <summary>
        /// 一般项
        /// </summary>
        [Description("一般项")]
        Common = 1,
        /// <summary>
        /// 工程价款项
        /// </summary>
        [Description("工程价款项")]
        BOQ,
        /// <summary>
        /// 汇总项
        /// </summary>
        [Description("汇总项")]
        Summary
    }
}
