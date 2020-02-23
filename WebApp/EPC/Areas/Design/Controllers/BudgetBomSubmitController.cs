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

namespace EPC.Areas.Design.Controllers
{
    public class BudgetBomSubmitController : EPCFormContorllor<S_E_Bom>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            dic.SetValue("ShowType", "Diff");

            #region 是否启用虚加载
            int detailCount = 0;
            var detailCountObj = this.EPCSQLDB.ExecuteScalar("select count(ID) from S_E_Bom_Detail with(nolock) WHERE VersionID='" + dic.GetValue("ID") + "'");
            detailCount = Convert.ToInt32(detailCountObj);
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
            var detail = this.GetEntityByID<S_E_Bom_Detail>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {
                var lastVersion = this.EPCEntites.Set<S_E_Bom>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.MajorCode == detail.MajorCode && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
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

        public JsonResult GetVersionTreeList(string VersionID, string ShowType)
        {
            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            var data = this.EPCEntites.Set<S_I_PBS>().Where(d => d.EngineeringInfoID == version.EngineeringInfoID).ToList();
            var result = new List<Dictionary<string, object>>();
            var versionList = new List<S_E_Bom_Detail>();

            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                versionList = version.S_E_Bom_Detail.Where(c => c.ModifyState != BomVersionModifyState.Normal.ToString()).OrderBy(c => c.SortIndex).ToList();
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                versionList = version.S_E_Bom_Detail.Where(c => c.ModifyState != BomVersionModifyState.Remove.ToString()).OrderBy(c => c.SortIndex).ToList();
            }
            else
            {
                //显示全部数据，体现差异
                versionList = version.S_E_Bom_Detail.OrderBy(c => c.SortIndex).ToList();
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
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        protected override void OnFlowEnd(S_E_Bom entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
            }
            this.EPCEntites.SaveChanges();
        }

    }
}
