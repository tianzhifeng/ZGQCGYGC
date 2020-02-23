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

namespace Project.Areas.Extend.Controllers
{
    public class ProductController : ProjectController<S_E_Product>
    {
        public override ActionResult Edit()
        {
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }

        //BaseConfigEntities baseconfig = FormulaHelper.GetEntities<BaseConfigEntities>();
        //public override ActionResult Edit()
        //{
        //    initSignMajor();
        //    return View();
        //}

        //public ActionResult UpVersionProduct()
        //{
        //    initSignMajor();
        //    return View();
        //}
        //public void initSignMajor()
        //{
        //    var WBSID = GetQueryString("WBSID");
        //    if (string.IsNullOrEmpty(WBSID))
        //    {
        //        var id = GetQueryString("ID");
        //        if (string.IsNullOrEmpty(id))
        //            id = GetQueryString("VersionID");
        //        var e = GetEntityByID(id);
        //        if (e == null)
        //            throw new Formula.Exceptions.BusinessException("未能找到对应的的WBS对象！");
        //        WBSID = e.WBSID;
        //    }

        //    var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
        //    if (wbs == null)
        //        throw new Formula.Exceptions.BusinessException("未能找到对应的WBS对象！");

        //    var major = WBSNodeType.Major.ToString();

        //    var majorwbs = wbs.Seniorities.Where(c => c.WBSType == major).FirstOrDefault();
        //    if (majorwbs == null)
        //        throw new Formula.Exceptions.BusinessException("未能找到对应的专业WBS对象！");

        //    var majorList = majorwbs.Parent.Children.Where(c => c.WBSType == major);
        //    ViewBag.Major = JsonHelper.ToJson(
        //        majorList.Select(c => new
        //        {
        //            Text = c.Name,
        //            Value = c.WBSValue,
        //            ChargeUserID = c.ChargeUserID,
        //            ChargeUserName = c.ChargeUserName,
        //        })
        //    );
        //}

        //public ActionResult DesignMain()
        //{
        //    string projectInfoID = GetQueryString("ProjectInfoID");
        //    string spaceCode = GetQueryString("SpaceCode");

        //    var projectInfo = entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.ID == projectInfoID);
        //    if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + projectInfoID + "】的项目！");
        //    if (projectInfo.WBSRoot == null) throw new Formula.Exceptions.BusinessException("找不到项目对应的WBS根节点！");
        //    ViewBag.ProjectInfo = projectInfo;
        //    ViewBag.SpaceCode = spaceCode;
        //    ViewBag.WBSRoot = projectInfo.WBSRoot;
        //    var transform = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => d.CanTransform == true.ToString()
        //       && d.Code != WBSNodeType.Work.ToString() && d.Code != WBSNodeType.Project.ToString()).
        //       Select(c => new { value = c.Code, text = "按" + c.Name, index = 1 }).ToList();
        //    transform.Add(new { value = "Project", text = "默认", index = 0 });
        //    transform.Add(new { value = "Progress", text = "按时间", index = 2 });
        //    ViewBag.TransForm = JsonHelper.ToJson(transform.OrderBy(d => d.index).ToList());
        //    var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
        //    ViewBag.DefaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;
        //    return View();
        //}
        //protected override void AfterGetMode(S_E_Product entity, bool isNew)
        //{
        //    if (isNew)
        //    {
        //        entity.FileType = "图纸";
        //    }
        //}

        //public override JsonResult Save()
        //{
        //    string wbsid = GetQueryString("WBSID");
        //    var wbs = GetEntityByID<S_W_WBS>(wbsid);
        //    if (wbs == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + wbsid + "】的wbs");
        //    var e = UpdateEntity<S_E_Product>();
        //    if (entities.Set<S_E_Product>().Count(p => p.VersionID != e.ID && p.Code == e.Code && p.WBSID == wbsid) > 0)
        //        throw new Formula.Exceptions.BusinessException("编号重复，请重新输入");
        //    wbs.SaveProduct(e);
        //    entities.SaveChanges();
        //    return Json(new { ID = e.ID });
        //}

