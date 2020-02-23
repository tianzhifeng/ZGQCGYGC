using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Formula;
using MvcAdapter;
using Formula.Helper;
using Config;
using Config.Logic;
using System.Data;
using Base.Logic.BusinessFacade;
using Formula.ImportExport;
using System.Web;


namespace Project.Logic.Domain
{
    public partial class S_D_DBS
    {
        #region 公开属性

        List<S_D_DBS> _children;
        /// <summary>
        /// 获取DBS子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_DBS> Children
        {
            get
            {
                if (_children == null)
                    _children = this.S_I_ProjectInfo.S_D_DBS.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }

        List<S_D_DBS> _allchildren;
        /// <summary>
        /// 获取DBS所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_DBS> AllChildren
        {
            get
            {
                if (_allchildren == null)
                    _allchildren = this.S_I_ProjectInfo.S_D_DBS.Where(d => d.FullID.StartsWith(this.FullID)).ToList();
                return _allchildren;
            }
        }

        List<S_D_DBS> _Ansestors;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_DBS> Ansestors
        {
            get
            {
                if (_Ansestors == null)
                    _Ansestors = this.S_I_ProjectInfo.S_D_DBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                return _Ansestors;
            }
        }

        S_D_DBS _parent;
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_D_DBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_ProjectInfo.S_D_DBS.SingleOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        #endregion

        /// <summary>
        /// 增加DBS子目录
        /// </summary>
        /// <param name="child">子目录对象</param>
        public S_D_DBS AddChild(S_D_DBS child)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_D_DBS>(child).State != System.Data.EntityState.Added && entities.Entry<S_D_DBS>(child).State != System.Data.EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态的DBS对象，无法调用AddChild方法");
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            if (String.IsNullOrEmpty(child.DBSType)) child.DBSType = Project.Logic.DBSType.Folder.ToString();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.ProjectInfoID = this.ProjectInfoID;
            child.S_I_ProjectInfo = this.S_I_ProjectInfo;
            child.CreateDate = DateTime.Now;
            child.CreateUserID = user.UserID;
            child.CreateUser = user.UserName;
            child.IsPublic = true;
            this.S_I_ProjectInfo.S_D_DBS.Add(child);
            child.InheritParentAuth();
            this.Children.Add(child);
            this.AllChildren.Add(child);
            return child;
        }

        /// <summary>
        /// 增加DBS子节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="code">节点编号</param>
        public S_D_DBS AddChild(string name, string code = "")
        {
            var child = new S_D_DBS();
            child.DBSCode = code;
            child.Name = name;
            child.DBSType = Project.Logic.DBSType.Folder.ToString();
            return this.AddChild(child);
        }

        /// <summary>
        /// 增加DBS节点
        /// </summary>
        /// <param name="define">DBS节点定义</param>
        public S_D_DBS AddChild(S_T_DBSDefine define)
        {
            var user = FormulaHelper.GetUserInfo();
            var child = new S_D_DBS();
            child.ID = FormulaHelper.CreateGuid();
            child.DBSCode = define.DBSCode;
            child.Name = define.Name;
            child.DBSType = define.DBSType;
            child.MappingNodeUrl = define.MappingNodeUrl;
            child.MappingType = define.MappingType;
            child.CreateDate = DateTime.Now;
            child.CreateUserID = user.UserID;
            child.CreateUser = user.UserName;
            child.InheritAuth = define.InheritAuth;
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.ProjectInfoID = this.ProjectInfoID;
            child.S_I_ProjectInfo = this.S_I_ProjectInfo;
            child.ConfigDBSID = define.ID;
            child.ArchiveFolder = define.ArchiveFolder;
            child.ArchiveFolderName = define.ArchiveFolderName;
            this.S_I_ProjectInfo.S_D_DBS.Add(child);
            foreach (var item in define.S_T_DBSSecurity.ToList())
            {
                var sec = new S_D_DBSSecurity();
                sec.ID = FormulaHelper.CreateGuid();
                sec.RoleCode = item.RoleCode;
                sec.RoleName = item.RoleName;
                sec.AuthType = item.AuthType;
                sec.RelateValue = "";
                child.S_D_DBSSecurity.Add(sec);
            }
            return child;
        }

