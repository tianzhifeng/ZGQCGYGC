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
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using MvcAdapter;


namespace Project
{
    public class ProjectController : MvcAdapter.BaseController
    {
        private DbContext _entities = null;
        protected override DbContext entities
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

        public virtual T CreateEmptyEntity<T>(string ID = "") where T : class, new()
        {
            var result = new T();
            if (String.IsNullOrEmpty(ID))
                FormulaHelper.SetProperty(result, "ID", FormulaHelper.CreateGuid());
            else
                FormulaHelper.SetProperty(result, "ID", ID);
            return result;
        }

        public virtual T GetEntityByID<T>(string ID) where T : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }
    }

    public class ProjectController<T> : MvcAdapter.BaseController<T> where T : class, new()
    {
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

        protected virtual void BeforeSave(T entity, bool isNew) { }

        protected virtual void AfterSave(T entity, bool isNew) { }

        protected virtual void BeforeGetMode(string id) { }

        protected virtual void AfterGetMode(T entity, bool isNew) { }

        public virtual void FillQueryBuilderFilter(QueryBuilder qb, bool igorPtyExist = false)
        {
            string tokenKey = !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TokenKey"]) ? System.Configuration.ConfigurationManager.AppSettings["TokenKey"] : "GWToken";
            //地址栏参数作为查询条件
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key == null || key == "_" || key == "_t" || key == "_winid" || key == "ID" || key == "FullID" || key == "FULLID" || key.StartsWith("$"))
                    continue;
                if (key.ToLower() == tokenKey.ToLower()) continue;
                if (key.ToLowerInvariant() == "functype") continue;
                if (igorPtyExist)
                    qb.Add(key, QueryMethod.In, Request[key].Split(','));
                else
                {
                    if (typeof(T).GetProperty(key) != null)
                    {
                        qb.Add(key, QueryMethod.In, Request[key].Split(','));
                    }
                    else if (typeof(T).GetProperty(key.ToUpper()) != null)//兼容Oracle
                    {
                        qb.Add(key, QueryMethod.In, Request[key].Split(','));
                    }
                }
            }
        }

        public virtual void FillQueryBuilderFilter<TEntity>(QueryBuilder qb, bool igorPtyExist = false) where TEntity : class
        {
            string tokenKey = !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TokenKey"]) ? System.Configuration.ConfigurationManager.AppSettings["TokenKey"] : "GWToken";
            //地址栏参数作为查询条件
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key == null || key == "_" || key == "_t" || key == "_winid" || key == "ID" || key == "FullID" || key == "FULLID" || key.StartsWith("$"))
                    continue;
                if (key.ToLower() == tokenKey.ToLower()) continue;
                if (key.ToLowerInvariant() == "functype") continue;
                if (igorPtyExist)
                    qb.Add(key, QueryMethod.In, Request[key].Split(','));
                else
                {
                    if (typeof(TEntity).GetProperty(key) != null)
                    {
                        qb.Add(key, QueryMethod.In, Request[key].Split(','));
                    }
                    else if (typeof(TEntity).GetProperty(key.ToUpper()) != null)//兼容Oracle
                    {
                        qb.Add(key, QueryMethod.In, Request[key].Split(','));
                    }
                }
            }
        }

        public override System.Web.Mvc.JsonResult GetModel(string id)
        {
            BeforeGetMode(id);
            var entity = this.GetEntity<T>(id);
            bool isNew = false;
            if (entities.Entry<T>(entity).State == System.Data.EntityState.Added || entities.Entry<T>(entity).State == System.Data.EntityState.Detached)
                isNew = true;
            AfterGetMode(entity, isNew);
            return Json(entity);
        }

        public override System.Web.Mvc.JsonResult Delete()
        {
            string listIDs = Request["ListIDs"];
            Specifications res = new Specifications();
            res.AndAlso("ID", listIDs.Split(','), QueryMethod.In);
            var list = entities.Set<T>().Where(res.GetExpression<T>()).ToList();
            this.BeforeDelete(list);
            foreach (var item in list)
                entities.Set<T>().Remove(item);
            entities.SaveChanges();
            this.AfterDelete(list);
            return Json("");
        }

        public override System.Web.Mvc.JsonResult Save()
        {
            bool isNew = false;
            var entity = UpdateEntity<T>();
            if (this.entities.Entry<T>(entity).State == System.Data.EntityState.Detached
                || this.entities.Entry<T>(entity).State == System.Data.EntityState.Added)
                isNew = true;
            BeforeSave(entity, isNew);
            entities.SaveChanges();
            AfterSave(entity, isNew);
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


}