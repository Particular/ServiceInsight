namespace Particular.ServiceInsight.Desktop.Modules
{
    using System;
    using System.IO;
    using Autofac;
    using Startup;

    public class PluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            builder.RegisterPluginModules(pluginFolder, "*.dll");
        }
    }
}