namespace Particular.ServiceInsight.Tests
{
    using Autofac;
    using Desktop.Shell;
    using Desktop.Startup;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using ReactiveUI.Testing;
    using Shouldly;

    [TestFixture, RequiresSTA]
    public class ContainerRegistrationTests
    {
        TestableAppBootstrapper Bootstrapper;
        IContainer Container;

        [SetUp]
        public void TestInitialize()
        {
            Bootstrapper = new TestableAppBootstrapper();
            Container = Bootstrapper.GetContainer();
        }

        [TearDown]
        public void TestCleanup()
        {
            Container?.Dispose();
        }

        [Test]
        [Ignore]
        public void should_resolve_the_shell()
        {
            new TestScheduler().With(sched =>
            {
                Should.NotThrow(() => Container.Resolve<ShellViewModel>());
            });
        }
    }

    public class TestableAppBootstrapper : AppBootstrapper
    {
        protected override void PrepareApplication()
        {
        }

        public IContainer GetContainer()
        {
            return container;
        }
    }
}