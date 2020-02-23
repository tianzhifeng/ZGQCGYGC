using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Base.Logic.Domain;
using Config;
using Config.Logic;
using Formula;
using System.Reflection;
using Formula.DynConditionObject;
using Base.Logic;
using Formula.Helper;

namespace Base.Areas.ShortMsg.Controllers
{
    public class AlarmConfigController : BaseController<S_S_AlarmConfig>
    {
        public override JsonResult GetModel(string id)
        {
            var entity = this.GetEntity<S_S_AlarmConfig>(id);
            bool isNew = false;
            if (entities.Entry<S_S_AlarmConfig>(entity).State == System.Data.EntityState.Added
                || entities.Entry<S_S_AlarmConfig>(entity).State == System.Data.EntityState.Detached)
                isNew = true;
            if (isNew)
            {
                entity.AlarmMode = AlarmMode.Alarm.ToString();
                entity.Frequency = AlarmFrequency.EveryWeek.ToString(); //Convert.ToInt32(AlarmFrequency.EveryWeek);
            }
            return Json(entity);
        }

        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_S_AlarmConfig>();

            if (entities.Entry<S_S_AlarmConfig>(entity).State == System.Data.EntityState.Added
            || entities.Entry<S_S_AlarmConfig>(entity).State == System.Data.EntityState.Detached)
                entity.State = "Normal";

            var db = SQLHelper.CreateSqlHelper(entity.Connection);
            string sql = "select count(0) from syscolumns where id=object_id('{0}') and name='{1}'";
            var obj = db.ExecuteScalar(String.Format(sql, entity.TableName, entity.PlanDateTimeField));
            if (Convert.ToInt32(obj) == 0) throw new Exception("计划完成日期字段不存在");
            if (!String.IsNullOrEmpty(entity.ProjectIDField))
            {
                obj = db.ExecuteScalar(String.Format(sql, entity.TableName, entity.ProjectIDField));
                if (Convert.ToInt32(obj) == 0) throw new Exception("项目ID字段不存在");
            }
            if (!String.IsNullOrEmpty(entity.FinishDateTimeField))
            {
                obj = db.ExecuteScalar(String.Format(sql, entity.TableName, entity.FinishDateTimeField));
                if (Convert.ToInt32(obj) == 0) throw new Exception("实际完成日期字段不存在");
            }
            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Valid(string List)
        {
            var list = JsonHelper.ToList(List);
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                var entity = this.entities.Set<S_S_AlarmConfig>().FirstOrDefault(d => d.ID == id);
                if (entity == null) continue;
                entity.State = "Valid";
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Enable(string List)
        {
            var list = JsonHelper.ToList(List);
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                var entity = this.entities.Set<S_S_AlarmConfig>().FirstOrDefault(d => d.ID == id);
                if (entity == null) continue;
                entity.State = "Normal";
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
