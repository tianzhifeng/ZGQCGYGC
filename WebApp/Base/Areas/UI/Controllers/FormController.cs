using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using Formula.Exceptions;
using MvcAdapter;
using Base.Logic.Domain;
using Base.Logic.Model.UI.Form;
using Base.Logic.BusinessFacade;
using Base.Logic;
using System.ComponentModel;

namespace Base.Areas.UI.Controllers
{
    public class FormController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetFormList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "select ID,Code,Name,ConnName,TableName,ModifyTime,CategoryID,VersionCount=0,IsUEditor,CompanyName from S_UI_Form where VersionEndDate is null";

            //表单定义增加子公司权限
            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"].ToLower() == "true"
                && Request["listType"] == "subCompany")
                sql += string.Format(" and CompanyID = '{0}'", FormulaHelper.GetUserInfo().AdminCompanyID);

            var dt = sqlHelper.ExecuteDataTable(sql, qb);
            sql = string.Format("select Code,VersionCount=COUNT(1) from S_UI_Form where Code in('{0}') group by Code ", string.Join("','", dt.AsEnumerable().Select(c => c["Code"].ToString())));
            var dtVersionCount = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                row["VersionCount"] = dtVersionCount.AsEnumerable().SingleOrDefault(c => c["Code"].ToString() == row["Code"].ToString())["VersionCount"];
            }
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        #endregion

        #region 基本信息

        public ActionResult LayoutEdit()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult LayoutEnUEditor()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult ItemList()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult Edit()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }

            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetModel(string id)
        {
            var entity = GetEntity<S_UI_Form>(id);
            entity.ScriptText = Server.HtmlDecode(entity.ScriptText);
            entity.MobileScriptText = Server.HtmlDecode(entity.MobileScriptText);
            return Json(entity);
        }
        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Form>();
            if (!(entity.VersionNum > 1))
                if (entities.Set<S_UI_Form>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                    throw new Exception(string.Format("表单编号重复，表单名称“{0}”，表单编号：“{1}”", entity.Name, entity.Code));

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            var user = FormulaHelper.GetUserInfo();
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.Items = "[]";
                entity.DefaultValueSettings = "[]";
                entity.FlowLogic = "[]";
                entity.Setttings = "{}";
                entity.SerialNumberSettings = "{\"Tmpl\":\"{YY}{MM}{DD}-{NNNN}\",\"ResetRule\":\"YearCode,MonthCode\"}";
                entity.CompanyID = user.AdminCompanyID;
                entity.CompanyName = user.AdminCompanyName;
            }

            entity.ModifyTime = DateTime.Now;
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            if (!string.IsNullOrEmpty(Request["MobileScript"]))
                entity.MobileScriptText = Request["MobileScript"];
            entity.ScriptText = Request["Script"];
            entity.WebPrintJS = Request["WebPrintJS"];

            if (entity.VersionStartDate == null)
            {
                entity.VersionStartDate = entity.CreateTime;
                entity.VersionNum = 1;
            }

            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Delete(string listIDs)
        {
            var lists = entities.Set<S_UI_Form>().Where(c => listIDs.Contains(c.ID));
            var codeList = lists.Select(c => c.Code).ToArray();
            entities.Set<S_UI_Form>().Delete(c => codeList.Contains(c.Code));
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 布局信息

        public JsonResult GetLayout(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(new { layout = entity.Layout ?? "" });
        }

        [ValidateInput(false)]
        public JsonResult SaveLayout(string ID, string layout)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            entity.Layout = layout;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult CreateLayout(string ID, string mode)
        {
            int layoutMode = int.Parse(mode.Split(',')[0]);
            string decorative = mode.Split(',')[1]; //fieldset装饰或table装饰

            #region 装饰

            string strDecorativeStart = "";
            string strDecorativeEnd = "";
            string strDecorativeTable = "";

            #region fieldset
            if (decorative == "fieldset")
            {
                strDecorativeStart = @"
<fieldset class='formDiv'>
    <legend>{0}</legend>
";
                strDecorativeEnd = @"
</fieldset>
";
                strDecorativeTable = "style='width:100%;'cellpadding='2' border='0'";
            }
            #endregion
            #region table
            else if (decorative == "table")
            {
                strDecorativeStart = @"
<table class='ke-zeroborder' style='width:90%;margin: 0px auto;table-layout:fixed' cellspacing='0' cellpadding='2' border='0'>
	<tbody>
		<tr>
			<td style='text-align:left;width:30%;'>
                版本/修改码：D/1
			</td>
			<td style='text-align:center;'>
                QM-P-03-02
			</td>
            <td style='text-align:right;width:30%;'>
                编号：{流水号}
			</td>
		</tr>
	</tbody>
</table>
";
                strDecorativeEnd = "";
                strDecorativeTable = "style='width:90%;margin: 0px auto;table-layout:fixed'cellpadding='2' border='1'";
            }
            #endregion
            #region tablenew
            else if (decorative == "tablenew")
            {
                strDecorativeTable = "style='width:90%;margin: 0 auto;table-layout:fixed' cellpadding='2' border='0'";
            }
            #endregion
            #endregion

            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            var listItems = JsonHelper.ToObject<List<FormItem>>(form.Items);
            StringBuilder sb = new StringBuilder();
            if (decorative == "fieldset")
            {
                sb.AppendFormat("<h1 align='center'>{0}</h1>", form.Name);
            }
            else
            {
                sb.AppendFormat("<h1 align='center'>{0}</h1> {1}", form.Name, strDecorativeStart);
            }
            foreach (var group in listItems.Select(c => c.Group ?? "").Distinct().ToList())
            {
                var list = listItems.Where(c => c.Group == group).ToList();
                //补齐
                while (list.Count % layoutMode != 0)
                {
                    list.Add(new FormItem() { Code = "", Name = "" });
                }
                //开始内容
                if (decorative == "fieldset")
                {
                    sb.AppendFormat(strDecorativeStart, group);
                    sb.AppendFormat(@"
    <table class='ke-zeroborder' {0}>", strDecorativeTable);
                }
                else
                {
                    sb.AppendFormat(@"
    <table class='ke-zeroborder' {0}> 
<caption align='top' style='text-align:left; height:22px; line-height:22px; margin: 20px -50px; font-weight: bold; font-size: 15px; padding: 0px; border-left: 5px solid #345a80;color: #375b81;position: relative;'>
<h4 style='margin: 0px; padding: 0px 20px; background-color: #fff;min-width: 60px;display: inline-block;z-index: 99;position: absolute;'>
{1}
</h4><em style='height: 0px; border-bottom: #ccc solid 1px;position: absolute;top: 12px;width: 100%;z-index: 98;'></em>
</caption>", strDecorativeTable, group);
                }
                string labelStyle = "style='width:15%; padding-left:20px; text-align:left;'";
                string inputStyle = "style='width: 35%'";
                if (layoutMode == 3)
                {
                    labelStyle = "style='width:13%; padding-left:20px; text-align:left;'";
                    inputStyle = "style='width: 20%'";
                }


                bool firstRow = true;
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (i < layoutMode)
                    {
                        firstRow = true;
                    }
                    else
                    {
                        firstRow = false;
                    }
                    if (i % layoutMode == 0)
                    {
                        //布局的首列
                        sb.Append("<tr>");
                        if (!firstRow)
                        {
                            labelStyle = "style='text-align:left;'";
                            inputStyle = "";
                        }
                        else
                        {
                            //布局的首行 
                            labelStyle = "style='width:13%;text-align:left;'";
                            inputStyle = "style='width: 20%'";
                        }
                    }
                    else
                    {
                        //非首行，非首列
                        if (!firstRow)
                        {
                            labelStyle = "style='padding-left:20px; text-align:left;'";
                            inputStyle = "";
                        }
                        else
                        {
                            //非首列，是首行
                            labelStyle = "style='width:13%;padding-left:20px; text-align:left;'";
                            inputStyle = "style='width: 20%'";
                        }
                    }
                    sb.AppendFormat("<td {2}>{0}</td><td {3}>{1}</td>",
                        item.Name,
                        string.IsNullOrEmpty(item.Name) ? "" : "{" + item.Name + "}",
                        labelStyle,
                        inputStyle);
                    if (i % layoutMode == layoutMode - 1)
                        sb.Append("</tr>");
                }
                sb.Append(@"
    </table>
");
                //结束内容
                sb.Append(strDecorativeEnd);
                //结束装饰
            }
            return Json(new { layout = sb.ToString() });
        }

        #endregion

        #region 英文布局

        public JsonResult GetLayoutEn(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            string layout = entity.LayoutEN;
            if (string.IsNullOrEmpty(layout))
                layout = entity.Layout;
            return Json(new { layout = layout });
        }

        [ValidateInput(false)]
        public JsonResult SaveLayoutEn(string ID, string layout)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            entity.LayoutEN = layout;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult CreateLayoutEn(string ID, string mode)
        {
            int layoutMode = int.Parse(mode.Split(',')[0]);
            string decorative = mode.Split(',')[1]; //fieldset装饰或table装饰

            #region 装饰

            string strDecorativeStart = "";
            string strDecorativeEnd = "";
            string strDecorativeTable = "";
            #region fieldset
            if (decorative == "fieldset")
            {
                strDecorativeStart = @"
<fieldset class='formDiv'>
    <legend>{0}</legend>
";
                strDecorativeEnd = @"
</fieldset>
";
                strDecorativeTable = "style='width:100%;'cellpadding='2' border='0'";
            }
            #endregion
            #region table
            else if (decorative == "table")
            {
                strDecorativeStart = @"
<table class='ke-zeroborder' style='width:100%;table-layout:fixed' cellspacing='0' cellpadding='2' border='0'>
	<tbody>
		<tr>
			<td style='text-align:left;width:30%;'>		
                版本/修改码：D/1		
			</td>
			<td style='text-align:center;'>		
                QM-P-03-02
			</td>
            <td style='text-align:right;width:30%;'>
                编号：{{流水号}}
			</td>
		</tr>
	</tbody>
</table>
";
                strDecorativeEnd = "";
                strDecorativeTable = "style='width:100%;table-layout:fixed'cellpadding='2' border='1'";
            }
            #endregion
            #region tablenew
            else if (decorative == "tablenew")
            {
                strDecorativeTable = "style='width:90%;margin: 0 auto;table-layout:fixed' cellpadding='2' border='0'";
            }
            #endregion
            #endregion

            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            var listItems = JsonHelper.ToObject<List<FormItem>>(form.Items);
            StringBuilder sb = new StringBuilder();
            if (decorative == "fieldset")
            {
                sb.AppendFormat("<h1 align='center'>{0}</h1>", form.Name);
            }
            else
            {
                sb.AppendFormat("<h1 align='center'>{0}</h1> {1}", form.Name, strDecorativeStart);
            }
            foreach (var group in listItems.Select(c => c.Group ?? "").Distinct().ToList())
            {
                var list = listItems.Where(c => c.Group == group).ToList();
                //补齐
                while (list.Count % layoutMode != 0)
                {
                    list.Add(new FormItem() { Code = "", Name = "" });
                }
                //开始内容
                if (decorative == "fieldset")
                {
                    sb.AppendFormat(strDecorativeStart, group);
                    sb.AppendFormat(@"
    <table class='ke-zeroborder' {0}>", strDecorativeTable);
                }
                else
                {
                    sb.AppendFormat(@"
    <table class='ke-zeroborder' {0}> 
<caption align='top' style='text-align:left; height:22px; line-height:22px; margin: 20px -50px; font-weight: bold; font-size: 15px; padding: 0px; border-left: 5px solid #345a80;color: #375b81;position: relative;'>
<h4 style='margin: 0px; padding: 0px 20px; background-color: #fff;min-width: 60px;display: inline-block;z-index: 99;position: absolute;'>
{1}
</h4><em style='height: 0px; border-bottom: #ccc solid 1px;position: absolute;top: 12px;width: 100%;z-index: 98;'></em>
</caption>", strDecorativeTable, group);
                }
                string labelStyle = "style='width:15%; padding-left:20px; text-align:left;'";
                string inputStyle = "style='width: 35%'";
                if (layoutMode == 3)
                {
                    labelStyle = "style='width:13%; padding-left:20px; text-align:left;'";
                    inputStyle = "style='width: 20%'";
                }

                bool firstRow = true;
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (i < layoutMode)
                    {
                        firstRow = true;
                    }
                    else
                    {
                        firstRow = false;
                    }
                    if (i % layoutMode == 0)
                    {
                        //布局的首列
                        sb.Append("<tr>");
                        if (!firstRow)
                        {
                            labelStyle = "style='text-align:left;'";
                            inputStyle = "";
                        }
                        else
                        {
                            //布局的首行 
                            labelStyle = "style='width:13%;text-align:left;'";
                            inputStyle = "style='width: 20%'";
                        }
                    }
                    else
                    {
                        //非首行，非首列
                        if (!firstRow)
                        {
                            labelStyle = "style='padding-left:20px; text-align:left;'";
                            inputStyle = "";
                        }
                        else
                        {
                            //非首列，是首行
                            labelStyle = "style='width:13%;padding-left:20px; text-align:left;'";
                            inputStyle = "style='width: 20%'";
                        }
                    }

                    sb.AppendFormat("<td {2}>{0}</td><td {3}>{1}</td>", item.Name, string.IsNullOrEmpty(item.Name) ? "" : "{" + item.Name + "}", labelStyle, inputStyle);

                    if (i % layoutMode == layoutMode - 1)
                        sb.Append("</tr>");
                }

                sb.Append(@"
    </table>
");
                //结束内容
                sb.Append(strDecorativeEnd);
                //结束装饰
            }
            return Json(new { layout = sb.ToString() });
        }

        #endregion

        #region 控件信息

        public JsonResult GetItemList(string formID)
        {
            return Json(entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID).Items);
        }

        public JsonResult GetCalItemList(string formID)
        {
            return Json(entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID).CalItems);
        }

        [ValidateInput(false)]
        public JsonResult SaveItemList()
        {
            bool hasEncryptField = false;//是否有加密字段

            var uiFO = FormulaHelper.CreateFO<UIFO>();

            var list = JsonHelper.ToObject<List<FormItem>>(Request["ItemList"]);
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Name))
                    continue;

                if (item.FieldType == "varbinary(500)")
                    hasEncryptField = true;

                item.Name = item.Name.Trim();
                if (item.Name.Contains('\\') || item.Name.Contains('/'))
                    throw new Exception(string.Format("名称中不能包含特殊字符\\或/,出错的字段名：{0}", item.Name));
                if (item.Name == "流水号")
                    item.Code = "SerialNumber";
                else if (string.IsNullOrEmpty(item.Code))
                    item.Code = uiFO.GetHanZiPinYinString(item.Name);
                item.Code = item.Code.Trim();
                #region 子表字段

                if (item.ItemType == "SubTable" && !string.IsNullOrEmpty(item.Settings))
                {
                    var itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item.Settings);
                    var itemSubList = JsonHelper.ToObject<List<FormItem>>(itemDic["listData"].ToString());
                    foreach (var subItem in itemSubList)
                    {
                        if (string.IsNullOrEmpty(subItem.Name))
                            continue;

                        subItem.Name = subItem.Name.Trim();
                        if (subItem.Name.Contains('\\') || subItem.Name.Contains('/'))
                            throw new Exception(string.Format("名称中不能包含特殊字符\\或/,出错的字段名：{0}", subItem.Name));
                        if (string.IsNullOrEmpty(subItem.Code))
                            subItem.Code = uiFO.GetHanZiPinYinString(subItem.Name);
                        subItem.Code = subItem.Code.Trim();

                        if (subItem.FieldType == "varbinary(500)")
                            hasEncryptField = true;
                    }

                    #region 验证编号名称不能重复

                    var subFieldNameList = itemSubList.Select(c => c.Name).Distinct().ToList();
                    if (subFieldNameList.Count() < itemSubList.Count)
                    {
                        foreach (var c in itemSubList)
                        {
                            if (!subFieldNameList.Contains(c.Name))
                                throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Name));
                            else subFieldNameList.Remove(c.Name);
                        }
                    }

                    var subFieldCodeList = itemSubList.Select(c => c.Code).Distinct().ToList();
                    if (subFieldCodeList.Count() < itemSubList.Count)
                    {
                        foreach (var c in itemSubList)
                        {
                            if (!subFieldCodeList.Contains(c.Code))
                                throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Code));
                            else subFieldCodeList.Remove(c.Code);
                        }
                    }

                    #endregion

                    //修改完成赋值回去
                    itemDic["listData"] = itemSubList;
                    item.Settings = JsonHelper.ToJson(itemDic);
                }

                #endregion
            }

            #region 校验编号名称不能重复

            var fieldNameList = list.Select(c => c.Name).Distinct().ToList();
            if (fieldNameList.Count() < list.Count)
            {
                foreach (var c in list)
                {
                    if (!fieldNameList.Contains(c.Name))
                        throw new Exception(string.Format("控件名称不能重复：“{0}”", c.Name));
                    else fieldNameList.Remove(c.Name);
                }
            }

            var fieldCodeList = list.Select(c => c.Code).Distinct().ToList();
            if (fieldCodeList.Count() < list.Count)
            {
                foreach (var c in list)
                {
                    if (!fieldCodeList.Contains(c.Code))
                        throw new Exception(string.Format("控件编号不能重复：“{0}”", c.Code));
                    else fieldCodeList.Remove(c.Code);
                }
            }

            #endregion

            //Oracle的字段名大写
            if (Config.Constant.IsOracleDb)
            {
                foreach (var item in list)
                {
                    item.Code = item.Code.ToUpper();
                }
            }


            string formID = Request["FormID"];
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            form.Items = JsonHelper.ToJson(list);

            var user = FormulaHelper.GetUserInfo();
            form.ModifyUserID = user.UserID;
            form.ModifyUserName = user.UserName;
            form.ModifyTime = DateTime.Now;
            entities.SaveChanges();

            if (hasEncryptField)
            {
                var _sql = @"
IF EXISTS (SELECT * FROM sys.symmetric_keys WHERE name='SymmetricByAsy')
DROP SYMMETRIC KEY SymmetricByAsy
IF EXISTS (SELECT * FROM sys.symmetric_keys WHERE name='SymmetricBySy')
DROP SYMMETRIC KEY SymmetricBySy
IF EXISTS (SELECT * FROM sys.symmetric_keys WHERE name='JinHuiSymmetric')
DROP SYMMETRIC KEY JinHuiSymmetric
IF EXISTS (SELECT * FROM sys.symmetric_keys WHERE name='SymmetricByCert')
DROP SYMMETRIC KEY SymmetricByCert
IF EXISTS (SELECT * FROM sys.asymmetric_keys WHERE name='JinHuiAsymmetric')
DROP ASYMMETRIC KEY JinHuiAsymmetric
IF EXISTS (SELECT * FROM sys.certificates WHERE name='JinHuiCertificate')
DROP CERTIFICATE JinHuiCertificate 
--select name,is_master_key_encrypted_by_server from sys.databases
IF EXISTS (SELECT * FROM sys.databases 
WHERE name='master' AND is_master_key_encrypted_by_server='1')
DROP MASTER KEY 

--创建数据库主密钥
IF EXISTS (SELECT is_master_key_encrypted_by_server FROM sys.databases 
WHERE name='master' AND is_master_key_encrypted_by_server='1')
CREATE MASTER KEY ENCRYPTION BY PASSWORD ='JinHui2019'
--创建证书
CREATE CERTIFICATE JinHuiCertificate 
with SUBJECT = 'JinHuiCertificate2019';

--创建非对称密钥
CREATE ASYMMETRIC KEY JinHuiAsymmetric
    WITH ALGORITHM = RSA_2048 
    ENCRYPTION BY PASSWORD = 'JinHui2019'; 

--创建对称密钥
CREATE SYMMETRIC KEY JinHuiSymmetric
    WITH ALGORITHM = AES_256
    ENCRYPTION BY PASSWORD = 'JinHui2019';


--第二步
--由证书加密对称密钥
CREATE SYMMETRIC KEY SymmetricByCert
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE JinHuiCertificate;


--由对称密钥加密对称密钥
OPEN SYMMETRIC KEY JinHuiSymmetric
    DECRYPTION BY PASSWORD='JinHui2019';

CREATE SYMMETRIC KEY SymmetricBySy
    WITH ALGORITHM = AES_256
    ENCRYPTION BY SYMMETRIC KEY JinHuiSymmetric;


--由非对称密钥加密对称密钥
CREATE SYMMETRIC KEY SymmetricByAsy
    WITH ALGORITHM = AES_256
    ENCRYPTION BY ASYMMETRIC KEY JinHuiASymmetric;
";
                SQLHelper officeSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
                try
                {
                    officeSqlHelper.ExecuteNonQuery(_sql);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Json("");
        }

        [ValidateInput(false)]
        public JsonResult SaveCalItemList()
        {
            string formID = Request["FormID"];
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (form == null) throw new Formula.Exceptions.BusinessValidationException("没有找到表单定义对象，无法保存计算信息");
            form.CalItems = Request["ItemList"];
            var user = FormulaHelper.GetUserInfo();
            form.ModifyUserID = user.UserID;
            form.ModifyUserName = user.UserName;
            form.ModifyTime = DateTime.Now;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetRealExpression(string Exp)
        {
            var calFo = FormulaHelper.CreateFO<Base.Logic.CaculateFo>();
            var result = calFo.ResolveExpressionToExpression(Exp);
            var list = calFo.GetInputParamListFromExpression(result, false);
            var refList = calFo.GetRefParamDataFields(result);
            return Json(new { resExp = result, ListData = list, RefColumns = refList });
        }

        public JsonResult GetParamTree(string DefineID)
        {
            var result = new List<Dictionary<string, object>>();
            var enums = EnumBaseHelper.GetEnumDef(typeof(ArgumentType));
            foreach (var item in enums.EnumItem)
            {
                if (item.Code == ArgumentType.DynCalArg.ToString())
                    continue;
                else if (item.Code == ArgumentType.CalArg.ToString())
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("Name", "其他计算参数");
                    node.SetValue("Code", item.Code);
                    node.SetValue("ID", item.Code);
                    node.SetValue("NodeType", "ParamType");
                    node.SetValue("ParamType", item.Code);
                    node.SetValue("ParentID", "Root");
                    result.Add(node);
                }
                else
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("Name", item.Name);
                    node.SetValue("Code", item.Code);
                    node.SetValue("ID", item.Code);
                    node.SetValue("NodeType", "ParamType");
                    node.SetValue("ParamType", item.Code);
                    node.SetValue("ParentID", "Root");
                    result.Add(node);
                }

                if (item.Code == ArgumentType.InputArg.ToString())
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("Name", "空白输入参数");
                    node.SetValue("Code", "Input_");
                    node.SetValue("ID", "Input_");
                    node.SetValue("NodeType", "Paramter");
                    node.SetValue("ParamType", ArgumentType.InputArg.ToString());
                    node.SetValue("ParentID", item.Code);
                    result.Add(node);
                }
            }

            var formNode = new Dictionary<string, object>();
            formNode.SetValue("Name", "表单输入");
            formNode.SetValue("Code", "Form");
            formNode.SetValue("NodeType", "Form");
            formNode.SetValue("ParamType", "Form");
            formNode.SetValue("ID", "Form");
            formNode.SetValue("ParentID", "");
            result.Add(formNode);

            var fieldEnum = GetFormFieldsEnum(DefineID);
            foreach (var field in fieldEnum)
            {
                var fieldNode = new Dictionary<string, object>();
                fieldNode.SetValue("Name", field.GetValue("text"));
                fieldNode.SetValue("Code", "Field_" + field.GetValue("value").Trim('{', '}'));
                fieldNode.SetValue("NodeType", "FormField");
                fieldNode.SetValue("ParamType", "Paramter");
                fieldNode.SetValue("ID", field.GetValue("value"));
                fieldNode.SetValue("ParentID", "Form");
                result.Add(fieldNode);
            }

            var rootNode = new Dictionary<string, object>();
            rootNode.SetValue("Name", "公用参数");
            rootNode.SetValue("Code", "Root");
            rootNode.SetValue("NodeType", "Root");
            rootNode.SetValue("ParamType", "Root");
            rootNode.SetValue("ID", "Root");
            rootNode.SetValue("ParentID", "");
            result.Add(rootNode);

            var funcNode = new Dictionary<string, object>();
            funcNode.SetValue("Name", "系统函数");
            funcNode.SetValue("Code", "SysFunc");
            funcNode.SetValue("NodeType", "SysFunc");
            funcNode.SetValue("ParamType", "Root");
            funcNode.SetValue("ID", "SysFuncRoot");
            funcNode.SetValue("ParentID", "");
            result.Add(funcNode);

            var emType = typeof(SystemFunction);
            foreach (var item in emType.GetFields())
            {
                if (item.FieldType.IsEnum)
                {
                    object[] arr = item.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    var text = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : item.Name;
                    var value = item.Name;
                    var funcSubNode = new Dictionary<string, object>();
                    funcSubNode.SetValue("Name", text);
                    funcSubNode.SetValue("Code", value);
                    funcSubNode.SetValue("NodeType", "Func");
                    funcSubNode.SetValue("ParamType", "Func");
                    funcSubNode.SetValue("ID", value);
                    funcSubNode.SetValue("ParentID", "SysFuncRoot");
                    result.Add(funcSubNode);
                }
            }

            string sql = @"select ID,Name+'['+Code+']' as Name,Code,NodeType,FullID,ParentID,ParamType,[Expression],SortIndex from S_M_Parameter
