using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using System.Text;
using Workflow.Logic.Domain;

namespace Project.Areas.AutoUI.Controllers
{
    public class CooperationExecuteController : ProjectFormContorllor<T_EXE_MajorPutInfo>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }

            base.PageView();
            string ID = Request["ID"];
            string wbsValue = string.Empty;
            string projectInfoID = string.Empty;
            string wbsID = Request["WBSID"];
            string majorWBSID = string.Empty;

            var entity = this.GetEntityByID<T_EXE_MajorPutInfo>(ID);
            if (entity == null)
            {
                projectInfoID = this.GetQueryString("ProjectInfoID");
                wbsValue = Request["OutMajorValue"];
            }
            else
            {
                projectInfoID = entity.ProjectInfoID;
                wbsValue = entity.OutMajorValue;
            }
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未找到ID为【" + projectInfoID + "】的项目信息");
            if (string.IsNullOrEmpty(wbsID))
                wbsID = projectInfo.WBSRoot.ID;
            else
            {
                var wbsNode = this.GetEntityByID<S_W_WBS>(wbsID.Split(',').FirstOrDefault());
                var majorNode = wbsNode.Seniorities.FirstOrDefault(a => a.WBSType == WBSNodeType.Major.ToString());
                if (majorNode != null)
                    majorWBSID = majorNode.ID;
            }
            //根据项目下专业节点，将项目项目下专业节点的WBSVAlue和Name拼成一个枚举
            StringBuilder sb = new StringBuilder("\n");
            sb.AppendLine();
            sb.AppendFormat("\n var WBSID = '{0}'; \n var MajorWBSID = '{1}';", wbsID, majorWBSID);
            sb.AppendLine();
            ViewBag.Script += sb.ToString();
            return View();
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (String.IsNullOrEmpty(dic.GetValue("Receiver")))
            {
                var wbsNode = this.GetEntityByID<S_W_WBS>(dic.GetValue("WBSID"));
                if (wbsNode != null)
                {
                    var inMajor = dic.GetValue("InMajorValue");
                    var majorNodes = wbsNode.AllChildren.Where(d => d.WBSType == WBSNodeType.Major.ToString() && inMajor.Contains(d.WBSValue)).ToList();
                    var reciviers = ""; var recivierNames = "";
                    foreach (var major in majorNodes)
                    {
                        string majorPrinciple = "MajorPrinciple";
                        var roleList = major.S_W_RBS.Where(d => d.RoleCode == majorPrinciple).ToList();
                        foreach (var item in roleList)
                        {
                            reciviers += item.UserID + ",";
                            recivierNames += item.UserName + ",";
                        }
                    }
                    dic.SetValue("Receiver", reciviers.TrimEnd(','));
                    dic.SetValue("ReceiverName", recivierNames.TrimEnd(','));
                }
            }
            if (string.IsNullOrEmpty(dic.GetValue("RelateWBSID")))
            {
                var wbsid = dic.GetValue("WBSID").Split(',').FirstOrDefault();
                var coopWBS = this.GetEntityByID<S_W_WBS>(wbsid);
                if (coopWBS != null)
                {
                    dic.SetValue("RelateWBSID", coopWBS.ParentID);
                }
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (!string.IsNullOrEmpty(dic.GetValue("CoopPlanID")))
            {
                var wbsNode = this.GetEntityByID<S_W_WBS>(dic.GetValue("WBSID"));
                var coopPlan = this.GetEntityByID<S_P_CooperationPlan>(dic.GetValue("CoopPlanID"));
                if (wbsNode != null && string.IsNullOrEmpty(wbsNode.RelateMileStone) && coopPlan != null)
                {
                    var mileStone = this.GetEntityByID<S_P_MileStone>(coopPlan.MileStoneID);
                    if (mileStone != null)
                    {
                        var mileDic = new Dictionary<string, object>();
                        mileDic.Add("ID", mileStone.ID);
                        mileDic.Add("Name", mileStone.Name);
                        mileDic.Add("PlanStart", mileStone.PlanStartDate);
                        mileDic.Add("PlanFinish", mileStone.PlanFinishDate);
                        var list = new List<object>();
                        list.Add(mileDic);
                        wbsNode.RelateMileStone = JsonHelper.ToJson(list);
                        this.BusinessEntities.SaveChanges();
                    }
                }
            }
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            var entity = this.BusinessEntities.Set<T_EXE_MajorPutInfo>().SingleOrDefault(c => c.ID == taskExec.S_WF_InsFlow.FormInstanceID);
            if (entity != null)
            {
                //CAD消息随大事记发送 2019年4月4日

                //var workFlowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
                //var nextStep = workFlowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
                //var receiverIds = nextExecUserIDs;
                //var receiverNames = nextExecUserNames;
                //var title =/* currentStep.Name + routing.Name +*/ "互提通知";
                //var content = "【" + entity.ProjectName + "】";
                //if (!string.IsNullOrEmpty(entity.SubProjectName))
                //    content += "【" + entity.SubProjectName + "】";
                //if (!string.IsNullOrEmpty(entity.Phase))
                //    content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Phase", entity.Phase) + "】";
                //if (!string.IsNullOrEmpty(entity.OutMajorValue))
                //    content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Major", entity.OutMajorValue) + "】";
                //if (routing.Code == "Receive")
                //{
                //    title = "互提流程通过通知";
                //    content += "有成果通过提资流程";
                //    receiverIds = entity.CreateUserID;
                //    receiverNames = entity.CreateUser;
                //}
                //else
                //    content += "提出资料需要" + nextStep.Name;
                //FormulaHelper.CreateFO<ProjectInfoFO>().SendNotice(entity.ProjectInfoID, entity.WBSID, entity.OutMajorValue, "",
                //    receiverIds, receiverNames, title, content, "", "", entity.ID, "T_EXE_MajorPutInfo", ProjectNoticeType.Major.ToString(), "", "");
            }
            return base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
        }

        /// <summary>
        /// 流程结束反写的业务逻辑
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="taskExec"></param>
        /// <param name="routing"></param>
        protected override void OnFlowEnd(T_EXE_MajorPutInfo entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
                entity.Publish();
            this.BusinessEntities.SaveChanges();

            if (!string.IsNullOrEmpty(entity.CoopPlanID))
            {
                var coopPlan = this.BusinessEntities.Set<S_P_CooperationPlan>().FirstOrDefault(c => c.ID == entity.CoopPlanID);
                if (coopPlan != null && !string.IsNullOrEmpty(coopPlan.MileStoneID))
                {
                    var mileStone = this.BusinessEntities.Set<S_P_MileStone>().FirstOrDefault(c => c.ID == coopPlan.MileStoneID);
                    //同步关联的收款项的里程碑信息：时间、状态
                    //var fo = new Basic.Controllers.MileStoneExecuteController();
                    //fo.SyncReceiptObj(mileStone);
                }
            }
        }

        public JsonResult GetCoopPlanEnum(string wbsID, string projectInfoID, string MajorCode)
        {
            var result = new List<object>();

            var coopPlanDT = this.ProjectSQLDB.ExecuteDataTable("select * from S_P_CooperationPlan where ProjectInfoID = '" + projectInfoID + "'");
            var wbsDT = this.ProjectSQLDB.ExecuteDataTable("select * from S_W_WBS with(nolock) where ProjectInfoID = '" + projectInfoID + "'");
            foreach (var wID in wbsID.Split(','))
            {
                var wbs = wbsDT.AsEnumerable().FirstOrDefault(a => a["ID"].ToString() == wID);
                if (wbs != null)
                {
                    var relateMileStoneStr = wbs["RelateMileStone"] is DBNull ? "" : wbs["RelateMileStone"].ToString();
                    var relateMileStone = JsonHelper.ToList(relateMileStoneStr);
                    if (relateMileStone.Count > 0)
                    {
                        //石化院模式，在WBS策划页面，提资包关联提资计划里程碑
                        foreach (var miles in relateMileStone)
                        {
                            var coop = coopPlanDT.AsEnumerable().FirstOrDefault(a => a["MileStoneID"].ToString() == miles.GetValue("ID").ToString());
                            if (coop != null)
                                result.Add(new
                                {
                                    MileStoneID = coop["MileStoneID"].ToString(),
                                    value = coop["ID"].ToString(),
                                    text = coop["CooperationContent"].ToString(),
                                    InMajorValue = coop["InMajorValue"].ToString(),
                                    WBSID = wID
                                });
                        }
                    }
                    else
                    {
                        var cPlans = coopPlanDT.AsEnumerable().Where(a => a["WBSID"].ToString() == wID);
                        if (cPlans.Count() > 0)
                        {
                            //建筑院发起计划提资，提资包根据提资计划直接生成
                            foreach (var coop in cPlans)
                            {
                                result.Add(new
                                {
                                    MileStoneID = coop["MileStoneID"].ToString(),
                                    value = coop["ID"].ToString(),
                                    text = coop["CooperationContent"].ToString(),
                                    InMajorValue = coop["InMajorValue"].ToString(),
                                    WBSID = coop["WBSID"].ToString()
                                });
                            }
                        }
                        else
                        {
                            //石化院，提资包没有关联提资计划，把所有提出专业相符的提资计划列出
                            var coopPlans = coopPlanDT.AsEnumerable().Where(a => a["OutMajorValue"].ToString() == MajorCode);
                            foreach (var coop in coopPlans)
                            {
                                result.Add(new
                                {
                                    MileStoneID = coop["MileStoneID"].ToString(),
                                    value = coop["ID"].ToString(),
                                    text = coop["CooperationContent"].ToString(),
                                    InMajorValue = coop["InMajorValue"].ToString(),
                                    WBSID = wID
                                });
                            }
                        }
                    }
                }
            }
            return Json(result);
        }

        public JsonResult GetRoleEnum(string wbsID, string Role)
        {
            var sql = @"select UserIDs as value,UserNames as text from dbo.V_I_UserRBSInfo
where WBSID in ('{0}') and RoleCode='{1}'";
            var dt = this.ProjectSQLDB.ExecuteDataTable(string.Format(sql, wbsID.Replace(",", "','"), Role));
            if (dt.Rows.Count == 0)
            {
                var wbsNode = this.GetEntityByID<S_W_WBS>(wbsID.Split(',').FirstOrDefault());
                if (wbsNode != null)
                {
                    foreach (var parent in wbsNode.Seniorities.OrderByDescending(a => a.FullID))
                    {
                        dt = this.ProjectSQLDB.ExecuteDataTable(string.Format(sql, parent.ID, Role));
                        if (dt.Rows.Count > 0)
                            break;
                    }
                }
            }
            return Json(dt);
        }
    }
}
