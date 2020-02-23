using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using System.Text.RegularExpressions;
using Formula.Exceptions;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsDefFlow
    {
        #region 创建流程

        /// <summary>
        /// 创建流程
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public S_WF_InsFlow CreateFlow(string formInstanceID, string userID, string userName)
        {
            try
            {
                S_WF_InsFlow insFlow = new S_WF_InsFlow();
                insFlow.S_WF_InsDefFlow = this;
                insFlow.ID = FormulaHelper.CreateGuid();
                insFlow.CreateTime = DateTime.Now;
                insFlow.CreateUserID = userID;
                insFlow.CreateUserName = userName;
                insFlow.FormInstanceID = formInstanceID;
                insFlow.Status = FlowTaskStatus.Processing.ToString();


                insFlow.FlowName = insFlow.GetInsName(this.FlowNameTemplete);
                insFlow.FlowCategory = insFlow.GetInsName(this.FlowCategorytemplete);
                insFlow.FlowSubCategory = insFlow.GetInsName(this.FlowSubCategoryTemplete);

                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                entities.S_WF_InsFlow.Add(insFlow);

                //初始化流程变量
                if (!string.IsNullOrEmpty(this.InitVariable))
                {
                    foreach (string str in this.InitVariable.Split(','))
                    {
                        if (string.IsNullOrWhiteSpace(str))
                            continue;

                        string strKey = str.Split('=')[0];
                        string strValue = str.Contains('=') ? str.Split('=')[1] : "";

                        strValue = strValue.Trim('\'');

                        var variable = new S_WF_InsVariable();
                        variable.ID = FormulaHelper.CreateGuid();
                        variable.VariableName = strKey;
                        variable.VariableValue = strValue;
                        insFlow.S_WF_InsVariable.Add(variable);
                    }
                }
                return insFlow;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion
    }
}
