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
    public class QBSDefineController : BaseConfigController<S_T_QBSTemplate>
    {
        protected override void AfterGetMode(S_T_QBSTemplate entity, bool isNew)
        {
            if (isNew)
            {
                entity.ConnName = ConnEnum.Project.ToString();
                entity.NodeType = this.GetQueryString("NodeType");
            }
        }

        protected override void BeforeSave(S_T_QBSTemplate entity, bool isNew)
        {
            if (isNew)
            {
                if (String.IsNullOrEmpty(entity.ParentID))
                {
                    entity.FullID = entity.ID;
                }
                else
                {
                    var parent = this.GetEntityByID<S_T_QBSTemplate>(entity.ParentID);
                    if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到父节点信息");
                    else
                    {
                        if (parent.NodeType == QBSNodeType.QBS.ToString())
                            throw new Formula.Exceptions.BusinessException("质量节点下不能增加子节点");
                        else if (parent.NodeType == QBSNodeType.Catagory.ToString() &&
                           entity.NodeType == QBSNodeType.Catagory.ToString())
                            throw new Formula.Exceptions.BusinessException("分类节点下只能增加质量节点");
                        entity.FullID = parent.FullID + "." + entity.ID;
                    }
                }
            }
        }

        protected override void BeforeDelete(List<S_T_QBSTemplate> entityList)
        {
            foreach (var item in entityList)
            {
                this.entities.Set<S_T_QBSTemplate>().Delete(d => d.FullID.StartsWith(item.FullID));
            }
        }

    }
}
