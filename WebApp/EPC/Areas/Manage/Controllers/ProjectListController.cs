using Config;
using EPC.Logic;
using Formula;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Formula.Helper;
using System.Text.RegularExpressions;

namespace EPC.Areas.Manage.Controllers
{
    public class ProjectListController : EPCController
    {
        public ActionResult List()
        {
            var tab = new Tab();
            tab.Categories.AddRange(GetCats());
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            List<dynamic> catDic = new List<dynamic>();
            catDic.AddRange(tab.Categories.Select(a => new { ID = FormulaHelper.CreateGuid(), Name = a.Name, Key = a.Key }));
            ViewBag.Categories = JsonHelper.ToJson(catDic);
            return View();
        }

        public ActionResult CategoryParamConfig()
        {
            return View();
        }

        public JsonResult GetTreeList(string categoryOrder)
        {
            List<Category> cats = GetCats().ToList();
            List<Category> catForSelect = new List<Category>();
            
            if (!string.IsNullOrEmpty(categoryOrder))
            {
                string[] catOrderArr = categoryOrder.Split(',');
                foreach (var catOrder in catOrderArr)
                {
                    var category = cats.SingleOrDefault(a => a.Key == catOrder);                   
                    catForSelect.Add(category);
                }
            }
            //什么都没有则全部属性都可用来筛选
            else
            {
                catForSelect.AddRange(cats);
            }
           
            List<dynamic> treeDic = new List<dynamic>();
            FillTreeDic(ref treeDic, "", 0, catForSelect);
            return Json(treeDic);
        }

        public void FillTreeDic(ref List<dynamic> treeDic, string parentID, int index, List<Category> cats)
        {
            if (index == cats.Count)
            {
                return;
            }

            Category cat = cats[index];
            index++;
            foreach (var item in cat.Items.Where(a => a.Value != "All"))
            {
                string pID = FormulaHelper.CreateGuid();
                treeDic.Add(new { ID = pID, Value = item.Value, Name = item.Name, ParentID = parentID, Category = cat.Key, Field = cat.QueryField });
                FillTreeDic(ref treeDic, pID, index, cats);
            }
        }        

        public JsonResult GetList(QueryBuilder qb)
        {
            string cats = GetQueryString("categoryList");
            QueryBuilder finalQb = qb;
            //eps筛选
            if (!string.IsNullOrEmpty(cats))
            {
                string searchKey = GetQueryString("searchKey");
                List<Dictionary<string, object>> catDics = JsonHelper.ToList(cats);
                finalQb.Items.Clear();
                foreach(var catDic in catDics)
                {
                    finalQb.Add(catDic.GetValue("Field"), QueryMethod.In, catDic.GetValue("Value"));
                }

                if (!string.IsNullOrEmpty(searchKey))
                {
                    finalQb.Add("Name", QueryMethod.InLike, searchKey,"orGroup");
                    finalQb.Add("SerialNumber", QueryMethod.InLike, searchKey, "orGroup");
                }
            }

            string sql = @"SELECT S_I_Engineering.*,
                         ISNULL(PlanReceiptInfo.PlanValue,0) AS ContractPlanValue,
	                     ISNULL(sc.ContractRMBValue,0) AS ContractRMBValue,
	                     ISNULL(sccount.ReceiptValueCount,0) AS ReceiptValue,
	                     ISNULL(Convert(decimal(18,2),sccount.ReceiptValueCount/ContractRMBValue*100),0) AS ReceiptRate,
	                     ISNULL(receiptMonth.ReceiptValue,0) AS currentReceiptValue,
	                     isnull(workhours.ActualHours,0)ActualHours,ISNULL(cbs.Budget,0)Budget,
	                     NULL AS PlanFinishDate,
	                     NULL AS FinishState,
	                     NULL AS CurrentProgress,
	                     NULL AS BudgetHours
                         FROM dbo.S_I_Engineering
                         LEFT JOIN (select Sum(PlanValue) as PlanValue,ProjectInfoID from S_M_ContractInfo_ReceiptObj_PlanReceipt group by ProjectInfoID) PlanReceiptInfo on S_I_Engineering.ID=PlanReceiptInfo.ProjectInfoID 
                         LEFT JOIN (SELECT EngineeringInfoID,SUM(Budget) AS Budget FROM S_I_CBS WHERE ParentID='' and NodeType='Root' GROUP BY EngineeringInfoID)cbs  ON S_I_Engineering.ID = cbs.EngineeringInfoID
                         LEFT JOIN (SELECT SUM(WorkHourValue) AS ActualHours,ProjectID FROM [EPC_Comprehensive].[dbo].S_W_UserWorkHour GROUP BY ProjectID)workhours ON S_I_Engineering.ID = workhours.ProjectID
                         LEFT JOIN (SELECT SUM(ContractRMBValue) AS ContractRMBValue ,ProjectInfo FROM dbo.S_M_ContractInfo GROUP BY ProjectInfo)sc ON S_I_Engineering.ID = sc.ProjectInfo
                         LEFT JOIN (SELECT SUM(ReceiptValue) AS ReceiptValueCount,ProjectInfo FROM S_M_Receipt GROUP BY ProjectInfo)sccount ON sccount.ProjectInfo = S_I_Engineering.ID
                         LEFT JOIN (SELECT SUM(ReceiptValue)ReceiptValue,ProjectInfo,BelongYear,BelongMonth FROM
	                     (SELECT ReceiptValue,ProjectInfo,YEAR(ReceiptDate) AS BelongYear,MONTH(ReceiptDate) AS BelongMonth FROM dbo.S_M_Receipt)sr 
	                     where BelongMonth = Month(GETDATE())
	                     GROUP BY ProjectInfo,BelongYear,BelongMonth
                         )receiptMonth ON S_I_Engineering.ID = receiptMonth.ProjectInfo
                         ";

            var data = SqlHelper.ExecuteGridData(sql, finalQb);
            return Json(data);
        }

