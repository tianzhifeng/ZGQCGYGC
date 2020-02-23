using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config.Logic;
using Newtonsoft.Json;
using Formula.ImportExport;
using Formula;


namespace HR.Areas.AutoForm.Controllers
{
    public class PostLevelTemplateController : HRFormContorllor<S_S_PostLevelTemplate>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                SetDtValue(dt, "BelongYear", DateTime.Now.Year);
                SetDtValue(dt, "BelongMonth", DateTime.Now.Month);
                SetDtValue(dt, "BelongQuarter", (DateTime.Now.Month + 2) / 3);
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new S_S_PostLevelTemplate();
            this.UpdateEntity(entity, dic);
            if (detail != null && detailList != null && detailList.Count > 0)
            {
                var postCode = detail.GetValue("PostCode");
                var postLevelCode = detail.GetValue("PostLevelCode");
                var existedRows = detailList.Where(c => c.GetValue("PostCode") == postCode && c.GetValue("PostLevelCode") == postLevelCode);
                if (existedRows != null && existedRows.Count() > 1)
                    throw new Formula.Exceptions.BusinessException("岗位为【" + detail.GetValue("PostName") + "】且岗级为【" + detail.GetValue("PostLevelName") + "】的记录存在" + existedRows.Count() + "条，请删除【" + (existedRows.Count() - 1) + "】条重复数据。");
            }

            if (entity != null)
                entity.Validate(detail);
        }

        #region Excel导入

        //验证逻辑
        public JsonResult VaildExcelFixedSalary()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "BaseSalary")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "薪资不能为空";
                    }
                    else
                    {
                        float value = 0;
                        bool isFloat = float.TryParse(e.Value, out value);
                        if (isFloat == false)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("必须为数字！", e.Value);
                        }
                    }

                }
                if (e.FieldName == "StartUseDate")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "启用日期不能为空";
                    }
                    else
                    {
                        DateTime dateTime = DateTime.Now;
                        bool isFloat = DateTime.TryParse(e.Value, out dateTime);
                        if (isFloat == false)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("必须为日期！", e.Value);
                        }
                    }
                }
                if (e.FieldName == "PostCode" && string.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "岗位编号不能为空";

                }
                if (e.FieldName == "PostName" && string.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "岗位名称不能为空";

                }
                if (e.FieldName == "PostLevelCode" && string.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "岗级编号不能为空";

                }
                if (e.FieldName == "PostLevelName" && string.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "岗级名称不能为空";

                }
            });
            return Json(errors);
        }

        //导入逻辑
        public JsonResult BatchSaveFixedSalary()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_S_PostLevelTemplate_PostList>>(tempdata["data"]);

            this.BusinessEntities.Configuration.AutoDetectChangesEnabled = false;
            var user = FormulaHelper.GetUserInfo();

            var tempID = FormulaHelper.CreateGuid();
            DateTime strteUserDate = DateTime.Now;
            var templateCode = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0')
                + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
            var tempLateName = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日" + DateTime.Now.Hour + "时" + DateTime.Now.Minute + "分" + DateTime.Now.Second + "秒"
                + "导入的模板";
            var i = 1;
            foreach (var item in list)
            {
                var entity = this.BusinessEntities.Set<S_S_PostLevelTemplate_PostList>().Create();
                entity.ID = FormulaHelper.CreateGuid();
                entity.S_S_PostLevelTemplateID = tempID;
                entity.SortIndex = i++;
                entity.PostCode = item.PostCode;
                entity.PostLevelCode = item.PostLevelCode;
                strteUserDate = (DateTime)item.StartUseDate;
                entity.StartUseDate = item.StartUseDate;
                entity.BelongYear = entity.StartUseDate.Value.Year.ToString();
                entity.BelongMonth = entity.StartUseDate.Value.Month.ToString();
                entity.BelongQuarter = ((entity.StartUseDate.Value.Month + 2) / 3).ToString();
                entity.PostName = item.PostName;
                entity.PostLevelName = item.PostLevelName;
                entity.BaseSalary = item.BaseSalary;
                entity.ReMark = item.ReMark;
                this.BusinessEntities.Set<S_S_PostLevelTemplate_PostList>().Add(entity);
            }
            //添加模板数据
            var tempEntity = this.BusinessEntities.Set<S_S_PostLevelTemplate>().Create();
            tempEntity.ID = tempID;
            tempEntity.TemplateCode = templateCode;
            tempEntity.TemplateName = tempLateName;
            tempEntity.StartUseDate = strteUserDate;
            tempEntity.BelongYear = tempEntity.StartUseDate.Value.Year.ToString();
            tempEntity.BelongMonth = tempEntity.StartUseDate.Value.Month.ToString();
            tempEntity.BelongQuarter = ((tempEntity.StartUseDate.Value.Month + 2) / 3).ToString();
            tempEntity.CreateDate = DateTime.Now;
            tempEntity.CreateUserID = user.UserID;
            tempEntity.CreateUser = user.UserName;
            this.BusinessEntities.Set<S_S_PostLevelTemplate>().Add(tempEntity);
            this.BusinessEntities.SaveChanges();
            return Json("Success");
        }
        #endregion
    }
}
