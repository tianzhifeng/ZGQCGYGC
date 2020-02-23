using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Entity;
using System.Collections;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;
using System.Linq.Expressions;
using Formula.DynConditionObject;
using System.Text.RegularExpressions;

namespace Project.Logic
{
    public class WBSFO
    {
        const char codeSpliteChar = '&';
        ProjectEntities instanceEnitites = FormulaHelper.GetEntities<ProjectEntities>();
        BaseConfigEntities configEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
        BaseConfigFO configFO = FormulaHelper.CreateFO<BaseConfigFO>();

        SQLHelper instanceSH = SQLHelper.CreateSqlHelper(ConnEnum.Project);
        SQLHelper configDB = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);

        public void ImportWBSNodes(Dictionary<string, object> targetNode, List<Dictionary<string, object>> sourceNodes)
        {
            string sql = @"select S_T_WBSStructInfo.* from S_T_WBSStructInfo
left join S_T_ProjectMode on S_T_WBSStructInfo.ModeID=S_T_ProjectMode.ID where ModeCode='{0}'";
            var projectInfoDt = this.instanceSH.ExecuteDataTable(String.Format("SELECT * FROM S_I_PROJECTINFO WHERE ID='{0}'", targetNode.GetValue("ProjectInfoID")));
            if (projectInfoDt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法导入WBS节点");
            }
            var wbsStructDefineDt = configDB.ExecuteDataTable(String.Format(sql, projectInfoDt.Rows[0]["ModeCode"]));
            var wbsDt = this.instanceSH.ExecuteDataTable("SELECT *,'True' as IsExist,ID as SourceNodeID FROM S_W_WBS WHERE ProjectInfoID='" + targetNode.GetValue("ProjectInfoID") + "' ");
            var taskDt = this.instanceSH.ExecuteDataTable("SELECT * FROM S_W_TaskWork WHERE 1<>1 ");

            var tmplWBSDt = wbsDt.Clone();
            tmplWBSDt.PrimaryKey = new DataColumn[] { tmplWBSDt.Columns["SourceNodeID"] };

            var targetDefineNode = wbsStructDefineDt.AsEnumerable().Where(c => c["Code"].ToString() == targetNode.GetValue("WBSStructCode")).FirstOrDefault();
            if (targetDefineNode == null)
                throw new Formula.Exceptions.BusinessValidationException("WBS定义错误，无法导入WBS节点");
            wbsDt.PrimaryKey = new DataColumn[] { wbsDt.Columns["ID"] };


            var list = sourceNodes.OrderBy(c => c.GetValue("FullID")).ToList();
            foreach (var sourceNode in list)
            {
                var parentNode = tmplWBSDt.Rows.Find(sourceNode.GetValue("ParentID"));  //wbsDt.AsEnumerable().FirstOrDefault(c => c["SourceNodeID"].ToString() == sourceNode.GetValue("ParentID"));//.Select(String.Format("SourceNodeID='{0}'", sourceNode.GetValue("ParentID")));
                if (parentNode == null)
                {
                    if (targetDefineNode["ChildCode"] != null && targetDefineNode["ChildCode"] != DBNull.Value &&
                        targetDefineNode["ChildCode"].ToString().Split(',').Contains(sourceNode.GetValue("WBSType")))
                    {
                        var wbsStructNode = wbsStructDefineDt.AsEnumerable().FirstOrDefault(c => c["Code"].ToString() == sourceNode.GetValue("WBSType"));
                        if (wbsStructNode == null)
                        {
                            continue;
                        }
                        AddWBSRow(wbsDt, taskDt, sourceNode, targetNode, wbsStructNode, projectInfoDt, tmplWBSDt);
                    }
                }
                else
                {
                    var parentStructNode = wbsStructDefineDt.AsEnumerable().FirstOrDefault(c => c["Code"].ToString() == parentNode["WBSType"].ToString());
                    if (parentStructNode != null && parentStructNode["ChildCode"] != null && parentStructNode["ChildCode"] != DBNull.Value
                        && parentStructNode["ChildCode"].ToString().Split(',').Contains(sourceNode.GetValue("WBSType")))
                    {
                        var wbsStructNode = wbsStructDefineDt.AsEnumerable().FirstOrDefault(c => c["Code"].ToString() == sourceNode.GetValue("WBSType"));
                        if (wbsStructNode == null)
                        {
                            continue;
                        }
                        AddWBSRow(wbsDt, taskDt, sourceNode, FormulaHelper.DataRowToDic(parentNode), wbsStructNode, projectInfoDt, tmplWBSDt);
                    }
                }
            }
            foreach (var item in wbsDt.Select("IsExist='True'"))
            {
                wbsDt.Rows.Remove(item);
            }
            if (wbsDt.Rows.Count > 0)
            {
                wbsDt.PrimaryKey = null;
                wbsDt.Columns.Remove("SourceNodeID");
                wbsDt.Columns.Remove("IsExist");
                this.instanceSH.BulkInsertToDB(wbsDt, "S_W_WBS");
            }
            if (taskDt.Rows.Count > 0)
            {
                this.instanceSH.BulkInsertToDB(taskDt, "S_W_TaskWork");
            }
        }

