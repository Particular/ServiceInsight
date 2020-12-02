﻿namespace ServiceInsight.Framework.Modules
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Autofac;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Settings;

    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrySettingsStore>().Named<ISettingsStorage>("Registry").WithParameter("registryKey", @"Software\ParticularSoftware\").SingleInstance();
            builder.RegisterType<AppDataSettingsStore>().Named<ISettingsStorage>("AppDataStore").SingleInstance();
            builder.RegisterType<SettingProviderProxy>().As<ISettingsProvider>().SingleInstance();

            base.Load(builder);
        }

        class SettingProviderProxy : Settings.SettingsProvider
        {
            IContainer container;

            public SettingProviderProxy(IContainer container)
            {
                this.container = container;
            }

            protected override T LoadSettings<T>(IList<SettingDescriptor> metadata)
            {
                var storageProvider = typeof(T).GetAttributes<SettingsProviderAttribute>(false).FirstOrDefault();
                if (storageProvider != null)
                {
                    var storageName = storageProvider.ProviderTypeName;
                    var storage = container.ResolveNamed<ISettingsStorage>(storageName);

                    return storage.Load<T>(GetKey<T>(), metadata);
                }

                return base.LoadSettings<T>(metadata);
            }
        }
    }
}