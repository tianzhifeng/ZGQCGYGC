using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Config;
using Config.Logic;
using Formula.Exceptions;
using Formula;
using Formula.Helper;
using System.Data;
using System.Web;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsDefRouting
    {
        #region 保存表单数据版本

        public void SaveFormDataVersion(S_WF_InsTask task)
        {
            SQLHelper SqlHelper = SQLHelper.CreateSqlHelper(task.S_WF_InsFlow.S_WF_InsDefFlow.ConnName);
            string sql = string.Format("select * from {0} where ID='{1}'", task.S_WF_InsFlow.S_WF_InsDefFlow.TableName, task.S_WF_InsFlow.FormInstanceID);

            DataTable dt = SqlHelper.ExecuteDataTable(sql);

            string json = JsonHelper.ToJson(dt.Rows[0]);

            task.FormVersion = json;

        }

        #endregion

        #region 保存流程变量版本

        public void SaveFlowVariableVersion(S_WF_InsTask task)
        {
            string json = JsonHelper.ToJson(task.S_WF_InsFlow.S_WF_InsVariable);
            task.VariableVersion = json;
        }

        #endregion

        #region 签字
        public void Signature(S_WF_InsTaskExec taskExec, string execComment)
        {
            if (string.IsNullOrEmpty(this.SignatureField))
                return;

            foreach (string field in this.SignatureField.Split(','))
            {
                if (string.IsNullOrEmpty(field)) continue;
                SignatureExec(field, taskExec, execComment);
            }

        }

        //签字(单个签字字段)
        private void SignatureExec(string signField, S_WF_InsTaskExec taskExec, string execComment)
        {
            if (string.IsNullOrEmpty(signField))
                return;

            SignatureObj signatureObj = new SignatureObj();
            UserInfo user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);

            string formData = HttpContext.Current.Request["FormData"];
            string connName = GetFormConnName();
            string tableName = GetFormTableName();
            string formInstanceID = taskExec.S_WF_InsTask.S_WF_InsFlow.FormInstanceID;

            //表单字典
            var formDic = S_WF_InsFlow.GetFormDic(formData, connName, tableName, formInstanceID);
            if (!formDic.ContainsKey(signField))
                throw new FlowException(string.Format("签字字段不存在:{0}", signField));

            if (!string.IsNullOrEmpty(this.SignatureProtectFields))
            {
                foreach (string protectField in this.SignatureProtectFields.Split(','))
                {
                    if (!formDic.ContainsKey(protectField))
                        throw new FlowException(string.Format("被保护的字段不存在:{0}", protectField));
                    signatureObj.SignatureProtectField.Add(protectField, formDic[protectField]);
                }
            }

            List<SignatureObj> list = null;
            string alreadySign = formDic[signField].ToString();
            if (alreadySign != "" && alreadySign.StartsWith("[") == false)
                throw new FlowException(string.Format("{0}字段的内容不能解析为签字格式！", signField));
            if (!string.IsNullOrEmpty(alreadySign))
            {
                list = JsonHelper.ToObject<List<SignatureObj>>(alreadySign);
                //容错错误数据
                if (list == null)
                    list = new List<SignatureObj>();

                if (System.Configuration.ConfigurationManager.AppSettings["Flow_SignDistinct"] != "False")
                {
                    //消除重复签字 
                    list = list.Where(c => c.ExecUserID != user.UserID).ToList();
                }

            }
            else
                list = new List<SignatureObj>();

            #region 处理用户分组

            List<Dictionary<string, string>> userList = null;
            if (!string.IsNullOrEmpty(taskExec.S_WF_InsTask.TaskUserIDsGroup))
            {
                userList = JsonHelper.ToObject<List<Dictionary<string, string>>>(taskExec.S_WF_InsTask.TaskUserIDsGroup);
                userList = userList.Where(c => c["UserID"] == taskExec.TaskUserID).ToList();
            }
            else
            {
                userList = new List<Dictionary<string, string>>();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("UserID", taskExec.TaskUserID);
                dic.Add("UserName", taskExec.TaskUserName);
                dic.Add("GroupName", "");
                userList.Add(dic);
            }

            #endregion

            foreach (var dic in userList)
            {
                SignatureObj obj = new SignatureObj();
                obj.TaskUserGroupName = dic["GroupName"];
                obj.TaskUserID = dic["UserID"];
                obj.TaskUserName = dic["UserName"];
                obj.ExecUserID = user.UserID;
                obj.ExecUserName = user.UserName;
                obj.RoutingCode = this.Code;
                obj.RoutingName = this.Name;
                obj.RoutingValue = this.Value;
                obj.SignTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                obj.SignComment = execComment;
                obj.SignatureProtectValue = JsonHelper.ToJson(obj.SignatureProtectField).GetHashCode().ToString();
                obj.StepCode = this.S_WF_InsDefStep.Code;
                obj.StepName = this.S_WF_InsDefStep.Name;

                //电子签章显示位置DivID
                obj.SignatureDivID = this.SignatureDivID == null ? "" : this.SignatureDivID;
                if (obj.SignatureDivID.EndsWith("@UserID"))
                    obj.SignatureDivID = obj.SignatureDivID.Replace("@UserID", user.UserID);

                list.Add(obj);

            }

            string strSign = JsonHelper.ToJson<List<SignatureObj>>(list);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            try
            {
                string strSql = string.Format("SELECT count(COLUMN_NAME) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = upper('{0}') and COLUMN_NAME='MODIFYDATE'", tableName);
                if (Config.Constant.IsOracleDb == false)
                    strSql = string.Format("select count(1) from syscolumns where id=object_id('{0}') and name='ModifyDate'", tableName);

                if (sqlHelper.ExecuteScalar(strSql).ToString() != "0")
                {
                    string sql = string.Format("update {0} set {1}='{2}',ModifyDate='{4}' where ID='{3}'", tableName, signField, strSign, formInstanceID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sqlHelper.ExecuteNonQuery(sql);
                }
                else
                {
                    string sql = string.Format("update {0} set {1}='{2}' where ID='{3}'", tableName, signField, strSign, formInstanceID);
                    sqlHelper.ExecuteNonQuery(sql);
                }

                //金格签字保存到签字表
                if (!string.IsNullOrEmpty(HttpContext.Current.Request["signatureid"]) && !string.IsNullOrEmpty(HttpContext.Current.Request["signatureData"]))
                {
                    SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    strSql = "insert into S_UI_JinGeSign (ID,FormID,signatureid,signatureData,signUserId) values('{0}','{1}','{2}','{3}','{4}')";
                    strSql = string.Format(strSql, FormulaHelper.CreateGuid(), formInstanceID, HttpContext.Current.Request["signatureid"], HttpContext.Current.Request["signatureData"], taskExec.ExecUserID);
                    baseSqlHelper.ExecuteNonQuery(strSql);
                }
            }
            catch (Exception ex)
            {
                throw new FlowException("sql执行出错!", ex);
            }
        }

        #endregion

        #region 执行自定义逻辑
        public void ExeLogic()
        {
            if (String.IsNullOrEmpty(this.ExecLogic))
            {
                return;
            }
            var logicList = JsonHelper.ToList(this.ExecLogic);
            foreach (var logic in logicList)
            {
                if (logic.GetValue("LogicType") == FlowExecLogicType.StartFlow.ToString())
                {
                    StartFlowExe(logic);
                }
                else if (logic.GetValue("LogicType") == FlowExecLogicType.DBSet.ToString())
                {
                    DbSetExe();
                }
            }
        }

        private void StartFlowExe(Dictionary<string, object> logicDef)
        {
            var settings = logicDef.GetValue("Settings");
            if (String.IsNullOrEmpty(settings))
            {
                return;
            }
            var settingDic = JsonHelper.ToObject(settings);
            var wfEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            var defFlow = wfEntities.S_WF_DefFlow.Find(settingDic.GetValue("FlowID"));
            string startTaskDefID = settingDic.GetValue("StartTaskID");
            if (defFlow == null)
            {
                throw new BusinessException("没有找到ID为【" + settingDic.GetValue("FlowID") + "】的流程定义，无法在流程执行过程中启动其他流程");
            }

            #region 确认表单定义对象
            var formCode = String.IsNullOrEmpty(settingDic.GetValue("FormCode")) ? defFlow.Code : settingDic.GetValue("FormCode");
            var baseEntities = FormulaHelper.GetEntities<Base.Logic.Domain.BaseEntities>();
            var tmplForm = baseEntities.S_UI_Form.FirstOrDefault(c => c.Code == formCode);
            #endregion

            var defStep = defFlow.S_WF_DefStep.FirstOrDefault(c => c.ID == startTaskDefID);
            if (defStep == null) throw new BusinessException("没有找到指定的定义实例，无法启动工作流");

            var user = FormulaHelper.GetUserInfo();

            #region 默认创建数据实例
            string sql = "select * from {0} where ID='{1}'";
            var flowIns = this.S_WF_InsDefFlow.S_WF_InsFlow.FirstOrDefault();
            var orialDt = SQLHelper.CreateSqlHelper(this.S_WF_InsDefFlow.ConnName).ExecuteDataTable(String.Format(sql, this.S_WF_InsDefFlow.TableName, flowIns.FormInstanceID));
            var orialDic = new Dictionary<string, object>();
            var formInstanceID = FormulaHelper.CreateGuid();
            var dic = new Dictionary<string, object>();
            dic.SetValue("CreateUserID", user.UserID);
            dic.SetValue("CreateUser", user.UserName);
            dic.SetValue("OrgID", user.UserOrgID);
            if (settingDic.ContainsKey("DataFormat") && settingDic["DataFormat"] != null)
            {
                var dataFormat = new List<Dictionary<string, object>>();
                if (settingDic["DataFormat"] is List<Dictionary<string, object>> || settingDic["DataFormat"] is Array)
                {
                    dataFormat = settingDic["DataFormat"] as List<Dictionary<string, object>>;
                }
                else if (!String.IsNullOrEmpty(settingDic.GetValue("DataFormat")))
                {
                    dataFormat = JsonHelper.ToList(settingDic.GetValue("DataFormat"));
                }

                #region 根据表单自定义配置来设定默认值
                if (tmplForm != null)
                {
                    var uiFO = FormulaHelper.CreateFO<Base.Logic.BusinessFacade.UIFO>();
                    Dictionary<string, DataTable> defaultValueRows = uiFO.GetDefaultValueDic(tmplForm.DefaultValueSettings, orialDt.Rows[0]);
                    var items = JsonHelper.ToObject<List<Base.Logic.Model.UI.Form.FormItem>>(tmplForm.Items);
                    items.Where(c => !string.IsNullOrEmpty(c.DefaultValue)).ToList().ForEach(d =>
                    {
                        if (d.ItemType == "SubTable")
                        {
                            //子表
                            dic.SetValue(d.Code, uiFO.GetDefaultValue(d.Code, d.DefaultValue, defaultValueRows));
                        }
                        else if ((d.ItemType == "ButtonEdit" || d.ItemType == "ComboBox") && d.DefaultValue.Split(',').Count() == 2)
                        {
                            //键值控件
                            string v1 = uiFO.GetDefaultValue(d.Code, d.DefaultValue.Split(',')[0], defaultValueRows);
                            string v2 = uiFO.GetDefaultValue(d.Code, d.DefaultValue.Split(',')[1], defaultValueRows);
                            if (!string.IsNullOrEmpty(v1) && v1.Contains('{') == false)
                                dic.SetValue(d.Code, v1);
                            if (!string.IsNullOrEmpty(v2) && v2.Contains('{') == false)
                            {
                                var fieldName = "";
                                if (Config.Constant.IsOracleDb)
                                {
                                    fieldName = d.Code + "NAME";
                                    if (!String.IsNullOrWhiteSpace(d.Settings))
                                    {
                                        var fieldSettings = JsonHelper.ToObject(d.Settings);
                                        var txtName = fieldSettings.GetValue("textName");
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
                                        var fieldSettings = JsonHelper.ToObject(d.Settings);
                                        var txtName = fieldSettings.GetValue("textName");
                                        if (!String.IsNullOrEmpty(txtName))
                                        {
                                            fieldName = txtName;
                                        }
                                    }
                                }
                                dic.SetValue(fieldName, v2);
                            }
                        }
                        else
                        {
                            //单值控件
                            string v = uiFO.GetDefaultValue(d.Code, d.DefaultValue, defaultValueRows);
                            if (!string.IsNullOrEmpty(v) && v.Contains('{') == false)
                                dic.SetValue(d.Code, v);
                        }
                    });
                }
                #endregion

                #region 设置当前表单内的默认值
                if (orialDt.Rows.Count > 0)
                {
                    var sourceRow = orialDt.Rows[0];
                    orialDic = FormulaHelper.DataRowToDic(sourceRow);
                    var fillFields = dataFormat.Where(c => !String.IsNullOrEmpty(c.GetValue("DefaultValue"))).ToList();
                    foreach (var field in fillFields)
                    {
                        Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
                        string defaultValueDef = field.GetValue("DefaultValue");
                        string result = reg.Replace(defaultValueDef, (Match m) =>
                        {
                            string value = m.Value.Trim('{', '}');
                            if (sourceRow != null && sourceRow.Table.Columns.Contains(value))
                                return sourceRow[value].ToString();
                            switch (value)
                            {
                                case Formula.Constant.CurrentUserID:
                                    return user.UserID;
                                case Formula.Constant.CurrentUserName:
                                    return user.UserName;
                                case Formula.Constant.CurrentUserOrgID:
                                    return user.UserOrgID;
                                case Formula.Constant.CurrentUserOrgCode:
                                    return user.UserOrgCode;
                                case Formula.Constant.CurrentUserOrgName:
                                    return user.UserOrgName;
                                case Formula.Constant.CurrentUserPrjID:
                                    return user.UserPrjID;
                                case Formula.Constant.CurrentUserPrjName:
                                    return user.UserPrjName;
                                case "CurrentUserCorpID":
                                    return user.UserCompanyID;
                                case "CurrentUserCorpName":
                                    return user.UserCompanyName;
                                case "CurrentTime":
                                    return DateTime.Now.ToString();
                                case "CurrentDate":
                                    return DateTime.Now.Date.ToString("yyyy-MM-dd");
                                case "CurrentYear":
                                    return DateTime.Now.Year.ToString();
                                case "CurrentMonth":
                                    return DateTime.Now.Month.ToString();
                                case "CurrentQuarter":
                                    return ((DateTime.Now.Month + 2) / 3).ToString();
                                case "CurrentUserOrgFullID":
                                    return user.UserFullOrgID;
                                default:
                                    return "";
                            }
                        });
                        dic.SetValue(field.GetValue("ColumnName"), result);
                    }
                }
                #endregion
            }
            var db = SQLHelper.CreateSqlHelper(defFlow.ConnName);
            dic.InsertDB(db, defFlow.TableName, formInstanceID);
            #endregion

            int maxReceiverCount = 50;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]))
                maxReceiverCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]);

            #region 获取工作流环节的指定执行人
            var userIDs = settingDic.GetValue("UserIDs");
            if (String.IsNullOrEmpty(userIDs))
            {
                userIDs = orialDic.GetValue(settingDic.GetValue("UserIDsFromField"));
            }
            if (String.IsNullOrEmpty(userIDs))
            {
                var roleIDs = settingDic.GetValue("RoleIDs");
                var orgIDs = settingDic.GetValue("OrgIDs");
                if (String.IsNullOrEmpty(roleIDs))
                {
                    roleIDs = orialDic.GetValue(settingDic.GetValue("RoleIDsFromField"));
                }
                if (String.IsNullOrEmpty(orgIDs))
                {
                    orgIDs = orialDic.GetValue(settingDic.GetValue("OrgIDFromField"));
                }
                if (!string.IsNullOrEmpty(roleIDs))
                {
                    userIDs = FormulaHelper.GetService<IRoleService>().GetUserIDsInRoles(roleIDs, orgIDs);
                    var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                    if (!string.IsNullOrEmpty(prjRoleUser))
                        userIDs = (prjRoleUser + "," + userIDs).Trim(',');
                }
                else
                    userIDs = FormulaHelper.GetService<IOrgService>().GetUserIDsInOrgs(orgIDs);
            }
            if (String.IsNullOrEmpty(userIDs))
            {
                throw new BusinessValidationException("没有找到环节执行人员，无法启动流程");
            }
            if (userIDs.Split(',').Length > maxReceiverCount)
                throw new FlowException("任务执行人不能超过" + maxReceiverCount + "个");
            #endregion

            var fo = FormulaHelper.CreateFO<Workflow.Logic.BusinessFacade.FlowFO>();

            var insDefFlow = defFlow.GetInsDefFlow();
            var insFlow = insDefFlow.CreateFlow(formInstanceID, user.UserID, user.UserName);

            //获取流程定义实例首环节
            string Inital = StepTaskType.Inital.ToString();
            S_WF_InsDefStep insDefStep = null; bool startWithInital = false;
            var insStartStep = insDefFlow.S_WF_InsDefStep.Where(c => c.Type == Inital).SingleOrDefault();
            if (insStartStep.DefStepID == startTaskDefID)
            {
                insDefStep = insStartStep;
                startWithInital = true;
            }
            else
            {
                var stepIds = insStartStep.S_WF_InsDefRouting.Select(c => c.EndID);
                insDefStep = insDefFlow.S_WF_InsDefStep.Where(c => stepIds.Contains(c.ID) && c.DefStepID == startTaskDefID).FirstOrDefault();
            }
            if (insDefStep == null)
            {
                throw new BusinessException("没有找到指定的定义实例，无法启动工作流");
            }
            //创建首环节任务
            var startUserID = userIDs.Split(',')[0];
            var insTask = insStartStep.CreateTask(insFlow, null, null, startUserID, FormulaHelper.GetService<IUserService>().GetUserNames(startUserID), "", "", "");
            //创建任务执行明细
            var taskExec = insTask.CreateTaskExec().FirstOrDefault();
            if (!startWithInital)
            {
                var insDefRouting = insStartStep.S_WF_InsDefRouting.Where(c => c.EndID == insDefStep.ID).FirstOrDefault();
                fo.ExecTask(taskExec.ID, insDefRouting.Code, userIDs, UserNames, "");
            }
        }

        private void DbSetExe()
        {

        }

        #endregion

        #region 设置表单数据

        /// <summary>
        /// 设置表单数据
        /// </summary>
        /// <param name="op"></param>
        /// <param name="execComment"></param>
        /// <param name="reverse"></param>
        public void SetFormData(string formInstanceID, S_WF_InsTaskExec taskExec, string execComment, string nextUserIDs, string nextUserNames)
        {
            if (string.IsNullOrEmpty(this.FormDataSet))
                return;

            string formDataSet = this.FormDataSet;

            //意见为空则不设置意见
            if (this.FormDataSet.Contains("{ExecComment}") && string.IsNullOrEmpty(execComment))
                return;

            if (Config.Constant.IsOracleDb)
            {
                string oracleDate = string.Format("to_date('{0}','yyyy-MM-dd HH24:mi:ss')", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                formDataSet = formDataSet.Replace("'{ExecTime}'", oracleDate);
            }


            Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
            string strFormDataSet = reg.Replace(formDataSet, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                switch (value)
                {
                    case "TaskUserID":
                        return taskExec.TaskUserID;
                    case "TaskUserName":
                        return taskExec.TaskUserName;
                    case "ExecUserID":
                        return taskExec.ExecUserID;
                    case "ExecUserName":
                        return taskExec.ExecUserName;
                    case "ExecTime":
                        return DateTime.Now.ToString();
                    case "ExecComment":
                        return execComment;
                    case "NextUserIDs":
                        return nextUserIDs;
                    case "NextUserNames":
                        return nextUserNames;
                    case "NextStepCode":
                        return this.S_WF_InsDefFlow.S_WF_InsDefStep.SingleOrDefault(c => c.ID == this.EndID).Code;
                    case "NextStepName":
                        return this.S_WF_InsDefFlow.S_WF_InsDefStep.SingleOrDefault(c => c.ID == this.EndID).Name;
                    case "StepCode":
                        return this.S_WF_InsDefStep.Code;
                    case "StepName":
                        return this.S_WF_InsDefStep.Name;
                    default:
                        return m.Value;
                }
            });

            string connName = GetFormConnName();
            string tableName = GetFormTableName();
            try
            {
                if (Config.Constant.IsOracleDb)
                {
                    //SELECT COUNT(1) into flagExist FROM USER_TAB_COLUMNS WHERE TABLE_NAME = upper('{0}') and column_name = upper('{1}')
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
                    if (sqlHelper.ExecuteScalar(string.Format("SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = upper('{0}') and column_name = upper('ModifyDate')", tableName)).ToString() != "0")
                    {
                        string sql = string.Format("update {0} set {1},ModifyDate=to_date('{3}','yyyy-MM-dd HH24:mi:ss') where ID='{2}'", tableName, strFormDataSet, formInstanceID, DateTime.Now);
                        sqlHelper.ExecuteNonQuery(sql);
                    }
                    else
                    {
                        string sql = string.Format("update {0} set {1} where ID='{2}'", tableName, strFormDataSet, formInstanceID);
                        sqlHelper.ExecuteNonQuery(sql);
                    }
                }
                else
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
                    if (sqlHelper.ExecuteScalar(string.Format("select count(1) from syscolumns where id=object_id('{0}') and name='ModifyDate'", tableName)).ToString() != "0")
                    {
                        string sql = string.Format("update {0} set {1},ModifyDate='{3}' where ID='{2}'", tableName, strFormDataSet, formInstanceID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sqlHelper.ExecuteNonQuery(sql);
                    }
                    else
                    {
                        string sql = string.Format("update {0} set {1} where ID='{2}'", tableName, strFormDataSet, formInstanceID);
                        sqlHelper.ExecuteNonQuery(sql);
                    }
                }
            }
            catch
            {
                string msg = string.Format("流程路由配置错误，FormDataSet：{0}", this.FormDataSet);
                throw new FlowException(msg);
            }

        }

        #endregion

        #region 设置流程变量

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableSet"></param>
        /// <param name="nextExecUserIDs"></param>
        /// <param name="execComment"></param>
        /// <param name="proxyRoles">当前用户所代理的角色</param>
        /// <param name="reverse">是否反向操作（撤回需要反向操作</param>
        public void SetFlowVariable(S_WF_InsFlow flow)
        {
            if (string.IsNullOrEmpty(this.VariableSet))
                return;

            foreach (string str in this.VariableSet.Split(','))
            {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                string strKey = str.Split('=')[0];
                string strValue = str.Contains('=') ? str.Split('=')[1] : "";


                strValue = strValue.Trim('\'', ' ');


                S_WF_InsVariable variable = flow.S_WF_InsVariable.Where(c => c.VariableName == strKey).SingleOrDefault();
                if (variable == null)
                {
                    variable = new S_WF_InsVariable();
                    variable.ID = FormulaHelper.CreateGuid();
                    variable.VariableName = strKey;
                    variable.VariableValue = strValue;
                    flow.S_WF_InsVariable.Add(variable);
                }
                else
                {
                    variable.VariableValue = strValue;
                }
            }
        }

        #endregion

        #region 设置表单阶段

        /// <summary>
        /// 设置表单数据
        /// </summary>
        /// <param name="op"></param>
        /// <param name="execComment"></param>
        /// <param name="reverse"></param>
        public void SetFormPhase(S_WF_InsFlow flow, S_WF_InsDefStep nextStep, string completeStepName)
        {
            try
            {
                if (string.IsNullOrEmpty(flow.FormInstanceID))
                    throw new FlowException("表单未找到。");

                //string connName = GetFormConnName();
                //string tableName = GetFormTableName();
                string connName = this.S_WF_InsDefFlow.ConnName;
                string tableName = this.S_WF_InsDefFlow.TableName;

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);

                string sql = string.Format("select * from {0} where ID='{1}'", tableName, flow.FormInstanceID);
                DataTable dt = sqlHelper.ExecuteDataTable(sql);

                string flowPhase = "";
                if (nextStep != null)
                    flowPhase = nextStep.Phase;
                else if (flow.CompleteTime != null)
                    flowPhase = FlowTaskStatus.Complete.ToString();

                sql = "";
                if (flowPhase != "")
                {
                    if (dt.Columns.Contains("FlowPhase") || dt.Columns.Contains("FLOWPHASE")) //FLOWPHASE为了兼容Oracle
                    {
                        var sqlUpdate = string.Format(" update {0} set FlowPhase='{1}' where ID='{2}'", tableName, flowPhase, flow.FormInstanceID);
                        sql += sqlUpdate;
                        if (Config.Constant.IsOracleDb)
                        {
                            sqlHelper.ExecuteNonQuery(sqlUpdate);
                        }
                    }
                }
                if (dt.Columns.Contains("StepName") || dt.Columns.Contains("STEPNAME"))   //STEPNAME为了兼容Oracle
                {
                    string stepName = "";
                    if (nextStep != null)
                        stepName = nextStep.Name;
                    if (dt.Rows.Count > 0)
                        stepName += "," + dt.Rows[0]["StepName"].ToString();
                    stepName = string.Join(",", stepName.Split(',').Distinct().Where(c => c != completeStepName && c != "").ToArray()).Trim(',');

                    //流程完成后环节清空
                    if (flow.CompleteTime != null)
                    {
                        stepName = "结束";                        
                    }
                    flow.CurrentStep = stepName;

                    var sqlUpdate = string.Format(" update {0} set StepName='{1}' where ID='{2}'", tableName, stepName, flow.FormInstanceID);
                    sql += sqlUpdate;
                    if (Config.Constant.IsOracleDb)
                    {
                        sqlHelper.ExecuteNonQuery(sqlUpdate);
                    }
                }

                if (sql != "" && !Config.Constant.IsOracleDb)
                    sqlHelper.ExecuteNonQuery(sql);


            }
            catch (Exception ex)
            {
                string msg = "表单表必须有字段FlowPhase!";
                throw new FlowException(msg, ex);
            }
        }

        #endregion

        #region 发消息

        public void SendMsg(S_WF_InsTaskExec taskExec, string execComment, string nextUserIDs, string nextUserNames)
        {
            try
            {
                string msg = "";

                #region 生成消息
                string tmpl = this.MsgTmpl;
                Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
                msg = reg.Replace(tmpl, (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');

                    //先替换表单字段
                    if (taskExec.S_WF_InsFlow.FormDic.ContainsKey(value))
                        return taskExec.S_WF_InsFlow.FormDic[value];

                    switch (value)
                    {
                        case "TaskName":
                            return taskExec.S_WF_InsTask.TaskName;
                        case "FlowName":
                            return taskExec.S_WF_InsFlow.FlowName;
                        case "RoutingName":
                            return this.Name;
                        case "TaskUserID":
                            return taskExec.TaskUserID;
                        case "TaskUserName":
                            return taskExec.TaskUserName;
                        case "ExecUserID":
                            return taskExec.ExecUserID;
                        case "ExecUserName":
                            return taskExec.ExecUserName;
                        case "ExecTime":
                            return DateTime.Now.ToString();
                        case "ExecComment":
                            return execComment;
                        case "NextUserIDs":
                            return nextUserIDs;
                        case "NextUserNames":
                            return nextUserNames;
                        case "NextStepCode":
                            return this.S_WF_InsDefFlow.S_WF_InsDefStep.SingleOrDefault(c => c.ID == this.EndID).Code;
                        case "NextStepName":
                            return this.S_WF_InsDefFlow.S_WF_InsDefStep.SingleOrDefault(c => c.ID == this.EndID).Name;
                        case "StepCode":
                            return this.S_WF_InsDefStep.Code;
                        case "StepName":
                            return this.S_WF_InsDefStep.Name;
                        default:
                            return m.Value;
                    }
                });

                #endregion

                if (msg == "")
                    return;

                List<string> userIdList = new List<string>();

                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                var flow = taskExec.S_WF_InsFlow;

                //给指定人发消息
                if (!string.IsNullOrEmpty(this.MsgUserIDs))
                    userIdList.AddRange(this.MsgUserIDs.Split(','));


                //环节任务执行人
                if (!string.IsNullOrEmpty(this.MsgUserIDsFromStepExec))
                {
                    foreach (string stepId in this.MsgUserIDsFromStepExec.Split(','))
                    {
                        var task = flow.S_WF_InsTask.LastOrDefault(c => c.InsDefStepID == stepId);
                        if (task == null)
                            throw new Exception("发送消息配置错误");
                        foreach (var item in task.S_WF_InsTaskExec)
                            userIdList.Add(item.ExecUserID);
                    }
                }
                //环节任务接收人
                if (!string.IsNullOrEmpty(this.MsgUserIDsFromStep))
                {
                    foreach (string stepId in this.MsgUserIDsFromStep.Split(','))
                    {

                        var task = flow.S_WF_InsTask.LastOrDefault(c => c.InsDefStepID == this.MsgUserIDsFromStep);
                        if (task == null)
                            throw new Exception("发送消息配置错误");
                        userIdList.AddRange(task.TaskUserIDs.Split(','));
                    }
                }
                //环节任务发送人
                if (!string.IsNullOrEmpty(this.MsgUserIDsFromStepSender))
                {
                    foreach (string stepId in this.MsgUserIDsFromStepSender.Split(','))
                    {
                        var task = flow.S_WF_InsTask.LastOrDefault(c => c.InsDefStepID == this.MsgUserIDsFromStepSender);
                        if (task == null)
                            throw new Exception("发送消息配置错误");
                        userIdList.AddRange(task.SendTaskUserIDs.Split(','));
                    }
                }
                //取自字段
                if (!string.IsNullOrEmpty(this.MsgUserIDsFromField))
                {
                    userIdList.AddRange(flow.FormDic[this.MsgUserIDsFromField].Split(','));
                }

                #region 指定组织、角色

                string orgIDs = this.MsgOrgIDs;
                string roleIDs = this.MsgRoleIDs;

                if (string.IsNullOrEmpty(orgIDs) && !string.IsNullOrEmpty(this.MsgOrgIDsFromField))
                {
                    orgIDs = flow.FormDic[this.MsgOrgIDsFromField];
                }
                if (string.IsNullOrEmpty(roleIDs) && !string.IsNullOrEmpty(this.MsgRoleIDsFromField))
                {
                    roleIDs = flow.FormDic[this.MsgRoleIDsFromField];
                }

                if (!string.IsNullOrEmpty(this.MsgOrgIDFromUser) && string.IsNullOrEmpty(orgIDs))
                {
                    var user = FormulaHelper.GetUserInfo();
                    orgIDs = user.UserOrgID;
                }


                if (!string.IsNullOrEmpty(roleIDs))
                {
                    IRoleService roleService = FormulaHelper.GetService<IRoleService>();
                    string userIDs = roleService.GetUserIDsInRoles(roleIDs, orgIDs);

                    //2018-1-30 剥离项目角色选人功能
                    var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                    if (!string.IsNullOrEmpty(prjRoleUser))
                        userIDs = (prjRoleUser + "," + userIDs).Trim(',');

                    userIdList.AddRange(userIDs.Split(','));
                }
                else if (!string.IsNullOrEmpty(orgIDs))
                {
                    IOrgService orgService = FormulaHelper.GetService<IOrgService>();
                    string userIDs = orgService.GetUserIDsInOrgs(orgIDs);
                    userIdList.AddRange(userIDs.Split(','));
                }

                #endregion

                if (this.MsgSendToTaskUser == "1")
                    userIdList.AddRange(nextUserIDs.Split(','));


                IUserService userService = FormulaHelper.GetService<IUserService>();
                IMessageService msgService = FormulaHelper.GetService<IMessageService>();
                string resutUserIDs = string.Join(",", userIdList.Distinct());
                resutUserIDs = resutUserIDs.Trim(' ', ',');
                string resultUserNames = userService.GetUserNames(resutUserIDs);

                var msgType = Config.MsgType.Normal;
                if (!string.IsNullOrEmpty(this.MsgType))
                    msgType = (MsgType)Enum.Parse(typeof(Config.MsgType), this.MsgType);

                string url = this.S_WF_InsDefFlow.FormUrl;
                if (url.Contains("?"))
                    url += "&";
                else
                    url += "?";
                url += "FuncType=View&ID=" + taskExec.S_WF_InsFlow.FormInstanceID;

                msgService.SendMsg(msg, msg, url, "", resutUserIDs, resultUserNames, null, MsgReceiverType.UserType, msgType);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }
        #endregion

        #region 获取表单的数据库连接和表名称

        private string GetFormConnName()
        {
            if (string.IsNullOrEmpty(this.S_WF_InsDefStep.SubFormID))
                return this.S_WF_InsDefFlow.ConnName;
            else
            {
                var subForm = FormulaHelper.GetEntities<WorkflowEntities>().S_WF_DefSubForm.SingleOrDefault(c => c.ID == this.S_WF_InsDefStep.SubFormID);
                return subForm.ConnName;
            }
        }

        private string GetFormTableName()
        {
            if (string.IsNullOrEmpty(this.S_WF_InsDefStep.SubFormID))
                return this.S_WF_InsDefFlow.TableName;
            else
            {
                var subForm = FormulaHelper.GetEntities<WorkflowEntities>().S_WF_DefSubForm.SingleOrDefault(c => c.ID == this.S_WF_InsDefStep.SubFormID);
                return subForm.TableName;
            }
        }

        #endregion

    }

    /// <summary>
    /// 签字对象
    /// </summary>    
    internal class SignatureObj
    {
        public string TaskUserGroupName = "";
        public string TaskUserID = "";
        public string TaskUserName = "";
        public string ExecUserID = "";
        public string ExecUserName = "";
        public string RoutingCode = "";
        public string RoutingName = "";
        public string RoutingValue = "";
        public string SignTime = "";
        public string SignComment = "";
        public string SignatureDivID = "";
        public Dictionary<string, object> SignatureProtectField { get; set; }
        public string SignatureProtectValue = "";
        public string StepName = "";
        public string StepCode = "";

        public SignatureObj()
        {
            SignatureProtectField = new Dictionary<string, object>();
        }
    }

}
