using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Linq.Expressions;
using Formula;
using Formula.DynConditionObject;
using Config;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Workflow.Logic;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using Formula.Helper;
using System.Data;

using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class ProductController : ProjectController<S_E_Product>
    {
        public override ActionResult Edit()
        {
            var wbsID = GetQueryString("WBSID");
            if (String.IsNullOrEmpty(wbsID))
            {
                var id = GetQueryString("ID");
                var product = this.GetEntityByID<S_E_Product>(id);
                if (product == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + id + "】的成果信息，无法进行编辑");
                wbsID = product.WBSID;
            }
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
            var majors = wbs.S_I_ProjectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString() && d.Code != wbs.Code).
               Select(d => new { Text = d.Name, Value = d.WBSValue, SortIndex = d.SortIndex }).Distinct().OrderBy(d => d.SortIndex).ToList();
            ViewBag.Major = JsonHelper.ToJson(majors);
            if (wbs.Parent == null) throw new Formula.Exceptions.BusinessException("未能找到子项节点，不能添加成果");
            var monomerEnum = wbs.Parent.S_W_Monomer.OrderBy(d => d.Name).Select(d => new { text = d.Name, value = d.Name }).ToList();
            if (wbs.WBSType == WBSNodeType.Work.ToString())
            {
                monomerEnum.Clear();
                var taskWork = wbs.S_W_TaskWork.FirstOrDefault();
                if (taskWork != null)
                    monomerEnum.Add(new { text = taskWork.DossierName, value = taskWork.DossierName });
            }
            ViewBag.Monomer = JsonHelper.ToJson(monomerEnum);

            var phaseCode = wbs.PhaseCode;
            if (String.IsNullOrEmpty(phaseCode))
            {
                phaseCode = wbs.S_I_ProjectInfo.PhaseValue;
            }
            else
            {
                if (phaseCode.Split('&').Length == 3)
                    phaseCode = phaseCode.Split('&')[2];
            }
            var PhaseRows = EnumBaseHelper.GetEnumTable("Project.Phase").Select(" value in ('" + phaseCode.Replace(",", "','") + "')");
            var phaseEnum = new List<Dictionary<string, object>>();
            foreach (DataRow item in PhaseRows)
            {
                phaseEnum.Add(FormulaHelper.DataRowToDic(item));
            }
            ViewBag.Phase = JsonHelper.ToJson(phaseEnum);
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }
        public ActionResult UpVersionProduct()
        {
            ViewBag.ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            return View();
        }

        protected override void AfterGetMode(S_E_Product entity, bool isNew)
        {
            if (isNew)
            {
                var wbsID = GetQueryString("WBSID");
                var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
                if (wbs == null) throw new Formula.Exceptions.BusinessException("找不到WBS节点，无法增加成果");
                if (!String.IsNullOrEmpty(wbs.PhaseCode))
                {
                    entity.PhaseValue = wbs.PhaseCode.Split(',')[0];
                    if (entity.PhaseValue.Split('&').Length == 3)
                        entity.PhaseValue = entity.PhaseValue.Split('&')[2];
                }
                entity.FileType = "图纸";
            }
            else
            {
                var versionID = GetQueryString("VersionID");
                if (!string.IsNullOrEmpty(versionID))
                {
                    var version = this.entities.Set<S_E_ProductVersion>().FirstOrDefault(a => a.ID == versionID);
                    if (version != null)
                    {
                        if (GetQueryString("FuncType").ToLower() == "view")
                            FormulaHelper.UpdateModel(entity, version);
                        //entity.MainFile = version.MainFile;
                        //entity.PdfFile = version.PdfFile;
                    }
                }
            }
        }

        protected override void BeforeSave(S_E_Product entity, bool isNew)
        {
            if (isNew)
            {
                if (entity.ParentID == null)
                    entity.ParentID = "";
                entity.Save();
                entity.Designer = entity.CreateUserID;
                entity.DesignerName = entity.CreateUser;

                if (!String.IsNullOrEmpty(entity.AuditID))
                {
                    var audit = entities.Set<T_EXE_Audit>().FirstOrDefault(d => d.ID == entity.AuditID);
                    if (audit != null)
                    {
                        var product = this.entities.Set<S_E_Product>().FirstOrDefault(d => d.AuditID == audit.ID && d.ID != entity.ID);
                        if (product != null)
                        {
                            entity.AuditState = product.AuditState;
                        }
                        else
                        {
                            entity.AuditState = AuditState.Design.ToString();
                        }
                    }
                }
                if (!String.IsNullOrEmpty(entity.ChangeAuditID))
                {
                    var audit = entities.Set<T_EXE_ChangeAudit>().FirstOrDefault(d => d.ID == entity.ChangeAuditID);
                    if (audit != null)
                    {
                        var product = this.entities.Set<S_E_Product>().FirstOrDefault(d => d.ChangeAuditID == audit.ID && d.ID != entity.ID);
                        if (product != null)
                        {
                            entity.AuditState = product.AuditState;
                        }
                        else
                        {
                            entity.AuditState = AuditState.Design.ToString();
                        }
                    }
                }
            }
            else
            {
                if (entity.AuditState != AuditState.Create.ToString()
                    && entity.AuditState != AuditState.Design.ToString()
                    && entity.AuditState != AuditState.Designer.ToString())
                {
                    throw new Formula.Exceptions.BusinessException("已经发起校审的成果不能编辑");
                }
                var wbsID = GetQueryString("WBSID");
                var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
                if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
                if (entities.Set<S_E_Product>().Where(d => d.WBSID == wbsID && d.Code == entity.Code && d.ID != entity.ID).Count() > 0)
                {
                    throw new Formula.Exceptions.BusinessException("已经存在图号为【" + entity.Code + "】的成果，不能有重复图号");
                }
                entity.Save();
            }
            if (entity.S_W_WBS.WBSType == WBSNodeType.Work.ToString())
            {
                var taskWork = entity.S_W_WBS.S_W_TaskWork.FirstOrDefault();
                if (taskWork != null)
                {
                    entity.MonomerCode = taskWork.DossierCode;
                    entity.MonomerInfo = taskWork.DossierName;
                    entity.PackageCode = taskWork.Code;
                    entity.PackageName = taskWork.Name;


                    if (entity.S_W_WBS.S_I_ProjectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TaskWorkDesingChange") == TrueOrFalse.True.ToString() && isNew)
                    {
                        if (taskWork.ChangeState == TaskWorkChangeState.AuditApprove.ToString() || taskWork.ChangeState == TaskWorkChangeState.AuditFinish.ToString())
                            throw new Formula.Exceptions.BusinessException("变更通知中及变更通知完成的卷册不能新增成果");
                    }
                }
            }

            if (String.IsNullOrEmpty(entity.AreaCode))
            {
                var areaNode = entity.S_W_WBS.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Area.ToString());
                if (areaNode != null)
                {
                    entity.AreaCode = areaNode.WBSValue;
                    entity.AreaInfo = areaNode.Name;
                }
            }

            if (String.IsNullOrEmpty(entity.DeviceCode))
            {
                var deviceNode = entity.S_W_WBS.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Device.ToString());
                if (deviceNode != null)
                {
                    entity.DeviceCode = deviceNode.WBSValue;
                    entity.DeviceInfo = deviceNode.Name;
                }
            }
        }

        public JsonResult GetVersion(string ID)
        {
            var product = entities.Set<S_E_Product>().FirstOrDefault(c => c.ID == ID);
            return Json(product);
        }

        public JsonResult GetVersionList(QueryBuilder qb)
        {
            string versionID = GetQueryString("ProductID");
            SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            qb.Add("ProductID", QueryMethod.Equal, versionID);
            string sql = @"select S_E_ProductVersion.* from S_E_ProductVersion
                    inner join S_E_Product on S_E_ProductVersion.ProductID=S_E_Product.ID";
            var grid = helper.ExecuteGridData(sql, qb);
            return Json(grid);
        }

        public JsonResult UpVersion()
        {
            var entity = this.UpdateEntity<S_E_Product>();
            entity.Upgrade();
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Change()
        {
            var entity = this.UpdateEntity<S_E_Product>();
            entity.Change(entity.MainFile, entity.PdfFile, entity.Attachments, entity.SwfFile);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertChange(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            entity.RevertChange();
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Invalid(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            entity.Invalid();
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertInvalid(string ProductID)
        {
            var entity = this.GetEntityByID(ProductID);
            entity.RevertInvalid();
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetSignState(string ProductIDs, string State)
        {
            var products = this.entities.Set<S_E_Product>().Where(a => ProductIDs.Contains(a.ID));
            foreach (var product in products)
            {
                product.SignState = State;
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
