using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Design.Controllers
{
    public class BudgetBOQController : EPCController
    {
        public ActionResult List()
        {
            //ViewBag.IsEdit = GetQueryString("IsEdit");

            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的项目信息");

            bool flowEnd = true;
            var version = entities.Set<S_C_BOQ_Version>()
                .Where(a => a.EngineeringInfoID == engineeringInfoID)
                .OrderByDescending(a => a.ID).FirstOrDefault();

            //没有发布任何版本
            if (version == null)
            {
                ViewBag.VersionID = "";
                ViewBag.VersionNo = "0";
                flowEnd = true;
            }
            else
            {
                if (version.FlowPhase != "End")
                    flowEnd = false;
                ViewBag.FlowPhase = version.FlowPhase;
                ViewBag.VersionID = version.ID;
                ViewBag.VersionNo = version.VersionNumber;
            }

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

            ViewBag.FlowEnd = flowEnd;
            return View();
        }

        public JsonResult GetTreeList(QueryBuilder qb, string VersionID, string ShowType, string showAllPBS)
        {
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            var version = entities.Set<S_C_BOQ_Version>().Find(VersionID);
            if (version == null)
            {
                return Json("");
            }
            var result = GetSearchList(qb, VersionID, ShowType, showAllPBS);            
            return Json(result);
        }

        public JsonResult Upgrade(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的项目信息");

            if (engineeringInfo.S_C_BOQ_Version.Count(c => c.FlowPhase != "End") > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("尚存在未发布的工程量清单数据，无法再次升版");
            }
           
            var userInfo = FormulaHelper.GetUserInfo();
            var lastVersion = engineeringInfo.S_C_BOQ_Version.Where(c => c.FlowPhase == "End" && c.BOQType == BomVersionPhase.预算版本.ToString()).
                OrderByDescending(c => c.ID).FirstOrDefault();
            var newVersion = new S_C_BOQ_Version();
            newVersion.ID = FormulaHelper.CreateGuid();
            newVersion.CreateDate = DateTime.Now;
            newVersion.CreateUser = userInfo.UserName;
            newVersion.CreateUserID = userInfo.UserID;
            newVersion.EngineeringInfoID = engineeringInfo.ID;
            newVersion.FlowPhase = "Start";
            newVersion.FlowInfo = "";
            if (lastVersion != null)
            {
                //var versionNum = String.IsNullOrEmpty(lastVersion.VersionNumber) || !Regex.IsMatch(lastVersion.VersionNumber, @"[1-9]\d*$") ? 0 :
                //    Convert.ToInt32(lastVersion.VersionNumber);
                var versionNum = lastVersion.VersionNumber;
                newVersion.VersionNumber = versionNum + 1;
                var list = entities.Set<S_C_BOQ_Version_Detail>().Where(c => c.ModifyState != "Remove" && c.VersionID == lastVersion.ID).ToList();

                foreach (var item in list)
                {
                    var detail = item.Clone<S_C_BOQ_Version_Detail>();
                    detail.ModifyState = "Normal";
                    detail.VersionID = newVersion.ID;
                    entities.Set<S_C_BOQ_Version_Detail>().Add(detail);
                }
            }
            //首次编辑升版
            else
            {                
                newVersion.VersionNumber = 1;
            }
            newVersion.VersionName = "第【" + newVersion.VersionNumber + "】版工程量清单";
            newVersion.BOQType = BomVersionPhase.预算版本.ToString();
            entities.Set<S_C_BOQ_Version>().Add(newVersion);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddItem(string targetID, string NodeType, string versionID)
        {
            var version = this.GetEntityByID<S_C_BOQ_Version>(versionID);
            if (version == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的BOQ清单版本，无法新增"); }
            if (version.FlowPhase == "End")
            {
                throw new Formula.Exceptions.BusinessValidationException("不能对已确认完成的BOQ清单版本进行新增操作，请先点击升版按钮进行升版");
            }
            var result = new Dictionary<string, object>();
            if (NodeType.ToLower() == "detail")
            {
                var preDetail = version.S_C_BOQ_Version_Detail.FirstOrDefault(c => c.ID == targetID);
                if (preDetail == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("未能找到指定的数据清单，无法再后面插入新行");
                }
                var newDetail = version.AddNewDetail(preDetail);
                result = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(newDetail);
                result.SetValue("NodeType", "Detail");
            }
            else
            {
                var pbs = this.GetEntityByID<S_I_PBS>(targetID);
                //if (pbs.StructNodeInfo.Children.Count > 0)去掉限制(有的项目模式不需要挂pbs子节点下,直接挂在根下面)
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("不能在指定的PBS节点下新增BOM数据，请检查定义，BOM数据只能新增在PBS叶子节点上");
                //}
                var newDetail = version.AddNewDetail(pbs);
                result = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(newDetail);
                result.SetValue("NodeType", "Detail");
            }
            entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveNodes(string listData, string versionID)
        {
            var list = JsonHelper.ToList(listData);

            foreach (var item in list)
            {
                string dataStateFromUICtrl = item.GetValue("_state").ToLower();
                var data = this.GetEntityByID<S_C_BOQ_Version_Detail>(item.GetValue("ID"));

                if (dataStateFromUICtrl == "removed")
                {
                    if (data != null)
                    {
                        //删除的数据就是该未发布版(暂存)新增数据
                        //则直接删除数据库数据
                        if (data.ModifyState == "Add")
                        {
                            string id = item.GetValue("ID");
                            entities.Set<S_C_BOQ_Version_Detail>().Delete(a => a.ID == id);
                        }
                        else
                        {
                            data.ModifyState = "Remove";
                        }
                    }
                }
                else if (dataStateFromUICtrl == "modified")
                {
                    if (data != null)
                    {
                        this.UpdateEntity<S_C_BOQ_Version_Detail>(data, item);
                        //Add的部分始终保持为add的state,不能混为修改项
                        //只找normal的state变为modify
                        if (data.ModifyState == "Normal")
                            data.ModifyState = "Modify";
                    }
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetLastDetailInfo(string ID, string VersionID)
        {
            var detail = this.GetEntityByID<S_C_BOQ_Version_Detail>(ID);
            var currDetailVersion = entities.Set<S_C_BOQ_Version>().Find(detail.VersionID);
            var result = new Dictionary<string, object>();
            if (detail != null && currDetailVersion != null)
            {
                var lastVersion = entities.Set<S_C_BOQ_Version>()
                    .Where(d => d.EngineeringInfoID == currDetailVersion.EngineeringInfoID && d.FlowPhase == "End" && d.VersionNumber < currDetailVersion.VersionNumber)
                    .OrderByDescending(c => c.VersionNumber).FirstOrDefault();

                if (lastVersion != null)
                {
                    var lastDetail = entities.Set<S_C_BOQ_Version_Detail>().FirstOrDefault(a => a.BOQID == detail.BOQID && a.VersionID == lastVersion.ID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_C_BOQ_Version>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            this.entities.Set<S_C_BOQ_Version_Detail>().Delete(a => a.VersionID == VersionID);
            this.entities.Set<S_C_BOQ_Version>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertNode(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                var detail = entities.Set<S_C_BOQ_Version_Detail>().Find(item.GetValue("ID"));
                if (detail == null) continue;
                var versionID = detail.VersionID;  //记录下版本ID，以免恢复上一条记录时候，将版本ID一起改变
                //如果是新增的记录，则直接删除
                if (detail.ModifyState == "Add")
                {
                    entities.Set<S_C_BOQ_Version_Detail>().Remove(detail);
                    var dic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(detail);
                    result.Add(dic);
                }
                else
                {
                    string engineeringInfoID = this.GetQueryString("EngineeringInfoID");

                    var lastVersion = entities.Set<S_C_BOQ_Version>().Where(a => a.FlowPhase == "End" && a.EngineeringInfoID == engineeringInfoID).OrderByDescending(a => a.ID).FirstOrDefault();
                    if (lastVersion != null)
                    {
                        var lastDetail = entities.Set<S_C_BOQ_Version_Detail>().Find(detail.BOQID);
                        if (lastDetail != null)
                        {
                            var detailDic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(lastDetail);
                            this.UpdateEntity<S_C_BOQ_Version_Detail>(detail, detailDic);
                            detail.VersionID = versionID;
                        }
                    }
                    detail.ModifyState = "Normal";
                    var dic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(detail);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult MoveItem(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_C_BOQ_Version_Detail>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");
            if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_C_BOQ_Version_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");

                S_C_BOQ_Version_Detail targetPre = entities.Set<S_C_BOQ_Version_Detail>()
                    .Where(a => a.ParentID == target.ParentID && a.SortIndex < target.SortIndex)
                    .OrderByDescending(a => a.SortIndex).FirstOrDefault();

                if (targetPre != null)
                {
                    sourceNode.SortIndex = (targetPre.SortIndex + target.SortIndex) / 2;
                }
                else
                {
                    sourceNode.SortIndex = target.SortIndex - 1;
                }

            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_C_BOQ_Version_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定内容,无法移动");

                S_C_BOQ_Version_Detail targetNext = entities.Set<S_C_BOQ_Version_Detail>()
                    .Where(a => a.ParentID == target.ParentID && a.SortIndex > target.SortIndex)
                    .OrderBy(a => a.SortIndex).FirstOrDefault();

                if (targetNext != null)
                {
                    sourceNode.SortIndex = (targetNext.SortIndex + target.SortIndex) / 2;
                }
                else
                {
                    sourceNode.SortIndex = target.SortIndex + 1;
                }
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        [ValidateInput(false)]
        public ActionResult ExportExcel(string VersionID, string jsonColumns, string title)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "BudgetBOQ";

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

            var detailList = GetSearchList(null, VersionID, "", "true");
            List<Dictionary<string, object>> dicList = FormulaHelper.CollectionToListDic(detailList);

            foreach (var dic in dicList)
            {
                var detailRow = dt.NewRow();

                foreach (var detailColumn in columns)
                {
                    if (detailColumn.FieldName == "Name")
                    {
                        string space = "";
                        if (!string.IsNullOrEmpty(dic.GetValue("CBSFullID")))
                        {
                            int spaceCount = dic.GetValue("CBSFullID").Split(',').Length;
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

            dt.TableName = "BudgetBOQ";
            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }

        private List<Dictionary<string, object>> GetSearchList(QueryBuilder qb, string VersionID, string ShowType, string showAllPBS)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_C_BOQ_Version>(VersionID);
            if (version == null) return result;

            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pbsDt = this.SqlHelper.ExecuteDataTable(String.Format(@"SELECT * FROM S_I_PBS WITH(NOLOCK) WHERE EngineeringInfoID='{0}' ORDER BY SORTINDEX", version.EngineeringInfoID));
            var sql = "SELECT * FROM S_C_BOQ_Version_Detail WITH(NOLOCK) WHERE VersionID='" + VersionID + "'";


            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, "Normal");

            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, "Remove");
            }
            else
            {

            }

            var versionDt = this.SqlHelper.ExecuteDataTable(sql, qb);
            foreach (DataRow pbsRow in pbsDt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(pbsRow);
                var details = versionDt.Select("PBSNodeID='" + dic.GetValue("ID") + "'");//.Where(c => c.PBSNodeID == item.ID).ToList();
                //判定是否要过滤掉没有设备材料的PBS节点
                if (!String.IsNullOrEmpty(showAllPBS) && showAllPBS.ToLower() == "false"
                    && versionDt.Select("PBSNodeFullID like '" + dic.GetValue("FullID") + "%'").Length == 0)
                {
                    continue;
                }
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.DataRowToDic(detail);
 
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }

            return result;
        }

        private double GetMaxOrderIndex()
        {
            var last = entities.Set<S_C_BOQ_Version_Detail>().OrderByDescending(a => a.SortIndex).FirstOrDefault();
            if (last != null)
            {
                return last.SortIndex;
            }
            return 0;
        }
    }
}
