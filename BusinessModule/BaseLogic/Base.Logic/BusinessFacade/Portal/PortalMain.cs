using Base.Logic.Domain;
using Config;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Config.Logic;
using System.Web;
using System.Configuration;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace Base.Logic.BusinessFacade.Portal
{
    public static class PortalMain
    {
        private static string enumID = "{|ID|}";
        private static string enumTitle = "{|Title|}";
        private static string enumLinkUrl = "{|LinkUrl|}";
        private static string enumLinkList = "{|LinkList|}";
        private static string enumTabListData = "{|TabListData|}";
        private static string enumMoreUrl = "{|MoreUrl|}";
        private static string enumMoreID = "{|MoreID|}";
        private static string enumHeight = "{|Height|}";
        private static string enumIsList = "{|IsList|}";
        private static string enumIsOldPortal = "{|IsOldPortal|}";
        private static string enumBackgroundColor = "{|BackgroundColor|}";
        private static string enumPaddingBottom = "{|padding-bottom|}";
        private static string enumDashKey = "dashboard_{0}_{1}";
        private static string enumDashRow = "<div class=\"row\">{0}</div>";
        private static string enumDashSection = "<section id=\"{0}\" class=\"col-xs-{1} connectedSortable\">{2}</section>";


        public static bool IsUseNewPortal
        {
            get
            {
                var isUseNewPortal = System.Configuration.ConfigurationManager.AppSettings["IsUseNewPortal"];
                if (!string.IsNullOrWhiteSpace(isUseNewPortal) && isUseNewPortal.ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool IsUseLeftShortCut
        {
            get
            {
                var isUseNewPortal = System.Configuration.ConfigurationManager.AppSettings["IsUseLeftShortCut"];
                if (!string.IsNullOrWhiteSpace(isUseNewPortal) && isUseNewPortal.ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static string PortalKeys
        {
            get
            {
                var portalKeys = System.Configuration.ConfigurationManager.AppSettings["PortalKeys"];
                if (!string.IsNullOrWhiteSpace(portalKeys))
                {
                    return portalKeys;
                }
                else
                {
                    return "";
                }
            }
        }

        public static string HomePageTemplet
        {
            get
            {
                var homePageTemplet = System.Configuration.ConfigurationManager.AppSettings["HomePageTemplet"];
                if (!string.IsNullOrWhiteSpace(homePageTemplet))
                {
                    return homePageTemplet;
                }
                else
                {
                    throw new Exception("请配置名称为 HomePageTemplet 的appSettings配置节！");
                }
            }
        }


        private static string GetBG(int cols, int curCol)
        {
            List<int> list = new List<int>()
            { 
                3,4,7,8,11,12,15,16,19,20,23,24,27,28,31,32,35,36,39,40,43,44,47,48
            };
            var bg = "background-color:#cbd4dd";
            if (list.Where(c => c == curCol).Count() > 0)
                return bg;
            else
                return "";
        }


        public static List<Dictionary<string, string>> GetPortalIndexHTML(string systemName)
        {
            List<Dictionary<string, string>> portals = new List<Dictionary<string, string>>();
            try
            {
                Dictionary<string, string> explainDic = new Dictionary<string, string>();
                explainDic.SetValue("ID", FormulaHelper.CreateGuid());
                explainDic.SetValue("Key", "explain");
                explainDic.SetValue("Title", "首页");
                explainDic.SetValue("Count", "1");
                explainDic.SetValue("Html", "<img class='nologin_home_ad' src='/PortalLTE/Images/ad.png'>");
                portals.Add(explainDic);
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var portalTemplet = GetUserRoleTemplets(!string.IsNullOrEmpty(systemName) ? FormulaHelper.UserID : "");
                if (portalTemplet.Rows.Count > 0)
                {
                    var items = JsonHelper.ToObject<List<Dictionary<string, object>>>(portalTemplet.Rows[0]["Items"].ToString()).OrderBy(c => c.GetValue("SortIndex")).ToList();
                    for (var i = 0; i < items.Count; i++)
                    {
                        StringBuilder sbPortal = new StringBuilder();
                        var type = items[i].GetValue("Type");
                        if (type == NewPortalType.Image.ToString()) //图片新闻
                        {
                            var randomID = (new Random().Next(0, 99999999) + i).ToString();
                            string normalPath = HomePageTemplet + "image.js";
                            string normalTemplet = PortalHelper.GetPageHtml(normalPath);
                            sbPortal.Append(normalTemplet.Replace(enumLinkList, randomID));
                        }
                        else
                        {
                            List<S_I_PublicInformCatalog> tabList = new List<S_I_PublicInformCatalog>();
                            var catalogs = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformCatalog");
                            var keys = items[i].GetValue("Keys").Split(',');
                            foreach (var key in keys)
                            {
                                var dr = catalogs.Select(string.Format(" CatalogKey='{0}'", key));
                                if (dr != null && dr.Count() > 0)
                                {
                                    tabList.Add(new S_I_PublicInformCatalog()
                                    {
                                        ID = dr[0]["ID"].ToString(),
                                        CatalogKey = dr[0]["CatalogKey"].ToString(),
                                        CatalogName = dr[0]["CatalogName"].ToString()
                                    });
                                }
                            }
                            foreach (var key in keys)
                            {
                                var dr = catalogs.Select(string.Format(" CatalogKey='{0}'", key));
                                if (dr != null && dr.Count() > 0)
                                {
                                    var randomID = (new Random().Next(0, 99999999) + i).ToString();
                                    string normalPath = HomePageTemplet + "normal.js";
                                    string normalTemplet = PortalHelper.GetPageHtml(normalPath);
                                    sbPortal.Append(normalTemplet.Replace(enumID, key).Replace(enumLinkList, randomID).Replace(enumTabListData, JsonHelper.ToJson(tabList)).Replace(enumBackgroundColor, ""));
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(sbPortal.ToString()))
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.SetValue("ID", FormulaHelper.CreateGuid());
                            dic.SetValue("Key", items[i].GetValue("Keys"));
                            dic.SetValue("Title", items[i].GetValue("Title"));
                            if (items[i].ContainsKey("Count"))
                                dic.SetValue("Count", items[i].GetValue("Count"));
                            dic.SetValue("Html", sbPortal.ToString());
                            portals.Add(dic);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception("门户配置有误,请联系管理员!");
            }

            return portals;
        }

        public static string GetIndexHTML(string url, string templateID = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                System.Random Random = new System.Random();
                var userInfo = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == FormulaHelper.UserID);
                if (userInfo != null)
                {
                    List<string> roles = new List<string>();
                    bool isPortalMain = url.IndexOf("/UI/Portal/Portal") >= 0;
                    string linkTemplate = PortalHelper.GetPageHtml(HomePageTemplet + "linkUrl.js");
                    var portalTemplet = sqlHelper.ExecuteDataTable("select ID as TempletID,Items from S_A_PortalTemplet");
                    DataRow dataRow = null;
                    if (!string.IsNullOrEmpty(templateID))
                    {
                        dataRow = portalTemplet.Select(string.Format(" TempletID='{0}'", templateID))[0];
                    }
                    else
                    {
                        portalTemplet = GetUserRoleTemplets(FormulaHelper.UserID);
                        if (portalTemplet.Rows.Count > 0)
                            dataRow = portalTemplet.Rows[0];
                    }
                    if (portalTemplet.Rows.Count > 0 && dataRow != null)
                    {
                        var items = dataRow["Items"].ToString();
                        if (!string.IsNullOrEmpty(userInfo.PortalSettings) && !isPortalMain)
                        {
                            if (string.IsNullOrEmpty(templateID) || (!string.IsNullOrEmpty(templateID) && userInfo.PortalSettings.Split(':')[0] == templateID))
                                items = userInfo.PortalSettings.Split(':')[1];
                        }
                        templateID = dataRow["TempletID"].ToString();
                        sb.Append(items);
                        Regex reg = new Regex(@"\{PORTALS}(.+?){/PORTALS}");
                        foreach (Match m in reg.Matches(items))
                        {
                            string portals = m.Value;
                            string result = Regex.Match(portals, "(?<={PORTALS}).*?(?={/PORTALS})").Value;
                            if (string.IsNullOrEmpty(result))
                            {
                                sb.Replace(portals, "");
                            }
                            else
                            {
                                StringBuilder sbPortal = new StringBuilder();
                                string condition = PortalHelper.SplitCondition(result);
                                string orderBy = string.Format("charindex(','+rtrim(cast(ID as varchar(100)))+',',',{0},')", result);
                                if (Config.Constant.IsOracleDb)
                                    orderBy = string.Format("'{0}',ID)", result);
                                var portal = sqlHelper.ExecuteDataTable(string.Format("select * from S_A_Portal where ID In ({0}) order by {1}", condition, orderBy));
                                if (portal.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in portal.Rows)
                                    {
                                        string id = dr["ID"].ToString();
                                        string title = dr["Title"].ToString();
                                        string linkUrl = Convert.ToString(dr["LinkUrl"]);
                                        string moreUrl = Convert.ToString(dr["MoreUrl"]);
                                        string height = Convert.ToString(dr["Height"]).Trim();
                                        bool IsList = Convert.ToString(dr["DisplayType"]) == DisplayType.List.ToString() || string.IsNullOrEmpty(Convert.ToString(dr["DisplayType"]));
                                        int bottom = !string.IsNullOrEmpty(height) ? 32 : 0;
                                        height = string.IsNullOrEmpty(height) ? "100%" : height.ToLower().Contains("px") ? height : height + "px";
                                        var portalType = (PortalType)Enum.Parse(typeof(PortalType), dr["Type"].ToString());
                                        var randomID = Random.Next(0, 99999999).ToString();
                                        switch (portalType)
                                        {
                                            case PortalType.Link:
                                                if (linkUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || linkUrl.IndexOf(".cshtml") <= 0)
                                                {
                                                    if (linkTemplate.Length > 0)
                                                    {
                                                        var rurl = System.Web.HttpContext.Current.Request.Url.Host;
                                                        var rport = System.Web.HttpContext.Current.Request.Url.Port;
                                                        var domain = linkUrl.Split('/'); //以“/”进行分割
                                                        var returnurl = "";
                                                        if (linkUrl.StartsWith("http") && domain.Length >= 2 && domain[2] != null)
                                                        {
                                                            returnurl = domain[0] + "//" + domain[2];
                                                        }
                                                        else
                                                        {
                                                            returnurl = ""; //如果url不正确就取空
                                                        }

                                                        var urlflag = false;

                                                        if (returnurl.IndexOf(":") >= 0)
                                                        {
                                                            if (returnurl.IndexOf(rurl + ":" + rport) < 0)
                                                            {
                                                                urlflag = true;
                                                            }
                                                        }
                                                        else if(returnurl != "")
                                                        {
                                                            if (rport.ToString() != "80" || returnurl.IndexOf(rurl) < 0)
                                                            {
                                                                urlflag = true;
                                                            }
                                                        }
                                                        if (urlflag)
                                                        {
                                                            var token = GetToken();
                                                            var tokenKey = String.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenKey"]) ? String.Empty : ConfigurationManager.AppSettings["TokenKey"];
                                                            if (!string.IsNullOrEmpty(tokenKey) && !string.IsNullOrEmpty(token))
                                                            {
                                                                if (linkUrl.IndexOf("?") >= 0)
                                                                {
                                                                    linkUrl = linkUrl + "&" + tokenKey + "=" + token;
                                                                }
                                                                else
                                                                {
                                                                    linkUrl = linkUrl + "?" + tokenKey + "=" + token;
                                                                }
                                                            }
                                                        }
                                                        linkTemplate = PortalHelper.SplitOnClick(linkTemplate, templateID, (isPortalMain ? id : ""));
                                                        sbPortal.Append(linkTemplate.Replace(enumID, id).Replace(enumTitle, title).Replace(enumLinkUrl, linkUrl).Replace(enumHeight, height).Replace(enumPaddingBottom, bottom.ToString()).Replace(enumIsList, IsList ? "true" : "false").Replace(enumIsOldPortal, !IsUseNewPortal ? "true" : "false"));
                                                    }
                                                }
                                                else
                                                {
                                                    string temp = PortalHelper.GetPageHtml(linkUrl);
                                                    temp = PortalHelper.SplitOnClick(temp, templateID, (isPortalMain ? id : ""));
                                                    sbPortal.Append(temp.Replace(enumID, id).Replace(enumTitle, title).Replace(enumLinkList, randomID).Replace(enumMoreID, randomID).Replace(enumMoreUrl, moreUrl).Replace(enumLinkUrl, linkUrl).Replace(enumHeight, height).Replace(enumPaddingBottom, bottom.ToString()).Replace(enumIsList, IsList ? "true" : "false").Replace(enumIsOldPortal, !IsUseNewPortal ? "true" : "false"));
                                                }
                                                break;
                                            case PortalType.Default:
                                                string defaultPath = HomePageTemplet + "default.js";
                                                string defaultTemplet = PortalHelper.GetPageHtml(defaultPath);
                                                defaultTemplet = PortalHelper.SplitOnClick(defaultTemplet, templateID, (isPortalMain ? id : ""));
                                                sbPortal.Append(defaultTemplet.Replace(enumID, id).Replace(enumTitle, title).Replace(enumLinkList, randomID).Replace(enumMoreID, randomID).Replace(enumMoreUrl, moreUrl).Replace(enumLinkUrl, linkUrl).Replace(enumHeight, height).Replace(enumPaddingBottom, bottom.ToString()).Replace(enumIsList, IsList ? "true" : "false").Replace(enumIsOldPortal, !IsUseNewPortal ? "true" : "false"));
                                                break;
                                        }
                                    }
                                }
                                sb.Replace(portals, sbPortal.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception("门户配置有误,请联系管理员!");
            }

            return sb.ToString();
        }

        public static Dictionary<string, string> GetPortalType()
        {
            Dictionary<string, string> lists = new Dictionary<string, string>();
            foreach (var item in Enum.GetNames(typeof(PortalType)))
            {
                lists.Add(item, PortalHelper.GetEnumDescription((PortalType)Enum.Parse(typeof(PortalType), item)));
            }
            return lists;
        }

        public static S_A_PortalTemplet GetPortalTemplet(string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            return entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.ID == id);
        }

        public static void CreatePortalTemplet(string id, string json, bool isLoad)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var templet = entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.ID == id);
            if (templet != null)
            {
                string rows = "1", cols = "3", colWidth = "4,4,4";
                if (string.IsNullOrEmpty(templet.Items))
                {
                    templet.Rows = rows;
                    templet.Cols = cols;
                    templet.ColsWidth = colWidth;
                    templet.Items = CalcTemplet(rows, cols, colWidth);
                }
                else
                {
                    if (!isLoad)
                    {
                        Regex reg = new Regex(@"\{PORTALS}(.+?){/PORTALS}");
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                        List<string> portalIDs = new List<string>();
                        foreach (Match m in reg.Matches(templet.Items))
                        {
                            string portals = m.Value;
                            string result = Regex.Match(portals, "(?<={PORTALS}).*?(?={/PORTALS})").Value;
                            if (!string.IsNullOrEmpty(result))
                                portalIDs.Add(result);
                        }
                        if (portalIDs.Count > 0)
                            sqlHelper.ExecuteNonQuery(string.Format("delete from S_A_Portal where ID In ({0})", PortalHelper.SplitCondition(string.Join(",", portalIDs.ToArray()))));
                    }

                    if (!string.IsNullOrEmpty(json) && !json.Equals("{}"))
                    {
                        Dictionary<string, object> list = JsonHelper.ToObject(json);
                        var templetType = list.GetValue("TempletType");
                        if (!string.IsNullOrEmpty(templetType))
                        {
                            colWidth = list.GetValue("ColsWidth");
                            if (templetType.Trim() == "1r2c")
                            {
                                cols = "2";
                            }
                            else if (templetType.Trim() == "custom")
                            {
                                rows = list.GetValue("Rows");
                                cols = list.GetValue("Cols");
                            }
                            templet.Rows = rows;
                            templet.Cols = cols;
                            templet.ColsWidth = colWidth;
                            templet.Items = CalcTemplet(rows, cols, colWidth);
                        }
                    }
                }
                entities.SaveChanges();
            }
        }

        private static string CalcTemplet(string rows, string cols, string colWidth, string portals = "")
        {
            string returnStr = "";
            if (!string.IsNullOrEmpty(rows))
            {
                for (int i = 0; i < Convert.ToInt32(rows); i++)
                {
                    string str = "";
                    string[] width = colWidth.Split('|');
                    for (int j = 0; j < Convert.ToInt32(cols.Split(',')[i]); j++)
                    {
                        string idKey = string.Format(enumDashKey, (i + 1).ToString(), (j + 1).ToString());
                        str += string.Format(enumDashSection, idKey, width[i].Split(',')[j], GetRowColPortal(idKey, portals));
                    }
                    returnStr += string.Format(enumDashRow, str);
                }
            }
            return returnStr;
        }

        private static string GetRowColPortal(string idKey, string portals)
        {
            string returnPortal = "";
            if (!string.IsNullOrEmpty(portals))
            {
                foreach (string item in portals.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string id = item.Split(':')[0];
                        string portal = item.Split(':')[1];
                        if (!string.IsNullOrEmpty(portal))
                        {
                            portal = portal.Replace('&', ',');
                        }
                        if (id == idKey)
                        {
                            if (!string.IsNullOrEmpty(portal))
                            {
                                returnPortal = "{PORTALS}" + portal + "{/PORTALS}";
                            }
                            break;
                        }
                    }
                }
            }
            return returnPortal;
        }

        public static string CreatePortal(string templetID, string portalID, bool isNew, string json)
        {
            try
            {
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                var templet = entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.ID == templetID);

                if (templet != null)
                {
                    var html = templet.Items;
                    Dictionary<string, object> list = JsonHelper.ToObject(json);
                    string type = list.GetValue("Type");
                    if (type != PortalType.Now.ToString())
                    {
                        if (isNew)
                        {
                            string linkUrl = list.GetValue("LinkUrl");
                            string moreUrl = list.GetValue("MoreUrl");
                            S_A_Portal portal = new S_A_Portal();
                            portalID = FormulaHelper.CreateGuid();
                            portal.ID = portalID;
                            portal.Title = list.GetValue("Title");
                            portal.Type = type;
                            if (string.IsNullOrEmpty(linkUrl))
                            {
                                portal.LinkUrl = "/Base/UI/Portal/Views?ID={ID}&portalID=" + portalID;
                            }
                            else
                            {
                                portal.LinkUrl = list.GetValue("LinkUrl");
                            }
                            if (string.IsNullOrEmpty(moreUrl))
                            {
                                portal.MoreUrl = "/Base/UI/Portal/ListView?portalID=" + portalID;
                            }
                            else
                            {
                                portal.MoreUrl = list.GetValue("MoreUrl");
                            }
                            portal.Height = list.GetValue("Height").Trim();
                            portal.DisplayType = list.GetValue("DisplayType").Trim();
                            portal.ConnName = list.GetValue("ConnName");
                            portal.SQL = list.GetValue("SQL");
                            entities.Set<S_A_Portal>().Add(portal);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(portalID))
                            {
                                var portal = entities.Set<S_A_Portal>().SingleOrDefault(c => c.ID == portalID);
                                if (portal != null)
                                {
                                    portal.ID = portalID;
                                    portal.Title = list.GetValue("Title");
                                    portal.Type = type;

                                    portal.ConnName = list.GetValue("ConnName");
                                    portal.SQL = list.GetValue("SQL");
                                    portal.LinkUrl = list.GetValue("LinkUrl");
                                    portal.MoreUrl = list.GetValue("MoreUrl");
                                    portal.Height = list.GetValue("Height").Trim();
                                    portal.DisplayType = list.GetValue("DisplayType").Trim();
                                }
                            }
                        }
                    }
                    else
                    {
                        Regex preg = new Regex(@"\{PORTALS}(.+?){/PORTALS}");
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                        List<string> portalIDs = new List<string>();
                        foreach (Match m in preg.Matches(templet.Items))
                        {
                            string portals = m.Value;
                            string result = Regex.Match(portals, "(?<={PORTALS}).*?(?={/PORTALS})").Value;
                            if (!string.IsNullOrEmpty(result))
                                portalIDs.Add(result);
                        }
                        List<string> nowPortal = new List<string>();
                        portalID = list.GetValue("Portal");
                        foreach (string item in portalID.Split(','))
                        {
                            if (portalIDs.Count > 0)
                            {
                                var query = sqlHelper.ExecuteScalar(string.Format("select 1 from S_A_Portal where ID In ({0}) and LinkUrl like '%{1}.cshtml%'", PortalHelper.SplitCondition(string.Join(",", portalIDs.ToArray())), item));
                                if (Convert.ToInt32(query) > 0)
                                {
                                    throw new Exception("不能添加重复的块，请点击块后进行修改!");
                                }
                            }
                            var nowID = FormulaHelper.CreateGuid();
                            if (!html.Contains(nowID))
                            {
                                entities.Set<S_A_Portal>().Add(new S_A_Portal()
                                {
                                    ID = nowID,
                                    Title = EnumBaseHelper.GetEnumDescription(typeof(Base.Logic.DefaultPortal), item),
                                    Type = PortalType.Link.ToString(),
                                    LinkUrl = string.Format("/PortalLTE/Views/Main/{0}.cshtml", item)
                                });
                                nowPortal.Add(nowID);
                            }
                        }
                        portalID = string.Join(",", nowPortal.ToArray());
                    }

                    int indexOf = html.IndexOf("</section>");
                    string reg = "(?<={PORTALS}).*?(?={/PORTALS})";
                    string firstHtml = html.Substring(0, indexOf);
                    string lastHtml = html.Substring(indexOf, html.Length - indexOf);
                    string MatchValue = Regex.Match(firstHtml, reg).Value;

                    if (string.IsNullOrEmpty(MatchValue))
                    {
                        if (!lastHtml.Contains(portalID))
                            templet.Items = firstHtml + ("{PORTALS}" + portalID + "{/PORTALS}") + lastHtml;
                    }
                    else
                    {
                        List<string> values = new List<string>();
                        foreach (Match m in Regex.Matches(firstHtml, reg))
                        {
                            values.Add(m.Value);
                        }
                        var target = string.Join(",", values.ToArray());
                        if (!firstHtml.Contains(portalID) && !lastHtml.Contains(portalID))
                            values.Add(portalID);
                        firstHtml = firstHtml.Replace(target, string.Join(",", values.ToArray()));
                        templet.Items = firstHtml + lastHtml;
                    }
                    entities.SaveChanges();
                }
            }
            catch (Exception e) { }
            return portalID;
        }

        public static bool SetTemplet(string id, string json)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var templet = entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.ID == id);
            try
            {
                if (templet != null)
                {
                    templet.Items = CalcTemplet(templet.Rows, templet.Cols, templet.ColsWidth, json);
                    entities.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool SaveUserTemplet(string id, string json)
        {
            try
            {
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                var user = FormulaHelper.GetUserInfo();
                var templet = entities.Set<S_A_PortalTemplet>().SingleOrDefault(c => c.ID == id);
                var portalTemplet = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == user.UserID);
                if (templet != null)
                {
                    portalTemplet.PortalSettings = id + ":" + CalcTemplet(templet.Rows, templet.Cols, templet.ColsWidth, json);
                    entities.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static DataTable GetUserTemplet(string searchKey, bool isNew)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var portalTemplet = sqlHelper.ExecuteDataTable(string.Format(@"select a.UserID as ID,b.Code as Account,b.Name,b.DeptID,b.DeptName,a.TempletID,c.Title as TempletName from (                
                select a.UserID,TempletID from S_A__RoleUser a,S_A_TempletRole b
                where a.RoleID=b.RoleID or a.RoleID=b.OrgID 
                union all
                select UserID as ID,TempletID from S_A_TempletRole where isnull(UserID,'')<>'' 
                union all
                select S_A_User.ID,S_A_TempletRole.TempletID
				from S_A_User join S_A__OrgUser on ID=UserID 
				right join S_A_TempletRole on S_A_TempletRole.OrgID=S_A__OrgUser.OrgID
				where IsDeleted='0' 
                ) a
                left join S_A_User b on a.UserID=b.ID 
                left join S_A_PortalTemplet c on a.TempletID=c.ID                
                where (c.IsEnabled is null or c.IsEnabled = 0) {0}
                order by c.Xorder asc,c.Code asc", isNew ? "and c.IsNewPortal='1'" : "and (c.IsNewPortal is null or c.IsNewPortal='')"));

            var query = from t in portalTemplet.AsEnumerable()
                        group t by new { ID = t.Field<string>("ID"), Account = t.Field<string>("Account") } into m
                        select new
                        {
                            ID = m.Key.ID,
                            Account = m.Key.Account
                        };

            DataTable table = portalTemplet.Clone();
            foreach (var item in query.ToList())
            {
                var userRoles = portalTemplet.Select(string.Format(" ID='{0}'", item.ID));
                foreach (var items in userRoles)
                {
                    List<string> portalNames = new List<string>();
                    foreach (var userRole in userRoles)
                    {
                        string tName = Convert.ToString(userRole["TempletName"]);
                        if (!string.IsNullOrEmpty(tName) && !portalNames.Contains(tName))
                            portalNames.Add(tName);
                    }
                    items["TempletName"] = string.Join(",", portalNames.ToArray());
                    table.ImportRow(items);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                Dictionary<string, string> queryDic = JsonHelper.ToObject<Dictionary<string, string>>(searchKey);
                List<string> list = new List<string>();
                foreach (var key in queryDic.Keys)
                {
                    var val = queryDic[key];
                    //处理无值的情况
                    if (!string.IsNullOrEmpty(val))
                    {
                        list.Add(string.Format(" {0} like '%{1}%'", key, val));
                    }
                }
                DataRow[] rows = table.Select(string.Join("or", list.ToArray()));
                if (rows == null || rows.Length == 0) return null;
                DataTable tmp = rows[0].Table.Clone();
                foreach (DataRow row in rows)
                {
                    tmp.ImportRow(row);
                }
                table = tmp;
            }
            return table;
        }

        public static DataTable GetUserRoleTemplets(string userID = "")
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            if (IsUseNewPortal && string.IsNullOrEmpty(userID))
            {
                return sqlHelper.ExecuteDataTable("select ID as TempletID,Title,Xorder,Items,Rows,Cols from S_A_PortalTemplet where IsNewPortal = '2'");
            }
            else
            {
                return sqlHelper.ExecuteDataTable(string.Format(@"select c.ID as TempletID,c.Title,c.Xorder,c.Items,PortalSettings,c.Rows,c.Cols from (                
                select a.UserID,TempletID from S_A__RoleUser a,S_A_TempletRole b
                where a.RoleID=b.RoleID or a.RoleID=b.OrgID 
                union all
                select UserID as ID,TempletID from S_A_TempletRole where isnull(UserID,'')<>'' 
                union all
                select S_A_User.ID,S_A_TempletRole.TempletID
				from S_A_User join S_A__OrgUser on ID=UserID 
				right join S_A_TempletRole on S_A_TempletRole.OrgID=S_A__OrgUser.OrgID
				where IsDeleted='0' 
                ) a
                left join S_A_User b on a.UserID=b.ID 
                left join S_A_PortalTemplet c on a.TempletID=c.ID                
                where (c.IsEnabled is null or c.IsEnabled = 0) {0} {1}
                group by c.ID,c.Title,c.Xorder,c.Items,PortalSettings,c.Rows,c.Cols
                order by c.Xorder
                ", string.IsNullOrEmpty(userID) ? "" : string.Format(" and a.UserID='{0}'", userID),
                     IsUseNewPortal ? "and IsNewPortal='1'" : "and (IsNewPortal is null or IsNewPortal = 0)"));
            }
        }
        public static string GetUserTemplets(string templetID)
        {
            StringBuilder sb = new StringBuilder();
            DataTable table = GetUserRoleTemplets(FormulaHelper.UserID);
            sb.Append("<div class=\"templet-default\">");
            string defaultID = templetID;
            if (table.Rows.Count > 1)
            {
                foreach (DataRow dr in table.Rows)
                {
                    var id = dr["TempletID"].ToString();
                    var title = dr["Title"].ToString();
                    var portalSettings = dr["PortalSettings"].ToString();
                    string dfDiv = string.Format("<div id=\"df-{0}\" onclick=\"openUserTemplet('{0}')\"><span>{1}</span><img src=\"/PortalLTE/Scripts/Portal/switch.png\" /></div>", id, title);
                    if (!string.IsNullOrEmpty(portalSettings))
                    {
                        string sid0 = portalSettings.Split(':')[0];
                        if (string.IsNullOrEmpty(templetID))
                        {
                            if (id == sid0)
                            {
                                defaultID = id;
                                sb.Append(dfDiv);
                            }
                        }
                        else
                        {
                            if (id == templetID)
                            {
                                defaultID = id;
                                sb.Append(dfDiv);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(templetID))
                        {
                            defaultID = id;
                            sb.Append(dfDiv);
                            break;
                        }
                        else
                        {
                            if (id == templetID)
                            {
                                defaultID = id;
                                sb.Append(dfDiv);
                            }
                        }
                    }
                }
            }
            sb.Append("</div><div class=\"templet-list\">");
            if (table.Rows.Count > 1)
            {
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    var id = table.Rows[i]["TempletID"].ToString();
                    var title = table.Rows[i]["Title"].ToString();
                    var rows = table.Rows[i]["Rows"].ToString();
                    var cols = table.Rows[i]["Cols"].ToString();
                    sb.Append(string.Format("<div class=\"templet-switch {1}\" id=\"{0}\" style=\"margin-left: {2}px;\" onclick=\"switchTemplet('{0}')\"><div class=\"templet-wrap\"><div class=\"templet-swrap\">", id, id == defaultID ? "selected" : "", i == 0 ? 15 : 0));
                    sb.Append(string.Format("<div class=\"templet-top\"><div class=\"templet-number\">{0}</div><div class=\"templet-title\">{1}</div><img src=\"/PortalLTE/Scripts/Portal/select.png\"/></div>", (i + 1).ToString(), title));
                    sb.Append(string.Format("<div class=\"templet-{0}\"></div></div></div></div><div>", rows == "1" && cols == "2" ? "r1c2" : rows == "1" && cols == "3" ? "r1c3" : "custom"));
                }
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        private static string GetToken()
        {
            var user = FormulaHelper.GetUserInfo();
            var token = "";
            var secretKey = String.IsNullOrEmpty(ConfigurationManager.AppSettings["SecretKey"]) ? String.Empty : ConfigurationManager.AppSettings["SecretKey"];
            if (!String.IsNullOrEmpty(secretKey))
            {
                var dic = new Dictionary<string, object>();
                dic["systemName"] = user.Code;
                var expiredTimeSpan = 1;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ExpiredTimeSpan"]))
                {
                    var timeSpan = ConfigurationManager.AppSettings["ExpiredTimeSpan"];
                    if (System.Text.RegularExpressions.Regex.IsMatch(timeSpan, "^[1-9]\\d*$"))
                    {
                        expiredTimeSpan = Convert.ToInt32(timeSpan);
                    }
                }
                var jwtcreatedOver =
    Math.Round((DateTime.UtcNow.AddMinutes(expiredTimeSpan) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);
                dic["exp"] = jwtcreatedOver;// 指定token的生命周期。unix时间戳格式               
                IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                token = encoder.Encode(dic, secretKey);
            }
            return token;
        }
    }
}
