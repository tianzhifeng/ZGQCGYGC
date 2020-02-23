using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using System.Data;
using Config.Logic;

namespace Project.Logic.Domain
{
    public partial class S_W_WBS_Version
    {
        public void Push()
        {
            var wbsIDList = this.S_W_WBS_Version_Node.Select(c => c.WBSID).ToList();
            var wbsIDs = string.Empty;
            foreach (var item in wbsIDList)
            {
                wbsIDs += item + ",";
            }
            string sql = String.Format(@"update S_W_WBS set State='{0}' where ID in ('{1}')", ProjectCommoneState.Execute.ToString(),
                wbsIDs.TrimEnd(',').Replace(",", "','"));
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            db.ExecuteNonQuery(sql);

            this.S_I_ProjectInfo.SetMajorSpace();
            this.S_I_ProjectInfo.ResetOBSUserFromRBS();
        }


        public static void UpgradeVersion(Dictionary<string, object> versionInfo)
        {
            var projectInfoID = versionInfo.GetValue("ProjectInfoID");
            if (String.IsNullOrEmpty(projectInfoID))
            {
                throw new Formula.Exceptions.BusinessValidationException("计划必须归属一个项目对象，操作失败");
            }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var resultDt = db.ExecuteDataTable("SELECT * FROM S_W_WBS_Version_Node WITH(NOLOCK) WHERE 1!=1");
            var wbsDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_W_WBS WITH(NOLOCK)  WHERE PROJECTINFOID='{0}'", projectInfoID));
            wbsDt.PrimaryKey = new DataColumn[] { wbsDt.Columns["ID"] };
            var rbsDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_W_RBS WITH(NOLOCK)  WHERE PROJECTINFOID='{0}'", projectInfoID)).AsEnumerable();
            var lastVersionDt = db.ExecuteDataTable(String.Format("select top 1 * from S_W_WBS_Version  where ProjectInfoID='{0}' and FlowPhase='End' order by ID desc", projectInfoID));
            if (lastVersionDt.Rows.Count == 0)
            {
                #region 当是第一个版本时，则默认是所有节点都新增，根据WBS数据来创建版本记录
                foreach (DataRow wbsNode in wbsDt.Rows)
                {
                    var versionRow = resultDt.NewRow();
                    versionRow["ID"] = FormulaHelper.CreateGuid();
                    foreach (DataColumn column in wbsDt.Columns)
                    {
                        if (!resultDt.Columns.Contains(column.ColumnName))
                        {
                            continue;
                        }
                        versionRow[column.ColumnName] = wbsNode[column.ColumnName];
                    }
                    versionRow["VersionID"] = versionInfo.GetValue("ID");
                    versionRow["WBSID"] = wbsNode["ID"].ToString();
                    versionRow["WBSParentID"] = wbsNode["ParentID"].ToString();
                    versionRow["ModifyState"] = "Add";
                    var rbsInfo = rbsDt.Where(c => c["WBSID"].ToString() == wbsNode["ID"].ToString()).ToList();
                    var rbsList = new List<Dictionary<string, object>>();
                    foreach (DataRow rbs in rbsInfo)
                    {
                        rbsList.Add(FormulaHelper.DataRowToDic(rbs));
                    }
                    versionRow["RBSInfo"] = JsonHelper.ToJson(rbsList);
                    resultDt.Rows.Add(versionRow);
                }
                #endregion
            }
            else
            {
                #region 当之前已有版本记录时候，需要用当前的WBS数据来和之前的版本做数据对比，已区分出新增，修改或是删除
                var lastNodeDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_W_WBS_Version_Node WITH(NOLOCK) WHERE VERSIONID='{0}' and ModifyState!='Remove'", lastVersionDt.Rows[0]["ID"])); // lastVersion.S_W_WBS_Version_Node.Where(c => c.ModifyState != "Remove").ToList();
                foreach (DataRow lastVersionNode in lastNodeDt.Rows)
                {
                    var versionRow = resultDt.NewRow();
                    foreach (DataColumn column in lastNodeDt.Columns)
                    {
                        if (!resultDt.Columns.Contains(column.ColumnName))
                        {
                            continue;
                        }
                        versionRow[column.ColumnName] = lastVersionNode[column.ColumnName];
                    }
                    versionRow["VersionID"] = versionInfo.GetValue("ID");
                    var rbsInfo = rbsDt.Where(c => c["WBSID"].ToString() == lastVersionNode["WBSID"].ToString()).ToList();
                    var rbsList = new List<Dictionary<string, object>>();
                    foreach (DataRow rbs in rbsInfo)
                    {
                        rbsList.Add(FormulaHelper.DataRowToDic(rbs));
                    }
                    versionRow["RBSInfo"] = JsonHelper.ToJson(rbsList);
                    var wbsNode = wbsDt.Rows.Find(versionRow["WBSID"]); 
                    if (wbsNode == null)
                    {
                        versionRow["ModifyState"] = "Remove";
                    }
                    else if (!CompareDataRow(wbsNode, versionRow))
                    {
                        versionRow["ModifyState"] = "Modify";
                        foreach (DataColumn column in wbsNode.Table.Columns)
                        {
                            if (!resultDt.Columns.Contains(column.ColumnName))
                            {
                                continue;
                            }
                            versionRow[column.ColumnName] = wbsNode[column.ColumnName];
                        }
                    }
                    else
                    {
                        versionRow["ModifyState"] = "Normal";
                    }
                    if (lastVersionNode["RBSInfo"].ToString() != versionRow["RBSInfo"].ToString()
                        && (lastVersionNode["ModifyState"].ToString() != "Remove"))
                    {
                        versionRow["ModifyState"] = "Modify";
                    }
                    versionRow["ID"] = FormulaHelper.CreateGuid();
                    resultDt.Rows.Add(versionRow);
                }

                #region 加入新增的节点
                var existWBSID = resultDt.AsEnumerable().Select(c => c["WBSID"].ToString()).ToList();
                var newWBSList = wbsDt.AsEnumerable().Where(c => !existWBSID.Contains(c["ID"].ToString())).ToList();
                foreach (var newWBSNode in newWBSList)
                {
                    var newVersionRow = resultDt.NewRow();
                    foreach (DataColumn column in newWBSNode.Table.Columns)
                    {
                        if (!resultDt.Columns.Contains(column.ColumnName))
                        {
                            continue;
                        }
                        newVersionRow[column.ColumnName] = newWBSNode[column.ColumnName];
                    }
                    newVersionRow["VersionID"] = versionInfo.GetValue("ID");
                    newVersionRow["WBSID"] = newWBSNode["ID"];
                    newVersionRow["WBSParentID"] = newWBSNode["ParentID"];
                    newVersionRow["ModifyState"] = "Add";
                    var rbsInfo = rbsDt.Where(c => c["WBSID"].ToString() == newWBSNode["ID"].ToString()).ToList();
                    var rbsList = new List<Dictionary<string, object>>();
                    foreach (DataRow rbs in rbsInfo)
                    {
                        rbsList.Add(FormulaHelper.DataRowToDic(rbs));
                    }
                    newVersionRow["RBSInfo"] = JsonHelper.ToJson(rbsList);
                    newVersionRow["ID"] = FormulaHelper.CreateGuid();
                    resultDt.Rows.Add(newVersionRow);
                #endregion

                }
                #endregion
            }
            db.BulkInsertToDB(resultDt, "S_W_WBS_Version_Node");
        }

