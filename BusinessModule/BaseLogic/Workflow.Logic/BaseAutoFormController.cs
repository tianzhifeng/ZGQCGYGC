using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Formula;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using Config;
using Config.Logic;
using Base.Logic.Domain;
using System.Transactions;
using Formula.Exceptions;
using Formula.Helper;
using Base.Logic.BusinessFacade;
using System.Data;
using System.Text;
using System.IO;
using System.Data.Entity;
using MvcAdapter;
using Base.Logic.Model.UI.Form;
using Formula.DynConditionObject;
using System.Reflection;
using System.Data.Entity.Validation;
using Base.Logic;

namespace Workflow.Logic
{
    public class BaseAutoFormController : MvcAdapter.BaseController, IFlowController
    {
        FlowService _flowService = null;
        protected FlowService flowService
        {
            get
            {
                if (_flowService == null)
                {
                    _flowService = new FlowService(this, Request["FormData"], Request["ID"], Request["TaskExecID"]);
                }
                return _flowService;
            }
        }

        protected override DbContext entities
        {
            get { return FormulaHelper.GetEntities<WorkflowEntities>(); }
        }

        public virtual DbContext BusinessEntities
        {
            get
            {
                return null;
            }
        }

        CaculateFo _fo = null;
        public CaculateFo CalFo
        {
            get
            {
                if (this._fo == null)
                {
                    _fo = FormulaHelper.CreateFO<CaculateFo>();
                }
                return _fo;
            }
        }

        #region 扩展虚方法

        protected virtual void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        { }

        protected virtual void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        { }

        protected virtual void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        { }

        protected virtual void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        { }

        protected virtual void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        { }

        protected virtual void BeforeDelete(string[] Ids)
        { }

        #endregion

        #region 页面初始化

