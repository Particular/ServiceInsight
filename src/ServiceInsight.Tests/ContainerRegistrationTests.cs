namespace Particular.ServiceInsight.Tests
{
    using System;
    using Autofac;
    using Desktop.Shell;
    using Desktop.Startup;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class ContainerRegistrationTests
    {
        AppBootstrapper Bootstrapper;
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
            Container.Dispose();
        }

        [Test]
        public void should_resolve_the_shell()
        {
            Should.NotThrow(() => Container.Resolve<ShellViewModel>());
        }
    }

    public class TestableAppBootstrapper : AppBootstrapper
    {
        protected override void PrepareApplication()
        {

        }

        protected override bool TryHandleException(Exception exception)
        {
            return false;
        }
    }
}