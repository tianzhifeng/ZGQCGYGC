using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Project.Logic
{
    [Description("成果出图状态")]
    public enum PrintState
    {
        UnPrint,
        Printing,
        Printed
    }

    [Description("成果状态")]
    public enum ProductState
    {
        [Description("未变更")]
        Create,
        [Description("变更中")]
        Change,
        [Description("作废中")]
        InInvalid,
        [Description("作废")]
        Invalid
    }

}
