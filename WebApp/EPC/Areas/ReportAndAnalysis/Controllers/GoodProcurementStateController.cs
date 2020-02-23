using EPC.Logic.Domain;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Formula.Helper;
using System.Linq.Expressions;
using Formula.DynConditionObject;
using Formula;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class GoodProcurementStateController : EPCController
    {
        public ActionResult Index()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            //默认展现所有（树节点展开至PBS定义的最下层）
            var list = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 1)
                {
                    ViewBag.ExpandLevel = i;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);

            var tab = new Tab();
            var catagory = CategoryFactory.GetCategory("Base.BOMMajor", "专业分类", "MajorCode");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb, string EngineeringInfoID, bool ShowDelay)
        {
            var pbsList = entities.Set<S_I_PBS>().Where(c => c.EngineeringInfoID == EngineeringInfoID && c.ParentID != "");

            Expression<Func<S_P_Bom, bool>> predicate = a => true;
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;

            if (ShowDelay)
            {
                predicate = a =>
                    //提资延期
                    (a.PlanFetchDate != null
                    &&
                    ((a.FactFetchDate != null && a.FactFetchDate > a.PlanFetchDate)
                    || (a.FactFetchDate == null && DateTime.Now > a.PlanFetchDate)))
                    ||
                    //询比价延期
                    (a.PlanInvitationDate != null
                    &&
                    ((a.FactInvitationDate != null && a.FactInvitationDate > a.PlanInvitationDate)
                    || (a.FactInvitationDate == null && DateTime.Now > a.PlanInvitationDate)))
                    ||
                    //合同签订延期
                    (a.PlanContractDate != null
                    &&
                    ((a.FactContractDate != null && a.FactContractDate > a.PlanContractDate)
                    || (a.FactContractDate == null && DateTime.Now > a.PlanContractDate)))
                    ||
                    //设备返资延期
                    (a.PlanReturnDate != null
                    &&
                    ((a.FactReturnDate != null && a.FactReturnDate > a.PlanReturnDate)
                    || (a.FactReturnDate == null && DateTime.Now > a.PlanReturnDate)))
                    ||
                    //物资到货延期
                    (a.PlanArrivedDate != null
                    &&
                    ((a.FactArrivedDate != null && a.FactArrivedDate > a.PlanArrivedDate)
                    || (a.FactArrivedDate == null && DateTime.Now > a.PlanArrivedDate)))
                    ;
            }

            var pBomList = entities.Set<S_P_Bom>().Where(predicate).Where(qb).Where(a => pbsList.Any(b => b.ID == a.PBSNodeID));
            //pbs的最低阶子节点
            var pbsList2 = pbsList.Where(a => pbsList.Any(b => b.ParentID != a.ID));
            //不包含bom的pbs最低阶子节点
            var pbsList3 = pbsList2.Where(a => pBomList.Any(b => b.ParentID != a.ID));
            //查询排除不含bom的pbs最低阶子节点及其对应的所有父节点后的pbs列表
            var pbsList4 = pbsList.Where(a => pbsList3.Any(b => !b.FullID.Contains(a.ID)));

            //合并pbs与bom表
            var results = pbsList4.Select(a => new
            {
                ID = a.ID,
                ParentID = a.ParentID,
                Name = a.Name,
                Code = a.Code,
                MajorCode = "",
                SortIndex = a.SortIndex,
                NodeType = a.NodeType,

                Quantity = "",
                ContractQuantity = "",
                ArriveQuantity = "",
                PlanFetchDate = "",
                FactFetchDate = "",
                PlanInvitationDate = "",
                FactInvitationDate = "",
                PlanContractDate = "",
                FactContractDate = "",
                PlanReturnDate="", 
                FactReturnDate="", 
                PlanArrivedDate="",
                FactArrivedDate="",
                Unit = ""
            })
            .Union(pBomList.Select(a => new
            {
                ID = a.ID,
                ParentID = a.ParentID,
                Name = a.Name,
                Code = a.Code,
                MajorCode = a.MajorCode,
                SortIndex = a.SortIndex,
                NodeType = "Detail",

                Quantity = a.Quantity == null ? "0" : SqlFunctions.StringConvert(a.Quantity),
                ContractQuantity = a.ContractQuantity == null ? "0" : SqlFunctions.StringConvert(a.ContractQuantity),
                ArriveQuantity = a.ArriveQuantity == null ? "0" : SqlFunctions.StringConvert(a.ArriveQuantity),
                PlanFetchDate = a.PlanFetchDate == null ? "" : SqlFunctions.DateName("yyyy", a.PlanFetchDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.PlanFetchDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.PlanFetchDate.Value),
                FactFetchDate = a.FactFetchDate == null ? "" : SqlFunctions.DateName("yyyy", a.FactFetchDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.FactFetchDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.FactFetchDate.Value),

                PlanInvitationDate = a.PlanInvitationDate == null ? "" : SqlFunctions.DateName("yyyy", a.PlanInvitationDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.PlanInvitationDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.PlanInvitationDate.Value),
                FactInvitationDate = a.FactInvitationDate == null ? "" : SqlFunctions.DateName("yyyy", a.FactInvitationDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.FactInvitationDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.FactInvitationDate.Value),

                PlanContractDate = a.PlanContractDate == null ? "" : SqlFunctions.DateName("yyyy", a.PlanContractDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.PlanContractDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.PlanContractDate.Value),
                FactContractDate = a.FactContractDate == null ? "" : SqlFunctions.DateName("yyyy", a.FactContractDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.FactContractDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.FactContractDate.Value),

                PlanReturnDate = a.PlanReturnDate == null ? "" : SqlFunctions.DateName("yyyy", a.PlanReturnDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.PlanReturnDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.PlanReturnDate.Value),
                FactReturnDate = a.FactReturnDate == null ? "" : SqlFunctions.DateName("yyyy", a.FactReturnDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.FactReturnDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.FactReturnDate.Value),
       
                PlanArrivedDate = a.PlanArrivedDate == null ? "" : SqlFunctions.DateName("yyyy", a.PlanArrivedDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.PlanArrivedDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.PlanArrivedDate.Value),
                FactArrivedDate = a.FactArrivedDate == null ? "" : SqlFunctions.DateName("yyyy", a.FactArrivedDate.Value)
                                                               + "-" +
                                                               SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", a.FactArrivedDate.Value)).Trim()
                                                               + "-" +
                                                               SqlFunctions.DateName("dd", a.FactArrivedDate.Value),

                Unit = a.Unit
            })).OrderBy(a => a.SortIndex);

            return Json(results);
        }

        public JsonResult GetSing(string id)
        {
            var bom = entities.Set<S_P_Bom>().Find(id);
            if (bom == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到id为" + id + "的物资(S_P_Bom)数据");
            }

            dynamic result = new
            {
                Name = bom.Name,
                PlanFetchDate = bom.PlanFetchDate == null ? "" : bom.PlanFetchDate.Value.ToString("yyyy-MM-dd"),
                FactFetchDate = bom.FactFetchDate == null ? "" : bom.FactFetchDate.Value.ToString("yyyy-MM-dd"),
                bFetch = bom.FactFetchDate != null,
                bFetchDateDelay = bom.PlanFetchDate != null
                    &&
                    ((bom.FactFetchDate != null && bom.FactFetchDate > bom.PlanFetchDate)
                    || (bom.FactFetchDate == null && DateTime.Now > bom.PlanFetchDate)),

                PlanInvitationDate = bom.PlanInvitationDate == null ? "" : bom.PlanInvitationDate.Value.ToString("yyyy-MM-dd"),
                FactInvitationDate = bom.FactInvitationDate == null ? "" : bom.FactInvitationDate.Value.ToString("yyyy-MM-dd"),
                bInvitation = bom.FactInvitationDate != null,
                bInvitationDateDelay = bom.PlanInvitationDate != null
                    &&
                    ((bom.FactInvitationDate != null && bom.FactInvitationDate > bom.PlanInvitationDate)
                    || (bom.FactInvitationDate == null && DateTime.Now > bom.PlanInvitationDate)),

                PlanContractDate = bom.PlanContractDate == null ? "" : bom.PlanContractDate.Value.ToString("yyyy-MM-dd"),
                FactContractDate = bom.FactContractDate == null ? "" : bom.FactContractDate.Value.ToString("yyyy-MM-dd"),
                bContract = bom.FactContractDate != null,
                bContractDateDelay = bom.PlanContractDate != null
                    &&
                    ((bom.FactContractDate != null && bom.FactContractDate > bom.PlanContractDate)
                    || (bom.FactContractDate == null && DateTime.Now > bom.PlanContractDate)),
                
                PlanReturnDate = bom.PlanReturnDate == null ? "" : bom.PlanReturnDate.Value.ToString("yyyy-MM-dd"),
                FactReturnDate = bom.FactReturnDate == null ? "" : bom.FactReturnDate.Value.ToString("yyyy-MM-dd"),
                bReturn = bom.FactReturnDate != null,
                bReturnDateDelay = bom.PlanReturnDate != null
                    &&
                    ((bom.FactReturnDate != null && bom.FactReturnDate > bom.PlanReturnDate)
                    || (bom.FactReturnDate == null && DateTime.Now > bom.PlanReturnDate)), 
               
                PlanArrivedDate = bom.PlanArrivedDate == null ? "" : bom.PlanArrivedDate.Value.ToString("yyyy-MM-dd"),
                FactArrivedDate = bom.FactArrivedDate == null ? "" : bom.FactArrivedDate.Value.ToString("yyyy-MM-dd"),
                bArrived = bom.FactArrivedDate != null,
                bArrivedDateDelay = bom.PlanArrivedDate != null
                    &&
                    ((bom.FactArrivedDate != null && bom.FactArrivedDate > bom.PlanArrivedDate)
                    || (bom.FactArrivedDate == null && DateTime.Now > bom.PlanArrivedDate)),

                Quantity = bom.Quantity ?? 0,
                ContractQuantity = bom.ContractQuantity ?? 0,
                ArriveQuantity = bom.ArriveQuantity ?? 0
            };

            return Json(result);
        }
    }
}
