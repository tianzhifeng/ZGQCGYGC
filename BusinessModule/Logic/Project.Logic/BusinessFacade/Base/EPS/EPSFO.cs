using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Formula;
using Config;
using Config.Logic;
using Project.Logic;
using Project.Logic.Domain;


namespace Project.Logic
{
    public partial class EPSFO
    {
        public void BuildEngineering(S_I_ProjectGroup group)
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var baseConfigEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var epsDefines = baseConfigEntities.Set<S_D_EPSDef>().OrderBy(d => d.FullID).ToList();
            var list = new List<S_I_ProjectGroup>();
            foreach (var item in epsDefines)
            {
                if (item.Type == EngineeringSpaceType.UpEngineering.ToString())
                {
                    #region 创建工程以上的EPS结构
                    var propertyInfoValue = group.GetPropertyString(item.GroupField);
                    if (String.IsNullOrEmpty(propertyInfoValue)) throw new Formula.Exceptions.BusinessException("");
                    var eps = projectEntities.S_I_ProjectGroup.FirstOrDefault(d => d.DefineID == item.ID
                        && d.Name == propertyInfoValue);
                    if (eps == null)
                    {
                        eps = projectEntities.S_I_ProjectGroup.Create();
                        eps.Type = EngineeringSpaceType.UpEngineering.ToString();                    
                        eps.ID = FormulaHelper.CreateGuid();
                        eps.Name = propertyInfoValue;
                        if (String.IsNullOrEmpty(item.ParentID))
                        {
                            eps.ParentID = "";
                            eps.FullID = eps.ID;
                            eps.RootID = eps.ID;
                            projectEntities.S_I_ProjectGroup.Add(eps);
                        }
                        else
                        {
                            var parentValue = group.GetPropertyString(item.Parent.GroupField);
                            var parentEps = list.FirstOrDefault(d => d.DefineID == item.ParentID && d.Name == parentValue);
                            if (parentEps != null)
                            {
                                parentEps.AddChild(eps);
                            }
                        }
                        eps.DefineID = item.ID;
                    }
                    list.Add(eps);
                    #endregion
                }
                else if (item.Type == EngineeringSpaceType.Engineering.ToString())
                {
                    group.DefineID = item.ID;
                    group.Type = EngineeringSpaceType.Engineering.ToString();
                    if (String.IsNullOrEmpty(item.ParentID))
                    {
                        if (String.IsNullOrEmpty(group.ID)) group.ID = FormulaHelper.CreateGuid();
                        group.FullID = group.ID;
                        group.ParentID = "";
                        group.RootID = group.ID;
                        projectEntities.S_I_ProjectGroup.Add(group);
                    }
                    else
                    {
                        var parentValue = group.GetPropertyString(item.Parent.GroupField);
                        var parent = list.FirstOrDefault(d => d.Name == parentValue && d.DefineID == item.ParentID);
                        parent.AddChild(group);
                    }
                }
            }
        }
    }
}
