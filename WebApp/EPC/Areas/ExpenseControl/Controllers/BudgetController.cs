using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.ExpenseControl.Controllers
{
    public class BudgetController : EPCController<S_I_BudgetInfo_Detail>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var version = engineeringInfo.S_I_BudgetInfo.OrderByDescending(c => c.ID).FirstOrDefault();
            var pushCount = engineeringInfo.S_I_BudgetInfo.Count(c => c.FlowPhase == "End");
            bool flowEnd = true; bool isFirst = false;
            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
                isFirst = true;
                ViewBag.VersionID = "";
                ViewBag.FlowPhase = "";
                ViewBag.VersionNo = "0";
            }
            else
            {
                if (version.FlowPhase != "End")
                    flowEnd = false;
                ViewBag.FlowPhase = version.FlowPhase;
                ViewBag.VersionID = version.ID;
                ViewBag.VersionNo = version.VersionNumber;
            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            ViewBag.Msg = engineeringInfo.NewBom;

            #region 是否启用虚加载
            int detailCount = 0;
            if (version != null)
            {
                var detailCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_I_BudgetInfo_Detail with(nolock) WHERE BudgetInfoID='" + version.ID + "'");
                int.TryParse((detailCountObj ?? "").ToString(), out detailCount);
            }
            ViewBag.VirtualScroll = "false";
            if ((detailCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }
            #endregion
            return View();
        }

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType, string showAll)
        {
            qb.PageSize = 0;
            var version = this.GetEntityByID<S_I_BudgetInfo>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("");
            var treeList = new List<S_I_BudgetInfo_Detail>();
            qb.PageSize = 0;
            qb.Add("BudgetInfoID", QueryMethod.Equal, VersionID);
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Normal.ToString());
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
            }
            if (!String.IsNullOrEmpty(showAll) && showAll.ToLower() != "true")
            {
                qb.Add("NodeType", QueryMethod.NotEqual, "Detail");
            }
            treeList = this.entities.Set<S_I_BudgetInfo_Detail>().Where(qb).ToList();
            return Json(treeList);
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_I_BudgetInfo_Detail>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {
                var lastVersion = this.entities.Set<S_I_BudgetInfo>().Where(d => d.EngineeringInfoID == detail.S_I_BudgetInfo.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.BudgetInfoID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion == null)
                {
                }
                else
                {
                    var lastDetail = lastVersion.S_I_BudgetInfo_Detail.FirstOrDefault(c => c.CBSID == detail.CBSID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_I_BudgetInfo_Detail>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult UpgradBudget(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程，无法创建预算信息");
            engineeringInfo.UpgradeBudgetInfo();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddVersionCBS(string NodeID, string AddMode, string VersionID)
        {
            var node = this.GetEntityByID<S_I_BudgetInfo_Detail>(NodeID);
            if (node == null) throw new Formula.Exceptions.BusinessValidationException("未能找到选中的费用科目节点，无法新增");
            var result = new Dictionary<string, object>();
            if (AddMode == "After")
            {
                var child = node.AddEmptyBrother();
                result = FormulaHelper.ModelToDic<S_I_BudgetInfo_Detail>(child);
            }
            else if (AddMode == "AddChild")
            {
                var child = node.AddEmptyChild();
                result = FormulaHelper.ModelToDic<S_I_BudgetInfo_Detail>(child);
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveNodes(string ListData, string VersionID)
        {
            var list = JsonHelper.ToList(ListData);
            var version = this.GetEntityByID<S_I_BudgetInfo>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的预算版本信息，保存失败");
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                var detail = this.GetEntityByID<S_I_BudgetInfo_Detail>(item.GetValue("ID"));
                if (detail != null)
                {
                    this.UpdateEntity<S_I_BudgetInfo_Detail>(detail, item);
                    if (detail.CustomEdit == "True")
                    {
                        if (this.entities.Set<S_I_BudgetInfo_Detail>().Count(c => c.CBSParentID == detail.CBSID && c.BudgetInfoID == version.ID) > 0)
                            detail.CustomEdit = "True";
                        else
                            detail.CustomEdit = "False";
                    }
                    if (detail.TotalValue == detail.LastVersionValue)
                    {
                        if (detail.ModifyState == BomVersionModifyState.Modify.ToString())
                            detail.ModifyState = BomVersionModifyState.Normal.ToString();
                    }
                    else
                    {
                        if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                            detail.ModifyState = BomVersionModifyState.Modify.ToString();
                    }
                }
            }
            version.SummaryTotalValue();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_I_BudgetInfo_Detail>(item.GetValue("ID"));
                if (detail == null)
                    continue;
                var parent = this.entities.Set<S_I_BudgetInfo_Detail>().FirstOrDefault(c => c.CBSID == detail.CBSParentID && c.BudgetInfoID == detail.BudgetInfoID);
                detail.Remove();
                if (parent != null)
                {
                    parent.SumTotalValue();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_I_BudgetInfo>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            if (version.FlowPhase == "End") { throw new Formula.Exceptions.BusinessValidationException("已经发布的版本不能进行撤销操作"); }
            this.entities.Set<S_I_BudgetInfo>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFromBid(string VersionID)
        {
            var version = this.entities.Set<S_I_BudgetInfo>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            if (version.FlowPhase == "End") { throw new Formula.Exceptions.BusinessValidationException("已经发布的版本不能进行导入操作"); }
            version.ImportFromBid();
            version.SummaryTotalValue();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportFromEBom(string VersionID, string EBomID)
        {
            var version = this.entities.Set<S_I_BudgetInfo>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            var eBom = this.entities.Set<S_E_Bom>().FirstOrDefault(c => c.ID == EBomID);
            if (eBom == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的BOM对象，无法导入");
            }
            version.ImportFromEBom(eBom);
            version.SummaryTotalValue();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPBS(string VersionID)
        {
            var version = this.entities.Set<S_I_BudgetInfo>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Exception("没有可编辑的版本，无法进行导入操作");

            version.ImportPBS();
            this.entities.SaveChanges();
            return Json("");
        }

        #region Excel导出
        [ValidateInput(false)]
        public ActionResult ExportExcel(string VersionID, string jsonColumns, string title)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "Budget";

            var version = this.GetEntityByID<S_I_BudgetInfo>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("未能找到预算信息，无法导出EXCEL");

            if (string.IsNullOrEmpty(title))
            {
                title = version.S_I_Engineering.Name + "_预算表";
            }

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


            var tmp = version.S_I_BudgetInfo_Detail.Where(c => c.ModifyState != BomVersionModifyState.Remove.ToString()).
                OrderBy(d => d.CBSFullID.Length).ThenBy(c => c.SortIndex).ToList();

            #region 重排
            Action<List<S_I_BudgetInfo_Detail>, string, List<S_I_BudgetInfo_Detail>> f = null;
            f = (res, parentID, source) =>
            {
                var children = source.Where(a => a.CBSParentID == parentID);
                foreach (var child in children)
                {
                    res.Add(child);
                    f(res, child.CBSID, source);
                }
            };
            List<S_I_BudgetInfo_Detail> data = new List<S_I_BudgetInfo_Detail>();
            var topDatas = tmp.Where(a => a.CBSFullID.Length == tmp.Min(b => b.CBSFullID.Length));
            foreach (var top in topDatas)
            {
                data.Add(top);
                f(data, top.CBSID, tmp);
            }
            #endregion

            var dt = new DataTable();
            dt.Columns.Add("NodeType");
            var type = typeof(S_I_BudgetInfo_Detail);
            foreach (var item in columns)
            {

                if (dt.Columns.Contains(item.FieldName))
                {
                    continue;
                }
                var property = type.GetProperty(item.FieldName);

                if (property != null)
                {
                    if ( property.PropertyType==typeof(System.Nullable<decimal>)||property.PropertyType==typeof(decimal)
                        || property.PropertyType == typeof(System.Nullable<float>) || property.PropertyType == typeof(float)
                        ||property.PropertyType == typeof(System.Nullable<double>) || property.PropertyType == typeof(double))
                    {
                        dt.Columns.Add(item.FieldName, typeof(decimal));
                    }
                    else if (property.PropertyType == typeof(System.Nullable<int>) || property.PropertyType == typeof(int))
                    {
                        dt.Columns.Add(item.FieldName, typeof(int));
                    }
                    else
                    {
                        dt.Columns.Add(item.FieldName);
                    }
                }
                else
                    dt.Columns.Add(item.FieldName);
            }
            foreach (var item in tmp)
            {
                var row = dt.NewRow();
                var dic = FormulaHelper.ModelToDic<S_I_BudgetInfo_Detail>(item);
                foreach (var detailColumn in columns)
                {
                    if (dic.ContainsKey(detailColumn.FieldName) && dic[detailColumn.FieldName] != null)
                        row[detailColumn.FieldName] = dic[detailColumn.FieldName];  //dic.GetValue(detailColumn.FieldName);
                }
                dt.Rows.Add(row);
            }
            dt.TableName = excelKey;
            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }
        #endregion
    }
}
