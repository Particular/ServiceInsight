namespace ServiceInsight.Framework.Settings
{
    using System.IO;
    using System.IO.IsolatedStorage;

    public class IsolatedStorageSettingsStore : JsonSettingsStoreBase
    {
        const IsolatedStorageScope Scope = IsolatedStorageScope.User |
                                                   IsolatedStorageScope.Assembly |
                                                   IsolatedStorageScope.Domain;

        protected override void WriteTextFile(string filename, string fileContents)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                using (var stream = new IsolatedStorageFileStream(filename, FileMode.Create, isoStore))
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(fileContents);
                    streamWriter.Flush();
                }
            }
        }

        protected override string ReadTextFile(string filename)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                if (isoStore.FileExists(filename))
                {
                    using (var stream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore))
                    {
                        return new StreamReader(stream).ReadToEnd();
                    }
                }
            }

            return null;
        }

        public override bool HasSettings(string key)
        {
            var filename = key + ".settings";

            using (var isoStore = IsolatedStorageFile.GetStore(Scope, null, null))
            {
                return isoStore.FileExists(filename);
            }
        }
    }
}