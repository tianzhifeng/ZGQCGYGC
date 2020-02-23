using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config.Logic;
using Base.Logic.Domain;
using Formula.Helper;
using Base.Logic;
using System.IO;
using MvcAdapter;
using System.Configuration;
using Formula;

namespace MvcConfig.Areas.UI.Controllers
{
    public class BIController : BaseController
    {
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                return Formula.FormulaHelper.GetEntities<BaseEntities>();
            }
        }

        public ActionResult PageView()
        {
            string tmpCode = this.GetQueryString("TmplCode");

            var config = this.entities.Set<S_UI_BIConfig>().FirstOrDefault(d => d.Code == tmpCode);
            if (config == null) throw new Exception("未能找到编号为【" + tmpCode + "】的BI定义对象");
            var layout = JsonHelper.ToList(config.Layout);
            var blocks = JsonHelper.ToList(config.Blocks);
            foreach (var item in blocks)
            {
                //去除DataSource属性不在页面数据岛上显示，以避免防止SQL注入攻击
                var settings = JsonHelper.ToObject(item.GetValue("Settings"));
                settings.Remove("dataSource");
                if (JsonHelper.ToJson(settings).IndexOf("FilterInfo") >= 0)
                {
                    var FilterInfo = JsonHelper.ToList(settings["FilterInfo"].ToString());
                    for (int i = 0; i < FilterInfo.Count(); i++)
                    {
                        if (JsonHelper.ToJson(FilterInfo[i]).IndexOf("EnumData") >= 0 && !string.IsNullOrEmpty(FilterInfo[i]["EnumData"].ToString()) && FilterInfo[i]["EnumData"].ToString().IndexOf("{") < 0)
                        {
                            IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                            FilterInfo[i]["EnumData"] = JsonHelper.ToJson(enumService.GetEnumDataSource(FilterInfo[i]["EnumData"].ToString()));
                        }
                    }
                    settings["FilterInfo"] = FilterInfo;
                }
                item.SetValue("Settings", JsonHelper.ToJson(settings));
            }
            ViewBag.LayOutInfo = layout;
            ViewBag.Blocks = blocks;
            ViewBag.PageTitle = "";
            ViewBag.LayOutJson = config.Layout;
            ViewBag.BlocksJson = JsonHelper.ToJson(blocks);
            ViewBag.RowsCount = layout.Count;
            ViewBag.ColumnsCount = layout.Count == 0 ? 0 : layout.Max(c => Convert.ToInt32(c["ColumnNumber"]));
            var templateInfo = new Dictionary<string, object>();
            foreach (var item in blocks)
            {
                var BlockType = item.GetValue("BlockType");
                string filePath = HttpContext.Server.MapPath(String.Format("/MvcConfig/Scripts/BI/template/div/{0}.html", BlockType));
                if (System.IO.File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(fs);
                    var divHtml = reader.ReadToEnd();
                    reader.Close();
                    fs.Close();
                    item.SetValue("divHtml", MvcHtmlString.Create(divHtml));
                }
                if (!templateInfo.ContainsKey(BlockType))
                {
                    string templatefilePath = HttpContext.Server.MapPath(String.Format("/MvcConfig/Scripts/BI/template/templ/{0}.html", BlockType));
                    if (System.IO.File.Exists(templatefilePath))
                    {
                        FileStream tmplfs = new FileStream(templatefilePath, FileMode.Open, FileAccess.Read);
                        StreamReader tmplreader = new StreamReader(tmplfs);
                        var tmplHtml = tmplreader.ReadToEnd();
                        tmplreader.Close();
                        tmplfs.Close();
                        templateInfo.SetValue(BlockType, MvcHtmlString.Create(tmplHtml));
                    }
                }
            }
            ViewBag.Script = HttpContext.Server.HtmlDecode(config.ScriptText);
            ViewBag.BlockTemplate = templateInfo;
            ViewBag.FullScreen = false;
            if (this.GetQueryString("FullScreen").Trim().ToLower() == true.ToString().ToLower())
            {
                ViewBag.FullScreen = true;
            }
            return View();
        }

        public ActionResult MobileView()
        {
            string tmpCode = this.GetQueryString("TmplCode");
            string title = this.GetQueryString("Title");
            if (!string.IsNullOrEmpty(title))
            {
                ViewBag.Title = title;
            }
            else
            {
                ViewBag.Title = "图表";
            }
            string tokenKey = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenKey"]) ? ConfigurationManager.AppSettings["TokenKey"] : "GWToken";
            var token = this.GetQueryString(tokenKey);
            if (!String.IsNullOrEmpty(token))
            {
                //根据密钥解析token，获取用户信息
                var secretKey = String.IsNullOrEmpty(ConfigurationManager.AppSettings["SecretKey"]) ? String.Empty : ConfigurationManager.AppSettings["SecretKey"];
                if (String.IsNullOrEmpty(secretKey))
                {
                    throw new Formula.Exceptions.BusinessException("系统未设置密钥，进行验证操作");
                }
                JWT.IJsonSerializer serializer = new JWT.Serializers.JsonNetSerializer();
                JWT.IDateTimeProvider provider = new JWT.UtcDateTimeProvider();
                JWT.IJwtValidator validator = new JWT.JwtValidator(serializer, provider);
                JWT.IBase64UrlEncoder urlEncoder = new JWT.JwtBase64UrlEncoder();
                JWT.IJwtDecoder decoder = new JWT.JwtDecoder(serializer, validator, urlEncoder);
                //var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
                var json = decoder.Decode(token, secretKey, verify: true);
                var dic = Formula.Helper.JsonHelper.ToObject(json);
                var sysName = dic.GetValue("systemName");

                //判断是否过期
                bool isExpire = false;
                if (!String.IsNullOrEmpty(dic.GetValue("ExpiredTime")))
                {
                    //兼容以前用自定义字段判断过期时间的写法，目前使用JWT固有的判断超时的字段“exp”，JWT校验的时候会失效，不用写代码判断是否失效
                    var expirationTime = String.IsNullOrEmpty(dic.GetValue("ExpiredTime")) ? DateTime.Now : Convert.ToDateTime(dic.GetValue("ExpiredTime"));
                    isExpire = DateTime.Now > expirationTime;
                }
                FormulaHelper.SetAuthCookie(sysName);
            }
            else
            {
                var sysName = this.GetQueryString("Account");
                FormulaHelper.SetAuthCookie(sysName);
                if (!Formula.AuthCodeHelper.CheckTokenRole() && !string.IsNullOrEmpty(sysName))
                {
                    return Redirect(HttpContext.Request.Url.ToString());
                }
            }


            var config = this.entities.Set<S_UI_BIConfig>().FirstOrDefault(d => d.Code == tmpCode);
            if (config == null) throw new Exception("未能找到编号为【" + tmpCode + "】的BI定义对象");
            var layout = JsonHelper.ToList(config.Layout);
            var blocks = JsonHelper.ToList(config.Blocks);
            foreach (var item in blocks)
            {
                //去除DataSource属性不在页面数据岛上显示，以避免防止SQL注入攻击
                var settings = JsonHelper.ToObject(item.GetValue("Settings"));
                settings.Remove("dataSource");
                if (JsonHelper.ToJson(settings).IndexOf("FilterInfo") >= 0)
                {
                    var FilterInfo = JsonHelper.ToList(settings["FilterInfo"].ToString());
                    for (int i = 0; i < FilterInfo.Count(); i++)
                    {
                        if (JsonHelper.ToJson(FilterInfo[i]).IndexOf("EnumData") >= 0 && !string.IsNullOrEmpty(FilterInfo[i]["EnumData"].ToString()) && FilterInfo[i]["EnumData"].ToString().IndexOf("{") < 0)
                        {
                            IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                            FilterInfo[i]["EnumData"] = JsonHelper.ToJson(enumService.GetEnumDataSource(FilterInfo[i]["EnumData"].ToString()));
                        }
                    }
                    settings["FilterInfo"] = FilterInfo;
                }
                item.SetValue("Settings", JsonHelper.ToJson(settings));
            }
            ViewBag.LayOutInfo = layout;
            ViewBag.Blocks = blocks;
            ViewBag.PageTitle = "";
            ViewBag.LayOutJson = config.Layout;
            ViewBag.BlocksJson = JsonHelper.ToJson(blocks);
            ViewBag.RowsCount = layout.Count;
            ViewBag.ColumnsCount = layout.Count == 0 ? 0 : layout.Max(c => Convert.ToInt32(c["ColumnNumber"]));
            var templateInfo = new Dictionary<string, object>();
            foreach (var item in blocks)
            {
                var BlockType = item.GetValue("BlockType");
                string filePath = HttpContext.Server.MapPath(String.Format("/MvcConfig/Scripts/BI/template/div/{0}.html", BlockType));
                if (System.IO.File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(fs);
                    var divHtml = reader.ReadToEnd();
                    reader.Close();
                    fs.Close();
                    item.SetValue("divHtml", MvcHtmlString.Create(divHtml));
                }
                if (!templateInfo.ContainsKey(BlockType))
                {
                    string templatefilePath = HttpContext.Server.MapPath(String.Format("/MvcConfig/Scripts/BI/template/templ/{0}.html", BlockType));
                    if (System.IO.File.Exists(templatefilePath))
                    {
                        FileStream tmplfs = new FileStream(templatefilePath, FileMode.Open, FileAccess.Read);
                        StreamReader tmplreader = new StreamReader(tmplfs);
                        var tmplHtml = tmplreader.ReadToEnd();
                        tmplreader.Close();
                        tmplfs.Close();
                        templateInfo.SetValue(BlockType, MvcHtmlString.Create(tmplHtml));
                    }
                }
            }
            ViewBag.Script = HttpContext.Server.HtmlDecode(config.ScriptText);
            ViewBag.BlockTemplate = templateInfo;
            ViewBag.FullScreen = false;
            if (this.GetQueryString("FullScreen").Trim().ToLower() == true.ToString().ToLower())
            {
                ViewBag.FullScreen = true;
            }
            return View();
        }
        [ValidateInput(false)]
        public JsonResult GetBlockData(string BlockID, string TmplCode, string isMobile = "false")
        {
            string filters = this.GetQueryString("filters");
            string filter = "";
            if (!string.IsNullOrEmpty(filters))
            {
                var filtersdic = JsonHelper.ToObject<List<Dictionary<string, string>>>(filters);
                foreach (var item in filtersdic)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = " where " + item["name"] + " like " + "'%" + item["value"] + "%' ";
                    }
                    else
                    {
                        filter = filter + "and " + item["name"] + " like " + "'%" + item["value"] + "%' ";
                    }
                }
            }

            var result = new Dictionary<string, object>();
            var biConfig = this.entities.Set<S_UI_BIConfig>().FirstOrDefault(c => c.Code == TmplCode);
            if (biConfig == null) throw new Exception("未能找到编号为【" + TmplCode + "】的BI定义对象");
            var blocks = JsonHelper.ToList(biConfig.Blocks);
            var block = blocks.FirstOrDefault(c => c.GetValue("_id") == BlockID);
            if (block == null) throw new Exception("未能找到块定义，ID【" + BlockID + "】");
            try
            {
                var settings = JsonHelper.ToObject(block.GetValue("Settings"));
                if (JsonHelper.ToJson(settings).IndexOf("dataSource") >= 0)
                {
                    if (!string.IsNullOrEmpty(settings["dataSource"].ToString()) && !string.IsNullOrEmpty(filter))
                    {
                        var dataSource = JsonHelper.ToList(settings.GetValue("dataSource"));
                        if (!string.IsNullOrEmpty(settings["dataSource"].ToString()) && !string.IsNullOrEmpty(dataSource.FirstOrDefault()["SQL"].ToString()))
                        {
                            dataSource.FirstOrDefault()["SQL"] = "select * from (" + dataSource.FirstOrDefault()["SQL"] + ") filter " + filter;
                            settings.SetValue("dataSource", dataSource);
                            block.SetValue("Settings", JsonHelper.ToJson(settings));
                        }
                    }
                }
                var component = BIComponentFactory.CreateComponent(block.GetValue("BlockType"), JsonHelper.ToJson(block));
                if (component == null)
                {
                    return Json("");
                }
                if (isMobile == "true")
                {
                    var data = component.Render("", true);
                    result.SetValue("Data", data);
                }
                else
                {
                    var data = component.Render();
                    result.SetValue("Data", data);
                }

                result.SetValue("TemplateType", block.GetValue("BlockType"));
                result.SetValue("ID", component.ID);

                result.SetValue("State", "1");
                return Json(result);
            }
            catch (Exception exp)
            {
                result.SetValue("State", "0");
                result.SetValue("Msg", exp.Message);
                result.SetValue("ID", block.GetValue("_id"));
                return Json(result);
            }
        }

        /// <summary>
        /// 获取表格数据分页.
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult GetGridPageData(string paramsql, string paramconnname, string qb, string gridid)
        {
            var result = new Dictionary<string, object>();
            try
            {
                var component = Base.Logic.BIComponentFactory.CreateComponent("Grid", "");
                var data = component.GetGridPageData(JsonHelper.ToObject<QueryBuilder>(qb), paramsql, paramconnname);

                result.SetValue("GridID", gridid);
                result.SetValue("Data", data);
                return Json(result);
            }
            catch (Exception exp)
            {
                result.SetValue("Error", "0");
                result.SetValue("GridID", gridid);
                result.SetValue("Msg", exp.Message);
                return Json(result);
            }
        }
    }
}
