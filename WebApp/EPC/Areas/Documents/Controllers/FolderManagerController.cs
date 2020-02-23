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
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Documents.Controllers
{
    public class FolderManagerController : EPCController<S_D_Folder>
    {
        public ActionResult Folder()
        {
            var level = String.IsNullOrEmpty(this.GetQueryString("Level").Trim()) ? "true" : this.GetQueryString("Level");
            ViewBag.DisplayLevel = level;
            return View();
        }

        public ActionResult FolderSelector()
        {
            var level = String.IsNullOrEmpty(this.GetQueryString("Level").Trim()) ? "true" : this.GetQueryString("Level");
            ViewBag.DisplayLevel = level;
            return View();
        }

        public override JsonResult GetTree()
        {
            string EngineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var data = this.entities.Set<S_D_Folder>().Where(d => d.EngineeringInfoID == EngineeringInfoID).OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult SaveChildFolder(string ListData, string ParentID)
        {
            var parentNode = this.GetEntityByID<S_D_Folder>(ParentID);
            if (parentNode == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法保存子目录");
            var list = JsonHelper.ToList(ListData);
            var result = new List<S_D_Folder>();
            foreach (var item in list)
            {
                var folder = this.GetEntityByID<S_D_Folder>(item.GetValue("ID"));
                if (folder == null)
                {
                    folder = this.entities.Set<S_D_Folder>().Create();
                    this.UpdateEntity<S_D_Folder>(folder, item);
                    parentNode.AddChildFolder(folder);
                }
                else
                {
                    this.UpdateEntity<S_D_Folder>(folder, item);
                }
                folder.ModifyDate = DateTime.Now;
                folder.ModifyUser = this.CurrentUserInfo.UserName;
                folder.ModifyUserID = this.CurrentUserInfo.UserID;
                result.Add(folder);
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult DeleteFolders(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var folder = this.GetEntityByID<S_D_Folder>(item.GetValue("ID"));
                if (folder == null) continue;
                folder.Delete();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetSubFolderList(QueryBuilder qb, string ParentID)
        {
            qb.PageSize = 0;
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            var data = this.entities.Set<S_D_Folder>().Where(d => d.ParentID == ParentID).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetFolderAuthList(string FolderID)
        {
            var folder = this.GetEntityByID<S_D_Folder>(FolderID);
            if (folder == null) return Json("");
            return Json(folder.FolderAuth.ToList());
        }

        public JsonResult IntertParentAuth(string FolderID)
        {
            var folder = this.GetEntityByID<S_D_Folder>(FolderID);
            if (folder == null) return Json("");
            folder.InhertParentAuth();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ChangeAuth(string FolderID)
        {
            var folder = this.GetEntityByID<S_D_Folder>(FolderID);
            if (folder == null) return Json("");
            folder.InhertAuth = false.ToString();
            if (folder.Parent != null)
            {
                var parentAuth = folder.Parent.S_D_Folder_Auth.ToList();
                foreach (var item in parentAuth)
                {
                    if (!folder.S_D_Folder_Auth.Any(a => a.RelateID == item.RelateID))
                    {
                        var auth = item.Clone<S_D_Folder_Auth>();
                        auth.ID = Formula.FormulaHelper.CreateGuid();
                        auth.FolderID = folder.ID;
                        folder.S_D_Folder_Auth.Add(auth);
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddRoleAuth(string ListData, string FolderID)
        {
            var folder = this.GetEntityByID<S_D_Folder>(FolderID);
            if (folder == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法设置权限"); }
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var roleCode = item.GetValue("RoleCode");
                var roleType = item.GetValue("RoleType");
                var role = folder.S_D_Folder_Auth.FirstOrDefault(d => d.RelateCode == roleCode && d.AuthRelateType == roleType);
                if (role == null)
                {
                    role = this.entities.Set<S_D_Folder_Auth>().Create();
                    role.ID = Formula.FormulaHelper.CreateGuid();
                    role.RelateCode = item.GetValue("RoleCode");
                    role.RelateName = item.GetValue("RoleName");
                    role.RelateID = item.GetValue("RoleCode");
                    role.AuthRelateType = roleType;
                    role.BrowseAuth = false.ToString();
                    role.DownLoadAuth = false.ToString();
                    role.WriteAuth = false.ToString();
                    folder.S_D_Folder_Auth.Add(role);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddUserAuth(string ListData, string FolderID)
        {
            var folder = this.GetEntityByID<S_D_Folder>(FolderID);
            if (folder == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法设置权限"); }
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var userID = item.GetValue("ID");
                var userCode = item.GetValue("WorkNo");
                var relateType = "User";
                var user = folder.S_D_Folder_Auth.FirstOrDefault(d => d.RelateID == userID && d.AuthRelateType == relateType);
                if (user == null)
                {
                    user = this.entities.Set<S_D_Folder_Auth>().Create();
                    user.ID = Formula.FormulaHelper.CreateGuid();
                    user.RelateCode = item.GetValue("WorkNo");
                    user.RelateName = item.GetValue("Name");
                    user.RelateID = item.GetValue("ID");
                    user.AuthRelateType = relateType;
                    user.BrowseAuth = false.ToString();
                    user.DownLoadAuth = false.ToString();
                    user.WriteAuth = false.ToString();
                    folder.S_D_Folder_Auth.Add(user);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveAuth(string ID)
        {
            this.entities.Set<S_D_Folder_Auth>().Delete(d => d.ID == ID);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveAuth(string ListData)
        {
            this.UpdateList<S_D_Folder_Auth>(ListData);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var dragNode = this.GetEntityByID<S_D_Folder>(sourceID);
            if (dragNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目录，无法移动");
            var targetNode = this.GetEntityByID<S_D_Folder>(targetID);
            if (targetNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标目录，无法移动");
            if (targetNode == null || dragNode == null) return Json("");
            if (dragAction.ToLower() == "add")
            {
                dragNode.ParentID = targetNode.ID;
                dragNode.FullID = targetNode.FullID + "." + dragNode.ID;
                if (targetNode.Children.Count == 0)
                    dragNode.SortIndex = 0;
                else
                {
                    var maxSortIndex = targetNode.Children.Max(c => c.SortIndex);
                    dragNode.SortIndex = maxSortIndex.HasValue ? 0 : maxSortIndex + 0.001;
                }
            }
            else if (dragAction.ToLower() == "after")
            {
                this.entities.Set<S_D_Folder>().Where(c => c.ParentID == targetNode.ParentID
                    && c.SortIndex > targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex + 0.001);
                dragNode.SortIndex = targetNode.SortIndex + 0.001;
            }
            else if (dragAction.ToLower() == "before")
            {
                this.entities.Set<S_D_Folder>().Where(c => c.ParentID == targetNode.ParentID && c.SortIndex < targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex - 0.001);
                dragNode.SortIndex = targetNode.SortIndex - 0.001;
            }
            this.entities.SaveChanges();
            return Json(dragNode);
        }

        public JsonResult SychorFromTemplate(string EngineeringInfoID, string IncludeAuth = "false")
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息，无法同步");
            var auth = IncludeAuth == true.ToString().ToLower() ? true : false;
            engineeringInfo.SychorFolderTemplate(auth);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetTable()
        {
            var sh = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var rtn = sh.ExecuteDataTable("SELECT Name AS [text] ,name AS [value] FROM SysObjects Where XType='U' ORDER BY Name");
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }
    }
}
