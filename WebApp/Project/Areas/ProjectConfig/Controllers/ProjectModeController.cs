using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Formula.Helper;


namespace Project.Areas.ProjectConfig.Controllers
{
    public class ProjectModeController : BaseConfigController<S_T_ProjectMode>
    {
        public override JsonResult Save()
        {
            var ety = this.UpdateEntity<S_T_ProjectMode>();

            string json = Request.Form["FormData"];
            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(json);
            var extDic = new Dictionary<string, object>();
            foreach (var item in formDic)
            {
                if (item.Key.StartsWith("Ext_"))
                    extDic.Add(item.Key, item.Value);
            }
            ety.ExtentionJson = JsonHelper.ToJson<Dictionary<string, object>>(extDic);

            ety.AddRootDBSDefine();
            return base.JsonSave<S_T_ProjectMode>(ety);
        }

        protected override void BeforeDelete(List<S_T_ProjectMode> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }

        public JsonResult Publish(string ModeCode)
        {
            var projectMode = BaseConfigFO.GetMode(ModeCode);
            if (projectMode != null)
                projectMode.Pubish();
            return Json("");
        }

        public JsonResult DoCopy(string ModeCode)
        {
            if (string.IsNullOrEmpty(ModeCode))
                throw new Formula.Exceptions.BusinessException("请选择要复制的模版");

            var projectMode = BaseConfigFO.GetMode(ModeCode);
            projectMode.DoCopy();
            entities.SaveChanges();

            return Json("");
        }

        public FileResult ExportSql(string IDs)
        {
            StringBuilder result = new StringBuilder();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);

            if (Config.Constant.IsOracleDb)
            {
            }
            else
            {
                result.AppendFormat("USE [{0}] \n", sqlHelper.DbName);

                //先删除
                result.AppendFormat("delete from  S_T_ProjectMode where ID in('{0}') \n", IDs.Replace(",", "','"));

                //S_T_ProjectMode
                string sql = string.Format("select * from {1} where ID in('{0}')", IDs.Replace(",", "','"), "S_T_ProjectMode");
                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_ProjectMode", dt));
                result.AppendLine();

                //S_T_SpaceDefine
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_SpaceDefine");
                DataTable dtS_T_SpaceDefine = sqlHelper.ExecuteDataTable(sql);
                //S_T_SpaceAuth
                string subIDs = string.Empty;
                foreach (DataRow row in dtS_T_SpaceDefine.Rows)
                    subIDs += row["ID"].ToString() + ",";
                subIDs = subIDs.TrimEnd(',');
                sql = string.Format("select * from {1} where SpaceID in('{0}')", subIDs.Replace(",", "','"), "S_T_SpaceAuth");
                DataTable dtS_T_SpaceAuth = sqlHelper.ExecuteDataTable(sql);

                result.Append(SQLHelper.CreateUpdateSql("S_T_SpaceDefine", dtS_T_SpaceDefine));
                result.AppendLine();
                result.Append(SQLHelper.CreateUpdateSql("S_T_SpaceAuth", dtS_T_SpaceAuth));
                result.AppendLine();

                //S_T_WBSStructInfo
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_WBSStructInfo");
                DataTable dtS_T_WBSStructInfo = sqlHelper.ExecuteDataTable(sql);
                //S_T_WBSStructRole
                subIDs = string.Empty;
                foreach (DataRow row in dtS_T_WBSStructInfo.Rows)
                    subIDs += row["ID"].ToString() + ",";
                subIDs = subIDs.TrimEnd(',');
                sql = string.Format("select * from {1} where WBSStructID in('{0}')", subIDs.Replace(",", "','"), "S_T_WBSStructRole");
                DataTable dtS_T_WBSStructRole = sqlHelper.ExecuteDataTable(sql);

                result.Append(SQLHelper.CreateUpdateSql("S_T_WBSStructInfo", dtS_T_WBSStructInfo));
                result.AppendLine();
                result.Append(SQLHelper.CreateUpdateSql("S_T_WBSStructRole", dtS_T_WBSStructRole));
                result.AppendLine();

                //S_T_DBSDefine
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_DBSDefine");
                DataTable dtS_T_DBSDefine = sqlHelper.ExecuteDataTable(sql);
                //S_T_DBSSecurity
                subIDs = string.Empty;
                foreach (DataRow row in dtS_T_DBSDefine.Rows)
                    subIDs += row["ID"].ToString() + ",";
                subIDs = subIDs.TrimEnd(',');
                sql = string.Format("select * from {1} where DBSDefineID in('{0}')", subIDs.Replace(",", "','"), "S_T_DBSSecurity");
                DataTable dtS_T_DBSSecurity = sqlHelper.ExecuteDataTable(sql);

                result.Append(SQLHelper.CreateUpdateSql("S_T_DBSDefine", dtS_T_DBSDefine));
                result.AppendLine();
                result.Append(SQLHelper.CreateUpdateSql("S_T_DBSSecurity", dtS_T_DBSSecurity));
                result.AppendLine();

                //S_T_QBSTemplate
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_QBSTemplate");
                DataTable dtS_T_QBSTemplate = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_QBSTemplate", dtS_T_QBSTemplate));
                result.AppendLine();

                //S_T_MileStone
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_MileStone");
                DataTable dtS_T_MileStone = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_MileStone", dtS_T_MileStone));
                result.AppendLine();

                //S_T_ISODefine
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_ISODefine");
                DataTable dtS_T_ISODefine = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_ISODefine", dtS_T_ISODefine));
                result.AppendLine();

                //S_T_FlowTraceDefine
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_FlowTraceDefine");
                DataTable dtS_T_FlowTraceDefine = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_FlowTraceDefine", dtS_T_FlowTraceDefine));
                result.AppendLine();

                //S_T_AuditMode
                sql = string.Format("select * from {1} where ProjectModeID in('{0}')", IDs.Replace(",", "','"), "S_T_AuditMode");
                DataTable dtS_T_AuditMode = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_AuditMode", dtS_T_AuditMode));
                result.AppendLine();

                //S_T_DataAuth
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_DataAuth");
                DataTable dtS_T_DataAuth = sqlHelper.ExecuteDataTable(sql);
                result.Append(SQLHelper.CreateUpdateSql("S_T_DataAuth", dtS_T_DataAuth));
                result.AppendLine();

                //S_T_ToDoListDefine
                sql = string.Format("select * from {1} where ModeID in('{0}')", IDs.Replace(",", "','"), "S_T_ToDoListDefine");
                DataTable dtS_T_ToDoListDefine = sqlHelper.ExecuteDataTable(sql);
                //S_T_WBSStructRole
                subIDs = string.Empty;
                foreach (DataRow row in dtS_T_WBSStructInfo.Rows)
                    subIDs += row["ID"].ToString() + ",";
                subIDs = subIDs.TrimEnd(',');
                sql = string.Format("select * from {1} where DefineID in('{0}')", subIDs.Replace(",", "','"), "S_T_ToDoListDefineNode");
                DataTable dtS_T_ToDoListDefineNode = sqlHelper.ExecuteDataTable(sql);

                result.Append(SQLHelper.CreateUpdateSql("S_T_ToDoListDefine", dtS_T_ToDoListDefine));
                result.AppendLine();
                result.Append(SQLHelper.CreateUpdateSql("S_T_ToDoListDefineNode", dtS_T_ToDoListDefineNode));
                result.AppendLine();
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.Default.GetBytes(result.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "ProjectModes.sql");
        }

    }
}
