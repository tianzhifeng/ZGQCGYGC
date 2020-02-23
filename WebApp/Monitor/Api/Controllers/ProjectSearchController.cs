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
    public class ProjectSearchController : ApiController
    {
        public ResultDTO Get(string content)
        {
            return ProjectSearchBll.GetSearchData(content);
        }

        public ResultDTO Get(string id, string type)
        {
            return ProjectSearchBll.SearchDetail(id, type);
        }
    }
}
