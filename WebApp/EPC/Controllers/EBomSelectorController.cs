using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Controllers
{
    public class EBomSelectorController : EPCController
    {
        public JsonResult GetList(string EngineeringInfoID)
        {
            string sql = @"select S_E_Bom.* from S_E_Bom inner join (
select Max(ID) MaxID,MajorCode,BomPhase from S_E_Bom where EngineeringInfoID='{0}'
and FlowPhase='End' group by MajorCode,BomPhase) MaxInfo on MaxInfo.MaxID = S_E_Bom.ID";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID));        
            return Json(dt);
        }

        public JsonResult GetTreeList(string VersionID)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_E_Bom>(VersionID);
            if (version == null) return Json(result);
            var data = this.entities.Set<S_I_PBS>().Where(d => d.EngineeringInfoID == version.EngineeringInfoID && d.NodeType != "Root").OrderBy(c => c.SortIndex).ToList();
            var versionList =version.S_E_Bom_Detail.Where(c => c.ModifyState != BomVersionModifyState.Remove.ToString()).OrderBy(c => c.SortIndex).ToList();

            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_I_PBS>(item);
                var details = versionList.Where(c => c.PBSNodeID == item.ID).ToList();
                //判定是否要过滤掉没有设备材料的PBS节点
                if (versionList.Count(c => c.PBSNodeFullID.StartsWith(item.FullID)) == 0)
                {
                    continue;
                }
                dic.SetValue("BomCount", details.Count);
                dic.SetValue("AddCount", versionList.Count(c => c.ModifyState == BomVersionModifyState.Add.ToString()));
                dic.SetValue("ModifyCount", versionList.Count(c => c.ModifyState == BomVersionModifyState.Modify.ToString()));
                dic.SetValue("RemoveCount", versionList.Count(c => c.ModifyState == BomVersionModifyState.Remove.ToString()));
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_E_Bom_Detail>(detail);
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }
    }
}
