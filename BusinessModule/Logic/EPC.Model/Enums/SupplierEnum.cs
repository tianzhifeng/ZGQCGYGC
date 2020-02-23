using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum SupplierState
    {
        [Description("待准入")]
        Create,
        [Description("合格供应商")]
        Qualification,
        [Description("不合格供应商")]
        DisQualification,
        [Description("黑名单")]
        BlackList
    }
}
