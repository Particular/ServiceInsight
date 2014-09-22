namespace Particular.ServiceInsight.FunctionalTests.Framework
{
    using System.Diagnostics;
    using TestStack.White;

    public class ApplicationLauncher
    {
        public static Application LaunchApplication(string processPath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = processPath,
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