using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Logic.Domain;

namespace Project.Logic
{
    public class GanttNode
    {
        public GanttNode()
        { }

        public GanttNode(S_P_MileStone item, int len)
        {
            this.UID = item.ID;
            this.Name = item.Name.Length >= 5 ? item.Name.Substring(0, 5) + ".." : item.Name;
            this.DisplayName = item.Name;
            this.Start = Convert.ToDateTime(item.PlanFinishDate);
            this.Finish = Convert.ToDateTime(item.PlanFinishDate).AddDays(len);
            if (item.ShowStatus == MileStoneShowStatus.NormalFinish.ToString())
            {
                this.FactFinishData = item.FactFinishDate == null ? "" : Convert.ToDateTime(item.FactFinishDate).ToShortDateString();
                this.PercentComplete = 0;  //甘特图控件要求，当正常完成时，标记里程碑进度为0，显示正常完成图标
            }
            else if (item.ShowStatus == MileStoneShowStatus.DelayFinish.ToString())
            {
                this.FactFinishData = item.FactFinishDate == null ? "" : Convert.ToDateTime(item.FactFinishDate).ToShortDateString();
                this.PercentComplete = 50; //甘特图控件要求，当延期完成时，标记里程碑进度为50，显示延期完成图标
            }
            else if (item.ShowStatus == MileStoneShowStatus.DelayUndone.ToString())
                this.PercentComplete = 100;  //甘特图控件要求，当延期未完成时，标记里程碑进度为50，显示延期未完成图标
            else
                this.PercentComplete = 10; //其它情况均认为正常进行中
            this.State = item.State;
        }

        /// <summary>
        /// 必须 String 唯一标识符
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 必须 String 任务名称 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 必须 Date 开始日期 
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 必须 Date 完成日期 
        /// </summary>
        public DateTime Finish { get; set; }

        /// <summary>
        /// 必须 Number 工期
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 必须 Number(0~100) 完成百分比
        /// </summary>
        public float PercentComplete { get; set; }

        ///// <summary>
        ///// 前置任务
        ///// </summary>
        //public string PredecessorLink { get; set; }

        /// <summary>
        /// Number(0或1) 摘要任务 当一个任务下有子任务的时候，这个任务就是摘要任务，当Summary为1时，此任务会两端黑色箭头显示 
        /// </summary>
        public int Summary { get; set; }

        /// <summary>
        /// Number(0或1) 关键任务 当Critical为1时，显示一个红色条形图 
        /// </summary>
        public int Critical { get; set; }


        /// <summary>
        /// Number(0或1) 里程碑 当Milestone为1时，显示一个菱形图标。
        /// </summary>
        public int Milestone { get; set; }

        public string MileStoneState
        {
            get;
            set;
        }

        public string DisplayName
        { get; set; }

        public string FactFinishData
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string ProjectInfoName { get; set; }
    }
}
