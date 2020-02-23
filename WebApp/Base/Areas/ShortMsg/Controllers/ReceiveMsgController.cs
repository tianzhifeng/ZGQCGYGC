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
    public class ReceiveMsgController : BaseController
    {
        private void ViewBagTitle()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.Unread = LGID ? "Unread" : "未读";
            ViewBag.Attachment = LGID ? "Attachment" : "有附件";
            ViewBag.Delete = LGID ? "Delete" : "批量删除";
            ViewBag.NonSystem = LGID ? "Non System" : "非系统";
            ViewBag.QueryInput = LGID ? "Please Input Keyword" : "请输入发送人姓名、标题或附件名";
            ViewBag.DetailedInquiry = LGID ? "Query" : "详细查询";
            ViewBag.Importance = LGID ? "Importance" : "重要性";
            ViewBag.Title = LGID ? "Title" : "标题";
            ViewBag.Send = LGID ? "Send" : "发送";
            ViewBag.Sender = LGID ? "Sender" : "发送人";
            ViewBag.Time = LGID ? "Time" : "时间";
            ViewBag.SendTime = LGID ? "Time" : "发送时间";
            ViewBag.Query = LGID ? "Query" : "查询";
            ViewBag.Clear = LGID ? "Clear" : "清空";
            ViewBag.Cancel = LGID ? "Cancel" : "取消";
            ViewBag.Help = LGID ? "Help" : "帮助";
            ViewBag.Read = LGID ? "Read Back Receipt" : "已读回执";
            ViewBag.EssentialInformation = LGID ? "Essential Information" : "基本信息";
            ViewBag.ReceiverName = LGID ? "Recipient" : "接收人";
            ViewBag.Content = LGID ? "Content" : "内容";
            ViewBag.ReceiverDepartment = LGID ? "Department" : "接收部门";
            ViewBag.Attachment = LGID ? "Attachment" : "附件";
            ViewBag.RelatLink = LGID ? "Link" : "相关链接";
            ViewBag.ReplyMessage = LGID ? "Reply" : "回复消息";
            ViewBag.ForwardMessage = LGID ? "Forward" : "转发消息";
            ViewBag.DeleteMessage = LGID ? "Delete" : "删除消息";
            ViewBag.CollectMessage = LGID ? "Collect" : "收藏消息";
            ViewBag.LookMessgeState = LGID ? "State" : "查看读信状态";
            ViewBag.State = LGID ? "State" : "状态";
            ViewBag.MessgeState = LGID ? "Messge State" : "读信状态";
            ViewBag.Time = LGID ? "Time" : "时间";
            ViewBag.ReadState = LGID ? "Read" : "已读";
            ViewBag.SetRead = LGID ? "SetRead" : "设置已读";
            ViewBag.SetUnRead = LGID ? "SetUnRead" : "设置未读";

        }

        public ActionResult List()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult Edit()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult Views()
        {
            ViewBagTitle();
            return View();
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb, string read, string attachment, string system)
        {
            string sql = "";
            if (Config.Constant.IsOracleDb)
            {
                string whereRead = read == "T" ? " and FirstViewTime is null" : "";
                string whereAttach = attachment == "T" ? " and nvl(AttachFileIDs,'') <> '' " : "";
                string whereSystem = system == "T" ? " and nvl(IsSystemMsg,'') <> '1' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgReceiver.IsCollect,S_S_MsgReceiver.CollectTime,S_S_MsgBody.TYPE,S_S_MsgBody.Title,substr(nvl(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,(case when FirstViewTime is null then 0 else 1 end) as AlreadyRead,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='0' {1} {2} {3}
", FormulaHelper.UserID, whereRead, whereAttach, whereSystem);
            }
            else
            {
                string whereRead = read == "T" ? " and FirstViewTime is null" : "";
                string whereAttach = attachment == "T" ? " and isnull(AttachFileIDs,'') <> '' " : "";
                string whereSystem = system == "T" ? " and isnull(IsSystemMsg,'') <> '1' " : "";
                sql = string.Format(@"
select S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.ParentID,S_S_MsgReceiver.IsCollect,S_S_MsgReceiver.CollectTime,S_S_MsgBody.[Type],S_S_MsgBody.Title,substring(isnull(S_S_MsgBody.ContentText,''),0,50) as ContentText
,S_S_MsgBody.AttachFileIDs,S_S_MsgBody.LinkUrl,S_S_MsgBody.IsSystemMsg,S_S_MsgBody.SendTime,S_S_MsgBody.SenderID
,S_S_MsgBody.SenderName,S_S_MsgBody.ReceiverIDs,S_S_MsgBody.ReceiverNames,AlreadyRead=case when FirstViewTime is null then 0 else 1 end,S_S_MsgBody.Importance
from S_S_MsgReceiver join S_S_MsgBody on S_S_MsgBody.ID = MsgBodyID where UserID='{0}' and S_S_MsgReceiver.IsDeleted='0' {1} {2} {3}
", FormulaHelper.UserID, whereRead, whereAttach, whereSystem);
            }
            return Json(SQLHelper.CreateSqlHelper("Base").ExecuteGridData(sql, qb));
        }

        public JsonResult GetModel(string id)
        {
            string forwardID = Request["ForwardID"];
            string replyID = Request["ReplyID"];

            UserInfo user = FormulaHelper.GetUserInfo();
            string orgUser = string.IsNullOrEmpty(user.UserOrgName) ? user.UserName : user.UserOrgName + "&nbsp;&nbsp;" + user.UserName;
            if (!string.IsNullOrEmpty(forwardID))
            {
                var msg = entities.Set<S_S_MsgBody>().Where(c => c.ID == forwardID).SingleOrDefault();
                msg.ID = "";
                msg.ParentID = forwardID;
                msg.ReceiverIDs = "";
                msg.ReceiverNames = "";
                msg.Title = "转发：" + msg.Title;
                msg.Content = "<p>&nbsp;</p><p></p><p></p><p></p><p>" + orgUser + "</p><hr />" + msg.Content;
                msg.IsReadReceipt = "0";
                return Json(msg);
            }
            else if (!string.IsNullOrEmpty(replyID))
            {
                var msg = entities.Set<S_S_MsgBody>().Where(c => c.ID == replyID).SingleOrDefault();
                msg.ID = "";
                msg.ParentID = replyID;
                msg.ReceiverIDs = msg.SenderID;
                msg.ReceiverNames = msg.SenderName;
                msg.Title = "回复：" + msg.Title;
                msg.Content = "<p>&nbsp;</p><p></p><p><p></p><p></p><hr/>发送人：" + user.UserName + string.Format("&nbsp;发送时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm")) + "</p>" + msg.Content;
                msg.IsReadReceipt = "0";
                return Json(msg);
            }
            else
            {
                // var msg = entities.Set<S_S_MsgBody>().Where(c => c.ID == id).SingleOrDefault();
                string userID = user.UserID;
                S_S_MsgReceiver receiver = entities.Set<S_S_MsgReceiver>().FirstOrDefault(c => c.MsgBodyID == id && c.UserID == userID && c.FirstViewTime == null);
                if (receiver != null)
                {
                    receiver.FirstViewTime = DateTime.Now;
                    S_S_MsgBody msg = entities.Set<S_S_MsgBody>().FirstOrDefault(c => c.ID == id);
                    if (msg != null)
                    {
                        //需要已读回执
                        if (msg.IsReadReceipt == "1")
                        {
                            string content = "<p style=\"text-align:left;text-indent:-72pt;margin-left:72pt;\" align=\"left\"><span style=\"font-family:宋体;\">消息</span></p>"
                                + "<p style=\"text-align:left;text-indent:-72pt;margin-left:72pt;\" align=\"left\"><span style=\"font-family:宋体;\"></span></p>"
                                + "<p style=\"text-align:left;text-indent:-72pt;margin-left:72pt;\" align=\"left\"><span style=\"font-family:宋体;\"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>接收人:<span> </span>{0}</span></p>"
                                + "<p style=\"text-align:left;text-indent:-72pt;margin-left:72pt;\" align=\"left\"><span style=\"font-family:宋体;\"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>标题:<span>&nbsp;&nbsp; </span>{1}</span></p>"
                                + "<p style=\"text-align:left;text-indent:-72pt;margin-left:72pt;\" align=\"left\"><span style=\"font-family:宋体;\"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>发送时间:<span> </span>{2}</span></p>"
                                + "<p style=\"text-align:left;\" align=\"left\"><span style=\"font-family:宋体;\"></span></p>"
                                + "<p style=\"text-align:left;\" align=\"left\"><span style=\"font-family:宋体;\">阅读时间为 {3}</span></p>";
                            content = string.Format(content, receiver.UserName, msg.Title, msg.SendTime, receiver.FirstViewTime);
                            FormulaHelper.GetService<IMessageService>().SendMsg("已读：" + msg.Title, content, string.Empty, string.Empty, msg.SenderID, msg.SenderName, null);
                        }
                    }
                    entities.SaveChanges();
                }
                var msgbody = this.GetEntity<S_S_MsgBody>(id);
                msgbody.ReceiverIDs = string.Join(",", msgbody.S_S_MsgReceiver.Select(a => a.UserID).ToList());
                msgbody.ReceiverNames = string.Join(",", msgbody.S_S_MsgReceiver.Select(a => a.UserName).ToList());
                return Json(msgbody);
            }
        }
        public JsonResult Collect(string ids)
        {
            string[] arr = ids.Split(',');
            List<S_S_MsgReceiver> receiver = entities.Set<S_S_MsgReceiver>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (S_S_MsgReceiver item in receiver)
            {
                item.IsCollect = "1";
                item.CollectTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult GetHistory(string parentID)
        {
            List<S_S_MsgBody> list = new List<S_S_MsgBody>();
            GetHistoryData(ref list, parentID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void GetHistoryData(ref List<S_S_MsgBody> list, string parentID)
        {
            List<S_S_MsgBody> listParent = entities.Set<S_S_MsgBody>().Where(c => c.ID == parentID).ToList();
            foreach (S_S_MsgBody item in listParent)
            {
                list.Add(item);
                GetHistoryData(ref list, item.ParentID);
            }
        }

        public JsonResult GetReceivers(string ID)
        {
            var receivers = entities.Set<S_S_MsgReceiver>().Where(a => a.MsgBodyID == ID).OrderBy(a => a.ID).ToList();
            return Json(receivers);
        }
        public JsonResult SetUnRead(string ids)
        {
            string[] arr = ids.Split(',');
            var objs = entities.Set<S_S_MsgReceiver>().Where(c => ids.Contains(c.ID));
            foreach (var obj in objs)
            {
                obj.FirstViewTime = null;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult SetRead(string ids)
        {
            string[] arr = ids.Split(',');
            var objs = entities.Set<S_S_MsgReceiver>().Where(c => ids.Contains(c.ID));
            foreach (var obj in objs)
            {
                obj.FirstViewTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }
    }
}
