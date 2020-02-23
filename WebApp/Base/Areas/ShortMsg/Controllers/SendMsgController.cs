using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using MvcAdapter;
using Formula.Exceptions;
using Config;
using System.Text.RegularExpressions;

namespace Base.Areas.ShortMsg.Controllers
{
    public class SendMsgController : BaseController<S_S_MsgBody>
    {
        public JsonResult GetSendList(MvcAdapter.QueryBuilder qb, string attachment)
        {
            string userID = FormulaHelper.UserID;
            IQueryable<S_S_MsgBody> query = entities.Set<S_S_MsgBody>().Where(c => c.SenderID == userID && c.IsDeleted == "0");
            List<S_S_MsgReceiver> recAll = entities.Set<S_S_MsgReceiver>().ToList();
            foreach (var item in query.ToArray())
            {
                string[] ids = item.ReceiverIDs.Split(',');
                string[] names = item.ReceiverNames.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    string id = ids[i];
                    var receiver = recAll.Where(c => c.MsgBodyID == item.ID && c.UserID == id).FirstOrDefault();
                    if (receiver != null && (!string.IsNullOrEmpty(receiver.FirstViewTime.ToString()) || Convert.ToInt32(receiver.IsDeleted) == 1))
                    {
                        names[i] = string.Format("<font style='color:#3c8dbc;' title='{0}'>{1}</font>",
                            string.IsNullOrEmpty(receiver.FirstViewTime.ToString()) ? ((DateTime)receiver.DeleteTime).ToString("yyyy年MM月dd日 HH:mm:ss") : ((DateTime)receiver.FirstViewTime).ToString("yyyy年MM月dd日 HH:mm:ss"), names[i]);
                    }
                }
                item.ReceiverNames = string.Join(",", names);
            }
            if (attachment == "T")
                query = query.Where(c => !string.IsNullOrEmpty(c.AttachFileIDs));
            return Json(query.WhereToGridData(qb));
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var msg = UpdateEntity<S_S_MsgBody>();

            var user = FormulaHelper.GetUserInfo();
            msg.SenderID = user.UserID;
            msg.SenderName = user.UserName;
            msg.SendTime = DateTime.Now;
            msg.Content = Request["content"];
            msg.ContentText = RemoveImg(Request["contenttext"]);
            if (msg.Content.Length > 2000)
                throw new BusinessException("消息长度不能超过2000个字符!");

            List<S_S_MsgReceiver> listReceiver = new List<S_S_MsgReceiver>();
            if (!string.IsNullOrEmpty(msg.ReceiverIDs) && !string.IsNullOrEmpty(msg.ReceiverNames))
            {
                string[] receiverIDs = msg.ReceiverIDs.Split(',');
                string[] receiverNames = msg.ReceiverNames.Split(',');

                for (int i = 0; i < receiverIDs.Length; i++)
                {
                    S_S_MsgReceiver receiver = new S_S_MsgReceiver();
                    receiver.ID = FormulaHelper.CreateGuid();
                    receiver.MsgBodyID = msg.ID;
                    receiver.UserID = receiverIDs[i];
                    receiver.UserName = receiverNames[i];
                    listReceiver.Add(receiver);
                }
            }
            if (!string.IsNullOrEmpty(msg.ReceiverDeptIDs) && string.IsNullOrEmpty(msg.ReceiverRoleIDs))
            {
                string[] arrDeptID = msg.ReceiverDeptIDs.Split(',');
                List<S_A_User> list = base.DataBaseFilter<S_A_User>(entities.Set<S_A__OrgUser>().Where(c => arrDeptID.Contains(c.OrgID)).Select(c => c.S_A_User)).Distinct().ToList();
                foreach (S_A_User item in list)
                {
                    S_S_MsgReceiver receiver = listReceiver.Find(c => c.UserID == item.ID);
                    if (receiver == null)
                    {
                        receiver = new S_S_MsgReceiver();
                        receiver.ID = FormulaHelper.CreateGuid();
                        receiver.MsgBodyID = msg.ID;
                        receiver.UserID = item.ID;
                        receiver.UserName = item.Name;
                        listReceiver.Add(receiver);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(msg.ReceiverDeptIDs) || !string.IsNullOrEmpty(msg.ReceiverRoleIDs))
            {
                IRoleService roleService = FormulaHelper.GetService<IRoleService>();
                IUserService userService = FormulaHelper.GetService<IUserService>();
                string userIds = roleService.GetUserIDsInRoles(msg.ReceiverRoleIDs, msg.ReceiverDeptIDs);
                string userNames = userService.GetUserNames(userIds);
                var userIdArr = userIds.Split(',');
                var userNameArr = userNames.Split(',');
                for (int i = 0; i < userIdArr.Length; i++)
                {
                    var userId = userIdArr[i];
                    if (userId == "") continue;
                    S_S_MsgReceiver receiver = listReceiver.Find(c => c.UserID == userId);
                    if (receiver == null)
                    {
                        receiver = new S_S_MsgReceiver();
                        receiver.ID = FormulaHelper.CreateGuid();
                        receiver.MsgBodyID = msg.ID;
                        receiver.UserID = userId;
                        receiver.UserName = userNameArr[i];
                        listReceiver.Add(receiver);
                    }

                }
            }

            foreach (S_S_MsgReceiver item in listReceiver)
            {
                entities.Set<S_S_MsgReceiver>().Add(item);
            }

            return base.JsonSave<S_S_MsgBody>(msg);
        }

        private string RemoveImg(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml)) return strHtml;
            strHtml = Regex.Replace(strHtml, "<img[^>]*>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return strHtml;
        }

