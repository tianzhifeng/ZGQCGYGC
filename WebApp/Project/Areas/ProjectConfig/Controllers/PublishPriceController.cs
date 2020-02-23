using Formula.Helper;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Formula;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class PublishPriceController : BaseConfigController<S_CAD_PublishPrice>
    {
        public override ActionResult List()
        {
            var specifications = EnumBaseHelper.GetEnumDef("Project.BorderSize").EnumItem.ToList();
            ViewBag.Specifications = specifications;
            return View();
        }
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var qbPublicationType = string.Empty;
            foreach (var item in qb.Items)
            {
                if (item.Field == "PublicationType")
                {
                    qbPublicationType = item.Value.ToString();
                }
            }

            var publicationType = EnumBaseHelper.GetEnumDef("Project.PublicationType").EnumItem.ToList();
            var specifications = EnumBaseHelper.GetEnumDef("Project.BorderSize").EnumItem.ToList();
            var publishPrices = this.entities.Set<S_CAD_PublishPrice>().ToList();
            if (!string.IsNullOrEmpty(qbPublicationType))
            {
                publicationType = publicationType.Where(a => a.Name.Contains(qbPublicationType)).ToList();
            }
            var list = new List<Dictionary<string, object>>();
            foreach (var type in publicationType)
            {
                var item = new Dictionary<string, object>();
                item.Add("PublicationType", type.Code);
                foreach (var specification in specifications)
                {
                    var price = publishPrices.FirstOrDefault(a => a.PublicationType == type.Code && a.Specification == specification.Code);
                    if (price == null)
                        item.Add(specification.Code, 0);
                    else
                        item.Add(specification.Code, price.Price);
                }
                list.Add(item);
            }
            return Json(list);
        }

        public JsonResult SaveListData(string listData)
        {
            var list = JsonHelper.ToObject<List<S_CAD_PublishPrice>>(listData);
            var publishPrices = this.entities.Set<S_CAD_PublishPrice>().ToList();
            foreach (var item in list)
            {
                var price = publishPrices.FirstOrDefault(a => a.PublicationType == item.PublicationType && a.Specification == item.Specification);
                if (price == null)
                {
                    price = new S_CAD_PublishPrice();
                    price.ID = FormulaHelper.CreateGuid();
                    price.PublicationType = item.PublicationType;
                    price.Specification = item.Specification;
                    this.entities.Set<S_CAD_PublishPrice>().Add(price);
                }
                price.Price = item.Price;
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
