using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum CBSBelongType
    {
        [Description("投标")]
        Tender,
        [Description("概算")]
        Estimate,
        [Description("预算")]
        Budget,
        [Description("合同")]
        Contract,
        [Description("结算")]
        Cost,
        [Description("决算")]
        Final,
        [Description("标准")]
        Standard
    }
}
