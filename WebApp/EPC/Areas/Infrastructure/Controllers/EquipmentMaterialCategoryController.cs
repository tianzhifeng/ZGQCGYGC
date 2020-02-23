using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using EPC.Logic;
using Formula;
using System.Text;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class EquipmentMaterialCategoryController : InfrastructureController<S_T_EquipmentMaterialCategory>
    {
        #region 设备分类树
        public override JsonResult GetTree()
        {
            var exsitCount = this.entities.Set<S_T_EquipmentMaterialCategory>().Count(a => a.ParentID == "");
            if (exsitCount == 0)
                CreateRoot();
            return base.GetTree();
        }

        public override JsonResult SaveNode()
        {
            var entity = UpdateNode<S_T_EquipmentMaterialCategory>();
            //根-专业分类-设备大类-设备小类-设备
            //五层以后为设备，设备作为叶子节点，不能继续增加节点
            if (entity.FullID.Split('.').Length > 5)
                throw new Formula.Exceptions.BusinessValidationException("设备节点下不能再增加分类");
            else if (entity.FullID.Split('.').Length == 5)
                entity.Type = EquipmentMaterialCategoryType.Entity.ToString();
            else
                entity.Type = EquipmentMaterialCategoryType.Category.ToString();

            entities.SaveChanges();

            return Json(new { ID = entity.ID, ParentID = entity.ParentID, FullID = entity.FullID, Type = entity.Type });
        }

        public override JsonResult SaveList()
        {
            //验证编号不能重复
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(Request["ListData"]);
            var groups = rows.GroupBy(a => a.GetValue("Code")).Select(g => new { g.Key, Counts = g.Count() }).ToList();
            if (groups.Count(a => a.Counts > 1) > 0)
                throw new Formula.Exceptions.BusinessValidationException("编码【" + groups.FirstOrDefault(a => a.Counts > 1).Key + "】重复");
            var list = this.entities.Set<S_T_EquipmentMaterialCategory>().ToList();
            var delIDs = new List<string>();
            foreach (var item in rows)
            {
                if (list.Exists(a => a.ID != item.GetValue("ID") && a.Code == item.GetValue("Code")))
                    throw new Formula.Exceptions.BusinessValidationException("编码【" + item.GetValue("Code") + "】重复");
                var _state = item.GetValue("_state");
                if (_state == "removed" || _state == "deleted")
                    delIDs.Add(item.GetValue("ID"));
            }

            var savelist = UpdateList<S_T_EquipmentMaterialCategory>(rows);
            foreach (var item in savelist)
            {
                if (!item.FullID.EndsWith(item.ID))
                    item.FullID = item.FullID + "." + item.ID;
            }
            entities.SaveChanges();

            var sb = new StringBuilder();
            foreach (var delID in delIDs)
            {
                if (string.IsNullOrEmpty(delID))
                    continue;
                sb.AppendLine(" delete from S_T_EquipmentMaterialCategory where fullid like '%" + delID + "%'");
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
                this.SqlHelper.ExecuteNonQuery(sb.ToString());
            return Json("");
        }
                                
        private void CreateRoot()
        {
            var root = new S_T_EquipmentMaterialCategory();
            root.ID = FormulaHelper.CreateGuid();
            root.Name = "设备材料分类";
            root.ParentID = string.Empty;
            root.FullID = root.ID;
            root.Type = EquipmentMaterialCategoryType.Category.ToString();
            this.EntityCreateLogic<S_T_EquipmentMaterialCategory>(root);
            this.entities.Set<S_T_EquipmentMaterialCategory>().Add(root);
            this.entities.SaveChanges();
        }

        #endregion

        #region 设备材料库

        public JsonResult SaveEntityList()
        {
            //验证编号不能重复
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(Request["ListData"]);
            var groups = rows.GroupBy(a => a.GetValue("Code")).Select(g => new { g.Key, Counts = g.Count() }).ToList();
            if (groups.Count(a => a.Counts > 1) > 0)
                throw new Formula.Exceptions.BusinessValidationException("编码【" + groups.FirstOrDefault(a => a.Counts > 1).Key + "】重复");
            var list = this.entities.Set<S_T_EquipmentMaterialTemplate>().ToList();
            foreach (var item in rows)
            {
                if (list.Exists(a => a.ID != item.GetValue("ID") && a.Code == item.GetValue("Code")))
                    throw new Formula.Exceptions.BusinessValidationException("编码【" + item.GetValue("Code") + "】重复");
            }

            //this.entities.Set<S_T_EquipmentMaterialTemplate>().GroupBy(a => a.Code).Select(a => new { a.Key });
            return base.JsonSaveList<S_T_EquipmentMaterialTemplate>(Request["ListData"]);
        }

        public JsonResult GetEntityList(QueryBuilder qb)
        {
            //var fullID = GetQueryString("FullID");
            //qb.Add("CategoryFullID", QueryMethod.StartsWith, fullID);

            return base.JsonGetList<S_T_EquipmentMaterialTemplate>(qb);
        }

        #endregion

        #region 设备材料价格库

        public JsonResult GetPriceList(QueryBuilder qb)
        {
            string categoryID = GetQueryString("CategoryID");
            if (string.IsNullOrEmpty(categoryID))
                throw new Formula.Exceptions.BusinessValidationException("未获得分类categoryID");

            string sql = @"select distinct tt.*,tmp1.UnitPrice,tmp1.ContractCode,tmp1.ProjectCode,tmp1.SupplierName,tmp1.ContractState from S_T_EquipmentMaterialTemplate  tt inner join 
                           (select pBom.*,tmp.SerialNumber as ContractCode, tmp.ProjectCode,tmp.UnitPrice,tmp.PartyBName as SupplierName,tmp.ContractState
                           from EPC_Engineering..S_P_Bom pBom inner join
                           (select S_P_ContractInfo.*,S_P_ContractInfo_Content.UnitPrice,S_P_ContractInfo_Content.PBomID,S_I_Engineering.SerialNumber as ProjectCode from 
                           EPC_Engineering..S_P_ContractInfo_Content inner join EPC_Engineering..S_P_ContractInfo on EPC_Engineering..S_P_ContractInfo.ID = EPC_Engineering..S_P_ContractInfo_Content.S_P_ContractInfoID
                           inner join EPC_Engineering..S_I_Engineering on EPC_Engineering..S_P_ContractInfo.EngineeringInfoID = EPC_Engineering..S_I_Engineering.ID) tmp 
                           on pBom.ID = tmp.PBomID) tmp1
                           on tt.Name = tmp1.Name and tt.Model = tmp1.Model and tt.Material = tmp1.Material and tt.Brand = tmp1.Branding and tt.ConnectionMode = tmp1.ConnectionType
                           where CategoryID = '{0}' and tmp1.ContractState = 'Sign'
";
            var data = this.SqlHelper.ExecuteGridData(string.Format(sql, categoryID), qb);
            return Json(data);
        }
        #endregion
    }
}
