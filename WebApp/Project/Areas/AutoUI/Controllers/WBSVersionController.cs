using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using System.Web.Routing;
using Config.Logic;
using System.Text;

namespace Project.Areas.AutoUI.Controllers
{
    public class WBSVersionController : ProjectFormContorllor<S_W_WBS_Version>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (!dt.Columns.Contains("ProjectInfoID") || dt.Rows[0]["ProjectInfoID"] == null || dt.Rows[0]["ProjectInfoID"] == DBNull.Value)
            {
                throw new Formula.Exceptions.BusinessValidationException("数据中必须包含ProjectInfoID列，请联系管理员");
            }
            string projectInfoID = dt.Rows[0]["ProjectInfoID"].ToString();
            if (dt.Columns.Contains("VersionNumber") && dt.Rows[0]["VersionNumber"] != DBNull.Value)
                ViewBag.VersionNo = dt.Rows[0]["VersionNumber"].ToString();
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到ID为【" + projectInfoID + "】的项目信息，请联系管理员");

            var sql = String.Format(@"select * from S_D_RoleDefine
where RoleCode in (select distinct RoleCode from dbo.S_T_WBSStructRole with(nolock)
where WBSStructID in (select ID from dbo.S_T_WBSStructInfo with(nolock)
where ModeID=(select top 1 ID from S_T_ProjectMode with(nolock)
where ModeCode='{0}') and SychWBS!='True'))
order by SortIndex", projectInfo.ModeCode);
            var roleColumns = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            ViewBag.RoleColumns = roleColumns;
            var sb = new StringBuilder();
            string returnParams = "value:ID,text:Name";
            foreach (DataRow item in roleColumns.Rows)
            {
                var field = item["RoleCode"] + "UserID";
                sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", field, returnParams);
            }
            ViewBag.SelectorScript = sb.ToString();

            var enumNodeType = new List<Dictionary<string, object>>();
            sql = @"select distinct [Level] from S_W_WBS_Version_Node where VersionID='" + dt.Rows[0]["ID"].ToString() + "' order by Level";
            var list = this.ProjectSQLDB.ExecuteDataTable(sql);
            for (int i = 0; i < list.Rows.Count; i++)
            {
                var item = list.Rows[i];
                if (i == list.Rows.Count - 1)
                {
                    ViewBag.ExpandLevel = i;
                    continue;
                }
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item["Level"]);
                dic.SetValue("text", "第" + item["Level"] + "层");
                dic.SetValue("sortindex", item["Level"]);
                enumNodeType.Add(dic);
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);

            var wbsCount = this.ProjectSQLDB.ExecuteScalar("select count(ID) from S_W_WBS_Version_Node WHERE VersionID='" + dt.Rows[0]["ID"].ToString() + "'");
            if (Convert.ToInt32(wbsCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScrollTree = "true";
            }

        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetVersionTreeList(string VersionID, string ShowType, string IncludeWorkTask)
        {
            var includeTask = true;
            if (!String.IsNullOrEmpty(IncludeWorkTask) && IncludeWorkTask.ToLower() == false.ToString().ToLower())
                includeTask = false;

            string sql = @"  select * from dbo.S_W_WBS_Version_Node with(nolock) where VersionID='{0}'   ";
            if (ShowType.ToLower() == "diff")
            {
                sql += " and ModifyState!='Normal'";
            }
            else if (ShowType.ToLower() == "new")
            {
                sql += " and ModifyState!='Remove'";
            }
            if (!includeTask)
            {
                sql += " and WBSType  not in ('" + WBSNodeType.Work.ToString() + "','" + WBSNodeType.CooperationPackage.ToString() + "')";
            }
            sql += " order by SortIndex";

            var dt = this.ProjectSQLDB.ExecuteDataTable(String.Format(sql, VersionID));
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(row);
                if (row["RBSInfo"] != null && row["RBSInfo"] != DBNull.Value && !String.IsNullOrEmpty(row["RBSInfo"].ToString()))
                {
                    var list = JsonHelper.ToList(row["RBSInfo"].ToString());
                    foreach (var item in list)
                    {
                        dic.SetValue(item.GetValue("RoleCode") + "UserID", item.GetValue("UserID"));
                        dic.SetValue(item.GetValue("RoleCode") + "UserName", item.GetValue("UserName"));
                    }
                }
                result.Add(dic);
            }
            return Json(result);
        }

        protected override void OnFlowEnd(S_W_WBS_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.BusinessEntities.SaveChanges();
            }
        }
    }
}
