using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Areas.Construction.Controllers
{
    public class BOQSubmitController : EPCFormContorllor<S_C_BOQ_Version>
    {
        public JsonResult GetLastDetailInfo(string ID, string VersionID)
        {
            var detail = this.GetEntityByID<S_C_BOQ_Version_Detail>(ID);
            var currDetailVersion = EPCEntites.Set<S_C_BOQ_Version>().Find(detail.VersionID);
            var result = new Dictionary<string, object>();
            if (detail != null && currDetailVersion != null)
            {
                var lastVersion = EPCEntites.Set<S_C_BOQ_Version>()
                    .Where(d => d.ContractInfoID == currDetailVersion.ContractInfoID && d.FlowPhase == "End" && d.VersionNumber < currDetailVersion.VersionNumber)
                    .OrderByDescending(c => c.VersionNumber).FirstOrDefault();

                if (lastVersion == null)
                {
                    var boq = this.GetEntityByID<S_C_BOQ>(detail.BOQID);
                    if (boq != null)
                    {
                        result = FormulaHelper.ModelToDic<S_C_BOQ>(boq);
                    }
                }
                else
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

        //界面dataGrid显示数据
        public JsonResult GetBOQList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var boqVersionList = GetSearchList(qb, VersionID, ShowType);
            return Json(boqVersionList);
        }

        private List<S_C_BOQ_Version_Detail> GetSearchList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var boqVersionList = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == VersionID);
            if (boqVersionList == null)
            {
                return new List<S_C_BOQ_Version_Detail>();
            }

            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, "Normal");
                boqVersionList = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == VersionID).Where(qb);

            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, "Remove");
                boqVersionList = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(c => c.VersionID == VersionID).Where(qb);
            }
            else
            {
                //显示全部数据，体现差异
                boqVersionList = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(c => c.VersionID == VersionID).Where(qb);

            }

            return boqVersionList.ToList();
        }

        protected override void OnFlowEnd(S_C_BOQ_Version entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            entity.State = "Publish";       
            var detailList = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == entity.ID).ToList();
            foreach (var item in detailList)
            {
                if (item.ModifyState == "Normal")
                {
                    continue;
                }
                var itemDic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(item);
                if (item.ModifyState == "Add")
                {
                    S_C_BOQ boq = new S_C_BOQ();
                    boq.ID = item.BOQID;
                    boq.Code = item.Code;
                    boq.Name = item.Name;
                    boq.Property = item.Property;
                    boq.Quantity = item.Quantity;
                    boq.Price = item.Price;
                    boq.Remark = item.Remark;
                    boq.Unit = item.Unit;
                    boq.UnitPrice = item.UnitPrice;
                    boq.VersionNo = entity.VersionNumber;
                    boq.ContractInfoID = entity.ContractInfoID;
                    EPCEntites.Set<S_C_BOQ>().Add(boq);
                }
                else if (item.ModifyState == "Remove")
                {
                    EPCEntites.Set<S_C_BOQ>().Delete(a => a.ID == item.BOQID);
                }
                else if (item.ModifyState == "Modify")
                {
                    var boq = EPCEntites.Set<S_C_BOQ>().Find(item.BOQID);
                    if (boq == null)
                    {
                        boq = new S_C_BOQ();
                        boq.ID = item.BOQID;
                    }
                    boq.VersionNo = entity.VersionNumber;
                    //boq._state
                    FormulaHelper.UpdateEntity<S_C_BOQ>(boq, itemDic, true);
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(item.ModifyState + "没有if分支处理");
                }

            }
            EPCEntites.SaveChanges();
        }

    }
}
