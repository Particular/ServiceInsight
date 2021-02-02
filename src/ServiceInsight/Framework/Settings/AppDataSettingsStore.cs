namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;

    public class AppDataSettingsStore : JsonSettingsStoreBase
    {
        const IsolatedStorageScope Scope = IsolatedStorageScope.User |
                    IsolatedStorageScope.Assembly |
                    IsolatedStorageScope.Domain;

        protected override void WriteTextFile(string filename, string fileContents)
        {
            using (var stream = new FileStream(GetFullPath(filename), FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(fileContents);
                writer.Flush();
                stream.SetLength(stream.Position);
            }
        }

        protected override string ReadTextFile(string filename)
        {
            var path = GetFullPath(filename);
            if (File.Exists(path))
            {
                return ReadFromAppData(path);
            }

            return ReadFromIsolatedStorage(filename);
        }

        string ReadFromAppData(string settingFile)
        {
            using (var stream = new FileStream(settingFile, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        string ReadFromIsolatedStorage(string filename)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                if (isoStore.FileExists(filename))
                {
                    using (var stream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore))
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return null;
        }

        string GetFullPath(string settingFile)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var serviceInsight = Path.Combine(appData, "Particular Software", "ServiceInsight");

            if (!Directory.Exists(serviceInsight))
            {
                Directory.CreateDirectory(serviceInsight);
            }

            var dataFilePath = Path.Combine(serviceInsight, settingFile);

            return dataFilePath;
        }

        public override bool HasSettings(string key)
        {
            var filename = key + ".settings";
            var dataFile = GetFullPath(filename);

            if (File.Exists(dataFile))
            {
                return true;
            }

            //For backward compatibility with Isolated Storage
            using (var isoStore = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                return isoStore.FileExists(filename);
            }
        }
    }
}