using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class ConstructionQuantityController : InfrastructureController<S_T_ConstructionQuantity>
    {
        public override ActionResult List()
        {
            ViewBag.IsForImport = GetQueryString("IsForImport");
            return View();
        }
        public ActionResult NodeAddOrEdit()
        {
            return View();
        }
        
        public JsonResult GetTreeList()
        {
            var exsitCount = this.entities.Set<S_T_ConstructionQuantity>().Count(a => a.ParentID == "");
            if (exsitCount == 0)
                CreateRoot();

            var result = entities.Set<S_T_ConstructionQuantity>().OrderBy(a => a.SortIndex);
            return Json(result);
        }

        public JsonResult ThisDeleteNode(string FullID)
        {
            S_T_ConstructionQuantity quantity = entities.Set<S_T_ConstructionQuantity>().SingleOrDefault(a => a.FullID == FullID);
            JsonResult jr = new JsonResult();

            if (quantity == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到FullID为" + FullID + "的S_T_ConstructionQuantity");
            }
            else if (quantity.ParentID == "")
            {
                throw new Formula.Exceptions.BusinessValidationException("不能删除根节点");
            }
            var allChildren = GetAllChildren(quantity);
            foreach (var tmp in allChildren)
            {
                entities.Set<S_T_ConstructionQuantity>().Remove(tmp);
            }
            entities.Set<S_T_ConstructionQuantity>().Delete(a => a.FullID == FullID);
            entities.SaveChanges();
            return Json("{}");
        }

        public override JsonResult SaveList()
        {
            var qID = GetQueryString("QuantityID");
            //验证编号不能重复
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(Request["ListData"]);
            var groups = rows.GroupBy(a => a.GetValue("Code")).Select(g => new { g.Key, Counts = g.Count() }).ToList();
            if (groups.Count(a => a.Counts > 1) > 0)
                throw new Formula.Exceptions.BusinessValidationException("编号【" + groups.FirstOrDefault(a => a.Counts > 1).Key + "】重复");

            
            foreach (var item in rows)
            {
                string id = item.GetValue("ID");
                string code = item.GetValue("Code");
                if (entities.Set<S_T_ConstructionQuantityDetail>()
                    .Any(a => a.ID != id && a.Code == code))
                {
                    throw new Formula.Exceptions.BusinessValidationException("编号【" + item.GetValue("Code") + "】重复");
            }
            }

            //this.entities.Set<S_T_ConstructionQuantityDetail>().GroupBy(a => a.Code).Select(a => new { a.Key });
            return base.JsonSaveList<S_T_ConstructionQuantityDetail>(Request["ListData"]);

        }

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var qID = GetQueryString("QuantityID");
            S_T_ConstructionQuantity quantity = entities.Set<S_T_ConstructionQuantity>().Find(qID);
            if (quantity == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + qID + "的S_T_ConstructionQuantity");
            }

            List<string> tmpCodes = new List<string>();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Code")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "清单编号不能为空";
                    }
                    else if (tmpCodes.Contains(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "excel表中清单编号重复";
                    }
                    else if (entities.Set<S_T_ConstructionQuantityDetail>().Any(a => a.Code == e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "清单编号已存在";
                    }
                    else
                    {
                        tmpCodes.Add(e.Value);
                    } 
                }
                else if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "名称不能为空";
                }
            });
            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);

            var qID = GetQueryString("QuantityID");
            S_T_ConstructionQuantity quantity = entities.Set<S_T_ConstructionQuantity>().Find(qID);
            if (quantity == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + qID + "的S_T_ConstructionQuantity");
            }

            var groups = list.GroupBy(a => a.GetValue("Code")).Select(g => new { g.Key, Counts = g.Count() }).ToList();
            if (groups.Count(a => a.Counts > 1) > 0)
                throw new Formula.Exceptions.BusinessValidationException("编号【" + groups.FirstOrDefault(a => a.Counts > 1).Key + "】重复");

            foreach (var item in list)
            {
                string code = item.GetValue("Code");
                if (entities.Set<S_T_ConstructionQuantityDetail>()
                    .Any(a => a.Code == code))
                {
                    throw new Formula.Exceptions.BusinessValidationException("编号【" + item.GetValue("Code") + "】重复");
                }

                S_T_ConstructionQuantityDetail detail = new S_T_ConstructionQuantityDetail();
                detail.ID = FormulaHelper.CreateGuid();                
                detail.QuantityID = quantity.ID;
                detail.QuantityFullID = quantity.FullID;
                detail.Code = item.GetValue("Code");
                detail.Name = item.GetValue("Name");
                detail.Property = item.GetValue("Property");
                detail.Unit = item.GetValue("Unit");
                detail.Remark = item.GetValue("Remark");

                detail.CreateDate = DateTime.Now;
                detail.CreateUserID = CurrentUserInfo.UserID;
                detail.CreateUser = CurrentUserInfo.UserName;
                detail.CompanyID = CurrentUserInfo.UserCompanyID;
                detail.OrgID = CurrentUserInfo.UserOrgID;

                entities.Set<S_T_ConstructionQuantityDetail>().Add(detail);
        }

            entities.SaveChanges();            
            return Json("Success");
        }

        public JsonResult GetDetailList(QueryBuilder qb)
        {
            var qID = GetQueryString("qID");
            qb.Add("QuantityFullID", QueryMethod.Contains, qID);
            return base.JsonGetList<S_T_ConstructionQuantityDetail>(qb);
        }

        private List<S_T_ConstructionQuantity> GetAllChildren(S_T_ConstructionQuantity quantity)
        {
            List<S_T_ConstructionQuantity> temps = new List<S_T_ConstructionQuantity>();
            List<S_T_ConstructionQuantity> children = entities.Set<S_T_ConstructionQuantity>().Where(a => a.ParentID == quantity.ID).ToList();
            foreach (var child in children)
            {
                temps.Add(child);
                temps.AddRange(GetAllChildren(child));
                foreach(var tmp in child.S_T_ConstructionQuantityDetail.ToList())
                {
                    entities.Set<S_T_ConstructionQuantityDetail>().Remove(tmp);
                }
            }
            return temps;
        }

        private void CreateRoot()
        {
            var root = new S_T_ConstructionQuantity();
            root.ID = FormulaHelper.CreateGuid();
            root.Name = "清单工程量分类";
            root.ParentID = "";
            root.FullID = root.ID;
            //root.Type = (int)ConstructionQuantityEnum.Top;
            this.EntityCreateLogic<S_T_ConstructionQuantity>(root);
            this.entities.Set<S_T_ConstructionQuantity>().Add(root);
            this.entities.SaveChanges();
        }
    }
}
