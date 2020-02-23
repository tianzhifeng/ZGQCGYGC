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
				logger.Error("�����ʼ��ʧ��:" + e.Message, e);
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
                logger.Info("���ڽ��г�ʼ��");
                ISchedulerFactory sf = new StdSchedulerFactory();
                IScheduler sched = sf.GetScheduler();

                logger.Info("��ʼ����ɣ�����׼���������...");


                //�û�ͬ����6�㵽����10��ÿʮ��ִ��һ�Σ�
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

                logger.Info(string.Format("-----�û�ͬ��----{0}����{1}��ʱ�����У����һ��ڱ��ʽ{2}�ظ�", job.Key, ft, trigger.CronExpressionString));

	            scheduler.Start();
	        }
	        catch (Exception ex)
	        {
	            logger.Fatal(string.Format("�������ʧ��: {0}", ex.Message), ex);
	            throw;
	        }

            logger.Info("������ȳɹ�");
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
                logger.Error(string.Format("����������ҵ: {0}", ex.Message), ex);
                throw;
            }

            logger.Info("����������ҵ�ɹ�");
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
