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
    public class BusinessOverviewController : ApiController
    {
        public ResultDTO Get(string type)
        {
            return BusinessOverviewBll.GetData(type);
        }
    }
}
