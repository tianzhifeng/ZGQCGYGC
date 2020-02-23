using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using System.IO;
using System.Text;
using Base.Logic.BusinessFacade;
using Formula.Helper;
using Formula;
using Base.Logic.Domain;
using MvcAdapter;

namespace Base.Areas.UI.Controllers
{
    public class ExportSqlController : BaseController
    {
        public FileResult SqlFile(string defID, string fileCode, string tableName)
        {
            string sql = string.Format("select * from {1} where ID in('{0}')", defID.Replace(",", "','"), tableName);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            //表单定义和word模板同时导出历史版本
            if (tableName.ToUpper() == "S_UI_FORM" || tableName.ToUpper() == "S_UI_WORD")
            {
                var codes = string.Join("','", dt.AsEnumerable().Select(c => c["Code"].ToString()).ToArray());
                sql = string.Format("select * from {1} where Code in('{0}')", codes, tableName);
                dt = sqlHelper.ExecuteDataTable(sql);
            }



            StringBuilder result = new StringBuilder(); ;
            if (Config.Constant.IsOracleDb)
            {
                //声明变量
                result.Append("DECLARE");
                result.AppendLine();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.DataType == typeof(string))
                    {
                        result.AppendFormat("{0} nclob;", col.ColumnName);
                        result.AppendLine();
                    }
                }
                result.AppendFormat("begin"); //begin

                result.AppendFormat(SQLHelper.CreateUpdateSql(tableName, dt));

                result.AppendFormat("end");
            }
            else
            {
                #region SqlServer版

                result.AppendLine(String.Format("USE [{0}] \n", sqlHelper.DbName));
                result.AppendLine(SQLHelper.CreateUpdateSql(tableName, dt));

                //如果导出的是表单，则增加生成表的Sql
                if (tableName == "S_UI_Form")
                {
                    UIFO uiFO = new UIFO();

                    StringBuilder sb = new StringBuilder("\n\n");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(String.Format("USE [{0}_{1}]", sqlHelper.DbName.Split('_').First(), row["ConnName"]));
                        sb.AppendLine();
                        sb.AppendLine(uiFO.CreateReleaseFormSql(row["ID"].ToString()));
                    }

                    result.AppendLine(sb.ToString());
                }

