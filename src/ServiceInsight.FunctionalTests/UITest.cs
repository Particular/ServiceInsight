namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using System.Reflection;
    using Autofac;
    using Castle.Core.Logging;
    using Desktop.Framework.Modules;
    using NUnit.Framework;
    using Services;
    using TestStack.White;
    using TestStack.White.Configuration;
    using TestStack.White.InputDevices;
    using TestStack.White.UIItems.WindowItems;
    using UI.Parts;

    [TestFixture]
    public abstract class UITest
    {
        protected LoggerLevel TestLoggerLevel = LoggerLevel.Debug;
        protected Window MainWindow;
        protected Application Application;
        protected TestConfiguration Configuration;
        protected IContainer Container;
        protected ILogger Logger;

        public ICoreConfiguration CoreConfiguration { get; set; }

        public IMouse Mouse { get; set; }

        public IKeyboard Keyboard { get; set; }

        public Waiter Wait { get; set; }

        [TestFixtureSetUp]
        public void InitializeApplication()
        {
            try
            {
                Logger = new WhiteDefaultLoggerFactory(TestLoggerLevel).Create(typeof(UITest));
                Configuration = new TestConfiguration();
                Application = Configuration.LaunchApplication();
                MainWindow = Configuration.GetMainWindow(Application);
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

        IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MainWindow);
            builder.RegisterAssemblyTypes(TestAssembly)
                   .Where(c => c.IsAssignableTo<IAutoRegister>())
                   .AsSelf()
                   .PropertiesAutowired();
            builder.RegisterAssemblyTypes(TestAssembly)
                   .Where(c => c.IsAssignableTo<ProfilerElement>())
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .PropertiesAutowired();

            builder.RegisterInstance(TestStack.White.InputDevices.Keyboard.Instance).As<IKeyboard>();
            builder.RegisterInstance(TestStack.White.InputDevices.Mouse.Instance).As<IMouse>();
            builder.RegisterInstance(CoreAppXmlConfiguration.Instance).As<ICoreConfiguration>();
            builder.RegisterInstance(Application);
            builder.RegisterInstance(Logger);

            builder.RegisterModule<CoreModule>();

            return builder.Build();
        }

        protected void OnApplicationInitialized()
        {
            Container.InjectProperties(this);
            CoreConfiguration.WaitBasedOnHourGlass = false;
            CoreConfiguration.InProc = true;
            CoreConfiguration.FindWindowTimeout = 60000;
            TestDataWriter.DeleteAll();
        }

        [TestFixtureTearDown]
        public void CleanUpTest()
        {
            Container.Dispose();
            TryCloseApplication();
        }

        void TryCloseApplication()
        {
            try
            {
                if (IsApplicationRunning())
                {
                    Application.ApplicationSession.Save();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not close application gracefully.", ex);
            }
        }

        bool IsApplicationRunning()
        {
            try
            {
                return Application != null && !Application.HasExited;
            }
            catch
            {
                return false;
            }
        }

        private Assembly TestAssembly
        {
            get { return GetType().Assembly; }
        }
    }
}