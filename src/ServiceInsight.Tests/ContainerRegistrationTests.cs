namespace ServiceInsight.Tests
{
    using System.Threading;
    using Autofac;
    using ServiceInsight.Shell;
    using ServiceInsight.Startup;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using ReactiveUI.Testing;
    using Shouldly;

    [TestFixture, Apartment(ApartmentState.STA)]
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
        [Explicit]
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