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
using Base.Logic.Domain;

namespace EPC.Areas.HSE.Controllers
{
    public class SafeCheckContentController : EPCFormContorllor<T_HSE_SafeCheck>
    {
        //
        // GET: /HSE/SafeCheckContent/

        private EPCEntities dbContext = FormulaHelper.GetEntities<EPCEntities>();
        public ActionResult Index()
        {
            return View();
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName,  detailList, formInfo);
            //编辑状态下判断
            //修改子项,修改的问题已经处于整改流程中或者已经结束，则不能够修改
            //修改项
            IEnumerable<Dictionary<string, string>> modifyItems = detailList.Where(a => a.GetValue("_state").ToLower() == "modified");
            foreach (var item in modifyItems)
            {
                string itemID = item.GetValue("ID");
                var problem = dbContext.T_HSE_RectifySheet_RectifyProblems.SingleOrDefault(a => a.T_HSE_SafeCheck_CheckContentRecordID == itemID);
                if (problem != null)
                {
                    if (dbContext.T_HSE_RectifySheet.Any(a => a.ID == problem.T_HSE_RectifySheetID))
                    {
                        throw new Formula.Exceptions.BusinessValidationException("HSE检查记录问题【" + item.GetValue("CheckContent") + "】在整改,因此不能修改");
                    }
                    
                    else
                    {
                        problem.Code = item.GetValue("SerialNumber");
                        problem.Problems = item.GetValue("MainProblems");
                        //检查项目  rp.? = item.GetValue("CheckContent");
                        //problem.EngineeringInfo = dic.GetValue("EngineeringInfo");
                        //problem.EngineeringInfoName = dic.GetValue("EngineeringInfoName");                        
                    }
                }
            }
            //删除子项,删除的问题已经处于整改流程中或者已经结束，则不能够删除
            //删除项
            string checkID = dic.GetValue("ID");
            List<string> ids = detailList.Select(a=>a.GetValue("ID")).ToList();
            var removeItems = dbContext.T_HSE_SafeCheck_CheckContentRecord.Where(a=>a.T_HSE_SafeCheckID == checkID && !ids.Contains(a.ID)).ToList();
            foreach (var item in removeItems)
            {
                string itemID = item.ID;
                var problem = dbContext.T_HSE_RectifySheet_RectifyProblems.SingleOrDefault(a => a.T_HSE_SafeCheck_CheckContentRecordID == itemID);
                if (problem != null)
                {
                    if (dbContext.T_HSE_RectifySheet.Any(a => a.ID == problem.T_HSE_RectifySheetID))
                    {
                        throw new Formula.Exceptions.BusinessValidationException("HSE检查记录问题【" + item.CheckContent + "】在整改,因此不能删除");
                    }
                    //同时删除在问题整改单列表的问题记录
                    else
                    {
                        dbContext.T_HSE_RectifySheet_RectifyProblems.Delete(a => a.T_HSE_SafeCheck_CheckContentRecordID == itemID);
                    }
                }
            }
            //增加项
            IEnumerable<Dictionary<string, string>> addItems = detailList.Where(a => a.GetValue("_state").ToLower() == "added");
            foreach (var item in addItems)
            {
                T_HSE_RectifySheet_RectifyProblems rp = new T_HSE_RectifySheet_RectifyProblems();
                rp.ID = FormulaHelper.CreateGuid();
                rp.T_HSE_SafeCheck_CheckContentRecordID = item.GetValue("ID");
                rp.Code = item.GetValue("SerialNumber");

                rp.Problems = item.GetValue("MainProblems");
                //检查项目  rp.? = item.GetValue("CheckContent");
                rp.EngineeringInfo = dic.GetValue("EngineeringInfo");
                rp.EngineeringInfoName = dic.GetValue("EngineeringInfoName");
                rp.RectifyState = "Register";
                dbContext.T_HSE_RectifySheet_RectifyProblems.Add(rp);
            }
        }

        protected override void BeforeDelete(string[] Ids)
        {
            base.BeforeDelete(Ids);
            //删除的问题已经处于整改的流程中或者已结束，则不能够删除问题
            foreach (string id in Ids)
            {
                var safeCheck = dbContext.T_HSE_SafeCheck.Find(id);
                if (safeCheck == null)
                    continue;

                var records = safeCheck.T_HSE_SafeCheck_CheckContentRecord;
                foreach (var record in records)
                {
                    var problem = dbContext.T_HSE_RectifySheet_RectifyProblems.SingleOrDefault(a => a.T_HSE_SafeCheck_CheckContentRecordID == record.ID);
                    if (problem != null)
                    {
                        if (dbContext.T_HSE_RectifySheet.Any(a => a.ID == problem.T_HSE_RectifySheetID))
                        {
                            throw new Formula.Exceptions.BusinessValidationException("HSE检查记录【" + safeCheck.CheckSubject + "】中有问题【" + record.CheckContent + "】在整改,因此不能删除");
                        }
                        //同时删除在问题整改单列表的问题记录
                        else
                        {
                            dbContext.T_HSE_RectifySheet_RectifyProblems.Delete(a => a.T_HSE_SafeCheck_CheckContentRecordID == record.ID);
                        }
                    }
                }
            }
        }

