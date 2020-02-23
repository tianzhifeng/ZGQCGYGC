using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Base.Logic.Domain;
using System.Web.Security;
using Base.Logic.BusinessFacade;
using Formula.Exceptions;
using System.Text.RegularExpressions;
using EPC.Areas.Cooperation.Models;
using System.Data;

namespace EPC.Areas.Manage.Controllers
{
    public class ParticipantUserController : EPCFormContorllor<S_A_ParticipantUser>
    {
        
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            string sysName = dic.GetValue("SysName");
            if (string.IsNullOrEmpty(sysName))
            {
                throw new BusinessException("账号不能为空！");
            }

            string sql = string.Format("select Count(ID) from S_A_ParticipantUser where SysName='{0}' and ID !='{1}' ", sysName, dic.GetValue("ID"));
            var sysNameCount = Convert.ToInt32(EPCSQLDB.ExecuteScalar(sql));
            if (sysNameCount > 0)
            {
                throw new BusinessException(string.Format("用户名【{0}】已存在", sysName));
            }

            sql = string.Format("select * from S_A_ParticipantUser where Participant='{0}' and  IsAdmin='1' ", dic.GetValue("Participant"));
            var dtParticipant = EPCSQLDB.ExecuteDataTable(sql);

            if (isNew)
            {
                if (dtParticipant.Rows.Count > 0)
                {
                    throw new BusinessException(string.Format("参建方【{0}】已存在管理员账号【{1}】", dic.GetValue("ParticipantName"), sysName ));
                }

                var password = dic.GetValue("Password");
                ValidatePwd(password);
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", sysName.ToLower(), password), "SHA1");
                dic.SetValue("Password", password);

            }
            else
            {
                if (dtParticipant.Rows.Count == 1)
                {
                    if (dic.GetValue("ID") != dtParticipant.Rows[0]["ID"].ToString())
                    {
                        throw new BusinessException(string.Format("参建方【{0}】只能有一个管理员账号！", dic.GetValue("ParticipantName")));
                    }
                }
                else if(dtParticipant.Rows.Count > 1)
                {
                    throw new BusinessException(string.Format("参建方【{0}】只能有一个管理员账号！", dic.GetValue("ParticipantName")));
                }

                dic.Remove("Password");
            }

            dic.SetValue("IsAdmin", "true");

        }

        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                LoggerHelper.InserLogger("S_A_ParticipantUser", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), string.Empty, dic.GetValue("UserName"), "新增账号密码");
            }
            else
            {
                LoggerHelper.InserLogger("S_A_ParticipantUser", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), string.Empty, dic.GetValue("UserName"), "修改账号信息");
            }
        }

        public JsonResult Reset()
        {
            var password = Request["pwd"];
            if (string.IsNullOrEmpty(password))
            {
                throw new BusinessException("请输入密码！");
            }
            ValidatePwd(password);

            var listData = JsonHelper.ToList(Request["ListData"]);
            if (listData.Count == 0)
            {
                throw new BusinessException("请选择要操作的行！");
            }
            var idList = listData.Select(a => a.GetValue("ParticipantUserID"));

            string sql = string.Format("select * from S_A_ParticipantUser where ID in ('{0}') and  IsAdmin='1' ", string.Join("','", idList));
            var dt = EPCSQLDB.ExecuteDataTable(sql);
            string updateSql = "";
            Dictionary<string, object> operaLog = new Dictionary<string, object>();
            var userInfo = FormulaHelper.GetUserInfo();
            foreach (DataRow dr in dt.Rows)
            {
                string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", dr["SysName"].ToString().ToLower(), password), "SHA1");
                if (pwd == dr["Password"].ToString())
                {
                    throw new BusinessException(string.Format("账号【{0}】的新密码与旧密码相同，请修改！", dr["SysName"].ToString()));
                }
                updateSql += string.Format("update S_A_ParticipantUser set Password = '{0}' where ID = '{1}' and  IsAdmin='1' ", pwd, dr["ID"]);

                operaLog.SetValue("OperateType", EnumOperaType.Update);
                operaLog.SetValue("TableIDs", dr["ID"].ToString());
                operaLog.SetValue("Name", dr["UserName"].ToString());
                operaLog.SetValue("EngineeringInfoID", string.Empty);
                operaLog.SetValue("DBTable", "S_A_ParticipantUser");
                operaLog.SetValue("DBTableName", "S_A_ParticipantUser");
                operaLog.SetValue("Remark", "重置密码");
                operaLog.SetValue("CreateUser", userInfo.UserName);
                operaLog.SetValue("CreateUserID", userInfo.UserID);
                operaLog.SetValue("CreateDate", DateTime.Now);
                updateSql += operaLog.CreateInsertSql(EPCSQLDB, "S_A_ParticipantOperaLog", FormulaHelper.CreateGuid());
            }
            EPCSQLDB.ExecuteNonQuery(updateSql);
            return Json("");
        }

        /// <summary>
        /// 验证密码是否符合规则，若不符规则，则弹窗提示。
        /// </summary>
        /// <param name="pwd">密码</param>
        private void ValidatePwd(string pwd)
        {
            string pwdPattern = @"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z]).{8,16}$";
            if (string.IsNullOrWhiteSpace(pwd) || !Regex.IsMatch(pwd, pwdPattern))
            {
                throw new BusinessException("密码必须是8-16位大、小写英文字母和数字的组合！");
            }
        }

    }
}
