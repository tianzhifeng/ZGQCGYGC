using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using MvcAdapter;


namespace Project.Areas.ProjectGroup.Controllers
{
    public class ProjectListController : ProjectController
    {
        public ActionResult List()
        {
            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("Market.ManDept", "责任部门", "ChargerDept");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var phaseCategory = CategoryFactory.GetCategory("Project.Phase", "设计阶段", "Phase");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = false;
            tab.Categories.Add(phaseCategory);

            var projectClassCategory = CategoryFactory.GetCategory("Base.ProjectClass", "业务类型", "ProjectClass");
            projectClassCategory.SetDefaultItem();
            projectClassCategory.Multi = false;
            tab.Categories.Add(projectClassCategory);

            var stateCategory = CategoryFactory.GetCategory(typeof(Project.Logic.ProjectCommoneState), "项目状态", "State");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;
            tab.Categories.Add(stateCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var currentCity = System.Configuration.ConfigurationManager.AppSettings["City"];
            var currentProvince = System.Configuration.ConfigurationManager.AppSettings["Province"];
            string sql = @"select 
isnull(ContractValue,0) as ContractValue,
isnull(SummaryReceiptValue,0) as SummaryReceiptValue,
isnull(CanReceiptValue,0) as CanReceiptValue,
case when isnull(ContractValue,0)=0 then 0 else isnull(SummaryReceiptValue,0)/ContractValue*100
end as ReceiptRatio,
case when Province='{0}' and City='{1}' then 'LocalCity'
when Province='{0}' and City!='{1}' then 'LocalProvnice'
when Province!='{0}' then 'OtherProvince'
when Country!='中国' then 'Foreign' else '' end as Location,
S_I_Project.*
 from S_I_Project
 left join ( select Sum(ProjectValue) as ContractValue,
 ProjectID  from S_C_ManageContract_ProjectRelation
 group by ProjectID) ProjectContractInfo
 on S_I_Project.ID = ProjectContractInfo.ProjectID
 left join (
select Sum(FactReceiptValue) as SummaryReceiptValue, 
Sum( case when MileStoneState='True' then isnull((ReceiptValue-FactReceiptValue),0) else 0 end) as CanReceiptValue,
ProjectInfo from S_C_ManageContract_ReceiptObj 
group by ProjectInfo) ReceiptInfoAmout
on ReceiptInfoAmout.ProjectInfo=S_I_Project.ID
";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var data = db.ExecuteGridData(String.Format(sql, currentProvince, currentCity), qb);
            return Json(data);
        }

        public JsonResult GetProjectInfoID(string RelateID)
        {
            var groupInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == RelateID);
            if (groupInfo == null) { throw new Formula.Exceptions.BusinessException("没有找到指定的工程内容，请联系管理员"); }
            return Json(groupInfo);
        }

