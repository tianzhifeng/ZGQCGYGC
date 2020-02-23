using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using System.Reflection;
using Formula.DynConditionObject;
using Formula.Helper;
using Formula;
using Project.Logic.Domain;
using Project.Logic;
using Config;
using Config.Logic;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using System.Web.Mvc;
using System.Transactions;

namespace Project
{
    public class ProjectFlowController<T> : MvcAdapter.BaseFlowController<T> where T : class, new()
    {
        private DbContext _flowEntities = null;
        protected virtual System.Data.Entity.DbContext flowEntities
        {
            get
            {
                if (_flowEntities == null)
                {
                    _flowEntities = FormulaHelper.GetEntities<Workflow.Logic.Domain.WorkflowEntities>();
                }
                return _flowEntities;
            }
        }


        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<ProjectEntities>();
                }
                return _entities;
            }
        }

        UserInfo _userInfo;
        protected UserInfo CurrentUserInfo
        {
            get
            {
                if (_userInfo == null)
                    _userInfo = FormulaHelper.GetUserInfo();
                return _userInfo;
            }
        }

        private SQLHelper _sqlHelper = null;
        protected virtual SQLHelper SqlHelper
        {
            get
            {
                if (_sqlHelper == null)
                {
                    _sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                }
                return _sqlHelper;
            }
        }

        protected virtual List<Dictionary<string, object>> GetRequestList(string paramName)
        {
            string data = this.Request[paramName];
            if (String.IsNullOrEmpty(data))
                return new List<Dictionary<string, object>>();
            return JsonHelper.ToList(data);
        }

        protected virtual Dictionary<string, object> GetRequestForm(string paramName)
        {
            string data = this.Request[paramName];
            if (String.IsNullOrEmpty(data))
                return new Dictionary<string, object>();
            return JsonHelper.ToObject(data);
        }

        protected virtual void BeforeDelete(List<T> entityList) { }

        protected virtual void AfterDelete(List<T> entityList) { }

        protected virtual void BeforeSave(T entity) { }

        protected virtual void AfterSave(T entity) { }

        protected virtual void BeforeGetMode(string id) { }

        protected virtual void AfterGetMode(T entity) { }

        public override System.Web.Mvc.JsonResult GetModel(string id)
        {
            BeforeGetMode(id);
            var entity = this.GetEntityByID<T>(id);
            AfterGetMode(entity);
            return Json(entity);
        }

        public override System.Web.Mvc.JsonResult Delete()
        {
            Func<JsonResult> deleteAction = () =>
            {
                string id = Request["ID"];
                string taskExecID = Request["TaskExecID"];
                var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
                if (!string.IsNullOrEmpty(id)) //流程启动时，从表单打开有地址栏参数ID
                {
                    flowEntities.S_WF_InsFlow.Delete(c => c.FormInstanceID == id);
                    flowEntities.SaveChanges();
                    return base.JsonDelete<T>(id);
                }
                else if (!string.IsNullOrEmpty(taskExecID)) //从任务列表打开删除
                {
                    S_WF_InsFlow flow = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault().S_WF_InsTask.S_WF_InsFlow;
                    id = flow.FormInstanceID;
                    flowEntities.S_WF_InsFlow.Remove(flow);
                    flowEntities.SaveChanges();

                    return base.JsonDelete<T>(id);
                }
                else //列表上的删除按钮
                {
                    var ids = Request["ListIDs"].Split(',');
                    flowEntities.S_WF_InsFlow.Delete(c => ids.Contains(c.FormInstanceID));
                    flowEntities.SaveChanges();
                    Specifications res = new Specifications();
                    res.AndAlso("ID", ids, QueryMethod.In);
                    var entitylist = this.entities.Set<T>().Where(res.GetExpression<T>()).ToList();
                    this.BeforeDelete(entitylist);
                    return base.JsonDelete<T>(Request["ListIDs"]);
                }
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var result = deleteAction();
                    ts.Complete();
                    return result;
                }
            }
            else
            {
                return deleteAction();
            }
        }

        public override System.Web.Mvc.JsonResult Save()
        {
            var entity = UpdateEntity<T>();
            BeforeSave(entity);
            entities.SaveChanges();
            AfterSave(entity);
            PropertyInfo pi = typeof(T).GetProperty("ID");
            if (pi != null)
                return Json(new { ID = pi.GetValue(entity, null) });
            return Json(new { ID = "" });
        }

        public virtual T GetEntityByID<T>(string ID) where T : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }

        public virtual T GetEntityByID(string ID)
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }

        public virtual T CreateEmptyEntity<T>(string ID = "") where T : class, new()
        {
            var result = new T();
            if (String.IsNullOrEmpty(ID))
                FormulaHelper.SetProperty(result, "ID", FormulaHelper.CreateGuid());
            else
                FormulaHelper.SetProperty(result, "ID", ID);
            return result;
        }

        public virtual T CreateEmptyEntity(string ID = "")
        {
            var result = new T();
            if (String.IsNullOrEmpty(ID))
                FormulaHelper.SetProperty(result, "ID", FormulaHelper.CreateGuid());
            else
                FormulaHelper.SetProperty(result, "ID", ID);
            return result;
        }
    }

    //public class AuditFlowController<T> : MvcAdapter.BaseController<T> where T : class, new()
    //{
    //    public virtual IAuditFlowService AuditFlowService
    //    {
    //        get
    //        {
    //            return AuditFlowServiceGenretor.CreateService();
    //        }
    //    }

    //    AuditStep _currentStep;
    //    /// <summary>
    //    /// 当前环节
    //    /// </summary>
    //    protected virtual AuditStep CurrentStep
    //    {
    //        get
    //        {
    //            if (_currentStep == null)
    //            {
    //                _currentStep = this.Steps.FirstOrDefault(d => d.StepKey.ToString() == this.CurrentActivity.ActivityKey);
    //            }
    //            return _currentStep;
    //        }
    //    }

    //    S_W_Activity _currentActivity;
    //    /// <summary>
    //    /// 当前活动（设计，校核，审核或审定）
    //    /// </summary>
    //    protected virtual S_W_Activity CurrentActivity
    //    {
    //        get
    //        {
    //            if (_currentActivity == null)
    //            {
    //                var activitiyID = this.Request["ActivityID"];
    //                _currentActivity = this.entities.Set<S_W_Activity>().SingleOrDefault(d => d.ID == activitiyID);
    //            }
    //            return _currentActivity;
    //        }
    //    }

    //    List<S_W_Activity> _activities;
    //    /// <summary>
    //    /// 当前流程的所有相关活动
    //    /// </summary>
    //    protected virtual List<S_W_Activity> Activities
    //    {
    //        get
    //        {
    //            if (_activities == null)
    //            {
    //                _activities = this.entities.Set<S_W_Activity>().Where(d => d.AuditPatchID == this.CurrentActivity.AuditPatchID).ToList();
    //            }
    //            return _activities;
    //        }
    //    }

    //    /// <summary>
    //    /// 流程环节执行前操作
    //    /// </summary>
    //    /// <param name="Entity"></param>
    //    /// <param name="Activity"></param>
    //    /// <param name="ExecuteOption"></param>
    //    protected virtual void OnExecute(T Entity, S_W_Activity Activity, string ExecuteOption, List<S_E_Product> products)
    //    { }
    //    /// <summary>
    //    /// 流程环节执行后操作
    //    /// </summary>
    //    /// <param name="Entity"></param>
    //    /// <param name="Activity"></param>
    //    /// <param name="ExecuteOption"></param>
    //    /// <param name="result"></param>
    //    protected virtual void OnExecuted(T Entity, S_W_Activity Activity, string ExecuteOption, ExeResult result, List<S_E_Product> products)
    //    { }
    //    /// <summary>
    //    /// 流程结束后操作
    //    /// </summary>
    //    /// <param name="Entity"></param>
    //    /// <param name="products"></param>
    //    /// <param name="ActivityID"></param>
    //    /// <param name="ExecuteOption"></param>
    //    protected virtual void OnSubmit(T Entity, List<S_E_Product> products, string ActivityID, string ExecuteOption)
    //    { }
    //    /// <summary>
    //    /// 设计人员撤销流程操作
    //    /// </summary>
    //    /// <param name="Entity"></param>
    //    /// <param name="products"></param>
    //    /// <param name="ActivityID"></param>
    //    /// <param name="ExecuteOption"></param>
    //    protected virtual void OnCancel(T Entity, List<S_E_Product> products, string ActivityID, string ExecuteOption)
    //    { }

    //    protected virtual void BeforeDelete(List<T> entityList) { }

    //    protected virtual void AfterDelete(List<T> entityList) { }

    //    protected virtual void BeforeSave(T entity, bool isNew) { }

    //    protected virtual void AfterSave(T entity, bool isNew) { }

    //    protected virtual void BeforeGetMode(string id) { }

    //    protected virtual void BeforeDeleteProduct(S_E_Product product) { }

    //    protected virtual void AfterGetMode(T entity, bool isNew) { }

    //    public virtual void ExecuteFlow(string DataForm, string ActivityID, string ExecuteOption, string SelectUserData)
    //    {
    //        var dic = JsonHelper.ToObject(DataForm);
    //        string id=dic.GetValue("ID");
    //        T entity = this.entities.Set<T>().Find(id);
    //        if (entity == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + id + "】的校审信息记录，无法执行校审");
    //        entity = this.UpdateEntity<T>(entity, dic);
    //        var activity = this.entities.Set<S_W_Activity>().SingleOrDefault(d => d.ID == ActivityID);

    //        //获得本次校审单中的所有成果
    //        var products = this.entities.Set<S_E_Product>().Where(d => d.AuditID == activity.AuditPatchID).ToList();
    //        var coSign = entities.Set<T_D_DesignCoSign>().FirstOrDefault(c => c.AuditFormID ==id );
    //        if (coSign != null)
    //        {
    //            if (coSign.CoSignState == CoSignState.Sign.ToString())
    //            {
    //                throw new Formula.Exceptions.BusinessException("有会签在进行中，不可执行此操作，请等待会签完成。");
    //            }
    //        }
    //        //执行会签
    //        if (ExecuteOption == "CoSign")
    //        {
    //            ExecuteCoSign(coSign, activity, products);
    //            this.entities.SaveChanges();
    //            return;
    //        }

    //        this.OnExecute(entity, activity, ExecuteOption, products);
    //        if (activity == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ActivityID + "】的活动对象，无法进行校审任务");
    //        var exeResult = this.AuditFlowService.Execute(activity, ExecuteOption, SelectUserData);
    //        this.OnExecuted(entity, activity, ExecuteOption, exeResult, products);

    //        var nextAct = exeResult.Activities.FirstOrDefault();
    //        if (nextAct != null)
    //        {
    //            foreach (var product in products)
    //                product.AuditState = Enum.Parse(typeof(AuditState), nextAct.ActivityKey).ToString();
    //        }
    //        if (exeResult.IsComplete)
    //        {
    //            if (exeResult.IsCancel)
    //            {
    //                this.OnCancel(entity, products, ActivityID, ExecuteOption);
    //            }
    //            else
    //            {
    //                foreach (var product in products)
    //                {
    //                    product.State = ProjectCommoneState.Finish.ToString();
    //                    product.AuditPassDate = DateTime.Now;
    //                    product.AuditState = AuditState.Pass.ToString();
    //                }
    //                this.OnSubmit(entity, products, ActivityID, ExecuteOption);
    //            }
    //        }
    //        this.entities.SaveChanges();
    //    }

    //    /// <summary>
    //    /// 启动会签
    //    /// </summary>
    //    /// <param name="coSign">会审单</param>
    //    /// <param name="act">当前校审环节</param>
    //    /// <param name="list">成果列表</param>
    //    public virtual void ExecuteCoSign(T_D_DesignCoSign coSign, S_W_Activity act, List<S_E_Product> list)
    //    {
    //        if (coSign == null)
    //            throw new Formula.Exceptions.BusinessException("未找到对应的会审信息。");
            
    //        coSign.CoSignState = CoSignState.Sign.ToString();
    //        string coSignMajor = "";
    //        foreach (var product in list)
    //        {
    //            if (product.IsCoSign == "True")
    //            {
    //                product.CoSignState = CoSignState.Sign.ToString();
    //                var majors = coSignMajor.Split(',');

    //                foreach (var newMajor in product.CoSignMajor.Split(','))
    //                {
    //                    if (!majors.Contains(newMajor))
    //                        coSignMajor = coSignMajor + newMajor + ",";
    //                }

    //            }
    //        }

    //        coSignMajor = coSignMajor.TrimEnd(',');
    //        if (coSignMajor != "")
    //        {
    //            var userList = getCoSignUser(coSignMajor, act);
    //            if (userList == null || userList.Count() <= 0)
    //                throw new Formula.Exceptions.BusinessException("未找到会审人员。");
    //            foreach (var dic in userList)
    //            {
    //                var signAct = new S_W_Activity();
    //                signAct.ID = GetGuid();
    //                signAct.ActivityKey = "CounterSign";
    //                signAct.ActvityName = "会签";
    //                signAct.DisplayName = act.DisplayName.Replace(act.DisplayName.Split('(').FirstOrDefault(),"会签");
    //                signAct.ProjectInfoID = act.ProjectInfoID;
    //                signAct.BusniessID = coSign.ID;
    //                signAct.WBSID = act.WBSID;
    //                signAct.AuditPatchID = coSign.AuditFormID;
    //                signAct.OwnerUserID = dic.GetValue("UserID");
    //                signAct.OwnerUserName = dic.GetValue("UserName");
    //                signAct.CreateDate = DateTime.Now;
    //                signAct.CreateUser = CurrentUserInfo.UserName;
    //                signAct.CreateUserID = CurrentUserInfo.UserID;
    //                signAct.State = ProjectCommoneState.Create.ToString();
    //                signAct.LinkUrl = "/Project/Form/CoSign/Edit";
    //                Dictionary<string, string> param = new Dictionary<string, string>();
    //                param["ID"] = coSign.ID;
    //                param["ProjectInfoID"] = act.ProjectInfoID;
    //                param["WBSID"] = act.WBSID;
    //                param["AuditFormID"] = coSign.AuditFormID;

    //                signAct.Params = JsonHelper.ToJson(param);

    //                entities.Set<S_W_Activity>().Add(signAct);
    //            }
    //        }


    //    }
    //    /// <summary>
    //    /// 取会签专业的会签人员
    //    /// </summary>
    //    /// <param name="majors">会签专业，逗号隔开</param>
    //    /// <param name="act">当前校审任务</param>
    //    /// <returns></returns>
    //    public virtual List<Dictionary<string,string>> getCoSignUser(string majors, S_W_Activity act)
    //    {
    //        List<Dictionary<string, string>> userList = new List<Dictionary<string, string>>();
    //        string projectInfoID = act.ProjectInfoID;
    //        AuditStep step = act.GetCurrentStep();
    //        string coSignRole = step.CoSignRole;
    //        string wbsType = WBSNodeType.Major.ToString();
    //        foreach (var major in majors.Split(','))
    //        {
    //            foreach (var role in coSignRole.Split(','))
    //            {
    //                var rbsUser = entities.Set<S_W_RBS>().Where(c => c.ProjectInfoID==act.ProjectInfoID&&c.WBSType == wbsType&&c.MajorValue==major&&c.RoleCode==role).Select(c => new { UserID=c.UserID,UserName=c.UserName}).Distinct();
    //                foreach (var userID in rbsUser)
    //                {
    //                    Dictionary<string, string> dic = new Dictionary<string, string>();
    //                    dic["UserID"] = userID.UserID;
    //                    dic["UserName"] = userID.UserName;

    //                    userList.Add(dic);
    //                }
    //            }
    //        }

    //        return userList;
    //    }
    //    public override System.Web.Mvc.JsonResult GetModel(string id)
    //    {
    //        BeforeGetMode(id);
    //        var entity = this.GetEntity<T>(id);
    //        bool isNew = false;
    //        if (entities.Entry<T>(entity).State == System.Data.EntityState.Added || entities.Entry<T>(entity).State == System.Data.EntityState.Detached)
    //            isNew = true;
    //        AfterGetMode(entity, isNew);
    //        return Json(entity);
    //    }

    //    public override System.Web.Mvc.JsonResult Delete()
    //    {
    //        string listIDs = Request["ListIDs"];
    //        Specifications res = new Specifications();
    //        res.AndAlso("ID", listIDs.Split(','), QueryMethod.In);
    //        var list = entities.Set<T>().Where(res.GetExpression<T>()).ToList();
    //        this.BeforeDelete(list);
    //        foreach (var item in list)
    //            entities.Set<T>().Remove(item);
    //        entities.SaveChanges();
    //        return Json("");
    //    }

    //    public override System.Web.Mvc.JsonResult Save()
    //    {
    //        bool isNew = false;
    //        var entity = UpdateEntity<T>();
    //        if (this.entities.Entry<T>(entity).State == System.Data.EntityState.Detached
    //            || this.entities.Entry<T>(entity).State == System.Data.EntityState.Added)
    //            isNew = true;
    //        BeforeSave(entity, isNew);
    //        entities.SaveChanges();
    //        AfterSave(entity, isNew);
    //        PropertyInfo pi = typeof(T).GetProperty("ID");
    //        if (pi != null)
    //            return Json(new { ID = pi.GetValue(entity, null) });
    //        return Json(new { ID = "" });
    //    }

    //    public System.Web.Mvc.JsonResult DeleteProduct(string Products)
    //    {
    //        var products = this.UpdateList<S_E_Product>(Products);
    //        foreach (var product in products)
    //        {
    //            BeforeDeleteProduct(product);
    //            product.Delete();
    //        }
    //        this.entities.SaveChanges();
    //        return Json("");
    //    }

    //    List<AuditStep> _steps;
    //    /// <summary>
    //    /// 当前流程的所有相关环节
    //    /// </summary>
    //    protected virtual List<AuditStep> Steps
    //    {
    //        get
    //        {
    //            if (_steps == null)
    //            {
    //                _steps = new List<AuditStep>();
    //                var stepLists = JsonHelper.ToList(this.CurrentActivity.DefSteps);
    //                foreach (var item in stepLists)
    //                    _steps.Add(new AuditStep(item));

    //            }
    //            return _steps;
    //        }
    //    }

    //    protected virtual void InitAuditFlowView()
    //    {
    //        ViewBag.CurrentStep = this.CurrentStep;
    //        ViewBag.Discrption = this.CurrentActivity.DisplayName;
    //        ViewBag.Activities = this.Activities;
    //        ViewBag.Steps = this.Steps;

    //        //2014-07-28 用于判断下一环节的人是否已经确认
    //        var currentStepIndex = this.Steps.IndexOf(this.CurrentStep) + 1;
    //        if (currentStepIndex < this.Steps.Count)
    //        {
    //            var nextStep = this.Steps[currentStepIndex];
    //            string nextUser = nextStep.UserID;
    //            if (nextStep != null)
    //                nextUser = nextStep.UserID;
    //            //如果没人，设置为单选人
    //            if (string.IsNullOrEmpty(nextUser))
    //                nextUser = "SelectOneUser";
    //            ViewBag.NextUser = nextUser;
    //        }
    //        //end

    //        //List<int> StepCount = new List<int>();
    //        //StepCount.Add(this.Activities.Count(d => d.ActivityKey == ActivityType.Design.ToString()));
    //        //StepCount.Add(this.Activities.Count(d => d.ActivityKey == ActivityType.Collact.ToString()));
    //        //StepCount.Add(this.Activities.Count(d => d.ActivityKey == ActivityType.Audit.ToString()));
    //        //StepCount.Add(this.Activities.Count(d => d.ActivityKey == ActivityType.Approve.ToString()));
    //        //ViewBag.StepsCount = StepCount;
    //        ViewBag.CurrentActivity = this.CurrentActivity;

    //        ViewBag.ShowSubmit = false;
    //        ViewBag.ShowPass = false;
    //        ViewBag.ShowRejectToCurrent = false;
    //        ViewBag.ShowReject = false;
    //        ViewBag.ShowCoSign = false;

    //        if (CurrentStep.Options.Exists(d => d == AuditOption.Pass))
    //            ViewBag.ShowSubmit = true;
    //        if (CurrentStep.Options.Exists(d => d == AuditOption.Over))
    //            ViewBag.ShowPass = true;
    //        if (CurrentStep.Options.Exists(d => d == AuditOption.Return))
    //            ViewBag.ShowRejectToCurrent = true;
    //        if (CurrentStep.Options.Exists(d => d == AuditOption.Back))
    //            ViewBag.ShowReject = true;

    //        if (CurrentStep.CoSign == "是")
    //        {
    //            string formID = this.CurrentActivity.BusniessID;
    //            var products = this.entities.Set<S_E_Product>().Where(d => d.AuditID == formID).ToList();
    //            foreach (var product in products)
    //            {
    //                if (product.IsCoSign == "True")
    //                {
    //                    ViewBag.ShowCoSign = true;
    //                    break;
    //                }
    //            }

    //        }
    //    }

    //    protected virtual string CollactRouteCode
    //    {
    //        get
    //        {
    //            return "Collact";
    //        }
    //    }

    //    protected virtual string AuditRouteCode
    //    {
    //        get
    //        {
    //            return "Audit";
    //        }
    //    }

    //    protected virtual string ApproveRouteCode
    //    {
    //        get
    //        {
    //            return "Approve";
    //        }
    //    }

    //    private DbContext _entities = null;
    //    protected override System.Data.Entity.DbContext entities
    //    {
    //        get
    //        {
    //            if (_entities == null)
    //            {
    //                _entities = FormulaHelper.GetEntities<ProjectEntities>();
    //            }
    //            return _entities;
    //        }
    //    }

    //    UserInfo _userInfo;
    //    protected UserInfo CurrentUserInfo
    //    {
    //        get
    //        {
    //            if (_userInfo == null)
    //                _userInfo = FormulaHelper.GetUserInfo();
    //            return _userInfo;
    //        }
    //    }


    //    public virtual T GetEntityByID(string ID)
    //    {
    //        var spec = new Specifications();
    //        spec.AndAlso("ID", ID, QueryMethod.Equal);
    //        var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
    //        return result;
    //    }
    //}

}