using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Formula;
using Config;
using Formula.Exceptions;
using MvcAdapter;



namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class LogisticsOfficeController : OfficeAutoFormContorllor<T_Logistics_Office>
    {
        // GET: /AutoForm/LogisticsOffice/

        #region 办公室位置

        public JsonResult GetTree()
        {
         
            string sql = "select * from S_E_Logistics_OfficePos order by SortIndex";

            var dt = SQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult DeleteNode()
        {
            var fullID = Request["FullID"].ToString();
            if (string.IsNullOrEmpty(fullID))
            {
                return Json("");
            }

            if (!fullID.Contains("."))
            {
                throw new BusinessException("不能作废整个位置");
            }

            var poss = this.BusinessEntities.Set<S_E_Logistics_OfficePos>().Where(c => c.FullID.StartsWith(fullID)).ToArray();

            foreach (var pos in poss)
            {
                //取消这个节点下关联的办公室详情
                var list = BusinessEntities.Set<T_Logistics_Office>().Where(p => !string.IsNullOrEmpty(p.NodeFullID) && p.NodeFullID.Contains(pos.ID));

                foreach (var item in list)
                {
                    item.NodeFullID = "";
                    item.NodeID = "";
                }

                BusinessEntities.Set<S_E_Logistics_OfficePos>().Remove(pos);
            }

            BusinessEntities.SaveChanges();

            return Json("");
        }

        #endregion

        #region 办公室详情

        public JsonResult GetOfficeList(QueryBuilder qb)
        {
            var nodeFullID = GetQueryString("nodeFullID");
            var applyType = GetQueryString("ApplyType");

            var useStatus = string.Empty;
            if (applyType == "Add")
            {
                useStatus = "NoUse";
            }

            if (applyType == "Back")
            {
                useStatus = "Use";
            }

            var dept = GetQueryString("DeptID");

            var list = BusinessEntities.Set<T_Logistics_Office>().Where(c => c.Status == "Normal" && c.NodeFullID.StartsWith(nodeFullID));

            if (!string.IsNullOrEmpty(useStatus))
            {
                list = list.Where(p => p.UseStatus == useStatus);
            }

            if (!string.IsNullOrEmpty(dept))
            {
                list = list.Where(p => p.CurrentUseDept == dept);
            }

            var data = list.WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult DelOffice(string IDs)
        {
            if (!string.IsNullOrEmpty(IDs))
            {
                var arr = IDs.Split(',');

                foreach (var item in arr)
                {
                    var entity = BusinessEntities.Set<T_Logistics_Office>().Find(item);

                    if (entity != null)
                    {
                        BusinessEntities.Set<T_Logistics_Office>().Remove(entity);
                    }
                }

                BusinessEntities.SaveChanges();
            }

            return Json("");
        }

        #endregion
    }
}
