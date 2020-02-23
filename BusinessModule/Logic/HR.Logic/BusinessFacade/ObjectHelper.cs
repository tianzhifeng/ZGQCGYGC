using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Data;
using System.Text.RegularExpressions;
using Formula.Helper;
using MvcAdapter;
using Newtonsoft.Json;
using Formula;

namespace HR.Logic.BusinessFacade
{
    public class ExportColumn
    {
        public string Header { get; set; }
        public string FieId { get; set; }
        public int Wdith { get; set; }
        public Type DateType { get; set; }
    }

    public class ObjectHelper
    {

        

        public static string ObjectToString(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return string.Empty;
            return obj.ToString();
        }

        public static int ObjectToInt(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return 0;
            int intValue = 0;
            try
            {
                intValue = Convert.ToInt32(obj.ToString());
            }
            catch
            {
                return 0;
            }
            return intValue;
        }

        public static long ObjectToLongInt(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return 0;
            long intValue = 0;
            try
            {
                intValue = Convert.ToInt64(obj.ToString());
            }
            catch
            {
                return 0;
            }
            return intValue;
        }
        public static double ObjectToDouble(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return 0;
            double fValue = 0;
            try
            {
                fValue = Convert.ToDouble(obj.ToString());
            }
            catch
            {
                return 0;
            }
            return fValue;
        }
        public static string ObjectToNbsp(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return "&nbsp";
            return obj.ToString();
        }




        #region 字符串帮助函数

