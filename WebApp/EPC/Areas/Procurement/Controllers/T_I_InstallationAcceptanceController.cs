using Config;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Procurement.Controllers
{
    public class T_I_InstallationAcceptanceController : EPCController<T_I_InstallationAcceptance>
    {
        public override JsonResult GetModel(string id)
        {
            var entity = this.GetEntityByID(id);
            string projectInfoID = this.Request["EngineeringInfoID"];
            if (entity != null)
            {
                return Json(entity);
            }
            else
            {

                var projectInfo = this.GetEntityByID<S_I_Engineering>(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("");
                T_I_InstallationAcceptance entiy = new T_I_InstallationAcceptance();
                entiy.XMMC = projectInfo.Name;
                //entiy.BGSQR = FormulaHelper.GetUserInfo().UserID;
                //entiy.BGSQRName = FormulaHelper.GetUserInfo().UserName;
                //entiy.BGRQ = DateTime.Now;
                entiy.EngineeringInfoID = projectInfoID;
                return Json(entiy);
            }
        }

        protected override void AfterSave(T_I_InstallationAcceptance entity, bool isNew)
        {
            string projectInfoID = this.Request["EngineeringInfoID"];
            string sql = string.Format(@"update  S_I_Engineering   set State='Completeinstallation' where ID='{0}'", projectInfoID);
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Engineering).ExecuteDataTable(sql);
        }


        public ContentResult UpdateWeekWork(string IDs, string Index, string EngineeringInfoID)
        {
            string[] ids = IDs.Split(':');
            string idx = "TN";
            if (Index == "C") { idx = "CN"; }
            try
            {
                var entity = FormulaHelper.GetEntities<EPCEntities>();
                var ProjectInfo = entity.Set<S_I_Engineering>().Find(EngineeringInfoID);
                string Cont = "";
                if (ids.Length > 0)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        string id = ids[i];
                        var WeekWord = entity.Set<T_I_InstallationAcceptance>().FirstOrDefault(c => c.ID ==id);
                        Cont += "项目：" + WeekWord.XMMC  + " 的安装验收单提交了请前往查看！";
                        if (Index == "T") { WeekWord.Type = "已提交";
                            var url = "/MvcConfig/UI/Form/PageView?TmplCode=InstallationAcceptance&FuncType=View&ID=" + id;
                            IMessageService ms = FormulaHelper.GetService<IMessageService>();
                            ms.SendMsg("安装验收单提交", Cont, url, "", ProjectInfo.ChargerUser, ProjectInfo.ChargerUserName, Config.MsgReceiverType.UserType, Config.MsgType.Normal, false, false);
                        }
                        else { WeekWord.Type = "未提交"; }
                    }
                }
               
                if (Index == "T") { idx = "T"; }
                else { idx = "C"; }
                entity.SaveChanges();
            }
            catch (Formula.Exceptions.BusinessValidationException e)
            {
            }
            return Content(idx);

        }
        

    }
}
