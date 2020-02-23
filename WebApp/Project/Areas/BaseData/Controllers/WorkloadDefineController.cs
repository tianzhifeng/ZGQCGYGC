using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using MvcAdapter;

namespace Project.Areas.BaseData.Controllers
{
    public class WorkloadDefineController : BaseConfigController
    {
        public JsonResult GetManageWorkloadDefineList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.JsonGetList<S_C_ManageWorkloadDefine>(qb);
        }

        public JsonResult SaveManageWorkloadDefineList()
        {
            var list = UpdateSortedList<S_C_ManageWorkloadDefine>(Request["SortedListData"], Request["DeletedListData"]);
            if (list.GroupBy(a => a.Code).Any(a => a.Count() > 1))
                throw new Formula.Exceptions.BusinessException("编号不能重复!");
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetMajorWorkloadDefineList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.JsonGetList<S_C_MajorWorkloadDefine>(qb);
        }

        public JsonResult SaveMajorWorkloadDefineList()
        {
            var list = UpdateSortedList<S_C_MajorWorkloadDefine>(Request["SortedListData"], Request["DeletedListData"]);
            if (list.GroupBy(a => a.MajorValue).Any(a => a.Count() > 1))
                throw new Formula.Exceptions.BusinessException("专业不能重复!");
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetRoleWorkloadDefineList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.JsonGetList<S_C_RoleWorkloadDefine>(qb);
        }

        public JsonResult SaveRoleWorkloadDefineList()
        {
            var list = UpdateSortedList<S_C_RoleWorkloadDefine>(Request["SortedListData"], Request["DeletedListData"]);
            if (list.GroupBy(a => a.RoleCode).Any(a => a.Count() > 1))
                throw new Formula.Exceptions.BusinessException("角色不能重复!");
            entities.SaveChanges();
            return Json("");
        }
    }
}
