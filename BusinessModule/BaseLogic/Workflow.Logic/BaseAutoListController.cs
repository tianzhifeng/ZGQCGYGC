using Base.Logic.BusinessFacade;
using Base.Logic.Domain;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;
using Config.Logic;
using Config;
using Formula.Exceptions;
using Workflow.Logic.BusinessFacade;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Transactions;

namespace Workflow.Logic
{
    public class BaseAutoListController : MvcAdapter.BaseController
    {
        #region entities

        protected override DbContext entities
        {
            get { return FormulaHelper.GetEntities<BaseEntities>(); }
        }

        public virtual DbContext BusinessEntities
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region 扩展虚方法

        protected virtual void BeforeSave(List<Dictionary<string, string>> list, S_UI_List listInfo)
        { }

        protected virtual void AfterSave(List<Dictionary<string, string>> list, List<Dictionary<string, string>> deleteList, S_UI_List listInfo)
        { }

        protected virtual void BeforeSaveDetail(Dictionary<string, string> detail, List<Dictionary<string, string>> list, S_UI_List listInfo, bool isNew)
        { }

        protected virtual void BeforeSaveDelete(List<Dictionary<string, string>> list, S_UI_List listInfo)
        { }

        protected virtual void BeforeDelete(string[] Ids)
        { }

        #endregion

        #region 页面初始化
        public virtual ActionResult ListView(string tmplCode)
        {
            return this.PageView(tmplCode);
        }

        public virtual ActionResult MultiSelector(string tmplCode)
        {
            return PageView(tmplCode);
        }

        public virtual ActionResult PageView(string tmplCode)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            ViewBag.ListHtml = uiFO.CreateListHtml(tmplCode);
            ViewBag.Script = uiFO.CreateListScript(tmplCode);
            ViewBag.RightKeyHtml = uiFO.GetGridRightKeyHtml(tmplCode);
            ViewBag.FixedFields = string.Format("var FixedFields={0};", Newtonsoft.Json.JsonConvert.SerializeObject(uiFO.GetFixedWidthFields(tmplCode)));
            ViewBag.Title = listDef.Name;
            ViewBag.VirtualScroll = "false";
            var isColumnEdit = false;
            if (!string.IsNullOrEmpty(listDef.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(listDef.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true")
                    isColumnEdit = true;
                if (!String.IsNullOrEmpty(settings.GetValue("virtualScroll")))
                {
                    ViewBag.VirtualScroll = settings.GetValue("virtualScroll");
                }
            }

            var tab = new Tab();
            if (!isColumnEdit)
            {
                //不可标签式查询
                var fields = JsonHelper.ToList(listDef.LayoutField);
                foreach (var field in fields)
                {
                    if (!field.ContainsKey("Settings"))
                        continue;
                    var settings = JsonHelper.ToObject(field["Settings"].ToString());

                    if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                        continue;

                    if (!settings.ContainsKey("TabSearchName") || settings["TabSearchName"].ToString() == "")
                        continue;


                    Category category = null;

                    //根据配置决定是否显示‘全部’选项
                    var hasAllAttr = true;
                    string enumKey = settings["EnumKey"].ToString();

                    if (settings.ContainsKey("ShowAll") && settings["ShowAll"].ToString() == "false"
                        && settings.ContainsKey("TabSearchDefault") && !String.IsNullOrEmpty(settings["TabSearchDefault"].ToString()))
                    {
                        hasAllAttr = false;
                    }
                    if (enumKey.StartsWith("["))
                        category = CategoryFactory.GetCategoryByString(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString(), hasAllAttr);
                    else
                        category = CategoryFactory.GetCategory(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString(), hasAllAttr);


                    //默认值
                    if (settings.ContainsKey("TabSearchDefault") && settings["TabSearchDefault"].ToString() != "")
                        category.SetDefaultItem(settings["TabSearchDefault"].ToString());
                    else
                        category.SetDefaultItem();

                    //多选或单选
                    if (settings.ContainsKey("TabSearchMulti") && settings["TabSearchMulti"].ToString() == "false")
                        category.Multi = false;
                    else
                        category.Multi = true;

                    tab.Categories.Add(category);
                }
            }

            if (tab.Categories.Count > 0)
            {
                tab.IsDisplay = true;
                ViewBag.Tab = tab;
                ViewBag.Layout = "~/Views/Shared/_AutoListLayoutTab.cshtml";
            }
            else
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            }

            return View();
        }
        #endregion

