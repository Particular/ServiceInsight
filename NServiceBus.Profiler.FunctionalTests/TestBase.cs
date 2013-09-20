using System;
using Autofac;
using Castle.Core.Logging;
using NServiceBus.Profiler.Desktop.Modules;
using NServiceBus.Profiler.FunctionalTests.Infrastructure;
using NServiceBus.Profiler.FunctionalTests.Screens;
using NUnit.Framework;
using TestStack.White;
using TestStack.White.Configuration;
using TestStack.White.InputDevices;

namespace NServiceBus.Profiler.FunctionalTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected readonly ILogger Logger = CoreAppXmlConfiguration.Instance.LoggerFactory.Create(typeof(TestBase));
        protected IMainWindow MainWindow;
        protected Application Application;
        protected IContainer Container;

        public ICoreConfiguration Configuration { get; set; }
        public IMouse Mouse { get; set; }
        public IKeyboard Keyboard { get; set; }

        [TestFixtureSetUp]
        public void InitializeApplication()
        {
            try
            {
                var configuration = new ProfilerConfiguration();
                
                Application = configuration.LaunchApplication();
                MainWindow = configuration.GetMainWindow(Application);
                Container = CreateContainer();
                OnApplicationInitialized();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch application and get main window", ex);
                TryCloseApplication();
                throw;
            }
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MainWindow);
            builder.RegisterAssemblyTypes(GetType().Assembly)
                   .Where(c => c.IsAssignableTo<ProfilerElement>())
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .PropertiesAutowired();

            builder.RegisterInstance(TestStack.White.InputDevices.Keyboard.Instance).As<IKeyboard>();
            builder.RegisterInstance(TestStack.White.InputDevices.Mouse.Instance).As<IMouse>();
            builder.RegisterInstance(CoreAppXmlConfiguration.Instance).As<ICoreConfiguration>();

            builder.RegisterModule<CoreModule>();

            return builder.Build();
        }

        protected void OnApplicationInitialized()
        {
            Container.InjectProperties(this);
            Configuration.WaitBasedOnHourGlass = false;
            Configuration.InProc = true;
            Configuration.FindWindowTimeout = 60000;
        }

        [TestFixtureTearDown]
        public void CleanUpTest()
        {
            Container.Dispose();
            Application.KillAndSaveState();
        }

        private void TryCloseApplication()
        {
            try
            {
                if (Application != null && !Application.HasExited)
                {
                    Application.KillAndSaveState();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not close application gracefully.", ex);
            }
        }
    }
}