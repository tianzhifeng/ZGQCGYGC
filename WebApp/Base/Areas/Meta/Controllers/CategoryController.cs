using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using System.Data;
using Formula.Exceptions;

namespace Base.Areas.Meta.Controllers
{
    public class CategoryController : BaseController<S_M_Category>
    {
        public override JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID,ParentID,FullID,Code,Name,CategoryCode,IconClass from S_M_Category where FullID like '{0}%'", "0"));

            if (Request["listType"] == "subCompany")
            {
                var companyId = Formula.FormulaHelper.GetUserInfo().AdminCompanyID;


                SQLHelper flowSqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                var dtFlow = flowSqlHelper.ExecuteDataTable(string.Format("select distinct CategoryID from S_WF_DefFlow where CompanyID='{0}'", companyId));
                var _dt = sqlHelper.ExecuteDataTable(string.Format(@"
select distinct CategoryID from S_UI_Form where CompanyID='{0}'
union 
select distinct CategoryID from S_UI_List where CompanyID='{0}'
union 
select distinct CategoryID from S_UI_Word where CompanyID='{0}'
union 
select distinct CategoryID from S_M_EnumDef where CompanyID='{0}'", companyId));
                var list = new List<string>();
                foreach (DataRow row in dtFlow.Rows)
                {
                    var categoryId = row["CategoryID"].ToString();
                    if (list.Contains(categoryId) == false)
                        list.Add(categoryId);
                }
                foreach (DataRow row in _dt.Rows)
                {
                    var categoryId = row["CategoryID"].ToString();
                    if (list.Contains(categoryId) == false)
                        list.Add(categoryId);
                }

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    var id = dt.Rows[i]["ID"].ToString();
                    if (dt.AsEnumerable().Count(c => c["ParentID"].ToString() == id) > 0)
                        continue;
                    if (list.Contains(id))
                        continue;
                    dt.Rows.RemoveAt(i);
                }
            }

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public override JsonResult DeleteNode()
        {
            string sql = @"
select count(ID) from (
select ID from S_M_Category where ParentID='{0}'
union
select ID from S_UI_Form where CategoryID='{0}'
union
select ID from S_UI_List where CategoryID='{0}'
union
select ID from S_UI_Word where CategoryID='{0}'
union
select ID from S_R_Define where CategoryID='{0}'
) as t
";
            string categoryID = Request["FullID"].Split('.').LastOrDefault();

            sql = string.Format(sql, categoryID);

            var baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var obj = baseSqlHelper.ExecuteScalar(sql);
            if (obj.ToString() != "0")
                throw new BusinessException("节点下内容不为空，不能删除");

            sql = string.Format("select count(ID) from S_WF_DefFlow where CategoryID='{0}'", categoryID);

            var workflowSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            obj = workflowSqlHelper.ExecuteScalar(sql);

            if (obj.ToString() != "0")
                throw new BusinessException("节点下内容不为空，不能删除");

            return base.DeleteNode();
        }
    }
}
