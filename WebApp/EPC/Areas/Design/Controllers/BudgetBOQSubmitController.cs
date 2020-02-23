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
    public class BudgetBOQSubmitController : EPCFormContorllor<S_C_BOQ_Version>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            dic.SetValue("ShowType", "Diff");
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //if (isNew)
            //{
            //    throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            //}
        }

        protected override void OnFlowEnd(S_C_BOQ_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            //获取detail

        }

        public JsonResult GetTreeList(QueryBuilder qb, string VersionID, string ShowType, string showAllPBS)
        {
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            var version = EPCEntites.Set<S_C_BOQ_Version>().Find(VersionID);
            if (version == null)
            {
                return Json("");
            }
            var result = GetSearchList(qb, VersionID, ShowType, showAllPBS);
            return Json(result);
        }

        public JsonResult GetLastDetailInfo(string ID, string VersionID)
        {
            var detail = this.GetEntityByID<S_C_BOQ_Version_Detail>(ID);
            var currDetailVersion = EPCEntites.Set<S_C_BOQ_Version>().Find(detail.VersionID);
            var result = new Dictionary<string, object>();
            if (detail != null && currDetailVersion != null)
            {
                var lastVersion = EPCEntites.Set<S_C_BOQ_Version>()
                    .Where(d => d.EngineeringInfoID == currDetailVersion.EngineeringInfoID && d.FlowPhase == "End" && d.VersionNumber < currDetailVersion.VersionNumber)
                    .OrderByDescending(c => c.VersionNumber).FirstOrDefault();

                if (lastVersion != null)
                {
                    var lastDetail = EPCEntites.Set<S_C_BOQ_Version_Detail>().FirstOrDefault(a => a.BOQID == detail.BOQID && a.VersionID == lastVersion.ID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(lastDetail);
                    }
                }
            }
            return Json(result);
        }


        private List<Dictionary<string, object>> GetSearchList(QueryBuilder qb, string VersionID, string ShowType, string showAllPBS)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_C_BOQ_Version>(VersionID);
            if (version == null) return result;

            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            var pbsDt = this.EPCSQLDB.ExecuteDataTable(String.Format(@"SELECT * FROM S_I_PBS WITH(NOLOCK) WHERE EngineeringInfoID='{0}' ORDER BY SORTINDEX", version.EngineeringInfoID));
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

            var versionDt = this.EPCSQLDB.ExecuteDataTable(sql, qb);
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
    }
}
