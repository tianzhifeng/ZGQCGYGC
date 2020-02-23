using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Collections;
using System.ComponentModel.DataAnnotations;


namespace DocSystem.Logic
{
    public class HtmlGenerator
    {
        public static string GeneratorNodeForm(string spaceID, string nodeID)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未能找到指定的档案配置空间，无法构成HTML");
            var node = space.S_DOC_Node.FirstOrDefault(d => d.ID == nodeID);
            if(node==null)
                throw new Formula.Exceptions.BusinessException("未能找到指定的节点定义，无法构成HTML");
            var form = node.GetEditForm();
            return form.Render();
        }
    }
}
