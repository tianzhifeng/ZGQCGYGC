using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using System.Data;
using Config.Logic;
using EPC.Logic.Domain;
using EPC.Logic;


namespace Project.Areas.AutoUI.Controllers
{
    public class TaskNoticeController : ProjectFormContorllor<T_CP_TaskNotice>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                if (!String.IsNullOrEmpty(upperVersionID))
                {
                    var lastVersion = this.GetEntityByID(upperVersionID);
                    if (string.IsNullOrEmpty(lastVersion.ProjectInfoID) || lastVersion.ProjectInfoID.Split(',').Length > 1)
                        throw new Formula.Exceptions.BusinessException("该任务单关联多个项目，无法升版");
                    if (dt.Columns.Contains("SerialNumber") && dt.Rows.Count > 0)
                    {
                        //项目编号不允许修改，故如果是升版每次都从系项目基本信息中获取项目编号
                        var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(lastVersion.ProjectInfoID);
                        if (projectInfo != null)
                        {
                            dt.Rows[0]["SerialNumber"] = projectInfo.Code;
                        }
                        else
                        {
                            dt.Rows[0]["SerialNumber"] = lastVersion.SerialNumber;
                        }
                    }
                }
                else
                {
                    //如果第一次下任务单给予一个初始的版本号
                    string FlowVersionNumberStart = "1";
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                        FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];
                    dt.Rows[0]["VersionNumber"] = FlowVersionNumberStart;
                }
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PhaseDetail")
            {
                var code = detail.GetValue("Code");
                var sameCode = this.BusinessEntities.Set<S_I_ProjectInfo>().FirstOrDefault(a => a.Code == code);
                if (sameCode != null)
                    throw new Formula.Exceptions.BusinessException("项目编号【" + sameCode + "】重复");
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_CP_TaskNotice();
            this.UpdateEntity(entity, dic);
            if (isNew)
            {
                //如果任务单一开始就有生产项目ID表示这里是任务单变更升版（因为任务单初次启动时，没有生产项目ID，立项完成后回写生产项目ID)
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
                if (projectInfo != null)
                {
                    //任务单升版时，项目编号不能进行变更
                    dic.SetValue("SerialNumber", projectInfo.Code);
                    var phaseNodes = projectInfo.S_W_WBS.ToList().Where(a => a.WBSType == WBSNodeType.Phase.ToString())
                        .Select(a => new { a.Name, a.WBSValue }).Distinct().ToList();
                    foreach (var item in phaseNodes)
                    {
                        if (!entity.Phase.Split(',').Contains(item.WBSValue))
                        {
                            //升版任务单时，不能删除已经存在wbs节点的阶段
                            throw new Formula.Exceptions.BusinessException("阶段【" + item.Name + "】已经存在策划信息，无法删除");
                        }
                    }
                }
                else
                {
                    if (this.BusinessEntities.Set<S_I_ProjectInfo>().Where(c => c.EngineeringInfoID == entity.EngineeringID
                         && c.PhaseValue.Contains(entity.Phase)).Count() > 0)
                    {
                        throw new Formula.Exceptions.BusinessException("阶段【" + entity.PhaseName + "】已经存在，无法重复下达任务单");
                    }
                }
            }
            if (string.IsNullOrEmpty(entity.ChargeDept))
            {
                dic.SetValue("ChargeDept", entity.DesignDept);
                dic.SetValue("ChargeDeptName", entity.DesignDeptName);
            }
            entity.Validate();
        }

        protected override void OnFlowEnd(T_CP_TaskNotice entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity == null) throw new Formula.Exceptions.BusinessException("没有找到指定的任务单，立项失败");
            var projectList = new List<S_I_ProjectInfo>();
            var dbContext = FormulaHelper.GetEntities<EPC.Logic.Domain.EPCEntities>();

            if (!String.IsNullOrEmpty(entity.ProjectInfoID) && this.BusinessEntities.Set<S_I_ProjectInfo>().Any(a => a.ID == entity.ProjectInfoID))
            {
                #region
                S_I_ProjectInfo projectInfo = null;
                projectInfo = entity.UpGrade();
                #endregion
            }
            else
            {
                #region 新下任务单
                var EngineeringID = entity.EngineeringID;
                if (string.IsNullOrWhiteSpace(EngineeringID))
                    EngineeringID = "aabe00fb-15ec-4eff-96ad-1e6ebb2c0bf7";
                      var engineeringInfo = dbContext.S_I_Engineering.Find(EngineeringID);
                if (engineeringInfo == null)
                    throw new Formula.Exceptions.BusinessValidationException("没有找对应的工程信息，无法下达任务单");
                if (engineeringInfo.S_I_WBS.Count == 0)
                {
                    if (String.IsNullOrEmpty(engineeringInfo.ModeCode))
                    {
                        engineeringInfo.initModeCode();
                    }
                    engineeringInfo.InitWBS();
                }
                var structNode = engineeringInfo.Mode.S_C_WBSStruct.FirstOrDefault(c => c.NodeType == "DesignProject");
                if (structNode == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到设计项目的WBS节点定义，无法下达任务单");
                }
                var parentNode = engineeringInfo.S_I_WBS.FirstOrDefault(c => c.StructInfoID == structNode.Parent.ID);
                if (parentNode == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("设计节点不能作为根节点定义，请联系管理员配置项目模式WBS模板");
                }
                var wbsNode = new S_I_WBS();
                wbsNode.Name = entity.ProjectInfo;
                wbsNode.Code = entity.SerialNumber;
                wbsNode.Value = entity.SerialNumber;
                wbsNode.PlanStartDate = entity.PlanStartDate;
                wbsNode.PlanEndDate = entity.PlanFinishDate;
                wbsNode.ChargerDept = entity.ChargeDept;
                wbsNode.ChargerDeptName = entity.ChargeDeptName;
                wbsNode.ChargerUser = entity.ChargeUser;
                wbsNode.ChargerUserName = entity.ChargeUserName;
                wbsNode.NodeType = structNode.NodeType;
                wbsNode.StructInfoID = structNode.ID;
                wbsNode.ID = FormulaHelper.CreateGuid();
                parentNode.AddChild(wbsNode);

                var managerIds = entity.ChargeUser.Split(',');
                var managerNames = entity.ChargeUserName.Split(',');

                for (int i = 0; i < managerIds.Length; i++)
                {
                    var managerID = managerIds[i];
                    var managerName = managerNames[i];
                    if (engineeringInfo.S_R_Resource.Count(c => c.ResourceID == managerID
                        && c.WBSID == wbsNode.ID && c.RoleCode == "DesignManager" && c.ResourceID == managerID) > 0)
                        continue;
                    var resource = new S_R_Resource();
                    resource.RoleName = "设计经理";
                    resource.RoleCode = "DesignManager";
                    resource.WBSID = wbsNode.ID;
                    resource.WBSFullID = wbsNode.FullID;
                    resource.TaskID = resource.WBSID;
                    resource.ResourceType = ResourceType.UserRole.ToString();
                    resource.ResourceID = managerID;
                    resource.ResourceName = managerName;
                    resource.ID = FormulaHelper.CreateGuid();
                    engineeringInfo.S_R_Resource.Add(resource);
                    engineeringInfo.SetOBSUser("DesignManager", managerID);
                }
                entity.ProjectInfoID = wbsNode.ID;
                var prj = AddProject(wbsNode, entity);
                engineeringInfo.SetWBSAuthWithUser();
                #endregion
            }

            dbContext.SaveChanges();

            this.BusinessEntities.SaveChanges();

            //#region 自动同步核算项目
            //Expenses.Logic.CBSInfoFO.SynCBSInfo(FormulaHelper.ModelToDic<T_CP_TaskNotice>(entity), Expenses.Logic.SetCBSOpportunity.TaskNoticeComplete);
            //#endregion
        }

        private S_I_ProjectInfo AddProject(S_I_WBS designNode, T_CP_TaskNotice entity, T_CP_TaskNotice_PhaseDetail singlePhase = null)
        {
            entity.ProjectInfoID = designNode.ID;
            S_I_ProjectInfo projectInfo = entity.Push();
            projectInfo.ModifyDate = projectInfo.CreateDate;
            projectInfo.ModifyUser = projectInfo.CreateUser;
            projectInfo.ModifyUserID = projectInfo.CreateUserID;

            //重新修改phaseValue、phaseName、Name、Code等信息
            if (singlePhase != null)
            {
                projectInfo.PhaseValue = singlePhase.Phase;
                projectInfo.WBSRoot.PhaseCode = singlePhase.Phase;
                var phaseList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Phase);
                var phaseItem = phaseList.FirstOrDefault(d => projectInfo.PhaseValue == d.Code);
                projectInfo.PhaseName = phaseItem.Name;
                projectInfo.Name = singlePhase.Name;
                projectInfo.Code = singlePhase.Code;
                projectInfo.ChargeDeptID = singlePhase.ChargeDept ?? entity.ChargeDept;
                projectInfo.ChargeDeptName = singlePhase.ChargeDeptName ?? entity.ChargeDeptName;
                projectInfo.ChargeUserID = singlePhase.ChargeUser ?? entity.ChargeUser;
                projectInfo.ChargeUserName = singlePhase.ChargeUserName ?? entity.ChargeUserName;
                projectInfo.OtherDeptID = singlePhase.OtherDept ?? entity.OtherDept;
                projectInfo.OtherDeptName = singlePhase.OtherDeptName ?? entity.OtherDeptName;
                projectInfo.PlanStartDate = singlePhase.PlanStartDate ?? entity.PlanStartDate;
                projectInfo.PlanFinishDate = singlePhase.PlanFinishDate ?? entity.PlanFinishDate;
            }
            projectInfo.ModifyDate = projectInfo.CreateDate;
            projectInfo.ModifyUser = projectInfo.CreateUser;
            projectInfo.ModifyUserID = projectInfo.CreateUserID;
            projectInfo.MarketProjectInfoID = projectInfo.ID;

            #region 默认创建EPS结构
            var group = this.BusinessEntities.Set<S_I_ProjectGroup>().FirstOrDefault(d => d.RelateID == entity.EngineeringID && d.Type == "Engineering");
            if (group == null)
            {
                group = new S_I_ProjectGroup();
                group.ID = Formula.FormulaHelper.CreateGuid();
                group.Name = designNode.S_I_Engineering.Name;
                group.Code = designNode.S_I_Engineering.SerialNumber;
                group.City = designNode.S_I_Engineering.City;
                group.Province = designNode.S_I_Engineering.Province;
                group.Area = designNode.S_I_Engineering.Area;
                group.ProjectClass = designNode.S_I_Engineering.ProjectClass;
                group.Investment = designNode.S_I_Engineering.Investment;
                group.PhaseContent = designNode.S_I_Engineering.PhaseValue;
                group.DeptID = designNode.S_I_Engineering.ChargerDept;
                group.DeptName = designNode.S_I_Engineering.ChargerDeptName;
                group.RelateID = designNode.S_I_Engineering.ID;
                group.EngineeringSpaceCode = ProjectMode.Standard.ToString();
                group.CreateDate = DateTime.Now;
                var fo = Formula.FormulaHelper.CreateFO<EPSFO>();
                fo.BuildEngineering(group);
            }
            group.BindingProject(projectInfo);
            entity.GroupID = group.ID;
            group.PhaseValue = designNode.S_I_Engineering.PhaseValue;
            #endregion

            projectInfo.InitDeisgnInputTemplate(true);

            //把设总放进RBS中
            if (projectInfo != null && !string.IsNullOrEmpty(entity.DesignManager))
            {
                projectInfo.WBSRoot.SetUsers(ProjectRole.DesignManager.ToString(), entity.DesignManager.Split(','), true, true, true, true);
            }
            return projectInfo;
        }

        public JsonResult ValidateUpVersion(string Data)
        {
            var data = JsonHelper.ToObject(Data);
            var taskNotice = this.GetEntityByID(data.GetValue("ID"));
            if (taskNotice == null) throw new Formula.Exceptions.BusinessException("没有找到指定对的任务单，不能进行升版操作");
            if (taskNotice.FlowPhase != "End" || String.IsNullOrEmpty(taskNotice.ProjectInfoID))
            {
                throw new Formula.Exceptions.BusinessException("不能对没有审批完成的任务单进行升版操作");
            }

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

        public JsonResult GetRestContractValue()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string sql = @"select case when  isnull(ContractRMBAmount,0)-isnull(ProjectValue,0)<0 then 0
else  isnull(ContractRMBAmount,0)-isnull(ProjectValue,0) end as RemainContractValue
 from S_C_ManageContract
left join (select Sum(ProjectValue) as ProjectValue,S_C_ManageContractID from S_C_ManageContract_ProjectRelation
group by S_C_ManageContractID) SplitInfo
on S_C_ManageContract.ID= SplitInfo.S_C_ManageContractID
where EngineeringInfo='{0}' 
and EngineeringInfo is not null and EngineeringInfo != ''";
            var marketSql = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var valObj = marketSql.ExecuteScalar(string.Format(sql, engineeringInfoID));
            if (valObj != null && valObj != DBNull.Value)
            {
                return Json(valObj);
            }

            return Json("");
        }
    }
}
