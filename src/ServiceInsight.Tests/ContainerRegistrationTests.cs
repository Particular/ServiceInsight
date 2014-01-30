using System;
using Autofac;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Startup;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class ContainerRegistrationTests
    {
        private AppBootstrapper Bootstrapper;
        private IContainer Container;

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
            Should.NotThrow(() => Container.Resolve<IShellViewModel>());
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