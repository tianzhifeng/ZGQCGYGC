using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;
using Base.Logic.BusinessFacade;
using Formula.ImportExport;
using DocSystem.Logic.Domain;
using System.Transactions;


namespace Project.Areas.Basic.Controllers
{
    public class DBSController : ProjectController<S_D_DBS>, IFormExport
    {
        public ViewResult Tab()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            ViewBag.ProjectInfoID = projectInfoID;
            return View();
        }

        public ActionResult DBSEdit()
        {
            var dbsTypeList = EnumBaseHelper.GetEnumDef(typeof(DBSType)).EnumItem.ToList();
            var dbsType = new List<Dictionary<string, object>>();
            foreach (var item in dbsTypeList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item.Code);
                dic.SetValue("text", item.Name);
                dbsType.Add(dic);
            }
            this.ViewBag.DBSType = JsonHelper.ToJson(dbsType);
            return this.View();
        }

        public override JsonResult GetTree()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var projectFo = FormulaHelper.CreateFO<ProjectInfoFO>();
            string isViewAuth = GetQueryString("IsViewAuth");
            var result = projectFo.GetDBSTree(projectInfoID, this.CurrentUserInfo.UserID, isViewAuth == "true" ? true : false, false, true);
            return Json(result);
        }

        public override JsonResult Delete()
        {
            string dbsID = this.Request["DBSID"];
            var dbs = this.GetEntityByID<S_D_DBS>(dbsID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + dbsID + "】的DBS目录，无法删除");
            //如果下层存在归档文件不能删除
            if (dbs.S_D_Document.Any(a => a.ArchiveDate != null))
                throw new Formula.Exceptions.BusinessException("目录下存在已经归档的文件，无法删除");
            dbs.Delete();
            this.entities.SaveChanges();
            return Json(dbs);
        }

        protected override void BeforeSave(S_D_DBS entity, bool isNew)
        {
            var parent = this.GetEntityByID<S_D_DBS>(entity.ParentID);
            if (isNew)
            {
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + entity.ParentID + "】的DBS目录，无法增加子目录");
                parent.AddChild(entity);
            }
            if (parent != null && !string.IsNullOrEmpty(parent.ArchiveFolder))
            {
                var docConfigEntities = FormulaHelper.GetEntities<DocConfigEntities>();
                var nodeConfig = docConfigEntities.Set<S_DOC_Node>().FirstOrDefault(a => a.ID == parent.ArchiveFolder);
                if (nodeConfig != null && nodeConfig.IsFreeNode == DocSystem.Logic.Domain.TrueOrFalse.True.ToString())
                {
                    entity.ArchiveFolder = parent.ArchiveFolder;
                    entity.ArchiveFolderName = parent.ArchiveFolderName;
                }
            }
        }

        protected override void AfterSave(S_D_DBS entity, bool isNew)
        {
            string dbsType = DBSType.Cloud.ToString();
            if (!isNew && entity.DBSType == dbsType)
            {
                var childrens = entity.AllChildren;
                if (childrens.Count != 0)
                    foreach (var children in childrens)
                        children.DBSType = dbsType;
                entities.SaveChanges();
            }
        }

        public override JsonResult GetList(QueryBuilder qb)
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

        public JsonResult GetAuthList()
        {
            string dbsID = this.GetQueryString("DBSID");
            string roleType = this.GetQueryString("RoleType");
            if (string.IsNullOrEmpty(roleType)) roleType = Project.Logic.RoleType.ProjectRoleType.ToString();

            string sql = @"select S_D_RoleDefine.RoleCode,S_D_RoleDefine.RoleName,
case when AuthType <>'{0}' or AuthType is null then 'False' else 'True' end as FullControlAuth,
case when AuthType <>'{1}' or AuthType is null then 'False' else 'True' end as WriteAuth,
case when AuthType <>'{2}' or AuthType is null then 'False' else 'True' end as ViewAuth
from " + SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).DbName + @"..S_D_RoleDefine left join (select * from S_D_DBSSecurity where DBSID='{3}' and RoleType='{4}') DBSSecurity
on DBSSecurity.RoleCode=S_D_RoleDefine.RoleCode";
            if (roleType != Project.Logic.RoleType.ProjectRoleType.ToString())
            {
                sql = @"select RoleTB.Code RoleCode,RoleTB.Name RoleName,
case when AuthType <>'{0}' or AuthType is null then 'False' else 'True' end as FullControlAuth,
case when AuthType <>'{1}' or AuthType is null then 'False' else 'True' end as WriteAuth,
case when AuthType <>'{2}' or AuthType is null then 'False' else 'True' end as ViewAuth
from (select * from " + SQLHelper.CreateSqlHelper(ConnEnum.Base.ToString()).DbName + "..S_A_Role where type='" + roleType.Replace("Type", "") + @"') RoleTB 
left join (select * from S_D_DBSSecurity where DBSID='{3}' and RoleType='{4}') DBSSecurity
on DBSSecurity.RoleCode=RoleTB.Code
";

            }
            sql = String.Format(sql, FolderAuthType.FullControl.ToString(), FolderAuthType.ReadAndWrite.ToString(), FolderAuthType.ReadOnly.ToString(), dbsID, roleType);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = sqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);

            //var dbs = this.GetEntityByID(dbsID);
            //var roleDefines = BaseConfigFO.GetRoleDefineList();
            //var securities = dbs.S_D_DBSSecurity.ToList();
            //var result = new List<Dictionary<string, object>>();
            //foreach (var item in roleDefines)
            //{
            //    var dic = new Dictionary<string, object>();
            //    dic.SetValue("RoleCode", item.RoleCode);
            //    dic.SetValue("RoleName", item.RoleName);
            //    dic.SetValue("FullControlAuth", false.ToString());
            //    dic.SetValue("WriteAuth", false.ToString());
            //    dic.SetValue("ViewAuth", false.ToString());
            //    result.Add(dic);
            //    var sec = securities.FirstOrDefault(d => d.RoleCode == item.RoleCode);
            //    if (sec == null)
            //        continue;
            //    else if (sec.AuthType == FolderAuthType.FullControl.ToString())
            //        dic.SetValue("FullControlAuth", true.ToString());
            //    else if (sec.AuthType == FolderAuthType.ReadAndWrite.ToString())
            //        dic.SetValue("WriteAuth", true.ToString());
            //    else if (sec.AuthType == FolderAuthType.ReadOnly.ToString())
            //        dic.SetValue("ViewAuth", true.ToString());
            //}
            //var data = new GridData(result);
            //return Json(data);
        }

        public JsonResult DeleteDocument(string Documents)
        {
            var list = JsonHelper.ToList(Documents);
            foreach (var item in list)
            {
                string documentID = item.GetValue("ID");
                this.entities.Set<S_D_Document>().Delete(d => d.ID == documentID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult InhertAuth(string DBSID)
        {
            var dbs = this.GetEntityByID(DBSID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + DBSID + "】的DBS对象，无法继承权限");
            dbs.InheritParentAuth();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetAuth(string RoleCode, string RoleName, string DBSID, string authType,string RoleType)
        {
            var dbs = this.GetEntityByID(DBSID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + DBSID + "】的DBS对象，无法设置权限");
            dbs.SetAuth(RoleCode, RoleName, authType, RoleType);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveAuth(string RoleCode, string RoleName, string DBSID)
        {
            var dbs = this.GetEntityByID(DBSID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + DBSID + "】的DBS对象，无法设置权限");
            dbs.RemoveAuth(RoleCode);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GatherDoc(string ProjectInfoID)
        {
            var mapType = DBSType.Mapping.ToString();
            var mappingFolders = this.entities.Set<S_D_DBS>().Where(d => d.ProjectInfoID == ProjectInfoID && d.DBSType == mapType
                && !String.IsNullOrEmpty(d.MappingType)).ToList();
            foreach (var folder in mappingFolders)
            {
                folder.GatherDocuments(this);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public string ExportPDF(string fileName, string formID, string tmpCode)
        {
            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dtWordTmpl = sqlHeper.ExecuteDataTable(string.Format("select * from S_UI_Word where Code='{0}'", tmpCode));
            if (dtWordTmpl.Rows.Count == 0)
                return "";
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            DataSet ds = uiFO.GetWordDataSource(tmpCode, formID);

            #region 获取word导出的版本
            DataRow wordTmplRow = dtWordTmpl.Rows[0];
            DateTime date;
            if (ds.Tables[0].Columns.Contains("CreateTime"))
                date = DateTime.Parse(ds.Tables[0].Rows[0]["CreateTime"].ToString());
            else
                date = DateTime.Parse(ds.Tables[0].Rows[0]["CreateDate"].ToString());
            foreach (DataRow row in dtWordTmpl.Rows)
            {
                var _startDate = DateTime.Parse(row["VersionStartDate"].ToString());
                var _endDate = DateTime.MaxValue;
                if (row["VersionEndDate"].ToString() != "")
                    _endDate = DateTime.Parse(row["VersionEndDate"].ToString());

                if (date > _startDate && date < _endDate)
                {
                    wordTmplRow = row;
                    break;
                }
            }
            int? versionNum = 1;
            if (wordTmplRow["VersionNum"].ToString() != "")
                versionNum = int.Parse(wordTmplRow["VersionNum"].ToString());
            string tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + "_" + versionNum + ".docx";
            string tempPath = Server.MapPath("/") + "WordTemplate/" + tmplName;

            if (System.IO.File.Exists(tempPath) == false)
            {
                tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + ".docx";
                tempPath = Server.MapPath("/") + "WordTemplate/" + tmplName;
            }
            #endregion

            var export = new Formula.ImportExport.AsposeWordExporter();
            byte[] result = export.ExportPDF(ds, tempPath);
            var name = fileName.IndexOf(".pdf") >= 0 ? fileName : fileName + ".pdf";
            return FileStoreHelper.UploadFile(name, result);
        }

        public JsonResult ProjectUnArchiveFiles(string projectInfoID, bool IsAll)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法归档");
            var documents = new List<S_D_Document>();
            if (IsAll)
                documents = this.entities.Set<S_D_Document>().
                Where(d => d.ProjectInfoID == projectInfoID && !d.ArchiveDate.HasValue
                    && d.S_D_DBS.ArchiveFolder != "" && d.S_D_DBS.ArchiveFolder != null).ToList();
            else
            {
                string sql = @"select m.* from S_D_Document m
cross apply (select top 1 * from S_D_Document d 
where ProjectInfoID='{0}'  and (ArchiveDate is null or ArchiveDate='')
and m.RelateID=d.RelateID order by d.Version desc) _d
left join S_D_DBS dbs on dbs.ID = m.DBSID
where m. ProjectInfoID='{0}'  and (m.ArchiveDate is null or m.ArchiveDate='')
and dbs.ArchiveFolder is not null and dbs.ArchiveFolder !=''  and _d.ID=m.ID";
                sql = string.Format(sql, projectInfoID);
                documents = this.SqlHelper.ExecuteList<S_D_Document>(sql);
            }
            return Json(documents);
        }

        public JsonResult ProjectTheseArchiveFiles(string projectInfoID, string rowStr)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var rows = JsonHelper.ToList(rowStr);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法归档");
            var documents = new List<S_D_Document>();
            foreach (var row in rows)
            {
                var doc = this.GetEntityByID<S_D_Document>(row.GetValue("ID"));
                if (doc != null)
                {
                    if (!String.IsNullOrEmpty(doc.S_D_DBS.ArchiveFolder))
                        documents.Add(doc);
                }
            }

            return Json(documents);
        }

        public JsonResult SynConfig(string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息");
            projectInfo.SyncDBS();

            var docConfigEntities = FormulaHelper.GetEntities<DocConfigEntities>();
            var dbslist = projectInfo.S_D_DBS.ToList();
            foreach (var dbs in dbslist)
            {
                if (!string.IsNullOrEmpty(dbs.ArchiveFolder))
                {
                    var nodeConfig = docConfigEntities.Set<S_DOC_Node>().FirstOrDefault(a => a.ID == dbs.ArchiveFolder);
                    if (nodeConfig != null && nodeConfig.IsFreeNode == DocSystem.Logic.Domain.TrueOrFalse.True.ToString())
                    {
                        dbslist.Where(a => a.FullID.StartsWith(dbs.FullID)).ToList().ForEach(a =>
                            {
                                a.ArchiveFolder = dbs.ArchiveFolder;
                                a.ArchiveFolderName = dbs.ArchiveFolderName;
                            });
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        #region 项目归档代码

        public JsonResult ArchiveFile(string ProjectInfoID, string Files)
        {
            Action action = () =>
               {
                   var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
                   if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法归档");
                   var rootFolder = projectInfo.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString());
                   if (String.IsNullOrEmpty(rootFolder.ArchiveFolder)) throw new Formula.Exceptions.BusinessException("没有为根节点定义归档目录，项目无法归档");


                   var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                   var dt = sqlHelper.ExecuteDataTable("select * from S_DOC_Node  where ID='" + rootFolder.ArchiveFolder + "'");
                   if (dt.Rows.Count == 0) throw new Formula.Exceptions.BusinessException("根节点定义的归档目录不存在，归档失败");
                   var folderDef = FormulaHelper.DataRowToDic(dt.Rows[0]);
                   var docSpace = DocSystem.Logic.DocConfigHelper.CreateConfigSpaceByID(folderDef.GetValue("SpaceID"));
                   if (docSpace == null) throw new Formula.Exceptions.BusinessException("所选择的档案空间定义不存在，请联系管理员确认档案空间配置是否正确");

                   #region 创建项目根节点
                   S_NodeInfo rootNode = S_NodeInfo.GetNode(docSpace.ID, rootFolder.ArchiveFolder, " where RelateID='" + projectInfo.DBSRoot.ID + "'");
                   if (rootNode == null)
                   {
                       rootNode = new DocSystem.Logic.Domain.S_NodeInfo(docSpace.ID, rootFolder.ArchiveFolder);
                       rootNode.Name = projectInfo.Name;
                       rootNode.DataEntity.SetValue("Code", projectInfo.Code);
                       rootNode.DataEntity.SetValue("State", DocState.Normal.ToString());
                   }
                   #endregion
                   this.SetAttr<S_I_ProjectInfo>(rootNode, projectInfo);
                   rootNode.DataEntity.SetValue("RelateID", projectInfo.DBSRoot.ID);
                   rootNode.Save(true);

                   var fileList = JsonHelper.ToList(Files);
                   foreach (var item in fileList)
                   {
                       if (item.Keys.Contains("NewAdd") && item.GetValue("NewAdd") == "T")
                           continue;
                       var doc = this.GetEntityByID<S_D_Document>(item.GetValue("DocumentID"));
                       if (doc == null) continue;
                       var folder = doc.S_D_DBS;
                       if (String.IsNullOrEmpty(folder.ArchiveFolder))
                           throw new Formula.Exceptions.BusinessException("没有为【" + folder.Name + "】配置指定归档信息，无法归档");
                       var node = rootNode.AllChildren.FirstOrDefault(d => d.RelateID == folder.ID);
                       if (node == null)
                       {
                           this._archiveDBS(projectInfo.DBSRoot, folder.Ansestors, rootNode, docSpace, false);
                           node = rootNode.AllChildren.FirstOrDefault(d => d.RelateID == folder.ID);
                       }
                       if (node == null) throw new Formula.Exceptions.BusinessException("目录创建失败，文件【" + doc.Name + "】归档失败");

                       #region 归档文件
                       var fileConfig = node.ConfigInfo.S_DOC_FileNodeRelation.FirstOrDefault();
                       if (fileConfig != null && fileConfig.S_DOC_File != null)
                       {
                           var file = S_FileInfo.GetFile(docSpace.ID, fileConfig.S_DOC_File.ID, " NodeID='" + node.ID + "' and RelateID='" + doc.ID + "' ");
                           if (file == null)
                           {
                               file = new DocSystem.Logic.Domain.S_FileInfo(docSpace.ID, fileConfig.S_DOC_File.ID);
                               if (!String.IsNullOrEmpty(doc.Attr))
                                   this.SetFileAttr(file, doc.Attr);
                               file.DataEntity.SetValue("Name", doc.Name);
                               file.DataEntity.SetValue("Code", doc.Code);
                               file.DataEntity.SetValue("RelateID", doc.ID);
                               file.DataEntity.SetValue("State", DocState.Normal);
                               if (!String.IsNullOrEmpty(doc.MainFiles) || !String.IsNullOrEmpty(doc.PDFFile) ||
                                   !String.IsNullOrEmpty(doc.PlotFile) || !String.IsNullOrEmpty(doc.XrefFile) ||
                                   !String.IsNullOrEmpty(doc.DwfFile) || !String.IsNullOrEmpty(doc.TiffFile) ||
                                   !String.IsNullOrEmpty(doc.SignPdfFile))
                               {
                                   var attachment = new S_Attachment(docSpace.ID);
                                   attachment.DataEntity.SetValue("MainFile", doc.MainFiles);
                                   attachment.DataEntity.SetValue("PDFFile", doc.PDFFile);
                                   attachment.DataEntity.SetValue("PlotFile", doc.PlotFile);
                                   attachment.DataEntity.SetValue("XrefFile", doc.XrefFile);
                                   attachment.DataEntity.SetValue("DwfFile", doc.DwfFile);
                                   attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                                   attachment.DataEntity.SetValue("SignPdfFile", doc.SignPdfFile);
                                   file.AddAttachment(attachment);
                               }
                               node.AddFile(file, true);
                           }
                           else
                           {
                               if (!String.IsNullOrEmpty(doc.Attr))
                                   this.SetFileAttr(file, doc.Attr);
                               file.DataEntity.SetValue("RelateID", doc.ID);
                               file.Save();
                               var attachment = new S_Attachment(docSpace.ID);
                               attachment.DataEntity.SetValue("MainFile", doc.MainFiles);
                               attachment.DataEntity.SetValue("PDFFile", doc.PDFFile);
                               attachment.DataEntity.SetValue("PlotFile", doc.PlotFile);
                               attachment.DataEntity.SetValue("XrefFile", doc.XrefFile);
                               attachment.DataEntity.SetValue("DwfFile", doc.DwfFile);
                               attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                               attachment.DataEntity.SetValue("SignPdfFile", doc.SignPdfFile);
                               file.AddAttachment(attachment);
                           }
                           doc.State = "Archive";
                           doc.ArchiveDate = DateTime.Now;
                           if (doc.RelateTable == "S_E_Product")
                           {
                               //string update = "Update S_E_Product Set ArchiveState='True', ArchiveDate='" + DateTime.Now.ToString() + "' where ID='" + doc.RelateID + "'";
                               string update = string.Format(@"update S_E_Product set ArchiveState='True', ArchiveDate='{2}' where ID='{0}'
update S_E_ProductVersion set ArchiveState='True', ArchiveDate='{2}' where ProductID='{0}' and Version='{1}'", doc.RelateID, doc.Version, DateTime.Now.ToString());
                               this.entities.Database.ExecuteSqlCommand(update);
                           }
                       }
                       #endregion

                   }
                   this.entities.SaveChanges();
               };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }
            return Json("");
        }

        public JsonResult Archive(string ProjectInfoID,bool IsAll)
        {
            Action action = () =>
                {
                    var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
                    if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法归档");

                    var rootFolder = projectInfo.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString());
                    if (String.IsNullOrEmpty(rootFolder.ArchiveFolder)) throw new Formula.Exceptions.BusinessException("没有为根节点定义归档目录，项目无法归档");

                    var archiveFolders = projectInfo.S_D_DBS.Where(d => !String.IsNullOrEmpty(d.ArchiveFolder)).ToList();

                    var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                    var dt = sqlHelper.ExecuteDataTable("select * from S_DOC_Node  where ID='" + rootFolder.ArchiveFolder + "'");
                    if (dt.Rows.Count == 0) throw new Formula.Exceptions.BusinessException("根节点定义的归档目录不存在，归档失败");
                    var folderDef = FormulaHelper.DataRowToDic(dt.Rows[0]);
                    var docSpace = DocSystem.Logic.DocConfigHelper.CreateConfigSpaceByID(folderDef.GetValue("SpaceID"));
                    if (docSpace == null) throw new Formula.Exceptions.BusinessException("所选择的档案空间定义不存在，请联系管理员确认档案空间配置是否正确");

                    S_NodeInfo rootNode = S_NodeInfo.GetNode(docSpace.ID, rootFolder.ArchiveFolder, " where RelateID='" + projectInfo.DBSRoot.ID + "'");
                    if (rootNode == null)
                    {
                        rootNode = new DocSystem.Logic.Domain.S_NodeInfo(docSpace.ID, rootFolder.ArchiveFolder);
                        rootNode.Name = projectInfo.Name;
                        rootNode.DataEntity.SetValue("Code", projectInfo.Code);
                        rootNode.DataEntity.SetValue("State", DocState.Normal.ToString());
                        rootNode.DataEntity.SetValue("RelateID", projectInfo.DBSRoot.ID);
                    }
                    this.SetAttr<S_I_ProjectInfo>(rootNode, projectInfo);
                    rootNode.Save(true);

                    var fileConfig = rootNode.ConfigInfo.S_DOC_FileNodeRelation.FirstOrDefault();
                    if (fileConfig != null && fileConfig.S_DOC_File != null)
                    {
                        Convert.ToDouble("");
                        var docList = projectInfo.DBSRoot.S_D_Document.Where(d => d.State != "Archive").ToList();
                        var _group = docList.GroupBy(a => a.RelateID).Select(a =>
                            new { a.Key, MaxVersion = a.Max(b => (string.IsNullOrEmpty(b.Version) ? 0d : Convert.ToDouble(b.Version))) }
                            ).ToList();
                        if (!IsAll)
                            docList = docList.Where(a => _group.Any(g => g.Key == a.RelateID && g.MaxVersion == (string.IsNullOrEmpty(a.Version) ? 0d : Convert.ToDouble(a.Version)))).ToList();
                        foreach (var doc in docList)
                        {
                            var file = new DocSystem.Logic.Domain.S_FileInfo(docSpace.ID, fileConfig.S_DOC_File.ID);
                            if (!String.IsNullOrEmpty(doc.Attr))
                                this.SetFileAttr(file, doc.Attr);
                            file.DataEntity.SetValue("Name", doc.Name);
                            file.DataEntity.SetValue("Code", doc.Code);
                            rootNode.AddFile(file, true);
                            if (!String.IsNullOrEmpty(doc.MainFiles) || !String.IsNullOrEmpty(doc.PDFFile) ||
                                !String.IsNullOrEmpty(doc.PlotFile) || !String.IsNullOrEmpty(doc.XrefFile) ||
                                !String.IsNullOrEmpty(doc.DwfFile) || !String.IsNullOrEmpty(doc.TiffFile) ||
                                !String.IsNullOrEmpty(doc.SignPdfFile))
                            {
                                var attachment = new S_Attachment(docSpace.ID);
                                attachment.DataEntity.SetValue("MainFile", doc.MainFiles);
                                attachment.DataEntity.SetValue("PDFFile", doc.PDFFile);
                                attachment.DataEntity.SetValue("PlotFile", doc.PlotFile);
                                attachment.DataEntity.SetValue("XrefFile", doc.XrefFile);
                                attachment.DataEntity.SetValue("DwfFile", doc.DwfFile);
                                attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                                attachment.DataEntity.SetValue("SignPdfFile", doc.SignPdfFile);
                                attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                                attachment.DataEntity.SetValue("CreateUser", this.CurrentUserInfo.UserID);
                                attachment.DataEntity.SetValue("CreateUserName", this.CurrentUserInfo.UserName);
                                file.AddAttachment(attachment);
                            }
                            doc.State = "Archive";
                            doc.ArchiveDate = DateTime.Now;
                            if (doc.RelateTable == "S_E_Product")
                            {
                                //string update = "Update S_E_Product Set ArchiveState='True', ArchiveDate='" + DateTime.Now.ToString() + "' where ID='" + doc.RelateID + "'";
                                string update = string.Format(@"update S_E_Product set ArchiveState='True', ArchiveDate='{2}' where ID='{0}'
update S_E_ProductVersion set ArchiveState='True', ArchiveDate='{2}' where ProductID='{0}' and Version='{1}'", doc.RelateID, doc.Version, DateTime.Now.ToString());
                                this.entities.Database.ExecuteSqlCommand(update);
                            }
                        }
                    }
                    this._archiveDBS(projectInfo.DBSRoot, archiveFolders, rootNode, docSpace, true, IsAll);
                    this.entities.SaveChanges();
                };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }
            return Json("");
        }

        void _archiveDBS(S_D_DBS parentDBS, List<S_D_DBS> archiveDbsList, S_NodeInfo parentNode, S_DOC_Space docSpace, bool archiveFiles = true, bool isAll = true)
        {
            var folders = archiveDbsList.Where(d => d.ParentID == parentDBS.ID).ToList();
            foreach (var folder in folders)
            {
                if (String.IsNullOrEmpty(folder.ArchiveFolder))
                {
                    continue;
                }
                S_NodeInfo node = S_NodeInfo.GetNode(docSpace.ID, folder.ArchiveFolder, " where Name='" + folder.Name + "' and ParentID='" + parentNode.ID + "' ");
                if (node == null)
                {
                    node = new DocSystem.Logic.Domain.S_NodeInfo(docSpace.ID, folder.ArchiveFolder);
                    this.SetAttr<S_D_DBS>(node, folder);
                    node.Name = folder.Name;
                    node.DataEntity.SetValue("State", DocState.Normal.ToString());
                    node.DataEntity.SetValue("RelateID", folder.ID);
                    parentNode.AddChild(node, true);
                }
                else
                {
                    this.SetAttr<S_D_DBS>(node, folder);
                    node.Name = folder.Name;
                    node.DataEntity.SetValue("State", DocState.Normal.ToString());
                    node.DataEntity.SetValue("RelateID", folder.ID);
                    node.Save();
                }
                if (archiveFiles && node.ConfigInfo.S_DOC_FileNodeRelation.Count > 0 && node.ConfigInfo.S_DOC_FileNodeRelation.FirstOrDefault().S_DOC_File != null)
                {
                    #region 归档文件
                    var fileConfig = node.ConfigInfo.S_DOC_FileNodeRelation.FirstOrDefault().S_DOC_File;
                    var docList = folder.S_D_Document.ToList();
                    var _group = docList.GroupBy(a => a.RelateID).Select(a =>
                        new { a.Key, MaxVersion = a.Max(b => (string.IsNullOrEmpty(b.Version) ? 0d : Convert.ToDouble(b.Version))) }
                        ).ToList();
                    if (!isAll)
                        docList = docList.Where(a => _group.Any(g => g.Key == a.RelateID && g.MaxVersion == (string.IsNullOrEmpty(a.Version) ? 0d : Convert.ToDouble(a.Version)))).ToList();
                    foreach (var doc in docList)
                    {
                        var file = S_FileInfo.GetFile(docSpace.ID, fileConfig.ID, " NodeID='" + node.ID + "' and RelateID='" + doc.ID + "' ");
                        if (file == null)
                        {
                            file = new DocSystem.Logic.Domain.S_FileInfo(docSpace.ID, fileConfig.ID);
                            if (!String.IsNullOrEmpty(doc.Attr))
                                this.SetFileAttr(file, doc.Attr);
                            file.DataEntity.SetValue("Name", doc.Name);
                            file.DataEntity.SetValue("Code", doc.Code);
                            file.DataEntity.SetValue("RelateID", doc.ID);
                            file.DataEntity.SetValue("State", DocState.Normal);
                            if (!String.IsNullOrEmpty(doc.MainFiles) || !String.IsNullOrEmpty(doc.PDFFile) ||
                                !String.IsNullOrEmpty(doc.PlotFile) || !String.IsNullOrEmpty(doc.XrefFile) ||
                                !String.IsNullOrEmpty(doc.DwfFile) || !String.IsNullOrEmpty(doc.TiffFile) ||
                                !String.IsNullOrEmpty(doc.SignPdfFile))
                            {
                                var attachment = new S_Attachment(docSpace.ID);
                                attachment.DataEntity.SetValue("MainFile", doc.MainFiles);
                                attachment.DataEntity.SetValue("PDFFile", doc.PDFFile);
                                attachment.DataEntity.SetValue("PlotFile", doc.PlotFile);
                                attachment.DataEntity.SetValue("XrefFile", doc.XrefFile);
                                attachment.DataEntity.SetValue("DwfFile", doc.DwfFile);
                                attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                                attachment.DataEntity.SetValue("SignPdfFile", doc.SignPdfFile);
                                attachment.DataEntity.SetValue("CreateUser", this.CurrentUserInfo.UserID);
                                attachment.DataEntity.SetValue("CreateUserName", this.CurrentUserInfo.UserName);
                                file.AddAttachment(attachment);
                            }
                            node.AddFile(file, true);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(doc.Attr))
                                this.SetFileAttr(file, doc.Attr);
                            file.Save();
                            var attachment = new S_Attachment(docSpace.ID);
                            attachment.DataEntity.SetValue("MainFile", doc.MainFiles);
                            attachment.DataEntity.SetValue("PDFFile", doc.PDFFile);
                            attachment.DataEntity.SetValue("PlotFile", doc.PlotFile);
                            attachment.DataEntity.SetValue("XrefFile", doc.XrefFile);
                            attachment.DataEntity.SetValue("DwfFile", doc.DwfFile);
                            attachment.DataEntity.SetValue("TiffFile", doc.TiffFile);
                            attachment.DataEntity.SetValue("SignPdfFile", doc.SignPdfFile);
                            attachment.DataEntity.SetValue("CreateUser", this.CurrentUserInfo.UserID);
                            attachment.DataEntity.SetValue("CreateUserName", this.CurrentUserInfo.UserName);
                            file.AddAttachment(attachment);
                        }
                        doc.State = "Archive";
                        doc.ArchiveDate = DateTime.Now;
                        if (doc.RelateTable == "S_E_Product")
                        {
                            //string update = "Update S_E_Product Set ArchiveState='True', ArchiveDate='" + DateTime.Now.ToString() + "' where ID='" + doc.RelateID + "'";
                            string update = string.Format(@"update S_E_Product set ArchiveState='True', ArchiveDate='{2}' where ID='{0}'
update S_E_ProductVersion set ArchiveState='True', ArchiveDate='{2}' where ProductID='{0}' and Version='{1}'", doc.RelateID, doc.Version, DateTime.Now.ToString());
                            this.entities.Database.ExecuteSqlCommand(update);
                        }
                    }
                    #endregion
                }
                _archiveDBS(folder, archiveDbsList, node, docSpace, archiveFiles, isAll);
            }
        }

        #region 归档用方法，根据属性自动赋值档案属性
        void SetAttr<T>(S_NodeInfo node, T entity)
        {
            var dic = FormulaHelper.ModelToDic<T>(entity);
            SetAttr(node, dic);
        }

        void SetAttr(S_NodeInfo node, Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                if (key == "ID" || key == "ParentID" || key == "FullPathID" || key == "SpaceID"
                    || key == "ConfigID" || key == "State" || key == "BorrowState"
                    || key == "BorrowUserID" || key == "BorrowUserName") continue;
                node.DataEntity.SetValue(key, dic.GetValue(key));
            }
        }

        void SetAttr(S_NodeInfo node, string Attr)
        {
            var dic = JsonHelper.ToObject(Attr);
            SetAttr(node, dic);
        }

        void SetFileAttr(S_FileInfo file, string Attr)
        {
            var dic = JsonHelper.ToObject(Attr);
            foreach (string key in dic.Keys)
            {
                if (key == "ID" || key == "NodeID" || key == "SpaceID" || key == "ConfigID" || key == "FullNodeID" || key == "State" || key == "BorrowState"
                    || key == "BorrowUserID" || key == "BorrowUserName") continue;
                file.DataEntity.SetValue(key, dic.GetValue(key));
            }
        }
        #endregion

        #endregion

    }
}
