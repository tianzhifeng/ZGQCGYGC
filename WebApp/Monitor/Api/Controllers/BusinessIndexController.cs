using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class BusinessIndexController : ApiController
    {
        public ResultDTO Get()
        {
            return BusinessIndexBll.GetData();
        }
    }
}
