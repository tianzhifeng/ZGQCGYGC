using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.Logic;
namespace Project.Logic.Domain
{
    public partial class T_EXE_MettingSign
    {
        public void Push()
        {
            var entites = this.GetDbContext<ProjectEntities>();
            var productList = entites.Set<S_E_Product>().Where(a => a.ProjectInfoID == this.ProjectInfoID).ToList();
            foreach (var item in this.T_EXE_MettingSign_ResultList.ToList())
            {
                var product = productList.FirstOrDefault(a => a.ID == item.ProductID);//this.ProjectEntites.Set<S_E_Product>().Find(item.ProductID);
                if (product != null)
                {
                    product.CoSignState = Project.Logic.CoSignState.SignComplete.ToString();
                    product.CounterSignAuditID = this.ID;
                    var coUserList = new List<Dictionary<string, object>>();
                    foreach (var member in this.T_EXE_MettingSign_ProjectGroupMembers.ToList())
                    {
                        var coUser = new Dictionary<string, object>();
                        coUser.SetValue("MajorValue", member.MajorCode);
                        coUser.SetValue("MajorName", member.Major);
                        coUser.SetValue("UserID", member.MettingUser);
                        coUser.SetValue("UserName", member.MettingUserName);
                        coUser.SetValue("SignDate", member.SignDate);
                        coUserList.Add(coUser);
                    }
                    product.CoSignUser = JsonHelper.ToJson(coUserList);
                    product.UpdateVersison();

                    //当只有原图校审，通过后，同步更新拆分图的签名信息、校审状态
                    var children = productList.Where(a => a.ParentID == product.ID && a.ParentVersion == product.Version).ToList();
                    foreach (var child in children)
                    {
                        child.CoSignState = Project.Logic.CoSignState.SignComplete.ToString();
                        child.CounterSignAuditID = this.ID;
                        child.CoSignUser = product.CoSignUser;
                        child.UpdateVersison();
                    }
                }
            }
        }
    }
}
