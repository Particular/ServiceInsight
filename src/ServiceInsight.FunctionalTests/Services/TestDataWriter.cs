namespace ServiceInsight.FunctionalTests.Services
{
    using System;
    using System.IO;
    using ServiceInsight.ExtensionMethods;
    using Newtonsoft.Json;

    public class TestDataWriter
    {
        public static void Write(string scenario, object data)
        {
            EnsureTestDataFolderExists();

            var content = JsonConvert.SerializeObject(data);
            var scenarioFile = Path.Combine(TestDataFolder, scenario + ".json");

            if (File.Exists(scenarioFile))
            {
                TryDelete(scenarioFile);
            }

            File.WriteAllText(scenarioFile, content);
        }

        public static void DeleteAll()
        {
            EnsureTestDataFolderExists();

            var folder = new DirectoryInfo(TestDataFolder);
            var files = folder.GetFiles();

            files.ForEach(f => TryDelete(f.FullName));
        }

        static void TryDelete(string scenarioFile)
        {
            try
            {
                File.Delete(scenarioFile);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not delete existing scenario file at {0}", scenarioFile), ex);
            }
        }

        static void EnsureTestDataFolderExists()
        {
            if (!Directory.Exists(TestDataFolder))
            {
                Directory.CreateDirectory(TestDataFolder);
            }
        }

        private static string TestDataFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"); }
        }
    }
}