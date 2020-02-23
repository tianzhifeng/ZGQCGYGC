using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Config;
using System.Data;
using Formula;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class PersonalLoanReminderIssuedController : OfficeAutoFormContorllor<T_BM_Personalloanreminderissued>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            /*
            var userIds = dic["JKRYHID"];
            var tjDate = dic["TJJZRQ"]; //统计截止日期
            var endDate = dic["HKJZRQ"]; //还款截止日期


            //获取要催款的用户
            var userList = new List<string>();


            if (string.IsNullOrEmpty(userIds))
            {
                //获取所有用户
                var sql = @"SELECT Actualborrower
                    
                    FROM T_BM_Loanapply  

                    where (ISNULL(Loanamount, 0) - ISNULL(Repayanmount, 0)) > 0

                    GROUP BY Actualborrower,Applydept,Applyphone";

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);

                DataTable dt = sqlHelper.ExecuteDataTable(sql);

                userList = dt.AsEnumerable().Select(p => p.Field<string>("Actualborrower")).ToList();
            }
            else
            {
                //获取选中的用户
                userList = userIds.Split(',').ToList();
            }

            var userDic = new Dictionary<string, object>();

            //创建用户对应的数据
            foreach (var item in userList)
            {
                var loanList = BusinessEntities.Set<T_BM_Loanapply>().Where(p => p.Actualborrower == item).ToList();

                var amount = loanList.Sum(p => (p.Loanamount - p.Repayanmount));

                var userEntity = loanList.FirstOrDefault();

                if (userEntity != null)
                {
                    T_BM_Personalloanremindernote entity = new T_BM_Personalloanremindernote()
                    {
                        ID = Formula.FormulaHelper.CreateGuid(),
                        Dept = userEntity.Applydept, //部门
                        DeptName = userEntity.ApplydeptName,
                        Name = userEntity.Actualborrower, //姓名
                        NameName = userEntity.ActualborrowerName,
                        JZ = Convert.ToDateTime(tjDate), //统计截止日期
                        E = amount, //个人借款
                        Y = Convert.ToDateTime(endDate) //截止日期
                    };

                    userDic.Add(item, new { id = entity.ID, actualBorrower = userEntity.Actualborrower, actualBorrowerName = userEntity.ActualborrowerName, amount = amount });

                    BusinessEntities.Set<T_BM_Personalloanremindernote>().Add(entity);
                }
            }
        
            BusinessEntities.SaveChanges();

            foreach (var item in userDic)
            {
                dynamic obj = item.Value;

                //发送消息给对应的用户
                var msgService = FormulaHelper.GetService<IMessageService>();
                var title = obj.actualBorrowerName + ",个人借款催款通知";
                var content = @"截至 " + Convert.ToDateTime(tjDate).ToString("yyyy-MM-dd") + @"，您的个人借款(不包括直接支付对方单位的借款)为 " + obj.amount + @" 元。按照公司财[2018]6号《关于清理个人借款的通知》的规定，请您于 " + Convert.ToDateTime(endDate).ToString("yyyy-MM-dd") + @" 前归还个人借款或履行报销程序。特此通知";
                msgService.SendMsg(title, content, @"/MvcConfig/UI/Form/PageView?TmplCode=BM_Personalloanremindernote&ID=" + obj.id, "", obj.actualBorrower, obj.actualBorrowerName);
            }*/

        }

    }
}
