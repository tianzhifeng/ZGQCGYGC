using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using EPC.Logic;
using Formula;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class BomConfigController : InfrastructureController<S_T_BomDefine>
    {
        public JsonResult GetDetailList(string DefineID)
        {
            var data= this.entities.Set<S_T_BomDefine_Detail>().Where(c => c.DefineID == DefineID).ToList();
            return Json(data);
        }

        public JsonResult AddNewDetail(string DefineID)
        {
            var define = this.GetEntityByID(DefineID);
            if (define == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的BOM配置内容");
            }
            var detail = this.entities.Set<S_T_BomDefine_Detail>().Create();
            detail.ID = FormulaHelper.CreateGuid();
            detail.Name = "新专业";
            detail.Code = "新专业";
            detail.SortIndex = define.S_T_BomDefine_Detail.Count + 1;
            detail.LinkUrl = "";
            detail.ConfigData = "";
            define.S_T_BomDefine_Detail.Add(detail);
            this.entities.SaveChanges();
            return Json(detail);
        }

        public JsonResult SaveDetailList()
        {
            this.UpdateList<S_T_BomDefine_Detail>();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DelDetail(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_T_BomDefine_Detail>(item.GetValue("ID"));
                if (detail == null) continue;
                this.entities.Set<S_T_BomDefine_Detail>().Remove(detail);
            }
            entities.SaveChanges();
            return Json("");
        }
    }
}
