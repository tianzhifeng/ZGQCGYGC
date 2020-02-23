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

namespace EPC.Areas.ExpenseControl.Controllers
{
    public class FinalReportController : EPCController
    {
        public ActionResult List()
        {
            string Code = this.GetQueryString("Code");
            var infrasEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = infrasEntities.S_T_CBSDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null)
                define = infrasEntities.S_T_CBSDefine.FirstOrDefault();
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("");
            var root = define.S_T_CBSNodeTemplate.FirstOrDefault(c => c.NodeType == "Root");
            var nodes = root.Children.Where(c => c.DefineType == "Static").OrderBy(c => c.SortIndex).ToList();
            ViewBag.Nodes = JsonHelper.ToJson(nodes);
            ViewBag.NodeIDs = String.Join(",", nodes.Select(c => c.ID).ToList());
            return View();
        }

        public JsonResult GetList(QueryBuilder qb, string DefinInfo, string Code)
        {
            string sql = @"select S_I_Engineering.*,
isnull(ContractValue,0) as ContractValue from S_I_Engineering
left join (select Sum(ContractRMBValue) as ContractValue,ProjectInfo from dbo.S_M_ContractInfo
where SignDate is not null
group by ProjectInfo) contractInfo
on S_I_Engineering.ID=contractInfo.ProjectInfo ";
            var engineerInfoDt = this.SqlHelper.ExecuteDataTable(sql, qb);

            var engineeringInfoIDs = (from d in engineerInfoDt.AsEnumerable()
                                      select d.Field<string>("ID")).ToList<string>();
            #region 设置查询的科目
            var defineList = new List<Dictionary<string, object>>();
            if (String.IsNullOrEmpty(DefinInfo))
            {
                var infrasEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
                var define = infrasEntities.S_T_CBSDefine.FirstOrDefault(c => c.Code == Code);
                if (define == null)
                    define = infrasEntities.S_T_CBSDefine.FirstOrDefault();
                if (define == null) throw new Formula.Exceptions.BusinessValidationException("");
                var root = define.S_T_CBSNodeTemplate.FirstOrDefault(c => c.NodeType == "Root");
                var nodes = root.Children.Where(c => c.DefineType == "Static").OrderBy(c => c.SortIndex).ToList();
                defineList = FormulaHelper.CollectionToListDic<S_T_CBSNodeTemplate>(nodes);
            }
            else
            {
                defineList = JsonHelper.ToList(DefinInfo);
            }
            #endregion

            var defineIds = new List<string>();
            foreach (var item in defineList)
            {
                defineIds.Add(item["ID"].ToString());
                engineerInfoDt.Columns.Add(item["ID"].ToString() + "_Budget", typeof(decimal));
                engineerInfoDt.Columns.Add(item["ID"].ToString() + "_Settle", typeof(decimal));
            }
            var cbsList = this.entities.Set<S_I_CBS>().Where(c => engineeringInfoIDs.Contains(c.EngineeringInfoID) && defineIds.Contains(c.CBSDefineID)).ToList();
            foreach (DataRow item in engineerInfoDt.Rows)
            {
                var id = item["ID"].ToString();
                var nodes = cbsList.Where(c => c.EngineeringInfoID == id).ToList();
                foreach (var define in defineList)
                {
                    item[define["ID"].ToString() + "_Budget"] = 0;
                    item[define["ID"].ToString() + "_Settle"] = 0;
                }
                foreach (var node in nodes)
                {
                    if (node.Budget.HasValue)
                        item[node.CBSDefineID + "_Budget"] = node.Budget ?? 0;
                    if (node.Settle.HasValue)
                        item[node.CBSDefineID + "_Settle"] = node.Settle ?? 0;
                }
            }
            var data = new GridData(engineerInfoDt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetCBSDefineTree(string Code)
        {
            var infrasEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = infrasEntities.S_T_CBSDefine.FirstOrDefault(c => c.Code == Code);
            if (define == null)
                define = infrasEntities.S_T_CBSDefine.FirstOrDefault();
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("");
            var nodes = define.S_T_CBSNodeTemplate.Where(c => c.DefineType == "Static" && c.NodeType != "Root").OrderBy(c => c.SortIndex).ToList();
            return Json(nodes);
        }
    }
}
