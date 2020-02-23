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


namespace EPC.Areas.Manage.Controllers
{
    public class PBSVersionController : EPCFormContorllor<S_I_PBS_Version>
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
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var version = this.GetEntityByID<S_I_PBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的结构策划版本");
            qb.PageSize = 0;
            qb.Add("VersionID", QueryMethod.Equal, VersionID);
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
            var result = this.EPCEntites.Set<S_I_PBS_Version_PBSData>().Where(qb).ToList();
            return Json(result);
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_I_PBS_Version_PBSData>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {

                var lastVersion = this.EPCEntites.Set<S_I_PBS_Version>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion != null)
                {
                    var lastDetail = lastVersion.S_I_PBS_Version_PBSData.FirstOrDefault(c => c.PBSID == detail.PBSID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        protected override void OnFlowEnd(S_I_PBS_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if(entity!=null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }      
        }
    }
}
