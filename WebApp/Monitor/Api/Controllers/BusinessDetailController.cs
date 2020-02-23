using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class BusinessDetailController : ApiController
    {
        public ResultDTO Get(string condition,string type)
        {
            return BusinessDetailBll.GetData(condition, type);
        }

        public ResultDTO Get(string condition,string type, string id)
        {
            return ContractBll.GetContracts(condition, type, id);
        }
    }
}
