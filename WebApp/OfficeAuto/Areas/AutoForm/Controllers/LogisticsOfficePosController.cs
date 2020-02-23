using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;


namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class LogisticsOfficePosController : OfficeAutoFormContorllor<S_E_Logistics_OfficePos>
    {
        // GET: /AutoForm/LogisticsOfficePos/


        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var parentId = dic["ParentID"].ToString();

            if (string.IsNullOrEmpty(parentId))
            {
                throw new Formula.Exceptions.BusinessException("未能找到该父节点");
            }

            //var code = dic["Code"].ToString();

            //var flag = MarketEntites.Set<S_E_SubjectMatter>().Where(p => p.Code == code && p.ID != entity.ID).Any();
            //if (flag)
            //{
            //    throw new Formula.Exceptions.BusinessException("供应商编码已存在，无法添加");
            //}

            var parent = BusinessEntities.Set<S_E_Logistics_OfficePos>().FirstOrDefault(p => p.ID == parentId);

            dic["FullID"] = parent.FullID + "." + dic["ID"].ToString();

            dic["Level"] = (Convert.ToInt32(parent.Level) + 1) + "";
        }


    }
}
