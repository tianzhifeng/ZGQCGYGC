using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using Formula.Helper;
using System.Text;
using System.Data;
using Formula.Exceptions;
using Formula;

namespace Base.Areas.UI.Controllers
{
    public class FreePivotInstanceController : BaseController<S_UI_FreePivotInstance>
    {
        #region 获取树

        public override JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_FreePivot where Code='{0}'", Request["TmplCode"]);
            var dtDef = sqlHelper.ExecuteDataTable(sql);
            if (dtDef.Rows.Count == 0)
                return Json("");
            sql = string.Format("select ID,FreePivotID,ParentID,FullID,Name,case when SQL is null or datalength(SQL)=0 then 'Category' else 'Instance' end Type from S_UI_FreePivotInstance where FullID like '%{0}%' order by SortIndex", dtDef.Rows[0]["ID"]);

            var dt = sqlHelper.ExecuteDataTable(sql);

            var row = dt.NewRow();
            var defRow = dtDef.Rows[0];
            row["ID"] = defRow["ID"];
            row["FreePivotID"] = defRow["ID"];
            row["ParentID"] = "";
            row["FullID"] = defRow["ID"];
            row["Name"] = defRow["Name"];
            row["Type"] = "Category";
            dt.Rows.InsertAt(row, 0);
            return Json(dt);
        }

        #endregion

        #region 获取数据源列表

        public JsonResult GetDataSourceList()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_FreePivot where Code='{0}'", Request["TmplCode"]);
            var dtDef = sqlHelper.ExecuteDataTable(sql);

            if (dtDef.Rows.Count == 0 || dtDef.Rows[0]["DataSource"].ToString().Trim() == "")
                return Json("");

            var dataSource = dtDef.Rows[0]["DataSource"].ToString();
            var list = JsonHelper.ToList(dataSource);
            var result = list.Select(c => new { ID = c["Name"], Name = c["Name"] });
            return Json(result);
        }

        #endregion

        #region 根据数据源名称生成SQL

        public JsonResult CreateSQL()
        {
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_FreePivot where Code='{0}'", Request["TmplCode"]);
            var dtDef = baseSqlHelper.ExecuteDataTable(sql);
            if (dtDef.Rows.Count == 0 || dtDef.Rows[0]["DataSource"].ToString().Trim() == "")
                Json(new { SQL = string.Format("自由透视表不存在：{0}", Request["TmplCode"]) });

            var strDataSource = dtDef.Rows[0]["DataSource"].ToString();
            var listDataSourceJson = JsonHelper.ToList(strDataSource);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(dtDef.Rows[0]["ConnName"].ToString());

            if (Request["DataSourceName"].Contains(',') == false)
            {
                string SQL = listDataSourceJson.SingleOrDefault(c => c["Name"].ToString().Trim() == Request["DataSourceName"].Trim())["Sql"].ToString();

                var dtField = sqlHelper.ExecuteDataTable(string.Format("select * from ({0}) dt where 1=2", SQL));
                string fields = "";
                foreach (DataColumn col in dtField.Columns)
                {
                    if (col.ColumnName.EndsWith("ID")) continue;
                    fields += "," + col.ColumnName;
                }
                fields = fields.Trim(',');

                return Json(new { SQL = string.Format("select distinct {0} from ({1}) dt", fields, SQL) });
            }

            List<DefInfo> list = new List<DefInfo>();
            foreach (var item in Request["DataSourceName"].Split(','))
            {
                var dataSource = listDataSourceJson.SingleOrDefault(c => c["Name"].ToString().Trim() == item.Trim());
                sql = dataSource["Sql"].ToString();
                var dt = sqlHelper.ExecuteDataTable(string.Format("select * from({0}) dt where 1=2", sql));
                DefInfo info = new DefInfo();
                info.Name = dataSource["Name"].ToString();
                info.Sql = sql;
                info.DT = dt;
                list.Add(info);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
 from
 ({0}) 
 {1}
", list[0].Sql, list[0].Name);


            List<DefInfo> deal = new List<DefInfo>();

            //处理完毕
            deal.Add(list[0]);
            list.RemoveAt(0);



            for (int i = 0; i < deal.Count && list.Count > 0; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (deal[i].DT.Columns.Contains(list[j].Name + "ID"))
                    {
                        //生成sql
                        sb.AppendFormat(@"
 left join
 ({0}) 
 {1} on {1}.{1}ID={2}.{1}ID
", list[j].Sql, list[j].Name, deal[i].Name);
                        //处理完成
                        deal.Add(list[j]);
                        list.RemoveAt(j);
                        j = j - 1;
                    }
                }
            }

            //如果有数据源没有办法外键关联,则返回异常信息
            if (list.Count > 0)
                Json(new { SQL = string.Format("没有合适的外键：", string.Join(",", list.Select(c => c.Name))) });


            List<string> listFields = new List<string>();
            foreach (var obj in deal)
            {
                foreach (DataColumn col in obj.DT.Columns)
                {
                    if (col.ColumnName.EndsWith("ID"))
                        continue;
                    listFields.Add(string.Format("{0}.{1} {2}", obj.Name, col.ColumnName, col.ColumnName.StartsWith(obj.Name) ? col.ColumnName : obj.Name + col.ColumnName));
                }
            }

            string result = string.Format("select distinct {0} ", string.Join(",", listFields)) + sb.ToString();

            return Json(new { SQL = result });
        }

        #endregion

        public JsonResult ImportField(string id)
        {
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select SQL,ConnName,Enum from S_UI_FreePivotInstance join S_UI_FreePivot on S_UI_FreePivotInstance.FullID like S_UI_FreePivot.ID+'%' where S_UI_FreePivotInstance.ID='{0}'", id);
            var dt = baseSqlHelper.ExecuteDataTable(sql);
            if (dt.Rows[0]["SQL"].ToString().Trim() == "")
                throw new BusinessException("透视表SQL不能为空！");

            //枚举定义
            var enumJsonList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dt.Rows[0]["Enum"].ToString());

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(dt.Rows[0]["ConnName"].ToString());
            sql = string.Format("select * from ({0}) dt where 1=2", dt.Rows[0]["SQL"]);
            dt = sqlHelper.ExecuteDataTable(sql);

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (DataColumn col in dt.Columns)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["Visible"] = "1";
                dic["FieldName"] = col.ColumnName;
                dic["Caption"] = col.ColumnName;

                var EnumObj = enumJsonList.SingleOrDefault(c => c["FieldName"] == col.ColumnName);
                if (EnumObj != null)
                    dic["Enum"] = EnumObj["EnumKey"];
                else
                    dic["Enum"] = "";

                list.Add(dic);
            }
            return Json(list);
        }
    }

    class DefInfo
    {
        public string Name { get; set; }
        public string Sql { get; set; }
        public DataTable DT { get; set; }
        public int SortIndex { get; set; }
    }
}
