﻿namespace ServiceInsight.Framework.Modules
{
    using System.Collections.Generic;
    using System.Xml;
    using Akavache;
    using Autofac;
    using Models;
    using Pirac;
    using ServiceControl;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.MessageDecoders;
    using Startup;
    using UI.ScreenManager;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StringContentDecoder>().As<IContentDecoder<string>>();
            builder.RegisterType<XmlContentDecoder>().As<IContentDecoder<XmlDocument>>();
            builder.RegisterType<HeaderContentDecoder>().As<IContentDecoder<IList<HeaderInfo>>>();
            builder.RegisterType<NetworkOperations>().SingleInstance();
            builder.RegisterType<AppLicenseManager>().SingleInstance();
            builder.RegisterType<ServiceControlConnectionProvider>().SingleInstance();
            builder.RegisterType<DefaultServiceControl>().As<IServiceControl>().InstancePerLifetimeScope();
            builder.RegisterInstance(new RxServiceControl(BlobCache.UserAccount)).As<IRxServiceControl>().SingleInstance();
            builder.RegisterType<CommandLineArgParser>().SingleInstance().OnActivating(e => e.Instance.Parse());
            builder.RegisterType<RxEventAggregator>().As<IRxEventAggregator>().SingleInstance();
            builder.RegisterType<WorkNotifier>().As<IWorkNotifier>();
            builder.RegisterType<WindowManagerEx>().As<IWindowManagerEx>();
            builder.RegisterInstance(PiracRunner.WindowManager).As<IWindowManager>();
        }
    }
}