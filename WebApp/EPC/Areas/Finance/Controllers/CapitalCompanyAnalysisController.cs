using EPC.Logic.Domain;
using Config.Logic;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic;
using Formula;
using Config;
using System.Data;

namespace EPC.Areas.Finance.Controllers
{
    public class CapitalCompanyAnalysisController : EPCController
    {
        private string yearAndMonthColPre = "colYearAndMonth";
        private string yearAndMonthColValueSuf = "_plan";
        private string yearAndMonthColRealDeltaSuf = "_delta";

        public ActionResult PageView()
        {
            List<dynamic> res = new List<dynamic>();
            int count = 5;
            int nowYear = DateTime.Now.Year;
            for (int i = 0; i < count; i++)
            {
                res.Add(new { text = (nowYear + i), value = i });
            }
            ViewBag.Years = JsonHelper.ToJson(res);
            ViewBag.YearAndMonthColPre = yearAndMonthColPre;
            ViewBag.YearAndMonthColValueSuf = yearAndMonthColValueSuf;
            ViewBag.YearAndMonthColRealDeltaSuf = yearAndMonthColRealDeltaSuf;
            return View();
        }

        //获取子表数据
        public JsonResult GetDetailList(string monthColArr, string templateCode)
        {
            return Json(GetDetailDicList(monthColArr, templateCode));
        }

        public JsonResult SaveList(string year, string listData, string templateCode)
        {
            var detailList = JsonHelper.ToList(listData);
            UpdateDetail(year, detailList, templateCode);
            entities.SaveChanges();
            return Json("");
        }

        [ValidateInput(false)]
        public ActionResult ExportExcel(string monthColArr, string jsonColumns, string title, string templateCode)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "CapitalCompany";

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            templatePath = Server.MapPath("/") + templatePath;

            if (System.IO.File.Exists(templatePath))
            {
                Formula.LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = Formula.ImportExport.FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, title);
            }

            var dt = new DataTable();
            foreach (var item in columns)
            {
                if (dt.Columns.Contains(item.FieldName))
                {
                    continue;
                }
                dt.Columns.Add(item.FieldName);
            }

            List<Dictionary<string, object>> dicList = GetDetailDicList(monthColArr, templateCode);

            foreach (var dic in dicList)
            {
                var detailRow = dt.NewRow();

                foreach (var detailColumn in columns)
                {
                    if (detailColumn.FieldName == "Name")
                    {
                        string space = "";
                        if (!string.IsNullOrEmpty(dic.GetValue("FullID")))
                        {
                            int spaceCount = dic.GetValue("FullID").Split(',').Length;
                            for (int i = 0; i < spaceCount; i++)
                            {
                                space += "    ";
                            }
                        }
                        detailRow[detailColumn.FieldName] = space + dic.GetValue(detailColumn.FieldName);
                    }
                    else
                    {
                        detailRow[detailColumn.FieldName] = dic.GetValue(detailColumn.FieldName);
                    }
                }
                dt.Rows.Add(detailRow);
            }

