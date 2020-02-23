using MvcAdapter;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.ProtectionArticle.Controllers
{
    public class LedgerController : OfficeAutoController<T_ProtectionArticle_Ledger>
    {
        //
        // GET: /ProtectionArticle/Ledger/
        public override JsonResult GetList(QueryBuilder qb)
        {

            var model = entities.Set<T_ProtectionArticle_Ledger>().FirstOrDefault();
            var lst = new List<Ledger>();
            lst.Add(new Ledger
            {
                id=model.ID,
                category = "安全帽",
                TotalQuantity = Convert.ToInt32(model.SafetyHatQuantity),
                UnitPrice = Convert.ToDouble(model.SafetyHatPrice),
                TotalPrice = Convert.ToDouble(model.SafetyHatPrice) * Convert.ToInt32(model.SafetyHatQuantity)

            });
            lst.Add(new Ledger
            {
                id = model.ID,
                category = "防护鞋",
                Size35Quantity = Convert.ToInt32(model.Size35Quantity),
                Size36Quantity = Convert.ToInt32(model.Size36Quantity),
                Size37Quantity = Convert.ToInt32(model.Size37Quantity),
                Size38Quantity = Convert.ToInt32(model.Size38Quantity),
                Size39Quantity = Convert.ToInt32(model.Size39Quantity),
                Size40Quantity = Convert.ToInt32(model.Size40Quantity),
                Size41Quantity = Convert.ToInt32(model.Size41Quantity),
                Size42Quantity = Convert.ToInt32(model.Size42Quantity),
                Size43Quantity = Convert.ToInt32(model.Size43Quantity),
                Size44Quantity = Convert.ToInt32(model.Size44Quantity),
                Size45Quantity = Convert.ToInt32(model.Size45Quantity),
                Size46Quantity = Convert.ToInt32(model.Size46Quantity),
                TotalQuantity = Convert.ToInt32(model.SafetyShoesQuantity),
                UnitPrice = Convert.ToDouble(model.SafetyShoesPrice),
                TotalPrice = Convert.ToDouble(model.SafetyShoesPrice) * Convert.ToInt32(model.SafetyShoesQuantity)

            });
            return Json(lst);
        }
        public class Ledger {
            public string id { get; set; }
            public string category { get; set; }
            [DefaultValue(0)]
            public int Size35Quantity { get; set; }
            [DefaultValue(0)]
            public int Size36Quantity { get; set; }
            [DefaultValue(0)]
            public int Size37Quantity { get; set; }
            [DefaultValue(0)]
            public int Size38Quantity { get; set; }
            [DefaultValue(0)]
            public int Size39Quantity { get; set; }
            [DefaultValue(0)]
            public int Size40Quantity { get; set; }
            [DefaultValue(0)]
            public int Size41Quantity { get; set; }
            [DefaultValue(0)]
            public int Size42Quantity { get; set; }
            [DefaultValue(0)]
            public int Size43Quantity { get; set; }
            [DefaultValue(0)]
            public int Size44Quantity { get; set; }
            [DefaultValue(0)]
            public int Size45Quantity { get; set; }
            [DefaultValue(0)]
            public int Size46Quantity { get; set; }

            [DefaultValue(0)]
            public int TotalQuantity { get; set; }
            [DefaultValue(0D)]
            //单价
            public double UnitPrice { get; set; }
            [DefaultValue(0D)]
            public double TotalPrice { get; set; } 
        }

    }
}
