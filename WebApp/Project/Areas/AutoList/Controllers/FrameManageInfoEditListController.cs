using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Project.Logic.Domain;
using Config.Logic;
using Formula.Exceptions;
using Project.Logic;
using Formula.Helper;
using Config;

namespace Project.Areas.AutoList.Controllers
{
    public class FrameManageInfoEditListController : ProjectEditListContorllor
    {
        public override ActionResult PageView(string tmplCode)
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }
            return base.PageView(tmplCode);
        }
        protected override void AfterSave(List<Dictionary<string, string>> list, List<Dictionary<string, string>> deleteList, Base.Logic.Domain.S_UI_List listInfo)
        {
            foreach (var dic in list)
            {
                var manageInfoID = dic.GetValue("ID");
                var bcs = this.BusinessEntities.Set<S_F_BorderConfig>().Where(a => a.ManageInfoID == manageInfoID).ToList();
                bcs.ForEach(a => this.BusinessEntities.Set<S_F_BorderConfig>().Remove(a));
                var borderConfigList = JsonHelper.ToList(dic.GetValue("BorderConfig"));
                foreach (var borderConfig in borderConfigList)
                {
                    var bc = new S_F_BorderConfig();
                    bc.ID = FormulaHelper.CreateGuid();
                    bc.BorderType = dic.GetValue("BorderType");
                    bc.BorderSize = dic.GetValue("Size");
                    bc.ManageInfoID = manageInfoID;
                    this.BusinessEntities.Set<S_F_BorderConfig>().Add(bc);
                    var removeStr = "ID,BorderType,Size,ManageInfoID".Split(',');
                    borderConfig.RemoveWhere(a => removeStr.Contains(a.Key));

                    this.UpdateEntity<S_F_BorderConfig>(bc, borderConfig);
                }
            }
            foreach (var dic in deleteList)
            {
                var manageInfoID = dic.GetValue("ID");
                var bcs = this.BusinessEntities.Set<S_F_BorderConfig>().Where(a => a.ManageInfoID == manageInfoID).ToList();
                bcs.ForEach(a => this.BusinessEntities.Set<S_F_BorderConfig>().Remove(a));
            }
            this.BusinessEntities.SaveChanges();
        }

        public JsonResult GetFrameTypeEnum()
        {
            var sql = "select ID as value,FrameTypeName as text from S_F_FrameInfo";
            var data = this.ProjectSQLDB.ExecuteDataTable(sql);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBorderInfo(string ManageInfoID, string BorderType, string BorderSize)
        {
            var sql = "select * from S_F_BorderConfig where ManageInfoID='" + ManageInfoID + "'";
            var list = this.ProjectSQLDB.ExecuteDataTable(sql);
            if (list.Rows.Count == 0)
            {
                var categoryList = EnumBaseHelper.GetEnumDef("Project.BorderCategory").EnumItem;
                sql = "select * from S_F_BorderConfig where (ManageInfoID='' or ManageInfoID is null) and BorderType='" + BorderType + "'and BorderSize='" + BorderSize + "'";
                var defaultList = this.ProjectSQLDB.ExecuteDataTable(sql);
                foreach (var category in categoryList)
                {
                    var newRow = list.NewRow();
                    var rows = defaultList.Select("Category='" + category.Code + "'");
                    if (rows.Length > 0)
                    {
                        newRow.ItemArray = rows[0].ItemArray;
                    }
                    else
                    {
                        newRow["Category"] = category.Code;
                    }
                    list.Rows.Add(newRow);
                }
            }
            return Json(list);
        }

        public JsonResult SetDefaultBorder(string DefaultTemplateName)
        {
            var sql = @"update S_F_BorderConfig set CurrentDefault='0'
update S_F_BorderConfig set CurrentDefault='1' where DefaultTemplateName='" + DefaultTemplateName + "'";
            this.ProjectSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }
    }
}