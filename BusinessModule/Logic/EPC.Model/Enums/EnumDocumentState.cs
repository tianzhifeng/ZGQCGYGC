using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EPC.Logic
{
    public enum EnumDocumentState
    {
        [Description("未提交(可删除)")]
        Create = 0,
        [Description("审核中")]
        Review,
        [Description("通过")]
        Passed,
        [Description("驳回(可删除)")]
        Reject
    }
}