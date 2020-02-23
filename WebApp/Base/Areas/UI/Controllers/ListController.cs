using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using Base.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Base.Logic.BusinessFacade;
using Formula;
using Formula.Exceptions;
using Config.Logic;

namespace Base.Areas.UI.Controllers
{
    public class ListController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            //列表定义增加子公司权限
            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"].ToLower() == "true"
                 && Request["listType"] == "subCompany")
                qb.Add("CompanyID", QueryMethod.Equal, FormulaHelper.GetUserInfo().AdminCompanyID);

            var list = entities.Set<S_UI_List>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, SQL = c.SQL, ModifyTime = c.ModifyTime, CompanyName = c.CompanyName });
            GridData data = new GridData(list);
            data.total = qb.TotolCount;

            return Json(data);
        }

        #endregion

        #region 基本信息

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
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetModel(string id)
        {
            var entity = GetEntity<S_UI_List>(id);
            entity.ScriptText = Server.HtmlDecode(entity.ScriptText);
            var dic = Formula.FormulaHelper.ModelToDic<S_UI_List>(entity);
            if (String.IsNullOrEmpty(entity.Settings))
            {
                dic.SetValue("ShowQueryForm", "F");
            }
            else
            {
                var settings = JsonHelper.ToObject(entity.Settings);
                var showQueryForm = settings.GetValue("ShowQueryForm");
                if (String.IsNullOrEmpty(showQueryForm))
                    dic.SetValue("ShowQueryForm", "F");
                else
                {
                    dic.SetValue("ShowQueryForm", showQueryForm);
                    dic.SetValue("QueryFormColmuns", settings.GetValue("QueryFormColmuns"));
                }
            }
            return Json(dic);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            string json = Request.Form["FormData"];
            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(json);

            var entity = UpdateEntity<S_UI_List>();
            if (entities.Set<S_UI_List>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("列表编号重复，表单名称“{0}”，表单编号：“{1}”", entity.Name, entity.Code));
            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            var user = FormulaHelper.GetUserInfo();
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.LayoutButton = @"[
{'id':'btnAdd', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}','iconcls':'icon-add','text':'增加','TextEN':'Add','Enabled':'true','Visible':'true','Settings':'{PopupTitle:\'增加\'}'},
{'id':'btnModify', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&ID={ID}','iconcls':'icon-edit','text':'编辑','TextEN':'Edit','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'编辑\'}'},
{'id':'btnDelete', 'iconcls':'icon-remove','text':'删除','TextEN':'Delete','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'del();\',SelectMode:\'mustSelectRow\',Confirm:\'true\'}'},
{'id':'btnView','URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&ID={ID}&FuncType=View','iconcls':'icon-search','text':'查看','TextEN':'View','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'查看\'}'},
{'id':'btnStart','URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&FlowCode={TmplCode}','iconcls':'icon-add','text':'启动流程','TextEN':'StartFlow','Enabled':'true','Visible':'true','Settings':'{PopupTitle:\'启动\'}'},
{'id':'btnView', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&FlowCode={TmplCode}&ID={ID}','iconcls':'icon-search','text':'查看流程表单','TextEN':'ViewFlow','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'查看\'}'},
{'id':'btnSelect','iconcls':'icon-refer','text':'选择','TextEN':'Select','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'returnList();\',SelectMode:\'mustSelectRow\',Confirm:\'false\'}'},
{'id':'btnTrace','URL':'/MvcConfig/Workflow/Trace/Diagram?ID={ID}&FuncType=FlowTrace','iconcls':'icon-flowstart','text':'流程跟踪','TextEN':'Trace','Enabled':'true','Visible':'true','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'流程跟踪\'}'},
{'id':'btnExportExcel','iconcls':'icon-excel','text':'导出Excel','TextEN':'Excel','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'exportList();\'}'},
{'id':'btnExportWord','iconcls':'icon-word','text':'导出Word','TextEN':'Word','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'exportWord();\'}'},
{'id':'btnImportExcel','iconcls':'icon-excel','text':'导入','TextEN':'Import','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'Import();\'}'},
{'id':'btnExportQRCode','iconcls':'icon-pdf','text':'导出二维码','TextEN':'QRCode','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'ExportQRCode();\'}'},
{'id':'btnPrintForm','iconcls':'icon-print','text':'Word打印','TextEN':'PrintForm','Enabled':'true','Visible':'false','Settings':'{\'onclick\':\'funPrintWord();\'}'},
{'id':'btnRowAdd','iconcls':'icon-add','text':'新增行','TextEN':'RowAdd','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'addEditRow();\'}'},
{'id':'btnRowDelete','iconcls':'icon-remove','text':'删除行','TextEN':'RowDelete','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'deleteEditRow();\'}'},
{'id':'btnRowMoveUp','iconcls':'icon-up','text':'上移','TextEN':'RowMoveUp','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'moveUp();\'}'},
{'id':'btnRowMoveDown','iconcls':'icon-down','text':'下移','TextEN':'RowMoveDown','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'moveDown();\'}'},
{'id':'btnRowSaveList','iconcls':'icon-save','text':'保存','TextEN':'RowSaveList','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'saveEditList();\'}'},
{'id':'btnPrint','iconcls':'icon-print','text':'打印','TextEN':'Print','Enabled':'true','Visible':'true','Settings':'{\'onclick\':\'printForm();\',SelectMode:\'mustSelectOneRow\',PopupTitle:\'打印\'}'},
{'id':'btnAddSubnode', 'URL':'/MvcConfig/UI/Form/PageView?TmplCode={TmplCode}&ParentID={ID}&IsTreeGrid=true','iconcls':'icon-add','text':'增加子节点','TextEN':'Add','Enabled':'false','Visible':'false','Settings':'{SelectMode:\'mustSelectOneRow\',PopupTitle:\'增加子节点\'}'},
{'id':'btnMoveNode', 'iconcls':'icon-left','text':'层级移动','TextEN':'MoveNode','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'moveNode();\',SelectMode:\'mustSelectRow\',Confirm:\'true\'}'},
{'id':'btnMoveLeftNode', 'iconcls':'icon-left','text':'向左移','TextEN':'MoveLeftNode','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'moveLeftNode();\',SelectMode:\'mustSelectRow\',Confirm:\'true\'}'},
{'id':'btnMoveRightNode', 'iconcls':'icon-right','text':'向右移','TextEN':'MoveRightNode','Enabled':'false','Visible':'false','Settings':'{\'onclick\':\'moveRightNode();\',SelectMode:\'mustSelectRow\',Confirm:\'true\'}'}
]";
                entity.LayoutButton.Replace("\r\n", "");
                entity.LayoutGrid = "{\"isTreeGrid\":\"false\",\"expandOnLoad\":\"true\",\"multiSelect\":\"true\",\"allowAlternating\":\"false\",\"frozenStartColumn\":\"\",\"frozenEndColumn\":\"\",\"drawcell\":\"\"}";
                entity.LayoutField = "[]";
                entity.LayoutSearch = "[]";
                entity.CompanyID = user.AdminCompanyID;
                entity.CompanyName = user.AdminCompanyName;
            }
            else
            {
                if (string.IsNullOrEmpty(entity.TableNames) && !string.IsNullOrEmpty(entity.LayoutGrid))
                {
                    var gridSetting = JsonHelper.ToObject(entity.LayoutGrid);
                    if (gridSetting.GetValue("allowCellEdit") == "true")
                        throw new BusinessException("列表允许【单元格编辑】时，【涉及到的表】必须有值！");
                }
            }
            var settings = new Dictionary<string, object>();
            settings.SetValue("ShowQueryForm", formDic.GetValue("ShowQueryForm"));
            settings.SetValue("QueryFormColmuns", formDic.GetValue("QueryFormColmuns"));
            entity.Settings = JsonHelper.ToJson(settings);  //settings.ToString(); 
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entity.ScriptText = Request["Script"];
            entity.SQL = Request["SQL"];

            return JsonSave<S_UI_List>(entity);
        }

        public JsonResult Delete()
        {
            return JsonDelete<S_UI_List>(Request["ListIDs"]);
        }

        #endregion

        #region Grid属性编辑

        public JsonResult GetLayoutGrid(string id)
        {
            string LayoutGrid = "";
            var listDef = entities.Set<S_UI_List>().FirstOrDefault(c => c.ID == id);
            if (listDef != null)
            {
                LayoutGrid = listDef.LayoutGrid;
                var settings = JsonHelper.ToObject(LayoutGrid);
                settings.SetValue("TableNames", listDef.TableNames);
                LayoutGrid = JsonHelper.ToJson(settings);
            }
            var layoutDef = entities.Set<S_UI_Layout>().FirstOrDefault(c => c.ID == id);
            if (layoutDef != null)
                LayoutGrid = layoutDef.LayoutGrid;

            return Json(LayoutGrid);
        }

        public ActionResult LayoutGridEdit(string id)
        {
            var listDef = entities.Set<S_UI_List>().FirstOrDefault(c => c.ID == id);
            if (listDef != null)
                ViewBag.FieldEnum = listDef.LayoutField;
            var layoutDef = entities.Set<S_UI_Layout>().FirstOrDefault(c => c.ID == id);
            if (layoutDef != null)
                ViewBag.FieldEnum = layoutDef.LayoutField;
            return View();
        }

        public JsonResult SaveLayoutGrid(string id)
        {
            var user = FormulaHelper.GetUserInfo();
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            if (entity != null)
            {
                entity.LayoutGrid = Request["FormData"];

                if (string.IsNullOrEmpty(entity.TableNames) && !string.IsNullOrEmpty(entity.LayoutGrid))
                {
                    var gridSetting = JsonHelper.ToObject(entity.LayoutGrid);
                    if (gridSetting.GetValue("allowCellEdit") == "true")
                        throw new BusinessException("列表允许【单元格编辑】时，【涉及到的表】必须有值！");
                }

                entity.ModifyTime = DateTime.Now;
                entity.ModifyUserID = user.UserID;
                entity.ModifyUserName = user.UserName;
            }
            else
            {
                var layout = entities.Set<S_UI_Layout>().FirstOrDefault(c => c.ID == id);
                layout.LayoutGrid = Request["FormData"];
                layout.ModifyTime = DateTime.Now;
                layout.ModifyUserID = user.UserID;
                layout.ModifyUserName = user.UserName;
            }
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 字段布局

        public ActionResult SettingsButtonEdit()
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(0, new { value = "SystemOrg", text = "选择部门" });
            list.Insert(0, new { value = "SystemUser", text = "选择用户" });
            ViewBag.SelectorEnum = JsonHelper.ToJson(list);
            return View();
        }

        public ActionResult LayoutFieldEdit()
        {
            var id = this.GetQueryString("ID");
            var listObj = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            var isColumnEdit = false; var showPager = true;
            if (!string.IsNullOrEmpty(listObj.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(listObj.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true")
                    isColumnEdit = true;
                if (settings.GetValue("showPager") == "false")
                    showPager = false;
            }
            ViewBag.IsColumnEdit = isColumnEdit;
            ViewBag.ShowPager = showPager;
            return View();
        }

        public JsonResult GetLayoutField(string id)
        {
            return Json(entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id).LayoutField);
        }

        public JsonResult SaveLayoutField(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            entity.LayoutField = Request["layoutField"];

            //字段名称不能重复，当有配置多表头时，字段名称若有重复，会导致应用程序池停止
            if (!string.IsNullOrEmpty(entity.LayoutField))
            {
                var fields = JsonHelper.ToObject<List<Dictionary<string, string>>>(entity.LayoutField);
                var groups = fields.GroupBy(a => a.GetValue("header"));
                var errorFields = groups.Where(a => a.Count() > 1).Select(a => a.Key).ToArray();
                if (errorFields.Count() > 0)
                    throw new BusinessException("字段名称不能重复：【" + string.Join("，", errorFields) + "】");
            }

            //允许单元格编辑后，字段保存时，不在表里的必须只读
            if (!string.IsNullOrEmpty(entity.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(entity.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true" && !string.IsNullOrEmpty(entity.LayoutField))
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
                    var fieldDt = sqlHelper.ExecuteDataTable("select top 1 * from " + entity.TableNames.Split(',')[0]);
                    var fields = JsonHelper.ToObject<List<Dictionary<string, string>>>(entity.LayoutField);
                    string errorFields = string.Empty;
                    foreach (var field in fields)
                    {
                        var key = field.GetValue("field");
                        if (!fieldDt.Columns.Contains(key) && field.GetValue("Visible") == "true"
                            && !string.IsNullOrEmpty(field.GetValue("Edit_ItemType")) && field.GetValue("readOnly") != "true")
                            errorFields += key + "<br>";
                    }
                    if (!string.IsNullOrEmpty(errorFields))
                        throw new BusinessException("以下字段不在表" + entity.TableNames.Split(',')[0] + "中，必须设置只读<br>" + errorFields);
                }
            }

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }


        public JsonResult ImportField(string id)
        {

            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            var list = JsonHelper.ToList(entity.LayoutField ?? "[]");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
            //string sql = entity.SQL.Split(new string[] { "ORDER BY", "order by", "Order By", "Order by" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            string sql = "";
            if (Config.Constant.IsOracleDb)
                sql = string.Format("select * from ({0}) table1 where 1=2", entity.SQL);
            else
                sql = string.Format("select * from ({0}) as table1 where 1=2", entity.SQL);

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            var tableNames = entity.TableNames.ToUpper().Split(',');
            var mFields = entities.Set<S_M_Field>().Where(c => tableNames.Contains(c.S_M_Table.Code.ToUpper())).ToList();

            foreach (DataColumn col in dt.Columns)
            {
                string code = col.ColumnName;
                string name = code;
                if (list.Where(c => c["field"].ToString() == code).Count() > 0)
                    continue;
                var mField = mFields.FirstOrDefault(c => c.Code == code);
                if (mField != null && !string.IsNullOrEmpty(mField.Name))
                    name = mField.Name;
                var dic = new Dictionary<string, object>();
                dic.Add("field", code);
                dic.Add("header", name);
                dic.Add("width", "");
                dic.Add("align", "left");
                dic.Add("Visible", "true");
                dic.Add("allowSort", "true");
                dic.Add("AllowSearch", "false");
                dic.Add("QueryMode", "");

                if (col.DataType == typeof(DateTime))
                    dic.Add("Settings", "{dateFormat:'yyyy-MM-dd'}");
                else if (code == "FlowPhase")
                {
                    dic.Add("Settings", "{EnumKey:'FlowPhase'}");
                }
                else
                    dic.Add("Settings", "{}");
                list.Add(dic);
            }

            return Json(list);
        }

        #endregion

        #region 默认值

        public JsonResult getDefaultValueSettings(string listID)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == listID);
            return Json(entity.DefaultValueSettings ?? "");
        }

        public JsonResult SaveDefaultValueSettings(string listID)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == listID);
            entity.DefaultValueSettings = Request["DefaultValueSettings"];
            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 按钮布局

        public ActionResult SettingsButton(string id)
        {
            var listDef = entities.Set<S_UI_List>().FirstOrDefault(c => c.ID == id);
            if (listDef != null)
                ViewBag.FieldEnum = listDef.LayoutField;
            var layoutDef = entities.Set<S_UI_Layout>().FirstOrDefault(c => c.ID == id);
            if (layoutDef != null)
                ViewBag.FieldEnum = layoutDef.LayoutField;
            return View();
        }

        public ActionResult LayoutButtonEdit()
        {
            string path = Server.MapPath("/CommonWebResource/Theme/Default/MiniUI/icons");
            List<object> list = new List<object>();
            foreach (string item in System.IO.Directory.EnumerateFiles(path))
            {
                string name = item.Split('\\').Last().Split('.').First();
                list.Add(new { value = "icon-" + name, text = name });
            }
            ViewBag.IconEnum = JsonHelper.ToJson(list);
            return View();
        }

        public JsonResult GetLayoutButton(string id)
        {
            return Json(entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id).LayoutButton);
        }

        public JsonResult SaveLayoutButton(string id)
        {
            var entity = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == id);
            entity.LayoutButton = Request["layoutButton"];

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 克隆

        public JsonResult Clone(string listID)
        {
            var listInfo = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == listID);
            var newFormInfo = new S_UI_List();
            FormulaHelper.UpdateModel(newFormInfo, listInfo);
            newFormInfo.ID = FormulaHelper.CreateGuid();
            newFormInfo.Code += "copy";
            newFormInfo.Name += "(副本)";
            newFormInfo.ModifyTime = null;
            newFormInfo.ModifyUserID = "";
            newFormInfo.ModifyUserName = "";
            entities.Set<S_UI_List>().Add(newFormInfo);
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        #region 创建菜单入口

        public JsonResult CreateMenu(string ListID)
        {
            var list = entities.Set<S_UI_List>().SingleOrDefault(c => c.ID == ListID);

            string url = "/MvcConfig/UI/List/PageView?TmplCode=" + list.Code;

            if (entities.Set<S_A_Res>().Count(c => c.Url == url) > 0)
            {
                throw new BusinessException("菜单入口已存在！");
            }

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == list.CategoryID);
            var pMenu = entities.Set<S_A_Res>().FirstOrDefault(c => c.FullID.StartsWith(Config.Constant.MenuRooID) && c.Name == category.Name);
            if (pMenu == null)
                pMenu = entities.Set<S_A_Res>().SingleOrDefault(c => c.FullID.StartsWith(Config.Constant.MenuRooID) && c.ID == Config.Constant.MenuRooID);
            var menu = new S_A_Res();
            menu.ID = FormulaHelper.CreateGuid();
            menu.Name = list.Name;
            menu.Url = url;
            menu.ParentID = pMenu.ID;
            menu.FullID = pMenu.FullID + "." + menu.ID;
            menu.SortIndex = 9999;
            menu.Type = "Menu";
            entities.Set<S_A_Res>().Add(menu);
            S_A__OrgRes orgRes = new S_A__OrgRes();
            orgRes.OrgID = Config.Constant.OrgRootID;
            orgRes.ResID = menu.ID;
            entities.Set<S_A__OrgRes>().Add(orgRes);
            entities.SaveChanges();
            return Json(new { ID = menu.ID });
        }

        #endregion

        #region  授权到子公司

        public JsonResult SetCompanyAuth(string objIds, string orgIds, string orgNames)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "update S_UI_List set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')";
            sql = string.Format(sql, orgIds, orgNames, objIds.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        #endregion
    }
}
