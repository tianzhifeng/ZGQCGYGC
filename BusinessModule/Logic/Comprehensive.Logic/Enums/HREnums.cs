using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Comprehensive.Logic
{
    [Description("员工状态")]
    public enum EmployeeState
    {
        [Description("在职")]
        Incumbency,
        [Description("离职")]
        Dimission,
        [Description("退休")]
        Retire,
        [Description("返聘")]
        ReEmploy,
        [Description("返聘离职")]
        ReEmployDimission,
    }
}
