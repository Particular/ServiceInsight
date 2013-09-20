using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;

namespace NServiceBus.Profiler.FunctionalTests.Infrastructure
{
    public class ProfilerConfiguration
    {
        private const int ExtraIdleWaitSecs = 5;
        private const string MainWindowTitle = "ServiceInsight for NServiceBus";
        private const string ApplicationProcess = "Particular.ServiceInsight.exe";

        public Application LaunchApplication()
        {
            var app = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ApplicationProcess);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = app,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            return Application.AttachOrLaunch(processStartInfo);
        }

        public IMainWindow GetMainWindow(Application application)
        {
            WaitForApplicationIdle(application);
            var mainWindow = application.GetWindow(SearchCriteria.ByAutomationId("ShellWindow"), InitializeOption.WithCache);
            var mainWindowAdapter = new ProxyGenerator().CreateInterfaceProxyWithoutTarget<IMainWindow>(new ForwardIfExistsInterceptor(mainWindow));
            return mainWindowAdapter;
        }

        private void WaitForApplicationIdle(Application application)
        {
            application.WaitWhileBusy();
            Thread.Sleep(ExtraIdleWaitSecs * 1000);
        }
    }
}