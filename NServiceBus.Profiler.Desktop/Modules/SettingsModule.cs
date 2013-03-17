using System.Collections.Generic;
using System.Configuration;
using Autofac;
using ExceptionHandler.Extensions;
using NServiceBus.Profiler.Core.Settings;
using SettingsProvider = NServiceBus.Profiler.Core.Settings.SettingsProvider;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrySettingsStore>().Named<ISettingsStorage>("Registry").WithParameter("registryKey", @"Software\ParticularSoftware\ServiceBus");
            builder.RegisterType<IsolatedStorageSettingsStore>().Named<ISettingsStorage>("IsolatedStore");
            builder.RegisterType<SettingProviderProxy>().As<ISettingsProvider>();

            base.Load(builder);
        }

        private class SettingProviderProxy : SettingsProvider
        {
            private readonly IContainer _container;

            public SettingProviderProxy(IContainer container)
            {
                _container = container;
            }

            protected override Dictionary<string, string> LoadSettings<T>()
            {
                var storageProvider = typeof(T).GetAttribute<SettingsProviderAttribute>();
                if (storageProvider != null)
                {
                    var storageName = storageProvider.ProviderTypeName;
                    var storage = _container.ResolveNamed<ISettingsStorage>(storageName);

                    return storage.Load(GetKey<T>());
                }

                return base.LoadSettings<T>();
            }
        }
    }
}