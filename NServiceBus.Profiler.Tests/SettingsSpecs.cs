using System.Collections.Generic;
using Machine.Specifications;
using NServiceBus.Profiler.Core.Settings;

namespace NServiceBus.Profiler.Tests.Settings
{
    [Subject("setting storage")]
    public class with_registry_storage
    {
        protected static RegistrySettingsStore RegistryStore;
        protected static Dictionary<string, string> SettingsToSave;
        protected static Dictionary<string, string> LoadedSettings;
        protected static string StorageKey = @"Software\ParticularSoftware";

        Establish context = () =>
        {
            RegistryStore = new RegistrySettingsStore(StorageKey);
        };

        Because of = () =>
        {
            SettingsToSave = new Dictionary<string, string>
            {
                {"MyStringSetting", "Some Value"}
            };

            RegistryStore.Save("Management", SettingsToSave);

            LoadedSettings = RegistryStore.Load("Management");
        };

        It should_load_back_the_settings_correctly = () => LoadedSettings.ShouldNotBeNull();
        It should_have_loaded_the_values = () => LoadedSettings["Management.MyStringSetting"].ShouldEqual("Some Value");
    }
}