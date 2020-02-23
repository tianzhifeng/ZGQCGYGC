using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Formula.ImportExport;
using Project.Logic;
using Project.Logic.Domain;
using Config;

using Newtonsoft.Json;
using Formula.Exceptions;
using Config.Logic;
using Base.Logic.Domain;

namespace Project.Areas.BaseData.Controllers
{
    public class PackageController : BaseConfigController<S_D_PackageDic>
    {
        #region Excel 批量导入
        //验证
        public JsonResult VaildExcelDataPackage()
        {
            #region 不为空

            Dictionary<string, string> notemptyfields = new Dictionary<string, string>();
            notemptyfields.Add("Code", "工作包编号");
            notemptyfields.Add("Name", "工作包名称");
            notemptyfields.Add("PackageType", "工作包类型");
            notemptyfields.Add("MajorName", "专业");
            notemptyfields.Add("PhaseName", "阶段");
            notemptyfields.Add("ProjectClass", "项目类型");
            #endregion

            #region 枚举
            var enumLists = GetExcelEnum();
            #endregion

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var orgs = entities.Set<S_D_PackageDic>().ToList();
            var errors = excelData.Vaildate(e =>
            {
                //不为空验证
                if (notemptyfields.Keys.Contains(e.FieldName) && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("{0}不能为空！", string.Join(",", notemptyfields.Where(c => c.Key == e.FieldName).Select(c => c.Value)));
                }

                //枚举验证
                var excelEmunField = enumLists.Where(c => c.ExcelFiled == e.FieldName).FirstOrDefault();
                if (excelEmunField != null && !string.IsNullOrWhiteSpace(e.Value) && excelEmunField.EnumSource.FirstOrDefault(c => c.Text == e.Value) == null)
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("{0}与指定的枚举不符！", string.Join(",", excelEmunField.ExcelFiledName));
                }
            });

            return Json(errors);
        }

        //保存
        public JsonResult BatchSavePackage()
        {
            #region 枚举
            var enumLists = GetExcelEnum();
            #endregion

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_D_PackageDic>>(tempdata["data"]);

            entities.Configuration.AutoDetectChangesEnabled = false;
            var oldpackages = entities.Set<S_D_PackageDic>().ToList();
            foreach (var package in list)
            {
                var newPackage = oldpackages.FirstOrDefault(op => op.Code == package.Code);
                if (newPackage == null)
                {
                    newPackage = new S_D_PackageDic();
                    newPackage.ID = FormulaHelper.CreateGuid();
                    entities.Set<S_D_PackageDic>().Add(newPackage);
                }
                newPackage.Code = package.Code;
                newPackage.Name = package.Name;
                newPackage.PackageType = GetEnumValue(enumLists, "PackageType", package.PackageType);

                newPackage.MajorName = package.MajorName;
                newPackage.MajorCode = GetEnumValue(enumLists, "MajorName", package.MajorName);
                newPackage.PhaseName = package.PhaseName;
                newPackage.PhaseCode = GetEnumValue(enumLists, "PhaseName", package.PhaseName);
                newPackage.AuditLevel = GetEnumValue(enumLists, "AuditLevel", package.AuditLevel);
                newPackage.ProjectClass = GetEnumValue(enumLists, "ProjectClass", package.ProjectClass);

                newPackage.WorkLoad = package.WorkLoad;
                newPackage.DrawingCount = package.DrawingCount;
            }
            try
            {
                entities.SaveChanges();
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                Formula.LogWriter.Error(ex);
                throw ex;
            }
            entities.Configuration.AutoDetectChangesEnabled = true;
            return Json("Success");
        }

        #region 获取excel枚举相关
        //枚举
        private List<ExcelEmunFields> GetExcelEnum()
        {
            var enumfields = new List<ExcelEmunFields>();
            var es = FormulaHelper.GetService<IEnumService>();
            var Major = Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major)
                .AsEnumerable()
                .Select(c => new Config.DicItem
                {
                    Text = c["text"].ToString(),
                    Value = c["value"].ToString()
                }).ToList();

