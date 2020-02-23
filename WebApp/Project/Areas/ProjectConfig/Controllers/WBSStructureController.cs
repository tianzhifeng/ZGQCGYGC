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
    public class WBSStructureController : BaseConfigController<S_T_WBSStructInfo>
    {
        public override JsonResult Save()
        {
            var data = UpdateEntity<S_T_WBSStructInfo>();
            string[] codes = data.Code.Split(',');

            var list = entities.Set<S_T_WBSStructInfo>().Where(c => c.ModeID == data.ModeID);

            for (int i = 0; i < codes.Length; i++)
            {
                var c = codes[i];
                var li = list.FirstOrDefault(d => d.Code == c);
                if (li == null)
                {
                    S_T_WBSStructInfo ssInfo = new S_T_WBSStructInfo();
                    ssInfo.ID = FormulaHelper.CreateGuid();
                    ssInfo.Code = c;
                    ssInfo.ModeID = data.ModeID;
                    ssInfo.Name = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), ssInfo.Code);
                    entities.Set<S_T_WBSStructInfo>().Add(ssInfo);
                }
            }
            entities.Set<S_T_WBSStructInfo>().Remove(data);
            entities.SaveChanges();
            return Json("");
        }

        public override JsonResult Delete()
        {
            string listIDs = Request["ListIDs"];
            if (String.IsNullOrWhiteSpace(listIDs)) return Json("");
            foreach (var id in listIDs.Split(','))
            {
                var structInfo = this.entities.Set<S_T_WBSStructInfo>().FirstOrDefault(d => d.ID == id);
                structInfo.Delete();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetWBSNodes()
        {
            var dt = EnumBaseHelper.GetEnumTable(typeof(Project.Logic.WBSNodeType));
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetWBSNodeList(string ModeID, string Code)
        {
            var mode = this.GetEntityByID<S_T_ProjectMode>(ModeID);
            if (mode == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【"+ModeID+"】的管理模式对象");
            var list = mode.S_T_WBSStructInfo.ToList();
            var gridData = new GridData(list);
            return Json(gridData);
        }

        public JsonResult GetRoleStructList(string WBSStructInfoID)
        {
            var list = this.entities.Set<S_T_WBSStructRole>().Where(d => d.WBSStructID == WBSStructInfoID).OrderBy(d=>d.SortIndex);
            var data = new GridData(list);
            return Json(data);
        }

        public JsonResult AddWBSStruct(string WBSNodeInfo, string ModeID)
        {
            if (String.IsNullOrEmpty(WBSNodeInfo) || String.IsNullOrEmpty(ModeID))
                throw new Formula.Exceptions.BusinessException("模式ID为空或没有选择任何节点信息");
            var mode = this.entities.Set<S_T_ProjectMode>().FirstOrDefault(d => d.ID == ModeID);
            if (mode == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ModeID + "】的管理模式对象，无法配置WBS结构");
            var data = JsonHelper.ToList(WBSNodeInfo);
            foreach (var item in data)
                mode.AddWBSSturct(item["value"].ToString());
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddRoleInfo(string WBSStructInfoID, string RoleInfo)
        {
            if (String.IsNullOrEmpty(RoleInfo) || String.IsNullOrEmpty(WBSStructInfoID))
                throw new Formula.Exceptions.BusinessException("WBS结构定义ID为空或没有选择任何节点信息");
            var wbsStNode = this.entities.Set<S_T_WBSStructInfo>().FirstOrDefault(d => d.ID == WBSStructInfoID);
            if (wbsStNode == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSStructInfoID + "】的管理模式对象，无法配置WBS结构");
            var data = JsonHelper.ToList(RoleInfo);
            foreach (var item in data)
                wbsStNode.AddRoleInfo(item["RoleCode"].ToString());
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteRoleInfo(string RoleInfo)
        {
            if (String.IsNullOrEmpty(RoleInfo))  return Json("");
            var data = JsonHelper.ToList(RoleInfo);
            foreach (var item in data)
            {
                var id = item["ID"].ToString();
                this.entities.Set<S_T_WBSStructRole>().Delete(d => d.ID == id);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRoleInfo(string RoleInfo)
        {
            if (String.IsNullOrEmpty(RoleInfo)) return Json("");
            this.UpdateList<S_T_WBSStructRole>(RoleInfo);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Move(string roleInfoID,string Direction)
        {
            var roleInfo = this.GetEntityByID<S_T_WBSStructRole>(roleInfoID);
            if (roleInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【】的角色关联定义对象，无法移动");
            if (Direction.ToLower() == "up")
                roleInfo.MoveUp();
            else
                roleInfo.MoveDown();
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
