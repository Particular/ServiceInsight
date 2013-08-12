using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using Caliburn.Core;
using Particular.ServiceInsight.Desktop.Core.Settings;
using SettingsProvider = Particular.ServiceInsight.Desktop.Core.Settings.SettingsProvider;

namespace Particular.ServiceInsightDesktop.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrySettingsStore>().Named<ISettingsStorage>("Registry").WithParameter("registryKey", @"Software\ParticularSoftware\ServiceBus").SingleInstance();
            builder.RegisterType<IsolatedStorageSettingsStore>().Named<ISettingsStorage>("IsolatedStore").SingleInstance();
            builder.RegisterType<SettingProviderProxy>().As<ISettingsProvider>().SingleInstance();

            base.Load(builder);
        }

        private class SettingProviderProxy : Particular.ServiceInsight.Desktop.Core.Settings.SettingsProvider
        {
            private readonly IContainer _container;

            public SettingProviderProxy(IContainer container)
            {
                _container = container;
            }

            protected override T LoadSettings<T>(IList<SettingDescriptor> metadata)
            {
                var storageProvider = typeof(T).GetAttributes<SettingsProviderAttribute>(false).FirstOrDefault();
                if (storageProvider != null)
                {
                    var storageName = storageProvider.ProviderTypeName;
                    var storage = _container.ResolveNamed<ISettingsStorage>(storageName);

                    return storage.Load<T>(GetKey<T>(), metadata);
                }

                return base.LoadSettings<T>(metadata);
            }
        }
    }
}