        void AddWBSRow(DataTable wbsDt, DataTable taskDt, Dictionary<string, object> sourceNode, Dictionary<string, object> parentNode,
            DataRow wbsStructNode, DataTable projectInfoDt, DataTable tmpWBSDt)
        {
            var newWBSRow = wbsDt.NewRow();
            DicFillDataRow(newWBSRow, sourceNode);
            newWBSRow["ProjectInfoID"] = parentNode.GetValue("ProjectInfoID");
            newWBSRow["ID"] = FormulaHelper.CreateGuid();
            newWBSRow["ChargeUserID"] = "";
            newWBSRow["ChargeUserName"] = "";
            newWBSRow["FullID"] = parentNode.GetValue("FullID") + "." + newWBSRow["ID"].ToString();
            newWBSRow["ParentID"] = parentNode.GetValue("ID");
            newWBSRow["SourceNodeID"] = sourceNode.GetValue("ID");
            newWBSRow["Name"] = sourceNode.GetValue("Name");
            if (wbsStructNode["CodeDefine"] != null && wbsStructNode["CodeDefine"] != DBNull.Value &&
                !String.IsNullOrEmpty(wbsStructNode["CodeDefine"].ToString()))
            {
                newWBSRow["Code"] = getSerialCode(projectInfoDt.Rows[0], newWBSRow, wbsStructNode, wbsDt);
            }
            else
            {
                newWBSRow["Code"] = "";
            }
            if (newWBSRow["WBSType"].ToString() == WBSNodeType.Major.ToString() || newWBSRow["WBSType"].ToString() == WBSNodeType.Phase.ToString())
            {
                newWBSRow["WBSValue"] = sourceNode.GetValue("WBSValue");
            }
            else
            {
                if (!String.IsNullOrEmpty(newWBSRow["Code"].ToString()))
                {
                    newWBSRow["WBSValue"] = newWBSRow["Code"];
                }
                else
                {
                    newWBSRow["WBSValue"] = newWBSRow["Name"];
                }
            }
            newWBSRow["Level"] = newWBSRow["FullID"].ToString().Split('.').Length;
            newWBSRow["State"] = ProjectCommoneState.Plan.ToString();
            newWBSRow["SourceNodeID"] = sourceNode.GetValue("ID");
            if (newWBSRow["SortIndex"] == DBNull.Value || Convert.ToDouble(newWBSRow["SortIndex"]) <= 0)
            {
                //child.SortIndex = child.Level * 1000 + this.Children.Count * 10;
                newWBSRow["SortIndex"] = newWBSRow["FullID"].ToString().Split('.').Length * 1000
                    + wbsDt.Select("ParentID = '" + parentNode.GetValue("ID") + "'").Length * 10;
            }
            wbsDt.Rows.Add(newWBSRow);
            setWBSCodes(wbsDt, newWBSRow);
            if (tmpWBSDt != null)
            {
                tmpWBSDt.ImportRow(newWBSRow);
            }
            if (newWBSRow["WBSType"].ToString() == WBSNodeType.Work.ToString())
            {
                var newTask = taskDt.NewRow();
                newTask["ID"] = FormulaHelper.CreateGuid();
                newTask["ProjectInfoID"] = newWBSRow["ProjectInfoID"];
                newTask["WBSID"] = newWBSRow["ID"];
                newTask["WBSFullID"] = newWBSRow["FullID"];
                newTask["SubProjectCode"] = newWBSRow["SubProjectCode"];
                newTask["MajorValue"] = newWBSRow["MajorCode"];
                newTask["PhaseValue"] = newWBSRow["PhaseCode"];
                newTask["AreaCode"] = newWBSRow["AreaCode"];
                newTask["DeviceCode"] = newWBSRow["DeviceCode"];
                newTask["Name"] = newWBSRow["Name"];
                newTask["Code"] = newWBSRow["Code"];
                newTask["State"] = ProjectCommoneState.Plan.ToString();
                newTask["CreateUserID"] = FormulaHelper.CreateGuid();
                newTask["CreateUser"] = FormulaHelper.CreateGuid();
                newTask["CreateDate"] = DateTime.Now;
                taskDt.Rows.Add(newTask);
            }
        }

        string getSerialCode(DataRow projectRow, DataRow wbsNode, DataRow nodeDefine, DataTable wbsDt)
        {
            var result = string.Empty;
            var ParentNode = wbsDt.Rows.Find(wbsNode["ParentID"].ToString());
            if (nodeDefine != null && nodeDefine["CodeDefine"] != null && nodeDefine["CodeDefine"] != DBNull.Value
                && !String.IsNullOrEmpty(nodeDefine["CodeDefine"].ToString()))
            {
                int number = 1;
                if (wbsDt.AsEnumerable().Count(c => c["ParentID"].ToString() == wbsNode["ParentID"].ToString()) > 0)//("ParentID='" + wbsNode["ParentID"].ToString() + "'").Length > 0)
                {
                    var maxCodeIndex = wbsDt.Compute("Max(CodeIndex)", "ParentID='" + wbsNode["ParentID"].ToString() + "'"); //wbsDt.AsEnumerable().Where(c => c["ParentID"].ToString() == wbsNode["ParentID"].ToString()).Max(c => c["CodeIndex"]); 
                    if (maxCodeIndex != null && maxCodeIndex != DBNull.Value)
                    {
                        number = Convert.ToInt32(maxCodeIndex) + 1;
                    }
                    wbsNode["CodeIndex"] = number;
                }
                Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
                result = reg.Replace(nodeDefine["CodeDefine"].ToString(), (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');
                    if (value.IndexOf(".") > 0)
                    {
                        var array = value.Split('.');
                        if (array[0].ToLower() == "parent" && ParentNode != null && ParentNode[array[1]] != null && ParentNode[array[1]] != DBNull.Value)
                        {
                            return ParentNode[array[1]].ToString();
                        }
                        else if (array[0].ToLower() == "project" && projectRow != null
                            && projectRow[array[1]] != null && projectRow[array[1]] != DBNull.Value)
                        {
                            return projectRow[array[1]].ToString();
                        }
                    }
                    else if (wbsNode.Table.Columns.Contains(value))
                    {
                        if (wbsNode[value] == null || wbsNode[value] == DBNull.Value)
                            return String.Empty;
                        else
                        {
                            return wbsNode[value].ToString();
                        }
                    }
                    if (value.Replace('N', ' ').Trim() == "") //顺序号
                        return number.ToString("D" + value.Length);
                    switch (value)
                    {
                        case "yyyy":
                        case "YYYY":
                            return DateTime.Now.ToString("yyyy");
                        case "yy":
                        case "YY":
                            return DateTime.Now.ToString("yy");
                        case "mm":
                        case "MM":
                            return DateTime.Now.ToString("MM");
                        case "dd":
                        case "DD":
                            return DateTime.Now.ToString("dd");
                    }
                    return m.Value;
                });
            }
            return result;
        }

        void DicFillDataRow(DataRow row, Dictionary<string, object> dic)
        {
            foreach (var key in dic.Keys)
            {
                if (row.Table.Columns.Contains(key) && dic[key] != null)
                {
                    row[key] = dic[key];
                }
            }
        }

        public void ImportPackageNodes(List<Dictionary<string, object>> targetNodes, List<Dictionary<string, object>> sourceNodes)
        {
            if (targetNodes.Count == 0) return;
            string sql = @"select S_T_WBSStructInfo.* from S_T_WBSStructInfo
left join S_T_ProjectMode on S_T_WBSStructInfo.ModeID=S_T_ProjectMode.ID where ModeCode='{0}'";
            var projectInfoDt = this.instanceSH.ExecuteDataTable(String.Format("SELECT * FROM S_I_PROJECTINFO WHERE ID='{0}'", targetNodes.FirstOrDefault().GetValue("ProjectInfoID")));
            if (projectInfoDt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法导入WBS节点");
            }
            var wbsStructDefineDt = configDB.ExecuteDataTable(String.Format(sql, projectInfoDt.Rows[0]["ModeCode"]));
            var wbsDt = this.instanceSH.ExecuteDataTable("SELECT *,'True' as IsExist,ID as SourceNodeID FROM S_W_WBS WHERE ProjectInfoID='" + targetNodes.FirstOrDefault().GetValue("ProjectInfoID") + "' ");
            var taskDt = this.instanceSH.ExecuteDataTable("SELECT * FROM S_W_TaskWork WHERE 1<>1 ");

            //var tmplWBSDt = wbsDt.Clone();
            //tmplWBSDt.PrimaryKey = new DataColumn[] { tmplWBSDt.Columns["SourceNodeID"] };
            wbsDt.PrimaryKey = new DataColumn[] { wbsDt.Columns["ID"] };

            foreach (var targetNode in targetNodes)
            {
                var targetDefineNode = wbsStructDefineDt.AsEnumerable().Where(c => c["Code"].ToString() == targetNode.GetValue("WBSStructCode")).FirstOrDefault();
                if (targetDefineNode == null)
                    throw new Formula.Exceptions.BusinessValidationException("WBS定义错误，无法导入WBS节点");
                var parentNode = wbsDt.Rows.Find(targetNode.GetValue("VirtualID"));  
                foreach (var sourceNode in sourceNodes)
                {
                    var parentStructNode = wbsStructDefineDt.AsEnumerable().FirstOrDefault(c => c["Code"].ToString() == targetNode["VirtualWBSType"].ToString());
                        if (parentStructNode != null && parentStructNode["ChildCode"] != null && parentStructNode["ChildCode"] != DBNull.Value
                            && parentStructNode["ChildCode"].ToString().Split(',').Contains(sourceNode.GetValue("WBSType")))
                        {
                            var wbsStructNode = wbsStructDefineDt.AsEnumerable().FirstOrDefault(c => c["Code"].ToString() == sourceNode.GetValue("WBSType"));
                            if (wbsStructNode == null)
                            {
                                continue;
                            }
                            AddPackageWBSRow(wbsDt, taskDt, sourceNode, FormulaHelper.DataRowToDic(parentNode), wbsStructNode, projectInfoDt);
                        }
                }
            }
            foreach (var item in wbsDt.Select("IsExist='True'"))
            {
                wbsDt.Rows.Remove(item);
            }
            if (wbsDt.Rows.Count > 0)
            {
                wbsDt.PrimaryKey = null;
                wbsDt.Columns.Remove("SourceNodeID");
                wbsDt.Columns.Remove("IsExist");
                this.instanceSH.BulkInsertToDB(wbsDt, "S_W_WBS");
            }
            if (taskDt.Rows.Count > 0)
            {
                this.instanceSH.BulkInsertToDB(taskDt, "S_W_TaskWork");
            }
        }