        /// <summary>
        /// string转换为Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int StrToIntZero(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// string转换为Double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double StrToFloatZero(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return 0.00;
            }
            else
            {
                return Convert.ToDouble(obj);
            }
        }


        /// <summary>
        /// object转换为Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ObjToIntZero(object obj)
        {
            if (obj == null || obj.ToString().Trim() == "")
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// object转换为Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Decimal ObjToDecimal(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return 0.0M;
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }

        /// <summary>
        /// object转换为Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float ObjToFloat(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return 0;
            }
            else
            {
                return (float)obj;
            }
        }


        /// <summary>
        /// object转换为Str
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToStr(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            else
            {
                return obj.ToString();
            }
        }


        #endregion

        #region --字符串验证函数

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            Regex r = new Regex(@"^\d+(\.)?\d*$");
            return r.IsMatch(value);
        }

        #endregion

        /// <summary>
        /// 附件名称格式化
        /// </summary>
        /// <param name="attachment">数据库中的附件字段值</param>
        /// <returns>格式化后的附件名称</returns>
        public static string FormatAttachmentName(string attachment)
        {
            if (String.IsNullOrEmpty(attachment))
            {
                return "";
            }
            string[] strArray = attachment.Split(',');
            string[] strTempArray = new string[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                int end = strArray[i].LastIndexOf('_');
                string strTemp = strArray[i].Substring(strArray[i].IndexOf('_') + 1,
                    strArray[i].Substring(0, end).Length - (strArray[i].IndexOf('_') + 1));
                strTempArray[i] = strTemp;
            }
            return String.Join(",", strTempArray);
        }

        
        public static void SyncPasscode(string sysName, string Password) 
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt_pwd_code = sqlHelper.ExecuteDataTable(string.Format("select * from PasscodeInfo where SystemName='{0}'", sysName));
            if (dt_pwd_code.Rows.Count <= 0)
            {
                sqlHelper.ExecuteNonQuery(string.Format("insert into PasscodeInfo values('{0}','{1}')", sysName, Password));
            }
            else
            {
                sqlHelper.ExecuteNonQuery(string.Format("update  PasscodeInfo set PassCode='{0}' Where SystemName='{1}'", Password, sysName));
            }         
        }

        /// <summary>
        /// 把一个对象的属性值赋值到另外一个对象里
        /// </summary>
        /// <typeparam name="T">原对象</typeparam>
        /// <typeparam name="L">要赋值的对象</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static L SetProperties<T, L>(T t) where L : new()
        {
            if (t == null)
            {
                return default(L);
            }
            System.Reflection.PropertyInfo[] propertiesT = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            System.Reflection.PropertyInfo[] propertiesL = typeof(L).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            //if (propertiesT.Length != propertiesL.Length || propertiesL.Length == 0)
            //{
            //    return default(L);
            //}
            L setT = new L();
            foreach (System.Reflection.PropertyInfo itemL in propertiesL)
            {
                foreach (System.Reflection.PropertyInfo itemT in propertiesT)
                {
                    if (itemL.Name == itemT.Name)
                    {
                        object value = itemT.GetValue(t, null);
                        itemL.SetValue(setT, value, null);
                    }
                }
            }
            return setT;
        }


        /// <summary>
        /// 消除流程界面重复点击暂存导致编号自增的方法
        /// </summary>
        /// <param name="sh">数据库</param>
        /// <param name="TableName">表名</param>
        /// <param name="Code">编号字段名</param>
        /// <param name="ID">表ID</param>
        /// <returns>格式化后的附件名称</returns>
        public static string CodeValidation(string TableName,string Code,SQLHelper sh,string ID)
        {
            string sql = string.Format("Select {0} From {1} Where ID='{2}'", Code, TableName, ID);
            var obj = sh.ExecuteScalar(sql);
            if (obj == null)
            {
                return "";
            }
            else return obj.ToString();
        }

        

        /// <summary>
        /// 获取底层平台的流水号
        /// </summary>
        /// <param name="TmplCode">表单配置的code</param>
        /// <param name="applySerialNumber"></param>
        /// <returns></returns>
        public static string GetFormSerialNumberString(string TmplCode, bool applySerialNumber = true, DataRow row = null, Dictionary<string, string> dic = null)
        {
            string sql = "select * from S_UI_Form where Code='" + TmplCode + "'";
            SQLHelper BaseDb = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = BaseDb.ExecuteDataTable(sql);
            string SerialNumberSettings = dt.Rows[0]["SerialNumberSettings"].ToString();
            var serialNumberDic = JsonHelper.ToObject(SerialNumberSettings);
            string tmpl = serialNumberDic["Tmpl"].ToString();
            string resetRule = serialNumberDic["ResetRule"].ToString();
            string CategoryCode = "";
            string SubCategoryCode = "";
            string OrderNumCode = "";
            string PrjCode = "";
            string OrgCode = "";
            string UserCode = "";

            if (serialNumberDic.ContainsKey("CategoryCode"))
                CategoryCode = ReplaceString(serialNumberDic["CategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("SubCategoryCode"))
                SubCategoryCode = ReplaceString(serialNumberDic["SubCategoryCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrderNumCode"))
                OrderNumCode = ReplaceString(serialNumberDic["OrderNumCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("PrjCode"))
                PrjCode = ReplaceString(serialNumberDic["PrjCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("OrgCode"))
                OrgCode = ReplaceString(serialNumberDic["OrgCode"].ToString(), row, dic);
            if (serialNumberDic.ContainsKey("UserCode"))
                UserCode = ReplaceString(serialNumberDic["UserCode"].ToString(), row, dic);

            SerialNumberParam param = new SerialNumberParam()
            {
                Code = TmplCode,
                PrjCode = PrjCode,
                OrgCode = OrgCode,
                UserCode = UserCode,
                CategoryCode = CategoryCode,
                SubCategoryCode = SubCategoryCode,
                OrderNumCode = OrderNumCode
            };
            string number = SerialNumberHelper.GetSerialNumberString(tmpl, param, resetRule, applySerialNumber);
            return number;
        }

        public static string ReplaceString(string sql, DataRow row = null, Dictionary<string, string> dic = null, Dictionary<string, DataTable> dtDic = null)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var userService = FormulaHelper.GetService<IUserService>();

            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                if (dtDic != null && dtDic.Count > 0)
                {
                    var arr = value.Split('.');
                    if (arr.Length == 1)
                    {
                        if (dtDic.ContainsKey(value)) //默认值为整个表
                            return JsonHelper.ToJson(dtDic[value]);
                    }
                    else if (arr.Length == 2) //默认子编号名.字段名
                    {
                        if (dtDic.ContainsKey(arr[0]))
                        {
                            var dt = dtDic[arr[0]];
                            if (dt.Rows.Count > 0 && dt.Columns.Contains(arr[1]))
                            {
                                return dt.Rows[0][arr[1]].ToString();
                            }
                        }
                    }

                }
                if (row != null && row.Table.Columns.Contains(value))
                    return row[value].ToString();
                if (dic != null && dic.ContainsKey(value))
                    return dic[value];

                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
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
    }
}
