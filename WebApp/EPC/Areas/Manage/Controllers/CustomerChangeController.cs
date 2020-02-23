using Config.Logic;
using EPC.Logic.Domain;
using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Workflow.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class CustomerChangeController : EPCFormContorllor<T_C_CustomerinfoChange>
    {
        //
        // GET: /Manage/CustomerChange
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string CustomerID = GetQueryString("CustomerID");
                if (!string.IsNullOrEmpty(CustomerID))
                {
                    int maxNumber = 0;
                    EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
                    var CustomerInfo = epcentity.Set<S_M_CustomerInfo>().FirstOrDefault(a => a.ID == CustomerID);
                    var Customers = epcentity.Set<T_C_CustomerinfoChange>().Where(c => c.CustomerID == CustomerID);

                    if (Customers != null && Customers.Count() > 0)
                    {
                        maxNumber = Customers.ToList().Max(c => Convert.ToInt32(c.VersionNumber));
                    }
                    maxNumber++;
                  
                    if (CustomerInfo != null)
                    {
                        Type tp = CustomerInfo.GetType();
                        var ps = tp.GetProperties();
                        DataRow dtr = dt.Rows[0];
                        var CustomerInfoLinkMan = epcentity.Set<S_M_CustomerInfo_LinkMan>().Where(a => a.S_M_CustomerInfoID == CustomerID);
                        string lms = "";
                        if (CustomerInfoLinkMan != null && CustomerInfoLinkMan.Count() > 0) {
                            List<S_M_CustomerInfo_LinkMan> smcil = CustomerInfoLinkMan.ToList();
                            List<T_C_CustomerinfoChange_LinkMan> tcclm = new List<T_C_CustomerinfoChange_LinkMan>();
                            foreach (S_M_CustomerInfo_LinkMan item in smcil) {
                                T_C_CustomerinfoChange_LinkMan smclk = new T_C_CustomerinfoChange_LinkMan();
                                smclk.Name = item.Name;
                                smclk.Position = item.Position;
                                smclk.Sex = item.Sex;
                                smclk.Phone = item.Phone;
                                smclk.EMail = item.EMail;
                                tcclm.Add(smclk);
                            }
                            lms =JsonConvert.SerializeObject(tcclm);
                           // JavaScriptSerializer js = new JavaScriptSerializer();
                            //lms=js.Serialize(smcil);
                        }
                       
                        foreach (PropertyInfo item in ps)
                        {
                            string cname = item.Name;
                            string type = item.PropertyType.Name;
                            if (type != "ICollection`1" && cname != "CustomerID" && cname != "VersionNumber" && dt.Columns.Contains(cname))
                            {
                                object value = item.GetValue(CustomerInfo, null);
                                if (cname == "ID")
                                {
                                    dtr.SetField("CustomerID", value);
                                }
                                else
                                {
                                    dtr.SetField(cname, value);
                                }
                            }
                        }
                        dtr.SetField("VersionNumber", maxNumber);
                        dtr.SetField("LinkMan", lms);
                    }
                }
            }
        }
        protected override void OnFlowEnd(T_C_CustomerinfoChange CustomerInfo, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
            string CustomerID = CustomerInfo.CustomerID;
            var model = epcentity.Set<S_M_CustomerInfo>().FirstOrDefault(a => a.ID == CustomerID);
            if (model != null && CurrentUserInfo != null)
            {
                model.Name = CustomerInfo.Name;
                model.Socialcreditcode = CustomerInfo.Socialcreditcode;
                model.Highunit = CustomerInfo.Highunit;
                model.HighunitName = CustomerInfo.HighunitName;
                model.SimpleName = CustomerInfo.SimpleName;
                model.Englishname = CustomerInfo.Englishname;
                model.Usedname1 = CustomerInfo.Usedname1;
                model.Usedname2 = CustomerInfo.Usedname2;
                model.Customersite = CustomerInfo.Customersite;
                model.Related = CustomerInfo.Related;
                model.JuridicalPerson = CustomerInfo.JuridicalPerson;
                model.Registered = CustomerInfo.Registered;
                model.Registeredcapital = CustomerInfo.Registeredcapital;
                model.Enterprisenature = CustomerInfo.Enterprisenature;
                model.Industrystatus = CustomerInfo.Industrystatus;
                model.Setupdate = CustomerInfo.Setupdate;
                model.Country = CustomerInfo.Country;
                model.Businesstypes = CustomerInfo.Businesstypes;
                model.ZipCode = CustomerInfo.ZipCode; 
                model.Province = CustomerInfo.Province;
                model.City = CustomerInfo.City;
                model.Primarycontact = CustomerInfo.Primarycontact;
                model.Contactphone = CustomerInfo.Contactphone;
                model.Fax = CustomerInfo.Fax;
                model.Email = CustomerInfo.Email;
                model.Companyprofile = CustomerInfo.Companyprofile;
                model.Enterpriseculture = CustomerInfo.Enterpriseculture;
                model.Registrar = CustomerInfo.Registrar;
                model.RegistrarName = CustomerInfo.RegistrarName;
                model.Registrationdate = CustomerInfo.Registrationdate;
                model.InvoiceName = CustomerInfo.InvoiceName;
                model.TaxAccounts = CustomerInfo.TaxAccounts;
                model.BankName = CustomerInfo.BankName;
                model.BankAccounts = CustomerInfo.BankAccounts;
                model.BankAddress = CustomerInfo.BankAddress;
                model.Telephone = CustomerInfo.Telephone;
                model.Attachments = CustomerInfo.Attachments;
                model.Remark = CustomerInfo.Attachments;
                model.Businessscope = CustomerInfo.Businessscope;
                var linkmans = epcentity.Set<S_M_CustomerInfo_LinkMan>().Where(a => a.S_M_CustomerInfoID == CustomerID);
                if (linkmans != null && linkmans.Count() > 0) {
                    var listmo = linkmans.ToList();
                    listmo.ForEach(c => {
                        epcentity.Set<S_M_CustomerInfo_LinkMan>().Remove(c);
                    });
                }
                var lkms = CustomerInfo.T_C_CustomerinfoChange_LinkMan;
                if (lkms != null) {
                    List<T_C_CustomerinfoChange_LinkMan> listlm = lkms.ToList();
                    foreach (T_C_CustomerinfoChange_LinkMan item in listlm) {
                        S_M_CustomerInfo_LinkMan smclk = new S_M_CustomerInfo_LinkMan();
                        smclk.Name = item.Name;
                        smclk.Position = item.Position;
                        smclk.Sex = item.Sex;
                        smclk.Phone = item.Phone;
                        smclk.EMail = item.EMail;
                        smclk.ID = FormulaHelper.CreateGuid();
                        smclk.S_M_CustomerInfoID = CustomerID;
                        epcentity.Set<S_M_CustomerInfo_LinkMan>().Add(smclk);
                    }
                }
                epcentity.SaveChanges();
            }
        }

        public JsonResult ValidateChange(string CustomerID)
        {
          
            var result = new Dictionary<string, object>();
            string sql = String.Format("select ID from T_C_CustomerinfoChange where CustomerID='{0}' and FlowPhase='{1}'", CustomerID, "Start");
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                result.SetValue("ID", dt.Rows[0]["ID"]);
                return Json(result);
            }
            sql = String.Format("select Count(ID) from T_C_CustomerinfoChange where CustomerID='{0}' and FlowPhase='{1}'", CustomerID, "Processing");
            var obj = Convert.ToInt32(this.EPCSQLDB.ExecuteScalar(sql));
            if (obj > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("顾客信息正在变更中，无法重复启动变更，请在变更结束后再启动变更");
            }
            return Json(result);
        }

    }
}
