using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum ProjectState
    {
        [Description("待立项")]
        Create,
        [Description("投标中")]
        Bid,
        [Description("策划中")]
        Plan,
        [Description("执行中")]
        Execute,
        [Description("已完工")]
        Finish,
        [Description("已暂停")]
        Pause,
        [Description("已终止")]
        Terminate
    }

    public enum ProjectModeType
    {
        EPC,
        EP,
        PC,
        E,
        P,
        C
    }
}
