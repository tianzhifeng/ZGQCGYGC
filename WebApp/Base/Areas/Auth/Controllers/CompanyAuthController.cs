using Base.Logic.Domain;
using Config;
using Formula;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Auth.Controllers
{
    public class CompanyAuthController : BaseController
    {
        public JsonResult GetOrgTree()
        {
            string sql = "select * from S_A_Org order by SortIndex";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            return Json(dt);
        }

        public JsonResult GetResTree(string CompanyID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = @"
                select  Checked=case when exists(select 1 from S_A_AuthCompany where ResID =a.ID and CompanyID='{2}' and Type='Res') then 1  else 0 end,
                a.ID,a.ParentID,a.Name,a.SortIndex,iconCls=case when parentid is null or parentid='' then '' else 'Menu' end
                from S_A_Res a  where FullID like '{0}%' and FullID not like '{1}%' order by ParentID,SortIndex";

            sql = string.Format(sql, Config.Constant.MenuRooID, Config.Constant.SystemMenuFullID, CompanyID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());

            return Json(dt);
        }

        public JsonResult GetRuleTree(string CompanyID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = @"
                select  Checked=case when exists(select 1 from S_A_AuthCompany where ResID =a.ID and CompanyID='{2}' and Type='Rule') then 1  else 0 end,
                a.ID,a.ParentID,a.Name,a.SortIndex,iconCls=case when parentid is null or parentid='' then '' else 'Rule' end
                from S_A_Res a  where FullID like '{0}%' and FullID not like '{1}%' order by ParentID,SortIndex";

            sql = string.Format(sql, Config.Constant.RuleRootID, Config.Constant.SystemMenuFullID, CompanyID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            return Json(dt);
        }

        public JsonResult GetFormTree(string CompanyID)
        {
            string sql = string.Format(@"
select ID,ParentID,flag='0',Name,Code='',CompanyName='',Checked=0,iconCls='None' from S_M_Category where Name <>'系统平台' and Name <>'流程模块'
union
select ID, CategoryID as ParentID
,flag=case when CompanyID='{0}' then '1' when CompanyID is not null and CompanyID <>'' then '2' else '0' end
,Name,Code,CompanyName 
,Checked=case when CompanyID = '{0}' then 1 else 0 end
,iconCls='Form' from S_UI_Form"
, CompanyID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            DealTree(dt);
            return Json(dt);
        }

        public JsonResult GetListTree(string CompanyID)
        {
            string sql = string.Format(@"
select ID,ParentID,flag='0',Name,Code='',CompanyName='',Checked=0,iconCls='None' from S_M_Category where Name <>'系统平台'  and Name <>'流程模块'
union
select ID, CategoryID as ParentID
,flag=case when CompanyID='{0}' then '1' when CompanyID is not null and CompanyID <>'' then '2' else '0' end
,Name,Code,CompanyName
,Checked=case when CompanyID = '{0}' then 1 else 0 end
,iconCls='List'from S_UI_List"
, CompanyID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            DealTree(dt);
            return Json(dt);
        }

        public JsonResult GetWordTree(string CompanyID)
        {
            string sql = string.Format(@"
select ID,ParentID,flag='0',Name,Code='',CompanyName='',Checked=0,iconCls='None' from S_M_Category where Name <>'系统平台'  and Name <>'流程模块'
union
select ID, CategoryID as ParentID
,flag=case when CompanyID='{0}' then '1' when CompanyID is not null and CompanyID <>'' then '2' else '0' end
,Name,Code,CompanyName
,Checked=case when CompanyID = '{0}' then 1 else 0 end
,iconCls='Word'from S_UI_Word"
, CompanyID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            DealTree(dt);
            return Json(dt);
        }

        public JsonResult GetFlowTree(string CompanyID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper flowSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

            var dtCategory = sqlHelper.ExecuteDataTable("select ID,ParentID,flag='0',Name,Code='',CompanyName='',Checked=0,iconCls='None' from S_M_Category where Name <>'系统平台' and Name <>'流程模块' ", new SearchCondition());


            string sql = string.Format(@"
select ID, CategoryID as ParentID
,flag=case when CompanyID='{0}' then '1' when CompanyID is not null and CompanyID <>'' then '2' else '0' end
,Name,Code,CompanyName
,Checked=case when CompanyID = '{0}' then 1 else 0 end
,iconCls='Flow' from S_WF_DefFlow
"
, CompanyID);
            var dt = flowSqlHelper.ExecuteDataTable(sql, new SearchCondition());

            foreach (DataRow row in dt.Rows)
            {
                var newRow = dtCategory.NewRow();
                dtCategory.Rows.Add(newRow);
                newRow["ID"] = row["ID"];
                newRow["ParentID"] = row["ParentID"];
                newRow["Name"] = row["Name"];
                newRow["Checked"] = row["Checked"];
                newRow["iconCls"] = row["iconCls"];
                newRow["flag"] = row["flag"];
                newRow["Code"] = row["Code"];
                newRow["CompanyName"] = row["CompanyName"];
            }
            DealTree(dtCategory);
            return Json(dtCategory);
        }

        public JsonResult GetEnumTree(string CompanyID)
        {
            string sql = string.Format(@"
select ID,ParentID,flag='0',Name,Code='',CompanyName='',Checked=0,iconCls='None' from S_M_Category where Name <>'系统平台'  and Name <>'流程模块'
union
select ID, CategoryID as ParentID
,flag=case when CompanyID='{0}' then '1' when CompanyID is not null and CompanyID <>'' then '2' else '0' end
,Name,Code,CompanyName
,Checked=case when CompanyID = '{0}' then 1 else 0 end
,iconCls='Enum' from S_M_EnumDef"
, CompanyID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            DealTree(dt);
            return Json(dt);
        }

        public JsonResult GetRoleTree(string CompanyID)
        {
            string sql = string.Format(@"
select ID='root1',ParentID='',flag=0,Name='系统角色',Code='',CompanyName='',Checked=0,iconCls='None' union
select ID='root2',ParentID='',flag=0,Name='组织角色',Code='',CompanyName='',Checked=0,iconCls='None' union
select ID, ParentID=case when Type='OrgRole' then 'root2' else 'root1' end
,flag=case when CorpID='{0}' then '1' when CorpID is not null and CorpID <>'' then '2' else '0' end
,Name,Code,CompanyName = CorpName
,Checked=case when CorpID = '{0}' then 1 else 0 end
,iconCls='Role' from S_A_Role"
, CompanyID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            return Json(dt);
        }

        public JsonResult GetUserGrid(string CompanyID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_A_User where AdminCompanyID='{0}'", CompanyID), new SearchCondition());
            return Json(dt);
        }

        public JsonResult SaveRes(string CompanyID, string selectedIDs)
        {
            entities.Set<S_A_AuthCompany>().Delete(c => c.CompanyID == CompanyID && c.Type == "Res");
            var arr = selectedIDs.Split(',');
            foreach (var id in arr)
            {
                var obj = new S_A_AuthCompany();
                obj.ID = FormulaHelper.CreateGuid();
                obj.CompanyID = CompanyID;
                obj.ResID = id;
                obj.Type = "Res";
                entities.Set<S_A_AuthCompany>().Add(obj);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRule(string CompanyID, string selectedIDs)
        {
            entities.Set<S_A_AuthCompany>().Delete(c => c.CompanyID == CompanyID && c.Type == "Rule");
            var arr = selectedIDs.Split(',');
            foreach (var id in arr)
            {
                var obj = new S_A_AuthCompany();
                obj.ID = FormulaHelper.CreateGuid();
                obj.CompanyID = CompanyID;
                obj.ResID = id;
                obj.Type = "Rule";
                entities.Set<S_A_AuthCompany>().Add(obj);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveForm(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("Update S_UI_Form set CompanyID='',CompanyName='' where CompanyID='{0}';update S_UI_Form set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveList(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("Update S_UI_List set CompanyID='',CompanyName='' where CompanyID='{0}';update S_UI_List set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveWord(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("Update S_UI_Word set CompanyID='',CompanyName='' where CompanyID='{0}';update S_UI_Word set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveFlow(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string sql = string.Format("Update S_WF_DefFlow set CompanyID='',CompanyName='' where CompanyID='{0}';update S_WF_DefFlow set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveEnum(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("Update S_M_EnumDef set CompanyID='',CompanyName='' where CompanyID='{0}';update S_M_EnumDef set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveRole(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("Update S_A_Role set CorpID='',CorpName='' where CorpID='{0}';update S_A_Role set CorpID='{0}',CorpName='{1}' where ID in('{2}')", CompanyID, CompanyName, selectedIDs.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveUser(string CompanyID, string CompanyName, string selectedIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format(@"
update S_A_User set AdminCompanyID='',AdminCompanyName='' where AdminCompanyID='{0}'
update S_A_User set AdminCompanyID='{0}',AdminCompanyName='{2}' where ID in('{1}')", CompanyID, selectedIDs.Replace(",", "','"), CompanyName);
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        private void DealTree(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var row = dt.Rows[i];
                if (row["iconCls"].ToString() != "None")
                    continue;
                var id = row["ID"].ToString();
                if (dt.AsEnumerable().Count(c => c["ParentID"].ToString() == id) == 0)
                    dt.Rows.Remove(row);
            }
        }
    }
}
