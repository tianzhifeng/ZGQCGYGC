using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Formula;

namespace Base.Controllers
{
    public class MessageController : ApiController
    {
        [ActionName("SendMsgToPerson")]
        [HttpPost]
        public string SendMsgToPerson([FromBody]MSG msg)
        {
            IMessageService service = FormulaHelper.GetService<IMessageService>();
            service.SendMsg(msg.title, msg.content, msg.link, "", msg.receiverIDs, msg.receiverNames);
            return "sucess";
        }

        public struct MSG
        {
            public string title { get; set; }
            public string content { get; set; }
            public string link { get; set; }
            public string receiverIDs { get; set; }
            public string receiverNames { get; set; }
        }
    }
}
