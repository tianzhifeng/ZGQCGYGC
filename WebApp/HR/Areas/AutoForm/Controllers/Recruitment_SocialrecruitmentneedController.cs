using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using HR.Logic.Domain;
using Base.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;
using MvcAdapter;


namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_SocialrecruitmentneedController : HRFormContorllor<T_Recruitment_Schoolrecruitmentneedsum>
    {
        // GET: /AutoForm/Recruitment_Socialrecruitmentneed/

        public JsonResult GetSocialrecruitmentneedsumList(QueryBuilder qb)
        {
            var year = DateTime.Now.Year;

            var data = BusinessEntities.Set<S_E_Recruitment_DeptRequirementReport_SZRCXQJH>().Where(p => p.Year == year).OrderBy(p => p.Dept);
                
            return Json(data);
        }





    }
}
