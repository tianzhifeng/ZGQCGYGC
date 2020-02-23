using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class ProjectIndexController : ApiController
    {
        public ResultDTO Get()
        {
            return ProjectIndexBll.GetData();
        }
    }
}