where  (ParamType in ('" + ArgumentType.RefArg.ToString() + "','"
                     + ArgumentType.InputArg.ToString() + "','" + ArgumentType.CalArg + "') and (ParentID ='' or ParentID is null)) ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = db.ExecuteDataTable(sql);
            foreach (DataRow parameter in dt.Rows)
            {
                var param = FormulaHelper.DataRowToDic(parameter);
                if (String.IsNullOrEmpty(param.GetValue("ParentID")))
                    param.SetValue("ParentID", param.GetValue("ParamType"));
                result.Add(param);
            }

            return Json(result);
        }

        List<Dictionary<string, object>> GetFormFieldsEnum(string formDefineID)
        {
            var define = this.entities.Set<S_UI_Form>().Find(formDefineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的额表单定义，请确认表单定义ID是否正确");
            var fieldsEnum = new List<Dictionary<string, object>>();
            var idEnum = new Dictionary<string, object>();
            idEnum.SetValue("text", "主键(ID)");
            idEnum.SetValue("value", "{ID}");
            idEnum.SetValue("ItemType", "PK");
            fieldsEnum.Add(idEnum);
            if (!String.IsNullOrEmpty(define.Items))
            {
                var items = JsonHelper.ToObject<List<FormItem>>(define.Items);  //JsonHelper.ToList(define.Items);
                foreach (var field in items)
                {
                    if (field.ItemType == "SubTable")
                    {
                        var fieldEnum = new Dictionary<string, object>();
                        fieldEnum.SetValue("text", field.Name + "(" + field.Code + ")");
                        fieldEnum.SetValue("value", "{" + field.Code + "}");
                        fieldEnum.SetValue("ItemType", field.ItemType);
                        fieldsEnum.Add(fieldEnum);

                        if (String.IsNullOrEmpty(field.Settings)) continue;
                        var itemSettings = JsonHelper.ToObject(field.Settings);
                        if (String.IsNullOrEmpty(itemSettings.GetValue("listData"))) continue;
                        var subItems = JsonHelper.ToList(itemSettings.GetValue("listData"));
                        foreach (var item in subItems)
                        {
                            fieldEnum = new Dictionary<string, object>();
                            fieldEnum.SetValue("text", field.Name + "." + "【" + item.GetValue("Name") + "(" + item.GetValue("Code") + ")】");
                            fieldEnum.SetValue("value", "{" + field.Code + "." + item.GetValue("Code") + "}");
                            fieldEnum.SetValue("ItemType", item.GetValue("ItemType"));
                            fieldsEnum.Add(fieldEnum);
                        }
                    }
                    else
                    {
                        var fieldEnum = new Dictionary<string, object>();
                        fieldEnum.SetValue("text", field.Name + "(" + field.Code + ")");
                        fieldEnum.SetValue("value", "{" + field.Code + "}");
                        fieldEnum.SetValue("ItemType", field.ItemType);
                        fieldsEnum.Add(fieldEnum);
                    }
                }
            }
            return fieldsEnum;
        }

        public ActionResult CalItemList()
        {
            var defineID = this.GetQueryString("FormID");
            var define = this.entities.Set<S_UI_Form>().Find(defineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的额表单定义，请确认表单定义ID是否正确");

            #region 表单环境变量枚举
            var fieldsEnum = this.GetFormFieldsEnum(defineID);
            var userEnum = new Dictionary<string, object>();
            userEnum.SetValue("text", "当前人ID(CurrentUserID)");
            userEnum.SetValue("value", "{CurrentUserID}");
            fieldsEnum.Add(userEnum);

            var orgEnum = new Dictionary<string, object>();
            orgEnum.SetValue("text", "当前人部门ID(CurrentUserOrgID)");
            orgEnum.SetValue("value", "{CurrentUserOrgID}");
            fieldsEnum.Add(orgEnum);

            var createTimeEnum = new Dictionary<string, object>();
            createTimeEnum.SetValue("text", "当前日期(CurrentDate)");
            createTimeEnum.SetValue("value", "{CurrentDate}");
            fieldsEnum.Add(createTimeEnum);

            ViewBag.FormFields = JsonHelper.ToJson(fieldsEnum);
            #endregion

            #region 表单数据源枚举
            var dataSourceFields = new List<Dictionary<string, object>>();
            var dsSource = Formula.Helper.JsonHelper.ToList(define.DefaultValueSettings);
            foreach (var dsItem in dsSource)
            {
                var firstCode = dsItem.GetValue("Code");
                string sql = dsItem.GetValue("SQL");
                var connName = dsItem.GetValue("ConnName");
                if (String.IsNullOrEmpty(sql) || String.IsNullOrEmpty(connName)) continue;
                var db = SQLHelper.CreateSqlHelper(connName);
                var dt = db.ExecuteDataTable(String.Format("SELECT * FROM ({0}) TABLEINFO WHERE 1<>1", sql));
                foreach (DataColumn col in dt.Columns)
                {
                    var fieldData = new Dictionary<string, object>();
                    fieldData.SetValue("value", firstCode + "." + col.ColumnName);
                    fieldData.SetValue("text", firstCode + "." + col.ColumnName);
                    dataSourceFields.Add(fieldData);
                }
            }
            ViewBag.DataSourceFields = JsonHelper.ToJson(dataSourceFields);
            #endregion

            return View();
        }

        public ActionResult SettingsButtonEdit()
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(0, new { value = "SystemOrg", text = "选择部门" });
            list.Insert(0, new { value = "SystemUser", text = "选择用户" });
            ViewBag.SelectorEnum = JsonHelper.ToJson(list);
            return View();
        }

        public ActionResult SettingsLinkEdit()
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(0, new { value = "SystemOrg", text = "选择部门" });
            list.Insert(0, new { value = "SystemUser", text = "选择用户" });
            ViewBag.SelectorEnum = JsonHelper.ToJson(list);
            return View();
        }

        private string GetFieldType(string name)
        {
            if (name.Contains("日期") || name.Contains("时间"))
                return "datetime.DatePicker";
            else if (name.Contains("附件"))
                return "nvarchar(max).MultiFile";
            else if (name.Contains("意见") || name.Contains("签字"))
                return "nvarchar(500).AuditSign";
            else if (name.Contains("元") || name.Contains("金额"))
                return "decimal(18 2).TextBox";
            else if (name.Contains("数量") || name.Contains("个"))
                return "int.TextBox";
            else
                return "nvarchar(500).TextBox";
        }

        #region 从布局导入

        public JsonResult ImportItemFromLayout(string formID)
        {
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (string.IsNullOrEmpty(form.Layout))
                throw new Exception("布局还没有建立");
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            Regex reg = new Regex(UIFO.uiRegStr);
            var matchs = reg.Matches(form.Layout, 0);

            var list = JsonHelper.ToObject<List<FormItem>>(form.Items ?? "[]");

            int index = list.Count;
            foreach (Match match in matchs)
            {
                string name = match.Value.Trim('{', '}');
                if (list.SingleOrDefault(c => c.Name == name) == null)
                    list.Add(new FormItem
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Code = uiFO.GetHanZiPinYinString(name),
                        Name = name,
                        Enabled = "true",
                        Visible = "true",
                        DefaultValue = "",
                        ItemType = GetFieldType(name).Split('.')[1],
                        FieldType = GetFieldType(name).Split('.')[0]
                    });
            }
            return Json(list);
        }

        #endregion

        #region 从数据库表导入

        public JsonResult ImportItemFormDB(string formID)
        {
            var formDef = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formDef.ConnName);

            string sql = string.Format(@"SELECT  Code= a.name , Name= isnull(g.[value],''),FieldType1=b.name,sortIndex=a.column_id
,Enabled='true',Visible='true',[Group]='基本信息'
FROM  sys.columns a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = a.column_id)
left join systypes b on a.user_type_id=b.xusertype  
WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name = '{0}')", formDef.TableName);


            var dt = sqlHelper.ExecuteDataTable(sql);




            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var row = dt.Rows[i];
                if ("ID,CreateUserID,CreateUserName,CreateTime,ModifyUserID,ModifyUserName,ModifyTime,CreateDate,ModifyDate,CreateUser,ModifyUser,OrgID,FlowPhase,StepName".Split(',').Contains(row["Code"].ToString()))
                    dt.Rows.Remove(row);
            }

            var arr = dt.AsEnumerable().Select(c => c["Code"].ToString()).ToArray();

            foreach (var fieldCode in arr)
            {
                if (arr.Contains(fieldCode + "Name"))
                {
                    var row = dt.AsEnumerable().SingleOrDefault(c => c["Code"].ToString() == fieldCode + "Name");
                    dt.Rows.Remove(row);
                }
            }

            dt.Columns.Add("ID");
            List<FormItem> list = new List<FormItem>();
            foreach (DataRow row in dt.Rows)
            {
                row["ID"] = FormulaHelper.CreateGuid();
                list.Add(new FormItem
                {
                    ID = Convert.ToString(row["ID"]),
                    Code = Convert.ToString(row["Code"]),
                    Name = string.IsNullOrEmpty(Convert.ToString(row["Name"])) ? Convert.ToString(row["Code"]) : Convert.ToString(row["Name"]),
                    Enabled = Convert.ToString(row["Enabled"]),
                    Visible = Convert.ToString(row["Visible"]),
                    Group = Convert.ToString(row["Group"]),
                    DefaultValue = "",
                    ItemType = GetFieldType(Convert.ToString(row["Name"])).Split('.')[1],
                    FieldType = GetFieldType(Convert.ToString(row["Name"])).Split('.')[0]
                });
            }

            return Json(list);
        }

        #endregion

        #endregion

        #region 默认值

        public JsonResult getDefaultValueSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.DefaultValueSettings ?? "");
        }

        public JsonResult SaveDefaultValueSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.DefaultValueSettings = Request["DefaultValueSettings"];
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 流水号

        public JsonResult GetSerialNumberSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.SerialNumberSettings ?? "");
        }

        public JsonResult SaveSerialNumberSettings(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.SerialNumberSettings = Request["FormData"];
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            UpdateList<S_UI_SerialNumber>();
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetSerialNumberList(MvcAdapter.QueryBuilder qb)
        {
            string formID = Request["FormID"];
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_SerialNumber where Code='{0}'", entity.Code);
            var dt = sqlHelper.ExecuteDataTable(sql, qb);
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetMaxSerialNumber()
        {
            string formID = Request["FormID"];
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (entity != null)
            {
                var serialNumberDic = JsonHelper.ToObject(entity.SerialNumberSettings);
                string tmpl = serialNumberDic["Tmpl"].ToString();

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
                string sql = @"select 1 from syscolumns a,sysobjects b where a.id=b.id
                        and ltrim(a.name)='SerialNumber' and ltrim(b.name)='{0}'";
                var obj = Convert.ToInt32(sqlHelper.ExecuteScalar(string.Format(sql, entity.TableName)));
                if (obj > 0)
                {
                    Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
                    string result = reg.Replace(tmpl, (Match m) =>
                    {
                        string number = "";
                        try
                        {
                            if (m.Value.IndexOf('{') >= 0)
                            {
                                string value = m.Value.Trim('{', '}');
                                if (value.ToUpper().IndexOf('N') >= 0)
                                {
                                    var length = value.Length;
                                    sql = @"select SerialNumber from {0}";
                                    var table = sqlHelper.ExecuteDataTable(string.Format(sql, entity.TableName));
                                    var str = tmpl.IndexOf(m.Value) > 0 ? tmpl.Substring(tmpl.IndexOf(m.Value) - 1, 1) : ""; //取前面一个字符
                                    if (!string.IsNullOrEmpty(str) && str.IndexOf('{') < 0)
                                    {
                                        List<string> serialNumberList = new List<string>();
                                        string subStr = tmpl.Substring(0, tmpl.IndexOf(m.Value));
                                        int l1 = tmpl.Length - tmpl.Replace(str, "").Length;
                                        int count = subStr.Length - subStr.Replace(str, "").Length;
                                        foreach (DataRow dr in table.Rows)
                                        {
                                            string serialNumber = dr["SerialNumber"].ToString();
                                            var arr = serialNumber.ToArray();
                                            int curIndex = 0;
                                            int l2 = serialNumber.Length - serialNumber.Replace(str, "").Length;
                                            for (int i = 0; i < arr.Length; i++)
                                            {
                                                if (arr[i].ToString() == str)
                                                {
                                                    curIndex = curIndex + 1;
                                                    if (curIndex == count && l1 == l2)
                                                    {
                                                        serialNumberList.Add(serialNumber.Substring(i + 1, length));
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        number = serialNumberList.Max();
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        return number;
                    });
                    return Json(Math.Abs(Convert.ToInt32(result)));
                }
            }
            return Json(0);
        }

        public JsonResult UpdateSerialNumber(string number)
        {
            string formID = Request["FormID"];
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (entity != null)
            {
                var formCode = entity.Code;
                UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
                var serialNumberDic = JsonHelper.ToObject(entity.SerialNumberSettings);
                string tmpl = serialNumberDic["Tmpl"].ToString();
                string resetRule = serialNumberDic["ResetRule"].ToString();
                string CategoryCode = "";
                string SubCategoryCode = "";
                string OrderNumCode = "";
                string PrjCode = "";
                string OrgCode = "";
                string UserCode = "";

                if (serialNumberDic.ContainsKey("CategoryCode"))
                    CategoryCode = uiFO.ReplaceString(serialNumberDic["CategoryCode"].ToString());
                if (serialNumberDic.ContainsKey("SubCategoryCode"))
                    SubCategoryCode = uiFO.ReplaceString(serialNumberDic["SubCategoryCode"].ToString());
                if (serialNumberDic.ContainsKey("OrderNumCode"))
                    OrderNumCode = uiFO.ReplaceString(serialNumberDic["OrderNumCode"].ToString());
                if (serialNumberDic.ContainsKey("PrjCode"))
                    PrjCode = uiFO.ReplaceString(serialNumberDic["PrjCode"].ToString());
                if (serialNumberDic.ContainsKey("OrgCode"))
                    OrgCode = uiFO.ReplaceString(serialNumberDic["OrgCode"].ToString());
                if (serialNumberDic.ContainsKey("UserCode"))
                    UserCode = uiFO.ReplaceString(serialNumberDic["UserCode"].ToString());

                SerialNumberParam param = new SerialNumberParam()
                {
                    Code = formCode,
                    PrjCode = PrjCode,
                    OrgCode = OrgCode,
                    UserCode = UserCode,
                    CategoryCode = CategoryCode,
                    SubCategoryCode = SubCategoryCode,
                    OrderNumCode = OrderNumCode
                };

                if (!string.IsNullOrEmpty(number))
                    SerialNumberHelper.UpdateSerialNumber(param, resetRule, Convert.ToInt32(number));
            }
            return Json("");
        }

        #endregion

        #region 流程逻辑

        public JsonResult GetFlowLoigcList(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(entity.FlowLogic ?? "[]");
        }
        public JsonResult SaveFlowLoigcList(string formID, string listData)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            entity.FlowLogic = listData;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }



        #endregion

        #region 克隆

        public JsonResult Clone(string formID)
        {
            var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            var newFormInfo = new S_UI_Form();
            FormulaHelper.UpdateModel(newFormInfo, formInfo);

            #region 修改ID、字段ID和子表字段ID
            newFormInfo.ID = FormulaHelper.CreateGuid();
            var items = JsonHelper.ToObject<List<FormItem>>(newFormInfo.Items);
            foreach (var item in items)
            {
                item.ID = FormulaHelper.CreateGuid();
                if (item.ItemType == "SubTable")
                {
                    if (string.IsNullOrEmpty(item.Settings)) continue;
                    var itemDic = JsonHelper.ToObject(item.Settings);
                    var subItems = JsonHelper.ToObject<List<FormItem>>(itemDic["listData"].ToString());
                    foreach (var subItem in subItems)
                    {
                        subItem.ID = FormulaHelper.CreateGuid();
                    }
                    //修改完成赋值回去
                    itemDic["listData"] = subItems;
                    item.Settings = JsonHelper.ToJson(itemDic);
                }
            }
            //修改完成赋值回去
            newFormInfo.Items = JsonHelper.ToJson(items);
            #endregion

            newFormInfo.Code += "copy";
            newFormInfo.TableName += "copy";
            newFormInfo.Name += "(副本)";
            newFormInfo.ModifyTime = null;
            newFormInfo.ModifyUserID = "";
            newFormInfo.ModifyUserName = "";
            newFormInfo.ReleaseTime = null;
            newFormInfo.ReleasedData = null;
            entities.Set<S_UI_Form>().Add(newFormInfo);
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 发布

        public JsonResult ReleaseForm(string id)
        {
            UIFO uiFO = new UIFO();
            uiFO.ReleaseForm(id);
            return Json("");
        }

        #endregion

        #region 移动端

        public JsonResult GetMobileItems(string formID)
        {
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            if (!string.IsNullOrEmpty(form.MobileItems))
                return Json(form.MobileItems);
            else
                return Json(new List<Dictionary<string, string>>());
        }

        public JsonResult ImportMobileItems(string formID)
        {
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);

            var mobileList = JsonHelper.ToObject<List<Dictionary<string, string>>>(string.IsNullOrEmpty(form.MobileItems) ? "[]" : form.MobileItems);
            var itemList = JsonHelper.ToObject<List<Dictionary<string, string>>>(string.IsNullOrEmpty(form.Items) ? "[]" : form.Items);
            foreach (var item in itemList)
            {
                if (mobileList.Count(c => c["Key"] == item["Code"]) > 0)
                    continue;
                var dic = new Dictionary<string, string>();
                dic.Add("KeyName", item["Name"]);
                dic.Add("Key", item["Code"]);
                dic.Add("IsEdit", "false");
                dic.Add("NotNull", "false");
                dic.Add("ValueType", "text");
                mobileList.Add(dic);
            }
            return Json(mobileList);
        }

        public JsonResult SaveMobileItems()
        {
            string formID = Request["FormID"];
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            form.MobileItems = JsonHelper.ToJson(Request["ItemList"]);

            var user = FormulaHelper.GetUserInfo();
            form.ModifyUserID = user.UserID;
            form.ModifyUserName = user.UserName;
            form.ModifyTime = DateTime.Now;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 创建新版本

        public ActionResult VersionEdit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetFormInfo(string formID)
        {
            var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(formInfo);
        }

        public JsonResult SaveVersion(string formID, string formData)
        {
            string newFormID = FormulaHelper.CreateGuid();

            var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);

            var newFormInfo = new S_UI_Form();
            FormulaHelper.UpdateModel(newFormInfo, formInfo);

            newFormInfo.ID = newFormID;
            newFormInfo.CreateTime = DateTime.Now;
            newFormInfo.ModifyTime = DateTime.Now;
            newFormInfo.ReleaseTime = null;
            newFormInfo.ReleasedData = null;


            newFormInfo.VersionNum = entities.Set<S_UI_Form>().Count(c => c.Code == formInfo.Code) + 1;
            //if (formInfo.VersionNum != null)
            //    newFormInfo.VersionNum = formInfo.VersionNum + 1;
            //else
            //    newFormInfo.VersionNum = entities.Set<S_UI_Form>().Count(c => c.Code == formInfo.Code) + 1;

            var dic = JsonHelper.ToObject(formData);

            if (dic["VersionStartDate"].ToString() != "")
            {
                newFormInfo.VersionStartDate = DateTime.Parse(dic["VersionStartDate"].ToString());
                formInfo.VersionEndDate = newFormInfo.VersionStartDate;
            }
            else
                throw new BusinessException("必须输入版本开始时间！");
            newFormInfo.VersionDesc = dic["VersionDesc"].ToString();

            entities.Set<S_UI_Form>().Add(newFormInfo);

            entities.SaveChanges();
            return Json(newFormInfo);
        }

        public JsonResult GetFormVersionList(string code, QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = sqlHelper.ExecuteGridData(string.Format("select ID,Code,Name,ConnName,TableName,ModifyTime,Category,VersionNum,VersionStartDate,VersionEndDate from S_UI_Form where Code='{0}'", code), qb);
            return Json(data);
        }

        public JsonResult DeleteVersion(string code)
        {
            var formInfoList = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderBy(c => c.VersionStartDate).ToList();
            if (formInfoList.Count == 1)
                throw new BusinessException("最后一个不能删除！");
            var formInfo = formInfoList.Last();
            entities.Set<S_UI_Form>().Remove(formInfo);
            formInfoList.Remove(formInfo);
            var newFormInfo = formInfoList.Last();
            newFormInfo.VersionEndDate = null;
            entities.SaveChanges();
            return Json("");

            //var formInfo = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == id);
            //entities.Set<S_UI_Form>().Remove(formInfo);
            //if (formInfo.VersionEndDate == null)
            //{
            //    var newFormInfo = entities.Set<S_UI_Form>().Where(c => c.Code == formInfo.Code).OrderByDescending(c => c.VersionStartDate).FirstOrDefault();
            //    if (newFormInfo != null)
            //        newFormInfo.VersionStartDate = null;
            //}
            //entities.SaveChanges();
            //return Json("");
        }

        #endregion

        #region 表单打印

        public JsonResult GetLayoutPrint(string formID)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == formID);
            return Json(new { layout = string.IsNullOrWhiteSpace(entity.LayoutPrint) ? entity.Layout : entity.LayoutPrint });
        }

        [ValidateInput(false)]
        public JsonResult SaveLayoutPrint(string ID, string layout)
        {
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == ID);
            entity.LayoutPrint = layout;
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region  授权到子公司

        public JsonResult SetCompanyAuth(string objIds, string orgIds, string orgNames)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "update S_UI_Form set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')";
            sql = string.Format(sql, orgIds, orgNames, objIds.Replace(",", "','"));
            sql += "SELECT @@ROWCOUNT AS UPDATECOUNT";
            DataTable ReslutDt = sqlHelper.ExecuteDataTable(sql);
            if (ReslutDt.Rows[0][0].ToString() == "0")
            {
                return Json(new { Result = "N" });//更新失败
            }
            else
            {
                return Json(new { Result = "Y" });//更新成功
            }
        }

        #endregion
    }
}
