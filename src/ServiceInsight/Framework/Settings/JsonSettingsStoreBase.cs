namespace ServiceInsight.Framework.Settings
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public abstract class JsonSettingsStoreBase : ISettingsStorage
    {
        public void Save<T>(string key, T settings)
        {
            var filename = key + ".settings";
            var serialized = JsonConvert.SerializeObject(settings);

            WriteTextFile(filename, serialized);
        }

        protected abstract void WriteTextFile(string filename, string fileContents);

        public T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new()
        {
            var filename = key + ".settings";

            var readTextFile = ReadTextFile(filename);
            if (!string.IsNullOrEmpty(readTextFile))
            {
                var serialized = JsonConvert.DeserializeObject<T>(readTextFile);
                return serialized;
            }

            return new T();
        }

        protected abstract string ReadTextFile(string filename);

        public virtual bool HasSettings(string key) => true;
    }
}