using System.Configuration;
using Autofac;
using ExceptionHandler;
using ExceptionHandler.IssueReporter.BitBucket;
using ExceptionHandler.Settings;
using ExceptionHandler.Wpf;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Startup;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class ShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //TODO: Consolidate
            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.IsViewOrViewModel())
                   .AsImplementedInterfaces()
                   .AsSelf();

            builder.RegisterSource(new SettingsSource());
            builder.RegisterType<ExplorerView>().As<IExplorerView>().SingleInstance();
            builder.RegisterType<ExplorerViewModel>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>().SingleInstance();
            builder.RegisterType<ShellView>().As<IShellView>().SingleInstance();
            builder.RegisterType<MenuManager>().As<IMenuManager>().SingleInstance();
            builder.RegisterType<StatusBarManager>().As<IStatusBarManager>().SingleInstance();
            builder.RegisterType<ScreenFactory>().As<IScreenFactory>().SingleInstance();
            builder.RegisterType<DefaultExceptionHandler>().As<IExceptionHandler>().SingleInstance();
            builder.RegisterType<ExceptionViewModel>().As<IExceptionViewModel>().InstancePerDependency();
            builder.RegisterType<WpfClipboard>().As<IClipboard>().SingleInstance();
            builder.RegisterType<BitBucketIssueTracker>().As<IIssueTracker>();
            builder.RegisterType<BitBucketIssueRelayApi>();
            builder.RegisterType<SimpleSettingsReader>().As<ISettingsReader>().WithParameter(TypedParameter.From(ConfigurationManager.AppSettings));
        }
    }
}