        /// <summary>
        /// 删除DBS目录及所有下层子目录
        /// </summary>
        /// <param name="validateMode">是否进行操作配置校验</param>
        public void Delete(bool validateMode = true)
        {
            foreach (var child in this.AllChildren)
                child.DeleteSelf();
            this.DeleteSelf();
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="document">文档对象</param>
        public void AddDocument(S_D_Document document)
        {
            document.DBSID = this.ID;
            document.ProjectInfoID = this.ProjectInfoID;
            document.CreateDate = DateTime.Now;
            document.DBSFullID = this.FullID;
            if (!document.IsPublic.HasValue)
                document.IsPublic = true;
            if (String.IsNullOrEmpty(document.CreateUserID))
            {
                var userInfo = FormulaHelper.GetUserInfo();
                document.CreateUserID = userInfo.UserID;
                document.CreateUser = userInfo.UserName;
            }
            this.S_D_Document.Add(document);
        }

        /// <summary>
        /// 清除DBS目录关联的所有文档
        /// </summary>
        public void ClearDocument()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            entities.S_D_Document.Delete(d => d.DBSID == this.ID);
        }

        /// <summary>
        /// 根据角色来设置DBS对象的权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="authType">权限类别</param>
        public void SetAuth(string roleCode, string roleName, FolderAuthType authType, string roleType)
        {
            this.SetAuth(roleCode, roleName, authType.ToString(), roleType);
        }

        /// <summary>
        /// 根据角色来设置DBS对象的权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="authType">权限类别</param>
        public void SetAuth(string roleCode, string roleName, string authType, string roleType)
        {
            var security = this.S_D_DBSSecurity.FirstOrDefault(d => d.RoleCode == roleCode);
            if (security == null)
            {
                security = new S_D_DBSSecurity();
                security.ID = FormulaHelper.CreateGuid();
                security.RoleCode = roleCode;
                security.RoleName = roleName;
                security.RoleType = roleType;
                this.S_D_DBSSecurity.Add(security);
            }
            security.AuthType = authType;
            this.InheritAuth = false.ToString();
            foreach (var child in this.AllChildren.Where(d => d.InheritAuth == true.ToString()).ToList())
                child.InheritNodeAuth(this);
        }

        /// <summary>
        /// 继承父节点权限
        /// </summary>
        public void InheritParentAuth()
        {
            if (this.Parent == null) return;
            this.InheritNodeAuth(this.Parent);
            foreach (var child in this.AllChildren.Where(d => d.InheritAuth == true.ToString()).ToList())
                child.InheritNodeAuth(this);
        }

        /// <summary>
        /// 集成指定目录的权限
        /// </summary>
        /// <param name="dbs">指定的DBS目录</param>
        public void InheritNodeAuth(S_D_DBS dbs)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            entities.S_D_DBSSecurity.Delete(d => d.DBSID == this.ID);
            foreach (var security in dbs.S_D_DBSSecurity.ToList())
            {
                var sec = new S_D_DBSSecurity();
                sec.ID = FormulaHelper.CreateGuid();
                sec.RoleCode = security.RoleCode;
                sec.RoleName = security.RoleName;
                sec.AuthType = security.AuthType;
                sec.RelateValue = security.RelateValue;
                sec.RoleType = security.RoleType;
                sec.DBSID = this.ID;
                this.S_D_DBSSecurity.Add(sec);
                sec.S_D_DBS = this;
            }
            this.InheritAuth = true.ToString();
        }

