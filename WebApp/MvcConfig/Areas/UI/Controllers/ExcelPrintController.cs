using Config;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcConfig.Areas.UI.Controllers
{
    public class ExcelPrintController : Controller
    {

        public FileResult ExcelTmpl(string tmplCode)
        {
            string filePath = Server.MapPath("/ExcelPrint") + string.Format("/{0}.xlsx", tmplCode);

            return File(filePath, "application/octet-stream ;", tmplCode + ".xlsx");
        }

        public JsonResult GetExcelPrintData(string tmplCode, string id)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_UI_ExcelPrint where Code='{0}'", tmplCode));
            string connName = dt.Rows[0]["ConnName"].ToString();
            string tableName = dt.Rows[0]["TableName"].ToString();
            string itemsString = dt.Rows[0]["Items"].ToString();


            sqlHelper = SQLHelper.CreateSqlHelper(connName);
            dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", tableName, id));
            var dataRow = dt.Rows[0];

            var itemSettings = JsonHelper.ToObject<List<Dictionary<string, string>>>(itemsString);

            foreach (var item in itemSettings)
            {
                var itemType = item["ItemType"];
                var itemCode = item["Code"];

                if (itemType == "Enum")
                {
                    var enumSettingsDic = JsonHelper.ToObject(item["Settings"]);
                    dataRow[itemCode] = GetEnumText(enumSettingsDic["data"].ToString(), dataRow[itemCode].ToString());
                }
                else if (itemType == "Sign")
                {
                    string str = dataRow[itemCode].ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        var tmpList = JsonHelper.ToList(dataRow[itemCode].ToString());
                        dataRow[itemCode] = tmpList[0]["ExecUserID"].ToString();
                    }
                }
                else if (itemType == "SubTable")
                {
                    var dic = JsonHelper.ToObject(item["Settings"]);
                    var listDic = JsonHelper.ToList(dic["listData"].ToString());

                    var subDT = sqlHelper.ExecuteDataTable(string.Format("select * from {0}_{1} where {0}ID='{2}'", tableName, itemCode, id));
                    if (subDT.Columns.Contains("XH") == false)
                        subDT.Columns.Add("XH");

                    for (int i = 0; i < subDT.Rows.Count; i++)
                    {
                        subDT.Rows[i]["XH"] = i + 1;
                    }

                    foreach (var subItem in listDic)
                    {
                        if (subItem["ItemType"].ToString() == "Enum")
                        {
                            var subEnumSettingsDic = JsonHelper.ToObject(subItem["Settings"].ToString());

                            var subItemCode = subItem["Code"].ToString();
                            foreach (DataRow subRow in subDT.Rows)
                            {
                                subRow[subItemCode] = GetEnumText(subEnumSettingsDic["data"].ToString(), subRow[subItemCode].ToString());
                            }
                        }
                    }

                    if (!dt.Columns.Contains(itemCode))
                        dt.Columns.Add(itemCode);

                    dataRow[itemCode] = JsonHelper.ToJson(subDT);
                }

            }

            dt.Columns.Add("ItemSettings");
            dataRow["ItemSettings"] = itemsString;

            var json = JsonHelper.ToJson(dt);

            return Json(json);
        }



        #region 私有方法

        private string GetEnumText(string enumStr, string val)
        {
            if (enumStr.StartsWith("["))
            {
                var list = JsonHelper.ToList(enumStr);
                foreach (var item in list)
                {
                    if (item["value"].ToString() == val)
                        return item["text"].ToString();
                }
            }
            else
            {
                SQLHelper sqlhelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var dt = sqlhelper.ExecuteDataTable(string.Format("select S_M_EnumItem.* from S_M_EnumItem join S_M_EnumDef on EnumDefID=S_M_EnumDef.ID where S_M_EnumDef.Code='{0}'", enumStr));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Code"].ToString() == val)
                        return row["Name"].ToString();
                }
            }

            return val;
        }

        #endregion

    }
}
