namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using Autofac;
    using Castle.Core.Logging;
    using Desktop.Modules;
    using Infrastructure;
    using NUnit.Framework;
    using Parts;
    using ServiceControlStub;
    using TestStack.White;
    using TestStack.White.Configuration;
    using TestStack.White.InputDevices;
    using TestStack.White.UIItems.WindowItems;

    [TestFixture]
    public abstract class TestBase
    {
        protected LoggerLevel TestLoggerLevel = LoggerLevel.Debug;
        protected Window MainWindow;
        protected Application Application;
        protected ProfilerConfiguration Configuration;
        protected IContainer Container;
        protected Waiter Wait;
        //protected NameGenerator NameGenerator;
        protected ServiceControl ServiceControlStub;
        protected ILogger Logger;

        public ICoreConfiguration CoreConfiguration { get; set; }
        public IMouse Mouse { get; set; }
        public IKeyboard Keyboard { get; set; }

        [TestFixtureSetUp]
        public void InitializeApplication()
        {
            try
            {
                Logger = new WhiteDefaultLoggerFactory(TestLoggerLevel).Create(typeof (TestBase));
                Wait = new Waiter();
                //NameGenerator = new NameGenerator();
                ServiceControlStub = ServiceControl.Start();
                Configuration = new ProfilerConfiguration();
                Application = Configuration.LaunchApplication();
                MainWindow = Configuration.GetMainWindow(Application);
                Container = CreateContainer();
                OnApplicationInitialized();
                OnSetupUI();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch application and get main window", ex);
                TryCloseApplication();
                throw;
            }
        }

        protected virtual void OnSetupUI()
        {
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
        }

        [TestFixtureTearDown]
        public void CleanUpTest()
        {
            ServiceControlStub.Stop();
            Container.Dispose();
            TryCloseApplication();
        }

        private void TryCloseApplication()
        {
            try
            {
                if (IsApplicationRunning())
                {
                    Application.ApplicationSession.Save();
//                    if (!Debugger.IsAttached)
//                    {
//                        Application.Kill();
//                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not close application gracefully.", ex);
            }
        }

        private bool IsApplicationRunning()
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
    }
}