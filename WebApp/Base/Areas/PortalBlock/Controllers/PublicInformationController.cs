﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Config;
using System.Data;
using Formula.Helper;
using Formula;
using System.Text.RegularExpressions;

namespace Base.Areas.PortalBlock.Controllers
{
    public class PublicInformationController : BaseController<S_I_PublicInformation>
    {
        public ActionResult Tabs()
        {
            return View();
        }

        public override ActionResult List()
        {
            ViewBag.CatalogEnum = JsonHelper.ToJson(SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable("select ID as value,CatalogName as text from S_I_PublicInformCatalog"));
            ViewBag.CatalogVisible = string.IsNullOrEmpty(this.Request["CatalogId"]) ? "true" : "false";
            return View();
        }

        public ActionResult ListView(string catalogKey)
        {
            ViewBag.Title = "新闻";
            ViewBag.CatalogKey = catalogKey;
            return View();
        }

        public ActionResult PublicView()
        {
            return View();
        }

        public ActionResult Views()
        {
            return View();
        }

        public override JsonResult GetModel(string id)
        {
            string catalogId = GetQueryString("CatalogId");
            S_I_PublicInformation model = GetEntity<S_I_PublicInformation>(id);
            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(catalogId))
                    model.CatalogId = catalogId;
                model.Important = "0";
                model.Urgency = "0";
            }
            return Json(model);
        }

        public JsonResult GetModelByDeptHome(string id)
        {
            string catalogId = GetQueryString("CatalogId");
            S_I_PublicInformation model = GetEntity<S_I_PublicInformation>(id);
            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(catalogId))
                    model.CatalogId = catalogId;
                model.Important = "0";
                model.Urgency = "0";
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.DeptDoorId = user.UserOrgID;
                model.DeptDoorName = user.UserOrgName;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = user.UserID;
                model.CreateUserName = user.UserName;
            }
            return Json(model);
        }

        public JsonResult SetReadCount(string id)
        {
            S_I_PublicInformation model = GetEntity<S_I_PublicInformation>(id);
            model.ReadCount = model.ReadCount == null ? 1 : model.ReadCount + 1;
            entities.SaveChanges();
            return Json(model);
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string catalogId = this.Request["CatalogId"];
            IQueryable<S_I_PublicInformation> query = entities.Set<S_I_PublicInformation>().Where(c => string.IsNullOrEmpty(c.DeptDoorId));
            if (!string.IsNullOrEmpty(catalogId))
                query = query.Where(c => c.CatalogId == catalogId);
            GridData grid = query.WhereToGridData(qb);
            return Json(grid);
        }

        public JsonResult GetListView(MvcAdapter.QueryBuilder qb, string catalogKey)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            try
            {
                List<S_I_PublicInformCatalog> listCatalog = entities.Set<S_I_PublicInformCatalog>().Where(c => c.CatalogKey == catalogKey).ToList();
                if (listCatalog.Count > 0)
                {
                    string whereReceiveRoleId = "";
                    string catalogID = listCatalog[0].ID;
                    Config.UserInfo ui = Formula.FormulaHelper.GetUserInfo();
                    string userID = ui.UserID;
                    string[] arrOrgFullID = ui.UserFullOrgIDs.Replace(",", ".").Split('.').Distinct().ToArray();

                    //处理接收角色
                    IUserService userService = FormulaHelper.GetService<IUserService>();
                    var roleIds = userService.GetRolesForUser(userID, "").Split(',');

                    foreach (var roleId in roleIds)
                    {
                        whereReceiveRoleId += string.Format(" or ReceiveRoleId like '%{0}%'", roleId);
                    }


                    if (Config.Constant.IsOracleDb)
                    {
                        string whereReceiveDeptId = string.Empty;
                        for (int i = 0; i < arrOrgFullID.Length; i++)
                        {
                            whereReceiveDeptId += "INSTR(ReceiveDeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                            if (i < arrOrgFullID.Length - 1)
                                whereReceiveDeptId += " or ";
                        }
                        string whereSql = " and ((nvl(ReceiveUserId,'empty') = 'empty' and nvl(ReceiveDeptId,'empty') = 'empty'  and nvl(ReceiveRoleId,'empty') = 'empty' ) or (nvl(ReceiveUserId,'empty') <> 'empty' and INSTR(ReceiveUserId,'" + userID + "',1,1) > 0) or (nvl(ReceiveDeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))" + whereReceiveRoleId + ") ";
                        GridData gridData = sqlHelper.ExecuteGridData(@"
select ID
      ,CatalogId
      ,Title
      ,Content
      ,Attachments
      ,ReceiveDeptId
      ,ReceiveDeptName
      ,ReceiveUserId
      ,ReceiveUserName
      ,ExpiresTime
      ,ReadCount
      ,IsTop
      ,CreateTime
      ,CreateUserName
      ,CreateUserID 
      ,Urgency
      ,Important
    from S_I_PublicInformation where CatalogId = '" + catalogID + "' and (DeptDoorId is null or DeptDoorId='') and (ExpiresTime is null or to_char(ExpiresTime,'yyyy-MM-dd') >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') " + whereSql + " ORDER BY IsTop DESC, CreateTime DESC", qb);
                        return Json(gridData);
                    }
                    else
                    {
                        string whereReceiveDeptId = string.Empty;
                        for (int i = 0; i < arrOrgFullID.Length; i++)
                        {
                            whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',ReceiveDeptId) > 0";
                            if (i < arrOrgFullID.Length - 1)
                                whereReceiveDeptId += " or ";
                        }
                        string whereSql = " and ((isnull(ReceiveUserId,'') = '' and isnull(ReceiveDeptId,'') = ''  and isnull(ReceiveRoleId,'') = '') or (isnull(ReceiveUserId,'') <> '' and charindex('" + userID + "',ReceiveUserId) > 0) or (isnull(ReceiveDeptId,'') <> '' and (" + whereReceiveDeptId + "))" + whereReceiveRoleId + ") ";
                        GridData gridData = sqlHelper.ExecuteGridData("select * from S_I_PublicInformation with(nolock) where CatalogId = '" + catalogID + "' and isnull(DeptDoorId,'') = '' and (ExpiresTime is null or ExpiresTime >= convert(datetime,convert(varchar(10),getdate(),120))) " + whereSql + " ORDER BY Important DESC,Urgency DESC, CreateTime DESC", qb);
                        return Json(gridData);
                    }

                    //IQueryable<S_I_PublicInformation> query = entities.Set<S_I_PublicInformation>().OrderByDescending(c => c.IsTop).OrderByDescending(c => c.CreateTime).Where(c => c.CatalogId == catalogID && (c.ExpiresTime == null || c.ExpiresTime >= DateTime.Today) && ((string.IsNullOrEmpty(c.ReceiveUserId) && string.IsNullOrEmpty(c.ReceiveDeptId)) || (!string.IsNullOrEmpty(c.ReceiveUserId) && c.ReceiveUserId.IndexOf(userID) > -1) || (!string.IsNullOrEmpty(c.ReceiveDeptId) && c.ReceiveDeptId.IndexOf(orgID) > -1)));
                    //GridData gridData = query.WhereToGridData(qb)  
                }
                else
                {
                    List<S_I_PublicInformation> list = new List<S_I_PublicInformation>();
                    GridData gridData = new GridData(list);
                    gridData.total = list.Count;
                    return Json(gridData);
                }
            }
            catch (Exception)
            {
                GridData gridData = sqlHelper.ExecuteGridData(@"select a.*,b.CatalogKey,b.CatalogName,IsPublic from S_I_PublicInformation a with(nolock) left join S_I_PublicInformCatalog b with(nolock) 
                        on a.CatalogId = b.id where b.CatalogKey='" + catalogKey + "' and b.IsPublic='1'", qb);
                return Json(gridData);
            }
        }

        /// <summary>
        /// 根据部门门户获取公共信息
        /// </summary>
        /// <param name="deptHomeID"></param>
        /// <param name="catalogID"></param>
        /// <returns></returns>
        public JsonResult GetListByDeptHome(string deptHomeID, string catalogID, string key)
        {
            IQueryable<S_I_PublicInformation> query = entities.Set<S_I_PublicInformation>().Where(c => c.DeptDoorId == deptHomeID && (c.ExpiresTime == null || (c.ExpiresTime != null && DateTime.Today <= c.ExpiresTime.Value)));
            if (!string.IsNullOrEmpty(catalogID))
                query = query.Where(c => c.CatalogId == catalogID);
            if (!string.IsNullOrEmpty(key))
                query = query.Where(c => c.Title.Contains(key) || c.Attachments.Contains(key) || c.ContentText.Contains(key));
            return Json(query.OrderByDescending(c => c.IsTop).ThenByDescending(c => c.CreateTime).ToList(), JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            this.ValidateRequest = false;
            var entity = UpdateEntity<S_I_PublicInformation>();
            entity.Content = Request["content"];
            //增加过滤html标签内容的方法，移动办公使用该值2018-2-27
            entity.ContentText = RemoveImg(Request["contenttext"]);
            entity.CreateTime = DateTime.Now;
            entity.CreateUserID = Formula.FormulaHelper.GetUserInfo().UserID;
            entity.CreateUserName = Formula.FormulaHelper.GetUserInfo().UserName;
            entity.Title = entity.Title.Replace("\"", " ").Replace("'", " ");
            entities.SaveChanges();
            return Json("");
        }

        private string RemoveImg(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml)) return strHtml;
            strHtml = Regex.Replace(strHtml, "<img[^>]*>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return strHtml;
        }
    }
}
