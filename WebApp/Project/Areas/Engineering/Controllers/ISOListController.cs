using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Project.Logic.Domain;
using Formula;
using Config.Logic;
using System.Data;
using Formula.Helper;
using Project.Logic;

namespace Project.Areas.Engineering.Controllers
{
    public class ISOListController : ProjectController<S_I_ProjectInfo>
    {
        public ActionResult ISOList()
        {
            return View();
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            var groupInfoID = this.GetQueryString("GroupInfoID");
            var group = this.GetEntityByID<S_I_ProjectGroup>(groupInfoID);
            if (group == null)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            }
            var groupTreeList = new List<GroupTree>();
            groupTreeList.Add(GetTreeItem(group.ID, group.Name, "", "", EngineeringSpaceType.Engineering.ToString()));

            var projectList = group.ProjectInfoList;
            foreach (var projectInfo in projectList)
            {
                groupTreeList.Add(GetTreeItem(projectInfo.ID, projectInfo.Name, "", group.ID, EngineeringSpaceType.Project.ToString()));

                if (projectInfo.ProjectMode == null) throw new Formula.Exceptions.BusinessException("没有设定项目的管理模式，无法显示内容");
                var mode = projectInfo.ProjectMode;
                var list = mode.S_T_ISODefine.OrderBy(d => d.SortIndex).ToList();
                foreach (var isoDef in list)
                {
                    var sql = "select count(0) from {0} where ProjectInfoID='{1}'";
                    var count = this.SqlHelper.ExecuteScalar(String.Format(sql, isoDef.TableName, projectInfo.ID));
                    groupTreeList.Add(GetTreeItem(isoDef.ID, isoDef.Name + "(" + count.ToString() + ")", isoDef.Code, projectInfo.ID, "ISO", isoDef.FormCode));
                }
            }
            //var projectList = entities.Set<S_I_ProjectGroup>().Where(d => d.RootID == group.ID).ToList();
            //foreach (var item in projectList)
            return Json(groupTreeList);
        }

        public JsonResult LoadFolders(string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            if (projectInfo.ProjectMode == null) throw new Formula.Exceptions.BusinessException("没有设定项目的管理模式，无法显示内容");
            var mode = projectInfo.ProjectMode;
            var list = mode.S_T_ISODefine.OrderBy(d => d.SortIndex).ToList();
            return Json(list);
        }

        public JsonResult GetFiles(string ISODefineID, string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            if (projectInfo.ProjectMode == null) throw new Formula.Exceptions.BusinessException("没有设定项目的管理模式，无法显示内容");
            var mode = projectInfo.ProjectMode;
            var isoDefine = mode.S_T_ISODefine.FirstOrDefault(d => d.ID == ISODefineID);
            string sql = "select * from {0} where ProjectInfoID='{1}' ";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, isoDefine.TableName, ProjectInfoID));
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var item = FormulaHelper.DataRowToDic(row);
                var enumDefList = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(isoDefine.EnumFieldInfo))
                    enumDefList = JsonHelper.ToList(isoDefine.EnumFieldInfo);
                string name = Function.ReplaceRegString(isoDefine.NameFieldInfo, FormulaHelper.DataRowToDic(row), enumDefList);
                item.SetValue("Name", name);

                if (!String.IsNullOrWhiteSpace(isoDefine.LinkFormUrl))
                {
                    var url = "";
                    if (isoDefine.LinkFormUrl.IndexOf("?") >= 0)
                    {
                        url = isoDefine.LinkFormUrl + "&ID=" + row["ID"].ToString() + "";
                    }
                    else
                    {
                        url = isoDefine.LinkFormUrl + "?ID=" + row["ID"].ToString() + "";
                    }
                    if (isoDefine.CanAddNewForm != "True")
                    {
                        #region 如果不允许在ISO表单中心编辑，则要将FUNCTYPE改成VIEW并且将FLOWCODE去掉以免可以在这里操作流程
                        url += "&FuncType=View";
                        var urlParams = url.Split('?')[1].Split('&');
                        foreach (var paramsString in urlParams)
                        {
                            if (paramsString.Split('=')[0].ToLowerInvariant() == "flowcode")
                            {
                                url = url.Replace(paramsString, "");
                            }
                        }
                        #endregion
                    }

                    item.SetValue("LinkUrl", url);
                }
                list.Add(item);
            }
            var result = new Dictionary<String, object>();
            result.SetValue("data", list.OrderBy(d => d["Name"]).ToList());
            result.SetValue("CanAddNewForm", isoDefine.CanAddNewForm);
            result.SetValue("AddUrl", isoDefine.LinkFormUrl);
            return Json(result);
        }

        public JsonResult Delete(string ListIDs, string FolderID)
        {
            var configEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var define = configEntities.Set<S_T_ISODefine>().Find(FolderID);
            if (define == null) { throw new Formula.Exceptions.BusinessException("没有找ISO表单定义对象，无法删除"); }
            string sql = "select * from {0} where ID='{1}'";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, define.TableName, ListIDs));
            if (dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("FlowPhase") && dt.Rows[0]["FlowPhase"].ToString() != "Start")
                {
                    throw new Formula.Exceptions.BusinessException("已经启动流程审批的记录不能删除");
                }
                this.SqlHelper.ExecuteNonQuery(String.Format("delete from {0} where ID='{1}'", define.TableName, ListIDs));
            }
            return Json("");
        }

        private GroupTree GetTreeItem(string id, string name, string code, string parentID, string type, string formCode = "")
        {
            var groupLevel = new GroupTree();
            groupLevel.ID = id;
            groupLevel.Name = name;
            groupLevel.Code = code;
            groupLevel.ParentID = parentID;
            groupLevel.Type = type;
            groupLevel.FormCode = formCode;
            return groupLevel;
        }

        public class GroupTree
        {
            public string ID { set; get; }
            public string Name { set; get; }
            public string Code { set; get; }
            public string ParentID { set; get; }
            public string Type { set; get; }
            public string FormCode { set; get; }
        }
    }
}
