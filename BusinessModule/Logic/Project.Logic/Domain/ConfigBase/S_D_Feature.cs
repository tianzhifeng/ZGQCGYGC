using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_D_Feature
    {
        /// <summary>
        /// 删除功能注册
        /// </summary>
        /// <param name="withSpaceDefine">是否联动删除已经注册到空间内的功能（默认为False）</param>
        public void Delete(bool withSpaceDefine=false)
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            var spDefineList = context.S_T_SpaceDefine.Where(d => d.FeatureID == this.ID).ToList();
            foreach (var item in spDefineList)
                item.Delete();
        }

        public void Save(bool withSpaceDefine = true)
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            var spDefineList = context.S_T_SpaceDefine.Where(d => d.FeatureID == this.ID).ToList();
            foreach (var item in spDefineList)
            {
                item.Name = this.Name;
                item.Code = this.Code;
                item.LinkUrl = this.LinkUrl;
            }
        }
    }
}
