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
    public class PBomController : EPCController<S_P_Bom>
    {
        public ActionResult MainTab()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.SpaceCode = GetQueryString("SpaceCode");
            ViewBag.AuthType = GetQueryString("AuthType");
            return View();
        }

        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            //默认展现所有（树节点展开至PBS定义的最下层）
            var list = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 1)
                {
                    ViewBag.ExpandLevel = i;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);

            var tab = new Tab();
            var catagory = CategoryFactory.GetCategory("Base.BOMMajor", "专业分类", "MajorCode");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult PBomTrace()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            //默认展现所有（树节点展开至PBS定义的最下层）
            var list = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 1)
                {
                    ViewBag.ExpandLevel = i;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);

            var tab = new Tab();
            string sql = String.Format(@"select distinct S_T_BomDefine_Detail.Name as text,S_T_BomDefine_Detail.Code as value,SortIndex from S_T_BomDefine_Detail
left join S_T_BomDefine on DefineID=S_T_BomDefine.ID
where BelongMode like '%{0}%' order by SortIndex ", engineeringInfo.Mode.ID);
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure).ExecuteDataTable(sql);
            var catagory = CategoryFactory.GetCategoryByString(JsonHelper.ToJson(dt), "专业分类", "MajorCode");

            catagory.Multi = false;
            tab.Categories.Add(catagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetTreeList(string EngineeringInfoID, QueryBuilder qb)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pBomList = this.entities.Set<S_P_Bom>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).Where(qb).ToList();
            var pbsList = engineeringInfo.S_I_PBS.OrderBy(c => c.SortIndex).ToList();//.Where(c => c.NodeType != "Root")
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
                    if (String.IsNullOrEmpty(detail.ReturnDatState) || detail.ReturnDatState != ProjectState.Finish.ToString())
                    {
                        detailDic.SetValue("ReturnDatState", "UnFinish");
                    }
                    if (String.IsNullOrEmpty(detail.Quantity.ToString()))
                        detailDic.SetValue("Quantity", 0);
                    if (String.IsNullOrEmpty(detail.PlanQuantity.ToString()))
                        detailDic.SetValue("PlanQuantity", 0);
                    if (String.IsNullOrEmpty(detail.InvitationQuantity.ToString()))
                        detailDic.SetValue("InvitationQuantity", 0);
                    if (String.IsNullOrEmpty(detail.ContractQuantity.ToString()))
                        detailDic.SetValue("ContractQuantity", 0);
                    if (String.IsNullOrEmpty(detail.ArriveQuantity.ToString()))
                        detailDic.SetValue("ArriveQuantity", 0);
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetTraceList(string EngineeringInfoID, QueryBuilder qb,string ShowAll)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pBomList = this.entities.Set<S_P_Bom>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).Where(qb).ToList();
            var pbsList = engineeringInfo.S_I_PBS.OrderBy(c => c.SortIndex).ToList();
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
                    string delayFields = "";

                    //判定提资计划
                    if (detail.PlanFetchDate.HasValue)
                    {
                        if (detail.FactFetchDate.HasValue && detail.FactFetchDate > detail.PlanFetchDate)
                            delayFields += "PlanFetchDate,FactFetchDate,";
                        else if (!detail.FactFetchDate.HasValue && DateTime.Now > detail.PlanFetchDate.Value.AddDays(1))
                            delayFields += "PlanFetchDate,FactFetchDate,";
                    }
                    if (detail.PlanReturnDate.HasValue)
                    {
                        if (detail.FactReturnDate.HasValue && detail.FactReturnDate > detail.PlanReturnDate)
                            delayFields += "PlanReturnDate,FactReturnDate,";
                        else if (!detail.FactReturnDate.HasValue && DateTime.Now > detail.PlanReturnDate.Value.AddDays(1))
                            delayFields += "PlanReturnDate,FactReturnDate,";
                    }
                    if (detail.PlanInvitationDate.HasValue)
                    {
                        if (detail.FactInvitationDate.HasValue && detail.FactInvitationDate > detail.PlanInvitationDate)
                            delayFields += "PlanInvitationDate,FactInvitationDate,";
                        else if (!detail.FactInvitationDate.HasValue && DateTime.Now > detail.PlanInvitationDate.Value.AddDays(1))
                            delayFields += "PlanInvitationDate,FactInvitationDate,";
                    }
                    if (detail.PlanContractDate.HasValue)
                    {
                        if (detail.FactContractDate.HasValue && detail.FactContractDate > detail.PlanContractDate)
                            delayFields += "PlanContractDate,FactContractDate,";
                        else if (!detail.FactContractDate.HasValue && DateTime.Now > detail.PlanContractDate.Value.AddDays(1))
                            delayFields += "PlanContractDate,FactContractDate,";
                    }
                    if (detail.PlanArrivedDate.HasValue)
                    {
                        if (detail.FactArrivedDate.HasValue && detail.FactArrivedDate > detail.PlanArrivedDate)
                            delayFields += "PlanArrivedDate,FactArrivedDate,";
                        else if (!detail.FactArrivedDate.HasValue && DateTime.Now > detail.PlanArrivedDate.Value.AddDays(1))
                            delayFields += "PlanArrivedDate,FactArrivedDate,";
                    }
                    detailDic.SetValue("DelayFields", delayFields.TrimEnd(','));
                    if (String.IsNullOrEmpty(delayFields))
                    {
                        detailDic.SetValue("Delay", false.ToString().ToLower());
                    }
                    else
                    {
                        detailDic.SetValue("Delay", true.ToString().ToLower());
                    }
                    if (ShowAll != true.ToString().ToLower() && detailDic.GetValue("Delay") == false.ToString().ToLower())
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(detail.Quantity.ToString()))
                        detailDic.SetValue("Quantity", 0);
                    if (String.IsNullOrEmpty(detail.ApplyQuantity.ToString()))
                        detailDic.SetValue("ApplyQuantity", 0);
                    if (String.IsNullOrEmpty(detail.PlanQuantity.ToString()))
                        detailDic.SetValue("PlanQuantity", 0);
                    if (String.IsNullOrEmpty(detail.InvitationQuantity.ToString()))
                        detailDic.SetValue("InvitationQuantity", 0);
                    if (String.IsNullOrEmpty(detail.ContractQuantity.ToString()))
                        detailDic.SetValue("ContractQuantity", 0);
                    if (String.IsNullOrEmpty(detail.ArriveQuantity.ToString()))
                        detailDic.SetValue("ArriveQuantity", 0);
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
        public ActionResult ExportExcel(string EngineeringInfoID, string jsonColumns, string title)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "Bom";
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
            var data = this.entities.Set<S_I_PBS>().Where(d => d.EngineeringInfoID == engineeringInfo.ID).
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
