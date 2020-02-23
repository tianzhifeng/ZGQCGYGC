using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Auth.Controllers
{
    public class OEMController : BaseController
    {
        public JsonResult RecoverUpdate(string type, string listIDs)
        {
            if (string.IsNullOrEmpty(listIDs))
            {
                return Json("至少选择一条数据！");
            }
            var idStr = listIDs.Replace(",", "','");
            if (!string.IsNullOrEmpty(idStr))
            {
                SQLHelper sqlHeler = SQLHelper.CreateSqlHelper(ConnEnum.Base);

                string sql = string.Empty;
                if (type.ToLower().Equals("task"))
                {
                    sql = string.Format(@"update S_OEM_TaskList set 
                                CreateTime=null,SyncState=null,SyncTime=null,RequestUrl=null,RequestData=null,Response=null,ErrorMsg=null 
                                where ID in('{0}')",
                       idStr);
                }
                else if (type.ToLower().Equals("file"))
                {
                    sql = string.Format(@"update S_OEM_TaskFileList set FsFileID=null, SyncState=null,SyncTime=null,RequestUrl=null,RequestData=null,Response=null,ErrorMsg=null where ID in('{0}')",
                            idStr);
                }

                sqlHeler.ExecuteNonQuery(sql);
            }

            return Json("");
        }
    }
}
