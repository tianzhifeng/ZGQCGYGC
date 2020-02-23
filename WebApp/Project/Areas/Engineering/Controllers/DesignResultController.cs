using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;
using Formula;
using Project.Logic;
using MvcAdapter;
using Config;

namespace Project.Areas.Engineering.Controllers
{
    public class DesignResultController : ProjectController<S_E_Product>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            string wbsID = this.GetQueryString("WBSID");
            string majorName = this.GetQueryString("MajorName");
            if (String.IsNullOrEmpty(wbsID) && string.IsNullOrEmpty(majorName)) return Json("");

            string where = "";

            if (string.IsNullOrEmpty(majorName))
                where = string.Format(" And (WBSFullID like '%{0}' or WBSFullID like '{0}%' or WBSFullID like '%{0}%') ", wbsID);
            else
                where = string.Format(" And MajorName = '{0}' ", majorName);

            string projectInfoID = GetQueryString("ProjectInfoID");
            string sql = string.Format(@"SElECT * from dbo.S_E_Product where  ProjectInfoID='{0}' {1}", projectInfoID, where);

            SQLHelper shProject = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            GridData data = shProject.ExecuteGridData(sql, qb);

            return Json(data);
        }

        public override JsonResult GetTree()
        {
            string groupInfoID = GetQueryString("GroupInfoID");
            List<Dictionary<string, object>> resList = new List<Dictionary<string, object>>();
            var fo = FormulaHelper.CreateFO<ProjectInfoFO>();
            var prjList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.GroupRootID == groupInfoID).ToList();
            if (prjList.Count != 0)
                foreach (var projectInfo in prjList)
                {
                    var result = fo.GetWBSTree(projectInfo.ID, WBSNodeType.Project.ToString(), true, "", false);
                    resList.AddRange(result);
                }
            return Json(resList);
        }

    }
}
