
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Formula.Helper;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class EngineeringListController : ProjectController
    {
        public ActionResult List()
        {
            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("Market.ManDept", "主要承接部门", "MainDept");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var phaseCategory = CategoryFactory.GetCategory("Project.Phase", "当前阶段", "PhaseValue");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = false;
            tab.Categories.Add(phaseCategory);
            var RelativeArea = CategoryFactory.GetCategory("System.RelativeArea", "地域", "Location");
            RelativeArea.SetDefaultItem();
            RelativeArea.Multi = false;
            tab.Categories.Add(RelativeArea);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var currentCity = System.Configuration.ConfigurationManager.AppSettings["City"];
            var currentProvince = System.Configuration.ConfigurationManager.AppSettings["Province"];
            string sql = @"select 
isnull(ContractValue,0) as ContractValue,
isnull(SumReceiptValue,0) as SumReceiptValue,
isnull(CanReceiptValue,0) as CanReceiptValue,
case when isnull(ContractValue,0)=0 then 0 else isnull(SumReceiptValue,0)/ContractValue*100
end as ReceiptRatio,
case when Province='{0}' and City='{1}' then 'LocalCity'
when Province='{0}' and City!='{1}' then 'LocalProvnice'
when Province!='{0}' then 'OtherProvince'
when Country!='中国' then 'Foreign' else '' end as Location,
S_I_Engineering.*
 from S_I_Engineering
 left join ( select Sum(ContractRMBAmount) as ContractValue,
 sum(SumReceiptValue) as SumReceiptValue,
 EngineeringInfo  from S_C_ManageContract
 group by EngineeringInfo) EngineeringInfoTable
 on S_I_Engineering.ID = EngineeringInfoTable.EngineeringInfo
 left join (
select  Sum(isnull((ReceiptValue-FactReceiptValue),0))  as CanReceiptValue,EngineeringInfo from S_C_ManageContract_ReceiptObj
left join S_C_ManageContract on S_C_ManageContract_ReceiptObj.S_C_ManageContractID=S_C_ManageContract.ID
where MileStoneState='True'
group by EngineeringInfo) ReceiptInfoAmout
on ReceiptInfoAmout.EngineeringInfo=S_I_Engineering.ID
left join (
select count(0) as ProjectCount,EngineeringInfo from dbo.S_I_Project
group by EngineeringInfo) ProjectCountInfo
on ProjectCountInfo.EngineeringInfo=S_I_Engineering.ID
where ProjectCount>0
";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var data = db.ExecuteGridData(String.Format(sql, currentProvince, currentCity), qb);
            return Json(data);
        }

        public JsonResult GetProjectList(QueryBuilder qb, string EngineeringID)
        {
            var list = this.entities.Set<S_I_ProjectInfo>().Where(d => d.EngineeringInfoID == EngineeringID).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var projectInfo in list)
            {
                var item = FormulaHelper.ModelToDic<S_I_ProjectInfo>(projectInfo);
                if (this.entities.Set<S_E_Product>().Where(d => d.ProjectInfoID == projectInfo.ID).Count() > 0)
                {
                    var printCount = this.entities.Set<S_E_Product>().Where(d => d.ProjectInfoID == projectInfo.ID).Sum(d => d.PrintCount);
                    item.SetValue("PrintCount", printCount.HasValue ? printCount.Value : 0);
                }
                else
                {
                    item.SetValue("PrintCount", 0);
                }
                var majors = String.Join(",", projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString()).Select(d => d.Name).Distinct().ToList());
                item.SetValue("Major", majors);
                item.SetValue("StateName", EnumBaseHelper.GetEnumDescription(typeof(Project.Logic.ProjectCommoneState), projectInfo.State));
                result.Add(item);
            }
            return Json(result);
        }

        public JsonResult GetGroupInfoID(string RelateID)
        {
            var groupInfo = this.entities.Set<S_I_ProjectGroup>().FirstOrDefault(d => d.RelateID == RelateID);
            if (groupInfo == null) { throw new Formula.Exceptions.BusinessException("没有找到指定的工程内容，请联系管理员"); }
            return Json(groupInfo);
        }
    }
}
