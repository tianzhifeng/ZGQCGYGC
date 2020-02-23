using Base.Logic.Domain;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Config.Logic;
using System.IO;

namespace MvcConfig.Controllers
{
    public class ChartAPIController : ApiController
    {
        /// <summary>
        /// 获取定义.
        /// </summary>
        /// <returns></returns>
        [ActionName("Blocks")]
        [HttpGet]
        public Dictionary<string, object> _GetBlocks(string blockID = "", string blockViewKey = "")
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var tmpCode = Request.RequestUri.ParseQueryString().Get("TmplCode");
            var config = baseEntities.Set<S_UI_BIConfig>().FirstOrDefault(d => d.Code == tmpCode);
            if (config == null) throw new Exception("未能找到编号为【" + tmpCode + "】的BI定义对象");
            var templateInfo = new Dictionary<string, object>();
            var blocks = JsonHelper.ToList(config.Blocks);
            string blocksJson = config.Blocks;
            if (!string.IsNullOrEmpty(blockID))
            {
                blocks = blocks.Where(c => c.GetValue("_id") == blockID).ToList();
                var list = JsonHelper.ToList(blocksJson);
                if (list != null)
                {
                    blocksJson = JsonHelper.ToJson(list.Where(c => c.GetValue("_id") == blockID).ToList());
                }
            }
            foreach (var item in blocks)
            {
                var BlockType = item.GetValue("BlockType");
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath(String.Format("/MvcConfig/Scripts/BI/template/div/{0}.html", BlockType));
                if (System.IO.File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(fs);
                    var divHtml = reader.ReadToEnd();
                    reader.Close();
                    fs.Close();
                    item.SetValue("divHtml", divHtml);
                }
                if (!templateInfo.ContainsKey(BlockType))
                {
                    string templatefilePath = System.Web.Hosting.HostingEnvironment.MapPath(String.Format("/MvcConfig/Scripts/BI/template/templ/{0}.html", BlockType));
                    if (System.IO.File.Exists(templatefilePath))
                    {
                        FileStream tmplfs = new FileStream(templatefilePath, FileMode.Open, FileAccess.Read);
                        StreamReader tmplreader = new StreamReader(tmplfs);
                        var tmplHtml = tmplreader.ReadToEnd();
                        tmplreader.Close();
                        tmplfs.Close();
                        templateInfo.SetValue(BlockType, tmplHtml);
                    }
                }
            }

            string templateHTML = "";
            dic.Add("BlocksJson", blocksJson);
            dic.Add("Blocks", blocks);
            foreach (var item in templateInfo.Keys)
            {
                templateHTML += templateInfo[item].ToString().Replace(item, blockViewKey + item);
            }
            dic.Add("Template", templateHTML);
            return dic;
        }

        /// <summary>
        /// 获取数据.
        /// </summary>
        /// <returns></returns>
        [ActionName("GetBlockData")]
        [HttpPost]
        public Dictionary<string, object> _GetBlockData([FromBody] BlockDataDTO blockData)
        {
            Formula.AuthCodeHelper.CheckTokenRole();
            var result = new Dictionary<string, object>();
            var block = JsonHelper.ToObject(blockData.Block);
            try
            {
                var component = Base.Logic.BIComponentFactory.CreateComponent(blockData.TemplateType, blockData.Block);
                if (component == null)
                {
                    return result;
                }
                var data = component.Render(blockData.Parameters, true);
                result.SetValue("TemplateType", blockData.BlockViewKey + blockData.TemplateType);
                result.SetValue("ID", component.ID);
                result.SetValue("Data", data);
                result.SetValue("State", "1");
                return result;
            }
            catch (Exception exp)
            {
                result.SetValue("State", "0");
                result.SetValue("Msg", exp.Message);
                result.SetValue("ID", block.GetValue("_id"));
                return result;
            }
        }
    }

    public class BlockDataDTO
    {
        /// <summary>
        /// 模板
        /// </summary>
        public string TemplateType { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Block { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 浏览临时Key
        /// </summary>
        public string BlockViewKey { get; set; }
        /// <summary>
        /// 移动端
        /// </summary>
        public bool IsMobile { get {return true;}}
    }
}