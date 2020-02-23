using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public class WBSConst
    {
        public const string phaseNodeType = "Phase";
        public const string taskNodeType = "Task";
    }

    public enum NodeClass
    {
        [Description("WBS")]
        WBS,
        [Description("PBS")]
        PBS,
        [Description("QBS")]
        QBS
    }

    public enum ResourceType
    {
        UserRole,
        BOM,
        BOQ,
        Quantity,
        Document
    }

    public enum TaskType
    {
        [Description("一般作业")]
        NormalTask,
        [Description("里程碑作业")]
        MileStone,
        [Description("提资作业")]
        CooperationTask,
        [Description("设计作业")]
        DesignTask,
        [Description("采购作业")]
        ProcurementTask,
        [Description("施工作业")]
        WorkTask,
        [Description("调试作业")]
        DebuggerTask,
        [Description("自由作业")]
        FreeTask
    }

    public enum WBSResourceType
    {
        UserRole,
        BOM,
        BOQ,
        QBS,
        Document,
        Contract
    }
}
