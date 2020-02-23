using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using Formula;

namespace Base.Areas.ShortMsg.Controllers
{
    public class DeleteMsgController : BaseController
    {
        //
        // GET: /ShortMsg/DeleteMsg/

        public JsonResult GetList(MvcAdapter.QueryBuilder qb, string attachment)
        {
            string sql = "";
            if (Config.Constant.IsOracleDb)
            {
                string whereAttach = attachment == "T" ? " and nvl(AttachFileIDs,'') <> '' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgBody.TYPE,S_S_MsgBody.Title,substr(nvl(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgReceiver.DeleteTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='1' {1}
union all
select ID,ID as ReceiverID,ParentID,[Type],Title,substring(isnull(ContentText,''),0,50),AttachFileIDs,LinkUrl,IsSystemMsg,SendTime,
DeleteTime,SenderID,SenderName,ReceiverIDs,ReceiverNames,Importance from S_S_MsgBody  where SenderID='{0}' and IsDeleted='1' {1}
", FormulaHelper.UserID, whereAttach);
            }
            else
            {
                string whereAttach = attachment == "T" ? " and isnull(AttachFileIDs,'') <> '' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgBody.[Type],S_S_MsgBody.Title,substring(isnull(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgReceiver.DeleteTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='1' {1}
union all
select ID,ID as ReceiverID,ParentID,[Type],Title,substring(isnull(ContentText,''),0,50),AttachFileIDs,LinkUrl,IsSystemMsg,SendTime,
DeleteTime,SenderID,SenderName,ReceiverIDs,ReceiverNames,Importance from S_S_MsgBody  where SenderID='{0}' and IsDeleted='1' {1}
", FormulaHelper.UserID, whereAttach);
            }
            return Json(SQLHelper.CreateSqlHelper("Base").ExecuteGridData(sql, qb));
        }

        public JsonResult Delete(string ids)
        {
            string[] arr = ids.Split(',');
            entities.Set<S_S_MsgReceiver>().Delete(c => ids.Contains(c.ID));
            entities.Set<S_S_MsgBody>().Delete(c => ids.Contains(c.ID));
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult Recovery(string ids)
        {
            string[] arr = ids.Split(',');
            List<S_S_MsgReceiver> receiver = entities.Set<S_S_MsgReceiver>().Where(c => arr.Contains(c.ID)).ToList();
            List<S_S_MsgBody> body = entities.Set<S_S_MsgBody>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (S_S_MsgReceiver item in receiver)
            {
                item.IsDeleted = "0";
                item.DeleteTime = null;
            }
            foreach (S_S_MsgBody item in body)
            {
                item.IsDeleted = "0";
                item.DeleteTime = null;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult DeleteMsg(string ids)
        {
            string[] arrIds = ids.Split(',');
            List<S_S_MsgReceiver> list = entities.Set<S_S_MsgReceiver>().Where(c => arrIds.Contains(c.ID)).ToList();
            foreach (S_S_MsgReceiver item in list)
            {
                item.IsDeleted = "1";
                item.DeleteTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json("");
        }

        public ActionResult List()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.Attachment = LGID ? "Attachment" : "有附件";
            ViewBag.Delete = LGID ? "Delete" : "批量删除";
            ViewBag.QueryInput = LGID ? "Please Input Keyword" : "请输入发送人姓名、标题或附件名";
            ViewBag.DetailedInquiry = LGID ? "Query" : "详细查询";
            ViewBag.Importance = LGID ? "Importance" : "重要性";
            ViewBag.Title = LGID ? "Title" : "标题";
            ViewBag.Sender = LGID ? "Sender" : "发送人";
            ViewBag.Time = LGID ? "Time" : "时间";
            ViewBag.SendTime = LGID ? "Time" : "发送时间";
            ViewBag.Query = LGID ? "Query" : "查询";
            ViewBag.Clear = LGID ? "Clear" : "清空";
            ViewBag.Content = LGID ? "Content" : "内容";
            ViewBag.Attachment = LGID ? "Attachment" : "附件";
            ViewBag.ReplyMessage = LGID ? "Reply" : "回复消息";
            ViewBag.ForwardMessage = LGID ? "Forward" : "转发消息";
            ViewBag.DeleteMessage = LGID ? "Delete" : "删除消息";
            ViewBag.RecoveryMessage = LGID ? "Recovery" : "消息恢复";
            return View();
        }

    }
}
