using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using System.Data;

namespace Project.Logic.Domain
{
    public partial class S_I_ProjectInfo
    {
        partial void onBuildComplete()
        {
            if (this.WBSRoot == null) throw new Formula.Exceptions.BusinessException("WBS根节点未创建，无法增加项目负责人角色");
            if (!string.IsNullOrEmpty(this.ChargeUserID))
                this.WBSRoot.SetUsers(ProjectRole.DesignManager.ToString(), this.ChargeUserID.Split(','));
        }

        /// <summary>
        /// 根据新的专业信息同步项目关联的专业以及同步新增专业的OBS结构
        /// </summary>
        /// <param name="addMajorList"></param>
        /// <param name="majorUserList"></param>
        /// <param name="newMajors"></param>
        /// <param name="isSynchOBS"></param>
        public void SynchMajorData(string newMajors)
        {
            if(String.IsNullOrEmpty(newMajors)) return;
            var majorEnumTable = BaseConfigFO.GetWBSEnum(WBSNodeType.Major);
            //反写项目的专业信息
            var major = newMajors.Split(',').Select(
                    c => new
                    {
                        Name = majorEnumTable.Select("value='" + c + "'")[0]["text"],
                        Value = c,
                        NameEN = majorEnumTable.Select("value='" + c + "'")[0]["NameEN"],
                    }
                );
            this.Major = JsonHelper.ToJson(major);
        }

        /// <summary>
        /// 将项目的参与专业转换为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable TranslateMajorDataToDataTable()
        {
            //获取项目参与专业信息
            var majorValues = this.MajorValue;
            var majorTexts = this.MajorText;

            var result = new DataTable();
            result.TableName = "ProjectMajor";
            result.Columns.Add("text");
            result.Columns.Add("value");
            var aMajorValue = majorValues.Split(',');
            var aMajorText = majorTexts.Split(',');

            for (int i = 0; i < aMajorValue.Length; i++)
            {
                var majorValue = aMajorValue[i];
                var majorText = aMajorText[i];
                var row = result.NewRow();
                row["text"] = majorText;
                row["value"] = majorValue;
                result.Rows.Add(row);
            }

            return result;

        }

        /// <summary>
        /// 将项目的专业节点转换为DataTable
        /// </summary>
        /// <param name="ignoreMajorCode"></param>
        public DataTable TranslateMajorNodeToDataTable(string ignoreMajorCode = null)
        {
            var result = new DataTable();
            result.TableName = "ProjectMajor";
            result.Columns.Add("text");
            result.Columns.Add("value");

            //获取专业节点
            var majorNodeList = this.GetMajorList().Select(d => new { value = d.WBSValue, text = d.Name }).Distinct();
            if (!string.IsNullOrEmpty(ignoreMajorCode))
            {
                var aIgnoreMajorCode = ignoreMajorCode.Split(',');
                majorNodeList = majorNodeList.Where(c => !aIgnoreMajorCode.Contains(c.value)).ToList();
            }
            foreach (var item in majorNodeList)
            {
                var row = result.NewRow();
                row["text"] = item.text;
                row["value"] = item.value;
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
