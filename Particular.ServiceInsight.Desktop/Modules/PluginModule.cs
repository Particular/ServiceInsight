using System;
using System.IO;
using Autofac;
using Particular.ServiceInsight.Desktop.Startup;

namespace Particular.ServiceInsight.Desktop.Modules
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