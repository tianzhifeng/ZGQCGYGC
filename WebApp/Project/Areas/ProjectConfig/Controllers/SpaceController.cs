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
using Formula.Helper;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class SpaceController : BaseConfigController<S_T_SpaceDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            string ModeID = this.Request["ModeID"];
            var gridData = entities.Set<S_T_SpaceDefine>().Where(d => d.Type == "Root" && d.ModeID == ModeID).WhereToGridData(qb);
            return Json(gridData);
        }

        protected override void BeforeDelete(List<S_T_SpaceDefine> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }

        public override JsonResult GetModel(string id)
        {
            var entity = this.GetEntity<S_T_SpaceDefine>(id);
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.DefineType = SpaceDefineType.Static.ToString();
                entity.DisplayField = "Name";
            }
            return Json(entity);
        }

        public override JsonResult Save()
        {
            var define = this.UpdateEntity<S_T_SpaceDefine>();
            if (define.Type == "Root")
                S_T_SpaceDefine.AddRoot(define);
            else if (String.IsNullOrEmpty(define.FeatureID))
            {
                var parent = this.GetEntityByID<S_T_SpaceDefine>(define.ParentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + define.ParentID + "】的定义节点，无法增加SpaceDefine子节点");
                parent.AddChild(define);
            }
            FormulaHelper.GetEntities<BaseConfigEntities>().SaveChanges();
            return Json(define);
        }

        public JsonResult GetDefineTree(string ID)
        {
            var dbContext = FormulaHelper.GetEntities<BaseConfigEntities>();
            var nodes = dbContext.S_T_SpaceDefine.Where(d => d.FullID.StartsWith(ID)).OrderBy(d => d
                .SortIndex).ToList();
            return Json(nodes);
        }

        public JsonResult AddFeature(string FeatureInfo, string SpaceDefineID)
        {
            if (String.IsNullOrEmpty(FeatureInfo)) return Json("");
            if (String.IsNullOrEmpty(SpaceDefineID)) throw new Formula.Exceptions.BusinessException("必须选择一个SpaceDefine节点，才能附加功能");
            var define = this.entities.Set<S_T_SpaceDefine>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (define == null) throw new Formula.Exceptions.BusinessException("SpaceDefine空间定义对象为空，附加功能失败");
            var list = JsonHelper.ToObject<List<S_D_Feature>>(FeatureInfo);
            foreach (var item in list)
            {
                if (define.Children.Exists(d => d.Type == SpaceNodeType.Feature.ToString() && d.Code == item.Code))
                    continue;
                define.AddFeature(item);
            }
            entities.SaveChanges();
            return Json("");
        }

        public void ImportMode(string modeID, string engineeringLevel)
        {
            var modeInfo = this.GetEntityByID<S_T_ProjectMode>(modeID);
            if (modeInfo == null)
                throw new Formula.Exceptions.BusinessException("该项目模式定义不存在，请确认是否已被删除！");
            //删除该模式下已存在的工程层级
            this.entities.Set<S_T_SpaceDefine>().Delete(d => d.ModeID == modeID);

            var list = JsonHelper.ToList(engineeringLevel);
            foreach (var item in list)
            {
                var row = new S_T_SpaceDefine();
                row.ID = FormulaHelper.CreateGuid();
                row.ModeID = modeID;
                row.Type = SpaceNodeType.Root.ToString();
                row.FullID = row.ID;
                row.Code = item["value"].ToString();
                row.Name = item["text"].ToString();
                row.SortIndex = 0;
                row.DefineType = SpaceDefineType.Static.ToString();
                row.IsDisplay = "是";
                row.DisplayField = "Name";
                this.entities.Set<S_T_SpaceDefine>().Add(row);
            }
            this.entities.SaveChanges();
        }
    }
}
