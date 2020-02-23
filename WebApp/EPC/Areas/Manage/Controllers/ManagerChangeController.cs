using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Base.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class ManagerChangeController : EPCFormContorllor<T_I_ManagerChange>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {

        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var sql = string.Format(@"select * from T_I_ManagerChange where ManagerNominateID='{0}' ", dic.GetValue("ManagerNominateID"));
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    dic.SetValue("FirstOne", "true");
                }
                else
                {
                    var list = FormulaHelper.DataTableToListDic(dt);
                    if (list.Exists(m => m.GetValue("FlowPhase") != "End" ))
                    {
                        throw new Formula.Exceptions.BusinessValidationException("此项目经理变更单在审核中，无法变更！");
                    }
                }
            }
        }

        protected override void OnFlowEnd(T_I_ManagerChange entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var updateSql = string.Empty;
            if (string.IsNullOrWhiteSpace(entity.LastVersionID))
            {
                var sql = string.Format(@"select * from T_I_ManagerNominate where ID='{0}' ", entity.ManagerNominateID);
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("此项目的项目经理信息不存在，请确认！");
                }
                var firstChange = FormulaHelper.DataRowToDic(dt.Rows[0]);
                firstChange.SetValue("NewManager", firstChange.GetValue("Manager"));
                firstChange.SetValue("NewManagerName", firstChange.GetValue("ManagerName"));
                firstChange.SetValue("ManagerNominateID", firstChange.GetValue("ID"));
                firstChange.SetValue("LastVersionID", string.Empty);
                firstChange.SetValue("FirstOne", "true");
                updateSql += firstChange.CreateInsertSql(EPCSQLDB, "T_I_ManagerChange", FormulaHelper.CreateGuid());
            }
            var managerNominate = entity.ToDic();
            managerNominate.SetValue("FlowPhase", "End");
            managerNominate.SetValue("Manager", managerNominate.GetValue("NewManager"));
            managerNominate.SetValue("ManagerName", managerNominate.GetValue("NewManagerName"));
            updateSql += managerNominate.CreateUpdateSql(EPCSQLDB, "T_I_ManagerNominate", managerNominate.GetValue("ManagerNominateID"));
            EPCSQLDB.ExecuteNonQuery(updateSql);

        }
    }
}
