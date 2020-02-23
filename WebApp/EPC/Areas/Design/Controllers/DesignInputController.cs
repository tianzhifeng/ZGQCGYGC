using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using System.Configuration;

namespace EPC.Areas.Design.Controllers
{
    public class DesignInputController : EPCController<S_E_DesignInput>
    {

        public ActionResult BusinessDocSelector()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            ViewBag.EngineeringInfo = engineeringInfo.ID;
            return View();
        }

        public override ActionResult List()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("找不到指定的项目信息，请确认ID为【" + engineeringInfoID + "】项目是否存在");
            ViewBag.EngineeringInfo = JsonHelper.ToJson(engineeringInfo);
            ViewBag.EngineeringInfoID = engineeringInfo.ID;
            ViewBag.SpaceCode = "Engineering";
            ViewBag.AuthType = this.GetQueryString("AuthType");
            ViewBag.IsEngineeringMode = ConfigurationManager.AppSettings["EngineeringMode"].ToLower();
            return View();
        }

        protected override void AfterGetMode(S_E_DesignInput entity, bool isNew)
        {
            if (isNew)
            {
                entity.Catagory = "Engineering";
            }
        }

        protected override void BeforeDelete(List<S_E_DesignInput> entityList)
        {
            var catagory = "Engineering";
            foreach (var item in entityList)
            {
                if (item.Catagory != catagory) throw new Formula.Exceptions.BusinessValidationException("不能删除除本专业以外的设计输入资料");
                if (item.S_E_DesignInput_Document.Count(d => d.AuditState == CommonConst.finishAuditState 
                    || d.AuditState == CommonConst.inAuditState) > 0)
                    throw new Formula.Exceptions.BusinessValidationException("【" + item.InfoName + "】已经发起评审或通过评审，不能进行删除操作");
            }
        }

        protected override void BeforeSave(S_E_DesignInput entity, bool isNew)
        {
            if (isNew)
            {
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(entity.EngineeringInfoID);
                if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程，无法增加设计输入");
                entity.State = CommonConst.unAuditState;
                var maxSortIndex = this.entities.Set<S_E_DesignInput>().Where(d => d.EngineeringInfoID == engineeringInfo.ID).Max(d => d.SortIndex);
                if (maxSortIndex.HasValue)
                    entity.SortIndex = maxSortIndex.Value + 100;
                else
                    entity.SortIndex = 0;
            }
        }

        public JsonResult GetTreeList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            qb.Add("EngineeringInfoID", QueryMethod.Equal, engineeringInfoID);
            qb.Add("Catagory", QueryMethod.Equal, "Engineering");
            qb.SortField = "InputTypeIndex,SortIndex"; qb.SortOrder = "asc,asc";
            var result = new List<Dictionary<string, object>>();
            var list = this.entities.Set<S_E_DesignInput>().Include("S_E_DesignInput_Document").Where(qb).Select(
                d => new
                {
                    ID = d.ID,
                    InfoName = d.InfoName,
                    Catagory = d.Catagory,
                    InputType = d.InputType,
                    SortIndex = d.SortIndex,
                    TypeIndex = d.InputTypeIndex,
                    FileCount = d.S_E_DesignInput_Document.Count,
                    LastUploadDate = d.S_E_DesignInput_Document.Max(p => p.CreateDate)
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

        public JsonResult GetDocument(QueryBuilder qb, string InputID, string Catagory = "Engineering")
        {
            qb.PageSize = 0;
            var data = this.entities.Set<S_E_DesignInput_Document>().Where(d => d.InputID == InputID && d.Catagory == Catagory).Where(qb).ToList();
            return Json(data);
        }

        public JsonResult DeleteDoc(string ID)
        {
            var doc = this.GetEntityByID<S_E_DesignInput_Document>(ID);
            if (doc != null)
            {
                if (doc.AuditState == CommonConst.inAuditState || doc.AuditState == CommonConst.finishAuditState)
                    throw new Formula.Exceptions.BusinessValidationException("评审中或者评审过得资料不能删除");
                this.entities.Set<S_E_DesignInput_Document>().Remove(doc);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddDocument(string FileID, string InputID, string Catagory = "Engineering")
        {
            foreach (var item in FileID.Split(','))
            {
                var input = this.GetEntityByID<S_E_DesignInput>(InputID);
                if (input == null) throw new Formula.Exceptions.BusinessValidationException("");
                var doc = new S_E_DesignInput_Document();
                var name = item.Substring(FileID.IndexOf("_") + 1, item.LastIndexOf(".") - item.IndexOf("_") - 1);
                doc.ID = FormulaHelper.CreateGuid();
                doc.Name = name;
                doc.Files = item;
                doc.CreateDate = DateTime.Now;
                doc.CreateUser = this.CurrentUserInfo.UserName;
                doc.CreateUserID = this.CurrentUserInfo.UserID;
                doc.Catagory = Catagory;
                doc.AuditState = CommonConst.unAuditState;
                input.S_E_DesignInput_Document.Add(doc);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult InitDesignInput(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            }
            engineeringInfo.InitDeisgnInputTemplate();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportTemplateData(string ListData, string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到项目信息，无法导入资料");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var name = item.GetValue("Name");
                var inputType = item.GetValue("Class");
                var input = engineeringInfo.S_E_DesignInput.FirstOrDefault(d => d.InfoName == name && d.InputType == inputType);
                if (input != null) continue;
                input = this.entities.Set<S_E_DesignInput>().Create();
                input.InfoName = name;
                input.InputType = item.GetValue("Class");
                input.ID = FormulaHelper.CreateGuid();
                input.DBSCode = item.GetValue("DocCode");
                input.State = CommonConst.unAuditState;
                input.Catagory = "Engineering";
                input.DocType = item.GetValue("DocType");
                input.EngineeringInfoID = engineeringInfo.ID;
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
                engineeringInfo.S_E_DesignInput.Add(input);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFiles(string InputID, string ListData, string FileField, string NameField, string CodeField, string Catagory)
        {
            var input = this.GetEntityByID(InputID);
            if (input == null) throw new Formula.Exceptions.BusinessValidationException("");
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
                var doc = input.S_E_DesignInput_Document.FirstOrDefault(d => d.Files == fileID);
                if (doc == null)
                {
                    doc = new S_E_DesignInput_Document();
                    doc.ID = FormulaHelper.CreateGuid();
                    doc.Name = documentName;
                    doc.CreateDate = DateTime.Now;
                    doc.CreateUser = this.CurrentUserInfo.UserName;
                    doc.CreateUserID = this.CurrentUserInfo.UserID;
                    doc.Catagory = String.IsNullOrEmpty(Catagory) ? "Engineering" : Catagory;
                    doc.Files = item.GetValue(FileField);
                    doc.AuditState = CommonConst.unAuditState;
                    input.S_E_DesignInput_Document.Add(doc);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportInput(string ListData, string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到项目信息，无法导入资料");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var orlInput = this.GetEntityByID(item.GetValue("ID"));
                if (orlInput == null) continue;
                var input = engineeringInfo.S_E_DesignInput.FirstOrDefault(d => d.InfoName == orlInput.InfoName && d.InputType == orlInput.InputType);
                if (input == null)
                {
                    input = this.entities.Set<S_E_DesignInput>().Create();
                    input.InfoName = orlInput.InfoName;
                    input.InputType = orlInput.InputType;
                    input.ID = FormulaHelper.CreateGuid();
                    input.DBSCode = orlInput.DBSCode;
                    input.Catagory = "Project";
                    input.InputTypeIndex = orlInput.InputTypeIndex;
                    input.SortIndex = orlInput.SortIndex;
                    input.State = CommonConst.unAuditState;
                    input.DocType = orlInput.DocType;
                    input.EngineeringInfoID = engineeringInfo.ID;
                    input.CreateDate = DateTime.Now;
                    input.CreateUser = this.CurrentUserInfo.UserName;
                    input.CreateUserID = this.CurrentUserInfo.UserID;
                    engineeringInfo.S_E_DesignInput.Add(input);
                }
                foreach (var doc in orlInput.S_E_DesignInput_Document.ToList())
                {
                    var newDoc = new S_E_DesignInput_Document();
                    newDoc.ID = FormulaHelper.CreateGuid();
                    newDoc.Name = doc.Name;
                    newDoc.Files = doc.Files;
                    newDoc.Catagory = "Project";
                    newDoc.CreateDate = DateTime.Now;
                    newDoc.CreateUser = this.CurrentUserInfo.UserName;
                    newDoc.CreateUserID = this.CurrentUserInfo.UserID;
                    newDoc.Attachments = doc.Attachments;
                    input.S_E_DesignInput_Document.Add(newDoc);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }
    
    }
}