        public JsonResult Pause(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经完工，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Pause.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经暂停，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Terminate.ToString()) throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经终止，无法进行操作");
                projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Pause.ToString();
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Terminate(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经完工，无法进行操作");
                if (projectInfo.State == ProjectCommoneState.Terminate.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已经终止，无法进行操作");
                if (projectInfo.State != ProjectCommoneState.Pause.ToString())
                    projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Terminate.ToString();
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;

            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Restart(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作");
                if (projectInfo.State != ProjectCommoneState.Pause.ToString() &&
                 projectInfo.State != ProjectCommoneState.Terminate.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】不是暂停或终止状态，无法进行操作");
                projectInfo.State = projectInfo.Status;
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Finish(string ListData, string factFinishDate)
        {
            var list = JsonHelper.ToList(ListData);
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            foreach (var item in list)
            {
                var projectInfoID = item.GetValue("ID");
                var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目，无法进行操作！");
                if (projectInfo.State == ProjectCommoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("项目【" + projectInfo.Name + "】已完工，无法进行操作！");
                projectInfo.Status = projectInfo.State;
                projectInfo.State = ProjectCommoneState.Finish.ToString();
                projectInfo.FactFinishDate = DateTime.Parse(factFinishDate);
                projectInfo.ModifyDate = DateTime.Now;
                projectInfo.ModifyUser = this.CurrentUserInfo.UserName;
                projectInfo.ModifyUserID = this.CurrentUserInfo.UserID;

                //var marketProjectInfo = marketEntities.Set<Market.Logic.Domain.S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
                //if (marketProjectInfo != null)
                //    marketProjectInfo.State = projectInfo.State;
                projectInfo.GroupInfo.State = projectInfo.State;
            }
            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateTaskNoticeUpgrade(string ProjectInfoID)
        {
            var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息");

            if (this.entities.Set<T_CP_TaskNotice>().Count(d => d.ProjectInfoID == ProjectInfoID && d.FlowPhase != "End") > 0)
                throw new Formula.Exceptions.BusinessException("在审批中的任务单不能升版");

            var taskNotice = this.entities.Set<T_CP_TaskNotice>().Where(d => d.ProjectInfoID == projectInfo.ID
            && d.FlowPhase == "End").OrderByDescending(d => d.ID).FirstOrDefault();
            if (taskNotice == null) throw new Formula.Exceptions.BusinessException("没有找项目【" + projectInfo.Name + "】的任务单信息");

            string FlowVersionNumberStart = "1";
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string sql = string.Format("select ID,VersionNumber,FlowPhase from {0} where 1=1 and ProjectInfoID='{1}'", "T_CP_TaskNotice", taskNotice.ProjectInfoID);
            sql += " order by ID desc";
            var dt = sqlHelper.ExecuteDataTable(sql);
            var versions = string.Join(",", dt.AsEnumerable().Select(c => c["VersionNumber"].ToString()));
            return Json(new
            {
                ID = taskNotice.ID,
                ProjectInfoID = taskNotice.ProjectInfoID,
                VersionNumber = taskNotice.VersionNumber,
                Versions = versions
            });
        }

        public JsonResult ValidateProjectManagerChange(string ProjectInfoID)
        {
            var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.MarketProjectInfoID == ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息");

            if (this.entities.Set<T_CP_ProjectInfoChange>().Count(d => d.ProjectInfoID == projectInfo.ID && d.FlowPhase != "End") > 0)
                throw new Formula.Exceptions.BusinessException("已存在审批中的项目负责人变更，不能重复申请");
            return Json(new
            {
                ProjectInfoID = projectInfo.ID
            });
        }

        public JsonResult DeleteProject(string rowIDs)
        {
            //项目列表
            var projectInfoList = this.entities.Set<S_I_ProjectInfo>().Where(d => rowIDs.Contains(d.MarketProjectInfoID)).ToList();
            if (projectInfoList.Count == 0) return Json("");
            var projectInfoIDs = projectInfoList.Select(d => d.ID).ToList();
            var projectInfoGroupIDs = projectInfoList.Select(d => d.GroupID).ToList();

            //经营项目IDs
            var marketProjectInfoIDs = rowIDs.Split(',');
            var marketProjectIDsString = "'" + rowIDs.Replace(",", "','") + "'";

            //删除前校验
            var productCount = this.entities.Set<S_E_Product>().Count(d => projectInfoIDs.Contains(d.ProjectInfoID));
            if (productCount > 0)
                throw new Formula.Exceptions.BusinessException("有成果的项目不能被删除！");

//            var marketEntities = FormulaHelper.GetEntities<MarketEntities>();
//            var receiptInfo = marketEntities.Set<S_C_ManageContract_ReceiptObj>().Count(d => marketProjectInfoIDs.Contains(d.ProjectInfo) && d.FactReceiptValue.HasValue && d.FactReceiptValue != 0);
//            var costInfo = marketEntities.Set<S_FC_CostInfo>().Count(d => marketProjectInfoIDs.Contains(d.ProjectID) && d.CostValue != 0);
//            if (receiptInfo != 0 || costInfo != 0)
//                throw new Formula.Exceptions.BusinessException("发生过成本的项目不能被删除！");

//            //删除经营项目关联收款项数据
//            string sql = string.Format(@" update S_C_PlanReceipt set ProjectID='',ProjectCode='',ProjectName='' where ProjectID in ({0})
// update S_C_ManageContract_ReceiptObj set ProjectInfo='',ProjectInfoName='',MileStoneID='',MileStoneName='',MileStoneState='False' where ProjectInfo in ({0})", marketProjectIDsString);
//            var marketSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Market);
//            marketSqlHelper.ExecuteNonQuery(sql);

//            //删除经营项目关联合同关系
//            marketEntities.Set<S_C_ManageContract_ProjectRelation>().Delete(d => marketProjectInfoIDs.Contains(d.ProjectID));
//            //删除经营项目
//            marketEntities.Set<S_I_Project>().Delete(d => marketProjectInfoIDs.Contains(d.ID));

            var projectList = this.entities.Set<S_I_ProjectInfo>().Where(d => projectInfoIDs.Contains(d.ID)).ToList();
            foreach (var item in projectList)
            {
                item.Delete();
            }

            //删除立项单
            this.entities.Set<T_CP_TaskNotice>().Delete(d => projectInfoIDs.Contains(d.ProjectInfoID));
            //删除工程表项目节点
            this.entities.Set<S_I_ProjectGroup>().Delete(d => projectInfoGroupIDs.Contains(d.ID));
            ////删除项目
            //this.entities.Set<S_I_ProjectInfo>().Delete(d => projectInfoIDs.Contains(d.ID));
            //删除常用项目记录
            this.entities.Set<S_I_UserDefaultProjectInfo>().Delete(d => projectInfoIDs.Contains(d.ProjectInfoID));

            //marketEntities.SaveChanges();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteProjectRelateTBForever(string ListIDs)
        {
            //foreach (var Id in ListIDs.Split(','))
            //{
            //    Expenses.Logic.CBSInfoFO.ValidateDeleteRelateData(Id);
            //}
            entities.Set<S_AE_Mistake>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_C_CBS>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_C_CBS_Budget>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_C_CBS_Cost>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_D_DBS>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_D_DBSSecurity>().Delete(a => entities.Set<S_D_DBS>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.DBSID));
            entities.Set<S_D_DBSArchiveList>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_D_DBSArchiveList_ArchiveFiles>().Delete(a => entities.Set<S_D_DBSArchiveList>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.S_D_DBSArchiveListID));
            entities.Set<S_D_Document>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_D_DocumentVersion>().Delete(a => entities.Set<S_D_Document>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.DocumentID));
            entities.Set<S_D_Input>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_D_InputDocument>().Delete(a => entities.Set<S_D_Input>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.InputID));
            entities.Set<S_D_ShareInfo>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_ForceProject>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_Product>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_ProductDirectory>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_ProductVersion>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_PublishInfo>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_E_PublishInfoDetail>().Delete(a => entities.Set<S_E_PublishInfo>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.S_E_PublishInfoID));
            entities.Set<S_I_ProjectGroup>().Delete(a => entities.Set<S_I_ProjectInfo>().Where(b => ListIDs.Contains(b.ID)).Any(c => c.GroupID == a.ID));
            entities.Set<S_D_DataCollection>().Delete(a => entities.Set<S_I_ProjectInfo>().Where(b => ListIDs.Contains(b.ID)).Any(c => c.GroupID == a.GroupInfoID));
            entities.Set<S_I_ProjectRelation>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_I_UserDefaultProjectInfo>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_I_UserFocusProjectInfo>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_P_CooperationPlan>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_P_MileStone>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_P_MileStone_ProductDetail>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_P_MileStoneHistory>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_P_MileStonePlan>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_Q_QBS>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_R_Risk>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_Activity>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_CooperationExe>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_Monomer>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_OBSUser>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_RBS>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_StandardWorkTime>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_StandardWorkTimeDetail>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_TaskWork>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<S_W_TaskWork_RoleRate>().Delete(a => entities.Set<S_W_TaskWork>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.S_W_TaskWorkID));
            entities.Set<S_W_WBS>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            //entities.Set<S_W_WBS_Log>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            //entities.Set<S_W_WBSVersion>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_CP_ProjectInfoChange>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_CP_TaskNotice>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_Audit>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_Audit_AdviceDetail>().Delete(a => entities.Set<T_EXE_Audit>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_AuditID));
            entities.Set<T_EXE_AuditReview>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_ChangeAudit>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_ChangeAudit_AdviceDetail>().Delete(a => entities.Set<T_EXE_ChangeAudit>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_ChangeAuditID));
            entities.Set<T_EXE_DesignChangeApply>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_DesignChangeApply_TaskWork>().Delete(a => entities.Set<T_EXE_DesignChangeApply>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_DesignChangeApplyID));
            entities.Set<T_EXE_DesignChangeNotice>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_MajorPutInfo>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_MajorPutInfo_FetchDrawingInfo>().Delete(a => entities.Set<T_EXE_MajorPutInfo>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_MajorPutInfoID));
            entities.Set<T_EXE_ManageWorkloadSettlement>().Delete(a => ListIDs.Contains(a.ProjectInfo));
            entities.Set<T_EXE_ManageWorkloadSettlement_ManageWorkloadList>().Delete(a => entities.Set<T_EXE_ManageWorkloadSettlement>().Where(b => ListIDs.Contains(b.ProjectInfo)).Any(c => c.ID == a.T_EXE_ManageWorkloadSettlementID));
            entities.Set<T_EXE_MettingSign>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_MettingSign_ProjectGroupMembers>().Delete(a => entities.Set<T_EXE_MettingSign>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_MettingSignID));
            entities.Set<T_EXE_MettingSign_ResultList>().Delete(a => entities.Set<T_EXE_MettingSign>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_MettingSignID));
            entities.Set<T_EXE_ProductReview>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_ProductReview_ProjectGroupMembers>().Delete(a => entities.Set<T_EXE_ProductReview>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_ProductReviewID));
            entities.Set<T_EXE_ProductReview_ResultList>().Delete(a => entities.Set<T_EXE_ProductReview>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_ProductReviewID));
            entities.Set<T_EXE_ProjectExamination>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_ProjectExamination_Content>().Delete(a => entities.Set<T_EXE_ProjectExamination>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_ProjectExaminationID));
            entities.Set<T_EXE_PublishApply>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_PublishApply_Products>().Delete(a => entities.Set<T_EXE_PublishApply>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_PublishApplyID));
            entities.Set<T_EXE_TaskWorkChange>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_TaskWorkChange_TaskWork>().Delete(a => entities.Set<T_EXE_TaskWorkChange>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_TaskWorkChangeID));
            entities.Set<T_EXE_TaskWorkPublish>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_EXE_TaskWorkPublish_TaskWork>().Delete(a => entities.Set<T_EXE_TaskWorkPublish>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_EXE_TaskWorkPublishID));
            entities.Set<T_EXE_TaskWorkSettlement>().Delete(a => ListIDs.Contains(a.ProjectInfo));
            entities.Set<T_EXE_TaskWorkSettlement_TaskWorkList>().Delete(a => entities.Set<T_EXE_TaskWorkSettlement>().Where(b => ListIDs.Contains(b.ProjectInfo)).Any(c => c.ID == a.T_EXE_TaskWorkSettlementID));
            entities.Set<T_SC_DesignInput>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_DesignInput_DesignInputList>().Delete(a => entities.Set<T_SC_DesignInput>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_DesignInputID));
            entities.Set<T_SC_DesignPlan>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_DesignPlan_MilestoneList>().Delete(a => entities.Set<T_SC_DesignPlan>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_DesignPlanID));            
            entities.Set<T_SC_ElectricalPowerProjectScheme>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_ElectricalPowerProjectScheme_MajorList>().Delete(a => entities.Set<T_SC_ElectricalPowerProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_ElectricalPowerProjectSchemeID));
            entities.Set<T_SC_ElectricalPowerProjectScheme_ManageWorkloadList>().Delete(a => entities.Set<T_SC_ElectricalPowerProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_ElectricalPowerProjectSchemeID));
            entities.Set<T_SC_ElectricalPowerProjectScheme_MileStoneList>().Delete(a => entities.Set<T_SC_ElectricalPowerProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_ElectricalPowerProjectSchemeID));
            entities.Set<T_SC_ElectricalPowerProjectScheme_TaskWorkList>().Delete(a => entities.Set<T_SC_ElectricalPowerProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_ElectricalPowerProjectSchemeID));
            entities.Set<T_SC_MajorDesignInput>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_MajorDesignInput_DesignInputList>().Delete(a => entities.Set<T_SC_MajorDesignInput>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_MajorDesignInputID));
            entities.Set<T_SC_MixProjectScheme>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_MixProjectScheme_MajorList>().Delete(a => entities.Set<T_SC_MajorDesignInput>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_MixProjectSchemeID));
            entities.Set<T_SC_MixProjectScheme_MileStoneList>().Delete(a => entities.Set<T_SC_MajorDesignInput>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_MixProjectSchemeID));
            entities.Set<T_SC_MixProjectScheme_SubProjectList>().Delete(a => entities.Set<T_SC_MajorDesignInput>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_MixProjectSchemeID));            
            entities.Set<T_SC_SchemeForm>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_SchemeForm_MajorList>().Delete(a => entities.Set<T_SC_SchemeForm>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SchemeFormID));
            entities.Set<T_SC_SchemeForm_SubProjectList>().Delete(a => entities.Set<T_SC_SchemeForm>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SchemeFormID));
            entities.Set<T_SC_SchemeForm_OEM_Szsow>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_SchemeForm_OEM_Szsow_MajorList>().Delete(a => entities.Set<T_SC_SchemeForm_OEM_Szsow>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SchemeForm_OEM_SzsowID));
            entities.Set<T_SC_SchemeForm_OEM_Szsow_SubProjectList>().Delete(a => entities.Set<T_SC_SchemeForm_OEM_Szsow>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SchemeForm_OEM_SzsowID));
            entities.Set<T_SC_SimpleProjectSchmea>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_SimpleProjectSchmea_MileStone>().Delete(a => entities.Set<T_SC_SimpleProjectSchmea>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SimpleProjectSchmeaID));
            entities.Set<T_SC_SingleProjectScheme>().Delete(a => ListIDs.Contains(a.ProjectInfoID));
            entities.Set<T_SC_SingleProjectScheme_MajorList>().Delete(a => entities.Set<T_SC_SingleProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SingleProjectSchemeID));
            entities.Set<T_SC_SingleProjectScheme_MileStoneList>().Delete(a => entities.Set<T_SC_SingleProjectScheme>().Where(b => ListIDs.Contains(b.ProjectInfoID)).Any(c => c.ID == a.T_SC_SingleProjectSchemeID));
            entities.Set<S_I_FileConverts>().Delete(a => ListIDs.Contains(a.PrjID));
            entities.Set<S_N_Notice>().Delete(a => entities.Set<S_I_ProjectInfo>().Where(b => ListIDs.Contains(b.ID)).Any(c => c.GroupID == a.GroupInfoID));
            entities.Set<T_SC_DesignOutline>().Delete(a => ListIDs.Contains(a.ProjectName));
            entities.Set<S_I_ProjectInfo>().Delete(a => ListIDs.Contains(a.ID));

            //未找到与项目的对应关系
            //S_E_ForceProjectChargeUser
            //S_EP_PlotSealGroup
            //S_EP_PlotSealGroup_GroupInfo
            //S_EP_PlotSealInfo  

            var marketProjIDs = entities.Set<S_I_ProjectInfo>().Where(a => ListIDs.Contains(a.ID)).Select(b => b.MarketProjectInfoID).ToList();
            entities.SaveChanges();
            
            //经营项目
            //var marketEntities = FormulaHelper.GetEntities<MarketEntities>();
            //marketEntities.Set<S_I_Project>().Delete(a => marketProjIDs.Contains(a.ID));
            //marketEntities.Set<S_B_Bid>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<S_B_Bond>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<S_C_Deposit>().Delete(a => marketProjIDs.Contains(a.ProjectInfo));
            //marketEntities.Set<S_FC_CostInfo>().Delete(a => marketProjIDs.Contains(a.ProjectID));
            //marketEntities.Set<T_B_BidApply>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<T_B_BidProcessApply>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<T_B_BondApply>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<T_SP_DesignReview>().Delete(a => marketProjIDs.Contains(a.Project));
            //marketEntities.Set<S_C_ManageContract_ProjectRelation>().Delete(a => marketProjIDs.Contains(a.ProjectID));
            //marketEntities.Set<T_C_ContractChange_ProjectRelation>().Delete(a => marketProjIDs.Contains(a.ProjectID));
            //marketEntities.SaveChanges();
            return Json("");
        }
    }
}