        /// <summary>
        /// 移除DBS权限
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        public void RemoveAuth(string roleCode)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            entities.S_D_DBSSecurity.Delete(d => d.DBSID == this.ID && d.RoleCode == roleCode);
        }

        /// <summary>
        ///获得人员在DBS上的权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>权限枚举</returns>
        public FolderAuthType GetUserAuth(string userID)
        {
            var result = FolderAuthType.None;
            var roles = this.S_I_ProjectInfo.GetUserRoleCodes(userID);
            if (this.DBSType == Project.Logic.DBSType.Cloud.ToString())
                return FolderAuthType.FullControl;
            var typeStr = RoleType.ProjectRoleType.ToString();
            foreach (var role in roles.Split(','))
            {
                if (result == FolderAuthType.FullControl) break;
                var sec = this.S_D_DBSSecurity.FirstOrDefault(d => d.RoleCode == role && d.RoleType == typeStr);
                if (sec == null) continue;
                var auth = (FolderAuthType)Enum.Parse(typeof(FolderAuthType), sec.AuthType);
                if (Convert.ToInt32(auth) <= Convert.ToInt32(result))
                    result = auth;
            }
            //支持系统角色和组织角色
            var sysroles = FormulaHelper.GetService<IUserService>().GetRoleCodesForUser(userID, this.S_I_ProjectInfo.ChargeDeptID);
            foreach (var role in sysroles.Split(','))
            {
                if (result == FolderAuthType.FullControl) break;
                var sec = this.S_D_DBSSecurity.FirstOrDefault(d => d.RoleCode == role && d.RoleType != typeStr);
                if (sec == null) continue;
                var auth = (FolderAuthType)Enum.Parse(typeof(FolderAuthType), sec.AuthType);
                if (Convert.ToInt32(auth) <= Convert.ToInt32(result))
                    result = auth;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GatherDocuments(IFormExport export)
        {
            if (this.DBSType != Project.Logic.DBSType.Mapping.ToString())
            {
                throw new Formula.Exceptions.BusinessException("只有映射目录可以调用资料收集方法");
            }
            if (this.MappingType == Project.Logic.DBSMappingType.Product.ToString())
            {
                _GatherProducts();
            }
            else if (this.MappingType == Project.Logic.DBSMappingType.ISO.ToString())
            {
                _GatherISOForm(export);
            }
            else if (this.MappingType == Project.Logic.DBSMappingType.DesignInput.ToString())
            {
                _GatherInput();
            }
            //else if (this.MappingType == Project.Logic.DBSMappingType.Cooperation.ToString())
            //{
            //    _GatherCoopration();
            //}
        }

        public void GatherProducts()
        {
            if (this.DBSType != Project.Logic.DBSType.Mapping.ToString() || this.MappingType != Project.Logic.DBSMappingType.Product.ToString())
            {
                throw new Formula.Exceptions.BusinessException("只有成果映射目录可以调用资料收集方法");
            }
            _GatherProducts();
        }

        /// <summary>
        /// 销毁DBS目录本身
        /// </summary>
        /// <param name="validateMode">是否进行操作配置校验</param>
        internal void DeleteSelf()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            this.ClearDocument();
            entities.S_D_DBSSecurity.Delete(d => d.DBSID == this.ID);
            entities.S_D_DBS.Remove(this);
        }

        #region 成果资料归集
        private void _GatherProducts()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var auditState = AuditState.Pass.ToString();
            var products = entities.S_E_Product.Where(d => d.ProjectInfoID == this.ProjectInfoID && d.AuditState == auditState).ToList();
            var dbsConfig = this.S_I_ProjectInfo.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.ID == this.ConfigDBSID);
            if (dbsConfig == null) return;
            var existFolderList = this.AllChildren;
            var configStructList = _initProductFoderConfig();
            foreach (var product in products)
            {
                if (product.State == ProductState.Invalid.ToString() || product.State == ProductState.InInvalid.ToString())
                    continue;
                if (configStructList.Count == 0)
                {
                    #region 增加成果至归档目录
                    _archiveProduct(this, product);
                    #endregion
                }
                else
                {
                    this._createProductMappingFolder(this, "", configStructList, existFolderList, product, dbsConfig);
                }
            }
        }

        List<Dictionary<string, object>> _initProductFoderConfig()
        {
            var result = new List<Dictionary<string, object>>();
            var dbsConfig = this.S_I_ProjectInfo.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.ID == this.ConfigDBSID);
            if (dbsConfig == null) return result;
            result = JsonHelper.ToList(dbsConfig.ProductStruct);
            for (int i = 0; i < result.Count; i++)
            {
                var configItem = result[i];
                configItem.SetValue("AddProduct", false);
                if (i == 0)
                {
                    configItem.SetValue("ParentID", "");
                }
                else
                {
                    var parent = result[i - 1];
                    configItem.SetValue("ParentID", parent.GetValue("FieldName"));
                }
                if (i == result.Count - 1)
                {
                    configItem.SetValue("AddProduct", true);
                }
            }
            return result;
        }

        void _createProductMappingFolder(S_D_DBS parent, string parentDefineCode, List<Dictionary<string, object>> folderDefines, List<S_D_DBS> existFolderList,
            S_E_Product product, S_T_DBSDefine defineInfo)
        {
            var folderDefine = folderDefines.FirstOrDefault(d => d["ParentID"].ToString() == parentDefineCode);
            if (folderDefine == null) return;
            var value = product.GetPropertyString(folderDefine.GetValue("FieldName"));
            if (String.IsNullOrEmpty(value))
            {
                _createProductMappingFolder(parent, folderDefine.GetValue("FieldName"), folderDefines, existFolderList, product, defineInfo);
            }
            else
            {
                if (!String.IsNullOrEmpty(folderDefine.GetValue("EnumKey")))
                {
                    var enumService = FormulaHelper.GetService<IEnumService>();
                    value = enumService.GetEnumText(folderDefine.GetValue("EnumKey"), value);
                }
                var folder = existFolderList.FirstOrDefault(d => d.Name == value && d.ParentID == parent.ID);
                if (folder == null)
                {
                    folder = new S_D_DBS();
                    folder.ID = FormulaHelper.CreateGuid();
                    folder.Name = value;
                    folder.DBSCode = folderDefine.GetValue("FieldName");
                    folder.DBSType = Project.Logic.DBSType.Mapping.ToString();
                    folder.MappingType = parent.MappingType;
                    folder.ConfigDBSID = defineInfo.ID + "." + folderDefine.GetValue("FieldName");
                    folder.ArchiveFolder = folderDefine.GetValue("ArchiveFolder");
                    folder.ArchiveFolderName = folderDefine.GetValue("ArchiveFolderName");
                    parent.AddChild(folder);
                    existFolderList.Add(folder);
                }

                bool addOnDocument = true;// Convert.ToBoolean(folderDefine.GetValue("AddProduct"));
                //根据归档结构决定是否添加成果
                var ansestorFolders = existFolderList.Where(d => folder.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                var preFlag = true;
                for (int i = 0; i < folderDefines.Count; i++)
                {
                    var fd = folderDefines[i];
                    var pdValue = product.GetPropertyString(fd.GetValue("FieldName"));//成果的结构所属字段对应值（阶段、子项、专业、工作包）
                    if (!String.IsNullOrEmpty(fd.GetValue("EnumKey")))
                    {
                        var enumService = FormulaHelper.GetService<IEnumService>();
                        pdValue = enumService.GetEnumText(fd.GetValue("EnumKey"), pdValue);
                    }
                    if (preFlag)
                    {
                        if (!string.IsNullOrEmpty(pdValue) && !ansestorFolders.Any(a => a.Name == pdValue)) //非空校验处理，成果只有阶段、专业属性情况
                        {
                            addOnDocument = false;//成果不属于当前分支，继续创建分支
                            break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pdValue)) //专业下有成果、工作包下也有成果，只有成果的工作包属性是空时，才归档在专业下
                        {
                            addOnDocument = false;
                            break;
                        }
                    }
                    if (fd == folderDefine)
                        preFlag = false;
                }
                if (addOnDocument)
                {
                    #region 增加成果至归档目录
                    _archiveProduct(folder, product);
                    #endregion
                }
                else
                {
                    _createProductMappingFolder(folder, folderDefine.GetValue("FieldName"), folderDefines, existFolderList, product, defineInfo);
                }
            }
        }

        void _archiveProduct(S_D_DBS dbs, S_E_Product product)
        {
            #region 增加成果至归档目录
            //20190508 一个成果本版一个document记录，不用version表记录，为了多个版本独立档案记录
            var productVersion = product.Version.HasValue ? product.Version.Value.ToString() : "";
            var doc = dbs.S_D_Document.FirstOrDefault(d => d.RelateID == product.ID && d.Version == productVersion);
            if (doc == null)
            {
                doc = new S_D_Document();
                doc.ID = FormulaHelper.CreateGuid();
                doc.Name = product.Name;
                doc.Code = product.Code;
                doc.MajorValue = product.MajorValue;
                doc.Attr = JsonHelper.ToJson(product);
                doc.Catagory = product.MonomerInfo;
                doc.RelateID = product.ID;
                doc.RelateTable = "S_E_Product";
                doc.CreateUser = product.CreateUser;
                doc.CreateUserID = product.CreateUserID;
                doc.State = "Normal";
                doc.MainFiles = product.MainFile;
                doc.PDFFile = product.PdfFile;
                doc.PlotFile = product.PlotFile;
                doc.XrefFile = product.XrefFile;
                doc.DwfFile = product.DwfFile;
                doc.TiffFile = product.TiffFile;
                doc.SignPdfFile = product.SignPdfFile;
                doc.Version = productVersion;
                doc.ArchiveDate = null;
                dbs.AddDocument(doc);
                //doc.AddDocumentVersion();
            }
            //else if (doc.Version != productVersion)
            //{
            //    doc.Name = product.Name;
            //    doc.Code = product.Code;
            //    doc.Attr = JsonHelper.ToJson(product);
            //    doc.Catagory = product.MonomerInfo;

            //    doc.MajorValue = product.MajorValue;
            //    doc.MainFiles = product.MainFile;
            //    doc.PDFFile = product.PdfFile;
            //    doc.PlotFile = product.PlotFile;
            //    doc.XrefFile = product.XrefFile;
            //    doc.DwfFile = product.DwfFile;
            //    doc.TiffFile = product.TiffFile;
            //    doc.SignPdfFile = product.SignPdfFile;
            //    doc.Version = product.Version.HasValue ? product.Version.Value.ToString() : "";
            //    doc.ArchiveDate = null;
            //    doc.State = "Normal";
            //    doc.AddDocumentVersion();
            //}
            else if (doc.State != "Archive")
            {
                doc.PDFFile = product.PdfFile;
                doc.PlotFile = product.PlotFile;
                doc.XrefFile = product.XrefFile;
                doc.DwfFile = product.DwfFile;
                doc.TiffFile = product.TiffFile;
                doc.SignPdfFile = product.SignPdfFile;

                var docVer = doc.S_D_DocumentVersion.FirstOrDefault(a => a.Version == doc.Version);
                if (docVer != null)
                {
                    docVer.PDFFile = doc.PDFFile;
                    docVer.PlotFile = doc.PlotFile;
                    docVer.XrefFile = doc.XrefFile;
                    docVer.DwfFile = doc.DwfFile;
                    docVer.TiffFile = doc.TiffFile;
                    docVer.SignPdfFile = doc.SignPdfFile;
                }
            }
            #endregion
        }
        #endregion

        #region ISO单资料归集
        private void _GatherISOForm(IFormExport export)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var isoDefineList = this.S_I_ProjectInfo.ProjectMode.S_T_ISODefine.ToList();
            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var existFolderList = this.AllChildren;
            var dbsConfig = this.S_I_ProjectInfo.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.ID == this.ConfigDBSID);
            if (dbsConfig == null) return;
            var configStructList = _initProductFoderConfig();
            foreach (var isoDefine in isoDefineList)
            {
                if (string.IsNullOrEmpty(isoDefine.FormCode))
                {
                    continue;
                }
                if (configStructList.Count > 0)
                {
                    _createISOMappingFolder(this, "", configStructList, existFolderList, isoDefine, dbsConfig, export);
                }
                else
                {
                    _createISOForm(isoDefine, this, export);
                }
            }
        }

        void _createISOForm(S_T_ISODefine isoDefine, S_D_DBS fileFolder, IFormExport export)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var db = SQLHelper.CreateSqlHelper(isoDefine.ConnName);
            string sql = "select * from {0} where ProjectInfoID='{1}' and FlowPhase='End'";
            var table = db.ExecuteDataTable(String.Format(sql, isoDefine.TableName, this.ProjectInfoID));
            foreach (DataRow isoRow in table.Rows)
            {
                #region 创建ISO表单
                var enumDefList = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(isoDefine.EnumFieldInfo))
                    enumDefList = JsonHelper.ToList(isoDefine.EnumFieldInfo);
                string name = Function.ReplaceRegString(isoDefine.NameFieldInfo, FormulaHelper.DataRowToDic(isoRow), enumDefList);
                var doc = fileFolder.S_D_Document.FirstOrDefault(d => d.RelateID == isoRow["ID"].ToString());
                var archiveFields = String.IsNullOrEmpty(isoDefine.ArchiveFields) ? "" : isoDefine.ArchiveFields;
                var isoDic = FormulaHelper.DataRowToDic(isoRow);
                var archiveAttachFields = archiveFields.Split(',');
                if (doc == null)
                {
                    doc = entities.S_D_Document.Create();
                    doc.ID = FormulaHelper.CreateGuid();
                    doc.Name = name;
                    doc.Attr = JsonHelper.ToJson(isoDic);
                    doc.Catagory = isoDefine.Name;
                    doc.MajorValue = isoDic.GetValue("MajorValue");
                    var pdfFiles = export.ExportPDF(isoDefine.Name, isoRow["ID"].ToString(), isoDefine.FormCode);
                    doc.Code = isoDic.GetValue("SerialNumber");
                    doc.Version = isoDic.GetValue("VersionNumber"); 
                    doc.MainFiles = pdfFiles;
                    doc.PDFFile = pdfFiles;
                    doc.RelateID = isoRow["ID"].ToString();
                    doc.RelateTable = isoDefine.TableName;
                    doc.CreateUser = isoDic.GetValue("CreateUser");
                    doc.CreateUserID = isoDic.GetValue("CreateUserID"); 
                    doc.State = "Normal";

                    foreach (var field in archiveAttachFields)
                    {
                        if (string.IsNullOrEmpty(isoDic.GetValue(field)))
                            continue;
                        doc.Attachments +=isoDic.GetValue(field)+",";
                    }
                    if (!string.IsNullOrEmpty(doc.Attachments))
                        doc.Attachments = doc.Attachments.TrimEnd(',');
                    fileFolder.AddDocument(doc);
                    //doc.AddDocumentVersion();
                }
                else if (string.IsNullOrEmpty(doc.MainFiles))
                {
                    var pdfFiles = export.ExportPDF(isoDefine.Name, isoRow["ID"].ToString(), isoDefine.FormCode);
                    doc.MainFiles = pdfFiles;
                    doc.PDFFile = pdfFiles;
                    foreach (var field in archiveAttachFields)
                    {
                        if (string.IsNullOrEmpty(isoDic.GetValue(field)))
                            continue;
                        doc.Attachments += isoDic.GetValue(field) + ",";
                    }
                    if (!string.IsNullOrEmpty(doc.Attachments))
                        doc.Attachments = doc.Attachments.TrimEnd(',');
                }
                #endregion
            }
        }

        void _createISOMappingFolder(S_D_DBS parent, string parentDefineCode, List<Dictionary<string, object>> folderDefines, List<S_D_DBS> existFolderList,
              S_T_ISODefine objMode, S_T_DBSDefine defineInfo, IFormExport export)
        {
            var folderDefine = folderDefines.FirstOrDefault(d => d["ParentID"].ToString() == parentDefineCode);
            if (folderDefine == null) return;
            var value = objMode.GetPropertyString(folderDefine.GetValue("FieldName"));
            if (String.IsNullOrEmpty(value)) { return; }
            if (!String.IsNullOrEmpty(folderDefine.GetValue("EnumKey")))
            {
                var enumService = FormulaHelper.GetService<IEnumService>();
                value = enumService.GetEnumText(folderDefine.GetValue("EnumKey"), value);
            }
            var folder = existFolderList.FirstOrDefault(d => d.Name == value && d.ParentID == parent.ID);
            if (folder == null)
            {
                folder = new S_D_DBS();
                folder.ID = FormulaHelper.CreateGuid();
                folder.Name = value;
                folder.DBSCode = objMode.ID;
                folder.DBSType = Project.Logic.DBSType.Mapping.ToString();
                folder.ConfigDBSID = defineInfo.ID + "." + folderDefine.GetValue("FieldName");
                folder.ArchiveFolder = folderDefine.GetValue("ArchiveFolder");
                folder.MappingType = parent.MappingType;
                folder.ArchiveFolderName = folderDefine.GetValue("ArchiveFolderName");
                parent.AddChild(folder);
                existFolderList.Add(folder);
            }
            bool addOnDocument = Convert.ToBoolean(folderDefine.GetValue("AddProduct"));
            if (addOnDocument)
            {
                if (folder.DBSCode == objMode.ID)
                    _createISOForm(objMode, folder, export);
            }
        }
        #endregion

        private void _GatherInput()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            string sql = @"select S_D_InputDocument.ID,InfoName,S_D_InputDocument.Catagory,S_D_Input.InputType,Name,Files,S_D_InputDocument.CreateUser,
