using Formula.Exceptions;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class FixedAssetsController : OfficeAutoFormContorllor<T_SceneMark_Dictionaries>
    {

        /// <summary>
        /// 获取标识入库列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDictList()
        {

            return Json(BusinessEntities.Set<T_SceneMark_Dictionaries>().ToList().Select(x=> new { Name=x.Name, Specifications= x.Specifications,Level=x.Level, UnitPrice=x.UnitPrice }));
        }

        /// <summary>
        /// 标识字典删除事件
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteDict()
        {
            var id = GetQueryString("DictID");
            var dictModel = BusinessEntities.Set<T_SceneMark_Dictionaries>().FirstOrDefault(d => d.ID == id);
            if (dictModel != null)
            {
                if (!string.IsNullOrWhiteSpace(dictModel.StockQuantity) && Convert.ToInt32(dictModel.StockQuantity) != 0)
                {
                    return Json(new { msg = "库存数量不为0，不能删除" });
                }
                else
                {
                    BusinessEntities.Set<T_SceneMark_Dictionaries>().Remove(dictModel);
                    BusinessEntities.SaveChanges();
                }

            }
            return Json(new { msg = "OK" });
        }

        /// <summary>
        /// 根据类型获取工装单价
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPriceByType()
        {
            var id = GetQueryString("Category");
            var model = BusinessEntities.Set<T_WorkClothes_Stock>().FirstOrDefault();
            var price = 0.0;
            switch (id)
            {
                case "管理夏装": price = Convert.ToDouble(model.ManageSummerPrice); break;
                case "管理秋装": price = Convert.ToDouble(model.ManageAutumnPrice); break;
                case "管理冬装": price = Convert.ToDouble(model.ManageWinterPrice); break;
                case "工人夏装": price = Convert.ToDouble(model.WorkerSummerPrice); break;
                case "工人秋装": price = Convert.ToDouble(model.WorkerAutumnPrice); break;
                default:
                    break;
            }

            return Json(new { result= price });
        }

    }
}
