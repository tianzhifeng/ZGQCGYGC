

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "Infrastructure"
//     Connection String:      "data source=10.10.1.244\sql2008;Initial Catalog=SINOAE_Infrastructure;User ID=sa;PWD=123.zxc;"

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

namespace EPC.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class InfrastructureEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_C_MenuAuth> S_C_MenuAuth { get; set; } // S_C_MenuAuth
        public IDbSet<S_C_Meun> S_C_Meun { get; set; } // S_C_Meun
        public IDbSet<S_C_MileStoneTemplate> S_C_MileStoneTemplate { get; set; } // S_C_MileStoneTemplate
        public IDbSet<S_C_Mode> S_C_Mode { get; set; } // S_C_Mode
        public IDbSet<S_C_OBSTemplate> S_C_OBSTemplate { get; set; } // S_C_OBSTemplate
        public IDbSet<S_C_PBSStruct> S_C_PBSStruct { get; set; } // S_C_PBSStruct
        public IDbSet<S_C_QBSStruct> S_C_QBSStruct { get; set; } // S_C_QBSStruct
        public IDbSet<S_C_ScheduleDefine> S_C_ScheduleDefine { get; set; } // S_C_ScheduleDefine
        public IDbSet<S_C_ScheduleDefine_Nodes> S_C_ScheduleDefine_Nodes { get; set; } // S_C_ScheduleDefine_Nodes
        public IDbSet<S_C_WBSStruct> S_C_WBSStruct { get; set; } // S_C_WBSStruct
        public IDbSet<S_F_CapitalPlanTemplate> S_F_CapitalPlanTemplate { get; set; } // S_F_CapitalPlanTemplate
        public IDbSet<S_F_CapitalPlanTemplate_Detail> S_F_CapitalPlanTemplate_Detail { get; set; } // S_F_CapitalPlanTemplate_Detail
        public IDbSet<S_HR_EmployeeBaseSet> S_HR_EmployeeBaseSet { get; set; } // S_HR_EmployeeBaseSet
        public IDbSet<S_P_ProcurementContractPaymentObjTemplate> S_P_ProcurementContractPaymentObjTemplate { get; set; } // S_P_ProcurementContractPaymentObjTemplate
        public IDbSet<S_T_BomDefine> S_T_BomDefine { get; set; } // S_T_BomDefine
        public IDbSet<S_T_BomDefine_Detail> S_T_BomDefine_Detail { get; set; } // S_T_BomDefine_Detail
        public IDbSet<S_T_BOQTemplate> S_T_BOQTemplate { get; set; } // S_T_BOQTemplate
        public IDbSet<S_T_CBSDefine> S_T_CBSDefine { get; set; } // S_T_CBSDefine
        public IDbSet<S_T_CBSNodeTemplate> S_T_CBSNodeTemplate { get; set; } // S_T_CBSNodeTemplate
        public IDbSet<S_T_ConstructionQuantity> S_T_ConstructionQuantity { get; set; } // S_T_ConstructionQuantity
        public IDbSet<S_T_ConstructionQuantityDetail> S_T_ConstructionQuantityDetail { get; set; } // S_T_ConstructionQuantityDetail
        public IDbSet<S_T_DefineParams> S_T_DefineParams { get; set; } // S_T_DefineParams
        public IDbSet<S_T_DesignInputDefine> S_T_DesignInputDefine { get; set; } // S_T_DesignInputDefine
        public IDbSet<S_T_EquipmentMaterialCategory> S_T_EquipmentMaterialCategory { get; set; } // S_T_EquipmentMaterialCategory
        public IDbSet<S_T_EquipmentMaterialPriceTemplate> S_T_EquipmentMaterialPriceTemplate { get; set; } // S_T_EquipmentMaterialPriceTemplate
        public IDbSet<S_T_EquipmentMaterialTemplate> S_T_EquipmentMaterialTemplate { get; set; } // S_T_EquipmentMaterialTemplate
        public IDbSet<S_T_FinanceSubject> S_T_FinanceSubject { get; set; } // S_T_FinanceSubject
        public IDbSet<S_T_FolderAuth> S_T_FolderAuth { get; set; } // S_T_FolderAuth
        public IDbSet<S_T_FolderDef> S_T_FolderDef { get; set; } // S_T_FolderDef
        public IDbSet<S_T_FolderTemplate> S_T_FolderTemplate { get; set; } // S_T_FolderTemplate
        public IDbSet<S_T_HotRangeStatisticsConfig> S_T_HotRangeStatisticsConfig { get; set; } // S_T_HotRangeStatisticsConfig
        public IDbSet<S_T_ISODefine> S_T_ISODefine { get; set; } // S_T_ISODefine
        public IDbSet<S_T_NodeTemplate> S_T_NodeTemplate { get; set; } // S_T_NodeTemplate
        public IDbSet<S_T_NodeTemplate_Detail> S_T_NodeTemplate_Detail { get; set; } // S_T_NodeTemplate_Detail
        public IDbSet<S_T_RoleDefine> S_T_RoleDefine { get; set; } // S_T_RoleDefine
        public IDbSet<S_T_TaskTemplate> S_T_TaskTemplate { get; set; } // S_T_TaskTemplate
        public IDbSet<S_T_TaskTemplate_Detail> S_T_TaskTemplate_Detail { get; set; } // S_T_TaskTemplate_Detail
        public IDbSet<S_T_ToDoListDefine> S_T_ToDoListDefine { get; set; } // S_T_ToDoListDefine
        public IDbSet<S_T_ToDoListDefineNode> S_T_ToDoListDefineNode { get; set; } // S_T_ToDoListDefineNode
        public IDbSet<S_T_WBSAttrDefine> S_T_WBSAttrDefine { get; set; } // S_T_WBSAttrDefine
        public IDbSet<S_T_WBSTypeDefine> S_T_WBSTypeDefine { get; set; } // S_T_WBSTypeDefine

        static InfrastructureEntities()
        {
            Database.SetInitializer<InfrastructureEntities>(null);
        }

        public InfrastructureEntities()
            : base("Name=Infrastructure")
        {
        }

        public InfrastructureEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_C_MenuAuthConfiguration());
            modelBuilder.Configurations.Add(new S_C_MeunConfiguration());
            modelBuilder.Configurations.Add(new S_C_MileStoneTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_C_ModeConfiguration());
            modelBuilder.Configurations.Add(new S_C_OBSTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_C_PBSStructConfiguration());
            modelBuilder.Configurations.Add(new S_C_QBSStructConfiguration());
            modelBuilder.Configurations.Add(new S_C_ScheduleDefineConfiguration());
            modelBuilder.Configurations.Add(new S_C_ScheduleDefine_NodesConfiguration());
            modelBuilder.Configurations.Add(new S_C_WBSStructConfiguration());
            modelBuilder.Configurations.Add(new S_F_CapitalPlanTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_F_CapitalPlanTemplate_DetailConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeBaseSetConfiguration());
            modelBuilder.Configurations.Add(new S_P_ProcurementContractPaymentObjTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_BomDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_BomDefine_DetailConfiguration());
            modelBuilder.Configurations.Add(new S_T_BOQTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_CBSDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_CBSNodeTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_ConstructionQuantityConfiguration());
            modelBuilder.Configurations.Add(new S_T_ConstructionQuantityDetailConfiguration());
            modelBuilder.Configurations.Add(new S_T_DefineParamsConfiguration());
            modelBuilder.Configurations.Add(new S_T_DesignInputDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_EquipmentMaterialCategoryConfiguration());
            modelBuilder.Configurations.Add(new S_T_EquipmentMaterialPriceTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_EquipmentMaterialTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_FinanceSubjectConfiguration());
            modelBuilder.Configurations.Add(new S_T_FolderAuthConfiguration());
            modelBuilder.Configurations.Add(new S_T_FolderDefConfiguration());
            modelBuilder.Configurations.Add(new S_T_FolderTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_HotRangeStatisticsConfigConfiguration());
            modelBuilder.Configurations.Add(new S_T_ISODefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_NodeTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_NodeTemplate_DetailConfiguration());
            modelBuilder.Configurations.Add(new S_T_RoleDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_TaskTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_T_TaskTemplate_DetailConfiguration());
            modelBuilder.Configurations.Add(new S_T_ToDoListDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_ToDoListDefineNodeConfiguration());
            modelBuilder.Configurations.Add(new S_T_WBSAttrDefineConfiguration());
            modelBuilder.Configurations.Add(new S_T_WBSTypeDefineConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_MenuAuth : Formula.BaseModel
    {
		/// <summary>主键</summary>	
		[Description("主键")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>外键</summary>	
		[Description("外键")]
        public string MeunID { get; set; } // MeunID
		/// <summary>权限等级（只读，控制，当前控制)</summary>	
		[Description("权限等级（只读，控制，当前控制)")]
        public string AuthType { get; set; } // AuthType
		/// <summary>角色编号</summary>	
		[Description("角色编号")]
        public string RoleKey { get; set; } // RoleKey
		/// <summary>角色类型</summary>	
		[Description("角色类型")]
        public string RoleType { get; set; } // RoleType
		/// <summary></summary>	
		[Description("")]
        public string IsDefault { get; set; } // IsDefault

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Meun S_C_Meun { get; set; } //  MeunID - FK_S_C_MenuAuth_S_C_Meun
    }

	/// <summary>菜单定义</summary>	
	[Description("菜单定义")]
    public partial class S_C_Meun : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>菜单类型（静态，动态)</summary>	
		[Description("菜单类型（静态，动态)")]
        public string MeunType { get; set; } // MeunType
		/// <summary></summary>	
		[Description("")]
        public string MeunDefineType { get; set; } // MeunDefineType
		/// <summary></summary>	
		[Description("")]
        public string NavWBSType { get; set; } // NavWBSType
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary>链接地址</summary>	
		[Description("链接地址")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string DataSourceType { get; set; } // DataSourceType
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string DataSourceSQL { get; set; } // DataSourceSQL
		/// <summary></summary>	
		[Description("")]
        public string Condition { get; set; } // Condition
		/// <summary>外键</summary>	
		[Description("外键")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string NameEN { get; set; } // NameEN
		/// <summary></summary>	
		[Description("")]
        public string ConditionSQL { get; set; } // ConditionSQL
		/// <summary></summary>	
		[Description("")]
        public string Expanded { get; set; } // Expanded

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_C_MenuAuth> S_C_MenuAuth { get { onS_C_MenuAuthGetting(); return _S_C_MenuAuth;} }
		private ICollection<S_C_MenuAuth> _S_C_MenuAuth;
		partial void onS_C_MenuAuthGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_Meun_S_C_Mode

        public S_C_Meun()
        {
			Expanded = "False";
            _S_C_MenuAuth = new List<S_C_MenuAuth>();
        }
    }

	/// <summary>里程碑模板</summary>	
	[Description("里程碑模板")]
    public partial class S_C_MileStoneTemplate : Formula.BaseModel
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
		/// <summary>权重</summary>	
		[Description("权重")]
        public decimal? Weight { get; set; } // Weight
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_MileStoneTemplate_S_C_Mode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_Mode : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>模式名称</summary>	
		[Description("模式名称")]
        public string Name { get; set; } // Name
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Condition { get; set; } // Condition
		/// <summary></summary>	
		[Description("")]
        public int? Priority { get; set; } // Priority
		/// <summary></summary>	
		[Description("")]
        public string IsDefault { get; set; } // IsDefault
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string ToDoListDefine { get; set; } // ToDoListDefine
		/// <summary></summary>	
		[Description("")]
        public string ToDoListDefineName { get; set; } // ToDoListDefineName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_C_Meun> S_C_Meun { get { onS_C_MeunGetting(); return _S_C_Meun;} }
		private ICollection<S_C_Meun> _S_C_Meun;
		partial void onS_C_MeunGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_MileStoneTemplate> S_C_MileStoneTemplate { get { onS_C_MileStoneTemplateGetting(); return _S_C_MileStoneTemplate;} }
		private ICollection<S_C_MileStoneTemplate> _S_C_MileStoneTemplate;
		partial void onS_C_MileStoneTemplateGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_OBSTemplate> S_C_OBSTemplate { get { onS_C_OBSTemplateGetting(); return _S_C_OBSTemplate;} }
		private ICollection<S_C_OBSTemplate> _S_C_OBSTemplate;
		partial void onS_C_OBSTemplateGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_PBSStruct> S_C_PBSStruct { get { onS_C_PBSStructGetting(); return _S_C_PBSStruct;} }
		private ICollection<S_C_PBSStruct> _S_C_PBSStruct;
		partial void onS_C_PBSStructGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_QBSStruct> S_C_QBSStruct { get { onS_C_QBSStructGetting(); return _S_C_QBSStruct;} }
		private ICollection<S_C_QBSStruct> _S_C_QBSStruct;
		partial void onS_C_QBSStructGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_ScheduleDefine> S_C_ScheduleDefine { get { onS_C_ScheduleDefineGetting(); return _S_C_ScheduleDefine;} }
		private ICollection<S_C_ScheduleDefine> _S_C_ScheduleDefine;
		partial void onS_C_ScheduleDefineGetting();

		[JsonIgnore]
        public virtual ICollection<S_C_WBSStruct> S_C_WBSStruct { get { onS_C_WBSStructGetting(); return _S_C_WBSStruct;} }
		private ICollection<S_C_WBSStruct> _S_C_WBSStruct;
		partial void onS_C_WBSStructGetting();


        public S_C_Mode()
        {
            _S_C_Meun = new List<S_C_Meun>();
            _S_C_MileStoneTemplate = new List<S_C_MileStoneTemplate>();
            _S_C_OBSTemplate = new List<S_C_OBSTemplate>();
            _S_C_PBSStruct = new List<S_C_PBSStruct>();
            _S_C_QBSStruct = new List<S_C_QBSStruct>();
            _S_C_ScheduleDefine = new List<S_C_ScheduleDefine>();
            _S_C_WBSStruct = new List<S_C_WBSStruct>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_OBSTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string OBSType { get; set; } // OBSType
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_OBSTemplate_S_C_Mode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_PBSStruct : Formula.BaseModel
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
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public bool? IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public bool? CanSynToWBS { get; set; } // CanSynToWBS
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_PBSStruct_S_C_Mode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_QBSStruct : Formula.BaseModel
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
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public bool? IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public bool CanImportWBS { get; set; } // CanImportWBS
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_QBSStruct_S_C_Mode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_ScheduleDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ModeID { get; set; } // ModeID
		/// <summary></summary>	
		[Description("")]
        public string PreScheduleCode { get; set; } // PreScheduleCode
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public bool ImportProject { get; set; } // ImportProject
		/// <summary></summary>	
		[Description("")]
        public bool ImportTaskTemplate { get; set; } // ImportTaskTemplate
		/// <summary></summary>	
		[Description("")]
        public bool ImportBOM { get; set; } // ImportBOM
		/// <summary></summary>	
		[Description("")]
        public bool ImportQBS { get; set; } // ImportQBS
		/// <summary></summary>	
		[Description("")]
        public bool ImportBid { get; set; } // ImportBid
		/// <summary></summary>	
		[Description("")]
        public bool ImportExcel { get; set; } // ImportExcel
		/// <summary></summary>	
		[Description("")]
        public string TabData { get; set; } // TabData
		/// <summary></summary>	
		[Description("")]
        public string AttrDefine { get; set; } // AttrDefine
		/// <summary></summary>	
		[Description("")]
        public bool ShowTab { get; set; } // ShowTab
		/// <summary></summary>	
		[Description("")]
        public string ColDefine { get; set; } // ColDefine
		/// <summary></summary>	
		[Description("")]
        public bool? ProductionValueSchedule { get; set; } // ProductionValueSchedule

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_C_ScheduleDefine_Nodes> S_C_ScheduleDefine_Nodes { get { onS_C_ScheduleDefine_NodesGetting(); return _S_C_ScheduleDefine_Nodes;} }
		private ICollection<S_C_ScheduleDefine_Nodes> _S_C_ScheduleDefine_Nodes;
		partial void onS_C_ScheduleDefine_NodesGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_ScheduleDefine_S_C_Mode

        public S_C_ScheduleDefine()
        {
            _S_C_ScheduleDefine_Nodes = new List<S_C_ScheduleDefine_Nodes>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_ScheduleDefine_Nodes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DefineID { get; set; } // DefineID
		/// <summary></summary>	
		[Description("")]
        public string StructInfoID { get; set; } // StructInfoID
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
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string TaskType { get; set; } // TaskType
		/// <summary></summary>	
		[Description("")]
        public string IsDynmanic { get; set; } // IsDynmanic
		/// <summary></summary>	
		[Description("")]
        public string IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public string IsLocked { get; set; } // IsLocked
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary></summary>	
		[Description("")]
        public string Visible { get; set; } // Visible
		/// <summary></summary>	
		[Description("")]
        public string CanAdd { get; set; } // CanAdd
		/// <summary></summary>	
		[Description("")]
        public string CanEdit { get; set; } // CanEdit
		/// <summary></summary>	
		[Description("")]
        public string CanDelete { get; set; } // CanDelete
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string TabData { get; set; } // TabData
		/// <summary></summary>	
		[Description("")]
        public string RelateStructNodeID { get; set; } // RelateStructNodeID
		/// <summary></summary>	
		[Description("")]
        public string RelateStructNodeName { get; set; } // RelateStructNodeName
		/// <summary></summary>	
		[Description("")]
        public string RelateType { get; set; } // RelateType
		/// <summary></summary>	
		[Description("")]
        public string ExtendFieldConfig { get; set; } // ExtendFieldConfig

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_ScheduleDefine S_C_ScheduleDefine { get; set; } //  DefineID - FK_S_C_ScheduleDefine_Nodes_S_C_ScheduleDefine
    }

	/// <summary>WBS结构属性定义</summary>	
	[Description("WBS结构属性定义")]
    public partial class S_C_WBSStruct : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>模式ID</summary>	
		[Description("模式ID")]
        public string ModeID { get; set; } // ModeID
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string TaskType { get; set; } // TaskType
		/// <summary>父节点ID</summary>	
		[Description("父节点ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全路径ID</summary>	
		[Description("全路径ID")]
        public string FullID { get; set; } // FullID
		/// <summary>是否动态节点</summary>	
		[Description("是否动态节点")]
        public string IsDynmanic { get; set; } // IsDynmanic
		/// <summary></summary>	
		[Description("")]
        public string IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_C_Mode S_C_Mode { get; set; } //  ModeID - FK_S_C_WBSStruct_S_C_Mode
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_F_CapitalPlanTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
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
        public string SerialNumber { get; set; } // SerialNumber
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_F_CapitalPlanTemplate_Detail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
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
        public string CapitalPlanType { get; set; } // CapitalPlanType
		/// <summary></summary>	
		[Description("")]
        public string SourceSQL { get; set; } // SourceSQL
		/// <summary></summary>	
		[Description("")]
        public string RealSourceSQL { get; set; } // RealSourceSQL
		/// <summary></summary>	
		[Description("")]
        public string SourceLink { get; set; } // SourceLink
		/// <summary></summary>	
		[Description("")]
        public string RealSourceLink { get; set; } // RealSourceLink
		/// <summary></summary>	
		[Description("")]
        public bool? IsReadOnly { get; set; } // IsReadOnly
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string S_F_CapitalPlanTemplateID { get; set; } // S_F_CapitalPlanTemplateID
    }

	/// <summary>功能_员工管理_子集配置</summary>	
	[Description("功能_员工管理_子集配置")]
    public partial class S_HR_EmployeeBaseSet : Formula.BaseModel
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
		/// <summary></summary>	
		[Description("")]
        public string FlowPhase { get; set; } // FlowPhase
		/// <summary></summary>	
		[Description("")]
        public string FlowInfo { get; set; } // FlowInfo
		/// <summary></summary>	
		[Description("")]
        public string StepName { get; set; } // StepName
		/// <summary>用工形式</summary>	
		[Description("用工形式")]
        public string EmploymentWay { get; set; } // EmploymentWay
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Title { get; set; } // Title
		/// <summary>标识</summary>	
		[Description("标识")]
        public string Code { get; set; } // Code
		/// <summary>页面URl</summary>	
		[Description("页面URl")]
        public string Url { get; set; } // Url
		/// <summary>过滤字段</summary>	
		[Description("过滤字段")]
        public string FilterField { get; set; } // FilterField
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public int? SortIndex { get; set; } // SortIndex
    }

	/// <summary>采购合同付款项模板</summary>	
	[Description("采购合同付款项模板")]
    public partial class S_P_ProcurementContractPaymentObjTemplate : Formula.BaseModel
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
		/// <summary></summary>	
		[Description("")]
        public string FlowPhase { get; set; } // FlowPhase
		/// <summary></summary>	
		[Description("")]
        public string FlowInfo { get; set; } // FlowInfo
		/// <summary></summary>	
		[Description("")]
        public string StepName { get; set; } // StepName
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string PayType { get; set; } // PayType
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Remark { get; set; } // Remark
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_BomDefine : Formula.BaseModel
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
        public string BelongMode { get; set; } // BelongMode
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_BomDefine_Detail> S_T_BomDefine_Detail { get { onS_T_BomDefine_DetailGetting(); return _S_T_BomDefine_Detail;} }
		private ICollection<S_T_BomDefine_Detail> _S_T_BomDefine_Detail;
		partial void onS_T_BomDefine_DetailGetting();


        public S_T_BomDefine()
        {
            _S_T_BomDefine_Detail = new List<S_T_BomDefine_Detail>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_BomDefine_Detail : Formula.BaseModel
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
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string ConfigData { get; set; } // ConfigData
		/// <summary></summary>	
		[Description("")]
        public string LinkUrl { get; set; } // LinkUrl

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_BomDefine S_T_BomDefine { get; set; } //  DefineID - FK_S_T_BomDefine_Detail_S_T_BomDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_BOQTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string BOQType { get; set; } // BOQType
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>技术特征</summary>	
		[Description("技术特征")]
        public string TechnicalCharacter { get; set; } // TechnicalCharacter
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_CBSDefine : Formula.BaseModel
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
        public string BelongMode { get; set; } // BelongMode
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_CBSNodeTemplate> S_T_CBSNodeTemplate { get { onS_T_CBSNodeTemplateGetting(); return _S_T_CBSNodeTemplate;} }
		private ICollection<S_T_CBSNodeTemplate> _S_T_CBSNodeTemplate;
		partial void onS_T_CBSNodeTemplateGetting();


        public S_T_CBSDefine()
        {
            _S_T_CBSNodeTemplate = new List<S_T_CBSNodeTemplate>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_CBSNodeTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CBSDefineID { get; set; } // CBSDefineID
		/// <summary></summary>	
		[Description("")]
        public string CBSName { get; set; } // CBSName
		/// <summary></summary>	
		[Description("")]
        public string CBSCode { get; set; } // CBSCode
		/// <summary></summary>	
		[Description("")]
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string DefineType { get; set; } // DefineType
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
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string FinanceCode { get; set; } // FinanceCode
		/// <summary></summary>	
		[Description("")]
        public string CanDelete { get; set; } // CanDelete
		/// <summary></summary>	
		[Description("")]
        public string CanEdit { get; set; } // CanEdit
		/// <summary></summary>	
		[Description("")]
        public string CanAdd { get; set; } // CanAdd
		/// <summary></summary>	
		[Description("")]
        public string BidEdit { get; set; } // BidEdit
		/// <summary></summary>	
		[Description("")]
        public string BidAdd { get; set; } // BidAdd
		/// <summary></summary>	
		[Description("")]
        public string BidDelete { get; set; } // BidDelete
		/// <summary></summary>	
		[Description("")]
        public string CalExpression { get; set; } // CalExpression

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_CBSDefine S_T_CBSDefine { get; set; } //  CBSDefineID - FK_S_T_CBSNodeTemplate_S_T_CBSDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ConstructionQuantity : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
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
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
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

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_ConstructionQuantityDetail> S_T_ConstructionQuantityDetail { get { onS_T_ConstructionQuantityDetailGetting(); return _S_T_ConstructionQuantityDetail;} }
		private ICollection<S_T_ConstructionQuantityDetail> _S_T_ConstructionQuantityDetail;
		partial void onS_T_ConstructionQuantityDetailGetting();


        public S_T_ConstructionQuantity()
        {
            _S_T_ConstructionQuantityDetail = new List<S_T_ConstructionQuantityDetail>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_ConstructionQuantityDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string QuantityID { get; set; } // QuantityID
		/// <summary></summary>	
		[Description("")]
        public string QuantityFullID { get; set; } // QuantityFullID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Unit { get; set; } // Unit
		/// <summary></summary>	
		[Description("")]
        public string Property { get; set; } // Property
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
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

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ConstructionQuantity S_T_ConstructionQuantity { get; set; } //  QuantityID - FK_S_T_ConstructionQuantityDetail_S_T_ConstructionQuantity
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_DefineParams : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Value { get; set; } // Value
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_DesignInputDefine : Formula.BaseModel
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
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Class { get; set; } // Class
		/// <summary></summary>	
		[Description("")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary></summary>	
		[Description("")]
        public string PhaseValues { get; set; } // PhaseValues
		/// <summary></summary>	
		[Description("")]
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
    public partial class S_T_EquipmentMaterialCategory : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
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
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
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

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_EquipmentMaterialTemplate> S_T_EquipmentMaterialTemplate { get { onS_T_EquipmentMaterialTemplateGetting(); return _S_T_EquipmentMaterialTemplate;} }
		private ICollection<S_T_EquipmentMaterialTemplate> _S_T_EquipmentMaterialTemplate;
		partial void onS_T_EquipmentMaterialTemplateGetting();


        public S_T_EquipmentMaterialCategory()
        {
            _S_T_EquipmentMaterialTemplate = new List<S_T_EquipmentMaterialTemplate>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_EquipmentMaterialPriceTemplate : Formula.BaseModel
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
		/// <summary>规格</summary>	
		[Description("规格")]
        public string Size { get; set; } // Size
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>材质</summary>	
		[Description("材质")]
        public string Material { get; set; } // Material
		/// <summary>连接方式</summary>	
		[Description("连接方式")]
        public string ConnectionMode { get; set; } // ConnectionMode
		/// <summary>品牌</summary>	
		[Description("品牌")]
        public string Brand { get; set; } // Brand
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>数量</summary>	
		[Description("数量")]
        public decimal? Quantity { get; set; } // Quantity
		/// <summary>单价</summary>	
		[Description("单价")]
        public decimal? Price { get; set; } // Price
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string ProjectID { get; set; } // ProjectID
		/// <summary></summary>	
		[Description("")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary></summary>	
		[Description("")]
        public string ProjectCode { get; set; } // ProjectCode
		/// <summary></summary>	
		[Description("")]
        public string ContractID { get; set; } // ContractID
		/// <summary></summary>	
		[Description("")]
        public string ContractName { get; set; } // ContractName
		/// <summary></summary>	
		[Description("")]
        public string ContractCode { get; set; } // ContractCode
		/// <summary></summary>	
		[Description("")]
        public string SupplerID { get; set; } // SupplerID
		/// <summary></summary>	
		[Description("")]
        public string SupplierName { get; set; } // SupplierName
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
        public double? SortIndex { get; set; } // SortIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_EquipmentMaterialTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string CategoryFullID { get; set; } // CategoryFullID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary>规格</summary>	
		[Description("规格")]
        public string Size { get; set; } // Size
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>材质</summary>	
		[Description("材质")]
        public string Material { get; set; } // Material
		/// <summary>连接方式</summary>	
		[Description("连接方式")]
        public string ConnectionMode { get; set; } // ConnectionMode
		/// <summary>品牌</summary>	
		[Description("品牌")]
        public string Brand { get; set; } // Brand
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
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
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_EquipmentMaterialCategory S_T_EquipmentMaterialCategory { get; set; } //  CategoryID - FK_S_T_EquipmentMaterialPriceTemplate_S_T_EquipmentMaterialCategory
    }

	/// <summary>财务科目维护</summary>	
	[Description("财务科目维护")]
    public partial class S_T_FinanceSubject : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>科目名称</summary>	
		[Description("科目名称")]
        public string Name { get; set; } // Name
		/// <summary>科目编号</summary>	
		[Description("科目编号")]
        public string Code { get; set; } // Code
		/// <summary>科目分类</summary>	
		[Description("科目分类")]
        public string SubjectType { get; set; } // SubjectType
		/// <summary>RelateCBSCode</summary>	
		[Description("RelateCBSCode")]
        public string RelateCBSCode { get; set; } // RelateCBSCode
		/// <summary>排序</summary>	
		[Description("排序")]
        public double? SortIndex { get; set; } // SortIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_FolderAuth : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string AuthRelateType { get; set; } // AuthRelateType
		/// <summary></summary>	
		[Description("")]
        public string RelateID { get; set; } // RelateID
		/// <summary></summary>	
		[Description("")]
        public string RelateName { get; set; } // RelateName
		/// <summary></summary>	
		[Description("")]
        public string RelateCode { get; set; } // RelateCode
		/// <summary></summary>	
		[Description("")]
        public string WriteAuth { get; set; } // WriteAuth
		/// <summary></summary>	
		[Description("")]
        public string DownLoadAuth { get; set; } // DownLoadAuth
		/// <summary></summary>	
		[Description("")]
        public string BrowseAuth { get; set; } // BrowseAuth
		/// <summary></summary>	
		[Description("")]
        public string FolderDefID { get; set; } // FolderDefID

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_FolderDef S_T_FolderDef { get; set; } //  FolderDefID - FK_S_T_FolderAuth_S_T_FolderDef
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_FolderDef : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>目录名称</summary>	
		[Description("目录名称")]
        public string Name { get; set; } // Name
		/// <summary>目录编码</summary>	
		[Description("目录编码")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string FolderKey { get; set; } // FolderKey
		/// <summary></summary>	
		[Description("")]
        public string ParentID { get; set; } // ParentID
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary>目录类别（管理表单，成果，附件，普通)</summary>	
		[Description("目录类别（管理表单，成果，附件，普通)")]
        public string FolderType { get; set; } // FolderType
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string AttrDefine { get; set; } // AttrDefine
		/// <summary></summary>	
		[Description("")]
        public string InhertAuth { get; set; } // InhertAuth
		/// <summary></summary>	
		[Description("")]
        public string TempateID { get; set; } // TempateID
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary>链接数据库</summary>	
		[Description("链接数据库")]
        public string MappingDefine { get; set; } // MappingDefine
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_FolderAuth> S_T_FolderAuth { get { onS_T_FolderAuthGetting(); return _S_T_FolderAuth;} }
		private ICollection<S_T_FolderAuth> _S_T_FolderAuth;
		partial void onS_T_FolderAuthGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_T_FolderTemplate S_T_FolderTemplate { get; set; } //  TempateID - FK_S_T_FolderDef_S_T_FolderTemplate

        public S_T_FolderDef()
        {
            _S_T_FolderAuth = new List<S_T_FolderAuth>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_FolderTemplate : Formula.BaseModel
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
        public string ModeKey { get; set; } // ModeKey
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string DisplayColJson { get; set; } // DisplayColJson

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_FolderDef> S_T_FolderDef { get { onS_T_FolderDefGetting(); return _S_T_FolderDef;} }
		private ICollection<S_T_FolderDef> _S_T_FolderDef;
		partial void onS_T_FolderDefGetting();


        public S_T_FolderTemplate()
        {
            _S_T_FolderDef = new List<S_T_FolderDef>();
        }
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
		/// <summary>缩放级别最小值</summary>	
		[Description("缩放级别最小值")]
        public int? MinZoomVal { get; set; } // MinZoomVal
		/// <summary>缩放级别最大值</summary>	
		[Description("缩放级别最大值")]
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
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Catagory { get; set; } // Catagory
		/// <summary></summary>	
		[Description("")]
        public string PhaseInfo { get; set; } // PhaseInfo
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
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_NodeTemplate : Formula.BaseModel
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
        public string Scale { get; set; } // Scale
		/// <summary></summary>	
		[Description("")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary></summary>	
		[Description("")]
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_NodeTemplate_Detail> S_T_NodeTemplate_Detail { get { onS_T_NodeTemplate_DetailGetting(); return _S_T_NodeTemplate_Detail;} }
		private ICollection<S_T_NodeTemplate_Detail> _S_T_NodeTemplate_Detail;
		partial void onS_T_NodeTemplate_DetailGetting();


        public S_T_NodeTemplate()
        {
            _S_T_NodeTemplate_Detail = new List<S_T_NodeTemplate_Detail>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_NodeTemplate_Detail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
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
        public string NodeType { get; set; } // NodeType
		/// <summary></summary>	
		[Description("")]
        public string NodeTemplateID { get; set; } // NodeTemplateID
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_NodeTemplate S_T_NodeTemplate { get; set; } //  NodeTemplateID - FK_S_T_NodeTemplate_Detail_S_T_NodeTemplate
    }

	/// <summary>角色定义</summary>	
	[Description("角色定义")]
    public partial class S_T_RoleDefine : Formula.BaseModel
    {
		/// <summary>主键ID</summary>	
		[Description("主键ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>角色名称</summary>	
		[Description("角色名称")]
        public string RoleName { get; set; } // RoleName
		/// <summary>角色编码</summary>	
		[Description("角色编码")]
        public string RoleCode { get; set; } // RoleCode
		/// <summary></summary>	
		[Description("")]
        public string RoleType { get; set; } // RoleType
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_TaskTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ScheduleCode { get; set; } // ScheduleCode
		/// <summary></summary>	
		[Description("")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary></summary>	
		[Description("")]
        public string ProjectScale { get; set; } // ProjectScale
		/// <summary></summary>	
		[Description("")]
        public string ProjectType { get; set; } // ProjectType
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
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

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_TaskTemplate_Detail> S_T_TaskTemplate_Detail { get { onS_T_TaskTemplate_DetailGetting(); return _S_T_TaskTemplate_Detail;} }
		private ICollection<S_T_TaskTemplate_Detail> _S_T_TaskTemplate_Detail;
		partial void onS_T_TaskTemplate_DetailGetting();


        public S_T_TaskTemplate()
        {
            _S_T_TaskTemplate_Detail = new List<S_T_TaskTemplate_Detail>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_TaskTemplate_Detail : Formula.BaseModel
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
        public string TaskType { get; set; } // TaskType
		/// <summary></summary>	
		[Description("")]
        public string MajorCode { get; set; } // MajorCode
		/// <summary></summary>	
		[Description("")]
        public string MajorName { get; set; } // MajorName
		/// <summary></summary>	
		[Description("")]
        public string SubProjectName { get; set; } // SubProjectName
		/// <summary></summary>	
		[Description("")]
        public string MoudleName { get; set; } // MoudleName
		/// <summary></summary>	
		[Description("")]
        public string PhaseName { get; set; } // PhaseName
		/// <summary></summary>	
		[Description("")]
        public string CategoryName { get; set; } // CategoryName
		/// <summary></summary>	
		[Description("")]
        public string EntityName { get; set; } // EntityName
		/// <summary></summary>	
		[Description("")]
        public string DesignName { get; set; } // DesignName
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_TaskTemplate S_T_TaskTemplate { get; set; } //  TemplateID - FK_S_T_TaskTemplate_Detail_S_T_TaskTemplate
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

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_ToDoListDefineNode> S_T_ToDoListDefineNode { get { onS_T_ToDoListDefineNodeGetting(); return _S_T_ToDoListDefineNode;} }
		private ICollection<S_T_ToDoListDefineNode> _S_T_ToDoListDefineNode;
		partial void onS_T_ToDoListDefineNodeGetting();


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

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_ToDoListDefine S_T_ToDoListDefine { get; set; } //  DefineID - FK_S_T_ToDoListDefineNode_S_T_ToDoListDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_WBSAttrDefine : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TypeDefineID { get; set; } // TypeDefineID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
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
        public string RelateDeptInfo { get; set; } // RelateDeptInfo
		/// <summary></summary>	
		[Description("")]
        public string RelateDeptInfoName { get; set; } // RelateDeptInfoName

        // Foreign keys
		[JsonIgnore]
        public virtual S_T_WBSTypeDefine S_T_WBSTypeDefine { get; set; } //  TypeDefineID - FK_S_T_WBSAttrDefine_S_T_WBSTypeDefine
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_T_WBSTypeDefine : Formula.BaseModel
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
        public string Class { get; set; } // Class
		/// <summary></summary>	
		[Description("")]
        public bool IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_T_WBSAttrDefine> S_T_WBSAttrDefine { get { onS_T_WBSAttrDefineGetting(); return _S_T_WBSAttrDefine;} }
		private ICollection<S_T_WBSAttrDefine> _S_T_WBSAttrDefine;
		partial void onS_T_WBSAttrDefineGetting();


        public S_T_WBSTypeDefine()
        {
            _S_T_WBSAttrDefine = new List<S_T_WBSAttrDefine>();
        }
    }


    // ************************************************************************
    // POCO Configuration

    // S_C_MenuAuth
    internal partial class S_C_MenuAuthConfiguration : EntityTypeConfiguration<S_C_MenuAuth>
    {
        public S_C_MenuAuthConfiguration()
        {
			ToTable("S_C_MENUAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.MeunID).HasColumnName("MEUNID").IsRequired().HasMaxLength(50);
            Property(x => x.AuthType).HasColumnName("AUTHTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleKey).HasColumnName("ROLEKEY").IsRequired().HasMaxLength(200);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_C_Meun).WithMany(b => b.S_C_MenuAuth).HasForeignKey(c => c.MeunID); // FK_S_C_MenuAuth_S_C_Meun
        }
    }

    // S_C_Meun
    internal partial class S_C_MeunConfiguration : EntityTypeConfiguration<S_C_Meun>
    {
        public S_C_MeunConfiguration()
        {
			ToTable("S_C_MEUN");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.MeunType).HasColumnName("MEUNTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.MeunDefineType).HasColumnName("MEUNDEFINETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.NavWBSType).HasColumnName("NAVWBSTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.DataSourceType).HasColumnName("DATASOURCETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.DataSourceSQL).HasColumnName("DATASOURCESQL").IsOptional();
            Property(x => x.Condition).HasColumnName("CONDITION").IsOptional();
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.NameEN).HasColumnName("NAMEEN").IsOptional().HasMaxLength(500);
            Property(x => x.ConditionSQL).HasColumnName("CONDITIONSQL").IsOptional();
            Property(x => x.Expanded).HasColumnName("EXPANDED").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_Meun).HasForeignKey(c => c.ModeID); // FK_S_C_Meun_S_C_Mode
        }
    }

    // S_C_MileStoneTemplate
    internal partial class S_C_MileStoneTemplateConfiguration : EntityTypeConfiguration<S_C_MileStoneTemplate>
    {
        public S_C_MileStoneTemplateConfiguration()
        {
			ToTable("S_C_MILESTONETEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Weight).HasColumnName("WEIGHT").IsOptional().HasPrecision(18,4);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_MileStoneTemplate).HasForeignKey(c => c.ModeID); // FK_S_C_MileStoneTemplate_S_C_Mode
        }
    }

    // S_C_Mode
    internal partial class S_C_ModeConfiguration : EntityTypeConfiguration<S_C_Mode>
    {
        public S_C_ModeConfiguration()
        {
			ToTable("S_C_MODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(200);
            Property(x => x.Condition).HasColumnName("CONDITION").IsOptional();
            Property(x => x.Priority).HasColumnName("PRIORITY").IsOptional();
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.ToDoListDefine).HasColumnName("TODOLISTDEFINE").IsOptional().HasMaxLength(50);
            Property(x => x.ToDoListDefineName).HasColumnName("TODOLISTDEFINENAME").IsOptional().HasMaxLength(500);
        }
    }

    // S_C_OBSTemplate
    internal partial class S_C_OBSTemplateConfiguration : EntityTypeConfiguration<S_C_OBSTemplate>
    {
        public S_C_OBSTemplateConfiguration()
        {
			ToTable("S_C_OBSTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.OBSType).HasColumnName("OBSTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_OBSTemplate).HasForeignKey(c => c.ModeID); // FK_S_C_OBSTemplate_S_C_Mode
        }
    }

    // S_C_PBSStruct
    internal partial class S_C_PBSStructConfiguration : EntityTypeConfiguration<S_C_PBSStruct>
    {
        public S_C_PBSStructConfiguration()
        {
			ToTable("S_C_PBSSTRUCT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsOptional();
            Property(x => x.CanSynToWBS).HasColumnName("CANSYNTOWBS").IsOptional();
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_C_Mode).WithMany(b => b.S_C_PBSStruct).HasForeignKey(c => c.ModeID); // FK_S_C_PBSStruct_S_C_Mode
        }
    }

    // S_C_QBSStruct
    internal partial class S_C_QBSStructConfiguration : EntityTypeConfiguration<S_C_QBSStruct>
    {
        public S_C_QBSStructConfiguration()
        {
			ToTable("S_C_QBSSTRUCT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsOptional();
            Property(x => x.CanImportWBS).HasColumnName("CANIMPORTWBS").IsRequired();
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_QBSStruct).HasForeignKey(c => c.ModeID); // FK_S_C_QBSStruct_S_C_Mode
        }
    }

    // S_C_ScheduleDefine
    internal partial class S_C_ScheduleDefineConfiguration : EntityTypeConfiguration<S_C_ScheduleDefine>
    {
        public S_C_ScheduleDefineConfiguration()
        {
			ToTable("S_C_SCHEDULEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.PreScheduleCode).HasColumnName("PRESCHEDULECODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.ImportProject).HasColumnName("IMPORTPROJECT").IsRequired();
            Property(x => x.ImportTaskTemplate).HasColumnName("IMPORTTASKTEMPLATE").IsRequired();
            Property(x => x.ImportBOM).HasColumnName("IMPORTBOM").IsRequired();
            Property(x => x.ImportQBS).HasColumnName("IMPORTQBS").IsRequired();
            Property(x => x.ImportBid).HasColumnName("IMPORTBID").IsRequired();
            Property(x => x.ImportExcel).HasColumnName("IMPORTEXCEL").IsRequired();
            Property(x => x.TabData).HasColumnName("TABDATA").IsOptional();
            Property(x => x.AttrDefine).HasColumnName("ATTRDEFINE").IsOptional();
            Property(x => x.ShowTab).HasColumnName("SHOWTAB").IsRequired();
            Property(x => x.ColDefine).HasColumnName("COLDEFINE").IsOptional();
            Property(x => x.ProductionValueSchedule).HasColumnName("PRODUCTIONVALUESCHEDULE").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_ScheduleDefine).HasForeignKey(c => c.ModeID); // FK_S_C_ScheduleDefine_S_C_Mode
        }
    }

    // S_C_ScheduleDefine_Nodes
    internal partial class S_C_ScheduleDefine_NodesConfiguration : EntityTypeConfiguration<S_C_ScheduleDefine_Nodes>
    {
        public S_C_ScheduleDefine_NodesConfiguration()
        {
			ToTable("S_C_SCHEDULEDEFINE_NODES");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefineID).HasColumnName("DEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.StructInfoID).HasColumnName("STRUCTINFOID").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.TaskType).HasColumnName("TASKTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.IsDynmanic).HasColumnName("ISDYNMANIC").IsOptional().HasMaxLength(50);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsOptional().HasMaxLength(50);
            Property(x => x.IsLocked).HasColumnName("ISLOCKED").IsOptional().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.Visible).HasColumnName("VISIBLE").IsOptional().HasMaxLength(50);
            Property(x => x.CanAdd).HasColumnName("CANADD").IsOptional().HasMaxLength(50);
            Property(x => x.CanEdit).HasColumnName("CANEDIT").IsOptional().HasMaxLength(50);
            Property(x => x.CanDelete).HasColumnName("CANDELETE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.TabData).HasColumnName("TABDATA").IsOptional();
            Property(x => x.RelateStructNodeID).HasColumnName("RELATESTRUCTNODEID").IsOptional().HasMaxLength(50);
            Property(x => x.RelateStructNodeName).HasColumnName("RELATESTRUCTNODENAME").IsOptional().HasMaxLength(200);
            Property(x => x.RelateType).HasColumnName("RELATETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ExtendFieldConfig).HasColumnName("EXTENDFIELDCONFIG").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_C_ScheduleDefine).WithMany(b => b.S_C_ScheduleDefine_Nodes).HasForeignKey(c => c.DefineID); // FK_S_C_ScheduleDefine_Nodes_S_C_ScheduleDefine
        }
    }

    // S_C_WBSStruct
    internal partial class S_C_WBSStructConfiguration : EntityTypeConfiguration<S_C_WBSStruct>
    {
        public S_C_WBSStructConfiguration()
        {
			ToTable("S_C_WBSSTRUCT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ModeID).HasColumnName("MODEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.TaskType).HasColumnName("TASKTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.IsDynmanic).HasColumnName("ISDYNMANIC").IsRequired().HasMaxLength(50);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsRequired().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_C_Mode).WithMany(b => b.S_C_WBSStruct).HasForeignKey(c => c.ModeID); // FK_S_C_WBSStruct_S_C_Mode
        }
    }

    // S_F_CapitalPlanTemplate
    internal partial class S_F_CapitalPlanTemplateConfiguration : EntityTypeConfiguration<S_F_CapitalPlanTemplate>
    {
        public S_F_CapitalPlanTemplateConfiguration()
        {
			ToTable("S_F_CAPITALPLANTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(100);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SerialNumber).HasColumnName("SERIALNUMBER").IsOptional().HasMaxLength(100);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_F_CapitalPlanTemplate_Detail
    internal partial class S_F_CapitalPlanTemplate_DetailConfiguration : EntityTypeConfiguration<S_F_CapitalPlanTemplate_Detail>
    {
        public S_F_CapitalPlanTemplate_DetailConfiguration()
        {
			ToTable("S_F_CAPITALPLANTEMPLATE_DETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional();
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.CapitalPlanType).HasColumnName("CAPITALPLANTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SourceSQL).HasColumnName("SOURCESQL").IsOptional();
            Property(x => x.RealSourceSQL).HasColumnName("REALSOURCESQL").IsOptional();
            Property(x => x.SourceLink).HasColumnName("SOURCELINK").IsOptional();
            Property(x => x.RealSourceLink).HasColumnName("REALSOURCELINK").IsOptional();
            Property(x => x.IsReadOnly).HasColumnName("ISREADONLY").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.S_F_CapitalPlanTemplateID).HasColumnName("S_F_CAPITALPLANTEMPLATEID").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_EmployeeBaseSet
    internal partial class S_HR_EmployeeBaseSetConfiguration : EntityTypeConfiguration<S_HR_EmployeeBaseSet>
    {
        public S_HR_EmployeeBaseSetConfiguration()
        {
			ToTable("S_HR_EMPLOYEEBASESET");
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
            Property(x => x.FlowPhase).HasColumnName("FLOWPHASE").IsOptional().HasMaxLength(50);
            Property(x => x.FlowInfo).HasColumnName("FLOWINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.StepName).HasColumnName("STEPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.EmploymentWay).HasColumnName("EMPLOYMENTWAY").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.FilterField).HasColumnName("FILTERFIELD").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_P_ProcurementContractPaymentObjTemplate
    internal partial class S_P_ProcurementContractPaymentObjTemplateConfiguration : EntityTypeConfiguration<S_P_ProcurementContractPaymentObjTemplate>
    {
        public S_P_ProcurementContractPaymentObjTemplateConfiguration()
        {
			ToTable("S_P_PROCUREMENTCONTRACTPAYMENTOBJTEMPLATE");
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
            Property(x => x.FlowPhase).HasColumnName("FLOWPHASE").IsOptional().HasMaxLength(50);
            Property(x => x.FlowInfo).HasColumnName("FLOWINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.StepName).HasColumnName("STEPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.PayType).HasColumnName("PAYTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_T_BomDefine
    internal partial class S_T_BomDefineConfiguration : EntityTypeConfiguration<S_T_BomDefine>
    {
        public S_T_BomDefineConfiguration()
        {
			ToTable("S_T_BOMDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.BelongMode).HasColumnName("BELONGMODE").IsRequired().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_BomDefine_Detail
    internal partial class S_T_BomDefine_DetailConfiguration : EntityTypeConfiguration<S_T_BomDefine_Detail>
    {
        public S_T_BomDefine_DetailConfiguration()
        {
			ToTable("S_T_BOMDEFINE_DETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefineID).HasColumnName("DEFINEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.ConfigData).HasColumnName("CONFIGDATA").IsOptional();
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.S_T_BomDefine).WithMany(b => b.S_T_BomDefine_Detail).HasForeignKey(c => c.DefineID); // FK_S_T_BomDefine_Detail_S_T_BomDefine
        }
    }

    // S_T_BOQTemplate
    internal partial class S_T_BOQTemplateConfiguration : EntityTypeConfiguration<S_T_BOQTemplate>
    {
        public S_T_BOQTemplateConfiguration()
        {
			ToTable("S_T_BOQTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.BOQType).HasColumnName("BOQTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(50);
            Property(x => x.TechnicalCharacter).HasColumnName("TECHNICALCHARACTER").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_T_CBSDefine
    internal partial class S_T_CBSDefineConfiguration : EntityTypeConfiguration<S_T_CBSDefine>
    {
        public S_T_CBSDefineConfiguration()
        {
			ToTable("S_T_CBSDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.BelongMode).HasColumnName("BELONGMODE").IsRequired().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_CBSNodeTemplate
    internal partial class S_T_CBSNodeTemplateConfiguration : EntityTypeConfiguration<S_T_CBSNodeTemplate>
    {
        public S_T_CBSNodeTemplateConfiguration()
        {
			ToTable("S_T_CBSNODETEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CBSDefineID).HasColumnName("CBSDEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.CBSName).HasColumnName("CBSNAME").IsRequired().HasMaxLength(500);
            Property(x => x.CBSCode).HasColumnName("CBSCODE").IsOptional().HasMaxLength(500);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.DefineType).HasColumnName("DEFINETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.CBSType).HasColumnName("CBSTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.FinanceCode).HasColumnName("FINANCECODE").IsOptional().HasMaxLength(500);
            Property(x => x.CanDelete).HasColumnName("CANDELETE").IsOptional().HasMaxLength(50);
            Property(x => x.CanEdit).HasColumnName("CANEDIT").IsOptional().HasMaxLength(50);
            Property(x => x.CanAdd).HasColumnName("CANADD").IsOptional().HasMaxLength(50);
            Property(x => x.BidEdit).HasColumnName("BIDEDIT").IsOptional().HasMaxLength(50);
            Property(x => x.BidAdd).HasColumnName("BIDADD").IsOptional().HasMaxLength(50);
            Property(x => x.BidDelete).HasColumnName("BIDDELETE").IsOptional().HasMaxLength(50);
            Property(x => x.CalExpression).HasColumnName("CALEXPRESSION").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_T_CBSDefine).WithMany(b => b.S_T_CBSNodeTemplate).HasForeignKey(c => c.CBSDefineID); // FK_S_T_CBSNodeTemplate_S_T_CBSDefine
        }
    }

    // S_T_ConstructionQuantity
    internal partial class S_T_ConstructionQuantityConfiguration : EntityTypeConfiguration<S_T_ConstructionQuantity>
    {
        public S_T_ConstructionQuantityConfiguration()
        {
			ToTable("S_T_CONSTRUCTIONQUANTITY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_ConstructionQuantityDetail
    internal partial class S_T_ConstructionQuantityDetailConfiguration : EntityTypeConfiguration<S_T_ConstructionQuantityDetail>
    {
        public S_T_ConstructionQuantityDetailConfiguration()
        {
			ToTable("S_T_CONSTRUCTIONQUANTITYDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.QuantityID).HasColumnName("QUANTITYID").IsOptional().HasMaxLength(50);
            Property(x => x.QuantityFullID).HasColumnName("QUANTITYFULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(50);
            Property(x => x.Property).HasColumnName("PROPERTY").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_T_ConstructionQuantity).WithMany(b => b.S_T_ConstructionQuantityDetail).HasForeignKey(c => c.QuantityID); // FK_S_T_ConstructionQuantityDetail_S_T_ConstructionQuantity
        }
    }

    // S_T_DefineParams
    internal partial class S_T_DefineParamsConfiguration : EntityTypeConfiguration<S_T_DefineParams>
    {
        public S_T_DefineParamsConfiguration()
        {
			ToTable("S_T_DEFINEPARAMS");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Value).HasColumnName("VALUE").IsRequired().HasMaxLength(500);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_DesignInputDefine
    internal partial class S_T_DesignInputDefineConfiguration : EntityTypeConfiguration<S_T_DesignInputDefine>
    {
        public S_T_DesignInputDefineConfiguration()
        {
			ToTable("S_T_DESIGNINPUTDEFINE");
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

    // S_T_EquipmentMaterialCategory
    internal partial class S_T_EquipmentMaterialCategoryConfiguration : EntityTypeConfiguration<S_T_EquipmentMaterialCategory>
    {
        public S_T_EquipmentMaterialCategoryConfiguration()
        {
			ToTable("S_T_EQUIPMENTMATERIALCATEGORY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_EquipmentMaterialPriceTemplate
    internal partial class S_T_EquipmentMaterialPriceTemplateConfiguration : EntityTypeConfiguration<S_T_EquipmentMaterialPriceTemplate>
    {
        public S_T_EquipmentMaterialPriceTemplateConfiguration()
        {
			ToTable("S_T_EQUIPMENTMATERIALPRICETEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Size).HasColumnName("SIZE").IsOptional().HasMaxLength(200);
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Material).HasColumnName("MATERIAL").IsOptional().HasMaxLength(200);
            Property(x => x.ConnectionMode).HasColumnName("CONNECTIONMODE").IsOptional().HasMaxLength(200);
            Property(x => x.Brand).HasColumnName("BRAND").IsOptional().HasMaxLength(200);
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Quantity).HasColumnName("QUANTITY").IsOptional().HasPrecision(18,4);
            Property(x => x.Price).HasColumnName("PRICE").IsOptional().HasPrecision(18,4);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectID).HasColumnName("PROJECTID").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectCode).HasColumnName("PROJECTCODE").IsOptional().HasMaxLength(200);
            Property(x => x.ContractID).HasColumnName("CONTRACTID").IsOptional().HasMaxLength(50);
            Property(x => x.ContractName).HasColumnName("CONTRACTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ContractCode).HasColumnName("CONTRACTCODE").IsOptional().HasMaxLength(200);
            Property(x => x.SupplerID).HasColumnName("SUPPLERID").IsOptional().HasMaxLength(50);
            Property(x => x.SupplierName).HasColumnName("SUPPLIERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_T_EquipmentMaterialTemplate
    internal partial class S_T_EquipmentMaterialTemplateConfiguration : EntityTypeConfiguration<S_T_EquipmentMaterialTemplate>
    {
        public S_T_EquipmentMaterialTemplateConfiguration()
        {
			ToTable("S_T_EQUIPMENTMATERIALTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.CategoryFullID).HasColumnName("CATEGORYFULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Size).HasColumnName("SIZE").IsOptional().HasMaxLength(200);
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Material).HasColumnName("MATERIAL").IsOptional().HasMaxLength(200);
            Property(x => x.ConnectionMode).HasColumnName("CONNECTIONMODE").IsOptional().HasMaxLength(200);
            Property(x => x.Brand).HasColumnName("BRAND").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_T_EquipmentMaterialCategory).WithMany(b => b.S_T_EquipmentMaterialTemplate).HasForeignKey(c => c.CategoryID); // FK_S_T_EquipmentMaterialPriceTemplate_S_T_EquipmentMaterialCategory
        }
    }

    // S_T_FinanceSubject
    internal partial class S_T_FinanceSubjectConfiguration : EntityTypeConfiguration<S_T_FinanceSubject>
    {
        public S_T_FinanceSubjectConfiguration()
        {
			ToTable("S_T_FINANCESUBJECT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.SubjectType).HasColumnName("SUBJECTTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.RelateCBSCode).HasColumnName("RELATECBSCODE").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_T_FolderAuth
    internal partial class S_T_FolderAuthConfiguration : EntityTypeConfiguration<S_T_FolderAuth>
    {
        public S_T_FolderAuthConfiguration()
        {
			ToTable("S_T_FOLDERAUTH");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.AuthRelateType).HasColumnName("AUTHRELATETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.RelateID).HasColumnName("RELATEID").IsRequired().HasMaxLength(50);
            Property(x => x.RelateName).HasColumnName("RELATENAME").IsRequired().HasMaxLength(50);
            Property(x => x.RelateCode).HasColumnName("RELATECODE").IsOptional().HasMaxLength(50);
            Property(x => x.WriteAuth).HasColumnName("WRITEAUTH").IsRequired().HasMaxLength(50);
            Property(x => x.DownLoadAuth).HasColumnName("DOWNLOADAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.BrowseAuth).HasColumnName("BROWSEAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.FolderDefID).HasColumnName("FOLDERDEFID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_T_FolderDef).WithMany(b => b.S_T_FolderAuth).HasForeignKey(c => c.FolderDefID); // FK_S_T_FolderAuth_S_T_FolderDef
        }
    }

    // S_T_FolderDef
    internal partial class S_T_FolderDefConfiguration : EntityTypeConfiguration<S_T_FolderDef>
    {
        public S_T_FolderDefConfiguration()
        {
			ToTable("S_T_FOLDERDEF");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.FolderKey).HasColumnName("FOLDERKEY").IsOptional().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(500);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.FolderType).HasColumnName("FOLDERTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.AttrDefine).HasColumnName("ATTRDEFINE").IsOptional();
            Property(x => x.InhertAuth).HasColumnName("INHERTAUTH").IsOptional().HasMaxLength(50);
            Property(x => x.TempateID).HasColumnName("TEMPATEID").IsRequired().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.MappingDefine).HasColumnName("MAPPINGDEFINE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_T_FolderTemplate).WithMany(b => b.S_T_FolderDef).HasForeignKey(c => c.TempateID); // FK_S_T_FolderDef_S_T_FolderTemplate
        }
    }

    // S_T_FolderTemplate
    internal partial class S_T_FolderTemplateConfiguration : EntityTypeConfiguration<S_T_FolderTemplate>
    {
        public S_T_FolderTemplateConfiguration()
        {
			ToTable("S_T_FOLDERTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.ModeKey).HasColumnName("MODEKEY").IsOptional().HasMaxLength(500);
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.DisplayColJson).HasColumnName("DISPLAYCOLJSON").IsOptional();
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
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.Catagory).HasColumnName("CATAGORY").IsRequired().HasMaxLength(50);
            Property(x => x.PhaseInfo).HasColumnName("PHASEINFO").IsOptional().HasMaxLength(50);
            Property(x => x.LinkFormUrl).HasColumnName("LINKFORMURL").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.NameFieldInfo).HasColumnName("NAMEFIELDINFO").IsRequired().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.EnumFieldInfo).HasColumnName("ENUMFIELDINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CanAddNewForm).HasColumnName("CANADDNEWFORM").IsRequired().HasMaxLength(50);
            Property(x => x.SendNotice).HasColumnName("SENDNOTICE").IsOptional().HasMaxLength(50);
            Property(x => x.FormCode).HasColumnName("FORMCODE").IsOptional().HasMaxLength(500);
            Property(x => x.FlowCode).HasColumnName("FLOWCODE").IsOptional().HasMaxLength(500);
            Property(x => x.LinkViewUrl).HasColumnName("LINKVIEWURL").IsOptional().HasMaxLength(500);
            Property(x => x.StartNoticeContent).HasColumnName("STARTNOTICECONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.EndNoticeContent).HasColumnName("ENDNOTICECONTENT").IsOptional().HasMaxLength(500);
        }
    }

    // S_T_NodeTemplate
    internal partial class S_T_NodeTemplateConfiguration : EntityTypeConfiguration<S_T_NodeTemplate>
    {
        public S_T_NodeTemplateConfiguration()
        {
			ToTable("S_T_NODETEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Scale).HasColumnName("SCALE").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsRequired().HasMaxLength(50);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_T_NodeTemplate_Detail
    internal partial class S_T_NodeTemplate_DetailConfiguration : EntityTypeConfiguration<S_T_NodeTemplate_Detail>
    {
        public S_T_NodeTemplate_DetailConfiguration()
        {
			ToTable("S_T_NODETEMPLATE_DETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.NodeType).HasColumnName("NODETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.NodeTemplateID).HasColumnName("NODETEMPLATEID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();

            // Foreign keys
            HasOptional(a => a.S_T_NodeTemplate).WithMany(b => b.S_T_NodeTemplate_Detail).HasForeignKey(c => c.NodeTemplateID); // FK_S_T_NodeTemplate_Detail_S_T_NodeTemplate
        }
    }

    // S_T_RoleDefine
    internal partial class S_T_RoleDefineConfiguration : EntityTypeConfiguration<S_T_RoleDefine>
    {
        public S_T_RoleDefineConfiguration()
        {
			ToTable("S_T_ROLEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleName).HasColumnName("ROLENAME").IsRequired().HasMaxLength(50);
            Property(x => x.RoleCode).HasColumnName("ROLECODE").IsRequired().HasMaxLength(50);
            Property(x => x.RoleType).HasColumnName("ROLETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_T_TaskTemplate
    internal partial class S_T_TaskTemplateConfiguration : EntityTypeConfiguration<S_T_TaskTemplate>
    {
        public S_T_TaskTemplateConfiguration()
        {
			ToTable("S_T_TASKTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.ScheduleCode).HasColumnName("SCHEDULECODE").IsRequired().HasMaxLength(500);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectScale).HasColumnName("PROJECTSCALE").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectType).HasColumnName("PROJECTTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
        }
    }

    // S_T_TaskTemplate_Detail
    internal partial class S_T_TaskTemplate_DetailConfiguration : EntityTypeConfiguration<S_T_TaskTemplate_Detail>
    {
        public S_T_TaskTemplate_DetailConfiguration()
        {
			ToTable("S_T_TASKTEMPLATE_DETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TemplateID).HasColumnName("TEMPLATEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.TaskType).HasColumnName("TASKTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.MajorCode).HasColumnName("MAJORCODE").IsOptional().HasMaxLength(50);
            Property(x => x.MajorName).HasColumnName("MAJORNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SubProjectName).HasColumnName("SUBPROJECTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.MoudleName).HasColumnName("MOUDLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.PhaseName).HasColumnName("PHASENAME").IsOptional().HasMaxLength(500);
            Property(x => x.CategoryName).HasColumnName("CATEGORYNAME").IsOptional().HasMaxLength(500);
            Property(x => x.EntityName).HasColumnName("ENTITYNAME").IsOptional().HasMaxLength(500);
            Property(x => x.DesignName).HasColumnName("DESIGNNAME").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_T_TaskTemplate).WithMany(b => b.S_T_TaskTemplate_Detail).HasForeignKey(c => c.TemplateID); // FK_S_T_TaskTemplate_Detail_S_T_TaskTemplate
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

            // Foreign keys
            HasRequired(a => a.S_T_ToDoListDefine).WithMany(b => b.S_T_ToDoListDefineNode).HasForeignKey(c => c.DefineID); // FK_S_T_ToDoListDefineNode_S_T_ToDoListDefine
        }
    }

    // S_T_WBSAttrDefine
    internal partial class S_T_WBSAttrDefineConfiguration : EntityTypeConfiguration<S_T_WBSAttrDefine>
    {
        public S_T_WBSAttrDefineConfiguration()
        {
			ToTable("S_T_WBSATTRDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TypeDefineID).HasColumnName("TYPEDEFINEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.BelongMode).HasColumnName("BELONGMODE").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.RelateDeptInfo).HasColumnName("RELATEDEPTINFO").IsOptional().HasMaxLength(500);
            Property(x => x.RelateDeptInfoName).HasColumnName("RELATEDEPTINFONAME").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasRequired(a => a.S_T_WBSTypeDefine).WithMany(b => b.S_T_WBSAttrDefine).HasForeignKey(c => c.TypeDefineID); // FK_S_T_WBSAttrDefine_S_T_WBSTypeDefine
        }
    }

    // S_T_WBSTypeDefine
    internal partial class S_T_WBSTypeDefineConfiguration : EntityTypeConfiguration<S_T_WBSTypeDefine>
    {
        public S_T_WBSTypeDefineConfiguration()
        {
			ToTable("S_T_WBSTYPEDEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
            Property(x => x.Class).HasColumnName("CLASS").IsRequired().HasMaxLength(50);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsRequired();
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
        }
    }

}

