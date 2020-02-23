using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Comprehensive.Logic
{
    public enum PhysicalState
    {
        [Description("在库")]
        在库,
        [Description("使用中")]
        使用中,
        [Description("已报废")]
        已报废
    }
}
