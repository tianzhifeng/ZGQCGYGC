using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using MvcAdapter;
using Formula.Helper;
using System.Data;
using System.Text;
using Formula.Exceptions;
using Base.Logic.BusinessFacade;
using Formula;

namespace Base.Areas.Meta.Controllers
{
    public class ConfigManageController : BaseController<S_M_ConfigManage>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            var CategoryID = Request["CategoryID"];
            if (string.IsNullOrEmpty(CategoryID))
                return base.GetList(qb);
            else
            {
                var list = entities.Set<S_M_Category>().Where(c => c.ID == CategoryID || c.ParentID == CategoryID).Select(c => c.ID);
                var data = entities.Set<S_M_ConfigManage>().Where(d => list.Contains(d.CategoryID)).WhereToGridData(qb);
                return Json(data);
            }
        }

        public JsonResult GetUITree(string DbServerAddr, string DbName, string DbLoginName, string DbPassword)
        {
            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName + "_Base", DbLoginName, DbPassword);
            SQLHelper sqlHelper = new SQLHelper(strConn);
            var dt = sqlHelper.ExecuteDataTable("select * from S_M_Category");
            return Json(dt);
        }

        public JsonResult GetUIList(string DbServerAddr, string DbName, string DbLoginName, string DbPassword, QueryBuilder qb)
        {
            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName + "_Base", DbLoginName, DbPassword);

            SQLHelper sqlHelper = new SQLHelper(strConn);

            string categoryID = Request["CategoryID"];
            if (!string.IsNullOrEmpty(categoryID))
            {
                string strSql = string.Format("select ID from S_M_Category where ID='{0}' or ParentID='{0}'", categoryID);
                var dt = sqlHelper.ExecuteDataTable(strSql);
                string ids = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            string sql = @"
select 
dt3.ID,dt3.Code,dt3.Name,ModifyUserName,ModifyTime,CategoryID,Type
from (	
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='Form' from S_UI_Form 
union
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='List' from S_UI_List
union
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='Selector' from S_UI_Selector
union
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='Word' from S_UI_Word
union
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='Report' from S_R_Define
union
select ID,Code,Name,'' as ModifyUserName,null as ModifyTime,CategoryID,Type='Enum' from S_M_EnumDef
union
select ID,Code,Name,ModifyUserName,ModifyTime,CategoryID,Type='Workflow' from FE_Workflow.dbo.S_WF_DefFlow
) dt3 
";

            sql = sql.Replace("FE_Workflow", DbName + "_Workflow");

            GridData result = null;
            //try
            //{
            result = sqlHelper.ExecuteGridData(sql, qb);
            //}
            //catch
            //{
            //    throw new BusinessException("无法连接目标数据库！");
            //}

            return Json(result);
        }

        public JsonResult GetImportConfig()
        {
            var entity = entities.Set<S_M_ConfigManage>().OrderByDescending(c => c.CreateTime).FirstOrDefault();
            if (entity == null)
                return Json("{}");
            return Json(entity);
        }

        public JsonResult ImportData(string listData, string connName, string categoryID)
        {
            var ids = GetValueString(listData, "ID");
            string sql = string.Format("select * from S_M_ConfigManage where ID in('{0}')", ids.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            var existIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
            var arrID = StringHelper.Exclude(ids, existIDs).Split(',');

            var list = JsonHelper.ToList(listData);
            list = list.Where(c => arrID.Contains(c["ID"].ToString())).ToList();

            var formDic = JsonHelper.ToObject(Request["formData"]);

            bool hasSurfix = false;
            if (formDic["HasSurfix"].ToString() == "1" || formDic["HasSurfix"].ToString() == "true")
                hasSurfix = true;

            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                string dbName = formDic["DbName"].ToString();

                string code = item["Code"].ToString();
                if (hasSurfix)
                    code = dbName + code;

                string str = string.Format(@"
INSERT INTO S_M_ConfigManage(ID,Code,Name,Type,DbServerAddr,DbName,DbLoginName,DbPassword,CreateTime,CategoryID,ConnName)
VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')
", item["ID"], code, item["Name"], item["Type"], formDic["DbServerAddr"], dbName, formDic["DbLoginName"], formDic["DbPassword"], DateTime.Now, categoryID, connName);
                sb.Append(str);
            }
            if (sb.ToString().Length > 0)
                sqlHelper.ExecuteNonQuery(sb.ToString());

            return Json("");
        }

        public JsonResult DoUpdate(string listData)
        {
            UIFO uiFO = new UIFO();

            var list = JsonHelper.ToList(listData);
            foreach (var item in list)
            {
                switch (item["Type"].ToString())
                {
                    case "Form":
                        DeleteWhereSaveCode("Base", "S_UI_Form", item["ID"].ToString(), item["Code"].ToString());
                        UpdateUI(item, "S_UI_Form");
                        //不生成数据库表
                        //uiFO.ReleaseForm(item["ID"].ToString()); 
                        RenameCode("Base", "S_UI_Form", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "List":
                        DeleteWhereSaveCode("Base", "S_UI_List", item["ID"].ToString(), item["Code"].ToString());
                        UpdateUI(item, "S_UI_List");
                        RenameCode("Base", "S_UI_List", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "Selector":
                        DeleteWhereSaveCode("Base", "S_UI_Selector", item["ID"].ToString(), item["Code"].ToString());
                        UpdateUI(item, "S_UI_Selector");
                        RenameCode("Base", "S_UI_Selector", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "Word":
                        DeleteWhereSaveCode("Base", "S_UI_Word", item["ID"].ToString(), item["Code"].ToString());
                        UpdateUI(item, "S_UI_Word");
                        RenameCode("Base", "S_UI_Word", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "Report":
                        DeleteWhereSaveCode("Base", "S_R_Define", item["ID"].ToString(), item["Code"].ToString());
                        UpdateReport(item);
                        RenameCode("Base", "S_R_Define", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "Workflow":
                        DeleteWhereSaveCode("Workflow", "S_WF_DefFlow", item["ID"].ToString(), item["Code"].ToString());
                        UpdateWorkflow(item);
                        RenameCode("Workflow", "S_WF_DefFlow", item["ID"].ToString(), item["Code"].ToString());
                        break;
                    case "Enum":
                        DeleteWhereSaveCode("Base", "S_M_EnumDef", item["ID"].ToString(), item["Code"].ToString());
                        UpdateEnum(item);
                        RenameCode("Base", "S_M_EnumDef", item["ID"].ToString(), item["Code"].ToString());
                        break;
                }
            }

            return Json("");
        }

        private void DeleteWhereSaveCode(string connName, string tableName, string id, string code)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            string sql = string.Format("delete from {0} where ID<>'{1}' and Code='{2}' ", tableName, id, code);//相同编号的删除
            sqlHelper.ExecuteNonQuery(sql);
        }

        private void RenameCode(string connName, string tableName, string id, string code)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            string sql = string.Format("select * from {0} where ID='{1}'", tableName, id);
            var dt = sqlHelper.ExecuteDataTable(sql);
            if (dt.Columns.Contains("ConnName"))
            {
                SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                sql = string.Format("select Code from S_M_Category where ID='{0}'", dt.Rows[0]["CategoryID"]);
                var obj = baseSqlHelper.ExecuteScalar(sql); //模块的connName

                sql = string.Format(" update {0} set Code='{1}',ConnName='{3}' where ID='{2}'", tableName, code, id, obj);
                sqlHelper.ExecuteNonQuery(sql);

            }
            else
            {               
                sql = string.Format(" update {0} set Code='{1}' where ID='{2}'", tableName, code, id);
                sqlHelper.ExecuteNonQuery(sql);
            }
        }

        private void UpdateUI(Dictionary<string, object> dic, string tableName)
        {
            string DbServerAddr = dic["DbServerAddr"].ToString();
            string DbName = dic["DbName"].ToString() + "_Base";
            string DbLoginName = dic["DbLoginName"].ToString();
            string DbPassword = dic["DbPassword"].ToString();

            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName, DbLoginName, DbPassword);
            SQLHelper srcSqlHelper = new SQLHelper(strConn);
            string sql = createSql(srcSqlHelper, dic["ID"].ToString(), tableName);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(sql);
            sqlHelper.ExecuteNonQuery(string.Format("update {0} set CategoryID='{1}' where ID='{2}'", tableName, dic["CategoryID"], dic["ID"]));

            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            baseHelper.ExecuteNonQuery(string.Format("update S_M_ConfigManage set SyncTime='{0}' where ID='{1}'", DateTime.Now, dic["ID"]));

            if (tableName == "S_UI_Form")//表单定义,同步元数据
            {
                var dtTable = srcSqlHelper.ExecuteDataTable(string.Format("select * from S_M_Table where ID='{0}'", dic["ID"]));
                var dtField = srcSqlHelper.ExecuteDataTable(string.Format("select * from S_M_Field where TableID='{0}'", dic["ID"]));
                if (dtTable.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("\n delete from S_M_Table where ID='{0}'", dic["ID"]);
                    var rowTable = dtTable.Rows[0];
                    sb.AppendFormat("\n insert into S_M_Table(ID,Code,Name,SortIndex,ConnName,Description) values('{0}','{1}','{2}','{3}','{4}','{5}')"
                        , rowTable["ID"], rowTable["Code"], rowTable["Name"], rowTable["SortIndex"], rowTable["ConnName"], rowTable["Description"]);

                    foreach (DataRow row in dtField.Rows)
                    {
                        sb.AppendFormat("\n insert into S_M_Field(ID,TableID,Code,Name,Type,SortIndex,EnumKey,Description) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
                            , row["ID"], row["TableID"], row["Code"], row["Name"], row["Type"], row["SortIndex"], row["EnumKey"], row["Description"]);
                    }
                    baseHelper.ExecuteNonQuery(sb.ToString());
                }

            }
        }

        private void UpdateReport(Dictionary<string, object> dic)
        {
            string DbServerAddr = dic["DbServerAddr"].ToString();
            string DbName = dic["DbName"].ToString() + "_Base";
            string DbLoginName = dic["DbLoginName"].ToString();
            string DbPassword = dic["DbPassword"].ToString();

            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName, DbLoginName, DbPassword);
            SQLHelper srcSqlHelper = new SQLHelper(strConn);
            string sql = createReportSql(srcSqlHelper, dic["ID"].ToString());
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(sql);
            sqlHelper.ExecuteNonQuery(string.Format("update {0} set CategoryID='{1}' where ID='{2}'", "S_R_Define", dic["CategoryID"], dic["ID"]));

            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            baseHelper.ExecuteNonQuery(string.Format("update S_M_ConfigManage set SyncTime='{0}' where ID='{1}'", DateTime.Now, dic["ID"]));
        }

        private void UpdateWorkflow(Dictionary<string, object> dic)
        {
            string DbServerAddr = dic["DbServerAddr"].ToString();
            string DbName = dic["DbName"].ToString() + "_Workflow";
            string DbLoginName = dic["DbLoginName"].ToString();
            string DbPassword = dic["DbPassword"].ToString();

            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName, DbLoginName, DbPassword);
            SQLHelper srcSqlHelper = new SQLHelper(strConn);
            string sql = createWorkflowSql(srcSqlHelper, dic["ID"].ToString());
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            sqlHelper.ExecuteNonQuery(sql);
            sqlHelper.ExecuteNonQuery(string.Format("update {0} set CategoryID='{1}' where ID='{2}'", "S_WF_DefFlow", dic["CategoryID"], dic["ID"]));

            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            baseHelper.ExecuteNonQuery(string.Format("update S_M_ConfigManage set SyncTime='{0}' where ID='{1}'", DateTime.Now, dic["ID"]));
        }

        private void UpdateEnum(Dictionary<string, object> dic)
        {
            string DbServerAddr = dic["DbServerAddr"].ToString();
            string DbName = dic["DbName"].ToString() + "_Base";
            string DbLoginName = dic["DbLoginName"].ToString();
            string DbPassword = dic["DbPassword"].ToString();

            string strConn = string.Format("data source={0};initial catalog={1};user id={2};password={3};", DbServerAddr, DbName, DbLoginName, DbPassword);
            SQLHelper srcSqlHelper = new SQLHelper(strConn);

            string sql = createEnumSql(srcSqlHelper, dic["ID"].ToString());
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            baseHelper.ExecuteNonQuery(sql);
            baseHelper.ExecuteNonQuery(string.Format("update {0} set CategoryID='{1}' where ID='{2}'", "S_M_EnumDef", dic["CategoryID"], dic["ID"]));

            baseHelper.ExecuteNonQuery(string.Format("update S_M_ConfigManage set SyncTime='{0}' where ID='{1}'", DateTime.Now, dic["ID"]));
        }

        private string createSql(SQLHelper sqlHelper, string defID, string tableName)
        {
            string sql = string.Format("select * from {1} where ID='{0}'", defID, tableName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            string result = string.Format("delete from {1} where ID='{0}' \n", defID, tableName);
            result += SQLHelper.CreateInsertSql(tableName, dt);
            return result;
        }

        private string createReportSql(SQLHelper sqlHelper, string defID)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format("select * from S_R_Define where ID='{0}'", defID);
            DataTable dtDefReport = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_R_DataSet where DefineID='{0}'", defID);
            DataTable dtDataSet = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select S_R_Field.* from S_R_Field join S_R_DataSet on S_R_DataSet.ID=DataSetID where DefineID='{0}'", defID);
            DataTable dtField = sqlHelper.ExecuteDataTable(sql);

            sb.AppendLine(string.Format("delete from S_R_Define where ID='{0}'", defID));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_Define", dtDefReport));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_DataSet", dtDataSet));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_R_Field", dtField));

            return sb.ToString();
        }

        private string createWorkflowSql(SQLHelper sqlHelper, string defID)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format("select * from S_WF_DefFlow where ID='{0}'", defID);
            DataTable dtDefFlow = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_WF_DefStep where DefFlowID='{0}'", defID);
            DataTable dtDefStep = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_WF_DefRouting where DefFlowID='{0}'", defID);
            DataTable dtDefRouting = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_WF_DefSubForm where DefFlowID='{0}'", defID);
            DataTable dtDefSubForm = sqlHelper.ExecuteDataTable(sql);

            sb.AppendLine(string.Format("delete from S_WF_DefFlow where ID='{0}'", defID));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefFlow", dtDefFlow));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefStep", dtDefStep));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefRouting", dtDefRouting));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefSubForm", dtDefSubForm));

            return sb.ToString();
        }

        private string createEnumSql(SQLHelper sqlHelper, string defID)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format("select * from S_M_EnumDef where ID='{0}'", defID);
            DataTable dtDefEnum = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_M_EnumItem where EnumDefID='{0}'", defID);
            DataTable dtEnumItem = sqlHelper.ExecuteDataTable(sql);

            sb.AppendLine(string.Format("delete from S_M_EnumDef where ID='{0}'", defID));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_M_EnumDef", dtDefEnum));
            sb.AppendLine(SQLHelper.CreateInsertSql("S_M_EnumItem", dtEnumItem));

            return sb.ToString();
        }
    }
}
