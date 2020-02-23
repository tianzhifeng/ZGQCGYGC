

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "Base"
//     Connection String:      "Data Source=.;Initial Catalog=SINOAE_Base;User ID=sa;PWD=123.zxc;"

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

namespace Base.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class BaseEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_A__OrgRes> S_A__OrgRes { get; set; } // S_A__OrgRes
        public IDbSet<S_A__OrgRole> S_A__OrgRole { get; set; } // S_A__OrgRole
        public IDbSet<S_A__OrgRoleUser> S_A__OrgRoleUser { get; set; } // S_A__OrgRoleUser
        public IDbSet<S_A__OrgUser> S_A__OrgUser { get; set; } // S_A__OrgUser
        public IDbSet<S_A__RoleRes> S_A__RoleRes { get; set; } // S_A__RoleRes
        public IDbSet<S_A__RoleUser> S_A__RoleUser { get; set; } // S_A__RoleUser
        public IDbSet<S_A__UserRes> S_A__UserRes { get; set; } // S_A__UserRes
        public IDbSet<S_A_AuthCompany> S_A_AuthCompany { get; set; } // S_A_AuthCompany
        public IDbSet<S_A_AuthInfo> S_A_AuthInfo { get; set; } // S_A_AuthInfo
        public IDbSet<S_A_AuthLevel> S_A_AuthLevel { get; set; } // S_A_AuthLevel
        public IDbSet<S_A_AuthLog> S_A_AuthLog { get; set; } // S_A_AuthLog
        public IDbSet<S_A_DefaultAdivce> S_A_DefaultAdivce { get; set; } // S_A_DefaultAdivce
        public IDbSet<S_A_Org> S_A_Org { get; set; } // S_A_Org
        public IDbSet<S_A_Portal> S_A_Portal { get; set; } // S_A_Portal
        public IDbSet<S_A_PortalTemplet> S_A_PortalTemplet { get; set; } // S_A_PortalTemplet
        public IDbSet<S_A_Res> S_A_Res { get; set; } // S_A_Res
        public IDbSet<S_A_Role> S_A_Role { get; set; } // S_A_Role
        public IDbSet<S_A_Security> S_A_Security { get; set; } // S_A_Security
        public IDbSet<S_A_TempletRole> S_A_TempletRole { get; set; } // S_A_TempletRole
        public IDbSet<S_A_User> S_A_User { get; set; } // S_A_User
        public IDbSet<S_A_UserImg> S_A_UserImg { get; set; } // S_A_UserImg
        public IDbSet<S_A_UserLinkMan> S_A_UserLinkMan { get; set; } // S_A_UserLinkMan
        public IDbSet<S_C_Holiday> S_C_Holiday { get; set; } // S_C_Holiday
        public IDbSet<S_D_FormToPDFTask> S_D_FormToPDFTask { get; set; } // S_D_FormToPDFTask
        public IDbSet<S_D_ModifyLog> S_D_ModifyLog { get; set; } // S_D_ModifyLog
        public IDbSet<S_D_PushTask> S_D_PushTask { get; set; } // S_D_PushTask
        public IDbSet<S_H_AllFeedback> S_H_AllFeedback { get; set; } // S_H_AllFeedback
        public IDbSet<S_H_Calendar> S_H_Calendar { get; set; } // S_H_Calendar
        public IDbSet<S_H_Feedback> S_H_Feedback { get; set; } // S_H_Feedback
        public IDbSet<S_H_ShortCut> S_H_ShortCut { get; set; } // S_H_ShortCut
        public IDbSet<S_I_FriendLink> S_I_FriendLink { get; set; } // S_I_FriendLink
        public IDbSet<S_I_NewsImage> S_I_NewsImage { get; set; } // S_I_NewsImage
        public IDbSet<S_I_NewsImageGroup> S_I_NewsImageGroup { get; set; } // S_I_NewsImageGroup
        public IDbSet<S_I_PublicInformation> S_I_PublicInformation { get; set; } // S_I_PublicInformation
        public IDbSet<S_I_PublicInformCatalog> S_I_PublicInformCatalog { get; set; } // S_I_PublicInformCatalog
        public IDbSet<S_L_LoginLog> S_L_LoginLog { get; set; } // S_L_LoginLog
        public IDbSet<S_M_Category> S_M_Category { get; set; } // S_M_Category
        public IDbSet<S_M_ConfigManage> S_M_ConfigManage { get; set; } // S_M_ConfigManage
        public IDbSet<S_M_EnumDef> S_M_EnumDef { get; set; } // S_M_EnumDef
        public IDbSet<S_M_EnumItem> S_M_EnumItem { get; set; } // S_M_EnumItem
        public IDbSet<S_M_Field> S_M_Field { get; set; } // S_M_Field
        public IDbSet<S_M_Parameter> S_M_Parameter { get; set; } // S_M_Parameter
        public IDbSet<S_M_Table> S_M_Table { get; set; } // S_M_Table
        public IDbSet<S_OEM_TaskFileList> S_OEM_TaskFileList { get; set; } // S_OEM_TaskFileList
        public IDbSet<S_OEM_TaskList> S_OEM_TaskList { get; set; } // S_OEM_TaskList
        public IDbSet<S_R_DataSet> S_R_DataSet { get; set; } // S_R_DataSet
        public IDbSet<S_R_Define> S_R_Define { get; set; } // S_R_Define
        public IDbSet<S_R_Field> S_R_Field { get; set; } // S_R_Field
        public IDbSet<S_RC_RuleCode> S_RC_RuleCode { get; set; } // S_RC_RuleCode
        public IDbSet<S_RC_RuleCodeData> S_RC_RuleCodeData { get; set; } // S_RC_RuleCodeData
        public IDbSet<S_S_Alarm> S_S_Alarm { get; set; } // S_S_Alarm
        public IDbSet<S_S_AlarmConfig> S_S_AlarmConfig { get; set; } // S_S_AlarmConfig
        public IDbSet<S_S_MsgBody> S_S_MsgBody { get; set; } // S_S_MsgBody
        public IDbSet<S_S_MsgReceiver> S_S_MsgReceiver { get; set; } // S_S_MsgReceiver
        public IDbSet<S_S_Notify> S_S_Notify { get; set; } // S_S_Notify
        public IDbSet<S_S_PostLevelTemplate> S_S_PostLevelTemplate { get; set; } // S_S_PostLevelTemplate
        public IDbSet<S_S_PostLevelTemplate_PostList> S_S_PostLevelTemplate_PostList { get; set; } // S_S_PostLevelTemplate_PostList
        public IDbSet<S_T_Task> S_T_Task { get; set; } // S_T_Task
        public IDbSet<S_T_TaskExec> S_T_TaskExec { get; set; } // S_T_TaskExec
        public IDbSet<S_UI_BIConfig> S_UI_BIConfig { get; set; } // S_UI_BIConfig
        public IDbSet<S_UI_Component> S_UI_Component { get; set; } // S_UI_Component
        public IDbSet<S_UI_DataSource> S_UI_DataSource { get; set; } // S_UI_DataSource
        public IDbSet<S_UI_ExcelImport> S_UI_ExcelImport { get; set; } // S_UI_ExcelImport
        public IDbSet<S_UI_ExcelPrint> S_UI_ExcelPrint { get; set; } // S_UI_ExcelPrint
        public IDbSet<S_UI_Form> S_UI_Form { get; set; } // S_UI_Form
        public IDbSet<S_UI_FreePivot> S_UI_FreePivot { get; set; } // S_UI_FreePivot
        public IDbSet<S_UI_FreePivotInstance> S_UI_FreePivotInstance { get; set; } // S_UI_FreePivotInstance
        public IDbSet<S_UI_FreePivotInstanceUser> S_UI_FreePivotInstanceUser { get; set; } // S_UI_FreePivotInstanceUser
        public IDbSet<S_UI_Help> S_UI_Help { get; set; } // S_UI_Help
        public IDbSet<S_UI_JinGeSign> S_UI_JinGeSign { get; set; } // S_UI_JinGeSign
        public IDbSet<S_UI_Layout> S_UI_Layout { get; set; } // S_UI_Layout
        public IDbSet<S_UI_List> S_UI_List { get; set; } // S_UI_List
        public IDbSet<S_UI_ModifyLogLog> S_UI_ModifyLogLog { get; set; } // S_UI_ModifyLogLog
        public IDbSet<S_UI_Pivot> S_UI_Pivot { get; set; } // S_UI_Pivot
        public IDbSet<S_UI_PivotUser> S_UI_PivotUser { get; set; } // S_UI_PivotUser
        public IDbSet<S_UI_RoleRes> S_UI_RoleRes { get; set; } // S_UI_RoleRes
        public IDbSet<S_UI_Selector> S_UI_Selector { get; set; } // S_UI_Selector
        public IDbSet<S_UI_SerialNumber> S_UI_SerialNumber { get; set; } // S_UI_SerialNumber
        public IDbSet<S_UI_Word> S_UI_Word { get; set; } // S_UI_Word
        public IDbSet<tmp_ms_xx_S_A_AuthLog> tmp_ms_xx_S_A_AuthLog { get; set; } // tmp_ms_xx_S_A_AuthLog

        static BaseEntities()
        {
            Database.SetInitializer<BaseEntities>(null);
        }

        public BaseEntities()
            : base("Name=Base")
        {
        }

        public BaseEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_A__OrgResConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgRoleConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgRoleUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__RoleResConfiguration());
            modelBuilder.Configurations.Add(new S_A__RoleUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__UserResConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthCompanyConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthInfoConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthLevelConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthLogConfiguration());
            modelBuilder.Configurations.Add(new S_A_DefaultAdivceConfiguration());
            modelBuilder.Configurations.Add(new S_A_OrgConfiguration());
            modelBuilder.Configurations.Add(new S_A_PortalConfiguration());
            modelBuilder.Configurations.Add(new S_A_PortalTempletConfiguration());
            modelBuilder.Configurations.Add(new S_A_ResConfiguration());
            modelBuilder.Configurations.Add(new S_A_RoleConfiguration());
            modelBuilder.Configurations.Add(new S_A_SecurityConfiguration());
            modelBuilder.Configurations.Add(new S_A_TempletRoleConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserImgConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserLinkManConfiguration());
            modelBuilder.Configurations.Add(new S_C_HolidayConfiguration());
            modelBuilder.Configurations.Add(new S_D_FormToPDFTaskConfiguration());
            modelBuilder.Configurations.Add(new S_D_ModifyLogConfiguration());
            modelBuilder.Configurations.Add(new S_D_PushTaskConfiguration());
            modelBuilder.Configurations.Add(new S_H_AllFeedbackConfiguration());
            modelBuilder.Configurations.Add(new S_H_CalendarConfiguration());
            modelBuilder.Configurations.Add(new S_H_FeedbackConfiguration());
            modelBuilder.Configurations.Add(new S_H_ShortCutConfiguration());
            modelBuilder.Configurations.Add(new S_I_FriendLinkConfiguration());
            modelBuilder.Configurations.Add(new S_I_NewsImageConfiguration());
            modelBuilder.Configurations.Add(new S_I_NewsImageGroupConfiguration());
            modelBuilder.Configurations.Add(new S_I_PublicInformationConfiguration());
            modelBuilder.Configurations.Add(new S_I_PublicInformCatalogConfiguration());
            modelBuilder.Configurations.Add(new S_L_LoginLogConfiguration());
            modelBuilder.Configurations.Add(new S_M_CategoryConfiguration());
            modelBuilder.Configurations.Add(new S_M_ConfigManageConfiguration());
            modelBuilder.Configurations.Add(new S_M_EnumDefConfiguration());
            modelBuilder.Configurations.Add(new S_M_EnumItemConfiguration());
            modelBuilder.Configurations.Add(new S_M_FieldConfiguration());
            modelBuilder.Configurations.Add(new S_M_ParameterConfiguration());
            modelBuilder.Configurations.Add(new S_M_TableConfiguration());
            modelBuilder.Configurations.Add(new S_OEM_TaskFileListConfiguration());
            modelBuilder.Configurations.Add(new S_OEM_TaskListConfiguration());
            modelBuilder.Configurations.Add(new S_R_DataSetConfiguration());
            modelBuilder.Configurations.Add(new S_R_DefineConfiguration());
            modelBuilder.Configurations.Add(new S_R_FieldConfiguration());
            modelBuilder.Configurations.Add(new S_RC_RuleCodeConfiguration());
            modelBuilder.Configurations.Add(new S_RC_RuleCodeDataConfiguration());
            modelBuilder.Configurations.Add(new S_S_AlarmConfiguration());
            modelBuilder.Configurations.Add(new S_S_AlarmConfigConfiguration());
            modelBuilder.Configurations.Add(new S_S_MsgBodyConfiguration());
            modelBuilder.Configurations.Add(new S_S_MsgReceiverConfiguration());
            modelBuilder.Configurations.Add(new S_S_NotifyConfiguration());
            modelBuilder.Configurations.Add(new S_S_PostLevelTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_S_PostLevelTemplate_PostListConfiguration());
            modelBuilder.Configurations.Add(new S_T_TaskConfiguration());
            modelBuilder.Configurations.Add(new S_T_TaskExecConfiguration());
            modelBuilder.Configurations.Add(new S_UI_BIConfigConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ComponentConfiguration());
            modelBuilder.Configurations.Add(new S_UI_DataSourceConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ExcelImportConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ExcelPrintConfiguration());
            modelBuilder.Configurations.Add(new S_UI_FormConfiguration());
            modelBuilder.Configurations.Add(new S_UI_FreePivotConfiguration());
            modelBuilder.Configurations.Add(new S_UI_FreePivotInstanceConfiguration());
            modelBuilder.Configurations.Add(new S_UI_FreePivotInstanceUserConfiguration());
            modelBuilder.Configurations.Add(new S_UI_HelpConfiguration());
            modelBuilder.Configurations.Add(new S_UI_JinGeSignConfiguration());
            modelBuilder.Configurations.Add(new S_UI_LayoutConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ListConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ModifyLogLogConfiguration());
            modelBuilder.Configurations.Add(new S_UI_PivotConfiguration());
            modelBuilder.Configurations.Add(new S_UI_PivotUserConfiguration());
            modelBuilder.Configurations.Add(new S_UI_RoleResConfiguration());
            modelBuilder.Configurations.Add(new S_UI_SelectorConfiguration());
            modelBuilder.Configurations.Add(new S_UI_SerialNumberConfiguration());
            modelBuilder.Configurations.Add(new S_UI_WordConfiguration());
            modelBuilder.Configurations.Add(new tmp_ms_xx_S_A_AuthLogConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_A__OrgRes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__OrgRes_S_A_Res
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_S_A__OrgRes_S_A_Org
    }

	/// <summary>组织和角色关系表</summary>	
	[Description("组织和角色关系表")]
    public partial class S_A__OrgRole : Formula.BaseModel
    {
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary>组织ID</summary>	
		[Description("组织ID")]
        public string OrgID { get; set; } // OrgID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_A_OrgRole_ARole
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_A_OrgRole_AOrg
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A__OrgRoleUser : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_S_A__OrgRoleUser_S_A_Role
    }

	/// <summary>组织和用户关系表</summary>	
	[Description("组织和用户关系表")]
    public partial class S_A__OrgUser : Formula.BaseModel
    {
		/// <summary>组织ID</summary>	
		[Description("组织ID")]
        public string OrgID { get; set; } // OrgID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_A_OrgUser_AOrg
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_A_OrgUser_AUser
    }

	/// <summary>角色和权限资源关系表</summary>	
	[Description("角色和权限资源关系表")]
    public partial class S_A__RoleRes : Formula.BaseModel
    {
		/// <summary>权限资源ID</summary>	
		[Description("权限资源ID")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__RoleRes_S_A_Res
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_S_A__RoleRes_S_A_Role
    }

	/// <summary>角色和用户关系表</summary>	
	[Description("角色和用户关系表")]
    public partial class S_A__RoleUser : Formula.BaseModel
    {
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_A_RoleUser_ARole
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_A_RoleUser_AUser
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A__UserRes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DenyAuth { get; set; } // DenyAuth

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A__UserRes_S_A_User
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__UserRes_S_A_Res
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_AuthCompany : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get; set; } // CompanyID
		/// <summary></summary>	
		[Description("")]
        public string ResID { get; set; } // ResID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
    }

	/// <summary>权限模块信息表</summary>	
	[Description("权限模块信息表")]
    public partial class S_A_AuthInfo : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>组织机构树的根</summary>	
		[Description("组织机构树的根")]
        public string OrgRootFullID { get; set; } // OrgRootFullID
		/// <summary>组织机构根</summary>	
		[Description("组织机构根")]
        public string ResRootFullID { get; set; } // ResRootFullID
		/// <summary>角色组ID</summary>	
		[Description("角色组ID")]
        public string RoleGroupID { get; set; } // RoleGroupID
		/// <summary>用户组ID</summary>	
		[Description("用户组ID")]
        public string UserGroupID { get; set; } // UserGroupID
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
    }

	/// <summary>分级授权</summary>	
	[Description("分级授权")]
    public partial class S_A_AuthLevel : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>用户姓名</summary>	
		[Description("用户姓名")]
        public string UserName { get; set; } // UserName
		/// <summary>可以授权的菜单根</summary>	
		[Description("可以授权的菜单根")]
        public string MenuRootFullID { get; set; } // MenuRootFullID
		/// <summary>可以授权的菜单根</summary>	
		[Description("可以授权的菜单根")]
        public string MenuRootName { get; set; } // MenuRootName
		/// <summary>可以授权的规则根</summary>	
		[Description("可以授权的规则根")]
        public string RuleRootFullID { get; set; } // RuleRootFullID
		/// <summary>可以授权的规则根</summary>	
		[Description("可以授权的规则根")]
        public string RuleRootName { get; set; } // RuleRootName
		/// <summary></summary>	
		[Description("")]
        public string CorpID { get; set; } // CorpID
		/// <summary></summary>	
		[Description("")]
        public string CorpName { get; set; } // CorpName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_AuthLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Operation { get; set; } // Operation
		/// <summary></summary>	
		[Description("")]
        public string OperationTarget { get; set; } // OperationTarget
		/// <summary></summary>	
		[Description("")]
        public string RelateData { get; set; } // RelateData
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ClientIP { get; set; } // ClientIP
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_DefaultAdivce : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string Advice { get; set; } // Advice
    }

	/// <summary>组织机构表</summary>	
	[Description("组织机构表")]
    public partial class S_A_Org : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全路径ID</summary>	
		[Description("全路径ID")]
        public string FullID { get; set; } // FullID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称（简称）</summary>	
		[Description("名称（简称）")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>全称</summary>	
		[Description("全称")]
        public string ShortName { get; set; } // ShortName
		/// <summary>性质</summary>	
		[Description("性质")]
        public string Character { get; set; } // Character
		/// <summary>所在地</summary>	
		[Description("所在地")]
        public string Location { get; set; } // Location
		/// <summary></summary>	
		[Description("")]
        public string IsShow { get; set; } // IsShow
		/// <summary></summary>	
		[Description("")]
        public string IsIndependentManagement { get; set; } // IsIndependentManagement
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRes> S_A__OrgRes { get { onS_A__OrgResGetting(); return _S_A__OrgRes;} }
		private ICollection<S_A__OrgRes> _S_A__OrgRes;
		partial void onS_A__OrgResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgRole> S_A__OrgRole { get { onS_A__OrgRoleGetting(); return _S_A__OrgRole;} }
		private ICollection<S_A__OrgRole> _S_A__OrgRole;
		partial void onS_A__OrgRoleGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgUser> S_A__OrgUser { get { onS_A__OrgUserGetting(); return _S_A__OrgUser;} }
		private ICollection<S_A__OrgUser> _S_A__OrgUser;
		partial void onS_A__OrgUserGetting();


        public S_A_Org()
        {
			IsDeleted = "0";
            _S_A__OrgRes = new List<S_A__OrgRes>();
            _S_A__OrgRole = new List<S_A__OrgRole>();
            _S_A__OrgUser = new List<S_A__OrgUser>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_Portal : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public string MoreUrl { get; set; } // MoreUrl
		/// <summary></summary>	
		[Description("")]
        public string Height { get; set; } // Height
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string DisplayType { get; set; } // DisplayType
		/// <summary></summary>	
		[Description("")]
        public string PublicInformCatalog { get; set; } // PublicInformCatalog
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_PortalTemplet : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string Rows { get; set; } // Rows
		/// <summary></summary>	
		[Description("")]
        public string Cols { get; set; } // Cols
		/// <summary></summary>	
		[Description("")]
        public string ColsWidth { get; set; } // ColsWidth
		/// <summary></summary>	
		[Description("")]
        public bool? IsEnabled { get; set; } // IsEnabled
		/// <summary></summary>	
		[Description("")]
        public int? Xorder { get; set; } // Xorder
		/// <summary></summary>	
		[Description("")]
        public string IsNewPortal { get; set; } // IsNewPortal

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A_TempletRole> S_A_TempletRole { get { onS_A_TempletRoleGetting(); return _S_A_TempletRole;} }
		private ICollection<S_A_TempletRole> _S_A_TempletRole;
		partial void onS_A_TempletRoleGetting();


        public S_A_PortalTemplet()
        {
            _S_A_TempletRole = new List<S_A_TempletRole>();
        }
    }

	/// <summary>权限资源表</summary>	
	[Description("权限资源表")]
    public partial class S_A_Res : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全ID</summary>	
		[Description("全ID")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>Url</summary>	
		[Description("Url")]
        public string Url { get; set; } // Url
		/// <summary>图标Url</summary>	
		[Description("图标Url")]
        public string IconUrl { get; set; } // IconUrl
		/// <summary>打开目标</summary>	
		[Description("打开目标")]
        public string Target { get; set; } // Target
		/// <summary>按钮ID</summary>	
		[Description("按钮ID")]
        public string ButtonID { get; set; } // ButtonID
		/// <summary>数据过滤</summary>	
		[Description("数据过滤")]
        public string DataFilter { get; set; } // DataFilter
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>PortalLTE</summary>	
		[Description("PortalLTE")]
        public string IconClass { get; set; } // IconClass
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN
		/// <summary></summary>	
		[Description("")]
        public string MenuID { get; set; } // MenuID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRes> S_A__OrgRes { get { onS_A__OrgResGetting(); return _S_A__OrgRes;} }
		private ICollection<S_A__OrgRes> _S_A__OrgRes;
		partial void onS_A__OrgResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleRes> S_A__RoleRes { get { onS_A__RoleResGetting(); return _S_A__RoleRes;} }
		private ICollection<S_A__RoleRes> _S_A__RoleRes;
		partial void onS_A__RoleResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__UserRes> S_A__UserRes { get { onS_A__UserResGetting(); return _S_A__UserRes;} }
		private ICollection<S_A__UserRes> _S_A__UserRes;
		partial void onS_A__UserResGetting();


        public S_A_Res()
        {
			Type = "Menu";
            _S_A__OrgRes = new List<S_A__OrgRes>();
            _S_A__RoleRes = new List<S_A__RoleRes>();
            _S_A__UserRes = new List<S_A__UserRes>();
        }
    }

	/// <summary>角色表</summary>	
	[Description("角色表")]
    public partial class S_A_Role : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>角色组ID</summary>	
		[Description("角色组ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID
		/// <summary></summary>	
		[Description("")]
        public string OrgName { get; set; } // OrgName
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN
		/// <summary>子公司权限功能使用</summary>	
		[Description("子公司权限功能使用")]
        public string CorpID { get; set; } // CorpID
		/// <summary></summary>	
		[Description("")]
        public string CorpName { get; set; } // CorpName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRole> S_A__OrgRole { get { onS_A__OrgRoleGetting(); return _S_A__OrgRole;} }
		private ICollection<S_A__OrgRole> _S_A__OrgRole;
		partial void onS_A__OrgRoleGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgRoleUser> S_A__OrgRoleUser { get { onS_A__OrgRoleUserGetting(); return _S_A__OrgRoleUser;} }
		private ICollection<S_A__OrgRoleUser> _S_A__OrgRoleUser;
		partial void onS_A__OrgRoleUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleRes> S_A__RoleRes { get { onS_A__RoleResGetting(); return _S_A__RoleRes;} }
		private ICollection<S_A__RoleRes> _S_A__RoleRes;
		partial void onS_A__RoleResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleUser> S_A__RoleUser { get { onS_A__RoleUserGetting(); return _S_A__RoleUser;} }
		private ICollection<S_A__RoleUser> _S_A__RoleUser;
		partial void onS_A__RoleUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A_TempletRole> S_A_TempletRole { get { onS_A_TempletRoleGetting(); return _S_A_TempletRole;} }
		private ICollection<S_A_TempletRole> _S_A_TempletRole;
		partial void onS_A_TempletRoleGetting();


        public S_A_Role()
        {
            _S_A__OrgRole = new List<S_A__OrgRole>();
            _S_A__OrgRoleUser = new List<S_A__OrgRoleUser>();
            _S_A__RoleRes = new List<S_A__RoleRes>();
            _S_A__RoleUser = new List<S_A__RoleUser>();
            _S_A_TempletRole = new List<S_A_TempletRole>();
        }
    }

	/// <summary>三权分离表</summary>	
	[Description("三权分离表")]
    public partial class S_A_Security : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>超级管理员账号,必须为administrator</summary>	
		[Description("超级管理员账号,必须为administrator")]
        public string SuperAdmin { get; set; } // SuperAdmin
		/// <summary>超级管理员密码</summary>	
		[Description("超级管理员密码")]
        public string SuperAdminPwd { get; set; } // SuperAdminPwd
		/// <summary>超级管理员密码安全</summary>	
		[Description("超级管理员密码安全")]
        public string SuperAdminSecurity { get; set; } // SuperAdminSecurity
		/// <summary>超级管理员密码修改时间</summary>	
		[Description("超级管理员密码修改时间")]
        public DateTime? SuperAdminModifyTime { get; set; } // SuperAdminModifyTime
		/// <summary>管理员IDs</summary>	
		[Description("管理员IDs")]
        public string AdminIDs { get; set; } // AdminIDs
		/// <summary>管理员Names</summary>	
		[Description("管理员Names")]
        public string AdminNames { get; set; } // AdminNames
		/// <summary>管理员修改时间</summary>	
		[Description("管理员修改时间")]
        public DateTime? AdminModifyTime { get; set; } // AdminModifyTime
		/// <summary>管理员安全</summary>	
		[Description("管理员安全")]
        public string AdminSecurity { get; set; } // AdminSecurity
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_TempletRole : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TempletID { get; set; } // TempletID
		/// <summary></summary>	
		[Description("")]
        public string RoleID { get; set; } // RoleID
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_PortalTemplet S_A_PortalTemplet { get; set; } //  TempletID - FK_S_A_TempletRole_S_A_PortalTemplet
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_S_A_TempletRole_S_A_Role
    }

	/// <summary>用户表</summary>	
	[Description("用户表")]
    public partial class S_A_User : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户组ID</summary>	
		[Description("用户组ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>帐号</summary>	
		[Description("帐号")]
        public string Code { get; set; } // Code
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Name { get; set; } // Name
		/// <summary>工号</summary>	
		[Description("工号")]
        public string WorkNo { get; set; } // WorkNo
		/// <summary>密码</summary>	
		[Description("密码")]
        public string Password { get; set; } // Password
		/// <summary>性别</summary>	
		[Description("性别")]
        public string Sex { get; set; } // Sex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>入职日期</summary>	
		[Description("入职日期")]
        public DateTime? InDate { get; set; } // InDate
		/// <summary>离职日期</summary>	
		[Description("离职日期")]
        public DateTime? OutDate { get; set; } // OutDate
		/// <summary>电话</summary>	
		[Description("电话")]
        public string Phone { get; set; } // Phone
		/// <summary>手机</summary>	
		[Description("手机")]
        public string MobilePhone { get; set; } // MobilePhone
		/// <summary>Email</summary>	
		[Description("Email")]
        public string Email { get; set; } // Email
		/// <summary>地址</summary>	
		[Description("地址")]
        public string Address { get; set; } // Address
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>最后登录时间</summary>	
		[Description("最后登录时间")]
        public string LastLoginTime { get; set; } // LastLoginTime
		/// <summary>最后登录IP</summary>	
		[Description("最后登录IP")]
        public string LastLoginIP { get; set; } // LastLoginIP
		/// <summary>最后登录SessionID</summary>	
		[Description("最后登录SessionID")]
        public string LastSessionID { get; set; } // LastSessionID
		/// <summary>登录错误次数</summary>	
		[Description("登录错误次数")]
        public int? ErrorCount { get; set; } // ErrorCount
		/// <summary></summary>	
		[Description("")]
        public DateTime? ErrorTime { get; set; } // ErrorTime
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>当前项目ID</summary>	
		[Description("当前项目ID")]
        public string PrjID { get; set; } // PrjID
		/// <summary>当前项目名称</summary>	
		[Description("当前项目名称")]
        public string PrjName { get; set; } // PrjName
		/// <summary>当前部门ID</summary>	
		[Description("当前部门ID")]
        public string DeptID { get; set; } // DeptID
		/// <summary>当前部门全ID</summary>	
		[Description("当前部门全ID")]
        public string DeptFullID { get; set; } // DeptFullID
		/// <summary>当前部门名称</summary>	
		[Description("当前部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary></summary>	
		[Description("")]
        public string RTX { get; set; } // RTX
		/// <summary>最后更新时间</summary>	
		[Description("最后更新时间")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string Weixin { get; set; } // Weixin
		/// <summary>签字密码</summary>	
		[Description("签字密码")]
        public string SignPwd { get; set; } // SignPwd
		/// <summary></summary>	
		[Description("")]
        public string PortalSettings { get; set; } // PortalSettings
		/// <summary>当前公司ID</summary>	
		[Description("当前公司ID")]
        public string CorpID { get; set; } // CorpID
		/// <summary>当前公司名称</summary>	
		[Description("当前公司名称")]
        public string CorpName { get; set; } // CorpName
		/// <summary>双语支持</summary>	
		[Description("双语支持")]
        public string NameEN { get; set; } // NameEN
		/// <summary></summary>	
		[Description("")]
        public DateTime? SyncCloudTime { get; set; } // SyncCloudTime
		/// <summary></summary>	
		[Description("")]
        public string Position { get; set; } // Position
		/// <summary></summary>	
		[Description("")]
        public string DingDing { get; set; } // DingDing
		/// <summary></summary>	
		[Description("")]
        public string AdminCompanyID { get; set; } // AdminCompanyID
		/// <summary></summary>	
		[Description("")]
        public string AdminCompanyName { get; set; } // AdminCompanyName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgUser> S_A__OrgUser { get { onS_A__OrgUserGetting(); return _S_A__OrgUser;} }
		private ICollection<S_A__OrgUser> _S_A__OrgUser;
		partial void onS_A__OrgUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleUser> S_A__RoleUser { get { onS_A__RoleUserGetting(); return _S_A__RoleUser;} }
		private ICollection<S_A__RoleUser> _S_A__RoleUser;
		partial void onS_A__RoleUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__UserRes> S_A__UserRes { get { onS_A__UserResGetting(); return _S_A__UserRes;} }
		private ICollection<S_A__UserRes> _S_A__UserRes;
		partial void onS_A__UserResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A_UserImg> S_A_UserImg { get { onS_A_UserImgGetting(); return _S_A_UserImg;} }
		private ICollection<S_A_UserImg> _S_A_UserImg;
		partial void onS_A_UserImgGetting();

		[JsonIgnore]
        public virtual ICollection<S_A_UserLinkMan> S_A_UserLinkMan { get { onS_A_UserLinkManGetting(); return _S_A_UserLinkMan;} }
		private ICollection<S_A_UserLinkMan> _S_A_UserLinkMan;
		partial void onS_A_UserLinkManGetting();


        public S_A_User()
        {
			SortIndex = 0;
			ErrorCount = 0;
			IsDeleted = "0";
            _S_A__OrgUser = new List<S_A__OrgUser>();
            _S_A__RoleUser = new List<S_A__RoleUser>();
            _S_A__UserRes = new List<S_A__UserRes>();
            _S_A_UserImg = new List<S_A_UserImg>();
            _S_A_UserLinkMan = new List<S_A_UserLinkMan>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_UserImg : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>签名图片</summary>	
		[Description("签名图片")]
        public byte[] SignImg { get; set; } // SignImg
		/// <summary>照片</summary>	
		[Description("照片")]
        public byte[] Picture { get; set; } // Picture
		/// <summary></summary>	
		[Description("")]
        public byte[] DwgFile { get; set; } // DwgFile

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A_UserImg_S_A_User
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_UserLinkMan : Formula.BaseModel
    {
		/// <summary>表格标识</summary>	
		[Description("表格标识")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID标识</summary>	
		[Description("用户ID标识")]
        public string UserID { get; set; } // UserID
		/// <summary>联系人ID标识</summary>	
		[Description("联系人ID标识")]
        public string LinkManID { get; set; } // LinkManID
		/// <summary>排序</summary>	
		[Description("排序")]
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A_UserLinkMan_S_A_User
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_Holiday : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public int? Year { get; set; } // Year
		/// <summary></summary>	
		[Description("")]
        public int? Month { get; set; } // Month
		/// <summary></summary>	
		[Description("")]
        public int? Day { get; set; } // Day
		/// <summary></summary>	
		[Description("")]
        public DateTime? Date { get; set; } // Date
		/// <summary></summary>	
		[Description("")]
        public string DayOfWeek { get; set; } // DayOfWeek
		/// <summary></summary>	
		[Description("")]
        public string IsHoliday { get; set; } // IsHoliday
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_FormToPDFTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TempCode { get; set; } // TempCode
		/// <summary></summary>	
		[Description("")]
        public string FormID { get; set; } // FormID
		/// <summary></summary>	
		[Description("")]
        public string PDFFileID { get; set; } // PDFFileID
		/// <summary>表单最后更新时间</summary>	
		[Description("表单最后更新时间")]
        public DateTime? FormLastModifyDate { get; set; } // FormLastModifyDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? BeginTime { get; set; } // BeginTime
		/// <summary>完成时间</summary>	
		[Description("完成时间")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string DoneLog { get; set; } // DoneLog
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary>新增转图片字段</summary>	
		[Description("新增转图片字段")]
        public string IMGFileIDs { get; set; } // IMGFileIDs
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_ModifyLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string ModifyMode { get; set; } // ModifyMode
		/// <summary></summary>	
		[Description("")]
        public string EntityKey { get; set; } // EntityKey
		/// <summary></summary>	
		[Description("")]
        public string CurrentValue { get; set; } // CurrentValue
		/// <summary></summary>	
		[Description("")]
        public string OriginalValue { get; set; } // OriginalValue
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ClientIP { get; set; } // ClientIP
		/// <summary></summary>	
		[Description("")]
        public string UserHostAddress { get; set; } // UserHostAddress
    }

	/// <summary>百度云消息推送</summary>	
	[Description("百度云消息推送")]
    public partial class S_D_PushTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SourceID { get; set; } // SourceID
		/// <summary></summary>	
		[Description("")]
        public string FormInstanceID { get; set; } // FormInstanceID
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string ShortContent { get; set; } // ShortContent
		/// <summary></summary>	
		[Description("")]
        public string SendUserID { get; set; } // SendUserID
		/// <summary></summary>	
		[Description("")]
        public string SendUserName { get; set; } // SendUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary></summary>	
		[Description("")]
        public string SourceType { get; set; } // SourceType
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string PushType { get; set; } // PushType
		/// <summary></summary>	
		[Description("")]
        public DateTime? BeginTime { get; set; } // BeginTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string DoneLog { get; set; } // DoneLog
		/// <summary>移动设备ID（百度云推送生成）</summary>	
		[Description("移动设备ID（百度云推送生成）")]
        public string ClientID { get; set; } // ClientID
		/// <summary>移动AppID（百度生成）</summary>	
		[Description("移动AppID（百度生成）")]
        public string AppID { get; set; } // AppID
		/// <summary>频道（百度云推送生成）</summary>	
		[Description("频道（百度云推送生成）")]
        public string ChannelID { get; set; } // ChannelID
		/// <summary></summary>	
		[Description("")]
        public string DeviceOS { get; set; } // DeviceOS

        public S_D_PushTask()
        {
			PushType = "Boardcast=0,Personal=1,Group=2";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_AllFeedback : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string IsUse { get; set; } // IsUse
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string DealStatus { get; set; } // DealStatus
		/// <summary></summary>	
		[Description("")]
        public string DealResult { get; set; } // DealResult
		/// <summary></summary>	
		[Description("")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary></summary>	
		[Description("")]
        public string DealUserName { get; set; } // DealUserName
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Level { get; set; } // Level
		/// <summary>所属模块</summary>	
		[Description("所属模块")]
        public string Module { get; set; } // Module
		/// <summary>计划完成时间</summary>	
		[Description("计划完成时间")]
        public DateTime? ExpectedCompleteTime { get; set; } // ExpectedCompleteTime
		/// <summary>问题性质</summary>	
		[Description("问题性质")]
        public string ProblemNature { get; set; } // ProblemNature
		/// <summary></summary>	
		[Description("")]
        public string ProjectPrincipal { get; set; } // ProjectPrincipal
		/// <summary></summary>	
		[Description("")]
        public string ProjectDept { get; set; } // ProjectDept
		/// <summary></summary>	
		[Description("")]
        public DateTime? ConfirmProblemsTime { get; set; } // ConfirmProblemsTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? PlanCompleteTime { get; set; } // PlanCompleteTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ActualCompleteTime { get; set; } // ActualCompleteTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ConfirmCompleteTime { get; set; } // ConfirmCompleteTime
		/// <summary></summary>	
		[Description("")]
        public string ConfirmProblemsUserID { get; set; } // ConfirmProblemsUserID
		/// <summary></summary>	
		[Description("")]
        public string ConfirmProblemsUserName { get; set; } // ConfirmProblemsUserName
		/// <summary></summary>	
		[Description("")]
        public string ConfirmCompleteUserID { get; set; } // ConfirmCompleteUserID
		/// <summary></summary>	
		[Description("")]
        public string ConfirmCompleteUserName { get; set; } // ConfirmCompleteUserName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_Calendar : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Grade { get; set; } // Grade
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string Sponsor { get; set; } // Sponsor
		/// <summary></summary>	
		[Description("")]
        public string SponsorID { get; set; } // SponsorID
		/// <summary></summary>	
		[Description("")]
        public string Participators { get; set; } // Participators
		/// <summary></summary>	
		[Description("")]
        public string ParticipatorsID { get; set; } // ParticipatorsID
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_Feedback : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string IsUse { get; set; } // IsUse
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string DealStatus { get; set; } // DealStatus
		/// <summary></summary>	
		[Description("")]
        public string DealResult { get; set; } // DealResult
		/// <summary></summary>	
		[Description("")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary></summary>	
		[Description("")]
        public string DealUserName { get; set; } // DealUserName
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Level { get; set; } // Level
		/// <summary>所属模块</summary>	
		[Description("所属模块")]
        public string Module { get; set; } // Module
		/// <summary>计划完成时间</summary>	
		[Description("计划完成时间")]
        public DateTime? ExpectedCompleteTime { get; set; } // ExpectedCompleteTime
		/// <summary>问题性质</summary>	
		[Description("问题性质")]
        public string ProblemNature { get; set; } // ProblemNature
		/// <summary></summary>	
		[Description("")]
        public string ProjectPrincipal { get; set; } // ProjectPrincipal
		/// <summary></summary>	
		[Description("")]
        public string ProjectDept { get; set; } // ProjectDept
		/// <summary></summary>	
		[Description("")]
        public DateTime? ConfirmProblemsTime { get; set; } // ConfirmProblemsTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? PlanCompleteTime { get; set; } // PlanCompleteTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ActualCompleteTime { get; set; } // ActualCompleteTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ConfirmCompleteTime { get; set; } // ConfirmCompleteTime
		/// <summary></summary>	
		[Description("")]
        public string ConfirmProblemsUserID { get; set; } // ConfirmProblemsUserID
		/// <summary></summary>	
		[Description("")]
        public string ConfirmProblemsUserName { get; set; } // ConfirmProblemsUserName
		/// <summary></summary>	
		[Description("")]
        public string ConfirmCompleteUserID { get; set; } // ConfirmCompleteUserID
		/// <summary></summary>	
		[Description("")]
        public string ConfirmCompleteUserName { get; set; } // ConfirmCompleteUserName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_ShortCut : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary>图标的路径</summary>	
		[Description("图标的路径")]
        public string IconImage { get; set; } // IconImage
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsUse { get; set; } // IsUse
		/// <summary></summary>	
		[Description("")]
        public int? PageIndex { get; set; } // PageIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_FriendLink : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Icon { get; set; } // Icon
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string DeptId { get; set; } // DeptId
		/// <summary></summary>	
		[Description("")]
        public string DeptName { get; set; } // DeptName
		/// <summary></summary>	
		[Description("")]
        public string UserId { get; set; } // UserId
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_NewsImage : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string GroupID { get; set; } // GroupID
		/// <summary></summary>	
		[Description("")]
        public string PictureName { get; set; } // PictureName
		/// <summary></summary>	
		[Description("")]
        public byte[] PictureEntire { get; set; } // PictureEntire
		/// <summary></summary>	
		[Description("")]
        public byte[] PictureThumb { get; set; } // PictureThumb
		/// <summary>图片</summary>	
		[Description("图片")]
        public string Src { get; set; } // Src
		/// <summary>链接</summary>	
		[Description("链接")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>排序</summary>	
		[Description("排序")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary>创建日期</summary>	
		[Description("创建日期")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_NewsImageGroup : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorId { get; set; } // DeptDoorId
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorName { get; set; } // DeptDoorName
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_PublicInformation : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CatalogId { get; set; } // CatalogId
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string ContentText { get; set; } // ContentText
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
		/// <summary></summary>	
		[Description("")]
        public string ReceiveDeptId { get; set; } // ReceiveDeptId
		/// <summary></summary>	
		[Description("")]
        public string ReceiveDeptName { get; set; } // ReceiveDeptName
		/// <summary></summary>	
		[Description("")]
        public string ReceiveUserId { get; set; } // ReceiveUserId
		/// <summary></summary>	
		[Description("")]
        public string ReceiveUserName { get; set; } // ReceiveUserName
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorId { get; set; } // DeptDoorId
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorName { get; set; } // DeptDoorName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ExpiresTime { get; set; } // ExpiresTime
		/// <summary></summary>	
		[Description("")]
        public int? ReadCount { get; set; } // ReadCount
		/// <summary>重要度 1重要，0一般</summary>	
		[Description("重要度 1重要，0一般")]
        public string Important { get; set; } // Important
		/// <summary>紧急度 1重要，0一般</summary>	
		[Description("紧急度 1重要，0一般")]
        public string Urgency { get; set; } // Urgency
		/// <summary>置顶</summary>	
		[Description("置顶")]
        public string IsTop { get; set; } // IsTop
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string ReceiveRoleId { get; set; } // ReceiveRoleId
		/// <summary></summary>	
		[Description("")]
        public string ReceiveRoleName { get; set; } // ReceiveRoleName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_PublicInformCatalog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CatalogName { get; set; } // CatalogName
		/// <summary></summary>	
		[Description("")]
        public string CatalogKey { get; set; } // CatalogKey
		/// <summary></summary>	
		[Description("")]
        public string IsOnHomePage { get; set; } // IsOnHomePage
		/// <summary></summary>	
		[Description("")]
        public int? InHomePageNum { get; set; } // InHomePageNum
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary>创建日期</summary>	
		[Description("创建日期")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string IsPublic { get; set; } // IsPublic
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_L_LoginLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string UserAccount { get; set; } // UserAccount
		/// <summary></summary>	
		[Description("")]
        public DateTime? LoginDate { get; set; } // LoginDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? LoginTime { get; set; } // LoginTime
		/// <summary></summary>	
		[Description("")]
        public string IPAddress { get; set; } // IPAddress
		/// <summary></summary>	
		[Description("")]
        public string Address { get; set; } // Address
		/// <summary></summary>	
		[Description("")]
        public string ComeForm { get; set; } // ComeForm
    }

	/// <summary>元数据分类表</summary>	
	[Description("元数据分类表")]
    public partial class S_M_Category : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全ID</summary>	
		[Description("全ID")]
        public string FullID { get; set; } // FullID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string CategoryCode { get; set; } // CategoryCode
		/// <summary></summary>	
		[Description("")]
        public string IconClass { get; set; } // IconClass
		/// <summary></summary>	
		[Description("")]
        public string IsUEditor { get; set; } // IsUEditor

        public S_M_Category()
        {
			IsUEditor = "T";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_M_ConfigManage : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string DbServerAddr { get; set; } // DbServerAddr
		/// <summary></summary>	
		[Description("")]
        public string DbName { get; set; } // DbName
		/// <summary></summary>	
		[Description("")]
        public string DbLoginName { get; set; } // DbLoginName
		/// <summary></summary>	
		[Description("")]
        public string DbPassword { get; set; } // DbPassword
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? SyncTime { get; set; } // SyncTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
    }

	/// <summary>枚举定义表</summary>	
	[Description("枚举定义表")]
    public partial class S_M_EnumDef : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>数据库连接名</summary>	
		[Description("数据库连接名")]
        public string ConnName { get; set; } // ConnName
		/// <summary>查询Sql</summary>	
		[Description("查询Sql")]
        public string Sql { get; set; } // Sql
		/// <summary>排序</summary>	
		[Description("排序")]
        public string Orderby { get; set; } // Orderby
		/// <summary>分类ID</summary>	
		[Description("分类ID")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get; set; } // CompanyID
		/// <summary></summary>	
		[Description("")]
        public string CompanyName { get; set; } // CompanyName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_M_EnumItem> S_M_EnumItem { get { onS_M_EnumItemGetting(); return _S_M_EnumItem;} }
		private ICollection<S_M_EnumItem> _S_M_EnumItem;
		partial void onS_M_EnumItemGetting();


        public S_M_EnumDef()
        {
			Type = "Normal";
			SortIndex = 0;
            _S_M_EnumItem = new List<S_M_EnumItem>();
        }
    }

	/// <summary>枚举表</summary>	
	[Description("枚举表")]
    public partial class S_M_EnumItem : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>枚举定义ID</summary>	
		[Description("枚举定义ID")]
        public string EnumDefID { get; set; } // EnumDefID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>子枚举编号</summary>	
		[Description("子枚举编号")]
        public string SubEnumDefCode { get; set; } // SubEnumDefCode
		/// <summary>枚举分类</summary>	
		[Description("枚举分类")]
        public string Category { get; set; } // Category
		/// <summary>枚举子分类</summary>	
		[Description("枚举子分类")]
        public string SubCategory { get; set; } // SubCategory
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN

        // Foreign keys
		[JsonIgnore]
        public virtual S_M_EnumDef S_M_EnumDef { get; set; } //  EnumDefID - FK_EnumItem_EnumDef

        public S_M_EnumItem()
        {
			SortIndex = 0.0;
        }
    }

	/// <summary>平台数据库字段表</summary>	
	[Description("平台数据库字段表")]
    public partial class S_M_Field : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>表ID</summary>	
		[Description("表ID")]
        public string TableID { get; set; } // TableID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description

        // Foreign keys
		[JsonIgnore]
        public virtual S_M_Table S_M_Table { get; set; } //  TableID - FK_S_M_Field_S_M_Table

        public S_M_Field()
        {
			SortIndex = 0;
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_M_Parameter : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Module { get; set; } // Module
		/// <summary></summary>	
		[Description("")]
        public string Category { get; set; } // Category
		/// <summary></summary>	
		[Description("")]
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string ParamType { get; set; } // ParamType
		/// <summary></summary>	
		[Description("")]
        public string CalType { get; set; } // CalType
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Expression { get; set; } // Expression
		/// <summary></summary>	
		[Description("")]
        public bool IsCollectionRef { get; set; } // IsCollectionRef
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string OrderBy { get; set; } // OrderBy
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public decimal SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
    }

	/// <summary>平台数据库表</summary>	
	[Description("平台数据库表")]
    public partial class S_M_Table : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>数据库连接名</summary>	
		[Description("数据库连接名")]
        public string ConnName { get; set; } // ConnName
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_M_Field> S_M_Field { get { onS_M_FieldGetting(); return _S_M_Field;} }
		private ICollection<S_M_Field> _S_M_Field;
		partial void onS_M_FieldGetting();


        public S_M_Table()
        {
            _S_M_Field = new List<S_M_Field>();
        }
    }

	/// <summary>四方文件日志查询</summary>	
	[Description("四方文件日志查询")]
    public partial class S_OEM_TaskFileList : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public long ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string OEMCode { get; set; } // OEMCode
		/// <summary>业务类型</summary>	
		[Description("业务类型")]
        public string BusinessCode { get; set; } // BusinessCode
		/// <summary></summary>	
		[Description("")]
        public string BusinessID { get; set; } // BusinessID
		/// <summary></summary>	
		[Description("")]
        public string MD5Code { get; set; } // MD5Code
		/// <summary>文件名</summary>	
		[Description("文件名")]
        public string FileName { get; set; } // FileName
		/// <summary>FsFileID</summary>	
		[Description("FsFileID")]
        public string FsFileID { get; set; } // FsFileID
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>同步状态</summary>	
		[Description("同步状态")]
        public string SyncState { get; set; } // SyncState
		/// <summary>同步时间</summary>	
		[Description("同步时间")]
        public DateTime? SyncTime { get; set; } // SyncTime
		/// <summary>请求URL</summary>	
		[Description("请求URL")]
        public string RequestUrl { get; set; } // RequestUrl
		/// <summary>请求数据</summary>	
		[Description("请求数据")]
        public string RequestData { get; set; } // RequestData
		/// <summary>应答</summary>	
		[Description("应答")]
        public string Response { get; set; } // Response
		/// <summary>错误消息</summary>	
		[Description("错误消息")]
        public string ErrorMsg { get; set; } // ErrorMsg
    }

	/// <summary>四方同步日志查询</summary>	
	[Description("四方同步日志查询")]
    public partial class S_OEM_TaskList : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public long ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string OEMCode { get; set; } // OEMCode
		/// <summary>业务动作</summary>	
		[Description("业务动作")]
        public string BusinessType { get; set; } // BusinessType
		/// <summary>业务类型</summary>	
		[Description("业务类型")]
        public string BusinessCode { get; set; } // BusinessCode
		/// <summary>业务ID</summary>	
		[Description("业务ID")]
        public string BusinessID { get; set; } // BusinessID
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>同步状态</summary>	
		[Description("同步状态")]
        public string SyncState { get; set; } // SyncState
		/// <summary>同步时间</summary>	
		[Description("同步时间")]
        public DateTime? SyncTime { get; set; } // SyncTime
		/// <summary>请求URL</summary>	
		[Description("请求URL")]
        public string RequestUrl { get; set; } // RequestUrl
		/// <summary>业务数据</summary>	
		[Description("业务数据")]
        public string RequestData { get; set; } // RequestData
		/// <summary>回应</summary>	
		[Description("回应")]
        public string Response { get; set; } // Response
		/// <summary>错误消息</summary>	
		[Description("错误消息")]
        public string ErrorMsg { get; set; } // ErrorMsg
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_DataSet : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DefineID { get; set; } // DefineID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string Sql { get; set; } // Sql

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_R_Field> S_R_Field { get { onS_R_FieldGetting(); return _S_R_Field;} }
		private ICollection<S_R_Field> _S_R_Field;
		partial void onS_R_FieldGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_R_Define S_R_Define { get; set; } //  DefineID - FK_S_R_DataSet_S_R_Define

        public S_R_DataSet()
        {
            _S_R_Field = new List<S_R_Field>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_Define : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_R_DataSet> S_R_DataSet { get { onS_R_DataSetGetting(); return _S_R_DataSet;} }
		private ICollection<S_R_DataSet> _S_R_DataSet;
		partial void onS_R_DataSetGetting();


        public S_R_Define()
        {
            _S_R_DataSet = new List<S_R_DataSet>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_Field : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DataSetID { get; set; } // DataSetID
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey

        // Foreign keys
		[JsonIgnore]
        public virtual S_R_DataSet S_R_DataSet { get; set; } //  DataSetID - FK_S_R_Field_S_R_DataSet
    }

	/// <summary>AHCJ_BASE.S_RC_RULECODE</summary>	
	[Description("AHCJ_BASE.S_RC_RULECODE")]
    public partial class S_RC_RuleCode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string RuleName { get; set; } // RuleName
		/// <summary></summary>	
		[Description("")]
        public string Prefix { get; set; } // Prefix
		/// <summary></summary>	
		[Description("")]
        public string PostFix { get; set; } // PostFix
		/// <summary></summary>	
		[Description("")]
        public string Seperative { get; set; } // Seperative
		/// <summary></summary>	
		[Description("")]
        public int? Digit { get; set; } // Digit
		/// <summary></summary>	
		[Description("")]
        public int? StartNumber { get; set; } // StartNumber
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
    }

	/// <summary>AHCJ_BASE.S_RC_RULECODEDATA</summary>	
	[Description("AHCJ_BASE.S_RC_RULECODEDATA")]
    public partial class S_RC_RuleCodeData : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public long? Year { get; set; } // Year
		/// <summary></summary>	
		[Description("")]
        public long? AutoNumber { get; set; } // AutoNumber
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_S_Alarm : Formula.BaseModel
    {
		/// <summary>报警ID</summary>	
		[Description("报警ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>重要度:枚举1,0(重要不重要)</summary>	
		[Description("重要度:枚举1,0(重要不重要)")]
        public string Important { get; set; } // Important
		/// <summary>紧急度:枚举1,0 (紧急不紧急)</summary>	
		[Description("紧急度:枚举1,0 (紧急不紧急)")]
        public string Urgency { get; set; } // Urgency
		/// <summary>报警类型</summary>	
		[Description("报警类型")]
        public string AlarmType { get; set; } // AlarmType
		/// <summary>标题</summary>	
		[Description("标题")]
        public string AlarmTitle { get; set; } // AlarmTitle
		/// <summary>正文内容</summary>	
		[Description("正文内容")]
        public string AlarmContent { get; set; } // AlarmContent
		/// <summary>链接地址</summary>	
		[Description("链接地址")]
        public string AlarmUrl { get; set; } // AlarmUrl
		/// <summary>拥有人</summary>	
		[Description("拥有人")]
        public string OwnerName { get; set; } // OwnerName
		/// <summary>拥有人ID</summary>	
		[Description("拥有人ID")]
        public string OwnerID { get; set; } // OwnerID
		/// <summary>提醒时间</summary>	
		[Description("提醒时间")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary>事务截止完成时间</summary>	
		[Description("事务截止完成时间")]
        public DateTime? DeadlineTime { get; set; } // DeadlineTime
		/// <summary></summary>	
		[Description("")]
        public string SenderName { get; set; } // SenderName
		/// <summary></summary>	
		[Description("")]
        public string SenderID { get; set; } // SenderID
		/// <summary></summary>	
		[Description("")]
        public string IsDelete { get; set; } // IsDelete
		/// <summary></summary>	
		[Description("")]
        public string ProjectInfoID { get; set; } // ProjectInfoID
		/// <summary></summary>	
		[Description("")]
        public string FormCode { get; set; } // FormCode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_S_AlarmConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Connection { get; set; } // Connection
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string PlanDateTimeField { get; set; } // PlanDateTimeField
		/// <summary></summary>	
		[Description("")]
        public string FinishDateTimeField { get; set; } // FinishDateTimeField
		/// <summary></summary>	
		[Description("")]
        public string IsImportant { get; set; } // IsImportant
		/// <summary></summary>	
		[Description("")]
        public string IsUrgency { get; set; } // IsUrgency
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string ContentTemplate { get; set; } // ContentTemplate
		/// <summary></summary>	
		[Description("")]
        public string ProjectIDField { get; set; } // ProjectIDField
		/// <summary></summary>	
		[Description("")]
        public string LinkURL { get; set; } // LinkURL
		/// <summary></summary>	
		[Description("")]
        public string Frequency { get; set; } // Frequency
		/// <summary></summary>	
		[Description("")]
        public string Condition { get; set; } // Condition
		/// <summary></summary>	
		[Description("")]
        public string AlarmMode { get; set; } // AlarmMode
		/// <summary></summary>	
		[Description("")]
        public string OtherDataSource { get; set; } // OtherDataSource
		/// <summary></summary>	
		[Description("")]
        public string Receivers { get; set; } // Receivers
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public DateTime? LastAlarmDate { get; set; } // LastAlarmDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ReceiverIDField { get; set; } // ReceiverIDField
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_S_MsgBody : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string ContentText { get; set; } // ContentText
		/// <summary></summary>	
		[Description("")]
        public string AttachFileIDs { get; set; } // AttachFileIDs
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public string IsSystemMsg { get; set; } // IsSystemMsg
		/// <summary></summary>	
		[Description("")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary></summary>	
		[Description("")]
        public string SenderID { get; set; } // SenderID
		/// <summary></summary>	
		[Description("")]
        public string SenderName { get; set; } // SenderName
		/// <summary></summary>	
		[Description("")]
        public string ReceiverIDs { get; set; } // ReceiverIDs
		/// <summary></summary>	
		[Description("")]
        public string ReceiverNames { get; set; } // ReceiverNames
		/// <summary></summary>	
		[Description("")]
        public string ReceiverDeptIDs { get; set; } // ReceiverDeptIDs
		/// <summary></summary>	
		[Description("")]
        public string ReceiverDeptNames { get; set; } // ReceiverDeptNames
		/// <summary></summary>	
		[Description("")]
        public string ReceiverRoleIDs { get; set; } // ReceiverRoleIDs
		/// <summary></summary>	
		[Description("")]
        public string ReceiverRoleNames { get; set; } // ReceiverRoleNames
		/// <summary></summary>	
		[Description("")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary></summary>	
		[Description("")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary></summary>	
		[Description("")]
        public string IsReadReceipt { get; set; } // IsReadReceipt
		/// <summary></summary>	
		[Description("")]
        public string Importance { get; set; } // Importance
		/// <summary></summary>	
		[Description("")]
        public string IsCollect { get; set; } // IsCollect
		/// <summary></summary>	
		[Description("")]
        public DateTime? CollectTime { get; set; } // CollectTime
		/// <summary></summary>	
		[Description("")]
        public string FlowMsgID { get; set; } // FlowMsgID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_S_MsgReceiver> S_S_MsgReceiver { get { onS_S_MsgReceiverGetting(); return _S_S_MsgReceiver;} }
		private ICollection<S_S_MsgReceiver> _S_S_MsgReceiver;
		partial void onS_S_MsgReceiverGetting();


        public S_S_MsgBody()
        {
			IsSystemMsg = "0";
			IsDeleted = "0";
			IsReadReceipt = "0";
			Importance = "0";
			IsCollect = "0";
            _S_S_MsgReceiver = new List<S_S_MsgReceiver>();
        }
    }

	/// <summary>短消息接收人表</summary>	
	[Description("短消息接收人表")]
    public partial class S_S_MsgReceiver : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>消息体ID</summary>	
		[Description("消息体ID")]
        public string MsgBodyID { get; set; } // MsgBodyID
		/// <summary>接收人ID</summary>	
		[Description("接收人ID")]
        public string UserID { get; set; } // UserID
		/// <summary>接收人姓名</summary>	
		[Description("接收人姓名")]
        public string UserName { get; set; } // UserName
		/// <summary>首次查看时间</summary>	
		[Description("首次查看时间")]
        public DateTime? FirstViewTime { get; set; } // FirstViewTime
		/// <summary>回复时间</summary>	
		[Description("回复时间")]
        public DateTime? ReplyTime { get; set; } // ReplyTime
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary></summary>	
		[Description("")]
        public string IsCollect { get; set; } // IsCollect
		/// <summary></summary>	
		[Description("")]
        public DateTime? CollectTime { get; set; } // CollectTime

        // Foreign keys
		[JsonIgnore]
        public virtual S_S_MsgBody S_S_MsgBody { get; set; } //  MsgBodyID - FK_S_S_MsgReceiver_S_S_MsgBody

        public S_S_MsgReceiver()
        {
			IsDeleted = "0";
			IsCollect = "0";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_S_Notify : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string DeptId { get; set; } // DeptId
		/// <summary></summary>	
		[Description("")]
        public string UserId { get; set; } // UserId
		/// <summary></summary>	
		[Description("")]
        public string Sql { get; set; } // Sql
		/// <summary></summary>	
		[Description("")]
        public string HTML { get; set; } // HTML
		/// <summary></summary>	
		[Description("")]
        public string IsEnabled { get; set; } // IsEnabled
		/// <summary></summary>	
		[Description("")]
        public int? XOrder { get; set; } // XOrder
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark

        public S_S_Notify()
        {
			IsEnabled = "0";
        }
    }

	/// <summary>岗位岗级模板管理</summary>	
	[Description("岗位岗级模板管理")]
    public partial class S_S_PostLevelTemplate : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID
		/// <summary></summary>	
		[Description("")]
        public string FlowPhase { get; set; } // FlowPhase
		/// <summary></summary>	
		[Description("")]
        public string StepName { get; set; } // StepName
		/// <summary>年份</summary>	
		[Description("年份")]
        public string BelongYear { get; set; } // BelongYear
		/// <summary>月份</summary>	
		[Description("月份")]
        public string BelongMonth { get; set; } // BelongMonth
		/// <summary>季度</summary>	
		[Description("季度")]
        public string BelongQuarter { get; set; } // BelongQuarter
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>模板编号</summary>	
		[Description("模板编号")]
        public string TemplateCode { get; set; } // TemplateCode
		/// <summary>模板名称</summary>	
		[Description("模板名称")]
        public string TemplateName { get; set; } // TemplateName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_S_PostLevelTemplate_PostList> S_S_PostLevelTemplate_PostList { get { onS_S_PostLevelTemplate_PostListGetting(); return _S_S_PostLevelTemplate_PostList;} }
		private ICollection<S_S_PostLevelTemplate_PostList> _S_S_PostLevelTemplate_PostList;
		partial void onS_S_PostLevelTemplate_PostListGetting();


        public S_S_PostLevelTemplate()
        {
            _S_S_PostLevelTemplate_PostList = new List<S_S_PostLevelTemplate_PostList>();
        }
    }

	/// <summary>岗位岗级字表</summary>	
	[Description("岗位岗级字表")]
    public partial class S_S_PostLevelTemplate_PostList : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string S_S_PostLevelTemplateID { get; set; } // S_S_PostLevelTemplateID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>岗位</summary>	
		[Description("岗位")]
        public string PostCode { get; set; } // PostCode
		/// <summary>岗级</summary>	
		[Description("岗级")]
        public string PostLevelCode { get; set; } // PostLevelCode
		/// <summary>备注</summary>	
		[Description("备注")]
        public string BZ { get; set; } // BZ
		/// <summary>年份</summary>	
		[Description("年份")]
        public string BelongYear { get; set; } // BelongYear
		/// <summary>月份</summary>	
		[Description("月份")]
        public string BelongMonth { get; set; } // BelongMonth
		/// <summary>季度</summary>	
		[Description("季度")]
        public string BelongQuarter { get; set; } // BelongQuarter

        // Foreign keys
		[JsonIgnore]
        public virtual S_S_PostLevelTemplate S_S_PostLevelTemplate { get; set; } //  S_S_PostLevelTemplateID - FK_S_S_PostLevelTemplate_PostList_S_S_PostLevelTemplate
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_Task : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TaskExecID { get; set; } // TaskExecID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ExecUserIDs { get; set; } // ExecUserIDs
		/// <summary></summary>	
		[Description("")]
        public string ExecUserNames { get; set; } // ExecUserNames
		/// <summary></summary>	
		[Description("")]
        public DateTime? DeadLine { get; set; } // DeadLine
		/// <summary></summary>	
		[Description("")]
        public string Urgency { get; set; } // Urgency
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public DateTime? CompleteTime { get; set; } // CompleteTime
		/// <summary></summary>	
		[Description("")]
        public string Score { get; set; } // Score
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? UpdateTime { get; set; } // UpdateTime

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_TaskExec> S_T_TaskExec { get { onS_T_TaskExecGetting(); return _S_T_TaskExec;} }
		private ICollection<S_T_TaskExec> _S_T_TaskExec;
		partial void onS_T_TaskExecGetting();


        public S_T_Task()
        {
            _S_T_TaskExec = new List<S_T_TaskExec>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_TaskExec : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TaskID { get; set; } // TaskID
		/// <summary></summary>	
		[Description("")]
        public string ExecUserID { get; set; } // ExecUserID
		/// <summary></summary>	
		[Description("")]
        public string ExecUserName { get; set; } // ExecUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ExecTime { get; set; } // ExecTime
		/// <summary></summary>	
		[Description("")]
        public string ExecDescription { get; set; } // ExecDescription
		/// <summary></summary>	
		[Description("")]
        public string ExecAttachment { get; set; } // ExecAttachment
		/// <summary></summary>	
		[Description("")]
        public string ExecScore { get; set; } // ExecScore
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_Task S_T_Task { get; set; } //  TaskID - FK_S_T_TaskExec_S_T_Task
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_BIConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Layout { get; set; } // Layout
		/// <summary></summary>	
		[Description("")]
        public string Blocks { get; set; } // Blocks
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Component : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Json { get; set; } // Json
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_DataSource : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string Fields { get; set; } // Fields
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_ExcelImport : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string LayoutField { get; set; } // LayoutField
		/// <summary></summary>	
		[Description("")]
        public string DataRule { get; set; } // DataRule
		/// <summary></summary>	
		[Description("")]
        public string Condition { get; set; } // Condition
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_ExcelPrint : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Form : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Category { get; set; } // Category
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public string FlowLogic { get; set; } // FlowLogic
		/// <summary></summary>	
		[Description("")]
        public string HiddenFields { get; set; } // HiddenFields
		/// <summary></summary>	
		[Description("")]
        public string Layout { get; set; } // Layout
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string Setttings { get; set; } // Setttings
		/// <summary></summary>	
		[Description("")]
        public string SerialNumberSettings { get; set; } // SerialNumberSettings
		/// <summary></summary>	
		[Description("")]
        public string DefaultValueSettings { get; set; } // DefaultValueSettings
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ReleaseTime { get; set; } // ReleaseTime
		/// <summary></summary>	
		[Description("")]
        public string ReleasedData { get; set; } // ReleasedData
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>冲突状态</summary>	
		[Description("冲突状态")]
        public string Collision { get; set; } // Collision
		/// <summary></summary>	
		[Description("")]
        public string LayoutEN { get; set; } // LayoutEN
		/// <summary></summary>	
		[Description("")]
        public string MobileItems { get; set; } // MobileItems
		/// <summary></summary>	
		[Description("")]
        public string IsPrint { get; set; } // IsPrint
		/// <summary></summary>	
		[Description("")]
        public string MobileListSql { get; set; } // MobileListSql
		/// <summary></summary>	
		[Description("")]
        public string ValidateUnique { get; set; } // ValidateUnique
		/// <summary></summary>	
		[Description("")]
        public DateTime? VersionEndDate { get; set; } // VersionEndDate
		/// <summary></summary>	
		[Description("")]
        public int? VersionNum { get; set; } // VersionNum
		/// <summary></summary>	
		[Description("")]
        public string VersionDesc { get; set; } // VersionDesc
		/// <summary></summary>	
		[Description("")]
        public DateTime? VersionStartDate { get; set; } // VersionStartDate
		/// <summary></summary>	
		[Description("")]
        public string WebPrintJS { get; set; } // WebPrintJS
		/// <summary></summary>	
		[Description("")]
        public string LayoutPrint { get; set; } // LayoutPrint
		/// <summary></summary>	
		[Description("")]
        public string IsUEditor { get; set; } // IsUEditor
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get; set; } // CompanyID
		/// <summary></summary>	
		[Description("")]
        public string CompanyName { get; set; } // CompanyName
		/// <summary></summary>	
		[Description("")]
        public string CalItems { get; set; } // CalItems
		/// <summary></summary>	
		[Description("")]
        public string MobileScriptText { get; set; } // MobileScriptText

        public S_UI_Form()
        {
			ValidateUnique = "multi";
			IsUEditor = "T";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_FreePivot : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string DataSource { get; set; } // DataSource
		/// <summary></summary>	
		[Description("")]
        public string Enum { get; set; } // Enum
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_UI_FreePivotInstance> S_UI_FreePivotInstance { get { onS_UI_FreePivotInstanceGetting(); return _S_UI_FreePivotInstance;} }
		private ICollection<S_UI_FreePivotInstance> _S_UI_FreePivotInstance;
		partial void onS_UI_FreePivotInstanceGetting();


        public S_UI_FreePivot()
        {
            _S_UI_FreePivotInstance = new List<S_UI_FreePivotInstance>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_FreePivotInstance : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FreePivotID { get; set; } // FreePivotID
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string DataSourceName { get; set; } // DataSourceName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string RowItems { get; set; } // RowItems
		/// <summary></summary>	
		[Description("")]
        public string ColumnItems { get; set; } // ColumnItems
		/// <summary></summary>	
		[Description("")]
        public string DataItems { get; set; } // DataItems
		/// <summary></summary>	
		[Description("")]
        public string FilterItems { get; set; } // FilterItems

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_UI_FreePivotInstanceUser> S_UI_FreePivotInstanceUser { get { onS_UI_FreePivotInstanceUserGetting(); return _S_UI_FreePivotInstanceUser;} }
		private ICollection<S_UI_FreePivotInstanceUser> _S_UI_FreePivotInstanceUser;
		partial void onS_UI_FreePivotInstanceUserGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_UI_FreePivot S_UI_FreePivot { get; set; } //  FreePivotID - FK_S_UI_FreePivotInstance_S_UI_FreePivot

        public S_UI_FreePivotInstance()
        {
            _S_UI_FreePivotInstanceUser = new List<S_UI_FreePivotInstanceUser>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_FreePivotInstanceUser : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FreePivotInstanceID { get; set; } // FreePivotInstanceID
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string RowItems { get; set; } // RowItems
		/// <summary></summary>	
		[Description("")]
        public string ColumnItems { get; set; } // ColumnItems
		/// <summary></summary>	
		[Description("")]
        public string DataItems { get; set; } // DataItems
		/// <summary></summary>	
		[Description("")]
        public string FilterItems { get; set; } // FilterItems

        // Foreign keys
		[JsonIgnore]
        public virtual S_UI_FreePivotInstance S_UI_FreePivotInstance { get; set; } //  FreePivotInstanceID - FK_S_UI_FreePivotInstanceUser_S_UI_FreePivotInstance
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Help : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string HelpPageType { get; set; } // HelpPageType
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string HelpUrl { get; set; } // HelpUrl
		/// <summary></summary>	
		[Description("")]
        public string Layout { get; set; } // Layout
		/// <summary></summary>	
		[Description("")]
        public string HelpFile { get; set; } // HelpFile
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime

        public S_UI_Help()
        {
			HelpPageType = "page";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_JinGeSign : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FormID { get; set; } // FormID
		/// <summary></summary>	
		[Description("")]
        public string signatureid { get; set; } // signatureid
		/// <summary></summary>	
		[Description("")]
        public string signatureData { get; set; } // signatureData
		/// <summary></summary>	
		[Description("")]
        public string signUserId { get; set; } // signUserId
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Layout : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Json { get; set; } // Json
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string Height { get; set; } // Height
		/// <summary></summary>	
		[Description("")]
        public string JS { get; set; } // JS
		/// <summary></summary>	
		[Description("")]
        public string EnumKeys { get; set; } // EnumKeys
		/// <summary></summary>	
		[Description("")]
        public string EnumNames { get; set; } // EnumNames
		/// <summary></summary>	
		[Description("")]
        public string ParameterEnumKeys { get; set; } // ParameterEnumKeys
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public string HasRowNumber { get; set; } // HasRowNumber
		/// <summary></summary>	
		[Description("")]
        public string LayoutGrid { get; set; } // LayoutGrid
		/// <summary></summary>	
		[Description("")]
        public string LayoutField { get; set; } // LayoutField
		/// <summary></summary>	
		[Description("")]
        public string LayoutSearch { get; set; } // LayoutSearch
		/// <summary></summary>	
		[Description("")]
        public string LayoutButton { get; set; } // LayoutButton
		/// <summary></summary>	
		[Description("")]
        public string LayoutTab { get; set; } // LayoutTab
		/// <summary></summary>	
		[Description("")]
        public string Settings { get; set; } // Settings
		/// <summary></summary>	
		[Description("")]
        public string Released { get; set; } // Released
		/// <summary></summary>	
		[Description("")]
        public string RelevanceForm { get; set; } // RelevanceForm
		/// <summary></summary>	
		[Description("")]
        public string IDField { get; set; } // IDField
		/// <summary></summary>	
		[Description("")]
        public string TextField { get; set; } // TextField
		/// <summary></summary>	
		[Description("")]
        public string ParentField { get; set; } // ParentField
		/// <summary></summary>	
		[Description("")]
        public string IsExpand { get; set; } // IsExpand
		/// <summary></summary>	
		[Description("")]
        public string FormCode { get; set; } // FormCode
		/// <summary></summary>	
		[Description("")]
        public string DenyDeleteFlow { get; set; } // DenyDeleteFlow
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
		/// <summary></summary>	
		[Description("")]
        public string OrderBy { get; set; } // OrderBy
		/// <summary></summary>	
		[Description("")]
        public string HasCheckboxColumn { get; set; } // HasCheckboxColumn
		/// <summary></summary>	
		[Description("")]
        public string UseType { get; set; } // UseType
		/// <summary></summary>	
		[Description("")]
        public string Mode { get; set; } // Mode
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_List : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public string HasRowNumber { get; set; } // HasRowNumber
		/// <summary></summary>	
		[Description("")]
        public string LayoutGrid { get; set; } // LayoutGrid
		/// <summary></summary>	
		[Description("")]
        public string LayoutField { get; set; } // LayoutField
		/// <summary></summary>	
		[Description("")]
        public string LayoutSearch { get; set; } // LayoutSearch
		/// <summary></summary>	
		[Description("")]
        public string LayoutButton { get; set; } // LayoutButton
		/// <summary></summary>	
		[Description("")]
        public string Settings { get; set; } // Settings
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string Released { get; set; } // Released
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string DenyDeleteFlow { get; set; } // DenyDeleteFlow
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
		/// <summary></summary>	
		[Description("")]
        public string OrderBy { get; set; } // OrderBy
		/// <summary></summary>	
		[Description("")]
        public string HasCheckboxColumn { get; set; } // HasCheckboxColumn
		/// <summary></summary>	
		[Description("")]
        public string DefaultValueSettings { get; set; } // DefaultValueSettings
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get; set; } // CompanyID
		/// <summary></summary>	
		[Description("")]
        public string CompanyName { get; set; } // CompanyName

        public S_UI_List()
        {
			HasCheckboxColumn = "0";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_ModifyLogLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public DateTime? IOTime { get; set; } // IOTime
		/// <summary></summary>	
		[Description("")]
        public string IOType { get; set; } // IOType
		/// <summary></summary>	
		[Description("")]
        public string RelateData { get; set; } // RelateData
		/// <summary></summary>	
		[Description("")]
        public string CategoryName { get; set; } // CategoryName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Pivot : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string RowItems { get; set; } // RowItems
		/// <summary></summary>	
		[Description("")]
        public string ColumnItems { get; set; } // ColumnItems
		/// <summary></summary>	
		[Description("")]
        public string DataItems { get; set; } // DataItems
		/// <summary></summary>	
		[Description("")]
        public string FilterItems { get; set; } // FilterItems
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_UI_PivotUser> S_UI_PivotUser { get { onS_UI_PivotUserGetting(); return _S_UI_PivotUser;} }
		private ICollection<S_UI_PivotUser> _S_UI_PivotUser;
		partial void onS_UI_PivotUserGetting();


        public S_UI_Pivot()
        {
            _S_UI_PivotUser = new List<S_UI_PivotUser>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_PivotUser : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string PivotID { get; set; } // PivotID
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string RowItems { get; set; } // RowItems
		/// <summary></summary>	
		[Description("")]
        public string ColumnItems { get; set; } // ColumnItems
		/// <summary></summary>	
		[Description("")]
        public string DataItems { get; set; } // DataItems
		/// <summary></summary>	
		[Description("")]
        public string FilterItems { get; set; } // FilterItems

        // Foreign keys
		[JsonIgnore]
        public virtual S_UI_Pivot S_UI_Pivot { get; set; } //  PivotID - FK_S_UI_PivotUser_S_UI_Pivot
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_RoleRes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UseID { get; set; } // UseID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string FullPath { get; set; } // FullPath
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string IsInvalid { get; set; } // IsInvalid
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Selector : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string URLSingle { get; set; } // URLSingle
		/// <summary></summary>	
		[Description("")]
        public string URLMulti { get; set; } // URLMulti
		/// <summary></summary>	
		[Description("")]
        public string Width { get; set; } // Width
		/// <summary></summary>	
		[Description("")]
        public string Height { get; set; } // Height
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_SerialNumber : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string YearCode { get; set; } // YearCode
		/// <summary></summary>	
		[Description("")]
        public string MonthCode { get; set; } // MonthCode
		/// <summary></summary>	
		[Description("")]
        public string DayCode { get; set; } // DayCode
		/// <summary></summary>	
		[Description("")]
        public string CategoryCode { get; set; } // CategoryCode
		/// <summary></summary>	
		[Description("")]
        public string SubCategoryCode { get; set; } // SubCategoryCode
		/// <summary></summary>	
		[Description("")]
        public string OrderNumCode { get; set; } // OrderNumCode
		/// <summary></summary>	
		[Description("")]
        public string PrjCode { get; set; } // PrjCode
		/// <summary></summary>	
		[Description("")]
        public string OrgCode { get; set; } // OrgCode
		/// <summary></summary>	
		[Description("")]
        public string UserCode { get; set; } // UserCode
		/// <summary></summary>	
		[Description("")]
        public int? Number { get; set; } // Number
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Word : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string Collision { get; set; } // Collision
		/// <summary></summary>	
		[Description("")]
        public int? VersionNum { get; set; } // VersionNum
		/// <summary></summary>	
		[Description("")]
        public string VersionDesc { get; set; } // VersionDesc
		/// <summary></summary>	
		[Description("")]
        public DateTime? VersionStartDate { get; set; } // VersionStartDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? VersionEndDate { get; set; } // VersionEndDate
		/// <summary></summary>	
		[Description("")]
        public string WordNameTmpl { get; set; } // WordNameTmpl
		/// <summary></summary>	
		[Description("")]
        public string CompanyID { get; set; } // CompanyID
		/// <summary></summary>	
		[Description("")]
        public string CompanyName { get; set; } // CompanyName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class tmp_ms_xx_S_A_AuthLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Operation { get; set; } // Operation
		/// <summary></summary>	
		[Description("")]
        public string OperationTarget { get; set; } // OperationTarget
		/// <summary></summary>	
		[Description("")]
        public string RelateData { get; set; } // RelateData
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ClientIP { get; set; } // ClientIP
    }


    // ************************************************************************
    // POCO Configuration

    // S_A__OrgRes
    internal partial class S_A__OrgResConfiguration : EntityTypeConfiguration<S_A__OrgRes>
    {
        public S_A__OrgResConfiguration()
        {
			ToTable("S_A__ORGRES");
            HasKey(x => new { x.ResID, x.OrgID });

            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__OrgRes).HasForeignKey(c => c.ResID); // FK_S_A__OrgRes_S_A_Res
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgRes).HasForeignKey(c => c.OrgID); // FK_S_A__OrgRes_S_A_Org
        }
    }

    // S_A__OrgRole
    internal partial class S_A__OrgRoleConfiguration : EntityTypeConfiguration<S_A__OrgRole>
    {
        public S_A__OrgRoleConfiguration()
        {
			ToTable("S_A__ORGROLE");
            HasKey(x => new { x.RoleID, x.OrgID });

            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__OrgRole).HasForeignKey(c => c.RoleID); // FK_A_OrgRole_ARole
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgRole).HasForeignKey(c => c.OrgID); // FK_A_OrgRole_AOrg
        }
    }

    // S_A__OrgRoleUser
    internal partial class S_A__OrgRoleUserConfiguration : EntityTypeConfiguration<S_A__OrgRoleUser>
    {
        public S_A__OrgRoleUserConfiguration()
        {
			ToTable("S_A__ORGROLEUSER");
            HasKey(x => new { x.OrgID, x.RoleID, x.UserID });

            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__OrgRoleUser).HasForeignKey(c => c.RoleID); // FK_S_A__OrgRoleUser_S_A_Role
        }
    }

    // S_A__OrgUser
    internal partial class S_A__OrgUserConfiguration : EntityTypeConfiguration<S_A__OrgUser>
    {
        public S_A__OrgUserConfiguration()
        {
			ToTable("S_A__ORGUSER");
            HasKey(x => new { x.OrgID, x.UserID });

            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgUser).HasForeignKey(c => c.OrgID); // FK_A_OrgUser_AOrg
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__OrgUser).HasForeignKey(c => c.UserID); // FK_A_OrgUser_AUser
        }
    }

    // S_A__RoleRes
    internal partial class S_A__RoleResConfiguration : EntityTypeConfiguration<S_A__RoleRes>
    {
        public S_A__RoleResConfiguration()
        {
			ToTable("S_A__ROLERES");
            HasKey(x => new { x.ResID, x.RoleID });

            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__RoleRes).HasForeignKey(c => c.ResID); // FK_S_A__RoleRes_S_A_Res
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__RoleRes).HasForeignKey(c => c.RoleID); // FK_S_A__RoleRes_S_A_Role
        }
    }

    // S_A__RoleUser
    internal partial class S_A__RoleUserConfiguration : EntityTypeConfiguration<S_A__RoleUser>
    {
        public S_A__RoleUserConfiguration()
        {
			ToTable("S_A__ROLEUSER");
            HasKey(x => new { x.RoleID, x.UserID });

            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__RoleUser).HasForeignKey(c => c.RoleID); // FK_A_RoleUser_ARole
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__RoleUser).HasForeignKey(c => c.UserID); // FK_A_RoleUser_AUser
        }
    }

    // S_A__UserRes
    internal partial class S_A__UserResConfiguration : EntityTypeConfiguration<S_A__UserRes>
    {
        public S_A__UserResConfiguration()
        {
			ToTable("S_A__USERRES");
            HasKey(x => new { x.UserID, x.ResID });

            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);
            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.DenyAuth).HasColumnName("DENYAUTH").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__UserRes).HasForeignKey(c => c.UserID); // FK_S_A__UserRes_S_A_User
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__UserRes).HasForeignKey(c => c.ResID); // FK_S_A__UserRes_S_A_Res
        }
    }

    // S_A_AuthCompany
    internal partial class S_A_AuthCompanyConfiguration : EntityTypeConfiguration<S_A_AuthCompany>
    {
        public S_A_AuthCompanyConfiguration()
        {
			ToTable("S_A_AUTHCOMPANY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
            Property(x => x.ResID).HasColumnName("RESID").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_AuthInfo
    internal partial class S_A_AuthInfoConfiguration : EntityTypeConfiguration<S_A_AuthInfo>
    {
        public S_A_AuthInfoConfiguration()
        {
			ToTable("S_A_AUTHINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.OrgRootFullID).HasColumnName("ORGROOTFULLID").IsOptional().HasMaxLength(50);
            Property(x => x.ResRootFullID).HasColumnName("RESROOTFULLID").IsOptional().HasMaxLength(50);
            Property(x => x.RoleGroupID).HasColumnName("ROLEGROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.UserGroupID).HasColumnName("USERGROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_A_AuthLevel
    internal partial class S_A_AuthLevelConfiguration : EntityTypeConfiguration<S_A_AuthLevel>
    {
        public S_A_AuthLevelConfiguration()
        {
			ToTable("S_A_AUTHLEVEL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.MenuRootFullID).HasColumnName("MENUROOTFULLID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.MenuRootName).HasColumnName("MENUROOTNAME").IsOptional().HasMaxLength(1073741823);
            Property(x => x.RuleRootFullID).HasColumnName("RULEROOTFULLID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.RuleRootName).HasColumnName("RULEROOTNAME").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CorpID).HasColumnName("CORPID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CorpName).HasColumnName("CORPNAME").IsOptional().HasMaxLength(1073741823);
        }
    }

    // S_A_AuthLog
    internal partial class S_A_AuthLogConfiguration : EntityTypeConfiguration<S_A_AuthLog>
    {
        public S_A_AuthLogConfiguration()
        {
			ToTable("S_A_AUTHLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Operation).HasColumnName("OPERATION").IsOptional().HasMaxLength(50);
            Property(x => x.OperationTarget).HasColumnName("OPERATIONTARGET").IsOptional();
            Property(x => x.RelateData).HasColumnName("RELATEDATA").IsOptional();
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ClientIP).HasColumnName("CLIENTIP").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_DefaultAdivce
    internal partial class S_A_DefaultAdivceConfiguration : EntityTypeConfiguration<S_A_DefaultAdivce>
    {
        public S_A_DefaultAdivceConfiguration()
        {
			ToTable("S_A_DEFAULTADIVCE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.Advice).HasColumnName("ADVICE").IsOptional().HasMaxLength(200);
        }
    }

    // S_A_Org
    internal partial class S_A_OrgConfiguration : EntityTypeConfiguration<S_A_Org>
    {
        public S_A_OrgConfiguration()
        {
			ToTable("S_A_ORG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.ShortName).HasColumnName("SHORTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Character).HasColumnName("CHARACTER").IsOptional().HasMaxLength(500);
            Property(x => x.Location).HasColumnName("LOCATION").IsOptional().HasMaxLength(50);
            Property(x => x.IsShow).HasColumnName("ISSHOW").IsOptional().HasMaxLength(50);
            Property(x => x.IsIndependentManagement).HasColumnName("ISINDEPENDENTMANAGEMENT").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_Portal
    internal partial class S_A_PortalConfiguration : EntityTypeConfiguration<S_A_Portal>
    {
        public S_A_PortalConfiguration()
        {
			ToTable("S_A_PORTAL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsRequired().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.MoreUrl).HasColumnName("MOREURL").IsOptional().HasMaxLength(500);
            Property(x => x.Height).HasColumnName("HEIGHT").IsOptional().HasMaxLength(10);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional();
            Property(x => x.DisplayType).HasColumnName("DISPLAYTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.PublicInformCatalog).HasColumnName("PUBLICINFORMCATALOG").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_PortalTemplet
    internal partial class S_A_PortalTempletConfiguration : EntityTypeConfiguration<S_A_PortalTemplet>
    {
        public S_A_PortalTempletConfiguration()
        {
			ToTable("S_A_PORTALTEMPLET");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsRequired().HasMaxLength(100);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional();
            Property(x => x.Rows).HasColumnName("ROWS").IsOptional().HasMaxLength(50);
            Property(x => x.Cols).HasColumnName("COLS").IsOptional().HasMaxLength(50);
            Property(x => x.ColsWidth).HasColumnName("COLSWIDTH").IsOptional().HasMaxLength(100);
            Property(x => x.IsEnabled).HasColumnName("ISENABLED").IsOptional();
            Property(x => x.Xorder).HasColumnName("XORDER").IsOptional();
            Property(x => x.IsNewPortal).HasColumnName("ISNEWPORTAL").IsOptional().HasMaxLength(1);
        }
    }

    // S_A_Res
    internal partial class S_A_ResConfiguration : EntityTypeConfiguration<S_A_Res>
    {
        public S_A_ResConfiguration()
        {
			ToTable("S_A_RES");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.IconUrl).HasColumnName("ICONURL").IsOptional().HasMaxLength(200);
            Property(x => x.Target).HasColumnName("TARGET").IsOptional().HasMaxLength(50);
            Property(x => x.ButtonID).HasColumnName("BUTTONID").IsOptional().HasMaxLength(2000);
            Property(x => x.DataFilter).HasColumnName("DATAFILTER").IsOptional().HasMaxLength(2000);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IconClass).HasColumnName("ICONCLASS").IsOptional().HasMaxLength(200);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(50);
            Property(x => x.MenuID).HasColumnName("MENUID").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_Role
    internal partial class S_A_RoleConfiguration : EntityTypeConfiguration<S_A_Role>
    {
        public S_A_RoleConfiguration()
        {
			ToTable("S_A_ROLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.OrgName).HasColumnName("ORGNAME").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(50);
            Property(x => x.CorpID).HasColumnName("CORPID").IsOptional().HasMaxLength(50);
            Property(x => x.CorpName).HasColumnName("CORPNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_Security
    internal partial class S_A_SecurityConfiguration : EntityTypeConfiguration<S_A_Security>
    {
        public S_A_SecurityConfiguration()
        {
			ToTable("S_A_SECURITY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SuperAdmin).HasColumnName("SUPERADMIN").IsOptional().HasMaxLength(50);
            Property(x => x.SuperAdminPwd).HasColumnName("SUPERADMINPWD").IsOptional().HasMaxLength(50);
            Property(x => x.SuperAdminSecurity).HasColumnName("SUPERADMINSECURITY").IsOptional().HasMaxLength(500);
            Property(x => x.SuperAdminModifyTime).HasColumnName("SUPERADMINMODIFYTIME").IsOptional();
            Property(x => x.AdminIDs).HasColumnName("ADMINIDS").IsOptional();
            Property(x => x.AdminNames).HasColumnName("ADMINNAMES").IsOptional();
            Property(x => x.AdminModifyTime).HasColumnName("ADMINMODIFYTIME").IsOptional();
            Property(x => x.AdminSecurity).HasColumnName("ADMINSECURITY").IsOptional().HasMaxLength(500);
        }
    }

    // S_A_TempletRole
    internal partial class S_A_TempletRoleConfiguration : EntityTypeConfiguration<S_A_TempletRole>
    {
        public S_A_TempletRoleConfiguration()
        {
			ToTable("S_A_TEMPLETROLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TempletID).HasColumnName("TEMPLETID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleID).HasColumnName("ROLEID").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional();
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_PortalTemplet).WithMany(b => b.S_A_TempletRole).HasForeignKey(c => c.TempletID); // FK_S_A_TempletRole_S_A_PortalTemplet
            HasOptional(a => a.S_A_Role).WithMany(b => b.S_A_TempletRole).HasForeignKey(c => c.RoleID); // FK_S_A_TempletRole_S_A_Role
        }
    }

    // S_A_User
    internal partial class S_A_UserConfiguration : EntityTypeConfiguration<S_A_User>
    {
        public S_A_UserConfiguration()
        {
			ToTable("S_A_USER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.WorkNo).HasColumnName("WORKNO").IsOptional().HasMaxLength(50);
            Property(x => x.Password).HasColumnName("PASSWORD").IsOptional().HasMaxLength(50);
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.InDate).HasColumnName("INDATE").IsOptional();
            Property(x => x.OutDate).HasColumnName("OUTDATE").IsOptional();
            Property(x => x.Phone).HasColumnName("PHONE").IsOptional().HasMaxLength(50);
            Property(x => x.MobilePhone).HasColumnName("MOBILEPHONE").IsOptional().HasMaxLength(50);
            Property(x => x.Email).HasColumnName("EMAIL").IsOptional().HasMaxLength(50);
            Property(x => x.Address).HasColumnName("ADDRESS").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.LastLoginTime).HasColumnName("LASTLOGINTIME").IsOptional().HasMaxLength(50);
            Property(x => x.LastLoginIP).HasColumnName("LASTLOGINIP").IsOptional().HasMaxLength(50);
            Property(x => x.LastSessionID).HasColumnName("LASTSESSIONID").IsOptional().HasMaxLength(50);
            Property(x => x.ErrorCount).HasColumnName("ERRORCOUNT").IsOptional();
            Property(x => x.ErrorTime).HasColumnName("ERRORTIME").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.PrjID).HasColumnName("PRJID").IsOptional().HasMaxLength(50);
            Property(x => x.PrjName).HasColumnName("PRJNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DeptID).HasColumnName("DEPTID").IsOptional().HasMaxLength(50);
            Property(x => x.DeptFullID).HasColumnName("DEPTFULLID").IsOptional().HasMaxLength(500);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.RTX).HasColumnName("RTX").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.Weixin).HasColumnName("WEIXIN").IsOptional().HasMaxLength(100);
            Property(x => x.SignPwd).HasColumnName("SIGNPWD").IsOptional().HasMaxLength(50);
            Property(x => x.PortalSettings).HasColumnName("PORTALSETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.CorpID).HasColumnName("CORPID").IsOptional().HasMaxLength(50);
            Property(x => x.CorpName).HasColumnName("CORPNAME").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(50);
            Property(x => x.SyncCloudTime).HasColumnName("SYNCCLOUDTIME").IsOptional();
            Property(x => x.Position).HasColumnName("POSITION").IsOptional().HasMaxLength(1000);
            Property(x => x.DingDing).HasColumnName("DINGDING").IsOptional().HasMaxLength(100);
            Property(x => x.AdminCompanyID).HasColumnName("ADMINCOMPANYID").IsOptional().HasMaxLength(50);
            Property(x => x.AdminCompanyName).HasColumnName("ADMINCOMPANYNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_UserImg
    internal partial class S_A_UserImgConfiguration : EntityTypeConfiguration<S_A_UserImg>
    {
        public S_A_UserImgConfiguration()
        {
			ToTable("S_A_USERIMG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.SignImg).HasColumnName("SIGNIMG").IsOptional().HasMaxLength(2147483647);
            Property(x => x.Picture).HasColumnName("PICTURE").IsOptional().HasMaxLength(2147483647);
            Property(x => x.DwgFile).HasColumnName("DWGFILE").IsOptional().HasMaxLength(2147483647);

            // Foreign keys
            HasOptional(a => a.S_A_User).WithMany(b => b.S_A_UserImg).HasForeignKey(c => c.UserID); // FK_S_A_UserImg_S_A_User
        }
    }

    // S_A_UserLinkMan
    internal partial class S_A_UserLinkManConfiguration : EntityTypeConfiguration<S_A_UserLinkMan>
    {
        public S_A_UserLinkManConfiguration()
        {
			ToTable("S_A_USERLINKMAN");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.LinkManID).HasColumnName("LINKMANID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_A_User).WithMany(b => b.S_A_UserLinkMan).HasForeignKey(c => c.UserID); // FK_S_A_UserLinkMan_S_A_User
        }
    }

    // S_C_Holiday
    internal partial class S_C_HolidayConfiguration : EntityTypeConfiguration<S_C_Holiday>
    {
        public S_C_HolidayConfiguration()
        {
			ToTable("S_C_HOLIDAY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Month).HasColumnName("MONTH").IsOptional();
            Property(x => x.Day).HasColumnName("DAY").IsOptional();
            Property(x => x.Date).HasColumnName("DATE").IsOptional();
            Property(x => x.DayOfWeek).HasColumnName("DAYOFWEEK").IsOptional().HasMaxLength(50);
            Property(x => x.IsHoliday).HasColumnName("ISHOLIDAY").IsOptional().HasMaxLength(1);
        }
    }

    // S_D_FormToPDFTask
    internal partial class S_D_FormToPDFTaskConfiguration : EntityTypeConfiguration<S_D_FormToPDFTask>
    {
        public S_D_FormToPDFTaskConfiguration()
        {
			ToTable("S_D_FORMTOPDFTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TempCode).HasColumnName("TEMPCODE").IsOptional().HasMaxLength(100);
            Property(x => x.FormID).HasColumnName("FORMID").IsOptional().HasMaxLength(100);
            Property(x => x.PDFFileID).HasColumnName("PDFFILEID").IsOptional().HasMaxLength(100);
            Property(x => x.FormLastModifyDate).HasColumnName("FORMLASTMODIFYDATE").IsOptional();
            Property(x => x.BeginTime).HasColumnName("BEGINTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.DoneLog).HasColumnName("DONELOG").IsOptional();
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.IMGFileIDs).HasColumnName("IMGFILEIDS").IsOptional().HasMaxLength(2000);
        }
    }

    // S_D_ModifyLog
    internal partial class S_D_ModifyLogConfiguration : EntityTypeConfiguration<S_D_ModifyLog>
    {
        public S_D_ModifyLogConfiguration()
        {
			ToTable("S_D_MODIFYLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(100);
            Property(x => x.ModifyMode).HasColumnName("MODIFYMODE").IsOptional().HasMaxLength(50);
            Property(x => x.EntityKey).HasColumnName("ENTITYKEY").IsOptional().HasMaxLength(200);
            Property(x => x.CurrentValue).HasColumnName("CURRENTVALUE").IsOptional();
            Property(x => x.OriginalValue).HasColumnName("ORIGINALVALUE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ClientIP).HasColumnName("CLIENTIP").IsOptional().HasMaxLength(50);
            Property(x => x.UserHostAddress).HasColumnName("USERHOSTADDRESS").IsOptional().HasMaxLength(50);
        }
    }

    // S_D_PushTask
    internal partial class S_D_PushTaskConfiguration : EntityTypeConfiguration<S_D_PushTask>
    {
        public S_D_PushTaskConfiguration()
        {
			ToTable("S_D_PUSHTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SourceID).HasColumnName("SOURCEID").IsOptional().HasMaxLength(50);
            Property(x => x.FormInstanceID).HasColumnName("FORMINSTANCEID").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(100);
            Property(x => x.ShortContent).HasColumnName("SHORTCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.SendUserID).HasColumnName("SENDUSERID").IsOptional();
            Property(x => x.SendUserName).HasColumnName("SENDUSERNAME").IsOptional();
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.SourceType).HasColumnName("SOURCETYPE").IsOptional().HasMaxLength(100);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional();
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional();
            Property(x => x.PushType).HasColumnName("PUSHTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.BeginTime).HasColumnName("BEGINTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.DoneLog).HasColumnName("DONELOG").IsOptional();
            Property(x => x.ClientID).HasColumnName("CLIENTID").IsOptional().HasMaxLength(50);
            Property(x => x.AppID).HasColumnName("APPID").IsOptional().HasMaxLength(50);
            Property(x => x.ChannelID).HasColumnName("CHANNELID").IsOptional().HasMaxLength(50);
            Property(x => x.DeviceOS).HasColumnName("DEVICEOS").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_AllFeedback
    internal partial class S_H_AllFeedbackConfiguration : EntityTypeConfiguration<S_H_AllFeedback>
    {
        public S_H_AllFeedbackConfiguration()
        {
			ToTable("S_H_ALLFEEDBACK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.Url).HasColumnName("URL").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional();
            Property(x => x.IsUse).HasColumnName("ISUSE").IsOptional().HasMaxLength(1);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DealStatus).HasColumnName("DEALSTATUS").IsOptional().HasMaxLength(50);
            Property(x => x.DealResult).HasColumnName("DEALRESULT").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DealUserName).HasColumnName("DEALUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Level).HasColumnName("LEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.Module).HasColumnName("MODULE").IsOptional().HasMaxLength(50);
            Property(x => x.ExpectedCompleteTime).HasColumnName("EXPECTEDCOMPLETETIME").IsOptional();
            Property(x => x.ProblemNature).HasColumnName("PROBLEMNATURE").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectPrincipal).HasColumnName("PROJECTPRINCIPAL").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectDept).HasColumnName("PROJECTDEPT").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmProblemsTime).HasColumnName("CONFIRMPROBLEMSTIME").IsOptional();
            Property(x => x.PlanCompleteTime).HasColumnName("PLANCOMPLETETIME").IsOptional();
            Property(x => x.ActualCompleteTime).HasColumnName("ACTUALCOMPLETETIME").IsOptional();
            Property(x => x.ConfirmCompleteTime).HasColumnName("CONFIRMCOMPLETETIME").IsOptional();
            Property(x => x.ConfirmProblemsUserID).HasColumnName("CONFIRMPROBLEMSUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmProblemsUserName).HasColumnName("CONFIRMPROBLEMSUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmCompleteUserID).HasColumnName("CONFIRMCOMPLETEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmCompleteUserName).HasColumnName("CONFIRMCOMPLETEUSERNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_Calendar
    internal partial class S_H_CalendarConfiguration : EntityTypeConfiguration<S_H_Calendar>
    {
        public S_H_CalendarConfiguration()
        {
			ToTable("S_H_CALENDAR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(4000);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(4000);
            Property(x => x.Grade).HasColumnName("GRADE").IsOptional().HasMaxLength(20);
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional().HasMaxLength(4000);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Sponsor).HasColumnName("SPONSOR").IsOptional().HasMaxLength(50);
            Property(x => x.SponsorID).HasColumnName("SPONSORID").IsOptional().HasMaxLength(50);
            Property(x => x.Participators).HasColumnName("PARTICIPATORS").IsOptional();
            Property(x => x.ParticipatorsID).HasColumnName("PARTICIPATORSID").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_Feedback
    internal partial class S_H_FeedbackConfiguration : EntityTypeConfiguration<S_H_Feedback>
    {
        public S_H_FeedbackConfiguration()
        {
			ToTable("S_H_FEEDBACK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.Url).HasColumnName("URL").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional();
            Property(x => x.IsUse).HasColumnName("ISUSE").IsOptional().HasMaxLength(1);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DealStatus).HasColumnName("DEALSTATUS").IsOptional().HasMaxLength(50);
            Property(x => x.DealResult).HasColumnName("DEALRESULT").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DealUserName).HasColumnName("DEALUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Level).HasColumnName("LEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.Module).HasColumnName("MODULE").IsOptional().HasMaxLength(50);
            Property(x => x.ExpectedCompleteTime).HasColumnName("EXPECTEDCOMPLETETIME").IsOptional();
            Property(x => x.ProblemNature).HasColumnName("PROBLEMNATURE").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectPrincipal).HasColumnName("PROJECTPRINCIPAL").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectDept).HasColumnName("PROJECTDEPT").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmProblemsTime).HasColumnName("CONFIRMPROBLEMSTIME").IsOptional();
            Property(x => x.PlanCompleteTime).HasColumnName("PLANCOMPLETETIME").IsOptional();
            Property(x => x.ActualCompleteTime).HasColumnName("ACTUALCOMPLETETIME").IsOptional();
            Property(x => x.ConfirmCompleteTime).HasColumnName("CONFIRMCOMPLETETIME").IsOptional();
            Property(x => x.ConfirmProblemsUserID).HasColumnName("CONFIRMPROBLEMSUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmProblemsUserName).HasColumnName("CONFIRMPROBLEMSUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmCompleteUserID).HasColumnName("CONFIRMCOMPLETEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ConfirmCompleteUserName).HasColumnName("CONFIRMCOMPLETEUSERNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_ShortCut
    internal partial class S_H_ShortCutConfiguration : EntityTypeConfiguration<S_H_ShortCut>
    {
        public S_H_ShortCutConfiguration()
        {
			ToTable("S_H_SHORTCUT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional();
            Property(x => x.IconImage).HasColumnName("ICONIMAGE").IsOptional().HasMaxLength(250);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsUse).HasColumnName("ISUSE").IsOptional().HasMaxLength(1);
            Property(x => x.PageIndex).HasColumnName("PAGEINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_FriendLink
    internal partial class S_I_FriendLinkConfiguration : EntityTypeConfiguration<S_I_FriendLink>
    {
        public S_I_FriendLinkConfiguration()
        {
			ToTable("S_I_FRIENDLINK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Icon).HasColumnName("ICON").IsOptional().HasMaxLength(100);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.DeptId).HasColumnName("DEPTID").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.UserId).HasColumnName("USERID").IsOptional().HasMaxLength(2000);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(4000);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_NewsImage
    internal partial class S_I_NewsImageConfiguration : EntityTypeConfiguration<S_I_NewsImage>
    {
        public S_I_NewsImageConfiguration()
        {
			ToTable("S_I_NEWSIMAGE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.PictureName).HasColumnName("PICTURENAME").IsOptional().HasMaxLength(500);
            Property(x => x.PictureEntire).HasColumnName("PICTUREENTIRE").IsOptional().HasMaxLength(2147483647);
            Property(x => x.PictureThumb).HasColumnName("PICTURETHUMB").IsOptional().HasMaxLength(2147483647);
            Property(x => x.Src).HasColumnName("SRC").IsOptional().HasMaxLength(500);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_NewsImageGroup
    internal partial class S_I_NewsImageGroupConfiguration : EntityTypeConfiguration<S_I_NewsImageGroup>
    {
        public S_I_NewsImageGroupConfiguration()
        {
			ToTable("S_I_NEWSIMAGEGROUP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptDoorId).HasColumnName("DEPTDOORID").IsOptional().HasMaxLength(200);
            Property(x => x.DeptDoorName).HasColumnName("DEPTDOORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_PublicInformation
    internal partial class S_I_PublicInformationConfiguration : EntityTypeConfiguration<S_I_PublicInformation>
    {
        public S_I_PublicInformationConfiguration()
        {
			ToTable("S_I_PUBLICINFORMATION");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CatalogId).HasColumnName("CATALOGID").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ContentText).HasColumnName("CONTENTTEXT").IsOptional();
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveDeptId).HasColumnName("RECEIVEDEPTID").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveDeptName).HasColumnName("RECEIVEDEPTNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveUserId).HasColumnName("RECEIVEUSERID").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveUserName).HasColumnName("RECEIVEUSERNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptDoorId).HasColumnName("DEPTDOORID").IsOptional().HasMaxLength(200);
            Property(x => x.DeptDoorName).HasColumnName("DEPTDOORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ExpiresTime).HasColumnName("EXPIRESTIME").IsOptional();
            Property(x => x.ReadCount).HasColumnName("READCOUNT").IsOptional();
            Property(x => x.Important).HasColumnName("IMPORTANT").IsOptional().HasMaxLength(50);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.IsTop).HasColumnName("ISTOP").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ReceiveRoleId).HasColumnName("RECEIVEROLEID").IsOptional().HasMaxLength(500);
            Property(x => x.ReceiveRoleName).HasColumnName("RECEIVEROLENAME").IsOptional().HasMaxLength(500);
        }
    }

    // S_I_PublicInformCatalog
    internal partial class S_I_PublicInformCatalogConfiguration : EntityTypeConfiguration<S_I_PublicInformCatalog>
    {
        public S_I_PublicInformCatalogConfiguration()
        {
			ToTable("S_I_PUBLICINFORMCATALOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CatalogName).HasColumnName("CATALOGNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CatalogKey).HasColumnName("CATALOGKEY").IsOptional().HasMaxLength(50);
            Property(x => x.IsOnHomePage).HasColumnName("ISONHOMEPAGE").IsOptional().HasMaxLength(1);
            Property(x => x.InHomePageNum).HasColumnName("INHOMEPAGENUM").IsOptional();
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.IsPublic).HasColumnName("ISPUBLIC").IsOptional().HasMaxLength(1);
        }
    }

    // S_L_LoginLog
    internal partial class S_L_LoginLogConfiguration : EntityTypeConfiguration<S_L_LoginLog>
    {
        public S_L_LoginLogConfiguration()
        {
			ToTable("S_L_LOGINLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.UserAccount).HasColumnName("USERACCOUNT").IsOptional().HasMaxLength(50);
            Property(x => x.LoginDate).HasColumnName("LOGINDATE").IsOptional();
            Property(x => x.LoginTime).HasColumnName("LOGINTIME").IsOptional();
            Property(x => x.IPAddress).HasColumnName("IPADDRESS").IsOptional().HasMaxLength(50);
            Property(x => x.Address).HasColumnName("ADDRESS").IsOptional().HasMaxLength(50);
            Property(x => x.ComeForm).HasColumnName("COMEFORM").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_Category
    internal partial class S_M_CategoryConfiguration : EntityTypeConfiguration<S_M_Category>
    {
        public S_M_CategoryConfiguration()
        {
			ToTable("S_M_CATEGORY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.CategoryCode).HasColumnName("CATEGORYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.IconClass).HasColumnName("ICONCLASS").IsOptional().HasMaxLength(50);
            Property(x => x.IsUEditor).HasColumnName("ISUEDITOR").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_ConfigManage
    internal partial class S_M_ConfigManageConfiguration : EntityTypeConfiguration<S_M_ConfigManage>
    {
        public S_M_ConfigManageConfiguration()
        {
			ToTable("S_M_CONFIGMANAGE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.DbServerAddr).HasColumnName("DBSERVERADDR").IsOptional().HasMaxLength(50);
            Property(x => x.DbName).HasColumnName("DBNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DbLoginName).HasColumnName("DBLOGINNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DbPassword).HasColumnName("DBPASSWORD").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.SyncTime).HasColumnName("SYNCTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_EnumDef
    internal partial class S_M_EnumDefConfiguration : EntityTypeConfiguration<S_M_EnumDef>
    {
        public S_M_EnumDefConfiguration()
        {
			ToTable("S_M_ENUMDEF");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Sql).HasColumnName("SQL").IsOptional().HasMaxLength(500);
            Property(x => x.Orderby).HasColumnName("ORDERBY").IsOptional().HasMaxLength(200);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_EnumItem
    internal partial class S_M_EnumItemConfiguration : EntityTypeConfiguration<S_M_EnumItem>
    {
        public S_M_EnumItemConfiguration()
        {
			ToTable("S_M_ENUMITEM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.EnumDefID).HasColumnName("ENUMDEFID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SubEnumDefCode).HasColumnName("SUBENUMDEFCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.SubCategory).HasColumnName("SUBCATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_M_EnumDef).WithMany(b => b.S_M_EnumItem).HasForeignKey(c => c.EnumDefID); // FK_EnumItem_EnumDef
        }
    }

    // S_M_Field
    internal partial class S_M_FieldConfiguration : EntityTypeConfiguration<S_M_Field>
    {
        public S_M_FieldConfiguration()
        {
			ToTable("S_M_FIELD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TableID).HasColumnName("TABLEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.S_M_Table).WithMany(b => b.S_M_Field).HasForeignKey(c => c.TableID); // FK_S_M_Field_S_M_Table
        }
    }

    // S_M_Parameter
    internal partial class S_M_ParameterConfiguration : EntityTypeConfiguration<S_M_Parameter>
    {
        public S_M_ParameterConfiguration()
        {
			ToTable("S_M_PARAMETER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Module).HasColumnName("MODULE").IsOptional().HasMaxLength(50);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ParamType).HasColumnName("PARAMTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.CalType).HasColumnName("CALTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Expression).HasColumnName("EXPRESSION").IsOptional();
            Property(x => x.IsCollectionRef).HasColumnName("ISCOLLECTIONREF").IsRequired();
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsRequired();
            Property(x => x.OrderBy).HasColumnName("ORDERBY").IsOptional().HasMaxLength(500);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired().HasPrecision(18,4);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_Table
    internal partial class S_M_TableConfiguration : EntityTypeConfiguration<S_M_Table>
    {
        public S_M_TableConfiguration()
        {
			ToTable("S_M_TABLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_OEM_TaskFileList
    internal partial class S_OEM_TaskFileListConfiguration : EntityTypeConfiguration<S_OEM_TaskFileList>
    {
        public S_OEM_TaskFileListConfiguration()
        {
			ToTable("S_OEM_TASKFILELIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OEMCode).HasColumnName("OEMCODE").IsOptional().HasMaxLength(50);
            Property(x => x.BusinessCode).HasColumnName("BUSINESSCODE").IsOptional().HasMaxLength(50);
            Property(x => x.BusinessID).HasColumnName("BUSINESSID").IsOptional().HasMaxLength(50);
            Property(x => x.MD5Code).HasColumnName("MD5CODE").IsOptional().HasMaxLength(500);
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(500);
            Property(x => x.FsFileID).HasColumnName("FSFILEID").IsOptional().HasMaxLength(500);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.SyncState).HasColumnName("SYNCSTATE").IsOptional().HasMaxLength(50);
            Property(x => x.SyncTime).HasColumnName("SYNCTIME").IsOptional();
            Property(x => x.RequestUrl).HasColumnName("REQUESTURL").IsOptional().HasMaxLength(200);
            Property(x => x.RequestData).HasColumnName("REQUESTDATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Response).HasColumnName("RESPONSE").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ErrorMsg).HasColumnName("ERRORMSG").IsOptional().HasMaxLength(1073741823);
        }
    }

    // S_OEM_TaskList
    internal partial class S_OEM_TaskListConfiguration : EntityTypeConfiguration<S_OEM_TaskList>
    {
        public S_OEM_TaskListConfiguration()
        {
			ToTable("S_OEM_TASKLIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OEMCode).HasColumnName("OEMCODE").IsOptional().HasMaxLength(50);
            Property(x => x.BusinessType).HasColumnName("BUSINESSTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.BusinessCode).HasColumnName("BUSINESSCODE").IsOptional().HasMaxLength(500);
            Property(x => x.BusinessID).HasColumnName("BUSINESSID").IsOptional().HasMaxLength(500);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.SyncState).HasColumnName("SYNCSTATE").IsOptional().HasMaxLength(500);
            Property(x => x.SyncTime).HasColumnName("SYNCTIME").IsOptional();
            Property(x => x.RequestUrl).HasColumnName("REQUESTURL").IsOptional().HasMaxLength(500);
            Property(x => x.RequestData).HasColumnName("REQUESTDATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Response).HasColumnName("RESPONSE").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ErrorMsg).HasColumnName("ERRORMSG").IsOptional().HasMaxLength(1073741823);
        }
    }

    // S_R_DataSet
    internal partial class S_R_DataSetConfiguration : EntityTypeConfiguration<S_R_DataSet>
    {
        public S_R_DataSetConfiguration()
        {
			ToTable("S_R_DATASET");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefineID).HasColumnName("DEFINEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(200);
            Property(x => x.Sql).HasColumnName("SQL").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_R_Define).WithMany(b => b.S_R_DataSet).HasForeignKey(c => c.DefineID); // FK_S_R_DataSet_S_R_Define
        }
    }

    // S_R_Define
    internal partial class S_R_DefineConfiguration : EntityTypeConfiguration<S_R_Define>
    {
        public S_R_DefineConfiguration()
        {
			ToTable("S_R_DEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
        }
    }

    // S_R_Field
    internal partial class S_R_FieldConfiguration : EntityTypeConfiguration<S_R_Field>
    {
        public S_R_FieldConfiguration()
        {
			ToTable("S_R_FIELD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DataSetID).HasColumnName("DATASETID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_R_DataSet).WithMany(b => b.S_R_Field).HasForeignKey(c => c.DataSetID); // FK_S_R_Field_S_R_DataSet
        }
    }

    // S_RC_RuleCode
    internal partial class S_RC_RuleCodeConfiguration : EntityTypeConfiguration<S_RC_RuleCode>
    {
        public S_RC_RuleCodeConfiguration()
        {
			ToTable("S_RC_RULECODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(72);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(100);
            Property(x => x.RuleName).HasColumnName("RULENAME").IsOptional().HasMaxLength(400);
            Property(x => x.Prefix).HasColumnName("PREFIX").IsOptional().HasMaxLength(100);
            Property(x => x.PostFix).HasColumnName("POSTFIX").IsOptional().HasMaxLength(100);
            Property(x => x.Seperative).HasColumnName("SEPERATIVE").IsOptional().HasMaxLength(100);
            Property(x => x.Digit).HasColumnName("DIGIT").IsOptional();
            Property(x => x.StartNumber).HasColumnName("STARTNUMBER").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(100);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(100);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
        }
    }

    // S_RC_RuleCodeData
    internal partial class S_RC_RuleCodeDataConfiguration : EntityTypeConfiguration<S_RC_RuleCodeData>
    {
        public S_RC_RuleCodeDataConfiguration()
        {
			ToTable("S_RC_RULECODEDATA");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(72);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(100);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.AutoNumber).HasColumnName("AUTONUMBER").IsOptional();
        }
    }

    // S_S_Alarm
    internal partial class S_S_AlarmConfiguration : EntityTypeConfiguration<S_S_Alarm>
    {
        public S_S_AlarmConfiguration()
        {
			ToTable("S_S_ALARM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Important).HasColumnName("IMPORTANT").IsOptional().HasMaxLength(50);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.AlarmType).HasColumnName("ALARMTYPE").IsOptional().HasMaxLength(100);
            Property(x => x.AlarmTitle).HasColumnName("ALARMTITLE").IsOptional().HasMaxLength(200);
            Property(x => x.AlarmContent).HasColumnName("ALARMCONTENT").IsOptional();
            Property(x => x.AlarmUrl).HasColumnName("ALARMURL").IsOptional().HasMaxLength(200);
            Property(x => x.OwnerName).HasColumnName("OWNERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.OwnerID).HasColumnName("OWNERID").IsOptional().HasMaxLength(50);
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.DeadlineTime).HasColumnName("DEADLINETIME").IsOptional();
            Property(x => x.SenderName).HasColumnName("SENDERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SenderID).HasColumnName("SENDERID").IsOptional().HasMaxLength(50);
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectInfoID).HasColumnName("PROJECTINFOID").IsOptional().HasMaxLength(50);
            Property(x => x.FormCode).HasColumnName("FORMCODE").IsOptional().HasMaxLength(50);
        }
    }

    // S_S_AlarmConfig
    internal partial class S_S_AlarmConfigConfiguration : EntityTypeConfiguration<S_S_AlarmConfig>
    {
        public S_S_AlarmConfigConfiguration()
        {
			ToTable("S_S_ALARMCONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Connection).HasColumnName("CONNECTION").IsRequired().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsRequired().HasMaxLength(50);
            Property(x => x.PlanDateTimeField).HasColumnName("PLANDATETIMEFIELD").IsRequired().HasMaxLength(50);
            Property(x => x.FinishDateTimeField).HasColumnName("FINISHDATETIMEFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.IsImportant).HasColumnName("ISIMPORTANT").IsOptional().HasMaxLength(50);
            Property(x => x.IsUrgency).HasColumnName("ISURGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional();
            Property(x => x.ContentTemplate).HasColumnName("CONTENTTEMPLATE").IsRequired();
            Property(x => x.ProjectIDField).HasColumnName("PROJECTIDFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.LinkURL).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.Frequency).HasColumnName("FREQUENCY").IsOptional().HasMaxLength(500);
            Property(x => x.Condition).HasColumnName("CONDITION").IsOptional();
            Property(x => x.AlarmMode).HasColumnName("ALARMMODE").IsRequired().HasMaxLength(50);
            Property(x => x.OtherDataSource).HasColumnName("OTHERDATASOURCE").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Receivers).HasColumnName("RECEIVERS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.LastAlarmDate).HasColumnName("LASTALARMDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ReceiverIDField).HasColumnName("RECEIVERIDFIELD").IsOptional().HasMaxLength(500);
        }
    }

    // S_S_MsgBody
    internal partial class S_S_MsgBodyConfiguration : EntityTypeConfiguration<S_S_MsgBody>
    {
        public S_S_MsgBodyConfiguration()
        {
			ToTable("S_S_MSGBODY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ContentText).HasColumnName("CONTENTTEXT").IsOptional();
            Property(x => x.AttachFileIDs).HasColumnName("ATTACHFILEIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.IsSystemMsg).HasColumnName("ISSYSTEMMSG").IsOptional().HasMaxLength(1);
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.SenderID).HasColumnName("SENDERID").IsOptional().HasMaxLength(50);
            Property(x => x.SenderName).HasColumnName("SENDERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ReceiverIDs).HasColumnName("RECEIVERIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiverNames).HasColumnName("RECEIVERNAMES").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiverDeptIDs).HasColumnName("RECEIVERDEPTIDS").IsOptional().HasMaxLength(4000);
            Property(x => x.ReceiverDeptNames).HasColumnName("RECEIVERDEPTNAMES").IsOptional().HasMaxLength(4000);
            Property(x => x.ReceiverRoleIDs).HasColumnName("RECEIVERROLEIDS").IsOptional().HasMaxLength(4000);
            Property(x => x.ReceiverRoleNames).HasColumnName("RECEIVERROLENAMES").IsOptional().HasMaxLength(4000);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.IsReadReceipt).HasColumnName("ISREADRECEIPT").IsRequired().HasMaxLength(1);
            Property(x => x.Importance).HasColumnName("IMPORTANCE").IsRequired().HasMaxLength(1);
            Property(x => x.IsCollect).HasColumnName("ISCOLLECT").IsOptional().HasMaxLength(1);
            Property(x => x.CollectTime).HasColumnName("COLLECTTIME").IsOptional();
            Property(x => x.FlowMsgID).HasColumnName("FLOWMSGID").IsOptional().HasMaxLength(50);
        }
    }

    // S_S_MsgReceiver
    internal partial class S_S_MsgReceiverConfiguration : EntityTypeConfiguration<S_S_MsgReceiver>
    {
        public S_S_MsgReceiverConfiguration()
        {
			ToTable("S_S_MSGRECEIVER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.MsgBodyID).HasColumnName("MSGBODYID").IsOptional().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.FirstViewTime).HasColumnName("FIRSTVIEWTIME").IsOptional();
            Property(x => x.ReplyTime).HasColumnName("REPLYTIME").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsOptional().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.IsCollect).HasColumnName("ISCOLLECT").IsOptional().HasMaxLength(1);
            Property(x => x.CollectTime).HasColumnName("COLLECTTIME").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_S_MsgBody).WithMany(b => b.S_S_MsgReceiver).HasForeignKey(c => c.MsgBodyID); // FK_S_S_MsgReceiver_S_S_MsgBody
        }
    }

    // S_S_Notify
    internal partial class S_S_NotifyConfiguration : EntityTypeConfiguration<S_S_Notify>
    {
        public S_S_NotifyConfiguration()
        {
			ToTable("S_S_NOTIFY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(800);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DeptId).HasColumnName("DEPTID").IsOptional().HasMaxLength(2000);
            Property(x => x.UserId).HasColumnName("USERID").IsOptional().HasMaxLength(2000);
            Property(x => x.Sql).HasColumnName("SQL").IsOptional().HasMaxLength(200);
            Property(x => x.HTML).HasColumnName("HTML").IsRequired().HasMaxLength(1073741823);
            Property(x => x.IsEnabled).HasColumnName("ISENABLED").IsOptional().HasMaxLength(10);
            Property(x => x.XOrder).HasColumnName("XORDER").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(2000);
        }
    }

    // S_S_PostLevelTemplate
    internal partial class S_S_PostLevelTemplateConfiguration : EntityTypeConfiguration<S_S_PostLevelTemplate>
    {
        public S_S_PostLevelTemplateConfiguration()
        {
			ToTable("S_S_POSTLEVELTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.FlowPhase).HasColumnName("FLOWPHASE").IsOptional().HasMaxLength(50);
            Property(x => x.StepName).HasColumnName("STEPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.BelongYear).HasColumnName("BELONGYEAR").IsOptional().HasMaxLength(50);
            Property(x => x.BelongMonth).HasColumnName("BELONGMONTH").IsOptional().HasMaxLength(50);
            Property(x => x.BelongQuarter).HasColumnName("BELONGQUARTER").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(2000);
            Property(x => x.TemplateCode).HasColumnName("TEMPLATECODE").IsOptional().HasMaxLength(200);
            Property(x => x.TemplateName).HasColumnName("TEMPLATENAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_S_PostLevelTemplate_PostList
    internal partial class S_S_PostLevelTemplate_PostListConfiguration : EntityTypeConfiguration<S_S_PostLevelTemplate_PostList>
    {
        public S_S_PostLevelTemplate_PostListConfiguration()
        {
			ToTable("S_S_POSTLEVELTEMPLATE_POSTLIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.S_S_PostLevelTemplateID).HasColumnName("S_S_POSTLEVELTEMPLATEID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.PostCode).HasColumnName("POSTCODE").IsOptional().HasMaxLength(200);
            Property(x => x.PostLevelCode).HasColumnName("POSTLEVELCODE").IsOptional().HasMaxLength(200);
            Property(x => x.BZ).HasColumnName("BZ").IsOptional().HasMaxLength(2000);
            Property(x => x.BelongYear).HasColumnName("BELONGYEAR").IsOptional().HasMaxLength(200);
            Property(x => x.BelongMonth).HasColumnName("BELONGMONTH").IsOptional().HasMaxLength(200);
            Property(x => x.BelongQuarter).HasColumnName("BELONGQUARTER").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.S_S_PostLevelTemplate).WithMany(b => b.S_S_PostLevelTemplate_PostList).HasForeignKey(c => c.S_S_PostLevelTemplateID); // FK_S_S_PostLevelTemplate_PostList_S_S_PostLevelTemplate
        }
    }

    // S_T_Task
    internal partial class S_T_TaskConfiguration : EntityTypeConfiguration<S_T_Task>
    {
        public S_T_TaskConfiguration()
        {
			ToTable("S_T_TASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TaskExecID).HasColumnName("TASKEXECID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ExecUserIDs).HasColumnName("EXECUSERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.ExecUserNames).HasColumnName("EXECUSERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.DeadLine).HasColumnName("DEADLINE").IsOptional();
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(2000);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.CompleteTime).HasColumnName("COMPLETETIME").IsOptional();
            Property(x => x.Score).HasColumnName("SCORE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.UpdateTime).HasColumnName("UPDATETIME").IsOptional();
        }
    }

    // S_T_TaskExec
    internal partial class S_T_TaskExecConfiguration : EntityTypeConfiguration<S_T_TaskExec>
    {
        public S_T_TaskExecConfiguration()
        {
			ToTable("S_T_TASKEXEC");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TaskID).HasColumnName("TASKID").IsOptional().HasMaxLength(50);
            Property(x => x.ExecUserID).HasColumnName("EXECUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ExecUserName).HasColumnName("EXECUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ExecTime).HasColumnName("EXECTIME").IsOptional();
            Property(x => x.ExecDescription).HasColumnName("EXECDESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.ExecAttachment).HasColumnName("EXECATTACHMENT").IsOptional().HasMaxLength(2000);
            Property(x => x.ExecScore).HasColumnName("EXECSCORE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_T_Task).WithMany(b => b.S_T_TaskExec).HasForeignKey(c => c.TaskID); // FK_S_T_TaskExec_S_T_Task
        }
    }

    // S_UI_BIConfig
    internal partial class S_UI_BIConfigConfiguration : EntityTypeConfiguration<S_UI_BIConfig>
    {
        public S_UI_BIConfigConfiguration()
        {
			ToTable("S_UI_BICONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Layout).HasColumnName("LAYOUT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Blocks).HasColumnName("BLOCKS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Component
    internal partial class S_UI_ComponentConfiguration : EntityTypeConfiguration<S_UI_Component>
    {
        public S_UI_ComponentConfiguration()
        {
			ToTable("S_UI_COMPONENT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Json).HasColumnName("JSON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
        }
    }

    // S_UI_DataSource
    internal partial class S_UI_DataSourceConfiguration : EntityTypeConfiguration<S_UI_DataSource>
    {
        public S_UI_DataSourceConfiguration()
        {
			ToTable("S_UI_DATASOURCE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Fields).HasColumnName("FIELDS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
        }
    }

    // S_UI_ExcelImport
    internal partial class S_UI_ExcelImportConfiguration : EntityTypeConfiguration<S_UI_ExcelImport>
    {
        public S_UI_ExcelImportConfiguration()
        {
			ToTable("S_UI_EXCELIMPORT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.LayoutField).HasColumnName("LAYOUTFIELD").IsOptional().HasMaxLength(1073741823);
            Property(x => x.DataRule).HasColumnName("DATARULE").IsOptional().HasMaxLength(10);
            Property(x => x.Condition).HasColumnName("CONDITION").IsOptional().HasMaxLength(2000);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
        }
    }

    // S_UI_ExcelPrint
    internal partial class S_UI_ExcelPrintConfiguration : EntityTypeConfiguration<S_UI_ExcelPrint>
    {
        public S_UI_ExcelPrintConfiguration()
        {
			ToTable("S_UI_EXCELPRINT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Form
    internal partial class S_UI_FormConfiguration : EntityTypeConfiguration<S_UI_Form>
    {
        public S_UI_FormConfiguration()
        {
			ToTable("S_UI_FORM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.FlowLogic).HasColumnName("FLOWLOGIC").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HiddenFields).HasColumnName("HIDDENFIELDS").IsOptional().HasMaxLength(500);
            Property(x => x.Layout).HasColumnName("LAYOUT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Setttings).HasColumnName("SETTTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.SerialNumberSettings).HasColumnName("SERIALNUMBERSETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.DefaultValueSettings).HasColumnName("DEFAULTVALUESETTINGS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ReleaseTime).HasColumnName("RELEASETIME").IsOptional();
            Property(x => x.ReleasedData).HasColumnName("RELEASEDDATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.LayoutEN).HasColumnName("LAYOUTEN").IsOptional().HasMaxLength(1073741823);
            Property(x => x.MobileItems).HasColumnName("MOBILEITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.IsPrint).HasColumnName("ISPRINT").IsOptional().HasMaxLength(10);
            Property(x => x.MobileListSql).HasColumnName("MOBILELISTSQL").IsOptional().HasMaxLength(500);
            Property(x => x.ValidateUnique).HasColumnName("VALIDATEUNIQUE").IsOptional().HasMaxLength(50);
            Property(x => x.VersionEndDate).HasColumnName("VERSIONENDDATE").IsOptional();
            Property(x => x.VersionNum).HasColumnName("VERSIONNUM").IsOptional();
            Property(x => x.VersionDesc).HasColumnName("VERSIONDESC").IsOptional().HasMaxLength(500);
            Property(x => x.VersionStartDate).HasColumnName("VERSIONSTARTDATE").IsOptional();
            Property(x => x.WebPrintJS).HasColumnName("WEBPRINTJS").IsOptional();
            Property(x => x.LayoutPrint).HasColumnName("LAYOUTPRINT").IsOptional();
            Property(x => x.IsUEditor).HasColumnName("ISUEDITOR").IsOptional().HasMaxLength(1);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(500);
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(500);
            Property(x => x.CalItems).HasColumnName("CALITEMS").IsOptional();
            Property(x => x.MobileScriptText).HasColumnName("MOBILESCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
        }
    }

    // S_UI_FreePivot
    internal partial class S_UI_FreePivotConfiguration : EntityTypeConfiguration<S_UI_FreePivot>
    {
        public S_UI_FreePivotConfiguration()
        {
			ToTable("S_UI_FREEPIVOT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DataSource).HasColumnName("DATASOURCE").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Enum).HasColumnName("ENUM").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_FreePivotInstance
    internal partial class S_UI_FreePivotInstanceConfiguration : EntityTypeConfiguration<S_UI_FreePivotInstance>
    {
        public S_UI_FreePivotInstanceConfiguration()
        {
			ToTable("S_UI_FREEPIVOTINSTANCE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FreePivotID).HasColumnName("FREEPIVOTID").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(50);
            Property(x => x.DataSourceName).HasColumnName("DATASOURCENAME").IsOptional().HasMaxLength(500);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.RowItems).HasColumnName("ROWITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ColumnItems).HasColumnName("COLUMNITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.DataItems).HasColumnName("DATAITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.FilterItems).HasColumnName("FILTERITEMS").IsOptional().HasMaxLength(1073741823);

            // Foreign keys
            HasOptional(a => a.S_UI_FreePivot).WithMany(b => b.S_UI_FreePivotInstance).HasForeignKey(c => c.FreePivotID); // FK_S_UI_FreePivotInstance_S_UI_FreePivot
        }
    }

    // S_UI_FreePivotInstanceUser
    internal partial class S_UI_FreePivotInstanceUserConfiguration : EntityTypeConfiguration<S_UI_FreePivotInstanceUser>
    {
        public S_UI_FreePivotInstanceUserConfiguration()
        {
			ToTable("S_UI_FREEPIVOTINSTANCEUSER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FreePivotInstanceID).HasColumnName("FREEPIVOTINSTANCEID").IsOptional().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.RowItems).HasColumnName("ROWITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ColumnItems).HasColumnName("COLUMNITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.DataItems).HasColumnName("DATAITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.FilterItems).HasColumnName("FILTERITEMS").IsOptional().HasMaxLength(1073741823);

            // Foreign keys
            HasOptional(a => a.S_UI_FreePivotInstance).WithMany(b => b.S_UI_FreePivotInstanceUser).HasForeignKey(c => c.FreePivotInstanceID); // FK_S_UI_FreePivotInstanceUser_S_UI_FreePivotInstance
        }
    }

    // S_UI_Help
    internal partial class S_UI_HelpConfiguration : EntityTypeConfiguration<S_UI_Help>
    {
        public S_UI_HelpConfiguration()
        {
			ToTable("S_UI_HELP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.HelpPageType).HasColumnName("HELPPAGETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(500);
            Property(x => x.HelpUrl).HasColumnName("HELPURL").IsOptional().HasMaxLength(500);
            Property(x => x.Layout).HasColumnName("LAYOUT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HelpFile).HasColumnName("HELPFILE").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
        }
    }

    // S_UI_JinGeSign
    internal partial class S_UI_JinGeSignConfiguration : EntityTypeConfiguration<S_UI_JinGeSign>
    {
        public S_UI_JinGeSignConfiguration()
        {
			ToTable("S_UI_JINGESIGN");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FormID).HasColumnName("FORMID").IsOptional().HasMaxLength(50);
            Property(x => x.signatureid).HasColumnName("SIGNATUREID").IsOptional().HasMaxLength(50);
            Property(x => x.signatureData).HasColumnName("SIGNATUREDATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.signUserId).HasColumnName("SIGNUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Layout
    internal partial class S_UI_LayoutConfiguration : EntityTypeConfiguration<S_UI_Layout>
    {
        public S_UI_LayoutConfiguration()
        {
			ToTable("S_UI_LAYOUT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Json).HasColumnName("JSON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.Height).HasColumnName("HEIGHT").IsOptional().HasMaxLength(50);
            Property(x => x.JS).HasColumnName("JS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.EnumKeys).HasColumnName("ENUMKEYS").IsOptional().HasMaxLength(2000);
            Property(x => x.EnumNames).HasColumnName("ENUMNAMES").IsOptional().HasMaxLength(2000);
            Property(x => x.ParameterEnumKeys).HasColumnName("PARAMETERENUMKEYS").IsOptional().HasMaxLength(2000);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HasRowNumber).HasColumnName("HASROWNUMBER").IsOptional().HasMaxLength(50);
            Property(x => x.LayoutGrid).HasColumnName("LAYOUTGRID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutField).HasColumnName("LAYOUTFIELD").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutSearch).HasColumnName("LAYOUTSEARCH").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutButton).HasColumnName("LAYOUTBUTTON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutTab).HasColumnName("LAYOUTTAB").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Settings).HasColumnName("SETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.Released).HasColumnName("RELEASED").IsOptional().HasMaxLength(50);
            Property(x => x.RelevanceForm).HasColumnName("RELEVANCEFORM").IsOptional().HasMaxLength(50);
            Property(x => x.IDField).HasColumnName("IDFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.TextField).HasColumnName("TEXTFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.ParentField).HasColumnName("PARENTFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.IsExpand).HasColumnName("ISEXPAND").IsOptional().HasMaxLength(50);
            Property(x => x.FormCode).HasColumnName("FORMCODE").IsOptional().HasMaxLength(50);
            Property(x => x.DenyDeleteFlow).HasColumnName("DENYDELETEFLOW").IsOptional().HasMaxLength(50);
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.OrderBy).HasColumnName("ORDERBY").IsOptional().HasMaxLength(500);
            Property(x => x.HasCheckboxColumn).HasColumnName("HASCHECKBOXCOLUMN").IsOptional().HasMaxLength(50);
            Property(x => x.UseType).HasColumnName("USETYPE").IsOptional().HasMaxLength(100);
            Property(x => x.Mode).HasColumnName("MODE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_List
    internal partial class S_UI_ListConfiguration : EntityTypeConfiguration<S_UI_List>
    {
        public S_UI_ListConfiguration()
        {
			ToTable("S_UI_LIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HasRowNumber).HasColumnName("HASROWNUMBER").IsOptional().HasMaxLength(50);
            Property(x => x.LayoutGrid).HasColumnName("LAYOUTGRID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutField).HasColumnName("LAYOUTFIELD").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutSearch).HasColumnName("LAYOUTSEARCH").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutButton).HasColumnName("LAYOUTBUTTON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Settings).HasColumnName("SETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.Released).HasColumnName("RELEASED").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.DenyDeleteFlow).HasColumnName("DENYDELETEFLOW").IsOptional().HasMaxLength(50);
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.OrderBy).HasColumnName("ORDERBY").IsOptional().HasMaxLength(500);
            Property(x => x.HasCheckboxColumn).HasColumnName("HASCHECKBOXCOLUMN").IsOptional().HasMaxLength(50);
            Property(x => x.DefaultValueSettings).HasColumnName("DEFAULTVALUESETTINGS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(500);
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(500);
        }
    }

    // S_UI_ModifyLogLog
    internal partial class S_UI_ModifyLogLogConfiguration : EntityTypeConfiguration<S_UI_ModifyLogLog>
    {
        public S_UI_ModifyLogLogConfiguration()
        {
			ToTable("S_UI_MODIFYLOGLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.IOTime).HasColumnName("IOTIME").IsOptional();
            Property(x => x.IOType).HasColumnName("IOTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.RelateData).HasColumnName("RELATEDATA").IsOptional();
            Property(x => x.CategoryName).HasColumnName("CATEGORYNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Pivot
    internal partial class S_UI_PivotConfiguration : EntityTypeConfiguration<S_UI_Pivot>
    {
        public S_UI_PivotConfiguration()
        {
			ToTable("S_UI_PIVOT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.RowItems).HasColumnName("ROWITEMS").IsOptional();
            Property(x => x.ColumnItems).HasColumnName("COLUMNITEMS").IsOptional();
            Property(x => x.DataItems).HasColumnName("DATAITEMS").IsOptional();
            Property(x => x.FilterItems).HasColumnName("FILTERITEMS").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_PivotUser
    internal partial class S_UI_PivotUserConfiguration : EntityTypeConfiguration<S_UI_PivotUser>
    {
        public S_UI_PivotUserConfiguration()
        {
			ToTable("S_UI_PIVOTUSER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.PivotID).HasColumnName("PIVOTID").IsOptional().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.RowItems).HasColumnName("ROWITEMS").IsOptional();
            Property(x => x.ColumnItems).HasColumnName("COLUMNITEMS").IsOptional();
            Property(x => x.DataItems).HasColumnName("DATAITEMS").IsOptional();
            Property(x => x.FilterItems).HasColumnName("FILTERITEMS").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_UI_Pivot).WithMany(b => b.S_UI_PivotUser).HasForeignKey(c => c.PivotID); // FK_S_UI_PivotUser_S_UI_Pivot
        }
    }

    // S_UI_RoleRes
    internal partial class S_UI_RoleResConfiguration : EntityTypeConfiguration<S_UI_RoleRes>
    {
        public S_UI_RoleResConfiguration()
        {
			ToTable("S_UI_ROLERES");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UseID).HasColumnName("USEID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.FullPath).HasColumnName("FULLPATH").IsOptional().HasMaxLength(1000);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(500);
            Property(x => x.IsInvalid).HasColumnName("ISINVALID").IsOptional().HasMaxLength(10);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Selector
    internal partial class S_UI_SelectorConfiguration : EntityTypeConfiguration<S_UI_Selector>
    {
        public S_UI_SelectorConfiguration()
        {
			ToTable("S_UI_SELECTOR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.URLSingle).HasColumnName("URLSINGLE").IsOptional().HasMaxLength(200);
            Property(x => x.URLMulti).HasColumnName("URLMULTI").IsOptional().HasMaxLength(200);
            Property(x => x.Width).HasColumnName("WIDTH").IsOptional().HasMaxLength(50);
            Property(x => x.Height).HasColumnName("HEIGHT").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_SerialNumber
    internal partial class S_UI_SerialNumberConfiguration : EntityTypeConfiguration<S_UI_SerialNumber>
    {
        public S_UI_SerialNumberConfiguration()
        {
			ToTable("S_UI_SERIALNUMBER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.YearCode).HasColumnName("YEARCODE").IsOptional().HasMaxLength(50);
            Property(x => x.MonthCode).HasColumnName("MONTHCODE").IsOptional().HasMaxLength(50);
            Property(x => x.DayCode).HasColumnName("DAYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.CategoryCode).HasColumnName("CATEGORYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.SubCategoryCode).HasColumnName("SUBCATEGORYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.OrderNumCode).HasColumnName("ORDERNUMCODE").IsOptional().HasMaxLength(50);
            Property(x => x.PrjCode).HasColumnName("PRJCODE").IsOptional().HasMaxLength(50);
            Property(x => x.OrgCode).HasColumnName("ORGCODE").IsOptional().HasMaxLength(50);
            Property(x => x.UserCode).HasColumnName("USERCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Number).HasColumnName("NUMBER").IsOptional();
        }
    }

    // S_UI_Word
    internal partial class S_UI_WordConfiguration : EntityTypeConfiguration<S_UI_Word>
    {
        public S_UI_WordConfiguration()
        {
			ToTable("S_UI_WORD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.Collision).HasColumnName("COLLISION").IsOptional().HasMaxLength(50);
            Property(x => x.VersionNum).HasColumnName("VERSIONNUM").IsOptional();
            Property(x => x.VersionDesc).HasColumnName("VERSIONDESC").IsOptional().HasMaxLength(500);
            Property(x => x.VersionStartDate).HasColumnName("VERSIONSTARTDATE").IsOptional();
            Property(x => x.VersionEndDate).HasColumnName("VERSIONENDDATE").IsOptional();
            Property(x => x.WordNameTmpl).HasColumnName("WORDNAMETMPL").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(500);
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(500);
        }
    }

    // tmp_ms_xx_S_A_AuthLog
    internal partial class tmp_ms_xx_S_A_AuthLogConfiguration : EntityTypeConfiguration<tmp_ms_xx_S_A_AuthLog>
    {
        public tmp_ms_xx_S_A_AuthLogConfiguration()
        {
			ToTable("TMP_MS_XX_S_A_AUTHLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Operation).HasColumnName("OPERATION").IsOptional().HasMaxLength(50);
            Property(x => x.OperationTarget).HasColumnName("OPERATIONTARGET").IsOptional().HasMaxLength(50);
            Property(x => x.RelateData).HasColumnName("RELATEDATA").IsOptional();
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ClientIP).HasColumnName("CLIENTIP").IsOptional().HasMaxLength(50);
        }
    }

}

