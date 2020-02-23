using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Formula;
using HR.Logic.Domain;
using Config;


namespace HR
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
                    _entities = FormulaHelper.GetEntities<HREntities>();
                }
                return _entities;
            }
        }

        private UserInfo _userInfo = null;
        protected UserInfo UserInfo
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = FormulaHelper.GetUserInfo();
                }
                return _userInfo;
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
                    _entities = FormulaHelper.GetEntities<HREntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseFlowController<T> : MvcAdapter.BaseFlowController<T> where T : class,new()
    {
        private DbContext _entities;
        protected override DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<HREntities>();
                }
                return _entities;
            }
        }
    }

}