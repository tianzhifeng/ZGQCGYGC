using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Reflection;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;
using Project.Logic;
using System.Linq.Expressions;
using Formula.DynConditionObject;
using System.Text;

namespace Project.Areas.Monitor.Controllers
{
    public class PublishInfoDetailController : ProjectController
    {
        /// <summary>
        /// 筛选获取图纸文件打印列表
        /// </summary>
        /// <returns>列表对应的json对象</returns>
        public JsonResult GetList()
        {
            Expression<Func<S_E_PublishInfoDetail, bool>> predicate = a => true;

            int pageIndex = 1;
            if (!string.IsNullOrEmpty(GetQueryString("CurPage")))
            {
                string tmp = GetQueryString("CurPage");
                Int32.TryParse(tmp, out pageIndex);
            }

            int pageSize = 1;
            if (!string.IsNullOrEmpty(GetQueryString("PageSize")))
            {
                string tmp = GetQueryString("PageSize");
                Int32.TryParse(tmp, out pageSize);
            }

            if (!string.IsNullOrEmpty(GetQueryString("Printed")))
            {
                bool bPrinted = false;
                string tmp = GetQueryString("Printed");
                if (Boolean.TryParse(tmp, out bPrinted))
                {
                    predicate = And(predicate, a => a.Printed == bPrinted);
                }
            }

            if (!string.IsNullOrEmpty(GetQueryString("ProjectInfoName")))
            {
                string tmp = GetQueryString("ProjectInfoName");
                predicate = And(predicate, a => a.ProjectInfoName.Contains(tmp));
            }

            if (!string.IsNullOrEmpty(GetQueryString("ProjectCode")))
            {
                string tmp = GetQueryString("ProjectCode");
                predicate = And(predicate, a => a.ProjectCode.Contains(tmp));
            }

            if (!string.IsNullOrEmpty(GetQueryString("ProjectManager")))
            {
                string tmp = GetQueryString("ProjectManager");
                predicate = And(predicate, a => a.ProjectManager.Contains(tmp));
            }

            if (!string.IsNullOrEmpty(GetQueryString("ChargeDeptName")))
            {
                string tmp = GetQueryString("ChargeDeptName");
                predicate = And(predicate, a => a.ChargeDeptName.Contains(tmp));
            }

            if (!string.IsNullOrEmpty(GetQueryString("DesignerName")))
            {
                string tmp = GetQueryString("DesignerName");
                predicate = And(predicate, a => a.DesingerName.Contains(tmp));
            }

            if (!string.IsNullOrEmpty(GetQueryString("PublishFrm")))
            {
                DateTime frm = Convert.ToDateTime(GetQueryString("PublishFrm"));
                predicate = And(predicate, a => a.PublishDate >= frm);
            }

            if (!string.IsNullOrEmpty(GetQueryString("PublishTo")))
            {
                DateTime to = Convert.ToDateTime(GetQueryString("PublishTo"));
                to = to.AddDays(1);
                predicate = And(predicate, a => a.PublishDate < to);
            }

            if (pageIndex < 1)
                pageIndex = 1;
            var itemIndex = (pageIndex - 1) * pageSize;

            var resutls = entities.Set<S_E_PublishInfoDetail>().Where(predicate.Compile()).OrderBy(a => a.ProductCode).Skip(itemIndex).Take(pageSize);
            int total = entities.Set<S_E_PublishInfoDetail>().Count(predicate.Compile());

            return Json(new { list = resutls.ToList(), total = total }, JsonRequestBehavior.AllowGet);
        }

        public void UpdatePrintConfig()
        {
            List<Dictionary<string, object>> detailPrintParams = JsonHelper.ToObject<List<Dictionary<string, object>>>(GetQueryString("UpdatePrintConfig"));
            foreach (Dictionary<string, object> dic in detailPrintParams)
            {
                string id = dic.GetValue("ID");
                string paperSize = dic.GetValue("PaperSize");
                bool isVertical = Convert.ToBoolean(dic.GetValue("IsVertical"));

                entities.Set<S_E_PublishInfoDetail>().Where(a => a.ID == id)
                    .Update(a =>
                    {                        
                        a.PaperSize = paperSize;
                        a.IsVertical = isVertical;
                    });
            }
            entities.SaveChanges();
        }

        public void Print()
        {
            Dictionary<string, object> dic = JsonHelper.ToObject<Dictionary<string, object>>(GetQueryString("Print"));
            //foreach (Dictionary<string, object> dic in detailPrintParams)
            {
                string id = dic.GetValue("ID");
                int count = Convert.ToInt32(dic.GetValue("Count"));

                entities.Set<S_E_PublishInfoDetail>().Where(a => a.ID == id)
                    .Update(a => 
                    {
                        a.Count = a.Count + count;
                        a.Printed = true;
                    });
            }
            entities.SaveChanges();
        }

        private  Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