        public virtual ActionResult PageView()
        {
            try
            {
                var upperVersionUrl = flowService.TransferToUpperVersionView();//升版逻辑
                if (!String.IsNullOrEmpty(upperVersionUrl))
                {
                    return Redirect(upperVersionUrl);
                }
                var uiFO = FormulaHelper.CreateFO<UIFO>();
                var formDef = UIFO.GetFormDef(Request["TmplCode"], Request["ID"]);
                bool isNew = false;
                DataTable formDataDT = null;
                if (Request["IsPreView"] == "True")
                {
                    formDataDT = new DataTable();
                    var row = formDataDT.NewRow();
                    formDataDT.Rows.Add(row);
                }
                else
                {
                    var UpperVersion = HttpContext.Request["UpperVersion"]; //要升版的表单ID
                    if (string.IsNullOrEmpty(UpperVersion))
                    {
                        string ID = Request["ID"];
                        string tmplCode = Request["TmplCode"];
                        var formInfo = baseEntities.Set<S_UI_Form>().OrderByDescending(c => c.CreateTime).FirstOrDefault(c => c.Code == tmplCode);
                        if (formInfo == null) throw new Exception("没有找到编号为【" + tmplCode + "】的表单定义");
                        ViewBag.Title = formInfo.Name;
                        string sql = string.Format("select count(ID) from {0} where ID='{1}'", formInfo.TableName, ID);
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                        var count = Convert.ToInt32(sqlHelper.ExecuteScalar(sql));
                        if (count == 0)
                        {
                            isNew = true;
                        }
                        formDataDT = GetModelDT(ID);
                    }
                    else
                    {
                        isNew = true;
                        formDataDT = GetModelDT(UpperVersion, true);
                        formDataDT.Rows[0]["ID"] = FormulaHelper.CreateGuid();
                        formDataDT.Rows[0]["FlowPhase"] = "";
                        formDataDT.Rows[0]["VersionNumber"] = "";
                    }

                    if (isNew)
                    {
                        //允许在数据初始化的时候进行计算逻辑的执行 2019-10-30 by Eric.Yang
                        this.CalculateFormData(formDataDT.Rows[0], formDef);
                    }
                    //允许在数据加载化的时候进行计算逻辑的执行 2019-11-21 by Eric.Yang
                    this.CalculateFormData(formDataDT.Rows[0], formDef, "FormLoad");
                    AfterGetData(formDataDT, isNew, UpperVersion);
                }
                ViewBag.FormData = JsonHelper.ToJson(formDataDT).Trim('[', ']');
                var fieldInfo = JsonHelper.ToJson(uiFO.GetFieldInfo(formDef));
                if (fieldInfo == "[]" || fieldInfo == "")
                    ViewBag.FieldInfo = "{}";
                else
                    ViewBag.FieldInfo = fieldInfo.Trim('[', ']');

                ViewBag.FormHtml = uiFO.CreateFormHtml(formDef);
                ViewBag.HiddenHtml = uiFO.CreateFormHiddenHtml(formDef);
                //此处允许开发者在自定义的controller中增加自己需要的脚本，同平台的脚本一起绘制在PageView中  by Eric.Yang 2019-3-27
                if (ViewBag.Script != null && !String.IsNullOrEmpty(ViewBag.Script))
                {
                    ViewBag.Script += "\n " + uiFO.CreateFormScript(formDef);
                }
                else
                {
                    ViewBag.Script = uiFO.CreateFormScript(formDef);
                }

                #region 子表大数据时，需要配置表单虚滚动加载
                var items = JsonHelper.ToObject<List<FormItem>>(formDef.Items);
                ViewBag.VirtualScroll = false.ToString().ToLower();
                var subTableList = items.Where(c => c.ItemType == "SubTable").ToList();
                foreach (var subTableItem in subTableList)
                {
                    if (String.IsNullOrEmpty(subTableItem.Settings)) continue;
                    var dic = JsonHelper.ToObject(subTableItem.Settings);
                    var dicSubTableSettings = JsonHelper.ToObject(dic["formData"].ToString());
                    if (dicSubTableSettings.GetValue("IsVirtualScroll") == "true")
                    {
                        ViewBag.VirtualScroll = true.ToString().ToLower();
                        break;
                    }
                }
                #endregion

                if (ViewBag.DataSource == null)
                {
                    string tmplCode = Request["TmplCode"];
                    var formInfo = baseEntities.Set<S_UI_Form>().OrderByDescending(c => c.CreateTime).FirstOrDefault(c => c.Code == tmplCode);
                    var defaultValueRows = uiFO.GetDefaultValueDic(formInfo.DefaultValueSettings);

                    #region 数据源到前台（开始叫“默认值”）2015-7-21
                    StringBuilder sb = new StringBuilder();
                    foreach (var key in defaultValueRows.Keys)
                    {
                        var guid = new Guid();
                        if (Guid.TryParse(key, out guid) == false)
                            sb.AppendFormat("\n var {0}={1}", key, JsonHelper.ToJson(defaultValueRows[key]));
                    }
                    ViewBag.DataSource = sb.ToString();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
            return View();
        }

        protected virtual void CalculateFormData(DataRow formData, S_UI_Form formDefine, string CalDefaultValue = "DefaultValue")
        {
            if (formDefine == null) throw new Formula.Exceptions.BusinessValidationException("计算表单数据时，表单定义对象不能为空值");
            if (formData == null) throw new Formula.Exceptions.BusinessValidationException("计算表单数据时，数据对象不能为空值");
            var data = FormulaHelper.DataRowToDic(formData);
            foreach (var calItem in formDefine.CalculateItems)
            {
                if (calItem.CalDefaultValue.Split(',').Contains(CalDefaultValue))
                    this.CalFo.CalculateCalItem(calItem, data);
            }
            foreach (var key in data.Keys)
            {
                if (formData.Table.Columns.Contains(key))
                {
                    formData[key] = data[key] == null ? DBNull.Value : data[key];
                }
            }
        }
        #endregion

        #region 导出HTML

        public virtual FileResult ExportHtml()
        {
            string tmplCode = Request["TmplCode"];
            var uiFO = FormulaHelper.CreateFO<UIFO>();
            var formDef = UIFO.GetFormDef(tmplCode, Request["ID"]);

            StringBuilder html = new StringBuilder(@"
<div class='mini-toolbar' id='btnDiv' style='width: 100%; position: fixed; top: 0; left: 0; z-index: 100;'>
        <table>
            <tr>
                <td>
                    <a id='btnSave' class='mini-button' plain='true' iconcls='icon-save' onclick='save();'>保存</a>
                    <a class='mini-button' plain='true' iconcls='icon-cancel' onclick='closeWindow()'>取消</a>
                </td>
                <td id='btnRight'>
                </td>
            </tr>
        </table>
</div>
<form id='dataForm' method='post' align='center' style='top: 30px; position: relative;'>
<input name='ID' class='mini-hidden' />
");

            html.AppendLine();
            html.Append(uiFO.CreateFormHiddenHtml(formDef));
            html.Append("\n<div class='formDiv'>");
            html.AppendLine();
            html.Append(uiFO.CreateFormHtml(formDef));
            html.AppendLine();
            html.Append(@"
</div>
</form>
<script type='text/javascript'>
");
            html.Append(uiFO.CreateFormScript(formDef, true));
            html.AppendLine();
            html.Append("</script>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(html.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", Request["TmplCode"] + ".cshtml");

        }





        #endregion

        #region 保存表单
        [ValidateInput(false)]
        public virtual JsonResult Save()
        {
            return this.SaveBA(Request["TmplCode"]);
        }
        #endregion

        public JsonResult SetSerialNumber(string id)
        {
            var tmplCode = Request["TmplCode"];
            var formInfo = UIFO.GetFormDef(tmplCode, id);
            Dictionary<string, object> serialNumberSettings = !String.IsNullOrEmpty(formInfo.SerialNumberSettings) ? JsonHelper.ToObject(formInfo.SerialNumberSettings) : null;
            string formData = Request["FormData"];
            Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
            var sqlcmd = new StringBuilder();
            if (String.IsNullOrEmpty(dic.GetValue("ÏD")))
            {
                //防止页面上没有ID的隐藏控件
                dic.SetValue("ID", id);
            }
            var db = SQLHelper.CreateSqlHelper(formInfo.ConnName);

            if (this.ExistForm(id))
            {
                //在流程执行到最后结束时，不存在新增情况，数据一定是存在的，如果不存在表单数据，则不做任何操作，说明数据出现了异常
                if (Config.Constant.IsOracleDb)
                {
                    if (dic.ContainsKey("SERIALNUMBER") && serialNumberSettings != null)
                    {
                        if (serialNumberSettings.ContainsKey("UserSerialNumber"))
                        {
                            var userSerialNumber = serialNumberSettings.GetValue("UserSerialNumber");
                            if (userSerialNumber.ToUpper().Trim().Equals("FLOWEND"))
                            {
                                dic["SERIALNUMBER"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                sqlcmd.AppendFormat("UPDATE {0} SET SERIALNUMBER='{1}' WHERE ID='{2}'", formInfo.TableName, dic["SERIALNUMBER"], id);
                            }
                        }
                    }
                }
                else
                {
                    if (dic.ContainsKey("SerialNumber") && serialNumberSettings != null)
                    {
                        if (serialNumberSettings.ContainsKey("UserSerialNumber"))
                        {
                            var userSerialNumber = serialNumberSettings.GetValue("UserSerialNumber");
                            if (userSerialNumber.ToUpper().Trim().Equals("FLOWEND"))
                            {
                                dic["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                sqlcmd.AppendFormat("UPDATE {0} SET SERIALNUMBER='{1}' WHERE ID='{2}'", formInfo.TableName, dic["SerialNumber"], id);
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(sqlcmd.ToString()))
                    if (Config.Constant.IsOracleDb)
                    {
                        db.ExecuteNonQuery(string.Format(@"
begin
{0}
end;", sqlcmd));
                    }
                    else
                    {
                        db.ExecuteNonQuery(sqlcmd.ToString());
                    }
            }
            else
            {
                throw new BusinessException("流程结束时候，未找到对应的表单数据，无法设置流水号");
            }

            return Json(dic);
        }

        #region 系统批量执行任务的保存方法
        [ValidateInput(false)]
        public virtual JsonResult SaveBA(string tmplCode)
        {
            try
            {
                DataTable formDataDT = new DataTable();
                Action action = () =>
                {
                    if (string.IsNullOrEmpty(tmplCode))
                        tmplCode = Request["TmplCode"];
                    string formData = Request["FormData"];
                    Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
                    string formID = dic["ID"];
                    var formInfo = UIFO.GetFormDef(tmplCode, formID);
                    StringBuilder sql = new StringBuilder();

                    //sql列
                    if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                    {
                        sql.AppendFormat(@"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate ");
                    }

                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                    var isTreeGrid = Request["IsTreeGrid"]; //是否树形
                    //保存流程意见
                    if (!string.IsNullOrEmpty(Request["FlowAdvice"]) && !string.IsNullOrEmpty(Request["TaskExecID"]))
                    {
                        string taskExecID = Request["TaskExecID"];
                        var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                        taskExec.ExecComment = Request["FlowAdvice"];
                        entities.SaveChanges();
                    }

                    //校验唯一
                    bool isNew = false;

                    Dictionary<string, object> serialNumberSettings = !String.IsNullOrEmpty(formInfo.SerialNumberSettings) ? JsonHelper.ToObject(formInfo.SerialNumberSettings) : null;

                    if (sqlHelper.ExecuteScalar(string.Format("select count(1) from {0} where ID='{1}'", formInfo.TableName, formID)).ToString() == "0")
                    {
                        #region

                        #region 设置流水号
                        //升版不生成流水号
                        if (Request["UpperVersion"] == null)
                        {
                            if (Config.Constant.IsOracleDb)
                            {
                                if (dic.ContainsKey("SERIALNUMBER") && serialNumberSettings != null) //重新获取并应用流水号
                                {
                                    if (serialNumberSettings.ContainsKey("UserSerialNumber"))
                                    {
                                        var userSerialNumber = serialNumberSettings.GetValue("UserSerialNumber");
                                        if (userSerialNumber.ToUpper().Trim().Equals("SUBMIT"))
                                        {
                                            dic["SERIALNUMBER"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                        }
                                        else if (userSerialNumber.ToUpper().Trim().Equals("FLOWEND") && String.IsNullOrEmpty(Request["FlowCode"]))
                                        {
                                            //如果设置了流水号配置定义为流程结束时生成编号，但是又没有配置流程，则自动根据提交时生成编号的规则来生成流水号
                                            dic["SERIALNUMBER"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (dic.ContainsKey("SerialNumber") && serialNumberSettings != null) //重新获取并应用流水号
                                {
                                    if (serialNumberSettings.ContainsKey("UserSerialNumber"))
                                    {
                                        var userSerialNumber = serialNumberSettings.GetValue("UserSerialNumber");
                                        if (userSerialNumber.ToUpper().Trim().Equals("SUBMIT"))
                                        {
                                            dic["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                        }
                                        else if (userSerialNumber.ToUpper().Trim().Equals("FLOWEND") && String.IsNullOrEmpty(Request["FlowCode"]))
                                        {
                                            //如果设置了流水号配置定义为流程结束时生成编号，但是又没有配置流程，则自动根据提交时生成编号的规则来生成流水号
                                            dic["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, true, null, dic);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        //树形添加子节点
                        if (!string.IsNullOrEmpty(isTreeGrid) && isTreeGrid.ToLower() == "true")
                        {
                            var listDef = baseEntities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
                            if (listDef == null)
                                throw new Exception(string.Format("编号为：“{0}”的列表不存在", tmplCode));
                            var layoutGrid = JsonHelper.ToObject(listDef.LayoutGrid);
                            var parentField = layoutGrid.GetValue("parentField");
                            if (!string.IsNullOrEmpty(parentField))
                                dic[parentField] = Request["ParentID"];
                        }

                        //校验唯一
                        ValidateUnique(formInfo, dic, formID);
                        isNew = true;
                        this.BeforeSave(dic, formInfo, isNew);
                        sql.Append(dic.CreateInsertSql(formInfo.ConnName, formInfo.TableName, formID));
                        #endregion
                    }
                    else
                    {
                        DataTable dt = GetModelDT(formID);
                        string json = JsonHelper.ToJson(dt);
                        var currentDic = JsonHelper.ToObject<List<Dictionary<string, string>>>(json)[0];
                        //校验唯一
                        ValidateUnique(formInfo, dic, formID);
                        isNew = false;
                        this.BeforeSave(dic, formInfo, isNew);

                        bool isFreeSerialNumber = false;
                        if ((dic.ContainsKey("SerialNumber") || dic.ContainsKey("SERIALNUMBER")) && serialNumberSettings != null && serialNumberSettings.ContainsKey("UserSerialNumber"))
                        {
                            var userSerialNumber = serialNumberSettings.GetValue("UserSerialNumber");
                            if (userSerialNumber.ToUpper().Trim().Equals("NONE"))
                            {
                                isFreeSerialNumber = true;
                            }
                        }

                        sql.Append(dic.CreateUpdateSql(currentDic, formInfo.ConnName, formInfo.TableName, formID, isFreeSerialNumber));
                    }


                    #region 保存子表

                    var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
                    foreach (var item in items)
                    {
                        if (item.ItemType != "SubTable")
                            continue;

                        if (!dic.ContainsKey(item.Code) || string.IsNullOrEmpty(dic[item.Code]))
                            continue;
                        var subTableDataList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic[item.Code]);
                        var autoDelete = true;
                        if (!string.IsNullOrEmpty(GetQueryString("AutoDeleteSubItem_" + item.Code)))
                            Boolean.TryParse(GetQueryString("AutoDeleteSubItem_" + item.Code), out autoDelete);
                        if (autoDelete)
                        {
                            var ids = subTableDataList.Where(c => c.ContainsKey("ID") && !string.IsNullOrEmpty(c["ID"])).Select(c => c["ID"]).ToList();
                            //获取全部子表项ID
                            var oldIds = sqlHelper.ExecuteDataTable(string.Format("select ID from {0}_{1} where {0}ID='{2}'", formInfo.TableName, item.Code, formID))
                               .AsEnumerable().Select(c => c.Field<string>("ID")).ToList();
                            string notExistIDs = string.Join("','", oldIds.Where(c => !ids.Contains(c)).ToArray());
                            //删除已经不存在的ID
                            sql.AppendFormat("\n delete from {0}_{1} where ID in('{2}');", formInfo.TableName, item.Code, notExistIDs);
                        }

                        int index = 0;
                        foreach (var subItem in subTableDataList)
                        {
                            //升版逻辑子表记录ID重新赋值
                            var upperVeriosn = Request["UpperVersion"];
                            if (!string.IsNullOrEmpty(upperVeriosn))
                                subItem["ID"] = "";

                            subItem[formInfo.TableName + "ID"] = formID;//父表ID
                            subItem["SortIndex"] = index++.ToString();
                            sql.Append("\n");
                            this.BeforeSaveDetail(dic, item.Code, subItem, subTableDataList, formInfo);
                            if (subItem.ContainsKey("ID") && !string.IsNullOrEmpty(subItem["ID"]))
                            {
                                sql.Append(subItem.CreateUpdateSql(formInfo.ConnName, formInfo.TableName + "_" + item.Code, subItem["ID"]));
                            }
                            else
                            {
                                subItem["ID"] = FormulaHelper.CreateGuid();
                                sql.Append(subItem.CreateInsertSql(formInfo.ConnName, formInfo.TableName + "_" + item.Code, subItem["ID"]));
                            }

                        }
                        this.BeforeSaveSubTable(dic, item.Code, subTableDataList, formInfo);
                    }


                    #endregion

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

                    this.AfterSave(dic, formInfo, isNew);
                    formDataDT = GetModelDT(formID);
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

                return Json(JsonHelper.ToJson(formDataDT).Trim('[', ']'));
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
                        info = string.Format("【{0}】{1}<br>", etyName, item2.ErrorMessage);
                    }
                }
                throw new DbEntityValidationException(info, ex);
            }
            catch (EntityCommandExecutionException ex)
            {
                string info = "错误：" + ex.Message;
                if (ex.InnerException != null)
                    info += "<br>详细：" + ex.InnerException.Message;
                throw new EntityCommandExecutionException(info, ex);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                throw ex;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
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

        #region 流程相关

        public virtual void UnExecTaskExec(string taskExecID)
        {
            FlowFO flowFO = new FlowFO();
            flowFO.UnExecTask(taskExecID);
        }

        public virtual bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            FlowFO flowFO = new FlowFO();
            bool flag = false;
            try
            {
                flag = flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);
                //流程表单定义的流程逻辑
                ExecFlowLogic(routing.Code, taskExec.S_WF_InsFlow.FormInstanceID, "");
                SetFormFlowInfo(taskExec.S_WF_InsFlow);
                if (routing != null)
                {
                    routing.ExeLogic();
                    this.entities.SaveChanges();
                }
                return flag;
            }
            catch (FlowException ex)
            {
                throw new FlowException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        public virtual bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, string code)
        {
            FlowFO flowFO = new FlowFO();
            bool flag = false;
            try
            {
                Action submitAction = () =>
                {
                    flag = flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);
                    //流程表单定义的流程逻辑
                    ExecFlowLogic(routing.Code, taskExec.S_WF_InsFlow.FormInstanceID, code);
                    SetFormFlowInfo(taskExec.S_WF_InsFlow);
                    if (routing != null)
                    {
                        routing.ExeLogic();
                        this.entities.SaveChanges();
                    }
                };

                if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
                {
                    TransactionOptions tranOp = new TransactionOptions();
                    tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                    {
                        submitAction();
                        ts.Complete();
                    }
                }
                else
                {
                    submitAction();
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        protected virtual string GetRoutingIconCls(string routingCode)
        {
            return flowService.GetRoutingIconCls(routingCode);
        }

        #region Delete方法

        [ValidateInput(false)]
        public virtual JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
                this.BeforeDelete(Request["ListIDs"].Split(','));
            else if (!String.IsNullOrEmpty(Request["ID"]))
                this.BeforeDelete(Request["ID"].Split(','));
            else if (!String.IsNullOrEmpty(Request["TaskExecID"]))
            {
                var taskExecID = Request["TaskExecID"].ToString();
                S_WF_InsFlow flow = FormulaHelper.GetEntities<WorkflowEntities>().S_WF_InsTaskExec
                    .Where(c => c.ID == taskExecID).SingleOrDefault().S_WF_InsTask.S_WF_InsFlow;
                if (flow != null && !String.IsNullOrEmpty(flow.FormInstanceID))
                    this.BeforeDelete(flow.FormInstanceID.Split(','));
            }
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);

            if (BusinessEntities != null)
                BusinessEntities.SaveChanges();

            return Json("");
        }

        #endregion

        #region 委托、传阅、加签

        #region DelegateTask

        [ValidateInput(false)]
        public virtual JsonResult DelegateTask()
        {
            flowService.DelegateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region AskTask

        [ValidateInput(false)]
        public virtual JsonResult AskTask()
        {
            flowService.AskTask(Request["TaskExecID"], Request["NextExecUserIDs"], Request["ExecComment"]);
            return Json("");
        }

        #endregion

        #region WithdrawAskTask

        [ValidateInput(false)]
        public virtual JsonResult WithdrawAskTask()
        {
            flowService.WithdrawAskTask(Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region CirculateTask

        [ValidateInput(false)]
        public virtual JsonResult CirculateTask()
        {
            flowService.CirculateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region ViewTask

        [ValidateInput(false)]
        public virtual JsonResult ViewTask()
        {
            flowService.ViewTask(Request["TaskExecID"], Request["ExecComment"]);
            return Json("");
        }


        #endregion

        #endregion

        #region DoBack

        [ValidateInput(false)]
        public virtual JsonResult DoBack(string taskExecID, string routingID, string execComment)
        {
            flowService.DoBack(taskExecID, routingID, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirst

        [ValidateInput(false)]
        public virtual JsonResult DoBackFirst(string taskExecId, string execComment)
        {
            flowService.DoBackFirst(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirstReturn

        [ValidateInput(false)]
        public virtual JsonResult DoBackFirstReturn(string taskExecId, string execComment)
        {
            flowService.DoBackFirstReturn(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region Submit

        [ValidateInput(false)]
        public virtual JsonResult Submit()
        {
            string id = flowService.Submit(GetQueryString("ID"), Request["RoutingID"], Request["TaskExecID"], Request["NextExecUserIDs"], Request["NextExecUserIDsGroup"], Request["nextExecRoleIDs"], Request["nextExecOrgIDs"], Request["ExecComment"], Request["AutoPass"] == "True");
            var dic = flowService.GetAutoSubmitParam(id, Request["RoutingID"], Request["TaskExecID"], Request["NextExecUserIDs"]);
            dic.Add("ID", id);
            return Json(dic);
        }

        #endregion

        #region DeleteFlow 其实为撤销方法
        [ValidateInput(false)]
        public virtual JsonResult DeleteFlow()
        {
            flowService.DeleteFlow(GetQueryString("ID"), Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region 发起人自由撤销流程

        [ValidateInput(false)]
        public virtual JsonResult FreeWidthdraw()
        {
            flowService.FreeWidthdraw(GetQueryString("ID"));
            return Json("");
        }

        #endregion

        #region GetFormControlInfo
        /// <summary>
        /// 获取表单控制信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFormControlInfo()
        {
            var dic = flowService.GetFormControlInfo(Request["TaskExecID"]);
            return Json(dic);
        }

        #endregion

        #region GetFlowButtons
        [ValidateInput(false)]
        public virtual JsonResult GetFlowButtons()
        {
            var btnList = flowService.JsonGetFlowButtons(Request["ID"], Request["TaskExecID"]);
            return Json(btnList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetUser

        public string GetUserIDs(string roleIDs, string orgIDs, string excludeUserIDs)
        {
            return flowService.GetUserIDs(roleIDs, orgIDs, excludeUserIDs);
        }

        public string UserNames(string userIDs)
        {
            return flowService.UserNames(userIDs);
        }

        #endregion

        public JsonResult GetBusLeftTaskList()
        {
            var obj = flowService.GetBusLeftTaskList(Request["ID"], Request["TaskExecID"]);
            return Json(obj);
        }

        #endregion

        #region 私有方法

        #region 获取表单数据

        public DataTable GetModelDT(string id, bool isUpVersion = false, string taskExecID = "", string code = "")
        {
            flowService.SetTaskFirstViewTime(string.IsNullOrEmpty(taskExecID) ? Request["TaskExecID"] : taskExecID);

            var uiFO = FormulaHelper.CreateFO<UIFO>();

            string tmplCode = string.IsNullOrEmpty(code) ? Request["TmplCode"] : code;
            var formInfo = baseEntities.Set<S_UI_Form>().Where(c => c.Code == tmplCode).OrderByDescending(c => c.ID).FirstOrDefault(); //获取最新一个版本即可
            if (formInfo == null)
                throw new Exception("表单编号为" + tmplCode + "不存在!");

            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);

            string sql = string.Format("select * from {0} where ID='{1}'", formInfo.TableName, id);


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            //获取数据后处理加密字段
            if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
            {
                foreach (var item in items)
                {
                    if (item.FieldType == "varbinary(500)")
                    {
                        //sbFields.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0})) ", item.Code);
                        string _sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
";
                        _sql += string.Format("select convert(nvarchar(500), DecryptByKey({1})) from {0} where ID='{2}'", formInfo.TableName, item.Code, id);
                        var obj = sqlHelper.ExecuteScalar(_sql);
                        if (dt.Rows.Count > 0)
                        {
                            dt.Columns.Remove(item.Code);
                            dt.Columns.Add(item.Code);
                            dt.Rows[0][item.Code] = obj.ToString();
                        }
                    }
                }
            }

            Dictionary<string, DataTable> defaultValueRows = null;// uiFO.GetDefaultValueDic(formInfo.DefaultValueSettings);
            if (dt.Rows.Count == 0)
                defaultValueRows = uiFO.GetDefaultValueDic(formInfo.DefaultValueSettings);
            else
                defaultValueRows = uiFO.GetDefaultValueDic(formInfo.DefaultValueSettings, dt.Rows[0]);

            #region 数据源到前台（开始叫“默认值”）2015-7-21
            StringBuilder sb = new StringBuilder();
            foreach (var key in defaultValueRows.Keys)
            {
                var guid = new Guid();
                if (Guid.TryParse(key, out guid) == false)
                    sb.AppendFormat("\n var {0}={1}", key, JsonHelper.ToJson(defaultValueRows[key]));
            }
            ViewBag.DataSource = sb.ToString();
            #endregion

            if (dt.Rows.Count == 0 || isUpVersion)
            {
                #region 新对象默认值

                DataRow row = null;
                if (dt.Rows.Count == 0)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(row);
                }
                else
                    row = dt.Rows[0];

                row["ID"] = string.IsNullOrEmpty(id) ? FormulaHelper.CreateGuid() : id;

                //包含默认值设置则初始化默认值
                items.Where(c => !string.IsNullOrEmpty(c.DefaultValue)).ToList().ForEach(d =>
                {
                    if (d.ItemType == "SubTable")
                    {
                        //子表
                        if (!dt.Columns.Contains(d.Code))
                            dt.Columns.Add(d.Code);

                        row[d.Code] = uiFO.GetDefaultValue(d.Code, d.DefaultValue, defaultValueRows);
                    }
                    else if (!dt.Columns.Contains(d.Code))
                        return;
                    else if ((d.ItemType == "ButtonEdit" || d.ItemType == "ComboBox") && d.DefaultValue.Split(',').Count() == 2)
                    {
                        //键值控件
                        string v1 = uiFO.GetDefaultValue(d.Code, d.DefaultValue.Split(',')[0], defaultValueRows);
                        string v2 = uiFO.GetDefaultValue(d.Code, d.DefaultValue.Split(',')[1], defaultValueRows);
                        if (!string.IsNullOrEmpty(v1) && v1.Contains('{') == false)
                            row[d.Code] = v1;
                        if (!string.IsNullOrEmpty(v2) && v2.Contains('{') == false)
                        {
                            var fieldName = "";
                            if (Config.Constant.IsOracleDb)
                            {
                                fieldName = d.Code + "NAME";
                                if (!String.IsNullOrWhiteSpace(d.Settings))
                                {
                                    var settings = JsonHelper.ToObject(d.Settings);
                                    var txtName = settings.GetValue("textName");
                                    if (!String.IsNullOrEmpty(txtName))
                                    {
                                        fieldName = txtName;
                                    }
                                }
                            }
                            else
                            {
                                fieldName = d.Code + "Name";
                                if (!String.IsNullOrWhiteSpace(d.Settings))
                                {
                                    var settings = JsonHelper.ToObject(d.Settings);
                                    var txtName = settings.GetValue("textName");
                                    if (!String.IsNullOrEmpty(txtName))
                                    {
                                        fieldName = txtName;
                                    }
                                }
                            }
                            row[fieldName] = v2;
                        }
                    }
                    else
                    {
                        //单值控件
                        string v = uiFO.GetDefaultValue(d.Code, d.DefaultValue, defaultValueRows);
                        if (!string.IsNullOrEmpty(v) && v.Contains('{') == false)
                            row[d.Code] = v;
                    }
                });

                #endregion

                #region 升版子表无默认值数据取历史版本
                foreach (var item in items)
                {
                    if (item.ItemType != "SubTable")
                        continue;

                    if (!dt.Columns.Contains(item.Code))
                        dt.Columns.Add(item.Code);
                    if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0][item.Code])))
                    {
                        sql = string.Format("select * from {0}_{1} where {0}ID='{2}' order by SortIndex", formInfo.TableName, item.Code, id);
                        DataTable dtSubTable = sqlHelper.ExecuteDataTable(sql);
                        if (dtSubTable.Rows.Count > 0)
                            dt.Rows[0][item.Code] = JsonHelper.ToJson(dtSubTable);
                    }
                }

                #endregion

                //设置默认流水号
                if (dt.Columns.Contains("SerialNumber") && !string.IsNullOrEmpty(formInfo.SerialNumberSettings) && !isUpVersion)
                {
                    var serialNumberDic = JsonHelper.ToObject(formInfo.SerialNumberSettings);
                    string userMode = "ONLYGET";//默认值兼容老项目没有配置该选项
                    if (serialNumberDic.ContainsKey("UserSerialNumber"))
                        userMode = serialNumberDic["UserSerialNumber"] == null ? "" : serialNumberDic["UserSerialNumber"].ToString().Trim().ToUpper();

                    if (userMode != "NONE" && userMode != "FLOWEND")
                    {
                        bool isAutoIncrease = userMode.Equals("ONLYGET");

                        if (Config.Constant.IsOracleDb)
                            row["SERIALNUMBER"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, isAutoIncrease, row, null);
                        else
                            row["SerialNumber"] = GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, isAutoIncrease, row, null);
                    }
                }
            }
            else //获取子表数据
            {
                #region 获取子表数据
                foreach (var item in items)
                {
                    if (item.ItemType != "SubTable")
                        continue;

                    //表单子表支持加密字段
                    if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                    {
                        var subTableSettings = JsonHelper.ToObject(item.Settings);
                        var subTableItems = JsonHelper.ToObject<List<FormItem>>(subTableSettings["listData"].ToString());
                        StringBuilder sbSubTableFields = new StringBuilder();
                        sbSubTableFields.AppendFormat("ID");
                        foreach (var subItem in subTableItems)
                        {
                            if (subItem.FieldType == "")
                            {
                            }
                            else if (subItem.FieldType == "varbinary(500)")
                            {
                                sbSubTableFields.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0})) ", subItem.Code);
                            }
                            else
                            {
                                sbSubTableFields.Append("," + subItem.Code);
                            }

                            if (subItem.ItemType == "ButtonEdit")
                            {
                                sbSubTableFields.Append("," + subItem.Code + "Name");
                            }
                            else if (subItem.ItemType == "ComboBox" && !String.IsNullOrEmpty(subItem.Settings))
                            {
                                var subSetting = JsonHelper.ToObject(subItem.Settings);
                                if (!String.IsNullOrEmpty(subSetting.GetValue("textName")))
                                    sbSubTableFields.Append("," + subSetting.GetValue("textName"));
                            }
                        }

                        sql = string.Format("select {3} from {0}_{1} where {0}ID='{2}' order by SortIndex", formInfo.TableName, item.Code, id, sbSubTableFields);

                        //子表支持加密
                        if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                        {
                            sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
" + sql;
                        }
                    }
                    else
                    {
                        sql = string.Format("select * from {0}_{1} where {0}ID='{2}' order by SortIndex", formInfo.TableName, item.Code, id);
                    }



                    DataTable dtSubTable = sqlHelper.ExecuteDataTable(sql);

                    if (!dt.Columns.Contains(item.Code))
                        dt.Columns.Add(item.Code);
                    dt.Rows[0][item.Code] = JsonHelper.ToJson(dtSubTable);
                }

                #endregion
            }

            //计算值(增加表单表没有的字段) 2015-6-16 新增功能
            foreach (var item in items)
            {
                if (dt.Columns.Contains(item.Code))
                    continue;
                if (string.IsNullOrEmpty(item.DefaultValue))
                    continue;

                dt.Columns.Add(item.Code);

                string v = uiFO.GetDefaultValue(item.Code, item.DefaultValue, defaultValueRows);
                if (!string.IsNullOrEmpty(v) && v.Contains('{') == false)
                    dt.Rows[0][item.Code] = v;
            }

            return dt;
        }

        #endregion

        #region 获取流水号

        protected string GetSerialNumber(string formCode, string SerialNumberSettings, bool applySerialNumber, DataRow row = null, Dictionary<string, string> dic = null)
        {
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();

            var serialNumberDic = JsonHelper.ToObject(SerialNumberSettings);
            string tmpl = serialNumberDic["Tmpl"].ToString();
            string resetRule = serialNumberDic["ResetRule"].ToString();
            string CategoryCode = "";
            string SubCategoryCode = "";
            string OrderNumCode = "";
            string PrjCode = "";
            string OrgCode = "";
            string UserCode = "";

            if (serialNumberDic.ContainsKey("CategoryCode"))
                CategoryCode = uiFO.ReplaceString(serialNumberDic["CategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("SubCategoryCode"))
                SubCategoryCode = uiFO.ReplaceString(serialNumberDic["SubCategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrderNumCode"))
                OrderNumCode = uiFO.ReplaceString(serialNumberDic["OrderNumCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("PrjCode"))
                PrjCode = uiFO.ReplaceString(serialNumberDic["PrjCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrgCode"))
                OrgCode = uiFO.ReplaceString(serialNumberDic["OrgCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("UserCode"))
                UserCode = uiFO.ReplaceString(serialNumberDic["UserCode"].ToString(), row, dic);

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

            string SerialNumber = SerialNumberHelper.GetSerialNumberString(tmpl, param, resetRule, applySerialNumber);

            return SerialNumber;
        }

        #endregion

        #region 执行流程逻辑

        protected void ExecFlowLogic(string routingCode, string id, string code)
        {
            try
            {
                var uiFO = FormulaHelper.CreateFO<UIFO>();
                string tmplCode = string.IsNullOrEmpty(code) ? Request["TmplCode"] : code;
                if (string.IsNullOrEmpty(tmplCode))
                    throw new FlowException(string.Format("表单编号{0}不存在!", tmplCode));
                var formInfo = UIFO.GetFormDef(tmplCode, id);
                var logicList = JsonHelper.ToObject<List<Dictionary<string, string>>>(formInfo.FlowLogic ?? "[]");

                foreach (var logic in logicList.Where(c => c["RoutingCode"] == routingCode))
                {
                    string sql = string.Format("select * from {0} where ID='{1}'", formInfo.TableName, id);
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                    DataTable dt = sqlHelper.ExecuteDataTable(sql);

                    var dic = JsonHelper.ToObject<Dictionary<string, string>>(HttpContext.Request["formData"]);
                    sql = uiFO.ReplaceString(logic["SQL"], dt.Rows[0], dic);
                    sqlHelper = SQLHelper.CreateSqlHelper(logic["ConnName"]);
                    sqlHelper.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion

        #region 校验唯一性

        protected void ValidateUnique(S_UI_Form formInfo, Dictionary<string, string> dic, string formID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            string sql = string.Format("select count(1) from {0} where ID!='{1}'", formInfo.TableName, formID);
            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);

            if (formInfo.ValidateUnique == "single")
            {
                foreach (var item in items)
                {
                    if (!dic.ContainsKey(item.Code))
                        continue;
                    string value = dic[item.Code].Replace("'", "''");//查询条件的单引号
                    if (item.Unique == "true")
                        if (sqlHelper.ExecuteScalar(sql + string.Format(" and {0}='{1}'", item.Code, value)).ToString() != "0")
                            throw new BusinessException(string.Format("{0}有重复", item.Name));
                }
            }
            else
            {
                string fieldName = "";
                foreach (var item in items)
                {
                    if (!dic.ContainsKey(item.Code))
                        continue;

                    if (string.IsNullOrEmpty(item.Unique))
                        continue;

                    if (item.Unique == "true")
                    {
                        string value = dic[item.Code].Replace("'", "''");//查询条件的单引号
                        sql += string.Format(" and {0}='{1}'", item.Code, value);
                        fieldName += item.Name + ",";
                    }
                }
                if (fieldName != "")
                {
                    var obj = sqlHelper.ExecuteScalar(sql);
                    if (obj.ToString() != "0")
                        throw new BusinessException(string.Format("{0}有重复", fieldName.Trim(',').Replace(",", "和")));
                    //throw new BusinessException(string.Format("唯一性校验失败：{0}", fieldName.Trim(',')));
                }
            }

            //string fieldName = "";
            //foreach (var item in items)
            //{
            //    if (!dic.ContainsKey(item.Code))
            //        continue;
            //    if (item.ItemType == "SubTable")
            //    {
            //        if (!string.IsNullOrEmpty(item.Settings))
            //        {
            //            var _dic = JsonHelper.ToObject(item.Settings);
            //            var subList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());

            //            if (subList.Where(c => c.Unique == "true").Count() > 0)
            //            {
            //                var dataList = JsonHelper.ToList(dic[item.Code]);
            //                List<string> tmpList = new List<string>();
            //                foreach (var c in dataList)
            //                    tmpList.Add("");
            //                string subFieldName = "";
            //                foreach (var subItem in subList.Where(c => c.Unique == "true"))
            //                {
            //                    for (int i = 0; i < dataList.Count; i++)
            //                    {
            //                        tmpList[i] += "_" + (!dataList[i].ContainsKey(subItem.Code) ? "" : dataList[i][subItem.Code].ToString());
            //                    }
            //                    subFieldName += subItem.Name + ",";
            //                }

            //                if (tmpList.Distinct().Count() < tmpList.Count)
            //                    throw new BusinessException(string.Format("{0}有重复", subFieldName.Trim(',')));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (string.IsNullOrEmpty(item.Unique))
            //            continue;

            //        if (item.Unique == "true")
            //        {
            //            string value = dic[item.Code].Replace("'", "''");//查询条件的单引号
            //            if (formInfo.ValidateUnique == "single")
            //            {
            //                //sql += string.Format(" or {0}='{1}'", item.Code, value);
            //                if (sqlHelper.ExecuteScalar(sql + string.Format(" and {0}='{1}'", item.Code, value)).ToString() != "0")
            //                    throw new BusinessException(string.Format("{0}有重复", item.Name));

            //            }
            //            else
            //                sql += string.Format(" and {0}='{1}'", item.Code, value);
            //            fieldName += item.Name + ",";
            //        }
            //    }
            //}
            //if (fieldName != "")
            //{
            //    var obj = sqlHelper.ExecuteScalar(sql);
            //    if (obj.ToString() != "0")
            //        throw new BusinessException(string.Format("{0}有重复", fieldName.Trim(',').Replace(",", "或")));
            //    //throw new BusinessException(string.Format("唯一性校验失败：{0}", fieldName.Trim(',')));

            //}
        }



        #endregion

        #region entities

        protected DbContext baseEntities
        {
            get
            {
                return FormulaHelper.GetEntities<BaseEntities>();
            }
        }

        #endregion

        public void SetFormFlowInfo(S_WF_InsFlow flowIns)
        {
            try
            {
                if (flowIns == null) throw new Exception("找不到流程实例信息，无法更新流程信息");
                string connName = flowIns.S_WF_InsDefFlow.ConnName;
                string tableName = flowIns.S_WF_InsDefFlow.TableName;
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
                SQLHelper flowHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                string sql = string.Format("select * from {0} where ID='{1}'", tableName, flowIns.FormInstanceID);
                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0 && (dt.Columns.Contains("FlowInfo") || dt.Columns.Contains("FLOWINFO")))
                {
                    var flowInfo = new Dictionary<string, object>();
                    sql = @"select S_WF_InsTask.ID,InsDefStepID,InsFlowID,S_WF_InsDefStep.Code,S_WF_InsDefStep.Name as StepName,TaskUserIDs,TaskUserNames,CreateTime as StartDate,
CompleteTime  as EndDate,Status from S_WF_InsTask 
left join S_WF_InsDefStep on S_WF_InsTask.InsDefStepID=S_WF_InsDefStep.ID
where InsFlowID = '{0}' order by S_WF_InsTask.CreateTime";
                    var detialInfo = flowHelper.ExecuteDataTable(String.Format(sql, flowIns.ID));
                    flowInfo["DetailInfo"] = detialInfo;
                    flowInfo["StartDate"] = flowIns.CreateTime;
                    flowInfo["EndDate"] = flowIns.CompleteTime;
                    flowInfo["StartUser"] = flowIns.CreateUserName;
                    flowInfo["StartUserID"] = flowIns.CreateUserID;
                    flowInfo["TimeConsuming"] = flowIns.TimeConsuming;
                    if (detialInfo.Rows.Count > 0)
                    {
                        flowInfo["CurrentStepUser"] = detialInfo.Rows[detialInfo.Rows.Count - 1]["TaskUserNames"];
                        flowInfo["CurrentStepUserID"] = detialInfo.Rows[detialInfo.Rows.Count - 1]["TaskUserIDs"];
                    }
                    sql = string.Format(" update {0} set FlowInfo='{1}' where ID='{2}';", tableName, JsonHelper.ToJson(flowInfo), flowIns.FormInstanceID);
                    if (Config.Constant.IsOracleDb)
                    {
                        sql = string.Format(@"
begin
{0}
end;
", sql);
                    }
                    sqlHelper.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 接口实现

        public void SetFormFlowInfo(string id, string flowInfo)
        {
            try
            {
                string tmplCode = Request["TmplCode"];
                var formInfo = baseEntities.Set<S_UI_Form>().FirstOrDefault(c => c.Code == tmplCode);
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                string sql = string.Format("select * from {0} where 1=2", formInfo.TableName);
                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                sql = "";
                if (dt.Columns.Contains("FlowInfo"))
                {
                    sql += string.Format(" update {0} set FlowInfo='{1}' where ID='{2}';", formInfo.TableName, flowInfo, id);
                    if (Config.Constant.IsOracleDb)
                    {
                        sql = string.Format(@"
begin
{0}
end;
", sql);
                    }
                    sqlHelper.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        public void SetFormFlowPhase(string id, string flowPhase, string stepName)
        {
            try
            {
                string tmplCode = Request["TmplCode"];
                var formInfo = baseEntities.Set<S_UI_Form>().FirstOrDefault(c => c.Code == tmplCode);
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);

                string sql = string.Format("select * from {0} where 1=2", formInfo.TableName);
                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                sql = "";
                if (dt.Columns.Contains("FlowPhase"))
                    sql += string.Format(" update {0} set FlowPhase='{1}' where ID='{2}';", formInfo.TableName, flowPhase, id);
                if (dt.Columns.Contains("StepName"))
                    sql += string.Format(" update {0} set StepName='{1}' where ID='{2}';", formInfo.TableName, stepName, id);
                if (Config.Constant.IsOracleDb)
                {
                    sql = string.Format(@"
begin
{0}
end;
", sql);
                }
                sqlHelper.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        public void DeleteForm(string ids)
        {
            try
            {
                string tmplCode = Request["TmplCode"];
                var form = baseEntities.Set<S_UI_Form>().FirstOrDefault(c => c.Code == tmplCode);
                if (form == null)
                    throw new Formula.Exceptions.BusinessException("未找到编号为【" + tmplCode + "】的表单定义");
                SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(form.ConnName);
                string sql = string.Format("delete from {0} where ID in('{1}')", form.TableName, ids.Replace(",", "','"));
                sqlHeler.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        public bool ExistForm(string id)
        {
            object obj = null;
            try
            {
                string tmplCode = Request["TmplCode"];
                var form = baseEntities.Set<S_UI_Form>().FirstOrDefault(c => c.Code == tmplCode);
                SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(form.ConnName);
                string sql = string.Format("select count(1) from {0} where ID ='{1}'", form.TableName, id);
                obj = sqlHeler.ExecuteScalar(sql);
                return Convert.ToInt32(obj) > 0;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion

        #region 对象方法重载

        protected virtual TEntity GetEntityByID<TEntity>(string ID, bool fromDB = false) where TEntity : class, new()
        {
            var spec = new Specifications();
            spec.AndAlso("ID", ID, QueryMethod.Equal);
            var result = this.BusinessEntities.Set<TEntity>().FirstOrDefault(spec.GetExpression<TEntity>());
            if (fromDB && result != null)
            {
                var propValues = this.BusinessEntities.Entry<TEntity>(result).GetDatabaseValues();
                return (TEntity)propValues.ToObject();
            }
            return result;
        }

        public override TEntity UpdateEntityFromJson<TEntity>(string formJson)
        {
            if (BusinessEntities == null)
                throw new Exception("没有初始化dbcontext上下文!");

            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(formJson);

            string id = formDic["ID"].ToString();
            var entity = this.BusinessEntities.Set<TEntity>().Find(id);
            if (entity == null)
            {
                entity = new TEntity { };
                PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                pi.SetValue(entity, id, null);
                EntityCreateLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity, formDic);
                this.BusinessEntities.Set<TEntity>().Add(entity);
            }
            else
            {
                EntityModifyLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity, formDic);
            }

            TryValidateModel(entity);

            return entity;
        }
        public override TEntity UpdateEntity<TEntity>(string id)
        {
            if (BusinessEntities == null)
                throw new Exception("没有初始化dbcontext上下文!");

            var entity = this.BusinessEntities.Set<TEntity>().Find(id);
            if (entity == null)
            {
                if (string.IsNullOrEmpty(id))
                    id = FormulaHelper.CreateGuid();
                entity = new TEntity { };
                PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                pi.SetValue(entity, id, null);

                //设置对象状态为添加
                pi = typeof(TEntity).GetProperty("_state");
                pi.SetValue(entity, EntityStatus.added.ToString(), null);

                // 设置SortIndex 字段的默认值
                SetSortIndexDefaultValue(entity);
                EntityCreateLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity);
                //pi.SetValue(entity, id, null); //ID被UpdateEntity冲掉，重新赋值
                BusinessEntities.Set<TEntity>().Add(entity);
            }
            else
            {
                PropertyInfo pi = typeof(TEntity).GetProperty("_state");
                //设置对象状态为修改
                pi = typeof(TEntity).GetProperty("_state");
                pi.SetValue(entity, EntityStatus.modified.ToString(), null);

                // 设置SortIndex 字段的默认值
                SetSortIndexDefaultValue(entity);
                EntityModifyLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity);
            }

            TryValidateModel(entity);

            return entity;
            //return base.UpdateEntity<TEntity>(id);
        }

        /// <summary>
        /// 设置SortIndex 字段的默认值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        private void SetSortIndexDefaultValue<TEntity>(TEntity entity) where TEntity : class, new()
        {
            var pi = typeof(TEntity).GetProperty("SortIndex");
            if (pi != null && pi.GetValue(entity, null) == null)
            {
                if (pi.PropertyType.FullName.IndexOf("Double") >= 0)
                {
                    pi.SetValue(entity, 0.0, null);
                }
                else
                {
                    pi.SetValue(entity, 0, null);
                }
            }
        }

        /// <summary>
        /// 设置datatable的虚拟列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void SetDtValue(DataTable dt, string key, object value)
        {
            if (dt.Rows.Count > 0)
            {
                if (!dt.Columns.Contains(key))
                    dt.Columns.Add(key);

                dt.Rows[0][key] = value;
            }
        }

        #endregion

        public JsonResult GetTaskExec(string taskexecID)
        {
            return Json(entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskexecID));
        }

        #region 批量审批表单验证

        public bool CheckBatchApproval(string insDefStepID, DataTable table)
        {
            bool result = false;
            try
            {
                var defRouting = entities.Set<S_WF_InsDefRouting>().Where(c => c.InsDefStepID == insDefStepID && !string.IsNullOrEmpty(c.NotNullFields));
                if (defRouting.Count() == 0)
                    return true;
                if (defRouting.First().NotNullFields.Length > 0)
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        List<string> list = new List<string>();
                        foreach (var item in defRouting.First().NotNullFields.Split(','))
                        {
                            foreach (DataRow dr in table.Rows)
                            {
                                if (!string.IsNullOrEmpty(dr[item].ToString()))
                                {
                                    list.Add(item);
                                }
                            }
                        }
                        if (list.Count == defRouting.First().NotNullFields.Split(',').Length)
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }


        }

        public Dictionary<string, string> SubmitBatchApproval(string ids, string routingID, string taskExecID, string nextExecUserIDs, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, string tmplCode)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                //批量审批一定不能做表单的保存动作，否则会发生异常，因为没有HTTP上下文内容，获取不到FormData数据 by Eric.Yang 2019-4-23
                string id = flowService.Submit(ids, routingID, taskExecID, nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs,
                    execComment, tmplCode, true);
                dic = flowService.GetAutoSubmitParam(ids, routingID, taskExecID, nextExecUserIDs);
                if (dic.Count > 0)
                {
                    dic.Add("ID", id);
                }
                return dic;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }
        #endregion

        #region 表单打印

        public ActionResult FormPrint()
        {
            var uiFO = FormulaHelper.CreateFO<UIFO>();
            ViewBag.FormPrintHtml = uiFO.FormToPrintHtml(Request["tmplCode"], Request["ID"]);
            ViewBag.webPrintLogo = System.Configuration.ConfigurationManager.AppSettings["webPrintLogo"];
            ViewBag.webPrintHeader = System.Configuration.ConfigurationManager.AppSettings["webPrintHeader"];
            return View();
        }

        #endregion
    }

}
