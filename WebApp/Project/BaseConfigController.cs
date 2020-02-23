using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using Formula;
using Project.Logic.Domain;
using Project.Logic;
using Formula.DynConditionObject;
using System.Reflection;
using Formula.Helper;
using Config;

namespace Project
{
    public class BaseConfigController:MvcAdapter.BaseController
    {
        private DbContext _entities = null;
        protected override DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<BaseConfigEntities>();
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
    }

    public class BaseConfigController<T> : MvcAdapter.BaseController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<BaseConfigEntities>();
                }
                return _entities;
            }
        }

        UserInfo _userInfo;
        protected UserInfo CurrentUserInfo
        {
            get {
                if (_userInfo == null)
                    _userInfo = FormulaHelper.GetUserInfo();
                return _userInfo;
            }
        }

        protected virtual List<Dictionary<string, object>> GetRequestList(string paramName)
        {
            string data = this.Request[paramName];
            if (String.IsNullOrEmpty(data))
                return new List<Dictionary<string, object>>();
            return JsonHelper.ToList(data);
        }

        protected virtual Dictionary<string,object> GetRequestForm(string paramName)
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
                return Json(entity);
            return Json(entity);
        }

        public virtual T GetEntityByID<T>(string ID) where T : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }

        public override System.Web.Mvc.JsonResult DeleteNode()
        {
            return base.DeleteNode();
        }
    }
}