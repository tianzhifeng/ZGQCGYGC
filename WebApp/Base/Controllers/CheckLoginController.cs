using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Formula;
using System.Web.Security;
using Base.Logic.Domain;
using Config;

namespace Base.Controllers
{
    public class CheckLoginController : ApiController
    {
        [HttpPost]
        public ResultDTO Login([FromBody]UserDTO userDTO)
        {
            var rtn = new ResultDTO() { ResultType = 0 };
            try
            {
                if (string.IsNullOrEmpty(userDTO.Account))
                    throw new Exception("没有传入账户名！");
                string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", userDTO.Account.ToLower(), userDTO.Password), "SHA1");
                var sqlHepler = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var sql = "select * from S_A_User where Code='" + userDTO.Account + "'";
                var userDt = sqlHepler.ExecuteDataTable(sql);
                if (userDt==null||userDt.Rows.Count == 0)
                    throw new Exception("账户不存在！");
                var userPWD = string.Empty;
                if (userDt.Rows[0]["Password"] != DBNull.Value && userDt.Rows[0]["Password"] != null)
                    userPWD = userDt.Rows[0]["Password"].ToString();
                if ( userPWD!= pwd)
                    throw new Exception("密码错误！");
                userDTO.ID = userDt.Rows[0]["ID"].ToString();
                userDTO.Password = string.Empty;
                var token = "";
                rtn.Token = token;
                rtn.AppendData = FormulaHelper.ModelToDic<UserDTO>(userDTO);
            }
            catch (Exception e)
            {
                rtn.ResultType = 1;
                rtn.Message = e.Message;
            }
            return rtn;
        }



        public struct UserDTO
        {
            public string ID { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
        }

        public struct ResultDTO
        {
            public int ResultType { get; set; }
            public string Message { get; set; }

            public string Token { get; set; }

            public Dictionary<string, object> AppendData { get; set; }
        }
    }
}
