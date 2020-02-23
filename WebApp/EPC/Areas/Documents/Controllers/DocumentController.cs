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
using Base.Logic.Domain;
using System.ComponentModel;
using Base.Logic.BusinessFacade;

namespace EPC.Areas.Documents.Controllers
{
    public class DocumentController : EPCFormContorllor<S_D_Document>
    {
        public ActionResult List()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var folderKey = this.GetQueryString("FolderKey");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfo.ID;
            var folder = engineeringInfo.S_D_Folder.FirstOrDefault(c => c.FolderKey == folderKey);
            ViewBag.HasAttr = false;
            if (folder == null)
            {
                folder = engineeringInfo.S_D_Folder.FirstOrDefault(c => c.Code == folderKey);
                if (folder == null) return View();
            }

            ViewBag.FolderID = folder.ID;
            string tmplCode = "DocumentBaseInfo";
            var baseDbContext = FormulaHelper.GetEntities<BaseEntities>();
            var formInfo = baseDbContext.S_UI_Form.FirstOrDefault(c => c.Code == tmplCode);
            if (formInfo == null)
            {
                ViewBag.BaseHtml = "";
                ViewBag.BaseScrpitHtml = "";
            }
            else
            {
                //var uiFo = FormulaHelper.CreateFO<Base.Logic.BusinessFacade.UIFO>();
                //ViewBag.BaseHtml = uiFo.CreateFormHtml(tmplCode, null);
                //ViewBag.BaseScrpitHtml = uiFo.CreateFormScript(tmplCode);
                var uiFO = FormulaHelper.CreateFO<UIFO>();
                var formDef = UIFO.GetFormDef(tmplCode, Request["ID"]);
                ViewBag.BaseHtml = uiFO.CreateFormHtml(formDef);
                if (ViewBag.BaseScrpitHtml != null && !String.IsNullOrEmpty(ViewBag.BaseScrpitHtml))
                {
                    ViewBag.BaseScrpitHtml += "\n " + uiFO.CreateFormScript(formDef);
                }
                else
                {
                    ViewBag.BaseScrpitHtml = uiFO.CreateFormScript(formDef);
                }
            }

            if (!String.IsNullOrEmpty(folder.AttrDefine))
            {
                var attrList = JsonHelper.ToList(folder.AttrDefine);
                if (attrList.Count > 0)
                {
                    ViewBag.HasAttr = true;
                }
            }
            return View();
        }

