namespace ServiceInsightInstaller.Managed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Caliburn.Micro;
    using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
    using ServiceInsightInstaller.Managed.UI.Shell;

    class CaliburnMicroBootstrapper : BootstrapperBase
    {
        BootstrapperApplication ba;
        IContainer container;

        public CaliburnMicroBootstrapper(BootstrapperApplication ba)
        {
            LogManager.GetLog = t => new WixLog(ba.Engine, t);

            this.ba = ba;

            Initialize();
        }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<EventAggregationAutoSubscriptionModule>();

            builder.RegisterInstance(ba);

            builder.RegisterType<WindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(type => type.Name.EndsWith("ViewModel"))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(type => type.Name.EndsWith("View"))
                .AsSelf()
                .InstancePerDependency();

            container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}