        void AddPackageWBSRow(DataTable wbsDt, DataTable taskDt, Dictionary<string, object> sourceNode, Dictionary<string, object> parentNode,
            DataRow wbsStructNode, DataTable projectInfoDt)
        {
            var newWBSRow = wbsDt.NewRow();
            DicFillDataRow(newWBSRow, sourceNode);
            newWBSRow["ProjectInfoID"] = parentNode.GetValue("ProjectInfoID");
            newWBSRow["ID"] = FormulaHelper.CreateGuid();
            newWBSRow["WBSStructCode"] = sourceNode.GetValue("WBSType");
            newWBSRow["ChargeUserID"] = "";
            newWBSRow["ChargeUserName"] = "";
            newWBSRow["FullID"] = parentNode.GetValue("FullID") + "." + newWBSRow["ID"].ToString();
            newWBSRow["ParentID"] = parentNode.GetValue("ID");
            newWBSRow["SourceNodeID"] = sourceNode.GetValue("ID");
            newWBSRow["Name"] = sourceNode.GetValue("Name");
            if (wbsStructNode["CodeDefine"] != null && wbsStructNode["CodeDefine"] != DBNull.Value &&
                !String.IsNullOrEmpty(wbsStructNode["CodeDefine"].ToString()))
            {
                newWBSRow["Code"] = getSerialCode(projectInfoDt.Rows[0], newWBSRow, wbsStructNode, wbsDt);
            }
            else
            {
                newWBSRow["Code"] = "";
            }
            if (!String.IsNullOrEmpty(newWBSRow["Code"].ToString()))
            {
                newWBSRow["WBSValue"] = newWBSRow["Code"];
            }
            else
            {
                newWBSRow["WBSValue"] = newWBSRow["Name"];
            }
            newWBSRow["Level"] = newWBSRow["FullID"].ToString().Split('.').Length;
            newWBSRow["State"] = ProjectCommoneState.Plan.ToString();
            newWBSRow["SourceNodeID"] = sourceNode.GetValue("ID");
            if (newWBSRow["SortIndex"] == DBNull.Value || Convert.ToDouble(newWBSRow["SortIndex"]) <= 0)
            {
                //child.SortIndex = child.Level * 1000 + this.Children.Count * 10;
                newWBSRow["SortIndex"] = newWBSRow["FullID"].ToString().Split('.').Length * 1000
                    + wbsDt.Select("ParentID = '" + parentNode.GetValue("ID") + "'").Length * 10;
            }
            wbsDt.Rows.Add(newWBSRow);
            setWBSCodes(wbsDt, newWBSRow);
            if (newWBSRow["WBSType"].ToString() == WBSNodeType.Work.ToString())
            {
                var newTask = taskDt.NewRow();
                newTask["ID"] = FormulaHelper.CreateGuid();
                newTask["ProjectInfoID"] = newWBSRow["ProjectInfoID"];
                newTask["WBSID"] = newWBSRow["ID"];
                newTask["WBSFullID"] = newWBSRow["FullID"];
                newTask["SubProjectCode"] = newWBSRow["SubProjectCode"];
                newTask["MajorValue"] = newWBSRow["MajorCode"];
                newTask["PhaseValue"] = newWBSRow["PhaseCode"];
                newTask["AreaCode"] = newWBSRow["AreaCode"];
                newTask["DeviceCode"] = newWBSRow["DeviceCode"];
                newTask["Name"] = newWBSRow["Name"];
                newTask["Code"] = newWBSRow["Code"];
                newTask["State"] = ProjectCommoneState.Plan.ToString();
                newTask["CreateUserID"] = FormulaHelper.CreateGuid();
                newTask["CreateUser"] = FormulaHelper.CreateGuid();
                newTask["CreateDate"] = DateTime.Now;
                taskDt.Rows.Add(newTask);
            }
        }

        void setWBSCodes(DataTable wbsDt, DataRow currentRow)
        {
            //获取所有上级节点，填充WBS类别属性字段
            var ancesters = wbsDt.AsEnumerable().Where(c => currentRow["FullID"].ToString().StartsWith(c["FullID"].ToString())).OrderBy(c => c["SortIndex"]).ToList();
            foreach (DataRow row in ancesters)
            {
                var fieldName = row["WBSType"].ToString() + "Code";
                if (row.Table.Columns.Contains(fieldName))
                    currentRow[fieldName] = row["WBSValue"];
            }
        }

