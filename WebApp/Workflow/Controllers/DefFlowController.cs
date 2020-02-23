using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Config;
using System.Data;
using Workflow.Logic.Domain;
using Workflow.Logic.BusinessFacade;
using Workflow;
using MvcAdapter;
using Formula.Helper;
using System.IO;
using System.Text;
using Workflow.Logic;
using System.Xml;
using System.Collections;

namespace WorkFlow.Controllers
{
    public class DefFlowController : Workflow.BaseController<S_WF_DefFlow>
    {
        public override ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public JsonResult GetRelationList(MvcAdapter.QueryBuilder qb)
        {
            //流程定义增加子公司权限
            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"].ToLower() == "true"
                 && Request["listType"] == "subCompany")
                qb.Add("CompanyID", QueryMethod.Like, FormulaHelper.GetUserInfo().UserCompanyID);

            GridData data;
            string categoryID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(categoryID))
            {
                data = new GridData(entities.Set<S_WF_DefFlow>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, TableName = c.TableName, Description = c.Description, AlreadyReleased = c.AlreadyReleased, ModifyTime = c.ModifyTime }));
                data.total = qb.TotolCount;
            }
            else
            {
                categoryID = categoryID.Split('.').Last();
                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                string sql = string.Format("select ID from S_M_Category where ID='{0}' or ParentID='{0}'", categoryID);
                var dt = baseSqlHelper.ExecuteDataTable(sql);
                string ids = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
                qb.Add("CategoryID", QueryMethod.In, ids);
                data = new GridData(entities.Set<S_WF_DefFlow>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, TableName = c.TableName, Description = c.Description, AlreadyReleased = c.AlreadyReleased, ModifyTime = c.ModifyTime }));
                data.total = qb.TotolCount;
            }


            return Json(data);
        }

        public JsonResult UnReleaseFlow(string id)
        {
            var defFlow = entities.Set<S_WF_DefFlow>().Where(c => c.ID == id).SingleOrDefault();
            defFlow.AlreadyReleased = "0";
            entities.SaveChanges();

            return Json(new { AlreadyReleased = defFlow.AlreadyReleased });
        }

        public override JsonResult Delete()
        {
            if (Config.Constant.IsOracleDb)
            {
                var listIDs = Request["ListIDs"].Split(',');
                var list = entities.Set<S_WF_DefFlow>().Where(c => listIDs.Contains(c.ID)).ToList();
                foreach (var def in list)
                {
                    entities.Set<S_WF_DefFlow>().Remove(def);
                }
                entities.SaveChanges();
                return Json("");
            }
            else
            {
                var listIDs = Request["ListIDs"].Split(',');
                var list = entities.Set<S_WF_DefFlow>().Where(c => listIDs.Contains(c.ID)).ToList();

                foreach (var def in list)
                {
                    entities.Set<S_WF_DefFlow>().Remove(def);
                }
                entities.SaveChanges();
                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                return Json("");
            }
        }

        #region 保存
        [ValidateInput(false)]
        public JsonResult SaveChangedData(string changedData, string layout, string id, bool releaseFlow = false, bool renameRouting = false)
        {
            var def = entities.Set<S_WF_DefFlow>().SingleOrDefault(c => c.ID == id);
            def.ViewConfig = layout;
            if (releaseFlow)
            {
                def.AlreadyReleased = "1";
                def.ModifyTime = DateTime.Now;
            }

            Dictionary<string, Dictionary<string, object>> dic = JsonHelper.ToObject<Dictionary<string, Dictionary<string, object>>>(changedData);
            foreach (var item in dic.Values)
            {
                if (item["Type"].ToString() == "Flow")
                {
                    string categoryID = def.CategoryID;
                    def = UpdateEntityFromJson<S_WF_DefFlow>(item["Data"].ToString());
                    if (entities.Set<S_WF_DefFlow>().SingleOrDefault(c => c.Code == def.Code && c.ID != def.ID) != null)
                        throw new Exception("流程编号不能重复！");
                    //if (categoryID != def.CategoryID)
                    //{
                    //    var obj = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar("select Code from S_M_Category where ID='" + def.CategoryID + "'");
                    //    def.ConnName = obj.ToString();
                    //}

                }
                else if (item["Type"].ToString() == "Step")
                {
                    Dictionary<string, object> itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item["Data"].ToString());
                    string itemID = itemDic["ID"].ToString();
                    var step = entities.Set<S_WF_DefStep>().SingleOrDefault(c => c.ID == itemID);
                    if (step != null)
                    {
                        UpdateEntity<S_WF_DefStep>(step, itemDic);

                        #region 将选择的字段加入到表单控制元素

                        var dataGrid = JsonHelper.ToObject<List<Dictionary<string, string>>>(itemDic["dataGrid"].ToString());

                        string Hidden = string.Join(",", dataGrid.Where(c => c["Hidden"] == "true").Select(c => c["fieldCode"]).ToArray());
                        string Visible = string.Join(",", dataGrid.Where(c => c["Visible"] == "true").Select(c => c["fieldCode"]).ToArray());
                        string Editable = string.Join(",", dataGrid.Where(c => c["Editable"] == "true").Select(c => c["fieldCode"]).ToArray());
                        string Disable = string.Join(",", dataGrid.Where(c => c["Disable"] == "true").Select(c => c["fieldCode"]).ToArray());

                        step.HiddenElements = StringHelper.Include(step.HiddenElements, Hidden);
                        step.VisibleElements = StringHelper.Include(step.VisibleElements, Visible);
                        step.EditableElements = StringHelper.Include(step.EditableElements, Editable);
                        step.DisableElements = StringHelper.Include(step.DisableElements, Disable);
                        #endregion
                    }
                }
                else
                {
                    Dictionary<string, object> itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item["Data"].ToString());
                    string itemID = itemDic["ID"].ToString();
                    var routing = entities.Set<S_WF_DefRouting>().SingleOrDefault(c => c.ID == itemID);
                    if (routing != null)
                    {
                        UpdateEntity<S_WF_DefRouting>(routing, itemDic);

                        #region 处理字段控制表

                        var dataGrid = JsonHelper.ToObject<List<Dictionary<string, string>>>(itemDic["dataGrid"].ToString());
                        var FormDataSet = string.Join(",", dataGrid.Where(c => !string.IsNullOrEmpty(c["FormDataSet"]))
                            .Select(c => c["fieldCode"] + "=" + c["FormDataSet"].Replace(',', '，')).ToArray());
                        routing.FormDataSet = FormDataSet;

                        //必填字段
                        routing.NotNullFields = string.Join(",", dataGrid.Where(c => c["NotNullFields"] == "true").Select(c => c["fieldCode"]).ToArray());
                        //签字字段
                        routing.SignatureField = string.Join(",", dataGrid.Where(c => c["SignatureField"] == "true").Select(c => c["fieldCode"]).ToArray());
                        #endregion
                    }
                }
            }

            if (renameRouting)
            {
                var steps = def.S_WF_DefStep.ToList();
                foreach (var routing in def.S_WF_DefRouting)
                {
                    routing.Name = "送" + steps.SingleOrDefault(c => c.ID == routing.EndID).Name;
                    if (routing.Name == "送结束")
                        routing.Name = "结束";
                }
            }

            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetData(string id, string type)
        {
            if (type == "Flow")
            {
                return Json(entities.Set<S_WF_DefFlow>().SingleOrDefault(c => c.ID == id));
            }
            else if (type == "Step")
            {
                var step = entities.Set<S_WF_DefStep>().SingleOrDefault(c => c.ID == id);

                #region 表单控制元素不显示字段

                var dtFields = GetStepFieldTable(id).AsEnumerable();
                string fields = string.Join(",", dtFields.Select(c => c.Field<string>("fieldCode")));
                step.HiddenElements = StringHelper.Exclude(step.HiddenElements, fields);
                step.VisibleElements = StringHelper.Exclude(step.VisibleElements, fields);
                step.EditableElements = StringHelper.Exclude(step.EditableElements, fields);
                step.DisableElements = StringHelper.Exclude(step.DisableElements, fields);

                #endregion

                return Json(step);
            }
            else
            {
                return Json(entities.Set<S_WF_DefRouting>().SingleOrDefault(c => c.ID == id));
            }
        }

        #endregion

        #region 流程新建克隆

        //创建新流程
        public JsonResult CreateFlowDef()
        {
            var def = UpdateEntity<S_WF_DefFlow>();
            if (entities.Set<S_WF_DefFlow>().SingleOrDefault(c => c.Code == def.Code && c.ID != def.ID) != null)
                throw new Exception("流程编号不能重复！");
            def.AlreadyReleased = "0";
            def.CategoryID = Request["CategoryID"];
            def.ConnName = Request["ConnName"];
            var user = FormulaHelper.GetUserInfo();
            if (def._state == EntityStatus.added.ToString())
            {
                def.CompanyID = user.AdminCompanyID;
                def.CompanyName = user.AdminCompanyName;
            }
            entities.Set<S_WF_DefFlow>().Add(def);
            entities.SaveChanges();
            return Json(new { ID = def.ID, Name = def.Name });
        }

        //克隆流程
        public JsonResult Clone(string id)
        {
            FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
            return Json(new { ID = flowFO.CloneDefFlow(id) });
            //return flowFO.CloneDefFlow(id);
        }

        #endregion

        #region 流程定义属性页

        public ActionResult DefProperty()
        {
            string sql = "select ID,ParentID,FullID,Code,Name,SortIndex,CategoryCode,IconClass from S_M_Category";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            ViewBag.EnumCategory = JsonHelper.ToJson(dt);
            return View();
        }


        #endregion

        #region 环节属性页

        public ActionResult StepProperty(string defFlowID)
        {
            var arr = entities.Set<S_WF_DefSubForm>().Where(c => c.DefFlowID == defFlowID).Select(c => new { value = c.ID, text = c.Name });
            ViewBag.SubForms = JsonHelper.ToJson(arr);
            return View();
        }

        #endregion

        #region 路由属性页


        #endregion

        #region 获取控制字段列表

        public JsonResult GetStepFieldList(string id)
        {
            return Json(GetStepFieldTable(id));
        }

        private DataTable GetStepFieldTable(string id)
        {
            var step = entities.Set<S_WF_DefStep>().SingleOrDefault(c => c.ID == id);
            var def = step.S_WF_DefFlow;

            string connName = def.ConnName;
            string tableName = def.TableName;
            if (!string.IsNullOrEmpty(step.SubFormID))
            {
                var subForm = entities.Set<S_WF_DefSubForm>().SingleOrDefault(c => c.ID == step.SubFormID);
                connName = subForm.ConnName;
                tableName = subForm.TableName;
            }

            DataTable dt = GetFields(connName, tableName);

            var Hidden = step.HiddenElements.Split(',');
            var Visible = step.VisibleElements.Split(',');
            var Editable = step.EditableElements.Split(',');
            var Disable = step.DisableElements.Split(',');
            dt.Columns.Add("Hidden");
            dt.Columns.Add("Visible");
            dt.Columns.Add("Editable");
            dt.Columns.Add("Disable");

            foreach (DataRow row in dt.Rows)
            {
                string fieldCode = row["fieldCode"].ToString();
                if (Hidden.Contains(fieldCode))
                    row["Hidden"] = "true";
                if (Visible.Contains(fieldCode))
                    row["Visible"] = "true";
                if (Editable.Contains(fieldCode))
                    row["Editable"] = "true";
                if (Disable.Contains(fieldCode))
                    row["Disable"] = "true";
            }

            return dt;
        }

        public JsonResult GetRoutingFieldList(string id)
        {
            return Json(GetRoutingFieldTable(id));
        }

        private DataTable GetRoutingFieldTable(string id)
        {
            var routing = entities.Set<S_WF_DefRouting>().SingleOrDefault(c => c.ID == id);
            var def = routing.S_WF_DefFlow;


            string connName = def.ConnName;
            string tableName = def.TableName;
            if (!string.IsNullOrEmpty(routing.S_WF_DefStep.SubFormID))
            {
                var subForm = entities.Set<S_WF_DefSubForm>().SingleOrDefault(c => c.ID == routing.S_WF_DefStep.SubFormID);
                connName = subForm.ConnName;
                tableName = subForm.TableName;
            }

            DataTable dt = GetFields(connName, tableName);

            var NotNullFields = routing.NotNullFields.Split(',');
            var SignatureField = routing.SignatureField.Split(',');
            var FormDataSet = routing.FormDataSet.Split(',');
            dt.Columns.Add("NotNullFields");
            dt.Columns.Add("SignatureField");
            dt.Columns.Add("FormDataSet");

            foreach (DataRow row in dt.Rows)
            {
                string fieldCode = row["fieldCode"].ToString();
                if (NotNullFields.Contains(fieldCode))
                    row["NotNullFields"] = "true";
                if (SignatureField.Contains(fieldCode))
                    row["SignatureField"] = "true";
                var set = FormDataSet.SingleOrDefault(c => c.Split('=')[0].Trim() == fieldCode);
                if (set != null)
                    row["FormDataSet"] = set.Split('=')[1];
            }

            return dt;
        }


        #region 私有方法

        private DataTable GetFields(string connName, string tableName)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);

            string sql = string.Format(@"SELECT  fieldCode= a.name , description= isnull(g.[value],''),fieldType=b.name,sortIndex=a.column_id
FROM  sys.columns a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = a.column_id)
left join systypes b on a.user_type_id=b.xusertype  
WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name = '{0}')", tableName);
            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format(@"select 
                     utc.column_name fieldCode, 
                     data_type fieldType, 
                     data_length fieldLenght,
                     comments as description,
                     0 as sortIndex
                     from USER_TAB_COLS utc join user_col_comments ucc on utc.table_name=ucc.table_name and utc.column_name=ucc.column_name  
                     where utc.table_name = upper('{0}')
                     order by column_id", tableName);
            }

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sql = string.Format(@"select S_M_Field.* from S_M_Field join S_M_Table on TableID=S_M_Table.ID where S_M_Table.Code='{0}' and ConnName='{1}'", tableName, connName);
            if (Config.Constant.IsOracleDb)
                sql = string.Format(@"select S_M_Field.* from S_M_Field join S_M_Table on TableID=S_M_Table.ID where S_M_Table.Code='{0}' and ConnName='{1}'", tableName.ToUpper(), connName);
            var dtFields = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            foreach (DataRow row in dt.Rows)
            {
                string fieldCode = row["fieldCode"].ToString();
                string description = fieldCode;
                var obj = dtFields.SingleOrDefault(c => c.Field<string>("Code").ToUpper() == fieldCode.ToUpper());
                if (obj != null)
                {
                    if (obj["Name"].ToString() != "")
                        description = obj["Name"].ToString();
                }
                row["description"] = description;
            }

            var fields = new string[] { "ID", "CREATETIME", "CREATEDATE", "CREATEUSERID", "CREATEUSER", "CEATEUSERNAME", "MODIFYTIME", "MODIFYDATE", "MODIFYUSER", "MODIFYUSERID", "MODIFYUSERNAME", "ORGID", "PRJID", "FLOWPHASE", "STEPNAME", "ISRELEASED", "SORTINDEX", tableName.ToUpper().Split('_').First() + "ID" };
            foreach (var row in dt.AsEnumerable().Where(c => fields.Contains(c["fieldCode"].ToString().ToUpper())).ToArray())
            {
                dt.Rows.Remove(row);
            }
            return dt;
        }

        #endregion

        #endregion

        #region 子表单

        public JsonResult GetSubFormList(string defFlowID)
        {
            return Json(entities.Set<S_WF_DefSubForm>().Where(c => c.DefFlowID == defFlowID));
        }

        public JsonResult GetSubForm(string id)
        {
            return base.JsonGetModel<S_WF_DefSubForm>(id);
        }

        public JsonResult SaveSubForm()
        {
            return base.JsonSave<S_WF_DefSubForm>();
        }

        public JsonResult DeleteSubForm(string listIDs)
        {
            return base.JsonDelete<S_WF_DefSubForm>(listIDs);
        }

        #endregion

        #region 导出Sql

        public FileResult ExportSql(string defID, string fileCode)
        {
            FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
            string sql = flowFO.exportSql(defID);
            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sql));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", fileCode + ".sql");
        }

        public FileResult ExportSqls(string defIDs)
        {
            FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();

            StringBuilder sb = new StringBuilder();
            foreach (string defID in defIDs.Split(','))
            {
                sb.AppendLine();
                sb.Append(flowFO.exportSql(defID));
            }

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "Workflow.sql");
        }

        #endregion

        public JsonResult SyncDefToInsDef(string defID)
        {
            FlowFO flowFO = new FlowFO();
            flowFO.SyncDefToInsDef(defID);
            return Json("");
        }

        #region 添加节点
        public string NewActivity(string defineID, string name, string type)
        {
            string result = "";
            string guid = FormulaHelper.CreateGuid();
            try
            {
                var flowDefine = entities.Set<S_WF_DefFlow>().Where(c => c.ID == defineID).SingleOrDefault();

                if (flowDefine == null)
                    throw new Exception("指定的工作流定义未找到");

                S_WF_DefStep step = new S_WF_DefStep();
                step.ID = guid.ToString();
                step.Type = type;
                step.Name = name;
                step.NameEN = type == "Completion" ? "End" : type == "Inital" ? "Start" : "Node";
                step.SortIndex = flowDefine.S_WF_DefStep.Count;
                step.DefFlowID = defineID;
                if (type == "Completion")
                    step.Phase = "End";
                else
                    step.Phase = "Processing";
                step.CooperationMode = TaskCooperationMode.Single.ToString();
                step.AllowDoBackFirst = "1";
                step.AllowDoBackFirstReturn = "0";
                entities.Set<S_WF_DefStep>().Add(step);
                entities.SaveChanges();


                result = guid;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);

                result = "Error";
            }
            return result;
        }

        #endregion

        #region 删除节点
        public string DeleteActivity(string acitivtyIDs)
        {
            string result = "";
            try
            {
                var stepIDs = acitivtyIDs.Split(',');
                var steps = entities.Set<S_WF_DefStep>().Where(c => stepIDs.Contains(c.ID)).ToArray();

                var allSteps = steps[0].S_WF_DefFlow.S_WF_DefStep.ToArray();
                foreach (var step in steps)
                {
                    //移除等待环节
                    foreach (var item in allSteps)
                    {
                        item.WaitingStepIDs = StringHelper.Exclude(item.WaitingStepIDs, step.ID);
                    }
                    //删除环节
                    entities.Set<S_WF_DefStep>().Remove(step);
                    //删除路由                    
                    entities.Set<S_WF_DefRouting>().Delete(c => c.EndID == step.ID);
                }
                entities.SaveChanges();
                result = "Ok";
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                result = "Error";
            }
            return result;
        }
        #endregion

        #region 删除路由
        public void DeleteDefRouting(string lineID)
        {
            try
            {
                var defRouting = entities.Set<S_WF_DefRouting>().Where(c => c.ID == lineID).SingleOrDefault();
                entities.Set<S_WF_DefRouting>().Remove(defRouting);
                entities.SaveChanges();
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
            }
        }
        #endregion

        #region 保存流程
        public void SaveFlowLayOut(string defFlowID, string json)
        {
            try
            {
                var flowDef = entities.Set<S_WF_DefFlow>().Where(c => c.ID == defFlowID).SingleOrDefault();
                flowDef.ViewConfig = json;
                entities.SaveChanges();
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
            }
        }
        #endregion

        #region 画线
        public string NewRule(string defineID, string beginActivityID, string endAcitivityID, string name)
        {
            string result = "";
            string guid = FormulaHelper.CreateGuid();
            try
            {
                var flowDef = entities.Set<S_WF_DefFlow>().Where(c => c.ID == defineID).SingleOrDefault();
                if (flowDef == null)
                    throw new Exception("指定的工作流定义未找到");
                var startStep = entities.Set<S_WF_DefStep>().Where(c => c.ID == beginActivityID).SingleOrDefault();
                var endStep = entities.Set<S_WF_DefStep>().Where(c => c.ID == endAcitivityID).SingleOrDefault();
                var routing = new S_WF_DefRouting();
                routing.ID = guid;
                routing.DefStepID = startStep.ID;
                routing.EndID = endStep.ID;
                routing.DefFlowID = defineID;
                routing.SortIndex = startStep.S_WF_DefRouting.Count;
                routing.Type = RoutingType.Normal.ToString();
                routing.Name = "送" + endStep.Name;
                routing.AllowDoBack = "1";//默认允许打回  2014-12-1
                routing.SaveForm = "1"; //默认自动保存表单 2014-12-1
                routing.MustInputComment = "1"; //默认弹出意见框 2014-12-1
                routing.AllowWithdraw = "1"; //默认允许撤销 2014-12-1
                entities.Set<S_WF_DefRouting>().Add(routing);

                if (routing.Name == "送结束")
                {
                    routing.Name = "结束";
                    routing.NameEN = "End";
                    routing.SelectMode = "";
                }

                entities.SaveChanges();

                result = guid;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                result = "Error";
            }
            return result;
        }

        #endregion

        #region 是否存在在开始节点
        public string IsExistBeginNode(string defineID)
        {
            var startStep = entities.Set<S_WF_DefStep>().Where(c => c.DefFlowID == defineID && c.Type == "Inital").ToArray();
            return startStep.Length.ToString();
        }
        #endregion

        #region 升级流程版本
        private string GetflowJson(string viewConfig)
        {
            StringBuilder json = new StringBuilder();
            XmlDocument doc = new XmlDocument();
            Hashtable htNode = new Hashtable();
            if (!string.IsNullOrEmpty(viewConfig))
            {
                doc.LoadXml(viewConfig);
                XmlNode rootNode = doc.SelectSingleNode("WorkFlow").SelectSingleNode("Activitys");
                json.Append("{states:{");
                for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                {
                    XmlElement xElement = (XmlElement)rootNode.ChildNodes[i];
                    string nodeType = "";
                    htNode.Add(xElement.GetAttribute("ID"), "rect" + (i + 1).ToString());
                    switch (xElement.GetAttribute("Type"))
                    {
                        case "Inital":
                            nodeType = "start";
                            break;
                        case "Normal":
                            nodeType = "task";
                            break;
                        case "Completion":
                            nodeType = "end";
                            break;
                    }

                    json.Append(@"" + ("rect" + (i + 1).ToString()) + @": {
                                    type: " + string.Format("'{0}'", nodeType) + @",
                                    ID: " + string.Format("'{0}'", xElement.GetAttribute("ID")) + @",
                                    text: {
                                        text: " + string.Format("'{0}'", xElement.GetAttribute("Name")) + @"},
                                        attr: {x: " + xElement.GetAttribute("PositionX") + ", y: " + xElement.GetAttribute("PositionY") + @",
                                        width: 48,height:48}}" + (i == rootNode.ChildNodes.Count - 1 ? "" : ",") + "");

                }
                json.Append("},paths:{");

                XmlNode rootRule = doc.SelectSingleNode("WorkFlow").SelectSingleNode("Rules");
                for (int i = 0; i < rootRule.ChildNodes.Count; i++)
                {
                    StringBuilder dots = new StringBuilder();
                    XmlElement xElement = (XmlElement)rootRule.ChildNodes[i];
                    if (Convert.ToDouble(xElement.GetAttribute("TurnPoint1X")) > 0 && Convert.ToDouble(xElement.GetAttribute("TurnPoint1Y")) > 0)
                    {
                        dots.Append("{x: " + xElement.GetAttribute("TurnPoint1X") + ",y: " + xElement.GetAttribute("TurnPoint1Y") + "}");
                    }
                    if (Convert.ToDouble(xElement.GetAttribute("TurnPoint2X")) > 0 && Convert.ToDouble(xElement.GetAttribute("TurnPoint2Y")) > 0)
                    {
                        dots.Append(",{x: " + xElement.GetAttribute("TurnPoint2X") + ",y: " + xElement.GetAttribute("TurnPoint2Y") + "}");
                    }
                    json.Append(@"
                                " + ("path" + (i + 1).ToString()) + @": {
                                    lineID: " + string.Format("'{0}'", xElement.GetAttribute("ID")) + @",
                                    from: " + string.Format("'{0}'", htNode[xElement.GetAttribute("BeginActivityID")].ToString()) + @",
                                    to: " + string.Format("'{0}'", htNode[xElement.GetAttribute("EndActivityID")].ToString()) + @",
                                    dots: [
                                        " + dots + @"
                                    ],
                                    text: {
                                        text: " + string.Format("'{0}'", xElement.GetAttribute("Name")) + @",
                                        textPos: {
                                            x: 0,
                                            y: -10
                                        }
                                    },
                                    props: {
                                        text: {
                                            value: ''
                                        }
                                    }
                                }" + (i == rootRule.ChildNodes.Count - 1 ? "" : ",") + "");
                }
                json.Append("}}");
            }
            return json.ToString();
        }

        public JsonResult UpgradeFlow()
        {
            int s = 0; int failed = 0;
            var flow = entities.Set<S_WF_DefFlow>().Where(c => c.ViewConfig.Contains("WorkFlow")).ToArray();
            foreach (var item in flow)
            {
                try
                {
                    item.ViewConfig = GetflowJson(item.ViewConfig);
                    entities.SaveChanges();
                    s++;
                }
                catch (Exception exp)
                {
                    failed++;
                    LogWriter.Error(item.ID + "【" + item.Name + "】同步旧版流程是出错：" + exp.Message);
                }
            }

            var insDefineFlow = entities.Set<S_WF_InsDefFlow>().Where(c => c.ViewConfig.Contains("WorkFlow")).ToArray();
            foreach (var item in insDefineFlow)
            {
                try
                {
                    item.ViewConfig = GetflowJson(item.ViewConfig);
                    entities.SaveChanges();
                    s++;
                }
                catch (Exception exp)
                {
                    failed++;
                    LogWriter.Error(item.ID + "【" + item.Name + "】同步旧版流程是出错：" + exp.Message);
                }
            }
            var result = new Dictionary<string, object>();
            result["Success"] = s;
            result["Failed"] = failed;
            return Json(result);
        }
        #endregion

        #region  配置选择在路由上要启动的流程和环节信息
        public JsonResult GetFlowDefRelateInfo(string DefID)
        {
            var def = this.entities.Set<S_WF_DefFlow>().Find(DefID);
            if (def == null) throw new Formula.Exceptions.BusinessException("没有找到ID为【" + DefID + "】的流程定义");

            #region 获取可启动的环节
            var startTask = def.S_WF_DefStep.FirstOrDefault(c => c.Type == "Inital");
            if (startTask == null) throw new Formula.Exceptions.BusinessException("流程没有开始任务环节，无法作为自动化流程启动");
            var canStartList = new List<Dictionary<string, object>>();
            var startDic = new Dictionary<string, object>();
            startDic["value"] = startTask.ID;
            startDic["text"] = startTask.Name;
            canStartList.Add(startDic);

            foreach (var defRouting in startTask.S_WF_DefRouting.ToList())
            {
                if (String.IsNullOrEmpty(defRouting.UserIDs) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromField) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromSql) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStep) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStepExec) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStepSender) &&
                    String.IsNullOrEmpty(defRouting.UserIDsGroupFromField) &&
                    String.IsNullOrEmpty(defRouting.OrgIDFromField) &&
                       String.IsNullOrEmpty(defRouting.OrgIDFromUser) &&
                       String.IsNullOrEmpty(defRouting.OrgIDs) &&
                       String.IsNullOrEmpty(defRouting.RoleIDs) &&
                       String.IsNullOrEmpty(defRouting.RoleIDsFromField)
                    )
                {
                    //自动执行到的环节必须预先有设定人员
                    continue;
                }

                var nextTask = def.S_WF_DefStep.FirstOrDefault(c => c.ID == defRouting.EndID);
                if (nextTask != null)
                {
                    var taskDic = new Dictionary<string, object>();
                    taskDic["value"] = nextTask.ID;
                    taskDic["text"] = nextTask.Name;
                    canStartList.Add(taskDic);
                }
            }
            #endregion

            var db = SQLHelper.CreateSqlHelper(def.ConnName);
            var tableName = def.TableName;
            string sql = @"SELECT 
