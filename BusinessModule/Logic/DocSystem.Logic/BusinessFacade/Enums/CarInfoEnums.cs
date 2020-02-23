using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DocSystem.Logic
{
    [Description("类型")]
    public enum ItemType
    {
        [Description("下载")]
        DownLoad,
        [Description("借阅")]
        Borrow
    }

    [Description("状态")]
    public enum ItemState
    {
        [Description("新建")]
        New,
        [Description("审核中")]
        Audit,
        [Description("审核通过")]
        Finish
    }

    [Description("借阅状态")]
    public enum BorrowReturnState
    {
        [Description("已借出")]
        Borrow,
        [Description("已归还")]
        Return
    }
}
