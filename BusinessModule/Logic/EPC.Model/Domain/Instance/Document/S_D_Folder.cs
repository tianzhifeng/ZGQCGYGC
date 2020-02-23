
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
using Base.Logic.BusinessFacade;
using Formula.Helper;
using System.Web;
using Base.Logic.Domain;

namespace EPC.Logic.Domain
{
    public partial class S_D_Folder
    {
        #region 公共属性

        List<S_D_Folder_Auth> _FolderAuth;
        [NotMapped]
        [JsonIgnore]
        public List<S_D_Folder_Auth> FolderAuth
        {
            get
            {
                if (_FolderAuth == null)
                {
                    if (this.InhertAuth == true.ToString())
                    {
                        var list = this.Ancestors.Where(d => d.S_D_Folder_Auth.Count > 0);
                        if (list.Count() == 0)
                        {
                            _FolderAuth = this.S_D_Folder_Auth.ToList();
                        }
                        else
                        {
                            var last = list.OrderBy(d => d.FullID).LastOrDefault();
                            if (last != null)
                            {
                                _FolderAuth = last.S_D_Folder_Auth.ToList();
                            }
                            else
                            {
                                _FolderAuth = this.S_D_Folder_Auth.ToList();
                            }
                        }
                    }
                    else
                        _FolderAuth = this.S_D_Folder_Auth.ToList();
                }
                return _FolderAuth;
            }
        }

        S_D_Folder _Parent = null;
        /// <summary>
        /// 父目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_D_Folder Parent
        {
            get
            {
                if (_Parent == null)
                {
                    if (!String.IsNullOrEmpty(this.ParentID))
                    {
                        _Parent = this.S_I_Engineering.S_D_Folder.FirstOrDefault(d => d.ID == this.ParentID);
                    }
                }
                return _Parent;
            }
        }

        List<S_D_Folder> _Ancestors;
        /// <summary>
        /// 所有上级目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_Folder> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    var dbContext = this.GetDbContext<EPCEntities>();

                    _Ancestors = dbContext.S_D_Folder.Include("S_D_Folder_Auth").Where(d => this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _Ancestors;
            }
        }

        List<S_D_Folder> _Children;
        /// <summary>
        /// 子目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_Folder> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = this.S_I_Engineering.S_D_Folder.Where(d => d.ParentID == this.ID).ToList();
                }
                return _Children;
            }
        }

        List<S_D_Folder> _AllChildren;
        /// <summary>
        /// 下级目录
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_D_Folder> AllChildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    _AllChildren = this.S_I_Engineering.S_D_Folder.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }

        #endregion
    }
}
