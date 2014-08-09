namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Autofac;
    using Castle.Core.Logging;
    using NUnit.Framework;
    using Services;
    using TestStack.White;
    using TestStack.White.Configuration;
    using TestStack.White.Factory;
    using TestStack.White.InputDevices;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;
    using UI.Parts;

    [TestFixture]
    public abstract class UITest
    {
        public Window MainWindow { get; set; }

        public Application Application { get; set; }
        
        public IContainer Container { get; set; }
        
        public ICoreConfiguration CoreConfiguration { get; set; }

        public IMouse Mouse { get; set; }

        public IKeyboard Keyboard { get; set; }

        public Waiter Wait { get; set; }

        public ILoggerFactory LoggerFactory { get; set; }

        public ILogger Logger { get; set; }

        [TestFixtureSetUp]
        public void InitializeApplication()
        {
            try
            {
                ConfigureLogging();
                Container = CreateContainer();
                OnApplicationInitializing();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch application and get main window", ex);
                TryCaptureScreenshot();
                TryCloseApplication();
                throw;
            }
        }

        protected void TryCaptureScreenshot()
        {
            try
            {
                var screenshot = Desktop.CaptureScreenshot();
                var testName = GetTestName();
                var screenshotFile = Path.Combine(TestConfiguration.ScreenshotFolder, string.Format(@"{0}.png", testName));
                
                screenshot.Save(screenshotFile);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to get the screenshot. Reason is: " + ex.GetBaseException().Message);
            }
        }

        protected string GetTestName()
        {
            var name = TestContext.CurrentContext.Test.FullName;
            var nameParts = name.Split('.');

            return string.Format("{0}.{1}", nameParts[nameParts.Length - 2], nameParts[nameParts.Length - 1]);
        }

        void ConfigureLogging()
        {
            LoggerFactory = new NLogFactory();
            CoreAppXmlConfiguration.Instance.LoggerFactory = LoggerFactory;
        }

        IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(TestAssembly)
                   .Where(c => c.IsAssignableTo<IAutoRegister>())
                   .AsSelf()
                   .PropertiesAutowired();
            builder.RegisterAssemblyTypes(TestAssembly)
                   .Where(c => c.IsAssignableTo<UIElement>())
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .PropertiesAutowired();

            builder.RegisterInstance(TestStack.White.InputDevices.Keyboard.Instance).As<IKeyboard>();
            builder.RegisterInstance(TestStack.White.InputDevices.Mouse.Instance).As<IMouse>();
            builder.RegisterInstance(CoreAppXmlConfiguration.Instance).As<ICoreConfiguration>();
            builder.Register(c => ApplicationLauncher.LaunchApplication());
            builder.Register(c => GetMainWindow(c.Resolve<Application>()));
            builder.RegisterInstance(LoggerFactory);
            builder.Register(c => c.Resolve<ILoggerFactory>().Create(GetType()));

            return builder.Build();
        }

        void OnApplicationInitializing()
        {
            Container.InjectProperties(this);
            CoreConfiguration.WaitBasedOnHourGlass = false;
            CoreConfiguration.InProc = true;
            CoreConfiguration.BusyTimeout = 5000;
            CoreConfiguration.FindWindowTimeout = 60000;
            MainWindow.TitleBar.MaximizeButton.Click();
            TestDataWriter.DeleteAll();
            OnApplicationInitialized();
        }

        protected virtual void OnApplicationInitialized()
        {
        }

        [TestFixtureTearDown]
        public void CleanUpTest()
        {
            Container.Dispose();
            TryCloseApplication();
        }

        void CaptureScreenIfTestFailed()
        {
            if (TestContext.CurrentContext.Result.Status == TestStatus.Failed)
            {
                TryCaptureScreenshot();
            }
        }

        [TearDown]
        public void CleanUp()
        {
            CaptureScreenIfTestFailed();
            if (!Debugger.IsAttached) TestDataWriter.DeleteAll();
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

        Window GetMainWindow(Application app)
        {
            var mainWindow = app.GetWindow(SearchCriteria.ByAutomationId("ShellWindow"), InitializeOption.NoCache);
            return mainWindow;
        }

        protected void WaitWhileBusy()
        {
            if(Application != null) Application.WaitWhileBusy();
            Thread.Sleep(TestConfiguration.ExtraIdleWaitSecs * 1000);
        }

        private Assembly TestAssembly
        {
            get { return GetType().Assembly; }
        }
    }
}