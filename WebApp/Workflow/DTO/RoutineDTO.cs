using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;


namespace Workflow.DTO
{
    public enum ChooseUserType
    {
        /// <summary>
        ///范围:指定范围不超过20个人,超过则返回All. 
        /// </summary>
        Range,
        /// <summary>
        /// 所有人.
        /// </summary>
        All,
        /// <summary>
        /// 指定人.
        /// </summary>
        Special,
    }
    public enum OwnerType
    {
        /// <summary>
        /// 不需要人(如：结束环节).
        /// </summary>
        None = 0,
        /// <summary>
        /// 单人.
        /// </summary>
        Single = 1,
        /// <summary>
        /// 多人.
        /// </summary>
        Multi = 2,


    }
    /// <summary>
    /// 任务.
    /// </summary>
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
        /// 发送时间.
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 接收人.
        /// </summary>
        public string TaskUserName { get; set; }
        /// <summary>
        /// 发送人.
        /// </summary>
        public string SendTaskUserNames { get; set; }
        /// <summary>
        /// 表单实体ID.
        /// </summary>
        public string FormInstanceID { get; set; }
        /// <summary>
        /// 表单字段与值.
        /// </summary>
        public StringDictionary FormDic { get; set; }
        /// <summary>
        /// 路由.
        /// </summary>
        public List<RoutineDTO> Routines { get; set; }
        /// <summary>
        /// 表单PDF文件.
        /// </summary>
        public string PDF { get; set; }
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
        /// 选择人员方式.
        /// </summary>
        public ChooseUserType ChooseUserType { get; set; }
        /// <summary>
        /// 执行人数.
        /// </summary>
        public OwnerType OwnerType { get; set; }
    }
}