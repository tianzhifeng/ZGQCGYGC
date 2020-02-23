using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using Project.Logic;
using System.Data;
using Formula.Helper;

namespace Project.Areas.BaseData.Controllers
{
    public class CBSDefineController : BaseConfigController<S_C_CBSDefine>
    {
        public override JsonResult GetTree()
        {
            var dt = EnumBaseHelper.GetEnumTable(typeof(CBSType));
            var result = new DataTable();
            result.Columns.Add("Name");
            result.Columns.Add("ParentID");
            result.Columns.Add("Type");
            result.Columns.Add("ID");
            var root = result.NewRow();
            root["Name"] = "费用科目维护";
            root["ID"] = "Root";
            root["Type"] = "Root";
            root["ParentID"] = "";
            result.Rows.Add(root);
            foreach (DataRow row in dt.Rows)
            {
                var child = result.NewRow();
                child["Name"] = row["text"];
                child["ID"] = row["value"];
                child["Type"] = "Child";
                child["ParentID"] = "Root";
                result.Rows.Add(child);
            }
            return Json(result);
        }

        protected override void BeforeSave(S_C_CBSDefine entity, bool isNew)
        {
            if (isNew)
                entity.Save();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var cbsType = this.Request["CBSType"];
            if (!string.IsNullOrEmpty(cbsType))
                qb.Add("CBSType", Formula.QueryMethod.Equal, cbsType);
            return base.GetList(qb);
        }
    }
}
