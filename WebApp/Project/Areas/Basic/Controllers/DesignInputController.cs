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
using System.Configuration;


namespace Project.Areas.Basic.Controllers
{
    public class DesignInputController : ProjectController<S_D_Input>
    {
        public override ActionResult List()
        {
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到指定的项目信息，请确认ID为【" + projectInfoID + "】项目是否存在");
            ViewBag.ProjectInfo = JsonHelper.ToJson(projectInfo);
            ViewBag.ProjectInfoID = projectInfo.ID;
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.IsEngineeringMode = ConfigurationManager.AppSettings["EngineeringMode"].ToLower();
            return View();
        }

        public ActionResult MajorList()
        {
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到指定的项目信息，请确认ID为【" + projectInfoID + "】项目是否存在");
            ViewBag.ProjectInfo = JsonHelper.ToJson(projectInfo);
            ViewBag.ProjectInfoID = projectInfo.ID;
            ViewBag.SpaceCode = this.GetQueryString("SpaceCode");
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.FuncType = this.GetQueryString("FuncType");
            ViewBag.FuncTitle = "设计输入资料";
            ViewBag.FlowTitle = "设计输入评审";
            var spaceCode = this.GetQueryString("SpaceCode");
            if (!String.IsNullOrEmpty(spaceCode))
            {
                var major = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major).FirstOrDefault(d => d.Code == spaceCode);
                if (major != null)
                {
                    ViewBag.FuncTitle = major.Name + "-设计输入资料";
                    ViewBag.FlowTitle = major.Name + "-设计输入评审";
                }
            }
            return View();
        }

        protected override void AfterGetMode(S_D_Input entity, bool isNew)
        {
            if (isNew)
            {
                entity.Catagory = this.GetQueryString("SpaceCode");
            }
        }

        protected override void BeforeDelete(List<S_D_Input> entityList)
        {
            var catagory = this.Request["SpaceCode"];
            foreach (var item in entityList)
            {
                if (item.Catagory != catagory) throw new Formula.Exceptions.BusinessException("不能删除除本专业以外的设计输入资料");
                if (item.S_D_InputDocument.Count(d => d.AuditState == CommonConst.finishAuditState || d.AuditState == CommonConst.inAuditState) > 0)
                    throw new Formula.Exceptions.BusinessException("【" + item.InfoName + "】已经发起评审或通过评审，不能进行删除操作");
            }
        }

        protected override void BeforeSave(S_D_Input entity, bool isNew)
        {
            if (isNew)
            {
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目，无法增加设计输入");
                entity.EngineeringInfoID = projectInfo.GroupID;
                entity.State = CommonConst.unAuditState;
                var maxSortIndex = this.entities.Set<S_D_Input>().Where(d => d.ProjectInfoID == projectInfo.ID).Max(d => d.SortIndex);
                if (maxSortIndex.HasValue)
                    entity.SortIndex = maxSortIndex.Value + 100;
                else
                    entity.SortIndex = 0;
            }
        }

        public JsonResult GetTreeList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            qb.Add("ProjectInfoID", QueryMethod.Equal, projectInfoID);
            //qb.Add("Catagory", QueryMethod.Equal, "Project");
            qb.SortField = "InputTypeIndex,SortIndex"; qb.SortOrder = "asc,asc";
            var result = new List<Dictionary<string, object>>();
            var list = this.entities.Set<S_D_Input>().Include("S_D_InputDocument").Where(qb).Select(
                d => new
                {
                    ID = d.ID,
                    InfoName = d.InfoName,
                    Catagory = d.Catagory,
                    InputType = d.InputType,
                    SortIndex = d.SortIndex,
                    TypeIndex = d.InputTypeIndex,
                    FileCount = d.S_D_InputDocument.Count,
                    LastUploadDate = d.S_D_InputDocument.Max(p => p.CreateDate)
                }).ToList();

            var typeList = list.OrderBy(d => d.TypeIndex).Select(d => new { Catagory = d.Catagory, Type = d.InputType, Name = d.InputType, SortIndex = d.TypeIndex }).Distinct().ToList();

