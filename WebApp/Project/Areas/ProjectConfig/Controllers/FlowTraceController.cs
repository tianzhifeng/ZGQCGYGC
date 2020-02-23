using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class FlowTraceController : BaseConfigController<S_T_FlowTraceDefine>
    {
        protected override void AfterGetMode(S_T_FlowTraceDefine entity, bool isNew)
        {
            if (isNew) {
                entity.ConnName = ConnEnum.Project.ToString();
                entity.Type = FlowTraceDefineNodeType.Catagory.ToString();
            }
        }

        protected override void BeforeSave(S_T_FlowTraceDefine entity, bool isNew)
        {
            if (this.entities.Set<S_T_FlowTraceDefine>().FirstOrDefault(a => a.SortIndex == entity.SortIndex 
                && a.ID != entity.ID && a.ModeID == entity.ModeID&&a.Type==entity.Type) != null)
                throw new Formula.Exceptions.BusinessException("相同模式下流程跟踪定义的排序不能重复");
            if (isNew)
            {
                if (String.IsNullOrEmpty(entity.ParentID))
                {
                    entity.FullID = entity.ID;
                }
                else
                {
                    var parent = this.GetEntityByID<S_T_FlowTraceDefine>(entity.ParentID);
                    if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到父节点信息");
                    else
                    {
                        if (parent.Type == FlowTraceDefineNodeType.FlowNode.ToString())
                            throw new Formula.Exceptions.BusinessException("流程节点下不能增加子节点");
                        entity.FullID = parent.FullID + "."+entity.ID;
                    }
                }
                entity.ConnName = ConnEnum.Project.ToString();
            }
        }

        protected override void BeforeDelete(List<S_T_FlowTraceDefine> entityList)
        {
            foreach (var item in entityList)
            {
                this.entities.Set<S_T_FlowTraceDefine>().Delete(d => d.FullID.StartsWith(item.FullID));
            }
        }

    }
}
