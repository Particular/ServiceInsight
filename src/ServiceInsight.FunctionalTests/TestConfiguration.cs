namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using TestStack.White;
    using TestStack.White.Factory;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;

    public class TestConfiguration          
    {
        const int ExtraIdleWaitSecs = 3;
        const string ApplicationProcess = "Particular.ServiceInsight.exe";

        public Application LaunchApplication()
        {
            var codebase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codebase);
            var path = Uri.UnescapeDataString(uri.Path);
            var app = Path.Combine(Path.GetDirectoryName(path), ApplicationProcess);
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = app,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "Execute?ResetLayout=True",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            return Application.AttachOrLaunch(processStartInfo);
        }

        public Window GetMainWindow(Application application)
        {
            WaitForApplicationIdle(application);
            var mainWindow = application.GetWindow(SearchCriteria.ByAutomationId("ShellWindow"), InitializeOption.NoCache);

            return mainWindow;
        }

        void WaitForApplicationIdle(Application application)
        {
            application.WaitWhileBusy();
            Thread.Sleep(ExtraIdleWaitSecs * 1000);
        }
    }
}