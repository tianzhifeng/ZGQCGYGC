using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;

namespace Project.Controllers
{
    public class ProjectApiController : ApiController
    {
        public IEnumerable<S_I_ProjectInfo> GetProject(string customerID)
        {
            LogWriter.Info("初始化项目管理内容");
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            return entities.S_I_ProjectInfo.Where(p => p.CustomerID == customerID).ToList();
        }
    }
}