        public ActionResult DocumentManager()
        {
            var baseDbContext = FormulaHelper.GetEntities<BaseEntities>();
            var level = String.IsNullOrEmpty(this.GetQueryString("Level").Trim()) ? "true" : this.GetQueryString("Level");
            ViewBag.DisplayLevel = level;
            string tmplCode = "DocumentBaseInfo";
            var formInfo = baseDbContext.S_UI_Form.FirstOrDefault(c => c.Code == tmplCode);
            if (formInfo == null)
            {
                ViewBag.BaseHtml = "";
                ViewBag.BaseScrpitHtml = "";
            }
            else
            {
                //var uiFo = FormulaHelper.CreateFO<Base.Logic.BusinessFacade.UIFO>();
                //ViewBag.BaseHtml = uiFo.CreateFormHtml(tmplCode, null);
                //ViewBag.BaseScrpitHtml = uiFo.CreateFormScript(tmplCode);
                var uiFO = FormulaHelper.CreateFO<UIFO>();
                var formDef = UIFO.GetFormDef(tmplCode, Request["ID"]);
                ViewBag.BaseHtml = uiFO.CreateFormHtml(formDef);
                if (ViewBag.BaseScrpitHtml != null && !String.IsNullOrEmpty(ViewBag.BaseScrpitHtml))
                {
                    ViewBag.BaseScrpitHtml += "\n " + uiFO.CreateFormScript(formDef);
                }
                else
                {
                    ViewBag.BaseScrpitHtml = uiFO.CreateFormScript(formDef);
                }
            }

            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo != null)
            {
                if(engineeringInfo.Mode == null)
                    throw new Formula.Exceptions.BusinessValidationException("未找到该项目的项目模式");

                var infrastructureDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
                var folderTemplate = infrastructureDbContext.Set<S_T_FolderTemplate>().FirstOrDefault(a => a.ModeKey.Contains(engineeringInfo.Mode.ID));
                if (folderTemplate == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的文档目录结构模板");

                ViewBag.QuickSearchName = "";
                ViewBag.QuickSearchCode = "";
                if (!string.IsNullOrEmpty(folderTemplate.DisplayColJson))
                {
                    var dicList = JsonHelper.ToList(folderTemplate.DisplayColJson);
                    ViewBag.ColDefine = dicList;
                    ViewBag.QuickSearchName = string.Join("或", dicList.Where(a => a.GetValue("IsQuickSearch") == "true")
                                                 .Select(a => a.GetValue("Name")));
                    ViewBag.QuickSearchCode = string.Join(",", dicList.Where(a => a.GetValue("IsQuickSearch") == "true")
                                                 .Select(a => a.GetValue("Code")));
                }
            }
            return View();
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string folderID = this.GetQueryString("FolderID");
            var folder = this.GetEntityByID<S_D_Folder>(folderID);
            if (folder == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目录信息，无法编辑文件"); }
            if (isNew)
            {
                dic.SetValue("FolderID", folder.ID);
                dic.SetValue("EngineeringInfoID", folder.EngineeringInfoID);
            }
            else
            {
                var ID = dic.GetValue("ID");
                var attrList = this.EPCEntites.Set<S_D_Document_Attr>().Where(c => c.DocumentID == ID).ToList();
                foreach (var item in attrList)
                {
                    dic.SetValue(item.AttrName, item.AttrValue);
                }
            }
            if (!String.IsNullOrEmpty(folder.AttrDefine))
            {
                ViewBag.HasAttrDefine = true;
                var attrDefine = JsonHelper.ToList(folder.AttrDefine);
                ViewBag.AttrHtml = folder.GetAttrHTML();
                ViewBag.AttrScript = folder.CreateScript();
            }
            else
            {
                ViewBag.HasAttrDefine = false;
                ViewBag.AttrHtml = "";
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            string folderID = dic.GetValue("FolderID");
            var ID = dic.GetValue("ID");
            var folder = this.GetEntityByID<S_D_Folder>(folderID);
            this.EPCEntites.Set<S_D_Document_Attr>().Delete(c => c.DocumentID == ID);
            if (!String.IsNullOrEmpty(folder.AttrDefine))
            {
                var attrDefines = JsonHelper.ToList(folder.AttrDefine);
                foreach (var item in attrDefines)
                {
                    var value = dic.GetValue(item.GetValue("Code"));
                    if (!String.IsNullOrEmpty(value))
                    {
                        var attr = new S_D_Document_Attr();
                        attr.DocumentID = dic.GetValue("ID");
                        attr.ID = FormulaHelper.CreateGuid();
                        attr.AttrName = item.GetValue("Code");
                        attr.AttrValue = value;
                        this.EPCEntites.Set<S_D_Document_Attr>().Add(attr);
                    }
                }
            }
            var CurrentVersion = String.IsNullOrEmpty(dic.GetValue("CurrentVersion")) ? 1 : Convert.ToDecimal(dic.GetValue("CurrentVersion"));
            var version = this.EPCEntites.Set<S_D_Document_Version>().FirstOrDefault(c => c.DocumentID == ID && c.VersionNo == CurrentVersion);
            if (version == null)
            {
                version = new S_D_Document_Version();
                version.ID = FormulaHelper.CreateGuid();
                version.DocumentID = dic.GetValue("ID");
                version.VersionName = dic.GetValue("Name");
                version.VersionNo = 1;
                version.CreateDate = DateTime.Now;
                version.CreateUser = this.CurrentUserInfo.UserName;
                version.CreateUserID = this.CurrentUserInfo.UserID;
                dic.SetValue("CurrentVersion", version.VersionNo.ToString());
                this.EPCEntites.Set<S_D_Document_Version>().Add(version);
            }
            version.MainFile = dic.GetValue("MainFile");
            version.Attachments = dic.GetValue("Attachments");
            if (!String.IsNullOrEmpty(version.MainFile))
            {
                var fileInfos = version.MainFile.Split('_');
                if (fileInfos.Length > 1)
                {
                    var filename = fileInfos[1];
                    var mainFileType = filename.Substring(filename.LastIndexOf('.'));
                    version.MainFileType = mainFileType.Replace(".", "");
                    dic.SetValue("MainFileType", version.MainFileType);
                }
            }
        }


        public JsonResult GetTree()
        {
            string EngineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Formula.Exceptions.BusinessException("没有找到指定的项目，无法展现目录结构");
            }
            string sql = @"select S_D_Folder.*,isnull(WriteAuth,'') as WriteAuth,
isnull(DownLoadAuth,'') as DownLoadAuth,
isnull(BrowseAuth,'') as BrowseAuth,len(FullID) as FullIDLen,FileCount from dbo.S_D_Folder 
left join (select count(0) as FileCount,FolderID from dbo.S_D_Document
where EngineeringInfoID='{0}'
group by FolderID) DocCountInfo
on S_D_Folder.ID = DocCountInfo.FolderID
left join (select Max(WriteAuth) as WriteAuth,Max(DownLoadAuth) as DownLoadAuth,Max(BrowseAuth) as BrowseAuth,
FolderID from S_D_Folder_Auth
where RelateID in ('{1}')
group by FolderID) AuthInfo on AuthInfo.FolderID=S_D_Folder.ID
where EngineeringInfoID='{0}'";

            var userService = FormulaHelper.GetService<IUserService>();
            var sysRoles = userService.GetRoleCodesForUser(this.CurrentUserInfo.UserID, string.Empty);
            var projectRole = engineeringInfo.GetUserOBSCode(this.CurrentUserInfo.UserID);
            var roleCodes = sysRoles.Trim().TrimEnd(',') + "," + projectRole.Trim();
            var dt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, engineeringInfo.ID, roleCodes.Replace(",", "','")));
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(row);
                if (row["InhertAuth"] != null && row["InhertAuth"] != DBNull.Value && row["InhertAuth"].ToString() == true.ToString())
                {
                    var parents = dt.Select(" '" + row["FullID"] + "' like FullID+'%'  and WriteAuth is not null and DownLoadAuth is not null and BrowseAuth is not null", " FullIDLen desc");
                    if (parents.Length > 0)
                    {
                        row["WriteAuth"] = parents[0]["WriteAuth"];
                        row["DownLoadAuth"] = parents[0]["DownLoadAuth"];
                        row["BrowseAuth"] = parents[0]["BrowseAuth"];
                    }
                    else
                    {
                        row["WriteAuth"] = "0";
                        row["DownLoadAuth"] = "0";
                        row["BrowseAuth"] = "0";
                    }
                }
                if (row["WriteAuth"].ToString() != "1"
                    && row["DownLoadAuth"].ToString() != "1"
                    && row["BrowseAuth"].ToString() != "1")
                {
                    continue;
                }
                result.Add(dic);
            }
            return Json(dt);
        }

        public JsonResult GetDocumentList(QueryBuilder qb, string FolderID)
        {
            var data = this.EPCEntites.Set<S_D_Document>().Where(c => c.FolderID == FolderID).WhereToGridData(qb);
            return Json(data);
        }
        
        public JsonResult DeleteDocument(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var doc = this.GetEntityByID<S_D_Document>(item.GetValue("ID"));
                if (doc == null) continue;
                if (doc.CreateUserID != this.CurrentUserInfo.UserID)
                {
                    throw new Formula.Exceptions.BusinessValidationException("文件【" + doc.Name + "】是别人上传的文件，您不能删除别人上传提交的文件");
                }
                this.EPCEntites.Set<S_D_Document>().Remove(doc);
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult GetVersionList(string DocumentID)
        {
            string sql = @"select * from (select S_D_Document_Version.ID,VersionName,VersionNo,
S_D_Document_Version.CreateUser,S_D_Document_Version.CreateDate,S_D_Document_Version.MainFile,
S_D_Document_Version.Attachments,
case when S_D_Document_Version.VersionNo = S_D_Document.CurrentVersion then '是'
else '' end as IsCurrentVersion,
substring(S_D_Document_Version.MainFile,
charindex('_',S_D_Document_Version.MainFile)+1,len(S_D_Document_Version.MainFile) - charindex('_',
S_D_Document_Version.MainFile)-charindex('_',Reverse(S_D_Document_Version.MainFile))) as MainFileName,
substring(S_D_Document_Version.Attachments,
charindex('_',S_D_Document_Version.Attachments)+1,len(S_D_Document_Version.Attachments) - charindex('_',
S_D_Document_Version.Attachments)-charindex('_',Reverse(S_D_Document_Version.Attachments))) as AttachmentsName
 from S_D_Document_Version
left join S_D_Document on DocumentID=S_D_Document.ID where DocumentID='{0}') tableInfo order by VersionNo desc";
            var data = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, DocumentID));
            return Json(data);
        }

        public JsonResult GetCusAttr(string ID)
        {
            var doc = this.GetEntityByID(ID);
            if (doc == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的文件");
            if (doc.S_D_Folder == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到指定的文件目录"); }
            var data = new Dictionary<string, object>();
            var html = doc.GetCusAttrViewHTML();
            data.SetValue("html", html);
            return Json(data);
        }

        public JsonResult RemoveVersion(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var versionID = item.GetValue("ID");
                var version = this.GetEntityByID<S_D_Document_Version>(versionID);
                if (version == null) continue;
                if (version.S_D_Document.CurrentVersion == version.VersionNo)
                {
                    throw new Formula.Exceptions.BusinessValidationException("不能删除当前最新版本");
                }
                this.EPCEntites.Set<S_D_Document_Version>().Remove(version);
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult SaveVersion()
        {
            string formData = Request["FormData"];
            var entity = this.UpdateEntity<S_D_Document_Version>();
            if (this.EPCEntites.Entry<S_D_Document_Version>(entity).State == EntityState.Added ||
              this.EPCEntites.Entry<S_D_Document_Version>(entity).State == EntityState.Detached)
            {
                var document = this.GetEntityByID<S_D_Document>(entity.DocumentID);
                document.CurrentVersion = entity.VersionNo;
                document.MainFile = entity.MainFile;
                document.Attachments = entity.Attachments;
            }
            this.EPCEntites.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult GetVersionModel(string ID, string DocumentID)
        {
            var document = this.GetEntityByID<S_D_Document>(DocumentID);
            var entity = this.GetEntity<S_D_Document_Version>(ID);
            var result = new Dictionary<string, object>();
            result = entity.ToDic();
            if (this.EPCEntites.Entry<S_D_Document_Version>(entity).State == EntityState.Added ||
                this.EPCEntites.Entry<S_D_Document_Version>(entity).State == EntityState.Detached)
            {
                var versionNo = 1m;
                if (document.S_D_Document_Version.Count > 0)
                {
                    versionNo = document.S_D_Document_Version.Max(c => c.VersionNo);
                }
                result.SetValue("MainFile", document.MainFile);
                result.SetValue("Attachments", document.Attachments);
                result.SetValue("VersionNo", Convert.ToDecimal(versionNo + 0.1m));
            }
            result.SetValue("Name", document.Name);
            return Json(result);
        }
    }
}
