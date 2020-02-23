using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using OfficeAuto.Logic;
using OfficeAuto.Logic.Domain;
using Formula;
using System.Collections.Generic;

namespace OfficeAuto.Areas.DocumentCabinets.Controllers
{
    public class CabinetsController : BaseController<S_F_DocumentInfo>
    {
        public override ActionResult Tree()
        {
            var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault();
            if (entity == null)
            {
                //初始化，新建顶级节点
                entity = GetEntity<S_F_DocumentInfo>(null);
                entity.FullPathID = entity.ID;
                entity.Name = "资料柜";
                entity.Code = "0001";
                entity.SortIndex = 0;
                entity.Level = 0;
                entity.IsInherit = 0;
                entity.IsValid = "T";
                entity.IsDeleted = 0;
                entities.Set<S_F_DocumentInfo>().Add(entity);
                entities.SaveChanges();
            }
            return base.Tree();
        }

        public ActionResult ViewTree()
        {
            return View();
        }

        protected override void EntityCreateLogic<TEntity>(TEntity entity)
        {
            base.EntityCreateLogic(entity);
            if (entity is S_F_DocumentInfo)
            {
                var level = GetQueryString("Level");
                var parentId = GetQueryString("ParentID");
                var fullPathId = GetQueryString("FullPathID");
                var sortIndex = entities.Set<S_F_DocumentInfo>().Where(i => i.ParentID == parentId).Max(i => i.SortIndex);
                var obj = entity as S_F_DocumentInfo;
                obj.ParentID = parentId;
                obj.FullPathID = fullPathId + "." + obj.ID;
                obj.Level = String.IsNullOrWhiteSpace(level) ? 1 : (Convert.ToInt32(level) + 1);
                obj.SortIndex = sortIndex == null ? 0 : sortIndex + 1;
                obj.IsInherit = 1;
                obj.IsValid = "T";
                obj.IsDeleted = 0;
            }
        }

        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_F_DocumentInfo>();
            //判断同级不能重名
            if (entities.Set<S_F_DocumentInfo>().Any(a => a.ParentID == entity.ParentID && a.Name == entity.Name && a.ID != entity.ID))
                throw new Formula.Exceptions.BusinessValidationException("同层节点下名称不能重复");

            var funcType = GetQueryString("FuncType");
            if (funcType == "insert")
            {
                //新增时处理权限继承的情况
                if (entity.IsInherit == 1)
                {
                    var list = entities.Set<S_F_DocumentFileAuthority>().Where(i => i.DocumentInfoID == entity.ParentID);
                    foreach (S_F_DocumentFileAuthority authority in list)
                    {
                        var newItem = new S_F_DocumentFileAuthority();
                        newItem.ID = FormulaHelper.CreateGuid();
                        newItem.DocumentInfoID = entity.ID;
                        newItem.AuthType = authority.AuthType;
                        newItem.RoleType = authority.RoleType;
                        newItem.RoleCode = authority.RoleCode;
                        newItem.IsParentAuth = 1;
                        entities.Set<S_F_DocumentFileAuthority>().Add(newItem);
                    }
                }
            }

            entities.SaveChanges();

            PropertyInfo pi = typeof(S_F_DocumentInfo).GetProperty("ID");
            if (pi != null)
                return Json(new { ID = pi.GetValue(entity, null) });
            return Json(new { ID = "" });
        }

        public override JsonResult GetTree()
        {
            //只显示没有作废且没有删除的节点
            var documentInfoList = entities.Set<S_F_DocumentInfo>().Where(a => a.IsDeleted == 0 && a.IsValid == "T").ToList();
            var list = documentInfoList.Select(documentInfo => new
            {
                documentInfo.ID,
                documentInfo.ParentID,
                documentInfo.FullPathID,
                documentInfo.Name,
                documentInfo.Code,
                documentInfo.SortIndex,
                documentInfo.Level,
                documentInfo.IsInherit,
                documentInfo.IsValid,
                IconType = "mini-tree-folder",
                Authority = DocumentCabinetsFO.GetHighestAuthorityString(documentInfo.ID)
            }).ToList();

            var result = list.Where(a => string.IsNullOrEmpty(a.ParentID)).ToList();
            foreach (var item in list)
            {
                if (item.Authority == DocumentCabinetsAuthType.CanWrite.ToString() ||
                    item.Authority == DocumentCabinetsAuthType.FullControl.ToString())
                {
                    var acestors = list.Where(a => item.FullPathID.Contains(a.ID)).ToList();
                    foreach (var acestor in acestors)
                    {
                        if (!result.Contains(acestor))
                            result.Add(acestor);
                    }
                }
            }

            return Json(result);
        }

        public JsonResult GetViewTree()
        {
            //只显示没有作废且没有删除的节点
            var documentInfoList = entities.Set<S_F_DocumentInfo>().Where(a => a.IsDeleted == 0 && a.IsValid == "T").ToList();
            var list = documentInfoList.Select(documentInfo => new
            {
                documentInfo.ID,
                documentInfo.ParentID,
                documentInfo.FullPathID,
                documentInfo.Name,
                documentInfo.Code,
                documentInfo.SortIndex,
                documentInfo.Level,
                documentInfo.IsInherit,
                documentInfo.IsValid,
                IconType = "mini-tree-folder",
                Authority = DocumentCabinetsFO.GetHighestAuthorityString(documentInfo.ID)
            }).ToList();

            var result = list.Where(a => string.IsNullOrEmpty(a.ParentID)).ToList();
            foreach (var item in list)
            {
                if (item.Authority != DocumentCabinetsAuthType.None.ToString())
                {
                    var acestors = list.Where(a => item.FullPathID.Contains(a.ID)).ToList();
                    foreach (var acestor in acestors)
                    {
                        if (!result.Contains(acestor))
                            result.Add(acestor);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult GetFilesCount(string id)
        {
            var count = entities.Set<S_F_FileInfo>().Count(i => i.DocumentInfoID == id && i.IsDeleted == 0);
            if (count == 0)
            {
                count = entities.Set<S_F_DocumentInfo>().Count(i => i.ParentID == id && i.IsDeleted == 0);
            }
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        #region 作废与取消作废

        /// <summary>
        /// 作废节点
        /// </summary>
        /// <param name="id">节点ID</param>
        public JsonResult DoInvalidNode(string id)
        {
            entities.Set<S_F_DocumentInfo>().Where(i => i.ID == id).Update(i => i.IsValid = "F");
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        /// <param name="id">节点ID</param>
        public JsonResult DoValidNode(string id)
        {
            entities.Set<S_F_DocumentInfo>().Where(i => i.ID == id).Update(i => i.IsValid = "T");
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 删除节点
        /// </summary>
        public override JsonResult DeleteNode()
        {
            string id = GetQueryString("ID");
            entities.Set<S_F_DocumentInfo>().Where(i => i.ID == id).Update(i => i.IsDeleted = 1);
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取节点的最高权限
        /// </summary>
        /// <param name="documentInfoId">节点ID</param>
        /// <returns>最高权限</returns>
        public JsonResult GetHighestAuthority(string documentInfoId)
        {
            var highestAuth = DocumentCabinetsFO.GetHighestAuthorityString(documentInfoId);
            return Json(new { HighestAuthority = highestAuth }, JsonRequestBehavior.AllowGet);
        }
    }
}