        //public override JsonResult GetList(QueryBuilder qb)
        //{
        //    string wbsID = this.GetQueryString("WBSID");
        //    if (String.IsNullOrEmpty(wbsID)) return Json("");
        //    qb.Add("WBSID", QueryMethod.Equal, wbsID);
        //    if (qb.SortField == "ID")
        //        qb.SortField = "Code";

        //    var result = entities.Set<S_E_Product>().Where(c => c.ID == c.VersionID).WhereToGridData(qb);
        //    return Json(result); ;
        //}
        //public JsonResult GetVersionList(QueryBuilder qb)
        //{
        //    string versionID = GetQueryString("VersionID");
        //    var result = entities.Set<S_E_Product>().Where(c => c.VersionID == versionID).WhereToGridData(qb);
        //    return Json(result);
        //}
        //public JsonResult SaveProducts(string ListData)
        //{
        //    string wbsid = GetQueryString("WBSID");
        //    var wbs = GetEntityByID<S_W_WBS>(wbsid);
        //    if (wbs == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + wbsid + "】的wbs");
        //    var listdata = UpdateList<S_E_Product>(ListData);
        //    foreach (var item in listdata)
        //    {
        //        if (entities.Set<S_E_Product>().FirstOrDefault(p => p.VersionID != item.ID && p.Code == item.Code && p.WBSID == wbsid) != null)
        //            throw new Formula.Exceptions.BusinessException("编号“" + item.Code + "”重复，请重新输入");
        //        wbs.SaveProduct(item);
        //    }
        //    entities.SaveChanges();
        //    return Json("");
        //}

        //protected override void BeforeDelete(List<S_E_Product> entityList)
        //{
        //    foreach (var item in entityList)
        //        item.Delete();
        //}

        //public JsonResult StartAuditFlow(string ProductIDs, string WBSID, string ProjectInfoID)
        //{
        //    //var productids = ProductIDs.Split(',');
        //    //var products = entities.Set<S_E_Product>().Where(c => c.ProjectInfoID == ProjectInfoID && productids.Contains(c.ID)).ToList();
        //    //var auditService = AuditFlowServiceGenretor.CreateService();
        //    //var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
        //    //if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS对象，无法启动校审流程");
        //    //var auditFO = FormulaHelper.CreateFO<CustomAuditFO>();
        //    //var auditForm = new T_AE_Audit();
        //    //auditForm.ID = FormulaHelper.CreateGuid();
        //    //auditForm.Name = wbs.S_I_ProjectInfo.Name + "-";
        //    //if (!String.IsNullOrEmpty(wbs.SubProjectCode))
        //    //    auditForm.Name += wbs.SubProjectCode.Split('&')[1] + "-";
        //    //if (!String.IsNullOrEmpty(wbs.MajorCode))
        //    //    auditForm.Name += wbs.MajorCode.Split('&')[1] + "-";
        //    //if (wbs.S_W_TaskWork.Count > 0)
        //    //    auditForm.Name += wbs.Name;
        //    //auditForm.FullID = wbs.FullID;
        //    //auditForm.SubmitUser = CurrentUserInfo.UserName;
        //    //auditForm.SubmitUserID = CurrentUserInfo.UserID;
        //    //auditForm.DeptID = CurrentUserInfo.UserOrgID;
        //    //auditForm.DeptName = CurrentUserInfo.UserOrgName;
        //    //auditForm.CreateDate = DateTime.Now;
        //    //auditForm.Code = getCode();
        //    //var currentActivity = auditFO.StartAuditFlow(auditForm, "/Project/Form/Audit/AuditForm", products, WBSID);
        //    //#region  有会签单信息
        //    ////如果有会签信息，生成会签单
        //    //string coSignMajor = "";
        //    //foreach (var product in products)
        //    //{
        //    //    if (product.IsCoSign == "True")
        //    //    {
        //    //        var majors = coSignMajor.Split(',');
        //    //        foreach (var newMajor in product.CoSignMajor.Split(','))
        //    //        {
        //    //            if (!majors.Contains(newMajor))
        //    //                coSignMajor = coSignMajor + newMajor + ",";
        //    //        }

