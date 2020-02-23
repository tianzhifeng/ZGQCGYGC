using EPC.Logic.Domain;
using Formula.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Formula;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class ToDoListController : EPCController
    {       
        public JsonResult GetList(string EngineeringInfoID)
        {
            var engineeringInfo = entities.Set<S_I_Engineering>().Find(EngineeringInfoID);
            if (engineeringInfo == null)
                throw new BusinessException("无法找到对应工程数据");
            
            if (engineeringInfo.Mode == null)
                throw new BusinessException("未能找到该工程的模式定义");

            string defineID = engineeringInfo.Mode.ToDoListDefine;
            List<Lane> lanes = new List<Lane>();
            if (!string.IsNullOrEmpty(defineID))
            {
                var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
                var define = dbContext.Set<S_T_ToDoListDefine>().Find(defineID);
                if (define == null)
                    throw new BusinessException("无法找到对应代办事项定义");

                var defineNodes = define.S_T_ToDoListDefineNode;
                var categories = defineNodes.Where(a => a.Type == "Category");
                
                #region 读取数据
                DateTime? minCreateDate = null;
                DateTime? maxCreateDate = null;
                foreach (var cat in categories)
                {
                    Lane lane = new Lane();
                    var taskList = defineNodes.Where(a => a.ParentID == cat.ID);
                    List<GNode> nodes = new List<GNode>();
                    foreach (var task in taskList)
                    {
                        GNode node = new GNode();
                        var toDo = entities.Set<S_T_ToDoList>().FirstOrDefault(a => a.EngineeringInfoID == EngineeringInfoID && a.DefineNodeID == task.ID);
                        node.BotTitle = task.Name;
                        if(toDo != null)
                        {
                            node.CreateDate = toDo.CreateDate;                                            
                            if(toDo.FinishTime != null)
                            {
                                node.State= "Excute";
                                node.ExcuteDate = toDo.FinishTime;
                            }
                            else
                            {
                                node.State = "Create";
                            }
                            minCreateDate = MinDate(minCreateDate, toDo.CreateDate.Date);
                            maxCreateDate = MaxDate(maxCreateDate, toDo.CreateDate.Date);
                        }
                        node.SortIndex = task.SortIndex;
                        node.Draw = true;
                        nodes.Add(node);
                    }
                    lane.Title = cat.Name;
                    lane.Nodes= nodes;
                    lanes.Add(lane);
                }
                #endregion 

                #region 数据处理
                //按时间分割排
                if(minCreateDate != null && maxCreateDate != null)
                {
                    DateTime index = minCreateDate.Value;
                    while (index <= maxCreateDate)
                    {                        
                        //遍历所有泳道,对节点进行重拍
                        foreach(var lan in lanes)
                        {
                            //该时间节点上的任务节点的draw为true
                            var nodes = lan.Nodes.Where(a => a.CreateDate != null && a.CreateDate.Date == index).ToList();                            
                            //如果没有,必须加一个绘制用节点
                            if(nodes.Count == 0)
                            {
                                GNode node = new GNode();
                                node.CreateDate = index;
                                node.Draw = false;
                                lan.Nodes.Add(node);
                            }
                            //移除多余节点,保留一个,并将描述合并到一个节点
                            else
                            {
                                var singNode = nodes[0];
                                nodes.Remove(singNode);
                                lan.Nodes.RemoveAll(a => nodes.Contains(a));

                                if (nodes.Count > 0)
                                {
                                    singNode.MultiBotTitle = singNode.BotTitle;
                                    singNode.BotTitle += "...";
                                }

                                foreach(var node in nodes)
                                {
                                    singNode.MultiBotTitle += "/" + node.BotTitle;
                                }
                                singNode.Draw = true;
                            }
                        }
                        index = index.AddDays(1);
                    }
                }               
                lanes.ForEach(a => a.Nodes = a.Nodes.OrderBy(b => b.CreateDate).ThenBy(c => c.SortIndex).ToList());//先由实例CreateDate排序,再由模板的SortIndex排序
                #endregion
            }

            return Json(new { Lanes = lanes,
                MaxNodeCount = lanes.Max(a => a.Nodes.Count),
                InstanceNodeCount = lanes.Max(a => a.Nodes.Count(b => b.CreateDate != DateTime.MaxValue)) });
        }

        private DateTime MinDate(DateTime? date1,DateTime date2)
        {
            if (date1 == null)
                return date2;

            if (date1.Value > date2)
                return date2;
            else
                return date1.Value;
        }

        private DateTime MaxDate(DateTime? date1, DateTime date2)
        {
            if (date1 == null)
                return date2;

            if (date1.Value > date2)
                return date1.Value;
            else
                return date2;
        }

        private class Lane
        {
            public Lane()
            {
                Nodes = new List<GNode>();
            }
            public string Title { get; set; }
            public List<GNode> Nodes { get; set; }
        }

        private class GNode {
            public GNode()
            {
                SortIndex = 0;
                Draw = false;
                CreateDate = DateTime.MaxValue;
            }
            public DateTime CreateDate { get; set; }
            public DateTime? ExcuteDate { get; set; }
            public string BotTitle { get; set; }
            public string MultiBotTitle { get; set; }//多个标题重叠
            public string InnerTopTitle { get { return CreateDate == DateTime.MaxValue ? "-" : CreateDate.ToString("yy/MM/dd"); } }
            public string InnerBotTitle { get { return ExcuteDate == null ? "-" : ExcuteDate.Value.ToString("yy/MM/dd"); } }
            public string State { get; set; }
            public bool Draw { get; set; }

            public int SortIndex { get; set; }
        }
    }
}
