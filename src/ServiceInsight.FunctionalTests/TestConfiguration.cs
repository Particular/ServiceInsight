namespace ServiceInsight.FunctionalTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public class TestConfiguration
    {
        public const int ExtraIdleWaitSecs = 3;

        const string ApplicationProcess = "ServiceInsight.exe";

        static TestConfiguration()
        {
            ApplicationPath = TestContext.CurrentContext.TestDirectory;
            LogsFolder = Path.Combine(ApplicationPath, "Logs");
            ScreenshotFolder = Path.Combine(ApplicationPath, "Screenshots");

            if (!Directory.Exists(LogsFolder)) Directory.CreateDirectory(LogsFolder);
            if (!Directory.Exists(ScreenshotFolder)) Directory.CreateDirectory(ScreenshotFolder);
        }

        static string GetApplicationPath()
        {
            var codebase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codebase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string ScreenshotFolder { get; private set; }

        public static string LogsFolder { get; private set; }

        public static string ApplicationPath { get; private set; }

        public static string ApplicationExecutablePath
        {
            get { return Path.Combine(ApplicationPath, ApplicationProcess); }
        }
    }
}