using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Workflow.Logic
{
    public enum FlowExecLogicType
    {
        [Description("启动其他流程")]
        StartFlow,
        [Description("操作数据表")]
        DBSet
    }

    [Description("流程任务状态")]
    public enum FlowTaskStatus
    {
        [Description("流程中")]
        Processing,
        [Description("已完成")]
        Complete,
        [Description("暂停")]
        Stop,
    }

    [Description("环节类型")]
    public enum StepTaskType
    {
        [Description("开始")]
        Inital,
        [Description("结束")]
        Completion,
        [Description("普通")]
        Normal,
        [Description("子流程")]
        SubFlow,

    }

    [Description("任务协作模式")]
    public enum TaskCooperationMode
    {
        [Description("单人完成")]
        Single,
        [Description("协作完成")]
        All,
        [Description("组单人完成")]
        GroupSingle,
    }

    [Description("路由类型")]
    public enum RoutingType
    {
        [Description("普通")]
        Normal,
        [Description("组内单发")]
        SingleBranch,
        [Description("分支/聚合")]
        Branch,
        [Description("单步回收")]
        WithdrawOther,
        [Description("弱控")]
        Weak,
        [Description("弱控完成")]
        AntiWeak,
        [Description("按人分发")]
        Distribute,
        [Description("打回")]
        Back
    }

    [Description("任务类型")]
    public enum TaskExecType
    {
        [Description("普通")]
        Normal,
        [Description("委托")]
        Delegate,
        [Description("传阅")]
        Circulate,
        [Description("加签")]
        Ask,
    }

    [Description("流程选人方式")]
    public enum FlowSelectUserMode
    {
        [Description("单选人")]
        SelectOneUser,
        [Description("多选人")]
        SelectMultiUser,
        [Description("范围单选人")]
        SelectOneUserInScope,
        [Description("范围多选人")]
        SelectMultiUserInScope,
        [Description("单选部门")]
        SelectOneOrg,
        [Description("多选部门")]
        SelectMultiOrg,
        //去掉重新范围选人功能
        //[Description("重新范围选人")]
        //SelectDoback,
        [Description("项目范围多选人")]
        SelectMultiPrjUser,
        [Description("项目范围单选人")]
        SelectOnePrjUser
    }

    [Description("钉钉审核")]
    public enum DingDingAudit
    {
        [Description("通过")]
        agree,
        [Description("拒绝")]
        refuse,
    }


    [Description("钉钉任务状态")]
    public enum DingDingTaskStatus
    {
        [Description("完成")]
        CANCELED,
        [Description("取消")]
        COMPLETED,
        //系统已经完成，没有更新到钉钉
        [Description("需要审核")]
        NeedAudit,

    }
}
