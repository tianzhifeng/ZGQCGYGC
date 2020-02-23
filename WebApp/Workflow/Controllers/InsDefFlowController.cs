using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Formula.Helper;
using System.Data;
using Config;
using Formula;
using Workflow.Logic;
using System.Text;
using System.Collections;
using System.Xml;

namespace Workflow.Web.Controllers
{
    public class InsDefFlowController : Workflow.BaseController
    {
        #region 流程定义属性页

        public ActionResult DefProperty()
        {
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

        public ActionResult RoutingProperty()
        {
            string defID = this.Request["defFlowID"];
            var FlowSteps = this.entities.Set<S_WF_InsDefStep>().Where(c => c.InsDefFlowID == defID).ToList();
            var FlowStepEnum = new List<Dictionary<string, object>>();
            for (int i = 0; i < FlowSteps.Count; i++)
            {
                var step = FlowSteps[i];
                var dic = new Dictionary<string, object>();
                dic["text"] = step.Name;
                dic["value"] = step.ID;
                dic["sortindex"] = i;
                FlowStepEnum.Add(dic);
            }
            ViewBag.FlowStep = JsonHelper.ToJson(FlowStepEnum);
            return View();
        }
        #endregion

        #region GetData

        public JsonResult GetData(string id, string type)
        {
            if (type == "Flow")
            {
                return Json(entities.Set<S_WF_InsDefFlow>().SingleOrDefault(c => c.ID == id));
            }
            else if (type == "Step")
            {
                var step = entities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == id);

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
                return Json(entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == id));
            }
        }

        #endregion

        #region 保存
        [ValidateInput(false)]
        public JsonResult SaveChangedData(string changedData, string layout, string id)
        {
            var def = entities.Set<S_WF_InsDefFlow>().SingleOrDefault(c => c.ID == id);
            def.ViewConfig = layout;


            Dictionary<string, Dictionary<string, object>> dic = JsonHelper.ToObject<Dictionary<string, Dictionary<string, object>>>(changedData);
            foreach (var item in dic.Values)
            {
                if (item["Type"].ToString() == "Flow")
                {
                    string categoryID = def.CategoryID;
                    def = UpdateEntityFromJson<S_WF_InsDefFlow>(item["Data"].ToString());

                }
                else if (item["Type"].ToString() == "Step")
                {
                    Dictionary<string, object> itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item["Data"].ToString());
                    string itemID = itemDic["ID"].ToString();
                    var step = entities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == itemID);
                    UpdateEntity<S_WF_InsDefStep>(step, itemDic);

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
                else
                {
                    Dictionary<string, object> itemDic = JsonHelper.ToObject<Dictionary<string, object>>(item["Data"].ToString());
                    string itemID = itemDic["ID"].ToString();
                    var routing = entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == itemID);
                    UpdateEntity<S_WF_InsDefRouting>(routing, itemDic);

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

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 获取控制字段列表

        public JsonResult GetStepFieldList(string id)
        {
            return Json(GetStepFieldTable(id));
        }

        private DataTable GetStepFieldTable(string id)
        {
            var step = entities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == id);
            var def = step.S_WF_InsDefFlow;

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
            var routing = entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == id);
            var def = routing.S_WF_InsDefFlow;


            string connName = def.ConnName;
            string tableName = def.TableName;
            if (!string.IsNullOrEmpty(routing.S_WF_InsDefStep.SubFormID))
            {
                var subForm = entities.Set<S_WF_DefSubForm>().SingleOrDefault(c => c.ID == routing.S_WF_InsDefStep.SubFormID);
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



        #region 添加节点
        public string NewActivity(string defineID, string name, string type)
        {
            string result = "";
            string guid = FormulaHelper.CreateGuid();
            try
            {
                var flowDefine = entities.Set<S_WF_InsDefFlow>().Where(c => c.ID == defineID).SingleOrDefault();

                if (flowDefine == null)
                    throw new Exception("指定的工作流定义未找到");

                S_WF_InsDefStep step = new S_WF_InsDefStep();
                step.ID = guid.ToString();
                step.Type = type;
                step.Name = name;
                step.SortIndex = flowDefine.S_WF_InsDefStep.Count;
                step.InsDefFlowID = defineID;
                if (type == "Completion")
                    step.Phase = "End";
                else
                    step.Phase = "Processing";
                step.CooperationMode = TaskCooperationMode.Single.ToString();
                step.AllowDoBackFirst = "1";
                step.AllowDoBackFirstReturn = "0";
                if (type == "Inital")
                {
                    step.HideAdvice = "1";
                }
                entities.Set<S_WF_InsDefStep>().Add(step);
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
                var steps = entities.Set<S_WF_InsDefStep>().Where(c => stepIDs.Contains(c.ID)).ToArray();

                var allSteps = steps[0].S_WF_InsDefFlow.S_WF_InsDefStep.ToArray();
                foreach (var step in steps)
                {
                    //移除等待环节
                    foreach (var item in allSteps)
                    {
                        item.WaitingStepIDs = StringHelper.Exclude(item.WaitingStepIDs, step.ID);
                    }
                    //删除环节
                    entities.Set<S_WF_InsDefStep>().Remove(step);
                    //删除路由                    
                    entities.Set<S_WF_InsDefRouting>().Delete(c => c.EndID == step.ID);
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
                var defRouting = entities.Set<S_WF_InsDefRouting>().Where(c => c.ID == lineID).SingleOrDefault();
                entities.Set<S_WF_InsDefRouting>().Remove(defRouting);
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
                var flowDef = entities.Set<S_WF_InsDefFlow>().Where(c => c.ID == defFlowID).SingleOrDefault();
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
                var flowDef = entities.Set<S_WF_InsDefFlow>().Where(c => c.ID == defineID).SingleOrDefault();
                if (flowDef == null)
                    throw new Exception("指定的工作流定义未找到");
                var startStep = entities.Set<S_WF_InsDefStep>().Where(c => c.ID == beginActivityID).SingleOrDefault();
                var endStep = entities.Set<S_WF_InsDefStep>().Where(c => c.ID == endAcitivityID).SingleOrDefault();
                var routing = new S_WF_InsDefRouting();
                routing.ID = guid;
                routing.InsDefStepID = startStep.ID;
                routing.EndID = endStep.ID;
                routing.InsDefFlowID = defineID;
                routing.SortIndex = startStep.S_WF_InsDefRouting.Count;
                routing.Type = RoutingType.Normal.ToString();
                routing.Name = "送" + endStep.Name;
                routing.AllowDoBack = "1";//默认允许打回  2014-12-1
                routing.SaveForm = "1"; //默认自动保存表单 2014-12-1
                routing.MustInputComment = "1"; //默认弹出意见框 2014-12-1
                routing.AllowWithdraw = "1"; //默认允许撤销 2014-12-1
                entities.Set<S_WF_InsDefRouting>().Add(routing);

                if (routing.Name == "送结束")
                {
                    routing.Name = "结束";
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
            var startStep = entities.Set<S_WF_InsDefStep>().Where(c => c.InsDefFlowID == defineID && c.Type == "Inital").ToArray();
            return startStep.Length.ToString();
        }
        #endregion


    }
}
