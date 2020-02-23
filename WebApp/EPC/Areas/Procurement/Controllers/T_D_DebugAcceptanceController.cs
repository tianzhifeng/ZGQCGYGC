using Config;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;

namespace EPC.Areas.Procurement.Controllers
{
    public class T_D_DebugAcceptanceController : EPCFormContorllor<T_D_DebugAcceptance>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var SFQBWC = dic.GetValue("SFQBWC").ToString();
            var EngineeringInfoID = dic.GetValue("EngineeringInfoID").ToString();
            if (SFQBWC == "是")
            {
                var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
                engineeringInfo.State = "Completeinspection";
                this.EPCEntites.SaveChanges();
            }
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
                        var WeekWord = entity.Set<T_D_DebugWeekly>().FirstOrDefault(c => c.ID == id);
                        Cont += "项目：" + WeekWord.ProjectName + " 的调试周报提交了请前往查看！";
                        if (Index == "T") { WeekWord.TJZT = "已提交";
                            var url = "/MvcConfig/UI/Form/PageView?TmplCode=DebugWeekly&FuncType=View&ID=" + id;
                            IMessageService ms = FormulaHelper.GetService<IMessageService>();
                            ms.SendMsg("周报提交", Cont, url, "", ProjectInfo.ChargerUser, ProjectInfo.ChargerUserName, Config.MsgReceiverType.UserType, Config.MsgType.Normal, false, false);
                        }
                        else { WeekWord.TJZT = "未提交"; }
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
