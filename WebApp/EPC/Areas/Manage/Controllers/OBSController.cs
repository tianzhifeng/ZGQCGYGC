using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;


namespace EPC.Areas.Manage.Controllers
{
    public class OBSController : EPCController
    {
        public JsonResult GetTreeList(QueryBuilder qb, string EngineeringInfoID)
        {
            string sql = @"select ID,ParentID, Name,Code,SortIndex,UserIDs,UserNames,EngineeringInfoID,NodeType from dbo.S_I_OBS
left join (SELECT OBSID,
[UserIDs]=STUFF((SELECT distinct ','+UserID FROM S_I_OBS_User A 
WHERE A.OBSID=S_I_OBS_User.OBSID FOR XML PATH('')), 1, 1, ''),
[UserNames]=STUFF((SELECT distinct ','+UserName FROM S_I_OBS_User A 
WHERE A.OBSID=S_I_OBS_User.OBSID FOR XML PATH('')), 1, 1, '')
FROM S_I_OBS_User GROUP BY OBSID) OBSUserInfo
on S_I_OBS.ID = OBSUserInfo.OBSID where EngineeringInfoID='{0}'";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql,EngineeringInfoID));
            return Json(dt);
        }
    }
}
