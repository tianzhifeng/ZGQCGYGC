using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EPC.Logic.Domain
{
    public partial class S_C_Meun
    {
        #region 公开属性
        /// <summary>
        /// 根节点定义
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_Meun Root
        {
            get
            {
                if (String.IsNullOrEmpty(this.ModeID)) throw new Formula.Exceptions.BusinessValidationException("未指定对象的ModeID无法获取管理模式对象，获取SpaceDefine根节点失败");
                if (this.S_C_Mode == null)
                    this.S_C_Mode = this.GetDbContext<InfrastructureEntities>().S_C_Mode.FirstOrDefault(d => d.ID == this.ModeID);
                if (this.S_C_Mode == null)
                    throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【】的管理模式对象，获取SpaceDefine根节点失败");
                return this.S_C_Mode.S_C_Meun.FirstOrDefault(d => this.FullID.StartsWith(d.FullID) && String.IsNullOrEmpty(d.ParentID));
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string SourceID
        {
            get;
            set;
        }


        List<S_C_Meun> _children;
        /// <summary>
        /// 获得下层子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_Meun> Children
        {
            get
            {
                _children = this.GetDbContext<InfrastructureEntities>().S_C_Meun.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }


        List<S_C_Meun> _allchildren;
        /// <summary>
        /// 获取所有子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_Meun> AllChildren
        {
            get
            {
                _allchildren = this.GetDbContext<InfrastructureEntities>().S_C_Meun.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                return _allchildren;
            }
        }

        S_C_Meun _Parent;
        [NotMapped]
        [JsonIgnore]
        public S_C_Meun Parent
        {
            get
            {
                if (_Parent == null)
                {
                    _Parent = this.S_C_Mode.S_C_Meun.FirstOrDefault(c => c.ID == this.ParentID);
                }
                return _Parent;
            }
        }

        //public string AuthType
        //{
        //    get
        //    {
        //        var resut = "";
        //        var list = this.S_C_MenuAuth.Select(c => c.AuthType).ToList();
        //        if (list.Count == 0) resut = "None";
        //        else if (list.Contains(SpaceAuthType.FullControl.ToString()))
        //            resut = SpaceAuthType.FullControl.ToString();
        //        else if (list.Contains(SpaceAuthType.CurrentFullControl.ToString()))
        //            resut = SpaceAuthType.CurrentFullControl.ToString();
        //        else
        //            resut = SpaceAuthType.View.ToString();
        //        return resut;
        //    }
        //}

        #endregion

        #region 公开的静态方法

        /// <summary>
        /// 增加根节点
        /// </summary>
        /// <param name="root">根节点对象</param>
        public static void AddRoot(S_C_Meun root)
        {
            root.ValidateEntity();
            var entities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var projectMode = entities.S_C_Mode.FirstOrDefault(d => d.ID == root.ModeID);
            if (projectMode == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【】的管理模式对象，无法增加SpaceDefine根");
            var meun = projectMode.S_C_Meun.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID) && d.Name == root.Name && d.ID != root.ID);
            if (meun != null) throw new Formula.Exceptions.BusinessValidationException("【" + projectMode.Name + "】的管理对象已经拥有名称【" + root.Name + "】的根菜单，不能重复增加");
            if (String.IsNullOrEmpty(root.ID))
                root.ID = FormulaHelper.CreateGuid();
            root.FullID = root.ID;
            root.MeunType = "Root";
            projectMode.S_C_Meun.Add(root);
        }

        #endregion

        /// <summary>
        /// 校验对象属性的合法性
        /// </summary>
        public void ValidateEntity()
        {
            if (String.IsNullOrEmpty(this.ModeID)) throw new Formula.Exceptions.BusinessValidationException("必须为空间定义根节点指定ModeID");
            if (String.IsNullOrEmpty(this.Name)) throw new Formula.Exceptions.BusinessValidationException("必须为空间定义根节点指定名称");
            if (String.IsNullOrEmpty(this.MeunDefineType)) throw new Formula.Exceptions.BusinessValidationException("必须为空间定义根节点指定类型");
        }
    }
}
