using System;
using Common.Logging;
using Quartz;

namespace Monitor.Quartz.Jobs
{
    public class UserJob : IJob
    {
        private readonly ILog logger;
        public UserJob()
        {
            logger = LogManager.GetLogger(GetType());
        }

        public virtual void Execute(IJobExecutionContext context)
        {

        }
    }
}
