using Formula;
using MvcAdapter;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using Config.Logic;

using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class FilePrintAPIController : ApiController
    {
        public class SearchPara
        {
            /// <summary>
            /// 图名
            /// </summary>
            public string ProductName = "";
            /// <summary>
            /// 图号
            /// </summary>
            public string ProductCode = "";
            /// <summary>
            /// 项目名称
            /// </summary>
            public string ProjectInfoName = "";
            /// <summary>
            /// 项目编号
            /// </summary>
            public string ProjectCode = "";
            /// <summary>
            /// 项目负责人
            /// </summary>
            public string ProjectManager = "";
            /// <summary>
            /// 图纸提供人
            /// </summary>
            public string Provider = "";
            /// <summary>
            /// 专业
            /// </summary>
            public string MajorCode = "";
            /// <summary>
            /// 提交起始日期
            /// </summary>
            public DateTime? SubmitDateFrm = null;
            /// <summary>
            /// 提交终止日期
            /// </summary>
            public DateTime? SubmitDateTo = null;
            /// <summary>
            /// 作业部门
            /// </summary>
            public string ChargeDeptName = "";
            /// <summary>
            ///  
            /// </summary>
            public string DesignerName = "";
            /// <summary>
            /// 出图单编号
            /// </summary>
            public string CTNum = "";
            /// <summary>
            /// 是否已打印
            /// </summary>
            public bool Printed = false;
            public int CurPage = 1;
            public int PageSize = 10;
        }

        public class SearchResult 
        {

            public List<Dictionary<string, object>> PageList = new List<Dictionary<string, object>>();

            public int Total = 0;
        }


        public class UpdatePrintConfigPara
        {
            public string ID { get; set; }
            public string PaperSize { get; set; }
            public bool IsVertical { get; set; }
        }

        public class PrintPara 
        {
            public string ID { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// 筛选获取图纸文件打印列表
        /// webapi 会对结果进行json化,因此只需要返回object
        /// </summary>
        /// <returns>列表对应的json对象</returns>
        [HttpPost]
        [ActionName("GetList")]
        public JsonAjaxResult _GetList(SearchPara para)
        {
            SearchResult result = new SearchResult();
            JsonAjaxResult jsonR = new JsonAjaxResult() { Data = result };
            Expression<Func<S_E_PublishInfoDetail, bool>> predicate = a => true;
            int pageIndex = 1;
            int pageSize = 10;

            if (para != null)
            {
                pageIndex = para.CurPage;
                pageSize = para.PageSize;
                predicate = And(predicate, a => a.Printed == para.Printed && !string.IsNullOrEmpty(a.PdfFile));
                if (!string.IsNullOrEmpty(para.ProjectInfoName))
                {
                    predicate = And(predicate, a => a.ProjectInfoName.Contains(para.ProjectInfoName));
                }

                if (!string.IsNullOrEmpty(para.ProjectCode))
                {
                    predicate = And(predicate, a => a.ProjectCode.Contains(para.ProjectCode));
                }

                if (!string.IsNullOrEmpty(para.ProjectManager))
                {
                    predicate = And(predicate, a => a.S_E_PublishInfo.S_I_ProjectInfo.ChargeUserName.Contains(para.ProjectManager));
                }

                if (!string.IsNullOrEmpty(para.ChargeDeptName))
                {
                    predicate = And(predicate, a => a.ChargeDeptName.Contains(para.ChargeDeptName));
                }

                if (!string.IsNullOrEmpty(para.DesignerName))
                {
                    predicate = And(predicate, a => a.DesingerName.Contains(para.DesignerName));
                }

                if (para.SubmitDateFrm != null)
                {
                    predicate = And(predicate, a => a.PublishDate >= para.SubmitDateFrm);
                }

                if (para.SubmitDateTo != null)
                {
                    DateTime to = para.SubmitDateTo.Value.AddDays(1);
                    predicate = And(predicate, a => a.PublishDate < to);
                }
            }
            else
            {
                jsonR.Message = "传入的SearchPara参数为空,过滤条件失效,默认显示10条数据";
            }

            if (pageIndex < 1)
                pageIndex = 1;
            var itemIndex = (pageIndex - 1) * pageSize;

            Formula.Exceptions.BusinessException e = null;
            try
            {
                result.PageList = entities.Set<S_E_PublishInfoDetail>().Include("S_E_PublishInfo").Include("S_E_PublishInfo.S_I_ProjectInfo")
                    .Where(predicate.Compile())
                    .OrderBy(a => a.ProductCode)
                    .Skip(itemIndex).Take(pageSize).ToList().Select(a => {
                        var dic = new Dictionary<string, object>();
                        dic = a.ToDic();
                        dic.SetValue("ProjectManagerName", a.S_E_PublishInfo.S_I_ProjectInfo.ChargeUserName);
                        dic.SetValue("MajorName", a.S_E_PublishInfo.MajorName);
                        dic.SetValue("StepName", a.S_E_PublishInfo.S_I_ProjectInfo.PhaseName);
                        return dic; 
                    }).ToList();

                result.Total = entities.Set<S_E_PublishInfoDetail>().Count(predicate.Compile());
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                e = ex;                
            }

            if (e != null)
            {
                jsonR.Message = e.Message;
                jsonR.StatusCode = 500;
            }
            else
            {
                jsonR.Message = "获取成功";
                jsonR.StatusCode = 200;
            }
            
            return jsonR;
        }

        [HttpPost]
        [ActionName("UpdatePrintConfig")]
        public JsonAjaxResult _UpdatePrintConfig(List<UpdatePrintConfigPara> paras)
        {
            JsonAjaxResult jsonR = new JsonAjaxResult();
            foreach (UpdatePrintConfigPara dic in paras)
            {
                entities.Set<S_E_PublishInfoDetail>().Where(a => a.ID == dic.ID)
                    .Update(a =>
                    {
                        a.PaperSize = dic.PaperSize;
                        a.IsVertical = dic.IsVertical;
                    });
            }

            Formula.Exceptions.BusinessException e = null;
            try
            {
                entities.SaveChanges();
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                jsonR.Message = ex.Message;
                jsonR.StatusCode = 500;
            }

            if (e != null)
            {
                jsonR.Message = e.Message;
                jsonR.StatusCode = 500;
            }
            else
            {
                jsonR.Message = "操作成功";
                jsonR.StatusCode = 200;
            }

            return jsonR;
        }

        [HttpPost]
        [ActionName("Print")]
        public JsonAjaxResult _Print(PrintPara para)
        {
            JsonAjaxResult jsonR = new JsonAjaxResult();
            entities.Set<S_E_PublishInfoDetail>().Where(a => a.ID == para.ID)
                .Update(a =>
                {
                    a.Count = a.Count + para.Count;
                    a.Printed = true;
                });

            Formula.Exceptions.BusinessException e = null;
            try
            {
                entities.SaveChanges();
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                jsonR.Message = ex.Message;
                jsonR.StatusCode = 500;
            }

            if (e != null)
            {
                jsonR.Message = e.Message;
                jsonR.StatusCode = 500;
            }
            else
            {
                jsonR.Message = "操作成功";
                jsonR.StatusCode = 200;
            }

            return jsonR;
        }

        private Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        private ProjectEntities entities
        {
            get
            {
                return FormulaHelper.GetEntities<ProjectEntities>();
            }
        }
    }
}