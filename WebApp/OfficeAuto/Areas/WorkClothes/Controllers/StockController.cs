using MvcAdapter;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.WorkClothes.Controllers
{
    public class StockController : OfficeAutoController<T_WorkClothes_Stock>
    {
        //
        // GET: /ProtectionArticle/Ledger/
        public override JsonResult GetList(QueryBuilder qb)
        {
            var model = entities.Set<T_WorkClothes_Stock>().FirstOrDefault();
            var lstDetail = entities.Set<T_WorkClothes_Stock_ClothesDetail>().ToList();

            List<Stock> lst = new List<Stock>();
            lst.Add(new Stock { Id = model.ID, Location = "天津", Category = "管理夏装", UnitPrice = Convert.ToDouble(model.ManageSummerPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "天津", Category = "管理秋装", UnitPrice = Convert.ToDouble(model.ManageAutumnPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "天津", Category = "管理冬装", UnitPrice = Convert.ToDouble(model.ManageWinterPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "天津", Category = "工人夏装", UnitPrice = Convert.ToDouble(model.WorkerSummerPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "天津", Category = "工人秋装", UnitPrice = Convert.ToDouble(model.WorkerAutumnPrice) });


            lst.Add(new Stock { Id = model.ID, Location = "洛阳", Category = "管理夏装", UnitPrice = Convert.ToDouble(model.ManageSummerPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "洛阳", Category = "管理秋装", UnitPrice = Convert.ToDouble(model.ManageAutumnPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "洛阳", Category = "管理冬装", UnitPrice = Convert.ToDouble(model.ManageWinterPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "洛阳", Category = "工人夏装", UnitPrice = Convert.ToDouble(model.WorkerSummerPrice) });
            lst.Add(new Stock { Id = model.ID, Location = "洛阳", Category = "工人秋装", UnitPrice = Convert.ToDouble(model.WorkerAutumnPrice) });
            lst.ForEach(x => {
                var detail = lstDetail.FirstOrDefault(d => d.Category == x.Category && d.Location == x.Location);
                if (detail != null)
                {
                    x.TotalQuantity = Convert.ToInt32(detail.TotalQuantity);
                    x.TotalPrice = x.TotalQuantity * x.UnitPrice;
                    x.Size155Quantity = Convert.ToInt32(detail.Size155Quantity);
                    x.Size160Quantity = Convert.ToInt32(detail.Size160Quantity);
                    x.Size165Quantity = Convert.ToInt32(detail.Size165Quantity);
                    x.Size170Quantity = Convert.ToInt32(detail.Size170Quantity);
                    x.Size175Quantity = Convert.ToInt32(detail.Size175Quantity);
                    x.Size180Quantity = Convert.ToInt32(detail.Size180Quantity);
                    x.Size185Quantity = Convert.ToInt32(detail.Size185Quantity);
                }
            });

            return Json(lst);
        }
        public class Stock
        {
            public string Id { get; set; }

            public string Location { get; set; }
            public string Category { get; set; }
            public int TotalQuantity { get; set; }
            //单价
            [DefaultValue(0D)]
            public double UnitPrice { get; set; } 
            [DefaultValue(0D)]
            public double TotalPrice { get; set; }
            [DefaultValue(0)]
            public int Size155Quantity { get; set; }
            [DefaultValue(0)]
            public int Size160Quantity { get; set; }
            [DefaultValue(0)]
            public int Size165Quantity { get; set; }
            [DefaultValue(0)]
            public int Size170Quantity { get; set; }
            [DefaultValue(0)]
            public int Size175Quantity { get; set; }
            [DefaultValue(0)]
            public int Size180Quantity { get; set; }
            [DefaultValue(0)]
            public int Size185Quantity { get; set; } 
        }

    }
}
