using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Formula;
using Workflow.Logic.Domain;

namespace OfficeAuto
{
    public class BaseWorkFlowController : MvcAdapter.BaseController
    {

        private DbContext _entities = null;
        protected override DbContext entities
        {
            get
            {
                if (_entities == null)
                    _entities = FormulaHelper.GetEntities<WorkflowEntities>();
                return _entities;
            }
        }
    }

    public class BaseWorkFlowController<T> : MvcAdapter.BaseController<T> where T : class, new()
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