S_D_InputDocument.CreateUserID,S_D_InputDocument.CreateDate from dbo.S_D_InputDocument
LEFT JOIN  S_D_Input on S_D_InputDocument.InputID= S_D_Input.ID
where ProjectInfoID='" + this.ProjectInfoID + "' ";
            var sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var docTable = sqlDB.ExecuteDataTable(sql);
            foreach (DataRow item in docTable.Rows)
            {
                var doc = this.S_D_Document.FirstOrDefault(d => d.RelateID == item["ID"].ToString());
                if (doc == null)
                {
                    doc = entities.S_D_Document.Create();
                    doc.ID = FormulaHelper.CreateGuid();
                    doc.RelateID = item["ID"].ToString();
                    doc.RelateTable = "S_D_InputDocument";
                    if (docTable.Columns.Contains("CreateUserID") && docTable.Columns.Contains("CreateUser"))
                    {
                        doc.CreateUser = item["CreateUser"].ToString();
                        doc.CreateUserID = item["CreateUserID"].ToString();
                    }
                    doc.State = "Normal";
                    this.AddDocument(doc);
                }
                doc.Name = item["InfoName"].ToString() + "(" + item["Name"].ToString() + ")";
                doc.Attr = JsonHelper.ToJson(item);
                doc.Catagory = item["InputType"].ToString();
                doc.MajorValue = item["Catagory"].ToString();
                if (doc.MajorValue == "Project") { doc.MajorValue = ""; }
                doc.MainFiles = item["Files"].ToString();
                //doc.AddDocumentVersion();
            }

        }

        private void _GatherCoopration()
        { }
    }
}
