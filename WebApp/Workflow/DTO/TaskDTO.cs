using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;


namespace Workflow.DTO
{
    public enum OwnerType
    {
        /// <summary>
        /// 不需要人
        /// </summary>
        None = 0,

        /// <summary>
        /// 单选人，如果OwnerUserIDS有值，则为范围单选人
        /// </summary>
        Single = 1,

        /// <summary>
        /// 多选人，如果OwnerUserIDs有值，则为范围多选人
        /// </summary>
        Multi = 2,

        /// <summary>
        /// 指定人，指定人为OwnerUserIDs的人
        /// </summary>
        Special,

        /// <summary>
        /// 流程配置异常，没有执行人
        /// </summary>
        Error,

    }
    public class TaskDTO : AbstractDTO
    {
        /// <summary>
        /// 任务名称.
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 紧急度.
        /// </summary>
        public string Urgency { get; set; }
        /// <summary>
        /// 流程名称.
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 流程分类.
        /// </summary>
        public string FlowCategory { get; set; }

        /// <summary>
        /// 发送人.
        /// </summary>
        public string SendTaskUserNames { get; set; }
        /// <summary>
        /// 表单发起人
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 表单发起人ID
        /// </summary>
        public string CreateUserID { get; set; }
        /// <summary>
        /// 发送时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 接收人.
        /// </summary>
        public string TaskUserName { get; set; }

        /// <summary>
        /// 接收人ID.
        /// </summary>
        public string TaskUserID { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public bool IsFinished { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? ExecTime { get; set; }
        /// <summary>
        /// 表单实体ID.
        /// </summary>
        public string FormInstanceID { get; set; }
        /// <summary>
        /// 表单实体表表名.
        /// </summary>
        public string FormInstanceCode { get; set; }
        /// <summary>
        /// 表单字段与值.
        /// </summary>
        public StringDictionary FormDic { get; set; }
        /// <summary>
        /// 路由.
        /// </summary>
        public List<RoutineDTO> Routines { get; set; }
        /// <summary>
        /// 表单LinkUrl.
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 当IsPDF时，LinkUrl为PDF文件名，否则是表单，LinkUrl为表单地址;
        /// </summary>
        public bool IsPDF { get; set; }
        /// <summary>
        /// 流程表单的附件
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Attachments { get; set; }

        public string DefStepID { get; set; }

        public string DefStepName { get; set; }

        /// <summary>
        /// 流程编号、表单编号
        /// </summary>
        public string FlowDefCode { get; set; }
        /// <summary>
        /// 是否隐藏意见框
        /// </summary>
        public bool HideAdvice { get; set; }
    }

    /// <summary>
    /// 路由.
    /// </summary>
    public class RoutineDTO : AbstractDTO
    {
        /// <summary>
        /// 路由名字.
        /// </summary>
        public string RoutineName { get; set; }
        /// <summary>
        /// 路由执行人员IDs，多个用逗号分隔.
        /// </summary>
        public string OwnerUserIDs { get; set; }
        /// <summary>
        /// 执行人数.
        /// </summary>
        public OwnerType OwnerType { get; set; }
        /// <summary>
        /// 意见是否必填
        /// </summary>
        public bool MustInputComment { get; set; }

        public bool SelectAgain { get; set; }

        //默认意见
        public string DefaultComment { get; set; }

        public RoutineDTO()
        {
            MustInputComment = true;
        }
    }

    /// <summary>
    /// 流程跟踪.
    /// </summary>
    public class FlowTraceDTO : AbstractDTO
    {
        /// <summary>
        /// 环节名称
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        public string ExecUserID { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        public string ExecUserName { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? ExecTime { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        public string ExecComment { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string ExecRoutingName { get; set; }
        /// <summary>
        /// 流程回复记录
        /// </summary>
        public List<FlowReplyDTO> FlowReplys { get; set; }
        /// <summary>
        /// 审批来源
        /// </summary>
        public string ApprovalInMobile { get; set; }
    }


    public class PostSubmitTaskDTO : AbstractDTO
    {
        /// <summary>
        /// 选择的下一步路由ID,可能为btnDoBack，btnDoBackFirst，btnDoBackFirstReturn
        /// </summary>
        public string NextRoutingID { get; set; }
        /// <summary>
        /// 当前执行任务ID.
        /// </summary>
        public string TaskExecID { get; set; }
        /// <summary>
        /// 表单实体ID.
        /// </summary>
        public string FormInstanceID { get; set; }
        /// <summary>
        /// 表单物理表名
        /// </summary>
        public string FormInstanceCode { get; set; }
        /// <summary>
        /// 下一环节执行人ID.
        /// </summary>
        public string UserIDs { get; set; }
        /// <summary>
        /// 意见.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        ///  任务执行人
        /// </summary>
        public string ExecUserID { get; set; }
        /// <summary>
        /// 修订字段集合 Dictionary<string,string>
        /// </summary>
        public string FormDic { get; set; }

        /// <summary>
        /// 新版移动通的表单定义编号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 新版移动通的流程定义编号
        /// </summary>
        public string FlowCode { get; set; }

    }

    public class FlowReplyDTO
    {
        /// <summary>
        /// 回复ID
        /// </summary>
        public string ReplyID { get; set; }
        /// <summary>
        /// 回复流程信息ID
        /// </summary>
        public string FlowMsgID { get; set; }
        /// <summary>
        /// 回复人ID
        /// </summary>
        public string SenderUserID { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        public DateTime? SenderDate { get; set; }
        /// <summary>
        /// 回复意见
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string AttachFileIDs { get; set; }
    }
}