using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Formula;
using Config;
using Formula.DynConditionObject;
using Formula.Helper;
using System.Reflection;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto
{
    public class BaseController : MvcAdapter.BaseController
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }

        public virtual T GetEntityByID<T>(string ID) where T : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }
    }

    public class BaseController<T> : MvcAdapter.BaseController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }
    }
    public class BaseFlowController<T> : MvcAdapter.BaseFlowController<T> where T : class,new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                }
                return _entities;
            }
        }
        public virtual T GetEntityByID<T>(string ID) where T : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.entities.Set<T>().FirstOrDefault(spec.GetExpression<T>());
            return result;
        }

    }  
}