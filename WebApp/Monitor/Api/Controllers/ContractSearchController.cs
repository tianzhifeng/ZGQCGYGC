using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class ContractSearchController : ApiController
    {
        public ResultDTO Get(string content)
        {
            return ContractSearchBll.GetSearchData(content);
        }

        public ResultDTO Get(string id,string type)
        {
            return ContractSearchBll.SearchDetail(id, type);
        }
    }
}
