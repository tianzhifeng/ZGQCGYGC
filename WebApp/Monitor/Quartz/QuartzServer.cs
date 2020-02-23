using System;
using System.Configuration;
using Monitor.Quartz.Jobs;
using Common.Logging;
using Quartz.Impl;
using Topshelf;

namespace Quartz.Server
{
	/// <summary>
	/// The main server logic.
	/// </summary>
	public class QuartzServer : ServiceControl, IQuartzServer
	{
		private readonly ILog logger;
		private ISchedulerFactory schedulerFactory;
		private IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
	    public QuartzServer()
	    {
	        logger = LogManager.GetLogger(GetType());
	    }

	    /// <summary>
		/// Initializes the instance of the <see cref="QuartzServer"/> class.
		/// </summary>
		public virtual void Initialize()
		{
			try
			{				
				schedulerFactory = CreateSchedulerFactory();
				scheduler = GetScheduler();
			}
			catch (Exception e)
			{
				logger.Error("服务初始化失败:" + e.Message, e);
				throw;
			}
		}

        /// <summary>
        /// Gets the scheduler with which this server should operate with.
        /// </summary>
        /// <returns></returns>
	    protected virtual IScheduler GetScheduler()
	    {
	        return schedulerFactory.GetScheduler();
	    }

        /// <summary>
        /// Returns the current scheduler instance (usually created in <see cref="Initialize" />
        /// using the <see cref="GetScheduler" /> method).
        /// </summary>
	    protected virtual IScheduler Scheduler
	    {
	        get { return scheduler; }
	    }

	    /// <summary>
        /// Creates the scheduler factory that will be the factory
        /// for all schedulers on this instance.
        /// </summary>
        /// <returns></returns>
	    protected virtual ISchedulerFactory CreateSchedulerFactory()
	    {
	        return new StdSchedulerFactory();
	    }

	    /// <summary>
		/// Starts this instance, delegates to scheduler.
		/// </summary>
		public virtual void Start()
		{
	        try
	        {
                logger.Info("正在进行初始化");
                ISchedulerFactory sf = new StdSchedulerFactory();
                IScheduler sched = sf.GetScheduler();

                logger.Info("初始化完成，正在准备任务调度...");


                //用户同步（6点到晚上10点每十钟执行一次）
                IJobDetail job = JobBuilder.Create<UserJob>()
                    .WithIdentity("userjob", "sync")
                    .Build();

                //var usercron = ConfigurationManager.AppSettings["UserCron"];
                //if (string.IsNullOrEmpty(usercron)) usercron = "0 0/10 6-22 * * ?";

                ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                    .WithIdentity("trigger1", "sync")
                    .WithCronSchedule("0 0/1 6-22 * * ?")
                    .Build();

                DateTimeOffset ft = sched.ScheduleJob(job, trigger);

                logger.Info(string.Format("-----用户同步----{0}将在{1}后时间运行，并且基于表达式{2}重复", job.Key, ft, trigger.CronExpressionString));

	            scheduler.Start();
	        }
	        catch (Exception ex)
	        {
	            logger.Fatal(string.Format("任务调度失败: {0}", ex.Message), ex);
	            throw;
	        }

            logger.Info("任务调度成功");
		}

		/// <summary>
		/// Stops this instance, delegates to scheduler.
		/// </summary>
		public virtual void Stop()
		{
            try
            {
                scheduler.Shutdown(true);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("结束任务作业: {0}", ex.Message), ex);
                throw;
            }

            logger.Info("结束任务作业成功");
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
	    public virtual void Dispose()
	    {
	        // no-op for now
	    }

        /// <summary>
        /// Pauses all activity in scheduler.
        /// </summary>
	    public virtual void Pause()
	    {
	        scheduler.PauseAll();
	    }

        /// <summary>
        /// Resumes all activity in server.
        /// </summary>
	    public void Resume()
	    {
	        scheduler.ResumeAll();
	    }

	    /// <summary>
        /// TopShelf's method delegated to <see cref="Start()"/>.
	    /// </summary>
	    public bool Start(HostControl hostControl)
	    {
	        Start();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Stop()"/>.
        /// </summary>
        public bool Stop(HostControl hostControl)
	    {
	        Stop();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Pause()"/>.
        /// </summary>
        public bool Pause(HostControl hostControl)
	    {
	        Pause();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Resume()"/>.
        /// </summary>
        public bool Continue(HostControl hostControl)
	    {
	        Resume();
	        return true;
	    }
	}
}