                #endregion
            }



            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(result.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", fileCode + ".sql");
        }

        public FileResult ExportSql(string categoryFullID, string startDate)
        {
            var key = FormulaHelper.CreateGuid();

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(@"
--本SQL使用方法：
--1、冲突确认，即执行后返回的列表信息即冲突信息；
--2、在系统中处理冲突；
--3、再次执行本sql；
");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper flowSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string preDBName = sqlHelper.DbName.Split('_').First();

            sb.AppendFormat("USE [{0}]", sqlHelper.DbName);
            sb.AppendLine();
            sb.AppendFormat(@"
--SQL只能执行一次
if exists (select ID from S_UI_ModifyLogLog where ID='{0}' and IOType='导入')
begin
    select '本SQL已经执行过，不能再次执行！'
	return
end
", key);
            sb.AppendLine();

            string sql = "select * from S_M_Category where FullID like '" + categoryFullID + "%'";
            var dtCategory = sqlHelper.ExecuteDataTable(sql);

            //模块名称
            string categoryName = dtCategory.AsEnumerable().SingleOrDefault(c => c["FullID"].ToString() == categoryFullID)["Name"].ToString();

            if (categoryFullID != "0")
            {
                var parentCategory = sqlHelper.ExecuteDataTable("select * from S_M_Category where ID='" + dtCategory.AsEnumerable().SingleOrDefault(c => c["FullID"].ToString() == categoryFullID)["ParentID"].ToString() + "'");
                sb.AppendFormat(@"
--模块不存在，不能导入
if not exists (select ID from S_M_Category where ID='{0}')
begin
    select '模块“{1}”不存在，不能导入！'
	return
end
", parentCategory.Rows[0]["ID"], parentCategory.Rows[0]["Name"]);
            }

            sb.AppendLine();
            sb.Append("--更新模块开始");
            sb.AppendFormat(SQLHelper.CreateUpdateSql("S_M_Category", dtCategory));
            sb.Append("--更新模块结束");
            sb.AppendLine();
            sb.AppendLine();

            sql = string.Format("select * from {2} where CategoryID in('{0}') and ModifyTime>'{1}'", string.Join("','", dtCategory.AsEnumerable().Select(c => c["ID"].ToString()).ToArray()), startDate, "{0}");
            var dtForm = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_Form"));
            var dtList = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_List"));
            var dtPivot = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_Pivot"));
            var dtSelector = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_Selector"));
            var dtWord = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_Word"));
            var dtExcelPrint = sqlHelper.ExecuteDataTable(string.Format(sql, "S_UI_ExcelPrint"));
            var dtReport = sqlHelper.ExecuteDataTable(string.Format(sql, "S_R_Define"));
            var dtEnum = sqlHelper.ExecuteDataTable(string.Format(sql, "S_M_EnumDef"));
            var dtWorkflow = sqlHelper.ExecuteDataTable(string.Format(sql, flowSqlHelper.DbName + ".dbo.S_WF_DefFlow"));

            sb.AppendLine();
            sb.AppendFormat("update S_UI_Form set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtForm.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_List set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtList.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Pivot set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtPivot.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Selector set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtSelector.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Word set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtWord.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_ExcelPrint set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtExcelPrint.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_R_Define set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtReport.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update S_M_EnumDef set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtEnum.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key);
            sb.AppendLine();
            sb.AppendFormat("update {3}.dbo.S_WF_DefFlow set Collision='Collision' where (Collision is null or Collision not in('{2}','keep','cover')) and ID in('{0}') and ModifyTime > '{1}'", string.Join("','", dtWorkflow.AsEnumerable().Select(c => c["ID"]).ToArray()), startDate, key, flowSqlHelper.DbName);



            sb.AppendLine();
            //如果存在日志修改记录，则停止执行sql
            sb.AppendLine();
            sb.AppendFormat(@"
--如果存在冲突则终止执行sql
if exists (select ID from S_UI_Form where Collision = 'Collision'
union
select ID from S_UI_List where Collision = 'Collision'
union
select ID from S_UI_Pivot where Collision = 'Collision'
union
select ID from S_UI_Selector where Collision = 'Collision'
union
select ID from S_UI_Word where Collision = 'Collision'
union
select ID from S_UI_ExcelPrint where Collision = 'Collision'
union
select ID from S_R_Define where Collision = 'Collision'
union
select ID from S_M_EnumDef where Collision = 'Collision'
union
select ID from {0}.dbo.S_WF_DefFlow where Collision = 'Collision')
begin
    select '请先处理冲突！'
	return
end
", flowSqlHelper.DbName);


            //表单           
            UIFO uiFO = new UIFO();
            foreach (DataRow row in dtForm.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_Form where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_Form where ID='{1}'
    {2}
    {3}
    {4}
end
", key
 , row["ID"]
, SQLHelper.CreateInsertSql("S_UI_Form", row)
, string.Format("USE [{0}_{1}]", preDBName, row["ConnName"])
, uiFO.CreateReleaseFormSql(row["ID"].ToString())
);
                sb.AppendFormat("USE [{0}_Base]", preDBName);
                sb.AppendLine();
            }

            //列表
            foreach (DataRow row in dtList.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_List where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_List where ID='{1}'
    {2}
end
", key, row["ID"], SQLHelper.CreateInsertSql("S_UI_List", row));
            }

            //Word            
            foreach (DataRow row in dtWord.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_Word where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_Word where ID='{1}'
    {2}
end
", key, row["ID"], SQLHelper.CreateInsertSql("S_UI_Word", row));
            }

            //ExcelPrint          
            foreach (DataRow row in dtExcelPrint.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_ExcelPrint where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_ExcelPrint where ID='{1}'
    {2}
end
", key, row["ID"], SQLHelper.CreateInsertSql("S_UI_ExcelPrint", row));
            }

            //选择器          
            foreach (DataRow row in dtSelector.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_Selector where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_Selector where ID='{1}'
    {2}
end
", key, row["ID"], SQLHelper.CreateInsertSql("S_UI_Selector", row));
            }

            //透视表            
            foreach (DataRow row in dtPivot.Rows)
            {
                sb.AppendFormat(@"
if not exists (select * from S_UI_Pivot where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_UI_Pivot where ID='{1}'
    {2}
end
", key, row["ID"], SQLHelper.CreateInsertSql("S_UI_Pivot", row));
            }

            //报表            
            foreach (DataRow row in dtReport.Rows)
            {
                var dtReportSub = sqlHelper.ExecuteDataTable(string.Format("select * from S_R_DataSet where DefineID in('{0}')", row["ID"]));
                var dtReportSubSub = sqlHelper.ExecuteDataTable(string.Format("select * from S_R_Field where DataSetID in('{0}')", string.Join("','", dtReportSub.AsEnumerable().Select(c => c["ID"].ToString()))));


                sb.AppendFormat(@"
if not exists (select * from S_R_Define where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_R_Define where ID='{1}'
    {2}
    {3}
    {4}
end
", key
 , row["ID"]
 , SQLHelper.CreateInsertSql("S_R_Define", row)
 , SQLHelper.CreateInsertSql("S_R_DataSet", dtReportSub)
 , SQLHelper.CreateInsertSql("S_R_Field", dtReportSubSub)
 );
            }

            //枚举          
            foreach (DataRow row in dtEnum.Rows)
            {
                var dtEnumSub = sqlHelper.ExecuteDataTable(string.Format("select * from S_M_EnumItem where EnumDefID in('{0}')", row["ID"]));
                sb.AppendFormat(@"
if not exists (select * from S_M_EnumDef where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_M_EnumDef where ID='{1}'
    {2}
    {3}
end
", key
 , row["ID"]
 , SQLHelper.CreateInsertSql("S_M_EnumDef", row)
 , SQLHelper.CreateInsertSql("S_M_EnumItem", dtEnumSub));
            }

            //流程
            sb.AppendLine();
            sb.AppendFormat("USE [{0}]", flowSqlHelper.DbName);
            sb.AppendLine();
            foreach (DataRow row in dtWorkflow.Rows)
            {
                sql = string.Format("select * from S_WF_DefStep where DefFlowID='{0}'", row["ID"]);
                DataTable dtDefStep = flowSqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefRouting where DefFlowID='{0}'", row["ID"]);
                DataTable dtDefRouting = flowSqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefSubForm where DefFlowID='{0}'", row["ID"]);
                DataTable dtDefSubForm = flowSqlHelper.ExecuteDataTable(sql);


                sb.AppendFormat(@"
if not exists (select * from S_WF_DefFlow where Collision in('keep','{0}') and ID ='{1}')
begin
    delete S_WF_DefFlow where ID='{1}'
    {2}
    {3}
    {4}
    {5}
end
", key
 , row["ID"]
 , SQLHelper.CreateInsertSql("S_WF_DefFlow", row)
 , SQLHelper.CreateInsertSql("S_WF_DefStep", dtDefStep)
 , SQLHelper.CreateInsertSql("S_WF_DefRouting", dtDefRouting)
 , SQLHelper.CreateInsertSql("S_WF_DefSubForm", dtDefSubForm)
);
            }

            sb.AppendFormat("USE [{0}]", sqlHelper.DbName);

            //记录更新
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendFormat("【表单名称】{0}\n", dtForm.Rows.Count == 0 ? "无" : string.Join(",", dtForm.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【列表名称】{0}\n", dtList.Rows.Count == 0 ? "无" : string.Join(",", dtList.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【流程名称】{0}\n", dtWorkflow.Rows.Count == 0 ? "无" : string.Join(",", dtWorkflow.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【报表名称】{0}\n", dtReport.Rows.Count == 0 ? "无" : string.Join(",", dtReport.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【选择器名称】{0}\n", dtSelector.Rows.Count == 0 ? "无" : string.Join(",", dtSelector.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【透视表名称】{0}\n", dtPivot.Rows.Count == 0 ? "无" : string.Join(",", dtPivot.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【Word导出名称】{0}\n", dtWord.Rows.Count == 0 ? "无" : string.Join(",", dtWord.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));
            sb1.AppendFormat("【枚举名称】{0}\n", dtEnum.Rows.Count == 0 ? "无" : string.Join(",", dtEnum.AsEnumerable().Select(c => c["Name"].ToString()).ToArray()));

            S_UI_ModifyLogLog log = new S_UI_ModifyLogLog();
            log.ID = key;
            log.IOTime = DateTime.Now;
            log.IOType = "导出";
            log.CategoryName = categoryName;
            log.RelateData = sb1.ToString();
            entities.Set<S_UI_ModifyLogLog>().Add(log);
            entities.SaveChanges();

            sb.AppendLine();
            sb.AppendFormat(@"
delete S_UI_ModifyLogLog where ID='{0}'
insert into S_UI_ModifyLogLog ([ID],[IOTime],[IOType],[RelateData],[CategoryName]) values ('{0}','{1}','导入','{2}','{3}')
", key, DateTime.Now, sb1.ToString(), categoryName);


            //更新冲突状态为完成
            sb.AppendFormat("USE [{0}]", sqlHelper.DbName);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Form set Collision='{1}' where ID in('{0}')", string.Join("','", dtForm.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_List set Collision='{1}' where ID in('{0}')", string.Join("','", dtList.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Pivot set Collision='{1}' where ID in('{0}')", string.Join("','", dtPivot.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Selector set Collision='{1}' where ID in('{0}')", string.Join("','", dtSelector.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_UI_Word set Collision='{1}' where ID in('{0}')", string.Join("','", dtWord.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_R_Define set Collision='{1}' where ID in('{0}')", string.Join("','", dtReport.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update S_M_EnumDef set Collision='{1}' where ID in('{0}')", string.Join("','", dtEnum.AsEnumerable().Select(c => c["ID"].ToString())), key);
            sb.AppendLine();
            sb.AppendFormat("update {2}.dbo.S_WF_DefFlow set Collision='{1}' where ID in('{0}')", string.Join("','", dtWorkflow.AsEnumerable().Select(c => c["ID"].ToString())), key, flowSqlHelper.DbName);


            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "base" + startDate + "更新脚本.sql");
        }


        #region 冲突处理

        public JsonResult GetCollisionList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'表单' as UIType from S_UI_Form where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'列表' as UIType from S_UI_List where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'透视表' as UIType from S_UI_Pivot where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'选择器' as UIType from S_UI_Selector where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'Word导出' as UIType from S_UI_Word where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'报表' as UIType from S_R_Define where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'枚举' as UIType from S_M_EnumDef where Collision in ('Collision','keep','cover')
union
select ID,Code,Name,ModifyTime,ModifyUserName,Collision,'流程' as UIType from {0}_Workflow.dbo.S_WF_DefFlow where Collision in ('Collision','keep','cover')
";
            sql = string.Format(sql, sqlHelper.DbName.Split('_').First());
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult CollisionKeep(string listIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"
update S_UI_Form set Collision = 'keep' where ID in ('{0}')
update S_UI_List set Collision = 'keep' where ID in ('{0}')
update S_UI_Pivot set Collision = 'keep' where ID in ('{0}')
update S_UI_Selector set Collision = 'keep' where ID in ('{0}')
update S_UI_Word set Collision = 'keep' where ID in ('{0}')
update S_R_Define set Collision = 'keep' where ID in ('{0}')
update S_M_EnumDef set Collision = 'keep' where ID in ('{0}')
update {1}_Workflow.dbo.S_WF_DefFlow set Collision = 'keep' where ID in ('{0}')
";
            sql = string.Format(sql, listIDs.Replace(",", "','"), sqlHelper.DbName.Split('_').First());

            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult CollisionCover(string listIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"
update S_UI_Form set Collision = 'cover' where ID in ('{0}')
update S_UI_List set Collision = 'cover' where ID in ('{0}')
update S_UI_Pivot set Collision = 'cover' where ID in ('{0}')
update S_UI_Selector set Collision = 'cover' where ID in ('{0}')
update S_UI_Word set Collision = 'cover' where ID in ('{0}')
update S_R_Define set Collision = 'cover' where ID in ('{0}')
update S_M_EnumDef set Collision = 'cover' where ID in ('{0}')
update {1}_Workflow.dbo.S_WF_DefFlow set Collision = 'cover' where ID in ('{0}')
";
            sql = string.Format(sql, listIDs.Replace(",", "','"), sqlHelper.DbName.Split('_').First());

            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult CollisionClear(string listIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"
update S_UI_Form set Collision = null where ID in ('{0}')
update S_UI_List set Collision = null where ID in ('{0}')
update S_UI_Pivot set Collision = null where ID in ('{0}')
update S_UI_Selector set Collision = null where ID in ('{0}')
update S_UI_Word set Collision = null where ID in ('{0}')
update S_R_Define set Collision = null where ID in ('{0}')
update S_M_EnumDef set Collision = null where ID in ('{0}')
update {1}_Workflow.dbo.S_WF_DefFlow set Collision = null where ID in ('{0}')
";
            sql = string.Format(sql, listIDs.Replace(",", "','"), sqlHelper.DbName.Split('_').First());

            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        #endregion

        #region 导入导出日志

        public JsonResult GetModifyLogLogList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "select * from S_UI_ModifyLogLog";
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        #endregion
    }
}
