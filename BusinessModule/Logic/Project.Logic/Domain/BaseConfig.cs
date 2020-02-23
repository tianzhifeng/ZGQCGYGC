

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "InfrasBaseConfig"
//     Connection String:      "Data Source=10.10.1.244\SQL2008;Initial Catalog=SINOAE_DesignInfrastructure;User ID=sa;PWD=123.zxc;"

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

namespace Project.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class BaseConfigEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_C_CBSDefine> S_C_CBSDefine { get; set; } // S_C_CBSDefine
        public IDbSet<S_C_MajorWorkloadDefine> S_C_MajorWorkloadDefine { get; set; } // S_C_MajorWorkloadDefine
        public IDbSet<S_C_ManageWorkloadDefine> S_C_ManageWorkloadDefine { get; set; } // S_C_ManageWorkloadDefine
        public IDbSet<S_C_RoleWorkloadDefine> S_C_RoleWorkloadDefine { get; set; } // S_C_RoleWorkloadDefine
        public IDbSet<S_CAD_AreaAuth> S_CAD_AreaAuth { get; set; } // S_CAD_AreaAuth
        public IDbSet<S_CAD_PublishPrice> S_CAD_PublishPrice { get; set; } // S_CAD_PublishPrice
        public IDbSet<S_D_DesignInputDefine> S_D_DesignInputDefine { get; set; } // S_D_DesignInputDefine
        public IDbSet<S_D_EPSDef> S_D_EPSDef { get; set; } // S_D_EPSDef
        public IDbSet<S_D_Feature> S_D_Feature { get; set; } // S_D_Feature
        public IDbSet<S_D_PackageDic> S_D_PackageDic { get; set; } // S_D_PackageDic
        public IDbSet<S_D_PackageDic_RoleRate> S_D_PackageDic_RoleRate { get; set; } // S_D_PackageDic_RoleRate
        public IDbSet<S_D_RoleDefine> S_D_RoleDefine { get; set; } // S_D_RoleDefine
        public IDbSet<S_D_Standard> S_D_Standard { get; set; } // S_D_Standard
        public IDbSet<S_D_StandardDocument> S_D_StandardDocument { get; set; } // S_D_StandardDocument
        public IDbSet<S_D_WBSAttrDefine> S_D_WBSAttrDefine { get; set; } // S_D_WBSAttrDefine
        public IDbSet<S_D_WBSAttrDeptInfo> S_D_WBSAttrDeptInfo { get; set; } // S_D_WBSAttrDeptInfo
        public IDbSet<S_D_WBSTemplate> S_D_WBSTemplate { get; set; } // S_D_WBSTemplate
        public IDbSet<S_D_WBSTemplateNode> S_D_WBSTemplateNode { get; set; } // S_D_WBSTemplateNode
        public IDbSet<S_T_AuditMode> S_T_AuditMode { get; set; } // S_T_AuditMode
        public IDbSet<S_T_DataAuth> S_T_DataAuth { get; set; } // S_T_DataAuth
        public IDbSet<S_T_DBSDefine> S_T_DBSDefine { get; set; } // S_T_DBSDefine
        public IDbSet<S_T_DBSSecurity> S_T_DBSSecurity { get; set; } // S_T_DBSSecurity
        public IDbSet<S_T_EngineeringSpace> S_T_EngineeringSpace { get; set; } // S_T_EngineeringSpace
        public IDbSet<S_T_EngineeringSpaceAuth> S_T_EngineeringSpaceAuth { get; set; } // S_T_EngineeringSpaceAuth
        public IDbSet<S_T_EngineeringSpaceRes> S_T_EngineeringSpaceRes { get; set; } // S_T_EngineeringSpaceRes
        public IDbSet<S_T_FlowTraceDefine> S_T_FlowTraceDefine { get; set; } // S_T_FlowTraceDefine
        public IDbSet<S_T_HotRangeStatisticsConfig> S_T_HotRangeStatisticsConfig { get; set; } // S_T_HotRangeStatisticsConfig
        public IDbSet<S_T_ISODefine> S_T_ISODefine { get; set; } // S_T_ISODefine
        public IDbSet<S_T_MileStone> S_T_MileStone { get; set; } // S_T_MileStone
        public IDbSet<S_T_ProjectMode> S_T_ProjectMode { get; set; } // S_T_ProjectMode
        public IDbSet<S_T_QBSTemplate> S_T_QBSTemplate { get; set; } // S_T_QBSTemplate
        public IDbSet<S_T_SpaceAuth> S_T_SpaceAuth { get; set; } // S_T_SpaceAuth
        public IDbSet<S_T_SpaceDefine> S_T_SpaceDefine { get; set; } // S_T_SpaceDefine
        public IDbSet<S_T_ToDoListDefine> S_T_ToDoListDefine { get; set; } // S_T_ToDoListDefine
        public IDbSet<S_T_ToDoListDefineNode> S_T_ToDoListDefineNode { get; set; } // S_T_ToDoListDefineNode
        public IDbSet<S_T_WBSStructInfo> S_T_WBSStructInfo { get; set; } // S_T_WBSStructInfo
        public IDbSet<S_T_WBSStructRole> S_T_WBSStructRole { get; set; } // S_T_WBSStructRole

        static BaseConfigEntities()
        {
            Database.SetInitializer<BaseConfigEntities>(null);
        }

        public BaseConfigEntities()
            : base("Name=InfrasBaseConfig")
        {
        }

        public BaseConfigEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_C_CBSDefineConfiguration());
            modelBuilder.Configurations.Add(new S_C_MajorWorkloadDefineConfiguration());
            modelBuilder.Configurations.Add(new S_C_ManageWorkloadDefineConfiguration());
            modelBuilder.Configurations.Add(new S_C_RoleWorkloadDefineConfiguration());
            modelBuilder.Configurations.Add(new S_CAD_AreaAuthConfiguration());
            modelBuilder.Configurations.Add(new S_CAD_PublishPriceConfiguration());
            modelBuilder.Configurations.Add(new S_D_DesignInputDefineConfiguration());
            modelBuilder.Configurations.Add(new S_D_EPSDefConfiguration());
            modelBuilder.Configurations.Add(new S_D_FeatureConfiguration());
            modelBuilder.Configurations.Add(new S_D_PackageDicConfiguration());
            modelBuilder.Configurations.Add(new S_D_PackageDic_RoleRateConfiguration());
            modelBuilder.Configurations.Add(new S_D_RoleDefineConfiguration());
            modelBuilder.Configurations.Add(new S_D_StandardConfiguration());
            modelBuilder.Configurations.Add(new S_D_StandardDocumentConfiguration());
            modelBuilder.Configurations.Add(new S_D_WBSAttrDefineConfiguration());
            modelBuilder.Configurations.Add(new S_D_WBSAttrDeptInfoConfiguration());
            modelBuilder.Configurations.Add(new S_D_WBSTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_D_WBSTemplateNodeConfiguration());
            modelBuilder.Configurations.Add(new S_T_AuditModeConfiguration());
            modelBuilder.Configurations.Add(new S_T_DataAuthConfiguration());
            modelBuilder.Configurations.Add(new S_T_DBSDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_DBSSecurityConfiguration());
            modelBuilder.Configurations.Add(new S_T_EngineeringSpaceConfiguration());
            modelBuilder.Configurations.Add(new S_T_EngineeringSpaceAuthConfiguration());
            modelBuilder.Configurations.Add(new S_T_EngineeringSpaceResConfiguration());
            modelBuilder.Configurations.Add(new S_T_FlowTraceDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_HotRangeStatisticsConfigConfiguration());
            modelBuilder.Configurations.Add(new S_T_ISODefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_MileStoneConfiguration());
            modelBuilder.Configurations.Add(new S_T_ProjectModeConfiguration());
            modelBuilder.Configurations.Add(new S_T_QBSTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_SpaceAuthConfiguration());
            modelBuilder.Configurations.Add(new S_T_SpaceDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_ToDoListDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_ToDoListDefineNodeConfiguration());
            modelBuilder.Configurations.Add(new S_T_WBSStructInfoConfiguration());
            modelBuilder.Configurations.Add(new S_T_WBSStructRoleConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_CBSDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string CBSCode { get; set; } // CBSCode
		/// <summary></summary>	
		[Description("")]
        public string CBSName { get; set; } // CBSName
		/// <summary></summary>	
		[Description("")]
        public string CBSType { get; set; } // CBSType
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_C_CBSDefine_S_T_ProjectMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_MajorWorkloadDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MajorName { get; set; } // MajorName
		/// <summary></summary>	
		[Description("")]
        public string MajorValue { get; set; } // MajorValue
		/// <summary></summary>	
		[Description("")]
        public decimal? Rate { get; set; } // Rate
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
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
        public string CompanyID { get; set; } // CompanyID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_ManageWorkloadDefine : Formula.BaseModel
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
        public decimal? Rate { get; set; } // Rate
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
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
        public string CompanyID { get; set; } // CompanyID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_RoleWorkloadDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RoleName { get; set; } // RoleName
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public decimal? Rate { get; set; } // Rate
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
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
        public string CompanyID { get; set; } // CompanyID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_CAD_AreaAuth : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string AuthType { get; set; } // AuthType
		/// <summary></summary>	
		[Description("")]
        public string HasCRUDAuth { get; set; } // HasCRUDAuth
		/// <summary></summary>	
		[Description("")]
        public string HasPrintAuth { get; set; } // HasPrintAuth
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_CAD_PublishPrice : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string PublicationType { get; set; } // PublicationType
		/// <summary></summary>	
		[Description("")]
        public string Specification { get; set; } // Specification
		/// <summary></summary>	
		[Description("")]
        public decimal Price { get; set; } // Price
    }

	/// <summary>设计资料类别定义</summary>	
	[Description("设计资料类别定义")]
    public partial class S_D_DesignInputDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
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
        public string CompanyID { get; set; } // CompanyID
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Class { get; set; } // Class
		/// <summary></summary>	
		[Description("")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary></summary>	
		[Description("")]
        public string PhaseValues { get; set; } // PhaseValues
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string DBSCode { get; set; } // DBSCode
		/// <summary></summary>	
		[Description("")]
        public string DocType { get; set; } // DocType
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public int? InputTypeIndex { get; set; } // InputTypeIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_EPSDef : Formula.BaseModel
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
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string GroupField { get; set; } // GroupField
		/// <summary></summary>	
		[Description("")]
        public string GroupTable { get; set; } // GroupTable
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_Feature : Formula.BaseModel
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
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public string RelateWBSAttrCode { get; set; } // RelateWBSAttrCode
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
    }

	/// <summary>石化院工作包词典维护</summary>	
	[Description("石化院工作包词典维护")]
    public partial class S_D_PackageDic : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>卷册类型</summary>	
		[Description("卷册类型")]
        public string PackageType { get; set; } // PackageType
		/// <summary>专业</summary>	
		[Description("专业")]
        public string MajorCode { get; set; } // MajorCode
		/// <summary>专业Name</summary>	
		[Description("专业Name")]
        public string MajorName { get; set; } // MajorName
		/// <summary>阶段</summary>	
		[Description("阶段")]
        public string PhaseCode { get; set; } // PhaseCode
		/// <summary>阶段Name</summary>	
		[Description("阶段Name")]
        public string PhaseName { get; set; } // PhaseName
		/// <summary>业务类型</summary>	
		[Description("业务类型")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary>校审级别</summary>	
		[Description("校审级别")]
        public string AuditLevel { get; set; } // AuditLevel
		/// <summary>定额工时</summary>	
		[Description("定额工时")]
        public decimal? WorkLoad { get; set; } // WorkLoad
		/// <summary>折合A1数</summary>	
		[Description("折合A1数")]
        public decimal? DrawingCount { get; set; } // DrawingCount
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>应用模式</summary>	
		[Description("应用模式")]
        public string ModeCodes { get; set; } // ModeCodes
		/// <summary>装置</summary>	
		[Description("装置")]
        public string DeviceName { get; set; } // DeviceName
		/// <summary>区域</summary>	
		[Description("区域")]
        public string AreaName { get; set; } // AreaName
		/// <summary>单体</summary>	
		[Description("单体")]
        public string EntityName { get; set; } // EntityName
		/// <summary>工作包类型</summary>	
		[Description("工作包类型")]
        public string WBSType { get; set; } // WBSType

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_D_PackageDic_RoleRate> S_D_PackageDic_RoleRate { get { onS_D_PackageDic_RoleRateGetting(); return _S_D_PackageDic_RoleRate;} }
		private ICollection<S_D_PackageDic_RoleRate> _S_D_PackageDic_RoleRate;
		partial void onS_D_PackageDic_RoleRateGetting();


        public S_D_PackageDic()
        {
            _S_D_PackageDic_RoleRate = new List<S_D_PackageDic_RoleRate>();
        }
    }

	/// <summary>角色比例</summary>	
	[Description("角色比例")]
    public partial class S_D_PackageDic_RoleRate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string S_D_PackageDicID { get; set; } // S_D_PackageDicID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>角色</summary>	
		[Description("角色")]
        public string Role { get; set; } // Role
		/// <summary>比例（%）</summary>	
		[Description("比例（%）")]
        public decimal? Rate { get; set; } // Rate
		/// <summary>定额工时</summary>	
		[Description("定额工时")]
        public decimal? WorkLoad { get; set; } // WorkLoad

        // Foreign keys
		[JsonIgnore]
        public virtual S_D_PackageDic S_D_PackageDic { get; set; } //  S_D_PackageDicID - FK_S_D_PackageDic_RoleRate_S_D_PackageDic
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_RoleDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleName { get; set; } // RoleName
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string RoleRelation { get; set; } // RoleRelation
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public decimal SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string OtherName { get; set; } // OtherName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_Standard : Formula.BaseModel
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
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_StandardDocument : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string StandardID { get; set; } // StandardID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Version { get; set; } // Version
		/// <summary></summary>	
		[Description("")]
        public string PublishMechanism { get; set; } // PublishMechanism
		/// <summary></summary>	
		[Description("")]
        public DateTime? PublishDate { get; set; } // PublishDate
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string FileName { get; set; } // FileName
		/// <summary></summary>	
		[Description("")]
        public string IsCurVersion { get; set; } // IsCurVersion
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_WBSAttrDefine : Formula.BaseModel
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
        public string WBSCode { get; set; } // WBSCode
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string BelongMode { get; set; } // BelongMode
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_D_WBSAttrDeptInfo> S_D_WBSAttrDeptInfo { get { onS_D_WBSAttrDeptInfoGetting(); return _S_D_WBSAttrDeptInfo;} }
		private ICollection<S_D_WBSAttrDeptInfo> _S_D_WBSAttrDeptInfo;
		partial void onS_D_WBSAttrDeptInfoGetting();


        public S_D_WBSAttrDefine()
        {
            _S_D_WBSAttrDeptInfo = new List<S_D_WBSAttrDeptInfo>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_WBSAttrDeptInfo : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string WBSAttrDefineID { get; set; } // WBSAttrDefineID
		/// <summary></summary>	
		[Description("")]
        public string DeptID { get; set; } // DeptID
		/// <summary></summary>	
		[Description("")]
        public string DeptName { get; set; } // DeptName

        // Foreign keys
		[JsonIgnore]
        public virtual S_D_WBSAttrDefine S_D_WBSAttrDefine { get; set; } //  WBSAttrDefineID - FK_S_D_WBSAttrDeptInfo_S_D_WBSAttrDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_WBSTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeCodes { get; set; } // ModeCodes
		/// <summary></summary>	
		[Description("")]
        public string ModeNames { get; set; } // ModeNames
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string ProjectType { get; set; } // ProjectType
		/// <summary></summary>	
		[Description("")]
        public string ProjectScope { get; set; } // ProjectScope

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_D_WBSTemplateNode> S_D_WBSTemplateNode { get { onS_D_WBSTemplateNodeGetting(); return _S_D_WBSTemplateNode;} }
		private ICollection<S_D_WBSTemplateNode> _S_D_WBSTemplateNode;
		partial void onS_D_WBSTemplateNodeGetting();


        public S_D_WBSTemplate()
        {
            _S_D_WBSTemplateNode = new List<S_D_WBSTemplateNode>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_WBSTemplateNode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TemplateID { get; set; } // TemplateID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string WBSValue { get; set; } // WBSValue
		/// <summary></summary>	
		[Description("")]
        public string WBSType { get; set; } // WBSType
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public int Level { get; set; } // Level
		/// <summary></summary>	
		[Description("")]
        public string WBSDeptID { get; set; } // WBSDeptID
		/// <summary></summary>	
		[Description("")]
        public string WBSDeptName { get; set; } // WBSDeptName
		/// <summary></summary>	
		[Description("")]
        public decimal? WorkLoad { get; set; } // WorkLoad
		/// <summary></summary>	
		[Description("")]
        public decimal? PlanWorkLoad { get; set; } // PlanWorkLoad
		/// <summary></summary>	
		[Description("")]
        public decimal? Weight { get; set; } // Weight

        // Foreign keys
		[JsonIgnore]
        public virtual S_D_WBSTemplate S_D_WBSTemplate { get; set; } //  TemplateID - FK_S_T_WBSTemplateNode_S_T_WBSTemplate
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_AuditMode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ProjectModeID { get; set; } // ProjectModeID
		/// <summary></summary>	
		[Description("")]
        public string AttrID { get; set; } // AttrID
		/// <summary></summary>	
		[Description("")]
        public string PhaseValue { get; set; } // PhaseValue
		/// <summary></summary>	
		[Description("")]
        public string PhaseName { get; set; } // PhaseName
		/// <summary></summary>	
		[Description("")]
        public string AuditMode { get; set; } // AuditMode

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ProjectModeID - FK_S_T_ProjectMode_S_T_AuditMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_DataAuth : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_DataAuth_S_T_ProjectMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_DBSDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string DBSCode { get; set; } // DBSCode
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public int Level { get; set; } // Level
		/// <summary></summary>	
		[Description("")]
        public string DBSType { get; set; } // DBSType
		/// <summary></summary>	
		[Description("")]
        public string MappingType { get; set; } // MappingType
		/// <summary></summary>	
		[Description("")]
        public string MappingNodeUrl { get; set; } // MappingNodeUrl
		/// <summary></summary>	
		[Description("")]
        public string InheritAuth { get; set; } // InheritAuth
		/// <summary></summary>	
		[Description("")]
        public string ArchiveFolder { get; set; } // ArchiveFolder
		/// <summary></summary>	
		[Description("")]
        public string ArchiveFolderName { get; set; } // ArchiveFolderName
		/// <summary></summary>	
		[Description("")]
        public string ProductStruct { get; set; } // ProductStruct

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_DBSSecurity> S_T_DBSSecurity { get { onS_T_DBSSecurityGetting(); return _S_T_DBSSecurity;} }
		private ICollection<S_T_DBSSecurity> _S_T_DBSSecurity;
		partial void onS_T_DBSSecurityGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_DBSDefine_S_T_ProjectMode

        public S_T_DBSDefine()
        {
            _S_T_DBSSecurity = new List<S_T_DBSSecurity>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_DBSSecurity : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>DBS目录定义ID</summary>	
		[Description("DBS目录定义ID")]
        public string DBSDefineID { get; set; } // DBSDefineID
		/// <summary>角色编码</summary>	
		[Description("角色编码")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary>角色名称</summary>	
		[Description("角色名称")]
        public string RoleName { get; set; } // RoleName
		/// <summary>权限</summary>	
		[Description("权限")]
        public string AuthType { get; set; } // AuthType
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_DBSDefine S_T_DBSDefine { get; set; } //  DBSDefineID - FK_S_T_DBSSecurity_S_T_DBSSecurity
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_EngineeringSpace : Formula.BaseModel
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
        public string Remark { get; set; } // Remark

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_EngineeringSpaceRes> S_T_EngineeringSpaceRes { get { onS_T_EngineeringSpaceResGetting(); return _S_T_EngineeringSpaceRes;} }
		private ICollection<S_T_EngineeringSpaceRes> _S_T_EngineeringSpaceRes;
		partial void onS_T_EngineeringSpaceResGetting();


        public S_T_EngineeringSpace()
        {
            _S_T_EngineeringSpaceRes = new List<S_T_EngineeringSpaceRes>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_EngineeringSpaceAuth : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string EngineeringSpaceResID { get; set; } // EngineeringSpaceResID
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string AuthType { get; set; } // AuthType

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_EngineeringSpaceRes S_T_EngineeringSpaceRes { get; set; } //  EngineeringSpaceResID - FK_S_T_EngineeringSpaceAuth_S_T_EngineeringSpaceRes
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_EngineeringSpaceRes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
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
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_EngineeringSpaceAuth> S_T_EngineeringSpaceAuth { get { onS_T_EngineeringSpaceAuthGetting(); return _S_T_EngineeringSpaceAuth;} }
		private ICollection<S_T_EngineeringSpaceAuth> _S_T_EngineeringSpaceAuth;
		partial void onS_T_EngineeringSpaceAuthGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_EngineeringSpace S_T_EngineeringSpace { get; set; } //  SpaceID - FK_S_T_EngineeringSpaceRes_S_T_EngineeringSpace

        public S_T_EngineeringSpaceRes()
        {
            _S_T_EngineeringSpaceAuth = new List<S_T_EngineeringSpaceAuth>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_FlowTraceDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
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
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string StateField { get; set; } // StateField
		/// <summary></summary>	
		[Description("")]
        public string StepField { get; set; } // StepField
		/// <summary></summary>	
		[Description("")]
        public string NameFieldInfo { get; set; } // NameFieldInfo
		/// <summary></summary>	
		[Description("")]
        public string EnumFieldInfo { get; set; } // EnumFieldInfo
		/// <summary></summary>	
		[Description("")]
        public string LinkFormUrl { get; set; } // LinkFormUrl
		/// <summary></summary>	
		[Description("")]
        public string LinkField { get; set; } // LinkField
		/// <summary></summary>	
		[Description("")]
        public int SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_FlowTraceDefine_S_T_ProjectMode
    }

	/// <summary>gis热点区域统计配置表</summary>	
	[Description("gis热点区域统计配置表")]
    public partial class S_T_HotRangeStatisticsConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>区域名称</summary>	
		[Description("区域名称")]
        public string Name { get; set; } // Name
		/// <summary>区域字段</summary>	
		[Description("区域字段")]
        public string Field { get; set; } // Field
		/// <summary>区域详细坐标经度</summary>	
		[Description("区域详细坐标经度")]
        public double? Long { get; set; } // Long
		/// <summary>区域详细坐标纬度</summary>	
		[Description("区域详细坐标纬度")]
        public double? Lat { get; set; } // Lat
		/// <summary>显示的最小值</summary>	
		[Description("显示的最小值")]
        public int? MinZoomVal { get; set; } // MinZoomVal
		/// <summary>显示的最大值</summary>	
		[Description("显示的最大值")]
        public int? MaxZoomVal { get; set; } // MaxZoomVal
		/// <summary>区域子项(逗号分割)</summary>	
		[Description("区域子项(逗号分割)")]
        public string SubList { get; set; } // SubList
		/// <summary>上级区域名</summary>	
		[Description("上级区域名")]
        public string UpName { get; set; } // UpName
		/// <summary>区域子项字段</summary>	
		[Description("区域子项字段")]
        public string SubField { get; set; } // SubField
		/// <summary>是否有效</summary>	
		[Description("是否有效")]
        public string Enable { get; set; } // Enable
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ISODefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Catagory { get; set; } // Catagory
		/// <summary></summary>	
		[Description("")]
        public string LinkFormUrl { get; set; } // LinkFormUrl
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string NameFieldInfo { get; set; } // NameFieldInfo
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string EnumFieldInfo { get; set; } // EnumFieldInfo
		/// <summary></summary>	
		[Description("")]
        public string CanAddNewForm { get; set; } // CanAddNewForm
		/// <summary></summary>	
		[Description("")]
        public string ArchiveFields { get; set; } // ArchiveFields
		/// <summary></summary>	
		[Description("")]
        public string SendNotice { get; set; } // SendNotice
		/// <summary></summary>	
		[Description("")]
        public string FormCode { get; set; } // FormCode
		/// <summary></summary>	
		[Description("")]
        public string FlowCode { get; set; } // FlowCode
		/// <summary></summary>	
		[Description("")]
        public string LinkViewUrl { get; set; } // LinkViewUrl
		/// <summary></summary>	
		[Description("")]
        public string StartNoticeContent { get; set; } // StartNoticeContent
		/// <summary></summary>	
		[Description("")]
        public string EndNoticeContent { get; set; } // EndNoticeContent
		/// <summary></summary>	
		[Description("")]
        public string StepNoticeContent { get; set; } // StepNoticeContent
		/// <summary></summary>	
		[Description("")]
        public string Level { get; set; } // Level
		/// <summary></summary>	
		[Description("")]
        public string MajorField { get; set; } // MajorField
		/// <summary></summary>	
		[Description("")]
        public decimal? ExpiresDate { get; set; } // ExpiresDate

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_ISODefine_S_T_ProjectMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_MileStone : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MileStoneName { get; set; } // MileStoneName
		/// <summary></summary>	
		[Description("")]
        public string MileStoneCode { get; set; } // MileStoneCode
		/// <summary></summary>	
		[Description("")]
        public string MileStoneType { get; set; } // MileStoneType
		/// <summary></summary>	
		[Description("")]
        public int? DefaultTimeSpan { get; set; } // DefaultTimeSpan
		/// <summary></summary>	
		[Description("")]
        public decimal? Weight { get; set; } // Weight
		/// <summary></summary>	
		[Description("")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary></summary>	
		[Description("")]
        public string PhaseValue { get; set; } // PhaseValue
		/// <summary></summary>	
		[Description("")]
        public string Necessity { get; set; } // Necessity
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string OutMajors { get; set; } // OutMajors
		/// <summary></summary>	
		[Description("")]
        public string InMajors { get; set; } // InMajors
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_MileStone_S_T_ProjectMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ProjectMode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ModeCode { get; set; } // ModeCode
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Condition { get; set; } // Condition
		/// <summary></summary>	
		[Description("")]
        public string IsDefault { get; set; } // IsDefault
		/// <summary></summary>	
		[Description("")]
        public int Priority { get; set; } // Priority
		/// <summary></summary>	
		[Description("")]
        public string ExtentionJson { get; set; } // ExtentionJson

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_C_CBSDefine> S_C_CBSDefine { get { onS_C_CBSDefineGetting(); return _S_C_CBSDefine;} }
		private ICollection<S_C_CBSDefine> _S_C_CBSDefine;
		partial void onS_C_CBSDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_AuditMode> S_T_AuditMode { get { onS_T_AuditModeGetting(); return _S_T_AuditMode;} }
		private ICollection<S_T_AuditMode> _S_T_AuditMode;
		partial void onS_T_AuditModeGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_DataAuth> S_T_DataAuth { get { onS_T_DataAuthGetting(); return _S_T_DataAuth;} }
		private ICollection<S_T_DataAuth> _S_T_DataAuth;
		partial void onS_T_DataAuthGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_DBSDefine> S_T_DBSDefine { get { onS_T_DBSDefineGetting(); return _S_T_DBSDefine;} }
		private ICollection<S_T_DBSDefine> _S_T_DBSDefine;
		partial void onS_T_DBSDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_FlowTraceDefine> S_T_FlowTraceDefine { get { onS_T_FlowTraceDefineGetting(); return _S_T_FlowTraceDefine;} }
		private ICollection<S_T_FlowTraceDefine> _S_T_FlowTraceDefine;
		partial void onS_T_FlowTraceDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_ISODefine> S_T_ISODefine { get { onS_T_ISODefineGetting(); return _S_T_ISODefine;} }
		private ICollection<S_T_ISODefine> _S_T_ISODefine;
		partial void onS_T_ISODefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_MileStone> S_T_MileStone { get { onS_T_MileStoneGetting(); return _S_T_MileStone;} }
		private ICollection<S_T_MileStone> _S_T_MileStone;
		partial void onS_T_MileStoneGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_QBSTemplate> S_T_QBSTemplate { get { onS_T_QBSTemplateGetting(); return _S_T_QBSTemplate;} }
		private ICollection<S_T_QBSTemplate> _S_T_QBSTemplate;
		partial void onS_T_QBSTemplateGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_SpaceDefine> S_T_SpaceDefine { get { onS_T_SpaceDefineGetting(); return _S_T_SpaceDefine;} }
		private ICollection<S_T_SpaceDefine> _S_T_SpaceDefine;
		partial void onS_T_SpaceDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_ToDoListDefine> S_T_ToDoListDefine { get { onS_T_ToDoListDefineGetting(); return _S_T_ToDoListDefine;} }
		private ICollection<S_T_ToDoListDefine> _S_T_ToDoListDefine;
		partial void onS_T_ToDoListDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_T_WBSStructInfo> S_T_WBSStructInfo { get { onS_T_WBSStructInfoGetting(); return _S_T_WBSStructInfo;} }
		private ICollection<S_T_WBSStructInfo> _S_T_WBSStructInfo;
		partial void onS_T_WBSStructInfoGetting();


        public S_T_ProjectMode()
        {
            _S_C_CBSDefine = new List<S_C_CBSDefine>();
            _S_T_AuditMode = new List<S_T_AuditMode>();
            _S_T_DataAuth = new List<S_T_DataAuth>();
            _S_T_DBSDefine = new List<S_T_DBSDefine>();
            _S_T_FlowTraceDefine = new List<S_T_FlowTraceDefine>();
            _S_T_ISODefine = new List<S_T_ISODefine>();
            _S_T_MileStone = new List<S_T_MileStone>();
            _S_T_QBSTemplate = new List<S_T_QBSTemplate>();
            _S_T_SpaceDefine = new List<S_T_SpaceDefine>();
            _S_T_ToDoListDefine = new List<S_T_ToDoListDefine>();
            _S_T_WBSStructInfo = new List<S_T_WBSStructInfo>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_QBSTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string QBSType { get; set; } // QBSType
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public string DataUnit { get; set; } // DataUnit

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_QBSTemplate_S_T_ProjectMode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_SpaceAuth : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string AuthType { get; set; } // AuthType

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_SpaceDefine S_T_SpaceDefine { get; set; } //  SpaceID - FK_S_T_SpaceAuth_S_T_SpaceDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_SpaceDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
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
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public int SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string RelateWBSAttrCode { get; set; } // RelateWBSAttrCode
		/// <summary></summary>	
		[Description("")]
        public string DefineType { get; set; } // DefineType
		/// <summary></summary>	
		[Description("")]
        public string DynamicDataFiled { get; set; } // DynamicDataFiled
		/// <summary></summary>	
		[Description("")]
        public string FeatureID { get; set; } // FeatureID
		/// <summary></summary>	
		[Description("")]
        public string IsDisplay { get; set; } // IsDisplay
		/// <summary></summary>	
		[Description("")]
        public string DisplayField { get; set; } // DisplayField
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_SpaceAuth> S_T_SpaceAuth { get { onS_T_SpaceAuthGetting(); return _S_T_SpaceAuth;} }
		private ICollection<S_T_SpaceAuth> _S_T_SpaceAuth;
		partial void onS_T_SpaceAuthGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_SpaceDefine_S_T_ProjectMode

        public S_T_SpaceDefine()
        {
            _S_T_SpaceAuth = new List<S_T_SpaceAuth>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ToDoListDefine : Formula.BaseModel
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
        public string Remak { get; set; } // Remak
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_ToDoListDefineNode> S_T_ToDoListDefineNode { get { onS_T_ToDoListDefineNodeGetting(); return _S_T_ToDoListDefineNode;} }
		private ICollection<S_T_ToDoListDefineNode> _S_T_ToDoListDefineNode;
		partial void onS_T_ToDoListDefineNodeGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_ToDoListDefine_S_T_ProjectMode

        public S_T_ToDoListDefine()
        {
            _S_T_ToDoListDefineNode = new List<S_T_ToDoListDefineNode>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ToDoListDefineNode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DefineID { get; set; } // DefineID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
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
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Category { get; set; } // Category
		/// <summary></summary>	
		[Description("")]
        public int SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary></summary>	
		[Description("")]
        public string UserIDs { get; set; } // UserIDs
		/// <summary></summary>	
		[Description("")]
        public string UserNames { get; set; } // UserNames
		/// <summary></summary>	
		[Description("")]
        public string UserIDsFromField { get; set; } // UserIDsFromField
		/// <summary></summary>	
		[Description("")]
        public string RoleIDs { get; set; } // RoleIDs
		/// <summary></summary>	
		[Description("")]
        public string RoleNames { get; set; } // RoleNames
		/// <summary></summary>	
		[Description("")]
        public string RoleIDsFromField { get; set; } // RoleIDsFromField
		/// <summary></summary>	
		[Description("")]
        public string OrgIDs { get; set; } // OrgIDs
		/// <summary></summary>	
		[Description("")]
        public string OrgNames { get; set; } // OrgNames
		/// <summary></summary>	
		[Description("")]
        public string OrgIDFromField { get; set; } // OrgIDFromField
		/// <summary></summary>	
		[Description("")]
        public string TriggeringCondition { get; set; } // TriggeringCondition
		/// <summary></summary>	
		[Description("")]
        public string ExecCondition { get; set; } // ExecCondition
		/// <summary></summary>	
		[Description("")]
        public string CloseCondition { get; set; } // CloseCondition
		/// <summary></summary>	
		[Description("")]
        public string ExeAction { get; set; } // ExeAction
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ToDoListDefine S_T_ToDoListDefine { get; set; } //  DefineID - FK_S_T_ToDoListDefineNode_S_T_ToDoListDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_WBSStructInfo : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string ChildCode { get; set; } // ChildCode
		/// <summary></summary>	
		[Description("")]
        public string ChildName { get; set; } // ChildName
		/// <summary></summary>	
		[Description("")]
        public string DynEnum { get; set; } // DynEnum
		/// <summary></summary>	
		[Description("")]
        public string WBSAttrInfo { get; set; } // WBSAttrInfo
		/// <summary></summary>	
		[Description("")]
        public string CanTransform { get; set; } // CanTransform
		/// <summary></summary>	
		[Description("")]
        public string CodeDefine { get; set; } // CodeDefine
		/// <summary></summary>	
		[Description("")]
        public string DefaultValueJson { get; set; } // DefaultValueJson
		/// <summary></summary>	
		[Description("")]
        public int? PadLength { get; set; } // PadLength
		/// <summary></summary>	
		[Description("")]
        public string PadChar { get; set; } // PadChar
		/// <summary></summary>	
		[Description("")]
        public string PadType { get; set; } // PadType

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_WBSStructRole> S_T_WBSStructRole { get { onS_T_WBSStructRoleGetting(); return _S_T_WBSStructRole;} }
		private ICollection<S_T_WBSStructRole> _S_T_WBSStructRole;
		partial void onS_T_WBSStructRoleGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ProjectMode S_T_ProjectMode { get; set; } //  ModeID - FK_S_T_WBSStructInfo_S_T_ProjectMode

        public S_T_WBSStructInfo()
        {
            _S_T_WBSStructRole = new List<S_T_WBSStructRole>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_WBSStructRole : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string WBSStructID { get; set; } // WBSStructID
		/// <summary></summary>	
		[Description("")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleName { get; set; } // RoleName
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string RoleRelation { get; set; } // RoleRelation
		/// <summary></summary>	
		[Description("")]
        public string Multi { get; set; } // Multi
		/// <summary></summary>	
		[Description("")]
        public string SychWBS { get; set; } // SychWBS
		/// <summary></summary>	
		[Description("")]
        public string SychWBSField { get; set; } // SychWBSField
		/// <summary></summary>	
		[Description("")]
        public string SychProject { get; set; } // SychProject
		/// <summary></summary>	
		[Description("")]
        public string SychProjectField { get; set; } // SychProjectField
		/// <summary></summary>	
		[Description("")]
        public int SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_WBSStructInfo S_T_WBSStructInfo { get; set; } //  WBSStructID - FK_S_T_WBSStructRole_S_T_WBSStructInfo
    }


    // ************************************************************************
    // POCO Configuration

    // S_C_CBSDefine
    internal partial class S_C_CBSDefineConfiguration : EntityTypeConfiguration<S_C_CBSDefine>
    {
        public S_C_CBSDefineConfiguration()
        {
			ToTable("S_C_CBSDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.CBSCode).HasColumnName("CBSCODE").IsRequired().HasMaxLength(50);
            Property(x => x.CBSName).HasColumnName("CBSNAME").IsRequired().HasMaxLength(50);
            Property(x => x.CBSType).HasColumnName("CBSTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_C_CBSDefine).HasForeignKey(c => c.ModeID); // FK_S_C_CBSDefine_S_T_ProjectMode
        }
    }

    // S_C_MajorWorkloadDefine
    internal partial class S_C_MajorWorkloadDefineConfiguration : EntityTypeConfiguration<S_C_MajorWorkloadDefine>
    {
        public S_C_MajorWorkloadDefineConfiguration()
        {
			ToTable("S_C_MAJORWORKLOADDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.MajorName).HasColumnName("MAJORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MajorValue).HasColumnName("MAJORVALUE").IsOptional().HasMaxLength(200);
            Property(x => x.Rate).HasColumnName("RATE").IsOptional().HasPrecision(18,2);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_C_ManageWorkloadDefine
    internal partial class S_C_ManageWorkloadDefineConfiguration : EntityTypeConfiguration<S_C_ManageWorkloadDefine>
    {
        public S_C_ManageWorkloadDefineConfiguration()
        {
			ToTable("S_C_MANAGEWORKLOADDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Rate).HasColumnName("RATE").IsOptional().HasPrecision(18,2);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_C_RoleWorkloadDefine
    internal partial class S_C_RoleWorkloadDefineConfiguration : EntityTypeConfiguration<S_C_RoleWorkloadDefine>
    {
        public S_C_RoleWorkloadDefineConfiguration()
        {
			ToTable("S_C_ROLEWORKLOADDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleName).HasColumnName("ROLENAME").IsOptional().HasMaxLength(200);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsOptional().HasMaxLength(200);
            Property(x => x.Rate).HasColumnName("RATE").IsOptional().HasPrecision(18,2);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_CAD_AreaAuth
    internal partial class S_CAD_AreaAuthConfiguration : EntityTypeConfiguration<S_CAD_AreaAuth>
    {
        public S_CAD_AreaAuthConfiguration()
        {
			ToTable("S_CAD_AREAAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.AuthType).HasColumnName("AUTHTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.HasCRUDAuth).HasColumnName("HASCRUDAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.HasPrintAuth).HasColumnName("HASPRINTAUTH").IsOptional().HasMaxLength(50);
        }
    }

    // S_CAD_PublishPrice
    internal partial class S_CAD_PublishPriceConfiguration : EntityTypeConfiguration<S_CAD_PublishPrice>
    {
        public S_CAD_PublishPriceConfiguration()
        {
			ToTable("S_CAD_PUBLISHPRICE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.PublicationType).HasColumnName("PUBLICATIONTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.Specification).HasColumnName("SPECIFICATION").IsRequired().HasMaxLength(50);
            Property(x => x.Price).HasColumnName("PRICE").IsRequired().HasPrecision(18,4);
        }
    }

    // S_D_DesignInputDefine
    internal partial class S_D_DesignInputDefineConfiguration : EntityTypeConfiguration<S_D_DesignInputDefine>
    {
        public S_D_DesignInputDefineConfiguration()
        {
			ToTable("S_D_DESIGNINPUTDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Class).HasColumnName("CLASS").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsOptional().HasMaxLength(500);
            Property(x => x.PhaseValues).HasColumnName("PHASEVALUES").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.DBSCode).HasColumnName("DBSCODE").IsOptional().HasMaxLength(200);
            Property(x => x.DocType).HasColumnName("DOCTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.InputTypeIndex).HasColumnName("INPUTTYPEINDEX").IsOptional();
        }
    }

    // S_D_EPSDef
    internal partial class S_D_EPSDefConfiguration : EntityTypeConfiguration<S_D_EPSDef>
    {
        public S_D_EPSDefConfiguration()
        {
			ToTable("S_D_EPSDEF");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.GroupField).HasColumnName("GROUPFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.GroupTable).HasColumnName("GROUPTABLE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_D_Feature
    internal partial class S_D_FeatureConfiguration : EntityTypeConfiguration<S_D_Feature>
    {
        public S_D_FeatureConfiguration()
        {
			ToTable("S_D_FEATURE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional();
            Property(x => x.RelateWBSAttrCode).HasColumnName("RELATEWBSATTRCODE").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_D_PackageDic
    internal partial class S_D_PackageDicConfiguration : EntityTypeConfiguration<S_D_PackageDic>
    {
        public S_D_PackageDicConfiguration()
        {
			ToTable("S_D_PACKAGEDIC");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.PackageType).HasColumnName("PACKAGETYPE").IsOptional().HasMaxLength(200);
            Property(x => x.MajorCode).HasColumnName("MAJORCODE").IsOptional().HasMaxLength(200);
            Property(x => x.MajorName).HasColumnName("MAJORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PhaseCode).HasColumnName("PHASECODE").IsOptional().HasMaxLength(200);
            Property(x => x.PhaseName).HasColumnName("PHASENAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsOptional().HasMaxLength(200);
            Property(x => x.AuditLevel).HasColumnName("AUDITLEVEL").IsOptional().HasMaxLength(200);
            Property(x => x.WorkLoad).HasColumnName("WORKLOAD").IsOptional().HasPrecision(18,2);
            Property(x => x.DrawingCount).HasColumnName("DRAWINGCOUNT").IsOptional().HasPrecision(18,2);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.ModeCodes).HasColumnName("MODECODES").IsOptional().HasMaxLength(500);
            Property(x => x.DeviceName).HasColumnName("DEVICENAME").IsOptional().HasMaxLength(200);
            Property(x => x.AreaName).HasColumnName("AREANAME").IsOptional().HasMaxLength(200);
            Property(x => x.EntityName).HasColumnName("ENTITYNAME").IsOptional().HasMaxLength(200);
            Property(x => x.WBSType).HasColumnName("WBSTYPE").IsOptional().HasMaxLength(200);
        }
    }

    // S_D_PackageDic_RoleRate
    internal partial class S_D_PackageDic_RoleRateConfiguration : EntityTypeConfiguration<S_D_PackageDic_RoleRate>
    {
        public S_D_PackageDic_RoleRateConfiguration()
        {
			ToTable("S_D_PACKAGEDIC_ROLERATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.S_D_PackageDicID).HasColumnName("S_D_PACKAGEDICID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Role).HasColumnName("ROLE").IsOptional().HasMaxLength(200);
            Property(x => x.Rate).HasColumnName("RATE").IsOptional().HasPrecision(18,2);
            Property(x => x.WorkLoad).HasColumnName("WORKLOAD").IsOptional().HasPrecision(18,2);

            // Foreign keys
            HasOptional(a => a.S_D_PackageDic).WithMany(b => b.S_D_PackageDic_RoleRate).HasForeignKey(c => c.S_D_PackageDicID); // FK_S_D_PackageDic_RoleRate_S_D_PackageDic
        }
    }

    // S_D_RoleDefine
    internal partial class S_D_RoleDefineConfiguration : EntityTypeConfiguration<S_D_RoleDefine>
    {
        public S_D_RoleDefineConfiguration()
        {
			ToTable("S_D_ROLEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(500);
            Property(x => x.RoleName).HasColumnName("ROLENAME").IsRequired().HasMaxLength(500);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsOptional().HasMaxLength(500);
            Property(x => x.RoleRelation).HasColumnName("ROLERELATION").IsOptional().HasMaxLength(500);
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired().HasPrecision(18,2);
            Property(x => x.OtherName).HasColumnName("OTHERNAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_D_Standard
    internal partial class S_D_StandardConfiguration : EntityTypeConfiguration<S_D_Standard>
    {
        public S_D_StandardConfiguration()
        {
			ToTable("S_D_STANDARD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_D_StandardDocument
    internal partial class S_D_StandardDocumentConfiguration : EntityTypeConfiguration<S_D_StandardDocument>
    {
        public S_D_StandardDocumentConfiguration()
        {
			ToTable("S_D_STANDARDDOCUMENT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.StandardID).HasColumnName("STANDARDID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Version).HasColumnName("VERSION").IsOptional().HasMaxLength(50);
            Property(x => x.PublishMechanism).HasColumnName("PUBLISHMECHANISM").IsOptional().HasMaxLength(200);
            Property(x => x.PublishDate).HasColumnName("PUBLISHDATE").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(200);
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(200);
            Property(x => x.IsCurVersion).HasColumnName("ISCURVERSION").IsOptional().HasMaxLength(50);
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
        }
    }

    // S_D_WBSAttrDefine
    internal partial class S_D_WBSAttrDefineConfiguration : EntityTypeConfiguration<S_D_WBSAttrDefine>
    {
        public S_D_WBSAttrDefineConfiguration()
        {
			ToTable("S_D_WBSATTRDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.WBSCode).HasColumnName("WBSCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.BelongMode).HasColumnName("BELONGMODE").IsOptional().HasMaxLength(500);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);
        }
    }

    // S_D_WBSAttrDeptInfo
    internal partial class S_D_WBSAttrDeptInfoConfiguration : EntityTypeConfiguration<S_D_WBSAttrDeptInfo>
    {
        public S_D_WBSAttrDeptInfoConfiguration()
        {
			ToTable("S_D_WBSATTRDEPTINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.WBSAttrDefineID).HasColumnName("WBSATTRDEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.DeptID).HasColumnName("DEPTID").IsRequired().HasMaxLength(500);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsRequired().HasMaxLength(500);

            // Foreign keys
            HasRequired(a => a.S_D_WBSAttrDefine).WithMany(b => b.S_D_WBSAttrDeptInfo).HasForeignKey(c => c.WBSAttrDefineID); // FK_S_D_WBSAttrDeptInfo_S_D_WBSAttrDefine
        }
    }

    // S_D_WBSTemplate
    internal partial class S_D_WBSTemplateConfiguration : EntityTypeConfiguration<S_D_WBSTemplate>
    {
        public S_D_WBSTemplateConfiguration()
        {
			ToTable("S_D_WBSTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeCodes).HasColumnName("MODECODES").IsRequired().HasMaxLength(500);
            Property(x => x.ModeNames).HasColumnName("MODENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectType).HasColumnName("PROJECTTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectScope).HasColumnName("PROJECTSCOPE").IsOptional().HasMaxLength(500);
        }
    }

    // S_D_WBSTemplateNode
    internal partial class S_D_WBSTemplateNodeConfiguration : EntityTypeConfiguration<S_D_WBSTemplateNode>
    {
        public S_D_WBSTemplateNodeConfiguration()
        {
			ToTable("S_D_WBSTEMPLATENODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TemplateID).HasColumnName("TEMPLATEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(500);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.WBSValue).HasColumnName("WBSVALUE").IsRequired().HasMaxLength(500);
            Property(x => x.WBSType).HasColumnName("WBSTYPE").IsRequired().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.Level).HasColumnName("LEVEL").IsRequired();
            Property(x => x.WBSDeptID).HasColumnName("WBSDEPTID").IsOptional().HasMaxLength(500);
            Property(x => x.WBSDeptName).HasColumnName("WBSDEPTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkLoad).HasColumnName("WORKLOAD").IsOptional().HasPrecision(18,2);
            Property(x => x.PlanWorkLoad).HasColumnName("PLANWORKLOAD").IsOptional().HasPrecision(18,2);
            Property(x => x.Weight).HasColumnName("WEIGHT").IsOptional().HasPrecision(18,2);

            // Foreign keys
            HasRequired(a => a.S_D_WBSTemplate).WithMany(b => b.S_D_WBSTemplateNode).HasForeignKey(c => c.TemplateID); // FK_S_T_WBSTemplateNode_S_T_WBSTemplate
        }
    }

    // S_T_AuditMode
    internal partial class S_T_AuditModeConfiguration : EntityTypeConfiguration<S_T_AuditMode>
    {
        public S_T_AuditModeConfiguration()
        {
			ToTable("S_T_AUDITMODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ProjectModeID).HasColumnName("PROJECTMODEID").IsOptional().HasMaxLength(50);
            Property(x => x.AttrID).HasColumnName("ATTRID").IsOptional().HasMaxLength(50);
            Property(x => x.PhaseValue).HasColumnName("PHASEVALUE").IsOptional().HasMaxLength(50);
            Property(x => x.PhaseName).HasColumnName("PHASENAME").IsOptional().HasMaxLength(50);
            Property(x => x.AuditMode).HasColumnName("AUDITMODE").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_T_ProjectMode).WithMany(b => b.S_T_AuditMode).HasForeignKey(c => c.ProjectModeID); // FK_S_T_ProjectMode_S_T_AuditMode
        }
    }

    // S_T_DataAuth
    internal partial class S_T_DataAuthConfiguration : EntityTypeConfiguration<S_T_DataAuth>
    {
        public S_T_DataAuthConfiguration()
        {
			ToTable("S_T_DATAAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_DataAuth).HasForeignKey(c => c.ModeID); // FK_S_T_DataAuth_S_T_ProjectMode
        }
    }

    // S_T_DBSDefine
    internal partial class S_T_DBSDefineConfiguration : EntityTypeConfiguration<S_T_DBSDefine>
    {
        public S_T_DBSDefineConfiguration()
        {
			ToTable("S_T_DBSDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.DBSCode).HasColumnName("DBSCODE").IsRequired().HasMaxLength(500);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Level).HasColumnName("LEVEL").IsRequired();
            Property(x => x.DBSType).HasColumnName("DBSTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.MappingType).HasColumnName("MAPPINGTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.MappingNodeUrl).HasColumnName("MAPPINGNODEURL").IsOptional().HasMaxLength(500);
            Property(x => x.InheritAuth).HasColumnName("INHERITAUTH").IsRequired().HasMaxLength(50);
            Property(x => x.ArchiveFolder).HasColumnName("ARCHIVEFOLDER").IsOptional().HasMaxLength(50);
            Property(x => x.ArchiveFolderName).HasColumnName("ARCHIVEFOLDERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ProductStruct).HasColumnName("PRODUCTSTRUCT").IsOptional().HasMaxLength(1073741823);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_DBSDefine).HasForeignKey(c => c.ModeID); // FK_S_T_DBSDefine_S_T_ProjectMode
        }
    }

    // S_T_DBSSecurity
    internal partial class S_T_DBSSecurityConfiguration : EntityTypeConfiguration<S_T_DBSSecurity>
    {
        public S_T_DBSSecurityConfiguration()
        {
			ToTable("S_T_DBSSECURITY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DBSDefineID).HasColumnName("DBSDEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleName).HasColumnName("ROLENAME").IsRequired().HasMaxLength(50);
            Property(x => x.AuthType).HasColumnName("AUTHTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_DBSDefine).WithMany(b => b.S_T_DBSSecurity).HasForeignKey(c => c.DBSDefineID); // FK_S_T_DBSSecurity_S_T_DBSSecurity
        }
    }

    // S_T_EngineeringSpace
    internal partial class S_T_EngineeringSpaceConfiguration : EntityTypeConfiguration<S_T_EngineeringSpace>
    {
        public S_T_EngineeringSpaceConfiguration()
        {
			ToTable("S_T_ENGINEERINGSPACE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_T_EngineeringSpaceAuth
    internal partial class S_T_EngineeringSpaceAuthConfiguration : EntityTypeConfiguration<S_T_EngineeringSpaceAuth>
    {
        public S_T_EngineeringSpaceAuthConfiguration()
        {
			ToTable("S_T_ENGINEERINGSPACEAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.EngineeringSpaceResID).HasColumnName("ENGINEERINGSPACERESID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.AuthType).HasColumnName("AUTHTYPE").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_EngineeringSpaceRes).WithMany(b => b.S_T_EngineeringSpaceAuth).HasForeignKey(c => c.EngineeringSpaceResID); // FK_S_T_EngineeringSpaceAuth_S_T_EngineeringSpaceRes
        }
    }

    // S_T_EngineeringSpaceRes
    internal partial class S_T_EngineeringSpaceResConfiguration : EntityTypeConfiguration<S_T_EngineeringSpaceRes>
    {
        public S_T_EngineeringSpaceResConfiguration()
        {
			ToTable("S_T_ENGINEERINGSPACERES");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(500);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional().HasMaxLength(1073741823);

            // Foreign keys
            HasRequired(a => a.S_T_EngineeringSpace).WithMany(b => b.S_T_EngineeringSpaceRes).HasForeignKey(c => c.SpaceID); // FK_S_T_EngineeringSpaceRes_S_T_EngineeringSpace
        }
    }

    // S_T_FlowTraceDefine
    internal partial class S_T_FlowTraceDefineConfiguration : EntityTypeConfiguration<S_T_FlowTraceDefine>
    {
        public S_T_FlowTraceDefineConfiguration()
        {
			ToTable("S_T_FLOWTRACEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.StateField).HasColumnName("STATEFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.StepField).HasColumnName("STEPFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.NameFieldInfo).HasColumnName("NAMEFIELDINFO").IsOptional().HasMaxLength(500);
            Property(x => x.EnumFieldInfo).HasColumnName("ENUMFIELDINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LinkFormUrl).HasColumnName("LINKFORMURL").IsOptional().HasMaxLength(500);
            Property(x => x.LinkField).HasColumnName("LINKFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_FlowTraceDefine).HasForeignKey(c => c.ModeID); // FK_S_T_FlowTraceDefine_S_T_ProjectMode
        }
    }

    // S_T_HotRangeStatisticsConfig
    internal partial class S_T_HotRangeStatisticsConfigConfiguration : EntityTypeConfiguration<S_T_HotRangeStatisticsConfig>
    {
        public S_T_HotRangeStatisticsConfigConfiguration()
        {
			ToTable("S_T_HOTRANGESTATISTICSCONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Field).HasColumnName("FIELD").IsOptional().HasMaxLength(200);
            Property(x => x.Long).HasColumnName("LONG").IsOptional();
            Property(x => x.Lat).HasColumnName("LAT").IsOptional();
            Property(x => x.MinZoomVal).HasColumnName("MINZOOMVAL").IsOptional();
            Property(x => x.MaxZoomVal).HasColumnName("MAXZOOMVAL").IsOptional();
            Property(x => x.SubList).HasColumnName("SUBLIST").IsOptional();
            Property(x => x.UpName).HasColumnName("UPNAME").IsOptional().HasMaxLength(200);
            Property(x => x.SubField).HasColumnName("SUBFIELD").IsOptional().HasMaxLength(200);
            Property(x => x.Enable).HasColumnName("ENABLE").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_ISODefine
    internal partial class S_T_ISODefineConfiguration : EntityTypeConfiguration<S_T_ISODefine>
    {
        public S_T_ISODefineConfiguration()
        {
			ToTable("S_T_ISODEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.Catagory).HasColumnName("CATAGORY").IsRequired().HasMaxLength(50);
            Property(x => x.LinkFormUrl).HasColumnName("LINKFORMURL").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.NameFieldInfo).HasColumnName("NAMEFIELDINFO").IsRequired().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.EnumFieldInfo).HasColumnName("ENUMFIELDINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CanAddNewForm).HasColumnName("CANADDNEWFORM").IsRequired().HasMaxLength(50);
            Property(x => x.ArchiveFields).HasColumnName("ARCHIVEFIELDS").IsOptional().HasMaxLength(500);
            Property(x => x.SendNotice).HasColumnName("SENDNOTICE").IsOptional().HasMaxLength(50);
            Property(x => x.FormCode).HasColumnName("FORMCODE").IsOptional().HasMaxLength(500);
            Property(x => x.FlowCode).HasColumnName("FLOWCODE").IsOptional().HasMaxLength(500);
            Property(x => x.LinkViewUrl).HasColumnName("LINKVIEWURL").IsOptional().HasMaxLength(500);
            Property(x => x.StartNoticeContent).HasColumnName("STARTNOTICECONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.EndNoticeContent).HasColumnName("ENDNOTICECONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.StepNoticeContent).HasColumnName("STEPNOTICECONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.Level).HasColumnName("LEVEL").IsOptional().HasMaxLength(500);
            Property(x => x.MajorField).HasColumnName("MAJORFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.ExpiresDate).HasColumnName("EXPIRESDATE").IsOptional().HasPrecision(18,0);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_ISODefine).HasForeignKey(c => c.ModeID); // FK_S_T_ISODefine_S_T_ProjectMode
        }
    }

    // S_T_MileStone
    internal partial class S_T_MileStoneConfiguration : EntityTypeConfiguration<S_T_MileStone>
    {
        public S_T_MileStoneConfiguration()
        {
			ToTable("S_T_MILESTONE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.MileStoneName).HasColumnName("MILESTONENAME").IsRequired().HasMaxLength(500);
            Property(x => x.MileStoneCode).HasColumnName("MILESTONECODE").IsOptional().HasMaxLength(500);
            Property(x => x.MileStoneType).HasColumnName("MILESTONETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.DefaultTimeSpan).HasColumnName("DEFAULTTIMESPAN").IsOptional();
            Property(x => x.Weight).HasColumnName("WEIGHT").IsOptional().HasPrecision(18,0);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsOptional().HasMaxLength(50);
            Property(x => x.PhaseValue).HasColumnName("PHASEVALUE").IsOptional().HasMaxLength(50);
            Property(x => x.Necessity).HasColumnName("NECESSITY").IsOptional().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.OutMajors).HasColumnName("OUTMAJORS").IsOptional().HasMaxLength(50);
            Property(x => x.InMajors).HasColumnName("INMAJORS").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_MileStone).HasForeignKey(c => c.ModeID); // FK_S_T_MileStone_S_T_ProjectMode
        }
    }

    // S_T_ProjectMode
    internal partial class S_T_ProjectModeConfiguration : EntityTypeConfiguration<S_T_ProjectMode>
    {
        public S_T_ProjectModeConfiguration()
        {
			ToTable("S_T_PROJECTMODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.ModeCode).HasColumnName("MODECODE").IsRequired().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional();
            Property(x => x.Condition).HasColumnName("CONDITION").IsOptional().HasMaxLength(1073741823);
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsRequired().HasMaxLength(50);
            Property(x => x.Priority).HasColumnName("PRIORITY").IsRequired();
            Property(x => x.ExtentionJson).HasColumnName("EXTENTIONJSON").IsOptional();
        }
    }

    // S_T_QBSTemplate
    internal partial class S_T_QBSTemplateConfiguration : EntityTypeConfiguration<S_T_QBSTemplate>
    {
        public S_T_QBSTemplateConfiguration()
        {
			ToTable("S_T_QBSTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.QBSType).HasColumnName("QBSTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsRequired().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.DataUnit).HasColumnName("DATAUNIT").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_QBSTemplate).HasForeignKey(c => c.ModeID); // FK_S_T_QBSTemplate_S_T_ProjectMode
        }
    }

    // S_T_SpaceAuth
    internal partial class S_T_SpaceAuthConfiguration : EntityTypeConfiguration<S_T_SpaceAuth>
    {
        public S_T_SpaceAuthConfiguration()
        {
			ToTable("S_T_SPACEAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.AuthType).HasColumnName("AUTHTYPE").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_SpaceDefine).WithMany(b => b.S_T_SpaceAuth).HasForeignKey(c => c.SpaceID); // FK_S_T_SpaceAuth_S_T_SpaceDefine
        }
    }

    // S_T_SpaceDefine
    internal partial class S_T_SpaceDefineConfiguration : EntityTypeConfiguration<S_T_SpaceDefine>
    {
        public S_T_SpaceDefineConfiguration()
        {
			ToTable("S_T_SPACEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.RelateWBSAttrCode).HasColumnName("RELATEWBSATTRCODE").IsOptional();
            Property(x => x.DefineType).HasColumnName("DEFINETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.DynamicDataFiled).HasColumnName("DYNAMICDATAFILED").IsOptional().HasMaxLength(100);
            Property(x => x.FeatureID).HasColumnName("FEATUREID").IsOptional().HasMaxLength(50);
            Property(x => x.IsDisplay).HasColumnName("ISDISPLAY").IsOptional().HasMaxLength(20);
            Property(x => x.DisplayField).HasColumnName("DISPLAYFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_SpaceDefine).HasForeignKey(c => c.ModeID); // FK_S_T_SpaceDefine_S_T_ProjectMode
        }
    }

    // S_T_ToDoListDefine
    internal partial class S_T_ToDoListDefineConfiguration : EntityTypeConfiguration<S_T_ToDoListDefine>
    {
        public S_T_ToDoListDefineConfiguration()
        {
			ToTable("S_T_TODOLISTDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.Remak).HasColumnName("REMAK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_T_ProjectMode).WithMany(b => b.S_T_ToDoListDefine).HasForeignKey(c => c.ModeID); // FK_S_T_ToDoListDefine_S_T_ProjectMode
        }
    }

    // S_T_ToDoListDefineNode
    internal partial class S_T_ToDoListDefineNodeConfiguration : EntityTypeConfiguration<S_T_ToDoListDefineNode>
    {
        public S_T_ToDoListDefineNodeConfiguration()
        {
			ToTable("S_T_TODOLISTDEFINENODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefineID).HasColumnName("DEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(200);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(200);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(200);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDs).HasColumnName("USERIDS").IsOptional().HasMaxLength(500);
            Property(x => x.UserNames).HasColumnName("USERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.UserIDsFromField).HasColumnName("USERIDSFROMFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.RoleIDs).HasColumnName("ROLEIDS").IsOptional().HasMaxLength(500);
            Property(x => x.RoleNames).HasColumnName("ROLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.RoleIDsFromField).HasColumnName("ROLEIDSFROMFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.OrgIDs).HasColumnName("ORGIDS").IsOptional().HasMaxLength(500);
            Property(x => x.OrgNames).HasColumnName("ORGNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.OrgIDFromField).HasColumnName("ORGIDFROMFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.TriggeringCondition).HasColumnName("TRIGGERINGCONDITION").IsOptional();
            Property(x => x.ExecCondition).HasColumnName("EXECCONDITION").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CloseCondition).HasColumnName("CLOSECONDITION").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ExeAction).HasColumnName("EXEACTION").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional();
            Property(x => x.ModeID).HasColumnName("MODEID").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_ToDoListDefine).WithMany(b => b.S_T_ToDoListDefineNode).HasForeignKey(c => c.DefineID); // FK_S_T_ToDoListDefineNode_S_T_ToDoListDefine
        }
    }

    // S_T_WBSStructInfo
    internal partial class S_T_WBSStructInfoConfiguration : EntityTypeConfiguration<S_T_WBSStructInfo>
    {
        public S_T_WBSStructInfoConfiguration()
        {
			ToTable("S_T_WBSSTRUCTINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(200);
            Property(x => x.ChildCode).HasColumnName("CHILDCODE").IsOptional().HasMaxLength(500);
            Property(x => x.ChildName).HasColumnName("CHILDNAME").IsOptional().HasMaxLength(500);
            Property(x => x.DynEnum).HasColumnName("DYNENUM").IsOptional().HasMaxLength(50);
            Property(x => x.WBSAttrInfo).HasColumnName("WBSATTRINFO").IsOptional();
            Property(x => x.CanTransform).HasColumnName("CANTRANSFORM").IsOptional().HasMaxLength(50);
            Property(x => x.CodeDefine).HasColumnName("CODEDEFINE").IsOptional();
            Property(x => x.DefaultValueJson).HasColumnName("DEFAULTVALUEJSON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.PadLength).HasColumnName("PADLENGTH").IsOptional();
            Property(x => x.PadChar).HasColumnName("PADCHAR").IsOptional().HasMaxLength(50);
            Property(x => x.PadType).HasColumnName("PADTYPE").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_ProjectMode).WithMany(b => b.S_T_WBSStructInfo).HasForeignKey(c => c.ModeID); // FK_S_T_WBSStructInfo_S_T_ProjectMode
        }
    }

    // S_T_WBSStructRole
    internal partial class S_T_WBSStructRoleConfiguration : EntityTypeConfiguration<S_T_WBSStructRole>
    {
        public S_T_WBSStructRoleConfiguration()
        {
			ToTable("S_T_WBSSTRUCTROLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.WBSStructID).HasColumnName("WBSSTRUCTID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleName).HasColumnName("ROLENAME").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleRelation).HasColumnName("ROLERELATION").IsOptional().HasMaxLength(500);
            Property(x => x.Multi).HasColumnName("MULTI").IsOptional().HasMaxLength(50);
            Property(x => x.SychWBS).HasColumnName("SYCHWBS").IsRequired().HasMaxLength(50);
            Property(x => x.SychWBSField).HasColumnName("SYCHWBSFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.SychProject).HasColumnName("SYCHPROJECT").IsRequired().HasMaxLength(50);
            Property(x => x.SychProjectField).HasColumnName("SYCHPROJECTFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();

            // Foreign keys
            HasRequired(a => a.S_T_WBSStructInfo).WithMany(b => b.S_T_WBSStructRole).HasForeignKey(c => c.WBSStructID); // FK_S_T_WBSStructRole_S_T_WBSStructInfo
        }
    }

}

