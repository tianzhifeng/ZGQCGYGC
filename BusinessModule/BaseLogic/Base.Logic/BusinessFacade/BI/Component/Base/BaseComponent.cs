using Base.Logic.BusinessFacade;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Config.Logic;
using Config;
using System.Text.RegularExpressions;
using Formula;
using MvcAdapter;

namespace Base.Logic
{
    public abstract class BaseComponent : IBIComponent
    {
        protected UIFO fo = new UIFO();

        public BaseComponent(string blockDef)
        {
            var block = JsonHelper.ToObject(blockDef);
            this.BlockDef = String.IsNullOrEmpty(block.GetValue("Settings")) ? new Dictionary<string, object>() :
               JsonHelper.ToObject(block.GetValue("Settings"));
            this.ID = block.GetValue("_id");
        }

        protected void FillDataSource(string parameters = "")
        {
            var dataSource = new Dictionary<string, DataTable>();
            if (!String.IsNullOrEmpty(this.BlockDef.GetValue("dataSource")))
            {
                var dataSourceDefList = JsonHelper.ToList(this.BlockDef.GetValue("dataSource"));
                foreach (var dataSourceDef in dataSourceDefList)
                {
                    var db = SQLHelper.CreateSqlHelper(dataSourceDef.GetValue("ConnName"));
                    var sql = dataSourceDef.GetValue("SQL");

                    if (!string.IsNullOrEmpty(parameters))
                    {
                        var paras = JsonHelper.ToObject<Dictionary<string, string>>(parameters);
                        Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
                        var user = FormulaHelper.GetUserInfo();
                        foreach (Match m in reg.Matches(sql))
                        {
                            string key = m.Value.Trim('{', '}');
                            var value = paras.GetValue(key);
                            switch (key)
                            {
                                case Formula.Constant.CurrentUserID:
                                    value = user.UserID;
                                    break;
                                case Formula.Constant.CurrentUserName:
                                    value = user.UserName;
                                    break;
                                case Formula.Constant.CurrentUserOrgID:
                                    value = user.UserOrgID;
                                    break;
                                case Formula.Constant.CurrentUserOrgCode:
                                    value = user.UserOrgCode;
                                    break;
                                case Formula.Constant.CurrentUserOrgName:
                                    value = user.UserOrgName;
                                    break;
                                case Formula.Constant.CurrentUserPrjID:
                                    value = user.UserPrjID;
                                    break;
                                case Formula.Constant.CurrentUserPrjName:
                                    value = user.UserPrjName;
                                    break;
                                case "CurrentUserOrgFullName":
                                    value = user.UserFullOrgName;
                                    break;
                                case "CurrentUserCorpID":
                                    value = user.UserCompanyID;
                                    break;
                                case "CurrentUserCorpName":
                                    value = user.UserCompanyName;
                                    break;
                                case "CurrentTime":
                                    value = DateTime.Now.ToString();
                                    break;
                                case "CurrentDate":
                                    value = DateTime.Now.Date.ToString("yyyy-MM-dd");
                                    break;
                                case "CurrentYear":
                                    value = DateTime.Now.Year.ToString();
                                    break;
                                case "CurrentMonth":
                                    value = DateTime.Now.Month.ToString();
                                    break;
                                case "CurrentQuarter":
                                    value = ((DateTime.Now.Month + 2) / 3).ToString();
                                    break;
                                case "CurrentUserOrgFullID":
                                    value = user.UserFullOrgID;
                                    break;
                            }

                            sql = sql.Replace(m.Value, value);
                        }
                    }
                    sql = fo.ReplaceString(sql);                   
                    var dt = db.ExecuteDataTable(sql);
                    dataSource.SetValue(dataSourceDef.GetValue("Code"), dt);
                }
            }
            this.DataSource = dataSource;
        }

        public abstract Dictionary<string, object> Render(string parameters = "", bool IsMobile = false);

        public GridData GetGridPageData(QueryBuilder qb, string sql, string ConnName)
        {
            var dataSource = new Dictionary<string, DataTable>();
            var db = SQLHelper.CreateSqlHelper(ConnName);
            #region 设置过滤
            var filters = JsonHelper.ToList(this.BlockDef.GetValue("FilterInfo"));
            foreach (var filter in filters)
            {
                var enumData = filter.GetValue("EnumData");
                if (!String.IsNullOrEmpty(enumData))
                {
                    var enumList = JsonHelper.ToList(enumData);
                    foreach (var item in enumList)
                    {
                        if (String.IsNullOrEmpty(item.GetValue("IsDefault")))
                        {
                            item.SetValue("IsDefault", "false");
                        }
                    }
                    filter.SetValue("EnumData", enumList);
                }
            }

            #endregion
            sql = fo.ReplaceString(sql);
            var dt = db.ExecuteGridData(sql, qb);
            return dt;
        }
        public virtual Dictionary<string, object> BlockDef
        {
            get;
            set;
        }

        public virtual Dictionary<string, System.Data.DataTable> DataSource
        {
            get;
            set;
        }


        public string ID
        {
            get;
            set;
        }



        protected void setStyle(Dictionary<string, object> result)
        {
            #region 设置样式
            var style = String.IsNullOrEmpty(this.BlockDef.GetValue("Style")) ? "lineblue" : "line" + this.BlockDef.GetValue("Style");
            var linecolor = "";
            switch (style)
            {
                case "lineblue":
                    linecolor = "#1c90db";
                    break;
                case "skyblue":
                    linecolor = "#00b4c1";
                    break;
                case "linered":
                    linecolor = "#c83841";
                    break;
                case "linegreen":
                    linecolor = "#7ebf01";
                    break;
                case "linepurple":
                    linecolor = "#e265cb";
                    break;
                case "lineorange":
                    linecolor = "#ed9b38";
                    break;
            }
            result.SetValue("Color", linecolor);
            result.SetValue("Style", style);
            #endregion
        }

        public string ReplaceRegString(string regString, Dictionary<string, object> data, List<Dictionary<string, object>> EnumDefine)
        {
            Regex reg = new Regex(@"{[^}]+}", RegexOptions.IgnoreCase);
            string result = reg.Replace(regString, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var sValue = data.GetValue(value);
                if (EnumDefine != null && EnumDefine.Count > 0)
                {
                    var define = EnumDefine.FirstOrDefault(d => d["FieldName"].ToString() == value);
                    if (define != null)
                    {
                        var enumValue = "";
                        var EnumService = FormulaHelper.GetService<IEnumService>();
                        foreach (var item in sValue.Split(','))
                        {
                            enumValue += EnumService.GetEnumText(define.GetValue("EnumKey"), item) + ",";
                        }
                        return enumValue.TrimEnd(',');
                    }
                }
                return sValue;
            });
            return result;
        }
    }
}