            var Phase = Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Phase)
                .AsEnumerable()
                .Select(c => new Config.DicItem
                {
                    Text = c["text"].ToString(),
                    Value = c["value"].ToString()
                }).ToList();

            var PackageType = es.GetEnumDataSource("Project.PackageType");
            var ProjectClass = es.GetEnumDataSource("Project.ProjectClass");
            var AuditLevel = es.GetEnumDataSource("Project.AuditLevel");

            enumfields.Add(new ExcelEmunFields()
            {
                EnumSource = Major,
                ExcelFiled = "MajorName",
                ExcelFiledName = "专业",
                ValueFiled = "MajorCode",
            });
            enumfields.Add(new ExcelEmunFields()
            {
                EnumSource = Phase,
                ExcelFiled = "PhaseName",
                ExcelFiledName = "阶段",
                ValueFiled = "PhaseCode",
            });
            enumfields.Add(new ExcelEmunFields()
            {
                EnumSource = ProjectClass,
                ExcelFiled = "ProjectClass",
                ExcelFiledName = "项目类型",
                ValueFiled = "ProjectClass",
            });
            enumfields.Add(new ExcelEmunFields()
            {
                EnumSource = PackageType,
                ExcelFiled = "PackageType",
                ExcelFiledName = "工作包类型",
                ValueFiled = "PackageType",
            });
            enumfields.Add(new ExcelEmunFields()
            {
                EnumSource = AuditLevel,
                ExcelFiled = "AuditLevel",
                ExcelFiledName = "校审级别",
                ValueFiled = "AuditLevel",
            });

            return enumfields;
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="enumLists">枚举定义</param>
        /// <param name="ExcelFiled">枚举值字段</param>
        /// <param name="text">枚举显示值</param>
        /// <returns></returns>
        public string GetEnumValue(List<ExcelEmunFields> enumLists, string ExcelFiled, string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            var enumItems = enumLists.First(c => c.ExcelFiled == ExcelFiled).EnumSource;
            return enumItems.FirstOrDefault(c => c.Text == text).Value;
        }
        #endregion

        #endregion

        #region 表单自定义 Excel导入

        public JsonResult VaildExcelData()
        {
            #region 不为空

            Dictionary<string, string> notemptyfields = new Dictionary<string, string>();
            notemptyfields.Add("Code", "卷册编号");
            notemptyfields.Add("Name", "卷册名称");
            notemptyfields.Add("PackageType", "卷册类型");
            notemptyfields.Add("MajorCode", "专业");
            notemptyfields.Add("PhaseCode", "阶段");
            notemptyfields.Add("ProjectClass", "项目类型");
            //notemptyfields.Add("WorkLoad", "额定工时");

            Dictionary<string, string> numberfields = new Dictionary<string, string>();
            var RoleDefine = entities.Set<S_D_RoleDefine>().ToList();
            RoleDefine.ForEach(a => numberfields.Add(a.RoleCode, a.RoleName + "比例"));
            #endregion

            #region 枚举
            var enumLists = GetExcelEnums();
            #endregion

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var dt = excelData.GetDataTable();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Code")
                {
                    //excel中不能重复
                    if (dt.Select("Code='" + e.Value + "'").Count() > 1)
                    {
                        e.IsValid = false;
                        e.ErrorText = "编号【" + e.Value + "】重复";
                    }
                }

                //不为空验证
                if (notemptyfields.Keys.Contains(e.FieldName) && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("{0}不能为空！", string.Join(",", notemptyfields.Where(c => c.Key == e.FieldName).Select(c => c.Value)));
                }

                //枚举验证
                var excelEmunField = enumLists.Where(c => c.ExcelFiled == e.FieldName).FirstOrDefault();
                if (excelEmunField != null && !string.IsNullOrWhiteSpace(e.Value))
                {
                    var thisValue = e.Value.Replace("，", ",");
                    foreach (var item in thisValue.Split(','))
                    {
                        if (excelEmunField.EnumSource.FirstOrDefault(c => c.Text == item) == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("{0}与指定的枚举不符！\n只能从以下枚举中选择：\n{1}", string.Join(",", excelEmunField.ExcelFiledName), String.Join("、", excelEmunField.EnumSource.Select(a => a.Text).ToList()));
                        }
                    }
                }

                if (numberfields.Keys.Contains(e.FieldName) && !string.IsNullOrWhiteSpace(e.Value))
                {
                    float rlt;
                    if (!float.TryParse(e.Value, out rlt))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("{0}必须填入数值！", string.Join(",", numberfields.Where(c => c.Key == e.FieldName).Select(c => c.Value)));
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult BatchSave()
        {
            #region 枚举
            var enumLists = GetExcelEnums();
            #endregion

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);

            var oldPackages = entities.Set<S_D_PackageDic>().ToList();
            var sql = "";
            foreach (var packageDic in list)
            {
                var fields = packageDic.Keys;
                var code = packageDic.GetValue("Code");
                var package = oldPackages.FirstOrDefault(a => a.Code == code);
                if (package == null)
                {
                    package = new S_D_PackageDic();
                    package.ID = FormulaHelper.CreateGuid();
                    sql += "insert into S_D_PackageDic (ID) values ('" + package.ID + "')\n";
                }
                var index = 0;
                foreach (var field in fields)
                {
                    if (!field.StartsWith("Rate"))
                    {
                        var updateSql = "update S_D_PackageDic set {0} = '{1}' where ID = '{2}' \n";
                        if (field != "WorkLoad")
                            if (enumLists.FirstOrDefault(a => a.ExcelFiled == field) == null)
                                sql += String.Format(updateSql, field, packageDic.GetValue(field), package.ID);
                            else
                                sql += String.Format(updateSql, field, GetEnumsValue(enumLists, field, packageDic.GetValue(field)), package.ID);
                        else
                        {
                            if (packageDic.GetValue(field) != "")
                                package.WorkLoad = Math.Round(decimal.Parse(packageDic.GetValue(field)), 2);
                            else
                                package.WorkLoad = 0;
                            sql += String.Format(updateSql, field, package.WorkLoad, package.ID);
                        }
                    }
                    else
                    {
                        if (packageDic.GetValue(field) != "")
                        {
                            var subTableField = field.Replace("Rate", "");
                            var packageDetail = package.S_D_PackageDic_RoleRate.FirstOrDefault(a => a.Role == subTableField);
                            if (packageDetail == null)
                            {
                                packageDetail = new S_D_PackageDic_RoleRate();
                                packageDetail.ID = FormulaHelper.CreateGuid();
                                sql += "insert into S_D_PackageDic_RoleRate (ID,S_D_PackageDicID,SortIndex) values ('" + packageDetail.ID + "','" + package.ID + "','" + (index++) + "')\n";
                            }
                            var updateSubSql = "update S_D_PackageDic_RoleRate set Role = '{0}',Rate = '{1}',WorkLoad = '{2}' where ID = '{3}' \n";
                            packageDetail.Rate = Math.Round(decimal.Parse(packageDic.GetValue(field)), 2);
                            packageDetail.WorkLoad = Math.Round((decimal)packageDetail.Rate * (decimal)package.WorkLoad / 100, 2);
                            sql += String.Format(updateSubSql, subTableField, packageDetail.Rate, packageDetail.WorkLoad, packageDetail.ID);
                        }
                    }
                }
            }
            try
            {
                SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteNonQuery(sql);
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                Formula.LogWriter.Error(ex);
                throw ex;
            }
            return Json("Success");
        }

        #region 获取excel枚举相关
        //枚举
        private List<ExcelEmunFields> GetExcelEnums()
        {
            var enumfields = new List<ExcelEmunFields>();
            var es = FormulaHelper.GetService<IEnumService>();
            var Items = JsonHelper.ToList(FormulaHelper.GetEntities<BaseEntities>().Set<S_UI_Form>().FirstOrDefault(a => a.Code == "PackageDic").Items);

            foreach (var item in Items)
            {
                if (item.GetValue("ItemType") == "ComboBox")
                {
                    var setting = JsonHelper.ToObject(item.GetValue("Settings"));
                    var data = setting.GetValue("data");
                    if (!data.StartsWith("[{"))
                    {
                        var enumItem = es.GetEnumDataSource(data);
                        enumfields.Add(new ExcelEmunFields()
                        {
                            EnumSource = enumItem,
                            ExcelFiled = item.GetValue("Code"),
                            ExcelFiledName = item.GetValue("Name"),
                            ValueFiled = item.GetValue("Code")
                        });
                    }
                }
            }

            return enumfields;
        }

        public string GetEnumsValue(List<ExcelEmunFields> enumLists, string ExcelFiled, string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            var txt = text.Replace('，', ',').Split(',');
            var enumItems = enumLists.First(c => c.ExcelFiled == ExcelFiled).EnumSource;
            return string.Join(",", enumItems.Where(c => txt.Contains(c.Text)).Select(c => c.Value));
        }

        #endregion

        #endregion

        public override ActionResult Edit()
        {
            ViewBag.Major = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            ViewBag.Phase = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Phase));
            return View();
        }

        public override ActionResult List()
        {
            var tab = new Tab();
            var majorCategory = CategoryFactory.GetCategory("Project.Major", "专业", "MajorCode");
            tab.Categories.Add(majorCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override JsonResult Save()
        {
            var pack = base.UpdateEntity<S_D_PackageDic>();

            if (entities.Set<S_D_PackageDic>().Where(c => c.Code == pack.Code && c.ID != pack.ID).Count() > 0)
                throw new BusinessException("编号不能重复！");

            return base.JsonSave<S_D_PackageDic>(pack);
        }
    }

    public class ExcelEmunFields
    {
        public string ExcelFiled { get; set; }
        public string ExcelFiledName { get; set; }
        public IList<Config.DicItem> EnumSource { get; set; }
        public string ValueFiled { get; set; }
    }
}
