using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum EnumOperaType
    {
        [Description("增加")]
        None = 0,
        [Description("增加")]
        Add,
        [Description("修改")]
        Update,
        [Description("删除")]
        Delete
    }
}
