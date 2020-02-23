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
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;


namespace Project.Areas.BaseData.Controllers
{
    public class WBSAttrController : BaseConfigController<S_D_WBSAttrDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            string wbsType = this.Request["Type"];
            qb.PageSize = 0;
            var list = entities.Set<S_D_WBSAttrDefine>().Where(d => d.Type == wbsType).WhereToGridData(qb);
            return Json(list);
        }

        public override JsonResult GetTree()
        {
            var dt = EnumBaseHelper.GetEnumTable(typeof(WBSNodeType));
            var result = new DataTable();
            result.Columns.Add("Name");
            result.Columns.Add("ParentID");
            result.Columns.Add("Type");
            result.Columns.Add("ID");
            var root = result.NewRow();
            root["Name"] = "WBS属性维护";
            root["ID"] = "Root";
            root["Type"] = "Root";
            root["ParentID"] = "";
            result.Rows.Add(root);
            foreach (DataRow row in dt.Rows)
            {
                var child = result.NewRow();
                child["Name"] = row["text"];
                child["ID"] = row["value"];
                child["Type"] = "Child";
                child["ParentID"] = "Root";
                result.Rows.Add(child);
            }
            return Json(result);
        }

        protected override void BeforeDelete(List<S_D_WBSAttrDefine> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }


        public JsonResult GetDeptInfo()
        {
            string ID = this.Request["WBSAttrDefineID"];
            return Json(this.entities.Set<S_D_WBSAttrDeptInfo>().Where(d => d.WBSAttrDefineID == ID));
        }

        public JsonResult SaveDeptInfo()
        {
            string deptInfo = this.Request["DeptInfo"];
            string wbsAttrID = this.Request["WBSAttrID"];
            var wbsAttr = this.entities.Set<S_D_WBSAttrDefine>().FirstOrDefault(d => d.ID == wbsAttrID);
            if (wbsAttr == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【】的wbs属性定义对象，无法设置部门");
            if (String.IsNullOrEmpty(deptInfo)) throw new Formula.Exceptions.BusinessException("部门信息未选择的情况下，不能设置部门");
            var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(deptInfo);
            foreach (var item in list)
            {
                string deptID = item["ID"].ToString();
                string deptName = item["Name"].ToString();
                wbsAttr.SetRelationDept(deptID, deptName);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DelDeptInfo()
        {
            string deptInfo = this.Request["DeptInfo"];
            var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(deptInfo);
            foreach (var item in list)
            {
                string ID = item["ID"].ToString();
                this.entities.Set<S_D_WBSAttrDeptInfo>().Delete(d => d.ID == ID);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        protected override void BeforeSave(S_D_WBSAttrDefine entity, bool isNew)
        {
            var major = WBSNodeType.Major.ToString();
            var defineInfo = this.entities.Set<S_D_WBSAttrDefine>().FirstOrDefault(d => d.Type == entity.Type && d.ID != entity.ID && d.Code == entity.Code);
            if (defineInfo != null) throw new Formula.Exceptions.BusinessException("编号不能重复！");

            base.BeforeSave(entity, isNew);
        }

        public void AddAuditMode(string rowID,string code,string name)
        {
            var projectModeList = this.entities.Set<S_T_ProjectMode>().ToList();
            foreach (var item in projectModeList)
            {
                var auditMode = new S_T_AuditMode();
                auditMode.ID = FormulaHelper.CreateGuid();
                auditMode.ProjectModeID = item.ID;
                auditMode.AttrID = rowID;
                auditMode.PhaseValue = code;
                auditMode.PhaseName = name;
                auditMode.AuditMode = "OneByTwo";

                this.entities.Set<S_T_AuditMode>().Add(auditMode);
            }
            this.entities.SaveChanges();
        }

        public void DeleteAuditMode(string rowIDs)
        {
            this.entities.Set<S_D_WBSAttrDefine>().Delete(d => rowIDs.Contains(d.ID));
            this.entities.Set<S_T_AuditMode>().Delete(d => rowIDs.Contains(d.AttrID));
            this.entities.SaveChanges();
        }
    }
}