            dt.TableName = "CapitalCompany";
            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }

        private List<Dictionary<string, object>> GetDetailDicList(string monthColArr, string templateCode)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var sourceDicList = JsonHelper.ToList(monthColArr);

            if (sourceDicList.Count == 0)
                return result;

            DateTime startDate = new DateTime(
                Convert.ToInt32(sourceDicList[0].GetValue("year")),
                Convert.ToInt32(sourceDicList[0].GetValue("month")),
                1);

            DateTime endDate = new DateTime(
                Convert.ToInt32(sourceDicList[sourceDicList.Count - 1].GetValue("year")),
                Convert.ToInt32(sourceDicList[sourceDicList.Count - 1].GetValue("month")),
                1);

            //明细
            var detailList = entities.Set<S_F_CapitalAnalysis_Detail>().Where(a => a.BudgetDate >= startDate && a.BudgetDate <= endDate && a.TemplateCode == templateCode).ToList();
            //明细对应的模板id列表
            var templateIDList = detailList.GroupBy(a => a.TemplateID).Select(a => a.First()).Select(a => a.TemplateID);

            foreach (var templateID in templateIDList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var sameTemplateIDDetailList = detailList.Where(a => a.TemplateID == templateID);
                var tmp = sameTemplateIDDetailList.FirstOrDefault();

                dic.SetValue("Name", tmp.Name);
                dic.SetValue("ID", tmp.TemplateID);//tree row id 取 模板id
                dic.SetValue("ParentID", tmp.ParentTemplateID);//tree row parentid 取 模板parentid
                dic.SetValue("FullID", tmp.FullTemplateID);
                dic.SetValue("SortIndex", tmp.SortIndex);
                dic.SetValue("CapitalPlanType", tmp.CapitalPlanType);
                dic.SetValue("AddOrSub", GetAddOrSub(tmp.CapitalPlanType));
                dic.SetValue("HaveSource", !string.IsNullOrEmpty(tmp.RealSourceSQL));
                dic.SetValue("RealSourceSQL", tmp.RealSourceSQL);
                dic.SetValue("RealSourceLink", tmp.RealSourceLink);
                dic.SetValue("SourceLink", tmp.SourceLink);
                dic.SetValue("HaveLink", !string.IsNullOrEmpty(tmp.SourceLink));
                dic.SetValue("HaveRealLink", !string.IsNullOrEmpty(tmp.RealSourceLink));
                dic.SetValue("IsReadOnly", tmp.IsReadOnly);

                //月份列
                foreach (var sourceDic in sourceDicList)
                {
                    DateTime budgetDate = Convert.ToDateTime(sourceDic.GetValue("year") + "-" + sourceDic.GetValue("month"));
                    var cell = sameTemplateIDDetailList.SingleOrDefault(a => a.TemplateID == templateID && a.BudgetDate == budgetDate);
                    if (cell != null)
                    {
                        dic.SetValue(sourceDic.GetValue("colName") + yearAndMonthColValueSuf, cell.BudgetValue);
                        dic.SetValue(sourceDic.GetValue("colName"), cell.RealValue ?? 0);
                        dic.SetValue(sourceDic.GetValue("colName") + yearAndMonthColRealDeltaSuf, (cell.RealValue ?? 0) - (cell.BudgetValue ?? 0));
                    }
                    //没有编制则为0
                    else
                    {
                        dic.SetValue(sourceDic.GetValue("colName") + yearAndMonthColValueSuf, 0);
                        dic.SetValue(sourceDic.GetValue("colName"), 0);
                        dic.SetValue(sourceDic.GetValue("colName") + yearAndMonthColRealDeltaSuf, 0);
                    }

                }
                result.Add(dic);
            }
            return result;
        }

        private void UpdateDetail(string year, List<Dictionary<string, object>> childrenDics, string templateCode)
        {
            //行
            foreach (var child in childrenDics)
            {
                //列
                foreach (var key in child.Keys)
                {
                    //月份列
                    if (!key.Contains(yearAndMonthColPre))
                        continue;

                    //排除plan
                    if (key.Contains(yearAndMonthColValueSuf))
                        continue;

                    //排除delta
                    if (key.Contains(yearAndMonthColRealDeltaSuf))
                        continue;

                    string month = key.Replace(yearAndMonthColPre, "");
                    DateTime bDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                    string templateID = child.GetValue("ID");
                    //唯一数据
                    S_F_CapitalAnalysis_Detail onlyOneDetail = entities.Set<S_F_CapitalAnalysis_Detail>().SingleOrDefault(
                        a => a.TemplateID == templateID
                            && a.TemplateCode == templateCode
                            && a.BudgetDate == bDate);

                    if (onlyOneDetail == null)
                    {
                        //没做过计划
                        continue;
                    }

                    onlyOneDetail.RealValue = Convert.ToDecimal(child.GetValue(key));
                }

                var childChildrenDics = JsonHelper.ToList(child.GetValue("children"));
                UpdateDetail(year, childChildrenDics, templateCode);
            }
        }

        //private void FillChildrenDic(string parentTemplateId, ref List<Dictionary<string, object>> result, List<Dictionary<string, object>> sourceDicList, string engineeringInfoID)
        //{
        //    var tmpList = entities.Set<S_F_CapitalPlanTemplate_Detail>().Where(a => a.ParentID == parentTemplateId).OrderBy(a => a.SortIndex).ToList();
        //    foreach (var tmp in tmpList)
        //    {
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        string newDicId = FormulaHelper.CreateGuid();
        //        dic.SetValue("Name", tmp.Name);
        //        dic.SetValue("ID", tmp.ID);//tree row id 取 模板id
        //        dic.SetValue("ParentID", tmp.ParentID);//tree row parentid 取 模板parentid
        //        dic.SetValue("SortIndex", tmp.SortIndex);
        //        dic.SetValue("CapitalPlanType", tmp.CapitalPlanType);
        //        dic.SetValue("AddOrSub", GetAddOrSub(tmp.CapitalPlanType));
        //        dic.SetValue("HaveSource", !string.IsNullOrEmpty(tmp.SourceSQL));
        //        dic.SetValue("SourceSQL", tmp.SourceSQL);
        //        dic.SetValue("RealSourceSQL", tmp.RealSourceSQL);
        //        dic.SetValue("IsReadOnly", tmp.IsReadOnly);

        //        //月份列
        //        foreach (var sourceDic in sourceDicList)
        //        {
        //            string sql = tmp.SourceSQL;
        //            if (!string.IsNullOrEmpty(sql))
        //            {
        //                DateTime budgetDate = new DateTime(Convert.ToInt32(sourceDic.GetValue("year")), Convert.ToInt32(sourceDic.GetValue("month")), 1);
        //                sql = sql.Replace("{CapitalPlanDate}", budgetDate.ToString("yyyy-MM-dd"));

        //                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
        //                object obj = sqlHelper.ExecuteScalar(sql);
        //                decimal val = 0;
        //                if (obj != null && decimal.TryParse(obj.ToString(), out val))
        //                {
        //                    dic.SetValue(sourceDic.GetValue("colName"), val);
        //                    continue;
        //                }
        //            }

        //            dic.SetValue(sourceDic.GetValue("colName"), 0);
        //        }

        //        result.Add(dic);
        //        FillChildrenDic(tmp.ID, ref result, sourceDicList, engineeringInfoID);
        //    }
        //}

        public JsonResult GetSourceResult(string nodesHaveSource, string yearAndMonth, string engineeringInfoID)
        {
            var nodeList = JsonHelper.ToList(nodesHaveSource);
            var yearAndMonthDic = JsonHelper.ToObject(yearAndMonth);

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var node in nodeList)
            {
                Dictionary<string, object> singResult = new Dictionary<string, object>();
                singResult.SetValue("ID", node.GetValue("ID"));
                singResult.SetValue("ParentID", node.GetValue("ParentID"));
                singResult.SetValue("colName", yearAndMonthDic.GetValue("colName"));
                result.Add(singResult);

                string year = yearAndMonthDic.GetValue("year");
                string month = yearAndMonthDic.GetValue("month");

                //var template = entities.Set<S_F_CapitalPlanTemplate_Detail>().Find(node.GetValue("ID"));
                //if (template != null)
                {
                    string sql = node.GetValue("RealSourceSQL");
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sql = sql.Replace("{year}", year).Replace("{month}", month);

                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                        object obj = sqlHelper.ExecuteScalar(sql);
                        decimal val = 0;
                        if (obj != null && decimal.TryParse(obj.ToString(), out val))
                        {
                            singResult.SetValue("SourceVal", val);
                        }
                        else
                        {
                            singResult.SetValue("SourceVal", 0);
                        }
                    }
                }
            }

            return Json(result);
        }

        //private void FillSum(List<Dictionary<string, object>> result, List<Dictionary<string, object>> monthDicList)
        //{
        //    var parents = result.Where(a => a.GetValue("ParentID") == ""
        //       && a.GetValue("CapitalPlanType") != CapitalPlanType.Total.ToString());

        //    foreach (var p in parents)
        //    {
        //        foreach (var monthDic in monthDicList)
        //        {
        //            var children = result.Where(a => a.GetValue("ParentID") == p.GetValue("ID"));
        //            decimal val = 0;
        //            foreach (var child in children)
        //            {
        //                decimal tmp = 0;
        //                decimal.TryParse(child.GetValue(monthDic.GetValue("colName")), out tmp);
        //                val += tmp;
        //            }
        //            p.SetValue(monthDic.GetValue("colName"), val);
        //        }
        //    }

        //    //total
        //    var totalItem = result.SingleOrDefault(a => a.GetValue("CapitalPlanType") == CapitalPlanType.Total.ToString());
        //    {
        //        foreach (var monthDic in monthDicList)
        //        {
        //            var children = result.Where(a => a.GetValue("ParentID") == "");
        //            decimal val = 0;
        //            foreach (var child in children)
        //            {
        //                decimal tmp = 0;
        //                decimal.TryParse(child.GetValue(monthDic.GetValue("colName")), out tmp);
        //                val += GetAddOrSub(child.GetValue("CapitalPlanType")) * tmp;
        //            }
        //            totalItem.SetValue(monthDic.GetValue("colName"), val);
        //        }
        //    }
        //}

        private decimal GetAddOrSub(string capitalPlanType)
        {
            if (capitalPlanType == CapitalPlanType.In.ToString())
            {
                return 1;
            }
            else if (capitalPlanType == CapitalPlanType.Out.ToString())
            {
                return -1;
            }
            else if (capitalPlanType == CapitalPlanType.Initial.ToString())
            {
                return 1;
            }
            else if (capitalPlanType == CapitalPlanType.Total.ToString())
            {
                //无意义
                return 9999;
            }
            else
            {
                throw new Formula.Exceptions.BusinessValidationException(capitalPlanType + "未在枚举CapitalPlanType中找到对应值");
            }
        }
    }
}
