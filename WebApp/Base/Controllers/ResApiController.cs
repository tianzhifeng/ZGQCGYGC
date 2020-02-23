using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using Formula;
using System.Data;
using Base.Logic.Domain;

namespace Base.Controllers
{
    public class ResApiController : ApiController
    {
        [System.Web.Http.ActionName("GetUserRes")]
        [System.Web.Http.HttpGet]
        public IEnumerable<Config.Res> GetUserRes()
        {
            string url = Request.RequestUri.ParseQueryString().Get("url");
            string type = Request.RequestUri.ParseQueryString().Get("type");
            string userID = Request.RequestUri.ParseQueryString().Get("userID");      

            var resService = Formula.FormulaHelper.GetService<IResService>();
            return resService.GetRes(url, type, userID);
        }

        [System.Web.Http.ActionName("GetMenus")]
        [System.Web.Http.HttpGet]
        public DataTable _GetMenus(string userID)
        {
            var entities = Formula.FormulaHelper.GetEntities<BaseEntities>();

            var  s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                var mobileTerminaMenuRooID = s_A_Res.ID;
                return FormulaHelper.GetService<IResService>().GetResTree(mobileTerminaMenuRooID, userID);
            }
            return null;
        }
    }
}
