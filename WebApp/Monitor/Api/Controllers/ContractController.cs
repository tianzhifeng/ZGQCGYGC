using System.Web.Http;
using Monitor.Logic.BusinessFacade;
using Monitor.Logic.Helper;

namespace Monitor.Api.Controllers
{
    public class ContractController : ApiController
    {
        [HttpGet]
        [ActionName("Detail")]
        public ResultDTO Get(string id)
        {
            return ContractBll.GetDetail(id);
        }

        [HttpGet]
        [ActionName("ReceiptRecord")]
        public ResultDTO ReceiptRecord(string id)
        {
           return ContractBll.GetReceiptRecords(id);
        }

        [HttpGet]
        [ActionName("PlanReceipt")]
        public ResultDTO PlanReceipt(string id)
        {
            return ContractBll.GetPlanReceipts(id);
        }

        public ResultDTO Put(string id,string date)
        {
            return ContractBll.UpdatePlanReceiptDate(id, date);
        }
    }
}
