using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic.Enums
{
    public enum UnitEnum
    {
        [Description("m")]
        M = 1,
        [Description("m2")]
        M2 = 2,
        [Description("m3")]
        M3 = 3,
        [Description("个")]
        Ge = 4,
        [Description("吨")]
        Dun = 5,
    }
}
