using Config;
using Config.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPC.Logic.Domain
{
    public partial class S_T_DefineParams
    {
        static Dictionary<string, object> _params;
        public static Dictionary<string, object> Params
        {
            get
            {
                if (_params == null)
                {
                    _params = new Dictionary<string, object>();
                    string sql = "select * from S_T_DefineParams with(nolock) ";
                    var db = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
                    var dt = db.ExecuteDataTable(sql);
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        _params.SetValue(row["Code"].ToString(), row["Value"]);
                    }
                }
                return _params;
            }
        }

        public static void Reset()
        {
            _params = null;
        }
    }
}
