using Formula;
using Formula.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_D_WBSTemplateNode
    {

        /// <summary>
        /// 是否为枚举型节点
        /// </summary>
        [NotMapped]
        public bool IsEnumNode
        {
            get
            {
                if (this.GetDbContext<BaseConfigEntities>().S_D_WBSAttrDefine.Count(d => d.Type == this.WBSType) > 0)
                    return true;
                else
                    return false;
            }
        }

        List<S_T_ProjectMode> _modes;
        [NotMapped]
        [JsonIgnore]
        public List<S_T_ProjectMode> Modes
        {
            get
            {
                if (_modes == null)
                    _modes = this.GetDbContext<BaseConfigEntities>().Set<S_T_ProjectMode>().
                        Where(d => this.S_D_WBSTemplate.ModeCodes.Contains(d.ModeCode)).ToList();
                return _modes;
            }
        }

        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_WBSStructInfo StructNodeInfo
        {
            get
            {
                return this.Modes.FirstOrDefault().S_T_WBSStructInfo.FirstOrDefault(d => d.Code == this.WBSType);
            }
        }

        S_D_WBSTemplateNode _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_D_WBSTemplateNode Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_D_WBSTemplate.S_D_WBSTemplateNode.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        List<S_D_WBSTemplateNode> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_WBSTemplateNode> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_D_WBSTemplate.S_D_WBSTemplateNode.Where(d => d.ParentID == this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _children;
            }
        }


        public S_D_WBSTemplateNode AddChild(S_D_WBSTemplateNode child, bool isValidate = true)
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            if (String.IsNullOrEmpty(child.WBSType))
                throw new Formula.Exceptions.BusinessException("必须指定节点的类型");
            if (entities.Entry(child).State != System.Data.EntityState.Detached && entities.Entry(child).State != System.Data.EntityState.Added)
                throw new Formula.Exceptions.BusinessException("非新增状态的对象，无法调用AddChild方法");
            child.TemplateID = this.TemplateID;
            child.S_D_WBSTemplate = this.S_D_WBSTemplate;
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.Level = this.Level + 1;
            if (child.SortIndex <= 0)
                child.SortIndex = child.Level * 1000 + this.Children.Count * 10;
            if (String.IsNullOrEmpty(child.WBSValue) && child.IsEnumNode)
                throw new Formula.Exceptions.BusinessException("枚举类WBS节点，必须指定WBSValue");
            if (String.IsNullOrEmpty(child.WBSValue))
                child.WBSValue = child.Code;
            if (String.IsNullOrEmpty(child.WBSValue))
                child.WBSValue = child.Name;

            if (isValidate)
            {
                if (this.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("【" + this.Name + "】未获取WBS结构定义对象，无法新增子节点");
                if (!this.StructNodeInfo.ValidateChildren(child.WBSType))
                    throw new Formula.Exceptions.BusinessException("【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), this.WBSType)
                        + "】节点下不包含【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), child.WBSType) + "】的子节点定义，无法新增子节点");
                if (child.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("模板定义中未能找到【" + child.WBSType + "】定义，无法增加子节点");
            }

            this.Children.Add(child);
            this.S_D_WBSTemplate.S_D_WBSTemplateNode.Add(child);
            return child;
        }

        public void Delete()
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            entities.Set<S_D_WBSTemplateNode>().Delete(d => d.FullID.StartsWith(this.FullID));
        }
    }
}