        //    //    }
        //    //}
        //    //coSignMajor = coSignMajor.TrimEnd(',');
        //    //if (coSignMajor != "")
        //    //{
        //    //    string coSignData = GetQueryString("CoSignData");
        //    //    if (string.IsNullOrEmpty(coSignData))
        //    //        throw new Formula.Exceptions.BusinessException("生成会签信息失败。");
        //    //    T_D_DesignCoSign coSign = new T_D_DesignCoSign();

        //    //    UpdateEntity<T_D_DesignCoSign>(coSign, JsonHelper.ToObject<Dictionary<string, object>>(coSignData));
        //    //    coSign.ID = GetGuid();
        //    //    coSign.AuditFormID = auditForm.ID;
        //    //    coSign.CoSignMajorCode = coSignMajor;
        //    //    coSign.CoSignState = CoSignState.NoSign.ToString();
        //    //    List<CoSignAdviceDTO> countsignlist = new List<CoSignAdviceDTO>();
        //    //    foreach (var product in products)
        //    //    {
        //    //        if (product.IsCoSign == "True")
        //    //        {
        //    //            product.CounterSignAuditID = coSign.ID;
        //    //            var list = GetSignUserInfo(product, currentActivity.FirstOrDefault());
        //    //            product.CoSignUser = JsonHelper.ToJson(list);
        //    //            countsignlist = countsignlist.Union(list).ToList();
        //    //        }
        //    //    }

        //    //    if (countsignlist.Count() > 0)
        //    //        coSign.Advice = JsonHelper.ToJson(countsignlist);

        //    //    entities.Set<T_D_DesignCoSign>().Add(coSign);
        //    //}
        //    ////-----------------------------------------------------------------------------------------------

        //    //#endregion

        //    //var activity = entities.Set<S_W_Activity>().FirstOrDefault(d => d.WBSID == wbs.ID && d.State == "Create" && d.ActivityKey == "DesignTask");
        //    //if (activity != null)
        //    //    activity.Finish();
        //    //this.entities.SaveChanges();
        //    return Json("");
        //}
        //public string getCode()
        //{
        //    string year = DateTime.Now.Year.ToString();
        //    string month = DateTime.Now.Month.ToString().PadLeft(2, '0');
        //    string maxCode = entities.Set<T_AE_Audit>().OrderBy("Code", false).Select(c => c.Code).FirstOrDefault();
        //    if (string.IsNullOrEmpty(maxCode))
        //        maxCode = year + month + "0001";
        //    else
        //    {
        //        if (maxCode.IndexOf(year + month) >= 0)
        //        {
        //            string indexCode = (Convert.ToInt32(maxCode.Substring(6)) + 1).ToString().PadLeft(4, '0');
        //            maxCode = year + month + indexCode;
        //        }
        //        else
        //        {
        //            maxCode = year + month + "0001";
        //        }
        //    }
        //    return maxCode;
        //}

        ////public List<CoSignAdviceDTO> GetSignUserInfo(S_E_Product product, S_W_Activity actity)
        ////{
        ////    string list = string.Empty;

        ////    var steps = actity.GetSteps();
        ////    AuditStep coSignStep = null;
        ////    foreach (var step in steps)
        ////    {
        ////        if (step.CoSign == "是")
        ////        {
        ////            coSignStep = step;
        ////            break;
        ////        }
        ////    }

        ////    return getCoSignUser(product, coSignStep.CoSignRole);
        ////}

