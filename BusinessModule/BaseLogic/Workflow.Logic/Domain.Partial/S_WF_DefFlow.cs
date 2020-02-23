using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Exceptions;
using System.Transactions;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_DefFlow
    {
        #region 克隆流程定义

        /// <summary>
        /// 克隆流程定义
        /// </summary>
        /// <returns></returns>
        public S_WF_DefFlow Clone()
        {

            var newDefFlow = new S_WF_DefFlow();
            try
            {
                FormulaHelper.UpdateModel(newDefFlow, this);
                newDefFlow.ID = GetInsIDs(this.ID);

                //环节
                foreach (var step in this.S_WF_DefStep)
                {
                    S_WF_DefStep newStep = new S_WF_DefStep();
                    FormulaHelper.UpdateModel(newStep, step);
                    newStep.ID = GetInsIDs(step.ID);
                    newStep.DefFlowID = GetInsIDs(step.DefFlowID);
                    newStep.WaitingStepIDs = GetInsIDs(step.WaitingStepIDs);
                    newStep.SubFormID = GetInsIDs(step.SubFormID);
                    newDefFlow.S_WF_DefStep.Add(newStep);
                }

                //路由
                foreach (var routing in this.S_WF_DefRouting)
                {
                    S_WF_DefRouting newRouting = new S_WF_DefRouting();
                    FormulaHelper.UpdateModel(newRouting, routing);
                    newRouting.EndID = GetInsIDs(routing.EndID);
                    newRouting.ID = GetInsIDs(routing.ID);
                    newRouting.DefFlowID = GetInsIDs(routing.DefFlowID);
                    newRouting.DefStepID = GetInsIDs(routing.DefStepID);
                    newRouting.UserIDsFromStep = GetInsIDs(routing.UserIDsFromStep);
                    newRouting.UserIDsFromStepSender = GetInsIDs(routing.UserIDsFromStepSender);
                    newRouting.UserIDsFromStepExec = GetInsIDs(routing.UserIDsFromStepExec);
                    newRouting.MsgUserIDsFromStep = GetInsIDs(routing.MsgUserIDsFromStep);
                    newRouting.MsgUserIDsFromStepSender = GetInsIDs(routing.MsgUserIDsFromStepSender);
                    newRouting.MsgUserIDsFromStepExec = GetInsIDs(routing.MsgUserIDsFromStepExec);

                    newDefFlow.S_WF_DefRouting.Add(newRouting);
                }

                //子表单
                foreach (var subForm in this.S_WF_DefSubForm)
                {
                    S_WF_DefSubForm newSubForm = new S_WF_DefSubForm();
                    FormulaHelper.UpdateModel(newSubForm, subForm);
                    newSubForm.ID = GetInsIDs(newSubForm.ID);
                    newSubForm.DefFlowID = GetInsIDs(newSubForm.DefFlowID);
                    newDefFlow.S_WF_DefSubForm.Add(newSubForm);
                }


                //更新流程图
                string flowView = newDefFlow.ViewConfig;
                foreach (string id in idDic.Keys)
                {
                    flowView = flowView.Replace(id, idDic[id]);
                }
                newDefFlow.ViewConfig = flowView;

                return newDefFlow;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 获取流程定义实例

        private static readonly object padlock = new object();

        public S_WF_InsDefFlow GetInsDefFlow()
        {
            try
            {
                if (this.AlreadyReleased != "1" || !this.ModifyTime.HasValue)
                    throw new FlowException("流程定义尚未发布！");

                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                var insDefFlow = entities.S_WF_InsDefFlow.Where(c => c.DefFlowID == this.ID && c.ModifyTime.HasValue && c.ModifyTime.Value == this.ModifyTime.Value).SingleOrDefault();

                if (insDefFlow == null)
                {
                    lock (padlock)
                    {
                        insDefFlow = entities.S_WF_InsDefFlow.Where(c => c.DefFlowID == this.ID && c.ModifyTime.HasValue && c.ModifyTime.Value == this.ModifyTime.Value).SingleOrDefault();
                        if (insDefFlow == null)
                        {
                            insDefFlow = new S_WF_InsDefFlow();
                            FormulaHelper.UpdateModel(insDefFlow, this);
                            insDefFlow.ID = GetInsIDs(this.ID);
                            insDefFlow.DefFlowID = this.ID;

                            foreach (var step in this.S_WF_DefStep)
                            {
                                S_WF_InsDefStep insStep = new S_WF_InsDefStep();
                                FormulaHelper.UpdateModel(insStep, step);
                                insStep.ID = GetInsIDs(step.ID);
                                insStep.DefStepID = step.ID;
                                insStep.InsDefFlowID = GetInsIDs(step.DefFlowID);
                                insStep.WaitingStepIDs = GetInsIDs(step.WaitingStepIDs);
                                insStep.EmptyToStep = GetInsIDs(step.EmptyToStep);

                                insDefFlow.S_WF_InsDefStep.Add(insStep);
                            }

                            foreach (var routing in this.S_WF_DefRouting)
                            {
                                S_WF_InsDefRouting insRouting = new S_WF_InsDefRouting();
                                FormulaHelper.UpdateModel(insRouting, routing);
                                insRouting.EndID = GetInsIDs(routing.EndID);
                                insRouting.ID = GetInsIDs(routing.ID);
                                insRouting.DefRoutingID = routing.ID;
                                insRouting.InsDefFlowID = GetInsIDs(routing.DefFlowID);
                                insRouting.InsDefStepID = GetInsIDs(routing.DefStepID);
                                insRouting.UserIDsFromStep = GetInsIDs(routing.UserIDsFromStep);
                                insRouting.UserIDsFromStepSender = GetInsIDs(routing.UserIDsFromStepSender);
                                insRouting.UserIDsFromStepExec = GetInsIDs(routing.UserIDsFromStepExec);
                                insRouting.MsgUserIDsFromStep = GetInsIDs(routing.MsgUserIDsFromStep);
                                insRouting.MsgUserIDsFromStepSender = GetInsIDs(routing.MsgUserIDsFromStepSender);
                                insRouting.MsgUserIDsFromStepExec = GetInsIDs(routing.MsgUserIDsFromStepExec);

                                var insStep = insDefFlow.S_WF_InsDefStep.Where(c => c.ID == insRouting.InsDefStepID).SingleOrDefault();
                                insStep.S_WF_InsDefRouting.Add(insRouting);
                            }

                            //更新流程图
                            string flowView = insDefFlow.ViewConfig;
                            foreach (string id in idDic.Keys)
                            {
                                flowView = flowView.Replace(id, idDic[id]);
                            }
                            insDefFlow.ViewConfig = flowView;

                            entities.S_WF_InsDefFlow.Add(insDefFlow);
                            entities.SaveChanges();

                        }
                    }
                }
                return insDefFlow;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }


        #endregion

        #region 私有方法

        private Dictionary<string, string> _idDic = null;
        private Dictionary<string, string> idDic
        {
            get
            {
                if (_idDic == null)
                {
                    int index = 1;
                    _idDic = new Dictionary<string, string>();
                    _idDic.Add(this.ID, FormulaHelper.CreateGuid(index++));
                    foreach (var step in this.S_WF_DefStep)
                        _idDic.Add(step.ID, FormulaHelper.CreateGuid(index++));
                    foreach (var routing in this.S_WF_DefRouting)
                        _idDic.Add(routing.ID, FormulaHelper.CreateGuid(index++));
                    foreach (var subForm in this.S_WF_DefSubForm)
                        _idDic.Add(subForm.ID, FormulaHelper.CreateGuid(index++));
                }
                return _idDic;
            }
        }

        private string GetInsIDs(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return ids;

            var arr = ids.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = idDic[arr[i]];
            }

            return string.Join(",", arr);
        }

        #endregion
    }
}
