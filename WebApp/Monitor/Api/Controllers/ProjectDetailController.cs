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
    public class ProjectDetailController : ApiController
    {
        [HttpGet]
        [ActionName("PrjInfo")]
        public ResultDTO GetInfo(string id)
        {
            return ProjectDetailBll.GetPrjInfo(id);
        }

        [HttpGet]
        [ActionName("PrjUsers")]
        public ResultDTO GetUsers(string id,string condition)
        {
            return ProjectDetailBll.GetPrjUsers(id, condition);
        }
    }
}
