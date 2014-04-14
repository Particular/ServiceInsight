using System;
using System.IO;
using Autofac;
using NServiceBus.Profiler.Desktop.Startup;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class PluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            builder.RegisterPluginModules(pluginFolder, "*.dll");
        }
    }
}