            //虚拟项目结构树，项目可以查看专业设计输入
            List<Dictionary<string, string>> CatagoryList = new List<Dictionary<string, string>>();
            var project = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var majorList = JsonHelper.ToList(project.Major);
            CatagoryList.Add(new Dictionary<string, string>() { { "Name", "项目资料" }, { "Catagory", "Project" } });
            foreach (var item in majorList)
                CatagoryList.Add(new Dictionary<string, string>() { { "Name", item.GetValue("Name") }, { "Catagory", item.GetValue("Value") } });
            var n = 0;
            foreach (var Catagory in CatagoryList)
            {
                var cg = new Dictionary<string, object>();
                cg.SetValue("ID", Catagory.GetValue("Catagory"));
                cg.SetValue("InfoName", Catagory.GetValue("Name"));
                cg.SetValue("Type", "InfoType");
                cg.SetValue("Catagory", Catagory.GetValue("Catagory"));
                cg.SetValue("InputType", "");
                cg.SetValue("SortIndex", n++);
                cg.SetValue("TypeIndex", n++);
                cg.SetValue("ParentID", "");
                cg.SetValue("FileCount", "");
                cg.SetValue("LastUploadDate", "");
                result.Add(cg);
                foreach (var item in typeList.Where(a => a.Catagory == Catagory.GetValue("Catagory")))
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("ID", item.Catagory + item.Type);
                    dic.SetValue("InfoName", item.Name);
                    dic.SetValue("Type", "InfoType");
                    dic.SetValue("Catagory", item.Type);
                    dic.SetValue("InputType", item.Type);
                    dic.SetValue("SortIndex", item.SortIndex);
                    dic.SetValue("TypeIndex", item.SortIndex);
                    dic.SetValue("ParentID", Catagory.GetValue("Catagory"));
                    dic.SetValue("FileCount", "");
                    dic.SetValue("LastUploadDate", "");
                    result.Add(dic);
                    var inputInfoList = list.Where(d => d.InputType == item.Type && d.Catagory == Catagory.GetValue("Catagory")).OrderBy(d => d.SortIndex).ToList();
                    foreach (var inputInfo in inputInfoList)
                    {
                        var infoDic = new Dictionary<string, object>();
                        infoDic.SetValue("ID", inputInfo.ID);
                        infoDic.SetValue("InfoName", inputInfo.InfoName);
                        infoDic.SetValue("Type", "InputInfo");
                        infoDic.SetValue("Catagory", inputInfo.Catagory);
                        infoDic.SetValue("InputType", inputInfo.InputType);
                        infoDic.SetValue("SortIndex", inputInfo.SortIndex);
                        infoDic.SetValue("TypeIndex", inputInfo.InputType);
                        infoDic.SetValue("ParentID", item.Catagory + item.Type);
                        infoDic.SetValue("FileCount", inputInfo.FileCount);
                        infoDic.SetValue("LastUploadDate", inputInfo.LastUploadDate);
                        result.Add(infoDic);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult GetMajorList(QueryBuilder qb)
        {
            var spaceCode = this.GetQueryString("SpaceCode");
            qb.PageSize = 0;
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            qb.Add("ProjectInfoID", QueryMethod.Equal, projectInfoID);
            qb.Add("Catagory", QueryMethod.Equal, spaceCode);
            qb.SortField = "InputTypeIndex,SortIndex"; qb.SortOrder = "asc,asc";
            var result = new List<Dictionary<string, object>>();
            var list = this.entities.Set<S_D_Input>().Include("S_D_InputDocument").Where(qb).Select(
                d => new
                {
                    ID = d.ID,
                    InfoName = d.InfoName,
                    Catagory = d.Catagory,
                    InputType = d.InputType,
                    SortIndex = d.SortIndex,
                    TypeIndex = d.InputTypeIndex,
                    FileCount = d.S_D_InputDocument.Count,
                    LastUploadDate = d.S_D_InputDocument.Max(p => p.CreateDate)
                }).ToList();

            var typeList = list.OrderBy(d => d.TypeIndex).Select(d => new { Type = d.InputType, Name = d.InputType, SortIndex = d.TypeIndex }).Distinct().ToList();
            foreach (var item in typeList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("ID", item.Type);
                dic.SetValue("InfoName", item.Name);
                dic.SetValue("Type", "InfoType");
                dic.SetValue("Catagory", item.Type);
                dic.SetValue("InputType", item.Type);
                dic.SetValue("SortIndex", item.SortIndex);
                dic.SetValue("TypeIndex", item.SortIndex);
                dic.SetValue("ParentID", "");
                dic.SetValue("FileCount", "");
                dic.SetValue("LastUploadDate", "");
                result.Add(dic);
                var inputInfoList = list.Where(d => d.InputType == item.Type).OrderBy(d => d.SortIndex).ToList();
                foreach (var inputInfo in inputInfoList)
                {
                    var infoDic = new Dictionary<string, object>();
                    infoDic.SetValue("ID", inputInfo.ID);
                    infoDic.SetValue("InfoName", inputInfo.InfoName);
                    infoDic.SetValue("Type", "InputInfo");
                    infoDic.SetValue("Catagory", inputInfo.Catagory);
                    infoDic.SetValue("InputType", inputInfo.InputType);
                    infoDic.SetValue("SortIndex", inputInfo.SortIndex);
                    infoDic.SetValue("TypeIndex", inputInfo.InputType);
                    infoDic.SetValue("ParentID", item.Type);
                    infoDic.SetValue("FileCount", inputInfo.FileCount);
                    infoDic.SetValue("LastUploadDate", inputInfo.LastUploadDate);
                    result.Add(infoDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetDocument(QueryBuilder qb, string InputID, string Catagory = "Project")
        {
            qb.PageSize = 0;
            var data = this.entities.Set<S_D_InputDocument>().Where(d => d.InputID == InputID && d.Catagory == Catagory).Where(qb).ToList();
            return Json(data);
        }

        public JsonResult DeleteDoc(string ID)
        {
            var doc = this.GetEntityByID<S_D_InputDocument>(ID);
            if (doc != null)
            {
                if (doc.AuditState == CommonConst.inAuditState || doc.AuditState == CommonConst.finishAuditState)
                    throw new Formula.Exceptions.BusinessException("评审中或者评审过得资料不能删除");
                this.entities.Set<S_D_InputDocument>().Remove(doc);
            }

            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddDocument(string FileID, string InputID, string Catagory = "Project")
        {
            foreach (var item in FileID.Split(','))
            {
                var input = this.GetEntityByID<S_D_Input>(InputID);
                if (input == null) throw new Formula.Exceptions.BusinessException("");
                var doc = new S_D_InputDocument();
                var name = item.Substring(FileID.IndexOf("_") + 1, item.LastIndexOf(".") - item.IndexOf("_") - 1);
                doc.ID = FormulaHelper.CreateGuid();
                doc.Name = name;
                doc.Files = item;
                doc.CreateDate = DateTime.Now;
                doc.CreateUser = this.CurrentUserInfo.UserName;
                doc.CreateUserID = this.CurrentUserInfo.UserID;
                doc.Catagory = Catagory;
                doc.AuditState = CommonConst.unAuditState;
                input.S_D_InputDocument.Add(doc);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult InitDesignInput(string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null)
            {
                throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            }
            projectInfo.InitDeisgnInputTemplate();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportTemplateData(string ListData, string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法导入资料");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var name = item.GetValue("Name");
                var inputType = item.GetValue("Class");
                var input = projectInfo.S_D_Input.FirstOrDefault(d => d.InfoName == name && d.InputType == inputType);
                if (input != null) continue;
                input = this.entities.Set<S_D_Input>().Create();
                input.InfoName = name;
                input.InputType = item.GetValue("Class");
                input.ID = FormulaHelper.CreateGuid();
                input.DBSCode = item.GetValue("DocCode");
                input.State = CommonConst.unAuditState;
                input.Catagory = "Project";
                input.DocType = item.GetValue("DocType");
                input.EngineeringInfoID = projectInfo.GroupID;
                input.CreateDate = DateTime.Now;
                input.CreateUser = this.CurrentUserInfo.UserName;
                input.CreateUserID = this.CurrentUserInfo.UserID;
                int? sortIndex = null;
                if (!string.IsNullOrEmpty(item.GetValue("SortIndex")))
                    sortIndex = int.Parse(item.GetValue("SortIndex"));
                input.SortIndex = sortIndex;
                int? inputTypeIndex = null;
                if (!string.IsNullOrEmpty(item.GetValue("InputTypeIndex")))
                    inputTypeIndex = int.Parse(item.GetValue("InputTypeIndex"));
                input.InputTypeIndex = inputTypeIndex;
                projectInfo.S_D_Input.Add(input);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFiles(string InputID, string ListData, string FileField, string NameField, string CodeField, string Catagory)
        {
            var input = this.GetEntityByID(InputID);
            if (input == null) throw new Formula.Exceptions.BusinessException("");
            var fileList = JsonHelper.ToList(ListData);
            foreach (var item in fileList)
            {
                var name = item.GetValue(NameField);
                var documentName = name;

                string code = "";
                if (!string.IsNullOrEmpty(CodeField) && item.Keys.Contains(CodeField))
                    code = item.GetValue(CodeField);
                if (!string.IsNullOrEmpty(code))
                    documentName += "(" + code + ")";
                var fileID = item.GetValue(FileField);
                var doc = input.S_D_InputDocument.FirstOrDefault(d => d.Files == fileID);
                if (doc == null)
                {
                    doc = new S_D_InputDocument();
                    doc.ID = FormulaHelper.CreateGuid();
                    doc.Name = documentName;
                    doc.CreateDate = DateTime.Now;
                    doc.CreateUser = this.CurrentUserInfo.UserName;
                    doc.CreateUserID = this.CurrentUserInfo.UserID;
                    doc.Catagory = String.IsNullOrEmpty(Catagory) ? "Project" : Catagory;
                    doc.Files = item.GetValue(FileField);
                    doc.AuditState = CommonConst.unAuditState;
                    input.S_D_InputDocument.Add(doc);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportInput(string ListData, string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到项目信息，无法导入资料");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var orlInput = this.GetEntityByID(item.GetValue("ID"));
                if (orlInput == null) continue;
                var input = projectInfo.S_D_Input.FirstOrDefault(d => d.InfoName == orlInput.InfoName && d.InputType == orlInput.InputType);
                if (input == null)
                {
                    input = this.entities.Set<S_D_Input>().Create();
                    input.InfoName = orlInput.InfoName;
                    input.InputType = orlInput.InputType;
                    input.ID = FormulaHelper.CreateGuid();
                    input.DBSCode = orlInput.DBSCode;
                    input.Catagory = "Project";
                    input.InputTypeIndex = orlInput.InputTypeIndex;
                    input.SortIndex = orlInput.SortIndex;
                    input.State = CommonConst.unAuditState;
                    input.DocType = orlInput.DocType;
                    input.EngineeringInfoID = projectInfo.GroupID;
                    input.CreateDate = DateTime.Now;
                    input.CreateUser = this.CurrentUserInfo.UserName;
                    input.CreateUserID = this.CurrentUserInfo.UserID;
                    projectInfo.S_D_Input.Add(input);
                }
                foreach (var doc in orlInput.S_D_InputDocument.ToList())
                {
                    var newDoc = new S_D_InputDocument();
                    newDoc.ID = FormulaHelper.CreateGuid();
                    newDoc.Name = doc.Name;
                    newDoc.Files = doc.Files;
                    newDoc.Catagory = "Project";
                    newDoc.CreateDate = DateTime.Now;
                    newDoc.CreateUser = this.CurrentUserInfo.UserName;
                    newDoc.CreateUserID = this.CurrentUserInfo.UserID;
                    newDoc.Attachments = doc.Attachments;
                    input.S_D_InputDocument.Add(newDoc);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