        ///// <summary>
        ///// 取会签专业的会签人员
        ///// </summary>
        ///// <param name="majors">会签专业，逗号隔开</param>
        ///// <param name="act">当前校审任务</param>
        ///// <returns></returns>
        //public List<CoSignAdviceDTO> getCoSignUser(S_E_Product product, string coSignRole)
        //{
        //    List<CoSignAdviceDTO> signList = new List<CoSignAdviceDTO>();
        //    string projectInfoID = product.ProjectInfoID;
        //    string wbsType = WBSNodeType.Major.ToString();
        //    foreach (var major in product.CoSignMajor.Split(','))
        //    {
        //        foreach (var role in coSignRole.Split(','))
        //        {
        //            var rbsUser = entities.Set<S_W_RBS>().Where(c => c.ProjectInfoID == product.ProjectInfoID && c.WBSType == wbsType && c.MajorValue == major && c.RoleCode == role).Select(c => new { UserID = c.UserID, UserName = c.UserName }).Distinct();
        //            foreach (var userID in rbsUser)
        //            {
        //                Dictionary<string, string> dic = new Dictionary<string, string>();
        //                CoSignAdviceDTO item = new CoSignAdviceDTO();
        //                item.ProductID = product.ID;
        //                item.ProductCode = product.Code;
        //                item.CoSignMajorCode = major;
        //                var attrdefine = baseconfig.S_D_WBSAttrDefine.Where(c => c.Type == "Major" && c.Code == major).FirstOrDefault();
        //                if (attrdefine != null)
        //                    item.CoSignMajorName = attrdefine.Name;

        //                item.CoSignUserID = userID.UserID;
        //                item.CoSignUserName = userID.UserName;

        //                signList.Add(item);
        //            }
        //        }
        //    }

        //    return signList;
        //}
        //public ActionResult SelectList()
        //{
        //    return View();
        //}

        //public JsonResult GetSelectList(MvcAdapter.QueryBuilder qb)
        //{
        //    string wbsID = GetQueryString("WBSID");
        //    string projectInfoID = GetQueryString("ProjectInfoID");
        //    var notInProductIDs = GetQueryString("NotInProductIDs").Split(',');

        //    var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
        //    if (wbs == null)
        //        return Json("");
        //    var list = this.entities.Set<S_E_Product>()
        //        .Where(d => d.ProjectInfoID == projectInfoID &&
        //                d.WBSFullID.StartsWith(wbs.FullID) &&
        //                !notInProductIDs.Contains(d.ID)
        //            ).WhereToGridData(qb);
        //    return Json(list);
        //}

        //public JsonResult GetVersion(string VersionID)
        //{
        //    var product = entities.Set<S_E_Product>().FirstOrDefault(c => c.ID == VersionID);
        //    var maxProduct = entities.Set<S_E_Product>().FirstOrDefault(c => c.ID == product.VersionID);
        //    S_E_Product versionProduct = new S_E_Product();
        //    if (maxProduct != null)
        //    {
        //        versionProduct = maxProduct.Clone<S_E_Product>();
        //    }
        //    else
        //    {
        //        versionProduct = product.Clone<S_E_Product>();
        //    }

        //    versionProduct.AuditID = "";
        //    versionProduct.ID = GetGuid();

        //    //versionProduct.VersionID = versionProduct.ID;
        //    //versionProduct.Version = (Convert.ToInt32(versionProduct.Version) + 1).ToString();

        //    return Json(versionProduct);
        //}

        //public JsonResult UpVersion()
        //{
        //    string wbsid = GetQueryString("WBSID");
        //    var wbs = GetEntityByID<S_W_WBS>(wbsid);
        //    if (wbs == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + wbsid + "】的wbs");
        //    var e = UpdateEntity<S_E_Product>();
        //    e.SignState = "False";
        //    e.CoSignState = "";
        //    e.CoSignUser = "";
        //    e.AuditSignUser = "";

        //    wbs.SaveProduct(e);

        //   // entities.Set<S_E_Product>().Where(c => c.VersionID == oldVersionID).Update(c => c.VersionID = e.ID);

        //    entities.SaveChanges();
        //    return Json("");
        //}
    }
}
