using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Project.Logic.Domain;
using Formula.Helper;

namespace Project.Areas.AutoUI.Controllers
{
    public class PlotSealGroupController : ProjectFormContorllor<S_EP_PlotSealGroup>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var GroupInfo = dic.GetValue("GroupInfo");
            var list = JsonHelper.ToList(GroupInfo);
            var mainSeals = list.Where(a => a.GetValue("IsMain") == "true");
            if (mainSeals.Count() == 0) throw new Formula.Exceptions.BusinessException("请选择主章");
            if (mainSeals.Count() > 1) throw new Formula.Exceptions.BusinessException("只能有一个图章作为主章");
            var mainSeal = mainSeals.First();
            var followSeals = list.Where(a => a.GetValue("IsMain") != "true");
            var str = "主章：" + mainSeal.GetValue("Name");
            if (followSeals.Count() > 0)
                str += "，从章：" + string.Join("、", followSeals.Select(a => a.GetValue("Name")));
            dic.SetValue("GroupDescription", str);
            if (followSeals.Count() == 0)
                dic.SetValue("MainPlotSealID", mainSeal.GetValue("PlotSeal"));
            else
                dic.SetValue("MainPlotSealID", "");
        }

        protected override void BeforeDelete(string[] Ids)
        {
            var sql = "select 1 from S_E_Product where 1=1 and ({0})";
            var whereList = new List<string>();
            foreach (var id in Ids)
                whereList.Add(" PlotSealGroup like '%" + id + "%'");
            if (whereList.Count() > 0)
            {
                sql = string.Format(sql, string.Join(" or ", whereList));
                var data = this.ProjectSQLDB.ExecuteDataTable(sql);
                if (data.Rows.Count > 0) throw new Formula.Exceptions.BusinessException("选中的出图章组合被使用在成果中，请重新选择");
            }
        }
    }
}