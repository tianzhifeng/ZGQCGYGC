

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "Comprehensive"
//     Connection String:      "data source=10.10.1.235\sql2008;Initial Catalog=EPC_Comprehensive;User ID=sa;PWD=123.zxc;"

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

namespace Comprehensive.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class ComprehensiveEntities : Formula.FormulaDbContext
    {
        public IDbSet<R_G_GoodsReport> R_G_GoodsReport { get; set; } // R_G_GoodsReport
        public IDbSet<R_G_GoodsReport_ApplyQuantity> R_G_GoodsReport_ApplyQuantity { get; set; } // R_G_GoodsReport_ApplyQuantity
        public IDbSet<S_A_AttendanceInfo> S_A_AttendanceInfo { get; set; } // S_A_AttendanceInfo
        public IDbSet<S_C_Certificate> S_C_Certificate { get; set; } // S_C_Certificate
        public IDbSet<S_C_Certificate_ApplyLog> S_C_Certificate_ApplyLog { get; set; } // S_C_Certificate_ApplyLog
        public IDbSet<S_G_GoodsAdditional> S_G_GoodsAdditional { get; set; } // S_G_GoodsAdditional
        public IDbSet<S_G_GoodsInfo> S_G_GoodsInfo { get; set; } // S_G_GoodsInfo
        public IDbSet<S_HR_Employee> S_HR_Employee { get; set; } // S_HR_Employee
        public IDbSet<S_HR_EmployeeAcademicDegree> S_HR_EmployeeAcademicDegree { get; set; } // S_HR_EmployeeAcademicDegree
        public IDbSet<S_HR_EmployeeAcademicTitle> S_HR_EmployeeAcademicTitle { get; set; } // S_HR_EmployeeAcademicTitle
        public IDbSet<S_HR_EmployeeContract> S_HR_EmployeeContract { get; set; } // S_HR_EmployeeContract
        public IDbSet<S_HR_EmployeeHonour> S_HR_EmployeeHonour { get; set; } // S_HR_EmployeeHonour
        public IDbSet<S_HR_EmployeeJob> S_HR_EmployeeJob { get; set; } // S_HR_EmployeeJob
        public IDbSet<S_HR_EmployeeQualification> S_HR_EmployeeQualification { get; set; } // S_HR_EmployeeQualification
        public IDbSet<S_HR_EmployeeRetired> S_HR_EmployeeRetired { get; set; } // S_HR_EmployeeRetired
        public IDbSet<S_HR_EmployeeWorkHistory> S_HR_EmployeeWorkHistory { get; set; } // S_HR_EmployeeWorkHistory
        public IDbSet<S_HR_EmployeeWorkPerformance> S_HR_EmployeeWorkPerformance { get; set; } // S_HR_EmployeeWorkPerformance
        public IDbSet<S_HR_EmployeeWorkPost> S_HR_EmployeeWorkPost { get; set; } // S_HR_EmployeeWorkPost
        public IDbSet<S_HR_UserCostInfo> S_HR_UserCostInfo { get; set; } // S_HR_UserCostInfo
        public IDbSet<S_HR_UserUnitPrice> S_HR_UserUnitPrice { get; set; } // S_HR_UserUnitPrice
        public IDbSet<S_I_InstrumentInfo> S_I_InstrumentInfo { get; set; } // S_I_InstrumentInfo
        public IDbSet<S_M_ConferenceRoom> S_M_ConferenceRoom { get; set; } // S_M_ConferenceRoom
        public IDbSet<S_SealManage_SealInfo> S_SealManage_SealInfo { get; set; } // S_SealManage_SealInfo
        public IDbSet<S_W_ProjectInfo> S_W_ProjectInfo { get; set; } // S_W_ProjectInfo
        public IDbSet<S_W_UserWorkHour> S_W_UserWorkHour { get; set; } // S_W_UserWorkHour
        public IDbSet<S_W_UserWorkHour_ApproveDetail> S_W_UserWorkHour_ApproveDetail { get; set; } // S_W_UserWorkHour_ApproveDetail
        public IDbSet<S_W_UserWorkHourSupplement> S_W_UserWorkHourSupplement { get; set; } // S_W_UserWorkHourSupplement
        public IDbSet<T_C_CertificateBorrow> T_C_CertificateBorrow { get; set; } // T_C_CertificateBorrow
        public IDbSet<T_C_CertificateBorrow_ApplyContent> T_C_CertificateBorrow_ApplyContent { get; set; } // T_C_CertificateBorrow_ApplyContent
        public IDbSet<T_ceshishuangyu> T_ceshishuangyu { get; set; } // T_ceshishuangyu
        public IDbSet<T_ceshishuangyu_uuuuu> T_ceshishuangyu_uuuuu { get; set; } // T_ceshishuangyu_uuuuu
        public IDbSet<T_D_DeptBudget> T_D_DeptBudget { get; set; } // T_D_DeptBudget
        public IDbSet<T_D_DeptBudgetUp> T_D_DeptBudgetUp { get; set; } // T_D_DeptBudgetUp
        public IDbSet<T_EmployeePersonalRecords> T_EmployeePersonalRecords { get; set; } // T_EmployeePersonalRecords
        public IDbSet<T_EmployeeSocialSecurity> T_EmployeeSocialSecurity { get; set; } // T_EmployeeSocialSecurity
        public IDbSet<T_Evection_EvectionApply> T_Evection_EvectionApply { get; set; } // T_Evection_EvectionApply
        public IDbSet<T_Evection_EvectionApply_IfAircraft> T_Evection_EvectionApply_IfAircraft { get; set; } // T_Evection_EvectionApply_IfAircraft
        public IDbSet<T_Evection_EvectionApply_Schedule> T_Evection_EvectionApply_Schedule { get; set; } // T_Evection_EvectionApply_Schedule
        public IDbSet<T_Evection_PettyCash> T_Evection_PettyCash { get; set; } // T_Evection_PettyCash
        public IDbSet<T_G_GoodsApply> T_G_GoodsApply { get; set; } // T_G_GoodsApply
        public IDbSet<T_G_GoodsApply_ApplyDetail> T_G_GoodsApply_ApplyDetail { get; set; } // T_G_GoodsApply_ApplyDetail
        public IDbSet<T_HR_PersonalBonusInput> T_HR_PersonalBonusInput { get; set; } // T_HR_PersonalBonusInput
        public IDbSet<T_HR_SalaryManage> T_HR_SalaryManage { get; set; } // T_HR_SalaryManage
        public IDbSet<T_HR_SocialRelation> T_HR_SocialRelation { get; set; } // T_HR_SocialRelation
        public IDbSet<T_I_InstrumentBorrow> T_I_InstrumentBorrow { get; set; } // T_I_InstrumentBorrow
        public IDbSet<T_I_InstrumentDiscard> T_I_InstrumentDiscard { get; set; } // T_I_InstrumentDiscard
        public IDbSet<T_I_InstrumentReturn> T_I_InstrumentReturn { get; set; } // T_I_InstrumentReturn
        public IDbSet<T_LeaveManage_LeaveApply> T_LeaveManage_LeaveApply { get; set; } // T_LeaveManage_LeaveApply
        public IDbSet<T_M_ConferenceApply> T_M_ConferenceApply { get; set; } // T_M_ConferenceApply
        public IDbSet<T_M_ConferenceApply_Budget> T_M_ConferenceApply_Budget { get; set; } // T_M_ConferenceApply_Budget
        public IDbSet<T_M_ConferenceRegist> T_M_ConferenceRegist { get; set; } // T_M_ConferenceRegist
        public IDbSet<T_M_ConferenceRegist_Settlement> T_M_ConferenceRegist_Settlement { get; set; } // T_M_ConferenceRegist_Settlement
        public IDbSet<T_M_ConferenceSummary> T_M_ConferenceSummary { get; set; } // T_M_ConferenceSummary
        public IDbSet<T_M_WeekConference> T_M_WeekConference { get; set; } // T_M_WeekConference
        public IDbSet<T_M_WeekConference_KeyItems> T_M_WeekConference_KeyItems { get; set; } // T_M_WeekConference_KeyItems
        public IDbSet<T_SealManage_SealAbolish> T_SealManage_SealAbolish { get; set; } // T_SealManage_SealAbolish
        public IDbSet<T_SealManage_SealBorrow> T_SealManage_SealBorrow { get; set; } // T_SealManage_SealBorrow
        public IDbSet<T_SealManage_SealEngraveAndChange> T_SealManage_SealEngraveAndChange { get; set; } // T_SealManage_SealEngraveAndChange
        public IDbSet<T_SealManage_SealTurnOver> T_SealManage_SealTurnOver { get; set; } // T_SealManage_SealTurnOver
        public IDbSet<T_SealManage_UseSealApply> T_SealManage_UseSealApply { get; set; } // T_SealManage_UseSealApply
        public IDbSet<T_TrainManage_TrainersManage> T_TrainManage_TrainersManage { get; set; } // T_TrainManage_TrainersManage

        static ComprehensiveEntities()
        {
            Database.SetInitializer<ComprehensiveEntities>(null);
        }

        public ComprehensiveEntities()
            : base("Name=Comprehensive")
        {
        }

        public ComprehensiveEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new R_G_GoodsReportConfiguration());
            modelBuilder.Configurations.Add(new R_G_GoodsReport_ApplyQuantityConfiguration());
            modelBuilder.Configurations.Add(new S_A_AttendanceInfoConfiguration());
            modelBuilder.Configurations.Add(new S_C_CertificateConfiguration());
            modelBuilder.Configurations.Add(new S_C_Certificate_ApplyLogConfiguration());
            modelBuilder.Configurations.Add(new S_G_GoodsAdditionalConfiguration());
            modelBuilder.Configurations.Add(new S_G_GoodsInfoConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeAcademicDegreeConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeAcademicTitleConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeContractConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeHonourConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeJobConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeQualificationConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeRetiredConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeWorkHistoryConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeWorkPerformanceConfiguration());
            modelBuilder.Configurations.Add(new S_HR_EmployeeWorkPostConfiguration());
            modelBuilder.Configurations.Add(new S_HR_UserCostInfoConfiguration());
            modelBuilder.Configurations.Add(new S_HR_UserUnitPriceConfiguration());
            modelBuilder.Configurations.Add(new S_I_InstrumentInfoConfiguration());
            modelBuilder.Configurations.Add(new S_M_ConferenceRoomConfiguration());
            modelBuilder.Configurations.Add(new S_SealManage_SealInfoConfiguration());
            modelBuilder.Configurations.Add(new S_W_ProjectInfoConfiguration());
            modelBuilder.Configurations.Add(new S_W_UserWorkHourConfiguration());
            modelBuilder.Configurations.Add(new S_W_UserWorkHour_ApproveDetailConfiguration());
            modelBuilder.Configurations.Add(new S_W_UserWorkHourSupplementConfiguration());
            modelBuilder.Configurations.Add(new T_C_CertificateBorrowConfiguration());
            modelBuilder.Configurations.Add(new T_C_CertificateBorrow_ApplyContentConfiguration());
            modelBuilder.Configurations.Add(new T_ceshishuangyuConfiguration());
            modelBuilder.Configurations.Add(new T_ceshishuangyu_uuuuuConfiguration());
            modelBuilder.Configurations.Add(new T_D_DeptBudgetConfiguration());
            modelBuilder.Configurations.Add(new T_D_DeptBudgetUpConfiguration());
            modelBuilder.Configurations.Add(new T_EmployeePersonalRecordsConfiguration());
            modelBuilder.Configurations.Add(new T_EmployeeSocialSecurityConfiguration());
            modelBuilder.Configurations.Add(new T_Evection_EvectionApplyConfiguration());
            modelBuilder.Configurations.Add(new T_Evection_EvectionApply_IfAircraftConfiguration());
            modelBuilder.Configurations.Add(new T_Evection_EvectionApply_ScheduleConfiguration());
            modelBuilder.Configurations.Add(new T_Evection_PettyCashConfiguration());
            modelBuilder.Configurations.Add(new T_G_GoodsApplyConfiguration());
            modelBuilder.Configurations.Add(new T_G_GoodsApply_ApplyDetailConfiguration());
            modelBuilder.Configurations.Add(new T_HR_PersonalBonusInputConfiguration());
            modelBuilder.Configurations.Add(new T_HR_SalaryManageConfiguration());
            modelBuilder.Configurations.Add(new T_HR_SocialRelationConfiguration());
            modelBuilder.Configurations.Add(new T_I_InstrumentBorrowConfiguration());
            modelBuilder.Configurations.Add(new T_I_InstrumentDiscardConfiguration());
            modelBuilder.Configurations.Add(new T_I_InstrumentReturnConfiguration());
            modelBuilder.Configurations.Add(new T_LeaveManage_LeaveApplyConfiguration());
            modelBuilder.Configurations.Add(new T_M_ConferenceApplyConfiguration());
            modelBuilder.Configurations.Add(new T_M_ConferenceApply_BudgetConfiguration());
            modelBuilder.Configurations.Add(new T_M_ConferenceRegistConfiguration());
            modelBuilder.Configurations.Add(new T_M_ConferenceRegist_SettlementConfiguration());
            modelBuilder.Configurations.Add(new T_M_ConferenceSummaryConfiguration());
            modelBuilder.Configurations.Add(new T_M_WeekConferenceConfiguration());
            modelBuilder.Configurations.Add(new T_M_WeekConference_KeyItemsConfiguration());
            modelBuilder.Configurations.Add(new T_SealManage_SealAbolishConfiguration());
            modelBuilder.Configurations.Add(new T_SealManage_SealBorrowConfiguration());
            modelBuilder.Configurations.Add(new T_SealManage_SealEngraveAndChangeConfiguration());
            modelBuilder.Configurations.Add(new T_SealManage_SealTurnOverConfiguration());
            modelBuilder.Configurations.Add(new T_SealManage_UseSealApplyConfiguration());
            modelBuilder.Configurations.Add(new T_TrainManage_TrainersManageConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary>功能_行政物品管理_物品基本信息</summary>	
	[Description("功能_行政物品管理_物品基本信息")]
    public partial class R_G_GoodsReport : Formula.BaseModel
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
		/// <summary>办公用品名称</summary>	
		[Description("办公用品名称")]
        public string Name { get; set; } // Name
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>期初数</summary>	
		[Description("期初数")]
        public int? InitialCount { get; set; } // InitialCount
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>库存数</summary>	
		[Description("库存数")]
        public int? StockCount { get; set; } // StockCount
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>物品ID</summary>	
		[Description("物品ID")]
        public string GoodsID { get; set; } // GoodsID
		/// <summary>所属年份</summary>	
		[Description("所属年份")]
        public int? BelongYear { get; set; } // BelongYear
		/// <summary>所属月份</summary>	
		[Description("所属月份")]
        public int? BelongMonth { get; set; } // BelongMonth

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<R_G_GoodsReport_ApplyQuantity> R_G_GoodsReport_ApplyQuantity { get { onR_G_GoodsReport_ApplyQuantityGetting(); return _R_G_GoodsReport_ApplyQuantity;} }
		private ICollection<R_G_GoodsReport_ApplyQuantity> _R_G_GoodsReport_ApplyQuantity;
		partial void onR_G_GoodsReport_ApplyQuantityGetting();


        public R_G_GoodsReport()
        {
            _R_G_GoodsReport_ApplyQuantity = new List<R_G_GoodsReport_ApplyQuantity>();
        }
    }

	/// <summary>领用数量</summary>	
	[Description("领用数量")]
    public partial class R_G_GoodsReport_ApplyQuantity : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string R_G_GoodsReportID { get; set; } // R_G_GoodsReportID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>部门</summary>	
		[Description("部门")]
        public string ApplyDept { get; set; } // ApplyDept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string ApplyDeptName { get; set; } // ApplyDeptName
		/// <summary>领用数量</summary>	
		[Description("领用数量")]
        public int? ApplyQuantity { get; set; } // ApplyQuantity

        // Foreign keys
		[JsonIgnore]
        public virtual R_G_GoodsReport R_G_GoodsReport { get; set; } //  R_G_GoodsReportID - FK_R_G_GoodsReport_ApplyQuantity_R_G_GoodsReport
    }

	/// <summary>功能_考勤管理_考勤信息</summary>	
	[Description("功能_考勤管理_考勤信息")]
    public partial class S_A_AttendanceInfo : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Person { get; set; } // Person
		/// <summary>姓名名称</summary>	
		[Description("姓名名称")]
        public string PersonName { get; set; } // PersonName
		/// <summary>年度</summary>	
		[Description("年度")]
        public int? Year { get; set; } // Year
		/// <summary>月份</summary>	
		[Description("月份")]
        public int? Month { get; set; } // Month
		/// <summary>日期</summary>	
		[Description("日期")]
        public DateTime? Date { get; set; } // Date
		/// <summary>上午</summary>	
		[Description("上午")]
        public string Morning { get; set; } // Morning
		/// <summary>到岗时间</summary>	
		[Description("到岗时间")]
        public DateTime? PostTime { get; set; } // PostTime
		/// <summary>下午</summary>	
		[Description("下午")]
        public string Afternoon { get; set; } // Afternoon
		/// <summary>离岗时间</summary>	
		[Description("离岗时间")]
        public DateTime? LeaveTime { get; set; } // LeaveTime
		/// <summary>考勤时间</summary>	
		[Description("考勤时间")]
        public DateTime? Time { get; set; } // Time
		/// <summary>上午请假类别</summary>	
		[Description("上午请假类别")]
        public string MorningType { get; set; } // MorningType
		/// <summary>下午请假类别</summary>	
		[Description("下午请假类别")]
        public string AfternoonType { get; set; } // AfternoonType
    }

	/// <summary>功能-公司证书管理-企业资质</summary>	
	[Description("功能-公司证书管理-企业资质")]
    public partial class S_C_Certificate : Formula.BaseModel
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
		/// <summary>序号</summary>	
		[Description("序号")]
        public string OrdinalNumber { get; set; } // OrdinalNumber
		/// <summary>证书等级</summary>	
		[Description("证书等级")]
        public string CertificateLevel { get; set; } // CertificateLevel
		/// <summary>证书名称</summary>	
		[Description("证书名称")]
        public string Name { get; set; } // Name
		/// <summary>持证部门</summary>	
		[Description("持证部门")]
        public string HoldDept { get; set; } // HoldDept
		/// <summary>持证部门名称</summary>	
		[Description("持证部门名称")]
        public string HoldDeptName { get; set; } // HoldDeptName
		/// <summary>发证机关</summary>	
		[Description("发证机关")]
        public string IssuingAuthority { get; set; } // IssuingAuthority
		/// <summary>保管单位</summary>	
		[Description("保管单位")]
        public string DepositoryUnit { get; set; } // DepositoryUnit
		/// <summary>证书编号</summary>	
		[Description("证书编号")]
        public string Code { get; set; } // Code
		/// <summary>证书类型</summary>	
		[Description("证书类型")]
        public string CertificateType { get; set; } // CertificateType
		/// <summary>正本数量</summary>	
		[Description("正本数量")]
        public int? OriginalNum { get; set; } // OriginalNum
		/// <summary>副本数量</summary>	
		[Description("副本数量")]
        public int? CopyNum { get; set; } // CopyNum
		/// <summary>发证时间</summary>	
		[Description("发证时间")]
        public DateTime? IssueDate { get; set; } // IssueDate
		/// <summary>年检日期</summary>	
		[Description("年检日期")]
        public DateTime? InspectionDate { get; set; } // InspectionDate
		/// <summary>有效期限(起)</summary>	
		[Description("有效期限(起)")]
        public DateTime? ValidStart { get; set; } // ValidStart
		/// <summary>有效期限(止)</summary>	
		[Description("有效期限(止)")]
        public DateTime? ValidEnd { get; set; } // ValidEnd
		/// <summary>申报日期</summary>	
		[Description("申报日期")]
        public DateTime? DeclareDate { get; set; } // DeclareDate
		/// <summary>年检预警日期</summary>	
		[Description("年检预警日期")]
        public DateTime? WarningDate { get; set; } // WarningDate
		/// <summary>业务范围</summary>	
		[Description("业务范围")]
        public string BusinessRange { get; set; } // BusinessRange
		/// <summary>资质管理办法注册人员配置要求</summary>	
		[Description("资质管理办法注册人员配置要求")]
        public string ConfigureDemand { get; set; } // ConfigureDemand
		/// <summary>正副本扫描附件</summary>	
		[Description("正副本扫描附件")]
        public string Attachment { get; set; } // Attachment
		/// <summary>办理相关说明附件</summary>	
		[Description("办理相关说明附件")]
        public string ExplainAttachment { get; set; } // ExplainAttachment
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>证书状态</summary>	
		[Description("证书状态")]
        public string CertificateState { get; set; } // CertificateState
		/// <summary>借用人</summary>	
		[Description("借用人")]
        public string BorrowUser { get; set; } // BorrowUser
		/// <summary>借用人名称</summary>	
		[Description("借用人名称")]
        public string BorrowUserName { get; set; } // BorrowUserName
    }

	/// <summary>功能-公司证书管理-证书借用记录</summary>	
	[Description("功能-公司证书管理-证书借用记录")]
    public partial class S_C_Certificate_ApplyLog : Formula.BaseModel
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
		/// <summary>企业资质</summary>	
		[Description("企业资质")]
        public string Certificate { get; set; } // Certificate
		/// <summary>企业资质名称</summary>	
		[Description("企业资质名称")]
        public string CertificateName { get; set; } // CertificateName
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>借用人</summary>	
		[Description("借用人")]
        public string BorrowUser { get; set; } // BorrowUser
		/// <summary>借用人名称</summary>	
		[Description("借用人名称")]
        public string BorrowUserName { get; set; } // BorrowUserName
		/// <summary>借用部门</summary>	
		[Description("借用部门")]
        public string BorrowDept { get; set; } // BorrowDept
		/// <summary>借用部门名称</summary>	
		[Description("借用部门名称")]
        public string BorrowDeptName { get; set; } // BorrowDeptName
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>借用日期</summary>	
		[Description("借用日期")]
        public DateTime? BorrowDate { get; set; } // BorrowDate
		/// <summary>计划归还日期</summary>	
		[Description("计划归还日期")]
        public DateTime? PlanReturnDate { get; set; } // PlanReturnDate
		/// <summary>证书用途</summary>	
		[Description("证书用途")]
        public string CertificatePurpose { get; set; } // CertificatePurpose
		/// <summary>是否归还</summary>	
		[Description("是否归还")]
        public string IsReturn { get; set; } // IsReturn
		/// <summary>归还日期</summary>	
		[Description("归还日期")]
        public DateTime? ReturnDate { get; set; } // ReturnDate
		/// <summary>申请单ID</summary>	
		[Description("申请单ID")]
        public string CertificateBorrowID { get; set; } // CertificateBorrowID
    }

	/// <summary>功能_行政物品管理_物品数量追加</summary>	
	[Description("功能_行政物品管理_物品数量追加")]
    public partial class S_G_GoodsAdditional : Formula.BaseModel
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
		/// <summary>行政物品名称</summary>	
		[Description("行政物品名称")]
        public string Goods { get; set; } // Goods
		/// <summary>行政物品名称名称</summary>	
		[Description("行政物品名称名称")]
        public string GoodsName { get; set; } // GoodsName
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>数量</summary>	
		[Description("数量")]
        public int? Quantity { get; set; } // Quantity
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>追加日期</summary>	
		[Description("追加日期")]
        public DateTime? AdditionalData { get; set; } // AdditionalData
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>功能_行政物品管理_物品基本信息</summary>	
	[Description("功能_行政物品管理_物品基本信息")]
    public partial class S_G_GoodsInfo : Formula.BaseModel
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
		/// <summary>行政物品名称</summary>	
		[Description("行政物品名称")]
        public string Name { get; set; } // Name
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>数量</summary>	
		[Description("数量")]
        public int? Quantity { get; set; } // Quantity
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>功能_员工管理_员工基本信息维护</summary>	
	[Description("功能_员工管理_员工基本信息维护")]
    public partial class S_HR_Employee : Formula.BaseModel
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
		/// <summary>员工工号</summary>	
		[Description("员工工号")]
        public string Code { get; set; } // Code
		/// <summary>曾用名</summary>	
		[Description("曾用名")]
        public string OldName { get; set; } // OldName
		/// <summary>员工姓名</summary>	
		[Description("员工姓名")]
        public string Name { get; set; } // Name
		/// <summary>民族</summary>	
		[Description("民族")]
        public string Nation { get; set; } // Nation
		/// <summary>性别</summary>	
		[Description("性别")]
        public string Sex { get; set; } // Sex
		/// <summary>移动电话</summary>	
		[Description("移动电话")]
        public string MobilePhone { get; set; } // MobilePhone
		/// <summary>家庭电话</summary>	
		[Description("家庭电话")]
        public string HomePhone { get; set; } // HomePhone
		/// <summary>办公电话</summary>	
		[Description("办公电话")]
        public string OfficePhone { get; set; } // OfficePhone
		/// <summary>Email</summary>	
		[Description("Email")]
        public string Email { get; set; } // Email
		/// <summary>联系地址</summary>	
		[Description("联系地址")]
        public string Address { get; set; } // Address
		/// <summary>政治面貌</summary>	
		[Description("政治面貌")]
        public string Political { get; set; } // Political
		/// <summary>身份证</summary>	
		[Description("身份证")]
        public string IdentityCardCode { get; set; } // IdentityCardCode
		/// <summary>出生日期</summary>	
		[Description("出生日期")]
        public DateTime? Birthday { get; set; } // Birthday
		/// <summary>籍贯</summary>	
		[Description("籍贯")]
        public string NativePlace { get; set; } // NativePlace
		/// <summary>健康程度</summary>	
		[Description("健康程度")]
        public string HealthStatus { get; set; } // HealthStatus
		/// <summary>婚姻状况</summary>	
		[Description("婚姻状况")]
        public string MaritalStatus { get; set; } // MaritalStatus
		/// <summary>爱人姓名</summary>	
		[Description("爱人姓名")]
        public string LoverName { get; set; } // LoverName
		/// <summary>爱人单位</summary>	
		[Description("爱人单位")]
        public string LoverUnit { get; set; } // LoverUnit
		/// <summary>第一外语</summary>	
		[Description("第一外语")]
        public string FirstForeignLanguage { get; set; } // FirstForeignLanguage
		/// <summary>第一外语程度</summary>	
		[Description("第一外语程度")]
        public string FirstForeignLanguageLevel { get; set; } // FirstForeignLanguageLevel
		/// <summary>第二外语</summary>	
		[Description("第二外语")]
        public string TwoForeignLanguage { get; set; } // TwoForeignLanguage
		/// <summary>第二外语程度</summary>	
		[Description("第二外语程度")]
        public string TwoForeignLanguageLevel { get; set; } // TwoForeignLanguageLevel
		/// <summary>参加工作日期</summary>	
		[Description("参加工作日期")]
        public DateTime? JoinWorkDate { get; set; } // JoinWorkDate
		/// <summary>入司时间</summary>	
		[Description("入司时间")]
        public DateTime? JoinCompanyDate { get; set; } // JoinCompanyDate
		/// <summary>员工来源</summary>	
		[Description("员工来源")]
        public string EmployeeSource { get; set; } // EmployeeSource
		/// <summary>用工形式</summary>	
		[Description("用工形式")]
        public string EmploymentWay { get; set; } // EmploymentWay
		/// <summary>人员类别（大类）</summary>	
		[Description("人员类别（大类）")]
        public string EmployeeBigType { get; set; } // EmployeeBigType
		/// <summary>人员类别（小类）</summary>	
		[Description("人员类别（小类）")]
        public string EmployeeSmallType { get; set; } // EmployeeSmallType
		/// <summary>从事专业</summary>	
		[Description("从事专业")]
        public string EngageMajor { get; set; } // EngageMajor
		/// <summary>当前部门</summary>	
		[Description("当前部门")]
        public string Dept { get; set; } // Dept
		/// <summary>当前部门名称</summary>	
		[Description("当前部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>岗位</summary>	
		[Description("岗位")]
        public string Post { get; set; } // Post
		/// <summary>岗级</summary>	
		[Description("岗级")]
        public string PostLevel { get; set; } // PostLevel
		/// <summary>学历</summary>	
		[Description("学历")]
        public string Educational { get; set; } // Educational
		/// <summary>学历专业</summary>	
		[Description("学历专业")]
        public string EducationalMajor { get; set; } // EducationalMajor
		/// <summary>合同类型</summary>	
		[Description("合同类型")]
        public string ContractType { get; set; } // ContractType
		/// <summary>职称</summary>	
		[Description("职称")]
        public string PositionalTitles { get; set; } // PositionalTitles
		/// <summary>定岗时间</summary>	
		[Description("定岗时间")]
        public DateTime? DeterminePostsDate { get; set; } // DeterminePostsDate
		/// <summary>是否开通系统账号</summary>	
		[Description("是否开通系统账号")]
        public string IsHaveAccount { get; set; } // IsHaveAccount
		/// <summary>系统账号ID</summary>	
		[Description("系统账号ID")]
        public string UserID { get; set; } // UserID
		/// <summary>员工头像</summary>	
		[Description("员工头像")]
        public byte[] Portrait { get; set; } // Portrait
		/// <summary>员工签名</summary>	
		[Description("员工签名")]
        public byte[] SignImage { get; set; } // SignImage
		/// <summary>身份证正面</summary>	
		[Description("身份证正面")]
        public byte[] IdentityCardFace { get; set; } // IdentityCardFace
		/// <summary>身份证反面</summary>	
		[Description("身份证反面")]
        public byte[] IdentityCardBack { get; set; } // IdentityCardBack
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>删除日期</summary>	
		[Description("删除日期")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>员工状态</summary>	
		[Description("员工状态")]
        public string EmployeeState { get; set; } // EmployeeState
		/// <summary>兼职部门</summary>	
		[Description("兼职部门")]
        public string ParttimeDept { get; set; } // ParttimeDept
		/// <summary>兼职部门名称</summary>	
		[Description("兼职部门名称")]
        public string ParttimeDeptName { get; set; } // ParttimeDeptName
		/// <summary>年假天数</summary>	
		[Description("年假天数")]
        public int? AnnualHolidays { get; set; } // AnnualHolidays
    }

	/// <summary>功能_员工管理_学历学位</summary>	
	[Description("功能_员工管理_学历学位")]
    public partial class S_HR_EmployeeAcademicDegree : Formula.BaseModel
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
		/// <summary>员工表ID</summary>	
		[Description("员工表ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>入学时间</summary>	
		[Description("入学时间")]
        public DateTime? EntrancelDate { get; set; } // EntrancelDate
		/// <summary>毕业时间</summary>	
		[Description("毕业时间")]
        public DateTime? GraduationDate { get; set; } // GraduationDate
		/// <summary>毕业学校</summary>	
		[Description("毕业学校")]
        public string School { get; set; } // School
		/// <summary>专业</summary>	
		[Description("专业")]
        public string FirstProfession { get; set; } // FirstProfession
		/// <summary>第二专业</summary>	
		[Description("第二专业")]
        public string TwoProfession { get; set; } // TwoProfession
		/// <summary>学制</summary>	
		[Description("学制")]
        public string SchoolingLength { get; set; } // SchoolingLength
		/// <summary>学习形式</summary>	
		[Description("学习形式")]
        public string SchoolShape { get; set; } // SchoolShape
		/// <summary>学位</summary>	
		[Description("学位")]
        public string Degree { get; set; } // Degree
		/// <summary>学历</summary>	
		[Description("学历")]
        public string Education { get; set; } // Education
		/// <summary>学位授予时间</summary>	
		[Description("学位授予时间")]
        public DateTime? DegreeGiveDate { get; set; } // DegreeGiveDate
		/// <summary>学位授予国家</summary>	
		[Description("学位授予国家")]
        public string DegreeGiveCountry { get; set; } // DegreeGiveCountry
		/// <summary>学历证书</summary>	
		[Description("学历证书")]
        public string DegreeAttachment { get; set; } // DegreeAttachment
		/// <summary>学位证书</summary>	
		[Description("学位证书")]
        public string EducationAttachment { get; set; } // EducationAttachment
		/// <summary>是否主责</summary>	
		[Description("是否主责")]
        public string IsMain { get; set; } // IsMain
    }

	/// <summary>功能_员工管理_职称信息</summary>	
	[Description("功能_员工管理_职称信息")]
    public partial class S_HR_EmployeeAcademicTitle : Formula.BaseModel
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
		/// <summary>职称级别</summary>	
		[Description("职称级别")]
        public string Level { get; set; } // Level
		/// <summary>职称</summary>	
		[Description("职称")]
        public string Title { get; set; } // Title
		/// <summary>专业</summary>	
		[Description("专业")]
        public string Major { get; set; } // Major
		/// <summary>审批部门</summary>	
		[Description("审批部门")]
        public string AuditDept { get; set; } // AuditDept
		/// <summary>证书编号</summary>	
		[Description("证书编号")]
        public string CertificateNumber { get; set; } // CertificateNumber
		/// <summary>发证日期</summary>	
		[Description("发证日期")]
        public DateTime? IssueDate { get; set; } // IssueDate
		/// <summary>聘用时间</summary>	
		[Description("聘用时间")]
        public DateTime? EmployDate { get; set; } // EmployDate
		/// <summary>证书</summary>	
		[Description("证书")]
        public string Certificate { get; set; } // Certificate
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>员工id</summary>	
		[Description("员工id")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>是否主要</summary>	
		[Description("是否主要")]
        public string IsMain { get; set; } // IsMain
    }

	/// <summary>功能_员工管理_合同管理</summary>	
	[Description("功能_员工管理_合同管理")]
    public partial class S_HR_EmployeeContract : Formula.BaseModel
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
		/// <summary>员工id</summary>	
		[Description("员工id")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>合同编号</summary>	
		[Description("合同编号")]
        public string Code { get; set; } // Code
		/// <summary>乙方</summary>	
		[Description("乙方")]
        public string EmployeeName { get; set; } // EmployeeName
		/// <summary>合同类别</summary>	
		[Description("合同类别")]
        public string ContractCategory { get; set; } // ContractCategory
		/// <summary>合同形式</summary>	
		[Description("合同形式")]
        public string ContractShape { get; set; } // ContractShape
		/// <summary>合同主体</summary>	
		[Description("合同主体")]
        public string ContractBody { get; set; } // ContractBody
		/// <summary>合同开始日期</summary>	
		[Description("合同开始日期")]
        public DateTime? ContractStartDate { get; set; } // ContractStartDate
		/// <summary>合同结束日期</summary>	
		[Description("合同结束日期")]
        public DateTime? ContractEndDate { get; set; } // ContractEndDate
		/// <summary>试用期开始日期</summary>	
		[Description("试用期开始日期")]
        public DateTime? PeriodStartDate { get; set; } // PeriodStartDate
		/// <summary>试用结束日期</summary>	
		[Description("试用结束日期")]
        public DateTime? PeriodEndDate { get; set; } // PeriodEndDate
		/// <summary>实习期满时间</summary>	
		[Description("实习期满时间")]
        public DateTime? PracticeEndDate { get; set; } // PracticeEndDate
		/// <summary>合同期限</summary>	
		[Description("合同期限")]
        public string ContractPeriod { get; set; } // ContractPeriod
		/// <summary>定岗时间</summary>	
		[Description("定岗时间")]
        public DateTime? PostDate { get; set; } // PostDate
		/// <summary>合同及附件</summary>	
		[Description("合同及附件")]
        public string ContractAttachment { get; set; } // ContractAttachment
		/// <summary>是否签订保密协议</summary>	
		[Description("是否签订保密协议")]
        public string IsConfidentialityAgreement { get; set; } // IsConfidentialityAgreement
		/// <summary>保密协议开始时间</summary>	
		[Description("保密协议开始时间")]
        public DateTime? ConfidentialityAgreementStartDate { get; set; } // ConfidentialityAgreementStartDate
		/// <summary>保密协议结束时间</summary>	
		[Description("保密协议结束时间")]
        public DateTime? ConfidentialityAgreementEndDate { get; set; } // ConfidentialityAgreementEndDate
		/// <summary>保密协议附件</summary>	
		[Description("保密协议附件")]
        public string ConfidentialityAttachment { get; set; } // ConfidentialityAttachment
		/// <summary>教育培训协议开始时间</summary>	
		[Description("教育培训协议开始时间")]
        public DateTime? TrainAgreementStartDate { get; set; } // TrainAgreementStartDate
		/// <summary>教育培训结束时间</summary>	
		[Description("教育培训结束时间")]
        public DateTime? TrainAgreementEndDate { get; set; } // TrainAgreementEndDate
		/// <summary>教育培训附件</summary>	
		[Description("教育培训附件")]
        public string TrainAttachment { get; set; } // TrainAttachment
		/// <summary>股权协议开始时间</summary>	
		[Description("股权协议开始时间")]
        public DateTime? StockAgreementStartDate { get; set; } // StockAgreementStartDate
		/// <summary>股权协议结束时间</summary>	
		[Description("股权协议结束时间")]
        public DateTime? StockAgreementEndDate { get; set; } // StockAgreementEndDate
		/// <summary>股权附件</summary>	
		[Description("股权附件")]
        public string StockAttachment { get; set; } // StockAttachment
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>是否签订竞业协议</summary>	
		[Description("是否签订竞业协议")]
        public string SFQDJYXY { get; set; } // SFQDJYXY
		/// <summary>竞业协议开始时间</summary>	
		[Description("竞业协议开始时间")]
        public DateTime? JYXYKSSJ { get; set; } // JYXYKSSJ
		/// <summary>竞业协议结束时间</summary>	
		[Description("竞业协议结束时间")]
        public DateTime? JYXYJSSJ { get; set; } // JYXYJSSJ
		/// <summary>竞业金</summary>	
		[Description("竞业金")]
        public string JYJ { get; set; } // JYJ
		/// <summary>竞业协议附件</summary>	
		[Description("竞业协议附件")]
        public string JYXYFJ { get; set; } // JYXYFJ
    }

	/// <summary>功能_员工管理_个人荣誉</summary>	
	[Description("功能_员工管理_个人荣誉")]
    public partial class S_HR_EmployeeHonour : Formula.BaseModel
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
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>获奖名称</summary>	
		[Description("获奖名称")]
        public string AwardName { get; set; } // AwardName
		/// <summary>获奖年份</summary>	
		[Description("获奖年份")]
        public string AwardYear { get; set; } // AwardYear
		/// <summary>发证日期</summary>	
		[Description("发证日期")]
        public DateTime? CertificationDate { get; set; } // CertificationDate
		/// <summary>发证单位</summary>	
		[Description("发证单位")]
        public string CertificationUnit { get; set; } // CertificationUnit
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Attachment { get; set; } // Attachment
    }

	/// <summary>功能_员工管理_职务信息</summary>	
	[Description("功能_员工管理_职务信息")]
    public partial class S_HR_EmployeeJob : Formula.BaseModel
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
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>是否删除（0未删除，1删除）</summary>	
		[Description("是否删除（0未删除，1删除）")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>人员类别</summary>	
		[Description("人员类别")]
        public string EmployeeCategory { get; set; } // EmployeeCategory
		/// <summary>职务类型</summary>	
		[Description("职务类型")]
        public string JobCategory { get; set; } // JobCategory
		/// <summary>集团</summary>	
		[Description("集团")]
        public string Clique { get; set; } // Clique
		/// <summary>子公司</summary>	
		[Description("子公司")]
        public string SubCompany { get; set; } // SubCompany
		/// <summary>部门</summary>	
		[Description("部门")]
        public string DeptID { get; set; } // DeptID
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptIDName { get; set; } // DeptIDName
		/// <summary>职务</summary>	
		[Description("职务")]
        public string JobID { get; set; } // JobID
		/// <summary>职务名称</summary>	
		[Description("职务名称")]
        public string JobName { get; set; } // JobName
		/// <summary>是否主责</summary>	
		[Description("是否主责")]
        public string IsMain { get; set; } // IsMain
		/// <summary>专业</summary>	
		[Description("专业")]
        public string Major { get; set; } // Major
		/// <summary>聘任时间</summary>	
		[Description("聘任时间")]
        public DateTime? EmployDate { get; set; } // EmployDate
		/// <summary>职批准文号</summary>	
		[Description("职批准文号")]
        public string JobAgreeCode { get; set; } // JobAgreeCode
		/// <summary>聘任批准文号</summary>	
		[Description("聘任批准文号")]
        public string EmployAgreeCode { get; set; } // EmployAgreeCode
		/// <summary>任满时间</summary>	
		[Description("任满时间")]
        public DateTime? EmployEndDate { get; set; } // EmployEndDate
		/// <summary>免职日期</summary>	
		[Description("免职日期")]
        public DateTime? ClearEmployDate { get; set; } // ClearEmployDate
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Attachment { get; set; } // Attachment
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>功能_员工管理_执业资格</summary>	
	[Description("功能_员工管理_执业资格")]
    public partial class S_HR_EmployeeQualification : Formula.BaseModel
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
		/// <summary>登记人</summary>	
		[Description("登记人")]
        public string RegisterID { get; set; } // RegisterID
		/// <summary>登记人名称</summary>	
		[Description("登记人名称")]
        public string RegisterIDName { get; set; } // RegisterIDName
		/// <summary>登记日期</summary>	
		[Description("登记日期")]
        public DateTime? RegisteDate { get; set; } // RegisteDate
		/// <summary>执业资格名称</summary>	
		[Description("执业资格名称")]
        public string QualificationName { get; set; } // QualificationName
		/// <summary>执业资格证书编号</summary>	
		[Description("执业资格证书编号")]
        public string QualificationCode { get; set; } // QualificationCode
		/// <summary>第一专业</summary>	
		[Description("第一专业")]
        public string FirstMajor { get; set; } // FirstMajor
		/// <summary>第二专业</summary>	
		[Description("第二专业")]
        public string TwoMajor { get; set; } // TwoMajor
		/// <summary>执业资格证书发证时间</summary>	
		[Description("执业资格证书发证时间")]
        public DateTime? QualificationIssueDate { get; set; } // QualificationIssueDate
		/// <summary>执业证书保管人</summary>	
		[Description("执业证书保管人")]
        public string QualificationKeeperID { get; set; } // QualificationKeeperID
		/// <summary>执业证书保管人名称</summary>	
		[Description("执业证书保管人名称")]
        public string QualificationKeeperIDName { get; set; } // QualificationKeeperIDName
		/// <summary>注册证书编号</summary>	
		[Description("注册证书编号")]
        public string RegisteCode { get; set; } // RegisteCode
		/// <summary>注册证发证日期</summary>	
		[Description("注册证发证日期")]
        public DateTime? RegisteIssueDate { get; set; } // RegisteIssueDate
		/// <summary>注册证书失效时间</summary>	
		[Description("注册证书失效时间")]
        public DateTime? RegistelLoseDate { get; set; } // RegistelLoseDate
		/// <summary>注册证书保管人</summary>	
		[Description("注册证书保管人")]
        public string RegisteKeeperID { get; set; } // RegisteKeeperID
		/// <summary>注册证书保管人名称</summary>	
		[Description("注册证书保管人名称")]
        public string RegisteKeeperIDName { get; set; } // RegisteKeeperIDName
		/// <summary>注册印章编号</summary>	
		[Description("注册印章编号")]
        public string SealCode { get; set; } // SealCode
		/// <summary>注册印章失效日期</summary>	
		[Description("注册印章失效日期")]
        public DateTime? SealLoseDate { get; set; } // SealLoseDate
		/// <summary>注册印章保管者</summary>	
		[Description("注册印章保管者")]
        public string SealKeeperID { get; set; } // SealKeeperID
		/// <summary>注册印章保管者名称</summary>	
		[Description("注册印章保管者名称")]
        public string SealKeeperIDName { get; set; } // SealKeeperIDName
		/// <summary>继续教育参加时间</summary>	
		[Description("继续教育参加时间")]
        public DateTime? ContinueTrainDate { get; set; } // ContinueTrainDate
		/// <summary>继续教育学时</summary>	
		[Description("继续教育学时")]
        public double? ContinueTrainLength { get; set; } // ContinueTrainLength
		/// <summary>继续教育已完成学时</summary>	
		[Description("继续教育已完成学时")]
        public double? ContinueTrainCompleteLength { get; set; } // ContinueTrainCompleteLength
		/// <summary>执业资格证书附件</summary>	
		[Description("执业资格证书附件")]
        public string QualificationAttachment { get; set; } // QualificationAttachment
		/// <summary>注册证附件</summary>	
		[Description("注册证附件")]
        public string RegisteAttachment { get; set; } // RegisteAttachment
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
    }

	/// <summary>人员离退</summary>	
	[Description("人员离退")]
    public partial class S_HR_EmployeeRetired : Formula.BaseModel
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
		/// <summary>员工编号</summary>	
		[Description("员工编号")]
        public string EmployeeCode { get; set; } // EmployeeCode
		/// <summary>员工姓名</summary>	
		[Description("员工姓名")]
        public string EmployeeName { get; set; } // EmployeeName
		/// <summary>离退时间</summary>	
		[Description("离退时间")]
        public DateTime? RetiredDate { get; set; } // RetiredDate
		/// <summary>离退类型</summary>	
		[Description("离退类型")]
        public string Type { get; set; } // Type
		/// <summary>离退去向</summary>	
		[Description("离退去向")]
        public string Direction { get; set; } // Direction
		/// <summary>离退原因</summary>	
		[Description("离退原因")]
        public string Reason { get; set; } // Reason
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
    }

	/// <summary>功能_员工管理_工作经历</summary>	
	[Description("功能_员工管理_工作经历")]
    public partial class S_HR_EmployeeWorkHistory : Formula.BaseModel
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
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>入职时间</summary>	
		[Description("入职时间")]
        public DateTime? JoinCompanyDate { get; set; } // JoinCompanyDate
		/// <summary>离职时间</summary>	
		[Description("离职时间")]
        public DateTime? LeaveCompanyDate { get; set; } // LeaveCompanyDate
		/// <summary>公司名称</summary>	
		[Description("公司名称")]
        public string CompanyName { get; set; } // CompanyName
		/// <summary>部门和职务</summary>	
		[Description("部门和职务")]
        public string DeptAndPost { get; set; } // DeptAndPost
		/// <summary>职责描述</summary>	
		[Description("职责描述")]
        public string Description { get; set; } // Description
		/// <summary>业绩</summary>	
		[Description("业绩")]
        public string Achievement { get; set; } // Achievement
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Attachment { get; set; } // Attachment
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>功能_员工管理_工作业绩</summary>	
	[Description("功能_员工管理_工作业绩")]
    public partial class S_HR_EmployeeWorkPerformance : Formula.BaseModel
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
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>项目编号</summary>	
		[Description("项目编号")]
        public string Code { get; set; } // Code
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string Name { get; set; } // Name
		/// <summary>项目类型</summary>	
		[Description("项目类型")]
        public string ProjectClass { get; set; } // ProjectClass
		/// <summary>项目规模</summary>	
		[Description("项目规模")]
        public string ProjectLevel { get; set; } // ProjectLevel
		/// <summary>生产项目状态</summary>	
		[Description("生产项目状态")]
        public string ProjectState { get; set; } // ProjectState
		/// <summary>项目开始日期</summary>	
		[Description("项目开始日期")]
        public DateTime? PlanStartDate { get; set; } // PlanStartDate
		/// <summary>项目结束日期</summary>	
		[Description("项目结束日期")]
        public DateTime? FactFinishDate { get; set; } // FactFinishDate
		/// <summary>项目范围及任务描述</summary>	
		[Description("项目范围及任务描述")]
        public string ProjectDescription { get; set; } // ProjectDescription
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Attachment { get; set; } // Attachment
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>关联业绩ID</summary>	
		[Description("关联业绩ID")]
        public string RelateID { get; set; } // RelateID
		/// <summary>担任项目角色</summary>	
		[Description("担任项目角色")]
        public string ProjectRole { get; set; } // ProjectRole
    }

	/// <summary>功能_员工管理_岗位/岗级</summary>	
	[Description("功能_员工管理_岗位/岗级")]
    public partial class S_HR_EmployeeWorkPost : Formula.BaseModel
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
		/// <summary>岗位</summary>	
		[Description("岗位")]
        public string Post { get; set; } // Post
		/// <summary>岗位名称</summary>	
		[Description("岗位名称")]
        public string PostName { get; set; } // PostName
		/// <summary>岗级</summary>	
		[Description("岗级")]
        public string PostLevel { get; set; } // PostLevel
		/// <summary>生效时间</summary>	
		[Description("生效时间")]
        public DateTime? EffectiveDate { get; set; } // EffectiveDate
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>是否主要</summary>	
		[Description("是否主要")]
        public string IsMain { get; set; } // IsMain
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_HR_UserCostInfo : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public DateTime StartDate { get; set; } // StartDate
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
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_HR_UserUnitPrice> S_HR_UserUnitPrice { get { onS_HR_UserUnitPriceGetting(); return _S_HR_UserUnitPrice;} }
		private ICollection<S_HR_UserUnitPrice> _S_HR_UserUnitPrice;
		partial void onS_HR_UserUnitPriceGetting();


        public S_HR_UserCostInfo()
        {
            _S_HR_UserUnitPrice = new List<S_HR_UserUnitPrice>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_HR_UserUnitPrice : Formula.BaseModel
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
        public string UserLevel { get; set; } // UserLevel
		/// <summary></summary>	
		[Description("")]
        public decimal UnitPrice { get; set; } // UnitPrice
		/// <summary></summary>	
		[Description("")]
        public string CostInfoID { get; set; } // CostInfoID

        // Foreign keys
		[JsonIgnore]
        public virtual S_HR_UserCostInfo S_HR_UserCostInfo { get; set; } //  CostInfoID - FK_S_HR_UserUnitPrice_S_HR_UserCostInfo
    }

	/// <summary>功能_仪器领用管理_仪器基本信息</summary>	
	[Description("功能_仪器领用管理_仪器基本信息")]
    public partial class S_I_InstrumentInfo : Formula.BaseModel
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
		/// <summary>物品名称</summary>	
		[Description("物品名称")]
        public string Name { get; set; } // Name
		/// <summary>物品编号</summary>	
		[Description("物品编号")]
        public string Code { get; set; } // Code
		/// <summary>登记人</summary>	
		[Description("登记人")]
        public string RegisterUser { get; set; } // RegisterUser
		/// <summary>登记人名称</summary>	
		[Description("登记人名称")]
        public string RegisterUserName { get; set; } // RegisterUserName
		/// <summary>登记日期</summary>	
		[Description("登记日期")]
        public DateTime? RegisterDate { get; set; } // RegisterDate
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>规格</summary>	
		[Description("规格")]
        public string Spec { get; set; } // Spec
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>价值</summary>	
		[Description("价值")]
        public decimal? Price { get; set; } // Price
		/// <summary>供应商名称</summary>	
		[Description("供应商名称")]
        public string SupplierName { get; set; } // SupplierName
		/// <summary>入库仓库</summary>	
		[Description("入库仓库")]
        public string Warehousing { get; set; } // Warehousing
		/// <summary>购置日期</summary>	
		[Description("购置日期")]
        public DateTime? PurchaseDate { get; set; } // PurchaseDate
		/// <summary>折旧年限</summary>	
		[Description("折旧年限")]
        public int? DepreciableLives { get; set; } // DepreciableLives
		/// <summary>计划报废日期</summary>	
		[Description("计划报废日期")]
        public DateTime? DisCardDate { get; set; } // DisCardDate
		/// <summary>实物状态</summary>	
		[Description("实物状态")]
        public string PhysicalState { get; set; } // PhysicalState
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>功能-会议管理-会议室基本信息</summary>	
	[Description("功能-会议管理-会议室基本信息")]
    public partial class S_M_ConferenceRoom : Formula.BaseModel
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
		/// <summary>会议室编号</summary>	
		[Description("会议室编号")]
        public string RoomName { get; set; } // RoomName
		/// <summary>联系人</summary>	
		[Description("联系人")]
        public string LinkName { get; set; } // LinkName
		/// <summary>最大使用人数</summary>	
		[Description("最大使用人数")]
        public string Capacity { get; set; } // Capacity
		/// <summary>管理部门</summary>	
		[Description("管理部门")]
        public string ManageDeptID { get; set; } // ManageDeptID
		/// <summary>管理部门名称</summary>	
		[Description("管理部门名称")]
        public string ManageDeptIDName { get; set; } // ManageDeptIDName
		/// <summary>会议室地址</summary>	
		[Description("会议室地址")]
        public string RoomAddress { get; set; } // RoomAddress
		/// <summary>设施情况</summary>	
		[Description("设施情况")]
        public string Configuredevice { get; set; } // Configuredevice
    }

	/// <summary>功能_印章管理_印章基本信息</summary>	
	[Description("功能_印章管理_印章基本信息")]
    public partial class S_SealManage_SealInfo : Formula.BaseModel
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
		/// <summary>开始使用时间</summary>	
		[Description("开始使用时间")]
        public DateTime? BeginUseTime { get; set; } // BeginUseTime
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>图片</summary>	
		[Description("图片")]
        public string Picture { get; set; } // Picture
		/// <summary>印章编号</summary>	
		[Description("印章编号")]
        public string Code { get; set; } // Code
		/// <summary>印章类别</summary>	
		[Description("印章类别")]
        public string Category { get; set; } // Category
		/// <summary>印章保管部门</summary>	
		[Description("印章保管部门")]
        public string KeepDept { get; set; } // KeepDept
		/// <summary>印章保管部门名称</summary>	
		[Description("印章保管部门名称")]
        public string KeepDeptName { get; set; } // KeepDeptName
		/// <summary>印章保管人</summary>	
		[Description("印章保管人")]
        public string Keeper { get; set; } // Keeper
		/// <summary>印章保管人名称</summary>	
		[Description("印章保管人名称")]
        public string KeeperName { get; set; } // KeeperName
		/// <summary>印章状态</summary>	
		[Description("印章状态")]
        public string State { get; set; } // State
		/// <summary>印章用途</summary>	
		[Description("印章用途")]
        public string Purpose { get; set; } // Purpose
		/// <summary>印章名称</summary>	
		[Description("印章名称")]
        public string Name { get; set; } // Name
    }

	/// <summary>功能_工时管理_工时项目维护</summary>	
	[Description("功能_工时管理_工时项目维护")]
    public partial class S_W_ProjectInfo : Formula.BaseModel
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
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>项目编号</summary>	
		[Description("项目编号")]
        public string ProjectCode { get; set; } // ProjectCode
		/// <summary>责任部门</summary>	
		[Description("责任部门")]
        public string ChargerDept { get; set; } // ChargerDept
		/// <summary>责任部门名称</summary>	
		[Description("责任部门名称")]
        public string ChargerDeptName { get; set; } // ChargerDeptName
		/// <summary>项目负责人</summary>	
		[Description("项目负责人")]
        public string ChargerUser { get; set; } // ChargerUser
		/// <summary>项目负责人名称</summary>	
		[Description("项目负责人名称")]
        public string ChargerUserName { get; set; } // ChargerUserName
		/// <summary>分类</summary>	
		[Description("分类")]
        public string WorkHourType { get; set; } // WorkHourType
		/// <summary>是否有效</summary>	
		[Description("是否有效")]
        public string IsValid { get; set; } // IsValid
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
    }

	/// <summary>工时审批增加字段</summary>	
	[Description("工时审批增加字段")]
    public partial class S_W_UserWorkHour : Formula.BaseModel
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
        public string UserCode { get; set; } // UserCode
		/// <summary></summary>	
		[Description("")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary></summary>	
		[Description("")]
        public string UserDeptID { get; set; } // UserDeptID
		/// <summary></summary>	
		[Description("")]
        public string UserDeptName { get; set; } // UserDeptName
		/// <summary></summary>	
		[Description("")]
        public DateTime WorkHourDate { get; set; } // WorkHourDate
		/// <summary></summary>	
		[Description("")]
        public decimal? NormalValue { get; set; } // NormalValue
		/// <summary></summary>	
		[Description("")]
        public decimal? AdditionalValue { get; set; } // AdditionalValue
		/// <summary></summary>	
		[Description("")]
        public decimal? WorkHourValue { get; set; } // WorkHourValue
		/// <summary></summary>	
		[Description("")]
        public decimal? ConfirmValue { get; set; } // ConfirmValue
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public int BelongYear { get; set; } // BelongYear
		/// <summary></summary>	
		[Description("")]
        public int BelongMonth { get; set; } // BelongMonth
		/// <summary></summary>	
		[Description("")]
        public int BelongQuarter { get; set; } // BelongQuarter
		/// <summary></summary>	
		[Description("")]
        public string WorkHourType { get; set; } // WorkHourType
		/// <summary></summary>	
		[Description("")]
        public string ProjectID { get; set; } // ProjectID
		/// <summary></summary>	
		[Description("")]
        public string ProjectCode { get; set; } // ProjectCode
		/// <summary></summary>	
		[Description("")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary></summary>	
		[Description("")]
        public string ProjectDept { get; set; } // ProjectDept
		/// <summary></summary>	
		[Description("")]
        public string ProjectDeptName { get; set; } // ProjectDeptName
		/// <summary></summary>	
		[Description("")]
        public string ProjectChargerUser { get; set; } // ProjectChargerUser
		/// <summary></summary>	
		[Description("")]
        public string ProjectChargerUserName { get; set; } // ProjectChargerUserName
		/// <summary></summary>	
		[Description("")]
        public string SubProjectName { get; set; } // SubProjectName
		/// <summary></summary>	
		[Description("")]
        public string SubProjectCode { get; set; } // SubProjectCode
		/// <summary></summary>	
		[Description("")]
        public string MajorCode { get; set; } // MajorCode
		/// <summary></summary>	
		[Description("")]
        public string MajorName { get; set; } // MajorName
		/// <summary></summary>	
		[Description("")]
        public string TaskWorkCode { get; set; } // TaskWorkCode
		/// <summary></summary>	
		[Description("")]
        public string TaskWorkName { get; set; } // TaskWorkName
		/// <summary></summary>	
		[Description("")]
        public string WorkContent { get; set; } // WorkContent
		/// <summary></summary>	
		[Description("")]
        public decimal? WorkHourDay { get; set; } // WorkHourDay
		/// <summary></summary>	
		[Description("")]
        public decimal? ConfirmDay { get; set; } // ConfirmDay
		/// <summary>第一步审批工时</summary>	
		[Description("第一步审批工时")]
        public decimal? Step1Value { get; set; } // Step1Value
		/// <summary>第一步审批工日</summary>	
		[Description("第一步审批工日")]
        public decimal? Step1Day { get; set; } // Step1Day
		/// <summary>第二步审批工时</summary>	
		[Description("第二步审批工时")]
        public decimal? Step2Value { get; set; } // Step2Value
		/// <summary>第二步审批工日</summary>	
		[Description("第二步审批工日")]
        public decimal? Step2Day { get; set; } // Step2Day
		/// <summary>第一步审批人</summary>	
		[Description("第一步审批人")]
        public string Step1User { get; set; } // Step1User
		/// <summary>第一步审批人名称</summary>	
		[Description("第一步审批人名称")]
        public string Step1UserName { get; set; } // Step1UserName
		/// <summary>第一步审批时间</summary>	
		[Description("第一步审批时间")]
        public DateTime? Step1Date { get; set; } // Step1Date
		/// <summary>第一步是否审批(0、1)</summary>	
		[Description("第一步是否审批(0、1)")]
        public string IsStep1 { get; set; } // IsStep1
		/// <summary>第二步审批人</summary>	
		[Description("第二步审批人")]
        public string Step2User { get; set; } // Step2User
		/// <summary>第二步审批人名称</summary>	
		[Description("第二步审批人名称")]
        public string Step2UserName { get; set; } // Step2UserName
		/// <summary>第二步审批时间</summary>	
		[Description("第二步审批时间")]
        public DateTime? Step2Date { get; set; } // Step2Date
		/// <summary>第二步是否审批(0、1)</summary>	
		[Description("第二步是否审批(0、1)")]
        public string IsStep2 { get; set; } // IsStep2
		/// <summary>最终确认人</summary>	
		[Description("最终确认人")]
        public string ConfirmUser { get; set; } // ConfirmUser
		/// <summary>最终确认人名称</summary>	
		[Description("最终确认人名称")]
        public string ConfirmUserName { get; set; } // ConfirmUserName
		/// <summary>最终确认时间</summary>	
		[Description("最终确认时间")]
        public DateTime? ConfirmDate { get; set; } // ConfirmDate
		/// <summary>是否最终确认(0、1)</summary>	
		[Description("是否最终确认(0、1)")]
        public string IsConfirm { get; set; } // IsConfirm
		/// <summary>CreateUserID</summary>	
		[Description("CreateUserID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>CreateUser</summary>	
		[Description("CreateUser")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>补填记录ID</summary>	
		[Description("补填记录ID")]
        public string SupplementID { get; set; } // SupplementID
		/// <summary></summary>	
		[Description("")]
        public string WorkTimeMajor { get; set; } // WorkTimeMajor

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_W_UserWorkHour_ApproveDetail> S_W_UserWorkHour_ApproveDetail { get { onS_W_UserWorkHour_ApproveDetailGetting(); return _S_W_UserWorkHour_ApproveDetail;} }
		private ICollection<S_W_UserWorkHour_ApproveDetail> _S_W_UserWorkHour_ApproveDetail;
		partial void onS_W_UserWorkHour_ApproveDetailGetting();


        public S_W_UserWorkHour()
        {
            _S_W_UserWorkHour_ApproveDetail = new List<S_W_UserWorkHour_ApproveDetail>();
        }
    }

	/// <summary>审批意见</summary>	
	[Description("审批意见")]
    public partial class S_W_UserWorkHour_ApproveDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string S_W_UserWorkHourID { get; set; } // S_W_UserWorkHourID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>审批工时</summary>	
		[Description("审批工时")]
        public decimal? ApproveValue { get; set; } // ApproveValue
		/// <summary>审批工日</summary>	
		[Description("审批工日")]
        public decimal? ApproveDay { get; set; } // ApproveDay
		/// <summary>审批时间</summary>	
		[Description("审批时间")]
        public DateTime? ApproveDate { get; set; } // ApproveDate
		/// <summary>审批环节</summary>	
		[Description("审批环节")]
        public string ApproveStep { get; set; } // ApproveStep
		/// <summary>审批人</summary>	
		[Description("审批人")]
        public string ApproveUser { get; set; } // ApproveUser
		/// <summary>审批人名称</summary>	
		[Description("审批人名称")]
        public string ApproveUserName { get; set; } // ApproveUserName

        // Foreign keys
		[JsonIgnore]
        public virtual S_W_UserWorkHour S_W_UserWorkHour { get; set; } //  S_W_UserWorkHourID - FK_S_W_UserWorkHour_ApproveDetail_S_W_UserWorkHour
    }

	/// <summary>工时批量补填</summary>	
	[Description("工时批量补填")]
    public partial class S_W_UserWorkHourSupplement : Formula.BaseModel
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
		/// <summary>人员</summary>	
		[Description("人员")]
        public string UserID { get; set; } // UserID
		/// <summary>人员名称</summary>	
		[Description("人员名称")]
        public string UserIDName { get; set; } // UserIDName
		/// <summary>部门</summary>	
		[Description("部门")]
        public string UserDept { get; set; } // UserDept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string UserDeptName { get; set; } // UserDeptName
		/// <summary>项目</summary>	
		[Description("项目")]
        public string ProjectID { get; set; } // ProjectID
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string ProjectIDName { get; set; } // ProjectIDName
		/// <summary>类别</summary>	
		[Description("类别")]
        public string WorkHourType { get; set; } // WorkHourType
		/// <summary>子项</summary>	
		[Description("子项")]
        public string SubProjectCode { get; set; } // SubProjectCode
		/// <summary>专业</summary>	
		[Description("专业")]
        public string MajorCode { get; set; } // MajorCode
		/// <summary>专业名称</summary>	
		[Description("专业名称")]
        public string MajorName { get; set; } // MajorName
		/// <summary>工作内容</summary>	
		[Description("工作内容")]
        public string WorkContent { get; set; } // WorkContent
		/// <summary>工时日期开始</summary>	
		[Description("工时日期开始")]
        public DateTime? WorkHourDateStart { get; set; } // WorkHourDateStart
		/// <summary>工时日期结束</summary>	
		[Description("工时日期结束")]
        public DateTime? WorkHourDateEnd { get; set; } // WorkHourDateEnd
		/// <summary>正班工时</summary>	
		[Description("正班工时")]
        public decimal? NormalValue { get; set; } // NormalValue
		/// <summary>加班工时</summary>	
		[Description("加班工时")]
        public decimal? AdditionalValue { get; set; } // AdditionalValue
		/// <summary>合计</summary>	
		[Description("合计")]
        public decimal? WorkHourValue { get; set; } // WorkHourValue
		/// <summary>UserName</summary>	
		[Description("UserName")]
        public string UserName { get; set; } // UserName
		/// <summary>UserDeptID</summary>	
		[Description("UserDeptID")]
        public string UserDeptID { get; set; } // UserDeptID
		/// <summary>项目编号</summary>	
		[Description("项目编号")]
        public string ProjectCode { get; set; } // ProjectCode
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>项目所属部门</summary>	
		[Description("项目所属部门")]
        public string ProjectDept { get; set; } // ProjectDept
		/// <summary>项目所属部门名称</summary>	
		[Description("项目所属部门名称")]
        public string ProjectDeptName { get; set; } // ProjectDeptName
		/// <summary>项目负责人</summary>	
		[Description("项目负责人")]
        public string ProjectChargerUser { get; set; } // ProjectChargerUser
		/// <summary>项目负责人名称</summary>	
		[Description("项目负责人名称")]
        public string ProjectChargerUserName { get; set; } // ProjectChargerUserName
		/// <summary>子项名称</summary>	
		[Description("子项名称")]
        public string SubProjectName { get; set; } // SubProjectName
		/// <summary>工作包编号</summary>	
		[Description("工作包编号")]
        public string TaskWorkCode { get; set; } // TaskWorkCode
		/// <summary>工作包名称</summary>	
		[Description("工作包名称")]
        public string TaskWorkName { get; set; } // TaskWorkName
		/// <summary>专业</summary>	
		[Description("专业")]
        public string WorkTimeMajor { get; set; } // WorkTimeMajor
    }

	/// <summary>流程-公司证书管理-证书借用管理</summary>	
	[Description("流程-公司证书管理-证书借用管理")]
    public partial class T_C_CertificateBorrow : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请部门</summary>	
		[Description("申请部门")]
        public string ApplyDept { get; set; } // ApplyDept
		/// <summary>申请部门名称</summary>	
		[Description("申请部门名称")]
        public string ApplyDeptName { get; set; } // ApplyDeptName
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string Project { get; set; } // Project
		/// <summary>项目名称名称</summary>	
		[Description("项目名称名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>项目编号</summary>	
		[Description("项目编号")]
        public string ProjectCode { get; set; } // ProjectCode
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>申请原因</summary>	
		[Description("申请原因")]
        public string ApplyReason { get; set; } // ApplyReason
		/// <summary>证书用途</summary>	
		[Description("证书用途")]
        public string CertificatePurpose { get; set; } // CertificatePurpose
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>借用日期</summary>	
		[Description("借用日期")]
        public DateTime? BorrowDate { get; set; } // BorrowDate
		/// <summary>预计归还日期</summary>	
		[Description("预计归还日期")]
        public DateTime? PlanReturnDate { get; set; } // PlanReturnDate
		/// <summary>部门负责人签字</summary>	
		[Description("部门负责人签字")]
        public string DeptSign { get; set; } // DeptSign
		/// <summary>公共信息部签字</summary>	
		[Description("公共信息部签字")]
        public string PublicDeptSign { get; set; } // PublicDeptSign

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_C_CertificateBorrow_ApplyContent> T_C_CertificateBorrow_ApplyContent { get { onT_C_CertificateBorrow_ApplyContentGetting(); return _T_C_CertificateBorrow_ApplyContent;} }
		private ICollection<T_C_CertificateBorrow_ApplyContent> _T_C_CertificateBorrow_ApplyContent;
		partial void onT_C_CertificateBorrow_ApplyContentGetting();


        public T_C_CertificateBorrow()
        {
            _T_C_CertificateBorrow_ApplyContent = new List<T_C_CertificateBorrow_ApplyContent>();
        }
    }

	/// <summary>申请内容</summary>	
	[Description("申请内容")]
    public partial class T_C_CertificateBorrow_ApplyContent : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_C_CertificateBorrowID { get; set; } // T_C_CertificateBorrowID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>证书名称</summary>	
		[Description("证书名称")]
        public string Certificate { get; set; } // Certificate
		/// <summary>证书名称名称</summary>	
		[Description("证书名称名称")]
        public string CertificateName { get; set; } // CertificateName
		/// <summary>证书编号</summary>	
		[Description("证书编号")]
        public string CertificateCode { get; set; } // CertificateCode
		/// <summary>证书类型</summary>	
		[Description("证书类型")]
        public string CertificateType { get; set; } // CertificateType
		/// <summary>数量</summary>	
		[Description("数量")]
        public string Counts { get; set; } // Counts

        // Foreign keys
		[JsonIgnore]
        public virtual T_C_CertificateBorrow T_C_CertificateBorrow { get; set; } //  T_C_CertificateBorrowID - FK_T_C_CertificateBorrow_ApplyContent_T_C_CertificateBorrow
    }

	/// <summary>测试双语</summary>	
	[Description("测试双语")]
    public partial class T_ceshishuangyu : Formula.BaseModel
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
		/// <summary>单行文本框</summary>	
		[Description("单行文本框")]
        public string sdgawr { get; set; } // sdgawr
		/// <summary>多行文本框</summary>	
		[Description("多行文本框")]
        public string sgeh { get; set; } // sgeh
		/// <summary>日期选择框</summary>	
		[Description("日期选择框")]
        public DateTime? haaa { get; set; } // haaa
		/// <summary>多选框列表</summary>	
		[Description("多选框列表")]
        public string fghfffff { get; set; } // fghfffff
		/// <summary>单选框列表</summary>	
		[Description("单选框列表")]
        public string dfgsss { get; set; } // dfgsss
		/// <summary>组合下拉框</summary>	
		[Description("组合下拉框")]
        public string qqqqq { get; set; } // qqqqq
		/// <summary>弹出用户</summary>	
		[Description("弹出用户")]
        public string wwwww { get; set; } // wwwww
		/// <summary>弹出用户名称</summary>	
		[Description("弹出用户名称")]
        public string wwwwwName { get; set; } // wwwwwName
		/// <summary>弹出部门</summary>	
		[Description("弹出部门")]
        public string eeeee { get; set; } // eeeee
		/// <summary>弹出部门名称</summary>	
		[Description("弹出部门名称")]
        public string eeeeeName { get; set; } // eeeeeName
		/// <summary>单附件上传</summary>	
		[Description("单附件上传")]
        public string rrrrr { get; set; } // rrrrr
		/// <summary>多附件上传</summary>	
		[Description("多附件上传")]
        public string ttttt { get; set; } // ttttt
		/// <summary>数字输入框</summary>	
		[Description("数字输入框")]
        public decimal? yyyyy { get; set; } // yyyyy
		/// <summary>子表</summary>	
		[Description("子表")]
        public string uuuuu { get; set; } // uuuuu
		/// <summary>复选框</summary>	
		[Description("复选框")]
        public string iiiii { get; set; } // iiiii
		/// <summary>流程签字框</summary>	
		[Description("流程签字框")]
        public string ewqtt { get; set; } // ewqtt

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_ceshishuangyu_uuuuu> T_ceshishuangyu_uuuuu { get { onT_ceshishuangyu_uuuuuGetting(); return _T_ceshishuangyu_uuuuu;} }
		private ICollection<T_ceshishuangyu_uuuuu> _T_ceshishuangyu_uuuuu;
		partial void onT_ceshishuangyu_uuuuuGetting();


        public T_ceshishuangyu()
        {
            _T_ceshishuangyu_uuuuu = new List<T_ceshishuangyu_uuuuu>();
        }
    }

	/// <summary>子表</summary>	
	[Description("子表")]
    public partial class T_ceshishuangyu_uuuuu : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_ceshishuangyuID { get; set; } // T_ceshishuangyuID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>标题一</summary>	
		[Description("标题一")]
        public string sdfg { get; set; } // sdfg
		/// <summary>标题2</summary>	
		[Description("标题2")]
        public string gerg { get; set; } // gerg
		/// <summary>标题3</summary>	
		[Description("标题3")]
        public DateTime? dger { get; set; } // dger
		/// <summary>标题4</summary>	
		[Description("标题4")]
        public string ergq { get; set; } // ergq
		/// <summary>标题5</summary>	
		[Description("标题5")]
        public string wef { get; set; } // wef
		/// <summary>标题5名称</summary>	
		[Description("标题5名称")]
        public string wefName { get; set; } // wefName
		/// <summary>标题6</summary>	
		[Description("标题6")]
        public string sdgf { get; set; } // sdgf
		/// <summary>标题6名称</summary>	
		[Description("标题6名称")]
        public string sdgfName { get; set; } // sdgfName

        // Foreign keys
		[JsonIgnore]
        public virtual T_ceshishuangyu T_ceshishuangyu { get; set; } //  T_ceshishuangyuID - FK_T_ceshishuangyu_uuuuu_T_ceshishuangyu
    }

	/// <summary>部门预算编制</summary>	
	[Description("部门预算编制")]
    public partial class T_D_DeptBudget : Formula.BaseModel
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
		/// <summary>序号</summary>	
		[Description("序号")]
        public string Number { get; set; } // Number
		/// <summary>预算类别</summary>	
		[Description("预算类别")]
        public string BudgetClass { get; set; } // BudgetClass
		/// <summary>预算分类</summary>	
		[Description("预算分类")]
        public string BudgetType { get; set; } // BudgetType
		/// <summary>预算金额（元）</summary>	
		[Description("预算金额（元）")]
        public decimal? MoneyValue { get; set; } // MoneyValue
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>年份</summary>	
		[Description("年份")]
        public int? Year { get; set; } // Year
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
    }

	/// <summary>部门预算编制升版</summary>	
	[Description("部门预算编制升版")]
    public partial class T_D_DeptBudgetUp : Formula.BaseModel
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
		/// <summary>序号</summary>	
		[Description("序号")]
        public string Number { get; set; } // Number
		/// <summary>预算类别</summary>	
		[Description("预算类别")]
        public string BudgetClass { get; set; } // BudgetClass
		/// <summary>预算分类</summary>	
		[Description("预算分类")]
        public string BudgetType { get; set; } // BudgetType
		/// <summary>预算金额（元）</summary>	
		[Description("预算金额（元）")]
        public decimal? MoneyValue { get; set; } // MoneyValue
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>年份</summary>	
		[Description("年份")]
        public int? Year { get; set; } // Year
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>主表ID</summary>	
		[Description("主表ID")]
        public string DeptBudgetID { get; set; } // DeptBudgetID
    }

	/// <summary>人事档案</summary>	
	[Description("人事档案")]
    public partial class T_EmployeePersonalRecords : Formula.BaseModel
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
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
		/// <summary>档案编号</summary>	
		[Description("档案编号")]
        public string Code { get; set; } // Code
		/// <summary>档案类型</summary>	
		[Description("档案类型")]
        public string Type { get; set; } // Type
		/// <summary>档案保存单位</summary>	
		[Description("档案保存单位")]
        public string KeepUnit { get; set; } // KeepUnit
		/// <summary>档案来源单位</summary>	
		[Description("档案来源单位")]
        public string SourceUnit { get; set; } // SourceUnit
		/// <summary>档案转出单位</summary>	
		[Description("档案转出单位")]
        public string ExitUnit { get; set; } // ExitUnit
		/// <summary>报到证提交时间</summary>	
		[Description("报到证提交时间")]
        public DateTime? ReportCardSubDate { get; set; } // ReportCardSubDate
		/// <summary>转入时间</summary>	
		[Description("转入时间")]
        public DateTime? EnterDate { get; set; } // EnterDate
		/// <summary>转出时间</summary>	
		[Description("转出时间")]
        public DateTime? ExitDate { get; set; } // ExitDate
		/// <summary>户口类型</summary>	
		[Description("户口类型")]
        public string ResidentAccountsType { get; set; } // ResidentAccountsType
		/// <summary>户口所属街道</summary>	
		[Description("户口所属街道")]
        public string ResidentAccountsStreet { get; set; } // ResidentAccountsStreet
    }

	/// <summary>社会关系</summary>	
	[Description("社会关系")]
    public partial class T_EmployeeSocialSecurity : Formula.BaseModel
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
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Name { get; set; } // Name
		/// <summary>与员工关系</summary>	
		[Description("与员工关系")]
        public string Relation { get; set; } // Relation
		/// <summary>出生日期</summary>	
		[Description("出生日期")]
        public DateTime? BirthDate { get; set; } // BirthDate
		/// <summary>性别</summary>	
		[Description("性别")]
        public string Sex { get; set; } // Sex
		/// <summary>工作单位</summary>	
		[Description("工作单位")]
        public string WorkUnit { get; set; } // WorkUnit
		/// <summary>职务</summary>	
		[Description("职务")]
        public string Job { get; set; } // Job
		/// <summary>联系电话</summary>	
		[Description("联系电话")]
        public string Phone { get; set; } // Phone
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>员工ID</summary>	
		[Description("员工ID")]
        public string EmployeeID { get; set; } // EmployeeID
    }

	/// <summary>流程_出差管理_出差申请</summary>	
	[Description("流程_出差管理_出差申请")]
    public partial class T_Evection_EvectionApply : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>联系电话</summary>	
		[Description("联系电话")]
        public string Phone { get; set; } // Phone
		/// <summary>项目经理签字</summary>	
		[Description("项目经理签字")]
        public string PMSign { get; set; } // PMSign
		/// <summary>部门经理签字</summary>	
		[Description("部门经理签字")]
        public string DeptSign { get; set; } // DeptSign
		/// <summary>出差项目名称名称</summary>	
		[Description("出差项目名称名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>出差开始时间</summary>	
		[Description("出差开始时间")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary>出差结束时间</summary>	
		[Description("出差结束时间")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary>出差目的地</summary>	
		[Description("出差目的地")]
        public string Destination { get; set; } // Destination
		/// <summary>出差目的</summary>	
		[Description("出差目的")]
        public string Target { get; set; } // Target
		/// <summary>项目经理</summary>	
		[Description("项目经理")]
        public string PM { get; set; } // PM
		/// <summary>项目经理ID</summary>	
		[Description("项目经理ID")]
        public string PMID { get; set; } // PMID
		/// <summary>出差申请单编号</summary>	
		[Description("出差申请单编号")]
        public string SerialNumber { get; set; } // SerialNumber
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>出差项目名称</summary>	
		[Description("出差项目名称")]
        public string Project { get; set; } // Project
		/// <summary>项目编号</summary>	
		[Description("项目编号")]
        public string ProjectID { get; set; } // ProjectID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_Evection_EvectionApply_IfAircraft> T_Evection_EvectionApply_IfAircraft { get { onT_Evection_EvectionApply_IfAircraftGetting(); return _T_Evection_EvectionApply_IfAircraft;} }
		private ICollection<T_Evection_EvectionApply_IfAircraft> _T_Evection_EvectionApply_IfAircraft;
		partial void onT_Evection_EvectionApply_IfAircraftGetting();

		[JsonIgnore]
        public virtual ICollection<T_Evection_EvectionApply_Schedule> T_Evection_EvectionApply_Schedule { get { onT_Evection_EvectionApply_ScheduleGetting(); return _T_Evection_EvectionApply_Schedule;} }
		private ICollection<T_Evection_EvectionApply_Schedule> _T_Evection_EvectionApply_Schedule;
		partial void onT_Evection_EvectionApply_ScheduleGetting();


        public T_Evection_EvectionApply()
        {
            _T_Evection_EvectionApply_IfAircraft = new List<T_Evection_EvectionApply_IfAircraft>();
            _T_Evection_EvectionApply_Schedule = new List<T_Evection_EvectionApply_Schedule>();
        }
    }

	/// <summary>如选择飞机作为交通工具请填写</summary>	
	[Description("如选择飞机作为交通工具请填写")]
    public partial class T_Evection_EvectionApply_IfAircraft : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_Evection_EvectionApplyID { get; set; } // T_Evection_EvectionApplyID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>折扣率</summary>	
		[Description("折扣率")]
        public decimal? Discount { get; set; } // Discount
		/// <summary>是否提前3天订购</summary>	
		[Description("是否提前3天订购")]
        public string IfAheadThreeDays { get; set; } // IfAheadThreeDays
		/// <summary>如未提前3天订购请说明原因</summary>	
		[Description("如未提前3天订购请说明原因")]
        public string NotAheadReason { get; set; } // NotAheadReason

        // Foreign keys
		[JsonIgnore]
        public virtual T_Evection_EvectionApply T_Evection_EvectionApply { get; set; } //  T_Evection_EvectionApplyID - FK_T_Flow_Evection_EvectionApply_IfAircraft_T_Flow_Evection_EvectionApply
    }

	/// <summary>具体出差安排</summary>	
	[Description("具体出差安排")]
    public partial class T_Evection_EvectionApply_Schedule : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_Evection_EvectionApplyID { get; set; } // T_Evection_EvectionApplyID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>交通方式</summary>	
		[Description("交通方式")]
        public string Traffic { get; set; } // Traffic
		/// <summary>出发日期</summary>	
		[Description("出发日期")]
        public DateTime? StartDate { get; set; } // StartDate
		/// <summary>起点</summary>	
		[Description("起点")]
        public string StartPoint { get; set; } // StartPoint
		/// <summary>终点</summary>	
		[Description("终点")]
        public string EndPont { get; set; } // EndPont
		/// <summary>期限（天）</summary>	
		[Description("期限（天）")]
        public int? Deadline { get; set; } // Deadline
		/// <summary>预计费用（元）</summary>	
		[Description("预计费用（元）")]
        public decimal? PredictCost { get; set; } // PredictCost
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark

        // Foreign keys
		[JsonIgnore]
        public virtual T_Evection_EvectionApply T_Evection_EvectionApply { get; set; } //  T_Evection_EvectionApplyID - FK_T_Flow_Evection_EvectionApply_Schedule_T_Flow_Evection_EvectionApply
    }

	/// <summary>流程_出差管理_出差备用金申请</summary>	
	[Description("流程_出差管理_出差备用金申请")]
    public partial class T_Evection_PettyCash : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>申请时间</summary>	
		[Description("申请时间")]
        public DateTime? ApplyDatetime { get; set; } // ApplyDatetime
		/// <summary>用途</summary>	
		[Description("用途")]
        public string Purpose { get; set; } // Purpose
		/// <summary>申请金额（元）</summary>	
		[Description("申请金额（元）")]
        public string AppliedAmount { get; set; } // AppliedAmount
		/// <summary>大写数目</summary>	
		[Description("大写数目")]
        public string BigAmount { get; set; } // BigAmount
		/// <summary>部门经理签字</summary>	
		[Description("部门经理签字")]
        public string PMSign { get; set; } // PMSign
		/// <summary>财务签字</summary>	
		[Description("财务签字")]
        public string FinanceSign { get; set; } // FinanceSign
		/// <summary>总经理签字</summary>	
		[Description("总经理签字")]
        public string GeneralSign { get; set; } // GeneralSign
		/// <summary>申请时间</summary>	
		[Description("申请时间")]
        public DateTime? ApplyTime { get; set; } // ApplyTime
    }

	/// <summary>流程_行政物品管理_行政物品领用</summary>	
	[Description("流程_行政物品管理_行政物品领用")]
    public partial class T_G_GoodsApply : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请部门</summary>	
		[Description("申请部门")]
        public string ApplyDept { get; set; } // ApplyDept
		/// <summary>申请部门名称</summary>	
		[Description("申请部门名称")]
        public string ApplyDeptName { get; set; } // ApplyDeptName
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>申请人签字</summary>	
		[Description("申请人签字")]
        public string ApplySign { get; set; } // ApplySign
		/// <summary>办公室意见</summary>	
		[Description("办公室意见")]
        public string OfficeSign { get; set; } // OfficeSign

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_G_GoodsApply_ApplyDetail> T_G_GoodsApply_ApplyDetail { get { onT_G_GoodsApply_ApplyDetailGetting(); return _T_G_GoodsApply_ApplyDetail;} }
		private ICollection<T_G_GoodsApply_ApplyDetail> _T_G_GoodsApply_ApplyDetail;
		partial void onT_G_GoodsApply_ApplyDetailGetting();


        public T_G_GoodsApply()
        {
            _T_G_GoodsApply_ApplyDetail = new List<T_G_GoodsApply_ApplyDetail>();
        }
    }

	/// <summary>申请内容</summary>	
	[Description("申请内容")]
    public partial class T_G_GoodsApply_ApplyDetail : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_G_GoodsApplyID { get; set; } // T_G_GoodsApplyID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>物品名称</summary>	
		[Description("物品名称")]
        public string Goods { get; set; } // Goods
		/// <summary>物品名称名称</summary>	
		[Description("物品名称名称")]
        public string GoodsName { get; set; } // GoodsName
		/// <summary>型号</summary>	
		[Description("型号")]
        public string Model { get; set; } // Model
		/// <summary>单位</summary>	
		[Description("单位")]
        public string Unit { get; set; } // Unit
		/// <summary>领用数量</summary>	
		[Description("领用数量")]
        public int? Quantity { get; set; } // Quantity

        // Foreign keys
		[JsonIgnore]
        public virtual T_G_GoodsApply T_G_GoodsApply { get; set; } //  T_G_GoodsApplyID - FK_T_G_GoodsApply_ApplyDetail_T_G_GoodsApply
    }

	/// <summary>功能_工资奖金管理_个人奖金录入</summary>	
	[Description("功能_工资奖金管理_个人奖金录入")]
    public partial class T_HR_PersonalBonusInput : Formula.BaseModel
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
		/// <summary>人员</summary>	
		[Description("人员")]
        public string Employee { get; set; } // Employee
		/// <summary>人员名称</summary>	
		[Description("人员名称")]
        public string EmployeeName { get; set; } // EmployeeName
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>奖金类别</summary>	
		[Description("奖金类别")]
        public string BonusCategory { get; set; } // BonusCategory
		/// <summary>批次</summary>	
		[Description("批次")]
        public string Batch { get; set; } // Batch
		/// <summary>发放金额（元）</summary>	
		[Description("发放金额（元）")]
        public double? SendOutMoney { get; set; } // SendOutMoney
		/// <summary>审批日期</summary>	
		[Description("审批日期")]
        public DateTime? ApproveDate { get; set; } // ApproveDate
		/// <summary>发放日期</summary>	
		[Description("发放日期")]
        public DateTime? SendOutDate { get; set; } // SendOutDate
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>项目号</summary>	
		[Description("项目号")]
        public string Project { get; set; } // Project
		/// <summary>项目号名称</summary>	
		[Description("项目号名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>员工编号</summary>	
		[Description("员工编号")]
        public string Code { get; set; } // Code
		/// <summary>年</summary>	
		[Description("年")]
        public string Year { get; set; } // Year
		/// <summary>月</summary>	
		[Description("月")]
        public string Month { get; set; } // Month
    }

	/// <summary>功能_工资奖金管理_人员工资管理</summary>	
	[Description("功能_工资奖金管理_人员工资管理")]
    public partial class T_HR_SalaryManage : Formula.BaseModel
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
		/// <summary>员工编号</summary>	
		[Description("员工编号")]
        public string Code { get; set; } // Code
		/// <summary>员工姓名</summary>	
		[Description("员工姓名")]
        public string Employee { get; set; } // Employee
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>员工岗位</summary>	
		[Description("员工岗位")]
        public string Position { get; set; } // Position
		/// <summary>税前工资</summary>	
		[Description("税前工资")]
        public decimal? Salary { get; set; } // Salary
		/// <summary>员工姓名名称</summary>	
		[Description("员工姓名名称")]
        public string EmployeeName { get; set; } // EmployeeName
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>基本工资</summary>	
		[Description("基本工资")]
        public decimal? Money { get; set; } // Money
		/// <summary>绩效基数</summary>	
		[Description("绩效基数")]
        public decimal? PerformanceBase { get; set; } // PerformanceBase
		/// <summary>绩效分</summary>	
		[Description("绩效分")]
        public decimal? PerformanceScore { get; set; } // PerformanceScore
		/// <summary>绩效工资</summary>	
		[Description("绩效工资")]
        public string PerformanceSalary { get; set; } // PerformanceSalary
		/// <summary>半全年</summary>	
		[Description("半全年")]
        public string HalfFullYear { get; set; } // HalfFullYear
		/// <summary>其他奖金</summary>	
		[Description("其他奖金")]
        public decimal? WholeBonus { get; set; } // WholeBonus
		/// <summary>项目奖金</summary>	
		[Description("项目奖金")]
        public decimal? ProjectBonus { get; set; } // ProjectBonus
		/// <summary>奖金合计</summary>	
		[Description("奖金合计")]
        public decimal? SumBonus { get; set; } // SumBonus
		/// <summary>加班费</summary>	
		[Description("加班费")]
        public decimal? OvertimePay { get; set; } // OvertimePay
		/// <summary>扣款</summary>	
		[Description("扣款")]
        public decimal? CutPayment { get; set; } // CutPayment
		/// <summary>津贴</summary>	
		[Description("津贴")]
        public decimal? Allowance { get; set; } // Allowance
		/// <summary>其他合计</summary>	
		[Description("其他合计")]
        public decimal? OtherSum { get; set; } // OtherSum
		/// <summary>养老（个人）</summary>	
		[Description("养老（个人）")]
        public decimal? POldAgePension { get; set; } // POldAgePension
		/// <summary>失业（个人）</summary>	
		[Description("失业（个人）")]
        public decimal? PUnemployment { get; set; } // PUnemployment
		/// <summary>医疗（个人）</summary>	
		[Description("医疗（个人）")]
        public decimal? PMedicalTreatment { get; set; } // PMedicalTreatment
		/// <summary>大病（个人）</summary>	
		[Description("大病（个人）")]
        public decimal? PSeriousIllness { get; set; } // PSeriousIllness
		/// <summary>工伤（个人）</summary>	
		[Description("工伤（个人）")]
        public decimal? POccupationalInjury { get; set; } // POccupationalInjury
		/// <summary>生育（个人）</summary>	
		[Description("生育（个人）")]
        public decimal? PBear { get; set; } // PBear
		/// <summary>社保合计（个人）</summary>	
		[Description("社保合计（个人）")]
        public decimal? PSocialSecurity { get; set; } // PSocialSecurity
		/// <summary>公积金（个人）</summary>	
		[Description("公积金（个人）")]
        public decimal? PAccumulationFund { get; set; } // PAccumulationFund
		/// <summary>小计（个人）</summary>	
		[Description("小计（个人）")]
        public decimal? PSubtotal { get; set; } // PSubtotal
		/// <summary>养老（公司）</summary>	
		[Description("养老（公司）")]
        public decimal? COldAgePension { get; set; } // COldAgePension
		/// <summary>失业（公司）</summary>	
		[Description("失业（公司）")]
        public decimal? CUnemployment { get; set; } // CUnemployment
		/// <summary>医疗（公司）</summary>	
		[Description("医疗（公司）")]
        public decimal? CMedicalTreatment { get; set; } // CMedicalTreatment
		/// <summary>大病（公司）</summary>	
		[Description("大病（公司）")]
        public decimal? CSeriousIllness { get; set; } // CSeriousIllness
		/// <summary>工伤（公司）</summary>	
		[Description("工伤（公司）")]
        public decimal? COccupationalInjury { get; set; } // COccupationalInjury
		/// <summary>生育（公司）</summary>	
		[Description("生育（公司）")]
        public decimal? CBear { get; set; } // CBear
		/// <summary>社保合计（公司）</summary>	
		[Description("社保合计（公司）")]
        public decimal? CSocialSecurity { get; set; } // CSocialSecurity
		/// <summary>公积金（公司）</summary>	
		[Description("公积金（公司）")]
        public decimal? CAccumulationFund { get; set; } // CAccumulationFund
		/// <summary>小计（公司）</summary>	
		[Description("小计（公司）")]
        public decimal? CSubtotal { get; set; } // CSubtotal
		/// <summary>养老（个人补缴）</summary>	
		[Description("养老（个人补缴）")]
        public decimal? POldAgePensionB { get; set; } // POldAgePensionB
		/// <summary>失业（个人补缴）</summary>	
		[Description("失业（个人补缴）")]
        public decimal? PUnemploymentB { get; set; } // PUnemploymentB
		/// <summary>医疗（个人补缴）</summary>	
		[Description("医疗（个人补缴）")]
        public decimal? PMedicalTreatmentB { get; set; } // PMedicalTreatmentB
		/// <summary>大病（个人补缴）</summary>	
		[Description("大病（个人补缴）")]
        public decimal? PSeriousIllnessB { get; set; } // PSeriousIllnessB
		/// <summary>工伤（个人补缴）</summary>	
		[Description("工伤（个人补缴）")]
        public decimal? POccupationalInjuryB { get; set; } // POccupationalInjuryB
		/// <summary>生育（个人补缴）</summary>	
		[Description("生育（个人补缴）")]
        public decimal? PBearB { get; set; } // PBearB
		/// <summary>社保合计（个人补缴）</summary>	
		[Description("社保合计（个人补缴）")]
        public decimal? PSocialSecurityB { get; set; } // PSocialSecurityB
		/// <summary>公积金（个人补缴）</summary>	
		[Description("公积金（个人补缴）")]
        public decimal? PAccumulationFundB { get; set; } // PAccumulationFundB
		/// <summary>小计（个人补缴）</summary>	
		[Description("小计（个人补缴）")]
        public decimal? PSubtotalB { get; set; } // PSubtotalB
		/// <summary>养老（公司补缴）</summary>	
		[Description("养老（公司补缴）")]
        public decimal? COldAgePensionB { get; set; } // COldAgePensionB
		/// <summary>失业（公司补缴）</summary>	
		[Description("失业（公司补缴）")]
        public decimal? CUnemploymentB { get; set; } // CUnemploymentB
		/// <summary>医疗（公司补缴）</summary>	
		[Description("医疗（公司补缴）")]
        public decimal? CMedicalTreatmentB { get; set; } // CMedicalTreatmentB
		/// <summary>大病（公司补缴）</summary>	
		[Description("大病（公司补缴）")]
        public decimal? CSeriousIllnessB { get; set; } // CSeriousIllnessB
		/// <summary>工伤（公司补缴）</summary>	
		[Description("工伤（公司补缴）")]
        public decimal? COccupationalInjuryB { get; set; } // COccupationalInjuryB
		/// <summary>生育（公司补缴）</summary>	
		[Description("生育（公司补缴）")]
        public decimal? CBearB { get; set; } // CBearB
		/// <summary>社保合计（公司补缴）</summary>	
		[Description("社保合计（公司补缴）")]
        public decimal? CSocialSecurityB { get; set; } // CSocialSecurityB
		/// <summary>公积金（公司补缴）</summary>	
		[Description("公积金（公司补缴）")]
        public decimal? CAccumulationFundB { get; set; } // CAccumulationFundB
		/// <summary>小计（公司补缴）</summary>	
		[Description("小计（公司补缴）")]
        public decimal? CSubtotalB { get; set; } // CSubtotalB
		/// <summary>养老</summary>	
		[Description("养老")]
        public decimal? OldAgePension { get; set; } // OldAgePension
		/// <summary>失业</summary>	
		[Description("失业")]
        public decimal? Unemployment { get; set; } // Unemployment
		/// <summary>医疗</summary>	
		[Description("医疗")]
        public decimal? MedicalTreatment { get; set; } // MedicalTreatment
		/// <summary>大病</summary>	
		[Description("大病")]
        public decimal? SeriousIllness { get; set; } // SeriousIllness
		/// <summary>工伤</summary>	
		[Description("工伤")]
        public decimal? OccupationalInjury { get; set; } // OccupationalInjury
		/// <summary>生育</summary>	
		[Description("生育")]
        public decimal? Bear { get; set; } // Bear
		/// <summary>社保合计</summary>	
		[Description("社保合计")]
        public decimal? SocialSecurity { get; set; } // SocialSecurity
		/// <summary>公积金</summary>	
		[Description("公积金")]
        public decimal? AccumulationFund { get; set; } // AccumulationFund
		/// <summary>小计</summary>	
		[Description("小计")]
        public decimal? Subtotal { get; set; } // Subtotal
		/// <summary>应发工资</summary>	
		[Description("应发工资")]
        public decimal? YFGZ { get; set; } // YFGZ
		/// <summary>个人所得税</summary>	
		[Description("个人所得税")]
        public decimal? GRSDS { get; set; } // GRSDS
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>实发工资</summary>	
		[Description("实发工资")]
        public decimal? PracticalSalary { get; set; } // PracticalSalary
		/// <summary>人力成本</summary>	
		[Description("人力成本")]
        public string HumanCost { get; set; } // HumanCost
		/// <summary>年份</summary>	
		[Description("年份")]
        public int? Year { get; set; } // Year
		/// <summary>月份</summary>	
		[Description("月份")]
        public int? Month { get; set; } // Month
    }

	/// <summary>功能_员工管理_社会关系</summary>	
	[Description("功能_员工管理_社会关系")]
    public partial class T_HR_SocialRelation : Formula.BaseModel
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
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Employee { get; set; } // Employee
		/// <summary>与员工关系</summary>	
		[Description("与员工关系")]
        public string Relation { get; set; } // Relation
		/// <summary>出生日期</summary>	
		[Description("出生日期")]
        public DateTime? Birthday { get; set; } // Birthday
		/// <summary>性别</summary>	
		[Description("性别")]
        public string Sex { get; set; } // Sex
		/// <summary>工作单位</summary>	
		[Description("工作单位")]
        public string WorkUnit { get; set; } // WorkUnit
		/// <summary>职务</summary>	
		[Description("职务")]
        public string Duty { get; set; } // Duty
		/// <summary>联系电话</summary>	
		[Description("联系电话")]
        public string Tel { get; set; } // Tel
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>员工编号</summary>	
		[Description("员工编号")]
        public string EmployeeID { get; set; } // EmployeeID
    }

	/// <summary>仪器借用登记</summary>	
	[Description("仪器借用登记")]
    public partial class T_I_InstrumentBorrow : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>所属部门</summary>	
		[Description("所属部门")]
        public string Dept { get; set; } // Dept
		/// <summary>所属部门名称</summary>	
		[Description("所属部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>借用日期</summary>	
		[Description("借用日期")]
        public DateTime? BorrowDate { get; set; } // BorrowDate
		/// <summary>物品</summary>	
		[Description("物品")]
        public string Instrument { get; set; } // Instrument
		/// <summary>物品名称</summary>	
		[Description("物品名称")]
        public string InstrumentName { get; set; } // InstrumentName
		/// <summary>说明</summary>	
		[Description("说明")]
        public string Description { get; set; } // Description
		/// <summary>借用人签字</summary>	
		[Description("借用人签字")]
        public string BorrowUserSign { get; set; } // BorrowUserSign
		/// <summary>办公室意见</summary>	
		[Description("办公室意见")]
        public string OfficeSign { get; set; } // OfficeSign
    }

	/// <summary>流程_仪器领用管理_仪器报废申请</summary>	
	[Description("流程_仪器领用管理_仪器报废申请")]
    public partial class T_I_InstrumentDiscard : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>所属部门</summary>	
		[Description("所属部门")]
        public string Dept { get; set; } // Dept
		/// <summary>所属部门名称</summary>	
		[Description("所属部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>报废日期</summary>	
		[Description("报废日期")]
        public DateTime? BorrowDate { get; set; } // BorrowDate
		/// <summary>物品</summary>	
		[Description("物品")]
        public string Instrument { get; set; } // Instrument
		/// <summary>物品名称</summary>	
		[Description("物品名称")]
        public string InstrumentName { get; set; } // InstrumentName
		/// <summary>说明</summary>	
		[Description("说明")]
        public string Description { get; set; } // Description
		/// <summary>申请人签字</summary>	
		[Description("申请人签字")]
        public string ApplyUserSign { get; set; } // ApplyUserSign
		/// <summary>办公室意见</summary>	
		[Description("办公室意见")]
        public string OfficeSign { get; set; } // OfficeSign
    }

	/// <summary>仪器归还登记</summary>	
	[Description("仪器归还登记")]
    public partial class T_I_InstrumentReturn : Formula.BaseModel
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
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>所属部门</summary>	
		[Description("所属部门")]
        public string Dept { get; set; } // Dept
		/// <summary>所属部门名称</summary>	
		[Description("所属部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>归还日期</summary>	
		[Description("归还日期")]
        public DateTime? ReturnDate { get; set; } // ReturnDate
		/// <summary>物品</summary>	
		[Description("物品")]
        public string Instrument { get; set; } // Instrument
		/// <summary>物品名称</summary>	
		[Description("物品名称")]
        public string InstrumentName { get; set; } // InstrumentName
		/// <summary>说明</summary>	
		[Description("说明")]
        public string Description { get; set; } // Description
		/// <summary>归还人签字</summary>	
		[Description("归还人签字")]
        public string ReturnUserSign { get; set; } // ReturnUserSign
		/// <summary>办公室意见</summary>	
		[Description("办公室意见")]
        public string OfficeSign { get; set; } // OfficeSign
		/// <summary>借用流程ID</summary>	
		[Description("借用流程ID")]
        public string BorrowID { get; set; } // BorrowID
    }

	/// <summary>流程_请假管理_请假申请</summary>	
	[Description("流程_请假管理_请假申请")]
    public partial class T_LeaveManage_LeaveApply : Formula.BaseModel
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
		/// <summary>创建人</summary>	
		[Description("创建人")]
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
		/// <summary>年假剩余天数</summary>	
		[Description("年假剩余天数")]
        public string AnnualLeaveResidueDay { get; set; } // AnnualLeaveResidueDay
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请部门</summary>	
		[Description("申请部门")]
        public string ApplyDept { get; set; } // ApplyDept
		/// <summary>申请部门名称</summary>	
		[Description("申请部门名称")]
        public string ApplyDeptName { get; set; } // ApplyDeptName
		/// <summary>请假日期（起）</summary>	
		[Description("请假日期（起）")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary>请假日期（止）</summary>	
		[Description("请假日期（止）")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary>请假类别</summary>	
		[Description("请假类别")]
        public string LeaveCategory { get; set; } // LeaveCategory
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Describe { get; set; } // Describe
		/// <summary>请假原因</summary>	
		[Description("请假原因")]
        public string LeaveReason { get; set; } // LeaveReason
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public string CreateTime { get; set; } // CreateTime
		/// <summary>请假天数</summary>	
		[Description("请假天数")]
        public double? LeaveDays { get; set; } // LeaveDays
		/// <summary>部门负责人签字</summary>	
		[Description("部门负责人签字")]
        public string DeptSign { get; set; } // DeptSign
		/// <summary>人事部门签字</summary>	
		[Description("人事部门签字")]
        public string PMSign { get; set; } // PMSign
		/// <summary>董事长签字</summary>	
		[Description("董事长签字")]
        public string ChairmanSign { get; set; } // ChairmanSign
		/// <summary>开始时间段</summary>	
		[Description("开始时间段")]
        public string StartTimeSlot { get; set; } // StartTimeSlot
		/// <summary>结束时间段</summary>	
		[Description("结束时间段")]
        public string EndTimeSlot { get; set; } // EndTimeSlot
		/// <summary>职务代理人</summary>	
		[Description("职务代理人")]
        public string AgentUser { get; set; } // AgentUser
		/// <summary>职务代理人名称</summary>	
		[Description("职务代理人名称")]
        public string AgentUserName { get; set; } // AgentUserName
    }

	/// <summary>流程-会议管理-会议申请</summary>	
	[Description("流程-会议管理-会议申请")]
    public partial class T_M_ConferenceApply : Formula.BaseModel
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
		/// <summary>会议主题</summary>	
		[Description("会议主题")]
        public string Title { get; set; } // Title
		/// <summary>主办部门</summary>	
		[Description("主办部门")]
        public string HostDept { get; set; } // HostDept
		/// <summary>主办部门名称</summary>	
		[Description("主办部门名称")]
        public string HostDeptName { get; set; } // HostDeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请时间</summary>	
		[Description("申请时间")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>会议开始日期</summary>	
		[Description("会议开始日期")]
        public DateTime? MeetingStart { get; set; } // MeetingStart
		/// <summary>会议开始时间H</summary>	
		[Description("会议开始时间H")]
        public int? MeetingStartHour { get; set; } // MeetingStartHour
		/// <summary>会议开始时间M</summary>	
		[Description("会议开始时间M")]
        public int? MeetingStartMin { get; set; } // MeetingStartMin
		/// <summary>会议结束日期</summary>	
		[Description("会议结束日期")]
        public DateTime? MeetingEnd { get; set; } // MeetingEnd
		/// <summary>会议结束时间H</summary>	
		[Description("会议结束时间H")]
        public int? MeetingEndHour { get; set; } // MeetingEndHour
		/// <summary>会议结束时间M</summary>	
		[Description("会议结束时间M")]
        public int? MeetingEndMin { get; set; } // MeetingEndMin
		/// <summary>会议室编号</summary>	
		[Description("会议室编号")]
        public string MeetingRoom { get; set; } // MeetingRoom
		/// <summary>会议室编号名称</summary>	
		[Description("会议室编号名称")]
        public string MeetingRoomName { get; set; } // MeetingRoomName
		/// <summary>会议地点</summary>	
		[Description("会议地点")]
        public string RoomAddress { get; set; } // RoomAddress
		/// <summary>主持人</summary>	
		[Description("主持人")]
        public string Host { get; set; } // Host
		/// <summary>主持人名称</summary>	
		[Description("主持人名称")]
        public string HostName { get; set; } // HostName
		/// <summary>主要内容</summary>	
		[Description("主要内容")]
        public string MainContent { get; set; } // MainContent
		/// <summary>我方人员</summary>	
		[Description("我方人员")]
        public string JoinUser { get; set; } // JoinUser
		/// <summary>我方人员名称</summary>	
		[Description("我方人员名称")]
        public string JoinUserName { get; set; } // JoinUserName
		/// <summary>贵宾人员</summary>	
		[Description("贵宾人员")]
        public string PartyBUsers { get; set; } // PartyBUsers
		/// <summary>我方人数</summary>	
		[Description("我方人数")]
        public int? SelfTotal { get; set; } // SelfTotal
		/// <summary>贵宾人数</summary>	
		[Description("贵宾人数")]
        public int? GuestTotal { get; set; } // GuestTotal
		/// <summary>我方领导人数</summary>	
		[Description("我方领导人数")]
        public int? SelfLeader { get; set; } // SelfLeader
		/// <summary>贵宾领导人数</summary>	
		[Description("贵宾领导人数")]
        public int? GuestLeader { get; set; } // GuestLeader
		/// <summary>其他</summary>	
		[Description("其他")]
        public string Others { get; set; } // Others
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>是否有会场需求</summary>	
		[Description("是否有会场需求")]
        public string IsNeedBanner { get; set; } // IsNeedBanner
		/// <summary>合计(元)</summary>	
		[Description("合计(元)")]
        public decimal? Sum { get; set; } // Sum
		/// <summary>总经理工作部意见</summary>	
		[Description("总经理工作部意见")]
        public string GeneralSign { get; set; } // GeneralSign
		/// <summary>会议管理员办理结果</summary>	
		[Description("会议管理员办理结果")]
        public string ConferenceAdminSign { get; set; } // ConferenceAdminSign
		/// <summary>状态</summary>	
		[Description("状态")]
        public string State { get; set; } // State

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_M_ConferenceApply_Budget> T_M_ConferenceApply_Budget { get { onT_M_ConferenceApply_BudgetGetting(); return _T_M_ConferenceApply_Budget;} }
		private ICollection<T_M_ConferenceApply_Budget> _T_M_ConferenceApply_Budget;
		partial void onT_M_ConferenceApply_BudgetGetting();


        public T_M_ConferenceApply()
        {
            _T_M_ConferenceApply_Budget = new List<T_M_ConferenceApply_Budget>();
        }
    }

	/// <summary>会议预算</summary>	
	[Description("会议预算")]
    public partial class T_M_ConferenceApply_Budget : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_M_ConferenceApplyID { get; set; } // T_M_ConferenceApplyID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>会议物品</summary>	
		[Description("会议物品")]
        public string Thing { get; set; } // Thing
		/// <summary>单价(元)</summary>	
		[Description("单价(元)")]
        public decimal? Price { get; set; } // Price
		/// <summary>数量</summary>	
		[Description("数量")]
        public decimal? Num { get; set; } // Num
		/// <summary>总额(元)</summary>	
		[Description("总额(元)")]
        public decimal? Sum { get; set; } // Sum

        // Foreign keys
		[JsonIgnore]
        public virtual T_M_ConferenceApply T_M_ConferenceApply { get; set; } //  T_M_ConferenceApplyID - FK_T_M_ConferenceApply_Budget_T_M_ConferenceApply
    }

	/// <summary>功能_会议管理_会议登记</summary>	
	[Description("功能_会议管理_会议登记")]
    public partial class T_M_ConferenceRegist : Formula.BaseModel
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
		/// <summary>会议主题</summary>	
		[Description("会议主题")]
        public string MeetApply { get; set; } // MeetApply
		/// <summary>会议主题名称</summary>	
		[Description("会议主题名称")]
        public string MeetApplyName { get; set; } // MeetApplyName
		/// <summary>主办部门</summary>	
		[Description("主办部门")]
        public string HostDept { get; set; } // HostDept
		/// <summary>主办部门名称</summary>	
		[Description("主办部门名称")]
        public string HostDeptName { get; set; } // HostDeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>计划举办时间</summary>	
		[Description("计划举办时间")]
        public DateTime? PlanMeetingDate { get; set; } // PlanMeetingDate
		/// <summary>计划会议地点</summary>	
		[Description("计划会议地点")]
        public string PlanMeetingPlace { get; set; } // PlanMeetingPlace
		/// <summary>编制人</summary>	
		[Description("编制人")]
        public string RegistUser { get; set; } // RegistUser
		/// <summary>编制人名称</summary>	
		[Description("编制人名称")]
        public string RegistUserName { get; set; } // RegistUserName
		/// <summary>编制日期</summary>	
		[Description("编制日期")]
        public DateTime? RegistDate { get; set; } // RegistDate
		/// <summary>计划参会人员</summary>	
		[Description("计划参会人员")]
        public string PlanJoinUserID { get; set; } // PlanJoinUserID
		/// <summary>计划参会人员名称</summary>	
		[Description("计划参会人员名称")]
        public string PlanJoinUserIDName { get; set; } // PlanJoinUserIDName
		/// <summary>实际举办时间起</summary>	
		[Description("实际举办时间起")]
        public DateTime? MeetingStartDate { get; set; } // MeetingStartDate
		/// <summary>实际举办时间止</summary>	
		[Description("实际举办时间止")]
        public DateTime? MeetingEndDate { get; set; } // MeetingEndDate
		/// <summary>实际举办地址</summary>	
		[Description("实际举办地址")]
        public string MettingPlace { get; set; } // MettingPlace
		/// <summary>实际参与人员</summary>	
		[Description("实际参与人员")]
        public string JoinUser { get; set; } // JoinUser
		/// <summary>实际参与人员名称</summary>	
		[Description("实际参与人员名称")]
        public string JoinUserName { get; set; } // JoinUserName
		/// <summary>合计</summary>	
		[Description("合计")]
        public double? Sum { get; set; } // Sum
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Attachment { get; set; } // Attachment

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_M_ConferenceRegist_Settlement> T_M_ConferenceRegist_Settlement { get { onT_M_ConferenceRegist_SettlementGetting(); return _T_M_ConferenceRegist_Settlement;} }
		private ICollection<T_M_ConferenceRegist_Settlement> _T_M_ConferenceRegist_Settlement;
		partial void onT_M_ConferenceRegist_SettlementGetting();


        public T_M_ConferenceRegist()
        {
            _T_M_ConferenceRegist_Settlement = new List<T_M_ConferenceRegist_Settlement>();
        }
    }

	/// <summary>会议结算</summary>	
	[Description("会议结算")]
    public partial class T_M_ConferenceRegist_Settlement : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_M_ConferenceRegistID { get; set; } // T_M_ConferenceRegistID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>会议物品</summary>	
		[Description("会议物品")]
        public string Thing { get; set; } // Thing
		/// <summary>单价(元)</summary>	
		[Description("单价(元)")]
        public decimal? Price { get; set; } // Price
		/// <summary>数量</summary>	
		[Description("数量")]
        public decimal? Num { get; set; } // Num
		/// <summary>总额(元)</summary>	
		[Description("总额(元)")]
        public decimal? Sum { get; set; } // Sum

        // Foreign keys
		[JsonIgnore]
        public virtual T_M_ConferenceRegist T_M_ConferenceRegist { get; set; } //  T_M_ConferenceRegistID - FK_T_M_ConferenceRegist_Settlement_T_M_ConferenceRegist
    }

	/// <summary>流程-会议管理-会议纪要</summary>	
	[Description("流程-会议管理-会议纪要")]
    public partial class T_M_ConferenceSummary : Formula.BaseModel
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
		/// <summary>会议主题</summary>	
		[Description("会议主题")]
        public string Title { get; set; } // Title
		/// <summary>会议主题名称</summary>	
		[Description("会议主题名称")]
        public string TitleName { get; set; } // TitleName
		/// <summary>参加人员</summary>	
		[Description("参加人员")]
        public string JoinUserID { get; set; } // JoinUserID
		/// <summary>参加人员名称</summary>	
		[Description("参加人员名称")]
        public string JoinUserIDName { get; set; } // JoinUserIDName
		/// <summary>主办部门</summary>	
		[Description("主办部门")]
        public string HostDepID { get; set; } // HostDepID
		/// <summary>主办部门名称</summary>	
		[Description("主办部门名称")]
        public string HostDepIDName { get; set; } // HostDepIDName
		/// <summary>主持人</summary>	
		[Description("主持人")]
        public string HostUserID { get; set; } // HostUserID
		/// <summary>主持人名称</summary>	
		[Description("主持人名称")]
        public string HostUserIDName { get; set; } // HostUserIDName
		/// <summary>会议日期</summary>	
		[Description("会议日期")]
        public DateTime? MeetingStart { get; set; } // MeetingStart
		/// <summary>记录人</summary>	
		[Description("记录人")]
        public string RecordUserID { get; set; } // RecordUserID
		/// <summary>记录人名称</summary>	
		[Description("记录人名称")]
        public string RecordUserIDName { get; set; } // RecordUserIDName
		/// <summary>会议纪要</summary>	
		[Description("会议纪要")]
        public string MeetingSummary { get; set; } // MeetingSummary
		/// <summary>相关附件</summary>	
		[Description("相关附件")]
        public string AboutInfomation { get; set; } // AboutInfomation
		/// <summary>是否需要审签</summary>	
		[Description("是否需要审签")]
        public string IsNeedSigned { get; set; } // IsNeedSigned
		/// <summary>审签人</summary>	
		[Description("审签人")]
        public string ApproveUserIDs { get; set; } // ApproveUserIDs
		/// <summary>审签人名称</summary>	
		[Description("审签人名称")]
        public string ApproveUserIDsName { get; set; } // ApproveUserIDsName
		/// <summary>批准人</summary>	
		[Description("批准人")]
        public string RatifyUserIDs { get; set; } // RatifyUserIDs
		/// <summary>批准人名称</summary>	
		[Description("批准人名称")]
        public string RatifyUserIDsName { get; set; } // RatifyUserIDsName
		/// <summary>主要内容</summary>	
		[Description("主要内容")]
        public string MainContent { get; set; } // MainContent
		/// <summary>详细内容</summary>	
		[Description("详细内容")]
        public string DetailContent { get; set; } // DetailContent
		/// <summary>审签人意见</summary>	
		[Description("审签人意见")]
        public string CountersignederComment { get; set; } // CountersignederComment
		/// <summary>批准人意见</summary>	
		[Description("批准人意见")]
        public string ApprovalerComment { get; set; } // ApprovalerComment
		/// <summary>会议主持人签发</summary>	
		[Description("会议主持人签发")]
        public string HostSign { get; set; } // HostSign
		/// <summary>会议记录人传阅</summary>	
		[Description("会议记录人传阅")]
        public string RecordSign { get; set; } // RecordSign
		/// <summary>相关人员会签</summary>	
		[Description("相关人员会签")]
        public string SignUsersSign { get; set; } // SignUsersSign
    }

	/// <summary>功能_会议管理_例会管理</summary>	
	[Description("功能_会议管理_例会管理")]
    public partial class T_M_WeekConference : Formula.BaseModel
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
		/// <summary>例会时间</summary>	
		[Description("例会时间")]
        public DateTime? ConferenceDate { get; set; } // ConferenceDate
		/// <summary>会议地点</summary>	
		[Description("会议地点")]
        public string MeetingAddress { get; set; } // MeetingAddress
		/// <summary>会议记录人</summary>	
		[Description("会议记录人")]
        public string ConferenceRecorder { get; set; } // ConferenceRecorder
		/// <summary>会议记录人名称</summary>	
		[Description("会议记录人名称")]
        public string ConferenceRecorderName { get; set; } // ConferenceRecorderName
		/// <summary>参会人员</summary>	
		[Description("参会人员")]
        public string JoinUsers { get; set; } // JoinUsers
		/// <summary>参会人员名称</summary>	
		[Description("参会人员名称")]
        public string JoinUsersName { get; set; } // JoinUsersName

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<T_M_WeekConference_KeyItems> T_M_WeekConference_KeyItems { get { onT_M_WeekConference_KeyItemsGetting(); return _T_M_WeekConference_KeyItems;} }
		private ICollection<T_M_WeekConference_KeyItems> _T_M_WeekConference_KeyItems;
		partial void onT_M_WeekConference_KeyItemsGetting();


        public T_M_WeekConference()
        {
            _T_M_WeekConference_KeyItems = new List<T_M_WeekConference_KeyItems>();
        }
    }

	/// <summary>重点事项</summary>	
	[Description("重点事项")]
    public partial class T_M_WeekConference_KeyItems : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string T_M_WeekConferenceID { get; set; } // T_M_WeekConferenceID
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsReleased { get; set; } // IsReleased
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Person { get; set; } // Person
		/// <summary>姓名名称</summary>	
		[Description("姓名名称")]
        public string PersonName { get; set; } // PersonName
		/// <summary>项目名称</summary>	
		[Description("项目名称")]
        public string Project { get; set; } // Project
		/// <summary>项目名称名称</summary>	
		[Description("项目名称名称")]
        public string ProjectName { get; set; } // ProjectName
		/// <summary>事宜</summary>	
		[Description("事宜")]
        public string Item { get; set; } // Item
		/// <summary>进展</summary>	
		[Description("进展")]
        public string Progress { get; set; } // Progress
		/// <summary>截止日期</summary>	
		[Description("截止日期")]
        public DateTime? EndDate { get; set; } // EndDate
		/// <summary>例会时间</summary>	
		[Description("例会时间")]
        public DateTime? ConferenceDate { get; set; } // ConferenceDate

        // Foreign keys
		[JsonIgnore]
        public virtual T_M_WeekConference T_M_WeekConference { get; set; } //  T_M_WeekConferenceID - FK_T_M_WeekConference_KeyItems_T_M_WeekConference
    }

	/// <summary>流程_印章管理_印章废止</summary>	
	[Description("流程_印章管理_印章废止")]
    public partial class T_SealManage_SealAbolish : Formula.BaseModel
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
		/// <summary>印章保管部门</summary>	
		[Description("印章保管部门")]
        public string Dept { get; set; } // Dept
		/// <summary>印章保管部门名称</summary>	
		[Description("印章保管部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>废止原因</summary>	
		[Description("废止原因")]
        public string AbolishReason { get; set; } // AbolishReason
		/// <summary>部门负责人意见</summary>	
		[Description("部门负责人意见")]
        public string DeptSign { get; set; } // DeptSign
		/// <summary>董事长意见</summary>	
		[Description("董事长意见")]
        public string BossSign { get; set; } // BossSign
		/// <summary>公共信息部意见</summary>	
		[Description("公共信息部意见")]
        public string PublicInformationDeptSign { get; set; } // PublicInformationDeptSign
		/// <summary>印章类别</summary>	
		[Description("印章类别")]
        public string SealCategory { get; set; } // SealCategory
		/// <summary>印章主键</summary>	
		[Description("印章主键")]
        public string SealPrimaryKey { get; set; } // SealPrimaryKey
		/// <summary>当前部门</summary>	
		[Description("当前部门")]
        public string CurrentDept { get; set; } // CurrentDept
		/// <summary>当前部门名称</summary>	
		[Description("当前部门名称")]
        public string CurrentDeptName { get; set; } // CurrentDeptName
		/// <summary>废止印章全称</summary>	
		[Description("废止印章全称")]
        public string AbolishSeal { get; set; } // AbolishSeal
		/// <summary>废止印章全称名称</summary>	
		[Description("废止印章全称名称")]
        public string AbolishSealName { get; set; } // AbolishSealName
		/// <summary>印章编号</summary>	
		[Description("印章编号")]
        public string Code { get; set; } // Code
		/// <summary>废止日期</summary>	
		[Description("废止日期")]
        public DateTime? AbolishDate { get; set; } // AbolishDate
    }

	/// <summary>流程_印章管理_印章借用</summary>	
	[Description("流程_印章管理_印章借用")]
    public partial class T_SealManage_SealBorrow : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>借用范畴</summary>	
		[Description("借用范畴")]
        public string BorrowRange { get; set; } // BorrowRange
		/// <summary>申请事由</summary>	
		[Description("申请事由")]
        public string ApplyReason { get; set; } // ApplyReason
		/// <summary>借用时间（起）</summary>	
		[Description("借用时间（起）")]
        public DateTime? BorrowStartTime { get; set; } // BorrowStartTime
		/// <summary>借用时间（止）</summary>	
		[Description("借用时间（止）")]
        public DateTime? BorrowEndTime { get; set; } // BorrowEndTime
		/// <summary>文件名称</summary>	
		[Description("文件名称")]
        public string FileName { get; set; } // FileName
		/// <summary>印章名称名称</summary>	
		[Description("印章名称名称")]
        public string SealName { get; set; } // SealName
		/// <summary>是否归还</summary>	
		[Description("是否归还")]
        public string IsReback { get; set; } // IsReback
		/// <summary>印章名称</summary>	
		[Description("印章名称")]
        public string Seal { get; set; } // Seal
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>部门总监审批</summary>	
		[Description("部门总监审批")]
        public string DeptDirectorSign { get; set; } // DeptDirectorSign
		/// <summary></summary>	
		[Description("")]
        public DateTime? ReturnTime { get; set; } // ReturnTime
    }

	/// <summary>流程_印章管理_印章刻制（更换）</summary>	
	[Description("流程_印章管理_印章刻制（更换）")]
    public partial class T_SealManage_SealEngraveAndChange : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请事由</summary>	
		[Description("申请事由")]
        public string ApplyReason { get; set; } // ApplyReason
		/// <summary>刻印数量（枚）</summary>	
		[Description("刻印数量（枚）")]
        public int? EngraveAmount { get; set; } // EngraveAmount
		/// <summary>备注</summary>	
		[Description("备注")]
        public string Remark { get; set; } // Remark
		/// <summary>部门总监意见</summary>	
		[Description("部门总监意见")]
        public string DeptSign { get; set; } // DeptSign
		/// <summary>总经理意见</summary>	
		[Description("总经理意见")]
        public string ApproveSign { get; set; } // ApproveSign
		/// <summary>印章全称</summary>	
		[Description("印章全称")]
        public string SealFull { get; set; } // SealFull
		/// <summary>印章形状</summary>	
		[Description("印章形状")]
        public string Shape { get; set; } // Shape
		/// <summary>申请日期</summary>	
		[Description("申请日期")]
        public DateTime? ApplyDate { get; set; } // ApplyDate
		/// <summary>印章全称名称</summary>	
		[Description("印章全称名称")]
        public string SealFullName { get; set; } // SealFullName
		/// <summary>公共信息部意见</summary>	
		[Description("公共信息部意见")]
        public string PublicInformationDeptSign { get; set; } // PublicInformationDeptSign
    }

	/// <summary>流程_印章管理_印章移交</summary>	
	[Description("流程_印章管理_印章移交")]
    public partial class T_SealManage_SealTurnOver : Formula.BaseModel
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
		/// <summary>印章名称名称</summary>	
		[Description("印章名称名称")]
        public string SealName { get; set; } // SealName
		/// <summary>移交原因</summary>	
		[Description("移交原因")]
        public string TurnOverReason { get; set; } // TurnOverReason
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Accessory { get; set; } // Accessory
		/// <summary>接收人</summary>	
		[Description("接收人")]
        public string Receiver { get; set; } // Receiver
		/// <summary>接收人名称</summary>	
		[Description("接收人名称")]
        public string ReceiverName { get; set; } // ReceiverName
		/// <summary>移交人</summary>	
		[Description("移交人")]
        public string PersonTurnOver { get; set; } // PersonTurnOver
		/// <summary>移交人名称</summary>	
		[Description("移交人名称")]
        public string PersonTurnOverName { get; set; } // PersonTurnOverName
		/// <summary>接收人签字</summary>	
		[Description("接收人签字")]
        public string PersonTurnOverSign { get; set; } // PersonTurnOverSign
		/// <summary>印章主键</summary>	
		[Description("印章主键")]
        public string SealPrimaryKey { get; set; } // SealPrimaryKey
		/// <summary>接收人部门ID</summary>	
		[Description("接收人部门ID")]
        public string ReceiveDeptID { get; set; } // ReceiveDeptID
		/// <summary>接收人部门</summary>	
		[Description("接收人部门")]
        public string ReceiveDept { get; set; } // ReceiveDept
		/// <summary>印章名称</summary>	
		[Description("印章名称")]
        public string Seal { get; set; } // Seal
		/// <summary>印章编号</summary>	
		[Description("印章编号")]
        public string Code { get; set; } // Code
    }

	/// <summary>流程_印章管理_用印申请</summary>	
	[Description("流程_印章管理_用印申请")]
    public partial class T_SealManage_UseSealApply : Formula.BaseModel
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
		/// <summary>部门</summary>	
		[Description("部门")]
        public string Dept { get; set; } // Dept
		/// <summary>部门名称</summary>	
		[Description("部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary>申请人</summary>	
		[Description("申请人")]
        public string ApplyUser { get; set; } // ApplyUser
		/// <summary>申请人名称</summary>	
		[Description("申请人名称")]
        public string ApplyUserName { get; set; } // ApplyUserName
		/// <summary>申请事由</summary>	
		[Description("申请事由")]
        public string ApplyReason { get; set; } // ApplyReason
		/// <summary>对公或对私</summary>	
		[Description("对公或对私")]
        public string PublicOrPrivate { get; set; } // PublicOrPrivate
		/// <summary>用印种类</summary>	
		[Description("用印种类")]
        public string UseSealCategory { get; set; } // UseSealCategory
		/// <summary>印章全称名称</summary>	
		[Description("印章全称名称")]
        public string SealFullName { get; set; } // SealFullName
		/// <summary>申请人职位</summary>	
		[Description("申请人职位")]
        public string ApplyUserPosition { get; set; } // ApplyUserPosition
		/// <summary>印章全称</summary>	
		[Description("印章全称")]
        public string SealFull { get; set; } // SealFull
		/// <summary>用印日期</summary>	
		[Description("用印日期")]
        public DateTime? UseDate { get; set; } // UseDate
    }

	/// <summary>功能_培训管理_员工培训管理</summary>	
	[Description("功能_培训管理_员工培训管理")]
    public partial class T_TrainManage_TrainersManage : Formula.BaseModel
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
		/// <summary>培训名称</summary>	
		[Description("培训名称")]
        public string TrainName { get; set; } // TrainName
		/// <summary>培训类别</summary>	
		[Description("培训类别")]
        public string TrainCategory { get; set; } // TrainCategory
		/// <summary>开始时间</summary>	
		[Description("开始时间")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary>结束时间</summary>	
		[Description("结束时间")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary>培训地点</summary>	
		[Description("培训地点")]
        public string TrainAddress { get; set; } // TrainAddress
		/// <summary>讲师</summary>	
		[Description("讲师")]
        public string TrainTeacher { get; set; } // TrainTeacher
		/// <summary>讲师名称</summary>	
		[Description("讲师名称")]
        public string TrainTeacherName { get; set; } // TrainTeacherName
		/// <summary>参加人员</summary>	
		[Description("参加人员")]
        public string Trainers { get; set; } // Trainers
		/// <summary>参加人员名称</summary>	
		[Description("参加人员名称")]
        public string TrainersName { get; set; } // TrainersName
		/// <summary>说明</summary>	
		[Description("说明")]
        public string Remark { get; set; } // Remark
		/// <summary>附件</summary>	
		[Description("附件")]
        public string Accessory { get; set; } // Accessory
		/// <summary>培训费用</summary>	
		[Description("培训费用")]
        public string PXFY { get; set; } // PXFY
		/// <summary>满意度</summary>	
		[Description("满意度")]
        public string MYD { get; set; } // MYD
    }


    // ************************************************************************
    // POCO Configuration

    // R_G_GoodsReport
    internal partial class R_G_GoodsReportConfiguration : EntityTypeConfiguration<R_G_GoodsReport>
    {
        public R_G_GoodsReportConfiguration()
        {
			ToTable("R_G_GOODSREPORT");
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
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.InitialCount).HasColumnName("INITIALCOUNT").IsOptional();
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.StockCount).HasColumnName("STOCKCOUNT").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.GoodsID).HasColumnName("GOODSID").IsOptional().HasMaxLength(200);
            Property(x => x.BelongYear).HasColumnName("BELONGYEAR").IsOptional();
            Property(x => x.BelongMonth).HasColumnName("BELONGMONTH").IsOptional();
        }
    }

    // R_G_GoodsReport_ApplyQuantity
    internal partial class R_G_GoodsReport_ApplyQuantityConfiguration : EntityTypeConfiguration<R_G_GoodsReport_ApplyQuantity>
    {
        public R_G_GoodsReport_ApplyQuantityConfiguration()
        {
			ToTable("R_G_GOODSREPORT_APPLYQUANTITY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.R_G_GoodsReportID).HasColumnName("R_G_GOODSREPORTID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.ApplyDept).HasColumnName("APPLYDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDeptName).HasColumnName("APPLYDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyQuantity).HasColumnName("APPLYQUANTITY").IsOptional();

            // Foreign keys
            HasOptional(a => a.R_G_GoodsReport).WithMany(b => b.R_G_GoodsReport_ApplyQuantity).HasForeignKey(c => c.R_G_GoodsReportID); // FK_R_G_GoodsReport_ApplyQuantity_R_G_GoodsReport
        }
    }

    // S_A_AttendanceInfo
    internal partial class S_A_AttendanceInfoConfiguration : EntityTypeConfiguration<S_A_AttendanceInfo>
    {
        public S_A_AttendanceInfoConfiguration()
        {
			ToTable("S_A_ATTENDANCEINFO");
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
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Person).HasColumnName("PERSON").IsOptional().HasMaxLength(200);
            Property(x => x.PersonName).HasColumnName("PERSONNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Month).HasColumnName("MONTH").IsOptional();
            Property(x => x.Date).HasColumnName("DATE").IsOptional();
            Property(x => x.Morning).HasColumnName("MORNING").IsOptional().HasMaxLength(200);
            Property(x => x.PostTime).HasColumnName("POSTTIME").IsOptional();
            Property(x => x.Afternoon).HasColumnName("AFTERNOON").IsOptional().HasMaxLength(200);
            Property(x => x.LeaveTime).HasColumnName("LEAVETIME").IsOptional();
            Property(x => x.Time).HasColumnName("TIME").IsOptional();
            Property(x => x.MorningType).HasColumnName("MORNINGTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.AfternoonType).HasColumnName("AFTERNOONTYPE").IsOptional().HasMaxLength(200);
        }
    }

    // S_C_Certificate
    internal partial class S_C_CertificateConfiguration : EntityTypeConfiguration<S_C_Certificate>
    {
        public S_C_CertificateConfiguration()
        {
			ToTable("S_C_CERTIFICATE");
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
            Property(x => x.OrdinalNumber).HasColumnName("ORDINALNUMBER").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateLevel).HasColumnName("CERTIFICATELEVEL").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.HoldDept).HasColumnName("HOLDDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.HoldDeptName).HasColumnName("HOLDDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.IssuingAuthority).HasColumnName("ISSUINGAUTHORITY").IsOptional().HasMaxLength(200);
            Property(x => x.DepositoryUnit).HasColumnName("DEPOSITORYUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateType).HasColumnName("CERTIFICATETYPE").IsOptional().HasMaxLength(200);
            Property(x => x.OriginalNum).HasColumnName("ORIGINALNUM").IsOptional();
            Property(x => x.CopyNum).HasColumnName("COPYNUM").IsOptional();
            Property(x => x.IssueDate).HasColumnName("ISSUEDATE").IsOptional();
            Property(x => x.InspectionDate).HasColumnName("INSPECTIONDATE").IsOptional();
            Property(x => x.ValidStart).HasColumnName("VALIDSTART").IsOptional();
            Property(x => x.ValidEnd).HasColumnName("VALIDEND").IsOptional();
            Property(x => x.DeclareDate).HasColumnName("DECLAREDATE").IsOptional();
            Property(x => x.WarningDate).HasColumnName("WARNINGDATE").IsOptional();
            Property(x => x.BusinessRange).HasColumnName("BUSINESSRANGE").IsOptional().HasMaxLength(500);
            Property(x => x.ConfigureDemand).HasColumnName("CONFIGUREDEMAND").IsOptional().HasMaxLength(500);
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.ExplainAttachment).HasColumnName("EXPLAINATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CertificateState).HasColumnName("CERTIFICATESTATE").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowUser).HasColumnName("BORROWUSER").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowUserName).HasColumnName("BORROWUSERNAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_C_Certificate_ApplyLog
    internal partial class S_C_Certificate_ApplyLogConfiguration : EntityTypeConfiguration<S_C_Certificate_ApplyLog>
    {
        public S_C_Certificate_ApplyLogConfiguration()
        {
			ToTable("S_C_CERTIFICATE_APPLYLOG");
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
            Property(x => x.Certificate).HasColumnName("CERTIFICATE").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateName).HasColumnName("CERTIFICATENAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowUser).HasColumnName("BORROWUSER").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowUserName).HasColumnName("BORROWUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowDept).HasColumnName("BORROWDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowDeptName).HasColumnName("BORROWDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.BorrowDate).HasColumnName("BORROWDATE").IsOptional();
            Property(x => x.PlanReturnDate).HasColumnName("PLANRETURNDATE").IsOptional();
            Property(x => x.CertificatePurpose).HasColumnName("CERTIFICATEPURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.IsReturn).HasColumnName("ISRETURN").IsOptional().HasMaxLength(200);
            Property(x => x.ReturnDate).HasColumnName("RETURNDATE").IsOptional();
            Property(x => x.CertificateBorrowID).HasColumnName("CERTIFICATEBORROWID").IsOptional().HasMaxLength(200);
        }
    }

    // S_G_GoodsAdditional
    internal partial class S_G_GoodsAdditionalConfiguration : EntityTypeConfiguration<S_G_GoodsAdditional>
    {
        public S_G_GoodsAdditionalConfiguration()
        {
			ToTable("S_G_GOODSADDITIONAL");
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
            Property(x => x.Goods).HasColumnName("GOODS").IsOptional().HasMaxLength(200);
            Property(x => x.GoodsName).HasColumnName("GOODSNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Quantity).HasColumnName("QUANTITY").IsOptional();
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.AdditionalData).HasColumnName("ADDITIONALDATA").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_G_GoodsInfo
    internal partial class S_G_GoodsInfoConfiguration : EntityTypeConfiguration<S_G_GoodsInfo>
    {
        public S_G_GoodsInfoConfiguration()
        {
			ToTable("S_G_GOODSINFO");
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
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Quantity).HasColumnName("QUANTITY").IsOptional();
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_HR_Employee
    internal partial class S_HR_EmployeeConfiguration : EntityTypeConfiguration<S_HR_Employee>
    {
        public S_HR_EmployeeConfiguration()
        {
			ToTable("S_HR_EMPLOYEE");
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
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.OldName).HasColumnName("OLDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Nation).HasColumnName("NATION").IsOptional().HasMaxLength(50);
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(50);
            Property(x => x.MobilePhone).HasColumnName("MOBILEPHONE").IsOptional().HasMaxLength(50);
            Property(x => x.HomePhone).HasColumnName("HOMEPHONE").IsOptional().HasMaxLength(50);
            Property(x => x.OfficePhone).HasColumnName("OFFICEPHONE").IsOptional().HasMaxLength(50);
            Property(x => x.Email).HasColumnName("EMAIL").IsOptional().HasMaxLength(200);
            Property(x => x.Address).HasColumnName("ADDRESS").IsOptional().HasMaxLength(500);
            Property(x => x.Political).HasColumnName("POLITICAL").IsOptional().HasMaxLength(50);
            Property(x => x.IdentityCardCode).HasColumnName("IDENTITYCARDCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Birthday).HasColumnName("BIRTHDAY").IsOptional();
            Property(x => x.NativePlace).HasColumnName("NATIVEPLACE").IsOptional().HasMaxLength(50);
            Property(x => x.HealthStatus).HasColumnName("HEALTHSTATUS").IsOptional().HasMaxLength(50);
            Property(x => x.MaritalStatus).HasColumnName("MARITALSTATUS").IsOptional().HasMaxLength(50);
            Property(x => x.LoverName).HasColumnName("LOVERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.LoverUnit).HasColumnName("LOVERUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.FirstForeignLanguage).HasColumnName("FIRSTFOREIGNLANGUAGE").IsOptional().HasMaxLength(50);
            Property(x => x.FirstForeignLanguageLevel).HasColumnName("FIRSTFOREIGNLANGUAGELEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.TwoForeignLanguage).HasColumnName("TWOFOREIGNLANGUAGE").IsOptional().HasMaxLength(50);
            Property(x => x.TwoForeignLanguageLevel).HasColumnName("TWOFOREIGNLANGUAGELEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.JoinWorkDate).HasColumnName("JOINWORKDATE").IsOptional();
            Property(x => x.JoinCompanyDate).HasColumnName("JOINCOMPANYDATE").IsOptional();
            Property(x => x.EmployeeSource).HasColumnName("EMPLOYEESOURCE").IsOptional().HasMaxLength(50);
            Property(x => x.EmploymentWay).HasColumnName("EMPLOYMENTWAY").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeBigType).HasColumnName("EMPLOYEEBIGTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeSmallType).HasColumnName("EMPLOYEESMALLTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.EngageMajor).HasColumnName("ENGAGEMAJOR").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Post).HasColumnName("POST").IsOptional();
            Property(x => x.PostLevel).HasColumnName("POSTLEVEL").IsOptional();
            Property(x => x.Educational).HasColumnName("EDUCATIONAL").IsOptional().HasMaxLength(50);
            Property(x => x.EducationalMajor).HasColumnName("EDUCATIONALMAJOR").IsOptional().HasMaxLength(200);
            Property(x => x.ContractType).HasColumnName("CONTRACTTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.PositionalTitles).HasColumnName("POSITIONALTITLES").IsOptional().HasMaxLength(50);
            Property(x => x.DeterminePostsDate).HasColumnName("DETERMINEPOSTSDATE").IsOptional();
            Property(x => x.IsHaveAccount).HasColumnName("ISHAVEACCOUNT").IsOptional().HasMaxLength(200);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.Portrait).HasColumnName("PORTRAIT").IsOptional().HasMaxLength(2147483647);
            Property(x => x.SignImage).HasColumnName("SIGNIMAGE").IsOptional().HasMaxLength(2147483647);
            Property(x => x.IdentityCardFace).HasColumnName("IDENTITYCARDFACE").IsOptional().HasMaxLength(2147483647);
            Property(x => x.IdentityCardBack).HasColumnName("IDENTITYCARDBACK").IsOptional().HasMaxLength(2147483647);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.EmployeeState).HasColumnName("EMPLOYEESTATE").IsOptional().HasMaxLength(200);
            Property(x => x.ParttimeDept).HasColumnName("PARTTIMEDEPT").IsOptional().HasMaxLength(500);
            Property(x => x.ParttimeDeptName).HasColumnName("PARTTIMEDEPTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.AnnualHolidays).HasColumnName("ANNUALHOLIDAYS").IsOptional();
        }
    }

    // S_HR_EmployeeAcademicDegree
    internal partial class S_HR_EmployeeAcademicDegreeConfiguration : EntityTypeConfiguration<S_HR_EmployeeAcademicDegree>
    {
        public S_HR_EmployeeAcademicDegreeConfiguration()
        {
			ToTable("S_HR_EMPLOYEEACADEMICDEGREE");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.EntrancelDate).HasColumnName("ENTRANCELDATE").IsOptional();
            Property(x => x.GraduationDate).HasColumnName("GRADUATIONDATE").IsOptional();
            Property(x => x.School).HasColumnName("SCHOOL").IsOptional().HasMaxLength(50);
            Property(x => x.FirstProfession).HasColumnName("FIRSTPROFESSION").IsOptional().HasMaxLength(50);
            Property(x => x.TwoProfession).HasColumnName("TWOPROFESSION").IsOptional().HasMaxLength(50);
            Property(x => x.SchoolingLength).HasColumnName("SCHOOLINGLENGTH").IsOptional().HasMaxLength(50);
            Property(x => x.SchoolShape).HasColumnName("SCHOOLSHAPE").IsOptional().HasMaxLength(50);
            Property(x => x.Degree).HasColumnName("DEGREE").IsOptional().HasMaxLength(50);
            Property(x => x.Education).HasColumnName("EDUCATION").IsOptional().HasMaxLength(50);
            Property(x => x.DegreeGiveDate).HasColumnName("DEGREEGIVEDATE").IsOptional();
            Property(x => x.DegreeGiveCountry).HasColumnName("DEGREEGIVECOUNTRY").IsOptional().HasMaxLength(50);
            Property(x => x.DegreeAttachment).HasColumnName("DEGREEATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.EducationAttachment).HasColumnName("EDUCATIONATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.IsMain).HasColumnName("ISMAIN").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_EmployeeAcademicTitle
    internal partial class S_HR_EmployeeAcademicTitleConfiguration : EntityTypeConfiguration<S_HR_EmployeeAcademicTitle>
    {
        public S_HR_EmployeeAcademicTitleConfiguration()
        {
			ToTable("S_HR_EMPLOYEEACADEMICTITLE");
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
            Property(x => x.Level).HasColumnName("LEVEL").IsOptional().HasMaxLength(200);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Major).HasColumnName("MAJOR").IsOptional().HasMaxLength(50);
            Property(x => x.AuditDept).HasColumnName("AUDITDEPT").IsOptional().HasMaxLength(50);
            Property(x => x.CertificateNumber).HasColumnName("CERTIFICATENUMBER").IsOptional().HasMaxLength(50);
            Property(x => x.IssueDate).HasColumnName("ISSUEDATE").IsOptional();
            Property(x => x.EmployDate).HasColumnName("EMPLOYDATE").IsOptional();
            Property(x => x.Certificate).HasColumnName("CERTIFICATE").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.IsMain).HasColumnName("ISMAIN").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_EmployeeContract
    internal partial class S_HR_EmployeeContractConfiguration : EntityTypeConfiguration<S_HR_EmployeeContract>
    {
        public S_HR_EmployeeContractConfiguration()
        {
			ToTable("S_HR_EMPLOYEECONTRACT");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeName).HasColumnName("EMPLOYEENAME").IsOptional().HasMaxLength(50);
            Property(x => x.ContractCategory).HasColumnName("CONTRACTCATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.ContractShape).HasColumnName("CONTRACTSHAPE").IsOptional().HasMaxLength(50);
            Property(x => x.ContractBody).HasColumnName("CONTRACTBODY").IsOptional().HasMaxLength(200);
            Property(x => x.ContractStartDate).HasColumnName("CONTRACTSTARTDATE").IsOptional();
            Property(x => x.ContractEndDate).HasColumnName("CONTRACTENDDATE").IsOptional();
            Property(x => x.PeriodStartDate).HasColumnName("PERIODSTARTDATE").IsOptional();
            Property(x => x.PeriodEndDate).HasColumnName("PERIODENDDATE").IsOptional();
            Property(x => x.PracticeEndDate).HasColumnName("PRACTICEENDDATE").IsOptional();
            Property(x => x.ContractPeriod).HasColumnName("CONTRACTPERIOD").IsOptional().HasMaxLength(50);
            Property(x => x.PostDate).HasColumnName("POSTDATE").IsOptional();
            Property(x => x.ContractAttachment).HasColumnName("CONTRACTATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.IsConfidentialityAgreement).HasColumnName("ISCONFIDENTIALITYAGREEMENT").IsOptional().HasMaxLength(50);
            Property(x => x.ConfidentialityAgreementStartDate).HasColumnName("CONFIDENTIALITYAGREEMENTSTARTDATE").IsOptional();
            Property(x => x.ConfidentialityAgreementEndDate).HasColumnName("CONFIDENTIALITYAGREEMENTENDDATE").IsOptional();
            Property(x => x.ConfidentialityAttachment).HasColumnName("CONFIDENTIALITYATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.TrainAgreementStartDate).HasColumnName("TRAINAGREEMENTSTARTDATE").IsOptional();
            Property(x => x.TrainAgreementEndDate).HasColumnName("TRAINAGREEMENTENDDATE").IsOptional();
            Property(x => x.TrainAttachment).HasColumnName("TRAINATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.StockAgreementStartDate).HasColumnName("STOCKAGREEMENTSTARTDATE").IsOptional();
            Property(x => x.StockAgreementEndDate).HasColumnName("STOCKAGREEMENTENDDATE").IsOptional();
            Property(x => x.StockAttachment).HasColumnName("STOCKATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.SFQDJYXY).HasColumnName("SFQDJYXY").IsOptional().HasMaxLength(200);
            Property(x => x.JYXYKSSJ).HasColumnName("JYXYKSSJ").IsOptional();
            Property(x => x.JYXYJSSJ).HasColumnName("JYXYJSSJ").IsOptional();
            Property(x => x.JYJ).HasColumnName("JYJ").IsOptional().HasMaxLength(200);
            Property(x => x.JYXYFJ).HasColumnName("JYXYFJ").IsOptional().HasMaxLength(500);
        }
    }

    // S_HR_EmployeeHonour
    internal partial class S_HR_EmployeeHonourConfiguration : EntityTypeConfiguration<S_HR_EmployeeHonour>
    {
        public S_HR_EmployeeHonourConfiguration()
        {
			ToTable("S_HR_EMPLOYEEHONOUR");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.AwardName).HasColumnName("AWARDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.AwardYear).HasColumnName("AWARDYEAR").IsOptional().HasMaxLength(50);
            Property(x => x.CertificationDate).HasColumnName("CERTIFICATIONDATE").IsOptional();
            Property(x => x.CertificationUnit).HasColumnName("CERTIFICATIONUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
        }
    }

    // S_HR_EmployeeJob
    internal partial class S_HR_EmployeeJobConfiguration : EntityTypeConfiguration<S_HR_EmployeeJob>
    {
        public S_HR_EmployeeJobConfiguration()
        {
			ToTable("S_HR_EMPLOYEEJOB");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeCategory).HasColumnName("EMPLOYEECATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.JobCategory).HasColumnName("JOBCATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.Clique).HasColumnName("CLIQUE").IsOptional().HasMaxLength(50);
            Property(x => x.SubCompany).HasColumnName("SUBCOMPANY").IsOptional().HasMaxLength(50);
            Property(x => x.DeptID).HasColumnName("DEPTID").IsOptional().HasMaxLength(50);
            Property(x => x.DeptIDName).HasColumnName("DEPTIDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.JobID).HasColumnName("JOBID").IsOptional().HasMaxLength(50);
            Property(x => x.JobName).HasColumnName("JOBNAME").IsOptional().HasMaxLength(50);
            Property(x => x.IsMain).HasColumnName("ISMAIN").IsOptional().HasMaxLength(50);
            Property(x => x.Major).HasColumnName("MAJOR").IsOptional().HasMaxLength(200);
            Property(x => x.EmployDate).HasColumnName("EMPLOYDATE").IsOptional();
            Property(x => x.JobAgreeCode).HasColumnName("JOBAGREECODE").IsOptional().HasMaxLength(200);
            Property(x => x.EmployAgreeCode).HasColumnName("EMPLOYAGREECODE").IsOptional().HasMaxLength(200);
            Property(x => x.EmployEndDate).HasColumnName("EMPLOYENDDATE").IsOptional();
            Property(x => x.ClearEmployDate).HasColumnName("CLEAREMPLOYDATE").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
        }
    }

    // S_HR_EmployeeQualification
    internal partial class S_HR_EmployeeQualificationConfiguration : EntityTypeConfiguration<S_HR_EmployeeQualification>
    {
        public S_HR_EmployeeQualificationConfiguration()
        {
			ToTable("S_HR_EMPLOYEEQUALIFICATION");
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
            Property(x => x.RegisterID).HasColumnName("REGISTERID").IsOptional().HasMaxLength(50);
            Property(x => x.RegisterIDName).HasColumnName("REGISTERIDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.RegisteDate).HasColumnName("REGISTEDATE").IsOptional();
            Property(x => x.QualificationName).HasColumnName("QUALIFICATIONNAME").IsOptional().HasMaxLength(200);
            Property(x => x.QualificationCode).HasColumnName("QUALIFICATIONCODE").IsOptional().HasMaxLength(200);
            Property(x => x.FirstMajor).HasColumnName("FIRSTMAJOR").IsOptional().HasMaxLength(200);
            Property(x => x.TwoMajor).HasColumnName("TWOMAJOR").IsOptional().HasMaxLength(200);
            Property(x => x.QualificationIssueDate).HasColumnName("QUALIFICATIONISSUEDATE").IsOptional();
            Property(x => x.QualificationKeeperID).HasColumnName("QUALIFICATIONKEEPERID").IsOptional().HasMaxLength(50);
            Property(x => x.QualificationKeeperIDName).HasColumnName("QUALIFICATIONKEEPERIDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.RegisteCode).HasColumnName("REGISTECODE").IsOptional().HasMaxLength(200);
            Property(x => x.RegisteIssueDate).HasColumnName("REGISTEISSUEDATE").IsOptional();
            Property(x => x.RegistelLoseDate).HasColumnName("REGISTELLOSEDATE").IsOptional();
            Property(x => x.RegisteKeeperID).HasColumnName("REGISTEKEEPERID").IsOptional().HasMaxLength(50);
            Property(x => x.RegisteKeeperIDName).HasColumnName("REGISTEKEEPERIDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SealCode).HasColumnName("SEALCODE").IsOptional().HasMaxLength(50);
            Property(x => x.SealLoseDate).HasColumnName("SEALLOSEDATE").IsOptional();
            Property(x => x.SealKeeperID).HasColumnName("SEALKEEPERID").IsOptional().HasMaxLength(50);
            Property(x => x.SealKeeperIDName).HasColumnName("SEALKEEPERIDNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ContinueTrainDate).HasColumnName("CONTINUETRAINDATE").IsOptional();
            Property(x => x.ContinueTrainLength).HasColumnName("CONTINUETRAINLENGTH").IsOptional();
            Property(x => x.ContinueTrainCompleteLength).HasColumnName("CONTINUETRAINCOMPLETELENGTH").IsOptional();
            Property(x => x.QualificationAttachment).HasColumnName("QUALIFICATIONATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.RegisteAttachment).HasColumnName("REGISTEATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_EmployeeRetired
    internal partial class S_HR_EmployeeRetiredConfiguration : EntityTypeConfiguration<S_HR_EmployeeRetired>
    {
        public S_HR_EmployeeRetiredConfiguration()
        {
			ToTable("S_HR_EMPLOYEERETIRED");
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
            Property(x => x.EmployeeCode).HasColumnName("EMPLOYEECODE").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeName).HasColumnName("EMPLOYEENAME").IsOptional().HasMaxLength(50);
            Property(x => x.RetiredDate).HasColumnName("RETIREDDATE").IsOptional();
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Direction).HasColumnName("DIRECTION").IsOptional().HasMaxLength(200);
            Property(x => x.Reason).HasColumnName("REASON").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_EmployeeWorkHistory
    internal partial class S_HR_EmployeeWorkHistoryConfiguration : EntityTypeConfiguration<S_HR_EmployeeWorkHistory>
    {
        public S_HR_EmployeeWorkHistoryConfiguration()
        {
			ToTable("S_HR_EMPLOYEEWORKHISTORY");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(200);
            Property(x => x.JoinCompanyDate).HasColumnName("JOINCOMPANYDATE").IsOptional();
            Property(x => x.LeaveCompanyDate).HasColumnName("LEAVECOMPANYDATE").IsOptional();
            Property(x => x.CompanyName).HasColumnName("COMPANYNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DeptAndPost).HasColumnName("DEPTANDPOST").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Achievement).HasColumnName("ACHIEVEMENT").IsOptional().HasMaxLength(500);
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
        }
    }

    // S_HR_EmployeeWorkPerformance
    internal partial class S_HR_EmployeeWorkPerformanceConfiguration : EntityTypeConfiguration<S_HR_EmployeeWorkPerformance>
    {
        public S_HR_EmployeeWorkPerformanceConfiguration()
        {
			ToTable("S_HR_EMPLOYEEWORKPERFORMANCE");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectClass).HasColumnName("PROJECTCLASS").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectLevel).HasColumnName("PROJECTLEVEL").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectState).HasColumnName("PROJECTSTATE").IsOptional().HasMaxLength(200);
            Property(x => x.PlanStartDate).HasColumnName("PLANSTARTDATE").IsOptional();
            Property(x => x.FactFinishDate).HasColumnName("FACTFINISHDATE").IsOptional();
            Property(x => x.ProjectDescription).HasColumnName("PROJECTDESCRIPTION").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(200);
            Property(x => x.RelateID).HasColumnName("RELATEID").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectRole).HasColumnName("PROJECTROLE").IsOptional().HasMaxLength(200);
        }
    }

    // S_HR_EmployeeWorkPost
    internal partial class S_HR_EmployeeWorkPostConfiguration : EntityTypeConfiguration<S_HR_EmployeeWorkPost>
    {
        public S_HR_EmployeeWorkPostConfiguration()
        {
			ToTable("S_HR_EMPLOYEEWORKPOST");
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
            Property(x => x.Post).HasColumnName("POST").IsOptional().HasMaxLength(50);
            Property(x => x.PostName).HasColumnName("POSTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.PostLevel).HasColumnName("POSTLEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.EffectiveDate).HasColumnName("EFFECTIVEDATE").IsOptional();
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.IsMain).HasColumnName("ISMAIN").IsOptional().HasMaxLength(50);
        }
    }

    // S_HR_UserCostInfo
    internal partial class S_HR_UserCostInfoConfiguration : EntityTypeConfiguration<S_HR_UserCostInfo>
    {
        public S_HR_UserCostInfoConfiguration()
        {
			ToTable("S_HR_USERCOSTINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.StartDate).HasColumnName("STARTDATE").IsRequired();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
        }
    }

    // S_HR_UserUnitPrice
    internal partial class S_HR_UserUnitPriceConfiguration : EntityTypeConfiguration<S_HR_UserUnitPrice>
    {
        public S_HR_UserUnitPriceConfiguration()
        {
			ToTable("S_HR_USERUNITPRICE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.UserLevel).HasColumnName("USERLEVEL").IsOptional().HasMaxLength(50);
            Property(x => x.UnitPrice).HasColumnName("UNITPRICE").IsRequired().HasPrecision(18,2);
            Property(x => x.CostInfoID).HasColumnName("COSTINFOID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_HR_UserCostInfo).WithMany(b => b.S_HR_UserUnitPrice).HasForeignKey(c => c.CostInfoID); // FK_S_HR_UserUnitPrice_S_HR_UserCostInfo
        }
    }

    // S_I_InstrumentInfo
    internal partial class S_I_InstrumentInfoConfiguration : EntityTypeConfiguration<S_I_InstrumentInfo>
    {
        public S_I_InstrumentInfoConfiguration()
        {
			ToTable("S_I_INSTRUMENTINFO");
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
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.RegisterUser).HasColumnName("REGISTERUSER").IsOptional().HasMaxLength(200);
            Property(x => x.RegisterUserName).HasColumnName("REGISTERUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RegisterDate).HasColumnName("REGISTERDATE").IsOptional();
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Spec).HasColumnName("SPEC").IsOptional().HasMaxLength(200);
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Price).HasColumnName("PRICE").IsOptional().HasPrecision(18,2);
            Property(x => x.SupplierName).HasColumnName("SUPPLIERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Warehousing).HasColumnName("WAREHOUSING").IsOptional().HasMaxLength(200);
            Property(x => x.PurchaseDate).HasColumnName("PURCHASEDATE").IsOptional();
            Property(x => x.DepreciableLives).HasColumnName("DEPRECIABLELIVES").IsOptional();
            Property(x => x.DisCardDate).HasColumnName("DISCARDDATE").IsOptional();
            Property(x => x.PhysicalState).HasColumnName("PHYSICALSTATE").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_M_ConferenceRoom
    internal partial class S_M_ConferenceRoomConfiguration : EntityTypeConfiguration<S_M_ConferenceRoom>
    {
        public S_M_ConferenceRoomConfiguration()
        {
			ToTable("S_M_CONFERENCEROOM");
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
            Property(x => x.RoomName).HasColumnName("ROOMNAME").IsOptional().HasMaxLength(200);
            Property(x => x.LinkName).HasColumnName("LINKNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Capacity).HasColumnName("CAPACITY").IsOptional().HasMaxLength(200);
            Property(x => x.ManageDeptID).HasColumnName("MANAGEDEPTID").IsOptional().HasMaxLength(200);
            Property(x => x.ManageDeptIDName).HasColumnName("MANAGEDEPTIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RoomAddress).HasColumnName("ROOMADDRESS").IsOptional().HasMaxLength(200);
            Property(x => x.Configuredevice).HasColumnName("CONFIGUREDEVICE").IsOptional().HasMaxLength(500);
        }
    }

    // S_SealManage_SealInfo
    internal partial class S_SealManage_SealInfoConfiguration : EntityTypeConfiguration<S_SealManage_SealInfo>
    {
        public S_SealManage_SealInfoConfiguration()
        {
			ToTable("S_SEALMANAGE_SEALINFO");
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
            Property(x => x.BeginUseTime).HasColumnName("BEGINUSETIME").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.Picture).HasColumnName("PICTURE").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.KeepDept).HasColumnName("KEEPDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.KeepDeptName).HasColumnName("KEEPDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Keeper).HasColumnName("KEEPER").IsOptional().HasMaxLength(200);
            Property(x => x.KeeperName).HasColumnName("KEEPERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.Purpose).HasColumnName("PURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_W_ProjectInfo
    internal partial class S_W_ProjectInfoConfiguration : EntityTypeConfiguration<S_W_ProjectInfo>
    {
        public S_W_ProjectInfoConfiguration()
        {
			ToTable("S_W_PROJECTINFO");
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
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectCode).HasColumnName("PROJECTCODE").IsOptional().HasMaxLength(200);
            Property(x => x.ChargerDept).HasColumnName("CHARGERDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.ChargerDeptName).HasColumnName("CHARGERDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ChargerUser).HasColumnName("CHARGERUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ChargerUserName).HasColumnName("CHARGERUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.WorkHourType).HasColumnName("WORKHOURTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.IsValid).HasColumnName("ISVALID").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
        }
    }

    // S_W_UserWorkHour
    internal partial class S_W_UserWorkHourConfiguration : EntityTypeConfiguration<S_W_UserWorkHour>
    {
        public S_W_UserWorkHourConfiguration()
        {
			ToTable("S_W_USERWORKHOUR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.UserCode).HasColumnName("USERCODE").IsOptional().HasMaxLength(50);
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.UserDeptID).HasColumnName("USERDEPTID").IsOptional().HasMaxLength(50);
            Property(x => x.UserDeptName).HasColumnName("USERDEPTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkHourDate).HasColumnName("WORKHOURDATE").IsRequired();
            Property(x => x.NormalValue).HasColumnName("NORMALVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.AdditionalValue).HasColumnName("ADDITIONALVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.WorkHourValue).HasColumnName("WORKHOURVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.ConfirmValue).HasColumnName("CONFIRMVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.State).HasColumnName("STATE").IsRequired().HasMaxLength(50);
            Property(x => x.BelongYear).HasColumnName("BELONGYEAR").IsRequired();
            Property(x => x.BelongMonth).HasColumnName("BELONGMONTH").IsRequired();
            Property(x => x.BelongQuarter).HasColumnName("BELONGQUARTER").IsRequired();
            Property(x => x.WorkHourType).HasColumnName("WORKHOURTYPE").IsRequired().HasMaxLength(50);
            Property(x => x.ProjectID).HasColumnName("PROJECTID").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectCode).HasColumnName("PROJECTCODE").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectDept).HasColumnName("PROJECTDEPT").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectDeptName).HasColumnName("PROJECTDEPTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectChargerUser).HasColumnName("PROJECTCHARGERUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ProjectChargerUserName).HasColumnName("PROJECTCHARGERUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SubProjectName).HasColumnName("SUBPROJECTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.SubProjectCode).HasColumnName("SUBPROJECTCODE").IsOptional().HasMaxLength(500);
            Property(x => x.MajorCode).HasColumnName("MAJORCODE").IsOptional().HasMaxLength(500);
            Property(x => x.MajorName).HasColumnName("MAJORNAME").IsOptional().HasMaxLength(500);
            Property(x => x.TaskWorkCode).HasColumnName("TASKWORKCODE").IsOptional().HasMaxLength(500);
            Property(x => x.TaskWorkName).HasColumnName("TASKWORKNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkContent).HasColumnName("WORKCONTENT").IsOptional();
            Property(x => x.WorkHourDay).HasColumnName("WORKHOURDAY").IsOptional().HasPrecision(18,2);
            Property(x => x.ConfirmDay).HasColumnName("CONFIRMDAY").IsOptional().HasPrecision(18,2);
            Property(x => x.Step1Value).HasColumnName("STEP1VALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.Step1Day).HasColumnName("STEP1DAY").IsOptional().HasPrecision(18,2);
            Property(x => x.Step2Value).HasColumnName("STEP2VALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.Step2Day).HasColumnName("STEP2DAY").IsOptional().HasPrecision(18,2);
            Property(x => x.Step1User).HasColumnName("STEP1USER").IsOptional().HasMaxLength(200);
            Property(x => x.Step1UserName).HasColumnName("STEP1USERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Step1Date).HasColumnName("STEP1DATE").IsOptional();
            Property(x => x.IsStep1).HasColumnName("ISSTEP1").IsOptional().HasMaxLength(200);
            Property(x => x.Step2User).HasColumnName("STEP2USER").IsOptional().HasMaxLength(200);
            Property(x => x.Step2UserName).HasColumnName("STEP2USERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Step2Date).HasColumnName("STEP2DATE").IsOptional();
            Property(x => x.IsStep2).HasColumnName("ISSTEP2").IsOptional().HasMaxLength(200);
            Property(x => x.ConfirmUser).HasColumnName("CONFIRMUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ConfirmUserName).HasColumnName("CONFIRMUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ConfirmDate).HasColumnName("CONFIRMDATE").IsOptional();
            Property(x => x.IsConfirm).HasColumnName("ISCONFIRM").IsOptional().HasMaxLength(200);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.SupplementID).HasColumnName("SUPPLEMENTID").IsOptional().HasMaxLength(200);
            Property(x => x.WorkTimeMajor).HasColumnName("WORKTIMEMAJOR").IsOptional().HasMaxLength(100);
        }
    }

    // S_W_UserWorkHour_ApproveDetail
    internal partial class S_W_UserWorkHour_ApproveDetailConfiguration : EntityTypeConfiguration<S_W_UserWorkHour_ApproveDetail>
    {
        public S_W_UserWorkHour_ApproveDetailConfiguration()
        {
			ToTable("S_W_USERWORKHOUR_APPROVEDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.S_W_UserWorkHourID).HasColumnName("S_W_USERWORKHOURID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.ApproveValue).HasColumnName("APPROVEVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.ApproveDay).HasColumnName("APPROVEDAY").IsOptional().HasPrecision(18,2);
            Property(x => x.ApproveDate).HasColumnName("APPROVEDATE").IsOptional();
            Property(x => x.ApproveStep).HasColumnName("APPROVESTEP").IsOptional().HasMaxLength(200);
            Property(x => x.ApproveUser).HasColumnName("APPROVEUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApproveUserName).HasColumnName("APPROVEUSERNAME").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.S_W_UserWorkHour).WithMany(b => b.S_W_UserWorkHour_ApproveDetail).HasForeignKey(c => c.S_W_UserWorkHourID); // FK_S_W_UserWorkHour_ApproveDetail_S_W_UserWorkHour
        }
    }

    // S_W_UserWorkHourSupplement
    internal partial class S_W_UserWorkHourSupplementConfiguration : EntityTypeConfiguration<S_W_UserWorkHourSupplement>
    {
        public S_W_UserWorkHourSupplementConfiguration()
        {
			ToTable("S_W_USERWORKHOURSUPPLEMENT");
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
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(200);
            Property(x => x.UserIDName).HasColumnName("USERIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.UserDept).HasColumnName("USERDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.UserDeptName).HasColumnName("USERDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectID).HasColumnName("PROJECTID").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectIDName).HasColumnName("PROJECTIDNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkHourType).HasColumnName("WORKHOURTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.SubProjectCode).HasColumnName("SUBPROJECTCODE").IsOptional().HasMaxLength(500);
            Property(x => x.MajorCode).HasColumnName("MAJORCODE").IsOptional().HasMaxLength(500);
            Property(x => x.MajorName).HasColumnName("MAJORNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkContent).HasColumnName("WORKCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.WorkHourDateStart).HasColumnName("WORKHOURDATESTART").IsOptional();
            Property(x => x.WorkHourDateEnd).HasColumnName("WORKHOURDATEEND").IsOptional();
            Property(x => x.NormalValue).HasColumnName("NORMALVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.AdditionalValue).HasColumnName("ADDITIONALVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.WorkHourValue).HasColumnName("WORKHOURVALUE").IsOptional().HasPrecision(18,2);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.UserDeptID).HasColumnName("USERDEPTID").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectCode).HasColumnName("PROJECTCODE").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectDept).HasColumnName("PROJECTDEPT").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectDeptName).HasColumnName("PROJECTDEPTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectChargerUser).HasColumnName("PROJECTCHARGERUSER").IsOptional().HasMaxLength(500);
            Property(x => x.ProjectChargerUserName).HasColumnName("PROJECTCHARGERUSERNAME").IsOptional().HasMaxLength(500);
            Property(x => x.SubProjectName).HasColumnName("SUBPROJECTNAME").IsOptional().HasMaxLength(500);
            Property(x => x.TaskWorkCode).HasColumnName("TASKWORKCODE").IsOptional().HasMaxLength(500);
            Property(x => x.TaskWorkName).HasColumnName("TASKWORKNAME").IsOptional().HasMaxLength(500);
            Property(x => x.WorkTimeMajor).HasColumnName("WORKTIMEMAJOR").IsOptional().HasMaxLength(200);
        }
    }

    // T_C_CertificateBorrow
    internal partial class T_C_CertificateBorrowConfiguration : EntityTypeConfiguration<T_C_CertificateBorrow>
    {
        public T_C_CertificateBorrowConfiguration()
        {
			ToTable("T_C_CERTIFICATEBORROW");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDept).HasColumnName("APPLYDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDeptName).HasColumnName("APPLYDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Project).HasColumnName("PROJECT").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectCode).HasColumnName("PROJECTCODE").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.ApplyReason).HasColumnName("APPLYREASON").IsOptional().HasMaxLength(500);
            Property(x => x.CertificatePurpose).HasColumnName("CERTIFICATEPURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.BorrowDate).HasColumnName("BORROWDATE").IsOptional();
            Property(x => x.PlanReturnDate).HasColumnName("PLANRETURNDATE").IsOptional();
            Property(x => x.DeptSign).HasColumnName("DEPTSIGN").IsOptional();
            Property(x => x.PublicDeptSign).HasColumnName("PUBLICDEPTSIGN").IsOptional();
        }
    }

    // T_C_CertificateBorrow_ApplyContent
    internal partial class T_C_CertificateBorrow_ApplyContentConfiguration : EntityTypeConfiguration<T_C_CertificateBorrow_ApplyContent>
    {
        public T_C_CertificateBorrow_ApplyContentConfiguration()
        {
			ToTable("T_C_CERTIFICATEBORROW_APPLYCONTENT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_C_CertificateBorrowID).HasColumnName("T_C_CERTIFICATEBORROWID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Certificate).HasColumnName("CERTIFICATE").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateName).HasColumnName("CERTIFICATENAME").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateCode).HasColumnName("CERTIFICATECODE").IsOptional().HasMaxLength(200);
            Property(x => x.CertificateType).HasColumnName("CERTIFICATETYPE").IsOptional().HasMaxLength(200);
            Property(x => x.Counts).HasColumnName("COUNTS").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_C_CertificateBorrow).WithMany(b => b.T_C_CertificateBorrow_ApplyContent).HasForeignKey(c => c.T_C_CertificateBorrowID); // FK_T_C_CertificateBorrow_ApplyContent_T_C_CertificateBorrow
        }
    }

    // T_ceshishuangyu
    internal partial class T_ceshishuangyuConfiguration : EntityTypeConfiguration<T_ceshishuangyu>
    {
        public T_ceshishuangyuConfiguration()
        {
			ToTable("T_CESHISHUANGYU");
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
            Property(x => x.sdgawr).HasColumnName("SDGAWR").IsOptional().HasMaxLength(200);
            Property(x => x.sgeh).HasColumnName("SGEH").IsOptional().HasMaxLength(500);
            Property(x => x.haaa).HasColumnName("HAAA").IsOptional();
            Property(x => x.fghfffff).HasColumnName("FGHFFFFF").IsOptional().HasMaxLength(200);
            Property(x => x.dfgsss).HasColumnName("DFGSSS").IsOptional().HasMaxLength(200);
            Property(x => x.qqqqq).HasColumnName("QQQQQ").IsOptional().HasMaxLength(200);
            Property(x => x.wwwww).HasColumnName("WWWWW").IsOptional().HasMaxLength(200);
            Property(x => x.wwwwwName).HasColumnName("WWWWWNAME").IsOptional().HasMaxLength(200);
            Property(x => x.eeeee).HasColumnName("EEEEE").IsOptional().HasMaxLength(200);
            Property(x => x.eeeeeName).HasColumnName("EEEEENAME").IsOptional().HasMaxLength(200);
            Property(x => x.rrrrr).HasColumnName("RRRRR").IsOptional().HasMaxLength(200);
            Property(x => x.ttttt).HasColumnName("TTTTT").IsOptional().HasMaxLength(500);
            Property(x => x.yyyyy).HasColumnName("YYYYY").IsOptional().HasPrecision(18,4);
            Property(x => x.uuuuu).HasColumnName("UUUUU").IsOptional();
            Property(x => x.iiiii).HasColumnName("IIIII").IsOptional().HasMaxLength(200);
            Property(x => x.ewqtt).HasColumnName("EWQTT").IsOptional();
        }
    }

    // T_ceshishuangyu_uuuuu
    internal partial class T_ceshishuangyu_uuuuuConfiguration : EntityTypeConfiguration<T_ceshishuangyu_uuuuu>
    {
        public T_ceshishuangyu_uuuuuConfiguration()
        {
			ToTable("T_CESHISHUANGYU_UUUUU");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_ceshishuangyuID).HasColumnName("T_CESHISHUANGYUID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.sdfg).HasColumnName("SDFG").IsOptional().HasMaxLength(200);
            Property(x => x.gerg).HasColumnName("GERG").IsOptional().HasMaxLength(200);
            Property(x => x.dger).HasColumnName("DGER").IsOptional();
            Property(x => x.ergq).HasColumnName("ERGQ").IsOptional().HasMaxLength(200);
            Property(x => x.wef).HasColumnName("WEF").IsOptional().HasMaxLength(200);
            Property(x => x.wefName).HasColumnName("WEFNAME").IsOptional().HasMaxLength(200);
            Property(x => x.sdgf).HasColumnName("SDGF").IsOptional().HasMaxLength(200);
            Property(x => x.sdgfName).HasColumnName("SDGFNAME").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_ceshishuangyu).WithMany(b => b.T_ceshishuangyu_uuuuu).HasForeignKey(c => c.T_ceshishuangyuID); // FK_T_ceshishuangyu_uuuuu_T_ceshishuangyu
        }
    }

    // T_D_DeptBudget
    internal partial class T_D_DeptBudgetConfiguration : EntityTypeConfiguration<T_D_DeptBudget>
    {
        public T_D_DeptBudgetConfiguration()
        {
			ToTable("T_D_DEPTBUDGET");
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
            Property(x => x.Number).HasColumnName("NUMBER").IsOptional().HasMaxLength(200);
            Property(x => x.BudgetClass).HasColumnName("BUDGETCLASS").IsOptional().HasMaxLength(200);
            Property(x => x.BudgetType).HasColumnName("BUDGETTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.MoneyValue).HasColumnName("MONEYVALUE").IsOptional().HasPrecision(18,6);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
        }
    }

    // T_D_DeptBudgetUp
    internal partial class T_D_DeptBudgetUpConfiguration : EntityTypeConfiguration<T_D_DeptBudgetUp>
    {
        public T_D_DeptBudgetUpConfiguration()
        {
			ToTable("T_D_DEPTBUDGETUP");
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
            Property(x => x.Number).HasColumnName("NUMBER").IsOptional().HasMaxLength(200);
            Property(x => x.BudgetClass).HasColumnName("BUDGETCLASS").IsOptional().HasMaxLength(200);
            Property(x => x.BudgetType).HasColumnName("BUDGETTYPE").IsOptional().HasMaxLength(200);
            Property(x => x.MoneyValue).HasColumnName("MONEYVALUE").IsOptional().HasPrecision(18,6);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(2000);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DeptBudgetID).HasColumnName("DEPTBUDGETID").IsOptional().HasMaxLength(200);
        }
    }

    // T_EmployeePersonalRecords
    internal partial class T_EmployeePersonalRecordsConfiguration : EntityTypeConfiguration<T_EmployeePersonalRecords>
    {
        public T_EmployeePersonalRecordsConfiguration()
        {
			ToTable("T_EMPLOYEEPERSONALRECORDS");
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
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.KeepUnit).HasColumnName("KEEPUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.SourceUnit).HasColumnName("SOURCEUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.ExitUnit).HasColumnName("EXITUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.ReportCardSubDate).HasColumnName("REPORTCARDSUBDATE").IsOptional();
            Property(x => x.EnterDate).HasColumnName("ENTERDATE").IsOptional();
            Property(x => x.ExitDate).HasColumnName("EXITDATE").IsOptional();
            Property(x => x.ResidentAccountsType).HasColumnName("RESIDENTACCOUNTSTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.ResidentAccountsStreet).HasColumnName("RESIDENTACCOUNTSSTREET").IsOptional().HasMaxLength(200);
        }
    }

    // T_EmployeeSocialSecurity
    internal partial class T_EmployeeSocialSecurityConfiguration : EntityTypeConfiguration<T_EmployeeSocialSecurity>
    {
        public T_EmployeeSocialSecurityConfiguration()
        {
			ToTable("T_EMPLOYEESOCIALSECURITY");
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
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Relation).HasColumnName("RELATION").IsOptional().HasMaxLength(50);
            Property(x => x.BirthDate).HasColumnName("BIRTHDATE").IsOptional();
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(50);
            Property(x => x.WorkUnit).HasColumnName("WORKUNIT").IsOptional().HasMaxLength(50);
            Property(x => x.Job).HasColumnName("JOB").IsOptional().HasMaxLength(50);
            Property(x => x.Phone).HasColumnName("PHONE").IsOptional().HasMaxLength(50);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(50);
        }
    }

    // T_Evection_EvectionApply
    internal partial class T_Evection_EvectionApplyConfiguration : EntityTypeConfiguration<T_Evection_EvectionApply>
    {
        public T_Evection_EvectionApplyConfiguration()
        {
			ToTable("T_EVECTION_EVECTIONAPPLY");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Phone).HasColumnName("PHONE").IsOptional().HasMaxLength(50);
            Property(x => x.PMSign).HasColumnName("PMSIGN").IsOptional();
            Property(x => x.DeptSign).HasColumnName("DEPTSIGN").IsOptional();
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.Destination).HasColumnName("DESTINATION").IsOptional().HasMaxLength(200);
            Property(x => x.Target).HasColumnName("TARGET").IsOptional().HasMaxLength(500);
            Property(x => x.PM).HasColumnName("PM").IsOptional().HasMaxLength(200);
            Property(x => x.PMID).HasColumnName("PMID").IsOptional().HasMaxLength(200);
            Property(x => x.SerialNumber).HasColumnName("SERIALNUMBER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.Project).HasColumnName("PROJECT").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectID).HasColumnName("PROJECTID").IsOptional().HasMaxLength(200);
        }
    }

    // T_Evection_EvectionApply_IfAircraft
    internal partial class T_Evection_EvectionApply_IfAircraftConfiguration : EntityTypeConfiguration<T_Evection_EvectionApply_IfAircraft>
    {
        public T_Evection_EvectionApply_IfAircraftConfiguration()
        {
			ToTable("T_EVECTION_EVECTIONAPPLY_IFAIRCRAFT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_Evection_EvectionApplyID).HasColumnName("T_EVECTION_EVECTIONAPPLYID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Discount).HasColumnName("DISCOUNT").IsOptional().HasPrecision(18,2);
            Property(x => x.IfAheadThreeDays).HasColumnName("IFAHEADTHREEDAYS").IsOptional().HasMaxLength(200);
            Property(x => x.NotAheadReason).HasColumnName("NOTAHEADREASON").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_Evection_EvectionApply).WithMany(b => b.T_Evection_EvectionApply_IfAircraft).HasForeignKey(c => c.T_Evection_EvectionApplyID); // FK_T_Flow_Evection_EvectionApply_IfAircraft_T_Flow_Evection_EvectionApply
        }
    }

    // T_Evection_EvectionApply_Schedule
    internal partial class T_Evection_EvectionApply_ScheduleConfiguration : EntityTypeConfiguration<T_Evection_EvectionApply_Schedule>
    {
        public T_Evection_EvectionApply_ScheduleConfiguration()
        {
			ToTable("T_EVECTION_EVECTIONAPPLY_SCHEDULE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_Evection_EvectionApplyID).HasColumnName("T_EVECTION_EVECTIONAPPLYID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Traffic).HasColumnName("TRAFFIC").IsOptional().HasMaxLength(50);
            Property(x => x.StartDate).HasColumnName("STARTDATE").IsOptional();
            Property(x => x.StartPoint).HasColumnName("STARTPOINT").IsOptional().HasMaxLength(200);
            Property(x => x.EndPont).HasColumnName("ENDPONT").IsOptional().HasMaxLength(200);
            Property(x => x.Deadline).HasColumnName("DEADLINE").IsOptional();
            Property(x => x.PredictCost).HasColumnName("PREDICTCOST").IsOptional().HasPrecision(18,2);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.T_Evection_EvectionApply).WithMany(b => b.T_Evection_EvectionApply_Schedule).HasForeignKey(c => c.T_Evection_EvectionApplyID); // FK_T_Flow_Evection_EvectionApply_Schedule_T_Flow_Evection_EvectionApply
        }
    }

    // T_Evection_PettyCash
    internal partial class T_Evection_PettyCashConfiguration : EntityTypeConfiguration<T_Evection_PettyCash>
    {
        public T_Evection_PettyCashConfiguration()
        {
			ToTable("T_EVECTION_PETTYCASH");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDatetime).HasColumnName("APPLYDATETIME").IsOptional();
            Property(x => x.Purpose).HasColumnName("PURPOSE").IsOptional().HasMaxLength(500);
            Property(x => x.AppliedAmount).HasColumnName("APPLIEDAMOUNT").IsOptional().HasMaxLength(50);
            Property(x => x.BigAmount).HasColumnName("BIGAMOUNT").IsOptional().HasMaxLength(50);
            Property(x => x.PMSign).HasColumnName("PMSIGN").IsOptional();
            Property(x => x.FinanceSign).HasColumnName("FINANCESIGN").IsOptional();
            Property(x => x.GeneralSign).HasColumnName("GENERALSIGN").IsOptional();
            Property(x => x.ApplyTime).HasColumnName("APPLYTIME").IsOptional();
        }
    }

    // T_G_GoodsApply
    internal partial class T_G_GoodsApplyConfiguration : EntityTypeConfiguration<T_G_GoodsApply>
    {
        public T_G_GoodsApplyConfiguration()
        {
			ToTable("T_G_GOODSAPPLY");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDept).HasColumnName("APPLYDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDeptName).HasColumnName("APPLYDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.ApplySign).HasColumnName("APPLYSIGN").IsOptional();
            Property(x => x.OfficeSign).HasColumnName("OFFICESIGN").IsOptional();
        }
    }

    // T_G_GoodsApply_ApplyDetail
    internal partial class T_G_GoodsApply_ApplyDetailConfiguration : EntityTypeConfiguration<T_G_GoodsApply_ApplyDetail>
    {
        public T_G_GoodsApply_ApplyDetailConfiguration()
        {
			ToTable("T_G_GOODSAPPLY_APPLYDETAIL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_G_GoodsApplyID).HasColumnName("T_G_GOODSAPPLYID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Goods).HasColumnName("GOODS").IsOptional().HasMaxLength(200);
            Property(x => x.GoodsName).HasColumnName("GOODSNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Model).HasColumnName("MODEL").IsOptional().HasMaxLength(200);
            Property(x => x.Unit).HasColumnName("UNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Quantity).HasColumnName("QUANTITY").IsOptional();

            // Foreign keys
            HasOptional(a => a.T_G_GoodsApply).WithMany(b => b.T_G_GoodsApply_ApplyDetail).HasForeignKey(c => c.T_G_GoodsApplyID); // FK_T_G_GoodsApply_ApplyDetail_T_G_GoodsApply
        }
    }

    // T_HR_PersonalBonusInput
    internal partial class T_HR_PersonalBonusInputConfiguration : EntityTypeConfiguration<T_HR_PersonalBonusInput>
    {
        public T_HR_PersonalBonusInputConfiguration()
        {
			ToTable("T_HR_PERSONALBONUSINPUT");
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
            Property(x => x.Employee).HasColumnName("EMPLOYEE").IsOptional().HasMaxLength(200);
            Property(x => x.EmployeeName).HasColumnName("EMPLOYEENAME").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.BonusCategory).HasColumnName("BONUSCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.Batch).HasColumnName("BATCH").IsOptional().HasMaxLength(200);
            Property(x => x.SendOutMoney).HasColumnName("SENDOUTMONEY").IsOptional();
            Property(x => x.ApproveDate).HasColumnName("APPROVEDATE").IsOptional();
            Property(x => x.SendOutDate).HasColumnName("SENDOUTDATE").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.Project).HasColumnName("PROJECT").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional().HasMaxLength(200);
            Property(x => x.Month).HasColumnName("MONTH").IsOptional().HasMaxLength(200);
        }
    }

    // T_HR_SalaryManage
    internal partial class T_HR_SalaryManageConfiguration : EntityTypeConfiguration<T_HR_SalaryManage>
    {
        public T_HR_SalaryManageConfiguration()
        {
			ToTable("T_HR_SALARYMANAGE");
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
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Employee).HasColumnName("EMPLOYEE").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.Position).HasColumnName("POSITION").IsOptional().HasMaxLength(200);
            Property(x => x.Salary).HasColumnName("SALARY").IsOptional().HasPrecision(18,2);
            Property(x => x.EmployeeName).HasColumnName("EMPLOYEENAME").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Money).HasColumnName("MONEY").IsOptional().HasPrecision(18,2);
            Property(x => x.PerformanceBase).HasColumnName("PERFORMANCEBASE").IsOptional().HasPrecision(18,2);
            Property(x => x.PerformanceScore).HasColumnName("PERFORMANCESCORE").IsOptional().HasPrecision(18,2);
            Property(x => x.PerformanceSalary).HasColumnName("PERFORMANCESALARY").IsOptional().HasMaxLength(200);
            Property(x => x.HalfFullYear).HasColumnName("HALFFULLYEAR").IsOptional().HasMaxLength(200);
            Property(x => x.WholeBonus).HasColumnName("WHOLEBONUS").IsOptional().HasPrecision(18,4);
            Property(x => x.ProjectBonus).HasColumnName("PROJECTBONUS").IsOptional().HasPrecision(18,4);
            Property(x => x.SumBonus).HasColumnName("SUMBONUS").IsOptional().HasPrecision(18,4);
            Property(x => x.OvertimePay).HasColumnName("OVERTIMEPAY").IsOptional().HasPrecision(18,4);
            Property(x => x.CutPayment).HasColumnName("CUTPAYMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.Allowance).HasColumnName("ALLOWANCE").IsOptional().HasPrecision(18,4);
            Property(x => x.OtherSum).HasColumnName("OTHERSUM").IsOptional().HasPrecision(18,4);
            Property(x => x.POldAgePension).HasColumnName("POLDAGEPENSION").IsOptional().HasPrecision(18,4);
            Property(x => x.PUnemployment).HasColumnName("PUNEMPLOYMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.PMedicalTreatment).HasColumnName("PMEDICALTREATMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.PSeriousIllness).HasColumnName("PSERIOUSILLNESS").IsOptional().HasPrecision(18,4);
            Property(x => x.POccupationalInjury).HasColumnName("POCCUPATIONALINJURY").IsOptional().HasPrecision(18,4);
            Property(x => x.PBear).HasColumnName("PBEAR").IsOptional().HasPrecision(18,4);
            Property(x => x.PSocialSecurity).HasColumnName("PSOCIALSECURITY").IsOptional().HasPrecision(18,4);
            Property(x => x.PAccumulationFund).HasColumnName("PACCUMULATIONFUND").IsOptional().HasPrecision(18,4);
            Property(x => x.PSubtotal).HasColumnName("PSUBTOTAL").IsOptional().HasPrecision(18,4);
            Property(x => x.COldAgePension).HasColumnName("COLDAGEPENSION").IsOptional().HasPrecision(18,4);
            Property(x => x.CUnemployment).HasColumnName("CUNEMPLOYMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.CMedicalTreatment).HasColumnName("CMEDICALTREATMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.CSeriousIllness).HasColumnName("CSERIOUSILLNESS").IsOptional().HasPrecision(18,4);
            Property(x => x.COccupationalInjury).HasColumnName("COCCUPATIONALINJURY").IsOptional().HasPrecision(18,4);
            Property(x => x.CBear).HasColumnName("CBEAR").IsOptional().HasPrecision(18,4);
            Property(x => x.CSocialSecurity).HasColumnName("CSOCIALSECURITY").IsOptional().HasPrecision(18,4);
            Property(x => x.CAccumulationFund).HasColumnName("CACCUMULATIONFUND").IsOptional().HasPrecision(18,4);
            Property(x => x.CSubtotal).HasColumnName("CSUBTOTAL").IsOptional().HasPrecision(18,4);
            Property(x => x.POldAgePensionB).HasColumnName("POLDAGEPENSIONB").IsOptional().HasPrecision(18,4);
            Property(x => x.PUnemploymentB).HasColumnName("PUNEMPLOYMENTB").IsOptional().HasPrecision(18,4);
            Property(x => x.PMedicalTreatmentB).HasColumnName("PMEDICALTREATMENTB").IsOptional().HasPrecision(18,4);
            Property(x => x.PSeriousIllnessB).HasColumnName("PSERIOUSILLNESSB").IsOptional().HasPrecision(18,4);
            Property(x => x.POccupationalInjuryB).HasColumnName("POCCUPATIONALINJURYB").IsOptional().HasPrecision(18,4);
            Property(x => x.PBearB).HasColumnName("PBEARB").IsOptional().HasPrecision(18,4);
            Property(x => x.PSocialSecurityB).HasColumnName("PSOCIALSECURITYB").IsOptional().HasPrecision(18,4);
            Property(x => x.PAccumulationFundB).HasColumnName("PACCUMULATIONFUNDB").IsOptional().HasPrecision(18,4);
            Property(x => x.PSubtotalB).HasColumnName("PSUBTOTALB").IsOptional().HasPrecision(18,4);
            Property(x => x.COldAgePensionB).HasColumnName("COLDAGEPENSIONB").IsOptional().HasPrecision(18,4);
            Property(x => x.CUnemploymentB).HasColumnName("CUNEMPLOYMENTB").IsOptional().HasPrecision(18,4);
            Property(x => x.CMedicalTreatmentB).HasColumnName("CMEDICALTREATMENTB").IsOptional().HasPrecision(18,4);
            Property(x => x.CSeriousIllnessB).HasColumnName("CSERIOUSILLNESSB").IsOptional().HasPrecision(18,4);
            Property(x => x.COccupationalInjuryB).HasColumnName("COCCUPATIONALINJURYB").IsOptional().HasPrecision(18,4);
            Property(x => x.CBearB).HasColumnName("CBEARB").IsOptional().HasPrecision(18,4);
            Property(x => x.CSocialSecurityB).HasColumnName("CSOCIALSECURITYB").IsOptional().HasPrecision(18,4);
            Property(x => x.CAccumulationFundB).HasColumnName("CACCUMULATIONFUNDB").IsOptional().HasPrecision(18,4);
            Property(x => x.CSubtotalB).HasColumnName("CSUBTOTALB").IsOptional().HasPrecision(18,4);
            Property(x => x.OldAgePension).HasColumnName("OLDAGEPENSION").IsOptional().HasPrecision(18,4);
            Property(x => x.Unemployment).HasColumnName("UNEMPLOYMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.MedicalTreatment).HasColumnName("MEDICALTREATMENT").IsOptional().HasPrecision(18,4);
            Property(x => x.SeriousIllness).HasColumnName("SERIOUSILLNESS").IsOptional().HasPrecision(18,4);
            Property(x => x.OccupationalInjury).HasColumnName("OCCUPATIONALINJURY").IsOptional().HasPrecision(18,4);
            Property(x => x.Bear).HasColumnName("BEAR").IsOptional().HasPrecision(18,4);
            Property(x => x.SocialSecurity).HasColumnName("SOCIALSECURITY").IsOptional().HasPrecision(18,4);
            Property(x => x.AccumulationFund).HasColumnName("ACCUMULATIONFUND").IsOptional().HasPrecision(18,4);
            Property(x => x.Subtotal).HasColumnName("SUBTOTAL").IsOptional().HasPrecision(18,4);
            Property(x => x.YFGZ).HasColumnName("YFGZ").IsOptional().HasPrecision(18,4);
            Property(x => x.GRSDS).HasColumnName("GRSDS").IsOptional().HasPrecision(18,4);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.PracticalSalary).HasColumnName("PRACTICALSALARY").IsOptional().HasPrecision(18,4);
            Property(x => x.HumanCost).HasColumnName("HUMANCOST").IsOptional().HasMaxLength(200);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Month).HasColumnName("MONTH").IsOptional();
        }
    }

    // T_HR_SocialRelation
    internal partial class T_HR_SocialRelationConfiguration : EntityTypeConfiguration<T_HR_SocialRelation>
    {
        public T_HR_SocialRelationConfiguration()
        {
			ToTable("T_HR_SOCIALRELATION");
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
            Property(x => x.Employee).HasColumnName("EMPLOYEE").IsOptional().HasMaxLength(200);
            Property(x => x.Relation).HasColumnName("RELATION").IsOptional().HasMaxLength(200);
            Property(x => x.Birthday).HasColumnName("BIRTHDAY").IsOptional();
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(200);
            Property(x => x.WorkUnit).HasColumnName("WORKUNIT").IsOptional().HasMaxLength(200);
            Property(x => x.Duty).HasColumnName("DUTY").IsOptional().HasMaxLength(200);
            Property(x => x.Tel).HasColumnName("TEL").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.EmployeeID).HasColumnName("EMPLOYEEID").IsOptional().HasMaxLength(200);
        }
    }

    // T_I_InstrumentBorrow
    internal partial class T_I_InstrumentBorrowConfiguration : EntityTypeConfiguration<T_I_InstrumentBorrow>
    {
        public T_I_InstrumentBorrowConfiguration()
        {
			ToTable("T_I_INSTRUMENTBORROW");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowDate).HasColumnName("BORROWDATE").IsOptional();
            Property(x => x.Instrument).HasColumnName("INSTRUMENT").IsOptional().HasMaxLength(200);
            Property(x => x.InstrumentName).HasColumnName("INSTRUMENTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.BorrowUserSign).HasColumnName("BORROWUSERSIGN").IsOptional();
            Property(x => x.OfficeSign).HasColumnName("OFFICESIGN").IsOptional();
        }
    }

    // T_I_InstrumentDiscard
    internal partial class T_I_InstrumentDiscardConfiguration : EntityTypeConfiguration<T_I_InstrumentDiscard>
    {
        public T_I_InstrumentDiscardConfiguration()
        {
			ToTable("T_I_INSTRUMENTDISCARD");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowDate).HasColumnName("BORROWDATE").IsOptional();
            Property(x => x.Instrument).HasColumnName("INSTRUMENT").IsOptional().HasMaxLength(200);
            Property(x => x.InstrumentName).HasColumnName("INSTRUMENTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.ApplyUserSign).HasColumnName("APPLYUSERSIGN").IsOptional();
            Property(x => x.OfficeSign).HasColumnName("OFFICESIGN").IsOptional();
        }
    }

    // T_I_InstrumentReturn
    internal partial class T_I_InstrumentReturnConfiguration : EntityTypeConfiguration<T_I_InstrumentReturn>
    {
        public T_I_InstrumentReturnConfiguration()
        {
			ToTable("T_I_INSTRUMENTRETURN");
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
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ReturnDate).HasColumnName("RETURNDATE").IsOptional();
            Property(x => x.Instrument).HasColumnName("INSTRUMENT").IsOptional().HasMaxLength(200);
            Property(x => x.InstrumentName).HasColumnName("INSTRUMENTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.ReturnUserSign).HasColumnName("RETURNUSERSIGN").IsOptional();
            Property(x => x.OfficeSign).HasColumnName("OFFICESIGN").IsOptional();
            Property(x => x.BorrowID).HasColumnName("BORROWID").IsOptional().HasMaxLength(200);
        }
    }

    // T_LeaveManage_LeaveApply
    internal partial class T_LeaveManage_LeaveApplyConfiguration : EntityTypeConfiguration<T_LeaveManage_LeaveApply>
    {
        public T_LeaveManage_LeaveApplyConfiguration()
        {
			ToTable("T_LEAVEMANAGE_LEAVEAPPLY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsOptional().HasMaxLength(50);
            Property(x => x.CompanyID).HasColumnName("COMPANYID").IsOptional().HasMaxLength(50);
            Property(x => x.FlowPhase).HasColumnName("FLOWPHASE").IsOptional().HasMaxLength(50);
            Property(x => x.FlowInfo).HasColumnName("FLOWINFO").IsOptional().HasMaxLength(1073741823);
            Property(x => x.StepName).HasColumnName("STEPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.AnnualLeaveResidueDay).HasColumnName("ANNUALLEAVERESIDUEDAY").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDept).HasColumnName("APPLYDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDeptName).HasColumnName("APPLYDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.LeaveCategory).HasColumnName("LEAVECATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.Describe).HasColumnName("DESCRIBE").IsOptional().HasMaxLength(500);
            Property(x => x.LeaveReason).HasColumnName("LEAVEREASON").IsOptional().HasMaxLength(500);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional().HasMaxLength(200);
            Property(x => x.LeaveDays).HasColumnName("LEAVEDAYS").IsOptional();
            Property(x => x.DeptSign).HasColumnName("DEPTSIGN").IsOptional();
            Property(x => x.PMSign).HasColumnName("PMSIGN").IsOptional();
            Property(x => x.ChairmanSign).HasColumnName("CHAIRMANSIGN").IsOptional();
            Property(x => x.StartTimeSlot).HasColumnName("STARTTIMESLOT").IsOptional().HasMaxLength(200);
            Property(x => x.EndTimeSlot).HasColumnName("ENDTIMESLOT").IsOptional().HasMaxLength(200);
            Property(x => x.AgentUser).HasColumnName("AGENTUSER").IsOptional().HasMaxLength(200);
            Property(x => x.AgentUserName).HasColumnName("AGENTUSERNAME").IsOptional().HasMaxLength(200);
        }
    }

    // T_M_ConferenceApply
    internal partial class T_M_ConferenceApplyConfiguration : EntityTypeConfiguration<T_M_ConferenceApply>
    {
        public T_M_ConferenceApplyConfiguration()
        {
			ToTable("T_M_CONFERENCEAPPLY");
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
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.HostDept).HasColumnName("HOSTDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.HostDeptName).HasColumnName("HOSTDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.MeetingStart).HasColumnName("MEETINGSTART").IsOptional();
            Property(x => x.MeetingStartHour).HasColumnName("MEETINGSTARTHOUR").IsOptional();
            Property(x => x.MeetingStartMin).HasColumnName("MEETINGSTARTMIN").IsOptional();
            Property(x => x.MeetingEnd).HasColumnName("MEETINGEND").IsOptional();
            Property(x => x.MeetingEndHour).HasColumnName("MEETINGENDHOUR").IsOptional();
            Property(x => x.MeetingEndMin).HasColumnName("MEETINGENDMIN").IsOptional();
            Property(x => x.MeetingRoom).HasColumnName("MEETINGROOM").IsOptional().HasMaxLength(200);
            Property(x => x.MeetingRoomName).HasColumnName("MEETINGROOMNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RoomAddress).HasColumnName("ROOMADDRESS").IsOptional().HasMaxLength(200);
            Property(x => x.Host).HasColumnName("HOST").IsOptional().HasMaxLength(200);
            Property(x => x.HostName).HasColumnName("HOSTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MainContent).HasColumnName("MAINCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.JoinUser).HasColumnName("JOINUSER").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUserName).HasColumnName("JOINUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PartyBUsers).HasColumnName("PARTYBUSERS").IsOptional().HasMaxLength(200);
            Property(x => x.SelfTotal).HasColumnName("SELFTOTAL").IsOptional();
            Property(x => x.GuestTotal).HasColumnName("GUESTTOTAL").IsOptional();
            Property(x => x.SelfLeader).HasColumnName("SELFLEADER").IsOptional();
            Property(x => x.GuestLeader).HasColumnName("GUESTLEADER").IsOptional();
            Property(x => x.Others).HasColumnName("OTHERS").IsOptional().HasMaxLength(500);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.IsNeedBanner).HasColumnName("ISNEEDBANNER").IsOptional().HasMaxLength(200);
            Property(x => x.Sum).HasColumnName("SUM").IsOptional().HasPrecision(18,2);
            Property(x => x.GeneralSign).HasColumnName("GENERALSIGN").IsOptional();
            Property(x => x.ConferenceAdminSign).HasColumnName("CONFERENCEADMINSIGN").IsOptional();
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(200);
        }
    }

    // T_M_ConferenceApply_Budget
    internal partial class T_M_ConferenceApply_BudgetConfiguration : EntityTypeConfiguration<T_M_ConferenceApply_Budget>
    {
        public T_M_ConferenceApply_BudgetConfiguration()
        {
			ToTable("T_M_CONFERENCEAPPLY_BUDGET");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_M_ConferenceApplyID).HasColumnName("T_M_CONFERENCEAPPLYID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Thing).HasColumnName("THING").IsOptional().HasMaxLength(200);
            Property(x => x.Price).HasColumnName("PRICE").IsOptional().HasPrecision(18,2);
            Property(x => x.Num).HasColumnName("NUM").IsOptional().HasPrecision(18,2);
            Property(x => x.Sum).HasColumnName("SUM").IsOptional().HasPrecision(18,2);

            // Foreign keys
            HasOptional(a => a.T_M_ConferenceApply).WithMany(b => b.T_M_ConferenceApply_Budget).HasForeignKey(c => c.T_M_ConferenceApplyID); // FK_T_M_ConferenceApply_Budget_T_M_ConferenceApply
        }
    }

    // T_M_ConferenceRegist
    internal partial class T_M_ConferenceRegistConfiguration : EntityTypeConfiguration<T_M_ConferenceRegist>
    {
        public T_M_ConferenceRegistConfiguration()
        {
			ToTable("T_M_CONFERENCEREGIST");
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
            Property(x => x.MeetApply).HasColumnName("MEETAPPLY").IsOptional().HasMaxLength(200);
            Property(x => x.MeetApplyName).HasColumnName("MEETAPPLYNAME").IsOptional().HasMaxLength(200);
            Property(x => x.HostDept).HasColumnName("HOSTDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.HostDeptName).HasColumnName("HOSTDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PlanMeetingDate).HasColumnName("PLANMEETINGDATE").IsOptional();
            Property(x => x.PlanMeetingPlace).HasColumnName("PLANMEETINGPLACE").IsOptional().HasMaxLength(200);
            Property(x => x.RegistUser).HasColumnName("REGISTUSER").IsOptional().HasMaxLength(200);
            Property(x => x.RegistUserName).HasColumnName("REGISTUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RegistDate).HasColumnName("REGISTDATE").IsOptional();
            Property(x => x.PlanJoinUserID).HasColumnName("PLANJOINUSERID").IsOptional().HasMaxLength(200);
            Property(x => x.PlanJoinUserIDName).HasColumnName("PLANJOINUSERIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MeetingStartDate).HasColumnName("MEETINGSTARTDATE").IsOptional();
            Property(x => x.MeetingEndDate).HasColumnName("MEETINGENDDATE").IsOptional();
            Property(x => x.MettingPlace).HasColumnName("METTINGPLACE").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUser).HasColumnName("JOINUSER").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUserName).HasColumnName("JOINUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Sum).HasColumnName("SUM").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(500);
        }
    }

    // T_M_ConferenceRegist_Settlement
    internal partial class T_M_ConferenceRegist_SettlementConfiguration : EntityTypeConfiguration<T_M_ConferenceRegist_Settlement>
    {
        public T_M_ConferenceRegist_SettlementConfiguration()
        {
			ToTable("T_M_CONFERENCEREGIST_SETTLEMENT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_M_ConferenceRegistID).HasColumnName("T_M_CONFERENCEREGISTID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Thing).HasColumnName("THING").IsOptional().HasMaxLength(200);
            Property(x => x.Price).HasColumnName("PRICE").IsOptional().HasPrecision(18,2);
            Property(x => x.Num).HasColumnName("NUM").IsOptional().HasPrecision(18,2);
            Property(x => x.Sum).HasColumnName("SUM").IsOptional().HasPrecision(18,2);

            // Foreign keys
            HasOptional(a => a.T_M_ConferenceRegist).WithMany(b => b.T_M_ConferenceRegist_Settlement).HasForeignKey(c => c.T_M_ConferenceRegistID); // FK_T_M_ConferenceRegist_Settlement_T_M_ConferenceRegist
        }
    }

    // T_M_ConferenceSummary
    internal partial class T_M_ConferenceSummaryConfiguration : EntityTypeConfiguration<T_M_ConferenceSummary>
    {
        public T_M_ConferenceSummaryConfiguration()
        {
			ToTable("T_M_CONFERENCESUMMARY");
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
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.TitleName).HasColumnName("TITLENAME").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUserID).HasColumnName("JOINUSERID").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUserIDName).HasColumnName("JOINUSERIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.HostDepID).HasColumnName("HOSTDEPID").IsOptional().HasMaxLength(200);
            Property(x => x.HostDepIDName).HasColumnName("HOSTDEPIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.HostUserID).HasColumnName("HOSTUSERID").IsOptional().HasMaxLength(200);
            Property(x => x.HostUserIDName).HasColumnName("HOSTUSERIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MeetingStart).HasColumnName("MEETINGSTART").IsOptional();
            Property(x => x.RecordUserID).HasColumnName("RECORDUSERID").IsOptional().HasMaxLength(200);
            Property(x => x.RecordUserIDName).HasColumnName("RECORDUSERIDNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MeetingSummary).HasColumnName("MEETINGSUMMARY").IsOptional().HasMaxLength(200);
            Property(x => x.AboutInfomation).HasColumnName("ABOUTINFOMATION").IsOptional().HasMaxLength(500);
            Property(x => x.IsNeedSigned).HasColumnName("ISNEEDSIGNED").IsOptional().HasMaxLength(200);
            Property(x => x.ApproveUserIDs).HasColumnName("APPROVEUSERIDS").IsOptional().HasMaxLength(200);
            Property(x => x.ApproveUserIDsName).HasColumnName("APPROVEUSERIDSNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RatifyUserIDs).HasColumnName("RATIFYUSERIDS").IsOptional().HasMaxLength(200);
            Property(x => x.RatifyUserIDsName).HasColumnName("RATIFYUSERIDSNAME").IsOptional().HasMaxLength(200);
            Property(x => x.MainContent).HasColumnName("MAINCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.DetailContent).HasColumnName("DETAILCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.CountersignederComment).HasColumnName("COUNTERSIGNEDERCOMMENT").IsOptional();
            Property(x => x.ApprovalerComment).HasColumnName("APPROVALERCOMMENT").IsOptional();
            Property(x => x.HostSign).HasColumnName("HOSTSIGN").IsOptional();
            Property(x => x.RecordSign).HasColumnName("RECORDSIGN").IsOptional();
            Property(x => x.SignUsersSign).HasColumnName("SIGNUSERSSIGN").IsOptional();
        }
    }

    // T_M_WeekConference
    internal partial class T_M_WeekConferenceConfiguration : EntityTypeConfiguration<T_M_WeekConference>
    {
        public T_M_WeekConferenceConfiguration()
        {
			ToTable("T_M_WEEKCONFERENCE");
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
            Property(x => x.ConferenceDate).HasColumnName("CONFERENCEDATE").IsOptional();
            Property(x => x.MeetingAddress).HasColumnName("MEETINGADDRESS").IsOptional().HasMaxLength(200);
            Property(x => x.ConferenceRecorder).HasColumnName("CONFERENCERECORDER").IsOptional().HasMaxLength(200);
            Property(x => x.ConferenceRecorderName).HasColumnName("CONFERENCERECORDERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.JoinUsers).HasColumnName("JOINUSERS").IsOptional();
            Property(x => x.JoinUsersName).HasColumnName("JOINUSERSNAME").IsOptional();
        }
    }

    // T_M_WeekConference_KeyItems
    internal partial class T_M_WeekConference_KeyItemsConfiguration : EntityTypeConfiguration<T_M_WeekConference_KeyItems>
    {
        public T_M_WeekConference_KeyItemsConfiguration()
        {
			ToTable("T_M_WEEKCONFERENCE_KEYITEMS");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.T_M_WeekConferenceID).HasColumnName("T_M_WEEKCONFERENCEID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsReleased).HasColumnName("ISRELEASED").IsOptional().HasMaxLength(1);
            Property(x => x.Person).HasColumnName("PERSON").IsOptional().HasMaxLength(200);
            Property(x => x.PersonName).HasColumnName("PERSONNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Project).HasColumnName("PROJECT").IsOptional().HasMaxLength(200);
            Property(x => x.ProjectName).HasColumnName("PROJECTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Item).HasColumnName("ITEM").IsOptional().HasMaxLength(200);
            Property(x => x.Progress).HasColumnName("PROGRESS").IsOptional().HasMaxLength(200);
            Property(x => x.EndDate).HasColumnName("ENDDATE").IsOptional();
            Property(x => x.ConferenceDate).HasColumnName("CONFERENCEDATE").IsOptional();

            // Foreign keys
            HasOptional(a => a.T_M_WeekConference).WithMany(b => b.T_M_WeekConference_KeyItems).HasForeignKey(c => c.T_M_WeekConferenceID); // FK_T_M_WeekConference_KeyItems_T_M_WeekConference
        }
    }

    // T_SealManage_SealAbolish
    internal partial class T_SealManage_SealAbolishConfiguration : EntityTypeConfiguration<T_SealManage_SealAbolish>
    {
        public T_SealManage_SealAbolishConfiguration()
        {
			ToTable("T_SEALMANAGE_SEALABOLISH");
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
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.AbolishReason).HasColumnName("ABOLISHREASON").IsOptional().HasMaxLength(500);
            Property(x => x.DeptSign).HasColumnName("DEPTSIGN").IsOptional();
            Property(x => x.BossSign).HasColumnName("BOSSSIGN").IsOptional();
            Property(x => x.PublicInformationDeptSign).HasColumnName("PUBLICINFORMATIONDEPTSIGN").IsOptional();
            Property(x => x.SealCategory).HasColumnName("SEALCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.SealPrimaryKey).HasColumnName("SEALPRIMARYKEY").IsOptional().HasMaxLength(200);
            Property(x => x.CurrentDept).HasColumnName("CURRENTDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.CurrentDeptName).HasColumnName("CURRENTDEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.AbolishSeal).HasColumnName("ABOLISHSEAL").IsOptional().HasMaxLength(200);
            Property(x => x.AbolishSealName).HasColumnName("ABOLISHSEALNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.AbolishDate).HasColumnName("ABOLISHDATE").IsOptional();
        }
    }

    // T_SealManage_SealBorrow
    internal partial class T_SealManage_SealBorrowConfiguration : EntityTypeConfiguration<T_SealManage_SealBorrow>
    {
        public T_SealManage_SealBorrowConfiguration()
        {
			ToTable("T_SEALMANAGE_SEALBORROW");
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
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.BorrowRange).HasColumnName("BORROWRANGE").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyReason).HasColumnName("APPLYREASON").IsOptional().HasMaxLength(500);
            Property(x => x.BorrowStartTime).HasColumnName("BORROWSTARTTIME").IsOptional();
            Property(x => x.BorrowEndTime).HasColumnName("BORROWENDTIME").IsOptional();
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(200);
            Property(x => x.SealName).HasColumnName("SEALNAME").IsOptional().HasMaxLength(200);
            Property(x => x.IsReback).HasColumnName("ISREBACK").IsOptional().HasMaxLength(200);
            Property(x => x.Seal).HasColumnName("SEAL").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.DeptDirectorSign).HasColumnName("DEPTDIRECTORSIGN").IsOptional();
            Property(x => x.ReturnTime).HasColumnName("RETURNTIME").IsOptional();
        }
    }

    // T_SealManage_SealEngraveAndChange
    internal partial class T_SealManage_SealEngraveAndChangeConfiguration : EntityTypeConfiguration<T_SealManage_SealEngraveAndChange>
    {
        public T_SealManage_SealEngraveAndChangeConfiguration()
        {
			ToTable("T_SEALMANAGE_SEALENGRAVEANDCHANGE");
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
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyReason).HasColumnName("APPLYREASON").IsOptional().HasMaxLength(500);
            Property(x => x.EngraveAmount).HasColumnName("ENGRAVEAMOUNT").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.DeptSign).HasColumnName("DEPTSIGN").IsOptional();
            Property(x => x.ApproveSign).HasColumnName("APPROVESIGN").IsOptional();
            Property(x => x.SealFull).HasColumnName("SEALFULL").IsOptional().HasMaxLength(200);
            Property(x => x.Shape).HasColumnName("SHAPE").IsOptional().HasMaxLength(500);
            Property(x => x.ApplyDate).HasColumnName("APPLYDATE").IsOptional();
            Property(x => x.SealFullName).HasColumnName("SEALFULLNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PublicInformationDeptSign).HasColumnName("PUBLICINFORMATIONDEPTSIGN").IsOptional();
        }
    }

    // T_SealManage_SealTurnOver
    internal partial class T_SealManage_SealTurnOverConfiguration : EntityTypeConfiguration<T_SealManage_SealTurnOver>
    {
        public T_SealManage_SealTurnOverConfiguration()
        {
			ToTable("T_SEALMANAGE_SEALTURNOVER");
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
            Property(x => x.SealName).HasColumnName("SEALNAME").IsOptional().HasMaxLength(200);
            Property(x => x.TurnOverReason).HasColumnName("TURNOVERREASON").IsOptional().HasMaxLength(500);
            Property(x => x.Accessory).HasColumnName("ACCESSORY").IsOptional().HasMaxLength(500);
            Property(x => x.Receiver).HasColumnName("RECEIVER").IsOptional().HasMaxLength(200);
            Property(x => x.ReceiverName).HasColumnName("RECEIVERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PersonTurnOver).HasColumnName("PERSONTURNOVER").IsOptional().HasMaxLength(200);
            Property(x => x.PersonTurnOverName).HasColumnName("PERSONTURNOVERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.PersonTurnOverSign).HasColumnName("PERSONTURNOVERSIGN").IsOptional();
            Property(x => x.SealPrimaryKey).HasColumnName("SEALPRIMARYKEY").IsOptional().HasMaxLength(200);
            Property(x => x.ReceiveDeptID).HasColumnName("RECEIVEDEPTID").IsOptional().HasMaxLength(200);
            Property(x => x.ReceiveDept).HasColumnName("RECEIVEDEPT").IsOptional().HasMaxLength(200);
            Property(x => x.Seal).HasColumnName("SEAL").IsOptional().HasMaxLength(200);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
        }
    }

    // T_SealManage_UseSealApply
    internal partial class T_SealManage_UseSealApplyConfiguration : EntityTypeConfiguration<T_SealManage_UseSealApply>
    {
        public T_SealManage_UseSealApplyConfiguration()
        {
			ToTable("T_SEALMANAGE_USESEALAPPLY");
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
            Property(x => x.Dept).HasColumnName("DEPT").IsOptional().HasMaxLength(200);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUser).HasColumnName("APPLYUSER").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserName).HasColumnName("APPLYUSERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyReason).HasColumnName("APPLYREASON").IsOptional().HasMaxLength(500);
            Property(x => x.PublicOrPrivate).HasColumnName("PUBLICORPRIVATE").IsOptional().HasMaxLength(200);
            Property(x => x.UseSealCategory).HasColumnName("USESEALCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.SealFullName).HasColumnName("SEALFULLNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ApplyUserPosition).HasColumnName("APPLYUSERPOSITION").IsOptional().HasMaxLength(200);
            Property(x => x.SealFull).HasColumnName("SEALFULL").IsOptional().HasMaxLength(200);
            Property(x => x.UseDate).HasColumnName("USEDATE").IsOptional();
        }
    }

    // T_TrainManage_TrainersManage
    internal partial class T_TrainManage_TrainersManageConfiguration : EntityTypeConfiguration<T_TrainManage_TrainersManage>
    {
        public T_TrainManage_TrainersManageConfiguration()
        {
			ToTable("T_TRAINMANAGE_TRAINERSMANAGE");
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
            Property(x => x.TrainName).HasColumnName("TRAINNAME").IsOptional().HasMaxLength(200);
            Property(x => x.TrainCategory).HasColumnName("TRAINCATEGORY").IsOptional().HasMaxLength(200);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.TrainAddress).HasColumnName("TRAINADDRESS").IsOptional().HasMaxLength(200);
            Property(x => x.TrainTeacher).HasColumnName("TRAINTEACHER").IsOptional().HasMaxLength(200);
            Property(x => x.TrainTeacherName).HasColumnName("TRAINTEACHERNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Trainers).HasColumnName("TRAINERS").IsOptional().HasMaxLength(200);
            Property(x => x.TrainersName).HasColumnName("TRAINERSNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.Accessory).HasColumnName("ACCESSORY").IsOptional().HasMaxLength(500);
            Property(x => x.PXFY).HasColumnName("PXFY").IsOptional().HasMaxLength(200);
            Property(x => x.MYD).HasColumnName("MYD").IsOptional().HasMaxLength(200);
        }
    }

}

