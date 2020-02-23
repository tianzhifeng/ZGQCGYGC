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

namespace EPC.Areas.Design.Controllers
{
    public class BOMPlanController : EPCController<S_P_Bom>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            //默认展现所有（树节点展开至PBS定义的最下层）
            var deepPBSStruct = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).LastOrDefault();
            if (deepPBSStruct != null)
            {
                ViewBag.ExpandLevel = deepPBSStruct.NodeType;
            }
            else
            {
                ViewBag.ExpandLevel = "";
            }
            var tab = new Tab();
            var catagory = CategoryFactory.GetCategory("Base.BOMMajor", "专业分类", "MajorCode");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetTreeList(string EngineeringInfoID, QueryBuilder qb)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Exception("未能找到指定的项目信息");
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pBomQuery = this.entities.Set<S_P_Bom>()
                .GroupJoin(this.entities.Set<T_E_BomFetchData_BomDetail>(), a => a.ID, b => b.BomID,
                (a, b) => new { Bom = a, Fetch = b.OrderByDescending(t => t.T_E_BomFetchData.CreateDate).FirstOrDefault(), SortIndex = a.SortIndex })
                .Where(c => c.Bom.EngineeringInfoID == engineeringInfo.ID);
            if (qb.Items.Count > 0)
            {
                string major = qb.Items[0].Value.ToString();
                pBomQuery = pBomQuery.Where(a => a.Bom.MajorCode == major);
            }

            var pBomList = pBomQuery.ToList();
            var pbsList = engineeringInfo.S_I_PBS./*Where(c => c.NodeType != "Root").*/OrderBy(c => c.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in pbsList)
            {
                var dic = FormulaHelper.ModelToDic<S_I_PBS>(item);
                var details = pBomList.Where(c => c.Bom.PBSNodeID == item.ID).ToList();
                if (pBomList.Count(c => !string.IsNullOrEmpty(c.Bom.PBSNodeFullID) && c.Bom.PBSNodeFullID.StartsWith(item.FullID)) == 0)
                {
                    continue;
                }
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_P_Bom>(detail.Bom);
                    if (detail.Fetch != null)
                    {
                        if (String.IsNullOrEmpty(detail.Bom.Attachment) && detail.Fetch.T_E_BomFetchData.FlowPhase != "End")
                        {
                            detailDic.SetValue("FetchState", "Processing");
                        }
                        else
                        {
                            detailDic.SetValue("FetchState", "Finish");
                        }
                    }
                    else
                    {
                        detailDic.SetValue("FetchState", "UnFinish");
                    }


                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

    }
}
