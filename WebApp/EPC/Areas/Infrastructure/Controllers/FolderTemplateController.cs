using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using System.ComponentModel;
using Formula;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class FolderTemplateController : InfrastructureController<S_T_FolderTemplate>
    {
        public ActionResult MappingFieldConfig()
        {
            ViewBag.DocumentProperties = GetDocumentProperties();
            ViewBag.TableProperties = GetTableProperties(GetQueryString("TableName"));
            return View();
        }

        protected override void BeforeSave(S_T_FolderTemplate entity, bool isNew)
        {            
            if (!string.IsNullOrEmpty(entity.DisplayColJson))
            {
                var dicList = JsonHelper.ToList(entity.DisplayColJson);
                var firstOne = dicList.FirstOrDefault(a => a.GetValue("IsQuickSearch") == "true" && a.GetValue("IsAttr") == "true");
                if (firstOne != null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("编号为【" + firstOne.GetValue("Code") + "】的扩展字段不支持快查");
                }
            }            

            entity.Save();
        }

        protected override void BeforeDelete(List<S_T_FolderTemplate> entityList)
        {
            foreach (var item in entityList)
            {
                item.Delete();
            }
        }

        public JsonResult GetSubFolderList(QueryBuilder qb, string ParentID)
        {
            qb.PageSize = 0;
            var data = this.entities.Set<S_T_FolderDef>().Where(d => d.ParentID == ParentID).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetFolderAuthList(string FolderID)
        {
            var folder = this.GetEntityByID<S_T_FolderDef>(FolderID);
            if (folder == null) return Json("");
            return Json(folder.FolderAuth.ToList());
        }

        public override JsonResult GetTree()
        {
            string templateID = this.GetQueryString("TemplateID");
            var data = this.entities.Set<S_T_FolderDef>().Where(d => d.TempateID == templateID).ToList();
            return Json(data);
        }

        public JsonResult SaveChildFolder(string ListData, string ParentID)
        {
            var parentNode = this.GetEntityByID<S_T_FolderDef>(ParentID);
            if (parentNode == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法保存子目录");
            var list = JsonHelper.ToList(ListData);
            var result = new List<S_T_FolderDef>();
            foreach (var item in list)
            {
                var folder = this.GetEntityByID<S_T_FolderDef>(item.GetValue("ID"));
                if (folder == null)
                {
                    folder = this.entities.Set<S_T_FolderDef>().Create();
                    this.UpdateEntity<S_T_FolderDef>(folder, item);
                    parentNode.AddChildFolder(folder);
                }
                else
                {
                    this.UpdateEntity<S_T_FolderDef>(folder, item);
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
                var folder = this.GetEntityByID<S_T_FolderDef>(item.GetValue("ID"));
                if (folder == null) continue;
                folder.Delete();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult IntertParentAuth(string FolderID)
        {
            var folder = this.GetEntityByID<S_T_FolderDef>(FolderID);
            if (folder == null) return Json("");
            folder.InhertParentAuth();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ChangeAuth(string FolderID)
        {
            var folder = this.GetEntityByID<S_T_FolderDef>(FolderID);
            if (folder == null) return Json("");
            folder.InhertAuth = false.ToString();
            if (folder.Parent != null)
            {
                var parentAuth = folder.Parent.S_T_FolderAuth.ToList();
                foreach (var item in parentAuth)
                {
                    var auth = item.Clone<S_T_FolderAuth>();
                    auth.ID = Formula.FormulaHelper.CreateGuid();
                    auth.FolderDefID = folder.ID;
                    folder.S_T_FolderAuth.Add(auth);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddRoleAuth(string ListData, string FolderID)
        {
            var folder = this.GetEntityByID<S_T_FolderDef>(FolderID);
            if (folder == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法设置权限"); }
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var roleCode = item.GetValue("RoleCode");
                var roleType = item.GetValue("RoleType");
               var role = folder.S_T_FolderAuth.FirstOrDefault(d => d.RelateCode == roleCode && d.AuthRelateType == roleType);
               if (role == null)
               {
                   role = this.entities.Set<S_T_FolderAuth>().Create();
                   role.ID = Formula.FormulaHelper.CreateGuid();
                   role.RelateCode = item.GetValue("RoleCode");
                   role.RelateName = item.GetValue("RoleName");
                   role.RelateID = item.GetValue("RoleCode");
                   role.AuthRelateType = roleType;
                   role.BrowseAuth = false.ToString();
                   role.DownLoadAuth = false.ToString();
                   role.WriteAuth = false.ToString();
                   folder.S_T_FolderAuth.Add(role);
               }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddUserAuth(string ListData, string FolderID)
        {
            var folder = this.GetEntityByID<S_T_FolderDef>(FolderID);
            if (folder == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的目录，无法设置权限"); }
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var userID = item.GetValue("ID");
                var userCode = item.GetValue("WorkNo");
                var relateType = "User";
                var user = folder.S_T_FolderAuth.FirstOrDefault(d => d.RelateID == userID && d.AuthRelateType == relateType);
                if (user == null)
                {
                    user = this.entities.Set<S_T_FolderAuth>().Create();
                    user.ID = Formula.FormulaHelper.CreateGuid();
                    user.RelateCode = item.GetValue("WorkNo");
                    user.RelateName = item.GetValue("Name");
                    user.RelateID = item.GetValue("ID");
                    user.AuthRelateType = relateType;
                    user.BrowseAuth = false.ToString();
                    user.DownLoadAuth = false.ToString();
                    user.WriteAuth = false.ToString();
                    folder.S_T_FolderAuth.Add(user);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveAuth(string ID)
        {
            this.entities.Set<S_T_FolderAuth>().Delete(d => d.ID == ID);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveAuth(string ListData)
        {
            this.UpdateList<S_T_FolderAuth>(ListData);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetTable()
        {
            var sh = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var rtn = sh.ExecuteDataTable("SELECT Name AS [text] ,name AS [value] FROM SysObjects Where XType='U' ORDER BY Name");
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var dragNode = this.GetEntityByID<S_T_FolderDef>(sourceID);
            if (dragNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目录，无法移动");
            var targetNode = this.GetEntityByID<S_T_FolderDef>(targetID);
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
                    dragNode.SortIndex = maxSortIndex + 0.001;
                }
            }
            else if (dragAction.ToLower() == "after")
            {
                this.entities.Set<S_T_FolderDef>().Where(c => c.ParentID == targetNode.ParentID
                    && c.SortIndex > targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex + 0.001);
                dragNode.SortIndex = targetNode.SortIndex + 0.001;
            }
            else if (dragAction.ToLower() == "before")
            {
                this.entities.Set<S_T_FolderDef>().Where(c => c.ParentID == targetNode.ParentID && c.SortIndex < targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex - 0.001);
                dragNode.SortIndex = targetNode.SortIndex - 0.001;
            }
            this.entities.SaveChanges();
            return Json(dragNode);
        }

        public string GetDocumentProperties()
        {
            var results = new List<string>();

            //S_D_Document属性
            var properties = typeof(S_D_Document).GetProperties();
            foreach (var property in properties)
            {
                object[] arr = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                results.Add(property.Name);
            }
            return JsonHelper.ToJson(results.Select(a => new { text = a, value = a }));
        }

        public string GetTableProperties(string tableName)
        {
            var sh = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var rtn = sh.ExecuteDataTable("SELECT COLUMN_NAME as [text],COLUMN_NAME as [value] FROM INFORMATION_SCHEMA.columns WHERE TABLE_NAME='" + tableName + "'");
            return JsonHelper.ToJson(rtn);
        }

        public JsonResult GetPropertyList()
        {
            var results = new List<dynamic>();

            //S_D_Document属性
            var properties = typeof(S_D_Document).GetProperties();
            foreach (var property in properties)
            {
                object[] arr = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                string Code = property.Name;
                string Name = Code;
                if (arr.Length > 0 && !string.IsNullOrEmpty(((DescriptionAttribute)arr[0]).Description))
                {
                    Name = ((DescriptionAttribute)arr[0]).Description;
                }

                results.Add(new { Code, Name, IsAttr = "false", Width = 0, Align = "left", IsDisplay = "true" });
            }

            //S_T_FolderDef的AttrDefine
            string templateID = this.GetQueryString("ID");
            var datas = this.entities.Set<S_T_FolderDef>().Where(d => d.TempateID == templateID).ToList();
            foreach (var data in datas)
            {
                var dicStr = data.AttrDefine;
                if (!string.IsNullOrEmpty(dicStr))
                {
                    var dicList = JsonHelper.ToList(dicStr);
                    foreach (var dic in dicList)
                    {
                        if (dic.GetValue("Enable") == "True" && dic.GetValue("Visible") == "True")
                        {
                            results.Add(new { Code = dic.GetValue("Code"), Name = dic.GetValue("Name"), IsAttr = "true", Width = 0, Align = "left", IsDisplay = "true"});
                        }
                    }
                }
            }

            return Json(results);
        }
    }
}
