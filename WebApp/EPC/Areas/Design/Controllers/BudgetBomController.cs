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
using System.IO;

namespace EPC.Areas.Design.Controllers
{
    public class BudgetBomController : EPCController<S_E_Bom_Detail>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            var version = engineeringInfo.S_E_Bom.Where(c => c.BomPhase == BomVersionPhase.预算版本.ToString()).OrderByDescending(c => c.ID).FirstOrDefault();
            bool flowEnd = true;
            if (version == null)
            {
                //此时如果要编辑，则直接做升版BOM操作
                flowEnd = true;
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
            ViewBag.EngineeringInfoID = engineeringInfoID;
            var pushCount = engineeringInfo.S_E_Bom.Count(c => c.FlowPhase == "End" && c.BomPhase == BomVersionPhase.预算版本.ToString());
            ViewBag.PushCount = pushCount;

            //默认展现所有（树节点展开至PBS定义的最下层）
            var list = engineeringInfo.Mode.S_C_PBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            var allDic = new Dictionary<string, object>();
            allDic.SetValue("value", "all");
            allDic.SetValue("text", "全部");
            allDic.SetValue("sortindex", -1);
            enumNodeType.Add(allDic);
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

            var infrasDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = infrasDbContext.S_T_BomDefine.FirstOrDefault(c => c.BelongMode.Contains(engineeringInfo.Mode.ID));
            if (define == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有指定BOM专业，无法进行BOM编辑");
            }
            ViewBag.BomCatagory = define.S_T_BomDefine_Detail.OrderBy(c => c.SortIndex).ToList();

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

            #region 是否启用虚加载

            int detailCount = 0;
            if (version != null)
            {
                var detailCountObj = this.SqlHelper.ExecuteScalar("select count(ID) from S_E_Bom_Detail WHERE VersionID='" + version.ID + "'");
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

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType, string showAllPBS)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            if (version == null) return Json(result);
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            //为了防止数据频繁读取和写入更新BOM表而导致表的死锁，这里改用ADO 读取数据
            var pbsDt = this.SqlHelper.ExecuteDataTable(String.Format(@"SELECT * FROM S_I_PBS WITH(NOLOCK) WHERE EngineeringInfoID='{0}' ORDER BY SORTINDEX", version.EngineeringInfoID));
            var sql = "SELECT * FROM S_E_Bom_Detail WITH(NOLOCK) WHERE VersionID='" + version.ID + "'";
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
            }
            else
            {
                //显示全部数据，体现差异
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
                    //var detailDic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_E_Bom_Detail>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {
                var verionName = BomVersionPhase.预算版本.ToString();
                var lastVersion = this.entities.Set<S_E_Bom>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.BomPhase == verionName && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion == null)
                {
                    var bom = this.GetEntityByID<S_P_Bom>(detail.BomID);
                    if (bom != null)
                    {
                        result = FormulaHelper.ModelToDic<S_P_Bom>(bom);
                    }
                }
                else
                {
                    var lastDetail = lastVersion.S_E_Bom_Detail.FirstOrDefault(c => c.BomID == detail.BomID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_E_Bom_Detail>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_E_Bom>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            this.entities.Set<S_E_Bom>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult UpgradBom(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var newVersion = engineeringInfo.UpgradeBudgetBomVersion();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddVersionDetail(string NodeID, string NodeType, string VersionID, string MajorCode)
        {
            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            if (version == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的BOM清单版本，无法新增"); }
            if (version.FlowPhase == "End")
            {
                throw new Formula.Exceptions.BusinessValidationException("不能对已确认完成的BOM清单版本进行新增操作，请先点击升版按钮进行升版");
            }
            var result = new Dictionary<string, object>();
            var bomMajor = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
            if (bomMajor == null) throw new Formula.Exceptions.BusinessValidationException("未能找到 设备分类专业枚举【Base.BOMMajor】请联系管理员");
            var enumMajor = bomMajor.EnumItem.FirstOrDefault(c => c.Code == MajorCode);
            if (NodeType.ToLower() == "detail")
            {
                var preDetail = version.S_E_Bom_Detail.FirstOrDefault(c => c.ID == NodeID);
                if (preDetail == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("未能找到指定的数据清单，无法再后面插入新行");
                }
                var newDetail = version.AddNewDetail(preDetail);
                if (enumMajor != null)
                {
                    newDetail.MajorCode = MajorCode;
                    newDetail.MajorName = enumMajor.Name;
                }
                result = FormulaHelper.ModelToDic<S_E_Bom_Detail>(newDetail);
                result.SetValue("NodeType", "Detail");
            }
            else
            {
                var pbs = this.GetEntityByID<S_I_PBS>(NodeID);
                //if (pbs.StructNodeInfo.Children.Count > 0)去掉限制(有的项目模式不需要挂pbs子节点下,直接挂在根下面)
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("不能在指定的PBS节点下新增BOM数据，请检查定义，BOM数据只能新增在PBS叶子节点上");
                //}
                var newDetail = version.AddNewDetail(pbs);
                if (enumMajor != null)
                {
                    newDetail.MajorCode = MajorCode;
                    newDetail.MajorName = enumMajor.Name;
                }
                result = FormulaHelper.ModelToDic<S_E_Bom_Detail>(newDetail);
                result.SetValue("NodeType", "Detail");
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult DeleteNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_E_Bom_Detail>(item.GetValue("ID"));
                if (detail == null)
                    continue;
                if (detail.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    this.entities.Set<S_E_Bom_Detail>().Remove(detail);
                }
                else
                {
                    detail.ValidateDelete();
                    detail.ModifyState = BomVersionModifyState.Remove.ToString();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                var detail = this.GetEntityByID<S_E_Bom_Detail>(item.GetValue("ID"));
                if (detail != null)
                {
                    decimal a = 0;
                    if (!decimal.TryParse(item.GetValue("Quantity"), out a))
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.Name + "】的数量必须为大于0的数字");
                    else if (a <= 0)
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.Name + "】的数量必须为大于0的数字");
                    this.UpdateEntity<S_E_Bom_Detail>(detail, item);
                    if (String.IsNullOrEmpty(detail.Name))
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.Name + "】的名称属性不能为空");
                    if (String.IsNullOrEmpty(detail.MajorCode))
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.Name + "】的专业分类属性不能为空");
                    if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                        detail.ModifyState = BomVersionModifyState.Modify.ToString();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertNode(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_E_Bom_Detail>(item.GetValue("ID"));
                if (detail == null) continue;
                var versionID = detail.VersionID;  //记录下版本ID，以免恢复上一条记录时候，将版本ID一起改变
                //如果是新增的BOM记录，则直接删除
                if (detail.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    this.entities.Set<S_E_Bom_Detail>().Remove(detail);
                    var dic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
                    result.Add(dic);
                }
                else
                {
                    //非新增的BOM记录，要去找到上一个审批完成的版本，如果上一个审批完成的版本没有
                    //则获取S_P_Bom 初始化的BOM数据中的内容来恢复，如果都没有，则不恢复。
                    var engineeringInfo = this.GetEntityByID<S_I_Engineering>(detail.EngineeringInfoID);
                    var lastVersion = engineeringInfo.S_E_Bom.Where(c => c.FlowPhase == "End" && c.BomPhase == BomVersionPhase.预算版本.ToString()).OrderByDescending(c => c.ID).FirstOrDefault();
                    if (lastVersion == null)
                    {
                        var bom = this.GetEntityByID<S_P_Bom>(detail.BomID);
                        if (bom != null)
                        {
                            var bomDic = FormulaHelper.ModelToDic<S_P_Bom>(bom);
                            this.UpdateEntity<S_E_Bom_Detail>(detail, bomDic);
                            detail.BomID = bom.ID;
                            detail.VersionID = versionID;
                        }
                    }
                    else
                    {
                        var lastDetail = lastVersion.S_E_Bom_Detail.FirstOrDefault(c => c.BomID == detail.BomID);
                        if (lastDetail != null)
                        {
                            var detailDic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(lastDetail);
                            this.UpdateEntity<S_E_Bom_Detail>(detail, detailDic);
                            detail.VersionID = versionID;
                        }
                    }
                    detail.ModifyState = BomVersionModifyState.Normal.ToString();
                    var dic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult ImportFromBid(string VersionID)
        {
            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本信息，导入失败");
            }
            version.ImportFromTopBidDetail();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetTemplate(string MajorCode, string key)
        {
            string sql = @"select S_T_EquipmentMaterialTemplate.ID,S_T_EquipmentMaterialCategory.Name,
Size,Model,Material,Brand,ConnectionMode,S_T_EquipmentMaterialTemplate.Code Number from dbo.S_T_EquipmentMaterialCategory
left join dbo.S_T_EquipmentMaterialTemplate on S_T_EquipmentMaterialTemplate.CategoryID = S_T_EquipmentMaterialCategory.ID
where Type='Entity' and  S_T_EquipmentMaterialTemplate.ID is not null and S_T_EquipmentMaterialCategory.Code like '" + MajorCode + "%'";
            if (!String.IsNullOrEmpty(key.Trim()))
            {
                sql += " and S_T_EquipmentMaterialCategory.Name like '%" + key + "%'";
            }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = db.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_E_Bom_Detail>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的设备，无法移动设备");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_I_PBS>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");

                sourceNode.ParentID = target.ID;
                sourceNode.PBSNodeFullID = target.FullID;
                sourceNode.PBSNodeID = target.ID;
                var details = this.entities.Set<S_E_Bom_Detail>().Where(c => c.PBSNodeID == target.ID).ToList();
                if (details.Count == 0)
                    sourceNode.SortIndex = 0;
                else
                {
                    var maxSortIndex = details.Max(c => c.SortIndex);
                    sourceNode.SortIndex = maxSortIndex + 0.001;
                }
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_E_Bom_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");

                this.entities.Set<S_E_Bom_Detail>().Where(c => c.PBSNodeID == target.PBSNodeID && c.VersionID == sourceNode.VersionID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                sourceNode.ParentID = target.ParentID;
                sourceNode.PBSNodeFullID = target.PBSNodeFullID;
                sourceNode.PBSNodeID = target.PBSNodeID;
                sourceNode.SortIndex = target.SortIndex - 0.001;

            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_E_Bom_Detail>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");

                this.entities.Set<S_E_Bom_Detail>().Where(c => c.PBSNodeID == target.PBSNodeID && c.VersionID == sourceNode.VersionID
                  && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                sourceNode.ParentID = target.ParentID;
                sourceNode.PBSNodeFullID = target.PBSNodeFullID;
                sourceNode.PBSNodeID = target.PBSNodeID;
                sourceNode.SortIndex = target.SortIndex + 0.001;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        #region Excel导出
        [ValidateInput(false)]
        public ActionResult ExportExcel(QueryBuilder qb, string VersionID, string jsonColumns, string title)
        {
            var columns = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Formula.ImportExport.ColumnInfo>>(jsonColumns);
            var exporter = new MvcAdapter.ImportExport.AsposeExcelExporter();
            byte[] templateBuffer = null;
            var excelKey = "Bid";

            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("未能找到设计BOM清单，无法导出EXCEL");

            if (string.IsNullOrEmpty(title))
            {
                title = version.S_I_Engineering.Name + "_预算设备清单";
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

            var modifyState = BomVersionModifyState.Remove.ToString();
            var versionList = this.entities.Set<S_E_Bom_Detail>().Where(c => c.VersionID == VersionID && c.ModifyState != modifyState).Where(qb).OrderBy(c => c.SortIndex).ToList();
            var tmp = this.entities.Set<S_I_PBS>().Where(d => d.EngineeringInfoID == version.EngineeringInfoID && d.NodeType != "Root").OrderBy(c => c.SortIndex).ToList();
            var pbsNodeTypeEnum = EnumBaseHelper.GetEnumDef("Base.PBSType");

            #region 重排
            Action<List<S_I_PBS>, string, List<S_I_PBS>> f = null;
            f = (res, parentID, source) =>
            {
                var children = source.Where(a => a.ParentID == parentID);
                foreach (var child in children)
                {
                    res.Add(child);
                    f(res, child.ID, source);
                }
            };
            List<S_I_PBS> data = new List<S_I_PBS>();
            var topDatas = tmp.Where(a => a.FullID.Length == tmp.Min(b => b.FullID.Length));
            foreach (var top in topDatas)
            {
                data.Add(top);
                f(data, top.ID, tmp);
            }
            #endregion

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
                    var detailDic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
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
            var versionDic = version.ToDic();
            exporter.OnExport = (sheet) =>
            {
                foreach (var item in versionDic)
                {
                    string val = "";
                    if (item.Value != null)
                        val = item.Value.ToString();
                    SetSignContentToSheet(sheet, "$" + item.Key, val);
                }
            };

            var buffer = exporter.Export(dt, templateBuffer);
            return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
        }

        private void SetSignContentToSheet(Aspose.Cells.Worksheet sheet, string fieldName, string fieldValue)
        {
            IUserService service = FormulaHelper.GetService<IUserService>();
            var cell = sheet.Cells.Find(fieldName, null, new Aspose.Cells.FindOptions() { LookInType = Aspose.Cells.LookInType.Values, LookAtType = Aspose.Cells.LookAtType.StartWith });

            if (cell != null)
            {
                var cellValue = cell.Value.ToString();
                //清除内容
                sheet.Cells.ClearContents(cell.Row, cell.Column, cell.Row, cell.Column);
                //带_sign,则显示签字图片
                if (cellValue.ToLower().Contains("_sign"))
                {
                    byte[] signImg = service.GetSignImg(fieldValue);
                    if (signImg != null)
                    {
                        MemoryStream ms = new MemoryStream(signImg);
                        sheet.Pictures.Add(cell.Row, cell.Column, cell.Row + 1, cell.Column + 1, ms);
                    }
                }
                //显示名称
                else
                {
                    cell.PutValue(fieldValue);
                }
            }
        }
        #endregion

        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var versionID = this.Request["VersionID"];
            var version = this.GetEntityByID<S_E_Bom>(versionID);
            if (version.S_I_Engineering == null) throw new Formula.Exceptions.BusinessValidationException("没有找到相应的工程信息");
            if (version.S_I_Engineering.PBSRoot == null) throw new Formula.Exceptions.BusinessValidationException("工程信息必须具备PBS根节点，请联系管理员");
            var rootStructNode = version.S_I_Engineering.PBSRoot.StructNodeInfo;
            var pbsList = version.S_I_Engineering.S_I_PBS.ToList();
            var pbsStructList = rootStructNode.AllChildren.OrderBy(c => c.FullID).ToList();
            var enumDef = EnumBaseHelper.GetEnumDef("Base.PBSType");
            if (enumDef == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到BOM分类枚举定义 【Base.PBSType】");
            }
            var majorEnumDef = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
            if (majorEnumDef == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到BOM分类枚举定义 【Base.BOMMajor】");
            }
            var errors = excelData.Vaildate(e =>
            {
                if (version == null)
                {
                    e.IsValid = false;
                    e.ErrorText = "没有找到BOM版本记录，无法导入EXCEL";
                }
                else
                {
                    if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "名称不能为空";
                    }
                    else if (e.FieldName == "NodeType")
                    {
                        if (!String.IsNullOrEmpty(e.Value))
                        {
                            var name = e.Record["Name"].ToString();
                            var enumItem = enumDef.EnumItem.FirstOrDefault(c => c.Name == e.Value);
                            if (enumItem == null)
                            {
                                e.IsValid = false;
                                e.ErrorText = "没有这个该类型的结构节点，请确认结构策划";
                            }
                            else if (pbsList.Count(c => c.NodeType == enumItem.Code && c.Name == name) == 0)
                            {
                                e.IsValid = false;
                                e.ErrorText = "没有这个结构内容，请确认结构策划";
                            }
                        }
                    }
                    if (e.FieldName == "MajorName")
                    {
                        if (String.IsNullOrEmpty(e.Value))
                        {
                            if (e.Record["NodeType"] != null && e.Record["NodeType"] != DBNull.Value && !String.IsNullOrEmpty(e.Record["MajorName"].ToString()))
                            {
                                e.IsValid = false;
                                e.ErrorText = "分类专业不能为空";
                            }
                        }
                        else
                        {
                            if (majorEnumDef.EnumItem.Count(c => c.Name == e.Value) == 0)
                            {
                                e.IsValid = false;
                                e.ErrorText = "没有这个专业，请确认该专业分类是否存在";
                            }
                        }
                    }
                }
            });
            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);
            var versionID = this.Request["VersionID"];
            var version = this.GetEntityByID<S_E_Bom>(versionID);
            if (version.S_I_Engineering == null) throw new Formula.Exceptions.BusinessValidationException("没有找到相应的工程信息");
            if (version.S_I_Engineering.PBSRoot == null) throw new Formula.Exceptions.BusinessValidationException("工程信息必须具备PBS根节点，请联系管理员");
            var enumDef = EnumBaseHelper.GetEnumDef("Base.PBSType");
            if (enumDef == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到BOM分类枚举定义 【Base.PBSType】");
            }
            var majorEnumDef = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
            if (majorEnumDef == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到BOM分类枚举定义 【Base.BOMMajor】");
            }
            var rootStructNode = version.S_I_Engineering.PBSRoot.StructNodeInfo;
            var pbsList = version.S_I_Engineering.S_I_PBS.ToList();
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到BOM版本记录，无法导入EXCEL");
            }

            if (rootStructNode == null || rootStructNode.Children.Count == 0)
            {
                //如果定义中PBS节点没有下级的任何节点，则默认没有PBS结构分解，EBOM 直接按专业平面结构存在与系统中
                var sortIndex = version.S_E_Bom_Detail.Count;
                foreach (var item in list)
                {
                    var newDetail = version.AddNewDetail();
                    newDetail.SortIndex = sortIndex;
                    this.UpdateEntity<S_E_Bom_Detail>(newDetail, item);
                }
            }
            else
            {
                //表示结构定义中有PBS存在，则必须要根据PBS结构来挂接 EBOM 数据
                var pbsStructList = rootStructNode.AllChildren.OrderBy(c => c.FullID).ToList();
                var lastSturctInfoID = pbsStructList.LastOrDefault().ID;
                var structDic = new Dictionary<string, S_I_PBS>();
                foreach (var item in pbsStructList)
                {
                    structDic.SetValue(item.ID, null);
                }
                var sortIndex = version.S_E_Bom_Detail.Count;
                foreach (var item in list)
                {
                    var nodeType = item.GetValue("NodeType");
                    sortIndex++;
                    if (String.IsNullOrEmpty(nodeType))
                    {
                        var pbs = structDic.GetValue(lastSturctInfoID);
                        if (pbs != null)
                        {
                            var newDetail = version.AddNewDetail(pbs, false);
                            newDetail.SortIndex = sortIndex;
                            this.UpdateEntity<S_E_Bom_Detail>(newDetail, item);
                            var majorEnumItem = majorEnumDef.EnumItem.FirstOrDefault(c => c.Name == newDetail.MajorName);
                            if (majorEnumItem != null)
                            {
                                newDetail.MajorCode = majorEnumItem.Code;
                                newDetail.MajorName = majorEnumItem.Name;
                            }
                        }
                    }
                    else
                    {
                        var name = item.GetValue("Name");
                        var enumItem = enumDef.EnumItem.FirstOrDefault(c => c.Name == nodeType);
                        if (enumItem == null) continue;
                        var structNode = pbsStructList.FirstOrDefault(c => c.NodeType == enumItem.Code);
                        if (structNode == null) continue;
                        if (structNode.Parent != null && structNode.Parent.NodeType != "Root")
                        {
                            var parentPBS = structDic.GetValue(structNode.ParentID);
                            if (parentPBS != null)
                            {
                                var pbs = parentPBS.Children.FirstOrDefault(c => c.Name == name && c.StructNodeID == structNode.ID);
                                structDic.SetValue(structNode.ID, pbs);
                            }
                        }
                        else
                        {
                            var pbs = pbsList.FirstOrDefault(c => c.NodeType == enumItem.Code && c.Name == name);
                            structDic.SetValue(structNode.ID, pbs);
                            foreach (var structChild in structNode.AllChildren)
                            {
                                structDic.SetValue(structChild.ID, null);
                            }
                        }
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("Success");
        }
        #endregion
    }
}
