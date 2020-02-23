using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Base.Logic.Domain;
using Formula.Helper;
using Config.Logic;

namespace Base.Controllers
{
    public class AccountsController : ApiController
    {
        [HttpGet]
        public string GetUserAccounts()
        {
            var entities = Formula.FormulaHelper.GetEntities<BaseEntities>();
            var users = entities.Set<S_A_User>().Where(c => c.IsDeleted != "1").ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in users)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("userName", item.Name);
                dic.SetValue("systemName", item.Code);
                dic.SetValue("email", String.IsNullOrEmpty(item.Email) ? item.Code + "@qq.com" : item.Email);
                result.Add(dic);
            }
            var json = JsonHelper.ToJson(result);
            return json;
        }
    }
}