        #region GetList

        public virtual JsonResult GetTotal(string tmplCode)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            string sql = listDef.SQL;
            sql = uiFO.ReplaceString(sql);
            return Json(sqlHeler.ExecuteDataTable(sql).Rows.Count);
        }

        public virtual JsonResult GetList(string tmplCode, QueryBuilder qb)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            var lefSettings = JsonHelper.ToObject(listDef.LayoutGrid);
            if (lefSettings.GetValue("isTreeGrid") == true.ToString().ToLower() &&
                this.Request["IsLoadChildren"] == true.ToString().ToLower())
            {
                //异步加载树列表
                var node = String.IsNullOrEmpty(this.Request["NodeInfo"]) ? new Dictionary<string, object>() :
                    JsonHelper.ToObject<Dictionary<string, object>>(this.Request["NodeInfo"]);
                var defaultValues = JsonHelper.ToList(listDef.DefaultValueSettings);
                var nodeTypeField = String.IsNullOrEmpty(lefSettings.GetValue("NodeTypeField")) ? "NodeType" : lefSettings.GetValue("NodeTypeField");
                var nodeType = String.IsNullOrEmpty(node.GetValue(nodeTypeField)) ? "Root" : node.GetValue(nodeTypeField);
                var treeGridDataSource = defaultValues.Where(c => c.GetValue("treeGridSource") == true.ToString().ToLower()).ToList();
                var dataSource = new Dictionary<string, object>();
                if (treeGridDataSource.Count == 1)
                {
                    dataSource = treeGridDataSource.FirstOrDefault();
                }
                else
                {
                    dataSource = treeGridDataSource.FirstOrDefault(c => c.GetValue("Code").Split(',').Contains(nodeType));
                }
                if (dataSource == null)
                {
                    return Json(new DataTable());
                }
                var db = SQLHelper.CreateSqlHelper(dataSource.GetValue("ConnName"));
                var sourceSQL = dataSource.GetValue("SQL");
                sourceSQL = uiFO.ReplaceDicString(sourceSQL, null, node);
                var children = db.ExecuteDataTable(sourceSQL, new SearchCondition());
                return Json(children);
            }
            else
            {
                string sql = listDef.SQL;
                #region TAB查询
                //解决tab查询需要在sql中间的问题
                var tabData = Request["queryTabData"];
                var fields = JsonHelper.ToList(listDef.LayoutField);
                var tabEmbeddedFields = new Dictionary<string, string>();
                foreach (var field in fields)
                {
                    if (!field.ContainsKey("Settings"))
                        continue;
                    var settings = JsonHelper.ToObject(field["Settings"].ToString());
                    if (!settings.ContainsKey("TabSearchName") || settings["TabSearchName"].ToString() == "")
                        continue;
                    if (!settings.ContainsKey("Embedded") || settings["Embedded"].ToString() != true.ToString().ToLower())
                        continue;

                    var cnd = qb.Items.FirstOrDefault(c => c.Field == field["field"].ToString());
                    if (cnd != null)
                    {
                        qb.Items.Remove(cnd);
                        var value = cnd.Value.ToString();
                        if (value.Contains(",") && (cnd.Method == QueryMethod.InLike || cnd.Method == QueryMethod.In))
                        {
                            value = value.Replace(",", "','");
                        }
                        tabEmbeddedFields.Add(field["field"].ToString(), value);
                    }
                    else
                    {
                        if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                            continue;
                        string enumKey = settings["EnumKey"].ToString();
                        var enumList = new List<Dictionary<string, object>>();
                        if (enumKey.StartsWith("["))
                            enumList = JsonHelper.ToList(enumKey);
                        else
                        {
                            var enumServcie = FormulaHelper.GetService<IEnumService>();
                            var dataTable = enumServcie.GetEnumTable(enumKey);
                            enumList = FormulaHelper.DataTableToListDic(dataTable);
                        }
                        var value = String.Join(",", enumList.Select(c => c["value"].ToString()).ToList());
                        if (value.Contains(","))
                        {
                            value = value.Replace(",", "','");
                        }
                        tabEmbeddedFields.Add(field["field"].ToString(), value);
                    }
                }
                #endregion

                sql = uiFO.ReplaceString(sql, null, tabEmbeddedFields);

                #region 地址栏过滤
                DataTable dtTmpl = sqlHeler.ExecuteDataTable(string.Format("SELECT * FROM ({0}) T WHERE 1=2", sql));
                foreach (string key in Request.QueryString.Keys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    if ("ID,FullID,FULLID,TmplCode,IsPreView,_winid,_t".Split(',').Contains(key) || key.StartsWith("$"))
                        continue;
                    if (dtTmpl.Columns.Contains(key))
                        qb.Add(key, QueryMethod.In, Request[key]); ;
                }
                #endregion

                //列表定义支持加密字段
                if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                {
                    StringBuilder sbField = new StringBuilder();
                    foreach (DataColumn col in dtTmpl.Columns)
                    {
                        if (col.DataType == typeof(byte[]) && fields.Count(c => c["field"].ToString() == col.ColumnName) > 0)
                        {
                            sbField.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0}))", col.ColumnName);
                        }
                        else
                        {
                            sbField.AppendFormat(",{0}", col.ColumnName);
                        }
                    }
                    sql = string.Format("select {1} from ({0}) as dt0", sql, sbField.ToString().Trim(' ', ','));
                }

                GridData data = null;
                if (listDef.LayoutGrid.Contains("\"showPager\":\"false\""))
                {
                    qb.PageSize = 0;
                }
                data = sqlHeler.ExecuteGridData(sql, qb, listDef.OrderBy);

                #region 计算汇总
                StringBuilder sb = new StringBuilder();
                foreach (var field in fields)
                {
                    if (field.ContainsKey("Settings") == false)
                        continue;
                    var settings = JsonHelper.ToObject(field["Settings"].ToString());
                    if (settings.ContainsKey("Collect") == false || settings["Collect"].ToString() == "")
                        continue;
                    if (Config.Constant.IsOracleDb)
                        sb.AppendFormat(",{1}({0}) as {0}", field["field"], settings["Collect"]);
                    else
                        sb.AppendFormat(",{0}={1}({0})", field["field"], settings["Collect"]);

                    if (settings["Collect"].ToString() == "sum")
                        data.sumData.Add(field["field"].ToString(), null);
                    else
                        data.avgData.Add(field["field"].ToString(), null);
                }
                if (sb.Length > 0)
                {
                    string companyAuth = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"] == "True")
                    {
                        var dt = sqlHeler.ExecuteDataTable(string.Format("select * from ({0}) tempDt1 where 1=2", sql));
                        if (dt.Columns.Contains("CompanyID"))
                        {
                            companyAuth = string.Format(" and CompanyID='{0}'", FormulaHelper.GetUserInfo().UserCompanyID);
                        }
                    }

                    string collectSql = "";
                    SearchCondition authCnd = FormulaHelper.CreateAuthDataFilter();
                    collectSql = string.Format("select * from ({0}) sourceTable1 where 1=1 {1} {2}", sql, authCnd.GetWhereString(false), companyAuth);
                    collectSql = string.Format("select {2} from ({0}) sourceTable {1}", collectSql, qb.GetWhereString(), sb.ToString().Trim(','));
                    DataTable dtCollect = sqlHeler.ExecuteDataTable(collectSql);

                    foreach (DataColumn col in dtCollect.Columns)
                    {
                        if (data.sumData.ContainsKey(col.ColumnName))
                            data.sumData[col.ColumnName] = dtCollect.Rows[0][col] is DBNull ? 0 : dtCollect.Rows[0][col];
                        else
                            data.avgData[col.ColumnName] = dtCollect.Rows[0][col] is DBNull ? 0 : dtCollect.Rows[0][col];
                    }
                }

                //汇总数据小数点
                foreach (var field in fields)
                {
                    if (field.ContainsKey("Settings") == false)
                        continue;
                    var settings = JsonHelper.ToObject(field["Settings"].ToString());
                    if (settings.ContainsKey("Collect") == false || settings["Collect"].ToString() == "")
                        continue;
                    int decimalPlaces = 2;
                    if (settings.ContainsKey("decimalPlaces") && settings["decimalPlaces"].ToString() != "")
                        decimalPlaces = Convert.ToInt32(settings["decimalPlaces"]);

                    string fieldCode = field["field"].ToString();

                    if (data.sumData.ContainsKey(fieldCode))
                    {
                        data.sumData[fieldCode] = string.Format("{0:F" + decimalPlaces + "}", Convert.ToDouble(data.sumData[fieldCode]));
                    }
                    else
                    {
                        data.avgData[fieldCode] = string.Format("{0:F" + decimalPlaces + "}", Convert.ToDouble(data.avgData[fieldCode]));
                    }
                }

                #endregion

                return Json(data);
            }
        }

        public virtual JsonResult GetSelectedList(string tmplCode, string ids)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            string sql = listDef.SQL;
            sql = uiFO.ReplaceString(sql);
            sql = string.Format("select * form ({0}) as dt0 where ID in ('{2}')", sql, ids.Replace(",", "','"));
            var dt = sqlHeler.ExecuteDataTable(sql);
            return Json(dt);
        }
        #endregion

        #region Delete

        public virtual JsonResult Delete(string tmplCode, string listIDs)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);

            if (listDef.DenyDeleteFlow == "1" || listDef.DenyDeleteFlow == "true")
            {
                SQLHelper flowHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                string flowSql = string.Format("select count(1) from S_WF_InsFlow where FormInstanceID in('{0}')", listIDs.Replace(",", "','"));
                var obj = flowHelper.ExecuteScalar(flowSql);
                if (obj.ToString() != "0")
                    throw new BusinessException("已经启动流程的表单不能删除！");
            }
            if (!String.IsNullOrEmpty(listIDs))
                this.BeforeDelete(listIDs.Split(','));

            SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);
            string sql = string.Format("delete from {0} where ID in('{1}')", listDef.TableNames.Split(',')[0], listIDs.Replace(",", "','"));
            sqlHeler.ExecuteNonQuery(sql);

            FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
            foreach (string id in listIDs.Split(','))
            {
                flowFO.DeleteFlowByFormInstanceID(id);
            }

            return Json("");
        }
        #endregion

        #region MoveNode
        public virtual JsonResult MoveNode(string tmplCode, string listIDs, string moveType, MvcAdapter.QueryBuilder qb)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", tmplCode));
            var layoutGrid = JsonHelper.ToObject(listDef.LayoutGrid);
            var parentField = layoutGrid.GetValue("parentField");
            var ids = listIDs.Split(',');
            if (!string.IsNullOrEmpty(parentField))
            {
                if (ids.Count() > 0)
                {
                    listIDs = listIDs.Replace(",", "','");
                    SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(listDef.ConnName);
                    string tableName = listDef.TableNames.Split(',')[0];
                    Dictionary<string, string> dics = new Dictionary<string, string>();
                    if (moveType == "left") //向左移动
                    {
                        string sql = string.Format(@"select * from (select ID,{0} from {1} where ID in (
	                    select {0} from {1} where ID in ('{2}'))
                        union all
                        select ID,{0} from {1} where ID in ('{2}')) a group by ID,{0}", parentField, tableName, listIDs);
                        var table = sqlHeler.ExecuteDataTable(sql);
                        foreach (var id in ids)
                        {
                            var dr = table.Select(string.Format(" ID='{0}'", id));
                            if (dr.Count() > 0)
                            {
                                var parentID = Convert.ToString(dr.First()[parentField]);
                                if (!string.IsNullOrEmpty(parentID))
                                {
                                    var parent = table.Select(string.Format(" ID='{0}'", parentID));
                                    if (parent.Count() > 0)
                                    {
                                        dics.Add(id, Convert.ToString(parent.First()[parentField]));
                                    }
                                }
                            }
                        }
                    }
                    else if (moveType == "right")
                    { //向右移动 
                        if (ids.Count() > 0)
                        {
                            #region 处理数据
                            List<Hierarchy> cacheLevel = new List<Hierarchy>();
                            var listData = Request["ListData"];
                            if (!string.IsNullOrEmpty(listData))
                            {
                                var datas = JsonHelper.ToList(listData);
                                foreach (var item in datas)
                                {
                                    cacheLevel.Add(new Hierarchy()
                                    {
                                        ID = item.GetValue("ID"),
                                        ParentID = item.GetValue(parentField)
                                    });
                                }
                            }
                            #endregion
                            if (cacheLevel.Count() > 0)
                            {
                                var selObjs = cacheLevel.Where(c => ids.Contains(c.ID)).ToList(); //选择的数据
                                if (selObjs.Count > 0)
                                {
                                    Dictionary<string, List<Hierarchy>> dic = new Dictionary<string, List<Hierarchy>>();
                                    if (selObjs.Count > 0) //选择的数据根据层级放到Dictionary中
                                    {
                                        for (int i = 0; i < selObjs.Count; i++)
                                        {
                                            var single = dic.Where(c => c.Key.Contains(selObjs[i].ParentID));
                                            if (single.Count() <= 0 || single.First().Value == null)
                                            {
                                                var obj = selObjs[i];
                                                List<Hierarchy> list = new List<Hierarchy>();
                                                list.Add(obj);
                                                dic.Add(obj.ParentID, list);
                                            }
                                            else
                                            {
                                                single.First().Value.Add(selObjs[i]);
                                            }
                                        }
                                    }
                                    if (dic.Count > 0)
                                    {
                                        foreach (var item in dic)
                                        {
                                            if (item.Value != null)
                                            {
                                                var prvNodeID = "";
                                                foreach (var obj in item.Value)
                                                {
                                                    var objID = obj.ID; //默认的ID
                                                    for (int i = 0; i < cacheLevel.Count; i++)
                                                    {
                                                        if (cacheLevel[i].ID == objID)
                                                        {
                                                            if (i > 0)
                                                            {
                                                                var pid = obj.ParentID;
                                                                var parents = cacheLevel.Where(c => c.ParentID == pid).ToList();
                                                                if (parents.Count() > 0)
                                                                {
                                                                    for (int j = 0; j < parents.Count(); j++)
                                                                    {
                                                                        if (parents[j].ID == objID)
                                                                        {
                                                                            if (j > 0)
                                                                            {
                                                                                objID = parents[j - 1].ID;
                                                                                for (int index = 0; index < item.Value.Count; index++)
                                                                                {
                                                                                    if (item.Value[index].ID == obj.ID)
                                                                                    {
                                                                                        if (index > 0 && item.Value[index - 1].ID == objID && !string.IsNullOrEmpty(prvNodeID))
                                                                                        {
                                                                                            objID = prvNodeID;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                objID = "";
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    prvNodeID = objID;
                                                    if (!string.IsNullOrEmpty(objID) && obj.ID != objID)
                                                        dics.Add(obj.ID, objID);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        if (ids.Length < 2)
                            throw new BusinessException("必须选择两条及以上记录,选择的第一条为父级！");
                        #region 处理数据
                        List<Hierarchy> cacheLevel = new List<Hierarchy>();
                        var listData = Request["ListData"];
                        if (!string.IsNullOrEmpty(listData))
                        {
                            var datas = JsonHelper.ToList(listData);
                            foreach (var item in datas)
                            {
                                cacheLevel.Add(new Hierarchy()
                                {
                                    ID = item.GetValue("ID"),
                                    ParentID = item.GetValue(parentField)
                                });
                            }
                        }
                        #endregion
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (i != 0)
                            {
                                bool isSubNode = false;
                                var datas = cacheLevel.Where(c => ids.Contains(c.ID)).ToList();
                                var first = datas.SingleOrDefault(c => c.ID == ids[0]);
                                foreach (var item in datas)
                                {
                                    if (first != null && item.ID == first.ParentID)
                                    {
                                        isSubNode = true;
                                        break;
                                    }
                                }
                                if (!isSubNode)
                                    dics.Add(ids[i], ids[0]);
                                else
                                    throw new BusinessException("请注意上下级关系,上级节点不能移动到下级节点!");
                            }
                        }
                    }

                    if (dics.Count > 0)
                    {
                        List<string> upList = new List<string>();
                        foreach (var item in dics)
                        {
                            upList.Add(string.Format("update {0} set {1}='{2}' where ID='{3}'", tableName, parentField, item.Value, item.Key));
                        }
                        sqlHeler.ExecuteNonQuery(string.Join(@" 
                        ", upList.ToArray()));
                    }
                }
            }

            return Json("");
        }

        #endregion

        #region Save

        [ValidateInput(false)]
        public virtual JsonResult SaveSortedList()
        {
            var tmplCode = Request["TmplCode"];
            try
            {
                Action action = () =>
                {
                    string sortedListData = Request["SortedListData"];//所有数据
                    string deletedListData = Request["DeletedListData"];
                    if (string.IsNullOrEmpty(sortedListData))
                    {
                        throw new BusinessException("列表数据无法反序列化");
                    }
                    var sortedDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(sortedListData);
                    var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(listDef.ConnName);
                    var fieldDt = DbHelper.GetFieldTable(listDef.ConnName, listDef.TableNames.Split(',')[0]).AsEnumerable();
                    StringBuilder sql = new StringBuilder();
                    //删除
                    var deleteDataList = new List<Dictionary<string, string>>();
                    if (!string.IsNullOrEmpty(deletedListData))
                    {
                        deleteDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(deletedListData);
                        this.BeforeSaveDelete(deleteDataList, listDef);
                        var idAry = deleteDataList.Select(a => a.GetValue("ID"));
                        var ids = string.Join("','", idAry);
                        if (!string.IsNullOrEmpty(ids))
                            sql.AppendFormat("\n delete from {0} where ID in('{1}');", listDef.TableNames.Split(',')[0], ids);
                    }
                    //已经存在数据
                    var oldIds = new List<string>();
                    string whereStr = string.Empty;//根据url参数筛选数据
                    var fields = fieldDt.Select(c => c[0].ToString()).ToArray();
                    foreach (var key in HttpContext.Request.QueryString.AllKeys)
                    {
                        if (fields.Contains(key))
                        {
                            if (key.ToLower() == "id") continue;
                            var value = HttpContext.Request.QueryString[key];
                            if (!string.IsNullOrEmpty(value))
                                whereStr += string.Format(" and {0}='{1}'", key, value);
                        }
                    }
                    oldIds = sqlHelper.ExecuteDataTable(string.Format("select ID from {0} where 1=1 {1}", listDef.TableNames.Split(',')[0], whereStr))
                       .AsEnumerable().Select(c => c.Field<string>("ID")).ToList();
                    //新增、修改
                    this.BeforeSave(sortedDataList, listDef);
                    int index = 0;
                    foreach (var subItem in sortedDataList)
                    {
                        var isNew = false;
                        string id = subItem.GetValue("ID");
                        if (string.IsNullOrEmpty(id) || !oldIds.Contains(id))
                        {
                            isNew = true;
                            id = FormulaHelper.CreateGuid();
                        }
                        subItem.SetValue("ID", id);
                        if (fields.Contains("SortIndex"))
                            subItem["SortIndex"] = index++.ToString();
                        sql.Append("\n");
                        this.BeforeSaveDetail(subItem, sortedDataList, listDef, isNew);
                        if (isNew)
                            sql.Append(subItem.CreateInsertSql(listDef.ConnName, listDef.TableNames.Split(',')[0], id, fieldDt));
                        else
                            sql.Append(subItem.CreateUpdateSql(listDef.ConnName, listDef.TableNames.Split(',')[0], id, fieldDt));
                    }

                    if (sql.ToString() != "")
                        if (Config.Constant.IsOracleDb)
                        {
                            sqlHelper.ExecuteNonQuery(string.Format(@"
begin
{0}
end;", sql));
                        }
                        else
                        {
                            sqlHelper.ExecuteNonQuery(sql.ToString());
                        }
                    if (BusinessEntities != null)
                        BusinessEntities.SaveChanges();
                    this.AfterSave(sortedDataList, deleteDataList, listDef);
                };

                if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        action();
                        ts.Complete();
                    }
                }
                else
                {
                    action();
                }

                return Json("");
            }
            catch (BusinessValidationException ex)
            {
                throw ex;
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch (DbEntityValidationException ex)
            {
                string info = "错误：" + ex.Message;
                info += "<br>详细：<br>";
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var item2 in item.ValidationErrors)
                    {
                        var etyName = item.Entry.Entity.GetType().Name;
                        if (etyName.LastIndexOf('_') > 0)
                            etyName = etyName.Substring(0, etyName.LastIndexOf('_'));
                        info = string.Format("【{0}】{1}<br>", etyName, item2.ErrorMessage);
                    }
                }
                throw new Formula.Exceptions.BusinessException(info, ex);
            }
            catch (EntityCommandExecutionException ex)
            {
                string info = "错误：" + ex.Message;
                if (ex.InnerException != null)
                    info += "<br>详细：" + ex.InnerException.Message;
                throw new FlowException(info, ex);
            }
            catch (Exception ex)
            {
                string info = string.Format(@"保存表单Save,参数信息(TmplCode:{0}),<br>
                    出错原因分析:<br>
                        1.表单数据出错,请检查是否存在特殊字符<br>
                        2.TmplCode不能包含数据库执行语句不识别的特殊字符<br>", tmplCode);
                throw new FlowException(info, ex);
            }
        }

        [ValidateInput(false)]
        public virtual JsonResult SaveList()
        {
            var tmplCode = Request["TmplCode"];
            try
            {
                Action action = () =>
                {
                    string listData = Request["ListData"];
                    if (string.IsNullOrEmpty(listData))
                    {
                        throw new BusinessException("列表数据无法反序列化");
                    }
                    var subTableDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(listData);
                    var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(listDef.ConnName);
                    var fieldDt = DbHelper.GetFieldTable(listDef.ConnName, listDef.TableNames.Split(',')[0]).AsEnumerable();
                    StringBuilder sql = new StringBuilder();
                    //删除
                    //modified、added、removed
                    var deleteDataList = subTableDataList.Where(a => a.GetValue("_state") == "removed").ToList();
                    this.BeforeSaveDelete(deleteDataList, listDef);
                    var idAry = deleteDataList.Select(a => a.GetValue("ID"));
                    var ids = string.Join("','", idAry);
                    if (!string.IsNullOrEmpty(ids))
                        sql.AppendFormat("\n delete from {0} where ID in('{1}');", listDef.TableNames.Split(',')[0], ids);
                    subTableDataList.RemoveWhere(a => a.GetValue("_state") == "removed");
                    //新增、修改
                    this.BeforeSave(subTableDataList, listDef);
                    foreach (var subItem in subTableDataList)
                    {
                        var isNew = false;
                        string id = subItem.GetValue("ID");
                        //modified、added、removed
                        if (subItem.GetValue("_state") == "removed")
                            continue;
                        if (subItem.GetValue("_state") == "added" || string.IsNullOrEmpty(id))
                        {
                            isNew = true;
                            id = FormulaHelper.CreateGuid();
                        }
                        subItem.SetValue("ID", id);
                        if (subItem.ContainsKey("ModifyDate"))
                        {
                            subItem.SetValue("ModifyDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (subItem.ContainsKey("ModifyUserID"))
                        {
                            subItem.SetValue("ModifyUserID", FormulaHelper.GetUserInfo().UserID);
                        }
                        if (subItem.ContainsKey("ModifyUser"))
                        {
                            subItem.SetValue("ModifyUser", FormulaHelper.GetUserInfo().UserName);
                        }
                        this.BeforeSaveDetail(subItem, subTableDataList, listDef, isNew);
                        sql.Append("\n");
                        if (isNew)
                            sql.Append(subItem.CreateInsertSql(listDef.ConnName, listDef.TableNames.Split(',')[0], id, fieldDt));
                        else
                            sql.Append(subItem.CreateUpdateSql(listDef.ConnName, listDef.TableNames.Split(',')[0], id, fieldDt));
                    }

                    if (sql.ToString() != "")
                        if (Config.Constant.IsOracleDb)
                        {
                            sqlHelper.ExecuteNonQuery(string.Format(@"
begin
{0}
end;", sql));
                        }
                        else
                        {
                            sqlHelper.ExecuteNonQuery(sql.ToString());
                        }
                    if (BusinessEntities != null)
                        BusinessEntities.SaveChanges();
                    this.AfterSave(subTableDataList, deleteDataList, listDef);
                };

                if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        action();
                        ts.Complete();
                    }
                }
                else
                {
                    action();
                }

                return Json("");
            }
            catch (BusinessValidationException ex)
            {
                throw ex;
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch (DbEntityValidationException ex)
            {
                string info = "错误：" + ex.Message;
                if (ex.InnerException != null)
                    info += "<br>详细：" + ex.InnerException.Message;
                throw new FlowException(info, ex);
            }
            catch (EntityCommandExecutionException ex)
            {
                string info = "错误：" + ex.Message;
                if (ex.InnerException != null)
                    info += "<br>详细：" + ex.InnerException.Message;
                throw new FlowException(info, ex);
            }
            catch (Exception ex)
            {
                string info = string.Format(@"保存表单Save,参数信息(TmplCode:{0}),<br>
                    出错原因分析:<br>
                        1.表单数据出错,请检查是否存在特殊字符<br>
                        2.TmplCode不能包含数据库执行语句不识别的特殊字符<br>", tmplCode);
                throw new FlowException(info, ex);
            }
        }

        #endregion

        #region 导出HTML

        public virtual FileResult ExportHtml()
        {
            string tmplCode = Request["TmplCode"];
            var uiFO = FormulaHelper.CreateFO<UIFO>();

            StringBuilder html = new StringBuilder();
            html.Append(uiFO.CreateListHtml(tmplCode));
            html.AppendLine();
            html.Append("<script type='text/javascript'>");
            html.Append(uiFO.CreateListScript(tmplCode, true));
            html.AppendLine();
            html.Append("</script>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(html.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", Request["TmplCode"] + ".cshtml");

        }





        #endregion




        #region 二维码

        private string GetQRCodePath()
        {
            string tmplPath = "/MvcConfig/Scripts/Images/";
            string directoryPath = System.Web.HttpContext.Current.Server.MapPath(tmplPath);
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        public void CreateQRCodes(string tmplCode, string listData)
        {
            var listDef = entities.Set<S_UI_List>().FirstOrDefault(c => c.Code == tmplCode);
            string pdfName = GetQRCodePath() + "QRCode/" + listDef.Name + ".pdf";
            var fields = JsonHelper.ToList(listDef.LayoutField);
            var listDic = JsonHelper.ToObject<List<Dictionary<string, object>>>(listData);
            QRCode.NewSourceImage();
            foreach (var field in fields)
            {
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("QRCode") && settings["QRCode"].ToString().ToLower() == "true")
                {
                    string key = field["field"].ToString();
                    foreach (var item in listDic)
                    {
                        string text = item[key].ToString();
                        CreateQRCode(item["ID"].ToString(), key, text, text);
                    }
                }

            }
            QRCode.TurnTheImageToPdf(pdfName);
        }

        public virtual FileResult QRCodeToPdf(string tmplCode)
        {
            var listDef = entities.Set<S_UI_List>().FirstOrDefault(c => c.Code == tmplCode);
            string pdfName = GetQRCodePath() + "QRCode/" + listDef.Name + ".pdf";
            return File(pdfName, "application/ms-pdf", listDef.Name + ".pdf");
        }

        public void DownLoadQRCode(string imgName)
        {
            string filePath = GetQRCodePath() + "QRCode/" + imgName;
            FileInfo fileInfo = new FileInfo(filePath);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + imgName);
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.WriteFile(fileInfo.FullName);
            Response.Flush();
            Response.End();
        }


        public void CreateQRCode(string id, string fieldCode, string text, string title = "")
        {
            string folder = GetQRCodePath();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", id + "_" + fieldCode);
            dic.Add("Title", title);
            dic.Add("Text", text);

            string logo = folder + "logo.png";
            QRCode.CreateQRCode(folder + "QRCode/", dic, logo, true, false);
        }


        #endregion
    }
}
