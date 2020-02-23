using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.ExpenseControl.Controllers
{
    public class BidController : EPCController<S_I_CBS_BidOffer>
    {
        public ActionResult TreeList()
        {
            return View();
        }

        public JsonResult GetTreeList(string EngineeringInfoID)
        {
            var result = new List<Dictionary<string, object>>();
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法展现工程投标概算");
            var cbsList = engineeringInfo.S_I_CBS.OrderBy(c => c.SortIndex).ToList();
            var detailList = this.entities.Set<S_I_CBS_BidOffer>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).OrderBy(c => c.ID).OrderBy(c=>c.SortIndex).ToList();
            foreach (var item in cbsList)
            {
                var dic = FormulaHelper.ModelToDic<S_I_CBS>(item);
                result.Add(dic);
                var details = detailList.Where(c => c.CBSID == item.ID).ToList();
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_I_CBS_BidOffer>(detail);
                    detailDic.SetValue("NodeType", "Detail");
                    detailDic.SetValue("Tender", detail.TotalValue);
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

    }
}
