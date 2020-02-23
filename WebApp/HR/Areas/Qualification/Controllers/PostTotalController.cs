using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using MvcAdapter;
using Config;
using System.Data;
using Formula.Helper;
using Formula;
using Config.Logic;
/**
 * 岗位资格汇总审批流程
 * **/
namespace HR.Areas.Qualification.Controllers
{
    public class PostTotalController : HRFormContorllor<T_Qualification_PostTotal>
    {
        //
        // GET: /Qualification/PostTotal/
         protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string sql = "select count(1) userCount from T_Qualification_Postqualificationmanagement where isnull(IsApprove,'False')='False'";
               dt.Rows[0]["TotalCount"] = HRSQLDB.ExecuteScalar(sql);
            }

        }

        public ActionResult Index()
        {
            return View();
        }
        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            var bol = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);

            if (routing.Code == "ToHR")
            {
                var formID = taskExec.S_WF_InsFlow.FormInstanceID;
                //反写流程表单ID至待审批资质
                string updateSql = string.Format("update  T_Qualification_Postqualificationmanagement set T_Qualification_PostTotalID = '{0}' where  isnull(IsApprove,'False')='False' ", formID);
                HRSQLDB.ExecuteNonQuery(updateSql);
            }


            return bol;
        }

        protected override void OnFlowEnd(T_Qualification_PostTotal entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity == null)
                return;
            var sql = string.Format("select * from  T_Qualification_Postqualificationmanagement where T_Qualification_PostTotalID ='{0}'", entity.ID);
            var dt = HRSQLDB.ExecuteDataTable(sql);
            var dicList = FormulaHelper.DataTableToListDic(dt);
            foreach (var item in dicList)
            {
                var userid = item.GetValue("Users");
                var quali = BusinessEntities.Set<S_Qualification_Postqualificationmanagement>().FirstOrDefault(p => p.Users == userid);
                if (quali == null)
                {
                    quali = new S_Qualification_Postqualificationmanagement();
                    quali.ID = FormulaHelper.CreateGuid();
                    BusinessEntities.Set<S_Qualification_Postqualificationmanagement>().Add(quali);
                }
                UpdateEntity<S_Qualification_Postqualificationmanagement>(quali, item);
            }
            BusinessEntities.SaveChanges();

            sql = string.Format("update T_Qualification_Postqualificationmanagement set IsApprove='True' where T_Qualification_PostTotalID ='{0}'", entity.ID);
            HRSQLDB.ExecuteNonQuery(sql);
        }

        public JsonResult GetPostInfoList(QueryBuilder qb)
        {

            var id = GetQueryString("ID");
            var sql = "";
            var oldSql = "";
            if (!string.IsNullOrEmpty(id))
            {
                sql = string.Format(@"select *from T_Qualification_Postqualificationmanagement where  T_Qualification_PostTotalID='{0}' ", id);
            }
            else
            {
                //自动查询出当前系统未提交审批的资质变更
                sql = string.Format(@"select *from T_Qualification_Postqualificationmanagement where isnull(IsApprove,'False')='False'");
            }

            DataTable dt = HRSQLDB.ExecuteDataTable(sql, qb);

            string[] UserIDs = dt.AsEnumerable().Select(d => d.Field<string>("Users")).ToArray();
            //根据查询出来的人员查找现在系统已经存在的资质信息，用于页面列表展示变更颜色
            sql = string.Format(@"select * from S_Qualification_Postqualificationmanagement where Users in ('{0}')", string.Join(",", UserIDs).Replace(",", "','"));
            DataTable qualifiDt = HRSQLDB.ExecuteDataTable(sql);

            //将已存在的人员资质写入到资质审批信息中
            foreach (DataRow item in dt.Rows)
            {
                var rowData = qualifiDt.Select(" Users = '" + item["Users"].ToString() + "'");
                if (rowData.Length == 0)
                    continue;

                foreach (DataColumn columns in qualifiDt.Columns)
                {
                    if (!dt.Columns.Contains("Old_" + columns.ColumnName))
                    {
                        dt.Columns.Add("Old_" + columns.ColumnName);
                    }
                    item["Old_" + columns.ColumnName] = rowData[0][columns.ColumnName];
                }
            }

            GridData gd = new GridData(dt);
            gd.total = qb.TotolCount;

            return Json(gd);
        }

    }
}
