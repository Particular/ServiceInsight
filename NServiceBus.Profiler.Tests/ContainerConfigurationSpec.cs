using System;
using Autofac;
using Machine.Specifications;
using Particular.ServiceInsight.Desktop.Shell;
using Particular.ServiceInsight.Desktop.Startup;

namespace NServiceBus.Profiler.Tests.Container
{
    [Subject("container")]
    public abstract class with_a_populated_container
    {
        protected static AppBootstrapper Bootstrapper;
        protected static IContainer Container;

        Establish context = () =>
        {
            Bootstrapper = new TestableAppBootstrapper();
            Container = Bootstrapper.GetContainer();
        };
    }

    [Ignore("Need a better way to make sure application is composed correctly")]
    public class resolving_components : with_a_populated_container
    {
        protected static IShellViewModel Shell;

        Because of = () => Shell = Container.Resolve<IShellViewModel>();

        It should_have_a_reference_to_the_shell = () => Shell.ShouldNotBeNull();
    }

    public class TestableAppBootstrapper : AppBootstrapper
    {
        protected override void PrepareApplication()
        {
        }

        protected override void TryDisplayUnhandledException(Exception exception)
        {
        }
    }
}