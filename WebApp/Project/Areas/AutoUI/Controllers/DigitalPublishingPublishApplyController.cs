using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;

namespace Project.Areas.AutoUI.Controllers
{
    public class DigitalPublishingPublishApplyController : ProjectFormContorllor<T_EXE_PublishApply>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }
            return base.PageView();
        }

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            var batchID = this.GetQueryString("BatchID");
            if (!string.IsNullOrEmpty(batchID) && dt.Rows.Count > 0)
            {
                if (!dt.Columns.Contains("Products"))
                    dt.Columns.Add("Products");
                if (dt.Rows[0]["Products"] != DBNull.Value && dt.Rows[0]["Products"] != null
                    && !string.IsNullOrEmpty(dt.Rows[0]["Products"].ToString()))
                {
                    var detailList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["Products"].ToString());
                    if (detailList.Count > 0)
                        return;
                }
                var pdDt = this.ProjectSQLDB.ExecuteDataTable(@"
select * from S_E_Product p cross apply(select top 1 ID as VersionID from S_E_ProductVersion
where p.ID = S_E_ProductVersion.ProductID
and p.Version = S_E_ProductVersion.Version) as pv where p.BatchID='" + batchID + "'");
                var list = FormulaHelper.DataTableToListDic(pdDt);
                foreach (var item in list)
                {
                    item.SetValue("ProductID", item.GetValue("ID"));
                    item.SetValue("VersionID", item.GetValue("VersionID"));
                    item.SetValue("ProductName", item.GetValue("Name"));
                    item.SetValue("ProductCode", item.GetValue("Code"));
                    item.SetValue("ProductVersion", item.GetValue("Version"));
                    item.SetValue("MainAttachments", item.GetValue("MainFile"));
                    item.SetValue("Specifications", item.GetValue("FileSize"));
                    item.SetValue("ID", "");
                }
                dt.Rows[0]["Products"] = JsonHelper.ToJson(list);
            }
        }

        public JsonResult GetOBSUser(string ProjectInfoID)
        {
            var sql = "select * from S_W_OBSUser where ProjectInfoID = '" + ProjectInfoID + "'";
            var dt = this.ProjectSQLDB.ExecuteDataTable(sql).AsEnumerable();
            var auditStateArray = Enum.GetNames(typeof(Project.Logic.AuditState));
            var dic = new Dictionary<string, object>();
            foreach (var item in auditStateArray)
            {
                var list = dt.Where(a => a["RoleCode"].ToString() == item)
                    .Select(a => new
                    {
                        text = a["UserName"].ToString(),
                        value = a["UserID"].ToString()
                    }).Distinct().ToArray();
                dic.SetValue(item, list);
            }
            return Json(dic);
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "Products")
            {
                var productID = detail.GetValue("ProductID");
                var versionID = detail.GetValue("VersionID");
                var product = this.BusinessEntities.Set<S_E_Product>().FirstOrDefault(a => a.ID == productID);
                var version = this.BusinessEntities.Set<S_E_ProductVersion>().FirstOrDefault(a => a.ID == versionID);
                if (product != null)
                {
                    product.PlotSealGroup = detail.GetValue("PlotSealGroup");
                    product.PlotSealGroupName = detail.GetValue("PlotSealGroupName");
                    product.PlotSealGroupKey = detail.GetValue("PlotSealGroupKey");
                    product.PDFSignPositionInfo = detail.GetValue("PDFSignPositionInfo");
                }
                if (version != null)
                {
                    version.PlotSealGroup = detail.GetValue("PlotSealGroup");
                    version.PlotSealGroupName = detail.GetValue("PlotSealGroupName");
                    version.PlotSealGroupKey = detail.GetValue("PlotSealGroupKey");
                    version.PDFSignPositionInfo = detail.GetValue("PDFSignPositionInfo");
                }
            }
        }

        protected override void OnFlowEnd(T_EXE_PublishApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var publishInfo = this.GetEntityByID<S_EP_PublishInfo>(entity.ID);
            if (publishInfo == null)
            {
                publishInfo = new S_EP_PublishInfo();
                this.BusinessEntities.Set<S_EP_PublishInfo>().Add(publishInfo);
            }
            this.UpdateEntity<S_EP_PublishInfo>(publishInfo, entity.ToDic());
            publishInfo.ID = entity.ID;
            publishInfo.SubmitTime = DateTime.Now;
            publishInfo.BelongYear = ((DateTime)publishInfo.SubmitTime).Year;
            publishInfo.BelongMonth = ((DateTime)publishInfo.SubmitTime).Month;
            publishInfo.BelongQuarter = ((publishInfo.BelongMonth - 1) / 3) + 1;
            foreach (var item in entity.T_EXE_PublishApply_PriceDetail)
            {
                var price = this.GetEntityByID<S_EP_PublishInfo_PriceDetail>(item.ID);
                if (price == null)
                {
                    price = new S_EP_PublishInfo_PriceDetail();
                    this.BusinessEntities.Set<S_EP_PublishInfo_PriceDetail>().Add(price);
                }
                this.UpdateEntity<S_EP_PublishInfo_PriceDetail>(price, item.ToDic());
                price.ID = item.ID;
                price.S_EP_PublishInfoID = publishInfo.ID;
            }
            foreach (var item in entity.T_EXE_PublishApply_Products)
            {
                var product = this.GetEntityByID<S_EP_PublishInfo_Products>(item.ID);
                if (product == null)
                {
                    product = new S_EP_PublishInfo_Products();
                    this.BusinessEntities.Set<S_EP_PublishInfo_Products>().Add(product);
                }
                this.UpdateEntity<S_EP_PublishInfo_Products>(product, item.ToDic());
                product.ID = item.ID;
                product.S_EP_PublishInfoID = publishInfo.ID;
            }

            entity.SetProductPrintState(PrintState.Printed.ToString());
            entity.SetProductSignUser();
            this.BusinessEntities.SaveChanges();
        }
    }
}