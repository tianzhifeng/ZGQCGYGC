using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Config;
using Formula.Helper;

namespace HR.Areas.Report.Controllers
{
    public class EmployeeAnalyseController : BaseController
    {
        //
        // GET: /Report/EmployeeAnalyse/

        public ActionResult EmployeeResourceRpt()
        {
            return View();
        }

        public JsonResult GetList()
        {
            List<EmployeeResourceModel> list = new List<EmployeeResourceModel>();

            SQLHelper hrHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            string sql = "SELECT *,DatePart(yyyy,getdate())-DatePart(yyyy,birthday) as Age,DatePart(yyyy,getdate())-DatePart(yyyy,JoinCompanyDate) as ThisCompanyAge,DatePart(yyyy,getdate())-DatePart(yyyy,JoinWorkDate) as WorkAge From T_Employee where isDeleted='0' ";
            DataTable dt = hrHelper.ExecuteDataTable(sql);
            int index = 0;

            int totalCount = dt.Rows.Count;
            if (totalCount == 0)
                return Json(null);
            list.Add(new EmployeeResourceModel() { Category = "总数", Item = "员工总数", PersonCount = totalCount, Ratio = "100%", Index = index++ });


            EnumCategoryDataToList("System.Sex", "性别", list, ref index, totalCount, dt, "Sex");

            //年龄
            DataRow[] dr0_30 = dt.Select("age>=0 and age<=30");
            DataRow[] dr31_40 = dt.Select("age>=31 and age<=40");
            DataRow[] dr41_50 = dt.Select("age>=41 and age<=50");
            DataRow[] dr51_60 = dt.Select("age>=51 and age<=60");
            DataRow[] dr61_100 = dt.Select("age>=61 and age<=100");
            list.Add(new EmployeeResourceModel() { Category = "年龄结构", Item = "0-30岁", PersonCount = dr0_30.Count(), Ratio = ((decimal)dr0_30.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "年龄结构", Item = "31-40岁", PersonCount = dr31_40.Count(), Ratio = ((decimal)dr31_40.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "年龄结构", Item = "41-50岁", PersonCount = dr41_50.Count(), Ratio = ((decimal)dr41_50.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "年龄结构", Item = "51-60岁", PersonCount = dr51_60.Count(), Ratio = ((decimal)dr51_60.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "年龄结构", Item = "61-100岁", PersonCount = dr61_100.Count(), Ratio = ((decimal)dr61_100.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });

            //学历
            EnumCategoryDataToList("HR.Education", "学历", list, ref index, totalCount, dt, "Educational");

            //职称
            string sqlPositionalTitles = "Select * from dbo.T_EmployeeAcademicTitle ati where exists (Select * From T_Employee e where isDeleted='0' and ati.EmployeeID=e.ID) ";
            DataTable dtPositionalTitles = hrHelper.ExecuteDataTable(sqlPositionalTitles);
            if (dtPositionalTitles.Rows.Count >= 0)
                EnumCategoryDataToList("HR.PositionalTitle", "职称", list, ref index, totalCount, dtPositionalTitles, "Title");

            //职务
            string sqlJob = "Select JobName Job,count(ID) as Count from dbo.T_EmployeeJob job where exists (Select * From T_Employee e where isDeleted='0' and job.EmployeeID=e.ID and JobName is not null and JobName !='' ) group by JobName";
            DataTable dtJob = hrHelper.ExecuteDataTable(sqlJob);
            if (dtJob.Rows.Count >= 0)
                foreach (DataRow row in dtJob.Rows)
                {
                    list.Add(new EmployeeResourceModel() { Category = "职务", Item = row["Job"].ToString(), PersonCount = Int32.Parse(row["Count"].ToString()), Ratio = ((decimal)Int32.Parse(row["Count"].ToString()) / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
                }


            //公司工龄
            DataRow[] drCompanyAge0_4 = dt.Select("thisCompanyAge>=0 and thisCompanyAge<=4");
            DataRow[] drCompanyAge5_8 = dt.Select("thisCompanyAge>=5 and thisCompanyAge<=8");
            DataRow[] drCompanyAge9_100 = dt.Select("thisCompanyAge>=9 and thisCompanyAge<=100");
            list.Add(new EmployeeResourceModel() { Category = "公司工龄", Item = "0-4年", PersonCount = drCompanyAge0_4.Count(), Ratio = ((decimal)drCompanyAge0_4.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "公司工龄", Item = "5-8年", PersonCount = drCompanyAge5_8.Count(), Ratio = ((decimal)drCompanyAge5_8.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "公司工龄", Item = "9-100年", PersonCount = drCompanyAge9_100.Count(), Ratio = ((decimal)drCompanyAge9_100.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });

            //资历
            DataRow[] drWorkAge0_4 = dt.Select("WorkAge>=0 and WorkAge<=4");
            DataRow[] drWorkAge5_8 = dt.Select("WorkAge>=5 and WorkAge<=8");
            DataRow[] drWorkAge9_100 = dt.Select("WorkAge>=9 and WorkAge<=100");
            list.Add(new EmployeeResourceModel() { Category = "资历(工作年限)", Item = "0-4年", PersonCount = drWorkAge0_4.Count(), Ratio = ((decimal)drWorkAge0_4.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "资历(工作年限)", Item = "5-8年", PersonCount = drWorkAge5_8.Count(), Ratio = ((decimal)drWorkAge5_8.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            list.Add(new EmployeeResourceModel() { Category = "资历(工作年限)", Item = "9-100年", PersonCount = drWorkAge9_100.Count(), Ratio = ((decimal)drWorkAge9_100.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });



            return Json(list);

        }

        /// <summary>
        /// 拼凑统计信息添加到列表内
        /// </summary>
        /// <param name="enumCode">枚举编号</param>
        /// <param name="enumName">枚举名称</param>
        /// <param name="list">最终列表</param>
        /// <param name="index">排序索引</param>
        /// <param name="totalCount">总人数</param>
        /// <param name="employeeDt">员工数据源</param>
        /// <param name="fieldName">员工信息中使用该枚举的字段</param>
        private void EnumCategoryDataToList(string enumCode, string enumName, List<EmployeeResourceModel> list, ref int index, int totalCount, DataTable employeeDt, string fieldName)
        {
            DataTable dt = EnumBaseHelper.GetEnumTable(enumCode);
            foreach (DataRow row in dt.Rows)
            {
                DataRow[] dr = employeeDt.Select(string.Format("{0}='{1}'", fieldName, row["value"]));
                list.Add(new EmployeeResourceModel() { Category = enumName, Item = row["text"].ToString(), PersonCount = dr.Count(), Ratio = ((decimal)dr.Count() / (decimal)totalCount * 100).ToString("#0.00") + "%", Index = index++ });
            }

        }




    }

    public class EmployeeResourceModel
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { set; get; }

        /// <summary>
        /// 项
        /// </summary>
        public string Item { set; get; }

        /// <summary>
        /// 人数
        /// </summary>
        public int PersonCount { set; get; }

        /// <summary>
        /// 比例
        /// </summary>
        public string Ratio { set; get; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { set; get; }

    }
}
