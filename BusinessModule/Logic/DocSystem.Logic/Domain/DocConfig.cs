

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "InfrasBaseConfig"
//     Connection String:      "Data Source=10.10.1.244\sql2008;Initial Catalog=SINOAE_DesignInfrastructure;User ID=sa;PWD=123.zxc;"

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

namespace DocSystem.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class DocConfigEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_DOC_File> S_DOC_File { get; set; } // S_DOC_File
        public IDbSet<S_DOC_FileAttr> S_DOC_FileAttr { get; set; } // S_DOC_FileAttr
        public IDbSet<S_DOC_FileNodeRelation> S_DOC_FileNodeRelation { get; set; } // S_DOC_FileNodeRelation
        public IDbSet<S_DOC_FulltextSearchConvertLog> S_DOC_FulltextSearchConvertLog { get; set; } // S_DOC_FulltextSearchConvertLog
        public IDbSet<S_DOC_ListConfig> S_DOC_ListConfig { get; set; } // S_DOC_ListConfig
        public IDbSet<S_DOC_ListConfigDetail> S_DOC_ListConfigDetail { get; set; } // S_DOC_ListConfigDetail
        public IDbSet<S_DOC_Node> S_DOC_Node { get; set; } // S_DOC_Node
        public IDbSet<S_DOC_NodeAttr> S_DOC_NodeAttr { get; set; } // S_DOC_NodeAttr
        public IDbSet<S_DOC_NodeStrcut> S_DOC_NodeStrcut { get; set; } // S_DOC_NodeStrcut
        public IDbSet<S_DOC_QueryParam> S_DOC_QueryParam { get; set; } // S_DOC_QueryParam
        public IDbSet<S_DOC_ReorganizeConfig> S_DOC_ReorganizeConfig { get; set; } // S_DOC_ReorganizeConfig
        public IDbSet<S_DOC_Space> S_DOC_Space { get; set; } // S_DOC_Space
        public IDbSet<S_DOC_TagConfig> S_DOC_TagConfig { get; set; } // S_DOC_TagConfig
        public IDbSet<S_DOC_TreeConfig> S_DOC_TreeConfig { get; set; } // S_DOC_TreeConfig
        public IDbSet<S_DOC_UserSearhHistory> S_DOC_UserSearhHistory { get; set; } // S_DOC_UserSearhHistory

        static DocConfigEntities()
        {
            Database.SetInitializer<DocConfigEntities>(null);
        }

        public DocConfigEntities()
            : base("Name=InfrasBaseConfig")
        {
        }

        public DocConfigEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_DOC_FileConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_FileAttrConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_FileNodeRelationConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_FulltextSearchConvertLogConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_ListConfigConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_ListConfigDetailConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_NodeConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_NodeAttrConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_NodeStrcutConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_QueryParamConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_ReorganizeConfigConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_SpaceConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_TagConfigConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_TreeConfigConfiguration());
            modelBuilder.Configurations.Add(new S_DOC_UserSearhHistoryConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_File : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string CanBorrow { get; set; } // CanBorrow
		/// <summary></summary>	
		[Description("")]
        public string BorrowFlowKey { get; set; } // BorrowFlowKey
		/// <summary></summary>	
		[Description("")]
        public string CanDownload { get; set; } // CanDownload
		/// <summary></summary>	
		[Description("")]
        public string DownloadFlowKey { get; set; } // DownloadFlowKey
		/// <summary></summary>	
		[Description("")]
        public string AllowDisplay { get; set; } // AllowDisplay
		/// <summary></summary>	
		[Description("")]
        public string AllowAdvancedQuery { get; set; } // AllowAdvancedQuery
		/// <summary></summary>	
		[Description("")]
        public string PreCondition { get; set; } // PreCondition
		/// <summary></summary>	
		[Description("")]
        public string IsShowIndex { get; set; } // IsShowIndex
		/// <summary></summary>	
		[Description("")]
        public string ExtentionJson { get; set; } // ExtentionJson
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_DOC_FileAttr> S_DOC_FileAttr { get { onS_DOC_FileAttrGetting(); return _S_DOC_FileAttr;} }
		private ICollection<S_DOC_FileAttr> _S_DOC_FileAttr;
		partial void onS_DOC_FileAttrGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_FileNodeRelation> S_DOC_FileNodeRelation { get { onS_DOC_FileNodeRelationGetting(); return _S_DOC_FileNodeRelation;} }
		private ICollection<S_DOC_FileNodeRelation> _S_DOC_FileNodeRelation;
		partial void onS_DOC_FileNodeRelationGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_File_S_C_Space

        public S_DOC_File()
        {
            _S_DOC_FileAttr = new List<S_DOC_FileAttr>();
            _S_DOC_FileNodeRelation = new List<S_DOC_FileNodeRelation>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_FileAttr : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string FileAttrName { get; set; } // FileAttrName
		/// <summary></summary>	
		[Description("")]
        public string FileAttrField { get; set; } // FileAttrField
		/// <summary></summary>	
		[Description("")]
        public string DataType { get; set; } // DataType
		/// <summary></summary>	
		[Description("")]
        public string InputType { get; set; } // InputType
		/// <summary></summary>	
		[Description("")]
        public string ValidateType { get; set; } // ValidateType
		/// <summary></summary>	
		[Description("")]
        public string AttrType { get; set; } // AttrType
		/// <summary></summary>	
		[Description("")]
        public string IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary></summary>	
		[Description("")]
        public string MultiSelect { get; set; } // MultiSelect
		/// <summary></summary>	
		[Description("")]
        public string Required { get; set; } // Required
		/// <summary></summary>	
		[Description("")]
        public string TextFieldName { get; set; } // TextFieldName
		/// <summary></summary>	
		[Description("")]
        public string VType { get; set; } // VType
		/// <summary></summary>	
		[Description("")]
        public int AttrSort { get; set; } // AttrSort
		/// <summary></summary>	
		[Description("")]
        public string Visible { get; set; } // Visible
		/// <summary></summary>	
		[Description("")]
        public string Disabled { get; set; } // Disabled
		/// <summary></summary>	
		[Description("")]
        public string DefaultValue { get; set; } // DefaultValue
		/// <summary></summary>	
		[Description("")]
        public string FulltextProp { get; set; } // FulltextProp
		/// <summary></summary>	
		[Description("")]
        public string AdvancedSearch { get; set; } // AdvancedSearch

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_File S_DOC_File { get; set; } //  FileID - FK_S_C_FileAttr_S_C_File
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_FileNodeRelation : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public int Sort { get; set; } // Sort

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Node S_DOC_Node { get; set; } //  NodeID - FK_S_C_FileNodeRelation_S_C_Node
		[JsonIgnore]
        public virtual S_DOC_File S_DOC_File { get; set; } //  FileID - FK_S_C_FileNodeRelation_S_C_File
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_FulltextSearchConvertLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FsFileID { get; set; } // FsFileID
		/// <summary></summary>	
		[Description("")]
        public string AttrID { get; set; } // AttrID
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ConvertState { get; set; } // ConvertState
		/// <summary></summary>	
		[Description("")]
        public string ErrorMeesage { get; set; } // ErrorMeesage
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_ListConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RelationID { get; set; } // RelationID
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string QueryKeyText { get; set; } // QueryKeyText
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_DOC_ListConfigDetail> S_DOC_ListConfigDetail { get { onS_DOC_ListConfigDetailGetting(); return _S_DOC_ListConfigDetail;} }
		private ICollection<S_DOC_ListConfigDetail> _S_DOC_ListConfigDetail;
		partial void onS_DOC_ListConfigDetailGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_QueryParam> S_DOC_QueryParam { get { onS_DOC_QueryParamGetting(); return _S_DOC_QueryParam;} }
		private ICollection<S_DOC_QueryParam> _S_DOC_QueryParam;
		partial void onS_DOC_QueryParamGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_ListConfig_S_C_Space

        public S_DOC_ListConfig()
        {
            _S_DOC_ListConfigDetail = new List<S_DOC_ListConfigDetail>();
            _S_DOC_QueryParam = new List<S_DOC_QueryParam>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_ListConfigDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ListConfigID { get; set; } // ListConfigID
		/// <summary></summary>	
		[Description("")]
        public string AttrName { get; set; } // AttrName
		/// <summary></summary>	
		[Description("")]
        public string AttrField { get; set; } // AttrField
		/// <summary></summary>	
		[Description("")]
        public int DetailSort { get; set; } // DetailSort
		/// <summary></summary>	
		[Description("")]
        public string AllowSort { get; set; } // AllowSort
		/// <summary></summary>	
		[Description("")]
        public string Align { get; set; } // Align
		/// <summary></summary>	
		[Description("")]
        public int? Width { get; set; } // Width
		/// <summary></summary>	
		[Description("")]
        public string Dispaly { get; set; } // Dispaly

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_ListConfig S_DOC_ListConfig { get; set; } //  ListConfigID - FK_S_C_ListConfigDetail_S_C_ListConfig
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_Node : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string CanBorrow { get; set; } // CanBorrow
		/// <summary></summary>	
		[Description("")]
        public string BorrowFlowKey { get; set; } // BorrowFlowKey
		/// <summary></summary>	
		[Description("")]
        public string AllowDisplay { get; set; } // AllowDisplay
		/// <summary></summary>	
		[Description("")]
        public string AllowAdvancedQuery { get; set; } // AllowAdvancedQuery
		/// <summary></summary>	
		[Description("")]
        public string IsFreeNode { get; set; } // IsFreeNode
		/// <summary></summary>	
		[Description("")]
        public string IsShowIndex { get; set; } // IsShowIndex
		/// <summary></summary>	
		[Description("")]
        public string ExtentionJson { get; set; } // ExtentionJson
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_DOC_FileNodeRelation> S_DOC_FileNodeRelation { get { onS_DOC_FileNodeRelationGetting(); return _S_DOC_FileNodeRelation;} }
		private ICollection<S_DOC_FileNodeRelation> _S_DOC_FileNodeRelation;
		partial void onS_DOC_FileNodeRelationGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_NodeAttr> S_DOC_NodeAttr { get { onS_DOC_NodeAttrGetting(); return _S_DOC_NodeAttr;} }
		private ICollection<S_DOC_NodeAttr> _S_DOC_NodeAttr;
		partial void onS_DOC_NodeAttrGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_Node_S_C_Space

        public S_DOC_Node()
        {
            _S_DOC_FileNodeRelation = new List<S_DOC_FileNodeRelation>();
            _S_DOC_NodeAttr = new List<S_DOC_NodeAttr>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_NodeAttr : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string AttrName { get; set; } // AttrName
		/// <summary></summary>	
		[Description("")]
        public string AttrField { get; set; } // AttrField
		/// <summary></summary>	
		[Description("")]
        public string DataType { get; set; } // DataType
		/// <summary></summary>	
		[Description("")]
        public string InputType { get; set; } // InputType
		/// <summary></summary>	
		[Description("")]
        public string ValidateType { get; set; } // ValidateType
		/// <summary></summary>	
		[Description("")]
        public string AttrType { get; set; } // AttrType
		/// <summary></summary>	
		[Description("")]
        public string IsEnum { get; set; } // IsEnum
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary></summary>	
		[Description("")]
        public string MultiSelect { get; set; } // MultiSelect
		/// <summary></summary>	
		[Description("")]
        public string TextFieldName { get; set; } // TextFieldName
		/// <summary></summary>	
		[Description("")]
        public string Required { get; set; } // Required
		/// <summary></summary>	
		[Description("")]
        public string VType { get; set; } // VType
		/// <summary></summary>	
		[Description("")]
        public int AttrSort { get; set; } // AttrSort
		/// <summary></summary>	
		[Description("")]
        public string Visible { get; set; } // Visible
		/// <summary></summary>	
		[Description("")]
        public string Disabled { get; set; } // Disabled
		/// <summary></summary>	
		[Description("")]
        public string DefaultValue { get; set; } // DefaultValue
		/// <summary></summary>	
		[Description("")]
        public string FulltextProp { get; set; } // FulltextProp
		/// <summary></summary>	
		[Description("")]
        public string AdvancedSearch { get; set; } // AdvancedSearch

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Node S_DOC_Node { get; set; } //  NodeID - FK_S_C_NodeAttr_S_C_Node
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_NodeStrcut : Formula.BaseModel
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
        public string FullPathID { get; set; } // FullPathID
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_NodeStrcut_S_C_Space
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_QueryParam : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string AttrName { get; set; } // AttrName
		/// <summary></summary>	
		[Description("")]
        public string AttrField { get; set; } // AttrField
		/// <summary></summary>	
		[Description("")]
        public string QueryType { get; set; } // QueryType
		/// <summary></summary>	
		[Description("")]
        public string InKey { get; set; } // InKey
		/// <summary></summary>	
		[Description("")]
        public string InnerField { get; set; } // InnerField
		/// <summary></summary>	
		[Description("")]
        public string InAdvancedQuery { get; set; } // InAdvancedQuery
		/// <summary></summary>	
		[Description("")]
        public string ListConfigID { get; set; } // ListConfigID
		/// <summary></summary>	
		[Description("")]
        public int QuerySort { get; set; } // QuerySort
		/// <summary></summary>	
		[Description("")]
        public string DataType { get; set; } // DataType

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_ListConfig S_DOC_ListConfig { get; set; } //  ListConfigID - FK_S_C_QueryParam_S_C_ListConfig
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_ReorganizeConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string CountSQL { get; set; } // CountSQL
		/// <summary></summary>	
		[Description("")]
        public string Enabled { get; set; } // Enabled

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_ReorganizeConfig_S_C_Space
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_Space : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string SpaceKey { get; set; } // SpaceKey
		/// <summary></summary>	
		[Description("")]
        public string Server { get; set; } // Server
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string DbName { get; set; } // DbName
		/// <summary></summary>	
		[Description("")]
        public string Pwd { get; set; } // Pwd
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_DOC_File> S_DOC_File { get { onS_DOC_FileGetting(); return _S_DOC_File;} }
		private ICollection<S_DOC_File> _S_DOC_File;
		partial void onS_DOC_FileGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_ListConfig> S_DOC_ListConfig { get { onS_DOC_ListConfigGetting(); return _S_DOC_ListConfig;} }
		private ICollection<S_DOC_ListConfig> _S_DOC_ListConfig;
		partial void onS_DOC_ListConfigGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_Node> S_DOC_Node { get { onS_DOC_NodeGetting(); return _S_DOC_Node;} }
		private ICollection<S_DOC_Node> _S_DOC_Node;
		partial void onS_DOC_NodeGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_NodeStrcut> S_DOC_NodeStrcut { get { onS_DOC_NodeStrcutGetting(); return _S_DOC_NodeStrcut;} }
		private ICollection<S_DOC_NodeStrcut> _S_DOC_NodeStrcut;
		partial void onS_DOC_NodeStrcutGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_ReorganizeConfig> S_DOC_ReorganizeConfig { get { onS_DOC_ReorganizeConfigGetting(); return _S_DOC_ReorganizeConfig;} }
		private ICollection<S_DOC_ReorganizeConfig> _S_DOC_ReorganizeConfig;
		partial void onS_DOC_ReorganizeConfigGetting();

		[JsonIgnore]
        public virtual ICollection<S_DOC_TreeConfig> S_DOC_TreeConfig { get { onS_DOC_TreeConfigGetting(); return _S_DOC_TreeConfig;} }
		private ICollection<S_DOC_TreeConfig> _S_DOC_TreeConfig;
		partial void onS_DOC_TreeConfigGetting();


        public S_DOC_Space()
        {
            _S_DOC_File = new List<S_DOC_File>();
            _S_DOC_ListConfig = new List<S_DOC_ListConfig>();
            _S_DOC_Node = new List<S_DOC_Node>();
            _S_DOC_NodeStrcut = new List<S_DOC_NodeStrcut>();
            _S_DOC_ReorganizeConfig = new List<S_DOC_ReorganizeConfig>();
            _S_DOC_TreeConfig = new List<S_DOC_TreeConfig>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_TagConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int ID { get; set; } // ID (Primary key)
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_TreeConfig : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string TreeDisplay { get; set; } // TreeDisplay
		/// <summary></summary>	
		[Description("")]
        public string TreeSort { get; set; } // TreeSort

        // Foreign keys
		[JsonIgnore]
        public virtual S_DOC_Space S_DOC_Space { get; set; } //  SpaceID - FK_S_C_TreeConfig_S_C_Space
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DOC_UserSearhHistory : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string OperateType { get; set; } // OperateType
    }


    // ************************************************************************
    // POCO Configuration

    // S_DOC_File
    internal partial class S_DOC_FileConfiguration : EntityTypeConfiguration<S_DOC_File>
    {
        public S_DOC_FileConfiguration()
        {
            ToTable("S_DOC_FILE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.CanBorrow).HasColumnName("CANBORROW").IsRequired().HasMaxLength(50);
            Property(x => x.BorrowFlowKey).HasColumnName("BORROWFLOWKEY").IsOptional().HasMaxLength(200);
            Property(x => x.CanDownload).HasColumnName("CANDOWNLOAD").IsRequired().HasMaxLength(50);
            Property(x => x.DownloadFlowKey).HasColumnName("DOWNLOADFLOWKEY").IsOptional().HasMaxLength(50);
            Property(x => x.AllowDisplay).HasColumnName("ALLOWDISPLAY").IsRequired().HasMaxLength(50);
            Property(x => x.AllowAdvancedQuery).HasColumnName("ALLOWADVANCEDQUERY").IsRequired().HasMaxLength(50);
            Property(x => x.PreCondition).HasColumnName("PRECONDITION").IsOptional().HasMaxLength(500);
            Property(x => x.IsShowIndex).HasColumnName("ISSHOWINDEX").IsOptional().HasMaxLength(50);
            Property(x => x.ExtentionJson).HasColumnName("EXTENTIONJSON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_DOC_Space).WithMany(b => b.S_DOC_File).HasForeignKey(c => c.SpaceID); // FK_S_C_File_S_C_Space
        }
    }

    // S_DOC_FileAttr
    internal partial class S_DOC_FileAttrConfiguration : EntityTypeConfiguration<S_DOC_FileAttr>
    {
        public S_DOC_FileAttrConfiguration()
        {
            ToTable("S_DOC_FILEATTR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsRequired().HasMaxLength(50);
            Property(x => x.FileAttrName).HasColumnName("FILEATTRNAME").IsRequired().HasMaxLength(500);
            Property(x => x.FileAttrField).HasColumnName("FILEATTRFIELD").IsRequired().HasMaxLength(500);
            Property(x => x.DataType).HasColumnName("DATATYPE").IsRequired().HasMaxLength(50);
            Property(x => x.InputType).HasColumnName("INPUTTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ValidateType).HasColumnName("VALIDATETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.AttrType).HasColumnName("ATTRTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsRequired().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.MultiSelect).HasColumnName("MULTISELECT").IsOptional().HasMaxLength(50);
            Property(x => x.Required).HasColumnName("REQUIRED").IsRequired().HasMaxLength(50);
            Property(x => x.TextFieldName).HasColumnName("TEXTFIELDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.VType).HasColumnName("VTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.AttrSort).HasColumnName("ATTRSORT").IsRequired();
            Property(x => x.Visible).HasColumnName("VISIBLE").IsRequired().HasMaxLength(50);
            Property(x => x.Disabled).HasColumnName("DISABLED").IsRequired().HasMaxLength(50);
            Property(x => x.DefaultValue).HasColumnName("DEFAULTVALUE").IsOptional().HasMaxLength(500);
            Property(x => x.FulltextProp).HasColumnName("FULLTEXTPROP").IsOptional().HasMaxLength(50);
            Property(x => x.AdvancedSearch).HasColumnName("ADVANCEDSEARCH").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_File).WithMany(b => b.S_DOC_FileAttr).HasForeignKey(c => c.FileID); // FK_S_C_FileAttr_S_C_File
        }
    }

    // S_DOC_FileNodeRelation
    internal partial class S_DOC_FileNodeRelationConfiguration : EntityTypeConfiguration<S_DOC_FileNodeRelation>
    {
        public S_DOC_FileNodeRelationConfiguration()
        {
            ToTable("S_DOC_FILENODERELATION");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.NodeID).HasColumnName("NODEID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsRequired().HasMaxLength(50);
            Property(x => x.Sort).HasColumnName("SORT").IsRequired();

            // Foreign keys
            HasRequired(a => a.S_DOC_Node).WithMany(b => b.S_DOC_FileNodeRelation).HasForeignKey(c => c.NodeID); // FK_S_C_FileNodeRelation_S_C_Node
            HasRequired(a => a.S_DOC_File).WithMany(b => b.S_DOC_FileNodeRelation).HasForeignKey(c => c.FileID); // FK_S_C_FileNodeRelation_S_C_File
        }
    }

    // S_DOC_FulltextSearchConvertLog
    internal partial class S_DOC_FulltextSearchConvertLogConfiguration : EntityTypeConfiguration<S_DOC_FulltextSearchConvertLog>
    {
        public S_DOC_FulltextSearchConvertLogConfiguration()
        {
            ToTable("S_DOC_FULLTEXTSEARCHCONVERTLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FsFileID).HasColumnName("FSFILEID").IsOptional().HasMaxLength(500);
            Property(x => x.AttrID).HasColumnName("ATTRID").IsOptional().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(50);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ConvertState).HasColumnName("CONVERTSTATE").IsOptional().HasMaxLength(50);
            Property(x => x.ErrorMeesage).HasColumnName("ERRORMEESAGE").IsOptional();
        }
    }

    // S_DOC_ListConfig
    internal partial class S_DOC_ListConfigConfiguration : EntityTypeConfiguration<S_DOC_ListConfig>
    {
        public S_DOC_ListConfigConfiguration()
        {
            ToTable("S_DOC_LISTCONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.RelationID).HasColumnName("RELATIONID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.QueryKeyText).HasColumnName("QUERYKEYTEXT").IsOptional().HasMaxLength(500);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_Space).WithMany(b => b.S_DOC_ListConfig).HasForeignKey(c => c.SpaceID); // FK_S_C_ListConfig_S_C_Space
        }
    }

    // S_DOC_ListConfigDetail
    internal partial class S_DOC_ListConfigDetailConfiguration : EntityTypeConfiguration<S_DOC_ListConfigDetail>
    {
        public S_DOC_ListConfigDetailConfiguration()
        {
            ToTable("S_DOC_LISTCONFIGDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ListConfigID).HasColumnName("LISTCONFIGID").IsRequired().HasMaxLength(50);
            Property(x => x.AttrName).HasColumnName("ATTRNAME").IsRequired().HasMaxLength(200);
            Property(x => x.AttrField).HasColumnName("ATTRFIELD").IsRequired().HasMaxLength(200);
            Property(x => x.DetailSort).HasColumnName("DETAILSORT").IsRequired();
            Property(x => x.AllowSort).HasColumnName("ALLOWSORT").IsRequired().HasMaxLength(50);
            Property(x => x.Align).HasColumnName("ALIGN").IsRequired().HasMaxLength(50);
            Property(x => x.Width).HasColumnName("WIDTH").IsOptional();
            Property(x => x.Dispaly).HasColumnName("DISPALY").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_ListConfig).WithMany(b => b.S_DOC_ListConfigDetail).HasForeignKey(c => c.ListConfigID); // FK_S_C_ListConfigDetail_S_C_ListConfig
        }
    }

    // S_DOC_Node
    internal partial class S_DOC_NodeConfiguration : EntityTypeConfiguration<S_DOC_Node>
    {
        public S_DOC_NodeConfiguration()
        {
            ToTable("S_DOC_NODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.CanBorrow).HasColumnName("CANBORROW").IsRequired().HasMaxLength(50);
            Property(x => x.BorrowFlowKey).HasColumnName("BORROWFLOWKEY").IsOptional().HasMaxLength(200);
            Property(x => x.AllowDisplay).HasColumnName("ALLOWDISPLAY").IsRequired().HasMaxLength(50);
            Property(x => x.AllowAdvancedQuery).HasColumnName("ALLOWADVANCEDQUERY").IsRequired().HasMaxLength(50);
            Property(x => x.IsFreeNode).HasColumnName("ISFREENODE").IsOptional().HasMaxLength(50);
            Property(x => x.IsShowIndex).HasColumnName("ISSHOWINDEX").IsOptional().HasMaxLength(50);
            Property(x => x.ExtentionJson).HasColumnName("EXTENTIONJSON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasRequired(a => a.S_DOC_Space).WithMany(b => b.S_DOC_Node).HasForeignKey(c => c.SpaceID); // FK_S_C_Node_S_C_Space
        }
    }

    // S_DOC_NodeAttr
    internal partial class S_DOC_NodeAttrConfiguration : EntityTypeConfiguration<S_DOC_NodeAttr>
    {
        public S_DOC_NodeAttrConfiguration()
        {
            ToTable("S_DOC_NODEATTR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.NodeID).HasColumnName("NODEID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.AttrName).HasColumnName("ATTRNAME").IsRequired().HasMaxLength(200);
            Property(x => x.AttrField).HasColumnName("ATTRFIELD").IsRequired().HasMaxLength(200);
            Property(x => x.DataType).HasColumnName("DATATYPE").IsRequired().HasMaxLength(50);
            Property(x => x.InputType).HasColumnName("INPUTTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ValidateType).HasColumnName("VALIDATETYPE").IsRequired().HasMaxLength(50);
            Property(x => x.AttrType).HasColumnName("ATTRTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.IsEnum).HasColumnName("ISENUM").IsRequired().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(500);
            Property(x => x.MultiSelect).HasColumnName("MULTISELECT").IsOptional().HasMaxLength(50);
            Property(x => x.TextFieldName).HasColumnName("TEXTFIELDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Required).HasColumnName("REQUIRED").IsRequired().HasMaxLength(50);
            Property(x => x.VType).HasColumnName("VTYPE").IsOptional().HasMaxLength(500);
            Property(x => x.AttrSort).HasColumnName("ATTRSORT").IsRequired();
            Property(x => x.Visible).HasColumnName("VISIBLE").IsRequired().HasMaxLength(50);
            Property(x => x.Disabled).HasColumnName("DISABLED").IsRequired().HasMaxLength(50);
            Property(x => x.DefaultValue).HasColumnName("DEFAULTVALUE").IsOptional().HasMaxLength(500);
            Property(x => x.FulltextProp).HasColumnName("FULLTEXTPROP").IsOptional().HasMaxLength(50);
            Property(x => x.AdvancedSearch).HasColumnName("ADVANCEDSEARCH").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_Node).WithMany(b => b.S_DOC_NodeAttr).HasForeignKey(c => c.NodeID); // FK_S_C_NodeAttr_S_C_Node
        }
    }

    // S_DOC_NodeStrcut
    internal partial class S_DOC_NodeStrcutConfiguration : EntityTypeConfiguration<S_DOC_NodeStrcut>
    {
        public S_DOC_NodeStrcutConfiguration()
        {
            ToTable("S_DOC_NODESTRCUT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullPathID).HasColumnName("FULLPATHID").IsRequired().HasMaxLength(500);
            Property(x => x.NodeID).HasColumnName("NODEID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_Space).WithMany(b => b.S_DOC_NodeStrcut).HasForeignKey(c => c.SpaceID); // FK_S_C_NodeStrcut_S_C_Space
        }
    }

    // S_DOC_QueryParam
    internal partial class S_DOC_QueryParamConfiguration : EntityTypeConfiguration<S_DOC_QueryParam>
    {
        public S_DOC_QueryParamConfiguration()
        {
            ToTable("S_DOC_QUERYPARAM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.AttrName).HasColumnName("ATTRNAME").IsRequired().HasMaxLength(500);
            Property(x => x.AttrField).HasColumnName("ATTRFIELD").IsRequired().HasMaxLength(500);
            Property(x => x.QueryType).HasColumnName("QUERYTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.InKey).HasColumnName("INKEY").IsRequired().HasMaxLength(50);
            Property(x => x.InnerField).HasColumnName("INNERFIELD").IsOptional().HasMaxLength(500);
            Property(x => x.InAdvancedQuery).HasColumnName("INADVANCEDQUERY").IsRequired().HasMaxLength(50);
            Property(x => x.ListConfigID).HasColumnName("LISTCONFIGID").IsRequired().HasMaxLength(50);
            Property(x => x.QuerySort).HasColumnName("QUERYSORT").IsRequired();
            Property(x => x.DataType).HasColumnName("DATATYPE").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_DOC_ListConfig).WithMany(b => b.S_DOC_QueryParam).HasForeignKey(c => c.ListConfigID); // FK_S_C_QueryParam_S_C_ListConfig
        }
    }

    // S_DOC_ReorganizeConfig
    internal partial class S_DOC_ReorganizeConfigConfiguration : EntityTypeConfiguration<S_DOC_ReorganizeConfig>
    {
        public S_DOC_ReorganizeConfigConfiguration()
        {
            ToTable("S_DOC_REORGANIZECONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(50);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CountSQL).HasColumnName("COUNTSQL").IsOptional();
            Property(x => x.Enabled).HasColumnName("ENABLED").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_DOC_Space).WithMany(b => b.S_DOC_ReorganizeConfig).HasForeignKey(c => c.SpaceID); // FK_S_C_ReorganizeConfig_S_C_Space
        }
    }

    // S_DOC_Space
    internal partial class S_DOC_SpaceConfiguration : EntityTypeConfiguration<S_DOC_Space>
    {
        public S_DOC_SpaceConfiguration()
        {
            ToTable("S_DOC_SPACE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.SpaceKey).HasColumnName("SPACEKEY").IsRequired().HasMaxLength(500);
            Property(x => x.Server).HasColumnName("SERVER").IsRequired().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.DbName).HasColumnName("DBNAME").IsRequired().HasMaxLength(50);
            Property(x => x.Pwd).HasColumnName("PWD").IsRequired().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_DOC_TagConfig
    internal partial class S_DOC_TagConfigConfiguration : EntityTypeConfiguration<S_DOC_TagConfig>
    {
        public S_DOC_TagConfigConfiguration()
        {
            ToTable("S_DOC_TAGCONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }

    // S_DOC_TreeConfig
    internal partial class S_DOC_TreeConfigConfiguration : EntityTypeConfiguration<S_DOC_TreeConfig>
    {
        public S_DOC_TreeConfigConfiguration()
        {
            ToTable("S_DOC_TREECONFIG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.TreeDisplay).HasColumnName("TREEDISPLAY").IsRequired().HasMaxLength(500);
            Property(x => x.TreeSort).HasColumnName("TREESORT").IsRequired();

            // Foreign keys
            HasRequired(a => a.S_DOC_Space).WithMany(b => b.S_DOC_TreeConfig).HasForeignKey(c => c.SpaceID); // FK_S_C_TreeConfig_S_C_Space
        }
    }

    // S_DOC_UserSearhHistory
    internal partial class S_DOC_UserSearhHistoryConfiguration : EntityTypeConfiguration<S_DOC_UserSearhHistory>
    {
        public S_DOC_UserSearhHistoryConfiguration()
        {
            ToTable("S_DOC_USERSEARHHISTORY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(50);
            Property(x => x.OperateType).HasColumnName("OPERATETYPE").IsOptional().HasMaxLength(50);
        }
    }

}

