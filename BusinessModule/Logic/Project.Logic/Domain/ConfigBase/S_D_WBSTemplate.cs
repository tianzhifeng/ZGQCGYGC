using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_D_WBSTemplate
    {
        public void Save()
        {
            var rootNode = this.S_D_WBSTemplateNode.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
            if (rootNode == null)
            {
                rootNode = new S_D_WBSTemplateNode();
                rootNode.ID = FormulaHelper.CreateGuid();
                rootNode.ParentID = "";
                rootNode.FullID = rootNode.ID;
                rootNode.WBSType = WBSNodeType.Project.ToString();
                rootNode.TemplateID = this.ID;
                rootNode.S_D_WBSTemplate = this;
                this.S_D_WBSTemplateNode.Add(rootNode);
            }
            rootNode.Name = this.Name;
            rootNode.WBSValue = this.Name;
        }


        public List<S_D_WBSTemplateNode> GetMajorList()
        {
            var entites = FormulaHelper.GetEntities<BaseConfigEntities>();
            var majorType = WBSNodeType.Major.ToString();
            return entites.S_D_WBSTemplateNode.Where(d => d.TemplateID == this.ID && d.WBSType == majorType).OrderBy(d => d.FullID).ToList();
        }
    }
}
