using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Formula.Helper;
using Formula;
using EPC.Logic.Domain;
using Config;
using Workflow.Logic.Domain;
using Workflow.Logic;
using System.Web.Mvc;
using System.Data;
using Base.Logic.Domain;
using Config.Logic;
using System.Text.RegularExpressions;


namespace EPC
{
    public class EPCFormContorllor<T> : BaseAutoFormController where T : class, new()
    {
        SQLHelper sqlDB;
        public virtual SQLHelper EPCSQLDB
        {
            get
            {
                if (sqlDB == null)
                    sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                return sqlDB;
            }
        }

        public virtual DbContext EPCEntites
        {
            get
            {
                return FormulaHelper.GetEntities<EPCEntities>();
            }
        }

        public override DbContext BusinessEntities
        {
            get
            {
                return this.EPCEntites;
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

        #region 扩展虚方法

        protected virtual void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            
        }

        protected virtual void OnFlowEnd(T entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {

        }

        protected virtual void AfterWithDrawingTask(T entity, S_WF_InsTaskExec taskExec)
        { }

        public virtual void SendStartNotice(S_UI_Form formInfo, T entity)
        {
            var infraEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var dic = FormulaHelper.ModelToDic<T>(entity);
            var engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null)
                return;
            var isoDefine = infraEntities.S_T_ISODefine.FirstOrDefault(d => d.FormCode == formInfo.Code);
            if (isoDefine == null) return;
            SendNotice(isoDefine, dic, engineeringInfo);
        }

        public virtual void SendEndNotice(S_UI_Form formInfo, T entity)
        {
            var infraEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var dic = FormulaHelper.ModelToDic<T>(entity);
            var engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null)
                return;
            var isoDefine = infraEntities.S_T_ISODefine.FirstOrDefault(d => d.FormCode == formInfo.Code);
            if (isoDefine == null) return;
            SendNotice(isoDefine, dic, engineeringInfo, "End");
        }

        #endregion

        public override ActionResult PageView()
        {
            return base.PageView();
        }

        public override bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing,
            string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup,
            string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().Where(c => c.Code == tmplCode).OrderByDescending(c => c.ID).FirstOrDefault(); //获取最新一个版本即可
            var entity = this.GetEntityByID<T>(taskExec.S_WF_InsFlow.FormInstanceID);
            bool flag = false;
            Workflow.Logic.BusinessFacade.FlowFO flowFO = new Workflow.Logic.BusinessFacade.FlowFO();
            flag = flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);
            //流程表单定义的流程逻辑
            ExecFlowLogic(routing.Code, taskExec.S_WF_InsFlow.FormInstanceID, "");
            SetFormFlowInfo(taskExec.S_WF_InsFlow);
            if (flag)
            {
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                OnFlowEnd(entity, taskExec, routing);
            }
            this.BusinessEntities.SaveChanges();
            if (routing != null)
            {
                routing.ExeLogic();
                this.entities.SaveChanges();
            }
            return flag;
        }

        public virtual T GetEntityByID(string ID, bool fromDB = false)
        {
            return GetEntityByID<T>(ID, fromDB);
        }

        public override JsonResult Save()
        {
            return this.SaveBA(Request["TmplCode"]);
        }

        public override JsonResult SaveBA(string tmplCode)
        {
            return base.SaveBA(tmplCode);
        }

        public override JsonResult Delete()
        {
            return base.Delete();
        }

        public override JsonResult WithdrawAskTask()
        {
            flowService.WithdrawAskTask(Request["TaskExecID"]);
            var taskExec = this.entities.Set<S_WF_InsTaskExec>().Find(Request["TaskExecID"]);
            if (taskExec != null)
            {
                var entity = this.GetEntityByID<T>(taskExec.S_WF_InsFlow.FormInstanceID);
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                this.AfterWithDrawingTask(entity, taskExec);
            }
            return Json("");
        }

        void SendNotice(S_T_ISODefine isoDefine, Dictionary<string, object> dic, S_I_Engineering engineeringInfo, string sendType = "Start")
        {
            var regText = sendType == "Start" ? isoDefine.StartNoticeContent : isoDefine.EndNoticeContent;
            if (isoDefine.SendNotice != "True" || String.IsNullOrEmpty(regText))
            {
                return;
            }
            var formID = dic.GetValue("ID");
            var notice = this.EPCEntites.Set<S_I_Notice>().FirstOrDefault(d => d.RelateType == sendType && d.RelateID == formID);
            if (notice == null)
            {
                notice = this.EPCEntites.Set<S_I_Notice>().Create();
                notice.ID = FormulaHelper.CreateGuid();
                var enumDefine = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(isoDefine.EnumFieldInfo))
                    enumDefine = JsonHelper.ToList(isoDefine.EnumFieldInfo);
                var content = replaceNameString(regText, dic, enumDefine);
                notice.Title = content;
                notice.Content = content;
                if (!String.IsNullOrEmpty(isoDefine.LinkViewUrl))
                {
                    notice.LinkUrl = isoDefine.LinkViewUrl.IndexOf("?") >= 0 ?
                     isoDefine.LinkViewUrl + "&ID=" + formID :
                          isoDefine.LinkViewUrl + "?ID=" + formID;
                }
                notice.CreateDate = DateTime.Now;
                notice.RelateID = formID;
                notice.RelateType = sendType;
                notice.CreateUserID = dic.GetValue("CreateUserID");
                notice.CreateUserName = dic.GetValue("CreateUser");
                notice.EngineeringInfoID = engineeringInfo.ID;
                notice.IsFromSys = "True";
                this.EPCEntites.Set<S_I_Notice>().Add(notice);
            }
        }


        string replaceNameString(string regString, Dictionary<string, object> data, List<Dictionary<string, object>> EnumDefine)
        {
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(regString, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var sValue = data.GetValue(value);
                var define = EnumDefine.FirstOrDefault(d => d["FieldName"].ToString() == value);
                if (define != null)
                {
                    var enumValue = "";
                    var EnumService = FormulaHelper.GetService<IEnumService>();
                    foreach (var item in sValue.Split(','))
                    {
                        enumValue += EnumService.GetEnumText(define.GetValue("EnumKey"), item) + ",";
                    }
                    return enumValue.TrimEnd(',');
                }
                return sValue;
            });
            return result;
        }

        /// <summary>
        /// 设置SortIndex 字段的默认值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        private void SetSortIndexDefaultValue<TEntity>(TEntity entity) where TEntity : class, new()
        {
            var pi = typeof(TEntity).GetProperty("SortIndex");
            if (pi != null && pi.GetValue(entity, null) == null)
            {
                if (pi.PropertyType.FullName.IndexOf("Double") >= 0)
                {
                    pi.SetValue(entity, 0.0, null);
                }
                else
                {
                    pi.SetValue(entity, 0, null);
                }
            }
        }
    }
}