        public void UpgradeFromWBS()
        {
            if (this.S_I_ProjectInfo == null) throw new Formula.Exceptions.BusinessValidationException("计划必须归属一个项目对象，操作失败");
            var lastVersion = this.S_I_ProjectInfo.S_W_WBS_Version.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
            var Db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var entities = this.GetDbContext<ProjectEntities>();
            var wbsList = entities.S_W_WBS.Where(c => c.ProjectInfoID == this.ProjectInfoID).ToList();
            var rbsList = entities.S_W_RBS.Where(c => c.ProjectInfoID == this.ProjectInfoID).ToList();
            if (lastVersion == null)
            {
                foreach (var wbsNode in wbsList)
                {
                    var wbsDic = FormulaHelper.ModelToDic<S_W_WBS>(wbsNode);
                    var versionNode = new S_W_WBS_Version_Node();
                    FormulaHelper.UpdateEntity<S_W_WBS_Version_Node>(versionNode, wbsDic);
                    versionNode.ID = FormulaHelper.CreateGuid();
                    versionNode.VersionID = this.ID;
                    versionNode.S_W_WBS_Version = this;
                    versionNode.WBSID = wbsDic.GetValue("ID");
                    versionNode.WBSParentID = wbsDic.GetValue("ParentID");
                    versionNode.ModifyState = "Add";
                    var rbsInfo = rbsList.Where(c => c.WBSID == wbsNode.ID).ToList();
                    versionNode.RBSInfo = JsonHelper.ToJson(rbsInfo);
                    this.S_W_WBS_Version_Node.Add(versionNode);
                }
            }
            else
            {
                var lastNodeList = lastVersion.S_W_WBS_Version_Node.Where(c => c.ModifyState != "Remove").ToList();
                foreach (var lastVersionNode in lastNodeList)
                {
                    var versionNode = lastVersionNode.Clone<S_W_WBS_Version_Node>();
                    versionNode.VersionID = this.ID;
                    versionNode.S_W_WBS_Version = this;
                    var rbsInfo = rbsList.Where(c => c.WBSID == lastVersionNode.WBSID).ToList();
                    versionNode.RBSInfo = JsonHelper.ToJson(rbsInfo);
                    var wbsNode = wbsList.FirstOrDefault(c => c.ID == versionNode.WBSID);
                    if (wbsNode == null)
                    {
                        versionNode.ModifyState = "Remove";
                    }
                    else if (!CompareObject<S_W_WBS, S_W_WBS_Version_Node>(wbsNode, versionNode))
                    {
                        versionNode.ModifyState = "Modify";
                        var wbsDic = FormulaHelper.ModelToDic<S_W_WBS>(wbsNode);
                        FormulaHelper.UpdateEntity<S_W_WBS_Version_Node>(versionNode, wbsDic);
                    }
                    else
                    {
                        versionNode.ModifyState = "Normal";
                    }

                    if (lastVersionNode.RBSInfo != versionNode.RBSInfo && lastVersionNode.ModifyState == "Normal")
                    {
                        versionNode.ModifyState = "Modify";
                    }
                    this.S_W_WBS_Version_Node.Add(versionNode);
                }

                var existWBSID = this.S_W_WBS_Version_Node.Select(c => c.WBSID).ToList();
                var newWBSList = wbsList.Where(c => !existWBSID.Contains(c.ID)).ToList();
                foreach (var item in newWBSList)
                {
                    var versionNode = new S_W_WBS_Version_Node();
                    var dic = FormulaHelper.ModelToDic<S_W_WBS>(item);
                    FormulaHelper.UpdateEntity<S_W_WBS_Version_Node>(versionNode, dic);
                    versionNode.ID = FormulaHelper.CreateGuid();
                    versionNode.VersionID = this.ID;
                    versionNode.S_W_WBS_Version = this;
                    versionNode.WBSID = item.ID;
                    versionNode.WBSParentID = item.ParentID;
                    versionNode.VersionID = this.ID;
                    versionNode.S_W_WBS_Version = this;
                    versionNode.ModifyState = "Add";
                    var rbsInfo = rbsList.Where(c => c.WBSID == item.ID).ToList();
                    versionNode.RBSInfo = JsonHelper.ToJson(rbsInfo);
                    this.S_W_WBS_Version_Node.Add(versionNode);
                }
            }
        }

