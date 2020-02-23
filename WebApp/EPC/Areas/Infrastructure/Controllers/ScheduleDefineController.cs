using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EPC.Logic.Domain;
using MvcAdapter;
using Formula.Helper;
using Config.Logic;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class ScheduleDefineController : InfrastructureController<S_C_ScheduleDefine>
    {
        public override ActionResult Edit()
        {
            var list = new List<Dictionary<string, object>>();
            string modeID = this.GetQueryString("ModeID");
            string ID = this.GetQueryString("ID");
            var mode = this.GetEntityByID<S_C_Mode>(modeID);
            if (mode != null)
            {
                var defineList = mode.S_C_ScheduleDefine.Where(c => c.ID != ID).OrderBy(c => c.SortIndex).ToList();
                foreach (var item in defineList)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("text", item.Name);
                    dic.SetValue("value", item.Code);
                    list.Add(dic);
                }
            }
            ViewBag.ScheduleList = JsonHelper.ToJson(list);
            return View();
        }

        public ActionResult ExtendFieldConfig()
        {
            var ID = this.GetQueryString("ID");
            ViewBag.ID = ID;
            return View();
        }

        public JsonResult GetDefineList(QueryBuilder qb, string ModeID)
        {
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var data = this.entities.Set<S_C_ScheduleDefine>().Where(c => c.ModeID == ModeID).Where(qb).ToList();
            return Json(data);
        }

        protected override void BeforeSave(S_C_ScheduleDefine entity, bool isNew)
        {
            if (isNew)
            {
                var mode = this.GetEntityByID<S_C_Mode>(entity.ModeID);
                var wbsStructRoot = mode.S_C_WBSStruct.FirstOrDefault(c => String.IsNullOrEmpty(c.ParentID));
                foreach (var item in mode.S_C_WBSStruct.OrderBy(c => c.FullID).ToList())
                {
                    var node = new S_C_ScheduleDefine_Nodes();
                    node.StructInfoID = item.ID;
                    node.ID = Formula.FormulaHelper.CreateGuid();
                    node.Name = item.Name;
                    node.NodeType = item.NodeType;
                    node.SortIndex = item.SortIndex;
                    node.FullID = item.FullID;
                    node.ParentID = item.ParentID;
                    node.StructInfoID = item.ID;
                    node.Code = item.Code;
                    node.TaskType = item.TaskType;
                    node.IsDynmanic = item.IsDynmanic;
                    node.IsEnum = item.IsEnum;
                    node.Visible = "1";
                    entity.S_C_ScheduleDefine_Nodes.Add(node);
                }
                entity.ImportProject = false;
                entity.ImportTaskTemplate = false;
                entity.ImportBid = false;
                entity.ImportBOM = false;
                entity.ImportQBS = false;
                entity.ImportExcel = false;
            }
            entity.SetAttrColDefine();
        }

        public JsonResult GetDefineNodes(string DefineID)
        {
            var data = this.entities.Set<S_C_ScheduleDefine_Nodes>().Where(c => c.DefineID == DefineID).OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult GetStaticTreeList(string DefineID)
        {
            var data = this.entities.Set<S_C_ScheduleDefine_Nodes>().Where(c => c.DefineID == DefineID
                && c.IsDynmanic == "0").OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult AddDefineNodes(string NodeID, string ParentID)
        {
            var node = this.GetEntityByID<S_C_ScheduleDefine_Nodes>(ParentID);
            if (node == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到父节点，无法新增节点");
            }
            var structNode = this.GetEntityByID<S_C_WBSStruct>(NodeID);
            if (structNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到WBS结构定义节点");
            }

            var defineNode = new S_C_ScheduleDefine_Nodes();
            defineNode.ID = Formula.FormulaHelper.CreateGuid();
            defineNode.NodeType = structNode.NodeType;
            defineNode.Name = structNode.Name;
            defineNode.Code = structNode.Code;
            defineNode.FullID = structNode.FullID;
            defineNode.StructInfoID = structNode.ID;
            defineNode.ParentID = structNode.ParentID;
            defineNode.TaskType = structNode.TaskType;
            defineNode.SortIndex = structNode.SortIndex;
            defineNode.IsDynmanic = structNode.IsDynmanic;
            defineNode.IsEnum = structNode.IsEnum;
            defineNode.DefineID = node.DefineID;
            defineNode.Visible = "1";
            this.entities.Set<S_C_ScheduleDefine_Nodes>().Add(defineNode);
            foreach (var item in structNode.AllChildren)
            {
                var child = new S_C_ScheduleDefine_Nodes();
                child.ID = Formula.FormulaHelper.CreateGuid();
                child.NodeType = item.NodeType;
                child.Name = item.Name;
                child.Code = item.Code;
                child.FullID = item.FullID;
                child.StructInfoID = item.ID;
                child.ParentID = item.ParentID;
                child.TaskType = item.TaskType;
                child.SortIndex = item.SortIndex;
                child.IsDynmanic = item.IsDynmanic;
                child.IsEnum = item.IsEnum;
                child.DefineID = node.DefineID;
                child.Visible = "1";
                this.entities.Set<S_C_ScheduleDefine_Nodes>().Add(child);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveNode(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("NodeType") == "Root")
                {
                    throw new Formula.Exceptions.BusinessValidationException("不能删除根节点");
                }
                var fullID = item.GetValue("FullID");
                var defineID = item.GetValue("DefineID");
                this.entities.Set<S_C_ScheduleDefine_Nodes>().Delete(c => c.FullID.StartsWith(fullID) && c.DefineID == defineID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetNode(string NodeID, string Data)
        {
            var node = this.GetEntityByID<S_C_ScheduleDefine_Nodes>(NodeID);
            if (node != null)
            {
                var dic = JsonHelper.ToObject(Data);
                node.CanAdd = String.IsNullOrEmpty(dic.GetValue("CanAdd")) ? "0" : dic.GetValue("CanAdd");
                node.CanDelete = String.IsNullOrEmpty(dic.GetValue("CanDelete")) ? "0" : dic.GetValue("CanDelete");
                node.CanEdit = String.IsNullOrEmpty(dic.GetValue("CanEdit")) ? "0" : dic.GetValue("CanEdit");
                node.Visible = String.IsNullOrEmpty(dic.GetValue("Visible")) ? "0" : dic.GetValue("Visible");
                node.IsLocked = String.IsNullOrEmpty(dic.GetValue("IsLocked")) ? "0" : dic.GetValue("IsLocked");
                node.RelateStructNodeID = dic.GetValue("RelateStructNodeID");
                node.RelateStructNodeName = dic.GetValue("RelateStructNodeName");
                node.RelateType = dic.GetValue("RelateType");
                this.entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult SaveColDefine(string ID, string ListData)
        {
            var define = this.GetEntityByID(ID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划，无法保存");
            define.ColDefine = ListData;
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetColDefList(string ID)
        {
            var define = this.GetEntityByID(ID);
            if (String.IsNullOrEmpty(define.ColDefine)) return Json("");
            var list = JsonHelper.ToList(define.ColDefine);
            list = list.OrderBy(c => Convert.ToInt32(c["sortIndex"])).ToList();
            return Json(list);
        }

        public JsonResult GetDefineID(string DefineID)
        {
            var define = this.entities.Set<S_C_ScheduleDefine>().Find(DefineID);
            if (define == null)
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划定义");
            if (String.IsNullOrEmpty(define.PreScheduleCode))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有依赖计划，无法进行关联绑定");
            }
            var preDefine = define.S_C_Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == define.PreScheduleCode);
            if (preDefine == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到编号为【" + define.PreScheduleCode + "】的计划，无法进行关联绑定");
            }
            return Json(new { ID = preDefine.ID });
        }

        public JsonResult GetExtendFieldDefineInfo(string ID)
        {
            var list = new List<Dictionary<string, object>>();
            var node = this.GetEntityByID<S_C_ScheduleDefine_Nodes>(ID);
            if (node == null)
            {
                throw new Formula.Exceptions.BusinessException("没有找到相关的节点数据");
            }
            if (!String.IsNullOrEmpty(node.S_C_ScheduleDefine.AttrDefine))
            {
                list = JsonHelper.ToList(node.S_C_ScheduleDefine.AttrDefine);
                if (!String.IsNullOrEmpty(node.ExtendFieldConfig))
                {
                    var configList = JsonHelper.ToList(node.ExtendFieldConfig);
                    foreach (var item in list)
                    {
                        var config = configList.FirstOrDefault(c => c.GetValue("AttrField") == item.GetValue("AttrField"));
                        item.SetValue("BindingNode", config.GetValue("BindingNode"));
                        item.SetValue("BindingNodeName", config.GetValue("BindingNodeName"));
                        item.SetValue("BindingField", config.GetValue("BindingField"));
                        item.SetValue("BindingNodeType", config.GetValue("BindingNodeType"));
                        item.SetValue("SortIndex", config.GetValue("SortIndex"));
                    }
                }
            }
            return Json(list);
        }

        public JsonResult GetPreScheduleTree(string NodeID)
        {
            var node = this.GetEntityByID<S_C_ScheduleDefine_Nodes>(NodeID);
            if (node == null)
            {
                throw new Formula.Exceptions.BusinessException("没有找到相关的节点数据");
            }
            string preCode = node.S_C_ScheduleDefine.PreScheduleCode;
            var preScheduleDefine =
                this.entities.Set<S_C_ScheduleDefine>().FirstOrDefault(c => c.ModeID == node.S_C_ScheduleDefine.ModeID && c.Code == preCode);
            var list = preScheduleDefine.S_C_ScheduleDefine_Nodes.ToList();
            return Json(list);
        }

        public JsonResult SaveExtendFieldConfig(string NodeID,string ListData)
        {
            var node = this.GetEntityByID<S_C_ScheduleDefine_Nodes>(NodeID);
            if (node == null)
            {
                throw new Formula.Exceptions.BusinessException("没有找到相关的节点数据");
            }
            node.ExtendFieldConfig = ListData;
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
