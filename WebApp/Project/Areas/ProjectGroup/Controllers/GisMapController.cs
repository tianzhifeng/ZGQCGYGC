using Config;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class GisMapController : ProjectController
    {
        public ActionResult Index()
        {
            string strViewPt = System.Configuration.ConfigurationManager.AppSettings["InitialViewPt"];
            var arr = (strViewPt ?? "error").Split(',');
            if (arr.Length > 1)
            {
                ViewBag.Long = arr[0];
                ViewBag.Lat = arr[1];
            }

            ViewBag.InitialZoomVal = 11;
            var list = FormulaHelper.GetEntities<BaseConfigEntities>().Set<S_T_HotRangeStatisticsConfig>().Where(a=>a.Enable == "1").ToList();
            var projZoomVal = (list.Max(a => a.MaxZoomVal) ?? 0) + 1;
            ViewBag.ProjZoomVal = projZoomVal;
            ViewBag.RangeConfig = JsonHelper.ToJson(FormulaHelper.GetEntities<BaseConfigEntities>().Set<S_T_HotRangeStatisticsConfig>().Where(a => a.Enable == "1").ToList());
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            if (qb.Items != null && qb.Items.Count != 0)
            {
                return Json(entities.Set<S_I_ProjectInfo>().Where(a => a.Long != null && a.Lat != null).Where(qb).ToList());
            }
            else
            {
                var ff = entities.Set<S_I_ProjectInfo>().Where(a => a.Long != null && a.Lat != null).ToList();
                return Json(entities.Set<S_I_ProjectInfo>().Where(a => a.Long != null && a.Lat != null));
            }            
        }

        public JsonResult GetProjectByArea(QueryBuilder qb)
        {
            string rangeSubItems = GetQueryString("rangeSubItems");
            string name = GetQueryString("name");
            string field = GetQueryString("field");
            string subField = GetQueryString("subField");
            
            string condition = null;
            if (!string.IsNullOrEmpty(field) && field.ToLower() != "null")
            {
                condition += string.Format(" (s_i_projectInfo.{0} = '{1}') ", field, name);
            }

            if (!string.IsNullOrEmpty(subField) && subField.ToLower() != "null")
            {
                if (string.IsNullOrEmpty(condition))
                {
                    condition = "";
                    condition += string.Format(" (CHARINDEX(s_i_projectInfo.{0} ,'{1}') > 0) ", subField, rangeSubItems);
                }
                else
                {
                    condition += string.Format(" or (CHARINDEX(s_i_projectInfo.{0} ,'{1}') > 0) ", subField, rangeSubItems);
                }
                
            }

            string sql = "select * from s_i_projectInfo  where (Long is not null and Lat is not null) and (" + (condition ?? "1=1") + ")";
            if (qb.Items != null && qb.Items.Count != 0)
            {
                return Json(SqlHelper.ExecuteList<S_I_ProjectInfo>(sql).AsQueryable().Where(qb).ToList());
            }
            else
            {
               return Json(SqlHelper.ExecuteList<S_I_ProjectInfo>(sql).AsQueryable().ToList());
            }
        }

        public JsonResult GetTree(string id)
        {
            var results = entities.Set<S_W_WBS>().Where(a => a.ProjectInfoID == id);
            return Json(results);
        }

        public JsonResult GetProjProperties(string projID)
        {
            var property = new Dictionary<string, object>();   
            {
                var marketDb = SQLHelper.CreateSqlHelper(ConnEnum.Market);
                string sql = @"SELECT isnull(ContractValue,0) as ContractValue, DATEDIFF(day,S_I_ProjectInfo.CreateDate,GETDATE()) AS DiffDate, isnull(PrintCount,0) as PrintCount,isnull(ProductCount,0) as ProductCount,latestProductDate, S_I_ProjectInfo.* FROM S_I_ProjectInfo
                             left join 
                             (select S_E_Product.ProjectInfoID, sum(isnull(PrintCount,0)) as PrintCount ,COUNT(S_E_Product.ID) as ProductCount,max(S_E_Product.CreateDate) as latestProductDate from S_E_Product group by S_E_Product.ProjectInfoID) product
                             on product.ProjectInfoID = S_I_ProjectInfo.ID 
                             left join ( select Sum(ProjectValue) as ContractValue, ProjectID  from [" + marketDb.DbName + @"].[dbo].S_C_ManageContract_ProjectRelation group by ProjectID)  ProjectContractInfo
                             on S_I_ProjectInfo.ID = ProjectContractInfo.ProjectID
                             where S_I_ProjectInfo.ID = '" + projID + "'";

                var dt = SqlHelper.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string json = JsonHelper.ToJson(dt);
                    var dicList = JsonHelper.ToList(json);

                    property = dicList[0];
                   
                    string submitText = "暂无成果";
                    if (property["latestProductDate"] != null)
                    {
                        DateTime lastestSubmitDate = Convert.ToDateTime(property["latestProductDate"]);
                        TimeSpan span = DateTime.Now - lastestSubmitDate.Date;
                        if (span.Days > 365)
                        { submitText = string.Format("{0}年前", span.Days / 365); }
                        else if (span.Days > 30)
                        { submitText = string.Format("{0}个月前", span.Days / 30); }
                        else if (span.Days > 7)
                        { submitText = string.Format("{0}周前", span.Days / 7); }
                        else if (span.Days >= 1)
                        { submitText = string.Format("{0}天前", span.Days); }
                        else
                        { submitText = "今天"; }
                    }
                    property["latestProductTitle"] = submitText;

                    string marketProjectInfoID = property["MarketProjectInfoID"].ToString();
                    //var costList = FormulaHelper.GetEntities<MarketEntities>().Set<S_FC_CostInfo>().Where(a => a.ProjectID == marketProjectInfoID && a.ProjectType == Market.Logic.Const.defaultDirectCostType).ToList();
                    //var directCostValue = costList.Where(a => a.CostType == Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
                    //var paymentCostValue = costList.Where(a => a.CostType != Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
                    //var totalCostValue = costList.Select(a => a.CostValue).Sum();
                    //property["TotalCostValue"] = string.Format("{0:N2}", totalCostValue / 10000);
                    //property["DirectValue"] = string.Format("{0:N2}", directCostValue / 10000);
                    //property["PaymentCostValue"] = string.Format("{0:N2}", paymentCostValue / 10000);
                    //property["ContractValue"] = string.Format("{0:N2}", paymentCostValue / 10000);                    
                }
            }

            var milestoneDicList = new List<Dictionary<string, object>>();
            {
                var tmpList = this.entities.Set<S_P_MileStone>().Where(d => d.ProjectInfoID == projID).ToList();
                var majorDic = new Dictionary<string, string>();

                var prjBaseHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                var majorDt = prjBaseHelper.ExecuteDataTable("select Name,Code from S_D_WBSAttrDefine where Type='Major'");
                foreach (DataRow item in majorDt.Rows)
                {
                    majorDic.Add(item["Code"].ToString(), item["Name"].ToString());
                }

                foreach (var milestone in tmpList)
                {
                    var dic = milestone.ToDic();

                    var finish = Project.Logic.ProjectCommoneState.Finish.ToString();
                    var dateNow = DateTime.Now.Date;
                    var stateSrc = "";
                    var milestoneTitle = "";
                    if (milestone.State == finish && milestone.PlanFinishDate.HasValue
                        && milestone.FactFinishDate.HasValue
                        && milestone.FactFinishDate.Value.Date > milestone.PlanFinishDate.Value)
                    {
                        stateSrc = "/Project/Scripts/Flag/images/DelayFinish.png";
                        milestoneTitle = "延期完成";
                    }
                    else if (milestone.State == finish && (!milestone.PlanFinishDate.HasValue
                       || !milestone.FactFinishDate.HasValue
                       || milestone.FactFinishDate.Value.Date <= milestone.PlanFinishDate.Value))
                    {
                        stateSrc = "/Project/Scripts/Flag/images/NormalFinish.png";
                        milestoneTitle = "正常完成";
                    }
                    else if (milestone.State != finish &&
                       milestone.PlanFinishDate.HasValue &&
                       dateNow > milestone.PlanFinishDate.Value)
                    {
                        stateSrc = "/Project/Scripts/Flag/images/DelayUndone.png";
                        milestoneTitle = "延期未完成";
                    }
                    else if (milestone.State != finish &&
                       milestone.PlanFinishDate.HasValue &&
                       (dateNow <= milestone.PlanFinishDate.Value)
                        || !milestone.PlanFinishDate.HasValue)
                    {
                        stateSrc = "/Project/Scripts/Flag/images/NormalUndone.png";
                        milestoneTitle = " 正常进行中";
                    }

                    dic["stateSrc"] = stateSrc;
                    dic["milestoneTitle"] = milestoneTitle;
                    if (milestone.PlanFinishDate != null)
                        dic["PlanFinishDate"] = milestone.PlanFinishDate.Value.ToString("yyyy-MM-dd");
                    else
                        dic["PlanFinishDate"] = "";

                    dic["majorName"] = "";
                    if (!string.IsNullOrEmpty(milestone.MajorValue))
                    {
                        if (majorDic.Keys.Contains(milestone.MajorValue))
                        {
                            dic["majorName"] = majorDic[milestone.MajorValue];                        
                        }
                        else { continue; }
                    }

                    milestoneDicList.Add(dic);
                }                
            }

            return Json(new { property = property, mileStoneCount = milestoneDicList.Count(), mileStoneList = milestoneDicList });
        }
    }
}
