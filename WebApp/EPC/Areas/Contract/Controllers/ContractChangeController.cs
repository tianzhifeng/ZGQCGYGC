using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;
using Config;
using Formula.Helper;
using Base.Logic.Model.UI.Form;
using Base.Logic.Domain;
using System.Data;
using Formula;
using EPC.Logic.Domain;
using System.Reflection;
using Newtonsoft.Json;

namespace EPC.Areas.Contract.Controllers
{
    public class ContractChangeController : EPCFormContorllor<T_M_ContractInfoChange>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string ContractInfoID = GetQueryString("ContractInfoID");
                if (!string.IsNullOrEmpty(ContractInfoID))
                {
                    int maxNumber = 0;
                    EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
                    var ContractInfo = epcentity.Set<T_M_ContractInfo>().FirstOrDefault(a => a.ID == ContractInfoID);
                    var Contracts = epcentity.Set<T_M_ContractInfoChange>().Where(c => c.ContractInfoID == ContractInfoID);

                    if (Contracts != null && Contracts.Count() > 0)
                    {
                        maxNumber = Contracts.ToList().Max(c => Convert.ToInt32(c.VersionNumber));
                    }
                    maxNumber++;
                    if (ContractInfo != null)
                    {
                        Type tp = ContractInfo.GetType();
                        var ps = tp.GetProperties();
                        DataRow dtr = dt.Rows[0];
                        var ContractInfoReceivePlan = epcentity.Set<T_M_ContractInfo_ReceivePlan>().Where(a => a.T_M_ContractInfoID == ContractInfoID);
                        var HTEAFPFJ = epcentity.Set<T_M_ContractInfo_HTEAFPFJ>().Where(a => a.T_M_ContractInfoID == ContractInfoID);
                        string lms = "";
                        string strht = "";
                        if (HTEAFPFJ != null && HTEAFPFJ.Count() > 0)
                        {
                            List<T_M_ContractInfo_HTEAFPFJ> smcil = HTEAFPFJ.ToList();
                            List<T_M_ContractInfoChange_HTEAFPFJ> tcclm = new List<T_M_ContractInfoChange_HTEAFPFJ>();
                            var psc = typeof(T_M_ContractInfoChange_ReceivePlan).GetProperties();

                            foreach (T_M_ContractInfo_HTEAFPFJ item in smcil)
                            {
                                T_M_ContractInfoChange_HTEAFPFJ smclk = new T_M_ContractInfoChange_HTEAFPFJ();
                                foreach (PropertyInfo pc in psc)
                                {
                                    string cname = pc.Name;
                                    string type = pc.PropertyType.Name;
                                    object value = pc.GetValue(item, null);
                                    if (type != "ICollection`1" && cname != "ID" && value != null)
                                    {
                                        pc.SetValue(smclk, value, null);
                                    }
                                }
                                tcclm.Add(smclk);
                            }
                            strht = JsonConvert.SerializeObject(tcclm);
                        }
                        if (ContractInfoReceivePlan != null && ContractInfoReceivePlan.Count() > 0)
                        {
                            List<T_M_ContractInfo_ReceivePlan> smcil = ContractInfoReceivePlan.ToList();
                            List<T_M_ContractInfoChange_ReceivePlan> tcclm = new List<T_M_ContractInfoChange_ReceivePlan>();
                            var psc = typeof(T_M_ContractInfoChange_ReceivePlan).GetProperties();

                            foreach (T_M_ContractInfo_ReceivePlan item in smcil)
                            {
                                T_M_ContractInfoChange_ReceivePlan smclk = new T_M_ContractInfoChange_ReceivePlan();
                                foreach (PropertyInfo pc in psc)
                                {
                                    string cname = pc.Name;
                                    string type = pc.PropertyType.Name;
                                    object value = pc.GetValue(item,null);
                                    if (type != "ICollection`1" && cname != "ID"&&value!=null)
                                    {
                                        pc.SetValue(smclk, value,null);
                                    }
                                }
                                tcclm.Add(smclk);
                            }
                            lms = JsonConvert.SerializeObject(tcclm);
                        }

                        foreach (PropertyInfo item in ps)
                        {
                            string cname = item.Name;
                            string type = item.PropertyType.Name;
                            if (type != "ICollection`1" && cname != "ContractInfoID" && cname != "VersionNumber" && dt.Columns.Contains(cname))
                            {
                                object value = item.GetValue(ContractInfo, null);
                                if (cname == "ID")
                                {
                                    dtr.SetField("ContractInfoID", value);
                                }
                                else
                                {
                                    dtr.SetField(cname, value);
                                }
                            }
                        }
                        dtr.SetField("VersionNumber", maxNumber);
                        dtr.SetField("ReceivePlan", lms);
                        dtr.SetField("HTEAFPFJ", HTEAFPFJ);
                    }
                }
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
           
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
          
        }

        public JsonResult RemoveReceiptObj(string ReceiptData)
        {
            var list = JsonHelper.ToList(ReceiptData);
            foreach (var item in list)
            {
                var receiptObject = EPCEntites.Set<S_M_ContractInfo_ReceiptObj>().Find(item.GetValue("OrlID"));//通过合同来验证
                if (receiptObject != null)
                {
                    if (receiptObject.FactInvoiceValue > 0) throw new Formula.Exceptions.BusinessValidationException("收款项【" + receiptObject.Name + "】已经有开票信息，无法进行删除");
                    if (receiptObject.FactReceiptValue > 0) throw new Formula.Exceptions.BusinessValidationException("收款项【" + receiptObject.Name + "】已经有收款信息，无法进行删除");
                }
            }
            entities.SaveChanges();
            return Json("true");
        }

        protected override void OnFlowEnd(T_M_ContractInfoChange entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            var contract = EPCEntites.Set<T_M_ContractInfo>().Find(entity.ContractInfoID);
            var scontract = EPCEntites.Set<S_M_ContractInfo>().FirstOrDefault(c=>c.TContractInfoID==contract.ID);
            if (entity!=null&&contract != null) {
                var ps = typeof(T_M_ContractInfo).GetProperties();
                foreach (PropertyInfo item in ps) {
                    string name = item.Name;
                    string type = item.PropertyType.Name;
                    object value = item.GetValue(entity, null);
                    if (type != "ICollection`1" && name != "ContractInfoID" && name != "VersionNumber" && name != "ID"&value!=null) {
                        item.SetValue(contract, value,null);
                    }
                }
            }
            if (entity != null && scontract != null)
            {
                var ps = typeof(S_M_ContractInfo).GetProperties();
                foreach (PropertyInfo item in ps)
                {
                    string name = item.Name;
                    string type = item.PropertyType.Name;
                    object value = item.GetValue(entity, null);
                    if (type != "ICollection`1" && name != "ContractInfoID" && name != "VersionNumber" && name != "ID" & value != null)
                    {
                        item.SetValue(scontract, value, null);
                    }
                }
            }
            var project = EPCEntites.Set<T_I_EngineeringBuild>().FirstOrDefault(c => c.ID == entity.ProjectInfo);

            if (project != null) {
                project.Programme = entity.Programme;
                project.ConstructionFeatures = entity.ConstructionFeatures;
                project.ConstructionSite = entity.ConstructionSite;
            }




            this.EPCEntites.SaveChanges();
        }
    }
}
