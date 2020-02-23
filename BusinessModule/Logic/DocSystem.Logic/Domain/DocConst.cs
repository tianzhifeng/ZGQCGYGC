

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "DocConst"
//     Connection String:      "Data Source=10.10.1.235\sql2008;Initial Catalog=Arch_DocConst;User ID=sa;PWD=123.zxc;"

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
    public partial class DocConstEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_ArchiveCache> S_ArchiveCache { get; set; } // S_ArchiveCache
        public IDbSet<S_ArchiveCacheCatalog> S_ArchiveCacheCatalog { get; set; } // S_ArchiveCacheCatalog
        public IDbSet<S_BorrowDetail> S_BorrowDetail { get; set; } // S_BorrowDetail
        public IDbSet<S_CarInfo> S_CarInfo { get; set; } // S_CarInfo
        public IDbSet<S_DocumentLog> S_DocumentLog { get; set; } // S_DocumentLog
        public IDbSet<S_DownloadDetail> S_DownloadDetail { get; set; } // S_DownloadDetail
        public IDbSet<S_UserAdvanceQueryInfo> S_UserAdvanceQueryInfo { get; set; } // S_UserAdvanceQueryInfo
        public IDbSet<T_Borrow> T_Borrow { get; set; } // T_Borrow
        public IDbSet<T_Borrow_FileInfo> T_Borrow_FileInfo { get; set; } // T_Borrow_FileInfo
        public IDbSet<T_Download> T_Download { get; set; } // T_Download
        public IDbSet<T_Download_FileInfo> T_Download_FileInfo { get; set; } // T_Download_FileInfo

        static DocConstEntities()
        {
            Database.SetInitializer<DocConstEntities>(null);
        }

        public DocConstEntities()
            : base("Name=DocConst")
        {
        }

        public DocConstEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_ArchiveCacheConfiguration());
            modelBuilder.Configurations.Add(new S_ArchiveCacheCatalogConfiguration());
            modelBuilder.Configurations.Add(new S_BorrowDetailConfiguration());
            modelBuilder.Configurations.Add(new S_CarInfoConfiguration());
            modelBuilder.Configurations.Add(new S_DocumentLogConfiguration());
            modelBuilder.Configurations.Add(new S_DownloadDetailConfiguration());
            modelBuilder.Configurations.Add(new S_UserAdvanceQueryInfoConfiguration());
            modelBuilder.Configurations.Add(new T_BorrowConfiguration());
            modelBuilder.Configurations.Add(new T_Borrow_FileInfoConfiguration());
            modelBuilder.Configurations.Add(new T_DownloadConfiguration());
            modelBuilder.Configurations.Add(new T_Download_FileInfoConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class S_ArchiveCache : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary></summary>	
		[Description("")]
        public string FileType { get; set; } // FileType
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string MainFile { get; set; } // MainFile
		/// <summary></summary>	
		[Description("")]
        public string PdfFile { get; set; } // PdfFile
		/// <summary></summary>	
		[Description("")]
        public string PrintFile { get; set; } // PrintFile
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string Data { get; set; } // Data
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime ArchiveDate { get; set; } // ArchiveDate
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string CatalogID { get; set; } // CatalogID
		/// <summary></summary>	
		[Description("")]
        public string FullCatalogID { get; set; } // FullCatalogID
		/// <summary></summary>	
		[Description("")]
        public string SN { get; set; } // SN
		/// <summary></summary>	
		[Description("")]
        public string SNFlag { get; set; } // SNFlag

        // Foreign keys
		[JsonIgnore]
        public virtual S_ArchiveCacheCatalog S_ArchiveCacheCatalog { get; set; } //  CatalogID - FK_S_ArchiveCache_S_ArchiveCacheCatalog
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_ArchiveCacheCatalog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
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
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Data { get; set; } // Data
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string RelateID { get; set; } // RelateID
		/// <summary></summary>	
		[Description("")]
        public string ArchiveFormID { get; set; } // ArchiveFormID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_ArchiveCache> S_ArchiveCache { get { onS_ArchiveCacheGetting(); return _S_ArchiveCache;} }
		private ICollection<S_ArchiveCache> _S_ArchiveCache;
		partial void onS_ArchiveCacheGetting();


        public S_ArchiveCacheCatalog()
        {
            _S_ArchiveCache = new List<S_ArchiveCache>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_BorrowDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string BorrowID { get; set; } // BorrowID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string DetailType { get; set; } // DetailType
		/// <summary></summary>	
		[Description("")]
        public string RelateID { get; set; } // RelateID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string ReturnUserID { get; set; } // ReturnUserID
		/// <summary></summary>	
		[Description("")]
        public string ReturnUser { get; set; } // ReturnUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? ReturnDate { get; set; } // ReturnDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? LendDate { get; set; } // LendDate
		/// <summary></summary>	
		[Description("")]
        public string LendUserID { get; set; } // LendUserID
		/// <summary></summary>	
		[Description("")]
        public string LendUserName { get; set; } // LendUserName
		/// <summary></summary>	
		[Description("")]
        public string BorrowState { get; set; } // BorrowState
		/// <summary></summary>	
		[Description("")]
        public DateTime? BorrowExpireDate { get; set; } // BorrowExpireDate
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_CarInfo : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string DataType { get; set; } // DataType
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string NodeName { get; set; } // NodeName
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DocumentLog : Formula.BaseModel
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
        public string NodeID { get; set; } // NodeID
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary></summary>	
		[Description("")]
        public string FullNodeID { get; set; } // FullNodeID
		/// <summary></summary>	
		[Description("")]
        public string LogType { get; set; } // LogType
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_DownloadDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DownloadID { get; set; } // DownloadID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary></summary>	
		[Description("")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string UserDeptID { get; set; } // UserDeptID
		/// <summary></summary>	
		[Description("")]
        public string UserDeptName { get; set; } // UserDeptName
		/// <summary></summary>	
		[Description("")]
        public DateTime? PassDate { get; set; } // PassDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? DownloadExpireDate { get; set; } // DownloadExpireDate
		/// <summary></summary>	
		[Description("")]
        public string DownloadState { get; set; } // DownloadState
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UserAdvanceQueryInfo : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string QueryData { get; set; } // QueryData
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
    }

	/// <summary>T</summary>	
	[Description("T")]
    public partial class T_Borrow : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string UserDept { get; set; } // UserDept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string UserDeptName { get; set; } // UserDeptName
		/// <summary>借阅日期</summary>	
		[Description("借阅日期")]
        public DateTime? LendDate { get; set; } // LendDate
		/// <summary>归还日期</summary>	
		[Description("归还日期")]
        public DateTime? ReturnDate { get; set; } // ReturnDate
		/// <summary>用途</summary>	
		[Description("用途")]
        public string Purpose { get; set; } // Purpose
		/// <summary>分机号</summary>	
		[Description("分机号")]
        public string Phone { get; set; } // Phone
		/// <summary>项目负责人</summary>	
		[Description("项目负责人")]
        public string PrjPrincipleSign { get; set; } // PrjPrincipleSign
		/// <summary>技术质保部负责人</summary>	
		[Description("技术质保部负责人")]
        public string DirecotorSign { get; set; } // DirecotorSign
		/// <summary>领导审批</summary>	
		[Description("领导审批")]
        public string LeaderSign { get; set; } // LeaderSign
		/// <summary>创建人名称</summary>	
		[Description("创建人名称")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>申请通过日期</summary>	
		[Description("申请通过日期")]
        public DateTime? PassDate { get; set; } // PassDate
		/// <summary>流程ID</summary>	
		[Description("流程ID")]
        public string FlowID { get; set; } // FlowID
		/// <summary>借阅状态</summary>	
		[Description("借阅状态")]
        public string BorrowState { get; set; } // BorrowState
		/// <summary>用途详细说明(暂未用到可删除)</summary>	
		[Description("用途详细说明(暂未用到可删除)")]
        public string PurposeDetail { get; set; } // PurposeDetail
		/// <summary>项目负责人ID</summary>	
		[Description("项目负责人ID")]
        public string PrjPrincipleID { get; set; } // PrjPrincipleID
		/// <summary>项目负责人Name</summary>	
		[Description("项目负责人Name")]
        public string PrjPrincipleName { get; set; } // PrjPrincipleName
		/// <summary>项目负责人签字日期</summary>	
		[Description("项目负责人签字日期")]
        public string PrjPrincipleDate { get; set; } // PrjPrincipleDate
		/// <summary>技术质保部负责人ID</summary>	
		[Description("技术质保部负责人ID")]
        public string DirecotorID { get; set; } // DirecotorID
		/// <summary>技术质保部负责人Name</summary>	
		[Description("技术质保部负责人Name")]
        public string DirecotorName { get; set; } // DirecotorName
		/// <summary>技术质保部负责人签字日期</summary>	
		[Description("技术质保部负责人签字日期")]
        public string DirecotorDate { get; set; } // DirecotorDate
		/// <summary>领导审批ID</summary>	
		[Description("领导审批ID")]
        public string LeaderID { get; set; } // LeaderID
		/// <summary>领导审批Name</summary>	
		[Description("领导审批Name")]
        public string LeaderName { get; set; } // LeaderName
		/// <summary>领导审批通过日期</summary>	
		[Description("领导审批通过日期")]
        public string LeaderDate { get; set; } // LeaderDate
		/// <summary>SpaceID</summary>	
		[Description("SpaceID")]
        public string SpaceID { get; set; } // SpaceID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_Borrow_FileInfo> T_Borrow_FileInfo { get { onT_Borrow_FileInfoGetting(); return _T_Borrow_FileInfo;} }
		private ICollection<T_Borrow_FileInfo> _T_Borrow_FileInfo;
		partial void onT_Borrow_FileInfoGetting();


        public T_Borrow()
        {
            _T_Borrow_FileInfo = new List<T_Borrow_FileInfo>();
        }
    }

	/// <summary>子表</summary>	
	[Description("子表")]
    public partial class T_Borrow_FileInfo : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>T_BorrowID</summary>	
		[Description("T_BorrowID")]
        public string T_BorrowID { get; set; } // T_BorrowID
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>已发布</summary>	
		[Description("已发布")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>案卷编号</summary>	
		[Description("案卷编号")]
        public string FileCode { get; set; } // FileCode
		/// <summary>案卷名称</summary>	
		[Description("案卷名称")]
        public string FileName { get; set; } // FileName
		/// <summary>FileID</summary>	
		[Description("FileID")]
        public string FileID { get; set; } // FileID
		/// <summary>CarInfoID</summary>	
		[Description("CarInfoID")]
        public string CarInfoID { get; set; } // CarInfoID
		/// <summary>ConfigID</summary>	
		[Description("ConfigID")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary>NodeID</summary>	
		[Description("NodeID")]
        public string NodeID { get; set; } // NodeID
		/// <summary>SpaceID</summary>	
		[Description("SpaceID")]
        public string SpaceID { get; set; } // SpaceID

        // Foreign keys
		[JsonIgnore]
        public virtual T_Borrow T_Borrow { get; set; } //  T_BorrowID - FK_T_Borrow_FileInfo_T_Borrow
    }

	/// <summary>T</summary>	
	[Description("T")]
    public partial class T_Download : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string UserDept { get; set; } // UserDept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string UserDeptName { get; set; } // UserDeptName
		/// <summary>用途</summary>	
		[Description("用途")]
        public string Purpose { get; set; } // Purpose
		/// <summary>创建人名称</summary>	
		[Description("创建人名称")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>申请通过日期</summary>	
		[Description("申请通过日期")]
        public DateTime? PassDate { get; set; } // PassDate
		/// <summary>下载状态</summary>	
		[Description("下载状态")]
        public string DownloadState { get; set; } // DownloadState
		/// <summary>SpaceID</summary>	
		[Description("SpaceID")]
        public string SpaceID { get; set; } // SpaceID
		/// <summary>流程ID</summary>	
		[Description("流程ID")]
        public string FlowID { get; set; } // FlowID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_Download_FileInfo> T_Download_FileInfo { get { onT_Download_FileInfoGetting(); return _T_Download_FileInfo;} }
		private ICollection<T_Download_FileInfo> _T_Download_FileInfo;
		partial void onT_Download_FileInfoGetting();


        public T_Download()
        {
            _T_Download_FileInfo = new List<T_Download_FileInfo>();
        }
    }

	/// <summary>子表</summary>	
	[Description("子表")]
    public partial class T_Download_FileInfo : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>T_DownloadID</summary>	
		[Description("T_DownloadID")]
        public string T_DownloadID { get; set; } // T_DownloadID
		/// <summary>排序号</summary>	
		[Description("排序号")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>已发布</summary>	
		[Description("已发布")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>文件编号</summary>	
		[Description("文件编号")]
        public string FileCode { get; set; } // FileCode
		/// <summary>文件名称</summary>	
		[Description("文件名称")]
        public string FileName { get; set; } // FileName
		/// <summary>FileID</summary>	
		[Description("FileID")]
        public string FileID { get; set; } // FileID
		/// <summary>CarInfoID</summary>	
		[Description("CarInfoID")]
        public string CarInfoID { get; set; } // CarInfoID
		/// <summary>ConfigID</summary>	
		[Description("ConfigID")]
        public string ConfigID { get; set; } // ConfigID
		/// <summary>日期</summary>	
		[Description("日期")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>Attachments</summary>	
		[Description("Attachments")]
        public string Attachments { get; set; } // Attachments
		/// <summary>NodeID</summary>	
		[Description("NodeID")]
        public string NodeID { get; set; } // NodeID
		/// <summary>类型</summary>	
		[Description("类型")]
        public string DataType { get; set; } // DataType
		/// <summary>SpaceID</summary>	
		[Description("SpaceID")]
        public string SpaceID { get; set; } // SpaceID

        // Foreign keys
		[JsonIgnore]
        public virtual T_Download T_Download { get; set; } //  T_DownloadID - FK_T_Download_FileInfo_T_Download
    }


    // ************************************************************************
    // POCO Configuration

    // S_ArchiveCache
    internal partial class S_ArchiveCacheConfiguration : EntityTypeConfiguration<S_ArchiveCache>
    {
        public S_ArchiveCacheConfiguration()
        {
            ToTable("S_ARCHIVECACHE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsRequired().HasMaxLength(50);
            Property(x => x.FileType).HasColumnName("FILETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.MainFile).HasColumnName("MAINFILE").IsOptional().HasMaxLength(500);
            Property(x => x.PdfFile).HasColumnName("PDFFILE").IsOptional().HasMaxLength(500);
            Property(x => x.PrintFile).HasColumnName("PRINTFILE").IsOptional().HasMaxLength(500);
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional();
            Property(x => x.Data).HasColumnName("DATA").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.ArchiveDate).HasColumnName("ARCHIVEDATE").IsRequired();
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.CatalogID).HasColumnName("CATALOGID").IsRequired().HasMaxLength(50);
            Property(x => x.FullCatalogID).HasColumnName("FULLCATALOGID").IsRequired().HasMaxLength(500);
            Property(x => x.SN).HasColumnName("SN").IsOptional().HasMaxLength(50);
            Property(x => x.SNFlag).HasColumnName("SNFLAG").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_ArchiveCacheCatalog).WithMany(b => b.S_ArchiveCache).HasForeignKey(c => c.CatalogID); // FK_S_ArchiveCache_S_ArchiveCacheCatalog
        }
    }

    // S_ArchiveCacheCatalog
    internal partial class S_ArchiveCacheCatalogConfiguration : EntityTypeConfiguration<S_ArchiveCacheCatalog>
    {
        public S_ArchiveCacheCatalogConfiguration()
        {
            ToTable("S_ARCHIVECACHECATALOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsRequired().HasMaxLength(500);
            Property(x => x.Data).HasColumnName("DATA").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.RelateID).HasColumnName("RELATEID").IsOptional().HasMaxLength(50);
            Property(x => x.ArchiveFormID).HasColumnName("ARCHIVEFORMID").IsOptional().HasMaxLength(50);
        }
    }

    // S_BorrowDetail
    internal partial class S_BorrowDetailConfiguration : EntityTypeConfiguration<S_BorrowDetail>
    {
        public S_BorrowDetailConfiguration()
        {
            ToTable("S_BORROWDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.BorrowID).HasColumnName("BORROWID").IsRequired().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.DetailType).HasColumnName("DETAILTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.RelateID).HasColumnName("RELATEID").IsRequired().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.ReturnUserID).HasColumnName("RETURNUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ReturnUser).HasColumnName("RETURNUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ReturnDate).HasColumnName("RETURNDATE").IsOptional();
            Property(x => x.LendDate).HasColumnName("LENDDATE").IsOptional();
            Property(x => x.LendUserID).HasColumnName("LENDUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.LendUserName).HasColumnName("LENDUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.BorrowState).HasColumnName("BORROWSTATE").IsRequired().HasMaxLength(50);
            Property(x => x.BorrowExpireDate).HasColumnName("BORROWEXPIREDATE").IsOptional();
        }
    }

    // S_CarInfo
    internal partial class S_CarInfoConfiguration : EntityTypeConfiguration<S_CarInfo>
    {
        public S_CarInfoConfiguration()
        {
            ToTable("S_CARINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.DataType).HasColumnName("DATATYPE").IsOptional().HasMaxLength(500);
            Property(x => x.Type).HasColumnName("TYPE").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(500);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(500);
            Property(x => x.NodeName).HasColumnName("NODENAME").IsOptional().HasMaxLength(500);
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsOptional().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(50);
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional();
        }
    }

    // S_DocumentLog
    internal partial class S_DocumentLogConfiguration : EntityTypeConfiguration<S_DocumentLog>
    {
        public S_DocumentLogConfiguration()
        {
            ToTable("S_DOCUMENTLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(50);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsOptional().HasMaxLength(50);
            Property(x => x.FullNodeID).HasColumnName("FULLNODEID").IsOptional().HasMaxLength(500);
            Property(x => x.LogType).HasColumnName("LOGTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
        }
    }

    // S_DownloadDetail
    internal partial class S_DownloadDetailConfiguration : EntityTypeConfiguration<S_DownloadDetail>
    {
        public S_DownloadDetailConfiguration()
        {
            ToTable("S_DOWNLOADDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DownloadID).HasColumnName("DOWNLOADID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsRequired().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(500);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsRequired().HasMaxLength(50);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsRequired().HasMaxLength(50);
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsRequired();
            Property(x => x.UserDeptID).HasColumnName("USERDEPTID").IsRequired().HasMaxLength(500);
            Property(x => x.UserDeptName).HasColumnName("USERDEPTNAME").IsRequired().HasMaxLength(500);
            Property(x => x.PassDate).HasColumnName("PASSDATE").IsOptional();
            Property(x => x.DownloadExpireDate).HasColumnName("DOWNLOADEXPIREDATE").IsOptional();
            Property(x => x.DownloadState).HasColumnName("DOWNLOADSTATE").IsRequired().HasMaxLength(50);
        }
    }

    // S_UserAdvanceQueryInfo
    internal partial class S_UserAdvanceQueryInfoConfiguration : EntityTypeConfiguration<S_UserAdvanceQueryInfo>
    {
        public S_UserAdvanceQueryInfoConfiguration()
        {
            ToTable("S_USERADVANCEQUERYINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.QueryData).HasColumnName("QUERYDATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
        }
    }

    // T_Borrow
    internal partial class T_BorrowConfiguration : EntityTypeConfiguration<T_Borrow>
    {
        public T_BorrowConfiguration()
        {
            ToTable("T_BORROW");
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
            Property(x => x.UserDept).HasColumnName("USERDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.UserDeptName).HasColumnName("USERDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.LendDate).HasColumnName("LENDDATE").IsOptional();
            Property(x => x.ReturnDate).HasColumnName("RETURNDATE").IsOptional();
            Property(x => x.Purpose).HasColumnName("PURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.Phone).HasColumnName("PHONE").IsOptional().HasMaxLength(200);
            Property(x => x.PrjPrincipleSign).HasColumnName("PRJPRINCIPLESIGN").IsOptional();
            Property(x => x.DirecotorSign).HasColumnName("DIRECOTORSIGN").IsOptional();
            Property(x => x.LeaderSign).HasColumnName("LEADERSIGN").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.PassDate).HasColumnName("PASSDATE").IsOptional();
            Property(x => x.FlowID).HasColumnName("FLOWID").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowState).HasColumnName("BORROWSTATE").IsOptional().HasMaxLength(200);
            Property(x => x.PurposeDetail).HasColumnName("PURPOSEDETAIL").IsOptional().HasMaxLength(200);
            Property(x => x.PrjPrincipleID).HasColumnName("PRJPRINCIPLEID").IsOptional().HasMaxLength(200);
            Property(x => x.PrjPrincipleName).HasColumnName("PRJPRINCIPLENAME").IsOptional().HasMaxLength(200);
            Property(x => x.PrjPrincipleDate).HasColumnName("PRJPRINCIPLEDATE").IsOptional().HasMaxLength(200);
            Property(x => x.DirecotorID).HasColumnName("DIRECOTORID").IsOptional().HasMaxLength(200);
            Property(x => x.DirecotorName).HasColumnName("DIRECOTORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DirecotorDate).HasColumnName("DIRECOTORDATE").IsOptional().HasMaxLength(200);
            Property(x => x.LeaderID).HasColumnName("LEADERID").IsOptional().HasMaxLength(200);
            Property(x => x.LeaderName).HasColumnName("LEADERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.LeaderDate).HasColumnName("LEADERDATE").IsOptional().HasMaxLength(200);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(200);
        }
    }

    // T_Borrow_FileInfo
    internal partial class T_Borrow_FileInfoConfiguration : EntityTypeConfiguration<T_Borrow_FileInfo>
    {
        public T_Borrow_FileInfoConfiguration()
        {
            ToTable("T_BORROW_FILEINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_BorrowID).HasColumnName("T_BORROWID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.FileCode).HasColumnName("FILECODE").IsOptional().HasMaxLength(200);
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(200);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(200);
            Property(x => x.CarInfoID).HasColumnName("CARINFOID").IsOptional().HasMaxLength(200);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsOptional().HasMaxLength(200);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(200);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_Borrow).WithMany(b => b.T_Borrow_FileInfo).HasForeignKey(c => c.T_BorrowID); // FK_T_Borrow_FileInfo_T_Borrow
        }
    }

    // T_Download
    internal partial class T_DownloadConfiguration : EntityTypeConfiguration<T_Download>
    {
        public T_DownloadConfiguration()
        {
            ToTable("T_DOWNLOAD");
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
            Property(x => x.UserDept).HasColumnName("USERDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.UserDeptName).HasColumnName("USERDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Purpose).HasColumnName("PURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.PassDate).HasColumnName("PASSDATE").IsOptional();
            Property(x => x.DownloadState).HasColumnName("DOWNLOADSTATE").IsOptional().HasMaxLength(200);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(200);
            Property(x => x.FlowID).HasColumnName("FLOWID").IsOptional().HasMaxLength(200);
        }
    }

    // T_Download_FileInfo
    internal partial class T_Download_FileInfoConfiguration : EntityTypeConfiguration<T_Download_FileInfo>
    {
        public T_Download_FileInfoConfiguration()
        {
            ToTable("T_DOWNLOAD_FILEINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_DownloadID).HasColumnName("T_DOWNLOADID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.FileCode).HasColumnName("FILECODE").IsOptional().HasMaxLength(200);
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(200);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(200);
            Property(x => x.CarInfoID).HasColumnName("CARINFOID").IsOptional().HasMaxLength(200);
            Property(x => x.ConfigID).HasColumnName("CONFIGID").IsOptional().HasMaxLength(200);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional().HasMaxLength(200);
            Property(x => x.NodeID).HasColumnName("NODEID").IsOptional().HasMaxLength(200);
            Property(x => x.DataType).HasColumnName("DATATYPE").IsOptional().HasMaxLength(200);
            Property(x => x.SpaceID).HasColumnName("SPACEID").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_Download).WithMany(b => b.T_Download_FileInfo).HasForeignKey(c => c.T_DownloadID); // FK_T_Download_FileInfo_T_Download
        }
    }

}

