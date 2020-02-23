using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using HR.Logic.Domain;
using MvcAdapter;
using Formula;
using System.Drawing;
using System.Drawing.Imaging;
using Formula.Helper;
using System.Reflection;
using HR.Logic.BusinessFacade;
using Base.Logic.Domain;
using System.Web.Security;
using Newtonsoft.Json;
using Formula.ImportExport;
using Config.Logic;
using Base.Logic.Model.UI.Form;
using Workflow.Logic;
using Formula.Exceptions;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeBaseController : HRFormContorllor<T_Employee>
    {
        public JsonResult GetEmployeeInfo(string EmployeeID)
        {
            var Model = BusinessEntities.Set<T_Employee>().Where(t => t.ID == EmployeeID).FirstOrDefault();
            return Json(Model);
        }

        #region 拼接tab

        public ActionResult Tab()
        {
            string Way = EmploymentWay.正式员工.ToString();
            string EmployeeID = Request["ID"];
            string FuncType = Request["FuncType"];
            ViewBag.TabHtml = EmployeeServiceAuto.BuildTabs(EmployeeID, FuncType, Way);
            return View();
        }
        #endregion

        #region 员工基本信息

        public JsonResult DeleteEmployee()
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            BusinessEntities.Set<T_Employee>().Where(c => idArray.Contains(c.ID)).Update(c =>
            {
                c.IsDeleted = "1";
                c.DeleteTime = DateTime.Now;
            });
            BusinessEntities.SaveChanges();
            EmployeeServiceAuto auto = new EmployeeServiceAuto();
            auto.EmployeeDeleteToUser(listIDs);

            var userIDs = this.BusinessEntities.Set<T_Employee>().Where(a => idArray.Contains(a.ID)).Select(a => a.UserID).ToList();
            var userAptitudes = this.BusinessEntities.Set<S_D_UserAptitude>().Where(a => userIDs.Contains(a.UserID)).ToList();
            userAptitudes.ForEach(a => this.BusinessEntities.Set<S_D_UserAptitude>().Remove(a));
            this.BusinessEntities.SaveChanges();

            return Json("");
        }

        /// <summary>
        /// 根据员工大类获取员工小类
        /// </summary>
        /// <param name="bigType"></param>
        /// <returns></returns>
        public JsonResult GetEmployeeSmallTypeEnum(string bigType)
        {
            List<object> list = CommonMethod.GetSubEnum("HR.EmployeeSmallTypeNew", bigType);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetForeignLevelEnum(string foreignLanguage)
        {
            List<object> list = CommonMethod.GetSubEnum("HR.ForeignLanguageLevel", foreignLanguage);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载Dwg文件
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public FileResult DownloadDwg(string ID)
        {
            T_Employee entity = BusinessEntities.Set<T_Employee>().Find(ID);
            if (entity == null) throw new BusinessException("未找到此员工信息");
            byte[] dwgFile = entity.SignDwg;
            if (dwgFile == null || dwgFile.Length == 0) throw new BusinessException("尚未上传Dwg签名文件");
            string contentType = "application/x-dwg";
            return File(dwgFile, contentType, entity.Name + ".dwg");
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="imageType">图片类型</param>
        /// <returns></returns>
        public JsonResult UploadImg(string imageType)
        {
            T_Employee entity = UpdateEntity<T_Employee>();

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
                    case "Dwg":
                        entity.SignDwg = bt;
                        break;
                }

                BusinessEntities.SaveChanges();
            }
            return Json(new { ID = entity.ID, ImageType = imageType });
        }

        public ActionResult GetPic(string id, string imageType)
        {
            ImageActionResult result = null;
            IUserService service = FormulaHelper.GetService<IUserService>();

            T_Employee entity = BusinessEntities.Set<T_Employee>().Find(id);
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
            T_Employee entity = BusinessEntities.Set<T_Employee>().Find(id);
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
                    case "Dwg":
                        entity.SignDwg = null;
                        break;
                }
            }
            BusinessEntities.SaveChanges();
            return Json(new { ID = id, ImageType = imageType });
        }

        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var id = dic.GetValue("ID");
            var model = this.BusinessEntities.Set<T_Employee>().FirstOrDefault(a => a.ID == id);
            model.SynchPostTemplate();
            model.IsDeleted = "0";
            model.EmployeeState = EmployeeState.Incumbency.ToString();

            //同步至系统用户
            if (model.IsHaveAccount == "1")
            {
                SyncEmployee(model);
            }

            var currentUser = FormulaHelper.GetUserInfo();

            //薪酬信息录入新员工薪酬
            var newEmployee = BusinessEntities.Set<T_Employee_Newemployeesalayinformation>().Where(p => p.Newstaffname == model.UserID).FirstOrDefault();

            if (newEmployee == null)
            {
                var nowTime = DateTime.Now;
                var newEmployeeEntity = new T_Employee_Newemployeesalayinformation()
                {
                    ID = FormulaHelper.CreateGuid(),
                    CreateUserID = currentUser.UserID,
                    CreateUser = currentUser.UserName,
                    CreateDate = nowTime,

                    Fillname = currentUser.UserID,
                    FillnameName = currentUser.UserName,
                    Filltime = nowTime,
                    Newstaffname = model.UserID,
                    NewstaffnameName = model.Name,
                    Number = model.Code,
                    Workdate = model.JoinCompanyDate,
                    Department = model.DeptID,
                    DepartmentName = model.DeptName,
                    Job = model.Post,
                    Title = model.PositionalTitles,
                    Educationalbackground = model.Educational,
                };

                if (model.EmployeeSource == "社会招聘")
                {
                    var social = BusinessEntities.Set<S_E_Recruitment_Socialrecruitmentemployment>().Find(model.Interview);
                    if (social != null)
                    {
                        newEmployeeEntity.Annualsalary = social.Annualsalary;
                        newEmployeeEntity.Monthsalary = social.Monthlysalary;
                        newEmployeeEntity.Pretest = social.Pretest;
                    }
                }
            }
            else
            {
                //if (model.EmployeeSource == "社会招聘")
                //{
                //    var social = BusinessEntities.Set<S_E_Recruitment_Socialrecruitmentemployment>().Find(model.Interview);
                //    if (social != null)
                //    {
                //        newEmployee.Annualsalary = social.Annualsalary;
                //        newEmployee.Monthsalary = social.Monthlysalary;
                //        newEmployee.Pretest = social.Pretest;
                //    }
                //}
            }

            BusinessEntities.SaveChanges();
            //base.AfterSave(dic, formInfo, isNew);
        }

        public JsonResult MultiSave(string IDs)
        {
            string formData = Request["FormData"];
            Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
            StringBuilder sql = new StringBuilder();
            var dtField = DbHelper.GetFieldTable(ConnEnum.HR.ToString(), "T_Employee").AsEnumerable();
            foreach (var item in IDs.Split(','))
            {
                sql.AppendLine(dic.CreateUpdateSql(ConnEnum.HR.ToString(), "T_Employee", item, dtField));
            }
            if (sql.ToString() != "")
                if (Config.Constant.IsOracleDb)
                {
                    this.HRSQLDB.ExecuteNonQuery(string.Format(@"
begin
{0}
end;", sql));
                }
                else
                {
                    this.HRSQLDB.ExecuteNonQuery(sql.ToString());
                }
            return Json("");
        }

        public JsonResult GetJobList(QueryBuilder qb, string employeeID)
        {
            var data = BusinessEntities.Set<T_EmployeeJob>().Where(c => c.EmployeeID == employeeID && c.IsDeleted == "0").WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult ReturnCheckExistCode()
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            if (idArray.Length == 1)
            {
                var id = idArray[0];
                var emp = BusinessEntities.Set<T_Employee>().FirstOrDefault(a => a.ID == id);
                var exist = BusinessEntities.Set<T_Employee>().Any(c => c.ID != emp.ID && c.IsDeleted == "0" && c.Code == emp.Code);
                if (exist)
                    return Json(true);
                else
                    return Json(false);
            }
            else
            {
                var error = string.Empty;
                var list = BusinessEntities.Set<T_Employee>().ToList();
                foreach (var emp in list.Where(a => idArray.Contains(a.ID)).ToList())
                {
                    var exist = list.Any(c => c.ID != emp.ID && c.IsDeleted == "0" && c.Code == emp.Code);
                    if (exist)
                        error += emp.Code + "，";
                }
                if (!string.IsNullOrEmpty(error))
                    return Json("账号【" + error.TrimEnd('，') + "】已经存在，只能批量操作不重复的账号");
                else
                    return Json("");
            }
        }

        #endregion

        #region 员工合同
        /// <summary>
        /// 根据合同类别获取合同形式
        /// </summary>
        /// <param name="bigType"></param>
        /// <returns></returns>
        public JsonResult GetContractShapeEnum(string contractCategory)
        {
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='HR.EmployeeContractShape') AND Category like '%{0}%' Order by SortIndex", contractCategory);
            DataTable dt = baseHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list);
        }

        //保存合同信息时，更新人员基本信息表里面的合同
        public JsonResult SaveContract()
        {
            var contract = UpdateEntity<T_EmployeeContract>();
            //校验合同编号是否唯一
            var SameModel = BusinessEntities.Set<T_EmployeeContract>().Where(t => t.Code == contract.Code && t.ID != contract.ID).FirstOrDefault();
            if (SameModel != null)
            {
                throw new BusinessException("合同编号【" + SameModel.Code + "】已存在，请重新输入。");
            }
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(contract.EmployeeID);
            if (employee != null)
            {
                var lastContract = BusinessEntities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == employee.ID).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (lastContract != null)
                {
                    employee.ContractType = lastContract.ContractCategory;
                    employee.DeterminePostsDate = lastContract.PostDate;
                    BusinessEntities.SaveChanges();
                }
            }

            return Json(new { ID = contract.ID });
        }

        //删除合同信息时，更新人员基本信息表里面的合同
        public JsonResult DeleteContract()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            BusinessEntities.Set<T_EmployeeContract>().Delete(c => idArray.Contains(c.ID));
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var lastContract = BusinessEntities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == employee.ID).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (lastContract != null)
                {
                    employee.ContractType = lastContract.ContractCategory;
                    employee.DeterminePostsDate = lastContract.PostDate;
                }
                else
                {
                    employee.ContractType = "";
                    employee.DeterminePostsDate = null;
                }
                BusinessEntities.SaveChanges();
            }

            return Json("");
        }

        #endregion

        #region 学历
        public JsonResult SaveDegree()
        {
            T_EmployeeAcademicDegree degree = UpdateEntity<T_EmployeeAcademicDegree>();
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(degree.EmployeeID);
            if (employee != null)
            {
                var ad = BusinessEntities.Set<T_EmployeeAcademicDegree>().Where(c => c.EmployeeID == employee.ID).OrderBy("GraduationDate", false).FirstOrDefault();
                if (ad != null)
                {
                    if (DateTime.Compare((DateTime)ad.GraduationDate, (DateTime)degree.GraduationDate) > 0)
                    {
                        employee.Educational = ad.Education;
                        employee.EducationalMajor = ad.FirstProfession;
                    }
                    else
                    {
                        employee.Educational = degree.Education;
                        employee.EducationalMajor = degree.FirstProfession;
                    }
                }
                else
                {
                    employee.Educational = null;
                    employee.EducationalMajor = null;
                }
                BusinessEntities.SaveChanges();
            }
            return Json(new { ID = degree.ID });
        }

        public JsonResult DeleteDegree()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            BusinessEntities.Set<T_EmployeeAcademicDegree>().Delete(c => idArray.Contains(c.ID));
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                var ad = BusinessEntities.Set<T_EmployeeAcademicDegree>().Where(c => c.EmployeeID == employee.ID && !idArray.Contains(c.ID)).OrderBy("GraduationDate", false).FirstOrDefault();
                if (ad != null)
                {
                    employee.Educational = ad.Education;
                    employee.EducationalMajor = ad.FirstProfession;
                }
                else
                {
                    employee.Educational = null;
                    employee.EducationalMajor = null;
                }
                BusinessEntities.SaveChanges();
            }
            return Json("");
        }

        #endregion

        #region 职务信息

        public JsonResult GetJobEnumByDept(string DeptID)
        {
            List<object> list = new List<object>();
            string sql = string.Format("SELECT * From dbo.S_A_Org Where ParentID='{0}' and IsDeleted='0' and Type='{1}'", DeptID, OrgType.Post);
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = baseHelper.ExecuteDataTable(sql);

            list.Add(new { text = "普通员工", value = "GeneralEmployee" });
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new { text = row["Name"], value = row["ID"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveJob()
        {
            T_EmployeeJob job = UpdateEntity<T_EmployeeJob>();
            job.IsDeleted = "0";

            var employee = BusinessEntities.Set<T_Employee>().Find(job.EmployeeID);
            if (job.IsMain == "1")
            {
                var list = BusinessEntities.Set<T_EmployeeJob>().Where(c => c.IsMain == "1" && c.ID != job.ID && c.IsDeleted == "0" && c.EmployeeID == job.EmployeeID).ToList();
                if (list.Count > 0)
                    throw new BusinessException(string.Format("已存在主责部门职务【{0}:{1}】！", list.FirstOrDefault().DeptIDName, list.FirstOrDefault().JobName));

                employee.DeptID = job.DeptID;
                employee.DeptIDName = job.DeptIDName;
                employee.DeptName = job.DeptIDName;
                employee.JobID = job.JobID;
                employee.JobName = job.JobName;
            }
            else
            {
                var mailJob = BusinessEntities.Set<T_EmployeeJob>().Where(c => c.ID != job.ID && c.EmployeeID == job.EmployeeID && c.IsMain == "1" && c.IsDeleted == "0").FirstOrDefault();
                if (mailJob == null)
                {
                    employee.DeptID = null;
                    employee.DeptIDName = null;
                    employee.DeptName = null;
                    employee.JobID = null;
                    employee.JobName = null;
                }
            }

            if (job != null)
            {
                var allJobs = this.BusinessEntities.Set<T_EmployeeJob>().Where(c => c.EmployeeID == job.EmployeeID && c.ID != job.ID && c.IsMain == "0").ToList();

                string deptIds = string.Empty;
                string deptNames = string.Empty;
                if (allJobs != null && allJobs.Count > 0)
                {
                    var tempdeptIds = allJobs.Select(c => c.DeptID).Distinct();
                    var tempdeptNames = allJobs.Select(c => c.DeptIDName).Distinct();
                    if (tempdeptIds != null && tempdeptIds.Count() > 0)
                        deptIds = string.Join(",", tempdeptIds);
                    if (tempdeptNames != null && tempdeptNames.Count() > 0)
                        deptNames = string.Join(",", tempdeptNames);
                }

                if (!deptIds.Contains(job.DeptID) && job.IsMain == "0")
                {
                    if (!string.IsNullOrEmpty(deptIds))
                        deptIds += "," + job.DeptID;
                    if (!string.IsNullOrEmpty(deptNames))
                        deptNames += "," + job.DeptIDName;
                }

                employee.ParttimeDeptID = deptIds;
                employee.ParttimeDeptName = deptNames;
            }

            BusinessEntities.SaveChanges();

            return Json(new { ID = job.ID });
        }

        public JsonResult DeleteJob()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            BusinessEntities.Set<T_EmployeeJob>().Delete(c => idArray.Contains(c.ID));
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.DeptID = null;
                employee.DeptIDName = null;
                employee.DeptName = null;
                var ad = BusinessEntities.Set<T_EmployeeJob>().Where(c => c.EmployeeID == employee.ID && !idArray.Contains(c.ID) && c.IsMain == "1").FirstOrDefault();
                if (ad != null)
                {
                    employee.DeptID = ad.DeptID;
                    employee.DeptIDName = ad.DeptIDName;
                    employee.DeptName = ad.DeptIDName;
                }
                BusinessEntities.SaveChanges();
            }
            return Json("");
        }

        #endregion

        #region 职称信息

        public JsonResult SaveAcademicTitle()
        {
            T_EmployeeAcademicTitle job = UpdateEntity<T_EmployeeAcademicTitle>();
            var employee = BusinessEntities.Set<T_Employee>().Find(job.EmployeeID);
            if (employee != null)
            {
                employee.PositionalTitles = job.Title;
                var atiList = BusinessEntities.Set<T_EmployeeAcademicTitle>().Where(c => c.EmployeeID == employee.ID && c.ID != job.ID).ToList();
                foreach (T_EmployeeAcademicTitle item in atiList)
                {
                    employee.PositionalTitles += "," + item.Title;
                }
                if (!string.IsNullOrEmpty(employee.PositionalTitles))
                    employee.PositionalTitles = employee.PositionalTitles.TrimEnd(',');
            }
            BusinessEntities.SaveChanges();
            return Json(new { ID = job.ID });
        }

        public JsonResult DeleteAcademicTitle()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            BusinessEntities.Set<T_EmployeeAcademicTitle>().Delete(c => idArray.Contains(c.ID));
            BusinessEntities.SaveChanges();

            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.PositionalTitles = "";
                var atiList = BusinessEntities.Set<T_EmployeeAcademicTitle>().Where(c => c.EmployeeID == employee.ID && !idArray.Contains(c.ID)).ToList();
                foreach (T_EmployeeAcademicTitle item in atiList)
                {
                    employee.PositionalTitles += item.Title + ",";
                }
                if (!string.IsNullOrEmpty(employee.PositionalTitles))
                    employee.PositionalTitles = employee.PositionalTitles.TrimEnd(',');
                BusinessEntities.SaveChanges();
            }
            return Json("");
        }

        #endregion

        #region 岗位岗级

        public JsonResult SaveWorkPost()
        {
            T_EmployeeWorkPost post = UpdateEntity<T_EmployeeWorkPost>();
            var employee = BusinessEntities.Set<T_Employee>().Find(post.EmployeeID);
            if (employee != null)
            {
                employee.Post = post.Post;
                employee.PostLevel = post.PostLevel;
                var wpList = BusinessEntities.Set<T_EmployeeWorkPost>().Where(c => c.EmployeeID == employee.ID && c.ID != post.ID).ToList();
                foreach (T_EmployeeWorkPost item in wpList)
                {
                    employee.Post += "," + item.Post;
                    employee.PostLevel += "," + item.PostLevel;
                }
            }
            BusinessEntities.SaveChanges();
            return Json(new { ID = post.ID });
        }


        public JsonResult DeleteWorkPost()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            BusinessEntities.Set<T_EmployeeWorkPost>().Delete(c => idArray.Contains(c.ID));

            var employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            if (employee != null)
            {
                employee.Post = null;
                employee.PostLevel = null;
                var wpList = BusinessEntities.Set<T_EmployeeWorkPost>().Where(c => c.EmployeeID == employee.ID && !idArray.Contains(c.ID)).ToList();
                foreach (T_EmployeeWorkPost item in wpList)
                {
                    employee.Post += item.Post + ",";
                    employee.PostLevel += item.PostLevel + ",";
                }
                if (!string.IsNullOrEmpty(employee.Post))
                {
                    employee.Post = employee.Post.TrimEnd(',');
                    employee.PostLevel = employee.PostLevel.TrimEnd(',');
                }
            }

            BusinessEntities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 人员调度
        public JsonResult GetJobInformationChangeModel(string id)
        {
            T_EmployeeJobChange model = new T_EmployeeJobChange();
            string employeeID = GetQueryString("EmployeeID");
            if (!string.IsNullOrEmpty(id))
                model = BusinessEntities.Set<T_EmployeeJobChange>().Find(id);
            else if (!string.IsNullOrEmpty(employeeID))
            {
                T_Employee employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
                if (employee == null)
                    throw new BusinessException("找不到员工信息！");

                model.EmployeeCode = employee.Code;
                model.EmployeeID = employee.ID;
                model.EmployeeName = employee.Name;
                string jobList = JsonHelper.ToJson(BusinessEntities.Set<T_EmployeeJob>().Where(c => c.EmployeeID == employeeID && (c.IsDeleted == "0" || c.IsDeleted == null || c.IsDeleted == "")).ToList());
                model.CurrentJobList = jobList;
            }
            return Json(model);
        }

        public JsonResult SaveJobChangeInfo()
        {
            T_EmployeeJobChange changeModel = UpdateEntity<T_EmployeeJobChange>();
            string changeJobList = Request["ChangeJobList"];
            List<T_EmployeeJob> list = UpdateJobInformationList(changeJobList, changeModel.EmployeeID);

            //将调动信息更新状态置空后的数据复制给调动信息
            changeModel.ChangeJobList = JsonHelper.ToJson(list);
            BusinessEntities.SaveChanges();

            return Json(new { ID = changeModel.ID });
        }


        /// <summary>
        /// 根据调动信息修改职务，
        /// 修改操作逻辑：将原职务做逻辑删除，将修改后记录新增为一条新纪录
        /// </summary>
        /// <param name="changeJobList">调动信息</param>
        /// <param name="employeeID">员工ID</param>
        private List<T_EmployeeJob> UpdateJobInformationList(string changeJobList, string employeeID)
        {
            T_Employee employee = BusinessEntities.Set<T_Employee>().Find(employeeID);
            List<T_EmployeeJob> list = new List<T_EmployeeJob>();
            List<Dictionary<string, object>> jobList = JsonHelper.ToObject<List<Dictionary<string, object>>>(changeJobList);
            foreach (var row in jobList)
            {
                String id = row.Keys.Contains("ID") && row["ID"] != null ? row["ID"].ToString() : "";
                String state = row.Keys.Contains("_state") && row["_state"] != null ? row["_state"].ToString() : "";

                T_EmployeeJob entity = new T_EmployeeJob();
                if (state == "added")
                {
                    UpdateEntity<T_EmployeeJob>(entity, row);
                    EntityCreateLogic<T_EmployeeJob>(entity);
                    EntitySaveLogic<T_EmployeeJob>(entity);
                    entity.ID = FormulaHelper.CreateGuid();
                    BusinessEntities.Set<T_EmployeeJob>().Add(entity);
                    entity.EmployeeID = employeeID;
                    entity._state = null;
                    list.Add(entity);
                }
                else if (state == "removed" || state == "deleted")
                {
                    entity = BusinessEntities.Set<T_EmployeeJob>().Find(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = "1";
                    }
                }
                else if (state == "modified")
                {
                    entity = BusinessEntities.Set<T_EmployeeJob>().Find(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = "1";
                    }
                    T_EmployeeJob model = JsonHelper.ToObject<T_EmployeeJob>(JsonHelper.ToJson(row));
                    model.ID = FormulaHelper.CreateGuid();
                    EntityCreateLogic<T_EmployeeJob>(model);
                    EntitySaveLogic<T_EmployeeJob>(model);
                    BusinessEntities.Set<T_EmployeeJob>().Add(model);
                    entity.EmployeeID = employeeID;
                    entity._state = null;
                    list.Add(model);
                }
                else
                    entity = JsonHelper.ToObject<T_EmployeeJob>(JsonHelper.ToJson(row));
            }
            employee.DeptID = "";
            employee.DeptIDName = "";
            employee.DeptName = "";
            foreach (T_EmployeeJob job in list)
            {
                if (job.IsMain == "1")
                {
                    employee.DeptID = job.DeptID;
                    employee.DeptIDName = job.DeptIDName;
                    employee.DeptName = job.DeptIDName;
                    //之前的设置为非主责部门
                    BusinessEntities.Set<T_EmployeeJob>().Where(t => t.EmployeeID == employee.ID && t.ID != job.ID).Update(t => t.IsMain = "0");
                    break;
                }

            }
            return list;
        }

        public JsonResult GetJobInformationChangeList(QueryBuilder qb, string employeeID)
        {
            var data = BusinessEntities.Set<T_EmployeeJobChange>().Where(c => c.EmployeeID == employeeID).WhereToGridData(qb);
            return Json(data);
        }
        #endregion

        #region 同步系统用户

        public JsonResult SyncSystemUser()
        {
            string Ids = Request["IDs"];
            if (string.IsNullOrWhiteSpace(Ids))
                return Json("");
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            EmployeeServiceAuto auto = new EmployeeServiceAuto();
            List<T_Employee> employeeList = BusinessEntities.Set<T_Employee>().Where(c => c.IsHaveAccount == "1" && Ids.Contains(c.ID)).ToList();
            if (employeeList == null || employeeList.Count == 0)
                return Json("");
            foreach (T_Employee employee in employeeList)
            {
                if (string.IsNullOrEmpty(employee.UserID))
                {
                    var user = baseEntities.Set<S_A_User>().FirstOrDefault(c => c.Code == employee.Code && c.IsDeleted == "0");
                    if (user == null)
                        auto.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                    else if (employee.IsDeleted == "0") //Update
                        auto.EmployeeUpdateToUser(employee);
                    else
                        auto.EmployeeDeleteToUser(employee);//Delete
                }
                else
                {
                    var user = baseEntities.Set<S_A_User>().Find(employee.UserID);
                    if (user == null)
                        user = baseEntities.Set<S_A_User>().FirstOrDefault(c => c.Code == employee.Code && c.IsDeleted == "0");
                    if (user == null)
                        auto.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                    else if (employee.IsDeleted == "0") //Update
                        auto.EmployeeUpdateToUser(employee);
                    else
                        auto.EmployeeDeleteToUser(employee);//Delete
                }
            }
            return Json(JsonAjaxResult.Successful());
        }

        public void SyncEmployee(T_Employee employee)
        {
            EmployeeServiceAuto auto = new EmployeeServiceAuto();

            if (string.IsNullOrEmpty(employee.UserID))
            {
                var user = baseEntities.Set<S_A_User>().FirstOrDefault(c => c.Code == employee.Code && c.IsDeleted == "0");
                if (user == null)
                    auto.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                else if (employee.IsDeleted == "0") //Update
                    auto.EmployeeUpdateToUser(employee);
                else
                    auto.EmployeeDeleteToUser(employee);//Delete
            }
            else
            {
                var user = baseEntities.Set<S_A_User>().Find(employee.UserID);
                if (user == null)
                    user = baseEntities.Set<S_A_User>().FirstOrDefault(c => c.Code == employee.Code && c.IsDeleted == "0");
                if (user == null)
                    auto.EmployeeAddToUser(employee, FormsAuthentication.HashPasswordForStoringInConfigFile(employee.Code.ToLower(), "SHA1"));
                else if (employee.IsDeleted == "0") //Update
                    auto.EmployeeUpdateToUser(employee);
                else
                    auto.EmployeeDeleteToUser(employee);//Delete
            }
        }

        #endregion

        #region 离退休管理

        //离退保存
        public JsonResult SaveRetired()
        {
            T_EmployeeRetired model = UpdateEntity<T_EmployeeRetired>();
            var employee = BusinessEntities.Set<T_Employee>().Find(model.EmployeeID);
            if (employee == null)
                throw new BusinessException("找不到该员工！");
            employee.IsDeleted = "1";
            if (model.Type == "退休")
                employee.EmployeeState = EmployeeState.Retire.ToString();
            else if (employee.EmployeeState == EmployeeState.ReEmploy.ToString())
                employee.EmployeeState = EmployeeState.ReEmployDimission.ToString();
            else
                employee.EmployeeState = EmployeeState.Dimission.ToString();
            employee.DeleteTime = DateTime.Now;
            BusinessEntities.Set<S_D_UserAptitude>().Delete(a => a.UserID == employee.UserID);
            BusinessEntities.SaveChanges();
            //同步到系统用户表
            EmployeeServiceAuto auto = new EmployeeServiceAuto();
            auto.EmployeeDeleteToUser(employee);
            return Json(new { ID = model.ID });
        }
        /// <summary>
        /// 撤销离职
        /// </summary>
        /// <returns></returns>
        public JsonResult CancelRetire(string NewCode)
        {

            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            foreach (string id in idArray)
            {
                T_Employee model = BusinessEntities.Set<T_Employee>().Find(id);
                if (model.EmployeeState == EmployeeState.ReEmployDimission.ToString())
                    model.EmployeeState = EmployeeState.ReEmploy.ToString();
                else
                    model.EmployeeState = EmployeeState.Incumbency.ToString();

                model.IsDeleted = "0";
                model.DeleteTime = null;
                if (!string.IsNullOrEmpty(NewCode))
                {
                    if (BusinessEntities.Set<T_Employee>().Any(a => a.ID != model.ID && a.IsDeleted == "0" && a.Code == NewCode))
                        throw new BusinessValidationException("已经存在账号为【" + NewCode + "】的员工，请重新设置");
                    if (!string.IsNullOrEmpty(model.UserID))
                    {
                        BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
                        var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == model.UserID);
                        if (user == null)
                            model.UserID = null;
                        else
                        {
                            if (baseEntities.Set<S_A_User>().Any(a => a.ID != user.ID && a.IsDeleted == "0" && a.Code == NewCode))
                                throw new BusinessValidationException("系统用户表S_A_User，账号【" + NewCode + "】已经存在");
                        }
                    }
                    model.Code = NewCode;
                }
                else
                    NewCode = model.Code;
            }
            BusinessEntities.SaveChanges();
            //恢复系统账号状态
            EmployeeServiceAuto auto = new EmployeeServiceAuto();
            auto.ResetSysUserState(listIDs, NewCode);
            return Json("");
        }

        /// <summary>
        /// 返聘
        /// </summary>
        /// <returns></returns>
        public JsonResult ReEmployee(string NewCode)
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');
            string reEmploy = EmployeeState.ReEmploy.ToString();
            BusinessEntities.Set<T_Employee>().Where(c => idArray.Contains(c.ID)).Update(c =>
            {
                c.IsDeleted = "0";
                c.EmployeeState = reEmploy;
                c.DeleteTime = null;
                if (!string.IsNullOrEmpty(NewCode))
                {
                    if (BusinessEntities.Set<T_Employee>().Any(a => a.ID != c.ID && a.IsDeleted == "0" && a.Code == NewCode))
                        throw new BusinessValidationException("已经存在账号为【" + NewCode + "】的员工，请重新设置");
                    if (!string.IsNullOrEmpty(c.UserID))
                    {
                        BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
                        var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == c.UserID);
                        if (user == null)
                            c.UserID = null;
                        else
                        {
                            if (baseEntities.Set<S_A_User>().Any(a => a.ID != user.ID && a.IsDeleted == "0" && a.Code == NewCode))
                                throw new BusinessValidationException("系统用户表S_A_User，账号【" + NewCode + "】已经存在");
                        }
                    }
                    c.Code = NewCode;
                }
            });
            BusinessEntities.SaveChanges();
            //恢复系统账号状态
            EmployeeServiceAuto auto = new EmployeeServiceAuto();
            auto.ResetSysUserState(listIDs, NewCode);
            return Json("");
        }
        #endregion

        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            List<S_A_Org> orgList = baseEntities.Set<S_A_Org>().Where(a => a.IsDeleted == "0").ToList();
            var sameNameOrgs = GetSameNameDept(orgList);//名字相同的部门获得全路径名称
            var dt = excelData.GetDataTable();
            //验证数据库字段
            var fieldDateList = new List<string>(); //所有日期字段
            var fieldNumList = new List<string>(); //所有数值字段
            var fieldTextList = new List<FieldText>();//所有文本字段及长度
            var formDef = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == "Employee_Base");//表单定义
            #region 根据表单定义 获取日期字段、数值字段、字符字段及长度限制
            var fieldDefItems = JsonHelper.ToObject<List<FormItem>>(formDef.Items);
            foreach (var item in fieldDefItems)
            {
                string fieldCode = item.Code;
                if (!string.IsNullOrEmpty(item.FieldType))
                {
                    if (item.FieldType.ToLower().StartsWith("int") || item.FieldType.ToLower().StartsWith("float") || item.FieldType.ToLower().StartsWith("decimal"))
                        fieldNumList.Add(item.Code);
                    else if (item.FieldType.ToLower() == "datetime")
                        fieldDateList.Add(item.Code);
                    else if (item.FieldType.ToLower().StartsWith("nvarchar"))
                    {
                        var length = item.FieldType.ToLower().Split('(')[1].Split(')')[0];
                        if (length != "max")
                            fieldTextList.Add(new FieldText { Field = fieldCode, MaxLength = Convert.ToInt32(length) });
                    }
                }
            }
            #endregion

            var columnsDt = HRSQLDB.ExecuteDataTable("select top 1 * from T_Employee");
            var errors = excelData.Vaildate(e =>
            {

                #region 字段验证：是否存在、类型、长度

                if (!columnsDt.Columns.Contains(e.FieldName))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("字段【{0}】不在数据库表T_Employee中", e.FieldName);
                }
                var fieldText = fieldTextList.FirstOrDefault(a => a.Field == e.FieldName);
                if (fieldText != null && !string.IsNullOrWhiteSpace(e.Value))
                {
                    if (e.Value.Length > fieldText.MaxLength)
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("文本长度超过限制", e.Value);
                    }
                }
                if (fieldDateList.Contains(e.FieldName))
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        DateTime outValue;
                        if (!DateTime.TryParse(e.Value, out outValue))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("无法转成日期数据", e.Value);
                        }
                    }
                }
                if (fieldNumList.Contains(e.Value))
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        Decimal outValue;
                        if (!Decimal.TryParse(e.Value, out outValue))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("无法转成数值数据", e.Value);
                        }
                    }
                }
                #endregion

                if (e.FieldName == "Code")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("员工编号不能为空", e.Value);
                    }
                    //excel中不能重复
                    if (dt.Select("Code='" + e.Value + "'").Count() > 1)
                    {
                        e.IsValid = false;
                        e.ErrorText = "员工编号【" + e.Value + "】重复";
                    }
                }
                if (e.FieldName == "DeptName")
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var dept = orgList.FirstOrDefault(o => o.Name == e.Value);
                        var sameKey = sameNameOrgs.Keys.FirstOrDefault(a => a.EndsWith(e.Value));
                        if (dept == null && string.IsNullOrEmpty(sameKey))
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
                if (e.FieldName == "ParttimeDeptName")
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        foreach (var item in e.Value.Replace('，', ',').Split(','))
                        {
                            var dept = orgList.FirstOrDefault(o => o.Name == item);
                            var sameKey = sameNameOrgs.Keys.FirstOrDefault(a => a.EndsWith(item));
                            if (dept == null && string.IsNullOrEmpty(sameKey))
                            {
                                e.IsValid = false;
                                e.ErrorText = string.Format("部门（{0}）不存在！", item);
                            }
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
            var currentUser = FormulaHelper.GetUserInfo();
            var orgService = FormulaHelper.GetService<IOrgService>();
            var enumService = FormulaHelper.GetService<IEnumService>();
            var dicList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tempdata["data"]);
            var columnsDt = HRSQLDB.ExecuteDataTable("select top 1 * from T_Employee");
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var baseDBHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //已经存在Hr数据
            var existList = this.BusinessEntities.Set<T_Employee>().Select(a => new { a.ID, a.Code, a.IsDeleted }).ToList();
            var sysUserList = baseEntities.Set<S_A_User>().ToList();
            var userOrgList = baseEntities.Set<S_A__OrgUser>().ToList();
            List<S_A_Org> orgList = baseEntities.Set<S_A_Org>().Where(a => a.IsDeleted == "0").ToList();
            var sameNameOrgs = GetSameNameDept(orgList);//名字相同的部门获得全路径名称
            StringBuilder sb = new StringBuilder();
            foreach (var item in dicList)
            {
                var hrUser = existList.FirstOrDefault(a => a.Code == item.GetValue("Code"));
                if (hrUser != null && hrUser.IsDeleted == "0") continue;
                if (item.GetValue("IsHaveAccount") != "否")
                    item.SetValue("IsHaveAccount", "1");
                item.SetValue("IsDeleted", "0");
                if (columnsDt.Columns.Contains("Sex") && item.ContainsKey("Sex") && !string.IsNullOrEmpty(item.GetValue("Sex")))
                    item.SetValue("Sex", enumService.GetEnumValue("System.Sex", item.GetValue("Sex")));
                item.SetValue("CreateDate", DateTime.Now.ToString());
                item.SetValue("CreateUser", currentUser.UserName);
                item.SetValue("CreateUserID", currentUser.UserID);
                var wayEnum = enumService.GetEnumDataSource("HR.EmploymentWay").ToList();
                if (columnsDt.Columns.Contains("EmploymentWay") && string.IsNullOrEmpty(item.GetValue("EmploymentWay"))
                    && wayEnum.Any(a => a.Text.Contains("正式")))
                    item.SetValue("EmploymentWay", wayEnum.FirstOrDefault(a => a.Text.Contains("正式")).Value);
                #region 部门处理
                S_A_Org dept = null;
                if (item.ContainsKey("DeptName") && !string.IsNullOrEmpty(item.GetValue("DeptName")))
                {
                    //适配部门a.b.c的情况，避免部门名字相同但是其实是不同部门的情况
                    var deptText = item.GetValue("DeptName");//获得所有层级的名称
                    if (deptText.IndexOf('.') >= 0)
                    {
                        var sameKey = sameNameOrgs.Keys.FirstOrDefault(a => a.EndsWith(deptText));
                        if (!string.IsNullOrEmpty(sameKey))
                        {
                            var deptID = sameNameOrgs.GetValue(sameKey);
                            if (!string.IsNullOrEmpty(deptID))
                                dept = orgList.FirstOrDefault(a => a.ID == deptID);
                            //若是重名部门列表没有查到数据，则找部门名称自身含点的部门
                            if (dept == null)
                                dept = orgList.Where(a => a.Name == deptText).OrderBy(a => a.FullID.Length).LastOrDefault();
                        }
                    }
                    else
                        dept = orgList.Where(a => a.Name == deptText).OrderBy(a => a.FullID.Length).LastOrDefault();
                    if (dept != null)
                    {
                        if (columnsDt.Columns.Contains("DeptID"))
                            item.SetValue("DeptID", dept.ID);
                        if (columnsDt.Columns.Contains("DeptIDName"))
                            item.SetValue("DeptIDName", dept.Name);
                        if (columnsDt.Columns.Contains("DeptName"))
                            item.SetValue("DeptName", dept.Name);
                    }
                }

                #endregion
                #region 兼职部门处理
                List<S_A_Org> parttimeDeptList = new List<S_A_Org>();
                if (item.ContainsKey("ParttimeDeptName") && !string.IsNullOrEmpty(item.GetValue("ParttimeDeptName")))
                {
                    //适配部门a.b.c的情况，避免部门名字相同但是其实是不同部门的情况
                    var deptTexts = item.GetValue("ParttimeDeptName");//获得所有层级的名称
                    foreach (var deptText in deptTexts.Replace('，', ',').Split(','))
                    {
                        S_A_Org parttimeDept = null;
                        if (deptText.IndexOf('.') >= 0)
                        {
                            var sameKey = sameNameOrgs.Keys.FirstOrDefault(a => a.EndsWith(deptText));
                            if (!string.IsNullOrEmpty(sameKey))
                            {
                                var deptID = sameNameOrgs.GetValue(sameKey);
                                if (!string.IsNullOrEmpty(deptID))
                                    parttimeDept = orgList.FirstOrDefault(a => a.ID == deptID);
                                //若是重名部门列表没有查到数据，则找部门名称自身含点的部门
                                if (parttimeDept == null)
                                    parttimeDept = orgList.Where(a => a.Name == deptText).OrderBy(a => a.FullID.Length).LastOrDefault();
                            }
                        }
                        else
                            parttimeDept = orgList.Where(a => a.Name == deptText).OrderBy(a => a.FullID.Length).LastOrDefault();
                        if (parttimeDept != null && !parttimeDeptList.Contains(parttimeDept))
                            parttimeDeptList.Add(parttimeDept);
                    }
                    if (parttimeDeptList.Count > 0)
                    {
                        if (columnsDt.Columns.Contains("ParttimeDeptID"))
                            item.SetValue("ParttimeDeptID", string.Join(",", parttimeDeptList.Select(a => a.ID).ToArray()));
                        if (columnsDt.Columns.Contains("ParttimeDeptName"))
                            item.SetValue("ParttimeDeptName", string.Join(",", parttimeDeptList.Select(a => a.Name).ToArray()));
                        if (columnsDt.Columns.Contains("ParttimeDeptIDName"))
                            item.SetValue("ParttimeDeptIDName", string.Join(",", parttimeDeptList.Select(a => a.Name).ToArray()));
                    }
                }
                #endregion
                #region 系统账户逻辑
                if (item.GetValue("IsHaveAccount") != "否")
                {
                    string companyID = "";
                    string companyName = "";
                    if (dept != null)
                    {
                        //逆序判断类型是否为集团/公司
                        string[] orgIDs = dept.FullID.Split('.');
                        var orgs = orgList.Where(c => orgIDs.Contains(c.ID)).ToDictionary<S_A_Org, string>(d => d.ID);

                        for (var i = orgIDs.Length; i > 0; i--)
                        {
                            if ((orgs[orgIDs[i - 1]].Type ?? "none").IndexOf("Company") > -1)
                            {
                                companyID = orgs[orgIDs[i - 1]].ID;
                                companyName = orgs[orgIDs[i - 1]].Name;
                                break;
                            }
                        }
                    }

                    var sysuserInsertSQL = "insert into " + baseDBHelper.DbName + "..S_A_User (ID{1}) values ('{0}'{2}) \n";
                    var sysuserUpdateSQL = "update " + baseDBHelper.DbName + "..S_A_User set {1} where ID = '{0}' \n";
                    var Password = FormsAuthentication.HashPasswordForStoringInConfigFile(item.GetValue("Code").ToLower(), "SHA1");
                    //添加新用户
                    S_A_User systemuser = sysUserList.FirstOrDefault(c => c.Code == item.GetValue("Code"));
                    if (systemuser == null)
                    {
                        systemuser = new S_A_User();
                        systemuser.ID = FormulaHelper.CreateGuid();
                        var fieldsStr = string.Empty;
                        var valuesStr = string.Empty;
                        fieldsStr += ",Code";
                        valuesStr += ",'" + item.GetValue("Code").Replace("'", "''") + "'";
                        fieldsStr += ",WorkNo";
                        valuesStr += ",'" + item.GetValue("Code").Replace("'", "''") + "'";
                        fieldsStr += ",Password";
                        valuesStr += ",'" + Password + "'";
                        fieldsStr += ",IsDeleted";
                        valuesStr += ",'0'";
                        fieldsStr += ",ModifyTime";
                        valuesStr += ",'" + DateTime.Now.ToString() + "'";
                        if (item.ContainsKey("Name"))
                        {
                            fieldsStr += ",Name";
                            valuesStr += ",'" + item.GetValue("Name").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("Address"))
                        {
                            fieldsStr += ",Address";
                            valuesStr += ",'" + item.GetValue("Address").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("Email"))
                        {
                            fieldsStr += ",Email";
                            valuesStr += ",'" + item.GetValue("Email").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("Sex"))
                        {
                            fieldsStr += ",Sex";
                            valuesStr += ",'" + item.GetValue("Sex").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("JoinCompanyDate") && !string.IsNullOrEmpty(item.GetValue("JoinCompanyDate")))
                        {
                            fieldsStr += ",InDate";
                            valuesStr += ",'" + item.GetValue("JoinCompanyDate").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("OfficePhone"))
                        {
                            fieldsStr += ",Phone";
                            valuesStr += ",'" + item.GetValue("OfficePhone").Replace("'", "''") + "'";
                        }
                        if (item.ContainsKey("MobilePhone"))
                        {
                            fieldsStr += ",MobilePhone";
                            valuesStr += ",'" + item.GetValue("MobilePhone").Replace("'", "''") + "'";
                        }
                        if (dept != null)
                        {
                            fieldsStr += ",DeptID";
                            valuesStr += ",'" + dept.ID + "'";
                            fieldsStr += ",DeptName";
                            valuesStr += ",'" + dept.Name.Replace("'", "''") + "'";
                            fieldsStr += ",DeptFullID";
                            valuesStr += ",'" + dept.FullID + "'";

                            fieldsStr += ",CorpID";
                            valuesStr += ",'" + companyID + "'";
                            fieldsStr += ",CorpName";
                            valuesStr += ",'" + companyName + "'";
                        }
                        sysuserInsertSQL = string.Format(sysuserInsertSQL, systemuser.ID, fieldsStr, valuesStr);
                        sb.AppendLine(sysuserInsertSQL);
                    }
                    else
                    {
                        var setStr = string.Empty;
                        setStr += string.Format("{0}='{1}'", "IsDeleted", "0");
                        if (dept != null)
                        {
                            setStr += string.Format(",{0}='{1}'", "DeptID", dept.ID);
                            setStr += string.Format(",{0}='{1}'", "DeptName", dept.Name.Replace("'", "''"));
                            setStr += string.Format(",{0}='{1}'", "DeptFullID", dept.FullID);

                            setStr += string.Format(",{0}='{1}'", "CorpID", companyID);
                            setStr += string.Format(",{0}='{1}'", "CorpName", companyName);
                        }
                        sysuserUpdateSQL = string.Format(sysuserUpdateSQL, systemuser.ID, setStr);
                        sb.AppendLine(sysuserUpdateSQL);
                    }

                    foreach (var orgId in dept.FullID.Split('.'))
                    {
                        S_A__OrgUser orgUser = userOrgList.FirstOrDefault(c => c.OrgID == orgId && c.UserID == systemuser.ID);
                        if (orgUser == null)
                        {
                            orgUser = new S_A__OrgUser();
                            orgUser.OrgID = orgId;
                            orgUser.UserID = systemuser.ID;
                            userOrgList.Add(orgUser);
                            sb.AppendLine("insert into " + baseDBHelper.DbName + "..S_A__OrgUser (OrgID,UserID) values ('" + orgId + "','" + systemuser.ID + "') \n");
                        }
                    }

                    foreach (var parttimeDept in parttimeDeptList)
                    {
                        foreach (var orgId in parttimeDept.FullID.Split('.'))
                        {
                            S_A__OrgUser orgUser = userOrgList.FirstOrDefault(c => c.OrgID == orgId && c.UserID == systemuser.ID);
                            if (orgUser == null)
                            {
                                orgUser = new S_A__OrgUser();
                                orgUser.OrgID = orgId;
                                orgUser.UserID = systemuser.ID;
                                userOrgList.Add(orgUser);
                                sb.AppendLine("insert into " + baseDBHelper.DbName + "..S_A__OrgUser (OrgID,UserID) values ('" + orgId + "','" + systemuser.ID + "') \n");
                            }
                        }
                    }

                    if (columnsDt.Columns.Contains("UserID"))
                        item.SetValue("UserID", systemuser.ID);
                }

                #endregion
                var insertSQL = "insert into T_Employee (ID{1}) values ('{0}'{2}) \n";
                var updateSQL = "update T_Employee set {1} where ID = '{0}' \n";
                #region 拼SQL语句
                //新增逻辑
                if (hrUser == null)
                {
                    var fieldsStr = string.Empty;
                    var valuesStr = string.Empty;
                    foreach (string key in item.Keys)
                    {
                        if (key == "ID")
                            continue;
                        string value = item[key] != null ? item[key].Replace("'", "''") : item[key];
                        if (!string.IsNullOrEmpty(value))
                        {
                            fieldsStr += "," + key;
                            valuesStr += ",'" + value + "'";
                        }
                    }
                    insertSQL = string.Format(insertSQL, FormulaHelper.CreateGuid(), fieldsStr, valuesStr);
                    sb.AppendLine(insertSQL);
                }
                //修改逻辑
                else
                {
                    var setStr = string.Empty;
                    foreach (string key in item.Keys)
                    {
                        if (key == "ID")
                            continue;
                        string value = item[key] != null ? item[key].Replace("'", "''") : item[key];
                        if (!string.IsNullOrEmpty(value))
                        {
                            setStr += string.Format(",{0}='{1}'", key, value);
                        }
                    }
                    updateSQL = string.Format(updateSQL, hrUser.ID, setStr.TrimStart(','));
                    sb.AppendLine(updateSQL);
                }

                #endregion
            }

            if (sb.Length > 0)
            {
                this.HRSQLDB.ExecuteNonQueryWithTrans(sb.ToString());
            }
            return Json("Success");
        }

        public Dictionary<string, string> GetSameNameDept(List<S_A_Org> orgs)
        {
            var result = new Dictionary<string, string>();
            var groups = orgs.GroupBy(a => a.Name).Where(a => a.Count() > 1).Select(a => a.Key).ToList();//所有名称相同的部门
            foreach (var item in orgs.Where(a => groups.Contains(a.Name)).ToList())
            {
                var key = string.Empty; var value = item.ID;
                foreach (var _id in item.FullID.Split('.'))
                {
                    var _item = orgs.FirstOrDefault(a => a.ID == _id);
                    key += _item.Name + ".";
                }
                result.SetValue(key.TrimEnd('.'), value);
            }
            return result;
        }

        #endregion
    }
    class FieldEnum
    {
        public string Field { get; set; }
        public IList<Config.DicItem> EnumItems { get; set; }
    }
    class FieldText
    {
        public string Field { get; set; }
        public int MaxLength { get; set; }
    }
}
