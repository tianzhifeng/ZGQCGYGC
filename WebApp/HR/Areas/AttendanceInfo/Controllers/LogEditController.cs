using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;
namespace HR.Areas.AttendanceInfo.Controllers
{
    public class LogEditController : BaseController<S_W_AttendanceEditLog>
    {

        public override ActionResult Edit()
        {
            string sourceID = this.Request["SourceID"];

            var entity = GetEntity<S_W_AttendanceInfo>(sourceID);

            ViewBag.OrgName = entity.OrgName;
            ViewBag.UserName = entity.UserName;
            ViewBag.WorkDay = entity.WorkDay.ToString().Split(' ')[0];
            ViewBag.CheckDate = entity.CheckDate.ToString().Split(' ')[0];
            var enumService = FormulaHelper.GetService<IEnumService>();
            ViewBag.CheckType = enumService.GetEnumText("HR.CheckType", entity.CheckType);

            return View();
        }

        protected override void EntityCreateLogic<TEntity>(TEntity entity)
        {
            base.EntityCreateLogic<TEntity>(entity);

            string sourceID = this.Request["SourceID"];

            if (!string.IsNullOrEmpty(sourceID))
            {
                var entityInfo = GetEntity<S_W_AttendanceInfo>(sourceID);
            
                if (entity is S_W_AttendanceEditLog)
                {
                    S_W_AttendanceEditLog obj = entity as S_W_AttendanceEditLog;
                    obj.SourceID = sourceID;
                    obj.CheckDate = entityInfo.LastModifyCheckDate == null ? entityInfo.CheckDate : entityInfo.LastModifyCheckDate;
                    obj.WorkDay = entityInfo.WorkDay;
                }
            }
        }

        public override JsonResult Save()
        {
            S_W_AttendanceEditLog model = UpdateEntity<S_W_AttendanceEditLog>();

            string sourceID = model.SourceID;
            var entity = GetEntity<S_W_AttendanceInfo>(sourceID);
            //var date = Convert.ToDateTime(entity.CheckDate).ToString("yyyy-MM-dd");
            //var time = Convert.ToDateTime(model.CheckDate).ToString("HH:mm:ss");

            //model.CheckDate = Convert.ToDateTime(date + " " + time);

            entities.Set<S_W_AttendanceInfo>().Where(s => s.ID == model.SourceID).Update(s => s.LastModifyCheckDate = model.CheckDate);
            entities.Set<S_W_AttendanceInfo>().Where(s => s.ID == model.SourceID).Update(s => s.WorkDay = model.WorkDay);
            entities.SaveChanges();

            return base.Save();

        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sourceID = this.Request["SourceID"];

            if (string.IsNullOrEmpty(sourceID))
            {
                return base.GetList(qb);
            }
            else
            {
                var result = entities.Set<V_S_W_AttendanceEditLog>().Where(c => c.SourceID == sourceID).WhereToGridData(qb);

                return Json(result);
            }
        }
    }
}
