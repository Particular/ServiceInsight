using System.IO;
using System.IO.IsolatedStorage;

namespace Particular.ServiceInsight.Desktop.Core.Settings
{
    public class IsolatedStorageSettingsStore : JsonSettingsStoreBase
    {
        private const IsolatedStorageScope Scope = IsolatedStorageScope.User     |
                                                   IsolatedStorageScope.Assembly | 
                                                   IsolatedStorageScope.Domain   | 
                                                   IsolatedStorageScope.Roaming;

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
                        return new StreamReader(stream).ReadToEnd();
                }
            }

            return null;
        }
    }
}
