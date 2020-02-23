using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config.Logic;
using Config;
using Formula;
using Formula.Helper;
using Project.Logic;
using Project.Logic.Domain;
using System.Data;

namespace Project.Areas.BaseData.Controllers
{
    public class WBSTemplateController : BaseConfigController<S_D_WBSTemplate>
    {

        public ActionResult WBSTemplateAddWithAttrDefine()
        {
            string childType = this.Request["Type"];
            ViewBag.NameTitle = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), childType);
            if (String.IsNullOrEmpty(childType)) throw new Formula.Exceptions.BusinessException("未指定需要增加的WBS节点类型，无法增加节点");
            var list = BaseConfigFO.GetWBSEnum(childType);
            ViewBag.DefineAttr = JsonHelper.ToJson(list);
            return this.View();
        }


        protected override void BeforeSave(S_D_WBSTemplate entity, bool isNew)
        {
            entity.Save();
        }

        public override JsonResult GetTree()
        {
            string templateID = this.Request["TemplateID"];
            var data = this.entities.Set<S_D_WBSTemplateNode>().Where(d => d.TemplateID == templateID).ToList();
            return Json(data);
        }

        public JsonResult GetMenu(string ID)
        {
            var wbs = this.GetEntityByID<S_D_WBSTemplateNode>(ID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的WBS对象");
            if (wbs.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("WBS节点未定义类别，无法显示菜单");
            var childNodeCodes = wbs.StructNodeInfo.ChildCode.Split(',');
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var item in childNodeCodes)
            {
                var menuItem = new Dictionary<string, object>();
                var name = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), item);

                menuItem["name"] = item;
                menuItem["text"] = "增加" + name;
                menuItem["iconCls"] = "icon-add";
                menuItem["onClick"] = "addNode";
                var attrDefineList = BaseConfigFO.GetWBSAttrList(item);
                if (attrDefineList.Count > 0)
                    menuItem["attrDefine"] = "true";
                else
                    menuItem["attrDefine"] = "false";
                result.Add(menuItem);
            }
            string json = JsonHelper.ToJson(result);
            return Json(result);
        }

        public JsonResult AddChild(string parentIDs, string Children, string WBSType)
        {
            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_D_WBSTemplateNode>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                var list = JsonHelper.ToList(Children);
                foreach (var item in list)
                {
                    var wbs = new S_D_WBSTemplateNode();
                    this.UpdateEntity<S_D_WBSTemplateNode>(wbs, item);
                    parent.AddChild(wbs);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddChildWithDefAttr(string Children)
        {
            var dic = JsonHelper.ToObject(Children);
            string parentIDs = dic["ParentIDs"].ToString();
            string children = dic["childNodes"].ToString();
            string type = dic["Type"].ToString();
            var list = BaseConfigFO.GetWBSAttrList(type);
            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_D_WBSTemplateNode>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                foreach (var item in children.Split(','))
                {
                    var attrDefine = list.FirstOrDefault(d => d.Code == item);
                    if (attrDefine == null) continue;
                    var wbs = new S_D_WBSTemplateNode();
                    wbs.WBSType = dic["Type"].ToString();
                    wbs.Name = attrDefine.Name;
                    wbs.SortIndex = attrDefine.SortIndex;
                    wbs.WBSValue = item;
                    parent.AddChild(wbs);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }


        public JsonResult DeleteWBS(string WBSInfo)
        {
            var list = JsonHelper.ToList(WBSInfo);
            foreach (var item in list)
            {
                var wbsId = item.GetValue("ID");
                if (String.IsNullOrEmpty(wbsId)) continue;
                var wbs = this.GetEntityByID<S_D_WBSTemplateNode>(wbsId);
                if (wbs == null) continue;
                if (wbs.WBSType == WBSNodeType.Project.ToString()) throw new Formula.Exceptions.BusinessException("WBS根节点不允许删除");
                wbs.Delete();
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveWBS(string WBSInfo)
        {
            this.UpdateList<S_D_WBSTemplateNode>(WBSInfo);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPackage(string DataSource, string TargetID, string TemplateID)
        {
            var templateInfo = this.GetEntityByID<S_D_WBSTemplate>(TemplateID);
            var list = JsonHelper.ToList(DataSource);
            if (templateInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + TemplateID + "】的项目对象，导入工作包失败");
            var majorList = templateInfo.GetMajorList();
            if (majorList.Count == 0) throw new Formula.Exceptions.BusinessException("没有策划专业节点，无法进行工作包导入操作");
            if (String.IsNullOrEmpty(TargetID))
            {
                foreach (var item in list)
                {
                    string majorValue = item.GetValue("MajorCode");
                    var wbslist = majorList.Where(d => d.WBSValue == majorValue).ToList();
                    foreach (var wbs in wbslist)
                    {
                        var task = new S_D_WBSTemplateNode();
                        task.Name = item.GetValue("Name");
                        task.WBSType = WBSNodeType.Work.ToString();
                        task.Code = item.GetValue("Code");
                        if (!String.IsNullOrEmpty(item.GetValue("WorkLoad")))
                            task.WorkLoad = Convert.ToDecimal(item.GetValue("WorkLoad"));
                        wbs.AddChild(task);
                    }
                }
            }
            else
            {
                var wbs = this.GetEntityByID<S_D_WBSTemplateNode>(TargetID);
                if (wbs == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + TargetID + "】的WBS节点，导入工作包失败");
                foreach (var item in list)
                {
                    string majorValue = item.GetValue("MajorCode");
                    var task = new S_D_WBSTemplateNode();
                    task.Name = item.GetValue("Name");
                    task.WBSType = WBSNodeType.Work.ToString();
                    task.Code = item.GetValue("Code");
                    if (!String.IsNullOrEmpty(item.GetValue("WorkLoad")))
                        task.WorkLoad = Convert.ToDecimal(item.GetValue("WorkLoad"));
                    wbs.AddChild(task);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
