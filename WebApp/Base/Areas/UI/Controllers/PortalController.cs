using Base.Logic.Domain;
using Config;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using System.Web.Script.Serialization;
using Base.Logic.BusinessFacade.Portal;
using System.Text.RegularExpressions;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Base.Areas.UI.Controllers
{
    public class PortalController : BaseController
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var isNew = Convert.ToBoolean(GetQueryString("IsNew"));
            return Json(sqlHelper.ExecuteGridData(string.Format("select * from S_A_PortalTemplet where {0}", isNew ? "IsNewPortal = '1'" : "IsNewPortal is null or IsNewPortal = 0"), qb));
        }
        public JsonResult GetRole(string TempletID, QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format(@"select a.ID,case when isnull(a.RoleID,'')<>'' then N'角色' when isnull(a.OrgID,'')<>'' then N'组织' else N'用户' end Type
                ,case when isnull(a.RoleID,'')<>'' then b.Name when isnull(a.OrgID,'')<>'' then c.Name else d.Name end Name
                ,a.RoleID,a.OrgID,a.UserID
                from S_A_TempletRole a left join S_A_Role b on a.RoleID=b.ID
                left join S_A_Org c on a.OrgID=c.ID
                left join S_A_User d on a.UserID=d.ID
                where TempletID='{0}'", TempletID);
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult SetRole(string ID, string IDs, QueryBuilder qb)
        {
            var list = entities.Set<S_A_TempletRole>().Where(c => c.TempletID == ID);
            foreach (var item in list)
            {
                entities.Set<S_A_TempletRole>().Remove(item);
            }

            if (!string.IsNullOrEmpty(IDs))
            {
                foreach (string items in IDs.Split(','))
                {
                    S_A_TempletRole tr = new S_A_TempletRole();
                    tr.ID = FormulaHelper.CreateGuid();
                    tr.TempletID = ID;
                    tr.RoleID = items;
                    entities.Set<S_A_TempletRole>().Add(tr);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetModel(string id)
        {
            var entity = GetEntity<S_A_PortalTemplet>(id);
            var dic = Formula.FormulaHelper.ModelToDic<S_A_PortalTemplet>(entity);
            return Json(dic);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            string json = Request.Form["FormData"];
            var isNew = Convert.ToBoolean(GetQueryString("IsNew"));
            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(json);

            var entity = UpdateEntity<S_A_PortalTemplet>();
            if (entities.Set<S_A_PortalTemplet>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("编号重复，标题“{0}”，编号：“{1}”", entity.Title, entity.Code));

            entity.Code = formDic.GetValue("Code");
            entity.Title = formDic.GetValue("Title");
            entity.Items = formDic.GetValue("Items");
            entity.IsEnabled = Convert.ToBoolean(formDic.GetValue("IsEnabled"));
            if (isNew)
                entity.IsNewPortal = "1";
            return JsonSave<S_A_PortalTemplet>(entity);
        }

        public JsonResult Delete()
        {
            string ListIDs = Request["ListIDs"];
            var list = entities.Set<S_A_PortalTemplet>().Where(c => ListIDs.Contains(c.ID)).ToList();
            foreach (var model in list)
            {
                entities.Set<S_A_TempletRole>().Delete(c => c.TempletID == model.ID);
                entities.Set<S_A_PortalTemplet>().Remove(model);
            }
            entities.SaveChanges();
            return Json("");
        }
        public ActionResult List()
        {
            if (PortalMain.IsUseNewPortal)
                Response.Redirect("/Base/UI/Portal/NewPortal");
            return View();
        }

        [Authorize]
        public ActionResult Main()
        {
            ViewBag.Templet = PortalMain.GetPortalTemplet(Request["ID"]);
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        [Authorize]
        public ActionResult Portal()
        {
            ViewBag.PortalLTE = PortalMain.GetIndexHTML(Request.Url.AbsoluteUri, Request["ID"]);
            return View();
        }

        public ActionResult NewPortal()
        {
            var catalogs = entities.Set<S_I_PublicInformCatalog>().ToList();
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (var item in catalogs)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.SetValue("value", item.CatalogKey);
                dic.SetValue("text", item.CatalogName);
                list.Add(dic);
            }
            ViewBag.Catalogs = JsonHelper.ToJson(list);
            return View();
        }

        public JsonResult SaveLoginPortal()
        {
            var data = Request["Data"];
            if (!string.IsNullOrEmpty(data))
            {
                var temp = entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.IsNewPortal == "2");
                if (temp != null)
                {
                    temp.Items = data;
                }
                else
                {
                    S_A_PortalTemplet templet = new S_A_PortalTemplet
                    {
                        ID = FormulaHelper.CreateGuid(),
                        Code = "publicPortal",
                        Title = "公共门户",
                        Items = data,
                        IsNewPortal = "2"
                    };
                    entities.Set<S_A_PortalTemplet>().Add(templet);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetPortal(string id)
        {
            var portal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == id);
            return Json(portal);
        }

        public string GetAllPortal()
        {
            var enumTable = EnumBaseHelper.GetEnumTable(typeof(Base.Logic.DefaultPortal));
            return JsonHelper.ToJson(enumTable);
        }

        public JsonResult DeletePortal(string id)
        {
            entities.Set<S_A_Portal>().Delete(c => c.ID == id);
            entities.SaveChanges();
            return Json("");
        }

        public string GetPublicInformCatalog()
        {
            return JsonHelper.ToJson(entities.Set<S_I_PublicInformCatalog>().ToList());
        }

        public string CreateTemplet(string id, string data, bool isLoad = false)
        {
            PortalMain.CreatePortalTemplet(id, data, isLoad);
            return "";
        }

        public string CreatePortal(string id, string portalID, bool isNew, string data)
        {
            return PortalMain.CreatePortal(id, portalID, isNew, data);
        }

        public JsonResult GetUrl(string portalID)
        {
            var protal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == portalID);
            if (protal != null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(protal.ConnName);
                string sql = replaceSql(protal.SQL);
                var dt = sqlHelper.ExecuteDataTable(sql);
                if (!dt.Columns.Contains("SortIndex"))
                {
                    dt.Columns.Add("SortIndex");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["SortIndex"] = (i + 1).ToString();
                    }
                }
                var json = JsonHelper.ToJson(dt);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }
        }

        private string replaceSql(string sql)
        {
            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (!string.IsNullOrEmpty(Request[value]))
                    return Request[value];

                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgCode:
                        return user.UserOrgCode;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    case "CurrentDate":
                        return DateTime.Now.Date.ToString("yyyy-MM-dd");
                    default:
                        return m.Value;
                }
            });

            return result;
        }


        public JsonResult SetTemplet(string id, string json)
        {
            return Json(PortalMain.SetTemplet(id, json));
        }

        public JsonResult GetViews(string portalID, string id)
        {
            var protal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == portalID);
            if (protal != null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(protal.ConnName);
                string sql = replaceSql(protal.SQL);
                var dt = sqlHelper.ExecuteDataTable(sql);
                DataTable newTable = dt.Clone();
                DataRow[] rows = dt.Select(string.Format("ID = '{0}'", id));
                newTable.ImportRow((DataRow)rows[0]);
                var json = JsonHelper.ToJson(newTable);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult SetReadCount(string portalID, string id)
        {
            var protal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == portalID);
            if (protal != null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(protal.ConnName);
                string sql = replaceSql(protal.SQL);
                var sqlList = sql.Split(' ');
                var mainTableName = sqlList[sqlList.ToList().IndexOf("from") + 1];
                var extSql = "update {0} set ReadCount = ReadCount+1 where ID = '{1}'";
                sqlHelper.ExecuteNonQuery(string.Format(extSql, mainTableName, id));
            }
            return Json("");
        }

        public JsonResult GetListView(string portalID, QueryBuilder qb)
        {
            var protal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == portalID);
            if (protal != null)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(protal.ConnName);
                string sql = replaceSql(protal.SQL);
                var dt = sqlHelper.ExecuteDataTable(sql, qb);
                var json = JsonHelper.ToJson(dt);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult SetOrgRelation()
        {
            string id = Request["ID"];
            string roleID = Request["RoleID"];
            string[] orgIDs = GetValues(Request["RelationData"], "ID").Distinct().ToArray();
            var templetRole = entities.Set<S_A_TempletRole>().SingleOrDefault(c => c.TempletID == id && c.RoleID == roleID);
            if (templetRole != null)
            {
                templetRole.OrgID = orgIDs.Length > 0 ? string.Join(",", orgIDs) : "";
                entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult GetTree(string id, string roleID)
        {
            string fullID = Request["RootFullID"];
            if (fullID == null)
                fullID = "";

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string sql = string.Format("select ID,Code,case when '{1}'='EN' then isnull(NameEN,Name) else Name end as Name,ParentID,FullID,Type,SortIndex,Description,0 IsCheck from S_A_Org where  FullID like '{0}%' and IsDeleted='0'", fullID, FormulaHelper.GetCurrentLGID());

            if (!string.IsNullOrEmpty(Request["OrgType"]))
                sql += string.Format(" and Type in ('{0}')", Request["OrgType"].Replace(",", "','"));

            sql += " order by ParentID,SortIndex";
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            var templetRole = entities.Set<S_A_TempletRole>().SingleOrDefault(c => c.TempletID == id && c.RoleID == roleID);
            if (templetRole != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string ids = row["ID"].ToString();
                    if (!string.IsNullOrEmpty(templetRole.OrgID) && templetRole.OrgID.Contains(ids))
                        row["IsCheck"] = 1;
                }
            }
            return Json(dt);
        }

        public string GetTempletRole(string roleID)
        {
            string message = "已存在相同角色的模板[{0}],将按模板升序的排序方式优先使用模板[{1}],你确认吗?";
            var templetRole = entities.Set<S_A_TempletRole>().Where(c => c.RoleID == roleID).GroupBy(c => c.TempletID).ToList();
            if (templetRole.Count > 0)
            {
                List<string> ids = new List<string>();
                foreach (var item in templetRole)
                {
                    ids.Add(item.Key);
                }
                if (ids.Count > 0)
                {
                    string templetIDs = string.Join(",", ids.ToArray());
                    var portalTemplet = entities.Set<S_A_PortalTemplet>().Where(c => templetIDs.Contains(c.ID)).OrderBy(c => c.Xorder).ToList();
                    List<string> msgList = new List<string>();
                    foreach (var item in portalTemplet)
                    {
                        msgList.Add(item.Title);
                    }
                    message = string.Format(message, string.Join(",", msgList.ToArray()), msgList.First());
                }
                else
                {
                    message = "true";
                }
            }
            else
            {
                message = "true";
            }
            return message;
        }

        public JsonResult GetUserTemplet(string searchKey, QueryBuilder qb)
        {
            var isNew = Convert.ToBoolean(GetQueryString("IsNew"));
            return Json(PortalMain.GetUserTemplet(searchKey, isNew));
        }

        public JsonResult GetNewPortalTemplet(QueryBuilder qb)
        {
            var PortalTemplet = entities.Set<S_A_PortalTemplet>().FirstOrDefault(c => c.IsNewPortal == "2");
            if (PortalTemplet != null)
                return Json(PortalTemplet.Items);
            else
                return Json("");
        }

        public JsonResult SavePortal()
        {
            var data = JsonHelper.ToObject<Dictionary<string, object>>(Request["data"]);
            this.ValidateRequest = false;
            var entity = UpdateEntity<S_A_PortalTemplet>();
            entity.Code = data.GetValue("Code");
            entity.Title = data.GetValue("Title");
            entity.Items = data.GetValue("Items");
            entity.IsNewPortal = "2";
            entities.SaveChanges();
            return Json(entity);
        }

        private string GetMapPath(string catalog)
        {
            return HttpContext.Server.MapPath(string.Format("/PortalLTE/Images/Portal/{0}/", catalog));
        }

        private bool SingleFileUpload(string catalog, string fileName)
        {
            if (Request.Files.Count > 0)
            {
                string fileFullName = GetMapPath(catalog) + fileName + Path.GetExtension(Request.Files[0].FileName);
                Request.Files[0].SaveAs(fileFullName);
                return true;
            }
            return false;
        }

        private bool MultipleFileUpload(string catalog)
        {
            if (Request.Files["FileData"] != null)
            {
                var t = Request.Files["FileData"].InputStream;
                string fileName = Request.Files["FileData"].FileName;
                string path = GetMapPath(catalog) + fileName;
                Image img = Image.FromStream(t);
                img.Save(path);
                return true;
            }
            return false;
        }

        public JsonResult SingleUpload(string fileName, string type = "Blue")
        {
            return Json(SingleFileUpload(type, fileName));
        }

        public JsonResult GetPictures(string type = "Blue")
        {
            string path = string.Format("/PortalLTE/Images/Portal/{0}/Banner", type);
            string mapPath = HttpContext.Server.MapPath(path);
            List<object> list = new List<object>();
            if (Directory.Exists(mapPath))
            {
                var files = Directory.GetFiles(mapPath);
                foreach (var item in files)
                {
                    var fileName = Path.GetFileName(item);
                    list.Add(new
                    {
                        ID = FormulaHelper.CreateGuid(),
                        PictureName = fileName,
                        Description = string.Format("<img style='width:50px;height:50px;' src='{0}'>", Path.Combine(path, fileName)),
                        Delete = string.Format("<a href=\"javascript:delRow('" + fileName + "', '" + type + "')\" title=\"删除\"><img border='0px' src='/CommonWebResource/RelateResource/image/ico16/del.gif' width='16px' height='16px' align='absmiddle' /></a>")
                    });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MultipleUpload(string type = "Blue")
        {
            return Json(MultipleFileUpload(Path.Combine(type, "Banner")));
        }

        public JsonResult DeletePicture(string fileName, string type = "Blue")
        {
            var path = GetMapPath(Path.Combine(type, "Banner")) + fileName;
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return Json("");
        }

        public JsonResult DeleteRole(string id)
        {
            entities.Set<S_A_TempletRole>().Delete(c => c.ID == id);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetPortalRoleRelation()
        {
            int type = Convert.ToInt32(GetQueryString("Type"));
            string templetID = GetQueryString("TempletID");
            string[] IDs = GetValues(GetQueryString("RelationData"), "ID").Distinct().ToArray();
            if (IDs.Length > 0)
            {
                foreach (var id in IDs)
                {
                    var templetRole = entities.Set<S_A_TempletRole>().Create();
                    templetRole.ID = FormulaHelper.CreateGuid();
                    templetRole.TempletID = templetID;
                    switch (type)
                    {
                        case 0:
                            templetRole.RoleID = id;
                            break;
                        case 1:
                            templetRole.OrgID = id;
                            break;
                        default:
                            templetRole.UserID = id;
                            break;
                    }
                    entities.Set<S_A_TempletRole>().Add(templetRole);
                }
                entities.SaveChanges();
            }
            return Json("");
        }
    }
}
