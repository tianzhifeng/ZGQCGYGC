

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "Workflow"
//     Connection String:      "Data Source=.;Initial Catalog=SINOAE_Workflow;User ID=sa;PWD=123.zxc;"

// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Newtonsoft.Json;
using System.ComponentModel;

//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace Workflow.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class WorkflowEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_E_RTXSynchNoticeAndTask> S_E_RTXSynchNoticeAndTask { get; set; } // S_E_RTXSynchNoticeAndTask
        public IDbSet<S_E_TxtOutTaskExec> S_E_TxtOutTaskExec { get; set; } // S_E_TxtOutTaskExec
        public IDbSet<S_E_TxtOutTaskExecLog> S_E_TxtOutTaskExecLog { get; set; } // S_E_TxtOutTaskExecLog
        public IDbSet<S_WF_DefDelegate> S_WF_DefDelegate { get; set; } // S_WF_DefDelegate
        public IDbSet<S_WF_DefFlow> S_WF_DefFlow { get; set; } // S_WF_DefFlow
        public IDbSet<S_WF_DefRouting> S_WF_DefRouting { get; set; } // S_WF_DefRouting
        public IDbSet<S_WF_DefStep> S_WF_DefStep { get; set; } // S_WF_DefStep
        public IDbSet<S_WF_DefSubForm> S_WF_DefSubForm { get; set; } // S_WF_DefSubForm
        public IDbSet<S_WF_InsDefFlow> S_WF_InsDefFlow { get; set; } // S_WF_InsDefFlow
        public IDbSet<S_WF_InsDefRouting> S_WF_InsDefRouting { get; set; } // S_WF_InsDefRouting
        public IDbSet<S_WF_InsDefStep> S_WF_InsDefStep { get; set; } // S_WF_InsDefStep
        public IDbSet<S_WF_InsFlow> S_WF_InsFlow { get; set; } // S_WF_InsFlow
        public IDbSet<S_WF_InsTask> S_WF_InsTask { get; set; } // S_WF_InsTask
        public IDbSet<S_WF_InsTaskExec> S_WF_InsTaskExec { get; set; } // S_WF_InsTaskExec
        public IDbSet<S_WF_InsVariable> S_WF_InsVariable { get; set; } // S_WF_InsVariable
        public IDbSet<Task> Task { get; set; } // Task

        static WorkflowEntities()
        {
            Database.SetInitializer<WorkflowEntities>(null);
        }

        public WorkflowEntities()
            : base("Name=Workflow")
        {
        }

        public WorkflowEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_E_RTXSynchNoticeAndTaskConfiguration());
            modelBuilder.Configurations.Add(new S_E_TxtOutTaskExecConfiguration());
            modelBuilder.Configurations.Add(new S_E_TxtOutTaskExecLogConfiguration());
            modelBuilder.Configurations.Add(new S_WF_DefDelegateConfiguration());
            modelBuilder.Configurations.Add(new S_WF_DefFlowConfiguration());
            modelBuilder.Configurations.Add(new S_WF_DefRoutingConfiguration());
            modelBuilder.Configurations.Add(new S_WF_DefStepConfiguration());
            modelBuilder.Configurations.Add(new S_WF_DefSubFormConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsDefFlowConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsDefRoutingConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsDefStepConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsFlowConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsTaskConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsTaskExecConfiguration());
            modelBuilder.Configurations.Add(new S_WF_InsVariableConfiguration());
            modelBuilder.Configurations.Add(new TaskConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_E_RTXSynchNoticeAndTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TaskExecIDOrMsgID { get{return _TaskExecIDOrMsgID;} set{_TaskExecIDOrMsgID = value??"";} } // TaskExecIDOrMsgID
		private string _TaskExecIDOrMsgID="";
		/// <summary>状态(Wait,Finish,Failed)</summary>	
		[Description("状态(Wait,Finish,Failed)")]
        public string State { get{return _State;} set{_State = value??"";} } // State
		private string _State="";
		/// <summary>标题</summary>	
		[Description("标题")]
        public string Title { get{return _Title;} set{_Title = value??"";} } // Title
		private string _Title="";
		/// <summary>内容</summary>	
		[Description("内容")]
        public string Content { get{return _Content;} set{_Content = value??"";} } // Content
		private string _Content="";
		/// <summary>页面链接地址</summary>	
		[Description("页面链接地址")]
        public string LinkURL { get{return _LinkURL;} set{_LinkURL = value??"";} } // LinkURL
		private string _LinkURL="";
		/// <summary>任务所属人</summary>	
		[Description("任务所属人")]
        public string OwnerUserID { get{return _OwnerUserID;} set{_OwnerUserID = value??"";} } // OwnerUserID
		private string _OwnerUserID="";
		/// <summary>任务所属人</summary>	
		[Description("任务所属人")]
        public string OwnerUserName { get{return _OwnerUserName;} set{_OwnerUserName = value??"";} } // OwnerUserName
		private string _OwnerUserName="";
		/// <summary></summary>	
		[Description("")]
        public string SendUserIDs { get{return _SendUserIDs;} set{_SendUserIDs = value??"";} } // SendUserIDs
		private string _SendUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string SendUserNames { get{return _SendUserNames;} set{_SendUserNames = value??"";} } // SendUserNames
		private string _SendUserNames="";
		/// <summary>任务创建日期</summary>	
		[Description("任务创建日期")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string InsFlowID { get{return _InsFlowID;} set{_InsFlowID = value??"";} } // InsFlowID
		private string _InsFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string FormID { get{return _FormID;} set{_FormID = value??"";} } // FormID
		private string _FormID="";
		/// <summary></summary>	
		[Description("")]
        public string TaskType { get{return _TaskType;} set{_TaskType = value??"";} } // TaskType
		private string _TaskType="";
		/// <summary>数据类型(消息(MSG),任务(TASK))</summary>	
		[Description("数据类型(消息(MSG),任务(TASK))")]
        public string DataType { get{return _DataType;} set{_DataType = value??"";} } // DataType
		private string _DataType="";
		/// <summary>任务执行时间</summary>	
		[Description("任务执行时间")]
        public DateTime? ExecTime { get; set; } // ExecTime
		/// <summary>错误信息</summary>	
		[Description("错误信息")]
        public string ErrorMsg { get{return _ErrorMsg;} set{_ErrorMsg = value??"";} } // ErrorMsg
		private string _ErrorMsg="";
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_E_TxtOutTaskExec : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string InsTaskExecID { get{return _InsTaskExecID;} set{_InsTaskExecID = value??"";} } // InsTaskExecID
		private string _InsTaskExecID="";
		/// <summary></summary>	
		[Description("")]
        public string InsFlowID { get{return _InsFlowID;} set{_InsFlowID = value??"";} } // InsFlowID
		private string _InsFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string OutProcessCode { get{return _OutProcessCode;} set{_OutProcessCode = value??"";} } // OutProcessCode
		private string _OutProcessCode="";
		/// <summary></summary>	
		[Description("")]
        public string OutProcessInstanceId { get{return _OutProcessInstanceId;} set{_OutProcessInstanceId = value??"";} } // OutProcessInstanceId
		private string _OutProcessInstanceId="";
		/// <summary></summary>	
		[Description("")]
        public string OutTaskId { get{return _OutTaskId;} set{_OutTaskId = value??"";} } // OutTaskId
		private string _OutTaskId="";
		/// <summary></summary>	
		[Description("")]
        public string OutMsg { get{return _OutMsg;} set{_OutMsg = value??"";} } // OutMsg
		private string _OutMsg="";
		/// <summary></summary>	
		[Description("")]
        public string Status { get{return _Status;} set{_Status = value??"";} } // Status
		private string _Status="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUid { get{return _CreateUid;} set{_CreateUid = value??"";} } // CreateUid
		private string _CreateUid="";
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_E_TxtOutTaskExecLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string InsTaskExecID { get{return _InsTaskExecID;} set{_InsTaskExecID = value??"";} } // InsTaskExecID
		private string _InsTaskExecID="";
		/// <summary></summary>	
		[Description("")]
        public string OutMsg { get{return _OutMsg;} set{_OutMsg = value??"";} } // OutMsg
		private string _OutMsg="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string Status { get{return _Status;} set{_Status = value??"";} } // Status
		private string _Status="";
    }

	/// <summary>流程委托表</summary>	
	[Description("流程委托表")]
    public partial class S_WF_DefDelegate : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary>流程定义ID</summary>	
		[Description("流程定义ID")]
        public string DefFlowID { get{return _DefFlowID;} set{_DefFlowID = value??"";} } // DefFlowID
		private string _DefFlowID="";
		/// <summary>委托用户ID</summary>	
		[Description("委托用户ID")]
        public string DelegateUserID { get{return _DelegateUserID;} set{_DelegateUserID = value??"";} } // DelegateUserID
		private string _DelegateUserID="";
		/// <summary>委托用户姓名</summary>	
		[Description("委托用户姓名")]
        public string DelegateUserName { get{return _DelegateUserName;} set{_DelegateUserName = value??"";} } // DelegateUserName
		private string _DelegateUserName="";
		/// <summary></summary>	
		[Description("")]
        public string DelegateRoleID { get{return _DelegateRoleID;} set{_DelegateRoleID = value??"";} } // DelegateRoleID
		private string _DelegateRoleID="";
		/// <summary></summary>	
		[Description("")]
        public string DelegateRoleName { get{return _DelegateRoleName;} set{_DelegateRoleName = value??"";} } // DelegateRoleName
		private string _DelegateRoleName="";
		/// <summary></summary>	
		[Description("")]
        public string DelegateOrgID { get{return _DelegateOrgID;} set{_DelegateOrgID = value??"";} } // DelegateOrgID
		private string _DelegateOrgID="";
		/// <summary></summary>	
		[Description("")]
        public string DelegateOrgName { get{return _DelegateOrgName;} set{_DelegateOrgName = value??"";} } // DelegateOrgName
		private string _DelegateOrgName="";
		/// <summary>被委托用户ID</summary>	
		[Description("被委托用户ID")]
        public string BeDelegateUserID { get{return _BeDelegateUserID;} set{_BeDelegateUserID = value??"";} } // BeDelegateUserID
		private string _BeDelegateUserID="";
		/// <summary>被委托用户姓名</summary>	
		[Description("被委托用户姓名")]
        public string BeDelegateUserName { get{return _BeDelegateUserName;} set{_BeDelegateUserName = value??"";} } // BeDelegateUserName
		private string _BeDelegateUserName="";
		/// <summary>委托时间</summary>	
		[Description("委托时间")]
        public DateTime? DelegateTime { get; set; } // DelegateTime
		/// <summary>开始时间</summary>	
		[Description("开始时间")]
        public DateTime? BeginTime { get; set; } // BeginTime
		/// <summary>结束时间</summary>	
		[Description("结束时间")]
        public DateTime? EndTime { get; set; } // EndTime

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_DefFlow S_WF_DefFlow { get; set; } //  DefFlowID - FK_S_WF_DefDelegate_S_WF_DefFlow
    }

	/// <summary>流程定义表</summary>	
	[Description("流程定义表")]
    public partial class S_WF_DefFlow : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary>连接名</summary>	
		[Description("连接名")]
        public string ConnName { get{return _ConnName;} set{_ConnName = value??"";} } // ConnName
		private string _ConnName="";
		/// <summary>表名</summary>	
		[Description("表名")]
        public string TableName { get{return _TableName;} set{_TableName = value??"";} } // TableName
		private string _TableName="";
		/// <summary></summary>	
		[Description("")]
        public string FormUrl { get{return _FormUrl;} set{_FormUrl = value??"";} } // FormUrl
		private string _FormUrl="";
		/// <summary></summary>	
		[Description("")]
        public string FormWidth { get{return _FormWidth;} set{_FormWidth = value??"";} } // FormWidth
		private string _FormWidth="";
		/// <summary></summary>	
		[Description("")]
        public string FormHeight { get{return _FormHeight;} set{_FormHeight = value??"";} } // FormHeight
		private string _FormHeight="";
		/// <summary>流程名模板</summary>	
		[Description("流程名模板")]
        public string FlowNameTemplete { get{return _FlowNameTemplete;} set{_FlowNameTemplete = value??"";} } // FlowNameTemplete
		private string _FlowNameTemplete="";
		/// <summary>流程分类模板</summary>	
		[Description("流程分类模板")]
        public string FlowCategorytemplete { get{return _FlowCategorytemplete;} set{_FlowCategorytemplete = value??"";} } // FlowCategorytemplete
		private string _FlowCategorytemplete="";
		/// <summary>流程子分类模板</summary>	
		[Description("流程子分类模板")]
        public string FlowSubCategoryTemplete { get{return _FlowSubCategoryTemplete;} set{_FlowSubCategoryTemplete = value??"";} } // FlowSubCategoryTemplete
		private string _FlowSubCategoryTemplete="";
		/// <summary>任务名模板</summary>	
		[Description("任务名模板")]
        public string TaskNameTemplete { get{return _TaskNameTemplete;} set{_TaskNameTemplete = value??"";} } // TaskNameTemplete
		private string _TaskNameTemplete="";
		/// <summary>任务分类模板</summary>	
		[Description("任务分类模板")]
        public string TaskCategoryTemplete { get{return _TaskCategoryTemplete;} set{_TaskCategoryTemplete = value??"";} } // TaskCategoryTemplete
		private string _TaskCategoryTemplete="";
		/// <summary>任务子分类模板</summary>	
		[Description("任务子分类模板")]
        public string TaskSubCategoryTemplete { get{return _TaskSubCategoryTemplete;} set{_TaskSubCategoryTemplete = value??"";} } // TaskSubCategoryTemplete
		private string _TaskSubCategoryTemplete="";
		/// <summary>初始化变量</summary>	
		[Description("初始化变量")]
        public string InitVariable { get{return _InitVariable;} set{_InitVariable = value??"";} } // InitVariable
		private string _InitVariable="";
		/// <summary>是否允许删除表单</summary>	
		[Description("是否允许删除表单")]
        public string AllowDeleteForm { get{return _AllowDeleteForm;} set{_AllowDeleteForm = value??"";} } // AllowDeleteForm
		private string _AllowDeleteForm="";
		/// <summary>是否给申请发消息</summary>	
		[Description("是否给申请发消息")]
        public string SendMsgToApplicant { get{return _SendMsgToApplicant;} set{_SendMsgToApplicant = value??"";} } // SendMsgToApplicant
		private string _SendMsgToApplicant="";
		/// <summary>流程图XML</summary>	
		[Description("流程图XML")]
        public string ViewConfig { get{return _ViewConfig;} set{_ViewConfig = value??"";} } // ViewConfig
		private string _ViewConfig="";
		/// <summary>最后修改时间</summary>	
		[Description("最后修改时间")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary>流程定义分类ID</summary>	
		[Description("流程定义分类ID")]
        public string CategoryID { get{return _CategoryID;} set{_CategoryID = value??"";} } // CategoryID
		private string _CategoryID="";
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get{return _Description;} set{_Description = value??"";} } // Description
		private string _Description="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartA { get{return _FormNumberPartA;} set{_FormNumberPartA = value??"";} } // FormNumberPartA
		private string _FormNumberPartA="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartB { get{return _FormNumberPartB;} set{_FormNumberPartB = value??"";} } // FormNumberPartB
		private string _FormNumberPartB="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartC { get{return _FormNumberPartC;} set{_FormNumberPartC = value??"";} } // FormNumberPartC
		private string _FormNumberPartC="";
		/// <summary></summary>	
		[Description("")]
        public string AlreadyReleased { get{return _AlreadyReleased;} set{_AlreadyReleased = value??"";} } // AlreadyReleased
		private string _AlreadyReleased="";
		/// <summary></summary>	
		[Description("")]
        public string MsgSendToAll { get{return _MsgSendToAll;} set{_MsgSendToAll = value??"";} } // MsgSendToAll
		private string _MsgSendToAll="";
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get{return _CreateUserID;} set{_CreateUserID = value??"";} } // CreateUserID
		private string _CreateUserID="";
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get{return _CreateUserName;} set{_CreateUserName = value??"";} } // CreateUserName
		private string _CreateUserName="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get{return _ModifyUserID;} set{_ModifyUserID = value??"";} } // ModifyUserID
		private string _ModifyUserID="";
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get{return _ModifyUserName;} set{_ModifyUserName = value??"";} } // ModifyUserName
		private string _ModifyUserName="";
		/// <summary></summary>	
		[Description("")]
        public string Collision { get{return _Collision;} set{_Collision = value??"";} } // Collision
		private string _Collision="";
		/// <summary></summary>	
		[Description("")]
        public string isFlowChart { get{return _isFlowChart;} set{_isFlowChart = value??"";} } // isFlowChart
		private string _isFlowChart="";
		/// <summary></summary>	
		[Description("")]
        public string CompanyName { get{return _CompanyName;} set{_CompanyName = value??"";} } // CompanyName
		private string _CompanyName="";
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get{return _CompanyID;} set{_CompanyID = value??"";} } // CompanyID
		private string _CompanyID="";

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_DefDelegate> S_WF_DefDelegate { get { onS_WF_DefDelegateGetting(); return _S_WF_DefDelegate;} }
		private ICollection<S_WF_DefDelegate> _S_WF_DefDelegate;
		partial void onS_WF_DefDelegateGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_DefRouting> S_WF_DefRouting { get { onS_WF_DefRoutingGetting(); return _S_WF_DefRouting;} }
		private ICollection<S_WF_DefRouting> _S_WF_DefRouting;
		partial void onS_WF_DefRoutingGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_DefStep> S_WF_DefStep { get { onS_WF_DefStepGetting(); return _S_WF_DefStep;} }
		private ICollection<S_WF_DefStep> _S_WF_DefStep;
		partial void onS_WF_DefStepGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_DefSubForm> S_WF_DefSubForm { get { onS_WF_DefSubFormGetting(); return _S_WF_DefSubForm;} }
		private ICollection<S_WF_DefSubForm> _S_WF_DefSubForm;
		partial void onS_WF_DefSubFormGetting();


        public S_WF_DefFlow()
        {
			FlowNameTemplete = "";
			TaskNameTemplete = "";
			AlreadyReleased = "0";
            _S_WF_DefDelegate = new List<S_WF_DefDelegate>();
            _S_WF_DefRouting = new List<S_WF_DefRouting>();
            _S_WF_DefStep = new List<S_WF_DefStep>();
            _S_WF_DefSubForm = new List<S_WF_DefSubForm>();
        }
    }

	/// <summary>流程定义路由表</summary>	
	[Description("流程定义路由表")]
    public partial class S_WF_DefRouting : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary>流程定义ID</summary>	
		[Description("流程定义ID")]
        public string DefFlowID { get{return _DefFlowID;} set{_DefFlowID = value??"";} } // DefFlowID
		private string _DefFlowID="";
		/// <summary>环节定义ID</summary>	
		[Description("环节定义ID")]
        public string DefStepID { get{return _DefStepID;} set{_DefStepID = value??"";} } // DefStepID
		private string _DefStepID="";
		/// <summary>下一环节ID</summary>	
		[Description("下一环节ID")]
        public string EndID { get{return _EndID;} set{_EndID = value??"";} } // EndID
		private string _EndID="";
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary>值</summary>	
		[Description("值")]
        public string Value { get{return _Value;} set{_Value = value??"";} } // Value
		private string _Value="";
		/// <summary>非空字段</summary>	
		[Description("非空字段")]
        public string NotNullFields { get{return _NotNullFields;} set{_NotNullFields = value??"";} } // NotNullFields
		private string _NotNullFields="";
		/// <summary>变量设置</summary>	
		[Description("变量设置")]
        public string VariableSet { get{return _VariableSet;} set{_VariableSet = value??"";} } // VariableSet
		private string _VariableSet="";
		/// <summary>表单设置</summary>	
		[Description("表单设置")]
        public string FormDataSet { get{return _FormDataSet;} set{_FormDataSet = value??"";} } // FormDataSet
		private string _FormDataSet="";
		/// <summary>是否保存表单</summary>	
		[Description("是否保存表单")]
        public string SaveForm { get{return _SaveForm;} set{_SaveForm = value??"";} } // SaveForm
		private string _SaveForm="";
		/// <summary>是否必填意见</summary>	
		[Description("是否必填意见")]
        public string MustInputComment { get{return _MustInputComment;} set{_MustInputComment = value??"";} } // MustInputComment
		private string _MustInputComment="";
		/// <summary>是否保存表单版本</summary>	
		[Description("是否保存表单版本")]
        public string SaveFormVersion { get{return _SaveFormVersion;} set{_SaveFormVersion = value??"";} } // SaveFormVersion
		private string _SaveFormVersion="";
		/// <summary>默认意见</summary>	
		[Description("默认意见")]
        public string DefaultComment { get{return _DefaultComment;} set{_DefaultComment = value??"";} } // DefaultComment
		private string _DefaultComment="";
		/// <summary>签字字段</summary>	
		[Description("签字字段")]
        public string SignatureField { get{return _SignatureField;} set{_SignatureField = value??"";} } // SignatureField
		private string _SignatureField="";
		/// <summary>签字保护字段</summary>	
		[Description("签字保护字段")]
        public string SignatureProtectFields { get{return _SignatureProtectFields;} set{_SignatureProtectFields = value??"";} } // SignatureProtectFields
		private string _SignatureProtectFields="";
		/// <summary>签字显示位置Div</summary>	
		[Description("签字显示位置Div")]
        public string SignatureDivID { get{return _SignatureDivID;} set{_SignatureDivID = value??"";} } // SignatureDivID
		private string _SignatureDivID="";
		/// <summary>取消签字的位置Div</summary>	
		[Description("取消签字的位置Div")]
        public string SignatureCancelDivIDs { get{return _SignatureCancelDivIDs;} set{_SignatureCancelDivIDs = value??"";} } // SignatureCancelDivIDs
		private string _SignatureCancelDivIDs="";
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>表单数据权限</summary>	
		[Description("表单数据权限")]
        public string AuthFormData { get{return _AuthFormData;} set{_AuthFormData = value??"";} } // AuthFormData
		private string _AuthFormData="";
		/// <summary></summary>	
		[Description("")]
        public string AuthTargetUser { get{return _AuthTargetUser;} set{_AuthTargetUser = value??"";} } // AuthTargetUser
		private string _AuthTargetUser="";
		/// <summary>组织权限</summary>	
		[Description("组织权限")]
        public string AuthOrgIDs { get{return _AuthOrgIDs;} set{_AuthOrgIDs = value??"";} } // AuthOrgIDs
		private string _AuthOrgIDs="";
		/// <summary>组织权限</summary>	
		[Description("组织权限")]
        public string AuthOrgNames { get{return _AuthOrgNames;} set{_AuthOrgNames = value??"";} } // AuthOrgNames
		private string _AuthOrgNames="";
		/// <summary>角色权限</summary>	
		[Description("角色权限")]
        public string AuthRoleIDs { get{return _AuthRoleIDs;} set{_AuthRoleIDs = value??"";} } // AuthRoleIDs
		private string _AuthRoleIDs="";
		/// <summary>角色权限</summary>	
		[Description("角色权限")]
        public string AuthRoleNames { get{return _AuthRoleNames;} set{_AuthRoleNames = value??"";} } // AuthRoleNames
		private string _AuthRoleNames="";
		/// <summary>用户权限</summary>	
		[Description("用户权限")]
        public string AuthUserIDs { get{return _AuthUserIDs;} set{_AuthUserIDs = value??"";} } // AuthUserIDs
		private string _AuthUserIDs="";
		/// <summary>用户权限</summary>	
		[Description("用户权限")]
        public string AuthUserNames { get{return _AuthUserNames;} set{_AuthUserNames = value??"";} } // AuthUserNames
		private string _AuthUserNames="";
		/// <summary>变量权限</summary>	
		[Description("变量权限")]
        public string AuthVariable { get{return _AuthVariable;} set{_AuthVariable = value??"";} } // AuthVariable
		private string _AuthVariable="";
		/// <summary>下一环节执行人</summary>	
		[Description("下一环节执行人")]
        public string UserIDs { get{return _UserIDs;} set{_UserIDs = value??"";} } // UserIDs
		private string _UserIDs="";
		/// <summary>下一环节执行人</summary>	
		[Description("下一环节执行人")]
        public string UserNames { get{return _UserNames;} set{_UserNames = value??"";} } // UserNames
		private string _UserNames="";
		/// <summary>下一环节执行人取自环节任务</summary>	
		[Description("下一环节执行人取自环节任务")]
        public string UserIDsFromStep { get{return _UserIDsFromStep;} set{_UserIDsFromStep = value??"";} } // UserIDsFromStep
		private string _UserIDsFromStep="";
		/// <summary>下一环节执行人取自环节任务发送人</summary>	
		[Description("下一环节执行人取自环节任务发送人")]
        public string UserIDsFromStepSender { get{return _UserIDsFromStepSender;} set{_UserIDsFromStepSender = value??"";} } // UserIDsFromStepSender
		private string _UserIDsFromStepSender="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromStepExec { get{return _UserIDsFromStepExec;} set{_UserIDsFromStepExec = value??"";} } // UserIDsFromStepExec
		private string _UserIDsFromStepExec="";
		/// <summary>下一环节执行人取自表单</summary>	
		[Description("下一环节执行人取自表单")]
        public string UserIDsFromField { get{return _UserIDsFromField;} set{_UserIDsFromField = value??"";} } // UserIDsFromField
		private string _UserIDsFromField="";
		/// <summary>下一环节执行人分组取自表单</summary>	
		[Description("下一环节执行人分组取自表单")]
        public string UserIDsGroupFromField { get{return _UserIDsGroupFromField;} set{_UserIDsGroupFromField = value??"";} } // UserIDsGroupFromField
		private string _UserIDsGroupFromField="";
		/// <summary>下一环节执行人角色</summary>	
		[Description("下一环节执行人角色")]
        public string RoleIDs { get{return _RoleIDs;} set{_RoleIDs = value??"";} } // RoleIDs
		private string _RoleIDs="";
		/// <summary>下一环节执行人角色</summary>	
		[Description("下一环节执行人角色")]
        public string RoleNames { get{return _RoleNames;} set{_RoleNames = value??"";} } // RoleNames
		private string _RoleNames="";
		/// <summary>下一环节执行人取自字段</summary>	
		[Description("下一环节执行人取自字段")]
        public string RoleIDsFromField { get{return _RoleIDsFromField;} set{_RoleIDsFromField = value??"";} } // RoleIDsFromField
		private string _RoleIDsFromField="";
		/// <summary>下一环节执行人部门</summary>	
		[Description("下一环节执行人部门")]
        public string OrgIDs { get{return _OrgIDs;} set{_OrgIDs = value??"";} } // OrgIDs
		private string _OrgIDs="";
		/// <summary>下一环节执行人部门</summary>	
		[Description("下一环节执行人部门")]
        public string OrgNames { get{return _OrgNames;} set{_OrgNames = value??"";} } // OrgNames
		private string _OrgNames="";
		/// <summary>下一环节执行人部门取自字段</summary>	
		[Description("下一环节执行人部门取自字段")]
        public string OrgIDFromField { get{return _OrgIDFromField;} set{_OrgIDFromField = value??"";} } // OrgIDFromField
		private string _OrgIDFromField="";
		/// <summary>下一环节执行人部门取自用户（当前，任务发送人）</summary>	
		[Description("下一环节执行人部门取自用户（当前，任务发送人）")]
        public string OrgIDFromUser { get{return _OrgIDFromUser;} set{_OrgIDFromUser = value??"";} } // OrgIDFromUser
		private string _OrgIDFromUser="";
		/// <summary>选择页面类型</summary>	
		[Description("选择页面类型")]
        public string SelectMode { get{return _SelectMode;} set{_SelectMode = value??"";} } // SelectMode
		private string _SelectMode="";
		/// <summary></summary>	
		[Description("")]
        public string SelectAgain { get{return _SelectAgain;} set{_SelectAgain = value??"";} } // SelectAgain
		private string _SelectAgain="";
		/// <summary></summary>	
		[Description("")]
        public string AllowWithdraw { get{return _AllowWithdraw;} set{_AllowWithdraw = value??"";} } // AllowWithdraw
		private string _AllowWithdraw="";
		/// <summary></summary>	
		[Description("")]
        public string Title { get{return _Title;} set{_Title = value??"";} } // Title
		private string _Title="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBack { get{return _AllowDoBack;} set{_AllowDoBack = value??"";} } // AllowDoBack
		private string _AllowDoBack="";
		/// <summary>直送（就是只能打回）=AllowDoBackReturn</summary>	
		[Description("直送（就是只能打回）=AllowDoBackReturn")]
        public string OnlyDoBack { get{return _OnlyDoBack;} set{_OnlyDoBack = value??"";} } // OnlyDoBack
		private string _OnlyDoBack="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDs { get{return _MsgUserIDs;} set{_MsgUserIDs = value??"";} } // MsgUserIDs
		private string _MsgUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserNames { get{return _MsgUserNames;} set{_MsgUserNames = value??"";} } // MsgUserNames
		private string _MsgUserNames="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStepExec { get{return _MsgUserIDsFromStepExec;} set{_MsgUserIDsFromStepExec = value??"";} } // MsgUserIDsFromStepExec
		private string _MsgUserIDsFromStepExec="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStep { get{return _MsgUserIDsFromStep;} set{_MsgUserIDsFromStep = value??"";} } // MsgUserIDsFromStep
		private string _MsgUserIDsFromStep="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStepSender { get{return _MsgUserIDsFromStepSender;} set{_MsgUserIDsFromStepSender = value??"";} } // MsgUserIDsFromStepSender
		private string _MsgUserIDsFromStepSender="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromField { get{return _MsgUserIDsFromField;} set{_MsgUserIDsFromField = value??"";} } // MsgUserIDsFromField
		private string _MsgUserIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleIDs { get{return _MsgRoleIDs;} set{_MsgRoleIDs = value??"";} } // MsgRoleIDs
		private string _MsgRoleIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleIDsFromField { get{return _MsgRoleIDsFromField;} set{_MsgRoleIDsFromField = value??"";} } // MsgRoleIDsFromField
		private string _MsgRoleIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDs { get{return _MsgOrgIDs;} set{_MsgOrgIDs = value??"";} } // MsgOrgIDs
		private string _MsgOrgIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDFromUser { get{return _MsgOrgIDFromUser;} set{_MsgOrgIDFromUser = value??"";} } // MsgOrgIDFromUser
		private string _MsgOrgIDFromUser="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDsFromField { get{return _MsgOrgIDsFromField;} set{_MsgOrgIDsFromField = value??"";} } // MsgOrgIDsFromField
		private string _MsgOrgIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgTmpl { get{return _MsgTmpl;} set{_MsgTmpl = value??"";} } // MsgTmpl
		private string _MsgTmpl="";
		/// <summary></summary>	
		[Description("")]
        public string MsgType { get{return _MsgType;} set{_MsgType = value??"";} } // MsgType
		private string _MsgType="";
		/// <summary></summary>	
		[Description("")]
        public string MsgSendToTaskUser { get{return _MsgSendToTaskUser;} set{_MsgSendToTaskUser = value??"";} } // MsgSendToTaskUser
		private string _MsgSendToTaskUser="";
		/// <summary>禁止自动通过</summary>	
		[Description("禁止自动通过")]
        public string DenyAutoPass { get{return _DenyAutoPass;} set{_DenyAutoPass = value??"";} } // DenyAutoPass
		private string _DenyAutoPass="";
		/// <summary>排除用户</summary>	
		[Description("排除用户")]
        public string ExcludeUser { get{return _ExcludeUser;} set{_ExcludeUser = value??"";} } // ExcludeUser
		private string _ExcludeUser="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleNames { get{return _MsgRoleNames;} set{_MsgRoleNames = value??"";} } // MsgRoleNames
		private string _MsgRoleNames="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgNames { get{return _MsgOrgNames;} set{_MsgOrgNames = value??"";} } // MsgOrgNames
		private string _MsgOrgNames="";
		/// <summary>是否需要输入签字密码</summary>	
		[Description("是否需要输入签字密码")]
        public string InputSignPwd { get{return _InputSignPwd;} set{_InputSignPwd = value??"";} } // InputSignPwd
		private string _InputSignPwd="";
		/// <summary></summary>	
		[Description("")]
        public string AuthFromSql { get{return _AuthFromSql;} set{_AuthFromSql = value??"";} } // AuthFromSql
		private string _AuthFromSql="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromSql { get{return _UserIDsFromSql;} set{_UserIDsFromSql = value??"";} } // UserIDsFromSql
		private string _UserIDsFromSql="";
		/// <summary></summary>	
		[Description("")]
        public string AuthFromSqlMemo { get{return _AuthFromSqlMemo;} set{_AuthFromSqlMemo = value??"";} } // AuthFromSqlMemo
		private string _AuthFromSqlMemo="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureType { get{return _SignatureType;} set{_SignatureType = value??"";} } // SignatureType
		private string _SignatureType="";
		/// <summary></summary>	
		[Description("")]
        public string ExecLogic { get{return _ExecLogic;} set{_ExecLogic = value??"";} } // ExecLogic
		private string _ExecLogic="";
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get{return _NameEN;} set{_NameEN = value??"";} } // NameEN
		private string _NameEN="";

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_DefFlow S_WF_DefFlow { get; set; } //  DefFlowID - FK_S_WF_DefRouting_S_WF_DefFlow
		[JsonIgnore]
        public virtual S_WF_DefStep S_WF_DefStep { get; set; } //  DefStepID - FK_S_WF_DefRouting_S_WF_DefStep

        public S_WF_DefRouting()
        {
			UserIDsGroupFromField = "";
			SelectMode = "SelectOneUser";
        }
    }

	/// <summary>流程定义环节表</summary>	
	[Description("流程定义环节表")]
    public partial class S_WF_DefStep : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string DefFlowID { get{return _DefFlowID;} set{_DefFlowID = value??"";} } // DefFlowID
		private string _DefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string SubFormID { get{return _SubFormID;} set{_SubFormID = value??"";} } // SubFormID
		private string _SubFormID="";
		/// <summary></summary>	
		[Description("")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary></summary>	
		[Description("")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string AllowDelegate { get{return _AllowDelegate;} set{_AllowDelegate = value??"";} } // AllowDelegate
		private string _AllowDelegate="";
		/// <summary></summary>	
		[Description("")]
        public string AllowCirculate { get{return _AllowCirculate;} set{_AllowCirculate = value??"";} } // AllowCirculate
		private string _AllowCirculate="";
		/// <summary></summary>	
		[Description("")]
        public string AllowAsk { get{return _AllowAsk;} set{_AllowAsk = value??"";} } // AllowAsk
		private string _AllowAsk="";
		/// <summary></summary>	
		[Description("")]
        public string AllowSave { get{return _AllowSave;} set{_AllowSave = value??"";} } // AllowSave
		private string _AllowSave="";
		/// <summary></summary>	
		[Description("")]
        public string SaveVariableAuth { get{return _SaveVariableAuth;} set{_SaveVariableAuth = value??"";} } // SaveVariableAuth
		private string _SaveVariableAuth="";
		/// <summary></summary>	
		[Description("")]
        public string SubFlowCode { get{return _SubFlowCode;} set{_SubFlowCode = value??"";} } // SubFlowCode
		private string _SubFlowCode="";
		/// <summary></summary>	
		[Description("")]
        public string WaitingStepIDs { get{return _WaitingStepIDs;} set{_WaitingStepIDs = value??"";} } // WaitingStepIDs
		private string _WaitingStepIDs="";
		/// <summary>Single</summary>	
		[Description("Single")]
        public string CooperationMode { get{return _CooperationMode;} set{_CooperationMode = value??"";} } // CooperationMode
		private string _CooperationMode="";
		/// <summary></summary>	
		[Description("")]
        public string Phase { get{return _Phase;} set{_Phase = value??"";} } // Phase
		private string _Phase="";
		/// <summary></summary>	
		[Description("")]
        public string HiddenElements { get{return _HiddenElements;} set{_HiddenElements = value??"";} } // HiddenElements
		private string _HiddenElements="";
		/// <summary></summary>	
		[Description("")]
        public string VisibleElements { get{return _VisibleElements;} set{_VisibleElements = value??"";} } // VisibleElements
		private string _VisibleElements="";
		/// <summary></summary>	
		[Description("")]
        public string EditableElements { get{return _EditableElements;} set{_EditableElements = value??"";} } // EditableElements
		private string _EditableElements="";
		/// <summary></summary>	
		[Description("")]
        public string DisableElements { get{return _DisableElements;} set{_DisableElements = value??"";} } // DisableElements
		private string _DisableElements="";
		/// <summary>紧急度</summary>	
		[Description("紧急度")]
        public string Urgency { get{return _Urgency;} set{_Urgency = value??"";} } // Urgency
		private string _Urgency="";
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutAutoPass { get; set; } // TimeoutAutoPass
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutNotice { get; set; } // TimeoutNotice
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutAlarm { get; set; } // TimeoutAlarm
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutDeadline { get; set; } // TimeoutDeadline
		/// <summary>流程结束时保留任务</summary>	
		[Description("流程结束时保留任务")]
        public string KeepWhenEnd { get{return _KeepWhenEnd;} set{_KeepWhenEnd = value??"";} } // KeepWhenEnd
		private string _KeepWhenEnd="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBackFirst { get{return _AllowDoBackFirst;} set{_AllowDoBackFirst = value??"";} } // AllowDoBackFirst
		private string _AllowDoBackFirst="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBackFirstReturn { get{return _AllowDoBackFirstReturn;} set{_AllowDoBackFirstReturn = value??"";} } // AllowDoBackFirstReturn
		private string _AllowDoBackFirstReturn="";
		/// <summary></summary>	
		[Description("")]
        public string DoBackSignField { get{return _DoBackSignField;} set{_DoBackSignField = value??"";} } // DoBackSignField
		private string _DoBackSignField="";
		/// <summary>是否允许推送到手机端,移动通白名单使用</summary>	
		[Description("是否允许推送到手机端,移动通白名单使用")]
        public string AllowToMobile { get{return _AllowToMobile;} set{_AllowToMobile = value??"";} } // AllowToMobile
		private string _AllowToMobile="";
		/// <summary>是否显示意见框</summary>	
		[Description("是否显示意见框")]
        public string HideAdvice { get{return _HideAdvice;} set{_HideAdvice = value??"";} } // HideAdvice
		private string _HideAdvice="";
		/// <summary>执行人为空时跳转到其他环节</summary>	
		[Description("执行人为空时跳转到其他环节")]
        public string EmptyToStep { get{return _EmptyToStep;} set{_EmptyToStep = value??"";} } // EmptyToStep
		private string _EmptyToStep="";
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get{return _NameEN;} set{_NameEN = value??"";} } // NameEN
		private string _NameEN="";
		/// <summary></summary>	
		[Description("")]
        public string DoBackButtonText { get{return _DoBackButtonText;} set{_DoBackButtonText = value??"";} } // DoBackButtonText
		private string _DoBackButtonText="";
		/// <summary>打回和打回首环节时是否保存表单</summary>	
		[Description("打回和打回首环节时是否保存表单")]
        public string DoBackSave { get{return _DoBackSave;} set{_DoBackSave = value??"";} } // DoBackSave
		private string _DoBackSave="";

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_DefRouting> S_WF_DefRouting { get { onS_WF_DefRoutingGetting(); return _S_WF_DefRouting;} }
		private ICollection<S_WF_DefRouting> _S_WF_DefRouting;
		partial void onS_WF_DefRoutingGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_DefFlow S_WF_DefFlow { get; set; } //  DefFlowID - FK_S_WF_DefStep_S_WF_DefFlow

        public S_WF_DefStep()
        {
			SubFlowCode = "";
			WaitingStepIDs = "";
			HiddenElements = "";
			VisibleElements = "";
			EditableElements = "";
			DisableElements = "";
            _S_WF_DefRouting = new List<S_WF_DefRouting>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_WF_DefSubForm : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string DefFlowID { get{return _DefFlowID;} set{_DefFlowID = value??"";} } // DefFlowID
		private string _DefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get{return _ConnName;} set{_ConnName = value??"";} } // ConnName
		private string _ConnName="";
		/// <summary></summary>	
		[Description("")]
        public string TableName { get{return _TableName;} set{_TableName = value??"";} } // TableName
		private string _TableName="";
		/// <summary></summary>	
		[Description("")]
        public string FormUrl { get{return _FormUrl;} set{_FormUrl = value??"";} } // FormUrl
		private string _FormUrl="";
		/// <summary></summary>	
		[Description("")]
        public string FormWidth { get{return _FormWidth;} set{_FormWidth = value??"";} } // FormWidth
		private string _FormWidth="";
		/// <summary></summary>	
		[Description("")]
        public string FormHeight { get{return _FormHeight;} set{_FormHeight = value??"";} } // FormHeight
		private string _FormHeight="";

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_DefFlow S_WF_DefFlow { get; set; } //  DefFlowID - FK_S_WF_DefSubForm_S_WF_DefFlow
    }

	/// <summary>流程定义实例表</summary>	
	[Description("流程定义实例表")]
    public partial class S_WF_InsDefFlow : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string DefFlowID { get{return _DefFlowID;} set{_DefFlowID = value??"";} } // DefFlowID
		private string _DefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary></summary>	
		[Description("")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get{return _ConnName;} set{_ConnName = value??"";} } // ConnName
		private string _ConnName="";
		/// <summary></summary>	
		[Description("")]
        public string TableName { get{return _TableName;} set{_TableName = value??"";} } // TableName
		private string _TableName="";
		/// <summary></summary>	
		[Description("")]
        public string FormUrl { get{return _FormUrl;} set{_FormUrl = value??"";} } // FormUrl
		private string _FormUrl="";
		/// <summary></summary>	
		[Description("")]
        public string FormWidth { get{return _FormWidth;} set{_FormWidth = value??"";} } // FormWidth
		private string _FormWidth="";
		/// <summary></summary>	
		[Description("")]
        public string FormHeight { get{return _FormHeight;} set{_FormHeight = value??"";} } // FormHeight
		private string _FormHeight="";
		/// <summary></summary>	
		[Description("")]
        public string FlowNameTemplete { get{return _FlowNameTemplete;} set{_FlowNameTemplete = value??"";} } // FlowNameTemplete
		private string _FlowNameTemplete="";
		/// <summary></summary>	
		[Description("")]
        public string FlowCategorytemplete { get{return _FlowCategorytemplete;} set{_FlowCategorytemplete = value??"";} } // FlowCategorytemplete
		private string _FlowCategorytemplete="";
		/// <summary></summary>	
		[Description("")]
        public string FlowSubCategoryTemplete { get{return _FlowSubCategoryTemplete;} set{_FlowSubCategoryTemplete = value??"";} } // FlowSubCategoryTemplete
		private string _FlowSubCategoryTemplete="";
		/// <summary></summary>	
		[Description("")]
        public string TaskNameTemplete { get{return _TaskNameTemplete;} set{_TaskNameTemplete = value??"";} } // TaskNameTemplete
		private string _TaskNameTemplete="";
		/// <summary></summary>	
		[Description("")]
        public string TaskCategoryTemplete { get{return _TaskCategoryTemplete;} set{_TaskCategoryTemplete = value??"";} } // TaskCategoryTemplete
		private string _TaskCategoryTemplete="";
		/// <summary></summary>	
		[Description("")]
        public string TaskSubCategoryTemplete { get{return _TaskSubCategoryTemplete;} set{_TaskSubCategoryTemplete = value??"";} } // TaskSubCategoryTemplete
		private string _TaskSubCategoryTemplete="";
		/// <summary></summary>	
		[Description("")]
        public string InitVariable { get{return _InitVariable;} set{_InitVariable = value??"";} } // InitVariable
		private string _InitVariable="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDeleteForm { get{return _AllowDeleteForm;} set{_AllowDeleteForm = value??"";} } // AllowDeleteForm
		private string _AllowDeleteForm="";
		/// <summary></summary>	
		[Description("")]
        public string SendMsgToApplicant { get{return _SendMsgToApplicant;} set{_SendMsgToApplicant = value??"";} } // SendMsgToApplicant
		private string _SendMsgToApplicant="";
		/// <summary></summary>	
		[Description("")]
        public string ViewConfig { get{return _ViewConfig;} set{_ViewConfig = value??"";} } // ViewConfig
		private string _ViewConfig="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get{return _CategoryID;} set{_CategoryID = value??"";} } // CategoryID
		private string _CategoryID="";
		/// <summary></summary>	
		[Description("")]
        public string Description { get{return _Description;} set{_Description = value??"";} } // Description
		private string _Description="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartA { get{return _FormNumberPartA;} set{_FormNumberPartA = value??"";} } // FormNumberPartA
		private string _FormNumberPartA="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartB { get{return _FormNumberPartB;} set{_FormNumberPartB = value??"";} } // FormNumberPartB
		private string _FormNumberPartB="";
		/// <summary></summary>	
		[Description("")]
        public string FormNumberPartC { get{return _FormNumberPartC;} set{_FormNumberPartC = value??"";} } // FormNumberPartC
		private string _FormNumberPartC="";
		/// <summary></summary>	
		[Description("")]
        public string MsgSendToAll { get{return _MsgSendToAll;} set{_MsgSendToAll = value??"";} } // MsgSendToAll
		private string _MsgSendToAll="";
		/// <summary></summary>	
		[Description("")]
        public string IsFreeFlow { get{return _IsFreeFlow;} set{_IsFreeFlow = value??"";} } // IsFreeFlow
		private string _IsFreeFlow="";

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_InsDefRouting> S_WF_InsDefRouting { get { onS_WF_InsDefRoutingGetting(); return _S_WF_InsDefRouting;} }
		private ICollection<S_WF_InsDefRouting> _S_WF_InsDefRouting;
		partial void onS_WF_InsDefRoutingGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_InsDefStep> S_WF_InsDefStep { get { onS_WF_InsDefStepGetting(); return _S_WF_InsDefStep;} }
		private ICollection<S_WF_InsDefStep> _S_WF_InsDefStep;
		partial void onS_WF_InsDefStepGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_InsFlow> S_WF_InsFlow { get { onS_WF_InsFlowGetting(); return _S_WF_InsFlow;} }
		private ICollection<S_WF_InsFlow> _S_WF_InsFlow;
		partial void onS_WF_InsFlowGetting();


        public S_WF_InsDefFlow()
        {
            _S_WF_InsDefRouting = new List<S_WF_InsDefRouting>();
            _S_WF_InsDefStep = new List<S_WF_InsDefStep>();
            _S_WF_InsFlow = new List<S_WF_InsFlow>();
        }
    }

	/// <summary>流程定义实例路由表</summary>	
	[Description("流程定义实例路由表")]
    public partial class S_WF_InsDefRouting : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string DefRoutingID { get{return _DefRoutingID;} set{_DefRoutingID = value??"";} } // DefRoutingID
		private string _DefRoutingID="";
		/// <summary></summary>	
		[Description("")]
        public string InsDefFlowID { get{return _InsDefFlowID;} set{_InsDefFlowID = value??"";} } // InsDefFlowID
		private string _InsDefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string InsDefStepID { get{return _InsDefStepID;} set{_InsDefStepID = value??"";} } // InsDefStepID
		private string _InsDefStepID="";
		/// <summary></summary>	
		[Description("")]
        public string EndID { get{return _EndID;} set{_EndID = value??"";} } // EndID
		private string _EndID="";
		/// <summary></summary>	
		[Description("")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary></summary>	
		[Description("")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public string Value { get{return _Value;} set{_Value = value??"";} } // Value
		private string _Value="";
		/// <summary></summary>	
		[Description("")]
        public string NotNullFields { get{return _NotNullFields;} set{_NotNullFields = value??"";} } // NotNullFields
		private string _NotNullFields="";
		/// <summary></summary>	
		[Description("")]
        public string VariableSet { get{return _VariableSet;} set{_VariableSet = value??"";} } // VariableSet
		private string _VariableSet="";
		/// <summary></summary>	
		[Description("")]
        public string FormDataSet { get{return _FormDataSet;} set{_FormDataSet = value??"";} } // FormDataSet
		private string _FormDataSet="";
		/// <summary></summary>	
		[Description("")]
        public string SaveForm { get{return _SaveForm;} set{_SaveForm = value??"";} } // SaveForm
		private string _SaveForm="";
		/// <summary></summary>	
		[Description("")]
        public string MustInputComment { get{return _MustInputComment;} set{_MustInputComment = value??"";} } // MustInputComment
		private string _MustInputComment="";
		/// <summary></summary>	
		[Description("")]
        public string SaveFormVersion { get{return _SaveFormVersion;} set{_SaveFormVersion = value??"";} } // SaveFormVersion
		private string _SaveFormVersion="";
		/// <summary></summary>	
		[Description("")]
        public string DefaultComment { get{return _DefaultComment;} set{_DefaultComment = value??"";} } // DefaultComment
		private string _DefaultComment="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureField { get{return _SignatureField;} set{_SignatureField = value??"";} } // SignatureField
		private string _SignatureField="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureProtectFields { get{return _SignatureProtectFields;} set{_SignatureProtectFields = value??"";} } // SignatureProtectFields
		private string _SignatureProtectFields="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureDivID { get{return _SignatureDivID;} set{_SignatureDivID = value??"";} } // SignatureDivID
		private string _SignatureDivID="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureCancelDivIDs { get{return _SignatureCancelDivIDs;} set{_SignatureCancelDivIDs = value??"";} } // SignatureCancelDivIDs
		private string _SignatureCancelDivIDs="";
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string AuthFormData { get{return _AuthFormData;} set{_AuthFormData = value??"";} } // AuthFormData
		private string _AuthFormData="";
		/// <summary></summary>	
		[Description("")]
        public string AuthTargetUser { get{return _AuthTargetUser;} set{_AuthTargetUser = value??"";} } // AuthTargetUser
		private string _AuthTargetUser="";
		/// <summary></summary>	
		[Description("")]
        public string AuthOrgIDs { get{return _AuthOrgIDs;} set{_AuthOrgIDs = value??"";} } // AuthOrgIDs
		private string _AuthOrgIDs="";
		/// <summary></summary>	
		[Description("")]
        public string AuthOrgNames { get{return _AuthOrgNames;} set{_AuthOrgNames = value??"";} } // AuthOrgNames
		private string _AuthOrgNames="";
		/// <summary></summary>	
		[Description("")]
        public string AuthRoleIDs { get{return _AuthRoleIDs;} set{_AuthRoleIDs = value??"";} } // AuthRoleIDs
		private string _AuthRoleIDs="";
		/// <summary></summary>	
		[Description("")]
        public string AuthRoleNames { get{return _AuthRoleNames;} set{_AuthRoleNames = value??"";} } // AuthRoleNames
		private string _AuthRoleNames="";
		/// <summary></summary>	
		[Description("")]
        public string AuthUserIDs { get{return _AuthUserIDs;} set{_AuthUserIDs = value??"";} } // AuthUserIDs
		private string _AuthUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string AuthUserNames { get{return _AuthUserNames;} set{_AuthUserNames = value??"";} } // AuthUserNames
		private string _AuthUserNames="";
		/// <summary></summary>	
		[Description("")]
        public string AuthVariable { get{return _AuthVariable;} set{_AuthVariable = value??"";} } // AuthVariable
		private string _AuthVariable="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDs { get{return _UserIDs;} set{_UserIDs = value??"";} } // UserIDs
		private string _UserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string UserNames { get{return _UserNames;} set{_UserNames = value??"";} } // UserNames
		private string _UserNames="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromStep { get{return _UserIDsFromStep;} set{_UserIDsFromStep = value??"";} } // UserIDsFromStep
		private string _UserIDsFromStep="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromStepSender { get{return _UserIDsFromStepSender;} set{_UserIDsFromStepSender = value??"";} } // UserIDsFromStepSender
		private string _UserIDsFromStepSender="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromStepExec { get{return _UserIDsFromStepExec;} set{_UserIDsFromStepExec = value??"";} } // UserIDsFromStepExec
		private string _UserIDsFromStepExec="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromField { get{return _UserIDsFromField;} set{_UserIDsFromField = value??"";} } // UserIDsFromField
		private string _UserIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsGroupFromField { get{return _UserIDsGroupFromField;} set{_UserIDsGroupFromField = value??"";} } // UserIDsGroupFromField
		private string _UserIDsGroupFromField="";
		/// <summary></summary>	
		[Description("")]
        public string RoleIDs { get{return _RoleIDs;} set{_RoleIDs = value??"";} } // RoleIDs
		private string _RoleIDs="";
		/// <summary></summary>	
		[Description("")]
        public string RoleNames { get{return _RoleNames;} set{_RoleNames = value??"";} } // RoleNames
		private string _RoleNames="";
		/// <summary></summary>	
		[Description("")]
        public string RoleIDsFromField { get{return _RoleIDsFromField;} set{_RoleIDsFromField = value??"";} } // RoleIDsFromField
		private string _RoleIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string OrgIDs { get{return _OrgIDs;} set{_OrgIDs = value??"";} } // OrgIDs
		private string _OrgIDs="";
		/// <summary></summary>	
		[Description("")]
        public string OrgNames { get{return _OrgNames;} set{_OrgNames = value??"";} } // OrgNames
		private string _OrgNames="";
		/// <summary></summary>	
		[Description("")]
        public string OrgIDFromField { get{return _OrgIDFromField;} set{_OrgIDFromField = value??"";} } // OrgIDFromField
		private string _OrgIDFromField="";
		/// <summary></summary>	
		[Description("")]
        public string OrgIDFromUser { get{return _OrgIDFromUser;} set{_OrgIDFromUser = value??"";} } // OrgIDFromUser
		private string _OrgIDFromUser="";
		/// <summary></summary>	
		[Description("")]
        public string SelectMode { get{return _SelectMode;} set{_SelectMode = value??"";} } // SelectMode
		private string _SelectMode="";
		/// <summary></summary>	
		[Description("")]
        public string SelectAgain { get{return _SelectAgain;} set{_SelectAgain = value??"";} } // SelectAgain
		private string _SelectAgain="";
		/// <summary></summary>	
		[Description("")]
        public string AllowWithdraw { get{return _AllowWithdraw;} set{_AllowWithdraw = value??"";} } // AllowWithdraw
		private string _AllowWithdraw="";
		/// <summary></summary>	
		[Description("")]
        public string Title { get{return _Title;} set{_Title = value??"";} } // Title
		private string _Title="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBack { get{return _AllowDoBack;} set{_AllowDoBack = value??"";} } // AllowDoBack
		private string _AllowDoBack="";
		/// <summary></summary>	
		[Description("")]
        public string OnlyDoBack { get{return _OnlyDoBack;} set{_OnlyDoBack = value??"";} } // OnlyDoBack
		private string _OnlyDoBack="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDs { get{return _MsgUserIDs;} set{_MsgUserIDs = value??"";} } // MsgUserIDs
		private string _MsgUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserNames { get{return _MsgUserNames;} set{_MsgUserNames = value??"";} } // MsgUserNames
		private string _MsgUserNames="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStepExec { get{return _MsgUserIDsFromStepExec;} set{_MsgUserIDsFromStepExec = value??"";} } // MsgUserIDsFromStepExec
		private string _MsgUserIDsFromStepExec="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStep { get{return _MsgUserIDsFromStep;} set{_MsgUserIDsFromStep = value??"";} } // MsgUserIDsFromStep
		private string _MsgUserIDsFromStep="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromStepSender { get{return _MsgUserIDsFromStepSender;} set{_MsgUserIDsFromStepSender = value??"";} } // MsgUserIDsFromStepSender
		private string _MsgUserIDsFromStepSender="";
		/// <summary></summary>	
		[Description("")]
        public string MsgUserIDsFromField { get{return _MsgUserIDsFromField;} set{_MsgUserIDsFromField = value??"";} } // MsgUserIDsFromField
		private string _MsgUserIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleIDs { get{return _MsgRoleIDs;} set{_MsgRoleIDs = value??"";} } // MsgRoleIDs
		private string _MsgRoleIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleIDsFromField { get{return _MsgRoleIDsFromField;} set{_MsgRoleIDsFromField = value??"";} } // MsgRoleIDsFromField
		private string _MsgRoleIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDs { get{return _MsgOrgIDs;} set{_MsgOrgIDs = value??"";} } // MsgOrgIDs
		private string _MsgOrgIDs="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDFromUser { get{return _MsgOrgIDFromUser;} set{_MsgOrgIDFromUser = value??"";} } // MsgOrgIDFromUser
		private string _MsgOrgIDFromUser="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgIDsFromField { get{return _MsgOrgIDsFromField;} set{_MsgOrgIDsFromField = value??"";} } // MsgOrgIDsFromField
		private string _MsgOrgIDsFromField="";
		/// <summary></summary>	
		[Description("")]
        public string MsgTmpl { get{return _MsgTmpl;} set{_MsgTmpl = value??"";} } // MsgTmpl
		private string _MsgTmpl="";
		/// <summary></summary>	
		[Description("")]
        public string MsgType { get{return _MsgType;} set{_MsgType = value??"";} } // MsgType
		private string _MsgType="";
		/// <summary></summary>	
		[Description("")]
        public string MsgSendToTaskUser { get{return _MsgSendToTaskUser;} set{_MsgSendToTaskUser = value??"";} } // MsgSendToTaskUser
		private string _MsgSendToTaskUser="";
		/// <summary></summary>	
		[Description("")]
        public string DenyAutoPass { get{return _DenyAutoPass;} set{_DenyAutoPass = value??"";} } // DenyAutoPass
		private string _DenyAutoPass="";
		/// <summary></summary>	
		[Description("")]
        public string ExcludeUser { get{return _ExcludeUser;} set{_ExcludeUser = value??"";} } // ExcludeUser
		private string _ExcludeUser="";
		/// <summary></summary>	
		[Description("")]
        public string MsgRoleNames { get{return _MsgRoleNames;} set{_MsgRoleNames = value??"";} } // MsgRoleNames
		private string _MsgRoleNames="";
		/// <summary></summary>	
		[Description("")]
        public string MsgOrgNames { get{return _MsgOrgNames;} set{_MsgOrgNames = value??"";} } // MsgOrgNames
		private string _MsgOrgNames="";
		/// <summary>需要输入签字密码</summary>	
		[Description("需要输入签字密码")]
        public string InputSignPwd { get{return _InputSignPwd;} set{_InputSignPwd = value??"";} } // InputSignPwd
		private string _InputSignPwd="";
		/// <summary></summary>	
		[Description("")]
        public string AuthFromSql { get{return _AuthFromSql;} set{_AuthFromSql = value??"";} } // AuthFromSql
		private string _AuthFromSql="";
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromSql { get{return _UserIDsFromSql;} set{_UserIDsFromSql = value??"";} } // UserIDsFromSql
		private string _UserIDsFromSql="";
		/// <summary></summary>	
		[Description("")]
        public string SignatureType { get{return _SignatureType;} set{_SignatureType = value??"";} } // SignatureType
		private string _SignatureType="";
		/// <summary></summary>	
		[Description("")]
        public string ExecLogic { get{return _ExecLogic;} set{_ExecLogic = value??"";} } // ExecLogic
		private string _ExecLogic="";
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get{return _NameEN;} set{_NameEN = value??"";} } // NameEN
		private string _NameEN="";

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsDefFlow S_WF_InsDefFlow { get; set; } //  InsDefFlowID - FK_S_WF_InsDefRouting_S_WF_InsDefFlow
		[JsonIgnore]
        public virtual S_WF_InsDefStep S_WF_InsDefStep { get; set; } //  InsDefStepID - FK_S_WF_InsDefRouting_S_WF_InsDefStep
    }

	/// <summary>流程定义实例环节表</summary>	
	[Description("流程定义实例环节表")]
    public partial class S_WF_InsDefStep : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string DefStepID { get{return _DefStepID;} set{_DefStepID = value??"";} } // DefStepID
		private string _DefStepID="";
		/// <summary></summary>	
		[Description("")]
        public string InsDefFlowID { get{return _InsDefFlowID;} set{_InsDefFlowID = value??"";} } // InsDefFlowID
		private string _InsDefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string SubFormID { get{return _SubFormID;} set{_SubFormID = value??"";} } // SubFormID
		private string _SubFormID="";
		/// <summary></summary>	
		[Description("")]
        public string Code { get{return _Code;} set{_Code = value??"";} } // Code
		private string _Code="";
		/// <summary></summary>	
		[Description("")]
        public string Name { get{return _Name;} set{_Name = value??"";} } // Name
		private string _Name="";
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string AllowDelegate { get{return _AllowDelegate;} set{_AllowDelegate = value??"";} } // AllowDelegate
		private string _AllowDelegate="";
		/// <summary></summary>	
		[Description("")]
        public string AllowCirculate { get{return _AllowCirculate;} set{_AllowCirculate = value??"";} } // AllowCirculate
		private string _AllowCirculate="";
		/// <summary></summary>	
		[Description("")]
        public string AllowAsk { get{return _AllowAsk;} set{_AllowAsk = value??"";} } // AllowAsk
		private string _AllowAsk="";
		/// <summary></summary>	
		[Description("")]
        public string AllowSave { get{return _AllowSave;} set{_AllowSave = value??"";} } // AllowSave
		private string _AllowSave="";
		/// <summary></summary>	
		[Description("")]
        public string SaveVariableAuth { get{return _SaveVariableAuth;} set{_SaveVariableAuth = value??"";} } // SaveVariableAuth
		private string _SaveVariableAuth="";
		/// <summary></summary>	
		[Description("")]
        public string SubFlowCode { get{return _SubFlowCode;} set{_SubFlowCode = value??"";} } // SubFlowCode
		private string _SubFlowCode="";
		/// <summary></summary>	
		[Description("")]
        public string WaitingStepIDs { get{return _WaitingStepIDs;} set{_WaitingStepIDs = value??"";} } // WaitingStepIDs
		private string _WaitingStepIDs="";
		/// <summary></summary>	
		[Description("")]
        public string CooperationMode { get{return _CooperationMode;} set{_CooperationMode = value??"";} } // CooperationMode
		private string _CooperationMode="";
		/// <summary></summary>	
		[Description("")]
        public string Phase { get{return _Phase;} set{_Phase = value??"";} } // Phase
		private string _Phase="";
		/// <summary></summary>	
		[Description("")]
        public string HiddenElements { get{return _HiddenElements;} set{_HiddenElements = value??"";} } // HiddenElements
		private string _HiddenElements="";
		/// <summary></summary>	
		[Description("")]
        public string VisibleElements { get{return _VisibleElements;} set{_VisibleElements = value??"";} } // VisibleElements
		private string _VisibleElements="";
		/// <summary></summary>	
		[Description("")]
        public string EditableElements { get{return _EditableElements;} set{_EditableElements = value??"";} } // EditableElements
		private string _EditableElements="";
		/// <summary></summary>	
		[Description("")]
        public string DisableElements { get{return _DisableElements;} set{_DisableElements = value??"";} } // DisableElements
		private string _DisableElements="";
		/// <summary></summary>	
		[Description("")]
        public string Urgency { get{return _Urgency;} set{_Urgency = value??"";} } // Urgency
		private string _Urgency="";
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutAutoPass { get; set; } // TimeoutAutoPass
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutNotice { get; set; } // TimeoutNotice
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutAlarm { get; set; } // TimeoutAlarm
		/// <summary></summary>	
		[Description("")]
        public int? TimeoutDeadline { get; set; } // TimeoutDeadline
		/// <summary></summary>	
		[Description("")]
        public string KeepWhenEnd { get{return _KeepWhenEnd;} set{_KeepWhenEnd = value??"";} } // KeepWhenEnd
		private string _KeepWhenEnd="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBackFirst { get{return _AllowDoBackFirst;} set{_AllowDoBackFirst = value??"";} } // AllowDoBackFirst
		private string _AllowDoBackFirst="";
		/// <summary></summary>	
		[Description("")]
        public string AllowDoBackFirstReturn { get{return _AllowDoBackFirstReturn;} set{_AllowDoBackFirstReturn = value??"";} } // AllowDoBackFirstReturn
		private string _AllowDoBackFirstReturn="";
		/// <summary></summary>	
		[Description("")]
        public string DoBackSignField { get{return _DoBackSignField;} set{_DoBackSignField = value??"";} } // DoBackSignField
		private string _DoBackSignField="";
		/// <summary></summary>	
		[Description("")]
        public string HideAdvice { get{return _HideAdvice;} set{_HideAdvice = value??"";} } // HideAdvice
		private string _HideAdvice="";
		/// <summary></summary>	
		[Description("")]
        public string EmptyToStep { get{return _EmptyToStep;} set{_EmptyToStep = value??"";} } // EmptyToStep
		private string _EmptyToStep="";
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get{return _NameEN;} set{_NameEN = value??"";} } // NameEN
		private string _NameEN="";
		/// <summary></summary>	
		[Description("")]
        public string DoBackButtonText { get{return _DoBackButtonText;} set{_DoBackButtonText = value??"";} } // DoBackButtonText
		private string _DoBackButtonText="";
		/// <summary>打回和打回首环节是否保存表单</summary>	
		[Description("打回和打回首环节是否保存表单")]
        public string DoBackSave { get{return _DoBackSave;} set{_DoBackSave = value??"";} } // DoBackSave
		private string _DoBackSave="";

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_InsDefRouting> S_WF_InsDefRouting { get { onS_WF_InsDefRoutingGetting(); return _S_WF_InsDefRouting;} }
		private ICollection<S_WF_InsDefRouting> _S_WF_InsDefRouting;
		partial void onS_WF_InsDefRoutingGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_InsTask> S_WF_InsTask { get { onS_WF_InsTaskGetting(); return _S_WF_InsTask;} }
		private ICollection<S_WF_InsTask> _S_WF_InsTask;
		partial void onS_WF_InsTaskGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsDefFlow S_WF_InsDefFlow { get; set; } //  InsDefFlowID - FK_S_WF_InsDefStep_S_WF_InsDefFlow

        public S_WF_InsDefStep()
        {
            _S_WF_InsDefRouting = new List<S_WF_InsDefRouting>();
            _S_WF_InsTask = new List<S_WF_InsTask>();
        }
    }

	/// <summary>流程表</summary>	
	[Description("流程表")]
    public partial class S_WF_InsFlow : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string InsDefFlowID { get{return _InsDefFlowID;} set{_InsDefFlowID = value??"";} } // InsDefFlowID
		private string _InsDefFlowID="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get{return _CreateUserID;} set{_CreateUserID = value??"";} } // CreateUserID
		private string _CreateUserID="";
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get{return _CreateUserName;} set{_CreateUserName = value??"";} } // CreateUserName
		private string _CreateUserName="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CompleteTime { get; set; } // CompleteTime
		/// <summary></summary>	
		[Description("")]
        public string FormInstanceID { get{return _FormInstanceID;} set{_FormInstanceID = value??"";} } // FormInstanceID
		private string _FormInstanceID="";
		/// <summary></summary>	
		[Description("")]
        public string FlowName { get{return _FlowName;} set{_FlowName = value??"";} } // FlowName
		private string _FlowName="";
		/// <summary></summary>	
		[Description("")]
        public string FlowCategory { get{return _FlowCategory;} set{_FlowCategory = value??"";} } // FlowCategory
		private string _FlowCategory="";
		/// <summary></summary>	
		[Description("")]
        public string FlowSubCategory { get{return _FlowSubCategory;} set{_FlowSubCategory = value??"";} } // FlowSubCategory
		private string _FlowSubCategory="";
		/// <summary></summary>	
		[Description("")]
        public string Status { get{return _Status;} set{_Status = value??"";} } // Status
		private string _Status="";
		/// <summary></summary>	
		[Description("")]
        public string FatherTaskID { get{return _FatherTaskID;} set{_FatherTaskID = value??"";} } // FatherTaskID
		private string _FatherTaskID="";
		/// <summary></summary>	
		[Description("")]
        public double? TimeConsuming { get; set; } // TimeConsuming
		/// <summary></summary>	
		[Description("")]
        public string CurrentStep { get{return _CurrentStep;} set{_CurrentStep = value??"";} } // CurrentStep
		private string _CurrentStep="";
		/// <summary></summary>	
		[Description("")]
        public string CurrentUserNames { get{return _CurrentUserNames;} set{_CurrentUserNames = value??"";} } // CurrentUserNames
		private string _CurrentUserNames="";

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_InsTask> S_WF_InsTask { get { onS_WF_InsTaskGetting(); return _S_WF_InsTask;} }
		private ICollection<S_WF_InsTask> _S_WF_InsTask;
		partial void onS_WF_InsTaskGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_InsTaskExec> S_WF_InsTaskExec { get { onS_WF_InsTaskExecGetting(); return _S_WF_InsTaskExec;} }
		private ICollection<S_WF_InsTaskExec> _S_WF_InsTaskExec;
		partial void onS_WF_InsTaskExecGetting();

		[JsonIgnore]
        public virtual ICollection<S_WF_InsVariable> S_WF_InsVariable { get { onS_WF_InsVariableGetting(); return _S_WF_InsVariable;} }
		private ICollection<S_WF_InsVariable> _S_WF_InsVariable;
		partial void onS_WF_InsVariableGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsDefFlow S_WF_InsDefFlow { get; set; } //  InsDefFlowID - FK_S_WF_InsFlow_S_WF_InsDefFlow

        public S_WF_InsFlow()
        {
            _S_WF_InsTask = new List<S_WF_InsTask>();
            _S_WF_InsTaskExec = new List<S_WF_InsTaskExec>();
            _S_WF_InsVariable = new List<S_WF_InsVariable>();
        }
    }

	/// <summary>任务表</summary>	
	[Description("任务表")]
    public partial class S_WF_InsTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string InsDefStepID { get{return _InsDefStepID;} set{_InsDefStepID = value??"";} } // InsDefStepID
		private string _InsDefStepID="";
		/// <summary></summary>	
		[Description("")]
        public string InsFlowID { get{return _InsFlowID;} set{_InsFlowID = value??"";} } // InsFlowID
		private string _InsFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string TaskName { get{return _TaskName;} set{_TaskName = value??"";} } // TaskName
		private string _TaskName="";
		/// <summary></summary>	
		[Description("")]
        public string TaskCategory { get{return _TaskCategory;} set{_TaskCategory = value??"";} } // TaskCategory
		private string _TaskCategory="";
		/// <summary></summary>	
		[Description("")]
        public string TaskSubCategory { get{return _TaskSubCategory;} set{_TaskSubCategory = value??"";} } // TaskSubCategory
		private string _TaskSubCategory="";
		/// <summary></summary>	
		[Description("")]
        public string SendTaskIDs { get{return _SendTaskIDs;} set{_SendTaskIDs = value??"";} } // SendTaskIDs
		private string _SendTaskIDs="";
		/// <summary></summary>	
		[Description("")]
        public string SendTaskUserIDs { get{return _SendTaskUserIDs;} set{_SendTaskUserIDs = value??"";} } // SendTaskUserIDs
		private string _SendTaskUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string SendTaskUserNames { get{return _SendTaskUserNames;} set{_SendTaskUserNames = value??"";} } // SendTaskUserNames
		private string _SendTaskUserNames="";
		/// <summary></summary>	
		[Description("")]
        public string TaskUserIDs { get{return _TaskUserIDs;} set{_TaskUserIDs = value??"";} } // TaskUserIDs
		private string _TaskUserIDs="";
		/// <summary></summary>	
		[Description("")]
        public string TaskUserNames { get{return _TaskUserNames;} set{_TaskUserNames = value??"";} } // TaskUserNames
		private string _TaskUserNames="";
		/// <summary></summary>	
		[Description("")]
        public string TaskUserIDsGroup { get{return _TaskUserIDsGroup;} set{_TaskUserIDsGroup = value??"";} } // TaskUserIDsGroup
		private string _TaskUserIDsGroup="";
		/// <summary></summary>	
		[Description("")]
        public string TaskRoleIDs { get{return _TaskRoleIDs;} set{_TaskRoleIDs = value??"";} } // TaskRoleIDs
		private string _TaskRoleIDs="";
		/// <summary></summary>	
		[Description("")]
        public string TaskOrgIDs { get{return _TaskOrgIDs;} set{_TaskOrgIDs = value??"";} } // TaskOrgIDs
		private string _TaskOrgIDs="";
		/// <summary></summary>	
		[Description("")]
        public string WaitingRoutings { get{return _WaitingRoutings;} set{_WaitingRoutings = value??"";} } // WaitingRoutings
		private string _WaitingRoutings="";
		/// <summary></summary>	
		[Description("")]
        public string WaitingSteps { get{return _WaitingSteps;} set{_WaitingSteps = value??"";} } // WaitingSteps
		private string _WaitingSteps="";
		/// <summary></summary>	
		[Description("")]
        public string ChildFlowID { get{return _ChildFlowID;} set{_ChildFlowID = value??"";} } // ChildFlowID
		private string _ChildFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string Status { get{return _Status;} set{_Status = value??"";} } // Status
		private string _Status="";
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? CompleteTime { get; set; } // CompleteTime
		/// <summary></summary>	
		[Description("")]
        public string FormVersion { get{return _FormVersion;} set{_FormVersion = value??"";} } // FormVersion
		private string _FormVersion="";
		/// <summary></summary>	
		[Description("")]
        public string Urgency { get{return _Urgency;} set{_Urgency = value??"";} } // Urgency
		private string _Urgency="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? FirstViewTime { get; set; } // FirstViewTime
		/// <summary></summary>	
		[Description("")]
        public string VariableVersion { get{return _VariableVersion;} set{_VariableVersion = value??"";} } // VariableVersion
		private string _VariableVersion="";
		/// <summary></summary>	
		[Description("")]
        public string DoBackRoutingID { get{return _DoBackRoutingID;} set{_DoBackRoutingID = value??"";} } // DoBackRoutingID
		private string _DoBackRoutingID="";
		/// <summary></summary>	
		[Description("")]
        public string OnlyDoBack { get{return _OnlyDoBack;} set{_OnlyDoBack = value??"";} } // OnlyDoBack
		private string _OnlyDoBack="";
		/// <summary></summary>	
		[Description("")]
        public double? TimeConsuming { get; set; } // TimeConsuming
		/// <summary></summary>	
		[Description("")]
        public int? GoToUp { get; set; } // GoToUp

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_WF_InsTaskExec> S_WF_InsTaskExec { get { onS_WF_InsTaskExecGetting(); return _S_WF_InsTaskExec;} }
		private ICollection<S_WF_InsTaskExec> _S_WF_InsTaskExec;
		partial void onS_WF_InsTaskExecGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsDefStep S_WF_InsDefStep { get; set; } //  InsDefStepID - FK_S_WF_InsTask_S_WF_InsDefStep
		[JsonIgnore]
        public virtual S_WF_InsFlow S_WF_InsFlow { get; set; } //  InsFlowID - FK_S_WF_InsTask_S_WF_InsFlow1

        public S_WF_InsTask()
        {
			TaskUserIDs = "";
			TaskUserNames = "";
			TaskUserIDsGroup = "";
			WaitingRoutings = "";
			WaitingSteps = "";
			Status = "";
            _S_WF_InsTaskExec = new List<S_WF_InsTaskExec>();
        }
    }

	/// <summary>任务执行明细表</summary>	
	[Description("任务执行明细表")]
    public partial class S_WF_InsTaskExec : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string InsFlowID { get{return _InsFlowID;} set{_InsFlowID = value??"";} } // InsFlowID
		private string _InsFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string InsTaskID { get{return _InsTaskID;} set{_InsTaskID = value??"";} } // InsTaskID
		private string _InsTaskID="";
		/// <summary></summary>	
		[Description("")]
        public string ExecRoutingIDs { get{return _ExecRoutingIDs;} set{_ExecRoutingIDs = value??"";} } // ExecRoutingIDs
		private string _ExecRoutingIDs="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string TaskUserID { get{return _TaskUserID;} set{_TaskUserID = value??"";} } // TaskUserID
		private string _TaskUserID="";
		/// <summary></summary>	
		[Description("")]
        public string TaskUserName { get{return _TaskUserName;} set{_TaskUserName = value??"";} } // TaskUserName
		private string _TaskUserName="";
		/// <summary></summary>	
		[Description("")]
        public string ExecUserID { get{return _ExecUserID;} set{_ExecUserID = value??"";} } // ExecUserID
		private string _ExecUserID="";
		/// <summary></summary>	
		[Description("")]
        public string ExecUserName { get{return _ExecUserName;} set{_ExecUserName = value??"";} } // ExecUserName
		private string _ExecUserName="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? ExecTime { get; set; } // ExecTime
		/// <summary></summary>	
		[Description("")]
        public string ExecComment { get{return _ExecComment;} set{_ExecComment = value??"";} } // ExecComment
		private string _ExecComment="";
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? FirstViewTime { get; set; } // FirstViewTime
		/// <summary></summary>	
		[Description("")]
        public string TimeoutAutoPassResult { get{return _TimeoutAutoPassResult;} set{_TimeoutAutoPassResult = value??"";} } // TimeoutAutoPassResult
		private string _TimeoutAutoPassResult="";
		/// <summary></summary>	
		[Description("")]
        public string TimeoutNoticeResult { get{return _TimeoutNoticeResult;} set{_TimeoutNoticeResult = value??"";} } // TimeoutNoticeResult
		private string _TimeoutNoticeResult="";
		/// <summary></summary>	
		[Description("")]
        public string TimeoutAlarmResult { get{return _TimeoutAlarmResult;} set{_TimeoutAlarmResult = value??"";} } // TimeoutAlarmResult
		private string _TimeoutAlarmResult="";
		/// <summary></summary>	
		[Description("")]
        public string TimeoutDeadlineResult { get{return _TimeoutDeadlineResult;} set{_TimeoutDeadlineResult = value??"";} } // TimeoutDeadlineResult
		private string _TimeoutDeadlineResult="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? TimeoutAutoPass { get; set; } // TimeoutAutoPass
		/// <summary></summary>	
		[Description("")]
        public DateTime? TimeoutNotice { get; set; } // TimeoutNotice
		/// <summary></summary>	
		[Description("")]
        public DateTime? TimeoutAlarm { get; set; } // TimeoutAlarm
		/// <summary></summary>	
		[Description("")]
        public DateTime? TimeoutDeadline { get; set; } // TimeoutDeadline
		/// <summary></summary>	
		[Description("")]
        public double? TimeConsuming { get; set; } // TimeConsuming
		/// <summary>已执行的弱控路由</summary>	
		[Description("已执行的弱控路由")]
        public string WeakedRoutingIDs { get{return _WeakedRoutingIDs;} set{_WeakedRoutingIDs = value??"";} } // WeakedRoutingIDs
		private string _WeakedRoutingIDs="";
		/// <summary></summary>	
		[Description("")]
        public string ExecRoutingName { get{return _ExecRoutingName;} set{_ExecRoutingName = value??"";} } // ExecRoutingName
		private string _ExecRoutingName="";
		/// <summary></summary>	
		[Description("")]
        public string ApprovalInMobile { get{return _ApprovalInMobile;} set{_ApprovalInMobile = value??"";} } // ApprovalInMobile
		private string _ApprovalInMobile="";

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsFlow S_WF_InsFlow { get; set; } //  InsFlowID - FK_S_WF_InsTaskExec_S_WF_InsFlow1
		[JsonIgnore]
        public virtual S_WF_InsTask S_WF_InsTask { get; set; } //  InsTaskID - FK_S_WF_InsTaskExec_S_WF_InsTask1

        public S_WF_InsTaskExec()
        {
			ExecRoutingIDs = "";
        }
    }

	/// <summary>流程变量表</summary>	
	[Description("流程变量表")]
    public partial class S_WF_InsVariable : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID (Primary key)
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string InsFlowID { get{return _InsFlowID;} set{_InsFlowID = value??"";} } // InsFlowID
		private string _InsFlowID="";
		/// <summary></summary>	
		[Description("")]
        public string VariableName { get{return _VariableName;} set{_VariableName = value??"";} } // VariableName
		private string _VariableName="";
		/// <summary></summary>	
		[Description("")]
        public string VariableValue { get{return _VariableValue;} set{_VariableValue = value??"";} } // VariableValue
		private string _VariableValue="";
		/// <summary></summary>	
		[Description("")]
        public string VariablePreValue { get{return _VariablePreValue;} set{_VariablePreValue = value??"";} } // VariablePreValue
		private string _VariablePreValue="";

        // Foreign keys
		[JsonIgnore]
        public virtual S_WF_InsFlow S_WF_InsFlow { get; set; } //  InsFlowID - FK_S_WF_InsVariable_S_WF_InsFlow
    }

	/// <summary></summary>	
	[Description("")]
    public partial class Task : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string Type { get{return _Type;} set{_Type = value??"";} } // Type
		private string _Type="";
		/// <summary></summary>	
		[Description("")]
        public string TaskExecID { get{return _TaskExecID;} set{_TaskExecID = value??"";} } // TaskExecID
		private string _TaskExecID="";
		/// <summary></summary>	
		[Description("")]
        public string ID { get{return _ID;} set{_ID = value??"";} } // ID
		private string _ID="";
		/// <summary></summary>	
		[Description("")]
        public string TaskID { get{return _TaskID;} set{_TaskID = value??"";} } // TaskID
		private string _TaskID="";
		/// <summary></summary>	
		[Description("")]
        public string StepID { get{return _StepID;} set{_StepID = value??"";} } // StepID
		private string _StepID="";
		/// <summary></summary>	
		[Description("")]
        public string FlowID { get{return _FlowID;} set{_FlowID = value??"";} } // FlowID
		private string _FlowID="";
		/// <summary></summary>	
		[Description("")]
        public string FlowName { get{return _FlowName;} set{_FlowName = value??"";} } // FlowName
		private string _FlowName="";
		/// <summary></summary>	
		[Description("")]
        public string DefStepName { get{return _DefStepName;} set{_DefStepName = value??"";} } // DefStepName
		private string _DefStepName="";
		/// <summary></summary>	
		[Description("")]
        public string TaskName { get{return _TaskName;} set{_TaskName = value??"";} } // TaskName
		private string _TaskName="";
		/// <summary></summary>	
		[Description("")]
        public string FormUrl { get{return _FormUrl;} set{_FormUrl = value??"";} } // FormUrl
		private string _FormUrl="";
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get{return _CreateUser;} set{_CreateUser = value??"";} } // CreateUser
		private string _CreateUser="";
		/// <summary></summary>	
		[Description("")]
        public DateTime? LaunchTime { get; set; } // LaunchTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ReceiveTime { get; set; } // ReceiveTime
    }


    // ************************************************************************
    // POCO Configuration

    // S_E_RTXSynchNoticeAndTask
    internal partial class S_E_RTXSynchNoticeAndTaskConfiguration : EntityTypeConfiguration<S_E_RTXSynchNoticeAndTask>
    {
        public S_E_RTXSynchNoticeAndTaskConfiguration()
        {
			ToTable("S_E_RTXSYNCHNOTICEANDTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TaskExecIDOrMsgID).HasColumnName("TASKEXECIDORMSGID").IsOptional().HasMaxLength(50);
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(2000);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional().HasMaxLength(2000);
            Property(x => x.LinkURL).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.OwnerUserID).HasColumnName("OWNERUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.OwnerUserName).HasColumnName("OWNERUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SendUserIDs).HasColumnName("SENDUSERIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.SendUserNames).HasColumnName("SENDUSERNAMES").IsOptional().HasMaxLength(2000);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.InsFlowID).HasColumnName("INSFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.FormID).HasColumnName("FORMID").IsOptional().HasMaxLength(50);
            Property(x => x.TaskType).HasColumnName("TASKTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.DataType).HasColumnName("DATATYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ExecTime).HasColumnName("EXECTIME").IsOptional();
            Property(x => x.ErrorMsg).HasColumnName("ERRORMSG").IsOptional().HasMaxLength(2000);
        }
    }

    // S_E_TxtOutTaskExec
    internal partial class S_E_TxtOutTaskExecConfiguration : EntityTypeConfiguration<S_E_TxtOutTaskExec>
    {
        public S_E_TxtOutTaskExecConfiguration()
        {
			ToTable("S_E_TXTOUTTASKEXEC");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.InsTaskExecID).HasColumnName("INSTASKEXECID").IsOptional().HasMaxLength(50);
            Property(x => x.InsFlowID).HasColumnName("INSFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.OutProcessCode).HasColumnName("OUTPROCESSCODE").IsOptional().HasMaxLength(50);
            Property(x => x.OutProcessInstanceId).HasColumnName("OUTPROCESSINSTANCEID").IsOptional().HasMaxLength(50);
            Property(x => x.OutTaskId).HasColumnName("OUTTASKID").IsOptional().HasMaxLength(50);
            Property(x => x.OutMsg).HasColumnName("OUTMSG").IsOptional().HasMaxLength(2147483647);
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUid).HasColumnName("CREATEUID").IsOptional().HasMaxLength(50);
        }
    }

    // S_E_TxtOutTaskExecLog
    internal partial class S_E_TxtOutTaskExecLogConfiguration : EntityTypeConfiguration<S_E_TxtOutTaskExecLog>
    {
        public S_E_TxtOutTaskExecLogConfiguration()
        {
			ToTable("S_E_TXTOUTTASKEXECLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.InsTaskExecID).HasColumnName("INSTASKEXECID").IsOptional().HasMaxLength(50);
            Property(x => x.OutMsg).HasColumnName("OUTMSG").IsOptional().HasMaxLength(2147483647);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
        }
    }

    // S_WF_DefDelegate
    internal partial class S_WF_DefDelegateConfiguration : EntityTypeConfiguration<S_WF_DefDelegate>
    {
        public S_WF_DefDelegateConfiguration()
        {
			ToTable("S_WF_DEFDELEGATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefFlowID).HasColumnName("DEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateUserID).HasColumnName("DELEGATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateUserName).HasColumnName("DELEGATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateRoleID).HasColumnName("DELEGATEROLEID").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateRoleName).HasColumnName("DELEGATEROLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateOrgID).HasColumnName("DELEGATEORGID").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateOrgName).HasColumnName("DELEGATEORGNAME").IsOptional().HasMaxLength(50);
            Property(x => x.BeDelegateUserID).HasColumnName("BEDELEGATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.BeDelegateUserName).HasColumnName("BEDELEGATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DelegateTime).HasColumnName("DELEGATETIME").IsOptional();
            Property(x => x.BeginTime).HasColumnName("BEGINTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_WF_DefFlow).WithMany(b => b.S_WF_DefDelegate).HasForeignKey(c => c.DefFlowID); // FK_S_WF_DefDelegate_S_WF_DefFlow
        }
    }

    // S_WF_DefFlow
    internal partial class S_WF_DefFlowConfiguration : EntityTypeConfiguration<S_WF_DefFlow>
    {
        public S_WF_DefFlowConfiguration()
        {
			ToTable("S_WF_DEFFLOW");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.FormUrl).HasColumnName("FORMURL").IsOptional().HasMaxLength(200);
            Property(x => x.FormWidth).HasColumnName("FORMWIDTH").IsOptional().HasMaxLength(50);
            Property(x => x.FormHeight).HasColumnName("FORMHEIGHT").IsOptional().HasMaxLength(50);
            Property(x => x.FlowNameTemplete).HasColumnName("FLOWNAMETEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.FlowCategorytemplete).HasColumnName("FLOWCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.FlowSubCategoryTemplete).HasColumnName("FLOWSUBCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskNameTemplete).HasColumnName("TASKNAMETEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskCategoryTemplete).HasColumnName("TASKCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskSubCategoryTemplete).HasColumnName("TASKSUBCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.InitVariable).HasColumnName("INITVARIABLE").IsOptional().HasMaxLength(500);
            Property(x => x.AllowDeleteForm).HasColumnName("ALLOWDELETEFORM").IsOptional().HasMaxLength(1);
            Property(x => x.SendMsgToApplicant).HasColumnName("SENDMSGTOAPPLICANT").IsOptional().HasMaxLength(1);
            Property(x => x.ViewConfig).HasColumnName("VIEWCONFIG").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.FormNumberPartA).HasColumnName("FORMNUMBERPARTA").IsOptional().HasMaxLength(50);
            Property(x => x.FormNumberPartB).HasColumnName("FORMNUMBERPARTB").IsOptional().HasMaxLength(50);
            Property(x => x.FormNumberPartC).HasColumnName("FORMNUMBERPARTC").IsOptional().HasMaxLength(50);
            Property(x => x.AlreadyReleased).HasColumnName("ALREADYRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.MsgSendToAll).HasColumnName("MSGSENDTOALL").IsOptional().HasMaxLength(1);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.isFlowChart).HasColumnName("ISFLOWCHART").IsOptional().HasMaxLength(1);
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(500);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(500);
        }
    }

    // S_WF_DefRouting
    internal partial class S_WF_DefRoutingConfiguration : EntityTypeConfiguration<S_WF_DefRouting>
    {
        public S_WF_DefRoutingConfiguration()
        {
			ToTable("S_WF_DEFROUTING");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefFlowID).HasColumnName("DEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.DefStepID).HasColumnName("DEFSTEPID").IsOptional().HasMaxLength(50);
            Property(x => x.EndID).HasColumnName("ENDID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Value).HasColumnName("VALUE").IsOptional().HasMaxLength(50);
            Property(x => x.NotNullFields).HasColumnName("NOTNULLFIELDS").IsOptional().HasMaxLength(2000);
            Property(x => x.VariableSet).HasColumnName("VARIABLESET").IsOptional().HasMaxLength(500);
            Property(x => x.FormDataSet).HasColumnName("FORMDATASET").IsOptional().HasMaxLength(2000);
            Property(x => x.SaveForm).HasColumnName("SAVEFORM").IsOptional().HasMaxLength(1);
            Property(x => x.MustInputComment).HasColumnName("MUSTINPUTCOMMENT").IsOptional().HasMaxLength(1);
            Property(x => x.SaveFormVersion).HasColumnName("SAVEFORMVERSION").IsOptional().HasMaxLength(1);
            Property(x => x.DefaultComment).HasColumnName("DEFAULTCOMMENT").IsOptional().HasMaxLength(50);
            Property(x => x.SignatureField).HasColumnName("SIGNATUREFIELD").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureProtectFields).HasColumnName("SIGNATUREPROTECTFIELDS").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureDivID).HasColumnName("SIGNATUREDIVID").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureCancelDivIDs).HasColumnName("SIGNATURECANCELDIVIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.AuthFormData).HasColumnName("AUTHFORMDATA").IsOptional().HasMaxLength(500);
            Property(x => x.AuthTargetUser).HasColumnName("AUTHTARGETUSER").IsOptional().HasMaxLength(500);
            Property(x => x.AuthOrgIDs).HasColumnName("AUTHORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthOrgNames).HasColumnName("AUTHORGNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthRoleIDs).HasColumnName("AUTHROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthRoleNames).HasColumnName("AUTHROLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthUserIDs).HasColumnName("AUTHUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthUserNames).HasColumnName("AUTHUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthVariable).HasColumnName("AUTHVARIABLE").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDs).HasColumnName("USERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.UserNames).HasColumnName("USERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDsFromStep).HasColumnName("USERIDSFROMSTEP").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromStepSender).HasColumnName("USERIDSFROMSTEPSENDER").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromStepExec).HasColumnName("USERIDSFROMSTEPEXEC").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromField).HasColumnName("USERIDSFROMFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDsGroupFromField).HasColumnName("USERIDSGROUPFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.RoleIDs).HasColumnName("ROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.RoleNames).HasColumnName("ROLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.RoleIDsFromField).HasColumnName("ROLEIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.OrgIDs).HasColumnName("ORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.OrgNames).HasColumnName("ORGNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.OrgIDFromField).HasColumnName("ORGIDFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.OrgIDFromUser).HasColumnName("ORGIDFROMUSER").IsOptional().HasMaxLength(50);
            Property(x => x.SelectMode).HasColumnName("SELECTMODE").IsOptional().HasMaxLength(50);
            Property(x => x.SelectAgain).HasColumnName("SELECTAGAIN").IsOptional().HasMaxLength(1);
            Property(x => x.AllowWithdraw).HasColumnName("ALLOWWITHDRAW").IsOptional().HasMaxLength(1);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(50);
            Property(x => x.AllowDoBack).HasColumnName("ALLOWDOBACK").IsOptional().HasMaxLength(1);
            Property(x => x.OnlyDoBack).HasColumnName("ONLYDOBACK").IsOptional().HasMaxLength(1);
            Property(x => x.MsgUserIDs).HasColumnName("MSGUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserNames).HasColumnName("MSGUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStepExec).HasColumnName("MSGUSERIDSFROMSTEPEXEC").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStep).HasColumnName("MSGUSERIDSFROMSTEP").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStepSender).HasColumnName("MSGUSERIDSFROMSTEPSENDER").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromField).HasColumnName("MSGUSERIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgRoleIDs).HasColumnName("MSGROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgRoleIDsFromField).HasColumnName("MSGROLEIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgOrgIDs).HasColumnName("MSGORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgOrgIDFromUser).HasColumnName("MSGORGIDFROMUSER").IsOptional().HasMaxLength(500);
            Property(x => x.MsgOrgIDsFromField).HasColumnName("MSGORGIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgTmpl).HasColumnName("MSGTMPL").IsOptional().HasMaxLength(500);
            Property(x => x.MsgType).HasColumnName("MSGTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.MsgSendToTaskUser).HasColumnName("MSGSENDTOTASKUSER").IsOptional().HasMaxLength(1);
            Property(x => x.DenyAutoPass).HasColumnName("DENYAUTOPASS").IsOptional().HasMaxLength(1);
            Property(x => x.ExcludeUser).HasColumnName("EXCLUDEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.MsgRoleNames).HasColumnName("MSGROLENAMES").IsOptional().HasMaxLength(50);
            Property(x => x.MsgOrgNames).HasColumnName("MSGORGNAMES").IsOptional().HasMaxLength(50);
            Property(x => x.InputSignPwd).HasColumnName("INPUTSIGNPWD").IsOptional().HasMaxLength(50);
            Property(x => x.AuthFromSql).HasColumnName("AUTHFROMSQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.UserIDsFromSql).HasColumnName("USERIDSFROMSQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.AuthFromSqlMemo).HasColumnName("AUTHFROMSQLMEMO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.SignatureType).HasColumnName("SIGNATURETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ExecLogic).HasColumnName("EXECLOGIC").IsOptional().HasMaxLength(1073741823);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.S_WF_DefFlow).WithMany(b => b.S_WF_DefRouting).HasForeignKey(c => c.DefFlowID); // FK_S_WF_DefRouting_S_WF_DefFlow
            HasOptional(a => a.S_WF_DefStep).WithMany(b => b.S_WF_DefRouting).HasForeignKey(c => c.DefStepID); // FK_S_WF_DefRouting_S_WF_DefStep
        }
    }

    // S_WF_DefStep
    internal partial class S_WF_DefStepConfiguration : EntityTypeConfiguration<S_WF_DefStep>
    {
        public S_WF_DefStepConfiguration()
        {
			ToTable("S_WF_DEFSTEP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefFlowID).HasColumnName("DEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.SubFormID).HasColumnName("SUBFORMID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.AllowDelegate).HasColumnName("ALLOWDELEGATE").IsOptional().HasMaxLength(1);
            Property(x => x.AllowCirculate).HasColumnName("ALLOWCIRCULATE").IsOptional().HasMaxLength(1);
            Property(x => x.AllowAsk).HasColumnName("ALLOWASK").IsOptional().HasMaxLength(1);
            Property(x => x.AllowSave).HasColumnName("ALLOWSAVE").IsOptional().HasMaxLength(1);
            Property(x => x.SaveVariableAuth).HasColumnName("SAVEVARIABLEAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.SubFlowCode).HasColumnName("SUBFLOWCODE").IsOptional().HasMaxLength(50);
            Property(x => x.WaitingStepIDs).HasColumnName("WAITINGSTEPIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.CooperationMode).HasColumnName("COOPERATIONMODE").IsOptional().HasMaxLength(50);
            Property(x => x.Phase).HasColumnName("PHASE").IsOptional().HasMaxLength(50);
            Property(x => x.HiddenElements).HasColumnName("HIDDENELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.VisibleElements).HasColumnName("VISIBLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.EditableElements).HasColumnName("EDITABLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.DisableElements).HasColumnName("DISABLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.TimeoutAutoPass).HasColumnName("TIMEOUTAUTOPASS").IsOptional();
            Property(x => x.TimeoutNotice).HasColumnName("TIMEOUTNOTICE").IsOptional();
            Property(x => x.TimeoutAlarm).HasColumnName("TIMEOUTALARM").IsOptional();
            Property(x => x.TimeoutDeadline).HasColumnName("TIMEOUTDEADLINE").IsOptional();
            Property(x => x.KeepWhenEnd).HasColumnName("KEEPWHENEND").IsOptional().HasMaxLength(1);
            Property(x => x.AllowDoBackFirst).HasColumnName("ALLOWDOBACKFIRST").IsOptional().HasMaxLength(1);
            Property(x => x.AllowDoBackFirstReturn).HasColumnName("ALLOWDOBACKFIRSTRETURN").IsOptional().HasMaxLength(1);
            Property(x => x.DoBackSignField).HasColumnName("DOBACKSIGNFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.AllowToMobile).HasColumnName("ALLOWTOMOBILE").IsOptional().HasMaxLength(1);
            Property(x => x.HideAdvice).HasColumnName("HIDEADVICE").IsOptional().HasMaxLength(1);
            Property(x => x.EmptyToStep).HasColumnName("EMPTYTOSTEP").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);
            Property(x => x.DoBackButtonText).HasColumnName("DOBACKBUTTONTEXT").IsOptional().HasMaxLength(50);
            Property(x => x.DoBackSave).HasColumnName("DOBACKSAVE").IsOptional().HasMaxLength(1);

            // Foreign keys
            HasOptional(a => a.S_WF_DefFlow).WithMany(b => b.S_WF_DefStep).HasForeignKey(c => c.DefFlowID); // FK_S_WF_DefStep_S_WF_DefFlow
        }
    }

    // S_WF_DefSubForm
    internal partial class S_WF_DefSubFormConfiguration : EntityTypeConfiguration<S_WF_DefSubForm>
    {
        public S_WF_DefSubFormConfiguration()
        {
			ToTable("S_WF_DEFSUBFORM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefFlowID).HasColumnName("DEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.FormUrl).HasColumnName("FORMURL").IsOptional().HasMaxLength(200);
            Property(x => x.FormWidth).HasColumnName("FORMWIDTH").IsOptional().HasMaxLength(50);
            Property(x => x.FormHeight).HasColumnName("FORMHEIGHT").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_WF_DefFlow).WithMany(b => b.S_WF_DefSubForm).HasForeignKey(c => c.DefFlowID); // FK_S_WF_DefSubForm_S_WF_DefFlow
        }
    }

    // S_WF_InsDefFlow
    internal partial class S_WF_InsDefFlowConfiguration : EntityTypeConfiguration<S_WF_InsDefFlow>
    {
        public S_WF_InsDefFlowConfiguration()
        {
			ToTable("S_WF_INSDEFFLOW");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefFlowID).HasColumnName("DEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.FormUrl).HasColumnName("FORMURL").IsOptional().HasMaxLength(200);
            Property(x => x.FormWidth).HasColumnName("FORMWIDTH").IsOptional().HasMaxLength(50);
            Property(x => x.FormHeight).HasColumnName("FORMHEIGHT").IsOptional().HasMaxLength(50);
            Property(x => x.FlowNameTemplete).HasColumnName("FLOWNAMETEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.FlowCategorytemplete).HasColumnName("FLOWCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.FlowSubCategoryTemplete).HasColumnName("FLOWSUBCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskNameTemplete).HasColumnName("TASKNAMETEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskCategoryTemplete).HasColumnName("TASKCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.TaskSubCategoryTemplete).HasColumnName("TASKSUBCATEGORYTEMPLETE").IsOptional().HasMaxLength(200);
            Property(x => x.InitVariable).HasColumnName("INITVARIABLE").IsOptional().HasMaxLength(500);
            Property(x => x.AllowDeleteForm).HasColumnName("ALLOWDELETEFORM").IsOptional().HasMaxLength(1);
            Property(x => x.SendMsgToApplicant).HasColumnName("SENDMSGTOAPPLICANT").IsOptional().HasMaxLength(1);
            Property(x => x.ViewConfig).HasColumnName("VIEWCONFIG").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.FormNumberPartA).HasColumnName("FORMNUMBERPARTA").IsOptional().HasMaxLength(50);
            Property(x => x.FormNumberPartB).HasColumnName("FORMNUMBERPARTB").IsOptional().HasMaxLength(50);
            Property(x => x.FormNumberPartC).HasColumnName("FORMNUMBERPARTC").IsOptional().HasMaxLength(50);
            Property(x => x.MsgSendToAll).HasColumnName("MSGSENDTOALL").IsOptional().HasMaxLength(1);
            Property(x => x.IsFreeFlow).HasColumnName("ISFREEFLOW").IsOptional().HasMaxLength(1);
        }
    }

    // S_WF_InsDefRouting
    internal partial class S_WF_InsDefRoutingConfiguration : EntityTypeConfiguration<S_WF_InsDefRouting>
    {
        public S_WF_InsDefRoutingConfiguration()
        {
			ToTable("S_WF_INSDEFROUTING");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefRoutingID).HasColumnName("DEFROUTINGID").IsOptional().HasMaxLength(50);
            Property(x => x.InsDefFlowID).HasColumnName("INSDEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.InsDefStepID).HasColumnName("INSDEFSTEPID").IsOptional().HasMaxLength(50);
            Property(x => x.EndID).HasColumnName("ENDID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Value).HasColumnName("VALUE").IsOptional().HasMaxLength(50);
            Property(x => x.NotNullFields).HasColumnName("NOTNULLFIELDS").IsOptional().HasMaxLength(2000);
            Property(x => x.VariableSet).HasColumnName("VARIABLESET").IsOptional().HasMaxLength(500);
            Property(x => x.FormDataSet).HasColumnName("FORMDATASET").IsOptional().HasMaxLength(2000);
            Property(x => x.SaveForm).HasColumnName("SAVEFORM").IsOptional().HasMaxLength(1);
            Property(x => x.MustInputComment).HasColumnName("MUSTINPUTCOMMENT").IsOptional().HasMaxLength(1);
            Property(x => x.SaveFormVersion).HasColumnName("SAVEFORMVERSION").IsOptional().HasMaxLength(1);
            Property(x => x.DefaultComment).HasColumnName("DEFAULTCOMMENT").IsOptional().HasMaxLength(50);
            Property(x => x.SignatureField).HasColumnName("SIGNATUREFIELD").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureProtectFields).HasColumnName("SIGNATUREPROTECTFIELDS").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureDivID).HasColumnName("SIGNATUREDIVID").IsOptional().HasMaxLength(2000);
            Property(x => x.SignatureCancelDivIDs).HasColumnName("SIGNATURECANCELDIVIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.AuthFormData).HasColumnName("AUTHFORMDATA").IsOptional().HasMaxLength(500);
            Property(x => x.AuthTargetUser).HasColumnName("AUTHTARGETUSER").IsOptional().HasMaxLength(50);
            Property(x => x.AuthOrgIDs).HasColumnName("AUTHORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthOrgNames).HasColumnName("AUTHORGNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthRoleIDs).HasColumnName("AUTHROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthRoleNames).HasColumnName("AUTHROLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthUserIDs).HasColumnName("AUTHUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AuthUserNames).HasColumnName("AUTHUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AuthVariable).HasColumnName("AUTHVARIABLE").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDs).HasColumnName("USERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.UserNames).HasColumnName("USERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDsFromStep).HasColumnName("USERIDSFROMSTEP").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromStepSender).HasColumnName("USERIDSFROMSTEPSENDER").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromStepExec).HasColumnName("USERIDSFROMSTEPEXEC").IsOptional().HasMaxLength(50);
            Property(x => x.UserIDsFromField).HasColumnName("USERIDSFROMFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDsGroupFromField).HasColumnName("USERIDSGROUPFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.RoleIDs).HasColumnName("ROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.RoleNames).HasColumnName("ROLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.RoleIDsFromField).HasColumnName("ROLEIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.OrgIDs).HasColumnName("ORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.OrgNames).HasColumnName("ORGNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.OrgIDFromField).HasColumnName("ORGIDFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.OrgIDFromUser).HasColumnName("ORGIDFROMUSER").IsOptional().HasMaxLength(50);
            Property(x => x.SelectMode).HasColumnName("SELECTMODE").IsOptional().HasMaxLength(50);
            Property(x => x.SelectAgain).HasColumnName("SELECTAGAIN").IsOptional().HasMaxLength(1);
            Property(x => x.AllowWithdraw).HasColumnName("ALLOWWITHDRAW").IsOptional().HasMaxLength(1);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(50);
            Property(x => x.AllowDoBack).HasColumnName("ALLOWDOBACK").IsOptional().HasMaxLength(1);
            Property(x => x.OnlyDoBack).HasColumnName("ONLYDOBACK").IsOptional().HasMaxLength(1);
            Property(x => x.MsgUserIDs).HasColumnName("MSGUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserNames).HasColumnName("MSGUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStepExec).HasColumnName("MSGUSERIDSFROMSTEPEXEC").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStep).HasColumnName("MSGUSERIDSFROMSTEP").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromStepSender).HasColumnName("MSGUSERIDSFROMSTEPSENDER").IsOptional().HasMaxLength(500);
            Property(x => x.MsgUserIDsFromField).HasColumnName("MSGUSERIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgRoleIDs).HasColumnName("MSGROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgRoleIDsFromField).HasColumnName("MSGROLEIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgOrgIDs).HasColumnName("MSGORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.MsgOrgIDFromUser).HasColumnName("MSGORGIDFROMUSER").IsOptional().HasMaxLength(500);
            Property(x => x.MsgOrgIDsFromField).HasColumnName("MSGORGIDSFROMFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.MsgTmpl).HasColumnName("MSGTMPL").IsOptional().HasMaxLength(500);
            Property(x => x.MsgType).HasColumnName("MSGTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.MsgSendToTaskUser).HasColumnName("MSGSENDTOTASKUSER").IsOptional().HasMaxLength(1);
            Property(x => x.DenyAutoPass).HasColumnName("DENYAUTOPASS").IsOptional().HasMaxLength(1);
            Property(x => x.ExcludeUser).HasColumnName("EXCLUDEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.MsgRoleNames).HasColumnName("MSGROLENAMES").IsOptional().HasMaxLength(50);
            Property(x => x.MsgOrgNames).HasColumnName("MSGORGNAMES").IsOptional().HasMaxLength(50);
            Property(x => x.InputSignPwd).HasColumnName("INPUTSIGNPWD").IsOptional().HasMaxLength(50);
            Property(x => x.AuthFromSql).HasColumnName("AUTHFROMSQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.UserIDsFromSql).HasColumnName("USERIDSFROMSQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.SignatureType).HasColumnName("SIGNATURETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ExecLogic).HasColumnName("EXECLOGIC").IsOptional().HasMaxLength(1073741823);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.S_WF_InsDefFlow).WithMany(b => b.S_WF_InsDefRouting).HasForeignKey(c => c.InsDefFlowID); // FK_S_WF_InsDefRouting_S_WF_InsDefFlow
            HasOptional(a => a.S_WF_InsDefStep).WithMany(b => b.S_WF_InsDefRouting).HasForeignKey(c => c.InsDefStepID); // FK_S_WF_InsDefRouting_S_WF_InsDefStep
        }
    }

    // S_WF_InsDefStep
    internal partial class S_WF_InsDefStepConfiguration : EntityTypeConfiguration<S_WF_InsDefStep>
    {
        public S_WF_InsDefStepConfiguration()
        {
			ToTable("S_WF_INSDEFSTEP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefStepID).HasColumnName("DEFSTEPID").IsOptional().HasMaxLength(50);
            Property(x => x.InsDefFlowID).HasColumnName("INSDEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.SubFormID).HasColumnName("SUBFORMID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.AllowDelegate).HasColumnName("ALLOWDELEGATE").IsOptional().HasMaxLength(1);
            Property(x => x.AllowCirculate).HasColumnName("ALLOWCIRCULATE").IsOptional().HasMaxLength(1);
            Property(x => x.AllowAsk).HasColumnName("ALLOWASK").IsOptional().HasMaxLength(1);
            Property(x => x.AllowSave).HasColumnName("ALLOWSAVE").IsOptional().HasMaxLength(1);
            Property(x => x.SaveVariableAuth).HasColumnName("SAVEVARIABLEAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.SubFlowCode).HasColumnName("SUBFLOWCODE").IsOptional().HasMaxLength(50);
            Property(x => x.WaitingStepIDs).HasColumnName("WAITINGSTEPIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.CooperationMode).HasColumnName("COOPERATIONMODE").IsOptional().HasMaxLength(50);
            Property(x => x.Phase).HasColumnName("PHASE").IsOptional().HasMaxLength(50);
            Property(x => x.HiddenElements).HasColumnName("HIDDENELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.VisibleElements).HasColumnName("VISIBLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.EditableElements).HasColumnName("EDITABLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.DisableElements).HasColumnName("DISABLEELEMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.TimeoutAutoPass).HasColumnName("TIMEOUTAUTOPASS").IsOptional();
            Property(x => x.TimeoutNotice).HasColumnName("TIMEOUTNOTICE").IsOptional();
            Property(x => x.TimeoutAlarm).HasColumnName("TIMEOUTALARM").IsOptional();
            Property(x => x.TimeoutDeadline).HasColumnName("TIMEOUTDEADLINE").IsOptional();
            Property(x => x.KeepWhenEnd).HasColumnName("KEEPWHENEND").IsOptional().HasMaxLength(1);
            Property(x => x.AllowDoBackFirst).HasColumnName("ALLOWDOBACKFIRST").IsOptional().HasMaxLength(1);
            Property(x => x.AllowDoBackFirstReturn).HasColumnName("ALLOWDOBACKFIRSTRETURN").IsOptional().HasMaxLength(1);
            Property(x => x.DoBackSignField).HasColumnName("DOBACKSIGNFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.HideAdvice).HasColumnName("HIDEADVICE").IsOptional().HasMaxLength(1);
            Property(x => x.EmptyToStep).HasColumnName("EMPTYTOSTEP").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);
            Property(x => x.DoBackButtonText).HasColumnName("DOBACKBUTTONTEXT").IsOptional().HasMaxLength(50);
            Property(x => x.DoBackSave).HasColumnName("DOBACKSAVE").IsOptional().HasMaxLength(1);

            // Foreign keys
            HasOptional(a => a.S_WF_InsDefFlow).WithMany(b => b.S_WF_InsDefStep).HasForeignKey(c => c.InsDefFlowID); // FK_S_WF_InsDefStep_S_WF_InsDefFlow
        }
    }

    // S_WF_InsFlow
    internal partial class S_WF_InsFlowConfiguration : EntityTypeConfiguration<S_WF_InsFlow>
    {
        public S_WF_InsFlowConfiguration()
        {
			ToTable("S_WF_INSFLOW");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.InsDefFlowID).HasColumnName("INSDEFFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CompleteTime).HasColumnName("COMPLETETIME").IsOptional();
            Property(x => x.FormInstanceID).HasColumnName("FORMINSTANCEID").IsOptional().HasMaxLength(50);
            Property(x => x.FlowName).HasColumnName("FLOWNAME").IsOptional().HasMaxLength(200);
            Property(x => x.FlowCategory).HasColumnName("FLOWCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.FlowSubCategory).HasColumnName("FLOWSUBCATEGORY").IsOptional().HasMaxLength(500);
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.FatherTaskID).HasColumnName("FATHERTASKID").IsOptional().HasMaxLength(50);
            Property(x => x.TimeConsuming).HasColumnName("TIMECONSUMING").IsOptional();
            Property(x => x.CurrentStep).HasColumnName("CURRENTSTEP").IsOptional().HasMaxLength(200);
            Property(x => x.CurrentUserNames).HasColumnName("CURRENTUSERNAMES").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_WF_InsDefFlow).WithMany(b => b.S_WF_InsFlow).HasForeignKey(c => c.InsDefFlowID); // FK_S_WF_InsFlow_S_WF_InsDefFlow
        }
    }

    // S_WF_InsTask
    internal partial class S_WF_InsTaskConfiguration : EntityTypeConfiguration<S_WF_InsTask>
    {
        public S_WF_InsTaskConfiguration()
        {
			ToTable("S_WF_INSTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.InsDefStepID).HasColumnName("INSDEFSTEPID").IsOptional().HasMaxLength(50);
            Property(x => x.InsFlowID).HasColumnName("INSFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.TaskName).HasColumnName("TASKNAME").IsOptional().HasMaxLength(200);
            Property(x => x.TaskCategory).HasColumnName("TASKCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.TaskSubCategory).HasColumnName("TASKSUBCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.SendTaskIDs).HasColumnName("SENDTASKIDS").IsOptional().HasMaxLength(500);
            Property(x => x.SendTaskUserIDs).HasColumnName("SENDTASKUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.SendTaskUserNames).HasColumnName("SENDTASKUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.TaskUserIDs).HasColumnName("TASKUSERIDS").IsOptional().HasMaxLength(4000);
            Property(x => x.TaskUserNames).HasColumnName("TASKUSERNAMES").IsOptional().HasMaxLength(4000);
            Property(x => x.TaskUserIDsGroup).HasColumnName("TASKUSERIDSGROUP").IsOptional().HasMaxLength(4000);
            Property(x => x.TaskRoleIDs).HasColumnName("TASKROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.TaskOrgIDs).HasColumnName("TASKORGIDS").IsOptional();
            Property(x => x.WaitingRoutings).HasColumnName("WAITINGROUTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.WaitingSteps).HasColumnName("WAITINGSTEPS").IsOptional().HasMaxLength(2000);
            Property(x => x.ChildFlowID).HasColumnName("CHILDFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CompleteTime).HasColumnName("COMPLETETIME").IsOptional();
            Property(x => x.FormVersion).HasColumnName("FORMVERSION").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.FirstViewTime).HasColumnName("FIRSTVIEWTIME").IsOptional();
            Property(x => x.VariableVersion).HasColumnName("VARIABLEVERSION").IsOptional().HasMaxLength(2000);
            Property(x => x.DoBackRoutingID).HasColumnName("DOBACKROUTINGID").IsOptional().HasMaxLength(50);
            Property(x => x.OnlyDoBack).HasColumnName("ONLYDOBACK").IsOptional().HasMaxLength(1);
            Property(x => x.TimeConsuming).HasColumnName("TIMECONSUMING").IsOptional();
            Property(x => x.GoToUp).HasColumnName("GOTOUP").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_WF_InsDefStep).WithMany(b => b.S_WF_InsTask).HasForeignKey(c => c.InsDefStepID); // FK_S_WF_InsTask_S_WF_InsDefStep
            HasOptional(a => a.S_WF_InsFlow).WithMany(b => b.S_WF_InsTask).HasForeignKey(c => c.InsFlowID); // FK_S_WF_InsTask_S_WF_InsFlow1
        }
    }

    // S_WF_InsTaskExec
    internal partial class S_WF_InsTaskExecConfiguration : EntityTypeConfiguration<S_WF_InsTaskExec>
    {
        public S_WF_InsTaskExecConfiguration()
        {
			ToTable("S_WF_INSTASKEXEC");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.InsFlowID).HasColumnName("INSFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.InsTaskID).HasColumnName("INSTASKID").IsOptional().HasMaxLength(50);
            Property(x => x.ExecRoutingIDs).HasColumnName("EXECROUTINGIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.TaskUserID).HasColumnName("TASKUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.TaskUserName).HasColumnName("TASKUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ExecUserID).HasColumnName("EXECUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ExecUserName).HasColumnName("EXECUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ExecTime).HasColumnName("EXECTIME").IsOptional();
            Property(x => x.ExecComment).HasColumnName("EXECCOMMENT").IsOptional().HasMaxLength(2000);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.FirstViewTime).HasColumnName("FIRSTVIEWTIME").IsOptional();
            Property(x => x.TimeoutAutoPassResult).HasColumnName("TIMEOUTAUTOPASSRESULT").IsOptional().HasMaxLength(500);
            Property(x => x.TimeoutNoticeResult).HasColumnName("TIMEOUTNOTICERESULT").IsOptional().HasMaxLength(500);
            Property(x => x.TimeoutAlarmResult).HasColumnName("TIMEOUTALARMRESULT").IsOptional().HasMaxLength(500);
            Property(x => x.TimeoutDeadlineResult).HasColumnName("TIMEOUTDEADLINERESULT").IsOptional().HasMaxLength(500);
            Property(x => x.TimeoutAutoPass).HasColumnName("TIMEOUTAUTOPASS").IsOptional();
            Property(x => x.TimeoutNotice).HasColumnName("TIMEOUTNOTICE").IsOptional();
            Property(x => x.TimeoutAlarm).HasColumnName("TIMEOUTALARM").IsOptional();
            Property(x => x.TimeoutDeadline).HasColumnName("TIMEOUTDEADLINE").IsOptional();
            Property(x => x.TimeConsuming).HasColumnName("TIMECONSUMING").IsOptional();
            Property(x => x.WeakedRoutingIDs).HasColumnName("WEAKEDROUTINGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.ExecRoutingName).HasColumnName("EXECROUTINGNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ApprovalInMobile).HasColumnName("APPROVALINMOBILE").IsOptional().HasMaxLength(1);

            // Foreign keys
            HasOptional(a => a.S_WF_InsFlow).WithMany(b => b.S_WF_InsTaskExec).HasForeignKey(c => c.InsFlowID); // FK_S_WF_InsTaskExec_S_WF_InsFlow1
            HasOptional(a => a.S_WF_InsTask).WithMany(b => b.S_WF_InsTaskExec).HasForeignKey(c => c.InsTaskID); // FK_S_WF_InsTaskExec_S_WF_InsTask1
        }
    }

    // S_WF_InsVariable
    internal partial class S_WF_InsVariableConfiguration : EntityTypeConfiguration<S_WF_InsVariable>
    {
        public S_WF_InsVariableConfiguration()
        {
			ToTable("S_WF_INSVARIABLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.InsFlowID).HasColumnName("INSFLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.VariableName).HasColumnName("VARIABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.VariableValue).HasColumnName("VARIABLEVALUE").IsOptional().HasMaxLength(50);
            Property(x => x.VariablePreValue).HasColumnName("VARIABLEPREVALUE").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_WF_InsFlow).WithMany(b => b.S_WF_InsVariable).HasForeignKey(c => c.InsFlowID); // FK_S_WF_InsVariable_S_WF_InsFlow
        }
    }

    // Task
    internal partial class TaskConfiguration : EntityTypeConfiguration<Task>
    {
        public TaskConfiguration()
        {
			ToTable("TASK");
            HasKey(x => new { x.Type, x.TaskExecID, x.TaskID });

            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(1);
            Property(x => x.TaskExecID).HasColumnName("TASKEXECID").IsRequired().HasMaxLength(50);
            Property(x => x.ID).HasColumnName("ID").IsOptional().HasMaxLength(50);
            Property(x => x.TaskID).HasColumnName("TASKID").IsRequired().HasMaxLength(50);
            Property(x => x.StepID).HasColumnName("STEPID").IsOptional().HasMaxLength(50);
            Property(x => x.FlowID).HasColumnName("FLOWID").IsOptional().HasMaxLength(50);
            Property(x => x.FlowName).HasColumnName("FLOWNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DefStepName).HasColumnName("DEFSTEPNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TaskName).HasColumnName("TASKNAME").IsOptional().HasMaxLength(200);
            Property(x => x.FormUrl).HasColumnName("FORMURL").IsOptional().HasMaxLength(200);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.LaunchTime).HasColumnName("LAUNCHTIME").IsOptional();
            Property(x => x.ReceiveTime).HasColumnName("RECEIVETIME").IsOptional();
        }
    }

}

