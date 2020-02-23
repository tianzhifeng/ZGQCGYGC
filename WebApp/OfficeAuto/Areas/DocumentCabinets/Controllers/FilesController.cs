using System.Linq;
using System.Web.Mvc;
using MvcAdapter;
using OfficeAuto.Logic;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.DocumentCabinets.Controllers
{
    public class FilesController : BaseController<S_F_FileInfo>
    {
        public override ActionResult List()
        {
            var documentInfoId = GetQueryString("DocumentInfoID");
            ViewBag.HighestAuthority = DocumentCabinetsFO.GetHighestAuthorityString(documentInfoId);
            var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault(i => i.ID == documentInfoId);
            if (entity != null)
            {
                ViewBag.IsValid = entity.IsValid;
            }
            return base.List();
        }

        protected override void EntityCreateLogic<TEntity>(TEntity entity)
        {
            base.EntityCreateLogic(entity);
            if (entity is S_F_FileInfo)
            {
                var documentInfoId = GetQueryString("DocumentInfoID");
                var obj = entity as S_F_FileInfo;
                obj.IsValid = "T";
                obj.IsDeleted = 0;
                if (documentInfoId != null)
                {
                    obj.DocumentInfoID = documentInfoId;
                }
            }
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            var parentId = GetQueryString("DocumentInfoID");
            var list = entities.Set<S_F_FileInfo>().Where(i => i.DocumentInfoID == parentId && i.IsDeleted == 0).WhereToGridData(qb);
            return Json(list);
        }

        public override JsonResult Delete()
        {
            var ids = Request["ListIDs"];
            entities.Set<S_F_FileInfo>().Where(i => ids.Contains(i.ID)).Update(i => i.IsDeleted = 1);
            entities.SaveChanges();
            return Json("");
        }

        /// <summary>
        /// 作废
        /// </summary>
        public JsonResult DoInvalid(string id)
        {
            entities.Set<S_F_FileInfo>().Where(i => i.ID == id).Update(i => i.IsValid = "F");
            entities.SaveChanges();
            return Json("");
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        public JsonResult DoValid(string id)
        {
            entities.Set<S_F_FileInfo>().Where(i => i.ID == id).Update(i => i.IsValid = "T");
            entities.SaveChanges();
            return Json("");
        }

        //public ActionResult PageViewList(string tmplCode)
        //{
        //    return base.PageView(tmplCode);
        //}
    }
}
