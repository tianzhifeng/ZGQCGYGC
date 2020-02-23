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
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;
using Formula.Helper;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class MileStoneController : BaseConfigController<S_T_MileStone>
    {
        public override ActionResult List()
        {
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.MileStoneLevel = level;           

            var enumDt = EnumBaseHelper.GetEnumTable("Project.Major");
            ViewBag.Major = JsonHelper.ToJson(enumDt);
            var row =  enumDt.NewRow();
            row["value"] = "All";
            row["text"] = "所有";
            enumDt.Rows.InsertAt(row,0);
            ViewBag.MajorWithAll = JsonHelper.ToJson(enumDt);

            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("Project.Phase", "阶段", "PhaseValue");
            deptCategory.Multi = false;
            deptCategory.SetDefaultItem();
            tab.Categories.Add(deptCategory);

            var projectClassCategory = CategoryFactory.GetCategory("Base.ProjectClass", "业务类型", "ProjectClass");
            projectClassCategory.Multi = false;
            projectClassCategory.SetDefaultItem();
            tab.Categories.Add(projectClassCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public override JsonResult Delete()
        {
            var list = this.GetRequestList("List");
            foreach (var item in list)
            {
                var ID = item.GetValue("ID");
                if (String.IsNullOrEmpty(ID))
                    continue;
                this.entities.Set<S_T_MileStone>().Delete(d => d.ID == ID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public override JsonResult SaveList()
        {
            var listData = Request["ListData"];
            var list = UpdateList<S_T_MileStone>(listData);
            foreach (var item in list)
            {
                if (item.MileStoneType == MileStoneType.Cooperation.ToString())
                {
                    if (String.IsNullOrEmpty(item.OutMajors) || String.IsNullOrEmpty(item.InMajors))
                        throw new Formula.Exceptions.BusinessException("【" + item.MileStoneName + "】类型为提资计划，必须指定提出专业和接收专业");
                    else if (item.OutMajors == "All" || item.OutMajors.Split(',').Length > 1)
                    {
                        if (item.InMajors.Split(',').Length > 1 || item.InMajors == "All")
                        {
                            throw new Formula.Exceptions.BusinessException("【" + item.MileStoneName + "】当提出专业为所有专业或多专业时，接收专业只能是一个专业");
                        }
                    }
                    else if ((item.OutMajors.Split(',').Contains("All") && item.OutMajors.Split(',').Length > 1))
                    {
                        throw new Formula.Exceptions.BusinessException("【" + item.MileStoneName + "】提出专业勾选所有专业时，不能再勾选任意其它专业");
                    }
                    else if (item.InMajors.Split(',').Contains("All") && item.InMajors.Split(',').Length > 1)
                    {
                        throw new Formula.Exceptions.BusinessException("【" + item.MileStoneName + "】接收专业勾选所有专业时，不能再勾选任意其它专业");
                    }
                }
                if (entities.Set<S_T_MileStone>().Count(d => d.ModeID == item.ModeID && d.PhaseValue == item.PhaseValue
                    && d.MileStoneName == item.MileStoneName && d.ID != item.ID) > 0
                    || list.Count(d => d.ModeID == item.ModeID && d.PhaseValue == item.PhaseValue
                    && d.MileStoneName == item.MileStoneName && d.ID != item.ID) > 0)
                {
                    throw new Formula.Exceptions.BusinessException("【" + item.MileStoneName + "】在同一阶段下，里程碑名称不能重复，请修改里程碑名称");
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.GetList(qb);
        }
    }
}
