using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_D_EPSDef
    {
        List<S_D_EPSDef> _Ancestors;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_EPSDef> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    var entities = this.GetDbContext<BaseConfigEntities>();
                    _Ancestors = entities.S_D_EPSDef.Where(d => this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _Ancestors;
            }
        }

        List<S_D_EPSDef> _Children;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_EPSDef> Children
        {
            get
            {
                if (_Children == null)
                {
                    var entities = this.GetDbContext<BaseConfigEntities>();
                    _Children = entities.S_D_EPSDef.Where(d => d.ParentID == this.ID).ToList();
                }
                return _Children;
            }
        }

        List<S_D_EPSDef> _AllChildren;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_EPSDef> Allhildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    var entities = this.GetDbContext<BaseConfigEntities>();
                    _AllChildren = entities.S_D_EPSDef.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }

        List<S_D_EPSDef> _Brothers;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_EPSDef> Brothers
        {
            get
            {
                if (_Brothers == null)
                {
                    var entities = this.GetDbContext<BaseConfigEntities>();
                    _Brothers = entities.S_D_EPSDef.Where(d => d.ParentID == this.ParentID && d.ID != this.ID).ToList();
                }
                return _Brothers;
            }
        }

        S_D_EPSDef _Parent;
        [NotMapped]
        [JsonIgnore]
        public S_D_EPSDef Parent
        {
            get
            {

                if (_Parent == null)
                {
                    var entities = this.GetDbContext<BaseConfigEntities>();
                    _Parent = entities.S_D_EPSDef.Find(this.ParentID);
                }
                return _Parent;
            }
        }



        public void AddChild(S_D_EPSDef child)
        {
            if (this.Children.Count > 0) throw new Formula.Exceptions.BusinessException("EPS结构只能有一个子节点");
            if (this.Type == EngineeringSpaceType.UnderEngineering.ToString()
                || this.Type == EngineeringSpaceType.Engineering.ToString())
            {
                child.Type = EngineeringSpaceType.UnderEngineering.ToString();
                child.GroupTable = "S_I_ProjectInfo";
            }
            else
            {
                child.Type = EngineeringSpaceType.UpEngineering.ToString();
                child.GroupTable = "S_I_ProjectGroup";
            }
            var entities = this.GetDbContext<BaseConfigEntities>();
            if (String.IsNullOrEmpty(child.ID))
                child.ID = Formula.FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            entities.S_D_EPSDef.Add(child);
        }

        public void Delete()
        {
            var entities = this.GetDbContext<BaseConfigEntities>();
            if (this.Type == EngineeringSpaceType.Engineering.ToString())
                throw new Formula.Exceptions.BusinessException("工程节点不允许删除");

            if (this.Children.Count > 0) {
                var child = this.Children.FirstOrDefault();
                var fullID = child.FullID;
                if (this.Parent == null)
                {
                    child.ParentID = "";
                    child.FullID = child.ID;
                }
                else
                {
                    child.ParentID = this.Parent.ID;
                    child.FullID = this.Parent.FullID + "." + child.ID;
                }
                foreach (var item in child.Allhildren)
                {
                    item.FullID = item.FullID.Replace(fullID, child.FullID);
                }
            }
            entities.S_D_EPSDef.Remove(this);
        }

        public void UpGrade()
        {
            if (this.Parent == null) throw new Formula.Exceptions.BusinessException("顶层节点不能进行升级操作");
            if (this.Type == EngineeringSpaceType.Engineering.ToString())
            {
                this.Parent.Type = EngineeringSpaceType.UnderEngineering.ToString();
                this.Parent.GroupTable = "S_I_ProjectInfo";
                this.Parent.GroupField = "";
                foreach (var item in   this.Parent.Brothers)
                {
                    item.Type = EngineeringSpaceType.UnderEngineering.ToString();
                    item.GroupTable = "S_I_ProjectInfo";
                    item.GroupField = "";
                }              
            }
            else if (this.Parent.Type == EngineeringSpaceType.Engineering.ToString())
            {
                this.GroupField = "";
                this.GroupTable = "S_I_ProjectGroup";
                this.Type = EngineeringSpaceType.UpEngineering.ToString();
            }
            var parent = this.Parent;
            var allChildren = this.Allhildren;
            var fulID = this.FullID;
            if (this.Parent.Parent == null) //父节点已经是顶层节点
            {
                this.ParentID = "";
                this.FullID = this.ID;
            
            }
            else
            {
                this.ParentID = parent.ParentID;
                this.FullID = parent.Parent.FullID + "."+this.ID;
            }
            parent.ParentID = this.ID;
            parent.FullID = this.FullID + "." + parent.ID;
            foreach (var child in Allhildren)
            {
                child.FullID = child.FullID.Replace(fulID, parent.FullID);
                child.ParentID = parent.ID;
            }
        }

        public void Down()
        {
            if (this.Children == null || this.Children.Count == 0)
            {
                throw new Formula.Exceptions.BusinessException("底层节点不能进行降级操作");
            }
            if (this.Type == EngineeringSpaceType.Engineering.ToString())
            {

            }
            else if (this.Children.Exists(d => d.Type == EngineeringSpaceType.Engineering.ToString()))
            { }
        }
    }

    public enum EngineeringSpaceType
    {
        UpEngineering,
        Engineering,
        UnderEngineering,
        Project
    }
}
