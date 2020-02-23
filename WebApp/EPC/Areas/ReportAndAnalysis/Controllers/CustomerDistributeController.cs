using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using Formula.Exceptions;
using MvcAdapter;
using Config;
using Config.Logic;
using EPC.Logic;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class CustomerDistributeController : EPCController
    {
        //省份
        private Dictionary<string, string> _province = null;
        private Dictionary<string, string> province
        {
            get
            {
                if (_province == null)
                {
                    //创建省份数据，省份格式为value:"beijing",text:"北京"
                    _province = CreateProvince();
                }
                return _province;
            }
        }
        private Dictionary<string, string> CreateProvince()
        {
            Dictionary<string, string> tempProvince = new Dictionary<string, string>();
            tempProvince.Add("beijing", "北京");
            tempProvince.Add("tianjing", "天津");
            tempProvince.Add("shanghai", "上海");
            tempProvince.Add("chongqing", "重庆");
            tempProvince.Add("hebei", "河北");
            tempProvince.Add("henan", "河南");
            tempProvince.Add("yunnan", "云南");
            tempProvince.Add("liaoning", "辽宁");
            tempProvince.Add("heilongjiang", "黑龙江");
            tempProvince.Add("hunan", "湖南");
            tempProvince.Add("anhui", "安徽");
            tempProvince.Add("shandong", "山东");
            tempProvince.Add("xinjiang", "新疆");
            tempProvince.Add("jiangsu", "江苏");
            tempProvince.Add("zhejiang", "浙江");
            tempProvince.Add("jiangxi", "江西");
            tempProvince.Add("hubei", "湖北");
            tempProvince.Add("guangxi", "广西");
            tempProvince.Add("gansu", "甘肃");
            tempProvince.Add("Shanxi", "山西");
            tempProvince.Add("neimenggu", "内蒙古");
            tempProvince.Add("shanxi", "陕西");
            tempProvince.Add("jilin", "吉林");
            tempProvince.Add("fujian", "福建");
            tempProvince.Add("guizhou", "贵州");
            tempProvince.Add("guangdong", "广东");
            tempProvince.Add("qinghai", "青海");
            tempProvince.Add("xizang", "西藏");
            tempProvince.Add("sichuan", "四川");
            tempProvince.Add("ningxia", "宁夏");
            tempProvince.Add("hainan", "海南");
            tempProvince.Add("taiwan", "台湾");
            tempProvince.Add("xianggang", "香港");
            tempProvince.Add("aomen", "澳门");
            tempProvince.Add("nanhai", "南海诸岛");
            return tempProvince;
        }

        //包含省份、前期项目个数、合同个数、客户个数的DataTable
        private DataTable _mapData = null;
        private DataTable mapData
        {
            get
            {
                if (_mapData == null)
                {
                    string mapDataSql = @"
                    SELECT  CASE WHEN ProvinceValue = '' THEN '空值'
                                 ELSE ProvinceValue
                            END AS Province ,
                            *
                    FROM    ( SELECT    CASE WHEN a.Province IS NULL THEN mp.Province
                                             ELSE a.Province
                                        END ProvinceValue ,
                                        ISNULL(a.CustomerCount, 0) CustomerCount ,
                                        ISNULL(a.ContractCount, 0) ContractCount ,
                                        ISNULL(PreProjectCount, 0) AS PreProjectCount
                              FROM      ( SELECT    CASE WHEN Province IS NULL
                                                              OR Province = '' THEN ''
                                                         ELSE Province
                                                    END Province ,
                                                    ISNULL(COUNT(0), 0) AS CustomerCount ,
                                                    ISNULL(SUM(ContractCount), 0) AS ContractCount
                                          FROM      dbo.S_M_CustomerInfo c
                                                    LEFT JOIN ( SELECT  PartyA ,
                                                                        COUNT(0) AS ContractCount
                                                                FROM    dbo.S_M_ContractInfo
                                                                WHERE   ContractState = 'Sign'
                                                                GROUP BY PartyA
                                                              ) mc ON mc.PartyA = c.ID
                                          WHERE     1 = 1
                                          GROUP BY  c.Province
                                        ) a
                                        FULL OUTER JOIN ( SELECT    COUNT(0) AS PreProjectCount ,
                                                                    Province
                                                          FROM      ( SELECT    CASE
                                                                                  WHEN S_M_CustomerInfo.Province IS NULL
                                                                                  OR S_M_CustomerInfo.Province = ''
                                                                                  THEN ''
                                                                                  ELSE S_M_CustomerInfo.Province
                                                                                END Province 
                                                            
                                                                      FROM      dbo.S_I_Engineering
												                      LEFT JOIN S_M_CustomerInfo ON S_M_CustomerInfo.ID = S_I_Engineering.CustomerInfo
                                                                    ) S_I_Project
                                                          GROUP BY  Province
                                                        ) mp ON mp.Province = a.Province
                            ) tmp
";
                    _mapData = this.SqlHelper.ExecuteDataTable(mapDataSql);
                }
                return _mapData;
            }
        }

        //包含省份、客户ID，客户Name，合同额,到款额的DataTable
        private static DataTable _listData = null;
        private static DataTable listData
        {
            get
            {
                if (_listData == null)
                {
                    string listDataSql = @"
                            select c.ID,c.Name,c.Province,ISNULL(ContractRMBAmount,0) as ContractRMBAmount,
                            ISNULL(ReceiptAmount,0) as ReceiptAmount
                            from S_M_CustomerInfo c
                            left join
                            (
                            --合同额
                            select PartyA,ISNULL(sum(ContractRMBValue),0) as ContractRMBAmount
                            from S_M_ContractInfo where ContractState = 'Sign'
                            group by PartyA
                            ) mc on mc.PartyA = c.ID
                            left join
                            (
                             --集团和子公司总到款额
                            select CustomerInfo,ISNULL(sum(ReceiptValue),0) as ReceiptAmount
                            from S_M_Receipt
                            group by CustomerInfo
                            ) r on r.CustomerInfo = c.ID
                            order by ContractRMBAmount desc";
                    SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                    _listData = helper.ExecuteDataTable(listDataSql);
                }
                return _listData;
            }
        }

        //包含省份、个数(前期项目个数、合同个数、客户个数)、类型(前期项目(PreProject)、合同(Contract)、客户(Customer))的类
        private class MapData
        {
            public string name; //province
            public string provinceValue;    //数据库中province的值
            public int value;    //count
            public string type;
            public List<ListData> list = new List<ListData>();
            public void CreateMapData(string tempName, string tempProvinceValue, string countValue, string typeValue)
            {
                name = tempName;
                provinceValue = tempProvinceValue;
                value = Convert.ToInt32(countValue);
                type = typeValue;
                if (type == "Customer")
                {
                    GetListData(ref list, tempName);
                }
            }
        }

        //包含省份、客户ID，客户Name，合同额,到款额的类
        private class ListData
        {
            public string province;
            public string customerID;
            public string customerName;
            public decimal contractRMBAmount;
            public decimal receiptRMBAmount;
            public double receiptRate;

            public void CreateListData(string tempProvinceValue, DataRow dataRow)
            {
                province = tempProvinceValue;
                customerID = dataRow["ID"] != null ? dataRow["ID"].ToString() : "";
                customerName = dataRow["Name"] != null ? dataRow["Name"].ToString() : "";
                contractRMBAmount = dataRow["ContractRMBAmount"] != null ? Convert.ToDecimal(dataRow["ContractRMBAmount"]) / 10000 : (decimal)0.0000;
                receiptRMBAmount = dataRow["ReceiptAmount"] != null ? Convert.ToDecimal(dataRow["ReceiptAmount"]) / 10000 : (decimal)0.0000;
                if (contractRMBAmount > 0)
                    receiptRate = Math.Round(Convert.ToDouble(receiptRMBAmount * 100 / contractRMBAmount), 2);
                else
                    receiptRate = 0.00;
            }
        }

        private static void GetListData(ref List<ListData> tempList, string tempProvinceValue)
        {
            //记录循环的次数
            int count = 0;
            for (int j = 0; j < listData.Rows.Count; j++)
            {
                if (count < 3)
                {
                    if (tempProvinceValue == "全国")
                    {
                        ListData row = new ListData();
                        row.CreateListData(tempProvinceValue, listData.Rows[j]);
                        tempList.Add(row);
                        count++;
                    }
                    else if (listData.Rows[j]["Province"] != null && listData.Rows[j]["Province"].ToString().Contains(tempProvinceValue))
                    {
                        ListData row = new ListData();
                        row.CreateListData(tempProvinceValue, listData.Rows[j]);
                        tempList.Add(row);
                        count++;
                    }
                }
            }
        }

        public JsonResult GetMapData()
        {
            //前期项目列表、合同列表、客户个数
            List<MapData> preProjectList = new List<MapData>();
            List<MapData> contractList = new List<MapData>();
            List<MapData> customerList = new List<MapData>();
            _listData = null;

            //每个省份合同额在前三名的客户
            List<ListData> list = new List<ListData>();

            //全国的前期项目、合同、客户个数
            int preProjectCount = 0;
            int contractCount = 0;
            int customerCount = 0;
            foreach (var item in province)
            {
                for (int i = 0; i < mapData.Rows.Count; i++)
                {
                    if (mapData.Rows[i]["Province"] != null && mapData.Rows[i]["Province"].ToString().Contains(item.Value))
                    {
                        MapData preProjectRow = new MapData();
                        if (mapData.Rows[i]["PreProjectCount"] != null)
                            preProjectRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), mapData.Rows[i]["PreProjectCount"].ToString(), "PreProject");
                        else
                            preProjectRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), "0", "PreProject");

                        MapData contractRow = new MapData();
                        if (mapData.Rows[i]["ContractCount"] != null)
                            contractRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), mapData.Rows[i]["ContractCount"].ToString(), "Contract");
                        else
                            contractRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), "0", "Contract");


                        MapData customerRow = new MapData();
                        if (mapData.Rows[i]["CustomerCount"] != null)
                            customerRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), mapData.Rows[i]["CustomerCount"].ToString(), "Customer");
                        else
                            customerRow.CreateMapData(item.Value, mapData.Rows[i]["Province"].ToString(), "0", "Customer");

                        preProjectList.Add(preProjectRow);
                        contractList.Add(contractRow);
                        customerList.Add(customerRow);

                        preProjectCount += preProjectRow.value;
                        contractCount += contractRow.value;
                        customerCount += customerRow.value;
                        break;
                    }
                }
            }

            //计算省份为空前期项目、合同、客户数据之和
            for (int i = 0; i < mapData.Rows.Count; i++)
            {
                if (mapData.Rows[i]["ProvinceValue"] == null || string.IsNullOrEmpty(mapData.Rows[i]["ProvinceValue"].ToString()))
                {
                    if (mapData.Rows[i]["PreProjectCount"] != null)
                        preProjectCount += Convert.ToInt32(mapData.Rows[i]["PreProjectCount"]);
                    if (mapData.Rows[i]["ContractCount"] != null)
                        contractCount += Convert.ToInt32(mapData.Rows[i]["ContractCount"]);
                    if (mapData.Rows[i]["CustomerCount"] != null)
                        customerCount += Convert.ToInt32(mapData.Rows[i]["CustomerCount"]);

                }
            }

            MapData preProjectRowOfChina = new MapData();
            preProjectRowOfChina.CreateMapData("全国", "", preProjectCount.ToString(), "PreProject");
            preProjectList.Add(preProjectRowOfChina);

            MapData contractRowOfChina = new MapData();
            contractRowOfChina.CreateMapData("全国", "", contractCount.ToString(), "Contract");
            contractList.Add(contractRowOfChina);

            MapData customerRowOfChina = new MapData();
            customerRowOfChina.CreateMapData("全国", "", customerCount.ToString(), "Customer");
            customerList.Add(customerRowOfChina);

            List<List<MapData>> returnList = new List<List<MapData>>();
            returnList.Add(customerList);
            returnList.Add(contractList);
            returnList.Add(preProjectList);

            return Json(returnList);
        }

        #region 区域饼图

        public ActionResult PieCharts()
        {
            var enumDef = this.GetQueryString("GroupFieldEnum");
            if (!string.IsNullOrEmpty(enumDef))
            {
                var enumName = enumDef.Split('.').Last();
                var json = FormulaHelper.GetService<IEnumService>().GetEnumJson(enumDef);
                string result = string.Format("var {0} = {1};", enumName, json);
                ViewBag.UrlEnumScript = result;
            }
            var groupField = this.GetQueryString("GroupField");
            if (string.IsNullOrEmpty(groupField))
                groupField = "Province";//默认看各省
            ViewBag.GroupField = groupField;
            return View();
        }

        public JsonResult GetList()
        {
            var groupField = this.GetQueryString("GroupField");
            if (string.IsNullOrEmpty(groupField))
                groupField = "Province";//默认看各省
            //需要先验证groupField字段是否存在于客户表S_M_CustomerInfo
            try
            {
                var exsit = this.SqlHelper.ExecuteNonQuery("select top 1 " + groupField + " from S_M_CustomerInfo ;");
            }
            catch (Formula.Exceptions.BusinessValidationException)
            {
                throw new Formula.Exceptions.BusinessValidationException("请确认字段【" + groupField + "】在客户表存在！");
            }
            string sql = @" select case when GroupValue='' then '空值' else GroupValue end as GroupText,* from (
                                select case when a.{0} IS null then mp.{0} else a.{0} end GroupValue, 
                                ISNULL(a.CustomerCount,0)CustomerCount,ISNULL(a.ContractCount,0)ContractCount,
                                ISNULL(PreProjectCount,0) as PreProjectCount,ISNULL(a.SumContractRMBAmount,0)SumContractRMBAmount from(
                                select case when {0} IS null or {0}='' then '' else {0} end {0},
                                ISNULL(count(0),0) as CustomerCount, 
                                ISNULL(sum(ContractCount),0) as ContractCount,
                                ISNULL(sum(SumContractRMBAmount),0) as SumContractRMBAmount
                                from S_M_CustomerInfo c left join 
                                (
                                select PartyA,count(0) as ContractCount,sum(ContractRMBValue) as SumContractRMBAmount
                                from S_M_ContractInfo where 1=1 {1} group by PartyA
                                ) mc on mc.PartyA = c.ID
                                where 1=1 
                                group by c.{0}
                                )a 
                                full outer join
                                (
                                select Count(0) as PreProjectCount,{0} from 
                                (select case when {0} IS null or {0}='' then '' else {0} end {0},
                                id from S_I_Engineering where 1=1 {2}) S_I_Project
                                group by {0}
                                )mp on mp.{0}=a.{0} 
                                ) tmp
                                order by CustomerCount DESC";

            string queryData = this.Request["QueryData"];
            string startDate = string.Empty;// new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
            //var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();
            //BusinessDate字段不存在
            //string customerWhereStr = string.Empty;
            string contractWhereStr = " and ContractState='Sign' ";
            string projectWhereStr = string.Empty;
            if (!String.IsNullOrEmpty(queryData))
            {
                var query = JsonHelper.ToObject(queryData);
                var lastYear = String.IsNullOrEmpty(query.GetValue("LastYear")) ? 0 : Convert.ToInt32(query.GetValue("LastYear"));
                if (lastYear > 0)
                {
                    startDate = new DateTime(DateTime.Now.Year - lastYear + 1, 1, 1).ToShortDateString();
                    //customerWhereStr += " and BusinessDate >='" + startDate + "' ";
                    contractWhereStr += " and SignDate >='" + startDate + "'";
                    projectWhereStr += " and CreateDate >='" + startDate + "' ";
                }
            }
            sql = String.Format(sql, groupField, contractWhereStr, projectWhereStr);
            var data = this.SqlHelper.ExecuteDataTable(sql);
            //替换枚举
            var enumKey = this.GetQueryString("GroupFieldEnum");
            if (!string.IsNullOrEmpty(enumKey))
            {
                var enumDef = EnumBaseHelper.GetEnumDef(enumKey);
                if (enumDef != null)
                {
                    var enums = enumDef.EnumItem.ToList();
                    if (enums.Count > 0)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["GroupText"] == null || row["GroupText"] == DBNull.Value)
                                continue;
                            var value = row["GroupText"].ToString();
                            var _e = enums.FirstOrDefault(a => a.Code == value);
                            if (_e != null)
                                row["GroupText"] = _e.Name;
                        }
                    }
                }
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", data);
            result["chartData_Customer"] = HighChartHelper.CreatePieChart("区域客户个数", "区域客户个数", data, "GroupText", "CustomerCount").Render();
            result["chartData_Contract"] = HighChartHelper.CreatePieChart("区域合同个数", "区域合同个数", data, "GroupText", "ContractCount").Render();
            result["chartData_PreProject"] = HighChartHelper.CreatePieChart("区域项目个数", "区域项目个数", data, "GroupText", "PreProjectCount").Render();
            result["StartDate"] = startDate;
            return Json(result);
        }

        #endregion

    }
}