ColumnName = c.name, 
Description = ex.value, 
ColumnType=t.name, 
Length=c.max_length FROM sys.columns c 
LEFT OUTER JOIN sys.extended_properties ex 
ON  ex.major_id = c.object_id 
AND ex.minor_id = c.column_id 
AND ex.name = 'MS_Description' 
left outer join systypes t  on c.system_type_id=t.xtype 
WHERE  OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0 
AND OBJECT_NAME(c.object_id) ='{0}' and t.name!='sysname'
and  c.name!='FlowPhase' and c.name!='ID' and c.name!='CreateDate'
and c.name!='ModifyDate' and c.name!='CreateUserID' and c.name!='CreateUser' 
and c.name!='ModifyUserID' and c.name!='ModifyUser' and c.name!='OrgID'
and c.name!='CompanyID' and c.name!='CompanyID' and c.name!='StepName' and c.name!='FlowInfo' ";
            var fieldtable = db.ExecuteDataTable(String.Format(sql, tableName));
            var result = new Dictionary<string, object>();
            result["CanStartTaskList"] = canStartList;
            result["DataFormat"] = fieldtable;
            result["StartTaskID"] = startTask.ID;
            return Json(result);
        }

        public JsonResult GetCanStartList(string DefID)
        {
            var def = this.entities.Set<S_WF_DefFlow>().Find(DefID);
            if (def == null) throw new Formula.Exceptions.BusinessException("没有找到ID为【" + DefID + "】的流程定义");
            #region 获取可启动的环节
            var startTask = def.S_WF_DefStep.FirstOrDefault(c => c.Type == "Inital");
            if (startTask == null) throw new Formula.Exceptions.BusinessException("流程没有开始任务环节，无法作为自动化流程启动");
            var canStartList = new List<Dictionary<string, object>>();
            var startDic = new Dictionary<string, object>();
            startDic["value"] = startTask.ID;
            startDic["text"] = startTask.Name;
            canStartList.Add(startDic);

            foreach (var defRouting in startTask.S_WF_DefRouting.ToList())
            {
                if (String.IsNullOrEmpty(defRouting.UserIDs) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromField) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromSql) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStep) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStepExec) &&
                    String.IsNullOrEmpty(defRouting.UserIDsFromStepSender) &&
                    String.IsNullOrEmpty(defRouting.UserIDsGroupFromField) &&
                    String.IsNullOrEmpty(defRouting.OrgIDFromField) &&
                       String.IsNullOrEmpty(defRouting.OrgIDFromUser) &&
                       String.IsNullOrEmpty(defRouting.OrgIDs) &&
                       String.IsNullOrEmpty(defRouting.RoleIDs) &&
                       String.IsNullOrEmpty(defRouting.RoleIDsFromField)
                    )
                {
                    //自动执行到的环节必须预先有设定人员
                    continue;
                }

                var nextTask = def.S_WF_DefStep.FirstOrDefault(c => c.ID == defRouting.EndID);
                if (nextTask != null)
                {
                    var taskDic = new Dictionary<string, object>();
                    taskDic["value"] = nextTask.ID;
                    taskDic["text"] = nextTask.Name;
                    canStartList.Add(taskDic);
                }
            }
            #endregion
            var result = new Dictionary<string, object>();
            result["CanStartList"] = canStartList;
            result["StartTaskID"] = startTask.ID;
            return Json(result);
        }
        #endregion

        #region  授权到子公司
        public JsonResult SetCompanyAuth(string objIds, string orgIds, string orgNames)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "update S_WF_DefFlow set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')";
            sql = string.Format(sql, orgIds, orgNames, objIds.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }
        #endregion


    }
}