        public JsonResult FindCountOfNodeCondition(string listData)
        {
            List<Dictionary<string, object>> dicsList = JsonHelper.ToList(listData);
            List<string> nameList = new List<string>();
            foreach (var dics in dicsList)
            {
                string strCondition = "";
                var list = JsonHelper.ToList(dics.GetValue("Params"));
                foreach (var dic in list)
                {
                    if (strCondition != "")
                    {
                        strCondition += " and ";
                    }
                    strCondition += dic.GetValue("Field") + " = " + "'" + dic.GetValue("Value") + "'";
                }

                string sql = "select count(*) from S_I_Engineering where " + strCondition;
                object objCount = SqlHelper.ExecuteScalar(sql);
                int count = 0;
                int.TryParse(objCount.ToString(), out count);
                string oldName = dics.GetValue("Name");
                oldName = Regex.Replace(oldName, @"\([^\(]*\)", "");
                nameList.Add(oldName + "(" + count + ")");
            }
            return Json(nameList);
        }

        private IEnumerable<Category> GetCats()
        {
            List<Category> categories = new List<Category>();
            //var deptCategory = CategoryFactory.GetCategory("Market.ManDept", "责任部门", "ChargerDept");
            //deptCategory.SetDefaultItem();
            //deptCategory.Multi = false;
            //tab.Categories.Add(deptCategory);            

            string pId1 = FormulaHelper.CreateGuid();
            var phaseCategory = CategoryFactory.GetCategory("Base.ProjectModeType", "承包方式", "ContractMode");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = false;

            string pId2 = FormulaHelper.CreateGuid();
            var projectClassCategory = CategoryFactory.GetCategory("Base.ProjectClass", "项目类型", "ProjectClass");
            projectClassCategory.SetDefaultItem();
            projectClassCategory.Multi = false;

            string pId3 = FormulaHelper.CreateGuid();
            var stateCategory = CategoryFactory.GetCategory(typeof(ProjectState), "项目状态", "State");
            stateCategory.SetDefaultItem();
            stateCategory.Multi = false;

            categories.Add(phaseCategory);
            categories.Add(projectClassCategory);
            categories.Add(stateCategory);

            return categories;
        }
    }
}
