namespace Particular.ServiceInsight.FunctionalTests
{
    using System.Diagnostics;
    using System.IO;
    using TestStack.White;

    public class ApplicationLauncher
    {
        const string ApplicationProcess = "Particular.ServiceInsight.exe";

        public static Application LaunchApplication()
        {
            var app = Path.Combine(TestConfiguration.ApplicationPath, ApplicationProcess);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = app,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "Execute?ResetLayout=True", //to restore to default layout for each test
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            return Application.AttachOrLaunch(processStartInfo);
        }
    }
}