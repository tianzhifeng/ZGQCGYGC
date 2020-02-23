using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;

namespace EPC.Areas.Design.Controllers
{
    public class WorkSpaceController : EPCController<S_E_DrawingResult>
    {
        public JsonResult GetDesignWorkSpaceTree(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法开展设计工作");
            var list = this.entities.Set<S_I_WBS>().Where(c => c.EngineeringInfoID == EngineeringInfoID).ToList();
            var designTaskType = TaskType.DesignTask.ToString();
            var taskList = this.entities.Set<S_I_WBS_Task>().Where(c => c.TaskType == designTaskType && c.EngineeringInfoID == EngineeringInfoID).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in taskList)
            {
                var wbs = list.FirstOrDefault(c => c.ID == item.ParentID);
                if (!result.Exists(c => c.GetValue("ID") == wbs.ID))
                {
                    var wbsDic = this.fillDicWithWBS(wbs);
                    result.Add(wbsDic);
                    foreach (var node in wbs.Ancestor)
                    {
                        if (result.Exists(c => c.GetValue("ID") == node.ID) || node.NodeType == "Root")
                            continue;
                        wbsDic = this.fillDicWithWBS(node);
                        result.Add(wbsDic);
                    }
                }               
                var dic = this.fillDicWithTask(item);
                result.Add(dic);
            }
            result = result.OrderBy(c => c["SortIndex"]).ToList();
            return Json(result);
        }

        public JsonResult GetDrawingList(QueryBuilder qb, string TaskID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法展现列表");
            qb.Add("WBSFullID", QueryMethod.StartsWith, wbs.FullID);
            if (!String.IsNullOrEmpty(TaskID))
            {
                qb.Add("TaskID", QueryMethod.Equal, TaskID);
            }
            var data = this.entities.Set<S_E_DrawingResult>().WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetAuditList(QueryBuilder qb, string TaskID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法展现列表");
            qb.Add("WBSID", QueryMethod.Equal, wbs.ID);
            if (!String.IsNullOrEmpty(TaskID))
            {
                qb.Add("TaskID", QueryMethod.Equal, TaskID);
            }
            var data = this.entities.Set<S_E_AuditForm>().WhereToGridData(qb);
            return Json(data);
        }

        Dictionary<string, object> fillDicWithWBS(S_I_WBS item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("FullID", item.FullID);
            dic.SetValue("Value", item.Value);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ID);
            dic.SetValue("NodeType", item.NodeType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", "");
            if (item.S_I_Engineering.S_R_Resource.Count(c => c.WBSID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID
                 && !String.IsNullOrEmpty(c.RoleCode) && String.IsNullOrEmpty(c.TaskID)) > 0)
            {
                dic.SetValue("InUser", true.ToString());
                var roleCodes = item.S_I_Engineering.S_R_Resource.Where(c => c.WBSID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID &&
                     !String.IsNullOrEmpty(c.RoleCode) && String.IsNullOrEmpty(c.TaskID)).Select(c => c.RoleCode).ToList();
                dic.SetValue("RoleCode", String.Join(",", roleCodes));
            }
            else
            {
                dic.SetValue("InUser", false.ToString());
                dic.SetValue("RoleCode", "");
            }
            return dic;
        }

        Dictionary<string, object> fillDicWithTask(S_I_WBS_Task item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("FullID", item.WBSFullID);
            dic.SetValue("Value", item.Code);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ParentID);
            dic.SetValue("NodeType", item.TaskType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", item.ID);
            dic.SetValue("DrawingCount", this.entities.Set<S_E_DrawingResult>().Count(c => c.TaskID == item.ID));
            if (item.S_I_Engineering.S_R_Resource.Count(c => c.TaskID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID && !String.IsNullOrEmpty(c.RoleCode)) > 0)
            {
                dic.SetValue("InUser", true.ToString());
                var roleCodes = item.S_I_Engineering.S_R_Resource.Where(c => c.TaskID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID &&
                    !String.IsNullOrEmpty(c.RoleCode)).Select(c => c.RoleCode).ToList();
                dic.SetValue("RoleCode", String.Join(",", roleCodes));
            }
            else
            {
                dic.SetValue("InUser", false.ToString());
                dic.SetValue("RoleCode", "");
            }
            return dic;
        }

        protected override void BeforeSave(S_E_DrawingResult entity, bool isNew)
        {
            if (isNew)
            {
                entity.Save();
            }
            else
            {
                entity.Validate();
            }
        }

        protected override void BeforeDelete(List<S_E_DrawingResult> entityList)
        {
            foreach (var item in entityList)
            {
                if (item.S_E_DrawingResult_Version.Count > 1)
                    throw new Formula.Exceptions.BusinessValidationException("【" + item.Name + "】已经有通过校审的版本，不能删除");
                if (item.AuditState != EPC.Logic.AuditState.Create.ToString() && item.AuditState != EPC.Logic.AuditState.Design.ToString())
                {
                    throw new Formula.Exceptions.BusinessValidationException("【" + item.Name + "】已经发起了校审，不能删除");
                }
            }
        }

        public JsonResult UpGrade()
        {
            var entity = this.UpdateEntity<S_E_DrawingResult>();
            entity.Upgrade(entity.MainFile, entity.PdfFile, entity.Attachments, "");
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetVersionList(QueryBuilder qb)
        {
            string versionID = GetQueryString("ProductID");
            SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            qb.Add("DrawingResultID", QueryMethod.Equal, versionID);
            string sql = @"select S_E_DrawingResult_Version.*,S_E_DrawingResult.Name,dbo.S_E_DrawingResult.Code,dbo.S_E_DrawingResult.FileType
,dbo.S_E_DrawingResult.CreateUser,dbo.S_E_DrawingResult.SubmitDate from S_E_DrawingResult_Version
inner join S_E_DrawingResult on S_E_DrawingResult_Version.DrawingResultID=S_E_DrawingResult.ID";
            var grid = helper.ExecuteGridData(sql, qb);
            return Json(grid);
        }

        public JsonResult SaveProducts(string ListData, string WBSID, string TaskID)
        {
            var wbs = GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("找不到ID为【" + WBSID + "】的wbs");
            var listdata = UpdateList<S_E_DrawingResult>(ListData);
            foreach (var item in listdata)
            {
                item.WBSID = wbs.ID;
                item.TaskID = TaskID;
                item.WBSFullID = wbs.FullID;
                item.EngineeringInfoID = wbs.EngineeringInfoID;
                item.S_I_WBS = wbs;
                item.Save();
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateStartFlow(string productIDs)
        {
            var products = this.entities.Set<S_E_DrawingResult>().Where(c => productIDs.Contains(c.ID)).ToList();
            var result = new Dictionary<string, object>();
            var batchID = FormulaHelper.CreateGuid();
            foreach (var product in products)
            {
                if (product.AuditState != AuditState.Create.ToString())
                    throw new Formula.Exceptions.BusinessValidationException("图号为【" + product.Code + "】的成果已经校审，不可再次发起校审。");
                product.BatchID = batchID;
                var version = product.S_E_DrawingResult_Version.FirstOrDefault(c => c.Version == product.Version);
                if (version != null)
                {
                    version.BatchID = batchID;
                }
            }
            this.entities.SaveChanges();
            result.SetValue("BatchID", batchID);
            return Json(result);
        }
    }
}
