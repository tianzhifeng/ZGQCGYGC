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

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementTraceController : EPCController<S_P_Bom>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            //默认展现所有（树节点展开至PBS定义的最下层）
            var deepPBSStruct = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).LastOrDefault();
            if (deepPBSStruct != null)
            {
                ViewBag.ExpandLevel = deepPBSStruct.NodeType;
            }
            else
            {
                ViewBag.ExpandLevel = "";
            }
            var tab = new Tab();
            var catagory = CategoryFactory.GetCategory("Base.BOMMajor", "专业分类", "MajorCode");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetTraceList(string EngineeringInfoID, QueryBuilder qb, string ShowAll)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pBomList = this.entities.Set<S_P_Bom>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).Where(qb).ToList();
            var pbsList = engineeringInfo.S_I_PBS.Where(c => c.NodeType != "Root").OrderBy(c => c.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in pbsList)
            {
                var dic = FormulaHelper.ModelToDic<S_I_PBS>(item);
                var details = pBomList.Where(c => c.PBSNodeID == item.ID).ToList();
                if (pBomList.Count(c => c.PBSNodeFullID.StartsWith(item.FullID)) == 0)
                {
                    continue;
                }
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_P_Bom>(detail);
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult SaveBom(string ListData)
        {
            this.UpdateList<S_P_Bom>(ListData);
            this.entities.SaveChanges();
            return Json("");
        }

        #region Excel导出
        [ValidateInput(false)]
        public ActionResult ExportExcel(string EngineeringInfoID, string jsonColumns, string title, string excelKey)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (string.IsNullOrEmpty(title))
            {
                title = engineeringInfo.Name + "_设计采购进度";
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


            var versionList = this.entities.Set<S_P_Bom>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).OrderBy(c => c.SortIndex).ToList();
            var data = this.entities.Set<S_I_PBS>().Where(d => d.EngineeringInfoID == engineeringInfo.ID && d.NodeType != "Root").
                OrderBy(c => c.SortIndex).ToList();
            var pbsNodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.PBSType");

            var dt = new DataTable();
            dt.Columns.Add("NodeType");
            foreach (var item in columns)
            {
                if (dt.Columns.Contains(item.FieldName))
                {
                    continue;
                }
                dt.Columns.Add(item.FieldName);
            }
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_I_PBS>(item);
                var details = versionList.Where(c => c.PBSNodeID == item.ID).ToList();
                //判定是否要过滤掉没有设备材料的PBS节点
                if (versionList.Count(c => c.PBSNodeFullID.StartsWith(item.FullID)) == 0)
                {
                    continue;
                }
                var row = dt.NewRow();
                foreach (var column in columns)
                {
                    row[column.FieldName] = dic.GetValue(column.FieldName);
                }
                var enumItem = pbsNodeTypeEnum.EnumItem.FirstOrDefault(c => c.Code == item.NodeType);
                if (enumItem == null)
                    row["NodeType"] = item.NodeType;
                else
                    row["NodeType"] = enumItem.Name;
                dt.Rows.Add(row);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_P_Bom>(detail);
                    var detailRow = dt.NewRow();
                    foreach (var detailColumn in columns)
                    {
                        detailRow[detailColumn.FieldName] = detailDic.GetValue(detailColumn.FieldName);
                    }
                    detailRow["NodeType"] = "";
                    dt.Rows.Add(detailRow);
                }
            }
            dt.TableName = excelKey;
            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }
        #endregion
    }
}
