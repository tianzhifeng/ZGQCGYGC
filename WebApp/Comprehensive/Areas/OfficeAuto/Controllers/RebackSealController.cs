using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Comprehensive.Logic.Domain;
using Formula.Helper;
using Newtonsoft.Json;
using EPC;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class RebackSealController : ComprehensiveFormController<T_SealManage_SealBorrow>
    {
        public JsonResult RebackSealResult(string listData)
        {
            var list = JsonHelper.ToList(listData);
            foreach (var item in list)
            {
                object value = null;
                item.TryGetValue("ID", out value);
                if (value != null)
                {
                    var entity = this.GetEntityByID(value.ToString(), false);
                    if (entity == null)
                    {
                        continue;
                    }
                    var entityList = this.ComprehensiveDbContext.Set<T_SealManage_SealBorrow>().Where(d => d.ID == entity.ID).ToList();
                    foreach (var e in entityList)
                    {
                        e.IsReback = "是";
                        e.ReturnTime = DateTime.Now;
                    }
                }
            }
            this.ComprehensiveDbContext.SaveChanges();
            return Json("");
        }

    }
}