        //protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        //{
        //    var saCheck = this.GetEntityByID(dic["ID"]);
            
        //    List<T_HSE_SafeCheck_CheckContentRecord> ccR = new List<T_HSE_SafeCheck_CheckContentRecord>();
        //    if (saCheck != null)
        //    {
        //        ccR = dbContext.T_HSE_SafeCheck_CheckContentRecord.Where(x => x.T_HSE_SafeCheckID == saCheck.ID).ToList();
        //    }
        //    if (ccR.Count > 0)
        //    {
        //        foreach (var item in ccR)
        //        {
        //            bool exist = dbContext.T_HSE_RectifySheet_RectifyProblems.Where(m => m.T_HSE_SafeCheck_CheckContentRecordID == item.ID).FirstOrDefault() == null ? false : true;
        //            if (!exist)
        //            { 
        //                T_HSE_RectifySheet_RectifyProblems rp = new T_HSE_RectifySheet_RectifyProblems();
        //                rp.ID = FormulaHelper.CreateGuid();
        //                rp.T_HSE_SafeCheck_CheckContentRecordID = item.ID;
        //                rp.Code = item.SerialNumber;
        //                rp.Problems = item.MainProblems;
        //                rp.EngineeringInfo = saCheck.EngineeringInfo;
        //                rp.EngineeringInfoName = saCheck.EngineeringInfoName;
        //                dbContext.T_HSE_RectifySheet_RectifyProblems.Add(rp);
        //            }
        //        }
        //    }
        //    dbContext.SaveChanges();
        //}

        //public void DeleteSafeCheck()
        //{
        //    if (!String.IsNullOrEmpty(Request["ListIDs"]))
        //    {
        //        Del();
        //    }
        //}

        //private void Del()
        //{
        //    T_HSE_SafeCheck sc = new T_HSE_SafeCheck();
        //    List<T_HSE_SafeCheck_CheckContentRecord> scc = new List<T_HSE_SafeCheck_CheckContentRecord>();
        //    T_HSE_RectifySheet_RectifyProblems rp = new T_HSE_RectifySheet_RectifyProblems();
        //    T_HSE_RectifySheet rs = new T_HSE_RectifySheet();

        //    foreach (var item in Request["ListIDs"].Split(','))
        //    {
        //        sc = dbContext.T_HSE_SafeCheck.Where(x => x.ID == item).FirstOrDefault();
        //        scc = sc == null ? null : dbContext.T_HSE_SafeCheck_CheckContentRecord.Where(y => y.T_HSE_SafeCheckID == item).ToList();
        //        if (scc != null)
        //        {
        //            foreach (var obj in scc)
        //            {
        //                rp = dbContext.T_HSE_RectifySheet_RectifyProblems.Where(m => m.T_HSE_SafeCheck_CheckContentRecordID == obj.ID).FirstOrDefault();
        //                rs = rp == null ? null : dbContext.T_HSE_RectifySheet.Where(n => n.ID == rp.T_HSE_RectifySheetID).FirstOrDefault();
        //                if (rs == null)
        //                {
        //                    dbContext.T_HSE_RectifySheet_RectifyProblems.Delete(i => i.T_HSE_SafeCheck_CheckContentRecordID == obj.ID);
        //                    dbContext.T_HSE_SafeCheck_CheckContentRecord.Delete(e => e.ID == obj.ID);
        //                }
        //                else
        //                {
        //                    throw new Formula.Exceptions.BusinessValidationException("问题【" + obj.MainProblems + "】已经发起问题整改，不能进行删除操作！");
        //                }
        //            }
        //        }
        //        dbContext.T_HSE_SafeCheck.Delete(s => s.ID == item);
        //    }
        //    dbContext.SaveChanges();
        //}

        //public void RemoveCheckContentRecord(string CheckContentRecordData)
        //{
        //    var list = JsonHelper.ToList(CheckContentRecordData);
        //    if(list!=null)
        //    { 
        //        foreach (var item in list)
        //        {
        //            var ccrObject = this.GetEntityByID<T_HSE_SafeCheck_CheckContentRecord>(item.GetValue("ID"));
        //            if (ccrObject == null) return;
        //            var rp = dbContext.T_HSE_RectifySheet_RectifyProblems.Where(x => x.T_HSE_SafeCheck_CheckContentRecordID == ccrObject.ID).FirstOrDefault();
        //            var rs = rp==null?null:dbContext.T_HSE_RectifySheet.Where(m => m.ID == rp.T_HSE_RectifySheetID).FirstOrDefault();
        //            if (rs != null)
        //            {
        //                throw new Formula.Exceptions.BusinessValidationException("问题【" + item.GetValue("MainProblems") + "】已经发起问题整改，不能进行删除操作！");
        //            }
        //            else
        //            {
        //                dbContext.T_HSE_SafeCheck_CheckContentRecord.Delete(y => y.ID == ccrObject.ID);
        //                dbContext.T_HSE_RectifySheet_RectifyProblems.Delete(n => n.T_HSE_SafeCheck_CheckContentRecordID == ccrObject.ID);
        //            }
        //        }
        //        dbContext.SaveChanges();
        //    }

        //}
    }
}
