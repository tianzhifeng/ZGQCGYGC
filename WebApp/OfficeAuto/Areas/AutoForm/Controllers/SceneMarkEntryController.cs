using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    /// <summary>
    /// 标识入库流程
    /// </summary>
    public class SceneMarkEntryController : OfficeAutoFormContorllor<T_SceneMark_Entry>
    {

        protected override void OnFlowEnd(T_SceneMark_Entry entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                //流程结束之后更新库存数量
                var lstDetail = BusinessEntities.Set<T_SceneMark_Entry_EntryDetail>().Where(x => x.T_SceneMark_EntryID == entity.ID);

                lstDetail.ToList().ForEach(x =>
                {

                    var dicModel = BusinessEntities.Set<T_SceneMark_Dictionaries>().FirstOrDefault(d => d.ID == x.DictID);
                    if (dicModel != null)
                        dicModel.StockQuantity = (Convert.ToInt32(dicModel.StockQuantity) + Convert.ToInt32(x.EntryQuantity)).ToString();

                });


                BusinessEntities.SaveChanges();

            }

            base.OnFlowEnd(entity, taskExec, routing);

        }


    }
}
