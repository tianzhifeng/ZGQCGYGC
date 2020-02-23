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
    public class BudgetSubmitController : EPCFormContorllor<S_I_BudgetInfo>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.VersionNo = dic.GetValue("VersionNumber");
            dic.SetValue("ShowType", "New");
            dic.SetValue("ShowAll", "false");

            #region 是否启用虚加载
            int detailCount = 0;
            var detailCountObj = this.EPCSQLDB.ExecuteScalar("select count(ID) from S_I_BudgetInfo_Detail with(nolock) WHERE BudgetInfoID='" + dic.GetValue("ID") + "'");
            int.TryParse((detailCountObj ?? "").ToString(), out detailCount);
            ViewBag.VirtualScroll = "false";
            if ((detailCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }
            #endregion
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_I_BudgetInfo_Detail>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {
                var lastVersion = this.EPCEntites.Set<S_I_BudgetInfo>().Where(d => d.EngineeringInfoID == detail.S_I_BudgetInfo.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.BudgetInfoID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion == null)
                {
                    //var bom = this.GetEntityByID<S_P_Bom>(detail.BomID);
                    //if (bom != null)
                    //{
                    //    result = FormulaHelper.ModelToDic<S_P_Bom>(bom);
                    //}
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

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType, string showAll)
        {
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
            treeList = this.EPCEntites.Set<S_I_BudgetInfo_Detail>().Where(qb).ToList();
            return Json(treeList);
        }

        protected override void OnFlowEnd(S_I_BudgetInfo entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
            }
            this.EPCEntites.SaveChanges();
        }


    }
}
