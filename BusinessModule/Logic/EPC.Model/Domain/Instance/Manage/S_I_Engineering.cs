
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Config;
using Config.Logic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Formula;
using System.Text.RegularExpressions;

namespace EPC.Logic.Domain
{
    public partial class S_I_Engineering
    {
        #region 公共属性

        S_C_Mode _mode;
        /// <summary>
        /// 管理模式
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_Mode Mode
        {
            get
            {
                if (_mode == null)
                    _mode = S_C_Mode.GetMode(this.ModeCode);
                if (_mode == null)
                {
                    _mode = S_C_Mode.GetMode(this.ModeCode);
                }
                if (_mode == null)
                    throw new Formula.Exceptions.BusinessValidationException("工程没有指定管理模式，无法获取管理模式对象");
                return _mode;
            }
        }

        S_I_CBS _cbsRoot;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS CBSRoot
        {
            get
            {
                if (_cbsRoot == null)
                    _cbsRoot = this.S_I_CBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _cbsRoot;
            }
        }

        S_I_CBS _deviceRoot;
        /// <summary>
        /// 设备材料费用根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS DeviceRoot
        {
            get
            {
                if (_deviceRoot == null)
                {
                    var list = this.S_I_CBS.Where(d => d.CBSType == "Device").OrderBy(d => d.FullID).ToList();
                    if (list.Count > 0)
                        _deviceRoot = list.FirstOrDefault();
                }

                return _deviceRoot;
            }
        }

        S_I_CBS _constructionRoot;
        /// <summary>
        /// 施工安装费用根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS ConstructionRoot
        {
            get
            {
                if (_constructionRoot == null)
                {
                    var list = this.S_I_CBS.Where(d => d.CBSType == "Construction").OrderBy(d => d.FullID).ToList();
                    if (list.Count > 0)
                        _constructionRoot = list.FirstOrDefault();
                }

                return _constructionRoot;
            }
        }

        S_I_CBS _laborRoot;
        /// <summary>
        /// 人工费费用根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS LaborRoot
        {
            get
            {
                if (_laborRoot == null)
                {
                    var list = this.S_I_CBS.Where(d => d.CBSType == "Labor").OrderBy(d => d.FullID).ToList();
                    if (list.Count > 0)
                        _laborRoot = list.FirstOrDefault();
                }

                return _laborRoot;
            }
        }



        S_I_WBS _rootNode;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS WBSRoot
        {
            get
            {
                if (_rootNode == null)
                {
                    _rootNode = this.S_I_WBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                    if (_rootNode == null)
                    {
                        _rootNode = this.S_I_WBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                    }
                }
                return _rootNode;
            }
        }

        S_I_PBS _pbsRoot;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_PBS PBSRoot
        {
            get
            {
                if (_pbsRoot == null)
                    _pbsRoot = this.S_I_PBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _pbsRoot;
            }
        }


        S_I_OBS _obsRoot;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_OBS OBSRoot
        {
            get
            {
                if (_obsRoot == null)
                    _obsRoot = this.S_I_OBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _obsRoot;
            }
        }

        #endregion
    }
}
