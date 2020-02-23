using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Entity;
using Formula;
using Config;
using Config.Logic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EPC.Logic.Domain
{
    public partial class S_T_FolderDef
    {
        #region 公开自定义属性
        List<S_T_FolderAuth> _FolderAuth;
        [NotMapped]
        [JsonIgnore]
        public List<S_T_FolderAuth> FolderAuth
        {
            get
            {
                if (_FolderAuth == null)
                {
                    if (this.InhertAuth == true.ToString())
                    {
                        var list = this.Ancestors.Where(d => d.S_T_FolderAuth.Count > 0);
                        if (list.Count() == 0)
                        {
                            _FolderAuth = this.S_T_FolderAuth.ToList();
                        }
                        else
                        {
                            var last = list.OrderBy(d => d.FullID).LastOrDefault();
                            if (last != null)
                            {
                                _FolderAuth = last.S_T_FolderAuth.ToList();
                            }
                            else
                            {
                                _FolderAuth = this.S_T_FolderAuth.ToList();
                            }
                        }
                    }
                    else
                        _FolderAuth = this.S_T_FolderAuth.ToList();
                }
                return _FolderAuth;
            }
        }

        S_T_FolderDef _Parent = null;
        /// <summary>
        /// 父目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_FolderDef Parent
        {
            get
            {
                if (_Parent == null)
                {
                    if (!String.IsNullOrEmpty(this.ParentID))
                    {
                        _Parent = this.S_T_FolderTemplate.S_T_FolderDef.FirstOrDefault(d => d.ID == this.ParentID);
                    }
                }
                return _Parent;
            }
        }

        List<S_T_FolderDef> _Ancestors;
        /// <summary>
        /// 所有上级目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_FolderDef> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    var dbContext = this.GetDbContext<InfrastructureEntities>();
                    _Ancestors = dbContext.S_T_FolderDef.Include("S_T_FolderAuth").Where(d => this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _Ancestors;
            }
        }

        List<S_T_FolderDef> _Children;
        /// <summary>
        /// 子目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_FolderDef> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = this.S_T_FolderTemplate.S_T_FolderDef.Where(d => d.ParentID == this.ID).ToList();
                }
                return _Children;
            }
        }

        List<S_T_FolderDef> _AllChildren;
        /// <summary>
        /// 下级目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_FolderDef> AllChildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    _AllChildren = this.S_T_FolderTemplate.S_T_FolderDef.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }
        #endregion
    }
}
