using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Areas.Quality.Controllers
{
    public class QBSVersionController : EPCFormContorllor<S_Q_QBS_Version>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("不能直接新增版本数据");
            }
            else
            {
                string engineeringInfoID = dic.GetValue("EngineeringInfoID");
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
                if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
                ViewBag.EngineeringInfoID = engineeringInfoID;
                dic.SetValue("ShowType", "Diff");
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_Q_QBS_Version>(VersionID);
            if (version == null) return Json(result);
            var data = this.EPCEntites.Set<S_I_Section>().Where(d => d.EngineeringInfoID == version.EngineeringInfoID).OrderBy(c => c.SortIndex).ToList();

            var versionList = new List<S_Q_QBS_Version_QBSData>();
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Normal.ToString());
                versionList = this.EPCEntites.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();

            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
                versionList = this.EPCEntites.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();
            }
            else
            {
                //显示全部数据，体现差异
                versionList = this.EPCEntites.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();

            }
            var rootStruct = version.S_I_Engineering.Mode.S_C_QBSStruct.FirstOrDefault(c => c.NodeType == "Root");
            if (rootStruct == null) throw new Formula.Exceptions.BusinessValidationException("QBS结构定义中没有根节点");
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_I_Section>(item);
                var details = versionList.Where(c => c.SectionID == item.ID).ToList();
                dic.SetValue("QBSID", item.ID);
                dic.SetValue("NodeType", "Root");
                dic.SetValue("StructNodeID", rootStruct.ID);
                dic.SetValue("FullID", item.ID);
                dic.SetValue("ParentID", "");
                dic.SetValue("CanAdd", true);
                dic.SetValue("CanParentAdd", false);
                dic.SetValue("CanDelete", false);
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(detail);
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_Q_QBS_Version_QBSData>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {

                var lastVersion = this.EPCEntites.Set<S_Q_QBS_Version>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion != null)
                {
                    var lastDetail = lastVersion.S_Q_QBS_Version_QBSData.FirstOrDefault(c => c.QBSID == detail.QBSID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        protected override void OnFlowEnd(S_Q_QBS_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
