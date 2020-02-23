using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class WorkSpaceController : ProjectController<S_E_Product>
    {

        public ActionResult CooperationPlanSpace()
        {
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoID");
            ViewBag.SpaceCode = GetQueryString("SpaceCode");
            return View();
        }

        public JsonResult GetWBSTree(string ProjectInfoID, string SpaceCode, string TreeViewType, string ShowOwnNode)
        {
            string treeViewType = GetQueryString("TreeViewType");
            if (string.IsNullOrEmpty(treeViewType))
                treeViewType = WBSNodeType.Major.ToString();
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目信息对象，无法展现工作区");
            var wbsFO = FormulaHelper.CreateFO<WBSFO>();
            var result = new List<Dictionary<string, object>>();
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            var filterAuthNode = false;
            if (ShowOwnNode == "True")
            {
                filterAuthNode = true;
            }
            result = wbsFO.CreateWBSTree(ProjectInfoID, treeViewType, true, SpaceCode, this.CurrentUserInfo.UserID, filterAuthNode, false);
            var taskList = this.entities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfo.ID && a.MajorValue == SpaceCode).ToList();
            foreach (var item in result)
            {
                var wbsid = item.GetValue("ID");
                var task = taskList.FirstOrDefault(a => a.WBSID == wbsid);
                item.SetValue("ChangeState", "");
                if (task != null)
                {
                    item.SetValue("ChangeState", task.ChangeState);
                    var passStr = AuditState.Pass.ToString();
                }
            }
            return Json(result);
        }


        public JsonResult GetAuditList(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS对象");
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            var wbsidlists = wbs.AllChildren.Select(a => a.ID).ToList();
            wbsidlists.Add(WBSID);
            var spaceCode = this.GetQueryString("SpaceCode");
            var data = entites.T_EXE_Audit.Where(d => wbsidlists.Contains(d.WBSID) && d.MajorCode == spaceCode).WhereToGridData(qb);
            return Json(data);
        }


        public JsonResult GetChangeAuditList(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS对象");
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            var wbsidlists = wbs.AllChildren.Select(a => a.ID).ToList();
            wbsidlists.Add(WBSID);
            var spaceCode = this.GetQueryString("SpaceCode");
            var data = entites.T_EXE_ChangeAudit.Where(d => wbsidlists.Contains(d.WBSID) && d.MajorCode == spaceCode).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetMettingList(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS对象");
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            var wbsidlists = wbs.AllChildren.Select(a => a.ID).ToList();
            wbsidlists.Add(WBSID);
            var spaceCode = this.GetQueryString("SpaceCode");
            var data = entites.T_EXE_MettingSign.Where(d => wbsidlists.Contains(d.WBSID) && d.Major == spaceCode).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetDirectoryList(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS对象");
            var wbsidlists = wbs.AllChildren.Select(a => a.ID).ToList();
            wbsidlists.Add(WBSID);
            var spaceCode = this.GetQueryString("SpaceCode");
            var directoryList = this.entities.Set<S_E_ProductDirectory>().Where(d => wbsidlists.Contains(d.WBSID));
            string treeViewType = GetQueryString("TreeViewType");
            if (string.IsNullOrEmpty(treeViewType))
                treeViewType = WBSNodeType.Major.ToString();
            var majorValueStr = string.Empty;
            if (treeViewType == WBSNodeType.Major.ToString())
                directoryList = directoryList.Where(a => a.MajorValue == spaceCode);
            return Json(directoryList.OrderBy(d => d.Index).WhereToGridData(qb));
        }

        public void DeleteDirectory(string rows)
        {
            var list = JsonHelper.ToList(rows);
            foreach (var item in list)
            {
                var id = item["ID"].ToString();
                this.entities.Set<S_E_ProductDirectory>().Delete(d => d.ID == id);
            }
            this.entities.SaveChanges();
        }

        public void MoveRows(string rows, string wbsID, string direction)
        {
            var list = JsonHelper.ToList(rows);
            var resultDirectorys = this.entities.Set<S_E_ProductDirectory>().Where(d => d.WBSID == wbsID).ToList();
            var num = list.Count;
            List<string> idList = new List<string>();
            List<string> unRunList = new List<string>();
            foreach (var item in list)
            {
                idList.Add(item["ID"].ToString());
            }
            unRunList.AddRange(idList);

            if (direction.ToLower() == "up")
            {
                var minIndex = resultDirectorys.Min(d => d.Index);
                foreach (var item in list)
                {
                    var directoryID = item["ID"].ToString();
                    unRunList.Remove(directoryID);
                    var itemInfo = this.GetEntityByID<S_E_ProductDirectory>(directoryID);
                    var upRows = resultDirectorys.Where(d => d.Index < itemInfo.Index).ToList();
                    var upRowIDs = upRows.Select(m => m.ID);
                    if (!(itemInfo.Index == minIndex || upRowIDs.All(d => unRunList.Contains(d))))
                    {
                        var upRow = resultDirectorys.OrderByDescending(d => d.Index).FirstOrDefault(d => d.Index < itemInfo.Index && !unRunList.Contains(d.ID));
                        if (upRow != null)
                        {
                            var tmpIndex = upRow.Index;
                            upRow.Index = itemInfo.Index;
                            itemInfo.Index = tmpIndex;
                        }
                    }
                }
            }
            else if (direction.ToLower() == "down")
            {
                var maxIndex = resultDirectorys.Max(d => d.Index);
                foreach (var item in list)
                {
                    var directoryID = item["ID"].ToString();
                    unRunList.Remove(directoryID);
                    var itemInfo = this.GetEntityByID<S_E_ProductDirectory>(directoryID);
                    var upRows = resultDirectorys.Where(d => d.Index > itemInfo.Index).ToList();
                    var upRowIDs = upRows.Select(m => m.ID);
                    if (!(itemInfo.Index == maxIndex || upRowIDs.All(d => unRunList.Contains(d))))
                    {
                        var upRow = resultDirectorys.OrderBy(d => d.Index).FirstOrDefault(d => d.Index > itemInfo.Index && !unRunList.Contains(d.ID));
                        if (upRow != null)
                        {
                            var tmpIndex = upRow.Index;
                            upRow.Index = itemInfo.Index;
                            itemInfo.Index = tmpIndex;
                        }
                    }
                }
            }
            this.entities.SaveChanges();
        }

        public void AddProductToDirectory(string rows)
        {
            var list = JsonHelper.ToList(rows);
            var wbsid = GetQueryString("WBSID");
            var pdList = this.entities.Set<S_E_ProductDirectory>().Where(a => a.WBSID == wbsid).ToList();
            foreach (var item in list)
            {
                var productId = item["ID"].ToString();
                var productDirectory = pdList.FirstOrDefault(d => d.ProductID == productId);
                if (productDirectory == null)
                {
                    productDirectory = new S_E_ProductDirectory();
                    productDirectory.ID = FormulaHelper.CreateGuid();
                    productDirectory.ProductID = productId;
                    productDirectory.ProjectInfoID = item.GetValue("ProjectInfoID");
                    productDirectory.WBSID = item.GetValue("WBSID");
                    productDirectory.MajorValue = item.GetValue("MajorValue");

                    pdList.Add(productDirectory);
                    this.entities.Set<S_E_ProductDirectory>().Add(productDirectory);
                }

                productDirectory.Name = item.GetValue("Name");
                productDirectory.Code = item.GetValue("Code");
                decimal? version = null;
                if (!string.IsNullOrEmpty(item.GetValue("Version")))
                    version = decimal.Parse(item.GetValue("Version"));
                productDirectory.Version = version;
                productDirectory.FileSize = item.GetValue("FileSize");
                productDirectory.Description = item.GetValue("Description");

                var maxIndex = pdList.Max(d => d.Index);
                if (maxIndex.HasValue)
                    productDirectory.Index = maxIndex + 1;
                else
                    productDirectory.Index = 0;
            }
            this.entities.SaveChanges();

        }

        #region  设计区

        public ActionResult DesginMain()
        {
            string showDirectory = String.IsNullOrEmpty(this.GetQueryString("ShowDir")) ? "True" : this.GetQueryString("ShowDir");
            string showMeeting = String.IsNullOrEmpty(this.GetQueryString("ShowMeetingSign")) ? "True" : this.GetQueryString("ShowMeetingSign");
            string singleMajorMode = String.IsNullOrEmpty(this.GetQueryString("SingleMajor")) ? "False" : this.GetQueryString("SingleMajor");
            string majorCode = this.GetQueryString("SpaceCode");


            ViewBag.ShowDir = showDirectory;
            ViewBag.ShowMeetingSign = showMeeting;
            ViewBag.SingleMajor = singleMajorMode;
            ViewBag.MajorCode = majorCode;

            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(this.GetQueryString("ProjectInfoID"));
            if (projectInfo == null)
                throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            if (singleMajorMode.ToLower() == "true")
            {
                var majorNode = projectInfo.S_W_WBS.FirstOrDefault(d => d.WBSValue == majorCode && d.WBSType == WBSNodeType.Major.ToString());
                if (majorNode == null)
                {
                    ViewBag.DisplayError = "True";
                    throw new Formula.Exceptions.BusinessException("策划尚未完成，无法进入项目设计工作区，没有找到对应的专业");
                }
                else
                    ViewBag.WBSID = majorNode.ID;
            }
            //是否应用卷册变更模式，校审后不能升版，需发起变更申请
            ViewBag.TaskWorkDesingChangeMode = projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TaskWorkDesingChange");
            ViewBag.ModeCode = projectInfo.ModeCode;
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }

        public ActionResult BatchAdd()
        {
            string wbsID = GetQueryString("WBSID");
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
            if (wbs.Parent == null) throw new Formula.Exceptions.BusinessException("未能找到子项节点，不能添加成果");
            var monomerEnum = wbs.Parent.S_W_Monomer.OrderBy(d => d.Name).Select(d => new { text = d.Name, value = d.Name }).ToList();
            ViewBag.Monomer = JsonHelper.ToJson(monomerEnum);
            ViewBag.WBSInfo = JsonHelper.ToJson(wbs);
            var phaseCode = wbs.PhaseCode.Split(',').ToList();
            var allPhase = EnumBaseHelper.GetEnumDef("Project.Phase");
            var phase = allPhase.EnumItem.Where(a => phaseCode.Contains(a.Code)).Select(a => new { text = a.Name, value = a.Code });
            ViewBag.Phase = JsonHelper.ToJson(phase);
            return View();
        }

        public ActionResult WorkSpace()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            string spaceCode = GetQueryString("SpaceCode");
            var TreeViewType = String.IsNullOrEmpty(GetQueryString("TreeViewType")) ? "Major" : GetQueryString("TreeViewType");
            var projectInfo = entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + projectInfoID + "】的项目！");
            if (projectInfo.WBSRoot == null) throw new Formula.Exceptions.BusinessException("找不到项目对应的WBS根节点！");
            ViewBag.ProjectInfo = projectInfo;
            ViewBag.SpaceCode = spaceCode;
            ViewBag.TreeViewType = TreeViewType;
            ViewBag.TaskWorkDesingChangeMode = projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TaskWorkDesingChange");
            return View();
        }

        #region 数字化出版出图单增加新成果
        public ActionResult WBSTree()
        {
            return View();
        }
        #endregion

        public JsonResult SaveProducts(string ListData)
        {
            string wbsid = GetQueryString("WBSID");
            var wbs = GetEntityByID<S_W_WBS>(wbsid);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + wbsid + "】的wbs");
            if (wbs.WBSType == WBSNodeType.Work.ToString())
            {
                if (wbs.S_I_ProjectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TaskWorkDesingChange") == TrueOrFalse.True.ToString())
                {
                    var task = wbs.S_W_TaskWork.FirstOrDefault();
                    if (task == null)
                        throw new Formula.Exceptions.BusinessException("卷册S_W_TaskWork对象为空");
                    if (task.ChangeState == TaskWorkChangeState.AuditApprove.ToString() || task.ChangeState == TaskWorkChangeState.AuditFinish.ToString())
                        throw new Formula.Exceptions.BusinessException("变更通知中及变更通知完成的卷册不能新增成果");
                }
            }
            var listdata = UpdateList<S_E_Product>(ListData);
            var productIDs = new List<string>();
            foreach (var item in listdata)
            {
                if (wbs.S_E_Product.FirstOrDefault(d => d.Code == item.Code) != null)
                    throw new Formula.Exceptions.BusinessException("编号“" + item.Code + "”重复，请重新输入");
                wbs.SaveProduct(item);
                productIDs.Add(item.ID);
            }
            entities.SaveChanges();
            var sql = @"select *,case when AuditSignUser is null then 1 else 0 end as CanSetUser from S_E_Product
cross apply(select top 1 ID as VersionID from S_E_ProductVersion
where S_E_Product.ID = S_E_ProductVersion.ProductID
and S_E_Product.Version = S_E_ProductVersion.Version) as pv
where S_E_Product.ID in ('" + string.Join("','", productIDs) + "')";
            var rtn = this.SqlHelper.ExecuteDataTable(sql);
            return Json(rtn);
        }

        protected override void BeforeDelete(List<S_E_Product> entityList)
        {
            foreach (var product in entityList)
            {
                if (product.AuditState != AuditState.Create.ToString()
                    && product.AuditState != AuditState.Design.ToString()
                    && product.AuditState != AuditState.Designer.ToString())
                {
                    throw new Formula.Exceptions.BusinessException("已经发起校审的成果不能删除。");
                }
                else if (product.State == ProductState.Change.ToString())
                    throw new Formula.Exceptions.BusinessException("已经变更的成果不能删除。");
                else if (product.State == ProductState.Invalid.ToString() || product.State == ProductState.InInvalid.ToString())
                    throw new Formula.Exceptions.BusinessException("已经作废的成果不能删除。");
                else if (product.Version != 1)
                {
                    throw new Formula.Exceptions.BusinessException("已经升版的成果不能删除。");
                }
            }
        }

        public JsonResult GetProductList(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            qb.Add("State", QueryMethod.NotEqual, ProductState.Invalid.ToString());
            var spaceCode = this.GetQueryString("SpaceCode");
            string treeViewType = GetQueryString("TreeViewType");
            if (string.IsNullOrEmpty(treeViewType))
                treeViewType = WBSNodeType.Major.ToString();
            var majorValueStr = string.Empty;
            if (treeViewType == WBSNodeType.Major.ToString())
                majorValueStr += " and MajorValue = '" + spaceCode + "'";
            var sql = @"
select *,'Product' Type from S_E_Product
where ProjectInfoID = '{0}'
and WBSFullID like '%{1}%' {2}
and (ParentID is null or ParentID = '' or ParentID = 'SPLIT')";
            sql = String.Format(sql, ProjectInfoID, WBSID, majorValueStr);
            var dataTable = this.SqlHelper.ExecuteDataTable(sql, qb);
            var list = FormulaHelper.DataTableToListDic(dataTable);
            var subSql = @"
select *,'SubProduct' Type from S_E_Product
where ProjectInfoID = '{0}'
and WBSFullID like '%{1}%'
and ParentID = '{2}' 
and ParentVersion = (select Version from S_E_Product where ID = '{2}')
";
            var allsubsql = "";
            foreach (var item in list)
            {
                allsubsql += String.Format(subSql, ProjectInfoID, WBSID, item["ID"].ToString()) + "union all";
            }
            if (!string.IsNullOrEmpty(allsubsql))
            {
                var subDataTable = this.SqlHelper.ExecuteDataTable(allsubsql.TrimEnd('l', 'l', 'a', ' ', 'n', 'o', 'i', 'n', 'u'));
                var subList = FormulaHelper.DataTableToListDic(subDataTable);
                subList.ForEach(a => list.Add(a));
            }
            var result = new Dictionary<string, object>();
            result["data"] = list;
            result["total"] = qb.TotolCount;
            string changeState = string.Empty;
            var task = this.entities.Set<S_W_TaskWork>().FirstOrDefault(a => a.WBSID == WBSID);
            if (task != null && !string.IsNullOrEmpty(task.ChangeState))
                changeState = task.ChangeState;
            result["changeState"] = changeState;
            return Json(result);
        }

        public JsonResult DesignChangeApplyValidate(string WBSID)
        {
            var task = this.entities.Set<S_W_TaskWork>().FirstOrDefault(a => a.WBSID == WBSID);
            if (task == null)
                throw new Formula.Exceptions.BusinessException("卷册未找到。");
            if (!string.IsNullOrEmpty(task.ChangeState) && task.ChangeState != TaskWorkChangeState.AuditFinish.ToString())
                throw new Formula.Exceptions.BusinessException("卷册已经发起变更申请中，无需再次发起");
            if (!this.entities.Set<T_EXE_Audit>().Any(a => a.WBSID == WBSID && a.FlowPhase == "End"))
                throw new Formula.Exceptions.BusinessException("至少有一个成果完成校审才能发起变更申请");
            if (this.entities.Set<T_EXE_Audit>().Any(a => a.WBSID == WBSID && a.FlowPhase != "End"))
                throw new Formula.Exceptions.BusinessException("有成果正在校审中，无法发起变更申请");
            return Json("");
        }

        public JsonResult DesignChangeAuditValidate(string WBSID)
        {
            var task = this.entities.Set<S_W_TaskWork>().FirstOrDefault(a => a.WBSID == WBSID);
            if (task == null)
                throw new Formula.Exceptions.BusinessException("卷册未找到。");
            if (task.ChangeState != TaskWorkChangeState.ApplyFinish.ToString())
                throw new Formula.Exceptions.BusinessException("只有变更申请完成的卷册才能发起变更通知");
            var passState = AuditState.Pass.ToString();
            var invalidState = ProductState.InInvalid.ToString();
            if (task.S_W_WBS.S_E_Product.Count(a => a.AuditState != passState || a.State == invalidState) == 0)
                throw new Formula.Exceptions.BusinessException("没有成果需要提交变更通知");
            if (this.entities.Set<T_EXE_ChangeAudit>().Any(a => a.WBSID == WBSID && a.FlowPhase != "End"))
                throw new Formula.Exceptions.BusinessException("请先结束已经发起的变更通知单");
            return Json("");
        }

        public JsonResult ProductChangeValidate(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            if (entity.S_W_WBS.WBSType == WBSNodeType.Work.ToString())
            {
                var task = entity.S_W_WBS.S_W_TaskWork.FirstOrDefault();
                if (task != null && task.ChangeState != TaskWorkChangeState.ApplyFinish.ToString()
                    && task.ChangeState != TaskWorkChangeState.AuditStart.ToString())
                    throw new Formula.Exceptions.BusinessException("只有卷册变更申请完成，成果才能进行变更");
            }
            if (entity.AuditState != AuditState.Pass.ToString())
                throw new Formula.Exceptions.BusinessException("只有校审通过的成果才能变更");
            if (entity.State != ProductState.Create.ToString())
                throw new Formula.Exceptions.BusinessException("只有未变更或未作废的成果才能变更");
            return Json("");
        }
        public JsonResult ProductRevertChangeValidate(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            if (entity.State != ProductState.Change.ToString())
                throw new Formula.Exceptions.BusinessException("只有变更的成果才能撤销变更");
            return Json("");
        }

        public JsonResult ProductInvalidValidate(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            if (entity.S_W_WBS.WBSType == WBSNodeType.Work.ToString())
            {
                var task = entity.S_W_WBS.S_W_TaskWork.FirstOrDefault();
                if (task != null && task.ChangeState != TaskWorkChangeState.ApplyFinish.ToString()
                    && task.ChangeState != TaskWorkChangeState.AuditStart.ToString())
                    throw new Formula.Exceptions.BusinessException("只有卷册变更申请完成，成果才能进行作废");
            }
            if (entity.AuditState != AuditState.Pass.ToString())
                throw new Formula.Exceptions.BusinessException("只有校审通过的成果才能作废");
            if (entity.State != ProductState.Create.ToString())
                throw new Formula.Exceptions.BusinessException("只有未变更或未作废的成果才能作废");
            return Json("");
        }
        public JsonResult ProductRevertInvalidValidate(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            if (entity.State != ProductState.InInvalid.ToString())
                throw new Formula.Exceptions.BusinessException("只有作废的成果才能撤销作废");
            return Json("");
        }

        public JsonResult IsProductCanAudit(string productIDs)
        {
            var wbsid = this.GetQueryString("WBSID");
            var isSimple = this.GetQueryString("IsSimple") == "true";
            var wbs = this.GetEntityByID<S_W_WBS>(wbsid);
            if (wbs == null)
                throw new Formula.Exceptions.BusinessException("WBS对象不存在，请联系管理员！");
            if (wbs.WBSType == WBSNodeType.Work.ToString())
            {
                if (wbs.S_I_ProjectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TaskWorkDesingChange") == TrueOrFalse.True.ToString())
                {
                    var task = wbs.S_W_TaskWork.FirstOrDefault();
                    if (task != null && !string.IsNullOrEmpty(task.ChangeState))
                        throw new Formula.Exceptions.BusinessException("只有未变更的卷册，成果才能发起校审");
                }
            }
            var products = this.entities.Set<S_E_Product>().Where(c => productIDs.Contains(c.ID)).ToList();
            var phaseValue = "";
            var batchID = FormulaHelper.CreateGuid();
            foreach (var product in products)
            {
                if (isSimple && !string.IsNullOrEmpty(product.ParentID) && product.ParentID.ToLower() != "split")
                    continue;
                if (product.IsAudited())
                    throw new Formula.Exceptions.BusinessException("图号为【" + product.Code + "】的成果已经校审，不可再次发起校审。");
                if (string.IsNullOrEmpty(phaseValue))
                    phaseValue = product.PhaseValue;
                else
                    if (phaseValue != product.PhaseValue)
                        throw new Formula.Exceptions.BusinessException("不同阶段的成果不可同时发起校审！");
                product.BatchID = batchID;
            }
            this.entities.SaveChanges();
            var projectBaseEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var auditModeInfo = projectBaseEntities.Set<S_T_AuditMode>().FirstOrDefault(d => d.ProjectModeID == wbs.S_I_ProjectInfo.ProjectMode.ID
                && d.PhaseValue == phaseValue);
            var result = new Dictionary<string, object>();
            if (auditModeInfo != null)
            {
                result.SetValue("auditModeInfo", auditModeInfo);
            }
            result.SetValue("BatchID", batchID);
            return Json(result);
        }

        public JsonResult IsProductCanMetting(string productIDs)
        {
            var isSimple = this.GetQueryString("IsSimple") == "true";
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var products = projectEntities.Set<S_E_Product>().Where(c => productIDs.Contains(c.ID)).ToList();
            var batchID = FormulaHelper.CreateGuid();
            foreach (var product in products)
            {
                if (isSimple && !string.IsNullOrEmpty(product.ParentID) && product.ParentID.ToLower() != "split")
                    continue;
                //if (product.CoSignState == CoSignState.Sign.ToString() || product.CoSignState == CoSignState.SignComplete.ToString())
                //    throw new Formula.Exceptions.BusinessException("图号为【" + product.Code + "】的成果已经发起会签，不可再次发起。");
                product.BatchID = batchID;
            }
            projectEntities.SaveChanges();
            var result = new Dictionary<string, object>();
            result.SetValue("BatchID", batchID);
            return Json(result);
        }

        public JsonResult DeleteAuditProduct()
        {
            string auditIDs = GetQueryString("AuditIDs");
            string projectID = GetQueryString("ProjectInfoID");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string sql = string.Format("DELETE FROM T_AE_Audit WHERE ProjectInfoID='{0}' AND ID IN('{1}')", projectID, auditIDs.TrimEnd(',').Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult SaveDirectoryList(string ListData)
        {
            var listdata = UpdateList<S_E_ProductDirectory>(ListData);
            foreach (var item in listdata)
            {
                var directoryInfo = entities.Set<S_E_ProductDirectory>().FirstOrDefault(d => d.ID == item.ID);
                if (directoryInfo == null)
                    throw new Formula.Exceptions.BusinessException("成果目录条目【" + item.Name + "】不存在，请确认！");
                directoryInfo.Description = item.Description;
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetAllWorkProduct(QueryBuilder qb, string ProjectInfoID, string WBSID)
        {
            var auditForms = this.entities.Set<T_EXE_Audit>().Where(a => a.WBSID == WBSID).ToList();
            if (auditForms.Exists(a => a.FlowPhase.ToLower() != "end"))
                throw new Formula.Exceptions.BusinessException("存在进行中的校审流程时，不能再次发起整卷校审");
            qb.PageSize = 0;
            qb.Add("AuditState", QueryMethod.Equal, "Create");
            return GetProductList(qb, ProjectInfoID, WBSID);
        }

        protected override void AfterDelete(List<S_E_Product> entityList)
        {
            var productIDs = String.Join(",", entityList.Select(a => a.ID));
            var children = this.entities.Set<S_E_Product>().Where(a => productIDs.Contains(a.ParentID));
            foreach (var child in children)
            {
                this.entities.Set<S_E_Product>().Remove(child);
            }
            this.entities.SaveChanges();
        }
        #endregion

    }
}
