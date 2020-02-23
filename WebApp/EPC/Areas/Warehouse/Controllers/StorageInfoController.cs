using EPC;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;
using Formula.Helper;

namespace EPC.Areas.Warehouse.Controllers
{
    public class StorageInfoController : EPCController<S_W_StorageInfo>
    {
        public JsonResult DelStorage(string listData)
        {
            var list = JsonHelper.ToList(listData);
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                var name = item.GetValue("Name");
                var accounts = this.entities.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == id).ToList();
                if (accounts.Count > 0)
                    throw new Formula.Exceptions.BusinessException("仓库【" + name + "】进行过入库操作，不允许删除");
                var storage = this.GetEntityByID<S_W_StorageInfo>(id);
                this.entities.Set<S_W_StorageInfo>().Remove(storage);
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}