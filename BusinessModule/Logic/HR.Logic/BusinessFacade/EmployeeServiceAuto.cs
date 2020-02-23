using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HR.Logic.Domain;
using System.Data;
using Formula;
using MvcAdapter;
using Config;
using Base.Logic.Domain;

namespace HR.Logic.BusinessFacade
{
    public class EmployeeServiceAuto
    {
        private static SQLHelper hrHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);

        /// <summary>
        /// 拼接人员基本信息及子集的tab页
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <param name="FuncType"></param>
        /// <param name="EmployWay">用工形式</param>
        /// <returns></returns>
        public static string BuildTabs(string EmployeeID, string FuncType, string EmployWay)
        {
            string TempTab = @"<div title='{0}' url='{1}' showCloseButton='true'>{2}</div>";
            StringBuilder sb = new StringBuilder();
            DataTable dt = hrHelper.ExecuteDataTable("select * from T_EmployeeBaseSet where EmploymentWay='" + EmployWay + "' order by SortIndex asc ");
            DataTable dtEmployee = hrHelper.ExecuteDataTable("select *  from T_Employee where ID='" + EmployeeID + "'");
            if (dt != null && dt.Rows.Count > 0 && dtEmployee != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string FilterField = ObjectHelper.ObjectToString(dt.Rows[i]["FilterField"]);
                    string Title = ObjectHelper.ObjectToString(dt.Rows[i]["Title"]);
                    string Url = ObjectHelper.ObjectToString(dt.Rows[i]["Url"]);
                    if (!string.IsNullOrWhiteSpace(Url))
                    {
                        if (!string.IsNullOrWhiteSpace(FilterField))
                        {
                            string Value = ObjectHelper.ObjectToString(dtEmployee.Rows[0][FilterField]);
                            Url = Url.Replace("{" + FilterField + "}", Value);
                        }
                        if (!string.IsNullOrWhiteSpace(FuncType))
                            Url += ((Url.IndexOf('?') > -1) ? "&" : "?") + "FuncType=" + FuncType;
                        sb.AppendFormat(TempTab, Title, Url, Title);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取在职人员列表
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public static GridData GetIncumbencyEmployeeData(QueryBuilder qb)
        {
            string sql = "SELECT * FROM T_Employee Where IsDeleted='0' ";
            GridData data = hrHelper.ExecuteGridData(sql, qb);
            return data;
        }


        /// <summary>
        /// 获取离退人员列表
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public static GridData GetRetiredEmployeeData(QueryBuilder qb)
        {
            string sql = "SELECT * FROM T_Employee Where IsDeleted='1' ";
            GridData data = hrHelper.ExecuteGridData(sql, qb);
            return data;
        }

        /// <summary>
        /// 获取所有人员列表
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public static GridData GetEmployeeData(QueryBuilder qb)
        {
            string sql = "SELECT * FROM T_Employee  ";
            GridData data = hrHelper.ExecuteGridData(sql, qb);
            return data;
        }


        /// <summary>
        /// 根据员工ID获取对应的系统用户ID
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public static string GetSystemUserIDByEmployeeID(string employeeID)
        {
            string sql = string.Format("SELECT * FROM dbo.T_Employee where id='{0}'", employeeID);
            DataTable dt = hrHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["UserID"].ToString();

        }

        /// <summary>
        /// 根据员工ID获取对应的系统用户ID
        /// </summary>
        /// <param name="employeeIDArray"></param>
        /// <returns></returns>
        public static string GetSystemUserIDsByEmployeeIDs(string[] employeeIDArray)
        {
            string employeeIDs = string.Join("','", employeeIDArray);
            string sql = string.Format("SELECT * FROM dbo.T_Employee where id in ('{0}')", employeeIDs);
            DataTable dt = hrHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return "";

            string userIDs = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["UserID"] == null)
                    continue;
                else
                    userIDs += dr["UserID"] + ",";
            }
            if (!string.IsNullOrEmpty(userIDs))
                userIDs = userIDs.TrimEnd(',');

            return userIDs;
        }

        /// <summary>
        /// 根据员工ID获取对应的系统用户ID
        /// </summary>
        /// <param name="employeeIDs"></param>
        /// <returns></returns>
        public static string GetSystemUserIDsByEmployeeIDs(string employeeIDs)
        {
            string[] employeeIDArray = employeeIDs.Split(',');
            return GetSystemUserIDsByEmployeeIDs(employeeIDArray);
        }


        /// <summary>
        /// 获取员工主责部门ID
        /// </summary>
        /// <param name="employeeID">人员</param>
        /// <returns>主责部门ID</returns>
        public static string GetEmployeeMainDeptID(string employeeID)
        {
            string sql = string.Format("SELECT * FROM dbo.T_EmployeeJob where employeeid ='{0}' and isMain='1' and isdeleted='0'", employeeID);
            DataTable dt = hrHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["DeptID"].ToString();

        }

        #region 同步系统用户
        //新添雇员加入系统用户
        public void EmployeeAddToUser(T_Employee employee, string Password)
        {
            if (employee.IsDeleted == "1")
                return;
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            //添加新用户
            S_A_User user = new S_A_User()
            {
                ID = FormulaHelper.CreateGuid(),
                Code = employee.Code,
                Name = employee.Name,
                WorkNo = employee.Code,
                Address = employee.Address,
                Email = employee.Email,
                Password = Password,
                Sex = employee.Sex,
                InDate = employee.JoinCompanyDate,
                Phone = employee.OfficePhone,
                MobilePhone = employee.MobilePhone,
                ModifyTime = DateTime.Now,
                IsDeleted = "0"
            };
            baseEntities.Set<S_A_User>().Add(user);

            //反写userID
            employee.UserID = user.ID;

            List<string> orgIDList = new List<string>();
            List<S_A_Org> orgList = baseEntities.Set<S_A_Org>().Where(a => a.IsDeleted == "0").ToList();
            S_A_Org org = null;
            if (employee.DeptID != null)
                org = orgList.Where(c => c.ID == employee.DeptID && c.IsDeleted == "0").FirstOrDefault();

            if (org == null)
                org = orgList.Where(c => c.ParentID == null || c.ParentID == "").FirstOrDefault();

            if (org != null)
            {
                user.DeptID = org.ID;
                user.DeptName = org.Name;
                user.DeptFullID = org.FullID;

                string companyID = "";
                string companyName = "";
                //逆序判断类型是否为集团/公司
                string[] orgIDs = org.FullID.Split('.');
                var orgs = orgList.Where(c => orgIDs.Contains(c.ID)).ToDictionary<S_A_Org, string>(d => d.ID);

                for (var i = orgIDs.Length; i > 0; i--)
                {
                    if ((orgs[orgIDs[i - 1]].Type ?? "none").IndexOf("Company") > -1)
                    {
                        companyID = orgs[orgIDs[i - 1]].ID;
                        companyName = orgs[orgIDs[i - 1]].Name;
                        break;
                    }
                }
                user.CorpID = companyID;
                user.CorpName = companyName;
            }
            if (!string.IsNullOrEmpty(user.DeptFullID))
            {
                foreach (var id in user.DeptFullID.Split('.'))
                {
                    if (!orgIDList.Contains(id))
                    {
                        S_A__OrgUser orgUser = new S_A__OrgUser();
                        orgUser.OrgID = id;
                        orgUser.UserID = user.ID;
                        baseEntities.Set<S_A__OrgUser>().Add(orgUser);
                        orgIDList.Add(id);
                    }
                }
            }
            if (!string.IsNullOrEmpty(employee.ParttimeDeptID))
            {
                foreach (var id in employee.ParttimeDeptID.Split(','))
                {
                    var parttimeOrg = orgList.FirstOrDefault(c => c.ID == employee.DeptID);
                    if (parttimeOrg != null)
                    {
                        foreach (var parttimeOrgID in parttimeOrg.FullID.Split('.'))
                        {
                            if (!orgIDList.Contains(parttimeOrgID))
                            {
                                S_A__OrgUser orgUser = new S_A__OrgUser();
                                orgUser.OrgID = parttimeOrgID;
                                orgUser.UserID = user.ID;
                                baseEntities.Set<S_A__OrgUser>().Add(orgUser);
                                orgIDList.Add(id);
                            }
                        }
                    }
                }
            }

            //添加用户图片
            S_A_UserImg userImg = new S_A_UserImg()
            {
                ID = FormulaHelper.CreateGuid(),
                UserID = user.ID,
                Picture = employee.Portrait,
                SignImg = employee.SignImage
            };
            baseEntities.Set<S_A_UserImg>().Add(userImg);

            entities.SaveChanges();
            baseEntities.SaveChanges();
        }

        //更新雇员加入系统用户
        public void EmployeeUpdateToUser(T_Employee employee)
        {
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            var enumService = FormulaHelper.GetService<IEnumService>();
            S_A_User  user = baseEntities.Set<S_A_User>().Find(employee.UserID);
            if (user == null)
                user = baseEntities.Set<S_A_User>().FirstOrDefault(c => c.Code == employee.Code && c.IsDeleted == "0");
            employee.UserID = user.ID;
            user.WorkNo = employee.Code;
            //user.Code = employee.Code;
            user.Name = employee.Name;
            user.IsDeleted = employee.IsDeleted;
            user.Sex = employee.Sex;
            user.ModifyTime = DateTime.Now;

            if (employee.Address != null)
                user.Address = employee.Address;
            if (employee.Email != null)
                user.Email = employee.Email;
            if (employee.OfficePhone != null)
                user.Phone = employee.OfficePhone;
            if (employee.MobilePhone != null)
                user.MobilePhone = employee.MobilePhone;


            S_A_Org org = null;
            user.S_A__OrgUser.RemoveWhere(c => c.S_A_Org.Type != "Post");
            List<string> orgIDList = user.S_A__OrgUser.Select(a => a.OrgID).ToList();
            List<S_A_Org> orgList = baseEntities.Set<S_A_Org>().Where(a => a.IsDeleted == "0").ToList();

            if (employee.DeptID != null)
                org = orgList.Where(c => c.ID == employee.DeptID).FirstOrDefault();

            if (org == null)
                org = orgList.Where(c => c.ParentID == null || c.ParentID == "").FirstOrDefault();

            if (org != null)
            {
                user.DeptID = org.ID;
                user.DeptName = org.Name;
                user.DeptFullID = org.FullID;

                string companyID = "";
                string companyName = "";
                //逆序判断类型是否为集团/公司
                string[] orgIDs = org.FullID.Split('.');
                var orgs = orgList.Where(c => orgIDs.Contains(c.ID)).ToDictionary<S_A_Org, string>(d => d.ID);

                for (var i = orgIDs.Length; i > 0; i--)
                {
                    if ((orgs[orgIDs[i - 1]].Type ?? "none").IndexOf("Company") > -1)
                    {
                        companyID = orgs[orgIDs[i - 1]].ID;
                        companyName = orgs[orgIDs[i - 1]].Name;
                        break;
                    }
                }
                user.CorpID = companyID;
                user.CorpName = companyName;
            }
            if (!string.IsNullOrEmpty(user.DeptFullID))
            {
                foreach (var id in user.DeptFullID.Split('.'))
                {
                    if (!orgIDList.Contains(id))
                    {
                        S_A__OrgUser orgUser = new S_A__OrgUser();
                        orgUser.OrgID = id;
                        orgUser.UserID = user.ID;
                        baseEntities.Set<S_A__OrgUser>().Add(orgUser);
                        orgIDList.Add(id);
                    }
                }
            }
            if (!string.IsNullOrEmpty(employee.ParttimeDeptID))
            {
                foreach (var id in employee.ParttimeDeptID.Split(','))
                {
                    var parttimeOrg = orgList.FirstOrDefault(c => c.ID == employee.DeptID);
                    if (parttimeOrg != null)
                    {
                        foreach (var parttimeOrgID in parttimeOrg.FullID.Split('.'))
                        {
                            if (!orgIDList.Contains(parttimeOrgID))
                            {
                                S_A__OrgUser orgUser = new S_A__OrgUser();
                                orgUser.OrgID = parttimeOrgID;
                                orgUser.UserID = user.ID;
                                baseEntities.Set<S_A__OrgUser>().Add(orgUser);
                                orgIDList.Add(id);
                            }
                        }
                    }
                }
            }
            //修改用户图片
            S_A_UserImg userImg = null;
            List<S_A_UserImg> userImgList = baseEntities.Set<S_A_UserImg>().Where(c => c.UserID == user.ID).ToList();
            if (userImgList.Count > 0)
                userImg = userImgList.FirstOrDefault();
            else
            {
                userImg = new S_A_UserImg()
                {
                    ID = FormulaHelper.CreateGuid(),
                    UserID = user.ID
                };
                baseEntities.Set<S_A_UserImg>().Add(userImg);
            }

            if (employee.Portrait != null)
                userImg.Picture = employee.Portrait;
            if (employee.SignImage != null)
                userImg.SignImg = employee.SignImage;
            if (employee.SignDwg != null)
                userImg.DwgFile = employee.SignDwg;
            
            entities.SaveChanges();
            baseEntities.SaveChanges();
        }


        /// <summary>
        /// 同步删除的人员到系统用户表
        /// </summary>
        /// <param name="employee"></param>
        public void EmployeeDeleteToUser(T_Employee employee)
        {
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            S_A_User user = baseEntities.Set<S_A_User>().Where(t => t.ID == employee.UserID).FirstOrDefault();
            if (user != null)
            {
                user.DeleteTime = DateTime.Now;
                user.ModifyTime = user.DeleteTime;
                user.IsDeleted = "1";
                user.Code = user.ID;
                baseEntities.Set<S_A__RoleUser>().Delete(a => a.UserID == user.ID);
                var retiredList = entities.Set<T_EmployeeRetired>().Where(c => c.EmployeeID == employee.ID).OrderBy("RetiredDate", false);
                if (retiredList.Count() > 0)
                {
                    user.OutDate = retiredList.FirstOrDefault().RetiredDate;
                }
                baseEntities.SaveChanges();
            }
        }
        /// <summary>
        /// 同步删除的人员到系统用户表
        /// </summary>
        /// <param name="EmployeeIds"></param>
        public void EmployeeDeleteToUser(string EmployeeIds)
        {
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            var EList = entities.Set<T_Employee>().Where(c => EmployeeIds.Contains(c.ID)).ToList();
            if (EList == null || EList.Count == 0)
                return;
            int count = 0;
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            foreach (var employee in EList)
            {
                var user = baseEntities.Set<S_A_User>().Where(t => t.ID == employee.UserID).FirstOrDefault();
                if (user != null)
                {
                    count++;
                    user.DeleteTime = DateTime.Now;
                    user.ModifyTime = user.DeleteTime;
                    user.IsDeleted = "1";

                    var retiredList = entities.Set<T_EmployeeRetired>().Where(c => c.EmployeeID == employee.ID).OrderBy("RetiredDate", false);
                    if (retiredList.Count() > 0)
                    {
                        user.OutDate = retiredList.FirstOrDefault().RetiredDate;
                    }
                }
            }
            if (count > 0)
                baseEntities.SaveChanges();
        }

        /// <summary>
        /// 恢复系统用户的账号状态
        /// </summary>
        /// <param name="EmployeeIds">员工基本信息表id</param>
        public void ResetSysUserState(string EmployeeIds,string NewCode)
        {
            if (string.IsNullOrWhiteSpace(EmployeeIds))
                return;
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            string[] UserArr = entities.Set<T_Employee>().Where(c => EmployeeIds.Contains(c.ID)).ToList().Select(t => t.UserID).ToArray();
            if (UserArr == null || UserArr.Length == 0)
                return;
            string UserIds = string.Join(",", UserArr);
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            baseEntities.Set<S_A_User>().Where(t => UserIds.Contains(t.ID)).Update(t =>
            {
                t.DeleteTime = null;
                t.ModifyTime = DateTime.Now;
                t.IsDeleted = "0";
                t.OutDate = null;
                if (!string.IsNullOrEmpty(NewCode))
                {
                    t.Code = NewCode;
                    t.WorkNo = NewCode;
                }
            });
            baseEntities.SaveChanges();
        }
        #endregion
    }
}
