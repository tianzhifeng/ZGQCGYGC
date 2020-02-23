using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using NBlock.Core.Logging;
using Ninject;
using Topshelf;

namespace Quartz.Server
{
    /// <summary>
    /// The server's main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        public static void Main()
        {
            var kernel = new Ninject.StandardKernel();
            // 加载所有定义在dll中的注册模块
            var files = "NBlock*.dll,*Models.dll,*Services.dll,";
            kernel.Load((files).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            // 注册服务
            kernel.Bind<ILogService>().To<Log4NetService>().InSingletonScope();

            // 设置依赖注入
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            // 配置Log服务
            ServiceLocator.Current.GetInstance<ILogService>().Configure();

            // 启动服务
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            HostFactory.Run(x =>
                                {
                                    x.RunAsLocalSystem();

                                    x.SetDescription(Configuration.ServiceDescription);
                                    x.SetDisplayName(Configuration.ServiceDisplayName);
                                    x.SetServiceName(Configuration.ServiceName);

                                    x.Service(factory =>
                                                  {
                                                      QuartzServer server = QuartzServerFactory.CreateServer();
                                                      server.Initialize();
                                                      return server;
                                                  });
                                });
        }


        /// <summary>
        /// 依赖注入定位类
        /// </summary>
        public class NinjectServiceLocator : ServiceLocatorImplBase
        {
            public IKernel Kernel { get; set; }

            public NinjectServiceLocator(IKernel kernel)
            {
                this.Kernel = kernel;
            }
            protected override object DoGetInstance(Type serviceType, string key)
            {
                return Kernel.Get(serviceType, key);
            }

            protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
            {
                return Kernel.GetAll(serviceType);
            }
        }
    }
}