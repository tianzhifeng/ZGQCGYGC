using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_QueryParam
    {
        public IControl GetCtontrol()
        {
            IControl control;
            if (this.S_DOC_ListConfig.Type == ListConfigType.Node.ToString())
            {
                var space = DocConfigHelper.CreateConfigSpaceByID(this.S_DOC_ListConfig.SpaceID);
                var node = space.S_DOC_Node.FirstOrDefault(d => d.ID == this.S_DOC_ListConfig.RelationID);
                if (node == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + this.S_DOC_ListConfig.RelationID + "】节点定义 ");
                var attr = node.S_DOC_NodeAttr.FirstOrDefault(d => d.AttrField == this.InnerField);
                if (attr == null)
                    throw new Formula.Exceptions.BusinessException("未能找到【" + this.InnerField + "】属性定义 ");
                control = ControlGenrator.GenrateMiniControl(attr.InputType);

                if (attr.IsEnum == TrueOrFalse.True.ToString() && !String.IsNullOrEmpty(attr.EnumKey))
                {
                    string enumKey = attr.EnumKey;
                    if (enumKey.Split('.').Length > 1)
                        enumKey = enumKey.Split('.')[1];
                    control.SetAttribute("data", enumKey);
                }    
            }
            else
            {
                var space = DocConfigHelper.CreateConfigSpaceByID(this.S_DOC_ListConfig.SpaceID);
                var file = space.S_DOC_File.FirstOrDefault(d => d.ID == this.S_DOC_ListConfig.RelationID);
                if (file == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + this.S_DOC_ListConfig.RelationID + "】节点定义 ");
                var attr = file.S_DOC_FileAttr.FirstOrDefault(d => d.FileAttrField == this.InnerField);
                if (attr == null)
                    throw new Formula.Exceptions.BusinessException("未能找到【" + this.InnerField + "】属性定义 ");
                control = ControlGenrator.GenrateMiniControl(attr.InputType);
                if (attr.IsEnum == TrueOrFalse.True.ToString() && !String.IsNullOrEmpty(attr.EnumKey))
                {
                    string enumKey = attr.EnumKey;
                    if (enumKey.Split('.').Length > 1)
                        enumKey = enumKey.Split('.')[1];
                    control.SetAttribute("data", enumKey);
                }    
            }
            if (String.IsNullOrEmpty(this.QueryType))
                control.Name = "$LK$" + this.InnerField;
            else
                control.Name = "$" + this.QueryType + "$" + this.InnerField;
            control.Style = " width:90% ";
            return control;
        }
    }
}
