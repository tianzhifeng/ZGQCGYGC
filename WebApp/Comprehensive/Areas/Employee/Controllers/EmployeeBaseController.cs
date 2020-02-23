using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Formula;
using System.Drawing.Imaging;
using System.Drawing;
using Base.Logic.Domain;
using System.Web.Security;
using Comprehensive.Logic.BusinessFacade;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeBaseController : ComprehensiveFormController<S_HR_Employee>
    {

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //model.SynchPostTemplate();
            base.BeforeSave(dic, formInfo, isNew);
        }

        public override JsonResult Delete()
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            ComprehensiveDbContext.Set<S_HR_Employee>().Where(c => idArray.Contains(c.ID)).Update(c =>
            {
                c.IsDeleted = "1";
                c.DeleteTime = DateTime.Now;
            });
            ComprehensiveDbContext.SaveChanges();
            EmployeeFo fo = new EmployeeFo();
            fo.EmployeeDeleteToUser(listIDs);
            return Json("");
            //return base.Delete();
        }

        public JsonResult UploadImg(string imageType)
        {
            S_HR_Employee entity = UpdateEntity<S_HR_Employee>();

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                switch (imageType)
                {
                    case "Portrait":
                        entity.Portrait = bt;
                        break;
                    case "IdentityCardFace":
                        entity.IdentityCardFace = bt;
                        break;
                    case "IdentityCardBack":
                        entity.IdentityCardBack = bt;
                        break;
                    case "Sign":
                        entity.SignImage = bt;
                        break;
                }

                ComprehensiveDbContext.SaveChanges();
            }
            return Json(new { ID = entity.ID, ImageType = imageType });
        }

        public ActionResult GetPic(string id, string imageType)
        {
            ImageActionResult result = null;
            IUserService service = FormulaHelper.GetService<IUserService>();

            S_HR_Employee entity = ComprehensiveDbContext.Set<S_HR_Employee>().Find(id);
            byte[] img = null;
            if (entity != null)
            {
                switch (imageType)
                {
                    case "Portrait":
                        img = entity.Portrait;
                        break;
                    case "IdentityCardFace":
                        img = entity.IdentityCardFace;
                        break;
                    case "IdentityCardBack":
                        img = entity.IdentityCardBack;
                        break;
                    case "Sign":
                        img = entity.SignImage;
                        break;
                }

                if (img != null)
                {
                    Image image = ImageHelper.BytesToImage(img);
                    if (image != null)
                    {
                        ImageFormat imageFormat = ImageHelper.GetImageFormat(image);
                        result = new ImageActionResult(image, imageFormat);
                    }
                }
            }
            return result;
        }
        
        public JsonResult DeleteImage(string imageType)
        {
            string id = GetQueryString("ID");
            S_HR_Employee entity = ComprehensiveDbContext.Set<S_HR_Employee>().Find(id);
            if (entity != null)
            {

                switch (imageType)
                {
                    case "Portrait":
                        entity.Portrait = null;
                        break;
                    case "IdentityCardFace":
                        entity.IdentityCardFace = null;
                        break;
                    case "IdentityCardBack":
                        entity.IdentityCardBack = null;
                        break;
                    case "Sign":
                        entity.SignImage = null;
                        break;
                }
            }
            ComprehensiveDbContext.SaveChanges();
            return Json(new { ID = id, ImageType = imageType });
        }
        
        public JsonResult SyncSystemUser()
        {
            string Ids = Request["IDs"];
            if (string.IsNullOrWhiteSpace(Ids))
                return Json("");
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            EmployeeFo fo = new EmployeeFo();
            List<S_HR_Employee> employeeList = this.ComprehensiveDbContext.Set<S_HR_Employee>().Where(c => c.IsHaveAccount == "1" && Ids.Contains(c.ID)).ToList();
            if (employeeList == null || employeeList.Count == 0)
                return Json("");
            foreach (S_HR_Employee employee in employeeList)
            {
                if (string.IsNullOrEmpty(employee.UserID))
                    fo.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                else
                {
                    var user = baseEntities.Set<S_A_User>().Find(employee.UserID);
                    if (user == null)
                        fo.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                    else if (employee.IsDeleted == "0") //Update
                        fo.EmployeeUpdateToUser(employee);
                    else
                        fo.EmployeeDeleteToUser(employee);//Delete
                }
            }
            return Json(JsonAjaxResult.Successful());
        }


        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var orgService = FormulaHelper.GetService<IOrgService>();
            var orgs = orgService.GetOrgs();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Code" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("员工编号不能为空", e.Value);
                }
                if (e.FieldName == "DeptName")
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var dept = orgs.FirstOrDefault(o => o.Name == e.Value);
                        if (dept == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("部门（{0}）不存在！", e.Value);
                        }
                    }
                    else
                    {
                        e.IsValid = false;
                        e.ErrorText = "部门字段不能为空！";
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_HR_Employee>>(tempdata["data"]);
            var currentUser = FormulaHelper.GetUserInfo();
            var orgService = FormulaHelper.GetService<IOrgService>();
            var orgs = orgService.GetOrgs();
            foreach (var user in list)
            {
                if (string.IsNullOrWhiteSpace(user.Code))
                    user.Code = user.Name;
                var hrUser = this.ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(d => d.Code == user.Code);
                if (hrUser != null) continue;
                if (hrUser == null)
                {
                    hrUser = this.ComprehensiveDbContext.Set<S_HR_Employee>().Create();
                    hrUser.ID = FormulaHelper.CreateGuid();
                    hrUser.Code = user.Code;
                    hrUser.IsHaveAccount = "1";
                    this.ComprehensiveDbContext.Set<S_HR_Employee>().Add(hrUser);
                }
                hrUser.Name = user.Name;
                hrUser.MobilePhone = user.MobilePhone;
                hrUser.OfficePhone = user.OfficePhone;
                hrUser.NativePlace = user.NativePlace;
                hrUser.Nation = user.Nation;
                hrUser.Birthday = user.Birthday;
                hrUser.Address = user.Address;
                hrUser.CreateDate = DateTime.Now;
                hrUser.CreateUser = currentUser.UserName;
                hrUser.CreateUserID = currentUser.UserID;
                hrUser.MaritalStatus = user.MaritalStatus;
                hrUser.JoinWorkDate = user.JoinWorkDate;
                hrUser.JoinCompanyDate = user.JoinCompanyDate;
                hrUser.IsDeleted = "0";
                hrUser.IdentityCardCode = user.IdentityCardCode;

                if (!String.IsNullOrEmpty(user.DeptName))
                {
                    var dept = orgs.FirstOrDefault(d => d.Name == user.DeptName);
                    if (dept != null)
                    {
                        hrUser.Dept = dept.ID;
                        hrUser.DeptName = dept.Name;
                    }
                }
                hrUser.Sex = user.Sex;
                hrUser.Educational = user.Educational;
                hrUser.Political = user.Political;
                hrUser.EmploymentWay = user.EmploymentWay;
                hrUser.Email = user.Email;
                ComprehensiveDbContext.SaveChanges();
            }
            return Json("Success");
        }

        #endregion
    }
}
