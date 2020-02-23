using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Logic
{
    public class ButtonInfo
    {
        public string id { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string onclick { get; set; }
        public RoutingParams routingParams { get; set; }
        public int sortIndex { get; set; }
    }

    public class RoutingParams
    {
        public string routingID { get; set; }
        public string routingCode { get; set; }
        public string routingName { get; set; }
        public string selectMode { get; set; }
        public bool selectAgain { get; set; }
        public bool closeForm { get; set; }
        public string orgIDs { get; set; }
        public string roleIDs { get; set; }
        public string userIDs { get; set; }
        public string excudeUserIDs { get; set; }
        public List<Dictionary<string, object>> userIDsGroup { get; set; }
        public string notNullFields { get; set; }
        public string defaultComment { get; set; }
        public bool mustInputComment { get; set; }
        public string signatureDivID { get; set; }
        public string signatureProtectFields { get; set; }
        public string signatureField { get; set; }
        public string orgIDFromField { get; set; }
        public string roleIDsFromField { get; set; }
        public string userIDsFromField { get; set; }
        public string userIDsGroupFromField { get; set; }
        public bool flowComplete { get; set; }
        public string inputSignPwd { get; set; }
        public string signatureType { get; set; }
    }

}
