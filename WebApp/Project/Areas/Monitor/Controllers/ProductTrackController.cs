using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;
using MvcAdapter;
using Config;
using Project.Logic;
using Formula.Helper;
using Formula;
using Base.Logic.BusinessFacade;

namespace Project.Areas.Monitor.Controllers
{
    public class ProductTrackController : ProjectController<S_E_Product>
    {

        public override ActionResult List()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = entities.Set<S_I_ProjectInfo>().Find(projectInfoID);

            var transform = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => d.CanTransform == true.ToString()
                 && d.Code != WBSNodeType.Work.ToString() && d.Code != WBSNodeType.Project.ToString()).
                 Select(c => new { value = c.Code, text = "按" + c.Name, index = 1 }).ToList();
            transform.Add(new { value = "Project", text = "默认", index = 0 });
            ViewBag.TransForm = JsonHelper.ToJson(transform.OrderBy(d => d.index).ToList());
            var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            ViewBag.DefaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }

        public ActionResult PageView()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = entities.Set<S_I_ProjectInfo>().Find(projectInfoID);
            var uiFO = FormulaHelper.CreateFO<UIFO>();
            ViewBag.ListHtml = uiFO.CreateListHtml("ProductTrack");
            ViewBag.Script = uiFO.CreateListScript("ProductTrack");
            ViewBag.FixedFields = string.Format("var FixedFields={0};", Newtonsoft.Json.JsonConvert.SerializeObject(uiFO.GetFixedWidthFields("ProductTrack")));
            var transform = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => d.CanTransform == true.ToString()
                 && d.Code != WBSNodeType.Work.ToString() && d.Code != WBSNodeType.Project.ToString()).
                 Select(c => new { value = c.Code, text = "按" + c.Name, index = 1 }).ToList();
            transform.Add(new { value = "Project", text = "默认", index = 0 });
            ViewBag.TransForm = JsonHelper.ToJson(transform.OrderBy(d => d.index).ToList());
            var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            ViewBag.DefaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.Add("State", QueryMethod.NotEqual, ProductState.Invalid.ToString());
            string projectInfoID = GetQueryString("ProjectInfoID");
            string wbsID = this.GetQueryString("WBSID");
            string majorValue = this.GetQueryString("MajorValue");
            if (String.IsNullOrEmpty(wbsID) && string.IsNullOrEmpty(majorValue)) return Json("");

            var sql = @"
select *,'Product' Type from S_E_Product
where ProjectInfoID='{0}' {1} and (ParentID is null or ParentID = '' or ParentID = 'SPILT')";

            string where = "";
            if (string.IsNullOrEmpty(majorValue))
                where = string.Format("and WBSFullID like '%{0}%'", wbsID);
            else
                where = string.Format("and MajorValue = '{0}'", majorValue);
            sql = string.Format(sql, projectInfoID, where);
            var dataTable = this.SqlHelper.ExecuteDataTable(sql, qb);
            var list = FormulaHelper.DataTableToListDic(dataTable);

            var subSql = @"
select *,'SubProduct' Type from S_E_Product
where ProjectInfoID = '{0}' {1} and ParentID = '{2}' 
and ParentVersion = (select Version from S_E_Product where ID = '{2}') ";
            var allsubsql = new List<string>();
            foreach (var item in list)
            {
                allsubsql.Add(string.Format(subSql, projectInfoID, where, item["ID"].ToString()));
            }
            if (allsubsql.Count > 0)
            {
                var subDataTable = this.SqlHelper.ExecuteDataTable(string.Join("union all", allsubsql));
                var subList = FormulaHelper.DataTableToListDic(subDataTable);
                list.AddRange(subList);
            }

            var result = new Dictionary<string, object>();
            result["data"] = list;
            result["total"] = qb.TotolCount;
            return Json(result);
        }

        public override JsonResult GetTree()
        {
            var fo = FormulaHelper.CreateFO<ProjectInfoFO>();
            string projectInfoID = this.Request["ProjectInfoID"];
            string viewType = String.IsNullOrEmpty(this.Request["ViewType"]) ? WBSNodeType.Project.ToString() : this.Request["ViewType"];
            if (String.IsNullOrEmpty(projectInfoID)) throw new Formula.Exceptions.BusinessException("参数ProjectInfoID不能为空");
            var result = fo.GetWBSTree(projectInfoID, viewType, true, "", false);
            return Json(result);
        }

        #region 按DBS查看

        public ActionResult DBSViewList()
        {
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }

        public JsonResult GetDBSTree()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var projectFo = FormulaHelper.CreateFO<ProjectInfoFO>();
            string isViewAuth = GetQueryString("IsViewAuth");
            var result = projectFo.GetDBSTree(projectInfoID, this.CurrentUserInfo.UserID, isViewAuth == "true" ? true : false, true);
            return Json(result);
        }

        public JsonResult GetDocumentList(QueryBuilder qb)
        {
            string dbsID = this.Request["DBSID"];
            string ShowAll = String.IsNullOrEmpty(this.Request["ShowAll"]) ? "false" : this.Request["ShowAll"];
            if (String.IsNullOrEmpty(dbsID)) return Json("");
            var query = this.entities.Set<S_D_Document>().Where(d => d.DBSID == dbsID);
            if (ShowAll == true.ToString().ToLower())
            {
                var dbs = this.GetEntityByID<S_D_DBS>(dbsID);
                if (dbs != null)
                {
                    query = this.entities.Set<S_D_Document>().Where(d => d.DBSFullID.StartsWith(dbs.FullID));
                }
            }
            qb.Add("State", QueryMethod.NotEqual, ProductState.Invalid.ToString());
            var data = query.WhereToGridData(qb);
            return Json(data);
        }

        #endregion
    }
}