        public List<Dictionary<string, object>> CreateWBSTreeWithMajorView(string projectInfoID, bool includeWork = false, string majorCode = "", string UserID = "")
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var sql = "";
            if (!String.IsNullOrEmpty(UserID))
            {
                sql = String.Format(@"select WBSInfo.*,UserCount,ID as VirtualID,
case when UserCount is null or UserCount=0 then 'False' else 'True' end as HasAuth
 from (select * from S_W_WBS with(nolock)
where ProjectInfoID='{0}') WBSInfo
left join (select count(ID) as UserCount,WBSID from S_W_RBS with(nolock)
where ProjectInfoID='{0}' and UserID='{1}' group by WBSID) RBSInfo 
on WBSInfo.ID=RBSInfo.WBSID  where 1=1 ", projectInfoID, UserID);
            }
            else
            {
                sql = String.Format(@"select ID as VirtualID,*,'True' as HasAuth from S_W_WBS with(nolock)
where ProjectInfoID='{0}' ", projectInfoID);
            }
            if (!includeWork)
                sql += " and WBSType!='" + WBSNodeType.Work.ToString() + "'";
            sql += " order by sortindex";
            var wbsDt = db.ExecuteDataTable(sql);
            var rootNode = wbsDt.Select("WBSType='" + WBSNodeType.Project.ToString() + "'").FirstOrDefault();
            if (rootNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定项目的根节点，无法展示WBS树");
            }
            var rootDic = FormulaHelper.DataRowToDic(rootNode);
            result.Add(rootDic);
            var majorList = wbsDt.AsEnumerable().Where(c => c["WBSType"].ToString() == WBSNodeType.Major.ToString()
                && c["WBSValue"].ToString() == majorCode).Select(c => new { WBSValue = c["WBSValue"].ToString(), Name = c["Name"].ToString() }).Distinct().ToList();

            if (!String.IsNullOrEmpty(majorCode))
            {
                majorList = majorList.Where(c => majorCode.Contains(c.WBSValue)).ToList();
            }
            foreach (var major in majorList)
            {
                var majorDic = new Dictionary<string, object>();
                majorDic.SetValue("Name", major.Name);
                majorDic.SetValue("WBSValue", major.WBSValue);
                majorDic.SetValue("WBSType", WBSNodeType.Major.ToString());
                majorDic.SetValue("Code", major.WBSValue);
                majorDic.SetValue("VirtualID", FormulaHelper.CreateGuid());
                majorDic.SetValue("ParentID", rootNode["ID"]);
                result.Add(majorDic);
                var majorNodes = wbsDt.Select(String.Format("WBSType='{0}' and WBSValue='{1}'", WBSNodeType.Major.ToString(), major.WBSValue));
                foreach (DataRow majorNode in majorNodes)
                {
                    var majorAcestors = wbsDt.AsEnumerable().Where(c => majorNode["FullID"].ToString().StartsWith(c["FullID"].ToString()) && c["WBSType"].ToString() != "Project"
                           && c["ID"].ToString() != majorNode["ID"].ToString()).OrderBy(c => c["SortIndex"]).ToList();

                    foreach (var acestor in majorAcestors)
                    {

                        if (acestor["ParentID"].ToString() == rootNode["ID"].ToString() && acestor["ID"].ToString() != majorNode["ParentID"].ToString())
                        {
                            var dic = FormulaHelper.DataRowToDic(acestor);
                            dic.SetValue("ParentID", majorDic.GetValue("VirtualID"));
                            if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                result.Add(dic);
                        }
                        else if (acestor["ID"].ToString() == majorNode["ParentID"].ToString() && acestor["ParentID"].ToString() != rootNode["ID"].ToString())
                        {
                            var dic = FormulaHelper.DataRowToDic(acestor);
                            dic.SetValue("VirtualID", majorNode["ID"]);
                            dic.SetValue("ID", majorNode["ID"]);
                            dic.SetValue("FullID", majorNode["FullID"]);
                            dic.SetValue("HasAuth", majorNode["HasAuth"]);
                            if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                result.Add(dic);
                        }
                        else if (acestor["ParentID"].ToString() == rootNode["ID"].ToString() && acestor["ID"].ToString() == majorNode["ParentID"].ToString())
                        {
                            var dic = FormulaHelper.DataRowToDic(acestor);
                            dic.SetValue("VirtualID", majorNode["ID"]);
                            dic.SetValue("ID", majorNode["ID"]);
                            dic.SetValue("FullID", majorNode["FullID"]);
                            dic.SetValue("HasAuth", majorNode["HasAuth"]);
                            dic.SetValue("ParentID", majorDic.GetValue("VirtualID"));
                            if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                result.Add(dic);
                        }
                    }
                    var allChildrenNodes = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(majorNode["FullID"].ToString())
                        && c["ID"].ToString() != majorNode["ID"].ToString()).OrderBy(c => c["SortIndex"]).ToList();
                    foreach (DataRow item in allChildrenNodes)
                    {
                        var dic = FormulaHelper.DataRowToDic(item);
                        result.Add(dic);
                    }
                }
            }
            return result;
        }

