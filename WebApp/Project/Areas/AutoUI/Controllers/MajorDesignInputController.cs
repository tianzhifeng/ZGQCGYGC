using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using System.Text;

namespace Project.Areas.AutoUI.Controllers
{
    public class MajorDesignInputController : ProjectFormContorllor<T_SC_MajorDesignInput>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var projectInfoID = this.GetQueryString("ProjectInfoID");
                var catagory = this.GetQueryString("MajorValue");
                string sql = @"select S_D_InputDocument.ID as DocID,InfoName,InputType,S_D_InputDocument.Catagory,
S_D_InputDocument.Name,S_D_InputDocument.CreateDate,S_D_Input.ID as InputID,Files,
S_D_InputDocument.CreateUser from S_D_InputDocument left join S_D_Input on S_D_InputDocument.InputID=S_D_Input.ID where S_D_InputDocument.Catagory ='{1}'  
and ProjectInfoID='{0}' and (AuditState='' or AuditState='未评审' or AuditState is null)";
                var detail = this.ProjectSQLDB.ExecuteDataTable(String.Format(sql, projectInfoID, catagory));
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (dt.Rows.Count > 0 && dt.Columns.Contains("DesignInputList"))
                    dt.Rows[0]["DesignInputList"] = JsonHelper.ToJson(detail);

                sql = @"select UserID,UserName from dbo.S_W_OBSUser where ProjectInfoID='{0}' and MajorValue='{1}' and RoleCode='MajorPrinciple'";
                var userDt = this.ProjectSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0 && dt.Columns.Contains("ProjectManager"))
                {
                    dt.Rows[0]["ProjectManager"] = projectInfo.ChargeUserID;
                    dt.Rows[0]["ProjectManagerName"] = projectInfo.ChargeUserName;
                    if (userDt.Rows.Count > 0)
                    {
                        dt.Rows[0]["MajorPrinciple"] = userDt.Rows[0]["UserID"];
                        dt.Rows[0]["MajorPrincipleName"] = userDt.Rows[0]["UserName"];
                    }
                }
            }
        }


        protected override void OnFlowEnd(T_SC_MajorDesignInput entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {

            if (entity != null)
                entity.Publish();
            this.BusinessEntities.SaveChanges();
        }
    }
}
