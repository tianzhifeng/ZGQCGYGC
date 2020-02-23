using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Formula;
using Workflow.Logic.Domain;


namespace Workflow
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
                    _entities = FormulaHelper.GetEntities<WorkflowEntities>();
                }
                return _entities;
            }
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
                    _entities = FormulaHelper.GetEntities<WorkflowEntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseController<T, TN> : MvcAdapter.BaseController<T, TN>
        where T : class, new()
        where TN : class,new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<WorkflowEntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseController<T, TN, N> : MvcAdapter.BaseController<T, TN, N>
        where T : class, new()
        where TN : class,new()
        where N : class,new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<WorkflowEntities>();
                }
                return _entities;
            }
        }
    }
}
