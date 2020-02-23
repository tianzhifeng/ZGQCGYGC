using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum ParticipationType
    {

        [Description("业主")]
        Customer = 0,
        [Description("监理")]
        Supervisor,
        [Description("供应商")]
        EquipmentSupplier,
        [Description("分包方")]
        Subcontractor,
        [Description("总包")]
        General
    }
}