        public List<Dictionary<string, object>> CreateWBSTree(string projectInfoID, string viewNodeType, bool includeWork = false, string majorCode = "", string UserID = "", bool filterAuthNode = false, bool includeCoop = true)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var projectInfo = instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            #region 查询出WBS结果数据
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var sql = "";
            if (!String.IsNullOrEmpty(UserID))
            {
                sql = String.Format(@"select WBSInfo.*,UserCount,ID as VirtualID,WBSType as VirtualWBSType,
case when UserCount is null or UserCount=0 then 'False' else 'True' end as HasAuth,FullID as VirtualFullID
 from (select * from S_W_WBS with(nolock)
where ProjectInfoID='{0}') WBSInfo
left join (select count(ID) as UserCount,WBSID from S_W_RBS with(nolock)
where ProjectInfoID='{0}' and UserID='{1}' group by WBSID) RBSInfo 
on WBSInfo.ID=RBSInfo.WBSID  where 1=1 ", projectInfoID, UserID);
            }
            else
            {
                sql = String.Format(@"select ID as VirtualID,WBSType as VirtualWBSType,*,'True' as HasAuth,FullID as VirtualFullID
from S_W_WBS with(nolock) where ProjectInfoID='{0}' ", projectInfoID);
            }
            if (!includeWork)
                sql += " and WBSType!='" + WBSNodeType.Work.ToString() + "'";
            if (!includeCoop)
                sql += " and WBSType!='" + WBSNodeType.CooperationPackage.ToString() + "'";
            sql += " order by Level,sortindex";
            var dt = db.ExecuteDataTable(sql);
            var wbsDt = dt.Clone();
            if (filterAuthNode)
            {
                var authNodes = dt.AsEnumerable().Where(c => c["HasAuth"].ToString() == "True").OrderBy(c => c["SortIndex"]).ToList();
                foreach (DataRow authNode in authNodes)
                {
                    wbsDt.ImportRow(authNode);
                    var ancesters = dt.AsEnumerable().Where(c => authNode["FullID"].ToString().StartsWith(c["FullID"].ToString())).OrderBy(c => c["SortIndex"]).ToList();
                    foreach (DataRow row in ancesters)
                    {
                        wbsDt.ImportRow(row);
                    }
                }
            }
            else
            {
                wbsDt = dt;
            }

            var rootNode = dt.Select("WBSType='" + WBSNodeType.Project.ToString() + "'").FirstOrDefault();
            if (rootNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定项目的根节点，无法展示WBS树");
            }
            var rootDic = FormulaHelper.DataRowToDic(rootNode);
            result.Add(rootDic);

            #endregion
            if (viewNodeType == WBSNodeType.Project.ToString() ||
                projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString()).ChildCode.Contains(viewNodeType))
            {
                if (String.IsNullOrEmpty(majorCode))
                {
                    foreach (DataRow row in wbsDt.Rows)
                    {
                        if (!result.Exists(c => c.GetValue("ID") == row["ID"].ToString()))
                        result.Add(FormulaHelper.DataRowToDic(row));
                    }
                }
                else
                {
                    var majorNodes = wbsDt.AsEnumerable().Where(c => c["WBSValue"].ToString() == majorCode).ToList();
                    foreach (var majorNode in majorNodes)
                    {
                        var majorChildNodes = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(majorNode["FullID"].ToString())).ToList();
                        var majorAncestorNodes = wbsDt.AsEnumerable().Where(c => majorNode["FullID"].ToString().StartsWith(c["FullID"].ToString())).ToList();
                        foreach (var node in majorChildNodes)
                        {
                            if (!result.Exists(c => c.GetValue("ID") == node["ID"].ToString()))
                                result.Add(FormulaHelper.DataRowToDic(node));
                        }
                        foreach (var node in majorAncestorNodes)
                        {
                            if (!result.Exists(c => c.GetValue("ID") == node["ID"].ToString()))
                                result.Add(FormulaHelper.DataRowToDic(node));
                        }
                    }
                }
            }
            else
            {                
                var viewNodeQuery = wbsDt.AsEnumerable().Where(c => c["WBSType"].ToString() == viewNodeType);//所有[专业]节点
                if (!String.IsNullOrEmpty(majorCode))
                {
                    viewNodeQuery = viewNodeQuery.Where(c => c["WBSValue"].ToString() == majorCode);//所有[指定专业]节点
                }
                //所有[指定专业]节点去重
                var nodeList = viewNodeQuery.Select(c => new { WBSValue = c["WBSValue"].ToString(), Name = c["Name"].ToString() }).Distinct().ToList();
                foreach (var node in nodeList)
                {
                    var nodeDic = new Dictionary<string, object>();
                    nodeDic.SetValue("Name", node.Name);
                    nodeDic.SetValue("WBSValue", node.WBSValue);
                    nodeDic.SetValue("WBSType", viewNodeType);
                    nodeDic.SetValue("Code", node.WBSValue);
                    nodeDic.SetValue("VirtualID", FormulaHelper.CreateGuid());
                    nodeDic.SetValue("ParentID", rootNode["ID"]);
                    nodeDic.SetValue("VirtualFullID", nodeDic.GetValue("ParentID") + "." + nodeDic.GetValue("VirtualID"));
                    nodeDic.SetValue("SortIndex", rootNode["SortIndex"]);
                    nodeDic.SetValue("Level", Convert.ToInt32(rootNode["Level"]) + 1);
                    result.Add(nodeDic);
                    var viewNodes = wbsDt.AsEnumerable().Where(c => c["WBSType"].ToString() == viewNodeType 
                        && c["WBSValue"].ToString() == node.WBSValue).ToList();
                    foreach (DataRow viewNode in viewNodes)
                    {
                        var acestors = wbsDt.AsEnumerable().Where(c => viewNode["FullID"].ToString().StartsWith(c["FullID"].ToString()) && c["WBSType"].ToString() != "Project"
                               && c["ID"].ToString() != viewNode["ID"].ToString()).ToList();
                        foreach (var acestor in acestors)
                        {
                            //根节点的子节点并且不是viewNode的父节点（项目-区域-装置-专业 中的区域）
                            if (acestor["ParentID"].ToString() == rootNode["ID"].ToString() && acestor["ID"].ToString() != viewNode["ParentID"].ToString())
                            {
                                var dic = FormulaHelper.DataRowToDic(acestor);
                                dic.SetValue("ParentID", nodeDic.GetValue("VirtualID"));
                                dic.SetValue("VirtualFullID", nodeDic.GetValue("VirtualFullID") + "." + dic.GetValue("VirtualID"));
                                if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                    result.Add(dic);
                            }
                            //viewNode的父节点并且不是根节点的子节点（项目-区域-装置-专业 中的装置）
                            else if (acestor["ID"].ToString() == viewNode["ParentID"].ToString() && acestor["ParentID"].ToString() != rootNode["ID"].ToString())
                            {
                                //var dic = FormulaHelper.DataRowToDic(acestor);
                                var dic = FormulaHelper.DataRowToDic(viewNode);
                                dic.SetValue("VirtualID", viewNode["ID"]);
                                dic.SetValue("VirtualWBSType", viewNode["WBSType"]);
                                dic.SetValue("ID", acestor["ID"]);
                                dic.SetValue("Name", acestor["Name"]);
                                dic.SetValue("Code", acestor["Code"]);
                                dic.SetValue("WBSType", acestor["WBSType"]);
                                dic.SetValue("SortIndex", acestor["SortIndex"]);
                                dic.SetValue("ParentID", acestor["ParentID"]);
                                var parentDic = result.FirstOrDefault(a => a.GetValue("VirtualID") == acestor["ParentID"].ToString());
                                if (parentDic != null)
                                    dic.SetValue("VirtualFullID", parentDic.GetValue("VirtualFullID") + "." + viewNode["ID"]);
                                if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                {
                                    result.Add(dic);
                                    var allChildrenNodes = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(viewNode["FullID"].ToString())
                      && c["ID"].ToString() != viewNode["ID"].ToString()).OrderBy(c => c["Level"]).ThenBy(c => c["SortIndex"]).ToList();
                                    foreach (DataRow item in allChildrenNodes)
                                    {
                                        var childDic = FormulaHelper.DataRowToDic(item);
                                        if (item["ParentID"] == viewNode["ID"])
                                            childDic.SetValue("VirtualFullID", dic.GetValue("VirtualFullID") + "." + childDic.GetValue("VirtualID"));
                                        else
                                        {
                                            var _parentDic = result.FirstOrDefault(a => a.GetValue("VirtualID") == childDic.GetValue("ParentID"));
                                            if (_parentDic != null)
                                                childDic.SetValue("VirtualFullID", _parentDic.GetValue("VirtualFullID") + "." + childDic.GetValue("VirtualID"));
                                        }
                                        if (!result.Exists(c => c["VirtualID"] == childDic["VirtualID"]))
                                            result.Add(childDic);
                                    }
                                }
                            }
                            //根节点的子节点并且viewNode的父节点（项目-装置-专业 中的装置、项目-子项-专业 中的子项）
                            else if (acestor["ParentID"].ToString() == rootNode["ID"].ToString() && acestor["ID"].ToString() == viewNode["ParentID"].ToString())
                            {
                                //var dic = FormulaHelper.DataRowToDic(acestor);
                                var dic = FormulaHelper.DataRowToDic(viewNode);
                                dic.SetValue("VirtualID", viewNode["ID"]);
                                dic.SetValue("VirtualWBSType", viewNode["WBSType"]);
                                dic.SetValue("ID", acestor["ID"]);
                                dic.SetValue("Name", acestor["Name"]);
                                dic.SetValue("Code", acestor["Code"]);
                                dic.SetValue("WBSType", acestor["WBSType"]);
                                dic.SetValue("SortIndex", acestor["SortIndex"]);
                                dic.SetValue("ParentID", nodeDic.GetValue("VirtualID"));
                                dic.SetValue("VirtualFullID", nodeDic.GetValue("VirtualFullID") + "." + viewNode["ID"]);
                                if (!result.Exists(c => c["VirtualID"] == dic["VirtualID"]))
                                {
                                    result.Add(dic);
                                    var allChildrenNodes = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(viewNode["FullID"].ToString())
                      && c["ID"].ToString() != viewNode["ID"].ToString()).OrderBy(c=>c["Level"]).ThenBy(c => c["SortIndex"]).ToList();
                                    foreach (DataRow item in allChildrenNodes)
                                    {
                                        var childDic = FormulaHelper.DataRowToDic(item);
                                        if (item["ParentID"] == viewNode["ID"])
                                            childDic.SetValue("VirtualFullID", dic.GetValue("VirtualFullID") + "." + childDic.GetValue("VirtualID"));
                                        else
                                        {
                                            var parentDic = result.FirstOrDefault(a => a.GetValue("VirtualID") == childDic.GetValue("ParentID"));
                                            if (parentDic != null)
                                                childDic.SetValue("VirtualFullID", parentDic.GetValue("VirtualFullID") + "." + childDic.GetValue("VirtualID"));
                                        }
                                        if (!result.Exists(c => c["VirtualID"] == childDic["VirtualID"]))
                                            result.Add(childDic);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result.OrderBy(a => Convert.ToInt32(a["Level"])).ThenBy(a => Convert.ToDouble(a["SortIndex"])).ToList();
        }

        #region 根据卷号册号自动创建卷结构
        public void AutoCreateWBSEntity(string ProjectInfoID)
        {
            var projectInfoDt = this.instanceSH.ExecuteDataTable(String.Format("SELECT * FROM S_I_PROJECTINFO WHERE ID='{0}'", ProjectInfoID));
            if (projectInfoDt.Rows.Count == 0)
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法导入WBS节点");
            var prjFO = new ProjectInfoFO();
            string sql = @"select S_T_WBSStructInfo.* from S_T_WBSStructInfo
left join S_T_ProjectMode on S_T_WBSStructInfo.ModeID=S_T_ProjectMode.ID where ModeCode='{0}'";
            var structList = configDB.ExecuteList<S_T_WBSStructInfo>(String.Format(sql, projectInfoDt.Rows[0]["ModeCode"]));
            var wbsDt = this.instanceSH.ExecuteDataTable("SELECT *,'' as AddState FROM S_W_WBS WHERE ProjectInfoID='" + ProjectInfoID + "' ");
            var taskDt = this.instanceSH.ExecuteDataTable("SELECT * FROM S_W_TaskWork WHERE ProjectInfoID='" + ProjectInfoID + "'  ").AsEnumerable();
            var sqlCommand = new StringBuilder();
            wbsDt.PrimaryKey = new DataColumn[] { wbsDt.Columns["ID"] };

            if (!structList.Any(a => a.Code == WBSNodeType.Entity.ToString() && a.ChildCode.Split(',').Contains(WBSNodeType.Work.ToString())))
                return;//有卷-册结构，才自动生成

            //获得所有册WBS
            var workRows = wbsDt.AsEnumerable().Where(a => a["WBSType"].ToString() == WBSNodeType.Work.ToString());
            foreach (var task in taskDt)
            {
                var workWBS = wbsDt.Rows.Find(task["WBSID"].ToString());
                if (workWBS == null) continue;
                var entityName = task["DossierName"].ToString();
                var entityCode = task["DossierCode"].ToString();
                var parentWBS = wbsDt.Rows.Find(workWBS["ParentID"].ToString());
                if (parentWBS["WBSType"].ToString() != WBSNodeType.Entity.ToString() &&
                    string.IsNullOrEmpty(entityName) && string.IsNullOrEmpty(entityCode))
                    continue;//不需要生成卷，或者移动
                else
                {
                    if (string.IsNullOrEmpty(entityCode)) entityCode = entityName;
                    if ( string.IsNullOrEmpty(entityName) && string.IsNullOrEmpty(entityCode))
                    {
                        #region 移动节点至专业下，专业-册
                        if (parentWBS["WBSType"].ToString() == WBSNodeType.Entity.ToString())
                        {
                            var _parentWBS = wbsDt.Rows.Find(parentWBS["ParentID"].ToString());
                            AppendWorkWBS(workWBS, _parentWBS, sqlCommand);
                        }
                        else
                            AppendWorkWBS(workWBS, parentWBS, sqlCommand);
                        #endregion
                    }
                    else
                    {
                        #region 新增卷节点、移动节点至卷下，卷-册
                        
                        var entityWBS = wbsDt.AsEnumerable().FirstOrDefault(a => a["WBSValue"].ToString() == entityCode
                            && a["WBSType"].ToString() == WBSNodeType.Entity.ToString());
                        if (entityWBS == null)
                        {
                            #region 新增卷节点、移动节点至新增卷节点下

                            //新增卷节点
                            entityWBS = wbsDt.NewRow();
                            entityWBS["ID"] = FormulaHelper.CreateGuid();
                            entityWBS["WBSType"] = WBSNodeType.Entity.ToString();
                            entityWBS["Name"] = entityName;
                            entityWBS["WBSValue"] = entityCode;
                            entityWBS["Code"] = entityCode;
                            if (parentWBS["WBSType"].ToString() == WBSNodeType.Entity.ToString())
                            {
                                var _parentWBS = wbsDt.Rows.Find(parentWBS["ParentID"].ToString());
                                prjFO.AddWBSChildWithAdo(_parentWBS, entityWBS, wbsDt, structList);
                            }
                            else
                                prjFO.AddWBSChildWithAdo(parentWBS, entityWBS, wbsDt, structList);
                            //移动节点至新卷
                            AppendWorkWBS(workWBS, entityWBS, sqlCommand);
                            #endregion
                        }
                        else
                        {
                            #region 移动节点至已有卷下
                            if (entityWBS["ID"].ToString() != parentWBS["ID"].ToString())
                            {
                                AppendWorkWBS(workWBS, entityWBS, sqlCommand);
                            }
                            else if (entityWBS["Name"].ToString() != entityName)
                            {
                                //修改名称
                                sqlCommand.AppendLine(String.Format("update s_w_wbs set Name='{0}' where id='{1}'",
                                    entityName, entityWBS["ID"].ToString()));
                            }
                            #endregion
                        }

                        #endregion
                    }
                }
            }

            //删除没有册的卷
            var entityRows = wbsDt.AsEnumerable().Where(a => a["WBSType"].ToString() == WBSNodeType.Entity.ToString());
            foreach (var entityWBS in entityRows)
            {
                if (!wbsDt.AsEnumerable().Any(a => a["ParentID"].ToString() == entityWBS["ID"].ToString()))
                {
                    sqlCommand.AppendLine(String.Format("delete from s_w_wbs where id='{0}'",
                        entityWBS["ID"].ToString()));
                }
            }

            var newWBSDt = wbsDt.Clone();
            var addesRows = wbsDt.AsEnumerable().Where(c => c["AddState"] != null && c["AddState"] != DBNull.Value && c["AddState"].ToString() == "true").ToList();
            foreach (var item in addesRows)
                newWBSDt.ImportRow(item);
            newWBSDt.Columns.Remove("AddState");
            this.instanceSH.BulkInsertToDB(newWBSDt, "S_W_WBS");
            if (!String.IsNullOrEmpty(sqlCommand.ToString()))
                this.instanceSH.ExecuteNonQuery(sqlCommand.ToString());
        }

        void AppendWorkWBS(DataRow sourceWBS, DataRow targetWBS, StringBuilder sqlCommand)
        {
            //成果.FullID,TaskWork.FullID
            sourceWBS["ParentID"] = targetWBS["ID"].ToString();
            sourceWBS["FullID"] = targetWBS["FullID"].ToString() + "." + sourceWBS["ID"].ToString();
            sqlCommand.AppendLine(String.Format("update S_W_WBS set ParentID='{0}',FullID='{1}' where id='{2}'",
                targetWBS["ID"].ToString(), targetWBS["FullID"].ToString() + "." + sourceWBS["ID"].ToString(), sourceWBS["ID"].ToString()));
            sqlCommand.AppendLine(String.Format("update S_E_Product set WBSFullID='{0}' where WBSID='{1}'",
               targetWBS["FullID"].ToString() + "." + sourceWBS["ID"].ToString(), sourceWBS["ID"].ToString()));
            sqlCommand.AppendLine(String.Format("update S_E_ProductVersion set WBSFullID='{0}' where WBSID='{1}'",
               targetWBS["FullID"].ToString() + "." + sourceWBS["ID"].ToString(), sourceWBS["ID"].ToString()));
            sqlCommand.AppendLine(String.Format("update S_W_TaskWork set WBSFullID='{0}' where WBSID='{1}'",
               targetWBS["FullID"].ToString() + "." + sourceWBS["ID"].ToString(), sourceWBS["ID"].ToString()));
        }
        #endregion

        ///// <summary>
        ///// 根据不同的视图节点，切换WBS树层，获得树层的数据结构
        ///// </summary>
        ///// <param name="projectInfoID">项目ID</param>
        ///// <param name="viewNodeType">查询视角（Major,SubProject,Phase等）</param>
        ///// <param name="includeWork">是否显示工作包，默认不显示</param>
        ///// <returns></returns>
        //public List<Dictionary<string, object>> CreateWBSTree(string projectInfoID, string viewNodeType, bool includeWork = false, string majorCode = "")
        //{
        //    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
        //    var projectInfo = instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
        //    List<S_W_WBS> wbsList;
        //    //如果要显示到工作包级别，则需要查询出所有的工作包节点
        //    //判定是否需要显示工作包节点，如果需要显示，则需要从结构内获取工作包节点的定义来组合SQL语句
        //    //无论是否包括工作包节点，项目节点总是排除在外的，因为项目永远都是根节点
        //    if (includeWork)
        //        wbsList = instanceEnitites.S_W_WBS.Include("S_W_RBS").OrderBy(d => d.SortIndex).ToList();
        //    else
        //        wbsList = instanceEnitites.S_W_WBS.Include("S_W_RBS").Where(d => d.WBSType != WBSNodeType.Work.ToString()).OrderBy(d => d.SortIndex).ToList();

        //    int max = wbsList.Max(d => d.Level);  //找到WBS结构中的最深一层的Level
        //    ////如果是项目视图或项目下一级，暨第二级视图，则直接按WBS数据结构查询数据
        //    if (viewNodeType == WBSNodeType.Project.ToString() ||
        //        projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString()).ChildCode.Contains(viewNodeType))
        //    {
        //        var resultList = new List<S_W_WBS>();
        //        if (!String.IsNullOrEmpty(majorCode))
        //        {
        //            var majorList = wbsList.Where(d => d.WBSValue == majorCode && d.WBSType == WBSNodeType.Major.ToString()).ToList();
        //            foreach (var major in majorList)
        //            {
        //                resultList.Add(major);
        //                var ancestor = wbsList.Where(c => major.FullID.StartsWith(c.FullID) && c.ID != major.ID).ToList();
        //                foreach (var item in ancestor)
        //                {
        //                    if (resultList.Exists(d => d.ID == item.ID)) continue;
        //                    resultList.Add(item);
        //                }
        //                resultList.AddRange(wbsList.Where(d => d.FullID.StartsWith(major.FullID) && d.ID != major.ID).ToList());
        //            }
        //        }
        //        else
        //            resultList.AddRange(wbsList);
        //        foreach (var wbs in resultList)
        //        {
        //            var dic = wbs.ToDic();
        //            dic.SetValue("VirtualID", wbs.ID);
        //            dic.SetValue("RBSLIst", wbs.S_W_RBS.ToList());
        //            result.Add(dic);
        //        }
        //    }
        //    else
        //    {
        //        //根据结构定义，组合树状梯形排序结构的SQL语句
        //        string Fields = string.Empty;
        //        var structList = wbsList.Select(d => new { WBSType = d.WBSType, Level = d.Level }).Distinct().Where(c => c.WBSType != viewNodeType).OrderBy(d => d.Level).ToList();
        //        foreach (var item in structList)
        //        {
        //            Fields += item.WBSType + "Code" + ",";
        //            if (item.Level == 1) Fields += viewNodeType + "Code" + ",";
        //        }
        //        Fields = Fields.TrimEnd(',');
        //        string sql = @"select * from (select distinct {1} from S_W_WBS where ProjectInfoID='{0}' and {2} group by  {1} with rollup) A where ProjectCode is not null  order by {1}";
        //        string condition = viewNodeType + "Code !='' ";
        //        if (!String.IsNullOrEmpty(majorCode))
        //            condition = viewNodeType + "Code like '%" + majorCode + "%' ";
        //        sql = String.Format(sql, projectInfoID, Fields, condition);
        //        //string sql = this.CreateViewTypeSql(projectInfoID, viewNodeType, deepType, structList);
        //        string[] levelField = Fields.Split(','); //需要重新组合的节点类型的WBSNodeType数组集合
        //        var dt = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteDataTable(sql);

        //        //定义父节点集合，循环增加虚拟节点时使用
        //        Dictionary<int, VirtualWBS> parentList = new Dictionary<int, VirtualWBS>();
        //        //增加虚拟根节点，永远都以项目节点作为根节点
        //        var rootVNode = new VirtualWBS(projectInfo.WBSRoot, true);
        //        rootVNode.VirtualLevel = 1;
        //        result.Add(rootVNode.ToDic());

        //        //将项目根节点压入父节点集合中，定义为一层的父节点
        //        parentList[rootVNode.VirtualLevel] = rootVNode;

        //        //如果视图节点不存在则直接返回
        //        if (!wbsList.Exists(d => d.WBSType == viewNodeType))
        //            return result;

        //        //循环梯形结果查询表
        //        for (int i = 1; i < dt.Rows.Count; i++)
        //        {
        //            var row = dt.Rows[i];
        //            //获得当前行在展现时的层次
        //            int level = this.GetLevel(row, levelField);
        //            //当前行所对应的唯一WBS节点
        //            var wbs = this.GetWBS(row, levelField, wbsList);
        //            var vNode = parentList[level - 1].CreateChildNode(wbs, true);
        //            //if (string.IsNullOrEmpty(row[levelField[level - 1]].ToString())) continue;
        //            vNode.Name = row[levelField[level - 1]].ToString();
        //            if (vNode.Name.Split(codeSpliteChar).Length > 1)
        //                vNode.Name = vNode.Name.Split(codeSpliteChar)[1];
        //            vNode.WBSType = levelField[level - 1].Replace("Code", "");
        //            var node = this.GetSimilarityWBS(row, levelField[level - 1], wbsList);
        //            if (node != null)
        //                vNode.Code = node.Code;
        //            result.Add(vNode.ToDic());
        //            parentList[level] = vNode;
        //        }
        //    }
        //    return result;
        //}


        ///// <summary>
        ///// 根据不同的视图节点，切换WBS树层，获得树层的数据结构（返回甘特图数据结构）
        ///// </summary>
        ///// <param name="projectInfoID">项目ID</param>
        ///// <param name="viewNodeType">查询视角（Major,SubProject,Phase等）</param>
        ///// <param name="includeWork">是否显示工作包，默认不显示</param>
        ///// <returns></returns>
        //public List<Dictionary<string, object>> GetGranttWBSTree(string projectInfoID, string viewNodeType, bool includeWork = false)
        //{
        //    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
        //    var projectInfo = instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
        //    List<S_W_WBS> wbsList;
        //    //如果要显示到工作包级别，则需要查询出所有的工作包节点
        //    //判定是否需要显示工作包节点，如果需要显示，则需要从结构内获取工作包节点的定义来组合SQL语句
        //    //无论是否包括工作包节点，项目节点总是排除在外的，因为项目永远都是根节点
        //    if (includeWork)
        //        wbsList = projectInfo.S_W_WBS.Where(c => projectInfo.ProjectMode.S_T_WBSStructInfo.Select(e => e.Code).Contains(c.WBSType)).OrderBy(d => d.SortIndex).ToList();
        //    else
        //        wbsList = projectInfo.S_W_WBS.Where(d => d.WBSType != WBSNodeType.Work.ToString()).Where(c => projectInfo.ProjectMode.S_T_WBSStructInfo.Select(e => e.Code).Contains(c.WBSType)).OrderBy(d => d.SortIndex).ToList();
        //    //int max = wbsList.Max(d => d.Level);  //找到WBS结构中的最深一层的Level
        //    //var deepType = wbsList.FirstOrDefault(d => d.Level == max).WBSType;  //获取最底层WBS节点的属性，后续组合梯形SQL语句时需要使用
        //    //if (deepType == WBSNodeType.Work.ToString())
        //    //    deepType = wbsList.FirstOrDefault(d => d.Level == max - 1).WBSType;
        //    //如果是项目视图或项目下一级，暨第二级视图，则直接按WBS数据结构查询数据
        //    if (viewNodeType == WBSNodeType.Project.ToString() || projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString()).ChildCode.Contains(viewNodeType))
        //    {
        //        foreach (var item in wbsList)
        //        {
        //            var dic = FormulaHelper.ModelToDic<S_W_WBS>(item);
        //            dic.SetValue("UID", item.ID);
        //            dic.SetValue("ParentTaskUID", item.ParentID);
        //            dic.SetValue("WBSID", item.ID);
        //            dic.SetValue("Start", item.PlanStartDate);
        //            dic.SetValue("Finish", item.PlanEndDate);
        //            result.Add(dic);
        //        }
        //    }
        //    else
        //    {
        //        //根据结构定义，组合树状梯形排序结构的SQL语句
        //        string Fields = string.Empty;
        //        var structList = wbsList.Select(d => new { WBSType = d.WBSType, Level = d.Level }).Distinct().Where(c => c.WBSType != viewNodeType).OrderBy(d => d.Level).ToList();
        //        foreach (var item in structList)
        //        {
        //            Fields += item.WBSType + "Code" + ",";
        //            if (item.Level == 1) Fields += viewNodeType + "Code" + ",";
        //        }
        //        Fields = Fields.TrimEnd(',');
        //        string sql = @"select * from (select distinct {1} from S_W_WBS where ProjectInfoID='{0}' and {2}!='' group by  {1} with rollup) A where ProjectCode is not null  order by {1}";
        //        sql = String.Format(sql, projectInfoID, Fields, viewNodeType + "Code");
        //        string[] levelField = Fields.Split(','); //需要重新组合的节点类型的WBSNodeType数组集合
        //        var dt = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteDataTable(sql);

        //        //定义父节点集合，循环增加虚拟节点时使用
        //        Dictionary<int, VirtualWBS> parentList = new Dictionary<int, VirtualWBS>();
        //        //增加虚拟根节点，永远都以项目节点作为根节点
        //        var rootVNode = new VirtualWBS(projectInfo.WBSRoot, true);
        //        rootVNode.VirtualLevel = 1;
        //        var rootDic = rootVNode.ToDic();
        //        rootDic.SetValue("UID", rootVNode.VirtualID);
        //        rootDic.SetValue("ParentTaskUID", rootVNode.ParentID);
        //        rootDic.SetValue("WBSID", rootVNode.ID);
        //        rootDic.SetValue("Start", rootVNode.PlanStartDate);
        //        rootDic.SetValue("Finish", rootVNode.PlanEndDate);
        //        result.Add(rootVNode.ToDic());

        //        //如果视图节点不存在则直接返回
        //        if (!wbsList.Exists(d => d.WBSType == viewNodeType))
        //            return result;

        //        //将项目根节点压入父节点集合中，定义为一层的父节点
        //        parentList[rootVNode.VirtualLevel] = rootVNode;

        //        //循环梯形结果查询表
        //        for (int i = 1; i < dt.Rows.Count; i++)
        //        {
        //            var row = dt.Rows[i];
        //            //获得当前行在展现时的层次
        //            int level = this.GetLevel(row, levelField);
        //            //当前行所对应的唯一WBS节点
        //            var wbs = this.GetWBS(row, levelField, wbsList);
        //            var vNode = parentList[level - 1].CreateChildNode(wbs, true);
        //            vNode.Name = row[levelField[level - 1]].ToString();
        //            if (vNode.Name.Split(codeSpliteChar).Length > 1)
        //                vNode.Name = vNode.Name.Split(codeSpliteChar)[1];
        //            vNode.WBSType = levelField[level - 1].Replace("Code", "");
        //            var node = this.GetSimilarityWBS(row, levelField[level - 1], wbsList);
        //            if (node != null)
        //                vNode.Code = node.Code;
        //            var dic = vNode.ToDic();
        //            dic.SetValue("UID", vNode.VirtualID);
        //            dic.SetValue("WBSID", vNode.ID);
        //            dic.SetValue("ParentTaskUID", vNode.ParentID);
        //            dic.SetValue("Start", vNode.PlanStartDate);
        //            dic.SetValue("Finish", vNode.PlanEndDate);
        //            result.Add(dic);
        //            parentList[level] = vNode;
        //        }
        //    }
        //    return result;

        //}

        #region 私有方法

        private void FillGantTreeList(Dictionary<string, object> parentNode, string parentID,
            List<Dictionary<string, object>> result, List<S_W_WBS> wbsList)
        {
            var children = wbsList.Where(d => d.ParentID == parentID).ToList();
            foreach (var child in children)
            {
                var dic = FormulaHelper.ModelToDic<S_W_WBS>(child);
                dic.SetValue("UID", child.ID);
                dic.SetValue("ParentTaskUID", child.ParentID);
                dic.SetValue("Start", child.PlanStartDate);
                dic.SetValue("Finish", child.PlanEndDate);
            }
        }

        #endregion

    }
}
