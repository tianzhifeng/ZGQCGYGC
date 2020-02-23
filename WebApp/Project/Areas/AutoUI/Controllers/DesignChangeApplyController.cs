using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config.Logic;
using Formula;
using Config;

namespace Project.Areas.AutoUI.Controllers
{
    public class DesignChangeApplyController : ProjectFormContorllor<T_EXE_DesignChangeApply>
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
            return base.PageView();
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var majorValue = dic.GetValue("MajorValue");
            var parent = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(
                a => a.ProjectInfoID == projectInfoID && a.WBSValue == majorValue);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前专业，保存失败。");
            var cbs = this.BusinessEntities.Set<S_C_CBS>().Where(a => a.Code == majorValue && a.ProjectInfoID == projectInfoID);
            var majorWorkload = Convert.ToDecimal(cbs.Sum(a => a.Quantity));
            var majorFinished = Convert.ToDecimal(cbs.Sum(a => a.SummaryCostQuantity));

            if (!string.IsNullOrEmpty(dic.GetValue("TaskWorkID")))
            {
                //专业工作区内发起设计变更申请（单卷册）
                var task = this.GetEntityByID<S_W_TaskWork>(dic.GetValue("TaskWorkID"));
                if (task == null)
                    throw new Formula.Exceptions.BusinessException("卷册【" + dic.GetValue("TaskWorkName") + "】不存在！");
                task.ChangeState = TaskWorkChangeState.ApplyStart.ToString();
            }
            if (!string.IsNullOrEmpty(dic.GetValue("TaskWork")))
            {
                var detail = JsonHelper.ToList(dic.GetValue("TaskWork"));
                if (detail.Count > 0)
                {
                    var taskWorkList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.MajorValue == majorValue && a.ProjectInfoID == projectInfoID).ToList();
                    //验证卷册工时 >=已结算，编号不能重复
                    List<string> existIds = new List<string>();
                    string taskWorkloadErrors = string.Empty, taskCodeErrors = string.Empty;
                    decimal sumWorkLoad = 0m;
                    foreach (var item in detail)
                    {
                        var code = item.GetValue("Code");
                        var taskWorkID = item.GetValue("TaskWorkID");
                        var itemWorkload = 0m;
                        if (!string.IsNullOrEmpty((item.GetValue("Workload"))))
                            itemWorkload = Convert.ToDecimal(item.GetValue("Workload"));
                        sumWorkLoad += itemWorkload;
                        var task = taskWorkList.FirstOrDefault(a => a.ID == taskWorkID);
                        if (task != null)
                        {
                            task.ChangeState = TaskWorkChangeState.ApplyStart.ToString();
                            existIds.Add(task.ID);
                            if (Convert.ToDecimal(task.WorkloadFinish) > itemWorkload)
                                taskWorkloadErrors += item.GetValue("Name") + ",";
                        }
                        var sameCode = taskWorkList.Any(a => a.Code == code && a.ID != taskWorkID);
                        if (sameCode || detail.Count(a => a.GetValue("Code") == code) > 1)
                            taskCodeErrors += item.GetValue("Name") + ",";
                    }

                    if (!isNew)
                    {
                        //移除的已存在的卷册，卷册状态需要变回变更申请前的状态
                        var mainId = dic.GetValue("ID");
                        var detailList = this.BusinessEntities.Set<T_EXE_DesignChangeApply_TaskWork>().Where(a => a.T_EXE_DesignChangeApplyID == mainId).ToList();
                        var detailIds = detail.Select(a => a.GetValue("ID")).ToList();
                        var deleteDetails = detailList.Where(a => !detailIds.Contains(a.ID)).ToList();
                        foreach (var deleteItem in deleteDetails)
                        {
                            var _task = taskWorkList.FirstOrDefault(a => a.ID == deleteItem.TaskWorkID);
                            if (this.BusinessEntities.Set<T_EXE_ChangeAudit>().Any(a => a.WBSID == _task.WBSID))
                                _task.ChangeState = TaskWorkChangeState.AuditFinish.ToString();
                            else
                                _task.ChangeState = string.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(taskWorkloadErrors))
                        throw new Formula.Exceptions.BusinessException("卷册【" + taskWorkloadErrors + "】的定额工时不能小于自身已结算工时！");
                    if (!string.IsNullOrEmpty(taskCodeErrors))
                        throw new Formula.Exceptions.BusinessException("卷册【" + taskCodeErrors + "】的编号重复！");
                    //验证专业工时 >=已结算  <=已策划
                    var taskWorkload = Convert.ToDecimal(taskWorkList.Where(a => !existIds.Contains(a.ID)).Sum(a => a.Workload));
                    //专业策划时，专业不校验已策划，所以此处也不进行校验
                    //if ((taskWorkload + sumWorkLoad) > majorWorkload)
                    //    throw new Formula.Exceptions.BusinessException("卷册的定额工时【" + (taskWorkload + sumWorkLoad).ToString() + "】已经超过专业定额工时【" + majorWorkload.ToString() + "】！");
                    if ((taskWorkload + sumWorkLoad) < majorFinished)
                        throw new Formula.Exceptions.BusinessException("卷册的定额工时【" + (taskWorkload + sumWorkLoad).ToString() + "】已经小于专业结算工时【" + majorFinished.ToString() + "】！");
                }
            }
        }

        protected override void OnFlowEnd(T_EXE_DesignChangeApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (!string.IsNullOrEmpty(entity.TaskWorkID))
            {
                //专业工作区内发起设计变更申请（单卷册）
                var task = this.GetEntityByID<S_W_TaskWork>(entity.TaskWorkID);
                if (task == null)
                    throw new Formula.Exceptions.BusinessException("卷册【" + task.Name + "】不存在！");
                if (task.Version == null)
                    task.Version = 1;
                else
                    task.Version++;
                task.ChangeState = TaskWorkChangeState.ApplyFinish.ToString();
                task.State = TaskWorkState.Plan.ToString();
                task.FactEndDate = null;
                task.FactYear = null;
                task.FactSeason = null;
                task.FactMonth = null;
            }
            else
            {
                //专业工作区外发起设计变更申请（多卷册）
                var majorWbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(
                    a => a.ProjectInfoID == entity.ProjectInfoID && a.WBSValue == entity.MajorValue);
                var taskWorkList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.MajorValue == entity.MajorValue && a.ProjectInfoID == entity.ProjectInfoID).ToList();
                foreach (var detail in entity.T_EXE_DesignChangeApply_TaskWork.ToList())
                {
                    var task = taskWorkList.FirstOrDefault(a => a.ID == detail.TaskWorkID);
                    if (task == null)
                    {
                        task = new S_W_TaskWork();
                    }
                    task.Name = detail.Name;
                    task.Code = detail.Code;
                    task.MajorValue = detail.Major;
                    task.PhaseValue = detail.Phase;
                    task.DossierCode = detail.DossierCode;
                    task.DossierName = detail.DossierName;
                    task.PlanEndDate = detail.PlanEndDate;
                    task.Workload = detail.Workload;
                    if (string.IsNullOrEmpty(task.ID))
                    {
                        task.ID = detail.TaskWorkID;
                        //根据专业同步人员
                        task.FillWBSUser(majorWbs);
                        majorWbs.AddTaskWork(task);
                        task.InitRoleRate();
                    }
                    else
                    {
                        task.Save();
                    }
                    task.ChangeState = TaskWorkChangeState.ApplyFinish.ToString();
                    task.S_W_WBS.PhaseCode = task.PhaseValue;
                    //修改成果的阶段、专业、卷号
                    foreach (var product in task.S_W_WBS.S_E_Product.ToList())
                    {
                        product.PhaseValue = task.PhaseValue;
                        product.MajorValue = task.MajorValue;
                        product.MonomerCode = task.DossierCode;
                        product.MonomerInfo = task.DossierName;
                        product.PackageCode = task.Code;
                        product.PackageName = task.Name;
                    }
                    if (task.Version == null)
                        task.Version = 1;
                    else
                        task.Version++;

                    task.State = TaskWorkState.Plan.ToString();
                    task.FactEndDate = null;
                    task.FactYear = null;
                    task.FactSeason = null;
                    task.FactMonth = null;
                }
            }
            this.BusinessEntities.SaveChanges();
        }

        public override JsonResult Delete()
        {
            List<T_EXE_DesignChangeApply> list = new List<T_EXE_DesignChangeApply>();
            if (!string.IsNullOrEmpty(Request["ListIDs"]))
            {
                var Ids = Request["ListIDs"].Split(',');
                list.AddRange(this.BusinessEntities.Set<T_EXE_DesignChangeApply>().Where(a => Ids.Contains(a.ID)).ToList());
            }
            if (!string.IsNullOrEmpty(Request["ID"]))
            {
                var id = Request["ID"];
                list.Add(this.BusinessEntities.Set<T_EXE_DesignChangeApply>().FirstOrDefault(a => a.ID == id));
            }
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.TaskWorkID))
                {
                    var task = this.GetEntityByID<S_W_TaskWork>(item.TaskWorkID);
                    if (task != null)
                    {
                        if (this.BusinessEntities.Set<T_EXE_ChangeAudit>().Any(a => a.WBSID == task.WBSID))
                            task.ChangeState = TaskWorkChangeState.AuditFinish.ToString();
                        else
                            task.ChangeState = string.Empty;
                    }
                }
                else
                {
                    foreach (var detail in item.T_EXE_DesignChangeApply_TaskWork.ToList())
                    {
                        var _task = this.GetEntityByID<S_W_TaskWork>(detail.TaskWorkID);
                        if (this.BusinessEntities.Set<T_EXE_ChangeAudit>().Any(a => a.WBSID == _task.WBSID))
                            _task.ChangeState = TaskWorkChangeState.AuditFinish.ToString();
                        else
                            _task.ChangeState = string.Empty;
                    }
                }
            }

            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);



            if (BusinessEntities != null)
                BusinessEntities.SaveChanges();

            return Json("");
        }

    }
}