        public JsonResult DeleteMsgBody(string ids)
        {
            string[] arr = ids.Split(',');
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            List<S_S_MsgBody> list = entities.Set<S_S_MsgBody>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (S_S_MsgBody item in list)
            {
                item.IsDeleted = "1";
                item.DeleteTime = DateTime.Now;
                sqlHelper.ExecuteNonQuery(string.Format("Update S_S_MsgReceiver set IsDeleted= '1',DeleteTime='{1}' Where MsgBodyID='{0}'", item.ID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }
        public JsonResult Collect(string ids)
        {
            string[] arr = ids.Split(',');
            List<S_S_MsgBody> body = entities.Set<S_S_MsgBody>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (S_S_MsgBody item in body)
            {
                item.IsCollect = "1";
                item.CollectTime = DateTime.Now;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        private void ViewBagTitle()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.Send = LGID ? "Send" : "发送";
            ViewBag.Title = LGID ? "Title" : "标题";
            ViewBag.Cancel = LGID ? "Cancel" : "取消";
            ViewBag.Important = LGID ? "Important" : "重要";
            ViewBag.Read = LGID ? "Read Back Receipt" : "已读回执";
            ViewBag.EssentialInformation = LGID ? "Essential Information" : "基本信息";
            ViewBag.ReceiverName = LGID ? "User" : "接收人员(姓名)";
            ViewBag.ReceiverDepartment = LGID ? "Department" : "接收部门(全员)";
            ViewBag.ReceiverRole = LGID ? "Role" : "接收角色";
            ViewBag.Content = LGID ? "Content" : "内容";
            ViewBag.Attachment = LGID ? "Attachment" : "附件";
            ViewBag.Delete = LGID ? "Delete" : "批量删除";
            ViewBag.QueryInput = LGID ? "Please Input Keyword" : "请输入发送人姓名、标题或附件名";
            ViewBag.DetailedInquiry = LGID ? "Query" : "详细查询";
            ViewBag.Importance = LGID ? "Importance" : "重要性";
            ViewBag.Recipient = LGID ? "Recipient" : "接收人";
            ViewBag.Time = LGID ? "Time" : "时间";
            ViewBag.Content = LGID ? "Content" : "内容";
            ViewBag.SendTime = LGID ? "Time" : "发送时间";
            ViewBag.Query = LGID ? "Query" : "查询";
            ViewBag.Clear = LGID ? "Clear" : "清空";
            ViewBag.ForwardMessage = LGID ? "Forward" : "转发消息";
            ViewBag.DeleteMessage = LGID ? "Delete" : "删除消息";
            ViewBag.CollectMessage = LGID ? "Collect" : "收藏消息";
        }

        public override ActionResult Edit()
        {
            ViewBagTitle();
            return View();
        }

        public override ActionResult List()
        {
            ViewBagTitle();
            return View();
        }

    }
}
