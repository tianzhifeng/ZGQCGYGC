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
using EPC.Logic;
using EPC.Logic.Domain;
using Config;


namespace  EPC.Areas.Infrastructure.Controllers
{
    public class WBSAttrController : InfrastructureController<S_T_WBSTypeDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.GetList(qb);
        }

        protected override void BeforeSave(S_T_WBSTypeDefine entity, bool isNew)
        {
            var defineInfo = this.entities.Set<S_T_WBSTypeDefine>().FirstOrDefault(d => d.Code == entity.Code && d.ID != entity.ID);
            if (defineInfo != null) throw new Formula.Exceptions.BusinessValidationException("编号不能重复！");
        }

        public JsonResult GetEnumList(string TypeDefineID)
        {
            var data = this.entities.Set<S_T_WBSAttrDefine>().Where(d => d.TypeDefineID == TypeDefineID).OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult SaveAttr()
        {
            this.UpdateEntity<S_T_WBSAttrDefine>();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetAttrInfo(string id)
        {
            var entity = this.GetEntity<S_T_WBSAttrDefine>(id);
            return Json(entity);
        }

        public JsonResult DeleteAttr(string rowIDs)
        {
            foreach (var item in rowIDs.Split(','))
            {
                this.entities.Set<S_T_WBSAttrDefine>().Delete(d => d.ID == item);
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
