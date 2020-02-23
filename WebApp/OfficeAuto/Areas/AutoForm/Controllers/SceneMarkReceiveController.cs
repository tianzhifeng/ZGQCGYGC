using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class SceneMarkReceiveController : OfficeAutoFormContorllor<T_SceneMark_Receive>
    {

        /// <summary>
        /// 标识申领流程
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="taskExec"></param>
        /// <param name="routing"></param>
        protected override void OnFlowEnd(T_SceneMark_Receive entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                //流程结束之后更新库存数量
                var lstDetail = BusinessEntities.Set<T_SceneMark_Receive_ReceiveDetail>().Where(x => x.T_SceneMark_ReceiveID == entity.ID);

                lstDetail.ToList().ForEach(x =>
                {
                    var dicModel = BusinessEntities.Set<T_SceneMark_Dictionaries>().FirstOrDefault(d => d.ID == x.DictID);
                    if (dicModel != null)
                        dicModel.StockQuantity = (Convert.ToInt32(dicModel.StockQuantity) - Convert.ToInt32(x.ReceiveQuantity)).ToString();
                });
                BusinessEntities.SaveChanges();
            }

            base.OnFlowEnd(entity, taskExec, routing);

        }




    }

}
