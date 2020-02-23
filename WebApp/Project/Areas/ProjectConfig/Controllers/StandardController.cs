using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;
using Formula;
using MvcAdapter;
using Formula.Helper;
using Config.Logic;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class StandardController : BaseConfigController<S_D_Standard>
    {
        public override JsonResult GetTree()
        {
            var standardList = this.entities.Set<S_D_Standard>().OrderBy(d => d.FullID).ToList();
            if (standardList.Count == 0)
            {
                //没有根目录，新增
                var standardInfo = this.entities.Set<S_D_Standard>().Create();
                standardInfo.ID = FormulaHelper.CreateGuid();
                standardInfo.FullID = standardInfo.ID;
                standardInfo.Type = "Root";
                standardInfo.Name = "标准规范";
                this.entities.Set<S_D_Standard>().Add(standardInfo);
                this.entities.SaveChanges();
                standardList.Add(standardInfo);
            }
            return Json(standardList);
        }

        public override JsonResult Delete()
        {
            string standardID = this.Request["StandardID"];
            var standard = this.entities.Set<S_D_Standard>().FirstOrDefault(d => d.ID == standardID);
            if (standard == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + standardID + "】的标准规范目录，无法删除");
            if (standard.Type == "Root")
                throw new Formula.Exceptions.BusinessException("根目录不可删除！");
            DeleteFolder(standard);
            this.entities.SaveChanges();
            return Json(standard);
        }
        private void DeleteFolder(S_D_Standard standard)
        {
            var standardList = this.entities.Set<S_D_Standard>().Where(d => d.FullID.StartsWith(standard.FullID)).OrderByDescending(d => d.FullID).ToList();
            foreach (var item in standardList)
            {
                ValidateDelete(item.ID);
                this.entities.Set<S_D_Standard>().Delete(d => d.ID == item.ID);
            }
        }

        private void ValidateDelete(string standardID)
        {
            var relateDoc = this.entities.Set<S_D_StandardDocument>().Where(d => d.StandardID == standardID).Count();
            if (relateDoc != 0)
                throw new Formula.Exceptions.BusinessException("文件夹下存在标准规范文件，不可删除！");
        }

        protected override void BeforeSave(S_D_Standard entity, bool isNew)
        {
            if (isNew)
                entity.ParentID = this.GetQueryString("ParentID");

            var parent = this.entities.Set<S_D_Standard>().FirstOrDefault(d => d.ID == entity.ParentID);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + entity.ParentID + "】的标准规范目录，无法增加子目录");

            var standardInfo = this.entities.Set<S_D_Standard>().FirstOrDefault(d => d.ParentID == entity.ParentID && d.Code == entity.Code && d.ID != entity.ID);
            if (standardInfo != null) throw new Formula.Exceptions.BusinessException("目录【" + parent.Name + "】下已存在编号为【" + entity.Code + "】的子目录,请尝试其它编号！");

            if (isNew)
            {
                if (String.IsNullOrEmpty(entity.ID))
                    entity.ID = FormulaHelper.CreateGuid();
                entity.Type = "Folder";
                entity.ParentID = parent.ID;
                entity.FullID = parent.FullID + "." + entity.ID;
                entity.CreateDate = DateTime.Now;
                entity.CreateUser = CurrentUserInfo.UserID;
                entity.CreateUserID = CurrentUserInfo.UserName;
            }
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string standardID = this.Request["StandardID"];
            if (String.IsNullOrEmpty(standardID)) return Json("");
            var data = this.entities.Set<S_D_StandardDocument>().Where(d => d.StandardID == standardID).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult DeleteDocument(string Documents)
        {
            var list = JsonHelper.ToList(Documents);
            foreach (var item in list)
            {
                string documentID = item.GetValue("ID");
                this.entities.Set<S_D_StandardDocument>().Delete(d => d.ID == documentID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