        const string compareFields = "Name,Code,WBSValue,WBSType,SortIndex,FullID,WBSDeptID,ChargeUserID,PlanStartDate,PlanEndDate,PlanWorkLoad,Weight";
        private bool CompareObject<T, TN>(T wbsNode, TN versionNode)
            where T : BaseModel
            where TN : BaseModel
        {
            var result = true;
            var propertyList = versionNode.GetProperties();
            var wbsProperties = wbsNode.GetProperties();
            foreach (var property in propertyList)
            {
                if (!compareFields.Contains(property.Name)) continue;
                if (property.Name == "ID" || property.Name == "SortIndex") continue;
                if (wbsProperties.Count(c => c.Name == property.Name) == 0) continue;
                if (!String.IsNullOrEmpty(wbsNode.GetPropertyString(property.Name)) && String.IsNullOrEmpty(versionNode.GetPropertyString(property.Name)))
                {
                    result = false; break;
                }
                else if (String.IsNullOrEmpty(wbsNode.GetPropertyString(property.Name)) && !String.IsNullOrEmpty(versionNode.GetPropertyString(property.Name)))
                {
                    result = false; break;
                }
                else if (String.IsNullOrEmpty(wbsNode.GetPropertyString(property.Name)) && String.IsNullOrEmpty(versionNode.GetPropertyString(property.Name)))
                {
                    continue;
                }
                else if (!wbsNode.GetPropertyString(property.Name).Equals(versionNode.GetPropertyString(property.Name)))
                {
                    result = false; break;
                }
            }
            return result;
        }

        static bool CompareDataRow(DataRow wbsRow, DataRow versionRow)
        {
            var result = true;
            foreach (DataColumn column in versionRow.Table.Columns)
            {
                if (!compareFields.Contains(column.ColumnName)) continue;
                if (column.ColumnName == "ID" || column.ColumnName == "SortIndex") continue;
                if (!wbsRow.Table.Columns.Contains(column.ColumnName)) continue;
                if (!(wbsRow[column.ColumnName] == null || wbsRow[column.ColumnName] == DBNull.Value)
                    && (versionRow[column.ColumnName] == null || versionRow[column.ColumnName] == DBNull.Value)
                    )
                {
                    result = false; break;
                }
                else if ((wbsRow[column.ColumnName] == null || wbsRow[column.ColumnName] == DBNull.Value)
                    && !(versionRow[column.ColumnName] == null || versionRow[column.ColumnName] == DBNull.Value))
                {
                    result = false; break;
                }
                else if (String.IsNullOrEmpty(wbsRow[column.ColumnName].ToString()) && !String.IsNullOrEmpty(versionRow[column.ColumnName].ToString()))
                {
                    result = false; break;
                }
                else if (!String.IsNullOrEmpty(wbsRow[column.ColumnName].ToString()) && String.IsNullOrEmpty(versionRow[column.ColumnName].ToString()))
                {
                    result = false; break;
                }
                else if (String.IsNullOrEmpty(wbsRow[column.ColumnName].ToString()) && String.IsNullOrEmpty(versionRow[column.ColumnName].ToString()))
                {
                    continue;
                }
                else if (!wbsRow[column.ColumnName].ToString().Equals(versionRow[column.ColumnName].ToString()))
                {
                    result = false; break;
                }
            }
            return result;
        }
    }
}
