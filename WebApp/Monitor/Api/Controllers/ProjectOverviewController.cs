using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class ProjectOverviewController : ApiController
    {
        [HttpGet]
        [ActionName("List")]
        public ResultDTO GetList(string condition)
        {
            return ProjectOverviewBll.GetPrjList(condition);
        }

        public ResultDTO Get(string id)
        {
            return ProjectOverviewBll.GetPrjMileStones(id);
        }
    }
}
