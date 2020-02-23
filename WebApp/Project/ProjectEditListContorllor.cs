using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Config;
using Formula;
using Project.Logic.Domain;
using Workflow.Logic;
using System.Web.Mvc;

namespace Project
{
    public class ProjectEditListContorllor : BaseAutoListController
    {
        SQLHelper sqlDB;
        public virtual SQLHelper ProjectSQLDB
        {
            get
            {
                if (sqlDB == null)
                    sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                return sqlDB;
            }
        }

        public override DbContext BusinessEntities
        {
            get
            {
                return FormulaHelper.GetEntities<ProjectEntities>();
            }
        }

        #region 扩展虚方法
        protected override void BeforeSave(List<Dictionary<string, string>> list, Base.Logic.Domain.S_UI_List listInfo)
        {
            base.BeforeSave(list, listInfo);
        }

        protected override void AfterSave(List<Dictionary<string, string>> list, List<Dictionary<string, string>> deleteList, Base.Logic.Domain.S_UI_List listInfo)
        {
            base.AfterSave(list, deleteList, listInfo);
        }

        protected override void BeforeSaveDelete(List<Dictionary<string, string>> list, Base.Logic.Domain.S_UI_List listInfo)
        {
            base.BeforeSaveDelete(list, listInfo);
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> detail, List<Dictionary<string, string>> list, Base.Logic.Domain.S_UI_List listInfo, bool isNew)
        {
            base.BeforeSaveDetail(detail, list, listInfo, isNew);
        }

        protected override void BeforeDelete(string[] Ids)
        {
            base.BeforeDelete(Ids);
        }
        #endregion

        public override ActionResult PageView(string tmplCode)
        {
            var rtn =  base.PageView(tmplCode);
            ViewBag.Layout = ViewBag.Layout.Replace("AutoList", "");
            return rtn;
        }
    }
}