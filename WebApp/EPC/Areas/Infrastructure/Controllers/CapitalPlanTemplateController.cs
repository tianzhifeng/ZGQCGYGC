using EPC.Logic;
using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class CapitalPlanTemplateController : InfrastructureController<S_F_CapitalPlanTemplate>
    {
        protected override void BeforeDelete(List<S_F_CapitalPlanTemplate> entityList)
        {
            base.BeforeDelete(entityList);

            if (entityList.Count <= 0)
                return;

            var entity = entityList[0];
            entities.Set<S_F_CapitalPlanTemplate_Detail>().Delete(a => a.S_F_CapitalPlanTemplateID == entity.ID);
        }

        public ActionResult TreeList()
        {
            string id = GetQueryString("ID");
            S_F_CapitalPlanTemplate planTemplate = entities.Set<S_F_CapitalPlanTemplate>().Find(id);
            if (planTemplate == null)
                throw new Exception("未找到id为【" + id + "】的S_F_CapitalPlanTemplate");

            //如果没有任何节点,插入CapitalPlanType枚举值对应的节点
            if (entities.Set<S_F_CapitalPlanTemplate_Detail>().Count(a => a.S_F_CapitalPlanTemplateID == id) == 0)
            {
                //初始
                {
                    S_F_CapitalPlanTemplate_Detail template = new S_F_CapitalPlanTemplate_Detail();
                    template.ID = FormulaHelper.CreateGuid();
                    template.FullID = template.ID;
                    template.S_F_CapitalPlanTemplateID = id;
                    template.ParentID = "";
                    template.Name = "初期合计";
                    template.IsReadOnly = true;
                    template.CapitalPlanType = CapitalPlanType.Initial.ToString();
                    template.CreateDate = DateTime.Now;
                    template.CreateUserID = CurrentUserInfo.UserID;
                    template.CreateUserName = CurrentUserInfo.UserName;
                    template.SortIndex = 0;
                    entities.Set<S_F_CapitalPlanTemplate_Detail>().Add(template);
                }
                //流入
                {
                    S_F_CapitalPlanTemplate_Detail template = new S_F_CapitalPlanTemplate_Detail();
                    template.ID = FormulaHelper.CreateGuid();
                    template.FullID = template.ID;
                    template.S_F_CapitalPlanTemplateID = id;
                    template.ParentID = "";
                    template.Name = "经营现金流入";
                    template.IsReadOnly = true;
                    template.CapitalPlanType = CapitalPlanType.In.ToString();
                    template.CreateDate = DateTime.Now;
                    template.CreateUserID = CurrentUserInfo.UserID;
                    template.CreateUserName = CurrentUserInfo.UserName;
                    template.SortIndex = 1;
                    entities.Set<S_F_CapitalPlanTemplate_Detail>().Add(template);
                }
                //支出
                {
                    S_F_CapitalPlanTemplate_Detail template = new S_F_CapitalPlanTemplate_Detail();
                    template.ID = FormulaHelper.CreateGuid();
                    template.FullID = template.ID;
                    template.S_F_CapitalPlanTemplateID = id;
                    template.ParentID = "";
                    template.Name = "经营性现金支出";
                    template.IsReadOnly = true;
                    template.CapitalPlanType = CapitalPlanType.Out.ToString();
                    template.CreateDate = DateTime.Now;
                    template.CreateUserID = CurrentUserInfo.UserID;
                    template.CreateUserName = CurrentUserInfo.UserName;
                    template.SortIndex = 2;
                    entities.Set<S_F_CapitalPlanTemplate_Detail>().Add(template);
                }
                //总计
                {
                    S_F_CapitalPlanTemplate_Detail template = new S_F_CapitalPlanTemplate_Detail();
                    template.ID = FormulaHelper.CreateGuid();
                    template.FullID = template.ID;
                    template.S_F_CapitalPlanTemplateID = id;
                    template.ParentID = "";
                    template.Name = "现金余缺";
                    template.IsReadOnly = true;
                    template.CapitalPlanType = CapitalPlanType.Total.ToString();
                    template.CreateDate = DateTime.Now;
                    template.CreateUserID = CurrentUserInfo.UserID;
                    template.CreateUserName = CurrentUserInfo.UserName;
                    template.SortIndex = 3;
                    entities.Set<S_F_CapitalPlanTemplate_Detail>().Add(template);
                }


                entities.SaveChanges();
            }

            //供选择的不包含CapitalPlanType.Total
            var dt = EnumBaseHelper.GetEnumTable(typeof(CapitalPlanType));
            //DataRow dr = dt.Select("value='" + CapitalPlanType.Total.ToString() + "'")[0];
            //dt.Rows.Remove(dr);
            ViewBag.CapitalPlanType = JsonHelper.ToJson(dt); ;
            return View();
        }

        public JsonResult GetTreeList(string ID)
        {
            return Json(entities.Set<S_F_CapitalPlanTemplate_Detail>()
                .Where(a => a.S_F_CapitalPlanTemplateID == ID).OrderBy(a => a.SortIndex).ToList());
        }

        public JsonResult AddItem(string targetID, string capitalPlanType, bool isSub)
        {
            S_F_CapitalPlanTemplate_Detail target = entities.Set<S_F_CapitalPlanTemplate_Detail>().Find(targetID);
            string templateID = GetQueryString("ID");

            //total不能添加子项
            if (target.CapitalPlanType == CapitalPlanType.Total.ToString() && isSub)
            {
                throw new Formula.Exceptions.BusinessValidationException("不能插入子项");
            }

            S_F_CapitalPlanTemplate_Detail template = new S_F_CapitalPlanTemplate_Detail();
            template.ID = FormulaHelper.CreateGuid();

            //插入子项
            if (isSub)
            {
                template.ParentID = targetID;
                template.FullID = string.IsNullOrEmpty(targetID) ?
                    template.ID : (target.FullID + "," + template.ID);
                template.SortIndex = GetMaxOrderIndex() + 1;
            }
            //插入同项
            else
            {
                template.ParentID = target.ParentID;
                template.FullID = target.FullID.Replace(target.ID, template.ID);

                S_F_CapitalPlanTemplate_Detail targetNext = entities.Set<S_F_CapitalPlanTemplate_Detail>()
                    .Where(a => a.ParentID == target.ParentID && a.SortIndex > target.SortIndex)
                    .OrderBy(a => a.SortIndex).FirstOrDefault();

                if (targetNext != null)
                {
                    template.SortIndex = (targetNext.SortIndex + target.SortIndex) / 2;
                }
                else
                {
                    template.SortIndex = target.SortIndex + 1;
                }
            }

            template.S_F_CapitalPlanTemplateID = templateID;

            template.Name = "新增项";
            template.IsReadOnly = false;
            template.CapitalPlanType = capitalPlanType;
            template.CreateDate = DateTime.Now;
            template.CreateUserID = CurrentUserInfo.UserID;
            template.CreateUserName = CurrentUserInfo.UserName;
            template._state = "add";

            var res = entities.Set<S_F_CapitalPlanTemplate_Detail>().Add(template);
            entities.SaveChanges();
            return Json(res);
        }
        public JsonResult DeleteItems(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var distinctList = list.Distinct();
            foreach (var item in distinctList)
            {
                string id = item["ID"].ToString();
                //if (item.GetValue("CapitalPlanType") == CapitalPlanType.Total.ToString())
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("不能删除【汇总项】");
                //}
                entities.Set<S_F_CapitalPlanTemplate_Detail>().Delete(a => a.ID == id);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveItems(string ListData, string templateID)
        {
            var list = JsonHelper.ToList(ListData);

            string strTotal = CapitalPlanType.Total.ToString();
            var idList = list.Select(a => a.GetValue("ID"));

            if (list.Count(a => a.GetValue("CapitalPlanType") == strTotal) > 1)
            {
                throw new Exception("只能存在一个类型为【汇总】的项");
            }

            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "remove") continue;

                var detail = this.GetEntityByID<S_F_CapitalPlanTemplate_Detail>(item.GetValue("ID"));
                if (detail != null)
                {

                    if (item.GetValue("CapitalPlanType") == strTotal)
                    {
                        if (entities.Set<S_F_CapitalPlanTemplate_Detail>().Any(a => a.S_F_CapitalPlanTemplateID == detail.S_F_CapitalPlanTemplateID
                            && a.CapitalPlanType == strTotal && !idList.Contains(a.ID)))
                        {
                            throw new Exception("只能存在一个类型为【汇总】的项");
                        }
                    }

                    this.UpdateEntity<S_F_CapitalPlanTemplate_Detail>(detail, item);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveItem(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_F_CapitalPlanTemplate_Detail>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");
            if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_F_CapitalPlanTemplate_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");

                S_F_CapitalPlanTemplate_Detail targetPre = entities.Set<S_F_CapitalPlanTemplate_Detail>()
                    .Where(a => a.ParentID == target.ParentID && a.SortIndex < target.SortIndex)
                    .OrderByDescending(a => a.SortIndex).FirstOrDefault();

                if (targetPre != null)
                {
                    sourceNode.SortIndex = (targetPre.SortIndex + target.SortIndex) / 2;
                }
                else
                {
                    sourceNode.SortIndex = target.SortIndex - 1;
                }

            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_F_CapitalPlanTemplate_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");

                S_F_CapitalPlanTemplate_Detail targetNext = entities.Set<S_F_CapitalPlanTemplate_Detail>()
                    .Where(a => a.ParentID == target.ParentID && a.SortIndex > target.SortIndex)
                    .OrderBy(a => a.SortIndex).FirstOrDefault();

                if (targetNext != null)
                {
                    sourceNode.SortIndex = (targetNext.SortIndex + target.SortIndex) / 2;
                }
                else
                {
                    sourceNode.SortIndex = target.SortIndex + 1;
                }
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        private double GetMaxOrderIndex()
        {
            var last = entities.Set<S_F_CapitalPlanTemplate_Detail>().OrderByDescending(a => a.SortIndex).FirstOrDefault();
            if (last != null)
            {
                return last.SortIndex;
            }
            return 0;
        }
    }
}
